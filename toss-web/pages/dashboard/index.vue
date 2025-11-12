<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref } from 'vue'
import {
  ArrowPathIcon,
  CurrencyDollarIcon,
  ShoppingCartIcon,
  UserGroupIcon,
  TruckIcon,
  CubeIcon,
  UsersIcon,
  ClockIcon,
  ExclamationTriangleIcon,
  SparklesIcon,
} from '@heroicons/vue/24/outline'

// @ts-ignore -- Nuxt auto-injects definePageMeta in setup
definePageMeta({
 
  middleware: 'auth',
})

// @ts-ignore -- Nuxt auto-injects useHead
useHead({
  title: 'Business Analytics - TOSS ERP',
  meta: [
    {
      name: 'description',
      content: 'Real-time business analytics and insights for your township business',
    },
  ],
})

const loading = ref(false)
const minutesAgo = ref(4)

const metrics = ref({
  totalRevenue: 45_678,
  revenueChange: 15.3,
  totalOrders: 234,
  ordersChange: 12.7,
  groupBuySavings: 8_945,
  activePoolsCount: 12,
  totalDeliveryCost: 1_250,
  stockValue: 89_456,
  totalDeliveries: 156,
})

const aiInsights = ref({
  potentialSavings: 3_450,
  itemsToReorder: 23,
  deliveryOptimization: -35,
})

const dailySalesLabels = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']
const dailySalesData = [3_200, 4_100, 3_800, 5_200, 4_800, 6_100, 5_500]

const categoryLabels = ['Groceries', 'Beverages', 'Snacks', 'Toiletries', 'Airtime']
const categoryData = [12_500, 8_900, 6_700, 5_400, 4_200]

const revenueSparkline = [3_200, 3_600, 3_400, 4_100, 4_500, 4_800, 5_200]
const ordersSparkline = [45, 52, 48, 61, 58, 67, 72]

const lowStockItems = ref([
  { id: 1, name: 'White Bread', quantity: 12, reorderLevel: 30 },
  { id: 2, name: 'Fresh Milk 1L', quantity: 8, reorderLevel: 20 },
  { id: 3, name: 'Sugar 2.5kg', quantity: 5, reorderLevel: 15 },
  { id: 4, name: 'Cooking Oil 750ml', quantity: 10, reorderLevel: 25 },
  { id: 5, name: 'Maize Meal 10kg', quantity: 6, reorderLevel: 20 },
])

const formatCurrency = (value: number) =>
  new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
    minimumFractionDigits: 0,
  })
    .format(value)
    .replace('ZAR', 'R')

const formatNumber = (value: number) => new Intl.NumberFormat('en-ZA').format(value)

const formatDate = (date: Date) =>
  new Intl.DateTimeFormat('en-ZA', {
    weekday: 'long',
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  }).format(date)

const refreshData = async () => {
  loading.value = true
  setTimeout(() => {
    loading.value = false
    minutesAgo.value = 0
  }, 1_000)
}

let intervalId: ReturnType<typeof setInterval> | null = null

onMounted(() => {
  intervalId = setInterval(() => {
    if (minutesAgo.value < 60) {
      minutesAgo.value += 1
    }
  }, 60_000)
})

onBeforeUnmount(() => {
  if (intervalId) {
    clearInterval(intervalId)
  }
})
</script>

