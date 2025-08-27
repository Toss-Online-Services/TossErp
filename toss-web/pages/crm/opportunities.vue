<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-slate-800 shadow-sm border-b border-slate-200 dark:border-slate-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-slate-900 dark:text-white">Sales Opportunities</h1>
              <p class="text-slate-600 dark:text-slate-400">Track and manage sales opportunities through your pipeline</p>
            </div>
            <div class="flex flex-col sm:flex-row gap-2 sm:gap-3">
              <button @click="showCreateOpportunityModal = true" class="bg-blue-600 text-white px-3 sm:px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors text-sm sm:text-base">
                <PlusIcon class="w-4 h-4 sm:w-5 sm:h-5 inline mr-1 sm:mr-2" />
                New Opportunity
              </button>
              <button @click="exportPipeline" class="bg-green-600 text-white px-3 sm:px-4 py-2 rounded-lg hover:bg-green-700 transition-colors text-sm sm:text-base">
                <DownloadIcon class="w-4 h-4 sm:w-5 sm:h-5 inline mr-1 sm:mr-2" />
                Export Pipeline
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
      <!-- Pipeline Summary -->
      <div class="grid grid-cols-1 xs:grid-cols-2 lg:grid-cols-4 gap-4 sm:gap-6 mb-6 sm:mb-8">
        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-slate-600 dark:text-slate-400">Total Pipeline</p>
              <p class="text-xl sm:text-2xl font-bold text-blue-600">R {{ formatCurrency(analytics?.pipeline?.totalValue || 0) }}</p>
            </div>
            <div class="p-2 sm:p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
              <CurrencyDollarIcon class="w-5 h-5 sm:w-6 sm:h-6 text-blue-600" />
            </div>
          </div>
          <div class="mt-2">
            <span class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">{{ opportunities.length }} opportunities</span>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-slate-600 dark:text-slate-400">Weighted Pipeline</p>
              <p class="text-2xl font-bold text-green-600">R {{ formatCurrency(analytics?.pipeline?.weightedValue || 0) }}</p>
            </div>
            <div class="p-3 bg-green-100 dark:bg-green-900 rounded-full">
              <TrendingUpIcon class="w-6 h-6 text-green-600" />
            </div>
          </div>
          <div class="mt-2">
            <span class="text-sm text-slate-600 dark:text-slate-400">Based on stage probability</span>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-6 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-slate-600 dark:text-slate-400">Average Deal Size</p>
              <p class="text-2xl font-bold text-purple-600">R {{ formatCurrency(averageDealSize) }}</p>
            </div>
            <div class="p-3 bg-purple-100 dark:bg-purple-900 rounded-full">
              <ChartBarIcon class="w-6 h-6 text-purple-600" />
            </div>
          </div>
          <div class="mt-2">
            <span class="text-sm text-slate-600 dark:text-slate-400">Across all stages</span>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-6 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-slate-600 dark:text-slate-400">Avg Cycle Length</p>
              <p class="text-2xl font-bold text-yellow-600">{{ analytics?.pipeline?.averageCycleLength || 0 }} days</p>
            </div>
            <div class="p-3 bg-yellow-100 dark:bg-yellow-900 rounded-full">
              <ClockIcon class="w-6 h-6 text-yellow-600" />
            </div>
          </div>
          <div class="mt-2">
            <span class="text-sm text-slate-600 dark:text-slate-400">Time to close</span>
          </div>
        </div>
      </div>

      <!-- Pipeline Kanban Board -->
      <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 mb-8">
        <div class="p-6 border-b border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Sales Pipeline</h3>
            <div class="flex space-x-2">
              <select v-model="viewMode" class="text-sm border border-slate-300 dark:border-slate-600 rounded-lg px-3 py-1 bg-white dark:bg-slate-700 text-slate-900 dark:text-white">
                <option value="kanban">Kanban View</option>
                <option value="table">Table View</option>
              </select>
            </div>
          </div>
        </div>
        
        <!-- Kanban View -->
        <div v-if="viewMode === 'kanban'" class="p-6">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-6">
            <div v-for="stage in pipelineStages" :key="stage.name" class="bg-slate-50 dark:bg-slate-700 rounded-lg p-4">
              <div class="flex items-center justify-between mb-4">
                <h4 class="font-medium text-slate-900 dark:text-white">{{ stage.name }}</h4>
                <div class="flex items-center space-x-2">
                  <span class="text-xs text-slate-500 dark:text-slate-400">{{ stage.count }}</span>
                  <span class="text-xs text-slate-500 dark:text-slate-400">R {{ formatCurrency(stage.value) }}</span>
                </div>
              </div>
              <div class="space-y-3">
                <div 
                  v-for="opportunity in getOpportunitiesByStage(stage.name)" 
                  :key="opportunity.id" 
                  class="bg-white dark:bg-slate-800 p-3 rounded border border-slate-200 dark:border-slate-600 cursor-pointer hover:shadow-md transition-shadow"
                  @click="viewOpportunity(opportunity)"
                >
                  <p class="font-medium text-sm text-slate-900 dark:text-white">{{ opportunity.title }}</p>
                  <p class="text-xs text-slate-600 dark:text-slate-400">{{ opportunity.customerName }}</p>
                  <div class="flex items-center justify-between mt-2">
                    <span class="text-sm font-semibold text-green-600">R {{ formatCurrency(opportunity.value) }}</span>
                    <span class="text-xs text-slate-500 dark:text-slate-400">{{ opportunity.probability }}%</span>
                  </div>
                  <div class="flex items-center justify-between mt-1">
                    <span class="text-xs text-slate-500 dark:text-slate-400">{{ formatDate(new Date(opportunity.expectedCloseDate)) }}</span>
                    <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full" :class="getPriorityColor(opportunity.priority)">
                      {{ opportunity.priority }}
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Table View -->
        <div v-else class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-slate-50 dark:bg-slate-700">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Opportunity</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Customer</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Value</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Stage</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Probability</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Expected Close</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white dark:bg-slate-800 divide-y divide-slate-200 dark:divide-slate-700">
              <tr v-for="opportunity in filteredOpportunities" :key="opportunity.id" class="hover:bg-slate-50 dark:hover:bg-slate-700">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div>
                    <div class="text-sm font-medium text-slate-900 dark:text-white">{{ opportunity.title }}</div>
                    <div class="text-sm text-slate-500 dark:text-slate-400">{{ opportunity.description }}</div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="text-sm text-slate-900 dark:text-white">{{ opportunity.customerName }}</span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="text-sm font-medium text-slate-900 dark:text-white">R {{ formatCurrency(opportunity.value) }}</span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full" :class="getStageColor(opportunity.stage)">
                    {{ opportunity.stage }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <span class="text-sm text-slate-900 dark:text-white mr-2">{{ opportunity.probability }}%</span>
                    <div class="w-16 bg-slate-200 dark:bg-slate-600 rounded-full h-2">
                      <div class="bg-blue-500 h-2 rounded-full" :style="{ width: opportunity.probability + '%' }"></div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-500 dark:text-slate-400">
                  {{ formatDate(new Date(opportunity.expectedCloseDate)) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <div class="flex space-x-2">
                    <button @click="viewOpportunity(opportunity)" class="text-blue-600 hover:text-blue-900">View</button>
                    <button @click="updateStage(opportunity)" class="text-green-600 hover:text-green-900">Move</button>
                    <button @click="editOpportunity(opportunity)" class="text-purple-600 hover:text-purple-900">Edit</button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Forecasting -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="p-6 border-b border-slate-200 dark:border-slate-700">
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Quarterly Forecast</h3>
          </div>
          <div class="p-6">
            <div class="space-y-4">
              <div v-for="quarter in quarterlyForecast" :key="quarter.period" class="flex items-center justify-between p-4 bg-slate-50 dark:bg-slate-700 rounded-lg">
                <div>
                  <p class="font-medium text-slate-900 dark:text-white">{{ quarter.period }}</p>
                  <p class="text-sm text-slate-600 dark:text-slate-400">{{ quarter.expectedDeals }} deals expected</p>
                </div>
                <div class="text-right">
                  <p class="font-semibold text-slate-900 dark:text-white">R {{ formatCurrency(quarter.projectedRevenue) }}</p>
                  <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full" :class="getConfidenceColor(quarter.confidence)">
                    {{ quarter.confidence }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="p-6 border-b border-slate-200 dark:border-slate-700">
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Stage Performance</h3>
          </div>
          <div class="p-6">
            <div class="space-y-4">
              <div v-for="stage in stagePerformance" :key="stage.stage" class="flex items-center justify-between">
                <div class="flex items-center space-x-3">
                  <span class="text-sm font-medium text-slate-900 dark:text-white">{{ stage.stage }}</span>
                </div>
                <div class="flex items-center space-x-4">
                  <div class="text-right">
                    <p class="text-sm text-slate-900 dark:text-white">{{ stage.count }} opportunities</p>
                    <p class="text-xs text-slate-500 dark:text-slate-500">{{ stage.percentage }}% of pipeline</p>
                  </div>
                  <div class="w-20 bg-slate-200 dark:bg-slate-600 rounded-full h-2">
                    <div class="bg-blue-500 h-2 rounded-full" :style="{ width: stage.percentage + '%' }"></div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Opportunity Modal -->
    <div v-if="showCreateOpportunityModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-slate-800 rounded-lg p-6 w-full max-w-lg">
        <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4">Create New Opportunity</h3>
        <form @submit.prevent="createOpportunity">
          <div class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300">Title</label>
              <input v-model="newOpportunity.title" type="text" required class="mt-1 block w-full border border-slate-300 dark:border-slate-600 rounded-lg px-3 py-2 bg-white dark:bg-slate-700 text-slate-900 dark:text-white">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300">Description</label>
              <textarea v-model="newOpportunity.description" class="mt-1 block w-full border border-slate-300 dark:border-slate-600 rounded-lg px-3 py-2 bg-white dark:bg-slate-700 text-slate-900 dark:text-white"></textarea>
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300">Value (R)</label>
                <input v-model.number="newOpportunity.value" type="number" required min="0" class="mt-1 block w-full border border-slate-300 dark:border-slate-600 rounded-lg px-3 py-2 bg-white dark:bg-slate-700 text-slate-900 dark:text-white">
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300">Probability (%)</label>
                <input v-model.number="newOpportunity.probability" type="number" required min="0" max="100" class="mt-1 block w-full border border-slate-300 dark:border-slate-600 rounded-lg px-3 py-2 bg-white dark:bg-slate-700 text-slate-900 dark:text-white">
              </div>
            </div>
            <div class="grid grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300">Stage</label>
                <select v-model="newOpportunity.stage" required class="mt-1 block w-full border border-slate-300 dark:border-slate-600 rounded-lg px-3 py-2 bg-white dark:bg-slate-700 text-slate-900 dark:text-white">
                  <option value="">Select Stage</option>
                  <option value="Prospecting">Prospecting</option>
                  <option value="Qualification">Qualification</option>
                  <option value="NeedsAnalysis">Needs Analysis</option>
                  <option value="Proposal">Proposal</option>
                  <option value="Negotiation">Negotiation</option>
                  <option value="Closed Won">Closed Won</option>
                  <option value="Closed Lost">Closed Lost</option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300">Priority</label>
                <select v-model="newOpportunity.priority" required class="mt-1 block w-full border border-slate-300 dark:border-slate-600 rounded-lg px-3 py-2 bg-white dark:bg-slate-700 text-slate-900 dark:text-white">
                  <option value="">Select Priority</option>
                  <option value="High">High</option>
                  <option value="Medium">Medium</option>
                  <option value="Low">Low</option>
                </select>
              </div>
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300">Expected Close Date</label>
              <input v-model="newOpportunity.expectedCloseDate" type="date" required class="mt-1 block w-full border border-slate-300 dark:border-slate-600 rounded-lg px-3 py-2 bg-white dark:bg-slate-700 text-slate-900 dark:text-white">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300">Customer Name</label>
              <input v-model="newOpportunity.customerName" type="text" required class="mt-1 block w-full border border-slate-300 dark:border-slate-600 rounded-lg px-3 py-2 bg-white dark:bg-slate-700 text-slate-900 dark:text-white">
            </div>
          </div>
          <div class="flex justify-end space-x-3 mt-6">
            <button type="button" @click="showCreateOpportunityModal = false" class="px-4 py-2 border border-slate-300 rounded-lg text-slate-700 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-700">
              Cancel
            </button>
            <button type="submit" :disabled="creatingOpportunity" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50">
              {{ creatingOpportunity ? 'Creating...' : 'Create Opportunity' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'

// Types
interface PipelineStage {
  name: string
  count: number
  value: number
}

// Icons (simplified for demo)
const PlusIcon = 'svg'
const DownloadIcon = 'svg'
const CurrencyDollarIcon = 'svg'
const TrendingUpIcon = 'svg'
const ChartBarIcon = 'svg'
const ClockIcon = 'svg'

// Reactive data
const viewMode = ref('kanban')
const showCreateOpportunityModal = ref(false)
const creatingOpportunity = ref(false)

const newOpportunity = ref({
  title: '',
  description: '',
  value: 0,
  probability: 50,
  stage: '',
  priority: '',
  expectedCloseDate: '',
  customerName: ''
})

// Data
const opportunities = ref<any[]>([])
const analytics = ref<any>({})
const opportunitiesPending = ref(false)

const pending = computed(() => opportunitiesPending.value)

// Fetch data functions
async function fetchOpportunities() {
  opportunitiesPending.value = true
  try {
    const response = await fetch('/api/crm/opportunities')
    const data = await response.json()
    opportunities.value = data || []
  } catch (error) {
    console.error('Error fetching opportunities:', error)
    opportunities.value = []
  } finally {
    opportunitiesPending.value = false
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

async function refreshData() {
  await Promise.all([fetchOpportunities(), fetchAnalytics()])
}

// Load data on mount
onMounted(async () => {
  await refreshData()
})

// Computed properties
const filteredOpportunities = computed(() => {
  return opportunities.value
})

const pipelineStages = computed(() => {
  if (analytics.value.pipeline?.stages) {
    return analytics.value.pipeline.stages
  }
  
  // Fallback calculation
  const stages = ['Prospecting', 'Qualification', 'NeedsAnalysis', 'Proposal', 'Negotiation']
  return stages.map(stage => {
    const stageOpps = opportunities.value.filter(o => o.stage === stage)
    return {
      name: stage,
      count: stageOpps.length,
      value: stageOpps.reduce((sum, o) => sum + (o.value || 0), 0)
    }
  })
})

const averageDealSize = computed(() => {
  if (opportunities.value.length === 0) return 0
  const total = opportunities.value.reduce((sum, o) => sum + (o.value || 0), 0)
  return Math.round(total / opportunities.value.length)
})

const quarterlyForecast = computed(() => {
  if (analytics.value.forecast) {
    return [
      { period: 'Q1 2024', ...analytics.value.forecast.q1_2024 },
      { period: 'Q2 2024', ...analytics.value.forecast.q2_2024 },
      { period: 'Q3 2024', ...analytics.value.forecast.q3_2024 },
      { period: 'Q4 2024', ...analytics.value.forecast.q4_2024 }
    ]
  }
  
  return [
    { period: 'Q1 2024', expectedDeals: 2, projectedRevenue: 425000, confidence: 'High' },
    { period: 'Q2 2024', expectedDeals: 1, projectedRevenue: 195000, confidence: 'Medium' },
    { period: 'Q3 2024', expectedDeals: 1, projectedRevenue: 8750, confidence: 'Low' },
    { period: 'Q4 2024', expectedDeals: 0, projectedRevenue: 0, confidence: 'Low' }
  ]
})

const stagePerformance = computed(() => {
  const total = opportunities.value.length || 1
  return pipelineStages.value.map((stage: PipelineStage) => ({
    stage: stage.name,
    count: stage.count,
    percentage: Math.round((stage.count / total) * 100)
  }))
})

// Methods
function getOpportunitiesByStage(stage: string) {
  return opportunities.value.filter(o => o.stage === stage).slice(0, 5) // Limit to 5 for display
}

async function createOpportunity() {
  creatingOpportunity.value = true
  try {
    const response = await fetch('/api/crm/opportunities', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(newOpportunity.value)
    })
    
    if (!response.ok) {
      throw new Error('Failed to create opportunity')
    }
    
    // Reset form and close modal
    newOpportunity.value = {
      title: '',
      description: '',
      value: 0,
      probability: 50,
      stage: '',
      priority: '',
      expectedCloseDate: '',
      customerName: ''
    }
    showCreateOpportunityModal.value = false
    
    // Refresh data
    await refreshData()
    
    alert('Opportunity created successfully!')
  } catch (error) {
    console.error('Error creating opportunity:', error)
    alert('Failed to create opportunity. Please try again.')
  } finally {
    creatingOpportunity.value = false
  }
}

function viewOpportunity(opportunity: any) {
  alert(`Viewing opportunity: ${opportunity.title}`)
}

function updateStage(opportunity: any) {
  alert(`Moving opportunity: ${opportunity.title} to next stage`)
}

function editOpportunity(opportunity: any) {
  alert(`Editing opportunity: ${opportunity.title}`)
}

function exportPipeline() {
  alert('Export pipeline functionality would generate CSV/Excel')
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
    year: 'numeric'
  }).format(date)
}

function getStageColor(stage: string): string {
  switch (stage) {
    case 'Prospecting': return 'bg-blue-100 text-blue-800'
    case 'Qualification': return 'bg-yellow-100 text-yellow-800'
    case 'NeedsAnalysis': return 'bg-orange-100 text-orange-800'
    case 'Proposal': return 'bg-purple-100 text-purple-800'
    case 'Negotiation': return 'bg-green-100 text-green-800'
    case 'Closed Won': return 'bg-green-100 text-green-800'
    case 'Closed Lost': return 'bg-red-100 text-red-800'
    default: return 'bg-slate-100 text-slate-800'
  }
}

function getPriorityColor(priority: string): string {
  switch (priority) {
    case 'High': return 'bg-red-100 text-red-800'
    case 'Medium': return 'bg-yellow-100 text-yellow-800'
    case 'Low': return 'bg-green-100 text-green-800'
    default: return 'bg-slate-100 text-slate-800'
  }
}

function getConfidenceColor(confidence: string): string {
  switch (confidence) {
    case 'High': return 'bg-green-100 text-green-800'
    case 'Medium': return 'bg-yellow-100 text-yellow-800'
    case 'Low': return 'bg-red-100 text-red-800'
    default: return 'bg-slate-100 text-slate-800'
  }
}
</script>
