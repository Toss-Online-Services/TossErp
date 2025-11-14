<template>
  <div class="p-4 sm:p-6 space-y-6">
    <div class="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <Button variant="ghost" size="sm" class="w-fit px-0 text-muted-foreground" @click="goBack">
          <ArrowLeft class="mr-2 h-4 w-4" />
          {{ t('common.back', 'Back to quotations') }}
        </Button>
        <div>
          <h1 class="text-2xl font-bold tracking-tight text-foreground">
            {{ t('sales.quotations.details.title', 'Quotation') }}
            <span v-if="quotation">#{{ quotation.number ?? quotation.id }}</span>
          </h1>
          <p class="text-sm text-muted-foreground" v-if="quotation">
            {{ t('sales.quotations.details.issuedOn', 'Issued on') }}
            {{ formatDate(quotation.date) }}
          </p>
        </div>
      </div>
      <div class="flex flex-col items-start gap-3 sm:items-end">
        <Badge v-if="quotation" :variant="statusVariant(quotation.status)">
          {{ statusLabel(quotation.status) }}
        </Badge>
        <div class="flex flex-wrap gap-2">
          <Button variant="outline" size="sm" :disabled="pending || !quotation" @click="navigateToEdit">
            <PenSquare class="mr-2 h-4 w-4" />
            {{ t('common.edit') }}
          </Button>
          <Dialog v-model:open="emailDialogOpen">
            <DialogTrigger as-child>
              <Button variant="secondary" size="sm" :disabled="pending || !quotation">
                <Mail class="mr-2 h-4 w-4" />
                {{ t('common.send') }}
              </Button>
            </DialogTrigger>
            <DialogContent class="sm:max-w-lg">
              <DialogHeader>
                <DialogTitle>{{ t('sales.quotations.details.emailTitle', 'Email quotation') }}</DialogTitle>
                <DialogDescription>
                  {{ t('sales.quotations.details.emailDescription', 'Send this quotation to the customer.') }}
                </DialogDescription>
              </DialogHeader>
              <div class="space-y-4 py-2">
                <div class="space-y-2">
                  <Label for="quotation-email-to">{{ t('common.to', 'To') }}</Label>
                  <Input id="quotation-email-to" v-model="emailForm.to" type="email" required />
                </div>
                <div class="space-y-2">
                  <Label for="quotation-email-subject">{{ t('common.subject', 'Subject') }}</Label>
                  <Input id="quotation-email-subject" v-model="emailForm.subject" required />
                </div>
                <div class="space-y-2">
                  <Label for="quotation-email-message">{{ t('common.message', 'Message') }}</Label>
                  <Textarea id="quotation-email-message" v-model="emailForm.message" rows="6" />
                </div>
              </div>
              <DialogFooter>
                <Button variant="secondary" @click="emailDialogOpen = false">
                  {{ t('common.cancel', 'Cancel') }}
                </Button>
                <Button :disabled="emailSubmitting" @click="handleSendEmail">
                  <Loader2 v-if="emailSubmitting" class="mr-2 h-4 w-4 animate-spin" />
                  {{ t('sales.quotations.details.sendEmail', 'Send email') }}
                </Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>
          <Button size="sm" :disabled="pending || !quotation || convertLoading" @click="handleConvertToOrder">
            <Loader2 v-if="convertLoading" class="mr-2 h-4 w-4 animate-spin" />
            <ArrowRightCircle v-else class="mr-2 h-4 w-4" />
            {{ t('sales.quotations.details.convertToOrder', 'Convert to order') }}
          </Button>
          <Button variant="outline" size="sm" :disabled="pending || !quotation || pdfLoading" @click="handleDownloadPdf">
            <Loader2 v-if="pdfLoading" class="mr-2 h-4 w-4 animate-spin" />
            <FileDown v-else class="mr-2 h-4 w-4" />
            {{ t('sales.quotations.details.downloadPdf', 'Download PDF') }}
          </Button>
          <AlertDialog v-model:open="deleteDialogOpen">
            <AlertDialogTrigger as-child>
              <Button variant="destructive" size="sm" :disabled="pending || !quotation">
                <Trash2 class="mr-2 h-4 w-4" />
                {{ t('common.delete') }}
              </Button>
            </AlertDialogTrigger>
            <AlertDialogContent>
              <AlertDialogHeader>
                <AlertDialogTitle>{{ t('sales.quotations.details.deleteTitle', 'Delete quotation?') }}</AlertDialogTitle>
                <AlertDialogDescription>
                  {{ t('sales.quotations.details.deleteDescription', 'This action cannot be undone.') }}
                </AlertDialogDescription>
              </AlertDialogHeader>
              <AlertDialogFooter>
                <AlertDialogCancel>{{ t('common.cancel', 'Cancel') }}</AlertDialogCancel>
                <AlertDialogAction :disabled="deleteLoading" @click="handleDeleteQuotation">
                  <Loader2 v-if="deleteLoading" class="mr-2 h-4 w-4 animate-spin" />
                  {{ t('common.delete') }}
                </AlertDialogAction>
              </AlertDialogFooter>
            </AlertDialogContent>
          </AlertDialog>
        </div>
      </div>
    </div>

    <Alert variant="destructive" v-if="errorMessage">
      <AlertTitle>{{ t('common.error', 'Error') }}</AlertTitle>
      <AlertDescription>{{ errorMessage }}</AlertDescription>
    </Alert>

    <Card v-if="pending">
      <CardContent class="space-y-6 p-6">
        <Skeleton class="h-6 w-48" />
        <Skeleton class="h-20 w-full" />
        <div class="space-y-3">
          <Skeleton class="h-5 w-32" />
          <Skeleton class="h-32 w-full" />
        </div>
        <Skeleton class="h-5 w-32" />
        <Skeleton class="h-24 w-full" />
      </CardContent>
    </Card>

    <Card v-else-if="quotation">
      <CardContent class="space-y-8 pt-6">
        <div class="grid gap-6 md:grid-cols-2">
          <div class="space-y-2">
            <CardTitle>{{ t('sales.quotations.details.billTo', 'Bill to') }}</CardTitle>
            <p class="text-sm text-muted-foreground">{{ quotation.customer.name }}</p>
            <p v-if="quotation.customer.address" class="text-sm text-muted-foreground">
              {{ quotation.customer.address }}
            </p>
            <p v-if="quotation.customer.email" class="text-sm text-muted-foreground">
              {{ quotation.customer.email }}
            </p>
          </div>
          <div class="space-y-2 text-sm text-muted-foreground md:text-right">
            <CardTitle>{{ t('sales.quotations.details.company', 'Company') }}</CardTitle>
            <p>TOSS Inc.</p>
            <p>123 Biz Street, Johannesburg, 2000</p>
            <p>info@toss.co.za</p>
            <p>VAT: 4123456789</p>
          </div>
        </div>

        <div>
          <CardTitle class="mb-3">
            {{ t('sales.quotations.details.items', 'Line items') }}
          </CardTitle>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>{{ t('sales.quotations.create.product', 'Product') }}</TableHead>
                <TableHead class="text-right">{{ t('sales.quotations.create.quantity', 'Quantity') }}</TableHead>
                <TableHead class="text-right">{{ t('sales.quotations.create.unitPrice', 'Unit price') }}</TableHead>
                <TableHead class="text-right">{{ t('sales.quotations.create.total', 'Total') }}</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              <TableRow v-for="item in quotation.items" :key="item.id ?? item.productId">
                <TableCell class="font-medium">{{ item.name }}</TableCell>
                <TableCell class="text-right">{{ item.quantity }}</TableCell>
                <TableCell class="text-right">{{ formatCurrency(item.unitPrice) }}</TableCell>
                <TableCell class="text-right">{{ formatCurrency(item.total ?? item.quantity * item.unitPrice) }}</TableCell>
              </TableRow>
            </TableBody>
          </Table>
        </div>

        <div class="flex justify-end">
          <div class="w-full max-w-sm space-y-2">
            <div class="flex items-center justify-between text-sm">
              <span class="text-muted-foreground">{{ t('sales.quotations.create.subtotal', 'Subtotal') }}</span>
              <span class="font-medium">{{ formatCurrency(quotation.subtotal ?? 0) }}</span>
            </div>
            <div class="flex items-center justify-between text-sm">
              <span class="text-muted-foreground">
                {{ t('sales.quotations.create.discount', 'Discount') }}
                <template v-if="quotation.discountRate"> ({{ quotation.discountRate }}%)</template>
              </span>
              <span class="font-medium">-{{ formatCurrency(quotation.discountAmount ?? 0) }}</span>
            </div>
            <div class="flex items-center justify-between text-sm">
              <span class="text-muted-foreground">
                {{ t('sales.quotations.create.vat', 'VAT') }}
                <template v-if="quotation.vatRate"> ({{ quotation.vatRate }}%)</template>
              </span>
              <span class="font-medium">{{ formatCurrency(quotation.vatAmount ?? 0) }}</span>
            </div>
            <Separator />
            <div class="flex items-center justify-between text-lg font-semibold">
              <span>{{ t('sales.quotations.create.grandTotal', 'Grand total') }}</span>
              <span>{{ formatCurrency(quotation.grandTotal ?? 0) }}</span>
            </div>
          </div>
        </div>

        <Separator />

        <div class="grid gap-6 md:grid-cols-2">
          <div>
            <CardTitle class="mb-2 text-sm font-semibold">
              {{ t('sales.quotations.create.termsAndConditions', 'Terms & conditions') }}
            </CardTitle>
            <p class="text-sm text-muted-foreground whitespace-pre-wrap">
              {{ quotation.terms || t('sales.quotations.details.noTerms', 'No terms provided.') }}
            </p>
          </div>
          <div>
            <CardTitle class="mb-2 text-sm font-semibold">
              {{ t('sales.quotations.create.internalNotes', 'Internal notes') }}
            </CardTitle>
            <p class="text-sm text-muted-foreground whitespace-pre-wrap">
              {{ quotation.notes || t('sales.quotations.details.noNotes', 'No notes recorded.') }}
            </p>
          </div>
        </div>
      </CardContent>
    </Card>

    <Card v-else>
      <CardContent class="p-6 text-sm text-muted-foreground">
        {{ t('sales.quotations.details.notFound', 'Quotation not found or has been removed.') }}
      </CardContent>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { computed, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAsyncData } from '#app'