<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <div class="sticky top-0 z-10 border-b shadow-sm border-slate-200/50 bg-white/80 backdrop-blur-xl dark:border-slate-700/50 dark:bg-slate-800/80">
      <div class="flex items-center justify-between px-4 py-6 mx-auto max-w-7xl sm:px-6 lg:px-8">
        <div>
          <h1 class="text-3xl font-bold text-transparent bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text">
            Business Analytics
          </h1>
          <p class="flex items-center mt-1 space-x-2 text-sm text-slate-600 dark:text-slate-400">
            <span>{{ formatDate(new Date()) }}</span>
            <span class="inline-flex items-center rounded-full bg-green-100 px-2 py-0.5 text-xs font-medium text-green-800 dark:bg-green-900/30 dark:text-green-400">
              <span class="mr-1.5 h-1.5 w-1.5 animate-pulse rounded-full bg-green-500" />
              Live
            </span>
          </p>
        </div>
        <div class="flex items-center space-x-3">
          <Button
            variant="outline"
            class="px-4 py-2 text-sm font-medium transition-all bg-white rounded-xl border-slate-200 text-slate-700 hover:bg-slate-50 hover:shadow-md dark:border-slate-600 dark:bg-slate-800 dark:text-slate-300 dark:hover:bg-slate-700"
            @click="refreshData"
          >
            <ArrowPathIcon :class="['mr-2 h-4 w-4', loading ? 'animate-spin' : '']" />
            Refresh
          </Button>
        </div>
      </div>
    </div>

    <div class="px-4 py-8 mx-auto max-w-7xl sm:px-6 lg:px-8">
      <div v-if="loading" class="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-4">
        <div v-for="index in 4" :key="index" class="h-32 bg-white shadow-lg animate-pulse rounded-2xl dark:bg-slate-800" />
      </div>

      <div v-else class="space-y-6">
        <div class="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-4">
          <StatsCard
            label="Total Revenue"
            :value="metrics.totalRevenue"
            :icon="CurrencyDollarIcon"
            :change="metrics.revenueChange"
            change-label="vs last month"
            gradient="green"
            :show-sparkline="true"
            :sparkline-data="revenueSparkline"
            prefix="R"
          />
          <StatsCard
            label="Total Orders"
            :value="metrics.totalOrders"
            :icon="ShoppingCartIcon"
            :change="metrics.ordersChange"
            change-label="vs last month"
            gradient="blue"
            :show-sparkline="true"
            :sparkline-data="ordersSparkline"
          />
          <StatsCard
            label="Group Buy Savings"
            :value="metrics.groupBuySavings"
            :icon="UserGroupIcon"
            :change="15.3"
            change-label="this month"
            gradient="purple"
            prefix="R"
          />
          <StatsCard
            label="Delivery Costs"
            :value="metrics.totalDeliveryCost"
            :icon="TruckIcon"
            :change="-12.5"
            change-label="saved vs solo"
            gradient="orange"
            prefix="R"
          />
        </div>

        <div class="grid grid-cols-1 gap-6 lg:grid-cols-3">
          <div class="p-6 transition-shadow duration-300 bg-white border shadow-lg lg:col-span-2 rounded-2xl border-slate-200 hover:shadow-xl dark:border-slate-700 dark:bg-slate-800">
            <div class="flex items-center justify-between mb-6">
              <div>
                <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Daily Sales</h3>
                <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
                  <span class="font-semibold text-green-600 dark:text-green-400">(+{{ metrics.revenueChange.toFixed(1) }}%)</span>
                  increase in today's sales
                </p>
              </div>
              <div class="flex items-center space-x-2 text-xs text-slate-500">
                <ClockIcon class="w-4 h-4" />
                <span>Updated {{ minutesAgo }} min ago</span>
              </div>
            </div>
            <ClientOnly>
              <LineChart
                :labels="dailySalesLabels"
                :data="dailySalesData"
                label="Revenue"
                color="#10B981"
                :height="280"
              />
            </ClientOnly>
          </div>

          <div class="p-6 transition-shadow duration-300 bg-white border shadow-lg rounded-2xl border-slate-200 hover:shadow-xl dark:border-slate-700 dark:bg-slate-800">
            <h3 class="mb-6 text-lg font-semibold text-slate-900 dark:text-white">Quick Stats</h3>
            <div class="space-y-4">
              <div class="flex items-center justify-between p-4 border rounded-xl border-blue-200/50 bg-gradient-to-r from-blue-50 to-blue-100/50 dark:border-blue-800/50 dark:from-blue-900/20 dark:to-blue-900/10">
                <div>
                  <p class="text-sm text-slate-600 dark:text-slate-400">Stock Value</p>
                  <p class="text-2xl font-bold text-slate-900 dark:text-white">{{ formatCurrency(metrics.stockValue) }}</p>
                </div>
                <div class="p-3 bg-blue-500 rounded-lg">
                  <CubeIcon class="w-6 h-6 text-white" />
                </div>
              </div>

              <div class="flex items-center justify-between p-4 border rounded-xl border-purple-200/50 bg-gradient-to-r from-purple-50 to-purple-100/50 dark:border-purple-800/50 dark:from-purple-900/20 dark:to-purple-900/10">
                <div>
                  <p class="text-sm text-slate-600 dark:text-slate-400">Active Pools</p>
                  <p class="text-2xl font-bold text-slate-900 dark:text-white">{{ metrics.activePoolsCount }}</p>
                </div>
                <div class="p-3 bg-purple-500 rounded-lg">
                  <UsersIcon class="w-6 h-6 text-white" />
                </div>
              </div>

              <div class="flex items-center justify-between p-4 border rounded-xl border-orange-200/50 bg-gradient-to-r from-orange-50 to-orange-100/50 dark:border-orange-800/50 dark:from-orange-900/20 dark:to-orange-900/10">
                <div>
                  <p class="text-sm text-slate-600 dark:text-slate-400">Deliveries</p>
                  <p class="text-2xl font-bold text-slate-900 dark:text-white">{{ metrics.totalDeliveries }}</p>
                </div>
                <div class="p-3 bg-orange-500 rounded-lg">
                  <TruckIcon class="w-6 h-6 text-white" />
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="grid grid-cols-1 gap-6 lg:grid-cols-2">
          <div class="p-6 transition-shadow duration-300 bg-white border shadow-lg rounded-2xl border-slate-200 hover:shadow-xl dark:border-slate-700 dark:bg-slate-800">
            <div class="flex items-center justify-between mb-6">
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Sales by Category</h3>
              <Button variant="link" class="text-sm text-blue-600 dark:text-blue-400">View All</Button>
            </div>
            <ClientOnly>
              <BarChart
                :labels="categoryLabels"
                :data="categoryData"
                label="Sales"
                color="#3B82F6"
                :height="280"
              />
            </ClientOnly>
          </div>

          <div class="p-6 transition-shadow duration-300 bg-white border shadow-lg rounded-2xl border-slate-200 hover:shadow-xl dark:border-slate-700 dark:bg-slate-800">
            <div class="flex items-center justify-between mb-6">
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Low Stock Items</h3>
              <span class="inline-flex items-center px-3 py-1 text-xs font-medium text-red-800 bg-red-100 rounded-full dark:bg-red-900/30 dark:text-red-400">
                {{ lowStockItems.length }} items
              </span>
            </div>
            <div class="space-y-3">
              <div
                v-for="item in lowStockItems.slice(0, 5)"
                :key="item.id"
                class="flex items-center justify-between p-4 transition-colors cursor-pointer rounded-xl bg-slate-50 hover:bg-slate-100 dark:bg-slate-700/50 dark:hover:bg-slate-700"
              >
                <div class="flex items-center space-x-3">
                  <div class="p-2 bg-orange-100 rounded-lg dark:bg-orange-900/30">
                    <ExclamationTriangleIcon class="w-5 h-5 text-orange-600 dark:text-orange-400" />
                  </div>
                  <div>
                    <p class="font-medium text-slate-900 dark:text-white">{{ item.name }}</p>
                    <p class="text-sm text-slate-600 dark:text-slate-400">{{ item.quantity }} units left</p>
                  </div>
                </div>
                <Button variant="link" class="text-sm text-blue-600 dark:text-blue-400">Reorder</Button>
              </div>
            </div>
            <NuxtLink to="/stock" class="block mt-4 text-sm text-center text-blue-600 hover:underline dark:text-blue-400">
              View all items →
            </NuxtLink>
          </div>
        </div>

        <div class="relative p-8 overflow-hidden text-white shadow-2xl rounded-2xl bg-gradient-to-br from-indigo-500 via-purple-500 to-pink-500">
          <div class="absolute top-0 right-0 w-64 h-64 rounded-full -translate-y-1/3 translate-x-1/3 bg-white/10" />
          <div class="absolute bottom-0 left-0 w-48 h-48 rounded-full -translate-x-1/3 translate-y-1/3 bg-black/10" />
          <div class="relative z-10">
            <div class="flex items-start justify-between">
              <div class="flex items-center mb-4 space-x-3">
                <div class="p-3 rounded-xl bg-white/20 backdrop-blur-sm">
                  <SparklesIcon class="w-6 h-6 text-white" />
                </div>
                <div>
                  <h3 class="text-xl font-bold">AI Copilot Insights</h3>
                  <p class="text-sm text-white/80">Smart recommendations for your business</p>
                </div>
              </div>
            </div>
            <div class="grid grid-cols-1 gap-4 mt-6 md:grid-cols-3">
              <div class="p-4 border rounded-xl border-white/20 bg-white/10 backdrop-blur-sm">
                <p class="mb-2 text-sm text-white/80">Cost Savings Opportunity</p>
                <p class="text-2xl font-bold">R{{ formatNumber(aiInsights.potentialSavings) }}</p>
                <p class="mt-2 text-xs text-white/70">Join 3 active group buying pools</p>
              </div>
              <div class="p-4 border rounded-xl border-white/20 bg-white/10 backdrop-blur-sm">
                <p class="mb-2 text-sm text-white/80">Reorder Suggestion</p>
                <p class="text-2xl font-bold">{{ aiInsights.itemsToReorder }} items</p>
                <p class="mt-2 text-xs text-white/70">Will run out in 3-5 days</p>
              </div>
              <div class="p-4 border rounded-xl border-white/20 bg-white/10 backdrop-blur-sm">
                <p class="mb-2 text-sm text-white/80">Delivery Optimization</p>
                <p class="text-2xl font-bold">{{ aiInsights.deliveryOptimization }}%</p>
                <p class="mt-2 text-xs text-white/70">Share next delivery with 2 neighbors</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
