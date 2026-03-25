<template>
  <div class="fixed inset-0 bg-black/80 flex items-center justify-center z-50 animate-fade-in p-4 sm:p-6">
    <div class="bg-zinc-900 border border-zinc-700 rounded-xl w-full max-w-4xl shadow-2xl relative flex flex-col max-h-[90vh] overflow-hidden">
      
      <div class="p-5 sm:p-6 border-b border-zinc-800 flex justify-between items-start shrink-0 bg-zinc-900">
        <div>
          <p class="text-zinc-500 text-xs font-medium uppercase tracking-wider mb-1">Visualização da Ferramenta</p>
          <h2 class="text-2xl sm:text-3xl font-extrabold text-white">Macho: {{ macho.codigo }}</h2>
          <p class="text-zinc-400 text-sm mt-1">ID Interno: <span class="font-mono text-orange-400">{{ macho.id }}</span></p>
        </div>
        
        <button @click="$emit('fechar')" class="text-zinc-500 bg-zinc-800/50 hover:bg-red-500 hover:text-white p-2 rounded-lg transition-colors active:scale-95 w-10 h-10 flex items-center justify-center font-bold text-xl">
          <strong class="text-red-400">X</strong>
        </button>
      </div>

      <div class="p-5 sm:p-6 overflow-y-auto flex-1 custom-scrollbar bg-zinc-900/50">
        <h3 class="text-lg font-bold text-orange-500 mb-4">Galeria de Fotos do Macho</h3>
        
        <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
          <div v-for="(slot, index) in ['imagem1', 'imagem2', 'imagem3']" :key="slot" class="bg-zinc-900 rounded-lg border border-zinc-800 p-3 flex flex-col items-center group">
            
            <p class="text-zinc-500 text-[10px] font-bold uppercase tracking-wider mb-2 w-full text-center border-b border-zinc-800 pb-1">Ângulo {{ index + 1 }}</p>
            
            <img v-if="macho[slot]" :src="'/imagens/machos/' + macho[slot]" class="w-full h-40 sm:h-48 object-cover rounded border border-zinc-700 shadow-sm transition-transform duration-300 group-hover:scale-105">
            
            <div v-else class="w-full h-40 sm:h-48 border border-dashed border-zinc-700 rounded flex flex-col items-center justify-center bg-zinc-950 text-zinc-600">
              <span class="text-4xl mb-2">📸</span>
              <p class="text-xs font-medium">Sem imagem cadastrada</p>
            </div>
          </div>
        </div>
      </div>

    </div>
  </div>
</template>

<script setup>
defineProps({ macho: Object })
defineEmits(['fechar'])
</script>

<style scoped>
.animate-fade-in { animation: fadeIn 0.15s ease-out; }
@keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: #18181b; border-radius: 8px; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #3f3f46; border-radius: 8px; }
.custom-scrollbar::-webkit-scrollbar-thumb:hover { background: #f97316; }
</style>