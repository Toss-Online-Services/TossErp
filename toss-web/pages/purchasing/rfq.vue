<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Request for Quotation (RFQ)</h1>
              <p class="text-gray-600 dark:text-gray-400">Solicit competitive quotes from multiple suppliers</p>
            </div>
            <div class="flex space-x-3">
              <button @click="openCreateModal" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center">
                <PlusIcon class="w-5 h-5 mr-2" />
                Create RFQ
              </button>
              <button @click="exportRFQs" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center">
                <ArrowDownTrayIcon class="w-5 h-5 mr-2" />
                Export
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- RFQ Stats -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="grid grid-cols-1 md:grid-cols-5 gap-6 mb-8">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-blue-100 dark:bg-blue-900/30">
              <DocumentTextIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total RFQs</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.totalRFQs }}</p>
              <p class="text-xs text-gray-500 dark:text-gray-500">{{ stats.thisMonth }} this month</p>
            </div>
          </div>
        </div>
        
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-yellow-100 dark:bg-yellow-900/30">
              <ClockIcon class="w-6 h-6 text-yellow-600 dark:text-yellow-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Open RFQs</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.openRFQs }}</p>
              <p class="text-xs text-gray-500 dark:text-gray-500">Awaiting quotes</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-green-100 dark:bg-green-900/30">
              <CheckCircleIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Quotes Received</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.quotesReceived }}</p>
              <p class="text-xs text-gray-500 dark:text-gray-500">{{ stats.responseRate }}% response</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-purple-100 dark:bg-purple-900/30">
              <CurrencyDollarIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Avg Savings</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.avgSavings }}%</p>
              <p class="text-xs text-gray-500 dark:text-gray-500">vs initial quote</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-orange-100 dark:bg-orange-900/30">
              <UserGroupIcon class="w-6 h-6 text-orange-600 dark:text-orange-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Active Suppliers</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.activeSuppliers }}</p>
              <p class="text-xs text-gray-500 dark:text-gray-500">in RFQ pool</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Filters -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6 mb-6">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Search</label>
            <div class="relative">
              <MagnifyingGlassIcon class="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
              <input 
                v-model="searchQuery"
                type="text" 
                placeholder="Search RFQs..."
                class="w-full pl-10 pr-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              />
            </div>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Status</label>
            <select v-model="selectedStatus" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Status</option>
              <option value="draft">Draft</option>
              <option value="sent">Sent to Suppliers</option>
              <option value="quotes-received">Quotes Received</option>
              <option value="awarded">Awarded</option>
              <option value="expired">Expired</option>
              <option value="cancelled">Cancelled</option>
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
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Deadline</label>
            <select v-model="selectedDeadline" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Deadlines</option>
              <option value="overdue">Overdue</option>
              <option value="today">Due Today</option>
              <option value="week">Due This Week</option>
            </select>
          </div>
        </div>
      </div>

      <!-- RFQs List -->
      <div class="space-y-6">
        <div v-for="rfq in filteredRFQs" :key="rfq.id" class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <!-- RFQ Header -->
          <div class="flex items-center justify-between mb-4">
            <div class="flex items-center space-x-4">
              <div>
                <h3 class="text-lg font-semibold text-gray-900 dark:text-white">{{ rfq.number }}</h3>
                <p class="text-sm text-gray-600 dark:text-gray-400">{{ rfq.title }}</p>
              </div>
              <span class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium"
                    :class="getStatusClass(rfq.status)">
                {{ rfq.status }}
              </span>
            </div>
            <div class="flex items-center space-x-3">
              <div class="text-right">
                <p class="text-sm text-gray-600 dark:text-gray-400">Deadline</p>
                <p class="text-sm font-medium text-gray-900 dark:text-white">{{ formatDate(rfq.deadline) }}</p>
                <p v-if="getDaysUntil(rfq.deadline) <= 2" class="text-xs text-red-500">
                  {{ getDaysUntil(rfq.deadline) }} days left
                </p>
              </div>
            </div>
          </div>

          <!-- RFQ Details -->
          <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-4">
            <div>
              <p class="text-xs text-gray-500 dark:text-gray-500 mb-1">Items</p>
              <p class="text-sm font-medium text-gray-900 dark:text-white">{{ rfq.items.length }} item(s)</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 dark:text-gray-500 mb-1">Suppliers Invited</p>
              <p class="text-sm font-medium text-gray-900 dark:text-white">{{ rfq.suppliersInvited }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 dark:text-gray-500 mb-1">Quotes Received</p>
              <p class="text-sm font-medium text-gray-900 dark:text-white">{{ rfq.quotesReceived }}/{{ rfq.suppliersInvited }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 dark:text-gray-500 mb-1">Best Quote</p>
              <p class="text-sm font-medium text-green-600">R {{ rfq.bestQuote?.toLocaleString() || 'N/A' }}</p>
            </div>
          </div>

          <!-- Suppliers -->
          <div class="mb-4">
            <p class="text-xs text-gray-500 dark:text-gray-500 mb-2">Invited Suppliers</p>
            <div class="flex flex-wrap gap-2">
              <span v-for="supplier in rfq.suppliers" :key="supplier.name" 
                    class="inline-flex items-center px-3 py-1 rounded-full text-xs font-medium"
                    :class="supplier.responded ? 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400' : 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400'">
                {{ supplier.name }}
                <CheckCircleIcon v-if="supplier.responded" class="w-3 h-3 ml-1" />
              </span>
            </div>
          </div>

          <!-- Actions -->
          <div class="flex items-center justify-between pt-4 border-t border-gray-200 dark:border-gray-700">
            <div class="flex space-x-2">
              <button @click="viewRFQ(rfq)" class="text-blue-600 hover:text-blue-800 text-sm font-medium flex items-center">
                <EyeIcon class="w-4 h-4 mr-1" />
                View Details
              </button>
              <button v-if="rfq.quotesReceived > 0" @click="compareQuotes(rfq)" class="text-purple-600 hover:text-purple-800 text-sm font-medium flex items-center">
                <ChartBarIcon class="w-4 h-4 mr-1" />
                Compare Quotes
              </button>
              <button v-if="rfq.status === 'quotes-received'" @click="awardRFQ(rfq)" class="text-green-600 hover:text-green-800 text-sm font-medium flex items-center">
                <TrophyIcon class="w-4 h-4 mr-1" />
                Award
              </button>
            </div>
            <div class="flex space-x-2">
              <button v-if="rfq.status === 'draft'" @click="sendRFQ(rfq)" class="px-3 py-1.5 bg-blue-600 text-white rounded-lg hover:bg-blue-700 text-sm">
                Send to Suppliers
              </button>
              <button @click="downloadRFQ(rfq)" class="p-1.5 text-gray-600 hover:text-gray-900 dark:text-gray-400">
                <ArrowDownTrayIcon class="w-5 h-5" />
              </button>
              <button @click="printRFQ(rfq)" class="p-1.5 text-gray-600 hover:text-gray-900 dark:text-gray-400">
                <PrinterIcon class="w-5 h-5" />
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create RFQ Modal -->
    <div v-if="showCreateModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-10 mx-auto p-5 border w-11/12 md:w-4/5 lg:w-3/4 shadow-lg rounded-md bg-white dark:bg-gray-800 max-h-[90vh] overflow-y-auto">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Create Request for Quotation</h3>
            <button @click="closeCreateModal" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-200">
              <XMarkIcon class="w-6 h-6" />
            </button>
          </div>
          
          <form @submit.prevent="submitRFQ" class="space-y-6">
            <!-- Basic Information -->
            <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">RFQ Number</label>
                <input 
                  v-model="newRFQ.number"
                  type="text" 
                  readonly
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-gray-100 dark:bg-gray-600 text-gray-900 dark:text-white"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">RFQ Title *</label>
                <input 
                  v-model="newRFQ.title"
                  type="text" 
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  placeholder="e.g., Office Equipment RFQ Q1 2025"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Deadline *</label>
                <input 
                  v-model="newRFQ.deadline"
                  type="date" 
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                />
              </div>
            </div>

            <!-- Supplier Selection -->
            <div class="border-t pt-4">
              <h4 class="text-md font-medium text-gray-900 dark:text-white mb-3">Select Suppliers</h4>
              <div class="grid grid-cols-1 md:grid-cols-3 gap-3 mb-3">
                <div v-for="supplier in availableSuppliers" :key="supplier.id" 
                     class="flex items-center p-3 border border-gray-200 dark:border-gray-600 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 cursor-pointer"
                     @click="toggleSupplier(supplier)">
                  <input 
                    type="checkbox" 
                    :checked="newRFQ.suppliers.some(s => s.id === supplier.id)"
                    class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                  />
                  <div class="ml-3 flex-1">
                    <p class="text-sm font-medium text-gray-900 dark:text-white">{{ supplier.name }}</p>
                    <div class="flex items-center">
                      <StarIcon v-for="i in 5" :key="i" class="w-3 h-3" :class="i <= supplier.rating ? 'text-yellow-400' : 'text-gray-300'" />
                      <span class="ml-1 text-xs text-gray-500">{{ supplier.responseRate }}% response</span>
                    </div>
                  </div>
                </div>
              </div>
              <p class="text-xs text-gray-500 dark:text-gray-500">Selected: {{ newRFQ.suppliers.length }} supplier(s)</p>
            </div>

            <!-- Items -->
            <div class="border-t pt-4">
              <div class="flex items-center justify-between mb-4">
                <h4 class="text-md font-medium text-gray-900 dark:text-white">RFQ Items</h4>
                <button type="button" @click="addRFQItem" class="bg-blue-600 text-white px-3 py-1.5 rounded text-sm hover:bg-blue-700 flex items-center">
                  <PlusIcon class="w-4 h-4 mr-1" />
                  Add Item
                </button>
              </div>
              
              <div class="space-y-3">
                <div v-for="(item, index) in newRFQ.items" :key="index" class="grid grid-cols-1 md:grid-cols-6 gap-3 p-3 border border-gray-200 dark:border-gray-600 rounded-lg">
                  <div class="md:col-span-2">
                    <input 
                      v-model="item.itemName"
                      type="text" 
                      placeholder="Item name or code"
                      required
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm"
                    />
                  </div>
                  <div>
                    <input 
                      v-model="item.quantity"
                      type="number" 
                      placeholder="Quantity"
                      min="1"
                      required
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm"
                    />
                  </div>
                  <div>
                    <select 
                      v-model="item.uom"
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm"
                    >
                      <option value="Nos">Nos</option>
                      <option value="Kg">Kg</option>
                      <option value="Litre">Litre</option>
                      <option value="Meter">Meter</option>
                      <option value="Box">Box</option>
                    </select>
                  </div>
                  <div>
                    <textarea 
                      v-model="item.specifications"
                      placeholder="Specifications"
                      rows="1"
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm"
                    ></textarea>
                  </div>
                  <div>
                    <button 
                      type="button"
                      @click="removeRFQItem(index)"
                      class="w-full text-red-600 hover:text-red-900 dark:text-red-400 text-sm p-2"
                    >
                      <XMarkIcon class="w-5 h-5 mx-auto" />
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <!-- Terms & Conditions -->
            <div class="border-t pt-4">
              <h4 class="text-md font-medium text-gray-900 dark:text-white mb-3">Terms & Conditions</h4>
              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Payment Terms</label>
                  <select 
                    v-model="newRFQ.paymentTerms"
                    class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  >
                    <option value="net30">Net 30</option>
                    <option value="net60">Net 60</option>
                    <option value="net90">Net 90</option>
                    <option value="cod">Cash on Delivery</option>
                  </select>
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Delivery Terms</label>
                  <select 
                    v-model="newRFQ.deliveryTerms"
                    class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  >
                    <option value="fob">FOB Destination</option>
                    <option value="cif">CIF</option>
                    <option value="exw">Ex Works</option>
                    <option value="ddp">Delivered Duty Paid</option>
                  </select>
                </div>
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Additional Notes</label>
              <textarea 
                v-model="newRFQ.notes"
                rows="3"
                class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                placeholder="Special requirements, delivery instructions, or other notes for suppliers..."
              ></textarea>
            </div>
            
            <div class="flex items-center justify-end space-x-3 pt-4">
              <button 
                type="button"
                @click="closeCreateModal"
                class="px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700"
              >
                Cancel
              </button>
              <button 
                type="submit"
                class="px-4 py-2 bg-blue-600 border border-transparent rounded-md text-sm font-medium text-white hover:bg-blue-700"
              >
                Create RFQ
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
  PlusIcon,
  ArrowDownTrayIcon,
  DocumentTextIcon,
  ClockIcon,
  CheckCircleIcon,
  CurrencyDollarIcon,
  UserGroupIcon,
  MagnifyingGlassIcon,
  EyeIcon,
  ChartBarIcon,
  TrophyIcon,
  PrinterIcon,
  XMarkIcon,
  StarIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Request for Quotation - TOSS ERP',
  meta: [
    { name: 'description', content: 'Create and manage RFQs with supplier comparison in TOSS ERP' }
  ]
})

