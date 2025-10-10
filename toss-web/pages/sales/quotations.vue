<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <!-- Mobile-First Page Container -->
    <div class="p-4 sm:p-6 space-y-4 sm:space-y-6 pb-20 lg:pb-6">
      <!-- Page Header -->
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3 sm:gap-0">
        <div>
          <h1 class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">Quotations</h1>
          <p class="text-slate-600 dark:text-slate-400 mt-1 text-sm sm:text-base">Manage quotes and convert prospects to customers</p>
        </div>
        <div class="flex flex-wrap gap-2 sm:gap-3">
          <button @click="showNewQuoteModal = true" 
                  class="flex-1 sm:flex-none px-4 py-2 sm:px-6 sm:py-3 bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg transition-colors text-sm sm:text-base">
            <DocumentPlusIcon class="w-4 h-4 sm:w-5 sm:h-5 inline mr-2" />
            New Quote
          </button>
          <button @click="exportQuotes" 
                  class="flex-1 sm:flex-none px-4 py-2 sm:px-6 sm:py-3 bg-slate-600 hover:bg-slate-700 text-white rounded-lg transition-colors text-sm sm:text-base">
            <ArrowDownTrayIcon class="w-4 h-4 sm:w-5 sm:h-5 inline mr-2" />
            Export
          </button>
        </div>
      </div>

      <!-- Quote Stats -->
      <div class="grid grid-cols-1 xs:grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-6">
        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Total Quotes</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ totalQuotes }}</p>
              <p class="text-xs sm:text-sm text-emerald-600">{{ newQuotes }} this week</p>
            </div>
            <div class="p-2 sm:p-3 bg-emerald-100 dark:bg-emerald-900 rounded-full">
              <DocumentTextIcon class="w-4 h-4 sm:w-6 sm:h-6 text-emerald-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Pending</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ pendingQuotes }}</p>
              <p class="text-xs sm:text-sm text-yellow-600">{{ expiringSoon }} expiring soon</p>
            </div>
            <div class="p-2 sm:p-3 bg-yellow-100 dark:bg-yellow-900 rounded-full">
              <ClockIcon class="w-4 h-4 sm:w-6 sm:h-6 text-yellow-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Accepted</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ acceptedQuotes }}</p>
              <p class="text-xs sm:text-sm text-green-600">{{ conversionRate }}% rate</p>
            </div>
            <div class="p-2 sm:p-3 bg-green-100 dark:bg-green-900 rounded-full">
              <CheckCircleIcon class="w-4 h-4 sm:w-6 sm:h-6 text-green-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Quote Value</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">R {{ formatCurrency(totalQuoteValue) }}</p>
              <p class="text-xs sm:text-sm text-blue-600">R {{ formatCurrency(avgQuoteValue) }} avg</p>
            </div>
            <div class="p-2 sm:p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
              <CurrencyDollarIcon class="w-4 h-4 sm:w-6 sm:h-6 text-blue-600" />
            </div>
          </div>
        </div>
      </div>

      <!-- Filters and Search -->
      <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 p-4 sm:p-6">
        <div class="flex flex-col sm:flex-row gap-3 sm:gap-4">
          <div class="flex-1">
            <input v-model="searchQuery" type="text" placeholder="Search quotes..." 
                   class="w-full px-3 sm:px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-emerald-500 dark:bg-slate-700 dark:text-white">
          </div>
          <div class="flex gap-2 sm:gap-3">
            <select v-model="statusFilter" 
                    class="px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-emerald-500 dark:bg-slate-700 dark:text-white">
              <option value="">All Status</option>
              <option value="draft">Draft</option>
              <option value="sent">Sent</option>
              <option value="accepted">Accepted</option>
              <option value="rejected">Rejected</option>
              <option value="expired">Expired</option>
            </select>
            <select v-model="dateFilter" 
                    class="px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-emerald-500 dark:bg-slate-700 dark:text-white">
              <option value="">All Time</option>
              <option value="today">Today</option>
              <option value="week">This Week</option>
              <option value="month">This Month</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Quotes List -->
      <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
        <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">Recent Quotes</h3>
        </div>
        <div class="p-4 sm:p-6">
          <div class="space-y-3 sm:space-y-4">
            <div v-for="quote in filteredQuotes" :key="quote.id" 
                 class="flex items-center justify-between p-4 rounded-lg border border-slate-100 dark:border-slate-700 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors">
              <div class="flex items-center space-x-3 flex-1 min-w-0">
                <div class="w-10 h-10 rounded-full flex items-center justify-center" :class="getStatusColor(quote.status)">
                  <DocumentTextIcon class="w-5 h-5 text-white" />
                </div>
                <div class="flex-1 min-w-0">
                  <div class="flex items-center gap-2">
                    <p class="text-sm font-medium text-slate-900 dark:text-white truncate">{{ quote.quoteNumber }}</p>
                    <span class="inline-flex px-2 py-1 text-xs rounded-full" :class="getStatusBadge(quote.status)">
                      {{ quote.status }}
                    </span>
                  </div>
                  <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">{{ quote.customer }}</p>
                  <p class="text-xs text-slate-500 dark:text-slate-500">Valid until {{ formatDate(quote.validUntil) }}</p>
                </div>
              </div>
              <div class="text-right">
                <p class="text-sm font-semibold text-slate-900 dark:text-white">R {{ formatCurrency(quote.amount) }}</p>
                <div class="flex gap-1 mt-1">
                  <button @click="viewQuote(quote)" class="p-1 text-blue-600 hover:bg-blue-50 dark:hover:bg-blue-900 rounded">
                    <EyeIcon class="w-4 h-4" />
                  </button>
                  <button @click="editQuote(quote)" class="p-1 text-emerald-600 hover:bg-emerald-50 dark:hover:bg-emerald-900 rounded">
                    <PencilIcon class="w-4 h-4" />
                  </button>
                  <button @click="duplicateQuote(quote)" class="p-1 text-purple-600 hover:bg-purple-50 dark:hover:bg-purple-900 rounded">
                    <DocumentDuplicateIcon class="w-4 h-4" />
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- New Quote Modal -->
    <div v-if="showNewQuoteModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-slate-800 rounded-xl sm:rounded-2xl shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-lg sm:text-xl font-semibold text-slate-900 dark:text-white">Create New Quote</h3>
        </div>
        <div class="p-4 sm:p-6">
          <form @submit.prevent="createQuote">
            <div class="space-y-4">
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Quote Number</label>
                  <input v-model="newQuote.quoteNumber" type="text" required readonly
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-slate-50 dark:bg-slate-700 dark:text-white">
                </div>
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Valid Until</label>
                  <input v-model="newQuote.validUntil" type="date" required 
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-emerald-500 dark:bg-slate-700 dark:text-white">
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Customer Information</label>
                <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <input v-model="newQuote.customerName" placeholder="Customer Name" required
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-emerald-500 dark:bg-slate-700 dark:text-white">
                  <input v-model="newQuote.customerEmail" type="email" placeholder="Email Address"
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-emerald-500 dark:bg-slate-700 dark:text-white">
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Items</label>
                <div class="space-y-2">
                  <div v-for="(item, index) in newQuote.items" :key="index" class="flex gap-2 items-end">
                    <div class="flex-1">
                      <input v-model="item.description" placeholder="Item description" required
                             class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-emerald-500 dark:bg-slate-700 dark:text-white text-sm">
                    </div>
                    <div class="w-20">
                      <input v-model.number="item.quantity" type="number" placeholder="Qty" min="1" required
                             class="w-full px-2 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-emerald-500 dark:bg-slate-700 dark:text-white text-sm">
                    </div>
                    <div class="w-24">
                      <input v-model.number="item.unitPrice" type="number" step="0.01" placeholder="Price" min="0" required
                             class="w-full px-2 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-emerald-500 dark:bg-slate-700 dark:text-white text-sm">
                    </div>
                    <button type="button" @click="removeQuoteItem(index)" class="p-2 text-red-600 hover:bg-red-50 dark:hover:bg-red-900 rounded-lg">
                      <XMarkIcon class="w-4 h-4" />
                    </button>
                  </div>
                </div>
                <button type="button" @click="addQuoteItem" class="mt-2 text-sm text-emerald-600 hover:text-emerald-700">
                  + Add Item
                </button>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Terms & Conditions</label>
                <textarea v-model="newQuote.terms" rows="3" placeholder="Enter terms and conditions..."
                          class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-emerald-500 dark:bg-slate-700 dark:text-white"></textarea>
              </div>

              <div class="bg-emerald-50 dark:bg-emerald-900/20 p-4 rounded-lg">
                <div class="flex justify-between items-center">
                  <span class="text-lg font-semibold text-slate-900 dark:text-white">Quote Total:</span>
                  <span class="text-xl font-bold text-emerald-600 dark:text-emerald-400">R {{ formatCurrency(calculateQuoteTotal()) }}</span>
                </div>
              </div>
            </div>

            <div class="flex justify-end space-x-3 mt-6">
              <button @click="showNewQuoteModal = false" type="button" 
                      class="px-4 py-2 text-slate-600 dark:text-slate-400 hover:text-slate-800 dark:hover:text-slate-200">
                Cancel
              </button>
              <button type="submit" 
                      class="px-6 py-2 bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg">
                Create Quote
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { 
  DocumentTextIcon,
  DocumentPlusIcon,
  ArrowDownTrayIcon,
  ClockIcon,
  CheckCircleIcon,
  CurrencyDollarIcon,
  EyeIcon,
  PencilIcon,
  DocumentDuplicateIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Quotations - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage quotes and convert prospects for Thabo\'s Spaza Shop' }
  ]
})

