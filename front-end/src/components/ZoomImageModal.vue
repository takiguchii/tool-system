<template>
  <div
    v-if="visivel"
    class="fixed inset-0 bg-black/90 flex items-center justify-center z-[100] animate-fade-in"
    @click.self="$emit('fechar')"
  >
    <button @click="$emit('fechar')" class="absolute top-6 right-6 text-white text-4xl font-bold hover:text-orange-500 transition-colors">
      &times;
    </button>

    <div class="max-w-[90vw] max-h-[90vh] flex items-center justify-center">
      <img :src="urlImagem" class="max-w-full max-h-full object-contain rounded-lg shadow-2xl"/>
    </div>

  </div>
</template>

<script setup>
defineProps({
  visivel: Boolean,
  urlImagem: String
})

defineEmits(['fechar'])
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
