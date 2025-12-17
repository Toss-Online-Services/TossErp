import { defineStore } from 'pinia'
import { useColorMode } from '@vueuse/core'

export const useThemeStore = defineStore('theme', () => {
  const mode = useColorMode({
    attribute: 'class',
    emitAuto: true,
    modes: {
      light: 'light',
      dark: 'dark'
    }
  })

  const toggle = () => {
    mode.value = mode.value === 'dark' ? 'light' : 'dark'
  }

  const isDark = computed(() => mode.value === 'dark')

  return { mode, isDark, toggle }
})

