<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <!-- Mobile-First Page Container -->
    <div class="p-4 sm:p-6 space-y-4 sm:space-y-6 pb-20 lg:pb-6">
      <!-- Page Header -->
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3 sm:gap-0">
        <div>
          <h1 class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">Sales Management</h1>
          <p class="text-slate-600 dark:text-slate-400 mt-1 text-sm sm:text-base">Track sales, manage orders, and grow your revenue</p>
        </div>
        <div class="flex flex-wrap gap-2 sm:gap-3">
          <button @click="showNewQuoteModal = true" 
                  class="flex-1 sm:flex-none px-4 py-2 sm:px-6 sm:py-3 bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg transition-colors text-sm sm:text-base">
            <DocumentTextIcon class="w-4 h-4 sm:w-5 sm:h-5 inline mr-2" />
            New Quote
          </button>
          <button @click="showNewSaleModal = true" 
                  class="flex-1 sm:flex-none px-4 py-2 sm:px-6 sm:py-3 bg-blue-600 hover:bg-blue-700 text-white rounded-lg transition-colors text-sm sm:text-base">
            <ShoppingCartIcon class="w-4 h-4 sm:w-5 sm:h-5 inline mr-2" />
            New Sale
          </button>
        </div>
      </div>

      <!-- Sales Stats - Mobile First Grid -->
      <div class="grid grid-cols-1 xs:grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-6">
        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Today's Sales</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">R {{ formatCurrency(todaysSales) }}</p>
              <p class="text-xs sm:text-sm text-green-600">+{{ todaysGrowth }}%</p>
            </div>
            <div class="p-2 sm:p-3 bg-green-100 dark:bg-green-900 rounded-full">
              <CurrencyDollarIcon class="w-4 h-4 sm:w-6 sm:h-6 text-green-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Orders</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ totalOrders }}</p>
              <p class="text-xs sm:text-sm text-blue-600">{{ pendingOrders }} pending</p>
            </div>
            <div class="p-2 sm:p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
              <ShoppingBagIcon class="w-4 h-4 sm:w-6 sm:h-6 text-blue-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Quotes</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ totalQuotes }}</p>
              <p class="text-xs sm:text-sm text-yellow-600">{{ activeQuotes }} active</p>
            </div>
            <div class="p-2 sm:p-3 bg-yellow-100 dark:bg-yellow-900 rounded-full">
              <DocumentTextIcon class="w-4 h-4 sm:w-6 sm:h-6 text-yellow-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Avg Order</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">R {{ formatCurrency(averageOrder) }}</p>
              <p class="text-xs sm:text-sm text-purple-600">{{ conversionRate }}% conv</p>
            </div>
            <div class="p-2 sm:p-3 bg-purple-100 dark:bg-purple-900 rounded-full">
              <ArrowTrendingUpIcon class="w-4 h-4 sm:w-6 sm:h-6 text-purple-600" />
            </div>
          </div>
        </div>
      </div>

      <!-- Main Content Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-4 sm:gap-6">
        <!-- Recent Sales -->
        <div class="lg:col-span-2">
          <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
            <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
              <div class="flex items-center justify-between">
                <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">Recent Sales</h3>
                <select v-model="salesFilter" class="text-xs sm:text-sm border border-slate-300 dark:border-slate-600 rounded-lg px-2 py-1 bg-white dark:bg-slate-700 text-slate-900 dark:text-white">
                  <option value="today">Today</option>
                  <option value="week">This Week</option>
                  <option value="month">This Month</option>
                </select>
              </div>
            </div>
            <div class="p-4 sm:p-6">
              <div class="space-y-3 sm:space-y-4">
                <div v-for="sale in recentSales" :key="sale.id" class="flex items-center justify-between p-3 rounded-lg border border-slate-100 dark:border-slate-700 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors">
                  <div class="flex items-center space-x-3 flex-1 min-w-0">
                    <div class="w-8 h-8 sm:w-10 sm:h-10 rounded-full flex items-center justify-center" :class="getStatusColor(sale.status)">
                      <ShoppingCartIcon class="w-4 h-4 sm:w-5 sm:h-5 text-white" />
                    </div>
                    <div class="flex-1 min-w-0">
                      <p class="text-sm font-medium text-slate-900 dark:text-white truncate">{{ sale.customer }}</p>
                      <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 truncate">{{ sale.items }} items • {{ formatDate(sale.date) }}</p>
                    </div>
                  </div>
                  <div class="text-right">
                    <p class="text-sm font-semibold text-slate-900 dark:text-white">R {{ formatCurrency(sale.amount) }}</p>
                    <span class="inline-flex px-2 py-1 text-xs rounded-full" :class="getStatusBadge(sale.status)">
                      {{ sale.status }}
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Sales Pipeline & Quick Actions -->
        <div class="space-y-4 sm:space-y-6">
          <!-- Sales Pipeline -->
          <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
            <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
              <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">Sales Pipeline</h3>
            </div>
            <div class="p-4 sm:p-6">
              <div class="space-y-3 sm:space-y-4">
                <div v-for="stage in pipelineStages" :key="stage.name" class="flex items-center justify-between">
                  <div>
                    <p class="text-sm font-medium text-slate-900 dark:text-white">{{ stage.name }}</p>
                    <p class="text-xs text-slate-600 dark:text-slate-400">{{ stage.count }} opportunities</p>
                  </div>
                  <div class="text-right">
                    <p class="text-sm font-semibold text-slate-900 dark:text-white">R {{ formatCurrency(stage.value) }}</p>
                    <div class="w-16 bg-slate-200 dark:bg-slate-700 rounded-full h-2 mt-1">
                      <div class="bg-blue-600 h-2 rounded-full" :style="{ width: stage.percentage + '%' }"></div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Quick Actions -->
          <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
            <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
              <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">Quick Actions</h3>
            </div>
            <div class="p-4 sm:p-6">
              <div class="space-y-2 sm:space-y-3">
                <button @click="showNewSaleModal = true" class="block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors">
                  <div class="flex items-center space-x-3">
                    <ShoppingCartIcon class="w-5 h-5 text-blue-600 flex-shrink-0" />
                    <div>
                      <p class="text-sm font-medium text-slate-900 dark:text-white">Create Sale</p>
                      <p class="text-xs text-slate-600 dark:text-slate-400">Record a new sale transaction</p>
                    </div>
                  </div>
                </button>

                <button @click="showNewQuoteModal = true" class="block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors">
                  <div class="flex items-center space-x-3">
                    <DocumentTextIcon class="w-5 h-5 text-emerald-600 flex-shrink-0" />
                    <div>
                      <p class="text-sm font-medium text-slate-900 dark:text-white">Create Quote</p>
                      <p class="text-xs text-slate-600 dark:text-slate-400">Generate a price quote</p>
                    </div>
                  </div>
                </button>

                <NuxtLink to="/inventory" class="block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors">
                  <div class="flex items-center space-x-3">
                    <CubeIcon class="w-5 h-5 text-purple-600 flex-shrink-0" />
                    <div>
                      <p class="text-sm font-medium text-slate-900 dark:text-white">Check Inventory</p>
                      <p class="text-xs text-slate-600 dark:text-slate-400">View stock levels</p>
                    </div>
                  </div>
                </NuxtLink>

                <button @click="generateReport" class="block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors">
                  <div class="flex items-center space-x-3">
                    <ChartBarIcon class="w-5 h-5 text-orange-600 flex-shrink-0" />
                    <div>
                      <p class="text-sm font-medium text-slate-900 dark:text-white">Sales Report</p>
                      <p class="text-xs text-slate-600 dark:text-slate-400">Generate analytics</p>
                    </div>
                  </div>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Collaborative Features -->
      <div class="bg-gradient-to-r from-blue-600 to-purple-600 rounded-xl shadow-sm p-4 sm:p-6 text-white">
        <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
          <div>
            <h3 class="text-lg sm:text-xl font-semibold">SMME Collaboration Network</h3>
            <p class="text-blue-100 text-sm sm:text-base mt-1">Connect with other township businesses for group buying, shared logistics, and joint opportunities</p>
          </div>
          <div class="flex flex-wrap gap-2 sm:gap-3 w-full sm:w-auto">
            <button class="flex-1 sm:flex-none px-4 py-2 bg-white/20 hover:bg-white/30 rounded-lg transition-colors text-sm">
              <UsersIcon class="w-4 h-4 inline mr-2" />
              Group Buying
            </button>
            <button class="flex-1 sm:flex-none px-4 py-2 bg-white/20 hover:bg-white/30 rounded-lg transition-colors text-sm">
              <TruckIcon class="w-4 h-4 inline mr-2" />
              Shared Logistics
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- New Sale Modal -->
    <div v-if="showNewSaleModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-slate-800 rounded-xl sm:rounded-2xl shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-lg sm:text-xl font-semibold text-slate-900 dark:text-white">Create New Sale</h3>
        </div>
        <div class="p-4 sm:p-6">
          <form @submit.prevent="createSale">
            <div class="space-y-3 sm:space-y-4">
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-3 sm:gap-4">
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Customer</label>
                  <select v-model="newSale.customerId" required 
                          class="w-full px-3 sm:px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-slate-700 dark:text-white">
                    <option value="">Select Customer</option>
                    <option v-for="customer in customers" :key="customer.id" :value="customer.id">{{ customer.name }}</option>
                  </select>
                </div>
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Till</label>
                  <select v-model="newSale.tillId" required 
                          class="w-full px-3 sm:px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-slate-700 dark:text-white">
                    <option value="">Select Till</option>
                    <option v-for="till in tills" :key="till.id" :value="till.id">{{ till.name }}</option>
                  </select>
                </div>
              </div>

              <!-- Sale Items -->
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Items</label>
                <div class="space-y-2">
                  <div v-for="(item, index) in newSale.items" :key="index" class="flex gap-2 items-end">
                    <div class="flex-1">
                      <input v-model="item.name" placeholder="Item name" required
                             class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white text-sm">
                    </div>
                    <div class="w-20">
                      <input v-model.number="item.quantity" type="number" placeholder="Qty" min="1" required
                             class="w-full px-2 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white text-sm">
                    </div>
                    <div class="w-24">
                      <input v-model.number="item.price" type="number" step="0.01" placeholder="Price" min="0" required
                             class="w-full px-2 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white text-sm">
                    </div>
                    <button type="button" @click="removeItem(index)" class="p-2 text-red-600 hover:bg-red-50 dark:hover:bg-red-900 rounded-lg">
                      <XMarkIcon class="w-4 h-4" />
                    </button>
                  </div>
                </div>
                <button type="button" @click="addItem" class="mt-2 text-sm text-blue-600 hover:text-blue-700">
                  + Add Item
                </button>
              </div>

              <div class="grid grid-cols-1 sm:grid-cols-2 gap-3 sm:gap-4">
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Discount Amount</label>
                  <input v-model.number="newSale.discountAmount" type="number" step="0.01" min="0"
                         class="w-full px-3 sm:px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                </div>
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Payment Method</label>
                  <select v-model="newSale.paymentMethod" required
                          class="w-full px-3 sm:px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                    <option value="cash">Cash</option>
                    <option value="card">Card</option>
                    <option value="mobile">Mobile Money</option>
                    <option value="eft">EFT</option>
                  </select>
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Notes</label>
                <textarea v-model="newSale.notes" rows="2"
                          class="w-full px-3 sm:px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white"></textarea>
              </div>

              <!-- Total Display -->
              <div class="bg-slate-50 dark:bg-slate-700 p-4 rounded-lg">
                <div class="flex justify-between items-center">
                  <span class="text-lg font-semibold text-slate-900 dark:text-white">Total:</span>
                  <span class="text-xl font-bold text-slate-900 dark:text-white">R {{ formatCurrency(calculateTotal()) }}</span>
                </div>
              </div>
            </div>

            <div class="flex justify-end space-x-3 mt-4 sm:mt-6">
              <button @click="showNewSaleModal = false" type="button" 
                      class="px-4 py-2 text-slate-600 dark:text-slate-400 hover:text-slate-800 dark:hover:text-slate-200">
                Cancel
              </button>
              <button type="submit" 
                      class="px-4 sm:px-6 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg">
                Create Sale
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- New Quote Modal -->
    <div v-if="showNewQuoteModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-slate-800 rounded-xl sm:rounded-2xl shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-lg sm:text-xl font-semibold text-slate-900 dark:text-white">Create New Quote</h3>
        </div>
        <div class="p-4 sm:p-6">
          <form @submit.prevent="createQuote">
            <div class="space-y-3 sm:space-y-4">
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-3 sm:gap-4">
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Customer Name</label>
                  <input v-model="newQuote.customerName" type="text" required 
                         class="w-full px-3 sm:px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-emerald-500 dark:bg-slate-700 dark:text-white">
                </div>
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Valid Until</label>
                  <input v-model="newQuote.validUntil" type="date" required 
                         class="w-full px-3 sm:px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-emerald-500 dark:bg-slate-700 dark:text-white">
                </div>
              </div>

              <!-- Quote Items -->
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Items</label>
                <div class="space-y-2">
                  <div v-for="(item, index) in newQuote.items" :key="index" class="flex gap-2 items-end">
                    <div class="flex-1">
                      <input v-model="item.description" placeholder="Item description" required
                             class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-emerald-500 dark:bg-slate-700 dark:text-white text-sm">
                    </div>
                    <div class="w-20">
                      <input v-model.number="item.quantity" type="number" placeholder="Qty" min="1" required
                             class="w-full px-2 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-emerald-500 dark:bg-slate-700 dark:text-white text-sm">
                    </div>
                    <div class="w-24">
                      <input v-model.number="item.unitPrice" type="number" step="0.01" placeholder="Price" min="0" required
                             class="w-full px-2 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-emerald-500 dark:bg-slate-700 dark:text-white text-sm">
                    </div>
                    <button type="button" @click="removeQuoteItem(index)" class="p-2 text-red-600 hover:bg-red-50 dark:hover:bg-red-900 rounded-lg">
                      <XMarkIcon class="w-4 h-4" />
                    </button>
                  </div>
                </div>
                <button type="button" @click="addQuoteItem" class="mt-2 text-sm text-emerald-600 hover:text-emerald-700">
                  + Add Item
                </button>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Terms & Conditions</label>
                <textarea v-model="newQuote.terms" rows="3" placeholder="Enter terms and conditions..."
                          class="w-full px-3 sm:px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-emerald-500 dark:bg-slate-700 dark:text-white"></textarea>
              </div>

              <!-- Quote Total -->
              <div class="bg-emerald-50 dark:bg-emerald-900/20 p-4 rounded-lg">
                <div class="flex justify-between items-center">
                  <span class="text-lg font-semibold text-slate-900 dark:text-white">Quote Total:</span>
                  <span class="text-xl font-bold text-emerald-600 dark:text-emerald-400">R {{ formatCurrency(calculateQuoteTotal()) }}</span>
                </div>
              </div>
            </div>

            <div class="flex justify-end space-x-3 mt-4 sm:mt-6">
              <button @click="showNewQuoteModal = false" type="button" 
                      class="px-4 py-2 text-slate-600 dark:text-slate-400 hover:text-slate-800 dark:hover:text-slate-200">
                Cancel
              </button>
              <button type="submit" 
                      class="px-4 sm:px-6 py-2 bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg">
                Create Quote
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { 
  CurrencyDollarIcon, 
  ShoppingBagIcon, 
  DocumentTextIcon, 
  ArrowTrendingUpIcon,
  ShoppingCartIcon,
  UsersIcon,
  TruckIcon,
  CubeIcon,
  ChartBarIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Sales Management - TOSS ERP',
  meta: [
    { name: 'description', content: 'Comprehensive sales management for South African SMMEs' }
  ]
})

