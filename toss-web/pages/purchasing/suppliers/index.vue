<script setup lang="ts">
/**
 * Purchasing - Suppliers Management
 * ERPNext Supplier DocType equivalent for TOSS
 */
import { ref } from 'vue'
import { Building2, Phone, Mail, MapPin, Plus, Search } from 'lucide-vue-next'

useHead({
  title: 'Suppliers - TOSS ERP',
  meta: [{ name: 'description', content: 'Manage your suppliers' }]
})

definePageMeta({
  layout: 'dashboard',
  middleware: ['auth']
})

const suppliers = ref([
  { id: 1, name: 'Mthembu Wholesalers', contact: 'Mr. Thabo Mthembu', phone: '+27 82 345 6789', email: 'thabo@mthembu.co.za', location: 'Soweto', totalOrders: 45, totalSpent: 125600, status: 'Active', rating: 4.8 },
  { id: 2, name: 'Dlamini Suppliers', contact: 'Mrs. Nomsa Dlamini', phone: '+27 81 234 5678', email: 'nomsa@dlamini.co.za', location: 'Alexandra', totalOrders: 32, totalSpent: 89400, status: 'Active', rating: 4.5 },
  { id: 3, name: 'Ubuntu Fresh Produce', contact: 'Mr. Sipho Khumalo', phone: '+27 83 456 7890', email: 'sipho@ubuntu.co.za', location: 'Tembisa', totalOrders: 28, totalSpent: 67200, status: 'Active', rating: 4.6 }
])

const searchQuery = ref('')
</script>

<template>
  <div class="p-4 md:p-6 space-y-6">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
      <div>
        <h1 class="text-2xl md:text-3xl font-bold text-gray-900 dark:text-white">Suppliers</h1>
        <p class="text-gray-600 dark:text-gray-400 mt-1">Manage your supplier relationships</p>
      </div>
      <button class="btn btn-primary flex items-center justify-center gap-2">
        <Plus :size="20" />
        <span>Add Supplier</span>
      </button>
    </div>

    <!-- Search and Filter -->
    <div class="flex flex-col sm:flex-row gap-3">
      <div class="flex-1 relative">
        <Search class="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
        <input
          v-model="searchQuery"
          type="search"
          placeholder="Search suppliers..."
          class="input w-full pl-10"
        />
      </div>
      <select class="input sm:w-48">
        <option>All Suppliers</option>
        <option>Active</option>
        <option>Inactive</option>
      </select>
    </div>

    <!-- Suppliers List -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-4">
      <div v-for="supplier in suppliers" :key="supplier.id" class="card p-5 hover:shadow-lg transition-shadow">
        <div class="flex items-start justify-between mb-4">
          <div class="flex items-start gap-3">
            <div class="w-12 h-12 rounded-full bg-primary/10 flex items-center justify-center">
              <Building2 class="w-6 h-6 text-primary" />
            </div>
            <div>
              <h3 class="font-semibold text-lg">{{ supplier.name }}</h3>
              <p class="text-sm text-gray-600 dark:text-gray-400">{{ supplier.contact }}</p>
            </div>
          </div>
          <span class="badge badge-success">{{ supplier.status }}</span>
        </div>

        <div class="space-y-2 mb-4">
          <div class="flex items-center gap-2 text-sm">
            <Phone class="w-4 h-4 text-gray-400" />
            <span>{{ supplier.phone }}</span>
          </div>
          <div class="flex items-center gap-2 text-sm">
            <Mail class="w-4 h-4 text-gray-400" />
            <span>{{ supplier.email }}</span>
          </div>
          <div class="flex items-center gap-2 text-sm">
            <MapPin class="w-4 h-4 text-gray-400" />
            <span>{{ supplier.location }}</span>
          </div>
        </div>

        <div class="grid grid-cols-3 gap-4 pt-4 border-t border-gray-200 dark:border-gray-700">
          <div>
            <p class="text-xs text-gray-600 dark:text-gray-400">Orders</p>
            <p class="font-semibold">{{ supplier.totalOrders }}</p>
          </div>
          <div>
            <p class="text-xs text-gray-600 dark:text-gray-400">Total Spent</p>
            <p class="font-semibold">R {{ supplier.totalSpent.toLocaleString() }}</p>
          </div>
          <div>
            <p class="text-xs text-gray-600 dark:text-gray-400">Rating</p>
            <p class="font-semibold">{{ supplier.rating }} ⭐</p>
          </div>
        </div>

        <div class="flex gap-2 mt-4">
          <button class="btn btn-sm btn-outline flex-1">View Details</button>
          <button class="btn btn-sm btn-primary flex-1">New Order</button>
        </div>
      </div>
    </div>
  </div>
</template>
