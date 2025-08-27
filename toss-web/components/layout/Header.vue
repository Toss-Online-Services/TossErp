<template>
  <header class="h-16 flex items-center justify-between px-4 sm:px-6 bg-white dark:bg-slate-800 border-b border-slate-200 dark:border-slate-700 shadow-sm">
    <!-- Search Section -->
    <div class="flex-1 max-w-lg">
      <div class="relative">
        <MagnifyingGlassIcon class="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-slate-400" />
        <input 
          type="text" 
          placeholder="Search..." 
          class="w-full pl-10 pr-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-slate-50 dark:bg-slate-700 text-slate-900 dark:text-slate-100 placeholder-slate-500 focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm sm:text-base"
        />
      </div>
    </div>

    <!-- Actions -->
    <div class="flex items-center space-x-2 sm:space-x-4 ml-4">
      <!-- Notifications -->
      <button class="relative p-2 text-slate-400 hover:text-slate-600 dark:hover:text-slate-300 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-700 transition-colors">
        <BellIcon class="w-5 h-5" />
        <span class="absolute top-1 right-1 w-2 h-2 bg-red-500 rounded-full"></span>
      </button>

      <!-- Theme Switcher -->
      <ThemeSwitcher />
      
      <!-- Desktop User Menu -->
      <div class="hidden sm:block">
        <UserMenu />
      </div>
    </div>
  </header>
</template>

<script setup>
import { MagnifyingGlassIcon, BellIcon } from '@heroicons/vue/24/outline'
import ThemeSwitcher from '~/components/common/ThemeSwitcher.vue'
import UserMenu from '~/components/common/UserMenu.vue'
</script>
