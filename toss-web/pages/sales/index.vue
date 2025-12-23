<script setup lang="ts">
import { ref } from 'vue'
import { Plus } from 'lucide-vue-next'
import AppCard from '~/components/common/AppCard.vue'
import AppTable from '~/components/common/AppTable.vue'
import AppSectionHeader from '~/components/common/AppSectionHeader.vue'
import { Tabs, TabsList, TabsTrigger, TabsContent } from '~/components/ui/tabs'

useHead({
  title: 'Sales - TOSS Admin',
  meta: [{ name: 'description', content: 'Sales management for TOSS Admin' }]
})

definePageMeta({
  layout: 'dashboard'
})

const activeTab = ref('orders')

const orders = ref([
  { id: 'ORD-001', customer: 'John Doe', items: 3, total: 'R245.50', status: 'Completed', date: '2024-01-15' },
  { id: 'ORD-002', customer: 'Jane Smith', items: 5, total: 'R389.00', status: 'Pending', date: '2024-01-15' },
  { id: 'ORD-003', customer: 'Mike Johnson', items: 2, total: 'R156.75', status: 'Completed', date: '2024-01-14' }
])

const posSessions = ref([
  { id: 'POS-001', operator: 'Sarah', sales: 12, total: 'R1,245.50', duration: '2h 15m', date: '2024-01-15' },
  { id: 'POS-002', operator: 'Tom', sales: 8, total: 'R890.00', duration: '1h 45m', date: '2024-01-15' }
])

const invoices = ref([
  { id: 'INV-001', customer: 'John Doe', amount: 'R245.50', status: 'Paid', date: '2024-01-15' },
  { id: 'INV-002', customer: 'Jane Smith', amount: 'R389.00', status: 'Pending', date: '2024-01-15' }
])
</script>

<template>
  <div class="space-y-6">
    <AppSectionHeader
      title="Sales"
      description="Manage orders, POS sessions, and invoices"
    >
      <template #action>
        <button
          class="flex items-center gap-2 px-4 py-2 bg-primary text-primary-foreground rounded-lg text-sm font-medium hover:opacity-90 transition-opacity"
        >
          <Plus :size="16" />
          New Sale
        </button>
      </template>
    </AppSectionHeader>

    <Tabs v-model="activeTab" class="w-full">
      <TabsList class="grid w-full grid-cols-3">
        <TabsTrigger value="orders">Orders</TabsTrigger>
        <TabsTrigger value="pos">POS Sessions</TabsTrigger>
        <TabsTrigger value="invoices">Invoices</TabsTrigger>
      </TabsList>

      <TabsContent value="orders" class="mt-6">
        <AppCard>
          <div class="mb-4 flex items-center justify-between">
            <input
              type="text"
              placeholder="Search orders..."
              class="px-3 py-2 border border-border rounded-lg text-sm w-64"
            />
            <select class="px-3 py-2 border border-border rounded-lg text-sm">
              <option>All Status</option>
              <option>Completed</option>
              <option>Pending</option>
            </select>
          </div>
          <AppTable :headers="['Order ID', 'Customer', 'Items', 'Total', 'Status', 'Date']">
            <tr
              v-for="order in orders"
              :key="order.id"
              class="hover:bg-muted/50 transition-colors cursor-pointer"
            >
              <td class="px-4 py-3 text-sm font-medium text-foreground">{{ order.id }}</td>
              <td class="px-4 py-3 text-sm text-foreground">{{ order.customer }}</td>
              <td class="px-4 py-3 text-sm text-muted-foreground">{{ order.items }}</td>
              <td class="px-4 py-3 text-sm font-medium text-foreground">{{ order.total }}</td>
              <td class="px-4 py-3">
                <span
                  :class="[
                    'inline-flex items-center px-2 py-1 rounded text-xs font-medium',
                    order.status === 'Completed' ? 'bg-green-100 dark:bg-green-900/20 text-green-700 dark:text-green-400' : 'bg-orange-100 dark:bg-orange-900/20 text-orange-700 dark:text-orange-400'
                  ]"
                >
                  {{ order.status }}
                </span>
              </td>
              <td class="px-4 py-3 text-sm text-muted-foreground">{{ order.date }}</td>
            </tr>
          </AppTable>
        </AppCard>
      </TabsContent>

      <TabsContent value="pos" class="mt-6">
        <AppCard>
          <AppTable :headers="['Session ID', 'Operator', 'Sales', 'Total', 'Duration', 'Date']">
            <tr
              v-for="session in posSessions"
              :key="session.id"
              class="hover:bg-muted/50 transition-colors"
            >
              <td class="px-4 py-3 text-sm font-medium text-foreground">{{ session.id }}</td>
              <td class="px-4 py-3 text-sm text-foreground">{{ session.operator }}</td>
              <td class="px-4 py-3 text-sm text-muted-foreground">{{ session.sales }}</td>
              <td class="px-4 py-3 text-sm font-medium text-foreground">{{ session.total }}</td>
              <td class="px-4 py-3 text-sm text-muted-foreground">{{ session.duration }}</td>
              <td class="px-4 py-3 text-sm text-muted-foreground">{{ session.date }}</td>
            </tr>
          </AppTable>
        </AppCard>
      </TabsContent>

      <TabsContent value="invoices" class="mt-6">
        <AppCard>
          <AppTable :headers="['Invoice ID', 'Customer', 'Amount', 'Status', 'Date']">
            <tr
              v-for="invoice in invoices"
              :key="invoice.id"
              class="hover:bg-muted/50 transition-colors"
            >
              <td class="px-4 py-3 text-sm font-medium text-foreground">{{ invoice.id }}</td>
              <td class="px-4 py-3 text-sm text-foreground">{{ invoice.customer }}</td>
              <td class="px-4 py-3 text-sm font-medium text-foreground">{{ invoice.amount }}</td>
              <td class="px-4 py-3">
                <span
                  :class="[
                    'inline-flex items-center px-2 py-1 rounded text-xs font-medium',
                    invoice.status === 'Paid' ? 'bg-green-100 dark:bg-green-900/20 text-green-700 dark:text-green-400' : 'bg-orange-100 dark:bg-orange-900/20 text-orange-700 dark:text-orange-400'
                  ]"
                >
                  {{ invoice.status }}
                </span>
              </td>
              <td class="px-4 py-3 text-sm text-muted-foreground">{{ invoice.date }}</td>
            </tr>
          </AppTable>
        </AppCard>
      </TabsContent>
    </Tabs>
  </div>
</template>
