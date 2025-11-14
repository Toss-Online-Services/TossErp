<template>
  <form class="space-y-6" @submit.prevent="handleSubmit">
    <Alert variant="destructive" v-if="loadError">
      <AlertTitle>{{ t('common.error', 'Something went wrong') }}</AlertTitle>
      <AlertDescription>{{ loadError }}</AlertDescription>
    </Alert>

    <Card>
      <CardHeader>
        <CardTitle>{{ t('sales.quotations.form.customerInformation', 'Customer information') }}</CardTitle>
        <CardDescription>
          {{ t('sales.quotations.form.customerSubtitle', 'Select the customer this quotation is for') }}
        </CardDescription>
      </CardHeader>
      <CardContent class="space-y-4">
        <div class="grid gap-4 md:grid-cols-2">
          <div class="space-y-2">
            <Label for="quotation-customer">{{ t('sales.quotations.form.customer', 'Customer') }}</Label>
            <Select v-model="form.customerId">
              <SelectTrigger id="quotation-customer">
                <SelectValue :placeholder="customerPlaceholder" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem v-for="customer in customers" :key="customer.id" :value="customer.id">
                  {{ customer.name }}
                </SelectItem>
              </SelectContent>
            </Select>
          </div>
          <div class="space-y-2">
            <Label for="quotation-salesperson">{{ t('sales.quotations.form.salesPerson', 'Sales representative') }}</Label>
            <Input
              id="quotation-salesperson"
              v-model="form.salesPerson"
              :placeholder="t('sales.quotations.form.salesPersonPlaceholder', 'Optional')"
            />
          </div>
        </div>

        <div v-if="selectedCustomer" class="rounded-md border border-dashed border-muted-foreground/30 p-4 text-sm">
          <div class="font-medium text-foreground">{{ selectedCustomer.name }}</div>
          <div class="grid gap-2 pt-2 md:grid-cols-2">
            <div class="space-y-1">
              <div class="text-muted-foreground">{{ t('sales.quotations.form.contact', 'Contact') }}</div>
              <div>{{ selectedCustomer.email || t('sales.quotations.form.noEmail', 'No email on file') }}</div>
              <div>{{ selectedCustomer.phone || t('sales.quotations.form.noPhone', 'No phone on file') }}</div>
            </div>
            <div class="space-y-1">
              <div class="text-muted-foreground">{{ t('sales.quotations.form.billing', 'Billing address') }}</div>
              <div>{{ selectedCustomer.billingAddress || selectedCustomer.address || t('sales.quotations.form.noAddress', 'Address not provided') }}</div>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>

    <Card>
      <CardHeader>
        <CardTitle>{{ t('sales.quotations.form.details', 'Quotation details') }}</CardTitle>
        <CardDescription>{{ t('sales.quotations.form.detailsSubtitle', 'Set scheduling and reference details') }}</CardDescription>
      </CardHeader>
      <CardContent class="grid gap-4 md:grid-cols-2">
        <div class="space-y-2">
          <Label for="quotation-issue-date">{{ t('sales.quotations.form.issueDate', 'Issue date') }}</Label>
          <Input id="quotation-issue-date" type="date" v-model="form.issueDate" />
        </div>
        <div class="space-y-2">
          <Label for="quotation-valid-until">{{ t('sales.quotations.form.validUntil', 'Valid until') }}</Label>
          <Input id="quotation-valid-until" type="date" v-model="form.validUntil" />
        </div>
      </CardContent>
    </Card>

    <Card>
      <CardHeader class="flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
        <div>
          <CardTitle>{{ t('sales.quotations.form.lineItems', 'Line items') }}</CardTitle>
          <CardDescription>
            {{ t('sales.quotations.form.lineItemsSubtitle', 'Add the products or services included in this quotation') }}
          </CardDescription>
        </div>
        <Button type="button" variant="outline" size="sm" @click="addItem">
          {{ t('sales.quotations.form.addItem', 'Add line item') }}
        </Button>
      </CardHeader>
      <CardContent class="space-y-4">
        <Alert v-if="!lineItems.length" variant="warning">
          <AlertTitle>{{ t('sales.quotations.form.noItemsTitle', 'No line items yet') }}</AlertTitle>
          <AlertDescription>
            {{ t('sales.quotations.form.noItemsDescription', 'Add at least one item to create a quotation.') }}
          </AlertDescription>
        </Alert>

        <div
          v-for="(item, index) in lineItems"
          :key="item.key"
          class="rounded-lg border border-border p-4 space-y-4"
        >
          <div class="flex flex-col gap-4 md:flex-row md:items-start md:justify-between">
            <div class="flex-1 space-y-2">
              <Label>{{ t('sales.quotations.form.product', 'Product') }}</Label>
              <Select v-model="item.productId" @update:modelValue="value => handleProductChange(item, value)">
                <SelectTrigger>
                  <SelectValue :placeholder="t('sales.quotations.form.productPlaceholder', 'Select a product')" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem v-for="product in products" :key="product.id" :value="product.id">
                    {{ product.name }} · {{ formatCurrency(product.unitPrice) }}
                  </SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div class="flex-1 space-y-2">
              <Label>{{ t('sales.quotations.form.itemName', 'Line item name') }}</Label>
              <Input
                v-model="item.name"
                :placeholder="t('sales.quotations.form.itemNamePlaceholder', 'Enter a short description')"
              />
            </div>
            <div class="md:w-24 space-y-2">
              <Label>{{ t('sales.quotations.form.quantity', 'Quantity') }}</Label>
              <Input type="number" min="1" step="1" v-model.number="item.quantity" />
            </div>
            <div class="md:w-32 space-y-2">
              <Label>{{ t('sales.quotations.form.unitPrice', 'Unit price') }}</Label>
              <Input type="number" min="0" step="0.01" v-model.number="item.unitPrice" />
            </div>
          </div>

          <div class="grid gap-4 md:grid-cols-5 items-end">
            <div class="space-y-2">
              <Label>{{ t('sales.quotations.form.discount', 'Discount (%)') }}</Label>
              <Input type="number" min="0" step="0.01" v-model.number="item.discountRate" />
            </div>
            <div class="space-y-2">
              <Label>{{ t('sales.quotations.form.vat', 'VAT (%)') }}</Label>
              <Input type="number" min="0" step="0.01" v-model.number="item.vatRate" />
            </div>
            <div class="space-y-2 md:col-span-2">
              <Label>{{ t('sales.quotations.form.itemDescription', 'Internal description') }}</Label>
              <Input
                v-model="item.description"
                :placeholder="t('sales.quotations.form.itemDescriptionPlaceholder', 'Optional notes for this line item')"
              />
            </div>
            <div class="flex items-center justify-between gap-2 md:justify-end">
              <div class="text-right">
                <div class="text-xs text-muted-foreground">{{ t('sales.quotations.form.lineTotal', 'Line total') }}</div>
                <div class="font-medium text-foreground">{{ formatCurrency(lineTotal(item)) }}</div>
              </div>
              <Button
                type="button"
                variant="ghost"
                size="icon"
                :disabled="lineItems.length === 1"
                @click="removeItem(index)"
              >
                <Trash2 class="h-4 w-4" />
              </Button>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>

    <Card>
      <CardHeader>
        <CardTitle>{{ t('sales.quotations.form.notes', 'Terms & notes') }}</CardTitle>
        <CardDescription>
          {{ t('sales.quotations.form.notesSubtitle', 'Share any special terms or internal notes for your team') }}
        </CardDescription>
      </CardHeader>
      <CardContent class="grid gap-4 md:grid-cols-2">
        <div class="space-y-2">
          <Label for="quotation-terms">{{ t('sales.quotations.form.termsHeading', 'Terms and conditions') }}</Label>
          <Textarea
            id="quotation-terms"
            v-model="form.terms"
            rows="4"
            :placeholder="t('sales.quotations.form.termsPlaceholder', 'Payment terms, delivery expectations, etc.')"
          />
        </div>
        <div class="space-y-2">
          <Label for="quotation-notes">{{ t('sales.quotations.form.internalNotes', 'Internal notes') }}</Label>
          <Textarea
            id="quotation-notes"
            v-model="form.notes"
            rows="4"
            :placeholder="t('sales.quotations.form.internalNotesPlaceholder', 'Visible only to your team')"
          />
        </div>
      </CardContent>
    </Card>

    <div class="grid gap-4 md:grid-cols-2">
      <Card>
        <CardHeader>
          <CardTitle>{{ t('sales.quotations.form.summary', 'Summary totals') }}</CardTitle>
        </CardHeader>
        <CardContent class="space-y-3 text-sm">
          <div class="flex justify-between">
            <span class="text-muted-foreground">{{ t('sales.quotations.form.subtotal', 'Subtotal') }}</span>
            <span class="font-medium">{{ formatCurrency(formTotals.subtotal) }}</span>
          </div>
          <div class="flex justify-between">
            <span class="text-muted-foreground">{{ t('sales.quotations.form.discountAmount', 'Discounts') }}</span>
            <span class="font-medium text-destructive">-{{ formatCurrency(formTotals.discountAmount) }}</span>
          </div>
          <div class="flex justify-between">
            <span class="text-muted-foreground">{{ t('sales.quotations.form.vatAmount', 'VAT') }}</span>
            <span class="font-medium">{{ formatCurrency(formTotals.vatAmount) }}</span>
          </div>
          <div class="border-t border-border pt-3 flex justify-between text-base font-semibold">
            <span>{{ t('sales.quotations.form.grandTotal', 'Grand total') }}</span>
            <span>{{ formatCurrency(formTotals.total) }}</span>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>{{ t('sales.quotations.form.quickStats', 'Quick stats') }}</CardTitle>
        </CardHeader>
        <CardContent class="space-y-2 text-sm">
          <div class="flex justify-between">
            <span class="text-muted-foreground">{{ t('sales.quotations.form.itemsCount', 'Line items') }}</span>
            <span class="font-medium">{{ lineItems.length }}</span>
          </div>
          <div class="flex justify-between">
            <span class="text-muted-foreground">{{ t('sales.quotations.form.totalQuantity', 'Total quantity') }}</span>
            <span class="font-medium">{{ totalQuantity }}</span>
          </div>
          <div>
            <div class="text-muted-foreground">{{ t('sales.quotations.form.productsSummary', 'Products summary') }}</div>
            <ul v-if="productSummaries.length" class="mt-2 space-y-1">
              <li v-for="summary in productSummaries" :key="summary.name" class="flex justify-between">
                <span>{{ summary.name }}</span>
                <span class="text-muted-foreground">× {{ summary.quantity }}</span>
              </li>
            </ul>
            <p v-else class="mt-2 text-muted-foreground">
              {{ t('sales.quotations.form.noProductsSummary', 'Add line items to see a quick summary here.') }}
            </p>
          </div>
        </CardContent>
      </Card>
    </div>

    <div class="flex justify-end gap-2">
      <Button type="button" variant="outline" @click="handleCancel">
        {{ t('common.cancel', 'Cancel') }}
      </Button>
      <Button type="submit" :disabled="disableSubmit">
        <Loader2 v-if="isSubmitting" class="mr-2 h-4 w-4 animate-spin" />
        {{ submitLabel }}
      </Button>
    </div>
  </form>
