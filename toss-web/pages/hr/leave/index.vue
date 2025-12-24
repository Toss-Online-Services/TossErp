<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const leaveRequests = ref([
  { id: 'LV-001', employee: 'Mary Sithole', type: 'Sick Leave', from: '2025-12-23', to: '2025-12-25', days: 3, status: 'pending' },
  { id: 'LV-002', employee: 'Thabo Mokoena', type: 'Annual Leave', from: '2025-12-27', to: '2026-01-03', days: 7, status: 'approved' },
  { id: 'LV-003', employee: 'Sipho Khumalo', type: 'Family Responsibility', from: '2025-12-24', to: '2025-12-24', days: 1, status: 'rejected' }
])

const columns = [
  { key: 'id', label: 'Request ID' },
  { key: 'employee', label: 'Employee' },
  { key: 'type', label: 'Leave Type' },
  { key: 'from', label: 'From' },
  { key: 'to', label: 'To' },
  { key: 'days', label: 'Days' },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions' }
]

const statusColors = {
  pending: 'warning',
  approved: 'success',
  rejected: 'error'
}
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6 flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold mb-2">Leave Requests</h1>
        <p class="text-muted-foreground">Manage employee leave applications</p>
      </div>
      <UButton>
        <UIcon name="i-heroicons-plus" class="mr-2" />
        Apply for Leave
      </UButton>
    </div>

    <!-- Filters -->
    <div class="mb-4 flex gap-2">
      <UButton size="sm" variant="outline">All Requests</UButton>
      <UButton size="sm" variant="ghost">Pending</UButton>
      <UButton size="sm" variant="ghost">Approved</UButton>
      <UButton size="sm" variant="ghost">Rejected</UButton>
    </div>

    <!-- Leave Requests Table -->
    <UCard>
      <UTable :rows="leaveRequests" :columns="columns">
        <template #from-data="{ row }">
          {{ new Date(row.from).toLocaleDateString() }}
        </template>

        <template #to-data="{ row }">
          {{ new Date(row.to).toLocaleDateString() }}
        </template>

        <template #days-data="{ row }">
          <span class="font-medium">{{ row.days }} day{{ row.days > 1 ? 's' : '' }}</span>
        </template>

        <template #status-data="{ row }">
          <UBadge :color="statusColors[row.status]">
            {{ row.status }}
          </UBadge>
        </template>

        <template #actions-data="{ row }">
          <div class="flex gap-2">
            <UButton v-if="row.status === 'pending'" size="xs" variant="ghost" color="green">Approve</UButton>
            <UButton v-if="row.status === 'pending'" size="xs" variant="ghost" color="red">Reject</UButton>
            <UButton size="xs" variant="ghost">View</UButton>
          </div>
        </template>
      </UTable>
    </UCard>
  </div>
</template>
