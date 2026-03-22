<template>
  <div class="contents">
    <div v-if="aberto" @click="$emit('fechar')" class="fixed inset-0 bg-black/80 z-40 md:hidden transition-opacity"></div>
    <aside :class="[
      'fixed md:static top-0 left-0 z-50 h-screen bg-zinc-950 border-zinc-800 transition-all duration-300 ease-in-out flex flex-col shrink-0 overflow-hidden',
      aberto ? 'w-64 translate-x-0 border-r' : 'w-64 -translate-x-full border-none md:w-0 md:-translate-x-full'
    ]">
      <div class="w-64 flex flex-col h-full">
        
        <div class="p-6 border-b border-zinc-800 flex justify-between items-center">
          <h2 class="text-2xl font-extrabold tracking-tight text-white">Fundição <span class="text-orange-500">Domínio</span></h2>
          <button @click="$emit('fechar')" class="md:hidden text-zinc-500 hover:text-orange-500 text-xl font-bold">X</button>
        </div>
        
        <nav class="flex-1 p-4 flex flex-col gap-2">
          
          <router-link to="/dashboard" active-class="bg-zinc-900 text-orange-500 border-zinc-800 shadow-[0_0_10px_rgba(249,115,22,0.1)]" class="p-3 rounded-lg text-zinc-400 hover:text-orange-500 hover:bg-zinc-900/50 flex items-center gap-3 transition-all border border-transparent font-medium">
            Moldes
          </router-link>

          <router-link to="/machos" active-class="bg-zinc-900 text-orange-500 border-zinc-800 shadow-[0_0_10px_rgba(249,115,22,0.1)]" class="p-3 rounded-lg text-zinc-400 hover:text-orange-500 hover:bg-zinc-900/50 flex items-center gap-3 transition-all border border-transparent font-medium">
            Machos
          </router-link>

          <router-link to="/categorias" active-class="bg-zinc-900 text-orange-500 border-zinc-800 shadow-[0_0_10px_rgba(249,115,22,0.1)]" class="p-3 rounded-lg text-zinc-400 hover:text-orange-500 hover:bg-zinc-900/50 flex items-center gap-3 transition-all border border-transparent font-medium">
            Categorias
          </router-link>

          <router-link to="/empresas" active-class="bg-zinc-900 text-orange-500 border-zinc-800 shadow-[0_0_10px_rgba(249,115,22,0.1)]" class="p-3 rounded-lg text-zinc-400 hover:text-orange-500 hover:bg-zinc-900/50 flex items-center gap-3 transition-all border border-transparent font-medium">
            Empresas
          </router-link>

        </nav>

        <div class="p-4 border-t border-zinc-800">
          <button @click="sairSistema" class="w-full text-left p-3 rounded-lg hover:bg-red-500/10 text-zinc-400 hover:text-red-400 transition-colors font-medium flex items-center gap-3">
            Sair do Sistema
          </button>
        </div>
        
      </div>
    </aside>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'

defineProps({
  aberto: Boolean
})

defineEmits(['fechar'])

const router = useRouter()

const sairSistema = () => {
  localStorage.removeItem('token')
  router.push('/')
}
</script>