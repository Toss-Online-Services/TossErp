<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <!-- Mobile-First Page Container -->
    <div class="p-4 sm:p-6 space-y-4 sm:space-y-6 pb-20 lg:pb-6">
      
      <!-- Enterprise Configuration -->
      <EnterpriseSelector @enterprise-changed="handleEnterpriseChange" />

      <!-- Page Header -->
      <div class="text-center sm:text-left">
        <h1 class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">
          {{ enterpriseTitle }} Contacts
        </h1>
        <p class="text-slate-600 dark:text-slate-400 mt-1 text-sm sm:text-base">
          {{ enterpriseSubtitle }}
        </p>
      </div>

      <!-- Quick Actions -->
      <div class="flex flex-wrap gap-3">
        <button
          @click="openCreateModal"
          class="inline-flex items-center px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white text-sm font-medium rounded-lg transition-colors"
        >
          <PlusIcon class="w-4 h-4 mr-2" />
          Add {{ getContactTypeLabel() }}
        </button>
        <NuxtLink
          to="/crm/automation"
          class="inline-flex items-center px-4 py-2 bg-orange-600 hover:bg-orange-700 text-white text-sm font-medium rounded-lg transition-colors"
        >
          <CogIcon class="w-4 h-4 mr-2" />
          Automation
        </NuxtLink>
        <button
          v-if="currentEnterprise?.serviceOfferings?.length"
          @click="showServicesModal = true"
          class="inline-flex items-center px-4 py-2 bg-indigo-600 hover:bg-indigo-700 text-white text-sm font-medium rounded-lg transition-colors"
        >
          <WrenchScrewdriverIcon class="w-4 h-4 mr-2" />
          Services
        </button>
        <button
          @click="exportContacts"
          class="inline-flex items-center px-4 py-2 bg-green-600 hover:bg-green-700 text-white text-sm font-medium rounded-lg transition-colors"
        >
          <ArrowDownTrayIcon class="w-4 h-4 mr-2" />
          Export
        </button>
        <button
          @click="importContacts"
          class="inline-flex items-center px-4 py-2 bg-purple-600 hover:bg-purple-700 text-white text-sm font-medium rounded-lg transition-colors"
        >
          <ArrowUpTrayIcon class="w-4 h-4 mr-2" />
          Import
        </button>
      </div>

      <!-- Search and Actions Bar - Mobile First -->
      <div class="space-y-3 sm:space-y-0 sm:flex sm:items-center sm:justify-between">
        <!-- Search -->
        <div class="relative flex-1 max-w-lg">
          <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <MagnifyingGlassIcon class="h-4 w-4 sm:h-5 sm:w-5 text-slate-400" />
          </div>
          <input
            v-model="searchQuery"
            type="text"
            class="block w-full pl-9 sm:pl-10 pr-3 py-2 sm:py-2.5 text-sm border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-800 text-slate-900 dark:text-white placeholder-slate-500 dark:placeholder-slate-400 focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            placeholder="Search contacts..."
          />
        </div>

        <!-- Filters and Actions -->
        <div class="flex flex-wrap gap-2 sm:gap-3 sm:ml-4">
          <!-- Type Filter -->
          <select
            v-model="selectedType"
            class="px-3 py-2 text-sm border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-800 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          >
            <option value="">All Types</option>
            <option
              v-for="contactType in availableContactTypes"
              :key="contactType.id"
              :value="contactType.name"
            >
              {{ contactType.name }}
            </option>
          </select>

        <!-- Status Filter -->
        <select
          v-model="selectedStatus"
          class="px-3 py-2 text-sm border border-slate-300 dark:border-slate-600 rounded-lg bg-white dark:bg-slate-800 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-transparent"
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
          class="inline-flex items-center rounded-lg bg-white dark:bg-slate-800 px-3 py-2 text-sm font-semibold text-slate-900 dark:text-white shadow-sm border border-slate-300 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700"
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
          <div class="relative overflow-hidden shadow-sm ring-1 ring-slate-200 dark:ring-slate-700 sm:rounded-lg">
            <table class="min-w-full table-fixed divide-y divide-slate-200 dark:divide-slate-700">
              <thead class="bg-slate-50 dark:bg-slate-800">
                <tr>
                  <th scope="col" class="relative px-7 sm:w-12 sm:px-6">
                    <input
                      v-model="selectAll"
                      @change="toggleSelectAll"
                      type="checkbox"
                      class="absolute left-4 top-1/2 -mt-2 h-4 w-4 rounded border-slate-300 dark:border-slate-600 text-blue-600 focus:ring-blue-500"
                    />
                  </th>
                  <th
                    v-for="column in columns"
                    :key="column.key"
                    scope="col"
                    :class="[
                      'min-w-0 py-3.5 pr-3 text-left text-sm font-semibold text-slate-900 dark:text-slate-100 cursor-pointer hover:bg-slate-100 dark:hover:bg-slate-700',
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
                        <ChevronUpDownIcon v-else class="w-4 h-4 text-slate-400 dark:text-slate-500" />
                      </template>
                    </div>
                  </th>
                  <th scope="col" class="relative py-3.5 pl-3 pr-4 sm:pr-6">
                    <span class="sr-only">Actions</span>
                  </th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-200 dark:divide-slate-700 bg-white dark:bg-slate-900">
                <tr
                  v-for="contact in paginatedContacts"
                  :key="contact.id"
                  :class="[
                    selectedContacts.includes(contact.id) ? 'bg-slate-50 dark:bg-slate-800' : 'hover:bg-slate-50 dark:hover:bg-slate-800'
                  ]"
                >
                  <td class="relative px-7 sm:w-12 sm:px-6">
                    <input
                      v-model="selectedContacts"
                      :value="contact.id"
                      type="checkbox"
                      class="absolute left-4 top-1/2 -mt-2 h-4 w-4 rounded border-slate-300 dark:border-slate-600 text-blue-600 focus:ring-blue-500"
                    />
                  </td>
                  <td class="whitespace-nowrap px-3 py-4 text-sm">
                    <div class="flex items-center">
                      <div class="h-10 w-10 flex-shrink-0">
                        <div class="h-10 w-10 rounded-full bg-blue-600 flex items-center justify-center">
                          <span class="text-sm font-medium text-white">
                            {{ getInitials(contact.name || `${contact.firstName} ${contact.lastName}`) }}
                          </span>
                        </div>
                      </div>
                      <div class="ml-4">
                        <div class="font-medium text-slate-900 dark:text-slate-100">{{ contact.name || `${contact.firstName} ${contact.lastName}` }}</div>
                        <div class="text-slate-500 dark:text-slate-400">{{ contact.company || 'No company' }}</div>
                      </div>
                    </div>
                  </td>
                  <td class="whitespace-nowrap px-3 py-4 text-sm text-slate-900 dark:text-slate-100">
                    <div>{{ contact.email }}</div>
                    <div class="text-slate-500 dark:text-slate-400">{{ contact.phone || 'No phone' }}</div>
                  </td>
                  <td class="whitespace-nowrap px-3 py-4 text-sm text-slate-500 dark:text-slate-400">
                    <span
                      :class="[
                        'inline-flex items-center rounded-md px-2 py-1 text-xs font-medium ring-1 ring-inset',
                        getTypeColor(contact.type)
                      ]"
                    >
                      {{ contact.type }}
                    </span>
                  </td>
                  <td class="whitespace-nowrap px-3 py-4 text-sm text-slate-500 dark:text-slate-400">
                    <span
                      :class="[
                        'inline-flex items-center rounded-md px-2 py-1 text-xs font-medium ring-1 ring-inset',
                        getStatusColor(contact.status)
                      ]"
                    >
                      {{ contact.status }}
                    </span>
                  </td>
                  <td class="whitespace-nowrap px-3 py-4 text-sm text-slate-500 dark:text-slate-400">
                    {{ formatDate(contact.createdAt) }}
                  </td>
                  <td class="whitespace-nowrap px-3 py-4 text-sm text-slate-500 dark:text-slate-400">
                    {{ formatDate(contact.lastActivity) }}
                  </td>
                  <td class="relative whitespace-nowrap py-4 pl-3 pr-4 text-right text-sm font-medium sm:pr-6">
                    <div class="flex items-center justify-end space-x-2">
                      <button
                        @click="viewContact(contact)"
                        class="text-blue-600 hover:text-blue-900 hover:bg-blue-50 dark:hover:bg-blue-900/20 p-2 rounded-md"
                        title="View Contact"
                      >
                        <EyeIcon class="w-4 h-4" />
                      </button>
                      <button
                        @click="editContact(contact)"
                        class="text-slate-600 dark:text-slate-400 hover:text-slate-900 dark:hover:text-slate-100 hover:bg-slate-50 dark:hover:bg-slate-700 p-2 rounded-md"
                        title="Edit Contact"
                      >
                        <PencilIcon class="w-4 h-4" />
                      </button>
                      <button
                        @click="deleteContact(contact)"
                        class="text-red-600 hover:text-red-900 hover:bg-red-50 dark:hover:bg-red-900/20 p-2 rounded-md"
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
              <UserIcon class="mx-auto h-12 w-12 text-slate-400 dark:text-slate-500" />
              <h3 class="mt-2 text-sm font-semibold text-slate-900 dark:text-slate-100">No contacts found</h3>
              <p class="mt-1 text-sm text-slate-500 dark:text-slate-400">
                {{ searchQuery ? 'Try adjusting your search criteria.' : 'Get started by creating your first contact.' }}
              </p>
              <div class="mt-6">
                <button
                  v-if="!searchQuery"
                  @click="openCreateModal"
                  type="button"
                  class="inline-flex items-center rounded-md bg-slate-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-slate-500 dark:bg-slate-700 dark:hover:bg-slate-600"
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
      class="flex items-center justify-between border-t border-slate-200 dark:border-slate-700 bg-white dark:bg-slate-900 px-4 py-3 sm:px-6 mt-6"
    >
      <div class="flex flex-1 justify-between sm:hidden">
        <button
          @click="previousPage"
          :disabled="currentPage === 1"
          class="relative inline-flex items-center rounded-md border border-slate-300 dark:border-slate-600 bg-white dark:bg-slate-800 px-4 py-2 text-sm font-medium text-slate-700 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-700 disabled:opacity-50"
        >
          Previous
        </button>
        <button
          @click="nextPage"
          :disabled="currentPage === totalPages"
          class="relative ml-3 inline-flex items-center rounded-md border border-slate-300 dark:border-slate-600 bg-white dark:bg-slate-800 px-4 py-2 text-sm font-medium text-slate-700 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-700 disabled:opacity-50"
        >
          Next
        </button>
      </div>
      <div class="hidden sm:flex sm:flex-1 sm:items-center sm:justify-between">
        <div>
          <p class="text-sm text-slate-700 dark:text-slate-300">
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
              class="relative inline-flex items-center rounded-l-md px-2 py-2 text-slate-400 dark:text-slate-500 ring-1 ring-inset ring-slate-300 dark:ring-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 focus:z-20 focus:outline-offset-0 disabled:opacity-50"
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
                  ? 'z-10 bg-blue-600 text-white focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-600'
                  : 'text-slate-900 dark:text-slate-100 ring-1 ring-inset ring-slate-300 dark:ring-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700'
              ]"
            >
              {{ page }}
            </button>
            
            <button
              @click="nextPage"
              :disabled="currentPage === totalPages"
              class="relative inline-flex items-center rounded-r-md px-2 py-2 text-slate-400 dark:text-slate-500 ring-1 ring-inset ring-slate-300 dark:ring-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 focus:z-20 focus:outline-offset-0 disabled:opacity-50"
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
        class="fixed bottom-6 left-1/2 transform -translate-x-1/2 bg-white dark:bg-slate-800 rounded-lg shadow-lg border border-slate-200 dark:border-slate-700 px-6 py-4 flex items-center space-x-4 z-50"
      >
        <span class="text-sm text-slate-700 dark:text-slate-300">
          {{ selectedContacts.length }} selected
        </span>
        <div class="flex space-x-2">
          <button
            @click="bulkEdit"
            class="inline-flex items-center rounded-md bg-white dark:bg-slate-800 px-3 py-2 text-sm font-semibold text-slate-900 dark:text-slate-100 shadow-sm ring-1 ring-inset ring-slate-300 dark:ring-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700"
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
            class="inline-flex items-center rounded-md bg-slate-600 px-3 py-2 text-sm font-semibold text-white shadow-sm hover:bg-slate-500"
          >
            <XMarkIcon class="w-4 h-4 mr-2" />
            Clear
          </button>
        </div>
      </div>
    </Transition>
    
    </div> <!-- End Page Container -->

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

    <!-- Services Modal -->
    <ServicesModal
      v-if="showServicesModal"
      :enterprise-name="currentEnterprise?.name || 'Business'"
      @close="showServicesModal = false"
      @service-booked="handleServiceBooked"
    />
  </div> <!-- End Main Container -->
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useEnterpriseConfig } from '../composables/useEnterpriseConfig'
import {
  PlusIcon,
  MagnifyingGlassIcon,
  ArrowDownTrayIcon,
  ArrowUpTrayIcon,
  CogIcon,
  WrenchScrewdriverIcon,
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
const showServicesModal = ref(false)

// Enterprise Configuration
const {
  currentEnterprise,
  loadEnterpriseFromStorage,
  getContactTypesForEnterprise,
  getServiceOfferingsForEnterprise
} = useEnterpriseConfig()

// Load enterprise config on mount
onMounted(() => {
  loadEnterpriseFromStorage()
})

// Enterprise-specific computed properties
const enterpriseTitle = computed(() => {
  if (!currentEnterprise.value) return 'Business'
  return currentEnterprise.value.name
})

const enterpriseSubtitle = computed(() => {
  if (!currentEnterprise.value) return 'Manage your business contacts and customer relationships'
  
  const descriptions: Record<string, string> = {
    beauty_salon: 'Manage your salon clients, appointments, and beauty service customers',
    plumbing_service: 'Track your plumbing service customers, emergency calls, and maintenance contracts',
    spaza_shop: 'Manage your shop customers, credit accounts, and regular shoppers'
  }
  
  return descriptions[currentEnterprise.value.id] || `Manage your ${currentEnterprise.value.type.toLowerCase()} customers and relationships`
})

const getContactTypeLabel = () => {
  if (!currentEnterprise.value) return 'Contact'
  
  const labels: Record<string, string> = {
    beauty_salon: 'Client',
    plumbing_service: 'Customer',
    spaza_shop: 'Customer'
  }
  
  return labels[currentEnterprise.value.id] || 'Contact'
}

const handleEnterpriseChange = (enterpriseType: string) => {
  // Reload contacts when enterprise changes
  loadContacts()
}

const handleServiceBooked = (service: any) => {
  console.log('Service booked:', service)
  showServicesModal.value = false
  // You could create a new lead/opportunity here
}

// Available contact types based on enterprise
const availableContactTypes = computed(() => {
  return getContactTypesForEnterprise.value || [
    { id: 'lead', name: 'Lead' },
    { id: 'customer', name: 'Customer' },
    { id: 'prospect', name: 'Prospect' }
  ]
})

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
    const response = await contactsApi.getContacts()
    // The API returns a wrapper object with data, totalCount, etc.
    allContacts.value = response.data.map((contact: any) => ({
      ...contact,
      name: `${contact.firstName} ${contact.lastName}`.trim() // Computed name from firstName + lastName
    }))
    totalContacts.value = response.totalCount
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

const totalContacts = ref(0)
const totalPages = computed(() => Math.ceil(totalContacts.value / pageSize.value))

const paginatedContacts = computed(() => {
  // Since the API handles pagination server-side, just return the contacts
  return allContacts.value
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
    Lead: 'bg-yellow-50 text-yellow-800 ring-yellow-600/20',
    Customer: 'bg-green-50 text-green-800 ring-green-600/20',
    Prospect: 'bg-blue-50 text-blue-800 ring-blue-600/20'
  }
  return colors[type as keyof typeof colors] || 'bg-gray-50 text-gray-800 ring-gray-600/20'
}

const getStatusColor = (status: string): string => {
  const colors = {
    Active: 'bg-green-50 text-green-800 ring-green-600/20',
    Inactive: 'bg-gray-50 text-gray-800 ring-gray-600/20'
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

const deleteContact = async (contact: Contact) => {
  if (confirm(`Are you sure you want to delete ${contact.name || `${contact.firstName} ${contact.lastName}`}?`)) {
    try {
      await contactsApi.deleteContact(contact.id)
      // Reload contacts from API to get updated data
      await loadContacts()
      // Remove from selection if selected
      selectedContacts.value = selectedContacts.value.filter(id => id !== contact.id)
    } catch (error) {
      console.error('Error deleting contact:', error)
      // Could add a toast notification here
    }
  }
}

const saveContact = async (contact: Contact) => {
  try {
    if (isEditMode.value && contact.id) {
      // Update existing contact via API
      await contactsApi.updateContact(contact.id, contact)
    } else {
      // Create new contact via API
      await contactsApi.createContact(contact)
    }
    
    // Reload contacts from API to get updated data
    await loadContacts()
    
    closeContactModal()
  } catch (error) {
    console.error('Error saving contact:', error)
    // Could add a toast notification here
  }
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
      contact.name || `${contact.firstName} ${contact.lastName}`,
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

const importContacts = () => {
  // Create file input
  const input = document.createElement('input')
  input.type = 'file'
  input.accept = '.csv,.xlsx,.json'
  input.onchange = (event) => {
    const file = (event.target as HTMLInputElement).files?.[0]
    if (file) {
      // TODO: Implement file parsing and contact import
      console.log('Importing file:', file.name)
      alert('Import functionality will be implemented soon!')
    }
  }
  input.click()
}

// Watchers
watch([searchQuery, selectedType, selectedStatus, currentPage, sortField, sortDirection], () => {
  loadContacts()
})

watch(selectedContacts, (newSelection) => {
  selectAll.value = newSelection.length === paginatedContacts.value.length && newSelection.length > 0
})
</script>
