<template>
  <div class="fixed inset-0 bg-black/80 flex items-center justify-center z-50 animate-fade-in p-4 sm:p-6">
    <div class="bg-zinc-900 border border-zinc-700 rounded-xl w-full max-w-lg shadow-2xl overflow-hidden max-h-[95vh] flex flex-col">
      
      <div class="p-5 sm:p-6 border-b border-zinc-800 shrink-0">
        <h2 class="text-xl sm:text-2xl font-extrabold text-white flex items-center gap-2 capitalize">
          Editar Molde: {{ props.molde.nome }}
        </h2>
      </div>

      <div class="p-5 sm:p-6 overflow-y-auto flex-1 custom-scrollbar">
        <form @submit.prevent="salvarEdicao" class="flex flex-col gap-5 relative">
          
          <div>
            <label class="block text-zinc-400 font-medium mb-1 text-sm">Nome da Peça</label>
            <input type="text" v-model="moldeAtualizado.nome" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2.5 focus:outline-none focus:border-orange-500 transition-colors" required>
          </div>

          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-zinc-400 font-medium mb-1 text-sm">Código</label>
              <input type="text" v-model="moldeAtualizado.codigo" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2.5 focus:outline-none focus:border-orange-500 transition-colors" required disabled>
            </div>
            <div>
              <label class="block text-zinc-400 font-medium mb-1 text-sm">Prateleira</label>
              <input type="text" v-model="moldeAtualizado.prateleira" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2.5 focus:outline-none focus:border-orange-500 transition-colors">
            </div>
          </div>

          <div>
            <label class="block text-zinc-400 font-medium mb-1 text-sm">Status Atual</label>
            <select v-model="moldeAtualizado.status" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2.5 focus:outline-none focus:border-orange-500 transition-colors appearance-none cursor-pointer">
              <option v-for="opcao in statusOpcoes" :key="opcao" :value="opcao">{{ opcao }}</option>
            </select>
          </div>

          <div class="grid grid-cols-2 gap-4 relative z-40">
            <div class="relative">
              <label class="block text-zinc-400 font-medium mb-1 text-sm">Empresa</label>
              <div class="relative">
                <input 
                  type="text" 
                  v-model="termoBuscaEmpresa"
                  @focus="dropdownEmpresaAberto = true"
                  placeholder="Pesquise..."
                  class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2.5 focus:outline-none focus:border-orange-500 transition-colors placeholder-zinc-600"
                >
                <button v-if="moldeAtualizado.empresaId" @click.prevent="limparSelecaoEmpresa" class="absolute right-3 top-1/2 -translate-y-1/2 text-zinc-500 hover:text-red-500 p-1">
                  ✕
                </button>
                <span v-else class="absolute right-3 top-1/2 -translate-y-1/2 text-zinc-500 pointer-events-none">▼</span>
              </div>

              <div v-if="dropdownEmpresaAberto" @click="dropdownEmpresaAberto = false" class="fixed inset-0 z-30"></div>

              <div v-if="dropdownEmpresaAberto" class="absolute z-50 w-full mt-1 bg-zinc-950 border border-zinc-700 rounded-lg shadow-2xl max-h-48 overflow-y-auto custom-scrollbar">
                <ul class="py-1">
                  <li 
                    v-for="empresa in empresasFiltradas" 
                    :key="empresa.id"
                    @click="selecionarEmpresa(empresa)"
                    class="px-4 py-2 hover:bg-orange-500/20 hover:text-orange-400 cursor-pointer text-sm border-b border-zinc-800/50 last:border-0 transition-colors truncate"
                    :class="moldeAtualizado.empresaId === empresa.id ? 'bg-orange-500/10 text-orange-500 font-bold' : 'text-zinc-300'"
                  >
                    {{ empresa.nome }}
                  </li>
                  <li v-if="empresasFiltradas.length === 0" class="p-3 text-zinc-500 text-sm text-center">Nenhuma encontrada.</li>
                </ul>
              </div>
            </div>

            <div>
              <label class="block text-zinc-400 font-medium mb-1 text-sm">Categoria</label>
              <select v-model="moldeAtualizado.categoriaId" class="w-full bg-zinc-950 text-white border border-zinc-800 rounded-lg px-4 py-2.5 focus:outline-none focus:border-orange-500 transition-colors appearance-none cursor-pointer">
                <option :value="null">Nenhuma...</option>
                <option v-for="categoria in categorias" :key="categoria.id" :value="categoria.id">{{ categoria.setor }}</option>
              </select>
            </div>
          </div>

          <div class="border-t border-zinc-800 pt-5 mt-3 relative z-10">
            <label class="block text-zinc-400 font-medium mb-3 text-sm">Gerenciar Imagens</label>
            <div class="grid grid-cols-3 gap-3">
              <div v-for="(slot, index) in ['imagem1', 'imagem2', 'imagem3']" :key="slot" class="bg-zinc-950 rounded-lg border border-zinc-800 p-2 flex flex-col items-center relative h-32">
                <p class="text-zinc-600 text-[10px] font-bold uppercase tracking-wider mb-1 w-full text-center">Foto {{ index + 1 }}</p>

                <img v-if="previews[slot]" :src="previews[slot]" class="w-full h-16 object-cover rounded border border-zinc-700 mb-2 transition-opacity duration-300">
                <img v-else-if="moldeAtualizado[slot]" :src="'/imagens/moldes/' + moldeAtualizado[slot]" class="w-full h-16 object-cover rounded border border-zinc-700 mb-2">
                <div v-else class="w-full h-16 border border-dashed border-zinc-700 rounded mb-2 flex flex-col items-center justify-center bg-zinc-900 text-zinc-600">
                  <span class="text-xl mb-1">📸</span>
                </div>

                <input type="file" :ref="el => inputsFile[slot] = el" @change="e => selecionarFoto(e, slot)" accept="image/*" class="hidden">

                <div class="w-full flex gap-1.5 mt-auto relative z-10">
                  <button type="button" @click="inputsFile[slot].click()" class="flex-1 bg-zinc-800 hover:bg-zinc-700 text-white text-[10px] font-semibold py-1.5 rounded transition-colors active:scale-95">
                    {{ previews[slot] || moldeAtualizado[slot] ? 'Trocar' : 'Anexar' }}
                  </button>
                  <button v-if="previews[slot] || moldeAtualizado[slot]" type="button" @click="removerFoto(slot)" class="bg-red-950/50 hover:bg-red-950 text-red-300 px-2 rounded transition-colors active:scale-95">
                    🗑️
                  </button>
                </div>
              </div>
            </div>
          </div>

          <p v-if="erro" class="text-red-500 text-sm mt-2 font-medium relative z-10">{{ erro }}</p>

          <div class="flex justify-end gap-3 mt-4 pt-4 border-t border-zinc-800 shrink-0 bg-zinc-900 relative z-10">
            <button type="button" @click="$emit('fechar')" class="px-5 py-2.5 rounded-lg text-zinc-400 hover:text-white hover:bg-zinc-800 font-medium transition-colors active:scale-95">
              Cancelar
            </button>
            <button type="submit" :disabled="carregando" class="bg-blue-600 hover:bg-blue-700 active:scale-95 text-white font-bold px-6 py-2.5 rounded-lg transition-all shadow-[0_0_10px_rgba(37,99,235,0.3)] disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center min-w-[140px]">
              <span v-if="carregando" class="animate-spin h-5 w-5 border-2 border-white border-t-transparent rounded-full"></span>
              <span v-else>Salvar Alterações</span>
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import axios from 'axios'

