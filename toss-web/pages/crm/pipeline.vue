<template>
  <div class="lead-pipeline-container bg-gray-50 min-h-screen">
    <!-- Pipeline Header -->
    <div class="bg-white shadow-sm border-b border-gray-200 px-6 py-4">
      <div class="flex justify-between items-center">
        <div>
          <h1 class="text-2xl font-bold text-gray-900">Lead Pipeline</h1>
          <p class="text-gray-600 mt-1">Visual sales pipeline management</p>
        </div>
        <div class="flex space-x-3">
          <button 
            @click="showFilters = !showFilters"
            class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
          >
            <FunnelIcon class="h-4 w-4 mr-2" />
            Filters
          </button>
          <button 
            @click="showMetrics = !showMetrics"
            class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
          >
            <ChartBarIcon class="h-4 w-4 mr-2" />
            Metrics
          </button>
          <NuxtLink 
            to="/crm/leads/new"
            class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700"
          >
            <PlusIcon class="h-4 w-4 mr-2" />
            New Lead
          </NuxtLink>
        </div>
      </div>
      
      <!-- Filters Bar -->
      <div v-if="showFilters" class="mt-4 flex flex-wrap gap-3">
        <select v-model="filters.assignedTo" @change="applyFilters" class="border border-gray-300 rounded-md px-3 py-1 text-sm">
          <option value="">All Users</option>
          <option v-for="user in users" :key="user.id" :value="user.id">{{ user.name }}</option>
        </select>
        <select v-model="filters.priority" @change="applyFilters" class="border border-gray-300 rounded-md px-3 py-1 text-sm">
          <option value="">All Priorities</option>
          <option value="1">Critical</option>
          <option value="2">High</option>
          <option value="3">Medium</option>
        </select>
        <select v-model="filters.source" @change="applyFilters" class="border border-gray-300 rounded-md px-3 py-1 text-sm">
          <option value="">All Sources</option>
          <option value="Website">Website</option>
          <option value="Referral">Referral</option>
          <option value="Phone">Phone</option>
        </select>
      </div>

      <!-- Metrics Summary -->
      <div v-if="showMetrics && pipelineData.metrics" class="mt-4 grid grid-cols-2 md:grid-cols-6 gap-4">
        <div class="bg-blue-50 p-3 rounded-lg">
          <div class="text-2xl font-bold text-blue-600">{{ pipelineData.metrics.totalLeads }}</div>
          <div class="text-sm text-gray-600">Total Leads</div>
        </div>
        <div class="bg-green-50 p-3 rounded-lg">
          <div class="text-2xl font-bold text-green-600">{{ pipelineData.metrics.qualifiedLeads }}</div>
          <div class="text-sm text-gray-600">Qualified</div>
        </div>
        <div class="bg-purple-50 p-3 rounded-lg">
          <div class="text-2xl font-bold text-purple-600">{{ pipelineData.metrics.conversionRate }}%</div>
          <div class="text-sm text-gray-600">Conversion Rate</div>
        </div>
        <div class="bg-yellow-50 p-3 rounded-lg">
          <div class="text-2xl font-bold text-yellow-600">{{ formatCurrency(pipelineData.metrics.totalPipelineValue) }}</div>
          <div class="text-sm text-gray-600">Pipeline Value</div>
        </div>
        <div class="bg-indigo-50 p-3 rounded-lg">
          <div class="text-2xl font-bold text-indigo-600">{{ Math.round(pipelineData.metrics.averageDaysInPipeline) }}</div>
          <div class="text-sm text-gray-600">Avg Days</div>
        </div>
        <div class="bg-red-50 p-3 rounded-lg">
          <div class="text-2xl font-bold text-red-600">{{ overdueLeads.length }}</div>
          <div class="text-sm text-gray-600">Overdue</div>
        </div>
      </div>
    </div>

    <!-- Pipeline Stages -->
    <div class="flex-1 overflow-x-auto">
      <div class="flex space-x-6 p-6 min-w-max">
        <div 
          v-for="stage in pipelineData.stages" 
          :key="stage.name"
          class="flex-shrink-0 w-80"
        >
          <!-- Stage Header -->
          <div 
            class="bg-white rounded-lg shadow-sm border border-gray-200 p-4 mb-4"
            :style="{ borderTopColor: stage.color, borderTopWidth: '4px' }"
          >
            <div class="flex items-center justify-between">
              <div class="flex items-center">
                <div 
                  class="w-8 h-8 rounded-full flex items-center justify-center text-white text-sm font-medium"
                  :style="{ backgroundColor: stage.color }"
                >
                  {{ stage.leadCount }}
                </div>
                <div class="ml-3">
                  <h3 class="text-lg font-semibold text-gray-900">{{ stage.displayName }}</h3>
                  <p class="text-sm text-gray-500">{{ formatCurrency(stage.totalValue) }} total value</p>
                </div>
              </div>
              <div class="text-right">
                <div class="text-sm font-medium text-gray-900">Avg: {{ Math.round(stage.averageScore) }}</div>
                <div class="text-xs text-gray-500">score</div>
              </div>
            </div>
          </div>

          <!-- Lead Cards -->
          <div 
            class="space-y-3 min-h-[600px] pb-6"
            @drop="onDrop($event, stage.name)"
            @dragover.prevent
            @dragenter.prevent
          >
            <div 
              v-for="lead in stage.leads" 
              :key="lead.id"
              class="bg-white rounded-lg shadow-sm border border-gray-200 p-4 cursor-pointer hover:shadow-md transition-shadow"
              :class="{
                'border-red-300 bg-red-50': lead.isOverdue,
                'border-orange-300 bg-orange-50': lead.isHighPriority && !lead.isOverdue,
                'ring-2 ring-blue-300': selectedLead?.id === lead.id
              }"
              draggable="true"
              @dragstart="onDragStart($event, lead)"
              @click="selectLead(lead)"
            >
              <!-- Lead Header -->
              <div class="flex justify-between items-start mb-3">
                <div class="flex-1 min-w-0">
                  <h4 class="text-sm font-semibold text-gray-900 truncate">{{ lead.fullName }}</h4>
                  <p class="text-xs text-gray-600 truncate">{{ lead.company }}</p>
                  <p v-if="lead.jobTitle" class="text-xs text-gray-500 truncate">{{ lead.jobTitle }}</p>
                </div>
                <div class="flex-shrink-0 ml-2">
                  <div 
                    class="w-6 h-6 rounded-full flex items-center justify-center text-xs font-bold text-white"
                    :class="getScoreColor(lead.score)"
                  >
                    {{ lead.score }}
                  </div>
                </div>
              </div>

              <!-- Lead Details -->
              <div class="space-y-2">
                <!-- Priority and Value -->
                <div class="flex justify-between items-center">
                  <span 
                    class="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium"
                    :class="getPriorityClass(lead.priority)"
                  >
                    {{ lead.priority }}
                  </span>
                  <span v-if="lead.estimatedValue" class="text-xs font-medium text-green-600">
                    {{ formatCurrency(lead.estimatedValue) }}
                  </span>
                </div>

                <!-- Qualification Status -->
                <div class="flex items-center justify-between">
                  <span class="text-xs text-gray-500">Qualification:</span>
                  <span 
                    class="text-xs font-medium"
                    :class="getQualificationColor(lead.qualificationStatus)"
                  >
                    {{ lead.qualificationStatus }}
                  </span>
                </div>

                <!-- Time Tracking -->
                <div class="text-xs text-gray-500 space-y-1">
                  <div class="flex justify-between">
                    <span>In stage:</span>
                    <span :class="{ 'text-red-600 font-medium': lead.isOverdue }">
                      {{ lead.daysInStage }} days
                    </span>
                  </div>
                  <div class="flex justify-between">
                    <span>Last contact:</span>
                    <span :class="{ 'text-red-600 font-medium': lead.daysSinceLastContact > 7 }">
                      {{ lead.daysSinceLastContact }} days ago
                    </span>
                  </div>
                </div>

                <!-- Expected Close Date -->
                <div v-if="lead.expectedCloseDate" class="text-xs text-gray-500">
                  <div class="flex justify-between">
                    <span>Expected close:</span>
                    <span>{{ formatDate(lead.expectedCloseDate) }}</span>
                  </div>
                </div>

                <!-- Assigned User -->
                <div v-if="lead.assignedTo" class="text-xs text-gray-500">
                  <div class="flex items-center">
                    <UserIcon class="h-3 w-3 mr-1" />
                    <span>{{ lead.assignedTo }}</span>
                  </div>
                </div>
              </div>

              <!-- Action Buttons -->
              <div class="mt-3 flex space-x-2">
                <button 
                  @click.stop="contactLead(lead)"
                  class="flex-1 text-xs bg-blue-50 text-blue-700 hover:bg-blue-100 px-2 py-1 rounded"
                >
                  Contact
                </button>
                <button 
                  @click.stop="editLead(lead)"
                  class="flex-1 text-xs bg-gray-50 text-gray-700 hover:bg-gray-100 px-2 py-1 rounded"
                >
                  Edit
                </button>
              </div>
            </div>

            <!-- Empty State -->
            <div v-if="stage.leads.length === 0" class="text-center py-12 text-gray-500">
              <div class="text-sm">No leads in this stage</div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Lead Detail Panel -->
    <LeadDetailPanel
      v-if="selectedLead"
      :lead="selectedLead"
      :show="!!selectedLead"
      @close="selectedLead = null"
      @updated="refreshPipeline"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { 
  FunnelIcon, 
  ChartBarIcon, 
  PlusIcon, 
  UserIcon 
} from '@heroicons/vue/24/outline'

