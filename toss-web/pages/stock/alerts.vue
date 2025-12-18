<template>
  <div class="h-full overflow-y-auto p-6 custom-scrollbar">
    <!-- Page Header -->
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-stone-900 dark:text-white mb-2">Stock Alerts</h1>
        <p class="text-stone-500 dark:text-stone-400">Monitor low stock, expiring items, and reorder notifications</p>
      </div>
      <Button variant="outline">
        <Icon name="lucide:settings" class="w-5 h-5 mr-2" />
        Alert Settings
      </Button>
    </div>

    <!-- Alert Summary -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
      <Card class="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-red-600 dark:text-red-400">Critical Low</p>
              <p class="text-3xl font-bold text-red-700 dark:text-red-300">8</p>
            </div>
            <Icon name="lucide:alert-octagon" class="w-10 h-10 text-red-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-orange-50 dark:bg-orange-900/20 border border-orange-200 dark:border-orange-800">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-orange-600 dark:text-orange-400">Low Stock</p>
              <p class="text-3xl font-bold text-orange-700 dark:text-orange-300">23</p>
            </div>
            <Icon name="lucide:alert-triangle" class="w-10 h-10 text-orange-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-yellow-50 dark:bg-yellow-900/20 border border-yellow-200 dark:border-yellow-800">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-yellow-600 dark:text-yellow-400">Expiring Soon</p>
              <p class="text-3xl font-bold text-yellow-700 dark:text-yellow-300">12</p>
            </div>
            <Icon name="lucide:clock" class="w-10 h-10 text-yellow-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-800">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-blue-600 dark:text-blue-400">Reorder Due</p>
              <p class="text-3xl font-bold text-blue-700 dark:text-blue-300">15</p>
            </div>
            <Icon name="lucide:shopping-cart" class="w-10 h-10 text-blue-500" />
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Alert List -->
    <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
      <CardHeader class="border-b border-stone-200 dark:border-stone-700">
        <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Active Alerts</CardTitle>
      </CardHeader>
      <CardContent class="p-0">
        <div class="divide-y divide-stone-200 dark:divide-stone-700">
          <div v-for="alert in alerts" :key="alert.id" class="p-4 hover:bg-stone-50 dark:hover:bg-stone-700 flex items-center justify-between">
            <div class="flex items-center gap-4">
              <div :class="[
                'w-10 h-10 rounded-lg flex items-center justify-center',
                alert.severity === 'critical' ? 'bg-red-100 dark:bg-red-900/30' :
                alert.severity === 'warning' ? 'bg-orange-100 dark:bg-orange-900/30' :
                'bg-yellow-100 dark:bg-yellow-900/30'
              ]">
                <Icon 
                  :name="alert.type === 'low_stock' ? 'lucide:package-x' : alert.type === 'expiring' ? 'lucide:calendar-x' : 'lucide:shopping-cart'" 
                  :class="[
                    'w-5 h-5',
                    alert.severity === 'critical' ? 'text-red-600' :
                    alert.severity === 'warning' ? 'text-orange-600' :
                    'text-yellow-600'
                  ]"
                />
              </div>
              <div>
                <p class="text-sm font-medium text-stone-900 dark:text-white">{{ alert.item }}</p>
                <p class="text-sm text-stone-500 dark:text-stone-400">{{ alert.message }}</p>
              </div>
            </div>
            <div class="flex items-center gap-4">
              <Badge :class="[
                alert.severity === 'critical' ? 'bg-red-100 text-red-800' :
                alert.severity === 'warning' ? 'bg-orange-100 text-orange-800' :
                'bg-yellow-100 text-yellow-800'
              ]">
                {{ alert.severity }}
              </Badge>
              <Button size="sm" variant="outline">
                Take Action
              </Button>
            </div>
          </div>
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

const alerts = ref([
  { id: 1, item: 'White Bread', type: 'low_stock', severity: 'critical', message: 'Only 3 units remaining - below minimum of 10' },
  { id: 2, item: 'Fresh Milk 1L', type: 'expiring', severity: 'warning', message: 'Expires in 2 days - 15 units at risk' },
  { id: 3, item: 'Coca-Cola 2L', type: 'low_stock', severity: 'warning', message: 'Only 8 units remaining - below minimum of 20' },
  { id: 4, item: 'Sunflower Oil', type: 'reorder', severity: 'info', message: 'Scheduled reorder date reached - suggest ordering 24 units' },
  { id: 5, item: 'Yogurt 500g', type: 'expiring', severity: 'critical', message: 'Expires tomorrow - 8 units at risk' },
  { id: 6, item: 'Maize Meal 2.5kg', type: 'low_stock', severity: 'warning', message: 'Only 12 units remaining - below minimum of 25' },
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
