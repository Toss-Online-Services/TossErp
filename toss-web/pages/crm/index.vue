<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-slate-800 shadow-sm border-b border-slate-200 dark:border-slate-700">
      <div class="max-w-7xl mx-auto px-6 py-6">
        <div class="flex items-center justify-between">
          <div>
            <h1 class="text-2xl font-bold text-slate-900 dark:text-white">Customer Relationship Management</h1>
            <p class="text-slate-600 dark:text-slate-400 mt-1">Manage customers, leads, and sales opportunities</p>
          </div>
          <div class="flex space-x-3">
            <button @click="showCreateLeadModal = true" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center">
              <PlusIcon class="w-5 h-5 mr-2" />
              New Lead
            </button>
            <button @click="showCreateCustomerModal = true" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center">
              <UserPlusIcon class="w-5 h-5 mr-2" />
              New Customer
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="pending" class="flex justify-center items-center h-64">
      <div class="animate-spin rounded-full h-32 w-32 border-b-2 border-blue-600"></div>
    </div>

    <!-- Main Content -->
    <div v-else class="max-w-7xl mx-auto px-6 py-6">
      <!-- Stats Cards -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-slate-800 p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-slate-600 dark:text-slate-400">Total Customers</p>
              <p class="text-2xl font-bold text-blue-600">{{ analytics?.totalCustomers || 0 }}</p>
            </div>
            <div class="p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
              <UsersIcon class="w-6 h-6 text-blue-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-slate-600 dark:text-slate-400">Active Leads</p>
              <p class="text-2xl font-bold text-yellow-600">{{ analytics?.activeLeads || 0 }}</p>
            </div>
            <div class="p-3 bg-yellow-100 dark:bg-yellow-900 rounded-full">
              <ChartBarIcon class="w-6 h-6 text-yellow-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-slate-600 dark:text-slate-400">Conversion Rate</p>
              <p class="text-2xl font-bold text-green-600">{{ analytics?.conversionRate || 0 }}%</p>
            </div>
            <div class="p-3 bg-green-100 dark:bg-green-900 rounded-full">
              <ArrowTrendingUpIcon class="w-6 h-6 text-green-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-slate-600 dark:text-slate-400">Pipeline Value</p>
              <p class="text-2xl font-bold text-purple-600">R {{ formatCurrency(analytics?.pipelineValue || 0) }}</p>
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
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Customers</h3>
            <div class="flex space-x-2">
              <input 
                v-model="searchTerm" 
                type="text" 
                placeholder="Search customers..." 
                class="px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm"
                @input="debouncedSearch"
              >
              <button @click="refreshCustomers" class="bg-gray-100 dark:bg-gray-700 text-gray-600 dark:text-gray-400 px-3 py-2 rounded-lg hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors">
                <ArrowPathIcon class="w-4 h-4" />
              </button>
            </div>
          </div>
        </div>
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Customer</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Segment</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Status</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Total Spent</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Last Purchase</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
              <tr v-for="customer in filteredCustomers" :key="customer.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center">
                      <span class="text-blue-600 font-medium text-sm">{{ getInitials(customer.fullName) }}</span>
                    </div>
                    <div class="ml-4">
                      <div class="text-sm font-medium text-gray-900 dark:text-white">{{ customer.fullName }}</div>
                      <div class="text-sm text-gray-500 dark:text-gray-400">{{ customer.email }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full" :class="getSegmentColor(customer.segment || 'Regular')">
                    {{ customer.segment || 'Regular' }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full" :class="getStatusColor(customer.status)">
                    {{ customer.status }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-white">
                  R {{ formatCurrency(customer.totalSpent) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 dark:text-gray-400">
                  {{ customer.lastPurchaseDate ? formatDate(new Date(customer.lastPurchaseDate)) : 'Never' }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <div class="flex space-x-2">
                    <button @click="viewCustomer(customer)" class="text-blue-600 hover:text-blue-900">View</button>
                    <button @click="contactCustomer(customer)" class="text-green-600 hover:text-green-900">Contact</button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Create Customer Modal -->
    <div v-if="showCreateCustomerModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-gray-800 rounded-lg p-6 w-full max-w-md">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Create New Customer</h3>
        <form @submit.prevent="createCustomer">
          <div class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">First Name</label>
              <input v-model="newCustomer.firstName" type="text" required class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-2 bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Last Name</label>
              <input v-model="newCustomer.lastName" type="text" required class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-2 bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Email</label>
              <input v-model="newCustomer.email" type="email" required class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-2 bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Phone</label>
              <input v-model="newCustomer.phone" type="tel" required class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-2 bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Address</label>
              <textarea v-model="newCustomer.address" required class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-2 bg-white dark:bg-gray-700 text-gray-900 dark:text-white"></textarea>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300">Date of Birth</label>
              <input v-model="newCustomer.dateOfBirth" type="date" required class="mt-1 block w-full border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-2 bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
            </div>
          </div>
          <div class="flex justify-end space-x-3 mt-6">
            <button type="button" @click="showCreateCustomerModal = false" class="px-4 py-2 border border-gray-300 rounded-lg text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700">
              Cancel
            </button>
            <button type="submit" :disabled="creatingCustomer" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50">
              {{ creatingCustomer ? 'Creating...' : 'Create Customer' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { 
  PlusIcon, 
  UserPlusIcon, 
  UsersIcon, 
  ChartBarIcon, 
  ArrowTrendingUpIcon, 
  CurrencyDollarIcon, 
  ArrowPathIcon,
  PhoneIcon,
  EnvelopeIcon,
  CalendarIcon,
  DocumentIcon
} from '@heroicons/vue/24/outline'
import { useCustomerStore } from '../../stores/customers'
import { storeToRefs } from 'pinia'

// Initialize store
const customersStore = useCustomerStore()

// Reactive data
const searchTerm = ref('')
const showCreateLeadModal = ref(false)
const showCreateCustomerModal = ref(false)
const creatingCustomer = ref(false)
const loading = ref(false)

const newCustomer = ref({
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  address: '',
  dateOfBirth: ''
})

// Load data on mount
onMounted(async () => {
  await customersStore.fetchCustomers()
})

// Computed properties from store
const customers = computed(() => customersStore.customers)
const customerStats = computed(() => customersStore.customerStats)
const pending = computed(() => customersStore.loading)

// Mock analytics data based on customer store
const analytics = computed(() => ({
  totalCustomers: customerStats.value.total,
  activeLeads: customerStats.value.leads,
  conversionRate: Math.round((customerStats.value.active / customerStats.value.total) * 100) || 0,
  pipelineValue: customerStats.value.total * 15000 // Mock pipeline value
}))

// Mock pipeline data
const pipelineStages = computed(() => [
  {
    name: 'Prospecting',
    count: 2,
    opportunities: [
      { id: 1, customer: 'TechCorp Ltd', product: 'ERP Package', value: 45000 },
      { id: 2, customer: 'NewCo Inc', product: 'CRM Solution', value: 25000 }
    ]
  },
  {
    name: 'Qualification',
    count: 1,
    opportunities: [
      { id: 3, customer: 'StartupCo', product: 'Basic Package', value: 15000 }
    ]
  },
  {
    name: 'Proposal',
    count: 1,
    opportunities: [
      { id: 4, customer: 'BigCorp', product: 'Enterprise Suite', value: 85000 }
    ]
  },
  {
    name: 'Negotiation',
    count: 1,
    opportunities: [
      { id: 5, customer: 'MegaCorp', product: 'Full Integration', value: 120000 }
    ]
  }
])

const recentActivities = computed(() => [
  { id: 1, type: 'call', description: 'New lead: TechCorp Ltd interested in ERP solution', date: new Date() },
  { id: 2, type: 'email', description: 'Follow-up sent to BigCorp for proposal review', date: new Date(Date.now() - 3600000) },
  { id: 3, type: 'meeting', description: 'Demo scheduled with StartupCo next week', date: new Date(Date.now() - 7200000) },
  { id: 4, type: 'note', description: 'Updated contact info for John Doe at Acme Corp', date: new Date(Date.now() - 86400000) }
])

const filteredCustomers = computed(() => {
  if (!searchTerm.value) return customers.value
  return customersStore.searchCustomers(searchTerm.value)
})

// Methods
const debouncedSearch = debounce(() => {
  // Search is reactive through computed property
}, 300)

async function createCustomer() {
  creatingCustomer.value = true
  try {
    const customerData = {
      ...newCustomer.value,
      fullName: `${newCustomer.value.firstName} ${newCustomer.value.lastName}`,
      status: 'Lead' as const,
      company: '',
      totalSpent: 0,
      lastPurchaseDate: null,
      notes: '',
      subscriptions: []
    }
    
    await customersStore.createCustomer(customerData)
    
    // Reset form and close modal
    newCustomer.value = {
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      address: '',
      dateOfBirth: ''
    }
    showCreateCustomerModal.value = false
    
    // Show success message
    alert('Customer created successfully!')
  } catch (error) {
    console.error('Error creating customer:', error)
    alert('Failed to create customer. Please try again.')
  } finally {
    creatingCustomer.value = false
  }
}

async function refreshCustomers() {
  await customersStore.fetchCustomers()
}

function viewCustomer(customer: any) {
  console.log('Viewing customer:', customer)
  alert(`Viewing ${customer.fullName}...`)
}

function contactCustomer(customer: any) {
  alert(`Contacting ${customer.fullName}...`)
}

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
    case 'call': return PhoneIcon
    case 'email': return EnvelopeIcon
    case 'meeting': return CalendarIcon
    case 'note': return DocumentIcon
    default: return DocumentIcon
  }
}

function getStatusColor(status: string): string {
  switch (status) {
    case 'Active': return 'bg-green-100 text-green-800'
    case 'Lead': return 'bg-yellow-100 text-yellow-800'
    case 'Prospect': return 'bg-blue-100 text-blue-800'
    case 'Inactive': return 'bg-gray-100 text-gray-800'
    default: return 'bg-gray-100 text-gray-800'
  }
}

function getSegmentColor(segment: string): string {
  switch (segment) {
    case 'Premium': return 'bg-purple-100 text-purple-800'
    case 'Gold': return 'bg-yellow-100 text-yellow-800'
    case 'Silver': return 'bg-gray-100 text-gray-800'
    case 'Regular': return 'bg-blue-100 text-blue-800'
    default: return 'bg-gray-100 text-gray-800'
  }
}

// Utility function
function debounce(func: Function, wait: number) {
  let timeout: NodeJS.Timeout
  return function executedFunction(...args: any[]) {
    const later = () => {
      clearTimeout(timeout)
      func(...args)
    }
    clearTimeout(timeout)
    timeout = setTimeout(later, wait)
  }
}
</script>
