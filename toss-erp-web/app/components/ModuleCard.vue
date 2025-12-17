<template>
  <NuxtLink 
    :to="link" 
    :class="cardClass"
    class="group block p-6 rounded-lg shadow-sm border transition-all duration-200 hover:shadow-lg hover:scale-105"
  >
    <div class="flex items-center mb-3">
      <div class="text-3xl mr-3">{{ icon }}</div>
      <h3 class="text-lg font-semibold text-gray-900 dark:text-white group-hover:text-blue-600 dark:group-hover:text-blue-400 transition-colors">
        {{ title }}
      </h3>
    </div>
    
    <p class="text-sm text-gray-600 dark:text-gray-400 mb-4">
      {{ description }}
    </p>
    
    <div class="flex items-center justify-between">
      <span :class="badgeClass" class="px-2 py-1 rounded-full text-xs font-medium">
        {{ badgeText }}
      </span>
      
      <svg class="w-5 h-5 text-gray-400 group-hover:text-blue-600 dark:group-hover:text-blue-400 transition-colors" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path>
      </svg>
    </div>
  </NuxtLink>
</template>

<script setup lang="ts">
interface Props {
  title: string
  description: string
  icon: string
  link: string
  color?: string
  status?: 'active' | 'beta' | 'coming-soon'
}

const props = withDefaults(defineProps<Props>(), {
  color: 'blue',
  status: 'active'
})

const colorClasses = {
  blue: 'bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700 hover:border-blue-300 dark:hover:border-blue-600',
  green: 'bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700 hover:border-green-300 dark:hover:border-green-600',
  purple: 'bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700 hover:border-purple-300 dark:hover:border-purple-600',
  orange: 'bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700 hover:border-orange-300 dark:hover:border-orange-600',
  teal: 'bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700 hover:border-teal-300 dark:hover:border-teal-600',
  indigo: 'bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700 hover:border-indigo-300 dark:hover:border-indigo-600',
  gray: 'bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700 hover:border-gray-300 dark:hover:border-gray-600',
  emerald: 'bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700 hover:border-emerald-300 dark:hover:border-emerald-600',
  red: 'bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700 hover:border-red-300 dark:hover:border-red-600',
  yellow: 'bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700 hover:border-yellow-300 dark:hover:border-yellow-600',
  pink: 'bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700 hover:border-pink-300 dark:hover:border-pink-600',
}

const statusClasses = {
  active: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300',
  beta: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300',
  'coming-soon': 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-300'
}

const statusText = {
  active: 'Active',
  beta: 'Beta',
  'coming-soon': 'Coming Soon'
}

const cardClass = computed(() => colorClasses[props.color] || colorClasses.blue)
const badgeClass = computed(() => statusClasses[props.status])
const badgeText = computed(() => statusText[props.status])
</script>
