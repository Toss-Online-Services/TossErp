<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-slate-800 shadow-sm border-b border-slate-200 dark:border-slate-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
        <div class="flex items-center justify-between">
          <div>
            <h1 class="text-3xl font-bold text-slate-900 dark:text-white">Business Dashboard</h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
              {{ formatDate(new Date()) }}
            </p>
          </div>
          <div class="flex items-center space-x-3">
            <ExportButton :data="dashboardData" filename="dashboard-report" />
            <button
              @click="refreshData"
              class="inline-flex items-center px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg text-sm font-medium text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-800 hover:bg-slate-50 dark:hover:bg-slate-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
            >
              <ArrowPathIcon class="w-4 h-4 mr-2" />
              Refresh
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Loading State -->
      <div v-if="loading" class="space-y-6">
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <div v-for="i in 4" :key="i" class="bg-white dark:bg-slate-800 rounded-lg p-6 animate-pulse">
            <div class="h-4 bg-slate-200 dark:bg-slate-700 rounded w-1/2 mb-2"></div>
            <div class="h-8 bg-slate-200 dark:bg-slate-700 rounded w-3/4"></div>
          </div>
        </div>
      </div>

      <!-- Dashboard Content -->
      <div v-else class="space-y-6">
        <!-- Key Metrics -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <!-- Revenue -->
          <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Total Revenue</p>
                <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">
                  {{ formatCurrency(metrics.totalRevenue) }}
                </p>
                <p class="mt-2 text-sm" :class="metrics.revenueChange >= 0 ? 'text-green-600' : 'text-red-600'">
                  <span class="font-medium">
                    {{ metrics.revenueChange >= 0 ? '+' : '' }}{{ metrics.revenueChange.toFixed(1) }}%
                  </span>
                  <span class="text-slate-600 dark:text-slate-400">vs last month</span>
                </p>
              </div>
              <div class="p-3 bg-green-100 dark:bg-green-900/30 rounded-lg">
                <CurrencyDollarIcon class="w-8 h-8 text-green-600 dark:text-green-400" />
              </div>
            </div>
          </div>

          <!-- Orders -->
          <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Total Orders</p>
                <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">{{ metrics.totalOrders }}</p>
                <p class="mt-2 text-sm" :class="metrics.ordersChange >= 0 ? 'text-green-600' : 'text-red-600'">
                  <span class="font-medium">
                    {{ metrics.ordersChange >= 0 ? '+' : '' }}{{ metrics.ordersChange.toFixed(1) }}%
                  </span>
                  <span class="text-slate-600 dark:text-slate-400">vs last month</span>
                </p>
              </div>
              <div class="p-3 bg-blue-100 dark:bg-blue-900/30 rounded-lg">
                <ShoppingCartIcon class="w-8 h-8 text-blue-600 dark:text-blue-400" />
              </div>
            </div>
          </div>

          <!-- Group Buying Savings -->
          <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Group Buy Savings</p>
                <p class="mt-2 text-3xl font-bold text-purple-600 dark:text-purple-400">
                  {{ formatCurrency(metrics.groupBuySavings) }}
                </p>
                <p class="mt-2 text-sm text-slate-600 dark:text-slate-400">
                  <span class="font-medium">{{ metrics.activePoolsCount }} active pools</span>
                </p>
              </div>
              <div class="p-3 bg-purple-100 dark:bg-purple-900/30 rounded-lg">
                <UserGroupIcon class="w-8 h-8 text-purple-600 dark:text-purple-400" />
              </div>
            </div>
          </div>

          <!-- Stock Value -->
          <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Stock Value</p>
                <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">
                  {{ formatCurrency(metrics.stockValue) }}
                </p>
                <p class="mt-2 text-sm text-orange-600 dark:text-orange-400">
                  <span class="font-medium">{{ metrics.lowStockItems }} low stock items</span>
                </p>
              </div>
              <div class="p-3 bg-orange-100 dark:bg-orange-900/30 rounded-lg">
                <CubeIcon class="w-8 h-8 text-orange-600 dark:text-orange-400" />
              </div>
            </div>
          </div>
        </div>

        <!-- Charts Row 1 -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <!-- Revenue Trend Chart -->
          <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
            <div class="p-6 border-b border-slate-200 dark:border-slate-700">
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Revenue Trend</h3>
              <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">Last 6 months performance</p>
            </div>
            <div class="p-6">
              <LineChart 
                :labels="revenueChartData.labels" 
                :datasets="revenueChartData.datasets" 
                height="300px" 
              />
            </div>
          </div>

          <!-- Orders by Status Chart -->
          <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
            <div class="p-6 border-b border-slate-200 dark:border-slate-700">
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Orders by Status</h3>
              <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">Current order distribution</p>
            </div>
            <div class="p-6">
              <PieChart 
                :labels="ordersChartData.labels" 
                :datasets="ordersChartData.datasets" 
                height="300px" 
              />
            </div>
          </div>
        </div>

        <!-- Charts Row 2 -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <!-- Top Products Chart -->
          <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
            <div class="p-6 border-b border-slate-200 dark:border-slate-700">
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Top Selling Products</h3>
              <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">Best performers this month</p>
            </div>
            <div class="p-6">
              <BarChart 
                :labels="topProductsChartData.labels" 
                :datasets="topProductsChartData.datasets" 
                height="300px" 
              />
            </div>
          </div>

          <!-- Group Buying Performance -->
          <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
            <div class="p-6 border-b border-slate-200 dark:border-slate-700">
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Group Buying Performance</h3>
              <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">Pool fill rates and savings</p>
            </div>
            <div class="p-6">
              <BarChart 
                :labels="groupBuyingChartData.labels" 
                :datasets="groupBuyingChartData.datasets" 
                height="300px" 
              />
            </div>
          </div>
        </div>

        <!-- Key Insights -->
        <div class="bg-gradient-to-r from-blue-50 to-indigo-50 dark:from-blue-900/20 dark:to-indigo-900/20 rounded-lg border border-blue-200 dark:border-blue-800 p-6">
          <div class="flex items-start">
            <div class="flex-shrink-0">
              <SparklesIcon class="w-8 h-8 text-blue-600 dark:text-blue-400" />
            </div>
            <div class="ml-4 flex-1">
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-3">AI Insights</h3>
              <ul class="space-y-2">
                <li v-for="(insight, index) in aiInsights" :key="index" class="flex items-start">
                  <span class="flex-shrink-0 w-6 h-6 flex items-center justify-center bg-blue-100 dark:bg-blue-900/50 rounded-full text-blue-600 dark:text-blue-400 text-xs font-bold mr-3">
                    {{ index + 1 }}
                  </span>
                  <p class="text-slate-700 dark:text-slate-300">{{ insight }}</p>
                </li>
              </ul>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import {
  CurrencyDollarIcon,
  ShoppingCartIcon,
  UserGroupIcon,
  CubeIcon,
  SparklesIcon,
  ArrowPathIcon
} from '@heroicons/vue/24/outline'
import LineChart from '~/components/charts/LineChart.vue'
import BarChart from '~/components/charts/BarChart.vue'
import PieChart from '~/components/charts/PieChart.vue'
import ExportButton from '~/components/common/ExportButton.vue'

