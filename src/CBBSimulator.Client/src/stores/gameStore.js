import { defineStore } from 'pinia'
import { ref } from 'vue'
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'

const SPEED_MS = { Instant: 0, Fast: 50, Medium: 500, Slow: 2000 }

export const useGameStore = defineStore('game', () => {
  const gameId = ref(null)
  const away = ref(null)
  const home = ref(null)
  const awayScore = ref(0)
  const homeScore = ref(0)
  const awayFouls = ref(0)
  const homeFouls = ref(0)
  const clock = ref('20:00')
  const period = ref('1st Half')
  const homeHasPossession = ref(false)
  const possessions = ref([])
  const isFinished = ref(false)
  const result = ref(null)
  const speed = ref('Medium')
  const lastScoringTeam = ref(null)
  const error = ref(null)
  const connected = ref(false)

  let connection = null

  function applyScoreboard(sb) {
    if (!sb) return
    awayScore.value = sb.awayScore
    homeScore.value = sb.homeScore
    awayFouls.value = sb.awayFouls
    homeFouls.value = sb.homeFouls
    clock.value = sb.clock
    period.value = sb.period
    homeHasPossession.value = sb.homeHasPossession
  }

  async function startGame(awayTeam, homeTeam, simSpeed = 'Medium') {
    reset()
    away.value = awayTeam
    home.value = homeTeam
    speed.value = simSpeed

    connection = new HubConnectionBuilder()
      .withUrl('/hubs/game')
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build()

    connection.on('GameCreated', (id) => { gameId.value = id })

    connection.on('ScoreUpdate', (e) => {
      applyScoreboard(e.scoreboard)
      if (e.pointsScored > 0) {
        // Suffix with timestamp so consecutive scores by the same team
        // still trigger watchers (the value must change).
        lastScoringTeam.value = `${e.scoringTeam}#${Date.now()}`
      }
    })

    connection.on('ClockAdvanced',   (e) => applyScoreboard(e.scoreboard))
    connection.on('PeriodStarted',   (e) => applyScoreboard(e.scoreboard))
    connection.on('PeriodEnded',     (e) => applyScoreboard(e.scoreboard))
    connection.on('Halftime',        (e) => applyScoreboard(e.scoreboard))
    connection.on('OvertimeStarted', (e) => applyScoreboard(e.scoreboard))

    connection.on('PossessionResult', (e) => {
      possessions.value.push(e)
      applyScoreboard(e.scoreboard)
    })

    connection.on('GameOver', (e) => {
      result.value = e.result
      isFinished.value = true
      applyScoreboard({
        awayScore: e.result.awayTeamScore,
        homeScore: e.result.homeTeamScore,
        awayFouls: awayFouls.value,
        homeFouls: homeFouls.value,
        clock: '0:00',
        period: period.value,
        homeHasPossession: homeHasPossession.value
      })
    })

    connection.on('Error', (e) => { error.value = e.message })

    connection.onclose(() => { connected.value = false })
    connection.onreconnected(() => { connected.value = true })

    try {
      await connection.start()
      connected.value = true
      const speedInt = SPEED_MS[simSpeed] ?? SPEED_MS.Medium
      await connection.invoke('StartGame', awayTeam.name, homeTeam.name, speedInt)
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
    gameId.value = null
    awayScore.value = 0
    homeScore.value = 0
    awayFouls.value = 0
    homeFouls.value = 0
    clock.value = '20:00'
    period.value = '1st Half'
    homeHasPossession.value = false
    possessions.value = []
    isFinished.value = false
    result.value = null
    lastScoringTeam.value = null
    error.value = null
  }

  return {
    gameId, away, home, awayScore, homeScore,
    awayFouls, homeFouls, clock, period, homeHasPossession,
    possessions, isFinished, result, speed,
    lastScoringTeam, error, connected,
    startGame, disconnect, reset
  }
})
