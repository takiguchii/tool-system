<template>
  <div class="fixed inset-0 bg-black/80 flex items-center justify-center z-50 animate-fade-in p-4 sm:p-6">
    <div class="bg-zinc-900 border border-zinc-700 rounded-xl w-full max-w-lg shadow-2xl overflow-hidden max-h-[95vh] flex flex-col">
      
      <div class="p-5 sm:p-6 border-b border-zinc-800 shrink-0">
        <h2 class="text-xl sm:text-2xl font-extrabold text-white flex items-center gap-2">
          Cadastrar Novo Macho
        </h2>
      </div>

      <div class="p-5 sm:p-6 overflow-y-auto flex-1 custom-scrollbar">
        <form @submit.prevent="salvarMacho" class="flex flex-col gap-5">
          
          <div>
            <label class="block text-zinc-400 font-medium mb-1 text-sm">Código do Macho (ID na Fábrica)</label>
            <input type="text" v-model="novoMacho.codigo" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2.5 focus:outline-none focus:border-orange-500 transition-colors" required placeholder="Ex: M-123">
          </div>

          <div>
            <label class="block text-zinc-400 font-medium mb-1 text-sm">Vincular ao Molde</label>
            <select v-model="moldeIdSelecionado" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2.5 focus:outline-none focus:border-orange-500 transition-colors appearance-none cursor-pointer" required>
              <option value="" disabled>Selecione o Molde...</option>
              <option v-for="molde in moldes" :key="molde.id" :value="molde.id">
                {{ molde.codigo }} - {{ molde.nome }}
              </option>
            </select>
          </div>

          <div class="border-t border-zinc-800 pt-4 mt-2">
            <label class="block text-zinc-400 font-medium mb-3 text-sm">Fotos Iniciais (Opcional)</label>
            <div class="grid grid-cols-3 gap-3">
              <div v-for="slot in ['imagem1', 'imagem2', 'imagem3']" :key="slot" class="bg-zinc-950 rounded-lg border border-zinc-800 p-2 flex flex-col items-center relative group h-28">
                
                <img v-if="previews[slot]" :src="previews[slot]" class="w-full h-16 object-cover rounded border border-zinc-700 mb-2">
                <div v-else class="w-full h-16 border border-dashed border-zinc-700 rounded mb-2 flex flex-col items-center justify-center bg-zinc-900 text-zinc-600">
                  <span class="text-xl mb-1">📸</span>
                </div>

                <input type="file" :ref="el => inputsFile[slot] = el" @change="e => selecionarFoto(e, slot)" accept="image/*" class="hidden">
                
                <button type="button" @click="inputsFile[slot].click()" class="w-full bg-zinc-800 hover:bg-zinc-700 text-white text-[10px] font-semibold py-1 rounded transition-colors active:scale-95">
                  {{ previews[slot] ? 'Trocar' : 'Anexar' }}
                </button>
              </div>
            </div>
          </div>

          <p v-if="erro" class="text-red-500 text-sm mt-2 font-medium">{{ erro }}</p>

          <div class="flex justify-end gap-3 mt-4 pt-4 border-t border-zinc-800">
            <button type="button" @click="$emit('fechar')" class="px-5 py-2.5 rounded-lg text-zinc-400 hover:text-white hover:bg-zinc-800 font-medium transition-colors active:scale-95">
              Cancelar
            </button>
            <button type="submit" :disabled="carregando" class="bg-orange-500 hover:bg-orange-600 active:scale-95 text-white font-bold px-6 py-2.5 rounded-lg transition-all shadow-[0_0_10px_rgba(249,115,22,0.3)] disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center min-w-[140px]">
              <span v-if="carregando" class="animate-spin h-5 w-5 border-2 border-white border-t-transparent rounded-full"></span>
              <span v-else>Salvar Macho</span>
            </button>
          </div>
        </form>
      </div>

    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'

const emit = defineEmits(['fechar', 'machoCadastrado'])

const novoMacho = ref({
  codigo: '',
  imagem1: null, imagem2: null, imagem3: null
})

const moldes = ref([])
const moldeIdSelecionado = ref('')

const erro = ref('')
const carregando = ref(false)

// CONTROLES DE IMAGEM
const arquivosSelecionados = ref({ imagem1: null, imagem2: null, imagem3: null })
const previews = ref({ imagem1: null, imagem2: null, imagem3: null })
const inputsFile = ref({})

// Carrega os moldes disponíveis ao abrir a tela
onMounted(async () => {
  const token = localStorage.getItem('token')
  try {
    const resposta = await axios.get('/api/Molde', { headers: { Authorization: `Bearer ${token}` } })
    moldes.value = resposta.data
  } catch (e) {
    erro.value = 'Falha ao carregar a lista de moldes.'
  }
})

const selecionarFoto = (event, slot) => {
  const file = event.target.files[0]
  if (!file) return
  arquivosSelecionados.value[slot] = file
  previews.value[slot] = URL.createObjectURL(file)
}

const salvarMacho = async () => {
  if (!moldeIdSelecionado.value) {
    erro.value = 'Por favor, selecione um molde para vincular o macho.'
    return
  }

  erro.value = ''
  carregando.value = true

  const token = localStorage.getItem('token')
  const config = { headers: { Authorization: `Bearer ${token}` } }

  try {
    for (const slot of ['imagem1', 'imagem2', 'imagem3']) {
      if (arquivosSelecionados.value[slot]) {
        const pacote = new FormData()
        pacote.append('arquivo', arquivosSelecionados.value[slot])
        
        const respostaUpload = await axios.post('/api/Upload/imagem/machos', pacote, config)
        novoMacho.value[slot] = respostaUpload.data.nomeImagem
      }
    }

    await axios.post(`/api/Macho?moldeId=${moldeIdSelecionado.value}`, novoMacho.value, config)
    
    emit('machoCadastrado')
  } catch (e) {
    erro.value = e.response?.data || 'Erro ao salvar o macho. Verifique os dados.'
  } finally {
    carregando.value = false
  }
}
</script>

<style scoped>
.animate-fade-in { animation: fadeIn 0.15s ease-out; }
@keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: #18181b; border-radius: 8px; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #3f3f46; border-radius: 8px; }
.custom-scrollbar::-webkit-scrollbar-thumb:hover { background: #f97316; }
</style>