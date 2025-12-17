<template>
  <TransitionRoot as="template" :show="true">
    <Dialog as="div" class="relative z-50" @close="$emit('close')">
      <TransitionChild
        as="template"
        enter="ease-out duration-300"
        enter-from="opacity-0"
        enter-to="opacity-100"
        leave="ease-in duration-200"
        leave-from="opacity-100"
        leave-to="opacity-0"
      >
        <div class="fixed inset-0 bg-black bg-opacity-50 transition-opacity" />
      </TransitionChild>

      <div class="fixed inset-0 z-10 overflow-y-auto">
        <div class="flex min-h-full items-end justify-center p-4 text-center sm:items-center sm:p-0">
          <TransitionChild
            as="template"
            enter="ease-out duration-300"
            enter-from="opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95"
            enter-to="opacity-100 translate-y-0 sm:scale-100"
            leave="ease-in duration-200"
            leave-from="opacity-100 translate-y-0 sm:scale-100"
            leave-to="opacity-0 translate-y-4 sm:translate-y-0 sm:scale-95"
          >
            <DialogPanel
              class="relative transform overflow-hidden rounded-lg bg-white shadow-xl transition-all sm:my-8 sm:w-full sm:max-w-4xl"
            >
              <!-- Header -->
              <div class="bg-white px-6 py-4 border-b border-gray-200">
                <div class="flex items-center justify-between">
                  <div>
                    <DialogTitle as="h3" class="text-xl font-semibold text-gray-900">
                      {{ opportunity.name }}
                    </DialogTitle>
                    <p class="text-sm text-gray-600 mt-1">{{ opportunity.customerName }}</p>
                  </div>
                  <div class="flex items-center space-x-3">
                    <span :class="['px-3 py-1 rounded-full text-sm font-medium', getStageColor(opportunity.stage)]">
                      {{ opportunity.stageName }}
                    </span>
                    <button
                      @click="$emit('close')"
                      class="text-gray-400 hover:text-gray-600 transition-colors"
                    >
                      <XMarkIcon class="w-6 h-6" />
                    </button>
                  </div>
                </div>
              </div>

              <!-- Content -->
              <div class="max-h-96 overflow-y-auto">
                <div class="px-6 py-4">
                  <!-- Key Metrics -->
                  <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
                    <div class="bg-blue-50 rounded-lg p-4">
                      <div class="flex items-center justify-between">
                        <div>
                          <p class="text-sm font-medium text-blue-600">Estimated Value</p>
                          <p class="text-2xl font-bold text-blue-900">{{ formatCurrency(opportunity.estimatedValue) }}</p>
                        </div>
                        <CurrencyDollarIcon class="w-8 h-8 text-blue-600" />
                      </div>
                    </div>

                    <div class="bg-green-50 rounded-lg p-4">
                      <div class="flex items-center justify-between">
                        <div>
                          <p class="text-sm font-medium text-green-600">Weighted Value</p>
                          <p class="text-2xl font-bold text-green-900">{{ formatCurrency(opportunity.weightedValue) }}</p>
                          <p class="text-sm text-green-600">{{ opportunity.probability }}% probability</p>
                        </div>
                        <ArrowTrendingUpIcon class="w-8 h-8 text-green-600" />
                      </div>
                    </div>

                    <div class="bg-purple-50 rounded-lg p-4">
                      <div class="flex items-center justify-between">
                        <div>
                          <p class="text-sm font-medium text-purple-600">Expected Close</p>
                          <p class="text-lg font-bold text-purple-900">{{ formatDate(opportunity.expectedCloseDate) }}</p>
                          <p :class="['text-sm', getDateColor()]">{{ getDateStatus() }}</p>
                        </div>
                        <CalendarIcon class="w-8 h-8 text-purple-600" />
                      </div>
                    </div>
                  </div>

                  <!-- Opportunity Details -->
                  <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
                    <!-- Left Column -->
                    <div>
                      <h4 class="text-lg font-semibold text-gray-900 mb-4">Opportunity Details</h4>
                      
                      <div class="space-y-4">
                        <div>
                          <label class="text-sm font-medium text-gray-700">Type</label>
                          <p class="text-sm text-gray-900">{{ opportunity.type }}</p>
                        </div>

                        <div>
                          <label class="text-sm font-medium text-gray-700">Priority</label>
                          <div class="flex items-center mt-1">
                            <div :class="['w-2 h-2 rounded-full mr-2', getPriorityColor(opportunity.priority)]"></div>
                            <span class="text-sm text-gray-900">{{ opportunity.priority }}</span>
                          </div>
                        </div>

                        <div v-if="opportunity.assignedTo">
                          <label class="text-sm font-medium text-gray-700">Assigned To</label>
                          <p class="text-sm text-gray-900">{{ opportunity.assignedTo }}</p>
                        </div>

                        <div v-if="opportunity.source">
                          <label class="text-sm font-medium text-gray-700">Source</label>
                          <p class="text-sm text-gray-900">{{ opportunity.source }}</p>
                        </div>

                        <div>
                          <label class="text-sm font-medium text-gray-700">Days in Pipeline</label>
                          <p class="text-sm text-gray-900">{{ opportunity.daysInPipeline }} days</p>
                        </div>

                        <div v-if="opportunity.daysSinceLastActivity !== null">
                          <label class="text-sm font-medium text-gray-700">Last Activity</label>
                          <p class="text-sm text-gray-900">{{ opportunity.daysSinceLastActivity }} days ago</p>
                        </div>
                      </div>
                    </div>

                    <!-- Right Column -->
                    <div>
                      <h4 class="text-lg font-semibold text-gray-900 mb-4">Status & Progress</h4>
                      
                      <div class="space-y-4">
                        <!-- Progress Indicators -->
                        <div v-if="opportunity.isOverdue" class="bg-red-50 border border-red-200 rounded-lg p-3">
                          <div class="flex items-center">
                            <ExclamationTriangleIcon class="w-5 h-5 text-red-600 mr-2" />
                            <span class="text-sm font-medium text-red-800">Overdue</span>
                          </div>
                          <p class="text-sm text-red-600 mt-1">Expected close date has passed</p>
                        </div>

                        <div v-else-if="opportunity.isClosingSoon" class="bg-yellow-50 border border-yellow-200 rounded-lg p-3">
                          <div class="flex items-center">
                            <ClockIcon class="w-5 h-5 text-yellow-600 mr-2" />
                            <span class="text-sm font-medium text-yellow-800">Closing Soon</span>
                          </div>
                          <p class="text-sm text-yellow-600 mt-1">Expected to close within 30 days</p>
                        </div>

                        <div v-if="opportunity.isHighPriority" class="bg-orange-50 border border-orange-200 rounded-lg p-3">
                          <div class="flex items-center">
                            <StarIcon class="w-5 h-5 text-orange-600 mr-2" />
                            <span class="text-sm font-medium text-orange-800">High Priority</span>
                          </div>
                          <p class="text-sm text-orange-600 mt-1">Requires immediate attention</p>
                        </div>

                        <!-- Stage Progress -->
                        <div>
                          <label class="text-sm font-medium text-gray-700 mb-2 block">Stage Progress</label>
                          <div class="flex items-center space-x-2">
                            <div
                              v-for="stage in allStages"
                              :key="stage"
                              :class="[
                                'flex-1 h-2 rounded-full',
                                getStageIndex(opportunity.stage) >= getStageIndex(stage)
                                  ? 'bg-blue-500'
                                  : 'bg-gray-200'
                              ]"
                            ></div>
                          </div>
                          <div class="flex justify-between text-xs text-gray-500 mt-1">
                            <span>Start</span>
                            <span>Close</span>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Footer -->
              <div class="bg-gray-50 px-6 py-4 flex items-center justify-between">
                <div class="flex items-center space-x-4">
                  <button
                    @click="editOpportunity"
                    class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
                  >
                    Edit Opportunity
                  </button>
                  <button
                    @click="scheduleActivity"
                    class="bg-gray-600 text-white px-4 py-2 rounded-lg hover:bg-gray-700 transition-colors"
                  >
                    Schedule Activity
                  </button>
                </div>
                <div class="flex items-center space-x-4">
                  <button
                    @click="moveToNextStage"
                    class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors"
                  >
                    Advance Stage
                  </button>
                  <button
                    @click="$emit('close')"
                    class="bg-gray-300 text-gray-700 px-4 py-2 rounded-lg hover:bg-gray-400 transition-colors"
                  >
                    Close
                  </button>
                </div>
              </div>
            </DialogPanel>
          </TransitionChild>
        </div>
      </div>
    </Dialog>
  </TransitionRoot>
