<script setup lang="ts">
import { ref } from 'vue'

interface Props {
  role?: 'admin' | 'retailer' | 'supplier' | 'driver'
  pageTitle?: string
  pageSubtitle?: string
  userInfo?: {
    name: string
    email: string
    avatar?: string
  }
}

const props = withDefaults(defineProps<Props>(), {
  role: 'retailer'
})

const sidebarOpen = ref(true)
const notificationCount = ref(5)

const handleSearch = (query: string) => {
  console.log('Search:', query)
  // Implement search functionality
}
</script>

<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Sidebar -->
    <MaterialSidebar
      v-model:open="sidebarOpen"
      :role="role"
      :user-info="userInfo"
    />

    <!-- Main Content Area -->
    <div class="lg:pl-72">
      <!-- Top Navigation -->
      <MaterialTopNav
        :title="pageTitle"
        :subtitle="pageSubtitle"
        :notification-count="notificationCount"
        @toggle-sidebar="sidebarOpen = !sidebarOpen"
        @search="handleSearch"
      />

      <!-- Page Content -->
      <main class="p-4 lg:p-6">
        <slot />
      </main>

      <!-- Footer -->
      <footer class="border-t border-slate-200 dark:border-slate-800 bg-white/50 dark:bg-slate-900/50 backdrop-blur-sm mt-8">
        <div class="max-w-7xl mx-auto px-4 lg:px-6 py-6">
          <div class="flex flex-col md:flex-row items-center justify-between space-y-4 md:space-y-0">
            <div class="text-sm text-slate-600 dark:text-slate-400">
              <p>&copy; {{ new Date().getFullYear() }} TOSS Online Services. Built for South African SMMEs.</p>
            </div>
            <div class="flex items-center space-x-6 text-sm">
              <NuxtLink to="/help" class="text-slate-600 dark:text-slate-400 hover:text-orange-600 dark:hover:text-orange-400 transition-colors">
                Help Center
              </NuxtLink>
              <NuxtLink to="/privacy" class="text-slate-600 dark:text-slate-400 hover:text-orange-600 dark:hover:text-orange-400 transition-colors">
                Privacy
              </NuxtLink>
              <NuxtLink to="/terms" class="text-slate-600 dark:text-slate-400 hover:text-orange-600 dark:hover:text-orange-400 transition-colors">
                Terms
              </NuxtLink>
            </div>
          </div>
        </div>
      </footer>
    </div>
  </div>
</template>
