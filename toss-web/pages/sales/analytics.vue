<template>
  <AppLayout>
    <div class="p-6 space-y-6">
      <!-- Header -->
      <div class="flex justify-between items-center">
        <div>
          <h1 class="text-2xl font-bold">Sales Analytics</h1>
          <p class="text-muted-foreground">Track your sales performance and trends</p>
        </div>
        <div class="flex items-center space-x-4">
          <select 
            v-model="selectedPeriod" 
            class="px-3 py-2 border rounded-md"
          >
            <option value="7d">Last 7 days</option>
            <option value="30d">Last 30 days</option>
            <option value="90d">Last 90 days</option>
            <option value="1y">Last year</option>
          </select>
          <Button>
            <Download class="h-4 w-4 mr-2" />
            Export Report
          </Button>
        </div>
      </div>

      <!-- Key Metrics -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-muted-foreground">Total Revenue</p>
              <p class="text-2xl font-bold">R {{ totalRevenue.toLocaleString() }}</p>
              <p class="text-sm text-green-600 flex items-center mt-1">
                <TrendingUp class="h-4 w-4 mr-1" />
                +12.5% vs last period
              </p>
            </div>
            <div class="p-3 bg-green-100 dark:bg-green-900 rounded-lg">
              <DollarSign class="h-6 w-6 text-green-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-muted-foreground">Total Orders</p>
              <p class="text-2xl font-bold">{{ totalOrders }}</p>
              <p class="text-sm text-green-600 flex items-center mt-1">
                <TrendingUp class="h-4 w-4 mr-1" />
                +8.3% vs last period
              </p>
            </div>
            <div class="p-3 bg-blue-100 dark:bg-blue-900 rounded-lg">
              <ShoppingBag class="h-6 w-6 text-blue-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-muted-foreground">Average Order</p>
              <p class="text-2xl font-bold">R {{ averageOrder.toFixed(2) }}</p>
              <p class="text-sm text-red-600 flex items-center mt-1">
                <TrendingDown class="h-4 w-4 mr-1" />
                -2.1% vs last period
              </p>
            </div>
            <div class="p-3 bg-orange-100 dark:bg-orange-900 rounded-lg">
              <Calculator class="h-6 w-6 text-orange-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-muted-foreground">Profit Margin</p>
              <p class="text-2xl font-bold">{{ profitMargin }}%</p>
              <p class="text-sm text-green-600 flex items-center mt-1">
                <TrendingUp class="h-4 w-4 mr-1" />
                +1.2% vs last period
              </p>
            </div>
            <div class="p-3 bg-purple-100 dark:bg-purple-900 rounded-lg">
              <Percent class="h-6 w-6 text-purple-600" />
            </div>
          </div>
        </div>
      </div>

      <!-- Charts Section -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Sales Trend Chart -->
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <h3 class="text-lg font-semibold mb-4">Sales Trend</h3>
          <div class="h-64 flex items-center justify-center border rounded-lg bg-gray-50 dark:bg-gray-700">
            <p class="text-muted-foreground">Chart component would go here</p>
          </div>
        </div>

        <!-- Top Products -->
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <h3 class="text-lg font-semibold mb-4">Top Selling Products</h3>
          <div class="space-y-4">
            <div v-for="product in topProducts" :key="product.id" class="flex items-center justify-between">
              <div class="flex items-center space-x-3">
                <div class="w-8 h-8 bg-gray-200 dark:bg-gray-600 rounded-lg"></div>
                <div>
                  <p class="font-medium">{{ product.name }}</p>
                  <p class="text-sm text-muted-foreground">{{ product.sold }} sold</p>
                </div>
              </div>
              <div class="text-right">
                <p class="font-medium">R {{ product.revenue.toLocaleString() }}</p>
                <p class="text-sm text-green-600">{{ product.growth }}%</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Sales by Category -->
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <h3 class="text-lg font-semibold mb-4">Sales by Category</h3>
          <div class="h-64 flex items-center justify-center border rounded-lg bg-gray-50 dark:bg-gray-700">
            <p class="text-muted-foreground">Pie chart would go here</p>
          </div>
        </div>

        <!-- Customer Insights -->
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <h3 class="text-lg font-semibold mb-4">Customer Insights</h3>
          <div class="space-y-4">
            <div class="flex justify-between items-center">
              <span class="text-muted-foreground">New Customers</span>
              <span class="font-medium">{{ newCustomers }}</span>
            </div>
            <div class="flex justify-between items-center">
              <span class="text-muted-foreground">Returning Customers</span>
              <span class="font-medium">{{ returningCustomers }}</span>
            </div>
            <div class="flex justify-between items-center">
              <span class="text-muted-foreground">Customer Retention</span>
              <span class="font-medium text-green-600">{{ customerRetention }}%</span>
            </div>
            <div class="flex justify-between items-center">
              <span class="text-muted-foreground">Avg. Customer Value</span>
              <span class="font-medium">R {{ avgCustomerValue.toLocaleString() }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Transactions -->
      <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
        <div class="flex justify-between items-center mb-4">
          <h3 class="text-lg font-semibold">Recent High-Value Transactions</h3>
          <NuxtLink to="/sales/orders" class="text-primary hover:underline">View All</NuxtLink>
        </div>
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead>
              <tr class="border-b">
                <th class="text-left py-2">Transaction ID</th>
                <th class="text-left py-2">Customer</th>
                <th class="text-left py-2">Amount</th>
                <th class="text-left py-2">Date</th>
                <th class="text-left py-2">Status</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="transaction in recentTransactions" :key="transaction.id" class="border-b">
                <td class="py-2">#{{ transaction.id }}</td>
                <td class="py-2">{{ transaction.customer }}</td>
                <td class="py-2 font-medium">R {{ transaction.amount.toLocaleString() }}</td>
                <td class="py-2 text-muted-foreground">{{ transaction.date }}</td>
                <td class="py-2">
                  <span class="px-2 py-1 rounded text-xs" :class="{
                    'bg-green-100 text-green-800': transaction.status === 'Completed',
                    'bg-yellow-100 text-yellow-800': transaction.status === 'Pending',
                    'bg-red-100 text-red-800': transaction.status === 'Failed'
                  }">
                    {{ transaction.status }}
                  </span>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </AppLayout>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { 
  Download, 
  TrendingUp, 
  TrendingDown,
  DollarSign, 
  ShoppingBag, 
  Calculator,
  Percent
} from 'lucide-vue-next'

