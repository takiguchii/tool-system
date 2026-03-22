<template>
  <div class="bg-zinc-950 border border-zinc-800 rounded-xl p-8 shadow-2xl text-white relative">
    
    <div class="flex justify-between items-center mb-6">
      <h2 class="text-2xl font-bold text-orange-500">Empresas Cadastradas</h2>
      <button @click="mostrarModalNovo = true" class="bg-orange-500 hover:bg-orange-600 active:scale-95 text-white font-bold py-2 px-4 rounded-lg transition-all shadow-[0_0_10px_rgba(249,115,22,0.3)]">
        + Nova Empresa
      </button>
    </div>

    <div v-if="carregando" class="text-zinc-500 animate-pulse font-medium p-4 text-center border border-dashed border-zinc-800 rounded-lg">
      Buscando empresas no cofre da API...
    </div>

    <div v-else-if="erro" class="text-red-500 font-bold p-4 bg-red-900/20 rounded-lg border border-red-900/50">
      {{ erro }}
    </div>

    <div v-else class="overflow-x-auto rounded-lg border border-zinc-800 shadow-lg">
      <table class="w-full text-left border-collapse">
        <thead>
          <tr class="bg-zinc-900 text-zinc-400 border-b border-zinc-800 text-sm uppercase tracking-wider">
            <th class="p-4 font-medium w-24">ID</th>
            <th class="p-4 font-medium">Nome da Empresa</th>
            <th class="p-4 font-medium">Cidade</th>
            <th class="p-4 font-medium text-center w-40">Ações</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-zinc-800">
          
          <tr v-for="empresa in empresas" :key="empresa.id" class="hover:bg-zinc-900/60 transition-colors duration-200">
            <td class="p-4 font-mono text-orange-400 font-bold">{{ empresa.id }}</td>
            <td class="p-4 font-medium text-zinc-200">{{ empresa.nome }}</td>
            <td class="p-4 text-zinc-400 truncate max-w-xs">{{ empresa.cidade || 'Não informada' }}</td>
            
            <td class="p-4 text-center flex justify-center gap-4">
              <button @click="abrirEdicao(empresa)" class="text-sm font-semibold text-zinc-500 hover:text-blue-500 transition-colors">
                Editar
              </button>
              <button @click="deletarEmpresa(empresa.id)" class="text-sm font-semibold text-zinc-500 hover:text-red-500 transition-colors">
                Excluir
              </button>
            </td>
          </tr>

        </tbody>
      </table>
      
      <div v-if="empresas.length === 0" class="p-8 text-center text-zinc-500 font-medium">
        Nenhuma empresa cadastrada no momento.
      </div>
    </div>

    <ModalNovaEmpresaComponent 
      v-if="mostrarModalNovo" 
      @fechar="mostrarModalNovo = false" 
      @empresaCadastrada="aoAtualizar" 
    />

    <ModalEditarEmpresaComponent 
      v-if="mostrarModalEditar" 
      :empresa="empresaSelecionada"
      @fechar="mostrarModalEditar = false" 
      @empresaEditada="aoAtualizar" 
    />

  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'

import ModalNovaEmpresaComponent from './ModalNovaEmpresaComponent.vue'
import ModalEditarEmpresaComponent from './ModalEditarEmpresaComponent.vue'

const empresas = ref([])
const carregando = ref(true)
const erro = ref('')

const mostrarModalNovo = ref(false)
const mostrarModalEditar = ref(false)
const empresaSelecionada = ref(null)

const buscarEmpresas = async () => {
  try {
    const token = localStorage.getItem('token')
    const resposta = await axios.get('/api/Empresa', {
      headers: { Authorization: `Bearer ${token}` }
    })
    empresas.value = resposta.data
  } catch (e) {
    erro.value = 'Falha ao buscar as empresas. Verifique sua conexão.'
  } finally {
    carregando.value = false
  }
}

const deletarEmpresa = async (id) => {
  const confirmacao = window.confirm("Tem certeza que deseja apagar esta empresa?")
  if (!confirmacao) return
  try {
    const token = localStorage.getItem('token')
    await axios.delete(`/api/Empresa/${id}`, {
      headers: { Authorization: `Bearer ${token}` }
    })
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

onMounted(() => {
  buscarEmpresas()
})
</script>