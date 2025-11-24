<template>
  <!-- Mobile Overlay -->
  <div
    v-if="isMobile && sidebarOpen"
    class="fixed inset-0 z-40 bg-black bg-opacity-50 lg:hidden"
    @click="sidebarOpen = false"
  />

  <!-- Sidebar -->
  <aside
    :class="[
      'fixed left-0 top-0 z-50 h-full bg-white dark:bg-gray-800 shadow-xl transition-all duration-300',
      isMobile ? (sidebarOpen ? 'translate-x-0' : '-translate-x-full') : '',
      collapsed ? 'w-20' : 'w-64'
    ]"
  >
    <!-- Logo Section -->
    <div class="flex items-center justify-between h-16 px-4 border-b border-gray-200 dark:border-gray-700">
      <div v-if="!collapsed" class="flex items-center space-x-3">
        <div class="flex items-center justify-center w-10 h-10 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-lg">
          <span class="text-xl font-bold text-white">T</span>
        </div>
        <div>
          <h1 class="text-lg font-bold text-gray-900 dark:text-white">TOSS</h1>
          <p class="text-xs text-gray-500 dark:text-gray-400">Admin Panel</p>
        </div>
      </div>
      <div v-else class="flex items-center justify-center w-full">
        <div class="flex items-center justify-center w-10 h-10 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-lg">
          <span class="text-xl font-bold text-white">T</span>
        </div>
      </div>
      <button
        v-if="!isMobile"
        @click="collapsed = !collapsed"
        class="p-1.5 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700 text-gray-600 dark:text-gray-400"
      >
        <ChevronLeft v-if="!collapsed" class="w-5 h-5" />
        <ChevronRight v-else class="w-5 h-5" />
      </button>
    </div>

    <!-- Navigation -->
    <nav class="flex-1 overflow-y-auto py-4">
      <!-- Dashboards Section -->
      <div class="px-4 mb-4">
        <p v-if="!collapsed" class="text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider mb-2">
          Dashboards
        </p>
      </div>
      <div class="space-y-1 px-2">
        <NuxtLink
          v-for="item in dashboardItems"
          :key="item.path"
          :to="item.path"
          :class="[
            'flex items-center space-x-3 px-4 py-3 rounded-lg transition-all duration-200',
            isActive(item.path)
              ? 'bg-gradient-to-r from-indigo-500 to-purple-600 text-white shadow-lg'
              : 'text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700'
          ]"
        >
          <component :is="item.icon" class="w-5 h-5 flex-shrink-0" />
          <span v-if="!collapsed" class="font-medium">{{ item.name }}</span>
        </NuxtLink>
      </div>

      <!-- Pages Section -->
      <div class="px-4 mt-6 mb-4">
        <p v-if="!collapsed" class="text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider mb-2">
          Pages
        </p>
      </div>
      <div class="space-y-1 px-2">
        <NuxtLink
          v-for="item in pageItems"
          :key="item.path"
          :to="item.path"
          :class="[
            'flex items-center space-x-3 px-4 py-3 rounded-lg transition-all duration-200',
            isActive(item.path)
              ? 'bg-gradient-to-r from-indigo-500 to-purple-600 text-white shadow-lg'
              : 'text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700'
          ]"
        >
          <component :is="item.icon" class="w-5 h-5 flex-shrink-0" />
          <span v-if="!collapsed" class="font-medium">{{ item.name }}</span>
        </NuxtLink>
      </div>
    </nav>

    <!-- User Profile Section -->
    <div class="border-t border-gray-200 dark:border-gray-700 p-4">
      <div class="flex items-center space-x-3">
        <img
          :src="userInfo.avatar"
          :alt="userInfo.name"
          class="w-10 h-10 rounded-full"
        />
        <div v-if="!collapsed" class="flex-1 min-w-0">
          <p class="text-sm font-semibold text-gray-900 dark:text-white truncate">
            {{ userInfo.name }}
          </p>
          <p class="text-xs text-gray-500 dark:text-gray-400 truncate">
            {{ userInfo.email }}
          </p>
        </div>
      </div>
      <div v-if="!collapsed" class="mt-3 space-y-1">
        <button
          class="flex items-center space-x-2 w-full px-4 py-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 rounded-lg transition-colors"
        >
          <UserCircle class="w-4 h-4" />
          <span>My Profile</span>
        </button>
        <button
          class="flex items-center space-x-2 w-full px-4 py-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 rounded-lg transition-colors"
        >
          <Settings class="w-4 h-4" />
          <span>Settings</span>
        </button>
        <button
          @click="handleLogout"
          class="flex items-center space-x-2 w-full px-4 py-2 text-sm text-red-600 dark:text-red-400 hover:bg-red-50 dark:hover:bg-red-900/20 rounded-lg transition-colors"
        >
          <LogOut class="w-4 h-4" />
          <span>Logout</span>
        </button>
      </div>
    </div>
  </aside>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  LayoutDashboard,
  Users,
  FileText,
  BarChart3,
  Settings,
  UserCircle,
  LogOut,
  ChevronLeft,
  ChevronRight
} from 'lucide-vue-next'
import { useAuth } from '~/composables/useAuth'

interface Props {
  open?: boolean
  userInfo?: {
    name: string
    email: string
    avatar?: string
  }
}

const props = withDefaults(defineProps<Props>(), {
  open: true
})

const emit = defineEmits<{
  'update:open': [value: boolean]
}>()

const route = useRoute()
const router = useRouter()
const { logout } = useAuth()
const isMobile = ref(false)
const collapsed = ref(false)

const sidebarOpen = computed({
  get: () => props.open,
  set: (value) => emit('update:open', value)
})

const checkMobile = () => {
  if (process.client) {
    isMobile.value = window.innerWidth < 1024
    if (isMobile.value) {
      sidebarOpen.value = false
    }
  }
}

const dashboardItems = [
  { name: 'Analytics', path: '/admin/dashboard', icon: LayoutDashboard }
]

const pageItems = [
  { name: 'Users', path: '/admin/users', icon: Users },
  { name: 'Orders', path: '/admin/orders', icon: FileText },
  { name: 'Analytics', path: '/sales/reports/analytics', icon: BarChart3 },
  { name: 'Settings', path: '/settings', icon: Settings }
]

const isActive = (path: string) => {
  return route.path === path || route.path.startsWith(path + '/')
}

const handleLogout = async () => {
  await logout()
  router.push('/auth/login')
}

onMounted(() => {
  if (process.client) {
    checkMobile()
    window.addEventListener('resize', checkMobile)
  }
})

onUnmounted(() => {
  if (process.client) {
    window.removeEventListener('resize', checkMobile)
  }
})
</script>

