<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full mx-auto px-3 sm:px-4 lg:px-8 py-4 sm:py-6">
        <div class="flex items-center justify-between">
          <div>
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">
              Sales Reports
            </h1>
            <p class="mt-1 text-xs sm:text-sm text-slate-600 dark:text-slate-400">
              Comprehensive sales analytics and reporting
            </p>
          </div>
          <NuxtLink 
            to="/sales" 
            class="px-4 py-2 rounded-xl text-sm font-medium text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-all"
          >
            ← Back to Dashboard
          </NuxtLink>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="w-full max-w-7xl mx-auto px-3 sm:px-4 lg:px-8 py-4 sm:py-8">
      <!-- Quick Stats -->
      <div class="grid grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-4 mb-6">
        <div class="bg-white dark:bg-slate-800 rounded-xl p-4 shadow-lg border border-slate-200 dark:border-slate-700">
          <p class="text-sm text-slate-600 dark:text-slate-400">Today's Sales</p>
          <p class="text-2xl font-bold text-slate-900 dark:text-white">R{{ formatCurrency(todaysSales) }}</p>
          <p class="text-xs text-green-600 mt-1">{{ todaysTransactions }} transactions</p>
        </div>
        <div class="bg-white dark:bg-slate-800 rounded-xl p-4 shadow-lg border border-slate-200 dark:border-slate-700">
          <p class="text-sm text-slate-600 dark:text-slate-400">Held Sales</p>
          <p class="text-2xl font-bold text-slate-900 dark:text-white">{{ heldSalesCount }}</p>
          <p class="text-xs text-orange-600 mt-1">R{{ formatCurrency(heldSalesTotal) }}</p>
        </div>
        <div class="bg-white dark:bg-slate-800 rounded-xl p-4 shadow-lg border border-slate-200 dark:border-slate-700">
          <p class="text-sm text-slate-600 dark:text-slate-400">Voided Sales</p>
          <p class="text-2xl font-bold text-slate-900 dark:text-white">{{ voidedSalesCount }}</p>
          <p class="text-xs text-red-600 mt-1">R{{ formatCurrency(voidedSalesTotal) }}</p>
        </div>
        <div class="bg-white dark:bg-slate-800 rounded-xl p-4 shadow-lg border border-slate-200 dark:border-slate-700">
          <p class="text-sm text-slate-600 dark:text-slate-400">Average Sale</p>
          <p class="text-2xl font-bold text-slate-900 dark:text-white">R{{ formatCurrency(averageSale) }}</p>
          <p class="text-xs text-blue-600 mt-1">Per transaction</p>
        </div>
      </div>

      <!-- Report Categories -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 sm:gap-6">
        <!-- Daily Sales Report -->
        <div 
          @click="openReport('daily')"
          class="bg-white dark:bg-slate-800 rounded-2xl p-6 shadow-lg border border-slate-200 dark:border-slate-700 hover:shadow-xl transition-all cursor-pointer group hover:-translate-y-1"
        >
          <div class="flex items-center justify-between mb-4">
            <div class="p-3 bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 7h6m0 10v-3m-3 3h.01M9 17h.01M9 14h.01M12 14h.01M15 11h.01M12 11h.01M9 11h.01M7 21h10a2 2 0 002-2V5a2 2 0 00-2-2H7a2 2 0 00-2 2v14a2 2 0 002 2z" />
              </svg>
            </div>
            <span class="px-3 py-1 bg-green-100 dark:bg-green-900/30 text-green-600 dark:text-green-400 rounded-full text-xs font-semibold">
              Daily
            </span>
          </div>
          <h3 class="text-lg font-bold text-slate-900 dark:text-white mb-2">Daily Sales Report</h3>
          <p class="text-sm text-slate-600 dark:text-slate-400">
            Complete overview of today's sales, transactions, and payment methods
          </p>
        </div>

        <!-- Payment Methods Report -->
        <div 
          @click="openReport('payment-methods')"
          class="bg-white dark:bg-slate-800 rounded-2xl p-6 shadow-lg border border-slate-200 dark:border-slate-700 hover:shadow-xl transition-all cursor-pointer group hover:-translate-y-1"
        >
          <div class="flex items-center justify-between mb-4">
            <div class="p-3 bg-gradient-to-br from-blue-500 to-indigo-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
              </svg>
            </div>
            <span class="px-3 py-1 bg-blue-100 dark:bg-blue-900/30 text-blue-600 dark:text-blue-400 rounded-full text-xs font-semibold">
              Analysis
            </span>
          </div>
          <h3 class="text-lg font-bold text-slate-900 dark:text-white mb-2">Payment Methods</h3>
          <p class="text-sm text-slate-600 dark:text-slate-400">
            Breakdown of sales by payment type: Cash, Card, Mobile Money, etc.
          </p>
        </div>

        <!-- Held Sales Report -->
        <div 
          @click="openReport('held-sales')"
          class="bg-white dark:bg-slate-800 rounded-2xl p-6 shadow-lg border border-slate-200 dark:border-slate-700 hover:shadow-xl transition-all cursor-pointer group hover:-translate-y-1"
        >
          <div class="flex items-center justify-between mb-4">
            <div class="p-3 bg-gradient-to-br from-orange-500 to-amber-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            <span class="px-3 py-1 bg-orange-100 dark:bg-orange-900/30 text-orange-600 dark:text-orange-400 rounded-full text-xs font-semibold">
              Pending
            </span>
          </div>
          <h3 class="text-lg font-bold text-slate-900 dark:text-white mb-2">Held Sales</h3>
          <p class="text-sm text-slate-600 dark:text-slate-400">
            View all held transactions waiting to be completed or cancelled
          </p>
        </div>

        <!-- Voided Sales Report -->
        <div 
          @click="openReport('voided-sales')"
          class="bg-white dark:bg-slate-800 rounded-2xl p-6 shadow-lg border border-slate-200 dark:border-slate-700 hover:shadow-xl transition-all cursor-pointer group hover:-translate-y-1"
        >
          <div class="flex items-center justify-between mb-4">
            <div class="p-3 bg-gradient-to-br from-red-500 to-rose-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            <span class="px-3 py-1 bg-red-100 dark:bg-red-900/30 text-red-600 dark:text-red-400 rounded-full text-xs font-semibold">
              Cancelled
            </span>
          </div>
          <h3 class="text-lg font-bold text-slate-900 dark:text-white mb-2">Voided Sales</h3>
          <p class="text-sm text-slate-600 dark:text-slate-400">
            Review cancelled transactions with void reasons and timestamps
          </p>
        </div>

        <!-- Sales by Product Report -->
        <div 
          @click="openReport('by-product')"
          class="bg-white dark:bg-slate-800 rounded-2xl p-6 shadow-lg border border-slate-200 dark:border-slate-700 hover:shadow-xl transition-all cursor-pointer group hover:-translate-y-1"
        >
          <div class="flex items-center justify-between mb-4">
            <div class="p-3 bg-gradient-to-br from-purple-500 to-pink-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
              </svg>
            </div>
            <span class="px-3 py-1 bg-purple-100 dark:bg-purple-900/30 text-purple-600 dark:text-purple-400 rounded-full text-xs font-semibold">
              Products
            </span>
          </div>
          <h3 class="text-lg font-bold text-slate-900 dark:text-white mb-2">Sales by Product</h3>
          <p class="text-sm text-slate-600 dark:text-slate-400">
            Analyze sales performance and revenue by individual products
          </p>
        </div>

        <!-- Sales by Category Report -->
        <div 
          @click="openReport('by-category')"
          class="bg-white dark:bg-slate-800 rounded-2xl p-6 shadow-lg border border-slate-200 dark:border-slate-700 hover:shadow-xl transition-all cursor-pointer group hover:-translate-y-1"
        >
          <div class="flex items-center justify-between mb-4">
            <div class="p-3 bg-gradient-to-br from-teal-500 to-cyan-600 rounded-xl shadow-lg group-hover:scale-110 transition-transform">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" />
              </svg>
            </div>
            <span class="px-3 py-1 bg-teal-100 dark:bg-teal-900/30 text-teal-600 dark:text-teal-400 rounded-full text-xs font-semibold">
              Categories
            </span>
          </div>
          <h3 class="text-lg font-bold text-slate-900 dark:text-white mb-2">Sales by Category</h3>
          <p class="text-sm text-slate-600 dark:text-slate-400">
            View revenue distribution across product categories
          </p>
        </div>
      </div>
    </div>

    <!-- Report Modal -->
    <div v-if="showReportModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div class="bg-white dark:bg-slate-800 rounded-xl p-6 max-w-6xl w-full max-h-[90vh] overflow-y-auto">
        <div class="flex items-center justify-between mb-6">
          <h3 class="text-2xl font-semibold text-gray-900 dark:text-white">{{ currentReportTitle }}</h3>
          <button @click="closeReport" class="text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <!-- Report Content -->
        <div v-if="currentReport === 'daily'">
          <!-- Daily Sales Report Content -->
          <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
            <div class="bg-green-50 dark:bg-green-900/20 p-4 rounded-lg">
              <p class="text-sm text-gray-600 dark:text-gray-400">Total Sales</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">R{{ formatCurrency(todaysSales) }}</p>
              <p class="text-xs text-green-600 mt-1">{{ todaysTransactions }} transactions</p>
            </div>
            <div class="bg-blue-50 dark:bg-blue-900/20 p-4 rounded-lg">
              <p class="text-sm text-gray-600 dark:text-gray-400">Average Sale</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">R{{ formatCurrency(averageSale) }}</p>
              <p class="text-xs text-blue-600 mt-1">Per transaction</p>
            </div>
            <div class="bg-purple-50 dark:bg-purple-900/20 p-4 rounded-lg">
              <p class="text-sm text-gray-600 dark:text-gray-400">Cash Float</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">R{{ formatCurrency(cashFloat) }}</p>
              <p class="text-xs text-purple-600 mt-1">In drawer</p>
            </div>
          </div>

          <!-- Payment Methods -->
          <div class="mb-6">
            <h4 class="text-lg font-semibold text-gray-900 dark:text-white mb-3">Payment Methods</h4>
            <div class="grid grid-cols-2 md:grid-cols-4 gap-3">
              <div v-for="method in paymentMethods" :key="method.method" class="bg-gray-50 dark:bg-slate-700 p-3 rounded-lg">
                <p class="text-xs text-gray-600 dark:text-gray-400">{{ method.method }}</p>
                <p class="text-lg font-bold text-gray-900 dark:text-white">R{{ formatCurrency(method.total) }}</p>
                <p class="text-xs text-gray-600 dark:text-gray-400">{{ method.count }} transactions</p>
              </div>
            </div>
          </div>
        </div>

        <div v-else-if="currentReport === 'payment-methods'">
          <!-- Payment Methods Report -->
          <div class="space-y-4">
            <div v-for="method in paymentMethods" :key="method.method" class="bg-gray-50 dark:bg-slate-700 p-4 rounded-lg">
              <div class="flex items-center justify-between mb-2">
                <h4 class="text-lg font-semibold text-gray-900 dark:text-white">{{ method.method }}</h4>
                <span class="text-sm text-gray-600 dark:text-gray-400">{{ method.count }} transactions</span>
              </div>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">R{{ formatCurrency(method.total) }}</p>
              <div class="mt-2 w-full bg-gray-200 dark:bg-slate-600 rounded-full h-2">
                <div 
                  class="bg-blue-600 h-2 rounded-full" 
                  :style="{ width: ((method.total / todaysSales) * 100) + '%' }"
                ></div>
              </div>
              <p class="text-xs text-gray-600 dark:text-gray-400 mt-1">
                {{ ((method.total / todaysSales) * 100).toFixed(1) }}% of total sales
              </p>
            </div>
          </div>
        </div>

        <div v-else-if="currentReport === 'held-sales'">
          <!-- Held Sales Report -->
          <div v-if="heldSales.length === 0" class="bg-gray-50 dark:bg-slate-700 p-8 rounded-lg text-center">
            <p class="text-gray-500 dark:text-gray-400">No held sales at the moment</p>
          </div>
          <div v-else class="space-y-3">
            <div 
              v-for="sale in heldSales" 
              :key="sale.id"
              class="bg-orange-50 dark:bg-orange-900/20 p-4 rounded-lg border border-orange-200 dark:border-orange-800"
            >
              <div class="flex items-center justify-between">
                <div>
                  <p class="font-semibold text-gray-900 dark:text-white">Receipt #{{ sale.receiptNumber || sale.id }}</p>
                  <p class="text-sm text-gray-600 dark:text-gray-400">{{ new Date(sale.createdAt).toLocaleString() }}</p>
                </div>
                <p class="text-xl font-bold text-orange-600">R{{ formatCurrency(sale.totalAmount) }}</p>
              </div>
            </div>
          </div>
        </div>

        <div v-else-if="currentReport === 'voided-sales'">
          <!-- Voided Sales Report -->
          <div v-if="voidedSales.length === 0" class="bg-gray-50 dark:bg-slate-700 p-8 rounded-lg text-center">
            <p class="text-gray-500 dark:text-gray-400">No voided sales today</p>
          </div>
          <div v-else class="space-y-3">
            <div 
              v-for="sale in voidedSales" 
              :key="sale.id"
              class="bg-red-50 dark:bg-red-900/20 p-4 rounded-lg border border-red-200 dark:border-red-800"
            >
              <div class="flex items-center justify-between mb-2">
                <div>
                  <p class="font-semibold text-gray-900 dark:text-white">Receipt #{{ sale.receiptNumber || sale.id }}</p>
                  <p class="text-sm text-gray-600 dark:text-gray-400">{{ new Date(sale.voidedAt).toLocaleString() }}</p>
                </div>
                <p class="text-xl font-bold text-red-600">R{{ formatCurrency(sale.totalAmount) }}</p>
              </div>
              <p class="text-sm text-gray-600 dark:text-gray-400">
                <span class="font-semibold">Reason:</span> {{ sale.voidReason || 'Not specified' }}
              </p>
            </div>
          </div>
        </div>

        <div v-else-if="currentReport === 'by-product'">
          <!-- Sales by Product Report -->
          <div class="space-y-3">
            <div 
              v-for="product in topProducts" 
              :key="product.name"
              class="bg-gray-50 dark:bg-slate-700 p-4 rounded-lg"
            >
              <div class="flex items-center justify-between mb-2">
                <h4 class="font-semibold text-gray-900 dark:text-white">{{ product.name }}</h4>
                <span class="text-sm text-gray-600 dark:text-gray-400">{{ product.quantity }} units</span>
              </div>
              <div class="w-full bg-gray-200 dark:bg-slate-600 rounded-full h-2">
                <div 
                  class="bg-purple-600 h-2 rounded-full" 
                  :style="{ width: ((product.quantity / Math.max(...topProducts.map(p => p.quantity))) * 100) + '%' }"
                ></div>
              </div>
            </div>
          </div>
        </div>

        <div v-else-if="currentReport === 'by-category'">
          <!-- Sales by Category Report -->
          <div class="space-y-3">
            <div 
              v-for="category in categorySales" 
              :key="category.name"
              class="bg-gray-50 dark:bg-slate-700 p-4 rounded-lg"
            >
              <div class="flex items-center justify-between mb-2">
                <h4 class="font-semibold text-gray-900 dark:text-white">{{ category.name }}</h4>
                <span class="text-lg font-bold text-gray-900 dark:text-white">R{{ formatCurrency(category.total) }}</span>
              </div>
              <div class="w-full bg-gray-200 dark:bg-slate-600 rounded-full h-2">
                <div 
                  class="bg-teal-600 h-2 rounded-full" 
                  :style="{ width: category.percentage + '%' }"
                ></div>
              </div>
              <p class="text-xs text-gray-600 dark:text-gray-400 mt-1">{{ category.percentage }}% of total</p>
            </div>
          </div>
        </div>

        <!-- Action Buttons -->
        <div class="flex justify-end gap-3 border-t dark:border-slate-700 pt-4 mt-6">
          <button @click="printReport" class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg font-medium transition-colors">
            Print Report
          </button>
          <button @click="closeReport" class="px-4 py-2 bg-gray-600 hover:bg-gray-700 text-white rounded-lg font-medium transition-colors">
            Close
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useSalesAPI } from '~/composables/useSalesAPI'
import { useDashboard } from '~/composables/useDashboard'
import { logError } from '~/utils/errorHandler'

