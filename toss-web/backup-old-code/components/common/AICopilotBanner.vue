<template>
  <div v-if="show" class="mb-6 bg-gradient-to-r from-blue-500 via-purple-500 to-pink-500 rounded-2xl shadow-2xl p-4 sm:p-6 text-white relative overflow-hidden">
    <div class="absolute top-0 right-0 w-32 h-32 sm:w-64 sm:h-64 bg-white/10 rounded-full -mr-16 sm:-mr-32 -mt-16 sm:-mt-32"></div>
    <div class="relative z-10 flex items-start gap-3 sm:gap-4">
      <div class="p-2 sm:p-3 bg-white/20 backdrop-blur-sm rounded-xl flex-shrink-0">
        <SparklesIcon class="w-5 h-5 sm:w-6 sm:h-6" />
      </div>
      <div class="flex-1 min-w-0">
        <h3 class="text-base sm:text-lg font-bold mb-1 sm:mb-2">{{ title }}</h3>
        <p class="text-white/90 text-xs sm:text-sm leading-relaxed break-words">
          {{ message }}
        </p>
        <div class="flex flex-wrap gap-2 sm:gap-3 mt-3 sm:mt-4">
          <button 
            v-for="(action, index) in actions"
            :key="index"
            @click="action.handler"
            :class="[
              'px-4 sm:px-5 py-2 sm:py-2.5 rounded-lg text-xs sm:text-sm font-medium transition-all duration-200',
              index === 0 
                ? 'bg-white/20 hover:bg-white/30 backdrop-blur-sm'
                : 'bg-white/10 hover:bg-white/20 backdrop-blur-sm'
            ]"
          >
            {{ action.label }}
          </button>
          <button 
            v-if="dismissible"
            @click="$emit('dismiss')"
            class="px-4 sm:px-5 py-2 sm:py-2.5 bg-white/10 hover:bg-white/20 backdrop-blur-sm rounded-lg text-xs sm:text-sm font-medium transition-all duration-200"
          >
            Dismiss
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { SparklesIcon } from '@heroicons/vue/24/outline'

interface Action {
  label: string
  handler: () => void
}

const props = defineProps<{
  show?: boolean
  title: string
  message: string
  actions: Action[]
  dismissible?: boolean
}>()

const emit = defineEmits<{
  dismiss: []
}>()
</script>

