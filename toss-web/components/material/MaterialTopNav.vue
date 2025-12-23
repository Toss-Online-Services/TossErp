<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { Search, Bell, Menu, ChevronDown, User, Command, Wifi, WifiOff } from 'lucide-vue-next'
import { useNetworkStatus } from '~/composables/useNetworkStatus'
import { useOutbox } from '~/composables/useOutbox'
import CommandPalette from '~/components/common/CommandPalette.vue'

interface Props {
  title?: string
  subtitle?: string
  notificationCount?: number
  userInfo?: {
    name: string
    email: string
    avatar?: string
  }
}

const props = withDefaults(defineProps<Props>(), {
  notificationCount: 0,
  userInfo: () => ({ name: 'User', email: 'user@example.com' })
})

const emit = defineEmits<{
  'toggle-sidebar': []
  'search': [query: string]
}>()

const route = useRoute()
const router = useRouter()
const { isOnline } = useNetworkStatus()
const { pendingCount } = useOutbox()

const searchQuery = ref('')
const showNotifications = ref(false)
const showUserMenu = ref(false)
const showCommandPalette = ref(false)

const handleSearch = (e: Event) => {
  e.preventDefault()
  emit('search', searchQuery.value)
}

const toggleNotifications = () => {
  showNotifications.value = !showNotifications.value
  showUserMenu.value = false
}

const toggleUserMenu = () => {
  showUserMenu.value = !showUserMenu.value
  showNotifications.value = false
}

const openCommandPalette = () => {
  showCommandPalette.value = true
}

const commandPaletteCommands = computed(() => [
  {
    id: 'dashboard',
    label: 'Go to Dashboard',
    action: () => router.push('/dashboard')
  },
  {
    id: 'sales',
    label: 'Go to Sales',
    action: () => router.push('/sales')
  },
  {
    id: 'pos',
    label: 'Open POS',
    action: () => router.push('/pos')
  },
  {
    id: 'stock',
    label: 'View Stock',
    action: () => router.push('/stock')
  },
  {
    id: 'buying',
    label: 'View Buying',
    action: () => router.push('/buying')
  },
  {
    id: 'settings',
    label: 'Open Settings',
    action: () => router.push('/settings')
  }
])

const handleKeyDown = (e: KeyboardEvent) => {
  if ((e.metaKey || e.ctrlKey) && e.key === 'k') {
    e.preventDefault()
    openCommandPalette()
  }
}

onMounted(() => {
  document.addEventListener('keydown', handleKeyDown)
})

onUnmounted(() => {
  document.removeEventListener('keydown', handleKeyDown)
})
</script>

