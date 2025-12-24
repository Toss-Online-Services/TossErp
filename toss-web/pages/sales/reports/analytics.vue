<template>
  <div class="analytics-dashboard-page">
    <!-- Header -->
    <div class="page-header mb-6">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-3xl font-bold text-gray-900 dark:text-white flex items-center">
            <Icon name="mdi:chart-line" class="mr-3 text-primary" />
            {{ t('analytics.title') }}
          </h1>
          <p class="mt-2 text-sm text-gray-600 dark:text-gray-400">
            {{ t('analytics.subtitle') }}
          </p>
        </div>
        <div class="flex space-x-3">
          <button
            @click="refreshData"
            class="btn btn-secondary"
            :disabled="loading"
          >
            <Icon :name="loading ? 'mdi:loading' : 'mdi:refresh'" :class="{ 'animate-spin': loading }" class="mr-2" />
            {{ t('common.refresh') }}
          </button>
          <button
            @click="exportAnalytics"
            class="btn btn-primary"
          >
            <Icon name="mdi:download" class="mr-2" />
            {{ t('analytics.actions.export') }}
          </button>
        </div>
      </div>
    </div>

    <!-- Date Range Selector -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6 mb-6">
      <div class="grid grid-cols-1 md:grid-cols-5 gap-4">
        <FormKit
          type="date"
          name="startDate"
          :label="t('analytics.filters.startDate')"
          v-model="filters.startDate"
          @input="fetchData"
        />
        <FormKit
          type="date"
          name="endDate"
          :label="t('analytics.filters.endDate')"
          v-model="filters.endDate"
          @input="fetchData"
        />
        <FormKit
          type="select"
          name="groupBy"
          :label="t('analytics.filters.groupBy')"
          v-model="filters.groupBy"
          :options="groupByOptions"
          @input="fetchData"
        />
        <FormKit
          type="select"
          name="territory"
          :label="t('analytics.filters.territory')"
          v-model="filters.territory"
          :options="territoryOptions"
          @input="fetchData"
        />
        <FormKit
          type="select"
          name="category"
          :label="t('analytics.filters.category')"
          v-model="filters.category"
          :options="categoryOptions"
          @input="fetchData"
        />
      </div>
    </div>

    <!-- Key Metrics Cards -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-6">
      <MetricCard
        :title="t('analytics.metrics.totalRevenue')"
        :value="formatCurrency(metrics.totalRevenue)"
        :change="metrics.revenueGrowth"
        icon="mdi:currency-usd"
        color="blue"
        :loading="loading"
      />
      <MetricCard
        :title="t('analytics.metrics.totalOrders')"
        :value="metrics.totalOrders?.toString() || '0'"
        :change="metrics.ordersGrowth"
        icon="mdi:cart"
        color="green"
        :loading="loading"
      />
      <MetricCard
        :title="t('analytics.metrics.averageOrderValue')"
        :value="formatCurrency(metrics.averageOrderValue)"
        :change="metrics.aovGrowth"
        icon="mdi:chart-areaspline"
        color="purple"
        :loading="loading"
      />
      <MetricCard
        :title="t('analytics.metrics.customerRetention')"
        :value="`${metrics.customerRetentionRate?.toFixed(1) || '0'}%`"
        :change="metrics.retentionGrowth"
        icon="mdi:account-multiple"
        color="orange"
        :loading="loading"
      />
    </div>

    <!-- Sales Trends Chart -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6 mb-6">
      <div class="flex items-center justify-between mb-6">
        <h2 class="text-xl font-bold text-gray-900 dark:text-white flex items-center">
          <Icon name="mdi:chart-timeline-variant" class="mr-2 text-primary" />
          {{ t('analytics.charts.salesTrends') }}
        </h2>
        <div class="flex space-x-2">
          <button
            v-for="view in ['revenue', 'orders', 'aov']"
            :key="view"
            @click="trendView = view"
            :class="[
              'px-3 py-1 rounded-lg text-sm font-medium transition-colors',
              trendView === view
                ? 'bg-blue-600 text-white'
                : 'bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-600'
            ]"
          >
            {{ t(`analytics.views.${view}`) }}
          </button>
        </div>
      </div>

      <div v-if="loading" class="h-80 flex items-center justify-center">
        <Icon name="mdi:loading" class="w-12 h-12 animate-spin text-primary" />
      </div>

      <canvas v-else id="salesTrendChart" class="w-full h-80"></canvas>
    </div>

    <!-- Product Performance & Customer Analytics -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
      <!-- Top Products -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
        <h2 class="text-xl font-bold text-gray-900 dark:text-white mb-6 flex items-center">
          <Icon name="mdi:trophy" class="mr-2 text-yellow-600" />
          {{ t('analytics.sections.topProducts') }}
        </h2>

        <div v-if="loading" class="flex justify-center py-12">
          <Icon name="mdi:loading" class="w-8 h-8 animate-spin text-primary" />
        </div>

        <div v-else class="space-y-4">
          <div
            v-for="(product, index) in topProducts.slice(0, 5)"
            :key="product.productId"
            class="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-900 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors"
          >
            <div class="flex items-center space-x-3">
              <div :class="[
                'flex items-center justify-center w-8 h-8 rounded-full text-white font-bold text-sm',
                index === 0 ? 'bg-yellow-500' : index === 1 ? 'bg-gray-400' : index === 2 ? 'bg-orange-600' : 'bg-gray-300'
              ]">
                {{ index + 1 }}
              </div>
              <div>
                <p class="font-semibold text-gray-900 dark:text-white">{{ product.productName }}</p>
                <p class="text-sm text-gray-600 dark:text-gray-400">
                  {{ product.quantitySold }} {{ t('analytics.labels.unitsSold') }}
                </p>
              </div>
            </div>
            <div class="text-right">
              <p class="font-bold text-gray-900 dark:text-white">{{ formatCurrency(product.revenue) }}</p>
              <p class="text-xs text-green-600 dark:text-green-400">
                {{ formatPercentage(product.margin) }} {{ t('analytics.labels.margin') }}
              </p>
            </div>
          </div>
        </div>
      </div>

      <!-- Customer Segments -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
        <h2 class="text-xl font-bold text-gray-900 dark:text-white mb-6 flex items-center">
          <Icon name="mdi:account-group" class="mr-2 text-primary" />
          {{ t('analytics.sections.customerSegments') }}
        </h2>

        <div v-if="loading" class="flex justify-center py-12">
          <Icon name="mdi:loading" class="w-8 h-8 animate-spin text-primary" />
        </div>

        <div v-else>
          <canvas id="customerSegmentChart" class="w-full h-64"></canvas>

          <div class="mt-6 grid grid-cols-2 gap-4">
            <div class="text-center p-3 bg-primary/10 rounded-lg">
              <p class="text-2xl font-bold text-primary">{{ customerAnalytics.newCustomers || 0 }}</p>
              <p class="text-sm text-gray-600 dark:text-gray-400">{{ t('analytics.labels.newCustomers') }}</p>
            </div>
            <div class="text-center p-3 bg-green-50 dark:bg-green-900/20 rounded-lg">
              <p class="text-2xl font-bold text-green-600">{{ customerAnalytics.returningCustomers || 0 }}</p>
              <p class="text-sm text-gray-600 dark:text-gray-400">{{ t('analytics.labels.returningCustomers') }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Sales by Category & Payment Methods -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
      <!-- Sales by Category -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
        <h2 class="text-xl font-bold text-gray-900 dark:text-white mb-6 flex items-center">
          <Icon name="mdi:tag-multiple" class="mr-2 text-green-600" />
          {{ t('analytics.sections.salesByCategory') }}
        </h2>

        <div v-if="loading" class="flex justify-center py-12">
          <Icon name="mdi:loading" class="w-8 h-8 animate-spin text-primary" />
        </div>

        <canvas v-else id="categoryChart" class="w-full h-64"></canvas>
      </div>

      <!-- Payment Methods -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
        <h2 class="text-xl font-bold text-gray-900 dark:text-white mb-6 flex items-center">
          <Icon name="mdi:credit-card-multiple" class="mr-2 text-primary" />
          {{ t('analytics.sections.paymentMethods') }}
        </h2>

        <div v-if="loading" class="flex justify-center py-12">
          <Icon name="mdi:loading" class="w-8 h-8 animate-spin text-primary" />
        </div>

        <div v-else class="space-y-3">
          <div
            v-for="method in paymentMethodBreakdown"
            :key="method.method"
            class="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-900 rounded-lg"
          >
            <div class="flex items-center space-x-3">
              <Icon :name="getPaymentIcon(method.method)" class="text-2xl text-gray-600" />
              <span class="font-medium text-gray-900 dark:text-white">
                {{ t(`analytics.paymentMethods.${method.method}`) }}
              </span>
            </div>
            <div class="text-right">
              <p class="font-bold text-gray-900 dark:text-white">{{ formatCurrency(method.amount) }}</p>
              <p class="text-xs text-gray-600 dark:text-gray-400">
                {{ formatPercentage(method.percentage) }}
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Sales Forecast -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
      <div class="flex items-center justify-between mb-6">
        <h2 class="text-xl font-bold text-gray-900 dark:text-white flex items-center">
          <Icon name="mdi:chart-box-outline" class="mr-2 text-indigo-600" />
          {{ t('analytics.sections.salesForecast') }}
        </h2>
        <button
          @click="generateForecast"
          class="btn btn-primary"
          :disabled="loadingForecast"
        >
          <Icon :name="loadingForecast ? 'mdi:loading' : 'mdi:crystal-ball'" :class="{ 'animate-spin': loadingForecast }" class="mr-2" />
          {{ t('analytics.actions.generateForecast') }}
        </button>
      </div>

      <div v-if="loadingForecast" class="h-80 flex items-center justify-center">
        <Icon name="mdi:loading" class="w-12 h-12 animate-spin text-primary" />
      </div>

      <canvas v-else-if="forecast" id="forecastChart" class="w-full h-80"></canvas>

      <div v-else class="h-80 flex flex-col items-center justify-center text-gray-400">
        <Icon name="mdi:chart-line-variant" class="w-16 h-16 mb-4" />
        <p>{{ t('analytics.messages.noForecast') }}</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useSalesAnalytics } from '~/composables/useSalesAnalytics'
