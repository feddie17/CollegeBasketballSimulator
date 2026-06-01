<template>
  <div class="season-view">
    <h1>Season Simulation</h1>

    <div v-if="!seasonStore.seasonId" class="setup">
      <p>Simulate a full season &mdash; advance a day or a week at a time, or run it to the end.</p>
      <button class="start-btn" @click="startSeason" :disabled="starting">
        {{ starting ? 'Connecting...' : 'Start Season' }}
      </button>
      <p v-if="seasonStore.error" class="error">{{ seasonStore.error }}</p>
    </div>

    <div v-else class="simulation">
      <div class="sim-header">
        <div class="status">
          <span class="status-dot" :class="{ connected: seasonStore.connected }"></span>
          <span v-if="seasonStore.isFinished" class="progress-label">Season Complete</span>
          <span v-else-if="seasonStore.currentWeek > 0" class="progress-label">
            Week {{ seasonStore.currentWeek }} of {{ seasonStore.totalWeeks }}
          </span>
          <span v-else class="progress-label">Preseason</span>
        </div>

        <div class="controls" v-if="!seasonStore.isFinished">
          <template v-if="seasonStore.running">
            <span class="running-label">Simulating&hellip;</span>
            <button class="ctrl-btn stop" @click="seasonStore.stop">Stop</button>
          </template>
          <template v-else>
            <button class="ctrl-btn" :disabled="stepDisabled" @click="seasonStore.simulateDay">
              Sim Day
            </button>
            <button class="ctrl-btn" :disabled="stepDisabled" @click="seasonStore.simulateWeek">
              Sim Week
            </button>
            <select v-model="selectedSpeed" class="speed-select" :disabled="stepDisabled">
              <option value="Instant">Instant</option>
              <option value="Fast">Fast</option>
              <option value="Medium">Medium</option>
              <option value="Slow">Slow</option>
            </select>
            <button class="ctrl-btn run" :disabled="stepDisabled" @click="simulateToEnd">
              Sim to End
            </button>
          </template>
        </div>

        <div v-else class="controls">
          <button class="ctrl-btn run" @click="createTournament">Create Tournament →</button>
          <button class="ctrl-btn" @click="resetSeason">New Season</button>
        </div>
      </div>

      <div class="sim-body">
        <StandingsTable class="standings-main" />

        <aside class="results-ticker">
          <div class="ticker-header">
            <h3>Recent Results</h3>
            <select v-model="resultConf" class="conf-select" v-if="resultConferences.length > 1">
              <option v-for="conf in resultConferences" :key="conf" :value="conf">{{ conf }}</option>
            </select>
          </div>
          <div v-if="!seasonStore.recentResults.length" class="empty">
            Game results will stream in here...
          </div>
          <div v-else-if="!filteredResults.length" class="empty">
            No {{ resultConf }} games in the recent feed.
          </div>
          <ul v-else>
            <li v-for="(r, i) in filteredResults" :key="i" class="result-row">
              <span class="result-line">
                <span class="winner">{{ r.winner }}</span> {{ r.winnerScore }}&ndash;{{ r.loserScore }}
                <span class="loser">{{ r.loser }}</span>
                <span v-if="r.overtimes > 0" class="ot-badge">{{ otLabel(r.overtimes) }}</span>
              </span>
              <span class="result-date">{{ formatDate(r.gameDate) }}</span>
            </li>
          </ul>
        </aside>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useSeasonStore } from '../stores/seasonStore'
import { useTournamentStore } from '../stores/tournamentStore'
import StandingsTable from '../components/StandingsTable.vue'

const router = useRouter()
const seasonStore = useSeasonStore()
const tournamentStore = useTournamentStore()
const selectedSpeed = ref('Medium')
const starting = ref(false)
const resultConf = ref('All')

const stepDisabled = computed(() => seasonStore.busy || seasonStore.running)

// Team -> conference lookup, built from the current standings.
const teamConfMap = computed(() => {
  const map = {}
  for (const t of seasonStore.rankings) map[t.name] = t.conference
  return map
})

const resultConferences = computed(() => {
  const set = new Set(seasonStore.rankings.map((t) => t.conference).filter(Boolean))
  return ['All', ...Array.from(set).sort((a, b) => a.localeCompare(b))]
})

// Results involving the selected conference (either team), newest first.
const filteredResults = computed(() => {
  if (resultConf.value === 'All') return seasonStore.recentResults
  const map = teamConfMap.value
  return seasonStore.recentResults.filter(
    (r) => map[r.winner] === resultConf.value || map[r.loser] === resultConf.value
  )
})

async function startSeason() {
  starting.value = true
  await seasonStore.startSeason()
  starting.value = false
}

