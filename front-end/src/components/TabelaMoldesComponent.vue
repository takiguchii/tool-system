<template>
  <div class="bg-zinc-950 border border-zinc-800 rounded-xl p-4 sm:p-8 shadow-2xl text-white relative flex flex-col h-full">
    
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6 shrink-0">
      <h2 class="text-xl sm:text-2xl font-bold text-orange-500">Moldes Cadastrados</h2>
      <button v-if="ehAdmin" @click="mostrarModalNovo = true" class="bg-orange-500 hover:bg-orange-600 active:scale-95 text-white font-bold py-2 px-4 rounded-lg transition-all shadow-[0_0_10px_rgba(249,115,22,0.3)] w-full sm:w-auto">
        + Novo Molde
      </button>
    </div>

    <div class="mb-6 shrink-0">
      <div class="relative">
        <span class="absolute inset-y-0 left-0 flex items-center pl-4 text-zinc-500">🔍</span>
        <input 
          type="text" 
          v-model="termoPesquisa" 
          @input="pesquisarBackend" 
          placeholder="Pesquise por peça ou código..." 
          class="w-full bg-zinc-900 border border-zinc-800 text-white rounded-lg pl-12 pr-4 py-3 focus:outline-none focus:border-orange-500 focus:ring-1 focus:ring-orange-500 transition-colors shadow-inner placeholder-zinc-600 text-sm sm:text-base"
        >
      </div>
    </div>

    <div v-if="carregando" class="text-zinc-500 animate-pulse font-medium p-4 text-center border border-dashed border-zinc-800 rounded-lg">
      Buscando informações e cruzando dados na fábrica...
    </div>

    <div v-else-if="erro" class="text-red-500 font-bold p-4 bg-red-900/20 rounded-lg border border-red-900/50">
      {{ erro }}
    </div>

    <div v-else class="overflow-y-auto sm:rounded-lg sm:border sm:border-zinc-800 sm:shadow-lg flex-1 custom-scrollbar flex flex-col">
      
      <table class="w-full text-left border-collapse block md:table flex-1">
        <thead class="hidden md:table-header-group sticky top-0 bg-zinc-900 z-10 shadow-md">
          <tr class="text-zinc-400 border-b border-zinc-800 text-sm uppercase tracking-wider">
            <th class="p-4 font-medium w-20">ID</th>
            <th class="p-4 font-medium">Código</th>
            <th class="p-4 font-medium">Nome da Peça</th>
            <th class="p-4 font-medium">Empresa</th>
            <th class="p-4 font-medium">Status</th>
            <th class="p-4 font-medium text-center w-64">Ações</th>
          </tr>
        </thead>
        
        <tbody class="block md:table-row-group divide-y divide-transparent md:divide-zinc-800">
          
          <tr v-for="molde in moldes" :key="molde.id" class="block md:table-row hover:bg-zinc-900/60 transition-colors duration-200 bg-zinc-900 md:bg-transparent rounded-xl mb-4 md:mb-0 border border-zinc-800 md:border-none p-4 md:p-0 shadow-sm md:shadow-none">
            
            <td class="flex justify-between items-center md:table-cell py-2 md:py-4 md:p-4 border-b border-zinc-800/50 md:border-none">
              <span class="md:hidden text-[10px] font-bold text-zinc-500 uppercase">ID</span>
              <span class="font-mono text-orange-400 font-bold">{{ molde.id }}</span>
            </td>

            <td class="flex justify-between items-center md:table-cell py-2 md:py-4 md:p-4 border-b border-zinc-800/50 md:border-none">
              <span class="md:hidden text-[10px] font-bold text-zinc-500 uppercase">Código</span>
              <span class="font-medium text-white">{{ molde.codigo }}</span>
            </td>

            <td class="flex justify-between items-center md:table-cell py-2 md:py-4 md:p-4 border-b border-zinc-800/50 md:border-none">
              <span class="md:hidden text-[10px] font-bold text-zinc-500 uppercase">Nome</span>
              <span class="font-medium text-zinc-300 capitalize">{{ molde.nome }}</span>
            </td>

            <td class="flex justify-between items-center md:table-cell py-2 md:py-4 md:p-4 border-b border-zinc-800/50 md:border-none">
              <span class="md:hidden text-[10px] font-bold text-zinc-500 uppercase">Empresa</span>
              <span class="text-zinc-400 text-sm truncate max-w-[150px] md:max-w-none text-right md:text-left">{{ getNomeEmpresa(molde.empresaId) }}</span>
            </td>

            <td class="flex justify-between items-center md:table-cell py-2 md:py-4 md:p-4 border-b border-zinc-800/50 md:border-none">
              <span class="md:hidden text-[10px] font-bold text-zinc-500 uppercase">Status</span>
              <span :class="molde.status === 'Desativado' ? 'text-red-400 bg-red-500/10 border-red-500/20' : 'text-green-400 bg-green-500/10 border-green-500/20'" class="border px-2 py-1 rounded text-[10px] sm:text-xs font-bold uppercase">
                {{ molde.status || 'Ativo' }}
              </span>
            </td>
            
            <td class="flex justify-between items-center md:table-cell py-2 md:py-4 md:p-4 border-b border-zinc-800/50 md:border-none">
              <span class="md:hidden text-[10px] font-bold text-zinc-500 uppercase">Ações</span>
              <div class="flex justify-center items-center gap-4">
                <button @click="abrirDetalhes(molde)" class="text-sm font-semibold text-zinc-400 hover:text-orange-500 transition-colors bg-zinc-950 md:bg-transparent px-3 py-1.5 md:px-0 md:py-0 rounded-lg border border-zinc-800 md:border-none">Detalhes</button>
                <button v-if="ehAdmin" @click="abrirEdicao(molde)" class="text-sm font-semibold text-zinc-400 hover:text-blue-500 transition-colors bg-zinc-950 md:bg-transparent px-3 py-1.5 md:px-0 md:py-0 rounded-lg border border-zinc-800 md:border-none">Editar</button>
                <button v-if="ehAdmin" @click="deletarMolde(molde.id)" class="text-sm font-semibold text-zinc-400 hover:text-red-500 transition-colors bg-zinc-950 md:bg-transparent px-3 py-1.5 md:px-0 md:py-0 rounded-lg border border-zinc-800 md:border-none">Excluir</button>
              </div>
            </td>
            
          </tr>
        </tbody>
      </table>

      <div v-if="totalPaginas > 1" class="flex items-center justify-between px-2 sm:px-6 py-3 sm:py-4 border-t border-zinc-800 bg-zinc-950/50 mt-auto shrink-0 gap-2">
        
        <button 
          @click="mudarPagina(paginaAtual - 1)" 
          :disabled="paginaAtual === 1" 
          class="flex justify-center items-center px-3 sm:px-4 py-2 text-sm font-bold text-white bg-zinc-800 rounded-lg hover:bg-zinc-700 disabled:opacity-30 disabled:cursor-not-allowed transition-colors active:scale-95 min-w-[44px]">
          ← <span class="hidden sm:inline ml-1.5">Anterior</span>
        </button>
        
        <span class="text-xs sm:text-sm text-zinc-400 font-medium text-center whitespace-nowrap">
          <span class="hidden sm:inline">Página</span>
          <span class="sm:hidden">Pág</span>
          <span class="font-bold text-orange-500 ml-1">{{ paginaAtual }}</span> 
          de 
          <span class="font-bold text-white">{{ totalPaginas }}</span>
        </span>
        
        <button 
          @click="mudarPagina(paginaAtual + 1)" 
          :disabled="paginaAtual === totalPaginas" 
          class="flex justify-center items-center px-3 sm:px-4 py-2 text-sm font-bold text-white bg-zinc-800 rounded-lg hover:bg-zinc-700 disabled:opacity-30 disabled:cursor-not-allowed transition-colors active:scale-95 min-w-[44px]">
          <span class="hidden sm:inline mr-1.5">Próximo</span> →
        </button>
        
      </div>
    </div>

    <ModalNovoMoldeComponent 
      v-if="mostrarModalNovo" 
      @fechar="mostrarModalNovo = false" 
      @moldeCadastrado="buscarEUnificarDados" 
    />
    
    <ModalEditarMoldeComponent 
      v-if="mostrarModalEditar" 
      :molde="moldeSelecionado" 
      @fechar="mostrarModalEditar = false" 
      @moldeEditado="buscarEUnificarDados" 
    />
    
    <ModalDetalhesMoldeComponent 
      v-if="mostrarModalDetalhes" 
      :molde="moldeSelecionado" 
      @fechar="mostrarModalDetalhes = false" 
      @fotoAtualizada="buscarEUnificarDados" 
    />
  </div>
  
