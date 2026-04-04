import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useTournamentStore = defineStore('tournament', () => {
  const tournamentId = ref(null)
  const bracket = ref([])
  const currentGame = ref(null)
  const completedGames = ref([])
  const currentRound = ref('')
  const champion = ref(null)
  const isFinished = ref(false)

  function applyGameCompleted(result, round) {
    completedGames.value.push({ result, round })
    currentGame.value = null
  }

  function setChampion(team) {
    champion.value = team
    isFinished.value = true
  }

  function reset() {
    tournamentId.value = null
    bracket.value = []
    currentGame.value = null
    completedGames.value = []
    currentRound.value = ''
    champion.value = null
    isFinished.value = false
  }

  return {
    tournamentId, bracket, currentGame, completedGames,
    currentRound, champion, isFinished,
    applyGameCompleted, setChampion, reset
  }
})
