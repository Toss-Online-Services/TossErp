<template>
  <div class="h-full overflow-y-auto p-6 custom-scrollbar">
    <!-- Page Header -->
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-stone-900 dark:text-white mb-2">Financial Reports</h1>
      <p class="text-stone-500 dark:text-stone-400">Generate and view financial statements and reports</p>
    </div>

    <!-- Report Cards -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
      <Card v-for="report in reports" :key="report.name" class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700 hover:shadow-lg transition-shadow cursor-pointer">
        <CardContent class="p-6">
          <div class="flex items-start justify-between mb-4">
            <div :class="['w-12 h-12 rounded-lg flex items-center justify-center', report.bgColor]">
              <Icon :name="report.icon" :class="['w-6 h-6', report.iconColor]" />
            </div>
            <Badge variant="secondary" class="text-xs">{{ report.period }}</Badge>
          </div>
          <h3 class="text-lg font-semibold text-stone-900 dark:text-white mb-2">{{ report.name }}</h3>
          <p class="text-sm text-stone-500 dark:text-stone-400 mb-4">{{ report.description }}</p>
          <div class="flex items-center justify-between">
            <span class="text-xs text-stone-400 dark:text-stone-500">Last generated: {{ report.lastGenerated }}</span>
            <Button size="sm" variant="outline">
              <Icon name="lucide:download" class="w-4 h-4 mr-1" />
              Generate
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Quick Summary -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardHeader class="border-b border-stone-200 dark:border-stone-700">
          <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Profit & Loss Summary</CardTitle>
        </CardHeader>
        <CardContent class="p-6">
          <div class="space-y-4">
            <div class="flex justify-between items-center py-2 border-b border-stone-100 dark:border-stone-700">
              <span class="text-sm text-stone-600 dark:text-stone-400">Total Revenue</span>
              <span class="text-sm font-semibold text-green-600">R 856,780</span>
            </div>
            <div class="flex justify-between items-center py-2 border-b border-stone-100 dark:border-stone-700">
              <span class="text-sm text-stone-600 dark:text-stone-400">Cost of Goods Sold</span>
              <span class="text-sm font-semibold text-stone-900 dark:text-white">R 542,340</span>
            </div>
            <div class="flex justify-between items-center py-2 border-b border-stone-100 dark:border-stone-700">
              <span class="text-sm text-stone-600 dark:text-stone-400">Gross Profit</span>
              <span class="text-sm font-semibold text-stone-900 dark:text-white">R 314,440</span>
            </div>
            <div class="flex justify-between items-center py-2 border-b border-stone-100 dark:border-stone-700">
              <span class="text-sm text-stone-600 dark:text-stone-400">Operating Expenses</span>
              <span class="text-sm font-semibold text-stone-900 dark:text-white">R 189,560</span>
            </div>
            <div class="flex justify-between items-center py-2 bg-stone-50 dark:bg-stone-900 px-2 rounded-lg">
              <span class="text-sm font-semibold text-stone-900 dark:text-white">Net Profit</span>
              <span class="text-lg font-bold text-green-600">R 124,880</span>
            </div>
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardHeader class="border-b border-stone-200 dark:border-stone-700">
          <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Balance Sheet Summary</CardTitle>
        </CardHeader>
        <CardContent class="p-6">
          <div class="space-y-4">
            <div class="flex justify-between items-center py-2 border-b border-stone-100 dark:border-stone-700">
              <span class="text-sm text-stone-600 dark:text-stone-400">Total Assets</span>
              <span class="text-sm font-semibold text-blue-600">R 450,000</span>
            </div>
            <div class="flex justify-between items-center py-2 border-b border-stone-100 dark:border-stone-700">
              <span class="text-sm text-stone-600 dark:text-stone-400">Current Assets</span>
              <span class="text-sm font-semibold text-stone-900 dark:text-white">R 320,000</span>
            </div>
            <div class="flex justify-between items-center py-2 border-b border-stone-100 dark:border-stone-700">
              <span class="text-sm text-stone-600 dark:text-stone-400">Fixed Assets</span>
              <span class="text-sm font-semibold text-stone-900 dark:text-white">R 130,000</span>
            </div>
            <div class="flex justify-between items-center py-2 border-b border-stone-100 dark:border-stone-700">
              <span class="text-sm text-stone-600 dark:text-stone-400">Total Liabilities</span>
              <span class="text-sm font-semibold text-red-600">R 125,000</span>
            </div>
            <div class="flex justify-between items-center py-2 bg-stone-50 dark:bg-stone-900 px-2 rounded-lg">
              <span class="text-sm font-semibold text-stone-900 dark:text-white">Owner's Equity</span>
              <span class="text-lg font-bold text-purple-600">R 325,000</span>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Card, CardContent, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'
import { Badge } from '~/components/ui/badge'

definePageMeta({
  layout: 'default'
})

const reports = [
  { name: 'Income Statement', description: 'Revenue, expenses, and profit summary', icon: 'lucide:trending-up', bgColor: 'bg-green-100 dark:bg-green-900/30', iconColor: 'text-green-600', period: 'Monthly', lastGenerated: 'Dec 1' },
  { name: 'Balance Sheet', description: 'Assets, liabilities, and equity snapshot', icon: 'lucide:scale', bgColor: 'bg-blue-100 dark:bg-blue-900/30', iconColor: 'text-blue-600', period: 'Monthly', lastGenerated: 'Dec 1' },
  { name: 'Cash Flow', description: 'Cash inflows and outflows analysis', icon: 'lucide:banknote', bgColor: 'bg-purple-100 dark:bg-purple-900/30', iconColor: 'text-purple-600', period: 'Monthly', lastGenerated: 'Nov 30' },
  { name: 'Trial Balance', description: 'Debit and credit balance verification', icon: 'lucide:check-circle', bgColor: 'bg-orange-100 dark:bg-orange-900/30', iconColor: 'text-orange-600', period: 'Weekly', lastGenerated: 'Dec 15' },
  { name: 'Aged Receivables', description: 'Customer outstanding amounts by age', icon: 'lucide:clock', bgColor: 'bg-red-100 dark:bg-red-900/30', iconColor: 'text-red-600', period: 'Weekly', lastGenerated: 'Dec 16' },
  { name: 'VAT Report', description: 'VAT collected and payable summary', icon: 'lucide:receipt', bgColor: 'bg-teal-100 dark:bg-teal-900/30', iconColor: 'text-teal-600', period: 'Bi-Monthly', lastGenerated: 'Dec 1' },
]
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: hsl(var(--muted)); }
.custom-scrollbar::-webkit-scrollbar-thumb { background: hsl(var(--muted-foreground)); border-radius: 3px; }
</style>
