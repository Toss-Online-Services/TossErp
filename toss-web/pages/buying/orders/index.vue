<template>
  <div class="p-6">
    <div class="flex items-center justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          Purchase Orders
        </h1>
        <p class="text-gray-600 dark:text-gray-400">
          Manage purchase orders and supplier relationships
        </p>
      </div>
      <button class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg">
        <Icon name="heroicons:plus" class="w-4 h-4 mr-2" />
        Create Order
      </button>
    </div>

    <!-- Order Stats -->
    <div class="grid grid-cols-1 md:grid-cols-5 gap-4 mb-6">
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-blue-100 rounded-full">
            <Icon name="mdi:clipboard-list-outline" class="w-5 h-5 text-blue-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Total Orders</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">387</p>
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
            <Icon name="heroicons:truck" class="w-5 h-5 text-green-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">In Transit</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">45</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-purple-100 rounded-full">
            <Icon name="heroicons:check-circle" class="w-5 h-5 text-purple-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Received</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">319</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-indigo-100 rounded-full">
            <Icon name="heroicons:banknotes" class="w-5 h-5 text-indigo-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Order Value</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">R 2.1M</p>
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
            placeholder="Search orders..."
            class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:border-gray-600 dark:text-white"
          >
        </div>
        <select class="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:border-gray-600 dark:text-white">
          <option value="">All Statuses</option>
          <option value="pending">Pending</option>
          <option value="approved">Approved</option>
          <option value="ordered">Ordered</option>
          <option value="in-transit">In Transit</option>
          <option value="received">Received</option>
          <option value="cancelled">Cancelled</option>
        </select>
        <select class="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:border-gray-600 dark:text-white">
          <option value="">All Suppliers</option>
          <option value="megasave">Mega Save Wholesalers</option>
          <option value="unilever">Unilever SA</option>
          <option value="cocacola">Coca-Cola SA</option>
          <option value="tiger">Tiger Brands</option>
        </select>
        <input 
          type="date" 
          class="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:border-gray-600 dark:text-white"
        >
      </div>
    </div>

    <!-- Purchase Orders List -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm">
      <div class="p-6">
        <h2 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
          Recent Purchase Orders
        </h2>
        
        <div class="space-y-4">
          <div v-for="order in mockOrders" :key="order.id" 
               class="border border-gray-200 dark:border-gray-700 rounded-lg p-4">
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <div class="flex items-center mb-3">
                  <div class="w-12 h-12 rounded-lg flex items-center justify-center mr-4"
                       :class="getOrderStatusColor(order.status)">
                    <Icon name="mdi:clipboard-list-outline" class="w-6 h-6 text-white" />
                  </div>
                  <div>
                    <h3 class="font-medium text-gray-900 dark:text-white text-lg">
                      PO #{{ order.orderNumber }}
                    </h3>
                    <div class="flex items-center space-x-3">
                      <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                            :class="getStatusClass(order.status)">
                        {{ formatStatus(order.status) }}
                      </span>
                      <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                            :class="getPriorityClass(order.priority)">
                        {{ formatPriority(order.priority) }}
                      </span>
                      <span class="text-sm text-gray-600 dark:text-gray-400">
                        {{ formatDate(order.orderDate) }}
                      </span>
                    </div>
                  </div>
                </div>
                
                <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Supplier:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      {{ order.supplier.name }}<br>
                      {{ order.supplier.contact }}
                    </div>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Order Details:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      {{ order.items.length }} item(s)<br>
                      R {{ order.totalAmount.toLocaleString() }}
                    </div>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Delivery:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      Expected: {{ formatDate(order.expectedDelivery) }}<br>
                      {{ order.deliveryMethod }}
                    </div>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Payment:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      {{ order.paymentTerms }}<br>
                      Due: {{ formatDate(order.paymentDueDate) }}
                    </div>
                  </div>
                </div>
                
                <div class="mt-3">
                  <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Items:</span>
                  <div class="mt-1">
                    <div v-for="item in order.items.slice(0, 3)" :key="item.id" 
                         class="inline-flex items-center px-2 py-1 text-xs bg-gray-50 text-gray-700 rounded mr-2 mb-1 dark:bg-gray-700 dark:text-gray-300">
                      {{ item.quantity }}x {{ item.product.name }}
                    </div>
                    <span v-if="order.items.length > 3" 
                          class="inline-flex items-center px-2 py-1 text-xs bg-blue-50 text-blue-700 rounded dark:bg-blue-900 dark:text-blue-300">
                      +{{ order.items.length - 3 }} more
                    </span>
                  </div>
                </div>
                
                <div v-if="order.notes" class="mt-3">
                  <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Notes:</span>
                  <p class="text-sm text-gray-600 dark:text-gray-400 mt-1">
                    {{ order.notes }}
                  </p>
                </div>
                
                <div v-if="order.tracking" class="mt-3">
                  <div class="flex items-center space-x-4">
                    <div class="flex items-center">
                      <Icon name="heroicons:truck" class="w-4 h-4 text-blue-600 mr-1" />
                      <span class="text-sm text-gray-600 dark:text-gray-400">
                        Tracking: {{ order.tracking.number }}
                      </span>
                    </div>
                    <div class="flex items-center">
                      <Icon name="heroicons:map-pin" class="w-4 h-4 text-green-600 mr-1" />
                      <span class="text-sm text-gray-600 dark:text-gray-400">
                        {{ order.tracking.location }}
                      </span>
                    </div>
                  </div>
                </div>
              </div>
              
              <div class="flex items-center space-x-2 ml-4">
                <button v-if="order.status === 'pending'"
                        class="text-green-600 hover:text-green-800 text-sm px-3 py-1 border border-green-200 rounded">
                  Approve
                </button>
                <button v-if="order.status === 'in-transit'"
                        class="text-blue-600 hover:text-blue-800 text-sm px-3 py-1 border border-blue-200 rounded">
                  Track
                </button>
                <button v-if="order.status === 'in-transit'"
                        class="text-purple-600 hover:text-purple-800 text-sm px-3 py-1 border border-purple-200 rounded">
                  Receive
                </button>
                <button class="text-gray-600 hover:text-gray-800 text-sm px-3 py-1 border border-gray-200 rounded">
                  View
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
  title: 'Purchase Orders - TOSS ERP'
})

