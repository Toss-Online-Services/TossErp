<template>
  <div class="relative min-h-screen overflow-hidden bg-gradient-to-br from-slate-50 via-blue-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Mobile-First Page Container -->
    <div class="p-4 pb-20 space-y-4 sm:p-6 sm:space-y-6 lg:pb-6">
    <!-- Page Header -->
      <div class="flex flex-col justify-between gap-3 sm:flex-row sm:items-center sm:gap-0">
        <div>
          <h1 class="text-2xl font-bold text-transparent sm:text-3xl bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text">Point of Sale</h1>
          <p class="mt-1 text-sm text-slate-600 dark:text-slate-400 sm:text-base">Quick checkout system for Thabo's Spaza Shop</p>
        </div>
        <div class="flex flex-wrap items-center gap-2 sm:gap-3">
          <!-- Queue Status Badge -->
          <div 
            v-if="queuePendingCount > 0" 
            class="flex items-center px-3 py-2 space-x-2 bg-orange-100 border border-orange-300 rounded-lg dark:bg-orange-900/30 dark:border-orange-700"
            title="Pending transactions awaiting sync"
          >
            <svg class="w-4 h-4 text-orange-600 dark:text-orange-400 animate-pulse" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            <span class="text-xs font-semibold text-orange-700 dark:text-orange-400">{{ queuePendingCount }} queued</span>
          </div>
          
          <NuxtLink 
            to="/sales/pos/queue"
            class="flex-1 px-4 py-2 text-sm font-semibold text-white transition-all duration-200 shadow-lg sm:flex-none sm:px-6 sm:py-3 bg-gradient-to-r from-orange-600 to-amber-600 hover:from-orange-700 hover:to-amber-700 rounded-xl hover:shadow-xl sm:text-base"
            title="View Order Queue"
          >
            📋 Order Queue
          </NuxtLink>
          <button @click="toggleFullscreen" 
                  :class="[
                    'flex-1 sm:flex-none px-4 py-2 sm:px-6 sm:py-3 text-white rounded-xl shadow-lg hover:shadow-xl transition-all duration-200 text-sm sm:text-base font-semibold',
                    isFullscreen ? 'bg-gradient-to-r from-purple-600 to-pink-600 hover:from-purple-700 hover:to-pink-700' : 'bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700'
                  ]"
                  :title="isFullscreen ? 'Exit Fullscreen (F11)' : 'Enter Fullscreen (F11)'">
            <component :is="isFullscreen ? ArrowsPointingInIcon : ArrowsPointingOutIcon" class="inline w-4 h-4 mr-2 sm:w-5 sm:h-5" />
            {{ isFullscreen ? 'Exit' : 'Fullscreen' }}
          </button>
          <button 
            @click="showSettingsModal = true"
            class="flex-1 px-4 py-2 text-sm font-semibold text-white transition-all duration-200 shadow-lg sm:flex-none sm:px-6 sm:py-3 bg-gradient-to-r from-slate-600 to-gray-600 hover:from-slate-700 hover:to-gray-700 rounded-xl hover:shadow-xl sm:text-base"
            title="Settings & Hardware"
          >
            <CogIcon class="inline w-4 h-4 mr-2 sm:w-5 sm:h-5" />
            Settings
          </button>
        </div>
      </div>

      <!-- Main POS Interface -->
      <div class="grid grid-cols-1 gap-6 lg:grid-cols-3">
        <!-- Product Search and Selection -->
        <div class="space-y-4 lg:col-span-2">
          <!-- Search and Scanner -->
          <MaterialCard class="mb-4">
            <template #body>
              <div class="flex items-center space-x-3">
                <MaterialInput
                  v-model="searchQuery"
                  type="text"
                  label="Search products"
                  placeholder="Scan barcode or search products..."
                  icon="heroicons:magnifying-glass"
                  class="flex-1"
                  @keyup.enter="addFirstProductToCart"
                  ref="searchInput"
                />
                <MaterialButton color="primary" @click="showBarcodeScanner = true" icon="heroicons:qr-code" size="lg" />
              </div>
            </template>
          </MaterialCard>

          <!-- Category Filters -->
          <MaterialCard class="mb-4">
            <template #body>
              <div v-if="isLoadingCategories" class="flex items-center justify-center py-4">
                <MaterialButton loading color="primary">Loading categories...</MaterialButton>
              </div>
              <div v-else class="flex flex-wrap gap-2">
                <MaterialButton
                  v-for="category in categories"
                  :key="category.id"
                  :color="selectedCategory === category.id ? 'primary' : 'default'"
                  @click="selectedCategory = category.id"
                  size="sm"
                >
                  {{ category.name }}
                </MaterialButton>
              </div>
            </template>
          </MaterialCard>

          <!-- Products Grid -->
          <MaterialCard>
            <template #body>
              <div v-if="isLoading || isLoadingProducts" class="flex flex-col items-center justify-center py-20">
                <MaterialButton loading color="primary">Loading products...</MaterialButton>
              </div>
              <div v-else-if="hasError" class="flex flex-col items-center justify-center py-20">
                <MaterialButton color="danger" @click="loadData">Unable to Load Data. Retry</MaterialButton>
                <p class="max-w-md mb-4 text-center text-gray-600">{{ error }}</p>
              </div>
              <div v-else-if="filteredProducts.length === 0" class="flex flex-col items-center justify-center py-20">
                <UiBadge color="default" class="mb-4">No Products Found</UiBadge>
                <p class="text-sm text-gray-500">Try adjusting your search or filter</p>
              </div>
              <div v-else class="grid grid-cols-2 gap-4 sm:grid-cols-3 lg:grid-cols-4">
                <MaterialCard
                  v-for="product in filteredProducts"
                  :key="product.id"
                  :disabled="product.stock === 0"
                  class="p-3 text-left cursor-pointer"
                  @click="addToCart(product)"
                >
                  <template #body>
                    <div class="flex items-center justify-center mb-3 overflow-hidden bg-gray-100 rounded-lg aspect-square">
                      <img v-if="product.image" :src="product.image" :alt="product.name" class="object-cover w-full h-full" />
                      <CubeIcon v-else class="w-8 h-8 text-slate-400" />
                    </div>
                    <h3 class="mb-1 text-sm font-medium text-gray-900 truncate">{{ product.name }}</h3>
                    <p class="mb-2 text-xs text-gray-500 truncate">{{ product.sku }}</p>
                    <div class="flex items-center justify-between">
                      <span class="text-sm font-bold text-blue-600">R{{ product.price.toFixed(2) }}</span>
                      <UiBadge :color="product.stock > 10 ? 'success' : product.stock > 0 ? 'warning' : 'danger'" class="text-xs">Stock: {{ product.stock }}</UiBadge>
                    </div>
                  </template>
                </MaterialCard>
              </div>
            </template>
          </MaterialCard>
        </div>

        <!-- Cart and Checkout -->
        <MaterialCard>
          <template #body>
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-lg font-bold">Current Sale</h3>
              <MaterialButton color="danger" size="sm" @click="clearCart" :disabled="cartItems.length === 0">Clear All</MaterialButton>
            </div>
            <div v-if="cartItems.length === 0" class="py-8 text-center">
              <ShoppingCartIcon class="w-16 h-16 mx-auto mb-4 text-gray-300" />
              <p class="text-gray-500">No items in cart</p>
              <p class="text-sm text-gray-400">Scan or click products to add</p>
            </div>
            <div v-else class="mb-4 space-y-3">
              <MaterialDataTable :items="cartItems" :columns="cartColumns">
                <template #cell-quantity="{ item }">
                  <div class="flex items-center space-x-2">
                    <MaterialButton size="xs" @click="updateQuantity(item.id, item.quantity - 1)" :disabled="item.quantity <= 1">-</MaterialButton>
                    <span class="w-8 text-sm font-medium text-center">{{ item.quantity }}</span>
                    <MaterialButton size="xs" @click="updateQuantity(item.id, item.quantity + 1)" :disabled="item.quantity >= item.stock">+</MaterialButton>
                  </div>
                </template>
                <template #cell-actions="{ item }">
                  <MaterialButton color="danger" size="xs" @click="removeFromCart(item.id)"><TrashIcon class="w-4 h-4" /></MaterialButton>
                </template>
                <template #cell-discount="{ item }">
                  <MaterialButton color="warning" size="xs" @click="toggleDiscountMode(item.id)">Discount</MaterialButton>
                  <div v-if="item.showDiscount === 'percent'" class="flex items-center flex-1 space-x-1">
                    <MaterialInput v-model.number="item.discountPercent" type="number" min="0" max="100" step="0.01" placeholder="0" size="xs" />
                    <span class="text-xs">%</span>
                  </div>
                  <div v-if="item.showDiscount === 'amount'" class="flex items-center flex-1 space-x-1">
                    <span class="text-xs">R</span>
                    <MaterialInput v-model.number="item.discountAmount" type="number" min="0" step="0.01" placeholder="0.00" size="xs" />
                  </div>
                  <MaterialButton v-if="item.showDiscount" color="danger" size="xs" @click="clearDiscount(item.id)">✕</MaterialButton>
                </template>
                <template #cell-lineTotal="{ item }">
                  <div class="text-right">
                    <div v-if="item.discountPercent > 0 || item.discountAmount > 0" class="text-orange-600 line-through text-xxs">
                      R{{ (item.price * item.quantity).toFixed(2) }}
                    </div>
                    <div class="font-semibold">
                      R{{ calculateItemLineTotal(item).toFixed(2) }}
                    </div>
                  </div>
                </template>
              </MaterialDataTable>
            </div>
            <MaterialCard class="mb-6">
              <template #body>
                <div class="space-y-2">
                  <div class="flex items-center justify-between text-sm">
                    <span>Subtotal</span>
                    <span class="font-medium">R{{ formatCurrency(cartTotals.subtotal) }}</span>
                  </div>
                  <div v-if="cartTotals.discountTotal > 0" class="flex items-center justify-between text-sm">
                    <span class="text-warning">Discounts</span>
                    <span class="font-medium text-warning">-R{{ formatCurrency(cartTotals.discountTotal) }}</span>
                  </div>
                  <div v-if="cartTotals.taxTotal > 0" class="flex items-center justify-between text-sm">
                    <span>VAT (15%)</span>
                    <span class="font-medium">R{{ formatCurrency(cartTotals.taxTotal) }}</span>
                  </div>
                  <div class="flex items-center justify-between pt-3 mt-3 border-t">
                    <span class="text-sm font-bold">Total</span>
                    <span class="text-lg font-bold text-blue-600">R{{ formatCurrency(cartTotal) }}</span>
                  </div>
                  <div class="pt-1 text-xs text-center">
                    {{ cartItems.length }} {{ cartItems.length === 1 ? 'item' : 'items' }}
                  </div>
                </div>
              </template>
            </MaterialCard>
            <MaterialInput v-model="customerSearchQuery" label="Customer" placeholder="Walk-in Customer" class="mb-4" @focus="showCustomerDropdown = true" @blur="handleCustomerBlur" />
            <MaterialCard v-if="showCustomerDropdown" class="mb-4">
              <template #body>
                <MaterialButton block @click="selectCustomer(null)">Walk-in Customer</MaterialButton>
                <MaterialButton v-for="customer in filteredCustomers" :key="customer.id" block @click="selectCustomer(customer)">{{ customer.name }} <span class="text-xs text-gray-500">{{ customer.phone }}</span></MaterialButton>
              </template>
            </MaterialCard>
            <div class="mb-6">
              <label class="block mb-3 text-sm font-medium">Payment Method</label>
              <div class="grid grid-cols-2 gap-2">
                <MaterialButton v-for="method in paymentMethods" :key="method.id" block :color="selectedPaymentMethod === method.id ? 'primary' : 'default'" @click="selectedPaymentMethod = method.id">{{ method.name }}</MaterialButton>
              </div>
            </div>
            <div class="mb-6 space-y-3">
              <MaterialButton block color="primary" size="lg" @click="showCreateOrderModal = true" :disabled="cartItems.length === 0">📝 Create Order (Queue) - R{{ formatCurrency(cartTotal) }}</MaterialButton>
              <MaterialButton block color="success" size="lg" @click="processPayment" :disabled="cartItems.length === 0">💰 Process Payment - R{{ formatCurrency(cartTotal) }}</MaterialButton>
            </div>
            <MaterialCard class="mb-4">
              <template #body>
                <MaterialButton block color="warning" size="md" @click="showHoldSaleModal = true" :disabled="cartItems.length === 0">⏸️ Hold Sale</MaterialButton>
                <MaterialButton block color="danger" size="md" @click="showVoidSaleModal = true" :disabled="cartItems.length === 0">❌ Void Sale</MaterialButton>
                <MaterialButton v-if="heldSales.length > 0" block color="secondary" size="md" @click="showHeldSalesModal = true">📋 Held Sales ({{ heldSales.length }})</MaterialButton>
              </template>
            </MaterialCard>
          </template>
        </MaterialCard>
      </div>
    </div>

    <!-- Payment Success Modal -->
    <div v-if="showSuccessModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
      <div class="w-full max-w-md p-6 bg-white rounded-xl">
      <div class="text-center">
          <div class="flex items-center justify-center w-16 h-16 mx-auto mb-4 bg-green-100 rounded-full">
            <CheckIcon class="w-8 h-8 text-green-600" />
        </div>
          <h3 class="mb-2 text-xl font-semibold text-gray-900">Payment Successful!</h3>
          <p class="mb-6 text-gray-600">Transaction completed successfully</p>
          <div class="flex space-x-3">
            <button 
              @click="printReceipt"
              class="flex-1 py-2 font-medium text-white transition-colors bg-blue-600 rounded-lg hover:bg-blue-700"
            >
              Print Receipt
            </button>
            <button 
              @click="emailReceipt"
              class="flex-1 py-2 font-medium text-white transition-colors bg-gray-600 rounded-lg hover:bg-gray-700"
            >
              Email Receipt
            </button>
        </div>
      </div>
    </div>

    <!-- Comprehensive Sales Report Modal -->
    <MaterialModal v-if="showSuccessModal" @close="closeSuccessModal" size="md">
      <template #header>
        <div class="flex items-center justify-center w-16 h-16 mx-auto mb-4 bg-green-100 rounded-full">
          <CheckIcon class="w-8 h-8 text-green-600" />
        </div>
      </template>
      <template #body>
        <div class="text-center">
          <h3 class="mb-2 text-xl font-semibold">Payment Successful!</h3>
          <p class="mb-6">Transaction completed successfully</p>
          <div class="flex space-x-3">
            <MaterialButton color="primary" @click="printReceipt">Print Receipt</MaterialButton>
            <MaterialButton color="secondary" @click="emailReceipt">Email Receipt</MaterialButton>
            <MaterialButton color="default" @click="closeSuccessModal">Close</MaterialButton>
          </div>
        </div>
      </template>
    </MaterialModal>
        <div class="mb-6">
          <h4 class="mb-3 text-lg font-semibold text-gray-900">Payment Methods</h4>
          <div class="grid grid-cols-2 gap-3 md:grid-cols-4">
            <div class="p-3 rounded-lg bg-gray-50">
              <p class="text-xs text-gray-600">Cash</p>
              <p class="text-lg font-bold text-gray-900">R12,350.00</p>
              <p class="text-xs text-gray-500">25 sales</p>
            </div>
            <div class="p-3 rounded-lg bg-gray-50">
              <p class="text-xs text-gray-600">Card</p>
              <p class="text-lg font-bold text-gray-900">R5,246.00</p>
              <p class="text-xs text-gray-500">18 sales</p>
            </div>
            <div class="p-3 rounded-lg bg-gray-50">
              <p class="text-xs text-gray-600">EFT</p>
              <p class="text-lg font-bold text-gray-900">R750.00</p>
              <p class="text-xs text-gray-500">3 sales</p>
            </div>
            <div class="p-3 rounded-lg bg-gray-50">
              <p class="text-xs text-gray-600">Account</p>
              <p class="text-lg font-bold text-gray-900">R150.00</p>
              <p class="text-xs text-gray-500">2 sales</p>
            </div>
          </div>
        </div>

        <!-- Held Sales Section -->
        <div class="pt-6 mb-6 border-t">
          <div class="flex items-center justify-between mb-3">
            <h4 class="text-lg font-semibold text-gray-900">
              Held Sales ({{ heldSales.length }})
            </h4>
            <span v-if="heldSales.length > 0" class="text-sm font-medium text-orange-600">
              Total: R{{ formatCurrency(heldSales.reduce((sum, sale) => sum + sale.total, 0)) }}
            </span>
          </div>
          <div v-if="heldSales.length === 0" class="p-4 text-center text-gray-500 rounded-lg bg-gray-50">
            No held sales today
          </div>
          <div v-else class="space-y-2 overflow-y-auto max-h-48">
            <div 
              v-for="sale in heldSales" 
              :key="sale.id"
              class="flex items-center justify-between p-3 transition-colors rounded-lg bg-orange-50 hover:bg-orange-100"
            >
              <div class="flex-1">
                <div class="flex items-center gap-2">
                  <span class="font-medium text-gray-900">{{ sale.saleNumber }}</span>
                  <span class="text-sm text-gray-600">• {{ sale.customer }}</span>
                </div>
                <div class="mt-1 text-xs text-gray-500">
                  {{ sale.items.length }} items • {{ sale.timestamp }}
                </div>
                <div v-if="sale.note" class="mt-1 text-xs italic text-gray-600">
                  "{{ sale.note }}"
                </div>
              </div>
              <div class="text-right">
                <div class="font-bold text-orange-600">R{{ formatCurrency(sale.total) }}</div>
                <div class="text-xs text-gray-500">{{ sale.paymentMethod }}</div>
              </div>
            </div>
          </div>
        </div>

        <!-- Voided Sales Section -->
        <div class="pt-6 mb-6 border-t">
          <div class="flex items-center justify-between mb-3">
            <h4 class="text-lg font-semibold text-gray-900">
              Voided Sales ({{ voidedSales.length }})
            </h4>
            <span v-if="voidedSales.length > 0" class="text-sm font-medium text-red-600">
              Total: R{{ formatCurrency(voidedSales.reduce((sum, sale) => sum + sale.total, 0)) }}
            </span>
          </div>
          <div v-if="voidedSales.length === 0" class="p-4 text-center text-gray-500 rounded-lg bg-gray-50">
            No voided sales today
          </div>
          <div v-else class="space-y-2 overflow-y-auto max-h-48">
            <div 
              v-for="sale in voidedSales" 
              :key="sale.id"
              class="flex items-center justify-between p-3 rounded-lg bg-red-50"
            >
              <div class="flex-1">
                <div class="flex items-center gap-2">
                  <span class="font-medium text-gray-900">{{ sale.saleNumber }}</span>
                  <span class="text-sm text-gray-600">• {{ sale.customer }}</span>
                </div>
                <div class="mt-1 text-xs text-gray-500">
                  {{ sale.items.length }} items • {{ sale.timestamp }}
                </div>
                <div v-if="sale.reason" class="mt-1 text-xs italic text-red-600">
                  Reason: "{{ sale.reason }}"
                </div>
              </div>
              <div class="text-right">
                <div class="font-bold text-red-600">R{{ formatCurrency(sale.total) }}</div>
                <div class="text-xs text-gray-500 line-through">{{ sale.paymentMethod }}</div>
              </div>
            </div>
          </div>
        </div>

        <div class="flex justify-end gap-3 pt-4 border-t">
          <button @click="printReport" class="px-4 py-2 font-medium text-white transition-colors bg-blue-600 rounded-lg hover:bg-blue-700">
            Print Report
          </button>
          <button @click="showReports = false" class="px-4 py-2 font-medium text-white transition-colors bg-gray-600 rounded-lg hover:bg-gray-700">
            Close
          </button>
        </div>
      </div>
    </div>

    <!-- Hold Sale Modal -->
    <div v-if="showHoldSaleModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
      <div class="w-full max-w-md p-6 bg-white rounded-xl">
        <h3 class="mb-4 text-xl font-semibold text-gray-900">Hold Sale</h3>
        
        <!-- Sale Summary -->
        <div class="p-3 mb-4 rounded-lg bg-blue-50">
          <div class="flex items-center justify-between mb-2">
            <span class="text-sm text-gray-600">Customer:</span>
            <span class="font-medium text-gray-900">{{ selectedCustomerName }}</span>
          </div>
          <div class="flex items-center justify-between">
            <span class="text-sm text-gray-600">Total Amount:</span>
            <span class="font-bold text-blue-600">R{{ formatCurrency(cartTotal) }}</span>
          </div>
          <div class="mt-1 text-sm text-gray-600">
            {{ cartItems.length }} items in cart
          </div>
        </div>
        
        <p class="mb-2 text-gray-600">Enter a note for this held sale:</p>
        <input 
          v-model="holdSaleNote"
          type="text"
          placeholder="e.g., Customer will return later"
          class="w-full px-3 py-2 mb-4 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
        />
        <div class="flex space-x-3">
          <button 
            @click="confirmHoldSale"
            class="flex-1 py-2 font-medium text-white transition-colors bg-orange-600 rounded-lg hover:bg-orange-700"
          >
            Hold Sale
          </button>
          <button 
            @click="showHoldSaleModal = false; holdSaleNote = ''"
            class="flex-1 py-2 font-medium text-white transition-colors bg-gray-600 rounded-lg hover:bg-gray-700"
          >
            Cancel
          </button>
        </div>
      </div>
    </div>

    <!-- Void Sale Modal -->
    <div v-if="showVoidSaleModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
      <div class="w-full max-w-md p-6 bg-white rounded-xl">
        <h3 class="mb-4 text-xl font-semibold text-gray-900">Void Sale</h3>
        <p class="mb-2 text-gray-600">Please provide a reason for voiding this sale:</p>
        <input 
          v-model="voidSaleReason"
          type="text"
          placeholder="e.g., Customer changed their mind"
          class="w-full px-3 py-2 mb-4 border border-gray-300 rounded-lg focus:ring-2 focus:ring-red-500"
        />
        <div class="flex space-x-3">
          <button 
            @click="confirmVoidSale"
            class="flex-1 py-2 font-medium text-white transition-colors bg-red-600 rounded-lg hover:bg-red-700"
          >
            Void Sale
          </button>
          <button 
            @click="showVoidSaleModal = false; voidSaleReason = ''"
            class="flex-1 py-2 font-medium text-white transition-colors bg-gray-600 rounded-lg hover:bg-gray-700"
          >
            Cancel
          </button>
        </div>
      </div>
    </div>

    <!-- Create Order Modal (Queue-based) -->
    <div v-if="showCreateOrderModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
      <div class="w-full max-w-md p-6 bg-white rounded-xl">
        <h3 class="mb-4 text-xl font-semibold text-gray-900">📝 Create Queue Order</h3>
        
        <!-- Order Summary -->
        <div class="p-3 mb-4 rounded-lg bg-blue-50">
          <div class="flex items-center justify-between">
            <span class="text-sm text-gray-600">Total Amount:</span>
            <span class="font-bold text-blue-600">R{{ formatCurrency(cartTotal) }}</span>
          </div>
          <div class="mt-1 text-sm text-gray-600">
            {{ cartItems.length }} items in order
          </div>
        </div>
        
        <!-- Customer Name -->
        <div class="mb-3">
          <label class="block mb-1 text-sm font-medium text-gray-700">Customer Name</label>
          <input 
            v-model="orderCustomerName"
            type="text"
            placeholder="e.g., Thabo Nkosi"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
            required
          />
        </div>

        <!-- Customer Phone -->
        <div class="mb-3">
          <label class="block mb-1 text-sm font-medium text-gray-700">Phone Number (optional)</label>
          <input 
            v-model="orderCustomerPhone"
            type="tel"
            placeholder="e.g., 076 123 4567"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
          />
        </div>

        <!-- Special Instructions -->
        <div class="mb-3">
          <label class="block mb-1 text-sm font-medium text-gray-700">Special Instructions (optional)</label>
          <textarea 
            v-model="orderCustomerNotes"
            placeholder="e.g., Extra peri-peri sauce, no onions"
            rows="2"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
          ></textarea>
        </div>

        <!-- Estimated Preparation Time -->
        <div class="mb-4">
          <label class="block mb-1 text-sm font-medium text-gray-700">
            Estimated Prep Time: {{ orderEstimatedMinutes }} minutes
          </label>
          <input 
            v-model.number="orderEstimatedMinutes"
            type="range"
            min="5"
            max="60"
            step="5"
            class="w-full"
          />
          <div class="flex justify-between mt-1 text-xs text-gray-500">
            <span>5 min</span>
            <span>60 min</span>
          </div>
        </div>
        
        <!-- Action Buttons -->
        <div class="flex space-x-3">
          <button 
            @click="confirmCreateOrder"
            :disabled="!orderCustomerName"
            class="flex-1 py-2 font-medium text-white transition-colors bg-blue-600 rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Create Order
          </button>
          <button 
            @click="showCreateOrderModal = false; orderCustomerName = ''; orderCustomerPhone = ''; orderCustomerNotes = ''; orderEstimatedMinutes = 15"
            class="flex-1 py-2 font-medium text-white transition-colors bg-gray-600 rounded-lg hover:bg-gray-700"
          >
            Cancel
          </button>
        </div>
      </div>
    </div>
    <div v-if="showVoidSaleModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
      <div class="w-full max-w-md p-6 bg-white rounded-xl">
        <h3 class="mb-4 text-xl font-semibold text-gray-900">Void Sale</h3>
        <p class="mb-4 text-gray-600">Enter a reason for voiding this sale:</p>
        <textarea 
          v-model="voidSaleReason"
          placeholder="e.g., Customer changed mind"
          rows="3"
          class="w-full px-3 py-2 mb-4 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
        ></textarea>
        <div class="flex space-x-3">
          <button 
            @click="confirmVoidSale"
            class="flex-1 py-2 font-medium text-white transition-colors bg-red-600 rounded-lg hover:bg-red-700"
          >
            Void Sale
          </button>
          <button 
            @click="showVoidSaleModal = false; voidSaleReason = ''"
            class="flex-1 py-2 font-medium text-white transition-colors bg-gray-600 rounded-lg hover:bg-gray-700"
          >
            Cancel
          </button>
        </div>
      </div>
    </div>

    <!-- Held Sales Modal -->
    <div v-if="showHeldSalesModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
      <div class="bg-white rounded-xl p-6 max-w-2xl w-full max-h-[80vh] flex flex-col">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-xl font-semibold text-gray-900">Held Sales ({{ heldSales.length }})</h3>
          <button @click="showHeldSalesModal = false; heldSalesSearchQuery = ''" class="text-gray-500 hover:text-gray-700">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </button>
        </div>
        
        <!-- Search Field -->
        <div class="mb-4">
          <div class="relative">
            <input
              v-model="heldSalesSearchQuery"
              type="text"
              placeholder="Search by customer, note, or amount..."
              class="w-full px-3 py-2 pl-10 text-gray-900 bg-white border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
            />
            <svg class="absolute w-5 h-5 text-gray-400 transform -translate-y-1/2 left-3 top-1/2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"></path>
            </svg>
          </div>
          <p v-if="filteredHeldSales.length !== heldSales.length" class="mt-1 text-xs text-gray-500">
            Showing {{ filteredHeldSales.length }} of {{ heldSales.length }} held sales
          </p>
        </div>
        
        <!-- Held Sales List -->
        <div class="flex-1 space-y-3 overflow-y-auto">
          <div v-if="filteredHeldSales.length === 0" class="py-8 text-center text-gray-500">
            <p>No held sales found</p>
            <p class="mt-1 text-sm">Try adjusting your search</p>
          </div>
          <div 
            v-for="(sale, index) in filteredHeldSales" 
            :key="index"
            class="p-4 transition-colors border border-gray-200 rounded-lg hover:border-blue-500"
          >
            <div class="flex items-start justify-between mb-2">
              <div class="flex-1">
                <div class="flex items-center gap-2 mb-1">
                  <p class="font-medium text-gray-900">R{{ formatCurrency(sale.total) }}</p>
                  <span class="text-xs px-2 py-0.5 bg-blue-100 text-blue-700 rounded-full">{{ sale.customer }}</span>
                </div>
                <p class="text-sm text-gray-500">{{ sale.items.length }} items • {{ sale.timestamp }}</p>
                <p v-if="sale.note" class="mt-1 text-sm text-gray-600">📝 {{ sale.note }}</p>
              </div>
              <div class="flex gap-2 ml-4">
                <button 
                  @click="retrieveHeldSale(getOriginalIndex(sale))"
                  class="px-3 py-1 text-sm text-white bg-blue-600 rounded hover:bg-blue-700 whitespace-nowrap"
                >
                  Retrieve
                </button>
                <button 
                  @click="deleteHeldSale(getOriginalIndex(sale))"
                  class="px-3 py-1 text-sm text-white bg-red-600 rounded hover:bg-red-700 whitespace-nowrap"
                >
                  Delete
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <!-- Barcode Scanner Component -->
    <BarcodeScanner 
      v-model="showBarcodeScanner" 
      @barcode-scanned="handleBarcodeScanned"
    />
    <!-- Settings Modal -->
    <div v-if="showSettingsModal" class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black bg-opacity-50">
      <div class="w-full max-w-lg p-6 bg-white rounded-xl">
        <div class="flex items-center justify-between mb-6">
          <h3 class="text-2xl font-semibold text-gray-900">Settings & Hardware</h3>
          <button @click="showSettingsModal = false" class="text-gray-500 hover:text-gray-700">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </button>
        </div>
        <!-- Hardware Status Section -->
        <div class="mb-6">
          <h4 class="mb-4 text-lg font-semibold text-gray-900">Hardware Status</h4>
          <div class="space-y-3">
            <div class="flex items-center justify-between p-3 rounded-lg bg-gray-50">
              <div class="flex items-center gap-3">
                <div class="flex items-center justify-center w-10 h-10 bg-blue-100 rounded-lg">
                  <QrCodeIcon class="w-6 h-6 text-blue-600" />
                </div>
                <div>
                  <p class="font-medium text-gray-900">Barcode Scanner</p>
                  <p class="text-xs text-gray-500">Keyboard wedge / USB scanner</p>
                </div>
              </div>
              <div class="flex items-center gap-2">
                <div class="w-3 h-3 rounded-full" :class="hardwareStatus.barcodeScanner ? 'bg-green-500' : 'bg-red-500'"></div>
                <span class="text-sm font-medium" :class="hardwareStatus.barcodeScanner ? 'text-green-600' : 'text-red-600'">
                  {{ hardwareStatus.barcodeScanner ? 'Connected' : 'Disconnected' }}
                </span>
              </div>
            </div>
            <div class="flex items-center justify-between p-3 rounded-lg bg-gray-50">
              <div class="flex items-center gap-3">
                <div class="flex items-center justify-center w-10 h-10 bg-purple-100 rounded-lg">
                  <CreditCardIcon class="w-6 h-6 text-purple-600" />
                </div>
                <div>
                  <p class="font-medium text-gray-900">Card Reader</p>
                  <p class="text-xs text-gray-500">USB / HID card terminal</p>
                </div>
              </div>
              <div class="flex items-center gap-2">
                <div class="w-3 h-3 rounded-full" :class="hardwareStatus.cardReader ? 'bg-green-500' : 'bg-red-500'"></div>
                <span class="text-sm font-medium" :class="hardwareStatus.cardReader ? 'text-green-600' : 'text-red-600'">
                  {{ hardwareStatus.cardReader ? 'Connected' : 'Disconnected' }}
                </span>
              </div>
            </div>
            <div class="flex items-center justify-between p-3 rounded-lg bg-gray-50">
              <div class="flex items-center gap-3">
                <div class="flex items-center justify-center w-10 h-10 bg-orange-100 rounded-lg">
                  <PrinterIcon class="w-6 h-6 text-orange-600" />
                </div>
                <div>
                  <p class="font-medium text-gray-900">Receipt Printer</p>
                  <p class="text-xs text-gray-500">ESC/POS thermal printer</p>
                </div>
              </div>
              <div class="flex items-center gap-2">
                <div class="w-3 h-3 rounded-full" :class="hardwareStatus.printer ? 'bg-green-500' : 'bg-red-500'"></div>
                <span class="text-sm font-medium" :class="hardwareStatus.printer ? 'text-green-600' : 'text-red-600'">
                  {{ hardwareStatus.printer ? 'Connected' : 'Disconnected' }}
                </span>
              </div>
            </div>
            <div class="flex items-center justify-between p-3 rounded-lg bg-gray-50">
              <div class="flex items-center gap-3">
                <div class="flex items-center justify-center w-10 h-10 bg-green-100 rounded-lg">
                  <CurrencyDollarIcon class="w-6 h-6 text-green-600" />
                </div>
                <div>
                  <p class="font-medium text-gray-900">Cash Drawer</p>
                  <p class="text-xs text-gray-500">Connected via printer</p>
                </div>
              </div>
              <div class="flex items-center gap-2">
                <div class="w-3 h-3 rounded-full" :class="hardwareStatus.cashDrawer ? 'bg-green-500' : 'bg-red-500'"></div>
                <span class="text-sm font-medium" :class="hardwareStatus.cashDrawer ? 'text-green-600' : 'text-red-600'">
                  {{ hardwareStatus.cashDrawer ? 'Connected' : 'Disconnected' }}
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- Quick Actions Section -->
        <div class="pt-4 mb-6 border-t">
          <h4 class="mb-4 text-lg font-semibold text-gray-900">Quick Actions</h4>
          <div class="grid grid-cols-2 gap-3">
            <button 
              @click="requestHardwareAccess"
              class="flex items-center justify-center gap-2 px-4 py-3 font-medium text-white transition-colors bg-blue-600 rounded-lg hover:bg-blue-700"
            >
              <CogIcon class="w-5 h-5" />
              Connect Hardware
            </button>
            <button 
              @click="openDrawer"
              class="flex items-center justify-center gap-2 px-4 py-3 font-medium text-white transition-colors bg-green-600 rounded-lg hover:bg-green-700"
            >
              <CurrencyDollarIcon class="w-5 h-5" />
              Open Drawer
            </button>
            <button 
              @click="openReportsModal"
              class="flex items-center justify-center gap-2 px-4 py-3 font-medium text-white transition-colors bg-purple-600 rounded-lg hover:bg-purple-700"
            >
              <ChartBarIcon class="w-5 h-5" />
              Reports
            </button>
            <button 
              @click="testPrinter"
              class="flex items-center justify-center gap-2 px-4 py-3 font-medium text-white transition-colors bg-orange-600 rounded-lg hover:bg-orange-700"
            >
              <PrinterIcon class="w-5 h-5" />
              Test Print
            </button>
          </div>
        </div>

        <!-- Close Button -->
        <div class="flex justify-end pt-4 border-t">
          <button 
            @click="showSettingsModal = false" 
            class="px-6 py-2 font-medium text-white transition-colors bg-gray-600 rounded-lg hover:bg-gray-700"
          >
            Close
          </button>
        </div>
      </div>
    </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import {
  MagnifyingGlassIcon,
  QrCodeIcon,
  CurrencyDollarIcon,
  ChartBarIcon,
  BanknotesIcon,
  ShoppingCartIcon,
  CubeIcon,
  PlusIcon,
  MinusIcon,
  TrashIcon,
  CheckIcon,
  CreditCardIcon,
  PrinterIcon,
  CogIcon,
  ArrowsPointingOutIcon,
  ArrowsPointingInIcon,
  ChevronUpIcon,
  ChevronDownIcon
} from '@heroicons/vue/24/outline'
import { Disclosure, DisclosureButton, DisclosurePanel } from '@headlessui/vue'
import BarcodeScanner from '~/components/pos/BarcodeScanner.vue'
import { useSalesAPI } from '~/composables/useSalesAPI'
import * as Sentry from '@sentry/nuxt'

