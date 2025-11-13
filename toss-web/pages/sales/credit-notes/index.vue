<template>
  <div class="p-4 sm:p-6">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          Credit Notes & Returns
        </h1>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          Manage product returns, refunds, and customer credit adjustments
        </p>
      </div>
      <div class="flex items-center gap-2">
        <button @click="showCreateModal = true" class="btn btn-primary">
          <Icon name="heroicons:plus" class="w-4 h-4 mr-2" />
          New Credit Note
        </button>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
      <div v-for="stat in stats" :key="stat.name" class="bg-white dark:bg-gray-800 shadow-sm rounded-lg p-4 flex items-start justify-between">
        <div>
          <p class="text-sm font-medium text-gray-500 dark:text-gray-400">{{ stat.name }}</p>
          <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stat.value }}</p>
        </div>
        <div class="p-2 rounded-full" :class="stat.bgColor">
          <Icon :name="stat.icon" class="w-6 h-6" :class="stat.iconColor" />
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg p-4 mb-6">
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-4">
        <FormKit
          type="search"
          placeholder="Search credit notes..."
          v-model="filters.search"
        />
        <FormKit
          type="select"
          label="Status"
          v-model="filters.status"
          :options="['All', 'Pending', 'Processed', 'Completed']"
        />
        <FormKit
          type="select"
          label="Refund Method"
          v-model="filters.refundMethod"
          :options="['All', 'Cash', 'Store Credit', 'Bank Refund', 'Replacement']"
        />
        <FormKit
          type="date"
          label="Start Date"
          v-model="filters.startDate"
        />
        <FormKit
          type="date"
          label="End Date"
          v-model="filters.endDate"
        />
      </div>
    </div>

    <!-- Credit Notes Table -->
    <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg overflow-hidden">
      <table class="min-w-full divide-y divide-gray-200 dark:divide-gray-700">
        <thead class="bg-gray-50 dark:bg-gray-700">
          <tr>
            <th class="table-header">Credit Note #</th>
            <th class="table-header">Original Invoice</th>
            <th class="table-header">Customer</th>
            <th class="table-header">Return Date</th>
            <th class="table-header text-right">Credit Amount</th>
            <th class="table-header">Refund Method</th>
            <th class="table-header text-center">Status</th>
            <th class="table-header text-right">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-gray-200 dark:divide-gray-700">
          <tr v-for="creditNote in filteredCreditNotes" :key="creditNote.id">
            <td class="table-cell font-medium">{{ creditNote.creditNoteNumber }}</td>
            <td class="table-cell">{{ creditNote.returnedInvoice }}</td>
            <td class="table-cell">{{ creditNote.customer.name }}</td>
            <td class="table-cell">{{ formatDate(creditNote.returnDate) }}</td>
            <td class="table-cell text-right">{{ formatCurrency(creditNote.financials.totalCreditAmount) }}</td>
            <td class="table-cell">
              <span :class="refundMethodClasses(creditNote.refundMethod)" class="px-2.5 py-0.5 text-xs font-medium rounded-full capitalize">
                {{ formatRefundMethod(creditNote.refundMethod) }}
              </span>
            </td>
            <td class="table-cell text-center">
              <span :class="statusClasses(creditNote.refundStatus)" class="px-2.5 py-0.5 text-xs font-medium rounded-full capitalize">
                {{ creditNote.refundStatus }}
              </span>
            </td>
            <td class="table-cell text-right">
              <div class="flex items-center justify-end gap-2">
                <button @click="viewCreditNote(creditNote)" class="text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-300">
                  <Icon name="heroicons:eye" class="w-4 h-4" />
                </button>
                <button v-if="creditNote.refundStatus === 'pending'" @click="processRefund(creditNote)" class="text-green-600 hover:text-green-800 dark:text-green-400 dark:hover:text-green-300">
                  <Icon name="heroicons:check-circle" class="w-4 h-4" />
                </button>
                <button @click="printCreditNote(creditNote)" class="text-gray-600 hover:text-gray-800 dark:text-gray-400 dark:hover:text-gray-300">
                  <Icon name="heroicons:printer" class="w-4 h-4" />
                </button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Create Credit Note Modal -->
    <div v-if="showCreateModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-10 mx-auto p-5 border w-full max-w-4xl shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">
              Create Credit Note
            </h3>
            <button @click="closeCreateModal" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
              <Icon name="heroicons:x-mark" class="w-6 h-6" />
            </button>
          </div>

          <FormKit
            type="form"
            @submit="createCreditNote"
            #default="{ value }"
            :actions="false"
          >
            <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
              <!-- Left Column -->
              <div>
                <div class="grid grid-cols-1 gap-4">
                  <FormKit
                    type="select"
                    name="returnedInvoice"
                    label="Original Invoice"
                    placeholder="Select invoice to return"
                    :options="availableInvoices"
                    validation="required"
                    @input="loadInvoiceDetails"
                  />
                  <FormKit
                    type="select"
                    name="refundMethod"
                    label="Refund Method"
                    :options="[
                      { label: 'Cash Refund', value: 'cash' },
                      { label: 'Store Credit', value: 'store_credit' },
                      { label: 'Product Replacement', value: 'replacement' },
                      { label: 'Bank Refund', value: 'bank_refund' }
                    ]"
                    validation="required"
                    value="cash"
                  />
                  <FormKit
                    type="textarea"
                    name="notes"
                    label="Notes"
                    placeholder="Additional notes about this return..."
                    rows="3"
                  />
                </div>
              </div>

              <!-- Right Column - Return Items -->
              <div>
                <h4 class="font-medium text-gray-900 dark:text-white mb-3">Return Items</h4>
                <div v-if="selectedInvoiceItems.length > 0" class="space-y-3 max-h-64 overflow-y-auto">
                  <div v-for="item in selectedInvoiceItems" :key="item.id" class="border rounded-lg p-3 dark:border-gray-600">
                    <div class="flex items-center justify-between">
                      <div>
                        <div class="font-medium">{{ item.productName }}</div>
                        <div class="text-sm text-gray-500">SKU: {{ item.productSku }} | Sold: {{ item.quantity }} | {{ formatCurrency(item.unitPrice) }}</div>
                      </div>
                    </div>
                    <div class="mt-2 grid grid-cols-2 gap-2">
                      <FormKit
                        type="number"
                        :name="`items[${item.id}].quantityReturned`"
                        label="Qty to Return"
                        :min="0"
                        :max="item.quantity"
                        value="0"
                        outer-class="mb-0"
                      />
                      <FormKit
                        type="select"
                        :name="`items[${item.id}].condition`"
                        label="Condition"
                        :options="[
                          { label: 'Damaged', value: 'damaged' },
                          { label: 'Expired', value: 'expired' },
                          { label: 'Wrong Item', value: 'wrong_item' },
                          { label: 'Customer Return', value: 'customer_return' }
                        ]"
                        value="customer_return"
                        outer-class="mb-0"
                      />
                    </div>
                    <FormKit
                      type="textarea"
                      :name="`items[${item.id}].reason`"
                      label="Return Reason"
                      placeholder="Why is this item being returned?"
                      rows="2"
                      outer-class="mt-2 mb-0"
                    />
                  </div>
                </div>
                <div v-else class="text-center text-gray-500 py-8">
                  Select an invoice to see returnable items
                </div>
              </div>
            </div>

            <div class="mt-6 flex justify-end gap-3">
              <button type="button" @click="closeCreateModal" class="btn btn-secondary">
                Cancel
              </button>
              <FormKit type="submit" label="Create Credit Note" outer-class="!mb-0" />
            </div>
          </FormKit>
        </div>
      </div>
    </div>

    <!-- View Credit Note Modal -->
    <div v-if="viewingCreditNote" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-10 mx-auto p-5 border w-full max-w-4xl shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">
              Credit Note: {{ viewingCreditNote.creditNoteNumber }}
            </h3>
            <button @click="viewingCreditNote = null" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
              <Icon name="heroicons:x-mark" class="w-6 h-6" />
            </button>
          </div>

          <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
            <!-- Credit Note Details -->
            <div class="lg:col-span-2">
              <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-4 mb-4">
                <h4 class="font-medium text-gray-900 dark:text-white mb-3">Credit Note Information</h4>
                <dl class="grid grid-cols-2 gap-x-4 gap-y-2 text-sm">
                  <div>
                    <dt class="text-gray-500 dark:text-gray-400">Original Invoice:</dt>
                    <dd class="text-gray-900 dark:text-white font-medium">{{ viewingCreditNote.returnedInvoice }}</dd>
                  </div>
                  <div>
                    <dt class="text-gray-500 dark:text-gray-400">Customer:</dt>
                    <dd class="text-gray-900 dark:text-white">{{ viewingCreditNote.customer.name }}</dd>
                  </div>
                  <div>
                    <dt class="text-gray-500 dark:text-gray-400">Return Date:</dt>
                    <dd class="text-gray-900 dark:text-white">{{ formatDate(viewingCreditNote.returnDate) }}</dd>
                  </div>
                  <div>
                    <dt class="text-gray-500 dark:text-gray-400">Processed By:</dt>
                    <dd class="text-gray-900 dark:text-white">{{ viewingCreditNote.processedBy }}</dd>
                  </div>
                </dl>
              </div>

              <!-- Returned Items -->
              <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-4">
                <h4 class="font-medium text-gray-900 dark:text-white mb-3">Returned Items</h4>
                <div class="space-y-3">
                  <div v-for="item in viewingCreditNote.returnedItems" :key="item.id" class="bg-white dark:bg-gray-600 rounded p-3">
                    <div class="flex justify-between items-start">
                      <div>
                        <div class="font-medium">{{ item.productName }}</div>
                        <div class="text-sm text-gray-500 dark:text-gray-400">SKU: {{ item.productSku }}</div>
                        <div class="text-sm text-gray-500 dark:text-gray-400">Condition: {{ item.condition.replace('_', ' ') }}</div>
                      </div>
                      <div class="text-right">
                        <div class="font-medium">{{ item.quantityReturned }} × {{ formatCurrency(item.unitPrice) }}</div>
                        <div class="text-sm font-bold">{{ formatCurrency(item.lineTotal) }}</div>
                      </div>
                    </div>
                    <div class="mt-2 text-sm text-gray-600 dark:text-gray-300">
                      <strong>Reason:</strong> {{ item.reason }}
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Financial Summary & Actions -->
            <div class="space-y-4">
              <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-4">
                <h4 class="font-medium text-gray-900 dark:text-white mb-3">Financial Summary</h4>
                <dl class="space-y-2 text-sm">
                  <div class="flex justify-between">
                    <dt class="text-gray-500 dark:text-gray-400">Subtotal:</dt>
                    <dd class="text-gray-900 dark:text-white">{{ formatCurrency(viewingCreditNote.financials.subtotal) }}</dd>
                  </div>
                  <div class="flex justify-between">
                    <dt class="text-gray-500 dark:text-gray-400">VAT:</dt>
                    <dd class="text-gray-900 dark:text-white">{{ formatCurrency(viewingCreditNote.financials.vatAmount) }}</dd>
                  </div>
                  <div class="flex justify-between font-bold border-t pt-2 dark:border-gray-600">
                    <dt class="text-gray-900 dark:text-white">Total Credit:</dt>
                    <dd class="text-gray-900 dark:text-white">{{ formatCurrency(viewingCreditNote.financials.totalCreditAmount) }}</dd>
                  </div>
                </dl>
              </div>

              <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-4">
                <h4 class="font-medium text-gray-900 dark:text-white mb-3">Refund Details</h4>
                <dl class="space-y-2 text-sm">
                  <div class="flex justify-between">
                    <dt class="text-gray-500 dark:text-gray-400">Method:</dt>
                    <dd class="text-gray-900 dark:text-white capitalize">{{ formatRefundMethod(viewingCreditNote.refundMethod) }}</dd>
                  </div>
                  <div class="flex justify-between">
                    <dt class="text-gray-500 dark:text-gray-400">Status:</dt>
                    <dd>
                      <span :class="statusClasses(viewingCreditNote.refundStatus)" class="px-2 py-1 text-xs font-medium rounded-full capitalize">
                        {{ viewingCreditNote.refundStatus }}
                      </span>
                    </dd>
                  </div>
                  <div v-if="viewingCreditNote.refundReference">
                    <dt class="text-gray-500 dark:text-gray-400">Reference:</dt>
                    <dd class="text-gray-900 dark:text-white font-mono text-xs">{{ viewingCreditNote.refundReference }}</dd>
                  </div>
                </dl>
              </div>

              <!-- Action Buttons -->
              <div class="space-y-2">
                <button v-if="viewingCreditNote.refundStatus === 'pending'" @click="processRefundModal(viewingCreditNote)" class="btn btn-primary w-full">
                  <Icon name="heroicons:check-circle" class="w-4 h-4 mr-2" />
                  Process Refund
                </button>
                <button @click="printCreditNote(viewingCreditNote)" class="btn btn-secondary w-full">
                  <Icon name="heroicons:printer" class="w-4 h-4 mr-2" />
                  Print Credit Note
                </button>
              </div>
            </div>
          </div>

          <div class="mt-6 flex justify-end">
            <button @click="viewingCreditNote = null" class="btn btn-secondary">
              Close
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import type { CreditNote } from '~/types/sales'

