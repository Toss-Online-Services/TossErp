<template>
  <div
    :class="[
      'rounded-xl bg-white dark:bg-gray-800 shadow-material p-6',
      'transition-all duration-200 hover:shadow-material-lg',
      class
    ]"
  >
    <slot />
  </div>
</template>

<script setup lang="ts">
interface Props {
  class?: string
}

defineProps<Props>()
</script>
