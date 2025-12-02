<template>
  <footer class="px-4 py-12 bg-white border-t border-slate-200 dark:border-slate-800 dark:bg-slate-950 sm:px-6 lg:px-8">
    <div class="mx-auto max-w-7xl">
      <div class="grid grid-cols-1 gap-8 md:grid-cols-4">
        <div>
          <div class="flex items-center space-x-3 mb-4">
            <div class="flex items-center justify-center w-10 h-10 rounded-xl shadow-lg bg-gradient-to-br from-orange-500 to-orange-600">
              <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M13 10V3L4 14h7v7l9-11h-7z"></path>
              </svg>
            </div>
            <span class="text-xl font-black text-slate-900 dark:text-white">TOSS</span>
          </div>
          <p class="text-sm text-slate-600 dark:text-slate-400">
            Township Operations Support System - Business tools for South African small businesses
          </p>
        </div>

        <div>
          <h3 class="mb-4 text-sm font-bold tracking-wider uppercase text-slate-900 dark:text-white">Product</h3>
          <ul class="space-y-3 text-sm">
            <li><a href="#features" class="text-slate-600 dark:text-slate-400 hover:text-orange-500 dark:hover:text-orange-400">Features</a></li>
            <li><a href="#pricing" class="text-slate-600 dark:text-slate-400 hover:text-orange-500 dark:hover:text-orange-400">Pricing</a></li>
            <li><a href="#faq" class="text-slate-600 dark:text-slate-400 hover:text-orange-500 dark:hover:text-orange-400">FAQ</a></li>
          </ul>
        </div>

        <div>
          <h3 class="mb-4 text-sm font-bold tracking-wider uppercase text-slate-900 dark:text-white">Company</h3>
          <ul class="space-y-3 text-sm">
            <li><a href="/about" class="text-slate-600 dark:text-slate-400 hover:text-orange-500 dark:hover:text-orange-400">About</a></li>
            <li><a href="/contact" class="text-slate-600 dark:text-slate-400 hover:text-orange-500 dark:hover:text-orange-400">Contact</a></li>
            <li><a href="/privacy" class="text-slate-600 dark:text-slate-400 hover:text-orange-500 dark:hover:text-orange-400">Privacy</a></li>
          </ul>
        </div>

        <div>
          <h3 class="mb-4 text-sm font-bold tracking-wider uppercase text-slate-900 dark:text-white">Support</h3>
          <ul class="space-y-3 text-sm">
            <li><a href="/help" class="text-slate-600 dark:text-slate-400 hover:text-orange-500 dark:hover:text-orange-400">Help Center</a></li>
            <li><a href="/docs" class="text-slate-600 dark:text-slate-400 hover:text-orange-500 dark:hover:text-orange-400">Documentation</a></li>
            <li><a href="/contact" class="text-slate-600 dark:text-slate-400 hover:text-orange-500 dark:hover:text-orange-400">Contact Support</a></li>
          </ul>
        </div>
      </div>

      <div class="pt-8 mt-12 text-sm text-center border-t border-slate-200 dark:border-slate-800 text-slate-600 dark:text-slate-400">
        <p>&copy; {{ new Date().getFullYear() }} TOSS Online Services. All rights reserved.</p>
      </div>
    </div>
  </footer>
</template>

<script setup lang="ts">
</script>
