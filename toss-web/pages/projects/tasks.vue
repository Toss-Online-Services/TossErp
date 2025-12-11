<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useProjectsStore, type ProjectTask, type TaskStatus } from '~/stores/projects'
import TaskModal from '~/components/projects/TaskModal.vue'

useHead({ title: 'Tasks - TOSS' })

const projectsStore = useProjectsStore()
const showModal = ref(false)
const selectedTask = ref<ProjectTask | null>(null)
const selectedProjectId = ref<number | null>(null)
const searchQuery = ref('')
const statusFilter = ref<TaskStatus | 'all'>('all')

const filteredTasks = computed(() => {
  let filtered = projectsStore.tasks

  // Filter by project if selected
  if (selectedProjectId.value) {
    filtered = filtered.filter(t => t.projectId === selectedProjectId.value)
  }

  // Filter by status
  if (statusFilter.value !== 'all') {
    filtered = filtered.filter(t => t.status === statusFilter.value)
  }

  // Filter by search query
  if (searchQuery.value.trim()) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(t =>
      t.title.toLowerCase().includes(query) ||
      t.description?.toLowerCase().includes(query) ||
      t.assigneeName?.toLowerCase().includes(query)
    )
  }

  return filtered.sort((a, b) => {
    // Sort by status priority, then by due date
    const statusOrder: Record<TaskStatus, number> = {
      InProgress: 0,
      ToDo: 1,
      Done: 2,
      Cancelled: 3
    }
    const statusDiff = statusOrder[a.status] - statusOrder[b.status]
    if (statusDiff !== 0) return statusDiff

    if (a.dueDate && b.dueDate) {
      return new Date(a.dueDate).getTime() - new Date(b.dueDate).getTime()
    }
    return 0
  })
})

const statusColors: Record<TaskStatus, string> = {
  ToDo: 'bg-gray-100 text-gray-800',
  InProgress: 'bg-blue-100 text-blue-800',
  Done: 'bg-green-100 text-green-800',
  Cancelled: 'bg-red-100 text-red-800'
}

function openCreateModal(projectId?: number) {
  selectedTask.value = null
  selectedProjectId.value = projectId || null
  showModal.value = true
}

function openEditModal(task: ProjectTask) {
  selectedTask.value = task
  selectedProjectId.value = task.projectId
  showModal.value = true
}

function handleSaved() {
  showModal.value = false
  selectedTask.value = null
  if (selectedProjectId.value) {
    projectsStore.fetchTasksByProject(selectedProjectId.value)
  } else {
    projectsStore.fetchProjects()
  }
}

function formatDate(date: Date | undefined) {
  if (!date) return '-'
  return new Intl.DateTimeFormat('en-ZA', {
    day: '2-digit',
    month: 'short',
    year: 'numeric'
  }).format(new Date(date))
}

function formatHours(hours: number | undefined) {
  if (!hours) return '-'
  return `${hours.toFixed(1)}h`
}

onMounted(() => {
  projectsStore.fetchProjects()
  // Load tasks for all projects
  projectsStore.projects.forEach(project => {
    projectsStore.fetchTasksByProject(project.id)
  })
})
</script>

