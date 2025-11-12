<template>
  <AppLayout>
    <div class="p-6 space-y-6">
      <!-- Header -->
      <div class="flex justify-between items-center">
        <div>
          <h1 class="text-2xl font-bold">Stock Adjustments</h1>
          <p class="text-muted-foreground">Manage inventory adjustments and stock corrections</p>
        </div>
        <div class="flex items-center space-x-4">
          <Button variant="outline">
            <Download class="h-4 w-4 mr-2" />
            Export Report
          </Button>
          <Button>
            <Plus class="h-4 w-4 mr-2" />
            New Adjustment
          </Button>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
        <Button variant="outline" class="h-20 flex-col space-y-2">
          <TrendingUp class="h-6 w-6 text-green-600" />
          <span class="text-sm">Stock In</span>
        </Button>
        <Button variant="outline" class="h-20 flex-col space-y-2">
          <TrendingDown class="h-6 w-6 text-red-600" />
          <span class="text-sm">Stock Out</span>
        </Button>
        <Button variant="outline" class="h-20 flex-col space-y-2">
          <RotateCcw class="h-6 w-6 text-blue-600" />
          <span class="text-sm">Stock Count</span>
        </Button>
        <Button variant="outline" class="h-20 flex-col space-y-2">
          <AlertTriangle class="h-6 w-6 text-yellow-600" />
          <span class="text-sm">Damaged Stock</span>
        </Button>
      </div>

      <!-- Recent Adjustments Summary -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-green-100 dark:bg-green-900 rounded-lg">
              <Plus class="h-6 w-6 text-green-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Stock Added</p>
              <p class="text-2xl font-bold">{{ stockAddedToday }}</p>
              <p class="text-xs text-green-600">Today</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-red-100 dark:bg-red-900 rounded-lg">
              <Minus class="h-6 w-6 text-red-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Stock Removed</p>
              <p class="text-2xl font-bold">{{ stockRemovedToday }}</p>
              <p class="text-xs text-red-600">Today</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-yellow-100 dark:bg-yellow-900 rounded-lg">
              <AlertCircle class="h-6 w-6 text-yellow-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Pending Approvals</p>
              <p class="text-2xl font-bold">{{ pendingApprovals }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-blue-100 dark:bg-blue-900 rounded-lg">
              <FileText class="h-6 w-6 text-blue-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Total Adjustments</p>
              <p class="text-2xl font-bold">{{ totalAdjustments }}</p>
              <p class="text-xs text-muted-foreground">This month</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Filters -->
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg border">
        <div class="flex flex-col md:flex-row md:items-center space-y-4 md:space-y-0 md:space-x-4">
          <div class="relative flex-1">
            <Search class="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search adjustments..."
              class="pl-10 pr-4 py-2 w-full rounded-md border border-input bg-background"
            />
          </div>
          <select v-model="typeFilter" class="px-3 py-2 border rounded-md">
            <option value="">All Types</option>
            <option value="stock-in">Stock In</option>
            <option value="stock-out">Stock Out</option>
            <option value="stock-count">Stock Count</option>
            <option value="damage">Damage/Loss</option>
            <option value="expired">Expired Items</option>
          </select>
          <select v-model="statusFilter" class="px-3 py-2 border rounded-md">
            <option value="">All Status</option>
            <option value="pending">Pending</option>
            <option value="approved">Approved</option>
            <option value="rejected">Rejected</option>
          </select>
          <input
            v-model="dateFilter"
            type="date"
            class="px-3 py-2 border rounded-md"
          />
        </div>
      </div>

      <!-- Adjustments Table -->
      <div class="bg-white dark:bg-gray-800 rounded-lg border">
        <div class="p-4 border-b">
          <h3 class="text-lg font-semibold">Stock Adjustment History</h3>
        </div>
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead>
              <tr class="border-b">
                <th class="text-left p-4 font-medium">Adjustment ID</th>
                <th class="text-left p-4 font-medium">Product</th>
                <th class="text-left p-4 font-medium">Type</th>
                <th class="text-left p-4 font-medium">Quantity</th>
                <th class="text-left p-4 font-medium">Reason</th>
                <th class="text-left p-4 font-medium">Date</th>
                <th class="text-left p-4 font-medium">User</th>
                <th class="text-left p-4 font-medium">Status</th>
                <th class="text-left p-4 font-medium">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="adjustment in filteredAdjustments" :key="adjustment.id" class="border-b hover:bg-muted/50">
                <td class="p-4 font-mono text-sm">{{ adjustment.id }}</td>
                <td class="p-4">
                  <div class="flex items-center space-x-3">
                    <div class="w-8 h-8 bg-gray-100 dark:bg-gray-700 rounded-lg flex items-center justify-center">
                      <Package class="h-4 w-4 text-gray-400" />
                    </div>
                    <div>
                      <p class="font-medium">{{ adjustment.productName }}</p>
                      <p class="text-sm text-muted-foreground">{{ adjustment.productCode }}</p>
                    </div>
                  </div>
                </td>
                <td class="p-4">
                  <span class="px-2 py-1 rounded text-xs font-medium" :class="getTypeColor(adjustment.type)">
                    {{ adjustment.type.replace('-', ' ').replace(/\b\w/g, l => l.toUpperCase()) }}
                  </span>
                </td>
                <td class="p-4">
                  <span :class="adjustment.quantity > 0 ? 'text-green-600' : 'text-red-600'" class="font-medium">
                    {{ adjustment.quantity > 0 ? '+' : '' }}{{ adjustment.quantity }} {{ adjustment.unit }}
                  </span>
                </td>
                <td class="p-4 text-muted-foreground">{{ adjustment.reason }}</td>
                <td class="p-4 text-muted-foreground">{{ adjustment.date }}</td>
                <td class="p-4">{{ adjustment.user }}</td>
                <td class="p-4">
                  <span class="px-2 py-1 rounded text-xs font-medium" :class="getStatusColor(adjustment.status)">
                    {{ adjustment.status }}
                  </span>
                </td>
                <td class="p-4">
                  <div class="flex items-center space-x-2">
                    <Button size="sm" variant="ghost">
                      <Eye class="h-4 w-4" />
                    </Button>
                    <Button size="sm" variant="ghost" v-if="adjustment.status === 'pending'">
                      <Check class="h-4 w-4" />
                    </Button>
                    <Button size="sm" variant="ghost" v-if="adjustment.status === 'pending'">
                      <X class="h-4 w-4" />
                    </Button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Stock Count Modal (placeholder) -->
      <div v-if="showStockCountModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg max-w-md w-full m-4">
          <h3 class="text-lg font-semibold mb-4">Quick Stock Count</h3>
          <p class="text-muted-foreground mb-4">Scan or enter product barcode to adjust stock levels</p>
          <div class="space-y-4">
            <input
              type="text"
              placeholder="Scan or enter barcode..."
              class="w-full px-3 py-2 border rounded-md"
            />
            <div class="flex justify-end space-x-2">
              <Button variant="outline" @click="showStockCountModal = false">Cancel</Button>
              <Button>Start Count</Button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </AppLayout>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { 
  Plus, 
  Download,
  Search, 
  Eye,
  Check,
  X,
  Package,
  TrendingUp,
  TrendingDown,
  RotateCcw,
  AlertTriangle,
  AlertCircle,
  FileText,
  Minus
} from 'lucide-vue-next'

// Reactive data
const searchQuery = ref('')
const typeFilter = ref('')
const statusFilter = ref('')
const dateFilter = ref('')
const showStockCountModal = ref(false)

// Stats
const stockAddedToday = ref(147)
const stockRemovedToday = ref(23)
const pendingApprovals = ref(5)
const totalAdjustments = ref(89)

// Mock adjustments data
const adjustments = ref([
  {
    id: 'ADJ-2024-001',
    productName: 'White Bread 700g',
    productCode: 'WB001',
    type: 'stock-in',
    quantity: 50,
    unit: 'loaves',
    reason: 'New delivery from supplier',
    date: '2024-01-15 09:30',
    user: 'Nomsa Mbeki',
    status: 'approved'
  },
  {
    id: 'ADJ-2024-002',
    productName: 'Maize Meal 2.5kg',
    productCode: 'MM001',
    type: 'stock-out',
    quantity: -15,
    unit: 'bags',
    reason: 'Damaged packaging',
    date: '2024-01-15 14:20',
    user: 'Thabo Mthembu',
    status: 'pending'
  },
  {
    id: 'ADJ-2024-003',
    productName: 'Cooking Oil 750ml',
    productCode: 'CO001',
    type: 'stock-count',
    quantity: -3,
    unit: 'bottles',
    reason: 'Stock count discrepancy',
    date: '2024-01-14 16:45',
    user: 'Zanele Dlamini',
    status: 'approved'
  },
  {
    id: 'ADJ-2024-004',
    productName: 'Sugar 2kg',
    productCode: 'SG001',
    type: 'damage',
    quantity: -8,
    unit: 'bags',
    reason: 'Water damage from leak',
    date: '2024-01-14 11:15',
    user: 'Sipho Ndaba',
    status: 'approved'
  },
  {
    id: 'ADJ-2024-005',
    productName: 'Coca Cola 2L',
    productCode: 'CC001',
    type: 'stock-in',
    quantity: 24,
    unit: 'bottles',
    reason: 'Emergency restock',
    date: '2024-01-13 08:00',
    user: 'Lerato Molefe',
    status: 'approved'
  },
  {
    id: 'ADJ-2024-006',
    productName: 'Washing Powder 1kg',
    productCode: 'WP001',
    type: 'expired',
    quantity: -5,
    unit: 'boxes',
    reason: 'Expired products removed',
    date: '2024-01-13 17:30',
    user: 'Nomsa Mbeki',
    status: 'pending'
  },
  {
    id: 'ADJ-2024-007',
    productName: 'Toothpaste 100ml',
    productCode: 'TP001',
    type: 'stock-count',
    quantity: 2,
    unit: 'tubes',
    reason: 'Found additional stock in storage',
    date: '2024-01-12 13:20',
    user: 'Thabo Mthembu',
    status: 'approved'
  }
])

// Computed properties
const filteredAdjustments = computed(() => {
  return adjustments.value.filter(adjustment => {
    const matchesSearch = adjustment.productName.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
                         adjustment.id.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
                         adjustment.productCode.toLowerCase().includes(searchQuery.value.toLowerCase())
    const matchesType = !typeFilter.value || adjustment.type === typeFilter.value
    const matchesStatus = !statusFilter.value || adjustment.status === statusFilter.value
    const matchesDate = !dateFilter.value || adjustment.date.startsWith(dateFilter.value)
    
    return matchesSearch && matchesType && matchesStatus && matchesDate
  })
})

// Methods
const getTypeColor = (type: string) => {
  const colors = {
    'stock-in': 'bg-green-100 text-green-800',
    'stock-out': 'bg-red-100 text-red-800',
    'stock-count': 'bg-blue-100 text-blue-800',
    'damage': 'bg-yellow-100 text-yellow-800',
    'expired': 'bg-orange-100 text-orange-800'
  }
  return colors[type as keyof typeof colors] || 'bg-gray-100 text-gray-800'
}

const getStatusColor = (status: string) => {
  const colors = {
    'pending': 'bg-yellow-100 text-yellow-800',
    'approved': 'bg-green-100 text-green-800',
    'rejected': 'bg-red-100 text-red-800'
  }
  return colors[status as keyof typeof colors] || 'bg-gray-100 text-gray-800'
}
</script>