</template>

<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { Loader2, Trash2 } from 'lucide-vue-next'

import { Alert, AlertDescription, AlertTitle } from '~/components/ui/alert'
import { Button } from '~/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Input } from '~/components/ui/input'
import { Label } from '~/components/ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '~/components/ui/select'
import { Textarea } from '~/components/ui/textarea'
import { useCustomers } from '~/composables/useCustomers'
import { useQuotations } from '~/composables/useQuotations'
import { useToast } from '~/composables/useToast'
import { mockStockItems } from '~/services/mock'
import type { Quotation, QuotationCustomer, SalesProductSummary } from '~/types/sales'

interface LineItemForm {
  key: string
  productId: string
  name: string
  description: string
  quantity: number
  unitPrice: number
  vatRate: number
  discountRate: number
}

const DEFAULT_VAT_RATE = 15
const { t } = useI18n()
const router = useRouter()
const toast = useToast()
const { getCustomers } = useCustomers()
const { createQuotation, updateQuotation, calculateTotals } = useQuotations()

const props = defineProps<{
  mode: 'create' | 'edit'
  quotation?: Quotation | null
}>()

const emit = defineEmits<{
  (e: 'submitted', quotation: Quotation): void
  (e: 'cancel'): void
}>()

const today = new Date()
const toDateInput = (date: Date) => date.toISOString().split('T')[0]
const defaultIssueDate = toDateInput(today)
const defaultValidUntil = toDateInput(new Date(today.getTime() + 14 * 86_400_000))

