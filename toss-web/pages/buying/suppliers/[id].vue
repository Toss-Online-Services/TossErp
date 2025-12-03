<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useBuyingStore, type Supplier } from '~/stores/buying'
import { useStockStore, type Item } from '~/stores/stock'

definePageMeta({
  layout: 'default',
  ssr: false
})

useHead({
  title: 'Supplier Details - TOSS'
})

const route = useRoute()
const buyingStore = useBuyingStore()
const stockStore = useStockStore()

const supplier = ref<Supplier | null>(null)
const supplierItems = ref<Item[]>([])
const loading = ref(false)

const supplierId = computed(() => {
  const id = route.params.id
  return Array.isArray(id) ? id[0] : String(id)
})

async function loadSupplier() {
  loading.value = true
  try {
    await buyingStore.fetchSuppliers()
    await stockStore.fetchItems()
    
    const id = supplierId.value
    console.log('Loading supplier with ID:', id)
    
    supplier.value = buyingStore.getSupplierById(id) || null
    
    if (!supplier.value) {
      console.error('Supplier not found for ID:', id)
      return
    }
    
    // Load items from this supplier
    supplierItems.value = stockStore.items.filter(item => 
      item.supplier && item.supplier.toLowerCase() === supplier.value?.name.toLowerCase()
    )
  } catch (error) {
    console.error('Failed to load supplier:', error)
  } finally {
    loading.value = false
  }
}

function getStatusColor(status: boolean) {
  return status ? 'text-green-600 bg-green-100' : 'text-gray-600 bg-gray-100'
}

function handlePrint() {
  window.print()
}

async function handleSend() {
  if (!supplier.value) return
  try {
    // TODO: Implement send functionality (email/SMS/WhatsApp)
    alert(`Sending supplier information for ${supplier.value.name}...`)
    // await buyingStore.sendSupplierInfo(supplier.value.id)
  } catch (error) {
    console.error('Failed to send supplier info:', error)
  }
}

onMounted(async () => {
  await loadSupplier()
})
</script>