// Types
interface LeadCardView {
  id: string
  fullName: string
  company: string
  jobTitle?: string
  score: number
  priority: string
  assignedTo?: string
  estimatedValue?: number
  expectedCloseDate?: string
  daysInStage: number
  isOverdue: boolean
  isHighPriority: boolean
  qualificationStatus: string
  lastContactedAt: string
  daysSinceLastContact: number
}

interface PipelineStageView {
  name: string
  displayName: string
  color: string
  icon: string
  order: number
  leadCount: number
  totalValue: number
  averageScore: number
  leads: LeadCardView[]
}

// State
const pipelineData = ref<{
  stages: PipelineStageView[]
  metrics: any
}>({ stages: [], metrics: {} })

const overdueLeads = ref<LeadCardView[]>([])
const selectedLead = ref<LeadCardView | null>(null)
const showFilters = ref(false)
const showMetrics = ref(true)

const filters = ref({
  assignedTo: '',
  priority: '',
  source: ''
})

const users = ref([
  { id: 'user1', name: 'John Smith' },
  { id: 'user2', name: 'Sarah Johnson' },
  { id: 'user3', name: 'Mike Wilson' }
])

// Methods
const loadPipelineData = async () => {
  try {
    // This would be replaced with actual API call
    const mockData = {
      stages: [
        {
          name: 'New',
          displayName: 'New Leads',
          color: '#94a3b8',
          icon: 'user-plus',
          order: 1,
          leadCount: 5,
          totalValue: 50000,
          averageScore: 35,
          leads: [
            {
              id: '1',
              fullName: 'Alice Cooper',
              company: 'Tech Solutions Ltd',
              jobTitle: 'CTO',
              score: 45,
              priority: 'High',
              estimatedValue: 15000,
              daysInStage: 2,
              isOverdue: false,
              isHighPriority: true,
              qualificationStatus: 'Not Evaluated',
              lastContactedAt: '2025-08-25',
              daysSinceLastContact: 1
            }
          ]
        },
        {
          name: 'Contacted',
          displayName: 'Initial Contact',
          color: '#3b82f6',
          icon: 'phone',
          order: 2,
          leadCount: 3,
          totalValue: 75000,
          averageScore: 55,
          leads: []
        },
        {
          name: 'Qualified',
          displayName: 'Qualified',
          color: '#10b981',
          icon: 'check-circle',
          order: 3,
          leadCount: 8,
          totalValue: 120000,
          averageScore: 75,
          leads: []
        },
        {
          name: 'Proposal',
          displayName: 'Proposal Sent',
          color: '#f59e0b',
          icon: 'document-text',
          order: 4,
          leadCount: 4,
          totalValue: 95000,
          averageScore: 85,
          leads: []
        },
        {
          name: 'Negotiation',
          displayName: 'In Negotiation',
          color: '#ef4444',
          icon: 'chat-bubble-left-right',
          order: 5,
          leadCount: 2,
          totalValue: 60000,
          averageScore: 90,
          leads: []
        }
      ],
      metrics: {
        totalLeads: 22,
        qualifiedLeads: 14,
        conversionRate: 23.5,
        totalPipelineValue: 400000,
        averageDaysInPipeline: 18
      }
    }
    
    pipelineData.value = mockData
  } catch (error) {
    console.error('Error loading pipeline data:', error)
  }
}