// API
const salesAPI = useSalesAPI()

// Cart math and offline queue composables
const cartMath = {
  calculateLineSubtotal: (line: any) => Math.max(0, line.quantity * line.unitPrice),
  calculateLineDiscount: (line: any) => {
    const subtotal = Math.max(0, line.quantity * line.unitPrice)
    if (line.discountAmount) return Math.min(line.discountAmount, subtotal)
    if (line.discountPercent) return (subtotal * Math.min(line.discountPercent, 100)) / 100
    return 0
  },
  calculateLineTax: (line: any) => {
    if (!line.taxRate) return 0
    const subtotal = Math.max(0, line.quantity * line.unitPrice)
    const discount = line.discountAmount ? Math.min(line.discountAmount, subtotal) : 
                     line.discountPercent ? (subtotal * Math.min(line.discountPercent, 100)) / 100 : 0
    return ((subtotal - discount) * line.taxRate) / 100
  },
  calculateLineTotal: (line: any) => {
    const subtotal = Math.max(0, line.quantity * line.unitPrice)
    const discount = line.discountAmount ? Math.min(line.discountAmount, subtotal) : 
                     line.discountPercent ? (subtotal * Math.min(line.discountPercent, 100)) / 100 : 0
    const taxableAmount = subtotal - discount
    const tax = line.taxRate ? (taxableAmount * line.taxRate) / 100 : 0
    return Math.round((taxableAmount + tax) * 100) / 100
  },
  calculateCartTotals: (lines: any[]) => {
    const subtotal = lines.reduce((sum, line) => sum + Math.max(0, line.quantity * line.unitPrice), 0)
    const discountTotal = lines.reduce((sum, line) => {
      const lineSubtotal = Math.max(0, line.quantity * line.unitPrice)
      const discount = line.discountAmount ? Math.min(line.discountAmount, lineSubtotal) : 
                       line.discountPercent ? (lineSubtotal * Math.min(line.discountPercent, 100)) / 100 : 0
      return sum + discount
    }, 0)
    const taxTotal = lines.reduce((sum, line) => {
      if (!line.taxRate) return sum
      const lineSubtotal = Math.max(0, line.quantity * line.unitPrice)
      const discount = line.discountAmount ? Math.min(line.discountAmount, lineSubtotal) : 
                       line.discountPercent ? (lineSubtotal * Math.min(line.discountPercent, 100)) / 100 : 0
      return sum + ((lineSubtotal - discount) * line.taxRate) / 100
    }, 0)
    return {
      subtotal: Math.round(subtotal * 100) / 100,
      discountTotal: Math.round(discountTotal * 100) / 100,
      taxTotal: Math.round(taxTotal * 100) / 100,
      grandTotal: Math.round((subtotal - discountTotal + taxTotal) * 100) / 100
    }
  }
}

