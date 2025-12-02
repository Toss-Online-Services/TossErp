<template>
  <div class="relative">
    <!-- Timeline Line -->
    <div class="absolute left-4 top-8 bottom-8 w-0.5 bg-slate-200 dark:bg-slate-700"></div>

    <div class="space-y-6">
      <!-- Event 1: Draft -->
      <div class="relative flex items-start gap-4">
        <div 
          class="flex-shrink-0 w-8 h-8 rounded-full flex items-center justify-center z-10 ring-4 ring-white dark:ring-slate-800"
          :class="isDraft || isSent || isPaid ? 'bg-slate-500' : 'bg-slate-300 dark:bg-slate-600'"
        >
          <DocumentTextIcon class="w-5 h-5 text-white" />
        </div>
        <div class="flex-1 pb-6">
          <div class="bg-white dark:bg-slate-800 rounded-lg p-4 shadow-sm border border-slate-200 dark:border-slate-700">
            <div class="flex items-center justify-between mb-2">
              <h4 class="font-bold text-slate-900 dark:text-white">📝 Draft</h4>
              <span v-if="isDraft || isSent || isPaid" class="text-xs text-slate-500 dark:text-slate-400">
                {{ formatDate(invoiceDate) }}
              </span>
            </div>
            <p class="text-sm text-slate-600 dark:text-slate-400">
              <span v-if="isDraft || isSent || isPaid">Invoice {{ invoiceNumber }} created and saved as draft.</span>
              <span v-else class="italic">Not yet created...</span>
            </p>
          </div>
        </div>
      </div>

      <!-- Event 2: Sent -->
      <div class="relative flex items-start gap-4">
        <div 
          class="flex-shrink-0 w-8 h-8 rounded-full flex items-center justify-center z-10 ring-4 ring-white dark:ring-slate-800"
          :class="isSent || isPaid ? 'bg-purple-500' : 'bg-slate-300 dark:bg-slate-600'"
        >
          <PaperAirplaneIcon class="w-5 h-5 text-white" />
        </div>
        <div class="flex-1 pb-6">
          <div class="bg-white dark:bg-slate-800 rounded-lg p-4 shadow-sm border border-slate-200 dark:border-slate-700">
            <div class="flex items-center justify-between mb-2">
              <h4 class="font-bold text-slate-900 dark:text-white">📧 Sent</h4>
              <span v-if="isSent || isPaid" class="text-xs text-slate-500 dark:text-slate-400">
                {{ formatDate(sentDate) }}
              </span>
            </div>
            <p class="text-sm text-slate-600 dark:text-slate-400">
              <span v-if="isSent || isPaid">Invoice sent to customer via email.</span>
              <span v-else class="italic">Not sent yet...</span>
            </p>
          </div>
        </div>
      </div>

      <!-- Event 3: Viewed (Optional) -->
      <div v-if="isViewed || isPaid" class="relative flex items-start gap-4">
        <div 
          class="flex-shrink-0 w-8 h-8 rounded-full flex items-center justify-center z-10 ring-4 ring-white dark:ring-slate-800"
          :class="isViewed || isPaid ? 'bg-blue-500' : 'bg-slate-300 dark:bg-slate-600'"
        >
          <EyeIcon class="w-5 h-5 text-white" />
        </div>
        <div class="flex-1 pb-6">
          <div class="bg-white dark:bg-slate-800 rounded-lg p-4 shadow-sm border border-slate-200 dark:border-slate-700">
            <div class="flex items-center justify-between mb-2">
              <h4 class="font-bold text-slate-900 dark:text-white">👁️ Viewed</h4>
              <span v-if="isViewed || isPaid" class="text-xs text-slate-500 dark:text-slate-400">
                {{ formatDate(viewedDate) }}
              </span>
            </div>
            <p class="text-sm text-slate-600 dark:text-slate-400">
              Customer opened and viewed the invoice.
            </p>
          </div>
        </div>
      </div>

      <!-- Event 4: Paid / Overdue -->
      <div class="relative flex items-start gap-4">
        <div 
          class="flex-shrink-0 w-8 h-8 rounded-full flex items-center justify-center z-10 ring-4 ring-white dark:ring-slate-800"
          :class="isPaid ? 'bg-green-500' : isOverdue ? 'bg-red-500' : 'bg-slate-300 dark:bg-slate-600'"
        >
          <CheckCircleIcon v-if="isPaid" class="w-5 h-5 text-white" />
          <ExclamationTriangleIcon v-else-if="isOverdue" class="w-5 h-5 text-white" />
          <CurrencyDollarIcon v-else class="w-5 h-5 text-white" />
        </div>
        <div class="flex-1 pb-6">
          <div class="bg-white dark:bg-slate-800 rounded-lg p-4 shadow-sm border border-slate-200 dark:border-slate-700">
            <div class="flex items-center justify-between mb-2">
              <h4 class="font-bold text-slate-900 dark:text-white">
                <span v-if="isPaid">✅ Paid</span>
                <span v-else-if="isOverdue">⚠️ Overdue</span>
                <span v-else>💰 Payment Pending</span>
              </h4>
              <span v-if="isPaid" class="text-xs text-slate-500 dark:text-slate-400">
                {{ formatDate(paidDate) }}
              </span>
              <span v-else-if="dueDate" class="text-xs text-slate-500 dark:text-slate-400">
                Due: {{ formatDate(dueDate) }}
              </span>
            </div>
            <p class="text-sm text-slate-600 dark:text-slate-400">
              <span v-if="isPaid">Payment received and invoice closed.</span>
              <span v-else-if="isOverdue" class="text-red-600 dark:text-red-400">
                Payment is overdue. Please follow up with customer.
              </span>
              <span v-else class="italic">
                Awaiting payment by {{ formatDate(dueDate) }}...
              </span>
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import {
  DocumentTextIcon,
  PaperAirplaneIcon,
  EyeIcon,
  CheckCircleIcon,
  ExclamationTriangleIcon,
  CurrencyDollarIcon
} from '@heroicons/vue/24/outline'

const props = defineProps<{
  status: string
  invoiceNumber: string
  invoiceDate: Date
  dueDate: Date
}>()

// Status checks
const isDraft = computed(() => {
  return ['draft', 'sent', 'viewed', 'paid'].includes(props.status.toLowerCase())
})

const isSent = computed(() => {
  return ['sent', 'viewed', 'paid'].includes(props.status.toLowerCase())
})

const isViewed = computed(() => {
  return ['viewed', 'paid'].includes(props.status.toLowerCase())
})

const isPaid = computed(() => {
  return props.status.toLowerCase() === 'paid'
})

const isOverdue = computed(() => {
  return props.status.toLowerCase() === 'overdue'
})

// Mock dates based on invoice date
const sentDate = computed(() => {
  if (!isSent.value) return null
  const date = new Date(props.invoiceDate)
  date.setHours(date.getHours() + 1)
  return date
})

const viewedDate = computed(() => {
  if (!isViewed.value) return null
  const date = new Date(props.invoiceDate)
  date.setHours(date.getHours() + 2)
  return date
})

const paidDate = computed(() => {
  if (!isPaid.value) return null
  const date = new Date(props.invoiceDate)
  date.setDate(date.getDate() + 15) // Assume paid 15 days after issue
  return date
})

// Format date
const formatDate = (date: Date | null) => {
  if (!date) return ''
  return new Date(date).toLocaleString('en-ZA', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}
</script>

