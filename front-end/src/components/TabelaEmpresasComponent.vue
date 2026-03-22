<template>
  <div class="bg-zinc-950 border border-zinc-800 rounded-xl p-4 md:p-8 shadow-2xl text-white relative">
    <div class="flex flex-col md:flex-row justify-between md:items-center mb-6 gap-4">
      <h2 class="text-2xl font-bold text-green-500">Empresas</h2>
      <button @click="abrirModalNovo()" class="w-full md:w-auto bg-green-500 hover:bg-green-600 text-white font-bold py-2 px-4 rounded-lg">
        + Nova Empresa
      </button>
    </div>

    <div v-if="carregando" class="text-center text-zinc-400 py-4">Carregando...</div>
    <div v-if="erro" class="text-center text-red-500 py-4">{{ erro }}</div>

    <div v-if="!carregando && !erro">
      <!-- Tabela para Desktop -->
      <div class="hidden md:block overflow-x-auto rounded-lg border border-zinc-800">
        <table class="w-full text-left">
          <thead class="bg-zinc-900 text-zinc-400 text-sm uppercase">
            <tr>
              <th class="p-4 font-medium">ID</th>
              <th class="p-4 font-medium">Nome</th>
              <th class="p-4 font-medium text-right">Ações</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-zinc-800">
            <tr v-for="item in itens" :key="item.id" class="hover:bg-zinc-900/60">
              <td class="p-4 font-mono text-zinc-500">{{ item.id }}</td>
              <td class="p-4 text-zinc-200">{{ item.nome }}</td>
              <td class="p-4 flex justify-end gap-4">
                <button @click="abrirModalEdicao(item)" class="text-sm font-semibold text-blue-500">Editar</button>
                <button @click="deletarItem(item.id)" class="text-sm font-semibold text-red-500">Excluir</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
      <!-- Cards para Mobile -->
      <div class="block md:hidden space-y-4">
        <div v-for="item in itens" :key="item.id + '-mobile'" class="bg-zinc-900 rounded-lg p-4 border border-zinc-800">
          <p class="text-zinc-200 text-lg">{{ item.nome }}</p>
          <div class="mt-4 pt-4 border-t border-zinc-800 flex justify-end gap-4">
            <button @click="abrirModalEdicao(item)" class="text-sm font-semibold text-blue-500">Editar</button>
            <button @click="deletarItem(item.id)" class="text-sm font-semibold text-red-500">Excluir</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal de Criar/Editar -->
    <div v-if="mostrarModal" class="fixed inset-0 bg-black/70 flex items-center justify-center z-50 p-4">
      <div class="bg-zinc-900 border border-zinc-700 rounded-xl p-8 w-full max-w-md shadow-2xl">
        <h2 class="text-2xl font-bold text-white mb-6">{{ itemSelecionado.id ? 'Editar' : 'Nova' }} Empresa</h2>
        <form @submit.prevent="salvarItem">
          <label class="block text-zinc-400 font-medium mb-1 text-sm">Nome</label>
          <input v-model="itemSelecionado.nome" type="text" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2" required>
          <div class="flex justify-end gap-3 mt-6">
            <button type="button" @click="fecharModal" class="px-4 py-2 rounded-lg text-zinc-400 hover:bg-zinc-800">Cancelar</button>
            <button type="submit" class="bg-green-500 hover:bg-green-600 text-white font-bold px-6 py-2 rounded-lg">Salvar</button>
          </div>
        </form>
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'

const itens = ref([])
const carregando = ref(true)
const erro = ref('')
const mostrarModal = ref(false)
const itemSelecionado = ref({})

const API_URL = '/api/Empresa'

const buscarItens = async () => {
  carregando.value = true
  try {
    const token = localStorage.getItem('token')
    const { data } = await axios.get(API_URL, { headers: { Authorization: `Bearer ${token}` } })
    itens.value = data
  } catch (e) {
    erro.value = 'Falha ao buscar empresas.'
  } finally {
    carregando.value = false
  }
}

const abrirModalNovo = () => {
  itemSelecionado.value = {}
  mostrarModal.value = true
}

const abrirModalEdicao = (item) => {
  itemSelecionado.value = { ...item }
  mostrarModal.value = true
}

const fecharModal = () => {
  mostrarModal.value = false
}

const salvarItem = async () => {
  try {
    const token = localStorage.getItem('token')
    const headers = { Authorization: `Bearer ${token}` }
    if (itemSelecionado.value.id) {
      await axios.put(`${API_URL}/${itemSelecionado.value.id}`, itemSelecionado.value, { headers })
    } else {
      await axios.post(API_URL, itemSelecionado.value, { headers })
    }
    fecharModal()
    buscarItens()
  } catch (e) {
    alert('Erro ao salvar empresa.')
  }
}

const deletarItem = async (id) => {
  if (!confirm('Tem certeza que deseja excluir esta empresa?')) return
  try {
    const token = localStorage.getItem('token')
    await axios.delete(`${API_URL}/${id}`, { headers: { Authorization: `Bearer ${token}` } })
    buscarItens()
  } catch (e) {
    alert('Erro ao excluir empresa.')
  }
}

onMounted(buscarItens)
</script>
