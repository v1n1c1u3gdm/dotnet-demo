import Vue from 'vue'
import Router from 'vue-router'

import HomeView from '@/views/HomeView.vue'
import ArticleView from '@/views/ArticleView.vue'
import AboutView from '@/views/AboutView.vue'
import AdminLoginView from '@/views/admin/AdminLoginView.vue'
import AdminDashboardView from '@/views/admin/AdminDashboardView.vue'
import { isAuthenticated } from '@/services/authService'

Vue.use(Router)

const router = new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/articles/:slug?',
      name: 'article',
      component: ArticleView
    },
    {
      path: '/about',
      name: 'about',
      component: AboutView
    },
    {
      path: '/admin/login',
      name: 'admin-login',
      component: AdminLoginView
    },
    {
      path: '/admin',
      name: 'admin-dashboard',
      component: AdminDashboardView,
      meta: {
        requiresAuth: true
      }
    }
  ]
})

router.beforeEach((to, from, next) => {
  if (to.matched.some(record => record.meta?.requiresAuth)) {
    if (!isAuthenticated()) {
      next({
        name: 'admin-login',
        query: { redirect: to.fullPath }
      })
      return
    }
  }

  if (to.name === 'admin-login' && isAuthenticated()) {
    next({ name: 'admin-dashboard' })
    return
  }

  next()
})

export default router

