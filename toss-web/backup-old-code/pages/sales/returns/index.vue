<template>
  <div class="returns-page">
    <!-- Page Header -->
    <div class="page-header mb-6">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-3xl font-bold text-gray-900 dark:text-white flex items-center">
            <Icon name="mdi:keyboard-return" class="mr-3 text-red-600" />
            {{ t('returns.title') }}
          </h1>
          <p class="mt-2 text-sm text-gray-600 dark:text-gray-400">
            {{ t('returns.subtitle') }}
          </p>
        </div>
        <NuxtLink
          to="/sales/returns/create"
          class="btn btn-primary"
        >
          <Icon name="mdi:plus" class="mr-2" />
          {{ t('returns.actions.createReturn') }}
        </NuxtLink>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-6">
      <div class="stat-card bg-gradient-to-br from-blue-500 to-blue-600">
        <div class="stat-icon">
          <Icon name="mdi:file-document-multiple" />
        </div>
        <div class="stat-content">
          <p class="stat-label">{{ t('returns.stats.totalReturns') }}</p>
          <p class="stat-value">{{ stats.totalReturns }}</p>
        </div>
      </div>

      <div class="stat-card bg-gradient-to-br from-yellow-500 to-yellow-600">
        <div class="stat-icon">
          <Icon name="mdi:clock-outline" />
        </div>
        <div class="stat-content">
          <p class="stat-label">{{ t('returns.stats.pending') }}</p>
          <p class="stat-value">{{ stats.pending }}</p>
        </div>
      </div>

      <div class="stat-card bg-gradient-to-br from-green-500 to-green-600">
        <div class="stat-icon">
          <Icon name="mdi:check-circle" />
        </div>
        <div class="stat-content">
          <p class="stat-label">{{ t('returns.stats.approved') }}</p>
          <p class="stat-value">{{ stats.approved }}</p>
        </div>
      </div>

      <div class="stat-card bg-gradient-to-br from-purple-500 to-purple-600">
        <div class="stat-icon">
          <Icon name="mdi:currency-usd" />
        </div>
        <div class="stat-content">
          <p class="stat-label">{{ t('returns.stats.totalValue') }}</p>
          <p class="stat-value">{{ formatCurrency(stats.totalValue) }}</p>
        </div>
      </div>
    </div>

    <!-- Filters and Search -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6 mb-6">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div class="md:col-span-2">
          <div class="relative">
            <Icon
              name="mdi:magnify"
              class="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400"
            />
            <input
              v-model="searchQuery"
              type="text"
              :placeholder="t('returns.search.placeholder')"
              class="w-full pl-10 pr-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
            />
          </div>
        </div>

        <select
          v-model="filterStatus"
          class="border border-gray-300 dark:border-gray-600 rounded-lg px-4 py-2 focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
        >
          <option value="">{{ t('returns.filters.allStatuses') }}</option>
          <option value="draft">{{ t('returns.status.draft') }}</option>
          <option value="pending">{{ t('returns.status.pending') }}</option>
          <option value="approved">{{ t('returns.status.approved') }}</option>
          <option value="rejected">{{ t('returns.status.rejected') }}</option>
        </select>

        <select
          v-model="filterPeriod"
          class="border border-gray-300 dark:border-gray-600 rounded-lg px-4 py-2 focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
        >
          <option value="all">{{ t('returns.filters.allTime') }}</option>
          <option value="today">{{ t('returns.filters.today') }}</option>
          <option value="week">{{ t('returns.filters.thisWeek') }}</option>
          <option value="month">{{ t('returns.filters.thisMonth') }}</option>
        </select>
      </div>
    </div>

    <!-- Returns Table -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md overflow-hidden">
      <!-- Loading State -->
      <div v-if="loading" class="p-12 text-center">
        <Icon name="mdi:loading" class="w-12 h-12 animate-spin mx-auto text-blue-600" />
        <p class="mt-4 text-gray-600 dark:text-gray-400">
          {{ t('common.loading') }}
        </p>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="p-12 text-center">
        <Icon name="mdi:alert-circle" class="w-12 h-12 mx-auto text-red-600" />
        <p class="mt-4 text-red-600">{{ error }}</p>
        <button @click="fetchData" class="mt-4 btn btn-primary">
          {{ t('common.retry') }}
        </button>
      </div>

      <!-- Empty State -->
      <div v-else-if="filteredReturns.length === 0" class="p-12 text-center">
        <Icon name="mdi:package-variant" class="w-16 h-16 mx-auto text-gray-400" />
        <p class="mt-4 text-gray-600 dark:text-gray-400">
          {{ t('returns.empty.message') }}
        </p>
        <NuxtLink to="/sales/returns/create" class="mt-4 btn btn-primary inline-flex">
          <Icon name="mdi:plus" class="mr-2" />
          {{ t('returns.actions.createReturn') }}
        </NuxtLink>
      </div>

      <!-- Table -->
      <div v-else class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gray-50 dark:bg-gray-900">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                {{ t('returns.table.returnNo') }}
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                {{ t('returns.table.customer') }}
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                {{ t('returns.table.invoiceRef') }}
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                {{ t('returns.table.returnDate') }}
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                {{ t('returns.table.reason') }}
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                {{ t('returns.table.amount') }}
              </th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                {{ t('returns.table.status') }}
              </th>
              <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                {{ t('returns.table.actions') }}
              </th>
            </tr>
          </thead>
          <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
            <tr
              v-for="returnItem in paginatedReturns"
              :key="returnItem.id"
              class="hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
            >
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <Icon name="mdi:file-document" class="mr-2 text-blue-600" />
                  <span class="font-medium text-gray-900 dark:text-white">
                    {{ returnItem.returnNo }}
                  </span>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900 dark:text-white">
                  {{ returnItem.customerName }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-600 dark:text-gray-400">
                  {{ returnItem.salesInvoiceRef }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-600 dark:text-gray-400">
                {{ formatDate(returnItem.returnDate) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span class="text-xs px-2 py-1 rounded-full bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-300">
                  {{ t(`returns.reasons.${returnItem.returnReason}`) }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap font-semibold text-gray-900 dark:text-white">
                {{ formatCurrency(returnItem.refundAmount) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span :class="getStatusClass(returnItem.status)">
                  {{ t(`returns.status.${returnItem.status}`) }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <div class="flex items-center justify-end space-x-2">
                  <button
                    @click="viewReturn(returnItem.id)"
                    class="text-blue-600 hover:text-blue-900 dark:hover:text-blue-400"
                    :title="t('common.view')"
                  >
                    <Icon name="mdi:eye" />
                  </button>
                  <button
                    v-if="returnItem.status === 'pending'"
                    @click="approveReturn(returnItem.id)"
                    class="text-green-600 hover:text-green-900 dark:hover:text-green-400"
                    :title="t('returns.actions.approve')"
                  >
                    <Icon name="mdi:check-circle" />
                  </button>
                  <button
                    v-if="returnItem.status === 'approved'"
                    @click="processRefund(returnItem.id)"
                    class="text-purple-600 hover:text-purple-900 dark:hover:text-purple-400"
                    :title="t('returns.actions.processRefund')"
                  >
                    <Icon name="mdi:cash-refund" />
                  </button>
                  <button
                    @click="printReturn(returnItem.id)"
                    class="text-gray-600 hover:text-gray-900 dark:hover:text-gray-400"
                    :title="t('common.print')"
                  >
                    <Icon name="mdi:printer" />
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div class="bg-gray-50 dark:bg-gray-900 px-6 py-4 flex items-center justify-between border-t border-gray-200 dark:border-gray-700">
        <div class="text-sm text-gray-600 dark:text-gray-400">
          {{ t('common.showing') }} {{ startIndex + 1 }} - {{ endIndex }} {{ t('common.of') }} {{ filteredReturns.length }}
        </div>
        <div class="flex space-x-2">
          <button
            @click="previousPage"
            :disabled="currentPage === 1"
            class="px-3 py-1 border rounded hover:bg-gray-100 dark:hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <Icon name="mdi:chevron-left" />
          </button>
          <span class="px-3 py-1 text-sm">
            {{ currentPage }} / {{ totalPages }}
          </span>
          <button
            @click="nextPage"
            :disabled="currentPage === totalPages"
            class="px-3 py-1 border rounded hover:bg-gray-100 dark:hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            <Icon name="mdi:chevron-right" />
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useSalesReturns } from '~/composables/useSalesReturns'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
const {
  returns,
  loading,
  error,
  fetchReturns,
  changeReturnStatus,
  processRefund: processRefundAction
} = useSalesReturns()

const router = useRouter()

// Filters
const searchQuery = ref('')
const filterStatus = ref('')
const filterPeriod = ref('all')

// Pagination
const currentPage = ref(1)
const itemsPerPage = ref(10)

// Stats
const stats = computed(() => {
  const allReturns = returns.value || []
  return {
    totalReturns: allReturns.length,
    pending: allReturns.filter(r => r.status === 'pending').length,
    approved: allReturns.filter(r => r.status === 'approved').length,
    totalValue: allReturns.reduce((sum, r) => sum + (r.refundAmount || 0), 0)
  }
})

// Filtered returns
const filteredReturns = computed(() => {
  let filtered = returns.value || []

  // Search filter
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(r =>
      r.returnNo?.toLowerCase().includes(query) ||
      r.customerName?.toLowerCase().includes(query) ||
      r.salesInvoiceRef?.toLowerCase().includes(query)
    )
  }

  // Status filter
  if (filterStatus.value) {
    filtered = filtered.filter(r => r.status === filterStatus.value)
  }

  // Period filter
  if (filterPeriod.value !== 'all') {
    const now = new Date()
    filtered = filtered.filter(r => {
      const returnDate = new Date(r.returnDate)
      switch (filterPeriod.value) {
        case 'today':
          return returnDate.toDateString() === now.toDateString()
        case 'week':
          const weekAgo = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000)
          return returnDate >= weekAgo
        case 'month':
          const monthAgo = new Date(now.getTime() - 30 * 24 * 60 * 60 * 1000)
          return returnDate >= monthAgo
        default:
          return true
      }
    })
  }

  return filtered
})

// Paginated returns
const totalPages = computed(() => Math.ceil(filteredReturns.value.length / itemsPerPage.value))
const startIndex = computed(() => (currentPage.value - 1) * itemsPerPage.value)
const endIndex = computed(() => Math.min(startIndex.value + itemsPerPage.value, filteredReturns.value.length))

const paginatedReturns = computed(() => {
  return filteredReturns.value.slice(startIndex.value, endIndex.value)
})

// Methods
const fetchData = async () => {
  await fetchReturns({ limit: 100 })
}

const viewReturn = (id: number) => {
  router.push(`/sales/returns/${id}`)
}

const approveReturn = async (id: number) => {
  if (confirm(t('returns.confirmations.approve'))) {
    await changeReturnStatus(id, 'approved')
    await fetchData()
  }
}

const processRefund = async (id: number) => {
  const returnItem = returns.value?.find(r => r.id === id)
  if (!returnItem) return

  if (confirm(t('returns.confirmations.processRefund', { amount: formatCurrency(returnItem.refundAmount) }))) {
    await processRefundAction(id, {
      method: 'cash',
      amount: returnItem.refundAmount
    })
    await fetchData()
  }
}

const printReturn = (id: number) => {
  // TODO: Implement print functionality
  console.log('Print return:', id)
}

const previousPage = () => {
  if (currentPage.value > 1) currentPage.value--
}

const nextPage = () => {
  if (currentPage.value < totalPages.value) currentPage.value++
}

const getStatusClass = (status: string) => {
  const baseClasses = 'px-2 py-1 text-xs font-semibold rounded-full'
  switch (status) {
    case 'draft':
      return `${baseClasses} bg-gray-100 dark:bg-gray-700 text-gray-800 dark:text-gray-300`
    case 'pending':
      return `${baseClasses} bg-yellow-100 dark:bg-yellow-900/30 text-yellow-800 dark:text-yellow-300`
    case 'approved':
      return `${baseClasses} bg-green-100 dark:bg-green-900/30 text-green-800 dark:text-green-300`
    case 'rejected':
      return `${baseClasses} bg-red-100 dark:bg-red-900/30 text-red-800 dark:text-red-300`
    default:
      return `${baseClasses} bg-gray-100 text-gray-800`
  }
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount || 0)
}

const formatDate = (date: string | Date) => {
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

// Load data on mount
onMounted(() => {
  fetchData()
})

// Watch for filter changes and reset pagination
watch([searchQuery, filterStatus, filterPeriod], () => {
  currentPage.value = 1
})
</script>

<style scoped>
.returns-page {
  @apply max-w-7xl mx-auto px-4 py-8;
}

.stat-card {
  @apply rounded-lg shadow-md p-6 text-white flex items-center space-x-4;
}

.stat-icon {
  @apply text-4xl opacity-80;
}

.stat-content {
  @apply flex-1;
}

.stat-label {
  @apply text-sm opacity-90 mb-1;
}

.stat-value {
  @apply text-2xl font-bold;
}

.btn {
  @apply px-4 py-2 rounded-lg font-medium transition-colors flex items-center;
}

.btn-primary {
  @apply bg-blue-600 text-white hover:bg-blue-700;
}
</style>
