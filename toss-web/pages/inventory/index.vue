<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Inventory Management</h1>
              <p class="text-gray-600 dark:text-gray-400">Track stock levels, manage warehouses, and optimize inventory</p>
            </div>
            <div class="flex space-x-3">
              <button @click="showAddStockModal = true" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors">
                <PlusIcon class="w-5 h-5 inline mr-2" />
                Add Stock
              </button>
              <button @click="showNewItemModal = true" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors">
                <CubeIcon class="w-5 h-5 inline mr-2" />
                New Item
              </button>
              <button @click="initiateStockTake" class="bg-purple-600 text-white px-4 py-2 rounded-lg hover:bg-purple-700 transition-colors">
                <ClipboardListIcon class="w-5 h-5 inline mr-2" />
                Stock Take
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Stats Cards -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Total Items</p>
              <p class="text-2xl font-bold text-blue-600">{{ stats.totalItems }}</p>
            </div>
            <div class="p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
              <CubeIcon class="w-6 h-6 text-blue-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Low Stock Items</p>
              <p class="text-2xl font-bold text-red-600">{{ stats.lowStockItems }}</p>
            </div>
            <div class="p-3 bg-red-100 dark:bg-red-900 rounded-full">
              <ExclamationTriangleIcon class="w-6 h-6 text-red-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Total Value</p>
              <p class="text-2xl font-bold text-green-600">R {{ formatCurrency(stats.totalValue) }}</p>
            </div>
            <div class="p-3 bg-green-100 dark:bg-green-900 rounded-full">
              <CurrencyDollarIcon class="w-6 h-6 text-green-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-600 dark:text-gray-400">Warehouses</p>
              <p class="text-2xl font-bold text-purple-600">{{ stats.warehouses }}</p>
            </div>
            <div class="p-3 bg-purple-100 dark:bg-purple-900 rounded-full">
              <BuildingStorefrontIcon class="w-6 h-6 text-purple-600" />
            </div>
          </div>
        </div>
      </div>

      <!-- Main Content Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
        <!-- Inventory Overview -->
        <div class="lg:col-span-2">
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
            <div class="p-6 border-b border-gray-200 dark:border-gray-700">
              <div class="flex items-center justify-between">
                <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Inventory Overview</h3>
                <div class="flex space-x-2">
                  <select class="text-sm border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-1 bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
                    <option>All Categories</option>
                    <option>Electronics</option>
                    <option>Clothing</option>
                    <option>Food & Beverages</option>
                    <option>Office Supplies</option>
                  </select>
                  <select class="text-sm border border-gray-300 dark:border-gray-600 rounded-lg px-3 py-1 bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
                    <option>All Warehouses</option>
                    <option>Main Warehouse</option>
                    <option>Store Front</option>
                    <option>Storage Room</option>
                  </select>
                </div>
              </div>
            </div>
            <div class="p-6">
              <div class="overflow-x-auto">
                <table class="w-full">
                  <thead>
                    <tr class="text-left text-xs font-medium text-gray-500 dark:text-gray-400 uppercase tracking-wider">
                      <th class="pb-3">Item</th>
                      <th class="pb-3">Stock Level</th>
                      <th class="pb-3">Unit Price</th>
                      <th class="pb-3">Value</th>
                      <th class="pb-3">Status</th>
                    </tr>
                  </thead>
                  <tbody class="space-y-2">
                    <tr v-for="item in inventoryItems" :key="item.id" class="border-b border-gray-100 dark:border-gray-700">
                      <td class="py-3">
                        <div class="flex items-center space-x-3">
                          <div class="w-10 h-10 bg-gray-100 dark:bg-gray-700 rounded-lg flex items-center justify-center">
                            <CubeIcon class="w-5 h-5 text-gray-600" />
                          </div>
                          <div>
                            <p class="font-medium text-gray-900 dark:text-white">{{ item.name }}</p>
                            <p class="text-sm text-gray-600 dark:text-gray-400">{{ item.category }}</p>
                          </div>
                        </div>
                      </td>
                      <td class="py-3">
                        <div>
                          <p class="font-medium text-gray-900 dark:text-white">{{ item.currentStock }}</p>
                          <p class="text-sm text-gray-600 dark:text-gray-400">Min: {{ item.minStock }}</p>
                        </div>
                      </td>
                      <td class="py-3 text-gray-900 dark:text-white">R {{ formatCurrency(item.unitPrice) }}</td>
                      <td class="py-3 font-medium text-gray-900 dark:text-white">R {{ formatCurrency(item.currentStock * item.unitPrice) }}</td>
                      <td class="py-3">
                        <span class="inline-flex px-2 py-1 text-xs font-semibold rounded-full" :class="getStockStatusColor(item)">
                          {{ getStockStatus(item) }}
                        </span>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>

        <!-- Quick Actions & Alerts -->
        <div class="space-y-6">
          <!-- Low Stock Alerts -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
            <div class="p-6 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Stock Alerts</h3>
            </div>
            <div class="p-6">
              <div class="space-y-4">
                <div v-for="alert in stockAlerts" :key="alert.id" class="flex items-center space-x-3 p-3 bg-red-50 dark:bg-red-900/20 rounded-lg border border-red-200 dark:border-red-800">
                  <ExclamationTriangleIcon class="w-5 h-5 text-red-600 flex-shrink-0" />
                  <div class="flex-1">
                    <p class="text-sm font-medium text-red-800 dark:text-red-400">{{ alert.item }}</p>
                    <p class="text-xs text-red-600 dark:text-red-500">{{ alert.current }} left (Min: {{ alert.minimum }})</p>
                  </div>
                  <button @click="reorderItem(alert.id)" class="text-red-600 hover:text-red-800 text-xs bg-white dark:bg-gray-800 px-2 py-1 rounded border border-red-300 dark:border-red-700">
                    Reorder
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Recent Movements -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
            <div class="p-6 border-b border-gray-200 dark:border-gray-700">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Recent Movements</h3>
            </div>
            <div class="p-6">
              <div class="space-y-4">
                <div v-for="movement in recentMovements" :key="movement.id" class="flex items-center justify-between">
                  <div class="flex items-center space-x-3">
                    <div class="w-8 h-8 rounded-full flex items-center justify-center" :class="getMovementColor(movement.type)">
                      <component :is="getMovementIcon(movement.type)" class="w-4 h-4 text-white" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-gray-900 dark:text-white">{{ movement.item }}</p>
                      <p class="text-xs text-gray-600 dark:text-gray-400">{{ movement.type }} • {{ formatDate(movement.date) }}</p>
                    </div>
                  </div>
                  <div class="text-right">
                    <p class="text-sm font-medium" :class="movement.type === 'IN' ? 'text-green-600' : 'text-red-600'">
                      {{ movement.type === 'IN' ? '+' : '-' }}{{ movement.quantity }}
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Group Purchasing Opportunities -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 mb-8">
        <div class="p-6 border-b border-gray-200 dark:border-gray-700">
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Group Purchasing Opportunities</h3>
            <button class="text-blue-600 hover:text-blue-800 text-sm font-medium">View All</button>
          </div>
        </div>
        <div class="p-6">
          <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div v-for="opportunity in groupPurchasingOpportunities" :key="opportunity.id" class="border border-gray-200 dark:border-gray-700 rounded-lg p-4 hover:shadow-md transition-shadow">
              <div class="flex items-center justify-between mb-3">
                <h4 class="font-medium text-gray-900 dark:text-white">{{ opportunity.item }}</h4>
                <span class="text-xs bg-green-100 dark:bg-green-900 text-green-800 dark:text-green-400 px-2 py-1 rounded-full">
                  Save {{ opportunity.savings }}%
                </span>
              </div>
              <div class="space-y-2">
                <div class="flex justify-between text-sm">
                  <span class="text-gray-600 dark:text-gray-400">Current Price:</span>
                  <span class="text-gray-900 dark:text-white">R {{ formatCurrency(opportunity.currentPrice) }}</span>
                </div>
                <div class="flex justify-between text-sm">
                  <span class="text-gray-600 dark:text-gray-400">Group Price:</span>
                  <span class="text-green-600 font-medium">R {{ formatCurrency(opportunity.groupPrice) }}</span>
                </div>
                <div class="flex justify-between text-sm">
                  <span class="text-gray-600 dark:text-gray-400">Participants:</span>
                  <span class="text-gray-900 dark:text-white">{{ opportunity.participants }}</span>
                </div>
              </div>
              <button @click="joinGroupPurchase(opportunity.id)" class="mt-4 w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition-colors text-sm">
                Join Group Purchase
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Icons (would normally import from a proper icon library)
const PlusIcon = 'svg'
const CubeIcon = 'svg'
const ClipboardListIcon = 'svg'
const ExclamationTriangleIcon = 'svg'
const CurrencyDollarIcon = 'svg'
const BuildingStorefrontIcon = 'svg'

