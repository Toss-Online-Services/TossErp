<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Header -->
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-gray-900 dark:text-white">Communication Center</h1>
        <p class="mt-2 text-gray-600 dark:text-gray-400">Email campaigns, SMS marketing, and customer communication</p>
      </div>

      <!-- Coming Soon Banner -->
      <div class="bg-cyan-50 dark:bg-cyan-900/20 border border-cyan-200 dark:border-cyan-800 rounded-lg p-6 text-center">
        <IconBase class="mx-auto h-12 w-12 text-cyan-600 mb-4">
          <path d="M21.75 6.75v10.5a2.25 2.25 0 0 1-2.25 2.25h-15a2.25 2.25 0 0 1-2.25-2.25V6.75m19.5 0A2.25 2.25 0 0 0 19.5 4.5h-15a2.25 2.25 0 0 0-2.25 2.25m19.5 0v.243a2.25 2.25 0 0 1-1.07 1.916l-7.5 4.615a2.25 2.25 0 0 1-2.36 0L3.32 8.91a2.25 2.25 0 0 1-1.07-1.916V6.75" />
        </IconBase>
        <h2 class="text-xl font-semibold text-cyan-900 dark:text-cyan-100 mb-2">Coming Soon</h2>
        <p class="text-cyan-700 dark:text-cyan-300">
          The communication center is currently under development and will be available soon.
        </p>
      </div>

      <!-- Feature Preview -->
      <div class="mt-8 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Email Marketing</h3>
          <p class="text-gray-600 dark:text-gray-400">Create and send newsletters, promotions, and targeted campaigns.</p>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">SMS Campaigns</h3>
          <p class="text-gray-600 dark:text-gray-400">Send bulk SMS messages and automated text notifications.</p>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Customer Portal</h3>
          <p class="text-gray-600 dark:text-gray-400">Provide customers with self-service access and support tickets.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
useHead({
  title: 'Communication Center - TOSS ERP III'
})
</script>
