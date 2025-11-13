<template>
  <div class="p-4 sm:p-6">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          Customer Groups
        </h1>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          Manage customer groups for targeted pricing, promotions, and credit management
        </p>
      </div>
      <div class="flex items-center gap-2">
        <button @click="showCreateModal = true" class="btn btn-primary">
          <Icon name="heroicons:plus" class="w-4 h-4 mr-2" />
          New Customer Group
        </button>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
      <div v-for="stat in stats" :key="stat.name" class="bg-white dark:bg-gray-800 shadow-sm rounded-lg p-4 flex items-start justify-between">
        <div>
          <p class="text-sm font-medium text-gray-500 dark:text-gray-400">{{ stat.name }}</p>
          <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stat.value }}</p>
        </div>
        <div class="p-2 rounded-full" :class="stat.bgColor">
          <Icon :name="stat.icon" class="w-6 h-6" :class="stat.iconColor" />
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg p-4 mb-6">
      <div class="grid grid-cols-1 sm:grid-cols-3 lg:grid-cols-5 gap-4">
        <FormKit
          type="search"
          placeholder="Search groups..."
          v-model="filters.search"
        />
        <FormKit
          type="select"
          label="Group Type"
          v-model="filters.groupType"
          :options="groupTypeOptions"
        />
        <FormKit
          type="select"
          label="Territory"
          v-model="filters.territory"
          :options="territoryOptions"
        />
        <FormKit
          type="select"
          label="Status"
          v-model="filters.status"
          :options="['All', 'Active', 'Inactive']"
        />
        <div class="flex items-end">
          <button @click="resetFilters" class="btn btn-secondary w-full">
            <Icon name="heroicons:x-mark" class="w-4 h-4 mr-2" />
            Reset
          </button>
        </div>
      </div>
    </div>

    <!-- Customer Groups Table -->
    <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
        <thead class="bg-gray-50 dark:bg-gray-700">
          <tr>
            <th class="table-header">Group Name</th>
            <th class="table-header">Type</th>
            <th class="table-header">Customers</th>
            <th class="table-header">Default Discount</th>
            <th class="table-header">Credit Limit</th>
            <th class="table-header">Territory</th>
            <th class="table-header text-center">Status</th>
            <th class="table-header text-right">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-for="group in filteredGroups" :key="group.id">
            <td class="table-cell">
              <div>
                <div class="font-medium">{{ group.name }}</div>
                <div class="text-xs text-gray-500">{{ group.description }}</div>
              </div>
            </td>
            <td class="table-cell">
              <span :class="groupTypeClasses(group.groupType)" class="px-2.5 py-0.5 text-xs font-medium rounded-full capitalize">
                {{ formatGroupType(group.groupType) }}
              </span>
            </td>
            <td class="table-cell">{{ group.totalCustomers || 0 }}</td>
            <td class="table-cell">{{ group.defaultDiscount }}%</td>
            <td class="table-cell">{{ formatCurrency(group.creditLimit) }}</td>
            <td class="table-cell">{{ group.territory || '-' }}</td>
            <td class="table-cell text-center">
              <span :class="group.isActive ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300' : 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'" class="px-2.5 py-0.5 text-xs font-medium rounded-full">
                {{ group.isActive ? 'Active' : 'Inactive' }}
              </span>
            </td>
            <td class="table-cell text-right">
              <div class="flex items-center justify-end gap-2">
                <button @click="viewGroup(group)" class="text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300">
                  <Icon name="heroicons:eye" class="w-4 h-4" />
                </button>
                <button @click="editGroup(group)" class="text-gray-600 hover:text-gray-800 dark:text-gray-400 dark:hover:text-gray-300">
                  <Icon name="heroicons:pencil" class="w-4 h-4" />
                </button>
                <button @click="deleteGroup(group)" class="text-red-600 hover:text-red-800 dark:text-red-400 dark:hover:text-red-300">
                  <Icon name="heroicons:trash" class="w-4 h-4" />
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Create/Edit Modal -->
    <div v-if="showCreateModal || editingGroup" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-20 mx-auto p-5 border w-full max-w-2xl shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">
              {{ editingGroup ? 'Edit Customer Group' : 'Create Customer Group' }}
            </h3>
            <button @click="closeModal" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
              <Icon name="heroicons:x-mark" class="w-6 h-6" />
            </button>
          </div>

          <FormKit
            type="form"
            @submit="saveGroup"
            :value="formData"
            #default="{ value }"
            :actions="false"
          >
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <FormKit
                type="text"
                name="name"
                label="Group Name"
                placeholder="e.g., VIP Customers"
                validation="required"
              />
              <FormKit
                type="select"
                name="groupType"
                label="Group Type"
                :options="groupTypeOptions.filter(opt => opt.value !== 'all')"
                validation="required"
              />
              <FormKit
                type="textarea"
                name="description"
                label="Description"
                placeholder="Brief description of this customer group"
                outer-class="sm:col-span-2"
              />
              <FormKit
                type="number"
                name="defaultDiscount"
                label="Default Discount (%)"
                min="0"
                max="100"
                step="0.1"
                value="0"
              />
              <FormKit
                type="number"
                name="creditLimit"
                label="Credit Limit (ZAR)"
                min="0"
                step="100"
                value="0"
              />
              <FormKit
                type="number"
                name="paymentTerms"
                label="Payment Terms (Days)"
                min="0"
                max="365"
                value="30"
              />
              <FormKit
                type="select"
                name="territory"
                label="Primary Territory"
                :options="territoryOptions"
              />
              <FormKit
                type="select"
                name="preferredPaymentMethod"
                label="Preferred Payment Method"
                :options="[
                  { label: 'Cash', value: 'cash' },
                  { label: 'Mobile Money', value: 'mobile_money' },
                  { label: 'Bank Transfer', value: 'bank_transfer' },
                  { label: 'Account/Credit', value: 'account' }
                ]"
                value="cash"
              />
              <FormKit
                type="select"
                name="deliveryPreference"
                label="Delivery Preference"
                :options="[
                  { label: 'Pickup Only', value: 'pickup' },
                  { label: 'Delivery Only', value: 'delivery' },
                  { label: 'Both', value: 'both' }
                ]"
                value="both"
              />
              <FormKit
                type="number"
                name="minimumOrderAmount"
                label="Minimum Order Amount (ZAR)"
                min="0"
                step="10"
                value="0"
              />
              <FormKit
                type="number"
                name="groupOrderDiscount"
                label="Group Order Discount (%)"
                help="Additional discount when multiple customers order together"
                min="0"
                max="50"
                step="0.1"
                value="0"
              />
              <FormKit
                type="checkbox"
                name="vatRegistered"
                label="VAT Registered"
              />
              <FormKit
                type="checkbox"
                name="allowGroupOrders"
                label="Allow Group Orders"
                help="Enable group buying features for this customer group"
              />
            </div>

            <div class="mt-6 flex justify-end gap-3">
              <button type="button" @click="closeModal" class="btn btn-secondary">
                Cancel
              </button>
              <FormKit type="submit" :label="editingGroup ? 'Update Group' : 'Create Group'" outer-class="!mb-0" />
            </div>
          </FormKit>
        </div>
      </div>
    </div>

    <!-- View Modal -->
    <div v-if="viewingGroup" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-10 mx-auto p-5 border w-full max-w-4xl shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">
              {{ viewingGroup.name }} Details
            </h3>
            <button @click="viewingGroup = null" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
              <Icon name="heroicons:x-mark" class="w-6 h-6" />
            </button>
          </div>

          <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <!-- Group Information -->
            <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-4">
              <h4 class="font-medium text-gray-900 dark:text-white mb-3">Group Information</h4>
              <dl class="space-y-2 text-sm">
                <div class="flex justify-between">
                  <dt class="text-gray-500 dark:text-gray-400">Type:</dt>
                  <dd class="text-gray-900 dark:text-white capitalize">{{ formatGroupType(viewingGroup.groupType) }}</dd>
                </div>
                <div class="flex justify-between">
                  <dt class="text-gray-500 dark:text-gray-400">Default Discount:</dt>
                  <dd class="text-gray-900 dark:text-white">{{ viewingGroup.defaultDiscount }}%</dd>
                </div>
                <div class="flex justify-between">
                  <dt class="text-gray-500 dark:text-gray-400">Credit Limit:</dt>
                  <dd class="text-gray-900 dark:text-white">{{ formatCurrency(viewingGroup.creditLimit) }}</dd>
                </div>
                <div class="flex justify-between">
                  <dt class="text-gray-500 dark:text-gray-400">Payment Terms:</dt>
                  <dd class="text-gray-900 dark:text-white">{{ viewingGroup.paymentTerms }} days</dd>
                </div>
                <div class="flex justify-between">
                  <dt class="text-gray-500 dark:text-gray-400">Territory:</dt>
                  <dd class="text-gray-900 dark:text-white">{{ viewingGroup.territory || '-' }}</dd>
                </div>
              </dl>
            </div>

            <!-- Statistics -->
            <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-4">
              <h4 class="font-medium text-gray-900 dark:text-white mb-3">Statistics</h4>
              <dl class="space-y-2 text-sm">
                <div class="flex justify-between">
                  <dt class="text-gray-500 dark:text-gray-400">Total Customers:</dt>
                  <dd class="text-gray-900 dark:text-white">{{ viewingGroup.totalCustomers || 0 }}</dd>
                </div>
                <div class="flex justify-between">
                  <dt class="text-gray-500 dark:text-gray-400">Average Order Value:</dt>
                  <dd class="text-gray-900 dark:text-white">{{ formatCurrency(viewingGroup.averageOrderValue || 0) }}</dd>
                </div>
                <div class="flex justify-between">
                  <dt class="text-gray-500 dark:text-gray-400">Total Revenue:</dt>
                  <dd class="text-gray-900 dark:text-white">{{ formatCurrency(viewingGroup.totalRevenue || 0) }}</dd>
                </div>
              </dl>
            </div>
          </div>

          <div class="mt-6 flex justify-end">
            <button @click="viewingGroup = null" class="btn btn-secondary">
              Close
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import type { CustomerGroup } from '~/types/sales'

