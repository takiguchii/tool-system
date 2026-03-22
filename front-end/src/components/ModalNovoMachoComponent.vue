<template>
  <div class="fixed inset-0 bg-black/80 flex items-center justify-center z-50 animate-fade-in p-4">
    <div class="bg-zinc-900 border border-zinc-700 rounded-xl p-8 w-full max-w-md shadow-2xl relative">
      <h2 class="text-2xl font-bold text-white mb-6">Cadastrar Macho</h2>
      
      <form @submit.prevent="salvarMacho" class="flex flex-col gap-4">
        
        <div>
          <label class="block text-zinc-400 font-medium mb-1 text-sm">Código do Macho</label>
          <input type="text" v-model="novoMacho.codigo" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2 focus:outline-none focus:border-orange-500" required placeholder="Ex: MCH-2050">
        </div>

        <div>
          <label class="block text-zinc-400 font-medium mb-1 text-sm">Vincular ao Molde</label>
          <select v-model="moldeSelecionado" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2 focus:outline-none focus:border-orange-500" required>
            <option value="" disabled selected>Selecione o Molde...</option>
            <option v-for="molde in moldes" :key="molde.id" :value="molde.id">
              {{ molde.nome }} ({{ molde.codigo }})
            </option>
          </select>
        </div>

        <p v-if="erro" class="text-red-500 text-sm mt-2">{{ erro }}</p>

        <div class="flex justify-end gap-3 mt-6">
          <button type="button" @click="$emit('fechar')" class="px-4 py-2 rounded-lg text-zinc-400 hover:text-white transition-colors font-medium">Cancelar</button>
          <button type="submit" :disabled="carregando" class="bg-orange-500 hover:bg-orange-600 text-white font-bold px-6 py-2 rounded-lg transition-all shadow-lg">
            Salvar Macho
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'

const emit = defineEmits(['fechar', 'machoCadastrado'])

const novoMacho = ref({ codigo: '' })
const moldeSelecionado = ref('')
const moldes = ref([])
const carregando = ref(false)
const erro = ref('')

// Assim que a prancheta abre, ela busca a lista de moldes
onMounted(async () => {
  try {
    const token = localStorage.getItem('token')
    const resposta = await axios.get('/api/Molde', { headers: { Authorization: `Bearer ${token}` } })
    moldes.value = resposta.data
  } catch (e) {
    erro.value = "Aviso: Falha ao carregar a lista de moldes."
  }
})

const salvarMacho = async () => {
  if(!moldeSelecionado.value) {
    erro.value = "É obrigatório selecionar um molde."
    return
  }
  carregando.value = true
  erro.value = ''
  try {
    const token = localStorage.getItem('token')
    await axios.post(`/api/Macho?moldeId=${moldeSelecionado.value}`, novoMacho.value, {
      headers: { Authorization: `Bearer ${token}` }
    })
    emit('machoCadastrado')
  } catch (e) {
    erro.value = 'Erro ao salvar. Verifique se o código não está duplicado.'
  } finally {
    carregando.value = false
  }
}
</script>

<style scoped>
.animate-fade-in { animation: fadeIn 0.2s ease-out; }
@keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
</style>