// Layout
definePageMeta({
  layout: 'default'
})

// Reactive data
const showNewQuoteModal = ref(false)
const searchQuery = ref('')
const statusFilter = ref('')
const dateFilter = ref('')

// Quote statistics
const totalQuotes = ref(28)
const newQuotes = ref(5)
const pendingQuotes = ref(12)
const expiringSoon = ref(3)
const acceptedQuotes = ref(16)
const conversionRate = ref(57)
const totalQuoteValue = ref(125400)
const avgQuoteValue = ref(4478)

// Sample quotes data for Thabo's Spaza Shop
const quotes = ref([
  {
    id: '1',
    quoteNumber: 'QUO-2025-001',
    customer: 'Nomsa Community Kitchen',
    customerEmail: 'nomsa@communitykitchen.co.za',
    amount: 2850,
    status: 'sent',
    validUntil: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000),
    createdAt: new Date(),
    items: [
      { description: 'Bulk Rice (25kg)', quantity: 2, unitPrice: 850 },
      { description: 'Cooking Oil (5L)', quantity: 3, unitPrice: 350 },
      { description: 'Maize Meal (12.5kg)', quantity: 1, unitPrice: 450 }
    ]
  },
  {
    id: '2',
    quoteNumber: 'QUO-2025-002',
    customer: 'Sipho Auto Repair',
    customerEmail: 'sipho@autorepair.co.za',
    amount: 1250,
    status: 'accepted',
    validUntil: new Date(Date.now() + 14 * 24 * 60 * 60 * 1000),
    createdAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
    items: [
      { description: 'Motor Oil (5L)', quantity: 5, unitPrice: 180 },
      { description: 'Air Fresheners', quantity: 10, unitPrice: 25 },
      { description: 'Car Wash Soap', quantity: 3, unitPrice: 150 }
    ]
  },
  {
    id: '3',
    quoteNumber: 'QUO-2025-003',
    customer: 'Lerato Hair Studio',
    customerEmail: 'lerato@hairstudio.co.za',
    amount: 3200,
    status: 'pending',
    validUntil: new Date(Date.now() + 3 * 24 * 60 * 60 * 1000),
    createdAt: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000),
    items: [
      { description: 'Hair Products Bundle', quantity: 1, unitPrice: 2500 },
      { description: 'Hair Accessories Kit', quantity: 1, unitPrice: 450 },
      { description: 'Professional Shampoo', quantity: 2, unitPrice: 125 }
    ]
  },
  {
    id: '4',
    quoteNumber: 'QUO-2025-004',
    customer: 'Mandla Construction',
    customerEmail: 'mandla@construction.co.za',
    amount: 8950,
    status: 'draft',
    validUntil: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000),
    createdAt: new Date(Date.now() - 4 * 60 * 60 * 1000),
    items: [
      { description: 'Building Materials Package', quantity: 1, unitPrice: 7500 },
      { description: 'Tools Rental', quantity: 1, unitPrice: 1200 },
      { description: 'Safety Equipment', quantity: 1, unitPrice: 250 }
    ]
  }
])

