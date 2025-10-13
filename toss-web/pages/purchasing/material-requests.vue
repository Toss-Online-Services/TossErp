<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Material Requests</h1>
              <p class="text-gray-600 dark:text-gray-400">Department requisitions for materials and services</p>
            </div>
            <div class="flex space-x-3">
              <button @click="openCreateModal" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center">
                <PlusIcon class="w-5 h-5 mr-2" />
                New Material Request
              </button>
              <button @click="exportRequests" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center">
                <ArrowDownTrayIcon class="w-5 h-5 mr-2" />
                Export
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Material Request Stats -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="grid grid-cols-1 md:grid-cols-5 gap-6 mb-8">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-blue-100 dark:bg-blue-900/30">
              <ClipboardDocumentListIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total Requests</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.totalRequests }}</p>
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
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Pending Approval</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.pendingApproval }}</p>
              <p class="text-xs text-gray-500 dark:text-gray-500">{{ stats.urgent }} urgent</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-green-100 dark:bg-green-900/30">
              <CheckCircleIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Approved</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.approved }}</p>
              <p class="text-xs text-gray-500 dark:text-gray-500">{{ stats.inStock }} in stock</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-purple-100 dark:bg-purple-900/30">
              <ShoppingCartIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">To Purchase</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.toPurchase }}</p>
              <p class="text-xs text-gray-500 dark:text-gray-500">R {{ stats.estimatedValue }}K</p>
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
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.inTransit }}</p>
              <p class="text-xs text-gray-500 dark:text-gray-500">{{ stats.avgLeadTime }} days avg</p>
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
                placeholder="Search material requests..."
                class="w-full pl-10 pr-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              />
            </div>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Status</label>
            <select v-model="selectedStatus" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Status</option>
              <option value="draft">Draft</option>
              <option value="pending">Pending Approval</option>
              <option value="approved">Approved</option>
              <option value="ordered">Ordered</option>
              <option value="received">Received</option>
              <option value="cancelled">Cancelled</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Department</label>
            <select v-model="selectedDepartment" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Departments</option>
              <option value="Production">Production</option>
              <option value="Maintenance">Maintenance</option>
              <option value="IT">IT</option>
              <option value="Admin">Admin</option>
              <option value="Sales">Sales</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Material Type</label>
            <select v-model="selectedType" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Types</option>
              <option value="raw-material">Raw Material</option>
              <option value="consumable">Consumable</option>
              <option value="spare-part">Spare Part</option>
              <option value="service">Service</option>
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

      <!-- Material Requests Table -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700 flex items-center justify-between">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Recent Material Requests</h3>
          <div class="flex items-center space-x-2">
            <button @click="bulkApprove" class="text-sm px-3 py-1.5 bg-green-600 text-white rounded-lg hover:bg-green-700">
              Bulk Approve
            </button>
            <button @click="bulkConvert" class="text-sm px-3 py-1.5 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
              Convert to PR
            </button>
          </div>
        </div>
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th class="px-6 py-3 text-left">
                  <input type="checkbox" v-model="selectAll" @change="toggleSelectAll" class="rounded border-gray-300 text-blue-600 focus:ring-blue-500" />
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Request #</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Department</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Items</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Required By</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Stock Status</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Status</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
              <tr v-for="request in filteredRequests" :key="request.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
                <td class="px-6 py-4">
                  <input type="checkbox" v-model="selectedRequests" :value="request.id" class="rounded border-gray-300 text-blue-600 focus:ring-blue-500" />
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm font-medium text-gray-900 dark:text-white">{{ request.number }}</div>
                  <div class="text-xs text-gray-500 dark:text-gray-400">{{ request.requester }}</div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm text-gray-900 dark:text-white">{{ request.department }}</div>
                  <div class="text-xs text-gray-500 dark:text-gray-400">{{ request.project || 'General' }}</div>
                </td>
                <td class="px-6 py-4">
                  <div class="text-sm text-gray-900 dark:text-white">{{ request.items.length }} item(s)</div>
                  <div class="text-xs text-gray-500 dark:text-gray-400">{{ request.items[0]?.itemName }}{{ request.items.length > 1 ? ` +${request.items.length - 1}` : '' }}</div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm text-gray-900 dark:text-white">{{ formatDate(request.requiredBy) }}</div>
                  <div v-if="getDaysUntil(request.requiredBy) <= 3" class="text-xs text-red-500">
                    {{ getDaysUntil(request.requiredBy) }} days left
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getStockStatusClass(request.stockStatus)">
                    {{ request.stockStatus }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getStatusClass(request.status)">
                    {{ request.status }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <div class="flex space-x-2">
                    <button @click="viewRequest(request)" class="text-blue-600 hover:text-blue-900 dark:text-blue-400">
                      <EyeIcon class="w-5 h-5" />
                    </button>
                    <button v-if="request.status === 'pending'" @click="approveRequest(request)" class="text-green-600 hover:text-green-900 dark:text-green-400">
                      <CheckCircleIcon class="w-5 h-5" />
                    </button>
                    <button v-if="request.status === 'approved'" @click="convertToPurchaseRequest(request)" class="text-purple-600 hover:text-purple-900 dark:text-purple-400">
                      <ArrowRightIcon class="w-5 h-5" />
                    </button>
                    <button @click="editRequest(request)" class="text-indigo-600 hover:text-indigo-900 dark:text-indigo-400">
                      <PencilIcon class="w-5 h-5" />
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Create Material Request Modal -->
    <div v-if="showCreateModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-10 mx-auto p-5 border w-11/12 md:w-4/5 lg:w-3/4 shadow-lg rounded-md bg-white dark:bg-gray-800 max-h-[90vh] overflow-y-auto">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Create Material Request</h3>
            <button @click="closeCreateModal" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-200">
              <XMarkIcon class="w-6 h-6" />
            </button>
          </div>
          
          <form @submit.prevent="submitMaterialRequest" class="space-y-6">
            <!-- Basic Information -->
            <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Request Number</label>
                <input 
                  v-model="newRequest.number"
                  type="text" 
                  readonly
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-gray-100 dark:bg-gray-600 text-gray-900 dark:text-white"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Department *</label>
                <select 
                  v-model="newRequest.department"
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                >
                  <option value="">Select Department</option>
                  <option value="Production">Production</option>
                  <option value="Maintenance">Maintenance</option>
                  <option value="IT">IT</option>
                  <option value="Admin">Admin</option>
                  <option value="Sales">Sales</option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Required By *</label>
                <input 
                  v-model="newRequest.requiredBy"
                  type="date" 
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                />
              </div>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Project/Cost Center</label>
                <input 
                  v-model="newRequest.project"
                  type="text" 
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  placeholder="Optional: Project or cost center"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Material Request Type</label>
                <select 
                  v-model="newRequest.requestType"
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                >
                  <option value="purchase">Purchase</option>
                  <option value="transfer">Material Transfer</option>
                  <option value="issue">Material Issue</option>
                  <option value="manufacture">Manufacture</option>
                </select>
              </div>
            </div>

            <!-- Material Items -->
            <div class="border-t pt-4">
              <div class="flex items-center justify-between mb-4">
                <h4 class="text-md font-medium text-gray-900 dark:text-white">Material Items</h4>
                <button type="button" @click="addMaterialItem" class="bg-blue-600 text-white px-3 py-1.5 rounded text-sm hover:bg-blue-700 flex items-center">
                  <PlusIcon class="w-4 h-4 mr-1" />
                  Add Item
                </button>
              </div>
              
              <div class="space-y-3">
                <div v-for="(item, index) in newRequest.items" :key="index" class="grid grid-cols-1 md:grid-cols-7 gap-3 p-3 border border-gray-200 dark:border-gray-600 rounded-lg">
                  <div class="md:col-span-2">
                    <input 
                      v-model="item.itemName"
                      type="text" 
                      placeholder="Item name or code"
                      required
                      @blur="checkStockAvailability(item)"
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
                    <input 
                      v-model="item.stockAvailable"
                      type="number" 
                      placeholder="In Stock"
                      readonly
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-gray-100 dark:bg-gray-600 text-gray-900 dark:text-white text-sm"
                    />
                  </div>
                  <div>
                    <select 
                      v-model="item.materialType"
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm"
                    >
                      <option value="raw-material">Raw Material</option>
                      <option value="consumable">Consumable</option>
                      <option value="spare-part">Spare Part</option>
                      <option value="service">Service</option>
                    </select>
                  </div>
                  <div>
                    <button 
                      type="button"
                      @click="removeMaterialItem(index)"
                      class="w-full text-red-600 hover:text-red-900 dark:text-red-400 text-sm p-2"
                    >
                      <XMarkIcon class="w-5 h-5 mx-auto" />
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <!-- Additional Details -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Purpose/Justification</label>
              <textarea 
                v-model="newRequest.purpose"
                rows="3"
                required
                class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                placeholder="Explain why these materials are needed..."
              ></textarea>
            </div>

            <!-- Auto-actions -->
            <div class="border-t pt-4">
              <h4 class="text-md font-medium text-gray-900 dark:text-white mb-3">Automated Actions</h4>
              <div class="space-y-2">
                <div class="flex items-center">
                  <input 
                    v-model="newRequest.autoConvertToPR"
                    type="checkbox" 
                    class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                  />
                  <label class="ml-2 text-sm text-gray-700 dark:text-gray-300">Auto-convert to Purchase Request when approved</label>
                </div>
                <div class="flex items-center">
                  <input 
                    v-model="newRequest.checkStock"
                    type="checkbox" 
                    checked
                    class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                  />
                  <label class="ml-2 text-sm text-gray-700 dark:text-gray-300">Check stock availability before approval</label>
                </div>
                <div class="flex items-center">
                  <input 
                    v-model="newRequest.notifyOnApproval"
                    type="checkbox" 
                    checked
                    class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                  />
                  <label class="ml-2 text-sm text-gray-700 dark:text-gray-300">Notify requester on approval/rejection</label>
                </div>
              </div>
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
                Create Material Request
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
  ClipboardDocumentListIcon,
  ClockIcon,
  CheckCircleIcon,
  ShoppingCartIcon,
  TruckIcon,
  MagnifyingGlassIcon,
  EyeIcon,
  PencilIcon,
  ArrowRightIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Material Requests - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage material requisitions and stock requests in TOSS ERP' }
  ]
})

