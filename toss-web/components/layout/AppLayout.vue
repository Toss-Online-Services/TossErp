<template>
  <div class="min-h-screen bg-background">
    <!-- Top Navigation -->
    <header class="border-b bg-white dark:bg-gray-900 sticky top-0 z-50">
      <div class="flex h-16 items-center px-4">
        <Button
          variant="ghost"
          class="lg:hidden"
          @click="toggleSidebar"
        >
          <Menu class="h-6 w-6" />
        </Button>

        <!-- Logo -->
        <div class="flex items-center space-x-4">
          <div class="flex items-center space-x-2">
            <div class="w-8 h-8 bg-primary rounded-lg flex items-center justify-center">
              <span class="text-white font-bold text-sm">T</span>
            </div>
            <div class="hidden sm:block">
              <h1 class="text-xl font-bold">TOSS ERP</h1>
              <p class="text-xs text-muted-foreground">{{ currentLabel }}</p>
            </div>
          </div>
        </div>

        <div class="flex-1" />

        <!-- Search -->
        <div class="hidden md:flex items-center space-x-4">
          <div class="relative">
            <Search class="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search anything..."
              class="pl-10 pr-4 py-2 w-64 rounded-md border border-input bg-background text-sm"
              @keyup.enter="performSearch"
            />
          </div>
        </div>

        <!-- Right side actions -->
        <div class="flex items-center space-x-4 ml-4">
          <!-- Notifications -->
          <Button variant="ghost" size="sm" class="relative">
            <Bell class="h-5 w-5" />
            <div v-if="notifications.length > 0" class="absolute -top-1 -right-1 w-5 h-5 bg-red-500 rounded-full flex items-center justify-center">
              <span class="text-xs text-white">{{ notifications.length }}</span>
            </div>
          </Button>

          <!-- AI Assistant Toggle -->
          <Button 
            variant="ghost" 
            size="sm"
            @click="toggleAIAssistant"
            :class="{ 'bg-primary text-primary-foreground': aiAssistantOpen }"
          >
            <Bot class="h-5 w-5" />
          </Button>

          <!-- Theme Toggle -->
          <Button variant="ghost" size="sm" @click="toggleTheme">
            <Sun v-if="isDark" class="h-5 w-5" />
            <Moon v-else class="h-5 w-5" />
          </Button>

          <!-- User Menu -->
          <div class="relative">
            <Button variant="ghost" class="flex items-center space-x-2" @click="showUserMenu = !showUserMenu">
              <div class="w-8 h-8 bg-primary rounded-full flex items-center justify-center">
                <span class="text-white text-sm">{{ userInitials }}</span>
              </div>
              <ChevronDown class="h-4 w-4" />
            </Button>
            
            <!-- User Dropdown -->
            <div v-if="showUserMenu" class="absolute right-0 mt-2 w-48 bg-white dark:bg-gray-800 rounded-md shadow-lg border">
              <div class="p-3 border-b">
                <p class="font-medium">{{ user.name }}</p>
                <p class="text-sm text-muted-foreground">{{ user.email }}</p>
              </div>
              <nav class="p-2">
                <NuxtLink to="/profile" class="flex items-center space-x-2 p-2 rounded hover:bg-muted">
                  <User class="h-4 w-4" />
                  <span>Profile</span>
                </NuxtLink>
                <NuxtLink to="/settings" class="flex items-center space-x-2 p-2 rounded hover:bg-muted">
                  <Settings class="h-4 w-4" />
                  <span>Settings</span>
                </NuxtLink>
                <hr class="my-2" />
                <button class="flex items-center space-x-2 p-2 rounded hover:bg-muted w-full text-left">
                  <LogOut class="h-4 w-4" />
                  <span>Sign out</span>
                </button>
              </nav>
            </div>
          </div>
        </div>
      </div>
    </header>

    <div class="flex">
      <!-- Sidebar -->
      <aside 
        class="fixed inset-y-0 left-0 z-40 w-64 bg-white dark:bg-gray-900 border-r transform transition-transform lg:translate-x-0"
        :class="{ '-translate-x-full': !sidebarOpen, 'translate-x-0': sidebarOpen }"
      >
        <div class="flex flex-col h-full pt-16">
          <!-- Main Navigation -->
          <nav class="flex-1 space-y-1 p-4">
            <div v-for="section in primarySections" :key="section.title" class="space-y-2">
              <div class="flex items-center space-x-2 p-2 text-sm font-medium text-muted-foreground">
                <span>{{ section.title }}</span>
              </div>
              <div class="space-y-1">
                <div v-for="item in section.items" :key="navKey([section.title], item.label)">
                  <div v-if="item.children?.length" class="space-y-1">
                    <button
                      type="button"
                      class="flex items-center justify-between w-full px-3 py-2 text-sm font-medium transition rounded-lg"
                      :class="isNodeActive(item)
                        ? 'bg-primary/10 text-foreground'
                        : 'text-muted-foreground hover:bg-muted hover:text-foreground'"
                      @click="toggleGroup(navKey([section.title], item.label))"
                    >
                      <div class="flex items-center gap-3">
                        <Icon v-if="item.icon" :name="item.icon" class="w-4 h-4" />
                        <span>{{ item.label }}</span>
                      </div>
                      <ChevronDown
                        class="w-4 h-4 transition-transform"
                        :class="isGroupExpanded(navKey([section.title], item.label)) ? 'rotate-180' : ''"
                      />
                    </button>
                    <div
                      v-if="isGroupExpanded(navKey([section.title], item.label))"
                      class="ml-6 space-y-1"
                    >
                      <NuxtLink
                        v-for="child in item.children"
                        :key="navKey([section.title, item.label], child.label)"
                        :to="child.to"
                        class="flex items-center space-x-3 p-2 rounded-lg text-sm transition-colors"
                        :class="isPathActive(activePath, child)
                          ? 'bg-primary text-primary-foreground'
                          : 'hover:bg-muted'"
                      >
                        <Icon v-if="child.icon" :name="child.icon" class="h-4 w-4" />
                        <span>{{ child.label }}</span>
                      </NuxtLink>
                    </div>
                  </div>
                  <NuxtLink
                    v-else-if="item.to"
                    :to="item.to"
                    class="flex items-center space-x-3 p-3 rounded-lg transition-colors"
                    :class="isPathActive(activePath, item)
                      ? 'bg-primary text-primary-foreground'
                      : 'hover:bg-muted'"
                  >
                    <Icon v-if="item.icon" :name="item.icon" class="w-5 h-5" />
                    <span>{{ item.label }}</span>
                  </NuxtLink>
                </div>
              </div>
            </div>
          </nav>

          <!-- Bottom Section -->
          <div class="border-t p-4">
            <div class="flex items-center space-x-3 p-3 bg-muted rounded-lg">
              <div class="w-8 h-8 bg-gradient-to-r from-blue-500 to-purple-600 rounded-full flex items-center justify-center">
                <Sparkles class="h-4 w-4 text-white" />
              </div>
              <div class="flex-1 min-w-0">
                <p class="text-sm font-medium">AI Copilot Active</p>
                <p class="text-xs text-muted-foreground">Ready to assist</p>
              </div>
            </div>
          </div>
        </div>
      </aside>

      <!-- Main Content -->
      <main class="flex-1 lg:ml-64 min-h-screen">
        <div class="p-6">
          <slot />
        </div>
      </main>
    </div>

    <!-- Mobile Sidebar Overlay -->
    <div 
      v-if="sidebarOpen" 
      class="fixed inset-0 z-30 bg-black bg-opacity-50 lg:hidden"
      @click="sidebarOpen = false"
    ></div>

    <!-- AI Assistant Sidebar -->
    <div 
      v-if="aiAssistantOpen"
      class="fixed right-0 top-0 h-full w-80 bg-white dark:bg-gray-900 border-l shadow-lg z-50 transform transition-transform"
    >
      <div class="flex items-center justify-between p-4 border-b">
        <h3 class="font-medium">AI Assistant</h3>
        <Button variant="ghost" size="sm" @click="aiAssistantOpen = false">
          <X class="h-4 w-4" />
        </Button>
      </div>
      <div class="p-4">
        <p class="text-sm text-muted-foreground mb-4">
          How can I help you today? Try asking about your business performance, inventory levels, or customer insights.
        </p>
        <!-- AI Chat Interface would go here -->
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute } from 'vue-router'
import { 
  Menu, 
  Search, 
  Bell, 
  Bot,
  Sun,
  Moon,
  ChevronDown,
  User,
  Settings,
  LogOut,
  X
} from 'lucide-vue-next'

