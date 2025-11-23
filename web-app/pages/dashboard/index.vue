<template>
  <div class="space-y-6">
    <!-- Page Header -->
    <div>
      <h1 class="text-3xl font-bold">Dashboard</h1>
      <p class="text-muted-foreground">Welcome back! Here's what's happening with your business today.</p>
    </div>
    
    <!-- Stats Grid -->
    <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
      <Card>
        <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle class="text-sm font-medium">Today's Sales</CardTitle>
          <Icon name="lucide:trending-up" class="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">R {{ formatCurrency(todaySales) }}</div>
          <p class="text-xs text-muted-foreground">+12.5% from yesterday</p>
        </CardContent>
      </Card>
      
      <Card>
        <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle class="text-sm font-medium">Low Stock Items</CardTitle>
          <Icon name="lucide:alert-triangle" class="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ lowStockCount }}</div>
          <p class="text-xs text-muted-foreground">Items need restocking</p>
        </CardContent>
      </Card>
      
      <Card>
        <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle class="text-sm font-medium">Pending Orders</CardTitle>
          <Icon name="lucide:shopping-bag" class="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ pendingOrders }}</div>
          <p class="text-xs text-muted-foreground">Awaiting delivery</p>
        </CardContent>
      </Card>
      
      <Card>
        <CardHeader class="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle class="text-sm font-medium">Cash Balance</CardTitle>
          <Icon name="lucide:wallet" class="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">R {{ formatCurrency(cashBalance) }}</div>
          <p class="text-xs text-muted-foreground">Available now</p>
        </CardContent>
      </Card>
    </div>
    
    <!-- Charts and Recent Activity -->
    <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-7">
      <Card class="col-span-4">
        <CardHeader>
          <CardTitle>Sales Overview</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="h-[300px] flex items-center justify-center text-muted-foreground">
            <div class="text-center">
              <Icon name="lucide:bar-chart" class="w-12 h-12 mx-auto mb-2 opacity-50" />
              <p>Sales chart will be displayed here</p>
            </div>
          </div>
        </CardContent>
      </Card>
      
      <Card class="col-span-3">
        <CardHeader>
          <CardTitle>AI Insights</CardTitle>
        </CardHeader>
        <CardContent class="space-y-4">
          <div class="p-4 bg-primary/10 rounded-lg border border-primary/20">
            <div class="flex items-start space-x-3">
              <Icon name="lucide:sparkles" class="w-5 h-5 text-primary mt-0.5" />
              <div>
                <p class="font-medium text-sm">Stock Alert</p>
                <p class="text-xs text-muted-foreground mt-1">
                  Bread stock is running low. Consider ordering 50 loaves by Friday.
                </p>
              </div>
            </div>
          </div>
          
          <div class="p-4 bg-muted rounded-lg">
            <div class="flex items-start space-x-3">
              <Icon name="lucide:trending-up" class="w-5 h-5 text-primary mt-0.5" />
              <div>
                <p class="font-medium text-sm">Sales Trend</p>
                <p class="text-xs text-muted-foreground mt-1">
                  Weekend sales are up 15%. Consider increasing stock for Saturdays.
                </p>
              </div>
            </div>
          </div>
          
          <div class="p-4 bg-muted rounded-lg">
            <div class="flex items-start space-x-3">
              <Icon name="lucide:dollar-sign" class="w-5 h-5 text-primary mt-0.5" />
              <div>
                <p class="font-medium text-sm">Cash Flow</p>
                <p class="text-xs text-muted-foreground mt-1">
                  Your cash balance is healthy. Good time to invest in new stock.
                </p>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
    
    <!-- Recent Transactions -->
    <Card>
      <CardHeader>
        <CardTitle>Recent Transactions</CardTitle>
      </CardHeader>
      <CardContent>
        <div class="space-y-4">
          <div v-for="transaction in recentTransactions" :key="transaction.id" class="flex items-center justify-between p-4 border rounded-lg">
            <div class="flex items-center space-x-4">
              <div class="w-10 h-10 rounded-full flex items-center justify-center"
                   :class="transaction.type === 'sale' ? 'bg-primary/10' : 'bg-muted'">
                <Icon :name="transaction.type === 'sale' ? 'lucide:arrow-down' : 'lucide:arrow-up'" 
                      class="w-5 h-5"
                      :class="transaction.type === 'sale' ? 'text-primary' : 'text-muted-foreground'" />
              </div>
              <div>
                <p class="font-medium">{{ transaction.description }}</p>
                <p class="text-sm text-muted-foreground">{{ transaction.time }}</p>
              </div>
            </div>
            <div class="text-right">
              <p class="font-medium"
                 :class="transaction.type === 'sale' ? 'text-primary' : 'text-muted-foreground'">
                {{ transaction.type === 'sale' ? '+' : '-' }}R {{ formatCurrency(transaction.amount) }}
              </p>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'admin'
})

const todaySales = ref(5420)
const lowStockCount = ref(8)
const pendingOrders = ref(3)
const cashBalance = ref(12500)

const recentTransactions = ref([
  { id: 1, type: 'sale', description: 'Sale - Bread, Milk, Sugar', amount: 45, time: '2 hours ago' },
  { id: 2, type: 'purchase', description: 'Stock Order - Wholesaler ABC', amount: 1200, time: '5 hours ago' },
  { id: 3, type: 'sale', description: 'Sale - Airtime, Snacks', amount: 85, time: '1 day ago' },
  { id: 4, type: 'sale', description: 'Sale - Cooking Oil, Maize Meal', amount: 120, time: '1 day ago' }
])

const formatCurrency = (amount: number) => {
  return amount.toLocaleString('en-ZA', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

useHead({
  title: 'Dashboard - TOSS'
})
</script>


