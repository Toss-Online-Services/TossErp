<template>
  <div class="container mx-auto p-6 max-w-7xl">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-6">
      <div>
        <h1 class="text-3xl font-bold">{{ $t('inventory.title', 'Inventory Management') }}</h1>
        <p class="text-muted-foreground mt-1">
          {{ $t('inventory.subtitle', 'Track stock levels and manage products') }}
        </p>
      </div>
      <div class="flex gap-2 mt-4 sm:mt-0">
        <Button variant="outline" @click="generateReorderReport">
          <FileText class="mr-2 h-4 w-4" />
          {{ $t('inventory.reorderReport') }}
        </Button>
        <Button variant="outline" @click="runStockCount">
          <ScanLine class="mr-2 h-4 w-4" />
          {{ $t('inventory.stockCount') }}
        </Button>
        <Button @click="showAddProductDialog = true">
          <Plus class="mr-2 h-4 w-4" />
          {{ $t('inventory.addProduct') }}
        </Button>
      </div>
    </div>

    <!-- Overview Cards -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-6">
      <Card>
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Total Products</p>
              <h3 class="text-2xl font-bold">{{ inventoryItems.length }}</h3>
            </div>
            <Package class="h-8 w-8 text-blue-600" />
          </div>
        </CardContent>
      </Card>
      <Card>
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Low Stock Items</p>
              <h3 class="text-2xl font-bold text-orange-600">{{ lowStockItems.length }}</h3>
            </div>
            <AlertTriangle class="h-8 w-8 text-orange-600" />
          </div>
        </CardContent>
      </Card>
      <Card>
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Total Value</p>
              <h3 class="text-2xl font-bold">{{ formatCurrency(totalInventoryValue) }}</h3>
            </div>
            <TrendingUp class="h-8 w-8 text-green-600" />
          </div>
        </CardContent>
      </Card>
      <Card>
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-muted-foreground">Expiring Soon</p>
              <h3 class="text-2xl font-bold text-red-600">{{ expiringSoonItems.length }}</h3>
            </div>
            <Clock class="h-8 w-8 text-red-600" />
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- AI Insights Alert -->
    <Card v-if="aiRecommendations.length > 0" class="mb-6 border-yellow-200 bg-yellow-50/50">
      <CardContent class="p-6">
        <div class="flex items-start gap-3">
          <Sparkles class="h-5 w-5 text-yellow-600 mt-1" />
          <div class="flex-1">
            <h3 class="font-medium text-yellow-800 mb-2">AI Inventory Insights</h3>
            <div class="space-y-2">
              <div v-for="recommendation in aiRecommendations" :key="recommendation.id" class="text-sm text-yellow-700">
                <strong>{{ recommendation.type }}:</strong> {{ recommendation.message }}
              </div>
            </div>
            <Button size="sm" variant="outline" class="mt-3" @click="viewAIInsights">
              View All AI Insights
            </Button>
          </div>
        </div>
      </CardContent>
    </Card>

    <!-- Filters and Search -->
    <Card class="mb-6">
      <CardContent class="p-6">
        <div class="grid grid-cols-1 md:grid-cols-5 gap-4">
          <div class="relative">
            <Search class="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
            <input
              v-model="searchTerm"
              type="text"
              :placeholder="$t('common.search')"
              class="pl-10 w-full h-10 rounded-md border border-input bg-background px-3 py-2 text-sm"
            />
          </div>
          <select v-model="selectedCategory" class="h-10 rounded-md border border-input bg-background px-3 py-2 text-sm">
            <option value="">All Categories</option>
            <option v-for="category in categories" :key="category" :value="category">
              {{ category }}
            </option>
          </select>
          <select v-model="stockFilter" class="h-10 rounded-md border border-input bg-background px-3 py-2 text-sm">
            <option value="">All Stock Levels</option>
            <option value="in-stock">In Stock</option>
            <option value="low-stock">Low Stock</option>
            <option value="out-of-stock">Out of Stock</option>
            <option value="expiring">Expiring Soon</option>
          </select>
          <select v-model="supplierFilter" class="h-10 rounded-md border border-input bg-background px-3 py-2 text-sm">
            <option value="">All Suppliers</option>
            <option v-for="supplier in suppliers" :key="supplier" :value="supplier">
              {{ supplier }}
            </option>
          </select>
          <Button variant="outline" @click="resetFilters" class="h-10">
            <X class="mr-2 h-4 w-4" />
            {{ $t('common.reset') }}
          </Button>
        </div>
      </CardContent>
    </Card>

    <!-- Inventory Table -->
    <Card>
      <CardContent class="p-0">
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead>
              <tr class="border-b">
                <th class="text-left p-4 font-medium">Product</th>
                <th class="text-left p-4 font-medium">Category</th>
                <th class="text-left p-4 font-medium">Stock</th>
                <th class="text-left p-4 font-medium">Unit Price</th>
                <th class="text-left p-4 font-medium">Value</th>
                <th class="text-left p-4 font-medium">Supplier</th>
                <th class="text-left p-4 font-medium">Expiry</th>
                <th class="text-left p-4 font-medium">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr 
                v-for="item in filteredItems" 
                :key="item.id"
                class="border-b hover:bg-muted/50"
                :class="getRowClass(item)"
              >
                <td class="p-4">
                  <div class="flex items-center gap-3">
                    <div class="w-10 h-10 rounded-lg bg-muted flex items-center justify-center">
                      <Package class="h-5 w-5" />
                    </div>
                    <div>
                      <p class="font-medium">{{ item.name }}</p>
                      <p class="text-sm text-muted-foreground">{{ item.sku }}</p>
                    </div>
                  </div>
                </td>
                <td class="p-4">
                  <Badge variant="secondary">{{ item.category }}</Badge>
                </td>
                <td class="p-4">
                  <div class="flex items-center gap-2">
                    <span class="font-medium">{{ item.currentStock }}</span>
                    <span class="text-sm text-muted-foreground">/ {{ item.reorderPoint }}</span>
                    <div v-if="isLowStock(item)" class="w-2 h-2 bg-orange-500 rounded-full"></div>
                    <div v-if="isOutOfStock(item)" class="w-2 h-2 bg-red-500 rounded-full"></div>
                  </div>
                </td>
                <td class="p-4">{{ formatCurrency(item.unitPrice) }}</td>
                <td class="p-4 font-medium">{{ formatCurrency(item.currentStock * item.unitPrice) }}</td>
                <td class="p-4 text-sm">{{ item.supplier }}</td>
                <td class="p-4">
                  <span v-if="item.expiryDate" class="text-sm" :class="getExpiryClass(item.expiryDate)">
                    {{ formatDate(item.expiryDate) }}
                  </span>
                  <span v-else class="text-sm text-muted-foreground">N/A</span>
                </td>
                <td class="p-4">
                  <div class="flex gap-1">
                    <Button size="sm" variant="ghost" @click="adjustStock(item)">
                      <Edit class="h-4 w-4" />
                    </Button>
                    <Button size="sm" variant="ghost" @click="reorderItem(item)">
                      <ShoppingCart class="h-4 w-4" />
                    </Button>
                    <Button size="sm" variant="ghost" @click="viewItemHistory(item)">
                      <History class="h-4 w-4" />
                    </Button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </CardContent>
    </Card>

    <!-- Quick Actions Bar -->
    <div class="fixed bottom-6 right-6 flex gap-2">
      <Button 
        v-if="lowStockItems.length > 0" 
        @click="createBulkReorder"
        class="shadow-lg"
        variant="destructive"
      >
        <AlertTriangle class="mr-2 h-4 w-4" />
        Reorder {{ lowStockItems.length }} Items
      </Button>
      <Button @click="scanBarcode" class="shadow-lg" variant="secondary">
        <ScanLine class="mr-2 h-4 w-4" />
        Scan
      </Button>
    </div>

    <!-- Stock Adjustment Dialog -->
    <StockAdjustmentDialog
      v-model="showStockDialog"
      :item="selectedItem"
      @save="handleStockAdjustment"
    />

    <!-- Add Product Dialog -->
    <AddProductDialog
      v-model="showAddProductDialog"
      @save="handleAddProduct"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { 
  Plus, 
  Search, 
  X,
  Package,
  AlertTriangle,
  TrendingUp,
  Clock,
  Sparkles,
  Edit,
  ShoppingCart,
  History,
  ScanLine,
  FileText
} from 'lucide-vue-next'

