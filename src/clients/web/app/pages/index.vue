<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Hero Section -->
    <div class="bg-gradient-to-r from-blue-600 to-blue-800 dark:from-blue-800 dark:to-blue-900">
      <div class="max-w-7xl mx-auto py-16 px-4 sm:py-24 sm:px-6 lg:px-8">
        <div class="text-center">
          <h1 class="text-4xl font-extrabold text-white sm:text-5xl md:text-6xl">
            <span class="block">Welcome to</span>
            <span class="block text-blue-200">TOSS ERP</span>
          </h1>
          <p class="mt-6 max-w-2xl mx-auto text-xl text-blue-100">
            Township One-Stop Solution - Complete business management for South African SMMEs
          </p>
          <div class="mt-10 flex justify-center space-x-4">
            <NuxtLink
              to="/dashboard"
              class="inline-flex items-center px-6 py-3 border border-transparent text-base font-medium rounded-md text-blue-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors"
            >
              Go to Dashboard
              <svg class="ml-2 -mr-1 w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10.293 3.293a1 1 0 011.414 0l6 6a1 1 0 010 1.414l-6 6a1 1 0 01-1.414-1.414L14.586 11H3a1 1 0 110-2h11.586l-4.293-4.293a1 1 0 010-1.414z" clip-rule="evenodd" />
              </svg>
            </NuxtLink>
            <NuxtLink
              to="/stock"
              class="inline-flex items-center px-6 py-3 border border-transparent text-base font-medium rounded-md text-white bg-blue-700 hover:bg-blue-800 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors"
            >
              Manage Stock
            </NuxtLink>
          </div>
        </div>
      </div>
    </div>

    <!-- Features Section -->
    <div class="py-16 bg-white dark:bg-gray-800">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center">
          <h2 class="text-3xl font-extrabold text-gray-900 dark:text-white sm:text-4xl">
            Complete Business Management
          </h2>
          <p class="mt-4 max-w-2xl mx-auto text-xl text-gray-500 dark:text-gray-300">
            Everything you need to run your township business efficiently
          </p>
        </div>

        <div class="mt-16">
          <div class="grid grid-cols-1 gap-8 sm:grid-cols-2 lg:grid-cols-4">
            <!-- Stock Management -->
            <div class="relative group">
              <div class="p-6 bg-gray-50 dark:bg-gray-700 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-600 transition-colors cursor-pointer" @click="navigateTo('/stock')">
                <div class="w-12 h-12 mx-auto bg-blue-500 rounded-lg flex items-center justify-center mb-4">
                  <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
                  </svg>
                </div>
                <h3 class="text-lg font-medium text-gray-900 dark:text-white text-center">Stock Management</h3>
                <p class="mt-2 text-sm text-gray-500 dark:text-gray-300 text-center">
                  Track inventory, manage suppliers, and optimize stock levels
                </p>
              </div>
            </div>

            <!-- Sales & POS -->
            <div class="relative group">
              <div class="p-6 bg-gray-50 dark:bg-gray-700 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-600 transition-colors cursor-pointer" @click="navigateTo('/sales')">
                <div class="w-12 h-12 mx-auto bg-green-500 rounded-lg flex items-center justify-center mb-4">
                  <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
                <h3 class="text-lg font-medium text-gray-900 dark:text-white text-center">Sales & POS</h3>
                <p class="mt-2 text-sm text-gray-500 dark:text-gray-300 text-center">
                  Process sales, manage customers, and track performance
                </p>
              </div>
            </div>

            <!-- Finance -->
            <div class="relative group">
              <div class="p-6 bg-gray-50 dark:bg-gray-700 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-600 transition-colors cursor-pointer" @click="navigateTo('/finance')">
                <div class="w-12 h-12 mx-auto bg-yellow-500 rounded-lg flex items-center justify-center mb-4">
                  <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                  </svg>
                </div>
                <h3 class="text-lg font-medium text-gray-900 dark:text-white text-center">Financial Management</h3>
                <p class="mt-2 text-sm text-gray-500 dark:text-gray-300 text-center">
                  Track expenses, manage cash flow, and financial reporting
                </p>
              </div>
            </div>

            <!-- Collaboration -->
            <div class="relative group">
              <div class="p-6 bg-gray-50 dark:bg-gray-700 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-600 transition-colors cursor-pointer" @click="navigateTo('/collaboration')">
                <div class="w-12 h-12 mx-auto bg-purple-500 rounded-lg flex items-center justify-center mb-4">
                  <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                  </svg>
                </div>
                <h3 class="text-lg font-medium text-gray-900 dark:text-white text-center">Collaboration</h3>
                <p class="mt-2 text-sm text-gray-500 dark:text-gray-300 text-center">
                  Group buying, shared logistics, and community features
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Quick Stats Section -->
    <div class="py-16 bg-gray-50 dark:bg-gray-900">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center">
          <h2 class="text-3xl font-extrabold text-gray-900 dark:text-white sm:text-4xl">
            Quick Business Overview
          </h2>
        </div>

        <div class="mt-12 grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4">
          <!-- Total Stock Value -->
          <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
            <div class="p-5">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <div class="w-8 h-8 bg-blue-500 rounded-md flex items-center justify-center">
                    <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
                    </svg>
                  </div>
                </div>
                <div class="ml-5 w-0 flex-1">
                  <dl>
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                      Total Stock Value
                    </dt>
                    <dd class="text-lg font-medium text-gray-900 dark:text-white">
                      R {{ formatCurrency(totalStockValue) }}
                    </dd>
                  </dl>
                </div>
              </div>
            </div>
          </div>

          <!-- Today's Sales -->
          <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
            <div class="p-5">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <div class="w-8 h-8 bg-green-500 rounded-md flex items-center justify-center">
                    <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                  </div>
                </div>
                <div class="ml-5 w-0 flex-1">
                  <dl>
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                      Today's Sales
                    </dt>
                    <dd class="text-lg font-medium text-gray-900 dark:text-white">
                      R {{ formatCurrency(todaysSales) }}
                    </dd>
                  </dl>
                </div>
              </div>
            </div>
          </div>

          <!-- Low Stock Items -->
          <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
            <div class="p-5">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <div class="w-8 h-8 bg-yellow-500 rounded-md flex items-center justify-center">
                    <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z" />
                    </svg>
                  </div>
                </div>
                <div class="ml-5 w-0 flex-1">
                  <dl>
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                      Low Stock Alerts
                    </dt>
                    <dd class="text-lg font-medium text-gray-900 dark:text-white">
                      {{ lowStockCount }} items
                    </dd>
                  </dl>
                </div>
              </div>
            </div>
          </div>

          <!-- Active Customers -->
          <div class="bg-white dark:bg-gray-800 overflow-hidden shadow rounded-lg">
            <div class="p-5">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <div class="w-8 h-8 bg-purple-500 rounded-md flex items-center justify-center">
                    <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                    </svg>
                  </div>
                </div>
                <div class="ml-5 w-0 flex-1">
                  <dl>
                    <dt class="text-sm font-medium text-gray-500 dark:text-gray-400 truncate">
                      Active Customers
                    </dt>
                    <dd class="text-lg font-medium text-gray-900 dark:text-white">
                      {{ activeCustomers }}
                    </dd>
                  </dl>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Getting Started Section -->
    <div class="py-16 bg-white dark:bg-gray-800">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center">
          <h2 class="text-3xl font-extrabold text-gray-900 dark:text-white sm:text-4xl">
            Get Started with TOSS ERP
          </h2>
          <p class="mt-4 max-w-2xl mx-auto text-xl text-gray-500 dark:text-gray-300">
            Follow these steps to set up your business management system
          </p>
        </div>

        <div class="mt-12">
          <div class="grid grid-cols-1 gap-8 md:grid-cols-3">
            <!-- Step 1 -->
            <div class="text-center">
              <div class="flex items-center justify-center w-16 h-16 mx-auto bg-blue-100 dark:bg-blue-900 rounded-full mb-4">
                <span class="text-2xl font-bold text-blue-600 dark:text-blue-400">1</span>
              </div>
              <h3 class="text-xl font-medium text-gray-900 dark:text-white mb-2">Set Up Your Inventory</h3>
              <p class="text-gray-500 dark:text-gray-300">
                Add your products, set up categories, and configure your warehouse
              </p>
              <NuxtLink
                to="/stock"
                class="mt-4 inline-flex items-center text-blue-600 dark:text-blue-400 hover:text-blue-800 dark:hover:text-blue-300"
              >
                Start managing stock →
              </NuxtLink>
            </div>

            <!-- Step 2 -->
            <div class="text-center">
              <div class="flex items-center justify-center w-16 h-16 mx-auto bg-green-100 dark:bg-green-900 rounded-full mb-4">
                <span class="text-2xl font-bold text-green-600 dark:text-green-400">2</span>
              </div>
              <h3 class="text-xl font-medium text-gray-900 dark:text-white mb-2">Configure Sales</h3>
              <p class="text-gray-500 dark:text-gray-300">
                Set up your POS system, payment methods, and customer database
              </p>
              <NuxtLink
                to="/sales"
                class="mt-4 inline-flex items-center text-green-600 dark:text-green-400 hover:text-green-800 dark:hover:text-green-300"
              >
                Set up sales →
              </NuxtLink>
            </div>

            <!-- Step 3 -->
            <div class="text-center">
              <div class="flex items-center justify-center w-16 h-16 mx-auto bg-purple-100 dark:bg-purple-900 rounded-full mb-4">
                <span class="text-2xl font-bold text-purple-600 dark:text-purple-400">3</span>
              </div>
              <h3 class="text-xl font-medium text-gray-900 dark:text-white mb-2">Start Collaborating</h3>
              <p class="text-gray-500 dark:text-gray-300">
                Connect with other businesses for group buying and shared logistics
              </p>
              <NuxtLink
                to="/collaboration"
                class="mt-4 inline-flex items-center text-purple-600 dark:text-purple-400 hover:text-purple-800 dark:hover:text-purple-300"
              >
                Explore collaboration →
              </NuxtLink>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Page meta
