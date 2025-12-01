<script setup lang="ts">
import { computed } from 'vue'
import { X, CheckCircle2, AlertCircle, AlertTriangle, Info } from 'lucide-vue-next'
import { useToast, type Toast } from '@/composables/useToast'

const props = defineProps<{
  toast: Toast
}>()

const { removeToast } = useToast()

const icon = computed(() => {
  switch (props.toast.type) {
    case 'success':
      return CheckCircle2
    case 'error':
      return AlertCircle
    case 'warning':
      return AlertTriangle
    default:
      return Info
  }
})

const bgColor = computed(() => {
  switch (props.toast.type) {
    case 'success':
      return 'bg-emerald-50 border-emerald-200 text-emerald-900'
    case 'error':
      return 'bg-red-50 border-red-200 text-red-900'
    case 'warning':
      return 'bg-amber-50 border-amber-200 text-amber-900'
    default:
      return 'bg-blue-50 border-blue-200 text-blue-900'
  }
})
</script>

<template>
  <div
    :class="[
      'flex items-start gap-3 p-4 border rounded-lg shadow-lg min-w-[300px] max-w-md',
      bgColor
    ]"
  >
    <component :is="icon" :size="20" class="flex-shrink-0 mt-0.5" />
    <div class="flex-1 text-sm font-medium">{{ toast.message }}</div>
    <button
      @click="removeToast(toast.id)"
      class="flex-shrink-0 hover:opacity-70 transition-opacity"
    >
      <X :size="16" />
    </button>
  </div>
</template>

