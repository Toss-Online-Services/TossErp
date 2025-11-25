<script setup lang="ts">
import { computed } from 'vue'
// Components are auto-imported in Nuxt - no need to import
import { Users, Building2, TrendingUp, DollarSign, ShoppingCart, Package, Warehouse } from 'lucide-vue-next'
import { Line } from 'vue-chartjs'
import type { ChartData, ChartOptions } from 'chart.js'

// TOSS ERP Dashboard
useHead({
  title: 'Dashboard - TOSS ERP'
})

const revenueData = computed<ChartData<'line'>>(() => ({
  labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
  datasets: [
    {
      label: 'Revenue',
      data: [18500, 19800, 19200, 22500, 21000, 23456],
      borderColor: 'rgb(66, 184, 131)',
      backgroundColor: 'rgba(66, 184, 131, 0.1)',
      tension: 0.4,
      fill: true
    }
  ]
}))

const revenueOptions = computed<ChartOptions<'line'>>(() => ({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: false
    },
    tooltip: {
      callbacks: {
        label: (context) => {
          const value = context.parsed.y
          return `Revenue: R ${(value).toLocaleString()}`
        }
      }
    }
  },
  scales: {
    y: {
      beginAtZero: true,
      ticks: {
        callback: (value) => `R ${Number(value).toLocaleString()}`
      }
    }
  }
}))
</script>

<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-xl font-bold tracking-tight">Dashboard</h1>
      <p class="text-muted-foreground">Welcome to your TOSS ERP dashboard</p>
    </div>

    <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
      <Card>
        <CardHeader class="flex flex-row items-center justify-between pb-2">
          <CardTitle class="text-sm font-medium">Today's Sales</CardTitle>
          <ShoppingCart class="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">R 12,450</div>
          <p class="text-xs text-muted-foreground">+12% from yesterday</p>
        </CardContent>
      </Card>

      <Card>
        <CardHeader class="flex flex-row items-center justify-between pb-2">
          <CardTitle class="text-sm font-medium">Stock Value</CardTitle>
          <Warehouse class="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">R 45,230</div>
          <p class="text-xs text-muted-foreground">+5% from last month</p>
        </CardContent>
      </Card>

      <Card>
        <CardHeader class="flex flex-row items-center justify-between pb-2">
          <CardTitle class="text-sm font-medium">Pending Orders</CardTitle>
          <Package class="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">8</div>
          <p class="text-xs text-muted-foreground">-2 from last week</p>
        </CardContent>
      </Card>

      <Card>
        <CardHeader class="flex flex-row items-center justify-between pb-2">
          <CardTitle class="text-sm font-medium">Total Revenue</CardTitle>
          <DollarSign class="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">R 234,567</div>
          <p class="text-xs text-muted-foreground">+18% from last month</p>
        </CardContent>
      </Card>
    </div>

    <Card>
      <CardHeader>
        <CardTitle>Revenue Trend</CardTitle>
      </CardHeader>
      <CardContent>
        <ClientOnly>
          <div class="h-[300px]">
            <Line :data="revenueData" :options="revenueOptions" />
          </div>
          <template #fallback>
            <div class="h-[300px] flex items-center justify-center">
              <p class="text-muted-foreground">Loading chart...</p>
            </div>
          </template>
        </ClientOnly>
      </CardContent>
    </Card>

    <div class="grid gap-4 md:grid-cols-2">
      <Card>
        <CardHeader>
          <CardTitle>Recent Activity</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="space-y-4">
            <div class="flex items-start gap-3">
              <div class="w-2 h-2 bg-primary rounded-full mt-2"></div>
              <div class="flex-1">
                <p class="text-sm font-medium">New sale completed</p>
                <p class="text-xs text-muted-foreground">R 1,250 POS transaction</p>
                <p class="text-xs text-muted-foreground">2 hours ago</p>
              </div>
            </div>
            <div class="flex items-start gap-3">
              <div class="w-2 h-2 bg-primary rounded-full mt-2"></div>
              <div class="flex-1">
                <p class="text-sm font-medium">Stock received</p>
                <p class="text-xs text-muted-foreground">Purchase order #PO-1234 delivered</p>
                <p class="text-xs text-muted-foreground">5 hours ago</p>
              </div>
            </div>
            <div class="flex items-start gap-3">
              <div class="w-2 h-2 bg-primary rounded-full mt-2"></div>
              <div class="flex-1">
                <p class="text-sm font-medium">Low stock alert</p>
                <p class="text-xs text-muted-foreground">Cooking oil running low</p>
                <p class="text-xs text-muted-foreground">1 day ago</p>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Upcoming Tasks</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="space-y-4">
            <div class="flex items-center gap-3">
              <input type="checkbox" class="w-4 h-4 rounded border-input" />
              <div class="flex-1">
                <p class="text-sm font-medium">Review purchase orders</p>
                <p class="text-xs text-muted-foreground">Today at 2:00 PM</p>
              </div>
            </div>
            <div class="flex items-center gap-3">
              <input type="checkbox" class="w-4 h-4 rounded border-input" />
              <div class="flex-1">
                <p class="text-sm font-medium">Stock take</p>
                <p class="text-xs text-muted-foreground">Tomorrow at 10:00 AM</p>
              </div>
            </div>
            <div class="flex items-center gap-3">
              <input type="checkbox" class="w-4 h-4 rounded border-input" />
              <div class="flex-1">
                <p class="text-sm font-medium">Supplier meeting</p>
                <p class="text-xs text-muted-foreground">Friday at 3:00 PM</p>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  </div>
</template>
