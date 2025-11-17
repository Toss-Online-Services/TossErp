<template>
  <svg
    :width="size"
    :height="size"
    :viewBox="viewBox"
    :class="classes"
    v-bind="$attrs"
  >
    <slot />
  </svg>
</template>

<script setup lang="ts">
interface Props {
  size?: number | string
  viewBox?: string
  classes?: string
}

withDefaults(defineProps<Props>(), {
  size: 24,
  viewBox: '0 0 24 24',
  classes: 'fill-current'
})
</script>
