<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Header -->
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-gray-900 dark:text-white">Tools & Resources</h1>
        <p class="mt-2 text-gray-600 dark:text-gray-400">Business tools, calculators, and utilities for SMMEs</p>
      </div>

      <!-- Coming Soon Banner -->
      <div class="bg-purple-50 dark:bg-purple-900/20 border border-purple-200 dark:border-purple-800 rounded-lg p-6 text-center">
        <IconBase class="mx-auto h-12 w-12 text-purple-600 mb-4">
          <path d="M11.78 5.22a.75.75 0 0 1 1.06 0l3 3a.75.75 0 0 1 0 1.06l-3 3a.75.75 0 1 1-1.06-1.06L13.94 9H7.5a.75.75 0 0 1 0-1.5h6.44l-2.16-2.16a.75.75 0 0 1 0-1.06Z" />
        </IconBase>
        <h2 class="text-xl font-semibold text-purple-900 dark:text-purple-100 mb-2">Coming Soon</h2>
        <p class="text-purple-700 dark:text-purple-300">
          The tools and resources section is currently under development and will be available soon.
        </p>
      </div>

      <!-- Feature Preview -->
      <div class="mt-8 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Business Calculators</h3>
          <p class="text-gray-600 dark:text-gray-400">ROI, break-even, loan, and tax calculators.</p>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Document Templates</h3>
          <p class="text-gray-600 dark:text-gray-400">Contracts, invoices, and business document templates.</p>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Compliance Tools</h3>
          <p class="text-gray-600 dark:text-gray-400">BEE scoring, tax compliance, and regulatory tools.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
useHead({
  title: 'Tools & Resources - TOSS ERP III'
})
</script>
