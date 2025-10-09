<template>
  <div class="p-6 space-y-6">
    <!-- Page Header -->
    <div class="flex justify-between items-center">
      <div>
        <h1 class="text-3xl font-bold text-slate-900 dark:text-white">Manufacturing Dashboard</h1>
        <p class="text-slate-600 dark:text-slate-400 mt-1">Monitor production operations and shop floor performance</p>
      </div>
      <div class="flex gap-3">
        <button class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">
          New Work Order
        </button>
        <button class="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition-colors">
          New BOM
        </button>
      </div>
    </div>

    <!-- KPI Cards -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div class="bg-white dark:bg-slate-800 rounded-lg shadow p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Active Work Orders</p>
            <p class="text-3xl font-bold text-slate-900 dark:text-white mt-2">23</p>
            <p class="text-sm text-blue-600 dark:text-blue-400 mt-2">5 released today</p>
          </div>
          <div class="p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
            <svg class="w-8 h-8 text-blue-600 dark:text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01"/>
            </svg>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-slate-800 rounded-lg shadow p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Units Produced Today</p>
            <p class="text-3xl font-bold text-slate-900 dark:text-white mt-2">1,250</p>
            <p class="text-sm text-green-600 dark:text-green-400 mt-2">Target: 1,500</p>
          </div>
          <div class="p-3 bg-green-100 dark:bg-green-900 rounded-full">
            <svg class="w-8 h-8 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
            </svg>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-slate-800 rounded-lg shadow p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Quality Rate</p>
            <p class="text-3xl font-bold text-slate-900 dark:text-white mt-2">96.5%</p>
            <p class="text-sm text-slate-600 dark:text-slate-400 mt-2">Rejected: 45</p>
          </div>
          <div class="p-3 bg-purple-100 dark:bg-purple-900 rounded-full">
            <svg class="w-8 h-8 text-purple-600 dark:text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4M7.835 4.697a3.42 3.42 0 001.946-.806 3.42 3.42 0 014.438 0 3.42 3.42 0 001.946.806 3.42 3.42 0 013.138 3.138 3.42 3.42 0 00.806 1.946 3.42 3.42 0 010 4.438 3.42 3.42 0 00-.806 1.946 3.42 3.42 0 01-3.138 3.138 3.42 3.42 0 00-1.946.806 3.42 3.42 0 01-4.438 0 3.42 3.42 0 00-1.946-.806 3.42 3.42 0 01-3.138-3.138 3.42 3.42 0 00-.806-1.946 3.42 3.42 0 010-4.438 3.42 3.42 0 00.806-1.946 3.42 3.42 0 013.138-3.138z"/>
            </svg>
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-slate-800 rounded-lg shadow p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Production Cost</p>
            <p class="text-3xl font-bold text-slate-900 dark:text-white mt-2">R28,500.00</p>
            <p class="text-sm text-orange-600 dark:text-orange-400 mt-2">Efficiency: 89.2%</p>
          </div>
          <div class="p-3 bg-orange-100 dark:bg-orange-900 rounded-full">
            <svg class="w-8 h-8 text-orange-600 dark:text-orange-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
            </svg>
          </div>
        </div>
      </div>
    </div>

    <!-- Charts Placeholder -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <div class="bg-white dark:bg-slate-800 rounded-lg shadow p-6">
        <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4">Production Trend (Last 7 Days)</h3>
        <div class="h-64 bg-slate-100 dark:bg-slate-700 rounded-lg flex items-center justify-center">
          <p class="text-slate-500 dark:text-slate-400">Chart will load here</p>
        </div>
      </div>

      <div class="bg-white dark:bg-slate-800 rounded-lg shadow p-6">
        <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-4">Capacity Utilization</h3>
        <div class="h-64 bg-slate-100 dark:bg-slate-700 rounded-lg flex items-center justify-center">
          <p class="text-slate-500 dark:text-slate-400">Chart will load here</p>
        </div>
      </div>
    </div>

    <!-- Work Order Board -->
    <div class="bg-white dark:bg-slate-800 rounded-lg shadow">
      <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700">
        <h2 class="text-xl font-semibold text-slate-900 dark:text-white">Work Order Board</h2>
      </div>
      <div class="p-6">
        <div class="grid grid-cols-1 lg:grid-cols-4 gap-4">
          <!-- Draft Column -->
          <div class="bg-slate-50 dark:bg-slate-900 rounded-lg p-4">
            <h3 class="font-semibold text-slate-700 dark:text-slate-300 mb-3">Draft (2)</h3>
            <div class="space-y-3">
              <div class="bg-white dark:bg-slate-800 p-4 rounded border-l-4 border-slate-400 shadow-sm hover:shadow-md transition-shadow cursor-pointer">
                <div class="font-semibold text-sm text-slate-900 dark:text-white">WO-001</div>
                <div class="text-xs text-slate-600 dark:text-slate-400 mt-1">Widget A</div>
                <div class="text-xs text-slate-500 dark:text-slate-500 mt-2">Qty: 100</div>
                <div class="mt-2 flex gap-2">
                  <span class="text-xs px-2 py-1 bg-slate-100 dark:bg-slate-700 text-slate-700 dark:text-slate-300 rounded">Production</span>
                  <span class="text-xs px-2 py-1 rounded bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200">P5</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Released Column -->
          <div class="bg-blue-50 dark:bg-blue-900/20 rounded-lg p-4">
            <h3 class="font-semibold text-slate-700 dark:text-slate-300 mb-3">Released (2)</h3>
            <div class="space-y-3">
              <div class="bg-white dark:bg-slate-800 p-4 rounded border-l-4 border-blue-400 shadow-sm hover:shadow-md transition-shadow cursor-pointer">
                <div class="font-semibold text-sm text-slate-900 dark:text-white">WO-003</div>
                <div class="text-xs text-slate-600 dark:text-slate-400 mt-1">Component C</div>
                <div class="text-xs text-slate-500 dark:text-slate-500 mt-2">Qty: 200</div>
                <div class="text-xs text-blue-600 dark:text-blue-400 mt-1">Start: Oct 09</div>
              </div>
            </div>
          </div>

          <!-- In Progress Column -->
          <div class="bg-green-50 dark:bg-green-900/20 rounded-lg p-4">
            <h3 class="font-semibold text-slate-700 dark:text-slate-300 mb-3">In Progress (2)</h3>
            <div class="space-y-3">
              <div class="bg-white dark:bg-slate-800 p-4 rounded border-l-4 border-green-400 shadow-sm hover:shadow-md transition-shadow cursor-pointer">
                <div class="font-semibold text-sm text-slate-900 dark:text-white">WO-005</div>
                <div class="text-xs text-slate-600 dark:text-slate-400 mt-1">Assembly E</div>
                <div class="text-xs text-slate-500 dark:text-slate-500 mt-2">120 / 150</div>
                <div class="w-full bg-slate-200 dark:bg-slate-700 rounded-full h-2 mt-2">
                  <div class="bg-green-600 h-2 rounded-full" style="width: 80%"></div>
                </div>
                <div class="text-xs text-slate-500 dark:text-slate-500 mt-1">80% Complete</div>
              </div>
            </div>
          </div>

          <!-- Completed Column -->
          <div class="bg-emerald-50 dark:bg-emerald-900/20 rounded-lg p-4">
            <h3 class="font-semibold text-slate-700 dark:text-slate-300 mb-3">Completed (1)</h3>
            <div class="space-y-3">
              <div class="bg-white dark:bg-slate-800 p-4 rounded border-l-4 border-emerald-400 shadow-sm hover:shadow-md transition-shadow cursor-pointer">
                <div class="font-semibold text-sm text-slate-900 dark:text-white">WO-007</div>
                <div class="text-xs text-slate-600 dark:text-slate-400 mt-1">Widget G</div>
                <div class="text-xs text-emerald-600 dark:text-emerald-400 mt-2">✓ 100 units produced</div>
                <div class="text-xs text-slate-500 dark:text-slate-500 mt-1">Oct 09</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'default'
})

useHead({
  title: 'Manufacturing Test - TOSS ERP III'
})
</script>
