<template>
  <div class="relative" :class="containerClass">
    <!-- Timeline Line -->
    <div 
      v-if="showLine"
      class="absolute top-0 bottom-0 w-0.5 bg-gray-200"
      :style="lineStyle"
    ></div>

    <!-- Timeline Items -->
    <div
      v-for="(item, index) in items"
      :key="item.id || index"
      class="relative mb-3 last:mb-0"
    >
      <!-- Timeline Icon -->
      <div
        v-if="item.icon || item.iconClass"
        class="flex absolute z-10 justify-center items-center w-12 h-12 rounded-full border-2 border-white shadow-sm"
        :class="[iconBaseClass, item.iconClass || iconClass]"
        :style="iconStyle"
      >
        <i v-if="item.icon" class="text-lg material-symbols-rounded">
          {{ item.icon }}
        </i>
        <slot v-else-if="$slots.icon" name="icon" :item="item" :index="index" />
      </div>

      <!-- Timeline Content -->
      <div class="pt-1" :class="contentClass">
        <slot name="item" :item="item" :index="index">
          <!-- Default content if no slot provided -->
          <h6 v-if="item.title" class="mb-0 text-sm font-semibold text-gray-900">
            {{ item.title }}
          </h6>
          <p v-if="item.date" class="mt-1 mb-0 text-xs text-gray-500">
            {{ item.date }}
          </p>
          <p v-if="item.description" class="mt-3 mb-2 text-sm text-gray-600">
            {{ item.description }}
          </p>
        </slot>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface TimelineItem {
  id?: string | number
  icon?: string
  iconClass?: string
  title?: string
  date?: string
  description?: string
  [key: string]: any
}

interface Props {
  items: TimelineItem[]
  showLine?: boolean
  linePosition?: 'left' | 'right' | 'center'
  iconPosition?: number // in rem (default: 1.5rem = 24px)
  containerClass?: string
  contentClass?: string
  iconClass?: string
  iconBaseClass?: string
}

const props = withDefaults(defineProps<Props>(), {
  showLine: true,
  linePosition: 'left',
  iconPosition: 1.5, // 1.5rem = 24px
  containerClass: 'pl-16',
  contentClass: 'ml-8',
  iconClass: 'bg-gray-100 text-gray-600',
  iconBaseClass: ''
})

const iconStyle = computed(() => {
  const position = props.iconPosition * 16 // Convert rem to px
  return {
    left: `${position}px`,
    transform: 'translateX(-50%)'
  }
})

const lineStyle = computed(() => {
  const position = props.iconPosition * 16 // Convert rem to px
  return {
    left: `${position}px`
  }
})
</script>

