<template>
  <div class="bg-white shadow rounded-lg p-6">
    <!-- Header -->
    <div class="flex items-center justify-between mb-6">
      <div>
        <h3 class="text-lg font-medium text-gray-900">Analytics Dashboard</h3>
        <p class="text-sm text-gray-500">Real-time business insights and performance metrics</p>
      </div>
      <div class="flex space-x-2">
        <select v-model="selectedPeriod" class="px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-orange-500">
          <option value="7d">Last 7 days</option>
          <option value="30d">Last 30 days</option>
          <option value="90d">Last 90 days</option>
          <option value="1y">Last year</option>
        </select>
        <button @click="refreshAnalytics" class="px-4 py-2 bg-orange-600 text-white rounded-md hover:bg-orange-700 focus:outline-none focus:ring-2 focus:ring-orange-500">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
          </svg>
        </button>
      </div>
    </div>

    <!-- Key Metrics Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      <div class="bg-gradient-to-r from-blue-500 to-blue-600 rounded-lg p-4 text-white">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm opacity-90">Total Revenue</p>
            <p class="text-2xl font-bold">{{ formatCurrency(analyticsData.totalRevenue) }}</p>
            <p class="text-sm opacity-90">{{ analyticsData.revenueChange >= 0 ? '+' : '' }}{{ analyticsData.revenueChange }}% vs last period</p>
          </div>
          <div class="w-12 h-12 bg-white/20 rounded-lg flex items-center justify-center">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
        </div>
      </div>

      <div class="bg-gradient-to-r from-green-500 to-green-600 rounded-lg p-4 text-white">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm opacity-90">Total Orders</p>
            <p class="text-2xl font-bold">{{ analyticsData.totalOrders.toLocaleString() }}</p>
            <p class="text-sm opacity-90">{{ analyticsData.ordersChange >= 0 ? '+' : '' }}{{ analyticsData.ordersChange }}% vs last period</p>
          </div>
          <div class="w-12 h-12 bg-white/20 rounded-lg flex items-center justify-center">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
            </svg>
          </div>
        </div>
      </div>

      <div class="bg-gradient-to-r from-purple-500 to-purple-600 rounded-lg p-4 text-white">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm opacity-90">Average Order Value</p>
            <p class="text-2xl font-bold">{{ formatCurrency(analyticsData.averageOrderValue) }}</p>
            <p class="text-sm opacity-90">{{ analyticsData.aovChange >= 0 ? '+' : '' }}{{ analyticsData.aovChange }}% vs last period</p>
          </div>
          <div class="w-12 h-12 bg-white/20 rounded-lg flex items-center justify-center">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
            </svg>
          </div>
        </div>
      </div>

      <div class="bg-gradient-to-r from-orange-500 to-orange-600 rounded-lg p-4 text-white">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm opacity-90">Conversion Rate</p>
            <p class="text-2xl font-bold">{{ analyticsData.conversionRate }}%</p>
            <p class="text-sm opacity-90">{{ analyticsData.conversionChange >= 0 ? '+' : '' }}{{ analyticsData.conversionChange }}% vs last period</p>
          </div>
          <div class="w-12 h-12 bg-white/20 rounded-lg flex items-center justify-center">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6" />
            </svg>
          </div>
        </div>
      </div>
    </div>

    <!-- Charts Section -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-8 mb-8">
      <!-- Revenue Trend Chart -->
      <div class="bg-gray-50 rounded-lg p-6">
        <h4 class="text-lg font-medium text-gray-900 mb-4">Revenue Trend</h4>
        <div class="h-64 flex items-center justify-center">
          <div class="text-center">
            <svg class="w-16 h-16 text-gray-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
            </svg>
            <p class="text-gray-500">Revenue trend chart will be implemented</p>
          </div>
        </div>
      </div>

      <!-- Orders Trend Chart -->
      <div class="bg-gray-50 rounded-lg p-6">
        <h4 class="text-lg font-medium text-gray-900 mb-4">Orders Trend</h4>
        <div class="h-64 flex items-center justify-center">
          <div class="text-center">
            <svg class="w-16 h-16 text-gray-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
            </svg>
            <p class="text-gray-500">Orders trend chart will be implemented</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Top Products and Categories -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-8 mb-8">
      <!-- Top Products -->
      <div class="bg-white border border-gray-200 rounded-lg">
        <div class="px-6 py-4 border-b border-gray-200">
          <h4 class="text-lg font-medium text-gray-900">Top Products</h4>
        </div>
        <div class="p-6">
          <div class="space-y-4">
            <div v-for="product in analyticsData.topProducts" :key="product.id" class="flex items-center justify-between">
              <div class="flex items-center">
                <div class="w-10 h-10 bg-gray-200 rounded-lg flex items-center justify-center mr-3">
                  <svg class="w-5 h-5 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
                  </svg>
                </div>
                <div>
                  <p class="text-sm font-medium text-gray-900">{{ product.name }}</p>
                  <p class="text-xs text-gray-500">{{ product.category }}</p>
                </div>
              </div>
              <div class="text-right">
                <p class="text-sm font-medium text-gray-900">{{ formatCurrency(product.revenue) }}</p>
                <p class="text-xs text-gray-500">{{ product.orders }} orders</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Top Categories -->
      <div class="bg-white border border-gray-200 rounded-lg">
        <div class="px-6 py-4 border-b border-gray-200">
          <h4 class="text-lg font-medium text-gray-900">Top Categories</h4>
        </div>
        <div class="p-6">
          <div class="space-y-4">
            <div v-for="category in analyticsData.topCategories" :key="category.id" class="flex items-center justify-between">
              <div class="flex items-center">
                <div class="w-10 h-10 bg-gray-200 rounded-lg flex items-center justify-center mr-3">
                  <svg class="w-5 h-5 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" />
                  </svg>
                </div>
                <div>
                  <p class="text-sm font-medium text-gray-900">{{ category.name }}</p>
                  <p class="text-xs text-gray-500">{{ category.products }} products</p>
                </div>
              </div>
              <div class="text-right">
                <p class="text-sm font-medium text-gray-900">{{ formatCurrency(category.revenue) }}</p>
                <p class="text-xs text-gray-500">{{ category.orders }} orders</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Customer Insights -->
    <div class="bg-white border border-gray-200 rounded-lg">
      <div class="px-6 py-4 border-b border-gray-200">
        <h4 class="text-lg font-medium text-gray-900">Customer Insights</h4>
      </div>
      <div class="p-6">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div class="text-center">
            <div class="text-3xl font-bold text-blue-600 mb-2">{{ analyticsData.newCustomers.toLocaleString() }}</div>
            <p class="text-sm text-gray-500">New Customers</p>
            <p class="text-xs text-green-600 mt-1">+{{ analyticsData.newCustomersChange }}% vs last period</p>
          </div>
          <div class="text-center">
            <div class="text-3xl font-bold text-green-600 mb-2">{{ analyticsData.repeatCustomers.toLocaleString() }}</div>
            <p class="text-sm text-gray-500">Repeat Customers</p>
            <p class="text-xs text-green-600 mt-1">+{{ analyticsData.repeatCustomersChange }}% vs last period</p>
          </div>
          <div class="text-center">
            <div class="text-3xl font-bold text-purple-600 mb-2">{{ analyticsData.customerLifetimeValue }}</div>
            <p class="text-sm text-gray-500">Avg. Customer Lifetime Value</p>
            <p class="text-xs text-green-600 mt-1">+{{ analyticsData.clvChange }}% vs last period</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'

