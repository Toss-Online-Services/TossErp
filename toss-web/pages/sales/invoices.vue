<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-orange-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header with Glass Morphism -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full mx-auto px-3 sm:px-4 lg:px-8 py-4 sm:py-6">
        <div class="flex flex-col space-y-3 sm:flex-row sm:items-center sm:justify-between sm:space-y-0">
          <div class="flex-1 min-w-0">
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-orange-600 to-blue-600 bg-clip-text text-transparent truncate">
              Sales Invoices
            </h1>
            <p class="mt-1 text-xs sm:text-sm text-slate-600 dark:text-slate-400 line-clamp-1">
              Manage billing and payment tracking
            </p>
          </div>
          <div class="flex space-x-2 sm:space-x-3 flex-shrink-0">
            <button 
              @click="showNewInvoiceModal = true" 
              class="inline-flex items-center justify-center px-3 sm:px-4 py-2 sm:py-2.5 bg-gradient-to-r from-orange-600 to-blue-600 text-white rounded-xl hover:from-orange-700 hover:to-blue-700 shadow-lg hover:shadow-xl transition-all duration-200 text-xs sm:text-sm font-semibold whitespace-nowrap"
            >
              <PlusIcon class="w-4 h-4 sm:mr-2" />
              <span class="hidden sm:inline">New Invoice</span>
            </button>
            <button 
              @click="exportInvoices" 
              class="inline-flex items-center justify-center px-3 sm:px-4 py-2 sm:py-2.5 rounded-xl text-xs sm:text-sm font-medium text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 hover:shadow-md transition-all duration-200 whitespace-nowrap"
            >
              <ArrowDownTrayIcon class="w-4 h-4 sm:mr-2" />
              <span class="hidden sm:inline">Export</span>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="w-full max-w-7xl mx-auto px-3 sm:px-4 lg:px-8 py-4 sm:py-8 space-y-4 sm:space-y-6">

      <!-- Invoice Stats -->
      <div class="grid grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-4">
        <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div class="flex-1 min-w-0">
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mb-1">Total Invoices</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white truncate">{{ totalInvoices }}</p>
              <p class="text-xs sm:text-sm text-blue-600 mt-1">{{ newInvoices }} this month</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-blue-500 to-indigo-600 rounded-xl shadow-lg flex-shrink-0">
              <DocumentTextIcon class="w-5 h-5 sm:w-6 sm:h-6 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div class="flex-1 min-w-0">
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mb-1">Outstanding</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white truncate">R{{ formatCurrency(outstandingAmount) }}</p>
              <p class="text-xs sm:text-sm text-red-600 mt-1">{{ overdueInvoices }} overdue</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-red-500 to-pink-600 rounded-xl shadow-lg flex-shrink-0">
              <ExclamationTriangleIcon class="w-5 h-5 sm:w-6 sm:h-6 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div class="flex-1 min-w-0">
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mb-1">Paid This Month</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white truncate">R{{ formatCurrency(paidThisMonth) }}</p>
              <p class="text-xs sm:text-sm text-green-600 mt-1">{{ paidInvoices }} invoices</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl shadow-lg flex-shrink-0">
              <CheckCircleIcon class="w-5 h-5 sm:w-6 sm:h-6 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div class="flex-1 min-w-0">
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mb-1">Average Invoice</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white truncate">R{{ formatCurrency(avgInvoiceValue) }}</p>
              <p class="text-xs sm:text-sm text-purple-600 mt-1">{{ paymentTerms }} day terms</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-purple-500 to-violet-600 rounded-xl shadow-lg flex-shrink-0">
              <CalculatorIcon class="w-5 h-5 sm:w-6 sm:h-6 text-white" />
            </div>
          </div>
        </div>
      </div>

      <!-- Filters and Search -->
      <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6">
        <div class="flex flex-col sm:flex-row gap-3 sm:gap-4">
          <div class="flex-1">
            <input v-model="searchQuery" type="text" placeholder="Search invoices..." 
                   class="w-full px-3 sm:px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
          </div>
          <div class="flex gap-2 sm:gap-3">
            <select v-model="statusFilter" 
                    class="px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
              <option value="">All Status</option>
              <option value="draft">Draft</option>
              <option value="sent">Sent</option>
              <option value="viewed">Viewed</option>
              <option value="paid">Paid</option>
              <option value="overdue">Overdue</option>
              <option value="cancelled">Cancelled</option>
            </select>
            <select v-model="periodFilter" 
                    class="px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
              <option value="">All Time</option>
              <option value="today">Today</option>
              <option value="week">This Week</option>
              <option value="month">This Month</option>
              <option value="quarter">This Quarter</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Invoices List -->
      <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 overflow-hidden">
        <div class="bg-gradient-to-r from-orange-50 to-blue-50 dark:from-orange-900/20 dark:to-blue-900/20 px-4 sm:px-6 py-4 border-b border-slate-200 dark:border-slate-600">
          <h3 class="text-base sm:text-lg font-bold text-slate-900 dark:text-white">Recent Invoices</h3>
        </div>
        <div class="p-4 sm:p-6">
          <div class="space-y-3 sm:space-y-4">
            <div v-for="invoice in filteredInvoices" :key="invoice.id" 
                 class="flex items-center justify-between p-4 rounded-xl border border-slate-200 dark:border-slate-700 hover:bg-slate-50 dark:hover:bg-slate-700 transition-all duration-200 hover:shadow-md">
              <div class="flex items-center space-x-3 flex-1 min-w-0">
                <div class="w-10 h-10 rounded-full flex items-center justify-center" :class="getStatusColor(invoice.status)">
                  <DocumentTextIcon class="w-5 h-5 text-white" />
                </div>
                <div class="flex-1 min-w-0">
                  <div class="flex items-center gap-2">
                    <p class="text-sm font-medium text-slate-900 dark:text-white truncate">{{ invoice.invoiceNumber }}</p>
                    <span class="inline-flex px-2 py-1 text-xs rounded-full" :class="getStatusBadge(invoice.status)">
                      {{ invoice.status }}
                    </span>
                  </div>
                  <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">{{ invoice.customer }}</p>
                  <p class="text-xs text-slate-500 dark:text-slate-500">Due {{ formatDueDate(invoice.dueDate) }}</p>
                </div>
              </div>
              <div class="text-right">
                <p class="text-sm font-semibold text-slate-900 dark:text-white">R {{ formatCurrency(invoice.amount) }}</p>
                <div class="flex gap-1 mt-1">
                  <button @click="viewInvoice(invoice)" class="p-1 text-blue-600 hover:bg-blue-50 dark:hover:bg-blue-900 rounded">
                    <EyeIcon class="w-4 h-4" />
                  </button>
                  <button @click="sendInvoice(invoice)" class="p-1 text-green-600 hover:bg-green-50 dark:hover:bg-green-900 rounded">
                    <PaperAirplaneIcon class="w-4 h-4" />
                  </button>
                  <button @click="printInvoice(invoice)" class="p-1 text-purple-600 hover:bg-purple-50 dark:hover:bg-purple-900 rounded">
                    <PrinterIcon class="w-4 h-4" />
                  </button>
                  <button @click="markAsPaid(invoice)" class="p-1 text-emerald-600 hover:bg-emerald-50 dark:hover:bg-emerald-900 rounded">
                    <BanknotesIcon class="w-4 h-4" />
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- New Invoice Modal -->
    <div v-if="showNewInvoiceModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-slate-800 rounded-xl sm:rounded-2xl shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-lg sm:text-xl font-semibold text-slate-900 dark:text-white">Create New Invoice</h3>
        </div>
        <div class="p-4 sm:p-6">
          <form @submit.prevent="createInvoice">
            <div class="space-y-4">
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Invoice Number</label>
                  <input v-model="newInvoice.invoiceNumber" type="text" required readonly
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-slate-50 dark:bg-slate-700 dark:text-white">
                </div>
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Due Date</label>
                  <input v-model="newInvoice.dueDate" type="date" required
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Customer Information</label>
                <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <input v-model="newInvoice.customerName" placeholder="Customer Name" required
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                  <input v-model="newInvoice.customerEmail" type="email" placeholder="Email Address"
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Billing Address</label>
                <textarea v-model="newInvoice.billingAddress" rows="2" placeholder="Enter billing address..."
                          class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white"></textarea>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Invoice Items</label>
                <div class="space-y-2">
                  <div v-for="(item, index) in newInvoice.items" :key="index" class="flex gap-2 items-end">
                    <div class="flex-1">
                      <input v-model="item.description" placeholder="Item description" required
                             class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white text-sm">
                    </div>
                    <div class="w-20">
                      <input v-model.number="item.quantity" type="number" placeholder="Qty" min="1" required
                             class="w-full px-2 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white text-sm">
                    </div>
                    <div class="w-24">
                      <input v-model.number="item.unitPrice" type="number" step="0.01" placeholder="Price" min="0" required
                             class="w-full px-2 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white text-sm">
                    </div>
                    <div class="w-24 text-right">
                      <p class="text-sm font-medium text-slate-900 dark:text-white py-2">R {{ formatCurrency(item.quantity * item.unitPrice) }}</p>
                    </div>
                    <button type="button" @click="removeInvoiceItem(index)" class="p-2 text-red-600 hover:bg-red-50 dark:hover:bg-red-900 rounded-lg">
                      <XMarkIcon class="w-4 h-4" />
                    </button>
                  </div>
                </div>
                <button type="button" @click="addInvoiceItem" class="mt-2 text-sm text-blue-600 hover:text-blue-700">
                  + Add Item
                </button>
              </div>

              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Tax Rate (%)</label>
                  <input v-model.number="newInvoice.taxRate" type="number" step="0.1" min="0" max="100"
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                </div>
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Discount (%)</label>
                  <input v-model.number="newInvoice.discountRate" type="number" step="0.1" min="0" max="100"
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Notes</label>
                <textarea v-model="newInvoice.notes" rows="2" placeholder="Additional notes or payment terms..."
                          class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white"></textarea>
              </div>

              <div class="bg-blue-50 dark:bg-blue-900/20 p-4 rounded-lg">
                <div class="space-y-2">
                  <div class="flex justify-between">
                    <span class="text-sm text-slate-600 dark:text-slate-400">Subtotal:</span>
                    <span class="text-sm font-medium text-slate-900 dark:text-white">R {{ formatCurrency(calculateSubtotal()) }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-sm text-slate-600 dark:text-slate-400">Tax ({{ newInvoice.taxRate }}%):</span>
                    <span class="text-sm font-medium text-slate-900 dark:text-white">R {{ formatCurrency(calculateTax()) }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-sm text-slate-600 dark:text-slate-400">Discount ({{ newInvoice.discountRate }}%):</span>
                    <span class="text-sm font-medium text-red-600">-R {{ formatCurrency(calculateDiscount()) }}</span>
                  </div>
                  <div class="border-t pt-2 flex justify-between items-center">
                    <span class="text-lg font-semibold text-slate-900 dark:text-white">Total:</span>
                    <span class="text-xl font-bold text-blue-600 dark:text-blue-400">R {{ formatCurrency(calculateInvoiceTotal()) }}</span>
                  </div>
                </div>
              </div>
            </div>

            <div class="flex justify-end space-x-3 mt-6">
              <button @click="showNewInvoiceModal = false" type="button" 
                      class="px-4 py-2 text-slate-600 dark:text-slate-400 hover:text-slate-800 dark:hover:text-slate-200">
                Cancel
              </button>
              <button @click="saveAsDraft" type="button" 
                      class="px-6 py-2 border border-slate-300 dark:border-slate-600 text-slate-700 dark:text-slate-300 rounded-lg hover:bg-slate-50 dark:hover:bg-slate-700">
                Save as Draft
              </button>
              <button type="submit" 
                      class="px-6 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg">
                Create & Send
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
  PlusIcon,
  ArrowDownTrayIcon,
  ExclamationTriangleIcon,
  CheckCircleIcon,
  CalculatorIcon,
  EyeIcon,
  PaperAirplaneIcon,
  PrinterIcon,
  BanknotesIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Sales Invoices - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage billing and payments for Thabo\'s Spaza Shop' }
  ]
})

