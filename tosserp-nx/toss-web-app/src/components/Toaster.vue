<script setup lang="ts">
import { useToast } from '@/composables/useToast'
import Toast from '@/components/ui/Toast.vue'

const { toasts } = useToast()
</script>

<template>
  <div
    class="fixed top-4 right-4 z-50 flex flex-col gap-2 pointer-events-none"
  >
    <Toast
      v-for="toast in toasts"
      :key="toast.id"
      :toast="toast"
      class="pointer-events-auto"
    />
  </div>
</template>

