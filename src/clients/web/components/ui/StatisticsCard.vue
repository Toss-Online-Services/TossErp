<template>
  <Card
    class="statistics-card"
    :variant="variant"
    :hover="hover"
    :clickable="clickable"
    @click="$emit('click', $event)"
  >
    <template #header>
      <div class="flex items-center justify-between">
        <div
          class="flex h-12 w-12 items-center justify-center rounded-lg bg-gradient-to-r shadow-sm"
          :class="iconClasses"
        >
          <component :is="icon" class="h-6 w-6 text-white" />
        </div>
        <slot name="headerActions" />
      </div>
    </template>

    <template #default>
      <div class="mt-4">
        <div class="flex items-baseline justify-between">
          <div class="flex-1">
            <p class="text-sm font-medium text-gray-600 mb-1">
              {{ title }}
            </p>
            <p class="text-2xl font-bold text-gray-900">
              {{ formattedValue }}
            </p>
          </div>
          <div v-if="trend" class="flex items-center ml-4">
            <component
              :is="trendIcon"
              class="h-4 w-4 mr-1"
              :class="trendClasses"
            />
            <span class="text-sm font-medium" :class="trendClasses">
              {{ trend }}
            </span>
          </div>
        </div>

        <div v-if="subtitle" class="mt-2">
          <p class="text-sm text-gray-500">
            {{ subtitle }}
          </p>
        </div>

        <div v-if="progress !== undefined" class="mt-4">
          <div class="flex items-center justify-between text-sm mb-1">
            <span class="text-gray-600">Progress</span>
            <span class="font-medium text-gray-900">{{ progress }}%</span>
          </div>
          <div class="w-full bg-gray-200 rounded-full h-2">
            <div
              class="h-2 rounded-full transition-all duration-300"
              :class="progressClasses"
              :style="`width: ${Math.min(progress, 100)}%`"
            />
          </div>
        </div>
      </div>
    </template>

    <template v-if="$slots.footer" #footer>
      <slot name="footer" />
    </template>
  </Card>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import {
  ArrowUpIcon,
  ArrowDownIcon,
  MinusIcon
} from '@heroicons/vue/24/outline'
import Card from './Card.vue'

interface Props {
  title: string
  value: string | number
  subtitle?: string
  icon: any
  color?: 'blue' | 'green' | 'red' | 'yellow' | 'purple' | 'pink' | 'indigo' | 'gray'
  variant?: 'default' | 'elevated' | 'outlined' | 'gradient'
  trend?: string
  trendDirection?: 'up' | 'down' | 'neutral'
  progress?: number
  prefix?: string
  suffix?: string
  precision?: number
  hover?: boolean
  clickable?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  color: 'blue',
  variant: 'default',
  trendDirection: 'neutral',
  precision: 0,
  hover: false,
  clickable: false
})

const emit = defineEmits<{
  click: [event: Event]
}>()

const formattedValue = computed(() => {
  let value = props.value
  
  if (typeof value === 'number') {
    if (props.precision > 0) {
      value = value.toFixed(props.precision)
    } else {
      value = Math.round(value).toLocaleString()
    }
  }
  
  return `${props.prefix || ''}${value}${props.suffix || ''}`
})

const iconClasses = computed(() => {
  switch (props.color) {
    case 'blue':
      return 'from-blue-500 to-blue-600'
    case 'green':
      return 'from-green-500 to-green-600'
    case 'red':
      return 'from-red-500 to-red-600'
    case 'yellow':
      return 'from-yellow-500 to-yellow-600'
    case 'purple':
      return 'from-purple-500 to-purple-600'
    case 'pink':
      return 'from-pink-500 to-pink-600'
    case 'indigo':
      return 'from-indigo-500 to-indigo-600'
    case 'gray':
      return 'from-gray-500 to-gray-600'
    default:
      return 'from-blue-500 to-blue-600'
  }
})

const trendIcon = computed(() => {
  switch (props.trendDirection) {
    case 'up':
      return ArrowUpIcon
    case 'down':
      return ArrowDownIcon
    case 'neutral':
    default:
      return MinusIcon
  }
})

const trendClasses = computed(() => {
  switch (props.trendDirection) {
    case 'up':
      return 'text-green-600'
    case 'down':
      return 'text-red-600'
    case 'neutral':
    default:
      return 'text-gray-500'
  }
})

const progressClasses = computed(() => {
  switch (props.color) {
    case 'blue':
      return 'bg-blue-500'
    case 'green':
      return 'bg-green-500'
    case 'red':
      return 'bg-red-500'
    case 'yellow':
      return 'bg-yellow-500'
    case 'purple':
      return 'bg-purple-500'
    case 'pink':
      return 'bg-pink-500'
    case 'indigo':
      return 'bg-indigo-500'
    case 'gray':
      return 'bg-gray-500'
    default:
      return 'bg-blue-500'
  }
})
</script>
