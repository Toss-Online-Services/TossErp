<template>
  <div class="space-y-6">
    <!-- Stats Cards Row 1 -->
    <div class="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-4">
      <MaterialStatsCard
        title="Bookings"
        :value="281"
        change="+55%"
        change-type="positive"
        subtitle="than last week"
        icon="weekend"
      />
      <MaterialStatsCard
        title="Today's Users"
        :value="2300"
        change="+3%"
        change-type="positive"
        subtitle="than last month"
        icon="leaderboard"
      />
      <MaterialStatsCard
        title="Revenue"
        value="R34,000"
        change="+35%"
        change-type="positive"
        subtitle="than last month"
        icon="store"
      />
      <MaterialStatsCard
        title="Followers"
        value="+2,910"
        change="Just updated"
        change-type="neutral"
        icon="person_add"
      />
    </div>

    <!-- Charts Row -->
    <div class="grid grid-cols-1 gap-6 lg:grid-cols-3">
      <!-- Website Views Chart -->
      <MaterialCard variant="elevated" class="p-6">
        <div class="mb-4">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Website Views</h3>
          <p class="text-sm text-gray-600 dark:text-gray-400">Last Campaign Performance</p>
        </div>
        <div class="h-64">
          <canvas ref="websiteViewsChart" />
        </div>
        <div class="mt-4 flex items-center text-xs text-gray-500 dark:text-gray-400">
          <ClockIcon class="w-4 h-4 mr-1" />
          <span>campaign sent 2 days ago</span>
        </div>
      </MaterialCard>

      <!-- Daily Sales Chart -->
      <MaterialCard variant="elevated" class="p-6">
        <div class="mb-4">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Daily Sales</h3>
          <p class="text-sm text-green-600 dark:text-green-400 font-medium">(+15%) increase in today sales.</p>
        </div>
        <div class="h-64">
          <canvas ref="dailySalesChart" />
        </div>
        <div class="mt-4 flex items-center text-xs text-gray-500 dark:text-gray-400">
          <ClockIcon class="w-4 h-4 mr-1" />
          <span>updated 4 min ago</span>
        </div>
      </MaterialCard>

      <!-- Completed Tasks Chart -->
      <MaterialCard variant="elevated" class="p-6">
        <div class="mb-4">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Completed Tasks</h3>
          <p class="text-sm text-gray-600 dark:text-gray-400">Last Campaign Performance</p>
        </div>
        <div class="h-64">
          <canvas ref="completedTasksChart" />
        </div>
        <div class="mt-4 flex items-center text-xs text-gray-500 dark:text-gray-400">
          <ClockIcon class="w-4 h-4 mr-1" />
          <span>just updated</span>
        </div>
      </MaterialCard>
    </div>

    <!-- Orders by Status & Sales by Country -->
    <div class="grid grid-cols-1 gap-6 lg:grid-cols-2">
      <!-- Orders by Status -->
      <MaterialCard variant="elevated" class="p-6">
        <div class="mb-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Orders by Status</h3>
          <p class="text-sm text-gray-600 dark:text-gray-400">Current order distribution</p>
        </div>
        <div class="grid grid-cols-2 gap-4 md:grid-cols-5">
          <div
            v-for="(count, status) in stats.ordersByStatus"
            :key="status"
            class="text-center p-4 bg-gray-50 dark:bg-gray-700/50 rounded-lg"
          >
            <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ count }}</p>
            <p class="text-xs text-gray-600 dark:text-gray-400 mt-1">{{ status }}</p>
          </div>
        </div>
        <div class="mt-4">
          <NuxtLink
            to="/admin/orders"
            class="text-sm text-indigo-600 dark:text-indigo-400 hover:text-indigo-700 dark:hover:text-indigo-300 font-medium"
          >
            View all orders →
          </NuxtLink>
        </div>
      </MaterialCard>

      <!-- Sales by Country -->
      <MaterialCard variant="elevated" class="p-6">
        <div class="mb-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Sales by Country</h3>
          <p class="text-sm text-gray-600 dark:text-gray-400">Check the sales, value and bounce rate by country.</p>
        </div>
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead>
              <tr class="border-b border-gray-200 dark:border-gray-700">
                <th class="text-left py-3 px-4 text-xs font-semibold text-gray-600 dark:text-gray-400 uppercase tracking-wider">Country</th>
                <th class="text-right py-3 px-4 text-xs font-semibold text-gray-600 dark:text-gray-400 uppercase tracking-wider">Sales</th>
                <th class="text-right py-3 px-4 text-xs font-semibold text-gray-600 dark:text-gray-400 uppercase tracking-wider">Value</th>
                <th class="text-right py-3 px-4 text-xs font-semibold text-gray-600 dark:text-gray-400 uppercase tracking-wider">Bounce</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
              <tr v-for="country in salesByCountry" :key="country.name" class="hover:bg-gray-50 dark:hover:bg-gray-700/50">
                <td class="py-3 px-4">
                  <div class="flex items-center space-x-2">
                    <span class="text-lg">{{ country.flag }}</span>
                    <span class="text-sm font-medium text-gray-900 dark:text-white">{{ country.name }}</span>
                  </div>
                </td>
                <td class="py-3 px-4 text-right text-sm text-gray-900 dark:text-white">{{ country.sales.toLocaleString() }}</td>
                <td class="py-3 px-4 text-right text-sm text-gray-900 dark:text-white">{{ formatCurrency(country.value) }}</td>
                <td class="py-3 px-4 text-right text-sm text-gray-900 dark:text-white">{{ country.bounce }}%</td>
              </tr>
            </tbody>
          </table>
        </div>
      </MaterialCard>
    </div>

    <!-- System Stats Grid -->
    <div class="grid grid-cols-1 gap-6 md:grid-cols-3">
      <!-- Active Retailers -->
      <MaterialCard variant="elevated" class="p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Active Retailers</p>
            <p class="text-3xl font-bold text-gray-900 dark:text-white mt-2">{{ stats.activeRetailers }}</p>
            <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">Registered shops</p>
          </div>
          <div class="p-3 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-xl">
            <ShoppingCart class="w-8 h-8 text-white" />
          </div>
        </div>
      </MaterialCard>

      <!-- Active Suppliers -->
      <MaterialCard variant="elevated" class="p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Active Suppliers</p>
            <p class="text-3xl font-bold text-gray-900 dark:text-white mt-2">{{ stats.activeSuppliers }}</p>
            <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">Partner suppliers</p>
          </div>
          <div class="p-3 bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl">
            <Users class="w-8 h-8 text-white" />
          </div>
        </div>
      </MaterialCard>

      <!-- Active Drivers -->
      <MaterialCard variant="elevated" class="p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Active Drivers</p>
            <p class="text-3xl font-bold text-gray-900 dark:text-white mt-2">{{ stats.activeDrivers }}</p>
            <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">Delivery drivers</p>
          </div>
          <div class="p-3 bg-gradient-to-br from-orange-500 to-red-600 rounded-xl">
            <Truck class="w-8 h-8 text-white" />
          </div>
        </div>
      </MaterialCard>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from 'vue'
