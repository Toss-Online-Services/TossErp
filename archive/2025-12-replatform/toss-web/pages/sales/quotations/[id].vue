<template>
  <div class="container mx-auto px-4 py-8 max-w-5xl">
    <!-- Loading State -->
    <div v-if="loading" class="text-center py-12">
      <Icon name="mdi:loading" class="animate-spin text-4xl text-blue-600 mb-4" />
      <p class="text-gray-600">{{ t('common.loading') }}</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="text-center py-12">
      <Icon name="mdi:alert-circle" class="text-4xl text-red-600 mb-4" />
      <p class="text-red-600 mb-4">{{ error }}</p>
      <button @click="navigateTo('/sales/quotations')" class="btn btn-secondary">
        {{ t('common.back') }}
      </button>
    </div>

    <!-- Quotation Content -->
    <div v-else-if="quotation">
      <!-- Header with Actions -->
      <div class="flex items-start justify-between mb-8">
        <div>
          <div class="flex items-center gap-2 mb-2">
            <NuxtLink to="/sales/quotations" class="text-gray-600 hover:text-gray-900">
              <Icon name="mdi:arrow-left" size="24" />
            </NuxtLink>
            <h1 class="text-3xl font-bold">{{ quotation.quotationNo }}</h1>
            <span :class="getStatusClass(quotation.status)" class="px-3 py-1 text-sm font-semibold rounded-full">
              {{ t(`sales.quotations.${quotation.status}`) }}
            </span>
          </div>
          <p class="text-gray-600">{{ t('sales.quotations.createdOn') }} {{ formatDate(quotation.createdAt) }}</p>
        </div>

        <!-- Action Buttons -->
        <div class="flex gap-2">
          <button
            v-if="quotation.status === 'draft' || quotation.status === 'sent'"
            @click="editQuotation"
            class="btn btn-secondary flex items-center gap-2"
          >
            <Icon name="mdi:pencil" />
            {{ t('common.edit') }}
          </button>
          
          <button
            v-if="quotation.status !== 'accepted' && quotation.status !== 'rejected'"
            @click="downloadPDF"
            class="btn btn-secondary flex items-center gap-2"
          >
            <Icon name="mdi:file-pdf-box" />
            {{ t('sales.quotations.viewPdf') }}
          </button>
          
          <button
            v-if="quotation.status === 'draft'"
            @click="sendEmail"
            class="btn btn-primary flex items-center gap-2"
          >
            <Icon name="mdi:email-send" />
            {{ t('sales.quotations.sendEmail') }}
          </button>
          
          <button
            v-if="quotation.status === 'accepted'"
            @click="convertToOrder"
            class="btn btn-primary flex items-center gap-2"
          >
            <Icon name="mdi:swap-horizontal" />
            {{ t('sales.quotations.convertToOrder') }}
          </button>
        </div>
      </div>

      <!-- Quotation Details Card -->
      <div class="bg-white rounded-lg shadow-sm mb-6">
        <!-- Header Section -->
        <div class="p-8 border-b">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
            <!-- From (Company) -->
            <div>
              <h3 class="text-sm font-semibold text-gray-500 uppercase mb-2">{{ t('sales.quotations.from') }}</h3>
              <div class="text-lg font-bold mb-1">TOSS Online Services</div>
              <div class="text-gray-600 text-sm">
                <p>123 Township Street</p>
                <p>Johannesburg, Gauteng 2000</p>
                <p>South Africa</p>
                <p class="mt-2">
                  <Icon name="mdi:phone" class="inline mr-1" />
                  +27 11 123 4567
                </p>
                <p>
                  <Icon name="mdi:email" class="inline mr-1" />
                  sales@toss.co.za
                </p>
              </div>
            </div>

            <!-- To (Customer) -->
            <div>
              <h3 class="text-sm font-semibold text-gray-500 uppercase mb-2">{{ t('sales.quotations.to') }}</h3>
              <div class="text-lg font-bold mb-1">{{ quotation.customerName }}</div>
              <div class="text-gray-600 text-sm">
                <p v-if="quotation.customer.businessName">{{ quotation.customer.businessName }}</p>
                <p>{{ quotation.customer.address }}</p>
                <p class="mt-2">
                  <Icon name="mdi:phone" class="inline mr-1" />
                  {{ quotation.customer.phone }}
                </p>
                <p v-if="quotation.customer.email">
                  <Icon name="mdi:email" class="inline mr-1" />
                  {{ quotation.customer.email }}
                </p>
              </div>
            </div>
          </div>

          <!-- Quotation Meta -->
          <div class="grid grid-cols-2 md:grid-cols-4 gap-4 mt-8 pt-6 border-t">
            <div>
              <div class="text-xs text-gray-500 uppercase">{{ t('sales.quotations.quotationNo') }}</div>
              <div class="font-medium">{{ quotation.quotationNo }}</div>
            </div>
            <div>
              <div class="text-xs text-gray-500 uppercase">{{ t('sales.quotations.date') }}</div>
              <div class="font-medium">{{ formatDate(quotation.quotationDate) }}</div>
            </div>
            <div>
              <div class="text-xs text-gray-500 uppercase">{{ t('sales.quotations.validUntil') }}</div>
              <div class="font-medium">{{ formatDate(quotation.validUntil) }}</div>
            </div>
            <div>
              <div class="text-xs text-gray-500 uppercase">{{ t('sales.quotations.priceList') }}</div>
              <div class="font-medium capitalize">{{ quotation.priceList }}</div>
            </div>
          </div>
        </div>

        <!-- Line Items -->
        <div class="p-8">
          <h3 class="text-lg font-semibold mb-4">{{ t('sales.quotations.items') }}</h3>
          <div class="overflow-x-auto">
            <table class="min-w-full">
              <thead class="border-b-2 border-gray-200">
                <tr>
                  <th class="text-left py-3 px-2 text-sm font-semibold text-gray-600">#</th>
                  <th class="text-left py-3 px-4 text-sm font-semibold text-gray-600">{{ t('sales.quotations.product') }}</th>
                  <th class="text-right py-3 px-4 text-sm font-semibold text-gray-600">{{ t('sales.quotations.quantity') }}</th>
                  <th class="text-right py-3 px-4 text-sm font-semibold text-gray-600">{{ t('sales.quotations.rate') }}</th>
                  <th class="text-right py-3 px-4 text-sm font-semibold text-gray-600">{{ t('sales.quotations.discount') }}</th>
                  <th class="text-right py-3 px-4 text-sm font-semibold text-gray-600">{{ t('sales.quotations.amount') }}</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(item, index) in quotation.items" :key="item.id" class="border-b border-gray-100">
                  <td class="py-3 px-2 text-gray-600">{{ index + 1 }}</td>
                  <td class="py-3 px-4">
                    <div class="font-medium">{{ item.productName }}</div>
                    <div class="text-sm text-gray-500">{{ item.description }}</div>
                  </td>
                  <td class="py-3 px-4 text-right">{{ formatNumber(item.quantity) }}</td>
                  <td class="py-3 px-4 text-right">R{{ formatCurrency(item.rate) }}</td>
                  <td class="py-3 px-4 text-right">{{ item.discountPercent }}%</td>
                  <td class="py-3 px-4 text-right font-medium">R{{ formatCurrency(item.amount) }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Totals -->
        <div class="px-8 pb-8">
          <div class="flex justify-end">
            <div class="w-full md:w-1/2 lg:w-1/3 space-y-2">
              <div class="flex justify-between py-2 border-b">
                <span class="text-gray-600">{{ t('sales.quotations.subtotal') }}</span>
                <span class="font-medium">R{{ formatCurrency(quotation.subtotal) }}</span>
              </div>
              
              <div v-if="quotation.discountAmount > 0" class="flex justify-between py-2 border-b">
                <span class="text-gray-600">{{ t('sales.quotations.discount') }}</span>
                <span class="font-medium text-red-600">-R{{ formatCurrency(quotation.discountAmount) }}</span>
              </div>
              
              <div class="flex justify-between py-2 border-b">
                <span class="text-gray-600">{{ t('sales.quotations.taxableAmount') }}</span>
                <span class="font-medium">R{{ formatCurrency(quotation.taxableAmount) }}</span>
              </div>
              
              <div class="flex justify-between py-2 border-b">
                <span class="text-gray-600">{{ t('sales.quotations.vat') }} (15%)</span>
                <span class="font-medium">R{{ formatCurrency(quotation.taxAmount) }}</span>
              </div>
              
              <div class="flex justify-between py-3 border-t-2 border-gray-300">
                <span class="text-lg font-bold">{{ t('sales.quotations.grandTotal') }}</span>
                <span class="text-2xl font-bold text-blue-600">R{{ formatCurrency(quotation.grandTotal) }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Terms and Notes -->
        <div v-if="quotation.termsAndConditions || quotation.notes" class="px-8 pb-8 border-t">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-8 mt-8">
            <div v-if="quotation.termsAndConditions">
              <h3 class="text-sm font-semibold text-gray-500 uppercase mb-2">
                {{ t('sales.quotations.termsAndConditions') }}
              </h3>
              <p class="text-sm text-gray-600 whitespace-pre-line">{{ quotation.termsAndConditions }}</p>
            </div>
            
            <div v-if="quotation.notes">
              <h3 class="text-sm font-semibold text-gray-500 uppercase mb-2">
                {{ t('sales.quotations.notes') }}
              </h3>
              <p class="text-sm text-gray-600 whitespace-pre-line">{{ quotation.notes }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Activity Timeline -->
      <div class="bg-white rounded-lg shadow-sm p-6">
        <h3 class="text-lg font-semibold mb-4 flex items-center gap-2">
          <Icon name="mdi:history" />
          {{ t('sales.quotations.activityTimeline') }}
        </h3>
        
        <div class="space-y-4">
          <div v-for="activity in quotation.activities" :key="activity.id" class="flex gap-4">
            <div class="flex-shrink-0">
              <div class="w-10 h-10 rounded-full bg-blue-100 flex items-center justify-center">
                <Icon :name="getActivityIcon(activity.type)" class="text-blue-600" />
              </div>
            </div>
            <div class="flex-1">
              <div class="flex items-center gap-2 mb-1">
                <span class="font-medium">{{ activity.title }}</span>
                <span class="text-sm text-gray-500">{{ formatDateTime(activity.timestamp) }}</span>
              </div>
              <p class="text-sm text-gray-600">{{ activity.description }}</p>
              <p v-if="activity.user" class="text-xs text-gray-500 mt-1">{{ t('sales.quotations.by') }} {{ activity.user }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRoute, useRouter, useNuxtApp } from '#app'
import type { QuotationRecord } from '../../../types/sales'
import { generateQuotationPdf } from '../../../utils/pdf/quotation'

const { t } = useI18n()
const route = useRoute()
const router = useRouter()
const toast = useToast()
const nuxtApp = useNuxtApp()

// State
const loading = ref(true)
const error = ref('')
const quotation = ref<QuotationRecord | null>(null)

// Fetch quotation on mount
onMounted(async () => {
  await fetchQuotation()
})

const fetchQuotation = async () => {
  try {
    loading.value = true
    error.value = ''
    const id = route.params.id as string
    
    const response = await nuxtApp.$fetch<{ data: QuotationRecord }>(`/api/sales/quotations/${id}`)
    quotation.value = response.data
    
    loading.value = false
  } catch (err: any) {
    error.value = err.message || t('sales.quotations.errorLoading')
    loading.value = false
    console.error('Error loading quotation:', err)
  }
}

// Methods
const getStatusClass = (status: string) => {
  const classes = {
    'draft': 'bg-gray-100 text-gray-800',
    'sent': 'bg-blue-100 text-blue-800',
    'accepted': 'bg-green-100 text-green-800',
    'rejected': 'bg-red-100 text-red-800',
    'expired': 'bg-orange-100 text-orange-800'
  }
  return classes[status] || 'bg-gray-100 text-gray-800'
}

const getActivityIcon = (type: string) => {
  const icons = {
    'created': 'mdi:file-document-plus',
    'sent': 'mdi:email-send',
    'accepted': 'mdi:check-circle',
    'rejected': 'mdi:close-circle',
    'converted': 'mdi:swap-horizontal',
    'updated': 'mdi:pencil'
  }
  return icons[type] || 'mdi:circle'
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('en-ZA', { 
    day: 'numeric',
    month: 'long',
    year: 'numeric'
  })
}

const formatDateTime = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleString('en-ZA', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const formatCurrency = (amount: number): string => {
  return amount.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',')
}

const formatNumber = (num: number): string => {
  return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',')
}

const editQuotation = () => {
  router.push(`/sales/quotations/${quotation.value?.id}/edit`)
}

const downloadPDF = async () => {
  if (!quotation.value) return
  
  try {
    await generateQuotationPdf(quotation.value)
    toast.add({
      title: t('sales.quotations.pdfGenerated'),
      color: 'green'
    })
  } catch (err) {
    console.error('Error generating PDF:', err)
    toast.add({
      title: t('sales.quotations.errorGeneratingPdf'),
      color: 'red'
    })
  }
}

const sendEmail = async () => {
  if (!quotation.value) return
  
  try {
    await nuxtApp.$fetch(`/api/sales/quotations/${quotation.value.id}/status`, {
      method: 'PATCH',
      body: { status: 'sent' }
    })
    
    quotation.value.status = 'sent'
    quotation.value.activities.push({
      id: String(quotation.value.activities.length + 1),
      type: 'sent',
      title: 'Quotation Sent',
      description: 'Quotation was sent to customer via email',
      user: 'Current User',
      timestamp: new Date().toISOString()
    })
    
    toast.add({
      title: t('sales.quotations.emailSent'),
      color: 'green'
    })
  } catch (err) {
    console.error('Error sending email:', err)
    toast.add({
      title: t('sales.quotations.errorSending'),
      color: 'red'
    })
  }
}

const convertToOrder = async () => {
  if (!quotation.value) return
  
  if (!confirm(t('sales.quotations.confirmConvertToOrder'))) {
    return
  }
  
  try {
    const response = await nuxtApp.$fetch<{ data: { orderId: string } }>(
      `/api/sales/quotations/${quotation.value.id}/convert`,
      { method: 'POST' }
    )
    
    toast.add({
      title: t('sales.quotations.convertedToOrder'),
      color: 'green'
    })
    
    // Navigate to the new order
    router.push(`/sales/orders/${response.data.orderId}`)
  } catch (err) {
    console.error('Error converting to order:', err)
    toast.add({
      title: t('sales.quotations.errorConverting'),
      color: 'red'
    })
  }
}

// Page meta
definePageMeta({
  layout: 'default',
  middleware: 'auth'
})

// SEO
useHead({
  title: computed(() => quotation.value ? quotation.value.quotationNumber : t('sales.quotations.title'))
})
</script>

<style scoped>
.btn {
  @apply px-4 py-2 rounded-lg font-medium transition-colors;
}

.btn-primary {
  @apply bg-blue-600 text-white hover:bg-blue-700;
}

.btn-secondary {
  @apply bg-gray-200 text-gray-800 hover:bg-gray-300;
}
</style>
