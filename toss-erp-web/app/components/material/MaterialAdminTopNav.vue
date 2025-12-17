<template>
  <header class="sticky top-0 z-30 bg-white dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700 shadow-sm">
    <div class="flex items-center justify-between h-16 px-4 lg:px-6">
      <!-- Left: Menu & Breadcrumbs -->
      <div class="flex items-center space-x-4">
        <button
          @click="emit('toggle-sidebar')"
          class="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700 text-gray-600 dark:text-gray-400 lg:hidden"
        >
          <Menu class="w-5 h-5" />
        </button>
        
        <div class="hidden md:flex items-center space-x-2 text-sm">
          <span class="text-gray-500 dark:text-gray-400">Pages</span>
          <ChevronRight class="w-4 h-4 text-gray-400" />
          <span class="font-semibold text-gray-900 dark:text-white">{{ title }}</span>
        </div>
      </div>

      <!-- Center: Search -->
      <div class="flex-1 max-w-md mx-4 hidden lg:block">
        <div class="relative">
          <MagnifyingGlassIcon class="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
          <input
            type="text"
            placeholder="Search here..."
            class="w-full pl-10 pr-4 py-2 bg-gray-50 dark:bg-gray-700 border border-gray-200 dark:border-gray-600 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
            @input="handleSearchInput"
          />
        </div>
      </div>

      <!-- Right: Actions -->
      <div class="flex items-center space-x-2">
        <!-- Notifications -->
        <button
          class="relative p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700 text-gray-600 dark:text-gray-400"
        >
          <BellIcon class="w-5 h-5" />
          <span
            v-if="notificationCount > 0"
            class="absolute top-1 right-1 w-2 h-2 bg-red-500 rounded-full"
          />
        </button>

        <!-- Settings -->
        <button
          class="p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700 text-gray-600 dark:text-gray-400"
        >
          <Cog6ToothIcon class="w-5 h-5" />
        </button>

        <!-- User Menu -->
        <div class="relative">
          <button
            @click="showUserMenu = !showUserMenu"
            class="flex items-center space-x-2 p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700"
          >
            <img
              src="https://ui-avatars.com/api/?name=Admin&background=6366f1&color=fff"
              alt="User"
              class="w-8 h-8 rounded-full"
            />
          </button>
        </div>
      </div>
    </div>

    <!-- Page Title & Subtitle -->
    <div v-if="title || subtitle" class="px-4 lg:px-6 pb-4">
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white">{{ title }}</h1>
      <p v-if="subtitle" class="text-sm text-gray-600 dark:text-gray-400 mt-1">{{ subtitle }}</p>
    </div>
  </header>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { Menu, Bell as BellIcon, Cog6Tooth as Cog6ToothIcon, MagnifyingGlass as MagnifyingGlassIcon, ChevronRight } from '@heroicons/vue/24/outline'

interface Props {
  title?: string
  subtitle?: string
  notificationCount?: number
}

withDefaults(defineProps<Props>(), {
  notificationCount: 0
})

const emit = defineEmits(['toggle-sidebar', 'search'])

const showUserMenu = ref(false)

const handleSearchInput = (event: Event) => {
  const target = event.target as HTMLInputElement
  emit('search', target.value as string)
}
</script>