// Layout
definePageMeta({
  layout: 'dashboard'
})

// Reactive data
const showNewSaleModal = ref(false)
const showNewQuoteModal = ref(false)
const salesFilter = ref('today')

// Sales statistics
const todaysSales = ref(24500)
const todaysGrowth = ref(15.8)
const totalOrders = ref(42)
const pendingOrders = ref(8)
const totalQuotes = ref(15)
const activeQuotes = ref(6)
const averageOrder = ref(580)
const conversionRate = ref(68)

// Recent sales data
const recentSales = ref([
  {
    id: '1',
    customer: 'Mapisa Hair Salon',
    amount: 850,
    items: 3,
    status: 'completed',
    date: new Date()
  },
  {
    id: '2',
    customer: 'Thabo Spaza Shop',
    amount: 1250,
    items: 12,
    status: 'pending',
    date: new Date(Date.now() - 2 * 60 * 60 * 1000)
  },
  {
    id: '3',
    customer: 'Nomsa Beauty',
    amount: 420,
    items: 2,
    status: 'completed',
    date: new Date(Date.now() - 4 * 60 * 60 * 1000)
  },
  {
    id: '4',
    customer: 'Sipho Auto Repair',
    amount: 2100,
    items: 1,
    status: 'processing',
    date: new Date(Date.now() - 6 * 60 * 60 * 1000)
  }
])