import {
  findNavItemByPath,
  isPathActive,
  navigation,
  type NavNode,
} from '../../lib/navigation'

import { Button } from '../ui/button'
import { Badge } from '../ui/badge'

// State
const { t } = useI18n()
const route = useRoute()
const sidebarOpen = ref(false)
const showUserMenu = ref(false)
const aiAssistantOpen = ref(false)
const searchQuery = ref('')
const isDark = ref(false)
const expandedGroups = ref(new Set<string>())

// Navigation state
const primarySections = navigation

const normalizePath = (path: string) => {
  if (!path) {
    return '/'
  }

  if (path.length > 1 && path.endsWith('/')) {
    return path.slice(0, -1)
  }

  return path
}

const activePath = computed(() => normalizePath(route.path))

const navKey = (parents: string[], label: string) => [...parents, label].join('>')

const syncExpandedGroups = () => {
  const match = findNavItemByPath(activePath.value)
  if (!match || !match.parents.length || !match.parents[0]) {
    expandedGroups.value = new Set()
    return
  }

  const keys = new Set<string>()
  let lineage: string[] = [match.parents[0]]
  for (const parentLabel of match.parents.slice(1)) {
    if (parentLabel) {
      const key = navKey(lineage, parentLabel)
      keys.add(key)
      lineage = [...lineage, parentLabel]
    }
  }

  expandedGroups.value = keys
}

