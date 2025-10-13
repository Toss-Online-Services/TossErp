<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Blanket Orders</h1>
              <p class="text-gray-600 dark:text-gray-400">Long-term supplier agreements with scheduled releases</p>
            </div>
            <div class="flex space-x-3">
              <button @click="openCreateModal" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center">
                <PlusIcon class="w-5 h-5 mr-2" />
                New Blanket Order
              </button>
              <button @click="exportOrders" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center">
                <ArrowDownTrayIcon class="w-5 h-5 mr-2" />
                Export
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Blanket Order Stats -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="grid grid-cols-1 md:grid-cols-5 gap-6 mb-8">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-blue-100 dark:bg-blue-900/30">
              <DocumentTextIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Active Agreements</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.activeAgreements }}</p>
              <p class="text-xs text-gray-500 dark:text-gray-500">{{ stats.totalValue }}M committed</p>
            </div>
          </div>
        </div>
        
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-green-100 dark:bg-green-900/30">
              <TruckIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Scheduled Releases</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.scheduledReleases }}</p>
              <p class="text-xs text-gray-500 dark:text-gray-500">{{ stats.thisMonth }} this month</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-purple-100 dark:bg-purple-900/30">
              <ChartBarIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Utilization Rate</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.utilizationRate }}%</p>
              <p class="text-xs text-gray-500 dark:text-gray-500">Of commitment</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-orange-100 dark:bg-orange-900/30">
              <CurrencyDollarIcon class="w-6 h-6 text-orange-600 dark:text-orange-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Cost Savings</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.costSavings }}%</p>
              <p class="text-xs text-gray-500 dark:text-gray-500">vs spot pricing</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-yellow-100 dark:bg-yellow-900/30">
              <ClockIcon class="w-6 h-6 text-yellow-600 dark:text-yellow-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Expiring Soon</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.expiringSoon }}</p>
              <p class="text-xs text-gray-500 dark:text-gray-500">Within 30 days</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Filters -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6 mb-6">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Search</label>
            <div class="relative">
              <MagnifyingGlassIcon class="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
              <input 
                v-model="searchQuery"
                type="text" 
                placeholder="Search blanket orders..."
                class="w-full pl-10 pr-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              />
            </div>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Status</label>
            <select v-model="selectedStatus" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Status</option>
              <option value="active">Active</option>
              <option value="draft">Draft</option>
              <option value="expired">Expired</option>
              <option value="cancelled">Cancelled</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Supplier</label>
            <select v-model="selectedSupplier" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Suppliers</option>
              <option value="Tech Solutions Inc">Tech Solutions Inc</option>
              <option value="Raw Materials Corp">Raw Materials Corp</option>
              <option value="Quality Equipment Co">Quality Equipment Co</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Expiry</label>
            <select v-model="selectedExpiry" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All</option>
              <option value="expiring">Expiring Soon</option>
              <option value="expired">Expired</option>
              <option value="active">Active</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Blanket Orders List -->
      <div class="space-y-6">
        <div v-for="order in filteredOrders" :key="order.id" class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden">
          <!-- Order Header -->
          <div class="bg-gradient-to-r from-blue-50 to-purple-50 dark:from-blue-900/20 dark:to-purple-900/20 px-6 py-4 border-b border-gray-200 dark:border-gray-600">
            <div class="flex items-center justify-between">
              <div class="flex items-center space-x-4">
                <div class="flex-shrink-0 h-14 w-14">
                  <div class="h-14 w-14 rounded-full bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center">
                    <span class="text-xl font-bold text-white">{{ order.supplier.charAt(0) }}</span>
                  </div>
                </div>
                <div>
                  <h3 class="text-lg font-bold text-gray-900 dark:text-white">{{ order.number }}</h3>
                  <p class="text-sm text-gray-600 dark:text-gray-400">{{ order.supplier }} • {{ order.orderType }}</p>
                </div>
                <span class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium"
                      :class="getStatusClass(order.status)">
                  {{ order.status }}
                </span>
              </div>
              <div class="text-right">
                <p class="text-2xl font-bold text-gray-900 dark:text-white">R {{ order.totalValue.toLocaleString() }}</p>
                <p class="text-xs text-gray-500 dark:text-gray-500">Total agreement value</p>
              </div>
            </div>
          </div>

          <!-- Order Details -->
          <div class="px-6 py-4">
            <div class="grid grid-cols-1 md:grid-cols-5 gap-4 mb-4">
              <div>
                <p class="text-xs text-gray-500 dark:text-gray-500 mb-1">Valid Period</p>
                <p class="text-sm font-medium text-gray-900 dark:text-white">{{ formatDate(order.startDate) }}</p>
                <p class="text-sm font-medium text-gray-900 dark:text-white">to {{ formatDate(order.endDate) }}</p>
                <p v-if="getDaysUntil(order.endDate) <= 30" class="text-xs text-orange-500 mt-1">
                  {{ getDaysUntil(order.endDate) }} days remaining
                </p>
              </div>
              <div>
                <p class="text-xs text-gray-500 dark:text-gray-500 mb-1">Commitment</p>
                <p class="text-sm font-medium text-gray-900 dark:text-white">{{ order.quantityCommitted.toLocaleString() }} {{ order.uom }}</p>
                <div class="mt-1">
                  <div class="w-full bg-gray-200 rounded-full h-2 dark:bg-gray-700">
                    <div class="bg-blue-600 h-2 rounded-full" :style="{ width: (order.quantityOrdered / order.quantityCommitted * 100) + '%' }"></div>
                  </div>
                  <p class="text-xs text-gray-500 dark:text-gray-500 mt-1">{{ order.quantityOrdered.toLocaleString() }} ordered</p>
                </div>
              </div>
              <div>
                <p class="text-xs text-gray-500 dark:text-gray-500 mb-1">Unit Price</p>
                <p class="text-sm font-medium text-gray-900 dark:text-white">R {{ order.unitPrice.toLocaleString() }}</p>
                <p class="text-xs text-green-600">{{ order.discountPercent }}% volume discount</p>
              </div>
              <div>
                <p class="text-xs text-gray-500 dark:text-gray-500 mb-1">Release Schedule</p>
                <p class="text-sm font-medium text-gray-900 dark:text-white">{{ order.releaseFrequency }}</p>
                <p class="text-xs text-gray-500 dark:text-gray-500">{{ order.releasesScheduled }} releases scheduled</p>
              </div>
              <div>
                <p class="text-xs text-gray-500 dark:text-gray-500 mb-1">Next Release</p>
                <p class="text-sm font-medium text-gray-900 dark:text-white">{{ formatDate(order.nextRelease) }}</p>
                <p class="text-xs text-gray-500 dark:text-gray-500">{{ order.nextReleaseQty }} {{ order.uom }}</p>
              </div>
            </div>

            <!-- Terms Summary -->
            <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-4 mb-4">
              <h4 class="text-sm font-medium text-gray-900 dark:text-white mb-2">Agreement Terms</h4>
              <div class="grid grid-cols-1 md:grid-cols-3 gap-3 text-sm">
                <div class="flex items-center">
                  <CheckCircleIcon class="w-4 h-4 text-green-500 mr-2" />
                  <span class="text-gray-700 dark:text-gray-300">Payment: {{ order.paymentTerms }}</span>
                </div>
                <div class="flex items-center">
                  <CheckCircleIcon class="w-4 h-4 text-green-500 mr-2" />
                  <span class="text-gray-700 dark:text-gray-300">Delivery: {{ order.deliveryTerms }}</span>
                </div>
                <div class="flex items-center">
                  <CheckCircleIcon class="w-4 h-4 text-green-500 mr-2" />
                  <span class="text-gray-700 dark:text-gray-300">Auto-renew: {{ order.autoRenew ? 'Yes' : 'No' }}</span>
                </div>
              </div>
            </div>

            <!-- Releases Timeline -->
            <div class="mb-4">
              <div class="flex items-center justify-between mb-3">
                <h4 class="text-sm font-medium text-gray-900 dark:text-white">Release History</h4>
                <button @click="scheduleRelease(order)" class="text-blue-600 hover:text-blue-800 text-sm font-medium flex items-center">
                  <CalendarIcon class="w-4 h-4 mr-1" />
                  Schedule New Release
                </button>
              </div>
              <div class="space-y-2">
                <div v-for="release in order.releases.slice(0, 3)" :key="release.id" 
                     class="flex items-center justify-between p-3 bg-gray-50 dark:bg-gray-700 rounded-lg">
                  <div class="flex items-center space-x-3">
                    <div class="flex-shrink-0">
                      <component :is="getReleaseIcon(release.status)" 
                                 class="w-5 h-5"
                                 :class="getReleaseIconColor(release.status)" />
                    </div>
                    <div>
                      <p class="text-sm font-medium text-gray-900 dark:text-white">{{ release.number }}</p>
                      <p class="text-xs text-gray-500 dark:text-gray-500">{{ formatDate(release.date) }}</p>
                    </div>
                  </div>
                  <div class="text-right">
                    <p class="text-sm font-medium text-gray-900 dark:text-white">{{ release.quantity }} {{ order.uom }}</p>
                    <span class="text-xs px-2 py-0.5 rounded-full" :class="getReleaseStatusClass(release.status)">
                      {{ release.status }}
                    </span>
                  </div>
                </div>
              </div>
              <button v-if="order.releases.length > 3" @click="viewAllReleases(order)" class="text-sm text-blue-600 hover:text-blue-800 mt-2">
                View all {{ order.releases.length }} releases →
              </button>
            </div>

            <!-- Actions -->
            <div class="flex items-center justify-between pt-4 border-t border-gray-200 dark:border-gray-700">
              <div class="flex space-x-2">
                <button @click="viewOrder(order)" class="text-blue-600 hover:text-blue-800 text-sm font-medium flex items-center">
                  <EyeIcon class="w-4 h-4 mr-1" />
                  View Details
                </button>
                <button @click="amendOrder(order)" class="text-purple-600 hover:text-purple-800 text-sm font-medium flex items-center">
                  <PencilIcon class="w-4 h-4 mr-1" />
                  Amend Terms
                </button>
                <button v-if="order.autoRenew" @click="toggleAutoRenew(order)" class="text-orange-600 hover:text-orange-800 text-sm font-medium flex items-center">
                  <ArrowPathIcon class="w-4 h-4 mr-1" />
                  Disable Auto-renew
                </button>
              </div>
              <div class="flex space-x-2">
                <button @click="downloadOrder(order)" class="p-1.5 text-gray-600 hover:text-gray-900 dark:text-gray-400">
                  <ArrowDownTrayIcon class="w-5 h-5" />
                </button>
                <button @click="printOrder(order)" class="p-1.5 text-gray-600 hover:text-gray-900 dark:text-gray-400">
                  <PrinterIcon class="w-5 h-5" />
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Blanket Order Modal -->
    <div v-if="showCreateModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-10 mx-auto p-5 border w-11/12 md:w-4/5 lg:w-3/4 shadow-lg rounded-md bg-white dark:bg-gray-800 max-h-[90vh] overflow-y-auto">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Create Blanket Order</h3>
            <button @click="closeCreateModal" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-200">
              <XMarkIcon class="w-6 h-6" />
            </button>
          </div>
          
          <form @submit.prevent="submitBlanketOrder" class="space-y-6">
            <!-- Basic Information -->
            <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Order Number</label>
                <input 
                  v-model="newOrder.number"
                  type="text" 
                  readonly
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-gray-100 dark:bg-gray-600 text-gray-900 dark:text-white"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Supplier *</label>
                <select 
                  v-model="newOrder.supplier"
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                >
                  <option value="">Select Supplier</option>
                  <option value="Tech Solutions Inc">Tech Solutions Inc</option>
                  <option value="Raw Materials Corp">Raw Materials Corp</option>
                  <option value="Quality Equipment Co">Quality Equipment Co</option>
                  <option value="Industrial Supplies SA">Industrial Supplies SA</option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Order Type</label>
                <select 
                  v-model="newOrder.orderType"
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                >
                  <option value="Materials">Materials</option>
                  <option value="Services">Services</option>
                  <option value="Equipment">Equipment</option>
                  <option value="Mixed">Mixed</option>
                </select>
              </div>
            </div>

            <!-- Agreement Period -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Start Date *</label>
                <input 
                  v-model="newOrder.startDate"
                  type="date" 
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">End Date *</label>
                <input 
                  v-model="newOrder.endDate"
                  type="date" 
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                />
              </div>
            </div>

            <!-- Item Details -->
            <div class="border-t pt-4">
              <h4 class="text-md font-medium text-gray-900 dark:text-white mb-4">Item Commitment</h4>
              <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Item/Service *</label>
                  <input 
                    v-model="newOrder.itemName"
                    type="text" 
                    required
                    class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                    placeholder="e.g., Steel Sheets Grade A"
                  />
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Total Quantity *</label>
                  <input 
                    v-model="newOrder.quantityCommitted"
                    type="number" 
                    min="1"
                    required
                    class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  />
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Unit</label>
                  <select 
                    v-model="newOrder.uom"
                    class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  >
                    <option value="Kg">Kg</option>
                    <option value="Litre">Litre</option>
                    <option value="Nos">Nos</option>
                    <option value="Meter">Meter</option>
                    <option value="Hours">Hours</option>
                  </select>
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Unit Price *</label>
                  <input 
                    v-model="newOrder.unitPrice"
                    type="number" 
                    step="0.01"
                    min="0"
                    required
                    class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  />
                </div>
              </div>
            </div>

            <!-- Release Schedule -->
            <div class="border-t pt-4">
              <h4 class="text-md font-medium text-gray-900 dark:text-white mb-4">Release Schedule</h4>
              <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Frequency</label>
                  <select 
                    v-model="newOrder.releaseFrequency"
                    class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  >
                    <option value="Weekly">Weekly</option>
                    <option value="Bi-weekly">Bi-weekly</option>
                    <option value="Monthly">Monthly</option>
                    <option value="Quarterly">Quarterly</option>
                    <option value="On-demand">On-demand</option>
                  </select>
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Quantity Per Release</label>
                  <input 
                    v-model="newOrder.qtyPerRelease"
                    type="number" 
                    min="1"
                    class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  />
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Lead Time (days)</label>
                  <input 
                    v-model="newOrder.leadTime"
                    type="number" 
                    min="1"
                    class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  />
                </div>
              </div>
            </div>

            <!-- Terms & Pricing -->
            <div class="border-t pt-4">
              <h4 class="text-md font-medium text-gray-900 dark:text-white mb-4">Terms & Pricing</h4>
              <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Volume Discount %</label>
                  <input 
                    v-model="newOrder.discountPercent"
                    type="number" 
                    step="0.1"
                    min="0"
                    max="100"
                    class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  />
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Payment Terms</label>
                  <select 
                    v-model="newOrder.paymentTerms"
                    class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  >
                    <option value="Net 30">Net 30</option>
                    <option value="Net 60">Net 60</option>
                    <option value="Net 90">Net 90</option>
                    <option value="COD">Cash on Delivery</option>
                  </select>
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Delivery Terms</label>
                  <select 
                    v-model="newOrder.deliveryTerms"
                    class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  >
                    <option value="FOB">FOB Destination</option>
                    <option value="CIF">CIF</option>
                    <option value="EXW">Ex Works</option>
                    <option value="DDP">Delivered Duty Paid</option>
                  </select>
                </div>
              </div>
            </div>

            <!-- Additional Options -->
            <div class="border-t pt-4">
              <h4 class="text-md font-medium text-gray-900 dark:text-white mb-3">Additional Options</h4>
              <div class="space-y-2">
                <div class="flex items-center">
                  <input 
                    v-model="newOrder.autoRenew"
                    type="checkbox" 
                    class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                  />
                  <label class="ml-2 text-sm text-gray-700 dark:text-gray-300">Auto-renew agreement at expiry</label>
                </div>
                <div class="flex items-center">
                  <input 
                    v-model="newOrder.priceProtection"
                    type="checkbox" 
                    class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                  />
                  <label class="ml-2 text-sm text-gray-700 dark:text-gray-300">Price protection (lock rates for agreement period)</label>
                </div>
                <div class="flex items-center">
                  <input 
                    v-model="newOrder.flexibleSchedule"
                    type="checkbox" 
                    class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                  />
                  <label class="ml-2 text-sm text-gray-700 dark:text-gray-300">Allow flexible release scheduling</label>
                </div>
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Terms & Conditions</label>
              <textarea 
                v-model="newOrder.terms"
                rows="3"
                class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                placeholder="Special terms, conditions, or notes..."
              ></textarea>
            </div>
            
            <div class="flex items-center justify-end space-x-3 pt-4">
              <button 
                type="button"
                @click="closeCreateModal"
                class="px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700"
              >
                Cancel
              </button>
              <button 
                type="submit"
                class="px-4 py-2 bg-blue-600 border border-transparent rounded-md text-sm font-medium text-white hover:bg-blue-700"
              >
                Create Blanket Order
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import {
  PlusIcon,
  ArrowDownTrayIcon,
  DocumentTextIcon,
  ClockIcon,
  TruckIcon,
  ChartBarIcon,
  CurrencyDollarIcon,
  MagnifyingGlassIcon,
  EyeIcon,
  PencilIcon,
  ArrowPathIcon,
  PrinterIcon,
  XMarkIcon,
  CheckCircleIcon,
  CalendarIcon,
  StarIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Blanket Orders - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage long-term supplier agreements and blanket orders in TOSS ERP' }
  ]
})

