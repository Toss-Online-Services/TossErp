<template>
  <div class="h-full overflow-y-auto p-6 custom-scrollbar">
    <!-- Page Header -->
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-stone-900 dark:text-white mb-2">Deliveries</h1>
        <p class="text-stone-500 dark:text-stone-400">Track and manage all deliveries</p>
      </div>
      <Button class="bg-stone-900 hover:bg-stone-800 dark:bg-white dark:hover:bg-stone-100 dark:text-stone-900">
        <Icon name="lucide:plus" class="w-5 h-5 mr-2" />
        Schedule Delivery
      </Button>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-5 gap-4 mb-8">
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-4">
          <p class="text-sm font-medium text-stone-500">Total Today</p>
          <p class="text-2xl font-bold text-stone-900 dark:text-white">48</p>
        </CardContent>
      </Card>
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-4">
          <p class="text-sm font-medium text-stone-500">Pending</p>
          <p class="text-2xl font-bold text-yellow-600">12</p>
        </CardContent>
      </Card>
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-4">
          <p class="text-sm font-medium text-stone-500">In Transit</p>
          <p class="text-2xl font-bold text-blue-600">18</p>
        </CardContent>
      </Card>
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-4">
          <p class="text-sm font-medium text-stone-500">Completed</p>
          <p class="text-2xl font-bold text-green-600">15</p>
        </CardContent>
      </Card>
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-4">
          <p class="text-sm font-medium text-stone-500">Failed</p>
          <p class="text-2xl font-bold text-red-600">3</p>
        </CardContent>
      </Card>
    </div>

    <!-- Deliveries Table -->
    <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
      <CardHeader class="border-b border-stone-200 dark:border-stone-700">
        <div class="flex items-center justify-between">
          <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Delivery Schedule</CardTitle>
          <div class="flex gap-2">
            <Button variant="outline" size="sm">
              <Icon name="lucide:filter" class="w-4 h-4 mr-2" />
              Filter
            </Button>
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
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Delivery #</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Customer</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Address</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Driver</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Status</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">ETA</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Actions</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-stone-200 dark:divide-stone-700">
              <tr v-for="delivery in deliveries" :key="delivery.id" class="hover:bg-stone-50 dark:hover:bg-stone-700">
                <td class="px-6 py-4 text-sm font-medium text-stone-900 dark:text-white">{{ delivery.number }}</td>
                <td class="px-6 py-4 text-sm text-stone-900 dark:text-white">{{ delivery.customer }}</td>
                <td class="px-6 py-4 text-sm text-stone-600 dark:text-stone-400 max-w-xs truncate">{{ delivery.address }}</td>
                <td class="px-6 py-4 text-sm text-stone-600 dark:text-stone-400">{{ delivery.driver }}</td>
                <td class="px-6 py-4">
                  <Badge :class="getStatusBadgeClass(delivery.status)">{{ delivery.status }}</Badge>
                </td>
                <td class="px-6 py-4 text-sm text-stone-600 dark:text-stone-400">{{ delivery.eta }}</td>
                <td class="px-6 py-4">
                  <div class="flex gap-2">
                    <Button variant="ghost" size="sm"><Icon name="lucide:map-pin" class="w-4 h-4" /></Button>
                    <Button variant="ghost" size="sm"><Icon name="lucide:phone" class="w-4 h-4" /></Button>
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

const deliveries = ref([
  { id: 1, number: 'DEL-001', customer: 'Mamas Kitchen', address: '45 Vilakazi St, Soweto', driver: 'Thabo M.', status: 'in-transit', eta: '10:30 AM' },
  { id: 2, number: 'DEL-002', customer: 'Quick Stop Spaza', address: '12 Main Rd, Alexandra', driver: 'Sipho N.', status: 'delivered', eta: 'Completed' },
  { id: 3, number: 'DEL-003', customer: 'Township Traders', address: '78 Freedom Ave, Tembisa', driver: 'Mandla K.', status: 'pending', eta: '11:00 AM' },
  { id: 4, number: 'DEL-004', customer: 'Corner Store', address: '23 Hope St, Diepsloot', driver: 'Unassigned', status: 'pending', eta: '12:30 PM' },
  { id: 5, number: 'DEL-005', customer: 'Fresh Foods', address: '56 Market Lane, Ivory Park', driver: 'Xolani M.', status: 'in-transit', eta: '11:45 AM' },
])

const getStatusBadgeClass = (status: string) => {
  const classes: Record<string, string> = {
    'pending': 'bg-yellow-100 text-yellow-800',
    'in-transit': 'bg-blue-100 text-blue-800',
    'delivered': 'bg-green-100 text-green-800',
    'failed': 'bg-red-100 text-red-800',
  }
  return classes[status] || 'bg-stone-100 text-stone-800'
}
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: hsl(var(--muted)); }
.custom-scrollbar::-webkit-scrollbar-thumb { background: hsl(var(--muted-foreground)); border-radius: 3px; }
</style>
