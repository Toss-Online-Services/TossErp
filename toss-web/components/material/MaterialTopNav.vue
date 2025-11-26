<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRoute } from 'vue-router'
import { Search, Bell, Menu, ChevronDown, User } from 'lucide-vue-next'

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
          <form @submit="handleSearch" class="hidden md:block">
            <div class="relative">
              <Search
                :size="18"
                class="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground"
              />
              <input
                v-model="searchQuery"
                type="search"
                placeholder="Search..."
                class="pl-10 pr-4 py-2 bg-background border border-border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary w-64 lg:w-96"
              />
            </div>
          </form>
        </div>

        <!-- Right Section -->
        <div class="flex items-center gap-2">
          <!-- Search Button (Mobile) -->
          <button
            @click="emit('toggle-sidebar')"
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
              <div class="w-8 h-8 rounded-full bg-[#e91e63] text-white flex items-center justify-center shadow-material-button">
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
  </nav>
</template>
