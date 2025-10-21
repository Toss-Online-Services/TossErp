<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-purple-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header with Glass Morphism -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 sm:py-6">
        <div class="flex items-center justify-between">
          <div class="flex-1 min-w-0">
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-purple-600 to-blue-600 bg-clip-text text-transparent">
              Suppliers
            </h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
              Manage your supplier network and relationships
            </p>
          </div>
          <div class="flex space-x-2 sm:space-x-3 flex-shrink-0">
            <button
              @click="showAddModal = true"
              class="inline-flex items-center justify-center px-4 sm:px-6 py-2.5 sm:py-3 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-xl hover:from-purple-700 hover:to-blue-700 shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 font-semibold text-sm sm:text-base"
            >
              <PlusIcon class="w-5 h-5 mr-2" />
              Add Supplier
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      
      <!-- Stats Cards -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Total Suppliers</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ stats.totalSuppliers }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-purple-500 to-blue-600 rounded-xl">
              <UserGroupIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Active</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ stats.activeSuppliers }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl">
              <CheckCircleIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Avg Rating</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ stats.avgRating }}/5</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-yellow-500 to-orange-600 rounded-xl">
              <StarIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">On-Time Rate</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ stats.onTimeDelivery }}%</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-blue-500 to-cyan-600 rounded-xl">
              <TruckIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>
      </div>

      <!-- Filters and Search -->
      <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 mb-6">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <!-- Search -->
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
              <MagnifyingGlassIcon class="h-5 w-5 text-slate-400" />
            </div>
            <input
              type="text"
              v-model="searchQuery"
              placeholder="Search suppliers..."
              class="w-full pl-11 pr-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white transition-all duration-200"
            />
          </div>

          <!-- Status Filter -->
          <select
            v-model="statusFilter"
            class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white transition-all duration-200"
          >
            <option value="">All Status</option>
            <option value="active">Active</option>
            <option value="inactive">Inactive</option>
          </select>

          <!-- Actions -->
          <div class="flex space-x-2">
            <button
              @click="exportSuppliers"
              class="flex-1 inline-flex items-center justify-center px-4 py-2.5 border-2 border-slate-300 dark:border-slate-600 rounded-xl text-sm font-medium text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-700 hover:bg-slate-50 dark:hover:bg-slate-600 hover:border-slate-400 dark:hover:border-slate-500 transition-all duration-200"
            >
              <ArrowDownTrayIcon class="w-4 h-4 mr-2" />
              Export
            </button>
          </div>
        </div>
      </div>

      <!-- Suppliers Grid -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div v-if="loading" v-for="n in 6" :key="n" class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 animate-pulse">
          <div class="flex items-center mb-4">
            <div class="h-12 w-12 bg-slate-200 dark:bg-slate-700 rounded-xl"></div>
            <div class="ml-4 flex-1">
              <div class="h-5 bg-slate-200 dark:bg-slate-700 rounded w-32 mb-2"></div>
              <div class="h-4 bg-slate-200 dark:bg-slate-700 rounded w-24"></div>
            </div>
          </div>
        </div>

        <div v-else v-for="supplier in filteredSuppliers" :key="supplier.id"
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 overflow-hidden hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1"
        >
          <!-- Supplier Header -->
          <div class="bg-gradient-to-r from-purple-50 to-blue-50 dark:from-purple-900/20 dark:to-blue-900/20 px-6 py-4 border-b border-slate-200 dark:border-slate-600">
            <div class="flex items-center justify-between">
              <div class="flex items-center space-x-4">
                <div class="flex-shrink-0 h-12 w-12">
                  <div class="h-12 w-12 rounded-xl bg-gradient-to-br from-purple-500 to-blue-600 flex items-center justify-center">
                    <span class="text-xl font-bold text-white">{{ supplier.name.charAt(0) }}</span>
                  </div>
                </div>
                <div>
                  <h3 class="text-lg font-bold text-slate-900 dark:text-white">{{ supplier.name }}</h3>
                  <div class="flex items-center mt-1">
                    <StarIcon v-for="i in 5" :key="i" class="w-4 h-4" :class="i <= supplier.rating ? 'text-yellow-400' : 'text-slate-300 dark:text-slate-600'" />
                    <span class="ml-1 text-xs text-slate-600 dark:text-slate-400">({{ supplier.rating }})</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Supplier Details -->
          <div class="px-6 py-4">
            <div class="space-y-3 mb-4">
              <div class="flex items-center text-sm">
                <EnvelopeIcon class="w-4 h-4 text-slate-400 mr-2" />
                <span class="text-slate-600 dark:text-slate-400">{{ supplier.email }}</span>
              </div>
              <div class="flex items-center text-sm">
                <PhoneIcon class="w-4 h-4 text-slate-400 mr-2" />
                <span class="text-slate-600 dark:text-slate-400">{{ supplier.phone }}</span>
              </div>
              <div class="flex items-center text-sm">
                <MapPinIcon class="w-4 h-4 text-slate-400 mr-2" />
                <span class="text-slate-600 dark:text-slate-400">{{ supplier.location }}</span>
              </div>
            </div>

            <div class="grid grid-cols-2 gap-3 mb-4 pt-4 border-t border-slate-200 dark:border-slate-700">
              <div>
                <p class="text-xs text-slate-500 dark:text-slate-500">Total Orders</p>
                <p class="text-lg font-bold text-slate-900 dark:text-white">{{ supplier.totalOrders }}</p>
              </div>
              <div>
                <p class="text-xs text-slate-500 dark:text-slate-500">On-Time Rate</p>
                <p class="text-lg font-bold text-green-600">{{ supplier.onTimeRate }}%</p>
              </div>
            </div>

            <!-- Status Badge -->
            <div class="mb-4">
              <span 
                class="px-3 py-1 rounded-full text-xs font-medium"
                :class="supplier.status === 'active' ? 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400' : 'bg-slate-100 text-slate-800 dark:bg-slate-900/30 dark:text-slate-400'"
              >
                {{ supplier.status === 'active' ? 'Active' : 'Inactive' }}
              </span>
            </div>

            <!-- Actions -->
            <div class="flex space-x-2">
              <button 
                @click="viewSupplier(supplier)"
                class="flex-1 px-4 py-2 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-lg hover:from-purple-700 hover:to-blue-700 shadow-md hover:shadow-lg transition-all duration-200 text-sm font-medium"
              >
                View Details
              </button>
              <button 
                @click="contactSupplier(supplier)"
                class="px-4 py-2 border-2 border-slate-300 dark:border-slate-600 rounded-lg text-sm font-medium text-slate-700 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-700 transition-all duration-200"
              >
                Contact
              </button>
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div v-if="filteredSuppliers.length === 0 && !loading" class="col-span-full bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-12 text-center">
          <div class="flex flex-col items-center justify-center">
            <div class="p-4 bg-gradient-to-br from-purple-100 to-blue-100 dark:from-purple-900/20 dark:to-blue-900/20 rounded-full mb-4">
              <UserGroupIcon class="w-12 h-12 text-purple-600 dark:text-purple-400" />
            </div>
            <p class="text-lg font-semibold text-slate-900 dark:text-white mb-2">No suppliers found</p>
            <p class="text-slate-600 dark:text-slate-400 mb-4">Start by adding your first supplier!</p>
            <button
              @click="showAddModal = true"
              class="inline-flex items-center px-6 py-3 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-xl hover:from-purple-700 hover:to-blue-700 shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 font-semibold"
            >
              <PlusIcon class="w-5 h-5 mr-2" />
              Add Supplier
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Add Supplier Modal (Placeholder) -->
    <div v-if="showAddModal" class="fixed inset-0 bg-slate-600 bg-opacity-50 overflow-y-auto h-full w-full z-50 flex items-center justify-center p-4">
      <div class="relative bg-white dark:bg-slate-800 rounded-2xl shadow-2xl border border-slate-200 dark:border-slate-700 w-full max-w-2xl p-6">
        <div class="flex items-center justify-between mb-6">
          <h3 class="text-xl font-bold bg-gradient-to-r from-purple-600 to-blue-600 bg-clip-text text-transparent">Add New Supplier</h3>
          <button @click="showAddModal = false" class="text-slate-400 hover:text-slate-600 dark:hover:text-slate-200">
            <XMarkIcon class="w-6 h-6" />
          </button>
        </div>
        
        <div class="text-center py-12">
          <p class="text-slate-600 dark:text-slate-400">Supplier creation form will be implemented here.</p>
          <p class="text-sm text-slate-500 dark:text-slate-500 mt-2">For MVP, contact suppliers directly via phone or email.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import {
  PlusIcon,
  MagnifyingGlassIcon,
  ArrowDownTrayIcon,
  UserGroupIcon,
  CheckCircleIcon,
  StarIcon,
  TruckIcon,
  EnvelopeIcon,
  PhoneIcon,
  MapPinIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Suppliers - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage your supplier network and relationships in TOSS ERP' }
  ]
})

