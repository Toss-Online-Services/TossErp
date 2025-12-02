<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full mx-auto px-4 sm:px-6 lg:px-8 py-6">
        <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between space-y-4 sm:space-y-0">
          <div>
            <h1 class="text-3xl font-bold bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">
              {{ t('sales.quotations.title') }}
            </h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
              {{ t('sales.quotations.subtitle') }}
            </p>
          </div>
          <div class="flex space-x-3">
            <button 
              @click="downloadTemplate"
              class="inline-flex items-center px-4 py-2.5 rounded-xl text-sm font-medium text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-all duration-200"
            >
              <Icon name="mdi:file-download" class="mr-2" />
              {{ t('common.template') }}
            </button>
            <NuxtLink 
              to="/sales/quotations/create"
              class="inline-flex items-center px-4 py-2.5 bg-gradient-to-r from-blue-600 to-purple-600 text-white rounded-xl hover:from-blue-700 hover:to-purple-700 shadow-lg hover:shadow-xl transition-all duration-200 text-sm font-semibold"
            >
              <Icon name="mdi:plus" class="mr-2" />
              {{ t('sales.quotations.newQuotation') }}
            </NuxtLink>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Stats Cards -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-slate-600 dark:text-slate-400">{{ t('sales.quotations.draft') }}</p>
              <p class="text-2xl font-bold text-slate-900 dark:text-white mt-1">{{ stats.draft }}</p>
            </div>
            <Icon name="mdi:file-document-outline" class="w-10 h-10 text-blue-500" />
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-slate-600 dark:text-slate-400">{{ t('sales.quotations.sent') }}</p>
              <p class="text-2xl font-bold text-slate-900 dark:text-white mt-1">{{ stats.sent }}</p>
            </div>
            <Icon name="mdi:email-send" class="w-10 h-10 text-green-500" />
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-slate-600 dark:text-slate-400">{{ t('sales.quotations.accepted') }}</p>
              <p class="text-2xl font-bold text-slate-900 dark:text-white mt-1">{{ stats.accepted }}</p>
            </div>
            <Icon name="mdi:check-circle" class="w-10 h-10 text-purple-500" />
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-slate-600 dark:text-slate-400">{{ t('sales.quotations.expired') }}</p>
              <p class="text-2xl font-bold text-slate-900 dark:text-white mt-1">{{ stats.expired }}</p>
            </div>
            <Icon name="mdi:clock-alert-outline" class="w-10 h-10 text-orange-500" />
          </div>
        </div>
      </div>

      <!-- Filters and Search -->
      <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 mb-8">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div class="relative">
            <Icon name="mdi:magnify" class="absolute left-3 top-1/2 transform -translate-y-1/2 text-slate-400" />
            <input
              v-model="searchQuery"
              type="text"
              :placeholder="t('common.search')"
              class="w-full pl-10 pr-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>

          <select
            v-model="filterStatus"
            class="px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          >
            <option value="">{{ t('sales.quotations.allStatus') }}</option>
            <option value="draft">{{ t('sales.quotations.draft') }}</option>
            <option value="sent">{{ t('sales.quotations.sent') }}</option>
            <option value="accepted">{{ t('sales.quotations.accepted') }}</option>
            <option value="rejected">{{ t('sales.quotations.rejected') }}</option>
            <option value="expired">{{ t('sales.quotations.expired') }}</option>
          </select>

          <input
            v-model="filterStartDate"
            type="date"
            class="px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          />

          <input
            v-model="filterEndDate"
            type="date"
            class="px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          />
        </div>
      </div>

      <!-- Quotations Table -->
      <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 overflow-hidden">
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-slate-200 dark:divide-slate-700">
            <thead class="bg-slate-50 dark:bg-slate-900">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">
                  {{ t('sales.quotations.quotationNo') }}
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">
                  {{ t('sales.quotations.customer') }}
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">
                  {{ t('sales.quotations.date') }}
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">
                  {{ t('sales.quotations.validUntil') }}
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">
                  {{ t('common.total') }}
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">
                  {{ t('common.status') }}
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-slate-500 dark:text-slate-400 uppercase tracking-wider">
                  {{ t('common.actions') }}
                </th>
              </tr>
            </thead>
            <tbody class="bg-white dark:bg-slate-800 divide-y divide-slate-200 dark:divide-slate-700">
              <tr v-for="quotation in filteredQuotations" :key="quotation.id" class="hover:bg-slate-50 dark:hover:bg-slate-700">
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-slate-900 dark:text-white">
                  {{ quotation.quotationNo }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-900 dark:text-white">
                  {{ quotation.customerName }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-600 dark:text-slate-400">
                  {{ formatDate(quotation.date) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-600 dark:text-slate-400">
                  {{ formatDate(quotation.validUntil) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-right font-semibold text-slate-900 dark:text-white">
                  R{{ formatCurrency(quotation.grandTotal) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span :class="getStatusClass(quotation.status)" class="px-2.5 py-1 rounded-full text-xs font-medium">
                    {{ t(`sales.quotations.${quotation.status}`) }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium space-x-2">
                  <button @click="viewQuotation(quotation.id)" class="text-blue-600 hover:text-blue-800">
                    <Icon name="mdi:eye" />
                  </button>
                  <button @click="editQuotation(quotation.id)" class="text-green-600 hover:text-green-800">
                    <Icon name="mdi:pencil" />
                  </button>
                  <button v-if="quotation.status === 'accepted'" @click="convertToOrder(quotation.id)" class="text-purple-600 hover:text-purple-800">
                    <Icon name="mdi:file-document-arrow-right" />
                  </button>
                  <button @click="deleteQuotation(quotation.id)" class="text-red-600 hover:text-red-800">
                    <Icon name="mdi:delete" />
                  </button>
                </td>
              </tr>
              <tr v-if="filteredQuotations.length === 0">
                <td colspan="7" class="px-6 py-12 text-center text-slate-500 dark:text-slate-400">
                  {{ t('sales.quotations.noQuotations') }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from '#i18n'

const { t } = useI18n()

// Page metadata
useHead({
  title: 'Quotations - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage sales quotations and convert them to orders' }
  ]
})

definePageMeta({
  layout: 'default'
})

// State
const searchQuery = ref('')
const filterStatus = ref('')
const filterStartDate = ref('')
const filterEndDate = ref('')

const stats = ref({
  draft: 0,
  sent: 0,
  accepted: 0,
  expired: 0
})

const quotations = ref<any[]>([])

// Computed
const filteredQuotations = computed(() => {
  return quotations.value.filter(q => {
    const matchesSearch = !searchQuery.value || 
      q.quotationNo.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      q.customerName.toLowerCase().includes(searchQuery.value.toLowerCase())
    
    const matchesStatus = !filterStatus.value || q.status === filterStatus.value
    
    const matchesDate = (!filterStartDate.value || new Date(q.date) >= new Date(filterStartDate.value)) &&
      (!filterEndDate.value || new Date(q.date) <= new Date(filterEndDate.value))
    
    return matchesSearch && matchesStatus && matchesDate
  })
})

// Methods
const loadQuotations = async () => {
  // TODO: Load from API
  quotations.value = [
    {
      id: 1,
      quotationNo: 'QTN-2025-001',
      customerName: 'Sipho Dlamini',
      date: '2025-11-01',
      validUntil: '2025-11-30',
      grandTotal: 15000,
      status: 'sent'
    },
    {
      id: 2,
      quotationNo: 'QTN-2025-002',
      customerName: 'Thandi Mkhize',
      date: '2025-11-05',
      validUntil: '2025-12-05',
      grandTotal: 8500,
      status: 'accepted'
    }
  ]
  
  updateStats()
}

const updateStats = () => {
  stats.value = {
    draft: quotations.value.filter(q => q.status === 'draft').length,
    sent: quotations.value.filter(q => q.status === 'sent').length,
    accepted: quotations.value.filter(q => q.status === 'accepted').length,
    expired: quotations.value.filter(q => q.status === 'expired').length
  }
}

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString('en-ZA')
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    draft: 'bg-gray-100 text-gray-800 dark:bg-gray-800 dark:text-gray-300',
    sent: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300',
    accepted: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300',
    rejected: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300',
    expired: 'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-300'
  }
  return classes[status] || classes.draft
}

const viewQuotation = (id: number) => {
  navigateTo(`/sales/quotations/${id}`)
}

const editQuotation = (id: number) => {
  navigateTo(`/sales/quotations/${id}/edit`)
}

const convertToOrder = async (id: number) => {
  // TODO: Implement conversion to sales order
  console.log('Converting quotation to order:', id)
}

const deleteQuotation = async (id: number) => {
  if (confirm(t('sales.quotations.confirmDelete'))) {
    quotations.value = quotations.value.filter(q => q.id !== id)
    updateStats()
  }
}

const downloadTemplate = () => {
  // TODO: Download quotation template
  console.log('Downloading template')
}

onMounted(() => {
  loadQuotations()
})
</script>