useHead({
  title: 'Credit Notes & Returns',
})

const salesEnhanced = useSalesEnhanced()

// State
const creditNotes = ref<CreditNote[]>([])
const showCreateModal = ref(false)
const viewingCreditNote = ref<CreditNote | null>(null)
const loading = ref(false)
const selectedInvoiceItems = ref<any[]>([])
const availableInvoices = ref([
  { label: 'INV-001 - Jabu\'s Spaza (R 5,250.00)', value: 'INV-001' },
  { label: 'INV-002 - Sipho\'s Tavern (R 8,700.50)', value: 'INV-002' },
  { label: 'INV-003 - The Gogo Shop (R 3,200.00)', value: 'INV-003' }
])

// Filters
const filters = ref({
  search: '',
  status: 'All',
  refundMethod: 'All',
  startDate: '',
  endDate: ''
})

// Mock Data (in real app, this would come from API)
const mockCreditNotes: CreditNote[] = [
  {
    id: 'CN-001',
    creditNoteNumber: 'CN-2025-001',
    returnedInvoice: 'INV-001',
    customer: {
      id: 'cust-001',
      name: 'Jabu\'s Spaza',
      groupId: 'group-001'
    },
    returnDate: new Date('2025-11-10'),
    processedBy: 'System Admin',
    returnedItems: [
      {
        id: 'item-1',
        productId: 'prod-001',
        productName: 'Albany Bread',
        productSku: 'ALB-BREAD-001',
        quantityReturned: 5,
        unitPrice: 15.50,
        lineTotal: 77.50,
        reason: 'Expired bread found in delivery',
        condition: 'expired',
        restockable: false
      }
    ],
    financials: {
      subtotal: 67.39,
      vatAmount: 10.11,
      totalCreditAmount: 77.50
    },
    refundMethod: 'store_credit',
    refundStatus: 'completed',
    refundReference: 'SC-2025-001',
    createdAt: '2025-11-10T10:30:00Z',
    updatedAt: '2025-11-10T11:15:00Z'
  },
  {
    id: 'CN-002',
    creditNoteNumber: 'CN-2025-002',
    returnedInvoice: 'INV-002',
    customer: {
      id: 'cust-002',
      name: 'Sipho\'s Tavern',
      groupId: 'group-002'
    },
    returnDate: new Date('2025-11-12'),
    processedBy: 'System Admin',
    returnedItems: [
      {
        id: 'item-2',
        productId: 'prod-002',
        productName: 'Clover Milk 1L',
        productSku: 'CLV-MILK-1L',
        quantityReturned: 2,
        unitPrice: 22.00,
        lineTotal: 44.00,
        reason: 'Wrong product delivered',
        condition: 'wrong_item',
        restockable: true
      }
    ],
    financials: {
      subtotal: 38.26,
      vatAmount: 5.74,
      totalCreditAmount: 44.00
    },
    refundMethod: 'cash',
    refundStatus: 'pending',
    createdAt: '2025-11-12T14:20:00Z',
    updatedAt: '2025-11-12T14:20:00Z'
  }
]

