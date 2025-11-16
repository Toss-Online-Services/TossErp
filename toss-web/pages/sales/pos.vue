<script setup lang="ts">
import { usePosSession } from '~/composables/usePosSession'
import { usePosMock } from '~/composables/usePosMock'
import { useToast } from '~/composables/useToast'
import ProductSearch from '~/components/sales/pos/ProductSearch.vue'
import ProductGrid from '~/components/sales/pos/ProductGrid.vue'
import CartPanel from '~/components/sales/pos/CartPanel.vue'
import PaymentPanel from '~/components/sales/pos/PaymentPanel.vue'
import QuickActions from '~/components/sales/pos/QuickActions.vue'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Badge } from '~/components/ui/badge'
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '~/components/ui/dialog'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '~/components/ui/table'
import { Button } from '~/components/ui/button'

definePageMeta({
  middleware: 'auth',
})

const { toast } = useToast()
const posSession = usePosSession()
const posMock = usePosMock()

// State
const products = ref<any[]>([])
const categories = ref<any[]>([])
const loadingProducts = ref(false)
const selectedCategory = ref<number | undefined>()
const searchTerm = ref('')
const shopId = ref(1) // TODO: Get from auth/shop context
const showHeldSales = ref(false)
const showRecentSales = ref(false)
const heldSales = ref<any[]>([])
const recentSales = ref<any[]>([])

// Initialize session
onMounted(async () => {
  const sessionId = `SESSION-${Date.now()}`
  posSession.initSession(sessionId)
  
  // Try to restore from localStorage
  const restored = posSession.restoreFromLocalStorage()
  if (restored) {
    toast({
      title: 'Session Restored',
      description: 'Your previous POS session has been restored.',
    })
  }
  
  // Load initial data
  await loadProducts()
  await loadCategories()
})

// Load products
const loadProducts = async () => {
  loadingProducts.value = true
  try {
    products.value = await posMock.fetchProducts(
      shopId.value,
      searchTerm.value || undefined,
      selectedCategory.value
    )
  } catch (error) {
    toast({
      title: 'Error',
      description: 'Failed to load products',
      variant: 'destructive',
    })
  } finally {
    loadingProducts.value = false
  }
}

// Load categories
const loadCategories = async () => {
  try {
    categories.value = await posMock.fetchCategories(shopId.value)
  } catch (error) {
    console.error('Failed to load categories:', error)
  }
}

// Product selection
const handleProductSelect = (product: any) => {
  posSession.addToCart(product, 1)
  toast({
    title: 'Added to Cart',
    description: `${product.name} added to cart`,
  })
}

// Search
const handleSearch = (term: string) => {
  searchTerm.value = term
  loadProducts()
}

const handleScanBarcode = async () => {
  // TODO: Implement barcode scanning
  toast({
    title: 'Barcode Scanner',
    description: 'Barcode scanning not yet implemented',
  })
}

// Category filter
const filterByCategory = (categoryId: number | undefined) => {
  selectedCategory.value = categoryId
  loadProducts()
}

// Payment
const handleAddPayment = (payment: any) => {
  posSession.addPayment(payment)
}

const handleRemovePayment = (paymentId: number) => {
  posSession.removePayment(paymentId)
}

const handleCompleteSale = async () => {
  try {
    const sale = await posSession.completeSale()
    toast({
      title: 'Sale Complete',
      description: `Sale ${sale.reference} completed successfully`,
    })
    posSession.clearLocalStorage()
  } catch (error: any) {
    toast({
      title: 'Error',
      description: error.message || 'Failed to complete sale',
      variant: 'destructive',
    })
  }
}

// Quick actions
const handleHoldSale = () => {
  try {
    const parkedSale = posSession.holdSale()
    toast({
      title: 'Sale Held',
      description: `Sale ${parkedSale.reference} has been parked`,
    })
    posSession.clearLocalStorage()
  } catch (error: any) {
    toast({
      title: 'Error',
      description: error.message || 'Failed to hold sale',
      variant: 'destructive',
    })
  }
}

const handleVoidSale = () => {
  posSession.voidSale()
  posSession.clearLocalStorage()
  toast({
    title: 'Sale Voided',
    description: 'The current sale has been cancelled',
  })
}

const handleViewHeldSales = () => {
  heldSales.value = posMock.listHeldSales()
  showHeldSales.value = true
}

const handleViewRecentSales = () => {
  recentSales.value = posMock.listRecentSales(posSession.sessionId.value)
  showRecentSales.value = true
}

const recallHeldSale = (sale: any) => {
  posSession.recallSale(sale)
  showHeldSales.value = false
  toast({
    title: 'Sale Recalled',
    description: `Sale ${sale.reference} has been restored`,
  })
}

const formatPrice = (price: number) => {
  return `R ${price.toFixed(2)}`
}

const formatDate = (date: string) => {
  return new Date(date).toLocaleString()
}
</script>

