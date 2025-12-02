<template>
  <section class="px-4 py-20 sm:px-6 lg:px-8 bg-slate-50 dark:bg-slate-900/40">
    <div class="mx-auto max-w-7xl">
      <div class="mb-16 text-center">
        <h2 class="mb-4 text-3xl font-bold text-slate-900 dark:text-white sm:text-4xl md:text-5xl">The everyday struggle</h2>
        <p class="max-w-2xl mx-auto text-lg text-slate-600 dark:text-slate-300">
          Running a small shop is hard work. Here's what you deal with every day:
        </p>
      </div>

      <div class="grid gap-8 mb-16 md:grid-cols-2">
        <!-- Problems -->
        <div class="p-8 bg-white border rounded-lg shadow-sm dark:bg-slate-900 dark:border-slate-800">
          <h3 class="mb-6 text-xl font-bold text-red-600 dark:text-red-400">😓 The Problems</h3>
          <ul class="space-y-4">
            <li class="flex items-start gap-3">
              <span class="flex-shrink-0 w-6 h-6 mt-0.5 text-red-500">✗</span>
              <span class="text-slate-700 dark:text-slate-300">You get to month-end with empty shelves and no cash</span>
            </li>
            <li class="flex items-start gap-3">
              <span class="flex-shrink-0 w-6 h-6 mt-0.5 text-red-500">✗</span>
              <span class="text-slate-700 dark:text-slate-300">You pay retail prices at the wholesaler because you buy alone</span>
            </li>
            <li class="flex items-start gap-3">
              <span class="flex-shrink-0 w-6 h-6 mt-0.5 text-red-500">✗</span>
              <span class="text-slate-700 dark:text-slate-300">You sell on credit but don't have proper records</span>
            </li>
            <li class="flex items-start gap-3">
              <span class="flex-shrink-0 w-6 h-6 mt-0.5 text-red-500">✗</span>
              <span class="text-slate-700 dark:text-slate-300">You close the shop to go buy stock and lose sales time</span>
            </li>
          </ul>
        </div>

        <!-- Solutions -->
        <div class="p-8 bg-white border rounded-lg shadow-sm dark:bg-slate-900 dark:border-slate-800">
          <h3 class="mb-6 text-xl font-bold text-green-600 dark:text-green-400">✓ How TOSS Helps</h3>
          <ul class="space-y-4">
            <li class="flex items-start gap-3">
              <span class="flex-shrink-0 w-6 h-6 mt-0.5 text-green-500">✓</span>
              <span class="text-slate-700 dark:text-slate-300">See what's selling and what's running out – in one view</span>
            </li>
            <li class="flex items-start gap-3">
              <span class="flex-shrink-0 w-6 h-6 mt-0.5 text-green-500">✓</span>
              <span class="text-slate-700 dark:text-slate-300">Join group orders so you can buy like a big shop</span>
            </li>
            <li class="flex items-start gap-3">
              <span class="flex-shrink-0 w-6 h-6 mt-0.5 text-green-500">✓</span>
              <span class="text-slate-700 dark:text-slate-300">Keep simple records of every sale and payment</span>
            </li>
            <li class="flex items-start gap-3">
              <span class="flex-shrink-0 w-6 h-6 mt-0.5 text-green-500">✓</span>
              <span class="text-slate-700 dark:text-slate-300">Order stock from your phone – it gets delivered to you</span>
            </li>
          </ul>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
</script>
