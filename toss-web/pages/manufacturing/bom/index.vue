<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const boms = ref([
  {
    id: 'BOM-001',
    product: 'Fresh Bread (1 loaf)',
    materials: 5,
    cost: 8.50,
    status: 'active'
  },
  {
    id: 'BOM-002',
    product: 'Cheese (1kg)',
    materials: 4,
    cost: 45.00,
    status: 'active'
  },
  {
    id: 'BOM-003',
    product: 'Maize Meal (5kg)',
    materials: 3,
    cost: 25.00,
    status: 'draft'
  }
])

const columns = [
  { key: 'id', label: 'BOM ID' },
  { key: 'product', label: 'Product' },
  { key: 'materials', label: 'Materials' },
  { key: 'cost', label: 'Cost (R)' },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions' }
]
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6 flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold mb-2">Bills of Materials</h1>
        <p class="text-muted-foreground">Manage product recipes and material requirements</p>
      </div>
      <UButton>
        <UIcon name="i-heroicons-plus" class="mr-2" />
        New BOM
      </UButton>
    </div>

    <!-- BOMs Table -->
    <UCard>
      <UTable :rows="boms" :columns="columns">
        <template #materials-data="{ row }">
          <span class="text-sm">{{ row.materials }} items</span>
        </template>

        <template #cost-data="{ row }">
          <span class="font-medium">R{{ row.cost.toFixed(2) }}</span>
        </template>

        <template #status-data="{ row }">
          <UBadge :color="row.status === 'active' ? 'success' : 'gray'">
            {{ row.status }}
          </UBadge>
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
