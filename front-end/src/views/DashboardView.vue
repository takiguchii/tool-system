<template>
  <div class="flex min-h-screen bg-zinc-900 font-sans">
    
    <Sidebar />
    
    <main class="flex-1 p-10 overflow-y-auto">
      <header class="mb-10">
        <h1 class="text-4xl font-extrabold text-white mb-2">Painel de Ferramentas</h1>
        <p class="text-zinc-400">Gerencie o acervo físico da fundição com rapidez e segurança.</p>
      </header>
      
      <div class="bg-zinc-950 border border-zinc-800 rounded-xl p-8 shadow-2xl">
        <div class="flex items-center justify-center h-64 border-2 border-dashed border-zinc-800 rounded-lg">
          <p class="text-zinc-600 font-medium animate-pulse">Buscando ferramentas no banco de dados...</p>
        </div>
      </div>
    </main>

  </div>
</template>

<script setup>
import Sidebar from '../components/SidebarComponent.vue'
</script>