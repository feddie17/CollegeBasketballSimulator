using System.Runtime.CompilerServices;
using CBBSimulator.Core.Configuration;
using CBBSimulator.Core.Models;

namespace CBBSimulator.Core.Simulation;

public class TournamentGameInfo
{
    public required string AwayTeam { get; init; }
    public required string HomeTeam { get; init; }
    public required string Round { get; init; }
    public string Region { get; init; } = "";
    public int GameNumber { get; init; }
}

public abstract class TournamentEvent
{
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public class TournamentBracketBuiltEvent : TournamentEvent
{
    public required TournamentBracket Bracket { get; init; }
}

public class TournamentGameStartedEvent : TournamentEvent
{
    public required TournamentGameInfo GameInfo { get; init; }
}

public class TournamentGameCompletedEvent : TournamentEvent
{
    public required MatchupResult Result { get; init; }
    public required string Round { get; init; }
    public string Region { get; init; } = "";
}

public class TournamentRoundCompletedEvent : TournamentEvent
{
    public required string Round { get; init; }
    public required List<MatchupResult> Results { get; init; }
}

public class TournamentCompletedEvent : TournamentEvent
{
    public required string Champion { get; init; }
    public required List<MatchupResult> AllResults { get; init; }
}

public class TournamentEngine
{
    private readonly GameEngine _gameEngine;

    private static readonly string[] RegionNames = ["South", "East", "West", "Midwest"];

    private static readonly (int High, int Low)[] FirstRoundMatchups =
    [
        (1, 16), (8, 9), (5, 12), (4, 13),
        (6, 11), (3, 14), (7, 10), (2, 15)
    ];

    public TournamentEngine(SimulationConfig config, Random? random = null)
    {
        _gameEngine = new GameEngine(config, random);
    }

    public static TournamentBracket BuildBracket(List<CollegeModel> rankedTeams)
    {
        if (rankedTeams.Count < 64)
            throw new ArgumentException($"Need at least 64 teams, got {rankedTeams.Count}");

        var regions = new List<TournamentRegion>();
        for (int r = 0; r < 4; r++)
            regions.Add(new TournamentRegion { Name = RegionNames[r] });

        int maxSeedLine = rankedTeams.Count >= 68 ? 15 : 16;

        for (int seedLine = 0; seedLine < maxSeedLine; seedLine++)
        {
            bool reverse = seedLine % 2 == 1;
            for (int r = 0; r < 4; r++)
            {
                int teamIndex = seedLine * 4 + r;
                if (teamIndex >= rankedTeams.Count) break;

                int regionIndex = reverse ? (3 - r) : r;
                regions[regionIndex].Seeds.Add(new TournamentSeed
                {
                    Seed = seedLine + 1,
                    TeamName = rankedTeams[teamIndex].Name
                });
            }
        }

        var playIns = new List<PlayInMatchup>();
        if (rankedTeams.Count >= 68)
        {
            for (int i = 0; i < 4; i++)
            {
                playIns.Add(new PlayInMatchup
                {
                    Team1 = rankedTeams[60 + i].Name,
                    Team2 = rankedTeams[67 - i].Name,
                    TargetRegion = RegionNames[i],
                    TargetSeed = 16
                });
            }
        }

        return new TournamentBracket
        {
            Regions = regions,
            PlayInGames = playIns
        };
    }

    public IAsyncEnumerable<TournamentEvent> SimulateTournamentAsync(
        List<CollegeModel> rankedTeams,
        SimSpeed speed = SimSpeed.Medium,
        CancellationToken ct = default)
    {
        var bracket = BuildBracket(rankedTeams);
        var teamLookup = rankedTeams.ToDictionary(t => t.Name);
        return SimulateTournamentAsync(bracket, teamLookup, speed, ct);
    }