useHead({
  title: 'Customer Groups Management',
})

const salesEnhanced = useSalesEnhanced()

// State
const customerGroups = ref<CustomerGroup[]>([])
const showCreateModal = ref(false)
const editingGroup = ref<CustomerGroup | null>(null)
const viewingGroup = ref<CustomerGroup | null>(null)
const loading = ref(false)

// Filters
const filters = ref({
  search: '',
  groupType: 'all',
  territory: 'all',
  status: 'All'
})

// Form data
const formData = ref({
  name: '',
  description: '',
  groupType: 'individual',
  defaultDiscount: 0,
  creditLimit: 0,
  paymentTerms: 30,
  territory: '',
  preferredPaymentMethod: 'cash',
  deliveryPreference: 'both',
  minimumOrderAmount: 0,
  vatRegistered: false,
  allowGroupOrders: false,
  groupOrderDiscount: 0
})

// Options
const groupTypeOptions = [
  { label: 'All Types', value: 'all' },
  { label: 'Individual', value: 'individual' },
  { label: 'Stokvel', value: 'stokvel' },
  { label: 'Burial Society', value: 'burial_society' },
  { label: 'Business', value: 'business' },
  { label: 'Wholesale', value: 'wholesale' }
]

const territoryOptions = [
  { label: 'All Territories', value: 'all' },
  { label: 'Soweto East', value: 'soweto-east' },
  { label: 'Soweto West', value: 'soweto-west' },
  { label: 'Alexandra', value: 'alexandra' },
  { label: 'Khayelitsha', value: 'khayelitsha' },
  { label: 'Rural Eastern Cape', value: 'rural-ec' }
]

