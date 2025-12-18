<template>
  <div class="h-full overflow-y-auto p-6 custom-scrollbar">
    <!-- Page Header -->
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-stone-900 dark:text-white mb-2">Point of Sale</h1>
      <p class="text-stone-500 dark:text-stone-400">Fast and efficient sales processing for your retail operations</p>
    </div>

    <!-- Quick Stats -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500 dark:text-stone-400">Today's Sales</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">R 12,450</p>
            </div>
            <div class="w-12 h-12 bg-green-100 dark:bg-green-900/30 rounded-lg flex items-center justify-center">
              <Icon name="lucide:trending-up" class="w-6 h-6 text-green-600 dark:text-green-400" />
            </div>
          </div>
          <div class="mt-4 flex items-center text-sm">
            <span class="text-green-600 dark:text-green-400 font-medium">+12%</span>
            <span class="text-stone-500 dark:text-stone-400 ml-2">from yesterday</span>
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500 dark:text-stone-400">Transactions</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">156</p>
            </div>
            <div class="w-12 h-12 bg-blue-100 dark:bg-blue-900/30 rounded-lg flex items-center justify-center">
              <Icon name="lucide:receipt" class="w-6 h-6 text-blue-600 dark:text-blue-400" />
            </div>
          </div>
          <div class="mt-4 flex items-center text-sm">
            <span class="text-blue-600 dark:text-blue-400 font-medium">+8%</span>
            <span class="text-stone-500 dark:text-stone-400 ml-2">from yesterday</span>
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500 dark:text-stone-400">Avg. Basket</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">R 79.80</p>
            </div>
            <div class="w-12 h-12 bg-purple-100 dark:bg-purple-900/30 rounded-lg flex items-center justify-center">
              <Icon name="lucide:shopping-basket" class="w-6 h-6 text-purple-600 dark:text-purple-400" />
            </div>
          </div>
          <div class="mt-4 flex items-center text-sm">
            <span class="text-purple-600 dark:text-purple-400 font-medium">+5%</span>
            <span class="text-stone-500 dark:text-stone-400 ml-2">from last week</span>
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500 dark:text-stone-400">Items Sold</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">892</p>
            </div>
            <div class="w-12 h-12 bg-orange-100 dark:bg-orange-900/30 rounded-lg flex items-center justify-center">
              <Icon name="lucide:package" class="w-6 h-6 text-orange-600 dark:text-orange-400" />
            </div>
          </div>
          <div class="mt-4 flex items-center text-sm">
            <span class="text-orange-600 dark:text-orange-400 font-medium">+15%</span>
            <span class="text-stone-500 dark:text-stone-400 ml-2">from yesterday</span>
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- POS Terminal -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Product Search & Grid -->
      <div class="lg:col-span-2 space-y-6">
        <!-- Search Bar -->
        <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
          <CardContent class="p-4">
            <div class="relative">
              <Icon name="lucide:search" class="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-stone-400" />
              <input
                v-model="searchQuery"
                type="text"
                placeholder="Search products by name, SKU, or scan barcode..."
                class="w-full pl-10 pr-4 py-3 border border-stone-200 dark:border-stone-700 rounded-lg bg-stone-50 dark:bg-stone-900 text-stone-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>
          </CardContent>
        </Card>

        <!-- Category Tabs -->
        <div class="flex gap-2 overflow-x-auto pb-2">
          <button
            v-for="category in categories"
            :key="category.id"
            @click="selectedCategory = category.id"
            :class="[
              'px-4 py-2 rounded-lg text-sm font-medium whitespace-nowrap transition-colors',
              selectedCategory === category.id
                ? 'bg-stone-900 text-white dark:bg-white dark:text-stone-900'
                : 'bg-white dark:bg-stone-800 text-stone-600 dark:text-stone-400 border border-stone-200 dark:border-stone-700 hover:bg-stone-50 dark:hover:bg-stone-700'
            ]"
          >
            {{ category.name }}
          </button>
        </div>

        <!-- Product Grid -->
        <div class="grid grid-cols-2 md:grid-cols-3 xl:grid-cols-4 gap-4">
          <Card
            v-for="product in filteredProducts"
            :key="product.id"
            class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700 cursor-pointer hover:shadow-lg transition-shadow"
            @click="addToCart(product)"
          >
            <CardContent class="p-4 text-center">
              <div class="w-16 h-16 mx-auto mb-3 bg-stone-100 dark:bg-stone-700 rounded-lg flex items-center justify-center">
                <Icon :name="product.icon" class="w-8 h-8 text-stone-600 dark:text-stone-300" />
              </div>
              <h3 class="text-sm font-medium text-stone-900 dark:text-white truncate">{{ product.name }}</h3>
              <p class="text-lg font-bold text-stone-900 dark:text-white mt-1">R {{ product.price.toFixed(2) }}</p>
              <p class="text-xs text-stone-500 dark:text-stone-400">Stock: {{ product.stock }}</p>
            </CardContent>
          </Card>
        </div>
      </div>

      <!-- Cart / Checkout -->
      <div class="lg:col-span-1">
        <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700 sticky top-6">
          <CardHeader class="border-b border-stone-200 dark:border-stone-700">
            <div class="flex items-center justify-between">
              <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Current Sale</CardTitle>
              <Badge variant="secondary" class="bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400">
                {{ cart.length }} items
              </Badge>
            </div>
          </CardHeader>

          <CardContent class="p-0">
            <!-- Cart Items -->
            <div class="max-h-[400px] overflow-y-auto">
              <div
                v-for="item in cart"
                :key="item.id"
                class="p-4 border-b border-stone-100 dark:border-stone-700 last:border-0"
              >
                <div class="flex items-center justify-between">
                  <div class="flex-1">
                    <h4 class="text-sm font-medium text-stone-900 dark:text-white">{{ item.name }}</h4>
                    <p class="text-xs text-stone-500 dark:text-stone-400">R {{ item.price.toFixed(2) }} each</p>
                  </div>
                  <div class="flex items-center gap-2">
                    <button
                      @click="updateQuantity(item.id, -1)"
                      class="w-8 h-8 rounded-lg bg-stone-100 dark:bg-stone-700 flex items-center justify-center hover:bg-stone-200 dark:hover:bg-stone-600"
                    >
                      <Icon name="lucide:minus" class="w-4 h-4" />
                    </button>
                    <span class="w-8 text-center font-medium">{{ item.quantity }}</span>
                    <button
                      @click="updateQuantity(item.id, 1)"
                      class="w-8 h-8 rounded-lg bg-stone-100 dark:bg-stone-700 flex items-center justify-center hover:bg-stone-200 dark:hover:bg-stone-600"
                    >
                      <Icon name="lucide:plus" class="w-4 h-4" />
                    </button>
                  </div>
                </div>
                <div class="flex items-center justify-between mt-2">
                  <button @click="removeFromCart(item.id)" class="text-xs text-red-500 hover:text-red-600">
                    Remove
                  </button>
                  <span class="text-sm font-semibold text-stone-900 dark:text-white">
                    R {{ (item.price * item.quantity).toFixed(2) }}
                  </span>
                </div>
              </div>

              <div v-if="cart.length === 0" class="p-8 text-center">
                <Icon name="lucide:shopping-cart" class="w-12 h-12 mx-auto text-stone-300 dark:text-stone-600 mb-3" />
                <p class="text-stone-500 dark:text-stone-400">Cart is empty</p>
                <p class="text-xs text-stone-400 dark:text-stone-500">Click products to add them</p>
              </div>
            </div>

            <!-- Totals -->
            <div class="p-4 bg-stone-50 dark:bg-stone-900 space-y-2">
              <div class="flex justify-between text-sm">
                <span class="text-stone-500 dark:text-stone-400">Subtotal</span>
                <span class="text-stone-900 dark:text-white">R {{ subtotal.toFixed(2) }}</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-stone-500 dark:text-stone-400">VAT (15%)</span>
                <span class="text-stone-900 dark:text-white">R {{ vat.toFixed(2) }}</span>
              </div>
              <div class="flex justify-between text-lg font-bold pt-2 border-t border-stone-200 dark:border-stone-700">
                <span class="text-stone-900 dark:text-white">Total</span>
                <span class="text-stone-900 dark:text-white">R {{ total.toFixed(2) }}</span>
              </div>
            </div>

            <!-- Payment Buttons -->
            <div class="p-4 space-y-3">
              <Button class="w-full bg-stone-900 hover:bg-stone-800 dark:bg-white dark:hover:bg-stone-100 dark:text-stone-900" size="lg">
                <Icon name="lucide:banknote" class="w-5 h-5 mr-2" />
                Pay Cash
              </Button>
              <Button variant="outline" class="w-full" size="lg">
                <Icon name="lucide:credit-card" class="w-5 h-5 mr-2" />
                Pay Card
              </Button>
              <Button variant="ghost" class="w-full text-red-500 hover:text-red-600 hover:bg-red-50 dark:hover:bg-red-900/20" @click="clearCart">
                <Icon name="lucide:trash-2" class="w-5 h-5 mr-2" />
                Clear Cart
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Card, CardContent, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'
import { Badge } from '~/components/ui/badge'

