<script setup lang="ts">
import { ref } from 'vue'
import AppCard from '~/components/common/AppCard.vue'
import AppTable from '~/components/common/AppTable.vue'
import AppSectionHeader from '~/components/common/AppSectionHeader.vue'
import AppKpiCard from '~/components/common/AppKpiCard.vue'
import { Building2, Package } from 'lucide-vue-next'

useHead({
  title: 'Buying - TOSS Admin',
  meta: [{ name: 'description', content: 'Procurement and buying for TOSS Admin' }]
})

definePageMeta({
  layout: 'dashboard'
})

const suppliers = ref([
  { id: 1, name: 'Supplier A', orders: 12, total: 'R45,250.00', status: 'Active' },
  { id: 2, name: 'Supplier B', orders: 8, total: 'R28,900.00', status: 'Active' },
  { id: 3, name: 'Supplier C', orders: 5, total: 'R15,600.00', status: 'Active' }
])

const purchaseOrders = ref([
  { id: 'PO-001', supplier: 'Supplier A', items: 15, total: 'R3,250.00', status: 'Delivered', date: '2024-01-15' },
  { id: 'PO-002', supplier: 'Supplier B', items: 8, total: 'R1,890.00', status: 'In Progress', date: '2024-01-14' },
  { id: 'PO-003', supplier: 'Supplier C', items: 12, total: 'R2,450.00', status: 'Pending', date: '2024-01-13' }
])
</script>

<template>
  <div class="space-y-6">
    <AppSectionHeader
      title="Buying & Procurement"
      description="Manage suppliers and purchase orders"
    />

    <!-- Suppliers Summary Cards -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
      <AppKpiCard
        v-for="supplier in suppliers"
        :key="supplier.id"
        :title="supplier.name"
        :value="supplier.total"
        :change="`${supplier.orders} orders`"
        change-type="neutral"
        :icon="Building2"
          />
        </div>

    <!-- Purchase Orders Table -->
    <AppCard title="Purchase Orders">
      <div class="mb-4 flex items-center justify-between">
        <input
          type="text"
          placeholder="Search orders..."
          class="px-3 py-2 border border-border rounded-lg text-sm w-64"
        />
        <select class="px-3 py-2 border border-border rounded-lg text-sm">
          <option>All Status</option>
          <option>Pending</option>
          <option>In Progress</option>
          <option>Delivered</option>
        </select>
      </div>
      <AppTable :headers="['Order ID', 'Supplier', 'Items', 'Total', 'Status', 'Date']">
        <tr
          v-for="order in purchaseOrders"
          :key="order.id"
          class="hover:bg-muted/50 transition-colors"
        >
          <td class="px-4 py-3 text-sm font-medium text-foreground">{{ order.id }}</td>
          <td class="px-4 py-3 text-sm text-foreground">{{ order.supplier }}</td>
          <td class="px-4 py-3 text-sm text-muted-foreground">{{ order.items }}</td>
          <td class="px-4 py-3 text-sm font-medium text-foreground">{{ order.total }}</td>
          <td class="px-4 py-3">
            <span
              :class="[
                'inline-flex items-center px-2 py-1 rounded text-xs font-medium',
                order.status === 'Delivered' ? 'bg-green-100 dark:bg-green-900/20 text-green-700 dark:text-green-400' : '',
                order.status === 'In Progress' ? 'bg-blue-100 dark:bg-blue-900/20 text-blue-700 dark:text-blue-400' : '',
                order.status === 'Pending' ? 'bg-orange-100 dark:bg-orange-900/20 text-orange-700 dark:text-orange-400' : ''
              ]"
            >
              {{ order.status }}
            </span>
          </td>
          <td class="px-4 py-3 text-sm text-muted-foreground">{{ order.date }}</td>
        </tr>
      </AppTable>
    </AppCard>
  </div>
</template>
