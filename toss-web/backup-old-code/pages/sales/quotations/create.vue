<template>
  <div class="container mx-auto px-4 py-8">
    <!-- Header -->
    <div class="mb-8">
      <div class="flex items-center gap-2 mb-2">
        <NuxtLink to="/sales/quotations" class="text-gray-600 hover:text-gray-900">
          <Icon name="mdi:arrow-left" size="24" />
        </NuxtLink>
        <h1 class="text-3xl font-bold">{{ t('sales.quotations.newQuotation') }}</h1>
      </div>
      <p class="text-gray-600">{{ t('sales.quotations.createSubtitle') }}</p>
    </div>

    <!-- Form -->
    <FormKit
      type="form"
      @submit="handleSubmit"
      :actions="false"
      v-model="formData"
    >
      <!-- Customer Section -->
      <div class="bg-white rounded-lg shadow-sm p-6 mb-6">
        <h2 class="text-lg font-semibold mb-4 flex items-center gap-2">
          <Icon name="mdi:account" />
          {{ t('sales.quotations.customerDetails') }}
        </h2>
        
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <FormKit
            type="select"
            name="customerId"
            :label="t('sales.quotations.customer')"
            :options="customerOptions"
            validation="required"
            :help="t('sales.quotations.selectCustomerHelp')"
          />
          
          <FormKit
            type="text"
            name="customerPoNo"
            :label="t('sales.quotations.customerPoNo')"
            :placeholder="t('sales.quotations.customerPoNoPlaceholder')"
          />
        </div>

        <!-- Customer Info Display -->
        <div v-if="selectedCustomer" class="mt-4 p-4 bg-blue-50 rounded-lg">
          <div class="grid grid-cols-1 md:grid-cols-3 gap-4 text-sm">
            <div>
              <span class="text-gray-600">{{ t('sales.quotations.phone') }}:</span>
              <span class="ml-2 font-medium">{{ selectedCustomer.phone }}</span>
            </div>
            <div>
              <span class="text-gray-600">{{ t('sales.quotations.creditLimit') }}:</span>
              <span class="ml-2 font-medium">R{{ formatCurrency(selectedCustomer.creditLimit) }}</span>
            </div>
            <div>
              <span class="text-gray-600">{{ t('sales.quotations.balance') }}:</span>
              <span class="ml-2 font-medium" :class="selectedCustomer.balance > 0 ? 'text-red-600' : 'text-green-600'">
                R{{ formatCurrency(selectedCustomer.balance) }}
              </span>
            </div>
          </div>
        </div>
      </div>

      <!-- Quotation Details -->
      <div class="bg-white rounded-lg shadow-sm p-6 mb-6">
        <h2 class="text-lg font-semibold mb-4 flex items-center gap-2">
          <Icon name="mdi:file-document" />
          {{ t('sales.quotations.quotationDetails') }}
        </h2>
        
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <FormKit
            type="date"
            name="quotationDate"
            :label="t('sales.quotations.date')"
            validation="required"
            :value="today"
          />
          
          <FormKit
            type="date"
            name="validUntil"
            :label="t('sales.quotations.validUntil')"
            validation="required|date_after:quotationDate"
            :help="t('sales.quotations.validUntilHelp')"
          />
          
          <FormKit
            type="select"
            name="priceList"
            :label="t('sales.quotations.priceList')"
            :options="priceListOptions"
            validation="required"
          />
        </div>
      </div>

      <!-- Line Items -->
      <div class="bg-white rounded-lg shadow-sm p-6 mb-6">
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-lg font-semibold flex items-center gap-2">
            <Icon name="mdi:format-list-bulleted" />
            {{ t('sales.quotations.items') }}
          </h2>
          <button
            type="button"
            @click="addLineItem"
            class="btn btn-sm btn-secondary flex items-center gap-2"
          >
            <Icon name="mdi:plus" />
            {{ t('sales.quotations.addItem') }}
          </button>
        </div>

        <!-- Items Table -->
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-3 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                  {{ t('sales.quotations.product') }}
                </th>
                <th class="px-3 py-3 text-left text-xs font-medium text-gray-500 uppercase">
                  {{ t('sales.quotations.description') }}
                </th>
                <th class="px-3 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                  {{ t('sales.quotations.quantity') }}
                </th>
                <th class="px-3 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                  {{ t('sales.quotations.rate') }}
                </th>
                <th class="px-3 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                  {{ t('sales.quotations.discount') }} %
                </th>
                <th class="px-3 py-3 text-right text-xs font-medium text-gray-500 uppercase">
                  {{ t('sales.quotations.amount') }}
                </th>
                <th class="px-3 py-3"></th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr v-for="(item, index) in lineItems" :key="index">
                <td class="px-3 py-3">
                  <select
                    v-model="item.productId"
                    @change="onProductChange(index, $event.target.value)"
                    class="w-full border rounded px-2 py-1"
                    required
                  >
                    <option value="">{{ t('sales.quotations.selectProduct') }}</option>
                    <option v-for="product in products" :key="product.id" :value="product.id">
                      {{ product.name }}
                    </option>
                  </select>
                </td>
                <td class="px-3 py-3">
                  <input
                    v-model="item.description"
                    type="text"
                    class="w-full border rounded px-2 py-1"
                    :placeholder="t('sales.quotations.descriptionPlaceholder')"
                  />
                </td>
                <td class="px-3 py-3">
                  <input
                    v-model.number="item.quantity"
                    type="number"
                    min="1"
                    step="0.01"
                    @input="calculateLineAmount(index)"
                    class="w-20 border rounded px-2 py-1 text-right"
                    required
                  />
                </td>
                <td class="px-3 py-3">
                  <input
                    v-model.number="item.rate"
                    type="number"
                    min="0"
                    step="0.01"
                    @input="calculateLineAmount(index)"
                    class="w-24 border rounded px-2 py-1 text-right"
                    required
                  />
                </td>
                <td class="px-3 py-3">
                  <input
                    v-model.number="item.discountPercent"
                    type="number"
                    min="0"
                    max="100"
                    step="0.01"
                    @input="calculateLineAmount(index)"
                    class="w-16 border rounded px-2 py-1 text-right"
                  />
                </td>
                <td class="px-3 py-3 text-right font-medium">
                  R{{ formatCurrency(item.amount) }}
                </td>
                <td class="px-3 py-3">
                  <button
                    type="button"
                    @click="removeLineItem(index)"
                    class="text-red-600 hover:text-red-800"
                    :disabled="lineItems.length === 1"
                  >
                    <Icon name="mdi:delete" />
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Add Item Button (Mobile) -->
        <button
          type="button"
          @click="addLineItem"
          class="mt-4 w-full btn btn-secondary flex items-center justify-center gap-2 md:hidden"
        >
          <Icon name="mdi:plus" />
          {{ t('sales.quotations.addItem') }}
        </button>
      </div>

      <!-- Totals -->
      <div class="bg-white rounded-lg shadow-sm p-6 mb-6">
        <div class="max-w-md ml-auto">
          <div class="flex justify-between py-2 border-b">
            <span>{{ t('sales.quotations.subtotal') }}</span>
            <span class="font-medium">R{{ formatCurrency(totals.subtotal) }}</span>
          </div>
          
          <div class="flex justify-between py-2 border-b">
            <span>{{ t('sales.quotations.discount') }}</span>
            <span class="font-medium text-red-600">-R{{ formatCurrency(totals.discount) }}</span>
          </div>
          
          <div class="flex justify-between py-2 border-b">
            <span>{{ t('sales.quotations.taxableAmount') }}</span>
            <span class="font-medium">R{{ formatCurrency(totals.taxableAmount) }}</span>
          </div>
          
          <div class="flex justify-between py-2 border-b">
            <span>{{ t('sales.quotations.vat') }} (15%)</span>
            <span class="font-medium">R{{ formatCurrency(totals.tax) }}</span>
          </div>
          
          <div class="flex justify-between py-3 text-lg font-bold">
            <span>{{ t('sales.quotations.grandTotal') }}</span>
            <span class="text-blue-600">R{{ formatCurrency(totals.grandTotal) }}</span>
          </div>
        </div>
      </div>

      <!-- Additional Details -->
      <div class="bg-white rounded-lg shadow-sm p-6 mb-6">
        <h2 class="text-lg font-semibold mb-4 flex items-center gap-2">
          <Icon name="mdi:text-box" />
          {{ t('sales.quotations.additionalDetails') }}
        </h2>
        
        <div class="grid grid-cols-1 gap-4">
          <FormKit
            type="textarea"
            name="termsAndConditions"
            :label="t('sales.quotations.termsAndConditions')"
            rows="4"
            :placeholder="t('sales.quotations.termsPlaceholder')"
          />
          
          <FormKit
            type="textarea"
            name="notes"
            :label="t('sales.quotations.notes')"
            rows="3"
            :placeholder="t('sales.quotations.notesPlaceholder')"
          />
        </div>
      </div>

      <!-- Action Buttons -->
      <div class="flex flex-col sm:flex-row gap-4 justify-end">
        <button
          type="button"
          @click="navigateTo('/sales/quotations')"
          class="btn btn-secondary"
        >
          <Icon name="mdi:close" class="mr-2" />
          {{ t('common.cancel') }}
        </button>
        
        <button
          type="button"
          @click="handleSubmit('draft')"
          class="btn btn-outline"
          :disabled="isSubmitting"
        >
          <Icon name="mdi:content-save" class="mr-2" />
          {{ t('sales.quotations.saveAsDraft') }}
        </button>
        
        <button
          type="submit"
          class="btn btn-primary"
          :disabled="isSubmitting"
        >
          <Icon name="mdi:send" class="mr-2" />
          {{ t('sales.quotations.saveAndSend') }}
        </button>
      </div>
    </FormKit>
  </div>
