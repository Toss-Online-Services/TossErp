<script setup lang="ts">
import { cn } from '@/lib/utils'

defineProps<{
  class?: string
}>()
</script>

<template>
  <div :class="cn('rounded-lg border border-stone-200 bg-card text-card-foreground shadow-material-sm', $props.class)">
    <slot />
  </div>
</template>
