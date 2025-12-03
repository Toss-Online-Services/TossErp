<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useSalesStore, type Quotation } from '~/stores/sales'

definePageMeta({
  layout: 'default',
  ssr: false
})

useHead({
  title: 'Quotation Details - TOSS'
})

const route = useRoute()
const salesStore = useSalesStore()

const quotation = ref<Quotation | null>(null)
const loading = ref(false)

const quotationId = computed(() => {
  const id = route.params.id
  return Array.isArray(id) ? id[0] : String(id)
})

async function loadQuotation() {
  loading.value = true
  try {
    await salesStore.fetchQuotations()
    const id = quotationId.value
    console.log('Loading quotation with ID:', id)
    
    quotation.value = salesStore.getQuotationById(id) || null
    
    if (!quotation.value) {
      console.error('Quotation not found for ID:', id)
    }
  } catch (error) {
    console.error('Failed to load quotation:', error)
  } finally {
    loading.value = false
  }
}

function formatDate(date: Date | undefined) {
  if (!date) return '-'
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

function getStatusColor(status: string) {
  const colors: Record<string, string> = {
    draft: 'text-gray-600 bg-gray-100',
    sent: 'text-blue-600 bg-blue-100',
    accepted: 'text-green-600 bg-green-100',
    rejected: 'text-red-600 bg-red-100',
    expired: 'text-orange-600 bg-orange-100'
  }
  return colors[status] || 'text-gray-600 bg-gray-100'
}

function getStatusLabel(status: string) {
  const labels: Record<string, string> = {
    draft: 'Draft',
    sent: 'Sent',
    accepted: 'Accepted',
    rejected: 'Rejected',
    expired: 'Expired'
  }
  return labels[status] || status
}

onMounted(async () => {
  await loadQuotation()
})
</script>

<template>
  <div class="py-6">
    <div v-if="loading" class="flex items-center justify-center py-12">
      <div class="text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4 animate-spin">refresh</i>
        <p class="text-gray-600">Loading quotation...</p>
      </div>
    </div>

    <div v-else-if="!quotation" class="flex items-center justify-center py-12">
      <div class="text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">error</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">Quotation not found</h3>
        <p class="text-gray-600 mb-6">The quotation you're looking for doesn't exist</p>
        <button
          @click="navigateTo('/sales/quotations')"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">arrow_back</i>
          <span>Back to Quotations</span>
        </button>
      </div>
    </div>

    <div v-else>
      <!-- Header -->
      <div class="mb-6 flex items-center justify-between">
        <div>
          <button
            @click="navigateTo('/sales/quotations')"
            class="inline-flex items-center gap-2 text-gray-600 hover:text-gray-900 mb-4 transition-colors"
          >
            <i class="material-symbols-rounded text-lg">arrow_back</i>
            <span>Back</span>
          </button>
          <h3 class="text-3xl font-bold text-gray-900 mb-2">{{ quotation.quotationNumber }}</h3>
          <p class="text-gray-600 text-sm">Quotation details and items</p>
        </div>
        <div class="flex items-center gap-3">
          <span :class="['px-3 py-1 text-sm font-medium rounded-full', getStatusColor(quotation.status)]">
            {{ getStatusLabel(quotation.status) }}
          </span>
        </div>
      </div>

      <!-- Info Cards -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4">Customer Information</h4>
          <div class="space-y-2">
            <div>
              <p class="text-sm text-gray-500">Customer</p>
              <p class="text-base font-medium text-gray-900">{{ quotation.customerName }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4">Quotation Dates</h4>
          <div class="space-y-2">
            <div>
              <p class="text-sm text-gray-500">Quotation Date</p>
              <p class="text-base font-medium text-gray-900">{{ formatDate(quotation.date) }}</p>
            </div>
            <div>
              <p class="text-sm text-gray-500">Valid Until</p>
              <p class="text-base font-medium text-gray-900">{{ formatDate(quotation.validUntil) }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Items Table -->
      <div class="bg-white rounded-xl shadow-card overflow-hidden mb-6">
        <div class="px-6 py-4 border-b border-gray-200">
          <h4 class="text-lg font-semibold text-gray-900">Quotation Items</h4>
        </div>
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Item</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Quantity</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Rate</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Discount</th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase">Amount</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="item in quotation.items" :key="item.id" class="hover:bg-gray-50">
              <td class="px-6 py-4">
                <div class="text-sm font-medium text-gray-900">{{ item.itemName }}</div>
              </td>
              <td class="px-6 py-4 text-sm text-gray-900">
                {{ item.quantity }}
              </td>
              <td class="px-6 py-4 text-sm text-gray-900">
                R {{ item.rate.toFixed(2) }}
              </td>
              <td class="px-6 py-4 text-sm text-gray-900">
                R {{ item.discount.toFixed(2) }}
              </td>
              <td class="px-6 py-4 text-sm font-medium text-gray-900 text-right">
                R {{ item.amount.toFixed(2) }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Totals -->
      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex justify-end">
          <div class="w-64 space-y-2">
            <div class="flex justify-between text-sm">
              <span class="text-gray-600">Subtotal:</span>
              <span class="text-gray-900">R {{ quotation.subtotal.toFixed(2) }}</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-gray-600">Discount:</span>
              <span class="text-gray-900">R {{ quotation.discount.toFixed(2) }}</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-gray-600">Tax (15%):</span>
              <span class="text-gray-900">R {{ quotation.tax.toFixed(2) }}</span>
            </div>
            <div class="flex justify-between text-lg font-bold border-t border-gray-200 pt-2">
              <span class="text-gray-900">Total:</span>
              <span class="text-gray-900">R {{ quotation.total.toFixed(2) }}</span>
            </div>
          </div>
        </div>
        <div v-if="quotation.notes" class="mt-6 pt-6 border-t border-gray-200">
          <h4 class="text-sm font-medium text-gray-600 mb-2">Notes</h4>
          <p class="text-sm text-gray-900">{{ quotation.notes }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

