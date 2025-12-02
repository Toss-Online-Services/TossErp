<script setup lang="ts">
import { onMounted } from 'vue'
import { navigateTo } from '#app'

// Redirect to dashboard immediately
onMounted(() => {
  navigateTo('/dashboard')
})
</script>

<template>
  <div class="flex items-center justify-center min-h-screen">
    <div class="text-center">
      <p class="text-lg text-muted-foreground">Redirecting to dashboard...</p>
    </div>
  </div>
</template>
