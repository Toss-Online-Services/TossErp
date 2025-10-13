<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Purchase Analytics</h1>
              <p class="text-gray-600 dark:text-gray-400">Procurement insights, spend analysis, and supplier performance</p>
            </div>
            <div class="flex space-x-3">
              <select v-model="selectedPeriod" class="px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
                <option value="week">This Week</option>
                <option value="month">This Month</option>
                <option value="quarter">This Quarter</option>
                <option value="year">This Year</option>
              </select>
              <button @click="exportReport" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center">
                <ArrowDownTrayIcon class="w-5 h-5 mr-2" />
                Export Report
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Key Metrics -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total Spend</p>
              <p class="text-3xl font-bold text-gray-900 dark:text-white">R {{ metrics.totalSpend }}M</p>
              <p class="text-sm text-green-600">+12.5% vs last period</p>
            </div>
            <div class="p-3 rounded-full bg-blue-100 dark:bg-blue-900/30">
              <CurrencyDollarIcon class="w-8 h-8 text-blue-600 dark:text-blue-400" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Cost Savings</p>
              <p class="text-3xl font-bold text-green-600">R {{ metrics.costSavings }}K</p>
              <p class="text-sm text-gray-500 dark:text-gray-500">{{ metrics.savingsPercent }}% reduction</p>
            </div>
            <div class="p-3 rounded-full bg-green-100 dark:bg-green-900/30">
              <TrendingDownIcon class="w-8 h-8 text-green-600 dark:text-green-400" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Active POs</p>
              <p class="text-3xl font-bold text-gray-900 dark:text-white">{{ metrics.activePOs }}</p>
              <p class="text-sm text-gray-500 dark:text-gray-500">{{ metrics.avgPOValue }}K avg value</p>
            </div>
            <div class="p-3 rounded-full bg-purple-100 dark:bg-purple-900/30">
              <ShoppingCartIcon class="w-8 h-8 text-purple-600 dark:text-purple-400" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Supplier Score</p>
              <p class="text-3xl font-bold text-gray-900 dark:text-white">{{ metrics.avgSupplierScore }}</p>
              <div class="flex items-center mt-1">
                <StarIcon v-for="i in 5" :key="i" class="w-4 h-4" :class="i <= Math.round(metrics.avgSupplierScore) ? 'text-yellow-400' : 'text-gray-300'" />
              </div>
            </div>
            <div class="p-3 rounded-full bg-yellow-100 dark:bg-yellow-900/30">
              <StarIcon class="w-8 h-8 text-yellow-600 dark:text-yellow-400" />
            </div>
          </div>
        </div>
      </div>

      <!-- Charts Row 1 -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
        <!-- Spend by Category -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Spend by Category</h3>
          <div class="space-y-4">
            <div v-for="category in spendByCategory" :key="category.name" class="space-y-2">
              <div class="flex justify-between items-center">
                <span class="text-sm font-medium text-gray-700 dark:text-gray-300">{{ category.name }}</span>
                <span class="text-sm font-bold text-gray-900 dark:text-white">R {{ category.amount }}K ({{ category.percentage }}%)</span>
              </div>
              <div class="w-full bg-gray-200 rounded-full h-3 dark:bg-gray-700">
                <div class="h-3 rounded-full" 
                     :class="category.color" 
                     :style="{ width: category.percentage + '%' }"></div>
              </div>
            </div>
          </div>
        </div>

        <!-- Top Suppliers -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Top Suppliers by Spend</h3>
          <div class="space-y-3">
            <div v-for="(supplier, index) in topSuppliers" :key="supplier.name" class="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-700 rounded-lg">
              <div class="flex items-center space-x-3">
                <div class="flex-shrink-0 h-10 w-10 rounded-full bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center">
                  <span class="text-sm font-bold text-white">#{{ index + 1 }}</span>
                </div>
                <div>
                  <p class="text-sm font-medium text-gray-900 dark:text-white">{{ supplier.name }}</p>
                  <div class="flex items-center">
                    <StarIcon v-for="i in 5" :key="i" class="w-3 h-3" :class="i <= supplier.rating ? 'text-yellow-400' : 'text-gray-300'" />
                    <span class="ml-1 text-xs text-gray-500">({{ supplier.rating }})</span>
                  </div>
                </div>
              </div>
              <div class="text-right">
                <p class="text-sm font-bold text-gray-900 dark:text-white">R {{ supplier.spend }}K</p>
                <p class="text-xs text-gray-500 dark:text-gray-500">{{ supplier.orders }} orders</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Charts Row 2 -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
        <!-- Purchase Order Trend -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Purchase Order Trend</h3>
          <div class="h-64 flex items-end justify-between space-x-2">
            <div v-for="month in poTrend" :key="month.month" class="flex-1 flex flex-col items-center">
              <div class="w-full bg-blue-600 rounded-t transition-all hover:bg-blue-700"
                   :style="{ height: (month.value / Math.max(...poTrend.map(m => m.value)) * 100) + '%' }">
              </div>
              <p class="text-xs text-gray-600 dark:text-gray-400 mt-2">{{ month.month }}</p>
              <p class="text-xs font-medium text-gray-900 dark:text-white">{{ month.value }}</p>
            </div>
          </div>
        </div>

        <!-- Supplier Performance -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Supplier Performance Metrics</h3>
          <div class="space-y-4">
            <div>
              <div class="flex justify-between text-sm mb-2">
                <span class="text-gray-600 dark:text-gray-400">On-Time Delivery</span>
                <span class="font-medium text-gray-900 dark:text-white">{{ performance.onTimeDelivery }}%</span>
              </div>
              <div class="w-full bg-gray-200 rounded-full h-3 dark:bg-gray-700">
                <div class="bg-green-600 h-3 rounded-full" :style="{ width: performance.onTimeDelivery + '%' }"></div>
              </div>
            </div>
            <div>
              <div class="flex justify-between text-sm mb-2">
                <span class="text-gray-600 dark:text-gray-400">Quality Pass Rate</span>
                <span class="font-medium text-gray-900 dark:text-white">{{ performance.qualityRate }}%</span>
              </div>
              <div class="w-full bg-gray-200 rounded-full h-3 dark:bg-gray-700">
                <div class="bg-blue-600 h-3 rounded-full" :style="{ width: performance.qualityRate + '%' }"></div>
              </div>
            </div>
            <div>
              <div class="flex justify-between text-sm mb-2">
                <span class="text-gray-600 dark:text-gray-400">Price Competitiveness</span>
                <span class="font-medium text-gray-900 dark:text-white">{{ performance.priceCompetitiveness }}%</span>
              </div>
              <div class="w-full bg-gray-200 rounded-full h-3 dark:bg-gray-700">
                <div class="bg-purple-600 h-3 rounded-full" :style="{ width: performance.priceCompetitiveness + '%' }"></div>
              </div>
            </div>
            <div>
              <div class="flex justify-between text-sm mb-2">
                <span class="text-gray-600 dark:text-gray-400">Response Time</span>
                <span class="font-medium text-gray-900 dark:text-white">{{ performance.responseTime }}%</span>
              </div>
              <div class="w-full bg-gray-200 rounded-full h-3 dark:bg-gray-700">
                <div class="bg-yellow-600 h-3 rounded-full" :style="{ width: performance.responseTime + '%' }"></div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- KPIs Grid -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
        <!-- Procurement Cycle Time -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center mb-4">
            <div class="p-3 rounded-full bg-blue-100 dark:bg-blue-900/30">
              <ClockIcon class="w-8 h-8 text-blue-600 dark:text-blue-400" />
            </div>
            <div class="ml-4">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Cycle Time</h3>
              <p class="text-sm text-gray-600 dark:text-gray-400">Requisition to delivery</p>
            </div>
          </div>
          <div class="text-center py-4">
            <p class="text-4xl font-bold text-blue-600">{{ kpis.avgCycleTime }}</p>
            <p class="text-sm text-gray-600 dark:text-gray-400">days average</p>
          </div>
          <div class="grid grid-cols-3 gap-2 text-center text-xs">
            <div>
              <p class="font-medium text-gray-900 dark:text-white">{{ kpis.fastestCycle }}</p>
              <p class="text-gray-500">Fastest</p>
            </div>
            <div>
              <p class="font-medium text-gray-900 dark:text-white">{{ kpis.avgCycleTime }}</p>
              <p class="text-gray-500">Average</p>
            </div>
            <div>
              <p class="font-medium text-gray-900 dark:text-white">{{ kpis.slowestCycle }}</p>
              <p class="text-gray-500">Slowest</p>
            </div>
          </div>
        </div>

        <!-- Compliance Rate -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center mb-4">
            <div class="p-3 rounded-full bg-green-100 dark:bg-green-900/30">
              <ShieldCheckIcon class="w-8 h-8 text-green-600 dark:text-green-400" />
            </div>
            <div class="ml-4">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Compliance</h3>
              <p class="text-sm text-gray-600 dark:text-gray-400">Policy adherence</p>
            </div>
          </div>
          <div class="text-center py-4">
            <p class="text-4xl font-bold text-green-600">{{ kpis.complianceRate }}%</p>
            <p class="text-sm text-gray-600 dark:text-gray-400">compliance rate</p>
          </div>
          <div class="grid grid-cols-2 gap-2 text-center text-xs">
            <div>
              <p class="font-medium text-green-600">{{ kpis.approvedWithinBudget }}</p>
              <p class="text-gray-500">Within budget</p>
            </div>
            <div>
              <p class="font-medium text-blue-600">{{ kpis.properApprovals }}</p>
              <p class="text-gray-500">Proper approvals</p>
            </div>
          </div>
        </div>

        <!-- ROI Analysis -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center mb-4">
            <div class="p-3 rounded-full bg-purple-100 dark:bg-purple-900/30">
              <ChartBarIcon class="w-8 h-8 text-purple-600 dark:text-purple-400" />
            </div>
            <div class="ml-4">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Procurement ROI</h3>
              <p class="text-sm text-gray-600 dark:text-gray-400">Return on investment</p>
            </div>
          </div>
          <div class="text-center py-4">
            <p class="text-4xl font-bold text-purple-600">{{ kpis.procurementROI }}%</p>
            <p class="text-sm text-gray-600 dark:text-gray-400">cost vs value</p>
          </div>
          <div class="grid grid-cols-2 gap-2 text-center text-xs">
            <div>
              <p class="font-medium text-gray-900 dark:text-white">R {{ kpis.totalValueAdded }}M</p>
              <p class="text-gray-500">Value added</p>
            </div>
            <div>
              <p class="font-medium text-gray-900 dark:text-white">R {{ kpis.procurementCost }}K</p>
              <p class="text-gray-500">Proc. cost</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Detailed Analytics -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Payment Analysis -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Payment Analysis</h3>
          <div class="space-y-4">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400 mb-2">Early Payments</p>
              <div class="flex items-center justify-between">
                <div class="w-full bg-gray-200 rounded-full h-2 mr-3 dark:bg-gray-700">
                  <div class="bg-green-600 h-2 rounded-full" style="width: 45%"></div>
                </div>
                <span class="text-sm font-medium">45%</span>
              </div>
            </div>
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400 mb-2">On-Time Payments</p>
              <div class="flex items-center justify-between">
                <div class="w-full bg-gray-200 rounded-full h-2 mr-3 dark:bg-gray-700">
                  <div class="bg-blue-600 h-2 rounded-full" style="width: 88%"></div>
                </div>
                <span class="text-sm font-medium">88%</span>
              </div>
            </div>
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400 mb-2">Late Payments</p>
              <div class="flex items-center justify-between">
                <div class="w-full bg-gray-200 rounded-full h-2 mr-3 dark:bg-gray-700">
                  <div class="bg-red-600 h-2 rounded-full" style="width: 8%"></div>
                </div>
                <span class="text-sm font-medium">8%</span>
              </div>
            </div>
            <div class="pt-3 border-t border-gray-200 dark:border-gray-700">
              <div class="flex justify-between">
                <span class="text-sm text-gray-600 dark:text-gray-400">Avg Payment Delay:</span>
                <span class="text-sm font-medium text-gray-900 dark:text-white">2.3 days</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Material Types Distribution -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Material Types Distribution</h3>
          <div class="space-y-3">
            <div v-for="type in materialTypes" :key="type.name" class="flex items-center justify-between">
              <div class="flex items-center flex-1">
                <div class="w-3 h-3 rounded-full mr-3" :class="type.color"></div>
                <span class="text-sm text-gray-700 dark:text-gray-300">{{ type.name }}</span>
              </div>
              <div class="flex items-center space-x-2">
                <span class="text-sm font-medium text-gray-900 dark:text-white">{{ type.count }}</span>
                <span class="text-xs text-gray-500 dark:text-gray-500">({{ type.percentage }}%)</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Recent Activity -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Recent Activity</h3>
          <div class="space-y-3">
            <div v-for="activity in recentActivity" :key="activity.id" class="flex items-start space-x-3 p-2">
              <div class="flex-shrink-0">
                <component :is="activity.icon" class="w-5 h-5" :class="activity.iconColor" />
              </div>
              <div class="flex-1 min-w-0">
                <p class="text-sm text-gray-900 dark:text-white">{{ activity.description }}</p>
                <p class="text-xs text-gray-500 dark:text-gray-500">{{ formatTimeAgo(activity.timestamp) }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import {
  ArrowDownTrayIcon,
  CurrencyDollarIcon,
  TrendingDownIcon,
  ShoppingCartIcon,
  StarIcon,
  ChartBarIcon,
  ClockIcon,
  ShieldCheckIcon,
  CheckCircleIcon,
  TruckIcon,
  ExclamationTriangleIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Purchase Analytics - TOSS ERP',
  meta: [
    { name: 'description', content: 'Procurement analytics and supplier performance insights in TOSS ERP' }
  ]
})