// Offline Queue (IndexedDB-backed)
const offlineQueue = {
  dbName: 'toss-offline-queue',
  storeName: 'transactions',
  
  async openDB() {
    return new Promise<IDBDatabase>((resolve, reject) => {
      const request = indexedDB.open(this.dbName, 1)
      request.onerror = () => reject(request.error)
      request.onsuccess = () => resolve(request.result)
      request.onupgradeneeded = (event: any) => {
        const db = event.target.result
        if (!db.objectStoreNames.contains(this.storeName)) {
          const store = db.createObjectStore(this.storeName, { keyPath: 'id', autoIncrement: true })
          store.createIndex('timestamp', 'timestamp', { unique: false })
          store.createIndex('status', 'status', { unique: false })
        }
      }
    })
  },
  
  async enqueue(transaction: any) {
    const db = await this.openDB()
    return new Promise((resolve, reject) => {
      const tx = db.transaction(this.storeName, 'readwrite')
      const store = tx.objectStore(this.storeName)
      const request = store.add({
        ...transaction,
        timestamp: Date.now(),
        status: 'pending',
        retryCount: 0
      })
      request.onsuccess = () => resolve(request.result)
      request.onerror = () => reject(request.error)
    })
  },
  
  async getPending() {
    const db = await this.openDB()
    return new Promise<any[]>((resolve, reject) => {
      const tx = db.transaction(this.storeName, 'readonly')
      const store = tx.objectStore(this.storeName)
      const index = store.index('status')
      const request = index.getAll('pending')
      request.onsuccess = () => resolve(request.result || [])
      request.onerror = () => reject(request.error)
    })
  },
  
  async remove(id: number) {
    const db = await this.openDB()
    return new Promise<void>((resolve, reject) => {
      const tx = db.transaction(this.storeName, 'readwrite')
      const store = tx.objectStore(this.storeName)
      const request = store.delete(id)
      request.onsuccess = () => resolve()
      request.onerror = () => reject(request.error)
    })
  },
  
  async updateStatus(id: number, status: string) {
    const db = await this.openDB()
    return new Promise<void>((resolve, reject) => {
      const tx = db.transaction(this.storeName, 'readwrite')
      const store = tx.objectStore(this.storeName)
      const getRequest = store.get(id)
      getRequest.onsuccess = () => {
        const record = getRequest.result
        if (record) {
          record.status = status
          record.retryCount = (record.retryCount || 0) + 1
          const putRequest = store.put(record)
          putRequest.onsuccess = () => resolve()
          putRequest.onerror = () => reject(putRequest.error)
        } else {
          resolve()
        }
      }
      getRequest.onerror = () => reject(getRequest.error)
    })
  }
}

