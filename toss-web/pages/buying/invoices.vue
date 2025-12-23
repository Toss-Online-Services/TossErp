<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-orange-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header with Glass Morphism -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full mx-auto px-3 sm:px-4 lg:px-8 py-4 sm:py-6">
        <div class="flex flex-col space-y-3 sm:flex-row sm:items-center sm:justify-between sm:space-y-0">
          <div class="flex-1 min-w-0">
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-orange-600 to-blue-600 bg-clip-text text-transparent truncate">
              Vendor Invoices
            </h1>
            <p class="mt-1 text-xs sm:text-sm text-slate-600 dark:text-slate-400 line-clamp-1">
              Manage vendor bills and payment tracking
            </p>
          </div>
          <div class="flex space-x-2 sm:space-x-3 flex-shrink-0">
            <button 
              @click="showNewInvoiceModal = true" 
              class="inline-flex items-center justify-center px-3 sm:px-4 py-2 sm:py-2.5 bg-gradient-to-r from-orange-600 to-blue-600 text-white rounded-xl hover:from-orange-700 hover:to-blue-700 shadow-lg hover:shadow-xl transition-all duration-200 text-xs sm:text-sm font-semibold whitespace-nowrap"
            >
              <PlusIcon class="w-4 h-4 sm:mr-2" />
              <span class="hidden sm:inline">New Invoice</span>
            </button>
            <button 
              @click="exportInvoices" 
              class="inline-flex items-center justify-center px-3 sm:px-4 py-2 sm:py-2.5 rounded-xl text-xs sm:text-sm font-medium text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-800 border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 hover:shadow-md transition-all duration-200 whitespace-nowrap"
            >
              <ArrowDownTrayIcon class="w-4 h-4 sm:mr-2" />
              <span class="hidden sm:inline">Export</span>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="w-full max-w-7xl mx-auto px-3 sm:px-4 lg:px-8 py-4 sm:py-8 space-y-4 sm:space-y-6">

      <!-- Invoice Stats - Clickable Filters -->
      <div class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-4 sm:gap-6 mb-8">
        <button 
          @click="filterByStatus('')"
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border-2 border-slate-200 dark:border-slate-700 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1 text-left"
          :class="{ 'ring-4 ring-blue-500 border-blue-500': statusFilter === '' }"
        >
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Total Invoices</p>
              <p class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">{{ stats.totalInvoices }}</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-blue-500 to-indigo-600 rounded-xl">
              <DocumentTextIcon class="w-6 h-6 sm:w-8 sm:h-8 text-white" />
            </div>
          </div>
        </button>

        <button
          @click="filterByStatus('draft')"
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border-2 border-slate-200 dark:border-slate-700 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1 text-left"
          :class="{ 'ring-4 ring-slate-500 border-slate-500': statusFilter === 'draft' }"
        >
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Draft</p>
              <p class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">{{ stats.draftInvoices }}</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-slate-500 to-slate-600 rounded-xl">
              <DocumentTextIcon class="w-6 h-6 sm:w-8 sm:h-8 text-white" />
            </div>
          </div>
        </button>

        <button
          @click="filterByStatus('sent')"
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border-2 border-slate-200 dark:border-slate-700 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1 text-left"
          :class="{ 'ring-4 ring-purple-500 border-purple-500': statusFilter === 'sent' }"
        >
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Sent</p>
              <p class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">{{ stats.sentInvoices }}</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-purple-500 to-violet-600 rounded-xl">
              <PaperAirplaneIcon class="w-6 h-6 sm:w-8 sm:h-8 text-white" />
            </div>
          </div>
        </button>

        <button
          @click="filterByStatus('paid')"
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border-2 border-slate-200 dark:border-slate-700 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1 text-left"
          :class="{ 'ring-4 ring-green-500 border-green-500': statusFilter === 'paid' }"
        >
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Paid</p>
              <p class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">{{ stats.paidInvoices }}</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl">
              <CheckCircleIcon class="w-6 h-6 sm:w-8 sm:h-8 text-white" />
            </div>
          </div>
        </button>

        <button
          @click="filterByStatus('overdue')"
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border-2 border-slate-200 dark:border-slate-700 p-4 sm:p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1 text-left"
          :class="{ 'ring-4 ring-red-500 border-red-500': statusFilter === 'overdue' }"
        >
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Overdue</p>
              <p class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">{{ stats.overdueInvoices }}</p>
            </div>
            <div class="p-2 sm:p-3 bg-gradient-to-br from-red-500 to-blue-600 rounded-xl">
              <ExclamationTriangleIcon class="w-6 h-6 sm:w-8 sm:h-8 text-white" />
            </div>
          </div>
        </button>
      </div>

      <!-- Filters and Search -->
      <div class="bg-white/90 dark:bg-slate-800/90 backdrop-blur-sm rounded-2xl shadow-lg border border-slate-200/50 dark:border-slate-700/50 p-4 sm:p-6">
        <div class="flex flex-col sm:flex-row gap-3 sm:gap-4">
          <div class="flex-1">
            <input v-model="searchQuery" type="text" placeholder="Search invoices..." 
                   class="w-full px-3 sm:px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
          </div>
          <div class="flex gap-2 sm:gap-3">
            <select v-model="statusFilter" 
                    class="px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
              <option value="">All Status</option>
              <option value="draft">Draft</option>
              <option value="sent">Sent</option>
              <option value="viewed">Viewed</option>
              <option value="paid">Paid</option>
              <option value="overdue">Overdue</option>
              <option value="cancelled">Cancelled</option>
            </select>
            <select v-model="periodFilter" 
                    class="px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
              <option value="">All Time</option>
              <option value="today">Today</option>
              <option value="week">This Week</option>
              <option value="month">This Month</option>
              <option value="quarter">This Quarter</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Invoices List -->
      <div v-if="filteredInvoices.length > 0" class="space-y-4">
        <div v-for="invoice in filteredInvoices" :key="invoice.id" 
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 overflow-hidden hover:shadow-xl transition-all duration-300"
        >
          <!-- Invoice Header -->
          <div 
            @click="toggleInvoiceExpansion(invoice.id)"
            class="bg-gradient-to-r from-orange-50 to-blue-50 dark:from-orange-900/20 dark:to-blue-900/20 px-6 py-4 border-b border-slate-200 dark:border-slate-600 cursor-pointer hover:from-orange-100 hover:to-blue-100 dark:hover:from-orange-900/30 dark:hover:to-blue-900/30 transition-all"
          >
            <div class="flex items-center justify-between">
              <div class="flex items-center space-x-4">
                <div class="flex-shrink-0 h-12 w-12">
                  <div class="h-12 w-12 rounded-xl bg-gradient-to-br from-orange-500 to-blue-600 flex items-center justify-center">
                    <span class="text-lg font-bold text-white">{{ invoice.customer?.charAt(0) || 'I' }}</span>
                  </div>
                </div>
                <div>
                  <h3 class="text-lg font-bold text-slate-900 dark:text-white">{{ invoice.invoiceNumber }}</h3>
                  <p class="text-sm text-slate-600 dark:text-slate-400">{{ invoice.customer }}</p>
                </div>
              </div>
              <div class="flex items-center space-x-3">
                <span 
                  class="px-3 py-1 rounded-full text-sm font-medium"
                  :class="getStatusBadge(invoice.status)"
                >
                  {{ getStatusLabel(invoice.status) }}
                </span>
                <div class="text-right">
                  <p class="text-2xl font-bold text-slate-900 dark:text-white">R{{ formatCurrency(invoice.total) }}</p>
                  <p class="text-xs text-slate-500 dark:text-slate-400">
                    {{ expandedInvoices.includes(invoice.id) ? '▲ Click to collapse' : '▼ Click to expand' }}
                  </p>
                </div>
              </div>
            </div>
          </div>

          <!-- Invoice Details (Expandable) -->
          <transition
            enter-active-class="transition-all duration-300 ease-out"
            leave-active-class="transition-all duration-300 ease-in"
            enter-from-class="opacity-0 max-h-0"
            enter-to-class="opacity-100 max-h-[2000px]"
            leave-from-class="opacity-100 max-h-[2000px]"
            leave-to-class="opacity-0 max-h-0"
          >
            <div v-if="expandedInvoices.includes(invoice.id)" class="px-6 py-4">
              <!-- Key Invoice Details -->
              <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
                <div>
                  <p class="text-xs text-slate-500 dark:text-slate-500 mb-1">Invoice Date</p>
                  <p class="text-sm font-medium text-slate-900 dark:text-white">{{ formatDate(invoice.invoiceDate) }}</p>
                </div>
                <div>
                  <p class="text-xs text-slate-500 dark:text-slate-500 mb-1">Due Date</p>
                  <p class="text-sm font-medium text-slate-900 dark:text-white">{{ formatDate(invoice.dueDate) }}</p>
                </div>
                <div>
                  <p class="text-xs text-slate-500 dark:text-slate-500 mb-1">Order Reference</p>
                  <p class="text-sm font-medium text-slate-900 dark:text-white">{{ invoice.orderNumber || 'N/A' }}</p>
                </div>
                <div>
                  <p class="text-xs text-slate-500 dark:text-slate-500 mb-1">Items</p>
                  <p class="text-sm font-medium text-slate-900 dark:text-white">{{ invoice.invoiceItems?.length || 0 }} items</p>
                </div>
              </div>

              <!-- Invoice Items Section -->
              <div v-if="invoice.invoiceItems && invoice.invoiceItems.length > 0" class="mb-6">
                <h4 class="text-sm font-bold text-slate-900 dark:text-white mb-4 flex items-center">
                  <span class="w-2 h-2 bg-orange-500 rounded-full mr-2"></span>
                  Invoice Items
                </h4>
                <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                  <div 
                    v-for="item in invoice.invoiceItems" 
                    :key="item.id"
                    class="bg-slate-50 dark:bg-slate-900 rounded-lg p-4 border border-slate-200 dark:border-slate-700"
                  >
                    <div class="flex justify-between items-start mb-2">
                      <h5 class="font-semibold text-slate-900 dark:text-white">{{ item.name }}</h5>
                      <span 
                        class="text-xs px-2 py-1 rounded-full"
                        :class="getStockClass(item.stock)"
                      >
                        Stock: {{ item.stock }}
                      </span>
                    </div>
                    <p class="text-xs text-slate-500 dark:text-slate-400 mb-2">SKU: {{ item.sku }}</p>
                    <div class="text-sm text-slate-700 dark:text-slate-300">
                      <p>{{ item.quantity }}x @ R{{ item.price.toFixed(2) }}</p>
                      <p class="font-bold text-blue-600 dark:text-blue-400 mt-1">
                        Total: R{{ (item.quantity * item.price).toFixed(2) }}
                      </p>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Invoice Timeline -->
              <div class="mb-6">
                <h4 class="text-sm font-bold text-slate-900 dark:text-white mb-4 flex items-center">
                  <span class="w-2 h-2 bg-orange-500 rounded-full mr-2"></span>
                  Invoice Timeline
                </h4>
                <InvoiceTimeline 
                  :status="invoice.status"
                  :invoice-number="invoice.invoiceNumber"
                  :invoice-date="invoice.invoiceDate"
                  :due-date="invoice.dueDate"
                />
              </div>

              <!-- Actions -->
              <div class="flex items-center justify-between pt-4 border-t border-slate-200 dark:border-slate-700">
                <div class="flex space-x-3">
                  <button 
                    @click.stop="printInvoice(invoice)" 
                    class="text-purple-600 hover:text-purple-800 dark:text-purple-400 dark:hover:text-purple-200 text-sm font-medium flex items-center transition-colors"
                  >
                    <PrinterIcon class="w-4 h-4 mr-1" />
                    Print
                  </button>
                  <button 
                    @click.stop="sendInvoice(invoice)" 
                    class="text-blue-600 hover:text-blue-800 dark:text-blue-400 dark:hover:text-blue-200 text-sm font-medium flex items-center transition-colors"
                  >
                    <PaperAirplaneIcon class="w-4 h-4 mr-1" />
                    Send
                  </button>
                  <button 
                    v-if="invoice.status !== 'paid'"
                    @click.stop="markAsPaid(invoice)" 
                    class="text-green-600 hover:text-green-800 dark:text-green-400 dark:hover:text-green-200 text-sm font-medium flex items-center transition-colors"
                  >
                    <BanknotesIcon class="w-4 h-4 mr-1" />
                    Mark as Paid
                  </button>
                </div>
                <button 
                  v-if="invoice.status === 'draft'"
                  @click.stop="cancelInvoice(invoice)" 
                  class="text-red-600 hover:text-red-800 dark:text-red-400 dark:hover:text-red-200 text-sm font-medium flex items-center transition-colors"
                >
                  <XMarkIcon class="w-4 h-4 mr-1" />
                  Cancel
                </button>
              </div>
            </div>
          </transition>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-12 text-center">
        <DocumentTextIcon class="w-16 h-16 text-slate-400 mx-auto mb-4" />
        <p class="text-lg font-semibold text-slate-900 dark:text-white mb-2">No invoices found</p>
        <p class="text-slate-600 dark:text-slate-400">Create your first invoice to get started.</p>
      </div>
    </div>

    <!-- New Invoice Modal -->
    <div v-if="showNewInvoiceModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-slate-800 rounded-xl sm:rounded-2xl shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-lg sm:text-xl font-semibold text-slate-900 dark:text-white">Create New Invoice</h3>
        </div>
        <div class="p-4 sm:p-6">
          <form @submit.prevent="createInvoice">
            <div class="space-y-4">
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Invoice Number</label>
                  <input v-model="newInvoice.invoiceNumber" type="text" required readonly
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg bg-slate-50 dark:bg-slate-700 dark:text-white">
                </div>
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Due Date</label>
                  <input v-model="newInvoice.dueDate" type="date" required
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Customer Information</label>
                <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <input v-model="newInvoice.customerName" placeholder="Customer Name" required
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                  <input v-model="newInvoice.customerEmail" type="email" placeholder="Email Address"
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Billing Address</label>
                <textarea v-model="newInvoice.billingAddress" rows="2" placeholder="Enter billing address..."
                          class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white"></textarea>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Invoice Items</label>
                <div class="space-y-2">
                  <div v-for="(item, index) in newInvoice.items" :key="index" class="flex gap-2 items-end">
                    <div class="flex-1">
                      <input v-model="item.description" placeholder="Item description" required
                             class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white text-sm">
                    </div>
                    <div class="w-20">
                      <input v-model.number="item.quantity" type="number" placeholder="Qty" min="1" required
                             class="w-full px-2 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white text-sm">
                    </div>
                    <div class="w-24">
                      <input v-model.number="item.unitPrice" type="number" step="0.01" placeholder="Price" min="0" required
                             class="w-full px-2 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white text-sm">
                    </div>
                    <div class="w-24 text-right">
                      <p class="text-sm font-medium text-slate-900 dark:text-white py-2">R {{ formatCurrency(item.quantity * item.unitPrice) }}</p>
                    </div>
                    <button type="button" @click="removeInvoiceItem(index)" class="p-2 text-red-600 hover:bg-red-50 dark:hover:bg-red-900 rounded-lg">
                      <XMarkIcon class="w-4 h-4" />
                    </button>
                  </div>
                </div>
                <button type="button" @click="addInvoiceItem" class="mt-2 text-sm text-blue-600 hover:text-blue-700">
                  + Add Item
                </button>
              </div>

              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Tax Rate (%)</label>
                  <input v-model.number="newInvoice.taxRate" type="number" step="0.1" min="0" max="100"
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                </div>
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Discount (%)</label>
                  <input v-model.number="newInvoice.discountRate" type="number" step="0.1" min="0" max="100"
                         class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white">
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Notes</label>
                <textarea v-model="newInvoice.notes" rows="2" placeholder="Additional notes or payment terms..."
                          class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white"></textarea>
              </div>

              <div class="bg-blue-50 dark:bg-blue-900/20 p-4 rounded-lg">
                <div class="space-y-2">
                  <div class="flex justify-between">
                    <span class="text-sm text-slate-600 dark:text-slate-400">Subtotal:</span>
                    <span class="text-sm font-medium text-slate-900 dark:text-white">R {{ formatCurrency(calculateSubtotal()) }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-sm text-slate-600 dark:text-slate-400">Tax ({{ newInvoice.taxRate }}%):</span>
                    <span class="text-sm font-medium text-slate-900 dark:text-white">R {{ formatCurrency(calculateTax()) }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-sm text-slate-600 dark:text-slate-400">Discount ({{ newInvoice.discountRate }}%):</span>
                    <span class="text-sm font-medium text-red-600">-R {{ formatCurrency(calculateDiscount()) }}</span>
                  </div>
                  <div class="border-t pt-2 flex justify-between items-center">
                    <span class="text-lg font-semibold text-slate-900 dark:text-white">Total:</span>
                    <span class="text-xl font-bold text-blue-600 dark:text-blue-400">R {{ formatCurrency(calculateInvoiceTotal()) }}</span>
                  </div>
                </div>
              </div>
            </div>

            <div class="flex justify-end space-x-3 mt-6">
              <button @click="showNewInvoiceModal = false" type="button" 
                      class="px-4 py-2 text-slate-600 dark:text-slate-400 hover:text-slate-800 dark:hover:text-slate-200">
                Cancel
              </button>
              <button @click="saveAsDraft" type="button" 
                      class="px-6 py-2 border border-slate-300 dark:border-slate-600 text-slate-700 dark:text-slate-300 rounded-lg hover:bg-slate-50 dark:hover:bg-slate-700">
                Save as Draft
              </button>
              <button type="submit" 
                      class="px-6 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg">
                Create & Send
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { 
  DocumentTextIcon,
  PlusIcon,
  ArrowDownTrayIcon,
  ExclamationTriangleIcon,
  CheckCircleIcon,
  CalculatorIcon,
  EyeIcon,
  PaperAirplaneIcon,
  PrinterIcon,
  BanknotesIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'
import { useBuyingAPI } from '~/composables/useBuyingAPI'
import InvoiceTimeline from '~/components/sales/InvoiceTimeline.vue'

// Page metadata
useHead({
  title: 'Vendor Invoices - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage vendor invoices and payments for Thabo\'s Spaza Shop' }
  ]
})

// Layout
definePageMeta({
  layout: 'default'
})

// API
const buyingAPI = useBuyingAPI()

// Reactive data
const showNewInvoiceModal = ref(false)
const searchQuery = ref('')
const statusFilter = ref('')
const periodFilter = ref('')
const expandedInvoices = ref<string[]>([])
const invoices = ref<any[]>([])
const loading = ref(true)

// Load invoices on mount
onMounted(async () => {
  await loadInvoices()
})

const loadInvoices = async () => {
  loading.value = true
  try {
    const res = await buyingAPI.getVendorInvoices({ pageNumber: 1, pageSize: 100 })
    const list = Array.isArray(res?.items) ? res.items : (Array.isArray(res) ? res : [])
    // Normalize to the same shape used by the sales page UI
    invoices.value = list.map((d: any) => ({
      id: d.id,
      invoiceNumber: d.invoiceNumber ?? d.documentNumber,
      customer: d.vendor ?? d.vendorName ?? 'Vendor',
      invoiceDate: d.invoiceDate ?? d.documentDate,
      dueDate: d.dueDate,
      total: d.total ?? d.totalAmount,
      status: d.status ?? (d.isPaid ? 'paid' : (d.dueDate && new Date(d.dueDate) < new Date() ? 'overdue' : 'sent')),
      orderNumber: d.purchaseOrderNumber ?? '',
      invoiceItems: d.invoiceItems ?? []
    }))
  } catch (error) {
    console.error('Failed to load invoices:', error)
  } finally {
    loading.value = false
  }
}

// Invoice statistics - computed from actual data
const stats = computed(() => ({
  totalInvoices: invoices.value.length,
  draftInvoices: invoices.value.filter((i: any) => i.status === 'draft').length,
  sentInvoices: invoices.value.filter((i: any) => i.status === 'sent').length,
  paidInvoices: invoices.value.filter((i: any) => i.status === 'paid').length,
  overdueInvoices: invoices.value.filter((i: any) => i.status === 'overdue').length
}))

// Helper functions
const toggleInvoiceExpansion = (invoiceId: string) => {
  const index = expandedInvoices.value.indexOf(invoiceId)
  if (index > -1) {
    expandedInvoices.value.splice(index, 1)
  } else {
    expandedInvoices.value.push(invoiceId)
  }
}

const filterByStatus = (status: string) => {
  statusFilter.value = status
}

// Helper function (must be defined before use)
const generateInvoiceNumber = () => {
  const date = new Date()
  const year = date.getFullYear()
  const nextNumber = (invoices.value.length + 1).toString().padStart(3, '0')
  return `INV-${year}-${nextNumber}`
}

// Form data
const newInvoice = ref({
  invoiceNumber: generateInvoiceNumber(),
  customerName: '',
  customerEmail: '',
  billingAddress: '',
  dueDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
  items: [
    { description: '', quantity: 1, unitPrice: 0 }
  ],
  taxRate: 15,
  discountRate: 0,
  notes: 'Payment due within 30 days. Thank you for your business!'
})

// Computed
const filteredInvoices = computed(() => {
  let filtered = invoices.value

  if (searchQuery.value) {
    filtered = filtered.filter((invoice: any) => 
      invoice.customer.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      invoice.invoiceNumber.toLowerCase().includes(searchQuery.value.toLowerCase())
    )
  }

  if (statusFilter.value) {
    filtered = filtered.filter((invoice: any) => invoice.status === statusFilter.value)
  }

  if (periodFilter.value) {
    const now = new Date()
    filtered = filtered.filter(invoice => {
      const issueDate = new Date(invoice.invoiceDate)
      switch (periodFilter.value) {
        case 'today':
          return issueDate.toDateString() === now.toDateString()
        case 'week':
          const weekAgo = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000)
          return issueDate >= weekAgo
        case 'month':
          return issueDate.getMonth() === now.getMonth() && issueDate.getFullYear() === now.getFullYear()
        case 'quarter':
          const quarter = Math.floor(now.getMonth() / 3)
          const invoiceQuarter = Math.floor(issueDate.getMonth() / 3)
          return invoiceQuarter === quarter && issueDate.getFullYear() === now.getFullYear()
        default:
          return true
      }
    })
  }

  return filtered
})

