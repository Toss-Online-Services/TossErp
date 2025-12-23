<script setup lang="ts">
import { ref, computed } from 'vue'
import { TrendingUp, DollarSign, ArrowDownCircle, ArrowUpCircle, Package, Truck } from 'lucide-vue-next'
import SalesTrendChart from '~/components/charts/SalesTrendChart.vue'
import TopProductsChart from '~/components/charts/TopProductsChart.vue'
import AppKpiCard from '~/components/common/AppKpiCard.vue'
import AppCard from '~/components/common/AppCard.vue'
import AppTable from '~/components/common/AppTable.vue'
import AppSectionHeader from '~/components/common/AppSectionHeader.vue'

useHead({
  title: 'Dashboard - TOSS Admin',
  meta: [{ name: 'description', content: 'Analytics dashboard for TOSS Admin' }]
})

// Mock data
const todaySales = ref(12500)
const cashIn = ref(15200)
const cashOut = ref(3200)
const lowStockCount = ref(8)
const pendingDeliveries = ref(3)

const salesTrendData = ref([
  { date: 'Mon', value: 3200 },
  { date: 'Tue', value: 4100 },
  { date: 'Wed', value: 3800 },
  { date: 'Thu', value: 5200 },
  { date: 'Fri', value: 4800 },
  { date: 'Sat', value: 6100 },
  { date: 'Sun', value: 5500 }
])

const topProductsData = ref([
  { name: 'White Bread', value: 2450 },
  { name: 'Fresh Milk 1L', value: 1890 },
  { name: 'Sugar 2.5kg', value: 1650 },
  { name: 'Cooking Oil', value: 1420 },
  { name: 'Maize Meal', value: 1280 }
])

const recentActivity = ref([
  { id: 1, type: 'Sale', description: 'POS Sale #1234', amount: 'R245.50', time: '2 min ago' },
  { id: 2, type: 'Purchase', description: 'Stock received from Supplier A', amount: 'R1,250.00', time: '15 min ago' },
  { id: 3, type: 'Sale', description: 'POS Sale #1233', amount: 'R89.00', time: '28 min ago' },
  { id: 4, type: 'Delivery', description: 'Delivery completed - Order #567', amount: '-', time: '1 hour ago' },
  { id: 5, type: 'Sale', description: 'POS Sale #1232', amount: 'R156.75', time: '1 hour ago' }
])

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
    minimumFractionDigits: 0
  }).format(value).replace('ZAR', 'R')
}

definePageMeta({
  layout: 'dashboard'
})
</script>

<template>
  <div class="space-y-6">
    <AppSectionHeader
      title="Dashboard"
      description="Overview of your business performance"
    />

    <!-- KPI Cards -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-4">
      <AppKpiCard
        title="Today Sales"
        :value="formatCurrency(todaySales)"
        change="+12.5%"
        change-type="positive"
        :icon="DollarSign"
      />
      <AppKpiCard
        title="Cash In"
        :value="formatCurrency(cashIn)"
        change="+8.3%"
        change-type="positive"
        :icon="ArrowUpCircle"
      />
      <AppKpiCard
        title="Cash Out"
        :value="formatCurrency(cashOut)"
        change="-5.2%"
        change-type="negative"
        :icon="ArrowDownCircle"
      />
      <AppKpiCard
        title="Low Stock"
        :value="lowStockCount"
        change="3 items"
        change-type="negative"
        :icon="Package"
      />
      <AppKpiCard
        title="Pending Deliveries"
        :value="pendingDeliveries"
        change="2 today"
        change-type="neutral"
        :icon="Truck"
      />
    </div>

    <!-- Charts Row -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Sales Trend Chart -->
      <AppCard title="Sales Trend" subtitle="Last 7 days">
        <SalesTrendChart :data="salesTrendData" />
      </AppCard>

      <!-- Top Products Chart -->
      <AppCard title="Top Products" subtitle="Best sellers this week">
        <TopProductsChart :data="topProductsData" />
      </AppCard>
    </div>

    <!-- Recent Activity Table -->
    <AppCard title="Recent Activity" subtitle="Latest transactions and events">
      <AppTable :headers="['Type', 'Description', 'Amount', 'Time']">
        <tr
          v-for="activity in recentActivity"
          :key="activity.id"
          class="hover:bg-muted/50 transition-colors"
        >
          <td class="px-4 py-3">
            <span
              :class="[
                'inline-flex items-center px-2 py-1 rounded text-xs font-medium',
                activity.type === 'Sale' ? 'bg-green-100 dark:bg-green-900/20 text-green-700 dark:text-green-400' : '',
                activity.type === 'Purchase' ? 'bg-blue-100 dark:bg-blue-900/20 text-blue-700 dark:text-blue-400' : '',
                activity.type === 'Delivery' ? 'bg-orange-100 dark:bg-orange-900/20 text-orange-700 dark:text-orange-400' : ''
              ]"
            >
              {{ activity.type }}
            </span>
          </td>
          <td class="px-4 py-3 text-sm text-foreground">{{ activity.description }}</td>
          <td class="px-4 py-3 text-sm font-medium text-foreground">{{ activity.amount }}</td>
          <td class="px-4 py-3 text-sm text-muted-foreground">{{ activity.time }}</td>
        </tr>
      </AppTable>
    </AppCard>
  </div>
</template>
