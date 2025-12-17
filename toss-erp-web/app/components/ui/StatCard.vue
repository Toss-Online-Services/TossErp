<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  label: string
  value: string
  delta?: string
  description?: string
  icon?: string
}>()

const deltaClass = computed(() => {
  if (!props.delta) return ''
  if (props.delta.startsWith('+')) return 'text-green-600 dark:text-green-400 bg-green-50 dark:bg-green-900/20'
  if (props.delta.startsWith('-')) return 'text-red-600 dark:text-red-400 bg-red-50 dark:bg-red-900/20'
  return 'text-stone-500 dark:text-stone-400 bg-stone-50 dark:bg-stone-900/20'
})
</script>

<template>
  <div class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700 rounded-lg p-6 flex flex-col h-full">
    <!-- Header -->
    <div class="mb-2">
      <h3 class="text-sm font-semibold text-stone-900 dark:text-white mb-1">
        {{ label }}
      </h3>
      <p v-if="description" class="text-xs text-stone-500 dark:text-stone-400">
        {{ description }}
      </p>
    </div>
    
    <!-- Value -->
    <div class="flex-1 flex items-center mb-4">
      <p class="text-2xl font-bold text-stone-900 dark:text-white">{{ value }}</p>
    </div>

    <!-- Delta badge -->
    <div class="flex items-center text-xs">
      <span v-if="delta" class="px-2 py-1 rounded-full font-normal" :class="deltaClass">
        {{ delta }}
      </span>
      <div v-else class="flex items-center">
        <div class="w-2 h-2 bg-green-500 dark:bg-green-400 rounded-full mr-2" />
        <span class="text-stone-500 dark:text-stone-400">Updated now</span>
      </div>
    </div>
  </div>
</template>