</template>


<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import ModalNovoMoldeComponent from './ModalNovoMoldeComponent.vue'
import ModalEditarMoldeComponent from './ModalEditarMoldeComponent.vue'
import ModalDetalhesMoldeComponent from './ModalDetalhesMoldeComponent.vue'

const moldes = ref([])
const empresas = ref([]) 
const carregando = ref(true)
const erro = ref('')

const termoPesquisa = ref('')
const paginaAtual = ref(1)
const totalPaginas = ref(1)
let timerPesquisa = null 

const mostrarModalNovo = ref(false)
const mostrarModalEditar = ref(false)
const mostrarModalDetalhes = ref(false)
const moldeSelecionado = ref(null)

const perfilUsuario = localStorage.getItem('perfil') || 'Consultor'
const ehAdmin = perfilUsuario === 'Admin'

const getNomeEmpresa = (id) => {
  if (!id) return '-'
  const emp = empresas.value.find(e => e.id === id)
  return emp ? emp.nome : 'Desconhecida'
}

const buscarEUnificarDados = async () => {
  carregando.value = true
  erro.value = ''
  
  try {
    const token = localStorage.getItem('token')
    const config = { headers: { Authorization: `Bearer ${token}` } }
    
    const parametros = { 
      pagina: paginaAtual.value,
      tamanhoPagina: 10
    }
    
    if (termoPesquisa.value.trim() !== '') {
      parametros.termoBusca = termoPesquisa.value
    }

    const [respMoldes, respEmpresas] = await Promise.all([
      axios.get('/api/Molde', { ...config, params: parametros }),
      axios.get('/api/Empresa', config)
    ])
    
    moldes.value = respMoldes.data.dados
    paginaAtual.value = respMoldes.data.paginaAtual
    totalPaginas.value = respMoldes.data.totalPaginas
    empresas.value = respEmpresas.data

    mostrarModalNovo.value = false
    mostrarModalEditar.value = false
    
  } catch (e) {
    erro.value = 'Falha ao buscar as informações do servidor.'
  } finally {
    carregando.value = false
  }
}

