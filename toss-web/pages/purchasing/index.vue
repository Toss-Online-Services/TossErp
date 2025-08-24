<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Purchasing & Procurement</h1>
              <p class="text-gray-600 dark:text-gray-400">Manage suppliers, purchase orders, and procurement processes</p>
            </div>
            <div class="flex space-x-3">
              <button class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors">
                <span class="inline-block w-5 h-5 mr-2">📄</span>
                New PO
              </button>
              <button class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors">
                <span class="inline-block w-5 h-5 mr-2">🤝</span>
                Find Group Buy
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Coming Soon Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <div class="text-center">
        <div class="w-32 h-32 mx-auto mb-8 bg-gradient-to-r from-purple-400 to-pink-500 rounded-full flex items-center justify-center">
          <span class="text-6xl text-white">🛍️</span>
        </div>
        <h2 class="text-3xl font-bold text-gray-900 dark:text-white mb-4">Purchasing & Procurement</h2>
        <p class="text-xl text-gray-600 dark:text-gray-400 mb-8 max-w-2xl mx-auto">
          Advanced procurement features are being developed with focus on collaborative purchasing, 
          supplier networks, and group buying integration for maximum cost savings.
        </p>
        <div class="flex justify-center space-x-4">
          <NuxtLink to="/group-buying" class="bg-blue-600 text-white px-6 py-3 rounded-lg hover:bg-blue-700 transition-colors">
            Explore Group Buying
          </NuxtLink>
          <NuxtLink to="/dashboard" class="bg-gray-600 text-white px-6 py-3 rounded-lg hover:bg-gray-700 transition-colors">
            Back to Dashboard
          </NuxtLink>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Page metadata
useHead({
  title: 'Purchasing - TOSS ERP',
  meta: [
    { name: 'description', content: 'Purchasing and procurement management in TOSS ERP' }
  ]
})
</script>
