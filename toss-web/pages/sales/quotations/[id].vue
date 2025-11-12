<template>
  <div class="p-4 sm:p-6">
    <!-- Page Header -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('sales.quotations.details.title') }} #{{ quotation.id }}
        </h1>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ $t('sales.quotations.details.issuedOn') }} {{ formatDate(quotation.date) }}
        </p>
      </div>

      <div class="flex items-center gap-2">
        <span :class="statusClasses(quotation.status)" class="px-2.5 py-0.5 text-xs font-medium rounded-full">
          {{ quotation.status }}
        </span>
      </div>
    </div>

    <!-- Action Buttons -->
    <div class="flex flex-wrap gap-2 mb-6">
      <button class="btn btn-secondary">
        <Icon name="heroicons:pencil" class="w-4 h-4 mr-2" />
        {{ $t('common.edit') }}
      </button>
      <button class="btn btn-secondary">
        <Icon name="heroicons:paper-airplane" class="w-4 h-4 mr-2" />
        {{ $t('common.send') }}
      </button>
      <button class="btn btn-primary">
        <Icon name="heroicons:arrow-right-circle" class="w-4 h-4 mr-2" />
        {{ $t('sales.quotations.details.convertToOrder') }}
      </button>
      <button class="btn btn-outline">
        <Icon name="heroicons:arrow-down-tray" class="w-4 h-4 mr-2" />
        {{ $t('common.download') }} PDF
      </button>
      <button class="btn btn-danger-outline">
        <Icon name="heroicons:trash" class="w-4 h-4 mr-2" />
        {{ $t('common.delete') }}
      </button>
    </div>

    <!-- Quotation Body -->
    <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg p-6">
      <!-- Customer & Company Info -->
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-6 mb-8">
        <div>
          <h3 class="font-semibold text-gray-800 dark:text-gray-200 mb-2">{{ $t('sales.quotations.details.billTo') }}</h3>
          <p class="text-gray-600 dark:text-gray-300">{{ quotation.customer.name }}</p>
          <p class="text-gray-500 dark:text-gray-400">{{ quotation.customer.address }}</p>
          <p class="text-gray-500 dark:text-gray-400">{{ quotation.customer.email }}</p>
        </div>
        <div class="sm:text-right">
          <h3 class="font-semibold text-gray-800 dark:text-gray-200 mb-2">TOSS Inc.</h3>
          <p class="text-gray-500 dark:text-gray-400">123 Biz Street, Johannesburg, 2000</p>
          <p class="text-gray-500 dark:text-gray-400">info@toss.co.za</p>
          <p class="text-gray-500 dark:text-gray-400">VAT: 4123456789</p>
        </div>
      </div>

      <!-- Line Items Table -->
      <div class="overflow-x-auto mb-8">
        <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
          <thead class="bg-gray-50 dark:bg-gray-700">
            <tr>
              <th scope="col" class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider dark:text-gray-300">{{ $t('sales.quotations.create.product') }}</th>
              <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider dark:text-gray-300">{{ $t('sales.quotations.create.quantity') }}</th>
              <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider dark:text-gray-300">{{ $t('sales.quotations.create.unitPrice') }}</th>
              <th scope="col" class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider dark:text-gray-300">{{ $t('sales.quotations.create.total') }}</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200 dark:bg-gray-800 dark:divide-gray-700">
            <tr v-for="item in quotation.items" :key="item.id">
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900 dark:text-white">{{ item.name }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 text-right dark:text-gray-300">{{ item.quantity }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 text-right dark:text-gray-300">{{ formatCurrency(item.unitPrice) }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500 text-right dark:text-gray-300">{{ formatCurrency(item.quantity * item.unitPrice) }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Totals Section -->
      <div class="flex justify-end mb-8">
        <div class="w-full max-w-xs">
          <div class="flex justify-between py-2 border-b border-gray-200 dark:border-gray-700">
            <span class="text-gray-600 dark:text-gray-300">{{ $t('sales.quotations.create.subtotal') }}:</span>
            <span class="font-medium text-gray-800 dark:text-gray-200">{{ formatCurrency(quotation.subtotal) }}</span>
          </div>
          <div class="flex justify-between py-2 border-b border-gray-200 dark:border-gray-700">
            <span class="text-gray-600 dark:text-gray-300">{{ $t('sales.quotations.create.discount') }} ({{ quotation.discountRate }}%):</span>
            <span class="font-medium text-gray-800 dark:text-gray-200">-{{ formatCurrency(quotation.discountAmount) }}</span>
          </div>
          <div class="flex justify-between py-2 border-b border-gray-200 dark:border-gray-700">
            <span class="text-gray-600 dark:text-gray-300">{{ $t('sales.quotations.create.vat') }} ({{ quotation.vatRate }}%):</span>
            <span class="font-medium text-gray-800 dark:text-gray-200">{{ formatCurrency(quotation.vatAmount) }}</span>
          </div>
          <div class="flex justify-between py-3 bg-gray-50 dark:bg-gray-700 px-4 -mx-4 rounded-md">
            <span class="text-lg font-bold text-gray-900 dark:text-white">{{ $t('sales.quotations.create.grandTotal') }}:</span>
            <span class="text-lg font-bold text-gray-900 dark:text-white">{{ formatCurrency(quotation.grandTotal) }}</span>
          </div>
        </div>
      </div>

      <!-- Terms & Notes -->
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-6">
        <div>
          <h4 class="font-semibold text-gray-800 dark:text-gray-200 mb-2">{{ $t('sales.quotations.create.termsAndConditions') }}</h4>
          <p class="text-sm text-gray-500 dark:text-gray-400 whitespace-pre-wrap">{{ quotation.terms }}</p>
        </div>
        <div>
          <h4 class="font-semibold text-gray-800 dark:text-gray-200 mb-2">{{ $t('sales.quotations.create.internalNotes') }}</h4>
          <p class="text-sm text-gray-500 dark:text-gray-400 p-3 bg-yellow-50 dark:bg-yellow-900/20 rounded-md border border-yellow-200 dark:border-yellow-800">{{ quotation.notes }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRoute } from 'vue-router'
import jsPDF from 'jspdf'
import 'jspdf-autotable'

const route = useRoute()
const { t } = useI18n()

// Mock data for the quotation
const quotation = ref({
  id: route.params.id,
  date: '2025-11-12',
  status: 'Sent',
  customer: {
    name: 'Jabu\'s Spaza',
    address: '123 Main Road, Soweto, 1804',
    email: 'jabu@spaza.co.za',
  },
  items: [
    { id: 1, name: 'White Bread', quantity: 20, unitPrice: 15.50 },
    { id: 2, name: 'Maize Meal 5kg', quantity: 10, unitPrice: 45.00 },
    { id: 3, name: 'Cooking Oil 2L', quantity: 15, unitPrice: 65.75 },
  ],
  subtotal: 1746.25,
  discountRate: 5,
  discountAmount: 87.31,
  vatRate: 15,
  vatAmount: 248.84,
  grandTotal: 1907.78,
  terms: 'Payment due within 30 days. Prices valid for 14 days.',
  notes: 'Customer requested early morning delivery.',
})

const generatePDF = () => {
  const doc = new jsPDF()
  const q = quotation.value

  // Header
  doc.setFontSize(20)
  doc.text(`Quotation #${q.id}`, 14, 22)
  doc.setFontSize(10)
  doc.text(`Date: ${formatDate(q.date)}`, 14, 30)
  doc.text(`Status: ${q.status}`, 14, 36)

  // Company & Customer Info
  doc.setFontSize(12)
  doc.text('From:', 14, 50)
  doc.setFontSize(10)
  doc.text('TOSS Inc.', 14, 56)
  doc.text('123 Biz Street, Johannesburg, 2000', 14, 62)
  
  doc.setFontSize(12)
  doc.text('To:', 140, 50)
  doc.setFontSize(10)
  doc.text(q.customer.name, 140, 56)
  doc.text(q.customer.address, 140, 62)

  // Line Items
  const tableColumn = ["Product", "Quantity", "Unit Price", "Total"];
  const tableRows = [];
  q.items.forEach(item => {
    const itemData = [
      item.name,
      item.quantity,
      formatCurrency(item.unitPrice),
      formatCurrency(item.quantity * item.unitPrice)
    ];
    tableRows.push(itemData);
  });

  (doc as any).autoTable({
    startY: 80,
    head: [tableColumn],
    body: tableRows,
  });

  // Totals
  const finalY = (doc as any).lastAutoTable.finalY;
  doc.setFontSize(10)
  doc.text(`Subtotal: ${formatCurrency(q.subtotal)}`, 140, finalY + 10)
  doc.text(`Discount (${q.discountRate}%): -${formatCurrency(q.discountAmount)}`, 140, finalY + 16)
  doc.text(`VAT (${q.vatRate}%): ${formatCurrency(q.vatAmount)}`, 140, finalY + 22)
  doc.setFontSize(12)
  doc.setFont('helvetica', 'bold')
  doc.text(`Grand Total: ${formatCurrency(q.grandTotal)}`, 140, finalY + 30)
  doc.setFont('helvetica', 'normal')

  // Footer
  doc.setFontSize(10)
  doc.text('Terms & Conditions', 14, finalY + 50)
  doc.text(q.terms, 14, finalY + 56, { maxWidth: 180 })

  doc.save(`Quotation-${q.id}.pdf`);
}

// Helper functions
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('en-ZA', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  })
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
  }).format(amount)
}

