<template>
  <div class="p-6">
    <div class="flex items-center justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          Customer Management
        </h1>
        <p class="text-gray-600 dark:text-gray-400">
          Manage SMME customers, spaza shops, and township businesses
        </p>
      </div>
      <button class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg">
        <Icon name="heroicons:plus" class="w-4 h-4 mr-2" />
        Add Customer
      </button>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-blue-100 rounded-full">
            <Icon name="heroicons:users" class="w-5 h-5 text-blue-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Total Customers</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">1,247</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-green-100 rounded-full">
            <Icon name="heroicons:chart-bar" class="w-5 h-5 text-green-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Active This Month</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">892</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-yellow-100 rounded-full">
            <Icon name="heroicons:star" class="w-5 h-5 text-yellow-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">VIP Customers</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">45</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-purple-100 rounded-full">
            <Icon name="heroicons:banknotes" class="w-5 h-5 text-purple-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Avg. Order Value</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">R 1,245</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm p-4 mb-6">
      <div class="flex flex-wrap gap-4">
        <div class="flex-1 min-w-[200px]">
          <input 
            type="text" 
            placeholder="Search customers..."
            class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>
        <select class="px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500">
          <option>All Groups</option>
          <option>Spaza Shops</option>
          <option>Taverns</option>
          <option>Stokvels</option>
          <option>VIP Customers</option>
        </select>
        <select class="px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500">
          <option>All Areas</option>
          <option>Soweto</option>
          <option>Alexandra</option>
          <option>Diepkloof</option>
          <option>Orange Farm</option>
        </select>
      </div>
    </div>

    <!-- Customer List -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm">
      <div class="p-6">
        <h2 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
          Customer Directory
        </h2>
        
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead>
              <tr class="border-b border-gray-200 dark:border-gray-700">
                <th class="text-left py-3 px-4 font-medium text-gray-700 dark:text-gray-300">Customer</th>
                <th class="text-left py-3 px-4 font-medium text-gray-700 dark:text-gray-300">Type</th>
                <th class="text-left py-3 px-4 font-medium text-gray-700 dark:text-gray-300">Location</th>
                <th class="text-left py-3 px-4 font-medium text-gray-700 dark:text-gray-300">Last Order</th>
                <th class="text-left py-3 px-4 font-medium text-gray-700 dark:text-gray-300">Total Orders</th>
                <th class="text-left py-3 px-4 font-medium text-gray-700 dark:text-gray-300">Status</th>
                <th class="text-left py-3 px-4 font-medium text-gray-700 dark:text-gray-300">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="customer in mockCustomers" :key="customer.id" 
                  class="border-b border-gray-200 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-700">
                <td class="py-3 px-4">
                  <div class="flex items-center">
                    <div class="w-8 h-8 bg-blue-100 rounded-full flex items-center justify-center mr-3">
                      <span class="text-blue-600 font-medium text-sm">
                        {{ customer.name.charAt(0) }}
                      </span>
                    </div>
                    <div>
                      <div class="font-medium text-gray-900 dark:text-white">{{ customer.name }}</div>
                      <div class="text-sm text-gray-500">{{ customer.phone }}</div>
                    </div>
                  </div>
                </td>
                <td class="py-3 px-4">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getCustomerTypeClass(customer.type)">
                    {{ formatCustomerType(customer.type) }}
                  </span>
                </td>
                <td class="py-3 px-4 text-sm text-gray-600 dark:text-gray-400">
                  {{ customer.location }}
                </td>
                <td class="py-3 px-4 text-sm text-gray-600 dark:text-gray-400">
                  {{ formatDate(customer.lastOrder) }}
                </td>
                <td class="py-3 px-4 text-sm text-gray-600 dark:text-gray-400">
                  {{ customer.totalOrders }}
                </td>
                <td class="py-3 px-4">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="customer.isActive ? 
                          'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200' : 
                          'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'">
                    {{ customer.isActive ? 'Active' : 'Inactive' }}
                  </span>
                </td>
                <td class="py-3 px-4">
                  <div class="flex items-center space-x-2">
                    <button class="text-blue-600 hover:text-blue-800 text-sm">
                      View
                    </button>
                    <button class="text-green-600 hover:text-green-800 text-sm">
                      Edit
                    </button>
                    <button class="text-purple-600 hover:text-purple-800 text-sm">
                      Orders
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

useHead({
  title: 'Customers - TOSS ERP'
})

// Mock data for demonstration
const mockCustomers = ref([
  {
    id: 'cust-001',
    name: 'Sipho\'s Spaza Shop',
    phone: '+27 82 123 4567',
    type: 'spaza-shop',
    location: 'Soweto, Orlando East',
    lastOrder: new Date('2025-01-15'),
    totalOrders: 145,
    isActive: true,
    totalSpent: 45600
  },
  {
    id: 'cust-002',
    name: 'Mama Nomsa\'s Tavern',
    phone: '+27 73 987 6543',
    type: 'tavern',
    location: 'Alexandra, East Bank',
    lastOrder: new Date('2025-01-14'),
    totalOrders: 89,
    isActive: true,
    totalSpent: 32100
  },
  {
    id: 'cust-003',
    name: 'Diepkloof Stokvel',
    phone: '+27 84 555 7890',
    type: 'stokvel',
    location: 'Diepkloof, Zone 3',
    lastOrder: new Date('2025-01-10'),
    totalOrders: 12,
    isActive: true,
    totalSpent: 67800
  },
  {
    id: 'cust-004',
    name: 'Thabo\'s Corner Store',
    phone: '+27 71 222 3333',
    type: 'spaza-shop',
    location: 'Orange Farm, Extensions',
    lastOrder: new Date('2025-01-08'),
    totalOrders: 67,
    isActive: true,
    totalSpent: 28400
  },
  {
    id: 'cust-005',
    name: 'Katlego General Store',
    phone: '+27 76 444 5555',
    type: 'general-store',
    location: 'Tembisa, Unit 9',
    lastOrder: new Date('2024-12-28'),
    totalOrders: 203,
    isActive: false,
    totalSpent: 89200
  }
])

const formatCustomerType = (type: string) => {
  const typeMap: Record<string, string> = {
    'spaza-shop': 'Spaza Shop',
    'tavern': 'Tavern',
    'stokvel': 'Stokvel',
    'general-store': 'General Store',
    'burial-society': 'Burial Society'
  }
  return typeMap[type] || type
}

const getCustomerTypeClass = (type: string) => {
  const classMap: Record<string, string> = {
    'spaza-shop': 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    'tavern': 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200',
    'stokvel': 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    'general-store': 'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-200',
    'burial-society': 'bg-indigo-100 text-indigo-800 dark:bg-indigo-900 dark:text-indigo-200'
  }
  return classMap[type] || 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
}

const formatDate = (date: Date) => {
  return new Date(date).toLocaleDateString('en-ZA')
}
</script>