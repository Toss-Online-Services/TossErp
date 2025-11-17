<template>
  <div class="p-4 sm:p-6">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('sales.invoices.detail.title') }} #{{ invoice.id }}
        </h1>
        <div class="flex items-center gap-2 text-sm text-gray-500 dark:text-gray-400">
          <span>{{ $t('common.status') }}:</span>
          <span :class="statusClasses(invoice.status)" class="px-2.5 py-0.5 text-xs font-medium rounded-full">
            {{ invoice.status }}
          </span>
        </div>
      </div>
      <div class="flex items-center gap-2">
        <button @click="printInvoice" class="btn btn-secondary">
          <Icon name="heroicons:printer" class="w-4 h-4 mr-2" />
          {{ $t('common.print') }}
        </button>
        <NuxtLink :to="`/sales/invoices/${invoice.id}/edit`" class="btn btn-primary">
          <Icon name="heroicons:pencil" class="w-4 h-4 mr-2" />
          {{ $t('common.edit') }}
        </NuxtLink>
      </div>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
      <!-- Main Content -->
      <div class="lg:col-span-2 space-y-6">
        <!-- Invoice Details -->
        <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg p-6">
          <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-4">{{ $t('sales.invoices.detail.invoiceDetails') }}</h3>
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4 text-sm">
            <div>
              <p class="text-gray-500 dark:text-gray-400 font-medium">{{ $t('sales.invoices.detail.customer') }}</p>
              <p class="text-gray-800 dark:text-gray-200">{{ invoice.customerName }}</p>
            </div>
            <div>
              <p class="text-gray-500 dark:text-gray-400 font-medium">{{ $t('sales.invoices.detail.invoiceDate') }}</p>
              <p class="text-gray-800 dark:text-gray-200">{{ formatDate(invoice.date) }}</p>
            </div>
            <div>
              <p class="text-gray-500 dark:text-gray-400 font-medium">{{ $t('sales.invoices.detail.dueDate') }}</p>
              <p class="text-gray-800 dark:text-gray-200">{{ formatDate(invoice.dueDate) }}</p>
            </div>
            <div v-if="invoice.orderId">
              <p class="text-gray-500 dark:text-gray-400 font-medium">{{ $t('sales.invoices.detail.relatedOrder') }}</p>
              <NuxtLink :to="`/sales/orders/${invoice.orderId}`" class="text-blue-600 hover:underline">{{ invoice.orderId }}</NuxtLink>
            </div>
          </div>
        </div>

        <!-- Items -->
        <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg">
          <h3 class="text-lg font-bold text-gray-900 dark:text-white p-6 border-b border-gray-200 dark:border-gray-700">{{ $t('sales.invoices.detail.itemsBilled') }}</h3>
          <table class="min-w-full">
            <thead class="bg-gray-50 dark:bg-gray-700/50">
              <tr>
                <th class="table-header">{{ $t('common.product') }}</th>
                <th class="table-header text-right">{{ $t('common.quantity') }}</th>
                <th class="table-header text-right">{{ $t('common.unitPrice') }}</th>
                <th class="table-header text-right">{{ $t('common.total') }}</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in invoice.items" :key="item.sku" class="border-b border-gray-200 dark:border-gray-700">
                <td class="table-cell">
                  <p class="font-medium text-gray-800 dark:text-gray-200">{{ item.name }}</p>
                  <p class="text-xs text-gray-500 dark:text-gray-400">{{ item.sku }}</p>
                </td>
                <td class="table-cell text-right">{{ item.quantity }}</td>
                <td class="table-cell text-right">{{ formatCurrency(item.unitPrice) }}</td>
                <td class="table-cell text-right font-medium">{{ formatCurrency(item.quantity * item.unitPrice) }}</td>
              </tr>
            </tbody>
            <tfoot class="bg-gray-50 dark:bg-gray-700/50">
              <tr>
                <td colspan="3" class="table-footer text-right">{{ $t('common.subtotal') }}</td>
                <td class="table-footer text-right">{{ formatCurrency(invoice.totals.subtotal) }}</td>
              </tr>
              <tr>
                <td colspan="3" class="table-footer text-right">{{ $t('common.vat') }} (15%)</td>
                <td class="table-footer text-right">{{ formatCurrency(invoice.totals.vat) }}</td>
              </tr>
              <tr>
                <td colspan="3" class="table-footer text-right font-bold text-base">{{ $t('common.total') }}</td>
                <td class="table-footer text-right font-bold text-base">{{ formatCurrency(invoice.totals.total) }}</td>
              </tr>
            </tfoot>
          </table>
        </div>
      </div>

      <!-- Sidebar -->
      <div class="lg:col-span-1 space-y-6">
        <!-- Actions -->
        <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg p-6">
            <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-4">{{ $t('common.actions') }}</h3>
            <div class="space-y-2">
                <button v-if="invoice.status === 'Draft'" @click="sendInvoice" class="btn btn-full btn-success">
                    <Icon name="heroicons:paper-airplane" class="w-4 h-4 mr-2" />
                    {{ $t('sales.invoices.detail.sendInvoice') }}
                </button>
                <button v-if="invoice.status === 'Sent'" @click="recordPayment" class="btn btn-full btn-secondary">
                    <Icon name="heroicons:banknotes" class="w-4 h-4 mr-2" />
                    {{ $t('sales.invoices.detail.recordPayment') }}
                </button>
            </div>
        </div>
        <!-- Payment History -->
        <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg p-6">
          <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-4">{{ $t('sales.invoices.detail.paymentHistory') }}</h3>
          <ul class="space-y-4">
            <li v-for="payment in invoice.paymentHistory" :key="payment.timestamp" class="flex gap-3">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 rounded-full flex items-center justify-center bg-green-100">
                  <Icon name="heroicons:check-circle" class="w-5 h-5 text-green-600" />
                </div>
              </div>
              <div>
                <p class="text-sm font-medium text-gray-800 dark:text-gray-200">{{ $t('sales.invoices.detail.paymentReceived') }} - {{ formatCurrency(payment.amount) }}</p>
                <p class="text-xs text-gray-500 dark:text-gray-400">{{ formatDateTime(payment.timestamp) }} via {{ payment.method }}</p>
              </div>
            </li>
             <li v-if="!invoice.paymentHistory.length">
                <p class="text-sm text-gray-500 dark:text-gray-400">{{ $t('sales.invoices.detail.noPayments') }}</p>
            </li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import jsPDF from 'jspdf'