// Reactive data
const selectedPeriod = ref('month')

// Metrics
const metrics = ref({
  totalSpend: 2.45,
  costSavings: 245,
  savingsPercent: 18,
  activePOs: 124,
  avgPOValue: 19.8,
  avgSupplierScore: 4.2
})

// KPIs
const kpis = ref({
  avgCycleTime: 12,
  fastestCycle: 5,
  slowestCycle: 28,
  complianceRate: 94,
  approvedWithinBudget: 156,
  properApprovals: 142,
  procurementROI: 385,
  totalValueAdded: 8.9,
  procurementCost: 245
})

// Spend by category
const spendByCategory = ref([
  { name: 'Raw Materials', amount: 980, percentage: 40, color: 'bg-blue-600' },
  { name: 'Equipment', amount: 612, percentage: 25, color: 'bg-green-600' },
  { name: 'Services', amount: 490, percentage: 20, color: 'bg-purple-600' },
  { name: 'Consumables', amount: 245, percentage: 10, color: 'bg-yellow-600' },
  { name: 'Other', amount: 123, percentage: 5, color: 'bg-gray-600' }
])

// Top suppliers
const topSuppliers = ref([
  { name: 'Raw Materials Corp', spend: 456, orders: 45, rating: 4 },
  { name: 'Tech Solutions Inc', spend: 389, orders: 38, rating: 5 },
  { name: 'Quality Equipment Co', spend: 312, orders: 28, rating: 5 },
  { name: 'Industrial Supplies SA', spend: 245, orders: 52, rating: 4 },
  { name: 'Service Pro LLC', spend: 198, orders: 31, rating: 4 }
])

