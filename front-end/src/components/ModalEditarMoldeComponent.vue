<template>
  <div class="fixed inset-0 bg-black/80 flex items-center justify-center z-50 animate-fade-in p-4">
    <div class="bg-zinc-900 border border-zinc-700 rounded-xl p-8 w-full max-w-md shadow-2xl relative">
      <h2 class="text-2xl font-bold text-white mb-6">Editar Molde</h2>
      
      <form @submit.prevent="salvarEdicao" class="flex flex-col gap-4">
        <div>
          <label class="block text-zinc-400 font-medium mb-1 text-sm">Nome da Peça</label>
          <input type="text" v-model="moldeEditado.nome" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2 focus:outline-none focus:border-blue-500" required>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-zinc-400 font-medium mb-1 text-sm">Código</label>
            <input type="text" v-model="moldeEditado.codigo" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2 focus:outline-none focus:border-blue-500" required>
          </div>
          <div>
            <label class="block text-zinc-400 font-medium mb-1 text-sm">Prateleira</label>
            <input type="text" v-model="moldeEditado.prateleira" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2 focus:outline-none focus:border-blue-500">
          </div>
        </div>

        <div>
          <label class="block text-zinc-400 font-medium mb-1 text-sm">Status Atual</label>
          <select v-model="moldeEditado.status" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2 focus:outline-none focus:border-blue-500" required>
            <option v-for="est in statusOpcoes" :key="est" :value="est">{{ est }}</option>
          </select>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div>
            <label class="block text-zinc-400 font-medium mb-1 text-sm">Empresa</label>
            <select v-model="moldeEditado.empresaId" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2 focus:outline-none focus:border-blue-500">
              <option :value="null">Nenhuma</option>
              <option v-for="emp in empresas" :key="emp.id" :value="emp.id">{{ emp.nome }}</option>
            </select>
          </div>
          <div>
            <label class="block text-zinc-400 font-medium mb-1 text-sm">Categoria</label>
            <select v-model="moldeEditado.categoriaId" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2 focus:outline-none focus:border-blue-500">
              <option :value="null">Nenhuma</option>
              <option v-for="cat in categorias" :key="cat.id" :value="cat.id">{{ cat.setor }}</option>
            </select>
          </div>
        </div>

        <p v-if="erro" class="text-red-500 text-sm mt-2">{{ erro }}</p>

        <div class="flex justify-end gap-3 mt-6">
          <button type="button" @click="$emit('fechar')" class="px-4 py-2 rounded-lg text-zinc-400 hover:text-white transition-colors font-medium">Cancelar</button>
          <button type="submit" :disabled="carregando" class="bg-blue-600 hover:bg-blue-700 text-white font-bold px-6 py-2 rounded-lg transition-all">Salvar Alterações</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'

const props = defineProps({ molde: Object })
const emit = defineEmits(['fechar', 'moldeEditado'])

const statusOpcoes = ['Disponível', 'Em Uso', 'Em Manutenção', 'Inativo']

const moldeEditado = ref({ ...props.molde, status: props.molde.status || 'Disponível' })

const empresas = ref([])
const categorias = ref([])
const carregando = ref(false)
const erro = ref('')

onMounted(async () => {
  try {
    const token = localStorage.getItem('token')
    const reqConfig = { headers: { Authorization: `Bearer ${token}` } }
    
    const [respEmp, respCat] = await Promise.all([
      axios.get('/api/Empresa', reqConfig),
      axios.get('/api/Categoria', reqConfig)
    ])
    
    empresas.value = respEmp.data
    categorias.value = respCat.data
  } catch (e) {
    erro.value = "Aviso: Não foi possível carregar as listas de apoio."
  }
})

const salvarEdicao = async () => {
  carregando.value = true
  erro.value = ''
  
  try {
    const token = localStorage.getItem('token')
    await axios.put(`/api/Molde/${moldeEditado.value.id}`, moldeEditado.value, { headers: { Authorization: `Bearer ${token}` } })
    emit('moldeEditado')
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