// Initialize with mock data
creditNotes.value = mockCreditNotes

// Computed
const stats = computed(() => [
  {
    name: 'Total Returns',
    value: creditNotes.value.length,
    icon: 'heroicons:arrow-uturn-left',
    bgColor: 'bg-red-100',
    iconColor: 'text-red-600'
  },
  {
    name: 'Pending Refunds',
    value: creditNotes.value.filter(cn => cn.refundStatus === 'pending').length,
    icon: 'heroicons:clock',
    bgColor: 'bg-yellow-100',
    iconColor: 'text-yellow-600'
  },
  {
    name: 'Total Credit Amount',
    value: formatCurrency(creditNotes.value.reduce((acc, cn) => acc + cn.financials.totalCreditAmount, 0)),
    icon: 'heroicons:banknotes',
    bgColor: 'bg-green-100',
    iconColor: 'text-green-600'
  },
  {
    name: 'This Month',
    value: creditNotes.value.filter(cn => new Date(cn.returnDate).getMonth() === new Date().getMonth()).length,
    icon: 'heroicons:calendar',
    bgColor: 'bg-blue-100',
    iconColor: 'text-blue-600'
  }
])

const filteredCreditNotes = computed(() => {
  return creditNotes.value.filter(cn => {
    const searchMatch = filters.value.search ? 
      cn.creditNoteNumber.toLowerCase().includes(filters.value.search.toLowerCase()) ||
      cn.customer.name.toLowerCase().includes(filters.value.search.toLowerCase()) ||
      cn.returnedInvoice.toLowerCase().includes(filters.value.search.toLowerCase()) : true
    
    const statusMatch = filters.value.status === 'All' ? true : cn.refundStatus === filters.value.status.toLowerCase()
    const methodMatch = filters.value.refundMethod === 'All' ? true : cn.refundMethod === filters.value.refundMethod.toLowerCase().replace(' ', '_')
    
    let dateMatch = true
    if (filters.value.startDate) {
      dateMatch = dateMatch && new Date(cn.returnDate) >= new Date(filters.value.startDate)
    }
    if (filters.value.endDate) {
      dateMatch = dateMatch && new Date(cn.returnDate) <= new Date(filters.value.endDate)
    }
    
    return searchMatch && statusMatch && methodMatch && dateMatch
  })
})

