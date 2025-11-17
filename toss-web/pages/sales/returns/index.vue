<template>
  <div class="p-6">
    <div class="flex items-center justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          Sales Returns
        </h1>
        <p class="text-gray-600 dark:text-gray-400">
          Track and manage product returns, refunds and exchanges
        </p>
      </div>
      <button class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg">
        <Icon name="heroicons:plus" class="w-4 h-4 mr-2" />
        Process Return
      </button>
    </div>

    <!-- Returns Stats -->
    <div class="grid grid-cols-1 md:grid-cols-5 gap-4 mb-6">
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-red-100 rounded-full">
            <Icon name="heroicons:arrow-left-on-rectangle" class="w-5 h-5 text-red-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Total Returns</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">142</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-orange-100 rounded-full">
            <Icon name="heroicons:clock" class="w-5 h-5 text-orange-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Pending</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">23</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-green-100 rounded-full">
            <Icon name="heroicons:check-circle" class="w-5 h-5 text-green-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Processed</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">119</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-purple-100 rounded-full">
            <Icon name="heroicons:banknotes" class="w-5 h-5 text-purple-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Refund Value</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">R 34K</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-blue-100 rounded-full">
            <Icon name="heroicons:chart-bar" class="w-5 h-5 text-blue-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Return Rate</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">3.2%</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Filter Bar -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm p-4 mb-6">
      <div class="flex flex-wrap gap-4">
        <div class="flex-1 min-w-64">
          <input 
            type="search" 
            placeholder="Search returns..."
            class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:border-gray-600 dark:text-white"
          >
        </div>
        <select class="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:border-gray-600 dark:text-white">
          <option value="">All Statuses</option>
          <option value="pending">Pending</option>
          <option value="approved">Approved</option>
          <option value="rejected">Rejected</option>
          <option value="processed">Processed</option>
        </select>
        <select class="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:border-gray-600 dark:text-white">
          <option value="">All Types</option>
          <option value="defective">Defective</option>
          <option value="damaged">Damaged</option>
          <option value="wrong-item">Wrong Item</option>
          <option value="expired">Expired</option>
          <option value="customer-change">Customer Change of Mind</option>
        </select>
        <input 
          type="date" 
          class="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:border-gray-600 dark:text-white"
        >
      </div>
    </div>

    <!-- Returns List -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm">
      <div class="p-6">
        <h2 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
          Recent Returns
        </h2>
        
        <div class="space-y-4">
          <div v-for="returnItem in mockReturns" :key="returnItem.id" 
               class="border border-gray-200 dark:border-gray-700 rounded-lg p-4">
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <div class="flex items-center mb-3">
                  <div class="w-12 h-12 rounded-lg flex items-center justify-center mr-4"
                       :class="getReturnStatusColor(returnItem.status)">
                    <Icon name="heroicons:arrow-left-on-rectangle" class="w-6 h-6 text-white" />
                  </div>
                  <div>
                    <h3 class="font-medium text-gray-900 dark:text-white text-lg">
                      Return #{{ returnItem.returnNumber }}
                    </h3>
                    <div class="flex items-center space-x-3">
                      <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                            :class="getStatusClass(returnItem.status)">
                        {{ formatStatus(returnItem.status) }}
                      </span>
                      <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                            :class="getReasonClass(returnItem.reason)">
                        {{ formatReason(returnItem.reason) }}
                      </span>
                      <span class="text-sm text-gray-600 dark:text-gray-400">
                        {{ formatDate(returnItem.createdDate) }}
                      </span>
                    </div>
                  </div>
                </div>
                
                <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Customer:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      {{ returnItem.customer.name }}<br>
                      {{ returnItem.customer.phone }}
                    </div>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Original Sale:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      Invoice #{{ returnItem.originalSale.invoiceNumber }}<br>
                      {{ formatDate(returnItem.originalSale.date) }}
                    </div>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Items & Value:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      {{ returnItem.items.length }} item(s)<br>
                      R {{ returnItem.refundAmount.toLocaleString() }}
                    </div>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Resolution:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      {{ formatResolution(returnItem.resolution) }}<br>
                      {{ returnItem.processedBy || 'Pending' }}
                    </div>
                  </div>
                </div>
                
                <div class="mt-3">
                  <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Items:</span>
                  <div class="mt-1">
                    <div v-for="item in returnItem.items" :key="item.id" 
                         class="inline-flex items-center px-2 py-1 text-xs bg-gray-50 text-gray-700 rounded mr-2 mb-1 dark:bg-gray-700 dark:text-gray-300">
                      {{ item.quantity }}x {{ item.product.name }}
                    </div>
                  </div>
                </div>
                
                <div v-if="returnItem.customerComments" class="mt-3">
                  <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Customer Comments:</span>
                  <p class="text-sm text-gray-600 dark:text-gray-400 mt-1 italic">
                    "{{ returnItem.customerComments }}"
                  </p>
                </div>
              </div>
              
              <div class="flex items-center space-x-2 ml-4">
                <button v-if="returnItem.status === 'pending'"
                        class="text-green-600 hover:text-green-800 text-sm px-3 py-1 border border-green-200 rounded">
                  Approve
                </button>
                <button v-if="returnItem.status === 'pending'"
                        class="text-red-600 hover:text-red-800 text-sm px-3 py-1 border border-red-200 rounded">
                  Reject
                </button>
                <button class="text-blue-600 hover:text-blue-800 text-sm px-3 py-1 border border-blue-200 rounded">
                  View Details
                </button>
                <button v-if="returnItem.status === 'approved'"
                        class="text-purple-600 hover:text-purple-800 text-sm px-3 py-1 border border-purple-200 rounded">
                  Process
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

