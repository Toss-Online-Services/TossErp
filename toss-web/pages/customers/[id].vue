<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useCrmStore, type Customer, type Communication } from '~/stores/crm'
import { useSalesStore, type Invoice, type SalesOrder, type Quotation } from '~/stores/sales'
import { usePosStore, type Sale } from '~/stores/pos'

definePageMeta({
  layout: 'default',
  ssr: false
})

useHead({
  title: 'Customer Details - TOSS'
})

const route = useRoute()
const crmStore = useCrmStore()
const salesStore = useSalesStore()
const posStore = usePosStore()

const customer = ref<Customer | null>(null)
const loading = ref(false)
const historyLoading = ref(false)

const customerId = computed(() => {
  const id = route.params.id
  return Array.isArray(id) ? id[0] : String(id)
})

// History data
const invoices = ref<Invoice[]>([])
const orders = ref<SalesOrder[]>([])
const quotations = ref<Quotation[]>([])
const posSales = ref<Sale[]>([])
const communications = ref<Communication[]>([])

// Combined history sorted by date
const customerHistory = computed(() => {
  const history: Array<{
    type: 'invoice' | 'order' | 'quotation' | 'pos_sale' | 'communication'
    date: Date
    title: string
    amount?: number
    status?: string
    id: string
    data: any
  }> = []

  // Add invoices
  invoices.value.forEach(inv => {
    history.push({
      type: 'invoice',
      date: inv.invoiceDate,
      title: `Invoice ${inv.invoiceNumber}`,
      amount: inv.total,
      status: inv.status,
      id: inv.id,
      data: inv
    })
  })

  // Add orders
  orders.value.forEach(order => {
    history.push({
      type: 'order',
      date: order.orderDate,
      title: `Sales Order ${order.orderNumber}`,
      amount: order.total,
      status: order.status,
      id: order.id,
      data: order
    })
  })

  // Add quotations
  quotations.value.forEach(quote => {
    history.push({
      type: 'quotation',
      date: quote.date,
      title: `Quotation ${quote.quotationNumber}`,
      amount: quote.total,
      status: quote.status,
      id: quote.id,
      data: quote
    })
  })

  // Add POS sales
  posSales.value.forEach(sale => {
    history.push({
      type: 'pos_sale',
      date: sale.createdAt,
      title: `POS Sale ${sale.invoiceNumber}`,
      amount: sale.total,
      status: sale.status,
      id: sale.id,
      data: sale
    })
  })

  // Add communications
  communications.value.forEach(comm => {
    history.push({
      type: 'communication',
      date: comm.createdAt,
      title: comm.subject,
      id: comm.id,
      data: comm
    })
  })

  // Sort by date (newest first)
  return history.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
})

async function loadCustomer() {
  loading.value = true
  try {
    await crmStore.fetchCustomers()
    const id = customerId.value
    customer.value = crmStore.getCustomerById(id) || null
    
    if (!customer.value) {
      // Fallback: try to find by matching the ID as a string
      const found = crmStore.customers.find(c => String(c.id) === String(id))
      if (found) {
        customer.value = found
        await loadHistory()
      }
    } else {
      await loadHistory()
    }
  } catch (error) {
    console.error('Failed to load customer:', error)
  } finally {
    loading.value = false
  }
}

