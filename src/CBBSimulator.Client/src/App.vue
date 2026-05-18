<template>
  <div id="app">
    <nav>
      <router-link to="/">Home</router-link>
      <router-link to="/game">Game</router-link>
      <router-link to="/tournament">Tournament</router-link>
      <router-link to="/season">Season</router-link>
    </nav>
    <main>
      <router-view />
    </main>
  </div>
</template>

<script setup>
</script>

<style>
:root {
  --bg: #0f1117;
  --surface: #1a1d27;
  --border: #2e3348;
  --text: #e1e4ed;
  --accent: #4f8ff7;
}

* { margin: 0; padding: 0; box-sizing: border-box; }

body {
  font-family: 'Segoe UI', -apple-system, sans-serif;
  background: var(--bg);
  color: var(--text);
}

#app {
  min-height: 100vh;
}

nav {
  display: flex;
  gap: 1.5rem;
  padding: 1rem 2rem;
  background: var(--surface);
  border-bottom: 1px solid var(--border);
}

nav a {
  color: var(--text);
  text-decoration: none;
  font-weight: 500;
}

nav a.router-link-active {
  color: var(--accent);
}

main {
  padding: 2rem;
}
</style>