// State
const loading = ref(false)
const showAddModal = ref(false)
const searchQuery = ref('')
const statusFilter = ref('')

// Stats
const stats = ref({
  totalSuppliers: 42,
  activeSuppliers: 38,
  avgRating: 4.2,
  onTimeDelivery: 94
})

// Mock suppliers data
const suppliers = ref([
  {
    id: 1,
    name: 'ABC Suppliers',
    email: 'contact@abc-suppliers.co.za',
    phone: '+27 11 123 4567',
    location: 'Johannesburg, GP',
    rating: 5,
    totalOrders: 124,
    onTimeRate: 98,
    status: 'active'
  },
  {
    id: 2,
    name: 'XYZ Wholesalers',
    email: 'sales@xyz-wholesale.co.za',
    phone: '+27 21 234 5678',
    location: 'Cape Town, WC',
    rating: 4,
    totalOrders: 89,
    onTimeRate: 92,
    status: 'active'
  },
  {
    id: 3,
    name: 'Quality Foods Ltd',
    email: 'info@qualityfoods.co.za',
    phone: '+27 31 345 6789',
    location: 'Durban, KZN',
    rating: 5,
    totalOrders: 156,
    onTimeRate: 96,
    status: 'active'
  },
  {
    id: 4,
    name: 'Tech Solutions Inc',
    email: 'support@techsolutions.co.za',
    phone: '+27 12 456 7890',
    location: 'Pretoria, GP',
    rating: 4,
    totalOrders: 67,
    onTimeRate: 88,
    status: 'active'
  },
  {
    id: 5,
    name: 'Fresh Produce Co',
    email: 'orders@freshproduce.co.za',
    phone: '+27 11 567 8901',
    location: 'Johannesburg, GP',
    rating: 3,
    totalOrders: 45,
    onTimeRate: 85,
    status: 'inactive'
  },
  {
    id: 6,
    name: 'Industrial Supplies SA',
    email: 'sales@industrial-sa.co.za',
    phone: '+27 21 678 9012',
    location: 'Cape Town, WC',
    rating: 5,
    totalOrders: 203,
    onTimeRate: 99,
    status: 'active'
  }
])

// Computed
const filteredSuppliers = computed(() => {
  return suppliers.value.filter(supplier => {
    const matchesSearch = !searchQuery.value || 
      supplier.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      supplier.email.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      supplier.location.toLowerCase().includes(searchQuery.value.toLowerCase())
    
    const matchesStatus = !statusFilter.value || supplier.status === statusFilter.value
    
    return matchesSearch && matchesStatus
  })
})

// Methods
const viewSupplier = (supplier: any) => {
  alert(`Viewing supplier details for ${supplier.name}`)
}

const contactSupplier = (supplier: any) => {
  alert(`Contacting ${supplier.name}...\nEmail: ${supplier.email}\nPhone: ${supplier.phone}`)
}

const exportSuppliers = () => {
  alert('Exporting suppliers to CSV/Excel')
}
</script>