// Pipeline stages
const pipelineStages = ref([
  { name: 'Leads', count: 18, value: 45000, percentage: 75 },
  { name: 'Proposals', count: 12, value: 28000, percentage: 60 },
  { name: 'Negotiations', count: 8, value: 35000, percentage: 45 },
  { name: 'Closed Won', count: 15, value: 52000, percentage: 100 }
])

// Form data
const newSale = ref({
  customerId: '',
  tillId: '',
  items: [
    { name: '', quantity: 1, price: 0 }
  ],
  discountAmount: 0,
  paymentMethod: 'cash',
  notes: ''
})

const newQuote = ref({
  customerName: '',
  validUntil: '',
  items: [
    { description: '', quantity: 1, unitPrice: 0 }
  ],
  terms: ''
})

// Mock data
const customers = ref([
  { id: '1', name: 'Mapisa Hair Salon' },
  { id: '2', name: 'Thabo Spaza Shop' },
  { id: '3', name: 'Nomsa Beauty' },
  { id: '4', name: 'Sipho Auto Repair' }
])

const tills = ref([
  { id: '1', name: 'Main Till' },
  { id: '2', name: 'Mobile Till' }
])

// Helper functions
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

const formatDate = (date: Date) => {
  return new Intl.DateTimeFormat('en-ZA', {
    hour: '2-digit',
    minute: '2-digit',
    day: 'numeric',
    month: 'short'
  }).format(date)
}

