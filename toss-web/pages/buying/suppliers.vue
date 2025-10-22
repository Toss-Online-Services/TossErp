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
              Manage your vendor relationships
            </p>
          </div>
          <div class="flex space-x-2 sm:space-x-3 flex-shrink-0">
            <button
              @click="showAddSupplierModal = true"
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
      
      <!-- Search and Filters -->
      <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 mb-6">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div class="relative md:col-span-2">
            <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
              <MagnifyingGlassIcon class="h-5 w-5 text-slate-400" />
            </div>
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search suppliers..."
              class="w-full pl-11 pr-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white transition-all duration-200"
            />
          </div>
          
          <select
            v-model="categoryFilter"
            class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white transition-all duration-200"
          >
            <option value="">All Categories</option>
            <option value="Beverages">Beverages</option>
            <option value="Groceries">Groceries</option>
            <option value="Snacks">Snacks</option>
            <option value="Household">Household</option>
            <option value="Technology">Technology</option>
            <option value="Other">Other</option>
          </select>
        </div>
      </div>

      <!-- Suppliers Grid -->
      <div v-if="filteredSuppliers.length > 0" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div
          v-for="supplier in filteredSuppliers"
          :key="supplier.id"
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1"
        >
          <!-- Supplier Header -->
          <div class="flex items-start justify-between mb-4">
            <div class="flex-1">
              <h3 class="text-lg font-bold text-slate-900 dark:text-white mb-1">{{ supplier.name }}</h3>
              <p class="text-sm text-slate-600 dark:text-slate-400">{{ supplier.category }}</p>
            </div>
            <div class="flex items-center space-x-1">
              <StarIcon class="w-5 h-5 text-yellow-400 fill-current" />
              <span class="text-sm font-bold text-slate-900 dark:text-white">{{ supplier.rating }}</span>
            </div>
          </div>

          <!-- Contact Info -->
          <div class="space-y-2 mb-4">
            <div class="flex items-center text-sm">
              <UserIcon class="w-4 h-4 text-slate-400 mr-2 flex-shrink-0" />
              <span class="text-slate-600 dark:text-slate-400 truncate">{{ supplier.contact }}</span>
            </div>
            <div class="flex items-center text-sm">
              <PhoneIcon class="w-4 h-4 text-slate-400 mr-2 flex-shrink-0" />
              <span class="text-slate-600 dark:text-slate-400">{{ supplier.phone }}</span>
            </div>
            <div class="flex items-center text-sm">
              <EnvelopeIcon class="w-4 h-4 text-slate-400 mr-2 flex-shrink-0" />
              <span class="text-slate-600 dark:text-slate-400 truncate">{{ supplier.email }}</span>
            </div>
            <div class="flex items-start text-sm">
              <MapPinIcon class="w-4 h-4 text-slate-400 mr-2 flex-shrink-0 mt-0.5" />
              <span class="text-slate-600 dark:text-slate-400">{{ supplier.address }}</span>
            </div>
          </div>

          <!-- Actions -->
          <div class="flex space-x-2 pt-4 border-t border-slate-200 dark:border-slate-700">
            <button
              @click="viewSupplier(supplier)"
              class="flex-1 px-4 py-2 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-lg hover:from-purple-700 hover:to-blue-700 shadow-md hover:shadow-lg transition-all duration-200 text-sm font-medium"
            >
              View Details
            </button>
            <button
              @click="createOrder(supplier)"
              class="px-4 py-2 border-2 border-purple-600 text-purple-600 rounded-lg hover:bg-purple-50 dark:hover:bg-purple-900/20 transition-all duration-200 text-sm font-medium"
            >
              Order
            </button>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-12 text-center">
        <div class="flex flex-col items-center justify-center">
          <div class="p-4 bg-gradient-to-br from-purple-100 to-blue-100 dark:from-purple-900/20 dark:to-blue-900/20 rounded-full mb-4">
            <BuildingStorefrontIcon class="w-12 h-12 text-purple-600 dark:text-purple-400" />
          </div>
          <p class="text-lg font-semibold text-slate-900 dark:text-white mb-2">No suppliers found</p>
          <p class="text-slate-600 dark:text-slate-400 mb-4">Add your first supplier to get started</p>
          <button
            @click="showAddSupplierModal = true"
            class="inline-flex items-center px-6 py-3 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-xl hover:from-purple-700 hover:to-blue-700 shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 font-semibold"
          >
            <PlusIcon class="w-5 h-5 mr-2" />
            Add Supplier
          </button>
        </div>
      </div>
    </div>

    <!-- Add Supplier Modal -->
    <Transition name="modal">
      <div v-if="showAddSupplierModal" class="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 overflow-y-auto">
        <div class="flex min-h-full items-center justify-center p-4">
          <div class="relative bg-white dark:bg-slate-800 rounded-2xl shadow-2xl border border-slate-200 dark:border-slate-700 w-full max-w-2xl">
            
            <!-- Header -->
            <div class="bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-4 flex items-center justify-between rounded-t-2xl">
              <h3 class="text-xl font-bold text-white">Add New Supplier</h3>
              <button @click="showAddSupplierModal = false" class="p-2 hover:bg-white/20 rounded-lg transition-colors">
                <XMarkIcon class="w-6 h-6 text-white" />
              </button>
            </div>

            <!-- Form -->
            <form @submit.prevent="addSupplier" class="p-6 space-y-4">
              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Supplier Name *</label>
                  <input
                    v-model="newSupplier.name"
                    type="text"
                    required
                    class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                  />
                </div>
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Category *</label>
                  <select
                    v-model="newSupplier.category"
                    required
                    class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                  >
                    <option value="">Select Category</option>
                    <option value="Beverages">Beverages</option>
                    <option value="Groceries">Groceries</option>
                    <option value="Snacks">Snacks</option>
                    <option value="Household">Household</option>
                    <option value="Technology">Technology</option>
                    <option value="Other">Other</option>
                  </select>
                </div>
              </div>

              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Contact Person *</label>
                  <input
                    v-model="newSupplier.contact"
                    type="text"
                    required
                    class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                  />
                </div>
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Phone *</label>
                  <input
                    v-model="newSupplier.phone"
                    type="tel"
                    required
                    class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                  />
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Email *</label>
                <input
                  v-model="newSupplier.email"
                  type="email"
                  required
                  class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                />
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Address *</label>
                <textarea
                  v-model="newSupplier.address"
                  required
                  rows="2"
                  class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white"
                ></textarea>
              </div>

              <div class="flex justify-end space-x-3 pt-4">
                <button
                  type="button"
                  @click="showAddSupplierModal = false"
                  class="px-6 py-2.5 border-2 border-slate-300 dark:border-slate-600 rounded-xl text-slate-700 dark:text-slate-300 font-medium hover:bg-slate-50 dark:hover:bg-slate-700 transition-all"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  class="px-6 py-2.5 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-xl font-medium hover:from-purple-700 hover:to-blue-700 shadow-lg hover:shadow-xl transition-all"
                >
                  Add Supplier
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import {
  PlusIcon,
  MagnifyingGlassIcon,
  StarIcon,
  UserIcon,
  PhoneIcon,
  EnvelopeIcon,
  MapPinIcon,
  BuildingStorefrontIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'
import { useBuyingAPI } from '~/composables/useBuyingAPI'

// Page metadata
useHead({
  title: 'Suppliers - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage vendor relationships in TOSS ERP' }
  ]
})

