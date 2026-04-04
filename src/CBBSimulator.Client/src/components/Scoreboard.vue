<template>
  <div class="scoreboard">
    <div class="team away">
      <span class="name">{{ gameStore.away?.name || 'Away' }}</span>
      <span class="score">{{ gameStore.awayScore }}</span>
    </div>
    <div class="center">
      <div class="clock">{{ gameStore.clock }}</div>
      <div class="period">{{ gameStore.period }}</div>
    </div>
    <div class="team home">
      <span class="score">{{ gameStore.homeScore }}</span>
      <span class="name">{{ gameStore.home?.name || 'Home' }}</span>
    </div>
  </div>
</template>

<script setup>
import { useGameStore } from '../stores/gameStore'
const gameStore = useGameStore()
</script>

<style scoped>
.scoreboard {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 2rem;
  background: #1a1d27;
  border: 1px solid #2e3348;
  border-radius: 12px;
  padding: 2rem;
  margin-bottom: 1.5rem;
}

.team {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.name { font-size: 1.2rem; font-weight: 600; }
.score { font-size: 2.5rem; font-weight: 700; color: #4f8ff7; }

.center { text-align: center; }
.clock { font-size: 1.5rem; font-weight: 700; }
.period { font-size: 0.85rem; color: #8b8fa8; }
</style>