definePageMeta({
  title: 'TOSS ERP - Township One-Stop Solution',
  description: 'Complete business management solution for township and rural SMMEs'
})

// Mock data - this will be replaced with real API calls
const totalStockValue = ref(45750.00)
const todaysSales = ref(1234.50)
const lowStockCount = ref(7)
const activeCustomers = ref(245)

// Utility function to format currency
const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

// Load dashboard data on mount
onMounted(async () => {
  try {
    // TODO: Replace with actual API calls to gateway
    // const dashboardData = await $fetch('/api/dashboard/summary')
    // totalStockValue.value = dashboardData.totalStockValue
    // todaysSales.value = dashboardData.todaysSales
    // lowStockCount.value = dashboardData.lowStockCount
    // activeCustomers.value = dashboardData.activeCustomers
    
    console.log('Dashboard loaded with mock data')
  } catch (error) {
    console.error('Failed to load dashboard data:', error)
  }
})

// SEO
useHead({
  title: 'TOSS ERP - Township One-Stop Solution',
  meta: [
    { name: 'description', content: 'Complete business management solution for township and rural SMMEs in South Africa' },
    { name: 'keywords', content: 'ERP, stock management, POS, township business, SMME, South Africa, business management' }
  ]
})
</script>

<style scoped>
/* Component-specific animations */
.group:hover .group-hover\:scale-105 {
  transform: scale(1.05);
}

/* Smooth transitions */
.transition-colors {
  transition-property: color, background-color, border-color;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 200ms;
}

/* Hover effects */
.hover\:bg-gray-50:hover {
  background-color: #f9fafb;
}

.dark .hover\:bg-gray-600:hover {
  background-color: #4b5563;
}
</style>

