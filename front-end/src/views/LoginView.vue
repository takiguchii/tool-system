<template>
  <div class="min-h-screen flex items-center justify-center bg-slate-200">
    <div class="bg-white p-8 rounded-lg shadow-lg w-96 border-t-4 border-blue-600">
      <h1 class="text-2xl font-extrabold text-center text-slate-800 mb-6">Fundição Sistema</h1>

      <form @submit.prevent="fazerLogin" class="flex flex-col gap-4">
        <div>
          <label class="block text-slate-600 font-bold mb-1">E-mail</label>
          <input type="email" v-model="email" class="w-full border border-slate-300 rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500" required>
        </div>

        <div>
          <label class="block text-slate-600 font-bold mb-1">Senha</label>
          <input type="password" v-model="senha" class="w-full border border-slate-300 rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500" required>
        </div>

        <button type="submit" class="bg-blue-600 text-white font-bold py-2 rounded hover:bg-blue-700 transition mt-4">
          Acessar Painel
        </button>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const email = ref('')
const senha = ref('')

const fazerLogin = () => {
  // Apenas anota no painel invisível por enquanto
  console.log("Tentando logar com:", email.value, senha.value)
}
</script>