// Form data
const newQuote = ref({
  quoteNumber: generateQuoteNumber(),
  customerName: '',
  customerEmail: '',
  validUntil: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
  items: [
    { description: '', quantity: 1, unitPrice: 0 }
  ],
  terms: 'Payment terms: 30 days\nDelivery: Within 7 business days\nPrices valid for 30 days'
})

// Computed
const filteredQuotes = computed(() => {
  let filtered = quotes.value

  if (searchQuery.value) {
    filtered = filtered.filter(quote => 
      quote.customer.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      quote.quoteNumber.toLowerCase().includes(searchQuery.value.toLowerCase())
    )
  }

  if (statusFilter.value) {
    filtered = filtered.filter(quote => quote.status === statusFilter.value)
  }

  return filtered
})

// Helper functions
function generateQuoteNumber() {
  const date = new Date()
  const year = date.getFullYear()
  const nextNumber = (quotes.value.length + 1).toString().padStart(3, '0')
  return `QUO-${year}-${nextNumber}`
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

const formatDate = (date: Date) => {
  return new Intl.DateTimeFormat('en-ZA', {
    day: 'numeric',
    month: 'short',
    year: 'numeric'
  }).format(date)
}

const getStatusColor = (status: string) => {
  const colors = {
    draft: 'bg-slate-600',
    sent: 'bg-blue-600',
    pending: 'bg-yellow-600',
    accepted: 'bg-green-600',
    rejected: 'bg-red-600',
    expired: 'bg-gray-600'
  }
  return colors[status as keyof typeof colors] || 'bg-slate-600'
}

const getStatusBadge = (status: string) => {
  const badges = {
    draft: 'bg-slate-100 text-slate-800 dark:bg-slate-900 dark:text-slate-200',
    sent: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    accepted: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    rejected: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    expired: 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200'
  }
  return badges[status as keyof typeof badges] || 'bg-slate-100 text-slate-800'
}

// Quote form functions
const addQuoteItem = () => {
  newQuote.value.items.push({ description: '', quantity: 1, unitPrice: 0 })
}

const removeQuoteItem = (index: number) => {
  if (newQuote.value.items.length > 1) {
    newQuote.value.items.splice(index, 1)
  }
}

const calculateQuoteTotal = () => {
  return newQuote.value.items.reduce((total, item) => {
    return total + (item.quantity * item.unitPrice)
  }, 0)
}

// Actions
const createQuote = async () => {
  try {
    const quote = {
      id: Date.now().toString(),
      quoteNumber: newQuote.value.quoteNumber,
      customer: newQuote.value.customerName,
      customerEmail: newQuote.value.customerEmail,
      amount: calculateQuoteTotal(),
      status: 'draft',
      validUntil: new Date(newQuote.value.validUntil),
      createdAt: new Date(),
      items: [...newQuote.value.items]
    }

    quotes.value.unshift(quote)
    totalQuotes.value += 1
    
    showNewQuoteModal.value = false
    
    // Reset form
    newQuote.value = {
      quoteNumber: generateQuoteNumber(),
      customerName: '',
      customerEmail: '',
      validUntil: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
      items: [{ description: '', quantity: 1, unitPrice: 0 }],
      terms: 'Payment terms: 30 days\nDelivery: Within 7 business days\nPrices valid for 30 days'
    }
    
    alert('Quote created successfully!')
  } catch (error) {
    console.error('Error creating quote:', error)
    alert('Failed to create quote. Please try again.')
  }
}

const viewQuote = (quote: any) => {
  // Show quote details in a modal or navigate to detail page
  const details = `
Quote Number: ${quote.quoteNumber}
Customer: ${quote.customer}
Email: ${quote.customerEmail}
Amount: R ${formatCurrency(quote.amount)}
Status: ${quote.status}
Valid Until: ${formatDate(quote.validUntil)}

Items:
${quote.items.map((item: any) => `- ${item.description} (${item.quantity} x R${item.unitPrice})`).join('\n')}
  `.trim()
  
  alert(details)
}

const editQuote = (quote: any) => {
  // Populate form with quote data for editing
  newQuote.value = {
    quoteNumber: quote.quoteNumber,
    customerName: quote.customer,
    customerEmail: quote.customerEmail,
    validUntil: new Date(quote.validUntil).toISOString().split('T')[0],
    items: [...quote.items],
    terms: newQuote.value.terms
  }
  
  // Remove original quote from list
  const index = quotes.value.findIndex(q => q.id === quote.id)
  if (index !== -1) {
    quotes.value.splice(index, 1)
    totalQuotes.value -= 1
  }
  
  showNewQuoteModal.value = true
}

const duplicateQuote = (quote: any) => {
  // Create a copy with new quote number
  const duplicatedQuote = {
    id: Date.now().toString(),
    quoteNumber: generateQuoteNumber(),
    customer: quote.customer,
    customerEmail: quote.customerEmail,
    amount: quote.amount,
    status: 'draft',
    validUntil: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000),
    createdAt: new Date(),
    items: [...quote.items]
  }
  
  quotes.value.unshift(duplicatedQuote)
  totalQuotes.value += 1
  
  alert(`Quote duplicated as ${duplicatedQuote.quoteNumber}`)
}

