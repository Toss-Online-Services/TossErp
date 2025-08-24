<template>
  <div class="lg:ml-64">
    <main class="flex-1 p-6">
      <slot />
    </main>
  </div>
</template>
<script setup lang="ts">
import { defineOptions } from 'vue'
defineOptions({ name: 'MainContent' })
</script>
