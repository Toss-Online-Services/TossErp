<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useProjectsStore, type Project, type ProjectStatus } from '~/stores/projects'
import ProjectModal from '~/components/projects/ProjectModal.vue'

useHead({ title: 'Projects - TOSS' })

const projectsStore = useProjectsStore()
const showModal = ref(false)
const selectedProject = ref<Project | null>(null)
const searchQuery = ref('')
const statusFilter = ref<ProjectStatus | 'all'>('all')

const filteredProjects = computed(() => {
  let filtered = projectsStore.projects

  // Filter by status
  if (statusFilter.value !== 'all') {
    filtered = filtered.filter(p => p.status === statusFilter.value)
  }

  // Filter by search query
  if (searchQuery.value.trim()) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(p =>
      p.title.toLowerCase().includes(query) ||
      p.description?.toLowerCase().includes(query) ||
      p.customerName?.toLowerCase().includes(query)
    )
  }

  return filtered.sort((a, b) => 
    new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime()
  )
})

const statusColors: Record<ProjectStatus, string> = {
  New: 'bg-blue-100 text-blue-800',
  InProgress: 'bg-yellow-100 text-yellow-800',
  OnHold: 'bg-orange-100 text-orange-800',
  Completed: 'bg-green-100 text-green-800',
  Closed: 'bg-gray-100 text-gray-800',
  Cancelled: 'bg-red-100 text-red-800'
}

function openCreateModal() {
  selectedProject.value = null
  showModal.value = true
}

function openEditModal(project: Project) {
  selectedProject.value = project
  showModal.value = true
}

function handleSaved() {
  showModal.value = false
  selectedProject.value = null
  projectsStore.fetchProjects()
}

function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
}

function formatDate(date: Date | undefined) {
  if (!date) return '-'
  return new Intl.DateTimeFormat('en-ZA', {
    day: '2-digit',
    month: 'short',
    year: 'numeric'
  }).format(new Date(date))
}

onMounted(() => {
  projectsStore.fetchProjects()
})
</script>

<template>
  <div class="py-6">
    <!-- Header -->
    <div class="mb-8 flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
      <div>
        <h3 class="text-3xl font-bold text-gray-900 mb-2">Projects</h3>
        <p class="text-gray-600 text-sm">Manage projects and track progress</p>
      </div>
      <button
        @click="openCreateModal"
        class="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 flex items-center gap-2 transition-colors"
      >
        <i class="material-symbols-rounded text-xl">add</i>
        <span>New Project</span>
      </button>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Active Projects</p>
            <p class="text-2xl font-bold text-gray-900">{{ projectsStore.activeProjects.length }}</p>
          </div>
          <div class="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-blue-600 text-2xl">work</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Completed</p>
            <p class="text-2xl font-bold text-gray-900">{{ projectsStore.completedProjects.length }}</p>
          </div>
          <div class="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-green-600 text-2xl">check_circle</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Overdue</p>
            <p class="text-2xl font-bold text-red-600">{{ projectsStore.overdueProjects.length }}</p>
          </div>
          <div class="w-12 h-12 bg-red-100 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-red-600 text-2xl">warning</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Total Value</p>
            <p class="text-2xl font-bold text-gray-900">{{ formatCurrency(projectsStore.totalProjectValue) }}</p>
          </div>
          <div class="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-purple-600 text-2xl">attach_money</i>
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
              placeholder="Search projects..."
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
            <option value="New">New</option>
            <option value="InProgress">In Progress</option>
            <option value="OnHold">On Hold</option>
            <option value="Completed">Completed</option>
            <option value="Closed">Closed</option>
            <option value="Cancelled">Cancelled</option>
          </select>
        </div>
      </div>
    </div>

    <!-- Projects List -->
    <div v-if="projectsStore.loading" class="text-center py-12">
      <i class="material-symbols-rounded text-6xl text-gray-400 animate-spin">refresh</i>
      <p class="text-gray-600 mt-4">Loading projects...</p>
    </div>

    <div v-else-if="filteredProjects.length === 0" class="bg-white rounded-xl shadow-sm p-12 text-center">
      <i class="material-symbols-rounded text-6xl text-gray-400 mb-4">work_off</i>
      <h3 class="text-xl font-semibold text-gray-900 mb-2">No Projects Found</h3>
      <p class="text-gray-600 mb-6">Get started by creating your first project</p>
      <button
        @click="openCreateModal"
        class="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
      >
        Create Project
      </button>
    </div>

    <div v-else class="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
      <div
        v-for="project in filteredProjects"
        :key="project.id"
        class="bg-white rounded-xl shadow-sm hover:shadow-md transition-shadow p-6 cursor-pointer"
        @click="openEditModal(project)"
      >
        <div class="flex items-start justify-between mb-4">
          <h4 class="text-lg font-semibold text-gray-900 flex-1">{{ project.title }}</h4>
          <span
            :class="['px-3 py-1 rounded-full text-xs font-medium', statusColors[project.status]]"
          >
            {{ project.status }}
          </span>
        </div>

        <p v-if="project.description" class="text-sm text-gray-600 mb-4 line-clamp-2">
          {{ project.description }}
        </p>

        <div class="space-y-2 mb-4">
          <div v-if="project.customerName" class="flex items-center gap-2 text-sm text-gray-600">
            <i class="material-symbols-rounded text-base">person</i>
            <span>{{ project.customerName }}</span>
          </div>
          <div v-if="project.expectedCompletionDate" class="flex items-center gap-2 text-sm text-gray-600">
            <i class="material-symbols-rounded text-base">calendar_today</i>
            <span>Due: {{ formatDate(project.expectedCompletionDate) }}</span>
          </div>
        </div>

        <div class="flex items-center justify-between pt-4 border-t border-gray-100">
          <div>
            <p class="text-xs text-gray-500">Total Cost</p>
            <p class="text-lg font-semibold text-gray-900">{{ formatCurrency(project.totalCost) }}</p>
          </div>
          <button
            @click.stop="openEditModal(project)"
            class="px-4 py-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
          >
            <i class="material-symbols-rounded">arrow_forward</i>
          </button>
        </div>
      </div>
    </div>

    <!-- Project Modal -->
    <ProjectModal
      v-if="showModal"
      :show="showModal"
      :project="selectedProject"
      @close="showModal = false"
      @saved="handleSaved"
    />
  </div>
</template>