// Types
interface AnalyticsData {
  totalRevenue: number
  revenueChange: number
  totalOrders: number
  ordersChange: number
  averageOrderValue: number
  aovChange: number
  conversionRate: number
  conversionChange: number
  newCustomers: number
  newCustomersChange: number
  repeatCustomers: number
  repeatCustomersChange: number
  customerLifetimeValue: number
  clvChange: number
  topProducts: Array<{
    id: string
    name: string
    category: string
    revenue: number
    orders: number
  }>
  topCategories: Array<{
    id: string
    name: string
    products: number
    revenue: number
    orders: number
  }>
}

// Reactive data
const selectedPeriod = ref('30d')
const analyticsData = ref<AnalyticsData>({
  totalRevenue: 1250000,
  revenueChange: 15.5,
  totalOrders: 8500,
  ordersChange: 12.3,
  averageOrderValue: 147.06,
  aovChange: 2.8,
  conversionRate: 3.2,
  conversionChange: 0.5,
  newCustomers: 1250,
  newCustomersChange: 8.7,
  repeatCustomers: 3200,
  repeatCustomersChange: 15.2,
  customerLifetimeValue: 450,
  clvChange: 5.3,
  topProducts: [
    { id: '1', name: 'Premium Widget', category: 'Electronics', revenue: 125000, orders: 850 },
    { id: '2', name: 'Smart Device', category: 'Technology', revenue: 98000, orders: 650 },
    { id: '3', name: 'Design Tool', category: 'Software', revenue: 75000, orders: 500 },
    { id: '4', name: 'Office Chair', category: 'Furniture', revenue: 62000, orders: 420 },
    { id: '5', name: 'Coffee Maker', category: 'Appliances', revenue: 48000, orders: 320 }
  ],
  topCategories: [
    { id: '1', name: 'Electronics', products: 45, revenue: 280000, orders: 1850 },
    { id: '2', name: 'Technology', products: 32, revenue: 220000, orders: 1450 },
    { id: '3', name: 'Software', products: 28, revenue: 180000, orders: 1200 },
    { id: '4', name: 'Furniture', products: 38, revenue: 150000, orders: 980 },
    { id: '5', name: 'Appliances', products: 25, revenue: 120000, orders: 820 }
  ]
})

// Methods
const refreshAnalytics = async () => {
  // Simulate API call
  await new Promise(resolve => setTimeout(resolve, 1000))
  
  // Update with new data based on selected period
  analyticsData.value = {
    ...analyticsData.value,
    totalRevenue: Math.floor(Math.random() * 2000000) + 800000,
    totalOrders: Math.floor(Math.random() * 15000) + 5000,
    averageOrderValue: Math.floor(Math.random() * 200) + 100,
    conversionRate: Math.random() * 5 + 1
  }
}

const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

// Watch for period changes
watch(selectedPeriod, () => {
  refreshAnalytics()
})

// Load initial data
onMounted(() => {
  refreshAnalytics()
})
</script>

<style scoped>
/* Component-specific styles */
</style>
