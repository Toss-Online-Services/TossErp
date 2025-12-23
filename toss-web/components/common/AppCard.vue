<script setup lang="ts">
interface Props {
  title?: string
  subtitle?: string
  padding?: 'none' | 'sm' | 'md' | 'lg'
}

const props = withDefaults(defineProps<Props>(), {
  padding: 'md'
})

const paddingClasses = {
  none: '',
  sm: 'p-3',
  md: 'p-4',
  lg: 'p-6'
}
</script>

<template>
  <div class="bg-card border border-border rounded-lg shadow-sm">
    <div v-if="title || subtitle" class="border-b border-border px-4 py-3">
      <h3 v-if="title" class="text-base font-semibold text-foreground">{{ title }}</h3>
      <p v-if="subtitle" class="text-sm text-muted-foreground mt-1">{{ subtitle }}</p>
    </div>
    <div :class="paddingClasses[padding]">
      <slot />
    </div>
  </div>
</template>


