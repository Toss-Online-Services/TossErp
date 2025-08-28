<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Company Setup</h1>
              <p class="text-gray-600 dark:text-gray-400">Manage company information and configurations</p>
            </div>
            <div class="flex space-x-3">
              <button @click="showCreateModal = true" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors">
                <PlusIcon class="w-5 h-5 inline mr-2" />
                New Company
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <!-- Companies Table -->
      <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg border border-gray-200 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <h2 class="text-lg font-medium text-gray-900 dark:text-white">Companies</h2>
            <div class="flex items-center space-x-4">
              <input 
                v-model="searchQuery"
                type="text" 
                placeholder="Search companies..."
                class="px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>
        </div>
        
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
            <thead class="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Company Name
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Abbreviation
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Domain
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Currency
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Country
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Status
                </th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">
                  Actions
                </th>
              </tr>
            </thead>
            <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
              <tr v-for="company in filteredCompanies" :key="company.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="flex-shrink-0 h-10 w-10">
                      <img v-if="company.logo" class="h-10 w-10 rounded-full" :src="company.logo" :alt="company.name" />
                      <div v-else class="h-10 w-10 rounded-full bg-blue-100 dark:bg-blue-900 flex items-center justify-center">
                        <span class="text-sm font-medium text-blue-600 dark:text-blue-300">{{ company.abbreviation }}</span>
                      </div>
                    </div>
                    <div class="ml-4">
                      <div class="text-sm font-medium text-gray-900 dark:text-white">{{ company.name }}</div>
                      <div class="text-sm text-gray-500 dark:text-gray-400">{{ company.description }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                  {{ company.abbreviation }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                  {{ company.domain }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                  {{ company.currency }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                  {{ company.country }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span :class="company.is_active ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'" class="px-2 py-1 text-xs font-semibold rounded-full">
                    {{ company.is_active ? 'Active' : 'Inactive' }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                  <button @click="editCompany(company)" class="text-blue-600 hover:text-blue-900 dark:text-blue-400 dark:hover:text-blue-300 mr-3">
                    Edit
                  </button>
                  <button @click="deleteCompany(company.id)" class="text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300">
                    Delete
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Create/Edit Modal -->
    <div v-if="showCreateModal || showEditModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50" @click="closeModal">
      <div class="relative top-20 mx-auto p-5 border w-11/12 md:w-3/4 lg:w-1/2 shadow-lg rounded-md bg-white dark:bg-gray-800" @click.stop>
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">
              {{ showCreateModal ? 'Create New Company' : 'Edit Company' }}
            </h3>
            <button @click="closeModal" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
              <XMarkIcon class="w-6 h-6" />
            </button>
          </div>
          
          <form @submit.prevent="saveCompany" class="space-y-4">
            <!-- Basic Information -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Company Name *</label>
                <input 
                  v-model="formData.name" 
                  type="text" 
                  required
                  class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Abbreviation *</label>
                <input 
                  v-model="formData.abbreviation" 
                  type="text" 
                  required
                  class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
                />
              </div>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Domain</label>
                <select 
                  v-model="formData.domain"
                  class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
                >
                  <option value="">Select Domain</option>
                  <option value="Manufacturing">Manufacturing</option>
                  <option value="Services">Services</option>
                  <option value="Retail">Retail</option>
                  <option value="Agriculture">Agriculture</option>
                  <option value="Education">Education</option>
                  <option value="Non Profit">Non Profit</option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Default Currency *</label>
                <select 
                  v-model="formData.currency" 
                  required
                  class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
                >
                  <option value="">Select Currency</option>
                  <option value="USD">USD - US Dollar</option>
                  <option value="EUR">EUR - Euro</option>
                  <option value="GBP">GBP - British Pound</option>
                  <option value="ZAR">ZAR - South African Rand</option>
                  <option value="JPY">JPY - Japanese Yen</option>
                </select>
              </div>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Country *</label>
                <select 
                  v-model="formData.country" 
                  required
                  class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
                >
                  <option value="">Select Country</option>
                  <option value="United States">United States</option>
                  <option value="United Kingdom">United Kingdom</option>
                  <option value="South Africa">South Africa</option>
                  <option value="Germany">Germany</option>
                  <option value="France">France</option>
                  <option value="Japan">Japan</option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Tax ID</label>
                <input 
                  v-model="formData.tax_id" 
                  type="text"
                  class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
                />
              </div>
            </div>

            <!-- Parent Company -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Parent Company</label>
              <select 
                v-model="formData.parent_company"
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
              >
                <option value="">No Parent Company</option>
                <option v-for="company in companies.filter(c => c.is_group)" :key="company.id" :value="company.id">
                  {{ company.name }}
                </option>
              </select>
            </div>

            <!-- Company Type -->
            <div class="flex items-center space-x-4">
              <label class="flex items-center">
                <input 
                  v-model="formData.is_group" 
                  type="checkbox"
                  class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                />
                <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">Is Group Company</span>
              </label>
              <label class="flex items-center">
                <input 
                  v-model="formData.is_active" 
                  type="checkbox"
                  class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
                />
                <span class="ml-2 text-sm text-gray-700 dark:text-gray-300">Active</span>
              </label>
            </div>

            <!-- Description -->
            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1">Description</label>
              <textarea 
                v-model="formData.description"
                rows="3"
                class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
                placeholder="Company description..."
              ></textarea>
            </div>

            <!-- Modal Actions -->
            <div class="flex justify-end space-x-3 pt-4 border-t border-gray-200 dark:border-gray-600">
              <button 
                type="button" 
                @click="closeModal"
                class="px-4 py-2 bg-gray-300 dark:bg-gray-600 text-gray-700 dark:text-gray-300 rounded-md hover:bg-gray-400 dark:hover:bg-gray-500 transition-colors"
              >
                Cancel
              </button>
              <button 
                type="submit"
                class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors"
              >
                {{ showCreateModal ? 'Create Company' : 'Update Company' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { PlusIcon, XMarkIcon } from '@heroicons/vue/24/outline'

// Reactive data
const companies = ref([])
const searchQuery = ref('')
const showCreateModal = ref(false)
const showEditModal = ref(false)
const editingCompany = ref(null)

// Form data
const formData = ref({
  name: '',
  abbreviation: '',
  domain: '',
  currency: '',
  country: '',
  tax_id: '',
  parent_company: '',
  is_group: false,
  is_active: true,
  description: ''
})

// Computed properties
const filteredCompanies = computed(() => {
  if (!searchQuery.value) return companies.value
  return companies.value.filter(company => 
    company.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
    company.abbreviation.toLowerCase().includes(searchQuery.value.toLowerCase())
  )
})

// Methods
const fetchCompanies = async () => {
  try {
    // TODO: Replace with actual API call
    // const response = await $fetch('/api/accounting/company')
    // companies.value = response.data
    
    // Mock data for now
    companies.value = [
      {
        id: 1,
        name: 'TOSS Technologies',
        abbreviation: 'TOSS',
        domain: 'Services',
        currency: 'USD',
        country: 'United States',
        tax_id: '12-3456789',
        parent_company: null,
        is_group: true,
        is_active: true,
        description: 'Main holding company',
        logo: null
      },
      {
        id: 2,
        name: 'TOSS Manufacturing',
        abbreviation: 'TMAN',
        domain: 'Manufacturing',
        currency: 'ZAR',
        country: 'South Africa',
        tax_id: '98-7654321',
        parent_company: 1,
        is_group: false,
        is_active: true,
        description: 'Manufacturing subsidiary',
        logo: null
      }
    ]
  } catch (error) {
    console.error('Error fetching companies:', error)
  }
}

const editCompany = (company) => {
  editingCompany.value = company
  formData.value = { ...company }
  showEditModal.value = true
}

const deleteCompany = async (id) => {
  if (confirm('Are you sure you want to delete this company?')) {
    try {
      // TODO: Replace with actual API call
      // await $fetch(`/api/accounting/company/${id}`, { method: 'DELETE' })
      companies.value = companies.value.filter(c => c.id !== id)
    } catch (error) {
      console.error('Error deleting company:', error)
    }
  }
}

const saveCompany = async () => {
  try {
    if (showCreateModal.value) {
      // TODO: Replace with actual API call
      // const response = await $fetch('/api/accounting/company', {
      //   method: 'POST',
      //   body: formData.value
      // })
      
      // Mock creation
      const newCompany = {
        id: companies.value.length + 1,
        ...formData.value
      }
      companies.value.push(newCompany)
    } else {
      // TODO: Replace with actual API call
      // await $fetch(`/api/accounting/company/${editingCompany.value.id}`, {
      //   method: 'PUT',
      //   body: formData.value
      // })
      
      // Mock update
      const index = companies.value.findIndex(c => c.id === editingCompany.value.id)
      if (index !== -1) {
        companies.value[index] = { ...formData.value, id: editingCompany.value.id }
      }
    }
    
    closeModal()
  } catch (error) {
    console.error('Error saving company:', error)
  }
}

const closeModal = () => {
  showCreateModal.value = false
  showEditModal.value = false
  editingCompany.value = null
  formData.value = {
    name: '',
    abbreviation: '',
    domain: '',
    currency: '',
    country: '',
    tax_id: '',
    parent_company: '',
    is_group: false,
    is_active: true,
    description: ''
  }
}

// Lifecycle
onMounted(() => {
  fetchCompanies()
})
</script>
