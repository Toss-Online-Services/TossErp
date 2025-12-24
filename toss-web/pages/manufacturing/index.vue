<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const stats = ref([
  { label: 'Active Work Orders', value: '12', icon: 'i-heroicons-clipboard-document-list', color: 'primary' },
  { label: 'Pending Materials', value: '5', icon: 'i-heroicons-cube', color: 'warning' },
  { label: 'Completed Today', value: '8', icon: 'i-heroicons-check-circle', color: 'success' },
  { label: 'In Production', value: '4', icon: 'i-heroicons-cog-6-tooth', color: 'info' }
])

const activeOrders = ref([
  { id: 'WO-001', product: 'Fresh Bread', quantity: 150, status: 'in-progress', progress: 60 },
  { id: 'WO-002', product: 'Maize Meal', quantity: 100, status: 'pending', progress: 0 },
  { id: 'WO-003', product: 'Cheese (1kg)', quantity: 50, status: 'in-progress', progress: 85 }
])
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6">
      <h1 class="text-2xl font-bold mb-2">Manufacturing</h1>
      <p class="text-muted-foreground">Manage production processes and work orders</p>
    </div>

    <!-- Stats Grid -->
    <div class="grid grid-cols-2 md:grid-cols-4 gap-4 mb-6">
      <UCard v-for="stat in stats" :key="stat.label" class="hover:shadow-md transition-shadow">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-muted-foreground">{{ stat.label }}</p>
            <p class="text-2xl font-bold mt-1">{{ stat.value }}</p>
          </div>
          <div :class="`p-3 rounded-lg bg-${stat.color}/10`">
            <UIcon :name="stat.icon" class="w-6 h-6" :class="`text-${stat.color}`" />
          </div>
        </div>
      </UCard>
    </div>

    <!-- Quick Actions -->
    <div class="mb-6 grid grid-cols-1 md:grid-cols-3 gap-4">
      <UButton to="/manufacturing/work-orders" size="lg" block>
        <UIcon name="i-heroicons-clipboard-document-list" class="mr-2" />
        Work Orders
      </UButton>
      <UButton to="/manufacturing/bom" size="lg" block variant="outline">
        <UIcon name="i-heroicons-queue-list" class="mr-2" />
        Bills of Materials
      </UButton>
      <UButton to="/manufacturing/production-plan" size="lg" block variant="outline">
        <UIcon name="i-heroicons-calendar" class="mr-2" />
        Production Planning
      </UButton>
    </div>

    <!-- Active Work Orders -->
    <UCard class="mt-6">
      <template #header>
        <div class="flex items-center justify-between">
          <h2 class="text-lg font-semibold">Active Work Orders</h2>
          <UButton to="/manufacturing/work-orders" size="sm" variant="ghost">View All</UButton>
        </div>
      </template>

      <div class="space-y-4">
        <div v-for="order in activeOrders" :key="order.id" class="p-4 border rounded-lg hover:bg-accent/50 transition-colors">
          <div class="flex items-center justify-between mb-2">
            <div>
              <h3 class="font-semibold">{{ order.product }}</h3>
              <p class="text-sm text-muted-foreground">{{ order.id }} • Qty: {{ order.quantity }}</p>
            </div>
            <UBadge :color="order.status === 'in-progress' ? 'primary' : 'gray'">
              {{ order.status }}
            </UBadge>
          </div>
          <div class="w-full bg-gray-200 rounded-full h-2">
            <div :class="`h-2 rounded-full ${order.progress > 0 ? 'bg-primary' : 'bg-gray-300'}`" :style="{ width: `${order.progress}%` }" />
          </div>
          <p class="text-xs text-muted-foreground mt-1">{{ order.progress }}% complete</p>
        </div>
      </div>
    </UCard>
  </div>
</template>