import { useApi } from '~/composables/useApi'
import { ShoppingCart, Users, Truck, Clock as ClockIcon } from 'lucide-vue-next'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  Title,
  Tooltip,
  Legend,
  Filler
} from 'chart.js'

definePageMeta({
  layout: 'default',
  middleware: 'auth',
  meta: {
    roles: ['Administrator'],
    role: 'admin',
    title: 'Admin Dashboard',
    subtitle: 'Overview of system activity and statistics'
  }
})

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  Title,
  Tooltip,
  Legend,
  Filler
)

const { get } = useApi()
const isLoading = ref(true)
const stats = ref({
  activeRetailers: 0,
  activeSuppliers: 0,
  activeDrivers: 0,
  totalSalesToday: 0,
  ordersByStatus: {
    'Draft': 0,
    'Submitted': 0,
    'Accepted': 0,
    'Shipped': 0,
    'Delivered': 0
  }
})

const salesByCountry = ref([
  { name: 'United States', flag: '🇺🇸', sales: 2500, value: 230900, bounce: 29.9 },
  { name: 'Germany', flag: '🇩🇪', sales: 3900, value: 440000, bounce: 40.22 },
  { name: 'Great Britain', flag: '🇬🇧', sales: 1400, value: 190700, bounce: 23.44 },
  { name: 'Brasil', flag: '🇧🇷', sales: 562, value: 143960, bounce: 32.14 }
])

