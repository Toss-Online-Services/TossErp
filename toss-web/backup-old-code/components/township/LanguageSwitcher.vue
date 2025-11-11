<template>
  <div class="relative">
    <button
      @click="isOpen = !isOpen"
      class="flex items-center gap-2 px-4 py-3 bg-white dark:bg-gray-800 rounded-lg border-2 border-gray-200 dark:border-gray-700 hover:border-blue-500 transition-colors touch-manipulation min-h-[48px] min-w-[48px]"
      :aria-label="$t('language.switchLanguage')"
    >
      <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5h12M9 3v2m1.048 9.5A18.022 18.022 0 016.412 9m6.088 9h7M11 21l5-10 5 10M12.751 5C11.783 10.77 8.07 15.61 3 18.129" />
      </svg>
      <span class="font-medium text-base">{{ currentLanguage.name }}</span>
      <svg class="w-4 h-4 transition-transform" :class="{ 'rotate-180': isOpen }" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
      </svg>
    </button>
    
    <!-- Language Options Dropdown -->
    <transition
      enter-active-class="transition ease-out duration-200"
      enter-from-class="opacity-0 translate-y-1"
      enter-to-class="opacity-100 translate-y-0"
      leave-active-class="transition ease-in duration-150"
      leave-from-class="opacity-100 translate-y-0"
      leave-to-class="opacity-0 translate-y-1"
    >
      <div
        v-if="isOpen"
        class="absolute right-0 mt-2 w-56 bg-white dark:bg-gray-800 rounded-xl shadow-xl border border-gray-200 dark:border-gray-700 py-2 z-50"
      >
        <button
          v-for="lang in languages"
          :key="lang.code"
          @click="selectLanguage(lang)"
          class="w-full text-left px-4 py-3 hover:bg-gray-100 dark:hover:bg-gray-700 flex items-center gap-3 transition-colors touch-manipulation min-h-[48px]"
          :class="{ 'bg-blue-50 dark:bg-blue-900': currentLanguage.code === lang.code }"
        >
          <span class="text-2xl">{{ lang.flag }}</span>
          <div class="flex-1">
            <div class="font-medium text-base">{{ lang.name }}</div>
            <div class="text-sm text-gray-600 dark:text-gray-400">{{ lang.nativeName }}</div>
          </div>
          <svg v-if="currentLanguage.code === lang.code" class="w-5 h-5 text-blue-600" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd" />
          </svg>
        </button>
      </div>
    </transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useI18n } from 'vue-i18n'

const { locale, t } = useI18n()

const isOpen = ref(false)

const languages = [
  { code: 'en', name: 'English', nativeName: 'English', flag: '🇬🇧' },
  { code: 'zu', name: 'isiZulu', nativeName: 'isiZulu', flag: '🇿🇦' },
  { code: 'xh', name: 'isiXhosa', nativeName: 'isiXhosa', flag: '🇿🇦' },
  { code: 'st', name: 'Sesotho', nativeName: 'Sesotho', flag: '🇿🇦' },
  { code: 'tn', name: 'Setswana', nativeName: 'Setswana', flag: '🇿🇦' },
  { code: 'af', name: 'Afrikaans', nativeName: 'Afrikaans', flag: '🇿🇦' }
]

const currentLanguage = computed(() => {
  return languages.find(lang => lang.code === locale.value) || languages[0]
})

const selectLanguage = (lang: typeof languages[0]) => {
  locale.value = lang.code
  isOpen.value = false
  localStorage.setItem('toss-language', lang.code)
}

// Click outside to close
const handleClickOutside = (event: MouseEvent) => {
  const target = event.target as HTMLElement
  if (isOpen.value && !target.closest('.relative')) {
    isOpen.value = false
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
  // Load saved language preference
  const savedLang = localStorage.getItem('toss-language')
  if (savedLang) {
    locale.value = savedLang
  }
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>

