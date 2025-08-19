<template>
  <div class="min-h-screen bg-gray-50 flex">
    <!-- Soft UI Sidebar -->
    <aside 
      :class="[
        'fixed inset-y-0 left-0 z-50 w-64 bg-white shadow-lg transform transition-transform duration-300 ease-in-out',
        sidebarOpen ? 'translate-x-0' : '-translate-x-full'
      ]"
      class="lg:translate-x-0 lg:static lg:inset-0"
    >
      <!-- Sidebar Header -->
      <div class="flex items-center justify-between h-16 px-6 border-b border-gray-200">
        <div class="flex items-center">
          <div class="h-8 w-8 bg-gradient-to-br from-orange-400 to-orange-600 rounded-lg flex items-center justify-center mr-3">
            <span class="text-white font-bold text-sm">T</span>
          </div>
          <h1 class="text-xl font-bold text-gray-900">TOSS ERP</h1>
        </div>
        <button
          @click="toggleSidebar"
          class="lg:hidden p-1 rounded-md text-gray-500 hover:text-gray-700 hover:bg-gray-100"
        >
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>

      <!-- Navigation Menu -->
      <nav class="flex-1 px-4 py-6 space-y-2 overflow-y-auto">
        <!-- Dashboards Section -->
        <div class="mb-6">
          <h3 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-3 px-3">
            Dashboards
          </h3>
          <div class="space-y-1">
            <NuxtLink
              to="/dashboard"
              class="flex items-center px-3 py-2 text-sm font-medium rounded-lg transition-colors"
              :class="[
                $route.path === '/dashboard' 
                  ? 'bg-orange-50 text-orange-600 border-r-2 border-orange-600' 
                  : 'text-gray-700 hover:text-orange-600 hover:bg-orange-50'
              ]"
            >
              <div class="w-5 h-5 mr-3 rounded flex items-center justify-center"
                   :class="$route.path === '/dashboard' ? 'bg-orange-600' : 'bg-gray-300'">
                <svg class="w-3 h-3 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2H5a2 2 0 00-2-2z" />
                </svg>
              </div>
              Default
            </NuxtLink>
            <NuxtLink
              to="/sales"
              class="flex items-center px-3 py-2 text-sm font-medium rounded-lg transition-colors"
              :class="[
                $route.path === '/sales' 
                  ? 'bg-orange-50 text-orange-600 border-r-2 border-orange-600' 
                  : 'text-gray-700 hover:text-orange-600 hover:bg-orange-50'
              ]"
            >
              <div class="w-5 h-5 mr-3 rounded flex items-center justify-center"
                   :class="$route.path === '/sales' ? 'bg-orange-600' : 'bg-gray-300'">
                <svg class="w-3 h-3 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
                </svg>
              </div>
              Sales Dashboard
            </NuxtLink>
          </div>
        </div>

        <!-- PAGES Section -->
        <div class="mb-6">
          <h3 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-3 px-3">
            PAGES
          </h3>
          <div class="space-y-1">
            <NuxtLink
              to="/stock"
              class="flex items-center px-3 py-2 text-sm font-medium rounded-lg transition-colors"
              :class="[
                $route.path === '/stock' 
                  ? 'bg-orange-50 text-orange-600 border-r-2 border-orange-600' 
                  : 'text-gray-700 hover:text-orange-600 hover:bg-orange-50'
              ]"
            >
              <div class="w-5 h-5 mr-3 rounded flex items-center justify-center"
                   :class="$route.path === '/stock' ? 'bg-orange-600' : 'bg-gray-300'">
                <svg class="w-3 h-3 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
                </svg>
              </div>
              Stock Management
            </NuxtLink>
            <NuxtLink
              to="/finance"
              class="flex items-center px-3 py-2 text-sm font-medium rounded-lg transition-colors"
              :class="[
                $route.path === '/finance' 
                  ? 'bg-orange-50 text-orange-600 border-r-2 border-orange-600' 
                  : 'text-gray-700 hover:text-orange-600 hover:bg-orange-50'
              ]"
            >
              <div class="w-5 h-5 mr-3 rounded flex items-center justify-center"
                   :class="$route.path === '/finance' ? 'bg-orange-600' : 'bg-gray-300'">
                <svg class="w-3 h-3 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </div>
              Finance
            </NuxtLink>
            <NuxtLink
              to="/collaboration"
              class="flex items-center px-3 py-2 text-sm font-medium rounded-lg transition-colors"
              :class="[
                $route.path === '/collaboration' 
                  ? 'bg-orange-50 text-orange-600 border-r-2 border-orange-600' 
                  : 'text-gray-700 hover:text-orange-600 hover:bg-orange-50'
              ]"
            >
              <div class="w-5 h-5 mr-3 rounded flex items-center justify-center"
                   :class="$route.path === '/collaboration' ? 'bg-orange-600' : 'bg-gray-300'">
                <svg class="w-3 h-3 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                </svg>
              </div>
              Collaboration
            </NuxtLink>
            <NuxtLink
              to="/reports"
              class="flex items-center px-3 py-2 text-sm font-medium rounded-lg transition-colors"
              :class="[
                $route.path === '/reports' 
                  ? 'bg-orange-50 text-orange-600 border-r-2 border-orange-600' 
                  : 'text-gray-700 hover:text-orange-600 hover:bg-orange-50'
              ]"
            >
              <div class="w-5 h-5 mr-3 rounded flex items-center justify-center"
                   :class="$route.path === '/reports' ? 'bg-orange-600' : 'bg-gray-300'">
                <svg class="w-3 h-3 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                </svg>
              </div>
              Reports
            </NuxtLink>
          </div>
        </div>

        <!-- Account Section -->
        <div class="mb-6">
          <h3 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-3 px-3">
            Account
          </h3>
          <div class="space-y-1">
            <NuxtLink
              to="/profile"
              class="flex items-center px-3 py-2 text-sm font-medium rounded-lg transition-colors"
              :class="[
                $route.path === '/profile' 
                  ? 'bg-orange-50 text-orange-600 border-r-2 border-orange-600' 
                  : 'text-gray-700 hover:text-orange-600 hover:bg-orange-50'
              ]"
            >
              <div class="w-5 h-5 mr-3 rounded flex items-center justify-center"
                   :class="$route.path === '/profile' ? 'bg-orange-600' : 'bg-gray-300'">
                <svg class="w-3 h-3 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                </svg>
              </div>
              Profile
            </NuxtLink>
            <NuxtLink
              to="/settings"
              class="flex items-center px-3 py-2 text-sm font-medium rounded-lg transition-colors"
              :class="[
                $route.path === '/settings' 
                  ? 'bg-orange-50 text-orange-600 border-r-2 border-orange-600' 
                  : 'text-gray-700 hover:text-orange-600 hover:bg-orange-50'
              ]"
            >
              <div class="w-5 h-5 mr-3 rounded flex items-center justify-center"
                   :class="$route.path === '/settings' ? 'bg-orange-600' : 'bg-gray-300'">
                <svg class="w-3 h-3 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                </svg>
              </div>
              Settings
            </NuxtLink>
          </div>
        </div>
      </nav>
    </aside>

    <!-- Main Content Area -->
    <div class="flex-1 flex flex-col lg:ml-64">
      <!-- Top Header -->
      <header class="bg-white shadow-sm border-b border-gray-200">
        <div class="flex items-center justify-between h-16 px-4 sm:px-6 lg:px-8">
          <!-- Mobile menu button -->
          <button
            @click="toggleSidebar"
            class="lg:hidden p-2 rounded-md text-gray-500 hover:text-gray-700 hover:bg-gray-100"
          >
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
            </svg>
          </button>

          <!-- Page Title -->
          <div class="flex-1 flex items-center justify-center lg:justify-start">
            <h1 class="text-lg font-semibold text-gray-900">
              {{ pageTitle }}
            </h1>
          </div>

          <!-- Right side actions -->
          <div class="flex items-center space-x-4">
            <!-- Search -->
            <div class="relative">
              <input
                type="text"
                placeholder="Type here..."
                class="w-64 pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-orange-500"
              />
              <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                <svg class="h-5 w-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                </svg>
              </div>
            </div>

            <!-- Sign In Button -->
            <button class="px-4 py-2 bg-orange-600 text-white rounded-lg hover:bg-orange-700 transition-colors">
              Sign In
            </button>
          </div>
        </div>
      </header>

      <!-- Main content -->
      <main class="flex-1 overflow-y-auto bg-gray-50">
        <slot />
      </main>
    </div>

    <!-- Overlay for mobile sidebar -->
    <div
      v-show="sidebarOpen"
      @click="toggleSidebar"
      class="fixed inset-0 bg-black bg-opacity-25 z-40 lg:hidden"
    ></div>

    <!-- Chatbot Component -->
    <Chatbot />
  </div>