// Methods
const createCreditNote = async (data: any) => {
  try {
    // Filter return items to only include those with quantity > 0
    const returnItems = Object.entries(data.items || {})
      .filter(([_, item]: [string, any]) => item.quantityReturned > 0)
      .map(([productId, item]: [string, any]) => ({
        productId,
        quantityReturned: item.quantityReturned,
        reason: item.reason || 'No reason provided',
        condition: item.condition,
        restockable: item.condition === 'wrong_item'
      }))

    if (returnItems.length === 0) {
      alert('Please select at least one item to return')
      return
    }

    const payload = {
      returnedInvoice: data.returnedInvoice,
      customerId: 'cust-001', // Should be derived from invoice
      returnedItems: returnItems,
      refundMethod: data.refundMethod,
      notes: data.notes
    }

    await salesEnhanced.createCreditNote(payload)
    closeCreateModal()
    // Reload credit notes in real app
  } catch (error) {
    console.error('Error creating credit note:', error)
  }
}

const loadInvoiceDetails = (invoiceId: string) => {
  // Mock invoice items - in real app, this would fetch from API
  const mockItems = {
    'INV-001': [
      { id: 'prod-001', productName: 'Albany Bread', productSku: 'ALB-BREAD-001', quantity: 10, unitPrice: 15.50 },
      { id: 'prod-002', productName: 'Clover Milk 1L', productSku: 'CLV-MILK-1L', quantity: 5, unitPrice: 22.00 }
    ],
    'INV-002': [
      { id: 'prod-002', productName: 'Clover Milk 1L', productSku: 'CLV-MILK-1L', quantity: 8, unitPrice: 22.00 },
      { id: 'prod-003', productName: 'Huletts Sugar 2kg', productSku: 'HUL-SUGAR-2KG', quantity: 3, unitPrice: 45.75 }
    ]
  }
  
  selectedInvoiceItems.value = mockItems[invoiceId as keyof typeof mockItems] || []
}

