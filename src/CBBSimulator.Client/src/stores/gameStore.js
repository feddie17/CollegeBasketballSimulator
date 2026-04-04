import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useGameStore = defineStore('game', () => {
  const gameId = ref(null)
  const away = ref(null)
  const home = ref(null)
  const awayScore = ref(0)
  const homeScore = ref(0)
  const clock = ref('20:00')
  const period = ref('1st Half')
  const possessions = ref([])
  const isFinished = ref(false)
  const result = ref(null)
  const speed = ref('medium')

  function applyPossession(event) {
    possessions.value.push(event)
    awayScore.value = event.scoreboard.awayScore
    homeScore.value = event.scoreboard.homeScore
    clock.value = event.scoreboard.clock
    period.value = event.scoreboard.period
  }

  function applyResult(matchup) {
    result.value = matchup
    isFinished.value = true
  }

  function reset() {
    gameId.value = null
    awayScore.value = 0
    homeScore.value = 0
    clock.value = '20:00'
    period.value = '1st Half'
    possessions.value = []
    isFinished.value = false
    result.value = null
  }

  return {
    gameId, away, home, awayScore, homeScore, clock, period,
    possessions, isFinished, result, speed,
    applyPossession, applyResult, reset
  }
})
