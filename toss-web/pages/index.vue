<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <!-- Main Container -->
    <div class="max-w-7xl mx-auto px-4 py-8 sm:px-6 lg:px-8">
      <!-- Header -->
      <div class="text-center mb-12">
        <h1 class="text-4xl font-bold text-slate-900 dark:text-white mb-3">TOSS</h1>
        <p class="text-xl text-slate-600 dark:text-slate-400">Township Operations Support System</p>
      </div>

      <!-- Quick Stats -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Today's Sales</p>
              <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">R 1,245</p>
              <p class="mt-1 text-sm text-green-600">+15% from yesterday</p>
            </div>
            <div class="p-3 bg-green-100 dark:bg-green-900/30 rounded-lg">
              <CurrencyDollarIcon class="w-8 h-8 text-green-600 dark:text-green-400" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Stock Items</p>
              <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">156</p>
              <p class="mt-1 text-sm text-orange-600">12 items low</p>
            </div>
            <div class="p-3 bg-blue-100 dark:bg-blue-900/30 rounded-lg">
              <ArchiveBoxIcon class="w-8 h-8 text-blue-600 dark:text-blue-400" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Pending Orders</p>
              <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">3</p>
              <p class="mt-1 text-sm text-yellow-600">1 arrives today</p>
            </div>
            <div class="p-3 bg-yellow-100 dark:bg-yellow-900/30 rounded-lg">
              <ShoppingCartIcon class="w-8 h-8 text-yellow-600 dark:text-yellow-400" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">This Month</p>
              <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">R 28.5K</p>
              <p class="mt-1 text-sm text-purple-600">Best month yet!</p>
            </div>
            <div class="p-3 bg-purple-100 dark:bg-purple-900/30 rounded-lg">
              <ArrowTrendingUpIcon class="w-8 h-8 text-purple-600 dark:text-purple-400" />
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
        <NuxtLink to="/dashboard" class="p-6 bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 hover:border-blue-500 dark:hover:border-blue-400 transition-all hover:shadow-md">
          <div class="flex items-center space-x-4">
            <div class="p-3 bg-blue-100 dark:bg-blue-900/30 rounded-lg">
              <ChartBarIcon class="w-8 h-8 text-blue-600 dark:text-blue-400" />
            </div>
            <div>
              <p class="text-lg font-bold text-slate-900 dark:text-white">Dashboard</p>
              <p class="text-sm text-slate-600 dark:text-slate-400">View metrics</p>
            </div>
                </div>
              </NuxtLink>

        <NuxtLink to="/purchasing" class="p-6 bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 hover:border-green-500 dark:hover:border-green-400 transition-all hover:shadow-md">
                <div class="flex items-center space-x-4">
            <div class="p-3 bg-green-100 dark:bg-green-900/30 rounded-lg">
              <ShoppingCartIcon class="w-8 h-8 text-green-600 dark:text-green-400" />
                  </div>
            <div>
              <p class="text-lg font-bold text-slate-900 dark:text-white">Purchasing</p>
              <p class="text-sm text-slate-600 dark:text-slate-400">Order stock</p>
                  </div>
                </div>
              </NuxtLink>

        <NuxtLink to="/stock" class="p-6 bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 hover:border-purple-500 dark:hover:border-purple-400 transition-all hover:shadow-md">
                <div class="flex items-center space-x-4">
            <div class="p-3 bg-purple-100 dark:bg-purple-900/30 rounded-lg">
              <ArchiveBoxIcon class="w-8 h-8 text-purple-600 dark:text-purple-400" />
            </div>
            <div>
              <p class="text-lg font-bold text-slate-900 dark:text-white">Stock</p>
              <p class="text-sm text-slate-600 dark:text-slate-400">Manage inventory</p>
            </div>
          </div>
        </NuxtLink>

        <NuxtLink to="/buying/group-buying" class="p-6 bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 hover:border-yellow-500 dark:hover:border-yellow-400 transition-all hover:shadow-md">
          <div class="flex items-center space-x-4">
            <div class="p-3 bg-yellow-100 dark:bg-yellow-900/30 rounded-lg">
              <UsersIcon class="w-8 h-8 text-yellow-600 dark:text-yellow-400" />
            </div>
            <div>
              <p class="text-lg font-bold text-slate-900 dark:text-white">Group Buy</p>
              <p class="text-sm text-slate-600 dark:text-slate-400">Save together</p>
            </div>
          </div>
        </NuxtLink>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { 
  CurrencyDollarIcon, 
  UsersIcon, 
  ArrowTrendingUpIcon,
  ArchiveBoxIcon,
  ShoppingCartIcon,
  ChartBarIcon
} from '@heroicons/vue/24/outline'

definePageMeta({
  layout: 'default'
})

useHead({
  title: 'TOSS - Township Operations Support System',
  meta: [
    { name: 'description', content: 'Manage your township business operations efficiently' }
  ]
})
</script>
