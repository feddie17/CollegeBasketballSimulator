import { defineStore } from 'pinia'
import { ref } from 'vue'
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'

const SPEED_MS = { Instant: 0, Fast: 50, Medium: 500, Slow: 2000 }
const MAX_RESULTS = 60

export const useSeasonStore = defineStore('season', () => {
  const seasonId = ref(null)
  const rankings = ref([])
  const currentWeek = ref(0)
  const totalWeeks = ref(0)
  const recentResults = ref([])
  const isFinished = ref(false)
  const connected = ref(false)
  const error = ref(null)
  // running: a "sim to end" loop is in flight. busy: a single day/week step is in flight.
  const running = ref(false)
  const busy = ref(false)

  let connection = null

  async function startSeason() {
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
      currentWeek.value = 0
      rankings.value = e.initialRankings ?? []
    })

    connection.on('SeasonDayCompleted', (e) => {
      currentWeek.value = e.weekNumber
      if (e.standings) rankings.value = e.standings
      const tagged = (e.results ?? []).map((r) => ({ ...r, gameDate: e.gameDate }))
      recentResults.value = [...tagged, ...recentResults.value].slice(0, MAX_RESULTS)
    })

    connection.on('SeasonWeekCompleted', (e) => {
      currentWeek.value = e.weekNumber
      if (e.rankings) rankings.value = e.rankings
    })

    connection.on('SeasonCompleted', (e) => {
      rankings.value = e.finalRankings ?? rankings.value
      isFinished.value = true
      running.value = false
    })

    connection.on('Error', (e) => {
      error.value = e.message
      running.value = false
      busy.value = false
    })

    connection.onclose(() => { connected.value = false })
    connection.onreconnected(() => { connected.value = true })

    try {
      await connection.start()
      connected.value = true
      await connection.invoke('StartSeason')
    } catch (err) {
      error.value = err.message ?? String(err)
      connected.value = false
    }
  }

  async function simulateDay() {
    if (!seasonId.value || busy.value || running.value) return
    busy.value = true
    try {
      await connection.invoke('SimulateDay', seasonId.value)
    } catch (err) {
      error.value = err.message ?? String(err)
    } finally {
      busy.value = false
    }
  }

  async function simulateWeek() {
    if (!seasonId.value || busy.value || running.value) return
    busy.value = true
    try {
      await connection.invoke('SimulateWeek', seasonId.value)
    } catch (err) {
      error.value = err.message ?? String(err)
    } finally {
      busy.value = false
    }
  }

  async function simulateToEnd(speed = 'Medium') {
    if (!seasonId.value || busy.value || running.value || isFinished.value) return
    running.value = true
    try {
      const speedInt = SPEED_MS[speed] ?? SPEED_MS.Medium
      await connection.invoke('SimulateToEnd', seasonId.value, speedInt)
    } catch (err) {
      error.value = err.message ?? String(err)
      running.value = false
    }
  }

  async function stop() {
    if (!seasonId.value) return
    running.value = false
    try {
      await connection.invoke('StopSeason', seasonId.value)
    } catch { /* ignore */ }
  }

  async function disconnect() {
    if (connection) {
      try { await connection.stop() } catch { /* ignore */ }
      connection = null
    }
    connected.value = false
    running.value = false
    busy.value = false
  }

  function reset() {
    seasonId.value = null
    rankings.value = []
    currentWeek.value = 0
    totalWeeks.value = 0
    recentResults.value = []
    isFinished.value = false
    running.value = false
    busy.value = false
    error.value = null
  }

  return {
    seasonId, rankings, currentWeek, totalWeeks, recentResults,
    isFinished, connected, error, running, busy,
    startSeason, simulateDay, simulateWeek, simulateToEnd, stop, disconnect, reset
  }
})
