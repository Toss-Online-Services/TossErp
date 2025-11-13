<template>
  <div class="space-y-6 p-4 sm:p-6">
    <Alert variant="destructive" v-if="fetchError">
      <AlertTitle>{{ t('common.error', 'Something went wrong') }}</AlertTitle>
      <AlertDescription>{{ errorMessage }}</AlertDescription>
    </Alert>

    <div v-if="pending">
      <Skeleton class="h-8 w-64" />
      <div class="flex flex-wrap gap-2">
        <Skeleton class="h-9 w-24" />
        <Skeleton class="h-9 w-24" />
        <Skeleton class="h-9 w-28" />
        <Skeleton class="h-9 w-28" />
        <Skeleton class="h-9 w-28" />
      </div>
      <div class="grid gap-4 lg:grid-cols-3">
        <Skeleton class="h-64 w-full lg:col-span-2" />
        <Skeleton class="h-64 w-full" />
      </div>
    </div>

    <div v-else-if="quotation">
      <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div class="space-y-1">
          <div class="flex flex-wrap items-center gap-3">
            <h1 class="text-2xl font-semibold tracking-tight text-foreground">
              {{ t('sales.quotations.details.title', 'Quotation') }}
              #{{ quotation.number || quotation.id }}
            </h1>
            <Badge v-if="quotation.convertedToOrderId" variant="secondary">
              {{ t('sales.quotations.details.convertedBadge', 'Converted') }}
            </Badge>
          </div>
          <div class="flex flex-wrap gap-3 text-sm text-muted-foreground">
            <span>
              {{ t('sales.quotations.details.issuedOn', 'Issued on {date}', { date: formatDate(quotation.date) }) }}
            </span>
            <span>
              {{ t('sales.quotations.details.validUntil', 'Valid until {date}', { date: formatDate(quotation.validUntil) }) }}
            </span>
            <span v-if="quotation.lastSentAt">
              {{ t('sales.quotations.details.lastSent', 'Last sent {date}', { date: formatDateTime(quotation.lastSentAt) }) }}
            </span>
          </div>
        </div>
        <div class="flex items-center gap-2">
          <Badge :variant="statusBadgeVariant(quotation.status)">
            {{ statusLabel(quotation.status) }}
          </Badge>
          <DropdownMenu>
            <DropdownMenuTrigger as-child>
              <Button variant="outline" size="icon" :disabled="statusUpdating !== null">
                <MoreHorizontal class="h-4 w-4" />
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent class="w-48">
              <DropdownMenuLabel>{{ t('sales.quotations.details.changeStatus', 'Change status') }}</DropdownMenuLabel>
              <DropdownMenuSeparator />
              <DropdownMenuItem
                v-for="option in statusOptions"
                :key="option.value"
                :disabled="option.value === quotation.status || statusUpdating === option.value"
                @click="handleStatusChange(option.value)"
              >
                <Loader2 v-if="statusUpdating === option.value" class="mr-2 h-4 w-4 animate-spin" />
                <Circle v-else class="mr-2 h-4 w-4" />
                <span>{{ option.label }}</span>
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </div>

      <div class="flex flex-wrap gap-2">
        <Button variant="secondary" size="sm" @click="handleEdit" :disabled="actionBusy">
          <Pencil class="mr-2 h-4 w-4" />
          {{ t('common.edit', 'Edit') }}
        </Button>
        <Dialog v-model:open="sendDialogOpen">
          <DialogTrigger as-child>
            <Button variant="secondary" size="sm" :disabled="actionBusy">
              <Send class="mr-2 h-4 w-4" />
              {{ t('common.send', 'Send') }}
            </Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>{{ t('sales.quotations.email.title', 'Send quotation via email') }}</DialogTitle>
              <DialogDescription>{{ t('sales.quotations.email.description', 'Complete the details below to email this quotation to the customer.') }}</DialogDescription>
            </DialogHeader>
            <div class="space-y-4 py-2">
              <div class="space-y-2">
                <Label for="email-to">{{ t('sales.quotations.email.to', 'Recipient') }}</Label>
                <Input id="email-to" v-model="emailForm.to" type="email" :placeholder="t('sales.quotations.email.toPlaceholder', 'customer@example.com')" />
              </div>
              <div class="space-y-2">
                <Label for="email-subject">{{ t('sales.quotations.email.subject', 'Subject') }}</Label>
                <Input id="email-subject" v-model="emailForm.subject" />
              </div>
              <div class="space-y-2">
                <Label for="email-message">{{ t('sales.quotations.email.message', 'Message') }}</Label>
                <Textarea id="email-message" v-model="emailForm.message" :rows="6" />
              </div>
            </div>
            <DialogFooter>
              <Button variant="outline" @click="sendDialogOpen = false">
                {{ t('common.cancel', 'Cancel') }}
              </Button>
              <Button :disabled="isSendingEmail || !canSendEmail" @click="handleSendEmail">
                <Loader2 v-if="isSendingEmail" class="mr-2 h-4 w-4 animate-spin" />
                {{ t('sales.quotations.email.sendAction', 'Send quotation') }}
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
        <Button variant="secondary" size="sm" :disabled="isConverting || actionBusy" @click="handleConvert">
          <Loader2 v-if="isConverting" class="mr-2 h-4 w-4 animate-spin" />
          <ArrowRight class="mr-2 h-4 w-4" v-else />
          {{ t('sales.quotations.details.convertToOrder', 'Convert to order') }}
        </Button>
        <Button variant="outline" size="sm" :disabled="isPdfDownloading || actionBusy" @click="handleDownloadPdf">
          <Loader2 v-if="isPdfDownloading" class="mr-2 h-4 w-4 animate-spin" />
          <Download v-else class="mr-2 h-4 w-4" />
          {{ t('sales.quotations.details.downloadPdf', 'Download PDF') }}
        </Button>
        <AlertDialog v-model:open="deleteDialogOpen">
          <AlertDialogTrigger as-child>
            <Button variant="destructive" size="sm" :disabled="isDeleting">
              <Loader2 v-if="isDeleting" class="mr-2 h-4 w-4 animate-spin" />
              <Trash2 v-else class="mr-2 h-4 w-4" />
              {{ t('common.delete', 'Delete') }}
            </Button>
          </AlertDialogTrigger>
          <AlertDialogContent>
            <AlertDialogHeader>
              <AlertDialogTitle>{{ t('sales.quotations.delete.title', 'Delete quotation?') }}</AlertDialogTitle>
              <AlertDialogDescription>
                {{ t('sales.quotations.delete.description', 'This action cannot be undone. The quotation will be removed permanently.') }}
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogCancel :disabled="isDeleting">{{ t('common.cancel', 'Cancel') }}</AlertDialogCancel>
              <AlertDialogAction :disabled="isDeleting" @click="handleDelete">
                <Loader2 v-if="isDeleting" class="mr-2 h-4 w-4 animate-spin" />
                {{ t('sales.quotations.delete.confirm', 'Delete quotation') }}
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
        <Button variant="ghost" size="sm" @click="refresh" :disabled="pending">
          <RefreshCcw class="mr-2 h-4 w-4" />
          {{ t('common.refresh', 'Refresh') }}
        </Button>
      </div>

      <div class="grid gap-6 lg:grid-cols-3">
        <Card class="lg:col-span-2">
          <CardHeader>
            <CardTitle>{{ t('sales.quotations.details.lineItems', 'Line items') }}</CardTitle>
            <CardDescription>{{ t('sales.quotations.details.lineItemsSubtitle', 'Products included in this quotation') }}</CardDescription>
          </CardHeader>
          <CardContent>
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>{{ t('sales.quotations.create.product', 'Product') }}</TableHead>
                  <TableHead class="text-right">{{ t('sales.quotations.create.quantity', 'Qty') }}</TableHead>
                  <TableHead class="text-right">{{ t('sales.quotations.create.unitPrice', 'Unit price') }}</TableHead>
                  <TableHead class="text-right">{{ t('sales.quotations.create.total', 'Total') }}</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                <TableRow v-for="item in quotation.items" :key="item.id">
                  <TableCell>
                    <div class="flex flex-col">
                      <span class="font-medium text-foreground">{{ item.name }}</span>
                      <span v-if="item.description" class="text-xs text-muted-foreground">{{ item.description }}</span>
                    </div>
                  </TableCell>
                  <TableCell class="text-right">{{ item.quantity }}</TableCell>
                  <TableCell class="text-right">{{ formatCurrency(item.unitPrice) }}</TableCell>
                  <TableCell class="text-right">{{ formatCurrency(item.total) }}</TableCell>
                </TableRow>
              </TableBody>
            </Table>
          </CardContent>
        </Card>

        <div class="space-y-6">
          <Card>
            <CardHeader>
              <CardTitle>{{ t('sales.quotations.details.summary', 'Summary') }}</CardTitle>
            </CardHeader>
            <CardContent class="space-y-3">
              <div class="flex items-center justify-between text-sm">
                <span class="text-muted-foreground">{{ t('sales.quotations.create.subtotal', 'Subtotal') }}</span>
                <span class="font-medium">{{ formatCurrency(quotation.subtotal) }}</span>
              </div>
              <div class="flex items-center justify-between text-sm">
                <span class="text-muted-foreground">
                  {{ t('sales.quotations.create.discount', 'Discount') }}
                  <span v-if="quotation.discountRate">({{ quotation.discountRate }}%)</span>
                </span>
                <span class="font-medium text-destructive">-{{ formatCurrency(quotation.discountAmount) }}</span>
              </div>
              <div class="flex items-center justify-between text-sm">
                <span class="text-muted-foreground">
                  {{ t('sales.quotations.create.vat', 'VAT') }}
                  <span>({{ quotation.vatRate }}%)</span>
                </span>
                <span class="font-medium">{{ formatCurrency(quotation.vatAmount) }}</span>
              </div>
              <Separator />
              <div class="flex items-center justify-between text-base font-semibold">
                <span>{{ t('sales.quotations.create.grandTotal', 'Grand total') }}</span>
                <span>{{ formatCurrency(quotation.grandTotal) }}</span>
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>{{ t('sales.quotations.details.customer', 'Customer information') }}</CardTitle>
              <CardDescription>{{ t('sales.quotations.details.customerSubtitle', 'Primary contact and billing details') }}</CardDescription>
            </CardHeader>
            <CardContent class="space-y-3 text-sm">
              <div>
                <p class="font-medium text-foreground">{{ quotation.customer.name }}</p>
                <p v-if="quotation.customer.email" class="text-muted-foreground">{{ quotation.customer.email }}</p>
                <p v-if="quotation.customer.phone" class="text-muted-foreground">{{ quotation.customer.phone }}</p>
              </div>
              <div v-if="quotation.customer.address">
                <Label class="text-xs uppercase text-muted-foreground">{{ t('sales.quotations.details.billingAddress', 'Billing address') }}</Label>
                <p>{{ quotation.customer.address }}</p>
              </div>
              <div v-if="quotation.customer.shippingAddress">
                <Label class="text-xs uppercase text-muted-foreground">{{ t('sales.quotations.details.shippingAddress', 'Shipping address') }}</Label>
                <p>{{ quotation.customer.shippingAddress }}</p>
              </div>
              <div class="grid grid-cols-2 gap-2 text-xs text-muted-foreground" v-if="quotation.customer.creditLimit">
                <div>
                  <span class="block font-medium text-foreground">
                    {{ t('sales.quotations.details.creditLimit', 'Credit limit') }}
                  </span>
                  <span>{{ formatCurrency(quotation.customer.creditLimit || 0) }}</span>
                </div>
                <div>
                  <span class="block font-medium text-foreground">
                    {{ t('sales.quotations.details.creditUsed', 'Credit used') }}
                  </span>
                  <span>{{ formatCurrency(quotation.customer.creditUsed || 0) }}</span>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>

      <div class="grid gap-6 lg:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>{{ t('sales.quotations.create.termsAndConditions', 'Terms and conditions') }}</CardTitle>
          </CardHeader>
          <CardContent>
            <p class="whitespace-pre-line text-sm text-muted-foreground">
              {{ quotation.terms || t('sales.quotations.details.noTerms', 'No terms provided.') }}
            </p>
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle>{{ t('sales.quotations.create.internalNotes', 'Internal notes') }}</CardTitle>
          </CardHeader>
          <CardContent>
            <p class="whitespace-pre-line text-sm text-muted-foreground">
              {{ quotation.notes || t('sales.quotations.details.noNotes', 'No internal notes recorded.') }}
            </p>
          </CardContent>
        </Card>
      </div>
    </div>

    <div v-else>
      <Card>
        <CardHeader>
          <CardTitle>{{ t('sales.quotations.details.notFoundTitle', 'Quotation not found') }}</CardTitle>
          <CardDescription>
            {{ t('sales.quotations.details.notFoundDescription', 'The quotation you are looking for may have been removed or is unavailable.') }}
          </CardDescription>
        </CardHeader>
        <CardContent>
          <Button @click="router.push('/sales/quotations')">
            {{ t('common.goBack', 'Back to quotations') }}
          </Button>
        </CardContent>
      </Card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAsyncData } from '#app'