</template>

<script setup lang="ts">
// Layout state management
const sidebarOpen = ref(false)

// Route for page title
const route = useRoute()

// Computed page title
const pageTitle = computed(() => {
  const path = route.path
  if (path === '/dashboard') return 'General Dashboard'
  if (path === '/stock') return 'Stock Management'
  if (path === '/sales') return 'Sales Dashboard'
  if (path === '/finance') return 'Financial Management'
  if (path === '/collaboration') return 'Business Collaboration'
  if (path === '/reports') return 'Reports'
  if (path === '/profile') return 'Profile'
  if (path === '/settings') return 'Settings'
  return 'TOSS ERP'
})

// Methods
const toggleSidebar = () => {
  sidebarOpen.value = !sidebarOpen.value
}

// Close sidebar on route change (mobile)
watch(() => route.path, () => {
  if (window.innerWidth < 1024) {
    sidebarOpen.value = false
  }
})
</script>

<style scoped>
/* Smooth transitions */
.transform {
  transition-property: transform;
}

.transition-transform {
  transition-property: transform;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 300ms;
}

.transition-colors {
  transition-property: color, background-color, border-color;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 200ms;
}

/* Custom scrollbar for sidebar */
.overflow-y-auto::-webkit-scrollbar {
  width: 4px;
}

.overflow-y-auto::-webkit-scrollbar-track {
  background: transparent;
}

.overflow-y-auto::-webkit-scrollbar-thumb {
  background: #e5e7eb;
  border-radius: 2px;
}

.overflow-y-auto::-webkit-scrollbar-thumb:hover {
  background: #d1d5db;
}
</style>
