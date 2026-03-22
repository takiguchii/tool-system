<template>
  <div class="fixed inset-0 bg-black/80 flex items-center justify-center z-50 animate-fade-in p-4 overflow-auto">
    <div class="bg-zinc-900 border border-zinc-700 rounded-xl p-4 md:p-8 w-full max-w-5xl shadow-2xl relative flex flex-col md:flex-row gap-8 my-auto">

      <button @click="$emit('fechar')" class="absolute top-4 right-4 text-zinc-500 hover:text-white transition-colors z-10">
        <span class="text-xl font-bold">X</span>
      </button>

      <div class="flex-1">
        <h2 class="text-2xl md:text-3xl font-extrabold text-white mb-2 capitalize">{{ molde.nome || 'Peça sem nome' }}</h2>
        <span class="inline-block bg-orange-500/20 text-orange-400 border border-orange-500/50 px-3 py-1 rounded-full text-xs font-bold uppercase tracking-wide mb-8">
          Código: {{ molde.codigo || 'N/A' }}
        </span>
        <div class="space-y-6">
          <div>
            <p class="text-zinc-500 text-sm font-medium uppercase tracking-wider mb-1">Localização</p>
            <p class="text-zinc-200 text-lg font-medium">Prateleira {{ molde.prateleira || 'Não informada' }}</p>
          </div>
          <div>
            <p class="text-zinc-500 text-sm font-medium uppercase tracking-wider mb-1">Status Atual</p>
            <p :class="molde.status === 'Disponível' ? 'text-green-400' : 'text-yellow-400'" class="font-bold uppercase">
              {{ molde.status || 'Disponível' }}
            </p>
          </div>
        </div>
      </div>

      <div class="flex-[2] bg-zinc-950 rounded-xl border border-zinc-800 p-4 md:p-6">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div v-for="slot in ['imagem1', 'imagem2', 'imagem3']" :key="slot" class="flex flex-col items-center justify-center text-zinc-500 relative group">
            
            <div class="w-full h-48 border-2 border-dashed border-zinc-700 rounded-lg mb-4 flex items-center justify-center bg-zinc-900/50 overflow-hidden">
              <img v-if="previews[slot]" :src="previews[slot]" @click="abrirZoom(previews[slot])" class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300 cursor-zoom-in">
              <div v-else class="text-center">
                <span class="text-3xl mb-2">📸</span>
                <p class="text-xs font-medium">Sem foto</p>
              </div>
            </div>

            <input type="file" :ref="el => { inputsFoto[slot] = el }" @change="e => selecionarFoto(e, slot)" accept="image/*" class="hidden">

            <div class="w-full flex gap-2">
              <button v-if="!fotosParaSalvar[slot]" @click="inputsFoto[slot].click()" :disabled="!!carregando" class="flex-1 bg-zinc-800 hover:bg-zinc-700 text-white font-semibold py-2 rounded-lg transition-colors text-xs shadow-inner">
                {{ molde[slot] ? 'Trocar' : 'Anexar' }}
              </button>
              
              <button v-if="fotosParaSalvar[slot]" @click="fazerUpload(slot)" :disabled="!!carregando" class="flex-1 bg-green-500 hover:bg-green-600 text-white font-bold py-2 rounded-lg text-xs flex items-center justify-center">
                <span v-if="carregando === slot" class="animate-spin h-3 w-3 border-2 border-white border-t-transparent rounded-full mr-2"></span>
                Salvar
              </button>

              <button v-if="molde[slot] || fotosParaSalvar[slot]" @click="removerFoto(slot)" :disabled="!!carregando" class="bg-red-900/50 hover:bg-red-900 text-white px-3 rounded-lg transition-colors text-xs">
                X
              </button>
            </div>
          </div>
        </div>
        <p v-if="erro" class="text-red-500 text-xs text-center mt-4">{{ erro }}</p>
      </div>
    </div>
    <ZoomImageModal :visivel="zoomVisivel" :url-imagem="zoomUrl" @fechar="fecharZoom" />
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import axios from 'axios'
import ZoomImageModal from './ZoomImageModal.vue'

const props = defineProps({ molde: Object })
const emit = defineEmits(['fechar', 'fotoAtualizada'])

const carregando = ref(false)
const erro = ref('')
const inputsFoto = reactive({})

const fotosParaSalvar = reactive({})
const previews = reactive({})

onMounted(() => {
  previews.imagem1 = props.molde.imagem1 ? `/imagens/moldes/${props.molde.imagem1}` : null
  previews.imagem2 = props.molde.imagem2 ? `/imagens/moldes/${props.molde.imagem2}` : null
  previews.imagem3 = props.molde.imagem3 ? `/imagens/moldes/${props.molde.imagem3}` : null
})

const zoomVisivel = ref(false)
const zoomUrl = ref('')

const abrirZoom = (url) => {
  if (!url) return
  zoomUrl.value = url
  zoomVisivel.value = true
}
const fecharZoom = () => zoomVisivel.value = false

const selecionarFoto = (event, slot) => {
  const file = event.target.files[0]
  if (!file) return
  fotosParaSalvar[slot] = file
  previews[slot] = URL.createObjectURL(file)
}

const fazerUpload = async (slot) => {
  const arquivoFisico = fotosParaSalvar[slot]
  if (!arquivoFisico) return

  carregando.value = slot
  erro.value = ''
  try {
    const token = localStorage.getItem('token')
    const pacote = new FormData()
    pacote.append('arquivo', arquivoFisico)

    const respostaUpload = await axios.post('/api/Upload/imagem/moldes', pacote, {
      headers: { 'Authorization': `Bearer ${token}` },
      timeout: 20000 
    })

    const nomeDaNovaImagem = respostaUpload.data.nomeImagem
    const moldeAtualizado = { ...props.molde, [slot]: nomeDaNovaImagem }

    await axios.put(`/api/Molde/${props.molde.id}`, moldeAtualizado, {
      headers: { Authorization: `Bearer ${token}` }
    })
    
    delete fotosParaSalvar[slot]
    emit('fotoAtualizada')

  } catch (e) {
    console.error(`ERRO DE UPLOAD [${slot}]:`, e)
    erro.value = e.response?.data?.title || 'Falha ao salvar a foto.'
  } finally {
    carregando.value = false
  }
}

const removerFoto = async (slot) => {
  // Se houver uma foto pendente de upload, apenas a remove do front-end
  if (fotosParaSalvar[slot]) {
    delete fotosParaSalvar[slot]
    previews[slot] = props.molde[slot] ? `/imagens/moldes/${props.molde[slot]}` : null
    return
  }

  // Se não houver foto pendente, remove a do back-end
  if (!props.molde[slot]) return

  carregando.value = slot
  erro.value = ''
  try {
    const token = localStorage.getItem('token')
    const moldeAtualizado = { ...props.molde, [slot]: null }

    await axios.put(`/api/Molde/${props.molde.id}`, moldeAtualizado, {
      headers: { Authorization: `Bearer ${token}` }
    })
    
    previews[slot] = null
    emit('fotoAtualizada')

  } catch(e) {
    console.error(`ERRO AO REMOVER FOTO [${slot}]:`, e)
    erro.value = 'Falha ao remover a foto.'
  } finally {
    carregando.value = false
  }
}
</script>

<style scoped>
.animate-fade-in { animation: fadeIn 0.2s ease-out; }
@keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
</style>