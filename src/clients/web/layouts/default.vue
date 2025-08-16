<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Sidebar -->
    <aside 
      :class="[
        'sidebar',
        isSidebarOpen ? 'sidebar-open' : 'sidebar-closed'
      ]"
      @click.self="closeSidebar"
    >
      <div class="flex items-center justify-between h-16 px-6 border-b border-gray-200 dark:border-gray-700">
        <div class="flex items-center">
          <div class="w-8 h-8 bg-primary-600 rounded-lg flex items-center justify-center">
            <span class="text-white font-bold text-sm">T</span>
          </div>
          <span class="ml-3 text-lg font-semibold text-gray-900 dark:text-white">TOSS ERP</span>
        </div>
        <button
          @click="closeSidebar"
          class="p-1 rounded-lg text-gray-400 hover:text-gray-600 hover:bg-gray-100 dark:hover:text-gray-300 dark:hover:bg-gray-800 lg:hidden"
        >
          <XMarkIcon class="w-5 h-5" />
        </button>
      </div>
      
      <nav class="flex-1 px-4 py-6 space-y-2">
        <NuxtLink
          v-for="item in navigationItems"
          :key="item.name"
          :to="item.href"
          :class="[
            'nav-link',
            $route.path === item.href ? 'nav-link-active' : 'nav-link-inactive'
          ]"
        >
          <component :is="item.icon" class="w-5 h-5 mr-3" />
          {{ item.name }}
        </NuxtLink>
      </nav>
      
      <div class="px-4 py-4 border-t border-gray-200 dark:border-gray-700">
        <div class="flex items-center">
          <div class="w-8 h-8 bg-gray-300 dark:bg-gray-600 rounded-full"></div>
          <div class="ml-3">
            <p class="text-sm font-medium text-gray-900 dark:text-white">Admin User</p>
            <p class="text-xs text-gray-500 dark:text-gray-400">admin@toss-erp.com</p>
          </div>
        </div>
      </div>
    </aside>

    <!-- Main content -->
    <div class="lg:pl-64">
      <!-- Header -->
      <header class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
        <div class="flex items-center justify-between h-16 px-4 sm:px-6 lg:px-8">
          <div class="flex items-center">
            <button
              @click="openSidebar"
              class="p-2 rounded-lg text-gray-400 hover:text-gray-600 hover:bg-gray-100 dark:hover:text-gray-300 dark:hover:bg-gray-700 lg:hidden"
            >
              <Bars3Icon class="w-5 h-5" />
            </button>
            <h1 class="ml-4 text-xl font-semibold text-gray-900 dark:text-white">
              {{ pageTitle }}
            </h1>
          </div>
          
          <div class="flex items-center space-x-4">
            <!-- Color mode toggle -->
            <button
              @click="toggleColorMode"
              class="p-2 rounded-lg text-gray-400 hover:text-gray-600 hover:bg-gray-100 dark:hover:text-gray-300 dark:hover:bg-gray-700"
            >
              <SunIcon v-if="colorMode.value === 'dark'" class="w-5 h-5" />
              <MoonIcon v-else class="w-5 h-5" />
            </button>
            
            <!-- Notifications -->
            <button class="p-2 rounded-lg text-gray-400 hover:text-gray-600 hover:bg-gray-100 dark:hover:text-gray-300 dark:hover:bg-gray-700">
              <BellIcon class="w-5 h-5" />
            </button>
            
            <!-- User menu -->
            <div class="relative">
              <button
                @click="isUserMenuOpen = !isUserMenuOpen"
                class="flex items-center space-x-2 p-2 rounded-lg text-gray-400 hover:text-gray-600 hover:bg-gray-100 dark:hover:text-gray-300 dark:hover:bg-gray-700"
              >
                <div class="w-6 h-6 bg-gray-300 dark:bg-gray-600 rounded-full"></div>
                <ChevronDownIcon class="w-4 h-4" />
              </button>
              
              <div
                v-if="isUserMenuOpen"
                class="absolute right-0 mt-2 w-48 bg-white dark:bg-gray-800 rounded-lg shadow-large border border-gray-200 dark:border-gray-700 py-1"
              >
                <a href="#" class="block px-4 py-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700">
                  Profile
                </a>
                <a href="#" class="block px-4 py-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700">
                  Settings
                </a>
                <hr class="my-1 border-gray-200 dark:border-gray-700">
                <a href="#" class="block px-4 py-2 text-sm text-red-600 dark:text-red-400 hover:bg-gray-100 dark:hover:bg-gray-700">
                  Sign out
                </a>
              </div>
            </div>
          </div>
        </div>
      </header>

      <!-- Page content -->
      <main class="p-4 sm:p-6 lg:p-8">
        <slot />
      </main>
    </div>

    <!-- Mobile sidebar overlay -->
    <div
      v-if="isSidebarOpen"
      class="fixed inset-0 z-40 bg-black bg-opacity-50 lg:hidden"
      @click="closeSidebar"
    ></div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import {
  Bars3Icon,
  XMarkIcon,
  SunIcon,
  MoonIcon,
  BellIcon,
  ChevronDownIcon,
  HomeIcon,
  CubeIcon,
  ChartBarIcon,
  Cog6ToothIcon,
  UserGroupIcon,
  TruckIcon
} from '@heroicons/vue/24/outline'

// Reactive state
const isSidebarOpen = ref(false)
const isUserMenuOpen = ref(false)

// Composables
const colorMode = useColorMode()
const route = useRoute()

// Navigation items
const navigationItems = [
  { name: 'Dashboard', href: '/', icon: HomeIcon },
  { name: 'Stock Management', href: '/stock', icon: CubeIcon },
  { name: 'Items', href: '/items', icon: CubeIcon },
  { name: 'Warehouses', href: '/warehouses', icon: TruckIcon },
  { name: 'Reports', href: '/reports', icon: ChartBarIcon },
  { name: 'Users', href: '/users', icon: UserGroupIcon },
  { name: 'Settings', href: '/settings', icon: Cog6ToothIcon }
]

// Computed
const pageTitle = computed(() => {
  const currentItem = navigationItems.find(item => item.href === route.path)
  return currentItem?.name || 'Dashboard'
})

// Methods
const openSidebar = () => {
  isSidebarOpen.value = true
}

const closeSidebar = () => {
  isSidebarOpen.value = false
}

const toggleColorMode = () => {
  colorMode.preference = colorMode.value === 'dark' ? 'light' : 'dark'
}

// Close user menu when clicking outside
onMounted(() => {
  document.addEventListener('click', (event) => {
    const target = event.target as HTMLElement
    if (!target.closest('.relative')) {
      isUserMenuOpen.value = false
    }
  })
})
</script>
