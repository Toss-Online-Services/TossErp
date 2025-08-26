<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Header -->
    <div class="bg-white shadow-sm border-b border-gray-200">
      <div class="px-6 py-4">
        <div class="flex items-center justify-between">
          <div>
            <h1 class="text-2xl font-bold text-gray-900">Customer Management</h1>
            <p class="text-sm text-gray-600 mt-1">Manage customer relationships, subscriptions, and revenue</p>
          </div>
          <div class="flex space-x-3">
            <button @click="exportCustomers" class="px-4 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 bg-white hover:bg-gray-50">
              <ArrowDownTrayIcon class="w-4 h-4 inline mr-2" />
              Export
            </button>
            <button @click="showCustomerModal = true; isEditMode = false; selectedCustomerForEdit = null" 
                    class="px-4 py-2 bg-blue-600 text-white rounded-md text-sm font-medium hover:bg-blue-700">
              <PlusIcon class="w-4 h-4 inline mr-2" />
              New Customer
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters and Search -->
    <div class="px-6 py-4 bg-white border-b border-gray-200">
      <div class="flex flex-wrap gap-4 items-center">
        <div class="flex-1 min-w-64">
          <div class="relative">
            <MagnifyingGlassIcon class="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search customers by name, email, or company..."
              class="pl-10 pr-4 py-2 w-full border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            />
          </div>
        </div>
        
        <div class="flex gap-3">
          <select v-model="selectedType" class="px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500">
            <option value="">All Types</option>
            <option value="Individual">Individual</option>
            <option value="Business">Business</option>
            <option value="Enterprise">Enterprise</option>
          </select>
          
          <select v-model="selectedStatus" class="px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500">
            <option value="">All Statuses</option>
            <option value="Prospect">Prospect</option>
            <option value="Active">Active</option>
            <option value="Inactive">Inactive</option>
            <option value="Churned">Churned</option>
          </select>
          
          <select v-model="selectedTier" class="px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500">
            <option value="">All Tiers</option>
            <option value="Standard">Standard</option>
            <option value="Premium">Premium</option>
            <option value="Enterprise">Enterprise</option>
          </select>
          
          <select v-model="selectedSubscriptionStatus" class="px-3 py-2 border border-gray-300 rounded-md focus:ring-2 focus:ring-blue-500">
            <option value="">All Subscriptions</option>
            <option value="Trial">Trial</option>
            <option value="Active">Active</option>
            <option value="Suspended">Suspended</option>
            <option value="Expired">Expired</option>
            <option value="Cancelled">Cancelled</option>
          </select>
          
          <button @click="clearFilters" class="px-3 py-2 text-gray-500 hover:text-gray-700">
            <XMarkIcon class="w-5 h-5" />
          </button>
        </div>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="px-6 py-4 bg-gray-50">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
        <div class="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div class="flex items-center">
            <div class="p-2 bg-blue-100 rounded-lg">
              <UsersIcon class="w-6 h-6 text-blue-600" />
            </div>
            <div class="ml-3">
              <p class="text-sm font-medium text-gray-500">Total Customers</p>
              <p class="text-2xl font-bold text-gray-900">{{ totalCustomers }}</p>
            </div>
          </div>
        </div>
        
        <div class="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div class="flex items-center">
            <div class="p-2 bg-green-100 rounded-lg">
              <BanknotesIcon class="w-6 h-6 text-green-600" />
            </div>
            <div class="ml-3">
              <p class="text-sm font-medium text-gray-500">Monthly Recurring Revenue</p>
              <p class="text-2xl font-bold text-gray-900">${{ totalMRR.toLocaleString() }}</p>
            </div>
          </div>
        </div>
        
        <div class="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div class="flex items-center">
            <div class="p-2 bg-yellow-100 rounded-lg">
              <ClockIcon class="w-6 h-6 text-yellow-600" />
            </div>
            <div class="ml-3">
              <p class="text-sm font-medium text-gray-500">Trial Customers</p>
              <p class="text-2xl font-bold text-gray-900">{{ trialCustomers }}</p>
            </div>
          </div>
        </div>
        
        <div class="bg-white p-4 rounded-lg shadow-sm border border-gray-200">
          <div class="flex items-center">
            <div class="p-2 bg-red-100 rounded-lg">
              <ExclamationTriangleIcon class="w-6 h-6 text-red-600" />
            </div>
            <div class="ml-3">
              <p class="text-sm font-medium text-gray-500">Expiring Soon</p>
              <p class="text-2xl font-bold text-gray-900">{{ expiringCustomers }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Customers Table -->
    <div class="px-6 py-4">
      <div class="bg-white rounded-lg shadow-sm border border-gray-200">
        <!-- Bulk Actions -->
        <div v-if="selectedCustomers.length > 0" class="px-6 py-3 bg-blue-50 border-b border-gray-200 flex items-center justify-between">
          <span class="text-sm text-blue-800">{{ selectedCustomers.length }} customers selected</span>
          <div class="space-x-2">
            <button @click="bulkUpdateStatus" class="px-3 py-1 bg-blue-600 text-white text-sm rounded hover:bg-blue-700">
              Update Status
            </button>
            <button @click="bulkAssign" class="px-3 py-1 bg-green-600 text-white text-sm rounded hover:bg-green-700">
              Assign
            </button>
          </div>
        </div>

        <!-- Table Header -->
        <div class="px-6 py-3 bg-gray-50 border-b border-gray-200">
          <div class="flex items-center">
            <input
              type="checkbox"
              v-model="selectAll"
              @change="toggleSelectAll"
              class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
            />
            <div class="ml-4 grid grid-cols-12 gap-4 text-xs font-medium text-gray-500 uppercase tracking-wider w-full">
              <div class="col-span-3 cursor-pointer flex items-center" @click="sortBy('name')">
                Customer
                <ChevronUpDownIcon class="w-4 h-4 ml-1" />
              </div>
              <div class="col-span-2 cursor-pointer flex items-center" @click="sortBy('type')">
                Type & Tier
                <ChevronUpDownIcon class="w-4 h-4 ml-1" />
              </div>
              <div class="col-span-2 cursor-pointer flex items-center" @click="sortBy('status')">
                Status
                <ChevronUpDownIcon class="w-4 h-4 ml-1" />
              </div>
              <div class="col-span-2 cursor-pointer flex items-center" @click="sortBy('subscriptionStatus')">
                Subscription
                <ChevronUpDownIcon class="w-4 h-4 ml-1" />
              </div>
              <div class="col-span-1 cursor-pointer flex items-center" @click="sortBy('monthlyRevenue')">
                MRR
                <ChevronUpDownIcon class="w-4 h-4 ml-1" />
              </div>
              <div class="col-span-1 cursor-pointer flex items-center" @click="sortBy('lastActivity')">
                Last Activity
                <ChevronUpDownIcon class="w-4 h-4 ml-1" />
              </div>
              <div class="col-span-1">Actions</div>
            </div>
          </div>
        </div>

        <!-- Table Body -->
        <div class="divide-y divide-gray-200">
          <div
            v-for="customer in paginatedCustomers"
            :key="customer.id"
            class="px-6 py-4 hover:bg-gray-50"
          >
            <div class="flex items-center">
              <input
                type="checkbox"
                :value="customer.id"
                v-model="selectedCustomers"
                class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
              />
              <div class="ml-4 grid grid-cols-12 gap-4 items-center w-full">
                <!-- Customer Info -->
                <div class="col-span-3">
                  <div class="flex items-center">
                    <div class="w-10 h-10 bg-blue-100 rounded-full flex items-center justify-center">
                      <span class="text-sm font-medium text-blue-800">{{ getInitials(customer.name) }}</span>
                    </div>
                    <div class="ml-3">
                      <p class="text-sm font-medium text-gray-900">{{ customer.name }}</p>
                      <p class="text-sm text-gray-500">{{ customer.primaryEmail || 'No email' }}</p>
                    </div>
                  </div>
                </div>

                <!-- Type & Tier -->
                <div class="col-span-2">
                  <div class="space-y-1">
                    <span :class="typeClass(customer.type)" class="inline-flex px-2 py-1 text-xs font-semibold rounded-full">
                      {{ customer.type }}
                    </span>
                    <div>
                      <span :class="tierClass(customer.tier)" class="inline-flex px-2 py-1 text-xs font-semibold rounded-full">
                        {{ customer.tier }}
                      </span>
                    </div>
                  </div>
                </div>

                <!-- Status -->
                <div class="col-span-2">
                  <span :class="statusClass(customer.status)" class="inline-flex px-2 py-1 text-xs font-semibold rounded-full">
                    {{ customer.status }}
                  </span>
                  <div class="text-xs text-gray-500 mt-1">
                    Score: {{ customer.customerScore }}/100
                  </div>
                </div>

                <!-- Subscription -->
                <div class="col-span-2">
                  <span :class="subscriptionStatusClass(customer.subscriptionStatus)" class="inline-flex px-2 py-1 text-xs font-semibold rounded-full">
                    {{ customer.subscriptionStatus }}
                  </span>
                  <div class="text-xs text-gray-500 mt-1">
                    <span v-if="customer.subscriptionEndDate">
                      Expires: {{ formatDate(customer.subscriptionEndDate) }}
                    </span>
                  </div>
                </div>

                <!-- MRR -->
                <div class="col-span-1">
                  <span class="text-sm font-medium text-gray-900">
                    ${{ customer.monthlyRecurringRevenue?.toLocaleString() || '0' }}
                  </span>
                </div>

                <!-- Last Activity -->
                <div class="col-span-1">
                  <span class="text-sm text-gray-500">
                    {{ formatDate(customer.lastActivityDate) }}
                  </span>
                </div>

                <!-- Actions -->
                <div class="col-span-1">
                  <div class="flex space-x-2">
                    <button @click="viewCustomer(customer)" class="text-blue-600 hover:text-blue-900">
                      <EyeIcon class="w-4 h-4" />
                    </button>
                    <button @click="editCustomer(customer)" class="text-gray-600 hover:text-gray-900">
                      <PencilIcon class="w-4 h-4" />
                    </button>
                    <Menu as="div" class="relative inline-block text-left">
                      <MenuButton class="text-gray-600 hover:text-gray-900">
                        <EllipsisVerticalIcon class="w-4 h-4" />
                      </MenuButton>
                      <MenuItems class="absolute right-0 z-10 mt-2 w-56 origin-top-right rounded-md bg-white shadow-lg ring-1 ring-black ring-opacity-5 focus:outline-none">
                        <div class="py-1">
                          <MenuItem>
                            <button @click="manageSubscription(customer)" class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 w-full text-left">
                              Manage Subscription
                            </button>
                          </MenuItem>
                          <MenuItem>
                            <button @click="viewActivities(customer)" class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 w-full text-left">
                              View Activities
                            </button>
                          </MenuItem>
                          <MenuItem>
                            <button @click="addNote(customer)" class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 w-full text-left">
                              Add Note
                            </button>
                          </MenuItem>
                          <MenuItem>
                            <button @click="deleteCustomer(customer)" class="block px-4 py-2 text-sm text-red-700 hover:bg-red-50 w-full text-left">
                              Delete Customer
                            </button>
                          </MenuItem>
                        </div>
                      </MenuItems>
                    </Menu>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Pagination -->
        <div class="px-6 py-3 bg-gray-50 border-t border-gray-200 flex items-center justify-between">
          <div class="flex items-center text-sm text-gray-700">
            <span>Showing {{ ((currentPage - 1) * pageSize) + 1 }} to {{ Math.min(currentPage * pageSize, totalCustomers) }} of {{ totalCustomers }} customers</span>
          </div>
          <div class="flex items-center space-x-2">
            <button
              @click="currentPage--"
              :disabled="currentPage === 1"
              class="px-3 py-1 border border-gray-300 rounded text-sm hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Previous
            </button>
            <span class="px-3 py-1 text-sm">{{ currentPage }} of {{ totalPages }}</span>
            <button
              @click="currentPage++"
              :disabled="currentPage === totalPages"
              class="px-3 py-1 border border-gray-300 rounded text-sm hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Next
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Customer Modal (placeholder for now) -->
    <div v-if="showCustomerModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white rounded-lg p-6 w-full max-w-2xl">
        <h3 class="text-lg font-medium mb-4">{{ isEditMode ? 'Edit Customer' : 'New Customer' }}</h3>
        <p class="text-gray-600">Customer modal implementation coming soon...</p>
        <div class="mt-4">
          <button @click="showCustomerModal = false" class="px-4 py-2 bg-gray-300 text-gray-700 rounded hover:bg-gray-400">
            Close
          </button>
        </div>
      </div>
    </div>

    <!-- Customer Details Modal -->
    <CustomerDetailsModal
      :is-open="showDetailsModal"
      :customer="transformCustomerForModal(selectedCustomer)"
      @close="showDetailsModal = false; selectedCustomer = null"
      @edit="handleCustomerEdit"
      @manage-subscription="handleSubscriptionManage"
    />

    <!-- Subscription Management Modal -->
    <SubscriptionModal
      :is-open="showSubscriptionModal"
      :customer="transformCustomerForModal(selectedCustomer)"
      @close="showSubscriptionModal = false; selectedCustomer = null"
      @subscription-updated="handleSubscriptionUpdate"
    />

    <!-- Customer Create/Edit Modal -->
    <CustomerModal
      :is-open="showCustomerModal"
      :customer="transformCustomerForModal(editingCustomer)"
      @close="showCustomerModal = false; editingCustomer = null"
      @save="handleCustomerSave"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import {
  PlusIcon,
  MagnifyingGlassIcon,
  EyeIcon,
  PencilIcon,
  EllipsisVerticalIcon,
  ChevronUpDownIcon,
  XMarkIcon,
  UsersIcon,
  BanknotesIcon,
  ClockIcon,
  ExclamationTriangleIcon,
  ArrowDownTrayIcon
} from '@heroicons/vue/24/outline'
import { Menu, MenuButton, MenuItems, MenuItem } from '@headlessui/vue'
// Transform customer data for modal compatibility
const transformCustomerForModal = (customer: any): any => {
  if (!customer) return null
  return {
    id: customer.id,
    companyName: customer.name,
    customerType: customer.type?.toLowerCase() === 'individual' ? 'individual' : 
                 customer.type?.toLowerCase() === 'business' ? 'business' : 'enterprise',
    email: customer.primaryEmail || customer.email || '',
    phone: customer.primaryPhone || customer.phone || '',
    address: customer.address || '',
    status: transformStatus(customer.status),
    tier: transformTier(customer.tier),
    notes: customer.notes || '',
    createdAt: customer.createdAt,
    updatedAt: customer.updatedAt || customer.createdAt
  } as any
}

const transformStatus = (status: string): 'active' | 'inactive' | 'suspended' => {
  const statusMap: { [key: string]: 'active' | 'inactive' | 'suspended' } = {
    'Active': 'active',
    'Inactive': 'inactive', 
    'Prospect': 'suspended',
    'Churned': 'inactive'
  }
  return statusMap[status] || 'active'
}

const transformTier = (tier: string): 'bronze' | 'silver' | 'gold' | 'platinum' => {
  const tierMap: { [key: string]: 'bronze' | 'silver' | 'gold' | 'platinum' } = {
    'Standard': 'bronze',
    'Premium': 'silver', 
    'Enterprise': 'gold'
  }
  return tierMap[tier] || 'bronze'
}

// Types
interface Customer {
  id: string
  tenantId: string
  customerNumber: string
  name: string
  type: 'Individual' | 'Business' | 'Enterprise'
  status: 'Prospect' | 'Active' | 'Inactive' | 'Churned'
  tier: 'Standard' | 'Premium' | 'Enterprise'
  source?: string
  primaryEmail?: string
  primaryPhone?: string
  billingAddress?: Address
  shippingAddress?: Address
  industry?: string
  employeeCount?: number
  annualRevenue?: number
  website?: string
  taxId?: string
  assignedTo?: string
  createdAt: string
  createdBy: string
  modifiedAt?: string
  modifiedBy?: string
  subscriptionStatus: 'Trial' | 'Active' | 'Suspended' | 'Expired' | 'Cancelled'
  subscriptionStartDate?: string
  subscriptionEndDate?: string
  monthlyRecurringRevenue?: number
  lastActivityDate?: string
  customerScore: number
  isSubscriptionActive: boolean
  isSubscriptionExpiring: boolean
}

interface Address {
  street: string
  city: string
  state: string
  postalCode: string
  country: string
}

// Reactive state
const searchQuery = ref('')
const selectedType = ref('')
const selectedStatus = ref('')
const selectedTier = ref('')
const selectedSubscriptionStatus = ref('')
const sortField = ref<string>('name')
const sortDirection = ref<'asc' | 'desc'>('asc')
const currentPage = ref(1)
const pageSize = ref(10)
const selectedCustomers = ref<string[]>([])
const selectAll = ref(false)
const showCustomerModal = ref(false)
const showDetailsModal = ref(false)
const showSubscriptionModal = ref(false)
const selectedCustomerForEdit = ref<Customer | null>(null)
const selectedCustomerForView = ref<Customer | null>(null)
const selectedCustomerForSubscription = ref<Customer | null>(null)
const isEditMode = ref(false)
const isLoading = ref(false)

// Additional modal state variables
const selectedCustomer = ref<Customer | null>(null)
const editingCustomer = ref<Customer | null>(null)

// Sample data for now - will be replaced with API calls
const allCustomers = ref<Customer[]>([
  {
    id: '1',
    tenantId: 'tenant1',
    customerNumber: 'CUST-001',
    name: 'TechCorp Solutions',
    type: 'Business',
    status: 'Active',
    tier: 'Premium',
    source: 'Website',
    primaryEmail: 'contact@techcorp.com',
    primaryPhone: '+1-555-0123',
    industry: 'Technology',
    employeeCount: 150,
    annualRevenue: 2500000,
    website: 'https://techcorp.com',
    assignedTo: 'John Smith',
    createdAt: '2025-01-15T10:00:00Z',
    createdBy: 'admin',
    subscriptionStatus: 'Active',
    subscriptionStartDate: '2025-01-15T10:00:00Z',
    subscriptionEndDate: '2026-01-15T10:00:00Z',
    monthlyRecurringRevenue: 5000,
    lastActivityDate: '2025-08-25T14:30:00Z',
    customerScore: 85,
    isSubscriptionActive: true,
    isSubscriptionExpiring: false
  },
  {
    id: '2',
    tenantId: 'tenant1',
    customerNumber: 'CUST-002',
    name: 'InnovateStartup',
    type: 'Business',
    status: 'Active',
    tier: 'Standard',
    source: 'Referral',
    primaryEmail: 'hello@innovatestartup.io',
    primaryPhone: '+1-555-0456',
    industry: 'SaaS',
    employeeCount: 25,
    annualRevenue: 500000,
    website: 'https://innovatestartup.io',
    assignedTo: 'Sarah Johnson',
    createdAt: '2025-02-20T15:00:00Z',
    createdBy: 'admin',
    subscriptionStatus: 'Trial',
    subscriptionStartDate: '2025-08-01T00:00:00Z',
    subscriptionEndDate: '2025-08-31T23:59:59Z',
    monthlyRecurringRevenue: 0,
    lastActivityDate: '2025-08-26T09:15:00Z',
    customerScore: 72,
    isSubscriptionActive: true,
    isSubscriptionExpiring: true
  },
  {
    id: '3',
    tenantId: 'tenant1',
    customerNumber: 'CUST-003',
    name: 'Enterprise Corp',
    type: 'Enterprise',
    status: 'Active',
    tier: 'Enterprise',
    source: 'Sales Team',
    primaryEmail: 'procurement@enterprisecorp.com',
    primaryPhone: '+1-555-0789',
    industry: 'Manufacturing',
    employeeCount: 5000,
    annualRevenue: 50000000,
    website: 'https://enterprisecorp.com',
    assignedTo: 'Michael Chen',
    createdAt: '2024-12-01T08:00:00Z',
    createdBy: 'admin',
    subscriptionStatus: 'Active',
    subscriptionStartDate: '2024-12-01T08:00:00Z',
    subscriptionEndDate: '2025-12-01T08:00:00Z',
    monthlyRecurringRevenue: 15000,
    lastActivityDate: '2025-08-24T16:45:00Z',
    customerScore: 95,
    isSubscriptionActive: true,
    isSubscriptionExpiring: false
  },
  {
    id: '4',
    tenantId: 'tenant1',
    customerNumber: 'CUST-004',
    name: 'Local Business Inc',
    type: 'Business',
    status: 'Prospect',
    tier: 'Standard',
    source: 'Trade Show',
    primaryEmail: 'info@localbusiness.com',
    primaryPhone: '+1-555-0321',
    industry: 'Retail',
    employeeCount: 50,
    annualRevenue: 1000000,
    createdAt: '2025-08-20T12:00:00Z',
    createdBy: 'admin',
    subscriptionStatus: 'Trial',
    subscriptionStartDate: '2025-08-20T12:00:00Z',
    subscriptionEndDate: '2025-09-20T12:00:00Z',
    monthlyRecurringRevenue: 0,
    lastActivityDate: '2025-08-22T10:30:00Z',
    customerScore: 60,
    isSubscriptionActive: true,
    isSubscriptionExpiring: false
  },
  {
    id: '5',
    tenantId: 'tenant1',
    customerNumber: 'CUST-005',
    name: 'Global Solutions Ltd',
    type: 'Enterprise',
    status: 'Inactive',
    tier: 'Premium',
    source: 'Website',
    primaryEmail: 'contact@globalsolutions.com',
    primaryPhone: '+1-555-0654',
    industry: 'Consulting',
    employeeCount: 500,
    annualRevenue: 10000000,
    website: 'https://globalsolutions.com',
    assignedTo: 'Emily Rodriguez',
    createdAt: '2024-06-15T14:00:00Z',
    createdBy: 'admin',
    subscriptionStatus: 'Expired',
    subscriptionStartDate: '2024-06-15T14:00:00Z',
    subscriptionEndDate: '2025-06-15T14:00:00Z',
    monthlyRecurringRevenue: 0,
    lastActivityDate: '2025-06-20T11:20:00Z',
    customerScore: 45,
    isSubscriptionActive: false,
    isSubscriptionExpiring: false
  }
])

// Computed properties
const filteredCustomers = computed(() => {
  return allCustomers.value.filter(customer => {
    const matchesSearch = !searchQuery.value || 
      customer.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      customer.primaryEmail?.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      customer.customerNumber.toLowerCase().includes(searchQuery.value.toLowerCase())
    
    const matchesType = !selectedType.value || customer.type === selectedType.value
    const matchesStatus = !selectedStatus.value || customer.status === selectedStatus.value
    const matchesTier = !selectedTier.value || customer.tier === selectedTier.value
    const matchesSubscription = !selectedSubscriptionStatus.value || customer.subscriptionStatus === selectedSubscriptionStatus.value
    
    return matchesSearch && matchesType && matchesStatus && matchesTier && matchesSubscription
  })
})

const sortedCustomers = computed(() => {
  const customers = [...filteredCustomers.value]
  
  customers.sort((a, b) => {
    const aValue = a[sortField.value as keyof Customer] as any
    const bValue = b[sortField.value as keyof Customer] as any
    
    if (aValue == null && bValue == null) return 0
    if (aValue == null) return sortDirection.value === 'asc' ? 1 : -1
    if (bValue == null) return sortDirection.value === 'asc' ? -1 : 1
    
    if (aValue < bValue) return sortDirection.value === 'asc' ? -1 : 1
    if (aValue > bValue) return sortDirection.value === 'asc' ? 1 : -1
    return 0
  })
  
  return customers
})

const paginatedCustomers = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return sortedCustomers.value.slice(start, end)
})