// Reactive data
const searchQuery = ref('')
const selectedStatus = ref('')
const selectedDepartment = ref('')
const selectedType = ref('')
const selectedDateRange = ref('')
const showCreateModal = ref(false)
const selectAll = ref(false)
const selectedRequests = ref<number[]>([])

// Stats
const stats = ref({
  totalRequests: 145,
  thisMonth: 28,
  pendingApproval: 15,
  urgent: 3,
  approved: 42,
  inStock: 18,
  toPurchase: 24,
  estimatedValue: 125,
  inTransit: 12,
  avgLeadTime: 7
})

// New material request form
const newRequest = ref({
  number: '',
  department: '',
  requiredBy: '',
  project: '',
  requestType: 'purchase',
  items: [
    { itemName: '', quantity: 1, uom: 'Nos', stockAvailable: 0, materialType: 'raw-material' }
  ],
  purpose: '',
  autoConvertToPR: true,
  checkStock: true,
  notifyOnApproval: true
})

// Mock material requests data
const materialRequests = ref([
  {
    id: 1,
    number: 'MR-2025-001',
    department: 'Production',
    requester: 'Thabo Mokoena',
    project: 'Widget Manufacturing',
    items: [
      { itemName: 'Steel Sheets (Grade A)', quantity: 50, uom: 'Kg', stockAvailable: 15 },
      { itemName: 'Aluminum Rods', quantity: 20, uom: 'Meter', stockAvailable: 0 }
    ],
    requiredBy: new Date('2025-01-20'),
    stockStatus: 'Partial',
    status: 'pending',
    createdAt: new Date('2025-01-13')
  },
  {
    id: 2,
    number: 'MR-2025-002',
    department: 'Maintenance',
    requester: 'Sipho Ndlovu',
    project: null,
    items: [
      { itemName: 'Hydraulic Oil', quantity: 100, uom: 'Litre', stockAvailable: 25 },
      { itemName: 'Air Filters', quantity: 10, uom: 'Nos', stockAvailable: 2 }
    ],
    requiredBy: new Date('2025-01-15'),
    stockStatus: 'Low Stock',
    status: 'approved',
    createdAt: new Date('2025-01-12')
  },
  {
    id: 3,
    number: 'MR-2025-003',
    department: 'IT',
    requester: 'Nomsa Khumalo',
    project: 'Office Upgrade',
    items: [
      { itemName: 'Cat6 Ethernet Cable', quantity: 200, uom: 'Meter', stockAvailable: 0 },
      { itemName: 'Network Switches', quantity: 5, uom: 'Nos', stockAvailable: 0 }
    ],
    requiredBy: new Date('2025-01-18'),
    stockStatus: 'Out of Stock',
    status: 'ordered',
    createdAt: new Date('2025-01-10')
  },
  {
    id: 4,
    number: 'MR-2025-004',
    department: 'Admin',
    requester: 'Lerato Dlamini',
    project: null,
    items: [
      { itemName: 'A4 Paper (Ream)', quantity: 20, uom: 'Box', stockAvailable: 30 }
    ],
    requiredBy: new Date('2025-01-25'),
    stockStatus: 'In Stock',
    status: 'received',
    createdAt: new Date('2025-01-09')
  }
])

