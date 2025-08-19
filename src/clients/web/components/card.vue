<template>
  <div 
    class="bg-white shadow rounded-lg border border-gray-200"
    :class="[padding, margin, customClass]"
  >
    <!-- Card Header -->
    <div v-if="$slots.header || title" class="px-6 py-4 border-b border-gray-200">
      <slot name="header">
        <div class="flex items-center justify-between">
          <div>
            <h3 v-if="title" class="text-lg font-medium text-gray-900">{{ title }}</h3>
            <p v-if="subtitle" class="text-sm text-gray-500 mt-1">{{ subtitle }}</p>
          </div>
          <div v-if="$slots.headerActions" class="flex items-center space-x-2">
            <slot name="headerActions" />
          </div>
        </div>
      </slot>
    </div>

    <!-- Card Body -->
    <div class="px-6 py-4">
      <slot />
    </div>

    <!-- Card Footer -->
    <div v-if="$slots.footer" class="px-6 py-4 border-t border-gray-200 bg-gray-50 rounded-b-lg">
      <slot name="footer" />
    </div>
  </div>
</template>

<script setup lang="ts">
interface Props {
  title?: string
  subtitle?: string
  padding?: string
  margin?: string
  customClass?: string
}

withDefaults(defineProps<Props>(), {
  padding: 'p-6',
  margin: '',
  customClass: ''
})
</script>

<style scoped>
/* Component-specific styles */
</style>
