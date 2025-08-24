<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Header -->
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-gray-900 dark:text-white">Reports & Analytics</h1>
        <p class="mt-2 text-gray-600 dark:text-gray-400">Business intelligence, financial reports, and performance analytics</p>
      </div>

      <!-- Coming Soon Banner -->
      <div class="bg-orange-50 dark:bg-orange-900/20 border border-orange-200 dark:border-orange-800 rounded-lg p-6 text-center">
        <IconBase class="mx-auto h-12 w-12 text-orange-600 mb-4">
          <path d="M3 13.125C3 12.504 3.504 12 4.125 12h2.25c.621 0 1.125.504 1.125 1.125v6.75C7.5 20.496 6.996 21 6.375 21h-2.25A1.125 1.125 0 0 1 3 19.875v-6.75ZM9.75 8.625c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125v11.25c0 .621-.504 1.125-1.125 1.125h-2.25a1.125 1.125 0 0 1-1.125-1.125V8.625ZM16.5 4.125c0-.621.504-1.125 1.125-1.125h2.25C20.496 3 21 3.504 21 4.125v15.75c0 .621-.504 1.125-1.125 1.125h-2.25a1.125 1.125 0 0 1-1.125-1.125V4.125Z" />
        </IconBase>
        <h2 class="text-xl font-semibold text-orange-900 dark:text-orange-100 mb-2">Coming Soon</h2>
        <p class="text-orange-700 dark:text-orange-300">
          The reports and analytics module is currently under development and will be available soon.
        </p>
      </div>

      <!-- Feature Preview -->
      <div class="mt-8 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Financial Reports</h3>
          <p class="text-gray-600 dark:text-gray-400">P&L, balance sheets, cash flow, and financial analysis.</p>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Business Intelligence</h3>
          <p class="text-gray-600 dark:text-gray-400">KPIs, trends, forecasting, and performance metrics.</p>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Custom Dashboards</h3>
          <p class="text-gray-600 dark:text-gray-400">Build custom reports and interactive dashboards.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
useHead({
  title: 'Reports & Analytics - TOSS ERP III'
})
</script>
