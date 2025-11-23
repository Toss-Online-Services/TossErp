<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-3xl font-bold">Suppliers</h1>
        <p class="text-muted-foreground">Manage your supplier network</p>
      </div>
      <Button>
        <Icon name="lucide:plus" class="w-4 h-4 mr-2" />
        Add Supplier
      </Button>
    </div>
    
    <!-- Stats -->
    <div class="grid gap-4 md:grid-cols-4">
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Total Suppliers</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ totalSuppliers }}</div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Active This Month</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ activeSuppliers }}</div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Network Partners</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ networkPartners }}</div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Pending Orders</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ pendingOrders }}</div>
        </CardContent>
      </Card>
    </div>
    
    <!-- Suppliers Grid -->
    <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      <Card v-for="supplier in suppliers" :key="supplier.id" class="hover:shadow-lg transition-shadow">
        <CardHeader>
          <div class="flex items-center justify-between">
            <div>
              <CardTitle>{{ supplier.name }}</CardTitle>
              <p class="text-sm text-muted-foreground mt-1">{{ supplier.category }}</p>
            </div>
            <div class="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center">
              <Icon name="lucide:truck" class="w-6 h-6 text-primary" />
            </div>
          </div>
        </CardHeader>
        <CardContent class="space-y-3">
          <div class="text-sm">
            <p class="text-muted-foreground">Contact</p>
            <p>{{ supplier.contact }}</p>
          </div>
          <div class="text-sm">
            <p class="text-muted-foreground">Rating</p>
            <div class="flex items-center space-x-1">
              <Icon v-for="i in 5" :key="i" name="lucide:star"
                    :class="i <= supplier.rating ? 'text-yellow-500 fill-yellow-500' : 'text-muted-foreground'"
                    class="w-4 h-4" />
              <span class="text-sm ml-1">({{ supplier.reviews }})</span>
            </div>
          </div>
          <div class="text-sm">
            <p class="text-muted-foreground">Network Status</p>
            <span class="px-2 py-1 rounded-full text-xs font-medium"
                  :class="supplier.inNetwork ? 'bg-primary/10 text-primary' : 'bg-muted text-muted-foreground'">
              {{ supplier.inNetwork ? 'Network Partner' : 'Direct Supplier' }}
            </span>
          </div>
          <div class="flex gap-2 pt-2">
            <Button variant="outline" size="sm" class="flex-1">View</Button>
            <Button size="sm" class="flex-1">Order</Button>
          </div>
        </CardContent>
      </Card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

definePageMeta({
  layout: 'admin'
})

const totalSuppliers = ref(12)
const activeSuppliers = ref(8)
const networkPartners = ref(5)
const pendingOrders = ref(3)

const suppliers = ref([
  { id: 1, name: 'Wholesaler ABC', category: 'General Goods', contact: '+27 11 123 4567', rating: 5, reviews: 24, inNetwork: true },
  { id: 2, name: 'Supplier XYZ', category: 'Food & Beverages', contact: '+27 11 234 5678', rating: 4, reviews: 18, inNetwork: true },
  { id: 3, name: 'Local Distributor', category: 'Household Items', contact: '+27 11 345 6789', rating: 4, reviews: 12, inNetwork: false }
])

useHead({
  title: 'Suppliers - TOSS'
})
</script>

