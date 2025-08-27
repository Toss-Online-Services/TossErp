<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Purchase Invoices</h1>
              <p class="text-gray-600 dark:text-gray-400">Manage supplier invoices and payment processing</p>
            </div>
            <div class="flex space-x-3">
              <button @click="openCreateInvoiceModal" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center">
                <PlusIcon class="w-5 h-5 mr-2" />
                Create Invoice
              </button>
              <button @click="importInvoices" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center">
                <ArrowUpTrayIcon class="w-5 h-5 mr-2" />
                Import
              </button>
              <button @click="exportInvoices" class="bg-purple-600 text-white px-4 py-2 rounded-lg hover:bg-purple-700 transition-colors flex items-center">
                <ArrowDownTrayIcon class="w-5 h-5 mr-2" />
                Export
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Invoice Stats -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="grid grid-cols-1 md:grid-cols-6 gap-6 mb-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-blue-100 dark:bg-blue-900/30">
              <DocumentTextIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total Invoices</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.totalInvoices }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-yellow-100 dark:bg-yellow-900/30">
              <ClockIcon class="w-6 h-6 text-yellow-600 dark:text-yellow-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Pending</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.pendingInvoices }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-orange-100 dark:bg-orange-900/30">
              <ExclamationTriangleIcon class="w-6 h-6 text-orange-600 dark:text-orange-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Overdue</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.overdueInvoices }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-green-100 dark:bg-green-900/30">
              <CheckCircleIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Paid</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ stats.paidInvoices }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-purple-100 dark:bg-purple-900/30">
              <CurrencyDollarIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Outstanding</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">${{ stats.outstandingAmount }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-indigo-100 dark:bg-indigo-900/30">
              <BanknotesIcon class="w-6 h-6 text-indigo-600 dark:text-indigo-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total Value</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">${{ stats.totalValue }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Filters -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6 mb-6">
        <div class="grid grid-cols-1 md:grid-cols-6 gap-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Search</label>
            <div class="relative">
              <MagnifyingGlassIcon class="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
              <input 
                v-model="searchQuery"
                type="text" 
                placeholder="Search invoice number..."
                class="w-full pl-10 pr-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              />
            </div>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Status</label>
            <select v-model="selectedStatus" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Status</option>
              <option value="draft">Draft</option>
              <option value="pending">Pending Approval</option>
              <option value="approved">Approved</option>
              <option value="paid">Paid</option>
              <option value="overdue">Overdue</option>
              <option value="disputed">Disputed</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Supplier</label>
            <select v-model="selectedSupplier" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Suppliers</option>
              <option value="Tech Solutions Inc">Tech Solutions Inc</option>
              <option value="Raw Materials Corp">Raw Materials Corp</option>
              <option value="Service Pro LLC">Service Pro LLC</option>
              <option value="Consumables Direct">Consumables Direct</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Date Range</label>
            <select v-model="selectedDateRange" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Dates</option>
              <option value="week">This Week</option>
              <option value="month">This Month</option>
              <option value="quarter">This Quarter</option>
              <option value="year">This Year</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Amount Range</label>
            <select v-model="selectedAmountRange" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All Amounts</option>
              <option value="0-1000">$0 - $1,000</option>
              <option value="1000-5000">$1,000 - $5,000</option>
              <option value="5000-10000">$5,000 - $10,000</option>
              <option value="10000+">$10,000+</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">PO Status</label>
            <select v-model="selectedPOStatus" class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white">
              <option value="">All PO Status</option>
              <option value="matched">PO Matched</option>
              <option value="unmatched">No PO</option>
              <option value="partial">Partial Match</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Purchase Invoices Table -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700">
        <div class="px-6 py-4 border-b border-gray-200 dark:border-gray-700">
          <h2 class="text-lg font-semibold text-gray-900 dark:text-white">Purchase Invoices</h2>
        </div>
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-gray-50 dark:bg-gray-700">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Invoice #</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Supplier</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">PO Reference</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Amount</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Invoice Date</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Due Date</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Status</th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 dark:text-gray-300 uppercase tracking-wider">Actions</th>
              </tr>
            </thead>
            <tbody class="bg-white dark:bg-gray-800 divide-y divide-gray-200 dark:divide-gray-700">
              <tr v-for="invoice in filteredInvoices" :key="invoice.id" class="hover:bg-gray-50 dark:hover:bg-gray-700">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm font-medium text-gray-900 dark:text-white">{{ invoice.number }}</div>
                  <div class="text-sm text-gray-500 dark:text-gray-400">{{ invoice.supplierReference }}</div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="flex items-center">
                    <div class="flex-shrink-0 h-8 w-8">
                      <div class="h-8 w-8 rounded-full bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center">
                        <span class="text-xs font-medium text-white">{{ invoice.supplier.charAt(0) }}</span>
                      </div>
                    </div>
                    <div class="ml-3">
                      <div class="text-sm font-medium text-gray-900 dark:text-white">{{ invoice.supplier }}</div>
                      <div class="text-sm text-gray-500 dark:text-gray-400">{{ invoice.supplierEmail }}</div>
                    </div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div v-if="invoice.poNumber" class="text-sm font-medium text-blue-600 dark:text-blue-400">{{ invoice.poNumber }}</div>
                  <div v-else class="text-sm text-red-500 dark:text-red-400">No PO</div>
                  <div v-if="invoice.receiptNumber" class="text-sm text-gray-500 dark:text-gray-400">RCP: {{ invoice.receiptNumber }}</div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm font-medium text-gray-900 dark:text-white">${{ invoice.amount.toLocaleString() }}</div>
                  <div v-if="invoice.taxAmount" class="text-sm text-gray-500 dark:text-gray-400">Tax: ${{ invoice.taxAmount.toLocaleString() }}</div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900 dark:text-white">
                  {{ formatDate(invoice.invoiceDate) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <div class="text-sm text-gray-900 dark:text-white">{{ formatDate(invoice.dueDate) }}</div>
                  <div v-if="getDaysUntilDue(invoice.dueDate) < 0" class="text-sm text-red-500">
                    {{ Math.abs(getDaysUntilDue(invoice.dueDate)) }} days overdue
                  </div>
                  <div v-else-if="getDaysUntilDue(invoice.dueDate) <= 7" class="text-sm text-yellow-500">
                    Due in {{ getDaysUntilDue(invoice.dueDate) }} days
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                        :class="getStatusClass(invoice.status)">
                    {{ invoice.status }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <div class="flex space-x-2">
                    <button @click="viewInvoice(invoice)" class="text-blue-600 hover:text-blue-900 dark:text-blue-400 dark:hover:text-blue-300">
                      <EyeIcon class="w-5 h-5" />
                    </button>
                    <button v-if="invoice.status === 'draft'" @click="editInvoice(invoice)" class="text-indigo-600 hover:text-indigo-900 dark:text-indigo-400 dark:hover:text-indigo-300">
                      <PencilIcon class="w-5 h-5" />
                    </button>
                    <button v-if="invoice.status === 'pending'" @click="approveInvoice(invoice)" class="text-green-600 hover:text-green-900 dark:text-green-400 dark:hover:text-green-300">
                      <CheckCircleIcon class="w-5 h-5" />
                    </button>
                    <button v-if="invoice.status === 'approved'" @click="markPaid(invoice)" class="text-purple-600 hover:text-purple-900 dark:text-purple-400 dark:hover:text-purple-300">
                      <BanknotesIcon class="w-5 h-5" />
                    </button>
                    <button @click="downloadInvoice(invoice)" class="text-gray-600 hover:text-gray-900 dark:text-gray-400 dark:hover:text-gray-300">
                      <ArrowDownTrayIcon class="w-5 h-5" />
                    </button>
                    <button @click="printInvoice(invoice)" class="text-purple-600 hover:text-purple-900 dark:text-purple-400 dark:hover:text-purple-300">
                      <PrinterIcon class="w-5 h-5" />
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <!-- Create Invoice Modal -->
    <div v-if="showCreateModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-10 mx-auto p-5 border w-11/12 md:w-4/5 lg:w-3/4 shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Create Purchase Invoice</h3>
            <button @click="closeCreateModal" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-200">
              <XMarkIcon class="w-6 h-6" />
            </button>
          </div>
          
          <form @submit.prevent="submitInvoice" class="space-y-6">
            <!-- Invoice Header -->
            <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Invoice Number</label>
                <input 
                  v-model="newInvoice.number"
                  type="text" 
                  readonly
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-gray-100 dark:bg-gray-600 text-gray-900 dark:text-white"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Supplier *</label>
                <select 
                  v-model="newInvoice.supplier"
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                >
                  <option value="">Select Supplier</option>
                  <option value="Tech Solutions Inc">Tech Solutions Inc</option>
                  <option value="Raw Materials Corp">Raw Materials Corp</option>
                  <option value="Service Pro LLC">Service Pro LLC</option>
                  <option value="Consumables Direct">Consumables Direct</option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Invoice Date *</label>
                <input 
                  v-model="newInvoice.invoiceDate"
                  type="date" 
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                />
              </div>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Supplier Invoice #</label>
                <input 
                  v-model="newInvoice.supplierReference"
                  type="text" 
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  placeholder="Supplier's invoice number"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Purchase Order</label>
                <select 
                  v-model="newInvoice.poNumber"
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                >
                  <option value="">Select PO (Optional)</option>
                  <option value="PO-2024-001">PO-2024-001</option>
                  <option value="PO-2024-002">PO-2024-002</option>
                  <option value="PO-2024-003">PO-2024-003</option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Due Date *</label>
                <input 
                  v-model="newInvoice.dueDate"
                  type="date" 
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                />
              </div>
            </div>

            <!-- Invoice Lines -->
            <div>
              <h4 class="text-md font-medium text-gray-900 dark:text-white mb-4">Invoice Line Items</h4>
              
              <div class="space-y-3">
                <div v-for="(line, index) in newInvoice.lineItems" :key="index" class="grid grid-cols-1 md:grid-cols-6 gap-3 p-3 border border-gray-200 dark:border-gray-600 rounded-lg">
                  <div>
                    <input 
                      v-model="line.description"
                      type="text" 
                      placeholder="Item description"
                      required
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm"
                    />
                  </div>
                  <div>
                    <input 
                      v-model="line.quantity"
                      type="number" 
                      placeholder="Qty"
                      min="1"
                      required
                      @input="calculateLineTotal(line)"
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm"
                    />
                  </div>
                  <div>
                    <input 
                      v-model="line.unitPrice"
                      type="number" 
                      step="0.01"
                      placeholder="Unit Price"
                      required
                      @input="calculateLineTotal(line)"
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm"
                    />
                  </div>
                  <div>
                    <input 
                      v-model="line.taxRate"
                      type="number" 
                      step="0.01"
                      placeholder="Tax %"
                      @input="calculateLineTotal(line)"
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-white dark:bg-gray-700 text-gray-900 dark:text-white text-sm"
                    />
                  </div>
                  <div>
                    <input 
                      v-model="line.total"
                      type="number" 
                      step="0.01"
                      placeholder="Total"
                      readonly
                      class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded bg-gray-100 dark:bg-gray-600 text-gray-900 dark:text-white text-sm"
                    />
                  </div>
                  <div class="flex items-center">
                    <button 
                      type="button"
                      @click="removeLineItem(index)"
                      class="text-red-600 hover:text-red-800 dark:text-red-400"
                    >
                      <XMarkIcon class="w-5 h-5" />
                    </button>
                  </div>
                </div>
              </div>
              
              <button 
                type="button"
                @click="addLineItem"
                class="mt-3 px-4 py-2 text-sm text-blue-600 hover:text-blue-800 dark:text-blue-400"
              >
                + Add Line Item
              </button>
            </div>

            <!-- Invoice Totals -->
            <div class="bg-gray-50 dark:bg-gray-700 p-4 rounded-lg">
              <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                <div></div>
                <div></div>
                <div class="space-y-2">
                  <div class="flex justify-between">
                    <span class="text-sm text-gray-600 dark:text-gray-400">Subtotal:</span>
                    <span class="text-sm font-medium">${{ invoiceSubtotal.toFixed(2) }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-sm text-gray-600 dark:text-gray-400">Tax:</span>
                    <span class="text-sm font-medium">${{ invoiceTax.toFixed(2) }}</span>
                  </div>
                  <div class="flex justify-between border-t pt-2">
                    <span class="text-base font-medium text-gray-900 dark:text-white">Total:</span>
                    <span class="text-base font-bold">${{ invoiceTotal.toFixed(2) }}</span>
                  </div>
                </div>
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Notes</label>
              <textarea 
                v-model="newInvoice.notes"
                rows="3"
                class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                placeholder="Additional notes or terms..."
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
                Create Invoice
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
  PlusIcon,
  ArrowUpTrayIcon,
  ArrowDownTrayIcon,
  DocumentTextIcon,
  ClockIcon,
  ExclamationTriangleIcon,
  CheckCircleIcon,
  CurrencyDollarIcon,
  BanknotesIcon,
  MagnifyingGlassIcon,
  EyeIcon,
  PencilIcon,
  PrinterIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

// Reactive data
const searchQuery = ref('')
const selectedStatus = ref('')
const selectedSupplier = ref('')
const selectedDateRange = ref('')
const selectedAmountRange = ref('')
const selectedPOStatus = ref('')
const showCreateModal = ref(false)

// Stats data
const stats = ref({
  totalInvoices: 156,
  pendingInvoices: 23,
  overdueInvoices: 8,
  paidInvoices: 118,
  outstandingAmount: '245K',
  totalValue: '2.3M'
})

// New invoice form
const newInvoice = ref({
  number: '',
  supplier: '',
  supplierReference: '',
  poNumber: '',
  invoiceDate: '',
  dueDate: '',
  lineItems: [
    { description: '', quantity: 1, unitPrice: 0, taxRate: 0, total: 0 }
  ],
  notes: ''
})

// Mock invoices data
const invoices = ref([
  {
    id: 1,
    number: 'INV-2024-001',
    supplier: 'Tech Solutions Inc',
    supplierEmail: 'billing@techsolutions.com',
    supplierReference: 'TS-INV-4521',
    poNumber: 'PO-2024-001',
    receiptNumber: 'RCP-2024-001',
    amount: 15750,
    taxAmount: 1575,
    invoiceDate: new Date('2024-01-15'),
    dueDate: new Date('2024-02-14'),
    status: 'approved'
  },
  {
    id: 2,
    number: 'INV-2024-002',
    supplier: 'Raw Materials Corp',
    supplierEmail: 'accounts@rawmaterials.com',
    supplierReference: 'RM-2024-789',
    poNumber: 'PO-2024-002',
    receiptNumber: 'RCP-2024-002',
    amount: 25000,
    taxAmount: 2500,
    invoiceDate: new Date('2024-01-18'),
    dueDate: new Date('2024-01-25'),
    status: 'overdue'
  },
  {
    id: 3,
    number: 'INV-2024-003',
    supplier: 'Service Pro LLC',
    supplierEmail: 'finance@servicepro.com',
    supplierReference: 'SP-INV-2024-12',
    poNumber: '',
    receiptNumber: '',
    amount: 3200,
    taxAmount: 320,
    invoiceDate: new Date('2024-01-20'),
    dueDate: new Date('2024-02-19'),
    status: 'pending'
  },
  {
    id: 4,
    number: 'INV-2024-004',
    supplier: 'Consumables Direct',
    supplierEmail: 'billing@consumablesdirect.com',
    supplierReference: 'CD-4456',
    poNumber: 'PO-2024-004',
    receiptNumber: 'RCP-2024-003',
    amount: 890,
    taxAmount: 89,
    invoiceDate: new Date('2024-01-22'),
    dueDate: new Date('2024-02-21'),
    status: 'paid'
  }
])

// Computed filtered invoices
const filteredInvoices = computed(() => {
  return invoices.value.filter(invoice => {
    const matchesSearch = !searchQuery.value || 
      invoice.number.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      invoice.supplierReference.toLowerCase().includes(searchQuery.value.toLowerCase())
    
    const matchesStatus = !selectedStatus.value || invoice.status === selectedStatus.value
    const matchesSupplier = !selectedSupplier.value || invoice.supplier === selectedSupplier.value
    
    return matchesSearch && matchesStatus && matchesSupplier
  })
})

// Computed invoice totals
const invoiceSubtotal = computed(() => {
  return newInvoice.value.lineItems.reduce((sum, line) => {
    const lineSubtotal = (line.quantity || 0) * (line.unitPrice || 0)
    return sum + lineSubtotal
  }, 0)
})

const invoiceTax = computed(() => {
  return newInvoice.value.lineItems.reduce((sum, line) => {
    const lineSubtotal = (line.quantity || 0) * (line.unitPrice || 0)
    const lineTax = lineSubtotal * ((line.taxRate || 0) / 100)
    return sum + lineTax
  }, 0)
})

const invoiceTotal = computed(() => {
  return invoiceSubtotal.value + invoiceTax.value
})

// Helper functions
const getStatusClass = (status: string) => {
  const classes = {
    draft: 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400',
    pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    approved: 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
    paid: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    overdue: 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400',
    disputed: 'bg-orange-100 text-orange-800 dark:bg-orange-900/30 dark:text-orange-400'
  }
  return classes[status as keyof typeof classes] || 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400'
}

const formatDate = (date: Date) => {
  return date.toLocaleDateString('en-US', { 
    year: 'numeric', 
    month: 'short', 
    day: 'numeric' 
  })
}

const getDaysUntilDue = (dueDate: Date) => {
  const today = new Date()
  const diffTime = dueDate.getTime() - today.getTime()
  return Math.ceil(diffTime / (1000 * 60 * 60 * 24))
}

// Generate new invoice number
const generateInvoiceNumber = () => {
  const year = new Date().getFullYear()
  const count = invoices.value.length + 1
  return `INV-${year}-${count.toString().padStart(3, '0')}`
}

// Modal functions
const openCreateInvoiceModal = () => {
  newInvoice.value.number = generateInvoiceNumber()
  newInvoice.value.invoiceDate = new Date().toISOString().split('T')[0]
  const dueDate = new Date()
  dueDate.setDate(dueDate.getDate() + 30)
  newInvoice.value.dueDate = dueDate.toISOString().split('T')[0]
  showCreateModal.value = true
}

const closeCreateModal = () => {
  showCreateModal.value = false
  newInvoice.value = {
    number: '',
    supplier: '',
    supplierReference: '',
    poNumber: '',
    invoiceDate: '',
    dueDate: '',
    lineItems: [
      { description: '', quantity: 1, unitPrice: 0, taxRate: 0, total: 0 }
    ],
    notes: ''
  }
}

// Line item functions
const addLineItem = () => {
  newInvoice.value.lineItems.push({
    description: '',
    quantity: 1,
    unitPrice: 0,
    taxRate: 0,
    total: 0
  })
}

const removeLineItem = (index: number) => {
  if (newInvoice.value.lineItems.length > 1) {
    newInvoice.value.lineItems.splice(index, 1)
  }
}

const calculateLineTotal = (line: any) => {
  const subtotal = (line.quantity || 0) * (line.unitPrice || 0)
  const tax = subtotal * ((line.taxRate || 0) / 100)
  line.total = subtotal + tax
}

// Form submission
const submitInvoice = () => {
  const invoice = {
    id: invoices.value.length + 1,
    number: newInvoice.value.number,
    supplier: newInvoice.value.supplier,
    supplierEmail: 'billing@example.com', // This would come from supplier data
    supplierReference: newInvoice.value.supplierReference,
    poNumber: newInvoice.value.poNumber,
    receiptNumber: '',
    amount: invoiceTotal.value,
    taxAmount: invoiceTax.value,
    invoiceDate: new Date(newInvoice.value.invoiceDate),
    dueDate: new Date(newInvoice.value.dueDate),
    status: 'draft'
  }
  
  invoices.value.unshift(invoice)
  closeCreateModal()
  alert('Purchase invoice created successfully!')
}

// Action functions
const viewInvoice = (invoice: any) => {
  console.log('View invoice:', invoice)
}

const editInvoice = (invoice: any) => {
  console.log('Edit invoice:', invoice)
}

const approveInvoice = (invoice: any) => {
  invoice.status = 'approved'
  alert(`Invoice ${invoice.number} approved!`)
}

const markPaid = (invoice: any) => {
  invoice.status = 'paid'
  alert(`Invoice ${invoice.number} marked as paid!`)
}

const downloadInvoice = (invoice: any) => {
  console.log('Download invoice:', invoice)
  alert('Download functionality will be implemented')
}

const printInvoice = (invoice: any) => {
  console.log('Print invoice:', invoice)
  alert('Print functionality will be implemented')
}

const importInvoices = () => {
  alert('Import functionality will be implemented')
}

const exportInvoices = () => {
  alert('Export functionality will be implemented')
}

onMounted(() => {
  console.log('Purchase Invoices page mounted')
})
</script>
