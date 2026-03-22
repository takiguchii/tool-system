<template>
  <div class="fixed inset-0 bg-black/70 flex items-center justify-center z-50 animate-fade-in">
    
    <div class="bg-zinc-900 border border-zinc-700 rounded-xl p-8 w-full max-w-md shadow-2xl relative">
      
      <h2 class="text-2xl font-bold text-white mb-6">Cadastrar Novo Molde</h2>

      <form @submit.prevent="salvarMolde" class="flex flex-col gap-4">
        
        <div>
          <label class="block text-zinc-400 font-medium mb-1 text-sm">Código da Peça</label>
          <input type="text" v-model="novoMolde.codigo" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2 focus:outline-none focus:border-orange-500 focus:ring-1 focus:ring-orange-500" required placeholder="Ex: MLD-001">
        </div>

        <div>
          <label class="block text-zinc-400 font-medium mb-1 text-sm">Nome / Descrição</label>
          <input type="text" v-model="novoMolde.nome" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2 focus:outline-none focus:border-orange-500 focus:ring-1 focus:ring-orange-500" required placeholder="Ex: Molde de Engrenagem">
        </div>

        <div>
          <label class="block text-zinc-400 font-medium mb-1 text-sm">Prateleira / Localização</label>
          <input type="text" v-model="novoMolde.prateleira" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2 focus:outline-none focus:border-orange-500 focus:ring-1 focus:ring-orange-500" required placeholder="Ex: A-12">
        </div>

        <p v-if="erro" class="text-red-500 text-sm mt-2">{{ erro }}</p>

        <div class="flex justify-end gap-3 mt-6">
          <button type="button" @click="$emit('fechar')" class="px-4 py-2 rounded-lg text-zinc-400 hover:text-white hover:bg-zinc-800 transition-colors font-medium">
            Cancelar
          </button>
          
          <button type="submit" :disabled="carregando" class="bg-orange-500 hover:bg-orange-600 text-white font-bold px-6 py-2 rounded-lg transition-all shadow-[0_0_10px_rgba(249,115,22,0.3)] flex items-center">
            <span v-if="carregando" class="animate-spin h-4 w-4 border-2 border-white border-t-transparent rounded-full mr-2"></span>
            Salvar Molde
          </button>
        </div>

      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import axios from 'axios'

const emit = defineEmits(['fechar', 'moldeCadastrado'])

const novoMolde = ref({
  codigo: '',
  nome: '',
  prateleira: ''
})

const carregando = ref(false)
const erro = ref('')

const salvarMolde = async () => {
  carregando.value = true
  erro.value = ''

  try {
    const token = localStorage.getItem('token')
    
    // ATENÇÃO: Verifique se a rota de POST no Swagger é exatamente esta
    await axios.post('/api/Molde', novoMolde.value, {
      headers: { Authorization: `Bearer ${token}` }
    })

    // Avisa a tabela que deu certo para ela recarregar a lista
    emit('moldeCadastrado')
    
  } catch (e) {
    erro.value = 'Erro ao salvar. Verifique se os dados estão corretos.'
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