// Reactive state for queue status
const queuePendingCount = ref(0)

// Process offline queue
const processOfflineQueue = async () => {
  try {
    const pending = await offlineQueue.getPending()
    let successCount = 0
    let failCount = 0
    
    for (const item of pending) {
      try {
        // Attempt to sync
        await salesAPI.createSale(item.data)
        await offlineQueue.remove(item.id)
        successCount++
      } catch (error) {
        // Max 3 retries
        if (item.retryCount >= 2) {
          await offlineQueue.updateStatus(item.id, 'failed')
          failCount++
        } else {
          await offlineQueue.updateStatus(item.id, 'pending')
        }
      }
    }
    
    // Update queue count
    const remainingPending = await offlineQueue.getPending()
    queuePendingCount.value = remainingPending.length
    
    if (successCount > 0 || failCount > 0) {
      showNotification(`Synced ${successCount} transactions${failCount > 0 ? `, ${failCount} failed` : ''}`)
    }
  } catch (error) {
    console.error('Queue processing error:', error)
  }
}

// Auto-sync setup
let autoSyncInterval: ReturnType<typeof setInterval> | null = null
const setupAutoSync = () => {
  if (autoSyncInterval) clearInterval(autoSyncInterval)
  autoSyncInterval = setInterval(async () => {
    if (navigator.onLine) {
      await processOfflineQueue()
    }
  }, 30000) // Every 30 seconds
}

