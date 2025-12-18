<template>
  <div
    class="rounded-xl bg-white dark:bg-gray-800 shadow-material p-6 hover:shadow-material-lg transition-all duration-200"
  >
    <div class="flex items-start justify-between">
      <div class="flex-1">
        <p class="text-sm text-gray-500 dark:text-gray-400 font-medium mb-1">
          {{ label }}
        </p>
        <p class="text-3xl font-bold text-gray-900 dark:text-white mb-2">
          {{ value }}
        </p>
        <div v-if="delta !== undefined" class="flex items-center gap-1">
          <span
            :class="[
              'material-symbols-rounded text-base',
              delta >= 0 ? 'text-green-600' : 'text-red-600'
            ]"
          >
            {{ delta >= 0 ? 'trending_up' : 'trending_down' }}
          </span>
          <span
            :class="[
              'text-sm font-semibold',
              delta >= 0 ? 'text-green-600' : 'text-red-600'
            ]"
          >
            {{ delta >= 0 ? '+' : '' }}{{ delta }}%
          </span>
          <span class="text-sm text-gray-500 dark:text-gray-400 ml-1">
            {{ deltaLabel }}
          </span>
        </div>
      </div>
      <div
        :class="[
          'flex items-center justify-center w-14 h-14 rounded-xl',
          'shadow-material',
          iconBgClass
        ]"
      >
        <span class="material-symbols-rounded text-3xl text-white">
          {{ icon }}
        </span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Props {
  label: string
  value: string | number
  delta?: number
  deltaLabel?: string
  icon: string
  iconBgClass?: string
}

withDefaults(defineProps<Props>(), {
  deltaLabel: 'than last week',
  iconBgClass: 'bg-gradient-primary'
})
</script>
