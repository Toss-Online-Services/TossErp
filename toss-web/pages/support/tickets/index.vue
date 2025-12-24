<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const tickets = ref([
  { id: 'TKT-001', title: 'Payment system not working', customer: 'John Doe', priority: 'high', status: 'open', created: '2025-12-23' },
  { id: 'TKT-002', title: 'Need help with order', customer: 'Jane Smith', priority: 'medium', status: 'in-progress', created: '2025-12-23' },
  { id: 'TKT-003', title: 'Product inquiry', customer: 'Mike Johnson', priority: 'low', status: 'resolved', created: '2025-12-22' },
  { id: 'TKT-004', title: 'Delivery issue', customer: 'Sarah Williams', priority: 'high', status: 'open', created: '2025-12-23' }
])

const columns = [
  { key: 'id', label: 'Ticket ID' },
  { key: 'title', label: 'Title' },
  { key: 'customer', label: 'Customer' },
  { key: 'priority', label: 'Priority' },
  { key: 'status', label: 'Status' },
  { key: 'created', label: 'Created' },
  { key: 'actions', label: 'Actions' }
]

const priorityColors = {
  high: 'error',
  medium: 'warning',
  low: 'gray'
}

const statusColors = {
  open: 'warning',
  'in-progress': 'primary',
  resolved: 'success',
  closed: 'gray'
}
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6 flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold mb-2">Support Tickets</h1>
        <p class="text-muted-foreground">View and manage all support tickets</p>
      </div>
      <UButton>
        <UIcon name="i-heroicons-plus" class="mr-2" />
        New Ticket
      </UButton>
    </div>

    <!-- Filters -->
    <div class="mb-4 flex gap-2">
      <UButton size="sm" variant="outline">All</UButton>
      <UButton size="sm" variant="ghost">Open</UButton>
      <UButton size="sm" variant="ghost">In Progress</UButton>
      <UButton size="sm" variant="ghost">Resolved</UButton>
    </div>

    <!-- Tickets Table -->
    <UCard>
      <UTable :rows="tickets" :columns="columns">
        <template #priority-data="{ row }">
          <UBadge :color="priorityColors[row.priority]">
            {{ row.priority }}
          </UBadge>
        </template>

        <template #status-data="{ row }">
          <UBadge :color="statusColors[row.status]">
            {{ row.status }}
          </UBadge>
        </template>

        <template #created-data="{ row }">
          {{ new Date(row.created).toLocaleDateString() }}
        </template>

        <template #actions-data="{ row }">
          <div class="flex gap-2">
            <UButton :to="`/support/tickets/${row.id}`" size="xs" variant="ghost">View</UButton>
            <UButton v-if="row.status !== 'resolved'" size="xs" variant="ghost">Resolve</UButton>
          </div>
        </template>
      </UTable>
    </UCard>
  </div>
</template>
