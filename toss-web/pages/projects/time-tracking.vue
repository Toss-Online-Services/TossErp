<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useProjectsStore, type LabourEntry } from '~/stores/projects'
import TimeEntryModal from '~/components/projects/TimeEntryModal.vue'

useHead({ title: 'Time Tracking - TOSS' })

const projectsStore = useProjectsStore()
const showModal = ref(false)
const selectedEntry = ref<LabourEntry | null>(null)
const selectedProjectId = ref<number | null>(null)
const dateFilter = ref<string>('')

const filteredEntries = computed(() => {
  let filtered = projectsStore.labourEntries

  // Filter by project if selected
  if (selectedProjectId.value) {
    filtered = filtered.filter(e => e.projectId === selectedProjectId.value)
  }

  // Filter by date if selected
  if (dateFilter.value) {
    const filterDate = new Date(dateFilter.value)
    filtered = filtered.filter(e => {
      const entryDate = new Date(e.workDate)
      return entryDate.toDateString() === filterDate.toDateString()
    })
  }

  return filtered.sort((a, b) => 
    new Date(b.workDate).getTime() - new Date(a.workDate).getTime()
  )
})

const totalHours = computed(() => {
  return filteredEntries.value.reduce((sum, entry) => sum + entry.hours, 0)
})

const totalCost = computed(() => {
  return filteredEntries.value.reduce((sum, entry) => sum + entry.totalCost, 0)
})

function openCreateModal(projectId?: number) {
  selectedEntry.value = null
  selectedProjectId.value = projectId || null
  showModal.value = true
}

function openEditModal(entry: LabourEntry) {
  selectedEntry.value = entry
  selectedProjectId.value = entry.projectId
  showModal.value = true
}

function handleSaved() {
  showModal.value = false
  selectedEntry.value = null
  if (selectedProjectId.value) {
    projectsStore.fetchLabourEntriesByProject(selectedProjectId.value)
  } else {
    projectsStore.fetchProjects()
  }
}

function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
}

function formatDate(date: Date) {
  return new Intl.DateTimeFormat('en-ZA', {
    day: '2-digit',
    month: 'short',
    year: 'numeric'
  }).format(new Date(date))
}

function formatDateTime(date: Date) {
  return new Intl.DateTimeFormat('en-ZA', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(date))
}

onMounted(() => {
  projectsStore.fetchProjects()
  // Load labour entries for all projects
  projectsStore.projects.forEach(project => {
    projectsStore.fetchLabourEntriesByProject(project.id)
  })
})
</script>

<template>
  <div class="py-6">
    <!-- Header -->
    <div class="mb-8 flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
      <div>
        <h3 class="text-3xl font-bold text-gray-900 mb-2">Time Tracking</h3>
        <p class="text-gray-600 text-sm">Track time spent on projects and tasks</p>
      </div>
      <button
        @click="openCreateModal()"
        class="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 flex items-center gap-2 transition-colors"
      >
        <i class="material-symbols-rounded text-xl">add</i>
        <span>Log Time</span>
      </button>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Total Hours</p>
            <p class="text-2xl font-bold text-gray-900">{{ totalHours.toFixed(1) }}h</p>
          </div>
          <div class="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-blue-600 text-2xl">schedule</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Total Cost</p>
            <p class="text-2xl font-bold text-gray-900">{{ formatCurrency(totalCost) }}</p>
          </div>
          <div class="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-green-600 text-2xl">attach_money</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Entries</p>
            <p class="text-2xl font-bold text-gray-900">{{ filteredEntries.length }}</p>
          </div>
          <div class="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-purple-600 text-2xl">list</i>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white rounded-xl shadow-sm p-4 mb-6">
      <div class="flex flex-col sm:flex-row gap-4">
        <div class="sm:w-64">
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
        <div class="sm:w-48">
          <input
            v-model="dateFilter"
            type="date"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          />
        </div>
      </div>
    </div>

    <!-- Time Entries List -->
    <div v-if="projectsStore.loading" class="text-center py-12">
      <i class="material-symbols-rounded text-6xl text-gray-400 animate-spin">refresh</i>
      <p class="text-gray-600 mt-4">Loading time entries...</p>
    </div>

    <div v-else-if="filteredEntries.length === 0" class="bg-white rounded-xl shadow-sm p-12 text-center">
      <i class="material-symbols-rounded text-6xl text-gray-400 mb-4">schedule</i>
      <h3 class="text-xl font-semibold text-gray-900 mb-2">No Time Entries Found</h3>
      <p class="text-gray-600 mb-6">Start tracking time by logging your first entry</p>
      <button
        @click="openCreateModal()"
        class="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
      >
        Log Time
      </button>
    </div>

    <div v-else class="bg-white rounded-xl shadow-sm overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gray-50 border-b border-gray-200">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Date</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Project</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Worker</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Hours</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Rate</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Total</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Description</th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr
              v-for="entry in filteredEntries"
              :key="entry.id"
              class="hover:bg-gray-50 transition-colors"
            >
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ formatDate(entry.workDate) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ projectsStore.getProjectById(entry.projectId)?.title || `Project #${entry.projectId}` }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ entry.userName || 'Unknown' }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ entry.hours.toFixed(1) }}h
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ formatCurrency(entry.rate) }}/h
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-semibold text-gray-900">
                {{ formatCurrency(entry.totalCost) }}
              </td>
              <td class="px-6 py-4 text-sm text-gray-600">
                {{ entry.description || '-' }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm">
                <button
                  @click="openEditModal(entry)"
                  class="text-blue-600 hover:text-blue-800 px-2 py-1 rounded hover:bg-blue-50 transition-colors"
                >
                  <i class="material-symbols-rounded text-base">edit</i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Time Entry Modal -->
    <TimeEntryModal
      v-if="showModal"
      :show="showModal"
      :entry="selectedEntry"
      :project-id="selectedProjectId"
      @close="showModal = false"
      @saved="handleSaved"
    />
  </div>
</template>
