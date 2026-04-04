<template>
  <div class="team-selector">
    <label>{{ label }}</label>
    <input
      type="text"
      v-model="query"
      @input="onInput"
      :placeholder="`Search for a team...`"
    />
    <ul v-if="results.length" class="dropdown">
      <li v-for="team in results" :key="team.name" @click="select(team)">
        #{{ team.rank }} {{ team.name }} ({{ team.conference }})
      </li>
    </ul>
    <div v-if="selected" class="selected">
      #{{ selected.rank }} {{ selected.name }}
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useTeamStore } from '../stores/teamStore'

const props = defineProps({ label: String })
const emit = defineEmits(['select'])

const teamStore = useTeamStore()
const query = ref('')
const results = ref([])
const selected = ref(null)

let debounceTimer = null

function onInput() {
  clearTimeout(debounceTimer)
  debounceTimer = setTimeout(async () => {
    if (query.value.length >= 2) {
      results.value = await teamStore.searchTeams(query.value)
    } else {
      results.value = []
    }
  }, 300)
}

function select(team) {
  selected.value = team
  query.value = team.name
  results.value = []
  emit('select', team)
}
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

.dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  background: #242838;
  border: 1px solid #2e3348;
  border-radius: 0 0 6px 6px;
  list-style: none;
  padding: 0;
  max-height: 200px;
  overflow-y: auto;
  z-index: 10;
}

.dropdown li {
  padding: 0.5rem 0.6rem;
  cursor: pointer;
  font-size: 0.85rem;
}

.dropdown li:hover { background: #2e3348; }

.selected {
  margin-top: 0.3rem;
  font-size: 0.8rem;
  color: #4f8ff7;
}
</style>
