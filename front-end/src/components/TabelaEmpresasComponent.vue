<template>
  <div class="bg-zinc-950 border border-zinc-800 rounded-xl p-4 sm:p-8 shadow-2xl text-white relative flex flex-col h-full">
    
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6 shrink-0">
      <h2 class="text-xl sm:text-2xl font-bold text-orange-500">Empresas Cadastradas</h2>
      <button @click="mostrarModalNovo = true" class="bg-orange-500 hover:bg-orange-600 active:scale-95 text-white font-bold py-2 px-4 rounded-lg transition-all shadow-[0_0_10px_rgba(249,115,22,0.3)] w-full sm:w-auto">
        + Nova Empresa
      </button>
    </div>

    <div class="mb-6 shrink-0">
      <div class="relative">
        <span class="absolute inset-y-0 left-0 flex items-center pl-4 text-zinc-500">🔍</span>
        <input 
          type="text" 
          v-model="termoPesquisa" 
          placeholder="Pesquise por nome ou cidade..." 
          class="w-full bg-zinc-900 border border-zinc-800 text-white rounded-lg pl-12 pr-4 py-3 focus:outline-none focus:border-orange-500 focus:ring-1 focus:ring-orange-500 transition-colors shadow-inner placeholder-zinc-600 text-sm sm:text-base"
        >
      </div>
    </div>

    <div v-if="carregando" class="text-zinc-500 animate-pulse font-medium p-4 text-center border border-dashed border-zinc-800 rounded-lg">
      Buscando empresas no cofre da API...
    </div>

    <div v-else-if="erro" class="text-red-500 font-bold p-4 bg-red-900/20 rounded-lg border border-red-900/50">
      {{ erro }}
    </div>

    <div v-else class="overflow-y-auto sm:rounded-lg sm:border sm:border-zinc-800 sm:shadow-lg flex-1 custom-scrollbar">
      <table class="w-full text-left border-collapse block md:table">
        <thead class="hidden md:table-header-group sticky top-0 bg-zinc-900 z-10 shadow-md">
          <tr class="text-zinc-400 border-b border-zinc-800 text-sm uppercase tracking-wider">
            <th class="p-4 font-medium w-24">ID</th>
            <th class="p-4 font-medium">Nome da Empresa</th>
            <th class="p-4 font-medium">Cidade</th>
            <th class="p-4 font-medium text-center w-40">Ações</th>
          </tr>
        </thead>
        <tbody class="block md:table-row-group divide-y divide-transparent md:divide-zinc-800">
          
          <tr v-for="empresa in empresasFiltradas" :key="empresa.id" class="block md:table-row hover:bg-zinc-900/60 transition-colors duration-200 bg-zinc-900 md:bg-transparent rounded-xl mb-4 md:mb-0 border border-zinc-800 md:border-none p-4 md:p-0 shadow-sm md:shadow-none">
            
            <td class="flex justify-between items-center md:table-cell py-2 md:py-4 md:p-4 border-b border-zinc-800/50 md:border-none">
              <span class="md:hidden text-[10px] font-bold text-zinc-500 uppercase">ID</span>
              <span class="font-mono text-orange-400 font-bold">{{ empresa.id }}</span>
            </td>

            <td class="flex justify-between items-center md:table-cell py-2 md:py-4 md:p-4 border-b border-zinc-800/50 md:border-none">
              <span class="md:hidden text-[10px] font-bold text-zinc-500 uppercase">Nome</span>
              <span class="font-medium text-zinc-200">{{ empresa.nome }}</span>
            </td>

            <td class="flex justify-between items-center md:table-cell py-2 md:py-4 md:p-4 border-b border-zinc-800/50 md:border-none">
              <span class="md:hidden text-[10px] font-bold text-zinc-500 uppercase">Cidade</span>
              <span class="text-zinc-400 truncate max-w-xs">{{ empresa.cidade || 'Não informada' }}</span>
            </td>
            
            <td class="block md:table-cell py-4 md:p-4 mt-2 md:mt-0">
              <div class="flex justify-center items-center gap-4">
                <button @click="abrirEdicao(empresa)" class="text-sm font-semibold text-zinc-400 hover:text-blue-500 transition-colors bg-zinc-950 md:bg-transparent px-3 py-1.5 md:px-0 md:py-0 rounded-lg border border-zinc-800 md:border-none">
                  Editar
                </button>
                <button @click="deletarEmpresa(empresa.id)" class="text-sm font-semibold text-zinc-400 hover:text-red-500 transition-colors bg-zinc-950 md:bg-transparent px-3 py-1.5 md:px-0 md:py-0 rounded-lg border border-zinc-800 md:border-none">
                  Excluir
                </button>
              </div>
            </td>
          </tr>

        </tbody>
      </table>
      
      <div v-if="empresas.length === 0" class="p-10 text-center text-zinc-500 font-medium">
        Nenhuma empresa cadastrada.
      </div>
      <div v-else-if="empresasFiltradas.length === 0" class="p-10 text-center text-zinc-500 font-medium">
        Nenhum resultado para "<span class="text-orange-400">{{ termoPesquisa }}</span>".
      </div>
    </div>

    <transition name="modal">
      <ModalNovaEmpresaComponent v-if="mostrarModalNovo" @fechar="mostrarModalNovo = false" @empresaCadastrada="aoAtualizar" />
    </transition>
    <transition name="modal">
      <ModalEditarEmpresaComponent v-if="mostrarModalEditar" :empresa="empresaSelecionada" @fechar="mostrarModalEditar = false" @empresaEditada="aoAtualizar" />
    </transition>

  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import axios from 'axios'

