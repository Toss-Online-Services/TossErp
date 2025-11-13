<template>
  <div class="p-6">
    <div class="flex items-center justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          Stock Movements
        </h1>
        <p class="text-gray-600 dark:text-gray-400">
          Track all stock transfers, adjustments and inventory changes
        </p>
      </div>
      <button class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg">
        <Icon name="heroicons:plus" class="w-4 h-4 mr-2" />
        Record Movement
      </button>
    </div>

    <!-- Movement Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-blue-100 rounded-full">
            <Icon name="mdi:transfer" class="w-5 h-5 text-blue-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Total Movements</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">2,456</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-green-100 rounded-full">
            <Icon name="heroicons:arrow-up-circle" class="w-5 h-5 text-green-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Stock In</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">1,847</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-red-100 rounded-full">
            <Icon name="heroicons:arrow-down-circle" class="w-5 h-5 text-red-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Stock Out</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">609</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-purple-100 rounded-full">
            <Icon name="heroicons:adjustments-horizontal" class="w-5 h-5 text-purple-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Adjustments</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">156</p>
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
            placeholder="Search movements..."
            class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:border-gray-600 dark:text-white"
          >
        </div>
        <select class="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:border-gray-600 dark:text-white">
          <option value="">All Types</option>
          <option value="inbound">Stock In</option>
          <option value="outbound">Stock Out</option>
          <option value="transfer">Transfer</option>
          <option value="adjustment">Adjustment</option>
          <option value="return">Return</option>
        </select>
        <select class="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:border-gray-600 dark:text-white">
          <option value="">All Locations</option>
          <option value="main">Main Warehouse</option>
          <option value="soweto">Soweto Hub</option>
          <option value="alexandra">Alexandra Hub</option>
          <option value="tembisa">Tembisa Hub</option>
        </select>
        <input 
          type="date" 
          class="px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:border-gray-600 dark:text-white"
        >
      </div>
    </div>

    <!-- Stock Movements List -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm">
      <div class="p-6">
        <h2 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
          Recent Stock Movements
        </h2>
        
        <div class="space-y-4">
          <div v-for="movement in mockMovements" :key="movement.id" 
               class="border border-gray-200 dark:border-gray-700 rounded-lg p-4">
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <div class="flex items-center mb-3">
                  <div class="w-12 h-12 rounded-lg flex items-center justify-center mr-4"
                       :class="getMovementTypeColor(movement.type)">
                    <Icon :name="getMovementTypeIcon(movement.type)" class="w-6 h-6 text-white" />
                  </div>
                  <div>
                    <h3 class="font-medium text-gray-900 dark:text-white text-lg">
                      {{ movement.reference }}
                    </h3>
                    <div class="flex items-center space-x-3">
                      <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                            :class="getTypeClass(movement.type)">
                        {{ formatMovementType(movement.type) }}
                      </span>
                      <span class="text-sm text-gray-600 dark:text-gray-400">
                        {{ formatDate(movement.movementDate) }} {{ formatTime(movement.movementDate) }}
                      </span>
                    </div>
                  </div>
                </div>
                
                <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Product:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      {{ movement.product.name }}<br>
                      SKU: {{ movement.product.sku }}
                    </div>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Quantity:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      <span :class="movement.quantity > 0 ? 'text-green-600' : 'text-red-600'">
                        {{ movement.quantity > 0 ? '+' : '' }}{{ movement.quantity }}
                      </span> {{ movement.product.unit }}<br>
                      Running: {{ movement.runningBalance }} {{ movement.product.unit }}
                    </div>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Location:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      {{ movement.location.from ? `${movement.location.from} → ` : '' }}{{ movement.location.to }}<br>
                      Zone: {{ movement.location.zone }}
                    </div>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">User & Cost:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      {{ movement.user }}<br>
                      Cost: R {{ movement.unitCost.toFixed(2) }}/unit
                    </div>
                  </div>
                </div>
                
                <div v-if="movement.reason" class="mt-3">
                  <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Reason:</span>
                  <p class="text-sm text-gray-600 dark:text-gray-400 mt-1">
                    {{ movement.reason }}
                  </p>
                </div>
                
                <div v-if="movement.batchNumber || movement.expiryDate" class="mt-3">
                  <div class="flex items-center space-x-6">
                    <div v-if="movement.batchNumber" class="flex items-center">
                      <Icon name="heroicons:hashtag" class="w-4 h-4 text-gray-500 mr-1" />
                      <span class="text-sm text-gray-600 dark:text-gray-400">
                        Batch: {{ movement.batchNumber }}
                      </span>
                    </div>
                    
                    <div v-if="movement.expiryDate" class="flex items-center">
                      <Icon name="heroicons:calendar" class="w-4 h-4 text-gray-500 mr-1" />
                      <span class="text-sm text-gray-600 dark:text-gray-400">
                        Expires: {{ formatDate(movement.expiryDate) }}
                      </span>
                    </div>
                  </div>
                </div>
                
                <div v-if="movement.relatedDocument" class="mt-3">
                  <div class="flex items-center">
                    <Icon name="heroicons:document-text" class="w-4 h-4 text-blue-600 mr-1" />
                    <span class="text-sm text-blue-600 hover:text-blue-800 cursor-pointer">
                      {{ movement.relatedDocument.type }}: {{ movement.relatedDocument.number }}
                    </span>
                  </div>
                </div>
              </div>
              
              <div class="flex items-center space-x-2 ml-4">
                <button class="text-blue-600 hover:text-blue-800 text-sm px-3 py-1 border border-blue-200 rounded">
                  View
                </button>
                <button v-if="movement.type === 'adjustment' && !movement.approved"
                        class="text-green-600 hover:text-green-800 text-sm px-3 py-1 border border-green-200 rounded">
                  Approve
                </button>
                <button class="text-purple-600 hover:text-purple-800 text-sm px-3 py-1 border border-purple-200 rounded">
                  Print
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
  title: 'Stock Movements - TOSS ERP'
})