// Computed
const stats = computed(() => [
  { 
    name: 'Total Groups', 
    value: customerGroups.value.length,
    icon: 'heroicons:user-group',
    bgColor: 'bg-blue-100',
    iconColor: 'text-blue-600'
  },
  { 
    name: 'Active Groups', 
    value: customerGroups.value.filter(g => g.isActive).length,
    icon: 'heroicons:check-circle',
    bgColor: 'bg-green-100',
    iconColor: 'text-green-600'
  },
  { 
    name: 'Avg Discount', 
    value: customerGroups.value.length > 0 ? 
      Math.round(customerGroups.value.reduce((acc, g) => acc + g.defaultDiscount, 0) / customerGroups.value.length * 10) / 10 + '%' : '0%',
    icon: 'heroicons:percent-badge',
    bgColor: 'bg-purple-100',
    iconColor: 'text-purple-600'
  },
  { 
    name: 'Total Customers', 
    value: customerGroups.value.reduce((acc, g) => acc + (g.totalCustomers || 0), 0),
    icon: 'heroicons:users',
    bgColor: 'bg-orange-100',
    iconColor: 'text-orange-600'
  }
])

const filteredGroups = computed(() => {
  return customerGroups.value.filter(group => {
    const searchMatch = filters.value.search ? 
      group.name.toLowerCase().includes(filters.value.search.toLowerCase()) ||
      group.description.toLowerCase().includes(filters.value.search.toLowerCase()) : true
    
    const typeMatch = filters.value.groupType === 'all' ? true : group.groupType === filters.value.groupType
    const territoryMatch = filters.value.territory === 'all' ? true : group.territory === filters.value.territory
    const statusMatch = filters.value.status === 'All' ? true : 
      (filters.value.status === 'Active' ? group.isActive : !group.isActive)
    
    return searchMatch && typeMatch && territoryMatch && statusMatch
  })
})

