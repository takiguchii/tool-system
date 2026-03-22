<template>
  <div class="flex h-screen bg-zinc-900 font-sans overflow-hidden">
    
    <SidebarComponent :aberto="menuAberto" @fechar="menuAberto = false" />
    
    <main class="flex-1 flex flex-col min-w-0 h-full">
      <header class="bg-zinc-950 border-b border-zinc-800 p-4 flex items-center shrink-0 shadow-sm z-10">
        <button @click="menuAberto = !menuAberto" class="text-zinc-400 hover:text-orange-500 p-2 rounded-lg hover:bg-zinc-900 transition-colors mr-4 md:hidden">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"></path></svg>
        </button>
        <h1 class="text-xl md:text-2xl font-extrabold text-white truncate">Gestão de Machos</h1>
      </header>
      
      <div class="p-4 md:p-10 overflow-y-auto flex-1">
        <header class="mb-8 hidden md:block">
          <p class="text-zinc-400">Cadastre, localize e anexe fotos aos machos da fundição.</p>
        </header>
        
        <TabelaMachosComponent />
      </div>
    </main>

  </div>
</template>

<script setup>
import { ref } from 'vue'
import SidebarComponent from '../components/SidebarComponent.vue'
import TabelaMachosComponent from '../components/TabelaMachosComponent.vue'

const menuAberto = ref(window.innerWidth > 768)
</script>