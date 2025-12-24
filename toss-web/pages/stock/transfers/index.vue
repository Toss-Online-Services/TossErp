<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const transfers = ref([
  { id: 'TRF-001', fromStore: 'Main Shop', toStore: 'Spaza Corner', date: '2025-12-23', items: 5, status: 'in-transit' },
  { id: 'TRF-002', fromStore: 'Spaza Corner', toStore: 'Main Shop', date: '2025-12-22', items: 3, status: 'completed' },
  { id: 'TRF-003', fromStore: 'Main Shop', toStore: 'Market Stall', date: '2025-12-21', items: 8, status: 'pending' }
])

const columns = [
  { key: 'id', label: 'Transfer #' },
  { key: 'fromStore', label: 'From' },
  { key: 'toStore', label: 'To' },
  { key: 'date', label: 'Date' },
  { key: 'items', label: 'Items' },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions' }
]

const statusColors = {
  pending: 'gray',
  'in-transit': 'info',
  completed: 'success',
  cancelled: 'error'
}
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6 flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold mb-2">Stock Transfers</h1>
        <p class="text-muted-foreground">Move stock between stores or locations</p>
      </div>
      <UButton>
        <UIcon name="i-heroicons-plus" class="mr-2" />
        New Transfer
      </UButton>
    </div>

    <!-- Filters -->
    <div class="mb-4 flex gap-2">
      <UButton size="sm" variant="outline">All</UButton>
      <UButton size="sm" variant="ghost">Pending</UButton>
      <UButton size="sm" variant="ghost">In Transit</UButton>
      <UButton size="sm" variant="ghost">Completed</UButton>
    </div>

    <!-- Transfers Table -->
    <UCard>
      <UTable :rows="transfers" :columns="columns">
        <template #date-data="{ row }">
          {{ new Date(row.date).toLocaleDateString() }}
        </template>

        <template #status-data="{ row }">
          <UBadge :color="statusColors[row.status]">
            {{ row.status }}
          </UBadge>
        </template>

        <template #actions-data="{ row }">
          <div class="flex gap-2">
            <UButton size="xs" variant="ghost">View</UButton>
            <UButton v-if="row.status === 'in-transit'" size="xs" variant="ghost">Receive</UButton>
          </div>
        </template>
      </UTable>
    </UCard>
  </div>
</template>