const totalCustomers = computed(() => filteredCustomers.value.length)
const totalPages = computed(() => Math.ceil(totalCustomers.value / pageSize.value))

// Stats
const totalMRR = computed(() => {
  return allCustomers.value.reduce((sum, customer) => sum + (customer.monthlyRecurringRevenue || 0), 0)
})

const trialCustomers = computed(() => {
  return allCustomers.value.filter(customer => customer.subscriptionStatus === 'Trial').length
})

const expiringCustomers = computed(() => {
  return allCustomers.value.filter(customer => customer.isSubscriptionExpiring).length
})

// Methods
const getInitials = (name: string): string => {
  return name?.split(' ').map(n => n[0]).join('').toUpperCase() || '??'
}

const typeClass = (type: string): string => {
  const classes = {
    'Individual': 'bg-gray-100 text-gray-800',
    'Business': 'bg-blue-100 text-blue-800',
    'Enterprise': 'bg-purple-100 text-purple-800'
  }
  return classes[type as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const tierClass = (tier: string): string => {
  const classes = {
    'Standard': 'bg-green-100 text-green-800',
    'Premium': 'bg-yellow-100 text-yellow-800',
    'Enterprise': 'bg-red-100 text-red-800'
  }
  return classes[tier as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const statusClass = (status: string): string => {
  const classes = {
    'Prospect': 'bg-yellow-100 text-yellow-800',
    'Active': 'bg-green-100 text-green-800',
    'Inactive': 'bg-gray-100 text-gray-800',
    'Churned': 'bg-red-100 text-red-800'
  }
  return classes[status as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const subscriptionStatusClass = (status: string): string => {
  const classes = {
    'Trial': 'bg-blue-100 text-blue-800',
    'Active': 'bg-green-100 text-green-800',
    'Suspended': 'bg-orange-100 text-orange-800',
    'Expired': 'bg-red-100 text-red-800',
    'Cancelled': 'bg-gray-100 text-gray-800'
  }
  return classes[status as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const formatDate = (dateString?: string): string => {
  if (!dateString) return 'Never'
  return new Date(dateString).toLocaleDateString()
}

const sortBy = (field: string): void => {
  if (sortField.value === field) {
    sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortField.value = field
    sortDirection.value = 'asc'
  }
}

const clearFilters = (): void => {
  searchQuery.value = ''
  selectedType.value = ''
  selectedStatus.value = ''
  selectedTier.value = ''
  selectedSubscriptionStatus.value = ''
  currentPage.value = 1
}

const toggleSelectAll = (): void => {
  if (selectAll.value) {
    selectedCustomers.value = paginatedCustomers.value.map(c => c.id)
  } else {
    selectedCustomers.value = []
  }
}

const viewCustomer = (customer: Customer): void => {
  selectedCustomerForView.value = customer
  showDetailsModal.value = true
}

const editCustomer = (customer: Customer): void => {
  selectedCustomerForEdit.value = customer
  isEditMode.value = true
  showCustomerModal.value = true
}

const saveCustomer = async (customerData: Customer): Promise<void> => {
  try {
    isLoading.value = true
    
    if (isEditMode.value && selectedCustomerForEdit.value) {
      // Update existing customer via API
      const response = await fetch(`http://localhost:5001/api/customers/${selectedCustomerForEdit.value.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(customerData)
      })
      
      if (response.ok) {
        const updatedCustomer = await response.json()
        const index = allCustomers.value.findIndex(c => c.id === selectedCustomerForEdit.value!.id)
        if (index !== -1) {
          allCustomers.value[index] = { ...updatedCustomer, modifiedAt: new Date().toISOString() }
        }
      } else {
        throw new Error(`Failed to update customer: ${response.statusText}`)
      }
    } else {
      // Create new customer via API
      const response = await fetch('http://localhost:5001/api/customers', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(customerData)
      })
      
      if (response.ok) {
        const newCustomer = await response.json()
        allCustomers.value.push(newCustomer)
      } else {
        throw new Error(`Failed to create customer: ${response.statusText}`)
      }
    }
    
    showCustomerModal.value = false
    selectedCustomerForEdit.value = null
    isEditMode.value = false
    
    // Refresh the customer list to get latest data
    await fetchCustomers()
  } catch (error) {
    console.error('Error saving customer:', error)
    // Show error to user (you could add a toast notification here)
    alert('Failed to save customer. Please try again.')
  } finally {
    isLoading.value = false
  }
}

const deleteCustomer = async (customer: Customer): Promise<void> => {
  if (confirm(`Are you sure you want to delete ${customer.name}?`)) {
    try {
      isLoading.value = true
      
      const response = await fetch(`http://localhost:5001/api/customers/${customer.id}`, {
        method: 'DELETE',
        headers: {
          'Content-Type': 'application/json',
        }
      })
      
      if (response.ok) {
        // Remove from local state
        const index = allCustomers.value.findIndex(c => c.id === customer.id)
        if (index !== -1) {
          allCustomers.value.splice(index, 1)
        }
        
        // Refresh the customer list to ensure consistency
        await fetchCustomers()
      } else {
        throw new Error(`Failed to delete customer: ${response.statusText}`)
      }
    } catch (error) {
      console.error('Error deleting customer:', error)
      alert('Failed to delete customer. Please try again.')
    } finally {
      isLoading.value = false
    }
  }
}

const manageSubscription = (customer: Customer): void => {
  selectedCustomerForSubscription.value = customer
  showSubscriptionModal.value = true
}

const updateSubscription = async (subscriptionData: any): Promise<void> => {
  try {
    const customer = selectedCustomerForSubscription.value
    if (!customer) {
      throw new Error('No customer selected for subscription update')
    }
    
    isLoading.value = true
    
    // Call the subscription management API endpoint
    const response = await fetch(`http://localhost:5001/api/customers/${customer.id}/subscription`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(subscriptionData)
    })
    
    if (response.ok) {
      const updatedCustomer = await response.json()
      
      // Update local state
      const index = allCustomers.value.findIndex(c => c.id === customer.id)
      if (index !== -1) {
        allCustomers.value[index] = {
          ...updatedCustomer,
          modifiedAt: new Date().toISOString()
        }
      }
      
      // Refresh the customer list to get latest data
      await fetchCustomers()
      
      showSubscriptionModal.value = false
      selectedCustomerForSubscription.value = null
    } else {
      throw new Error(`Failed to update subscription: ${response.statusText}`)
    }
  } catch (error) {
    console.error('Error updating subscription:', error)
    alert('Failed to update subscription. Please try again.')
  } finally {
    isLoading.value = false
  }
}

const viewActivities = (customer: Customer): void => {
  // Navigate to customer activities page
  console.log('View activities for', customer.name)
}

const addNote = (customer: Customer): void => {
  // Open note modal
  console.log('Add note for', customer.name)
}

const bulkUpdateStatus = (): void => {
  // Open bulk status update modal
  console.log('Bulk update status for', selectedCustomers.value.length, 'customers')
}

const bulkAssign = (): void => {
  // Open bulk assignment modal
  console.log('Bulk assign', selectedCustomers.value.length, 'customers')
}

// Additional modal handler methods
const handleCustomerEdit = (customer: any): void => {
  // Convert the customer data to match the modal interface
  editingCustomer.value = {
    id: customer.id,
    companyName: customer.name,
    customerType: customer.type.toLowerCase() as 'individual' | 'business' | 'enterprise',
    email: customer.primaryEmail || '',
    phone: customer.primaryPhone,
    address: customer.billingAddress ? `${customer.billingAddress.street}, ${customer.billingAddress.city}, ${customer.billingAddress.state} ${customer.billingAddress.postalCode}` : '',
    status: customer.status === 'Active' ? 'active' : customer.status === 'Inactive' ? 'inactive' : 'suspended' as 'active' | 'inactive' | 'suspended',
    tier: customer.tier === 'Standard' ? 'bronze' : customer.tier === 'Premium' ? 'silver' : customer.tier === 'Enterprise' ? 'gold' : 'platinum' as 'bronze' | 'silver' | 'gold' | 'platinum',
    subscriptionStatus: customer.subscriptionStatus === 'Active' ? 'active' : customer.subscriptionStatus === 'Trial' ? 'trial' : customer.subscriptionStatus === 'Suspended' ? 'suspended' : 'cancelled' as 'trial' | 'active' | 'suspended' | 'cancelled',
    subscriptionStartDate: customer.subscriptionStartDate,
    subscriptionEndDate: customer.subscriptionEndDate,
    monthlyRevenue: customer.monthlyRecurringRevenue,
    totalRevenue: customer.annualRevenue,
    contactCount: 1,
    createdAt: customer.createdAt,
    updatedAt: customer.modifiedAt || customer.createdAt,
    notes: ''
  } as any
  showCustomerModal.value = true
}

const handleSubscriptionManage = (customer: any): void => {
  // Convert the customer data to match the modal interface
  selectedCustomer.value = {
    id: customer.id,
    companyName: customer.name,
    customerType: customer.type.toLowerCase() as 'individual' | 'business' | 'enterprise',
    email: customer.primaryEmail || '',
    phone: customer.primaryPhone,
    address: customer.billingAddress ? `${customer.billingAddress.street}, ${customer.billingAddress.city}, ${customer.billingAddress.state} ${customer.billingAddress.postalCode}` : '',
    status: customer.status === 'Active' ? 'active' : customer.status === 'Inactive' ? 'inactive' : 'suspended' as 'active' | 'inactive' | 'suspended',
    tier: customer.tier === 'Standard' ? 'bronze' : customer.tier === 'Premium' ? 'silver' : customer.tier === 'Enterprise' ? 'gold' : 'platinum' as 'bronze' | 'silver' | 'gold' | 'platinum',
    subscriptionStatus: customer.subscriptionStatus === 'Active' ? 'active' : customer.subscriptionStatus === 'Trial' ? 'trial' : customer.subscriptionStatus === 'Suspended' ? 'suspended' : 'cancelled' as 'trial' | 'active' | 'suspended' | 'cancelled',
    subscriptionStartDate: customer.subscriptionStartDate,
    subscriptionEndDate: customer.subscriptionEndDate,
    monthlyRevenue: customer.monthlyRecurringRevenue,
    totalRevenue: customer.annualRevenue,
    contactCount: 1,
    createdAt: customer.createdAt,
    updatedAt: customer.modifiedAt || customer.createdAt,
    notes: ''
  } as any
  showSubscriptionModal.value = true
}

const handleCustomerSave = async (customerData: any): Promise<void> => {
  try {
    if (editingCustomer.value) {
      // Update existing customer - convert back to original format
      const index = allCustomers.value.findIndex(c => c.id === editingCustomer.value!.id)
      if (index !== -1) {
        allCustomers.value[index] = {
          ...allCustomers.value[index],
          name: customerData.companyName,
          type: customerData.customerType.charAt(0).toUpperCase() + customerData.customerType.slice(1) as 'Individual' | 'Business' | 'Enterprise',
          primaryEmail: customerData.email,
          primaryPhone: customerData.phone,
          status: customerData.status === 'active' ? 'Active' : customerData.status === 'inactive' ? 'Inactive' : 'Prospect' as 'Prospect' | 'Active' | 'Inactive' | 'Churned',
          tier: customerData.tier === 'bronze' ? 'Standard' : customerData.tier === 'silver' ? 'Premium' : 'Enterprise' as 'Standard' | 'Premium' | 'Enterprise',
          modifiedAt: new Date().toISOString()
        }
      }
    } else {
      // Create new customer
      const newCustomer = {
        id: Date.now().toString(),
        tenantId: 'tenant1',
        customerNumber: `CUST-${String(allCustomers.value.length + 1).padStart(3, '0')}`,
        name: customerData.companyName,
        type: customerData.customerType.charAt(0).toUpperCase() + customerData.customerType.slice(1) as 'Individual' | 'Business' | 'Enterprise',
        status: 'Prospect' as 'Prospect' | 'Active' | 'Inactive' | 'Churned',
        tier: customerData.tier === 'bronze' ? 'Standard' : customerData.tier === 'silver' ? 'Premium' : 'Enterprise' as 'Standard' | 'Premium' | 'Enterprise',
        source: 'Manual',
        primaryEmail: customerData.email,
        primaryPhone: customerData.phone,
        industry: '',
        assignedTo: 'current-user',
        createdAt: new Date().toISOString(),
        createdBy: 'current-user',
        subscriptionStatus: 'Trial' as 'Trial' | 'Active' | 'Suspended' | 'Expired' | 'Cancelled',
        monthlyRecurringRevenue: 0,
        customerScore: 50,
        isSubscriptionActive: false,
        isSubscriptionExpiring: false
      }
      allCustomers.value.push(newCustomer)
    }
    
    showCustomerModal.value = false
    editingCustomer.value = null
  } catch (error) {
    console.error('Error saving customer:', error)
  }
}

const handleSubscriptionUpdate = (data: any): void => {
  console.log('Subscription updated:', data)
  showSubscriptionModal.value = false
  selectedCustomer.value = null
}

const exportCustomers = (): void => {
  // Export customers to CSV
  const csvContent = [
    ['Customer Number', 'Name', 'Type', 'Status', 'Tier', 'Email', 'Phone', 'Industry', 'MRR', 'Created At'].join(','),
    ...filteredCustomers.value.map(customer => [
      customer.customerNumber,
      customer.name,
      customer.type,
      customer.status,
      customer.tier,
      customer.primaryEmail || '',
      customer.primaryPhone || '',
      customer.industry || '',
      customer.monthlyRecurringRevenue || 0,
      customer.createdAt
    ].join(','))
  ].join('\n')
  
  const blob = new Blob([csvContent], { type: 'text/csv' })
  const url = window.URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = `customers-${new Date().toISOString().split('T')[0]}.csv`
  link.click()
  window.URL.revokeObjectURL(url)
}

// Watchers
watch([searchQuery, selectedType, selectedStatus, selectedTier, selectedSubscriptionStatus], () => {
  currentPage.value = 1
})

watch(selectedCustomers, () => {
  selectAll.value = selectedCustomers.value.length === paginatedCustomers.value.length && paginatedCustomers.value.length > 0
})

onMounted(async () => {
  // Load customers data from API
  await fetchCustomers()
})

// API integration functions
const fetchCustomers = async (): Promise<void> => {
  try {
    isLoading.value = true
    const response = await fetch('/api/crm/customers')
    if (response.ok) {
      const data = await response.json()
      if (data && data.data) {
        allCustomers.value = data.data
      }
    }
  } catch (error) {
    console.error('Error fetching customers:', error)
    // Keep sample data as fallback for now
  } finally {
    isLoading.value = false
  }
}
</script>