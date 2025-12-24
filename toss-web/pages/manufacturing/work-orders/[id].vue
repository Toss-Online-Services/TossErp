<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const route = useRoute()
const workOrderId = route.params.id

const workOrder = ref({
  id: workOrderId,
  product: 'Fresh Bread',
  quantity: 150,
  plannedDate: '2025-12-24',
  startedAt: '2025-12-23 08:00',
  status: 'in-progress',
  progress: 60,
  bom: 'BOM-001',
  materials: [
    { item: 'Flour (500g)', required: 75, issued: 75, unit: 'kg' },
    { item: 'Water (300ml)', required: 45, issued: 45, unit: 'L' },
    { item: 'Yeast (10g)', required: 1.5, issued: 1.5, unit: 'kg' },
    { item: 'Salt (5g)', required: 0.75, issued: 0.75, unit: 'kg' }
  ],
  operations: [
    { name: 'Mixing', status: 'completed', duration: '30 min' },
    { name: 'Proofing', status: 'completed', duration: '60 min' },
    { name: 'Baking', status: 'in-progress', duration: '45 min' },
    { name: 'Cooling', status: 'pending', duration: '30 min' }
  ]
})
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6">
      <UButton to="/manufacturing/work-orders" variant="ghost" size="sm" class="mb-4">
        <UIcon name="i-heroicons-arrow-left" class="mr-2" />
        Back to Work Orders
      </UButton>
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold mb-2">Work Order {{ workOrderId }}</h1>
          <p class="text-muted-foreground">{{ workOrder.product }} • {{ workOrder.quantity }} units</p>
        </div>
        <UBadge :color="workOrder.status === 'in-progress' ? 'primary' : 'gray'" size="lg">
          {{ workOrder.status }}
        </UBadge>
      </div>
    </div>

    <!-- Progress Bar -->
    <UCard class="mb-6">
      <div class="space-y-2">
        <div class="flex items-center justify-between">
          <span class="font-semibold">Production Progress</span>
          <span class="text-sm text-muted-foreground">{{ workOrder.progress }}%</span>
        </div>
        <div class="w-full bg-gray-200 rounded-full h-3">
          <div class="h-3 rounded-full bg-primary" :style="{ width: `${workOrder.progress}%` }" />
        </div>
      </div>
    </UCard>

    <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
      <!-- Materials Required -->
      <UCard>
        <template #header>
          <h2 class="text-lg font-semibold">Materials Required</h2>
        </template>
        <div class="space-y-3">
          <div v-for="material in workOrder.materials" :key="material.item" class="flex items-center justify-between p-3 border rounded-lg">
            <div>
              <p class="font-medium">{{ material.item }}</p>
              <p class="text-sm text-muted-foreground">Required: {{ material.required }} {{ material.unit }}</p>
            </div>
            <div class="text-right">
              <UBadge :color="material.issued >= material.required ? 'success' : 'warning'">
                {{ material.issued >= material.required ? 'Issued' : 'Pending' }}
              </UBadge>
            </div>
          </div>
        </div>
      </UCard>

      <!-- Operations -->
      <UCard>
        <template #header>
          <h2 class="text-lg font-semibold">Operations</h2>
        </template>
        <div class="space-y-3">
          <div v-for="(operation, index) in workOrder.operations" :key="operation.name" class="flex items-center justify-between p-3 border rounded-lg">
            <div class="flex items-center gap-3">
              <div class="w-8 h-8 rounded-full flex items-center justify-center" :class="{
                'bg-success text-success-foreground': operation.status === 'completed',
                'bg-primary text-primary-foreground': operation.status === 'in-progress',
                'bg-gray-200 text-gray-600': operation.status === 'pending'
              }">
                {{ index + 1 }}
              </div>
              <div>
                <p class="font-medium">{{ operation.name }}</p>
                <p class="text-sm text-muted-foreground">Duration: {{ operation.duration }}</p>
              </div>
            </div>
            <UBadge :color="operation.status === 'completed' ? 'success' : operation.status === 'in-progress' ? 'primary' : 'gray'">
              {{ operation.status }}
            </UBadge>
          </div>
        </div>
      </UCard>
    </div>

    <!-- Action Buttons -->
    <div class="mt-6 flex gap-3">
      <UButton v-if="workOrder.status === 'in-progress'" size="lg">
        <UIcon name="i-heroicons-check" class="mr-2" />
        Mark as Complete
      </UButton>
      <UButton variant="outline" size="lg">
        <UIcon name="i-heroicons-pause" class="mr-2" />
        Pause Production
      </UButton>
      <UButton variant="outline" size="lg" color="red">
        <UIcon name="i-heroicons-x-mark" class="mr-2" />
        Cancel Order
      </UButton>
    </div>
  </div>
</template>
