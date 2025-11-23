<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-3xl font-bold">Sales & POS</h1>
        <p class="text-muted-foreground">Point of sale and sales management</p>
      </div>
      <Button>
        <Icon name="lucide:plus" class="w-4 h-4 mr-2" />
        New Sale
      </Button>
    </div>
    
    <!-- Quick Stats -->
    <div class="grid gap-4 md:grid-cols-4">
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Today's Sales</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">R {{ formatCurrency(todaySales) }}</div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">This Week</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">R {{ formatCurrency(weekSales) }}</div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Transactions</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">{{ transactionCount }}</div>
        </CardContent>
      </Card>
      <Card>
        <CardHeader class="pb-2">
          <CardTitle class="text-sm font-medium">Avg. Sale</CardTitle>
        </CardHeader>
        <CardContent>
          <div class="text-2xl font-bold">R {{ formatCurrency(avgSale) }}</div>
        </CardContent>
      </Card>
    </div>
    
    <!-- POS Interface -->
    <div class="grid gap-4 lg:grid-cols-3">
      <Card class="lg:col-span-2">
        <CardHeader>
          <CardTitle>Point of Sale</CardTitle>
        </CardHeader>
        <CardContent>
          <!-- Cart Items -->
          <div class="space-y-2 mb-4 min-h-[300px]">
            <div v-if="cart.length === 0" class="text-center py-12 text-muted-foreground">
              <Icon name="lucide:shopping-cart" class="w-12 h-12 mx-auto mb-2 opacity-50" />
              <p>Cart is empty. Add products to start a sale.</p>
            </div>
            <div v-for="item in cart" :key="item.id" class="flex items-center justify-between p-3 border rounded-lg">
              <div class="flex-1">
                <p class="font-medium">{{ item.name }}</p>
                <p class="text-sm text-muted-foreground">R {{ formatCurrency(item.price) }} each</p>
              </div>
              <div class="flex items-center space-x-3">
                <div class="flex items-center space-x-2">
                  <Button variant="outline" size="sm" @click="updateQuantity(item.id, -1)">-</Button>
                  <span class="w-12 text-center">{{ item.quantity }}</span>
                  <Button variant="outline" size="sm" @click="updateQuantity(item.id, 1)">+</Button>
                </div>
                <p class="font-medium w-20 text-right">R {{ formatCurrency(item.price * item.quantity) }}</p>
                <Button variant="ghost" size="sm" @click="removeFromCart(item.id)">
                  <Icon name="lucide:trash" class="w-4 h-4" />
                </Button>
              </div>
            </div>
          </div>
          
          <!-- Total -->
          <div v-if="cart.length > 0" class="border-t pt-4 space-y-2">
            <div class="flex justify-between text-lg">
              <span class="font-medium">Total:</span>
              <span class="font-bold text-2xl">R {{ formatCurrency(cartTotal) }}</span>
            </div>
            <Button class="w-full" size="lg" @click="processSale">
              Complete Sale
            </Button>
          </div>
        </CardContent>
      </Card>
      
      <!-- Product Search -->
      <Card>
        <CardHeader>
          <CardTitle>Add Products</CardTitle>
        </CardHeader>
        <CardContent>
          <Input placeholder="Search products..." class="mb-4" v-model="searchQuery" />
          <div class="space-y-2 max-h-[500px] overflow-y-auto">
            <div
              v-for="product in filteredProducts"
              :key="product.id"
              class="p-3 border rounded-lg hover:bg-accent cursor-pointer"
              @click="addToCart(product)"
            >
              <p class="font-medium">{{ product.name }}</p>
              <p class="text-sm text-muted-foreground">R {{ formatCurrency(product.price) }}</p>
              <p class="text-xs text-muted-foreground">Stock: {{ product.stock }}</p>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
    
    <!-- Recent Sales -->
    <Card>
      <CardHeader>
        <CardTitle>Recent Sales</CardTitle>
      </CardHeader>
      <CardContent>
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead>
              <tr class="border-b">
                <th class="text-left p-4 font-medium">Date</th>
                <th class="text-left p-4 font-medium">Invoice #</th>
                <th class="text-left p-4 font-medium">Customer</th>
                <th class="text-left p-4 font-medium">Items</th>
                <th class="text-left p-4 font-medium">Total</th>
                <th class="text-right p-4 font-medium">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="sale in recentSales" :key="sale.id" class="border-b hover:bg-muted/50">
                <td class="p-4">{{ sale.date }}</td>
                <td class="p-4">{{ sale.invoice }}</td>
                <td class="p-4">{{ sale.customer || 'Walk-in' }}</td>
                <td class="p-4">{{ sale.items }}</td>
                <td class="p-4">R {{ formatCurrency(sale.total) }}</td>
                <td class="p-4 text-right">
                  <Button variant="ghost" size="sm">
                    <Icon name="lucide:eye" class="w-4 h-4" />
                  </Button>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'admin'
})

const todaySales = ref(5420)
const weekSales = ref(28500)
const transactionCount = ref(127)
const avgSale = ref(42.68)

const searchQuery = ref('')
const cart = ref<Array<{ id: number; name: string; price: number; quantity: number }>>([])

const products = ref([
  { id: 1, name: 'White Bread', price: 12.50, stock: 25 },
  { id: 2, name: 'Cooking Oil 2L', price: 45.00, stock: 8 },
  { id: 3, name: 'Maize Meal 5kg', price: 65.00, stock: 15 },
  { id: 4, name: 'Sugar 2kg', price: 35.00, stock: 0 },
  { id: 5, name: 'Milk 2L', price: 28.00, stock: 12 }
])

const recentSales = ref([
  { id: 1, date: '2025-01-15', invoice: 'INV-001', customer: 'John Doe', items: 3, total: 85.50 },
  { id: 2, date: '2025-01-15', invoice: 'INV-002', customer: null, items: 2, total: 45.00 },
  { id: 3, date: '2025-01-14', invoice: 'INV-003', customer: 'Jane Smith', items: 5, total: 120.00 }
])

const filteredProducts = computed(() => {
  if (!searchQuery.value) return products.value
  return products.value.filter(p => 
    p.name.toLowerCase().includes(searchQuery.value.toLowerCase())
  )
})

const cartTotal = computed(() => {
  return cart.value.reduce((sum, item) => sum + (item.price * item.quantity), 0)
})

const addToCart = (product: typeof products.value[0]) => {
  const existing = cart.value.find(item => item.id === product.id)
  if (existing) {
    existing.quantity++
  } else {
    cart.value.push({ ...product, quantity: 1 })
  }
}

const updateQuantity = (id: number, delta: number) => {
  const item = cart.value.find(i => i.id === id)
  if (item) {
    item.quantity = Math.max(1, item.quantity + delta)
  }
}

const removeFromCart = (id: number) => {
  cart.value = cart.value.filter(item => item.id !== id)
}

const processSale = () => {
  // TODO: Implement sale processing
  alert('Sale processed!')
  cart.value = []
}

const formatCurrency = (amount: number) => {
  return amount.toLocaleString('en-ZA', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
}

useHead({
  title: 'Sales & POS - TOSS'
})
</script>