// Mock data for demonstration
const mockOrders = ref([
  {
    id: 'po-001',
    orderNumber: '2024-001',
    status: 'in-transit',
    priority: 'high',
    supplier: {
      name: 'Mega Save Wholesalers',
      contact: 'Tshepo Mthembu',
      phone: '+27 11 789 0123'
    },
    orderDate: new Date('2024-01-10'),
    expectedDelivery: new Date('2024-01-15'),
    paymentDueDate: new Date('2024-02-10'),
    paymentTerms: '30 days',
    deliveryMethod: 'Direct delivery',
    items: [
      {
        id: 'item-1',
        product: { name: 'Sunlight Dishwashing Liquid 750ml', sku: 'SUN-750' },
        quantity: 240,
        unitPrice: 12.50,
        totalPrice: 3000
      },
      {
        id: 'item-2',
        product: { name: 'All Gold Tomato Sauce 700ml', sku: 'AG-700' },
        quantity: 120,
        unitPrice: 15.80,
        totalPrice: 1896
      },
      {
        id: 'item-3',
        product: { name: 'Ace Maize Meal 2.5kg', sku: 'ACE-2.5' },
        quantity: 100,
        unitPrice: 28.90,
        totalPrice: 2890
      }
    ],
    totalAmount: 7786,
    notes: 'Rush order for weekend stock replenishment',
    tracking: {
      number: 'TRK-789456',
      location: 'Johannesburg DC - Out for delivery'
    },
    createdBy: 'Admin User'
  },
  {
    id: 'po-002',
    orderNumber: '2024-002',
    status: 'pending',
    priority: 'medium',
    supplier: {
      name: 'Unilever South Africa',
      contact: 'Sarah Johnson',
      phone: '+27 11 456 7890'
    },
    orderDate: new Date('2024-01-12'),
    expectedDelivery: new Date('2024-01-18'),
    paymentDueDate: new Date('2024-02-12'),
    paymentTerms: '30 days',
    deliveryMethod: 'Supplier delivery',
    items: [
      {
        id: 'item-4',
        product: { name: 'Knorr Stock Cubes (24 pack)', sku: 'KNR-24' },
        quantity: 50,
        unitPrice: 45.60,
        totalPrice: 2280
      },
      {
        id: 'item-5',
        product: { name: 'Omo Washing Powder 2kg', sku: 'OMO-2' },
        quantity: 80,
        unitPrice: 67.50,
        totalPrice: 5400
      }
    ],
    totalAmount: 7680,
    notes: 'Monthly stock replenishment order',
    tracking: null,
    createdBy: 'Store Manager'
  },
  {
    id: 'po-003',
    orderNumber: '2024-003',
    status: 'received',
    priority: 'low',
    supplier: {
      name: 'Coca-Cola South Africa',
      contact: 'Mike Chen',
      phone: '+27 11 234 5678'
    },
    orderDate: new Date('2024-01-08'),
    expectedDelivery: new Date('2024-01-12'),
    paymentDueDate: new Date('2024-02-08'),
    paymentTerms: '30 days',
    deliveryMethod: 'Direct delivery',
    items: [
      {
        id: 'item-6',
        product: { name: 'Coca-Cola 330ml (24 pack)', sku: 'CC-330-24' },
        quantity: 20,
        unitPrice: 89.90,
        totalPrice: 1798
      },
      {
        id: 'item-7',
        product: { name: 'Sprite 330ml (24 pack)', sku: 'SPR-330-24' },
        quantity: 15,
        unitPrice: 89.90,
        totalPrice: 1348.50
      },
      {
        id: 'item-8',
        product: { name: 'Fanta Orange 330ml (24 pack)', sku: 'FAN-330-24' },
        quantity: 10,
        unitPrice: 89.90,
        totalPrice: 899
      }
    ],
    totalAmount: 4045.50,
    notes: 'Summer beverage stock increase',
    tracking: {
      number: 'TRK-654321',
      location: 'Delivered - Signed by Store Manager'
    },
    createdBy: 'Admin User'
  },
  {
    id: 'po-004',
    orderNumber: '2024-004',
    status: 'cancelled',
    priority: 'medium',
    supplier: {
      name: 'Tiger Brands',
      contact: 'David Wilson',
      phone: '+27 11 987 6543'
    },
    orderDate: new Date('2024-01-05'),
    expectedDelivery: new Date('2024-01-12'),
    paymentDueDate: new Date('2024-02-05'),
    paymentTerms: '30 days',
    deliveryMethod: 'Supplier delivery',
    items: [
      {
        id: 'item-9',
        product: { name: 'Jungle Oats 1kg', sku: 'JO-1' },
        quantity: 60,
        unitPrice: 34.90,
        totalPrice: 2094
      }
    ],
    totalAmount: 2094,
    notes: 'Cancelled due to supplier stock shortage',
    tracking: null,
    createdBy: 'Procurement Manager'
  }
])

