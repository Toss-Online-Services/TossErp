<template>
  <div class="p-4 sm:p-6">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('sales.orders.detail.title') }} #{{ order.id }}
        </h1>
        <div class="flex items-center gap-2 text-sm text-gray-500 dark:text-gray-400">
          <span>{{ $t('common.status') }}:</span>
          <span :class="statusClasses(order.status)" class="px-2.5 py-0.5 text-xs font-medium rounded-full">
            {{ order.status }}
          </span>
        </div>
      </div>
      <div class="flex items-center gap-2">
        <button @click="printOrder" class="btn btn-secondary">
          <Icon name="heroicons:printer" class="w-4 h-4 mr-2" />
          {{ $t('common.print') }}
        </button>
        <NuxtLink :to="`/sales/orders/${order.id}/edit`" class="btn btn-primary">
          <Icon name="heroicons:pencil" class="w-4 h-4 mr-2" />
          {{ $t('common.edit') }}
        </NuxtLink>
      </div>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
      <!-- Main Content -->
      <div class="lg:col-span-2 space-y-6">
        <!-- Order Details -->
        <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg p-6">
          <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-4">{{ $t('sales.orders.detail.orderDetails') }}</h3>
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4 text-sm">
            <div>
              <p class="text-gray-500 dark:text-gray-400 font-medium">{{ $t('sales.orders.detail.customer') }}</p>
              <p class="text-gray-800 dark:text-gray-200">{{ order.customerName }}</p>
            </div>
            <div>
              <p class="text-gray-500 dark:text-gray-400 font-medium">{{ $t('sales.orders.detail.orderDate') }}</p>
              <p class="text-gray-800 dark:text-gray-200">{{ formatDate(order.date) }}</p>
            </div>
            <div>
              <p class="text-gray-500 dark:text-gray-400 font-medium">{{ $t('sales.orders.detail.shippingAddress') }}</p>
              <p class="text-gray-800 dark:text-gray-200">{{ order.shippingAddress }}</p>
            </div>
            <div v-if="order.quotationId">
              <p class="text-gray-500 dark:text-gray-400 font-medium">{{ $t('sales.orders.detail.relatedQuotation') }}</p>
              <NuxtLink :to="`/sales/quotations/${order.quotationId}`" class="text-blue-600 hover:underline">{{ order.quotationId }}</NuxtLink>
            </div>
          </div>
        </div>

        <!-- Items -->
        <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg">
          <h3 class="text-lg font-bold text-gray-900 dark:text-white p-6 border-b border-gray-200 dark:border-gray-700">{{ $t('sales.orders.detail.itemsOrdered') }}</h3>
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
              <tr v-for="item in order.items" :key="item.sku" class="border-b border-gray-200 dark:border-gray-700">
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
                <td class="table-footer text-right">{{ formatCurrency(order.totals.subtotal) }}</td>
              </tr>
              <tr>
                <td colspan="3" class="table-footer text-right">{{ $t('common.vat') }} (15%)</td>
                <td class="table-footer text-right">{{ formatCurrency(order.totals.vat) }}</td>
              </tr>
              <tr>
                <td colspan="3" class="table-footer text-right font-bold text-base">{{ $t('common.total') }}</td>
                <td class="table-footer text-right font-bold text-base">{{ formatCurrency(order.totals.total) }}</td>
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
                <button v-if="order.status === 'Confirmed'" @click="generateInvoice" class="btn btn-full btn-secondary">
                    <Icon name="heroicons:document-text" class="w-4 h-4 mr-2" />
                    {{ $t('sales.orders.detail.generateInvoice') }}
                </button>
                <button v-if="order.status === 'Processing'" @click="createDeliveryNote" class="btn btn-full btn-secondary">
                    <Icon name="heroicons:truck" class="w-4 h-4 mr-2" />
                    {{ $t('sales.orders.detail.createDeliveryNote') }}
                </button>
                 <button v-if="order.status === 'Pending'" @click="confirmOrder" class="btn btn-full btn-success">
                    <Icon name="heroicons:check-circle" class="w-4 h-4 mr-2" />
                    {{ $t('sales.orders.detail.confirmOrder') }}
                </button>
            </div>
        </div>
        <!-- Status History -->
        <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg p-6">
          <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-4">{{ $t('sales.orders.detail.statusHistory') }}</h3>
          <ul class="space-y-4">
            <li v-for="history in order.statusHistory" :key="history.timestamp" class="flex gap-3">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 rounded-full flex items-center justify-center" :class="statusHistoryIcon(history.status).bg">
                  <Icon :name="statusHistoryIcon(history.status).icon" class="w-5 h-5" :class="statusHistoryIcon(history.status).text" />
                </div>
              </div>
              <div>
                <p class="text-sm font-medium text-gray-800 dark:text-gray-200">{{ history.status }}</p>
                <p class="text-xs text-gray-500 dark:text-gray-400">{{ formatDateTime(history.timestamp) }}</p>
              </div>
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

