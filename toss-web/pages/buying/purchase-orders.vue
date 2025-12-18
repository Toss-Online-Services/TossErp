<template>
  <div class="h-full overflow-y-auto p-6 custom-scrollbar">
    <!-- Page Header -->
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-stone-900 dark:text-white mb-2">Purchase Orders</h1>
        <p class="text-stone-500 dark:text-stone-400">Create and manage purchase orders to suppliers</p>
      </div>
      <Button class="bg-stone-900 hover:bg-stone-800 dark:bg-white dark:hover:bg-stone-100 dark:text-stone-900">
        <Icon name="lucide:plus" class="w-5 h-5 mr-2" />
        New Purchase Order
      </Button>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Open Orders</p>
              <p class="text-2xl font-bold text-blue-600">15</p>
            </div>
            <Icon name="lucide:file-text" class="w-10 h-10 text-blue-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Pending Value</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">R 156,780</p>
            </div>
            <Icon name="lucide:wallet" class="w-10 h-10 text-green-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Awaiting Delivery</p>
              <p class="text-2xl font-bold text-orange-600">8</p>
            </div>
            <Icon name="lucide:truck" class="w-10 h-10 text-orange-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Completed MTD</p>
              <p class="text-2xl font-bold text-green-600">42</p>
            </div>
            <Icon name="lucide:check-circle" class="w-10 h-10 text-green-500" />
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Purchase Orders Table -->
    <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
      <CardHeader class="border-b border-stone-200 dark:border-stone-700">
        <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Purchase Orders</CardTitle>
      </CardHeader>
      <CardContent class="p-0">
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-stone-50 dark:bg-stone-900">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">PO Number</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Supplier</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Date</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Items</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Total</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Status</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-stone-200 dark:divide-stone-700">
              <tr v-for="po in purchaseOrders" :key="po.id" class="hover:bg-stone-50 dark:hover:bg-stone-700">
                <td class="px-6 py-4 text-sm font-medium text-stone-900 dark:text-white">{{ po.number }}</td>
                <td class="px-6 py-4 text-sm text-stone-900 dark:text-white">{{ po.supplier }}</td>
                <td class="px-6 py-4 text-sm text-stone-600 dark:text-stone-400">{{ po.date }}</td>
                <td class="px-6 py-4 text-sm text-stone-900 dark:text-white">{{ po.items }}</td>
                <td class="px-6 py-4 text-sm font-medium text-stone-900 dark:text-white">R {{ po.total.toFixed(2) }}</td>
                <td class="px-6 py-4">
                  <Badge :class="[
                    po.status === 'received' ? 'bg-green-100 text-green-800' :
                    po.status === 'sent' ? 'bg-blue-100 text-blue-800' :
                    'bg-yellow-100 text-yellow-800'
                  ]">
                    {{ po.status }}
                  </Badge>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { Card, CardContent, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'
import { Badge } from '~/components/ui/badge'

definePageMeta({
  layout: 'default'
})

const purchaseOrders = ref([
  { id: 1, number: 'PO-2024-001', supplier: 'Metro Cash & Carry', date: '2024-12-17', items: 24, total: 45680.00, status: 'sent' },
  { id: 2, number: 'PO-2024-002', supplier: 'Makro', date: '2024-12-16', items: 18, total: 32450.50, status: 'received' },
  { id: 3, number: 'PO-2024-003', supplier: 'Pioneer Foods', date: '2024-12-15', items: 12, total: 18920.00, status: 'draft' },
  { id: 4, number: 'PO-2024-004', supplier: 'Tiger Brands', date: '2024-12-15', items: 30, total: 56780.00, status: 'sent' },
])
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: hsl(var(--muted)); }
.custom-scrollbar::-webkit-scrollbar-thumb { background: hsl(var(--muted-foreground)); border-radius: 3px; }
</style>
