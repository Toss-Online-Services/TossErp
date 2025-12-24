<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const searchQuery = ref('')
const selectedCategory = ref('all')

const categories = ['all', 'groceries', 'beverages', 'fresh-produce', 'bakery', 'meat-poultry', 'dairy', 'snacks']

const items = ref([
  {
    id: 'ITM-001',
    name: 'White Bread 700g',
    category: 'bakery',
    sku: 'BRD-WHT-700',
    stockQty: 45,
    reorderLevel: 20,
    unit: 'loaf',
    buyingPrice: 8.50,
    sellingPrice: 12.00,
    status: 'in-stock'
  },
  {
    id: 'ITM-002',
    name: 'Maize Meal 5kg',
    category: 'groceries',
    sku: 'GRO-MAZ-5KG',
    stockQty: 12,
    reorderLevel: 15,
    unit: 'bag',
    buyingPrice: 45.00,
    sellingPrice: 55.00,
    status: 'low-stock'
  },
  {
    id: 'ITM-003',
    name: 'Fresh Milk 2L',
    category: 'dairy',
    sku: 'DRY-MLK-2L',
    stockQty: 28,
    reorderLevel: 25,
    unit: 'bottle',
    buyingPrice: 22.00,
    sellingPrice: 28.00,
    status: 'in-stock'
  },
  {
    id: 'ITM-004',
    name: 'Cooking Oil 750ml',
    category: 'groceries',
    sku: 'GRO-OIL-750',
    stockQty: 3,
    reorderLevel: 10,
    unit: 'bottle',
    buyingPrice: 18.50,
    sellingPrice: 24.00,
    status: 'critical'
  },
  {
    id: 'ITM-005',
    name: 'Chicken Pieces 1kg',
    category: 'meat-poultry',
    sku: 'MET-CHK-1KG',
    stockQty: 15,
    reorderLevel: 10,
    unit: 'kg',
    buyingPrice: 35.00,
    sellingPrice: 48.00,
    status: 'in-stock'
  },
  {
    id: 'ITM-006',
    name: 'Tomatoes 1kg',
    category: 'fresh-produce',
    sku: 'PRD-TOM-1KG',
    stockQty: 8,
    reorderLevel: 12,
    unit: 'kg',
    buyingPrice: 12.00,
    sellingPrice: 18.00,
    status: 'low-stock'
  }
])

const columns = [
  { key: 'sku', label: 'SKU' },
  { key: 'name', label: 'Item Name' },
  { key: 'category', label: 'Category' },
  { key: 'stockQty', label: 'Stock' },
  { key: 'unit', label: 'Unit' },
  { key: 'buyingPrice', label: 'Buy Price' },
  { key: 'sellingPrice', label: 'Sell Price' },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions' }
]

const filteredItems = computed(() => {
  let result = items.value

  if (selectedCategory.value !== 'all') {
    result = result.filter(item => item.category === selectedCategory.value)
  }

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(item =>
      item.name.toLowerCase().includes(query) ||
      item.sku.toLowerCase().includes(query)
    )
  }

  return result
})

const statusColor = (status: string) => {
  switch (status) {
    case 'in-stock': return 'success'
    case 'low-stock': return 'warning'
    case 'critical': return 'error'
    default: return 'gray'
  }
}

