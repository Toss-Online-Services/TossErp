<template>
  <div class="p-6 bg-white min-h-screen">
    <!-- Header Section -->
    <div class="sm:flex sm:items-center sm:justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold leading-tight tracking-tight text-gray-900 sm:text-4xl">
          Contacts
        </h1>
        <p class="mt-2 text-sm text-gray-700">
          Manage your business contacts and customer relationships
        </p>
      </div>
      <div class="mt-4 sm:ml-16 sm:mt-0 sm:flex-none">
        <button
          @click="openCreateModal"
          type="button"
          class="block rounded-md bg-indigo-600 px-3 py-2 text-center text-sm font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600"
        >
          <PlusIcon class="w-4 h-4 inline mr-2" />
          Add Contact
        </button>
      </div>
    </div>

    <!-- Filters and Search -->
    <div class="mb-6 space-y-4 sm:space-y-0 sm:flex sm:items-center sm:space-x-4">
      <!-- Search -->
      <div class="flex-1 max-w-lg">
        <div class="relative">
          <div class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3">
            <MagnifyingGlassIcon class="h-5 w-5 text-gray-400" />
          </div>
          <input
            v-model="searchQuery"
            type="text"
            class="block w-full rounded-md border-0 py-1.5 pl-10 pr-3 text-gray-900 ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6"
            placeholder="Search contacts..."
          />
        </div>
      </div>

      <!-- Filters -->
      <div class="flex space-x-4">
        <!-- Type Filter -->
        <select
          v-model="selectedType"
          class="rounded-md border-0 py-1.5 pl-3 pr-10 text-gray-900 ring-1 ring-inset ring-gray-300 focus:ring-2 focus:ring-indigo-600 sm:text-sm sm:leading-6"
        >
          <option value="">All Types</option>
          <option value="lead">Lead</option>
          <option value="customer">Customer</option>
          <option value="vendor">Vendor</option>
          <option value="partner">Partner</option>
        </select>

        <!-- Status Filter -->
        <select
          v-model="selectedStatus"
          class="rounded-md border-0 py-1.5 pl-3 pr-10 text-gray-900 ring-1 ring-inset ring-gray-300 focus:ring-2 focus:ring-indigo-600 sm:text-sm sm:leading-6"
        >
          <option value="">All Statuses</option>
          <option value="active">Active</option>
          <option value="inactive">Inactive</option>
          <option value="prospect">Prospect</option>
        </select>

        <!-- Export Button -->
        <button
          @click="exportContacts"
          type="button"
          class="inline-flex items-center rounded-md bg-white px-3 py-2 text-sm font-semibold text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 hover:bg-gray-50"
        >
          <ArrowDownTrayIcon class="w-4 h-4 mr-2" />
          Export
        </button>
      </div>
    </div>

    <!-- Contacts Table -->
    <div class="mt-8 flow-root">
      <div class="-mx-4 -my-2 overflow-x-auto sm:-mx-6 lg:-mx-8">
        <div class="inline-block min-w-full py-2 align-middle sm:px-6 lg:px-8">
          <div class="relative overflow-hidden shadow ring-1 ring-black ring-opacity-5 sm:rounded-lg">
            <table class="min-w-full table-fixed divide-y divide-gray-300">
              <thead class="bg-gray-50">
                <tr>
                  <th scope="col" class="relative px-7 sm:w-12 sm:px-6">
                    <input
                      v-model="selectAll"
                      @change="toggleSelectAll"
                      type="checkbox"
                      class="absolute left-4 top-1/2 -mt-2 h-4 w-4 rounded border-gray-300 text-indigo-600 focus:ring-indigo-600"
                    />
                  </th>
                  <th
                    v-for="column in columns"
                    :key="column.key"
                    scope="col"
                    :class="[
                      'min-w-0 py-3.5 pr-3 text-left text-sm font-semibold text-gray-900 cursor-pointer hover:bg-gray-100',
                      column.sortable ? 'select-none' : ''
                    ]"
                    @click="column.sortable ? sortBy(column.key) : null"
                  >
                    <div class="flex items-center space-x-1">
                      <span>{{ column.label }}</span>
                      <template v-if="column.sortable">
                        <ChevronUpIcon
                          v-if="sortField === column.key && sortDirection === 'asc'"
                          class="w-4 h-4"
                        />
                        <ChevronDownIcon
                          v-else-if="sortField === column.key && sortDirection === 'desc'"
                          class="w-4 h-4"
                        />
                        <ChevronUpDownIcon v-else class="w-4 h-4 text-gray-400" />
                      </template>
                    </div>
                  </th>
                  <th scope="col" class="relative py-3.5 pl-3 pr-4 sm:pr-6">
                    <span class="sr-only">Actions</span>
                  </th>
                </tr>
              </thead>
              <tbody class="divide-y divide-gray-200 bg-white">
                <tr
                  v-for="contact in paginatedContacts"
                  :key="contact.id"
                  :class="[
                    selectedContacts.includes(contact.id) ? 'bg-gray-50' : 'hover:bg-gray-50'
                  ]"
                >
                  <td class="relative px-7 sm:w-12 sm:px-6">
                    <input
                      v-model="selectedContacts"
                      :value="contact.id"
                      type="checkbox"
                      class="absolute left-4 top-1/2 -mt-2 h-4 w-4 rounded border-gray-300 text-indigo-600 focus:ring-indigo-600"
                    />
                  </td>
                  <td class="whitespace-nowrap px-3 py-4 text-sm">
                    <div class="flex items-center">
                      <div class="h-10 w-10 flex-shrink-0">
                        <div class="h-10 w-10 rounded-full bg-indigo-600 flex items-center justify-center">
                          <span class="text-sm font-medium text-white">
                            {{ getInitials(contact.name || `${contact.firstName} ${contact.lastName}`) }}
                          </span>
                        </div>
                      </div>
                      <div class="ml-4">
                        <div class="font-medium text-gray-900">{{ contact.name || `${contact.firstName} ${contact.lastName}` }}</div>
                        <div class="text-gray-500">{{ contact.company || 'No company' }}</div>
                      </div>
                    </div>
                  </td>
                  <td class="whitespace-nowrap px-3 py-4 text-sm text-gray-900">
                    <div>{{ contact.email }}</div>
                    <div class="text-gray-500">{{ contact.phone || 'No phone' }}</div>
                  </td>
                  <td class="whitespace-nowrap px-3 py-4 text-sm text-gray-500">
                    <span
                      :class="[
                        'inline-flex items-center rounded-md px-2 py-1 text-xs font-medium ring-1 ring-inset',
                        getTypeColor(contact.type)
                      ]"
                    >
                      {{ contact.type }}
                    </span>
                  </td>
                  <td class="whitespace-nowrap px-3 py-4 text-sm text-gray-500">
                    <span
                      :class="[
                        'inline-flex items-center rounded-md px-2 py-1 text-xs font-medium ring-1 ring-inset',
                        getStatusColor(contact.status)
                      ]"
                    >
                      {{ contact.status }}
                    </span>
                  </td>
                  <td class="whitespace-nowrap px-3 py-4 text-sm text-gray-500">
                    {{ formatDate(contact.createdAt) }}
                  </td>
                  <td class="whitespace-nowrap px-3 py-4 text-sm text-gray-500">
                    {{ formatDate(contact.lastActivity) }}
                  </td>
                  <td class="relative whitespace-nowrap py-4 pl-3 pr-4 text-right text-sm font-medium sm:pr-6">
                    <div class="flex items-center justify-end space-x-2">
                      <button
                        @click="viewContact(contact)"
                        class="text-indigo-600 hover:text-indigo-900 hover:bg-indigo-50 p-2 rounded-md"
                        title="View Contact"
                      >
                        <EyeIcon class="w-4 h-4" />
                      </button>
                      <button
                        @click="editContact(contact)"
                        class="text-gray-600 hover:text-gray-900 hover:bg-gray-50 p-2 rounded-md"
                        title="Edit Contact"
                      >
                        <PencilIcon class="w-4 h-4" />
                      </button>
                      <button
                        @click="deleteContact(contact)"
                        class="text-red-600 hover:text-red-900 hover:bg-red-50 p-2 rounded-md"
                        title="Delete Contact"
                      >
                        <TrashIcon class="w-4 h-4" />
                      </button>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>

            <!-- Empty State -->
            <div v-if="paginatedContacts.length === 0" class="text-center py-12">
              <UserIcon class="mx-auto h-12 w-12 text-gray-400" />
              <h3 class="mt-2 text-sm font-semibold text-gray-900">No contacts found</h3>
              <p class="mt-1 text-sm text-gray-500">
                {{ searchQuery ? 'Try adjusting your search criteria.' : 'Get started by creating your first contact.' }}
              </p>
              <div class="mt-6">
                <button
                  v-if="!searchQuery"
                  @click="openCreateModal"
                  type="button"
                  class="inline-flex items-center rounded-md bg-indigo-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-500"
                >
                  <PlusIcon class="w-4 h-4 mr-2" />
                  Add Contact
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Pagination -->
    <nav
      v-if="totalPages > 1"
      class="flex items-center justify-between border-t border-gray-200 bg-white px-4 py-3 sm:px-6 mt-6"
    >
      <div class="flex flex-1 justify-between sm:hidden">
        <button
          @click="previousPage"
          :disabled="currentPage === 1"
          class="relative inline-flex items-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-50"
        >
          Previous
        </button>
        <button
          @click="nextPage"
          :disabled="currentPage === totalPages"
          class="relative ml-3 inline-flex items-center rounded-md border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50 disabled:opacity-50"
        >
          Next
        </button>
      </div>
      <div class="hidden sm:flex sm:flex-1 sm:items-center sm:justify-between">
        <div>
          <p class="text-sm text-gray-700">
            Showing
            {{ ' ' }}
            <span class="font-medium">{{ (currentPage - 1) * pageSize + 1 }}</span>
            {{ ' ' }}
            to
            {{ ' ' }}
            <span class="font-medium">{{ Math.min(currentPage * pageSize, totalContacts) }}</span>
            {{ ' ' }}
            of
            {{ ' ' }}
            <span class="font-medium">{{ totalContacts }}</span>
            {{ ' ' }}
            results
          </p>
        </div>
        <div>
          <nav class="isolate inline-flex -space-x-px rounded-md shadow-sm">
            <button
              @click="previousPage"
              :disabled="currentPage === 1"
              class="relative inline-flex items-center rounded-l-md px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0 disabled:opacity-50"
            >
              <ChevronLeftIcon class="h-5 w-5" />
            </button>
            
            <button
              v-for="page in visiblePages"
              :key="page"
              @click="goToPage(page)"
              :class="[
                'relative inline-flex items-center px-4 py-2 text-sm font-semibold focus:z-20 focus:outline-offset-0',
                page === currentPage
                  ? 'z-10 bg-indigo-600 text-white focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600'
                  : 'text-gray-900 ring-1 ring-inset ring-gray-300 hover:bg-gray-50'
              ]"
            >
              {{ page }}
            </button>
            
            <button
              @click="nextPage"
              :disabled="currentPage === totalPages"
              class="relative inline-flex items-center rounded-r-md px-2 py-2 text-gray-400 ring-1 ring-inset ring-gray-300 hover:bg-gray-50 focus:z-20 focus:outline-offset-0 disabled:opacity-50"
            >
              <ChevronRightIcon class="h-5 w-5" />
            </button>
          </nav>
        </div>
      </div>
    </nav>

    <!-- Bulk Actions Bar -->
    <Transition
      enter-active-class="transform ease-out duration-300 transition"
      enter-from-class="translate-y-2 opacity-0 sm:translate-y-0 sm:translate-x-2"
      enter-to-class="translate-y-0 opacity-100 sm:translate-x-0"
      leave-active-class="transition ease-in duration-100"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div
        v-if="selectedContacts.length > 0"
        class="fixed bottom-6 left-1/2 transform -translate-x-1/2 bg-white rounded-lg shadow-lg border border-gray-200 px-6 py-4 flex items-center space-x-4 z-50"
      >
        <span class="text-sm text-gray-700">
          {{ selectedContacts.length }} selected
        </span>
        <div class="flex space-x-2">
          <button
            @click="bulkEdit"
            class="inline-flex items-center rounded-md bg-white px-3 py-2 text-sm font-semibold text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 hover:bg-gray-50"
          >
            <PencilIcon class="w-4 h-4 mr-2" />
            Edit
          </button>
          <button
            @click="bulkDelete"
            class="inline-flex items-center rounded-md bg-red-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-red-500"
          >
            <TrashIcon class="w-4 h-4 mr-2" />
            Delete
          </button>
          <button
            @click="clearSelection"
            class="inline-flex items-center rounded-md bg-gray-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-gray-500"
          >
            <XMarkIcon class="w-4 h-4 mr-2" />
            Clear
          </button>
        </div>
      </div>
    </Transition>

    <!-- Contact Modal (Create/Edit) -->
    <ContactModal
      v-if="showContactModal"
      :contact="selectedContactForEdit"
      :is-edit-mode="isEditMode"
      @close="closeContactModal"
      @save="saveContact"
    />

    <!-- Contact Details Modal -->
    <ContactDetailsModal
      v-if="showDetailsModal"
      :contact="selectedContactForView"
      @close="closeDetailsModal"
      @edit="editContactFromDetails"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import {
  PlusIcon,
  MagnifyingGlassIcon,
  ArrowDownTrayIcon,
  ChevronUpIcon,
  ChevronDownIcon,
  ChevronUpDownIcon,
  ChevronLeftIcon,
  ChevronRightIcon,
  EyeIcon,
  PencilIcon,
  TrashIcon,
  UserIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

// Types
interface Contact {
  id: string
  firstName: string
  lastName: string
  email: string
  phone?: string
  company?: string
  jobTitle?: string
  type: 'Lead' | 'Customer' | 'Prospect' 
  status: 'Active' | 'Inactive'
  source?: string
  createdAt: string
  lastActivity?: string
  // Computed properties for compatibility
  name?: string // Will be computed from firstName + lastName
}

interface Column {
  key: string
  label: string
  sortable: boolean
}

// Reactive state
const searchQuery = ref('')
const selectedType = ref('')
const selectedStatus = ref('')
const sortField = ref<string>('name')
const sortDirection = ref<'asc' | 'desc'>('asc')
const currentPage = ref(1)
const pageSize = ref(10)
const selectedContacts = ref<string[]>([])
const selectAll = ref(false)
const showContactModal = ref(false)
const showDetailsModal = ref(false)
const selectedContactForEdit = ref<Contact | null>(null)
const selectedContactForView = ref<Contact | null>(null)
const isEditMode = ref(false)

// Table columns
const columns: Column[] = [
  { key: 'name', label: 'Name', sortable: true },
  { key: 'email', label: 'Contact Info', sortable: true },
  { key: 'type', label: 'Type', sortable: true },
  { key: 'status', label: 'Status', sortable: true },
  { key: 'createdAt', label: 'Created', sortable: true },
  { key: 'lastActivity', label: 'Last Activity', sortable: true }
]

// API Configuration
const API_BASE_URL = 'http://localhost:5048/api'

// API service for contacts
const contactsApi = {
  async getContacts(query: Record<string, any> = {}) {
    const params = new URLSearchParams()
    Object.entries(query).forEach(([key, value]) => {
      if (value !== undefined && value !== null && value !== '') {
        params.append(key, value.toString())
      }
    })
    
    const response = await fetch(`${API_BASE_URL}/crmcontacts?${params}`)
    if (!response.ok) throw new Error('Failed to fetch contacts')
    return await response.json()
  },

  async getContact(id: string) {
    const response = await fetch(`${API_BASE_URL}/crmcontacts/${id}`)
    if (!response.ok) throw new Error('Failed to fetch contact')
    return await response.json()
  },

  async createContact(contact: Contact) {
    const response = await fetch(`${API_BASE_URL}/crmcontacts`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(contact)
    })
    if (!response.ok) throw new Error('Failed to create contact')
    return await response.json()
  },

  async updateContact(id: string, contact: Contact) {
    const response = await fetch(`${API_BASE_URL}/crmcontacts/${id}`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(contact)
    })
    if (!response.ok) throw new Error('Failed to update contact')
    return await response.json()
  },

  async deleteContact(id: string) {
    const response = await fetch(`${API_BASE_URL}/crmcontacts/${id}`, {
      method: 'DELETE'
    })
    if (!response.ok) throw new Error('Failed to delete contact')
  },

  async bulkDeleteContacts(ids: string[]) {
    const response = await fetch(`${API_BASE_URL}/crmcontacts/bulk-delete`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(ids)
    })
    if (!response.ok) throw new Error('Failed to bulk delete contacts')
    return await response.json()
  },

  async exportContacts() {
    const response = await fetch(`${API_BASE_URL}/crmcontacts/export`)
    if (!response.ok) throw new Error('Failed to export contacts')
    return await response.blob()
  }
}
const allContacts = ref<Contact[]>([])
const loading = ref(false)
const error = ref<string | null>(null)

