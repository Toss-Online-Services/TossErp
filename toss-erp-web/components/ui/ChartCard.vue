<script setup lang="ts">
defineProps<{
  title: string
  subtitle?: string
}>()
</script>

<template>
  <div class="card-surface p-4 space-y-2">
    <div class="flex items-start justify-between gap-3">
      <div>
        <p class="text-sm text-muted-foreground">{{ subtitle }}</p>
        <h3 class="text-lg font-semibold">{{ title }}</h3>
      </div>
    </div>
    <div class="h-64">
      <slot />
    </div>
  </div>
</template>