// More helper functions
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

const formatDate = (date: Date) => {
  return new Date(date).toLocaleDateString('en-ZA', { 
    year: 'numeric', 
    month: 'short', 
    day: 'numeric' 
  })
}

const formatDueDate = (date: Date) => {
  const now = new Date()
  const dueDate = new Date(date)
  const diffDays = Math.ceil((dueDate.getTime() - now.getTime()) / (1000 * 60 * 60 * 24))
  
  if (diffDays < 0) {
    return `${Math.abs(diffDays)} days overdue`
  } else if (diffDays === 0) {
    return 'Due today'
  } else if (diffDays === 1) {
    return 'Due tomorrow'
  } else {
    return `Due in ${diffDays} days`
  }
}

const getStatusLabel = (status: string) => {
  const labels: Record<string, string> = {
    'draft': 'Draft',
    'sent': 'Sent',
    'viewed': 'Viewed',
    'paid': 'Paid',
    'overdue': 'Overdue',
    'cancelled': 'Cancelled'
  }
  return labels[status.toLowerCase()] || status
}

const getStockClass = (stock: number) => {
  if (stock > 10) {
    return 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400'
  } else if (stock > 0) {
    return 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400'
  } else {
    return 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400'
  }
}

const getStatusColor = (status: string) => {
  const colors = {
    draft: 'bg-slate-600',
    sent: 'bg-blue-600',
    viewed: 'bg-purple-600',
    paid: 'bg-green-600',
    overdue: 'bg-red-600',
    cancelled: 'bg-gray-600'
  }
  return colors[status as keyof typeof colors] || 'bg-slate-600'
}

