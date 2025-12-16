<script setup lang="ts">
const props = defineProps<{
  label: string
  value: string
  delta?: string
  icon?: string
}>()

const deltaClass = computed(() => {
  if (!props.delta) return ''
  if (props.delta.startsWith('+')) return 'text-success'
  if (props.delta.startsWith('-')) return 'text-destructive'
  return 'text-muted-foreground'
})
</script>

<template>
  <div class="card-surface p-4 flex justify-between items-start gap-3">
    <div class="space-y-1">
      <p class="text-sm text-muted-foreground">{{ label }}</p>
      <p class="text-2xl font-semibold">{{ value }}</p>
      <p v-if="delta" class="text-xs font-medium" :class="deltaClass">{{ delta }}</p>
    </div>
    <div
      v-if="icon"
      class="h-11 w-11 rounded-xl bg-primary/10 text-primary flex items-center justify-center"
    >
      <span class="material-symbols-rounded text-xl">{{ icon }}</span>
    </div>
  </div>
</template>