// Page metadata
useHead({
  title: 'Sales Reports - TOSS ERP',
  meta: [
    { name: 'description', content: 'Comprehensive sales reports and analytics' }
  ]
})

// API
const salesAPI = useSalesAPI()
const dashboardAPI = useDashboard()

// State
const showReportModal = ref(false)
const currentReport = ref('')
const todaysSales = ref(0)
const todaysTransactions = ref(0)
const averageSale = ref(0)
const cashFloat = ref(2500)
const heldSales = ref<any[]>([])
const voidedSales = ref<any[]>([])
const paymentMethods = ref<any[]>([])
const topProducts = ref<any[]>([])
const categorySales = ref<any[]>([])

// Computed
const heldSalesCount = computed(() => heldSales.value.length)
const heldSalesTotal = computed(() => 
  heldSales.value.reduce((sum, sale) => sum + (sale.totalAmount || 0), 0)
)
const voidedSalesCount = computed(() => voidedSales.value.length)
const voidedSalesTotal = computed(() => 
  voidedSales.value.reduce((sum, sale) => sum + (sale.totalAmount || 0), 0)
)

const currentReportTitle = computed(() => {
  const titles: Record<string, string> = {
    'daily': 'Daily Sales Report',
    'payment-methods': 'Payment Methods Report',
    'held-sales': 'Held Sales Report',
    'voided-sales': 'Voided Sales Report',
    'by-product': 'Sales by Product Report',
    'by-category': 'Sales by Category Report'
  }
  return titles[currentReport.value] || 'Report'
})

