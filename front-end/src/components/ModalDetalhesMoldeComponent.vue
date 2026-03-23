<template>
  <div class="fixed inset-0 bg-black/80 flex items-center justify-center z-50 animate-fade-in p-4 sm:p-6">
    
    <div class="bg-zinc-900 border border-zinc-700 rounded-xl w-full max-w-5xl shadow-2xl relative flex flex-col max-h-[95vh] overflow-hidden">
      
      <div class="p-5 sm:p-6 border-b border-zinc-800 flex justify-between items-start shrink-0 bg-zinc-900 z-10">
        <div>
          <p class="text-zinc-500 text-xs font-medium uppercase tracking-wider mb-1">Dossier Completo do Molde</p>
          <h2 class="text-2xl sm:text-3xl font-extrabold text-white capitalize">{{ molde.nome || 'Peça sem nome' }}</h2>
          <span class="inline-block bg-orange-500/20 text-orange-400 border border-orange-500/50 px-3 py-1 rounded-full text-xs font-bold uppercase tracking-wide mt-2">
            Código: {{ molde.codigo || 'N/A' }}
          </span>
        </div>
        
        <button @click="$emit('fechar')" class="text-zinc-500 bg-zinc-800/50 hover:bg-red-500 hover:text-white p-2 rounded-lg transition-colors active:scale-95 flex items-center justify-center w-10 h-10">
          <span class="text-xl font-bold leading-none">X</span>
        </button>
      </div>

      <div class="p-5 sm:p-6 overflow-y-auto flex-1 custom-scrollbar flex flex-col gap-6 bg-zinc-900/50">
        
        <div class="bg-zinc-950 border border-zinc-800 rounded-xl p-5 grid grid-cols-2 md:grid-cols-4 gap-4 shadow-inner">
          <div>
            <p class="text-zinc-500 text-[10px] font-medium uppercase tracking-wider mb-1">Status Atual</p>
            <p class="text-green-400 font-bold uppercase text-sm sm:text-base">{{ molde.status || 'Disponível' }}</p>
          </div>
          <div>
            <p class="text-zinc-500 text-[10px] font-medium uppercase tracking-wider mb-1">Localização</p>
            <p class="text-zinc-200 font-medium text-sm sm:text-base">Prateleira {{ molde.prateleira || 'N/A' }}</p>
          </div>
          <div>
            <p class="text-zinc-500 text-[10px] font-medium uppercase tracking-wider mb-1">Empresa Proprietária</p>
            <p class="text-zinc-200 font-medium text-sm sm:text-base truncate" :title="nomeEmpresa">{{ nomeEmpresa }}</p>
          </div>
          <div>
            <p class="text-zinc-500 text-[10px] font-medium uppercase tracking-wider mb-1">Setor / Categoria</p>
            <p class="text-zinc-200 font-medium text-sm sm:text-base truncate" :title="nomeCategoria">{{ nomeCategoria }}</p>
          </div>
        </div>

        <div class="bg-zinc-950 border border-zinc-800 rounded-xl p-5 shadow-inner">
          <h3 class="text-lg font-bold text-orange-500 mb-4 flex items-center gap-2">
            Ferramentas Amarradas (Machos)
            <span class="text-xs bg-zinc-800 text-zinc-400 px-2 py-0.5 rounded-full font-mono">{{ machosVinculados.length }}</span>
          </h3>
          
          <div v-if="buscandoMachos" class="text-zinc-500 text-sm animate-pulse flex items-center gap-2 p-2">
             <span class="w-2 h-2 rounded-full bg-orange-500 animate-pulse"></span> Buscando na fábrica...
          </div>
          
          <div v-else-if="machosVinculados.length === 0" class="text-zinc-600 text-sm italic p-4 bg-zinc-900 rounded-lg border border-dashed border-zinc-800 text-center">
            Nenhuma ferramenta amarrada a este molde.
          </div>
          
          <div v-else class="grid grid-cols-1 sm:grid-cols-2 gap-3 max-h-40 overflow-y-auto pr-2 custom-scrollbar">
            <div v-for="macho in machosVinculados" :key="macho.id" class="bg-zinc-900 border border-zinc-800 rounded-lg p-2.5 flex items-center gap-3">
              <img v-if="macho.imagem1" :src="'/imagens/machos/' + macho.imagem1" class="w-10 h-10 rounded object-cover border border-zinc-700 shadow">
              <div v-else class="w-10 h-10 rounded bg-zinc-950 border border-zinc-800 flex items-center justify-center text-[10px] text-zinc-700">📸</div>
              <div class="min-w-0">
                <p class="text-white font-bold text-sm truncate">{{ macho.codigo }}</p>
                <p class="text-zinc-500 text-[10px]">ID: <span class="font-mono text-orange-400">{{ macho.id }}</span></p>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-zinc-950 border border-zinc-800 rounded-xl p-5 shadow-inner">
          <h3 class="text-lg font-bold text-orange-500 mb-4">Galeria do Molde</h3>
          
          <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
            <div v-for="(slot, index) in ['imagem1', 'imagem2', 'imagem3']" :key="slot" class="bg-zinc-900 rounded-lg border border-zinc-800 p-3 flex flex-col items-center relative group">
              
              <p class="text-zinc-500 text-[10px] font-bold uppercase tracking-wider mb-2 w-full text-center border-b border-zinc-800 pb-1">Foto {{ index + 1 }}</p>
              
              <img v-if="moldeAtualizado[slot]" :src="'/imagens/moldes/' + moldeAtualizado[slot]" class="w-full h-32 sm:h-40 object-cover rounded mb-3 border border-zinc-700 shadow-sm transition-transform duration-300 group-hover:scale-105">
              
              <div v-else class="w-full h-32 sm:h-40 border border-dashed border-zinc-700 rounded mb-3 flex flex-col items-center justify-center bg-zinc-950 text-zinc-600">
                <span class="text-3xl mb-1">📸</span>
                <p class="text-[10px] font-medium">Vazio</p>
              </div>

              <input type="file" :ref="el => inputsFile[slot] = el" @change="event => fazerUpload(event, slot)" accept="image/*" class="hidden">

              <div class="w-full flex gap-2 mt-auto">
                <button @click="inputsFile[slot].click()" :disabled="carregandoSlot === slot" class="flex-1 bg-zinc-800 hover:bg-zinc-700 text-white text-[10px] font-semibold py-2 rounded transition-colors flex items-center justify-center active:scale-95">
                  <span v-if="carregandoSlot === slot" class="animate-spin h-3 w-3 border-2 border-white border-t-transparent rounded-full mr-1"></span>
                  {{ moldeAtualizado[slot] ? 'Trocar' : 'Anexar' }}
                </button>
                
                <button v-if="moldeAtualizado[slot]" @click="removerFoto(slot)" class="bg-red-950 hover:bg-red-900 text-red-300 px-2.5 rounded transition-colors text-sm active:scale-95" title="Remover">
                  🗑️
                </button>
              </div>
            </div>
          </div>
          
          <p v-if="erroUpload" class="text-red-500 text-xs text-center mt-4 p-2 bg-red-900/20 rounded border border-red-900/50">{{ erroUpload }}</p>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import axios from 'axios'