<template>
  <nav class="sticky top-0 z-40 bg-white border-b border-gray-200 shadow-material mt-2 mx-3 border-radius-lg">
    <div class="px-4 lg:px-6">
      <div class="flex items-center justify-between h-16">
        <!-- Left Section -->
        <div class="flex items-center gap-4">
          <button
            @click="emit('toggle-sidebar')"
            class="p-2 rounded-lg hover:bg-accent transition-colors lg:hidden"
            aria-label="Toggle sidebar"
          >
            <Menu :size="20" />
          </button>

          <!-- Breadcrumbs -->
          <div v-if="title" class="hidden md:block">
            <h1 class="text-lg font-semibold text-foreground">{{ title }}</h1>
            <p v-if="subtitle" class="text-xs text-muted-foreground">{{ subtitle }}</p>
          </div>

          <!-- Search -->
          <button
            @click="openCommandPalette"
            class="hidden md:flex items-center gap-2 px-3 py-2 bg-background border border-border rounded-lg text-sm text-muted-foreground hover:border-primary transition-colors w-64 lg:w-96"
          >
            <Search :size="18" />
            <span class="flex-1 text-left">Search...</span>
            <kbd class="hidden lg:inline-flex items-center gap-1 px-2 py-0.5 text-xs font-semibold text-muted-foreground bg-muted rounded border border-border">
              <Command :size="12" />K
            </kbd>
          </button>
        </div>

        <!-- Right Section -->
        <div class="flex items-center gap-2">
          <!-- Offline Indicator -->
          <div class="relative">
            <div
              :class="[
                'flex items-center gap-1.5 px-2.5 py-1.5 rounded-lg text-xs font-medium',
                isOnline ? 'bg-green-50 dark:bg-green-900/20 text-green-700 dark:text-green-400' : 'bg-orange-50 dark:bg-orange-900/20 text-orange-700 dark:text-orange-400'
              ]"
            >
              <Wifi v-if="isOnline" :size="14" />
              <WifiOff v-else :size="14" />
              <span class="hidden sm:inline">{{ isOnline ? 'Online' : 'Offline' }}</span>
              <span v-if="!isOnline && pendingCount > 0" class="ml-1 px-1.5 py-0.5 bg-orange-200 dark:bg-orange-800 rounded text-xs">
                {{ pendingCount }}
              </span>
            </div>
          </div>

          <!-- Search Button (Mobile) -->
          <button
            @click="openCommandPalette"
            class="p-2 rounded-lg hover:bg-accent transition-colors md:hidden"
            aria-label="Search"
          >
            <Search :size="20" />
          </button>

          <!-- Notifications -->
          <div class="relative">
            <button
              @click="toggleNotifications"
              class="relative p-2 rounded-lg hover:bg-accent transition-colors"
              aria-label="Notifications"
            >
              <Bell :size="20" />
              <span
                v-if="notificationCount > 0"
                class="absolute top-1 right-1 w-2 h-2 bg-destructive rounded-full"
              />
            </button>

            <!-- Notifications Dropdown -->
            <div
              v-if="showNotifications"
              class="absolute right-0 mt-2 w-80 bg-card border border-border rounded-lg shadow-lg z-50"
            >
              <div class="p-4 border-b border-border">
                <h3 class="text-sm font-semibold">Notifications</h3>
              </div>
              <div class="max-h-96 overflow-y-auto">
                <div class="p-4 text-sm text-muted-foreground text-center">
                  No new notifications
                </div>
              </div>
            </div>
          </div>

          <!-- User Menu -->
          <div class="relative">
            <button
              @click="toggleUserMenu"
              class="flex items-center gap-2 px-3 py-2 rounded-lg hover:bg-accent transition-colors"
              aria-label="User menu"
            >
              <div class="w-8 h-8 rounded-full bg-primary text-primary-foreground flex items-center justify-center shadow-sm">
                <User :size="18" />
              </div>
              <ChevronDown :size="16" class="hidden sm:block" />
            </button>

            <!-- User Dropdown -->
            <div
              v-if="showUserMenu"
              class="absolute right-0 mt-2 w-56 bg-card border border-border rounded-lg shadow-lg z-50"
            >
              <div class="px-4 py-3 border-b border-border">
                <p class="text-sm font-medium">{{ userInfo.name }}</p>
                <p class="text-xs text-muted-foreground">{{ userInfo.email }}</p>
              </div>
              <NuxtLink
                to="/settings"
                class="block px-4 py-2 text-sm hover:bg-accent rounded-b-lg"
                @click="showUserMenu = false"
              >
                Settings
              </NuxtLink>
              <button
                class="w-full text-left px-4 py-2 text-sm text-destructive hover:bg-accent"
                @click="showUserMenu = false"
              >
                Logout
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Mobile Search -->
      <div class="pb-3 md:hidden">
        <form @submit="handleSearch">
          <div class="relative">
            <Search
              :size="18"
              class="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground"
            />
            <input
              v-model="searchQuery"
              type="search"
              placeholder="Search..."
              class="w-full pl-10 pr-4 py-2 bg-background border border-border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
        </form>
      </div>
    </div>

    <!-- Click outside to close menus -->
    <div
      v-if="showNotifications || showUserMenu"
      @click="showNotifications = false; showUserMenu = false"
      class="fixed inset-0 z-40"
    />

    <!-- Command Palette -->
    <CommandPalette v-model:open="showCommandPalette" :commands="commandPaletteCommands" />
  </nav>
</template>