useHead({
  title: 'Sales Returns - TOSS ERP'
})

// Mock data for demonstration
const mockReturns = ref([
  {
    id: 'return-001',
    returnNumber: 'RET2024-001',
    status: 'pending',
    reason: 'defective',
    customer: {
      name: 'Themba\'s Spaza',
      phone: '+27 82 345 6789',
      email: 'themba@spaza.co.za'
    },
    originalSale: {
      invoiceNumber: 'INV-2024-0156',
      date: new Date('2024-01-10'),
      total: 2450
    },
    items: [
      {
        id: 'item-1',
        product: { name: 'Sunlight Soap 500ml', sku: 'SUN-500' },
        quantity: 12,
        unitPrice: 15.50,
        reason: 'Damaged packaging'
      }
    ],
    refundAmount: 186,
    resolution: 'refund',
    customerComments: 'Some bottles were damaged during delivery, liquid leaked out',
    createdDate: new Date('2024-01-12'),
    processedBy: null
  },
  {
    id: 'return-002',
    returnNumber: 'RET2024-002',
    status: 'approved',
    reason: 'wrong-item',
    customer: {
      name: 'Nomsa\'s Tavern',
      phone: '+27 76 789 0123',
      email: 'nomsa@tavern.co.za'
    },
    originalSale: {
      invoiceNumber: 'INV-2024-0134',
      date: new Date('2024-01-08'),
      total: 890
    },
    items: [
      {
        id: 'item-2',
        product: { name: 'Castle Lite 330ml (24 pack)', sku: 'CST-330-24' },
        quantity: 2,
        unitPrice: 185,
        reason: 'Ordered Castle Lager, received Castle Lite'
      }
    ],
    refundAmount: 370,
    resolution: 'exchange',
    customerComments: 'I specifically ordered Castle Lager for the weekend customers',
    createdDate: new Date('2024-01-09'),
    processedBy: 'Admin User'
  },
  {
    id: 'return-003',
    returnNumber: 'RET2024-003',
    status: 'processed',
    reason: 'expired',
    customer: {
      name: 'Mpho\'s Mini Market',
      phone: '+27 72 123 4567',
      email: 'mpho@minimarket.co.za'
    },
    originalSale: {
      invoiceNumber: 'INV-2024-0145',
      date: new Date('2024-01-05'),
      total: 1245
    },
    items: [
      {
        id: 'item-3',
        product: { name: 'Fresh Milk 1L', sku: 'MILK-1L' },
        quantity: 24,
        unitPrice: 18.90,
        reason: 'Expired before sell-by date'
      }
    ],
    refundAmount: 453.60,
    resolution: 'refund',
    customerComments: 'Half the milk expired 2 days before the sell-by date printed on carton',
    createdDate: new Date('2024-01-07'),
    processedBy: 'Sarah Johnson'
  },
  {
    id: 'return-004',
    returnNumber: 'RET2024-004',
    status: 'rejected',
    reason: 'customer-change',
    customer: {
      name: 'Lucky\'s Tuck Shop',
      phone: '+27 83 456 7890',
      email: 'lucky@tuckshop.co.za'
    },
    originalSale: {
      invoiceNumber: 'INV-2024-0123',
      date: new Date('2023-12-20'),
      total: 340
    },
    items: [
      {
        id: 'item-4',
        product: { name: 'Christmas Decorations Set', sku: 'XMAS-DEC' },
        quantity: 5,
        unitPrice: 68,
        reason: 'Changed mind after Christmas'
      }
    ],
    refundAmount: 340,
    resolution: 'rejected',
    customerComments: 'Bought too many decorations, want to return unused ones',
    createdDate: new Date('2024-01-05'),
    processedBy: 'Admin User'
  }
])

const getReturnStatusColor = (status: string) => {
  const colorMap: Record<string, string> = {
    pending: 'bg-gradient-to-br from-orange-500 to-orange-600',
    approved: 'bg-gradient-to-br from-blue-500 to-blue-600',
    processed: 'bg-gradient-to-br from-green-500 to-green-600',
    rejected: 'bg-gradient-to-br from-red-500 to-red-600'
  }
  return colorMap[status] || 'bg-gradient-to-br from-gray-500 to-gray-600'
}

const getStatusClass = (status: string) => {
  const classMap: Record<string, string> = {
    pending: 'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-200',
    approved: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    processed: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    rejected: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  return classMap[status] || 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
}

const getReasonClass = (reason: string) => {
  const classMap: Record<string, string> = {
    defective: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    damaged: 'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-200',
    'wrong-item': 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200',
    expired: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    'customer-change': 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
  }
  return classMap[reason] || 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
}

const formatStatus = (status: string) => {
  const statusMap: Record<string, string> = {
    pending: 'Pending Review',
    approved: 'Approved',
    processed: 'Processed',
    rejected: 'Rejected'
  }
  return statusMap[status] || status
}

const formatReason = (reason: string) => {
  const reasonMap: Record<string, string> = {
    defective: 'Defective',
    damaged: 'Damaged',
    'wrong-item': 'Wrong Item',
    expired: 'Expired',
    'customer-change': 'Customer Change'
  }
  return reasonMap[reason] || reason
}

const formatResolution = (resolution: string) => {
  const resolutionMap: Record<string, string> = {
    refund: 'Full Refund',
    exchange: 'Exchange',
    credit: 'Store Credit',
    rejected: 'Rejected'
  }
  return resolutionMap[resolution] || resolution
}

const formatDate = (date: Date) => {
  return new Date(date).toLocaleDateString('en-ZA', { 
    year: 'numeric', 
    month: 'short', 
    day: 'numeric' 
  })
}
</script>