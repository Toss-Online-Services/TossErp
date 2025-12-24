<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const assets = ref([
  { id: 'AST-001', name: 'Delivery Van', category: 'Vehicle', purchaseDate: '2023-01-15', value: 85000, depreciation: 17000, status: 'active' },
  { id: 'AST-002', name: 'Industrial Oven', category: 'Equipment', purchaseDate: '2022-06-20', value: 25000, depreciation: 7500, status: 'active' },
  { id: 'AST-003', name: 'Sewing Machine', category: 'Equipment', purchaseDate: '2024-03-10', value: 8500, depreciation: 850, status: 'maintenance' },
  { id: 'AST-004', name: 'Refrigerator', category: 'Equipment', purchaseDate: '2021-09-05', value: 12000, depreciation: 6000, status: 'active' }
])

const columns = [
  { key: 'id', label: 'Asset ID' },
  { key: 'name', label: 'Name' },
  { key: 'category', label: 'Category' },
  { key: 'purchaseDate', label: 'Purchase Date' },
  { key: 'value', label: 'Current Value' },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions' }
]
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6 flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold mb-2">Assets List</h1>
        <p class="text-muted-foreground">View and manage all business assets</p>
      </div>
      <UButton>
        <UIcon name="i-heroicons-plus" class="mr-2" />
        Add Asset
      </UButton>
    </div>

    <!-- Filters -->
    <div class="mb-4 flex gap-2">
      <UButton size="sm" variant="outline">All</UButton>
      <UButton size="sm" variant="ghost">Vehicle</UButton>
      <UButton size="sm" variant="ghost">Equipment</UButton>
      <UButton size="sm" variant="ghost">Furniture</UButton>
    </div>

    <!-- Assets Table -->
    <UCard>
      <UTable :rows="assets" :columns="columns">
        <template #purchaseDate-data="{ row }">
          {{ new Date(row.purchaseDate).toLocaleDateString() }}
        </template>

        <template #value-data="{ row }">
          <div>
            <p class="font-bold">R{{ row.value.toLocaleString() }}</p>
            <p class="text-xs text-muted-foreground">-R{{ row.depreciation.toLocaleString() }} depreciation</p>
          </div>
        </template>

        <template #status-data="{ row }">
          <UBadge :color="row.status === 'active' ? 'success' : row.status === 'maintenance' ? 'warning' : 'gray'">
            {{ row.status }}
          </UBadge>
        </template>

        <template #actions-data="{ row }">
          <div class="flex gap-2">
            <UButton size="xs" variant="ghost">View</UButton>
            <UButton size="xs" variant="ghost">Maintain</UButton>
          </div>
        </template>
      </UTable>
    </UCard>
  </div>
</template>