// Reactive data
const searchQuery = ref('')
const selectedStatus = ref('')
const selectedDateRange = ref('')
const selectedDeadline = ref('')
const showCreateModal = ref(false)

// Stats
const stats = ref({
  totalRFQs: 67,
  thisMonth: 12,
  openRFQs: 8,
  quotesReceived: 45,
  responseRate: 87,
  avgSavings: 14.5,
  activeSuppliers: 23
})

// Available suppliers for RFQ
const availableSuppliers = ref([
  { id: 1, name: 'Tech Solutions Inc', rating: 5, responseRate: 95 },
  { id: 2, name: 'Raw Materials Corp', rating: 4, responseRate: 88 },
  { id: 3, name: 'Service Pro LLC', rating: 4, responseRate: 92 },
  { id: 4, name: 'Consumables Direct', rating: 3, responseRate: 78 },
  { id: 5, name: 'Quality Equipment Co', rating: 5, responseRate: 100 },
  { id: 6, name: 'Industrial Supplies SA', rating: 4, responseRate: 85 }
])

// New RFQ form
const newRFQ = ref({
  number: '',
  title: '',
  deadline: '',
  suppliers: [] as any[],
  items: [
    { itemName: '', quantity: 1, uom: 'Nos', specifications: '' }
  ],
  paymentTerms: 'net30',
  deliveryTerms: 'fob',
  notes: ''
})