import { useHead, useI18n } from '#imports'
import type { Quotation, QuotationStatus } from '~/types/sales'
import { useQuotations } from '~/composables/useQuotations'
import { useToast } from '~/components/ui/use-toast'
import {
  ArrowLeft,
  ArrowRightCircle,
  FileDown,
  Loader2,
  Mail,
  PenSquare,
  Trash2
} from 'lucide-vue-next'
import { Button } from '~/components/ui/button'
import { Card, CardContent, CardTitle } from '~/components/ui/card'
import { Badge } from '~/components/ui/badge'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '~/components/ui/table'
import { Separator } from '~/components/ui/separator'
import { Alert, AlertDescription, AlertTitle } from '~/components/ui/alert'
import { Skeleton } from '~/components/ui/skeleton'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger
} from '~/components/ui/dialog'
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger
} from '~/components/ui/alert-dialog'
import { Input } from '~/components/ui/input'
import { Label } from '~/components/ui/label'
import { Textarea } from '~/components/ui/textarea'

interface EmailPayload {
  to: string
  subject: string
  message: string
}

const { t } = useI18n()
const route = useRoute()
const router = useRouter()
const { toast } = useToast()
const quotationId = computed(() => route.params.id as string)

const {
  fetchQuotation,
  fetchStats,
  generatePDF,
  sendEmail,
  convertToSalesOrder,
  deleteQuotation
} = useQuotations()

