import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/LoginView.vue'
import DashboardView from '../views/DashboardView.vue'
import EmpresasView from '../views/EmpresasView.vue'
import CategoriasView from '../views/CategoriasView.vue' // <-- 1. Importação da tela

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
    },
    {
      path: '/categorias', 
      name: 'categorias',
      component: CategoriasView
    }
  ]
})

export default router