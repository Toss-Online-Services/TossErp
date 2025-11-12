<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { FormKit } from '@formkit/vue'
import { useI18n } from 'vue-i18n'
import { useHead } from '#imports'

const { t } = useI18n()
const route = useRoute()
const quotationId = route.params.id

useHead({
  title: t('sales.quotations.edit.pageTitle', { id: quotationId }),
})

// MOCK: Fetch existing quotation data
const existingQuotation = ref({
  customerId: 1,
  items: [
    { productId: 1, quantity: 20, unitPrice: 15.50 },
    { productId: 2, quantity: 10, unitPrice: 45.00 },
  ],
  discountRate: 5,
  terms: 'Payment due within 30 days. Prices valid for 14 days.',
  notes: 'Customer requested early morning delivery.',
})

// Mock data for options
const customers = [
  { label: 'Jabu\'s Spaza', value: 1, creditLimit: 5000, balance: 1200 },
  { label: 'Sipho\'s Tavern', value: 2, creditLimit: 10000, balance: 8500 },
  { label: 'The Gogo Shop', value: 3, creditLimit: 2500, balance: 0 },
]
const products = [
  { label: 'White Bread', value: 1, price: 15.50 },
  { label: 'Maize Meal 5kg', value: 2, price: 45.00 },
  { label: 'Cooking Oil 2L', value: 3, price: 65.75 },
  { label: 'Coca-Cola 2L', value: 4, price: 22.00 },
]

const vatRate = ref(15)
const form = ref()
const formNode = computed(() => form.value?.node)

const selectedCustomer = computed(() => {
  const customerId = formNode.value?.value?.customerId
  return customers.find(c => c.value === customerId) || null
})

const totals = computed(() => {
    const items = formNode.value?.value?.items || []
    const discountRate = formNode.value?.value?.discountRate || 0

    const subtotal = items.reduce((acc: number, item: any) => acc + (item.quantity || 0) * (item.unitPrice || 0), 0)
    const discountAmount = subtotal * (discountRate / 100)
    const taxableAmount = subtotal - discountAmount
    const vatAmount = taxableAmount * (vatRate.value / 100)
    const grandTotal = taxableAmount + vatAmount
    return { subtotal, discountAmount, vatAmount, grandTotal }
})

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
  }).format(amount || 0)
}

async function updateQuotation(data: any, node: any) {
  const submitter = node.target.dataset.name
  console.log('Updating quotation with data:', data)
  console.log('Submitter:', submitter)
  const payload = { ...data, totals: totals.value }

  if (submitter === 'update_send') {
    alert(`${t('sales.quotations.edit.updateAndSendSuccess')}!\n\n${JSON.stringify(payload, null, 2)}`)
  } else {
    alert(`${t('sales.quotations.edit.updateDraftSuccess')}!\n\n${JSON.stringify(payload, null, 2)}`)
  }
}

onMounted(() => {
  form.value = document.querySelector('[data-formkit-type="form"]')
  
  watch(() => formNode.value?.value?.items, (newItems, oldItems) => {
    if (!newItems) return

    newItems.forEach((item: any, index: number) => {
      const oldItem = oldItems?.[index]
      if (item.productId && item.productId !== oldItem?.productId) {
        const product = products.find(p => p.value === item.productId)
        if (product) {
          formNode.value?.at(`items.${index}.unitPrice`)?.input(product.price)
        }
      }
    })
  }, { deep: true })
})
</script>

