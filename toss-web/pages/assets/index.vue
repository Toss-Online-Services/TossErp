<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const stats = ref([
  { label: 'Total Assets', value: '24', icon: 'i-heroicons-cube', color: 'primary' },
  { label: 'Total Value', value: 'R125k', icon: 'i-heroicons-banknotes', color: 'success' },
  { label: 'Due for Maintenance', value: '3', icon: 'i-heroicons-wrench-screwdriver', color: 'warning' },
  { label: 'Depreciation This Year', value: 'R15k', icon: 'i-heroicons-arrow-trending-down', color: 'info' }
])

const recentAssets = ref([
  { id: 'AST-001', name: 'Delivery Van', category: 'Vehicle', value: 85000, status: 'active', lastMaintenance: '2025-11-15' },
  { id: 'AST-002', name: 'Industrial Oven', category: 'Equipment', value: 25000, status: 'active', lastMaintenance: '2025-12-01' },
  { id: 'AST-003', name: 'Sewing Machine', category: 'Equipment', value: 8500, status: 'maintenance', lastMaintenance: '2025-12-20' }
])
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6">
      <h1 class="text-2xl font-bold mb-2">Asset Management</h1>
      <p class="text-muted-foreground">Track and manage business assets</p>
    </div>

    <!-- Stats Grid -->
    <div class="grid grid-cols-2 md:grid-cols-4 gap-4 mb-6">
      <UCard v-for="stat in stats" :key="stat.label" class="hover:shadow-md transition-shadow">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-muted-foreground">{{ stat.label }}</p>
            <p class="text-2xl font-bold mt-1">{{ stat.value }}</p>
          </div>
          <div :class="`p-3 rounded-lg bg-${stat.color}/10`">
            <UIcon :name="stat.icon" class="w-6 h-6" :class="`text-${stat.color}`" />
          </div>
        </div>
      </UCard>
    </div>

    <!-- Quick Actions -->
    <div class="mb-6 flex gap-3">
      <UButton size="lg">
        <UIcon name="i-heroicons-plus" class="mr-2" />
        Add Asset
      </UButton>
      <UButton to="/assets/list" size="lg" variant="outline">
        <UIcon name="i-heroicons-queue-list" class="mr-2" />
        All Assets
      </UButton>
      <UButton to="/assets/maintenance" size="lg" variant="outline">
        <UIcon name="i-heroicons-wrench-screwdriver" class="mr-2" />
        Maintenance
      </UButton>
    </div>

    <!-- Recent Assets -->
    <UCard>
      <template #header>
        <h2 class="text-lg font-semibold">Assets Overview</h2>
      </template>

      <div class="space-y-3">
        <div v-for="asset in recentAssets" :key="asset.id" class="p-4 border rounded-lg hover:bg-accent/50 transition-colors">
          <div class="flex items-start justify-between mb-2">
            <div class="flex-1">
              <h3 class="font-semibold">{{ asset.name }}</h3>
              <p class="text-sm text-muted-foreground">{{ asset.id }} • {{ asset.category }}</p>
            </div>
            <div class="text-right">
              <p class="font-bold text-lg">R{{ (asset.value / 1000).toFixed(0) }}k</p>
              <UBadge :color="asset.status === 'active' ? 'success' : 'warning'">
                {{ asset.status }}
              </UBadge>
            </div>
          </div>
          <p class="text-xs text-muted-foreground">Last maintenance: {{ new Date(asset.lastMaintenance).toLocaleDateString() }}</p>
        </div>
      </div>
    </UCard>
  </div>
</template>