const {
  data,
  pending,
  error,
  refresh
} = await useAsyncData<Quotation | null>(
  () => fetchQuotation(quotationId.value),
  {
    watch: [quotationId]
  }
)

const quotation = computed(() => data.value ?? null)

watch(
  () => quotation.value,
  (value) => {
    if (!value) {
      return
    }
    emailForm.to = value.customer.email ?? ''
    emailForm.subject = t('sales.quotations.details.emailSubject', 'Quotation {{id}}', {
      id: value.number ?? value.id
    })
    emailForm.message = t(
      'sales.quotations.details.emailBody',
      'Hi {{name}},\n\nPlease find your quotation attached.\n\nRegards,\nTOSS Team',
      {
        name: value.customer.name
      }
    )
    useHead({
      title: t('sales.quotations.details.pageTitle', 'Quotation {{id}}', {
        id: value.number ?? value.id
      })
    })
  },
  { immediate: true }
)

const emailDialogOpen = ref(false)
const deleteDialogOpen = ref(false)
const emailSubmitting = ref(false)
const convertLoading = ref(false)
const pdfLoading = ref(false)
const deleteLoading = ref(false)

const emailForm = reactive<EmailPayload>({
  to: '',
  subject: '',
  message: ''
})

const statusVariant = (status: QuotationStatus | string = 'draft') => {
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

const statusLabel = (status: QuotationStatus | string = 'draft') => {
  switch (status) {
    case 'draft':
      return t('sales.quotations.status.draft', 'Draft')
    case 'sent':
      return t('sales.quotations.status.sent', 'Sent')
    case 'accepted':
      return t('sales.quotations.status.accepted', 'Accepted')
    case 'converted':
      return t('sales.quotations.status.converted', 'Converted')
    case 'rejected':
      return t('sales.quotations.status.rejected', 'Rejected')
    case 'expired':
      return t('sales.quotations.status.expired', 'Expired')
    default:
      return status
  }
}

const errorMessage = computed(() => {
  const value = error.value
  if (!value) {
    return null
  }
  if (value instanceof Error) {
    return value.message
  }
  return String(value)
})

const formatDate = (value: string) =>
  new Date(value).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  })

