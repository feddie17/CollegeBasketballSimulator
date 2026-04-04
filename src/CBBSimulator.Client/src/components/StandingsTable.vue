<template>
  <div class="standings-table">
    <h3>Standings</h3>
    <div v-if="!rankings.length" class="empty">
      Standings will appear here once the season starts...
    </div>
    <table v-else>
      <thead>
        <tr>
          <th>Rank</th>
          <th>Team</th>
          <th>Conference</th>
          <th>W</th>
          <th>L</th>
          <th>Conf</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="team in rankings" :key="team.name">
          <td>{{ team.rank }}</td>
          <td>{{ team.name }}</td>
          <td>{{ team.conference }}</td>
          <td>{{ team.wins }}</td>
          <td>{{ team.losses }}</td>
          <td>{{ team.confWins }}-{{ team.confLosses }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { ref } from 'vue'

// TODO: Connect to SeasonHub events
const rankings = ref([])
</script>

<style scoped>
.standings-table {
  background: #1a1d27;
  border: 1px solid #2e3348;
  border-radius: 12px;
  padding: 1.5rem;
}

.empty { color: #8b8fa8; font-style: italic; }

table { width: 100%; border-collapse: collapse; }

th, td {
  padding: 0.5rem 0.8rem;
  text-align: left;
  border-bottom: 1px solid #2e3348;
}

th {
  font-size: 0.75rem;
  text-transform: uppercase;
  color: #8b8fa8;
}
</style>