const props = defineProps({ molde: Object })
const emit = defineEmits(['fechar', 'fotoAtualizada'])

const moldeAtualizado = reactive({ ...props.molde })
const nomeEmpresa = ref('Carregando...')
const nomeCategoria = ref('Carregando...')
const machosVinculados = ref([])
const buscandoMachos = ref(true)

const carregandoSlot = ref('') 
const erroUpload = ref('')
const inputsFile = reactive({}) 

onMounted(async () => {
  const token = localStorage.getItem('token')
  const config = { headers: { Authorization: `Bearer ${token}` } }

  try {
    if (props.molde.empresaId) {
      const resEmp = await axios.get('/api/Empresa', config)
      const emp = resEmp.data.find(e => e.id === props.molde.empresaId)
      nomeEmpresa.value = emp ? emp.nome : 'Empresa não encontrada'
    } else {
      nomeEmpresa.value = 'Nenhuma vinculada'
    }

    if (props.molde.categoriaId) {
      const resCat = await axios.get('/api/Categoria', config)
      const cat = resCat.data.find(c => c.id === props.molde.categoriaId)
      nomeCategoria.value = cat ? cat.setor : 'Categoria não encontrada'
    } else {
      nomeCategoria.value = 'Nenhuma vinculada'
    }
  } catch(e) {
    nomeEmpresa.value = 'Erro ao carregar'
    nomeCategoria.value = 'Erro ao carregar'
  }

  try {
    const resMachos = await axios.get(`/api/Molde/${props.molde.id}/machos`, config)
    machosVinculados.value = resMachos.data
  } catch(e) {
    console.error("Erro ao buscar machos:", e)
  } finally {
    buscandoMachos.value = false
  }
})

const fazerUpload = async (event, slotImagem) => {
  const arquivoFisico = event.target.files[0]
  if (!arquivoFisico) return

  carregandoSlot.value = slotImagem
  erroUpload.value = ''

  try {
    const token = localStorage.getItem('token')
    const pacote = new FormData()
    pacote.append('arquivo', arquivoFisico) 

    const respostaUpload = await axios.post('/api/Upload/imagem/moldes', pacote, {
      headers: { 'Authorization': `Bearer ${token}` }
    })

    moldeAtualizado[slotImagem] = respostaUpload.data.nomeImagem

    await axios.put(`/api/Molde/${props.molde.id}`, moldeAtualizado, {
      headers: { Authorization: `Bearer ${token}` }
    })

    emit('fotoAtualizada') 
  } catch (e) {
    erroUpload.value = e.response?.data || 'Falha ao salvar a imagem na fábrica.'
    moldeAtualizado[slotImagem] = props.macho[slotImagem]
  } finally {
    carregandoSlot.value = ''
    event.target.value = '' 
  }
}

const removerFoto = async (slotImagem) => {
  if(!window.confirm(`Tem certeza que deseja apagar a foto ${slotImagem.replace('imagem', '')} deste molde?`)) return
  
  erroUpload.value = ''
  const imagemAntiga = moldeAtualizado[slotImagem]
  moldeAtualizado[slotImagem] = null 

  try {
    const token = localStorage.getItem('token')
    await axios.put(`/api/Molde/${props.molde.id}`, moldeAtualizado, {
      headers: { Authorization: `Bearer ${token}` }
    })
    emit('fotoAtualizada')
  } catch (e) {
    erroUpload.value = 'Falha ao remover a foto no servidor.'
    moldeAtualizado[slotImagem] = imagemAntiga 
  }
}
</script>

<style scoped>
.animate-fade-in { animation: fadeIn 0.15s ease-out; }
@keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }

/* Estilização da barra de rolagem moderna para não quebrar o layout */
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: #18181b; border-radius: 8px; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #3f3f46; border-radius: 8px; }
.custom-scrollbar::-webkit-scrollbar-thumb:hover { background: #f97316; }
</style>