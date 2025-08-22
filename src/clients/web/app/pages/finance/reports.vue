<template>
  <div class="container mx-auto px-4 py-8">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">
        Financial Reports
      </h1>
      <p class="text-gray-600 dark:text-gray-300">
        Comprehensive financial reporting and analysis
      </p>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      <!-- Profit & Loss Report -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
        <h3 class="text-lg font-semibold mb-4">Profit & Loss</h3>
        <p class="text-gray-600 dark:text-gray-400 mb-4">
          Monthly and yearly P&L statements
        </p>
        <button class="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700">
          Generate Report
        </button>
      </div>

      <!-- Balance Sheet -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
        <h3 class="text-lg font-semibold mb-4">Balance Sheet</h3>
        <p class="text-gray-600 dark:text-gray-400 mb-4">
          Assets, liabilities, and equity overview
        </p>
        <button class="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700">
          Generate Report
        </button>
      </div>

      <!-- Cash Flow -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
        <h3 class="text-lg font-semibold mb-4">Cash Flow</h3>
        <p class="text-gray-600 dark:text-gray-400 mb-4">
          Track cash inflows and outflows
        </p>
        <button class="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700">
          Generate Report
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Financial reports page logic
</script>
