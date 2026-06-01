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

  function buildConnection() {
    const conn = new HubConnectionBuilder()
      .withUrl('/hubs/tournament')
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build()

    conn.on('TournamentCreated', (id) => { tournamentId.value = id })
    conn.on('BracketBuilt', (e) => { bracket.value = e.bracket })
    conn.on('GameStarted', (e) => {
      currentGame.value = e.gameInfo
      currentRound.value = e.gameInfo.round
    })
    conn.on('GameCompleted', (e) => {
      completedGames.value.push({ result: e.result, round: e.round, region: e.region })
      currentGame.value = null
    })
    conn.on('RoundCompleted', (e) => {
      roundResults.value.push({ round: e.round, results: e.results })
    })
    conn.on('TournamentCompleted', (e) => {
      champion.value = e.champion
      isFinished.value = true
    })
    conn.on('Error', (e) => { error.value = e.message })
    conn.onclose(() => { connected.value = false })
    conn.onreconnected(() => { connected.value = true })

    return conn
  }

  async function startTournament(speed = 'Medium') {
    reset()
    connection = buildConnection()
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

  // Seed a tournament from an ordered list of team names (e.g. a completed
  // season's final standings). The backend preserves the order as seed order.
  async function startTournamentFromTeams(teamNames, speed = 'Medium') {
    reset()
    connection = buildConnection()
    try {
      await connection.start()
      connected.value = true
      const speedInt = SPEED_MS[speed] ?? SPEED_MS.Medium
      await connection.invoke('StartSeededTournament', JSON.stringify(teamNames), speedInt)
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
    startTournament, startTournamentFromTeams, disconnect, reset
  }
})
