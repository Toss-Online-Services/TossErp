<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header with Glass Morphism -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 sm:py-6">
        <div class="flex items-center justify-between">
          <div class="flex-1 min-w-0">
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">
              Buying
            </h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
              Manage orders, suppliers, and group buying
            </p>
          </div>
          <div class="flex space-x-2 sm:space-x-3 flex-shrink-0">
            <NuxtLink 
              to="/buying/orders/create-order"
              class="inline-flex items-center justify-center px-4 sm:px-6 py-2.5 sm:py-3 bg-gradient-to-r from-blue-600 to-purple-600 text-white rounded-xl hover:from-blue-700 hover:to-purple-700 shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 font-semibold text-sm sm:text-base"
            >
              <ShoppingCartIcon class="w-5 h-5 mr-2" />
              Create Order
            </NuxtLink>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      
      <!-- Key Metrics -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Total Spend</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">R{{ metrics.totalSpend }}K</p>
              <p class="text-sm text-green-600 mt-1">+8.2% this month</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-blue-500 to-purple-600 rounded-xl">
              <ShoppingCartIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Cost Savings</p>
              <p class="text-3xl font-bold text-green-600">R{{ metrics.costSavings }}K</p>
              <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">15% reduction</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl">
              <CurrencyDollarIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Pending Orders</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ metrics.pendingOrders }}</p>
              <p class="text-sm text-orange-500 mt-1">Awaiting delivery</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-orange-500 to-red-600 rounded-xl">
              <ClockIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Active Suppliers</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ metrics.activeSuppliers }}</p>
              <p class="text-sm text-blue-600 mt-1">Verified partners</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-purple-500 to-pink-600 rounded-xl">
              <TruckIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>
      </div>

      <!-- Charts & Analytics -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
        <!-- Spending Trends Chart -->
        <div class="lg:col-span-2 bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-shadow duration-300">
          <div class="flex items-center justify-between mb-6">
            <div>
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Spending Trends</h3>
              <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">Last 7 days Buying activity</p>
            </div>
          </div>
          <LineChart
            :labels="['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']"
            :data="[45, 52, 48, 65, 58, 72, 68]"
            label="Spending (R thousands)"
            color="#3B82F6"
            :height="280"
          />
        </div>

        <!-- Order Status Distribution -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-shadow duration-300">
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-6">Order Status</h3>
          <div class="space-y-4">
            <div>
              <div class="flex justify-between text-sm mb-2">
                <span class="text-slate-600 dark:text-slate-400">Delivered</span>
                <span class="font-medium text-slate-900 dark:text-white">44%</span>
              </div>
              <div class="w-full bg-slate-200 rounded-full h-3 dark:bg-slate-700">
                <div class="bg-gradient-to-r from-green-500 to-emerald-600 h-3 rounded-full" style="width: 44%"></div>
              </div>
            </div>
            <div>
              <div class="flex justify-between text-sm mb-2">
                <span class="text-slate-600 dark:text-slate-400">In Transit</span>
                <span class="font-medium text-slate-900 dark:text-white">31%</span>
              </div>
              <div class="w-full bg-slate-200 rounded-full h-3 dark:bg-slate-700">
                <div class="bg-gradient-to-r from-blue-500 to-purple-600 h-3 rounded-full" style="width: 31%"></div>
              </div>
            </div>
            <div>
              <div class="flex justify-between text-sm mb-2">
                <span class="text-slate-600 dark:text-slate-400">Pending</span>
                <span class="font-medium text-slate-900 dark:text-white">25%</span>
              </div>
              <div class="w-full bg-slate-200 rounded-full h-3 dark:bg-slate-700">
                <div class="bg-gradient-to-r from-orange-500 to-red-600 h-3 rounded-full" style="width: 25%"></div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Supplier Performance & Category Breakdown -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        <!-- Top Suppliers Chart -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-shadow duration-300">
          <div class="flex items-center justify-between mb-6">
            <div>
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Top Suppliers by Spend</h3>
              <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">This month</p>
            </div>
          </div>
          <BarChart
            :labels="['ABC Suppliers', 'XYZ Wholesale', 'Quality Foods', 'Tech Solutions']"
            :data="[85, 72, 68, 45]"
            label="Spend (R thousands)"
            color="#8B5CF6"
            :height="280"
          />
        </div>

        <!-- Category Spending -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-shadow duration-300">
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-6">Spending by Category</h3>
          <div class="space-y-4">
            <div>
              <div class="flex justify-between text-sm mb-2">
                <span class="text-slate-600 dark:text-slate-400">🥘 Food & Beverages</span>
                <span class="font-medium text-slate-900 dark:text-white">R125K (40%)</span>
              </div>
              <div class="w-full bg-slate-200 rounded-full h-3 dark:bg-slate-700">
                <div class="bg-gradient-to-r from-blue-500 to-purple-600 h-3 rounded-full" style="width: 40%"></div>
              </div>
            </div>
            <div>
              <div class="flex justify-between text-sm mb-2">
                <span class="text-slate-600 dark:text-slate-400">📦 Supplies & Equipment</span>
                <span class="font-medium text-slate-900 dark:text-white">R82K (27%)</span>
              </div>
              <div class="w-full bg-slate-200 rounded-full h-3 dark:bg-slate-700">
                <div class="bg-gradient-to-r from-green-500 to-emerald-600 h-3 rounded-full" style="width: 27%"></div>
              </div>
            </div>
            <div>
              <div class="flex justify-between text-sm mb-2">
                <span class="text-slate-600 dark:text-slate-400">🏠 Household Items</span>
                <span class="font-medium text-slate-900 dark:text-white">R63K (20%)</span>
              </div>
              <div class="w-full bg-slate-200 rounded-full h-3 dark:bg-slate-700">
                <div class="bg-gradient-to-r from-yellow-500 to-orange-600 h-3 rounded-full" style="width: 20%"></div>
              </div>
            </div>
            <div>
              <div class="flex justify-between text-sm mb-2">
                <span class="text-slate-600 dark:text-slate-400">💊 Health & Beauty</span>
                <span class="font-medium text-slate-900 dark:text-white">R38K (13%)</span>
              </div>
              <div class="w-full bg-slate-200 rounded-full h-3 dark:bg-slate-700">
                <div class="bg-gradient-to-r from-pink-500 to-red-600 h-3 rounded-full" style="width: 13%"></div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
        <!-- Buy Orders -->
        <NuxtLink 
          to="/buying/orders"
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 group"
        >
          <div class="flex items-center mb-4">
            <div class="p-3 bg-gradient-to-br from-blue-100 to-blue-200 dark:from-blue-900/30 dark:to-blue-900/20 rounded-xl group-hover:scale-110 transition-transform">
              <ShoppingCartIcon class="w-8 h-8 text-blue-600 dark:text-blue-400" />
            </div>
            <div class="ml-4">
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Buy Orders</h3>
              <p class="text-sm text-slate-600 dark:text-slate-400">Manage your orders</p>
            </div>
          </div>
          <div class="space-y-2">
            <div class="flex justify-between text-sm">
              <span class="text-slate-600 dark:text-slate-400">Active Orders:</span>
              <span class="font-medium text-slate-900 dark:text-white">24</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-slate-600 dark:text-slate-400">In Transit:</span>
              <span class="font-medium text-blue-600">15</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-slate-600 dark:text-slate-400">Delivered Today:</span>
              <span class="font-medium text-green-600">6</span>
            </div>
          </div>
        </NuxtLink>

        <!-- Suppliers -->
        <NuxtLink 
          to="/buying/suppliers"
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 group"
        >
          <div class="flex items-center mb-4">
            <div class="p-3 bg-gradient-to-br from-purple-100 to-purple-200 dark:from-purple-900/30 dark:to-purple-900/20 rounded-xl group-hover:scale-110 transition-transform">
              <TruckIcon class="w-8 h-8 text-purple-600 dark:text-purple-400" />
            </div>
            <div class="ml-4">
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Suppliers</h3>
              <p class="text-sm text-slate-600 dark:text-slate-400">Manage procurement partners</p>
            </div>
          </div>
          <div class="space-y-2">
            <div class="flex justify-between text-sm">
              <span class="text-slate-600 dark:text-slate-400">Total Suppliers:</span>
              <span class="font-medium text-slate-900 dark:text-white">42</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-slate-600 dark:text-slate-400">Products Linked:</span>
              <span class="font-medium text-purple-600">180+</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-slate-600 dark:text-slate-400">On-time Delivery:</span>
              <span class="font-medium text-green-600">94%</span>
            </div>
          </div>
        </NuxtLink>

        <!-- Group Buying -->
        <NuxtLink 
          to="/buying/group-buying"
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 group"
        >
          <div class="flex items-center mb-4">
            <div class="p-3 bg-gradient-to-br from-green-100 to-green-200 dark:from-green-900/30 dark:to-green-900/20 rounded-xl group-hover:scale-110 transition-transform">
              <UserGroupIcon class="w-8 h-8 text-green-600 dark:text-green-400" />
            </div>
            <div class="ml-4">
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Group Buying</h3>
              <p class="text-sm text-slate-600 dark:text-slate-400">Buy together, save more</p>
            </div>
          </div>
          <div class="space-y-2">
            <div class="flex justify-between text-sm">
              <span class="text-slate-600 dark:text-slate-400">Active Groups:</span>
              <span class="font-medium text-slate-900 dark:text-white">5</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-slate-600 dark:text-slate-400">Network Savings:</span>
              <span class="font-medium text-green-600">R145K</span>
            </div>
            <div class="flex justify-between text-sm">
              <span class="text-slate-600 dark:text-slate-400">Your Participation:</span>
              <span class="font-medium text-blue-600">2 active</span>
            </div>
          </div>
        </NuxtLink>
      </div>

      <!-- Secondary Actions -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <!-- Create Order -->
        <NuxtLink 
          to="/buying/create-order"
          class="bg-gradient-to-r from-blue-500 to-purple-600 rounded-2xl shadow-lg p-6 hover:shadow-xl transition-all duration-300 group text-white"
        >
          <div class="flex items-center justify-between">
            <div>
              <h3 class="text-xl font-bold mb-2">Create Order</h3>
              <p class="text-blue-100 text-sm mb-4">AI-powered smart ordering</p>
              <button class="bg-white/20 hover:bg-white/30 backdrop-blur-sm px-4 py-2 rounded-lg text-sm font-medium transition-all duration-200">
                Start Ordering →
              </button>
            </div>
            <div class="p-4 bg-white/20 rounded-xl group-hover:scale-110 transition-transform">
              <ShoppingCartIcon class="w-12 h-12 text-white" />
            </div>
          </div>
        </NuxtLink>

        <!-- Track Orders -->
        <NuxtLink 
          to="/buying/track-orders"
          class="bg-gradient-to-r from-green-500 to-emerald-600 rounded-2xl shadow-lg p-6 hover:shadow-xl transition-all duration-300 group text-white"
        >
          <div class="flex items-center justify-between">
            <div>
              <h3 class="text-xl font-bold mb-2">Track Orders</h3>
              <p class="text-green-100 text-sm mb-4">Monitor delivery status</p>
              <button class="bg-white/20 hover:bg-white/30 backdrop-blur-sm px-4 py-2 rounded-lg text-sm font-medium transition-all duration-200">
                View Tracking →
              </button>
            </div>
            <div class="p-4 bg-white/20 rounded-xl group-hover:scale-110 transition-transform">
              <TruckIcon class="w-12 h-12 text-white" />
            </div>
          </div>
        </NuxtLink>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import {
  ShoppingCartIcon,
  CurrencyDollarIcon,
  ClockIcon,
  TruckIcon,
  UserGroupIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Buying - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage Buying, orders, suppliers, and group buying in TOSS ERP' }
  ]
})

// Metrics
const metrics = ref({
  totalSpend: 245,
  costSavings: 32,
  pendingOrders: 18,
  activeSuppliers: 42
})
</script>