async function loadHistory() {
  if (!customer.value) return
  
  historyLoading.value = true
  try {
    // Fetch all sales data
    await Promise.all([
      salesStore.fetchInvoices(),
      salesStore.fetchOrders(),
      salesStore.fetchQuotations(),
      crmStore.fetchCommunications(),
      posStore.fetchRecentSales()
    ])

    // Filter by customer ID or name (to handle different ID formats)
    const id = customer.value.id
    const name = customer.value.name
    // Handle both '1' and 'cust-1' formats
    const idVariants = [id, `cust-${id}`, `cust_${id}`, `customer-${id}`]
    
    
    invoices.value = salesStore.invoices.filter(inv => {
      const matchesId = idVariants.some(variant => inv.customerId === variant || inv.customerId === String(variant))
      const matchesName = inv.customerName === name
      return matchesId || matchesName
    })
    
    orders.value = salesStore.orders.filter(order => {
      const matchesId = idVariants.some(variant => order.customerId === variant || order.customerId === String(variant))
      const matchesName = order.customerName === name
      return matchesId || matchesName
    })
    
    quotations.value = salesStore.quotations.filter(quote => {
      const matchesId = idVariants.some(variant => quote.customerId === variant || quote.customerId === String(variant))
      const matchesName = quote.customerName === name
      return matchesId || matchesName
    })
    
    communications.value = crmStore.communications.filter(comm => {
      if (comm.relatedTo !== 'customer') return false
      return idVariants.some(variant => comm.relatedId === variant || comm.relatedId === String(variant))
    })

    // Load POS sales
    if (posStore.recentSales && posStore.recentSales.length > 0) {
      posSales.value = posStore.recentSales.filter(sale => {
        if (!sale.customerId) return false
        const matchesId = idVariants.some(variant => sale.customerId === variant || sale.customerId === String(variant))
        const matchesName = sale.customerName === name
        return matchesId || matchesName
      })
    }
    
  } catch (error) {
    console.error('Failed to load customer history:', error)
  } finally {
    historyLoading.value = false
  }
}

function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
}

function formatDate(date: Date | undefined) {
  if (!date) return 'Never'
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })
}