// Methods
const loadCustomerGroups = async () => {
  try {
    loading.value = true
    customerGroups.value = await salesEnhanced.getCustomerGroups()
  } catch (error) {
    console.error('Error loading customer groups:', error)
  } finally {
    loading.value = false
  }
}

const saveGroup = async (data: any) => {
  try {
    if (editingGroup.value) {
      await salesEnhanced.updateCustomerGroup({ ...data, id: editingGroup.value.id })
    } else {
      await salesEnhanced.createCustomerGroup(data)
    }
    await loadCustomerGroups()
    closeModal()
  } catch (error) {
    console.error('Error saving group:', error)
  }
}

const editGroup = (group: CustomerGroup) => {
  editingGroup.value = group
  formData.value = { ...group }
}

const viewGroup = (group: CustomerGroup) => {
  viewingGroup.value = group
}

const deleteGroup = async (group: CustomerGroup) => {
  if (confirm(`Are you sure you want to delete "${group.name}"?`)) {
    try {
      await salesEnhanced.deleteCustomerGroup(group.id)
      await loadCustomerGroups()
    } catch (error) {
      console.error('Error deleting group:', error)
    }
  }
}

const closeModal = () => {
  showCreateModal.value = false
  editingGroup.value = null
  formData.value = {
    name: '',
    description: '',
    groupType: 'individual',
    defaultDiscount: 0,
    creditLimit: 0,
    paymentTerms: 30,
    territory: '',
    preferredPaymentMethod: 'cash',
    deliveryPreference: 'both',
    minimumOrderAmount: 0,
    vatRegistered: false,
    allowGroupOrders: false,
    groupOrderDiscount: 0
  }
}

const resetFilters = () => {
  filters.value = {
    search: '',
    groupType: 'all',
    territory: 'all',
    status: 'All'
  }
}

// Helper functions
const formatGroupType = (type: string) => {
  return type.replace(/_/g, ' ')
}

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(value || 0)
}

const groupTypeClasses = (type: string) => {
  const classes = {
    individual: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300',
    stokvel: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300',
    burial_society: 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-300',
    business: 'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-300',
    wholesale: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300'
  }
  return classes[type as keyof typeof classes] || classes.individual
}

// Initialize
onMounted(() => {
  loadCustomerGroups()
})
</script>

<style scoped>
.table-header {
  @apply px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider dark:text-gray-300;
}
.table-cell {
  @apply px-6 py-4 whitespace-nowrap text-sm text-gray-600 dark:text-gray-300;
}
.btn {
  @apply inline-flex items-center justify-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-offset-2;
}
.btn-primary {
  @apply text-white bg-blue-600 hover:bg-blue-700 focus:ring-blue-500;
}
.btn-secondary {
  @apply text-gray-700 bg-gray-100 hover:bg-gray-200 dark:bg-gray-700 dark:text-gray-200 dark:hover:bg-gray-600 focus:ring-gray-500;
}
</style>