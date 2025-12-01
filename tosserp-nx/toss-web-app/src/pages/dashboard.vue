<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import CardSkeleton from '@/components/ui/CardSkeleton.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import { 
  TrendingUp, 
  TrendingDown, 
  DollarSign, 
  ShoppingBag,
  Boxes,
  Users,
  AlertCircle,
  CheckCircle,
  Info
} from 'lucide-vue-next'

const isLoading = ref(true)
const kpiData = ref({
  todaySales: { value: 0, change: 0, status: 'neutral' as 'good' | 'warning' | 'bad' | 'neutral' },
  moneyIn: { value: 0, change: 0, status: 'neutral' as 'good' | 'warning' | 'bad' | 'neutral' },
  moneyOut: { value: 0, change: 0, status: 'neutral' as 'good' | 'warning' | 'bad' | 'neutral' },
  lowStockCount: { value: 0, status: 'neutral' as 'good' | 'warning' | 'bad' | 'neutral' }
})

// Simulate data loading
onMounted(async () => {
  // TODO: Replace with actual API call
  await new Promise(resolve => setTimeout(resolve, 1000))
  
  kpiData.value = {
    todaySales: { value: 12450, change: 12.5, status: 'good' },
    moneyIn: { value: 18500, change: 8.2, status: 'good' },
    moneyOut: { value: 3200, change: -5.1, status: 'good' },
    lowStockCount: { value: 3, status: 'warning' }
  }
  
  isLoading.value = false
})

const getStatusIcon = (status: string) => {
  switch (status) {
    case 'good':
      return CheckCircle
    case 'warning':
      return AlertCircle
    case 'bad':
      return AlertCircle
    default:
      return Info
  }
}

const getStatusColor = (status: string) => {
  switch (status) {
    case 'good':
      return 'text-emerald-600'
    case 'warning':
      return 'text-amber-600'
    case 'bad':
      return 'text-red-600'
    default:
      return 'text-muted-foreground'
  }
}

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
    minimumFractionDigits: 0
  }).format(value)
}
</script>

