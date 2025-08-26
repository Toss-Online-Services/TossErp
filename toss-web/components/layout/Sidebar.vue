<template>
  <aside class="w-64 flex-shrink-0 bg-white dark:bg-gray-800 border-r border-gray-200 dark:border-gray-700 flex flex-col">
    <div class="h-16 flex items-center justify-center px-4">
      <img src="/logo.svg" alt="TOSS ERP" class="h-8 w-auto">
      <h1 class="text-2xl font-bold ml-2">TOSS ERP</h1>
    </div>
    <nav class="flex-1 px-2 py-4 space-y-1">
      <NuxtLink to="/" class="flex items-center px-2 py-2 text-sm font-medium rounded-md hover:bg-gray-100 dark:hover:bg-gray-700">
        <HomeIcon class="h-6 w-6 mr-3" />
        Dashboard
      </NuxtLink>
      <NuxtLink to="/crm" class="flex items-center px-2 py-2 text-sm font-medium rounded-md hover:bg-gray-100 dark:hover:bg-gray-700">
        <UsersIcon class="h-6 w-6 mr-3" />
        CRM
      </NuxtLink>
      <NuxtLink to="/projects" class="flex items-center px-2 py-2 text-sm font-medium rounded-md hover:bg-gray-100 dark:hover:bg-gray-700">
        <BriefcaseIcon class="h-6 w-6 mr-3" />
        Projects
      </NuxtLink>
      <NuxtLink to="/accounts" class="flex items-center px-2 py-2 text-sm font-medium rounded-md hover:bg-gray-100 dark:hover:bg-gray-700">
        <CurrencyDollarIcon class="h-6 w-6 mr-3" />
        Accounts
      </NuxtLink>
      <NuxtLink to="/hr" class="flex items-center px-2 py-2 text-sm font-medium rounded-md hover:bg-gray-100 dark:hover:bg-gray-700">
        <UserGroupIcon class="h-6 w-6 mr-3" />
        HR
      </NuxtLink>
      <NuxtLink to="/stock" class="flex items-center px-2 py-2 text-sm font-medium rounded-md hover:bg-gray-100 dark:hover:bg-gray-700">
        <ArchiveIcon class="h-6 w-6 mr-3" />
        Stock
      </NuxtLink>
      <NuxtLink to="/selling" class="flex items-center px-2 py-2 text-sm font-medium rounded-md hover:bg-gray-100 dark:hover:bg-gray-700">
        <ShoppingCartIcon class="h-6 w-6 mr-3" />
        Selling
      </NuxtLink>
      <NuxtLink to="/buying" class="flex items-center px-2 py-2 text-sm font-medium rounded-md hover:bg-gray-100 dark:hover:bg-gray-700">
        <ShoppingBagIcon class="h-6 w-6 mr-3" />
        Buying
      </NuxtLink>
    </nav>
  </aside>
</template>

<script setup>
import { HomeIcon, UsersIcon, BriefcaseIcon, CurrencyDollarIcon, UserGroupIcon, ArchiveIcon, ShoppingCartIcon, ShoppingBagIcon } from '@heroicons/vue/24/outline'
</script>
