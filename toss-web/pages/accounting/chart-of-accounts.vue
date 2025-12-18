<template>
  <div class="h-full overflow-y-auto p-6 custom-scrollbar">
    <!-- Page Header -->
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-stone-900 dark:text-white mb-2">Chart of Accounts</h1>
        <p class="text-stone-500 dark:text-stone-400">Manage your business accounting structure</p>
      </div>
      <Button class="bg-stone-900 hover:bg-stone-800 dark:bg-white dark:hover:bg-stone-100 dark:text-stone-900">
        <Icon name="lucide:plus" class="w-5 h-5 mr-2" />
        New Account
      </Button>
    </div>

    <!-- Account Type Summary -->
    <div class="grid grid-cols-1 md:grid-cols-5 gap-4 mb-8">
      <Card v-for="type in accountTypes" :key="type.name" class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-4">
          <div class="flex items-center gap-3">
            <div :class="['w-10 h-10 rounded-lg flex items-center justify-center', type.bgColor]">
              <Icon :name="type.icon" :class="['w-5 h-5', type.iconColor]" />
            </div>
            <div>
              <p class="text-xs font-medium text-stone-500 dark:text-stone-400">{{ type.name }}</p>
              <p class="text-lg font-bold text-stone-900 dark:text-white">{{ type.count }}</p>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Accounts Tree -->
    <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
      <CardHeader class="border-b border-stone-200 dark:border-stone-700">
        <div class="flex items-center justify-between">
          <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Account Structure</CardTitle>
          <div class="flex gap-2">
            <Button variant="outline" size="sm">
              <Icon name="lucide:download" class="w-4 h-4 mr-2" />
              Export
            </Button>
          </div>
        </div>
      </CardHeader>
      <CardContent class="p-0">
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-stone-50 dark:bg-stone-900">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Account Code</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Account Name</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Type</th>
                <th class="px-6 py-3 text-right text-xs font-normal text-stone-500 uppercase">Balance</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Actions</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-stone-200 dark:divide-stone-700">
              <tr v-for="account in accounts" :key="account.code" class="hover:bg-stone-50 dark:hover:bg-stone-700">
                <td class="px-6 py-4 text-sm font-mono font-medium text-stone-900 dark:text-white">{{ account.code }}</td>
                <td class="px-6 py-4">
                  <span :class="['text-sm text-stone-900 dark:text-white', account.level > 0 ? 'ml-' + (account.level * 4) : '']">
                    {{ account.name }}
                  </span>
                </td>
                <td class="px-6 py-4">
                  <Badge :class="getTypeBadgeClass(account.type)">{{ account.type }}</Badge>
                </td>
                <td class="px-6 py-4 text-right text-sm font-medium" :class="account.balance >= 0 ? 'text-green-600' : 'text-red-600'">
                  R {{ Math.abs(account.balance).toFixed(2) }}
                </td>
                <td class="px-6 py-4">
                  <Button variant="ghost" size="sm">
                    <Icon name="lucide:edit" class="w-4 h-4" />
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

const accountTypes = [
  { name: 'Assets', count: 15, icon: 'lucide:building-2', bgColor: 'bg-blue-100 dark:bg-blue-900/30', iconColor: 'text-blue-600' },
  { name: 'Liabilities', count: 8, icon: 'lucide:credit-card', bgColor: 'bg-red-100 dark:bg-red-900/30', iconColor: 'text-red-600' },
  { name: 'Equity', count: 4, icon: 'lucide:landmark', bgColor: 'bg-purple-100 dark:bg-purple-900/30', iconColor: 'text-purple-600' },
  { name: 'Income', count: 12, icon: 'lucide:trending-up', bgColor: 'bg-green-100 dark:bg-green-900/30', iconColor: 'text-green-600' },
  { name: 'Expenses', count: 18, icon: 'lucide:trending-down', bgColor: 'bg-orange-100 dark:bg-orange-900/30', iconColor: 'text-orange-600' },
]

const accounts = ref([
  { code: '1000', name: 'Assets', type: 'Asset', level: 0, balance: 450000 },
  { code: '1100', name: 'Current Assets', type: 'Asset', level: 1, balance: 320000 },
  { code: '1110', name: 'Cash at Bank', type: 'Asset', level: 2, balance: 156780 },
  { code: '1120', name: 'Accounts Receivable', type: 'Asset', level: 2, balance: 45680 },
  { code: '1130', name: 'Inventory', type: 'Asset', level: 2, balance: 117540 },
  { code: '2000', name: 'Liabilities', type: 'Liability', level: 0, balance: -125000 },
  { code: '2100', name: 'Accounts Payable', type: 'Liability', level: 1, balance: -78450 },
  { code: '3000', name: 'Equity', type: 'Equity', level: 0, balance: -325000 },
  { code: '4000', name: 'Revenue', type: 'Income', level: 0, balance: -890450 },
  { code: '4100', name: 'Sales Revenue', type: 'Income', level: 1, balance: -856780 },
  { code: '5000', name: 'Expenses', type: 'Expense', level: 0, balance: 456890 },
])

const getTypeBadgeClass = (type: string) => {
  const classes: Record<string, string> = {
    Asset: 'bg-blue-100 text-blue-800',
    Liability: 'bg-red-100 text-red-800',
    Equity: 'bg-purple-100 text-purple-800',
    Income: 'bg-green-100 text-green-800',
    Expense: 'bg-orange-100 text-orange-800',
  }
  return classes[type] || 'bg-stone-100 text-stone-800'
}
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: hsl(var(--muted)); }
.custom-scrollbar::-webkit-scrollbar-thumb { background: hsl(var(--muted-foreground)); border-radius: 3px; }
</style>
