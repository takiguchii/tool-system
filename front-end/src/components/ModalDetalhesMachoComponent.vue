<template>
  <div class="fixed inset-0 bg-black/80 flex items-center justify-center z-50 animate-fade-in p-4 overflow-y-auto">
    <div class="bg-zinc-900 border border-zinc-700 rounded-xl p-8 w-full max-w-4xl shadow-2xl relative my-8">

      <button @click="$emit('fechar')" class="absolute top-4 right-4 text-zinc-500 hover:text-white transition-colors z-10">
        <span class="text-xl font-bold">X</span>
      </button>

      <div class="flex flex-col gap-8">
        <div class="border-b border-zinc-800 pb-6">
          <p class="text-zinc-500 text-sm font-medium uppercase tracking-wider mb-1">Ficha Técnica do Macho</p>
          <h2 class="text-3xl font-extrabold text-white capitalize">Código: {{ macho.codigo }}</h2>
          <span class="inline-block bg-zinc-800 text-zinc-400 border border-zinc-700 px-3 py-1 rounded-full text-xs font-mono mt-2">
            ID Interno: {{ macho.id }}
          </span>
        </div>

        <div>
          <h3 class="text-xl font-bold text-orange-500 mb-4">Galeria de Fotos (Máx. 3)</h3>
          
          <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
            
            <div v-for="slot in ['imagem1', 'imagem2', 'imagem3']" :key="slot" class="bg-zinc-950 rounded-xl border border-zinc-800 p-4 flex flex-col items-center relative group">
              
              <img v-if="machoAtualizado[slot]" :src="'/imagens/machos/' + machoAtualizado[slot]" class="w-full h-48 object-cover rounded-lg mb-4 border border-zinc-700 shadow-md">
              
              <div v-else class="w-full h-48 border-2 border-dashed border-zinc-700 rounded-lg mb-4 flex flex-col items-center justify-center bg-zinc-900/50 text-zinc-600">
                <span class="text-5xl mb-2">📸</span>
                <p class="text-xs font-medium">Slot Vazio</p>
              </div>

              <input type="file" :ref="el => inputsFile[slot] = el" @change="event => fazerUpload(event, slot)" accept="image/*" class="hidden">

              <div class="w-full flex gap-2">
                <button @click="inputsFile[slot].click()" :disabled="carregandoSlot === slot" class="flex-1 bg-zinc-800 hover:bg-zinc-700 text-white text-xs font-semibold py-2 rounded-lg transition-colors flex items-center justify-center">
                  <span v-if="carregandoSlot === slot" class="animate-spin h-3 w-3 border-2 border-white border-t-transparent rounded-full mr-2"></span>
                  {{ machoAtualizado[slot] ? 'Trocar' : 'Anexar' }}
                </button>
                
                <button v-if="machoAtualizado[slot]" @click="removerFoto(slot)" class="bg-red-950 hover:bg-red-900 text-red-300 px-3 py-2 rounded-lg transition-colors text-xs">
                  🗑️
                </button>
              </div>
            </div>

          </div>
          
          <p v-if="erro" class="text-red-500 text-sm text-center mt-4 p-2 bg-red-900/20 rounded-lg border border-red-900/50">{{ erro }}</p>
        </div>
      </div>

    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
import axios from 'axios'

const props = defineProps({
  macho: Object
})

const emit = defineEmits(['fechar', 'fotosAtualizadas'])

const machoAtualizado = reactive({ ...props.macho })
const carregandoSlot = ref('') 
const erro = ref('')
const inputsFile = reactive({}) 

const fazerUpload = async (event, slotImagem) => {
  const arquivoFisico = event.target.files[0]
  if (!arquivoFisico) return

  carregandoSlot.value = slotImagem
  erro.value = ''

  try {
    const token = localStorage.getItem('token')
    
    const pacote = new FormData()
    pacote.append('arquivo', arquivoFisico) 

    const respostaUpload = await axios.post('/api/Upload/imagem/machos', pacote, {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    })

    const nomeDaNovaImagem = respostaUpload.data.nomeImagem
    
    machoAtualizado[slotImagem] = nomeDaNovaImagem

    // 4. Atualiza o registro do Macho na API
    await axios.put(`/api/Macho/${props.macho.id}`, machoAtualizado, {
      headers: { Authorization: `Bearer ${token}` }
    })

    emit('fotosAtualizadas') 
    
  } catch (e) {
    erro.value = e.response?.data || 'Falha ao enviar a foto para a fábrica.'
    // Reverte a mudança local em caso de erro na API PUT
    machoAtualizado[slotImagem] = props.macho[slotImagem]
  } finally {
    carregandoSlot.value = ''
    event.target.value = '' 
  }
}

const removerFoto = async (slotImagem) => {
  if(!window.confirm("Tem certeza que deseja remover esta foto?")) return
  
  erro.value = ''
  const imagemAntiga = machoAtualizado[slotImagem]
  machoAtualizado[slotImagem] = null 

  try {
    const token = localStorage.getItem('token')
    
    await axios.put(`/api/Macho/${props.macho.id}`, machoAtualizado, {
      headers: { Authorization: `Bearer ${token}` }
    })
    
    emit('fotosAtualizadas')
  } catch (e) {
    erro.value = 'Falha ao remover a foto no servidor.'
    machoAtualizado[slotImagem] = imagemAntiga 
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