const getStatusBadge = (status: string) => {
  const badges = {
    draft: 'bg-slate-100 text-slate-800 dark:bg-slate-900 dark:text-slate-200',
    sent: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    viewed: 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200',
    paid: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    overdue: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    cancelled: 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-200'
  }
  return badges[status as keyof typeof badges] || 'bg-slate-100 text-slate-800'
}

// Invoice calculation functions
const calculateSubtotal = () => {
  return newInvoice.value.items.reduce((total: number, item: any) => {
    return total + (item.quantity * item.unitPrice)
  }, 0)
}

const calculateTax = () => {
  const subtotal = calculateSubtotal()
  return subtotal * (newInvoice.value.taxRate / 100)
}

const calculateDiscount = () => {
  const subtotal = calculateSubtotal()
  return subtotal * (newInvoice.value.discountRate / 100)
}

const calculateInvoiceTotal = () => {
  const subtotal = calculateSubtotal()
  const tax = calculateTax()
  const discount = calculateDiscount()
  return subtotal + tax - discount
}

// Invoice form functions
const addInvoiceItem = () => {
  newInvoice.value.items.push({ description: '', quantity: 1, unitPrice: 0 })
}

const removeInvoiceItem = (index: number) => {
  if (newInvoice.value.items.length > 1) {
    newInvoice.value.items.splice(index, 1)
  }
}