// Load contacts from API
const loadContacts = async () => {
  try {
    loading.value = true
    error.value = null
    const data = await contactsApi.getContacts()
    // Transform API data to match our interface
    allContacts.value = data.map((contact: any) => ({
      ...contact,
      name: `${contact.firstName} ${contact.lastName}`.trim() // Computed name from firstName + lastName
    }))
  } catch (err) {
    error.value = 'Failed to load contacts'
    console.error('Error loading contacts:', err)
    // Fallback to sample data for now
    allContacts.value = [
      {
        id: '1',
        firstName: 'Thabo',
        lastName: 'Mthembu',
        name: 'Thabo Mthembu',
        email: 'thabo@ruralenterprises.co.za',
        phone: '+27 82 123 4567',
        company: 'Mthembu Spaza Shop',
        jobTitle: 'Owner',
        type: 'Customer',
        status: 'Active',
        source: 'Website',
        createdAt: '2024-01-15T10:30:00Z',
        lastActivity: '2024-08-20T14:22:00Z'
      },
      {
        id: '2',
        firstName: 'Nomsa',
        lastName: 'Khumalo',
        name: 'Nomsa Khumalo',
        email: 'nomsa.khumalo@gmail.com',
        phone: '+27 83 456 7890',
        company: 'Khumalo Catering Services',
        jobTitle: 'CEO',
        type: 'Lead',
        status: 'Active',
        source: 'Referral',
        createdAt: '2024-02-01T09:15:00Z',
        lastActivity: '2024-08-18T11:45:00Z'
      },
      {
        id: '3',
        firstName: 'Johannes',
        lastName: 'Pieterse',
        name: 'Johannes Pieterse',
        email: 'johannes@farmsupplies.co.za',
        phone: '+27 81 789 0123',
        company: 'Pieterse Farm Supplies',
        jobTitle: 'Manager',
        type: 'Prospect',
        status: 'Active',
        source: 'Trade Show',
        createdAt: '2024-01-20T16:45:00Z',
        lastActivity: '2024-08-19T09:30:00Z'
      },
      {
        id: '4',
        firstName: 'Lerato',
        lastName: 'Mokone',
        name: 'Lerato Mokone',
        email: 'lerato@handicrafts.org',
        phone: '+27 84 234 5678',
        company: 'Traditional Handicrafts Co-op',
        jobTitle: 'Coordinator',
        type: 'Customer',
        status: 'Active',
        source: 'Partner',
        createdAt: '2024-03-10T13:20:00Z',
        lastActivity: '2024-08-21T15:10:00Z'
      },
      {
        id: '5',
        firstName: 'Sipho',
        lastName: 'Ndlovu',
        name: 'Sipho Ndlovu',
        email: 'sipho.ndlovu@transport.co.za',
        phone: '+27 85 345 6789',
        company: 'Ndlovu Transport Services',
        jobTitle: 'Owner',
        type: 'Customer',
        status: 'Active',
        source: 'Cold Call',
        createdAt: '2024-02-14T11:00:00Z',
        lastActivity: '2024-08-17T12:35:00Z'
      }
    ]
  } finally {
    loading.value = false
  }
}

