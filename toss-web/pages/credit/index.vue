<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Header -->
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-gray-900 dark:text-white">Credit & Financing</h1>
        <p class="mt-2 text-gray-600 dark:text-gray-400">Credit management, pooled financing, and business loans</p>
      </div>

      <!-- Coming Soon Banner -->
      <div class="bg-green-50 dark:bg-green-900/20 border border-green-200 dark:border-green-800 rounded-lg p-6 text-center">
        <IconBase class="mx-auto h-12 w-12 text-green-600 mb-4">
          <path d="M2.25 18.75a.75.75 0 0 0 1.06 0l16.5-16.5a.75.75 0 1 0-1.06-1.06L2.25 17.69a.75.75 0 0 0 0 1.06Z" />
        </IconBase>
        <h2 class="text-xl font-semibold text-green-900 dark:text-green-100 mb-2">Coming Soon</h2>
        <p class="text-green-700 dark:text-green-300">
          The credit and financing module is currently under development and will be available soon.
        </p>
      </div>

      <!-- Feature Preview -->
      <div class="mt-8 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Credit Engine</h3>
          <p class="text-gray-600 dark:text-gray-400">AI-powered credit scoring and risk assessment.</p>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Pooled Credit</h3>
          <p class="text-gray-600 dark:text-gray-400">Mutual financing and collective credit pools.</p>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Loan Management</h3>
          <p class="text-gray-600 dark:text-gray-400">Application processing and repayment tracking.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
useHead({
  title: 'Credit & Financing - TOSS ERP III'
})
</script>
