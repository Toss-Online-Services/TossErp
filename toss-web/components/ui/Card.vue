<script setup lang="ts">
interface CardProps {
  title?: string
  subtitle?: string
  gradient?: boolean
  class?: string
}

const props = defineProps<CardProps>()
</script>

<template>
  <div :class="['bg-[hsl(var(--ct-surface))] text-[hsl(var(--ct-on-surface))] rounded-[var(--ct-radius-xl)] shadow-elevation-1 p-6 transition-all hover:shadow-elevation-3', props.class]">
    <div v-if="title || subtitle" class="mb-4">
      <h3 v-if="title" class="text-lg font-semibold">
        {{ title }}
      </h3>
      <p v-if="subtitle" class="text-sm text-[hsl(var(--ct-secondary))] mt-1">
        {{ subtitle }}
      </p>
    </div>
    <div class="card-content">
      <slot />
    </div>
  </div>
</template>

