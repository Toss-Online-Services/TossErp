<template>
  <div class="p-6">
    <!-- Header Section -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-8">
      <div>
        <h1 class="text-3xl font-bold tracking-tight">{{ $t('dashboard.welcome') }}</h1>
        <p class="text-muted-foreground mt-1">{{ $t('dashboard.subtitle') }}</p>
      </div>
      <div class="flex gap-2 mt-4 sm:mt-0">
        <Button variant="outline" size="sm">
          <Calendar class="mr-2 h-4 w-4" />
          {{ formatDate(new Date()) }}
        </Button>
        <Button size="sm">
          <Plus class="mr-2 h-4 w-4" />
          {{ $t('common.quickAction') }}
        </Button>
      </div>
    </div>

    <!-- Key Metrics Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      <MetricCard
        v-for="metric in keyMetrics"
        :key="metric.id"
        :title="metric.title"
        :value="metric.value"
        :change="metric.change"
        :trend="metric.trend"
        :icon="metric.icon"
        :color="metric.color"
      />
    </div>

    <!-- Main Content Grid -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
      <!-- Sales Performance Chart -->
      <Card class="lg:col-span-2">
        <CardHeader>
          <CardTitle>{{ $t('dashboard.salesPerformance') }}</CardTitle>
          <CardDescription>{{ $t('dashboard.salesSubtitle') }}</CardDescription>
        </CardHeader>
        <CardContent>
          <div class="h-80 w-full">
            <SalesChart :data="salesData" />
          </div>
        </CardContent>
      </Card>

      <!-- Recent Activity -->
      <Card>
        <CardHeader>
          <CardTitle>{{ $t('dashboard.recentActivity') }}</CardTitle>
        </CardHeader>
        <CardContent class="p-0">
          <div class="space-y-4 p-6">
            <ActivityItem
              v-for="activity in recentActivities"
              :key="activity.id"
              :activity="activity"
            />
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- AI Insights & Quick Actions -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- AI Business Insights -->
      <Card>
        <CardHeader>
          <div class="flex items-center justify-between">
            <CardTitle class="flex items-center gap-2">
              <Sparkles class="h-5 w-5 text-yellow-500" />
              {{ $t('dashboard.aiInsights') }}
            </CardTitle>
            <Badge variant="secondary">{{ $t('dashboard.powered') }} TOSS AI</Badge>
          </div>
        </CardHeader>
        <CardContent>
          <div class="space-y-4">
            <InsightCard
              v-for="insight in aiInsights"
              :key="insight.id"
              :insight="insight"
            />
          </div>
          <Button class="w-full mt-4" variant="outline">
            <Bot class="mr-2 h-4 w-4" />
            {{ $t('dashboard.viewAllInsights') }}
          </Button>
        </CardContent>
      </Card>

      <!-- Quick Actions -->
      <Card>
        <CardHeader>
          <CardTitle>{{ $t('dashboard.quickActions') }}</CardTitle>
          <CardDescription>{{ $t('dashboard.quickActionsSubtitle') }}</CardDescription>
        </CardHeader>
        <CardContent>
          <div class="grid grid-cols-2 gap-4">
            <QuickActionButton
              v-for="action in quickActions"
              :key="action.id"
              :action="action"
              @click="action.handler"
            />
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Low Stock Alerts -->
    <Card v-if="lowStockItems.length > 0" class="mt-6">
      <CardHeader>
        <CardTitle class="flex items-center gap-2">
          <AlertTriangle class="h-5 w-5 text-orange-500" />
          {{ $t('dashboard.lowStockAlert') }}
        </CardTitle>
      </CardHeader>
      <CardContent>
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <LowStockItem
            v-for="item in lowStockItems"
            :key="item.id"
            :item="item"
          />
        </div>
        <Button class="w-full mt-4">
          <ShoppingCart class="mr-2 h-4 w-4" />
          {{ $t('dashboard.createReorderList') }}
        </Button>
      </CardContent>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import { 
  Plus, 
  Calendar, 
  Sparkles, 
  Bot, 
  AlertTriangle, 
  ShoppingCart,
  TrendingUp,
  TrendingDown,
  Users,
  DollarSign,
  Package
} from 'lucide-vue-next'

import { Button } from '../ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../ui/card'
import { Badge } from '../ui/badge'

