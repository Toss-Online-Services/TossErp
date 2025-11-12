<template>
  <AppLayout>
    <div class="p-6 space-y-6">
      <!-- Header -->
      <div class="flex justify-between items-center">
        <div>
          <h1 class="text-2xl font-bold">Suppliers</h1>
          <p class="text-muted-foreground">Manage your supplier relationships and contacts</p>
        </div>
        <div class="flex items-center space-x-4">
          <Button variant="outline">
            <Upload class="h-4 w-4 mr-2" />
            Import Suppliers
          </Button>
          <Button>
            <Plus class="h-4 w-4 mr-2" />
            Add Supplier
          </Button>
        </div>
      </div>

      <!-- Supplier Stats -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-blue-100 dark:bg-blue-900 rounded-lg">
              <Truck class="h-6 w-6 text-blue-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Total Suppliers</p>
              <p class="text-2xl font-bold">{{ totalSuppliers }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-green-100 dark:bg-green-900 rounded-lg">
              <CheckCircle class="h-6 w-6 text-green-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Active Suppliers</p>
              <p class="text-2xl font-bold">{{ activeSuppliers }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-yellow-100 dark:bg-yellow-900 rounded-lg">
              <Clock class="h-6 w-6 text-yellow-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Avg. Lead Time</p>
              <p class="text-2xl font-bold">{{ avgLeadTime }} days</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-purple-100 dark:bg-purple-900 rounded-lg">
              <DollarSign class="h-6 w-6 text-purple-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Monthly Spend</p>
              <p class="text-2xl font-bold">R {{ monthlySpend.toLocaleString() }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
        <h3 class="text-lg font-semibold mb-4">Quick Actions</h3>
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <Button variant="outline" class="h-20 flex-col space-y-2">
            <ShoppingCart class="h-6 w-6 text-blue-600" />
            <span class="text-sm">Create Purchase Order</span>
          </Button>
          <Button variant="outline" class="h-20 flex-col space-y-2">
            <FileText class="h-6 w-6 text-green-600" />
            <span class="text-sm">Request Quote</span>
          </Button>
          <Button variant="outline" class="h-20 flex-col space-y-2">
            <Star class="h-6 w-6 text-yellow-600" />
            <span class="text-sm">Rate Suppliers</span>
          </Button>
          <Button variant="outline" class="h-20 flex-col space-y-2">
            <MessageSquare class="h-6 w-6 text-purple-600" />
            <span class="text-sm">Send Message</span>
          </Button>
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
              placeholder="Search suppliers..."
              class="pl-10 pr-4 py-2 w-full rounded-md border border-input bg-background"
            />
          </div>
          <select v-model="categoryFilter" class="px-3 py-2 border rounded-md">
            <option value="">All Categories</option>
            <option value="groceries">Groceries</option>
            <option value="beverages">Beverages</option>
            <option value="household">Household Items</option>
            <option value="fresh-produce">Fresh Produce</option>
            <option value="packaging">Packaging</option>
          </select>
          <select v-model="statusFilter" class="px-3 py-2 border rounded-md">
            <option value="">All Status</option>
            <option value="active">Active</option>
            <option value="inactive">Inactive</option>
            <option value="pending">Pending</option>
          </select>
          <select v-model="locationFilter" class="px-3 py-2 border rounded-md">
            <option value="">All Locations</option>
            <option value="johannesburg">Johannesburg</option>
            <option value="cape-town">Cape Town</option>
            <option value="durban">Durban</option>
            <option value="pretoria">Pretoria</option>
          </select>
        </div>
      </div>

      <!-- Suppliers Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
        <div v-for="supplier in filteredSuppliers" :key="supplier.id" class="bg-white dark:bg-gray-800 border rounded-lg p-6 hover:shadow-md transition-shadow">
          <div class="flex items-start justify-between mb-4">
            <div class="flex items-center space-x-3">
              <div class="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center">
                <span class="text-primary font-bold text-lg">{{ supplier.name.charAt(0) }}</span>
              </div>
              <div>
                <h3 class="text-lg font-semibold">{{ supplier.name }}</h3>
                <p class="text-sm text-muted-foreground">{{ supplier.category }}</p>
              </div>
            </div>
            <span class="px-2 py-1 rounded text-xs font-medium" :class="getStatusColor(supplier.status)">
              {{ supplier.status }}
            </span>
          </div>

          <div class="space-y-3 mb-4">
            <div class="flex items-center space-x-2 text-sm">
              <MapPin class="h-4 w-4 text-muted-foreground" />
              <span>{{ supplier.location }}</span>
            </div>
            <div class="flex items-center space-x-2 text-sm">
              <Phone class="h-4 w-4 text-muted-foreground" />
              <span>{{ supplier.phone }}</span>
            </div>
            <div class="flex items-center space-x-2 text-sm">
              <Mail class="h-4 w-4 text-muted-foreground" />
              <span>{{ supplier.email }}</span>
            </div>
            <div class="flex items-center space-x-2 text-sm">
              <Clock class="h-4 w-4 text-muted-foreground" />
              <span>{{ supplier.leadTime }} day lead time</span>
            </div>
          </div>

          <!-- Supplier Metrics -->
          <div class="grid grid-cols-2 gap-4 mb-4">
            <div class="text-center">
              <div class="flex items-center justify-center space-x-1">
                <Star class="h-4 w-4 text-yellow-500" />
                <span class="font-semibold">{{ supplier.rating }}</span>
              </div>
              <p class="text-xs text-muted-foreground">Rating</p>
            </div>
            <div class="text-center">
              <p class="font-semibold">{{ supplier.orders }}</p>
              <p class="text-xs text-muted-foreground">Orders</p>
            </div>
          </div>

          <!-- Payment Terms -->
          <div class="bg-muted/50 rounded-lg p-3 mb-4">
            <p class="text-sm font-medium">Payment Terms</p>
            <p class="text-xs text-muted-foreground">{{ supplier.paymentTerms }}</p>
          </div>

          <!-- Actions -->
          <div class="flex space-x-2">
            <Button size="sm" variant="outline" class="flex-1">
              <Phone class="h-4 w-4 mr-1" />
              Call
            </Button>
            <Button size="sm" variant="outline" class="flex-1">
              <Mail class="h-4 w-4 mr-1" />
              Email
            </Button>
            <Button size="sm" class="flex-1">
              <ShoppingCart class="h-4 w-4 mr-1" />
              Order
            </Button>
          </div>
        </div>
      </div>

      <!-- Top Suppliers by Volume -->
      <div class="bg-white dark:bg-gray-800 rounded-lg border">
        <div class="p-6 border-b">
          <h3 class="text-lg font-semibold">Top Suppliers by Volume</h3>
        </div>
        <div class="p-6">
          <div class="space-y-4">
            <div v-for="supplier in topSuppliers" :key="supplier.id" class="flex items-center justify-between p-4 bg-muted/30 rounded-lg">
              <div class="flex items-center space-x-4">
                <div class="w-10 h-10 bg-primary/10 rounded-lg flex items-center justify-center">
                  <span class="text-primary font-bold">{{ supplier.name.charAt(0) }}</span>
                </div>
                <div>
                  <p class="font-medium">{{ supplier.name }}</p>
                  <p class="text-sm text-muted-foreground">{{ supplier.category }}</p>
                </div>
              </div>
              <div class="text-right">
                <p class="font-bold text-lg">R {{ supplier.monthlyVolume.toLocaleString() }}</p>
                <p class="text-sm text-muted-foreground">Monthly volume</p>
              </div>
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
  Upload,
  Search, 
  Truck,
  CheckCircle,
  Clock,
  DollarSign,
  ShoppingCart,
  FileText,
  Star,
  MessageSquare,
  MapPin,
  Phone,
  Mail
} from 'lucide-vue-next'

// Reactive data
const searchQuery = ref('')
const categoryFilter = ref('')
const statusFilter = ref('')
const locationFilter = ref('')

// Stats
const totalSuppliers = ref(23)
const activeSuppliers = ref(18)
const avgLeadTime = ref(3.5)
const monthlySpend = ref(85420)

// Mock suppliers data
const suppliers = ref([
  {
    id: 1,
    name: 'Fresh Produce Wholesalers',
    category: 'fresh-produce',
    status: 'active',
    location: 'Johannesburg Central Market',
    phone: '+27 11 123 4567',
    email: 'orders@freshproduce.co.za',
    leadTime: 1,
    rating: 4.8,
    orders: 45,
    paymentTerms: '30 days net',
    monthlyVolume: 25400
  },
  {
    id: 2,
    name: 'Metro Groceries Supply',
    category: 'groceries',
    status: 'active',
    location: 'Cape Town',
    phone: '+27 21 987 6543',
    email: 'sales@metrogsupply.co.za',
    leadTime: 2,
    rating: 4.6,
    orders: 32,
    paymentTerms: '21 days net',
    monthlyVolume: 18700
  },
  {
    id: 3,
    name: 'Beverage Distributors SA',
    category: 'beverages',
    status: 'active',
    location: 'Durban',
    phone: '+27 31 555 7890',
    email: 'info@bevdist.co.za',
    leadTime: 3,
    rating: 4.4,
    orders: 28,
    paymentTerms: '14 days net',
    monthlyVolume: 15600
  },
  {
    id: 4,
    name: 'Household Essentials Ltd',
    category: 'household',
    status: 'active',
    location: 'Pretoria',
    phone: '+27 12 444 5678',
    email: 'orders@householdessentials.co.za',
    leadTime: 4,
    rating: 4.2,
    orders: 19,
    paymentTerms: '30 days net',
    monthlyVolume: 12300
  },
  {
    id: 5,
    name: 'Township Supply Co',
    category: 'groceries',
    status: 'active',
    location: 'Soweto',
    phone: '+27 11 789 0123',
    email: 'hello@townshipsupply.co.za',
    leadTime: 2,
    rating: 4.7,
    orders: 38,
    paymentTerms: '15 days net',
    monthlyVolume: 22100
  },
  {
    id: 6,
    name: 'Packaging Solutions',
    category: 'packaging',
    status: 'active',
    location: 'Johannesburg',
    phone: '+27 11 333 4444',
    email: 'sales@packagingsolutions.co.za',
    leadTime: 5,
    rating: 4.1,
    orders: 12,
    paymentTerms: '45 days net',
    monthlyVolume: 8900
  },
  {
    id: 7,
    name: 'Local Farmers Co-op',
    category: 'fresh-produce',
    status: 'pending',
    location: 'Alexandra',
    phone: '+27 11 666 7777',
    email: 'orders@localfarmers.co.za',
    leadTime: 1,
    rating: 4.0,
    orders: 8,
    paymentTerms: '7 days net',
    monthlyVolume: 5600
  },
  {
    id: 8,
    name: 'Bulk Buy Groceries',
    category: 'groceries',
    status: 'inactive',
    location: 'Cape Town',
    phone: '+27 21 777 8888',
    email: 'info@bulkbuy.co.za',
    leadTime: 6,
    rating: 3.8,
    orders: 5,
    paymentTerms: '60 days net',
    monthlyVolume: 3400
  }
])

// Computed properties
const filteredSuppliers = computed(() => {
  return suppliers.value.filter(supplier => {
    const matchesSearch = supplier.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
                         supplier.email.toLowerCase().includes(searchQuery.value.toLowerCase())
    const matchesCategory = !categoryFilter.value || supplier.category === categoryFilter.value
    const matchesStatus = !statusFilter.value || supplier.status === statusFilter.value
    const matchesLocation = !locationFilter.value || supplier.location.toLowerCase().includes(locationFilter.value)
    
    return matchesSearch && matchesCategory && matchesStatus && matchesLocation
  })
})

const topSuppliers = computed(() => {
  return suppliers.value
    .filter(s => s.status === 'active')
    .sort((a, b) => b.monthlyVolume - a.monthlyVolume)
    .slice(0, 5)
})

// Methods
const getStatusColor = (status: string) => {
  const colors = {
    'active': 'bg-green-100 text-green-800',
    'inactive': 'bg-gray-100 text-gray-800',
    'pending': 'bg-yellow-100 text-yellow-800'
  }
  return colors[status as keyof typeof colors] || 'bg-gray-100 text-gray-800'
}
</script>