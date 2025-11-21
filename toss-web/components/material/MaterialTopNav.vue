<script setup lang="ts">
import { ref } from 'vue'
import { cn } from '~/lib/utils'
import {
  Bars3Icon,
  BellIcon,
  MagnifyingGlassIcon,
  SunIcon,
  MoonIcon
} from '@heroicons/vue/24/outline'

interface Props {
  title?: string
  subtitle?: string
  showSearch?: boolean
  showNotifications?: boolean
  showThemeToggle?: boolean
  notificationCount?: number
}

const props = withDefaults(defineProps<Props>(), {
  showSearch: true,
  showNotifications: true,
  showThemeToggle: true,
  notificationCount: 0
})

const emit = defineEmits<{
  'toggle-sidebar': []
  'search': [query: string]
}>()

const searchQuery = ref('')
const colorMode = useColorMode()

const toggleTheme = () => {
  colorMode.preference = colorMode.value === 'dark' ? 'light' : 'dark'
}

const handleSearch = () => {
  if (searchQuery.value.trim()) {
    emit('search', searchQuery.value)
  }
}
</script>

<template>
  <header class="sticky top-0 z-30 bg-white/95 dark:bg-slate-900/95 backdrop-blur-md border-b border-slate-200 dark:border-slate-800 shadow-sm">
    <div class="flex items-center justify-between h-16 px-4 lg:px-6">
      <!-- Left Section -->
      <div class="flex items-center space-x-4">
        <!-- Mobile Menu Toggle -->
        <MaterialButton
          variant="text"
          icon
          @click="$emit('toggle-sidebar')"
          class="lg:hidden"
        >
          <Bars3Icon class="w-6 h-6" />
        </MaterialButton>

        <!-- Page Title -->
        <div v-if="title" class="hidden sm:block">
          <h1 class="text-xl font-bold text-slate-900 dark:text-white">{{ title }}</h1>
          <p v-if="subtitle" class="text-xs text-slate-500 dark:text-slate-400">{{ subtitle }}</p>
        </div>
      </div>

      <!-- Center Section - Search (Desktop) -->
      <div v-if="showSearch" class="hidden md:flex flex-1 max-w-xl mx-8">
        <div class="relative w-full">
          <MagnifyingGlassIcon class="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
          <input
            v-model="searchQuery"
            type="text"
            placeholder="Search products, orders, customers..."
            @keyup.enter="handleSearch"
            class="w-full pl-10 pr-4 py-2.5 bg-slate-50 dark:bg-slate-800 border-2 border-transparent rounded-lg text-sm focus:outline-none focus:ring-4 focus:ring-orange-500/50 focus:border-orange-500 dark:focus:border-orange-500 transition-all"
          />
        </div>
      </div>

      <!-- Right Section -->
      <div class="flex items-center space-x-2">
        <!-- Mobile Search Toggle -->
        <MaterialButton
          v-if="showSearch"
          variant="text"
          icon
          class="md:hidden"
        >
          <MagnifyingGlassIcon class="w-5 h-5" />
        </MaterialButton>

        <!-- Notifications -->
        <div v-if="showNotifications" class="relative">
          <MaterialButton
            variant="text"
            icon
          >
            <BellIcon class="w-5 h-5" />
            <span
              v-if="notificationCount > 0"
              class="absolute -top-1 -right-1 w-5 h-5 bg-red-500 text-white text-xs font-bold rounded-full flex items-center justify-center"
            >
              {{ notificationCount > 9 ? '9+' : notificationCount }}
            </span>
          </MaterialButton>
        </div>

        <!-- Theme Toggle -->
        <MaterialButton
          v-if="showThemeToggle"
          variant="text"
          icon
          @click="toggleTheme"
        >
          <SunIcon v-if="colorMode.value === 'dark'" class="w-5 h-5" />
          <MoonIcon v-else class="w-5 h-5" />
        </MaterialButton>

        <!-- User Menu -->
        <UiDropdownMenu>
          <UiDropdownMenuTrigger as-child>
            <button class="flex items-center space-x-2 p-1.5 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-800 transition-colors">
              <div class="w-8 h-8 rounded-full bg-gradient-to-br from-orange-400 to-orange-600 flex items-center justify-center">
                <span class="text-white font-bold text-sm">U</span>
              </div>
            </button>
          </UiDropdownMenuTrigger>
          <UiDropdownMenuContent align="end" class="w-56">
            <UiDropdownMenuLabel>My Account</UiDropdownMenuLabel>
            <UiDropdownMenuSeparator />
            <UiDropdownMenuItem>
              <NuxtLink to="/profile" class="flex items-center w-full">
                Profile
              </NuxtLink>
            </UiDropdownMenuItem>
            <UiDropdownMenuItem>
              <NuxtLink to="/settings" class="flex items-center w-full">
                Settings
              </NuxtLink>
            </UiDropdownMenuItem>
            <UiDropdownMenuSeparator />
            <UiDropdownMenuItem>
              <NuxtLink to="/auth/logout" class="flex items-center w-full text-red-600 dark:text-red-400">
                Sign out
              </NuxtLink>
            </UiDropdownMenuItem>
          </UiDropdownMenuContent>
        </UiDropdownMenu>
      </div>
    </div>

    <!-- Mobile Search Bar -->
    <div v-if="showSearch" class="md:hidden px-4 pb-3">
      <div class="relative">
        <MagnifyingGlassIcon class="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search..."
          @keyup.enter="handleSearch"
          class="w-full pl-10 pr-4 py-2.5 bg-slate-50 dark:bg-slate-800 border-2 border-transparent rounded-lg text-sm focus:outline-none focus:ring-4 focus:ring-orange-500/50 focus:border-orange-500"
        />
      </div>
    </div>
  </header>
</template>
