<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Community Projects</h1>
              <p class="text-gray-600 dark:text-gray-400">Manage community initiatives and collaborative projects</p>
            </div>
            <div class="flex space-x-3">
              <button @click="showNewProjectModal = true" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors">
                <PlusIcon class="w-5 h-5 inline mr-2" />
                New Project
              </button>
              <button @click="viewProjectCalendar" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors">
                <CalendarIcon class="w-5 h-5 inline mr-2" />
                Calendar
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center">
            <ProjectIcon class="w-8 h-8 text-blue-600" />
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Active Projects</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.activeProjects }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center">
            <CheckCircleIcon class="w-8 h-8 text-green-600" />
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Completed</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.completedProjects }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center">
            <UsersIcon class="w-8 h-8 text-purple-600" />
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Community Members</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.communityMembers }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center">
            <CurrencyDollarIcon class="w-8 h-8 text-yellow-600" />
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total Budget</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">R{{ stats.totalBudget.toLocaleString() }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Active Projects -->
      <div class="mb-8">
        <h2 class="text-xl font-bold text-gray-900 dark:text-white mb-4">Active Community Projects</h2>
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div v-for="project in activeProjects" :key="project.id" class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">{{ project.name }}</h3>
              <span :class="getStatusColor(project.status)" class="px-2 py-1 text-xs font-medium rounded-full">
                {{ project.status }}
              </span>
            </div>
            <p class="text-gray-600 dark:text-gray-400 mb-4">{{ project.description }}</p>
            
            <div class="grid grid-cols-2 gap-4 mb-4">
              <div>
                <p class="text-sm text-gray-500 dark:text-gray-400">Budget</p>
                <p class="font-medium text-gray-900 dark:text-white">R{{ project.budget.toLocaleString() }}</p>
              </div>
              <div>
                <p class="text-sm text-gray-500 dark:text-gray-400">Participants</p>
                <p class="font-medium text-gray-900 dark:text-white">{{ project.participants }}</p>
              </div>
            </div>

            <div class="mb-4">
              <div class="flex items-center justify-between mb-2">
                <span class="text-sm text-gray-600 dark:text-gray-400">Progress</span>
                <span class="text-sm font-medium text-gray-900 dark:text-white">{{ project.progress }}%</span>
              </div>
              <div class="w-full bg-gray-200 rounded-full h-2">
                <div :style="`width: ${project.progress}%`" class="bg-blue-600 h-2 rounded-full"></div>
              </div>
            </div>

            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm text-gray-500 dark:text-gray-400">Due Date</p>
                <p class="text-sm font-medium text-gray-900 dark:text-white">{{ formatDate(project.dueDate) }}</p>
              </div>
              <div class="flex space-x-2">
                <button @click="viewProject(project)" class="text-blue-600 hover:text-blue-800 text-sm">View</button>
                <button @click="editProject(project)" class="text-green-600 hover:text-green-800 text-sm">Edit</button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Activities -->
      <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <h2 class="text-xl font-bold text-gray-900 dark:text-white mb-4">Recent Project Activities</h2>
        <div class="space-y-4">
          <div v-for="activity in recentActivities" :key="activity.id" class="flex items-center space-x-4">
            <div class="flex-shrink-0">
              <div class="w-8 h-8 bg-blue-100 rounded-full flex items-center justify-center">
                <ActivityIcon class="w-4 h-4 text-blue-600" />
              </div>
            </div>
            <div class="flex-1">
              <p class="text-sm text-gray-900 dark:text-white">{{ activity.description }}</p>
              <p class="text-xs text-gray-500 dark:text-gray-400">{{ formatDateTime(activity.timestamp) }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- New Project Modal -->
    <div v-if="showNewProjectModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-96 shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white text-center">Create New Community Project</h3>
          <div class="mt-4 space-y-4">
            <input v-model="newProject.name" placeholder="Project Name" class="w-full p-2 border border-gray-300 rounded-lg dark:bg-gray-700 dark:border-gray-600 dark:text-white">
            <textarea v-model="newProject.description" placeholder="Project Description" rows="3" class="w-full p-2 border border-gray-300 rounded-lg dark:bg-gray-700 dark:border-gray-600 dark:text-white"></textarea>
            <input v-model="newProject.budget" placeholder="Budget (R)" type="number" class="w-full p-2 border border-gray-300 rounded-lg dark:bg-gray-700 dark:border-gray-600 dark:text-white">
            <input v-model="newProject.dueDate" type="date" class="w-full p-2 border border-gray-300 rounded-lg dark:bg-gray-700 dark:border-gray-600 dark:text-white">
          </div>
          <div class="items-center px-4 py-3">
            <button @click="createProject" class="px-4 py-2 bg-blue-500 text-white text-base font-medium rounded-md w-full shadow-sm hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-300">
              Create Project
            </button>
            <button @click="showNewProjectModal = false" class="mt-3 px-4 py-2 bg-gray-500 text-white text-base font-medium rounded-md w-full shadow-sm hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-gray-300">
              Cancel
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

definePageMeta({
  title: 'Community Projects - TOSS ERP',
  description: 'Manage community initiatives and collaborative projects'
})

// Icons
const PlusIcon = 'svg'
const CalendarIcon = 'svg'
const ProjectIcon = 'svg'
const CheckCircleIcon = 'svg'
const UsersIcon = 'svg'
const CurrencyDollarIcon = 'svg'
const ActivityIcon = 'svg'

// Reactive data
const showNewProjectModal = ref(false)

const stats = ref({
  activeProjects: 5,
  completedProjects: 12,
  communityMembers: 45,
  totalBudget: 150000
})

const activeProjects = ref([
  {
    id: 1,
    name: 'Spaza Shop WiFi Network',
    description: 'Install free WiFi for customers and neighboring businesses to boost foot traffic and community connectivity.',
    budget: 25000,
    participants: 8,
    progress: 75,
    status: 'In Progress',
    dueDate: new Date('2024-05-15')
  },
  {
    id: 2,
    name: 'Community Garden Initiative',
    description: 'Create a vegetable garden behind the shop to supply fresh produce and teach sustainable farming.',
    budget: 15000,
    participants: 12,
    progress: 45,
    status: 'Planning',
    dueDate: new Date('2024-06-30')
  },
  {
    id: 3,
    name: 'Youth Skills Development',
    description: 'Train local youth in retail, customer service, and basic business skills through part-time work programs.',
    budget: 35000,
    participants: 15,
    progress: 60,
    status: 'In Progress',
    dueDate: new Date('2024-07-20')
  },
  {
    id: 4,
    name: 'Mobile Money Integration',
    description: 'Set up mobile payment systems to serve customers without bank accounts and improve financial inclusion.',
    budget: 20000,
    participants: 6,
    progress: 30,
    status: 'Planning',
    dueDate: new Date('2024-05-30')
  },
  {
    id: 5,
    name: 'Community Safety Patrol',
    description: 'Organize neighborhood watch program with other business owners to improve area security.',
    budget: 8000,
    participants: 20,
    progress: 80,
    status: 'In Progress',
    dueDate: new Date('2024-04-30')
  }
])

const recentActivities = ref([
  {
    id: 1,
    description: 'WiFi router installation completed at main location',
    timestamp: new Date(Date.now() - 2 * 60 * 60 * 1000)
  },
  {
    id: 2,
    description: 'Youth training session #3 completed - 12 participants',
    timestamp: new Date(Date.now() - 6 * 60 * 60 * 1000)
  },
  {
    id: 3,
    description: 'Garden site preparation started with community volunteers',
    timestamp: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000)
  },
  {
    id: 4,
    description: 'Mobile money partnership agreement signed with local bank',
    timestamp: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000)
  },
  {
    id: 5,
    description: 'Safety patrol meeting held - 15 business owners attended',
    timestamp: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000)
  }
])