const statusClasses = (status: string) => {
  switch (status) {
    case 'Draft':
      return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300'
    case 'Sent':
      return 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300'
    case 'Accepted':
      return 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
    case 'Expired':
      return 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300'
    default:
      return 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
  }
}

useHead({
  title: t('sales.quotations.details.pageTitle', { id: quotation.value.id }),
})
</script>

<style scoped>
/* Scoped styles for buttons and other elements */
.btn {
  @apply inline-flex items-center justify-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-offset-2;
}
.btn-primary {
  @apply text-white bg-blue-600 hover:bg-blue-700 focus:ring-blue-500;
}
.btn-secondary {
  @apply text-gray-700 bg-gray-100 hover:bg-gray-200 focus:ring-gray-500 dark:bg-gray-700 dark:text-gray-200 dark:hover:bg-gray-600;
}
.btn-outline {
  @apply text-gray-700 bg-white border-gray-300 hover:bg-gray-50 focus:ring-blue-500 dark:bg-gray-800 dark:border-gray-600 dark:text-gray-300 dark:hover:bg-gray-700;
}
.btn-danger-outline {
  @apply text-red-700 bg-white border-red-300 hover:bg-red-50 focus:ring-red-500 dark:bg-gray-800 dark:border-red-600 dark:text-red-400 dark:hover:bg-gray-700;
}
</style>
