<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useApi } from '~/composables/useApi'
import { FileText, Package, TrendingUp, CheckCircle, XCircle, Clock } from 'lucide-vue-next'

definePageMeta({
  layout: 'dashboard',
  middleware: 'auth',
  meta: {
    roles: ['Supplier']
  }
})

const { get } = useApi()
const isLoading = ref(true)
const stats = ref({
  totalOrders: 0,
  pendingOrders: 0,
  acceptedOrders: 0,
  rejectedOrders: 0,
  totalProducts: 0
})

const loadStats = async () => {
  isLoading.value = true
  try {
    // TODO: Replace with actual API calls
    stats.value = {
      totalOrders: 0,
      pendingOrders: 0,
      acceptedOrders: 0,
      rejectedOrders: 0,
      totalProducts: 0
    }
  } catch (error) {
    console.error('Error loading stats:', error)
  } finally {
    isLoading.value = false
  }
}

onMounted(() => {
  loadStats()
})
</script>

<template>
  <div class="space-y-6">
    <!-- Page Header -->
    <div>
      <h1 class="text-2xl font-bold text-foreground">Supplier Dashboard</h1>
      <p class="text-sm text-muted-foreground mt-1">Manage orders and products</p>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="flex items-center justify-center py-12">
      <div class="w-8 h-8 border-4 border-primary border-t-transparent rounded-full animate-spin" />
      <p class="ml-3 text-sm text-muted-foreground">Loading dashboard...</p>
    </div>

    <!-- Dashboard Content -->
    <div v-else class="space-y-6">
      <!-- Stats Cards -->
      <div class="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-5">
        <!-- Total Orders -->
        <div class="toss-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Total Orders</p>
              <p class="text-2xl font-bold text-foreground mt-2">{{ stats.totalOrders }}</p>
            </div>
            <div class="p-3 bg-primary/10 rounded-lg">
              <FileText class="w-6 h-6 text-primary" />
            </div>
          </div>
        </div>

        <!-- Pending Orders -->
        <div class="toss-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Pending</p>
              <p class="text-2xl font-bold text-foreground mt-2">{{ stats.pendingOrders }}</p>
            </div>
            <div class="p-3 bg-primary/10 rounded-lg">
              <Clock class="w-6 h-6 text-primary" />
            </div>
          </div>
        </div>

        <!-- Accepted Orders -->
        <div class="toss-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Accepted</p>
              <p class="text-2xl font-bold text-foreground mt-2">{{ stats.acceptedOrders }}</p>
            </div>
            <div class="p-3 bg-primary/10 rounded-lg">
              <CheckCircle class="w-6 h-6 text-primary" />
            </div>
          </div>
        </div>

        <!-- Rejected Orders -->
        <div class="toss-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Rejected</p>
              <p class="text-2xl font-bold text-foreground mt-2">{{ stats.rejectedOrders }}</p>
            </div>
            <div class="p-3 bg-primary/10 rounded-lg">
              <XCircle class="w-6 h-6 text-primary" />
            </div>
          </div>
        </div>

        <!-- Total Products -->
        <div class="toss-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Products</p>
              <p class="text-2xl font-bold text-foreground mt-2">{{ stats.totalProducts }}</p>
            </div>
            <div class="p-3 bg-primary/10 rounded-lg">
              <Package class="w-6 h-6 text-primary" />
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="toss-card p-6">
        <h2 class="text-lg font-semibold text-foreground mb-4">Quick Actions</h2>
        <div class="grid grid-cols-2 gap-4 md:grid-cols-4">
          <NuxtLink
            to="/supplier/orders"
            class="p-4 bg-primary/5 hover:bg-primary/10 rounded-lg text-center transition-colors"
          >
            <FileText class="w-8 h-8 mx-auto mb-2 text-primary" />
            <p class="text-sm font-medium text-foreground">View Orders</p>
          </NuxtLink>
          <NuxtLink
            to="/supplier/products"
            class="p-4 bg-primary/5 hover:bg-primary/10 rounded-lg text-center transition-colors"
          >
            <Package class="w-8 h-8 mx-auto mb-2 text-primary" />
            <p class="text-sm font-medium text-foreground">Products</p>
          </NuxtLink>
          <NuxtLink
            to="/sales/reports/analytics"
            class="p-4 bg-primary/5 hover:bg-primary/10 rounded-lg text-center transition-colors"
          >
            <TrendingUp class="w-8 h-8 mx-auto mb-2 text-primary" />
            <p class="text-sm font-medium text-foreground">Analytics</p>
          </NuxtLink>
        </div>
      </div>

      <!-- Recent Orders -->
      <div class="toss-card p-6">
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-lg font-semibold text-foreground">Recent Orders</h2>
          <NuxtLink
            to="/supplier/orders"
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
              <p class="text-sm font-medium text-foreground">Order #{{ 1000 + i }}</p>
              <p class="text-xs text-muted-foreground">From Shop {{ i }}</p>
            </div>
            <span class="px-3 py-1 text-xs font-medium bg-primary/10 text-primary rounded-full">
              Pending
            </span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