const newProject = ref({
  name: '',
  description: '',
  budget: 0,
  dueDate: ''
})

// Methods
const getStatusColor = (status) => {
  switch (status) {
    case 'In Progress':
      return 'bg-blue-100 text-blue-800'
    case 'Planning':
      return 'bg-yellow-100 text-yellow-800'
    case 'Completed':
      return 'bg-green-100 text-green-800'
    default:
      return 'bg-gray-100 text-gray-800'
  }
}

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('en-ZA')
}

const formatDateTime = (date) => {
  return new Date(date).toLocaleString('en-ZA')
}

const createProject = () => {
  if (newProject.value.name && newProject.value.description) {
    const project = {
      id: activeProjects.value.length + 1,
      ...newProject.value,
      participants: 0,
      progress: 0,
      status: 'Planning'
    }
    activeProjects.value.push(project)
    newProject.value = { name: '', description: '', budget: 0, dueDate: '' }
    showNewProjectModal.value = false
    stats.value.activeProjects++
  }
}

const viewProject = (project) => {
  console.log('View project:', project.name)
}

const editProject = (project) => {
  console.log('Edit project:', project.name)
}

const viewProjectCalendar = () => {
  console.log('View project calendar')
}

useHead({
  title: 'Community Projects - TOSS ERP',
  meta: [
    { name: 'description', content: 'Community project management and initiatives in TOSS ERP' }
  ]
})
</script>