import ModalNovaEmpresaComponent from './ModalNovaEmpresaComponent.vue'
import ModalEditarEmpresaComponent from './ModalEditarEmpresaComponent.vue'

const empresas = ref([])
const carregando = ref(true)
const erro = ref('')
const termoPesquisa = ref('')

const empresasFiltradas = computed(() => {
  if (!termoPesquisa.value) return empresas.value
  const termo = termoPesquisa.value.toLowerCase()
  return empresas.value.filter(emp => {
    const nome = emp.nome ? emp.nome.toLowerCase() : ''
    const cidade = emp.cidade ? emp.cidade.toLowerCase() : ''
    return nome.includes(termo) || cidade.includes(termo)
  })
})

const mostrarModalNovo = ref(false)
const mostrarModalEditar = ref(false)
const empresaSelecionada = ref(null)

const buscarEmpresas = async () => {
  try {
    const token = localStorage.getItem('token')
    const resposta = await axios.get('/api/Empresa', { headers: { Authorization: `Bearer ${token}` } })
    empresas.value = resposta.data
  } catch (e) {
    erro.value = 'Falha ao buscar as empresas.'
  } finally {
    carregando.value = false
  }
}

const deletarEmpresa = async (id) => {
  if (!window.confirm("Tem certeza que deseja apagar esta empresa?")) return
  try {
    const token = localStorage.getItem('token')
    await axios.delete(`/api/Empresa/${id}`, { headers: { Authorization: `Bearer ${token}` } })
    buscarEmpresas()
  } catch (e) {
    alert("Erro ao excluir. Verifique se existem ferramentas usando esta empresa.")
  }
}

const aoAtualizar = () => {
  mostrarModalNovo.value = false
  mostrarModalEditar.value = false
  carregando.value = true
  buscarEmpresas()
}

const abrirEdicao = (empresa) => {
  empresaSelecionada.value = empresa
  mostrarModalEditar.value = true
}

onMounted(() => buscarEmpresas())
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: #18181b; border-radius: 8px; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #3f3f46; border-radius: 8px; }
.custom-scrollbar::-webkit-scrollbar-thumb:hover { background: #f97316; }
</style>