<template>
  <div class="game-view">
    <h1>Game Simulation</h1>

    <div v-if="!gameStore.gameId" class="setup">
      <TeamSelector label="Away Team" @select="awayTeam = $event" />
      <span class="vs">VS</span>
      <TeamSelector label="Home Team" @select="homeTeam = $event" />

      <div class="speed-row">
        <label for="speed-sel">Speed:</label>
        <select id="speed-sel" v-model="speed">
          <option value="Instant">Instant</option>
          <option value="Fast">Fast</option>
          <option value="Medium">Medium</option>
          <option value="Slow">Slow</option>
        </select>
      </div>

      <button
        class="start-btn"
        @click="startGame"
        :disabled="!awayTeam || !homeTeam"
      >
        Start Game
      </button>
    </div>

    <div v-else class="simulation">
      <Scoreboard />
      <PossessionLog />
      <button v-if="gameStore.isFinished" class="new-game-btn" @click="newGame">
        New Game
      </button>
    </div>

    <div v-if="gameStore.error" class="error">⚠ {{ gameStore.error }}</div>
  </div>
</template>

<script setup>
import { ref, onUnmounted } from 'vue'
import { useGameStore } from '../stores/gameStore'
import TeamSelector from '../components/TeamSelector.vue'
import Scoreboard from '../components/Scoreboard.vue'
import PossessionLog from '../components/PossessionLog.vue'

const gameStore = useGameStore()
const awayTeam = ref(null)
const homeTeam = ref(null)
const speed = ref('Medium')

async function startGame() {
  if (!awayTeam.value || !homeTeam.value) return
  await gameStore.startGame(awayTeam.value, homeTeam.value, speed.value)
}

async function newGame() {
  await gameStore.disconnect()
  gameStore.reset()
  awayTeam.value = null
  homeTeam.value = null
}

onUnmounted(() => { gameStore.disconnect() })
</script>

<style scoped>
.game-view { max-width: 1200px; margin: 0 auto; }
.setup {
  display: flex;
  align-items: center;
  gap: 1.5rem;
  justify-content: center;
  margin-top: 2rem;
  flex-wrap: wrap;
}
.vs { font-size: 1.5rem; font-weight: 700; color: #8b8fa8; }
.speed-row { display: flex; align-items: center; gap: 0.5rem; }
.speed-row label { font-size: 0.85rem; color: #8b8fa8; }
.speed-row select {
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
  margin-top: 1rem;
  width: 100%;
  max-width: 300px;
}
.start-btn:disabled { opacity: 0.5; cursor: not-allowed; }
.new-game-btn {
  margin-top: 1rem;
  background: transparent;
  border: 1px solid #4f8ff7;
  color: #4f8ff7;
  padding: 0.6rem 1.5rem;
  border-radius: 8px;
  cursor: pointer;
}
.error {
  margin-top: 1rem;
  padding: 0.6rem 0.9rem;
  background: rgba(248, 113, 113, 0.1);
  border: 1px solid rgba(248, 113, 113, 0.4);
  color: #f87171;
  border-radius: 8px;
}
</style>
