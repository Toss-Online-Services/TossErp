<script setup lang="ts">
// @ts-nocheck
import { ref, computed } from 'vue'
import { useNetwork } from '@vueuse/core'
import { useAuth } from '../composables/useAuth'
import GlobalAiAssistant from '../components/ai/GlobalAiAssistant.vue'

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
const { isOnline } = useNetwork()

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
  <div class="min-h-screen min-vh-100 bg-gray-100 text-foreground g-sidenav-show">
    <div class="flex min-h-screen overflow-hidden bg-gray-100">
      <!-- Sidebar -->
      <MaterialSidebar
        v-model:open="sidebarOpen"
        :role="role"
        :user-info="userInfo"
      />

      <!-- Main Content Area -->
      <div
        :class="[
          'main-content relative flex-1 flex flex-col overflow-hidden transition-all duration-300',
          sidebarOpen ? 'lg:ml-[272px]' : 'lg:ml-0'
        ]"
      >
        <!-- Top Navigation -->
        <MaterialTopNav
          :title="pageTitle"
          :subtitle="pageSubtitle"
          :notification-count="notificationCount"
          @toggle-sidebar="sidebarOpen = !sidebarOpen"
          @search="handleSearch"
        />

        <!-- Page Content -->
        <main class="flex-1 overflow-y-auto px-3 pb-10 bg-gray-50">
          <div class="container-fluid py-4">
            <slot />
          </div>
        </main>

        <!-- Footer -->
        <footer class="mx-3 mb-4 border-t border-border bg-card/90 backdrop-blur border-radius-lg shadow-sm">
          <div class="flex flex-col md:flex-row items-center justify-between px-4 py-4 space-y-2 md:space-y-0">
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

    <Transition
      enter-active-class="transition-all duration-300 ease-out"
      enter-from-class="translate-y-2 opacity-0"
      enter-to-class="translate-y-0 opacity-100"
      leave-active-class="transition-all duration-200 ease-in"
      leave-from-class="translate-y-0 opacity-100"
      leave-to-class="translate-y-2 opacity-0"
    >
      <div
        v-if="!isOnline"
        class="fixed bottom-6 right-6 bg-red-500 text-white px-4 py-3 rounded-lg shadow-material-lg flex items-center gap-2"
      >
        <span class="material-symbols-rounded text-xl">cloud_off</span>
        <span class="font-medium">You are offline</span>
      </div>
    </Transition>

    <GlobalAiAssistant />
  </div>
</template>
