<template>
  <div class="season-view">
    <h1>Season Simulation</h1>

    <div v-if="!started" class="setup">
      <p>Simulate a full season with weekly ranking updates</p>
      <button class="start-btn" @click="startSeason">
        Start Season
      </button>
    </div>

    <div v-else class="simulation">
      <StandingsTable />
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import StandingsTable from '../components/StandingsTable.vue'

const started = ref(false)

// TODO: Wire up SignalR connection
async function startSeason() {
  // Will be implemented when SeasonHub is connected
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
