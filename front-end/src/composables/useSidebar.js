import { ref } from 'vue'

const isSidebarOpen = ref(true)

export function useSidebar() {
  const toggleSidebar = () => {
    isSidebarOpen.value = !isSidebarOpen.value
  }

  return {
    isSidebarOpen,
    toggleSidebar
  }
}
