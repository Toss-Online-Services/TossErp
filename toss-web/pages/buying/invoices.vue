<template>
  <div class="space-y-6">
    <PageHeader
      title="Purchase Invoices"
      description="Manage supplier invoices and payments"
    />

    <div class="flex justify-between items-center">
      <div class="flex gap-4">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Search invoices..."
          class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white"
        />
        <select v-model="filterStatus" class="px-4 py-2 border rounded-lg dark:bg-gray-700 dark:text-white">
          <option value="">All Status</option>
          <option value="Draft">Draft</option>
          <option value="Submitted">Submitted</option>
          <option value="Paid">Paid</option>
          <option value="Overdue">Overdue</option>
        </select>
      </div>
      <button class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
        Record Invoice
      </button>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-4">
        <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">Total Invoices</p>
        <p class="text-2xl font-bold dark:text-white">{{ invoices.length }}</p>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-4">
        <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">Outstanding</p>
        <p class="text-2xl font-bold text-red-600">{{ formatCurrency(outstandingTotal) }}</p>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-4">
        <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">Paid This Month</p>
        <p class="text-2xl font-bold text-green-600">{{ formatCurrency(paidThisMonth) }}</p>
      </div>
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-4">
        <p class="text-sm text-gray-600 dark:text-gray-400 mb-1">Overdue</p>
        <p class="text-2xl font-bold text-orange-600">{{ invoices.filter(i => i.status === 'Overdue').length }}</p>
      </div>
    </div>

    <div class="bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden">
      <table class="w-full">
        <thead class="bg-gray-50 dark:bg-gray-700">
          <tr>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Invoice #</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Supplier</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">PO Reference</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Invoice Date</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Due Date</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Amount</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Outstanding</th>
            <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Status</th>
            <th class="px-6 py-3 text-right text-xs font-medium text-gray-500 dark:text-gray-300 uppercase">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-for="invoice in filteredInvoices" :key="invoice.id" class="dark:text-white hover:bg-gray-50 dark:hover:bg-gray-700">
            <td class="px-6 py-4 font-mono text-sm">{{ invoice.invoiceNumber }}</td>
            <td class="px-6 py-4">
              <div>
                <p class="font-medium">{{ invoice.supplier }}</p>
                <p class="text-xs text-gray-500 dark:text-gray-400">{{ invoice.supplierEmail }}</p>
              </div>
            </td>
            <td class="px-6 py-4 text-sm">{{ invoice.poReference }}</td>
            <td class="px-6 py-4 text-sm">{{ formatDate(invoice.invoiceDate) }}</td>
            <td class="px-6 py-4 text-sm" :class="isOverdue(invoice.dueDate) ? 'text-red-600 font-medium' : ''">
              {{ formatDate(invoice.dueDate) }}
            </td>
            <td class="px-6 py-4 text-right font-medium">{{ formatCurrency(invoice.amount) }}</td>
            <td class="px-6 py-4 text-right font-medium" :class="invoice.outstanding > 0 ? 'text-red-600' : 'text-green-600'">
              {{ formatCurrency(invoice.outstanding) }}
            </td>
            <td class="px-6 py-4">
              <span :class="getStatusClass(invoice.status)" class="px-2 py-1 text-xs rounded-full">
                {{ invoice.status }}
              </span>
            </td>
            <td class="px-6 py-4 text-right">
              <button class="text-blue-600 hover:text-blue-800 mr-3">View</button>
              <button v-if="invoice.outstanding > 0" class="text-green-600 hover:text-green-800">Pay</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

definePageMeta({
  middleware: ['auth'],
  layout: 'default',
})

useHead({
  title: 'Purchase Invoices - TOSS ERP',
})

const searchQuery = ref('')
const filterStatus = ref('')

const invoices = ref([
  { id: 1, invoiceNumber: 'PINV-2024-001', supplier: 'ABC Suppliers Ltd', supplierEmail: 'billing@abc.com', poReference: 'PO-2024-001', invoiceDate: '2024-01-20', dueDate: '2024-02-20', amount: 45000, outstanding: 0, status: 'Paid' },
  { id: 2, invoiceNumber: 'PINV-2024-002', supplier: 'XYZ Manufacturing', supplierEmail: 'accounts@xyz.com', poReference: 'PO-2024-002', invoiceDate: '2024-01-25', dueDate: '2024-02-25', amount: 32500, outstanding: 32500, status: 'Submitted' },
  { id: 3, invoiceNumber: 'PINV-2024-003', supplier: 'Global Trade Co', supplierEmail: 'finance@global.com', poReference: 'PO-2024-003', invoiceDate: '2024-02-05', dueDate: '2024-02-20', amount: 67800, outstanding: 67800, status: 'Overdue' },
  { id: 4, invoiceNumber: 'PINV-2024-004', supplier: 'Tech Supplies Inc', supplierEmail: 'billing@tech.com', poReference: 'PO-2024-004', invoiceDate: '2024-02-10', dueDate: '2024-03-10', amount: 125000, outstanding: 125000, status: 'Submitted' },
  { id: 5, invoiceNumber: 'PINV-2024-005', supplier: 'Quality Parts Ltd', supplierEmail: 'invoices@quality.com', poReference: 'PO-2024-005', invoiceDate: '2024-02-12', dueDate: '2024-03-12', amount: 54300, outstanding: 0, status: 'Paid' },
])

const filteredInvoices = computed(() => {
  let result = invoices.value

  if (filterStatus.value) {
    result = result.filter(i => i.status === filterStatus.value)
  }

  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    result = result.filter(i => 
      i.invoiceNumber.toLowerCase().includes(query) ||
      i.supplier.toLowerCase().includes(query) ||
      i.poReference.toLowerCase().includes(query)
    )
  }

  return result
})

const outstandingTotal = computed(() => invoices.value.reduce((sum, i) => sum + i.outstanding, 0))
const paidThisMonth = computed(() => {
  const currentMonth = new Date().toISOString().slice(0, 7)
  return invoices.value
    .filter(i => i.status === 'Paid' && i.invoiceDate.startsWith(currentMonth))
    .reduce((sum, i) => sum + i.amount, 0)
})

const isOverdue = (dueDate: string) => {
  return new Date(dueDate) < new Date()
}

const getStatusClass = (status: string) => {
  const classes = {
    'Draft': 'bg-gray-100 text-gray-800',
    'Submitted': 'bg-yellow-100 text-yellow-800',
    'Paid': 'bg-green-100 text-green-800',
    'Overdue': 'bg-red-100 text-red-800',
  }
  return classes[status as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(amount)
}

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('en-ZA')
}
</script>
