<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const maintenanceSchedule = ref([
  { id: 'MNT-001', asset: 'Delivery Van', type: 'Oil Change', dueDate: '2025-12-28', status: 'upcoming', lastDone: '2025-09-28' },
  { id: 'MNT-002', asset: 'Industrial Oven', type: 'Deep Cleaning', dueDate: '2026-01-15', status: 'upcoming', lastDone: '2025-10-15' },
  { id: 'MNT-003', asset: 'Sewing Machine', type: 'Motor Repair', dueDate: '2025-12-23', status: 'overdue', lastDone: '-' }
])

const columns = [
  { key: 'id', label: 'ID' },
  { key: 'asset', label: 'Asset' },
  { key: 'type', label: 'Maintenance Type' },
  { key: 'dueDate', label: 'Due Date' },
  { key: 'lastDone', label: 'Last Done' },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions' }
]

const statusColors = {
  upcoming: 'info',
  overdue: 'error',
  completed: 'success'
}
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6 flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold mb-2">Asset Maintenance</h1>
        <p class="text-muted-foreground">Schedule and track asset maintenance</p>
      </div>
      <UButton>
        <UIcon name="i-heroicons-plus" class="mr-2" />
        Schedule Maintenance
      </UButton>
    </div>

    <!-- Filters -->
    <div class="mb-4 flex gap-2">
      <UButton size="sm" variant="outline">All</UButton>
      <UButton size="sm" variant="ghost">Upcoming</UButton>
      <UButton size="sm" variant="ghost">Overdue</UButton>
      <UButton size="sm" variant="ghost">Completed</UButton>
    </div>

    <!-- Maintenance Table -->
    <UCard>
      <UTable :rows="maintenanceSchedule" :columns="columns">
        <template #dueDate-data="{ row }">
          <span :class="{
            'text-error font-medium': row.status === 'overdue',
            'text-warning font-medium': new Date(row.dueDate).getTime() - new Date().getTime() < 7 * 24 * 60 * 60 * 1000
          }">
            {{ new Date(row.dueDate).toLocaleDateString() }}
          </span>
        </template>

        <template #status-data="{ row }">
          <UBadge :color="statusColors[row.status]">
            {{ row.status }}
          </UBadge>
        </template>

        <template #actions-data="{ row }">
          <div class="flex gap-2">
            <UButton size="xs" variant="ghost">View</UButton>
            <UButton v-if="row.status !== 'completed'" size="xs" variant="ghost">Complete</UButton>
          </div>
        </template>
      </UTable>
    </UCard>
  </div>
</template>
