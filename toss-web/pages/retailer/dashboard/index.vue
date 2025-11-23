<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useApi } from '~/composables/useApi'
import { DollarSign, Package, AlertTriangle, FileText, Plus, ShoppingCart } from 'lucide-vue-next'

definePageMeta({
  layout: 'dashboard',
  middleware: 'auth',
  meta: {
    roles: ['StoreOwner', 'Vendor', 'Retailer']
  }
})

const { get } = useApi()
const isLoading = ref(true)
const stats = ref({
  todaySales: 0,
  totalProducts: 0,
  lowStockItems: 0,
  pendingOrders: 0
})

const loadStats = async () => {
  isLoading.value = true
  try {
    // TODO: Replace with actual API calls
    stats.value = {
      todaySales: 0,
      totalProducts: 0,
      lowStockItems: 0,
      pendingOrders: 0
    }
  } catch (error) {
    console.error('Error loading stats:', error)
  } finally {
    isLoading.value = false
  }
}

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
    minimumFractionDigits: 2
  }).format(value)
}

onMounted(() => {
  loadStats()
})
</script>

<template>
  <div class="space-y-6">
    <!-- Page Header -->
    <div>
      <h1 class="text-2xl font-bold text-foreground">Dashboard</h1>
      <p class="text-sm text-muted-foreground mt-1">Your business at a glance</p>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="flex items-center justify-center py-12">
      <div class="w-8 h-8 border-4 border-primary border-t-transparent rounded-full animate-spin" />
      <p class="ml-3 text-sm text-muted-foreground">Loading dashboard...</p>
    </div>

    <!-- Dashboard Content -->
    <div v-else class="space-y-6">
      <!-- Stats Cards -->
      <div class="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-4">
        <!-- Today's Sales -->
        <div class="toss-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Today's Sales</p>
              <p class="text-2xl font-bold text-foreground mt-2">
                {{ formatCurrency(stats.todaySales) }}
              </p>
              <p class="text-xs text-muted-foreground mt-1">Total revenue today</p>
            </div>
            <div class="p-3 bg-primary/10 rounded-lg">
              <DollarSign class="w-6 h-6 text-primary" />
            </div>
          </div>
        </div>

        <!-- Total Products -->
        <div class="toss-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Total Products</p>
              <p class="text-2xl font-bold text-foreground mt-2">{{ stats.totalProducts }}</p>
              <p class="text-xs text-muted-foreground mt-1">Active products</p>
            </div>
            <div class="p-3 bg-primary/10 rounded-lg">
              <Package class="w-6 h-6 text-primary" />
            </div>
          </div>
        </div>

        <!-- Low Stock Items -->
        <div class="toss-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Low Stock Items</p>
              <p class="text-2xl font-bold text-foreground mt-2">{{ stats.lowStockItems }}</p>
              <p class="text-xs text-muted-foreground mt-1">Need restocking</p>
            </div>
            <div class="p-3 bg-primary/10 rounded-lg">
              <AlertTriangle class="w-6 h-6 text-primary" />
            </div>
          </div>
        </div>

        <!-- Pending Orders -->
        <div class="toss-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Pending Orders</p>
              <p class="text-2xl font-bold text-foreground mt-2">{{ stats.pendingOrders }}</p>
              <p class="text-xs text-muted-foreground mt-1">Awaiting delivery</p>
            </div>
            <div class="p-3 bg-primary/10 rounded-lg">
              <FileText class="w-6 h-6 text-primary" />
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="toss-card p-6">
        <h2 class="text-lg font-semibold text-foreground mb-4">Quick Actions</h2>
        <div class="grid grid-cols-2 gap-4 md:grid-cols-4">
          <NuxtLink
            to="/sales/pos"
            class="p-4 bg-primary/5 hover:bg-primary/10 rounded-lg text-center transition-colors"
          >
            <ShoppingCart class="w-8 h-8 mx-auto mb-2 text-primary" />
            <p class="text-sm font-medium text-foreground">POS</p>
          </NuxtLink>
          <NuxtLink
            to="/retailer/products"
            class="p-4 bg-primary/5 hover:bg-primary/10 rounded-lg text-center transition-colors"
          >
            <Plus class="w-8 h-8 mx-auto mb-2 text-primary" />
            <p class="text-sm font-medium text-foreground">Add Product</p>
          </NuxtLink>
          <NuxtLink
            to="/retailer/orders/new"
            class="p-4 bg-primary/5 hover:bg-primary/10 rounded-lg text-center transition-colors"
          >
            <FileText class="w-8 h-8 mx-auto mb-2 text-primary" />
            <p class="text-sm font-medium text-foreground">New Order</p>
          </NuxtLink>
          <NuxtLink
            to="/retailer/inventory"
            class="p-4 bg-primary/5 hover:bg-primary/10 rounded-lg text-center transition-colors"
          >
            <Package class="w-8 h-8 mx-auto mb-2 text-primary" />
            <p class="text-sm font-medium text-foreground">Inventory</p>
          </NuxtLink>
        </div>
      </div>

      <!-- Recent Activity -->
      <div class="grid grid-cols-1 gap-6 lg:grid-cols-2">
        <!-- Recent Sales -->
        <div class="toss-card p-6">
          <div class="flex items-center justify-between mb-4">
            <h2 class="text-lg font-semibold text-foreground">Recent Sales</h2>
            <NuxtLink
              to="/sales"
              class="text-sm text-primary hover:text-primary/80 font-medium"
            >
              View all
            </NuxtLink>
          </div>
          <div class="space-y-4">
            <div
              v-for="i in 5"
              :key="i"
              class="flex items-center justify-between p-3 bg-muted/50 rounded-lg"
            >
              <div>
                <p class="text-sm font-medium text-foreground">Sale #{{ i }}</p>
                <p class="text-xs text-muted-foreground">2 hours ago</p>
              </div>
              <p class="text-sm font-semibold text-foreground">R{{ 100 * i }}.00</p>
            </div>
          </div>
        </div>

        <!-- Low Stock Alerts -->
        <div class="toss-card p-6">
          <div class="flex items-center justify-between mb-4">
            <h2 class="text-lg font-semibold text-foreground">Low Stock Alerts</h2>
            <NuxtLink
              to="/retailer/inventory"
              class="text-sm text-primary hover:text-primary/80 font-medium"
            >
              View all
            </NuxtLink>
          </div>
          <div class="space-y-4">
            <div
              v-for="i in 5"
              :key="i"
              class="flex items-center justify-between p-3 bg-muted/50 rounded-lg"
            >
              <div>
                <p class="text-sm font-medium text-foreground">Product {{ i }}</p>
                <p class="text-xs text-muted-foreground">{{ 5 - i }} units remaining</p>
              </div>
              <AlertTriangle class="w-5 h-5 text-yellow-500" />
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
