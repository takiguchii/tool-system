<template>
  <div class="w-64 bg-zinc-950 text-white min-h-screen border-r border-zinc-800 flex flex-col">
    <div class="p-6 border-b border-zinc-800">
      <h2 class="text-2xl font-extrabold tracking-tight">Fundição <span class="text-orange-500">Domínio</span></h2>
    </div>
    
    <nav class="flex-1 p-4 flex flex-col gap-2">
      <router-link to="/dashboard" class="p-3 rounded-lg bg-zinc-900 text-orange-500 flex items-center gap-3 transition-all border border-zinc-800 shadow-[0_0_10px_rgba(249,115,22,0.1)]">
        <span class="w-2 h-2 rounded-full bg-orange-500 animate-pulse"></span>
        Moldes e Machos
      </router-link>
      </nav>

    <div class="p-4 border-t border-zinc-800">
      <button @click="sairSistema" class="w-full text-left p-3 rounded-lg hover:bg-red-500/10 text-zinc-400 hover:text-red-400 transition-colors font-medium">
        Sair do Sistema
      </button>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'

const router = useRouter()

const sairSistema = () => {
  localStorage.removeItem('token')
  router.push('/')
}
</script>