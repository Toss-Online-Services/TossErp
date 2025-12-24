<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const selectedDate = ref(new Date())
const attendance = ref([
  { id: 'EMP-001', name: 'Thabo Mokoena', status: 'present', checkIn: '08:00', checkOut: '17:00' },
  { id: 'EMP-002', name: 'Sipho Khumalo', status: 'present', checkIn: '08:15', checkOut: '-' },
  { id: 'EMP-003', name: 'Mary Sithole', status: 'absent', checkIn: '-', checkOut: '-' },
  { id: 'EMP-004', name: 'John Dlamini', status: 'present', checkIn: '07:45', checkOut: '16:30' }
])

const columns = [
  { key: 'id', label: 'Employee ID' },
  { key: 'name', label: 'Name' },
  { key: 'checkIn', label: 'Check In' },
  { key: 'checkOut', label: 'Check Out' },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions' }
]

const statusColors = {
  present: 'success',
  absent: 'error',
  late: 'warning',
  'half-day': 'info'
}
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6">
      <h1 class="text-2xl font-bold mb-2">Attendance</h1>
      <p class="text-muted-foreground">Track and manage employee attendance</p>
    </div>

    <!-- Date Selector -->
    <div class="mb-4 flex items-center gap-4">
      <div class="flex items-center gap-2">
        <UIcon name="i-heroicons-calendar" class="w-5 h-5 text-muted-foreground" />
        <span class="font-medium">{{ selectedDate.toLocaleDateString() }}</span>
      </div>
      <UButton size="sm" variant="outline">Today</UButton>
      <UButton size="sm" variant="ghost">
        <UIcon name="i-heroicons-chevron-left" />
      </UButton>
      <UButton size="sm" variant="ghost">
        <UIcon name="i-heroicons-chevron-right" />
      </UButton>
    </div>

    <!-- Attendance Table -->
    <UCard>
      <template #header>
        <div class="flex items-center justify-between">
          <h2 class="text-lg font-semibold">Today's Attendance</h2>
          <UButton size="sm">Mark All Present</UButton>
        </div>
      </template>

      <UTable :rows="attendance" :columns="columns">
        <template #status-data="{ row }">
          <UBadge :color="statusColors[row.status]">
            {{ row.status }}
          </UBadge>
        </template>

        <template #actions-data="{ row }">
          <div class="flex gap-2">
            <UButton v-if="row.status === 'absent'" size="xs" variant="ghost">Mark Present</UButton>
            <UButton v-else size="xs" variant="ghost">Mark Absent</UButton>
          </div>
        </template>
      </UTable>
    </UCard>
  </div>
</template>
