<script setup lang="ts">
defineProps<{
  title: string
  subtitle?: string
}>()
</script>

<template>
  <div class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700 rounded-lg">
    <div class="flex flex-col space-y-1.5 p-6">
      <h3 class="text-sm font-semibold text-stone-900 dark:text-white">{{ title }}</h3>
      <p v-if="subtitle" class="text-xs text-stone-500 dark:text-stone-400">{{ subtitle }}</p>
    </div>
    <div class="p-6 pt-0">
      <div class="h-[300px]">
        <slot />
      </div>
    </div>
  </div>
</template>