// Hardware status tracking
const hardwareStatus = ref({
  barcodeScanner: false,
  cardReader: false,
  printer: false,
  cashDrawer: false
})

// Barcode scanning state
let barcodeBuffer = ''
let barcodeTimeout: ReturnType<typeof setTimeout> | null = null

// Reactive data
const searchQuery = ref('')
const selectedCategory = ref('all')
const scannerActive = ref(false)
const showBarcodeScanner = ref(false)
const cartItems = ref<any[]>([])
const selectedCustomer = ref('')
const selectedCustomerName = ref('Walk-in Customer')
const selectedPaymentMethod = ref('Cash')
const showSuccessModal = ref(false)
const showReports = ref(false)
const showSettingsModal = ref(false)
const searchInput = ref<HTMLInputElement>()
const isFullscreen = ref(false)
const products = ref<any[]>([])
const customers = ref<any[]>([])

// Customer search
const customerSearchQuery = ref('Walk-in Customer')
const showCustomerDropdown = ref(false)

// Hold/Void Sale
const showHoldSaleModal = ref(false)
const showVoidSaleModal = ref(false)
const showHeldSalesModal = ref(false)
const holdSaleNote = ref('')
const voidSaleReason = ref('')
const heldSales = ref<any[]>([])
const voidedSales = ref<any[]>([])
const heldSalesSearchQuery = ref('')

// Create Order (Queue-based)
const showCreateOrderModal = ref(false)
const orderCustomerName = ref('')
const orderCustomerPhone = ref('')
const orderCustomerNotes = ref('')
const orderEstimatedMinutes = ref(15)

// POS Stats
const todaySales = ref(18496)
const todayTransactions = ref(48)
const averageSale = ref(285)
const cashFloat = ref(2500)

// Categories - will be loaded from API
const categories = ref<any[]>([
  { id: 'all', name: 'All', productCount: 0 }
])

// Shop ID - get from session or default to 1
const shopId = ref(1)

// Loading and error states
const isLoading = ref(true)
const isLoadingProducts = ref(false)
const isLoadingCategories = ref(false)
const isLoadingCustomers = ref(false)
const error = ref<string | null>(null)
const hasError = ref(false)

// Load data on mount
const loadData = async () => {
  isLoading.value = true
  error.value = null
  hasError.value = false
  
  try {
    // Get categories from backend API
    isLoadingCategories.value = true
    console.log('🔍 Fetching categories for shopId:', shopId.value)
    const categoriesResponse = await salesAPI.getCategories(shopId.value)
    console.log('📁 Categories response:', categoriesResponse)
    isLoadingCategories.value = false
    
    // Get products from backend API
    isLoadingProducts.value = true
    console.log('🔍 Fetching products for shopId:', shopId.value)
    const productsResponse = await salesAPI.getProducts(shopId.value)
    console.log('📦 Products response received:', productsResponse)
    console.log('📦 Products response type:', Array.isArray(productsResponse) ? 'Array' : typeof productsResponse)
    console.log('📦 Products response length:', productsResponse?.length || 0)
    isLoadingProducts.value = false
    
    // Transform backend response to POS format
    // API returns PascalCase properties, handle both PascalCase and camelCase
    products.value = (productsResponse || []).map((p: any) => ({
      id: p.id || p.Id || 0,
      name: p.name || p.Name || 'Unknown Product',
      sku: p.sku || p.SKU || 'NO-SKU',
      price: Number(p.basePrice || p.BasePrice) || 0,
      categoryId: p.categoryId || p.CategoryId || 0,
      category: p.categoryName || p.CategoryName || 'Unknown',
      stock: Number(p.availableStock || p.AvailableStock) || 0,
      image: p.imageUrl || p.ImageUrl || null,
      barcode: p.barcode || p.Barcode || p.SKU || p.sku || 'NO-BARCODE'
    }))
    console.log('✅ Transformed products:', products.value.length, 'items')
    console.log('📝 Sample product:', products.value[0])
    
    // Count products per category
    const productCountByCategory = products.value.reduce((acc: any, product: any) => {
      if (product.categoryId) {
        acc[product.categoryId] = (acc[product.categoryId] || 0) + 1
      }
      return acc
    }, {})
    
    // Filter categories to only show those with products
    const categoriesWithProducts = (categoriesResponse || []).filter((cat: any) =>      
      productCountByCategory[cat.id] > 0
    )
    
    // Add "All" category at the beginning with total product count
    categories.value = [
      { id: 'all', name: 'All', productCount: products.value.length },
      ...categoriesWithProducts
    ]
    
    // Get customers from backend API
    isLoadingCustomers.value = true
    const customersResponse = await salesAPI.getCustomers(shopId.value)
    isLoadingCustomers.value = false
    
    // Handle paginated response
    const customersList = Array.isArray(customersResponse) 
      ? customersResponse 
      : customersResponse.items || []
      
    customers.value = customersList.map((c: any) => ({
      id: c.id,
      name: c.fullName || `${c.firstName} ${c.lastName}`.trim(),
      phone: c.phoneNumber || '',
      email: c.email || ''
    }))
    
    // Load held sales from database
    await loadHeldSales()
    
    console.log(`✅ Loaded ${categories.value.length - 1} categories (with products), ${products.value.length} products, ${customers.value.length} customers, and ${heldSales.value.length} held sales from API`)
    
    isLoading.value = false
  } catch (err: any) {
    console.error('❌ Failed to load POS data:', err)
    console.error('❌ Error details:', err?.response || err?.message || err)
    
    // Set error state
    hasError.value = true
    error.value = err.message || 'Failed to connect to server. Please check your connection and try again.'
    isLoading.value = false
    isLoadingProducts.value = false
    isLoadingCategories.value = false
    isLoadingCustomers.value = false
    
    // Show user-friendly error notification
    showNotification('⚠️ Failed to load data from server. Please refresh the page.', 'error')
  }
}

// Payment methods
const paymentMethods = ref([
  { id: 'Cash', name: 'Cash' },
  { id: 'Card', name: 'Card' },
  { id: 'MobileMoney', name: 'Mobile Money' },
  { id: 'BankTransfer', name: 'Bank Transfer' },
  { id: 'PayLink', name: 'Pay Link' }
])

