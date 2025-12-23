<script setup lang="ts">
interface Props {
  label: string
  value: string | number
  size?: 'sm' | 'md' | 'lg'
}

const props = withDefaults(defineProps<Props>(), {
  size: 'md'
})

const sizeClasses = {
  sm: 'text-lg',
  md: 'text-2xl',
  lg: 'text-3xl'
}
</script>

<template>
  <div class="flex flex-col">
    <span class="text-xs text-muted-foreground mb-1">{{ label }}</span>
    <span :class="['font-semibold text-foreground', sizeClasses[size]]">{{ value }}</span>
  </div>
</template>