// Mock RFQ data
const rfqs = ref([
  {
    id: 1,
    number: 'RFQ-2025-001',
    title: 'Office Equipment & Furniture',
    status: 'sent',
    deadline: new Date('2025-01-20'),
    items: [
      { itemName: 'Office Desks', quantity: 10 },
      { itemName: 'Ergonomic Chairs', quantity: 10 },
      { itemName: 'Filing Cabinets', quantity: 5 }
    ],
    suppliersInvited: 4,
    quotesReceived: 2,
    suppliers: [
      { name: 'Tech Solutions Inc', responded: true },
      { name: 'Quality Equipment Co', responded: true },
      { name: 'Industrial Supplies SA', responded: false },
      { name: 'Service Pro LLC', responded: false }
    ],
    bestQuote: 125000,
    createdAt: new Date('2025-01-10')
  },
  {
    id: 2,
    number: 'RFQ-2025-002',
    title: 'Raw Materials for Production',
    status: 'quotes-received',
    deadline: new Date('2025-01-18'),
    items: [
      { itemName: 'Steel Sheets', quantity: 500 },
      { itemName: 'Aluminum Rods', quantity: 200 }
    ],
    suppliersInvited: 3,
    quotesReceived: 3,
    suppliers: [
      { name: 'Raw Materials Corp', responded: true },
      { name: 'Industrial Supplies SA', responded: true },
      { name: 'Quality Equipment Co', responded: true }
    ],
    bestQuote: 89500,
    createdAt: new Date('2025-01-08')
  },
  {
    id: 3,
    number: 'RFQ-2025-003',
    title: 'IT Infrastructure Upgrade',
    status: 'draft',
    deadline: new Date('2025-01-25'),
    items: [
      { itemName: 'Servers', quantity: 3 },
      { itemName: 'Network Switches', quantity: 10 },
      { itemName: 'UPS Systems', quantity: 5 }
    ],
    suppliersInvited: 5,
    quotesReceived: 0,
    suppliers: [
      { name: 'Tech Solutions Inc', responded: false },
      { name: 'Quality Equipment Co', responded: false },
      { name: 'Industrial Supplies SA', responded: false },
      { name: 'Service Pro LLC', responded: false },
      { name: 'Consumables Direct', responded: false }
    ],
    bestQuote: null,
    createdAt: new Date('2025-01-13')
  }
])

