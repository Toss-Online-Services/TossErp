<template>
  <div class="h-full overflow-y-auto p-6 custom-scrollbar">
    <!-- Page Header -->
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-stone-900 dark:text-white mb-2">Projects</h1>
        <p class="text-stone-500 dark:text-stone-400">Manage and track your business projects</p>
      </div>
      <Button class="bg-stone-900 hover:bg-stone-800 dark:bg-white dark:hover:bg-stone-100 dark:text-stone-900">
        <Icon name="lucide:plus" class="w-5 h-5 mr-2" />
        New Project
      </Button>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Total Projects</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">12</p>
            </div>
            <Icon name="lucide:folder" class="w-10 h-10 text-blue-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">In Progress</p>
              <p class="text-2xl font-bold text-blue-600">8</p>
            </div>
            <Icon name="lucide:play-circle" class="w-10 h-10 text-blue-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Completed</p>
              <p class="text-2xl font-bold text-green-600">3</p>
            </div>
            <Icon name="lucide:check-circle" class="w-10 h-10 text-green-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">On Hold</p>
              <p class="text-2xl font-bold text-yellow-600">1</p>
            </div>
            <Icon name="lucide:pause-circle" class="w-10 h-10 text-yellow-500" />
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Projects Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <Card v-for="project in projects" :key="project.id" class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700 hover:shadow-lg transition-shadow">
        <CardHeader class="pb-3">
          <div class="flex items-start justify-between">
            <div :class="['w-10 h-10 rounded-lg flex items-center justify-center', project.bgColor]">
              <Icon :name="project.icon" :class="['w-5 h-5', project.iconColor]" />
            </div>
            <Badge :class="getStatusBadgeClass(project.status)">{{ project.status }}</Badge>
          </div>
          <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white mt-3">{{ project.name }}</CardTitle>
          <p class="text-sm text-stone-500 dark:text-stone-400">{{ project.description }}</p>
        </CardHeader>
        <CardContent>
          <div class="space-y-4">
            <div>
              <div class="flex justify-between text-sm mb-1">
                <span class="text-stone-500">Progress</span>
                <span class="font-medium text-stone-900 dark:text-white">{{ project.progress }}%</span>
              </div>
              <div class="w-full bg-stone-200 dark:bg-stone-700 rounded-full h-2">
                <div :class="['h-2 rounded-full', project.progressColor]" :style="`width: ${project.progress}%`"></div>
              </div>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-stone-500">Tasks</span>
              <span class="font-medium text-stone-900 dark:text-white">{{ project.completedTasks }}/{{ project.totalTasks }}</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-stone-500">Due Date</span>
              <span class="font-medium text-stone-900 dark:text-white">{{ project.dueDate }}</span>
            </div>
            <div class="flex items-center justify-between pt-2 border-t border-stone-200 dark:border-stone-700">
              <div class="flex -space-x-2">
                <div v-for="n in 3" :key="n" class="w-8 h-8 rounded-full bg-stone-300 dark:bg-stone-600 border-2 border-white dark:border-stone-800"></div>
              </div>
              <Button variant="outline" size="sm">View</Button>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Card, CardContent, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'
import { Badge } from '~/components/ui/badge'

definePageMeta({
  layout: 'default'
})

const projects = ref([
  { id: 1, name: 'Store Expansion', description: 'Opening new branch in Tembisa', icon: 'lucide:building', bgColor: 'bg-blue-100 dark:bg-blue-900/30', iconColor: 'text-blue-600', status: 'in-progress', progress: 65, progressColor: 'bg-blue-500', completedTasks: 13, totalTasks: 20, dueDate: 'Jan 15, 2025' },
  { id: 2, name: 'POS System Upgrade', description: 'Upgrading all terminals', icon: 'lucide:monitor', bgColor: 'bg-purple-100 dark:bg-purple-900/30', iconColor: 'text-purple-600', status: 'in-progress', progress: 40, progressColor: 'bg-purple-500', completedTasks: 8, totalTasks: 20, dueDate: 'Dec 30, 2024' },
  { id: 3, name: 'Supplier Onboarding', description: 'Adding 10 new suppliers', icon: 'lucide:truck', bgColor: 'bg-green-100 dark:bg-green-900/30', iconColor: 'text-green-600', status: 'completed', progress: 100, progressColor: 'bg-green-500', completedTasks: 15, totalTasks: 15, dueDate: 'Completed' },
  { id: 4, name: 'Staff Training', description: 'Q1 training program', icon: 'lucide:graduation-cap', bgColor: 'bg-orange-100 dark:bg-orange-900/30', iconColor: 'text-orange-600', status: 'in-progress', progress: 25, progressColor: 'bg-orange-500', completedTasks: 5, totalTasks: 20, dueDate: 'Feb 28, 2025' },
  { id: 5, name: 'Website Redesign', description: 'New e-commerce platform', icon: 'lucide:globe', bgColor: 'bg-teal-100 dark:bg-teal-900/30', iconColor: 'text-teal-600', status: 'on-hold', progress: 10, progressColor: 'bg-teal-500', completedTasks: 2, totalTasks: 20, dueDate: 'TBD' },
  { id: 6, name: 'Inventory Audit', description: 'Annual stock count', icon: 'lucide:clipboard-list', bgColor: 'bg-red-100 dark:bg-red-900/30', iconColor: 'text-red-600', status: 'in-progress', progress: 80, progressColor: 'bg-red-500', completedTasks: 16, totalTasks: 20, dueDate: 'Dec 20, 2024' },
])

const getStatusBadgeClass = (status: string) => {
  const classes: Record<string, string> = {
    'in-progress': 'bg-blue-100 text-blue-800',
    'completed': 'bg-green-100 text-green-800',
    'on-hold': 'bg-yellow-100 text-yellow-800',
    'planning': 'bg-purple-100 text-purple-800',
  }
  return classes[status] || 'bg-stone-100 text-stone-800'
}
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: hsl(var(--muted)); }
.custom-scrollbar::-webkit-scrollbar-thumb { background: hsl(var(--muted-foreground)); border-radius: 3px; }
</style>