definePageMeta({
  layout: 'default'
})

const searchQuery = ref('')
const selectedCategory = ref('all')

const categories = [
  { id: 'all', name: 'All Products' },
  { id: 'groceries', name: 'Groceries' },
  { id: 'beverages', name: 'Beverages' },
  { id: 'snacks', name: 'Snacks' },
  { id: 'household', name: 'Household' },
  { id: 'airtime', name: 'Airtime' },
]

const products = ref([
  { id: 1, name: 'White Bread', price: 18.99, stock: 45, category: 'groceries', icon: 'lucide:sandwich' },
  { id: 2, name: 'Maize Meal 2.5kg', price: 42.99, stock: 32, category: 'groceries', icon: 'lucide:wheat' },
  { id: 3, name: 'Sunflower Oil 750ml', price: 38.50, stock: 28, category: 'groceries', icon: 'lucide:droplet' },
  { id: 4, name: 'Sugar 2.5kg', price: 54.99, stock: 40, category: 'groceries', icon: 'lucide:candy' },
  { id: 5, name: 'Coca-Cola 2L', price: 24.99, stock: 60, category: 'beverages', icon: 'lucide:cup-soda' },
  { id: 6, name: 'Fanta Orange 2L', price: 24.99, stock: 48, category: 'beverages', icon: 'lucide:cup-soda' },
  { id: 7, name: 'Milk 1L', price: 19.99, stock: 35, category: 'beverages', icon: 'lucide:milk' },
  { id: 8, name: 'Simba Chips', price: 14.99, stock: 80, category: 'snacks', icon: 'lucide:cookie' },
  { id: 9, name: 'NikNaks', price: 12.99, stock: 90, category: 'snacks', icon: 'lucide:cookie' },
  { id: 10, name: 'Toilet Paper 9pk', price: 89.99, stock: 20, category: 'household', icon: 'lucide:scroll' },
  { id: 11, name: 'Sunlight Soap', price: 24.99, stock: 55, category: 'household', icon: 'lucide:sparkles' },
  { id: 12, name: 'MTN R10 Airtime', price: 10.00, stock: 999, category: 'airtime', icon: 'lucide:smartphone' },
])