// Load contacts on component mount
onMounted(() => {
  loadContacts()
})

// Computed properties
const filteredContacts = computed(() => {
  let filtered = allContacts.value

  // Search filter
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(contact =>
      (contact.name || `${contact.firstName} ${contact.lastName}`).toLowerCase().includes(query) ||
      contact.email.toLowerCase().includes(query) ||
      contact.company?.toLowerCase().includes(query) ||
      contact.phone?.includes(query)
    )
  }

  // Type filter
  if (selectedType.value) {
    filtered = filtered.filter(contact => contact.type === selectedType.value)
  }

  // Status filter
  if (selectedStatus.value) {
    filtered = filtered.filter(contact => contact.status === selectedStatus.value)
  }

  return filtered
})

const sortedContacts = computed(() => {
  const sorted = [...filteredContacts.value]
  
  if (sortField.value) {
    sorted.sort((a, b) => {
      const aValue = a[sortField.value as keyof Contact] || ''
      const bValue = b[sortField.value as keyof Contact] || ''
      
      let comparison = 0
      if (aValue < bValue) comparison = -1
      if (aValue > bValue) comparison = 1
      
      return sortDirection.value === 'desc' ? -comparison : comparison
    })
  }
  
  return sorted
})

const totalContacts = computed(() => sortedContacts.value.length)
const totalPages = computed(() => Math.ceil(totalContacts.value / pageSize.value))

