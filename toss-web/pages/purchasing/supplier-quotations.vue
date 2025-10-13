<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Supplier Quotations</h1>
              <p class="text-gray-600 dark:text-gray-400">Review and compare supplier quotes for purchase decisions</p>
            </div>
            <div class="flex space-x-3">
              <button @click="openComparisonTool" class="bg-purple-600 text-white px-4 py-2 rounded-lg hover:bg-purple-700 transition-colors flex items-center">
                <ChartBarIcon class="w-5 h-5 mr-2" />
                Compare Quotes
              </button>
              <button @click="exportQuotations" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center">
                <ArrowDownTrayIcon class="w-5 h-5 mr-2" />
                Export
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Quotation Stats -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="grid grid-cols-1 md:grid-cols-5 gap-6 mb-8">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-blue-100 dark:bg-blue-900/30">
              <DocumentTextIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total Quotations</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.totalQuotations }}</p>
              <p class="text-xs text-gray-500 dark:text-gray-500">{{ stats.thisWeek }} this week</p>
            </div>
          </div>
        </div>
        
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-yellow-100 dark:bg-yellow-900/30">
              <ClockIcon class="w-6 h-6 text-yellow-600 dark:text-yellow-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Under Review</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.underReview }}</p>
              <p class="text-xs text-gray-500 dark:text-gray-500">Pending decision</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-green-100 dark:bg-green-900/30">
              <TrophyIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Awarded</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.awarded }}</p>
              <p class="text-xs text-gray-500 dark:text-gray-500">Converted to PO</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-purple-100 dark:bg-purple-900/30">
              <CurrencyDollarIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Best Price Avg</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">R {{ stats.bestPriceAvg }}K</p>
              <p class="text-xs text-gray-500 dark:text-gray-500">Per quotation</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-orange-100 dark:bg-orange-900/30">
              <PercentBadgeIcon class="w-6 h-6 text-orange-600 dark:text-orange-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Cost Reduction</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.costReduction }}%</p>
              <p class="text-xs text-gray-500 dark:text-gray-500">vs highest quote</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Filters -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6 mb-6">
        <div class="grid grid-cols-1 md:grid-cols-5 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Search</label>
            <div class="relative">
              <MagnifyingGlassIcon class="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
              <input 
                v-model="searchQuery"
                type="text" 
                placeholder="Search quotations..."
                class="w-full pl-10 pr-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              />
            </div>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Status</label>
            <select v-model="selectedStatus" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Status</option>
              <option value="submitted">Submitted</option>
              <option value="under-review">Under Review</option>
              <option value="accepted">Accepted</option>
              <option value="rejected">Rejected</option>
              <option value="expired">Expired</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Supplier</label>
            <select v-model="selectedSupplier" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Suppliers</option>
              <option value="Tech Solutions Inc">Tech Solutions Inc</option>
              <option value="Raw Materials Corp">Raw Materials Corp</option>
              <option value="Quality Equipment Co">Quality Equipment Co</option>
              <option value="Industrial Supplies SA">Industrial Supplies SA</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">RFQ Reference</label>
            <select v-model="selectedRFQ" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All RFQs</option>
              <option value="RFQ-2025-001">RFQ-2025-001</option>
              <option value="RFQ-2025-002">RFQ-2025-002</option>
              <option value="RFQ-2025-003">RFQ-2025-003</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Date Range</label>
            <select v-model="selectedDateRange" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Dates</option>
              <option value="today">Today</option>
              <option value="week">This Week</option>
              <option value="month">This Month</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Quotations Grid -->
      <div class="grid grid-cols-1 gap-6">
        <div v-for="quotation in filteredQuotations" :key="quotation.id" class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden">
          <!-- Quotation Header -->
          <div class="bg-gray-50 dark:bg-gray-700 px-6 py-4 border-b border-gray-200 dark:border-gray-600">
            <div class="flex items-center justify-between">
              <div class="flex items-center space-x-4">
                <div class="flex-shrink-0 h-12 w-12">
                  <div class="h-12 w-12 rounded-full bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center">
                    <span class="text-lg font-medium text-white">{{ quotation.supplier.charAt(0) }}</span>
                  </div>
                </div>
                <div>
                  <h3 class="text-lg font-semibold text-gray-900 dark:text-white">{{ quotation.supplier }}</h3>
                  <p class="text-sm text-gray-600 dark:text-gray-400">Quote #{{ quotation.quoteNumber }} • RFQ: {{ quotation.rfqNumber }}</p>
                </div>
              </div>
              <div class="flex items-center space-x-3">
                <span class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium"
                      :class="getStatusClass(quotation.status)">
                  {{ quotation.status }}
                </span>
                <div class="text-right">
                  <p class="text-2xl font-bold text-gray-900 dark:text-white">R {{ quotation.totalAmount.toLocaleString() }}</p>
                  <p class="text-xs text-gray-500 dark:text-gray-500">Total (incl. VAT)</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Quotation Details -->
          <div class="px-6 py-4">
            <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-4">
              <div>
                <p class="text-xs text-gray-500 dark:text-gray-500 mb-1">Valid Until</p>
                <p class="text-sm font-medium text-gray-900 dark:text-white">{{ formatDate(quotation.validUntil) }}</p>
                <p v-if="getDaysUntil(quotation.validUntil) <= 3" class="text-xs text-red-500">
                  Expires in {{ getDaysUntil(quotation.validUntil) }} days
                </p>
              </div>
              <div>
                <p class="text-xs text-gray-500 dark:text-gray-500 mb-1">Delivery Time</p>
                <p class="text-sm font-medium text-gray-900 dark:text-white">{{ quotation.deliveryTime }} days</p>
              </div>
              <div>
                <p class="text-xs text-gray-500 dark:text-gray-500 mb-1">Payment Terms</p>
                <p class="text-sm font-medium text-gray-900 dark:text-white">{{ quotation.paymentTerms }}</p>
              </div>
              <div>
                <p class="text-xs text-gray-500 dark:text-gray-500 mb-1">Rating</p>
                <div class="flex items-center">
                  <StarIcon v-for="i in 5" :key="i" class="w-4 h-4" :class="i <= quotation.supplierRating ? 'text-yellow-400' : 'text-gray-300'" />
                  <span class="ml-1 text-sm text-gray-600 dark:text-gray-400">({{ quotation.supplierRating }})</span>
                </div>
              </div>
            </div>

            <!-- Line Items -->
            <div class="border-t border-gray-200 dark:border-gray-700 pt-4 mb-4">
              <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-3">Quoted Items</h4>
              <div class="space-y-2">
                <div v-for="(item, idx) in quotation.items" :key="idx" class="flex items-center justify-between py-2 px-3 bg-gray-50 dark:bg-gray-700 rounded">
                  <div class="flex-1">
                    <p class="text-sm font-medium text-gray-900 dark:text-white">{{ item.itemName }}</p>
                    <p class="text-xs text-gray-500 dark:text-gray-500">{{ item.quantity }} {{ item.uom }}</p>
                  </div>
                  <div class="text-right">
                    <p class="text-sm font-medium text-gray-900 dark:text-white">R {{ item.unitPrice.toLocaleString() }}</p>
                    <p class="text-xs text-gray-500 dark:text-gray-500">per {{ item.uom }}</p>
                  </div>
                  <div class="text-right ml-4">
                    <p class="text-sm font-bold text-gray-900 dark:text-white">R {{ (item.quantity * item.unitPrice).toLocaleString() }}</p>
                    <p class="text-xs text-gray-500 dark:text-gray-500">subtotal</p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Price Breakdown -->
            <div class="border-t border-gray-200 dark:border-gray-700 pt-4 mb-4">
              <div class="space-y-2 max-w-md ml-auto">
                <div class="flex justify-between text-sm">
                  <span class="text-gray-600 dark:text-gray-400">Subtotal:</span>
                  <span class="font-medium text-gray-900 dark:text-white">R {{ quotation.subtotal.toLocaleString() }}</span>
                </div>
                <div class="flex justify-between text-sm">
                  <span class="text-gray-600 dark:text-gray-400">VAT (15%):</span>
                  <span class="font-medium text-gray-900 dark:text-white">R {{ quotation.vat.toLocaleString() }}</span>
                </div>
                <div v-if="quotation.discount > 0" class="flex justify-between text-sm">
                  <span class="text-gray-600 dark:text-gray-400">Discount:</span>
                  <span class="font-medium text-green-600">-R {{ quotation.discount.toLocaleString() }}</span>
                </div>
                <div class="flex justify-between text-base font-bold border-t pt-2">
                  <span class="text-gray-900 dark:text-white">Total:</span>
                  <span class="text-gray-900 dark:text-white">R {{ quotation.totalAmount.toLocaleString() }}</span>
                </div>
              </div>
            </div>

            <!-- Actions -->
            <div class="flex items-center justify-between pt-4 border-t border-gray-200 dark:border-gray-700">
              <div class="flex space-x-2">
                <button @click="viewQuotation(quotation)" class="text-blue-600 hover:text-blue-800 text-sm font-medium flex items-center">
                  <EyeIcon class="w-4 h-4 mr-1" />
                  View Full Details
                </button>
                <button @click="negotiateTerms(quotation)" class="text-purple-600 hover:text-purple-800 text-sm font-medium flex items-center">
                  <ChatBubbleLeftRightIcon class="w-4 h-4 mr-1" />
                  Negotiate
                </button>
                <button @click="requestRevision(quotation)" class="text-orange-600 hover:text-orange-800 text-sm font-medium flex items-center">
                  <ArrowPathIcon class="w-4 h-4 mr-1" />
                  Request Revision
                </button>
              </div>
              <div class="flex space-x-2">
                <button v-if="quotation.status === 'submitted' || quotation.status === 'under-review'" 
                        @click="acceptQuotation(quotation)" 
                        class="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 text-sm font-medium">
                  Accept & Award
                </button>
                <button v-if="quotation.status === 'submitted' || quotation.status === 'under-review'" 
                        @click="rejectQuotation(quotation)" 
                        class="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 text-sm font-medium">
                  Reject
                </button>
                <button @click="downloadQuotation(quotation)" class="p-2 text-gray-600 hover:text-gray-900 dark:text-gray-400">
                  <ArrowDownTrayIcon class="w-5 h-5" />
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Comparison Tool Modal -->
    <div v-if="showComparisonModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-10 mx-auto p-5 border w-11/12 shadow-lg rounded-md bg-white dark:bg-gray-800 max-h-[90vh] overflow-y-auto">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Quote Comparison Matrix</h3>
            <button @click="closeComparisonModal" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-200">
              <XMarkIcon class="w-6 h-6" />
            </button>
          </div>
          
          <!-- Comparison Table -->
          <div class="overflow-x-auto">
            <table class="w-full border-collapse">
              <thead>
                <tr class="bg-gray-50 dark:bg-gray-700">
                  <th class="px-4 py-3 text-left text-sm font-medium text-gray-900 dark:text-white border border-gray-200 dark:border-gray-600">Criteria</th>
                  <th v-for="quote in comparisonQuotes" :key="quote.id" 
                      class="px-4 py-3 text-center text-sm font-medium text-gray-900 dark:text-white border border-gray-200 dark:border-gray-600">
                    {{ quote.supplier }}
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td class="px-4 py-3 text-sm font-medium text-gray-700 dark:text-gray-300 border border-gray-200 dark:border-gray-600">Total Price</td>
                  <td v-for="quote in comparisonQuotes" :key="quote.id" 
                      class="px-4 py-3 text-center text-sm border border-gray-200 dark:border-gray-600"
                      :class="quote.totalAmount === Math.min(...comparisonQuotes.map(q => q.totalAmount)) ? 'bg-green-50 dark:bg-green-900/20 font-bold text-green-600' : ''">
                    R {{ quote.totalAmount.toLocaleString() }}
                  </td>
                </tr>
                <tr>
                  <td class="px-4 py-3 text-sm font-medium text-gray-700 dark:text-gray-300 border border-gray-200 dark:border-gray-600">Delivery Time</td>
                  <td v-for="quote in comparisonQuotes" :key="quote.id" 
                      class="px-4 py-3 text-center text-sm border border-gray-200 dark:border-gray-600"
                      :class="quote.deliveryTime === Math.min(...comparisonQuotes.map(q => q.deliveryTime)) ? 'bg-green-50 dark:bg-green-900/20 font-bold text-green-600' : ''">
                    {{ quote.deliveryTime }} days
                  </td>
                </tr>
                <tr>
                  <td class="px-4 py-3 text-sm font-medium text-gray-700 dark:text-gray-300 border border-gray-200 dark:border-gray-600">Payment Terms</td>
                  <td v-for="quote in comparisonQuotes" :key="quote.id" 
                      class="px-4 py-3 text-center text-sm border border-gray-200 dark:border-gray-600">
                    {{ quote.paymentTerms }}
                  </td>
                </tr>
                <tr>
                  <td class="px-4 py-3 text-sm font-medium text-gray-700 dark:text-gray-300 border border-gray-200 dark:border-gray-600">Supplier Rating</td>
                  <td v-for="quote in comparisonQuotes" :key="quote.id" 
                      class="px-4 py-3 text-center text-sm border border-gray-200 dark:border-gray-600">
                    <div class="flex items-center justify-center">
                      <StarIcon v-for="i in 5" :key="i" class="w-3 h-3" :class="i <= quote.supplierRating ? 'text-yellow-400' : 'text-gray-300'" />
                    </div>
                  </td>
                </tr>
                <tr>
                  <td class="px-4 py-3 text-sm font-medium text-gray-700 dark:text-gray-300 border border-gray-200 dark:border-gray-600">Warranty</td>
                  <td v-for="quote in comparisonQuotes" :key="quote.id" 
                      class="px-4 py-3 text-center text-sm border border-gray-200 dark:border-gray-600">
                    {{ quote.warranty || 'Standard' }}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <div class="mt-6 flex justify-end">
            <button @click="closeComparisonModal" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
              Close Comparison
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import {
  ArrowDownTrayIcon,
  ChartBarIcon,
  DocumentTextIcon,
  ClockIcon,
  TrophyIcon,
  CurrencyDollarIcon,
  PercentBadgeIcon,
  MagnifyingGlassIcon,
  EyeIcon,
  ChatBubbleLeftRightIcon,
  ArrowPathIcon,
  PrinterIcon,
  XMarkIcon,
  StarIcon,
  CheckCircleIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Supplier Quotations - TOSS ERP',
  meta: [
    { name: 'description', content: 'Review and compare supplier quotations in TOSS ERP' }
  ]
})

