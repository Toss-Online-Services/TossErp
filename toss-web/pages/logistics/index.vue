<script setup lang="ts">
import { ref } from 'vue'
import { Truck, User } from 'lucide-vue-next'
import AppCard from '~/components/common/AppCard.vue'
import AppSectionHeader from '~/components/common/AppSectionHeader.vue'

useHead({
  title: 'Logistics - TOSS Admin',
  meta: [{ name: 'description', content: 'Logistics and deliveries for TOSS Admin' }]
})

definePageMeta({
  layout: 'dashboard'
})

const todayDeliveries = ref([
  { id: 1, order: 'ORD-001', customer: 'John Doe', address: '123 Main St', status: 'In Progress', driver: 'Driver A' },
  { id: 2, order: 'ORD-002', customer: 'Jane Smith', address: '456 Oak Ave', status: 'Scheduled', driver: 'Driver B' }
])

const inProgressDeliveries = ref([
  { id: 1, order: 'ORD-001', customer: 'John Doe', address: '123 Main St', status: 'In Progress', driver: 'Driver A' }
])

const doneDeliveries = ref([
  { id: 3, order: 'ORD-003', customer: 'Mike Johnson', address: '789 Pine Rd', status: 'Delivered', driver: 'Driver A' }
])

const drivers = ref([
  { id: 1, name: 'Driver A', phone: '+27 82 111 2222', vehicle: 'Van ABC-123', deliveries: 12, status: 'Active' },
  { id: 2, name: 'Driver B', phone: '+27 83 222 3333', vehicle: 'Truck XYZ-456', deliveries: 8, status: 'Active' }
])
</script>

<template>
  <div class="space-y-6">
    <AppSectionHeader
      title="Logistics & Deliveries"
      description="Manage deliveries and drivers"
    />

    <!-- Deliveries Board -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Today -->
      <AppCard title="Today" :subtitle="`${todayDeliveries.length} deliveries`">
        <div class="space-y-3">
          <div
            v-for="delivery in todayDeliveries"
            :key="delivery.id"
            class="p-4 border border-border rounded-lg hover:bg-muted transition-colors"
          >
            <p class="font-medium text-foreground">{{ delivery.order }}</p>
            <p class="text-sm text-muted-foreground mt-1">{{ delivery.customer }}</p>
            <p class="text-xs text-muted-foreground mt-1">{{ delivery.address }}</p>
            <p class="text-xs text-muted-foreground mt-1">Driver: {{ delivery.driver }}</p>
          </div>
        </div>
      </AppCard>

      <!-- In Progress -->
      <AppCard title="In Progress" :subtitle="`${inProgressDeliveries.length} active`">
        <div class="space-y-3">
          <div
            v-for="delivery in inProgressDeliveries"
            :key="delivery.id"
            class="p-4 border border-border rounded-lg hover:bg-muted transition-colors"
          >
            <p class="font-medium text-foreground">{{ delivery.order }}</p>
            <p class="text-sm text-muted-foreground mt-1">{{ delivery.customer }}</p>
            <p class="text-xs text-muted-foreground mt-1">{{ delivery.address }}</p>
            <p class="text-xs text-muted-foreground mt-1">Driver: {{ delivery.driver }}</p>
          </div>
        </div>
      </AppCard>

      <!-- Done -->
      <AppCard title="Done" :subtitle="`${doneDeliveries.length} completed`">
        <div class="space-y-3">
          <div
            v-for="delivery in doneDeliveries"
            :key="delivery.id"
            class="p-4 border border-border rounded-lg hover:bg-muted transition-colors"
          >
            <p class="font-medium text-foreground">{{ delivery.order }}</p>
            <p class="text-sm text-muted-foreground mt-1">{{ delivery.customer }}</p>
            <p class="text-xs text-muted-foreground mt-1">{{ delivery.address }}</p>
            <p class="text-xs text-muted-foreground mt-1">Driver: {{ delivery.driver }}</p>
          </div>
        </div>
      </AppCard>
        </div>

    <!-- Driver List -->
    <AppCard title="Drivers">
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div
          v-for="driver in drivers"
          :key="driver.id"
          class="p-4 border border-border rounded-lg hover:bg-muted transition-colors"
        >
          <div class="flex items-start justify-between">
            <div>
              <p class="font-medium text-foreground">{{ driver.name }}</p>
              <p class="text-sm text-muted-foreground mt-1">{{ driver.phone }}</p>
              <p class="text-sm text-muted-foreground mt-1">{{ driver.vehicle }}</p>
              <p class="text-xs text-muted-foreground mt-1">{{ driver.deliveries }} deliveries</p>
            </div>
            <span
              :class="[
                'inline-flex items-center px-2 py-1 rounded text-xs font-medium',
                driver.status === 'Active' ? 'bg-green-100 dark:bg-green-900/20 text-green-700 dark:text-green-400' : ''
              ]"
            >
              {{ driver.status }}
                    </span>
          </div>
        </div>
      </div>
    </AppCard>
  </div>
  </template>
