<script setup lang="ts">
import { ref, computed } from 'vue'
import { useAuth } from '~/composables/useAuth'
import GlobalAiAssistant from '~/components/ai/GlobalAiAssistant.vue'

interface Props {
  role?: 'admin' | 'retailer' | 'supplier' | 'driver'
  pageTitle?: string
  pageSubtitle?: string
}

const props = withDefaults(defineProps<Props>(), {
  role: 'retailer'
})

const { user } = useAuth()
const sidebarOpen = ref(true)
const notificationCount = ref(0)

const userInfo = computed(() => {
  if (user.value) {
    return {
      name: user.value.name || 'User',
      email: user.value.email || '',
      avatar: user.value.avatar
    }
  }
  return {
    name: 'Guest',
    email: 'guest@toss.co.za'
  }
})

const handleSearch = (query: string) => {
  console.log('Search:', query)
  // Implement search functionality
}
</script>

<template>
  <div class="min-h-screen bg-background text-foreground">
    <div class="flex h-screen overflow-hidden">
      <!-- Sidebar -->
      <MaterialSidebar
        v-model:open="sidebarOpen"
        :role="role"
        :user-info="userInfo"
      />

      <!-- Main Content Area -->
      <div class="flex-1 flex flex-col overflow-hidden lg:ml-[272px]">
        <!-- Top Navigation -->
        <MaterialTopNav
          :title="pageTitle"
          :subtitle="pageSubtitle"
          :notification-count="notificationCount"
          :user-info="userInfo"
          @toggle-sidebar="sidebarOpen = !sidebarOpen"
          @search="handleSearch"
        />

        <!-- Page Content -->
        <main class="flex-1 overflow-y-auto p-4 lg:p-6 bg-background">
          <slot />
        </main>

        <!-- Footer -->
        <footer class="border-t border-border bg-card py-4 px-4 lg:px-6">
          <div class="flex flex-col md:flex-row items-center justify-between space-y-2 md:space-y-0">
            <div class="text-sm text-muted-foreground">
              <p>&copy; {{ new Date().getFullYear() }} TOSS Online Services. Built for South African SMMEs.</p>
            </div>
            <div class="flex items-center space-x-6 text-sm">
              <NuxtLink
                to="/help"
                class="text-muted-foreground hover:text-foreground transition-colors"
              >
                Help Center
              </NuxtLink>
              <NuxtLink
                to="/privacy"
                class="text-muted-foreground hover:text-foreground transition-colors"
              >
                Privacy
              </NuxtLink>
              <NuxtLink
                to="/terms"
                class="text-muted-foreground hover:text-foreground transition-colors"
              >
                Terms
              </NuxtLink>
            </div>
          </div>
        </footer>
      </div>
    </div>

    <GlobalAiAssistant />
  </div>
</template>
