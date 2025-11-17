<script setup lang="ts">
import {
  ArchiveBoxIcon,
  ArrowTrendingUpIcon,
  ChartBarIcon,
  CurrencyDollarIcon,
  ShoppingCartIcon,
  UsersIcon,
} from '@heroicons/vue/24/outline'

// @ts-ignore -- Nuxt auto-injects useHead
useHead({
  title: 'TOSS - Township Operations Support System',
  meta: [
    { name: 'description', content: 'Manage your township business operations efficiently' },
  ],
})
</script>

<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <div class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
      <div class="mb-12 text-center">
        <h1 class="mb-3 text-4xl font-bold text-slate-900 dark:text-white">TOSS</h1>
        <p class="text-xl text-slate-600 dark:text-slate-400">Township Operations Support System</p>
      </div>

      <div class="mb-8 grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-4">
        <div class="rounded-lg border border-slate-200 bg-white p-6 shadow-sm dark:border-slate-700 dark:bg-slate-800">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Today's Sales</p>
              <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">R 1,245</p>
              <p class="mt-1 text-sm text-green-600">+15% from yesterday</p>
            </div>
            <div class="rounded-lg bg-green-100 p-3 dark:bg-green-900/30">
              <CurrencyDollarIcon class="h-8 w-8 text-green-600 dark:text-green-400" />
            </div>
          </div>
        </div>

        <div class="rounded-lg border border-slate-200 bg-white p-6 shadow-sm dark:border-slate-700 dark:bg-slate-800">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Stock Items</p>
              <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">156</p>
              <p class="mt-1 text-sm text-orange-600">12 items low</p>
            </div>
            <div class="rounded-lg bg-blue-100 p-3 dark:bg-blue-900/30">
              <ArchiveBoxIcon class="h-8 w-8 text-blue-600 dark:text-blue-400" />
            </div>
          </div>
        </div>

        <div class="rounded-lg border border-slate-200 bg-white p-6 shadow-sm dark:border-slate-700 dark:bg-slate-800">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Pending Orders</p>
              <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">3</p>
              <p class="mt-1 text-sm text-yellow-600">1 arrives today</p>
            </div>
            <div class="rounded-lg bg-yellow-100 p-3 dark:bg-yellow-900/30">
              <ShoppingCartIcon class="h-8 w-8 text-yellow-600 dark:text-yellow-400" />
            </div>
          </div>
        </div>

        <div class="rounded-lg border border-slate-200 bg-white p-6 shadow-sm dark:border-slate-700 dark:bg-slate-800">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">This Month</p>
              <p class="mt-2 text-3xl font-bold text-slate-900 dark:text-white">R 28.5K</p>
              <p class="mt-1 text-sm text-purple-600">Best month yet!</p>
            </div>
            <div class="rounded-lg bg-purple-100 p-3 dark:bg-purple-900/30">
              <ArrowTrendingUpIcon class="h-8 w-8 text-purple-600 dark:text-purple-400" />
            </div>
          </div>
        </div>
      </div>

      <div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
        <NuxtLink
          to="/dashboard"
          class="flex items-center space-x-4 rounded-lg border border-slate-200 bg-white p-6 shadow-sm transition-all hover:border-blue-500 hover:shadow-md dark:border-slate-700 dark:bg-slate-800 dark:hover:border-blue-400"
        >
          <div class="rounded-lg bg-blue-100 p-3 dark:bg-blue-900/30">
            <ChartBarIcon class="h-8 w-8 text-blue-600 dark:text-blue-400" />
          </div>
          <div>
            <p class="text-lg font-bold text-slate-900 dark:text-white">Dashboard</p>
            <p class="text-sm text-slate-600 dark:text-slate-400">View metrics</p>
          </div>
        </NuxtLink>

        <NuxtLink
          to="/buying/group-buying"
          class="flex items-center space-x-4 rounded-lg border border-slate-200 bg-white p-6 shadow-sm transition-all hover:border-green-500 hover:shadow-md dark:border-slate-700 dark:bg-slate-800 dark:hover:border-green-400"
        >
          <div class="rounded-lg bg-green-100 p-3 dark:bg-green-900/30">
            <ShoppingCartIcon class="h-8 w-8 text-green-600 dark:text-green-400" />
          </div>
          <div>
            <p class="text-lg font-bold text-slate-900 dark:text-white">Purchasing</p>
            <p class="text-sm text-slate-600 dark:text-slate-400">Order stock</p>
          </div>
        </NuxtLink>

        <NuxtLink
          to="/stock"
          class="flex items-center space-x-4 rounded-lg border border-slate-200 bg-white p-6 shadow-sm transition-all hover:border-purple-500 hover:shadow-md dark:border-slate-700 dark:bg-slate-800 dark:hover:border-purple-400"
        >
          <div class="rounded-lg bg-purple-100 p-3 dark:bg-purple-900/30">
            <ArchiveBoxIcon class="h-8 w-8 text-purple-600 dark:text-purple-400" />
          </div>
          <div>
            <p class="text-lg font-bold text-slate-900 dark:text-white">Stock</p>
            <p class="text-sm text-slate-600 dark:text-slate-400">Manage inventory</p>
          </div>
        </NuxtLink>

        <NuxtLink
          to="/crm/customers"
          class="flex items-center space-x-4 rounded-lg border border-slate-200 bg-white p-6 shadow-sm transition-all hover:border-yellow-500 hover:shadow-md dark:border-slate-700 dark:bg-slate-800 dark:hover:border-yellow-400"
        >
          <div class="rounded-lg bg-yellow-100 p-3 dark:bg-yellow-900/30">
            <UsersIcon class="h-8 w-8 text-yellow-600 dark:text-yellow-400" />
          </div>
          <div>
            <p class="text-lg font-bold text-slate-900 dark:text-white">Group Buy</p>
            <p class="text-sm text-slate-600 dark:text-slate-400">Save together</p>
          </div>
        </NuxtLink>
      </div>

      <div class="mt-12 flex flex-col items-center justify-center gap-4 sm:flex-row">
        <Button size="lg" class="px-10" as-child>
          <NuxtLink to="/auth/register">Get Started</NuxtLink>
        </Button>
        <Button size="lg" variant="outline" class="px-10" as-child>
          <NuxtLink to="/auth/login">Sign In</NuxtLink>
        </Button>
      </div>
    </div>
  </div>
</template>