// Computed filtered RFQs
const filteredRFQs = computed(() => {
  return rfqs.value.filter(rfq => {
    const matchesSearch = !searchQuery.value || 
      rfq.number.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      rfq.title.toLowerCase().includes(searchQuery.value.toLowerCase())
    
    const matchesStatus = !selectedStatus.value || rfq.status === selectedStatus.value
    
    return matchesSearch && matchesStatus
  })
})

// Helper functions
const getStatusClass = (status: string) => {
  const classes = {
    draft: 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400',
    sent: 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
    'quotes-received': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    awarded: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    expired: 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400',
    cancelled: 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400'
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

const generateRFQNumber = () => {
  const year = new Date().getFullYear()
  const count = rfqs.value.length + 1
  return `RFQ-${year}-${count.toString().padStart(3, '0')}`
}

// Modal functions
const openCreateModal = () => {
  newRFQ.value.number = generateRFQNumber()
  const nextWeek = new Date()
  nextWeek.setDate(nextWeek.getDate() + 7)
  newRFQ.value.deadline = nextWeek.toISOString().split('T')[0]
  showCreateModal.value = true
}

const closeCreateModal = () => {
  showCreateModal.value = false
  newRFQ.value = {
    number: '',
    title: '',
    deadline: '',
    suppliers: [],
    items: [
      { itemName: '', quantity: 1, uom: 'Nos', specifications: '' }
    ],
    paymentTerms: 'net30',
    deliveryTerms: 'fob',
    notes: ''
  }
}

// Supplier selection
const toggleSupplier = (supplier: any) => {
  const index = newRFQ.value.suppliers.findIndex(s => s.id === supplier.id)
  if (index > -1) {
    newRFQ.value.suppliers.splice(index, 1)
  } else {
    newRFQ.value.suppliers.push({ ...supplier, responded: false })
  }
}

// Item management
const addRFQItem = () => {
  newRFQ.value.items.push({
    itemName: '',
    quantity: 1,
    uom: 'Nos',
    specifications: ''
  })
}

const removeRFQItem = (index: number) => {
  if (newRFQ.value.items.length > 1) {
    newRFQ.value.items.splice(index, 1)
  }
}

// Form submission
const submitRFQ = () => {
  if (newRFQ.value.suppliers.length === 0) {
    alert('Please select at least one supplier')
    return
  }
  
  const rfq = {
    id: rfqs.value.length + 1,
    number: newRFQ.value.number,
    title: newRFQ.value.title,
    status: 'draft',
    deadline: new Date(newRFQ.value.deadline),
    items: newRFQ.value.items,
    suppliersInvited: newRFQ.value.suppliers.length,
    quotesReceived: 0,
    suppliers: newRFQ.value.suppliers,
    bestQuote: null,
    createdAt: new Date()
  }
  
  rfqs.value.unshift(rfq)
  closeCreateModal()
  alert('RFQ created successfully! You can now send it to suppliers.')
}

// Action functions
const viewRFQ = (rfq: any) => {
  const details = `
RFQ: ${rfq.number}
Title: ${rfq.title}
Status: ${rfq.status}
Deadline: ${formatDate(rfq.deadline)}
Suppliers Invited: ${rfq.suppliersInvited}
Quotes Received: ${rfq.quotesReceived}
${rfq.bestQuote ? `Best Quote: R ${rfq.bestQuote.toLocaleString()}` : ''}

Items:
${rfq.items.map((item: any) => `- ${item.itemName} (${item.quantity} ${item.uom || 'units'})`).join('\n')}
`
  alert(details)
}

const compareQuotes = (rfq: any) => {
  alert(`Quote comparison matrix for ${rfq.number} will show detailed pricing from all ${rfq.quotesReceived} suppliers`)
}

const awardRFQ = (rfq: any) => {
  if (confirm(`Award RFQ ${rfq.number} to the best supplier?`)) {
    rfq.status = 'awarded'
    alert('RFQ awarded! Purchase Order will be auto-generated.')
  }
}

const sendRFQ = (rfq: any) => {
  rfq.status = 'sent'
  alert(`RFQ ${rfq.number} sent to ${rfq.suppliersInvited} suppliers via email`)
}

const downloadRFQ = (rfq: any) => {
  alert(`Downloading RFQ ${rfq.number} as PDF...`)
}

const printRFQ = (rfq: any) => {
  alert(`Printing RFQ ${rfq.number}...`)
}

const exportRFQs = () => {
  alert('Export all RFQs to CSV/Excel')
}
</script>