// Actions
const createInvoice = async (sendImmediately = true) => {
  try {
    // For vendor invoice, create a purchase document
    await buyingAPI.createVendorInvoice({
      purchaseOrderId: 1, // TODO: select PO in form
      vendorId: 1, // TODO: select vendor in form
      invoiceNumber: newInvoice.value.invoiceNumber,
      invoiceDate: new Date().toISOString(),
      dueDate: newInvoice.value.dueDate,
      subtotal: calculateSubtotal(),
      taxAmount: calculateTax(),
      totalAmount: calculateInvoiceTotal(),
      notes: newInvoice.value.notes
    })

    await loadInvoices()
    showNewInvoiceModal.value = false
    
    // Reset form
    newInvoice.value = {
      invoiceNumber: generateInvoiceNumber(),
      customerName: '',
      customerEmail: '',
      billingAddress: '',
      dueDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
      items: [{ description: '', quantity: 1, unitPrice: 0 }],
      taxRate: 15,
      discountRate: 0,
      notes: 'Payment due within 30 * days. Thank you for your business!'
    }
    
    alert(`Invoice ${sendImmediately ? 'created and sent' : 'saved as draft'} successfully!`)
  } catch (error) {
    console.error('Error creating invoice:', error)
    alert('Failed to create invoice. Please try again.')
  }
}