import { useI18n } from 'vue-i18n'
import { ArrowRight, Circle, Download, Loader2, MoreHorizontal, Pencil, RefreshCcw, Send, Trash2 } from 'lucide-vue-next'

import { Button } from '~/components/ui/button'
import { Badge } from '~/components/ui/badge'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '~/components/ui/table'
import { Alert, AlertDescription, AlertTitle } from '~/components/ui/alert'
import { Skeleton } from '~/components/ui/skeleton'
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuLabel, DropdownMenuSeparator, DropdownMenuTrigger } from '~/components/ui/dropdown-menu'
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle, DialogTrigger } from '~/components/ui/dialog'
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle, AlertDialogTrigger } from '~/components/ui/alert-dialog'
import { Input } from '~/components/ui/input'
import { Textarea } from '~/components/ui/textarea'
import { Label } from '~/components/ui/label'
import { Separator } from '~/components/ui/separator'

import { useQuotations } from '~/composables/useQuotations'
import { useToast } from '~/composables/useToast'
import type { Quotation, QuotationStatus } from '~/types/sales'

const { t } = useI18n()
const route = useRoute()
const router = useRouter()
const quotationId = computed(() => String(route.params.id))

const { fetchQuotation, deleteQuotation, convertToSalesOrder, generatePDF, sendEmail, changeStatus } = useQuotations()
const toast = useToast()

