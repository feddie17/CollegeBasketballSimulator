import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useTeamStore = defineStore('teams', () => {
  const teams = ref([])
  const loading = ref(false)
  const error = ref(null)

  async function fetchTeams() {
    loading.value = true
    try {
      const res = await fetch('/api/teams')
      teams.value = await res.json()
    } catch (err) {
      error.value = err.message
    } finally {
      loading.value = false
    }
  }

  async function searchTeams(query) {
    if (!query || query.length < 2) return []
    const res = await fetch(`/api/teams/search?q=${encodeURIComponent(query)}`)
    return await res.json()
  }

  return { teams, loading, error, fetchTeams, searchTeams }
})
