<template>
  <div class="p-4 sm:p-6">
    <div class="flex items-center justify-between mb-6">
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('sales.invoices.edit.title') }} #{{ invoiceId }}
      </h1>
    </div>

    <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg p-6">
      <FormKit
        type="form"
        v-model="formData"
        :actions="false"
        @submit="submitHandler"
        ref="form"
      >
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <FormKit
            type="text"
            name="customerName"
            :label="$t('sales.invoices.create.customer')"
            validation="required"
            disabled
          />
          <FormKit
            type="select"
            name="status"
            :label="$t('common.status')"
            :options="statusOptions"
            validation="required"
          />
        </div>
        
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mt-4">
            <FormKit
                type="date"
                name="date"
                :label="$t('sales.invoices.create.invoiceDate')"
                validation="required"
            />
            <FormKit
                type="date"
                name="dueDate"
                :label="$t('sales.invoices.create.dueDate')"
                validation="required|date_after:date"
            />
        </div>

        <div class="mt-8">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">{{ $t('sales.invoices.create.items') }}</h3>
          <FormKit type="list" name="items" :value="[]" dynamic #default="{ items, node, value }">
            <div v-for="(item, index) in items" :key="item" class="flex items-end gap-4 p-4 mb-4 border rounded-lg">
              <div class="grid grid-cols-1 sm:grid-cols-4 gap-4 flex-grow">
                <FormKit
                  type="select"
                  :name="`items[${index}].productId`"
                  :label="$t('common.product')"
                  placeholder="Select a product"
                  :options="productOptions"
                  validation="required"
                  @change="handleProductChange(index, $event)"
                />
                <FormKit
                  type="number"
                  :name="`items[${index}].quantity`"
                  :label="$t('common.quantity')"
                  validation="required|min:1"
                  :value="1"
                  @input="updateTotals"
                />
                <FormKit
                  type="number"
                  :name="`items[${index}].unitPrice`"
                  :label="$t('common.unitPrice')"
                  step="0.01"
                  validation="required|min:0"
                  :disabled="true"
                />
                <div class="flex flex-col justify-end">
                    <label class="formkit-label">{{ $t('common.total') }}</label>
                    <p class="mt-2 font-medium text-gray-700 dark:text-gray-300">
                        {{ formatCurrency(calculateItemTotal(index)) }}
                    </p>
                </div>
              </div>
              <button @click="() => node.input(value.filter((_, i) => i !== index))" type="button" class="btn btn-danger mb-2">
                <Icon name="heroicons:trash" class="w-4 h-4" />
              </button>
            </div>
            <button @click="() => node.input([...value, {}])" type="button" class="btn btn-secondary">
              {{ $t('sales.invoices.create.addItem') }}
            </button>
          </FormKit>
        </div>

        <div class="mt-8 flex justify-end">
          <div class="w-full max-w-sm space-y-2 text-sm">
            <div class="flex justify-between">
              <span class="text-gray-500 dark:text-gray-400">{{ $t('common.subtotal') }}:</span>
              <span class="font-medium text-gray-800 dark:text-gray-200">{{ formatCurrency(totals.subtotal) }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-gray-500 dark:text-gray-400">{{ $t('common.vat') }} (15%):</span>
              <span class="font-medium text-gray-800 dark:text-gray-200">{{ formatCurrency(totals.vat) }}</span>
            </div>
            <div class="flex justify-between text-base font-bold">
              <span class="text-gray-900 dark:text-white">{{ $t('common.total') }}:</span>
              <span class="text-gray-900 dark:text-white">{{ formatCurrency(totals.total) }}</span>
            </div>
          </div>
        </div>

        <div class="flex justify-end gap-4 mt-8">
          <NuxtLink :to="`/sales/invoices/${invoiceId}`" class="btn btn-secondary">
            {{ $t('common.cancel') }}
          </NuxtLink>
          <FormKit type="submit" :label="$t('common.saveChanges')" />
        </div>
      </FormKit>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed, watch } from 'vue'