const form = reactive({
  customerId: '',
  salesPerson: '',
  issueDate: defaultIssueDate,
  validUntil: defaultValidUntil,
  terms: '',
  notes: ''
})

const lineItems = ref<LineItemForm[]>([])

const createEmptyItem = (): LineItemForm => ({
  key: Math.random().toString(36).slice(2),
  productId: '',
  name: '',
  description: '',
  quantity: 1,
  unitPrice: 0,
  vatRate: DEFAULT_VAT_RATE,
  discountRate: 0
})

const products = ref<SalesProductSummary[]>(
  mockStockItems.map((item) => ({
    id: String(item.id),
    sku: item.sku,
    name: item.name,
    description: `${item.category} · ${item.warehouseName}`,
    unitPrice: item.unitPrice,
    vatRate: DEFAULT_VAT_RATE,
    uom: 'unit',
    stockOnHand: item.currentStock
  }))
)

const productMap = computed(() => {
  const map = new Map<string, SalesProductSummary>()
  products.value.forEach((product) => map.set(product.id, product))
  return map
})

const customers = ref<QuotationCustomer[]>([])
const customersMap = computed(() => {
  const map = new Map<string, QuotationCustomer>()
  customers.value.forEach((customer) => map.set(customer.id, customer))
  return map
})

