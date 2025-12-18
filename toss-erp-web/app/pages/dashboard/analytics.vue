<template>
  <div class="min-h-screen bg-gray-100 dark:bg-gray-900 p-6">
    <!-- Header -->
    <div class="mb-6">
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">
        Analytics Dashboard
      </h1>
      <p class="text-gray-600 dark:text-gray-400">
        Monitor your business performance and key metrics
      </p>
    </div>

    <!-- KPI Cards Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-6">
      <StatCard
        label="Total Revenue"
        :value="formatCurrency(totalRevenue)"
        :delta="12.5"
        delta-label="than last month"
        icon="payments"
        icon-bg-class="bg-gradient-primary"
      />
      <StatCard
        label="Total Orders"
        :value="totalOrders.toLocaleString()"
        :delta="8.2"
        delta-label="than last month"
        icon="shopping_cart"
        icon-bg-class="bg-gradient-info"
      />
      <StatCard
        label="New Customers"
        :value="newCustomers.toLocaleString()"
        :delta="15.3"
        delta-label="than last month"
        icon="group"
        icon-bg-class="bg-gradient-success"
      />
      <StatCard
        label="Inventory Value"
        :value="formatCurrency(inventoryValue)"
        :delta="-2.1"
        delta-label="than last month"
        icon="inventory_2"
        icon-bg-class="bg-gradient-warning"
      />
    </div>

    <!-- Charts Grid -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
      <!-- Revenue Trend Chart -->
      <Card>
        <div class="mb-4">
          <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-1">
            Revenue Trend
          </h3>
          <p class="text-sm text-gray-500 dark:text-gray-400">
            Monthly revenue over the past 6 months
          </p>
        </div>
        <div class="h-64">
          <LineChart
            :labels="revenueLabels"
            :data="revenueData"
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
            :labels="categoryLabels"
            :data="categoryData"
            label="Sales"
            color="#10B981"
          />
        </div>
      </Card>
    </div>

    <!-- Full-width Chart -->
    <Card class="mb-6">
      <div class="mb-4 flex items-center justify-between">
        <div>
          <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-1">
            Orders Overview
          </h3>
          <p class="text-sm text-gray-500 dark:text-gray-400">
            Daily orders for the past 30 days
          </p>
        </div>
        <div class="flex gap-2">
          <button
            v-for="period in ['7D', '30D', '90D']"
            :key="period"
            :class="[
              'px-3 py-1 rounded-lg text-sm font-medium transition-colors',
              selectedPeriod === period
                ? 'bg-purple-100 dark:bg-purple-900 text-purple-700 dark:text-purple-300'
                : 'text-gray-600 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-gray-700'
            ]"
            @click="selectedPeriod = period"
          >
            {{ period }}
          </button>
        </div>
      </div>
      <div class="h-80">
        <LineChart
          :labels="ordersLabels"
          :data="ordersData"
          label="Orders"
          color="#3B82F6"
        />
      </div>
    </Card>

    <!-- Sales by Region Table -->
    <Card>
      <div class="mb-4">
        <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-1">
          Sales by Region
        </h3>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          Regional sales performance and growth
        </p>
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

    <!-- Offline Indicator -->
    <Transition
      enter-active-class="transition-all duration-300 ease-out"
      enter-from-class="translate-y-2 opacity-0"
      enter-to-class="translate-y-0 opacity-100"
      leave-active-class="transition-all duration-200 ease-in"
      leave-from-class="translate-y-0 opacity-100"
      leave-to-class="translate-y-2 opacity-0"
    >
      <div
        v-if="!isOnline"
        class="fixed bottom-6 right-6 bg-red-500 text-white px-4 py-3 rounded-lg shadow-material-lg flex items-center gap-2"
      >
        <span class="material-symbols-rounded text-xl">cloud_off</span>
        <span class="font-medium">You are offline</span>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useNetwork } from '@vueuse/core'
import Card from '~/components/common/Card.vue'
import StatCard from '~/components/charts/StatCard.vue'
import LineChart from '~/components/charts/LineChart.vue'
import BarChart from '~/components/charts/BarChart.vue'

definePageMeta({
  layout: 'dashboard',
  middleware: 'auth'
})

// Online status
const { isOnline } = useNetwork()

// Selected period
const selectedPeriod = ref('30D')

// KPI Data
const totalRevenue = ref(458750)
const totalOrders = ref(2547)
const newCustomers = ref(356)
const inventoryValue = ref(287500)

// Revenue Trend Data
const revenueLabels = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun']
const revenueData = [65000, 78000, 71000, 85000, 92000, 98000]

// Sales by Category Data
const categoryLabels = ['Food', 'Beverages', 'Household', 'Personal Care', 'Other']
const categoryData = [45000, 32000, 28000, 22000, 15000]

// Orders Overview Data
const ordersLabels = Array.from({ length: 30 }, (_, i) => `Day ${i + 1}`)
const ordersData = Array.from({ length: 30 }, () => Math.floor(Math.random() * 100) + 50)

// Sales by Region Data
const salesByRegion = [
  { name: 'South Africa', flag: 'za', sales: 185000, growth: 15.2, orders: 1248 },
  { name: 'Nigeria', flag: 'ng', sales: 142000, growth: 12.8, orders: 956 },
  { name: 'Kenya', flag: 'ke', sales: 98000, growth: 8.4, orders: 674 },
  { name: 'Ghana', flag: 'gh', sales: 76000, growth: -2.3, orders: 512 },
  { name: 'Tanzania', flag: 'tz', sales: 54000, growth: 18.9, orders: 387 }
]

// Helper function
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
    minimumFractionDigits: 0
  }).format(amount)
}
</script>