const applyFilters = () => {
  // Refresh pipeline with filters
  loadPipelineData()
}

const refreshPipeline = () => {
  loadPipelineData()
}

// Drag and Drop
const onDragStart = (event: DragEvent, lead: LeadCardView) => {
  if (event.dataTransfer) {
    event.dataTransfer.setData('text/plain', JSON.stringify(lead))
  }
}

const onDrop = async (event: DragEvent, targetStage: string) => {
  event.preventDefault()
  if (event.dataTransfer) {
    const leadData = JSON.parse(event.dataTransfer.getData('text/plain'))
    await moveLeadToStage(leadData.id, targetStage)
  }
}

const moveLeadToStage = async (leadId: string, targetStage: string) => {
  try {
    // This would be replaced with actual API call
    console.log(`Moving lead ${leadId} to stage ${targetStage}`)
    await refreshPipeline()
  } catch (error) {
    console.error('Error moving lead:', error)
  }
}

// Lead Actions
const selectLead = (lead: LeadCardView) => {
  selectedLead.value = lead
}

const contactLead = (lead: LeadCardView) => {
  // Navigate to contact/communication page
  console.log('Contacting lead:', lead.fullName)
}

const editLead = (lead: LeadCardView) => {
  // Navigate to edit lead page using Nuxt navigation
  window.location.href = `/crm/leads/${lead.id}/edit`
}

