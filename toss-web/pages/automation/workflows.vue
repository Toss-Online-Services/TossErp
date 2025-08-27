<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="border-b border-gray-200 dark:border-gray-700 pb-4">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Automation Workflows</h1>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            Create and manage automated business processes
          </p>
        </div>
        <div class="flex space-x-3">
          <button class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600">
            <FolderIcon class="w-4 h-4 mr-2" />
            Import Template
          </button>
          <button class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-lg text-white bg-blue-600 hover:bg-blue-700">
            <PlusIcon class="w-4 h-4 mr-2" />
            Create Workflow
          </button>
        </div>
      </div>
    </div>

    <!-- Workflow Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-green-100 dark:bg-green-900 rounded-lg flex items-center justify-center">
            <CheckCircleIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Active Workflows</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.active }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center">
            <PlayIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Total Executions</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.executions.toLocaleString() }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-yellow-100 dark:bg-yellow-900 rounded-lg flex items-center justify-center">
            <ClockIcon class="w-6 h-6 text-yellow-600 dark:text-yellow-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Time Saved</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.timeSaved }}h</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-red-100 dark:bg-red-900 rounded-lg flex items-center justify-center">
            <ExclamationTriangleIcon class="w-6 h-6 text-red-600 dark:text-red-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Failed Runs</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.failed }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters and Search -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Status</label>
          <select v-model="selectedStatus" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
            <option value="">All Status</option>
            <option value="active">Active</option>
            <option value="paused">Paused</option>
            <option value="draft">Draft</option>
            <option value="disabled">Disabled</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Category</label>
          <select v-model="selectedCategory" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
            <option value="">All Categories</option>
            <option value="sales">Sales</option>
            <option value="crm">CRM</option>
            <option value="inventory">Inventory</option>
            <option value="accounting">Accounting</option>
            <option value="hr">HR</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Created By</label>
          <select v-model="selectedCreator" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
            <option value="">All Users</option>
            <option value="admin">Admin</option>
            <option value="user1">John Doe</option>
            <option value="user2">Jane Smith</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Search</label>
          <div class="relative">
            <MagnifyingGlassIcon class="w-5 h-5 absolute left-3 top-2.5 text-gray-400" />
            <input v-model="searchQuery" type="text" placeholder="Search workflows..." class="w-full pl-10 pr-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400">
          </div>
        </div>
      </div>
    </div>

    <!-- Workflows List -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white">All Workflows</h3>
      </div>
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Workflow
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Category
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Trigger
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Last Run
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Success Rate
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Status
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Actions
              </th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
            <tr v-for="workflow in filteredWorkflows" :key="workflow.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="w-8 h-8 rounded-lg flex items-center justify-center mr-3" :class="workflow.iconBg">
                    <component :is="workflow.icon" class="w-4 h-4" :class="workflow.iconColor" />
                  </div>
                  <div>
                    <div class="text-sm font-medium text-gray-900 dark:text-white">{{ workflow.name }}</div>
                    <div class="text-sm text-gray-500 dark:text-gray-400">{{ workflow.description }}</div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200">
                  {{ workflow.category }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ workflow.trigger }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900 dark:text-white">{{ workflow.lastRun }}</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">{{ workflow.lastRunTime }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="w-16 bg-gray-200 dark:bg-gray-700 rounded-full h-2 mr-2">
                    <div class="bg-green-500 h-2 rounded-full" :style="{ width: workflow.successRate + '%' }"></div>
                  </div>
                  <span class="text-sm text-gray-900 dark:text-white">{{ workflow.successRate }}%</span>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full" :class="getStatusClass(workflow.status)">
                  {{ workflow.status }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                <div class="flex space-x-2">
                  <button class="text-blue-600 dark:text-blue-400 hover:text-blue-700" title="Edit">
                    <PencilIcon class="w-4 h-4" />
                  </button>
                  <button v-if="workflow.status === 'active'" class="text-yellow-600 dark:text-yellow-400 hover:text-yellow-700" title="Pause">
                    <PauseIcon class="w-4 h-4" />
                  </button>
                  <button v-else class="text-green-600 dark:text-green-400 hover:text-green-700" title="Start">
                    <PlayIcon class="w-4 h-4" />
                  </button>
                  <button class="text-gray-600 dark:text-gray-400 hover:text-gray-700" title="View Logs">
                    <DocumentTextIcon class="w-4 h-4" />
                  </button>
                  <button class="text-red-600 dark:text-red-400 hover:text-red-700" title="Delete">
                    <TrashIcon class="w-4 h-4" />
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Quick Start Templates -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
      <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Quick Start Templates</h3>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div v-for="template in quickStartTemplates" :key="template.id" class="p-4 border border-gray-200 dark:border-gray-600 rounded-lg hover:shadow-md transition-shadow cursor-pointer">
          <div class="flex items-center mb-3">
            <div class="w-10 h-10 rounded-lg flex items-center justify-center mr-3" :class="template.iconBg">
              <component :is="template.icon" class="w-5 h-5" :class="template.iconColor" />
            </div>
            <div>
              <h4 class="font-medium text-gray-900 dark:text-white">{{ template.name }}</h4>
              <p class="text-xs text-gray-500 dark:text-gray-400">{{ template.category }}</p>
            </div>
          </div>
          <p class="text-sm text-gray-600 dark:text-gray-400 mb-3">{{ template.description }}</p>
          <div class="flex items-center justify-between">
            <span class="text-xs text-gray-500 dark:text-gray-400">{{ template.estimatedTime }}</span>
            <button class="text-xs text-blue-600 dark:text-blue-400 hover:text-blue-700 font-medium">
              Use Template
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import {
  PlusIcon,
  FolderIcon,
  CheckCircleIcon,
  PlayIcon,
  ClockIcon,
  ExclamationTriangleIcon,
  MagnifyingGlassIcon,
  PencilIcon,
  PauseIcon,
  DocumentTextIcon,
  TrashIcon,
  EnvelopeIcon,
  UserGroupIcon,
  ShoppingCartIcon,
  CurrencyDollarIcon,
  ArchiveBoxIcon,
  ChartBarIcon,
  BellIcon
} from '@heroicons/vue/24/outline'

// Reactive data
const selectedStatus = ref('')
const selectedCategory = ref('')
const selectedCreator = ref('')
const searchQuery = ref('')

// Stats
const stats = ref({
  active: 12,
  executions: 1247,
  timeSaved: 42,
  failed: 3
})

// Sample workflows data
const workflows = ref([
  {
    id: 1,
    name: 'Lead Assignment Workflow',
    description: 'Automatically assign new leads to sales representatives',
    category: 'CRM',
    trigger: 'New lead created',
    lastRun: '2 minutes ago',
    lastRunTime: '14:35',
    successRate: 98,
    status: 'active',
    icon: UserGroupIcon,
    iconBg: 'bg-blue-100 dark:bg-blue-900',
    iconColor: 'text-blue-600 dark:text-blue-400'
  },
  {
    id: 2,
    name: 'Invoice Payment Reminder',
    description: 'Send automatic payment reminders for overdue invoices',
    category: 'Accounting',
    trigger: 'Invoice overdue',
    lastRun: '1 hour ago',
    lastRunTime: '13:45',
    successRate: 95,
    status: 'active',
    icon: EnvelopeIcon,
    iconBg: 'bg-green-100 dark:bg-green-900',
    iconColor: 'text-green-600 dark:text-green-400'
  },
  {
    id: 3,
    name: 'Low Stock Alert',
    description: 'Notify when inventory levels fall below minimum',
    category: 'Inventory',
    trigger: 'Stock level threshold',
    lastRun: '3 hours ago',
    lastRunTime: '11:20',
    successRate: 92,
    status: 'paused',
    icon: ArchiveBoxIcon,
    iconBg: 'bg-orange-100 dark:bg-orange-900',
    iconColor: 'text-orange-600 dark:text-orange-400'
  },
  {
    id: 4,
    name: 'Order Processing',
    description: 'Automatically process and fulfill customer orders',
    category: 'Sales',
    trigger: 'Order confirmed',
    lastRun: '5 minutes ago',
    lastRunTime: '14:30',
    successRate: 99,
    status: 'active',
    icon: ShoppingCartIcon,
    iconBg: 'bg-purple-100 dark:bg-purple-900',
    iconColor: 'text-purple-600 dark:text-purple-400'
  },
  {
    id: 5,
    name: 'Welcome Email Campaign',
    description: 'Send welcome emails to new customers with onboarding',
    category: 'CRM',
    trigger: 'Customer registration',
    lastRun: '30 minutes ago',
    lastRunTime: '14:05',
    successRate: 97,
    status: 'active',
    icon: EnvelopeIcon,
    iconBg: 'bg-blue-100 dark:bg-blue-900',
    iconColor: 'text-blue-600 dark:text-blue-400'
  },
  {
    id: 6,
    name: 'Task Auto-Assignment',
    description: 'Distribute tasks based on team workload and skills',
    category: 'HR',
    trigger: 'New task created',
    lastRun: 'Never',
    lastRunTime: '-',
    successRate: 0,
    status: 'draft',
    icon: UserGroupIcon,
    iconBg: 'bg-gray-100 dark:bg-gray-700',
    iconColor: 'text-gray-600 dark:text-gray-400'
  }
])

const quickStartTemplates = ref([
  {
    id: 1,
    name: 'Customer Onboarding',
    category: 'CRM',
    description: 'Complete customer welcome and setup automation',
    estimatedTime: '5 mins setup',
    icon: UserGroupIcon,
    iconBg: 'bg-blue-100 dark:bg-blue-900',
    iconColor: 'text-blue-600 dark:text-blue-400'
  },
  {
    id: 2,
    name: 'Payment Processing',
    category: 'Accounting',
    description: 'Automate invoice generation and payment tracking',
    estimatedTime: '10 mins setup',
    icon: CurrencyDollarIcon,
    iconBg: 'bg-green-100 dark:bg-green-900',
    iconColor: 'text-green-600 dark:text-green-400'
  },
  {
    id: 3,
    name: 'Inventory Management',
    category: 'Inventory',
    description: 'Smart reordering and stock level monitoring',
    estimatedTime: '8 mins setup',
    icon: ArchiveBoxIcon,
    iconBg: 'bg-orange-100 dark:bg-orange-900',
    iconColor: 'text-orange-600 dark:text-orange-400'
  }
])

// Computed properties
const filteredWorkflows = computed(() => {
  let filtered = workflows.value

  if (selectedStatus.value) {
    filtered = filtered.filter(w => w.status === selectedStatus.value)
  }

  if (selectedCategory.value) {
    filtered = filtered.filter(w => w.category.toLowerCase() === selectedCategory.value)
  }

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(w => 
      w.name.toLowerCase().includes(query) ||
      w.description.toLowerCase().includes(query) ||
      w.category.toLowerCase().includes(query)
    )
  }

  return filtered
})

// Methods
const getStatusClass = (status: string) => {
  const classes = {
    'active': 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    'paused': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    'draft': 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200',
    'disabled': 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  return classes[status as keyof typeof classes] || classes.draft
}
</script>
