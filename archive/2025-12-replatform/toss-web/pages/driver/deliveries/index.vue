<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useApi } from '~/composables/useApi'
import { Truck, MapPin, CheckCircle, Clock, Package } from 'lucide-vue-next'

definePageMeta({
  layout: 'dashboard',
  middleware: 'auth',
  meta: {
    roles: ['Driver']
  }
})

const { get } = useApi()
const isLoading = ref(true)
const stats = ref({
  totalDeliveries: 0,
  pendingDeliveries: 0,
  completedDeliveries: 0,
  inTransit: 0
})

const loadStats = async () => {
  isLoading.value = true
  try {
    // TODO: Replace with actual API calls
    stats.value = {
      totalDeliveries: 0,
      pendingDeliveries: 0,
      completedDeliveries: 0,
      inTransit: 0
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
      <h1 class="text-2xl font-bold text-foreground">Driver Dashboard</h1>
      <p class="text-sm text-muted-foreground mt-1">Manage your deliveries</p>
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
        <!-- Total Deliveries -->
        <div class="toss-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Total Deliveries</p>
              <p class="text-2xl font-bold text-foreground mt-2">{{ stats.totalDeliveries }}</p>
            </div>
            <div class="p-3 bg-primary/10 rounded-lg">
              <Truck class="w-6 h-6 text-primary" />
            </div>
          </div>
        </div>

        <!-- Pending Deliveries -->
        <div class="toss-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Pending</p>
              <p class="text-2xl font-bold text-foreground mt-2">{{ stats.pendingDeliveries }}</p>
            </div>
            <div class="p-3 bg-primary/10 rounded-lg">
              <Clock class="w-6 h-6 text-primary" />
            </div>
          </div>
        </div>

        <!-- In Transit -->
        <div class="toss-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">In Transit</p>
              <p class="text-2xl font-bold text-foreground mt-2">{{ stats.inTransit }}</p>
            </div>
            <div class="p-3 bg-primary/10 rounded-lg">
              <MapPin class="w-6 h-6 text-primary" />
            </div>
          </div>
        </div>

        <!-- Completed -->
        <div class="toss-card p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Completed</p>
              <p class="text-2xl font-bold text-foreground mt-2">{{ stats.completedDeliveries }}</p>
            </div>
            <div class="p-3 bg-primary/10 rounded-lg">
              <CheckCircle class="w-6 h-6 text-primary" />
            </div>
          </div>
        </div>
      </div>

      <!-- Active Deliveries -->
      <div class="toss-card p-6">
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-lg font-semibold text-foreground">Active Deliveries</h2>
        </div>
        <div class="space-y-4">
          <div
            v-for="i in 3"
            :key="i"
            class="p-4 bg-muted/50 rounded-lg border border-border"
          >
            <div class="flex items-center justify-between">
              <div class="flex-1">
                <div class="flex items-center gap-3 mb-2">
                  <Package class="w-5 h-5 text-primary" />
                  <p class="text-sm font-semibold text-foreground">Delivery #{{ 2000 + i }}</p>
                </div>
                <div class="space-y-1 text-xs text-muted-foreground">
                  <p>From: Supplier {{ i }}</p>
                  <p>To: Shop {{ i }}</p>
                  <p>Status: In Transit</p>
                </div>
              </div>
              <NuxtLink
                :to="`/driver/deliveries/${2000 + i}`"
                class="px-4 py-2 text-sm font-medium bg-primary text-primary-foreground rounded-lg hover:bg-primary/90 transition-colors"
              >
                View Details
              </NuxtLink>
            </div>
          </div>
        </div>
      </div>

      <!-- Delivery History -->
      <div class="toss-card p-6">
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-lg font-semibold text-foreground">Recent Deliveries</h2>
        </div>
        <div class="space-y-4">
          <div
            v-for="i in 5"
            :key="i"
            class="flex items-center justify-between p-3 bg-muted/50 rounded-lg"
          >
            <div>
              <p class="text-sm font-medium text-foreground">Delivery #{{ 2000 - i }}</p>
              <p class="text-xs text-muted-foreground">Completed 2 hours ago</p>
            </div>
            <CheckCircle class="w-5 h-5 text-green-500" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
