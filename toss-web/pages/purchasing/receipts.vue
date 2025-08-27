<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Purchase Receipts</h1>
              <p class="text-gray-600 dark:text-gray-400">Record and track incoming deliveries from suppliers</p>
            </div>
            <div class="flex space-x-3">
              <button @click="openCreateReceiptModal" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center">
                <PlusIcon class="w-5 h-5 mr-2" />
                Record Receipt
              </button>
              <button @click="scanBarcode" class="bg-purple-600 text-white px-4 py-2 rounded-lg hover:bg-purple-700 transition-colors flex items-center">
                <QrCodeIcon class="w-5 h-5 mr-2" />
                Scan Barcode
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Receipt Stats -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="grid grid-cols-1 md:grid-cols-5 gap-6 mb-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-blue-100 dark:bg-blue-900/30">
              <ClipboardDocumentCheckIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total Receipts</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.totalReceipts }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-yellow-100 dark:bg-yellow-900/30">
              <ExclamationTriangleIcon class="w-6 h-6 text-yellow-600 dark:text-yellow-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Pending QC</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.pendingQC }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-green-100 dark:bg-green-900/30">
              <CheckCircleIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Accepted</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.acceptedReceipts }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-red-100 dark:bg-red-900/30">
              <XCircleIcon class="w-6 h-6 text-red-600 dark:text-red-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Rejected</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.rejectedReceipts }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-purple-100 dark:bg-purple-900/30">
              <CurrencyDollarIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total Value</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">${{ stats.totalValue }}</p>
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
                placeholder="Search receipt or PO number..."
                class="w-full pl-10 pr-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              />
            </div>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Status</label>
            <select v-model="selectedStatus" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Status</option>
              <option value="pending">Pending QC</option>
              <option value="accepted">Accepted</option>
              <option value="rejected">Rejected</option>
              <option value="partial">Partially Received</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Supplier</label>
            <select v-model="selectedSupplier" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Suppliers</option>
              <option value="Tech Solutions Inc">Tech Solutions Inc</option>
              <option value="Raw Materials Corp">Raw Materials Corp</option>
              <option value="Service Pro LLC">Service Pro LLC</option>
              <option value="Consumables Direct">Consumables Direct</option>
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
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Quality Status</label>
            <select v-model="selectedQualityStatus" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Quality Status</option>
              <option value="passed">Passed</option>
              <option value="failed">Failed</option>
              <option value="pending">Pending Review</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Purchase Receipts Table -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h2 class="text-lg font-semibold text-gray-900 dark:text-white">Purchase Receipts</h2>
        </div>
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Receipt #</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">PO Reference</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Supplier</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Items Received</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Received Date</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Quality Status</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Status</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
              <tr v-for="receipt in filteredReceipts" :key="receipt.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm font-medium text-gray-900 dark:text-white">{{ receipt.number }}</div>
                  <div class="text-sm text-gray-500 dark:text-gray-400">{{ receipt.receivedBy }}</div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm font-medium text-blue-600 dark:text-blue-400">{{ receipt.poNumber }}</div>
                  <div class="text-sm text-gray-500 dark:text-gray-400">{{ receipt.poDate }}</div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="flex-shrink-0 h-8 w-8">
                      <div class="h-8 w-8 rounded-full bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center">
                        <span class="text-xs font-medium text-white">{{ receipt.supplier.charAt(0) }}</span>
                      </div>
                    </div>
                    <div class="ml-3">
                      <div class="text-sm font-medium text-gray-900 dark:text-white">{{ receipt.supplier }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4">
                  <div class="text-sm text-gray-900 dark:text-white">{{ receipt.itemsReceived }}/{{ receipt.itemsExpected }} items</div>
                  <div class="w-full bg-gray-200 rounded-full h-2 dark:bg-gray-700 mt-1">
                    <div class="bg-blue-600 h-2 rounded-full" :style="{ width: (receipt.itemsReceived / receipt.itemsExpected * 100) + '%' }"></div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                  {{ formatDate(receipt.receivedAt) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getQualityStatusClass(receipt.qualityStatus)">
                    {{ receipt.qualityStatus }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getStatusClass(receipt.status)">
                    {{ receipt.status }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <div class="flex space-x-2">
                    <button @click="viewReceipt(receipt)" class="text-blue-600 hover:text-blue-900 dark:text-blue-400 dark:hover:text-blue-300">
                      <EyeIcon class="w-5 h-5" />
                    </button>
                    <button v-if="receipt.status === 'pending'" @click="approveReceipt(receipt)" class="text-green-600 hover:text-green-900 dark:text-green-400 dark:hover:text-green-300">
                      <CheckCircleIcon class="w-5 h-5" />
                    </button>
                    <button v-if="receipt.status === 'pending'" @click="rejectReceipt(receipt)" class="text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300">
                      <XCircleIcon class="w-5 h-5" />
                    </button>
                    <button @click="editReceipt(receipt)" class="text-indigo-600 hover:text-indigo-900 dark:text-indigo-400 dark:hover:text-indigo-300">
                      <PencilIcon class="w-5 h-5" />
                    </button>
                    <button @click="printReceipt(receipt)" class="text-purple-600 hover:text-purple-900 dark:text-purple-400 dark:hover:text-purple-300">
                      <PrinterIcon class="w-5 h-5" />
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Create Receipt Modal -->
    <div v-if="showCreateModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-10 mx-auto p-5 border w-11/12 md:w-4/5 lg:w-3/4 shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Record Purchase Receipt</h3>
            <button @click="closeCreateModal" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-200">
              <XMarkIcon class="w-6 h-6" />
            </button>
          </div>
          
          <form @submit.prevent="submitReceipt" class="space-y-6">
            <!-- Receipt Header -->
            <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Receipt Number</label>
                <input 
                  v-model="newReceipt.number"
                  type="text" 
                  readonly
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-gray-100 dark:bg-gray-600 text-gray-900 dark:text-white"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Purchase Order *</label>
                <select 
                  v-model="newReceipt.poNumber"
                  required
                  @change="loadPODetails"
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                >
                  <option value="">Select Purchase Order</option>
                  <option value="PO-2024-001">PO-2024-001 - Tech Solutions Inc</option>
                  <option value="PO-2024-002">PO-2024-002 - Raw Materials Corp</option>
                  <option value="PO-2024-003">PO-2024-003 - Service Pro LLC</option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Received Date *</label>
                <input 
                  v-model="newReceipt.receivedAt"
                  type="date" 
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                />
              </div>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Received By</label>
                <input 
                  v-model="newReceipt.receivedBy"
                  type="text" 
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  placeholder="Enter receiver name"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Delivery Note #</label>
                <input 
                  v-model="newReceipt.deliveryNote"
                  type="text" 
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  placeholder="Supplier delivery note number"
                />
              </div>
            </div>

            <!-- Items Received -->
            <div>
              <h4 class="text-md font-medium text-gray-900 dark:text-white mb-4">Items Received</h4>
              
              <div class="space-y-3">
                <div v-for="(item, index) in newReceipt.items" :key="index" class="grid grid-cols-1 md:grid-cols-6 gap-3 p-3 border border-gray-200 dark:border-gray-600 rounded-lg">
                  <div>
                    <input 
                      v-model="item.name"
                      type="text" 
                      placeholder="Item name"
                      required
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm"
                    />
                  </div>
                  <div>
                    <input 
                      v-model="item.expectedQty"
                      type="number" 
                      placeholder="Expected"
                      readonly
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-gray-100 dark:bg-gray-600 text-gray-900 dark:text-white text-sm"
                    />
                  </div>
                  <div>
                    <input 
                      v-model="item.receivedQty"
                      type="number" 
                      placeholder="Received"
                      min="0"
                      required
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm"
                    />
                  </div>
                  <div>
                    <select 
                      v-model="item.condition"
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm"
                    >
                      <option value="good">Good</option>
                      <option value="damaged">Damaged</option>
                      <option value="defective">Defective</option>
                    </select>
                  </div>
                  <div>
                    <input 
                      v-model="item.batchNumber"
                      type="text" 
                      placeholder="Batch/Serial #"
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm"
                    />
                  </div>
                  <div>
                    <input 
                      v-model="item.location"
                      type="text" 
                      placeholder="Storage Location"
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm"
                    />
                  </div>
                </div>
              </div>
            </div>

            <!-- Quality Check -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Quality Status</label>
                <select 
                  v-model="newReceipt.qualityStatus"
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                >
                  <option value="pending">Pending Review</option>
                  <option value="passed">Passed</option>
                  <option value="failed">Failed</option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Inspector</label>
                <input 
                  v-model="newReceipt.inspector"
                  type="text" 
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  placeholder="Quality inspector name"
                />
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Notes/Comments</label>
              <textarea 
                v-model="newReceipt.notes"
                rows="3"
                class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                placeholder="Any observations, damages, or special notes..."
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
                Record Receipt
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import {
  PlusIcon,
  QrCodeIcon,
  ClipboardDocumentCheckIcon,
  ExclamationTriangleIcon,
  CheckCircleIcon,
  XCircleIcon,
  CurrencyDollarIcon,
  MagnifyingGlassIcon,
  EyeIcon,
  PencilIcon,
  PrinterIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

// Page metadata
definePageMeta({
  title: 'Purchase Receipts - TOSS ERP',
  description: 'Record and manage purchase receipts and deliveries in TOSS ERP'
})

// Reactive data
const searchQuery = ref('')
const selectedStatus = ref('')
const selectedSupplier = ref('')
const selectedDateRange = ref('')
const selectedQualityStatus = ref('')
const showCreateModal = ref(false)

// Stats data
const stats = ref({
  totalReceipts: 245,
  pendingQC: 18,
  acceptedReceipts: 198,
  rejectedReceipts: 12,
  totalValue: '1.8M'
})

// New receipt form
const newReceipt = ref({
  number: '',
  poNumber: '',
  receivedAt: '',
  receivedBy: '',
  deliveryNote: '',
  qualityStatus: 'pending',
  inspector: '',
  notes: '',
  items: []
})

// Mock receipts data
const receipts = ref([
  {
    id: 1,
    number: 'RCP-2024-001',
    poNumber: 'PO-2024-001',
    poDate: '2024-01-15',
    supplier: 'Tech Solutions Inc',
    itemsReceived: 8,
    itemsExpected: 10,
    receivedAt: new Date('2024-01-20'),
    receivedBy: 'John Warehouse',
    qualityStatus: 'passed',
    status: 'accepted',
    inspector: 'Sarah QC'
  },
  {
    id: 2,
    number: 'RCP-2024-002',
    poNumber: 'PO-2024-002',
    poDate: '2024-01-14',
    supplier: 'Raw Materials Corp',
    itemsReceived: 100,
    itemsExpected: 100,
    receivedAt: new Date('2024-01-19'),
    receivedBy: 'Mike Receiver',
    qualityStatus: 'pending',
    status: 'pending',
    inspector: 'Quality Team'
  },
  {
    id: 3,
    number: 'RCP-2024-003',
    poNumber: 'PO-2024-004',
    poDate: '2024-01-12',
    supplier: 'Consumables Direct',
    itemsReceived: 1,
    itemsExpected: 1,
    receivedAt: new Date('2024-01-18'),
    receivedBy: 'Lisa Store',
    qualityStatus: 'passed',
    status: 'accepted',
    inspector: 'Auto-accepted'
  },
  {
    id: 4,
    number: 'RCP-2024-004',
    poNumber: 'PO-2024-005',
    poDate: '2024-01-11',
    supplier: 'Tech Solutions Inc',
    itemsReceived: 1,
    itemsExpected: 2,
    receivedAt: new Date('2024-01-17'),
    receivedBy: 'Tom Handler',
    qualityStatus: 'failed',
    status: 'partial',
    inspector: 'Quality Team'
  }
])

// Computed filtered receipts
const filteredReceipts = computed(() => {
  return receipts.value.filter(receipt => {
    const matchesSearch = !searchQuery.value || 
      receipt.number.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      receipt.poNumber.toLowerCase().includes(searchQuery.value.toLowerCase())
    
    const matchesStatus = !selectedStatus.value || receipt.status === selectedStatus.value
    const matchesSupplier = !selectedSupplier.value || receipt.supplier === selectedSupplier.value
    const matchesQualityStatus = !selectedQualityStatus.value || receipt.qualityStatus === selectedQualityStatus.value
    
    return matchesSearch && matchesStatus && matchesSupplier && matchesQualityStatus
  })
})

// Helper functions
const getStatusClass = (status: string) => {
  const classes = {
    pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    accepted: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    rejected: 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400',
    partial: 'bg-orange-100 text-orange-800 dark:bg-orange-900/30 dark:text-orange-400'
  }
  return classes[status as keyof typeof classes] || 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400'
}

const getQualityStatusClass = (status: string) => {
  const classes = {
    pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    passed: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    failed: 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400'
  }
  return classes[status as keyof typeof classes] || 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400'
}

const formatDate = (date: Date) => {
  return date.toLocaleDateString('en-US', { 
    year: 'numeric', 
    month: 'short', 
    day: 'numeric' 
  })
}

// Generate new receipt number
const generateReceiptNumber = () => {
  const year = new Date().getFullYear()
  const count = receipts.value.length + 1
  return `RCP-${year}-${count.toString().padStart(3, '0')}`
}

// Modal functions
const openCreateReceiptModal = () => {
  newReceipt.value.number = generateReceiptNumber()
  newReceipt.value.receivedAt = new Date().toISOString().split('T')[0]
  showCreateModal.value = true
}

const closeCreateModal = () => {
  showCreateModal.value = false
  newReceipt.value = {
    number: '',
    poNumber: '',
    receivedAt: '',
    receivedBy: '',
    deliveryNote: '',
    qualityStatus: 'pending',
    inspector: '',
    notes: '',
    items: []
  }
}

// Load PO details when selected
const loadPODetails = () => {
  // Mock loading PO items
  if (newReceipt.value.poNumber) {
    newReceipt.value.items = [
      { name: 'Sample Item', expectedQty: 10, receivedQty: 0, condition: 'good', batchNumber: '', location: '' }
    ]
  }
}

// Form submission
const submitReceipt = () => {
  const receipt = {
    id: receipts.value.length + 1,
    ...newReceipt.value,
    poDate: '2024-01-15', // This would come from PO
    supplier: 'Selected Supplier', // This would come from PO
    itemsReceived: newReceipt.value.items.reduce((sum, item) => sum + (item.receivedQty || 0), 0),
    itemsExpected: newReceipt.value.items.reduce((sum, item) => sum + (item.expectedQty || 0), 0),
    receivedAt: new Date(newReceipt.value.receivedAt),
    status: 'pending'
  }
  
  receipts.value.unshift(receipt)
  closeCreateModal()
  alert('Purchase receipt recorded successfully!')
}

// Action functions
const viewReceipt = (receipt: any) => {
  console.log('View receipt:', receipt)
}

const editReceipt = (receipt: any) => {
  console.log('Edit receipt:', receipt)
}

const approveReceipt = (receipt: any) => {
  receipt.status = 'accepted'
  alert(`Receipt ${receipt.number} approved!`)
}

const rejectReceipt = (receipt: any) => {
  receipt.status = 'rejected'
  alert(`Receipt ${receipt.number} rejected!`)
}

const printReceipt = (receipt: any) => {
  console.log('Print receipt:', receipt)
  alert('Print functionality will be implemented')
}

const scanBarcode = () => {
  alert('Barcode scanning will be implemented')
}

onMounted(() => {
  console.log('Purchase Receipts page mounted')
})
</script>