// Mock data for demonstration
const keyMetrics = ref([
  {
    id: 'revenue',
    title: 'Monthly Revenue',
    value: 'R 127,450',
    change: '+12.5%',
    trend: 'up',
    icon: DollarSign,
    color: 'green'
  },
  {
    id: 'orders',
    title: 'Orders Today',
    value: '23',
    change: '+3 from yesterday',
    trend: 'up',
    icon: ShoppingCart,
    color: 'blue'
  },
  {
    id: 'customers',
    title: 'Active Customers',
    value: '156',
    change: '+8 this week',
    trend: 'up',
    icon: Users,
    color: 'purple'
  },
  {
    id: 'inventory',
    title: 'Low Stock Items',
    value: '5',
    change: 'Need reordering',
    trend: 'down',
    icon: Package,
    color: 'orange'
  }
])

const salesData = ref([
  { month: 'Jan', revenue: 45000 },
  { month: 'Feb', revenue: 52000 },
  { month: 'Mar', revenue: 48000 },
  { month: 'Apr', revenue: 61000 },
  { month: 'May', revenue: 55000 },
  { month: 'Jun', revenue: 67000 }
])

const recentActivities = ref([
  {
    id: 1,
    type: 'sale',
    description: 'New sale to Mthunzi\'s Spaza Shop',
    amount: 'R 1,250.00',
    time: '5 min ago',
    icon: DollarSign
  },
  {
    id: 2,
    type: 'order',
    description: 'Purchase order from supplier',
    amount: 'R 8,500.00',
    time: '15 min ago',
    icon: ShoppingCart
  },
  {
    id: 3,
    type: 'customer',
    description: 'New customer registered',
    amount: 'Nomsa\'s Chisa Nyama',
    time: '1 hour ago',
    icon: Users
  }
])

const aiInsights = ref([
  {
    id: 1,
    type: 'opportunity',
    title: 'Group Buying Opportunity',
    description: 'Coordinate with 3 nearby spaza shops to save 15% on maize meal purchases',
    action: 'Start Group Buy',
    priority: 'high'
  },
  {
    id: 2,
    type: 'trend',
    title: 'Seasonal Demand Increase',
    description: 'Soft drinks demand typically increases by 30% next month. Consider stocking up.',
    action: 'View Forecast',
    priority: 'medium'
  },
  {
    id: 3,
    type: 'risk',
    title: 'Credit Risk Alert',
    description: 'Customer payment delays increased by 20%. Review credit terms.',
    action: 'Review Accounts',
    priority: 'high'
  }
])

const lowStockItems = ref([
  {
    id: 1,
    name: 'White Bread Loaf',
    currentStock: 8,
    reorderLevel: 15,
    supplier: 'Albany Bakeries',
    urgency: 'high'
  },
  {
    id: 2,
    name: 'Cooking Oil (750ml)',
    currentStock: 12,
    reorderLevel: 20,
    supplier: 'Sunfoil',
    urgency: 'medium'
  }
])

const quickActions = ref([
  {
    id: 'new-sale',
    label: 'New Sale',
    description: 'Create quick sale',
    icon: Plus,
    handler: () => router.push('/sales/pos')
  },
  {
    id: 'new-order',
    label: 'New Order',
    description: 'Create sales order',
    icon: ShoppingCart,
    handler: () => router.push('/sales/orders/create')
  },
  {
    id: 'view-analytics',
    label: 'Analytics',
    description: 'View reports',
    icon: TrendingUp,
    handler: () => router.push('/sales/analytics')
  },
  {
    id: 'inventory',
    label: 'Inventory',
    description: 'Check stock',
    icon: Package,
    handler: () => router.push('/stock')
  }
])

const { t } = useI18n()
const router = useRouter()

const formatDate = (date: Date) => {
  return new Intl.DateTimeFormat('en-ZA', {
    day: 'numeric',
    month: 'short',
    year: 'numeric'
  }).format(date)
}

// Page metadata
useHead({
  title: 'Dashboard - TOSS ERP',
  meta: [
    { name: 'description', content: 'TOSS ERP dashboard with business insights and quick actions' }
  ]
})

onMounted(() => {
  // Load dashboard data
  console.log('Enhanced dashboard loaded with shadcn/ui components')
})
</script>

<style scoped>
/* Enhanced Dashboard Styling */
</style>