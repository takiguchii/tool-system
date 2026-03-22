<template>
  <div class="fixed inset-0 bg-black/80 flex items-center justify-center z-50 animate-fade-in p-4">
    <div class="bg-zinc-900 border border-zinc-700 rounded-xl p-8 w-full max-w-3xl shadow-2xl relative flex flex-col md:flex-row gap-8">

      <button @click="$emit('fechar')" class="absolute top-4 right-4 text-zinc-500 hover:text-white transition-colors">
        <span class="text-xl font-bold">X</span>
      </button>

      <div class="flex-1">
        <h2 class="text-3xl font-extrabold text-white mb-2 capitalize">{{ molde.nome || 'Peça sem nome' }}</h2>
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
            <p class="text-green-400 font-bold uppercase">{{ molde.status || 'Disponível' }}</p>
          </div>
        </div>
      </div>

      <div class="flex-1 bg-zinc-950 rounded-xl border border-zinc-800 p-6 flex flex-col items-center justify-center text-zinc-500 relative">
        
        <img v-if="molde.imagem1" :src="'/imagens/moldes/' + molde.imagem1" class="w-full h-56 object-cover rounded-lg mb-4 border border-zinc-700 shadow-md">
        
        <div v-else class="w-full h-56 border-2 border-dashed border-zinc-700 rounded-lg mb-4 flex flex-col items-center justify-center bg-zinc-900/50">
          <span class="text-4xl mb-3">📸</span>
          <p class="text-sm font-medium">Nenhuma foto registrada</p>
        </div>

        <input type="file" ref="inputFoto" @change="fazerUpload" accept="image/*" class="hidden">

        <button @click="$refs.inputFoto.click()" :disabled="carregando" class="w-full bg-zinc-800 hover:bg-zinc-700 text-white font-semibold py-3 rounded-lg transition-colors text-sm shadow-inner flex items-center justify-center">
          <span v-if="carregando" class="animate-spin h-4 w-4 border-2 border-white border-t-transparent rounded-full mr-2"></span>
          {{ carregando ? 'Processando...' : (molde.imagem1 ? 'Trocar Foto da Peça' : 'Anexar Foto da Peça') }}
        </button>
        
        <p v-if="erro" class="text-red-500 text-xs text-center mt-2">{{ erro }}</p>
      </div>

    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import axios from 'axios'

const props = defineProps({
  molde: Object
})

const emit = defineEmits(['fechar', 'fotoAtualizada'])

const carregando = ref(false)
const erro = ref('')
const inputFoto = ref(null)

const fazerUpload = async (event) => {
  const arquivoFisico = event.target.files[0]
  if (!arquivoFisico) return

  carregando.value = true
  erro.value = ''

  try {
    const token = localStorage.getItem('token')
    
    const pacote = new FormData()
    pacote.append('arquivo', arquivoFisico) 

    const respostaUpload = await axios.post('/api/Upload/imagem/moldes', pacote, {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    })

    const nomeDaNovaImagem = respostaUpload.data.nomeImagem
    const moldeAtualizado = { ...props.molde, imagem1: nomeDaNovaImagem }

    await axios.put(`/api/Molde/${props.molde.id}`, moldeAtualizado, {
      headers: { Authorization: `Bearer ${token}` }
    })

    emit('fotoAtualizada')
    
  } catch (e) {
    erro.value = e.response?.data || 'Falha ao enviar a foto para a fábrica.'
  } finally {
    carregando.value = false
    event.target.value = '' 
  }
}
</script>

<style scoped>
.animate-fade-in {
  animation: fadeIn 0.2s ease-out;
}
@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}
</style>