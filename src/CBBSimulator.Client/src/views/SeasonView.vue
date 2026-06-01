<template>
  <div class="season-view">
    <h1>Season Simulation</h1>

    <div v-if="!seasonStore.seasonId" class="setup">
      <p>Simulate a full season with weekly ranking updates</p>
      <div class="controls">
        <div class="speed-row">
          <label for="speed-sel">Speed:</label>
          <select id="speed-sel" v-model="selectedSpeed" class="speed-select">
            <option value="Instant">Instant</option>
            <option value="Fast">Fast</option>
            <option value="Medium">Medium</option>
            <option value="Slow">Slow</option>
          </select>
        </div>
        <button class="start-btn" @click="startSeason" :disabled="starting">
          {{ starting ? 'Connecting...' : 'Start Season' }}
        </button>
      </div>
      <p v-if="seasonStore.error" class="error">{{ seasonStore.error }}</p>
    </div>

    <div v-else class="simulation">
      <div class="sim-header">
        <div class="status">
          <span class="status-dot" :class="{ connected: seasonStore.connected }"></span>
          <span v-if="seasonStore.isFinished" class="progress-label">Season Complete</span>
          <span v-else-if="seasonStore.totalWeeks" class="progress-label">
            Week {{ seasonStore.currentWeek }} of {{ seasonStore.totalWeeks }}
          </span>
          <span v-else class="progress-label">Starting season...</span>
        </div>
        <button v-if="seasonStore.isFinished" class="reset-btn" @click="resetSeason">
          New Season
        </button>
      </div>

      <div class="sim-body">
        <StandingsTable class="standings-main" />

        <aside class="results-ticker">
          <h3>Recent Results</h3>
          <div v-if="!seasonStore.recentResults.length" class="empty">
            Game results will stream in here...
          </div>
          <ul v-else>
            <li v-for="(r, i) in seasonStore.recentResults" :key="i" class="result-row">
              <span class="result-line">
                <span class="winner">{{ r.winner }}</span> {{ r.winnerScore }}–{{ r.loserScore }}
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
import { ref, onUnmounted } from 'vue'
import { useSeasonStore } from '../stores/seasonStore'
import StandingsTable from '../components/StandingsTable.vue'

const seasonStore = useSeasonStore()
const selectedSpeed = ref('Medium')
const starting = ref(false)

async function startSeason() {
  starting.value = true
  await seasonStore.startSeason(selectedSpeed.value)
  starting.value = false
}

async function resetSeason() {
  await seasonStore.disconnect()
  seasonStore.reset()
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

.controls {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  flex-wrap: wrap;
}

.speed-row { display: flex; align-items: center; gap: 0.5rem; }
.speed-row label { font-size: 0.85rem; color: #8b8fa8; }
.speed-select {
  padding: 0.5rem 0.6rem;
  background: #242838;
  border: 1px solid #2e3348;
  border-radius: 6px;
  color: #e1e4ed;
  font-size: 0.9rem;
}

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

.reset-btn {
  background: transparent;
  border: 1px solid #4f8ff7;
  color: #4f8ff7;
  padding: 0.5rem 1.2rem;
  border-radius: 8px;
  cursor: pointer;
  font-size: 0.85rem;
}

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
.results-ticker h3 { margin: 0 0 1rem; }
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
