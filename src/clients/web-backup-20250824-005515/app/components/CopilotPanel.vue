<template>
  <!-- Floating AI Co-Pilot -->
  <div class="fixed bottom-6 right-6 z-50">
    <div class="relative">
      <!-- AI Co-Pilot Button -->
      <button @click="toggleCopilot" data-testid="ai-copilot-trigger" class="h-16 w-16 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-full shadow-lg hover:shadow-xl transform hover:scale-110 transition-all duration-300 animate-pulse-slow flex items-center justify-center">
        <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
        </svg>
      </button>
      <!-- Notification Badge -->
      <div class="absolute -top-2 -right-2 h-6 w-6 bg-red-500 text-white text-xs font-bold rounded-full flex items-center justify-center animate-bounce">
        {{ copilotNotifications }}
      </div>
      <!-- Quick Action Floating Bubbles -->
      <div v-if="showQuickActions" class="fixed bottom-20 right-4 space-y-3">
        <div v-for="action in quickActions" :key="action.id" 
             @click="executeQuickAction(action)" 
             class="w-12 h-12 bg-white dark:bg-gray-800 text-gray-700 dark:text-gray-300 rounded-full shadow-lg hover:shadow-xl transform hover:scale-110 transition-all duration-300 flex items-center justify-center cursor-pointer border border-gray-200 dark:border-gray-600 hover:border-purple-300 dark:hover:border-purple-500"
             :title="action.title">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" :d="action.icon" />
          </svg>
        </div>
      </div>
      <!-- AI Co-Pilot Backdrop -->
      <div v-if="showCopilot" @click.self="showCopilot = false" class="fixed inset-0 z-40 bg-black bg-opacity-25"></div>
      <!-- AI Co-Pilot Chat Panel -->
      <div v-if="showCopilot" @click.stop data-testid="ai-copilot-panel" class="fixed bottom-4 right-4 w-96 max-h-[calc(100vh-100px)] bg-white dark:bg-gray-800 rounded-lg shadow-2xl border border-gray-200 dark:border-gray-700 transform transition-all duration-300 scale-100 origin-bottom-right flex flex-col z-50">
        <!-- ...existing code for chat header, tabs, content... -->
        <slot />
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from 'vue'
import { defineOptions } from 'vue'
defineOptions({ name: 'CopilotPanel' })
const showCopilot = ref(false)
const copilotNotifications = ref(0)
const showQuickActions = ref(false)
const quickActions = ref([
  { id: 1, title: 'Action 1', icon: 'M5 12h14' },
  { id: 2, title: 'Action 2', icon: 'M12 5v14' }
])
function toggleCopilot() {
  showCopilot.value = !showCopilot.value
}
function executeQuickAction(action: { id: number; title: string; icon: string }) {
  // Implement action logic here
}
</script>
