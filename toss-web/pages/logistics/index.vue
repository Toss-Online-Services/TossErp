<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Header -->
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-gray-900 dark:text-white">Logistics Management</h1>
        <p class="mt-2 text-gray-600 dark:text-gray-400">Manage shipping, carriers, warehousing, and supply chain operations</p>
      </div>

      <!-- Coming Soon Banner -->
      <div class="bg-blue-50 dark:bg-blue-900/20 border border-blue-200 dark:border-blue-800 rounded-lg p-6 text-center">
        <IconBase class="mx-auto h-12 w-12 text-blue-600 mb-4">
          <path d="M8 16.5a1.5 1.5 0 003 0V15h6v1.5a1.5 1.5 0 003 0V6.621a2 2 0 00-1.106-1.789L14.5 2.668A2 2 0 0013.382 2H10.618a2 2 0 00-1.118.668L5.106 4.832A2 2 0 004 6.621V16.5z" />
        </IconBase>
        <h2 class="text-xl font-semibold text-blue-900 dark:text-blue-100 mb-2">Coming Soon</h2>
        <p class="text-blue-700 dark:text-blue-300">
          The logistics management module is currently under development and will be available soon.
        </p>
      </div>

      <!-- Feature Preview -->
      <div class="mt-8 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Shipping Management</h3>
          <p class="text-gray-600 dark:text-gray-400">Track shipments, manage carriers, and optimize delivery routes.</p>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Warehouse Operations</h3>
          <p class="text-gray-600 dark:text-gray-400">Wave picking, cross-docking, and warehouse automation.</p>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Supply Chain Analytics</h3>
          <p class="text-gray-600 dark:text-gray-400">Insights, forecasting, and performance metrics.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
useHead({
  title: 'Logistics Management - TOSS ERP III'
})
</script>
