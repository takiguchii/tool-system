<template>
  <div class="flex min-h-screen bg-zinc-900 font-sans">
    
    <SidebarComponent />
    
    <main class="flex-1 p-10 overflow-y-auto">
      <header class="mb-10">
        <h1 class="text-4xl font-extrabold text-white mb-2">Painel de Ferramentas</h1>
        <p class="text-zinc-400">Gerencie o acervo físico da fundição com rapidez e segurança.</p>
      </header>
      
      <TabelaMoldesComponent />
      
    </main>
  </div>
</template>

<script setup>
import SidebarComponent from '../components/SidebarComponent.vue'
import TabelaMoldesComponent from '../components/TabelaMoldesComponent.vue' // <-- Chave de fenda correta
</script>