// Reactive data
const selectedPeriod = ref('30d')

// Mock data for analytics
const totalRevenue = ref(234580)
const totalOrders = ref(1247)
const averageOrder = computed(() => totalRevenue.value / totalOrders.value)
const profitMargin = ref(24.5)
const newCustomers = ref(89)
const returningCustomers = ref(156)
const customerRetention = ref(67.8)
const avgCustomerValue = ref(1880)

const topProducts = ref([
  {
    id: 1,
    name: 'Maize Meal 2.5kg',
    sold: 156,
    revenue: 23400,
    growth: 12.5
  },
  {
    id: 2,
    name: 'Cooking Oil 750ml',
    sold: 89,
    revenue: 17800,
    growth: 8.3
  },
  {
    id: 3,
    name: 'Bread Loaves',
    sold: 234,
    revenue: 11700,
    growth: -2.1
  },
  {
    id: 4,
    name: 'Sugar 2kg',
    sold: 67,
    revenue: 10050,
    growth: 15.2
  }
])

const recentTransactions = ref([
  {
    id: 'TXN001',
    customer: 'Nomsa Mbeki',
    amount: 2450,
    date: '2024-01-15',
    status: 'Completed'
  },
  {
    id: 'TXN002',
    customer: 'Thabo Mthembu',
    amount: 1890,
    date: '2024-01-15',
    status: 'Completed'
  },
  {
    id: 'TXN003',
    customer: 'Sipho Ndaba',
    amount: 3200,
    date: '2024-01-14',
    status: 'Pending'
  },
  {
    id: 'TXN004',
    customer: 'Zanele Dlamini',
    amount: 1560,
    date: '2024-01-14',
    status: 'Completed'
  }
])
</script>