const sendDialogOpen = ref(false)
const deleteDialogOpen = ref(false)
const isSendingEmail = ref(false)
const isConverting = ref(false)
const isPdfDownloading = ref(false)
const isDeleting = ref(false)
const statusUpdating = ref<QuotationStatus | null>(null)

const emailForm = reactive({
  to: '',
  subject: '',
  message: ''
})

const { data, pending, error: fetchError, refresh } = await useAsyncData<Quotation | null>(
  () => fetchQuotation(quotationId.value),
  {
    server: false,
    watch: [quotationId]
  }
)

const quotation = computed(() => data.value)

watch(
  quotation,
  (value) => {
    if (!value) return
    emailForm.to = value.customer.email || emailForm.to
    emailForm.subject = t('sales.quotations.email.subjectTemplate', 'Quotation {number}', {
      number: value.number || value.id
    })
    emailForm.message = t(
      'sales.quotations.email.defaultMessage',
      'Hi {name},\n\nPlease find your quotation attached.\n\nKind regards,\nTOSS Team',
      { name: value.customer.name }
    )
  },
  { immediate: true }
)

const statusOptions = computed<Array<{ value: QuotationStatus; label: string }>>(() => [
  { value: 'draft', label: t('sales.quotations.status.draft', 'Draft') },
  { value: 'sent', label: t('sales.quotations.status.sent', 'Sent') },
  { value: 'accepted', label: t('sales.quotations.status.accepted', 'Accepted') },
  { value: 'rejected', label: t('sales.quotations.status.rejected', 'Rejected') },
  { value: 'expired', label: t('sales.quotations.status.expired', 'Expired') },
  { value: 'converted', label: t('sales.quotations.status.converted', 'Converted') }
])

