<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Customer Relationship Management</h1>
              <p class="text-gray-600 dark:text-gray-400">Manage customers, leads, and sales opportunities</p>
            </div>
            <div class="flex space-x-3">
              <button @click="showCreateLeadModal = true" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors">
                <PlusIcon class="w-5 h-5 inline mr-2" />
                New Lead
              </button>
              <button @click="showCreateCustomerModal = true" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors">
                <UserPlusIcon class="w-5 h-5 inline mr-2" />
                New Customer
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Total Customers</p>
              <p class="text-2xl font-bold text-blue-600">{{ stats.totalCustomers }}</p>
            </div>
            <div class="p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
              <UsersIcon class="w-6 h-6 text-blue-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Active Leads</p>
              <p class="text-2xl font-bold text-yellow-600">{{ stats.activeLeads }}</p>
            </div>
            <div class="p-3 bg-yellow-100 dark:bg-yellow-900 rounded-full">
              <ChartBarIcon class="w-6 h-6 text-yellow-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Conversion Rate</p>
              <p class="text-2xl font-bold text-green-600">{{ stats.conversionRate }}%</p>
            </div>
            <div class="p-3 bg-green-100 dark:bg-green-900 rounded-full">
              <TrendingUpIcon class="w-6 h-6 text-green-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Pipeline Value</p>
              <p class="text-2xl font-bold text-purple-600">R {{ formatCurrency(stats.pipelineValue) }}</p>
            </div>
            <div class="p-3 bg-purple-100 dark:bg-purple-900 rounded-full">
              <CurrencyDollarIcon class="w-6 h-6 text-purple-600" />
            </div>
          </div>
        </div>
      </div>

      <!-- Main Content Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
        <!-- Sales Pipeline -->
        <div class="lg:col-span-2">
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
            <div class="p-6 border-b border-gray-200 dark:border-gray-700">
              <div class="flex items-center justify-between">
                <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Sales Pipeline</h3>
                <select class="text-sm border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-1 bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
                  <option>All Stages</option>
                  <option>Prospecting</option>
                  <option>Qualification</option>
                  <option>Proposal</option>
                  <option>Negotiation</option>
                </select>
              </div>
            </div>
            <div class="p-6">
              <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
                <div v-for="stage in pipelineStages" :key="stage.name" class="bg-gray-50 dark:bg-gray-700 rounded-lg p-4">
                  <div class="flex items-center justify-between mb-3">
                    <h4 class="font-medium text-gray-900 dark:text-white">{{ stage.name }}</h4>
                    <span class="text-xs text-gray-500 dark:text-gray-400">{{ stage.count }}</span>
                  </div>
                  <div class="space-y-2">
                    <div v-for="opportunity in stage.opportunities" :key="opportunity.id" class="bg-white dark:bg-gray-800 p-3 rounded border border-gray-200 dark:border-gray-600 cursor-pointer hover:shadow-md transition-shadow">
                      <p class="font-medium text-sm text-gray-900 dark:text-white">{{ opportunity.customer }}</p>
                      <p class="text-xs text-gray-600 dark:text-gray-400">{{ opportunity.product }}</p>
                      <p class="text-sm font-semibold text-green-600 mt-1">R {{ formatCurrency(opportunity.value) }}</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Recent Activities -->
        <div>
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
            <div class="p-6 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Recent Activities</h3>
            </div>
            <div class="p-6">
              <div class="space-y-4">
                <div v-for="activity in recentActivities" :key="activity.id" class="flex items-start space-x-3">
                  <div class="w-8 h-8 rounded-full flex items-center justify-center" :class="getActivityColor(activity.type)">
                    <component :is="getActivityIcon(activity.type)" class="w-4 h-4 text-white" />
                  </div>
                  <div class="flex-1">
                    <p class="text-sm text-gray-900 dark:text-white">{{ activity.description }}</p>
                    <p class="text-xs text-gray-600 dark:text-gray-400">{{ formatDate(activity.date) }}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Customer List -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="p-6 border-b border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Recent Customers</h3>
            <div class="flex space-x-2">
              <input type="text" placeholder="Search customers..." class="px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm">
              <button class="bg-gray-100 dark:bg-gray-700 text-gray-600 dark:text-gray-400 px-3 py-2 rounded-lg hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors">
                <FunnelIcon class="w-4 h-4" />
              </button>
            </div>
          </div>
        </div>
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Customer</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Company</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Status</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Total Sales</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Last Contact</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
              <tr v-for="customer in customers" :key="customer.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center">
                      <span class="text-blue-600 font-medium text-sm">{{ getInitials(customer.name) }}</span>
                    </div>
                    <div class="ml-4">
                      <div class="text-sm font-medium text-gray-900 dark:text-white">{{ customer.name }}</div>
                      <div class="text-sm text-gray-500 dark:text-gray-400">{{ customer.email }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">{{ customer.company }}</td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full" :class="getStatusColor(customer.status)">
                    {{ customer.status }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-white">
                  R {{ formatCurrency(customer.totalSales) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
                  {{ formatDate(customer.lastContact) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <div class="flex space-x-2">
                    <button class="text-blue-600 hover:text-blue-900">View</button>
                    <button class="text-green-600 hover:text-green-900">Contact</button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Icons (would normally import from a proper icon library)
const PlusIcon = 'svg'
const UserPlusIcon = 'svg'
const UsersIcon = 'svg'
const ChartBarIcon = 'svg'
const TrendingUpIcon = 'svg'
const CurrencyDollarIcon = 'svg'
const FunnelIcon = 'svg'

// Sample data
const stats = ref({
  totalCustomers: 1247,
  activeLeads: 89,
  conversionRate: 24.5,
  pipelineValue: 450000
})

const pipelineStages = ref([
  {
    name: 'Prospecting',
    count: 12,
    opportunities: [
      { id: 1, customer: 'ABC Corp', product: 'ERP System', value: 25000 },
      { id: 2, customer: 'XYZ Ltd', product: 'Inventory Management', value: 15000 }
    ]
  },
  {
    name: 'Qualification',
    count: 8,
    opportunities: [
      { id: 3, customer: 'Tech Solutions', product: 'CRM Integration', value: 35000 },
      { id: 4, customer: 'Local Store', product: 'POS System', value: 8000 }
    ]
  },
  {
    name: 'Proposal',
    count: 5,
    opportunities: [
      { id: 5, customer: 'Manufacturing Co', product: 'Full ERP', value: 120000 }
    ]
  },
  {
    name: 'Negotiation',
    count: 3,
    opportunities: [
      { id: 6, customer: 'Retail Chain', product: 'Multi-store System', value: 85000 }
    ]
  }
])

const recentActivities = ref([
  { id: 1, type: 'call', description: 'Called ABC Corp about proposal follow-up', date: new Date() },
  { id: 2, type: 'email', description: 'Sent quote to XYZ Ltd', date: new Date(Date.now() - 3600000) },
  { id: 3, type: 'meeting', description: 'Demo scheduled with Tech Solutions', date: new Date(Date.now() - 7200000) },
  { id: 4, type: 'note', description: 'Updated lead status for Local Store', date: new Date(Date.now() - 86400000) }
])

const customers = ref([
  {
    id: 1,
    name: 'John Smith',
    email: 'john@abccorp.co.za',
    company: 'ABC Corp',
    status: 'Active',
    totalSales: 125000,
    lastContact: new Date(Date.now() - 86400000)
  },
  {
    id: 2,
    name: 'Sarah Johnson',
    email: 'sarah@xyzltd.co.za',
    company: 'XYZ Ltd',
    status: 'Lead',
    totalSales: 0,
    lastContact: new Date(Date.now() - 172800000)
  },
  {
    id: 3,
    name: 'Mike Wilson',
    email: 'mike@techsol.co.za',
    company: 'Tech Solutions',
    status: 'Prospect',
    totalSales: 45000,
    lastContact: new Date(Date.now() - 259200000)
  }
])

const showCreateLeadModal = ref(false)
const showCreateCustomerModal = ref(false)

// Helper functions
function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('en-ZA', {
    style: 'decimal',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

function formatDate(date: Date): string {
  return new Intl.DateTimeFormat('en-ZA', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date)
}

function getInitials(name: string): string {
  return name.split(' ').map(n => n[0]).join('').toUpperCase()
}

function getActivityColor(type: string): string {
  switch (type) {
    case 'call': return 'bg-blue-500'
    case 'email': return 'bg-green-500'
    case 'meeting': return 'bg-purple-500'
    case 'note': return 'bg-yellow-500'
    default: return 'bg-gray-500'
  }
}

function getActivityIcon(type: string) {
  switch (type) {
    case 'call': return 'svg'
    case 'email': return 'svg'
    case 'meeting': return 'svg'
    case 'note': return 'svg'
    default: return 'svg'
  }
}

function getStatusColor(status: string): string {
  switch (status) {
    case 'Active': return 'bg-green-100 text-green-800'
    case 'Lead': return 'bg-yellow-100 text-yellow-800'
    case 'Prospect': return 'bg-blue-100 text-blue-800'
    default: return 'bg-gray-100 text-gray-800'
  }
}

// Page metadata
useHead({
  title: 'CRM - TOSS ERP',
  meta: [
    { name: 'description', content: 'Customer relationship management and sales pipeline in TOSS ERP' }
  ]
})
</script>
