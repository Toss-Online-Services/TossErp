<template>
  <div class="h-full overflow-y-auto p-6 custom-scrollbar">
    <!-- Page Header -->
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-stone-900 dark:text-white mb-2">Employees</h1>
        <p class="text-stone-500 dark:text-stone-400">Manage your workforce and employee records</p>
      </div>
      <Button class="bg-stone-900 hover:bg-stone-800 dark:bg-white dark:hover:bg-stone-100 dark:text-stone-900">
        <Icon name="lucide:user-plus" class="w-5 h-5 mr-2" />
        Add Employee
      </Button>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Total Employees</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">48</p>
            </div>
            <Icon name="lucide:users" class="w-10 h-10 text-blue-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Active</p>
              <p class="text-2xl font-bold text-green-600">45</p>
            </div>
            <Icon name="lucide:user-check" class="w-10 h-10 text-green-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">On Leave</p>
              <p class="text-2xl font-bold text-yellow-600">3</p>
            </div>
            <Icon name="lucide:palm-tree" class="w-10 h-10 text-yellow-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">New This Month</p>
              <p class="text-2xl font-bold text-purple-600">+4</p>
            </div>
            <Icon name="lucide:user-plus-2" class="w-10 h-10 text-purple-500" />
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Employees Table -->
    <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
      <CardHeader class="border-b border-stone-200 dark:border-stone-700">
        <div class="flex items-center justify-between">
          <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Employee Directory</CardTitle>
          <div class="flex gap-2">
            <div class="relative">
              <Icon name="lucide:search" class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-stone-400" />
              <input type="text" placeholder="Search employees..." class="pl-9 pr-4 py-2 text-sm border border-stone-200 dark:border-stone-700 rounded-lg bg-white dark:bg-stone-900 text-stone-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-stone-500" />
            </div>
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
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Employee</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Department</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Position</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Status</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Start Date</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Actions</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-stone-200 dark:divide-stone-700">
              <tr v-for="employee in employees" :key="employee.id" class="hover:bg-stone-50 dark:hover:bg-stone-700">
                <td class="px-6 py-4">
                  <div class="flex items-center gap-3">
                    <div class="w-10 h-10 rounded-full bg-stone-200 dark:bg-stone-700 flex items-center justify-center">
                      <span class="text-sm font-medium text-stone-600 dark:text-stone-300">{{ employee.initials }}</span>
                    </div>
                    <div>
                      <p class="text-sm font-medium text-stone-900 dark:text-white">{{ employee.name }}</p>
                      <p class="text-xs text-stone-500">{{ employee.email }}</p>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 text-sm text-stone-600 dark:text-stone-400">{{ employee.department }}</td>
                <td class="px-6 py-4 text-sm text-stone-600 dark:text-stone-400">{{ employee.position }}</td>
                <td class="px-6 py-4">
                  <Badge :class="getStatusBadgeClass(employee.status)">{{ employee.status }}</Badge>
                </td>
                <td class="px-6 py-4 text-sm text-stone-600 dark:text-stone-400">{{ employee.startDate }}</td>
                <td class="px-6 py-4">
                  <div class="flex gap-2">
                    <Button variant="ghost" size="sm"><Icon name="lucide:eye" class="w-4 h-4" /></Button>
                    <Button variant="ghost" size="sm"><Icon name="lucide:edit" class="w-4 h-4" /></Button>
                  </div>
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

const employees = ref([
  { id: 1, name: 'Nomvula Mbeki', initials: 'NM', email: 'nomvula@toss.co.za', department: 'Sales', position: 'Store Manager', status: 'active', startDate: 'Jan 15, 2022' },
  { id: 2, name: 'Thabo Molefe', initials: 'TM', email: 'thabo@toss.co.za', department: 'Logistics', position: 'Driver Supervisor', status: 'active', startDate: 'Mar 20, 2021' },
  { id: 3, name: 'Lindiwe Nkosi', initials: 'LN', email: 'lindiwe@toss.co.za', department: 'Finance', position: 'Accountant', status: 'active', startDate: 'Jul 5, 2023' },
  { id: 4, name: 'Sipho Dlamini', initials: 'SD', email: 'sipho@toss.co.za', department: 'Operations', position: 'Warehouse Lead', status: 'on-leave', startDate: 'Feb 10, 2020' },
  { id: 5, name: 'Zanele Mthembu', initials: 'ZM', email: 'zanele@toss.co.za', department: 'Sales', position: 'Cashier', status: 'active', startDate: 'Sep 1, 2024' },
  { id: 6, name: 'Mandla Khumalo', initials: 'MK', email: 'mandla@toss.co.za', department: 'Logistics', position: 'Delivery Driver', status: 'active', startDate: 'Nov 15, 2023' },
])

const getStatusBadgeClass = (status: string) => {
  const classes: Record<string, string> = {
    'active': 'bg-green-100 text-green-800',
    'on-leave': 'bg-yellow-100 text-yellow-800',
    'inactive': 'bg-stone-100 text-stone-800',
    'terminated': 'bg-red-100 text-red-800',
  }
  return classes[status] || 'bg-stone-100 text-stone-800'
}
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: hsl(var(--muted)); }
.custom-scrollbar::-webkit-scrollbar-thumb { background: hsl(var(--muted-foreground)); border-radius: 3px; }
</style>
