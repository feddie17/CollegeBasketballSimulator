<template>
  <div class="standings-table">
    <div class="standings-header">
      <h3>Standings</h3>
      <div v-if="seasonStore.rankings.length" class="conf-filter">
        <label for="conf-sel">Conference:</label>
        <select id="conf-sel" v-model="selectedConference" class="conf-select">
          <option v-for="conf in conferences" :key="conf" :value="conf">{{ conf }}</option>
        </select>
      </div>
    </div>

    <div v-if="!seasonStore.rankings.length" class="empty">
      Standings will appear here once the season starts...
    </div>
    <table v-else>
      <thead>
        <tr>
          <th
            v-for="col in columns"
            :key="col.key"
            :class="{ active: sortKey === col.key, num: col.num }"
            @click="toggleSort(col.key)"
          >
            {{ col.label }}
            <span v-if="sortKey === col.key" class="arrow">{{ sortDir === 'asc' ? '▲' : '▼' }}</span>
          </th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="team in displayedRankings" :key="team.name">
          <td class="num" :class="{ top3: team.rank <= 3 }">{{ team.rank }}</td>
          <td>{{ team.name }}</td>
          <td>{{ team.conference }}</td>
          <td class="num">{{ team.wins }}</td>
          <td class="num">{{ team.losses }}</td>
          <td class="num">{{ team.confWins }}-{{ team.confLosses }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useSeasonStore } from '../stores/seasonStore'

const seasonStore = useSeasonStore()

const columns = [
  { key: 'rank', label: 'Rank', num: true },
  { key: 'name', label: 'Team', num: false },
  { key: 'conference', label: 'Conference', num: false },
  { key: 'wins', label: 'W', num: true },
  { key: 'losses', label: 'L', num: true },
  { key: 'conf', label: 'Conf', num: true }
]

const selectedConference = ref('All')
const sortKey = ref('rank')
const sortDir = ref('asc')

const conferences = computed(() => {
  const set = new Set(seasonStore.rankings.map((t) => t.conference).filter(Boolean))
  return ['All', ...Array.from(set).sort((a, b) => a.localeCompare(b))]
})

function toggleSort(key) {
  if (sortKey.value === key) {
    sortDir.value = sortDir.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortKey.value = key
    sortDir.value = 'asc'
  }
}

function compare(a, b) {
  const key = sortKey.value
  if (key === 'name' || key === 'conference') {
    return a[key].localeCompare(b[key])
  }
  if (key === 'conf') {
    return (a.confWins - b.confWins) || (b.confLosses - a.confLosses)
  }
  return a[key] - b[key]
}

const displayedRankings = computed(() => {
  const filtered = selectedConference.value === 'All'
    ? seasonStore.rankings
    : seasonStore.rankings.filter((t) => t.conference === selectedConference.value)

  const sorted = [...filtered].sort(compare)
  if (sortDir.value === 'desc') sorted.reverse()
  return sorted
})
</script>

<style scoped>
.standings-table {
  background: #1a1d27;
  border: 1px solid #2e3348;
  border-radius: 12px;
  padding: 1.5rem;
}

.standings-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}
.standings-header h3 { margin: 0; }

.conf-filter { display: flex; align-items: center; gap: 0.5rem; }
.conf-filter label { font-size: 0.85rem; color: #8b8fa8; }
.conf-select {
  padding: 0.4rem 0.6rem;
  background: #242838;
  border: 1px solid #2e3348;
  border-radius: 6px;
  color: #e1e4ed;
  font-size: 0.85rem;
}

.empty { color: #8b8fa8; font-style: italic; }

table { width: 100%; border-collapse: collapse; }

th, td {
  padding: 0.5rem 0.8rem;
  text-align: left;
  border-bottom: 1px solid #2e3348;
}
th.num, td.num { text-align: right; }

th {
  font-size: 0.75rem;
  text-transform: uppercase;
  color: #8b8fa8;
  cursor: pointer;
  user-select: none;
  white-space: nowrap;
}
th:hover { color: #e1e4ed; }
th.active { color: #4f8ff7; }

.arrow { font-size: 0.65rem; }

td.top3 { color: #34d399; font-weight: 600; }
</style>
