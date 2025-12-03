<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useBuyingStore, type Supplier } from '~/stores/buying'
import SupplierModal from '~/components/buying/SupplierModal.vue'

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'Suppliers - TOSS'
})

const buyingStore = useBuyingStore()
const searchQuery = ref('')
const showAddModal = ref(false)
const showEditModal = ref(false)
const selectedSupplier = ref<Supplier | null>(null)

// Computed
const filteredSuppliers = computed(() => {
  let filtered = buyingStore.suppliers

  if (searchQuery.value.trim()) {
    const query = searchQuery.value.toLowerCase()
    filtered = filtered.filter(supplier =>
      supplier.name.toLowerCase().includes(query) ||
      supplier.code.toLowerCase().includes(query) ||
      supplier.email?.toLowerCase().includes(query) ||
      supplier.phone?.toLowerCase().includes(query)
    )
  }

  return filtered
})

const stats = computed(() => {
  const suppliers = buyingStore.suppliers
  return {
    total: suppliers.length,
    active: suppliers.filter(s => s.isActive).length,
    totalOutstanding: buyingStore.totalOutstanding
  }
})

// Methods
onMounted(async () => {
  await buyingStore.fetchSuppliers()
})

function handleAdd() {
  selectedSupplier.value = null
  showAddModal.value = true
}

function handleView(supplier: Supplier) {
  navigateTo(`/buying/suppliers/${supplier.id}`)
}

function handleEdit(supplier: Supplier) {
  selectedSupplier.value = supplier
  showEditModal.value = true
}

function handleSupplierSaved() {
  showAddModal.value = false
  showEditModal.value = false
  selectedSupplier.value = null
  buyingStore.fetchSuppliers()
}

function getStatusColor(status: boolean) {
  return status ? 'text-green-600 bg-green-100' : 'text-gray-600 bg-gray-100'
}
</script>

<template>
  <div class="py-6">
    <div class="mb-8">
      <h3 class="text-3xl font-bold text-gray-900 mb-2">Suppliers</h3>
      <p class="text-gray-600 text-sm">Manage supplier relationships and contacts</p>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Total Suppliers</p>
            <p class="mt-2 text-3xl font-bold text-gray-900">{{ stats.total }}</p>
          </div>
          <div class="p-3 bg-blue-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-blue-600">store</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Active Suppliers</p>
            <p class="mt-2 text-3xl font-bold text-green-600">{{ stats.active }}</p>
          </div>
          <div class="p-3 bg-green-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-green-600">check_circle</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-card p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-gray-600">Outstanding Balance</p>
            <p class="mt-2 text-3xl font-bold text-gray-900">R {{ stats.totalOutstanding.toFixed(2) }}</p>
          </div>
          <div class="p-3 bg-orange-100 rounded-lg">
            <i class="material-symbols-rounded text-2xl text-orange-600">account_balance</i>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters and Actions -->
    <div class="bg-white rounded-xl shadow-card p-6 mb-6">
      <div class="flex flex-col md:flex-row gap-4 items-center justify-between">
        <div class="flex-1 w-full">
          <div class="relative">
            <i class="material-symbols-rounded absolute left-3 top-1/2 -translate-y-1/2 text-gray-400">search</i>
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search by name, code, email, or phone..."
              class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-gray-900 focus:border-transparent"
            />
          </div>
        </div>
        <button
          @click="handleAdd"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors whitespace-nowrap"
        >
          <i class="material-symbols-rounded text-lg">add</i>
          <span>Add Supplier</span>
        </button>
      </div>
    </div>

    <!-- Suppliers Table -->
    <div class="bg-white rounded-xl shadow-card overflow-hidden">
      <div v-if="buyingStore.suppliers.length === 0" class="p-12 text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">store</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">No suppliers found</h3>
        <p class="text-gray-600 mb-6">Get started by adding your first supplier</p>
        <button
          @click="handleAdd"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">add</i>
          <span>Add Supplier</span>
        </button>
      </div>

      <div v-else-if="filteredSuppliers.length === 0" class="p-12 text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">search</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">No suppliers match your search</h3>
        <p class="text-gray-600 mb-6">Try adjusting your search terms</p>
      </div>

      <table v-else class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Supplier</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Contact</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Payment Terms</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Balance</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="supplier in filteredSuppliers" :key="supplier.id" class="hover:bg-gray-50">
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="flex items-center">
                <i class="material-symbols-rounded text-gray-400 mr-3">store</i>
                <div>
                  <div class="text-sm font-medium text-gray-900">{{ supplier.name }}</div>
                  <div class="text-sm text-gray-500">{{ supplier.code }}</div>
                </div>
              </div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <div class="text-sm text-gray-900">{{ supplier.contactPerson || '-' }}</div>
              <div class="text-sm text-gray-500">{{ supplier.email || '-' }}</div>
              <div class="text-sm text-gray-500">{{ supplier.phone || '-' }}</div>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{{ supplier.paymentTerms }}</td>
            <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
              R {{ supplier.currentBalance.toFixed(2) }}
            </td>
            <td class="px-6 py-4 whitespace-nowrap">
              <span :class="['px-2 py-1 text-xs font-medium rounded-full', getStatusColor(supplier.isActive)]">
                {{ supplier.isActive ? 'Active' : 'Inactive' }}
              </span>
            </td>
            <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
              <div class="flex items-center justify-end gap-2">
                <button
                  @click="handleView(supplier)"
                  class="p-2 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                  title="View"
                >
                  <i class="material-symbols-rounded text-lg">visibility</i>
                </button>
                <button
                  @click="handleEdit(supplier)"
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

    <!-- Modals -->
    <SupplierModal
      :show="showAddModal"
      :supplier="null"
      @close="showAddModal = false"
      @saved="handleSupplierSaved"
    />
    <SupplierModal
      :show="showEditModal"
      :supplier="selectedSupplier"
      @close="showEditModal = false; selectedSupplier = null"
      @saved="handleSupplierSaved"
    />
  </div>
</template>

