<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const reportType = ref('stock-levels')

const stockLevels = ref([
  { item: 'White Bread', current: 120, reorderPoint: 30, status: 'good' },
  { item: 'Fresh Milk', current: 45, reorderPoint: 20, status: 'good' },
  { item: 'Maize Meal 5kg', current: 8, reorderPoint: 15, status: 'low' },
  { item: 'Fresh Cheese', current: 0, reorderPoint: 10, status: 'out' }
])

const movements = ref([
  { date: '2025-12-23', item: 'White Bread', type: 'sale', quantity: 15, balance: 120 },
  { date: '2025-12-23', item: 'Fresh Milk', type: 'purchase', quantity: 20, balance: 45 },
  { date: '2025-12-22', item: 'Maize Meal 5kg', type: 'sale', quantity: 5, balance: 8 }
])
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6">
      <h1 class="text-2xl font-bold mb-2">Stock Reports</h1>
      <p class="text-muted-foreground">Analyze inventory levels and movements</p>
    </div>

    <!-- Report Type Selector -->
    <div class="mb-6 flex gap-2">
      <UButton :variant="reportType === 'stock-levels' ? 'solid' : 'outline'" @click="reportType = 'stock-levels'">
        Current Levels
      </UButton>
      <UButton :variant="reportType === 'movements' ? 'solid' : 'outline'" @click="reportType = 'movements'">
        Movements
      </UButton>
      <UButton :variant="reportType === 'valuation' ? 'solid' : 'outline'" @click="reportType = 'valuation'">
        Valuation
      </UButton>
    </div>

    <!-- Stock Levels Report -->
    <UCard v-if="reportType === 'stock-levels'">
      <template #header>
        <h2 class="text-lg font-semibold">Current Stock Levels</h2>
      </template>

      <div class="space-y-3">
        <div v-for="item in stockLevels" :key="item.item" class="p-3 border rounded-lg">
          <div class="flex items-center justify-between mb-2">
            <span class="font-semibold">{{ item.item }}</span>
            <UBadge :color="item.status === 'good' ? 'success' : item.status === 'low' ? 'warning' : 'error'">
              {{ item.status }}
            </UBadge>
          </div>
          <div class="flex justify-between text-sm">
            <span class="text-muted-foreground">Current: {{ item.current }} units</span>
            <span class="text-muted-foreground">Reorder at: {{ item.reorderPoint }} units</span>
          </div>
          <div class="mt-2 w-full bg-gray-200 rounded-full h-2">
            <div :class="`h-2 rounded-full ${item.status === 'good' ? 'bg-success' : item.status === 'low' ? 'bg-warning' : 'bg-error'}`"
                 :style="{ width: `${Math.min((item.current / (item.reorderPoint * 2)) * 100, 100)}%` }"></div>
          </div>
        </div>
      </div>
    </UCard>

    <!-- Movements Report -->
    <UCard v-if="reportType === 'movements'">
      <template #header>
        <h2 class="text-lg font-semibold">Stock Movements (Last 7 Days)</h2>
      </template>

      <div class="space-y-2">
        <div v-for="(mov, index) in movements" :key="index" class="flex items-center justify-between p-3 border-b last:border-0">
          <div class="flex items-center gap-3">
            <div :class="`p-2 rounded-lg ${mov.type === 'purchase' ? 'bg-success/10' : 'bg-info/10'}`">
              <UIcon :name="mov.type === 'purchase' ? 'i-heroicons-arrow-down-tray' : 'i-heroicons-arrow-up-tray'" 
                     :class="`w-4 h-4 ${mov.type === 'purchase' ? 'text-success' : 'text-info'}`" />
            </div>
            <div>
              <p class="font-medium">{{ mov.item }}</p>
              <p class="text-xs text-muted-foreground">{{ new Date(mov.date).toLocaleDateString() }} • {{ mov.type }}</p>
            </div>
          </div>
          <div class="text-right">
            <p class="font-bold">{{ mov.type === 'purchase' ? '+' : '-' }}{{ mov.quantity }} units</p>
            <p class="text-xs text-muted-foreground">Balance: {{ mov.balance }}</p>
          </div>
        </div>
      </div>
    </UCard>

    <!-- Valuation Report -->
    <UCard v-if="reportType === 'valuation'">
      <template #header>
        <h2 class="text-lg font-semibold">Stock Valuation</h2>
      </template>

      <div class="space-y-4">
        <div class="p-4 bg-primary/10 rounded-lg">
          <p class="text-sm text-muted-foreground mb-1">Total Stock Value</p>
          <p class="text-3xl font-bold text-primary">R25,400</p>
        </div>

        <div class="grid grid-cols-2 gap-4">
          <div class="p-3 border rounded">
            <p class="text-xs text-muted-foreground">Categories</p>
            <p class="text-xl font-bold">8</p>
          </div>
          <div class="p-3 border rounded">
            <p class="text-xs text-muted-foreground">Total Items</p>
            <p class="text-xl font-bold">173</p>
          </div>
        </div>
      </div>
    </UCard>
  </div>
</template>
