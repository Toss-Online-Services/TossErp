<template>
  <div class="p-4 sm:p-6">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('sales.deliveryNotes.detail.title') }} #{{ deliveryNote.id }}
        </h1>
        <div class="flex items-center gap-2 text-sm text-gray-500 dark:text-gray-400">
          <span>{{ $t('common.status') }}:</span>
          <span :class="statusClasses(deliveryNote.status)" class="px-2.5 py-0.5 text-xs font-medium rounded-full">
            {{ deliveryNote.status }}
          </span>
        </div>
      </div>
      <div class="flex items-center gap-2">
        <button @click="printDeliveryNote" class="btn btn-secondary">
          <Icon name="heroicons:printer" class="w-4 h-4 mr-2" />
          {{ $t('common.print') }}
        </button>
        <NuxtLink :to="`/sales/delivery-notes/${deliveryNote.id}/edit`" class="btn btn-primary">
          <Icon name="heroicons:pencil" class="w-4 h-4 mr-2" />
          {{ $t('common.edit') }}
        </NuxtLink>
      </div>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
      <!-- Main Content -->
      <div class="lg:col-span-2 space-y-6">
        <!-- Delivery Details -->
        <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg p-6">
          <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-4">{{ $t('sales.deliveryNotes.detail.deliveryDetails') }}</h3>
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4 text-sm">
            <div>
              <p class="text-gray-500 dark:text-gray-400 font-medium">{{ $t('sales.deliveryNotes.detail.customer') }}</p>
              <p class="text-gray-800 dark:text-gray-200">{{ deliveryNote.customerName }}</p>
            </div>
            <div>
              <p class="text-gray-500 dark:text-gray-400 font-medium">{{ $t('sales.deliveryNotes.detail.deliveryDate') }}</p>
              <p class="text-gray-800 dark:text-gray-200">{{ formatDate(deliveryNote.deliveryDate) }}</p>
            </div>
            <div>
              <p class="text-gray-500 dark:text-gray-400 font-medium">{{ $t('sales.deliveryNotes.detail.deliveryAddress') }}</p>
              <p class="text-gray-800 dark:text-gray-200">{{ deliveryNote.deliveryAddress }}</p>
            </div>
            <div>
              <p class="text-gray-500 dark:text-gray-400 font-medium">{{ $t('sales.deliveryNotes.detail.driver') }}</p>
              <p class="text-gray-800 dark:text-gray-200">{{ deliveryNote.driver }}</p>
            </div>
            <div>
              <p class="text-gray-500 dark:text-gray-400 font-medium">{{ $t('sales.deliveryNotes.detail.relatedOrder') }}</p>
              <NuxtLink :to="`/sales/orders/${deliveryNote.orderId}`" class="text-blue-600 hover:underline">{{ deliveryNote.orderId }}</NuxtLink>
            </div>
          </div>
        </div>

        <!-- Items -->
        <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg">
          <h3 class="text-lg font-bold text-gray-900 dark:text-white p-6 border-b border-gray-200 dark:border-gray-700">{{ $t('sales.deliveryNotes.detail.itemsDelivered') }}</h3>
          <table class="min-w-full">
            <thead class="bg-gray-50 dark:bg-gray-700/50">
              <tr>
                <th class="table-header">{{ $t('common.product') }}</th>
                <th class="table-header text-right">{{ $t('common.quantity') }}</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in deliveryNote.items" :key="item.sku" class="border-b border-gray-200 dark:border-gray-700">
                <td class="table-cell">
                  <p class="font-medium text-gray-800 dark:text-gray-200">{{ item.name }}</p>
                  <p class="text-xs text-gray-500 dark:text-gray-400">{{ item.sku }}</p>
                </td>
                <td class="table-cell text-right">{{ item.quantity }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Sidebar -->
      <div class="lg:col-span-1 space-y-6">
        <!-- Status History -->
        <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg p-6">
          <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-4">{{ $t('sales.deliveryNotes.detail.statusHistory') }}</h3>
          <ul class="space-y-4">
            <li v-for="history in deliveryNote.statusHistory" :key="history.timestamp" class="flex gap-3">
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
        <!-- Notes -->
        <div v-if="deliveryNote.notes" class="bg-white dark:bg-gray-800 shadow-sm rounded-lg p-6">
          <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-4">{{ $t('sales.deliveryNotes.detail.notes') }}</h3>
          <p class="text-sm text-gray-600 dark:text-gray-300">{{ deliveryNote.notes }}</p>
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

const deliveryNoteId = route.params.id

// Mock Data
const deliveryNote = ref({
  id: `DN-001`,
  orderId: 'SO-123',
  customerName: 'Jabu\'s Spaza',
  deliveryDate: '2025-11-12',
  deliveryAddress: '123 Main Road, Soweto, Johannesburg, 1818',
  driver: 'Themba Khumalo',
  status: 'Delivered',
  notes: 'Customer requested delivery after 2 PM. Confirmed receipt with signature.',
  items: [
    { sku: 'BRD-WHT-001', name: 'Albany Superior White Bread', quantity: 20 },
    { sku: 'MLK-FUL-1L', name: 'Clover Full Cream Milk 1L', quantity: 12 },
    { sku: 'SUG-BRN-2KG', name: 'Huletts Brown Sugar 2kg', quantity: 5 },
  ],
  statusHistory: [
    { status: 'Delivered', timestamp: '2025-11-12T14:35:00Z' },
    { status: 'In-Transit', timestamp: '2025-11-12T11:10:00Z' },
    { status: 'Scheduled', timestamp: '2025-11-11T09:00:00Z' },
    { status: 'Created', timestamp: '2025-11-10T16:20:00Z' },
  ]
})

useHead({
  title: t('sales.deliveryNotes.detail.pageTitle', { id: deliveryNote.value.id }),
})

// Helper functions
const formatDate = (dateString: string) => new Date(dateString).toLocaleDateString('en-ZA')
const formatDateTime = (dateString: string) => new Date(dateString).toLocaleString('en-ZA', { dateStyle: 'short', timeStyle: 'short' })

const statusClasses = (status: string) => {
  switch (status) {
    case 'Scheduled': return 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300'
    case 'In-Transit': return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300'
    case 'Delivered': return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
    case 'Failed': return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300'
    default: return 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
  }
}

const statusHistoryIcon = (status: string) => {
  switch (status) {
    case 'Created': return { icon: 'heroicons:document-plus', bg: 'bg-gray-100', text: 'text-gray-600' }
    case 'Scheduled': return { icon: 'heroicons:calendar-days', bg: 'bg-blue-100', text: 'text-blue-600' }
    case 'In-Transit': return { icon: 'heroicons:truck', bg: 'bg-yellow-100', text: 'text-yellow-600' }
    case 'Delivered': return { icon: 'heroicons:check-circle', bg: 'bg-green-100', text: 'text-green-600' }
    default: return { icon: 'heroicons:question-mark-circle', bg: 'bg-gray-100', text: 'text-gray-600' }
  }
}

const printDeliveryNote = () => {
  const doc = new jsPDF()
  const dn = deliveryNote.value

  // Header
  doc.setFontSize(20)
  doc.text(t('sales.deliveryNotes.detail.pdfTitle'), 14, 22)
  doc.setFontSize(12)
  doc.text(`${t('sales.deliveryNotes.detail.deliveryNote')} #: ${dn.id}`, 14, 30)

  // Info
  autoTable(doc, {
    startY: 40,
    body: [
      [{ content: t('sales.deliveryNotes.detail.customer'), styles: { fontStyle: 'bold' } }, dn.customerName],
      [{ content: t('sales.deliveryNotes.detail.deliveryDate'), styles: { fontStyle: 'bold' } }, formatDate(dn.deliveryDate)],
      [{ content: t('sales.deliveryNotes.detail.deliveryAddress'), styles: { fontStyle: 'bold' } }, dn.deliveryAddress],
      [{ content: t('sales.deliveryNotes.detail.relatedOrder'), styles: { fontStyle: 'bold' } }, dn.orderId],
      [{ content: t('sales.deliveryNotes.detail.driver'), styles: { fontStyle: 'bold' } }, dn.driver],
    ],
    theme: 'plain',
    styles: { fontSize: 10 },
  })

  // Items Table
  autoTable(doc, {
    startY: (doc as any).lastAutoTable.finalY + 10,
    head: [[t('common.product'), t('common.quantity')]],
    body: dn.items.map(item => [item.name, item.quantity]),
    headStyles: { fillColor: [22, 160, 133] },
  })

  // Footer
  const finalY = (doc as any).lastAutoTable.finalY
  doc.setFontSize(10)
  if (dn.notes) {
    doc.text(`${t('sales.deliveryNotes.detail.notes')}: ${dn.notes}`, 14, finalY + 15)
  }
  doc.line(14, finalY + 25, 80, finalY + 25)
  doc.text(t('sales.deliveryNotes.detail.receivedBy'), 14, finalY + 30)
  
  doc.line(130, finalY + 25, 196, finalY + 25)
  doc.text(t('common.date'), 130, finalY + 30)

  doc.save(`Delivery_Note_${dn.id}.pdf`)
}

// Need to define autoTable for jspdf
const autoTable = (doc: jsPDF, options: any) => {
  (doc as any).autoTable(options)
}
</script>

<style scoped>
.table-header {
  @apply px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider dark:text-gray-300;
}
.table-cell {
  @apply px-6 py-4 whitespace-nowrap text-sm;
}
.btn {
  @apply inline-flex items-center justify-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-offset-2;
}
.btn-primary {
  @apply text-white bg-blue-600 hover:bg-blue-700 focus:ring-blue-500;
}
.btn-secondary {
  @apply text-gray-700 bg-gray-100 hover:bg-gray-200 dark:bg-gray-700 dark:text-gray-200 dark:hover:bg-gray-600 focus:ring-gray-500;
}
</style>