import { Button } from '../../components/ui/button'
import { Card, CardContent } from '../../components/ui/card'
import { Badge } from '../../components/ui/badge'

// Types
interface InventoryItem {
  id: string
  name: string
  sku: string
  category: string
  currentStock: number
  reorderPoint: number
  maxStock: number
  unitPrice: number
  costPrice: number
  supplier: string
  expiryDate?: Date
  lastUpdated: Date
}

interface AIRecommendation {
  id: string
  type: 'Reorder Alert' | 'Price Optimization' | 'Demand Forecast'
  message: string
}

// State
const { t } = useI18n()
const searchTerm = ref('')
const selectedCategory = ref('')
const stockFilter = ref('')
const supplierFilter = ref('')
const showStockDialog = ref(false)
const showAddProductDialog = ref(false)
const selectedItem = ref<InventoryItem | null>(null)

// Mock data
const inventoryItems = ref<InventoryItem[]>([
  {
    id: '1',
    name: 'White Bread Loaf',
    sku: 'BRD-001',
    category: 'Bakery',
    currentStock: 8,
    reorderPoint: 15,
    maxStock: 50,
    unitPrice: 12.99,
    costPrice: 8.50,
    supplier: 'Albany Bakeries',
    expiryDate: new Date(2024, 1, 20),
    lastUpdated: new Date()
  },
  {
    id: '2',
    name: 'Cooking Oil (750ml)',
    sku: 'OIL-001',
    category: 'Cooking',
    currentStock: 12,
    reorderPoint: 20,
    maxStock: 100,
    unitPrice: 34.99,
    costPrice: 28.00,
    supplier: 'Sunfoil',
    lastUpdated: new Date()
  },
  {
    id: '3',
    name: 'Maize Meal (5kg)',
    sku: 'MAZ-001',
    category: 'Staples',
    currentStock: 25,
    reorderPoint: 15,
    maxStock: 80,
    unitPrice: 45.99,
    costPrice: 38.50,
    supplier: 'White Star',
    lastUpdated: new Date()
  },
  {
    id: '4',
    name: 'Milk (1L)',
    sku: 'MLK-001',
    category: 'Dairy',
    currentStock: 3,
    reorderPoint: 10,
    maxStock: 30,
    unitPrice: 18.99,
    costPrice: 15.00,
    supplier: 'Clover',
    expiryDate: new Date(2024, 1, 18),
    lastUpdated: new Date()
  },
  {
    id: '5',
    name: 'Sugar (2.5kg)',
    sku: 'SUG-001',
    category: 'Staples',
    currentStock: 18,
    reorderPoint: 12,
    maxStock: 60,
    unitPrice: 39.99,
    costPrice: 32.00,
    supplier: 'Huletts',
    lastUpdated: new Date()
  }
])

