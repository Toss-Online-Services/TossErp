<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const stats = ref([
  { label: 'Active Projects', value: '8', icon: 'i-heroicons-folder-open', color: 'primary' },
  { label: 'Tasks Due Today', value: '12', icon: 'i-heroicons-calendar', color: 'warning' },
  { label: 'Completed This Week', value: '24', icon: 'i-heroicons-check-circle', color: 'success' },
  { label: 'Team Members', value: '15', icon: 'i-heroicons-users', color: 'info' }
])

const projects = ref([
  {
    id: 'PRJ-001',
    name: 'Lotus Primary Classroom Block',
    client: 'Lotus Primary School',
    progress: 60,
    dueDate: '2025-05-30',
    status: 'active',
    tasks: { total: 15, completed: 9 }
  },
  {
    id: 'PRJ-002',
    name: 'Community Center Renovation',
    client: 'Township Council',
    progress: 35,
    dueDate: '2025-07-15',
    status: 'active',
    tasks: { total: 20, completed: 7 }
  },
  {
    id: 'PRJ-003',
    name: 'Wedding Catering - Naledi',
    client: 'Naledi Mokgoko',
    progress: 100,
    dueDate: '2025-01-15',
    status: 'completed',
    tasks: { total: 10, completed: 10 }
  }
])
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6">
      <h1 class="text-2xl font-bold mb-2">Projects</h1>
      <p class="text-muted-foreground">Manage projects, tasks, and track progress</p>
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
        New Project
      </UButton>
      <UButton to="/projects/tasks" size="lg" variant="outline">
        <UIcon name="i-heroicons-list-bullet" class="mr-2" />
        My Tasks
      </UButton>
    </div>

    <!-- Projects List -->
    <div class="space-y-4">
      <UCard v-for="project in projects" :key="project.id" class="hover:shadow-md transition-shadow cursor-pointer" @click="navigateTo(`/projects/${project.id}`)">
        <div class="space-y-4">
          <div class="flex items-start justify-between">
            <div class="flex-1">
              <div class="flex items-center gap-2 mb-1">
                <h3 class="text-lg font-semibold">{{ project.name }}</h3>
                <UBadge :color="project.status === 'completed' ? 'success' : 'primary'">
                  {{ project.status }}
                </UBadge>
              </div>
              <p class="text-sm text-muted-foreground">{{ project.id }} • Client: {{ project.client }}</p>
            </div>
            <div class="text-right text-sm">
              <p class="text-muted-foreground">Due Date</p>
              <p class="font-medium">{{ new Date(project.dueDate).toLocaleDateString() }}</p>
            </div>
          </div>

          <div class="space-y-2">
            <div class="flex items-center justify-between text-sm">
              <span class="text-muted-foreground">Progress</span>
              <span class="font-medium">{{ project.progress }}%</span>
            </div>
            <div class="w-full bg-gray-200 rounded-full h-2">
              <div class="h-2 rounded-full bg-primary" :style="{ width: `${project.progress}%` }" />
            </div>
          </div>

          <div class="flex items-center justify-between pt-2 border-t">
            <div class="flex items-center gap-4 text-sm text-muted-foreground">
              <span>
                <UIcon name="i-heroicons-check-circle" class="inline mr-1" />
                {{ project.tasks.completed }}/{{ project.tasks.total }} tasks
              </span>
            </div>
            <UButton size="xs" variant="ghost" @click.stop>
              View Details
              <UIcon name="i-heroicons-arrow-right" class="ml-1" />
            </UButton>
          </div>
        </div>
      </UCard>
    </div>
  </div>
</template>
