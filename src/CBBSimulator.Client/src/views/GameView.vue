<template>
  <div class="game-view">
    <h1>Game Simulation</h1>

    <div v-if="!gameStore.gameId" class="setup">
      <TeamSelector label="Away Team" @select="awayTeam = $event" />
      <span class="vs">VS</span>
      <TeamSelector label="Home Team" @select="homeTeam = $event" />

      <button class="start-btn" @click="startGame" :disabled="!awayTeam || !homeTeam">
        Start Game
      </button>
    </div>

    <div v-else class="simulation">
      <Scoreboard />
      <PossessionLog />
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useGameStore } from '../stores/gameStore'
import TeamSelector from '../components/TeamSelector.vue'
import Scoreboard from '../components/Scoreboard.vue'
import PossessionLog from '../components/PossessionLog.vue'

const gameStore = useGameStore()
const awayTeam = ref(null)
const homeTeam = ref(null)

// TODO: Wire up SignalR connection
async function startGame() {
  // Will be implemented when GameHub is connected
}
</script>

<style scoped>
.setup {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  justify-content: center;
  margin-top: 2rem;
  flex-wrap: wrap;
}

.vs {
  font-size: 1.5rem;
  font-weight: 700;
  color: #8b8fa8;
}

.start-btn {
  background: #4f8ff7;
  color: white;
  border: none;
  padding: 0.8rem 2rem;
  border-radius: 8px;
  font-size: 1rem;
  cursor: pointer;
  margin-top: 1rem;
  width: 100%;
  max-width: 300px;
}

.start-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
</style>
