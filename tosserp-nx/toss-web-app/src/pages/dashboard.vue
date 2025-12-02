<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import { 
  Calendar,
  Users,
  DollarSign,
  UserPlus,
  TrendingUp,
  BarChart3
} from 'lucide-vue-next'

// KPI Data
const kpiData = ref({
  bookings: { value: 281, change: 55, period: 'last week' },
  todayUsers: { value: 2300, change: 3, period: 'last month' },
  revenue: { value: 34000, change: 35, period: 'last month' },
  followers: { value: 2910, change: null, period: 'Just updated' }
})

// Chart data - Website Views (Bar Chart)
const websiteViewsData = computed(() => ({
  labels: ['M', 'T', 'W', 'T', 'F', 'S', 'S'],
  datasets: [{
    label: 'Views',
    data: [65, 78, 90, 81, 95, 55, 40],
    backgroundColor: 'rgba(34, 197, 94, 0.8)',
    borderColor: 'rgba(34, 197, 94, 1)',
    borderWidth: 1
  }]
}))

// Chart data - Daily Sales (Line Chart)
const dailySalesData = computed(() => ({
  labels: ['J', 'F', 'M', 'A', 'M', 'J', 'J', 'A', 'S', 'O', 'N', 'D'],
  datasets: [{
    label: 'Sales',
    data: [300, 400, 350, 450, 500, 550, 480, 520, 490, 530, 510, 580],
    borderColor: 'rgba(34, 197, 94, 1)',
    backgroundColor: 'rgba(34, 197, 94, 0.1)',
    tension: 0.4,
    fill: true
  }]
}))

// Chart data - Completed Tasks (Line Chart)
const completedTasksData = computed(() => ({
  labels: ['Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
  datasets: [{
    label: 'Tasks',
    data: [400, 450, 420, 480, 500, 520, 490, 510, 530],
    borderColor: 'rgba(34, 197, 94, 1)',
    backgroundColor: 'rgba(34, 197, 94, 0.1)',
    tension: 0.4,
    fill: true
  }]
}))

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
    minimumFractionDigits: 0
  }).format(value)
}

// Simple chart rendering using SVG (no external library needed for MVP)
const renderBarChart = (data: number[], maxValue: number = 100) => {
  const max = Math.max(...data, maxValue)
  return data.map((value, index) => ({
    x: index,
    value,
    height: (value / max) * 100
  }))
}

const renderLineChart = (data: number[], maxValue: number = 600) => {
  const max = Math.max(...data, maxValue)
  return data.map((value, index) => ({
    x: index,
    value,
    y: 100 - (value / max) * 100
  }))
}

const websiteViewsBars = computed(() => renderBarChart(websiteViewsData.value.datasets[0].data))
const dailySalesPoints = computed(() => renderLineChart(dailySalesData.value.datasets[0].data))
const completedTasksPoints = computed(() => renderLineChart(completedTasksData.value.datasets[0].data))
</script>