// Reactive state
const loading = ref(true)
const metrics = ref({
  totalRevenue: 485600,
  revenueChange: 12.5,
  totalOrders: 342,
  ordersChange: 8.3,
  groupBuySavings: 45820,
  activePoolsCount: 23,
  stockValue: 156780,
  lowStockItems: 12
})

// Chart data - formatted for chart components
const revenueChartData = computed(() => ({
  labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
  datasets: [
    {
      label: 'Revenue (R)',
      data: [385000, 412000, 398000, 445000, 467000, 485600],
      borderColor: 'rgb(59, 130, 246)',
      backgroundColor: 'rgba(59, 130, 246, 0.1)',
      tension: 0.4
    }
  ]
}))

const ordersChartData = computed(() => ({
  labels: ['Confirmed', 'In Transit', 'Delivered', 'Pending'],
  datasets: [
    {
      label: 'Orders',
      data: [89, 45, 156, 52],
      backgroundColor: [
        'rgba(34, 197, 94, 0.8)',
        'rgba(59, 130, 246, 0.8)',
        'rgba(168, 85, 247, 0.8)',
        'rgba(251, 191, 36, 0.8)'
      ]
    }
  ]
}))

const topProductsChartData = computed(() => ({
  labels: ['Bread', 'Milk', 'Maize Meal', 'Sugar', 'Cooking Oil'],
  datasets: [
    {
      label: 'Units Sold',
      data: [1250, 980, 875, 720, 650],
      backgroundColor: 'rgba(59, 130, 246, 0.8)'
    }
  ]
}))

const groupBuyingChartData = computed(() => ({
  labels: ['Week 1', 'Week 2', 'Week 3', 'Week 4'],
  datasets: [
    {
      label: 'Pool Fill Rate (%)',
      data: [68, 75, 82, 88],
      backgroundColor: 'rgba(168, 85, 247, 0.8)'
    },
    {
      label: 'Avg Savings (%)',
      data: [15, 18, 22, 20],
      backgroundColor: 'rgba(34, 197, 94, 0.8)'
    }
  ]
}))

// AI Insights
const aiInsights = computed(() => [
  `Revenue increased 12.5% this month. Your top-selling product is White Bread with 1,250 units sold.`,
  `${metrics.value.activePoolsCount} active group buying pools are saving shops an average of R1,992 each.`,
  `${metrics.value.lowStockItems} items are running low. Consider joining pools for Sugar and Maize Meal to save 20%.`,
  `Delivery costs reduced by 30% through shared logistics. Keep using shared runs for better margins.`
])

// Dashboard data for export
const dashboardData = computed(() => ({
  metrics: metrics.value,
  generatedAt: new Date().toISOString()
}))

// Methods
const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
    minimumFractionDigits: 0
  }).format(amount).replace('ZAR', 'R')
}

const formatDate = (date: Date): string => {
  return new Intl.DateTimeFormat('en-ZA', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  }).format(date)
}

const refreshData = async () => {
  loading.value = true
  // Simulate API call
  await new Promise(resolve => setTimeout(resolve, 1000))
  loading.value = false
}

// Lifecycle
onMounted(async () => {
  await refreshData()
})

// Meta
definePageMeta({
  layout: 'default',
  middleware: 'auth'
})

useHead({
  title: 'Business Dashboard - TOSS ERP',
  meta: [
    { name: 'description', content: 'View your business performance metrics and analytics' }
  ]
})
</script>