// Load data on mount
onMounted(async () => {
  await loadReportsData()
})

// Load all report data
const loadReportsData = async () => {
  try {
    const shopId = 1 // TODO: Get from session/auth

    // Get daily summary
  const summary = await salesAPI.getDailySummary(shopId)
  // Align with backend DailySummaryDto fields: TotalSales (sum), SaleCount (count), AverageSaleValue
  todaysSales.value = summary.totalSales || 0
  todaysTransactions.value = summary.saleCount || 0
  averageSale.value = summary.averageSaleValue || 0

    // Get held sales
    heldSales.value = await salesAPI.getHeldSales(shopId)

    // Get voided sales
    voidedSales.value = await salesAPI.getVoidedSales(shopId)

    // Get payment methods breakdown
    const salesData = await salesAPI.getSales({ shopId })
    const sales = salesData.items || salesData || []
    const paymentBreakdown = sales.reduce((acc: any, sale: any) => {
      const method = sale.paymentMethod || 'Cash'
      if (!acc[method]) {
        acc[method] = { count: 0, total: 0 }
      }
      acc[method].count++
      acc[method].total += sale.totalAmount || 0
      return acc
    }, {})

    paymentMethods.value = Object.entries(paymentBreakdown).map(([method, data]: [string, any]) => ({
      method,
      count: data.count,
      total: data.total
    }))

    // Get top products
    const topProductsData = await dashboardAPI.getTopProducts({ shopId, limit: 10 })
    topProducts.value = topProductsData.map((p: any) => ({
      name: p.productName,
      quantity: p.quantitySold
    }))

    // Get category sales
    const endDate = new Date()
    const startDate = new Date()
    startDate.setDate(startDate.getDate() - 30)
    const categoryData = await dashboardAPI.getCategorySales({
      shopId,
      startDate: startDate.toISOString(),
      endDate: endDate.toISOString()
    })
    categorySales.value = categoryData.map((c: any) => ({
      name: c.categoryName,
      total: Number(c.totalSales),
      percentage: Number(c.percentage)
    }))

  } catch (error) {
    logError(error, 'load_reports_data', 'Failed to load reports data')
  }
}

// Functions
const openReport = (reportType: string) => {
  currentReport.value = reportType
  showReportModal.value = true
}

const closeReport = () => {
  showReportModal.value = false
  currentReport.value = ''
}

const printReport = () => {
  window.print()
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}
</script>
