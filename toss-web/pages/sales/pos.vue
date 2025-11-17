<script setup lang="ts">
import { nextTick, onBeforeUnmount } from 'vue'
import { usePosSession } from '~/composables/usePosSession'
import { usePosMock } from '~/composables/usePosMock'
import type { Customer } from '~/composables/usePosMock'
import type { PosSale } from '~/types/sales'
import { useToast } from '~/composables/useToast'
import ProductSearch from '~/components/sales/pos/ProductSearch.vue'
import ProductGrid from '~/components/sales/pos/ProductGrid.vue'
import CartPanel from '~/components/sales/pos/CartPanel.vue'
import PaymentPanel from '~/components/sales/pos/PaymentPanel.vue'
import QuickActions from '~/components/sales/pos/QuickActions.vue'
import ReceiptPreview from '~/components/sales/pos/ReceiptPreview.vue'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Badge } from '~/components/ui/badge'
import { Input } from '~/components/ui/input'
import { Label } from '~/components/ui/label'
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '~/components/ui/dialog'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '~/components/ui/table'
import { Button } from '~/components/ui/button'
import { Loader2, Printer, Scan, UserRound } from 'lucide-vue-next'

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
const productSearchRef = ref<InstanceType<typeof ProductSearch> | null>(null)

const barcodeDialogOpen = ref(false)
const barcodeValue = ref('')
const barcodeError = ref('')
const barcodeLoading = ref(false)
const barcodeInputRef = ref<{ focus: () => void } | null>(null)

const customerDialogOpen = ref(false)
const customerQuery = ref('')
const customerLookupResult = ref<Customer | null>(null)
const customerLookupLoading = ref(false)
const customerLookupError = ref('')
const customerInputRef = ref<{ focus: () => void } | null>(null)

const receiptDialogOpen = ref(false)
const lastCompletedSale = ref<PosSale | null>(null)
const receiptContentRef = ref<HTMLElement | null>(null)

const remainingCredit = computed(() => {
  const currentCustomer = posSession.customer.value
  if (!currentCustomer?.creditLimit) {
    return null
  }
  const used = currentCustomer.creditBalance ?? 0
  return currentCustomer.creditLimit - used
})

const focusSearchInput = () => {
  productSearchRef.value?.focusSearch?.()
}

watch(barcodeDialogOpen, (open) => {
  if (open) {
    barcodeError.value = ''
    barcodeValue.value = ''
    nextTick(() => barcodeInputRef.value?.focus?.())
    return
  }
  barcodeLoading.value = false
  barcodeValue.value = ''
  barcodeError.value = ''
})

watch(customerDialogOpen, (open) => {
  if (open) {
    customerLookupError.value = ''
    nextTick(() => customerInputRef.value?.focus?.())
    return
  }
  customerLookupLoading.value = false
  customerLookupResult.value = null
  customerLookupError.value = ''
})

const handleShortcuts = (event: KeyboardEvent) => {
  const actionableKeys = ['F2', 'F4', 'F6', 'F8', 'F9', 'Escape']
  if (!actionableKeys.includes(event.key)) {
    return
  }

  if ((barcodeDialogOpen.value || customerDialogOpen.value) && event.key !== 'Escape') {
    return
  }

  event.preventDefault()

  switch (event.key) {
    case 'F2':
      focusSearchInput()
      break
    case 'F4':
      if (posSession.canHoldSale.value) {
        handleHoldSale()
      }
      break
    case 'F6':
      openBarcodeDialog()
      break
    case 'F8':
      handleViewHeldSales()
      break
    case 'F9':
      if (posSession.canCompleteSale.value) {
        handleCompleteSale()
      }
      break
    case 'Escape':
      if (posSession.cart.value.length > 0) {
        handleVoidSale()
      }
      break
  }
}

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

  window.addEventListener('keydown', handleShortcuts)
})

