# CBBSimulator Web Migration Roadmap

**.NET 8 Console App -> ASP.NET Core + SignalR + Vue.js 3**

## Table of Contents

1. [Current State Analysis](#1-current-state-analysis)
2. [Target Architecture](#2-target-architecture)
3. [Real-Time Strategy (SignalR)](#3-real-time-strategy-signalr)
4. [Data Layer & Caching](#4-data-layer--caching)
5. [Backend API Design](#5-backend-api-design)
6. [Frontend Design](#6-frontend-design)
7. [Concurrency & Scaling](#7-concurrency--scaling)
8. [Hosting](#8-hosting)
9. [Deployment & CI/CD](#9-deployment--cicd)
10. [Testing Strategy](#10-testing-strategy)
11. [Phased Roadmap](#11-phased-roadmap)
12. [Risks & Open Questions](#12-risks--open-questions)
13. [Milestone Tracker](#13-milestone-tracker)

## 1. Current State Analysis

The simulator is a **.NET 8 console application** that scrapes team statistics from Bart Torvik, then runs possession-level simulations for single games, full seasons, and March Madness tournaments.

### Key Source Files

| File | ~Lines | Responsibility |
|---|---:|---|
| `DataController.cs` | 3,200 | Scraping, data processing, game simulation, season simulation |
| `MarchMadnessController.cs` | 1,000 | Bracket seeding, round-by-round tournament simulation |
| `Simulator2026.cs` | 600 | Season orchestration, scheduling, conference play |
| `DataModels.cs` | 120 | CollegeModel, PossessionResult, MatchupResult, ScheduleGame |

### Core Domain Models

- `CollegeModel` - team stats (AdjO, AdjD, tempo, FT%, 3P%, etc.)
- `PossessionResult` - single-possession outcome (points, event type)
- `MatchupResult` - full game result (scores, possessions, winner)
- `ScheduleGame` - scheduled game with teams, date, conference flag

> **Key challenge:** `DataController.cs` is a 3,200-line monolith mixing I/O, simulation logic, and UI. The migration is primarily a separation-of-concerns refactor that also adds a web layer.

## 2. Target Architecture

Four-project solution: shared core, data access, web host, and Vue SPA client.

```text
CBBSimulator/
+-- CBBSimulator.Core/          Models, simulation engines, interfaces
|   +-- Models/                 CollegeModel, GameEvent, SimulationConfig
|   +-- Engines/                PossessionEngine, GameEngine, TournamentEngine, SeasonEngine
|   +-- Interfaces/             IGameEngine, ITournamentEngine, ISeasonEngine
|
+-- CBBSimulator.Data/          Scrapers, caching, data services
|   +-- Scrapers/               TorkvikScraper, CsvDataLoader
|   +-- Services/               TeamDataService, TeamDataCache
|   +-- Interfaces/             ITeamDataService
|
+-- CBBSimulator.Web/           ASP.NET Core host
|   +-- Hubs/                   GameHub, TournamentHub, SeasonHub
|   +-- Endpoints/              TeamsEndpoints, SimulationEndpoints
|   +-- Services/               DataRefreshService, SimulationWorkerService
|   +-- Program.cs
|
+-- CBBSimulator.Client/        Vue 3 SPA (Vite)
    +-- src/
        +-- views/              HomeView, GameView, TournamentView, SeasonView
        +-- components/         TeamSelector, Scoreboard, PossessionLog, BracketViewer, StandingsTable
        +-- composables/        useSignalR, useGameState
        +-- stores/             teams.ts, game.ts, tournament.ts, season.ts
```

```text
Architecture Diagram

+-------------------+         +-----------------------+
|   Vue.js 3 SPA    |  HTTP   |   ASP.NET Core 8      |
|  (Vite + Pinia)   |<------->|   REST Endpoints      |
|                   |         |                       |
|  useSignalR()     |  WSS    |   SignalR Hubs         |
|  composable       |<=======>|   Game/Tourn/Season   |
+-------------------+         +----------+------------+
                                          |
                         +----------------+----------------+
                         |                |                 |
                 +-------v------+  +------v-------+  +-----v--------+
                 |  GameEngine  |  | TournEngine  |  | SeasonEngine |
                 |  (Core)      |  | (Core)       |  | (Core)       |
                 +--------------+  +--------------+  +--------------+
                         |
                 +-------v--------------+
                 |  TeamDataService      |
                 |  IMemoryCache (1hr)   |
                 |  TorkvikScraper       |
                 +----------------------+
```

## 3. Real-Time Strategy (SignalR)

| Hub | Events (Server -> Client) | Methods (Client -> Server) |
|---|---|---|
| `GameHub` | `PossessionResult`, `ScoreUpdate`, `GameOver` | `StartGame(team1, team2)`, `SetSpeed(ms)` |
| `TournamentHub` | `MatchComplete`, `RoundComplete`, `ChampionCrowned` | `StartTournament(config)`, `SimulateNextRound()` |
| `SeasonHub` | `GameResult`, `StandingsUpdate`, `SeasonComplete` | `StartSeason(config)`, `SimulateWeek()` |

### Group Isolation

Each simulation gets a unique group ID (`Guid`). Clients join via `Groups.AddToGroupAsync`. Events are scoped to the group so multiple users can run independent simulations concurrently.

### Streaming Pattern

```csharp
// Server-side: GameEngine exposes IAsyncEnumerable
public async IAsyncEnumerable<GameEvent> SimulateGame(
    CollegeModel home, CollegeModel away, SimulationConfig config)
{
    while (!gameOver)
    {
        var result = _possessionEngine.RunPossession(offense, defense);
        yield return new GameEvent { Type = "Possession", Data = result };
        await Task.Delay(config.PossessionDelayMs);
    }
    yield return new GameEvent { Type = "GameOver", Data = finalScore };
}

// Hub streams to group
await foreach (var evt in _engine.SimulateGame(home, away, config))
{
    await Clients.Group(groupId).SendAsync(evt.Type, evt.Data);
}
```

## 4. Data Layer & Caching

> **No database needed.** ~360 D1 teams fit comfortably in memory (<1MB serialized). The only external data source is Bart Torvik.

### Two-Layer Cache Strategy

| Layer | Implementation | TTL | Purpose |
|---|---|---|---|
| L1 | `IMemoryCache` | 1 hour | Hot path - team lookups during simulation |
| Background | `DataRefreshService` | Periodic | Proactive refresh before cache expires |

- **Batch FT% pre-fetch:** Scrape all team free throw percentages in a single pass during refresh, avoiding per-team HTTP calls during simulation.
- **Redis:** Only needed if scaling horizontally. Single-server deployment uses in-memory cache exclusively.
- **Fallback:** If Torvik is unreachable, serve stale cache with a warning header. `CsvDataLoader` provides a static fallback dataset.

## 5. Backend API Design

### REST Endpoints (Minimal APIs)

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/teams` | All teams (cached) |
| `GET` | `/api/teams/{name}` | Single team by name |
| `GET` | `/api/teams/search?q=` | Fuzzy search (autocomplete) |
| `GET` | `/api/health` | Health check (cache age, team count) |
| `POST` | `/api/simulate/quick` | Stateless quick sim - returns MatchupResult |

### GameEngine Core Loop

```csharp
public async IAsyncEnumerable<GameEvent> SimulateGame(
    CollegeModel home, CollegeModel away, SimulationConfig config)
{
    var state = new GameState(home, away);

    while (state.PossessionsRemaining > 0)
    {
        var possession = _possessionEngine.Run(state);
        state.Apply(possession);

        yield return GameEvent.Possession(possession);
        yield return GameEvent.ScoreUpdate(state.Score);

        await Task.Delay(config.DelayMs);
    }

    yield return GameEvent.GameOver(state.FinalResult);
}
```

## 6. Frontend Design

### Tech Stack

- **Vue.js 3** with Composition API + `<script setup>`
- **Vite** for dev server and build
- **Pinia** for state management
- **@microsoft/signalr** for real-time communication

### Views & Components

| Views | Key Components |
|---|---|
| `HomeView` | Mode selector, quick stats |
| `GameView` | `TeamSelector`, `Scoreboard`, `PossessionLog` |
| `TournamentView` | `BracketViewer`, round controls |
| `SeasonView` | `StandingsTable`, week navigator |

### SignalR Composable

```typescript
// composables/useSignalR.ts
export function useSignalR(hubUrl: string) {
  const connection = ref<HubConnection | null>(null)
  const connected = ref(false)

  async function start() {
    connection.value = new HubConnectionBuilder()
      .withUrl(hubUrl)
      .withAutomaticReconnect()
      .build()
    await connection.value.start()
    connected.value = true
  }

  function on(event: string, handler: (...args: any[]) => void) {
    connection.value?.on(event, handler)
  }

  async function invoke(method: string, ...args: any[]) {
    return connection.value?.invoke(method, ...args)
  }

  onUnmounted(() => connection.value?.stop())

  return { connection, connected, start, on, invoke }
}
```

### Pinia Stores

- `useTeamStore` - team list, search results, selected teams
- `useGameStore` - current game state, score, possession log
- `useTournamentStore` - bracket state, round results
- `useSeasonStore` - standings, weekly results

## 7. Concurrency & Scaling

### Worker Pool Pattern

```csharp
// Bounded channel prevents runaway memory usage
var channel = Channel.CreateBounded<SimulationRequest>(
    new BoundedChannelOptions(100)
    {
        FullMode = BoundedChannelFullMode.Wait
    });

// SimulationWorkerService reads from channel
protected override async Task ExecuteAsync(CancellationToken ct)
{
    await foreach (var request in _channel.Reader.ReadAllAsync(ct))
    {
        _ = ProcessSimulation(request, ct);  // fire-and-forget per sim
    }
}
```

### Scaling Path

| Stage | Approach | Capacity |
|---|---|---|
| 1. Single Server | In-process channels + IMemoryCache | ~1,000+ concurrent users |
| 2. Vertical | Bigger VM, more worker tasks | ~5,000+ concurrent users |
| 3. Horizontal | Redis backplane for SignalR, shared cache | Effectively unlimited |

## 8. Hosting

> **Recommendation:** Azure App Service B1 (~$13/mo). WebSocket support, easy deployment via GitHub Actions, custom domain + TLS included.

| Option | Cost | Pros | Cons |
|---|---|---|---|
| Azure App Service B1 | ~$13/mo | Native .NET, easy CI/CD, WebSockets | Cold starts on lower tiers |
| Fly.io | ~$5-10/mo | Docker-native, global edge | Less .NET tooling |
| Hetzner VPS | ~$5/mo | Cheapest, full control | Manual ops, no managed TLS |

**Initial deployment:** Serve the Vue SPA as static files from `wwwroot/` in the ASP.NET Core project. No separate hosting needed for the frontend.

## 9. Deployment & CI/CD

### GitHub Actions Pipeline

```yaml
# .github/workflows/deploy.yml
name: Build & Deploy
on:
  push:
    branches: [master]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with: { dotnet-version: '8.0.x' }

      - name: Setup Node
        uses: actions/setup-node@v4
        with: { node-version: '20' }

      - name: Build Vue client
        run: cd src/CBBSimulator.Client && npm ci && npm run build

      - name: Copy dist to wwwroot
        run: cp -r src/CBBSimulator.Client/dist/* src/CBBSimulator.Web/wwwroot/

      - name: Build & Test .NET
        run: dotnet test --configuration Release

      - name: Publish
        run: dotnet publish src/CBBSimulator.Web -c Release -o ./publish

      - name: Deploy to Azure
        uses: azure/webapps-deploy@v3
        with:
          app-name: cbbsimulator
          package: ./publish
```

### Docker Multi-Stage Build

```dockerfile
# Stage 1: Build Vue client
FROM node:20-alpine AS client-build
WORKDIR /app/client
COPY src/CBBSimulator.Client/package*.json ./
RUN npm ci
COPY src/CBBSimulator.Client/ ./
RUN npm run build

# Stage 2: Build .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dotnet-build
WORKDIR /src
COPY . .
COPY --from=client-build /app/client/dist src/CBBSimulator.Web/wwwroot
RUN dotnet publish src/CBBSimulator.Web -c Release -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=dotnet-build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "CBBSimulator.Web.dll"]
```

## 10. Testing Strategy

| Layer | Tool | What to Test |
|---|---|---|
| Unit | xUnit + seeded `Random` | PossessionEngine determinism, score calculations, model validation |
| Integration | `WebApplicationFactory` + SignalR test client | API endpoints, hub connections, cache behavior |
| E2E | Playwright | Full game flow, bracket interaction, team search |
| Resilience | Weekly scheduled job | Scraper still parses Torvik HTML correctly |

> **Scraper fragility:** Torvik has no public API. Schedule a weekly CI job that scrapes a known team and asserts expected fields are present. Alert on failure.

## 11. Phased Roadmap

| Phase | Scope | Duration |
|---|---|---|
| **1. Foundation & Core Extraction** | Create solution structure. Extract models and engine interfaces into `CBBSimulator.Core`. Set up data service layer in `CBBSimulator.Data`. Scaffold Vue client. | 2-3 weeks |
| **2. Web API & SignalR Backend** | Implement REST endpoints, SignalR hubs, background worker service. Port `PossessionEngine` and `GameEngine` logic from `DataController`. | 2 weeks |
| **3. Vue.js Frontend - Game Mode** | Build `TeamSelector`, `Scoreboard`, `PossessionLog`. Wire SignalR for live game simulation. Implement `useSignalR` composable. | 2-3 weeks |
| **4. Tournament & Season Modes** | Port tournament logic from `MarchMadnessController`. Port season logic from `Simulator2026`. Build `BracketViewer` and `StandingsTable`. | 2-3 weeks |
| **5. Polish, Deploy & Harden** | Error handling, loading states, responsive design. CI/CD pipeline. Production deployment. Performance profiling. | 1-2 weeks |
| **6. Future Enhancements** | Spectator mode (shareable URLs), historical data, player-level simulation, custom bracket creation, dark/light theme toggle. | Ongoing |

## 12. Risks & Open Questions

| Risk | Impact | Mitigation |
|---|---|---|
| Torvik HTML structure changes | High - scraper breaks | Weekly CI canary test; CSV fallback dataset |
| Rate limiting by Torvik | Medium - data refresh fails | Aggressive caching; batch requests; respect `robots.txt` |
| Simulation logic bugs after extraction | High - wrong results | Seeded-random golden tests comparing old vs. new output |
| SignalR connection limits | Low - unlikely at current scale | Bounded channel; group isolation; scale-out plan ready |
| Scope creep | Medium - delays launch | Strict phase gates; ship game mode first, iterate |

## 13. Milestone Tracker

### Overall Progress

- **Complete:** 15
- **In Progress:** 0
- **Not Started:** 10
- **Total:** 25
- **Progress:** 60%

### Latest Push Update

- **Milestone impact:** Milestone 15 moved to **Complete**
- **Files:** `src/CBBSimulator.Client/src/components/TeamSelector.vue`, `src/CBBSimulator.Web/Endpoints/TeamsEndpoints.cs`
- **Summary:** Polished the TeamSelector autocomplete: full keyboard navigation (arrow keys, Enter, Escape), click-outside dismissal, race-condition guard (token-based stale-response rejection), loading and "no results" states, ARIA roles (combobox/listbox). Trimmed `/api/teams/search` response from full `CollegeModel` (30+ fields) to the same lean DTO used by `/api/teams` (Rank, Name, Conference, W/L, conf W/L) — ~80% smaller payload, no leaked simulation-internal stats.
- **Next step:** Milestone 16 (Implement live Scoreboard - Real-time score updates via SignalR)

### Milestones

| # | Milestone | Status | Details |
|---:|---|---|---|
| **Phase 1 - Foundation & Core Extraction** ||||
| 1 | Project skeleton scaffolded | Complete | Solution structure, all projects created, references configured, Vue scaffolded |
| 2 | Data models extracted to Core | Complete | CollegeModel, PossessionResult, MatchupResult, ScheduleGame, GameEvent, SimulationConfig |
| 3 | Simulation engine interfaces defined | Complete | IGameEngine, PossessionEngine, GameEngine, TournamentEngine, SeasonEngine stubs |
| 4 | Data service layer created | Complete | ITeamDataService, TeamDataService, TorkvikScraper, CsvDataLoader, TeamDataCache stubs |
| 5 | Web API endpoints stubbed | Complete | TeamsEndpoints, SimulationEndpoints, health check |
| 6 | SignalR hubs stubbed | Complete | GameHub, TournamentHub, SeasonHub with group management |
| 7 | Background services created | Complete | DataRefreshService, SimulationWorkerService with bounded Channel |
| 8 | Vue.js SPA scaffolded | Complete | Router, Pinia stores, SignalR composable, all views and components stubbed |
| 9 | Build and tests passing | Complete | 5 tests pass, solution builds cleanly |
| **Phase 2 - Web API & SignalR Backend** ||||
| 10 | Port PossessionEngine logic | Complete | Ported RunPossession from DataController, refactored for testability with injected Random |
| 11 | Port GameEngine logic | Complete | Ported full game flow with period lifecycle events, clock/score streaming, halftime foul reset, late-game logic, overtime support, and final MatchupResult emission with deterministic tests |
| 12 | Port data scrapers | Complete | Implemented CsvDataLoader (CSV-based fallback dataset) and updated TeamDataCache; TorkvikScraper refactored |
| 13 | Implement DataRefreshService | Complete | Hourly CSV refresh with FT% loaded inline; added /api/health endpoint exposing team count, FTP coverage, last refresh, and cache age |
| 14 | Wire GameHub to GameEngine | Complete | GameHub enqueues SimulationRequest; worker resolves IGameEngine + IHubContext, streams GameEvents to SignalR group with one method per event type (PossessionResult, ScoreUpdate, GameOver, etc.) |
| **Phase 3 - Vue.js Frontend - Game Mode** ||||
| 15 | Implement TeamSelector with search | Complete | Debounced autocomplete with keyboard nav, click-outside dismiss, race-condition guard, loading/empty states, ARIA roles; search endpoint trimmed to lean DTO |
| 16 | Implement live Scoreboard | Not Started | Real-time score updates via SignalR |
| 17 | Implement PossessionLog | Not Started | Scrolling play-by-play feed |
| **Phase 4 - Tournament & Season Modes** ||||
| 18 | Port TournamentEngine logic | Not Started | Extract from MarchMadnessController |
| 19 | Build BracketViewer component | Not Started | SVG/CSS bracket visualization |
| 20 | Port SeasonEngine logic | Not Started | Extract from Simulator2026 |
| 21 | Build StandingsTable component | Not Started | Sortable standings with conference filter |
| **Phase 5 - Polish, Deploy & Harden** ||||
| 22 | Spectator mode | Not Started | Shareable URLs for watching same simulation |
| 23 | CI/CD pipeline | Not Started | GitHub Actions build/test/deploy |
| 24 | Production deployment | Not Started | Deploy to Azure App Service or alternative |
| 25 | E2E tests | Not Started | Playwright tests for critical user flows |

---

CBBSimulator Web Migration Roadmap - March 2026  
ASP.NET Core 8 - SignalR - Vue.js 3 - Vite - Pinia - xUnit - Playwright - Docker - GitHub Actions
