<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Mobile Sidebar -->
    <USlideover v-model="sidebarOpen" side="left" class="lg:hidden">
      <UCard class="h-full w-64" :ui="{ body: { padding: '' } }">
        <template #header>
          <div class="flex items-center justify-between">
            <NuxtLink to="/" class="flex items-center">
              <UIcon name="i-heroicons-bolt" class="w-8 h-8 text-primary mr-3" />
              <span class="font-bold text-lg">TOSS ERP III</span>
            </NuxtLink>
            <UButton
              icon="i-heroicons-x-mark"
              variant="ghost"
              @click="sidebarOpen = false"
            />
          </div>
        </template>

        <div class="p-4">
          <SidebarNavigation @close="sidebarOpen = false" />
        </div>
      </UCard>
    </USlideover>

    <!-- Desktop Sidebar -->
    <div class="hidden lg:fixed lg:inset-y-0 lg:left-0 lg:z-50 lg:block lg:w-64 lg:overflow-y-auto lg:bg-white lg:dark:bg-gray-800 lg:shadow-lg">
      <div class="flex items-center h-16 px-6 border-b border-gray-200 dark:border-gray-700">
        <NuxtLink to="/" class="flex items-center">
          <UIcon name="i-heroicons-bolt" class="w-8 h-8 text-primary mr-3" />
          <span class="font-bold text-lg">TOSS ERP III</span>
        </NuxtLink>
      </div>
      
      <div class="p-4">
        <SidebarNavigation />
      </div>
    </div>

    <!-- Main Content Area -->
    <div class="lg:ml-64">
      <!-- Top Navigation Bar -->
      <UContainer class="max-w-none">
        <div class="flex items-center justify-between h-16 px-4 bg-white dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700">
          <div class="flex items-center space-x-4">
            <!-- Mobile menu button -->
            <UButton
              icon="i-heroicons-bars-3"
              variant="ghost"
              class="lg:hidden"
              @click="sidebarOpen = true"
            />
            
            <!-- Search Bar -->
            <div class="hidden md:block">
              <UInput
                icon="i-heroicons-magnifying-glass"
                placeholder="Search..."
                class="w-64"
                v-model="searchQuery"
              />
            </div>
          </div>

          <!-- Right side -->
          <div class="flex items-center space-x-3">
            <!-- Language Selector -->
            <USelectMenu
              v-model="selectedLocale"
              :options="locales"
              option-attribute="name"
              value-attribute="code"
              @change="setLocale"
              :ui="{ width: 'w-32' }"
            >
              <template #label>
                <UIcon name="i-heroicons-language" class="w-4 h-4 mr-1" />
                {{ selectedLocale?.name }}
              </template>
            </USelectMenu>

            <!-- Notifications -->
            <UButton
              icon="i-heroicons-bell"
              variant="ghost"
              :badge="notificationCount"
              @click="showNotifications = true"
            />

            <!-- Dark mode toggle -->
            <UButton
              :icon="$colorMode.value === 'dark' ? 'i-heroicons-sun' : 'i-heroicons-moon'"
              variant="ghost"
              @click="$colorMode.preference = $colorMode.value === 'dark' ? 'light' : 'dark'"
            />

            <!-- User Profile -->
            <UDropdown
              :items="userMenuItems"
              :ui="{ item: { disabled: 'cursor-text select-text' } }"
              :popper="{ strategy: 'fixed' }"
            >
              <UAvatar
                src="/avatar-placeholder.jpg"
                alt="User Avatar"
                size="sm"
                class="cursor-pointer"
              />
            </UDropdown>
          </div>
        </div>
      </UContainer>

      <!-- Main Content -->
      <main class="p-6">
        <slot />
      </main>
    </div>

    <!-- Floating AI Co-Pilot -->
    <div class="fixed bottom-6 right-6 z-50">
      <UButton
        icon="i-heroicons-sparkles"
        size="xl"
        :ui="{ rounded: 'rounded-full' }"
        class="h-16 w-16 shadow-lg animate-pulse"
        @click="showCopilot = true"
        :badge="copilotNotifications > 0 ? copilotNotifications : null"
      />
    </div>

    <!-- AI Co-Pilot Chat Modal -->
    <UModal v-model="showCopilot" :ui="{ width: 'sm:max-w-lg' }">
      <UCard>
        <template #header>
          <div class="flex items-center justify-between">
            <div class="flex items-center">
              <UIcon name="i-heroicons-sparkles" class="w-6 h-6 text-primary mr-2" />
              <div>
                <h3 class="text-lg font-semibold">{{ $t('ai.copilot') }}</h3>
                <p class="text-sm text-green-500">Online</p>
              </div>
            </div>
          </div>
        </template>

        <div class="space-y-4 h-80 overflow-y-auto">
          <div class="flex items-start space-x-2">
            <UAvatar icon="i-heroicons-sparkles" size="xs" />
            <div class="bg-gray-100 dark:bg-gray-800 rounded-lg p-3 flex-1">
              <p class="text-sm">Hi! I'm your AI Assistant. I can help you navigate the ERP system, generate insights, and automate tasks. What would you like to do today?</p>
            </div>
          </div>
        </div>

        <template #footer>
          <div class="flex space-x-2">
            <UInput
              v-model="chatInput"
              placeholder="Ask me anything..."
              class="flex-1"
              @keyup.enter="sendMessage"
            />
            <UButton
              icon="i-heroicons-paper-airplane"
              @click="sendMessage"
              :disabled="!chatInput.trim()"
            />
          </div>
        </template>
      </UCard>
    </UModal>

    <!-- Notifications Slideover -->
    <USlideover v-model="showNotifications" side="right">
      <UCard class="h-full">
        <template #header>
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-semibold">Notifications</h3>
            <UButton
              icon="i-heroicons-x-mark"
              variant="ghost"
              @click="showNotifications = false"
            />
          </div>
        </template>

        <div class="space-y-4">
          <UAlert
            icon="i-heroicons-information-circle"
            color="blue"
            variant="soft"
            title="System Update"
            description="Your TOSS ERP III system has been updated to the latest version."
          />
          <UAlert
            icon="i-heroicons-exclamation-triangle"
            color="amber"
            variant="soft"
            title="Low Stock Alert"
            description="5 items are running low in stock."
          />
        </div>
      </UCard>
    </USlideover>
  </div>
