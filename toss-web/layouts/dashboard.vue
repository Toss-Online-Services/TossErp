<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Top Navigation Bar -->
    <nav class="bg-white shadow-sm border-b border-gray-200">
      <div class="mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16">
          <!-- Left side - Logo and title -->
          <div class="flex items-center">
            <button
              @click="sidebarOpen = !sidebarOpen"
              class="p-2 rounded-md text-gray-400 hover:text-gray-500 hover:bg-gray-100 lg:hidden"
            >
              <svg class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
              </svg>
            </button>
            <div class="flex-shrink-0 flex items-center ml-4 lg:ml-0">
              <h1 class="text-xl font-bold text-gray-900">TOSS ERP</h1>
            </div>
          </div>

          <!-- Right side - User menu and notifications -->
          <div class="flex items-center space-x-4">
            <!-- Notifications -->
            <button class="p-2 rounded-full text-gray-400 hover:text-gray-500 hover:bg-gray-100 relative">
              <svg class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
              </svg>
              <span class="absolute top-1 right-1 block h-2 w-2 rounded-full bg-red-500"></span>
            </button>

            <!-- User menu -->
            <div class="relative">
              <button
                @click="userMenuOpen = !userMenuOpen"
                class="flex items-center space-x-3 p-2 rounded-lg hover:bg-gray-100"
              >
                <div class="h-8 w-8 rounded-full bg-blue-500 flex items-center justify-center text-white font-semibold">
                  {{ userInitials }}
                </div>
                <span class="hidden md:block text-sm font-medium text-gray-700">{{ userName }}</span>
                <svg class="h-5 w-5 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
                </svg>
              </button>

              <!-- User dropdown menu -->
              <div
                v-if="userMenuOpen"
                class="absolute right-0 mt-2 w-48 rounded-md shadow-lg bg-white ring-1 ring-black ring-opacity-5 z-50"
              >
                <div class="py-1">
                  <NuxtLink to="/profile" class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100">
                    Your Profile
                  </NuxtLink>
                  <NuxtLink to="/settings" class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100">
                    Settings
                  </NuxtLink>
                  <button
                    @click="handleLogout"
                    class="block w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                  >
                    Sign out
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </nav>

    <div class="flex">
      <!-- Sidebar -->
      <aside
        :class="[
          'fixed inset-y-0 left-0 z-40 w-64 bg-white border-r border-gray-200 transition-transform duration-300 ease-in-out lg:translate-x-0 lg:static',
          sidebarOpen ? 'translate-x-0' : '-translate-x-full'
        ]"
        style="top: 64px"
      >
        <nav class="h-full overflow-y-auto py-4">
          <div v-for="section in navigation" :key="section.name" class="mb-6">
            <h3 class="px-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">
              {{ section.name }}
            </h3>
            <div class="mt-2 space-y-1">
              <NuxtLink
                v-for="item in section.items"
                :key="item.name"
                :to="item.href"
                class="flex items-center px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-100 hover:text-gray-900"
                active-class="bg-blue-50 text-blue-700"
              >
                <component :is="item.icon" class="mr-3 h-5 w-5" />
                {{ item.name }}
              </NuxtLink>
            </div>
          </div>
        </nav>
      </aside>

      <!-- Overlay for mobile -->
      <div
        v-if="sidebarOpen"
        @click="sidebarOpen = false"
        class="fixed inset-0 bg-gray-600 bg-opacity-75 z-30 lg:hidden"
        style="top: 64px"
      ></div>

      <!-- Main content -->
      <main class="flex-1 overflow-y-auto p-6">
        <slot />
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
const { user, logout } = useAuth()
const sidebarOpen = ref(false)
const userMenuOpen = ref(false)

const userName = computed(() => user.value?.name || 'User')
const userInitials = computed(() => {
  const name = user.value?.name || 'U'
  return name
    .split(' ')
    .map(n => n[0])
    .join('')
    .toUpperCase()
    .slice(0, 2)
})

const navigation = [
  {
    name: 'Main',
    items: [
      { name: 'Dashboard', href: '/dashboard', icon: 'i-heroicons-home' },
    ],
  },
  {
    name: 'Sales',
    items: [
      { name: 'Point of Sale', href: '/sales/pos', icon: 'i-heroicons-shopping-cart' },
      { name: 'Orders', href: '/sales/orders', icon: 'i-heroicons-document-text' },
      { name: 'Invoices', href: '/sales/invoices', icon: 'i-heroicons-document-duplicate' },
      { name: 'Customers', href: '/crm/customers', icon: 'i-heroicons-users' },
      { name: 'Analytics', href: '/sales/analytics', icon: 'i-heroicons-chart-bar' },
    ],
  },
  {
    name: 'Inventory',
    items: [
      { name: 'Products', href: '/inventory/items', icon: 'i-heroicons-cube' },
      { name: 'Stock', href: '/inventory', icon: 'i-heroicons-archive-box' },
      { name: 'Warehouses', href: '/inventory/warehouses', icon: 'i-heroicons-building-storefront' },
      { name: 'Movements', href: '/inventory/movements', icon: 'i-heroicons-arrow-path' },
    ],
  },
  {
    name: 'Purchasing',
    items: [
      { name: 'Suppliers', href: '/purchasing/suppliers', icon: 'i-heroicons-truck' },
      { name: 'Purchase Orders', href: '/purchasing/orders', icon: 'i-heroicons-shopping-bag' },
      { name: 'Invoices', href: '/purchasing/invoices', icon: 'i-heroicons-document-text' },
    ],
  },
  {
    name: 'Finance',
    items: [
      { name: 'Accounts', href: '/accounts', icon: 'i-heroicons-banknotes' },
      { name: 'Journal', href: '/accounts/journal', icon: 'i-heroicons-book-open' },
      { name: 'Reports', href: '/finance', icon: 'i-heroicons-chart-pie' },
    ],
  },
  {
    name: 'People',
    items: [
      { name: 'Employees', href: '/hr/employees', icon: 'i-heroicons-user-group' },
      { name: 'Attendance', href: '/hr', icon: 'i-heroicons-clock' },
      { name: 'Leave', href: '/hr/leave', icon: 'i-heroicons-calendar' },
      { name: 'Payroll', href: '/hr/payroll', icon: 'i-heroicons-currency-dollar' },
    ],
  },
  {
    name: 'Collaboration',
    items: [
      { name: 'Group Buying', href: '/group-buying', icon: 'i-heroicons-user-group' },
      { name: 'Logistics', href: '/logistics', icon: 'i-heroicons-truck' },
      { name: 'Community', href: '/collaboration', icon: 'i-heroicons-chat-bubble-left-right' },
    ],
  },
  {
    name: 'System',
    items: [
      { name: 'Reports', href: '/reports', icon: 'i-heroicons-document-chart-bar' },
      { name: 'Settings', href: '/settings', icon: 'i-heroicons-cog-6-tooth' },
    ],
  },
]

const handleLogout = async () => {
  await logout()
}
</script>