// Mock data for demonstration
const mockMovements = ref([
  {
    id: 'movement-001',
    reference: 'STK-IN-2024-001',
    type: 'inbound',
    movementDate: new Date('2024-01-14T10:30:00'),
    product: {
      name: 'Sunlight Dishwashing Liquid 750ml',
      sku: 'SUN-750',
      unit: 'units'
    },
    quantity: 240,
    runningBalance: 356,
    unitCost: 12.50,
    location: {
      from: null,
      to: 'Main Warehouse',
      zone: 'A1-03'
    },
    reason: 'Purchase order PO-2024-001 delivery',
    user: 'Store Manager',
    batchNumber: 'SUN240114A',
    expiryDate: new Date('2026-01-14'),
    relatedDocument: {
      type: 'Purchase Order',
      number: 'PO-2024-001'
    },
    approved: true
  },
  {
    id: 'movement-002',
    reference: 'STK-OUT-2024-045',
    type: 'outbound',
    movementDate: new Date('2024-01-14T14:15:00'),
    product: {
      name: 'All Gold Tomato Sauce 700ml',
      sku: 'AG-700',
      unit: 'units'
    },
    quantity: -24,
    runningBalance: 78,
    unitCost: 15.80,
    location: {
      from: 'Main Warehouse',
      to: 'Soweto Hub',
      zone: 'B2-05'
    },
    reason: 'Transfer to Soweto distribution hub',
    user: 'Logistics Coordinator',
    batchNumber: 'AG231201B',
    expiryDate: new Date('2025-12-01'),
    relatedDocument: {
      type: 'Transfer Order',
      number: 'TO-2024-012'
    },
    approved: true
  },
  {
    id: 'movement-003',
    reference: 'STK-ADJ-2024-008',
    type: 'adjustment',
    movementDate: new Date('2024-01-14T16:45:00'),
    product: {
      name: 'Ace Maize Meal 2.5kg',
      sku: 'ACE-2.5',
      unit: 'kg'
    },
    quantity: -5,
    runningBalance: 245,
    unitCost: 28.90,
    location: {
      from: null,
      to: 'Main Warehouse',
      zone: 'C1-08'
    },
    reason: 'Stock count discrepancy - damaged packaging',
    user: 'Warehouse Supervisor',
    batchNumber: 'ACE240105C',
    expiryDate: new Date('2025-06-05'),
    relatedDocument: null,
    approved: false
  },
  {
    id: 'movement-004',
    reference: 'STK-RET-2024-003',
    type: 'return',
    movementDate: new Date('2024-01-13T11:20:00'),
    product: {
      name: 'Fresh Milk 1L',
      sku: 'MILK-1L',
      unit: 'litres'
    },
    quantity: 12,
    runningBalance: 89,
    unitCost: 18.90,
    location: {
      from: 'Alexandra Hub',
      to: 'Main Warehouse',
      zone: 'COLD-02'
    },
    reason: 'Customer return - approaching expiry date',
    user: 'Returns Handler',
    batchNumber: 'MILK240108A',
    expiryDate: new Date('2024-01-15'),
    relatedDocument: {
      type: 'Return Note',
      number: 'RET-2024-003'
    },
    approved: true
  },
  {
    id: 'movement-005',
    reference: 'STK-TRN-2024-019',
    type: 'transfer',
    movementDate: new Date('2024-01-13T09:10:00'),
    product: {
      name: 'Omo Washing Powder 2kg',
      sku: 'OMO-2',
      unit: 'units'
    },
    quantity: 0, // Transfer (out from one location, in to another)
    runningBalance: 156,
    unitCost: 67.50,
    location: {
      from: 'Tembisa Hub',
      to: 'Alexandra Hub',
      zone: 'D1-12'
    },
    reason: 'Inter-hub stock balancing',
    user: 'Operations Manager',
    batchNumber: 'OMO231220D',
    expiryDate: new Date('2026-12-20'),
    relatedDocument: {
      type: 'Transfer Order',
      number: 'TO-2024-011'
    },
    approved: true
  }
])

