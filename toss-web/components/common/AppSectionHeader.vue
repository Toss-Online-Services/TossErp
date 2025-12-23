<script setup lang="ts">
interface Props {
  title: string
  description?: string
  actionLabel?: string
}

const props = defineProps<Props>()

const emit = defineEmits<{
  action: []
}>()
</script>

<template>
  <div class="flex items-center justify-between mb-6">
    <div>
      <h2 class="text-xl font-semibold text-foreground">{{ title }}</h2>
      <p v-if="description" class="text-sm text-muted-foreground mt-1">{{ description }}</p>
    </div>
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