const selectedCustomer = computed(() => {
  return customersMap.value.get(form.customerId) ?? null
})

const isLoadingCustomers = ref(false)
const loadError = ref<string | null>(null)
const isSubmitting = ref(false)

const submitLabel = computed(() =>
  props.mode === 'edit'
    ? t('sales.quotations.form.updateAction', 'Update quotation')
    : t('sales.quotations.form.createAction', 'Create quotation')
)

const customerPlaceholder = computed(() =>
  isLoadingCustomers.value
    ? t('sales.quotations.form.loadingCustomers', 'Loading customers...')
    : t('sales.quotations.form.customerPlaceholder', 'Select a customer')
)

const formTotals = computed(() =>
  calculateTotals(
    lineItems.value.map((item) => ({
      quantity: item.quantity,
      unitPrice: item.unitPrice,
      discountRate: item.discountRate,
      taxRate: item.vatRate
    }))
  )
)

const totalQuantity = computed(() =>
  lineItems.value.reduce((sum, item) => sum + (Number(item.quantity) || 0), 0)
)

const productSummaries = computed(() => {
  const map = new Map<string, number>()
  lineItems.value.forEach((item) => {
    const name = item.name?.trim() || productMap.value.get(item.productId)?.name
    if (!name) return
    const current = map.get(name) ?? 0
    map.set(name, current + (Number(item.quantity) || 0))
  })
  return Array.from(map.entries()).map(([name, quantity]) => ({ name, quantity }))
})

const disableSubmit = computed(() => {
  const hasCustomer = Boolean(form.customerId)
  const hasValidLine = lineItems.value.some((item) => hasItemContent(item) && Number(item.quantity) > 0)
  return isSubmitting.value || !hasCustomer || !hasValidLine
})

const addItem = () => {
  lineItems.value.push(createEmptyItem())
}

const removeItem = (index: number) => {
  if (lineItems.value.length === 1) return
  lineItems.value.splice(index, 1)
}

const handleProductChange = (item: LineItemForm, productId?: string) => {
  item.productId = productId ?? ''
  const product = productMap.value.get(item.productId)
  if (product) {
    item.name = product.name
    item.unitPrice = product.unitPrice
    item.vatRate = product.vatRate ?? DEFAULT_VAT_RATE
    item.description = product.description ?? ''
  }
}

const formatCurrency = (amount: number) =>
  new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(amount ?? 0)

const lineTotal = (item: LineItemForm) => {
  const base = Number(item.quantity || 0) * Number(item.unitPrice || 0)
  const discount = (base * Number(item.discountRate || 0)) / 100
  const net = base - discount
  const vat = (net * Number(item.vatRate || DEFAULT_VAT_RATE)) / 100
  return net + vat
}

const hasItemContent = (item: LineItemForm) => {
  return Boolean(item.productId?.trim()) || Boolean(item.name?.trim())
}

const mapCustomer = (input: any): QuotationCustomer => ({
  id: String(input.id ?? input.customerId ?? input.fullName ?? input.name ?? Math.random().toString(36).slice(2)),
  name:
    input.fullName ??
    input.name ??
    [input.firstName, input.lastName].filter((value: string | undefined) => Boolean(value)).join(' ').trim() ||
    t('sales.quotations.form.unknownCustomer', 'Unknown customer'),
  email: input.email,
  phone: input.phone,
  address: input.address,
  billingAddress: input.billingAddress ?? input.address,
  shippingAddress: input.shippingAddress ?? input.address,
  territory: input.territory ?? '',
  customerGroup: input.segment ?? input.customerGroup ?? '',
  creditLimit: Number(input.creditLimit ?? input.totalSpent ?? 0),
  creditUsed: Number(input.creditUsed ?? 0),
  paymentTerms: Number(input.paymentTerms ?? 0),
  primaryContact: input.primaryContact ?? undefined
})

const ensureCustomerInList = (customer: QuotationCustomer) => {
  if (!customersMap.value.has(customer.id)) {
    customers.value = [...customers.value, customer]
  }
}

const resetForm = () => {
  form.customerId = ''
  form.salesPerson = ''
  form.issueDate = defaultIssueDate
  form.validUntil = defaultValidUntil
  form.terms = ''
  form.notes = ''
  lineItems.value = [createEmptyItem()]
}

