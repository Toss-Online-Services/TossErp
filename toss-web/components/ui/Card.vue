<script setup lang="ts">
interface CardProps {
  title?: string
  subtitle?: string
  gradient?: boolean
  class?: string
}

const props = defineProps<CardProps>()
</script>

<template>
  <div :class="['bg-white dark:bg-gray-800 rounded-lg shadow-md p-6 transition-all hover:shadow-lg', props.class]">
    <div v-if="title || subtitle" class="mb-4">
      <h3 v-if="title" class="text-lg font-semibold text-gray-900 dark:text-white">
        {{ title }}
      </h3>
      <p v-if="subtitle" class="text-sm text-gray-600 dark:text-gray-400 mt-1">
        {{ subtitle }}
      </p>
    </div>
    <div class="card-content">
      <slot />
    </div>
  </div>
</template>

