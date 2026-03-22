<template>
  <div class="bg-zinc-950 border border-zinc-800 rounded-xl p-4 md:p-8 shadow-2xl text-white relative">
    
    <div class="flex flex-col md:flex-row justify-between md:items-center mb-6 gap-4">
      <h2 class="text-2xl font-bold text-orange-500">Acervo de Moldes</h2>
      <button @click="mostrarModal = true" class="w-full md:w-auto bg-orange-500 hover:bg-orange-600 active:scale-95 text-white font-bold py-2 px-4 rounded-lg transition-all shadow-[0_0_10px_rgba(249,115,22,0.3)]">
        + Novo Molde
      </button>
    </div>

    <div v-if="carregando" class="text-zinc-500 animate-pulse font-medium p-4 text-center border border-dashed border-zinc-800 rounded-lg">
      Buscando ferramentas no cofre da API...
    </div>

    <div v-else-if="erro" class="text-red-500 font-bold p-4 bg-red-900/20 rounded-lg border border-red-900/50">
      {{ erro }}
    </div>

    <div v-else>
      <!-- Tabela para telas médias e maiores -->
      <div class="hidden md:block overflow-x-auto rounded-lg border border-zinc-800 shadow-lg">
        <table class="w-full text-left border-collapse">
          <thead>
            <tr class="bg-zinc-900 text-zinc-400 border-b border-zinc-800 text-sm uppercase tracking-wider">
              <th class="p-4 font-medium">ID / Código</th>
              <th class="p-4 font-medium">Descrição da Peça</th>
              <th class="p-4 font-medium">Detalhe Técnico</th>
              <th class="p-4 font-medium">Status</th>
              <th class="p-4 font-medium text-center">Ações</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-zinc-800">
            <tr v-for="molde in moldes" :key="molde.id" class="hover:bg-zinc-900/60 transition-colors duration-200">
              <td class="p-4 font-mono text-orange-400 font-bold">{{ molde.id || molde.codigo || '000' }}</td>
              <td class="p-4 font-medium text-zinc-200">{{ molde.nome || 'Sem descrição' }}</td>
              <td class="p-4 text-zinc-400">{{ molde.prateleira || 'Não informado' }}</td>
              <td class="p-4">
                <span :class="molde.status === 'Disponível' ? 'bg-green-900/30 text-green-400 border-green-800' : 'bg-yellow-900/30 text-yellow-400 border-yellow-800'" class="border px-3 py-1 rounded-full text-xs font-bold uppercase tracking-wide">
                  {{ molde.status || 'Disponível' }}
                </span>
              </td>
              <td class="p-4 text-center flex justify-center gap-4">
                  <button @click="abrirDetalhes(molde)" class="text-sm font-semibold text-zinc-500 hover:text-orange-500">Ver</button>
                  <button @click="abrirEdicao(molde)" class="text-sm font-semibold text-zinc-500 hover:text-blue-500">Editar</button>
                  <button @click="deletarMolde(molde.id)" class="text-sm font-semibold text-zinc-500 hover:text-red-500">Excluir</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Cards para telas pequenas -->
      <div class="block md:hidden space-y-4">
        <div v-for="molde in moldes" :key="molde.id + '-mobile'" class="bg-zinc-900 rounded-lg p-4 border border-zinc-800">
          <div class="flex justify-between items-start">
            <div>
              <p class="font-medium text-zinc-200 text-lg">{{ molde.nome || 'Sem descrição' }}</p>
              <p class="font-mono text-orange-400 text-sm">{{ molde.id || molde.codigo || '000' }}</p>
            </div>
            <span :class="molde.status === 'Disponível' ? 'bg-green-900/30 text-green-400' : 'bg-yellow-900/30 text-yellow-400'" class="px-2 py-1 rounded-full text-xs font-bold uppercase tracking-wide">
              {{ molde.status || 'Disponível' }}
            </span>
          </div>
          <p class="text-zinc-400 mt-2">Local: {{ molde.prateleira || 'Não informado' }}</p>
          <div class="mt-4 pt-4 border-t border-zinc-800 flex justify-end gap-4">
            <button @click="abrirDetalhes(molde)" class="text-sm font-semibold text-orange-500">Ver Detalhes</button>
            <button @click="abrirEdicao(molde)" class="text-sm font-semibold text-blue-500">Editar</button>
            <button @click="deletarMolde(molde.id)" class="text-sm font-semibold text-red-500">Excluir</button>
          </div>
        </div>
      </div>
      
      <div v-if="moldes.length === 0" class="p-8 text-center text-zinc-500 font-medium">
        Nenhum molde cadastrado no sistema no momento.
      </div>
    </div>

    <ModalNovoMoldeComponent 
      v-if="mostrarModal" 
      @fechar="mostrarModal = false" 
      @moldeCadastrado="aoCadastrarComSucesso"
    />

    <ModalEditarMoldeComponent 
      v-if="mostrarModalEditar" 
      :molde="moldeSelecionado"
      @fechar="mostrarModalEditar = false" 
      @moldeEditado="aoEditarComSucesso"
    />

    <ModalDetalhesMoldeComponent 
      v-if="mostrarModalDetalhes" 
      :molde="moldeSelecionado"
      @fechar="mostrarModalDetalhes = false" 
      @fotoAtualizada="aoEditarComSucesso"
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
const carregando = ref(true)
const erro = ref('')
const mostrarModal = ref(false)
const mostrarModalEditar = ref(false)
const mostrarModalDetalhes = ref(false)
const moldeSelecionado = ref(null)

const buscarMoldes = async () => {
  carregando.value = true
  try {
    const token = localStorage.getItem('token')
    const resposta = await axios.get('/api/Molde', {
      headers: { Authorization: `Bearer ${token}` }
    })
    moldes.value = resposta.data
  } catch (e) {
    erro.value = 'Falha ao buscar os moldes. Verifique sua conexão.'
  } finally {
    carregando.value = false
  }
}

const aoCadastrarComSucesso = () => {
  mostrarModal.value = false
  buscarMoldes()
}

const aoEditarComSucesso = () => {
  mostrarModalEditar.value = false
  mostrarModalDetalhes.value = false // Fecha também os detalhes se estiver aberto
  buscarMoldes()
}

const deletarMolde = async (id) => {
  if (!window.confirm("Tem certeza que deseja apagar esta ferramenta?")) return
  try {
    const token = localStorage.getItem('token')
    await axios.delete(`/api/Molde/${id}`, {
      headers: { Authorization: `Bearer ${token}` }
    })
    buscarMoldes()
  } catch (e) {
    alert("Erro ao tentar excluir.")
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

onMounted(buscarMoldes)
</script>