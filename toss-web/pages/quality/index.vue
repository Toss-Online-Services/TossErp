<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const stats = ref([
  { label: 'Active Inspections', value: '5', icon: 'i-heroicons-clipboard-document-check', color: 'primary' },
  { label: 'Pass Rate', value: '94%', icon: 'i-heroicons-chart-bar', color: 'success' },
  { label: 'Failed This Week', value: '2', icon: 'i-heroicons-x-circle', color: 'error' },
  { label: 'Procedures', value: '12', icon: 'i-heroicons-document-text', color: 'info' }
])

const recentInspections = ref([
  { id: 'QI-001', item: 'Fresh Cheese Batch #45', result: 'pass', inspector: 'Mary S.', date: '2025-12-23' },
  { id: 'QI-002', item: 'Bread Batch #120', result: 'fail', inspector: 'John D.', date: '2025-12-22' },
  { id: 'QI-003', item: 'Maize Meal #88', result: 'pass', inspector: 'Mary S.', date: '2025-12-22' }
])
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6">
      <h1 class="text-2xl font-bold mb-2">Quality Management</h1>
      <p class="text-muted-foreground">Manage quality inspections and procedures</p>
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
        New Inspection
      </UButton>
      <UButton to="/quality/inspections" size="lg" variant="outline">
        <UIcon name="i-heroicons-clipboard-document-check" class="mr-2" />
        All Inspections
      </UButton>
      <UButton to="/quality/procedures" size="lg" variant="outline">
        <UIcon name="i-heroicons-document-text" class="mr-2" />
        Procedures
      </UButton>
    </div>

    <!-- Recent Inspections -->
    <UCard>
      <template #header>
        <h2 class="text-lg font-semibold">Recent Inspections</h2>
      </template>

      <div class="space-y-3">
        <div v-for="inspection in recentInspections" :key="inspection.id" class="p-4 border rounded-lg hover:bg-accent/50 transition-colors">
          <div class="flex items-start justify-between">
            <div class="flex-1">
              <h3 class="font-semibold">{{ inspection.item }}</h3>
              <p class="text-sm text-muted-foreground">{{ inspection.id }} • Inspector: {{ inspection.inspector }}</p>
            </div>
            <div class="text-right">
              <UBadge :color="inspection.result === 'pass' ? 'success' : 'error'" size="lg">
                {{ inspection.result.toUpperCase() }}
              </UBadge>
              <p class="text-xs text-muted-foreground mt-1">{{ new Date(inspection.date).toLocaleDateString() }}</p>
            </div>
          </div>
        </div>
      </div>
    </UCard>
  </div>
</template>
