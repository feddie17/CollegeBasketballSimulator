<template>
  <div class="standings-table">
    <div class="standings-header">
      <h3>Standings</h3>
      <div class="header-controls" v-if="seasonStore.rankings.length">
        <div class="view-toggle">
          <button :class="{ active: viewMode === 'overall' }" @click="viewMode = 'overall'">Overall</button>
          <button :class="{ active: viewMode === 'conference' }" @click="viewMode = 'conference'">By Conference</button>
        </div>
        <div v-if="viewMode === 'overall'" class="conf-filter">
          <label for="conf-sel">Conference:</label>
          <select id="conf-sel" v-model="selectedConference" class="conf-select">
            <option v-for="conf in conferences" :key="conf" :value="conf">{{ conf }}</option>
          </select>
        </div>
      </div>
    </div>

    <div v-if="!seasonStore.rankings.length" class="empty">
      Standings will appear here once the season starts...
    </div>

    <!-- Overall (flat) view -->
    <table v-else-if="viewMode === 'overall'">
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
          <td class="num rank-cell">
            <span class="overall-rank" :class="{ top3: team.rank <= 3 }">{{ team.rank }}</span>
            <span class="conf-rank">{{ team.conference }} #{{ confRankMap[team.name] }}</span>
          </td>
          <td>{{ team.name }}</td>
          <td class="num">{{ team.wins }}</td>
          <td class="num">{{ team.losses }}</td>
          <td class="num">{{ team.confWins }}-{{ team.confLosses }}</td>
        </tr>
      </tbody>
    </table>

    <!-- Grouped-by-conference view -->
    <div v-else class="conf-groups">
      <section v-for="group in groupedByConference" :key="group.conference" class="conf-group">
        <header class="conf-group-header">
          <span class="conf-name">{{ group.conference }}</span>
          <span class="conf-count">{{ group.teams.length }} teams</span>
        </header>
        <table>
          <tbody>
            <tr v-for="(team, i) in group.teams" :key="team.name">
              <td class="num conf-pos">#{{ i + 1 }}</td>
              <td class="num overall" :class="{ top3: team.rank <= 3 }">{{ team.rank }}</td>
              <td>{{ team.name }}</td>
              <td class="num">{{ team.wins }}</td>
              <td class="num">{{ team.losses }}</td>
              <td class="num">{{ team.confWins }}-{{ team.confLosses }}</td>
            </tr>
          </tbody>
        </table>
      </section>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useSeasonStore } from '../stores/seasonStore'

const seasonStore = useSeasonStore()

const columns = [
  { key: 'rank', label: 'Rank', num: true },
  { key: 'name', label: 'Team', num: false },
  { key: 'wins', label: 'W', num: true },
  { key: 'losses', label: 'L', num: true },
  { key: 'conf', label: 'Conf', num: true }
]

const viewMode = ref('overall')
const selectedConference = ref('All')
const sortKey = ref('rank')
const sortDir = ref('asc')

const conferences = computed(() => {
  const set = new Set(seasonStore.rankings.map((t) => t.conference).filter(Boolean))
  return ['All', ...Array.from(set).sort((a, b) => a.localeCompare(b))]
})

// Each team's rank within its own conference, by overall rank.
const confRankMap = computed(() => {
  const counts = {}
  const map = {}
  const ordered = [...seasonStore.rankings].sort((a, b) => a.rank - b.rank)
  for (const t of ordered) {
    const c = t.conference || '—'
    counts[c] = (counts[c] || 0) + 1
    map[t.name] = counts[c]
  }
  return map
})

// Conferences as containers, each team list sorted by overall rank;
// conferences ordered by their strongest (lowest overall rank) member.
const groupedByConference = computed(() => {
  const groups = {}
  const ordered = [...seasonStore.rankings].sort((a, b) => a.rank - b.rank)
  for (const t of ordered) {
    const c = t.conference || '—'
    if (!groups[c]) groups[c] = []
    groups[c].push(t)
  }
  return Object.entries(groups)
    .map(([conference, teams]) => ({ conference, teams, topRank: teams[0].rank }))
    .sort((a, b) => a.topRank - b.topRank)
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
  if (key === 'name') return a.name.localeCompare(b.name)
  if (key === 'conf') return (a.confWins - b.confWins) || (b.confLosses - a.confLosses)
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
  gap: 1rem;
  flex-wrap: wrap;
  margin-bottom: 1rem;
}
.standings-header h3 { margin: 0; }

.header-controls { display: flex; align-items: center; gap: 1rem; flex-wrap: wrap; }

.view-toggle { display: flex; border: 1px solid #2e3348; border-radius: 6px; overflow: hidden; }
.view-toggle button {
  background: #242838;
  border: none;
  color: #8b8fa8;
  padding: 0.4rem 0.8rem;
  font-size: 0.8rem;
  cursor: pointer;
}
.view-toggle button.active { background: #4f8ff7; color: white; }

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

.rank-cell { display: flex; flex-direction: column; align-items: flex-end; gap: 0.1rem; }
.overall-rank { font-variant-numeric: tabular-nums; }
.overall-rank.top3 { color: #34d399; font-weight: 600; }
.conf-rank { font-size: 0.68rem; color: #8b8fa8; white-space: nowrap; }

/* Grouped view */
.conf-groups {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
  gap: 1rem;
}
.conf-group {
  background: #171a23;
  border: 1px solid #2e3348;
  border-radius: 8px;
  overflow: hidden;
}
.conf-group-header {
  display: flex;
  align-items: baseline;
  justify-content: space-between;
  padding: 0.6rem 0.8rem;
  background: #242838;
  border-bottom: 1px solid #2e3348;
}
.conf-name { font-weight: 600; color: #a78bfa; font-size: 0.9rem; }
.conf-count { font-size: 0.72rem; color: #8b8fa8; }
.conf-pos { color: #8b8fa8; font-size: 0.78rem; width: 2.5rem; }
.overall.top3 { color: #34d399; font-weight: 600; }
.conf-group td { padding: 0.4rem 0.8rem; font-size: 0.85rem; }
</style>