</template>

<script setup lang="ts">
const { t } = useI18n()
const router = useRouter()

// Form state
const isSubmitting = ref(false)
const today = new Date().toISOString().split('T')[0]

const formData = ref({
  customerId: '',
  customerPoNo: '',
  quotationDate: today,
  validUntil: '',
  priceList: 'standard',
  termsAndConditions: '',
  notes: ''
})

// Line items
const lineItems = ref([
  {
    productId: '',
    description: '',
    quantity: 1,
    rate: 0,
    discountPercent: 0,
    amount: 0
  }
])

// Mock data - Replace with API calls
const customers = ref([
  { id: 1, name: 'Mama Dlamini Spaza', phone: '082 123 4567', creditLimit: 5000, balance: 1250 },
  { id: 2, name: 'Sibusiso Butchery', phone: '083 234 5678', creditLimit: 10000, balance: 0 },
  { id: 3, name: 'Thandi Hair Salon', phone: '084 345 6789', creditLimit: 3000, balance: 750 }
])

const products = ref([
  { id: 1, name: 'White Bread', rate: 15.99, description: 'Albany White Bread 700g' },
  { id: 2, name: 'Milk 2L', rate: 32.99, description: 'Full Cream Milk 2L' },
  { id: 3, name: 'Sugar 2.5kg', rate: 42.99, description: 'White Sugar 2.5kg' },
  { id: 4, name: 'Cooking Oil 2L', rate: 54.99, description: 'Sunflower Oil 2L' },
  { id: 5, name: 'Maize Meal 5kg', rate: 78.99, description: 'Super Maize Meal 5kg' }
])