<template>
  <div class="py-6">
    <!-- Header -->
    <div class="mb-8 flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
      <div>
        <h3 class="text-3xl font-bold text-gray-900 mb-2">Tasks</h3>
        <p class="text-gray-600 text-sm">Manage project tasks and assignments</p>
      </div>
      <button
        @click="openCreateModal()"
        class="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 flex items-center gap-2 transition-colors"
      >
        <i class="material-symbols-rounded text-xl">add</i>
        <span>New Task</span>
      </button>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">To Do</p>
            <p class="text-2xl font-bold text-gray-900">{{ projectsStore.tasksByStatus.ToDo.length }}</p>
          </div>
          <div class="w-12 h-12 bg-gray-100 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-gray-600 text-2xl">radio_button_unchecked</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">In Progress</p>
            <p class="text-2xl font-bold text-blue-600">{{ projectsStore.tasksByStatus.InProgress.length }}</p>
          </div>
          <div class="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-blue-600 text-2xl">sync</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Done</p>
            <p class="text-2xl font-bold text-green-600">{{ projectsStore.tasksByStatus.Done.length }}</p>
          </div>
          <div class="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-green-600 text-2xl">check_circle</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Total Tasks</p>
            <p class="text-2xl font-bold text-gray-900">{{ projectsStore.tasks.length }}</p>
          </div>
          <div class="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-purple-600 text-2xl">task</i>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white rounded-xl shadow-sm p-4 mb-6">
      <div class="flex flex-col sm:flex-row gap-4">
        <div class="flex-1">
          <div class="relative">
            <i class="material-symbols-rounded absolute left-3 top-1/2 -translate-y-1/2 text-gray-400">search</i>
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search tasks..."
              class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
        </div>
        <div class="sm:w-48">
          <select
            v-model="statusFilter"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          >
            <option value="all">All Status</option>
            <option value="ToDo">To Do</option>
            <option value="InProgress">In Progress</option>
            <option value="Done">Done</option>
            <option value="Cancelled">Cancelled</option>
          </select>
        </div>
        <div class="sm:w-48">
          <select
            v-model="selectedProjectId"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          >
            <option :value="null">All Projects</option>
            <option
              v-for="project in projectsStore.projects"
              :key="project.id"
              :value="project.id"
            >
              {{ project.title }}
            </option>
          </select>
        </div>
      </div>
    </div>

    <!-- Tasks List -->
    <div v-if="projectsStore.loading" class="text-center py-12">
      <i class="material-symbols-rounded text-6xl text-gray-400 animate-spin">refresh</i>
      <p class="text-gray-600 mt-4">Loading tasks...</p>
    </div>

    <div v-else-if="filteredTasks.length === 0" class="bg-white rounded-xl shadow-sm p-12 text-center">
      <i class="material-symbols-rounded text-6xl text-gray-400 mb-4">task_alt</i>
      <h3 class="text-xl font-semibold text-gray-900 mb-2">No Tasks Found</h3>
      <p class="text-gray-600 mb-6">Create your first task to get started</p>
      <button
        @click="openCreateModal()"
        class="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
      >
        Create Task
      </button>
    </div>

    <div v-else class="space-y-4">
      <div
        v-for="task in filteredTasks"
        :key="task.id"
        class="bg-white rounded-xl shadow-sm hover:shadow-md transition-shadow p-6"
      >
        <div class="flex items-start justify-between mb-4">
          <div class="flex-1">
            <div class="flex items-center gap-3 mb-2">
              <h4 class="text-lg font-semibold text-gray-900">{{ task.title }}</h4>
              <span
                :class="['px-3 py-1 rounded-full text-xs font-medium', statusColors[task.status]]"
              >
                {{ task.status }}
              </span>
            </div>
            <p v-if="task.description" class="text-sm text-gray-600 mb-3">
              {{ task.description }}
            </p>
          </div>
          <button
            @click="openEditModal(task)"
            class="ml-4 px-3 py-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
          >
            <i class="material-symbols-rounded">edit</i>
          </button>
        </div>

        <div class="grid grid-cols-2 md:grid-cols-4 gap-4 pt-4 border-t border-gray-100">
          <div>
            <p class="text-xs text-gray-500 mb-1">Project</p>
            <p class="text-sm font-medium text-gray-900">
              {{ projectsStore.getProjectById(task.projectId)?.title || `Project #${task.projectId}` }}
            </p>
          </div>
          <div>
            <p class="text-xs text-gray-500 mb-1">Assignee</p>
            <p class="text-sm font-medium text-gray-900">
              {{ task.assigneeName || 'Unassigned' }}
            </p>
          </div>
          <div>
            <p class="text-xs text-gray-500 mb-1">Due Date</p>
            <p class="text-sm font-medium text-gray-900">{{ formatDate(task.dueDate) }}</p>
          </div>
          <div>
            <p class="text-xs text-gray-500 mb-1">Hours</p>
            <p class="text-sm font-medium text-gray-900">
              {{ formatHours(task.actualHours) }} / {{ formatHours(task.estimatedHours) }}
            </p>
          </div>
        </div>
      </div>
    </div>

    <!-- Task Modal -->
    <TaskModal
      v-if="showModal"
      :show="showModal"
      :task="selectedTask"
      :project-id="selectedProjectId"
      @close="showModal = false"
      @saved="handleSaved"
    />
  </div>
</template>