// PO Trend
const poTrend = ref([
  { month: 'Jul', value: 28 },
  { month: 'Aug', value: 35 },
  { month: 'Sep', value: 42 },
  { month: 'Oct', value: 38 },
  { month: 'Nov', value: 45 },
  { month: 'Dec', value: 52 },
  { month: 'Jan', value: 48 }
])

// Performance metrics
const performance = ref({
  onTimeDelivery: 94,
  qualityRate: 96,
  priceCompetitiveness: 87,
  responseTime: 92
})

// Material types
const materialTypes = ref([
  { name: 'Raw Materials', count: 156, percentage: 45, color: 'bg-blue-500' },
  { name: 'Spare Parts', count: 89, percentage: 26, color: 'bg-green-500' },
  { name: 'Consumables', count: 67, percentage: 19, color: 'bg-purple-500' },
  { name: 'Services', count: 34, percentage: 10, color: 'bg-yellow-500' }
])

// Recent activity
const recentActivity = ref([
  {
    id: 1,
    description: 'PO-2025-045 delivered by Raw Materials Corp',
    icon: CheckCircleIcon,
    iconColor: 'text-green-600',
    timestamp: new Date(Date.now() - 2 * 60 * 60 * 1000)
  },
  {
    id: 2,
    description: 'RFQ-2025-008 awarded to Tech Solutions Inc',
    icon: TruckIcon,
    iconColor: 'text-blue-600',
    timestamp: new Date(Date.now() - 5 * 60 * 60 * 1000)
  },
  {
    id: 3,
    description: 'MR-2025-023 pending approval (Urgent)',
    icon: ExclamationTriangleIcon,
    iconColor: 'text-yellow-600',
    timestamp: new Date(Date.now() - 8 * 60 * 60 * 1000)
  }
])

// Helper functions
const formatTimeAgo = (date: Date) => {
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()
  const diffHours = Math.floor(diffMs / (1000 * 60 * 60))
  const diffDays = Math.floor(diffHours / 24)
  
  if (diffDays > 0) {
    return `${diffDays} day${diffDays > 1 ? 's' : ''} ago`
  } else if (diffHours > 0) {
    return `${diffHours} hour${diffHours > 1 ? 's' : ''} ago`
  } else {
    return 'Just now'
  }
}

const exportReport = () => {
  alert('Exporting comprehensive purchase analytics report...')
}
</script>

