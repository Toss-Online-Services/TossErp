<script setup lang="ts">
interface Props {
  title: string
  value: string | number
  change?: string
  changeType?: 'positive' | 'negative' | 'neutral'
  icon: string
  iconBg?: string
}

const props = withDefaults(defineProps<Props>(), {
  iconBg: 'bg-gradient-to-br from-gray-700 to-gray-900',
  changeType: 'neutral'
})

const changeColor = {
  positive: 'text-green-600',
  negative: 'text-red-600',
  neutral: 'text-gray-600'
}
</script>

<template>
  <div class="bg-white rounded-xl shadow-sm hover:shadow-md transition-shadow duration-200">
    <div class="p-4">
      <div class="flex justify-between items-start">
        <div class="flex-1">
          <p class="text-sm text-gray-600 mb-1 capitalize">{{ title }}</p>
          <h4 class="text-2xl font-bold text-gray-900 mb-0">{{ value }}</h4>
        </div>
        <div :class="['w-12 h-12 rounded-lg flex items-center justify-center shadow-lg', iconBg]">
          <span class="material-symbols-rounded text-white text-2xl">{{ icon }}</span>
        </div>
      </div>
    </div>
    <hr class="border-gray-200 my-0">
    <div class="px-4 py-2">
      <p v-if="change" class="text-sm mb-0">
        <span :class="['font-semibold', changeColor[changeType]]">{{ change }}</span>
        <span class="text-gray-600 ml-1">than last period</span>
      </p>
      <p v-else class="text-sm text-gray-600 mb-0">Just updated</p>
    </div>
  </div>
</template>

