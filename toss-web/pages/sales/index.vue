<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Mobile-First Page Container -->
    <div class="p-4 pb-20 space-y-4 sm:p-6 sm:space-y-6 lg:pb-6">
      <!-- Page Header -->
      <div class="flex flex-col justify-between gap-3 sm:flex-row sm:items-center sm:gap-0">
        <div>
          <h1 class="text-2xl font-bold sm:text-3xl text-gray-900">Sales Management</h1>
          <p class="mt-1 text-sm text-gray-600 sm:text-base">Track sales, manage orders, and grow your revenue</p>
        </div>
        <div class="flex flex-wrap gap-2 sm:gap-3">
          <button @click="showNewQuoteModal = true" 
                  class="flex-1 px-4 py-2 text-sm text-white transition-colors rounded-lg sm:flex-none sm:px-6 sm:py-3 bg-emerald-600 hover:bg-emerald-700 sm:text-base">
            <DocumentTextIcon class="inline w-4 h-4 mr-2 sm:w-5 sm:h-5" />
            New Quote
          </button>
          <button @click="showNewSaleModal = true" 
                  class="flex-1 px-4 py-2 text-sm text-white transition-colors bg-blue-600 rounded-lg sm:flex-none sm:px-6 sm:py-3 hover:bg-blue-700 sm:text-base">
            <ShoppingCartIcon class="inline w-4 h-4 mr-2 sm:w-5 sm:h-5" />
            New Sale
          </button>
        </div>
      </div>

      <!-- Sales Stats - Mobile First Grid -->
      <div class="grid grid-cols-1 gap-3 xs:grid-cols-2 lg:grid-cols-4 sm:gap-6">
        <div class="p-4 bg-white border shadow-sm dark:bg-slate-800 sm:p-6 rounded-xl border-gray-200">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-gray-600">Today's Sales</p>
              <p class="text-lg font-bold sm:text-2xl text-gray-900">R {{ formatCurrency(todaysSales) }}</p>
              <p class="text-xs text-green-600 sm:text-sm">+{{ todaysGrowth }}%</p>
            </div>
            <div class="p-2 bg-green-100 rounded-full sm:p-3 dark:bg-green-900">
              <CurrencyDollarIcon class="w-4 h-4 text-green-600 sm:w-6 sm:h-6" />
            </div>
          </div>
        </div>

        <div class="p-4 bg-white border shadow-sm dark:bg-slate-800 sm:p-6 rounded-xl border-gray-200">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-gray-600">Orders</p>
              <p class="text-lg font-bold sm:text-2xl text-gray-900">{{ totalOrders }}</p>
              <p class="text-xs text-blue-600 sm:text-sm">{{ pendingOrders }} pending</p>
            </div>
            <div class="p-2 bg-blue-100 rounded-full sm:p-3 dark:bg-blue-900">
              <ShoppingBagIcon class="w-4 h-4 text-blue-600 sm:w-6 sm:h-6" />
            </div>
          </div>
        </div>

        <div class="p-4 bg-white border shadow-sm dark:bg-slate-800 sm:p-6 rounded-xl border-gray-200">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-gray-600">Quotes</p>
              <p class="text-lg font-bold sm:text-2xl text-gray-900">{{ totalQuotes }}</p>
              <p class="text-xs text-yellow-600 sm:text-sm">{{ activeQuotes }} active</p>
            </div>
            <div class="p-2 bg-yellow-100 rounded-full sm:p-3 dark:bg-yellow-900">
              <DocumentTextIcon class="w-4 h-4 text-yellow-600 sm:w-6 sm:h-6" />
            </div>
          </div>
        </div>

        <div class="p-4 bg-white border shadow-sm dark:bg-slate-800 sm:p-6 rounded-xl border-gray-200">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-gray-600">Avg Order</p>
              <p class="text-lg font-bold sm:text-2xl text-gray-900">R {{ formatCurrency(averageOrder) }}</p>
              <p class="text-xs text-purple-600 sm:text-sm">{{ conversionRate }}% conv</p>
            </div>
            <div class="p-2 bg-purple-100 rounded-full sm:p-3 dark:bg-purple-900">
              <ArrowTrendingUpIcon class="w-4 h-4 text-purple-600 sm:w-6 sm:h-6" />
            </div>
          </div>
        </div>
      </div>

      <!-- Main Content Grid -->
      <div class="grid grid-cols-1 gap-4 lg:grid-cols-3 sm:gap-6">
        <!-- Recent Sales -->
        <div class="lg:col-span-2">
          <div class="bg-white border shadow-sm dark:bg-slate-800 rounded-xl border-gray-200">
            <div class="p-4 border-b sm:p-6 border-gray-200">
              <div class="flex items-center justify-between">
                <h3 class="text-base font-semibold sm:text-lg text-gray-900">Recent Sales</h3>
                <select v-model="salesFilter" class="px-2 py-1 text-xs bg-white border rounded-lg sm:text-sm border-slate-300 dark:border-slate-600 dark:bg-slate-700 text-gray-900">
                  <option value="today">Today</option>
                  <option value="week">This Week</option>
                  <option value="month">This Month</option>
                </select>
              </div>
            </div>
            <div class="p-4 sm:p-6">
              <div class="space-y-3 sm:space-y-4">
                <div v-for="sale in recentSales" :key="sale.id" class="flex items-center justify-between p-3 transition-colors border rounded-lg border-slate-100 dark:border-slate-700 hover:bg-slate-50 dark:hover:bg-slate-700">
                  <div class="flex items-center flex-1 min-w-0 space-x-3">
                    <div class="flex items-center justify-center w-8 h-8 rounded-full sm:w-10 sm:h-10" :class="getStatusColor(sale.status)">
                      <ShoppingCartIcon class="w-4 h-4 text-white sm:w-5 sm:h-5" />
                    </div>
                    <div class="flex-1 min-w-0">
                      <p class="text-sm font-medium truncate text-gray-900">{{ sale.customer }}</p>
                      <p class="text-xs truncate sm:text-sm text-gray-600">{{ sale.items }} items • {{ formatDate(sale.date) }}</p>
                    </div>
                  </div>
                  <div class="text-right">
                    <p class="text-sm font-semibold text-gray-900">R {{ formatCurrency(sale.amount) }}</p>
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
          <div class="bg-white border shadow-sm dark:bg-slate-800 rounded-xl border-gray-200">
            <div class="p-4 border-b sm:p-6 border-gray-200">
              <h3 class="text-base font-semibold sm:text-lg text-gray-900">Sales Pipeline</h3>
            </div>
            <div class="p-4 sm:p-6">
              <div class="space-y-3 sm:space-y-4">
                <div v-for="stage in pipelineStages" :key="stage.name" class="flex items-center justify-between">
                  <div>
                    <p class="text-sm font-medium text-gray-900">{{ stage.name }}</p>
                    <p class="text-xs text-gray-600">{{ stage.count }} opportunities</p>
                  </div>
                  <div class="text-right">
                    <p class="text-sm font-semibold text-gray-900">R {{ formatCurrency(stage.value) }}</p>
                    <div class="w-16 h-2 mt-1 rounded-full bg-slate-200 dark:bg-slate-700">
                      <div class="h-2 bg-blue-600 rounded-full" :style="{ width: stage.percentage + '%' }"></div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Quick Actions -->
          <div class="bg-white border shadow-sm dark:bg-slate-800 rounded-xl border-gray-200">
            <div class="p-4 border-b sm:p-6 border-gray-200">
              <h3 class="text-base font-semibold sm:text-lg text-gray-900">Quick Actions</h3>
            </div>
            <div class="p-4 sm:p-6">
              <div class="space-y-2 sm:space-y-3">
                <button @click="showNewSaleModal = true" class="block w-full p-3 text-left transition-colors border rounded-lg border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700">
                  <div class="flex items-center space-x-3">
                    <ShoppingCartIcon class="flex-shrink-0 w-5 h-5 text-blue-600" />
                    <div>
                      <p class="text-sm font-medium text-gray-900">Create Sale</p>
                      <p class="text-xs text-gray-600">Record a new sale transaction</p>
                    </div>
                  </div>
                </button>

                <button @click="showNewQuoteModal = true" class="block w-full p-3 text-left transition-colors border rounded-lg border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700">
                  <div class="flex items-center space-x-3">
                    <DocumentTextIcon class="flex-shrink-0 w-5 h-5 text-emerald-600" />
                    <div>
                      <p class="text-sm font-medium text-gray-900">Create Quote</p>
                      <p class="text-xs text-gray-600">Generate a price quote</p>
                    </div>
                  </div>
                </button>

                <NuxtLink to="/inventory" class="block w-full p-3 text-left transition-colors border rounded-lg border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700">
                  <div class="flex items-center space-x-3">
                    <CubeIcon class="flex-shrink-0 w-5 h-5 text-purple-600" />
                    <div>
                      <p class="text-sm font-medium text-gray-900">Check Inventory</p>
                      <p class="text-xs text-gray-600">View stock levels</p>
                    </div>
                  </div>
                </NuxtLink>

                <button @click="generateReport" class="block w-full p-3 text-left transition-colors border rounded-lg border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700">
                  <div class="flex items-center space-x-3">
                    <ChartBarIcon class="flex-shrink-0 w-5 h-5 text-orange-600" />
                    <div>
                      <p class="text-sm font-medium text-gray-900">Sales Report</p>
                      <p class="text-xs text-gray-600">Generate analytics</p>
                    </div>
                  </div>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Collaborative Features -->
      <div class="p-4 text-white shadow-sm bg-gradient-to-r from-blue-600 to-purple-600 rounded-xl sm:p-6">
        <div class="flex flex-col items-start justify-between gap-4 sm:flex-row sm:items-center">
          <div>
            <h3 class="text-lg font-semibold sm:text-xl">SMME Collaboration Network</h3>
            <p class="mt-1 text-sm text-blue-100 sm:text-base">Connect with other township businesses for group buying, shared logistics, and joint opportunities</p>
          </div>
          <div class="flex flex-wrap w-full gap-2 sm:gap-3 sm:w-auto">
            <button class="flex-1 px-4 py-2 text-sm transition-colors rounded-lg sm:flex-none bg-white/20 hover:bg-white/30">
              <UsersIcon class="inline w-4 h-4 mr-2" />
              Group Buying
            </button>
            <button class="flex-1 px-4 py-2 text-sm transition-colors rounded-lg sm:flex-none bg-white/20 hover:bg-white/30">
              <TruckIcon class="inline w-4 h-4 mr-2" />
              Shared Logistics
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- New Sale Modal -->
    <div v-if="showNewSaleModal" class="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50">
      <div class="bg-white rounded-xl sm:rounded-2xl shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-4 border-b sm:p-6 border-gray-200">
          <h3 class="text-lg font-semibold sm:text-xl text-gray-900">Create New Sale</h3>
        </div>
        <div class="p-4 sm:p-6">
          <form @submit.prevent="createSale">
            <div class="space-y-3 sm:space-y-4">
              <div class="grid grid-cols-1 gap-3 sm:grid-cols-2 sm:gap-4">
                <div>
                  <label class="block mb-2 text-sm font-medium text-slate-700 dark:text-slate-300">Customer</label>
                  <select v-model="newSale.customerId" required 
                          class="w-full px-3 py-2 border rounded-lg sm:px-4 border-slate-300 dark:border-slate-600 focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-slate-700 dark:text-white">
                    <option value="">Select Customer</option>
                    <option v-for="customer in customers" :key="customer.id" :value="customer.id">{{ customer.name }}</option>
                  </select>
                </div>
                <div>
                  <label class="block mb-2 text-sm font-medium text-slate-700 dark:text-slate-300">Till</label>
                  <select v-model="newSale.tillId" required 
                          class="w-full px-3 py-2 border rounded-lg sm:px-4 border-slate-300 dark:border-slate-600 focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-slate-700 dark:text-white">
                    <option value="">Select Till</option>
                    <option v-for="till in tills" :key="till.id" :value="till.id">{{ till.name }}</option>
                  </select>
                </div>
              </div>

              <!-- Sale Items -->
              <div>
                <label class="block mb-2 text-sm font-medium text-slate-700 dark:text-slate-300">Items</label>
                <div class="space-y-2">
                  <div v-for="(item, index) in newSale.items" :key="index" class="flex items-end gap-2">
                    <div class="flex-1">
                      <input v-model="item.name" placeholder="Item name" required
                             class="w-full px-3 py-2 text-sm border rounded-lg border-slate-300 dark:border-slate-600 focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                    </div>
                    <div class="w-20">
                      <input v-model.number="item.quantity" type="number" placeholder="Qty" min="1" required
                             class="w-full px-2 py-2 text-sm border rounded-lg border-slate-300 dark:border-slate-600 focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                    </div>
                    <div class="w-24">
                      <input v-model.number="item.price" type="number" step="0.01" placeholder="Price" min="0" required
                             class="w-full px-2 py-2 text-sm border rounded-lg border-slate-300 dark:border-slate-600 focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                    </div>
                    <button type="button" @click="removeItem(index)" class="p-2 text-red-600 rounded-lg hover:bg-red-50 dark:hover:bg-red-900">
                      <XMarkIcon class="w-4 h-4" />
                    </button>
                  </div>
                </div>
                <button type="button" @click="addItem" class="mt-2 text-sm text-blue-600 hover:text-blue-700">
                  + Add Item
                </button>
              </div>

              <div class="grid grid-cols-1 gap-3 sm:grid-cols-2 sm:gap-4">
                <div>
                  <label class="block mb-2 text-sm font-medium text-slate-700 dark:text-slate-300">Discount Amount</label>
                  <input v-model.number="newSale.discountAmount" type="number" step="0.01" min="0"
                         class="w-full px-3 py-2 border rounded-lg sm:px-4 border-slate-300 dark:border-slate-600 focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                </div>
                <div>
                  <label class="block mb-2 text-sm font-medium text-slate-700 dark:text-slate-300">Payment Method</label>
                  <select v-model="newSale.paymentMethod" required
                          class="w-full px-3 py-2 border rounded-lg sm:px-4 border-slate-300 dark:border-slate-600 focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                    <option value="cash">Cash</option>
                    <option value="card">Card</option>
                    <option value="mobile">Mobile Money</option>
                    <option value="eft">EFT</option>
                  </select>
                </div>
              </div>

              <div>
                <label class="block mb-2 text-sm font-medium text-slate-700 dark:text-slate-300">Notes</label>
                <textarea v-model="newSale.notes" rows="2"
                          class="w-full px-3 py-2 border rounded-lg sm:px-4 border-slate-300 dark:border-slate-600 focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white"></textarea>
              </div>

              <!-- Total Display -->
              <div class="p-4 rounded-lg bg-slate-50 dark:bg-slate-700">
                <div class="flex items-center justify-between">
                  <span class="text-lg font-semibold text-gray-900">Total:</span>
                  <span class="text-xl font-bold text-gray-900">R {{ formatCurrency(calculateTotal()) }}</span>
                </div>
              </div>
            </div>

            <div class="flex justify-end mt-4 space-x-3 sm:mt-6">
              <button @click="showNewSaleModal = false" type="button" 
                      class="px-4 py-2 text-gray-600 hover:text-slate-800 dark:hover:text-slate-200">
                Cancel
              </button>
              <button type="submit" 
                      class="px-4 py-2 text-white bg-blue-600 rounded-lg sm:px-6 hover:bg-blue-700">
                Create Sale
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- New Quote Modal -->
    <div v-if="showNewQuoteModal" class="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50">
      <div class="bg-white rounded-xl sm:rounded-2xl shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-4 border-b sm:p-6 border-gray-200">
          <h3 class="text-lg font-semibold sm:text-xl text-gray-900">Create New Quote</h3>
        </div>
        <div class="p-4 sm:p-6">
          <form @submit.prevent="createQuote">
            <div class="space-y-3 sm:space-y-4">
              <div class="grid grid-cols-1 gap-3 sm:grid-cols-2 sm:gap-4">
                <div>
                  <label class="block mb-2 text-sm font-medium text-slate-700 dark:text-slate-300">Customer Name</label>
                  <input v-model="newQuote.customerName" type="text" required 
                         class="w-full px-3 py-2 border rounded-lg sm:px-4 border-slate-300 dark:border-slate-600 focus:ring-2 focus:ring-emerald-500 dark:bg-slate-700 dark:text-white">
                </div>
                <div>
                  <label class="block mb-2 text-sm font-medium text-slate-700 dark:text-slate-300">Valid Until</label>
                  <input v-model="newQuote.validUntil" type="date" required 
                         class="w-full px-3 py-2 border rounded-lg sm:px-4 border-slate-300 dark:border-slate-600 focus:ring-2 focus:ring-emerald-500 dark:bg-slate-700 dark:text-white">
                </div>
              </div>

              <!-- Quote Items -->
              <div>
                <label class="block mb-2 text-sm font-medium text-slate-700 dark:text-slate-300">Items</label>
                <div class="space-y-2">
                  <div v-for="(item, index) in newQuote.items" :key="index" class="flex items-end gap-2">
                    <div class="flex-1">
                      <input v-model="item.description" placeholder="Item description" required
                             class="w-full px-3 py-2 text-sm border rounded-lg border-slate-300 dark:border-slate-600 focus:ring-2 focus:ring-emerald-500 dark:bg-slate-700 dark:text-white">
                    </div>
                    <div class="w-20">
                      <input v-model.number="item.quantity" type="number" placeholder="Qty" min="1" required
                             class="w-full px-2 py-2 text-sm border rounded-lg border-slate-300 dark:border-slate-600 focus:ring-2 focus:ring-emerald-500 dark:bg-slate-700 dark:text-white">
                    </div>
                    <div class="w-24">
                      <input v-model.number="item.unitPrice" type="number" step="0.01" placeholder="Price" min="0" required
                             class="w-full px-2 py-2 text-sm border rounded-lg border-slate-300 dark:border-slate-600 focus:ring-2 focus:ring-emerald-500 dark:bg-slate-700 dark:text-white">
                    </div>
                    <button type="button" @click="removeQuoteItem(index)" class="p-2 text-red-600 rounded-lg hover:bg-red-50 dark:hover:bg-red-900">
                      <XMarkIcon class="w-4 h-4" />
                    </button>
                  </div>
                </div>
                <button type="button" @click="addQuoteItem" class="mt-2 text-sm text-emerald-600 hover:text-emerald-700">
                  + Add Item
                </button>
              </div>

              <div>
                <label class="block mb-2 text-sm font-medium text-slate-700 dark:text-slate-300">Terms & Conditions</label>
                <textarea v-model="newQuote.terms" rows="3" placeholder="Enter terms and conditions..."
                          class="w-full px-3 py-2 border rounded-lg sm:px-4 border-slate-300 dark:border-slate-600 focus:ring-2 focus:ring-emerald-500 dark:bg-slate-700 dark:text-white"></textarea>
              </div>

              <!-- Quote Total -->
              <div class="p-4 rounded-lg bg-emerald-50 dark:bg-emerald-900/20">
                <div class="flex items-center justify-between">
                  <span class="text-lg font-semibold text-gray-900">Quote Total:</span>
                  <span class="text-xl font-bold text-emerald-600 dark:text-emerald-400">R {{ formatCurrency(calculateQuoteTotal()) }}</span>
                </div>
              </div>
            </div>

            <div class="flex justify-end mt-4 space-x-3 sm:mt-6">
              <button @click="showNewQuoteModal = false" type="button" 
                      class="px-4 py-2 text-gray-600 hover:text-slate-800 dark:hover:text-slate-200">
                Cancel
              </button>
              <button type="submit" 
                      class="px-4 py-2 text-white rounded-lg sm:px-6 bg-emerald-600 hover:bg-emerald-700">
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
  layout: 'default'
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