const viewCreditNote = (creditNote: CreditNote) => {
  viewingCreditNote.value = creditNote
}

const processRefund = async (creditNote: CreditNote) => {
  try {
    await salesEnhanced.processRefund(creditNote.id, {
      refundMethod: creditNote.refundMethod,
      refundReference: `REF-${Date.now()}`,
      notes: 'Processed automatically'
    })
    // Update local state
    const index = creditNotes.value.findIndex(cn => cn.id === creditNote.id)
    if (index !== -1) {
      creditNotes.value[index].refundStatus = 'processed'
    }
  } catch (error) {
    console.error('Error processing refund:', error)
  }
}

const processRefundModal = (creditNote: CreditNote) => {
  if (confirm(`Process refund of ${formatCurrency(creditNote.financials.totalCreditAmount)} for ${creditNote.customer.name}?`)) {
    processRefund(creditNote)
  }
}

const printCreditNote = (creditNote: CreditNote) => {
  // In real app, this would generate and print/download a PDF
  alert(`Printing credit note ${creditNote.creditNoteNumber}`)
}

const closeCreateModal = () => {
  showCreateModal.value = false
  selectedInvoiceItems.value = []
}

// Helper functions
const formatDate = (date: Date | string) => {
  return new Date(date).toLocaleDateString('en-ZA')
}

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('en-ZA', { style: 'currency', currency: 'ZAR' }).format(value || 0)
}

const formatRefundMethod = (method: string) => {
  return method.replace(/_/g, ' ')
}

const statusClasses = (status: string) => {
  const classes = {
    pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300',
    processed: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300',
    completed: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300'
  }
  return classes[status as keyof typeof classes] || classes.pending
}

const refundMethodClasses = (method: string) => {
  const classes = {
    cash: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300',
    store_credit: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-300',
    replacement: 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-300',
    bank_refund: 'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-300'
  }
  return classes[method as keyof typeof classes] || classes.cash
}
</script>

<style scoped>
.table-header {
  @apply px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider dark:text-gray-300;
}
.table-cell {
  @apply px-6 py-4 whitespace-nowrap text-sm text-gray-600 dark:text-gray-300;
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