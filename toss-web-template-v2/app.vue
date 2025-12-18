<template>
  <NuxtLayout>
    <NuxtPage />
  </NuxtLayout>
</template>

<script setup>
// Global app configuration
useHead({
  bodyAttrs: {
    class: 'g-sidenav-show bg-gray-100'
  }
})
</script>