</template>

<script setup lang="ts">
import { Dialog, DialogPanel, DialogTitle, TransitionChild, TransitionRoot } from '@headlessui/vue'
import {
  XMarkIcon,
  CurrencyDollarIcon,
  ArrowTrendingUpIcon,
  CalendarIcon,
  ExclamationTriangleIcon,
  ClockIcon,
  StarIcon
} from '@heroicons/vue/24/outline'

interface Opportunity {
  id: string
  name: string
  customerName: string
  stage: string
  stageName: string
  type: string
  priority: string
  estimatedValue: number
  currency: string
  probability: number
  weightedValue: number
  expectedCloseDate: string
  assignedTo?: string
  isOverdue: boolean
  isClosingSoon: boolean
  isHighPriority: boolean
  daysInPipeline: number
  daysSinceLastActivity: number | null
  source?: string
}

interface Props {
  opportunity: Opportunity
}

defineProps<Props>()

defineEmits<{
  close: []
  updated: [opportunity: Opportunity]
}>()

const allStages = ['Prospecting', 'Qualification', 'NeedsAnalysis', 'Proposal', 'Negotiation', 'ClosedWon']

function formatCurrency(value: number): string {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(value)
}

function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

function getDateColor(): string {
  const props = defineProps<Props>()
  if (props.opportunity.isOverdue) {
    return 'text-red-600'
  } else if (props.opportunity.isClosingSoon) {
    return 'text-yellow-600'
  }
  return 'text-gray-600'
}

