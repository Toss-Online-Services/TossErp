<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const procedures = ref([
  { id: 'QP-001', name: 'Fresh Cheese Quality Check', category: 'Production', version: '1.2', status: 'active', lastUpdated: '2025-11-15' },
  { id: 'QP-002', name: 'Incoming Raw Material Inspection', category: 'Receipt', version: '2.0', status: 'active', lastUpdated: '2025-10-20' },
  { id: 'QP-003', name: 'Bakery Product Standards', category: 'Production', version: '1.5', status: 'active', lastUpdated: '2025-12-01' },
  { id: 'QP-004', name: 'Equipment Calibration', category: 'Maintenance', version: '1.0', status: 'draft', lastUpdated: '2025-12-15' }
])

const columns = [
  { key: 'id', label: 'ID' },
  { key: 'name', label: 'Procedure Name' },
  { key: 'category', label: 'Category' },
  { key: 'version', label: 'Version' },
  { key: 'status', label: 'Status' },
  { key: 'lastUpdated', label: 'Last Updated' },
  { key: 'actions', label: 'Actions' }
]
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6 flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold mb-2">Quality Procedures</h1>
        <p class="text-muted-foreground">Manage quality control procedures and standards</p>
      </div>
      <UButton>
        <UIcon name="i-heroicons-plus" class="mr-2" />
        New Procedure
      </UButton>
    </div>

    <!-- Procedures Table -->
    <UCard>
      <UTable :rows="procedures" :columns="columns">
        <template #status-data="{ row }">
          <UBadge :color="row.status === 'active' ? 'success' : 'gray'">
            {{ row.status }}
          </UBadge>
        </template>

        <template #lastUpdated-data="{ row }">
          {{ new Date(row.lastUpdated).toLocaleDateString() }}
        </template>

        <template #actions-data="{ row }">
          <div class="flex gap-2">
            <UButton size="xs" variant="ghost">View</UButton>
            <UButton size="xs" variant="ghost">Edit</UButton>
          </div>
        </template>
      </UTable>
    </UCard>
  </div>
</template>
