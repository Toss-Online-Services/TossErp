<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const stats = ref([
  { label: 'Total Employees', value: '15', icon: 'i-heroicons-users', color: 'primary' },
  { label: 'Present Today', value: '14', icon: 'i-heroicons-check-badge', color: 'success' },
  { label: 'On Leave', value: '1', icon: 'i-heroicons-calendar-days', color: 'warning' },
  { label: 'Payroll Due', value: '5 days', icon: 'i-heroicons-banknotes', color: 'info' }
])

const recentActivity = ref([
  { type: 'leave', message: 'Mary S. requested 3 days leave', time: '2 hours ago' },
  { type: 'attendance', message: '14 employees marked present', time: 'Today 9:00 AM' },
  { type: 'payroll', message: 'December payroll processed', time: 'Yesterday' }
])
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6">
      <h1 class="text-2xl font-bold mb-2">Human Resources</h1>
      <p class="text-muted-foreground">Manage employees, attendance, leave, and payroll</p>
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
    <div class="mb-6 grid grid-cols-2 md:grid-cols-4 gap-4">
      <UButton to="/hr/employees" size="lg" block>
        <UIcon name="i-heroicons-users" class="mr-2" />
        Employees
      </UButton>
      <UButton to="/hr/attendance" size="lg" block variant="outline">
        <UIcon name="i-heroicons-clipboard-document-check" class="mr-2" />
        Attendance
      </UButton>
      <UButton to="/hr/leave" size="lg" block variant="outline">
        <UIcon name="i-heroicons-calendar-days" class="mr-2" />
        Leave Requests
      </UButton>
      <UButton to="/hr/payroll" size="lg" block variant="outline">
        <UIcon name="i-heroicons-banknotes" class="mr-2" />
        Payroll
      </UButton>
    </div>

    <!-- Recent Activity -->
    <UCard>
      <template #header>
        <h2 class="text-lg font-semibold">Recent Activity</h2>
      </template>
      <div class="space-y-3">
        <div v-for="(activity, index) in recentActivity" :key="index" class="flex items-start gap-3 p-3 border-b last:border-b-0">
          <div class="w-8 h-8 rounded-full bg-primary/10 flex items-center justify-center flex-shrink-0">
            <UIcon :name="activity.type === 'leave' ? 'i-heroicons-calendar-days' : activity.type === 'attendance' ? 'i-heroicons-check-badge' : 'i-heroicons-banknotes'" class="w-4 h-4 text-primary" />
          </div>
          <div class="flex-1">
            <p class="text-sm font-medium">{{ activity.message }}</p>
            <p class="text-xs text-muted-foreground">{{ activity.time }}</p>
          </div>
        </div>
      </div>
    </UCard>
  </div>
</template>