function formatDateTime(date: Date | undefined) {
  if (!date) return 'Never'
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

function getHistoryTypeIcon(type: string) {
  const icons: Record<string, string> = {
    invoice: 'receipt',
    order: 'shopping_cart',
    quotation: 'description',
    pos_sale: 'point_of_sale',
    communication: 'chat'
  }
  return icons[type] || 'history'
}

function getHistoryTypeColor(type: string) {
  const colors: Record<string, string> = {
    invoice: 'text-blue-600 bg-blue-100',
    order: 'text-green-600 bg-green-100',
    quotation: 'text-purple-600 bg-purple-100',
    pos_sale: 'text-orange-600 bg-orange-100',
    communication: 'text-gray-600 bg-gray-100'
  }
  return colors[type] || 'text-gray-600 bg-gray-100'
}

function getHistoryTypeLabel(type: string) {
  const labels: Record<string, string> = {
    invoice: 'Invoice',
    order: 'Sales Order',
    quotation: 'Quotation',
    pos_sale: 'POS Sale',
    communication: 'Communication'
  }
  return labels[type] || type
}

function getStatusColor(status: string) {
  const colors: Record<string, string> = {
    draft: 'text-gray-600 bg-gray-100',
    sent: 'text-blue-600 bg-blue-100',
    paid: 'text-green-600 bg-green-100',
    partially_paid: 'text-orange-600 bg-orange-100',
    overdue: 'text-red-600 bg-red-100',
    cancelled: 'text-gray-600 bg-gray-100',
    confirmed: 'text-green-600 bg-green-100',
    delivered: 'text-green-600 bg-green-100',
    accepted: 'text-green-600 bg-green-100',
    rejected: 'text-red-600 bg-red-100',
    completed: 'text-green-600 bg-green-100',
    pending_sync: 'text-yellow-600 bg-yellow-100'
  }
  return colors[status] || 'text-gray-600 bg-gray-100'
}

function handleHistoryItemClick(item: any) {
  if (item.type === 'invoice') {
    navigateTo(`/sales/invoices/${item.id}`)
  } else if (item.type === 'order') {
    navigateTo(`/sales/orders/${item.id}`)
  } else if (item.type === 'quotation') {
    navigateTo(`/sales/quotations/${item.id}`)
  } else if (item.type === 'pos_sale') {
    // POS sales might not have a detail page, could navigate to POS with filter
    navigateTo('/pos')
  }
}

function getCustomerStatusColor(status: string) {
  return status === 'active' 
    ? 'text-green-600 bg-green-100' 
    : 'text-gray-600 bg-gray-100'
}

function handlePrint() {
  window.print()
}

async function handleSend() {
  if (!customer.value) return
  try {
    // TODO: Implement send functionality (email/SMS/WhatsApp)
    alert(`Sending customer information for ${customer.value.name}...`)
    // await crmStore.sendCustomerInfo(customer.value.id)
  } catch (error) {
    console.error('Failed to send customer info:', error)
  }
}

onMounted(async () => {
  await loadCustomer()
})
</script>

<template>
  <div class="py-6">
    <div v-if="loading" class="flex items-center justify-center py-12">
      <div class="text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4 animate-spin">refresh</i>
        <p class="text-gray-600">Loading customer...</p>
      </div>
    </div>

    <div v-else-if="!customer" class="flex items-center justify-center py-12">
      <div class="text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">error</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">Customer not found</h3>
        <p class="text-gray-600 mb-6">The customer you're looking for doesn't exist</p>
        <button
          @click="navigateTo('/customers')"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">arrow_back</i>
          <span>Back to Customers</span>
        </button>
      </div>
    </div>

    <div v-else>
      <!-- Header -->
      <div class="mb-6">
        <button
          @click="navigateTo('/customers')"
          class="inline-flex items-center gap-2 text-gray-600 hover:text-gray-900 mb-4 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">arrow_back</i>
          <span>Back to Customers</span>
        </button>
        <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
          <div class="flex items-center gap-4">
            <div class="h-16 w-16 bg-gradient-to-br from-gray-800 to-gray-900 rounded-full flex items-center justify-center text-white text-2xl font-bold">
              {{ customer.name.charAt(0) }}
            </div>
            <div>
              <h3 class="text-3xl font-bold text-gray-900 mb-2">{{ customer.name }}</h3>
              <p class="text-gray-600 text-sm">{{ customer.customerType === 'business' ? 'Business' : 'Individual' }} Customer</p>
            </div>
          </div>
          <div class="flex flex-wrap items-center gap-3">
            <span :class="['px-3 py-1 text-sm font-medium rounded-full', getCustomerStatusColor(customer.status)]">
              {{ customer.status === 'active' ? 'Active' : 'Inactive' }}
            </span>
            <button
              @click="handlePrint"
              class="inline-flex items-center gap-2 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
            >
              <i class="material-symbols-rounded text-lg">print</i>
              <span>Print</span>
            </button>
            <button
              @click="handleSend"
              class="inline-flex items-center gap-2 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
            >
              <i class="material-symbols-rounded text-lg">send</i>
              <span>Send</span>
            </button>
          </div>
        </div>
      </div>

      <!-- Info Cards -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">contact_mail</i>
            Contact Information
          </h4>
          <div class="space-y-3">
            <div v-if="customer.phone">
              <p class="text-xs text-gray-500 mb-1">Phone</p>
              <p class="text-base font-semibold text-gray-900">{{ customer.phone }}</p>
            </div>
            <div v-if="customer.email">
              <p class="text-xs text-gray-500 mb-1">Email</p>
              <p class="text-base font-semibold text-gray-900">{{ customer.email }}</p>
            </div>
            <div v-if="customer.address">
              <p class="text-xs text-gray-500 mb-1">Address</p>
              <p class="text-sm text-gray-700">{{ customer.address }}</p>
              <p v-if="customer.city" class="text-sm text-gray-700">{{ customer.city }}{{ customer.postalCode ? ` ${customer.postalCode}` : '' }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">account_balance</i>
            Financial Information
          </h4>
          <div class="space-y-3">
            <div>
              <p class="text-xs text-gray-500 mb-1">Credit Limit</p>
              <p class="text-base font-semibold text-gray-900">{{ formatCurrency(customer.creditLimit) }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 mb-1">Current Balance</p>
              <p class="text-base font-semibold text-gray-900">{{ formatCurrency(customer.currentBalance) }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 mb-1">Outstanding Amount</p>
              <p :class="['text-lg font-bold', customer.outstandingAmount > 0 ? 'text-red-600' : 'text-gray-900']">
                {{ formatCurrency(customer.outstandingAmount) }}
              </p>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">trending_up</i>
            Purchase History
          </h4>
          <div class="space-y-3">
            <div>
              <p class="text-xs text-gray-500 mb-1">Total Purchases</p>
              <p class="text-lg font-bold text-gray-900">{{ formatCurrency(customer.totalPurchases) }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 mb-1">Last Purchase</p>
              <p class="text-base font-semibold text-gray-900">{{ formatDate(customer.lastPurchaseDate) }}</p>
            </div>
            <div v-if="customer.tags.length > 0">
              <p class="text-xs text-gray-500 mb-2">Tags</p>
              <div class="flex flex-wrap gap-2">
                <span
                  v-for="tag in customer.tags"
                  :key="tag"
                  class="px-2 py-1 text-xs font-medium bg-gray-100 text-gray-700 rounded-full"
                >
                  {{ tag }}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Notes -->
      <div v-if="customer.notes" class="bg-white rounded-xl shadow-card p-6 mb-6">
        <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
          <i class="material-symbols-rounded text-lg">note</i>
          Notes
        </h4>
        <p class="text-sm text-gray-700 leading-relaxed">{{ customer.notes }}</p>
      </div>

      <!-- Customer History -->
      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between mb-6">
          <h4 class="text-lg font-semibold text-gray-900 flex items-center gap-2">
            <i class="material-symbols-rounded text-xl">history</i>
            Customer History
          </h4>
          <div class="text-sm text-gray-600">
            {{ customerHistory.length }} {{ customerHistory.length === 1 ? 'item' : 'items' }}
          </div>
        </div>

        <div v-if="historyLoading" class="flex items-center justify-center py-8">
          <div class="text-center">
            <i class="material-symbols-rounded text-4xl text-gray-300 mb-2 animate-spin">refresh</i>
            <p class="text-sm text-gray-600">Loading history...</p>
          </div>
        </div>

        <div v-else-if="customerHistory.length === 0" class="text-center py-12">
          <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">history</i>
          <h3 class="text-lg font-medium text-gray-900 mb-2">No history found</h3>
          <p class="text-gray-600">This customer has no transactions or communications yet</p>
        </div>

        <div v-else class="space-y-3">
          <div
            v-for="item in customerHistory"
            :key="`${item.type}-${item.id}`"
            @click="handleHistoryItemClick(item)"
            class="flex items-start gap-4 p-4 border border-gray-200 rounded-lg hover:bg-gray-50 hover:border-gray-300 transition-colors cursor-pointer"
            :class="{ 'cursor-pointer': item.type !== 'communication' }"
          >
            <!-- Icon -->
            <div :class="['p-2 rounded-lg', getHistoryTypeColor(item.type)]">
              <i class="material-symbols-rounded text-lg">{{ getHistoryTypeIcon(item.type) }}</i>
            </div>

            <!-- Content -->
            <div class="flex-1 min-w-0">
              <div class="flex items-start justify-between gap-4 mb-1">
                <div class="flex-1">
                  <h5 class="text-sm font-semibold text-gray-900 mb-1">{{ item.title }}</h5>
                  <p class="text-xs text-gray-500">{{ getHistoryTypeLabel(item.type) }}</p>
                </div>
                <div v-if="item.amount !== undefined" class="text-right">
                  <p class="text-sm font-semibold text-gray-900">{{ formatCurrency(item.amount) }}</p>
                </div>
              </div>
              
              <div class="flex items-center gap-3 mt-2">
                <span class="text-xs text-gray-500">
                  <i class="material-symbols-rounded text-xs align-middle">schedule</i>
                  {{ formatDateTime(item.date) }}
                </span>
                <span
                  v-if="item.status"
                  :class="['px-2 py-1 text-xs font-medium rounded-full', getStatusColor(item.status)]"
                >
                  {{ item.status.replace('_', ' ').replace(/\b\w/g, l => l.toUpperCase()) }}
                </span>
              </div>

              <!-- Communication specific content -->
              <div v-if="item.type === 'communication'" class="mt-2">
                <p class="text-xs text-gray-600 line-clamp-2">{{ item.data.content }}</p>
                <div class="flex items-center gap-2 mt-1">
                  <span class="text-xs text-gray-500">
                    {{ item.data.type }} • {{ item.data.direction }}
                  </span>
                </div>
              </div>
            </div>

            <!-- Action icon -->
            <div v-if="item.type !== 'communication'" class="text-gray-400">
              <i class="material-symbols-rounded text-lg">chevron_right</i>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