const aiRecommendations = ref<AIRecommendation[]>([
  {
    id: '1',
    type: 'Reorder Alert',
    message: 'White bread and milk are running low. Consider group buying with nearby shops for 12% savings.'
  },
  {
    id: '2',
    type: 'Demand Forecast',
    message: 'Cooking oil demand typically increases by 25% in the next 2 weeks. Stock up now.'
  },
  {
    id: '3',
    type: 'Price Optimization',
    message: 'Maize meal price can be increased by R3 based on local market analysis.'
  }
])

// Computed
const categories = computed(() => {
  const cats = inventoryItems.value.map(item => item.category)
  return [...new Set(cats)].sort()
})

const suppliers = computed(() => {
  const sups = inventoryItems.value.map(item => item.supplier)
  return [...new Set(sups)].sort()
})

const lowStockItems = computed(() => {
  return inventoryItems.value.filter(item => item.currentStock <= item.reorderPoint)
})

const expiringSoonItems = computed(() => {
  const nextWeek = new Date()
  nextWeek.setDate(nextWeek.getDate() + 7)
  return inventoryItems.value.filter(item => 
    item.expiryDate && item.expiryDate <= nextWeek
  )
})

const totalInventoryValue = computed(() => {
  return inventoryItems.value.reduce((total, item) => 
    total + (item.currentStock * item.unitPrice), 0
  )
})

