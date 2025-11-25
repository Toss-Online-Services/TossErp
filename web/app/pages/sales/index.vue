<script setup lang="ts">
import { computed, ref } from 'vue'
import { 
  DollarSign, 
  ShoppingCart, 
  FileText, 
  Users,
  TrendingUp,
  Clock,
  CheckCircle,
  XCircle,
  ArrowUpRight,
  ArrowDownRight
} from 'lucide-vue-next'
import { Line } from 'vue-chartjs'
import type { ChartData, ChartOptions } from 'chart.js'
import 'chart.js/auto'
import Card from '~/components/ui/Card.vue'
import CardHeader from '~/components/ui/CardHeader.vue'
import CardTitle from '~/components/ui/CardTitle.vue'
import CardContent from '~/components/ui/CardContent.vue'
import Badge from '~/app/components/ui/badge/Badge.vue'

useHead({
  title: 'Sales Dashboard - TOSS ERP'
})

// Mock data - will be replaced with API calls
const salesMetrics = ref({
  totalRevenue: 234567,
  monthlyRevenue: 45678,
  pendingOrders: 8,
  activeCustomers: 156,
  quotationsDraft: 12,
  quotationsSent: 5,
  ordersToDeliver: 3,
  overdueInvoices: 2
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

const recentQuotations = ref([
  { id: 'QUO-001', customer: 'ABC Trading', amount: 12500, status: 'Sent', date: '2025-01-20' },
  { id: 'QUO-002', customer: 'XYZ Stores', amount: 8500, status: 'Draft', date: '2025-01-19' },
  { id: 'QUO-003', customer: 'Retail Co', amount: 15200, status: 'Accepted', date: '2025-01-18' }
])

const recentOrders = ref([
  { id: 'SO-001', customer: 'ABC Trading', amount: 12500, status: 'To Deliver', date: '2025-01-20' },
  { id: 'SO-002', customer: 'Retail Co', amount: 15200, status: 'Completed', date: '2025-01-15' }
])

const getStatusBadgeVariant = (status: string) => {
  const variants: Record<string, string> = {
    'Draft': 'secondary',
    'Sent': 'default',
    'Accepted': 'default',
    'To Deliver': 'default',
    'Completed': 'default',
    'Rejected': 'destructive'
  }
  return variants[status] || 'secondary'
}
</script>

<template>
  <div class="space-y-6">
    <!-- Page Header -->
    <div>
      <h1 class="text-3xl font-bold tracking-tight">Sales Dashboard</h1>
      <p class="text-muted-foreground mt-2">Overview of your sales performance and activities</p>
    </div>

    <!-- Key Metrics Cards -->
    <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
      <Card>
        <CardHeader class="flex flex-row items-center justify-between pb-2">
          <CardTitle class="text-sm font-medium">Total Revenue</CardTitle>
          <DollarSign class="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">R {{ salesMetrics.totalRevenue.toLocaleString() }}</div>
          <p class="text-xs text-muted-foreground flex items-center gap-1 mt-1">
            <ArrowUpRight class="h-3 w-3 text-green-600" />
            <span class="text-green-600">+18%</span> from last month
          </p>
        </CardContent>
      </Card>

      <Card>
        <CardHeader class="flex flex-row items-center justify-between pb-2">
          <CardTitle class="text-sm font-medium">Monthly Revenue</CardTitle>
          <TrendingUp class="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">R {{ salesMetrics.monthlyRevenue.toLocaleString() }}</div>
          <p class="text-xs text-muted-foreground flex items-center gap-1 mt-1">
            <ArrowUpRight class="h-3 w-3 text-green-600" />
            <span class="text-green-600">+12%</span> from last month
          </p>
        </CardContent>
      </Card>

      <Card>
        <CardHeader class="flex flex-row items-center justify-between pb-2">
          <CardTitle class="text-sm font-medium">Pending Orders</CardTitle>
          <ShoppingCart class="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ salesMetrics.pendingOrders }}</div>
          <p class="text-xs text-muted-foreground flex items-center gap-1 mt-1">
            <Clock class="h-3 w-3 text-orange-600" />
            <span>{{ salesMetrics.ordersToDeliver }} to deliver</span>
          </p>
        </CardContent>
      </Card>

      <Card>
        <CardHeader class="flex flex-row items-center justify-between pb-2">
          <CardTitle class="text-sm font-medium">Active Customers</CardTitle>
          <Users class="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ salesMetrics.activeCustomers }}</div>
          <p class="text-xs text-muted-foreground flex items-center gap-1 mt-1">
            <ArrowUpRight class="h-3 w-3 text-green-600" />
            <span class="text-green-600">+5</span> new this month
          </p>
        </CardContent>
      </Card>
    </div>

    <!-- Revenue Chart -->
    <Card>
      <CardHeader>
        <CardTitle>Revenue Trend</CardTitle>
      </CardHeader>
      <CardContent>
        <div class="h-[300px]">
          <ClientOnly>
            <Line :data="revenueData" :options="revenueOptions" />
          </ClientOnly>
        </div>
      </CardContent>
    </Card>

    <!-- Quick Stats -->
    <div class="grid gap-4 md:grid-cols-3">
      <Card>
        <CardHeader>
          <CardTitle class="text-base">Quotations</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="space-y-2">
            <div class="flex items-center justify-between">
              <span class="text-sm text-muted-foreground">Draft</span>
              <Badge variant="secondary">{{ salesMetrics.quotationsDraft }}</Badge>
            </div>
            <div class="flex items-center justify-between">
              <span class="text-sm text-muted-foreground">Sent</span>
              <Badge>{{ salesMetrics.quotationsSent }}</Badge>
            </div>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle class="text-base">Orders</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="space-y-2">
            <div class="flex items-center justify-between">
              <span class="text-sm text-muted-foreground">To Deliver</span>
              <Badge variant="default">{{ salesMetrics.ordersToDeliver }}</Badge>
            </div>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle class="text-base">Invoices</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="space-y-2">
            <div class="flex items-center justify-between">
              <span class="text-sm text-muted-foreground">Overdue</span>
              <Badge variant="destructive">{{ salesMetrics.overdueInvoices }}</Badge>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Recent Activity -->
    <div class="grid gap-4 md:grid-cols-2">
      <!-- Recent Quotations -->
      <Card>
        <CardHeader>
          <div class="flex items-center justify-between">
            <CardTitle>Recent Quotations</CardTitle>
            <NuxtLink to="/sales/quotations" class="text-sm text-primary hover:underline">
              View all
            </NuxtLink>
          </div>
        </CardHeader>
        <CardContent>
          <div class="space-y-4">
            <div
              v-for="quote in recentQuotations"
              :key="quote.id"
              class="flex items-center justify-between border-b pb-3 last:border-0"
            >
              <div class="flex-1">
                <div class="flex items-center gap-2">
                  <p class="text-sm font-medium">{{ quote.id }}</p>
                  <Badge :variant="getStatusBadgeVariant(quote.status) as any">
                    {{ quote.status }}
                  </Badge>
                </div>
                <p class="text-xs text-muted-foreground mt-1">{{ quote.customer }}</p>
                <p class="text-xs text-muted-foreground">{{ quote.date }}</p>
              </div>
              <div class="text-right">
                <p class="text-sm font-semibold">R {{ quote.amount.toLocaleString() }}</p>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      <!-- Recent Orders -->
      <Card>
        <CardHeader>
          <div class="flex items-center justify-between">
            <CardTitle>Recent Orders</CardTitle>
            <NuxtLink to="/sales/orders" class="text-sm text-primary hover:underline">
              View all
            </NuxtLink>
          </div>
        </CardHeader>
        <CardContent>
          <div class="space-y-4">
            <div
              v-for="order in recentOrders"
              :key="order.id"
              class="flex items-center justify-between border-b pb-3 last:border-0"
            >
              <div class="flex-1">
                <div class="flex items-center gap-2">
                  <p class="text-sm font-medium">{{ order.id }}</p>
                  <Badge :variant="getStatusBadgeVariant(order.status) as any">
                    {{ order.status }}
                  </Badge>
                </div>
                <p class="text-xs text-muted-foreground mt-1">{{ order.customer }}</p>
                <p class="text-xs text-muted-foreground">{{ order.date }}</p>
              </div>
              <div class="text-right">
                <p class="text-sm font-semibold">R {{ order.amount.toLocaleString() }}</p>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  </div>
</template>