// Utility Functions
const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(value)
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('en-ZA')
}

const getScoreColor = (score: number) => {
  if (score >= 80) return 'bg-green-500'
  if (score >= 60) return 'bg-yellow-500'
  if (score >= 40) return 'bg-orange-500'
  return 'bg-red-500'
}

const getPriorityClass = (priority: string) => {
  switch (priority.toLowerCase()) {
    case 'critical':
      return 'bg-red-100 text-red-800'
    case 'high':
      return 'bg-orange-100 text-orange-800'
    case 'medium':
      return 'bg-yellow-100 text-yellow-800'
    case 'low':
      return 'bg-green-100 text-green-800'
    default:
      return 'bg-gray-100 text-gray-800'
  }
}

const getQualificationColor = (status: string) => {
  switch (status.toLowerCase()) {
    case 'fully qualified':
      return 'text-green-600'
    case 'highly qualified':
      return 'text-blue-600'
    case 'partially qualified':
      return 'text-yellow-600'
    default:
      return 'text-gray-600'
  }
}

// Lifecycle
onMounted(() => {
  loadPipelineData()
})
</script>

<style scoped>
.lead-pipeline-container {
  display: flex;
  flex-direction: column;
  height: 100vh;
}

/* Custom scrollbar for pipeline */
.overflow-x-auto::-webkit-scrollbar {
  height: 8px;
}

.overflow-x-auto::-webkit-scrollbar-track {
  background: #f1f5f9;
}

.overflow-x-auto::-webkit-scrollbar-thumb {
  background: #cbd5e1;
  border-radius: 4px;
}

.overflow-x-auto::-webkit-scrollbar-thumb:hover {
  background: #94a3b8;
}

/* Drag and drop visual feedback */
.lead-card:hover {
  transform: translateY(-1px);
}

.lead-card.dragging {
  opacity: 0.6;
  transform: rotate(2deg);
}

/* Animation for stage transitions */
.stage-enter-active,
.stage-leave-active {
  transition: all 0.3s ease;
}

.stage-enter-from,
.stage-leave-to {
  opacity: 0;
  transform: translateY(10px);
}
</style>
