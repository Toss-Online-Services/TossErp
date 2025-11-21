<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <MaterialSidebar v-model="sidebarOpen" role="supplier" :user-info="userInfo" />
    <MaterialTopNav
      :sidebar-open="sidebarOpen"
      @toggle-sidebar="sidebarOpen = !sidebarOpen"
      :notification-count="notificationCount"
      :user-info="userInfo"
      @search="handleSearch"
    />

    <!-- Main Content -->
    <main class="lg:pl-72 pt-16">
      <div class="p-6">
        <slot />
      </div>

      <!-- Footer -->
      <footer class="border-t border-slate-200 dark:border-slate-700 py-6 px-6 mt-12">
        <div class="flex flex-col md:flex-row justify-between items-center gap-4">
          <p class="text-sm text-slate-600 dark:text-slate-400">
            &copy; {{ new Date().getFullYear() }} TOSS. All rights reserved.
          </p>
          <div class="flex gap-6">
            <NuxtLink to="/help" class="text-sm text-slate-600 dark:text-slate-400 hover:text-orange-600 dark:hover:text-orange-400 transition-colors">
              Help
            </NuxtLink>
            <NuxtLink to="/privacy" class="text-sm text-slate-600 dark:text-slate-400 hover:text-orange-600 dark:hover:text-orange-400 transition-colors">
              Privacy
            </NuxtLink>
            <NuxtLink to="/terms" class="text-sm text-slate-600 dark:text-slate-400 hover:text-orange-600 dark:hover:text-orange-400 transition-colors">
              Terms
            </NuxtLink>
          </div>
        </div>
      </footer>
    </main>
  </div>
</template>

<script setup lang="ts">
const userStore = useUserStore()
const sidebarOpen = ref(false)
const notificationCount = ref(5)

const userInfo = computed(() => ({
  name: userStore.fullName || 'Supplier',
  email: userStore.email || 'supplier@business.co.za',
  avatar: userStore.avatar,
  role: 'Supplier'
}))

const handleSearch = (query: string) => {
  console.log('Search:', query)
}
</script>

