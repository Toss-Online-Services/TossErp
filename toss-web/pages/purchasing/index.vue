<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Purchasing & Procurement Dashboard</h1>
              <p class="text-gray-600 dark:text-gray-400">Complete procurement lifecycle management with TOSS collaborative features</p>
            </div>
            <div class="flex space-x-3">
              <button @click="startGroupBuy" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center">
                <UserGroupIcon class="w-5 h-5 mr-2" />
                Start Group Buy
              </button>
              <button @click="quickScan" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center">
                <QrCodeIcon class="w-5 h-5 mr-2" />
                Quick Scan
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Dashboard Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <!-- Key Metrics -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-blue-100 dark:bg-blue-900/30">
              <ShoppingCartIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total Spend</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">${{ metrics.totalSpend }}K</p>
            </div>
          </div>
        </div>
        
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-green-100 dark:bg-green-900/30">
              <CurrencyDollarIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Cost Savings</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">${{ metrics.costSavings }}K</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-yellow-100 dark:bg-yellow-900/30">
              <ClockIcon class="w-6 h-6 text-yellow-600 dark:text-yellow-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Pending Orders</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ metrics.pendingOrders }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-purple-100 dark:bg-purple-900/30">
              <TruckIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Active Suppliers</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ metrics.activeSuppliers }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- TOSS Collaboration Features -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <!-- Group Buying Opportunities -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center mb-4">
            <div class="p-3 rounded-full bg-blue-100 dark:bg-blue-900/30">
              <UserGroupIcon class="w-8 h-8 text-blue-600 dark:text-blue-400" />
            </div>
            <div class="ml-4">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Group Buying</h3>
              <p class="text-sm text-gray-600 dark:text-gray-400">Collaborative procurement</p>
            </div>
          </div>
          <div class="space-y-2 mb-4">
            <div class="flex justify-between text-sm">
              <span class="text-gray-600 dark:text-gray-400">Active Group Buys:</span>
              <span class="font-medium">23</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-gray-600 dark:text-gray-400">Network Savings:</span>
              <span class="font-medium text-green-600">$485K</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-gray-600 dark:text-gray-400">Your Participation:</span>
              <span class="font-medium">2 active</span>
            </div>
          </div>
          <NuxtLink to="/purchasing/group-buying" class="block w-full text-center bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition-colors">
            Explore Group Buys
          </NuxtLink>
        </div>

        <!-- Shared Assets -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center mb-4">
            <div class="p-3 rounded-full bg-orange-100 dark:bg-orange-900/30">
              <TruckIcon class="w-8 h-8 text-orange-600 dark:text-orange-400" />
            </div>
            <div class="ml-4">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Asset Sharing</h3>
              <p class="text-sm text-gray-600 dark:text-gray-400">Network equipment pool</p>
            </div>
          </div>
          <div class="space-y-2 mb-4">
            <div class="flex justify-between text-sm">
              <span class="text-gray-600 dark:text-gray-400">Available Assets:</span>
              <span class="font-medium">156</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-gray-600 dark:text-gray-400">Cost Savings:</span>
              <span class="font-medium text-green-600">$125K</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-gray-600 dark:text-gray-400">Your Bookings:</span>
              <span class="font-medium">3 upcoming</span>
            </div>
          </div>
          <NuxtLink to="/purchasing/asset-sharing" class="block w-full text-center bg-orange-600 text-white py-2 rounded-lg hover:bg-orange-700 transition-colors">
            Browse Assets
          </NuxtLink>
        </div>

        <!-- Pooled Credit -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center mb-4">
            <div class="p-3 rounded-full bg-green-100 dark:bg-green-900/30">
              <BanknotesIcon class="w-8 h-8 text-green-600 dark:text-green-400" />
            </div>
            <div class="ml-4">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Pooled Credit</h3>
              <p class="text-sm text-gray-600 dark:text-gray-400">Mutual financing network</p>
            </div>
          </div>
          <div class="space-y-2 mb-4">
            <div class="flex justify-between text-sm">
              <span class="text-gray-600 dark:text-gray-400">Available Credit:</span>
              <span class="font-medium">$245K</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-gray-600 dark:text-gray-400">Your Credit Limit:</span>
              <span class="font-medium text-blue-600">$75K</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-gray-600 dark:text-gray-400">Network Rate:</span>
              <span class="font-medium">6.2% APR</span>
            </div>
          </div>
          <NuxtLink to="/purchasing/pooled-credit" class="block w-full text-center bg-green-600 text-white py-2 rounded-lg hover:bg-green-700 transition-colors">
            Access Credit Pool
          </NuxtLink>
        </div>
      </div>

      <!-- Core Purchasing Features -->
      <div class="mb-8">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Core Purchasing Features</h3>
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
          <NuxtLink to="/purchasing/material-requests" class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4 hover:shadow-md transition-shadow">
            <div class="flex items-center space-x-3">
              <div class="p-2 rounded-lg bg-blue-100 dark:bg-blue-900/30">
                <ClipboardDocumentListIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
              </div>
              <div>
                <p class="text-sm font-semibold text-gray-900 dark:text-white">Material Requests</p>
                <p class="text-xs text-gray-600 dark:text-gray-400">Department requisitions</p>
              </div>
            </div>
          </NuxtLink>

          <NuxtLink to="/purchasing/rfq" class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4 hover:shadow-md transition-shadow">
            <div class="flex items-center space-x-3">
              <div class="p-2 rounded-lg bg-purple-100 dark:bg-purple-900/30">
                <DocumentTextIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
              </div>
              <div>
                <p class="text-sm font-semibold text-gray-900 dark:text-white">Request for Quotation</p>
                <p class="text-xs text-gray-600 dark:text-gray-400">Multi-supplier RFQ</p>
              </div>
            </div>
          </NuxtLink>

          <NuxtLink to="/purchasing/supplier-quotations" class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4 hover:shadow-md transition-shadow">
            <div class="flex items-center space-x-3">
              <div class="p-2 rounded-lg bg-green-100 dark:bg-green-900/30">
                <ClipboardDocumentCheckIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
              </div>
              <div>
                <p class="text-sm font-semibold text-gray-900 dark:text-white">Supplier Quotations</p>
                <p class="text-xs text-gray-600 dark:text-gray-400">Compare & award</p>
              </div>
            </div>
          </NuxtLink>

          <NuxtLink to="/purchasing/blanket-orders" class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4 hover:shadow-md transition-shadow">
            <div class="flex items-center space-x-3">
              <div class="p-2 rounded-lg bg-orange-100 dark:bg-orange-900/30">
                <DocumentDuplicateIcon class="w-6 h-6 text-orange-600 dark:text-orange-400" />
              </div>
              <div>
                <p class="text-sm font-semibold text-gray-900 dark:text-white">Blanket Orders</p>
                <p class="text-xs text-gray-600 dark:text-gray-400">Long-term agreements</p>
              </div>
            </div>
          </NuxtLink>
        </div>
      </div>

      <!-- Module Navigation -->
      <div class="mb-8">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Procurement Management</h3>
        <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <!-- Suppliers -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
            <div class="flex items-center mb-4">
              <div class="p-3 rounded-full bg-purple-100 dark:bg-purple-900/30">
                <BuildingOfficeIcon class="w-8 h-8 text-purple-600 dark:text-purple-400" />
              </div>
              <div class="ml-4">
                <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Supplier Management</h3>
                <p class="text-sm text-gray-600 dark:text-gray-400">Vendor relationships & performance</p>
              </div>
            </div>
            <div class="space-y-2 mb-4">
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">Total Suppliers:</span>
                <span class="font-medium">42</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">Avg Rating:</span>
                <span class="font-medium">4.2/5</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">On-time Delivery:</span>
                <span class="font-medium text-green-600">94%</span>
              </div>
            </div>
            <NuxtLink to="/purchasing/suppliers" class="block w-full text-center bg-purple-600 text-white py-2 rounded-lg hover:bg-purple-700 transition-colors">
              Manage Suppliers
            </NuxtLink>
          </div>

          <!-- Purchase Requests -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
            <div class="flex items-center mb-4">
              <div class="p-3 rounded-full bg-blue-100 dark:bg-blue-900/30">
                <DocumentTextIcon class="w-8 h-8 text-blue-600 dark:text-blue-400" />
              </div>
              <div class="ml-4">
                <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Purchase Requests</h3>
                <p class="text-sm text-gray-600 dark:text-gray-400">Request approval workflow</p>
              </div>
            </div>
            <div class="space-y-2 mb-4">
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">Pending Approval:</span>
                <span class="font-medium text-yellow-600">8</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">Approved Today:</span>
                <span class="font-medium">12</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">Avg Processing:</span>
                <span class="font-medium">2.3 days</span>
              </div>
            </div>
            <NuxtLink to="/purchasing/requests" class="block w-full text-center bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 transition-colors">
              View Requests
            </NuxtLink>
          </div>

          <!-- Purchase Orders -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
            <div class="flex items-center mb-4">
              <div class="p-3 rounded-full bg-green-100 dark:bg-green-900/30">
                <ShoppingCartIcon class="w-8 h-8 text-green-600 dark:text-green-400" />
              </div>
              <div class="ml-4">
                <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Purchase Orders</h3>
                <p class="text-sm text-gray-600 dark:text-gray-400">Order lifecycle management</p>
              </div>
            </div>
            <div class="space-y-2 mb-4">
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">Active Orders:</span>
                <span class="font-medium">24</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">In Transit:</span>
                <span class="font-medium text-blue-600">15</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">Delivered Today:</span>
                <span class="font-medium text-green-600">6</span>
              </div>
            </div>
            <NuxtLink to="/purchasing/orders" class="block w-full text-center bg-green-600 text-white py-2 rounded-lg hover:bg-green-700 transition-colors">
              Manage Orders
            </NuxtLink>
          </div>
        </div>
      </div>

      <!-- Secondary Modules -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
        <!-- Receipts & Quality Control -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center mb-4">
            <div class="p-3 rounded-full bg-orange-100 dark:bg-orange-900/30">
              <ClipboardDocumentCheckIcon class="w-8 h-8 text-orange-600 dark:text-orange-400" />
            </div>
            <div class="ml-4">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Receipts & Quality Control</h3>
              <p class="text-sm text-gray-600 dark:text-gray-400">Receiving and inspection workflow</p>
            </div>
          </div>
          <div class="grid grid-cols-2 gap-4 mb-4">
            <div class="text-center">
              <p class="text-2xl font-bold text-orange-600">18</p>
              <p class="text-xs text-gray-600 dark:text-gray-400">Pending Receipt</p>
            </div>
            <div class="text-center">
              <p class="text-2xl font-bold text-green-600">96%</p>
              <p class="text-xs text-gray-600 dark:text-gray-400">Quality Pass Rate</p>
            </div>
          </div>
          <NuxtLink to="/purchasing/receipts" class="block w-full text-center bg-orange-600 text-white py-2 rounded-lg hover:bg-orange-700 transition-colors">
            Process Receipts
          </NuxtLink>
        </div>

        <!-- Invoices & Payments -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center mb-4">
            <div class="p-3 rounded-full bg-indigo-100 dark:bg-indigo-900/30">
              <CreditCardIcon class="w-8 h-8 text-indigo-600 dark:text-indigo-400" />
            </div>
            <div class="ml-4">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Invoices & Payments</h3>
              <p class="text-sm text-gray-600 dark:text-gray-400">Financial processing & three-way matching</p>
            </div>
          </div>
          <div class="grid grid-cols-2 gap-4 mb-4">
            <div class="text-center">
              <p class="text-2xl font-bold text-indigo-600">12</p>
              <p class="text-xs text-gray-600 dark:text-gray-400">Awaiting Payment</p>
            </div>
            <div class="text-center">
              <p class="text-2xl font-bold text-green-600">$45K</p>
              <p class="text-xs text-gray-600 dark:text-gray-400">Paid This Month</p>
            </div>
          </div>
          <NuxtLink to="/purchasing/invoices" class="block w-full text-center bg-indigo-600 text-white py-2 rounded-lg hover:bg-indigo-700 transition-colors">
            Manage Invoices
          </NuxtLink>
        </div>
      </div>

      <!-- Recent Activity & Quick Actions -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Recent Activity Feed -->
        <div class="lg:col-span-2 bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Recent Activity</h3>
          <div class="space-y-4">
            <div v-for="activity in recentActivity" :key="activity.id" class="flex items-start space-x-3">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 rounded-full flex items-center justify-center" :class="getActivityIconBg(activity.type)">
                  <component :is="getActivityIcon(activity.type)" class="w-4 h-4" :class="getActivityIconColor(activity.type)" />
                </div>
              </div>
              <div class="flex-1 min-w-0">
                <p class="text-sm text-gray-900 dark:text-white">{{ activity.description }}</p>
                <p class="text-xs text-gray-600 dark:text-gray-400">{{ formatTimeAgo(activity.timestamp) }}</p>
              </div>
              <div class="flex-shrink-0">
                <span class="text-xs px-2 py-1 rounded-full" :class="getActivityStatusClass(activity.type)">
                  {{ activity.status }}
                </span>
              </div>
            </div>
          </div>
        </div>

        <!-- Quick Actions & Network Status -->
        <div class="space-y-6">
          <!-- Quick Actions -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Quick Actions</h3>
            <div class="space-y-3">
              <button @click="createRequest" class="w-full text-left p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
                <div class="flex items-center">
                  <PlusIcon class="w-5 h-5 text-blue-600 mr-3" />
                  <span class="text-sm font-medium text-gray-900 dark:text-white">Create Purchase Request</span>
                </div>
              </button>
              <button @click="scanReceipt" class="w-full text-left p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
                <div class="flex items-center">
                  <QrCodeIcon class="w-5 h-5 text-green-600 mr-3" />
                  <span class="text-sm font-medium text-gray-900 dark:text-white">Scan Receipt/QR Code</span>
                </div>
              </button>
              <button @click="viewAnalytics" class="w-full text-left p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors">
                <div class="flex items-center">
                  <ChartBarIcon class="w-5 h-5 text-purple-600 mr-3" />
                  <span class="text-sm font-medium text-gray-900 dark:text-white">View Analytics</span>
                </div>
              </button>
            </div>
          </div>

          <!-- TOSS Network Status -->
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">TOSS Network Status</h3>
            <div class="space-y-3">
              <div class="flex items-center justify-between">
                <span class="text-sm text-gray-600 dark:text-gray-400">Network Health</span>
                <div class="flex items-center">
                  <div class="w-2 h-2 bg-green-400 rounded-full mr-2"></div>
                  <span class="text-sm font-medium text-green-600">Excellent</span>
                </div>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-sm text-gray-600 dark:text-gray-400">Active Members</span>
                <span class="text-sm font-medium text-gray-900 dark:text-white">1,247</span>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-sm text-gray-600 dark:text-gray-400">Your Trust Score</span>
                <div class="flex items-center">
                  <StarIcon v-for="i in 5" :key="i" class="w-3 h-3" :class="i <= 4 ? 'text-yellow-400' : 'text-gray-300'" />
                </div>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-sm text-gray-600 dark:text-gray-400">Network Savings</span>
                <span class="text-sm font-medium text-green-600">+22% this month</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import {
  ShoppingCartIcon,
  CurrencyDollarIcon,
  ClockIcon,
  TruckIcon,
  UserGroupIcon,
  BanknotesIcon,
  BuildingOfficeIcon,
  DocumentTextIcon,
  ClipboardDocumentCheckIcon,
  ClipboardDocumentListIcon,
  DocumentDuplicateIcon,
  CreditCardIcon,
  PlusIcon,
  QrCodeIcon,
  ChartBarIcon,
  StarIcon,
  CheckCircleIcon,
  ExclamationTriangleIcon,
  InformationCircleIcon
} from '@heroicons/vue/24/outline'