onBeforeUnmount(() => {
  window.removeEventListener('keydown', handleShortcuts)
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

const openBarcodeDialog = () => {
  barcodeDialogOpen.value = true
}

const handleScanBarcode = () => {
  openBarcodeDialog()
}

const submitBarcode = async () => {
  if (!barcodeValue.value) {
    barcodeError.value = 'Enter or scan a barcode first'
    return
  }

  barcodeLoading.value = true
  barcodeError.value = ''

  try {
    const product = await posMock.getProductByBarcode(barcodeValue.value, shopId.value)
    if (!product) {
      barcodeError.value = 'No product found for that barcode'
      return
    }
    handleProductSelect(product)
    barcodeDialogOpen.value = false
    toast({
      title: 'Product Added',
      description: `${product.name} scanned successfully`,
    })
  } catch (error) {
    barcodeError.value = 'Failed to read barcode'
    console.error(error)
  } finally {
    barcodeLoading.value = false
  }
}

// Category filter
const filterByCategory = (categoryId: number | undefined) => {
  selectedCategory.value = categoryId
  loadProducts()
}

const openCustomerDialog = () => {
  customerDialogOpen.value = true
  const existing = posSession.customer.value
  customerLookupResult.value = existing
  customerQuery.value = existing?.phone || existing?.name || ''
}

const searchCustomer = async () => {
  if (!customerQuery.value) {
    customerLookupResult.value = null
    customerLookupError.value = 'Enter a phone number or business name'
    return
  }

  customerLookupLoading.value = true
  customerLookupError.value = ''
  try {
    const found = await posMock.fetchCustomerByPhoneOrName(customerQuery.value)
    if (found) {
      customerLookupResult.value = found
    } else {
      customerLookupResult.value = null
      customerLookupError.value = 'No customer found'
    }
  } catch (error) {
    console.error(error)
    customerLookupError.value = 'Failed to search customers'
  } finally {
    customerLookupLoading.value = false
  }
}

const assignCustomer = () => {
  if (!customerLookupResult.value) return
  posSession.setCustomer(customerLookupResult.value)
  toast({
    title: 'Customer Assigned',
    description: `${customerLookupResult.value.name} linked to this sale.`,
  })
  customerDialogOpen.value = false
}

const clearCustomerSelection = () => {
  posSession.setCustomer(null)
  toast({
    title: 'Customer Cleared',
    description: 'Customer removed from the active sale.',
  })
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
    lastCompletedSale.value = sale
    receiptDialogOpen.value = true
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

const openReceiptDialog = () => {
  if (!lastCompletedSale.value) return
  receiptDialogOpen.value = true
}

const printReceipt = () => {
  if (!receiptContentRef.value) return
  const html = receiptContentRef.value.innerHTML
  const printWindow = window.open('', 'PRINT', 'height=600,width=400')
  if (!printWindow) return

  printWindow.document.write(`
    <html>
      <head>
        <title>Receipt</title>
        <style>
          body {
            font-family: ${getComputedStyle(document.documentElement).getPropertyValue('--font-mono') || 'monospace'};
            padding: 16px;
            background: white;
            color: #111;
          }
          .receipt-paper {
            width: 280px;
            margin: 0 auto;
          }
        </style>
      </head>
      <body>
        ${html}
      </body>
    </html>
  `)

  printWindow.document.close()
  printWindow.focus()
  printWindow.print()
  printWindow.close()
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
  <div class="flex flex-col h-screen bg-gradient-to-br from-background via-background to-muted/20">
    <!-- Top Bar -->
    <div class="border-b shadow-sm bg-card">
      <div class="px-4 py-3 space-y-3 md:px-6">
        <div class="flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between">
          <div class="flex flex-wrap gap-3 items-center">
            <div class="flex justify-center items-center w-10 h-10 rounded-lg bg-primary/10">
              <span class="text-xl font-bold text-primary">₵</span>
            </div>
            <div>
              <h1 class="text-xl font-bold tracking-tight">Point of Sale</h1>
              <p class="text-xs text-muted-foreground">Session {{ posSession.sessionId.value?.slice(-8) }}</p>
            </div>

            <div class="flex flex-wrap gap-2 items-center ml-0 lg:ml-4">
              <Button variant="outline" size="sm" class="flex gap-2 items-center" @click="openCustomerDialog">
                <UserRound class="w-4 h-4" />
                <span>
                  {{ posSession.customer.value ? posSession.customer.value.name : 'Walk-in customer' }}
                </span>
              </Button>
              <Badge
                v-if="remainingCredit !== null"
                variant="secondary"
                :class="remainingCredit < 0 ? 'text-destructive border-destructive/50' : 'text-green-700 border-green-200'"
              >
                Credit left: R {{ Number(remainingCredit ?? 0).toFixed(2) }}
              </Badge>
              <Button
                v-if="posSession.customer.value"
                variant="ghost"
                size="sm"
                class="text-muted-foreground"
                @click="clearCustomerSelection"
              >
                Clear
              </Button>
            </div>
          </div>

          <div class="flex flex-col gap-2">
            <div class="flex flex-wrap gap-2 justify-end">
              <Button variant="outline" size="sm" class="hidden md:flex" @click="focusSearchInput" title="Shortcut F2">
                F2 Focus Search
              </Button>
              <Button variant="outline" size="sm" @click="openBarcodeDialog" title="Shortcut F6">
                <Scan class="mr-2 w-4 h-4" />
                Scan Barcode
              </Button>
              <Button
                variant="outline"
                size="sm"
                :disabled="!lastCompletedSale"
                @click="openReceiptDialog"
                title="View last receipt"
              >
                <Printer class="mr-2 w-4 h-4" />
                Last Receipt
              </Button>
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
        <p class="text-xs text-muted-foreground">
          Hotkeys: F2 Focus Search • F4 Hold Sale • F6 Scan Barcode • F8 Held Sales • F9 Complete Sale
        </p>
      </div>
    </div>

    <!-- Main Layout -->
    <div class="overflow-hidden flex-1">
      <div class="h-full grid gap-0 lg:grid-cols-[1fr,420px]">
        <!-- Left: Products -->
        <div class="flex flex-col h-full border-r bg-background">
          <div class="flex-none p-4 space-y-3 border-b bg-card/50">
            <ProductSearch
              ref="productSearchRef"
              @search="handleSearch"
              @scan-barcode="handleScanBarcode"
            />

            <!-- Category Pills -->
            <div class="flex overflow-x-auto gap-2 pb-1 scrollbar-hide">
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

          <div class="overflow-y-auto flex-1 p-4">
            <ProductGrid
              :products="products"
              :loading="loadingProducts"
              @select-product="handleProductSelect"
            />
          </div>
        </div>

        <!-- Right: Cart & Payment -->
        <div class="flex flex-col h-full min-h-0 backdrop-blur-sm bg-card/30">
          <div class="flex-1 min-h-0">
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

    <!-- Barcode Scanner Dialog -->
    <Dialog v-model:open="barcodeDialogOpen">
      <DialogContent class="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Scan Barcode</DialogTitle>
          <DialogDescription>Use a scanner or type the barcode manually.</DialogDescription>
        </DialogHeader>
        <form class="space-y-4" @submit.prevent="submitBarcode">
          <div class="space-y-2">
            <Label for="barcode-input">Barcode</Label>
            <Input
              id="barcode-input"
              ref="barcodeInputRef"
              v-model="barcodeValue"
              placeholder="Scan or type barcode"
              autocomplete="off"
            />
          </div>
          <p v-if="barcodeError" class="text-sm text-destructive">{{ barcodeError }}</p>
          <div class="flex flex-wrap gap-2 justify-between items-center text-xs text-muted-foreground">
            <span>Shortcut: press F6 to focus this scanner.</span>
            <div class="flex gap-2">
              <Button variant="ghost" type="button" @click="barcodeDialogOpen = false">Cancel</Button>
              <Button type="submit" :disabled="barcodeLoading || !barcodeValue">
                <Loader2 v-if="barcodeLoading" class="mr-2 w-4 h-4 animate-spin" />
                Add Product
              </Button>
            </div>
          </div>
        </form>
      </DialogContent>
    </Dialog>

    <!-- Customer Lookup Dialog -->
    <Dialog v-model:open="customerDialogOpen">
      <DialogContent class="sm:max-w-lg">
        <DialogHeader>
          <DialogTitle>Select Customer</DialogTitle>
          <DialogDescription>Search mock customers by phone number or business name.</DialogDescription>
        </DialogHeader>
        <form class="space-y-4" @submit.prevent="searchCustomer">
          <div class="space-y-2">
            <Label for="customer-query">Phone or Name</Label>
            <Input
              id="customer-query"
              ref="customerInputRef"
              v-model="customerQuery"
              placeholder="+2782 123 4567 or Jabu's Spaza"
            />
          </div>
          <div class="flex gap-2 justify-end">
            <Button type="submit" :disabled="customerLookupLoading">
              <Loader2 v-if="customerLookupLoading" class="mr-2 w-4 h-4 animate-spin" />
              Find Customer
            </Button>
          </div>
        </form>
        <p v-if="customerLookupError" class="text-sm text-destructive">{{ customerLookupError }}</p>
        <div
          v-if="customerLookupResult"
          class="p-4 space-y-2 rounded-lg border border-border"
        >
          <div>
            <p class="font-semibold">{{ customerLookupResult.name }}</p>
            <p class="text-xs text-muted-foreground">
              {{ customerLookupResult.phone || 'No phone on record' }}
            </p>
          </div>
          <div class="space-y-1 text-xs text-muted-foreground">
            <p>Credit Limit: R {{ Number(customerLookupResult.creditLimit ?? 0).toFixed(2) }}</p>
            <p>Balance Used: R {{ Number(customerLookupResult.creditBalance ?? 0).toFixed(2) }}</p>
          </div>
          <div class="flex flex-wrap gap-2">
            <Button size="sm" @click="assignCustomer">Assign to Sale</Button>
            <Button size="sm" variant="outline" @click="clearCustomerSelection">Clear Customer</Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>

    <!-- Receipt Preview Dialog -->
    <Dialog v-model:open="receiptDialogOpen">
      <DialogContent class="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Receipt Preview</DialogTitle>
          <DialogDescription>Printable summary of the last completed sale.</DialogDescription>
        </DialogHeader>
        <div ref="receiptContentRef" class="max-h-[60vh] overflow-y-auto p-2 bg-white rounded">
          <ReceiptPreview :sale="lastCompletedSale" />
        </div>
        <div class="flex gap-2 justify-end">
          <Button variant="outline" :disabled="!lastCompletedSale" @click="printReceipt">
            <Printer class="mr-2 w-4 h-4" />
            Print Receipt
          </Button>
          <Button variant="ghost" @click="receiptDialogOpen = false">Close</Button>
        </div>
      </DialogContent>
    </Dialog>
  </div>
</template>
