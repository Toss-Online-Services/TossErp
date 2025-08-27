<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Purchasing & Procurement Dashboard</h1>
              <p class="text-gray-600 dark:text-gray-400">Complete procurement lifecycle management with collaborative features</p>
            </div>
            <div class="flex space-x-3">
              <NuxtLink to="/purchasing/orders" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center">
                <PlusIcon class="w-5 h-5 mr-2" />
                New Purchase Order
              </NuxtLink>
              <button @click="startGroupBuy" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center">
                <UserGroupIcon class="w-5 h-5 mr-2" />
                Start Group Buy
              </button>
              <button @click="scanReceipt" class="bg-purple-600 text-white px-4 py-2 rounded-lg hover:bg-purple-700 transition-colors flex items-center">
                <QrCodeIcon class="w-5 h-5 mr-2" />
                Scan Receipt
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Key Metrics -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="grid grid-cols-1 md:grid-cols-6 gap-6 mb-8">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-blue-100 dark:bg-blue-900/30">
              <ShoppingCartIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Active POs</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ metrics.activePOs }}</p>
            </div>
          </div>
        </div>
        
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-green-100 dark:bg-green-900/30">
              <BuildingOfficeIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Suppliers</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ metrics.suppliers }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-yellow-100 dark:bg-yellow-900/30">
              <ClipboardDocumentCheckIcon class="w-6 h-6 text-yellow-600 dark:text-yellow-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Pending Receipts</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ metrics.pendingReceipts }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-purple-100 dark:bg-purple-900/30">
              <DocumentTextIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Open Invoices</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ metrics.openInvoices }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-orange-100 dark:bg-orange-900/30">
              <CurrencyDollarIcon class="w-6 h-6 text-orange-600 dark:text-orange-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Monthly Spend</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">${{ metrics.monthlySpend }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-indigo-100 dark:bg-indigo-900/30">
              <UserGroupIcon class="w-6 h-6 text-indigo-600 dark:text-indigo-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Group Buys</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ metrics.groupBuys }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Actions Grid -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <!-- Suppliers Quick Access -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Supplier Management</h3>
            <NuxtLink to="/purchasing/suppliers" class="text-blue-600 hover:text-blue-800 dark:text-blue-400">
              <ArrowRightIcon class="w-5 h-5" />
            </NuxtLink>
          </div>
          <p class="text-gray-600 dark:text-gray-400 mb-4">Manage supplier relationships, ratings, and performance metrics</p>
          <div class="space-y-2">
            <div class="flex justify-between">
              <span class="text-sm text-gray-600 dark:text-gray-400">Active Suppliers:</span>
              <span class="text-sm font-medium">{{ supplierStats.active }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-sm text-gray-600 dark:text-gray-400">Avg Rating:</span>
              <span class="text-sm font-medium">{{ supplierStats.avgRating }}/5</span>
            </div>
            <div class="flex justify-between">
              <span class="text-sm text-gray-600 dark:text-gray-400">New This Month:</span>
              <span class="text-sm font-medium">{{ supplierStats.newThisMonth }}</span>
            </div>
          </div>
          <NuxtLink to="/purchasing/suppliers" class="block mt-4 text-center bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition-colors">
            Manage Suppliers
          </NuxtLink>
        </div>

        <!-- Purchase Requests -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Purchase Requests</h3>
            <NuxtLink to="/purchasing/requests" class="text-blue-600 hover:text-blue-800 dark:text-blue-400">
              <ArrowRightIcon class="w-5 h-5" />
            </NuxtLink>
          </div>
          <p class="text-gray-600 dark:text-gray-400 mb-4">Create and manage purchase requests with group buying options</p>
          <div class="space-y-2">
            <div class="flex justify-between">
              <span class="text-sm text-gray-600 dark:text-gray-400">Pending Requests:</span>
              <span class="text-sm font-medium">{{ requestStats.pending }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-sm text-gray-600 dark:text-gray-400">Group Buy Opportunities:</span>
              <span class="text-sm font-medium">{{ requestStats.groupBuyOpportunities }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-sm text-gray-600 dark:text-gray-400">Approved Today:</span>
              <span class="text-sm font-medium">{{ requestStats.approvedToday }}</span>
            </div>
          </div>
          <NuxtLink to="/purchasing/requests" class="block mt-4 text-center bg-green-600 text-white py-2 rounded-lg hover:bg-green-700 transition-colors">
            View Requests
          </NuxtLink>
        </div>

        <!-- Recent Activity -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Recent Activity</h3>
            <ClockIcon class="w-5 h-5 text-gray-400" />
          </div>
          <div class="space-y-3">
            <div v-for="activity in recentActivities" :key="activity.id" class="flex items-center space-x-3">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 rounded-full flex items-center justify-center" :class="getActivityTypeClass(activity.type)">
                  <component :is="getActivityIcon(activity.type)" class="w-4 h-4" />
                </div>
              </div>
              <div class="flex-1 min-w-0">
                <p class="text-sm text-gray-900 dark:text-white truncate">{{ activity.description }}</p>
                <p class="text-xs text-gray-500 dark:text-gray-400">{{ formatTimeAgo(activity.timestamp) }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Module Navigation -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <!-- Purchase Orders -->
        <NuxtLink to="/purchasing/orders" class="block">
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6 hover:shadow-md transition-shadow">
            <div class="flex items-center mb-4">
              <div class="p-3 rounded-full bg-green-100 dark:bg-green-900/30">
                <ShoppingCartIcon class="w-8 h-8 text-green-600 dark:text-green-400" />
              </div>
              <div class="ml-4">
                <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Purchase Orders</h3>
                <p class="text-sm text-gray-600 dark:text-gray-400">Create and track purchase orders</p>
              </div>
            </div>
            <div class="grid grid-cols-2 gap-4 text-sm">
              <div>
                <span class="text-gray-600 dark:text-gray-400">Active:</span>
                <span class="font-medium ml-1">{{ orderStats.active }}</span>
              </div>
              <div>
                <span class="text-gray-600 dark:text-gray-400">Delivered:</span>
                <span class="font-medium ml-1">{{ orderStats.delivered }}</span>
              </div>
            </div>
          </div>
        </NuxtLink>

        <!-- Purchase Receipts -->
        <NuxtLink to="/purchasing/receipts" class="block">
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6 hover:shadow-md transition-shadow">
            <div class="flex items-center mb-4">
              <div class="p-3 rounded-full bg-yellow-100 dark:bg-yellow-900/30">
                <ClipboardDocumentCheckIcon class="w-8 h-8 text-yellow-600 dark:text-yellow-400" />
              </div>
              <div class="ml-4">
                <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Purchase Receipts</h3>
                <p class="text-sm text-gray-600 dark:text-gray-400">Record and verify deliveries</p>
              </div>
            </div>
            <div class="grid grid-cols-2 gap-4 text-sm">
              <div>
                <span class="text-gray-600 dark:text-gray-400">Pending QC:</span>
                <span class="font-medium ml-1">{{ receiptStats.pendingQC }}</span>
              </div>
              <div>
                <span class="text-gray-600 dark:text-gray-400">Accepted:</span>
                <span class="font-medium ml-1">{{ receiptStats.accepted }}</span>
              </div>
            </div>
          </div>
        </NuxtLink>

        <!-- Purchase Invoices -->
        <NuxtLink to="/purchasing/invoices" class="block">
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6 hover:shadow-md transition-shadow">
            <div class="flex items-center mb-4">
              <div class="p-3 rounded-full bg-purple-100 dark:bg-purple-900/30">
                <DocumentTextIcon class="w-8 h-8 text-purple-600 dark:text-purple-400" />
              </div>
              <div class="ml-4">
                <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Purchase Invoices</h3>
                <p class="text-sm text-gray-600 dark:text-gray-400">Process supplier invoices</p>
              </div>
            </div>
            <div class="grid grid-cols-2 gap-4 text-sm">
              <div>
                <span class="text-gray-600 dark:text-gray-400">Pending:</span>
                <span class="font-medium ml-1">{{ invoiceStats.pending }}</span>
              </div>
              <div>
                <span class="text-gray-600 dark:text-gray-400">Outstanding:</span>
                <span class="font-medium ml-1">${{ invoiceStats.outstanding }}K</span>
              </div>
            </div>
          </div>
        </NuxtLink>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import {
  PlusIcon,
  UserGroupIcon,
  QrCodeIcon,
  ShoppingCartIcon,
  BuildingOfficeIcon,
  ClipboardDocumentCheckIcon,
  DocumentTextIcon,
  CurrencyDollarIcon,
  ArrowRightIcon,
  ClockIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Purchasing Dashboard - TOSS ERP',
  meta: [
    { name: 'description', content: 'Comprehensive purchasing and procurement management dashboard in TOSS ERP' }
  ]
})

// Reactive data for metrics
const metrics = ref({
  activePOs: 24,
  suppliers: 45,
  pendingReceipts: 12,
  openInvoices: 8,
  monthlySpend: '125K',
  groupBuys: 3
})

const supplierStats = ref({
  active: 45,
  avgRating: 4.2,
  newThisMonth: 3
})

const requestStats = ref({
  pending: 15,
  groupBuyOpportunities: 7,
  approvedToday: 5
})

const orderStats = ref({
  active: 24,
  delivered: 142
})

const receiptStats = ref({
  pendingQC: 12,
  accepted: 89
})

const invoiceStats = ref({
  pending: 8,
  outstanding: 245
})

const recentActivities = ref([
  {
    id: 1,
    type: 'purchase_order',
    description: 'PO-2024-015 created for Tech Solutions Inc',
    timestamp: new Date(Date.now() - 1000 * 60 * 30) // 30 minutes ago
  },
  {
    id: 2,
    type: 'group_buy',
    description: 'Group buy for office supplies reached minimum quantity',
    timestamp: new Date(Date.now() - 1000 * 60 * 60 * 2) // 2 hours ago
  },
  {
    id: 3,
    type: 'receipt',
    description: 'Receipt RCP-2024-045 quality check completed',
    timestamp: new Date(Date.now() - 1000 * 60 * 60 * 4) // 4 hours ago
  },
  {
    id: 4,
    type: 'invoice',
    description: 'Invoice INV-2024-089 approved for payment',
    timestamp: new Date(Date.now() - 1000 * 60 * 60 * 6) // 6 hours ago
  },
  {
    id: 5,
    type: 'supplier',
    description: 'New supplier "Green Office Supplies" added',
    timestamp: new Date(Date.now() - 1000 * 60 * 60 * 24) // 1 day ago
  }
])

// Helper functions
const getActivityTypeClass = (type: string) => {
  const classes = {
    purchase_order: 'bg-green-100 text-green-600 dark:bg-green-900/30 dark:text-green-400',
    group_buy: 'bg-blue-100 text-blue-600 dark:bg-blue-900/30 dark:text-blue-400',
    receipt: 'bg-yellow-100 text-yellow-600 dark:bg-yellow-900/30 dark:text-yellow-400',
    invoice: 'bg-purple-100 text-purple-600 dark:bg-purple-900/30 dark:text-purple-400',
    supplier: 'bg-indigo-100 text-indigo-600 dark:bg-indigo-900/30 dark:text-indigo-400'
  }
  return classes[type as keyof typeof classes] || 'bg-gray-100 text-gray-600 dark:bg-gray-900/30 dark:text-gray-400'
}

const getActivityIcon = (type: string) => {
  const icons = {
    purchase_order: ShoppingCartIcon,
    group_buy: UserGroupIcon,
    receipt: ClipboardDocumentCheckIcon,
    invoice: DocumentTextIcon,
    supplier: BuildingOfficeIcon
  }
  return icons[type as keyof typeof icons] || ClockIcon
}

const formatTimeAgo = (timestamp: Date) => {
  const now = new Date()
  const diffInMinutes = Math.floor((now.getTime() - timestamp.getTime()) / (1000 * 60))
  
  if (diffInMinutes < 60) {
    return `${diffInMinutes}m ago`
  } else if (diffInMinutes < 1440) {
    return `${Math.floor(diffInMinutes / 60)}h ago`
  } else {
    return `${Math.floor(diffInMinutes / 1440)}d ago`
  }
}

// Action functions
const startGroupBuy = () => {
  navigateTo('/purchasing/requests?groupBuy=true')
}

const scanReceipt = () => {
  navigateTo('/purchasing/receipts?scan=true')
}

onMounted(() => {
  console.log('Purchasing Dashboard loaded')
})
</script>
