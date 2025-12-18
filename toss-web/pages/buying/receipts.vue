<template>
  <div class="h-full overflow-y-auto p-6 custom-scrollbar">
    <!-- Page Header -->
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-stone-900 dark:text-white mb-2">Goods Receipts</h1>
        <p class="text-stone-500 dark:text-stone-400">Record and manage stock received from suppliers</p>
      </div>
      <Button class="bg-stone-900 hover:bg-stone-800 dark:bg-white dark:hover:bg-stone-100 dark:text-stone-900">
        <Icon name="lucide:plus" class="w-5 h-5 mr-2" />
        New Receipt
      </Button>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Today's Receipts</p>
              <p class="text-2xl font-bold text-green-600">R 28,450</p>
            </div>
            <Icon name="lucide:package-check" class="w-10 h-10 text-green-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Pending QC</p>
              <p class="text-2xl font-bold text-orange-600">5</p>
            </div>
            <Icon name="lucide:clipboard-check" class="w-10 h-10 text-orange-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Month to Date</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">R 456,780</p>
            </div>
            <Icon name="lucide:trending-up" class="w-10 h-10 text-blue-500" />
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Receipts Table -->
    <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
      <CardHeader class="border-b border-stone-200 dark:border-stone-700">
        <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Recent Receipts</CardTitle>
      </CardHeader>
      <CardContent class="p-0">
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-stone-50 dark:bg-stone-900">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">GRN Number</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">PO Reference</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Supplier</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Date</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Value</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Status</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-stone-200 dark:divide-stone-700">
              <tr v-for="receipt in receipts" :key="receipt.id" class="hover:bg-stone-50 dark:hover:bg-stone-700">
                <td class="px-6 py-4 text-sm font-medium text-stone-900 dark:text-white">{{ receipt.number }}</td>
                <td class="px-6 py-4 text-sm text-stone-600 dark:text-stone-400">{{ receipt.poRef }}</td>
                <td class="px-6 py-4 text-sm text-stone-900 dark:text-white">{{ receipt.supplier }}</td>
                <td class="px-6 py-4 text-sm text-stone-600 dark:text-stone-400">{{ receipt.date }}</td>
                <td class="px-6 py-4 text-sm font-medium text-stone-900 dark:text-white">R {{ receipt.value.toFixed(2) }}</td>
                <td class="px-6 py-4">
                  <Badge :class="receipt.status === 'completed' ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800'">
                    {{ receipt.status }}
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

const receipts = ref([
  { id: 1, number: 'GRN-001', poRef: 'PO-2024-001', supplier: 'Metro Cash & Carry', date: '2024-12-17', value: 28450.00, status: 'completed' },
  { id: 2, number: 'GRN-002', poRef: 'PO-2024-002', supplier: 'Makro', date: '2024-12-16', value: 32450.50, status: 'pending' },
  { id: 3, number: 'GRN-003', poRef: 'PO-2024-003', supplier: 'Pioneer Foods', date: '2024-12-15', value: 18920.00, status: 'completed' },
])
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: hsl(var(--muted)); }
.custom-scrollbar::-webkit-scrollbar-thumb { background: hsl(var(--muted-foreground)); border-radius: 3px; }
</style>
