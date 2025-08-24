<template>
  <div class="container mx-auto px-4 py-8">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">
        Sales Dashboard
      </h1>
      <p class="text-gray-600 dark:text-gray-300">
        Track sales performance and manage opportunities
      </p>
    </div>

    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      <!-- Sales Metrics -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
        <h3 class="text-sm font-medium text-gray-500 mb-2">Today's Sales</h3>
        <p class="text-2xl font-bold text-green-600">R 15,420</p>
        <span class="text-sm text-green-500">+12% from yesterday</span>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
        <h3 class="text-sm font-medium text-gray-500 mb-2">Monthly Target</h3>
        <p class="text-2xl font-bold text-blue-600">R 250,000</p>
        <span class="text-sm text-blue-500">68% completed</span>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
        <h3 class="text-sm font-medium text-gray-500 mb-2">Active Leads</h3>
        <p class="text-2xl font-bold text-purple-600">24</p>
        <span class="text-sm text-purple-500">+3 this week</span>
      </div>

      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
        <h3 class="text-sm font-medium text-gray-500 mb-2">Conversion Rate</h3>
        <p class="text-2xl font-bold text-orange-600">18.5%</p>
        <span class="text-sm text-orange-500">Above average</span>
      </div>
    </div>

    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
      <h3 class="text-lg font-semibold mb-4">Recent Sales Activity</h3>
      <div class="space-y-4">
        <div class="flex justify-between items-center p-4 border-l-4 border-green-500 bg-green-50 dark:bg-green-900/20">
          <div>
            <p class="font-medium">Sale #1234 - Customer ABC</p>
            <p class="text-sm text-gray-600">2 hours ago</p>
          </div>
          <span class="text-green-600 font-bold">R 2,850</span>
        </div>
        <div class="flex justify-between items-center p-4 border-l-4 border-blue-500 bg-blue-50 dark:bg-blue-900/20">
          <div>
            <p class="font-medium">Quote #5678 - Customer XYZ</p>
            <p class="text-sm text-gray-600">4 hours ago</p>
          </div>
          <span class="text-blue-600 font-bold">R 5,200</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Sales dashboard logic
</script>
