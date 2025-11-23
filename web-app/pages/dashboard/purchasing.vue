<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-3xl font-bold">Purchasing</h1>
        <p class="text-muted-foreground">Manage purchase orders and suppliers</p>
      </div>
      <Button>
        <Icon name="lucide:plus" class="w-4 h-4 mr-2" />
        New Purchase Order
      </Button>
    </div>
    
    <!-- Stats -->
    <div class="grid gap-4 md:grid-cols-4">
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Pending Orders</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ pendingOrders }}</div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">This Month</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">R {{ formatCurrency(monthPurchases) }}</div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Suppliers</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ supplierCount }}</div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Group Orders</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ groupOrders }}</div>
        </CardContent>
      </Card>
    </div>
    
    <!-- Purchase Orders -->
    <Card>
      <CardHeader>
        <div class="flex items-center justify-between">
          <CardTitle>Purchase Orders</CardTitle>
          <div class="flex gap-2">
            <Button variant="outline" size="sm">Pending</Button>
            <Button variant="ghost" size="sm">Completed</Button>
            <Button variant="ghost" size="sm">Cancelled</Button>
          </div>
        </div>
      </CardHeader>
      <CardContent>
        <div class="space-y-4">
          <div v-for="order in purchaseOrders" :key="order.id" class="p-4 border rounded-lg hover:bg-muted/50">
            <div class="flex items-center justify-between mb-3">
              <div>
                <p class="font-medium">PO-{{ order.id.toString().padStart(4, '0') }}</p>
                <p class="text-sm text-muted-foreground">{{ order.supplier }}</p>
              </div>
              <div class="text-right">
                <p class="font-medium">R {{ formatCurrency(order.total) }}</p>
                <span class="px-2 py-1 rounded-full text-xs font-medium"
                      :class="getStatusClass(order.status)">
                  {{ order.status }}
                </span>
              </div>
            </div>
            <div class="flex items-center justify-between text-sm text-muted-foreground">
              <span>{{ order.items }} items</span>
              <span>Ordered: {{ order.date }}</span>
            </div>
            <div v-if="order.status === 'Pending'" class="mt-3 flex gap-2">
              <Button size="sm" variant="outline">View Details</Button>
              <Button size="sm">Approve</Button>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
    
    <!-- Group Buying Opportunities -->
    <Card>
      <CardHeader>
        <CardTitle>Group Buying Opportunities</CardTitle>
      </CardHeader>
      <CardContent>
        <div class="space-y-4">
          <div v-for="opportunity in groupOpportunities" :key="opportunity.id" class="p-4 border rounded-lg">
            <div class="flex items-start justify-between mb-3">
              <div>
                <p class="font-medium">{{ opportunity.product }}</p>
                <p class="text-sm text-muted-foreground">{{ opportunity.supplier }}</p>
              </div>
              <div class="text-right">
                <p class="text-sm text-muted-foreground line-through">R {{ formatCurrency(opportunity.regularPrice) }}</p>
                <p class="font-medium text-primary">R {{ formatCurrency(opportunity.groupPrice) }}</p>
              </div>
            </div>
            <div class="flex items-center justify-between text-sm mb-3">
              <span class="text-muted-foreground">{{ opportunity.participants }} shops participating</span>
              <span class="text-muted-foreground">Min: {{ opportunity.minQuantity }} units</span>
            </div>
            <div class="w-full bg-muted rounded-full h-2 mb-3">
              <div class="bg-primary h-2 rounded-full"
                   :style="{ width: `${(opportunity.currentQuantity / opportunity.minQuantity) * 100}%` }">
              </div>
            </div>
            <Button class="w-full" size="sm">Join Group Order</Button>
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

const pendingOrders = ref(3)
const monthPurchases = ref(45000)
const supplierCount = ref(12)
const groupOrders = ref(5)

const purchaseOrders = ref([
  { id: 1, supplier: 'Wholesaler ABC', items: 15, total: 3500, date: '2025-01-14', status: 'Pending' },
  { id: 2, supplier: 'Supplier XYZ', items: 8, total: 1200, date: '2025-01-13', status: 'Completed' },
  { id: 3, supplier: 'Local Distributor', items: 20, total: 4500, date: '2025-01-12', status: 'Pending' }
])

const groupOpportunities = ref([
  { id: 1, product: 'Cooking Oil 2L', supplier: 'Wholesaler ABC', regularPrice: 45, groupPrice: 38, participants: 8, minQuantity: 100, currentQuantity: 75 },
  { id: 2, product: 'Maize Meal 5kg', supplier: 'Supplier XYZ', regularPrice: 65, groupPrice: 55, participants: 12, minQuantity: 200, currentQuantity: 180 }
])

const formatCurrency = (amount: number) => {
  return amount.toLocaleString('en-ZA', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

const getStatusClass = (status: string) => {
  if (status === 'Completed') return 'bg-primary/10 text-primary'
  if (status === 'Pending') return 'bg-yellow-500/10 text-yellow-600'
  return 'bg-muted text-muted-foreground'
}

useHead({
  title: 'Purchasing - TOSS'
})
</script>