const props = defineProps({
  molde: {
    type: Object,
    required: true
  }
})

const emit = defineEmits(['fechar', 'moldeEditado'])

const statusOpcoes = ['Ativo', 'Desativado']
const moldeAtualizado = ref({ ...props.molde })

const empresas = ref([])
const categorias = ref([])
const erro = ref('')
const carregando = ref(false)

const termoBuscaEmpresa = ref('')
const dropdownEmpresaAberto = ref(false)

const empresasFiltradas = computed(() => {
  if (moldeAtualizado.value.empresaId || !termoBuscaEmpresa.value) return empresas.value
  const termo = termoBuscaEmpresa.value.toLowerCase()
  return empresas.value.filter(emp => emp.nome && emp.nome.toLowerCase().includes(termo))
})

const selecionarEmpresa = (empresa) => {
  moldeAtualizado.value.empresaId = empresa.id
  termoBuscaEmpresa.value = empresa.nome
  dropdownEmpresaAberto.value = false
}

const limparSelecaoEmpresa = () => {
  moldeAtualizado.value.empresaId = null
  termoBuscaEmpresa.value = ''
  dropdownEmpresaAberto.value = true
}

const arquivosSelecionados = ref({ imagem1: null, imagem2: null, imagem3: null })
const previews = ref({ imagem1: null, imagem2: null, imagem3: null })
const inputsFile = ref({})

const selecionarFoto = (event, slot) => {
  const file = event.target.files[0]
  if (!file) return
  arquivosSelecionados.value[slot] = file
  previews.value[slot] = URL.createObjectURL(file)
}

const removerFoto = (slot) => {
  if(!window.confirm("Tem certeza que deseja remover esta foto?")) return
  arquivosSelecionados.value[slot] = null
  previews.value[slot] = null
  moldeAtualizado.value[slot] = null
}

onMounted(async () => {
  const token = localStorage.getItem('token')
  const config = { headers: { Authorization: `Bearer ${token}` } }
  try {
    const [resEmp, resCat] = await Promise.all([
      axios.get('/api/Empresa', config),
      axios.get('/api/Categoria', config)
    ])
    empresas.value = resEmp.data
    categorias.value = resCat.data

    if (moldeAtualizado.value.empresaId) {
      const empAtual = empresas.value.find(e => e.id === moldeAtualizado.value.empresaId)
      if (empAtual) {
        termoBuscaEmpresa.value = empAtual.nome
      }
    }

  } catch (e) {
    erro.value = 'Falha ao carregar as opções de empresas e categorias.'
  }
})

const salvarEdicao = async () => {
  erro.value = ''
  carregando.value = true

  const token = localStorage.getItem('token')
  const config = { headers: { Authorization: `Bearer ${token}` } }

  try {
    for (const slot of ['imagem1', 'imagem2', 'imagem3']) {
      if (arquivosSelecionados.value[slot]) {
        const pacote = new FormData()
        pacote.append('arquivo', arquivosSelecionados.value[slot])
        const respostaUpload = await axios.post('/api/Upload/imagem/moldes', pacote, config)
        moldeAtualizado.value[slot] = respostaUpload.data.nomeImagem
      }
    }

    await axios.put(`/api/Molde/${props.molde.id}`, moldeAtualizado.value, config)
    
    emit('moldeEditado')
  } catch (e) {
    erro.value = e.response?.data || 'Erro ao salvar as alterações do molde.'
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