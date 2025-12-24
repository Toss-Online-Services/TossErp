<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const route = useRoute()
const projectId = route.params.id

const project = ref({
  id: projectId,
  name: 'Lotus Primary Classroom Block',
  client: 'Lotus Primary School',
  startDate: '2025-03-01',
  endDate: '2025-05-30',
  budget: 150000,
  spent: 90000,
  progress: 60,
  status: 'active',
  description: 'Construction of a new classroom block with 4 classrooms, including foundation, walls, roofing, and finishing work.',
  tasks: [
    { id: 1, name: 'Foundation Work', status: 'completed', dueDate: '2025-03-15', assignee: 'Thabo M.', progress: 100 },
    { id: 2, name: 'Wall Construction', status: 'completed', dueDate: '2025-04-10', assignee: 'Thabo M.', progress: 100 },
    { id: 3, name: 'Roofing', status: 'in-progress', dueDate: '2025-04-25', assignee: 'Sipho K.', progress: 70 },
    { id: 4, name: 'Electrical Installation', status: 'pending', dueDate: '2025-05-05', assignee: 'John D.', progress: 0 },
    { id: 5, name: 'Plumbing', status: 'pending', dueDate: '2025-05-10', assignee: 'Mary S.', progress: 0 },
    { id: 6, name: 'Interior Finishing', status: 'pending', dueDate: '2025-05-25', assignee: 'Thabo M.', progress: 0 }
  ],
  milestones: [
    { name: 'Foundation Complete', date: '2025-03-15', status: 'completed' },
    { name: 'Structure Complete', date: '2025-04-30', status: 'in-progress' },
    { name: 'Final Handover', date: '2025-05-30', status: 'pending' }
  ]
})
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6">
      <UButton to="/projects" variant="ghost" size="sm" class="mb-4">
        <UIcon name="i-heroicons-arrow-left" class="mr-2" />
        Back to Projects
      </UButton>
      <div class="flex items-start justify-between">
        <div>
          <h1 class="text-2xl font-bold mb-2">{{ project.name }}</h1>
          <p class="text-muted-foreground">{{ project.id }} • Client: {{ project.client }}</p>
        </div>
        <UBadge :color="project.status === 'completed' ? 'success' : 'primary'" size="lg">
          {{ project.status }}
        </UBadge>
      </div>
    </div>

    <!-- Overview Cards -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <UCard>
        <div>
          <p class="text-sm text-muted-foreground">Progress</p>
          <p class="text-2xl font-bold">{{ project.progress }}%</p>
        </div>
      </UCard>
      <UCard>
        <div>
          <p class="text-sm text-muted-foreground">Budget</p>
          <p class="text-2xl font-bold">R{{ (project.budget / 1000).toFixed(0) }}k</p>
        </div>
      </UCard>
      <UCard>
        <div>
          <p class="text-sm text-muted-foreground">Spent</p>
          <p class="text-2xl font-bold">R{{ (project.spent / 1000).toFixed(0) }}k</p>
        </div>
      </UCard>
      <UCard>
        <div>
          <p class="text-sm text-muted-foreground">Days Left</p>
          <p class="text-2xl font-bold">{{ Math.ceil((new Date(project.endDate).getTime() - new Date().getTime()) / (1000 * 60 * 60 * 24)) }}</p>
        </div>
      </UCard>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <!-- Project Details -->
      <div class="md:col-span-2 space-y-6">
        <!-- Description -->
        <UCard>
          <template #header>
            <h2 class="text-lg font-semibold">Project Description</h2>
          </template>
          <p class="text-muted-foreground">{{ project.description }}</p>
          <div class="mt-4 grid grid-cols-2 gap-4 pt-4 border-t">
            <div>
              <p class="text-sm text-muted-foreground">Start Date</p>
              <p class="font-medium">{{ new Date(project.startDate).toLocaleDateString() }}</p>
            </div>
            <div>
              <p class="text-sm text-muted-foreground">End Date</p>
              <p class="font-medium">{{ new Date(project.endDate).toLocaleDateString() }}</p>
            </div>
          </div>
        </UCard>

        <!-- Tasks -->
        <UCard>
          <template #header>
            <div class="flex items-center justify-between">
              <h2 class="text-lg font-semibold">Tasks</h2>
              <UButton size="sm">
                <UIcon name="i-heroicons-plus" class="mr-1" />
                Add Task
              </UButton>
            </div>
          </template>
          <div class="space-y-3">
            <div v-for="task in project.tasks" :key="task.id" class="p-3 border rounded-lg hover:bg-accent/50 transition-colors">
              <div class="flex items-start justify-between mb-2">
                <div class="flex-1">
                  <h3 class="font-medium">{{ task.name }}</h3>
                  <p class="text-sm text-muted-foreground">Assigned to: {{ task.assignee }}</p>
                </div>
                <UBadge :color="task.status === 'completed' ? 'success' : task.status === 'in-progress' ? 'primary' : 'gray'">
                  {{ task.status }}
                </UBadge>
              </div>
              <div class="space-y-1">
                <div class="flex items-center justify-between text-xs">
                  <span class="text-muted-foreground">Due: {{ new Date(task.dueDate).toLocaleDateString() }}</span>
                  <span class="font-medium">{{ task.progress }}%</span>
                </div>
                <div class="w-full bg-gray-200 rounded-full h-1.5">
                  <div class="h-1.5 rounded-full bg-primary" :style="{ width: `${task.progress}%` }" />
                </div>
              </div>
            </div>
          </div>
        </UCard>
      </div>

      <!-- Sidebar -->
      <div class="space-y-6">
        <!-- Milestones -->
        <UCard>
          <template #header>
            <h2 class="text-lg font-semibold">Milestones</h2>
          </template>
          <div class="space-y-3">
            <div v-for="milestone in project.milestones" :key="milestone.name" class="flex items-start gap-3">
              <div class="w-8 h-8 rounded-full flex items-center justify-center" :class="{
                'bg-success text-success-foreground': milestone.status === 'completed',
                'bg-primary text-primary-foreground': milestone.status === 'in-progress',
                'bg-gray-200 text-gray-600': milestone.status === 'pending'
              }">
                <UIcon v-if="milestone.status === 'completed'" name="i-heroicons-check" class="w-4 h-4" />
                <UIcon v-else-if="milestone.status === 'in-progress'" name="i-heroicons-ellipsis-horizontal" class="w-4 h-4" />
                <UIcon v-else name="i-heroicons-clock" class="w-4 h-4" />
              </div>
              <div class="flex-1">
                <p class="font-medium text-sm">{{ milestone.name }}</p>
                <p class="text-xs text-muted-foreground">{{ new Date(milestone.date).toLocaleDateString() }}</p>
              </div>
            </div>
          </div>
        </UCard>

        <!-- Budget Tracker -->
        <UCard>
          <template #header>
            <h2 class="text-lg font-semibold">Budget Tracker</h2>
          </template>
          <div class="space-y-3">
            <div class="flex items-center justify-between">
              <span class="text-sm text-muted-foreground">Budget</span>
              <span class="font-medium">R{{ project.budget.toLocaleString() }}</span>
            </div>
            <div class="flex items-center justify-between">
              <span class="text-sm text-muted-foreground">Spent</span>
              <span class="font-medium text-primary">R{{ project.spent.toLocaleString() }}</span>
            </div>
            <div class="flex items-center justify-between">
              <span class="text-sm text-muted-foreground">Remaining</span>
              <span class="font-medium text-success">R{{ (project.budget - project.spent).toLocaleString() }}</span>
            </div>
            <div class="pt-3 border-t">
              <div class="w-full bg-gray-200 rounded-full h-2">
                <div class="h-2 rounded-full bg-primary" :style="{ width: `${(project.spent / project.budget * 100)}%` }" />
              </div>
              <p class="text-xs text-muted-foreground mt-1">{{ ((project.spent / project.budget * 100).toFixed(1)) }}% of budget used</p>
            </div>
          </div>
        </UCard>
      </div>
    </div>
  </div>
</template>
