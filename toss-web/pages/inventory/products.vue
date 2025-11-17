<template>
  <AppLayout>
    <div class="p-6 space-y-6">
      <!-- Header -->
      <div class="flex justify-between items-center">
        <div>
          <h1 class="text-2xl font-bold">Product Catalog</h1>
          <p class="text-muted-foreground">Manage your product inventory and pricing</p>
        </div>
        <div class="flex items-center space-x-4">
          <Button variant="outline">
            <Upload class="h-4 w-4 mr-2" />
            Import Products
          </Button>
          <Button>
            <Plus class="h-4 w-4 mr-2" />
            Add Product
          </Button>
        </div>
      </div>

      <!-- Quick Stats -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-blue-100 dark:bg-blue-900 rounded-lg">
              <Package class="h-6 w-6 text-blue-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Total Products</p>
              <p class="text-2xl font-bold">{{ totalProducts }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-yellow-100 dark:bg-yellow-900 rounded-lg">
              <AlertTriangle class="h-6 w-6 text-yellow-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Low Stock Items</p>
              <p class="text-2xl font-bold">{{ lowStockItems }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-red-100 dark:bg-red-900 rounded-lg">
              <XCircle class="h-6 w-6 text-red-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Out of Stock</p>
              <p class="text-2xl font-bold">{{ outOfStockItems }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-green-100 dark:bg-green-900 rounded-lg">
              <DollarSign class="h-6 w-6 text-green-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Inventory Value</p>
              <p class="text-2xl font-bold">R {{ inventoryValue.toLocaleString() }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Filters and Search -->
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg border">
        <div class="flex flex-col md:flex-row md:items-center space-y-4 md:space-y-0 md:space-x-4">
          <div class="relative flex-1">
            <Search class="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
            <input
              v-model="searchQuery"
              type="text"
              placeholder="Search products..."
              class="pl-10 pr-4 py-2 w-full rounded-md border border-input bg-background"
            />
          </div>
          <select v-model="categoryFilter" class="px-3 py-2 border rounded-md">
            <option value="">All Categories</option>
            <option value="groceries">Groceries</option>
            <option value="beverages">Beverages</option>
            <option value="household">Household Items</option>
            <option value="personal-care">Personal Care</option>
            <option value="snacks">Snacks & Confectionery</option>
          </select>
          <select v-model="statusFilter" class="px-3 py-2 border rounded-md">
            <option value="">All Stock Status</option>
            <option value="in-stock">In Stock</option>
            <option value="low-stock">Low Stock</option>
            <option value="out-of-stock">Out of Stock</option>
          </select>
        </div>
      </div>

      <!-- Products Grid/Table -->
      <div class="bg-white dark:bg-gray-800 rounded-lg border">
        <div class="p-4 border-b flex justify-between items-center">
          <h3 class="text-lg font-semibold">Product Inventory</h3>
          <div class="flex items-center space-x-2">
            <Button 
              variant="ghost" 
              size="sm" 
              :class="{ 'bg-muted': viewMode === 'grid' }"
              @click="viewMode = 'grid'"
            >
              <Grid class="h-4 w-4" />
            </Button>
            <Button 
              variant="ghost" 
              size="sm" 
              :class="{ 'bg-muted': viewMode === 'table' }"
              @click="viewMode = 'table'"
            >
              <List class="h-4 w-4" />
            </Button>
          </div>
        </div>

        <!-- Grid View -->
        <div v-if="viewMode === 'grid'" class="p-6">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
            <div v-for="product in filteredProducts" :key="product.id" class="border rounded-lg p-4 hover:shadow-md transition-shadow">
              <div class="aspect-square bg-gray-100 dark:bg-gray-700 rounded-lg mb-4 flex items-center justify-center">
                <Package class="h-12 w-12 text-gray-400" />
              </div>
              <div class="space-y-2">
                <h4 class="font-medium text-sm">{{ product.name }}</h4>
                <p class="text-xs text-muted-foreground">{{ product.category }}</p>
                <div class="flex justify-between items-center">
                  <span class="font-bold text-lg">R {{ product.price.toFixed(2) }}</span>
                  <span class="text-sm" :class="getStockColor(product.stock, product.minStock)">
                    {{ product.stock }} in stock
                  </span>
                </div>
                <div class="flex justify-between items-center text-xs text-muted-foreground">
                  <span>Cost: R {{ product.cost.toFixed(2) }}</span>
                  <span>Barcode: {{ product.barcode }}</span>
                </div>
                <div class="flex space-x-2 mt-3">
                  <Button size="sm" variant="outline" class="flex-1">
                    <Edit class="h-4 w-4" />
                  </Button>
                  <Button size="sm" variant="outline" class="flex-1">
                    <Eye class="h-4 w-4" />
                  </Button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Table View -->
        <div v-else class="overflow-x-auto">
          <table class="w-full">
            <thead>
              <tr class="border-b">
                <th class="text-left p-4 font-medium">Product</th>
                <th class="text-left p-4 font-medium">Category</th>
                <th class="text-left p-4 font-medium">Barcode</th>
                <th class="text-left p-4 font-medium">Cost Price</th>
                <th class="text-left p-4 font-medium">Sell Price</th>
                <th class="text-left p-4 font-medium">Stock</th>
                <th class="text-left p-4 font-medium">Status</th>
                <th class="text-left p-4 font-medium">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="product in filteredProducts" :key="product.id" class="border-b hover:bg-muted/50">
                <td class="p-4">
                  <div class="flex items-center space-x-3">
                    <div class="w-10 h-10 bg-gray-100 dark:bg-gray-700 rounded-lg flex items-center justify-center">
                      <Package class="h-5 w-5 text-gray-400" />
                    </div>
                    <div>
                      <p class="font-medium">{{ product.name }}</p>
                      <p class="text-sm text-muted-foreground">{{ product.unit }}</p>
                    </div>
                  </div>
                </td>
                <td class="p-4 text-muted-foreground">{{ product.category }}</td>
                <td class="p-4 font-mono text-sm">{{ product.barcode }}</td>
                <td class="p-4">R {{ product.cost.toFixed(2) }}</td>
                <td class="p-4 font-medium">R {{ product.price.toFixed(2) }}</td>
                <td class="p-4">
                  <span :class="getStockColor(product.stock, product.minStock)">
                    {{ product.stock }} {{ product.unit }}
                  </span>
                </td>
                <td class="p-4">
                  <span class="px-2 py-1 rounded text-xs font-medium" :class="getStatusColor(product.stock, product.minStock)">
                    {{ getStockStatus(product.stock, product.minStock) }}
                  </span>
                </td>
                <td class="p-4">
                  <div class="flex items-center space-x-2">
                    <Button size="sm" variant="ghost">
                      <Edit class="h-4 w-4" />
                    </Button>
                    <Button size="sm" variant="ghost">
                      <Eye class="h-4 w-4" />
                    </Button>
                    <Button size="sm" variant="ghost">
                      <Copy class="h-4 w-4" />
                    </Button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </AppLayout>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { 
  Plus, 
  Upload,
  Search, 
  Edit, 
  Eye,
  Copy,
  Package,
  AlertTriangle,
  XCircle,
  DollarSign,
  Grid,
  List
} from 'lucide-vue-next'

// Reactive data
const searchQuery = ref('')
const categoryFilter = ref('')
const statusFilter = ref('')
const viewMode = ref('table')

// Stats
const totalProducts = ref(342)
const lowStockItems = ref(15)
const outOfStockItems = ref(3)
const inventoryValue = ref(125680)

// Mock products data
const products = ref([
  {
    id: 1,
    name: 'White Bread 700g',
    category: 'groceries',
    unit: 'loaf',
    barcode: '6001234567890',
    cost: 8.50,
    price: 12.50,
    stock: 24,
    minStock: 10
  },
  {
    id: 2,
    name: 'Maize Meal 2.5kg',
    category: 'groceries',
    unit: 'bag',
    barcode: '6001234567891',
    cost: 15.00,
    price: 22.00,
    stock: 8,
    minStock: 15
  },
  {
    id: 3,
    name: 'Cooking Oil 750ml',
    category: 'groceries',
    unit: 'bottle',
    barcode: '6001234567892',
    cost: 18.50,
    price: 28.90,
    stock: 45,
    minStock: 20
  },
  {
    id: 4,
    name: 'Sugar 2kg',
    category: 'groceries',
    unit: 'bag',
    barcode: '6001234567893',
    cost: 22.00,
    price: 35.50,
    stock: 0,
    minStock: 10
  },
  {
    id: 5,
    name: 'Coca Cola 2L',
    category: 'beverages',
    unit: 'bottle',
    barcode: '6001234567894',
    cost: 12.00,
    price: 18.50,
    stock: 36,
    minStock: 24
  },
  {
    id: 6,
    name: 'Washing Powder 1kg',
    category: 'household',
    unit: 'box',
    barcode: '6001234567895',
    cost: 25.00,
    price: 38.90,
    stock: 12,
    minStock: 8
  },
  {
    id: 7,
    name: 'Toothpaste 100ml',
    category: 'personal-care',
    unit: 'tube',
    barcode: '6001234567896',
    cost: 8.50,
    price: 15.90,
    stock: 28,
    minStock: 12
  },
  {
    id: 8,
    name: 'Simba Chips 120g',
    category: 'snacks',
    unit: 'packet',
    barcode: '6001234567897',
    cost: 6.50,
    price: 12.50,
    stock: 5,
    minStock: 20
  }
])

// Computed properties
const filteredProducts = computed(() => {
  return products.value.filter(product => {
    const matchesSearch = product.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
                         product.barcode.includes(searchQuery.value)
    const matchesCategory = !categoryFilter.value || product.category === categoryFilter.value
    let matchesStatus = true
    
    if (statusFilter.value) {
      const status = getStockStatus(product.stock, product.minStock)
      matchesStatus = status.toLowerCase().replace(' ', '-') === statusFilter.value
    }
    
    return matchesSearch && matchesCategory && matchesStatus
  })
})

// Methods
const getStockColor = (stock: number, minStock: number) => {
  if (stock === 0) return 'text-red-600'
  if (stock <= minStock) return 'text-yellow-600'
  return 'text-green-600'
}

const getStatusColor = (stock: number, minStock: number) => {
  if (stock === 0) return 'bg-red-100 text-red-800'
  if (stock <= minStock) return 'bg-yellow-100 text-yellow-800'
  return 'bg-green-100 text-green-800'
}

const getStockStatus = (stock: number, minStock: number) => {
  if (stock === 0) return 'Out of Stock'
  if (stock <= minStock) return 'Low Stock'
  return 'In Stock'
}
</script>