<template>
  <div class="py-6">
    <div v-if="loading" class="flex items-center justify-center py-12">
      <div class="text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4 animate-spin">refresh</i>
        <p class="text-gray-600">Loading supplier...</p>
      </div>
    </div>

    <div v-else-if="!supplier" class="flex items-center justify-center py-12">
      <div class="text-center">
        <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">error</i>
        <h3 class="text-lg font-semibold text-gray-900 mb-2">Supplier not found</h3>
        <p class="text-gray-600 mb-6">The supplier you're looking for doesn't exist</p>
        <button
          @click="navigateTo('/buying/suppliers')"
          class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">arrow_back</i>
          <span>Back to Suppliers</span>
        </button>
      </div>
    </div>

    <div v-else>
      <!-- Header -->
      <div class="mb-6">
        <button
          @click="navigateTo('/buying/suppliers')"
          class="inline-flex items-center gap-2 text-gray-600 hover:text-gray-900 mb-4 transition-colors"
        >
          <i class="material-symbols-rounded text-lg">arrow_back</i>
          <span>Back to Suppliers</span>
        </button>
        <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
          <div>
            <h3 class="text-3xl font-bold text-gray-900 mb-2">{{ supplier.name }}</h3>
            <p class="text-gray-600 text-sm">Supplier details and information</p>
          </div>
          <div class="flex flex-wrap items-center gap-3">
            <span :class="['px-3 py-1 text-sm font-medium rounded-full', getStatusColor(supplier.isActive)]">
              {{ supplier.isActive ? 'Active' : 'Inactive' }}
            </span>
            <button
              @click="handlePrint"
              class="inline-flex items-center gap-2 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
            >
              <i class="material-symbols-rounded text-lg">print</i>
              <span>Print</span>
            </button>
            <button
              @click="handleSend"
              class="inline-flex items-center gap-2 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
            >
              <i class="material-symbols-rounded text-lg">send</i>
              <span>Send</span>
            </button>
          </div>
        </div>
      </div>

      <!-- Info Cards -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">store</i>
            Supplier Information
          </h4>
          <div class="space-y-3">
            <div>
              <p class="text-xs text-gray-500 mb-1">Supplier Code</p>
              <p class="text-base font-semibold text-gray-900">{{ supplier.code }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 mb-1">Supplier Name</p>
              <p class="text-base font-semibold text-gray-900">{{ supplier.name }}</p>
            </div>
            <div v-if="supplier.contactPerson">
              <p class="text-xs text-gray-500 mb-1">Contact Person</p>
              <p class="text-base font-semibold text-gray-900">{{ supplier.contactPerson }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">contact_mail</i>
            Contact Details
          </h4>
          <div class="space-y-3">
            <div v-if="supplier.email">
              <p class="text-xs text-gray-500 mb-1">Email</p>
              <p class="text-base font-semibold text-gray-900">{{ supplier.email }}</p>
            </div>
            <div v-if="supplier.phone">
              <p class="text-xs text-gray-500 mb-1">Phone</p>
              <p class="text-base font-semibold text-gray-900">{{ supplier.phone }}</p>
            </div>
            <div v-if="supplier.address">
              <p class="text-xs text-gray-500 mb-1">Address</p>
              <p class="text-sm text-gray-700">{{ supplier.address }}</p>
              <p v-if="supplier.city" class="text-sm text-gray-700">{{ supplier.city }}{{ supplier.province ? `, ${supplier.province}` : '' }}</p>
              <p v-if="supplier.postalCode" class="text-sm text-gray-700">{{ supplier.postalCode }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-card p-6">
          <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
            <i class="material-symbols-rounded text-lg">account_balance</i>
            Financial Information
          </h4>
          <div class="space-y-3">
            <div>
              <p class="text-xs text-gray-500 mb-1">Payment Terms</p>
              <p class="text-base font-semibold text-gray-900">{{ supplier.paymentTerms }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 mb-1">Currency</p>
              <p class="text-base font-semibold text-gray-900">{{ supplier.currency }}</p>
            </div>
            <div v-if="supplier.creditLimit">
              <p class="text-xs text-gray-500 mb-1">Credit Limit</p>
              <p class="text-base font-semibold text-gray-900">R {{ supplier.creditLimit.toFixed(2) }}</p>
            </div>
            <div>
              <p class="text-xs text-gray-500 mb-1">Current Balance</p>
              <p class="text-lg font-bold text-gray-900">R {{ supplier.currentBalance.toFixed(2) }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Supplier Products -->
      <div class="bg-white rounded-xl shadow-card overflow-hidden mb-6">
        <div class="px-6 py-4 border-b border-gray-200 flex items-center justify-between">
          <h4 class="text-lg font-semibold text-gray-900 flex items-center gap-2">
            <i class="material-symbols-rounded text-xl">inventory</i>
            Supplier Products
          </h4>
          <span class="text-sm text-gray-500">{{ supplierItems.length }} item(s)</span>
        </div>
        
        <div v-if="supplierItems.length === 0" class="p-12 text-center">
          <i class="material-symbols-rounded text-6xl text-gray-300 mb-4">inventory_2</i>
          <h3 class="text-lg font-semibold text-gray-900 mb-2">No products found</h3>
          <p class="text-gray-600 mb-6">No items are currently linked to this supplier</p>
          <NuxtLink
            to="/stock/items"
            class="inline-flex items-center gap-2 px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
          >
            <i class="material-symbols-rounded text-lg">add</i>
            <span>Add Items</span>
          </NuxtLink>
        </div>
        
        <div v-else class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Item</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Category</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Stock</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Cost Price</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Selling Price</th>
                <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="item in supplierItems" :key="item.id" class="hover:bg-gray-50 transition-colors">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <i class="material-symbols-rounded text-gray-400 mr-3">inventory_2</i>
                    <div>
                      <div class="text-sm font-medium text-gray-900">{{ item.name }}</div>
                      <div class="text-sm text-gray-500">{{ item.code }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="text-sm text-gray-900">{{ item.category }}</span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center gap-2">
                    <span class="text-sm font-medium text-gray-900">{{ item.currentStock }} {{ item.unit }}</span>
                    <span
                      v-if="item.currentStock <= item.minStock"
                      class="px-2 py-0.5 text-xs font-medium rounded-full bg-red-100 text-red-800"
                    >
                      Low
                    </span>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="text-sm text-gray-900">R {{ item.costPrice.toFixed(2) }}</span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="text-sm font-medium text-gray-900">R {{ item.sellingPrice.toFixed(2) }}</span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                  <NuxtLink
                    :to="`/stock/items/${item.id}`"
                    class="inline-flex items-center gap-1 px-3 py-1.5 text-gray-600 hover:text-gray-900 hover:bg-gray-100 rounded-lg transition-colors"
                  >
                    <i class="material-symbols-rounded text-base">visibility</i>
                    <span>View</span>
                  </NuxtLink>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Notes -->
      <div v-if="supplier.notes" class="bg-white rounded-xl shadow-card p-6">
        <h4 class="text-sm font-medium text-gray-600 mb-4 flex items-center gap-2">
          <i class="material-symbols-rounded text-lg">note</i>
          Notes
        </h4>
        <p class="text-sm text-gray-700 leading-relaxed">{{ supplier.notes }}</p>
      </div>
    </div>
  </div>
</template>

