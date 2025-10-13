<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Supplier Management</h1>
              <p class="text-gray-600 dark:text-gray-400">Manage your supplier network and relationships</p>
            </div>
            <div class="flex space-x-3">
              <button @click="openAddSupplierModal" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center">
                <PlusIcon class="w-5 h-5 mr-2" />
                Add Supplier
              </button>
              <button @click="exportSuppliers" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center">
                <DocumentArrowDownIcon class="w-5 h-5 mr-2" />
                Export
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters and Search -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6 mb-6">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Search Suppliers</label>
            <div class="relative">
              <MagnifyingGlassIcon class="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
              <input 
                v-model="searchQuery"
                type="text" 
                placeholder="Search by name, email, or code..."
                class="w-full pl-10 pr-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              />
            </div>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Status</label>
            <select v-model="selectedStatus" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Status</option>
              <option value="active">Active</option>
              <option value="inactive">Inactive</option>
              <option value="pending">Pending Approval</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Category</label>
            <select v-model="selectedCategory" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Categories</option>
              <option value="materials">Raw Materials</option>
              <option value="equipment">Equipment</option>
              <option value="services">Services</option>
              <option value="consumables">Consumables</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Rating</label>
            <select v-model="selectedRating" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Ratings</option>
              <option value="5">5 Stars</option>
              <option value="4">4+ Stars</option>
              <option value="3">3+ Stars</option>
              <option value="2">2+ Stars</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Suppliers Stats Cards -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-blue-100 dark:bg-blue-900/30">
              <UserGroupIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total Suppliers</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.totalSuppliers }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-green-100 dark:bg-green-900/30">
              <CheckCircleIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Active Suppliers</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.activeSuppliers }}</p>
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
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.pendingSuppliers }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-purple-100 dark:bg-purple-900/30">
              <CurrencyDollarIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Avg. Order Value</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">${{ stats.avgOrderValue }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Suppliers Table -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h2 class="text-lg font-semibold text-gray-900 dark:text-white">Suppliers Directory</h2>
        </div>
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Supplier</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Contact Info</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Category</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Rating</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Orders</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Status</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
              <tr v-for="supplier in filteredSuppliers" :key="supplier.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="flex-shrink-0 h-10 w-10">
                      <div class="h-10 w-10 rounded-full bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center">
                        <span class="text-sm font-medium text-white">{{ supplier.name.charAt(0) }}</span>
                      </div>
                    </div>
                    <div class="ml-4">
                      <div class="text-sm font-medium text-gray-900 dark:text-white">{{ supplier.name }}</div>
                      <div class="text-sm text-gray-500 dark:text-gray-400">{{ supplier.code }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm text-gray-900 dark:text-white">{{ supplier.email }}</div>
                  <div class="text-sm text-gray-500 dark:text-gray-400">{{ supplier.phone }}</div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getCategoryClass(supplier.category)">
                    {{ supplier.category }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="flex text-yellow-400">
                      <StarIcon v-for="i in 5" :key="i" 
                                :class="i <= supplier.rating ? 'fill-current' : ''" 
                                class="w-4 h-4" />
                    </div>
                    <span class="ml-2 text-sm text-gray-600 dark:text-gray-400">({{ supplier.rating }})</span>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                  {{ supplier.totalOrders }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getStatusClass(supplier.status)">
                    {{ supplier.status }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <div class="flex space-x-2">
                    <button @click="viewSupplier(supplier)" class="text-blue-600 hover:text-blue-900 dark:text-blue-400 dark:hover:text-blue-300">
                      <EyeIcon class="w-5 h-5" />
                    </button>
                    <button @click="editSupplier(supplier)" class="text-indigo-600 hover:text-indigo-900 dark:text-indigo-400 dark:hover:text-indigo-300">
                      <PencilIcon class="w-5 h-5" />
                    </button>
                    <button @click="deleteSupplier(supplier)" class="text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300">
                      <TrashIcon class="w-5 h-5" />
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        
        <!-- Pagination -->
        <div class="bg-white dark:bg-gray-800 px-4 py-3 border-t border-gray-200 dark:border-gray-700 sm:px-6">
          <div class="flex items-center justify-between">
            <div class="flex-1 flex justify-between sm:hidden">
              <button class="relative inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600">
                Previous
              </button>
              <button class="ml-3 relative inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 text-sm font-medium rounded-md text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600">
                Next
              </button>
            </div>
            <div class="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
              <div>
                <p class="text-sm text-gray-700 dark:text-gray-300">
                  Showing <span class="font-medium">1</span> to <span class="font-medium">10</span> of <span class="font-medium">{{ suppliers.length }}</span> results
                </p>
              </div>
              <div>
                <nav class="relative z-0 inline-flex rounded-md shadow-sm -space-x-px" aria-label="Pagination">
                  <button class="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-sm font-medium text-gray-500 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-600">
                    <ChevronLeftIcon class="h-5 w-5" />
                  </button>
                  <button class="relative inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-600">
                    1
                  </button>
                  <button class="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-700 text-sm font-medium text-gray-500 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-600">
                    <ChevronRightIcon class="h-5 w-5" />
                  </button>
                </nav>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Add Supplier Modal -->
    <div v-if="showAddModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-11/12 md:w-3/4 lg:w-1/2 shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Add New Supplier</h3>
            <button @click="closeAddSupplierModal" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-200">
              <XMarkIcon class="w-6 h-6" />
            </button>
          </div>
          
          <form @submit.prevent="submitSupplier" class="space-y-4">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Supplier Name *</label>
                <input 
                  v-model="newSupplier.name"
                  type="text" 
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  placeholder="Enter supplier name"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Supplier Code *</label>
                <input 
                  v-model="newSupplier.code"
                  type="text" 
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  placeholder="SUP-001"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Email *</label>
                <input 
                  v-model="newSupplier.email"
                  type="email" 
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  placeholder="supplier@example.com"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Phone</label>
                <input 
                  v-model="newSupplier.phone"
                  type="tel" 
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  placeholder="+1 (555) 123-4567"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Category *</label>
                <select 
                  v-model="newSupplier.category"
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                >
                  <option value="">Select Category</option>
                  <option value="materials">Raw Materials</option>
                  <option value="equipment">Equipment</option>
                  <option value="services">Services</option>
                  <option value="consumables">Consumables</option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Status</label>
                <select 
                  v-model="newSupplier.status"
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                >
                  <option value="active">Active</option>
                  <option value="inactive">Inactive</option>
                  <option value="pending">Pending Approval</option>
                </select>
              </div>
            </div>
            
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Address</label>
              <textarea 
                v-model="newSupplier.address"
                rows="3"
                class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                placeholder="Enter supplier address"
              ></textarea>
            </div>
            
            <div class="flex items-center justify-end space-x-3 pt-4">
              <button 
                type="button"
                @click="closeAddSupplierModal"
                class="px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700"
              >
                Cancel
              </button>
              <button 
                type="submit"
                class="px-4 py-2 bg-blue-600 border border-transparent rounded-md text-sm font-medium text-white hover:bg-blue-700"
              >
                Add Supplier
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
  MagnifyingGlassIcon,
  UserGroupIcon,
  CheckCircleIcon,
  ClockIcon,
  CurrencyDollarIcon,
  EyeIcon,
  PencilIcon,
  TrashIcon,
  StarIcon,
  ChevronLeftIcon,
  ChevronRightIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Suppliers - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage suppliers and vendor relationships in TOSS ERP' }
  ]
})

// Reactive data
const searchQuery = ref('')
const selectedStatus = ref('')
const selectedCategory = ref('')
const selectedRating = ref('')
const showAddModal = ref(false)

// New supplier form
const newSupplier = ref({
  name: '',
  code: '',
  email: '',
  phone: '',
  category: '',
  status: 'active',
  address: ''
})

// Stats data
const stats = ref({
  totalSuppliers: 147,
  activeSuppliers: 134,
  pendingSuppliers: 8,
  avgOrderValue: '12,450'
})

// Mock suppliers data
const suppliers = ref([
  {
    id: 1,
    name: 'Tech Solutions Inc',
    code: 'SUP-001',
    email: 'contact@techsolutions.com',
    phone: '+1 (555) 123-4567',
    category: 'equipment',
    rating: 5,
    totalOrders: 45,
    status: 'active',
    address: '123 Tech Street, Silicon Valley, CA'
  },
  {
    id: 2,
    name: 'Raw Materials Corp',
    code: 'SUP-002',
    email: 'sales@rawmaterials.com',
    phone: '+1 (555) 234-5678',
    category: 'materials',
    rating: 4,
    totalOrders: 89,
    status: 'active',
    address: '456 Industrial Ave, Manufacturing District'
  },
  {
    id: 3,
    name: 'Service Pro LLC',
    code: 'SUP-003',
    email: 'info@servicepro.com',
    phone: '+1 (555) 345-6789',
    category: 'services',
    rating: 4,
    totalOrders: 23,
    status: 'pending',
    address: '789 Service Road, Business Park'
  },
  {
    id: 4,
    name: 'Consumables Direct',
    code: 'SUP-004',
    email: 'orders@consumablesdirect.com',
    phone: '+1 (555) 456-7890',
    category: 'consumables',
    rating: 3,
    totalOrders: 67,
    status: 'active',
    address: '321 Supply Chain Blvd, Logistics Center'
  },
  {
    id: 5,
    name: 'Quality Equipment Co',
    code: 'SUP-005',
    email: 'support@qualityequipment.com',
    phone: '+1 (555) 567-8901',
    category: 'equipment',
    rating: 5,
    totalOrders: 156,
    status: 'inactive',
    address: '654 Equipment Lane, Industrial Zone'
  }
])

// Computed filtered suppliers
const filteredSuppliers = computed(() => {
  return suppliers.value.filter(supplier => {
    const matchesSearch = !searchQuery.value || 
      supplier.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      supplier.email.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      supplier.code.toLowerCase().includes(searchQuery.value.toLowerCase())
    
    const matchesStatus = !selectedStatus.value || supplier.status === selectedStatus.value
    const matchesCategory = !selectedCategory.value || supplier.category === selectedCategory.value
    const matchesRating = !selectedRating.value || supplier.rating >= parseInt(selectedRating.value)
    
    return matchesSearch && matchesStatus && matchesCategory && matchesRating
  })
})

// Helper functions
const getCategoryClass = (category: string) => {
  const classes = {
    materials: 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
    equipment: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    services: 'bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400',
    consumables: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400'
  }
  return classes[category as keyof typeof classes] || 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400'
}

const getStatusClass = (status: string) => {
  const classes = {
    active: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    inactive: 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400',
    pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400'
  }
  return classes[status as keyof typeof classes] || 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400'
}

// Modal functions
const openAddSupplierModal = () => {
  showAddModal.value = true
}

const closeAddSupplierModal = () => {
  showAddModal.value = false
  newSupplier.value = {
    name: '',
    code: '',
    email: '',
    phone: '',
    category: '',
    status: 'active',
    address: ''
  }
}

const submitSupplier = () => {
  // Add new supplier to the list
  const supplier = {
    id: suppliers.value.length + 1,
    ...newSupplier.value,
    rating: 0,
    totalOrders: 0
  }
  suppliers.value.unshift(supplier)
  closeAddSupplierModal()
  
  // Show success message (you can replace with proper toast notification)
  alert('Supplier added successfully!')
}

// Action functions
const viewSupplier = (supplier: any) => {
  // Navigate to supplier detail page or open detail modal
  console.log('View supplier:', supplier)
}

const editSupplier = (supplier: any) => {
  // Open edit modal or navigate to edit page
  console.log('Edit supplier:', supplier)
}

const deleteSupplier = (supplier: any) => {
  if (confirm(`Are you sure you want to delete ${supplier.name}?`)) {
    const index = suppliers.value.findIndex(s => s.id === supplier.id)
    if (index > -1) {
      suppliers.value.splice(index, 1)
    }
  }
}

const exportSuppliers = async () => {
  const exportData = filteredSuppliers.value.map(supplier => ({
    'Supplier Code': supplier.code,
    'Supplier Name': supplier.name,
    'Email': supplier.email,
    'Phone': supplier.phone,
    'Category': supplier.category,
    'Rating': supplier.rating,
    'Total Orders': supplier.totalOrders,
    'Status': supplier.status,
    'Address': supplier.address
  }))

  try {
    const { exportData: exportDataFn } = await import('~/composables/useExport')
    await exportDataFn(exportData, 'suppliers', 'csv')
    alert('Suppliers exported successfully!')
  } catch (error) {
    console.error('Export failed:', error)
    alert('Failed to export suppliers. Please try again.')
  }
}

onMounted(() => {
  // Load suppliers data
  console.log('Suppliers page mounted')
})
</script>
