<template>
  <div class="relative flex min-h-screen bg-zinc-900 font-sans">
    
    <SidebarComponent :is-mobile-open="isSidebarOpen" @close="isSidebarOpen = false" />
    
    <div class="flex-1 flex flex-col">
      <header class="p-4 md:p-10 flex items-center justify-between md:justify-end">
        <!-- Hamburger button, only visible on mobile -->
        <button @click="isSidebarOpen = true" class="md:hidden p-2 rounded-lg bg-zinc-800 text-white">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16m-7 6h7"></path></svg>
        </button>
        <!-- A placeholder or other header content for desktop can go here -->
        <div class="hidden md:block">
          <!-- Ex: User menu, notifications, etc. -->
        </div>
      </header>
      
      <main class="flex-1 p-4 md:px-10">
        <div class="mb-10">
          <h1 class="text-3xl md:text-4xl font-extrabold text-white mb-2">Painel de Ferramentas</h1>
          <p class="text-zinc-400">Gerencie o acervo físico da fundição com rapidez e segurança.</p>
        </div>
        
        <TabelaMoldesComponent />
        
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import SidebarComponent from '../components/SidebarComponent.vue'
import TabelaMoldesComponent from '../components/TabelaMoldesComponent.vue'

const isSidebarOpen = ref(false)
</script>