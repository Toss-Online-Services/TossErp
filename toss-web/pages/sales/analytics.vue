<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <!-- Mobile-First Page Container -->
    <div class="p-4 sm:p-6 space-y-4 sm:space-y-6 pb-20 lg:pb-6">
      <!-- Page Header -->
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3 sm:gap-0">
        <div>
          <div class="flex items-center gap-3">
            <h1 class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">Sales Analytics</h1>
            <span class="px-3 py-1 text-xs bg-gradient-to-r from-blue-500 to-purple-600 text-white rounded-full flex items-center gap-1">
              <SparklesIcon class="w-3 h-3" />
              AI-Powered
            </span>
          </div>
          <p class="text-slate-600 dark:text-slate-400 mt-1 text-sm sm:text-base">Performance insights for Thabo's Spaza Shop</p>
        </div>
        <div class="flex flex-wrap gap-2 sm:gap-3">
          <select v-model="selectedPeriod" 
                  class="px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
            <option value="today">Today</option>
            <option value="week">This Week</option>
            <option value="month">This Month</option>
            <option value="quarter">This Quarter</option>
            <option value="year">This Year</option>
          </select>
          <button @click="exportReport" 
                  class="flex-1 sm:flex-none px-4 py-2 sm:px-6 sm:py-3 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-colors text-sm sm:text-base">
            <ArrowDownTrayIcon class="w-4 h-4 sm:w-5 sm:h-5 inline mr-2" />
            Export
          </button>
        </div>
      </div>

      <!-- Key Metrics -->
      <div class="grid grid-cols-1 xs:grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-6">
        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Revenue</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">R {{ formatCurrency(totalRevenue) }}</p>
              <p class="text-xs sm:text-sm" :class="revenueGrowth >= 0 ? 'text-green-600' : 'text-red-600'">
                {{ revenueGrowth >= 0 ? '+' : '' }}{{ revenueGrowth }}% vs last period
              </p>
            </div>
            <div class="p-2 sm:p-3 bg-green-100 dark:bg-green-900 rounded-full">
              <CurrencyDollarIcon class="w-4 h-4 sm:w-6 sm:h-6 text-green-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Transactions</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ totalTransactions }}</p>
              <p class="text-xs sm:text-sm" :class="transactionGrowth >= 0 ? 'text-green-600' : 'text-red-600'">
                {{ transactionGrowth >= 0 ? '+' : '' }}{{ transactionGrowth }}% vs last period
              </p>
            </div>
            <div class="p-2 sm:p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
              <ShoppingCartIcon class="w-4 h-4 sm:w-6 sm:h-6 text-blue-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Avg Order</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">R {{ formatCurrency(avgOrderValue) }}</p>
              <p class="text-xs sm:text-sm" :class="avgOrderGrowth >= 0 ? 'text-green-600' : 'text-red-600'">
                {{ avgOrderGrowth >= 0 ? '+' : '' }}{{ avgOrderGrowth }}% vs last period
              </p>
            </div>
            <div class="p-2 sm:p-3 bg-purple-100 dark:bg-purple-900 rounded-full">
              <CalculatorIcon class="w-4 h-4 sm:w-6 sm:h-6 text-purple-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Customers</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ totalCustomers }}</p>
              <p class="text-xs sm:text-sm" :class="customerGrowth >= 0 ? 'text-green-600' : 'text-red-600'">
                {{ customerGrowth >= 0 ? '+' : '' }}{{ customerGrowth }}% vs last period
              </p>
            </div>
            <div class="p-2 sm:p-3 bg-yellow-100 dark:bg-yellow-900 rounded-full">
              <UsersIcon class="w-4 h-4 sm:w-6 sm:h-6 text-yellow-600" />
            </div>
          </div>
        </div>
      </div>

      <!-- Charts Row -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-4 sm:gap-6">
        <!-- Sales Trend Chart -->
        <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 p-4 sm:p-6">
          <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white mb-4">Sales Trend</h3>
          <div class="h-64 flex items-center justify-center bg-slate-50 dark:bg-slate-700 rounded-lg">
            <div class="text-center">
              <ChartBarIcon class="w-12 h-12 text-slate-400 mx-auto mb-2" />
              <p class="text-slate-600 dark:text-slate-400">Sales chart visualization</p>
              <p class="text-sm text-slate-500 dark:text-slate-500">R {{ formatCurrency(totalRevenue) }} total revenue</p>
            </div>
          </div>
        </div>

        <!-- Category Performance -->
        <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 p-4 sm:p-6">
          <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white mb-4">Top Categories</h3>
          <div class="space-y-4">
            <div v-for="category in topCategories" :key="category.name" class="flex items-center justify-between">
              <div class="flex items-center space-x-3">
                <div class="w-8 h-8 rounded-full flex items-center justify-center" :style="{ backgroundColor: category.color }">
                  <TagIcon class="w-4 h-4 text-white" />
                </div>
                <div>
                  <p class="font-medium text-slate-900 dark:text-white">{{ category.name }}</p>
                  <p class="text-sm text-slate-600 dark:text-slate-400">{{ category.transactions }} transactions</p>
                </div>
              </div>
              <div class="text-right">
                <p class="font-semibold text-slate-900 dark:text-white">R {{ formatCurrency(category.revenue) }}</p>
                <p class="text-sm text-slate-600 dark:text-slate-400">{{ category.percentage }}%</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Performance Tables -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-4 sm:gap-6">
        <!-- Top Products -->
        <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
            <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">Top Selling Products</h3>
          </div>
          <div class="p-4 sm:p-6">
            <div class="space-y-4">
              <div v-for="(product, index) in topProducts" :key="product.id" class="flex items-center justify-between">
                <div class="flex items-center space-x-3">
                  <div class="w-8 h-8 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center">
                    <span class="text-sm font-bold text-blue-600">{{ index + 1 }}</span>
                  </div>
                  <div>
                    <p class="font-medium text-slate-900 dark:text-white">{{ product.name }}</p>
                    <p class="text-sm text-slate-600 dark:text-slate-400">{{ product.sold }} units sold</p>
                  </div>
                </div>
                <div class="text-right">
                  <p class="font-semibold text-slate-900 dark:text-white">R {{ formatCurrency(product.revenue) }}</p>
                  <p class="text-sm text-slate-600 dark:text-slate-400">R {{ formatCurrency(product.price) }} each</p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Customer Insights -->
        <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
            <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">Customer Insights</h3>
          </div>
          <div class="p-4 sm:p-6">
            <div class="space-y-4">
              <div class="flex justify-between items-center">
                <span class="text-slate-600 dark:text-slate-400">Peak Hours</span>
                <span class="font-semibold text-slate-900 dark:text-white">{{ peakHours }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-slate-600 dark:text-slate-400">Avg Items per Sale</span>
                <span class="font-semibold text-slate-900 dark:text-white">{{ avgItemsPerSale }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-slate-600 dark:text-slate-400">Return Rate</span>
                <span class="font-semibold text-slate-900 dark:text-white">{{ returnRate }}%</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-slate-600 dark:text-slate-400">Cash vs Card</span>
                <span class="font-semibold text-slate-900 dark:text-white">{{ cashVsCard }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-slate-600 dark:text-slate-400">Repeat Customers</span>
                <span class="font-semibold text-slate-900 dark:text-white">{{ repeatCustomers }}%</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Payment Method Analysis -->
      <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
        <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">Payment Methods</h3>
        </div>
        <div class="p-4 sm:p-6">
          <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
            <div v-for="method in paymentMethods" :key="method.type" class="text-center">
              <div class="w-12 h-12 rounded-full flex items-center justify-center mx-auto mb-3" :style="{ backgroundColor: method.color + '20' }">
                <BanknotesIcon v-if="method.type === 'Cash'" class="w-6 h-6" :style="{ color: method.color }" />
                <CreditCardIcon v-else-if="method.type === 'Card'" class="w-6 h-6" :style="{ color: method.color }" />
                <DevicePhoneMobileIcon v-else-if="method.type === 'Mobile'" class="w-6 h-6" :style="{ color: method.color }" />
                <GiftIcon v-else class="w-6 h-6" :style="{ color: method.color }" />
              </div>
              <p class="font-semibold text-slate-900 dark:text-white">{{ method.type }}</p>
              <p class="text-2xl font-bold" :style="{ color: method.color }">{{ method.percentage }}%</p>
              <p class="text-sm text-slate-600 dark:text-slate-400">R {{ formatCurrency(method.amount) }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Sales Forecasting -->
      <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
        <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
          <div class="flex justify-between items-center">
            <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">Sales Forecast</h3>
            <span class="text-sm text-slate-600 dark:text-slate-400">AI-powered predictions</span>
          </div>
        </div>
        <div class="p-4 sm:p-6">
          <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
            <div class="text-center p-4 bg-blue-50 dark:bg-blue-900/20 rounded-lg">
              <p class="text-sm text-slate-600 dark:text-slate-400">Tomorrow</p>
              <p class="text-xl font-bold text-blue-600 dark:text-blue-400">R {{ formatCurrency(forecast.tomorrow) }}</p>
              <p class="text-xs text-slate-500">{{ forecastConfidence }}% confidence</p>
            </div>
            <div class="text-center p-4 bg-green-50 dark:bg-green-900/20 rounded-lg">
              <p class="text-sm text-slate-600 dark:text-slate-400">Next Week</p>
              <p class="text-xl font-bold text-green-600 dark:text-green-400">R {{ formatCurrency(forecast.nextWeek) }}</p>
              <p class="text-xs text-slate-500">{{ forecastConfidence - 5 }}% confidence</p>
            </div>
            <div class="text-center p-4 bg-purple-50 dark:bg-purple-900/20 rounded-lg">
              <p class="text-sm text-slate-600 dark:text-slate-400">Next Month</p>
              <p class="text-xl font-bold text-purple-600 dark:text-purple-400">R {{ formatCurrency(forecast.nextMonth) }}</p>
              <p class="text-xs text-slate-500">{{ forecastConfidence - 10 }}% confidence</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Recommendations -->
      <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
        <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">AI Recommendations</h3>
        </div>
        <div class="p-4 sm:p-6">
          <div class="space-y-4">
            <div v-for="recommendation in recommendations" :key="recommendation.id" 
                 class="flex items-start space-x-3 p-4 bg-slate-50 dark:bg-slate-700 rounded-lg">
              <div class="w-8 h-8 rounded-full flex items-center justify-center" :class="getRecommendationColor(recommendation.type)">
                <LightBulbIcon class="w-4 h-4 text-white" />
              </div>
              <div class="flex-1">
                <p class="font-medium text-slate-900 dark:text-white">{{ recommendation.title }}</p>
                <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">{{ recommendation.description }}</p>
                <div class="flex items-center mt-2 gap-2">
                  <span class="text-xs px-2 py-1 rounded-full" :class="getRecommendationBadge(recommendation.impact)">
                    {{ recommendation.impact }} impact
                  </span>
                  <span class="text-xs text-slate-500">{{ recommendation.effort }} effort</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { 
  CurrencyDollarIcon,
  ShoppingCartIcon,
  CalculatorIcon,
  UsersIcon,
  ChartBarIcon,
  TagIcon,
  ArrowDownTrayIcon,
  BanknotesIcon,
  CreditCardIcon,
  DevicePhoneMobileIcon,
  GiftIcon,
  LightBulbIcon,
  SparklesIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Sales Analytics - TOSS ERP',
  meta: [
    { name: 'description', content: 'Performance insights for Thabo\'s Spaza Shop' }
  ]
})

// Layout
definePageMeta({
  layout: 'dashboard'
})

// Reactive data
const selectedPeriod = ref('month')

// Key metrics for Thabo's Spaza Shop
const totalRevenue = ref(89450)
const revenueGrowth = ref(12.5)
const totalTransactions = ref(456)
const transactionGrowth = ref(8.3)
const avgOrderValue = ref(196)
const avgOrderGrowth = ref(4.2)
const totalCustomers = ref(234)
const customerGrowth = ref(15.7)

// Top categories
const topCategories = ref([
  { name: 'Groceries', revenue: 34200, transactions: 156, percentage: 38, color: '#10b981' },
  { name: 'Beverages', revenue: 23100, transactions: 134, percentage: 26, color: '#3b82f6' },
  { name: 'Snacks', revenue: 15800, transactions: 89, percentage: 18, color: '#f59e0b' },
  { name: 'Household', revenue: 12500, transactions: 67, percentage: 14, color: '#8b5cf6' },
  { name: 'Personal Care', revenue: 3850, transactions: 45, percentage: 4, color: '#ef4444' }
])

// Top products
const topProducts = ref([
  { id: '1', name: 'Coca Cola 2L', sold: 89, revenue: 3115, price: 35 },
  { id: '2', name: 'White Bread 700g', sold: 156, revenue: 2808, price: 18 },
  { id: '3', name: 'Milk 1L', sold: 78, revenue: 1716, price: 22 },
  { id: '4', name: 'Maggi 2-Minute Noodles', sold: 234, revenue: 1872, price: 8 },
  { id: '5', name: 'Sunlight Soap 250g', sold: 67, revenue: 1005, price: 15 }
])

// Customer insights
const peakHours = ref('4PM - 7PM')
const avgItemsPerSale = ref(4.2)
const returnRate = ref(2.1)
const cashVsCard = ref('65% / 35%')
const repeatCustomers = ref(68)

// Payment methods
const paymentMethods = ref([
  { type: 'Cash', percentage: 65, amount: 58142, color: '#10b981' },
  { type: 'Card', percentage: 28, amount: 25046, color: '#3b82f6' },
  { type: 'Mobile', percentage: 5, amount: 4472, color: '#8b5cf6' },
  { type: 'Credit', percentage: 2, amount: 1790, color: '#f59e0b' }
])

// Sales forecast
const forecast = ref({
  tomorrow: 2850,
  nextWeek: 19600,
  nextMonth: 94500
})
const forecastConfidence = ref(87)

// AI Recommendations
const recommendations = ref([
  {
    id: '1',
    type: 'inventory',
    title: 'Stock More Milk During Peak Hours',
    description: 'Milk runs out frequently during 4-7PM. Consider increasing stock by 25% for these hours.',
    impact: 'high',
    effort: 'low'
  },
  {
    id: '2',
    type: 'pricing',
    title: 'Bundle Popular Snacks',
    description: 'Create combo deals with chips and beverages to increase average order value.',
    impact: 'medium',
    effort: 'low'
  },
  {
    id: '3',
    type: 'customer',
    title: 'Weekend Promotion Strategy',
    description: 'Sales drop 15% on weekends. Consider weekend-specific promotions.',
    impact: 'medium',
    effort: 'medium'
  },
  {
    id: '4',
    type: 'operational',
    title: 'Optimize Staff Scheduling',
    description: 'Peak transaction times suggest need for additional staff during 4-7PM.',
    impact: 'high',
    effort: 'medium'
  }
])

// Helper functions
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

const getRecommendationColor = (type: string) => {
  const colors = {
    inventory: 'bg-blue-600',
    pricing: 'bg-green-600',
    customer: 'bg-purple-600',
    operational: 'bg-orange-600'
  }
  return colors[type as keyof typeof colors] || 'bg-slate-600'
}

const getRecommendationBadge = (impact: string) => {
  const badges = {
    high: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    medium: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    low: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
  }
  return badges[impact as keyof typeof badges] || 'bg-slate-100 text-slate-800'
}

// Actions
const exportReport = () => {
  alert('Exporting analytics report... Feature coming soon!')
}

// Watch for period changes
watch(selectedPeriod, (newPeriod) => {
  // In a real app, this would fetch new data based on the selected period
  console.log('Period changed to:', newPeriod)
})
</script>