// Reactive data
const searchQuery = ref('')
const selectedStatus = ref('')
const selectedSupplier = ref('')
const selectedRFQ = ref('')
const selectedDateRange = ref('')
const showComparisonModal = ref(false)
const comparisonQuotes = ref<any[]>([])

// Stats
const stats = ref({
  totalQuotations: 156,
  thisWeek: 18,
  underReview: 12,
  awarded: 89,
  bestPriceAvg: 145,
  costReduction: 18.5
})

// Mock quotations data
const quotations = ref([
  {
    id: 1,
    quoteNumber: 'SQ-2025-001',
    rfqNumber: 'RFQ-2025-001',
    supplier: 'Tech Solutions Inc',
    supplierRating: 5,
    status: 'submitted',
    validUntil: new Date('2025-01-30'),
    deliveryTime: 14,
    paymentTerms: 'Net 30',
    warranty: '12 months',
    items: [
      { itemName: 'Office Desks', quantity: 10, uom: 'Nos', unitPrice: 8500 },
      { itemName: 'Ergonomic Chairs', quantity: 10, uom: 'Nos', unitPrice: 4200 },
      { itemName: 'Filing Cabinets', quantity: 5, uom: 'Nos', unitPrice: 3500 }
    ],
    subtotal: 110000,
    vat: 16500,
    discount: 0,
    totalAmount: 126500,
    submittedAt: new Date('2025-01-13')
  },
  {
    id: 2,
    quoteNumber: 'SQ-2025-002',
    rfqNumber: 'RFQ-2025-001',
    supplier: 'Quality Equipment Co',
    supplierRating: 5,
    status: 'under-review',
    validUntil: new Date('2025-01-28'),
    deliveryTime: 10,
    paymentTerms: 'Net 60',
    warranty: '24 months',
    items: [
      { itemName: 'Office Desks', quantity: 10, uom: 'Nos', unitPrice: 7800 },
      { itemName: 'Ergonomic Chairs', quantity: 10, uom: 'Nos', unitPrice: 3900 },
      { itemName: 'Filing Cabinets', quantity: 5, uom: 'Nos', unitPrice: 3200 }
    ],
    subtotal: 101000,
    vat: 15150,
    discount: 5000,
    totalAmount: 111150,
    submittedAt: new Date('2025-01-13')
  },
  {
    id: 3,
    quoteNumber: 'SQ-2025-003',
    rfqNumber: 'RFQ-2025-002',
    supplier: 'Raw Materials Corp',
    supplierRating: 4,
    status: 'accepted',
    validUntil: new Date('2025-02-05'),
    deliveryTime: 7,
    paymentTerms: 'Net 30',
    warranty: 'Standard',
    items: [
      { itemName: 'Steel Sheets', quantity: 500, uom: 'Kg', unitPrice: 145 },
      { itemName: 'Aluminum Rods', quantity: 200, uom: 'Meter', unitPrice: 89 }
    ],
    subtotal: 90300,
    vat: 13545,
    discount: 0,
    totalAmount: 103845,
    submittedAt: new Date('2025-01-11')
  },
  {
    id: 4,
    quoteNumber: 'SQ-2025-004',
    rfqNumber: 'RFQ-2025-002',
    supplier: 'Industrial Supplies SA',
    supplierRating: 4,
    status: 'rejected',
    validUntil: new Date('2025-02-01'),
    deliveryTime: 21,
    paymentTerms: 'COD',
    warranty: '6 months',
    items: [
      { itemName: 'Steel Sheets', quantity: 500, uom: 'Kg', unitPrice: 168 },
      { itemName: 'Aluminum Rods', quantity: 200, uom: 'Meter', unitPrice: 105 }
    ],
    subtotal: 105000,
    vat: 15750,
    discount: 0,
    totalAmount: 120750,
    submittedAt: new Date('2025-01-11')
  }
])

