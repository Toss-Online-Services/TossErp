<script setup lang="ts">
import { ref, inject, type Ref } from 'vue'
import { Menu, PanelLeftClose, PanelLeft, Wifi, WifiOff, Moon, Sun, Bell, Search, ChevronDown, User, Settings, LogOut } from 'lucide-vue-next'
import { useThemeStore } from '~/stores/theme'
import { useOfflineStatus } from '~/composables/useOfflineStatus'

const emit = defineEmits<{
  'toggle-sidebar': []
}>()

const theme = useThemeStore()
const { isOnline } = useOfflineStatus()

const sidebarCollapsed = inject<Ref<boolean>>('sidebarCollapsed')
const toggleCollapse = inject<() => void>('toggleCollapse')

const accountDropdownOpen = ref(false)
const searchQuery = ref('')

const handleSearch = (e: Event) => {
  e.preventDefault()
  console.log('Search:', searchQuery.value)
}

const toggleAccountDropdown = () => {
  accountDropdownOpen.value = !accountDropdownOpen.value
}

const closeAccountDropdown = () => {
  accountDropdownOpen.value = false
}
</script>

<template>
  <nav class="bg-white dark:bg-stone-900 border-b border-stone-200 dark:border-stone-700 sticky top-0 z-40">
    <div class="px-4 lg:px-8">
      <div class="flex items-center justify-between h-16">
        <div class="flex items-center gap-4">
          <!-- Mobile menu toggle -->
          <button
            @click="emit('toggle-sidebar')"
            class="lg:hidden p-2 hover:bg-stone-100 dark:hover:bg-stone-800 rounded-md transition-colors"
            aria-label="Toggle menu"
          >
            <Menu :size="20" class="text-stone-700 dark:text-stone-300" />
          </button>

          <!-- Search form -->
          <form @submit="handleSearch" class="hidden md:block">
            <div class="relative">
              <Search :size="18" class="absolute left-3 top-1/2 -translate-y-1/2 text-stone-400" />
              <input
                v-model="searchQuery"
                type="search"
                placeholder="Search..."
                class="pl-10 pr-4 py-2 bg-background border border-stone-200 dark:border-stone-700 rounded-md focus:outline-none focus:ring-2 focus:ring-stone-400 w-64 lg:w-96 text-sm"
              />
            </div>
          </form>
        </div>

        <div class="flex items-center gap-2">
          <!-- Desktop sidebar toggle -->
          <button
            v-if="toggleCollapse"
            @click="toggleCollapse"
            class="hidden lg:block p-2 hover:bg-stone-100 dark:hover:bg-stone-800 rounded-md transition-colors"
            :aria-label="sidebarCollapsed ? 'Expand sidebar' : 'Collapse sidebar'"
          >
            <component :is="sidebarCollapsed ? PanelLeft : PanelLeftClose" :size="20" class="text-stone-600 dark:text-stone-400" />
          </button>

          <!-- Online status -->
          <span
            class="hidden sm:inline-flex items-center gap-1.5 rounded-full px-2.5 py-1 text-xs font-normal"
            :class="isOnline 
              ? 'text-green-600 dark:text-green-400 bg-green-50 dark:bg-green-900/20' 
              : 'text-amber-600 dark:text-amber-400 bg-amber-50 dark:bg-amber-900/20'"
            :title="isOnline ? 'Connected' : 'Offline mode'"
          >
            <component :is="isOnline ? Wifi : WifiOff" class="h-3.5 w-3.5" />
            {{ isOnline ? 'Online' : 'Offline' }}
          </span>

          <!-- Notifications -->
          <button
            class="relative p-2 hover:bg-stone-100 dark:hover:bg-stone-800 rounded-md transition-colors"
            title="Notifications"
          >
            <Bell :size="20" class="text-stone-600 dark:text-stone-400" />
            <span class="absolute top-1 right-1 h-4 w-4 rounded-full bg-red-500 text-white text-[10px] font-medium flex items-center justify-center">
              3
            </span>
          </button>

          <!-- Theme toggle -->
          <button
            @click="theme.toggle()"
            class="p-2 hover:bg-stone-100 dark:hover:bg-stone-800 rounded-md transition-colors"
            :title="theme.isDark ? 'Switch to light mode' : 'Switch to dark mode'"
          >
            <component :is="theme.isDark ? Sun : Moon" :size="20" class="text-stone-600 dark:text-stone-400" />
          </button>

          <!-- Account dropdown -->
          <div class="relative">
            <button
              @click="toggleAccountDropdown"
              class="flex items-center gap-2 px-3 py-2 hover:bg-stone-100 dark:hover:bg-stone-800 rounded-md transition-colors"
              aria-label="Account menu"
            >
              <div class="w-8 h-8 rounded-full bg-stone-800 dark:bg-stone-100 text-stone-50 dark:text-stone-900 flex items-center justify-center text-sm font-semibold">
                <User :size="18" />
              </div>
              <span class="hidden sm:block text-sm font-medium text-stone-700 dark:text-stone-300">Account</span>
              <ChevronDown :size="16" class="hidden sm:block text-stone-500" />
            </button>

            <div
              v-if="accountDropdownOpen"
              @click="closeAccountDropdown"
              class="fixed inset-0 z-40"
            />

            <transition
              enter-active-class="transition ease-out duration-100"
              enter-from-class="transform opacity-0 scale-95"
              enter-to-class="transform opacity-100 scale-100"
              leave-active-class="transition ease-in duration-75"
              leave-from-class="transform opacity-100 scale-100"
              leave-to-class="transform opacity-0 scale-95"
            >
              <div
                v-if="accountDropdownOpen"
                class="absolute right-0 mt-2 w-56 bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700 rounded-md shadow-lg py-1 z-50"
              >
                <div class="px-4 py-3 border-b border-stone-200 dark:border-stone-700">
                  <p class="text-sm font-medium text-stone-900 dark:text-white">Admin User</p>
                  <p class="text-xs text-stone-500 dark:text-stone-400">admin@toss.co.za</p>
                </div>

                <NuxtLink
                  to="/settings"
                  @click="closeAccountDropdown"
                  class="w-full flex items-center gap-2 px-4 py-2 text-sm text-stone-700 dark:text-stone-300 hover:bg-stone-100 dark:hover:bg-stone-700 transition-colors"
                >
                  <Settings :size="16" />
                  <span>Settings</span>
                </NuxtLink>

                <NuxtLink
                  to="/admin/users"
                  @click="closeAccountDropdown"
                  class="w-full flex items-center gap-2 px-4 py-2 text-sm text-stone-700 dark:text-stone-300 hover:bg-stone-100 dark:hover:bg-stone-700 transition-colors"
                >
                  <User :size="16" />
                  <span>Profile</span>
                </NuxtLink>

                <div class="border-t border-stone-200 dark:border-stone-700 my-1" />

                <button
                  @click="closeAccountDropdown"
                  class="w-full flex items-center gap-2 px-4 py-2 text-sm text-red-600 dark:text-red-400 hover:bg-stone-100 dark:hover:bg-stone-700 transition-colors"
                >
                  <LogOut :size="16" />
                  <span>Logout</span>
                </button>
              </div>
            </transition>
          </div>
        </div>
      </div>

      <!-- Mobile search -->
      <div class="md:hidden pb-3">
        <form @submit="handleSearch">
          <div class="relative">
            <Search :size="18" class="absolute left-3 top-1/2 -translate-y-1/2 text-stone-400" />
            <input
              v-model="searchQuery"
              type="search"
              placeholder="Search..."
              class="w-full pl-10 pr-4 py-2 bg-background border border-stone-200 dark:border-stone-700 rounded-md focus:outline-none focus:ring-2 focus:ring-stone-400 text-sm"
            />
          </div>
        </form>
      </div>
    </div>
  </nav>
</template>
