<template>
  <div class="h-full overflow-y-auto p-6 custom-scrollbar">
    <!-- Page Header -->
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-stone-900 dark:text-white mb-2">Payroll</h1>
        <p class="text-stone-500 dark:text-stone-400">Manage employee salaries and payments</p>
      </div>
      <div class="flex gap-2">
        <Button variant="outline">
          <Icon name="lucide:calendar" class="w-5 h-5 mr-2" />
          Dec 2024
        </Button>
        <Button class="bg-stone-900 hover:bg-stone-800 dark:bg-white dark:hover:bg-stone-100 dark:text-stone-900">
          <Icon name="lucide:play" class="w-5 h-5 mr-2" />
          Run Payroll
        </Button>
      </div>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Total Payroll</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">R 456,780</p>
            </div>
            <Icon name="lucide:wallet" class="w-10 h-10 text-blue-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Net Salaries</p>
              <p class="text-2xl font-bold text-green-600">R 378,450</p>
            </div>
            <Icon name="lucide:banknote" class="w-10 h-10 text-green-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Tax (PAYE)</p>
              <p class="text-2xl font-bold text-orange-600">R 62,340</p>
            </div>
            <Icon name="lucide:receipt" class="w-10 h-10 text-orange-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">UIF</p>
              <p class="text-2xl font-bold text-purple-600">R 15,990</p>
            </div>
            <Icon name="lucide:shield" class="w-10 h-10 text-purple-500" />
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Payroll Status -->
    <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700 mb-6">
      <CardContent class="p-6">
        <div class="flex items-center justify-between">
          <div class="flex items-center gap-4">
            <div class="w-12 h-12 rounded-full bg-yellow-100 dark:bg-yellow-900/30 flex items-center justify-center">
              <Icon name="lucide:clock" class="w-6 h-6 text-yellow-600" />
            </div>
            <div>
              <h3 class="text-lg font-semibold text-stone-900 dark:text-white">December 2024 Payroll</h3>
              <p class="text-sm text-stone-500 dark:text-stone-400">Payment date: December 25, 2024</p>
            </div>
          </div>
          <Badge class="bg-yellow-100 text-yellow-800">Pending Approval</Badge>
        </div>
        <div class="mt-4 pt-4 border-t border-stone-200 dark:border-stone-700">
          <div class="flex gap-8">
            <div>
              <p class="text-xs text-stone-500">Employees</p>
              <p class="text-lg font-semibold text-stone-900 dark:text-white">48</p>
            </div>
            <div>
              <p class="text-xs text-stone-500">Processed</p>
              <p class="text-lg font-semibold text-green-600">45</p>
            </div>
            <div>
              <p class="text-xs text-stone-500">Pending</p>
              <p class="text-lg font-semibold text-yellow-600">3</p>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>

    <!-- Payroll Table -->
    <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
      <CardHeader class="border-b border-stone-200 dark:border-stone-700">
        <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Employee Payslips</CardTitle>
      </CardHeader>
      <CardContent class="p-0">
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-stone-50 dark:bg-stone-900">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Employee</th>
                <th class="px-6 py-3 text-right text-xs font-normal text-stone-500 uppercase">Basic</th>
                <th class="px-6 py-3 text-right text-xs font-normal text-stone-500 uppercase">Allowances</th>
                <th class="px-6 py-3 text-right text-xs font-normal text-stone-500 uppercase">Deductions</th>
                <th class="px-6 py-3 text-right text-xs font-normal text-stone-500 uppercase">Net Pay</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Status</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Actions</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-stone-200 dark:divide-stone-700">
              <tr v-for="payslip in payslips" :key="payslip.id" class="hover:bg-stone-50 dark:hover:bg-stone-700">
                <td class="px-6 py-4">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-full bg-stone-200 dark:bg-stone-700 flex items-center justify-center">
                      <span class="text-xs font-medium text-stone-600 dark:text-stone-300">{{ payslip.initials }}</span>
                    </div>
                    <div>
                      <p class="text-sm font-medium text-stone-900 dark:text-white">{{ payslip.name }}</p>
                      <p class="text-xs text-stone-500">{{ payslip.position }}</p>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 text-sm text-right text-stone-900 dark:text-white">R {{ payslip.basic.toLocaleString() }}</td>
                <td class="px-6 py-4 text-sm text-right text-green-600">+R {{ payslip.allowances.toLocaleString() }}</td>
                <td class="px-6 py-4 text-sm text-right text-red-600">-R {{ payslip.deductions.toLocaleString() }}</td>
                <td class="px-6 py-4 text-sm text-right font-semibold text-stone-900 dark:text-white">R {{ payslip.netPay.toLocaleString() }}</td>
                <td class="px-6 py-4">
                  <Badge :class="payslip.status === 'processed' ? 'bg-green-100 text-green-800' : 'bg-yellow-100 text-yellow-800'">
                    {{ payslip.status }}
                  </Badge>
                </td>
                <td class="px-6 py-4">
                  <Button variant="ghost" size="sm">
                    <Icon name="lucide:file-text" class="w-4 h-4 mr-1" />
                    View
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

const payslips = ref([
  { id: 1, name: 'Nomvula Mbeki', initials: 'NM', position: 'Store Manager', basic: 25000, allowances: 3500, deductions: 5840, netPay: 22660, status: 'processed' },
  { id: 2, name: 'Thabo Molefe', initials: 'TM', position: 'Driver Supervisor', basic: 18000, allowances: 2500, deductions: 4120, netPay: 16380, status: 'processed' },
  { id: 3, name: 'Lindiwe Nkosi', initials: 'LN', position: 'Accountant', basic: 22000, allowances: 2000, deductions: 5040, netPay: 18960, status: 'pending' },
  { id: 4, name: 'Sipho Dlamini', initials: 'SD', position: 'Warehouse Lead', basic: 16000, allowances: 2000, deductions: 3640, netPay: 14360, status: 'processed' },
  { id: 5, name: 'Zanele Mthembu', initials: 'ZM', position: 'Cashier', basic: 8500, allowances: 500, deductions: 1820, netPay: 7180, status: 'processed' },
])
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: hsl(var(--muted)); }
.custom-scrollbar::-webkit-scrollbar-thumb { background: hsl(var(--muted-foreground)); border-radius: 3px; }
</style>