import { useI18n } from 'vue-i18n'
import { Chart, registerables } from 'chart.js'

Chart.register(...registerables)

const { t } = useI18n()
const {
  metrics,
  trends,
  productPerformance,
  customerAnalytics,
  loading,
  error,
  fetchMetrics,
  fetchTrends,
  fetchProductPerformance,
  fetchCustomerAnalytics,
  generateForecast: generateForecastData,
  exportAnalytics: exportData
} = useSalesAnalytics()

// Filters
const filters = ref({
  startDate: getDefaultStartDate(),
  endDate: getDefaultEndDate(),
  groupBy: 'day',
  territory: '',
  category: ''
})

const trendView = ref('revenue')
const loadingForecast = ref(false)
const forecast = ref<any>(null)

// Charts
let salesTrendChart: Chart | null = null
let categoryChart: Chart | null = null
let customerSegmentChart: Chart | null = null
let forecastChart: Chart | null = null

// Computed
const topProducts = computed(() => productPerformance.value?.slice(0, 10) || [])

const paymentMethodBreakdown = computed(() => {
  const total = metrics.value?.totalRevenue || 0
  return [
    { method: 'cash', amount: total * 0.45, percentage: 45 },
    { method: 'card', amount: total * 0.30, percentage: 30 },
    { method: 'mobile', amount: total * 0.20, percentage: 20 },
    { method: 'credit', amount: total * 0.05, percentage: 5 }
  ]
})