// Reactive data
const metrics = ref({
  totalSpend: 245,
  costSavings: 32,
  pendingOrders: 18,
  activeSuppliers: 42
})

const recentActivity = ref([
  {
    id: 1,
    type: 'group_buy',
    description: 'Joined office supplies group buy with 12 other members',
    status: 'Active',
    timestamp: new Date(Date.now() - 2 * 60 * 60 * 1000)
  },
  {
    id: 2,
    type: 'order',
    description: 'Purchase order PO-2025-0089 delivered successfully',
    status: 'Completed',
    timestamp: new Date(Date.now() - 4 * 60 * 60 * 1000)
  },
  {
    id: 3,
    type: 'invoice',
    description: 'Invoice INV-2025-0234 approved for payment',
    status: 'Approved',
    timestamp: new Date(Date.now() - 6 * 60 * 60 * 1000)
  },
  {
    id: 4,
    type: 'asset',
    description: 'Booked industrial 3D printer from TechFab Solutions',
    status: 'Confirmed',
    timestamp: new Date(Date.now() - 8 * 60 * 60 * 1000)
  },
  {
    id: 5,
    type: 'credit',
    description: 'Loan application for equipment purchase approved',
    status: 'Approved',
    timestamp: new Date(Date.now() - 12 * 60 * 60 * 1000)
  }
])