// Computed properties
const filteredProducts = computed(() => {
  console.log('🔄 Computing filteredProducts...')
  console.log('   - Total products:', products.value.length)
  console.log('   - Selected category:', selectedCategory.value)
  console.log('   - Search query:', searchQuery.value)
  
  let filtered = products.value

  if (selectedCategory.value !== 'all') {
    // Filter by categoryId (numeric) from API
    console.log('   - Filtering by category...')
    filtered = filtered.filter((p: any) => p.categoryId === selectedCategory.value)
    console.log('   - After category filter:', filtered.length, 'products')
  }

  if (searchQuery.value) {
    console.log('   - Filtering by search...')
    filtered = filtered.filter((p: any) => 
      p.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      p.sku.toLowerCase().includes(searchQuery.value.toLowerCase())
    )
    console.log('   - After search filter:', filtered.length, 'products')
  }

  console.log('   ✅ Final filtered products:', filtered.length)
  return filtered
})

// Cart totals using cart math utilities
const cartTotals = computed(() => {
  const lines = cartItems.value.map(item => ({
    productId: String(item.id),
    name: item.name,
    quantity: item.quantity,
    unitPrice: item.price,
    discountPercent: item.discountPercent,
    discountAmount: item.discountAmount,
    taxRate: item.taxRate || 15 // Default 15% SA VAT
  }))
  return cartMath.calculateCartTotals(lines)
})

const cartTotal = computed(() => {
  return cartTotals.value.grandTotal
})

const filteredCustomers = computed(() => {
  if (!customerSearchQuery.value || customerSearchQuery.value === 'Walk-in Customer') {
    return customers.value
  }
  const query = customerSearchQuery.value.toLowerCase()
  return customers.value.filter((c: any) => 
    c.name.toLowerCase().includes(query) ||
    c.phone.toLowerCase().includes(query) ||
    c.email.toLowerCase().includes(query)
  )
})

const filteredHeldSales = computed(() => {
  if (!heldSalesSearchQuery.value) {
    return heldSales.value
  }
  const query = heldSalesSearchQuery.value.toLowerCase()
  return heldSales.value.filter((sale: any) => 
    sale.customer.toLowerCase().includes(query) ||
    (sale.note && sale.note.toLowerCase().includes(query)) ||
    sale.total.toString().includes(query) ||
    formatCurrency(sale.total).includes(query)
  )
})

// Methods
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const addToCart = (product: any) => {
  if (product.stock === 0) return

  const existingItem = cartItems.value.find((item: any) => item.id === product.id)
  if (existingItem) {
    existingItem.quantity += 1
    
    // Sentry breadcrumb
    if (Sentry) {
      Sentry.addBreadcrumb({
        category: 'pos.cart',
        message: `Updated quantity for ${product.name}`,
        level: 'info',
        data: {
          productId: product.id,
          productName: product.name,
          newQuantity: existingItem.quantity
        }
      })
    }
  } else {
    cartItems.value.push({
      id: product.id,
      name: product.name,
      price: product.price,
      quantity: 1,
      taxRate: 15, // Default 15% SA VAT
      discountPercent: undefined,
      discountAmount: undefined,
      showDiscount: undefined // UI state for discount controls
    })
    
    // Sentry breadcrumb
    if (Sentry) {
      Sentry.addBreadcrumb({
        category: 'pos.cart',
        message: `Added ${product.name} to cart`,
        level: 'info',
        data: {
          productId: product.id,
          productName: product.name,
          quantity: 1
        }
      })
    }
  }
}

const removeFromCart = (productId: number | string) => {
  const item = cartItems.value.find((i: any) => i.id == productId)
  if (item && Sentry) {
    Sentry.addBreadcrumb({
      category: 'pos.cart',
      message: `Removed ${item.name} from cart`,
      level: 'info',
      data: {
        productId,
        productName: item.name
      }
    })
  }
  cartItems.value = cartItems.value.filter((item: any) => item.id != productId)
}

const updateQuantity = (productId: number | string, newQuantity: number) => {
  if (newQuantity <= 0) {
    removeFromCart(productId)
    return
  }

  const item = cartItems.value.find((item: any) => item.id == productId)
  if (item) {
    const oldQty = item.quantity
    item.quantity = newQuantity
    
    if (Sentry) {
      Sentry.addBreadcrumb({
        category: 'pos.cart',
        message: `Updated quantity for ${item.name}`,
        level: 'info',
        data: {
          productId,
          oldQuantity: oldQty,
          newQuantity
        }
      })
    }
  }
}

const clearCart = () => {
  const itemCount = cartItems.value.length
  cartItems.value = []
  selectedCustomer.value = ''
  selectedCustomerName.value = 'Walk-in Customer'
  customerSearchQuery.value = 'Walk-in Customer'
  selectedPaymentMethod.value = 'Cash'
  
  if (Sentry && itemCount > 0) {
    Sentry.addBreadcrumb({
      category: 'pos.cart',
      message: 'Cleared cart',
      level: 'info',
      data: { itemCount }
    })
  }
}

// Discount control methods
const toggleDiscountMode = (productId: number | string) => {
  const item = cartItems.value.find(i => i.id === productId)
  if (!item) return

  // Toggle through modes: none -> percent -> amount -> none
  if (!item.showDiscount) {
    item.showDiscount = 'percent'
  } else if (item.showDiscount === 'percent') {
    item.showDiscount = 'amount'
    item.discountPercent = undefined
  } else {
    item.showDiscount = undefined
    item.discountAmount = undefined
  }
}

const handleDiscountChange = (productId: number | string, type: 'percent' | 'amount', value: any) => {
  const item = cartItems.value.find(i => i.id === productId)
  if (!item) return

  const numValue = parseFloat(value) || 0

  if (type === 'percent') {
    // Cap at 100%
    item.discountPercent = Math.min(Math.max(0, numValue), 100)
    item.discountAmount = undefined // Clear opposite discount
  } else {
    // Cap at line subtotal
    const lineSubtotal = item.price * item.quantity
    item.discountAmount = Math.min(Math.max(0, numValue), lineSubtotal)
    item.discountPercent = undefined // Clear opposite discount
  }

  if (Sentry) {
    Sentry.addBreadcrumb({
      category: 'pos.discount',
      message: `Applied ${type} discount to ${item.name}`,
      level: 'info',
      data: {
        productId,
        productName: item.name,
        discountType: type,
        discountValue: numValue
      }
    })
  }
}

const clearDiscount = (productId: number | string) => {
  const item = cartItems.value.find(i => i.id === productId)
  if (!item) return

  item.discountPercent = undefined
  item.discountAmount = undefined
  item.showDiscount = undefined

  if (Sentry) {
    Sentry.addBreadcrumb({
      category: 'pos.discount',
      message: `Cleared discount from ${item.name}`,
      level: 'info',
      data: { productId, productName: item.name }
    })
  }
}

const calculateItemLineTotal = (item: any) => {
  // Use inline cart math to calculate line total
  const line = {
    productId: item.id,
    name: item.name,
    quantity: item.quantity,
    unitPrice: item.price,
    discountPercent: item.discountPercent,
    discountAmount: item.discountAmount,
    taxRate: item.taxRate || 15
  }
  return cartMath.calculateLineTotal(line)
}

// Customer selection methods
const selectCustomer = (customer: any) => {
  if (customer === null) {
    selectedCustomer.value = ''
    selectedCustomerName.value = 'Walk-in Customer'
    customerSearchQuery.value = 'Walk-in Customer'
  } else {
    selectedCustomer.value = customer.id
    selectedCustomerName.value = customer.name
    customerSearchQuery.value = customer.name
  }
  showCustomerDropdown.value = false
}

const handleCustomerBlur = () => {
  setTimeout(() => {
    showCustomerDropdown.value = false
  }, 200)
}

const addFirstProductToCart = () => {
  if (filteredProducts.value.length > 0) {
    addToCart(filteredProducts.value[0])
    searchQuery.value = ''
  }
}

// Initialize hardware on mount
onMounted(async () => {
  await loadData()
  await initializeHardware()
  setupBarcodeScanning()
  
  // Setup offline queue auto-sync
  setupAutoSync()
  
  // Load initial queue count
  try {
    const pending = await offlineQueue.getPending()
    queuePendingCount.value = pending.length
  } catch (error) {
    console.error('Failed to load queue count:', error)
  }
  
  // Add fullscreen event listeners
  document.addEventListener('fullscreenchange', handleFullscreenChange)
  document.addEventListener('keydown', handleKeyboardShortcut)
})

onUnmounted(() => {
  cleanupHardware()
  
  // Cleanup auto-sync interval
  if (autoSyncInterval) clearInterval(autoSyncInterval)
  
  // Remove fullscreen event listeners
  document.removeEventListener('fullscreenchange', handleFullscreenChange)
  document.removeEventListener('keydown', handleKeyboardShortcut)
})

// Hardware initialization
const initializeHardware = async () => {
  try {
    // Check for barcode scanner (keyboard wedge is always available)
    hardwareStatus.value.barcodeScanner = true
    
    // Check for card reader
    if ('hid' in navigator) {
      const devices = await (navigator as any).hid.getDevices()
      hardwareStatus.value.cardReader = devices.length > 0
    }
    
    // Check for receipt printer
    if ('serial' in navigator) {
      const ports = await (navigator as any).serial.getPorts()
      hardwareStatus.value.printer = ports.length > 0
    }
    
    // Cash drawer usually connected to printer
    hardwareStatus.value.cashDrawer = hardwareStatus.value.printer
  } catch (error) {
    console.error('Hardware initialization failed:', error)
  }
}

// Barcode scanning setup
const setupBarcodeScanning = () => {
  document.addEventListener('keypress', handleBarcodeInput)
}

const handleBarcodeInput = (event: KeyboardEvent) => {
  // Clear timeout on each keypress
  if (barcodeTimeout) {
    clearTimeout(barcodeTimeout)
  }

  // Add character to buffer
  if (event.key.length === 1) {
    barcodeBuffer += event.key
  }

  // Process barcode on Enter key (scanners send Enter after barcode)
  if (event.key === 'Enter' && barcodeBuffer.length >= 8) {
    processBarcode(barcodeBuffer)
    barcodeBuffer = ''
    event.preventDefault()
    return
  }

  // Reset buffer after 100ms of inactivity (scanners are fast)
  barcodeTimeout = setTimeout(() => {
    barcodeBuffer = ''
  }, 100)
}

