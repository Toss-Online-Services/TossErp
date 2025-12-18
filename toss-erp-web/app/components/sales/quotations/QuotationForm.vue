<template>
  <FormKit type="form" v-model="formState" :actions="false" @submit="() => handleSubmit('sent')">
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

      <div v-if="selectedCustomer" class="mt-4 p-4 bg-blue-50 rounded-lg">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4 text-sm">
          <div>
            <span class="text-gray-600">{{ t('sales.quotations.phone') }}:</span>
            <span class="ml-2 font-medium">{{ selectedCustomer.phone }}</span>
          </div>
          <div>
            <span class="text-gray-600">{{ t('sales.quotations.creditLimit') }}:</span>
            <span class="ml-2 font-medium">{{ formatCurrency(selectedCustomer.creditLimit) }}</span>
          </div>
          <div>
            <span class="text-gray-600">{{ t('sales.quotations.balance') }}:</span>
            <span class="ml-2 font-medium" :class="selectedCustomer.balance > 0 ? 'text-red-600' : 'text-green-600'">
              {{ formatCurrency(selectedCustomer.balance) }}
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
        <button type="button" @click="addLineItem" class="btn btn-sm btn-secondary flex items-center gap-2">
          <Icon name="mdi:plus" />
          {{ t('sales.quotations.addItem') }}
        </button>
      </div>

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
                  @change="onProductChange(index, (item.productId as string))"
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
                  @input="updateLineAmount(index)"
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
                  @input="updateLineAmount(index)"
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
                  @input="updateLineAmount(index)"
                  class="w-16 border rounded px-2 py-1 text-right"
                />
              </td>
              <td class="px-3 py-3 text-right font-medium">
                {{ formatCurrency(item.amount || 0) }}
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

      <button type="button" @click="addLineItem" class="mt-4 w-full btn btn-secondary flex items-center justify-center gap-2 md:hidden">
        <Icon name="mdi:plus" />
        {{ t('sales.quotations.addItem') }}
      </button>
    </div>

    <!-- Totals -->
    <div class="bg-white rounded-lg shadow-sm p-6 mb-6">
      <div class="max-w-md ml-auto">
        <div class="flex justify-between py-2 border-b">
          <span>{{ t('sales.quotations.subtotal') }}</span>
          <span class="font-medium">{{ formatCurrency(totals.subtotal) }}</span>
        </div>

        <div class="flex justify-between py-2 border-b">
          <span>{{ t('sales.quotations.discount') }}</span>
          <span class="font-medium text-red-600">-{{ formatCurrency(totals.discount || totals.discountAmount) }}</span>
        </div>

        <div class="flex justify-between py-2 border-b">
          <span>{{ t('sales.quotations.taxableAmount') }}</span>
          <span class="font-medium">{{ formatCurrency(totals.taxableAmount) }}</span>
        </div>

        <div class="flex justify-between py-2 border-b">
          <span>{{ t('sales.quotations.vat') }} (15%)</span>
          <span class="font-medium">{{ formatCurrency(totals.tax || totals.taxAmount) }}</span>
        </div>

        <div class="flex justify-between py-3 text-lg font-bold">
          <span>{{ t('sales.quotations.grandTotal') }}</span>
          <span class="text-blue-600">{{ formatCurrency(totals.grandTotal) }}</span>
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

    <!-- Actions -->
    <div class="flex flex-col sm:flex-row gap-4 justify-end">
      <button type="button" @click="emit('cancel')" class="btn btn-secondary">
        <Icon name="mdi:close" class="mr-2" />
        {{ t('common.cancel') }}
      </button>

      <button type="button" class="btn btn-outline" :disabled="submitting" @click="handleSubmit('draft')">
        <Icon name="mdi:content-save" class="mr-2" />
        {{ t('sales.quotations.saveAsDraft') }}
      </button>

      <button type="submit" class="btn btn-primary" :disabled="submitting">
        <Icon name="mdi:send" class="mr-2" />
        {{ t('sales.quotations.saveAndSend') }}
      </button>
    </div>
  </FormKit>
</template>

<script setup lang="ts">
import type { QuotationMeta, QuotationRecord, QuotationFormItemInput, QuotationTotals } from '~/types/sales'
import { buildQuotationItemsPayload, calculateLineAmount, calculateQuotationTotals } from '~/utils/quotations'

const props = defineProps<{
  meta: QuotationMeta
  initialQuotation?: QuotationRecord
  mode?: 'create' | 'edit'
  submitting?: boolean
}>()

