<template>
  <div class="container mx-auto px-4 py-8">
    <div class="mb-8">
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white mb-2">
        Stock Management
      </h1>
      <p class="text-gray-600 dark:text-gray-300">
        Manage inventory levels and stock movements
      </p>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-8">
      <!-- Low Stock Alerts -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
        <h3 class="text-lg font-semibold mb-4 text-red-600">Low Stock Alerts</h3>
        <div class="space-y-3">
          <div class="flex justify-between items-center p-3 bg-red-50 dark:bg-red-900/20 rounded">
            <span class="font-medium">Product A</span>
            <span class="text-red-600 font-bold">5 units left</span>
          </div>
          <div class="flex justify-between items-center p-3 bg-red-50 dark:bg-red-900/20 rounded">
            <span class="font-medium">Product B</span>
            <span class="text-red-600 font-bold">2 units left</span>
          </div>
        </div>
      </div>

      <!-- Recent Movements -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6">
        <h3 class="text-lg font-semibold mb-4">Recent Stock Movements</h3>
        <div class="space-y-3">
          <div class="flex justify-between items-center p-3 border-l-4 border-green-500">
            <div>
              <span class="font-medium">Product C</span>
              <p class="text-sm text-gray-600">Stock In</p>
            </div>
            <span class="text-green-600 font-bold">+50 units</span>
          </div>
          <div class="flex justify-between items-center p-3 border-l-4 border-red-500">
            <div>
              <span class="font-medium">Product D</span>
              <p class="text-sm text-gray-600">Stock Out</p>
            </div>
            <span class="text-red-600 font-bold">-25 units</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Stock management page logic
</script>