const processBarcode = (barcode: string) => {
  // Find product by SKU or barcode
  const product = products.value.find((p: any) => 
    p.sku.toLowerCase() === barcode.toLowerCase() ||
    barcode.includes(p.id.toString())
  )
  
  if (product && product.stock > 0) {
    addToCart(product)
    showNotification(`Added ${product.name} to cart`)
  } else {
    showNotification('Product not found or out of stock', 'error')
  }
}

const showNotification = (message: string, type: 'success' | 'error' = 'success') => {
  const notification = document.createElement('div')
  notification.textContent = type === 'success' ? `✓ ${message}` : `✗ ${message}`
  notification.className = `fixed top-20 right-4 ${type === 'success' ? 'bg-green-600' : 'bg-red-600'} text-white px-4 py-2 rounded-lg shadow-lg z-50 animate-fade-in`
  document.body.appendChild(notification)
  setTimeout(() => notification.remove(), 2000)
}

const cleanupHardware = () => {
  document.removeEventListener('keypress', handleBarcodeInput)
  if (barcodeTimeout) {
    clearTimeout(barcodeTimeout)
  }
}

const toggleScanner = () => {
  scannerActive.value = !scannerActive.value
  if (scannerActive.value) {
    showNotification('Barcode scanner activated - ready to scan')
  }
}

const handleBarcodeScanned = (barcode: string) => {
  // Find product by barcode or SKU
  const product = products.value.find((p: any) => 
    p.sku.toLowerCase() === barcode.toLowerCase() ||
    barcode.toLowerCase().includes(p.sku.toLowerCase())
  )
  
  if (product && product.stock > 0) {
    addToCart(product)
    showNotification(`✓ Added ${product.name} to cart`)
  } else {
    showNotification(`✗ Product not found: ${barcode}`, 'error')
  }
}

const requestHardwareAccess = async () => {
  try {
    // Request serial port access (for receipt printer)
    if ('serial' in navigator) {
      await (navigator as any).serial.requestPort()
      hardwareStatus.value.printer = true
      hardwareStatus.value.cashDrawer = true
      showNotification('✓ Printer and cash drawer connected')
    }
    
    // Request HID access (for card reader)
    if ('hid' in navigator) {
      await (navigator as any).hid.requestDevice({ filters: [] })
      hardwareStatus.value.cardReader = true
      showNotification('✓ Card reader connected')
    }
    
    // Re-initialize hardware
    await initializeHardware()
  } catch (error) {
    console.error('Hardware request cancelled or failed:', error)
  }
}

const processPayment = async () => {
  if (cartItems.value.length === 0) return
  
  try {
    if (selectedPaymentMethod.value === 'Card' && hardwareStatus.value.cardReader) {
      showNotification('Processing card payment...')
      await new Promise(resolve => setTimeout(resolve, 2000))
    }

    // Get customer ID (use undefined for walk-in customers)
    const customerId = selectedCustomer.value ? parseInt(selectedCustomer.value) : undefined

    // Build sale data with full cart details (including discounts)
    const saleData = {
      shopId: shopId.value,
      customerId: customerId,
      items: cartItems.value.map((item: any) => ({
        productId: item.id,
        quantity: item.quantity,
        unitPrice: item.price,
        discountPercent: item.discountPercent,
        discountAmount: item.discountAmount,
        taxRate: item.taxRate || 15
      })),
      paymentType: selectedPaymentMethod.value,
      totalAmount: cartTotal.value
    }

    // Try online first, fallback to queue
    let saleId: number | null = null
    let wasQueued = false
    
    if (navigator.onLine) {
      try {
        const result = await salesAPI.createSale(saleData)
        saleId = result.id
        console.log(`✅ Sale ${saleId} created successfully (online)`)
        
        if (Sentry) {
          Sentry.addBreadcrumb({
            category: 'pos.payment',
            message: 'Sale processed online',
            level: 'info',
            data: { saleId, paymentMethod: selectedPaymentMethod.value, total: cartTotal.value }
          })
        }
      } catch (apiError) {
        console.warn('API call failed, queueing for later:', apiError)
        await offlineQueue.enqueue({ data: saleData })
        wasQueued = true
        
        if (Sentry) {
          Sentry.addBreadcrumb({
            category: 'pos.payment',
            message: 'Sale queued for offline sync',
            level: 'warning',
            data: { paymentMethod: selectedPaymentMethod.value, total: cartTotal.value }
          })
        }
      }
    } else {
      // Offline mode: enqueue immediately
      await offlineQueue.enqueue({ data: saleData })
      wasQueued = true
      
      if (Sentry) {
        Sentry.addBreadcrumb({
          category: 'pos.payment',
          message: 'Sale queued (offline)',
          level: 'info',
          data: { paymentMethod: selectedPaymentMethod.value, total: cartTotal.value }
        })
      }
    }
    
    // Update queue count
    const pending = await offlineQueue.getPending()
    queuePendingCount.value = pending.length
    
    // Show success notification
    if (wasQueued) {
      showNotification(`✓ Sale queued for sync (${queuePendingCount.value} pending)`)
    } else {
      showNotification(`✓ Sale completed! Transaction #${saleId}`)
    }
    
    showSuccessModal.value = true
    clearCart()
  } catch (error) {
    console.error('Payment processing failed:', error)
    showNotification('✗ Payment failed. Please try again.', 'error')
    
    if (Sentry) {
      Sentry.captureException(error)
    }
  }
}

// Hold Sale functionality
const confirmHoldSale = async () => {
  if (cartItems.value.length === 0) return
  
  try {
    const customerId = selectedCustomer.value ? parseInt(selectedCustomer.value) : undefined
    
    // Save held sale to database
    const result = await salesAPI.holdSale({
      shopId: shopId.value,
      customerId: customerId,
      items: cartItems.value.map((item: any) => ({
        productId: item.id,
        quantity: item.quantity,
        unitPrice: item.price
      })),
      paymentMethod: selectedPaymentMethod.value,
      totalAmount: cartTotal.value,
      notes: holdSaleNote.value
    })
    
    // Reload held sales from database
    await loadHeldSales()
    
    showNotification(`✓ Sale held successfully (ID: ${result.id})`)
    showHoldSaleModal.value = false
    holdSaleNote.value = ''
    clearCart()
  } catch (error) {
    console.error('Failed to hold sale:', error)
    showNotification('✗ Failed to hold sale', 'error')
  }
}

// Void Sale functionality
const confirmVoidSale = () => {
  if (cartItems.value.length === 0) return
  
  // Note: In the current flow, void is for current cart before it's saved
  // For voiding completed sales, use the voidSale API with a sale ID
  console.log(`Sale voided. Reason: ${voidSaleReason.value}`)
  showNotification('✓ Sale voided successfully')
  
  showVoidSaleModal.value = false
  voidSaleReason.value = ''
  clearCart()
}

// Create Queue Order functionality
const confirmCreateOrder = async () => {
  if (cartItems.value.length === 0) return
  
  try {
    const customerId = selectedCustomer.value ? parseInt(selectedCustomer.value) : undefined
    
    // Create queue order via API
    const result = await salesAPI.createQueueOrder({
      shopId: shopId.value,
      customerId: customerId,
      customerName: orderCustomerName.value,
      customerPhone: orderCustomerPhone.value,
      customerNotes: orderCustomerNotes.value,
      items: cartItems.value.map((item: any) => ({
        productId: item.id,
        quantity: item.quantity,
        unitPrice: item.price
      })),
      paymentType: selectedPaymentMethod.value as any,
      totalAmount: cartTotal.value,
      estimatedPreparationMinutes: orderEstimatedMinutes.value
    })
    
    console.log(`✅ Order ${result.id} created successfully (Queue-based)`)
    showNotification(`✓ Order created! Order #${result.id} added to queue`)
    
    // Clear form and close modal
    showCreateOrderModal.value = false
    orderCustomerName.value = ''
    orderCustomerPhone.value = ''
    orderCustomerNotes.value = ''
    orderEstimatedMinutes.value = 15
    clearCart()
  } catch (error) {
    console.error('Failed to create queue order:', error)
    showNotification('✗ Failed to create order', 'error')
  }
}

// Load held sales from database
const loadHeldSales = async () => {
  try {
    const held = await salesAPI.getHeldSales(shopId.value)
    heldSales.value = (held || []).map((sale: any) => ({
      id: sale.id || sale.Id || 0,
      saleNumber: sale.saleNumber || sale.SaleNumber || '',
      items: (sale.items || sale.Items || []).map((item: any) => ({
        id: item.productId || item.ProductId || 0,
        name: item.productName || item.ProductName || 'Unknown',
        quantity: item.quantity || item.Quantity || 0,
        price: item.unitPrice || item.UnitPrice || 0
      })),
      customer: sale.customerName || sale.CustomerName || '',
      customerId: sale.customerId || sale.CustomerId || '',
      paymentMethod: sale.paymentMethod || sale.PaymentMethod || 'Cash',
      total: sale.total || sale.Total || 0,
      note: sale.notes || sale.Notes || '',
      timestamp: new Date(sale.heldAt || sale.HeldAt).toLocaleString('en-ZA')
    }))
  } catch (error) {
    console.error('Failed to load held sales:', error)
  }
}