const populateFromQuotation = (quotation: Quotation) => {
  ensureCustomerInList(quotation.customer)
  form.customerId = quotation.customer.id
  form.salesPerson = quotation.salesPerson ?? ''
  form.issueDate = quotation.date ?? defaultIssueDate
  form.validUntil = quotation.validUntil ?? defaultValidUntil
  form.terms = quotation.terms ?? ''
  form.notes = quotation.notes ?? ''
  lineItems.value = quotation.items.map((item) => ({
    key: item.id ?? Math.random().toString(36).slice(2),
    productId: item.productId ?? '',
    name: item.name,
    description: item.description ?? '',
    quantity: item.quantity,
    unitPrice: item.unitPrice,
    vatRate: item.vatRate ?? DEFAULT_VAT_RATE,
    discountRate: item.discountRate ?? 0
  }))
  if (!lineItems.value.length) {
    lineItems.value = [createEmptyItem()]
  }
}

const loadCustomers = async () => {
  isLoadingCustomers.value = true
  loadError.value = null
  try {
    const response = await getCustomers({ pageSize: 100 })
    const raw = Array.isArray(response)
      ? response
      : Array.isArray((response as any)?.data)
        ? (response as any).data
        : Array.isArray((response as any)?.customers)
          ? (response as any).customers
          : []
    const mapped = raw.map(mapCustomer)
    customers.value = Array.from(new Map(mapped.map((customer) => [customer.id, customer])).values())
    if (props.quotation) {
      ensureCustomerInList(props.quotation.customer)
    }
  } catch (error: any) {
    loadError.value = error?.message ?? t('sales.quotations.form.customerLoadFailed', 'Unable to load customers. Try again later.')
  } finally {
    isLoadingCustomers.value = false
  }
}

watch(
  () => props.quotation,
  (value) => {
    if (value) {
      populateFromQuotation(value)
    } else {
      resetForm()
    }
  }
)

watch(
  lineItems,
  (items) => {
    items.forEach((item) => {
      if (item.quantity < 1) item.quantity = 1
      if (item.unitPrice < 0) item.unitPrice = 0
      if (item.vatRate < 0) item.vatRate = 0
      if (item.discountRate < 0) item.discountRate = 0
    })
  },
  { deep: true }
)

onMounted(() => {
  resetForm()
  loadCustomers()
  if (props.quotation) {
    populateFromQuotation(props.quotation)
  }
})

const handleCancel = () => {
  emit('cancel')
  router.push('/sales/quotations')
}

const handleSubmit = async () => {
  if (disableSubmit.value || isSubmitting.value) {
    return
  }

  if (!selectedCustomer.value) {
    toast.error(t('sales.quotations.form.customerRequired', 'Please select a customer'))
    return
  }

  const validItems = lineItems.value.filter((item) => hasItemContent(item) && Number(item.quantity) > 0)

  if (!validItems.length) {
    toast.error(t('sales.quotations.form.itemsRequired', 'Add at least one line item with quantity greater than zero'))
    return
  }

  const customerPayload = JSON.parse(JSON.stringify(selectedCustomer.value)) as QuotationCustomer
  const itemsPayload = validItems.map((item, index) => {
    const product = productMap.value.get(item.productId)
    const resolvedName = item.name?.trim() || product?.name
    if (!resolvedName) {
      throw new Error(t('sales.quotations.form.itemNameRequired', 'Each line item needs a name'))
    }
    return {
      id: props.mode === 'edit' ? item.key : undefined,
      productId: item.productId || product?.sku || `manual-${index + 1}`,
      name: resolvedName,
      description: item.description || product?.description,
      quantity: item.quantity,
      unitPrice: item.unitPrice,
      discountRate: item.discountRate || undefined,
      vatRate: item.vatRate
    }
  })

  const payload: any = {
    customer: customerPayload,
    salesPerson: form.salesPerson || undefined,
    date: form.issueDate,
    validUntil: form.validUntil || undefined,
    terms: form.terms || undefined,
    notes: form.notes || undefined,
    vatRate: DEFAULT_VAT_RATE,
    items: itemsPayload
  }

  isSubmitting.value = true
  try {
    const quotation = props.mode === 'edit' && props.quotation
      ? await updateQuotation(props.quotation.id, payload)
      : await createQuotation(payload)
    emit('submitted', quotation)
  } catch (error: any) {
    toast.error(error?.message ?? t('sales.quotations.form.submitFailed', 'Unable to save quotation'))
  } finally {
    isSubmitting.value = false
  }
}
</script>
