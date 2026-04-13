<template>
  <div class="fixed inset-0 bg-black/80 flex items-center justify-center z-50 p-4">
    <div class="bg-zinc-900 border border-zinc-700 rounded-xl p-8 w-full max-w-md shadow-2xl relative">
      <h2 class="text-2xl font-bold text-white mb-6">Editar Empresa</h2>
      
      <form @submit.prevent="salvarEdicao" class="flex flex-col gap-4">
        <div>
          <label class="block text-zinc-400 font-medium mb-1 text-sm">Nome da Empresa</label>
          <input type="text" v-model="empresaEditada.nome" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2 focus:outline-none focus:border-blue-500 focus:ring-1 focus:ring-blue-500" required>
        </div>

        <div>
          <label class="block text-zinc-400 font-medium mb-1 text-sm">Cidade</label>
          <input type="text" v-model="empresaEditada.cidade" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2 focus:outline-none focus:border-blue-500 focus:ring-1 focus:ring-blue-500">
        </div>

        <p v-if="erro" class="text-red-500 text-sm mt-2">{{ erro }}</p>

        <div class="flex justify-end gap-3 mt-6">
          <button type="button" @click="$emit('fechar')" class="px-4 py-2 rounded-lg text-zinc-400 hover:text-white hover:bg-zinc-800 transition-colors font-medium">
            Cancelar
          </button>
          <button type="submit" :disabled="carregando" class="bg-blue-600 hover:bg-blue-700 text-white font-bold px-6 py-2 rounded-lg transition-all shadow-[0_0_10px_rgba(37,99,235,0.3)] flex items-center">
            <span v-if="carregando" class="animate-spin h-4 w-4 border-2 border-white border-t-transparent rounded-full mr-2"></span>
            Salvar Alterações
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'

const props = defineProps({
  empresa: Object
})
const emit = defineEmits(['fechar', 'empresaEditada'])

const empresaEditada = ref({ id: '', nome: '', cidade: '' })
const carregando = ref(false)
const erro = ref('')

onMounted(() => {
  empresaEditada.value = { ...props.empresa }
})

const salvarEdicao = async () => {
  carregando.value = true
  erro.value = ''
  try {
    const token = localStorage.getItem('token')
    await axios.put(`/api/Empresa/${empresaEditada.value.id}`, empresaEditada.value, {
      headers: { Authorization: `Bearer ${token}` }
    })
    emit('empresaEditada')
  } catch (e) {
    erro.value = 'Erro ao atualizar os dados.'
  } finally {
    carregando.value = false
  }
}
</script>

<style scoped>
.animate-fade-in { animation: fadeIn 0.2s ease-out; }
@keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
</style>