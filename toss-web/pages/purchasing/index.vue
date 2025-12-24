<script setup lang="ts">
/**
 * Purchasing/Buying Module - Main Dashboard
 * 
 * ERPNext-aligned Buying module for TOSS
 * Manages: Suppliers, RFQs, Purchase Orders, Receipts, Purchase Invoices
 * 
 * TOSS Context: Helps township businesses manage stock purchases, supplier relationships
 * Mobile-first with offline support for viewing supplier info and creating orders
 */
import { ref } from 'vue'
import { Building2, FileText, Package, TrendingDown, AlertCircle, Plus } from 'lucide-vue-next'

useHead({
  title: 'Purchasing - TOSS ERP',
  meta: [{ name: 'description', content: 'Manage suppliers and stock purchases' }]
})

definePageMeta({
  layout: 'dashboard',
  middleware: ['auth']
})

// Mock data - will be replaced with API calls
const stats = ref({
  suppliers: 24,
  pendingOrders: 8,
  thisMonthPurchases: 45250,
  awaitingReceipt: 5
})

const recentOrders = ref([
  { id: 'PO-001', supplier: 'Mthembu Wholesalers', items: 12, total: 3450, status: 'Delivered', date: '2025-12-20' },
  { id: 'PO-002', supplier: 'Dlamini Suppliers', items: 8, total: 2100, status: 'Pending', date: '2025-12-22' },
  { id: 'PO-003', supplier: 'Ubuntu Fresh Produce', items: 15, total: 1890, status: 'In Transit', date: '2025-12-23' }
])

const topSuppliers = ref([
  { name: 'Mthembu Wholesalers', orders: 45, total: 125600, reliability: 98 },
  { name: 'Dlamini Suppliers', orders: 32, total: 89400, reliability: 95 },
  { name: 'Ubuntu Fresh Produce', orders: 28, total: 67200, reliability: 92 }
])
</script>

<template>
  <div class="p-4 md:p-6 space-y-6">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
      <div>
        <h1 class="text-2xl md:text-3xl font-bold text-gray-900 dark:text-white">Purchasing</h1>
        <p class="text-gray-600 dark:text-gray-400 mt-1">Manage suppliers and stock purchases</p>
      </div>
      <NuxtLink to="/buying/orders/create-order" class="btn btn-primary flex items-center justify-center gap-2">
        <Plus :size="20" />
        <span>New Purchase Order</span>
      </NuxtLink>
    </div>

    <!-- Quick Stats -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
      <div class="card p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 dark:text-gray-400">Total Suppliers</p>
            <p class="text-2xl font-bold mt-1">{{ stats.suppliers }}</p>
          </div>
          <Building2 class="w-10 h-10 text-blue-500" />
        </div>
      </div>

      <div class="card p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 dark:text-gray-400">Pending Orders</p>
            <p class="text-2xl font-bold mt-1">{{ stats.pendingOrders }}</p>
          </div>
          <AlertCircle class="w-10 h-10 text-yellow-500" />
        </div>
      </div>

      <div class="card p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 dark:text-gray-400">This Month</p>
            <p class="text-2xl font-bold mt-1">R {{ stats.thisMonthPurchases.toLocaleString() }}</p>
          </div>
          <TrendingDown class="w-10 h-10 text-green-500" />
        </div>
      </div>

      <div class="card p-4">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 dark:text-gray-400">Awaiting Receipt</p>
            <p class="text-2xl font-bold mt-1">{{ stats.awaitingReceipt }}</p>
          </div>
          <Package class="w-10 h-10 text-purple-500" />
        </div>
      </div>
    </div>

    <!-- Quick Links -->
    <div class="grid grid-cols-2 sm:grid-cols-4 gap-3">
      <NuxtLink to="/buying/suppliers" class="card p-4 hover:shadow-lg transition-shadow text-center">
        <Building2 class="w-8 h-8 mx-auto text-blue-500 mb-2" />
        <p class="font-semibold text-sm">Suppliers</p>
      </NuxtLink>

      <NuxtLink to="/buying/orders" class="card p-4 hover:shadow-lg transition-shadow text-center">
        <FileText class="w-8 h-8 mx-auto text-green-500 mb-2" />
        <p class="font-semibold text-sm">Purchase Orders</p>
      </NuxtLink>

      <NuxtLink to="/buying/invoices" class="card p-4 hover:shadow-lg transition-shadow text-center">
        <FileText class="w-8 h-8 mx-auto text-purple-500 mb-2" />
        <p class="font-semibold text-sm">Invoices</p>
      </NuxtLink>

      <NuxtLink to="/buying/group-buying" class="card p-4 hover:shadow-lg transition-shadow text-center">
        <Package class="w-8 h-8 mx-auto text-yellow-500 mb-2" />
        <p class="font-semibold text-sm">Group Buying</p>
      </NuxtLink>
    </div>

    <!-- Recent Orders -->
    <div class="card">
      <div class="p-4 border-b border-gray-200 dark:border-gray-700 flex items-center justify-between">
        <h2 class="text-lg font-semibold">Recent Purchase Orders</h2>
        <NuxtLink to="/buying/orders" class="text-sm text-primary hover:underline">View All</NuxtLink>
      </div>

      <div class="overflow-x-auto">
        <table class="table">
          <thead>
            <tr>
              <th>Order #</th>
              <th>Supplier</th>
              <th>Items</th>
              <th>Total</th>
              <th>Status</th>
              <th>Date</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="order in recentOrders" :key="order.id" class="hover:bg-gray-50 dark:hover:bg-gray-800">
              <td class="font-mono">{{ order.id }}</td>
              <td>{{ order.supplier }}</td>
              <td>{{ order.items }}</td>
              <td class="font-semibold">R {{ order.total.toLocaleString() }}</td>
              <td>
                <span
                  :class="[
                    'badge',
                    order.status === 'Delivered' ? 'badge-success' : '',
                    order.status === 'Pending' ? 'badge-warning' : '',
                    order.status === 'In Transit' ? 'badge-info' : ''
                  ]"
                >
                  {{ order.status }}
                </span>
              </td>
              <td>{{ order.date }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Top Suppliers -->
    <div class="card">
      <div class="p-4 border-b border-gray-200 dark:border-gray-700">
        <h2 class="text-lg font-semibold">Top Suppliers This Month</h2>
      </div>

      <div class="p-4 space-y-4">
        <div v-for="supplier in topSuppliers" :key="supplier.name" class="flex items-center justify-between">
          <div class="flex-1">
            <p class="font-semibold">{{ supplier.name }}</p>
            <p class="text-sm text-gray-600 dark:text-gray-400">{{ supplier.orders }} orders</p>
          </div>
          <div class="text-right">
            <p class="font-bold">R {{ supplier.total.toLocaleString() }}</p>
            <p class="text-sm text-gray-600 dark:text-gray-400">{{ supplier.reliability }}% reliability</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
