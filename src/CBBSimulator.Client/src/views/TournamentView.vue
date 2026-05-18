<template>
  <div class="tournament-view">
    <h1>Tournament Simulation</h1>

    <div v-if="!tournamentStore.tournamentId" class="setup">
      <p>Simulate a full 64-team March Madness bracket</p>
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
        <button class="start-btn" @click="startTournament" :disabled="starting">
          {{ starting ? 'Connecting...' : 'Start Tournament' }}
        </button>
      </div>
      <p v-if="tournamentStore.error" class="error">{{ tournamentStore.error }}</p>
    </div>

    <div v-else class="simulation">
      <div class="sim-header">
        <div class="status">
          <span class="status-dot" :class="{ connected: tournamentStore.connected }"></span>
          <span v-if="tournamentStore.currentRound" class="round-label">{{ tournamentStore.currentRound }}</span>
          <span v-else-if="tournamentStore.isFinished" class="round-label">Tournament Complete</span>
        </div>
        <button v-if="tournamentStore.isFinished" class="reset-btn" @click="resetTournament">
          New Tournament
        </button>
      </div>
      <BracketViewer />
    </div>
  </div>
</template>

<script setup>
import { ref, onUnmounted } from 'vue'
import { useTournamentStore } from '../stores/tournamentStore'
import BracketViewer from '../components/BracketViewer.vue'

const tournamentStore = useTournamentStore()
const selectedSpeed = ref('Medium')
const starting = ref(false)

async function startTournament() {
  starting.value = true
  await tournamentStore.startTournament(selectedSpeed.value)
  starting.value = false
}

async function resetTournament() {
  await tournamentStore.disconnect()
  tournamentStore.reset()
}

onUnmounted(() => { tournamentStore.disconnect() })
</script>

<style scoped>
.tournament-view { padding: 0 1rem; }

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

.status {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.status-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: #f87171;
}
.status-dot.connected { background: #34d399; }

.round-label {
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

.error {
  margin-top: 1rem;
  padding: 0.6rem 0.9rem;
  background: rgba(248, 113, 113, 0.1);
  border: 1px solid rgba(248, 113, 113, 0.4);
  color: #f87171;
  border-radius: 8px;
  display: inline-block;
}
</style>
