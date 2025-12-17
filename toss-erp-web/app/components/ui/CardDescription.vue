<script setup lang="ts">
import { cn } from '~/lib/utils'

defineProps<{
  class?: string
}>()
</script>

<template>
  <p :class="cn('text-xs text-stone-500 dark:text-stone-400', $props.class)">
    <slot />
  </p>
</template>