// Helper functions
const getActivityIcon = (type: string) => {
  const icons = {
    group_buy: UserGroupIcon,
    order: ShoppingCartIcon,
    invoice: CreditCardIcon,
    asset: TruckIcon,
    credit: BanknotesIcon,
    request: DocumentTextIcon,
    receipt: ClipboardDocumentCheckIcon
  }
  return icons[type as keyof typeof icons] || InformationCircleIcon
}

const getActivityIconBg = (type: string) => {
  const colors = {
    group_buy: 'bg-blue-100 dark:bg-blue-900/30',
    order: 'bg-green-100 dark:bg-green-900/30',
    invoice: 'bg-purple-100 dark:bg-purple-900/30',
    asset: 'bg-orange-100 dark:bg-orange-900/30',
    credit: 'bg-yellow-100 dark:bg-yellow-900/30',
    request: 'bg-indigo-100 dark:bg-indigo-900/30',
    receipt: 'bg-teal-100 dark:bg-teal-900/30'
  }
  return colors[type as keyof typeof colors] || 'bg-gray-100 dark:bg-gray-900/30'
}

const getActivityIconColor = (type: string) => {
  const colors = {
    group_buy: 'text-blue-600 dark:text-blue-400',
    order: 'text-green-600 dark:text-green-400',
    invoice: 'text-purple-600 dark:text-purple-400',
    asset: 'text-orange-600 dark:text-orange-400',
    credit: 'text-yellow-600 dark:text-yellow-400',
    request: 'text-indigo-600 dark:text-indigo-400',
    receipt: 'text-teal-600 dark:text-teal-400'
  }
  return colors[type as keyof typeof colors] || 'text-gray-600 dark:text-gray-400'
}

