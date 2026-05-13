import { defineStore } from 'pinia'
import { ref } from 'vue'
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'

const SPEED_MS = { Instant: 0, Fast: 50, Medium: 500, Slow: 2000 }

export const useTournamentStore = defineStore('tournament', () => {
  const tournamentId = ref(null)
  const bracket = ref(null)
  const currentGame = ref(null)
  const completedGames = ref([])
  const currentRound = ref('')
  const roundResults = ref([])
  const champion = ref(null)
  const isFinished = ref(false)
  const connected = ref(false)
  const error = ref(null)

  let connection = null

  async function startTournament(speed = 'Medium') {
    reset()

    connection = new HubConnectionBuilder()
      .withUrl('/hubs/tournament')
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build()

    connection.on('TournamentCreated', (id) => {
      tournamentId.value = id
    })

    connection.on('BracketBuilt', (e) => {
      bracket.value = e.bracket
    })

    connection.on('GameStarted', (e) => {
      currentGame.value = e.gameInfo
      currentRound.value = e.gameInfo.round
    })

    connection.on('GameCompleted', (e) => {
      completedGames.value.push({ result: e.result, round: e.round, region: e.region })
      currentGame.value = null
    })

    connection.on('RoundCompleted', (e) => {
      roundResults.value.push({ round: e.round, results: e.results })
    })

    connection.on('TournamentCompleted', (e) => {
      champion.value = e.champion
      isFinished.value = true
    })

    connection.on('Error', (e) => {
      error.value = e.message
    })

    connection.onclose(() => { connected.value = false })
    connection.onreconnected(() => { connected.value = true })

    try {
      await connection.start()
      connected.value = true
      const speedInt = SPEED_MS[speed] ?? SPEED_MS.Medium
      await connection.invoke('StartTournament', speedInt)
    } catch (err) {
      error.value = err.message ?? String(err)
      connected.value = false
    }
  }

  async function disconnect() {
    if (connection) {
      try { await connection.stop() } catch { /* ignore */ }
      connection = null
    }
    connected.value = false
  }

  function reset() {
    tournamentId.value = null
    bracket.value = null
    currentGame.value = null
    completedGames.value = []
    currentRound.value = ''
    roundResults.value = []
    champion.value = null
    isFinished.value = false
    error.value = null
  }

  return {
    tournamentId, bracket, currentGame, completedGames,
    currentRound, roundResults, champion, isFinished,
    connected, error,
    startTournament, disconnect, reset
  }
})