interface CartItem {
  id: number
  name: string
  price: number
  quantity: number
}

const cart = ref<CartItem[]>([])

const filteredProducts = computed(() => {
  return products.value.filter(p => {
    const matchesCategory = selectedCategory.value === 'all' || p.category === selectedCategory.value
    const matchesSearch = p.name.toLowerCase().includes(searchQuery.value.toLowerCase())
    return matchesCategory && matchesSearch
  })
})

const subtotal = computed(() => cart.value.reduce((sum, item) => sum + item.price * item.quantity, 0))
const vat = computed(() => subtotal.value * 0.15)
const total = computed(() => subtotal.value + vat.value)

const addToCart = (product: typeof products.value[0]) => {
  const existing = cart.value.find(item => item.id === product.id)
  if (existing) {
    existing.quantity++
  } else {
    cart.value.push({
      id: product.id,
      name: product.name,
      price: product.price,
      quantity: 1
    })
  }
}

const updateQuantity = (id: number, delta: number) => {
  const item = cart.value.find(i => i.id === id)
  if (item) {
    item.quantity += delta
    if (item.quantity <= 0) {
      removeFromCart(id)
    }
  }
}

const removeFromCart = (id: number) => {
  const index = cart.value.findIndex(i => i.id === id)
  if (index > -1) {
    cart.value.splice(index, 1)
  }
}

const clearCart = () => {
  cart.value = []
}
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar {
  width: 6px;
}

.custom-scrollbar::-webkit-scrollbar-track {
  background: hsl(var(--muted));
}

.custom-scrollbar::-webkit-scrollbar-thumb {
  background: hsl(var(--muted-foreground));
  border-radius: 3px;
}
</style>