// Options
const groupByOptions = [
  { value: 'day', label: t('analytics.groupBy.day') },
  { value: 'week', label: t('analytics.groupBy.week') },
  { value: 'month', label: t('analytics.groupBy.month') }
]

const territoryOptions = [
  { value: '', label: t('common.all') },
  { value: 'soweto', label: 'Soweto' },
  { value: 'alexandra', label: 'Alexandra' },
  { value: 'khayelitsha', label: 'Khayelitsha' }
]

const categoryOptions = [
  { value: '', label: t('common.all') },
  { value: 'groceries', label: t('categories.groceries') },
  { value: 'beverages', label: t('categories.beverages') },
  { value: 'household', label: t('categories.household') }
]

// Methods
function getDefaultStartDate() {
  const date = new Date()
  date.setDate(date.getDate() - 30)
  return date.toISOString().split('T')[0]
}

function getDefaultEndDate() {
  return new Date().toISOString().split('T')[0]
}

const fetchData = async () => {
  await Promise.all([
    fetchMetrics(filters.value),
    fetchTrends(filters.value),
    fetchProductPerformance(filters.value),
    fetchCustomerAnalytics(filters.value)
  ])

  nextTick(() => {
    renderCharts()
  })
}

const refreshData = async () => {
  await fetchData()
}

const generateForecast = async () => {
  loadingForecast.value = true
  try {
    forecast.value = await generateForecastData(30)
    nextTick(() => {
      renderForecastChart()
    })
  } finally {
    loadingForecast.value = false
  }
}

const exportAnalytics = async () => {
  await exportData('csv', filters.value)
}

const renderCharts = () => {
  renderSalesTrendChart()
  renderCategoryChart()
  renderCustomerSegmentChart()
}

