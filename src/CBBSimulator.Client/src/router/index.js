import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', name: 'home', component: HomeView },
    { path: '/game', name: 'game', component: () => import('../views/GameView.vue') },
    { path: '/game/:id', name: 'game-spectate', component: () => import('../views/GameView.vue') },
    { path: '/tournament', name: 'tournament', component: () => import('../views/TournamentView.vue') },
    { path: '/season', name: 'season', component: () => import('../views/SeasonView.vue') },
  ]
})

export default router
