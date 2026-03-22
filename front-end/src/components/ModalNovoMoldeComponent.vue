<template>
  <div class="fixed inset-0 bg-black/70 flex items-center justify-center z-50 animate-fade-in p-4">

    <div class="bg-zinc-900 border border-zinc-700 rounded-xl w-full max-w-5xl shadow-2xl flex flex-col max-h-[90vh]">
      
      <!-- Conteúdo Principal com Scroll -->
      <div class="overflow-y-auto">
        <div class="p-4 md:p-8 flex flex-col md:flex-row gap-8">
          <!-- Coluna de Inputs -->
          <div class="flex-1">
            <h2 class="text-2xl md:text-3xl font-bold text-white mb-6">Cadastrar Novo Molde</h2>
            <form @submit.prevent="salvarMolde" class="flex flex-col gap-4">
              <div>
                <label class="block text-zinc-400 font-medium mb-1 text-sm">Código da Peça</label>
                <input type="text" v-model="novoMolde.codigo" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2 focus:outline-none focus:border-orange-500" required placeholder="Ex: MLD-001">
              </div>
              <div>
                <label class="block text-zinc-400 font-medium mb-1 text-sm">Nome / Descrição</label>
                <input type="text" v-model="novoMolde.nome" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2 focus:outline-none focus:border-orange-500" required placeholder="Ex: Molde de Engrenagem">
              </div>
              <div>
                <label class="block text-zinc-400 font-medium mb-1 text-sm">Prateleira / Localização</label>
                <input type="text" v-model="novoMolde.prateleira" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2 focus:outline-none focus:border-orange-500" required placeholder="Ex: A-12">
              </div>
            </form>
          </div>
          
          <!-- Coluna de Imagens -->
          <div class="flex-[2] bg-zinc-950 rounded-xl border border-zinc-800 p-4 md:p-6">
            <div class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-6">
              <div v-for="slot in ['imagem1', 'imagem2', 'imagem3']" :key="slot" class="flex flex-col items-center justify-center text-zinc-500 relative group">
                <div class="w-full h-48 border-2 border-dashed border-zinc-700 rounded-lg mb-4 flex items-center justify-center bg-zinc-900/50 overflow-hidden">
                  <img v-if="fotosPreview[slot]" :src="fotosPreview[slot]" class="w-full h-full object-cover">
                  <div v-else class="text-center">
                    <span class="text-3xl mb-2">📸</span>
                    <p class="text-xs font-medium">Sem foto</p>
                  </div>
                </div>
                <input type="file" :ref="el => { inputsFoto[slot] = el }" @change="e => selecionarFoto(e, slot)" accept="image/*" class="hidden">
                <div class="w-full flex gap-2">
                  <button @click="inputsFoto[slot].click()" type="button" class="w-full bg-zinc-800 hover:bg-zinc-700 text-white font-semibold py-2 rounded-lg transition-colors text-xs shadow-inner">
                    {{ fotosArquivos[slot] ? 'Trocar' : 'Anexar' }}
                  </button>
                  <button v-if="fotosArquivos[slot]" @click="removerFoto(slot)" type="button" class="bg-red-900/50 hover:bg-red-900 text-white px-3 rounded-lg transition-colors text-xs">
                    X
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Barra de Ações Fixa no Fundo do Modal -->
      <div class="bg-zinc-900 border-t border-zinc-700 p-4 flex justify-end gap-3 rounded-b-xl">
          <p v-if="erro" class="text-red-500 text-sm self-center mr-auto">{{ erro }}</p>
          <button type="button" @click="$emit('fechar')" class="px-4 py-2 rounded-lg text-zinc-400 hover:text-white hover:bg-zinc-800 transition-colors font-medium">
            Cancelar
          </button>
          <button @click="salvarMolde" :disabled="carregando" class="bg-orange-500 hover:bg-orange-600 text-white font-bold px-6 py-2 rounded-lg transition-all shadow-[0_0_10px_rgba(249,115,22,0.3)] flex items-center min-w-[120px] justify-center">
            <span v-if="carregando" class="animate-spin h-4 w-4 border-2 border-white border-t-transparent rounded-full mr-2"></span>
            {{ carregando ? 'Salvando...' : 'Salvar Molde' }}
          </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import axios from 'axios'

const emit = defineEmits(['fechar', 'moldeCadastrado'])

const novoMolde = ref({
  codigo: '',
  nome: '',
  prateleira: ''
})

const carregando = ref(false)
const erro = ref('')
const inputsFoto = reactive({})
const fotosArquivos = reactive({})
const fotosPreview = reactive({})

const selecionarFoto = (event, slot) => {
  const arquivo = event.target.files[0]
  if (arquivo) {
    fotosArquivos[slot] = arquivo
    fotosPreview[slot] = URL.createObjectURL(arquivo)
  }
}

const removerFoto = (slot) => {
  fotosArquivos[slot] = null
  fotosPreview[slot] = null
}

const salvarMolde = async () => {
  if (!novoMolde.value.codigo || !novoMolde.value.nome || !novoMolde.value.prateleira) {
    erro.value = 'Preencha todos os campos obrigatórios.'
    return
  }
  
  carregando.value = true
  erro.value = ''

  try {
    const token = localStorage.getItem('token')
    
    const respostaMolde = await axios.post('/api/Molde', novoMolde.value, {
      headers: { Authorization: `Bearer ${token}` }
    })

    const moldeCriado = respostaMolde.data
    let algumaFotoEnviada = false
    
    const uploads = Object.keys(fotosArquivos).map(async (slot) => {
      const arquivo = fotosArquivos[slot]
      if (arquivo) {
        const pacote = new FormData()
        pacote.append('arquivo', arquivo)

        const respostaUpload = await axios.post('/api/Upload/imagem/moldes', pacote, {
          headers: { 'Authorization': `Bearer ${token}` },
          timeout: 20000
        })
        
        moldeCriado[slot] = respostaUpload.data.nomeImagem
        algumaFotoEnviada = true
      }
    })

    await Promise.all(uploads)

    if (algumaFotoEnviada) {
      await axios.put(`/api/Molde/${moldeCriado.id}`, moldeCriado, {
        headers: { Authorization: `Bearer ${token}` }
      })
    }

    emit('moldeCadastrado')
    emit('fechar')
    
  } catch (e) {
    console.error("ERRO AO SALVAR MOLDE:", e)
    if (e.code === 'ECONNABORTED') {
      erro.value = 'Tempo esgotado. A rede do Docker pode estar offline.'
    } else {
      erro.value = e.response?.data?.title || e.response?.data || 'Falha ao salvar o molde.'
    }
  } finally {
    carregando.value = false
  }
}
</script>

<style scoped>
.animate-fade-in { animation: fadeIn 0.2s ease-out; }
@keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
</style>