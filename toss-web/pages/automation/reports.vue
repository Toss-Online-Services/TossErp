<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="border-b border-gray-200 dark:border-gray-700 pb-4">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Automation Reports</h1>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            Analytics and insights for your automation workflows
          </p>
        </div>
        <div class="flex space-x-3">
          <button class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600">
            <CalendarIcon class="w-4 h-4 mr-2" />
            Schedule Report
          </button>
          <button class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-lg text-white bg-blue-600 hover:bg-blue-700">
            <DocumentChartBarIcon class="w-4 h-4 mr-2" />
            Generate Report
          </button>
        </div>
      </div>
    </div>

    <!-- Report Categories -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
      <div v-for="category in reportCategories" :key="category.id" 
           class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6 cursor-pointer hover:shadow-md transition-shadow"
           @click="selectedCategory = category.id">
        <div class="flex items-center">
          <div class="w-12 h-12 rounded-lg flex items-center justify-center mr-4" :class="category.iconBg">
            <component :is="category.icon" class="w-6 h-6" :class="category.iconColor" />
          </div>
          <div class="flex-1">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">{{ category.name }}</h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">{{ category.count }} reports</p>
          </div>
        </div>
        <p class="mt-3 text-sm text-gray-600 dark:text-gray-400">{{ category.description }}</p>
      </div>
    </div>

    <!-- Key Metrics Dashboard -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-green-100 dark:bg-green-900 rounded-lg flex items-center justify-center">
            <CheckCircleIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Success Rate</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ metrics.successRate }}%</p>
            <p class="text-xs text-green-600 dark:text-green-400">+2.5% from last month</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center">
            <ClockIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Time Saved</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ metrics.timeSaved }}h</p>
            <p class="text-xs text-blue-600 dark:text-blue-400">+15 hours this week</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-purple-100 dark:bg-purple-900 rounded-lg flex items-center justify-center">
            <CurrencyDollarIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Cost Savings</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">${{ metrics.costSavings.toLocaleString() }}</p>
            <p class="text-xs text-purple-600 dark:text-purple-400">+12% this quarter</p>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-orange-100 dark:bg-orange-900 rounded-lg flex items-center justify-center">
            <ChartBarIcon class="w-6 h-6 text-orange-600 dark:text-orange-400" />
          </div>
          <div class="ml-4">
            <p class="text-sm font-medium text-gray-500 dark:text-gray-400">Active Workflows</p>
            <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ metrics.activeWorkflows }}</p>
            <p class="text-xs text-orange-600 dark:text-orange-400">3 new this month</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Charts and Analytics -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Performance Chart -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Workflow Performance</h3>
          <select class="px-3 py-1 text-sm border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
            <option>Last 7 days</option>
            <option>Last 30 days</option>
            <option>Last 90 days</option>
          </select>
        </div>
        <div class="h-64 flex items-center justify-center border-2 border-dashed border-gray-200 dark:border-gray-600 rounded-lg">
          <div class="text-center">
            <ChartBarIcon class="w-12 h-12 text-gray-400 mx-auto mb-2" />
            <p class="text-gray-500 dark:text-gray-400">Performance chart would be displayed here</p>
            <p class="text-sm text-gray-400 dark:text-gray-500">Integration with charting library needed</p>
          </div>
        </div>
      </div>

      <!-- Execution Timeline -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Recent Executions</h3>
        <div class="space-y-4">
          <div v-for="execution in recentExecutions" :key="execution.id" class="flex items-center p-3 border border-gray-200 dark:border-gray-600 rounded-lg">
            <div class="w-8 h-8 rounded-lg flex items-center justify-center mr-3" :class="execution.statusBg">
              <component :is="execution.statusIcon" class="w-4 h-4" :class="execution.statusColor" />
            </div>
            <div class="flex-1">
              <div class="flex items-center justify-between">
                <p class="text-sm font-medium text-gray-900 dark:text-white">{{ execution.workflow }}</p>
                <span class="text-xs text-gray-500 dark:text-gray-400">{{ execution.time }}</span>
              </div>
              <p class="text-sm text-gray-600 dark:text-gray-400">{{ execution.description }}</p>
              <div class="flex items-center mt-1 text-xs text-gray-500 dark:text-gray-400">
                <span>Duration: {{ execution.duration }}ms</span>
                <span class="mx-2">•</span>
                <span>Trigger: {{ execution.trigger }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Report Filters and Export -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
      <div class="grid grid-cols-1 md:grid-cols-6 gap-4">
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Date Range</label>
          <select v-model="selectedDateRange" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
            <option value="7d">Last 7 days</option>
            <option value="30d">Last 30 days</option>
            <option value="90d">Last 90 days</option>
            <option value="custom">Custom range</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Status</label>
          <select v-model="selectedStatus" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
            <option value="">All Status</option>
            <option value="success">Success</option>
            <option value="failed">Failed</option>
            <option value="pending">Pending</option>
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Workflow</label>
          <select v-model="selectedWorkflow" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
            <option value="">All Workflows</option>
            <option value="lead-assignment">Lead Assignment</option>
            <option value="invoice-processing">Invoice Processing</option>
            <option value="order-fulfillment">Order Fulfillment</option>
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
          </select>
        </div>
        <div>
          <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Export Format</label>
          <select v-model="selectedFormat" class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
            <option value="pdf">PDF</option>
            <option value="excel">Excel</option>
            <option value="csv">CSV</option>
            <option value="json">JSON</option>
          </select>
        </div>
        <div class="flex items-end">
          <button class="w-full px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
            Export Report
          </button>
        </div>
      </div>
    </div>

    <!-- Detailed Reports Table -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
      <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Detailed Execution Reports</h3>
      </div>
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Workflow
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Executed
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Status
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Duration
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Trigger
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Result
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                Actions
              </th>
            </tr>
          </thead>
          <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
            <tr v-for="report in detailedReports" :key="report.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="w-8 h-8 rounded-lg flex items-center justify-center mr-3" :class="report.iconBg">
                    <component :is="report.icon" class="w-4 h-4" :class="report.iconColor" />
                  </div>
                  <div>
                    <div class="text-sm font-medium text-gray-900 dark:text-white">{{ report.workflow }}</div>
                    <div class="text-sm text-gray-500 dark:text-gray-400">{{ report.module }}</div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900 dark:text-white">{{ report.executedAt }}</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">{{ report.executedTime }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full" :class="getStatusClass(report.status)">
                  {{ report.status }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ report.duration }}ms
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                {{ report.trigger }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900 dark:text-white">{{ report.result }}</div>
                <div class="text-sm text-gray-500 dark:text-gray-400">{{ report.resultDetails }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                <div class="flex space-x-2">
                  <button class="text-blue-600 dark:text-blue-400 hover:text-blue-700" title="View Details">
                    <EyeIcon class="w-4 h-4" />
                  </button>
                  <button class="text-gray-600 dark:text-gray-400 hover:text-gray-700" title="Download Log">
                    <ArrowDownTrayIcon class="w-4 h-4" />
                  </button>
                  <button class="text-green-600 dark:text-green-400 hover:text-green-700" title="Replay">
                    <ArrowPathIcon class="w-4 h-4" />
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Scheduled Reports -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
      <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Scheduled Reports</h3>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div v-for="scheduled in scheduledReports" :key="scheduled.id" class="p-4 border border-gray-200 dark:border-gray-600 rounded-lg">
          <div class="flex items-center justify-between mb-3">
            <h4 class="font-medium text-gray-900 dark:text-white">{{ scheduled.name }}</h4>
            <span class="text-xs px-2 py-1 rounded-full" :class="getFrequencyClass(scheduled.frequency)">
              {{ scheduled.frequency }}
            </span>
          </div>
          <p class="text-sm text-gray-600 dark:text-gray-400 mb-3">{{ scheduled.description }}</p>
          <div class="flex items-center justify-between text-xs text-gray-500 dark:text-gray-400">
            <span>Next: {{ scheduled.nextRun }}</span>
            <span>Recipients: {{ scheduled.recipients }}</span>
          </div>
          <div class="mt-3 flex space-x-2">
            <button class="flex-1 px-3 py-1 text-xs bg-blue-600 text-white rounded hover:bg-blue-700">
              Edit
            </button>
            <button class="px-3 py-1 text-xs border border-gray-300 dark:border-gray-600 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-50 dark:hover:bg-gray-700">
              Pause
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
  CalendarIcon,
  DocumentChartBarIcon,
  CheckCircleIcon,
  ClockIcon,
  CurrencyDollarIcon,
  ChartBarIcon,
  EyeIcon,
  ArrowDownTrayIcon,
  ArrowPathIcon,
  XCircleIcon,
  ExclamationTriangleIcon,
  UserGroupIcon,
  EnvelopeIcon,
  ShoppingCartIcon,
  ArchiveBoxIcon
} from '@heroicons/vue/24/outline'

// Reactive data
const selectedCategory = ref('')
const selectedDateRange = ref('7d')
const selectedStatus = ref('')
const selectedWorkflow = ref('')
const selectedModule = ref('')
const selectedFormat = ref('pdf')

// Metrics
const metrics = ref({
  successRate: 96.8,
  timeSaved: 127,
  costSavings: 45800,
  activeWorkflows: 18
})

// Report categories
const reportCategories = ref([
  {
    id: 'performance',
    name: 'Performance',
    count: 8,
    description: 'Workflow execution and efficiency metrics',
    icon: ChartBarIcon,
    iconBg: 'bg-blue-100 dark:bg-blue-900',
    iconColor: 'text-blue-600 dark:text-blue-400'
  },
  {
    id: 'usage',
    name: 'Usage',
    count: 6,
    description: 'Automation adoption and utilization reports',
    icon: UserGroupIcon,
    iconBg: 'bg-green-100 dark:bg-green-900',
    iconColor: 'text-green-600 dark:text-green-400'
  },
  {
    id: 'financial',
    name: 'Financial',
    count: 4,
    description: 'Cost savings and ROI analysis',
    icon: CurrencyDollarIcon,
    iconBg: 'bg-purple-100 dark:bg-purple-900',
    iconColor: 'text-purple-600 dark:text-purple-400'
  },
  {
    id: 'operational',
    name: 'Operational',
    count: 5,
    description: 'Process improvement and optimization insights',
    icon: ClockIcon,
    iconBg: 'bg-orange-100 dark:bg-orange-900',
    iconColor: 'text-orange-600 dark:text-orange-400'
  }
])

// Recent executions
const recentExecutions = ref([
  {
    id: 1,
    workflow: 'Lead Assignment',
    description: 'Assigned 5 new leads to sales team',
    time: '2 min ago',
    duration: 245,
    trigger: 'New lead created',
    statusIcon: CheckCircleIcon,
    statusBg: 'bg-green-100 dark:bg-green-900',
    statusColor: 'text-green-600 dark:text-green-400'
  },
  {
    id: 2,
    workflow: 'Invoice Processing',
    description: 'Processed 12 invoices automatically',
    time: '15 min ago',
    duration: 1840,
    trigger: 'Scheduled',
    statusIcon: CheckCircleIcon,
    statusBg: 'bg-green-100 dark:bg-green-900',
    statusColor: 'text-green-600 dark:text-green-400'
  },
  {
    id: 3,
    workflow: 'Stock Alert',
    description: 'Failed to send low stock notification',
    time: '1 hour ago',
    duration: 5000,
    trigger: 'Stock threshold',
    statusIcon: XCircleIcon,
    statusBg: 'bg-red-100 dark:bg-red-900',
    statusColor: 'text-red-600 dark:text-red-400'
  }
])

// Detailed reports
const detailedReports = ref([
  {
    id: 1,
    workflow: 'Customer Onboarding',
    module: 'CRM',
    executedAt: 'Today',
    executedTime: '14:30',
    status: 'Success',
    duration: 2340,
    trigger: 'Customer registration',
    result: 'Completed',
    resultDetails: '15 new customers onboarded',
    icon: UserGroupIcon,
    iconBg: 'bg-blue-100 dark:bg-blue-900',
    iconColor: 'text-blue-600 dark:text-blue-400'
  },
  {
    id: 2,
    workflow: 'Order Processing',
    module: 'Sales',
    executedAt: 'Today',
    executedTime: '14:15',
    status: 'Success',
    duration: 156,
    trigger: 'Order confirmed',
    result: 'Processed',
    resultDetails: '23 orders processed',
    icon: ShoppingCartIcon,
    iconBg: 'bg-green-100 dark:bg-green-900',
    iconColor: 'text-green-600 dark:text-green-400'
  },
  {
    id: 3,
    workflow: 'Email Campaign',
    module: 'Marketing',
    executedAt: 'Today',
    executedTime: '13:45',
    status: 'Failed',
    duration: 8900,
    trigger: 'Scheduled',
    result: 'Error',
    resultDetails: 'SMTP connection failed',
    icon: EnvelopeIcon,
    iconBg: 'bg-red-100 dark:bg-red-900',
    iconColor: 'text-red-600 dark:text-red-400'
  },
  {
    id: 4,
    workflow: 'Inventory Update',
    module: 'Inventory',
    executedAt: 'Today',
    executedTime: '12:30',
    status: 'Warning',
    duration: 4560,
    trigger: 'Stock movement',
    result: 'Partial',
    resultDetails: '2 items failed to update',
    icon: ArchiveBoxIcon,
    iconBg: 'bg-yellow-100 dark:bg-yellow-900',
    iconColor: 'text-yellow-600 dark:text-yellow-400'
  }
])

// Scheduled reports
const scheduledReports = ref([
  {
    id: 1,
    name: 'Weekly Performance Summary',
    description: 'Comprehensive automation performance metrics',
    frequency: 'Weekly',
    nextRun: 'Monday 9:00 AM',
    recipients: '5 users'
  },
  {
    id: 2,
    name: 'Monthly ROI Analysis',
    description: 'Cost savings and return on investment analysis',
    frequency: 'Monthly',
    nextRun: '1st of next month',
    recipients: '3 users'
  },
  {
    id: 3,
    name: 'Daily Execution Log',
    description: 'All workflow executions and their outcomes',
    frequency: 'Daily',
    nextRun: 'Tomorrow 6:00 AM',
    recipients: '8 users'
  }
])

// Methods
const getStatusClass = (status: string) => {
  const classes = {
    'Success': 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    'Failed': 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    'Warning': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    'Pending': 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200'
  }
  return classes[status as keyof typeof classes] || classes.Pending
}

const getFrequencyClass = (frequency: string) => {
  const classes = {
    'Daily': 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    'Weekly': 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    'Monthly': 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200'
  }
  return classes[frequency as keyof typeof classes] || classes.Daily
}
</script>
