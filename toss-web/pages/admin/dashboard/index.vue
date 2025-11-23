<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useApi } from '~/composables/useApi'
import { DollarSign, Users, Truck, ShoppingCart, TrendingUp, Settings } from 'lucide-vue-next'

definePageMeta({
  layout: 'dashboard',
  middleware: 'auth',
  meta: {
    roles: ['Administrator']
  }
})

const { get } = useApi()
const isLoading = ref(true)
const stats = ref({
  activeRetailers: 0,
  activeSuppliers: 0,
  activeDrivers: 0,
  totalSalesToday: 0,
  ordersByStatus: {
    'Draft': 0,
    'Submitted': 0,
    'Accepted': 0,
    'Shipped': 0,
    'Delivered': 0
  }
})

const loadStats = async () => {
  isLoading.value = true
  try {
    // TODO: Replace with actual API calls when backend endpoints are ready
    const users = await get('/api/users/list?take=1000')
    const retailers = users.filter((u: any) => u.roles?.includes('StoreOwner') || u.roles?.includes('Vendor'))
    const suppliers = users.filter((u: any) => u.roles?.includes('Supplier'))
    const drivers = users.filter((u: any) => u.roles?.includes('Driver'))

    stats.value = {
      activeRetailers: retailers.length,
      activeSuppliers: suppliers.length,
      activeDrivers: drivers.length,
      totalSalesToday: 0,
      ordersByStatus: {
        'Draft': 0,
        'Submitted': 0,
        'Accepted': 0,
        'Shipped': 0,
        'Delivered': 0
      }
    }
  } catch (error) {
    console.error('Error loading stats:', error)
  } finally {
    isLoading.value = false
  }
}

const getStatusColor = (status: string) => {
  const colors: Record<string, string> = {
    'Draft': 'bg-gray-100 text-gray-800',
    'Submitted': 'bg-blue-100 text-blue-800',
    'Accepted': 'bg-green-100 text-green-800',
    'Shipped': 'bg-yellow-100 text-yellow-800',
    'Delivered': 'bg-purple-100 text-purple-800'
  }
  return colors[status] || 'bg-gray-100 text-gray-800'
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
      <h1 class="text-2xl font-bold text-foreground">Admin Dashboard</h1>
      <p class="text-sm text-muted-foreground mt-1">Overview of system activity and statistics</p>
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
        <!-- Active Retailers -->
        <div class="toss-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Active Retailers</p>
              <p class="text-2xl font-bold text-foreground mt-2">{{ stats.activeRetailers }}</p>
              <p class="text-xs text-muted-foreground mt-1">Registered shops</p>
            </div>
            <div class="p-3 bg-primary/10 rounded-lg">
              <ShoppingCart class="w-6 h-6 text-primary" />
            </div>
          </div>
        </div>

        <!-- Active Suppliers -->
        <div class="toss-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Active Suppliers</p>
              <p class="text-2xl font-bold text-foreground mt-2">{{ stats.activeSuppliers }}</p>
              <p class="text-xs text-muted-foreground mt-1">Partner suppliers</p>
            </div>
            <div class="p-3 bg-primary/10 rounded-lg">
              <Users class="w-6 h-6 text-primary" />
            </div>
          </div>
        </div>

        <!-- Active Drivers -->
        <div class="toss-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Active Drivers</p>
              <p class="text-2xl font-bold text-foreground mt-2">{{ stats.activeDrivers }}</p>
              <p class="text-xs text-muted-foreground mt-1">Delivery drivers</p>
            </div>
            <div class="p-3 bg-primary/10 rounded-lg">
              <Truck class="w-6 h-6 text-primary" />
            </div>
          </div>
        </div>

        <!-- Total Sales Today -->
        <div class="toss-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Total Sales Today</p>
              <p class="text-2xl font-bold text-foreground mt-2">
                {{ formatCurrency(stats.totalSalesToday) }}
              </p>
              <p class="text-xs text-muted-foreground mt-1">Across all shops</p>
            </div>
            <div class="p-3 bg-primary/10 rounded-lg">
              <DollarSign class="w-6 h-6 text-primary" />
            </div>
          </div>
        </div>
      </div>

      <!-- Orders by Status -->
      <div class="toss-card p-6">
        <div class="flex items-center justify-between mb-6">
          <h2 class="text-lg font-semibold text-foreground">Orders by Status</h2>
          <NuxtLink
            to="/admin/orders"
            class="text-sm text-primary hover:text-primary/80 font-medium"
          >
            View all
          </NuxtLink>
        </div>
        <div class="grid grid-cols-2 gap-4 md:grid-cols-5">
          <div
            v-for="(count, status) in stats.ordersByStatus"
            :key="status"
            class="text-center p-4 bg-muted/50 rounded-lg"
          >
            <p class="text-2xl font-bold text-foreground">{{ count }}</p>
            <p class="text-xs text-muted-foreground mt-1">{{ status }}</p>
          </div>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="toss-card p-6">
        <h2 class="text-lg font-semibold text-foreground mb-4">Quick Actions</h2>
        <div class="grid grid-cols-2 gap-4 md:grid-cols-4">
          <NuxtLink
            to="/admin/users"
            class="p-4 bg-primary/5 hover:bg-primary/10 rounded-lg text-center transition-colors"
          >
            <Users class="w-8 h-8 mx-auto mb-2 text-primary" />
            <p class="text-sm font-medium text-foreground">Manage Users</p>
          </NuxtLink>
          <NuxtLink
            to="/admin/orders"
            class="p-4 bg-primary/5 hover:bg-primary/10 rounded-lg text-center transition-colors"
          >
            <ShoppingCart class="w-8 h-8 mx-auto mb-2 text-primary" />
            <p class="text-sm font-medium text-foreground">View Orders</p>
          </NuxtLink>
          <NuxtLink
            to="/sales/reports/analytics"
            class="p-4 bg-primary/5 hover:bg-primary/10 rounded-lg text-center transition-colors"
          >
            <TrendingUp class="w-8 h-8 mx-auto mb-2 text-primary" />
            <p class="text-sm font-medium text-foreground">Analytics</p>
          </NuxtLink>
          <NuxtLink
            to="/settings"
            class="p-4 bg-primary/5 hover:bg-primary/10 rounded-lg text-center transition-colors"
          >
            <Settings class="w-8 h-8 mx-auto mb-2 text-primary" />
            <p class="text-sm font-medium text-foreground">Settings</p>
          </NuxtLink>
        </div>
      </div>
    </div>
  </div>
</template>
