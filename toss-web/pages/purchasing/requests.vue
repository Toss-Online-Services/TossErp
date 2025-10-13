<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Purchase Requests</h1>
              <p class="text-gray-600 dark:text-gray-400">Manage and track purchase requests across your organization</p>
            </div>
            <div class="flex space-x-3">
              <button @click="openCreateRequestModal" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center">
                <PlusIcon class="w-5 h-5 mr-2" />
                New Request
              </button>
              <button @click="openGroupBuyModal" class="bg-purple-600 text-white px-4 py-2 rounded-lg hover:bg-purple-700 transition-colors flex items-center">
                <UserGroupIcon class="w-5 h-5 mr-2" />
                Group Buy
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Request Stats -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="grid grid-cols-1 md:grid-cols-5 gap-6 mb-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-blue-100 dark:bg-blue-900/30">
              <DocumentTextIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total Requests</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.totalRequests }}</p>
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
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.pendingRequests }}</p>
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
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.approvedRequests }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-purple-100 dark:bg-purple-900/30">
              <UserGroupIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Group Buys</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.groupBuys }}</p>
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
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.rejectedRequests }}</p>
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
                placeholder="Search requests..."
                class="w-full pl-10 pr-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              />
            </div>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Status</label>
            <select v-model="selectedStatus" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Status</option>
              <option value="draft">Draft</option>
              <option value="pending">Pending</option>
              <option value="approved">Approved</option>
              <option value="rejected">Rejected</option>
              <option value="converted">Converted to PO</option>
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
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Department</label>
            <select v-model="selectedDepartment" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Departments</option>
              <option value="IT">IT</option>
              <option value="Operations">Operations</option>
              <option value="Marketing">Marketing</option>
              <option value="Finance">Finance</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Type</label>
            <select v-model="selectedType" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Types</option>
              <option value="individual">Individual</option>
              <option value="group">Group Buy</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Purchase Requests Table -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h2 class="text-lg font-semibold text-gray-900 dark:text-white">Purchase Requests</h2>
        </div>
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Request #</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Requester</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Items</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Total Amount</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Priority</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Status</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Date</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
              <tr v-for="request in filteredRequests" :key="request.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="text-sm font-medium text-gray-900 dark:text-white">{{ request.number }}</div>
                    <span v-if="request.type === 'group'" class="ml-2 inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400">
                      GROUP
                    </span>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="flex-shrink-0 h-8 w-8">
                      <div class="h-8 w-8 rounded-full bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center">
                        <span class="text-xs font-medium text-white">{{ request.requester.charAt(0) }}</span>
                      </div>
                    </div>
                    <div class="ml-3">
                      <div class="text-sm font-medium text-gray-900 dark:text-white">{{ request.requester }}</div>
                      <div class="text-sm text-gray-500 dark:text-gray-400">{{ request.department }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4">
                  <div class="text-sm text-gray-900 dark:text-white">{{ request.items.length }} item(s)</div>
                  <div class="text-sm text-gray-500 dark:text-gray-400">{{ request.items[0]?.name }}{{ request.items.length > 1 ? ` +${request.items.length - 1} more` : '' }}</div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                  ${{ request.totalAmount.toLocaleString() }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getPriorityClass(request.priority)">
                    {{ request.priority }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getStatusClass(request.status)">
                    {{ request.status }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                  {{ formatDate(request.createdAt) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <div class="flex space-x-2">
                    <button @click="viewRequest(request)" class="text-blue-600 hover:text-blue-900 dark:text-blue-400 dark:hover:text-blue-300">
                      <EyeIcon class="w-5 h-5" />
                    </button>
                    <button v-if="request.status === 'pending'" @click="approveRequest(request)" class="text-green-600 hover:text-green-900 dark:text-green-400 dark:hover:text-green-300">
                      <CheckCircleIcon class="w-5 h-5" />
                    </button>
                    <button v-if="request.status === 'approved'" @click="convertToPO(request)" class="text-purple-600 hover:text-purple-900 dark:text-purple-400 dark:hover:text-purple-300">
                      <ArrowRightIcon class="w-5 h-5" />
                    </button>
                    <button @click="editRequest(request)" class="text-indigo-600 hover:text-indigo-900 dark:text-indigo-400 dark:hover:text-indigo-300">
                      <PencilIcon class="w-5 h-5" />
                    </button>
                    <button @click="deleteRequest(request)" class="text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300">
                      <TrashIcon class="w-5 h-5" />
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Create Request Modal -->
    <div v-if="showCreateModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-10 mx-auto p-5 border w-11/12 md:w-4/5 lg:w-3/4 shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Create Purchase Request</h3>
            <button @click="closeCreateModal" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-200">
              <XMarkIcon class="w-6 h-6" />
            </button>
          </div>
          
          <form @submit.prevent="submitRequest" class="space-y-6">
            <!-- Request Header -->
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
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Department</label>
                <select 
                  v-model="newRequest.department"
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                >
                  <option value="">Select Department</option>
                  <option value="IT">IT</option>
                  <option value="Operations">Operations</option>
                  <option value="Marketing">Marketing</option>
                  <option value="Finance">Finance</option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Priority</label>
                <select 
                  v-model="newRequest.priority"
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                >
                  <option value="low">Low</option>
                  <option value="medium">Medium</option>
                  <option value="high">High</option>
                  <option value="urgent">Urgent</option>
                </select>
              </div>
            </div>

            <!-- Request Items -->
            <div>
              <div class="flex items-center justify-between mb-4">
                <h4 class="text-md font-medium text-gray-900 dark:text-white">Request Items</h4>
                <button type="button" @click="addItem" class="bg-blue-600 text-white px-3 py-1 rounded text-sm hover:bg-blue-700">
                  Add Item
                </button>
              </div>
              
              <div class="space-y-3">
                <div v-for="(item, index) in newRequest.items" :key="index" class="grid grid-cols-1 md:grid-cols-6 gap-3 p-3 border border-gray-200 dark:border-gray-600 rounded-lg">
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
                      v-model="item.estimatedPrice"
                      type="number" 
                      step="0.01"
                      placeholder="Est. Price"
                      required
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm"
                    />
                  </div>
                  <div>
                    <input 
                      :value="(item.quantity || 0) * (item.estimatedPrice || 0)"
                      type="text" 
                      readonly
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-gray-100 dark:bg-gray-600 text-gray-900 dark:text-white text-sm"
                    />
                  </div>
                  <div>
                    <button type="button" @click="removeItem(index)" class="w-full text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300 text-sm">
                      Remove
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <!-- Request Notes -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Notes/Justification</label>
              <textarea 
                v-model="newRequest.notes"
                rows="3"
                class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                placeholder="Provide justification for this purchase request..."
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
                Create Request
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Group Buy Modal -->
    <div v-if="showGroupBuyModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-11/12 md:w-2/3 shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Create Group Buy Request</h3>
            <button @click="closeGroupBuyModal" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-200">
              <XMarkIcon class="w-6 h-6" />
            </button>
          </div>
          
          <div class="text-center py-8">
            <UserGroupIcon class="w-16 h-16 mx-auto text-purple-600 dark:text-purple-400 mb-4" />
            <h4 class="text-xl font-semibold text-gray-900 dark:text-white mb-2">Group Buying Feature</h4>
            <p class="text-gray-600 dark:text-gray-400 mb-6">
              Join forces with other organizations to get better pricing through collective purchasing power.
              This feature allows you to create or join group buying initiatives.
            </p>
            <div class="space-y-3">
              <button class="w-full bg-purple-600 text-white px-6 py-3 rounded-lg hover:bg-purple-700 transition-colors">
                Browse Available Group Buys
              </button>
              <button class="w-full bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 transition-colors">
                Create New Group Buy
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import {
  PlusIcon,
  UserGroupIcon,
  DocumentTextIcon,
  ClockIcon,
  CheckCircleIcon,
  XCircleIcon,
  MagnifyingGlassIcon,
  EyeIcon,
  PencilIcon,
  TrashIcon,
  ArrowRightIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Purchase Requests - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage purchase requests and group buying in TOSS ERP' }
  ]
})

// Reactive data
const searchQuery = ref('')
const selectedStatus = ref('')
const selectedPriority = ref('')
const selectedDepartment = ref('')
const selectedType = ref('')
const showCreateModal = ref(false)
const showGroupBuyModal = ref(false)

// Stats data
const stats = ref({
  totalRequests: 89,
  pendingRequests: 23,
  approvedRequests: 45,
  groupBuys: 8,
  rejectedRequests: 13
})

// New request form
const newRequest = ref({
  number: '',
  department: '',
  priority: 'medium',
  items: [{ name: '', description: '', quantity: 1, estimatedPrice: 0 }],
  notes: ''
})

// Mock requests data
const requests = ref([
  {
    id: 1,
    number: 'PR-2024-001',
    requester: 'John Smith',
    department: 'IT',
    items: [
      { name: 'Dell Laptop', description: 'High-performance laptop for development', quantity: 5, estimatedPrice: 1200 }
    ],
    totalAmount: 6000,
    priority: 'high',
    status: 'pending',
    type: 'individual',
    createdAt: new Date('2024-01-15')
  },
  {
    id: 2,
    number: 'PR-2024-002',
    requester: 'Sarah Johnson',
    department: 'Operations',
    items: [
      { name: 'Office Supplies', description: 'Monthly office supply order', quantity: 1, estimatedPrice: 850 }
    ],
    totalAmount: 850,
    priority: 'medium',
    status: 'approved',
    type: 'individual',
    createdAt: new Date('2024-01-14')
  },
  {
    id: 3,
    number: 'GB-2024-001',
    requester: 'Multiple Orgs',
    department: 'Procurement',
    items: [
      { name: 'Industrial Printers', description: 'Bulk order for better pricing', quantity: 50, estimatedPrice: 800 }
    ],
    totalAmount: 40000,
    priority: 'medium',
    status: 'pending',
    type: 'group',
    createdAt: new Date('2024-01-13')
  },
  {
    id: 4,
    number: 'PR-2024-003',
    requester: 'Mike Wilson',
    department: 'Marketing',
    items: [
      { name: 'Marketing Software', description: 'Annual license for marketing tools', quantity: 10, estimatedPrice: 299 }
    ],
    totalAmount: 2990,
    priority: 'low',
    status: 'rejected',
    type: 'individual',
    createdAt: new Date('2024-01-12')
  },
  {
    id: 5,
    number: 'PR-2024-004',
    requester: 'Lisa Brown',
    department: 'Finance',
    items: [
      { name: 'Accounting Software', description: 'Upgrade to enterprise version', quantity: 1, estimatedPrice: 5000 }
    ],
    totalAmount: 5000,
    priority: 'urgent',
    status: 'converted',
    type: 'individual',
    createdAt: new Date('2024-01-11')
  }
])

// Computed filtered requests
const filteredRequests = computed(() => {
  return requests.value.filter(request => {
    const matchesSearch = !searchQuery.value || 
      request.number.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      request.requester.toLowerCase().includes(searchQuery.value.toLowerCase())
    
    const matchesStatus = !selectedStatus.value || request.status === selectedStatus.value
    const matchesPriority = !selectedPriority.value || request.priority === selectedPriority.value
    const matchesDepartment = !selectedDepartment.value || request.department === selectedDepartment.value
    const matchesType = !selectedType.value || request.type === selectedType.value
    
    return matchesSearch && matchesStatus && matchesPriority && matchesDepartment && matchesType
  })
})

// Helper functions
const getPriorityClass = (priority: string) => {
  const classes = {
    urgent: 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400',
    high: 'bg-orange-100 text-orange-800 dark:bg-orange-900/30 dark:text-orange-400',
    medium: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    low: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400'
  }
  return classes[priority as keyof typeof classes] || 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400'
}

const getStatusClass = (status: string) => {
  const classes = {
    draft: 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400',
    pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    approved: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    rejected: 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400',
    converted: 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400'
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

// Generate new request number
const generateRequestNumber = () => {
  const year = new Date().getFullYear()
  const count = requests.value.length + 1
  return `PR-${year}-${count.toString().padStart(3, '0')}`
}

// Modal functions
const openCreateRequestModal = () => {
  newRequest.value.number = generateRequestNumber()
  showCreateModal.value = true
}

const closeCreateModal = () => {
  showCreateModal.value = false
  newRequest.value = {
    number: '',
    department: '',
    priority: 'medium',
    items: [{ name: '', description: '', quantity: 1, estimatedPrice: 0 }],
    notes: ''
  }
}

const openGroupBuyModal = () => {
  showGroupBuyModal.value = true
}

const closeGroupBuyModal = () => {
  showGroupBuyModal.value = false
}

// Item management
const addItem = () => {
  newRequest.value.items.push({ name: '', description: '', quantity: 1, estimatedPrice: 0 })
}

const removeItem = (index: number) => {
  if (newRequest.value.items.length > 1) {
    newRequest.value.items.splice(index, 1)
  }
}

// Form submission
const submitRequest = () => {
  const totalAmount = newRequest.value.items.reduce((sum, item) => 
    sum + (item.quantity * item.estimatedPrice), 0
  )
  
  const request = {
    id: requests.value.length + 1,
    ...newRequest.value,
    requester: 'Current User', // This would come from auth
    totalAmount,
    status: 'draft',
    type: 'individual',
    createdAt: new Date()
  }
  
  requests.value.unshift(request)
  closeCreateModal()
  alert('Purchase request created successfully!')
}

// Action functions
const viewRequest = (request: any) => {
  const details = `
Purchase Request: ${request.number}
Requester: ${request.requester}
Department: ${request.department}
Status: ${request.status}
Priority: ${request.priority}
Total Amount: $${request.totalAmount.toLocaleString()}
Created: ${formatDate(request.createdAt)}

Items:
${request.items.map((item: any) => `- ${item.name}: ${item.quantity} @ $${item.estimatedPrice}`).join('\n')}
`
  alert(details)
}

const editRequest = (request: any) => {
  console.log('Edit request:', request)
  alert('Edit functionality will open a pre-filled form')
}

const deleteRequest = (request: any) => {
  if (confirm(`Are you sure you want to delete ${request.number}?`)) {
    const index = requests.value.findIndex(r => r.id === request.id)
    if (index > -1) {
      requests.value.splice(index, 1)
      alert(`Request ${request.number} deleted successfully!`)
    }
  }
}

const approveRequest = (request: any) => {
  request.status = 'approved'
  alert(`Request ${request.number} approved! You can now convert it to a Purchase Order.`)
}

const convertToPO = (request: any) => {
  if (confirm(`Convert ${request.number} to Purchase Order?`)) {
    request.status = 'converted'
    alert(`Request ${request.number} converted to Purchase Order PO-2025-XXX`)
    // In real app, navigate to the new PO
  }
}

onMounted(() => {
  console.log('Purchase Requests page mounted')
})
</script>
