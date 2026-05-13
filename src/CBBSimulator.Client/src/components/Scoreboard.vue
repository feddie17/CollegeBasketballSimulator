<template>
  <div class="scoreboard" :class="{ final: gameStore.isFinished }">
    <div
      class="team away"
      :class="{
        scoring: scoringFlash === 'away',
        'has-ball': !gameStore.homeHasPossession && !gameStore.isFinished
      }"
    >
      <div class="meta">
        <span v-if="gameStore.away?.rank" class="rank">#{{ gameStore.away.rank }}</span>
        <span class="name">{{ gameStore.away?.name || 'Away' }}</span>
        <span
          v-if="!gameStore.homeHasPossession && !gameStore.isFinished"
          class="ball"
          aria-label="has possession"
        >●</span>
      </div>
      <div class="score">{{ gameStore.awayScore }}</div>
      <div class="fouls">Fouls: {{ gameStore.awayFouls }}</div>
    </div>

    <div class="center">
      <div class="clock">{{ gameStore.clock }}</div>
      <div class="period">{{ gameStore.period }}</div>
      <div v-if="gameStore.isFinished" class="final-badge">
        FINAL<span v-if="gameStore.result?.overtimes"> / {{ gameStore.result.overtimes }}OT</span>
      </div>
    </div>

    <div
      class="team home"
      :class="{
        scoring: scoringFlash === 'home',
        'has-ball': gameStore.homeHasPossession && !gameStore.isFinished
      }"
    >
      <div class="meta">
        <span
          v-if="gameStore.homeHasPossession && !gameStore.isFinished"
          class="ball"
          aria-label="has possession"
        >●</span>
        <span class="name">{{ gameStore.home?.name || 'Home' }}</span>
        <span v-if="gameStore.home?.rank" class="rank">#{{ gameStore.home.rank }}</span>
      </div>
      <div class="score">{{ gameStore.homeScore }}</div>
      <div class="fouls">Fouls: {{ gameStore.homeFouls }}</div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'
import { useGameStore } from '../stores/gameStore'

const gameStore = useGameStore()
const scoringFlash = ref(null)

watch(
  () => gameStore.lastScoringTeam,
  (signal) => {
    if (!signal) return
    const teamName = signal.split('#')[0]
    if (teamName === gameStore.away?.name) scoringFlash.value = 'away'
    else if (teamName === gameStore.home?.name) scoringFlash.value = 'home'
    setTimeout(() => { scoringFlash.value = null }, 800)
  }
)
</script>

<style scoped>
.scoreboard {
  display: grid;
  grid-template-columns: 1fr auto 1fr;
  align-items: center;
  gap: 2rem;
  background: #1a1d27;
  border: 1px solid #2e3348;
  border-radius: 12px;
  padding: 2rem;
  margin-bottom: 1.5rem;
  transition: background 0.4s ease;
}
.scoreboard.final { background: #1a2333; }

.team {
  display: grid;
  grid-template-columns: 1fr;
  gap: 0.4rem;
  padding: 1rem;
  border-radius: 10px;
  transition: background 0.7s ease;
}
.team.away { text-align: right; }
.team.home { text-align: left; }
.team.has-ball { background: rgba(79, 143, 247, 0.06); }
.team.scoring { background: rgba(79, 143, 247, 0.25); animation: flash 0.8s ease; }

@keyframes flash {
  0%   { background: rgba(79, 143, 247, 0.55); }
  100% { background: rgba(79, 143, 247, 0.06); }
}

.meta {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  font-size: 1rem;
  font-weight: 600;
  color: #e1e4ed;
}
.team.away .meta { justify-content: flex-end; }
.team.home .meta { justify-content: flex-start; }

.rank { color: #8b8fa8; font-size: 0.8rem; font-variant-numeric: tabular-nums; }
.name { color: #e1e4ed; }
.ball { color: #4f8ff7; font-size: 0.7rem; line-height: 1; }

.score {
  font-size: 3rem;
  font-weight: 800;
  color: #4f8ff7;
  font-variant-numeric: tabular-nums;
  line-height: 1;
}

.fouls { color: #8b8fa8; font-size: 0.78rem; }

.center { text-align: center; min-width: 9rem; }
.clock {
  font-size: 1.6rem;
  font-weight: 700;
  font-variant-numeric: tabular-nums;
  color: #e1e4ed;
}
.period { font-size: 0.85rem; color: #8b8fa8; margin-top: 0.2rem; }
.final-badge {
  margin-top: 0.6rem;
  display: inline-block;
  padding: 0.2rem 0.7rem;
  background: #4f8ff7;
  color: #0f1117;
  border-radius: 999px;
  font-size: 0.75rem;
  font-weight: 800;
  letter-spacing: 0.05em;
}
</style>