<template>
  <div class="h-screen flex flex-col bg-gradient-to-br from-background via-background to-muted/20">
    <!-- Top Bar -->
    <div class="bg-card border-b shadow-sm">
      <div class="px-4 md:px-6 py-3">
        <div class="flex items-center justify-between">
          <div class="flex items-center gap-3">
            <div class="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center">
              <span class="text-xl font-bold text-primary">₵</span>
            </div>
            <div>
              <h1 class="text-xl font-bold tracking-tight">Point of Sale</h1>
              <p class="text-xs text-muted-foreground">Session {{ posSession.sessionId.value?.slice(-8) }}</p>
            </div>
          </div>
          <QuickActions
            :can-hold="posSession.canHoldSale.value"
            :can-void="posSession.cart.value.length > 0"
            :can-complete="posSession.canCompleteSale.value"
            @hold-sale="handleHoldSale"
            @void-sale="handleVoidSale"
            @view-held-sales="handleViewHeldSales"
            @view-recent-sales="handleViewRecentSales"
          />
        </div>
      </div>
    </div>

    <!-- Main Layout -->
    <div class="flex-1 overflow-hidden">
      <div class="h-full grid gap-0 lg:grid-cols-[1fr,420px]">
        <!-- Left: Products -->
        <div class="flex flex-col h-full border-r bg-background">
          <div class="flex-none p-4 space-y-3 border-b bg-card/50">
            <ProductSearch
              @search="handleSearch"
              @scan-barcode="handleScanBarcode"
            />

            <!-- Category Pills -->
            <div class="flex gap-2 overflow-x-auto pb-1 scrollbar-hide">
              <Button
                variant="ghost"
                size="sm"
                :class="[
                  'rounded-full px-4 transition-all',
                  !selectedCategory 
                    ? 'bg-primary text-primary-foreground shadow-sm hover:bg-primary/90' 
                    : 'hover:bg-accent'
                ]"
                @click="filterByCategory(undefined)"
              >
                All Products
              </Button>
              <Button
                v-for="category in categories"
                :key="category.id"
                variant="ghost"
                size="sm"
                :class="[
                  'rounded-full px-4 whitespace-nowrap transition-all',
                  selectedCategory === category.id 
                    ? 'bg-primary text-primary-foreground shadow-sm hover:bg-primary/90' 
                    : 'hover:bg-accent'
                ]"
                @click="filterByCategory(category.id)"
              >
                {{ category.name }}
              </Button>
            </div>
          </div>

          <div class="flex-1 overflow-y-auto p-4">
            <ProductGrid
              :products="products"
              :loading="loadingProducts"
              @select-product="handleProductSelect"
            />
          </div>
        </div>

        <!-- Right: Cart & Payment -->
        <div class="flex flex-col h-full bg-card/30 backdrop-blur-sm">
          <div class="flex-1 overflow-y-auto">
            <CartPanel
              :items="posSession.cart.value"
              :subtotal="posSession.subtotal.value"
              :total-tax="posSession.totalTax.value"
              :total-discount="posSession.totalDiscount.value"
              :total="posSession.total.value"
              @update-quantity="posSession.updateQuantity"
              @remove-item="posSession.removeFromCart"
              @apply-discount="posSession.applyDiscount"
            />
          </div>

          <div class="flex-none border-t bg-card">
            <PaymentPanel
              :total="posSession.total.value"
              :payments="posSession.payments.value"
              :total-paid="posSession.totalPaid.value"
              :balance="posSession.balance.value"
              :is-complete="posSession.isPaymentComplete.value"
              @add-payment="handleAddPayment"
              @remove-payment="handleRemovePayment"
              @complete-sale="handleCompleteSale"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- Held Sales Dialog -->
    <Dialog v-model:open="showHeldSales">
      <DialogContent class="max-w-4xl">
        <DialogHeader>
          <DialogTitle>Held Sales</DialogTitle>
          <DialogDescription>Select a sale to recall</DialogDescription>
        </DialogHeader>
        <div class="max-h-[60vh] overflow-y-auto">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Reference</TableHead>
                <TableHead>Items</TableHead>
                <TableHead>Total</TableHead>
                <TableHead>Date</TableHead>
                <TableHead>Action</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              <TableRow v-for="sale in heldSales" :key="sale.reference">
                <TableCell>{{ sale.reference }}</TableCell>
                <TableCell>{{ sale.items.length }}</TableCell>
                <TableCell>{{ formatPrice(sale.total) }}</TableCell>
                <TableCell>{{ formatDate(sale.createdAt) }}</TableCell>
                <TableCell>
                  <Button size="sm" @click="recallHeldSale(sale)">Recall</Button>
                </TableCell>
              </TableRow>
              <TableRow v-if="heldSales.length === 0">
                <TableCell colspan="5" class="text-center text-muted-foreground">
                  No held sales
                </TableCell>
              </TableRow>
            </TableBody>
          </Table>
        </div>
      </DialogContent>
    </Dialog>

    <!-- Recent Sales Dialog -->
    <Dialog v-model:open="showRecentSales">
      <DialogContent class="max-w-4xl">
        <DialogHeader>
          <DialogTitle>Recent Sales</DialogTitle>
          <DialogDescription>Sales from this session</DialogDescription>
        </DialogHeader>
        <div class="max-h-[60vh] overflow-y-auto">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Reference</TableHead>
                <TableHead>Items</TableHead>
                <TableHead>Total</TableHead>
                <TableHead>Payment</TableHead>
                <TableHead>Date</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              <TableRow v-for="sale in recentSales" :key="sale.reference">
                <TableCell>{{ sale.reference }}</TableCell>
                <TableCell>{{ sale.items.length }}</TableCell>
                <TableCell>{{ formatPrice(sale.total) }}</TableCell>
                <TableCell>
                  <div class="flex flex-wrap gap-1">
                    <Badge v-for="payment in sale.payments" :key="payment.id" variant="secondary" class="text-xs">
                      {{ payment.method }}: {{ formatPrice(payment.amount) }}
                    </Badge>
                  </div>
                </TableCell>
                <TableCell>{{ formatDate(sale.createdAt) }}</TableCell>
              </TableRow>
              <TableRow v-if="recentSales.length === 0">
                <TableCell colspan="5" class="text-center text-muted-foreground">
                  No recent sales
                </TableCell>
              </TableRow>
            </TableBody>
          </Table>
        </div>
      </DialogContent>
    </Dialog>
  </div>
</template>