<template>
  <ClientOnly>
    <div class="space-y-6">
    <!-- Header Section -->
    <div>
      <Breadcrumbs />
      <h1 class="text-3xl font-bold tracking-tight mt-2">Analytics</h1>
      <p class="text-muted-foreground mt-1">Check the sales, value and bounce rate by country.</p>
    </div>

    <!-- Charts Row - Top -->
    <div class="grid gap-4 md:grid-cols-3">
      <!-- Website Views Chart -->
      <Card>
        <CardHeader>
          <CardTitle class="text-lg font-semibold">Website Views</CardTitle>
          <p class="text-sm text-muted-foreground">Last Campaign Performance</p>
        </CardHeader>
        <CardContent>
          <div class="h-[200px] relative">
            <svg class="w-full h-full" viewBox="0 0 300 200" preserveAspectRatio="none">
              <g v-for="(bar, index) in websiteViewsBars" :key="index">
                <rect
                  :x="(index * 40) + 20"
                  :y="200 - (bar.height * 1.8)"
                  width="30"
                  :height="bar.height * 1.8"
                  fill="rgba(34, 197, 94, 0.8)"
                  rx="2"
                />
              </g>
            </svg>
          </div>
          <p class="text-xs text-muted-foreground mt-2">campaign sent 2 days ago</p>
        </CardContent>
      </Card>

      <!-- Daily Sales Chart -->
      <Card>
        <CardHeader>
          <CardTitle class="text-lg font-semibold">Daily Sales</CardTitle>
          <p class="text-sm text-muted-foreground">
            <span class="text-emerald-600 font-semibold">(+15%)</span> increase in today sales.
          </p>
        </CardHeader>
        <CardContent>
          <div class="h-[200px] relative">
            <svg class="w-full h-full" viewBox="0 0 300 200" preserveAspectRatio="none">
              <defs>
                <linearGradient id="salesGradient" x1="0%" y1="0%" x2="0%" y2="100%">
                  <stop offset="0%" style="stop-color:rgba(34, 197, 94, 0.3);stop-opacity:1" />
                  <stop offset="100%" style="stop-color:rgba(34, 197, 94, 0.05);stop-opacity:1" />
                </linearGradient>
              </defs>
              <path
                :d="`M ${dailySalesPoints.map((p, i) => `${(i * 25) + 10},${p.y * 1.8}`).join(' L ')}`"
                fill="url(#salesGradient)"
                stroke="rgba(34, 197, 94, 1)"
                stroke-width="2"
              />
            </svg>
          </div>
          <p class="text-xs text-muted-foreground mt-2">updated 4 min ago</p>
        </CardContent>
      </Card>

      <!-- Completed Tasks Chart -->
      <Card>
        <CardHeader>
          <CardTitle class="text-lg font-semibold">Completed Tasks</CardTitle>
          <p class="text-sm text-muted-foreground">Last Campaign Performance</p>
        </CardHeader>
        <CardContent>
          <div class="h-[200px] relative">
            <svg class="w-full h-full" viewBox="0 0 300 200" preserveAspectRatio="none">
              <defs>
                <linearGradient id="tasksGradient" x1="0%" y1="0%" x2="0%" y2="100%">
                  <stop offset="0%" style="stop-color:rgba(34, 197, 94, 0.3);stop-opacity:1" />
                  <stop offset="100%" style="stop-color:rgba(34, 197, 94, 0.05);stop-opacity:1" />
                </linearGradient>
              </defs>
              <path
                :d="`M ${completedTasksPoints.map((p, i) => `${(i * 30) + 10},${p.y * 1.8}`).join(' L ')}`"
                fill="url(#tasksGradient)"
                stroke="rgba(34, 197, 94, 1)"
                stroke-width="2"
              />
            </svg>
          </div>
          <p class="text-xs text-muted-foreground mt-2">just updated</p>
        </CardContent>
      </Card>
    </div>

    <!-- KPI Cards Row - Middle -->
    <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
      <!-- Bookings -->
      <Card>
        <CardHeader class="flex flex-row items-center justify-between pb-2">
          <CardTitle class="text-sm font-medium">Bookings</CardTitle>
          <Calendar class="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ kpiData.bookings.value }}</div>
          <p class="text-xs text-emerald-600 mt-1">
            +{{ kpiData.bookings.change }}% than {{ kpiData.bookings.period }}
          </p>
        </CardContent>
      </Card>

      <!-- Today's Users -->
      <Card>
        <CardHeader class="flex flex-row items-center justify-between pb-2">
          <CardTitle class="text-sm font-medium">Today's Users</CardTitle>
          <Users class="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ kpiData.todayUsers.value.toLocaleString() }}</div>
          <p class="text-xs text-emerald-600 mt-1">
            +{{ kpiData.todayUsers.change }}% than {{ kpiData.todayUsers.period }}
          </p>
        </CardContent>
      </Card>

      <!-- Revenue -->
      <Card>
        <CardHeader class="flex flex-row items-center justify-between pb-2">
          <CardTitle class="text-sm font-medium">Revenue</CardTitle>
          <DollarSign class="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ formatCurrency(kpiData.revenue.value) }}</div>
          <p class="text-xs text-emerald-600 mt-1">
            +{{ kpiData.revenue.change }}% than {{ kpiData.revenue.period }}
          </p>
        </CardContent>
      </Card>

      <!-- Followers -->
      <Card>
        <CardHeader class="flex flex-row items-center justify-between pb-2">
          <CardTitle class="text-sm font-medium">Followers</CardTitle>
          <UserPlus class="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">+{{ kpiData.followers.value.toLocaleString() }}</div>
          <p class="text-xs text-muted-foreground mt-1">{{ kpiData.followers.period }}</p>
        </CardContent>
      </Card>
    </div>

    <!-- Image Cards Row - Bottom -->
    <div class="grid gap-4 md:grid-cols-3">
      <Card class="overflow-hidden p-0">
        <div class="h-64 bg-gradient-to-br from-blue-400 to-blue-600 relative">
          <div class="absolute inset-0 bg-black/20"></div>
          <div class="absolute bottom-4 left-4 right-4">
            <h3 class="text-white font-semibold text-lg">VIVA MONTI</h3>
            <p class="text-white/80 text-sm">4BN DISCOUNT</p>
          </div>
        </div>
      </Card>

      <Card class="overflow-hidden p-0">
        <div class="h-64 bg-gradient-to-br from-amber-400 to-orange-600 relative">
          <div class="absolute inset-0 bg-black/20"></div>
          <div class="absolute bottom-4 left-4 right-4">
            <h3 class="text-white font-semibold text-lg">Ancient Ruins</h3>
            <p class="text-white/80 text-sm">Historical Site</p>
          </div>
        </div>
      </Card>

      <Card class="overflow-hidden p-0">
        <div class="h-64 bg-gradient-to-br from-emerald-400 to-green-600 relative">
          <div class="absolute inset-0 bg-black/20"></div>
          <div class="absolute bottom-4 left-4 right-4">
            <h3 class="text-white font-semibold text-lg">Tropical Resort</h3>
            <p class="text-white/80 text-sm">Paradise Destination</p>
          </div>
        </div>
      </Card>
    </div>
    </div>
    <template #fallback>
      <div class="space-y-6">
        <div class="h-8 bg-gray-200 rounded animate-pulse"></div>
        <div class="grid gap-4 md:grid-cols-3">
          <div class="h-64 bg-gray-200 rounded animate-pulse" v-for="i in 3" :key="i"></div>
        </div>
        <div class="grid gap-4 md:grid-cols-4">
          <div class="h-32 bg-gray-200 rounded animate-pulse" v-for="i in 4" :key="i"></div>
        </div>
      </div>
    </template>
  </ClientOnly>
</template>
