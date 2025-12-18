<template>
  <div class="min-h-screen bg-gray-100 dark:bg-gray-900 p-6">
    <!-- Header -->
    <div class="mb-6 flex flex-wrap items-start justify-between gap-3">
      <div>
        <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-1">
          Analytics Dashboard
        </h1>
        <p class="text-gray-600 dark:text-gray-400">
          Monitor your business performance and key metrics
        </p>
        <p class="text-xs text-gray-500 dark:text-gray-500 mt-1">
          Last updated: {{ formattedUpdatedAt }}
        </p>
      </div>
      <button
        class="inline-flex items-center gap-2 rounded-lg bg-gradient-primary px-4 py-2 text-sm font-semibold text-white shadow-material hover:shadow-material-lg transition"
        @click="refresh"
      >
        <span class="material-symbols-rounded text-base">refresh</span>
        Refresh
      </button>
    </div>

    <Card
      v-if="error"
      class="mb-6 border border-red-200 bg-red-50 text-red-800 dark:bg-red-900/30 dark:border-red-800 dark:text-red-200"
    >
      <div class="flex items-start justify-between gap-3">
        <div>
          <p class="font-semibold">Failed to load analytics</p>
          <p class="text-sm">{{ error?.message || 'Please try again shortly.' }}</p>
        </div>
        <button
          class="inline-flex items-center gap-2 rounded-lg border border-red-300 px-3 py-2 text-sm font-semibold hover:bg-red-100 dark:hover:bg-red-800"
          @click="refresh"
        >
          Retry
        </button>
      </div>
    </Card>

    <div v-if="showSkeleton" class="space-y-6">
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <div v-for="i in 4" :key="i" class="rounded-xl bg-white dark:bg-gray-800 p-6 shadow-material animate-pulse">
          <div class="h-4 w-24 bg-gray-200 dark:bg-gray-700 rounded mb-3"></div>
          <div class="h-8 w-32 bg-gray-200 dark:bg-gray-700 rounded mb-2"></div>
          <div class="h-4 w-20 bg-gray-200 dark:bg-gray-700 rounded"></div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div class="h-72 rounded-xl bg-white dark:bg-gray-800 shadow-material p-6 animate-pulse"></div>
        <div class="h-72 rounded-xl bg-white dark:bg-gray-800 shadow-material p-6 animate-pulse"></div>
      </div>

      <div class="h-96 rounded-xl bg-white dark:bg-gray-800 shadow-material p-6 animate-pulse"></div>

      <div class="h-80 rounded-xl bg-white dark:bg-gray-800 shadow-material p-6 animate-pulse"></div>
    </div>

    <div v-else class="space-y-6">
      <!-- KPI Cards Grid -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <StatCard
          label="Total Revenue"
          :value="formatCurrency(kpis.totalRevenue)"
          :delta="kpis.revenueDelta"
          delta-label="than last month"
          icon="payments"
          icon-bg-class="bg-gradient-primary"
        />
        <StatCard
          label="Total Orders"
          :value="kpis.totalOrders.toLocaleString()"
          :delta="kpis.ordersDelta"
          delta-label="than last month"
          icon="shopping_cart"
          icon-bg-class="bg-gradient-info"
        />
        <StatCard
          label="New Customers"
          :value="kpis.newCustomers.toLocaleString()"
          :delta="kpis.customersDelta"
          delta-label="than last month"
          icon="group"
          icon-bg-class="bg-gradient-success"
        />
        <StatCard
          label="Inventory Value"
          :value="formatCurrency(kpis.inventoryValue)"
          :delta="kpis.inventoryDelta"
          delta-label="than last month"
          icon="inventory_2"
          icon-bg-class="bg-gradient-warning"
        />
      </div>

      <!-- Charts Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Revenue Trend Chart -->
        <Card>
          <div class="mb-4">
            <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-1">
              Revenue Trend
            </h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">
              Monthly revenue over the past 12 months
            </p>
          </div>
          <div class="h-64">
            <LineChart
              :labels="revenueTrend.labels"
              :data="revenueTrend.data"
              label="Revenue"
              color="#8B5CF6"
            />
          </div>
        </Card>

        <!-- Sales by Category Chart -->
        <Card>
          <div class="mb-4">
            <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-1">
              Sales by Category
            </h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">
              Top product categories this month
            </p>
          </div>
          <div class="h-64">
            <BarChart
              :labels="salesByCategory.labels"
              :data="salesByCategory.data"
              label="Sales"
              color="#10B981"
            />
          </div>
        </Card>
      </div>

      <!-- Full-width Chart -->
      <Card>
        <div class="mb-4 flex items-center justify-between">
          <div>
            <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-1">
              Orders Overview
            </h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">
              Daily orders for the selected period
            </p>
          </div>
          <div class="flex gap-2">
            <button
              v-for="period in periods"
              :key="period"
              :class="[
                'px-3 py-1 rounded-lg text-sm font-medium transition-colors',
                selectedPeriod === period
                  ? 'bg-purple-100 dark:bg-purple-900 text-purple-700 dark:text-purple-300'
                  : 'text-gray-600 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-700'
              ]"
              @click="setPeriod(period)"
            >
              {{ period }}
            </button>
          </div>
        </div>
        <div class="h-80">
          <LineChart
            :labels="ordersSeries.labels"
            :data="ordersSeries.data"
            label="Orders"
            color="#3B82F6"
          />
        </div>
      </Card>

      <!-- Sales by Region Table -->
      <Card>
        <div class="mb-4 flex items-center justify-between">
          <div>
            <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-1">
              Sales by Region
            </h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">
              Regional sales performance and growth
            </p>
          </div>
          <span class="text-sm text-gray-500 dark:text-gray-400">
            Totals include deliveries and airtime where applicable
          </span>
        </div>
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead>
              <tr class="border-b border-gray-200 dark:border-gray-700">
                <th class="text-left py-3 px-4 text-sm font-semibold text-gray-700 dark:text-gray-300">
                  Region
                </th>
                <th class="text-left py-3 px-4 text-sm font-semibold text-gray-700 dark:text-gray-300">
                  Sales
                </th>
                <th class="text-left py-3 px-4 text-sm font-semibold text-gray-700 dark:text-gray-300">
                  Growth
                </th>
                <th class="text-left py-3 px-4 text-sm font-semibold text-gray-700 dark:text-gray-300">
                  Orders
                </th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="region in salesByRegion"
                :key="region.name"
                class="border-b border-gray-100 dark:border-gray-800 hover:bg-gray-50 dark:hover:bg-gray-800 transition-colors"
              >
                <td class="py-3 px-4">
                  <div class="flex items-center gap-3">
                    <img
                      :src="`https://flagcdn.com/w40/${region.flag}.png`"
                      :alt="region.name"
                      class="w-6 h-4 rounded object-cover"
                    />
                    <span class="text-sm font-medium text-gray-900 dark:text-white">
                      {{ region.name }}
                    </span>
                  </div>
                </td>
                <td class="py-3 px-4 text-sm text-gray-900 dark:text-white font-medium">
                  {{ formatCurrency(region.sales) }}
                </td>
                <td class="py-3 px-4">
                  <div class="flex items-center gap-1">
                    <span
                      :class="[
                        'material-symbols-rounded text-base',
                        region.growth >= 0 ? 'text-green-600' : 'text-red-600'
                      ]"
                    >
                      {{ region.growth >= 0 ? 'trending_up' : 'trending_down' }}
                    </span>
                    <span
                      :class="[
                        'text-sm font-semibold',
                        region.growth >= 0 ? 'text-green-600' : 'text-red-600'
                      ]"
                    >
                      {{ region.growth >= 0 ? '+' : '' }}{{ region.growth }}%
                    </span>
                  </div>
                </td>
                <td class="py-3 px-4 text-sm text-gray-600 dark:text-gray-400">
                  {{ region.orders.toLocaleString() }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </Card>
    </div>
  </div>
</template>

<script setup lang="ts">
// @ts-nocheck
import { computed, ref } from 'vue'
import { definePageMeta, useFetch } from 'nuxt/app'
import Card from '../../components/common/Card.vue'
import StatCard from '../../components/charts/StatCard.vue'
import LineChart from '../../components/charts/LineChart.vue'
import BarChart from '../../components/charts/BarChart.vue'

type PeriodOption = '7D' | '30D' | '90D'

interface KPISet {
  totalRevenue: number
  revenueDelta: number
  totalOrders: number
  ordersDelta: number
  newCustomers: number
  customersDelta: number
  inventoryValue: number
  inventoryDelta: number
}

interface TimeSeries {
  labels: string[]
  data: number[]
}

interface OrdersSeries extends TimeSeries {
  period: PeriodOption
}

interface RegionPerformance {
  name: string
  flag: string
  sales: number
  growth: number
  orders: number
}

interface AnalyticsResponse {
  kpis: KPISet
  revenueTrend: TimeSeries
  salesByCategory: TimeSeries
  ordersOverview: Record<PeriodOption, OrdersSeries>
  salesByRegion: RegionPerformance[]
  updatedAt: string
}

definePageMeta({
  layout: 'dashboard',
  middleware: 'auth'
})

const fallbackData: AnalyticsResponse = {
  kpis: {
    totalRevenue: 1250000,
    revenueDelta: 12.5,
    totalOrders: 2457,
    ordersDelta: 8.2,
    newCustomers: 356,
    customersDelta: 15.3,
    inventoryValue: 287500,
    inventoryDelta: -2.1
  },
  revenueTrend: {
    labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
    data: [185000, 198500, 205000, 218000, 224000, 231500, 240000, 248500, 256000, 263500, 271000, 280500]
  },
  salesByCategory: {
    labels: ['Groceries', 'Prepared Food', 'Airtime & Data', 'Household', 'Toiletries', 'Other'],
    data: [485000, 312000, 205000, 164000, 132500, 78000]
  },
  ordersOverview: {
    '7D': {
      period: '7D',
      labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
      data: [180, 192, 205, 210, 224, 238, 252]
    },
    '30D': {
      period: '30D',
      labels: Array.from({ length: 30 }, (_, i) => `Day ${i + 1}`),
      data: Array.from({ length: 30 }, (_, i) => 165 + Math.round(Math.sin(i / 3) * 28) + i % 5)
    },
    '90D': {
      period: '90D',
      labels: Array.from({ length: 90 }, (_, i) => `Day ${i + 1}`),
      data: Array.from({ length: 90 }, (_, i) => 150 + Math.round(Math.cos(i / 4) * 35) + (i % 6))
    }
  },
  salesByRegion: [
    { name: 'South Africa', flag: 'za', sales: 465000, growth: 12.4, orders: 1280 },
    { name: 'Nigeria', flag: 'ng', sales: 348500, growth: 9.8, orders: 1034 },
    { name: 'Kenya', flag: 'ke', sales: 218750, growth: 7.5, orders: 724 },
    { name: 'Ghana', flag: 'gh', sales: 164500, growth: 3.2, orders: 518 },
    { name: 'Tanzania', flag: 'tz', sales: 142750, growth: 5.9, orders: 442 }
  ],
  updatedAt: new Date().toISOString()
}

const periods: PeriodOption[] = ['7D', '30D', '90D']
const selectedPeriod = ref<PeriodOption>('30D')

const { data: analytics, pending, error, refresh } = useFetch<AnalyticsResponse>('/api/analytics/dashboard', {
  default: () => fallbackData
})

const kpis = computed(() => analytics.value?.kpis ?? fallbackData.kpis)
const revenueTrend = computed(() => analytics.value?.revenueTrend ?? fallbackData.revenueTrend)
const salesByCategory = computed(() => analytics.value?.salesByCategory ?? fallbackData.salesByCategory)
const ordersOverview = computed(() => analytics.value?.ordersOverview ?? fallbackData.ordersOverview)
const salesByRegion = computed(() => analytics.value?.salesByRegion ?? fallbackData.salesByRegion)

const ordersSeries = computed(() => ordersOverview.value?.[selectedPeriod.value] ?? ordersOverview.value['30D'])
const formattedUpdatedAt = computed(() => new Date(analytics.value?.updatedAt ?? fallbackData.updatedAt).toLocaleString('en-ZA'))
const showSkeleton = computed(() => pending.value && !analytics.value)

const setPeriod = (period: PeriodOption) => {
  selectedPeriod.value = period
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
    minimumFractionDigits: 0
  }).format(amount)
}
</script>
