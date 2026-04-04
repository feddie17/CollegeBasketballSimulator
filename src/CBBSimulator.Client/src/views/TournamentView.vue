<template>
  <div class="tournament-view">
    <h1>Tournament Simulation</h1>

    <div v-if="!tournamentStore.tournamentId" class="setup">
      <p>Simulate a full 64-team March Madness bracket</p>
      <button class="start-btn" @click="startTournament">
        Start Tournament
      </button>
    </div>

    <div v-else class="simulation">
      <BracketViewer />
    </div>
  </div>
</template>

<script setup>
import { useTournamentStore } from '../stores/tournamentStore'
import BracketViewer from '../components/BracketViewer.vue'

const tournamentStore = useTournamentStore()

// TODO: Wire up SignalR connection
async function startTournament() {
  // Will be implemented when TournamentHub is connected
}
</script>

<style scoped>
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
</style>