</template>

<script setup lang="ts">
// Composables
const { locale, locales, setLocale } = useI18n()
const colorMode = useColorMode()

// State
const sidebarOpen = ref(false)
const showCopilot = ref(false)
const showNotifications = ref(false)
const searchQuery = ref('')
const chatInput = ref('')
const copilotNotifications = ref(3)
const notificationCount = ref(2)

// Computed
const selectedLocale = computed(() => {
  return locales.value.find(l => l.code === locale.value)
})

// User menu items
const userMenuItems = [
  [{
    label: 'Profile',
    icon: 'i-heroicons-user',
    to: '/profile'
  }, {
    label: 'Settings',
    icon: 'i-heroicons-cog-6-tooth',
    to: '/settings'
  }],
  [{
    label: 'Sign out',
    icon: 'i-heroicons-arrow-right-on-rectangle',
    click: () => {
      // Handle sign out
      console.log('Sign out')
    }
  }]
]

// Methods
const sendMessage = () => {
  if (chatInput.value.trim()) {
    // Handle chat message
    console.log('Send message:', chatInput.value)
    chatInput.value = ''
  }
}

// Watch for sidebar state on mobile
watch(sidebarOpen, (newValue) => {
  if (newValue && process.client) {
    document.body.style.overflow = 'hidden'
  } else if (process.client) {
    document.body.style.overflow = 'unset'
  }
})

// Cleanup on unmount
onUnmounted(() => {
  if (process.client) {
    document.body.style.overflow = 'unset'
  }
})
</script>
