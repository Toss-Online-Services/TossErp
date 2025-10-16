<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="pb-4 border-b border-gray-200 dark:border-gray-700">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Automation Hub</h1>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            AI-powered automation to streamline your business processes
          </p>
        </div>
        <div class="flex space-x-3">
          <button class="inline-flex items-center px-4 py-2 text-sm font-medium text-white bg-blue-600 border border-transparent rounded-lg hover:bg-blue-700">
            <PlusIcon class="w-4 h-4 mr-2" />
            Create Workflow
          </button>
        </div>
      </div>
    </div>

    <!-- AI Assistant Quick Actions -->
    <div class="p-6 border border-blue-200 bg-gradient-to-r from-blue-50 to-purple-50 dark:from-blue-900/20 dark:to-purple-900/20 rounded-xl dark:border-blue-800">
      <div class="flex items-center mb-4">
        <div class="flex items-center justify-center w-10 h-10 mr-3 bg-blue-100 rounded-lg dark:bg-blue-900">
          <SparklesIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
        </div>
        <div>
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">AI Assistant</h3>
          <p class="text-sm text-gray-600 dark:text-gray-400">Let AI help automate your workflows</p>
        </div>
      </div>
      <div class="grid grid-cols-1 gap-4 md:grid-cols-3">
        <button class="p-4 text-left transition-shadow bg-white border border-gray-200 rounded-lg dark:bg-gray-800 dark:border-gray-700 hover:shadow-md">
          <div class="flex items-center mb-2">
            <BoltIcon class="w-5 h-5 mr-2 text-yellow-500" />
            <span class="font-medium text-gray-900 dark:text-white">Smart Triggers</span>
          </div>
          <p class="text-sm text-gray-600 dark:text-gray-400">Automatically trigger actions based on data changes</p>
        </button>
        <button class="p-4 text-left transition-shadow bg-white border border-gray-200 rounded-lg dark:bg-gray-800 dark:border-gray-700 hover:shadow-md">
          <div class="flex items-center mb-2">
            <DocumentTextIcon class="w-5 h-5 mr-2 text-green-500" />
            <span class="font-medium text-gray-900 dark:text-white">Document AI</span>
          </div>
          <p class="text-sm text-gray-600 dark:text-gray-400">Extract data from documents automatically</p>
        </button>
        <button class="p-4 text-left transition-shadow bg-white border border-gray-200 rounded-lg dark:bg-gray-800 dark:border-gray-700 hover:shadow-md">
          <div class="flex items-center mb-2">
            <ChartBarIcon class="w-5 h-5 mr-2 text-purple-500" />
            <span class="font-medium text-gray-900 dark:text-white">Predictive Analytics</span>
          </div>
          <p class="text-sm text-gray-600 dark:text-gray-400">Forecast trends and automate decisions</p>
        </button>
      </div>
    </div>

    <!-- Automation Stats -->
    <div class="grid grid-cols-1 gap-6 md:grid-cols-4">
      <div class="p-6 bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="flex items-center">
          <div class="flex items-center justify-center w-10 h-10 bg-green-100 rounded-lg dark:bg-green-900">
            <CheckCircleIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Active Workflows</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">12</p>
          </div>
        </div>
      </div>

      <div class="p-6 bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="flex items-center">
          <div class="flex items-center justify-center w-10 h-10 bg-blue-100 rounded-lg dark:bg-blue-900">
            <ClockIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Time Saved</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">42h</p>
          </div>
        </div>
      </div>

      <div class="p-6 bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="flex items-center">
          <div class="flex items-center justify-center w-10 h-10 bg-purple-100 rounded-lg dark:bg-purple-900">
            <PlayIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Executions</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">1,247</p>
          </div>
        </div>
      </div>

      <div class="p-6 bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="flex items-center">
          <div class="flex items-center justify-center w-10 h-10 bg-red-100 rounded-lg dark:bg-red-900">
            <ExclamationTriangleIcon class="w-6 h-6 text-red-600 dark:text-red-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Failed Runs</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">3</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Active Workflows -->
    <div class="grid grid-cols-1 gap-6 lg:grid-cols-2">
      <!-- Workflow List -->
      <div class="p-6 bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Active Workflows</h3>
          <NuxtLink to="/automation/workflows" class="text-sm text-blue-600 dark:text-blue-400 hover:text-blue-700">
            View all
          </NuxtLink>
        </div>
        <div class="space-y-3">
          <div v-for="workflow in activeWorkflows" :key="workflow.id" class="flex items-center justify-between p-3 rounded-lg bg-gray-50 dark:bg-gray-700">
            <div class="flex items-center">
              <div class="flex items-center justify-center w-8 h-8 mr-3 bg-blue-100 rounded-lg dark:bg-blue-900">
                <component :is="workflow.icon" class="w-4 h-4 text-blue-600 dark:text-blue-400" />
              </div>
              <div>
                <p class="text-sm font-medium text-gray-900 dark:text-white">{{ workflow.name }}</p>
                <p class="text-xs text-gray-500 dark:text-gray-400">{{ workflow.description }}</p>
              </div>
            </div>
            <div class="flex items-center space-x-2">
              <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full" :class="workflow.statusClass">
                {{ workflow.status }}
              </span>
              <button class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
                <EllipsisVerticalIcon class="w-4 h-4" />
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Activity -->
      <div class="p-6 bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Recent Activity</h3>
          <NuxtLink to="/automation/reports" class="text-sm text-blue-600 dark:text-blue-400 hover:text-blue-700">
            View reports
          </NuxtLink>
        </div>
        <div class="space-y-3">
          <div v-for="activity in recentActivity" :key="activity.id" class="flex items-start space-x-3">
            <div class="w-2 h-2 mt-2 rounded-full" :class="activity.statusColor"></div>
            <div class="flex-1 min-w-0">
              <p class="text-sm text-gray-900 dark:text-white">{{ activity.message }}</p>
              <p class="text-xs text-gray-500 dark:text-gray-400">{{ activity.time }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Automation Templates -->
    <div class="p-6 bg-white border border-gray-200 rounded-lg shadow-sm dark:bg-gray-800 dark:border-gray-700">
      <h3 class="mb-4 text-lg font-semibold text-gray-900 dark:text-white">Automation Templates</h3>
      <div class="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
        <div v-for="template in automationTemplates" :key="template.id" class="p-4 transition-shadow border border-gray-200 rounded-lg cursor-pointer dark:border-gray-600 hover:shadow-md">
          <div class="flex items-center mb-3">
            <div class="flex items-center justify-center w-10 h-10 mr-3 rounded-lg" :class="template.iconBg">
              <component :is="template.icon" class="w-5 h-5" :class="template.iconColor" />
            </div>
            <div>
              <h4 class="font-medium text-gray-900 dark:text-white">{{ template.name }}</h4>
              <p class="text-xs text-gray-500 dark:text-gray-400">{{ template.category }}</p>
            </div>
          </div>
          <p class="mb-3 text-sm text-gray-600 dark:text-gray-400">{{ template.description }}</p>
          <div class="flex items-center justify-between">
            <span class="text-xs text-gray-500 dark:text-gray-400">{{ template.uses }} uses</span>
            <button class="text-xs font-medium text-blue-600 dark:text-blue-400 hover:text-blue-700">
              Use Template
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import {
  PlusIcon,
  SparklesIcon,
  BoltIcon,
  DocumentTextIcon,
  ChartBarIcon,
  CheckCircleIcon,
  ClockIcon,
  PlayIcon,
  ExclamationTriangleIcon,
  EllipsisVerticalIcon,
  EnvelopeIcon,
  UserGroupIcon,
  ShoppingCartIcon,
  CurrencyDollarIcon,
  ArchiveBoxIcon
} from '@heroicons/vue/24/outline'

// Load mock data
const { get, useMockData } = useApi()
import { MockAutomationService } from '~/services/mock'

const activeWorkflows = ref([
  {
    id: 1,
    name: 'Lead Assignment',
    description: 'Auto-assign leads to sales reps',
    status: 'Active',
    statusClass: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    icon: UserGroupIcon
  },
  {
    id: 2,
    name: 'Invoice Reminder',
    description: 'Send payment reminders',
    status: 'Active',
    statusClass: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    icon: EnvelopeIcon
  },
  {
    id: 3,
    name: 'Stock Alerts',
    description: 'Low stock notifications',
    status: 'Paused',
    statusClass: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    icon: ArchiveBoxIcon
  },
  {
    id: 4,
    name: 'Order Processing',
    description: 'Automate order fulfillment',
    status: 'Active',
    statusClass: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    icon: ShoppingCartIcon
  }
])

// Load automation stats
const loadStats = async () => {
  try {
    const stats = MockAutomationService.getAutomationStats()
    // Stats loaded from mock service
  } catch (error) {
    console.error('Failed to load automation stats:', error)
  }
}

onMounted(() => {
  loadStats()
})

const recentActivity = ref([
  {
    id: 1,
    message: 'Lead Assignment workflow executed successfully for 5 new leads',
    time: '2 minutes ago',
    statusColor: 'bg-green-500'
  },
  {
    id: 2,
    message: 'Invoice Reminder sent to 12 customers',
    time: '15 minutes ago',
    statusColor: 'bg-blue-500'
  },
  {
    id: 3,
    message: 'Stock Alert workflow failed - check configuration',
    time: '1 hour ago',
    statusColor: 'bg-red-500'
  },
  {
    id: 4,
    message: 'Order Processing automated 23 orders today',
    time: '2 hours ago',
    statusColor: 'bg-green-500'
  }
])

const automationTemplates = ref([
  {
    id: 1,
    name: 'Welcome Email',
    category: 'CRM',
    description: 'Send welcome emails to new customers automatically',
    uses: 150,
    icon: EnvelopeIcon,
    iconBg: 'bg-blue-100 dark:bg-blue-900',
    iconColor: 'text-blue-600 dark:text-blue-400'
  },
  {
    id: 2,
    name: 'Payment Follow-up',
    category: 'Accounting',
    description: 'Automatically follow up on overdue payments',
    uses: 89,
    icon: CurrencyDollarIcon,
    iconBg: 'bg-green-100 dark:bg-green-900',
    iconColor: 'text-green-600 dark:text-green-400'
  },
  {
    id: 3,
    name: 'Reorder Alert',
    category: 'Inventory',
    description: 'Alert when items reach reorder level',
    uses: 67,
    icon: ArchiveBoxIcon,
    iconBg: 'bg-orange-100 dark:bg-orange-900',
    iconColor: 'text-orange-600 dark:text-orange-400'
  },
  {
    id: 4,
    name: 'Lead Scoring',
    category: 'Sales',
    description: 'Automatically score leads based on activities',
    uses: 45,
    icon: ChartBarIcon,
    iconBg: 'bg-purple-100 dark:bg-purple-900',
    iconColor: 'text-purple-600 dark:text-purple-400'
  },
  {
    id: 5,
    name: 'Task Assignment',
    category: 'Projects',
    description: 'Auto-assign tasks based on workload',
    uses: 32,
    icon: UserGroupIcon,
    iconBg: 'bg-indigo-100 dark:bg-indigo-900',
    iconColor: 'text-indigo-600 dark:text-indigo-400'
  },
  {
    id: 6,
    name: 'Order Approval',
    category: 'Sales',
    description: 'Automate order approval workflow',
    uses: 28,
    icon: CheckCircleIcon,
    iconBg: 'bg-green-100 dark:bg-green-900',
    iconColor: 'text-green-600 dark:text-green-400'
  }
])
</script>