// Layout
definePageMeta({
  layout: 'default'
})

// Reactive data
const showNewInvoiceModal = ref(false)
const searchQuery = ref('')
const statusFilter = ref('')
const periodFilter = ref('')

// Invoice statistics
const totalInvoices = ref(89)
const newInvoices = ref(12)
const outstandingAmount = ref(28450)
const overdueInvoices = ref(3)
const paidThisMonth = ref(145800)
const paidInvoices = ref(23)
const avgInvoiceValue = ref(2850)
const paymentTerms = ref(30)

// Sample invoices data for Thabo's Spaza Shop
const invoices = ref([
  {
    id: '1',
    invoiceNumber: 'INV-2025-001',
    customer: 'Nomsa Community Kitchen',
    customerEmail: 'nomsa.kitchen@gmail.com',
    amount: 4850,
    status: 'sent',
    issueDate: new Date(),
    dueDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000),
    billingAddress: '123 Community Street, Soweto, 1818',
    notes: 'Monthly catering supplies order'
  },
  {
    id: '2',
    invoiceNumber: 'INV-2025-002',
    customer: 'Sipho Auto Repair',
    customerEmail: 'sipho.auto@outlook.com',
    amount: 1250,
    status: 'paid',
    issueDate: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000),
    dueDate: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
    billingAddress: '456 Garage Road, Alexandra, 2090',
    notes: 'Workshop consumables and refreshments'
  },
  {
    id: '3',
    invoiceNumber: 'INV-2025-003',
    customer: 'Lerato Hair Studio',
    customerEmail: 'lerato.hair@yahoo.com',
    amount: 890,
    status: 'overdue',
    issueDate: new Date(Date.now() - 45 * 24 * 60 * 60 * 1000),
    dueDate: new Date(Date.now() - 15 * 24 * 60 * 60 * 1000),
    billingAddress: '789 Beauty Lane, Diepsloot, 2189',
    notes: 'Hair care products - regular client'
  },
  {
    id: '4',
    invoiceNumber: 'INV-2025-004',
    customer: 'Mandla Construction',
    customerEmail: 'mandla.builds@hotmail.com',
    amount: 12500,
    status: 'viewed',
    issueDate: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
    dueDate: new Date(Date.now() + 28 * 24 * 60 * 60 * 1000),
    billingAddress: 'Construction Site, 321 Building Ave, Orange Farm, 1841',
    notes: 'Bulk supplies for construction crew'
  },
  {
    id: '5',
    invoiceNumber: 'INV-2025-005',
    customer: 'Grace Catering Services',
    customerEmail: 'grace.catering@gmail.com',
    amount: 3200,
    status: 'draft',
    issueDate: new Date(),
    dueDate: new Date(Date.now() + 15 * 24 * 60 * 60 * 1000),
    billingAddress: '654 Event Hall, Tembisa, 1632',
    notes: 'Wedding event supplies - rush order'
  }
])

