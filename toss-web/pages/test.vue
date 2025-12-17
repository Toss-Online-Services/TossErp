<template>
  <div class="p-8">
    <h1 class="text-4xl font-bold text-blue-600">Test Page Works!</h1>
    <p class="mt-4 text-lg">If you can see this, Nuxt routing is working correctly.</p>
    <p class="mt-2">Current time: {{ new Date().toLocaleString() }}</p>
  </div>
</template>

<script setup>
console.log('Test page loaded successfully!')
</script>