const emit = defineEmits<{
  (e: 'submit', payload: { status: 'draft' | 'sent'; body: Record<string, any> }): void
  (e: 'cancel'): void
}>()

// const { t } = useI18n() // TODO: Configure i18n if needed
const t = (key: string) => key // Temporary fallback
const today = new Date().toISOString().split('T')[0]

const createEmptyLineItem = (): QuotationFormItemInput => ({
  productId: '',
  productName: '',
  description: '',
  quantity: 1,
  rate: 0,
  discountPercent: 0,
  amount: 0
})

const defaultFormState = () => ({
  customerId: '',
  customerPoNo: '',
  quotationDate: today,
  validUntil: today,
  priceList: props.meta.priceLists[0]?.value || 'standard',
  termsAndConditions: '',
  notes: ''
})

const formState = ref(defaultFormState())
const lineItems = ref<QuotationFormItemInput[]>([createEmptyLineItem()])

const customerOptions = computed(() => props.meta.customers.map((customer) => ({ value: customer.id, label: customer.businessName })))
const priceListOptions = computed(() => props.meta.priceLists)
const products = computed(() => props.meta.products)

const selectedCustomer = computed(() => props.meta.customers.find((customer) => customer.id === formState.value.customerId))

const totals = computed<QuotationTotals>(() => {
  const calculated = calculateQuotationTotals(lineItems.value.map(item => ({
    id: item.productId,
    productId: item.productId,
    productName: item.productName || '',
    description: item.description,
    quantity: item.quantity,
    rate: item.rate || 0,
    discountPercent: item.discountPercent,
    vatRate: 15, // Default VAT rate
    amount: item.amount || 0
  })))
  return {
    ...calculated,
    discount: calculated.discountAmount,
    tax: calculated.taxAmount
  }
})

const formatCurrency = (amount: number): string => `R${amount.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',')}`

const resetFromQuotation = (quotation?: QuotationRecord) => {
  if (!quotation) {
    formState.value = defaultFormState()
    lineItems.value = [createEmptyLineItem()]
    return
  }

  formState.value = {
    customerId: quotation.customerId,
    customerPoNo: '',
    quotationDate: quotation.quotationDate.slice(0, 10),
    validUntil: quotation.validUntil.slice(0, 10),
    priceList: quotation.priceList,
    termsAndConditions: quotation.termsAndConditions || '',
    notes: quotation.notes || ''
  }

  lineItems.value = quotation.items.map((item) => ({
    productId: item.productId,
    productName: item.productName,
    description: item.description || '',
    quantity: item.quantity,
    rate: item.rate,
    discountPercent: item.discountPercent,
    vatRate: item.vatRate,
    amount: item.amount
  }))
}

watch(
  () => props.initialQuotation,
  (value) => {
    resetFromQuotation(value)
  },
  { immediate: true }
)

const addLineItem = () => {
  lineItems.value.push(createEmptyLineItem())
}

const removeLineItem = (index: number) => {
  if (lineItems.value.length === 1) {
    return
  }
  lineItems.value.splice(index, 1)
}

const updateLineAmount = (index: number) => {
  const item = lineItems.value[index]
  if (item && item.rate !== undefined) {
    item.amount = calculateLineAmount(item.quantity, item.rate, item.discountPercent || 0)
  }
}

const onProductChange = (index: number, productId: string) => {
  const product = products.value.find((item) => item.id === productId)
  if (!product) {
    return
  }

  if (lineItems.value[index]) {
    lineItems.value[index].rate = product.price
    lineItems.value[index].description = product.description
    lineItems.value[index].productName = product.name
    updateLineAmount(index)
  }
}

const handleSubmit = (status: 'draft' | 'sent') => {
  if (!formState.value.customerId) {
    alert(t('sales.quotations.selectCustomerHelp'))
    return
  }

  const itemsPayload = buildQuotationItemsPayload(lineItems.value, products.value)
  if (!itemsPayload.length) {
    alert(t('sales.quotations.addAtLeastOneItem'))
    return
  }

  const body = {
    customerId: formState.value.customerId,
    quotationDate: formState.value.quotationDate,
    validUntil: formState.value.validUntil,
    priceList: formState.value.priceList,
    currency: 'ZAR',
    termsAndConditions: formState.value.termsAndConditions,
    notes: formState.value.notes,
    customerPoNumber: formState.value.customerPoNo,
    items: itemsPayload,
    status
  }

  emit('submit', { status, body })
}
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