const getStatusColor = (status: string) => {
  const colors = {
    completed: 'bg-green-600',
    pending: 'bg-yellow-600',
    processing: 'bg-blue-600',
    cancelled: 'bg-red-600'
  }
  return colors[status as keyof typeof colors] || 'bg-slate-600'
}

const getStatusBadge = (status: string) => {
  const badges = {
    completed: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    processing: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    cancelled: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  return badges[status as keyof typeof badges] || 'bg-slate-100 text-slate-800'
}

// Sale form functions
const addItem = () => {
  newSale.value.items.push({ name: '', quantity: 1, price: 0 })
}

const removeItem = (index: number) => {
  if (newSale.value.items.length > 1) {
    newSale.value.items.splice(index, 1)
  }
}

const calculateTotal = () => {
  const subtotal = newSale.value.items.reduce((total, item) => {
    return total + (item.quantity * item.price)
  }, 0)
  return Math.max(0, subtotal - (newSale.value.discountAmount || 0))
}

// Quote form functions
const addQuoteItem = () => {
  newQuote.value.items.push({ description: '', quantity: 1, unitPrice: 0 })
}

const removeQuoteItem = (index: number) => {
  if (newQuote.value.items.length > 1) {
    newQuote.value.items.splice(index, 1)
  }
}

const calculateQuoteTotal = () => {
  return newQuote.value.items.reduce((total, item) => {
    return total + (item.quantity * item.unitPrice)
  }, 0)
}

// Actions
const createSale = async () => {
  try {
    // TODO: Integrate with backend API
    console.log('Creating sale:', newSale.value)
    
    // Mock success
    showNewSaleModal.value = false
    
    // Add to recent sales
    recentSales.value.unshift({
      id: Date.now().toString(),
      customer: customers.value.find(c => c.id === newSale.value.customerId)?.name || 'Unknown',
      amount: calculateTotal(),
      items: newSale.value.items.length,
      status: 'completed',
      date: new Date()
    })
    
    // Update stats
    todaysSales.value += calculateTotal()
    totalOrders.value += 1
    
    // Reset form
    newSale.value = {
      customerId: '',
      tillId: '',
      items: [{ name: '', quantity: 1, price: 0 }],
      discountAmount: 0,
      paymentMethod: 'cash',
      notes: ''
    }
    
    // Show success message
    alert('Sale created successfully!')
  } catch (error) {
    console.error('Error creating sale:', error)
    alert('Failed to create sale. Please try again.')
  }
}

const createQuote = async () => {
  try {
    // TODO: Integrate with backend API
    console.log('Creating quote:', newQuote.value)
    
    // Mock success
    showNewQuoteModal.value = false
    totalQuotes.value += 1
    activeQuotes.value += 1
    
    // Reset form
    newQuote.value = {
      customerName: '',
      validUntil: '',
      items: [{ description: '', quantity: 1, unitPrice: 0 }],
      terms: ''
    }
    
    // Show success message
    alert('Quote created successfully!')
  } catch (error) {
    console.error('Error creating quote:', error)
    alert('Failed to create quote. Please try again.')
  }
}

const generateReport = () => {
  // TODO: Implement report generation
  alert('Generating sales report... This feature will be implemented soon.')
}
</script>
