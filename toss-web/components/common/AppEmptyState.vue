<script setup lang="ts">
import type { Component } from 'vue'

interface Props {
  title: string
  description?: string
  icon?: Component
  actionLabel?: string
}

const emit = defineEmits<{
  action: []
}>()
</script>

<template>
  <div class="flex flex-col items-center justify-center py-12 px-4 text-center">
    <div v-if="icon" class="mb-4 p-3 rounded-full bg-muted">
      <component :is="icon" :size="48" class="text-muted-foreground" />
    </div>
    <h3 class="text-lg font-semibold text-foreground mb-2">{{ title }}</h3>
    <p v-if="description" class="text-sm text-muted-foreground max-w-md mb-6">
      {{ description }}
    </p>
    <slot name="action">
      <button
        v-if="actionLabel"
        @click="emit('action')"
        class="px-4 py-2 bg-primary text-primary-foreground rounded-lg text-sm font-medium hover:opacity-90 transition-opacity"
      >
        {{ actionLabel }}
      </button>
    </slot>
  </div>
</template>


