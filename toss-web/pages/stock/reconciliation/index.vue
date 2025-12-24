<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const reconciliations = ref([
  { id: 'REC-001', date: '2025-12-23', location: 'Main Shop', items: 45, discrepancies: 2, status: 'pending' },
  { id: 'REC-002', date: '2025-12-20', location: 'Main Shop', items: 45, discrepancies: 0, status: 'approved' },
  { id: 'REC-003', date: '2025-12-15', location: 'Spaza Corner', items: 32, discrepancies: 1, status: 'approved' }
])

const columns = [
  { key: 'id', label: 'Reconciliation #' },
  { key: 'date', label: 'Date' },
  { key: 'location', label: 'Location' },
  { key: 'items', label: 'Items Counted' },
  { key: 'discrepancies', label: 'Discrepancies' },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions' }
]
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6 flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold mb-2">Stock Reconciliation</h1>
        <p class="text-muted-foreground">Physical count vs system records</p>
      </div>
      <UButton>
        <UIcon name="i-heroicons-plus" class="mr-2" />
        Start Count
      </UButton>
    </div>

    <!-- Reconciliations Table -->
    <UCard>
      <UTable :rows="reconciliations" :columns="columns">
        <template #date-data="{ row }">
          {{ new Date(row.date).toLocaleDateString() }}
        </template>

        <template #discrepancies-data="{ row }">
          <span :class="row.discrepancies > 0 ? 'text-warning font-bold' : 'text-success'">
            {{ row.discrepancies }}
          </span>
        </template>

        <template #status-data="{ row }">
          <UBadge :color="row.status === 'approved' ? 'success' : 'warning'">
            {{ row.status }}
          </UBadge>
        </template>

        <template #actions-data="{ row }">
          <div class="flex gap-2">
            <UButton size="xs" variant="ghost">View</UButton>
            <UButton v-if="row.status === 'pending'" size="xs" variant="ghost">Approve</UButton>
          </div>
        </template>
      </UTable>
    </UCard>
  </div>
</template>