const toggleGroup = (key: string) => {
  const next = new Set(expandedGroups.value)
  if (next.has(key)) {
    next.delete(key)
  } else {
    next.add(key)
  }
  expandedGroups.value = next
}

const isGroupExpanded = (key: string) => expandedGroups.value.has(key)

const isNodeActive = (item: NavNode): boolean => {
  if (isPathActive(activePath.value, item)) {
    return true
  }

  return item.children?.some(child => isNodeActive(child)) ?? false
}

// Mock data
const user = {
  name: 'Mthunzi Mthembu',
  email: 'mthunzi@spaza.shop',
  avatar: null
}

const notifications = ref([
  { id: 1, message: 'Low stock alert: White bread' },
  { id: 2, message: 'New customer registered' }
])

// Computed
const userInitials = computed(() => {
  return user.name.split(' ').map(n => n[0]).join('')
})

// Methods
const toggleSidebar = () => {
  sidebarOpen.value = !sidebarOpen.value
}

const toggleAIAssistant = () => {
  aiAssistantOpen.value = !aiAssistantOpen.value
}

const toggleTheme = () => {
  isDark.value = !isDark.value
  // Implementation for theme switching
}

const performSearch = () => {
  console.log('Searching for:', searchQuery.value)
  // Implementation for search
}

// Watch for route changes
watch(
  () => route.fullPath,
  () => {
    sidebarOpen.value = false
    syncExpandedGroups()
  },
)

onMounted(() => {
  syncExpandedGroups()
})

const currentLabel = computed(() => findNavItemByPath(activePath.value)?.label ?? 'Dashboard')
</script>