import 'jspdf-autotable'

const { t } = useI18n()
const route = useRoute()

const invoiceId = route.params.id

// Mock Data
const invoice = ref({
  id: `INV-001`,
  orderId: 'SO-123',
  customerName: 'Jabu\'s Spaza',
  date: '2025-11-12',
  dueDate: '2025-12-12',
  status: 'Paid',
  items: [
    { sku: 'BRD-WHT-001', name: 'Albany Superior White Bread', quantity: 20, unitPrice: 15.50 },
    { sku: 'MLK-FUL-1L', name: 'Clover Full Cream Milk 1L', quantity: 12, unitPrice: 22.00 },
    { sku: 'SUG-BRN-2KG', name: 'Huletts Brown Sugar 2kg', quantity: 5, unitPrice: 45.75 },
  ],
  totals: {
    subtotal: 802.75,
    vat: 120.41,
    total: 923.16,
  },
  paymentHistory: [
    { amount: 923.16, timestamp: '2025-11-20T10:00:00Z', method: 'EFT' },
  ]
})

useHead({
  title: t('sales.invoices.detail.pageTitle', { id: invoice.value.id }),
})

// Helper functions
const formatDate = (dateString: string) => new Date(dateString).toLocaleDateString('en-ZA')
const formatDateTime = (dateString: string) => new Date(dateString).toLocaleString('en-ZA', { dateStyle: 'short', timeStyle: 'short' })
const formatCurrency = (value: number) => new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(value)

const statusClasses = (status: string) => {
  const classes = {
    Draft: 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300',
    Sent: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300',
    Paid: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300',
    Overdue: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300',
    Cancelled: 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300',
  }
  return classes[status] || classes.Cancelled
}

const printInvoice = () => {
  const doc = new jsPDF()
  const i = invoice.value

  doc.setFontSize(20)
  doc.text(t('sales.invoices.detail.pdfTitle'), 14, 22)
  doc.setFontSize(12)
  doc.text(`${t('sales.invoices.detail.invoice')} #: ${i.id}`, 14, 30)

  autoTable(doc, {
    startY: 40,
    body: [
      [{ content: t('sales.invoices.detail.customer'), styles: { fontStyle: 'bold' } }, i.customerName],
      [{ content: t('sales.invoices.detail.invoiceDate'), styles: { fontStyle: 'bold' } }, formatDate(i.date)],
      [{ content: t('sales.invoices.detail.dueDate'), styles: { fontStyle: 'bold' } }, formatDate(i.dueDate)],
    ],
    theme: 'plain',
    styles: { fontSize: 10 },
  })

  autoTable(doc, {
    startY: (doc as any).lastAutoTable.finalY + 10,
    head: [[t('common.product'), t('common.quantity'), t('common.unitPrice'), t('common.total')]],
    body: i.items.map(item => [item.name, item.quantity, formatCurrency(item.unitPrice), formatCurrency(item.quantity * item.unitPrice)]),
    foot: [
        [{ content: t('common.subtotal'), colSpan: 3, styles: { halign: 'right' } }, { content: formatCurrency(i.totals.subtotal), styles: { halign: 'right' } }],
        [{ content: `${t('common.vat')} (15%)`, colSpan: 3, styles: { halign: 'right' } }, { content: formatCurrency(i.totals.vat), styles: { halign: 'right' } }],
        [{ content: t('common.total'), colSpan: 3, styles: { halign: 'right', fontStyle: 'bold' } }, { content: formatCurrency(i.totals.total), styles: { halign: 'right', fontStyle: 'bold' } }],
    ],
    headStyles: { fillColor: [22, 160, 133] },
    footStyles: { fillColor: [245, 245, 245], textColor: [0,0,0] },
  })

  doc.save(`Invoice_${i.id}.pdf`)
}

const autoTable = (doc: jsPDF, options: any) => {
  (doc as any).autoTable(options)
}

// Action handlers
const sendInvoice = () => alert('Send Invoice action triggered!')
const recordPayment = () => alert('Record Payment action triggered!')

</script>

<style scoped>
.table-header {
  @apply px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider dark:text-gray-300;
}
.table-cell {
  @apply px-6 py-4 whitespace-nowrap text-sm;
}
.table-footer {
    @apply px-6 py-3 text-sm text-gray-700 dark:text-gray-200;
}
.btn {
  @apply inline-flex items-center justify-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-offset-2;
}
.btn-full {
    @apply w-full;
}
.btn-primary {
  @apply text-white bg-blue-600 hover:bg-blue-700 focus:ring-blue-500;
}
.btn-secondary {
  @apply text-gray-700 bg-gray-100 hover:bg-gray-200 dark:bg-gray-700 dark:text-gray-200 dark:hover:bg-gray-600 focus:ring-gray-500;
}
.btn-success {
    @apply text-white bg-green-600 hover:bg-green-700 focus:ring-green-500;
}
</style>
