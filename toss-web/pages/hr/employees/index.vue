<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const employees = ref([
  { id: 'EMP-001', name: 'Thabo Mokoena', role: 'Foreman', department: 'Construction', phone: '082 123 4567', status: 'active' },
  { id: 'EMP-002', name: 'Sipho Khumalo', role: 'Roofer', department: 'Construction', phone: '083 234 5678', status: 'active' },
  { id: 'EMP-003', name: 'Mary Sithole', role: 'Plumber', department: 'Construction', phone: '084 345 6789', status: 'active' },
  { id: 'EMP-004', name: 'John Dlamini', role: 'Electrician', department: 'Construction', phone: '085 456 7890', status: 'active' }
])

const columns = [
  { key: 'id', label: 'ID' },
  { key: 'name', label: 'Name' },
  { key: 'role', label: 'Role' },
  { key: 'department', label: 'Department' },
  { key: 'phone', label: 'Phone' },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions' }
]
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6 flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold mb-2">Employees</h1>
        <p class="text-muted-foreground">Manage employee information and records</p>
      </div>
      <UButton>
        <UIcon name="i-heroicons-plus" class="mr-2" />
        Add Employee
      </UButton>
    </div>

    <!-- Employees Table -->
    <UCard>
      <UTable :rows="employees" :columns="columns">
        <template #status-data="{ row }">
          <UBadge :color="row.status === 'active' ? 'success' : 'gray'">
            {{ row.status }}
          </UBadge>
        </template>

        <template #actions-data="{ row }">
          <div class="flex gap-2">
            <UButton size="xs" variant="ghost">View</UButton>
            <UButton size="xs" variant="ghost">Edit</UButton>
          </div>
        </template>
      </UTable>
    </UCard>
  </div>
</template>
