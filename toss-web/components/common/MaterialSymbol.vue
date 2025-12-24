<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  name: string
  size?: number | string
}

const props = withDefaults(defineProps<Props>(), {
  size: 24
})

const fontSize = computed(() => {
  return typeof props.size === 'number' ? `${props.size}px` : props.size
})
</script>

<template>
  <span
    class="material-symbols-rounded"
    :style="{ fontSize }"
    aria-hidden="true"
  >{{ name }}</span>
</template>
