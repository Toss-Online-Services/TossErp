<template>
  <div class="h-full overflow-y-auto p-6 custom-scrollbar">
    <!-- Page Header -->
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-stone-900 dark:text-white mb-2">Deliveries</h1>
        <p class="text-stone-500 dark:text-stone-400">Manage and track sales deliveries to customers</p>
      </div>
      <Button class="bg-stone-900 hover:bg-stone-800 dark:bg-white dark:hover:bg-stone-100 dark:text-stone-900">
        <Icon name="lucide:plus" class="w-5 h-5 mr-2" />
        Schedule Delivery
      </Button>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Today's Deliveries</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">12</p>
            </div>
            <Icon name="lucide:truck" class="w-10 h-10 text-blue-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">In Transit</p>
              <p class="text-2xl font-bold text-orange-600">5</p>
            </div>
            <Icon name="lucide:navigation" class="w-10 h-10 text-orange-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Completed</p>
              <p class="text-2xl font-bold text-green-600">7</p>
            </div>
            <Icon name="lucide:check-circle" class="w-10 h-10 text-green-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Pending</p>
              <p class="text-2xl font-bold text-yellow-600">8</p>
            </div>
            <Icon name="lucide:clock" class="w-10 h-10 text-yellow-500" />
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Deliveries Table -->
    <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
      <CardHeader class="border-b border-stone-200 dark:border-stone-700">
        <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Delivery Schedule</CardTitle>
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
              </tr>
            </thead>
            <tbody class="divide-y divide-stone-200 dark:divide-stone-700">
              <tr v-for="delivery in deliveries" :key="delivery.id" class="hover:bg-stone-50 dark:hover:bg-stone-700">
                <td class="px-6 py-4 text-sm font-medium text-stone-900 dark:text-white">{{ delivery.number }}</td>
                <td class="px-6 py-4 text-sm text-stone-900 dark:text-white">{{ delivery.customer }}</td>
                <td class="px-6 py-4 text-sm text-stone-600 dark:text-stone-400">{{ delivery.address }}</td>
                <td class="px-6 py-4 text-sm text-stone-900 dark:text-white">{{ delivery.driver }}</td>
                <td class="px-6 py-4">
                  <Badge :class="[
                    delivery.status === 'delivered' ? 'bg-green-100 text-green-800' :
                    delivery.status === 'in-transit' ? 'bg-blue-100 text-blue-800' :
                    'bg-yellow-100 text-yellow-800'
                  ]">
                    {{ delivery.status }}
                  </Badge>
                </td>
                <td class="px-6 py-4 text-sm text-stone-600 dark:text-stone-400">{{ delivery.eta }}</td>
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
  { id: 1, number: 'DEL-001', customer: 'Thabo Molefe', address: 'Shop 12, Soweto Mall', driver: 'Sipho N.', status: 'delivered', eta: 'Completed' },
  { id: 2, number: 'DEL-002', customer: 'Nomsa Dlamini', address: '45 Main Rd, Alexandra', driver: 'David M.', status: 'in-transit', eta: '14:30' },
  { id: 3, number: 'DEL-003', customer: 'Grace Mahlangu', address: '78 Station St, Mamelodi', driver: 'Bongani Z.', status: 'pending', eta: '16:00' },
  { id: 4, number: 'DEL-004', customer: 'Lindiwe Mthembu', address: 'Plot 23, Diepsloot', driver: 'Sipho N.', status: 'in-transit', eta: '15:15' },
])
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: hsl(var(--muted)); }
.custom-scrollbar::-webkit-scrollbar-thumb { background: hsl(var(--muted-foreground)); border-radius: 3px; }
</style>