// Reactive data
const searchQuery = ref('')
const selectedStatus = ref('')
const selectedSupplier = ref('')
const selectedExpiry = ref('')
const showCreateModal = ref(false)

// Stats
const stats = ref({
  activeAgreements: 12,
  totalValue: 2.4,
  scheduledReleases: 45,
  thisMonth: 18,
  utilizationRate: 78,
  costSavings: 22,
  expiringSoon: 3
})

// New blanket order form
const newOrder = ref({
  number: '',
  supplier: '',
  orderType: 'Materials',
  startDate: '',
  endDate: '',
  itemName: '',
  quantityCommitted: 1000,
  uom: 'Kg',
  unitPrice: 0,
  releaseFrequency: 'Monthly',
  qtyPerRelease: 100,
  leadTime: 7,
  discountPercent: 0,
  paymentTerms: 'Net 30',
  deliveryTerms: 'FOB',
  autoRenew: false,
  priceProtection: true,
  flexibleSchedule: false,
  terms: ''
})

// Mock blanket orders data
const blanketOrders = ref([
  {
    id: 1,
    number: 'BO-2025-001',
    supplier: 'Raw Materials Corp',
    orderType: 'Materials',
    status: 'active',
    startDate: new Date('2025-01-01'),
    endDate: new Date('2025-12-31'),
    itemName: 'Steel Sheets Grade A',
    quantityCommitted: 10000,
    quantityOrdered: 4500,
    uom: 'Kg',
    unitPrice: 145,
    totalValue: 1450000,
    discountPercent: 12,
    paymentTerms: 'Net 30',
    deliveryTerms: 'FOB',
    releaseFrequency: 'Monthly',
    releasesScheduled: 12,
    nextRelease: new Date('2025-02-01'),
    nextReleaseQty: 833,
    autoRenew: true,
    releases: [
      { id: 1, number: 'BO-2025-001-R01', date: new Date('2025-01-05'), quantity: 833, status: 'delivered' },
      { id: 2, number: 'BO-2025-001-R02', date: new Date('2025-01-12'), quantity: 833, status: 'in-transit' },
      { id: 3, number: 'BO-2025-001-R03', date: new Date('2025-02-01'), quantity: 833, status: 'scheduled' }
    ]
  },
  {
    id: 2,
    number: 'BO-2025-002',
    supplier: 'Tech Solutions Inc',
    orderType: 'Services',
    status: 'active',
    startDate: new Date('2025-01-01'),
    endDate: new Date('2025-06-30'),
    itemName: 'IT Support Services',
    quantityCommitted: 1000,
    quantityOrdered: 256,
    uom: 'Hours',
    unitPrice: 850,
    totalValue: 850000,
    discountPercent: 15,
    paymentTerms: 'Net 60',
    deliveryTerms: 'On-site',
    releaseFrequency: 'Weekly',
    releasesScheduled: 26,
    nextRelease: new Date('2025-01-20'),
    nextReleaseQty: 38,
    autoRenew: false,
    releases: [
      { id: 1, number: 'BO-2025-002-R01', date: new Date('2025-01-06'), quantity: 38, status: 'completed' },
      { id: 2, number: 'BO-2025-002-R02', date: new Date('2025-01-13'), quantity: 38, status: 'completed' },
      { id: 3, number: 'BO-2025-002-R03', date: new Date('2025-01-20'), quantity: 38, status: 'scheduled' }
    ]
  },
  {
    id: 3,
    number: 'BO-2024-015',
    supplier: 'Quality Equipment Co',
    orderType: 'Equipment',
    status: 'active',
    startDate: new Date('2024-06-01'),
    endDate: new Date('2025-01-31'),
    itemName: 'Hydraulic Oil Premium',
    quantityCommitted: 5000,
    quantityOrdered: 4800,
    uom: 'Litre',
    unitPrice: 95,
    totalValue: 475000,
    discountPercent: 8,
    paymentTerms: 'Net 30',
    deliveryTerms: 'FOB',
    releaseFrequency: 'Bi-weekly',
    releasesScheduled: 16,
    nextRelease: new Date('2025-01-23'),
    nextReleaseQty: 312,
    autoRenew: true,
    releases: [
      { id: 1, number: 'BO-2024-015-R14', date: new Date('2024-12-15'), quantity: 312, status: 'delivered' },
      { id: 2, number: 'BO-2024-015-R15', date: new Date('2025-01-09'), quantity: 312, status: 'delivered' },
      { id: 3, number: 'BO-2024-015-R16', date: new Date('2025-01-23'), quantity: 312, status: 'scheduled' }
    ]
  }
])

