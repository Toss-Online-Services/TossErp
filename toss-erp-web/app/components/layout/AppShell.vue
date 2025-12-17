<script setup lang="ts">
import { ref, onMounted, onUnmounted, provide } from 'vue'

const sidebarCollapsed = ref(false)
const isMobile = ref(false)

const checkMobile = () => {
  isMobile.value = window.innerWidth < 1024
}

const toggleCollapse = () => {
  sidebarCollapsed.value = !sidebarCollapsed.value
}

onMounted(() => {
  checkMobile()
  window.addEventListener('resize', checkMobile)
})

onUnmounted(() => {
  window.removeEventListener('resize', checkMobile)
})

provide('sidebarCollapsed', sidebarCollapsed)
provide('toggleCollapse', toggleCollapse)
provide('isMobile', isMobile)

defineExpose({ sidebarCollapsed, toggleCollapse, isMobile })
</script>

<template>
  <div class="min-h-screen bg-background text-foreground flex">
    <!-- Desktop sidebar -->
    <aside
      :class="[
        'hidden lg:flex flex-col shrink-0 border-r border-stone-200 dark:border-stone-700 bg-white dark:bg-stone-900 transition-all duration-300 ease-in-out h-screen sticky top-0',
        sidebarCollapsed ? 'w-16' : 'w-60'
      ]"
    >
      <slot name="sidebar" :collapsed="sidebarCollapsed" :toggle="toggleCollapse" />
    </aside>

    <div class="flex-1 flex min-h-screen flex-col">
      <header class="border-b border-stone-200 dark:border-stone-700 bg-white/80 dark:bg-stone-900/80 backdrop-blur-sm supports-[backdrop-filter]:bg-white/60 dark:supports-[backdrop-filter]:bg-stone-900/60 sticky top-0 z-20">
        <slot name="topbar" :collapsed="sidebarCollapsed" :toggle="toggleCollapse" />
      </header>
      <main class="flex-1 p-4 md:p-6 lg:p-8">
        <slot />
      </main>
      <slot name="footer" />
    </div>
  </div>
</template>
