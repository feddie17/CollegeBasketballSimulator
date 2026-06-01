<template>
  <div class="team-selector" ref="rootEl">
    <label :for="inputId">{{ label }}</label>
    <input
      :id="inputId"
      type="text"
      v-model="query"
      @input="onInput"
      @keydown="onKeydown"
      @focus="open = results.length > 0"
      :placeholder="`Search for a team...`"
      autocomplete="off"
      role="combobox"
      :aria-expanded="open"
      :aria-controls="listboxId"
      :aria-activedescendant="activeId"
    />

    <div v-if="loading" class="status">Searching…</div>

    <ul
      v-if="open && results.length"
      :id="listboxId"
      class="dropdown"
      role="listbox"
    >
      <li
        v-for="(team, i) in results"
        :key="team.name"
        :id="`${listboxId}-opt-${i}`"
        :class="{ active: i === activeIndex }"
        role="option"
        :aria-selected="i === activeIndex"
        @mousedown.prevent="select(team)"
        @mouseenter="activeIndex = i"
      >
        <span class="rank">#{{ team.rank }}</span>
        <span class="name">{{ team.name }}</span>
        <span class="meta">
          <span class="conf">{{ team.conference }}</span>
          <span v-if="team.record" class="record">{{ team.record }}</span>
          <span v-if="team.sos" class="sos">SOS {{ formatSos(team.sos) }}</span>
        </span>
      </li>
    </ul>

    <div
      v-else-if="open && !loading && query.length >= 2 && !results.length"
      class="status empty"
    >
      No teams match "{{ query }}"
    </div>

    <div v-if="selected" class="selected">
      Selected: #{{ selected.rank }} {{ selected.name }}
      <span v-if="selected.conference" class="selected-meta">
        {{ selected.conference }}<template v-if="selected.record"> · {{ selected.record }}</template><template v-if="selected.sos"> · SOS {{ formatSos(selected.sos) }}</template>
      </span>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { useTeamStore } from '../stores/teamStore'

const props = defineProps({ label: String })
const emit = defineEmits(['select'])

const teamStore = useTeamStore()
const query = ref('')
const results = ref([])
const selected = ref(null)
const open = ref(false)
const loading = ref(false)
const activeIndex = ref(-1)
const rootEl = ref(null)

function formatSos(sos) {
  return Number(sos).toFixed(3)
}

const uid = Math.random().toString(36).slice(2, 8)
const inputId = `team-input-${uid}`
const listboxId = `team-list-${uid}`
const activeId = computed(() =>
  activeIndex.value >= 0 ? `${listboxId}-opt-${activeIndex.value}` : null
)

let debounceTimer = null
let activeRequestToken = 0

async function onInput() {
  selected.value = null
  clearTimeout(debounceTimer)

  if (query.value.length < 2) {
    results.value = []
    open.value = false
    loading.value = false
    return
  }

  debounceTimer = setTimeout(async () => {
    const myToken = ++activeRequestToken
    loading.value = true
    try {
      const data = await teamStore.searchTeams(query.value)
      if (myToken !== activeRequestToken) return
      results.value = data
      activeIndex.value = data.length ? 0 : -1
      open.value = true
    } finally {
      if (myToken === activeRequestToken) loading.value = false
    }
  }, 300)
}

function onKeydown(e) {
  if (!open.value && (e.key === 'ArrowDown' || e.key === 'ArrowUp')) {
    if (results.value.length) open.value = true
    return
  }
  if (e.key === 'ArrowDown') {
    e.preventDefault()
    activeIndex.value = (activeIndex.value + 1) % results.value.length
  } else if (e.key === 'ArrowUp') {
    e.preventDefault()
    activeIndex.value =
      (activeIndex.value - 1 + results.value.length) % results.value.length
  } else if (e.key === 'Enter') {
    if (activeIndex.value >= 0 && results.value[activeIndex.value]) {
      e.preventDefault()
      select(results.value[activeIndex.value])
    }
  } else if (e.key === 'Escape') {
    open.value = false
    activeIndex.value = -1
  }
}

function select(team) {
  selected.value = team
  query.value = team.name
  results.value = []
  open.value = false
  activeIndex.value = -1
  emit('select', team)
}

function onDocumentClick(e) {
  if (rootEl.value && !rootEl.value.contains(e.target)) {
    open.value = false
  }
}

onMounted(() => document.addEventListener('mousedown', onDocumentClick))
onUnmounted(() => document.removeEventListener('mousedown', onDocumentClick))
</script>

<style scoped>
.team-selector { position: relative; min-width: 250px; }
label { display: block; margin-bottom: 0.3rem; font-weight: 600; font-size: 0.85rem; }

input {
  width: 100%;
  padding: 0.6rem;
  background: #242838;
  border: 1px solid #2e3348;
  border-radius: 6px;
  color: #e1e4ed;
  font-size: 0.9rem;
}
input:focus { outline: 2px solid #4f8ff7; border-color: #4f8ff7; }

.dropdown {
  position: absolute;
  top: 100%; left: 0; right: 0;
  background: #242838;
  border: 1px solid #2e3348;
  border-radius: 0 0 6px 6px;
  list-style: none;
  padding: 0;
  margin: 0;
  max-height: 240px;
  overflow-y: auto;
  z-index: 10;
}

.dropdown li {
  padding: 0.5rem 0.6rem;
  cursor: pointer;
  font-size: 0.85rem;
  display: grid;
  grid-template-columns: 2.5rem 1fr auto;
  gap: 0.5rem;
  align-items: center;
}

.dropdown li.active,
.dropdown li:hover { background: #2e3348; }

.rank { color: #8b8fa8; font-variant-numeric: tabular-nums; }
.name { color: #e1e4ed; }
.meta { display: flex; align-items: center; gap: 0.5rem; justify-self: end; }
.conf { color: #8b8fa8; font-size: 0.78rem; }
.record { color: #34d399; font-size: 0.78rem; font-variant-numeric: tabular-nums; }
.sos { color: #8b8fa8; font-size: 0.72rem; font-variant-numeric: tabular-nums; }

.selected-meta { color: #8b8fa8; }

.status {
  position: absolute;
  top: 100%; left: 0; right: 0;
  padding: 0.5rem 0.6rem;
  background: #242838;
  border: 1px solid #2e3348;
  border-radius: 0 0 6px 6px;
  font-size: 0.82rem;
  color: #8b8fa8;
  z-index: 10;
}

.selected {
  margin-top: 0.3rem;
  font-size: 0.8rem;
  color: #4f8ff7;
}
</style>
