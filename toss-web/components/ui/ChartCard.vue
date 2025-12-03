<script setup lang="ts">
interface Props {
  title: string
  subtitle?: string
  footerText?: string
  footerIcon?: string
}

defineProps<Props>()
</script>

<template>
  <div class="bg-white rounded-xl shadow-sm hover:shadow-md transition-shadow duration-200">
    <div class="p-6">
      <h6 class="text-base font-semibold text-gray-900 mb-0">{{ title }}</h6>
      <p v-if="subtitle" class="text-sm text-gray-600 mt-1">{{ subtitle }}</p>
      <div class="mt-4">
        <slot />
      </div>
    </div>
    <hr class="border-gray-200 my-0">
    <div v-if="footerText" class="px-6 py-3 flex items-center">
      <span v-if="footerIcon" class="material-symbols-rounded text-sm text-gray-600 mr-2">{{ footerIcon }}</span>
      <p class="text-sm text-gray-600 mb-0">{{ footerText }}</p>
    </div>
  </div>
</template>