const getOrderStatusColor = (status: string) => {
  const colorMap: Record<string, string> = {
    pending: 'bg-gradient-to-br from-orange-500 to-orange-600',
    approved: 'bg-gradient-to-br from-blue-500 to-blue-600',
    ordered: 'bg-gradient-to-br from-indigo-500 to-indigo-600',
    'in-transit': 'bg-gradient-to-br from-purple-500 to-purple-600',
    received: 'bg-gradient-to-br from-green-500 to-green-600',
    cancelled: 'bg-gradient-to-br from-red-500 to-red-600'
  }
  return colorMap[status] || 'bg-gradient-to-br from-gray-500 to-gray-600'
}

const getStatusClass = (status: string) => {
  const classMap: Record<string, string> = {
    pending: 'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-200',
    approved: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    ordered: 'bg-indigo-100 text-indigo-800 dark:bg-indigo-900 dark:text-indigo-200',
    'in-transit': 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200',
    received: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    cancelled: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  return classMap[status] || 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
}

const getPriorityClass = (priority: string) => {
  const classMap: Record<string, string> = {
    high: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    medium: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    low: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
  }
  return classMap[priority] || 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
}

const formatStatus = (status: string) => {
  const statusMap: Record<string, string> = {
    pending: 'Pending Approval',
    approved: 'Approved',
    ordered: 'Ordered',
    'in-transit': 'In Transit',
    received: 'Received',
    cancelled: 'Cancelled'
  }
  return statusMap[status] || status
}

const formatPriority = (priority: string) => {
  const priorityMap: Record<string, string> = {
    high: 'High Priority',
    medium: 'Medium Priority',
    low: 'Low Priority'
  }
  return priorityMap[priority] || priority
}

const formatDate = (date: Date) => {
  return new Date(date).toLocaleDateString('en-ZA', { 
    year: 'numeric', 
    month: 'short', 
    day: 'numeric' 
  })
}
</script>