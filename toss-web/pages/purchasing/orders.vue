<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Purchase Orders</h1>
              <p class="text-gray-600 dark:text-gray-400">Manage and track purchase orders with suppliers</p>
            </div>
            <div class="flex space-x-3">
              <button @click="openCreatePOModal" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center">
                <PlusIcon class="w-5 h-5 mr-2" />
                Create PO
              </button>
              <button @click="exportPOs" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center">
                <DocumentArrowDownIcon class="w-5 h-5 mr-2" />
                Export
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- PO Stats -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="grid grid-cols-1 md:grid-cols-5 gap-6 mb-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-blue-100 dark:bg-blue-900/30">
              <DocumentTextIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total POs</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.totalPOs }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-yellow-100 dark:bg-yellow-900/30">
              <ClockIcon class="w-6 h-6 text-yellow-600 dark:text-yellow-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Pending</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.pendingPOs }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-green-100 dark:bg-green-900/30">
              <CheckCircleIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Delivered</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.deliveredPOs }}</p>
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
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-orange-100 dark:bg-orange-900/30">
              <TruckIcon class="w-6 h-6 text-orange-600 dark:text-orange-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">In Transit</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.inTransitPOs }}</p>
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
                placeholder="Search PO number or supplier..."
                class="w-full pl-10 pr-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              />
            </div>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Status</label>
            <select v-model="selectedStatus" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Status</option>
              <option value="draft">Draft</option>
              <option value="sent">Sent to Supplier</option>
              <option value="confirmed">Confirmed</option>
              <option value="partial">Partially Received</option>
              <option value="delivered">Delivered</option>
              <option value="cancelled">Cancelled</option>
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
              <option value="quarter">This Quarter</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Priority</label>
            <select v-model="selectedPriority" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Priorities</option>
              <option value="urgent">Urgent</option>
              <option value="high">High</option>
              <option value="medium">Medium</option>
              <option value="low">Low</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Purchase Orders Table -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h2 class="text-lg font-semibold text-gray-900 dark:text-white">Purchase Orders</h2>
        </div>
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">PO Number</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Supplier</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Items</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Total Amount</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Expected Date</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Status</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Progress</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
              <tr v-for="po in filteredPOs" :key="po.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm font-medium text-gray-900 dark:text-white">{{ po.number }}</div>
                  <div class="text-sm text-gray-500 dark:text-gray-400">{{ formatDate(po.createdAt) }}</div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="flex-shrink-0 h-8 w-8">
                      <div class="h-8 w-8 rounded-full bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center">
                        <span class="text-xs font-medium text-white">{{ po.supplier.charAt(0) }}</span>
                      </div>
                    </div>
                    <div class="ml-3">
                      <div class="text-sm font-medium text-gray-900 dark:text-white">{{ po.supplier }}</div>
                      <div class="text-sm text-gray-500 dark:text-gray-400">{{ po.supplierCode }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4">
                  <div class="text-sm text-gray-900 dark:text-white">{{ po.items.length }} item(s)</div>
                  <div class="text-sm text-gray-500 dark:text-gray-400">{{ po.items[0]?.name }}{{ po.items.length > 1 ? ` +${po.items.length - 1} more` : '' }}</div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                  ${{ po.totalAmount.toLocaleString() }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                  {{ formatDate(po.expectedDelivery) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getStatusClass(po.status)">
                    {{ po.status }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="w-full bg-gray-200 rounded-full h-2 dark:bg-gray-700">
                      <div class="bg-blue-600 h-2 rounded-full" :style="{ width: po.progress + '%' }"></div>
                    </div>
                    <span class="ml-2 text-sm text-gray-600 dark:text-gray-400">{{ po.progress }}%</span>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <div class="flex space-x-2">
                    <button @click="viewPO(po)" class="text-blue-600 hover:text-blue-900 dark:text-blue-400 dark:hover:text-blue-300">
                      <EyeIcon class="w-5 h-5" />
                    </button>
                    <button v-if="po.status === 'draft'" @click="sendPO(po)" class="text-green-600 hover:text-green-900 dark:text-green-400 dark:hover:text-green-300">
                      <PaperAirplaneIcon class="w-5 h-5" />
                    </button>
                    <button @click="editPO(po)" class="text-indigo-600 hover:text-indigo-900 dark:text-indigo-400 dark:hover:text-indigo-300">
                      <PencilIcon class="w-5 h-5" />
                    </button>
                    <button @click="downloadPO(po)" class="text-purple-600 hover:text-purple-900 dark:text-purple-400 dark:hover:text-purple-300">
                      <DocumentArrowDownIcon class="w-5 h-5" />
                    </button>
                    <button v-if="po.status !== 'cancelled'" @click="cancelPO(po)" class="text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300">
                      <XCircleIcon class="w-5 h-5" />
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Create PO Modal -->
    <div v-if="showCreateModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-10 mx-auto p-5 border w-11/12 md:w-4/5 lg:w-3/4 shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Create Purchase Order</h3>
            <button @click="closeCreateModal" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-200">
              <XMarkIcon class="w-6 h-6" />
            </button>
          </div>
          
          <form @submit.prevent="submitPO" class="space-y-6">
            <!-- PO Header -->
            <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">PO Number</label>
                <input 
                  v-model="newPO.number"
                  type="text" 
                  readonly
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-gray-100 dark:bg-gray-600 text-gray-900 dark:text-white"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Supplier *</label>
                <select 
                  v-model="newPO.supplier"
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                >
                  <option value="">Select Supplier</option>
                  <option value="Tech Solutions Inc">Tech Solutions Inc</option>
                  <option value="Raw Materials Corp">Raw Materials Corp</option>
                  <option value="Service Pro LLC">Service Pro LLC</option>
                  <option value="Consumables Direct">Consumables Direct</option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Expected Delivery</label>
                <input 
                  v-model="newPO.expectedDelivery"
                  type="date" 
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                />
              </div>
            </div>

            <!-- PO Items -->
            <div>
              <div class="flex items-center justify-between mb-4">
                <h4 class="text-md font-medium text-gray-900 dark:text-white">Order Items</h4>
                <button type="button" @click="addPOItem" class="bg-blue-600 text-white px-3 py-1 rounded text-sm hover:bg-blue-700">
                  Add Item
                </button>
              </div>
              
              <div class="space-y-3">
                <div v-for="(item, index) in newPO.items" :key="index" class="grid grid-cols-1 md:grid-cols-6 gap-3 p-3 border border-gray-200 dark:border-gray-600 rounded-lg">
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
                      v-model="item.description"
                      type="text" 
                      placeholder="Description"
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm"
                    />
                  </div>
                  <div>
                    <input 
                      v-model="item.quantity"
                      type="number" 
                      placeholder="Qty"
                      min="1"
                      required
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm"
                    />
                  </div>
                  <div>
                    <input 
                      v-model="item.unitPrice"
                      type="number" 
                      step="0.01"
                      placeholder="Unit Price"
                      required
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm"
                    />
                  </div>
                  <div>
                    <input 
                      :value="(item.quantity || 0) * (item.unitPrice || 0)"
                      type="text" 
                      readonly
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-gray-100 dark:bg-gray-600 text-gray-900 dark:text-white text-sm"
                    />
                  </div>
                  <div>
                    <button type="button" @click="removePOItem(index)" class="w-full text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300 text-sm">
                      Remove
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <!-- Terms and Notes -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Payment Terms</label>
                <select 
                  v-model="newPO.paymentTerms"
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                >
                  <option value="net30">Net 30</option>
                  <option value="net60">Net 60</option>
                  <option value="net90">Net 90</option>
                  <option value="cod">Cash on Delivery</option>
                  <option value="advance">Advance Payment</option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Delivery Terms</label>
                <select 
                  v-model="newPO.deliveryTerms"
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                >
                  <option value="fob">FOB Destination</option>
                  <option value="cif">CIF</option>
                  <option value="exw">Ex Works</option>
                  <option value="ddp">Delivered Duty Paid</option>
                </select>
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Notes/Special Instructions</label>
              <textarea 
                v-model="newPO.notes"
                rows="3"
                class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                placeholder="Special delivery instructions, terms, or notes..."
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
                Create Purchase Order
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
  DocumentArrowDownIcon,
  DocumentTextIcon,
  ClockIcon,
  CheckCircleIcon,
  CurrencyDollarIcon,
  TruckIcon,
  MagnifyingGlassIcon,
  EyeIcon,
  PencilIcon,
  PaperAirplaneIcon,
  XCircleIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

// Page metadata
definePageMeta({
  title: 'Purchase Orders - TOSS ERP',
  description: 'Manage purchase orders and supplier deliveries in TOSS ERP'
})

// Reactive data
const searchQuery = ref('')
const selectedStatus = ref('')
const selectedSupplier = ref('')
const selectedDateRange = ref('')
const selectedPriority = ref('')
const showCreateModal = ref(false)

// Stats data
const stats = ref({
  totalPOs: 167,
  pendingPOs: 34,
  deliveredPOs: 98,
  totalValue: '2.4M',
  inTransitPOs: 23
})

// New PO form
const newPO = ref({
  number: '',
  supplier: '',
  expectedDelivery: '',
  items: [{ name: '', description: '', quantity: 1, unitPrice: 0 }],
  paymentTerms: 'net30',
  deliveryTerms: 'fob',
  notes: ''
})

// Mock PO data
const purchaseOrders = ref([
  {
    id: 1,
    number: 'PO-2024-001',
    supplier: 'Tech Solutions Inc',
    supplierCode: 'SUP-001',
    items: [
      { name: 'Dell Laptops', description: 'High-performance laptops', quantity: 10, unitPrice: 1200 }
    ],
    totalAmount: 12000,
    expectedDelivery: new Date('2024-02-15'),
    status: 'confirmed',
    progress: 75,
    paymentTerms: 'net30',
    deliveryTerms: 'fob',
    createdAt: new Date('2024-01-15')
  },
  {
    id: 2,
    number: 'PO-2024-002',
    supplier: 'Raw Materials Corp',
    supplierCode: 'SUP-002',
    items: [
      { name: 'Steel Sheets', description: 'Grade A steel sheets', quantity: 100, unitPrice: 50 }
    ],
    totalAmount: 5000,
    expectedDelivery: new Date('2024-02-20'),
    status: 'sent',
    progress: 25,
    paymentTerms: 'net60',
    deliveryTerms: 'cif',
    createdAt: new Date('2024-01-14')
  },
  {
    id: 3,
    number: 'PO-2024-003',
    supplier: 'Service Pro LLC',
    supplierCode: 'SUP-003',
    items: [
      { name: 'Maintenance Service', description: 'Annual maintenance contract', quantity: 1, unitPrice: 15000 }
    ],
    totalAmount: 15000,
    expectedDelivery: new Date('2024-03-01'),
    status: 'draft',
    progress: 0,
    paymentTerms: 'advance',
    deliveryTerms: 'ddp',
    createdAt: new Date('2024-01-13')
  },
  {
    id: 4,
    number: 'PO-2024-004',
    supplier: 'Consumables Direct',
    supplierCode: 'SUP-004',
    items: [
      { name: 'Office Supplies', description: 'Monthly office supply package', quantity: 1, unitPrice: 850 }
    ],
    totalAmount: 850,
    expectedDelivery: new Date('2024-02-10'),
    status: 'delivered',
    progress: 100,
    paymentTerms: 'net30',
    deliveryTerms: 'fob',
    createdAt: new Date('2024-01-12')
  },
  {
    id: 5,
    number: 'PO-2024-005',
    supplier: 'Tech Solutions Inc',
    supplierCode: 'SUP-001',
    items: [
      { name: 'Server Hardware', description: 'High-capacity server equipment', quantity: 2, unitPrice: 8500 }
    ],
    totalAmount: 17000,
    expectedDelivery: new Date('2024-02-25'),
    status: 'partial',
    progress: 50,
    paymentTerms: 'net30',
    deliveryTerms: 'exw',
    createdAt: new Date('2024-01-11')
  }
])

// Computed filtered POs
const filteredPOs = computed(() => {
  return purchaseOrders.value.filter(po => {
    const matchesSearch = !searchQuery.value || 
      po.number.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      po.supplier.toLowerCase().includes(searchQuery.value.toLowerCase())
    
    const matchesStatus = !selectedStatus.value || po.status === selectedStatus.value
    const matchesSupplier = !selectedSupplier.value || po.supplier === selectedSupplier.value
    // Add date range and priority filtering logic here if needed
    
    return matchesSearch && matchesStatus && matchesSupplier
  })
})

// Helper functions
const getStatusClass = (status: string) => {
  const classes = {
    draft: 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400',
    sent: 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
    confirmed: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    partial: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    delivered: 'bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400',
    cancelled: 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400'
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

// Generate new PO number
const generatePONumber = () => {
  const year = new Date().getFullYear()
  const count = purchaseOrders.value.length + 1
  return `PO-${year}-${count.toString().padStart(3, '0')}`
}

// Modal functions
const openCreatePOModal = () => {
  newPO.value.number = generatePONumber()
  showCreateModal.value = true
}

const closeCreateModal = () => {
  showCreateModal.value = false
  newPO.value = {
    number: '',
    supplier: '',
    expectedDelivery: '',
    items: [{ name: '', description: '', quantity: 1, unitPrice: 0 }],
    paymentTerms: 'net30',
    deliveryTerms: 'fob',
    notes: ''
  }
}

// Item management
const addPOItem = () => {
  newPO.value.items.push({ name: '', description: '', quantity: 1, unitPrice: 0 })
}

const removePOItem = (index: number) => {
  if (newPO.value.items.length > 1) {
    newPO.value.items.splice(index, 1)
  }
}

// Form submission
const submitPO = () => {
  const totalAmount = newPO.value.items.reduce((sum, item) => 
    sum + (item.quantity * item.unitPrice), 0
  )
  
  const po = {
    id: purchaseOrders.value.length + 1,
    ...newPO.value,
    supplierCode: 'SUP-XXX', // This would be fetched based on supplier
    totalAmount,
    expectedDelivery: new Date(newPO.value.expectedDelivery),
    status: 'draft',
    progress: 0,
    createdAt: new Date()
  }
  
  purchaseOrders.value.unshift(po)
  closeCreateModal()
  alert('Purchase Order created successfully!')
}

// Action functions
const viewPO = (po: any) => {
  console.log('View PO:', po)
}

const editPO = (po: any) => {
  console.log('Edit PO:', po)
}

const sendPO = (po: any) => {
  po.status = 'sent'
  po.progress = 25
  alert(`PO ${po.number} sent to supplier!`)
}

const downloadPO = (po: any) => {
  console.log('Download PO PDF:', po)
  alert('PO PDF download will be implemented')
}

const cancelPO = (po: any) => {
  if (confirm(`Are you sure you want to cancel ${po.number}?`)) {
    po.status = 'cancelled'
    po.progress = 0
  }
}

const exportPOs = () => {
  console.log('Export POs')
  alert('Export functionality will be implemented')
}

onMounted(() => {
  console.log('Purchase Orders page mounted')
})
</script>