// Load voided sales from database
const loadVoidedSales = async () => {
  try {
    const voided = await salesAPI.getVoidedSales(shopId.value)
    voidedSales.value = (voided || []).map((sale: any) => ({
      id: sale.id || sale.Id || 0,
      saleNumber: sale.saleNumber || sale.SaleNumber || '',
      items: (sale.items || sale.Items || []).map((item: any) => ({
        id: item.productId || item.ProductId || 0,
        name: item.productName || item.ProductName || 'Unknown',
        quantity: item.quantity || item.Quantity || 0,
        price: item.unitPrice || item.UnitPrice || 0
      })),
      customer: sale.customerName || sale.CustomerName || '',
      customerId: sale.customerId || sale.CustomerId || '',
      paymentMethod: sale.paymentMethod || sale.PaymentMethod || 'Cash',
      total: sale.total || sale.Total || 0,
      reason: sale.voidReason || sale.VoidReason || '',
      timestamp: new Date(sale.voidedAt || sale.VoidedAt).toLocaleString('en-ZA')
    }))
  } catch (error) {
    console.error('Failed to load voided sales:', error)
  }
}

// Get original index from filtered array
const getOriginalIndex = (sale: any) => {
  return heldSales.value.findIndex((s: any) => s.id === sale.id)
}

// Retrieve held sale
const retrieveHeldSale = async (index: number) => {
  const sale = heldSales.value[index]
  
  try {
    // Update status in database from OnHold to Pending
    await salesAPI.retrieveHeldSale(sale.id)
    
    // Load into cart
    cartItems.value = [...sale.items]
    selectedCustomer.value = sale.customerId || ''
    selectedCustomerName.value = sale.customer
    customerSearchQuery.value = sale.customer
    selectedPaymentMethod.value = sale.paymentMethod
    
    // Reload held sales from database
    await loadHeldSales()
    
    showHeldSalesModal.value = false
    heldSalesSearchQuery.value = ''
    showNotification(`✓ Sale retrieved successfully`)
  } catch (error) {
    console.error('Failed to retrieve held sale:', error)
    showNotification('✗ Failed to retrieve sale', 'error')
  }
}

// Delete held sale
const deleteHeldSale = async (index: number) => {
  if (confirm('Are you sure you want to delete this held sale?')) {
    const sale = heldSales.value[index]
    
    try {
      await salesAPI.deleteHeldSale(sale.id)
      
      // Reload held sales from database
      await loadHeldSales()
      
      showNotification('✓ Held sale deleted')
      if (heldSales.value.length === 0) {
        showHeldSalesModal.value = false
        heldSalesSearchQuery.value = ''
      }
    } catch (error) {
      console.error('Failed to delete held sale:', error)
      showNotification('✗ Failed to delete sale', 'error')
    }
  }
}

// Open reports modal and load all report data
const openReportsModal = async () => {
  showReports.value = true
  // Load held and voided sales data
  await Promise.all([
    loadHeldSales(),
    loadVoidedSales()
  ])
}

// Print report
const printReport = () => {
  showNotification('✓ Report sent to printer')
  window.print()
}

const openDrawer = async () => {
  try {
    if (hardwareStatus.value.cashDrawer) {
      // Send ESC/POS command to open drawer via printer
      if ('serial' in navigator) {
        const ports = await (navigator as any).serial.getPorts()
        if (ports.length > 0) {
          const printer = ports[0]
          await printer.open({ baudRate: 9600 })
          
          // ESC p m t1 t2 - Open cash drawer command
          const command = new Uint8Array([0x1B, 0x70, 0x00, 0x19, 0xFA])
          
          const writer = printer.writable.getWriter()
          await writer.write(command)
          writer.releaseLock()
          
          await printer.close()
          showNotification('Cash drawer opened')
          return
        }
      }
    }
    
    // Fallback notification
    showNotification('Cash drawer opened (simulated)')
  } catch (error) {
    console.error('Failed to open drawer:', error)
    showNotification('Failed to open cash drawer', 'error')
  }
}

const testPrinter = async () => {
  try {
    if (hardwareStatus.value.printer) {
      const testReceipt = {
        storeName: "THABO'S SPAZA SHOP",
        storeAddress: '123 Main Street, Soweto',
        storePhone: '+27 11 123 4567',
        receiptNumber: `TEST-${Date.now()}`,
        date: new Date().toLocaleString('en-ZA'),
        cashier: 'Thabo',
        customer: 'Test Print',
        items: [
          { name: 'Test Item 1', quantity: 1, price: 10.00, total: 10.00 },
          { name: 'Test Item 2', quantity: 2, price: 5.00, total: 10.00 }
        ],
        total: 20.00,
        paymentMethod: 'Cash'
      }
      
      await printESCPOSReceipt(testReceipt)
      showNotification('✓ Test receipt printed successfully')
    } else {
      showNotification('⚠️ Printer not connected. Please connect hardware first.', 'error')
    }
  } catch (error) {
    console.error('Test print failed:', error)
    showNotification('✗ Test print failed', 'error')
  }
}

const printReceipt = async () => {
  try {
    if (hardwareStatus.value.printer) {
      // Generate and print receipt
      const receiptData = {
        storeName: "THABO'S SPAZA SHOP",
        storeAddress: '123 Main Street, Soweto',
        storePhone: '+27 11 123 4567',
        receiptNumber: `RCP-${Date.now()}`,
        date: new Date().toLocaleString('en-ZA'),
        cashier: 'Thabo',
        customer: selectedCustomer.value || 'Walk-in Customer',
        items: cartItems.value.map((item: any) => ({
          name: item.name,
          quantity: item.quantity,
          price: item.price,
          total: item.price * item.quantity
        })),
        total: cartTotal.value,
        paymentMethod: selectedPaymentMethod.value
      }
      
      await printESCPOSReceipt(receiptData)
      showNotification('Receipt printed successfully')
    } else {
      // Fallback to browser print
      window.print()
    }
    
    closeSuccessModal()
  } catch (error) {
    console.error('Print failed:', error)
    window.print() // Fallback
    closeSuccessModal()
  }
}

const printESCPOSReceipt = async (receiptData: any) => {
  if ('serial' in navigator) {
    const ports = await (navigator as any).serial.getPorts()
    if (ports.length > 0) {
      const printer = ports[0]
      await printer.open({ baudRate: 9600 })
      
      const encoder = new TextEncoder()
      const ESC = 0x1B
      const GS = 0x1D
      
      let commands: number[] = []
      
      // Initialize
      commands.push(ESC, 0x40)
      
      // Center align
      commands.push(ESC, 0x61, 0x01)
      
      // Store name (bold)
      commands.push(ESC, 0x45, 0x01)
      commands.push(...encoder.encode(receiptData.storeName + '\n'))
      commands.push(ESC, 0x45, 0x00)
      
      // Store info
      commands.push(...encoder.encode(receiptData.storeAddress + '\n'))
      commands.push(...encoder.encode(receiptData.storePhone + '\n\n'))
      
      // Left align
      commands.push(ESC, 0x61, 0x00)
      
      // Receipt details
      commands.push(...encoder.encode(`Receipt: ${receiptData.receiptNumber}\n`))
      commands.push(...encoder.encode(`Date: ${receiptData.date}\n`))
      commands.push(...encoder.encode(`Cashier: ${receiptData.cashier}\n\n`))
      commands.push(...encoder.encode('--------------------------------\n'))
      
      // Items
      receiptData.items.forEach((item: any) => {
        commands.push(...Array.from(encoder.encode(`${item.name}\n`)))
        commands.push(...Array.from(encoder.encode(`${item.quantity} x R${item.price.toFixed(2)} = R${item.total.toFixed(2)}\n`)))
      })
      
      commands.push(...encoder.encode('--------------------------------\n'))
      
      // Total (bold)
      commands.push(ESC, 0x45, 0x01)
      commands.push(...encoder.encode(`TOTAL: R${receiptData.total.toFixed(2)}\n`))
      commands.push(ESC, 0x45, 0x00)
      
      commands.push(...encoder.encode(`Payment: ${receiptData.paymentMethod}\n\n`))
      
      // Footer
      commands.push(ESC, 0x61, 0x01)
      commands.push(...encoder.encode('Thank you!\n\n\n'))
      
      // Cut paper
      commands.push(GS, 0x56, 0x00)
      
      const writer = printer.writable.getWriter()
      await writer.write(new Uint8Array(commands))
      writer.releaseLock()
      
      await printer.close()
    }
  }
}

const emailReceipt = () => {
  // In production, send email via API
  showNotification('Receipt emailed successfully')
  closeSuccessModal()
}

const closeSuccessModal = () => {
  showSuccessModal.value = false
  clearCart()
}

// Fullscreen functionality
const toggleFullscreen = async () => {
  try {
    if (!document.fullscreenElement) {
      // Enter fullscreen
      await document.documentElement.requestFullscreen()
      isFullscreen.value = true
      
      // Trigger sidebar collapse by dispatching a custom event
      window.dispatchEvent(new CustomEvent('collapse-sidebar', { detail: { collapse: true } }))
      
      showNotification('✓ Entered fullscreen mode. Press F11 or ESC to exit')
    } else {
      // Exit fullscreen
      await document.exitFullscreen()
      isFullscreen.value = false
      
      // Trigger sidebar expand by dispatching a custom event
      window.dispatchEvent(new CustomEvent('collapse-sidebar', { detail: { collapse: false } }))
      
      showNotification('✓ Exited fullscreen mode')
    }
  } catch (error) {
    console.error('Fullscreen error:', error)
    showNotification('✗ Fullscreen not supported or blocked', 'error')
  }
}

// Handle fullscreen changes (F11, ESC, etc.)
const handleFullscreenChange = () => {
  isFullscreen.value = !!document.fullscreenElement
}

// Keyboard shortcut for fullscreen (F11)
const handleKeyboardShortcut = (event: KeyboardEvent) => {
  if (event.key === 'F11') {
    event.preventDefault()
    toggleFullscreen()
  }
}

// Page metadata
definePageMeta({
  layout: 'default',
  title: 'Point of Sale - TOSS ERP'
})

// Meta
useHead({
  title: 'Point of Sale - TOSS ERP',
  meta: [
    { name: 'description', content: 'Quick checkout system for retail businesses' }
  ]
})
</script>