// Computed filtered requests
const filteredRequests = computed(() => {
  return materialRequests.value.filter(request => {
    const matchesSearch = !searchQuery.value || 
      request.number.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      request.requester.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      request.items.some(item => item.itemName.toLowerCase().includes(searchQuery.value.toLowerCase()))
    
    const matchesStatus = !selectedStatus.value || request.status === selectedStatus.value
    const matchesDepartment = !selectedDepartment.value || request.department === selectedDepartment.value
    const matchesType = !selectedType.value || request.items.some(item => item.materialType === selectedType.value)
    
    return matchesSearch && matchesStatus && matchesDepartment && matchesType
  })
})

// Helper functions
const getStatusClass = (status: string) => {
  const classes = {
    draft: 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400',
    pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    approved: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    ordered: 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
    received: 'bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400',
    cancelled: 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400'
  }
  return classes[status as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const getStockStatusClass = (status: string) => {
  const classes = {
    'In Stock': 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    'Low Stock': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    'Out of Stock': 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400',
    'Partial': 'bg-orange-100 text-orange-800 dark:bg-orange-900/30 dark:text-orange-400'
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

const generateRequestNumber = () => {
  const year = new Date().getFullYear()
  const count = materialRequests.value.length + 1
  return `MR-${year}-${count.toString().padStart(3, '0')}`
}

// Modal functions
const openCreateModal = () => {
  newRequest.value.number = generateRequestNumber()
  const tomorrow = new Date()
  tomorrow.setDate(tomorrow.getDate() + 7)
  newRequest.value.requiredBy = tomorrow.toISOString().split('T')[0]
  showCreateModal.value = true
}

const closeCreateModal = () => {
  showCreateModal.value = false
  newRequest.value = {
    number: '',
    department: '',
    requiredBy: '',
    project: '',
    requestType: 'purchase',
    items: [
      { itemName: '', quantity: 1, uom: 'Nos', stockAvailable: 0, materialType: 'raw-material' }
    ],
    purpose: '',
    autoConvertToPR: true,
    checkStock: true,
    notifyOnApproval: true
  }
}

// Item management
const addMaterialItem = () => {
  newRequest.value.items.push({
    itemName: '',
    quantity: 1,
    uom: 'Nos',
    stockAvailable: 0,
    materialType: 'raw-material'
  })
}

const removeMaterialItem = (index: number) => {
  if (newRequest.value.items.length > 1) {
    newRequest.value.items.splice(index, 1)
  }
}

const checkStockAvailability = (item: any) => {
  // Mock stock check - in real app, this would query inventory
  const stockLevels: Record<string, number> = {
    'Steel Sheets (Grade A)': 15,
    'Aluminum Rods': 0,
    'Hydraulic Oil': 25,
    'Air Filters': 2,
    'Cat6 Ethernet Cable': 0,
    'Network Switches': 0,
    'A4 Paper (Ream)': 30
  }
  
  item.stockAvailable = stockLevels[item.itemName] || 0
}

// Form submission
const submitMaterialRequest = () => {
  const request = {
    id: materialRequests.value.length + 1,
    number: newRequest.value.number,
    department: newRequest.value.department,
    requester: 'Current User',
    project: newRequest.value.project,
    items: newRequest.value.items.map(item => ({
      ...item,
      materialType: item.materialType || 'raw-material'
    })),
    requiredBy: new Date(newRequest.value.requiredBy),
    stockStatus: determineStockStatus(newRequest.value.items),
    status: 'draft',
    createdAt: new Date()
  }
  
  materialRequests.value.unshift(request)
  closeCreateModal()
  alert('Material request created successfully!')
}

const determineStockStatus = (items: any[]) => {
  const allInStock = items.every(item => item.stockAvailable >= item.quantity)
  const noneInStock = items.every(item => item.stockAvailable === 0)
  const someInStock = items.some(item => item.stockAvailable > 0 && item.stockAvailable < item.quantity)
  
  if (allInStock) return 'In Stock'
  if (noneInStock) return 'Out of Stock'
  if (someInStock) return 'Partial'
  return 'Low Stock'
}

// Bulk actions
const toggleSelectAll = () => {
  if (selectAll.value) {
    selectedRequests.value = filteredRequests.value.map(r => r.id)
  } else {
    selectedRequests.value = []
  }
}

const bulkApprove = () => {
  if (selectedRequests.value.length === 0) {
    alert('Please select at least one request to approve')
    return
  }
  
  materialRequests.value.forEach(request => {
    if (selectedRequests.value.includes(request.id) && request.status === 'pending') {
      request.status = 'approved'
    }
  })
  
  alert(`${selectedRequests.value.length} material requests approved!`)
  selectedRequests.value = []
  selectAll.value = false
}

const bulkConvert = () => {
  if (selectedRequests.value.length === 0) {
    alert('Please select at least one approved request to convert')
    return
  }
  
  const approved = materialRequests.value.filter(r => 
    selectedRequests.value.includes(r.id) && r.status === 'approved'
  )
  
  if (approved.length === 0) {
    alert('No approved requests selected. Only approved requests can be converted.')
    return
  }
  
  alert(`Converting ${approved.length} material requests to purchase requests...`)
  selectedRequests.value = []
  selectAll.value = false
}

// Action functions
const viewRequest = (request: any) => {
  const details = `
Material Request: ${request.number}
Department: ${request.department}
Requester: ${request.requester}
${request.project ? `Project: ${request.project}` : ''}
Required By: ${formatDate(request.requiredBy)}
Stock Status: ${request.stockStatus}
Status: ${request.status}

Items:
${request.items.map((item: any) => `- ${item.itemName} (Qty: ${item.quantity} ${item.uom}, Stock: ${item.stockAvailable})`).join('\n')}
`
  alert(details)
}

const approveRequest = (request: any) => {
  request.status = 'approved'
  alert(`Material request ${request.number} approved!`)
  
  if (request.autoConvertToPR) {
    setTimeout(() => {
      if (confirm('Auto-convert this request to Purchase Request?')) {
        convertToPurchaseRequest(request)
      }
    }, 500)
  }
}

const convertToPurchaseRequest = (request: any) => {
  request.status = 'ordered'
  alert(`Material request ${request.number} converted to Purchase Request PR-2025-XXX`)
}

const editRequest = (request: any) => {
  console.log('Edit request:', request)
  alert('Edit functionality will open a pre-filled form')
}

const exportRequests = () => {
  alert('Export functionality will be implemented with the shared export utility')
}
</script>

