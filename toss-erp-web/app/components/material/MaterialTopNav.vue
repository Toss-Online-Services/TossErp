<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRoute } from 'vue-router'
import { Search, Bell, Menu, ChevronDown, User, Settings } from 'lucide-vue-next'

interface Props {
  title?: string
  subtitle?: string
  notificationCount?: number
}

const props = withDefaults(defineProps<Props>(), {
  notificationCount: 0
})

const emit = defineEmits<{
  'toggle-sidebar': []
  'search': [query: string]
}>()

const route = useRoute()
const searchQuery = ref('')
const showNotifications = ref(false)
const showUserMenu = ref(false)

const heading = computed(() => props.title || (route.meta?.title as string) || 'Dashboard')

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
</script>

<template>
  <nav class="sticky top-1 z-40 mt-2 mx-3">
    <div class="bg-white/90 backdrop-blur border border-gray-200 shadow-material border-radius-lg px-4 lg:px-6">
      <div class="flex items-center justify-between h-16 gap-4">
        <!-- Left Section -->
        <div class="flex items-center gap-3">
          <button
            @click="emit('toggle-sidebar')"
            class="inline-flex items-center justify-center rounded-lg border border-gray-200 p-2 text-stone-700 hover:bg-gray-100 transition-colors"
            aria-label="Toggle sidebar"
          >
            <div class="flex flex-col gap-0.5">
              <span class="block h-0.5 w-4 rounded bg-stone-700"></span>
              <span class="block h-0.5 w-4 rounded bg-stone-700"></span>
              <span class="block h-0.5 w-4 rounded bg-stone-700"></span>
            </div>
          </button>

          <div class="hidden sm:block">
            <p class="text-[11px] uppercase tracking-wide text-muted-foreground">Pages</p>
            <h1 class="text-lg font-semibold text-foreground">{{ heading }}</h1>
            <p v-if="subtitle" class="text-xs text-muted-foreground">{{ subtitle }}</p>
          </div>
        </div>

        <!-- Search + Actions -->
        <div class="flex items-center gap-2 flex-1 justify-end">
          <form @submit="handleSearch" class="hidden md:block flex-1 max-w-xl">
            <label class="sr-only" for="nav-search">Search</label>
            <div class="relative">
              <Search
                :size="18"
                class="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground"
              />
              <input
                id="nav-search"
                v-model="searchQuery"
                type="search"
                placeholder="Search here"
                class="w-full pl-10 pr-4 py-2 bg-white border border-gray-200 rounded-full text-sm focus:outline-none focus:ring-2 focus:ring-primary shadow-sm"
              />
            </div>
          </form>

          <!-- Mobile search toggle -->
          <button
            @click="emit('toggle-sidebar')"
            class="p-2 rounded-lg hover:bg-gray-100 transition-colors md:hidden"
            aria-label="Toggle sidebar"
          >
            <Menu :size="20" />
          </button>

          <button
            class="hidden md:inline-flex items-center justify-center p-2 rounded-lg hover:bg-gray-100 transition-colors text-stone-600"
            aria-label="Settings"
          >
            <Settings :size="20" />
          </button>

          <!-- Notifications -->
          <div class="relative">
            <button
              @click="toggleNotifications"
              class="relative p-2 rounded-lg hover:bg-gray-100 transition-colors text-stone-700"
              aria-label="Notifications"
            >
              <Bell :size="20" />
              <span
                v-if="notificationCount > 0"
                class="absolute -top-0.5 -right-0.5 w-2 h-2 bg-destructive rounded-full"
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
              class="flex items-center gap-2 px-3 py-2 rounded-lg hover:bg-gray-100 transition-colors"
              aria-label="User menu"
            >
              <div class="w-8 h-8 rounded-full bg-[#e91e63] text-white flex items-center justify-center shadow-material-button">
                <User :size="18" />
              </div>
              <ChevronDown :size="16" class="hidden sm:block text-stone-600" />
            </button>

            <!-- User Dropdown -->
            <div
              v-if="showUserMenu"
              class="absolute right-0 mt-2 w-56 bg-card border border-border rounded-lg shadow-lg z-50"
            >
              <div class="px-4 py-3 border-b border-border">
                <p class="text-sm font-medium">John Doe</p>
                <p class="text-xs text-muted-foreground">john@example.com</p>
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
              placeholder="Search here"
              class="w-full pl-10 pr-4 py-2 bg-white border border-gray-200 rounded-full text-sm focus:outline-none focus:ring-2 focus:ring-primary shadow-sm"
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
  </nav>
</template>