function simulateToEnd() {
  seasonStore.simulateToEnd(selectedSpeed.value)
}

async function resetSeason() {
  await seasonStore.disconnect()
  seasonStore.reset()
}

// Hand the completed season's final standings to the tournament, seeded in
// standings order, then jump to the tournament view to watch it play out.
function createTournament() {
  const teamNames = seasonStore.rankings.map((t) => t.name)
  tournamentStore.startTournamentFromTeams(teamNames, selectedSpeed.value)
  router.push('/tournament')
}

function otLabel(n) {
  return n === 1 ? 'OT' : `${n}OT`
}

function formatDate(d) {
  if (!d) return ''
  const date = new Date(d)
  return Number.isNaN(date.getTime())
    ? ''
    : date.toLocaleDateString(undefined, { month: 'short', day: 'numeric' })
}

onUnmounted(() => { seasonStore.disconnect() })
</script>

<style scoped>
.season-view { max-width: 1200px; margin: 0 auto; padding: 0 1rem; }

.setup { text-align: center; margin-top: 2rem; }
.setup p { color: #8b8fa8; margin-bottom: 1.5rem; }

.start-btn {
  background: #4f8ff7;
  color: white;
  border: none;
  padding: 0.8rem 2rem;
  border-radius: 8px;
  font-size: 1rem;
  cursor: pointer;
}
.start-btn:disabled { opacity: 0.5; cursor: not-allowed; }

.sim-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  flex-wrap: wrap;
  margin-bottom: 1rem;
}

.status { display: flex; align-items: center; gap: 0.5rem; }

.status-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: #f87171;
}
.status-dot.connected { background: #34d399; }

.progress-label {
  font-size: 0.9rem;
  font-weight: 600;
  color: #a78bfa;
}

.controls { display: flex; align-items: center; gap: 0.5rem; flex-wrap: wrap; }
.running-label { font-size: 0.85rem; color: #8b8fa8; font-style: italic; }

.ctrl-btn {
  background: #242838;
  border: 1px solid #2e3348;
  color: #e1e4ed;
  padding: 0.5rem 1rem;
  border-radius: 8px;
  cursor: pointer;
  font-size: 0.85rem;
}
.ctrl-btn:hover:not(:disabled) { border-color: #4f8ff7; }
.ctrl-btn:disabled { opacity: 0.4; cursor: not-allowed; }
.ctrl-btn.run { background: #4f8ff7; border-color: #4f8ff7; color: white; }
.ctrl-btn.stop { background: transparent; border-color: #f87171; color: #f87171; }

.speed-select {
  padding: 0.5rem 0.6rem;
  background: #242838;
  border: 1px solid #2e3348;
  border-radius: 8px;
  color: #e1e4ed;
  font-size: 0.85rem;
}
.speed-select:disabled { opacity: 0.4; }

.sim-body {
  display: grid;
  grid-template-columns: 1fr 320px;
  gap: 1rem;
  align-items: start;
}

.results-ticker {
  background: #1a1d27;
  border: 1px solid #2e3348;
  border-radius: 12px;
  padding: 1.5rem;
}
.ticker-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.5rem;
  margin-bottom: 1rem;
}
.ticker-header h3 { margin: 0; }
.ticker-header .conf-select {
  padding: 0.3rem 0.5rem;
  background: #242838;
  border: 1px solid #2e3348;
  border-radius: 6px;
  color: #e1e4ed;
  font-size: 0.78rem;
}
.results-ticker .empty { color: #8b8fa8; font-style: italic; }
.results-ticker ul {
  list-style: none;
  margin: 0;
  padding: 0;
  max-height: 520px;
  overflow-y: auto;
}

.result-row {
  display: flex;
  flex-direction: column;
  gap: 0.15rem;
  padding: 0.5rem 0;
  border-bottom: 1px solid #2e3348;
  font-size: 0.85rem;
}
.result-line { color: #e1e4ed; }
.winner { color: #34d399; font-weight: 600; }
.loser { color: #8b8fa8; }
.ot-badge {
  margin-left: 0.4rem;
  font-size: 0.65rem;
  font-weight: 700;
  color: #f59e0b;
  border: 1px solid #f59e0b;
  border-radius: 4px;
  padding: 0 0.3rem;
}
.result-date { font-size: 0.7rem; color: #8b8fa8; }

.error {
  margin-top: 1rem;
  padding: 0.6rem 0.9rem;
  background: rgba(248, 113, 113, 0.1);
  border: 1px solid rgba(248, 113, 113, 0.4);
  color: #f87171;
  border-radius: 8px;
  display: inline-block;
}

@media (max-width: 860px) {
  .sim-body { grid-template-columns: 1fr; }
}
</style>