const exportQuotes = () => {
  try {
    // Prepare data for export
    const exportData = filteredQuotes.value.map(quote => ({
      'Quote Number': quote.quoteNumber,
      'Customer': quote.customer,
      'Email': quote.customerEmail,
      'Amount': quote.amount,
      'Status': quote.status,
      'Valid Until': formatDate(quote.validUntil),
      'Created': formatDate(quote.createdAt),
      'Items': quote.items.map((item: any) => 
        `${item.description} (${item.quantity} x R${item.unitPrice})`
      ).join('; ')
    }))
    
    // Convert to CSV
    const headers = Object.keys(exportData[0])
    const csvContent = [
      headers.join(','),
      ...exportData.map(row => 
        headers.map(header => {
          const value = row[header as keyof typeof row]
          // Escape commas and quotes in values
          return typeof value === 'string' && (value.includes(',') || value.includes('"'))
            ? `"${value.replace(/"/g, '""')}"`
            : value
        }).join(',')
      )
    ].join('\n')
    
    // Create download link
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
    const link = document.createElement('a')
    const url = URL.createObjectURL(blob)
    
    link.setAttribute('href', url)
    link.setAttribute('download', `quotes_${new Date().toISOString().split('T')[0]}.csv`)
    link.style.visibility = 'hidden'
    
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    
    alert('Quotes exported successfully!')
  } catch (error) {
    console.error('Export failed:', error)
    alert('Failed to export quotes. Please try again.')
  }
}
</script>