const filteredItems = computed(() => {
  return inventoryItems.value.filter(item => {
    const matchesSearch = !searchTerm.value || 
      item.name.toLowerCase().includes(searchTerm.value.toLowerCase()) ||
      item.sku.toLowerCase().includes(searchTerm.value.toLowerCase())
    
    const matchesCategory = !selectedCategory.value || item.category === selectedCategory.value
    const matchesSupplier = !supplierFilter.value || item.supplier === supplierFilter.value
    
    let matchesStock = true
    if (stockFilter.value === 'low-stock') {
      matchesStock = item.currentStock <= item.reorderPoint
    } else if (stockFilter.value === 'out-of-stock') {
      matchesStock = item.currentStock === 0
    } else if (stockFilter.value === 'in-stock') {
      matchesStock = item.currentStock > item.reorderPoint
    } else if (stockFilter.value === 'expiring') {
      const nextWeek = new Date()
      nextWeek.setDate(nextWeek.getDate() + 7)
      matchesStock = item.expiryDate && item.expiryDate <= nextWeek || false
    }
    
    return matchesSearch && matchesCategory && matchesSupplier && matchesStock
  })
})

// Methods
const isLowStock = (item: InventoryItem) => {
  return item.currentStock <= item.reorderPoint && item.currentStock > 0
}

const isOutOfStock = (item: InventoryItem) => {
  return item.currentStock === 0
}

const getRowClass = (item: InventoryItem) => {
  if (isOutOfStock(item)) return 'bg-red-50 dark:bg-red-900/20'
  if (isLowStock(item)) return 'bg-orange-50 dark:bg-orange-900/20'
  return ''
}

const getExpiryClass = (expiryDate: Date) => {
  const today = new Date()
  const daysUntilExpiry = Math.ceil((expiryDate.getTime() - today.getTime()) / (1000 * 60 * 60 * 24))
  
  if (daysUntilExpiry <= 3) return 'text-red-600 font-medium'
  if (daysUntilExpiry <= 7) return 'text-orange-600'
  return 'text-muted-foreground'
}

const adjustStock = (item: InventoryItem) => {
  selectedItem.value = item
  showStockDialog.value = true
}

const reorderItem = (item: InventoryItem) => {
  console.log('Reordering item:', item.name)
  // Navigate to purchase order creation
}

const viewItemHistory = (item: InventoryItem) => {
  console.log('Viewing history for:', item.name)
  // Show item history modal
}

const createBulkReorder = () => {
  console.log('Creating bulk reorder for', lowStockItems.value.length, 'items')
  // Create bulk purchase order
}

const scanBarcode = () => {
  console.log('Opening barcode scanner')
  // Open barcode scanner
}

const generateReorderReport = () => {
  console.log('Generating reorder report')
  // Generate and download report
}

const runStockCount = () => {
  console.log('Starting stock count')
  // Start stock count process
}

const viewAIInsights = () => {
  // Navigate to AI insights page
  console.log('Viewing AI insights')
}

const resetFilters = () => {
  searchTerm.value = ''
  selectedCategory.value = ''
  stockFilter.value = ''
  supplierFilter.value = ''
}

const handleStockAdjustment = (adjustment: any) => {
  console.log('Stock adjustment:', adjustment)
  // Update stock levels
  showStockDialog.value = false
}

const handleAddProduct = (product: any) => {
  console.log('Adding product:', product)
  // Add new product
  showAddProductDialog.value = false
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
    minimumFractionDigits: 2
  }).format(amount)
}

const formatDate = (date: Date) => {
  return new Intl.DateTimeFormat('en-ZA', {
    day: 'numeric',
    month: 'short'
  }).format(date)
}

onMounted(() => {
  console.log('Inventory Management loaded')
})
</script>