const websiteViewsChart = ref<HTMLCanvasElement | null>(null)
const dailySalesChart = ref<HTMLCanvasElement | null>(null)
const completedTasksChart = ref<HTMLCanvasElement | null>(null)

let chartInstances: any[] = []

const loadStats = async () => {
  isLoading.value = true
  try {
    const users = await get('/api/users/list?take=1000')
    const retailers = users.filter((u: any) => u.roles?.includes('StoreOwner') || u.roles?.includes('Vendor'))
    const suppliers = users.filter((u: any) => u.roles?.includes('Supplier'))
    const drivers = users.filter((u: any) => u.roles?.includes('Driver'))

    stats.value = {
      activeRetailers: retailers.length,
      activeSuppliers: suppliers.length,
      activeDrivers: drivers.length,
      totalSalesToday: 0,
      ordersByStatus: {
        'Draft': 0,
        'Submitted': 0,
        'Accepted': 0,
        'Shipped': 0,
        'Delivered': 0
      }
    }
  } catch (error) {
    console.error('Error loading stats:', error)
  } finally {
    isLoading.value = false
  }
}

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(value)
}

const createCharts = () => {
  // Website Views Chart (Bar Chart)
  if (websiteViewsChart.value) {
    const ctx = websiteViewsChart.value.getContext('2d')
    if (ctx) {
      const chart = new ChartJS(ctx, {
        type: 'bar',
        data: {
          labels: ['M', 'T', 'W', 'T', 'F', 'S', 'S'],
          datasets: [{
            label: 'Views',
            data: [50, 45, 25, 30, 45, 55, 65],
            backgroundColor: 'rgba(99, 102, 241, 0.8)',
            borderColor: 'rgba(99, 102, 241, 1)',
            borderWidth: 1,
            borderRadius: 8
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
              beginAtZero: true,
              grid: {
                color: 'rgba(0, 0, 0, 0.05)'
              }
            },
            x: {
              grid: {
                display: false
              }
            }
          }
        }
      })
      chartInstances.push(chart)
    }
  }

  // Daily Sales Chart (Line Chart)
  if (dailySalesChart.value) {
    const ctx = dailySalesChart.value.getContext('2d')
    if (ctx) {
      const chart = new ChartJS(ctx, {
        type: 'line',
        data: {
          labels: ['J', 'F', 'M', 'A', 'M', 'J', 'J', 'A', 'S', 'O', 'N', 'D'],
          datasets: [{
            label: 'Sales',
            data: [200, 250, 300, 400, 350, 380, 400, 350, 300, 320, 300, 300],
            borderColor: 'rgba(34, 197, 94, 1)',
            backgroundColor: 'rgba(34, 197, 94, 0.1)',
            fill: true,
            tension: 0.4,
            borderWidth: 2
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
              beginAtZero: true,
              grid: {
                color: 'rgba(0, 0, 0, 0.05)'
              }
            },
            x: {
              grid: {
                display: false
              }
            }
          }
        }
      })
      chartInstances.push(chart)
    }
  }

  // Completed Tasks Chart (Line Chart)
  if (completedTasksChart.value) {
    const ctx = completedTasksChart.value.getContext('2d')
    if (ctx) {
      const chart = new ChartJS(ctx, {
        type: 'line',
        data: {
          labels: ['Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
          datasets: [{
            label: 'Tasks',
            data: [300, 350, 400, 450, 500, 450, 400, 450, 500],
            borderColor: 'rgba(99, 102, 241, 1)',
            backgroundColor: 'rgba(99, 102, 241, 0.1)',
            fill: true,
            tension: 0.4,
            borderWidth: 2
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
              beginAtZero: true,
              grid: {
                color: 'rgba(0, 0, 0, 0.05)'
              }
            },
            x: {
              grid: {
                display: false
              }
            }
          }
        }
      })
      chartInstances.push(chart)
    }
  }
}

onMounted(async () => {
  await loadStats()
  setTimeout(() => {
    createCharts()
  }, 100)
})

onBeforeUnmount(() => {
  chartInstances.forEach(chart => chart.destroy())
  chartInstances = []
})
</script>
