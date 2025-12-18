<template>
  <div class="h-full overflow-y-auto p-6 custom-scrollbar">
    <!-- Page Header -->
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-stone-900 dark:text-white mb-2">Journal Entries</h1>
        <p class="text-stone-500 dark:text-stone-400">Record and manage accounting journal entries</p>
      </div>
      <Button class="bg-stone-900 hover:bg-stone-800 dark:bg-white dark:hover:bg-stone-100 dark:text-stone-900">
        <Icon name="lucide:plus" class="w-5 h-5 mr-2" />
        New Entry
      </Button>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Total Entries</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">1,234</p>
            </div>
            <Icon name="lucide:file-text" class="w-10 h-10 text-blue-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">This Month</p>
              <p class="text-2xl font-bold text-green-600">156</p>
            </div>
            <Icon name="lucide:calendar" class="w-10 h-10 text-green-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Total Debits</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">R 2.4M</p>
            </div>
            <Icon name="lucide:arrow-up-right" class="w-10 h-10 text-purple-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Total Credits</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">R 2.4M</p>
            </div>
            <Icon name="lucide:arrow-down-right" class="w-10 h-10 text-orange-500" />
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Journal Entries Table -->
    <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
      <CardHeader class="border-b border-stone-200 dark:border-stone-700">
        <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Recent Entries</CardTitle>
      </CardHeader>
      <CardContent class="p-0">
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-stone-50 dark:bg-stone-900">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Entry #</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Date</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Description</th>
                <th class="px-6 py-3 text-right text-xs font-normal text-stone-500 uppercase">Debit</th>
                <th class="px-6 py-3 text-right text-xs font-normal text-stone-500 uppercase">Credit</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Status</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-stone-200 dark:divide-stone-700">
              <tr v-for="entry in journalEntries" :key="entry.id" class="hover:bg-stone-50 dark:hover:bg-stone-700">
                <td class="px-6 py-4 text-sm font-medium text-stone-900 dark:text-white">{{ entry.number }}</td>
                <td class="px-6 py-4 text-sm text-stone-600 dark:text-stone-400">{{ entry.date }}</td>
                <td class="px-6 py-4 text-sm text-stone-900 dark:text-white">{{ entry.description }}</td>
                <td class="px-6 py-4 text-sm text-right font-medium text-stone-900 dark:text-white">R {{ entry.debit.toFixed(2) }}</td>
                <td class="px-6 py-4 text-sm text-right font-medium text-stone-900 dark:text-white">R {{ entry.credit.toFixed(2) }}</td>
                <td class="px-6 py-4">
                  <Badge :class="entry.status === 'posted' ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800'">
                    {{ entry.status }}
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

const journalEntries = ref([
  { id: 1, number: 'JE-001', date: '2024-12-17', description: 'Sales revenue - Cash sales', debit: 15680.00, credit: 15680.00, status: 'posted' },
  { id: 2, number: 'JE-002', date: '2024-12-17', description: 'Purchase inventory - Metro', debit: 28450.00, credit: 28450.00, status: 'posted' },
  { id: 3, number: 'JE-003', date: '2024-12-16', description: 'Payroll expenses', debit: 45000.00, credit: 45000.00, status: 'draft' },
  { id: 4, number: 'JE-004', date: '2024-12-16', description: 'Rent payment', debit: 8500.00, credit: 8500.00, status: 'posted' },
])
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: hsl(var(--muted)); }
.custom-scrollbar::-webkit-scrollbar-thumb { background: hsl(var(--muted-foreground)); border-radius: 3px; }
</style>
