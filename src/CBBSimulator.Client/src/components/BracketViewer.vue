<template>
  <div class="bracket-viewer">
    <div v-if="!tournamentStore.bracket" class="empty">
      Bracket will appear here once the tournament starts...
    </div>

    <div v-else class="bracket-container" ref="containerRef">
      <div class="bracket-grid">

        <!-- Left regions: South (top), East (bottom) -->
        <div
          v-for="(region, idx) in leftRegions"
          :key="region.name"
          class="region region--left"
          :style="{ gridColumn: 1, gridRow: idx + 1 }"
        >
          <div class="region-label">{{ region.name }}</div>
          <div class="region-rounds">
            <div
              v-for="round in REGION_ROUNDS"
              :key="round"
              class="round-column"
            >
              <div
                v-for="(pair, pi) in toPairs(region.rounds[round])"
                :key="pi"
                class="matchup-pair"
                :class="{ 'last-round': round === 'Elite 8' }"
              >
                <div
                  v-for="(slot, si) in pair"
                  :key="si"
                  class="matchup"
                  :class="slotClasses(slot)"
                  :ref="el => tagCurrentRef(el, slot)"
                >
                  <div
                    class="matchup-team"
                    :class="teamClasses(slot, slot.team1)"
                  >
                    <span class="seed" v-if="slot.team1?.seed">{{ slot.team1.seed }}</span>
                    <span class="team-name" :title="slot.team1?.name">{{ slot.team1?.name || 'TBD' }}</span>
                    <span class="score" v-if="slot.result">{{ scoreFor(slot, slot.team1) }}</span>
                  </div>
                  <div
                    class="matchup-team"
                    :class="teamClasses(slot, slot.team2)"
                  >
                    <span class="seed" v-if="slot.team2?.seed">{{ slot.team2.seed }}</span>
                    <span class="team-name" :title="slot.team2?.name">{{ slot.team2?.name || 'TBD' }}</span>
                    <span class="score" v-if="slot.result">{{ scoreFor(slot, slot.team2) }}</span>
                  </div>
                  <span v-if="slot.result?.overtimes" class="ot-badge">
                    {{ slot.result.overtimes > 1 ? slot.result.overtimes + 'OT' : 'OT' }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Center: Final Four + Championship -->
        <div class="center-column">
          <div
            class="matchup ff-matchup"
            :class="slotClasses(bracketData.finalFour[0])"
            :ref="el => tagCurrentRef(el, bracketData.finalFour[0])"
          >
            <div class="matchup-team" :class="teamClasses(bracketData.finalFour[0], bracketData.finalFour[0].team1)">
              <span class="team-name" :title="bracketData.finalFour[0].team1?.name">{{ bracketData.finalFour[0].team1?.name || 'TBD' }}</span>
              <span class="score" v-if="bracketData.finalFour[0].result">{{ scoreFor(bracketData.finalFour[0], bracketData.finalFour[0].team1) }}</span>
            </div>
            <div class="matchup-team" :class="teamClasses(bracketData.finalFour[0], bracketData.finalFour[0].team2)">
              <span class="team-name" :title="bracketData.finalFour[0].team2?.name">{{ bracketData.finalFour[0].team2?.name || 'TBD' }}</span>
              <span class="score" v-if="bracketData.finalFour[0].result">{{ scoreFor(bracketData.finalFour[0], bracketData.finalFour[0].team2) }}</span>
            </div>
            <span v-if="bracketData.finalFour[0].result?.overtimes" class="ot-badge">
              {{ bracketData.finalFour[0].result.overtimes > 1 ? bracketData.finalFour[0].result.overtimes + 'OT' : 'OT' }}
            </span>
          </div>

          <div class="championship-area">
            <div class="ff-label">Final Four</div>
            <div
              class="matchup champ-matchup"
              :class="slotClasses(bracketData.championship)"
              :ref="el => tagCurrentRef(el, bracketData.championship)"
            >
              <div class="matchup-team" :class="teamClasses(bracketData.championship, bracketData.championship.team1)">
                <span class="team-name" :title="bracketData.championship.team1?.name">{{ bracketData.championship.team1?.name || 'TBD' }}</span>
                <span class="score" v-if="bracketData.championship.result">{{ scoreFor(bracketData.championship, bracketData.championship.team1) }}</span>
              </div>
              <div class="matchup-team" :class="teamClasses(bracketData.championship, bracketData.championship.team2)">
                <span class="team-name" :title="bracketData.championship.team2?.name">{{ bracketData.championship.team2?.name || 'TBD' }}</span>
                <span class="score" v-if="bracketData.championship.result">{{ scoreFor(bracketData.championship, bracketData.championship.team2) }}</span>
              </div>
              <span v-if="bracketData.championship.result?.overtimes" class="ot-badge">
                {{ bracketData.championship.result.overtimes > 1 ? bracketData.championship.result.overtimes + 'OT' : 'OT' }}
              </span>
            </div>
            <div v-if="tournamentStore.champion" class="champion-banner">
              Champion: {{ tournamentStore.champion }}
            </div>
          </div>

          <div
            class="matchup ff-matchup"
            :class="slotClasses(bracketData.finalFour[1])"
            :ref="el => tagCurrentRef(el, bracketData.finalFour[1])"
          >
            <div class="matchup-team" :class="teamClasses(bracketData.finalFour[1], bracketData.finalFour[1].team1)">
              <span class="team-name" :title="bracketData.finalFour[1].team1?.name">{{ bracketData.finalFour[1].team1?.name || 'TBD' }}</span>
              <span class="score" v-if="bracketData.finalFour[1].result">{{ scoreFor(bracketData.finalFour[1], bracketData.finalFour[1].team1) }}</span>
            </div>
            <div class="matchup-team" :class="teamClasses(bracketData.finalFour[1], bracketData.finalFour[1].team2)">
              <span class="team-name" :title="bracketData.finalFour[1].team2?.name">{{ bracketData.finalFour[1].team2?.name || 'TBD' }}</span>
              <span class="score" v-if="bracketData.finalFour[1].result">{{ scoreFor(bracketData.finalFour[1], bracketData.finalFour[1].team2) }}</span>
            </div>
            <span v-if="bracketData.finalFour[1].result?.overtimes" class="ot-badge">
              {{ bracketData.finalFour[1].result.overtimes > 1 ? bracketData.finalFour[1].result.overtimes + 'OT' : 'OT' }}
            </span>
          </div>
        </div>

        <!-- Right regions: West (top), Midwest (bottom) -->
        <div
          v-for="(region, idx) in rightRegions"
          :key="region.name"
          class="region region--right"
          :style="{ gridColumn: 3, gridRow: idx + 1 }"
        >
          <div class="region-label">{{ region.name }}</div>
          <div class="region-rounds">
            <div
              v-for="round in REGION_ROUNDS"
              :key="round"
              class="round-column"
            >
              <div
                v-for="(pair, pi) in toPairs(region.rounds[round])"
                :key="pi"
                class="matchup-pair"
                :class="{ 'last-round': round === 'Elite 8' }"
              >
                <div
                  v-for="(slot, si) in pair"
                  :key="si"
                  class="matchup"
                  :class="slotClasses(slot)"
                  :ref="el => tagCurrentRef(el, slot)"
                >
                  <div
                    class="matchup-team"
                    :class="teamClasses(slot, slot.team1)"
                  >
                    <span class="score score--left" v-if="slot.result">{{ scoreFor(slot, slot.team1) }}</span>
                    <span class="team-name" :title="slot.team1?.name">{{ slot.team1?.name || 'TBD' }}</span>
                    <span class="seed" v-if="slot.team1?.seed">{{ slot.team1.seed }}</span>
                  </div>
                  <div
                    class="matchup-team"
                    :class="teamClasses(slot, slot.team2)"
                  >
                    <span class="score score--left" v-if="slot.result">{{ scoreFor(slot, slot.team2) }}</span>
                    <span class="team-name" :title="slot.team2?.name">{{ slot.team2?.name || 'TBD' }}</span>
                    <span class="seed" v-if="slot.team2?.seed">{{ slot.team2.seed }}</span>
                  </div>
                  <span v-if="slot.result?.overtimes" class="ot-badge">
                    {{ slot.result.overtimes > 1 ? slot.result.overtimes + 'OT' : 'OT' }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, ref, watch, nextTick } from 'vue'
import { useTournamentStore } from '../stores/tournamentStore'

const tournamentStore = useTournamentStore()
const containerRef = ref(null)
const currentEl = ref(null)
const justCompletedKey = ref(null)

const REGION_ROUNDS = ['Round of 64', 'Round of 32', 'Sweet 16', 'Elite 8']
const FIRST_ROUND_MATCHUPS = [[1,16],[8,9],[5,12],[4,13],[6,11],[3,14],[7,10],[2,15]]

function toPairs(arr) {
  if (!arr) return []
  const pairs = []
  for (let i = 0; i < arr.length; i += 2) {
    pairs.push(arr.slice(i, Math.min(i + 2, arr.length)))
  }
  return pairs
}

function winnerOf(slot) {
  if (!slot?.result) return null
  const w = slot.result.winner
  if (slot.team1?.name === w) return { ...slot.team1 }
  if (slot.team2?.name === w) return { ...slot.team2 }
  return { name: w, seed: null }
}

function isCurrentGame(slot) {
  const cg = tournamentStore.currentGame
  if (!cg || !slot) return false
  if (slot.team1?.name === cg.awayTeam && slot.team2?.name === cg.homeTeam) return true
  if (slot.team1?.name === cg.homeTeam && slot.team2?.name === cg.awayTeam) return true
  return false
}

function scoreFor(slot, team) {
  if (!slot.result || !team) return ''
  if (team.name === slot.result.awayTeam) return slot.result.awayTeamScore
  if (team.name === slot.result.homeTeam) return slot.result.homeTeamScore
  return ''
}

const bracketData = computed(() => {
  if (!tournamentStore.bracket) return null

  const seedsByRegion = {}
  for (const region of tournamentStore.bracket.regions) {
    seedsByRegion[region.name] = {}
    for (const s of region.seeds) {
      seedsByRegion[region.name][s.seed] = s.teamName
    }
  }

  // Apply play-in winners to seed 16
  const firstFourGames = tournamentStore.completedGames.filter(g => g.round === 'First Four')
  for (const g of firstFourGames) {
    if (g.region && seedsByRegion[g.region]) {
      seedsByRegion[g.region][16] = g.result.winner
    }
  }

  const gameIndex = {}
  for (const game of tournamentStore.completedGames) {
    const key = `${game.region || ''}|${game.round}`
    if (!gameIndex[key]) gameIndex[key] = []
    gameIndex[key].push(game)
  }

  const regions = {}
  for (const regionName of ['South', 'East', 'West', 'Midwest']) {
    const rounds = {}
    const seeds = seedsByRegion[regionName] || {}

    rounds['Round of 64'] = FIRST_ROUND_MATCHUPS.map(([high, low], i) => {
      const games = gameIndex[`${regionName}|Round of 64`] || []
      const game = games[i] || null
      const slot = {
        team1: { name: seeds[high] || 'TBD', seed: high },
        team2: { name: seeds[low] || 'TBD', seed: low },
        result: game?.result || null
      }
      slot.isCurrent = isCurrentGame(slot)
      return slot
    })

    let prevRound = 'Round of 64'
    for (const roundName of ['Round of 32', 'Sweet 16', 'Elite 8']) {
      const prevSlots = rounds[prevRound]
      const slotCount = prevSlots.length / 2
      const games = gameIndex[`${regionName}|${roundName}`] || []

      rounds[roundName] = Array.from({ length: slotCount }, (_, i) => {
        const slot = {
          team1: winnerOf(prevSlots[i * 2]),
          team2: winnerOf(prevSlots[i * 2 + 1]),
          result: games[i]?.result || null
        }
        slot.isCurrent = isCurrentGame(slot)
        return slot
      })
      prevRound = roundName
    }

    regions[regionName] = { name: regionName, rounds }
  }

  const ffGames = gameIndex['|Final Four'] || []
  const finalFour = [
    (() => {
      const slot = {
        team1: winnerOf(regions['South'].rounds['Elite 8'][0]),
        team2: winnerOf(regions['East'].rounds['Elite 8'][0]),
        result: ffGames[0]?.result || null
      }
      slot.isCurrent = isCurrentGame(slot)
      return slot
    })(),
    (() => {
      const slot = {
        team1: winnerOf(regions['West'].rounds['Elite 8'][0]),
        team2: winnerOf(regions['Midwest'].rounds['Elite 8'][0]),
        result: ffGames[1]?.result || null
      }
      slot.isCurrent = isCurrentGame(slot)
      return slot
    })()
  ]

  const champGames = gameIndex['|Championship'] || []
  const championship = (() => {
    const slot = {
      team1: winnerOf(finalFour[0]),
      team2: winnerOf(finalFour[1]),
      result: champGames[0]?.result || null
    }
    slot.isCurrent = isCurrentGame(slot)
    return slot
  })()

  return { regions, finalFour, championship }
})

const leftRegions = computed(() => {
  if (!bracketData.value) return []
  return [
    bracketData.value.regions['South'],
    bracketData.value.regions['East']
  ]
})

const rightRegions = computed(() => {
  if (!bracketData.value) return []
  return [
    bracketData.value.regions['West'],
    bracketData.value.regions['Midwest']
  ]
})

function slotClasses(slot) {
  if (!slot) return {}
  const key = slot.result ? `${slot.result.awayTeam}|${slot.result.homeTeam}` : null
  return {
    'is-current': slot.isCurrent,
    'has-result': !!slot.result,
    'just-completed': key && justCompletedKey.value === key
  }
}

function teamClasses(slot, team) {
  if (!slot?.result || !team) return {}
  return {
    'is-winner': team.name === slot.result.winner,
    'is-loser': team.name !== slot.result.winner
  }
}

function tagCurrentRef(el, slot) {
  if (slot?.isCurrent && el) {
    currentEl.value = el
  }
}

watch(
  () => tournamentStore.completedGames.length,
  (newLen, oldLen) => {
    if (newLen > oldLen) {
      const game = tournamentStore.completedGames[newLen - 1]
      const key = `${game.result.awayTeam}|${game.result.homeTeam}`
      justCompletedKey.value = key
      setTimeout(() => { justCompletedKey.value = null }, 1500)
    }
  }
)

watch(
  () => tournamentStore.currentGame,
  async () => {
    await nextTick()
    if (currentEl.value) {
      currentEl.value.scrollIntoView({ behavior: 'smooth', block: 'nearest', inline: 'center' })
    }
  }
)
</script>

<style scoped>
.bracket-viewer {
  width: 100%;
}

.empty {
  text-align: center;
  color: #8b8fa8;
  font-style: italic;
  padding: 3rem 1rem;
}

.bracket-container {
  overflow-x: auto;
  padding: 1rem 0;
}

.bracket-grid {
  display: grid;
  grid-template-columns: 1fr auto 1fr;
  grid-template-rows: 1fr 1fr;
  min-width: 1400px;
  gap: 0;
}

/* --- Regions --- */
.region {
  padding: 0.5rem;
}

.region-label {
  text-align: center;
  font-size: 0.85rem;
  font-weight: 700;
  color: #a78bfa;
  text-transform: uppercase;
  letter-spacing: 0.1em;
  margin-bottom: 0.5rem;
}

.region-rounds {
  display: grid;
  grid-template-columns: repeat(4, minmax(140px, 1fr));
  align-items: stretch;
}

/* --- Round columns --- */
.round-column {
  display: flex;
  flex-direction: column;
  justify-content: space-around;
  min-height: 100%;
}

/* --- Matchup pairs --- */
.matchup-pair {
  display: flex;
  flex-direction: column;
  justify-content: center;
  position: relative;
  flex: 1;
}

/* --- Connector lines for LEFT regions --- */
.region--left .matchup-pair:not(.last-round)::after {
  content: '';
  position: absolute;
  right: -1px;
  top: 25%;
  height: 50%;
  width: 10px;
  border: 1.5px solid #3a3f58;
  border-left: none;
  border-radius: 0 4px 4px 0;
  pointer-events: none;
}

/* --- Connector lines for RIGHT regions --- */
.region--right .matchup-pair:not(.last-round)::after {
  content: '';
  position: absolute;
  left: -1px;
  top: 25%;
  height: 50%;
  width: 10px;
  border: 1.5px solid #3a3f58;
  border-right: none;
  border-radius: 4px 0 0 4px;
  pointer-events: none;
}

/* --- Matchup box --- */
.matchup {
  background: var(--surface, #1a1d27);
  border: 1.5px solid var(--border, #2e3348);
  border-radius: 6px;
  margin: 3px 6px;
  min-width: 130px;
  position: relative;
  transition: border-color 0.3s, box-shadow 0.3s, background 0.3s;
}

.matchup.is-current {
  border-color: #4f8ff7;
  box-shadow: 0 0 12px rgba(79, 143, 247, 0.3);
  animation: pulse-border 2s ease-in-out infinite;
}

.matchup.just-completed {
  animation: score-reveal 1.5s ease;
}

@keyframes pulse-border {
  0%, 100% { box-shadow: 0 0 8px rgba(79, 143, 247, 0.2); }
  50%      { box-shadow: 0 0 18px rgba(79, 143, 247, 0.5); }
}

@keyframes score-reveal {
  0%   { background: rgba(52, 211, 153, 0.25); }
  100% { background: var(--surface, #1a1d27); }
}

/* --- Team row inside matchup --- */
.matchup-team {
  display: flex;
  align-items: center;
  gap: 0.3rem;
  padding: 0.25rem 0.5rem;
  font-size: 0.72rem;
  color: #c8cad8;
  transition: opacity 0.3s;
}

.matchup-team:first-child {
  border-bottom: 1px solid #242838;
}

.matchup-team.is-winner {
  color: #34d399;
  font-weight: 700;
}

.matchup-team.is-loser {
  opacity: 0.45;
}

.seed {
  color: #8b8fa8;
  font-size: 0.65rem;
  min-width: 14px;
  text-align: center;
  font-weight: 600;
}

.team-name {
  flex: 1;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.score {
  font-weight: 700;
  font-variant-numeric: tabular-nums;
  min-width: 20px;
  text-align: right;
  color: #e1e4ed;
}

.score--left {
  text-align: left;
}

.ot-badge {
  position: absolute;
  top: -6px;
  right: -4px;
  font-size: 0.55rem;
  font-weight: 700;
  color: #f59e0b;
  background: #1a1d27;
  padding: 0 3px;
  border-radius: 3px;
  border: 1px solid #f59e0b;
}

/* --- Center column (Final Four + Championship) --- */
.center-column {
  grid-row: 1 / 3;
  grid-column: 2;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  gap: 1.5rem;
  padding: 1rem 1.5rem;
  min-width: 180px;
}

.ff-matchup {
  min-width: 160px;
}

.championship-area {
  text-align: center;
}

.ff-label {
  font-size: 0.75rem;
  font-weight: 700;
  color: #a78bfa;
  text-transform: uppercase;
  letter-spacing: 0.1em;
  margin-bottom: 0.5rem;
}

.champ-matchup {
  min-width: 170px;
  border-color: #f59e0b;
}

.champion-banner {
  margin-top: 1rem;
  font-size: 1.1rem;
  font-weight: 800;
  color: #f59e0b;
  padding: 0.6rem 1.2rem;
  background: rgba(245, 158, 11, 0.1);
  border: 2px solid #f59e0b;
  border-radius: 10px;
  animation: champion-glow 2s ease-in-out infinite alternate;
}

@keyframes champion-glow {
  from { box-shadow: 0 0 10px rgba(245, 158, 11, 0.2); }
  to   { box-shadow: 0 0 25px rgba(245, 158, 11, 0.5); }
}

/* --- Right-side regions: reverse round column order --- */
.region--right .region-rounds {
  direction: rtl;
}

.region--right .region-rounds > * {
  direction: ltr;
}

.region--right .matchup-team {
  flex-direction: row-reverse;
}

.region--right .matchup-team .team-name {
  text-align: right;
}
</style>
