<template>
  <div class="fixed inset-0 bg-black/70 flex items-center justify-center z-50 animate-fade-in">
    <div class="bg-zinc-900 border border-zinc-700 rounded-xl p-8 w-full max-w-md shadow-2xl relative">
      <h2 class="text-2xl font-bold text-white mb-6">Editar Molde</h2>

      <form @submit.prevent="salvarEdicao" class="flex flex-col gap-4">
        <div>
          <label class="block text-zinc-400 font-medium mb-1 text-sm">Código da Peça</label>
          <input type="text" v-model="moldeEditado.codigo" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2 focus:outline-none focus:border-blue-500 focus:ring-1 focus:ring-blue-500" required>
        </div>

        <div>
          <label class="block text-zinc-400 font-medium mb-1 text-sm">Nome / Descrição</label>
          <input type="text" v-model="moldeEditado.nome" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2 focus:outline-none focus:border-blue-500 focus:ring-1 focus:ring-blue-500" required>
        </div>

        <div>
          <label class="block text-zinc-400 font-medium mb-1 text-sm">Prateleira / Localização</label>
          <input type="text" v-model="moldeEditado.prateleira" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2 focus:outline-none focus:border-blue-500 focus:ring-1 focus:ring-blue-500" required>
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
  molde: Object
})

const emit = defineEmits(['fechar', 'moldeEditado'])

const moldeEditado = ref({
  id: '',
  codigo: '',
  nome: '',
  prateleira: ''
})

const carregando = ref(false)
const erro = ref('')

onMounted(() => {
  moldeEditado.value = { ...props.molde }
})

const salvarEdicao = async () => {
  carregando.value = true
  erro.value = ''

  try {
    const token = localStorage.getItem('token')
    
    await axios.put(`/api/Molde/${moldeEditado.value.id}`, moldeEditado.value, {
      headers: { Authorization: `Bearer ${token}` }
    })

    emit('moldeEditado')
  } catch (e) {
    erro.value = 'Erro ao atualizar. Verifique os dados.'
  } finally {
    carregando.value = false
  }
}
</script>

<style scoped>
.animate-fade-in {
  animation: fadeIn 0.2s ease-out;
}
@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}
</style>