const paginatedContacts = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return sortedContacts.value.slice(start, end)
})

const visiblePages = computed(() => {
  const pages: number[] = []
  const start = Math.max(1, currentPage.value - 2)
  const end = Math.min(totalPages.value, currentPage.value + 2)
  
  for (let i = start; i <= end; i++) {
    pages.push(i)
  }
  
  return pages
})

// Methods
const getInitials = (name: string): string => {
  return name
    .split(' ')
    .map(word => word.charAt(0))
    .join('')
    .toUpperCase()
    .slice(0, 2)
}

const getTypeColor = (type: string): string => {
  const colors = {
    lead: 'bg-yellow-50 text-yellow-800 ring-yellow-600/20',
    customer: 'bg-green-50 text-green-800 ring-green-600/20',
    vendor: 'bg-blue-50 text-blue-800 ring-blue-600/20',
    partner: 'bg-purple-50 text-purple-800 ring-purple-600/20'
  }
  return colors[type as keyof typeof colors] || 'bg-gray-50 text-gray-800 ring-gray-600/20'
}

const getStatusColor = (status: string): string => {
  const colors = {
    active: 'bg-green-50 text-green-800 ring-green-600/20',
    inactive: 'bg-gray-50 text-gray-800 ring-gray-600/20',
    prospect: 'bg-yellow-50 text-yellow-800 ring-yellow-600/20'
  }
  return colors[status as keyof typeof colors] || 'bg-gray-50 text-gray-800 ring-gray-600/20'
}

