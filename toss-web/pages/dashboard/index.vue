<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
                Welcome back, {{ userStore.user?.firstName }}!
              </h1>
              <p class="text-gray-600 dark:text-gray-400">Overview of {{ userStore.user?.businessName || 'your business' }} performance and key metrics</p>
            </div>
            <div class="flex space-x-3">
              <button class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors">
                <span class="inline-block w-5 h-5 mr-2">📊</span>
                Generate Report
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Dashboard Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <!-- Key Metrics Cards -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Revenue This Month</p>
              <p class="text-2xl font-bold text-green-600">R {{ formatCurrency(metrics.revenue) }}</p>
              <p class="text-sm text-green-600 mt-1">↗ +12.5% from last month</p>
            </div>
            <div class="p-3 bg-green-100 dark:bg-green-900 rounded-full">
              <span class="text-2xl">💰</span>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Active Customers</p>
              <p class="text-2xl font-bold text-blue-600">{{ metrics.customers }}</p>
              <p class="text-sm text-blue-600 mt-1">↗ +8.2% from last month</p>
            </div>
            <div class="p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
              <span class="text-2xl">👥</span>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Inventory Value</p>
              <p class="text-2xl font-bold text-purple-600">R {{ formatCurrency(metrics.inventory) }}</p>
              <p class="text-sm text-red-600 mt-1">↘ -3.1% from last month</p>
            </div>
            <div class="p-3 bg-purple-100 dark:bg-purple-900 rounded-full">
              <span class="text-2xl">📦</span>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Group Buy Savings</p>
              <p class="text-2xl font-bold text-orange-600">R {{ formatCurrency(metrics.savings) }}</p>
              <p class="text-sm text-orange-600 mt-1">↗ +24.8% from last month</p>
            </div>
            <div class="p-3 bg-orange-100 dark:bg-orange-900 rounded-full">
              <span class="text-2xl">🤝</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Charts and Quick Actions -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
        <!-- Revenue Chart -->
        <div class="lg:col-span-2">
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
            <div class="p-6 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Revenue Overview</h3>
            </div>
            <div class="p-6">
              <div class="h-64 flex items-center justify-center bg-gray-50 dark:bg-gray-700 rounded-lg">
                <p class="text-gray-500 dark:text-gray-400">Revenue chart would be displayed here</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Quick Actions -->
        <div>
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
            <div class="p-6 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Quick Actions</h3>
            </div>
            <div class="p-6">
              <div class="space-y-4">
                <NuxtLink to="/inventory" class="flex items-center space-x-3 p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
                  <span class="text-2xl">📦</span>
                  <div>
                    <p class="font-medium text-gray-900 dark:text-white">Check Inventory</p>
                    <p class="text-sm text-gray-600 dark:text-gray-400">{{ lowStockItems }} items low on stock</p>
                  </div>
                </NuxtLink>

                <NuxtLink to="/group-buying" class="flex items-center space-x-3 p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
                  <span class="text-2xl">🤝</span>
                  <div>
                    <p class="font-medium text-gray-900 dark:text-white">Join Group Buys</p>
                    <p class="text-sm text-gray-600 dark:text-gray-400">{{ activeGroupBuys }} active opportunities</p>
                  </div>
                </NuxtLink>

                <NuxtLink to="/crm" class="flex items-center space-x-3 p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
                  <span class="text-2xl">👥</span>
                  <div>
                    <p class="font-medium text-gray-900 dark:text-white">Manage Customers</p>
                    <p class="text-sm text-gray-600 dark:text-gray-400">{{ pendingLeads }} leads need follow-up</p>
                  </div>
                </NuxtLink>

                <NuxtLink to="/accounts" class="flex items-center space-x-3 p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
                  <span class="text-2xl">💰</span>
                  <div>
                    <p class="font-medium text-gray-900 dark:text-white">View Financials</p>
                    <p class="text-sm text-gray-600 dark:text-gray-400">{{ overduelnvoices }} overdue invoices</p>
                  </div>
                </NuxtLink>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Activity -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="p-6 border-b border-gray-200 dark:border-gray-700">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Recent Activity</h3>
        </div>
        <div class="p-6">
          <div class="space-y-4">
            <div v-for="activity in recentActivities" :key="activity.id" class="flex items-center space-x-4 p-4 bg-gray-50 dark:bg-gray-700 rounded-lg">
              <div class="w-10 h-10 rounded-full flex items-center justify-center" :class="getActivityColor(activity.type)">
                <span class="text-white text-sm">{{ getActivityIcon(activity.type) }}</span>
              </div>
              <div class="flex-1">
                <p class="font-medium text-gray-900 dark:text-white">{{ activity.description }}</p>
                <p class="text-sm text-gray-600 dark:text-gray-400">{{ formatDate(activity.date) }} • {{ activity.user }}</p>
              </div>
              <div class="text-right">
                <span class="text-sm font-medium text-gray-900 dark:text-white">{{ activity.amount }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Middleware
definePageMeta({
  middleware: 'auth'
})

// Store
const userStore = useUserStore()

// Sample data
const metrics = ref({
  revenue: 125600,
  customers: 847,
  inventory: 89400,
  savings: 12300
})

const lowStockItems = ref(23)
const activeGroupBuys = ref(8)
const pendingLeads = ref(12)
const overduelnvoices = ref(3)

const recentActivities = ref([
  {
    id: 1,
    type: 'sale',
    description: 'New sale completed',
    date: new Date(),
    user: 'John Smith',
    amount: 'R 1,250.00'
  },
  {
    id: 2,
    type: 'groupbuy',
    description: 'Joined group purchase for office supplies',
    date: new Date(Date.now() - 3600000),
    user: 'You',
    amount: 'R 340.00 saved'
  },
  {
    id: 3,
    type: 'inventory',
    description: 'Stock level alert for Maize Meal',
    date: new Date(Date.now() - 7200000),
    user: 'System',
    amount: '15 remaining'
  },
  {
    id: 4,
    type: 'payment',
    description: 'Payment received from ABC Corp',
    date: new Date(Date.now() - 86400000),
    user: 'ABC Corp',
    amount: 'R 8,750.00'
  }
])

// Helper functions
function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('en-ZA', {
    style: 'decimal',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

function formatDate(date: Date): string {
  return new Intl.DateTimeFormat('en-ZA', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date)
}

function getActivityColor(type: string): string {
  switch (type) {
    case 'sale': return 'bg-green-500'
    case 'groupbuy': return 'bg-blue-500'
    case 'inventory': return 'bg-yellow-500'
    case 'payment': return 'bg-purple-500'
    default: return 'bg-gray-500'
  }
}

function getActivityIcon(type: string): string {
  switch (type) {
    case 'sale': return '💰'
    case 'groupbuy': return '🤝'
    case 'inventory': return '📦'
    case 'payment': return '💳'
    default: return '📋'
  }
}

// Page metadata
useHead({
  title: 'Dashboard - TOSS ERP',
  meta: [
    { name: 'description', content: 'Business dashboard with key metrics and insights in TOSS ERP' }
  ]
})
</script>