import { useHead } from '#imports'

const { t } = useI18n()
const route = useRoute()
const invoiceId = route.params.id

// Mock Data
const mockInvoice = {
  id: `INV-001`,
  customerName: 'Jabu\'s Spaza',
  date: '2025-11-12',
  dueDate: '2025-12-12',
  status: 'Paid',
  items: [
    { productId: 'prod-1', name: 'Albany Superior White Bread', quantity: 20, unitPrice: 15.50 },
    { productId: 'prod-2', name: 'Clover Full Cream Milk 1L', quantity: 12, unitPrice: 22.00 },
    { productId: 'prod-3', name: 'Huletts Brown Sugar 2kg', quantity: 5, unitPrice: 45.75 },
  ],
}

const mockProducts = [
  { id: 'prod-1', name: 'Albany Superior White Bread', price: 15.50 },
  { id: 'prod-2', name: 'Clover Full Cream Milk 1L', price: 22.00 },
  { id: 'prod-3', name: 'Huletts Brown Sugar 2kg', price: 45.75 },
  { id: 'prod-4', name: 'Coca-Cola 2L', price: 25.00 },
  { id: 'prod-5', name: 'Simba Chips Large', price: 18.99 },
]

const formData = ref<any>({})
const totals = reactive({ subtotal: 0, vat: 0, total: 0 })
const form = ref()
const formNode = computed(() => form.value?.node)

const productOptions = mockProducts.map(p => ({ label: p.name, value: p.id }))
const statusOptions = ['Draft', 'Sent', 'Paid', 'Overdue', 'Cancelled'].map(s => ({ label: t(`sales.status.${s.toLowerCase()}`), value: s }))

onMounted(() => {
  // In a real app, you would fetch the invoice data from an API
  formData.value = { ...mockInvoice }
  updateTotals()

  watch(() => formData.value?.items, (newItems, oldItems) => {
    if (!newItems) return;
    newItems.forEach((item: any, index: number) => {
      const oldItem = oldItems?.[index];
      if (item && item.productId && item.productId !== oldItem?.productId) {
        handleProductChange(index, item.productId);
      }
    });
    updateTotals();
  }, { deep: true });
})

useHead({
  title: t('sales.invoices.edit.pageTitle', { id: invoiceId }),
})

const formatCurrency = (value: number) => new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(value || 0)

const handleProductChange = (index: number, productId: string) => {
  const product = mockProducts.find(p => p.id === productId)
  if (product) {
    const priceNode = formNode.value?.at(`items.${index}.unitPrice`)
    if (priceNode) {
      priceNode.input(product.price)
    }
  }
}

const calculateItemTotal = (index: number) => {
    const item = formData.value?.items?.[index]
    if (item && item.quantity && item.unitPrice) {
        return item.quantity * item.unitPrice
    }
    return 0
}

const updateTotals = () => {
  const items = formData.value?.items || []
  let subtotal = 0
  if (Array.isArray(items)) {
    subtotal = items.reduce((acc, item) => {
      const quantity = Number(item.quantity) || 0
      const price = Number(item.unitPrice) || 0
      return acc + (quantity * price)
    }, 0)
  }
  
  const vat = subtotal * 0.15
  const total = subtotal + vat

  totals.subtotal = subtotal
  totals.vat = vat
  totals.total = total
}

const submitHandler = async () => {
  // Here you would send the updated data to your API
  console.log('Form submitted:', formData.value)
  alert('Invoice updated successfully!')
  // navigateTo(`/sales/invoices/${invoiceId}`)
}
</script>

<style scoped>
.btn {
  @apply inline-flex items-center justify-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-offset-2;
}
.btn-secondary {
  @apply text-gray-700 bg-gray-100 hover:bg-gray-200 dark:bg-gray-700 dark:text-gray-200 dark:hover:bg-gray-600 focus:ring-gray-500;
}
.btn-danger {
  @apply text-white bg-red-600 hover:bg-red-700 focus:ring-red-500;
}
</style>
