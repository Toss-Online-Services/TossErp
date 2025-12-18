<template>
  <div class="h-full overflow-y-auto p-6 custom-scrollbar">
    <!-- Page Header -->
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-stone-900 dark:text-white mb-2">Delivery Routes</h1>
        <p class="text-stone-500 dark:text-stone-400">Plan and optimize delivery routes</p>
      </div>
      <Button class="bg-stone-900 hover:bg-stone-800 dark:bg-white dark:hover:bg-stone-100 dark:text-stone-900">
        <Icon name="lucide:route" class="w-5 h-5 mr-2" />
        New Route
      </Button>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Active Routes</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">8</p>
            </div>
            <Icon name="lucide:map" class="w-10 h-10 text-blue-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Total Stops</p>
              <p class="text-2xl font-bold text-green-600">56</p>
            </div>
            <Icon name="lucide:map-pin" class="w-10 h-10 text-green-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Est. Distance</p>
              <p class="text-2xl font-bold text-purple-600">245 km</p>
            </div>
            <Icon name="lucide:gauge" class="w-10 h-10 text-purple-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">On-Time Rate</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">94%</p>
            </div>
            <Icon name="lucide:clock" class="w-10 h-10 text-orange-500" />
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Routes Grid -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <Card v-for="route in routes" :key="route.id" class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardHeader class="border-b border-stone-200 dark:border-stone-700">
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-3">
              <div :class="['w-10 h-10 rounded-lg flex items-center justify-center', route.bgColor]">
                <Icon name="lucide:route" :class="['w-5 h-5', route.iconColor]" />
              </div>
              <div>
                <CardTitle class="text-base font-semibold text-stone-900 dark:text-white">{{ route.name }}</CardTitle>
                <p class="text-xs text-stone-500">{{ route.driver }}</p>
              </div>
            </div>
            <Badge :class="route.status === 'active' ? 'bg-green-100 text-green-800' : 'bg-stone-100 text-stone-800'">
              {{ route.status }}
            </Badge>
          </div>
        </CardHeader>
        <CardContent class="p-4">
          <div class="space-y-3">
            <div class="flex justify-between text-sm">
              <span class="text-stone-500">Stops</span>
              <span class="font-medium text-stone-900 dark:text-white">{{ route.stops }} locations</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-stone-500">Distance</span>
              <span class="font-medium text-stone-900 dark:text-white">{{ route.distance }} km</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-stone-500">Est. Time</span>
              <span class="font-medium text-stone-900 dark:text-white">{{ route.time }}</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-stone-500">Progress</span>
              <span class="font-medium text-stone-900 dark:text-white">{{ route.completed }}/{{ route.stops }}</span>
            </div>
            <div class="w-full bg-stone-200 dark:bg-stone-700 rounded-full h-2">
              <div class="bg-green-500 h-2 rounded-full" :style="`width: ${(route.completed / route.stops) * 100}%`"></div>
            </div>
          </div>
          <div class="flex gap-2 mt-4">
            <Button variant="outline" size="sm" class="flex-1">
              <Icon name="lucide:map" class="w-4 h-4 mr-2" />
              View Map
            </Button>
            <Button variant="outline" size="sm" class="flex-1">
              <Icon name="lucide:edit" class="w-4 h-4 mr-2" />
              Edit
            </Button>
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

const routes = ref([
  { id: 1, name: 'Soweto Route A', driver: 'Thabo Molefe', status: 'active', stops: 8, distance: 32, time: '2h 15m', completed: 5, bgColor: 'bg-blue-100 dark:bg-blue-900/30', iconColor: 'text-blue-600' },
  { id: 2, name: 'Alexandra Route', driver: 'Sipho Ndlovu', status: 'active', stops: 6, distance: 24, time: '1h 45m', completed: 2, bgColor: 'bg-green-100 dark:bg-green-900/30', iconColor: 'text-green-600' },
  { id: 3, name: 'Tembisa Express', driver: 'Mandla Khumalo', status: 'active', stops: 10, distance: 45, time: '3h 00m', completed: 7, bgColor: 'bg-purple-100 dark:bg-purple-900/30', iconColor: 'text-purple-600' },
  { id: 4, name: 'Diepsloot Loop', driver: 'Bongani Dlamini', status: 'pending', stops: 5, distance: 18, time: '1h 15m', completed: 0, bgColor: 'bg-orange-100 dark:bg-orange-900/30', iconColor: 'text-orange-600' },
])
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: hsl(var(--muted)); }
.custom-scrollbar::-webkit-scrollbar-thumb { background: hsl(var(--muted-foreground)); border-radius: 3px; }
</style>
