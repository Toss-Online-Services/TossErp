<template>
  <div class="h-full overflow-y-auto p-6 custom-scrollbar">
    <!-- Page Header -->
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-stone-900 dark:text-white mb-2">Tasks</h1>
        <p class="text-stone-500 dark:text-stone-400">Manage project tasks and assignments</p>
      </div>
      <Button class="bg-stone-900 hover:bg-stone-800 dark:bg-white dark:hover:bg-stone-100 dark:text-stone-900">
        <Icon name="lucide:plus" class="w-5 h-5 mr-2" />
        Add Task
      </Button>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Total Tasks</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">156</p>
            </div>
            <Icon name="lucide:list-todo" class="w-10 h-10 text-blue-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">To Do</p>
              <p class="text-2xl font-bold text-yellow-600">42</p>
            </div>
            <Icon name="lucide:circle" class="w-10 h-10 text-yellow-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">In Progress</p>
              <p class="text-2xl font-bold text-blue-600">28</p>
            </div>
            <Icon name="lucide:loader-circle" class="w-10 h-10 text-blue-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Completed</p>
              <p class="text-2xl font-bold text-green-600">86</p>
            </div>
            <Icon name="lucide:check-circle" class="w-10 h-10 text-green-500" />
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Kanban-style columns -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <!-- To Do Column -->
      <Card class="bg-stone-50 dark:bg-stone-900 border border-stone-200 dark:border-stone-700">
        <CardHeader class="pb-3">
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-2">
              <div class="w-3 h-3 rounded-full bg-yellow-500"></div>
              <CardTitle class="text-sm font-semibold text-stone-900 dark:text-white">To Do</CardTitle>
            </div>
            <Badge variant="secondary">{{ todoTasks.length }}</Badge>
          </div>
        </CardHeader>
        <CardContent class="space-y-3">
          <div v-for="task in todoTasks" :key="task.id" class="p-4 bg-white dark:bg-stone-800 rounded-lg border border-stone-200 dark:border-stone-700 cursor-pointer hover:shadow-md transition-shadow">
            <div class="flex items-start justify-between mb-2">
              <Badge :class="getPriorityBadgeClass(task.priority)" class="text-xs">{{ task.priority }}</Badge>
              <Icon name="lucide:more-horizontal" class="w-4 h-4 text-stone-400" />
            </div>
            <h4 class="text-sm font-medium text-stone-900 dark:text-white mb-1">{{ task.title }}</h4>
            <p class="text-xs text-stone-500 dark:text-stone-400 mb-3">{{ task.project }}</p>
            <div class="flex items-center justify-between">
              <div class="w-6 h-6 rounded-full bg-stone-300 dark:bg-stone-600"></div>
              <span class="text-xs text-stone-500">{{ task.dueDate }}</span>
            </div>
          </div>
        </CardContent>
      </Card>

      <!-- In Progress Column -->
      <Card class="bg-stone-50 dark:bg-stone-900 border border-stone-200 dark:border-stone-700">
        <CardHeader class="pb-3">
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-2">
              <div class="w-3 h-3 rounded-full bg-blue-500"></div>
              <CardTitle class="text-sm font-semibold text-stone-900 dark:text-white">In Progress</CardTitle>
            </div>
            <Badge variant="secondary">{{ inProgressTasks.length }}</Badge>
          </div>
        </CardHeader>
        <CardContent class="space-y-3">
          <div v-for="task in inProgressTasks" :key="task.id" class="p-4 bg-white dark:bg-stone-800 rounded-lg border border-stone-200 dark:border-stone-700 cursor-pointer hover:shadow-md transition-shadow">
            <div class="flex items-start justify-between mb-2">
              <Badge :class="getPriorityBadgeClass(task.priority)" class="text-xs">{{ task.priority }}</Badge>
              <Icon name="lucide:more-horizontal" class="w-4 h-4 text-stone-400" />
            </div>
            <h4 class="text-sm font-medium text-stone-900 dark:text-white mb-1">{{ task.title }}</h4>
            <p class="text-xs text-stone-500 dark:text-stone-400 mb-3">{{ task.project }}</p>
            <div class="flex items-center justify-between">
              <div class="w-6 h-6 rounded-full bg-stone-300 dark:bg-stone-600"></div>
              <span class="text-xs text-stone-500">{{ task.dueDate }}</span>
            </div>
          </div>
        </CardContent>
      </Card>

      <!-- Completed Column -->
      <Card class="bg-stone-50 dark:bg-stone-900 border border-stone-200 dark:border-stone-700">
        <CardHeader class="pb-3">
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-2">
              <div class="w-3 h-3 rounded-full bg-green-500"></div>
              <CardTitle class="text-sm font-semibold text-stone-900 dark:text-white">Completed</CardTitle>
            </div>
            <Badge variant="secondary">{{ completedTasks.length }}</Badge>
          </div>
        </CardHeader>
        <CardContent class="space-y-3">
          <div v-for="task in completedTasks" :key="task.id" class="p-4 bg-white dark:bg-stone-800 rounded-lg border border-stone-200 dark:border-stone-700 cursor-pointer hover:shadow-md transition-shadow opacity-75">
            <div class="flex items-start justify-between mb-2">
              <Badge :class="getPriorityBadgeClass(task.priority)" class="text-xs">{{ task.priority }}</Badge>
              <Icon name="lucide:check" class="w-4 h-4 text-green-500" />
            </div>
            <h4 class="text-sm font-medium text-stone-900 dark:text-white mb-1 line-through">{{ task.title }}</h4>
            <p class="text-xs text-stone-500 dark:text-stone-400 mb-3">{{ task.project }}</p>
            <div class="flex items-center justify-between">
              <div class="w-6 h-6 rounded-full bg-stone-300 dark:bg-stone-600"></div>
              <span class="text-xs text-green-500">Done</span>
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

const todoTasks = ref([
  { id: 1, title: 'Review supplier contracts', project: 'Supplier Onboarding', priority: 'high', dueDate: 'Dec 20' },
  { id: 2, title: 'Update inventory counts', project: 'Inventory Audit', priority: 'medium', dueDate: 'Dec 19' },
  { id: 3, title: 'Schedule staff meeting', project: 'Staff Training', priority: 'low', dueDate: 'Dec 22' },
])

const inProgressTasks = ref([
  { id: 4, title: 'Configure new POS terminals', project: 'POS System Upgrade', priority: 'high', dueDate: 'Dec 18' },
  { id: 5, title: 'Site survey for new branch', project: 'Store Expansion', priority: 'medium', dueDate: 'Dec 21' },
])

const completedTasks = ref([
  { id: 6, title: 'Vendor agreement signed', project: 'Supplier Onboarding', priority: 'high', dueDate: 'Completed' },
  { id: 7, title: 'Training materials prepared', project: 'Staff Training', priority: 'medium', dueDate: 'Completed' },
])

const getPriorityBadgeClass = (priority: string) => {
  const classes: Record<string, string> = {
    'high': 'bg-red-100 text-red-800',
    'medium': 'bg-yellow-100 text-yellow-800',
    'low': 'bg-green-100 text-green-800',
  }
  return classes[priority] || 'bg-stone-100 text-stone-800'
}
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: hsl(var(--muted)); }
.custom-scrollbar::-webkit-scrollbar-thumb { background: hsl(var(--muted-foreground)); border-radius: 3px; }
</style>
