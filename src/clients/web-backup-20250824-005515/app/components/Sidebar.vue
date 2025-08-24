<template>
  <div>
    <!-- Sidebar navigation, logo, modules -->
    <div class="fixed inset-y-0 left-0 z-50 w-64 bg-white dark:bg-gray-800 shadow-lg transform transition-transform duration-300 ease-in-out" :class="{ '-translate-x-full': !sidebarOpen, 'translate-x-0': sidebarOpen }">
      <!-- Logo Section -->
      <div class="flex items-center justify-between h-16 px-6 border-b border-gray-200 dark:border-gray-700">
        <NuxtLink to="/" class="flex items-center">
          <div class="w-8 h-8 bg-gradient-to-r from-purple-600 to-blue-600 rounded-lg flex items-center justify-center mr-3">
            <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
            </svg>
          </div>
          <span class="font-bold text-lg text-gray-900 dark:text-white">TOSS ERP III</span>
        </NuxtLink>
        <button @click="$emit('toggleSidebar')" class="lg:hidden p-1 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>
      <!-- Navigation Menu -->
      <nav class="flex-1 px-4 py-6 overflow-y-auto">
        <!-- Dashboard -->
        <div class="mb-6">
          <NuxtLink to="/" class="flex items-center px-4 py-3 text-gray-700 dark:text-gray-300 hover:bg-purple-50 dark:hover:bg-gray-700 hover:text-purple-600 dark:hover:text-purple-400 rounded-lg transition-colors duration-200">
            <svg class="w-5 h-5 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2H5a2 2 0 00-2-2z" />
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 5a2 2 0 012-2h4a2 2 0 012 2v3H8V5z" />
            </svg>
            <span class="font-medium">Dashboard</span>
          </NuxtLink>
        </div>
        <!-- Service-as-a-Software Section -->
        <div class="mb-6">
          <div class="px-4 py-2 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">
            Service-as-a-Software
          </div>
          <!-- ...copy all Service-as-a-Software links from default.vue... -->
        </div>
        <!-- Core ERP Modules -->
        <div class="mb-6">
          <div class="px-4 py-2 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">
            Core Modules
          </div>
          <!-- ...copy all Core Modules links from default.vue... -->
        </div>
        <!-- AI Tools -->
        <div class="mb-6">
          <div class="px-4 py-2 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wide">
            AI Tools
          </div>
          <!-- ...copy all AI Tools links from default.vue... -->
        </div>
      </nav>
    </div>
    <!-- Sidebar Overlay for Mobile -->
    <div v-show="sidebarOpen" @click="$emit('closeSidebar')" class="fixed inset-0 z-40 bg-black bg-opacity-50 lg:hidden"></div>
  </div>
</template>
<script setup lang="ts">
import { defineProps, defineEmits, defineOptions } from 'vue'
defineOptions({ name: 'Sidebar' })
const props = defineProps<{ sidebarOpen: boolean; openModules: any }>()
const emit = defineEmits(['toggleSidebar', 'closeSidebar'])
</script>
