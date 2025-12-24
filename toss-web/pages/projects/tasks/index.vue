<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const tasks = ref([
  {
    id: 1,
    title: 'Foundation Work',
    project: 'Lotus Primary Classroom Block',
    dueDate: '2025-03-15',
    priority: 'high',
    status: 'completed',
    assignee: 'You'
  },
  {
    id: 2,
    title: 'Roofing',
    project: 'Lotus Primary Classroom Block',
    dueDate: '2025-04-25',
    priority: 'high',
    status: 'in-progress',
    assignee: 'You'
  },
  {
    id: 3,
    title: 'Order Materials',
    project: 'Community Center Renovation',
    dueDate: '2025-12-25',
    priority: 'medium',
    status: 'pending',
    assignee: 'You'
  },
  {
    id: 4,
    title: 'Client Meeting',
    project: 'Community Center Renovation',
    dueDate: '2025-12-24',
    priority: 'high',
    status: 'pending',
    assignee: 'You'
  }
])

const columns = [
  { key: 'title', label: 'Task' },
  { key: 'project', label: 'Project' },
  { key: 'dueDate', label: 'Due Date' },
  { key: 'priority', label: 'Priority' },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions' }
]

const priorityColors = {
  high: 'error',
  medium: 'warning',
  low: 'gray'
}

const statusColors = {
  'pending': 'gray',
  'in-progress': 'primary',
  'completed': 'success'
}
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6 flex items-center justify-between">
      <div>
        <h1 class="text-2xl font-bold mb-2">My Tasks</h1>
        <p class="text-muted-foreground">View and manage all your assigned tasks</p>
      </div>
      <UButton>
        <UIcon name="i-heroicons-plus" class="mr-2" />
        New Task
      </UButton>
    </div>

    <!-- Filters -->
    <div class="mb-4 flex gap-2">
      <UButton size="sm" variant="outline">All Tasks</UButton>
      <UButton size="sm" variant="ghost">Pending</UButton>
      <UButton size="sm" variant="ghost">In Progress</UButton>
      <UButton size="sm" variant="ghost">Completed</UButton>
      <UButton size="sm" variant="ghost">Due Today</UButton>
    </div>

    <!-- Tasks Table -->
    <UCard>
      <UTable :rows="tasks" :columns="columns">
        <template #title-data="{ row }">
          <div>
            <p class="font-medium">{{ row.title }}</p>
            <p class="text-xs text-muted-foreground">{{ row.assignee }}</p>
          </div>
        </template>

        <template #dueDate-data="{ row }">
          <span :class="{
            'text-error font-medium': new Date(row.dueDate) < new Date(),
            'text-warning font-medium': new Date(row.dueDate).toDateString() === new Date().toDateString()
          }">
            {{ new Date(row.dueDate).toLocaleDateString() }}
          </span>
        </template>

        <template #priority-data="{ row }">
          <UBadge :color="priorityColors[row.priority]">
            {{ row.priority }}
          </UBadge>
        </template>

        <template #status-data="{ row }">
          <UBadge :color="statusColors[row.status]">
            {{ row.status }}
          </UBadge>
        </template>

        <template #actions-data="{ row }">
          <div class="flex gap-2">
            <UButton size="xs" variant="ghost">View</UButton>
            <UButton v-if="row.status !== 'completed'" size="xs" variant="ghost">Complete</UButton>
          </div>
        </template>
      </UTable>
    </UCard>
  </div>
</template>
