<template>
  <div
    class="bg-white rounded-lg shadow-sm border border-gray-200 p-4 hover:shadow-md transition-shadow cursor-pointer"
    :class="{
      'border-red-200 bg-red-50': opportunity.isOverdue,
      'border-yellow-200 bg-yellow-50': opportunity.isClosingSoon && !opportunity.isOverdue,
      'border-blue-200': opportunity.isHighPriority && !opportunity.isOverdue && !opportunity.isClosingSoon
    }"
    @click="$emit('click')"
  >
    <!-- Priority and Value Header -->
    <div class="flex items-center justify-between mb-3">
      <div class="flex items-center space-x-2">
        <div
          :class="[
            'w-2 h-2 rounded-full',
            getPriorityColor(opportunity.priority)
          ]"
        ></div>
        <span :class="['text-xs px-2 py-1 rounded-full', getPriorityBadgeColor(opportunity.priority)]">
          {{ opportunity.priority }}
        </span>
      </div>
      <div class="text-right">
        <p class="text-lg font-bold text-gray-900">{{ formatCurrency(opportunity.estimatedValue) }}</p>
        <p class="text-xs text-gray-500">{{ opportunity.probability }}% prob</p>
      </div>
    </div>

    <!-- Opportunity Name and Customer -->
    <div class="mb-3">
      <h4 class="font-semibold text-gray-900 text-sm leading-tight mb-1">
        {{ opportunity.name }}
      </h4>
      <p class="text-gray-600 text-xs">{{ opportunity.customerName }}</p>
    </div>

    <!-- Weighted Value and Type -->
    <div class="flex items-center justify-between mb-3">
      <span class="text-xs text-gray-500">
        Weighted: {{ formatCurrency(opportunity.weightedValue) }}
      </span>
      <span class="text-xs text-gray-500 bg-gray-100 px-2 py-1 rounded">
        {{ opportunity.type }}
      </span>
    </div>

    <!-- Expected Close Date -->
    <div class="flex items-center mb-3">
      <CalendarIcon class="w-4 h-4 text-gray-400 mr-2" />
      <span
        :class="[
          'text-xs',
          {
            'text-red-600 font-medium': opportunity.isOverdue,
            'text-yellow-600 font-medium': opportunity.isClosingSoon && !opportunity.isOverdue,
            'text-gray-600': !opportunity.isOverdue && !opportunity.isClosingSoon
          }
        ]"
      >
        {{ formatDate(opportunity.expectedCloseDate) }}
      </span>
      <span v-if="opportunity.isOverdue" class="ml-2 text-xs text-red-600 font-medium">
        OVERDUE
      </span>
      <span v-else-if="opportunity.isClosingSoon" class="ml-2 text-xs text-yellow-600 font-medium">
        CLOSING SOON
      </span>
    </div>

    <!-- Assigned To -->
    <div v-if="opportunity.assignedTo" class="flex items-center mb-3">
      <UserIcon class="w-4 h-4 text-gray-400 mr-2" />
      <span class="text-xs text-gray-600">{{ opportunity.assignedTo }}</span>
    </div>

    <!-- Source Badge -->
    <div v-if="opportunity.source" class="flex items-center mb-3">
      <TagIcon class="w-4 h-4 text-gray-400 mr-2" />
      <span class="text-xs text-gray-600">{{ opportunity.source }}</span>
    </div>

    <!-- Progress Indicators -->
    <div class="flex items-center justify-between">
      <div class="flex items-center space-x-2">
        <!-- Days in Pipeline -->
        <div class="flex items-center">
          <ClockIcon class="w-3 h-3 text-gray-400 mr-1" />
          <span class="text-xs text-gray-500">{{ opportunity.daysInPipeline }}d</span>
        </div>

        <!-- Last Activity -->
        <div v-if="opportunity.daysSinceLastActivity !== null" class="flex items-center">
          <div
            :class="[
              'w-2 h-2 rounded-full mr-1',
              opportunity.daysSinceLastActivity <= 7 ? 'bg-green-400' :
              opportunity.daysSinceLastActivity <= 14 ? 'bg-yellow-400' : 'bg-red-400'
            ]"
          ></div>
          <span class="text-xs text-gray-500">{{ opportunity.daysSinceLastActivity }}d ago</span>
        </div>
      </div>

      <!-- Action Menu -->
      <div class="flex items-center space-x-1">
        <button
          @click.stop="$emit('edit')"
          class="p-1 text-gray-400 hover:text-blue-600 transition-colors"
          title="Edit Opportunity"
        >
          <PencilIcon class="w-4 h-4" />
        </button>
        <button
          @click.stop="$emit('delete')"
          class="p-1 text-gray-400 hover:text-red-600 transition-colors"
          title="Delete Opportunity"
        >
          <TrashIcon class="w-4 h-4" />
        </button>
      </div>
    </div>

    <!-- High Priority Alert -->
    <div v-if="opportunity.isHighPriority" class="mt-3 pt-3 border-t border-gray-200">
      <div class="flex items-center text-orange-600">
        <ExclamationTriangleIcon class="w-4 h-4 mr-2" />
        <span class="text-xs font-medium">High Priority</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import {
  CalendarIcon,
  UserIcon,
  TagIcon,
  ClockIcon,
  PencilIcon,
  TrashIcon,
  ExclamationTriangleIcon
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
  click: []
  edit: []
  delete: []
}>()

function formatCurrency(value: number): string {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(value)
}

function formatDate(dateString: string): string {
  const date = new Date(dateString)
  const now = new Date()
  const diffTime = date.getTime() - now.getTime()
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))

  if (diffDays === 0) {
    return 'Today'
  } else if (diffDays === 1) {
    return 'Tomorrow'
  } else if (diffDays === -1) {
    return 'Yesterday'
  } else if (diffDays > 0 && diffDays <= 7) {
    return `In ${diffDays} days`
  } else if (diffDays < 0 && diffDays >= -7) {
    return `${Math.abs(diffDays)} days ago`
  } else {
    return date.toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric'
    })
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

function getPriorityBadgeColor(priority: string): string {
  switch (priority) {
    case 'High':
      return 'bg-red-100 text-red-800'
    case 'Medium':
      return 'bg-yellow-100 text-yellow-800'
    case 'Low':
      return 'bg-green-100 text-green-800'
    default:
      return 'bg-gray-100 text-gray-800'
  }
}
</script>
