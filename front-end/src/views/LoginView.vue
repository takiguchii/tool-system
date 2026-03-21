<template>
  <div class="min-h-screen flex items-center justify-center bg-zinc-950 relative overflow-hidden">
    
    <div class="absolute top-[-10%] left-[-10%] w-96 h-96 bg-orange-600 rounded-full mix-blend-multiply filter blur-[128px] opacity-20 animate-pulse"></div>

    <div class="bg-zinc-900 p-8 rounded-xl shadow-2xl w-full max-w-sm border-t-4 border-orange-500 z-10 transform transition-all duration-300 hover:scale-[1.02]">
      
      <h1 class="text-3xl font-extrabold text-center text-white mb-1">Fundição <span class="text-orange-500">Domínio</span></h1>
      <p class="text-zinc-500 text-center text-sm mb-8 italic">"Entender para atender"</p>

      <form @submit.prevent="fazerLogin" class="flex flex-col gap-5">
        <div>
          <label class="block text-zinc-400 font-medium mb-1 text-sm">E-mail</label>
          <input type="email" v-model="email" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-3 focus:outline-none focus:border-orange-500 focus:ring-1 focus:ring-orange-500 transition-colors" required>
        </div>

        <div>
          <label class="block text-zinc-400 font-medium mb-1 text-sm">Senha</label>
          <input type="password" v-model="senha" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-3 focus:outline-none focus:border-orange-500 focus:ring-1 focus:ring-orange-500 transition-colors" required>
        </div>

        <p v-if="erro" class="text-red-500 text-sm text-center animate-bounce">{{ erro }}</p>

        <button type="submit" :disabled="carregando" class="w-full bg-orange-500 text-white font-bold py-3 rounded-lg hover:bg-orange-600 active:scale-95 transition-all shadow-[0_0_15px_rgba(249,115,22,0.3)] mt-2 flex justify-center items-center h-12">
          <span v-if="carregando" class="animate-spin h-5 w-5 border-2 border-white border-t-transparent rounded-full"></span>
          <span v-else>Acessar Sistema</span>
        </button>
      </form>

    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import axios from 'axios'

const email = ref('')
const senha = ref('')
const erro = ref('')
const carregando = ref(false)

const fazerLogin = async () => {
  erro.value = ''
  carregando.value = true

  try {
    const resposta = await axios.post('http://localhost:5183/api/Auth/login', {
      email: email.value,
      senha: senha.value
    })

    localStorage.setItem('token', resposta.data.token)
    
    alert("Crachá JWT recebido com sucesso! Acesso Liberado.")

  } catch (e) {
    erro.value = 'E-mail ou senha incorretos.'
  } finally {
    carregando.value = false
  }
}
</script>