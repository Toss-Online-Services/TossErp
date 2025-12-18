<template>
  <div class="h-full overflow-y-auto p-6 custom-scrollbar">
    <!-- Page Header -->
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-stone-900 dark:text-white mb-2">Drivers</h1>
        <p class="text-stone-500 dark:text-stone-400">Manage delivery drivers and their assignments</p>
      </div>
      <Button class="bg-stone-900 hover:bg-stone-800 dark:bg-white dark:hover:bg-stone-100 dark:text-stone-900">
        <Icon name="lucide:user-plus" class="w-5 h-5 mr-2" />
        Add Driver
      </Button>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Total Drivers</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">24</p>
            </div>
            <Icon name="lucide:users" class="w-10 h-10 text-blue-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">On Duty</p>
              <p class="text-2xl font-bold text-green-600">18</p>
            </div>
            <Icon name="lucide:circle-check" class="w-10 h-10 text-green-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">In Transit</p>
              <p class="text-2xl font-bold text-orange-600">12</p>
            </div>
            <Icon name="lucide:truck" class="w-10 h-10 text-orange-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Avg Rating</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">4.7 ⭐</p>
            </div>
            <Icon name="lucide:star" class="w-10 h-10 text-yellow-500" />
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Drivers Table -->
    <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
      <CardHeader class="border-b border-stone-200 dark:border-stone-700">
        <div class="flex items-center justify-between">
          <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Driver List</CardTitle>
          <div class="relative">
            <Icon name="lucide:search" class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-stone-400" />
            <input type="text" placeholder="Search drivers..." class="pl-9 pr-4 py-2 text-sm border border-stone-200 dark:border-stone-700 rounded-lg bg-white dark:bg-stone-900 text-stone-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-stone-500" />
          </div>
        </div>
      </CardHeader>
      <CardContent class="p-0">
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-stone-50 dark:bg-stone-900">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Driver</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Contact</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Vehicle</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Status</th>
                <th class="px-6 py-3 text-center text-xs font-normal text-stone-500 uppercase">Deliveries</th>
                <th class="px-6 py-3 text-center text-xs font-normal text-stone-500 uppercase">Rating</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Actions</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-stone-200 dark:divide-stone-700">
              <tr v-for="driver in drivers" :key="driver.id" class="hover:bg-stone-50 dark:hover:bg-stone-700">
                <td class="px-6 py-4">
                  <div class="flex items-center gap-3">
                    <div class="w-10 h-10 rounded-full bg-stone-200 dark:bg-stone-700 flex items-center justify-center">
                      <span class="text-sm font-medium text-stone-600 dark:text-stone-300">{{ driver.initials }}</span>
                    </div>
                    <div>
                      <p class="text-sm font-medium text-stone-900 dark:text-white">{{ driver.name }}</p>
                      <p class="text-xs text-stone-500">{{ driver.area }}</p>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 text-sm text-stone-600 dark:text-stone-400">{{ driver.phone }}</td>
                <td class="px-6 py-4 text-sm text-stone-600 dark:text-stone-400">{{ driver.vehicle }}</td>
                <td class="px-6 py-4">
                  <Badge :class="getStatusBadgeClass(driver.status)">{{ driver.status }}</Badge>
                </td>
                <td class="px-6 py-4 text-sm text-center text-stone-900 dark:text-white">{{ driver.deliveries }}</td>
                <td class="px-6 py-4 text-sm text-center text-stone-900 dark:text-white">{{ driver.rating }} ⭐</td>
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

const drivers = ref([
  { id: 1, name: 'Thabo Molefe', initials: 'TM', area: 'Soweto', phone: '082 345 6789', vehicle: 'Toyota Quantum', status: 'in-transit', deliveries: 456, rating: 4.8 },
  { id: 2, name: 'Sipho Ndlovu', initials: 'SN', area: 'Alexandra', phone: '073 456 7890', vehicle: 'VW Caddy', status: 'available', deliveries: 389, rating: 4.6 },
  { id: 3, name: 'Mandla Khumalo', initials: 'MK', area: 'Tembisa', phone: '084 567 8901', vehicle: 'Nissan NP200', status: 'in-transit', deliveries: 521, rating: 4.9 },
  { id: 4, name: 'Bongani Dlamini', initials: 'BD', area: 'Diepsloot', phone: '071 678 9012', vehicle: 'Hyundai H100', status: 'off-duty', deliveries: 278, rating: 4.5 },
  { id: 5, name: 'Xolani Mabaso', initials: 'XM', area: 'Ivory Park', phone: '062 789 0123', vehicle: 'Toyota Hilux', status: 'available', deliveries: 345, rating: 4.7 },
])

const getStatusBadgeClass = (status: string) => {
  const classes: Record<string, string> = {
    'available': 'bg-green-100 text-green-800',
    'in-transit': 'bg-blue-100 text-blue-800',
    'off-duty': 'bg-stone-100 text-stone-800',
    'busy': 'bg-orange-100 text-orange-800',
  }
  return classes[status] || 'bg-stone-100 text-stone-800'
}
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: hsl(var(--muted)); }
.custom-scrollbar::-webkit-scrollbar-thumb { background: hsl(var(--muted-foreground)); border-radius: 3px; }
</style>
