<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-3xl font-bold">Financials</h1>
      <p class="text-muted-foreground">Track your business finances and cash flow</p>
    </div>
    
    <!-- Financial Overview -->
    <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Total Revenue</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">R {{ formatCurrency(totalRevenue) }}</div>
          <p class="text-xs text-muted-foreground mt-1">This month</p>
        </CardContent>
      </Card>
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Total Expenses</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">R {{ formatCurrency(totalExpenses) }}</div>
          <p class="text-xs text-muted-foreground mt-1">This month</p>
        </CardContent>
      </Card>
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Net Profit</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold" :class="netProfit >= 0 ? 'text-primary' : 'text-destructive'">
            R {{ formatCurrency(netProfit) }}
          </div>
          <p class="text-xs text-muted-foreground mt-1">This month</p>
        </CardContent>
      </Card>
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Cash Balance</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">R {{ formatCurrency(cashBalance) }}</div>
          <p class="text-xs text-muted-foreground mt-1">Available now</p>
        </CardContent>
      </Card>
    </div>
    
    <!-- Charts -->
    <div class="grid gap-4 lg:grid-cols-2">
      <Card>
        <CardHeader>
          <CardTitle>Revenue vs Expenses</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="h-[300px] flex items-center justify-center text-muted-foreground">
            <div class="text-center">
              <Icon name="lucide:trending-up" class="w-12 h-12 mx-auto mb-2 opacity-50" />
              <p>Financial chart will be displayed here</p>
            </div>
          </div>
        </CardContent>
      </Card>
      
      <Card>
        <CardHeader>
          <CardTitle>Cash Flow</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="h-[300px] flex items-center justify-center text-muted-foreground">
            <div class="text-center">
              <Icon name="lucide:wallet" class="w-12 h-12 mx-auto mb-2 opacity-50" />
              <p>Cash flow chart will be displayed here</p>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
    
    <!-- Transactions -->
    <Card>
      <CardHeader>
        <div class="flex items-center justify-between">
          <CardTitle>Recent Transactions</CardTitle>
          <div class="flex gap-2">
            <Button variant="outline" size="sm">All</Button>
            <Button variant="ghost" size="sm">Income</Button>
            <Button variant="ghost" size="sm">Expenses</Button>
          </div>
        </div>
      </CardHeader>
      <CardContent>
        <div class="space-y-4">
          <div v-for="transaction in transactions" :key="transaction.id" class="flex items-center justify-between p-4 border rounded-lg">
            <div class="flex items-center space-x-4">
              <div class="w-10 h-10 rounded-full flex items-center justify-center"
                   :class="transaction.type === 'income' ? 'bg-primary/10' : 'bg-destructive/10'">
                <Icon :name="transaction.type === 'income' ? 'lucide:arrow-down' : 'lucide:arrow-up'"
                      class="w-5 h-5"
                      :class="transaction.type === 'income' ? 'text-primary' : 'text-destructive'" />
              </div>
              <div>
                <p class="font-medium">{{ transaction.description }}</p>
                <p class="text-sm text-muted-foreground">{{ transaction.category }} • {{ transaction.date }}</p>
              </div>
            </div>
            <div class="text-right">
              <p class="font-medium"
                 :class="transaction.type === 'income' ? 'text-primary' : 'text-destructive'">
                {{ transaction.type === 'income' ? '+' : '-' }}R {{ formatCurrency(transaction.amount) }}
              </p>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

definePageMeta({
  layout: 'admin'
})

const totalRevenue = ref(28500)
const totalExpenses = ref(18000)
const netProfit = computed(() => totalRevenue.value - totalExpenses.value)
const cashBalance = ref(12500)

const transactions = ref([
  { id: 1, type: 'income', description: 'Daily Sales', category: 'Sales', amount: 5420, date: '2025-01-15' },
  { id: 2, type: 'expense', description: 'Stock Purchase', category: 'Purchasing', amount: 3500, date: '2025-01-14' },
  { id: 3, type: 'income', description: 'Daily Sales', category: 'Sales', amount: 4850, date: '2025-01-14' },
  { id: 4, type: 'expense', description: 'Rent', category: 'Operating', amount: 2500, date: '2025-01-10' }
])

const formatCurrency = (amount: number) => {
  return amount.toLocaleString('en-ZA', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

useHead({
  title: 'Financials - TOSS'
})
</script>

