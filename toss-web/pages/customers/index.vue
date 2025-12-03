<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useCrmStore, type Customer } from '~/stores/crm'
import CustomerModal from '~/components/crm/CustomerModal.vue'

useHead({
  title: 'Customers - TOSS'
})

const crmStore = useCrmStore()
const searchQuery = ref('')
const showAddModal = ref(false)
const showEditModal = ref(false)
const selectedCustomer = ref<Customer | null>(null)
const selectedFilter = ref('all')

// Computed
const filteredCustomers = computed(() => {
  let filtered = crmStore.customers

  if (selectedFilter.value === 'active') {
    filtered = filtered.filter(c => c.status === 'active')
  } else if (selectedFilter.value === 'credit') {
    filtered = filtered.filter(c => c.outstandingAmount > 0)
  } else if (selectedFilter.value === 'vip') {
    filtered = filtered.filter(c => c.tags.includes('vip'))
  }

  if (searchQuery.value) {
    filtered = crmStore.searchCustomers(searchQuery.value)
  }

  return filtered
})

// Load data
onMounted(() => {
  crmStore.fetchCustomers()
})

// Methods
function formatCurrency(amount: number) {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR'
  }).format(amount)
}

function formatDate(date?: Date) {
  if (!date) return 'Never'
  return new Date(date).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

function handleView(customer: Customer) {
  navigateTo(`/customers/${customer.id}`)
}

function handleEdit(customer: Customer) {
  selectedCustomer.value = customer
  showEditModal.value = true
}

function handleCustomerSaved() {
  showAddModal.value = false
  showEditModal.value = false
  selectedCustomer.value = null
  crmStore.fetchCustomers()
}

function getStatusColor(status: string) {
  return status === 'active' 
    ? 'text-green-600 bg-green-100' 
    : 'text-gray-600 bg-gray-100'
}
</script>

<template>
  <div class="py-6">
    <!-- Page Header -->
    <div class="mb-8 flex items-center justify-between">
      <div>
        <h3 class="text-3xl font-bold text-gray-900 mb-2">Customers</h3>
        <p class="text-gray-600 text-sm">
          Manage your customer relationships and track credit
        </p>
      </div>
      <button
        @click="showAddModal = true"
        class="flex items-center gap-2 px-4 py-2 bg-gradient-to-br from-gray-800 to-gray-900 text-white rounded-lg hover:shadow-lg transition-shadow"
      >
        <i class="material-symbols-rounded text-xl">person_add</i>
        <span>Add Customer</span>
      </button>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Total Customers</p>
            <h4 class="text-2xl font-bold text-gray-900">{{ crmStore.customers.length }}</h4>
          </div>
          <div class="w-12 h-12 rounded-lg bg-gradient-to-br from-gray-800 to-gray-900 flex items-center justify-center text-white">
            <i class="material-symbols-rounded text-2xl">group</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Active</p>
            <h4 class="text-2xl font-bold text-gray-900">{{ crmStore.activeCustomers.length }}</h4>
          </div>
          <div class="w-12 h-12 rounded-lg bg-gradient-to-br from-green-500 to-green-600 flex items-center justify-center text-white">
            <i class="material-symbols-rounded text-2xl">check_circle</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">With Credit</p>
            <h4 class="text-2xl font-bold text-gray-900">{{ crmStore.customersWithCredit.length }}</h4>
          </div>
          <div class="w-12 h-12 rounded-lg bg-gradient-to-br from-orange-500 to-orange-600 flex items-center justify-center text-white">
            <i class="material-symbols-rounded text-2xl">credit_card</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Outstanding</p>
            <h4 class="text-2xl font-bold text-gray-900">{{ formatCurrency(crmStore.totalOutstanding) }}</h4>
          </div>
          <div class="w-12 h-12 rounded-lg bg-gradient-to-br from-red-500 to-red-600 flex items-center justify-center text-white">
            <i class="material-symbols-rounded text-2xl">account_balance_wallet</i>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters and Search -->
    <div class="bg-white rounded-xl shadow-sm p-6 mb-6">
      <div class="flex flex-col md:flex-row gap-4">
        <!-- Search -->
        <div class="flex-1">
          <div class="relative">
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search by name, phone, or email..."
              class="w-full px-4 py-2 pr-10 border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500"
            >
            <i class="material-symbols-rounded absolute right-3 top-1/2 -translate-y-1/2 text-gray-400">search</i>
          </div>
        </div>

        <!-- Filter Buttons -->
        <div class="flex gap-2">
          <button
            @click="selectedFilter = 'all'"
            :class="[
              'px-4 py-2 rounded-lg transition-colors',
              selectedFilter === 'all'
                ? 'bg-gray-900 text-white'
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            ]"
          >
            All
          </button>
          <button
            @click="selectedFilter = 'active'"
            :class="[
              'px-4 py-2 rounded-lg transition-colors',
              selectedFilter === 'active'
                ? 'bg-gray-900 text-white'
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            ]"
          >
            Active
          </button>
          <button
            @click="selectedFilter = 'credit'"
            :class="[
              'px-4 py-2 rounded-lg transition-colors',
              selectedFilter === 'credit'
                ? 'bg-gray-900 text-white'
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            ]"
          >
            With Credit
          </button>
          <button
            @click="selectedFilter = 'vip'"
            :class="[
              'px-4 py-2 rounded-lg transition-colors',
              selectedFilter === 'vip'
                ? 'bg-gray-900 text-white'
                : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
            ]"
          >
            VIP
          </button>
        </div>
      </div>
    </div>

    <!-- Customers Table -->
    <div class="bg-white rounded-xl shadow-sm overflow-hidden">
      <div class="overflow-x-auto">
        <table class="min-w-full divide-y divide-gray-200">
          <thead class="bg-gray-50">
            <tr>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Customer
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Contact
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Total Purchases
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Outstanding
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Last Purchase
              </th>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                Status
              </th>
              <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                Actions
              </th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr v-for="customer in filteredCustomers" :key="customer.id" class="hover:bg-gray-50">
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="flex items-center">
                  <div class="flex-shrink-0 h-10 w-10 bg-gradient-to-br from-gray-800 to-gray-900 rounded-full flex items-center justify-center text-white font-bold">
                    {{ customer.name.charAt(0) }}
                  </div>
                  <div class="ml-4">
                    <div class="text-sm font-medium text-gray-900">{{ customer.name }}</div>
                    <div class="text-sm text-gray-500">{{ customer.customerType }}</div>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm text-gray-900">{{ customer.phone }}</div>
                <div v-if="customer.email" class="text-sm text-gray-500">{{ customer.email }}</div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ formatCurrency(customer.totalPurchases) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <div class="text-sm font-semibold" :class="customer.outstandingAmount > 0 ? 'text-red-600' : 'text-gray-900'">
                  {{ formatCurrency(customer.outstandingAmount) }}
                </div>
                <div v-if="customer.creditLimit > 0" class="text-xs text-gray-500">
                  Limit: {{ formatCurrency(customer.creditLimit) }}
                </div>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ formatDate(customer.lastPurchaseDate) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap">
                <span :class="['px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full', getStatusColor(customer.status)]">
                  {{ customer.status }}
                </span>
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                <div class="flex items-center justify-end gap-2">
                  <button
                    @click="handleView(customer)"
                    class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                    title="View"
                  >
                    <i class="material-symbols-rounded text-lg">visibility</i>
                  </button>
                  <button
                    @click="handleEdit(customer)"
                    class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                    title="Edit"
                  >
                    <i class="material-symbols-rounded text-lg">edit</i>
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Empty State -->
      <div v-if="filteredCustomers.length === 0" class="text-center py-12">
        <i class="material-symbols-rounded text-6xl text-gray-400 mb-4">group</i>
        <h3 class="text-lg font-medium text-gray-900 mb-2">No customers found</h3>
        <p class="text-gray-600 mb-4">
          {{ searchQuery ? 'Try adjusting your search' : 'Get started by adding your first customer' }}
        </p>
        <button
          v-if="!searchQuery"
          @click="showAddModal = true"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800"
        >
          <i class="material-symbols-rounded">person_add</i>
          <span>Add Customer</span>
        </button>
      </div>
    </div>

    <!-- Modals -->
    <CustomerModal
      :show="showAddModal"
      :customer="null"
      @close="showAddModal = false"
      @saved="handleCustomerSaved"
    />
    <CustomerModal
      :show="showEditModal"
      :customer="selectedCustomer"
      @close="showEditModal = false; selectedCustomer = null"
      @saved="handleCustomerSaved"
    />
  </div>
</template>

