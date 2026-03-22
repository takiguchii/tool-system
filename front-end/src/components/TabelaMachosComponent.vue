<template>
  <div class="bg-zinc-950 border border-zinc-800 rounded-xl p-8 shadow-2xl text-white relative">
    
    <div class="flex justify-between items-center mb-6">
      <h2 class="text-2xl font-bold text-orange-500">Machos Cadastrados</h2>
      <button @click="mostrarModalNovo = true" class="bg-orange-500 hover:bg-orange-600 active:scale-95 text-white font-bold py-2 px-4 rounded-lg transition-all shadow-[0_0_10px_rgba(249,115,22,0.3)]">
        + Novo Macho
      </button>
    </div>

    <div v-if="carregando" class="text-zinc-500 animate-pulse font-medium p-4 text-center border border-dashed border-zinc-800 rounded-lg">
      Buscando machos no cofre da API...
    </div>

    <div v-else-if="erro" class="text-red-500 font-bold p-4 bg-red-900/20 rounded-lg border border-red-900/50">
      {{ erro }}
    </div>

    <div v-else class="overflow-x-auto rounded-lg border border-zinc-800 shadow-lg">
      <table class="w-full text-left border-collapse">
        <thead>
          <tr class="bg-zinc-900 text-zinc-400 border-b border-zinc-800 text-sm uppercase tracking-wider">
            <th class="p-4 font-medium w-24">ID</th>
            <th class="p-4 font-medium">Código do Macho</th>
            <th class="p-4 font-medium text-center w-64">Ações</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-zinc-800">
          
          <tr v-for="macho in machos" :key="macho.id" class="hover:bg-zinc-900/60 transition-colors duration-200">
            <td class="p-4 font-mono text-orange-400 font-bold">{{ macho.id }}</td>
            <td class="p-4 font-medium text-zinc-200">{{ macho.codigo }}</td>
            
            <td class="p-4 text-center flex justify-center gap-4">
              <button @click="abrirDetalhes(macho)" class="text-sm font-semibold text-zinc-500 hover:text-orange-500 transition-colors">
                Ver Detalhes / Fotos
              </button>
              <button @click="abrirEdicao(macho)" class="text-sm font-semibold text-zinc-500 hover:text-blue-500 transition-colors">
                Editar
              </button>
              <button @click="deletarMacho(macho.id)" class="text-sm font-semibold text-zinc-500 hover:text-red-500 transition-colors">
                Excluir
              </button>
            </td>
          </tr>

        </tbody>
      </table>
      
      <div v-if="machos.length === 0" class="p-8 text-center text-zinc-500 font-medium">
        Nenhum macho cadastrado no momento.
      </div>
    </div>

    <ModalNovoMachoComponent 
      v-if="mostrarModalNovo" 
      @fechar="mostrarModalNovo = false" 
      @machoCadastrado="aoAtualizarListagem" 
    />

    <ModalEditarMachoComponent 
      v-if="mostrarModalEditar" 
      :macho="machoSelecionado"
      @fechar="mostrarModalEditar = false" 
      @machoEditado="aoAtualizarListagem" 
    />

    <ModalDetalhesMachoComponent
      v-if="mostrarModalDetalhes"
      :macho="machoSelecionadoParaDetalhes"
      @fechar="mostrarModalDetalhes = false"
      @fotosAtualizadas="buscarMachos"
    />

  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'

import ModalNovoMachoComponent from './ModalNovoMachoComponent.vue'
import ModalEditarMachoComponent from './ModalEditarMachoComponent.vue'
import ModalDetalhesMachoComponent from './ModalDetalhesMachoComponent.vue'

const machos = ref([])
const carregando = ref(true)
const erro = ref('')

const mostrarModalNovo = ref(false)
const mostrarModalEditar = ref(false)
const machoSelecionado = ref(null)

const mostrarModalDetalhes = ref(false)
const machoSelecionadoParaDetalhes = ref(null)

const buscarMachos = async () => {
  try {
    const token = localStorage.getItem('token')
    const resposta = await axios.get('/api/Macho', { headers: { Authorization: `Bearer ${token}` } })
    machos.value = resposta.data
  } catch (e) {
    erro.value = 'Falha ao buscar os machos. Verifique sua conexão.'
  } finally {
    carregando.value = false
  }
}

const deletarMacho = async (id) => {
  const confirmacao = window.confirm("Tem certeza que deseja apagar este macho?")
  if (!confirmacao) return
  try {
    const token = localStorage.getItem('token')
    await axios.delete(`/api/Macho/${id}`, { headers: { Authorization: `Bearer ${token}` } })
    buscarMachos()
  } catch (e) {
    alert("Erro ao excluir. O banco de dados pode estar protegendo o histórico deste macho.")
  }
}

const aoAtualizarListagem = () => {
  mostrarModalNovo.value = false
  mostrarModalEditar.value = false
  carregando.value = true
  buscarMachos()
}

const abrirEdicao = (macho) => {
  machoSelecionado.value = macho
  mostrarModalEditar.value = true
}

const abrirDetalhes = (macho) => {
  machoSelecionadoParaDetalhes.value = macho
  mostrarModalDetalhes.value = true
}

onMounted(() => {
  buscarMachos()
})
</script>