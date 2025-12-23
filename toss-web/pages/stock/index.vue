<script setup lang="ts">
import { ref, computed } from 'vue'
import { Package, AlertTriangle } from 'lucide-vue-next'
import AppCard from '~/components/common/AppCard.vue'
import AppTable from '~/components/common/AppTable.vue'
import AppSectionHeader from '~/components/common/AppSectionHeader.vue'
import AppKpiCard from '~/components/common/AppKpiCard.vue'

useHead({
  title: 'Stock - TOSS Admin',
  meta: [{ name: 'description', content: 'Stock management for TOSS Admin' }]
})

definePageMeta({
  layout: 'dashboard'
})

const items = ref([
  { id: 1, name: 'White Bread', category: 'Bakery', quantity: 45, reorderLevel: 30, status: 'In Stock' },
  { id: 2, name: 'Fresh Milk 1L', category: 'Dairy', quantity: 8, reorderLevel: 20, status: 'Low Stock' },
  { id: 3, name: 'Sugar 2.5kg', category: 'Groceries', quantity: 5, reorderLevel: 15, status: 'Low Stock' },
  { id: 4, name: 'Cooking Oil 750ml', category: 'Groceries', quantity: 15, reorderLevel: 25, status: 'In Stock' },
  { id: 5, name: 'Maize Meal 10kg', category: 'Groceries', quantity: 6, reorderLevel: 20, status: 'Low Stock' }
])

const lowStockItems = computed(() => items.value.filter(item => item.quantity <= item.reorderLevel))

const stockMovements = ref([
  { id: 1, item: 'White Bread', type: 'Stock In', quantity: 50, date: '2024-01-15', reference: 'PO-001' },
  { id: 2, item: 'Fresh Milk 1L', type: 'Stock Out', quantity: -12, date: '2024-01-15', reference: 'SALE-123' },
  { id: 3, item: 'Sugar 2.5kg', type: 'Stock In', quantity: 20, date: '2024-01-14', reference: 'PO-002' }
])
</script>

<template>
  <div class="space-y-6">
    <AppSectionHeader
      title="Stock Management"
      description="Manage inventory items and stock levels"
    />

    <!-- Low Stock Widget -->
    <AppCard title="Low Stock Alert">
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div
          v-for="item in lowStockItems.slice(0, 3)"
          :key="item.id"
          class="p-4 border border-border rounded-lg hover:bg-muted transition-colors"
        >
          <p class="font-medium text-foreground">{{ item.name }}</p>
          <p class="text-sm text-muted-foreground mt-1">Only {{ item.quantity }} units left</p>
          <p class="text-xs text-muted-foreground mt-1">Reorder at {{ item.reorderLevel }} units</p>
        </div>
      </div>
    </AppCard>

    <!-- Items Table -->
    <AppCard title="Stock Items">
      <div class="mb-4 flex items-center justify-between">
        <input
          type="text"
          placeholder="Search items..."
          class="px-3 py-2 border border-border rounded-lg text-sm w-64"
        />
        <select class="px-3 py-2 border border-border rounded-lg text-sm">
          <option>All Categories</option>
          <option>Bakery</option>
          <option>Dairy</option>
          <option>Groceries</option>
        </select>
      </div>
      <AppTable :headers="['Item Name', 'Category', 'Quantity', 'Reorder Level', 'Status']">
        <tr
          v-for="item in items"
          :key="item.id"
          class="hover:bg-muted/50 transition-colors"
        >
          <td class="px-4 py-3 text-sm font-medium text-foreground">{{ item.name }}</td>
          <td class="px-4 py-3 text-sm text-muted-foreground">{{ item.category }}</td>
          <td class="px-4 py-3 text-sm text-foreground">{{ item.quantity }}</td>
          <td class="px-4 py-3 text-sm text-muted-foreground">{{ item.reorderLevel }}</td>
          <td class="px-4 py-3">
            <span
              :class="[
                'inline-flex items-center px-2 py-1 rounded text-xs font-medium',
                item.status === 'In Stock' ? 'bg-green-100 dark:bg-green-900/20 text-green-700 dark:text-green-400' : 'bg-orange-100 dark:bg-orange-900/20 text-orange-700 dark:text-orange-400'
              ]"
            >
              {{ item.status }}
            </span>
          </td>
        </tr>
      </AppTable>
    </AppCard>

    <!-- Stock Movements -->
    <AppCard title="Recent Stock Movements">
      <AppTable :headers="['Item', 'Type', 'Quantity', 'Date', 'Reference']">
        <tr
          v-for="movement in stockMovements"
          :key="movement.id"
          class="hover:bg-muted/50 transition-colors"
        >
          <td class="px-4 py-3 text-sm font-medium text-foreground">{{ movement.item }}</td>
          <td class="px-4 py-3">
            <span
              :class="[
                'inline-flex items-center px-2 py-1 rounded text-xs font-medium',
                movement.type === 'Stock In' ? 'bg-green-100 dark:bg-green-900/20 text-green-700 dark:text-green-400' : 'bg-red-100 dark:bg-red-900/20 text-red-700 dark:text-red-400'
              ]"
            >
              {{ movement.type }}
            </span>
          </td>
          <td class="px-4 py-3 text-sm font-medium text-foreground">{{ movement.quantity > 0 ? '+' : '' }}{{ movement.quantity }}</td>
          <td class="px-4 py-3 text-sm text-muted-foreground">{{ movement.date }}</td>
          <td class="px-4 py-3 text-sm text-muted-foreground">{{ movement.reference }}</td>
        </tr>
      </AppTable>
    </AppCard>
  </div>
</template>
