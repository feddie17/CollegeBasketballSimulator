<template>
  <div class="bracket-viewer">
    <h3>Tournament Bracket</h3>
    <div v-if="!tournamentStore.completedGames.length" class="empty">
      Bracket will appear here once the tournament starts...
    </div>
    <div v-else class="bracket">
      <!-- TODO: Implement SVG or CSS Grid bracket layout -->
      <div v-for="(game, i) in tournamentStore.completedGames" :key="i" class="bracket-game">
        <span class="round">{{ game.round }}</span>
        <span class="matchup">
          {{ game.result.awayTeam }} {{ game.result.awayTeamScore }} -
          {{ game.result.homeTeamScore }} {{ game.result.homeTeam }}
        </span>
        <span class="winner">W: {{ game.result.winner }}</span>
      </div>
    </div>
    <div v-if="tournamentStore.champion" class="champion">
      Champion: {{ tournamentStore.champion }}
    </div>
  </div>
</template>

<script setup>
import { useTournamentStore } from '../stores/tournamentStore'
const tournamentStore = useTournamentStore()
</script>

<style scoped>
.bracket-viewer {
  background: #1a1d27;
  border: 1px solid #2e3348;
  border-radius: 12px;
  padding: 1.5rem;
}

.empty { color: #8b8fa8; font-style: italic; }

.bracket-game {
  display: flex;
  gap: 1rem;
  padding: 0.5rem 0;
  border-bottom: 1px solid #242838;
  font-size: 0.85rem;
}

.round { color: #a78bfa; font-weight: 600; min-width: 100px; }
.matchup { flex: 1; }
.winner { color: #34d399; font-weight: 600; }

.champion {
  margin-top: 1.5rem;
  text-align: center;
  font-size: 1.5rem;
  font-weight: 700;
  color: #f59e0b;
}
</style>
