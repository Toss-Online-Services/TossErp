<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Navigation -->
    <AppNavigation />
    
    <!-- Main Content -->
    <main class="flex-1">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <slot />
      </div>
    </main>
    
    <!-- AI Copilot Chat -->
    <AICopilotChat />
    
    <!-- Global Notifications -->
    <NotificationContainer />
  </div>
</template>

<script setup lang="ts">
// Layout setup
useHead({
  titleTemplate: '%s - TOSS ERP III',
  meta: [
    { name: 'viewport', content: 'width=device-width, initial-scale=1' },
    { name: 'theme-color', content: '#1d4ed8' },
  ],
})

// Initialize stores
const userStore = useUserStore()
const settingsStore = useSettingsStore()

// Check authentication status
onMounted(async () => {
  await userStore.checkAuth()
  await settingsStore.loadSettings()
})
</script>