const getMovementTypeColor = (type: string) => {
  const colorMap: Record<string, string> = {
    inbound: 'bg-gradient-to-br from-green-500 to-green-600',
    outbound: 'bg-gradient-to-br from-red-500 to-red-600',
    transfer: 'bg-gradient-to-br from-blue-500 to-blue-600',
    adjustment: 'bg-gradient-to-br from-orange-500 to-orange-600',
    return: 'bg-gradient-to-br from-purple-500 to-purple-600'
  }
  return colorMap[type] || 'bg-gradient-to-br from-gray-500 to-gray-600'
}

const getMovementTypeIcon = (type: string) => {
  const iconMap: Record<string, string> = {
    inbound: 'heroicons:arrow-up-circle',
    outbound: 'heroicons:arrow-down-circle',
    transfer: 'mdi:transfer',
    adjustment: 'heroicons:adjustments-horizontal',
    return: 'heroicons:arrow-uturn-left'
  }
  return iconMap[type] || 'mdi:transfer'
}

const getTypeClass = (type: string) => {
  const classMap: Record<string, string> = {
    inbound: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    outbound: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    transfer: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    adjustment: 'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-200',
    return: 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200'
  }
  return classMap[type] || 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
}

const formatMovementType = (type: string) => {
  const typeMap: Record<string, string> = {
    inbound: 'Stock In',
    outbound: 'Stock Out',
    transfer: 'Transfer',
    adjustment: 'Adjustment',
    return: 'Return'
  }
  return typeMap[type] || type
}

const formatDate = (date: Date) => {
  return new Date(date).toLocaleDateString('en-ZA', { 
    year: 'numeric', 
    month: 'short', 
    day: 'numeric' 
  })
}

const formatTime = (date: Date) => {
  return new Date(date).toLocaleTimeString('en-ZA', { 
    hour: '2-digit', 
    minute: '2-digit' 
  })
}
</script>