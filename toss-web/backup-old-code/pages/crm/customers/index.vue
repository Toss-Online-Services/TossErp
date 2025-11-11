<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-slate-100">
    <!-- Page Header -->
    <div class="bg-white shadow-sm border-b border-slate-200 sticky top-0 z-10">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 sm:py-6">
        <div class="flex items-center justify-between">
          <div class="flex-1 min-w-0">
            <div class="flex items-center space-x-3">
              <NuxtLink to="/crm" class="text-slate-400 hover:text-slate-600">
                <ArrowLeftIcon class="w-5 h-5" />
              </NuxtLink>
              <div>
                <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-blue-600 to-green-600 bg-clip-text text-transparent">
                  Customers
                </h1>
                <p class="mt-1 text-sm text-slate-600">
                  Manage your customer database
                </p>
              </div>
            </div>
          </div>
          <div class="flex space-x-2 sm:space-x-3">
            <button
              @click="showAddCustomer = true"
              class="inline-flex items-center px-4 sm:px-6 py-2.5 bg-gradient-to-r from-blue-600 to-green-600 text-white rounded-xl hover:from-blue-700 hover:to-green-700 shadow-lg font-semibold text-sm"
            >
              <UserPlusIcon class="w-5 h-5 mr-2" />
              Add Customer
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      
      <!-- Stats Overview -->
      <div class="grid grid-cols-2 sm:grid-cols-4 gap-4 mb-8">
        <div class="bg-white rounded-xl shadow-md border border-slate-200 p-4">
          <div class="flex items-center">
            <div class="p-2 bg-blue-100 rounded-lg">
              <UsersIcon class="w-6 h-6 text-blue-600" />
            </div>
            <div class="ml-3">
              <p class="text-sm text-slate-600">Total</p>
              <p class="text-xl font-bold text-slate-900">{{ customers.length }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-md border border-slate-200 p-4">
          <div class="flex items-center">
            <div class="p-2 bg-green-100 rounded-lg">
              <CheckCircleIcon class="w-6 h-6 text-green-600" />
            </div>
            <div class="ml-3">
              <p class="text-sm text-slate-600">Active</p>
              <p class="text-xl font-bold text-slate-900">{{ activeCustomers }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-md border border-slate-200 p-4">
          <div class="flex items-center">
            <div class="p-2 bg-orange-100 rounded-lg">
              <CreditCardIcon class="w-6 h-6 text-orange-600" />
            </div>
            <div class="ml-3">
              <p class="text-sm text-slate-600">On Credit</p>
              <p class="text-xl font-bold text-slate-900">{{ creditCustomers }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-md border border-slate-200 p-4">
          <div class="flex items-center">
            <div class="p-2 bg-purple-100 rounded-lg">
              <StarIcon class="w-6 h-6 text-purple-600" />
            </div>
            <div class="ml-3">
              <p class="text-sm text-slate-600">VIP</p>
              <p class="text-xl font-bold text-slate-900">{{ vipCustomers }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Filters and Search -->
      <div class="bg-white rounded-2xl shadow-lg border border-slate-200 p-6 mb-6">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <!-- Search -->
          <div class="md:col-span-2">
            <div class="relative">
              <MagnifyingGlassIcon class="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-slate-400" />
              <input
                v-model="searchQuery"
                type="text"
                placeholder="Search customers..."
                class="w-full pl-10 pr-4 py-2.5 bg-slate-50 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>

          <!-- Status Filter -->
          <div>
            <select
              v-model="statusFilter"
              class="w-full px-4 py-2.5 bg-slate-50 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500"
            >
              <option value="all">All Status</option>
              <option value="active">Active</option>
              <option value="inactive">Inactive</option>
            </select>
          </div>

          <!-- Type Filter -->
          <div>
            <select
              v-model="typeFilter"
              class="w-full px-4 py-2.5 bg-slate-50 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500"
            >
              <option value="all">All Types</option>
              <option value="regular">Regular</option>
              <option value="credit">Credit</option>
              <option value="vip">VIP</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Customers Table -->
      <div class="bg-white rounded-2xl shadow-lg border border-slate-200 overflow-hidden">
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-slate-200">
            <thead class="bg-slate-50">
              <tr>
                <th class="px-6 py-4 text-left text-xs font-semibold text-slate-600 uppercase">Customer</th>
                <th class="px-6 py-4 text-left text-xs font-semibold text-slate-600 uppercase">Contact</th>
                <th class="px-6 py-4 text-left text-xs font-semibold text-slate-600 uppercase">Type</th>
                <th class="px-6 py-4 text-left text-xs font-semibold text-slate-600 uppercase">Orders</th>
                <th class="px-6 py-4 text-left text-xs font-semibold text-slate-600 uppercase">Lifetime Value</th>
                <th class="px-6 py-4 text-left text-xs font-semibold text-slate-600 uppercase">Status</th>
                <th class="px-6 py-4 text-right text-xs font-semibold text-slate-600 uppercase">Actions</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-slate-200">
              <tr
                v-for="customer in filteredCustomers"
                :key="customer.id"
                class="hover:bg-slate-50"
              >
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="flex-shrink-0 h-10 w-10 bg-gradient-to-br from-blue-500 to-green-500 rounded-full flex items-center justify-center text-white font-semibold">
                      {{ customer.name.charAt(0) }}
                    </div>
                    <div class="ml-4">
                      <div class="text-sm font-semibold text-slate-900">{{ customer.name }}</div>
                      <div class="text-xs text-slate-500">ID: {{ customer.id }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm text-slate-900">{{ customer.phone }}</div>
                  <div class="text-xs text-slate-500">{{ customer.email }}</div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span
                    class="inline-flex items-center px-2.5 py-1 rounded-full text-xs font-medium"
                    :class="{
                      'bg-purple-100 text-purple-800': customer.type === 'vip',
                      'bg-orange-100 text-orange-800': customer.type === 'credit',
                      'bg-slate-100 text-slate-800': customer.type === 'regular'
                    }"
                  >
                    <StarIcon v-if="customer.type === 'vip'" class="w-3 h-3 mr-1" />
                    {{ customer.type.toUpperCase() }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-slate-900">
                  {{ customer.totalOrders }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-semibold text-slate-900">
                  R{{ customer.lifetimeValue.toLocaleString() }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span
                    class="inline-flex items-center px-2.5 py-1 rounded-full text-xs font-medium"
                    :class="{
                      'bg-green-100 text-green-800': customer.status === 'active',
                      'bg-slate-100 text-slate-800': customer.status === 'inactive'
                    }"
                  >
                    {{ customer.status === 'active' ? 'Active' : 'Inactive' }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-right text-sm">
                  <div class="flex items-center justify-end space-x-2">
                    <button
                      @click="viewCustomer(customer)"
                      class="text-blue-600 hover:text-blue-900 p-1 rounded hover:bg-blue-50"
                      title="View"
                    >
                      <EyeIcon class="w-5 h-5" />
                    </button>
                    <button
                      @click="editCustomer(customer)"
                      class="text-slate-600 hover:text-slate-900 p-1 rounded hover:bg-slate-100"
                      title="Edit"
                    >
                      <PencilIcon class="w-5 h-5" />
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>

          <!-- Empty State -->
          <div v-if="filteredCustomers.length === 0" class="text-center py-12">
            <UsersIcon class="mx-auto h-12 w-12 text-slate-400" />
            <h3 class="mt-2 text-sm font-medium text-slate-900">No customers found</h3>
            <p class="mt-1 text-sm text-slate-500">Try adjusting your filters</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Add Customer Modal -->
    <div v-if="showAddCustomer" class="fixed inset-0 z-50 overflow-y-auto">
      <div class="flex items-center justify-center min-h-screen px-4">
        <div class="fixed inset-0 bg-slate-900/75" @click="showAddCustomer = false"></div>

        <div class="relative w-full max-w-2xl bg-white shadow-2xl rounded-2xl">
          <div class="bg-gradient-to-r from-blue-600 to-green-600 px-6 py-4">
            <h3 class="text-xl font-bold text-white">Add New Customer</h3>
          </div>

          <div class="px-6 py-6 space-y-4">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 mb-1">Full Name *</label>
                <input
                  v-model="newCustomer.name"
                  type="text"
                  class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  placeholder="John Doe"
                />
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 mb-1">Phone *</label>
                <input
                  v-model="newCustomer.phone"
                  type="tel"
                  class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                  placeholder="+27 12 345 6789"
                />
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-1">Email</label>
              <input
                v-model="newCustomer.email"
                type="email"
                class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                placeholder="john@example.com"
              />
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-1">Type</label>
              <select
                v-model="newCustomer.type"
                class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              >
                <option value="regular">Regular</option>
                <option value="credit">Credit</option>
                <option value="vip">VIP</option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-slate-700 mb-1">Address</label>
              <textarea
                v-model="newCustomer.address"
                rows="3"
                class="w-full px-4 py-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                placeholder="Street address, township, city"
              ></textarea>
            </div>
          </div>

          <div class="bg-slate-50 px-6 py-4 flex justify-end space-x-3">
            <button
              @click="showAddCustomer = false"
              class="px-6 py-2.5 text-slate-700 bg-white border border-slate-300 rounded-lg hover:bg-slate-50"
            >
              Cancel
            </button>
            <button
              @click="saveCustomer"
              class="px-6 py-2.5 bg-gradient-to-r from-blue-600 to-green-600 text-white rounded-lg hover:from-blue-700 hover:to-green-700"
            >
              Add Customer
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import {
  UsersIcon,
  UserPlusIcon,
  StarIcon,
  CreditCardIcon,
  CheckCircleIcon,
  MagnifyingGlassIcon,
  EyeIcon,
  PencilIcon,
  ArrowLeftIcon
} from '@heroicons/vue/24/outline'

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'Customers - TOSS ERP'
})

const showAddCustomer = ref(false)
const searchQuery = ref('')
const statusFilter = ref('all')
const typeFilter = ref('all')

const newCustomer = ref({
  name: '',
  phone: '',
  email: '',
  type: 'regular',
  address: ''
})

const customers = ref([
  {
    id: 'CUS001',
    name: 'Thabo Molefe',
    phone: '+27 12 345 6789',
    email: 'thabo@example.com',
    type: 'vip',
    status: 'active',
    totalOrders: 45,
    lifetimeValue: 12500
  },
  {
    id: 'CUS002',
    name: 'Nomsa Dlamini',
    phone: '+27 12 345 6790',
    email: 'nomsa@example.com',
    type: 'credit',
    status: 'active',
    totalOrders: 32,
    lifetimeValue: 8900
  },
  {
    id: 'CUS003',
    name: 'Sipho Khumalo',
    phone: '+27 12 345 6791',
    email: 'sipho@example.com',
    type: 'regular',
    status: 'active',
    totalOrders: 18,
    lifetimeValue: 4200
  }
])

const activeCustomers = computed(() => {
  return customers.value.filter(c => c.status === 'active').length
})

const creditCustomers = computed(() => {
  return customers.value.filter(c => c.type === 'credit').length
})

const vipCustomers = computed(() => {
  return customers.value.filter(c => c.type === 'vip').length
})

const filteredCustomers = computed(() => {
  return customers.value.filter(customer => {
    const matchesSearch = searchQuery.value === '' ||
      customer.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      customer.phone.includes(searchQuery.value) ||
      customer.email.toLowerCase().includes(searchQuery.value.toLowerCase())

    const matchesStatus = statusFilter.value === 'all' || customer.status === statusFilter.value
    const matchesType = typeFilter.value === 'all' || customer.type === typeFilter.value

    return matchesSearch && matchesStatus && matchesType
  })
})

const saveCustomer = () => {
  if (!newCustomer.value.name || !newCustomer.value.phone) {
    alert('Please fill in required fields')
    return
  }

  const newId = `CUS${String(customers.value.length + 1).padStart(3, '0')}`
  customers.value.push({
    id: newId,
    name: newCustomer.value.name,
    phone: newCustomer.value.phone,
    email: newCustomer.value.email || '',
    type: newCustomer.value.type,
    status: 'active',
    totalOrders: 0,
    lifetimeValue: 0
  })

  newCustomer.value = {
    name: '',
    phone: '',
    email: '',
    type: 'regular',
    address: ''
  }
  showAddCustomer.value = false
}

const viewCustomer = (customer: any) => {
  navigateTo(`/crm/customers/${customer.id}`)
}

const editCustomer = (customer: any) => {
  console.log('Edit customer:', customer)
}
</script>

