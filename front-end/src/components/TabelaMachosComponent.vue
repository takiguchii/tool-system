<template>
  <div class="bg-zinc-950 border border-zinc-800 rounded-xl p-4 md:p-8 shadow-2xl text-white relative">
    <div class="flex flex-col md:flex-row justify-between md:items-center mb-6 gap-4">
      <h2 class="text-2xl font-bold text-red-500">Machos</h2>
      <button @click="abrirModalNovo()" class="w-full md:w-auto bg-red-500 hover:bg-red-600 text-white font-bold py-2 px-4 rounded-lg">
        + Novo Macho
      </button>
    </div>

    <div v-if="carregando" class="text-center text-zinc-400 py-4">Carregando...</div>
    <div v-if="erro" class="text-center text-red-500 py-4">{{ erro }}</div>

    <div v-if="!carregando && !erro" class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
      <div v-for="item in itens" :key="item.id" class="bg-zinc-900 rounded-lg border border-zinc-800 flex flex-col">
        <img :src="item.imagem1 ? `/imagens/machos/${item.imagem1}` : '/placeholder.png'" @error="onImgError" class="w-full h-40 object-cover rounded-t-lg bg-zinc-800">
        <div class="p-4 flex-1 flex flex-col">
          <p class="text-zinc-200 font-bold">{{ item.codigo }}</p>
          <div class="mt-auto pt-4 flex justify-end gap-2">
            <button @click="abrirModalEdicao(item)" class="text-sm font-semibold text-blue-500">Editar</button>
            <button @click="deletarItem(item.id)" class="text-sm font-semibold text-red-500">Excluir</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal de Criar/Editar -->
    <div v-if="mostrarModal" class="fixed inset-0 bg-black/70 flex items-center justify-center z-50 p-4">
      <div class="bg-zinc-900 border border-zinc-700 rounded-xl p-8 w-full max-w-4xl shadow-2xl max-h-[90vh] flex flex-col">
        <h2 class="text-2xl font-bold text-white mb-6">{{ itemSelecionado.id ? 'Editar' : 'Novo' }} Macho</h2>
        
        <div class="overflow-y-auto flex-1">
          <div class="flex flex-col md:flex-row gap-8">
            <div class="flex-1">
              <label class="block text-zinc-400 font-medium mb-1 text-sm">Código</label>
              <input v-model="itemSelecionado.codigo" type="text" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2" required>
            </div>
            <div class="flex-1 grid grid-cols-3 gap-4">
              <div v-for="slot in ['imagem1', 'imagem2', 'imagem3']" :key="slot">
                <label class="block text-zinc-400 font-medium mb-1 text-sm text-center">Imagem {{ slot.slice(-1) }}</label>
                <div class="w-full h-24 border-2 border-dashed border-zinc-700 rounded-lg flex items-center justify-center bg-zinc-900/50 overflow-hidden relative">
                  <img v-if="previews[slot]" :src="previews[slot]" class="w-full h-full object-cover">
                  <input type="file" @change="e => selecionarFoto(e, slot)" accept="image/*" class="absolute inset-0 opacity-0 cursor-pointer">
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="flex justify-end gap-3 mt-6 pt-4 border-t border-zinc-800">
          <button type="button" @click="fecharModal" class="px-4 py-2 rounded-lg text-zinc-400 hover:bg-zinc-800">Cancelar</button>
          <button @click="salvarItem" class="bg-red-500 hover:bg-red-600 text-white font-bold px-6 py-2 rounded-lg">Salvar</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import axios from 'axios'

const itens = ref([])
const carregando = ref(true)
const erro = ref('')
const mostrarModal = ref(false)
const itemSelecionado = ref({})
const fotosParaSalvar = reactive({})
const previews = reactive({})

const API_URL = '/api/Macho'

const onImgError = (e) => { e.target.src = 'https://via.placeholder.com/300x200?text=Sem+Imagem' }

const buscarItens = async () => {
  carregando.value = true
  try {
    const token = localStorage.getItem('token')
    const { data } = await axios.get(API_URL, { headers: { Authorization: `Bearer ${token}` } })
    itens.value = data
  } catch (e) { erro.value = 'Falha ao buscar machos.' } finally { carregando.value = false }
}

const setupPreviews = (item) => {
  previews.imagem1 = item.imagem1 ? `/imagens/machos/${item.imagem1}` : null
  previews.imagem2 = item.imagem2 ? `/imagens/machos/${item.imagem2}` : null
  previews.imagem3 = item.imagem3 ? `/imagens/machos/${item.imagem3}` : null
}

const abrirModalNovo = () => {
  itemSelecionado.value = {}
  setupPreviews({})
  Object.keys(fotosParaSalvar).forEach(key => delete fotosParaSalvar[key])
  mostrarModal.value = true
}

const abrirModalEdicao = (item) => {
  itemSelecionado.value = { ...item }
  setupPreviews(item)
  Object.keys(fotosParaSalvar).forEach(key => delete fotosParaSalvar[key])
  mostrarModal.value = true
}

const fecharModal = () => { mostrarModal.value = false }

const selecionarFoto = (event, slot) => {
  const file = event.target.files[0]
  if (!file) return
  fotosParaSalvar[slot] = file
  previews[slot] = URL.createObjectURL(file)
}

const salvarItem = async () => {
  const token = localStorage.getItem('token')
  const headers = { Authorization: `Bearer ${token}` }
  
  // Upload de imagens primeiro
  for (const slot in fotosParaSalvar) {
    const file = fotosParaSalvar[slot]
    if (file) {
      const pacote = new FormData()
      pacote.append('arquivo', file)
      try {
        const res = await axios.post('/api/Upload/imagem/machos', pacote, { headers })
        itemSelecionado.value[slot] = res.data.nomeImagem
      } catch (e) {
        alert('Erro ao fazer upload da imagem.')
        return
      }
    }
  }

  // Salvar o item (criar ou editar)
  try {
    if (itemSelecionado.value.id) {
      await axios.put(`${API_URL}/${itemSelecionado.value.id}`, itemSelecionado.value, { headers })
    } else {
      await axios.post(API_URL, itemSelecionado.value, { headers })
    }
    fecharModal()
    buscarItens()
  } catch (e) {
    alert('Erro ao salvar macho.')
  }
}

const deletarItem = async (id) => {
  if (!confirm('Tem certeza que deseja excluir este macho?')) return
  try {
    const token = localStorage.getItem('token')
    await axios.delete(`${API_URL}/${id}`, { headers: { Authorization: `Bearer ${token}` } })
    buscarItens()
  } catch (e) {
    alert('Erro ao excluir macho.')
  }
}

onMounted(buscarItens)
</script>
