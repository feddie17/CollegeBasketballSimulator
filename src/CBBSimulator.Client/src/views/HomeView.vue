<template>
  <div class="home">
    <h1>College Basketball Simulator</h1>
    <p class="subtitle">Real-time game simulation powered by Bart Torvik data</p>

    <div class="mode-grid">
      <router-link to="/game" class="mode-card">
        <h2>Single Game</h2>
        <p>Pick two teams and watch a game unfold possession by possession</p>
      </router-link>

      <router-link to="/tournament" class="mode-card">
        <h2>Tournament</h2>
        <p>Simulate a full March Madness bracket game by game</p>
      </router-link>

      <router-link to="/season" class="mode-card">
        <h2>Season</h2>
        <p>Simulate a full conference or national schedule with rankings</p>
      </router-link>
    </div>
  </div>
</template>

<script setup>
</script>

<style scoped>
.home { text-align: center; padding-top: 3rem; }
.home h1 { font-size: 2.5rem; margin-bottom: 0.5rem; }
.subtitle { color: #8b8fa8; margin-bottom: 3rem; }

.mode-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 1.5rem;
  max-width: 900px;
  margin: 0 auto;
}

.mode-card {
  background: #1a1d27;
  border: 1px solid #2e3348;
  border-radius: 12px;
  padding: 2rem;
  text-decoration: none;
  color: inherit;
  transition: border-color 0.2s;
}

.mode-card:hover {
  border-color: #4f8ff7;
}

.mode-card h2 {
  color: #4f8ff7;
  margin-bottom: 0.5rem;
}

.mode-card p {
  color: #8b8fa8;
  font-size: 0.9rem;
}
</style>