const formatCurrency = (value: number) =>
  new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(value ?? 0)

const goBack = () => {
  router.push('/sales/quotations')
}

const navigateToEdit = () => {
  if (!quotation.value) {
    return
  }
  router.push(`/sales/quotations/${quotationId.value}/edit`)
}

const handleSendEmail = async () => {
  if (!quotation.value) {
    return
  }
  emailSubmitting.value = true
  try {
    await sendEmail(quotationId.value, emailForm)
    toast({
      description: t('sales.quotations.details.emailSuccess', 'Quotation emailed successfully.')
    })
    emailDialogOpen.value = false
  } catch (err) {
    console.error(err)
    toast({
      variant: 'destructive',
      description: t('sales.quotations.details.emailError', 'Failed to send quotation email.')
    })
  } finally {
    emailSubmitting.value = false
  }
}

const handleDownloadPdf = async () => {
  if (!quotation.value) {
    return
  }
  pdfLoading.value = true
  try {
    const pdf = await generatePDF(quotationId.value)
    const link = document.createElement('a')
    link.href = `data:application/pdf;base64,${pdf.base64}`
    link.download = pdf.filename
    link.click()
    toast({
      description: t('sales.quotations.details.downloadSuccess', 'Quotation PDF downloaded.')
    })
  } catch (err) {
    console.error(err)
    toast({
      variant: 'destructive',
      description: t('sales.quotations.details.downloadError', 'Failed to download PDF.')
    })
  } finally {
    pdfLoading.value = false
  }
}

const handleConvertToOrder = async () => {
  if (!quotation.value) {
    return
  }
  convertLoading.value = true
  try {
    const order = await convertToSalesOrder(quotationId.value)
    toast({
      description: t(
        'sales.quotations.details.convertSuccess',
        'Quotation converted to order {{id}}.',
        {
          id: order.number ?? order.id
        }
      )
    })
    await refresh()
    await fetchStats()
  } catch (err) {
    console.error(err)
    toast({
      variant: 'destructive',
      description: t('sales.quotations.details.convertError', 'Failed to convert quotation.')
    })
  } finally {
    convertLoading.value = false
  }
}

const handleDeleteQuotation = async () => {
  if (!quotation.value) {
    return
  }
  deleteLoading.value = true
  try {
    await deleteQuotation(quotationId.value)
    toast({
      description: t('sales.quotations.details.deleteSuccess', 'Quotation deleted successfully.')
    })
    deleteDialogOpen.value = false
    router.push('/sales/quotations')
  } catch (err) {
    console.error(err)
    toast({
      variant: 'destructive',
      description: t('sales.quotations.details.deleteError', 'Failed to delete quotation.')
    })
  } finally {
    deleteLoading.value = false
  }
}
</script>
