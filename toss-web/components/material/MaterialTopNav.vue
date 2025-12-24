<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useNetworkStatus } from '~/composables/useNetworkStatus'
import { useOutbox } from '~/composables/useOutbox'
import CommandPalette from '~/components/common/CommandPalette.vue'
import MaterialSymbol from '~/components/common/MaterialSymbol.vue'
import { resolveBreadcrumbsFromNav } from '~/lib/navigation/materialDashboardRoutes'

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

const toTitleCase = (raw: string) => {
  return raw
    .split('-')
    .filter(Boolean)
    .map((word) => word.charAt(0).toUpperCase() + word.slice(1))
    .join(' ')
}

const breadcrumbs = computed(() => {
  const fromNav = resolveBreadcrumbsFromNav(route.path)
  if (fromNav.length) return fromNav

  const segments = route.path.split('/').filter(Boolean)
  const crumbs: { label: string; to?: string }[] = []
  let current = ''
  for (const segment of segments) {
    current += `/${segment}`
    crumbs.push({ label: toTitleCase(segment), to: current })
  }
  return crumbs
})

const pageTitle = computed(() => {
  const last = breadcrumbs.value.length ? breadcrumbs.value[breadcrumbs.value.length - 1] : undefined
  return props.title || last?.label || 'Dashboard'
})

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
  <nav class="sticky top-0 z-40 mx-4 mt-3 rounded-xl bg-background/80 backdrop-blur-md border border-border shadow-sm">
    <div class="px-4 lg:px-6">
      <div class="flex items-center justify-between h-16">
        <!-- Left: Menu + Breadcrumbs/Title -->
        <div class="flex items-center gap-4 min-w-0">
          <button
            @click="emit('toggle-sidebar')"
            class="p-2 rounded-lg hover:bg-accent transition-colors lg:hidden"
            aria-label="Toggle sidebar"
          >
            <MaterialSymbol name="menu" :size="20" />
          </button>

          <div class="min-w-0">
            <div class="hidden md:flex items-center gap-1 text-xs text-muted-foreground truncate">
              <template v-if="breadcrumbs.length">
                <NuxtLink
                  v-if="breadcrumbs[0]?.to"
                  :to="breadcrumbs[0].to"
                  class="hover:text-foreground transition-colors"
                >
                  {{ breadcrumbs[0]?.label || 'Dashboard' }}
                </NuxtLink>
                <span v-else>
                  {{ breadcrumbs[0]?.label || 'Dashboard' }}
                </span>
                <span
                  v-for="(crumb, index) in breadcrumbs.slice(1)"
                  :key="crumb.to || `${crumb.label}-${index}`"
                  class="inline-flex items-center gap-1"
                >
                  <span class="opacity-60">/</span>
                  <span
                    v-if="index === breadcrumbs.slice(1).length - 1 || !crumb.to"
                    class="text-muted-foreground"
                  >
                    {{ crumb.label }}
                  </span>
                  <NuxtLink
                    v-else
                    :to="crumb.to"
                    class="hover:text-foreground transition-colors"
                  >
                    {{ crumb.label }}
                  </NuxtLink>
                </span>
              </template>
            </div>
            <h1 class="text-lg font-semibold text-foreground truncate">{{ pageTitle }}</h1>
            <p v-if="subtitle" class="text-xs text-muted-foreground truncate">{{ subtitle }}</p>
          </div>
        </div>

        <!-- Right: Search + Actions -->
        <div class="flex items-center gap-2 flex-shrink-0">
          <button
            @click="openCommandPalette"
            class="hidden md:flex items-center gap-2 px-3 py-2 bg-background border border-border rounded-lg text-sm text-muted-foreground hover:border-primary transition-colors w-56 lg:w-80"
          >
            <MaterialSymbol name="search" :size="18" />
            <span class="flex-1 text-left">Search...</span>
            <kbd class="hidden lg:inline-flex items-center gap-1 px-2 py-0.5 text-xs font-semibold text-muted-foreground bg-muted rounded border border-border">
              <MaterialSymbol name="keyboard_command_key" :size="12" />K
            </kbd>
          </button>

          <!-- Offline Indicator (compact) -->
          <div class="relative" :title="isOnline ? 'Online' : 'Offline'">
            <button
              type="button"
              class="relative p-2 rounded-lg hover:bg-accent transition-colors"
              :class="isOnline ? 'text-muted-foreground' : 'text-orange-600'"
              aria-label="Network status"
            >
              <MaterialSymbol v-if="isOnline" name="wifi" :size="18" />
              <MaterialSymbol v-else name="wifi_off" :size="18" />
              <span
                v-if="!isOnline && pendingCount > 0"
                class="absolute -top-0.5 -right-0.5 min-w-4 h-4 px-1 rounded-full bg-orange-600 text-white text-[10px] leading-4 text-center"
              >
                {{ pendingCount }}
              </span>
            </button>
          </div>

          <!-- Search Button (Mobile) -->
          <button
            @click="openCommandPalette"
            class="p-2 rounded-lg hover:bg-accent transition-colors md:hidden"
            aria-label="Search"
          >
            <MaterialSymbol name="search" :size="20" />
          </button>

          <!-- Notifications -->
          <div class="relative">
            <button
              @click="toggleNotifications"
              class="relative p-2 rounded-lg hover:bg-accent transition-colors"
              aria-label="Notifications"
            >
              <MaterialSymbol name="notifications" :size="20" />
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
                <MaterialSymbol name="account_circle" :size="18" />
              </div>
              <MaterialSymbol name="expand_more" :size="16" class="hidden sm:block" />
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
            <MaterialSymbol
              name="search"
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
    <CommandPalette
      :open="showCommandPalette"
      :commands="commandPaletteCommands"
      @update:open="(value) => (showCommandPalette = value)"
    />
  </nav>
</template>
