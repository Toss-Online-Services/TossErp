<template>
  <section id="pricing" class="relative px-4 py-20 overflow-hidden bg-white sm:px-6 lg:px-8 dark:bg-slate-950">
    <div class="mx-auto max-w-3xl">
      <div class="mb-12 text-center">
        <h2 class="mb-4 text-3xl font-bold text-slate-900 dark:text-white sm:text-4xl md:text-5xl">
          Start using TOSS today
        </h2>
        <p class="text-lg text-slate-600 dark:text-slate-300">
          Join thousands of South African businesses already growing with TOSS
        </p>
      </div>

      <div class="flex flex-col items-center justify-center gap-6">
        <a href="/auth/register" class="inline-flex items-center gap-2 px-12 py-5 text-xl font-semibold text-white transition-all duration-300 rounded-md shadow-xl bg-orange-500 hover:bg-orange-600 dark:bg-orange-600 dark:hover:bg-orange-700 hover:shadow-2xl hover:scale-105">
          <span>Create your account</span>
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7l5 5m0 0l-5 5m5-5H6"></path>
          </svg>
        </a>
        
        <p class="text-sm text-slate-500 dark:text-slate-400">
          Already have an account? 
          <a href="/auth/login" class="font-semibold text-orange-500 hover:text-orange-600 dark:text-orange-400 dark:hover:text-orange-300">
            Sign in here
          </a>
        </p>

        <div class="flex flex-wrap items-center justify-center gap-6 mt-6 text-sm text-slate-600 dark:text-slate-400">
          <div class="flex items-center gap-2">
            <svg class="w-5 h-5 text-green-500" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
            </svg>
            <span>Free to get started</span>
          </div>
          <div class="flex items-center gap-2">
            <svg class="w-5 h-5 text-green-500" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
            </svg>
            <span>No credit card required</span>
          </div>
          <div class="flex items-center gap-2">
            <svg class="w-5 h-5 text-green-500" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
            </svg>
            <span>Ready in 5 minutes</span>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
</script>
