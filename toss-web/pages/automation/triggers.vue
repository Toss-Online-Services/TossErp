<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="border-b border-gray-200 dark:border-gray-700 pb-4">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Automation Triggers</h1>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            Configure events that initiate automated workflows
          </p>
        </div>
        <div class="flex space-x-3">
          <button class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600">
            <DocumentTextIcon class="w-4 h-4 mr-2" />
            View Logs
          </button>
          <button class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-lg text-white bg-blue-600 hover:bg-blue-700">
            <PlusIcon class="w-4 h-4 mr-2" />
            Create Trigger
          </button>
        </div>
      </div>
    </div>

    <!-- Trigger Categories -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
      <div v-for="category in triggerCategories" :key="category.id" 
           class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6 cursor-pointer hover:shadow-md transition-shadow"
           @click="selectedCategory = category.id">
        <div class="flex items-center">
          <div class="w-12 h-12 rounded-lg flex items-center justify-center mr-4" :class="category.iconBg">
            <component :is="category.icon" class="w-6 h-6" :class="category.iconColor" />
          </div>
          <div class="flex-1">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">{{ category.name }}</h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">{{ category.count }} triggers</p>
          </div>
        </div>
        <p class="mt-3 text-sm text-gray-600 dark:text-gray-400">{{ category.description }}</p>
      </div>
    </div>

    <!-- Trigger Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-green-100 dark:bg-green-900 rounded-lg flex items-center justify-center">
            <CheckCircleIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Active Triggers</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.activeTriggers }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center">
            <BoltIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Today's Fires</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.todayFires.toLocaleString() }}</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-purple-100 dark:bg-purple-900 rounded-lg flex items-center justify-center">
            <ClockIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Avg Response</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.avgResponse }}ms</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-red-100 dark:bg-red-900 rounded-lg flex items-center justify-center">
            <ExclamationTriangleIcon class="w-6 h-6 text-red-600 dark:text-red-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Failed Triggers</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.failedTriggers }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
      <div class="grid grid-cols-1 md:grid-cols-5 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Category</label>
          <select v-model="selectedTriggerCategory" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
            <option value="">All Categories</option>
            <option value="data">Data Events</option>
            <option value="time">Time-based</option>
            <option value="external">External Events</option>
            <option value="user">User Actions</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Status</label>
          <select v-model="selectedStatus" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
            <option value="">All Status</option>
            <option value="active">Active</option>
            <option value="paused">Paused</option>
            <option value="disabled">Disabled</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Module</label>
          <select v-model="selectedModule" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
            <option value="">All Modules</option>
            <option value="crm">CRM</option>
            <option value="sales">Sales</option>
            <option value="inventory">Inventory</option>
            <option value="accounting">Accounting</option>
            <option value="hr">HR</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Priority</label>
          <select v-model="selectedPriority" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
            <option value="">All Priorities</option>
            <option value="high">High</option>
            <option value="medium">Medium</option>
            <option value="low">Low</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Search</label>
          <div class="relative">
            <MagnifyingGlassIcon class="w-5 h-5 absolute left-3 top-2.5 text-gray-400" />
            <input v-model="searchQuery" type="text" placeholder="Search triggers..." class="w-full pl-10 pr-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400">
          </div>
        </div>
      </div>
    </div>

    <!-- Triggers List -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white">All Triggers</h3>
      </div>
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Trigger
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Event Type
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Module
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Conditions
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Last Fired
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Fire Count
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
            <tr v-for="trigger in filteredTriggers" :key="trigger.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="w-8 h-8 rounded-lg flex items-center justify-center mr-3" :class="trigger.iconBg">
                    <component :is="trigger.icon" class="w-4 h-4" :class="trigger.iconColor" />
                  </div>
                  <div>
                    <div class="text-sm font-medium text-gray-900 dark:text-white">{{ trigger.name }}</div>
                    <div class="text-sm text-gray-500 dark:text-gray-400">{{ trigger.description }}</div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full" :class="getEventTypeClass(trigger.eventType)">
                  {{ trigger.eventType }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200">
                  {{ trigger.module }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                <div class="max-w-xs truncate" :title="trigger.conditions">
                  {{ trigger.conditions }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900 dark:text-white">{{ trigger.lastFired }}</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">{{ trigger.lastFiredTime }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ trigger.fireCount.toLocaleString() }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full" :class="getStatusClass(trigger.status)">
                  {{ trigger.status }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                <div class="flex space-x-2">
                  <button class="text-blue-600 dark:text-blue-400 hover:text-blue-700" title="Edit">
                    <PencilIcon class="w-4 h-4" />
                  </button>
                  <button class="text-gray-600 dark:text-gray-400 hover:text-gray-700" title="Test Trigger">
                    <PlayIcon class="w-4 h-4" />
                  </button>
                  <button class="text-gray-600 dark:text-gray-400 hover:text-gray-700" title="View History">
                    <ClockIcon class="w-4 h-4" />
                  </button>
                  <button v-if="trigger.status === 'active'" class="text-yellow-600 dark:text-yellow-400 hover:text-yellow-700" title="Pause">
                    <PauseIcon class="w-4 h-4" />
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

    <!-- Recent Activity -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
      <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Recent Trigger Activity</h3>
      <div class="space-y-4">
        <div v-for="activity in recentActivity" :key="activity.id" class="flex items-center p-4 border border-gray-200 dark:border-gray-600 rounded-lg">
          <div class="w-10 h-10 rounded-lg flex items-center justify-center mr-4" :class="activity.iconBg">
            <component :is="activity.icon" class="w-5 h-5" :class="activity.iconColor" />
          </div>
          <div class="flex-1">
            <div class="flex items-center justify-between">
              <p class="text-sm font-medium text-gray-900 dark:text-white">{{ activity.trigger }}</p>
              <span class="text-xs text-gray-500 dark:text-gray-400">{{ activity.time }}</span>
            </div>
            <p class="text-sm text-gray-600 dark:text-gray-400">{{ activity.description }}</p>
            <div class="flex items-center mt-2 space-x-2">
              <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full" :class="getResultClass(activity.result)">
                {{ activity.result }}
              </span>
              <span class="text-xs text-gray-500 dark:text-gray-400">{{ activity.duration }}ms</span>
            </div>
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
  DocumentTextIcon,
  CheckCircleIcon,
  BoltIcon,
  ClockIcon,
  ExclamationTriangleIcon,
  MagnifyingGlassIcon,
  PencilIcon,
  PlayIcon,
  PauseIcon,
  TrashIcon,
  CircleStackIcon,
  CalendarIcon,
  GlobeAltIcon,
  UserIcon,
  EnvelopeIcon,
  ShoppingCartIcon,
  CurrencyDollarIcon,
  ArchiveBoxIcon
} from '@heroicons/vue/24/outline'

// Reactive data
const selectedCategory = ref('')
const selectedTriggerCategory = ref('')
const selectedStatus = ref('')
const selectedModule = ref('')
const selectedPriority = ref('')
const searchQuery = ref('')

// Stats
const stats = ref({
  activeTriggers: 18,
  todayFires: 247,
  avgResponse: 125,
  failedTriggers: 2
})

// Trigger categories
const triggerCategories = ref([
  {
    id: 'data',
    name: 'Data Events',
    count: 12,
    description: 'Triggers based on data changes and database events',
    icon: CircleStackIcon,
    iconBg: 'bg-blue-100 dark:bg-blue-900',
    iconColor: 'text-blue-600 dark:text-blue-400'
  },
  {
    id: 'time',
    name: 'Time-based',
    count: 8,
    description: 'Scheduled triggers and time-driven automation',
    icon: ClockIcon,
    iconBg: 'bg-green-100 dark:bg-green-900',
    iconColor: 'text-green-600 dark:text-green-400'
  },
  {
    id: 'external',
    name: 'External Events',
    count: 6,
    description: 'API calls, webhooks, and external system events',
    icon: GlobeAltIcon,
    iconBg: 'bg-purple-100 dark:bg-purple-900',
    iconColor: 'text-purple-600 dark:text-purple-400'
  },
  {
    id: 'user',
    name: 'User Actions',
    count: 15,
    description: 'User interactions and behavior-based triggers',
    icon: UserIcon,
    iconBg: 'bg-orange-100 dark:bg-orange-900',
    iconColor: 'text-orange-600 dark:text-orange-400'
  }
])

// Sample triggers data
const triggers = ref([
  {
    id: 1,
    name: 'New Customer Registration',
    description: 'Trigger when a new customer signs up',
    eventType: 'Create',
    module: 'CRM',
    conditions: 'customer.status = "new" AND customer.verified = true',
    lastFired: '5 minutes ago',
    lastFiredTime: '14:30',
    fireCount: 45,
    status: 'active',
    icon: UserIcon,
    iconBg: 'bg-blue-100 dark:bg-blue-900',
    iconColor: 'text-blue-600 dark:text-blue-400'
  },
  {
    id: 2,
    name: 'Low Stock Alert',
    description: 'Trigger when inventory falls below threshold',
    eventType: 'Update',
    module: 'Inventory',
    conditions: 'stock.quantity <= stock.minimum_threshold',
    lastFired: '2 hours ago',
    lastFiredTime: '12:35',
    fireCount: 23,
    status: 'active',
    icon: ArchiveBoxIcon,
    iconBg: 'bg-orange-100 dark:bg-orange-900',
    iconColor: 'text-orange-600 dark:text-orange-400'
  },
  {
    id: 3,
    name: 'Order Confirmation',
    description: 'Trigger when order status changes to confirmed',
    eventType: 'Update',
    module: 'Sales',
    conditions: 'order.status = "confirmed" AND order.payment_status = "paid"',
    lastFired: '10 minutes ago',
    lastFiredTime: '14:25',
    fireCount: 156,
    status: 'active',
    icon: ShoppingCartIcon,
    iconBg: 'bg-green-100 dark:bg-green-900',
    iconColor: 'text-green-600 dark:text-green-400'
  },
  {
    id: 4,
    name: 'Daily Sales Report',
    description: 'Generate daily sales summary at 6 PM',
    eventType: 'Schedule',
    module: 'Sales',
    conditions: 'time = "18:00" AND weekday != "saturday,sunday"',
    lastFired: 'Yesterday',
    lastFiredTime: '18:00',
    fireCount: 365,
    status: 'active',
    icon: CalendarIcon,
    iconBg: 'bg-purple-100 dark:bg-purple-900',
    iconColor: 'text-purple-600 dark:text-purple-400'
  },
  {
    id: 5,
    name: 'Payment Overdue',
    description: 'Trigger when invoice payment is overdue',
    eventType: 'Time',
    module: 'Accounting',
    conditions: 'invoice.due_date < NOW() AND invoice.status = "unpaid"',
    lastFired: '30 minutes ago',
    lastFiredTime: '14:05',
    fireCount: 89,
    status: 'paused',
    icon: CurrencyDollarIcon,
    iconBg: 'bg-red-100 dark:bg-red-900',
    iconColor: 'text-red-600 dark:text-red-400'
  }
])

// Recent activity
const recentActivity = ref([
  {
    id: 1,
    trigger: 'New Customer Registration',
    description: 'Customer John Doe registered and verification email sent',
    time: '2 min ago',
    result: 'Success',
    duration: 245,
    icon: UserIcon,
    iconBg: 'bg-green-100 dark:bg-green-900',
    iconColor: 'text-green-600 dark:text-green-400'
  },
  {
    id: 2,
    trigger: 'Order Confirmation',
    description: 'Order #12345 confirmed, inventory updated, invoice generated',
    time: '5 min ago',
    result: 'Success',
    duration: 156,
    icon: ShoppingCartIcon,
    iconBg: 'bg-green-100 dark:bg-green-900',
    iconColor: 'text-green-600 dark:text-green-400'
  },
  {
    id: 3,
    trigger: 'Low Stock Alert',
    description: 'Stock alert for Product ABC failed to send notification',
    time: '15 min ago',
    result: 'Failed',
    duration: 5000,
    icon: ArchiveBoxIcon,
    iconBg: 'bg-red-100 dark:bg-red-900',
    iconColor: 'text-red-600 dark:text-red-400'
  },
  {
    id: 4,
    trigger: 'Payment Reminder',
    description: 'Reminder email sent to 15 customers with overdue invoices',
    time: '1 hour ago',
    result: 'Success',
    duration: 2340,
    icon: EnvelopeIcon,
    iconBg: 'bg-green-100 dark:bg-green-900',
    iconColor: 'text-green-600 dark:text-green-400'
  }
])

// Computed properties
const filteredTriggers = computed(() => {
  let filtered = triggers.value

  if (selectedTriggerCategory.value) {
    // This would filter based on the category in a real implementation
  }

  if (selectedStatus.value) {
    filtered = filtered.filter(t => t.status === selectedStatus.value)
  }

  if (selectedModule.value) {
    filtered = filtered.filter(t => t.module.toLowerCase() === selectedModule.value)
  }

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(t => 
      t.name.toLowerCase().includes(query) ||
      t.description.toLowerCase().includes(query) ||
      t.conditions.toLowerCase().includes(query)
    )
  }

  return filtered
})

// Methods
const getStatusClass = (status: string) => {
  const classes = {
    'active': 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    'paused': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    'disabled': 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  return classes[status as keyof typeof classes] || classes.disabled
}

const getEventTypeClass = (eventType: string) => {
  const classes = {
    'Create': 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    'Update': 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    'Delete': 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    'Schedule': 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200',
    'Time': 'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-200'
  }
  return classes[eventType as keyof typeof classes] || 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-200'
}

const getResultClass = (result: string) => {
  const classes = {
    'Success': 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    'Failed': 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    'Warning': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200'
  }
  return classes[result as keyof typeof classes] || classes.Failed
}
</script>