<template>
  <div class="space-y-6">
    <div>
      <Breadcrumbs />
      <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">Dashboard</h1>
      <p class="text-muted-foreground mt-1">Today's overview and quick actions</p>
    </div>

    <!-- KPI Cards with Traffic Light Indicators -->
    <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
      <template v-if="isLoading">
        <CardSkeleton v-for="i in 4" :key="i" />
      </template>
      <template v-else>
        <!-- Today's Sales -->
        <Card>
          <CardHeader class="flex flex-row items-center justify-between pb-2">
            <CardTitle class="text-sm font-medium">Today's Sales</CardTitle>
            <div class="flex items-center gap-2">
              <component 
                :is="getStatusIcon(kpiData.todaySales.status)" 
                :class="['h-4 w-4', getStatusColor(kpiData.todaySales.status)]"
              />
              <ShoppingBag class="h-4 w-4 text-muted-foreground" />
            </div>
          </CardHeader>
          <CardContent>
            <div class="text-2xl font-bold">{{ formatCurrency(kpiData.todaySales.value) }}</div>
            <div class="flex items-center gap-1 text-xs mt-1">
              <TrendingUp 
                v-if="kpiData.todaySales.change > 0" 
                class="h-3 w-3 text-emerald-600" 
              />
              <TrendingDown 
                v-else-if="kpiData.todaySales.change < 0" 
                class="h-3 w-3 text-red-600" 
              />
              <span 
                :class="kpiData.todaySales.change > 0 ? 'text-emerald-600' : 'text-red-600'"
              >
                {{ Math.abs(kpiData.todaySales.change) }}% from yesterday
              </span>
            </div>
          </CardContent>
        </Card>

        <!-- Money In -->
        <Card>
          <CardHeader class="flex flex-row items-center justify-between pb-2">
            <CardTitle class="text-sm font-medium">Money In</CardTitle>
            <div class="flex items-center gap-2">
              <component 
                :is="getStatusIcon(kpiData.moneyIn.status)" 
                :class="['h-4 w-4', getStatusColor(kpiData.moneyIn.status)]"
              />
              <TrendingUp class="h-4 w-4 text-muted-foreground" />
            </div>
          </CardHeader>
          <CardContent>
            <div class="text-2xl font-bold">{{ formatCurrency(kpiData.moneyIn.value) }}</div>
            <div class="flex items-center gap-1 text-xs mt-1">
              <TrendingUp 
                v-if="kpiData.moneyIn.change > 0" 
                class="h-3 w-3 text-emerald-600" 
              />
              <TrendingDown 
                v-else-if="kpiData.moneyIn.change < 0" 
                class="h-3 w-3 text-red-600" 
              />
              <span 
                :class="kpiData.moneyIn.change > 0 ? 'text-emerald-600' : 'text-red-600'"
              >
                {{ Math.abs(kpiData.moneyIn.change) }}% from yesterday
              </span>
            </div>
          </CardContent>
        </Card>

        <!-- Money Out -->
        <Card>
          <CardHeader class="flex flex-row items-center justify-between pb-2">
            <CardTitle class="text-sm font-medium">Money Out</CardTitle>
            <div class="flex items-center gap-2">
              <component 
                :is="getStatusIcon(kpiData.moneyOut.status)" 
                :class="['h-4 w-4', getStatusColor(kpiData.moneyOut.status)]"
              />
              <TrendingDown class="h-4 w-4 text-muted-foreground" />
            </div>
          </CardHeader>
          <CardContent>
            <div class="text-2xl font-bold">{{ formatCurrency(kpiData.moneyOut.value) }}</div>
            <div class="flex items-center gap-1 text-xs mt-1">
              <TrendingUp 
                v-if="kpiData.moneyOut.change > 0" 
                class="h-3 w-3 text-red-600" 
              />
              <TrendingDown 
                v-else-if="kpiData.moneyOut.change < 0" 
                class="h-3 w-3 text-emerald-600" 
              />
              <span 
                :class="kpiData.moneyOut.change > 0 ? 'text-red-600' : 'text-emerald-600'"
              >
                {{ Math.abs(kpiData.moneyOut.change) }}% from yesterday
              </span>
            </div>
          </CardContent>
        </Card>

        <!-- Low Stock Alert -->
        <Card>
          <CardHeader class="flex flex-row items-center justify-between pb-2">
            <CardTitle class="text-sm font-medium">Low Stock Items</CardTitle>
            <div class="flex items-center gap-2">
              <component 
                :is="getStatusIcon(kpiData.lowStockCount.status)" 
                :class="['h-4 w-4', getStatusColor(kpiData.lowStockCount.status)]"
              />
              <Boxes class="h-4 w-4 text-muted-foreground" />
            </div>
          </CardHeader>
          <CardContent>
            <div class="text-2xl font-bold">{{ kpiData.lowStockCount.value }}</div>
            <p class="text-xs text-muted-foreground mt-1">
              {{ kpiData.lowStockCount.value === 0 ? 'All good!' : 'Need reorder' }}
            </p>
          </CardContent>
        </Card>
      </template>
    </div>

    <!-- Quick Actions -->
    <Card>
      <CardHeader>
        <CardTitle class="text-lg">Quick Actions</CardTitle>
      </CardHeader>
      <CardContent>
        <div class="grid gap-3 md:grid-cols-2 lg:grid-cols-4">
          <NuxtLink
            to="/sales/pos"
            class="flex items-center gap-3 p-4 border rounded-lg hover:bg-accent transition-colors"
          >
            <ShoppingBag class="h-5 w-5 text-primary" />
            <div>
              <p class="font-medium text-sm">New Sale</p>
              <p class="text-xs text-muted-foreground">Start POS transaction</p>
            </div>
          </NuxtLink>
          
          <NuxtLink
            to="/stock"
            class="flex items-center gap-3 p-4 border rounded-lg hover:bg-accent transition-colors"
          >
            <Boxes class="h-5 w-5 text-primary" />
            <div>
              <p class="font-medium text-sm">Check Stock</p>
              <p class="text-xs text-muted-foreground">View inventory levels</p>
            </div>
          </NuxtLink>
          
          <NuxtLink
            to="/people"
            class="flex items-center gap-3 p-4 border rounded-lg hover:bg-accent transition-colors"
          >
            <Users class="h-5 w-5 text-primary" />
            <div>
              <p class="font-medium text-sm">Add Customer</p>
              <p class="text-xs text-muted-foreground">Register new customer</p>
            </div>
          </NuxtLink>
          
          <NuxtLink
            to="/money"
            class="flex items-center gap-3 p-4 border rounded-lg hover:bg-accent transition-colors"
          >
            <DollarSign class="h-5 w-5 text-primary" />
            <div>
              <p class="font-medium text-sm">View Money</p>
              <p class="text-xs text-muted-foreground">Cash flow summary</p>
            </div>
          </NuxtLink>
        </div>
      </CardContent>
    </Card>

    <!-- AI Copilot Suggestions -->
    <Card>
      <CardHeader>
        <CardTitle class="text-lg flex items-center gap-2">
          <Info class="h-5 w-5" />
          AI Copilot Suggestions
        </CardTitle>
      </CardHeader>
      <CardContent>
        <div class="space-y-3">
          <div 
            v-if="kpiData.lowStockCount.value > 0"
            class="flex items-start gap-3 p-3 bg-amber-50 border border-amber-200 rounded-lg"
          >
            <AlertCircle class="h-5 w-5 text-amber-600 mt-0.5" />
            <div class="flex-1">
              <p class="text-sm font-medium text-amber-900">
                You have {{ kpiData.lowStockCount.value }} item(s) running low on stock
              </p>
              <p class="text-xs text-amber-700 mt-1">
                Consider reordering to avoid stockouts. <NuxtLink to="/stock" class="underline">View stock alerts</NuxtLink>
              </p>
            </div>
          </div>
          
          <div class="flex items-start gap-3 p-3 bg-sky-50 border border-sky-200 rounded-lg">
            <Info class="h-5 w-5 text-sky-600 mt-0.5" />
            <div class="flex-1">
              <p class="text-sm font-medium text-sky-900">
                Today's sales are {{ kpiData.todaySales.change > 0 ? 'up' : 'down' }} {{ Math.abs(kpiData.todaySales.change) }}%
              </p>
              <p class="text-xs text-sky-700 mt-1">
                {{ kpiData.todaySales.change > 0 ? 'Great work! Keep it up.' : 'Consider promotions or marketing to boost sales.' }}
              </p>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  </div>
</template>
