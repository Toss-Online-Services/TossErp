<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const inspections = ref([
  { id: 'QI-001', item: 'Fresh Cheese Batch #45', type: 'Production', result: 'pass', inspector: 'Mary S.', date: '2025-12-23', notes: 'All parameters within acceptable range' },
  { id: 'QI-002', item: 'Bread Batch #120', type: 'Production', result: 'fail', inspector: 'John D.', date: '2025-12-22', notes: 'Moisture content too high' },
  { id: 'QI-003', item: 'Maize Meal #88', type: 'Production', result: 'pass', inspector: 'Mary S.', date: '2025-12-22', notes: 'Quality standards met' },
  { id: 'QI-004', item: 'Incoming Flour Delivery', type: 'Receipt', result: 'pass', inspector: 'John D.', date: '2025-12-21', notes: 'Supplier quality verified' }
])

const columns = [
  { key: 'id', label: 'ID' },
  { key: 'item', label: 'Item/Batch' },
  { key: 'type', label: 'Type' },
  { key: 'result', label: 'Result' },
  { key: 'inspector', label: 'Inspector' },
  { key: 'date', label: 'Date' },
  { key: 'actions', label: 'Actions' }
]
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6 flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold mb-2">Quality Inspections</h1>
        <p class="text-muted-foreground">View and manage quality inspection records</p>
      </div>
      <UButton>
        <UIcon name="i-heroicons-plus" class="mr-2" />
        New Inspection
      </UButton>
    </div>

    <!-- Filters -->
    <div class="mb-4 flex gap-2">
      <UButton size="sm" variant="outline">All</UButton>
      <UButton size="sm" variant="ghost">Pass</UButton>
      <UButton size="sm" variant="ghost">Fail</UButton>
      <UButton size="sm" variant="ghost">Production</UButton>
      <UButton size="sm" variant="ghost">Receipt</UButton>
    </div>

    <!-- Inspections Table -->
    <UCard>
      <UTable :rows="inspections" :columns="columns">
        <template #result-data="{ row }">
          <UBadge :color="row.result === 'pass' ? 'success' : 'error'">
            {{ row.result.toUpperCase() }}
          </UBadge>
        </template>

        <template #date-data="{ row }">
          {{ new Date(row.date).toLocaleDateString() }}
        </template>

        <template #actions-data="{ row }">
          <div class="flex gap-2">
            <UButton size="xs" variant="ghost">View Details</UButton>
            <UButton size="xs" variant="ghost">Print Report</UButton>
          </div>
        </template>
      </UTable>
    </UCard>
  </div>
</template>