const mudarPagina = (novaPagina) => {
  if (novaPagina >= 1 && novaPagina <= totalPaginas.value) {
    paginaAtual.value = novaPagina
    buscarEUnificarDados() 
  }
}

const pesquisarBackend = () => {
  clearTimeout(timerPesquisa)
  timerPesquisa = setTimeout(() => {
    paginaAtual.value = 1 
    buscarEUnificarDados()
  }, 500)
}

const deletarMolde = async (id) => {
  if (!window.confirm("Tem certeza que deseja apagar este molde?")) return
  try {
    const token = localStorage.getItem('token')
    await axios.delete(`/api/Molde/${id}`, { headers: { Authorization: `Bearer ${token}` } })
    buscarEUnificarDados() 
  } catch (e) {
    alert("Erro ao excluir. O banco de dados pode estar protegendo o histórico deste molde.")
  }
}

const abrirEdicao = (molde) => {
  moldeSelecionado.value = molde
  mostrarModalEditar.value = true
}

const abrirDetalhes = (molde) => {
  moldeSelecionado.value = molde
  mostrarModalDetalhes.value = true
}

onMounted(() => {
  buscarEUnificarDados()
})
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 4px; height: 4px; }
.custom-scrollbar::-webkit-scrollbar-track { background: #18181b; border-radius: 8px; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #3f3f46; border-radius: 8px; }
.custom-scrollbar::-webkit-scrollbar-thumb:hover { background: #f97316; }
</style>