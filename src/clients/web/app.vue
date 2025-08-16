<template>
  <div id="app" class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>
  </div>
</template>

<script setup lang="ts">
// App-level setup
useHead({
  title: 'TOSS ERP - Stock Management',
  meta: [
    { name: 'description', content: 'Enterprise Stock Management System' },
    { name: 'theme-color', content: '#2563eb' }
  ]
})

// Initialize color mode
const colorMode = useColorMode()
colorMode.preference = 'system'


</script>

<style>
/* Global styles */
html {
  scroll-behavior: smooth;
}

body {
  font-feature-settings: 'cv02', 'cv03', 'cv04', 'cv11';
}

/* Custom scrollbar */
::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}

::-webkit-scrollbar-track {
  background: transparent;
}

::-webkit-scrollbar-thumb {
  background: #cbd5e1;
  border-radius: 3px;
}

::-webkit-scrollbar-thumb:hover {
  background: #94a3b8;
}

.dark ::-webkit-scrollbar-thumb {
  background: #475569;
}

.dark ::-webkit-scrollbar-thumb:hover {
  background: #64748b;
}
</style>
