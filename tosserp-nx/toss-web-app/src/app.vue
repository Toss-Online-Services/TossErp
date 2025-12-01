<script setup lang="ts">
import { onMounted } from 'vue'

onMounted(() => {
  // Apply dark mode by default
  document.documentElement.classList.add('dark')
})
</script>

<template>
  <NuxtLayout>
    <NuxtPage />
  </NuxtLayout>
</template>