const getMargin = (buying: number, selling: number) => {
  return ((selling - buying) / buying * 100).toFixed(1)
}
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6 flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold mb-2">Stock Items</h1>
        <p class="text-muted-foreground">Manage your inventory items and stock levels</p>
      </div>
      <UButton>
        <UIcon name="i-heroicons-plus" class="mr-2" />
        Add New Item
      </UButton>
    </div>

    <!-- Filters -->
    <div class="mb-4 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      <div>
        <UInput
          v-model="searchQuery"
          placeholder="Search items by name or SKU..."
          icon="i-heroicons-magnifying-glass"
        />
      </div>
      <div>
        <USelect
          v-model="selectedCategory"
          :options="categories.map(c => ({ value: c, label: c.replace(/-/g, ' ').replace(/\b\w/g, l => l.toUpperCase()) }))"
          placeholder="Select category"
        />
      </div>
      <div class="flex gap-2">
        <UButton size="sm" variant="outline">
          <UIcon name="i-heroicons-funnel" class="mr-1" />
          More Filters
        </UButton>
        <UButton size="sm" variant="ghost">
          <UIcon name="i-heroicons-arrow-down-tray" class="mr-1" />
          Export
        </UButton>
      </div>
    </div>

    <!-- Quick Stats -->
    <div class="mb-6 grid grid-cols-2 md:grid-cols-4 gap-4">
      <UCard>
        <div class="text-center">
          <p class="text-sm text-muted-foreground mb-1">Total Items</p>
          <p class="text-2xl font-bold">{{ items.length }}</p>
        </div>
      </UCard>
      <UCard>
        <div class="text-center">
          <p class="text-sm text-muted-foreground mb-1">Low Stock</p>
          <p class="text-2xl font-bold text-yellow-600">{{ items.filter(i => i.status === 'low-stock').length }}</p>
        </div>
      </UCard>
      <UCard>
        <div class="text-center">
          <p class="text-sm text-muted-foreground mb-1">Critical</p>
          <p class="text-2xl font-bold text-red-600">{{ items.filter(i => i.status === 'critical').length }}</p>
        </div>
      </UCard>
      <UCard>
        <div class="text-center">
          <p class="text-sm text-muted-foreground mb-1">In Stock</p>
          <p class="text-2xl font-bold text-green-600">{{ items.filter(i => i.status === 'in-stock').length }}</p>
        </div>
      </UCard>
    </div>

    <!-- Items Table -->
    <UCard>
      <UTable :rows="filteredItems" :columns="columns">
        <template #name-data="{ row }">
          <div>
            <p class="font-medium">{{ row.name }}</p>
            <p class="text-xs text-muted-foreground">{{ row.sku }}</p>
          </div>
        </template>

        <template #category-data="{ row }">
          <span class="text-sm capitalize">{{ row.category.replace(/-/g, ' ') }}</span>
        </template>

        <template #stockQty-data="{ row }">
          <div>
            <p :class="row.stockQty <= row.reorderLevel ? 'text-red-600 font-bold' : ''">
              {{ row.stockQty }}
            </p>
            <p class="text-xs text-muted-foreground">Min: {{ row.reorderLevel }}</p>
          </div>
        </template>

        <template #buyingPrice-data="{ row }">
          <span class="text-sm">R{{ row.buyingPrice.toFixed(2) }}</span>
        </template>

        <template #sellingPrice-data="{ row }">
          <div>
            <p class="text-sm font-medium">R{{ row.sellingPrice.toFixed(2) }}</p>
            <p class="text-xs text-green-600">+{{ getMargin(row.buyingPrice, row.sellingPrice) }}%</p>
          </div>
        </template>

        <template #status-data="{ row }">
          <UBadge :color="statusColor(row.status)" size="xs">
            {{ row.status.replace(/-/g, ' ') }}
          </UBadge>
        </template>

        <template #actions-data="{ row }">
          <div class="flex gap-2">
            <UButton size="xs" variant="ghost" icon="i-heroicons-pencil" />
            <UButton size="xs" variant="ghost" icon="i-heroicons-arrow-path" title="Adjust Stock" />
            <UButton size="xs" variant="ghost" icon="i-heroicons-eye" />
          </div>
        </template>
      </UTable>

      <!-- Empty State -->
      <div v-if="filteredItems.length === 0" class="text-center py-12">
        <UIcon name="i-heroicons-inbox" class="w-12 h-12 text-muted-foreground mx-auto mb-3" />
        <p class="text-muted-foreground">No items found</p>
        <p class="text-sm text-muted-foreground mt-1">Try adjusting your search or filters</p>
      </div>
    </UCard>
  </div>
</template>