const formatDate = (dateString: string | undefined): string => {
  if (!dateString) return 'Never'
  const date = new Date(dateString)
  return date.toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

const sortBy = (field: string) => {
  if (sortField.value === field) {
    sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortField.value = field
    sortDirection.value = 'asc'
  }
}

const toggleSelectAll = () => {
  if (selectAll.value) {
    selectedContacts.value = paginatedContacts.value.map(contact => contact.id)
  } else {
    selectedContacts.value = []
  }
}

const clearSelection = () => {
  selectedContacts.value = []
  selectAll.value = false
}

const goToPage = (page: number) => {
  currentPage.value = page
}

const previousPage = () => {
  if (currentPage.value > 1) {
    currentPage.value--
  }
}

const nextPage = () => {
  if (currentPage.value < totalPages.value) {
    currentPage.value++
  }
}

const openCreateModal = () => {
  selectedContactForEdit.value = null
  isEditMode.value = false
  showContactModal.value = true
}

const closeContactModal = () => {
  showContactModal.value = false
  selectedContactForEdit.value = null
}

const viewContact = (contact: Contact) => {
  selectedContactForView.value = contact
  showDetailsModal.value = true
}

const closeDetailsModal = () => {
  showDetailsModal.value = false
  selectedContactForView.value = null
}

const editContact = (contact: Contact) => {
  selectedContactForEdit.value = contact
  isEditMode.value = true
  showContactModal.value = true
}

const editContactFromDetails = (contact: Contact) => {
  closeDetailsModal()
  editContact(contact)
}

const deleteContact = (contact: Contact) => {
  if (confirm(`Are you sure you want to delete ${contact.name}?`)) {
    const index = allContacts.value.findIndex(c => c.id === contact.id)
    if (index > -1) {
      allContacts.value.splice(index, 1)
    }
    // Remove from selection if selected
    selectedContacts.value = selectedContacts.value.filter(id => id !== contact.id)
  }
}

const saveContact = (contact: Contact) => {
  if (isEditMode.value) {
    // Update existing contact
    const index = allContacts.value.findIndex(c => c.id === contact.id)
    if (index > -1) {
      allContacts.value[index] = { ...contact }
    }
  } else {
    // Add new contact
    const newContact: Contact = {
      ...contact,
      id: Date.now().toString(),
      createdAt: new Date().toISOString(),
      lastActivity: new Date().toISOString()
    }
    allContacts.value.push(newContact)
  }
  closeContactModal()
}

const bulkEdit = () => {
  // Implement bulk edit functionality
  console.log('Bulk edit contacts:', selectedContacts.value)
}

const bulkDelete = () => {
  if (confirm(`Are you sure you want to delete ${selectedContacts.value.length} contacts?`)) {
    allContacts.value = allContacts.value.filter(contact => 
      !selectedContacts.value.includes(contact.id)
    )
    clearSelection()
  }
}

const exportContacts = () => {
  // Implement export functionality
  const data = filteredContacts.value
  const csv = [
    ['Name', 'Email', 'Phone', 'Company', 'Type', 'Status', 'Created'],
    ...data.map(contact => [
      contact.name,
      contact.email,
      contact.phone || '',
      contact.company || '',
      contact.type,
      contact.status,
      formatDate(contact.createdAt)
    ])
  ].map(row => row.join(',')).join('\n')
  
  const blob = new Blob([csv], { type: 'text/csv' })
  const url = URL.createObjectURL(blob)
  const a = document.createElement('a')
  a.href = url
  a.download = 'contacts.csv'
  a.click()
  URL.revokeObjectURL(url)
}

// Watchers
watch([searchQuery, selectedType, selectedStatus], () => {
  currentPage.value = 1
})

watch(selectedContacts, (newSelection) => {
  selectAll.value = newSelection.length === paginatedContacts.value.length && newSelection.length > 0
})
</script>
