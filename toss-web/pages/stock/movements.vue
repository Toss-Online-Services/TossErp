<template>
  <div class="h-full overflow-y-auto p-6 custom-scrollbar">
    <!-- Page Header -->
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-stone-900 dark:text-white mb-2">Stock Movements</h1>
        <p class="text-stone-500 dark:text-stone-400">Track all inventory movements in and out of your store</p>
      </div>
      <Button class="bg-stone-900 hover:bg-stone-800 dark:bg-white dark:hover:bg-stone-100 dark:text-stone-900">
        <Icon name="lucide:plus" class="w-5 h-5 mr-2" />
        New Movement
      </Button>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Stock In Today</p>
              <p class="text-2xl font-bold text-green-600">+R 15,420</p>
            </div>
            <Icon name="lucide:arrow-down-circle" class="w-10 h-10 text-green-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Stock Out Today</p>
              <p class="text-2xl font-bold text-orange-600">-R 8,950</p>
            </div>
            <Icon name="lucide:arrow-up-circle" class="w-10 h-10 text-orange-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Adjustments</p>
              <p class="text-2xl font-bold text-blue-600">23</p>
            </div>
            <Icon name="lucide:refresh-cw" class="w-10 h-10 text-blue-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Transfers</p>
              <p class="text-2xl font-bold text-purple-600">5</p>
            </div>
            <Icon name="lucide:arrow-right-left" class="w-10 h-10 text-purple-500" />
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Movements Table -->
    <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
      <CardHeader class="border-b border-stone-200 dark:border-stone-700">
        <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Recent Movements</CardTitle>
      </CardHeader>
      <CardContent class="p-0">
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-stone-50 dark:bg-stone-900">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Date</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Reference</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Type</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Item</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Qty</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Value</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-stone-200 dark:divide-stone-700">
              <tr v-for="movement in movements" :key="movement.id" class="hover:bg-stone-50 dark:hover:bg-stone-700">
                <td class="px-6 py-4 text-sm text-stone-900 dark:text-white">{{ movement.date }}</td>
                <td class="px-6 py-4 text-sm text-stone-600 dark:text-stone-400">{{ movement.reference }}</td>
                <td class="px-6 py-4">
                  <Badge :class="movement.type === 'IN' ? 'bg-green-100 text-green-800' : movement.type === 'OUT' ? 'bg-orange-100 text-orange-800' : 'bg-blue-100 text-blue-800'">
                    {{ movement.type }}
                  </Badge>
                </td>
                <td class="px-6 py-4 text-sm text-stone-900 dark:text-white">{{ movement.item }}</td>
                <td class="px-6 py-4 text-sm font-medium" :class="movement.type === 'IN' ? 'text-green-600' : 'text-orange-600'">
                  {{ movement.type === 'IN' ? '+' : '-' }}{{ movement.qty }}
                </td>
                <td class="px-6 py-4 text-sm text-stone-900 dark:text-white">R {{ movement.value.toFixed(2) }}</td>
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

const movements = ref([
  { id: 1, date: '2024-12-17', reference: 'GRN-001', type: 'IN', item: 'Coca-Cola 2L', qty: 48, value: 1199.52 },
  { id: 2, date: '2024-12-17', reference: 'INV-245', type: 'OUT', item: 'White Bread', qty: 12, value: 227.88 },
  { id: 3, date: '2024-12-17', reference: 'ADJ-012', type: 'ADJ', item: 'Simba Chips', qty: 5, value: 74.95 },
  { id: 4, date: '2024-12-16', reference: 'GRN-002', type: 'IN', item: 'Maize Meal 2.5kg', qty: 24, value: 1031.76 },
  { id: 5, date: '2024-12-16', reference: 'INV-244', type: 'OUT', item: 'Sugar 2.5kg', qty: 8, value: 439.92 },
])
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar {
  width: 6px;
}
.custom-scrollbar::-webkit-scrollbar-track {
  background: hsl(var(--muted));
}
.custom-scrollbar::-webkit-scrollbar-thumb {
  background: hsl(var(--muted-foreground));
  border-radius: 3px;
}
</style>
