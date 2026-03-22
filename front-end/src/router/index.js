import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import DashboardView from '../views/DashboardView.vue'
import EmpresasView from '../views/EmpresasView.vue' 

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'login',
      component: LoginView
    },
    {
      path: '/dashboard', 
      name: 'dashboard',
      component: DashboardView
    },
    {
      path: '/empresas', 
      name: 'empresas',
      component: EmpresasView
    }
  ]
})

export default router