<template>
  <AppLayout>
    <div class="p-6 space-y-6">
      <!-- Header -->
      <div class="flex justify-between items-center">
        <div>
          <h1 class="text-2xl font-bold">Goods Receiving</h1>
          <p class="text-muted-foreground">Track and process incoming deliveries and receipts</p>
        </div>
        <div class="flex items-center space-x-4">
          <Button variant="outline">
            <Scanner class="h-4 w-4 mr-2" />
            Scan Delivery
          </Button>
          <Button>
            <Plus class="h-4 w-4 mr-2" />
            Record Receipt
          </Button>
        </div>
      </div>

      <!-- Receiving Stats -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-blue-100 dark:bg-blue-900 rounded-lg">
              <Package class="h-6 w-6 text-blue-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Expected Today</p>
              <p class="text-2xl font-bold">{{ expectedToday }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-green-100 dark:bg-green-900 rounded-lg">
              <CheckCircle class="h-6 w-6 text-green-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Received Today</p>
              <p class="text-2xl font-bold">{{ receivedToday }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-yellow-100 dark:bg-yellow-900 rounded-lg">
              <Clock class="h-6 w-6 text-yellow-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Pending Inspection</p>
              <p class="text-2xl font-bold">{{ pendingInspection }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-red-100 dark:bg-red-900 rounded-lg">
              <AlertTriangle class="h-6 w-6 text-red-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Discrepancies</p>
              <p class="text-2xl font-bold">{{ discrepancies }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Expected Deliveries Today -->
      <div class="bg-white dark:bg-gray-800 rounded-lg border">
        <div class="p-6 border-b">
          <div class="flex justify-between items-center">
            <h3 class="text-lg font-semibold">Expected Deliveries Today</h3>
            <Button variant="outline" size="sm">
              <RefreshCw class="h-4 w-4 mr-2" />
              Refresh
            </Button>
          </div>
        </div>
        <div class="p-6">
          <div class="space-y-4">
            <div v-for="delivery in expectedDeliveries" :key="delivery.id" class="flex items-center justify-between p-4 border rounded-lg hover:bg-muted/50">
              <div class="flex items-center space-x-4">
                <div class="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center">
                  <Truck class="h-6 w-6 text-primary" />
                </div>
                <div>
                  <p class="font-medium">PO #{{ delivery.poNumber }}</p>
                  <p class="text-sm text-muted-foreground">{{ delivery.supplier }}</p>
                  <div class="flex items-center space-x-4 mt-1">
                    <span class="text-xs bg-blue-100 text-blue-800 px-2 py-1 rounded">{{ delivery.items }} items</span>
                    <span class="text-xs text-muted-foreground flex items-center">
                      <Clock class="h-3 w-3 mr-1" />
                      {{ delivery.expectedTime }}
                    </span>
                  </div>
                </div>
              </div>
              <div class="flex items-center space-x-2">
                <Button size="sm" variant="outline">
                  <Eye class="h-4 w-4" />
                </Button>
                <Button size="sm">
                  <Package class="h-4 w-4 mr-2" />
                  Receive
                </Button>
              </div>
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
              placeholder="Search receipts..."
              class="pl-10 pr-4 py-2 w-full rounded-md border border-input bg-background"
            />
          </div>
          <select v-model="statusFilter" class="px-3 py-2 border rounded-md">
            <option value="">All Status</option>
            <option value="received">Received</option>
            <option value="partial">Partial</option>
            <option value="pending">Pending</option>
            <option value="inspecting">Inspecting</option>
            <option value="completed">Completed</option>
          </select>
          <select v-model="supplierFilter" class="px-3 py-2 border rounded-md">
            <option value="">All Suppliers</option>
            <option value="fresh-produce">Fresh Produce Wholesalers</option>
            <option value="metro-groceries">Metro Groceries Supply</option>
            <option value="beverage-dist">Beverage Distributors SA</option>
          </select>
          <input
            v-model="dateFilter"
            type="date"
            class="px-3 py-2 border rounded-md"
          />
        </div>
      </div>

      <!-- Receiving History -->
      <div class="bg-white dark:bg-gray-800 rounded-lg border">
        <div class="p-6 border-b">
          <h3 class="text-lg font-semibold">Receiving History</h3>
        </div>
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead>
              <tr class="border-b">
                <th class="text-left p-4 font-medium">Receipt #</th>
                <th class="text-left p-4 font-medium">PO Number</th>
                <th class="text-left p-4 font-medium">Supplier</th>
                <th class="text-left p-4 font-medium">Items</th>
                <th class="text-left p-4 font-medium">Received Date</th>
                <th class="text-left p-4 font-medium">Status</th>
                <th class="text-left p-4 font-medium">Inspector</th>
                <th class="text-left p-4 font-medium">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="receipt in filteredReceipts" :key="receipt.id" class="border-b hover:bg-muted/50">
                <td class="p-4 font-mono text-sm">{{ receipt.receiptNumber }}</td>
                <td class="p-4 font-mono text-sm">{{ receipt.poNumber }}</td>
                <td class="p-4">{{ receipt.supplier }}</td>
                <td class="p-4">
                  <div>
                    <p class="font-medium">{{ receipt.itemsReceived }}/{{ receipt.itemsExpected }} items</p>
                    <div class="w-full bg-gray-200 rounded-full h-2 mt-1">
                      <div 
                        class="h-2 rounded-full transition-all"
                        :class="getProgressColor(receipt.itemsReceived / receipt.itemsExpected * 100)"
                        :style="{ width: (receipt.itemsReceived / receipt.itemsExpected * 100) + '%' }"
                      ></div>
                    </div>
                  </div>
                </td>
                <td class="p-4 text-muted-foreground">{{ receipt.receivedDate }}</td>
                <td class="p-4">
                  <span class="px-2 py-1 rounded text-xs font-medium" :class="getStatusColor(receipt.status)">
                    {{ receipt.status }}
                  </span>
                </td>
                <td class="p-4">{{ receipt.inspector }}</td>
                <td class="p-4">
                  <div class="flex items-center space-x-2">
                    <Button size="sm" variant="ghost">
                      <Eye class="h-4 w-4" />
                    </Button>
                    <Button size="sm" variant="ghost" v-if="receipt.status !== 'completed'">
                      <Edit class="h-4 w-4" />
                    </Button>
                    <Button size="sm" variant="ghost">
                      <FileText class="h-4 w-4" />
                    </Button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Quality Control Alerts -->
      <div v-if="qualityAlerts.length > 0" class="bg-white dark:bg-gray-800 rounded-lg border border-yellow-200 dark:border-yellow-800">
        <div class="p-6 border-b border-yellow-200 dark:border-yellow-800">
          <div class="flex items-center space-x-2">
            <AlertTriangle class="h-5 w-5 text-yellow-600" />
            <h3 class="text-lg font-semibold text-yellow-800 dark:text-yellow-200">Quality Control Alerts</h3>
          </div>
        </div>
        <div class="p-6">
          <div class="space-y-3">
            <div v-for="alert in qualityAlerts" :key="alert.id" class="flex items-center justify-between p-3 bg-yellow-50 dark:bg-yellow-900/20 rounded-lg">
              <div>
                <p class="font-medium text-yellow-800 dark:text-yellow-200">{{ alert.message }}</p>
                <p class="text-sm text-yellow-600 dark:text-yellow-400">Receipt #{{ alert.receiptNumber }} - {{ alert.product }}</p>
              </div>
              <Button size="sm" variant="outline">
                Investigate
              </Button>
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
  Scanner,
  Search, 
  Eye,
  Edit,
  FileText,
  Package,
  CheckCircle,
  Clock,
  AlertTriangle,
  Truck,
  RefreshCw
} from 'lucide-vue-next'

// Reactive data
const searchQuery = ref('')
const statusFilter = ref('')
const supplierFilter = ref('')
const dateFilter = ref('')

// Stats
const expectedToday = ref(8)
const receivedToday = ref(5)
const pendingInspection = ref(3)
const discrepancies = ref(2)

// Expected deliveries today
const expectedDeliveries = ref([
  {
    id: 1,
    poNumber: '2024-001',
    supplier: 'Fresh Produce Wholesalers',
    items: 25,
    expectedTime: '10:00 AM'
  },
  {
    id: 2,
    poNumber: '2024-002',
    supplier: 'Metro Groceries Supply',
    items: 18,
    expectedTime: '2:00 PM'
  },
  {
    id: 3,
    poNumber: '2024-003',
    supplier: 'Beverage Distributors SA',
    items: 32,
    expectedTime: '4:30 PM'
  }
])

// Mock receipts data
const receipts = ref([
  {
    id: 1,
    receiptNumber: 'REC-2024-001',
    poNumber: 'PO-2024-001',
    supplier: 'Fresh Produce Wholesalers',
    itemsExpected: 25,
    itemsReceived: 25,
    receivedDate: '2024-01-15 09:30',
    status: 'completed',
    inspector: 'Nomsa Mbeki'
  },
  {
    id: 2,
    receiptNumber: 'REC-2024-002',
    poNumber: 'PO-2024-002',
    supplier: 'Metro Groceries Supply',
    itemsExpected: 18,
    itemsReceived: 16,
    receivedDate: '2024-01-15 14:20',
    status: 'partial',
    inspector: 'Thabo Mthembu'
  },
  {
    id: 3,
    receiptNumber: 'REC-2024-003',
    poNumber: 'PO-2024-003',
    supplier: 'Beverage Distributors SA',
    itemsExpected: 32,
    itemsReceived: 32,
    receivedDate: '2024-01-14 16:45',
    status: 'inspecting',
    inspector: 'Zanele Dlamini'
  },
  {
    id: 4,
    receiptNumber: 'REC-2024-004',
    poNumber: 'PO-2024-004',
    supplier: 'Household Essentials Ltd',
    itemsExpected: 12,
    itemsReceived: 12,
    receivedDate: '2024-01-14 11:15',
    status: 'completed',
    inspector: 'Sipho Ndaba'
  },
  {
    id: 5,
    receiptNumber: 'REC-2024-005',
    poNumber: 'PO-2024-005',
    supplier: 'Township Supply Co',
    itemsExpected: 20,
    itemsReceived: 18,
    receivedDate: '2024-01-13 08:00',
    status: 'partial',
    inspector: 'Lerato Molefe'
  },
  {
    id: 6,
    receiptNumber: 'REC-2024-006',
    poNumber: 'PO-2024-006',
    supplier: 'Fresh Produce Wholesalers',
    itemsExpected: 15,
    itemsReceived: 0,
    receivedDate: '2024-01-13 00:00',
    status: 'pending',
    inspector: ''
  }
])

// Quality alerts
const qualityAlerts = ref([
  {
    id: 1,
    receiptNumber: 'REC-2024-002',
    product: 'White Bread 700g',
    message: 'Short expiry date detected - expires in 2 days'
  },
  {
    id: 2,
    receiptNumber: 'REC-2024-005',
    product: 'Cooking Oil 750ml',
    message: 'Damaged packaging on 2 items'
  }
])

// Computed properties
const filteredReceipts = computed(() => {
  return receipts.value.filter(receipt => {
    const matchesSearch = receipt.receiptNumber.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
                         receipt.poNumber.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
                         receipt.supplier.toLowerCase().includes(searchQuery.value.toLowerCase())
    const matchesStatus = !statusFilter.value || receipt.status === statusFilter.value
    const matchesSupplier = !supplierFilter.value || receipt.supplier.toLowerCase().includes(supplierFilter.value)
    const matchesDate = !dateFilter.value || receipt.receivedDate.startsWith(dateFilter.value)
    
    return matchesSearch && matchesStatus && matchesSupplier && matchesDate
  })
})

// Methods
const getStatusColor = (status: string) => {
  const colors = {
    'completed': 'bg-green-100 text-green-800',
    'partial': 'bg-yellow-100 text-yellow-800',
    'pending': 'bg-blue-100 text-blue-800',
    'inspecting': 'bg-purple-100 text-purple-800',
    'received': 'bg-green-100 text-green-800'
  }
  return colors[status as keyof typeof colors] || 'bg-gray-100 text-gray-800'
}

const getProgressColor = (percentage: number) => {
  if (percentage === 100) return 'bg-green-500'
  if (percentage >= 80) return 'bg-yellow-500'
  return 'bg-red-500'
}
</script>