// Computed filtered orders
const filteredOrders = computed(() => {
  return blanketOrders.value.filter(order => {
    const matchesSearch = !searchQuery.value || 
      order.number.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      order.supplier.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      order.itemName.toLowerCase().includes(searchQuery.value.toLowerCase())
    
    const matchesStatus = !selectedStatus.value || order.status === selectedStatus.value
    const matchesSupplier = !selectedSupplier.value || order.supplier === selectedSupplier.value
    
    return matchesSearch && matchesStatus && matchesSupplier
  })
})

// Helper functions
const getStatusClass = (status: string) => {
  const classes = {
    draft: 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400',
    active: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    expired: 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400',
    cancelled: 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400'
  }
  return classes[status as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const getReleaseStatusClass = (status: string) => {
  const classes = {
    scheduled: 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
    'in-transit': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    delivered: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    completed: 'bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400',
    cancelled: 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400'
  }
  return classes[status as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const getReleaseIcon = (status: string) => {
  const icons = {
    scheduled: CalendarIcon,
    'in-transit': TruckIcon,
    delivered: CheckCircleIcon,
    completed: CheckCircleIcon
  }
  return icons[status as keyof typeof icons] || CalendarIcon
}

const getReleaseIconColor = (status: string) => {
  const colors = {
    scheduled: 'text-blue-600',
    'in-transit': 'text-yellow-600',
    delivered: 'text-green-600',
    completed: 'text-purple-600'
  }
  return colors[status as keyof typeof colors] || 'text-gray-600'
}

const formatDate = (date: Date) => {
  return date.toLocaleDateString('en-US', { 
    year: 'numeric', 
    month: 'short', 
    day: 'numeric' 
  })
}

const getDaysUntil = (date: Date) => {
  const today = new Date()
  const diffTime = date.getTime() - today.getTime()
  return Math.ceil(diffTime / (1000 * 60 * 60 * 24))
}

const generateOrderNumber = () => {
  const year = new Date().getFullYear()
  const count = blanketOrders.value.length + 1
  return `BO-${year}-${count.toString().padStart(3, '0')}`
}

// Modal functions
const openCreateModal = () => {
  newOrder.value.number = generateOrderNumber()
  const today = new Date()
  newOrder.value.startDate = today.toISOString().split('T')[0]
  const endDate = new Date()
  endDate.setMonth(endDate.getMonth() + 12)
  newOrder.value.endDate = endDate.toISOString().split('T')[0]
  showCreateModal.value = true
}

const closeCreateModal = () => {
  showCreateModal.value = false
  newOrder.value = {
    number: '',
    supplier: '',
    orderType: 'Materials',
    startDate: '',
    endDate: '',
    itemName: '',
    quantityCommitted: 1000,
    uom: 'Kg',
    unitPrice: 0,
    releaseFrequency: 'Monthly',
    qtyPerRelease: 100,
    leadTime: 7,
    discountPercent: 0,
    paymentTerms: 'Net 30',
    deliveryTerms: 'FOB',
    autoRenew: false,
    priceProtection: true,
    flexibleSchedule: false,
    terms: ''
  }
}

// Form submission
const submitBlanketOrder = () => {
  const totalValue = newOrder.value.quantityCommitted * newOrder.value.unitPrice * (1 - newOrder.value.discountPercent / 100)
  
  const order = {
    id: blanketOrders.value.length + 1,
    number: newOrder.value.number,
    supplier: newOrder.value.supplier,
    orderType: newOrder.value.orderType,
    status: 'draft',
    startDate: new Date(newOrder.value.startDate),
    endDate: new Date(newOrder.value.endDate),
    itemName: newOrder.value.itemName,
    quantityCommitted: newOrder.value.quantityCommitted,
    quantityOrdered: 0,
    uom: newOrder.value.uom,
    unitPrice: newOrder.value.unitPrice,
    totalValue: totalValue,
    discountPercent: newOrder.value.discountPercent,
    paymentTerms: newOrder.value.paymentTerms,
    deliveryTerms: newOrder.value.deliveryTerms,
    releaseFrequency: newOrder.value.releaseFrequency,
    releasesScheduled: 0,
    nextRelease: new Date(newOrder.value.startDate),
    nextReleaseQty: newOrder.value.qtyPerRelease,
    autoRenew: newOrder.value.autoRenew,
    releases: []
  }
  
  blanketOrders.value.unshift(order)
  closeCreateModal()
  alert('Blanket order created successfully!')
}

// Action functions
const viewOrder = (order: any) => {
  const details = `
Blanket Order: ${order.number}
Supplier: ${order.supplier}
Item: ${order.itemName}
Period: ${formatDate(order.startDate)} - ${formatDate(order.endDate)}
Commitment: ${order.quantityCommitted.toLocaleString()} ${order.uom}
Ordered: ${order.quantityOrdered.toLocaleString()} ${order.uom} (${Math.round(order.quantityOrdered / order.quantityCommitted * 100)}%)
Unit Price: R ${order.unitPrice.toLocaleString()}
Discount: ${order.discountPercent}%
Total Value: R ${order.totalValue.toLocaleString()}
Payment: ${order.paymentTerms}
Auto-renew: ${order.autoRenew ? 'Yes' : 'No'}

Releases: ${order.releases.length} completed
Next Release: ${formatDate(order.nextRelease)} (${order.nextReleaseQty} ${order.uom})
`
  alert(details)
}

const amendOrder = (order: any) => {
  alert(`Amendment form for ${order.number} will be implemented`)
}

const toggleAutoRenew = (order: any) => {
  order.autoRenew = !order.autoRenew
  alert(`Auto-renew ${order.autoRenew ? 'enabled' : 'disabled'} for ${order.number}`)
}

const scheduleRelease = (order: any) => {
  alert(`Schedule new release for ${order.number}`)
}

const viewAllReleases = (order: any) => {
  alert(`Viewing all ${order.releases.length} releases for ${order.number}`)
}

const downloadOrder = (order: any) => {
  alert(`Downloading ${order.number} as PDF...`)
}

const printOrder = (order: any) => {
  alert(`Printing ${order.number}...`)
}

const exportOrders = () => {
  alert('Export all blanket orders to CSV/Excel')
}
</script>

