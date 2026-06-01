import { defineStore } from 'pinia'
import { ref } from 'vue'
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'

const SPEED_MS = { Instant: 0, Fast: 50, Medium: 500, Slow: 2000 }
const MAX_RESULTS = 40

export const useSeasonStore = defineStore('season', () => {
  const seasonId = ref(null)
  const rankings = ref([])
  const currentWeek = ref(0)
  const totalWeeks = ref(0)
  const recentResults = ref([])
  const isFinished = ref(false)
  const connected = ref(false)
  const error = ref(null)

  let connection = null

  async function startSeason(speed = 'Medium') {
    reset()

    connection = new HubConnectionBuilder()
      .withUrl('/hubs/season')
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build()

    connection.on('SeasonCreated', (id) => {
      seasonId.value = id
    })

    connection.on('SeasonStarted', (e) => {
      totalWeeks.value = e.totalWeeks
      rankings.value = e.initialRankings ?? []
    })

    connection.on('SeasonDayCompleted', (e) => {
      const tagged = (e.results ?? []).map((r) => ({ ...r, gameDate: e.gameDate }))
      recentResults.value = [...tagged, ...recentResults.value].slice(0, MAX_RESULTS)
    })

    connection.on('SeasonWeekCompleted', (e) => {
      currentWeek.value = e.weekNumber
      rankings.value = e.rankings ?? []
    })

    connection.on('SeasonCompleted', (e) => {
      rankings.value = e.finalRankings ?? rankings.value
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
      await connection.invoke('StartSeason', speedInt)
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
    seasonId.value = null
    rankings.value = []
    currentWeek.value = 0
    totalWeeks.value = 0
    recentResults.value = []
    isFinished.value = false
    error.value = null
  }

  return {
    seasonId, rankings, currentWeek, totalWeeks,
    recentResults, isFinished, connected, error,
    startSeason, disconnect, reset
  }
})