    public async IAsyncEnumerable<TournamentEvent> SimulateTournamentAsync(
        TournamentBracket bracket,
        Dictionary<string, CollegeModel> teamLookup,
        SimSpeed speed = SimSpeed.Medium,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        int delayMs = (int)speed;
        var allResults = new List<MatchupResult>();
        int gameNumber = 0;

        var regionSeeds = bracket.Regions.ToDictionary(
            r => r.Name,
            r => r.Seeds.ToDictionary(s => s.Seed, s => s.TeamName));

        yield return new TournamentBracketBuiltEvent { Bracket = bracket };

        // --- First Four (play-in games) ---
        if (bracket.PlayInGames.Count > 0)
        {
            var roundResults = new List<MatchupResult>();

            foreach (var playIn in bracket.PlayInGames)
            {
                ct.ThrowIfCancellationRequested();
                gameNumber++;

                var team1 = Resolve(playIn.Team1, teamLookup);
                var team2 = Resolve(playIn.Team2, teamLookup);

                yield return new TournamentGameStartedEvent
                {
                    GameInfo = new TournamentGameInfo
                    {
                        AwayTeam = team1.Name,
                        HomeTeam = team2.Name,
                        Round = "First Four",
                        Region = playIn.TargetRegion,
                        GameNumber = gameNumber
                    }
                };

                var result = await QuickSimAsync(team1, team2, ct);
                roundResults.Add(result);
                allResults.Add(result);

                yield return new TournamentGameCompletedEvent
                {
                    Result = result,
                    Round = "First Four",
                    Region = playIn.TargetRegion
                };

                regionSeeds[playIn.TargetRegion][playIn.TargetSeed] = result.Winner;

                if (delayMs > 0) await Task.Delay(delayMs, ct);
            }

            yield return new TournamentRoundCompletedEvent
            {
                Round = "First Four",
                Results = roundResults
            };
        }

        // --- Round of 64 ---
        var regionWinners = new Dictionary<string, List<string>>();
        {
            var roundResults = new List<MatchupResult>();

            foreach (var region in bracket.Regions)
            {
                regionWinners[region.Name] = new List<string>();
                var seeds = regionSeeds[region.Name];

                foreach (var (high, low) in FirstRoundMatchups)
                {
                    ct.ThrowIfCancellationRequested();
                    gameNumber++;

                    var team1 = Resolve(seeds[high], teamLookup);
                    var team2 = Resolve(seeds[low], teamLookup);

                    yield return new TournamentGameStartedEvent
                    {
                        GameInfo = new TournamentGameInfo
                        {
                            AwayTeam = team1.Name,
                            HomeTeam = team2.Name,
                            Round = "Round of 64",
                            Region = region.Name,
                            GameNumber = gameNumber
                        }
                    };

                    var result = await QuickSimAsync(team1, team2, ct);
                    roundResults.Add(result);
                    allResults.Add(result);
                    regionWinners[region.Name].Add(result.Winner);

                    yield return new TournamentGameCompletedEvent
                    {
                        Result = result,
                        Round = "Round of 64",
                        Region = region.Name
                    };

                    if (delayMs > 0) await Task.Delay(delayMs, ct);
                }
            }

            yield return new TournamentRoundCompletedEvent
            {
                Round = "Round of 64",
                Results = roundResults
            };
        }

        // --- Round of 32, Sweet 16, Elite 8 (pair adjacent winners each round) ---
        foreach (var roundName in new[] { "Round of 32", "Sweet 16", "Elite 8" })
        {
            var roundResults = new List<MatchupResult>();

            foreach (var region in bracket.Regions)
            {
                var winners = regionWinners[region.Name];
                var nextWinners = new List<string>();

                for (int i = 0; i < winners.Count; i += 2)
                {
                    ct.ThrowIfCancellationRequested();
                    gameNumber++;

                    var team1 = Resolve(winners[i], teamLookup);
                    var team2 = Resolve(winners[i + 1], teamLookup);

                    yield return new TournamentGameStartedEvent
                    {
                        GameInfo = new TournamentGameInfo
                        {
                            AwayTeam = team1.Name,
                            HomeTeam = team2.Name,
                            Round = roundName,
                            Region = region.Name,
                            GameNumber = gameNumber
                        }
                    };

                    var result = await QuickSimAsync(team1, team2, ct);
                    roundResults.Add(result);
                    allResults.Add(result);
                    nextWinners.Add(result.Winner);

                    yield return new TournamentGameCompletedEvent
                    {
                        Result = result,
                        Round = roundName,
                        Region = region.Name
                    };

                    if (delayMs > 0) await Task.Delay(delayMs, ct);
                }

                regionWinners[region.Name] = nextWinners;
            }

            yield return new TournamentRoundCompletedEvent
            {
                Round = roundName,
                Results = roundResults
            };
        }

        // --- Final Four (South vs East, West vs Midwest) ---
        {
            var ffTeams = bracket.Regions.Select(r => regionWinners[r.Name][0]).ToList();
            var roundResults = new List<MatchupResult>();

            for (int i = 0; i < ffTeams.Count; i += 2)
            {
                ct.ThrowIfCancellationRequested();
                gameNumber++;

                var team1 = Resolve(ffTeams[i], teamLookup);
                var team2 = Resolve(ffTeams[i + 1], teamLookup);

                yield return new TournamentGameStartedEvent
                {
                    GameInfo = new TournamentGameInfo
                    {
                        AwayTeam = team1.Name,
                        HomeTeam = team2.Name,
                        Round = "Final Four",
                        GameNumber = gameNumber
                    }
                };

                var result = await QuickSimAsync(team1, team2, ct);
                roundResults.Add(result);
                allResults.Add(result);

                yield return new TournamentGameCompletedEvent
                {
                    Result = result,
                    Round = "Final Four"
                };

                if (delayMs > 0) await Task.Delay(delayMs, ct);
            }

            yield return new TournamentRoundCompletedEvent
            {
                Round = "Final Four",
                Results = roundResults
            };

            // --- Championship ---
            gameNumber++;
            var fin1 = Resolve(roundResults[0].Winner, teamLookup);
            var fin2 = Resolve(roundResults[1].Winner, teamLookup);

            yield return new TournamentGameStartedEvent
            {
                GameInfo = new TournamentGameInfo
                {
                    AwayTeam = fin1.Name,
                    HomeTeam = fin2.Name,
                    Round = "Championship",
                    GameNumber = gameNumber
                }
            };

            var championship = await QuickSimAsync(fin1, fin2, ct);
            allResults.Add(championship);

            yield return new TournamentGameCompletedEvent
            {
                Result = championship,
                Round = "Championship"
            };

            yield return new TournamentCompletedEvent
            {
                Champion = championship.Winner,
                AllResults = allResults
            };
        }
    }

    private async Task<MatchupResult> QuickSimAsync(
        CollegeModel away, CollegeModel home, CancellationToken ct)
    {
        await foreach (var evt in _gameEngine.SimulateGameAsync(away, home, SimSpeed.Instant, ct))
        {
            if (evt is GameCompletedEvent completed)
                return completed.Result;
        }

        throw new InvalidOperationException("Game simulation did not produce a result");
    }

    private static CollegeModel Resolve(string name, Dictionary<string, CollegeModel> lookup)
    {
        if (lookup.TryGetValue(name, out var team))
            return team;
        throw new InvalidOperationException($"Team not found in bracket: {name}");
    }
}