const statusLabel = (status: QuotationStatus) => {
  const option = statusOptions.value.find((item) => item.value === status)
  return option?.label ?? status
}

const statusBadgeVariant = (status: QuotationStatus) => {
  switch (status) {
    case 'draft':
      return 'outline'
    case 'sent':
      return 'secondary'
    case 'accepted':
    case 'converted':
      return 'default'
    case 'rejected':
    case 'expired':
      return 'destructive'
    default:
      return 'secondary'
  }
}

const actionBusy = computed(() =>
  isSendingEmail.value ||
  isConverting.value ||
  isPdfDownloading.value ||
  isDeleting.value ||
  statusUpdating.value !== null
)

const canSendEmail = computed(() => Boolean(emailForm.to && emailForm.subject && emailForm.message))

const errorMessage = computed(
  () => (fetchError.value as Error | null)?.message ?? t('errors.unexpectedError', 'Unexpected error loading the quotation.')
)

const formatDate = (value: string) =>
  new Date(value).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })

const formatDateTime = (value: string) =>
  new Date(value).toLocaleString('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })

const formatCurrency = (amount: number) =>
  new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(amount ?? 0)

const handleEdit = () => {
  router.push(`/sales/quotations/${quotationId.value}/edit`)
}

const handleStatusChange = async (nextStatus: QuotationStatus) => {
  if (!quotation.value || quotation.value.status === nextStatus) return
  statusUpdating.value = nextStatus
  try {
    await changeStatus(quotationId.value, nextStatus)
    await fetchQuotation(quotationId.value)
    toast.success(t('sales.quotations.status.updated', 'Quotation status updated'))
  } catch (err: any) {
    toast.error(err?.message ?? t('sales.quotations.status.updateFailed', 'Could not update quotation status'))
  } finally {
    statusUpdating.value = null
  }
}

const handleSendEmail = async () => {
  if (!quotation.value || !canSendEmail.value) return
  isSendingEmail.value = true
  try {
    await sendEmail(quotationId.value, {
      to: emailForm.to,
      subject: emailForm.subject,
      message: emailForm.message
    })
    toast.success(t('sales.quotations.email.success', 'Quotation emailed successfully'))
    sendDialogOpen.value = false
  } catch (err: any) {
    toast.error(err?.message ?? t('sales.quotations.email.failed', 'Unable to send quotation email'))
  } finally {
    isSendingEmail.value = false
  }
}

const handleConvert = async () => {
  if (!quotation.value) return
  isConverting.value = true
  try {
    const order = await convertToSalesOrder(quotationId.value)
    toast.success(
      t('sales.quotations.converted', 'Quotation converted to order {order}', {
        order: order.number || order.id
      })
    )
    await router.push(`/sales/orders/${order.id}`)
  } catch (err: any) {
    toast.error(err?.message ?? t('sales.quotations.convertFailed', 'Unable to convert quotation'))
  } finally {
    isConverting.value = false
  }
}

const handleDownloadPdf = async () => {
  if (!quotation.value) return
  isPdfDownloading.value = true
  try {
    const pdf = await generatePDF(quotationId.value)
    const blob = base64ToBlob(pdf.base64, 'application/pdf')
    const url = URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = pdf.filename || `quotation-${quotation.value.number || quotation.value.id}.pdf`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    URL.revokeObjectURL(url)
    toast.success(t('sales.quotations.pdf.success', 'Quotation PDF downloaded'))
  } catch (err: any) {
    toast.error(err?.message ?? t('sales.quotations.pdf.failed', 'Unable to download quotation PDF'))
  } finally {
    isPdfDownloading.value = false
  }
}

const handleDelete = async () => {
  if (!quotation.value) return
  isDeleting.value = true
  try {
    await deleteQuotation(quotationId.value)
    toast.success(t('sales.quotations.delete.success', 'Quotation deleted successfully'))
    await router.push('/sales/quotations')
  } catch (err: any) {
    toast.error(err?.message ?? t('sales.quotations.delete.failed', 'Unable to delete quotation'))
  } finally {
    isDeleting.value = false
  }
}

const base64ToBlob = (base64: string, type: string) => {
  const binary = atob(base64)
  const len = binary.length
  const buffer = new Uint8Array(len)
  for (let i = 0; i < len; i += 1) {
    buffer[i] = binary.charCodeAt(i)
  }
  return new Blob([buffer], { type })
}
</script>
