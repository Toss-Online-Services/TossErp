<template>
  <div class="p-4 sm:p-6">
    <div class="flex items-center justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('sales.invoices.create.title') }}
        </h1>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ $t('sales.invoices.create.description') }}
        </p>
      </div>
    </div>

    <FormKit
      type="form"
      @submit="createInvoice"
      #default="{ value }"
      :actions="false"
    >
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Main Form -->
        <div class="lg:col-span-2 bg-white dark:bg-gray-800 shadow-sm rounded-lg p-6">
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-6">
            <FormKit
              type="select"
              name="customerId"
              :label="$t('sales.invoices.create.customer')"
              :placeholder="$t('sales.invoices.create.selectCustomer')"
              :options="customers"
              validation="required"
            />
            <FormKit
              type="select"
              name="orderId"
              :label="$t('sales.invoices.create.relatedOrder')"
              :placeholder="$t('sales.invoices.create.selectOrder')"
              :options="['SO-123', 'SO-124', 'SO-125']"
            />
            <FormKit
              type="date"
              name="issueDate"
              :label="$t('sales.invoices.create.issueDate')"
              validation="required"
              :value="new Date().toISOString().split('T')[0]"
            />
            <FormKit
              type="date"
              name="dueDate"
              :label="$t('sales.invoices.create.dueDate')"
              validation="required"
            />
          </div>

          <!-- Line Items -->
          <div class="mt-8">
            <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-4">{{ $t('common.lineItems') }}</h3>
            <FormKit type="list" name="items" :value="[{}]" dynamic #default="{ items, node, value }">
              <div v-for="(item, index) in items" :key="item" class="flex items-end gap-4 mb-4 p-4 border rounded-lg dark:border-gray-700">
                <div class="grid grid-cols-1 sm:grid-cols-5 gap-4 flex-grow">
                  <FormKit
                    type="select"
                    :name="`items[${index}].productId`"
                    :label="$t('common.product')"
                    :placeholder="$t('sales.invoices.create.selectProduct')"
                    :options="products"
                    outer-class="sm:col-span-2"
                  />
                  <FormKit type="number" :name="`items[${index}].quantity`" :label="$t('common.quantity')" value="1" min="1" />
                  <FormKit type="number" :name="`items[${index}].unitPrice`" :label="$t('common.unitPrice')" step="0.01" />
                  <div class="flex items-center sm:col-span-1">
                    <p class="text-sm font-medium text-gray-700 dark:text-gray-300 mt-6">
                      {{ formatCurrency(getLineTotal(value[index])) }}
                    </p>
                  </div>
                </div>
                <button type="button" @click="() => node.input(value.filter((_, i) => i !== index))" class="text-red-500 hover:text-red-700 mb-2">
                  <Icon name="heroicons:trash" class="w-5 h-5" />
                </button>
              </div>
              <button type="button" @click="() => node.input([...(value || []), {}])" class="btn btn-secondary">
                <Icon name="heroicons:plus" class="w-4 h-4 mr-2" />
                {{ $t('common.addItem') }}
              </button>
            </FormKit>
          </div>
        </div>

        <!-- Summary & Actions -->
        <div class="lg:col-span-1">
          <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg p-6 sticky top-6">
            <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-4">{{ $t('common.summary') }}</h3>
            <div class="space-y-3 text-sm">
              <div class="flex justify-between">
                <span class="text-gray-500 dark:text-gray-400">{{ $t('common.subtotal') }}:</span>
                <span class="font-medium text-gray-800 dark:text-gray-200">{{ formatCurrency(totals.subtotal) }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-gray-500 dark:text-gray-400">{{ $t('common.vat') }} (15%):</span>
                <span class="font-medium text-gray-800 dark:text-gray-200">{{ formatCurrency(totals.vat) }}</span>
              </div>
              <div class="flex justify-between text-lg font-bold">
                <span class="text-gray-900 dark:text-white">{{ $t('common.total') }}:</span>
                <span class="text-gray-900 dark:text-white">{{ formatCurrency(totals.total) }}</span>
              </div>
            </div>
            <div class="mt-6 space-y-2">
              <FormKit type="submit" :label="$t('sales.invoices.create.saveDraft')" name="save_draft" outer-class="!mb-0" />
              <FormKit type="submit" :label="$t('sales.invoices.create.saveAndSend')" name="save_send" outer-class="!mb-0" />
            </div>
          </div>
        </div>
      </div>
    </FormKit>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useFormKitContext } from '@formkit/vue'

const { t } = useI18n()
useHead({
  title: t('sales.invoices.create.pageTitle'),
})

// Mock Data
const customers = [
  { label: 'Jabu\'s Spaza', value: 'cust-001' },
  { label: 'Sipho\'s Tavern', value: 'cust-002' },
  { label: 'The Gogo Shop', value: 'cust-003' },
]
const products = [
  { label: 'Albany Bread', value: 'prod-001', price: 15.50 },
  { label: 'Clover Milk 1L', value: 'prod-002', price: 22.00 },
  { label: 'Huletts Sugar 2kg', value: 'prod-003', price: 45.75 },
]

const form = ref()
const formNode = computed(() => form.value?.node)

const totals = computed(() => {
  const items = formNode.value?.value?.items || []
  const subtotal = items.reduce((acc, item) => acc + getLineTotal(item), 0)
  const vat = subtotal * 0.15
  const total = subtotal + vat
  return { subtotal, vat, total }
})

watch(() => formNode.value?.value?.items, (newItems) => {
  if (newItems) {
    newItems.forEach((item, index) => {
      if (item.productId) {
        const product = products.find(p => p.value === item.productId)
        if (product && !item.unitPrice) {
          // This is tricky with FormKit's reactivity. A better way is to handle this in a change event handler.
          formNode.value?.at(`items.${index}.unitPrice`)?.input(product.price)
        }
      }
    })
  }
}, { deep: true })


const getLineTotal = (item: any) => {
  if (!item || !item.quantity || !item.unitPrice) return 0
  return item.quantity * item.unitPrice
}

const createInvoice = async (formData: any, node: any) => {
  const action = node.target.name
  console.log(`Action: ${action}`, formData)
  
  const status = action === 'save_draft' ? 'Draft' : 'Sent'
  const payload = { ...formData, status, totals: totals.value }

  alert(`${t('sales.invoices.create.invoice')} ${status}!\n\n${JSON.stringify(payload, null, 2)}`)
}

const formatCurrency = (value: number) => new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(value || 0)

onMounted(() => {
  form.value = document.querySelector('[data-formkit-type="form"]')
})
</script>

<style scoped>
.btn {
  @apply inline-flex items-center justify-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-offset-2;
}
.btn-secondary {
  @apply text-gray-700 bg-gray-100 hover:bg-gray-200 dark:bg-gray-700 dark:text-gray-200 dark:hover:bg-gray-600 focus:ring-gray-500;
}
</style>