function getDateStatus(): string {
  const props = defineProps<Props>()
  const date = new Date(props.opportunity.expectedCloseDate)
  const now = new Date()
  const diffTime = date.getTime() - now.getTime()
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))

  if (diffDays < 0) {
    return `${Math.abs(diffDays)} days overdue`
  } else if (diffDays === 0) {
    return 'Due today'
  } else if (diffDays <= 30) {
    return `${diffDays} days remaining`
  } else {
    return `${diffDays} days remaining`
  }
}

function getPriorityColor(priority: string): string {
  switch (priority) {
    case 'High':
      return 'bg-red-500'
    case 'Medium':
      return 'bg-yellow-500'
    case 'Low':
      return 'bg-green-500'
    default:
      return 'bg-gray-500'
  }
}

function getStageColor(stage: string): string {
  switch (stage) {
    case 'Prospecting':
      return 'bg-blue-100 text-blue-800'
    case 'Qualification':
      return 'bg-yellow-100 text-yellow-800'
    case 'NeedsAnalysis':
      return 'bg-orange-100 text-orange-800'
    case 'Proposal':
      return 'bg-purple-100 text-purple-800'
    case 'Negotiation':
      return 'bg-indigo-100 text-indigo-800'
    case 'ClosedWon':
      return 'bg-green-100 text-green-800'
    case 'ClosedLost':
      return 'bg-red-100 text-red-800'
    default:
      return 'bg-gray-100 text-gray-800'
  }
}

function getStageIndex(stage: string): number {
  return allStages.indexOf(stage)
}

function editOpportunity() {
  // TODO: Implement edit functionality
  console.log('Edit opportunity')
}

function scheduleActivity() {
  // TODO: Implement schedule activity functionality
  console.log('Schedule activity')
}

function moveToNextStage() {
  // TODO: Implement move to next stage functionality
  console.log('Move to next stage')
}
</script>
