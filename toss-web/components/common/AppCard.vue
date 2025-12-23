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
  <div class="bg-card border-0 rounded-xl" style="box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);">
    <div v-if="title || subtitle" class="px-4 py-4">
      <h3 v-if="title" class="text-base font-semibold text-foreground mb-0">{{ title }}</h3>
      <p v-if="subtitle" class="text-sm text-muted-foreground mt-0.5 mb-0">{{ subtitle }}</p>
    </div>
    <div :class="paddingClasses[padding]">
      <slot />
    </div>
  </div>
</template>