const priceListOptions = [
  { value: 'standard', label: t('sales.quotations.standardPricing') },
  { value: 'wholesale', label: t('sales.quotations.wholesalePricing') },
  { value: 'bulk', label: t('sales.quotations.bulkPricing') }
]

// Computed
const customerOptions = computed(() => 
  customers.value.map(c => ({ value: c.id, label: c.name }))
)

const selectedCustomer = computed(() => 
  customers.value.find(c => c.id === formData.value.customerId)
)

const totals = computed(() => {
  const subtotal = lineItems.value.reduce((sum, item) => {
    const lineTotal = item.quantity * item.rate
    return sum + lineTotal
  }, 0)
  
  const discount = lineItems.value.reduce((sum, item) => {
    const lineTotal = item.quantity * item.rate
    const lineDiscount = (lineTotal * item.discountPercent) / 100
    return sum + lineDiscount
  }, 0)
  
  const taxableAmount = subtotal - discount
  const tax = taxableAmount * 0.15 // 15% VAT
  const grandTotal = taxableAmount + tax
  
  return {
    subtotal,
    discount,
    taxableAmount,
    tax,
    grandTotal
  }
})

// Methods
const addLineItem = () => {
  lineItems.value.push({
    productId: '',
    description: '',
    quantity: 1,
    rate: 0,
    discountPercent: 0,
    amount: 0
  })
}

const removeLineItem = (index: number) => {
  if (lineItems.value.length > 1) {
    lineItems.value.splice(index, 1)
  }
}

const onProductChange = (index: number, productId: string) => {
  const product = products.value.find(p => p.id === parseInt(productId))
  if (product) {
    lineItems.value[index].description = product.description
    lineItems.value[index].rate = product.rate
    calculateLineAmount(index)
  }
}

const calculateLineAmount = (index: number) => {
  const item = lineItems.value[index]
  const lineTotal = item.quantity * item.rate
  const discount = (lineTotal * item.discountPercent) / 100
  item.amount = lineTotal - discount
}

const formatCurrency = (amount: number): string => {
  return amount.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',')
}

const handleSubmit = async (status: string = 'sent') => {
  // Validate at least one item
  if (lineItems.value.length === 0 || !lineItems.value[0].productId) {
    alert(t('sales.quotations.addAtLeastOneItem'))
    return
  }
  
  isSubmitting.value = true
  
  try {
    const quotationData = {
      ...formData.value,
      status,
      items: lineItems.value,
      subtotal: totals.value.subtotal,
      taxAmount: totals.value.tax,
      discountAmount: totals.value.discount,
      grandTotal: totals.value.grandTotal
    }
    
    // API call would go here
    console.log('Creating quotation:', quotationData)
    
    // Simulate API call
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    // Show success message
    alert(t('sales.quotations.quotationCreated'))
    
    // Navigate back to list
    router.push('/sales/quotations')
  } catch (error) {
    console.error('Error creating quotation:', error)
    alert(t('sales.quotations.errorCreating'))
  } finally {
    isSubmitting.value = false
  }
}

// Page meta
definePageMeta({
  layout: 'default',
  middleware: 'auth'
})

// SEO
useHead({
  title: t('sales.quotations.newQuotation')
})
</script>

<style scoped>
.btn {
  @apply px-4 py-2 rounded-lg font-medium transition-colors;
}

.btn-primary {
  @apply bg-blue-600 text-white hover:bg-blue-700;
}

.btn-secondary {
  @apply bg-gray-200 text-gray-800 hover:bg-gray-300;
}

.btn-outline {
  @apply border-2 border-blue-600 text-blue-600 hover:bg-blue-50;
}

.btn-sm {
  @apply px-3 py-1 text-sm;
}

.btn:disabled {
  @apply opacity-50 cursor-not-allowed;
}
</style>