const saveAsDraft = () => {
  createInvoice(false)
}

const viewInvoice = (invoice: any) => {
  alert(`Viewing invoice ${invoice.invoiceNumber} for ${invoice.customer}`)
}

const sendInvoice = async (invoice: any) => {
  try {
    if (invoice.status === 'draft') {
  await buyingAPI.updateVendorInvoiceStatus(invoice.id, 'sent')
      await loadInvoices()
      alert(`Invoice ${invoice.invoiceNumber} sent to ${invoice.customer}`)
    } else {
      alert(`Invoice ${invoice.invoiceNumber} resent to ${invoice.customer}`)
    }
  } catch (error) {
    console.error('Failed to send invoice:', error)
    alert('Failed to send invoice')
  }
}

const printInvoice = (invoice: any) => {
  alert(`Printing invoice ${invoice.invoiceNumber}`)
}

const markAsPaid = async (invoice: any) => {
  if (invoice.status !== 'paid') {
    try {
  await buyingAPI.updateVendorInvoiceStatus(invoice.id, 'paid')
      await loadInvoices()
      alert(`Invoice ${invoice.invoiceNumber} marked as paid`)
    } catch (error) {
      console.error('Failed to mark as paid:', error)
      alert('Failed to update invoice status')
    }
  }
}

