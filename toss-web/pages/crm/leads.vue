<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-slate-800 shadow-sm border-b border-slate-200 dark:border-slate-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-slate-900 dark:text-white">Leads Management</h1>
              <p class="text-slate-600 dark:text-slate-400">Track and manage potential customers</p>
            </div>
            <div class="flex space-x-3">
              <button @click="showCreateLeadModal = true" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors">
                <PlusIcon class="w-5 h-5 inline mr-2" />
                New Lead
              </button>
              <button @click="importLeads" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors">
                <UploadIcon class="w-5 h-5 inline mr-2" />
                Import Leads
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="pending" class="flex justify-center items-center h-64">
      <div class="animate-spin rounded-full h-32 w-32 border-b-2 border-blue-600"></div>
    </div>

    <!-- Main Content -->
    <div v-else class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <!-- Stats Cards -->
      <div class="grid grid-cols-1 xs:grid-cols-2 lg:grid-cols-4 gap-4 sm:gap-6 mb-6 sm:mb-8">
        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-slate-600 dark:text-slate-400">Total Leads</p>
              <p class="text-xl sm:text-2xl font-bold text-blue-600">{{ analytics?.leads?.totalLeads || leads.length }}</p>
            </div>
            <div class="p-2 sm:p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
              <UsersIcon class="w-5 h-5 sm:w-6 sm:h-6 text-blue-600" />
            </div>
          </div>
          <div class="mt-2">
            <span class="text-xs sm:text-sm text-green-600">+{{ analytics?.leads?.newThisMonth || 0 }} this month</span>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-slate-600 dark:text-slate-400">Qualified Leads</p>
              <p class="text-xl sm:text-2xl font-bold text-green-600">{{ analytics?.leads?.qualifiedLeads || 0 }}</p>
            </div>
            <div class="p-2 sm:p-3 bg-green-100 dark:bg-green-900 rounded-full">
              <CheckCircleIcon class="w-5 h-5 sm:w-6 sm:h-6 text-green-600" />
            </div>
          </div>
          <div class="mt-2">
            <span class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">{{ analytics?.leads?.conversionRate || 0 }}% conversion rate</span>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-slate-600 dark:text-slate-400">Average Score</p>
              <p class="text-xl sm:text-2xl font-bold text-yellow-600">{{ analytics?.leads?.averageScore || 0 }}</p>
            </div>
            <div class="p-2 sm:p-3 bg-yellow-100 dark:bg-yellow-900 rounded-full">
              <StarIcon class="w-5 h-5 sm:w-6 sm:h-6 text-yellow-600" />
            </div>
          </div>
          <div class="mt-2">
            <span class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Out of 100</span>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-slate-600 dark:text-slate-400">Hot Leads</p>
              <p class="text-xl sm:text-2xl font-bold text-red-600">{{ hotLeadsCount }}</p>
            </div>
            <div class="p-2 sm:p-3 bg-red-100 dark:bg-red-900 rounded-full">
              <FireIcon class="w-5 h-5 sm:w-6 sm:h-6 text-red-600" />
            </div>
          </div>
          <div class="mt-2">
            <span class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Score > 80</span>
          </div>
        </div>
      </div>

      <!-- Lead Source Breakdown -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-4 sm:gap-6 mb-6 sm:mb-8">
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Lead Sources</h3>
          </div>
          <div class="p-4 sm:p-6">
            <div class="space-y-4">
              <div v-for="source in leadSources" :key="source.source" class="flex items-center justify-between">
                <div class="flex items-center space-x-3">
                  <div class="w-4 h-4 rounded-full" :class="getSourceColor(source.source)"></div>
                  <span class="text-sm font-medium text-slate-900 dark:text-white">{{ source.source }}</span>
                </div>
                <div class="flex items-center space-x-3">
                  <span class="text-sm text-slate-600 dark:text-slate-400">{{ source.count }}</span>
                  <span class="text-sm text-slate-500 dark:text-slate-500">{{ source.percentage }}%</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Lead Status Distribution</h3>
          </div>
          <div class="p-4 sm:p-6">
            <div class="space-y-4">
              <div v-for="status in leadStatusDistribution" :key="status.status" class="flex items-center justify-between">
                <div class="flex items-center space-x-3">
                  <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full" :class="getStatusColor(status.status)">
                    {{ status.status }}
                  </span>
                </div>
                <div class="flex items-center space-x-3">
                  <span class="text-sm text-slate-600 dark:text-slate-400">{{ status.count }}</span>
                  <span class="text-sm text-slate-500 dark:text-slate-500">{{ status.percentage }}%</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Leads Table -->
      <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
        <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
          <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white">All Leads</h3>
            <div class="flex flex-col sm:flex-row gap-2 w-full sm:w-auto">
              <select v-model="statusFilter" class="text-sm border border-slate-300 dark:border-slate-600 rounded-lg px-3 py-1 bg-white dark:bg-slate-700 text-slate-900 dark:text-white">
                <option value="">All Status</option>
                <option value="New">New</option>
                <option value="Contacted">Contacted</option>
                <option value="Qualified">Qualified</option>
                <option value="Converted">Converted</option>
                <option value="Lost">Lost</option>
              </select>
              <input 
                v-model="searchTerm" 
                type="text" 
                placeholder="Search leads..." 
                class="px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-700 text-slate-900 dark:text-white text-sm"
              >
              <button @click="refreshLeads" class="bg-slate-100 dark:bg-slate-700 text-slate-600 dark:text-slate-400 px-3 py-2 rounded-lg hover:bg-slate-200 dark:hover:bg-slate-600 transition-colors">
                <RefreshIcon class="w-4 h-4" />
              </button>
            </div>
          </div>
        </div>
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-slate-50 dark:bg-slate-700">
              <tr>
                <th class="px-4 sm:px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Lead</th>
                <th class="px-4 sm:px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Source</th>
                <th class="px-4 sm:px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Score</th>
                <th class="px-4 sm:px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Status</th>
                <th class="px-4 sm:px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Created</th>
                <th class="px-4 sm:px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white dark:bg-slate-800 divide-y divide-slate-200 dark:divide-slate-700">
              <tr v-for="lead in filteredLeads" :key="lead.id" class="hover:bg-slate-50 dark:hover:bg-slate-700">
                <td class="px-4 sm:px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="w-8 h-8 sm:w-10 sm:h-10 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center">
                      <span class="text-blue-600 font-medium text-xs sm:text-sm">{{ getInitials(lead.firstName, lead.lastName) }}</span>
                    </div>
                    <div class="ml-3 sm:ml-4">
                      <div class="text-sm font-medium text-slate-900 dark:text-white">{{ lead.firstName }} {{ lead.lastName }}</div>
                      <div class="text-sm text-slate-500 dark:text-slate-400">{{ lead.email }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-4 sm:px-6 py-4 whitespace-nowrap">
                  <span class="text-sm text-slate-900 dark:text-white">{{ lead.source }}</span>
                </td>
                <td class="px-4 sm:px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <span class="text-sm font-medium text-slate-900 dark:text-white mr-2">{{ lead.score }}</span>
                    <div class="w-16 sm:w-20 bg-slate-200 dark:bg-slate-600 rounded-full h-2">
                      <div class="h-2 rounded-full" :class="getScoreColor(lead.score)" :style="{ width: lead.score + '%' }"></div>
                    </div>
                  </div>
                </td>
                <td class="px-4 sm:px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full" :class="getStatusColor(lead.status)">
                    {{ lead.status }}
                  </span>
                </td>
                <td class="px-4 sm:px-6 py-4 whitespace-nowrap text-sm text-slate-500 dark:text-slate-400">
                  {{ formatDate(new Date(lead.createdAt)) }}
                </td>
                <td class="px-4 sm:px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <div class="flex flex-col sm:flex-row gap-1 sm:gap-2">
                    <button @click="viewLead(lead)" class="text-blue-600 hover:text-blue-900">View</button>
                    <button @click="qualifyLead(lead)" class="text-green-600 hover:text-green-900">Qualify</button>
                    <button @click="convertLead(lead)" class="text-purple-600 hover:text-purple-900">Convert</button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Create Lead Modal -->
    <div v-if="showCreateLeadModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div class="bg-white dark:bg-slate-800 rounded-lg p-4 sm:p-6 w-full max-w-md max-h-[90vh] overflow-y-auto">
        <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4">Create New Lead</h3>
        <form @submit.prevent="createLead">
          <div class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300">First Name</label>
              <input v-model="newLead.firstName" type="text" required class="mt-1 block w-full border border-slate-300 dark:border-slate-600 rounded-lg px-3 py-2 bg-white dark:bg-slate-700 text-slate-900 dark:text-white">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300">Last Name</label>
              <input v-model="newLead.lastName" type="text" required class="mt-1 block w-full border border-slate-300 dark:border-slate-600 rounded-lg px-3 py-2 bg-white dark:bg-slate-700 text-slate-900 dark:text-white">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300">Email</label>
              <input v-model="newLead.email" type="email" required class="mt-1 block w-full border border-slate-300 dark:border-slate-600 rounded-lg px-3 py-2 bg-white dark:bg-slate-700 text-slate-900 dark:text-white">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300">Phone</label>
              <input v-model="newLead.phone" type="tel" required class="mt-1 block w-full border border-slate-300 dark:border-slate-600 rounded-lg px-3 py-2 bg-white dark:bg-slate-700 text-slate-900 dark:text-white">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300">Company</label>
              <input v-model="newLead.company" type="text" class="mt-1 block w-full border border-slate-300 dark:border-slate-600 rounded-lg px-3 py-2 bg-white dark:bg-slate-700 text-slate-900 dark:text-white">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300">Source</label>
              <select v-model="newLead.source" required class="mt-1 block w-full border border-slate-300 dark:border-slate-600 rounded-lg px-3 py-2 bg-white dark:bg-slate-700 text-slate-900 dark:text-white">
                <option value="">Select Source</option>
                <option value="Website">Website</option>
                <option value="Referral">Referral</option>
                <option value="SocialMedia">Social Media</option>
                <option value="Email">Email Campaign</option>
                <option value="TradeShow">Trade Show</option>
                <option value="Cold Call">Cold Call</option>
              </select>
            </div>
          </div>
          <div class="flex flex-col sm:flex-row justify-end gap-3 mt-6">
            <button type="button" @click="showCreateLeadModal = false" class="px-4 py-2 border border-slate-300 rounded-lg text-slate-700 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-700">
              Cancel
            </button>
            <button type="submit" :disabled="creatingLead" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50">
              {{ creatingLead ? 'Creating...' : 'Create Lead' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'

// Icons (simplified for demo)
const PlusIcon = 'svg'
const UploadIcon = 'svg'
const UsersIcon = 'svg'
const CheckCircleIcon = 'svg'
const StarIcon = 'svg'
const FireIcon = 'svg'
const RefreshIcon = 'svg'

// Reactive data
const searchTerm = ref('')
const statusFilter = ref('')
const showCreateLeadModal = ref(false)
const creatingLead = ref(false)

const newLead = ref({
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  company: '',
  source: ''
})

// Data
const leads = ref<any[]>([])
const analytics = ref<any>({})
const leadsPending = ref(false)

const pending = computed(() => leadsPending.value)

// Fetch data functions
async function fetchLeads() {
  leadsPending.value = true
  try {
    const response = await fetch('/api/crm/leads')
    const data = await response.json()
    leads.value = data || []
  } catch (error) {
    console.error('Error fetching leads:', error)
    leads.value = []
  } finally {
    leadsPending.value = false
  }
}

async function fetchAnalytics() {
  try {
    const response = await fetch('/api/crm/analytics')
    const data = await response.json()
    analytics.value = data || {}
  } catch (error) {
    console.error('Error fetching analytics:', error)
    analytics.value = {}
  }
}

async function refreshLeads() {
  await Promise.all([fetchLeads(), fetchAnalytics()])
}

// Load data on mount
onMounted(async () => {
  await refreshLeads()
})

// Computed properties
const filteredLeads = computed(() => {
  let filtered = leads.value
  
  if (statusFilter.value) {
    filtered = filtered.filter(lead => lead.status === statusFilter.value)
  }
  
  if (searchTerm.value) {
    const term = searchTerm.value.toLowerCase()
    filtered = filtered.filter(lead => 
      lead.firstName.toLowerCase().includes(term) ||
      lead.lastName.toLowerCase().includes(term) ||
      lead.email.toLowerCase().includes(term) ||
      lead.company?.toLowerCase().includes(term)
    )
  }
  
  return filtered
})

const hotLeadsCount = computed(() => {
  return leads.value.filter(lead => lead.score > 80).length
})

const leadSources = computed(() => {
  if (analytics.value.leads?.sourceBreakdown) {
    return analytics.value.leads.sourceBreakdown
  }
  
  // Fallback calculation
  const sources = leads.value.reduce((acc: any, lead) => {
    acc[lead.source] = (acc[lead.source] || 0) + 1
    return acc
  }, {})
  
  const total = leads.value.length || 1
  return Object.entries(sources).map(([source, count]: [string, any]) => ({
    source,
    count,
    percentage: ((count / total) * 100).toFixed(1)
  }))
})

const leadStatusDistribution = computed(() => {
  const statuses = leads.value.reduce((acc: any, lead) => {
    acc[lead.status] = (acc[lead.status] || 0) + 1
    return acc
  }, {})
  
  const total = leads.value.length || 1
  return Object.entries(statuses).map(([status, count]: [string, any]) => ({
    status,
    count,
    percentage: ((count / total) * 100).toFixed(1)
  }))
})

// Methods
async function createLead() {
  creatingLead.value = true
  try {
    const response = await fetch('/api/crm/leads', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(newLead.value)
    })
    
    if (!response.ok) {
      throw new Error('Failed to create lead')
    }
    
    // Reset form and close modal
    newLead.value = {
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      company: '',
      source: ''
    }
    showCreateLeadModal.value = false
    
    // Refresh leads list
    await refreshLeads()
    
    alert('Lead created successfully!')
  } catch (error) {
    console.error('Error creating lead:', error)
    alert('Failed to create lead. Please try again.')
  } finally {
    creatingLead.value = false
  }
}

function viewLead(lead: any) {
  alert(`Viewing lead: ${lead.firstName} ${lead.lastName}`)
}

function qualifyLead(lead: any) {
  alert(`Qualifying lead: ${lead.firstName} ${lead.lastName}`)
}

function convertLead(lead: any) {
  alert(`Converting lead: ${lead.firstName} ${lead.lastName} to customer`)
}

function importLeads() {
  alert('Import leads functionality would open file picker')
}

// Helper functions
function formatDate(date: Date): string {
  return new Intl.DateTimeFormat('en-ZA', {
    month: 'short',
    day: 'numeric',
    year: 'numeric'
  }).format(date)
}

function getInitials(firstName: string, lastName: string): string {
  return `${firstName?.[0] || ''}${lastName?.[0] || ''}`.toUpperCase()
}

function getSourceColor(source: string): string {
  switch (source) {
    case 'Website': return 'bg-blue-500'
    case 'Referral': return 'bg-green-500'
    case 'SocialMedia': return 'bg-purple-500'
    case 'Email': return 'bg-yellow-500'
    case 'TradeShow': return 'bg-red-500'
    default: return 'bg-gray-500'
  }
}

function getStatusColor(status: string): string {
  switch (status) {
    case 'New': return 'bg-blue-100 text-blue-800'
    case 'Contacted': return 'bg-yellow-100 text-yellow-800'
    case 'Qualified': return 'bg-green-100 text-green-800'
    case 'Converted': return 'bg-purple-100 text-purple-800'
    case 'Lost': return 'bg-red-100 text-red-800'
    default: return 'bg-gray-100 text-gray-800'
  }
}

function getScoreColor(score: number): string {
  if (score >= 80) return 'bg-green-500'
  if (score >= 60) return 'bg-yellow-500'
  if (score >= 40) return 'bg-orange-500'
  return 'bg-red-500'
}
</script>
