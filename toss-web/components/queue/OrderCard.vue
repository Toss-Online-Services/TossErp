<template>
  <div 
    :class="[
      'bg-white rounded-xl shadow-lg p-6 border-l-4 transition-all duration-200 hover:shadow-xl',
      statusColorClass
    ]"
  >
    <!-- Order Header -->
    <div class="flex items-start justify-between mb-4">
      <div class="flex-1">
        <h3 class="text-lg font-bold text-gray-900">{{ order.saleNumber }}</h3>
        <div class="flex items-center text-sm text-gray-600 mt-1">
          <UserIcon class="w-4 h-4 mr-1" />
          {{ order.customerName }}
        </div>
        <div v-if="order.customerPhone" class="flex items-center text-sm text-gray-600 mt-1">
          <PhoneIcon class="w-4 h-4 mr-1" />
          {{ order.customerPhone }}
        </div>
      </div>
      <span 
        :class="[
          'px-3 py-1 rounded-full text-xs font-semibold',
          statusBadgeClass
        ]"
      >
        {{ statusLabel }}
      </span>
    </div>

    <!-- Order Items -->
    <div class="mb-4">
      <p class="text-sm font-medium text-gray-700 mb-2">Items:</p>
      <div class="space-y-1">
        <div 
          v-for="item in order.items" 
          :key="item.id"
          class="flex justify-between text-sm"
        >
          <span class="text-gray-600">{{ item.quantity }}x {{ item.productName }}</span>
          <span class="font-medium text-gray-900">R{{ item.lineTotal.toFixed(2) }}</span>
        </div>
      </div>
    </div>

    <!-- Customer Notes -->
    <div v-if="order.customerNotes" class="mb-4 p-3 bg-amber-50 rounded-lg border border-amber-200">
      <p class="text-xs font-medium text-amber-800 mb-1"> Special Instructions:</p>
      <p class="text-sm text-amber-700">{{ order.customerNotes }}</p>
    </div>

    <!-- Order Footer -->
    <div class="flex items-center justify-between pt-4 border-t border-gray-200">
      <div>
        <p class="text-sm text-gray-600">Total</p>
        <p class="text-xl font-bold text-gray-900">R{{ order.total.toFixed(2) }}</p>
      </div>
      
      <!-- Expected Time -->
      <div v-if="order.expectedCompletionTime" class="text-right">
        <p class="text-xs text-gray-500">Expected</p>
        <p class="text-sm font-semibold text-gray-700">
          {{ formatTime(order.expectedCompletionTime) }}
        </p>
      </div>
    </div>

    <!-- Action Button -->
    <button
      v-if="actionButton"
      @click="handleAction"
      :class="[
        'w-full mt-4 py-3 rounded-lg font-semibold shadow-lg hover:shadow-xl transition-all duration-200',
        actionButton.class
      ]"
    >
      {{ actionButton.label }}
    </button>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { UserIcon, PhoneIcon } from '@heroicons/vue/24/outline'

const props = defineProps<{
  order: any
  statusColor: 'yellow' | 'blue' | 'green'
}>()

const emit = defineEmits<{
  'start-preparation': [orderId: number]
  'mark-ready': [orderId: number]
  'complete-order': [orderId: number]
}>()

// Status-based classes
const statusColorClass = computed(() => {
  const colors = {
    yellow: 'border-yellow-500',
    blue: 'border-blue-500',
    green: 'border-green-500'
  }
  return colors[props.statusColor]
})

const statusBadgeClass = computed(() => {
  const classes = {
    yellow: 'bg-yellow-100 text-yellow-800',
    blue: 'bg-blue-100 text-blue-800',
    green: 'bg-green-100 text-green-800'
  }
  return classes[props.statusColor]
})

const statusLabel = computed(() => {
  const labels = {
    Pending: 'Pending',
    InProgress: 'In Progress',
    Ready: 'Ready'
  }
  return labels[props.order.status as keyof typeof labels] || props.order.status
})

// Action button configuration
const actionButton = computed(() => {
  if (props.order.status === 'Pending') {
    return {
      label: ' Start Preparation',
      class: 'bg-blue-600 hover:bg-blue-700 text-white'
    }
  } else if (props.order.status === 'InProgress') {
    return {
      label: ' Mark as Ready',
      class: 'bg-green-600 hover:bg-green-700 text-white'
    }
  } else if (props.order.status === 'Ready') {
    return {
      label: ' Complete & Pay',
      class: 'bg-gradient-to-r from-green-600 to-emerald-600 hover:from-green-700 hover:to-emerald-700 text-white'
    }
  }
  return null
})

// Handle button action
const handleAction = () => {
  if (props.order.status === 'Pending') {
    emit('start-preparation', props.order.id)
  } else if (props.order.status === 'InProgress') {
    emit('mark-ready', props.order.id)
  } else if (props.order.status === 'Ready') {
    emit('complete-order', props.order.id)
  }
}

// Format time
const formatTime = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleTimeString('en-ZA', { hour: '2-digit', minute: '2-digit' })
}
</script>

<style scoped>
/* Add any custom styles here */
</style>
