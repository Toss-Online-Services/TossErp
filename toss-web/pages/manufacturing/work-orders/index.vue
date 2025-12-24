<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const workOrders = ref([
  {
    id: 'WO-001',
    product: 'Fresh Bread',
    quantity: 150,
    plannedDate: '2025-12-24',
    status: 'in-progress',
    progress: 60
  },
  {
    id: 'WO-002',
    product: 'Maize Meal',
    quantity: 100,
    plannedDate: '2025-12-25',
    status: 'pending',
    progress: 0
  },
  {
    id: 'WO-003',
    product: 'Cheese (1kg)',
    quantity: 50,
    plannedDate: '2025-12-23',
    status: 'completed',
    progress: 100
  }
])

const columns = [
  { key: 'id', label: 'Work Order' },
  { key: 'product', label: 'Product' },
  { key: 'quantity', label: 'Quantity' },
  { key: 'plannedDate', label: 'Planned Date' },
  { key: 'progress', label: 'Progress' },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions' }
]

const statusColors = {
  'pending': 'gray',
  'in-progress': 'primary',
  'completed': 'success',
  'cancelled': 'error'
}
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6 flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold mb-2">Work Orders</h1>
        <p class="text-muted-foreground">Manage production work orders and track progress</p>
      </div>
      <UButton>
        <UIcon name="i-heroicons-plus" class="mr-2" />
        New Work Order
      </UButton>
    </div>

    <!-- Filters -->
    <div class="mb-4 flex gap-2">
      <UButton size="sm" variant="outline">All</UButton>
      <UButton size="sm" variant="ghost">Pending</UButton>
      <UButton size="sm" variant="ghost">In Progress</UButton>
      <UButton size="sm" variant="ghost">Completed</UButton>
    </div>

    <!-- Work Orders Table -->
    <UCard>
      <UTable :rows="workOrders" :columns="columns">
        <template #quantity-data="{ row }">
          <span class="font-medium">{{ row.quantity }} units</span>
        </template>

        <template #progress-data="{ row }">
          <div class="flex items-center gap-2">
            <div class="w-24 bg-gray-200 rounded-full h-2">
              <div :class="`h-2 rounded-full ${row.progress > 0 ? 'bg-primary' : 'bg-gray-300'}`" :style="{ width: `${row.progress}%` }" />
            </div>
            <span class="text-xs text-muted-foreground">{{ row.progress }}%</span>
          </div>
        </template>

        <template #status-data="{ row }">
          <UBadge :color="statusColors[row.status]">
            {{ row.status }}
          </UBadge>
        </template>

        <template #actions-data="{ row }">
          <div class="flex gap-2">
            <UButton :to="`/manufacturing/work-orders/${row.id}`" size="xs" variant="ghost">View</UButton>
            <UButton v-if="row.status === 'pending'" size="xs" variant="ghost">Start</UButton>
            <UButton v-if="row.status === 'in-progress'" size="xs" variant="ghost">Complete</UButton>
          </div>
        </template>
      </UTable>
    </UCard>
  </div>
</template>
