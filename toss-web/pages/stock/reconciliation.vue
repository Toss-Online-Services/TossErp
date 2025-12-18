<template>
  <div class="h-full overflow-y-auto p-6 custom-scrollbar">
    <!-- Page Header -->
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-stone-900 dark:text-white mb-2">Stock Reconciliation</h1>
        <p class="text-stone-500 dark:text-stone-400">Verify physical stock against system records</p>
      </div>
      <Button class="bg-stone-900 hover:bg-stone-800 dark:bg-white dark:hover:bg-stone-100 dark:text-stone-900">
        <Icon name="lucide:clipboard-check" class="w-5 h-5 mr-2" />
        Start Count
      </Button>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Last Count</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">Dec 15, 2024</p>
            </div>
            <Icon name="lucide:calendar" class="w-10 h-10 text-blue-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Variance Value</p>
              <p class="text-2xl font-bold text-red-600">-R 2,340</p>
            </div>
            <Icon name="lucide:alert-triangle" class="w-10 h-10 text-red-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Accuracy Rate</p>
              <p class="text-2xl font-bold text-green-600">98.5%</p>
            </div>
            <Icon name="lucide:check-circle" class="w-10 h-10 text-green-500" />
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Reconciliation History -->
    <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
      <CardHeader class="border-b border-stone-200 dark:border-stone-700">
        <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Reconciliation History</CardTitle>
      </CardHeader>
      <CardContent class="p-0">
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-stone-50 dark:bg-stone-900">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Date</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Reference</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Items Counted</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Variance</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Status</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Actions</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-stone-200 dark:divide-stone-700">
              <tr v-for="recon in reconciliations" :key="recon.id" class="hover:bg-stone-50 dark:hover:bg-stone-700">
                <td class="px-6 py-4 text-sm text-stone-900 dark:text-white">{{ recon.date }}</td>
                <td class="px-6 py-4 text-sm text-stone-600 dark:text-stone-400">{{ recon.reference }}</td>
                <td class="px-6 py-4 text-sm text-stone-900 dark:text-white">{{ recon.itemsCounted }}</td>
                <td class="px-6 py-4 text-sm font-medium" :class="recon.variance < 0 ? 'text-red-600' : 'text-green-600'">
                  R {{ recon.variance.toFixed(2) }}
                </td>
                <td class="px-6 py-4">
                  <Badge :class="recon.status === 'approved' ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800'">
                    {{ recon.status }}
                  </Badge>
                </td>
                <td class="px-6 py-4">
                  <Button variant="ghost" size="sm">
                    <Icon name="lucide:eye" class="w-4 h-4" />
                  </Button>
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

const reconciliations = ref([
  { id: 1, date: '2024-12-15', reference: 'REC-001', itemsCounted: 156, variance: -2340.00, status: 'approved' },
  { id: 2, date: '2024-12-01', reference: 'REC-002', itemsCounted: 148, variance: -890.50, status: 'approved' },
  { id: 3, date: '2024-11-15', reference: 'REC-003', itemsCounted: 152, variance: 120.00, status: 'approved' },
  { id: 4, date: '2024-11-01', reference: 'REC-004', itemsCounted: 145, variance: -1560.25, status: 'pending' },
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