const getActivityStatusClass = (type: string) => {
  const classes = {
    group_buy: 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
    order: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    invoice: 'bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400',
    asset: 'bg-orange-100 text-orange-800 dark:bg-orange-900/30 dark:text-orange-400',
    credit: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    request: 'bg-indigo-100 text-indigo-800 dark:bg-indigo-900/30 dark:text-indigo-400',
    receipt: 'bg-teal-100 text-teal-800 dark:bg-teal-900/30 dark:text-teal-400'
  }
  return classes[type as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const formatTimeAgo = (date: Date) => {
  const now = new Date()
  const diffMs = now.getTime() - date.getTime()
  const diffHours = Math.floor(diffMs / (1000 * 60 * 60))
  const diffDays = Math.floor(diffHours / 24)
  
  if (diffDays > 0) {
    return `${diffDays} day${diffDays > 1 ? 's' : ''} ago`
  } else if (diffHours > 0) {
    return `${diffHours} hour${diffHours > 1 ? 's' : ''} ago`
  } else {
    return 'Just now'
  }
}

// Action functions
const startGroupBuy = () => {
  console.log('Starting group buy')
  window.location.href = '/purchasing/requests?groupBuy=true'
}

const quickScan = () => {
  console.log('Quick scan initiated')
  window.location.href = '/purchasing/receipts?scan=true'
}

const createRequest = () => {
  console.log('Creating purchase request')
  window.location.href = '/purchasing/requests'
}

const scanReceipt = () => {
  console.log('Scanning receipt')
  window.location.href = '/purchasing/receipts?scan=true'
}

const viewAnalytics = () => {
  console.log('Viewing analytics')
  navigateTo('/purchasing/analytics')
}

onMounted(() => {
  console.log('Purchasing & Procurement Dashboard loaded with TOSS collaboration features')
})
</script>