const orderId = route.params.id

// Mock Data
const order = ref({
  id: `SO-123`,
  quotationId: 'QT-001',
  customerName: 'Jabu\'s Spaza',
  date: '2025-11-10',
  shippingAddress: '123 Main Road, Soweto, Johannesburg, 1818',
  status: 'Completed',
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
  statusHistory: [
    { status: 'Completed', timestamp: '2025-11-12T14:35:00Z' },
    { status: 'Shipped', timestamp: '2025-11-12T11:10:00Z' },
    { status: 'Processing', timestamp: '2025-11-11T09:00:00Z' },
    { status: 'Confirmed', timestamp: '2025-11-10T17:00:00Z' },
    { status: 'Pending', timestamp: '2025-11-10T16:20:00Z' },
  ]
})

useHead({
  title: t('sales.orders.detail.pageTitle', { id: order.value.id }),
})

// Helper functions
const formatDate = (dateString: string) => new Date(dateString).toLocaleDateString('en-ZA')
const formatDateTime = (dateString: string) => new Date(dateString).toLocaleString('en-ZA', { dateStyle: 'short', timeStyle: 'short' })
const formatCurrency = (value: number) => new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(value)

const statusClasses = (status: string) => {
  const classes = {
    Pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300',
    Confirmed: 'bg-sky-100 text-sky-800 dark:bg-sky-900 dark:text-sky-300',
    Processing: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300',
    Shipped: 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-300',
    Completed: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300',
    Cancelled: 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300',
  }
  return classes[status] || classes.Cancelled
}

const statusHistoryIcon = (status: string) => {
    const icons = {
        Pending: { icon: 'heroicons:clock', bg: 'bg-yellow-100', text: 'text-yellow-600' },
        Confirmed: { icon: 'heroicons:check-badge', bg: 'bg-sky-100', text: 'text-sky-600' },
        Processing: { icon: 'heroicons:cog', bg: 'bg-blue-100', text: 'text-blue-600' },
        Shipped: { icon: 'heroicons:truck', bg: 'bg-purple-100', text: 'text-purple-600' },
        Completed: { icon: 'heroicons:check-circle', bg: 'bg-green-100', text: 'text-green-600' },
        Cancelled: { icon: 'heroicons:x-circle', bg: 'bg-gray-100', text: 'text-gray-600' },
    }
    return icons[status] || icons.Cancelled
}

const printOrder = () => {
  const doc = new jsPDF()
  const o = order.value

  doc.setFontSize(20)
  doc.text(t('sales.orders.detail.pdfTitle'), 14, 22)
  doc.setFontSize(12)
  doc.text(`${t('sales.orders.detail.order')} #: ${o.id}`, 14, 30)

  autoTable(doc, {
    startY: 40,
    body: [
      [{ content: t('sales.orders.detail.customer'), styles: { fontStyle: 'bold' } }, o.customerName],
      [{ content: t('sales.orders.detail.orderDate'), styles: { fontStyle: 'bold' } }, formatDate(o.date)],
      [{ content: t('sales.orders.detail.shippingAddress'), styles: { fontStyle: 'bold' } }, o.shippingAddress],
    ],
    theme: 'plain',
    styles: { fontSize: 10 },
  })

  autoTable(doc, {
    startY: (doc as any).lastAutoTable.finalY + 10,
    head: [[t('common.product'), t('common.quantity'), t('common.unitPrice'), t('common.total')]],
    body: o.items.map(item => [item.name, item.quantity, formatCurrency(item.unitPrice), formatCurrency(item.quantity * item.unitPrice)]),
    foot: [
        [{ content: t('common.subtotal'), colSpan: 3, styles: { halign: 'right' } }, { content: formatCurrency(o.totals.subtotal), styles: { halign: 'right' } }],
        [{ content: `${t('common.vat')} (15%)`, colSpan: 3, styles: { halign: 'right' } }, { content: formatCurrency(o.totals.vat), styles: { halign: 'right' } }],
        [{ content: t('common.total'), colSpan: 3, styles: { halign: 'right', fontStyle: 'bold' } }, { content: formatCurrency(o.totals.total), styles: { halign: 'right', fontStyle: 'bold' } }],
    ],
    headStyles: { fillColor: [22, 160, 133] },
    footStyles: { fillColor: [245, 245, 245], textColor: [0,0,0] },
  })

  doc.save(`Sales_Order_${o.id}.pdf`)
}

const autoTable = (doc: jsPDF, options: any) => {
  (doc as any).autoTable(options)
}

// Action handlers
const generateInvoice = () => alert('Generate Invoice action triggered!')
const createDeliveryNote = () => alert('Create Delivery Note action triggered!')
const confirmOrder = () => alert('Confirm Order action triggered!')

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