// Sample data
const stats = ref({
  totalItems: 1247,
  lowStockItems: 23,
  totalValue: 650000,
  warehouses: 3
})

const inventoryItems = ref([
  {
    id: 1,
    name: 'Maize Meal 5kg',
    category: 'Food & Beverages',
    currentStock: 15,
    minStock: 20,
    unitPrice: 45.99,
    lastUpdated: new Date()
  },
  {
    id: 2,
    name: 'Cooking Oil 2L',
    category: 'Food & Beverages',
    currentStock: 8,
    minStock: 15,
    unitPrice: 89.99,
    lastUpdated: new Date()
  },
  {
    id: 3,
    name: 'Office Paper A4',
    category: 'Office Supplies',
    currentStock: 50,
    minStock: 10,
    unitPrice: 12.50,
    lastUpdated: new Date()
  },
  {
    id: 4,
    name: 'USB Flash Drive 32GB',
    category: 'Electronics',
    currentStock: 25,
    minStock: 5,
    unitPrice: 125.00,
    lastUpdated: new Date()
  }
])

const stockAlerts = ref([
  { id: 1, item: 'Maize Meal 5kg', current: 15, minimum: 20 },
  { id: 2, item: 'Cooking Oil 2L', current: 8, minimum: 15 },
  { id: 3, item: 'Sugar 2.5kg', current: 3, minimum: 10 }
])

