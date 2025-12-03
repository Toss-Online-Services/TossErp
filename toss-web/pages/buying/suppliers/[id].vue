<script setup lang="ts">
import { onMounted, ref, computed } from 'vue'
import { useBuyingStore, type Supplier } from '~/stores/buying'

definePageMeta({
  layout: 'default',
  ssr: false
})

useHead({
  title: 'Supplier Details - TOSS'
})

const route = useRoute()
const buyingStore = useBuyingStore()

const supplier = ref<Supplier | null>(null)
const loading = ref(false)

const supplierId = computed(() => {
  const id = route.params.id
  return Array.isArray(id) ? id[0] : String(id)
})

async function loadSupplier() {
  loading.value = true
  try {
    await buyingStore.fetchSuppliers()
    const id = supplierId.value
    console.log('Loading supplier with ID:', id)
    
    supplier.value = buyingStore.getSupplierById(id) || null
    
    if (!supplier.value) {
      console.error('Supplier not found for ID:', id)
    }
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

