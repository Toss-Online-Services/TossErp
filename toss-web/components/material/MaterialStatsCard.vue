<script setup lang="ts">
import { computed } from 'vue'
import MaterialCard from './MaterialCard.vue'

interface Props {
  title: string
  value: string | number
  change?: string
  changeType?: 'positive' | 'negative' | 'neutral'
  subtitle?: string
  icon?: string
}

const props = withDefaults(defineProps<Props>(), {
  changeType: 'neutral'
})

const changeColorClass = computed(() => {
  switch (props.changeType) {
    case 'positive':
      return 'text-green-600 dark:text-green-400'
    case 'negative':
      return 'text-red-600 dark:text-red-400'
    default:
      return 'text-gray-600 dark:text-gray-400'
  }
})

const iconComponent = computed(() => {
  const icons: Record<string, any> = {
    weekend: '📅',
    leaderboard: '👥',
    store: '🏪',
    person_add: '➕'
  }
  return icons[props.icon || ''] || '📊'
})
</script>

<template>
  <MaterialCard variant="elevated" class="p-6 hover:shadow-xl transition-all duration-300">
    <div class="flex items-center justify-between">
      <div class="flex-1">
        <p class="text-sm font-medium text-gray-600 dark:text-gray-400 mb-1">
          {{ title }}
        </p>
        <p class="text-2xl font-bold text-gray-900 dark:text-white mb-1">
          {{ typeof value === 'number' ? value.toLocaleString() : value }}
        </p>
        <div v-if="change" class="flex items-center text-sm">
          <span :class="['font-semibold', changeColorClass]">
            {{ change }}
          </span>
          <span v-if="subtitle" class="ml-1 text-gray-600 dark:text-gray-400">
            {{ subtitle }}
          </span>
        </div>
      </div>
      
      <div class="p-3 bg-gray-100 dark:bg-gray-700 rounded-lg">
        <span class="text-2xl">{{ iconComponent }}</span>
      </div>
    </div>
  </MaterialCard>
</template>