const router = useRouter()
const buyingAPI = useBuyingAPI()

// State
const suppliers = ref<any[]>([])
const searchQuery = ref('')
const categoryFilter = ref('')
const showAddSupplierModal = ref(false)
const loading = ref(true)

// New supplier form
const newSupplier = ref({
  name: '',
  category: '',
  contact: '',
  phone: '',
  email: '',
  address: ''
})

// Load suppliers on mount
onMounted(async () => {
  await loadSuppliers()
})

const loadSuppliers = async () => {
  loading.value = true
  try {
    suppliers.value = await buyingAPI.getSuppliers()
  } catch (error) {
    console.error('Failed to load suppliers:', error)
  } finally {
    loading.value = false
  }
}

// Computed
const filteredSuppliers = computed(() => {
  return suppliers.value.filter((supplier: any) => {
    const matchesSearch = !searchQuery.value || 
      supplier.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      supplier.contact.toLowerCase().includes(searchQuery.value.toLowerCase())
    
    const matchesCategory = !categoryFilter.value || supplier.category === categoryFilter.value
    
    return matchesSearch && matchesCategory
  })
})

// Methods
const addSupplier = async () => {
  try {
    await buyingAPI.addSupplier(newSupplier.value)
    await loadSuppliers()
    
    // Reset form
    newSupplier.value = {
      name: '',
      category: '',
      contact: '',
      phone: '',
      email: '',
      address: ''
    }
    
    showAddSupplierModal.value = false
    alert('✓ Supplier added successfully!')
  } catch (error) {
    console.error('Failed to add supplier:', error)
    alert('✗ Failed to add supplier')
  }
}

const viewSupplier = (supplier: any) => {
  alert(`Viewing details for ${supplier.name}`)
}

const createOrder = (supplier: any) => {
  router.push({
    path: '/buying/orders/create-order',
    query: { supplier: supplier.name }
  })
}
</script>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-active > div,
.modal-leave-active > div {
  transition: transform 0.3s ease, opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-from > div,
.modal-leave-to > div {
  transform: scale(0.95);
  opacity: 0;
}
</style>

