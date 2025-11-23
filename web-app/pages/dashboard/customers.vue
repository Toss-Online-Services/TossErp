<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-3xl font-bold">Customers</h1>
        <p class="text-muted-foreground">Manage your customer relationships</p>
      </div>
      <Button>
        <Icon name="lucide:plus" class="w-4 h-4 mr-2" />
        Add Customer
      </Button>
    </div>
    
    <!-- Stats -->
    <div class="grid gap-4 md:grid-cols-4">
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Total Customers</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ totalCustomers }}</div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Credit Sales</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">R {{ formatCurrency(creditSales) }}</div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Outstanding</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">R {{ formatCurrency(outstanding) }}</div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Active This Month</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ activeCustomers }}</div>
        </CardContent>
      </Card>
    </div>
    
    <!-- Customers Table -->
    <Card>
      <CardHeader>
        <div class="flex items-center justify-between">
          <CardTitle>Customer List</CardTitle>
          <Input placeholder="Search customers..." class="w-64" />
        </div>
      </CardHeader>
      <CardContent>
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead>
              <tr class="border-b">
                <th class="text-left p-4 font-medium">Name</th>
                <th class="text-left p-4 font-medium">Contact</th>
                <th class="text-left p-4 font-medium">Total Purchases</th>
                <th class="text-left p-4 font-medium">Outstanding</th>
                <th class="text-left p-4 font-medium">Last Purchase</th>
                <th class="text-right p-4 font-medium">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="customer in customers" :key="customer.id" class="border-b hover:bg-muted/50">
                <td class="p-4">
                  <div class="font-medium">{{ customer.name }}</div>
                  <div class="text-sm text-muted-foreground">{{ customer.type }}</div>
                </td>
                <td class="p-4">
                  <div>{{ customer.phone }}</div>
                  <div class="text-sm text-muted-foreground">{{ customer.email }}</div>
                </td>
                <td class="p-4">R {{ formatCurrency(customer.totalPurchases) }}</td>
                <td class="p-4">
                  <span :class="customer.outstanding > 0 ? 'text-destructive font-medium' : 'text-muted-foreground'">
                    R {{ formatCurrency(customer.outstanding) }}
                  </span>
                </td>
                <td class="p-4">{{ customer.lastPurchase }}</td>
                <td class="p-4 text-right">
                  <div class="flex items-center justify-end space-x-2">
                    <Button variant="ghost" size="sm">
                      <Icon name="lucide:eye" class="w-4 h-4" />
                    </Button>
                    <Button variant="ghost" size="sm">
                      <Icon name="lucide:edit" class="w-4 h-4" />
                    </Button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

definePageMeta({
  layout: 'admin'
})

const totalCustomers = ref(156)
const creditSales = ref(12500)
const outstanding = ref(3200)
const activeCustomers = ref(89)

const customers = ref([
  { id: 1, name: 'John Doe', type: 'Regular', phone: '+27 12 345 6789', email: 'john@example.com', totalPurchases: 4500, outstanding: 0, lastPurchase: '2025-01-15' },
  { id: 2, name: 'Jane Smith', type: 'Credit', phone: '+27 12 345 6790', email: 'jane@example.com', totalPurchases: 3200, outstanding: 450, lastPurchase: '2025-01-14' },
  { id: 3, name: 'Mike Johnson', type: 'Regular', phone: '+27 12 345 6791', email: 'mike@example.com', totalPurchases: 2800, outstanding: 0, lastPurchase: '2025-01-13' }
])

const formatCurrency = (amount: number) => {
  return amount.toLocaleString('en-ZA', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

useHead({
  title: 'Customers - TOSS'
})
</script>