// Computed filtered quotations
const filteredQuotations = computed(() => {
  return quotations.value.filter(quotation => {
    const matchesSearch = !searchQuery.value || 
      quotation.quoteNumber.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      quotation.supplier.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      quotation.rfqNumber.toLowerCase().includes(searchQuery.value.toLowerCase())
    
    const matchesStatus = !selectedStatus.value || quotation.status === selectedStatus.value
    const matchesSupplier = !selectedSupplier.value || quotation.supplier === selectedSupplier.value
    const matchesRFQ = !selectedRFQ.value || quotation.rfqNumber === selectedRFQ.value
    
    return matchesSearch && matchesStatus && matchesSupplier && matchesRFQ
  })
})

// Helper functions
const getStatusClass = (status: string) => {
  const classes = {
    submitted: 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
    'under-review': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    accepted: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    rejected: 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400',
    expired: 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400'
  }
  return classes[status as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const formatDate = (date: Date) => {
  return date.toLocaleDateString('en-US', { 
    year: 'numeric', 
    month: 'short', 
    day: 'numeric' 
  })
}

const getDaysUntil = (date: Date) => {
  const today = new Date()
  const diffTime = date.getTime() - today.getTime()
  return Math.ceil(diffTime / (1000 * 60 * 60 * 24))
}

// Comparison tool
const openComparisonTool = () => {
  // Get all quotations for the same RFQ
  const rfqNumber = 'RFQ-2025-001' // In real app, user would select which RFQ to compare
  comparisonQuotes.value = quotations.value.filter(q => q.rfqNumber === rfqNumber)
  
  if (comparisonQuotes.value.length < 2) {
    alert('Select an RFQ with multiple quotations to compare')
    return
  }
  
  showComparisonModal.value = true
}

const closeComparisonModal = () => {
  showComparisonModal.value = false
  comparisonQuotes.value = []
}

// Action functions
const viewQuotation = (quotation: any) => {
  const details = `
Supplier Quotation: ${quotation.quoteNumber}
RFQ Reference: ${quotation.rfqNumber}
Supplier: ${quotation.supplier}
Status: ${quotation.status}
Valid Until: ${formatDate(quotation.validUntil)}
Delivery Time: ${quotation.deliveryTime} days
Payment Terms: ${quotation.paymentTerms}

Items:
${quotation.items.map((item: any) => `- ${item.itemName}: ${item.quantity} ${item.uom} @ R${item.unitPrice.toLocaleString()}`).join('\n')}

Subtotal: R ${quotation.subtotal.toLocaleString()}
VAT (15%): R ${quotation.vat.toLocaleString()}
${quotation.discount > 0 ? `Discount: -R ${quotation.discount.toLocaleString()}` : ''}
Total: R ${quotation.totalAmount.toLocaleString()}
`
  alert(details)
}

const negotiateTerms = (quotation: any) => {
  alert(`Negotiation chat with ${quotation.supplier} will be implemented`)
}

const requestRevision = (quotation: any) => {
  alert(`Request revision sent to ${quotation.supplier}`)
}

const acceptQuotation = (quotation: any) => {
  if (confirm(`Accept quotation from ${quotation.supplier} for R ${quotation.totalAmount.toLocaleString()}?`)) {
    quotation.status = 'accepted'
    alert(`Quotation accepted! Purchase Order will be auto-generated.`)
  }
}

const rejectQuotation = (quotation: any) => {
  if (confirm(`Reject quotation from ${quotation.supplier}?`)) {
    quotation.status = 'rejected'
    alert('Quotation rejected. Supplier will be notified.')
  }
}

const downloadQuotation = (quotation: any) => {
  alert(`Downloading quotation ${quotation.quoteNumber} as PDF...`)
}

const exportQuotations = () => {
  alert('Export all quotations to CSV/Excel')
}
</script>

