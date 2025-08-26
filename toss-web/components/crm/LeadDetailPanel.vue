<template>
  <div 
    v-if="show" 
    class="fixed inset-y-0 right-0 w-96 bg-white shadow-xl z-50 transform transition-transform duration-300"
    :class="show ? 'translate-x-0' : 'translate-x-full'"
  >
    <!-- Panel Header -->
    <div class="bg-gray-50 px-6 py-4 border-b border-gray-200">
      <div class="flex items-center justify-between">
        <h2 class="text-lg font-semibold text-gray-900">Lead Details</h2>
        <button 
          @click="$emit('close')"
          class="text-gray-400 hover:text-gray-600"
        >
          <XMarkIcon class="h-6 w-6" />
        </button>
      </div>
    </div>

    <!-- Panel Content -->
    <div class="flex-1 overflow-y-auto p-6">
      <div v-if="lead" class="space-y-6">
        <!-- Basic Info -->
        <div>
          <h3 class="text-xl font-bold text-gray-900">{{ lead.fullName }}</h3>
          <p class="text-gray-600">{{ lead.company }}</p>
          <p v-if="lead.jobTitle" class="text-sm text-gray-500">{{ lead.jobTitle }}</p>
        </div>

        <!-- Score and Priority -->
        <div class="grid grid-cols-2 gap-4">
          <div class="bg-gray-50 p-3 rounded-lg">
            <div class="text-2xl font-bold" :class="getScoreColor(lead.score)">{{ lead.score }}</div>
            <div class="text-sm text-gray-600">Lead Score</div>
          </div>
          <div class="bg-gray-50 p-3 rounded-lg">
            <div 
              class="text-sm font-medium px-2 py-1 rounded-full inline-block"
              :class="getPriorityClass(lead.priority)"
            >
              {{ lead.priority }}
            </div>
            <div class="text-sm text-gray-600 mt-1">Priority</div>
          </div>
        </div>

        <!-- Qualification Status -->
        <div>
          <h4 class="font-medium text-gray-900 mb-2">Qualification Status</h4>
          <div 
            class="text-sm font-medium"
            :class="getQualificationColor(lead.qualificationStatus)"
          >
            {{ lead.qualificationStatus }}
          </div>
        </div>

        <!-- Timeline -->
        <div>
          <h4 class="font-medium text-gray-900 mb-3">Timeline</h4>
          <div class="space-y-2 text-sm">
            <div class="flex justify-between">
              <span class="text-gray-600">Days in current stage:</span>
              <span :class="{ 'text-red-600 font-medium': lead.isOverdue }">
                {{ lead.daysInStage }} days
              </span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-600">Last contacted:</span>
              <span :class="{ 'text-red-600 font-medium': lead.daysSinceLastContact > 7 }">
                {{ lead.daysSinceLastContact }} days ago
              </span>
            </div>
            <div v-if="lead.expectedCloseDate" class="flex justify-between">
              <span class="text-gray-600">Expected close:</span>
              <span>{{ formatDate(lead.expectedCloseDate) }}</span>
            </div>
          </div>
        </div>

        <!-- Value -->
        <div v-if="lead.estimatedValue">
          <h4 class="font-medium text-gray-900 mb-2">Estimated Value</h4>
          <div class="text-lg font-semibold text-green-600">
            {{ formatCurrency(lead.estimatedValue) }}
          </div>
        </div>

        <!-- Assignment -->
        <div v-if="lead.assignedTo">
          <h4 class="font-medium text-gray-900 mb-2">Assigned To</h4>
          <div class="flex items-center">
            <UserIcon class="h-4 w-4 mr-2 text-gray-400" />
            <span>{{ lead.assignedTo }}</span>
          </div>
        </div>

        <!-- Quick Actions -->
        <div class="space-y-3">
          <h4 class="font-medium text-gray-900">Quick Actions</h4>
          <div class="grid grid-cols-2 gap-3">
            <button 
              @click="contactLead"
              class="flex items-center justify-center px-3 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 text-sm"
            >
              <PhoneIcon class="h-4 w-4 mr-1" />
              Call
            </button>
            <button 
              @click="emailLead"
              class="flex items-center justify-center px-3 py-2 bg-green-600 text-white rounded-md hover:bg-green-700 text-sm"
            >
              <EnvelopeIcon class="h-4 w-4 mr-1" />
              Email
            </button>
            <button 
              @click="scheduleMeeting"
              class="flex items-center justify-center px-3 py-2 bg-purple-600 text-white rounded-md hover:bg-purple-700 text-sm"
            >
              <CalendarIcon class="h-4 w-4 mr-1" />
              Meeting
            </button>
            <button 
              @click="editLead"
              class="flex items-center justify-center px-3 py-2 bg-gray-600 text-white rounded-md hover:bg-gray-700 text-sm"
            >
              <PencilIcon class="h-4 w-4 mr-1" />
              Edit
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- Backdrop -->
  <div 
    v-if="show"
    class="fixed inset-0 bg-black bg-opacity-25 z-40"
    @click="$emit('close')"
  ></div>
</template>

<script setup lang="ts">
import { 
  XMarkIcon, 
  UserIcon, 
  PhoneIcon, 
  EnvelopeIcon, 
  CalendarIcon, 
  PencilIcon 
} from '@heroicons/vue/24/outline'

// Props
interface Props {
  lead: any | null
  show: boolean
}

const props = defineProps<Props>()

// Emits
const emit = defineEmits<{
  close: []
  updated: []
}>()

// Methods
const contactLead = () => {
  console.log('Contacting lead:', props.lead?.fullName)
  // Implementation for contacting lead
}

const emailLead = () => {
  console.log('Emailing lead:', props.lead?.fullName)
  // Implementation for emailing lead
}

const scheduleMeeting = () => {
  console.log('Scheduling meeting with:', props.lead?.fullName)
  // Implementation for scheduling meeting
}

const editLead = () => {
  console.log('Editing lead:', props.lead?.fullName)
  // Implementation for editing lead
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
  if (score >= 80) return 'text-green-600'
  if (score >= 60) return 'text-yellow-600'
  if (score >= 40) return 'text-orange-600'
  return 'text-red-600'
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
</script>