const cancelInvoice = async (invoice: any) => {
  if (confirm(`Are you sure you want to cancel invoice ${invoice.invoiceNumber}?`)) {
    try {
  await buyingAPI.updateVendorInvoiceStatus(invoice.id, 'cancelled')
      await loadInvoices()
      alert(`Invoice ${invoice.invoiceNumber} cancelled`)
    } catch (error) {
      console.error('Failed to cancel invoice:', error)
      alert('Failed to cancel invoice')
    }
  }
}

const exportInvoices = () => {
  try {
    // Get filtered invoices
    const invoicesToExport = filteredInvoices.value
    
    if (invoicesToExport.length === 0) {
      alert('No invoices to export')
      return
    }

    // Create CSV header
  const headers = ['Invoice #', 'Customer/Vendor', 'Date', 'Due Date', 'Amount', 'Status', 'Items']
    
    // Create CSV rows
    const rows = invoicesToExport.map((inv: any) => {
      const items = Array.isArray(inv.invoiceItems) ? inv.invoiceItems : []
      const itemsSummary = items.map((item: any) => 
        `${item.name ?? item.description ?? ''} (${item.quantity ?? 0})`
      ).join('; ')
      
      return [
        inv.invoiceNumber,
        inv.customer ?? inv.vendor ?? '',
        inv.invoiceDate ? new Date(inv.invoiceDate).toLocaleDateString() : '',
        inv.dueDate ? new Date(inv.dueDate).toLocaleDateString() : '',
        `R${Number(inv.total ?? 0).toFixed(2)}`,
        inv.status,
        itemsSummary
      ]
    })
    
    // Combine headers and rows
    const csvContent = [
      headers.join(','),
      ...rows.map(row => row.map(cell => `"${cell}"`).join(','))
    ].join('\n')
    
    // Create blob and download
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
    const link = document.createElement('a')
    const url = URL.createObjectURL(blob)
    
    link.setAttribute('href', url)
  link.setAttribute('download', `vendor_invoices_${new Date().toISOString().split('T')[0]}.csv`)
    link.style.visibility = 'hidden'
    
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    
    alert(`✓ Exported ${invoicesToExport.length} invoices successfully!`)
  } catch (error) {
    console.error('Export failed:', error)
    alert('✗ Failed to export invoices')
  }
}
</script>