// Form data
const newInvoice = ref({
  invoiceNumber: generateInvoiceNumber(),
  customerName: '',
  customerEmail: '',
  billingAddress: '',
  dueDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
  items: [
    { description: '', quantity: 1, unitPrice: 0 }
  ],
  taxRate: 15,
  discountRate: 0,
  notes: 'Payment due within 30 days. Thank you for your business!'
})

// Computed
const filteredInvoices = computed(() => {
  let filtered = invoices.value

  if (searchQuery.value) {
    filtered = filtered.filter((invoice: any) => 
      invoice.customer.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      invoice.invoiceNumber.toLowerCase().includes(searchQuery.value.toLowerCase())
    )
  }

  if (statusFilter.value) {
    filtered = filtered.filter((invoice: any) => invoice.status === statusFilter.value)
  }

  if (periodFilter.value) {
    const now = new Date()
    filtered = filtered.filter(invoice => {
      const issueDate = new Date(invoice.issueDate)
      switch (periodFilter.value) {
        case 'today':
          return issueDate.toDateString() === now.toDateString()
        case 'week':
          const weekAgo = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000)
          return issueDate >= weekAgo
        case 'month':
          return issueDate.getMonth() === now.getMonth() && issueDate.getFullYear() === now.getFullYear()
        case 'quarter':
          const quarter = Math.floor(now.getMonth() / 3)
          const invoiceQuarter = Math.floor(issueDate.getMonth() / 3)
          return invoiceQuarter === quarter && issueDate.getFullYear() === now.getFullYear()
        default:
          return true
      }
    })
  }

  return filtered
})