const renderSalesTrendChart = () => {
  const canvas = document.getElementById('salesTrendChart') as HTMLCanvasElement
  if (!canvas) return

  if (salesTrendChart) salesTrendChart.destroy()

  const trendData = trends.value || []
  const labels = trendData.map(t => t.date)
  
  let datasetData: number[]
  let label: string
  
  switch (trendView.value) {
    case 'revenue':
      datasetData = trendData.map(t => t.revenue)
      label = t('analytics.metrics.revenue')
      break
    case 'orders':
      datasetData = trendData.map(t => t.orders)
      label = t('analytics.metrics.orders')
      break
    case 'aov':
      datasetData = trendData.map(t => t.averageOrderValue)
      label = t('analytics.metrics.averageOrderValue')
      break
    default:
      datasetData = trendData.map(t => t.revenue)
      label = t('analytics.metrics.revenue')
  }

  salesTrendChart = new Chart(canvas, {
    type: 'line',
    data: {
      labels,
      datasets: [{
        label,
        data: datasetData,
        borderColor: 'rgb(59, 130, 246)',
        backgroundColor: 'rgba(59, 130, 246, 0.1)',
        tension: 0.4,
        fill: true
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          display: false
        }
      },
      scales: {
        y: {
          beginAtZero: true
        }
      }
    }
  })
}

const renderCategoryChart = () => {
  const canvas = document.getElementById('categoryChart') as HTMLCanvasElement
  if (!canvas) return

  if (categoryChart) categoryChart.destroy()

  categoryChart = new Chart(canvas, {
    type: 'doughnut',
    data: {
      labels: [t('categories.groceries'), t('categories.beverages'), t('categories.household'), t('categories.other')],
      datasets: [{
        data: [40, 25, 20, 15],
        backgroundColor: [
          'rgb(59, 130, 246)',
          'rgb(16, 185, 129)',
          'rgb(245, 158, 11)',
          'rgb(156, 163, 175)'
        ]
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: 'bottom'
        }
      }
    }
  })
}

const renderCustomerSegmentChart = () => {
  const canvas = document.getElementById('customerSegmentChart') as HTMLCanvasElement
  if (!canvas) return

  if (customerSegmentChart) customerSegmentChart.destroy()

  customerSegmentChart = new Chart(canvas, {
    type: 'pie',
    data: {
      labels: [t('analytics.segments.new'), t('analytics.segments.returning'), t('analytics.segments.inactive')],
      datasets: [{
        data: [
          customerAnalytics.value?.newCustomers || 0,
          customerAnalytics.value?.returningCustomers || 0,
          customerAnalytics.value?.inactiveCustomers || 0
        ],
        backgroundColor: [
          'rgb(59, 130, 246)',
          'rgb(16, 185, 129)',
          'rgb(239, 68, 68)'
        ]
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: 'bottom'
        }
      }
    }
  })
}

const renderForecastChart = () => {
  const canvas = document.getElementById('forecastChart') as HTMLCanvasElement
  if (!canvas || !forecast.value) return

  if (forecastChart) forecastChart.destroy()

  forecastChart = new Chart(canvas, {
    type: 'line',
    data: {
      labels: forecast.value.dates,
      datasets: [{
        label: t('analytics.labels.forecast'),
        data: forecast.value.values,
        borderColor: 'rgb(99, 102, 241)',
        backgroundColor: 'rgba(99, 102, 241, 0.1)',
        borderDash: [5, 5],
        tension: 0.4,
        fill: true
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          display: false
        }
      },
      scales: {
        y: {
          beginAtZero: true
        }
      }
    }
  })
}

const getPaymentIcon = (method: string) => {
  const icons: Record<string, string> = {
    cash: 'mdi:cash',
    card: 'mdi:credit-card',
    mobile: 'mdi:cellphone',
    credit: 'mdi:account-credit-outline'
  }
  return icons[method] || 'mdi:help-circle'
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount || 0)
}

const formatPercentage = (value: number) => {
  return `${(value || 0).toFixed(1)}%`
}

// Watch trend view changes
watch(trendView, () => {
  renderSalesTrendChart()
})

// Load data on mount
onMounted(() => {
  fetchData()
})
</script>

<style scoped>
.analytics-dashboard-page {
  max-width: 1400px;
  margin: 0 auto;
  padding: 2rem 1rem;
}

.btn {
  @apply px-4 py-2 rounded-lg font-medium transition-colors flex items-center;
}

.btn-primary {
  @apply bg-primary text-primary-foreground hover:bg-primary/90;
}

.btn-secondary {
  @apply bg-gray-200 text-gray-800 hover:bg-gray-300 dark:bg-gray-700 dark:text-gray-200 dark:hover:bg-gray-600;
}

.btn:disabled {
  @apply opacity-50 cursor-not-allowed;
}
</style>
