<template>
  <div class="possession-log">
    <h3>Play-by-Play</h3>
    <div class="log-container" ref="logContainer">
      <div v-if="!gameStore.possessions.length" class="empty">
        Waiting for game to start...
      </div>
      <div
        v-for="(p, i) in gameStore.possessions"
        :key="i"
        class="log-entry"
      >
        <span class="timestamp">{{ p.scoreboard?.clock }}</span>
        <span class="team-name">{{ p.team }}</span>
        <span class="action">{{ p.action }}</span>
        <span v-if="p.pointsScored" class="points">+{{ p.pointsScored }}</span>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, nextTick } from 'vue'
import { useGameStore } from '../stores/gameStore'

const gameStore = useGameStore()
const logContainer = ref(null)

watch(() => gameStore.possessions.length, async () => {
  await nextTick()
  if (logContainer.value) {
    logContainer.value.scrollTop = logContainer.value.scrollHeight
  }
})
</script>

<style scoped>
.possession-log {
  background: #1a1d27;
  border: 1px solid #2e3348;
  border-radius: 12px;
  padding: 1.5rem;
}

h3 { margin-bottom: 1rem; }

.log-container {
  max-height: 400px;
  overflow-y: auto;
}

.empty { color: #8b8fa8; font-style: italic; }

.log-entry {
  display: flex;
  gap: 0.8rem;
  padding: 0.4rem 0;
  border-bottom: 1px solid #242838;
  font-size: 0.85rem;
}

.timestamp { color: #8b8fa8; min-width: 50px; }
.team-name { font-weight: 600; min-width: 120px; }
.action { flex: 1; }
.points { color: #34d399; font-weight: 600; }
</style>