<template>
  <div class="p-4 sm:p-6">
    <div class="flex items-center justify-between mb-6">
      <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
        {{ $t('sales.quotations.edit.title') }} #{{ quotationId }}
      </h1>
    </div>

    <FormKit type="form" :value="existingQuotation" @submit="updateQuotation" form-class="space-y-8" ref="form">
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Main Content -->
        <div class="lg:col-span-2 space-y-8">
          <!-- Customer Section -->
          <div class="card">
            <div class="card-header">
              <h3 class="card-title">{{ $t('sales.quotations.create.customerDetails') }}</h3>
            </div>
            <div class="card-body">
              <FormKit
                type="select"
                name="customerId"
                :label="$t('sales.quotations.create.customer')"
                placeholder="Select a customer"
                :options="customers"
                validation="required"
              />
              <div v-if="selectedCustomer" class="mt-4 p-3 bg-gray-50 dark:bg-gray-700/50 rounded-lg text-sm">
                <p><strong>{{ $t('sales.quotations.create.creditLimit') }}:</strong> {{ formatCurrency(selectedCustomer.creditLimit) }}</p>
                <p><strong>{{ $t('sales.quotations.create.outstandingBalance') }}:</strong> {{ formatCurrency(selectedCustomer.balance) }}</p>
              </div>
            </div>
          </div>

          <!-- Line Items Section -->
          <div class="card">
            <div class="card-header">
              <h3 class="card-title">{{ $t('sales.quotations.create.lineItems') }}</h3>
            </div>
            <div class="card-body">
              <FormKit type="list" name="items" dynamic #default="{ items, node, value: listValue }">
                <div v-for="(item, index) in items" :key="item" class="flex flex-col sm:flex-row gap-4 items-start mb-4 p-4 border dark:border-gray-700 rounded-lg">
                  <div class="flex-grow grid grid-cols-1 sm:grid-cols-3 gap-4 w-full">
                    <FormKit
                      type="select"
                      :index="index"
                      name="productId"
                      :label="$t('sales.quotations.create.product')"
                      placeholder="Select a product"
                      :options="products"
                      validation="required"
                    />
                    <FormKit
                      type="number"
                      :index="index"
                      name="quantity"
                      :label="$t('sales.quotations.create.quantity')"
                      min="1"
                      validation="required|min:1"
                    />
                    <FormKit
                      type="number"
                      :index="index"
                      name="unitPrice"
                      :label="$t('sales.quotations.create.unitPrice')"
                      step="0.01"
                      validation="required|min:0"
                    />
                  </div>
                  <button type="button" @click="() => node.input(listValue.filter((_, i) => i !== index))" class="btn btn-danger-outline mt-6 sm:mt-0">
                    <Icon name="heroicons:trash" class="w-4 h-4" />
                  </button>
                </div>
                <button type="button" @click="() => node.input([...listValue, {}])" class="btn btn-secondary">
                  <Icon name="heroicons:plus" class="w-4 h-4 mr-2" />
                  {{ $t('sales.quotations.create.addLineItem') }}
                </button>
              </FormKit>
            </div>
          </div>
        </div>

        <!-- Sidebar -->
        <div class="lg:col-span-1 space-y-8">
          <!-- Summary & Actions -->
          <div class="card">
            <div class="card-header">
              <h3 class="card-title">{{ $t('sales.quotations.create.summary') }}</h3>
            </div>
            <div class="card-body space-y-4">
              <div class="flex justify-between py-2 border-b">
                <span>{{ $t('sales.quotations.create.subtotal') }}:</span>
                <span class="font-medium">{{ formatCurrency(totals.subtotal) }}</span>
              </div>
              <div class="flex justify-between items-center py-2 border-b">
                <label for="discountRate">{{ $t('sales.quotations.create.discount') }} (%):</label>
                <FormKit
                  type="number"
                  name="discountRate"
                  outer-class="!mb-0"
                  inner-class="max-w-[80px]"
                  input-class="text-right"
                  min="0"
                  max="100"
                />
              </div>
               <div class="flex justify-between py-2 border-b">
                <span>{{ $t('sales.quotations.create.discountAmount') }}:</span>
                <span class="font-medium text-red-500">-{{ formatCurrency(totals.discountAmount) }}</span>
              </div>
              <div class="flex justify-between py-2 border-b">
                <span>{{ $t('sales.quotations.create.vat') }} ({{ vatRate }}%):</span>
                <span class="font-medium">{{ formatCurrency(totals.vatAmount) }}</span>
              </div>
              <div class="flex justify-between py-3">
                <span class="text-lg font-bold">{{ $t('sales.quotations.create.grandTotal') }}:</span>
                <span class="text-lg font-bold">{{ formatCurrency(totals.grandTotal) }}</span>
              </div>
            </div>
            <div class="card-footer flex flex-col sm:flex-row gap-2">
                <FormKit
                  type="submit"
                  :label="$t('sales.quotations.edit.updateAndSend')"
                  name="update_send"
                  outer-class="flex-grow"
                  input-class="w-full"
                />
                <FormKit
                  type="submit"
                  :label="$t('sales.quotations.edit.updateDraft')"
                  name="update_draft"
                  :classes="{
                    input: 'w-full justify-center btn-secondary'
                  }"
                  outer-class="flex-grow"
                />
            </div>
          </div>

          <!-- Other Details -->
          <div class="card">
            <div class="card-body">
              <FormKit
                type="textarea"
                name="terms"
                :label="$t('sales.quotations.create.termsAndConditions')"
                :rows="4"
              />
              <FormKit
                type="textarea"
                name="notes"
                :label="$t('sales.quotations.create.internalNotes')"
                :rows="3"
              />
            </div>
          </div>
        </div>
      </div>
    </FormKit>
  </div>
</template>

<style scoped>
.card { @apply bg-white dark:bg-gray-800 shadow-sm rounded-lg; }
.card-header { @apply p-4 border-b border-gray-200 dark:border-gray-700; }
.card-title { @apply text-lg font-semibold text-gray-800 dark:text-gray-200; }
.card-body { @apply p-4; }
.card-footer { @apply p-4 bg-gray-50 dark:bg-gray-900/50 border-t border-gray-200 dark:border-gray-700 rounded-b-lg; }
.btn { @apply inline-flex items-center justify-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-offset-2; }
.btn-secondary { @apply text-gray-700 bg-gray-100 hover:bg-gray-200 focus:ring-gray-500 dark:bg-gray-700 dark:text-gray-200 dark:hover:bg-gray-600; }
.btn-danger-outline { @apply text-red-700 bg-white border-red-300 hover:bg-red-50 focus:ring-red-500 dark:bg-gray-800 dark:border-red-600 dark:text-red-400 dark:hover:bg-gray-700; }
</style>