// Helper functions
function generateInvoiceNumber() {
  const date = new Date()
  const year = date.getFullYear()
  const nextNumber = (invoices.value.length + 1).toString().padStart(3, '0')
  return `INV-${year}-${nextNumber}`
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

const formatDueDate = (date: Date) => {
  const now = new Date()
  const dueDate = new Date(date)
  const diffDays = Math.ceil((dueDate.getTime() - now.getTime()) / (1000 * 60 * 60 * 24))
  
  if (diffDays < 0) {
    return `${Math.abs(diffDays)} days overdue`
  } else if (diffDays === 0) {
    return 'Due today'
  } else if (diffDays === 1) {
    return 'Due tomorrow'
  } else {
    return `Due in ${diffDays} days`
  }
}

const getStatusColor = (status: string) => {
  const colors = {
    draft: 'bg-slate-600',
    sent: 'bg-blue-600',
    viewed: 'bg-purple-600',
    paid: 'bg-green-600',
    overdue: 'bg-red-600',
    cancelled: 'bg-gray-600'
  }
  return colors[status as keyof typeof colors] || 'bg-slate-600'
}

const getStatusBadge = (status: string) => {
  const badges = {
    draft: 'bg-slate-100 text-slate-800 dark:bg-slate-900 dark:text-slate-200',
    sent: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    viewed: 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200',
    paid: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    overdue: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    cancelled: 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200'
  }
  return badges[status as keyof typeof badges] || 'bg-slate-100 text-slate-800'
}

// Invoice calculation functions
const calculateSubtotal = () => {
  return newInvoice.value.items.reduce((total: number, item: any) => {
    return total + (item.quantity * item.unitPrice)
  }, 0)
}

const calculateTax = () => {
  const subtotal = calculateSubtotal()
  return subtotal * (newInvoice.value.taxRate / 100)
}

const calculateDiscount = () => {
  const subtotal = calculateSubtotal()
  return subtotal * (newInvoice.value.discountRate / 100)
}

const calculateInvoiceTotal = () => {
  const subtotal = calculateSubtotal()
  const tax = calculateTax()
  const discount = calculateDiscount()
  return subtotal + tax - discount
}

// Invoice form functions
const addInvoiceItem = () => {
  newInvoice.value.items.push({ description: '', quantity: 1, unitPrice: 0 })
}

const removeInvoiceItem = (index: number) => {
  if (newInvoice.value.items.length > 1) {
    newInvoice.value.items.splice(index, 1)
  }
}

// Actions
const createInvoice = async (sendImmediately = true) => {
  try {
    const invoice = {
      id: Date.now().toString(),
      invoiceNumber: newInvoice.value.invoiceNumber,
      customer: newInvoice.value.customerName,
      customerEmail: newInvoice.value.customerEmail,
      amount: calculateInvoiceTotal(),
      status: sendImmediately ? 'sent' : 'draft',
      issueDate: new Date(),
      dueDate: new Date(newInvoice.value.dueDate),
      billingAddress: newInvoice.value.billingAddress,
      notes: newInvoice.value.notes
    }

    invoices.value.unshift(invoice)
    totalInvoices.value += 1
    
    showNewInvoiceModal.value = false
    
    // Reset form
    newInvoice.value = {
      invoiceNumber: generateInvoiceNumber(),
      customerName: '',
      customerEmail: '',
      billingAddress: '',
      dueDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
      items: [{ description: '', quantity: 1, unitPrice: 0 }],
      taxRate: 15,
      discountRate: 0,
      notes: 'Payment due within 30 days. Thank you for your business!'
    }
    
    alert(`Invoice ${invoice.invoiceNumber} ${sendImmediately ? 'created and sent' : 'saved as draft'} successfully!`)
  } catch (error) {
    console.error('Error creating invoice:', error)
    alert('Failed to create invoice. Please try again.')
  }
}

const saveAsDraft = () => {
  createInvoice(false)
}

const viewInvoice = (invoice: any) => {
  alert(`Viewing invoice ${invoice.invoiceNumber} for ${invoice.customer}`)
}

const sendInvoice = (invoice: any) => {
  if (invoice.status === 'draft') {
    invoice.status = 'sent'
    alert(`Invoice ${invoice.invoiceNumber} sent to ${invoice.customer}`)
  } else {
    alert(`Invoice ${invoice.invoiceNumber} resent to ${invoice.customer}`)
  }
}

const printInvoice = (invoice: any) => {
  alert(`Printing invoice ${invoice.invoiceNumber}`)
}

const markAsPaid = (invoice: any) => {
  if (invoice.status !== 'paid') {
    invoice.status = 'paid'
    paidThisMonth.value += invoice.amount
    paidInvoices.value += 1
    if (invoice.status === 'overdue') {
      overdueInvoices.value -= 1
    }
    outstandingAmount.value -= invoice.amount
    alert(`Invoice ${invoice.invoiceNumber} marked as paid`)
  }
}

const exportInvoices = () => {
  alert('Exporting invoices... Feature coming soon!')
}
</script>