const recentMovements = ref([
  { id: 1, item: 'Office Paper A4', type: 'IN', quantity: 20, date: new Date() },
  { id: 2, item: 'USB Flash Drive', type: 'OUT', quantity: 5, date: new Date(Date.now() - 3600000) },
  { id: 3, item: 'Maize Meal 5kg', type: 'OUT', quantity: 10, date: new Date(Date.now() - 7200000) }
])

const groupPurchasingOpportunities = ref([
  {
    id: 1,
    item: 'Bulk Maize Meal (50x 5kg)',
    currentPrice: 45.99,
    groupPrice: 35.99,
    savings: 22,
    participants: 8
  },
  {
    id: 2,
    item: 'Cooking Oil Case (12x 2L)',
    currentPrice: 89.99,
    groupPrice: 74.99,
    savings: 17,
    participants: 5
  },
  {
    id: 3,
    item: 'Office Supplies Bundle',
    currentPrice: 350.00,
    groupPrice: 275.00,
    savings: 21,
    participants: 12
  }
])

const showAddStockModal = ref(false)
const showNewItemModal = ref(false)

// Helper functions
function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('en-ZA', {
    style: 'decimal',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

function formatDate(date: Date): string {
  return new Intl.DateTimeFormat('en-ZA', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date)
}

function getStockStatus(item: any): string {
  if (item.currentStock <= item.minStock) return 'Low Stock'
  if (item.currentStock <= item.minStock * 1.5) return 'Reorder Soon'
  return 'In Stock'
}

function getStockStatusColor(item: any): string {
  if (item.currentStock <= item.minStock) return 'bg-red-100 text-red-800'
  if (item.currentStock <= item.minStock * 1.5) return 'bg-yellow-100 text-yellow-800'
  return 'bg-green-100 text-green-800'
}

function getMovementColor(type: string): string {
  return type === 'IN' ? 'bg-green-500' : 'bg-red-500'
}

function getMovementIcon(type: string) {
  return type === 'IN' ? 'svg' : 'svg'
}

function initiateStockTake() {
  // TODO: Implement stock take functionality
  console.log('Initiating stock take...')
}

function reorderItem(alertId: number) {
  // TODO: Implement reorder functionality
  console.log('Reordering item:', alertId)
}

function joinGroupPurchase(opportunityId: number) {
  // TODO: Implement group purchase join functionality
  console.log('Joining group purchase:', opportunityId)
}

// Page metadata
useHead({
  title: 'Inventory - TOSS ERP',
  meta: [
    { name: 'description', content: 'Inventory management and stock control in TOSS ERP' }
  ]
})
</script>
