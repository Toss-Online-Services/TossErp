<template>
  <section class="relative px-4 pt-32 pb-24 overflow-hidden sm:px-6 lg:px-8">
    <!-- Animated Background -->
    <div class="absolute inset-0 -z-10 bg-gradient-to-br from-orange-50/50 via-white to-orange-50/30 dark:from-slate-950 dark:via-slate-900 dark:to-slate-950">
      <div class="absolute top-20 left-1/4 w-96 h-96 bg-orange-500/10 dark:bg-orange-500/5 rounded-full blur-3xl animate-pulse"></div>
      <div class="absolute bottom-20 right-1/4 w-96 h-96 bg-orange-400/10 dark:bg-orange-400/5 rounded-full blur-3xl animate-pulse delay-1000"></div>
    </div>

    <div class="mx-auto max-w-7xl">
      <div class="text-center">
        <!-- Badge -->
        <div class="inline-flex items-center gap-2 px-4 py-2 mb-8 text-sm font-semibold border rounded-full shadow-sm text-orange-700 dark:text-orange-300 bg-orange-100/80 dark:bg-orange-900/30 border-orange-200 dark:border-orange-800 backdrop-blur-sm animate-fade-in-up">
          <span class="relative flex w-2 h-2">
            <span class="absolute inline-flex w-full h-full rounded-full opacity-75 animate-ping bg-orange-400"></span>
            <span class="relative inline-flex w-2 h-2 rounded-full bg-orange-500"></span>
          </span>
          Built for South African small businesses
        </div>

        <!-- Main Heading -->
        <h1 class="mb-6 text-5xl font-extrabold tracking-tight text-slate-900 dark:text-white sm:text-6xl md:text-7xl animate-fade-in-up animation-delay-100">
          <span class="block">Run your hustle like</span>
          <span class="block mt-2 overflow-visible pb-2">
            <span class="text-orange-500 animate-gradient">a big enterprise</span>
          </span>
        </h1>

        <!-- Subheading -->
        <p class="max-w-3xl mx-auto mb-4 text-xl font-medium leading-relaxed text-slate-600 dark:text-slate-300 animate-fade-in-up animation-delay-200">
          One simple platform to track your sales and stock, get smart advice, and buy together with other shops to save money
        </p>
        <p class="max-w-2xl mx-auto mb-12 text-lg text-slate-500 dark:text-slate-400 animate-fade-in-up animation-delay-300">
          Works offline. In your language. Made for township and rural businesses like yours.
        </p>

        <!-- CTA Buttons -->
        <div class="flex flex-col items-center justify-center gap-4 mb-16 sm:flex-row animate-fade-in-up animation-delay-400">
          <a href="/auth/register" class="inline-flex items-center gap-2 px-8 py-4 text-lg font-semibold text-white transition-all duration-300 rounded-md shadow-xl bg-orange-500 hover:bg-orange-600 dark:bg-orange-600 dark:hover:bg-orange-700 hover:shadow-2xl hover:scale-105 group">
            <span>Join TOSS</span>
            <svg class="w-5 h-5 transition-transform group-hover:translate-x-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7l5 5m0 0l-5 5m5-5H6"></path>
            </svg>
          </a>
          <a href="#how-it-works" class="inline-flex items-center gap-2 px-8 py-4 text-lg font-semibold transition-all duration-300 border-2 rounded-md text-slate-700 dark:text-slate-300 border-slate-300 dark:border-slate-700 hover:border-slate-400 dark:hover:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-900 group">
            <svg class="w-5 h-5 transition-transform group-hover:scale-110" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
            </svg>
            <span>See how it works</span>
          </a>
        </div>

        <!-- Trust Indicators -->
        <div class="flex flex-wrap items-center justify-center gap-8 text-sm text-slate-500 dark:text-slate-400 animate-fade-in-up animation-delay-500">
          <div class="flex items-center gap-2">
            <svg class="w-5 h-5 text-green-500" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
            </svg>
            <span>Works offline</span>
          </div>
          <div class="flex items-center gap-2">
            <svg class="w-5 h-5 text-green-500" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
            </svg>
            <span>Save money by buying together</span>
          </div>
          <div class="flex items-center gap-2">
            <svg class="w-5 h-5 text-green-500" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
            </svg>
            <span>Stock delivered to your door</span>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
</script>

<style scoped>
@keyframes fade-in-up {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fade-in-up {
  animation: fade-in-up 0.6s ease-out;
}

.animation-delay-100 {
  animation-delay: 0.1s;
  animation-fill-mode: backwards;
}

.animation-delay-200 {
  animation-delay: 0.2s;
  animation-fill-mode: backwards;
}

.animation-delay-300 {
  animation-delay: 0.3s;
  animation-fill-mode: backwards;
}

.animation-delay-400 {
  animation-delay: 0.4s;
  animation-fill-mode: backwards;
}

.animation-delay-500 {
  animation-delay: 0.5s;
  animation-fill-mode: backwards;
}

@keyframes gradient {
  0%, 100% {
    background-position: 0% 50%;
  }
  50% {
    background-position: 100% 50%;
  }
}

.animate-gradient {
  background: linear-gradient(90deg, #f97316, #ea580c, #f97316);
  background-size: 200% 200%;
  animation: gradient 3s ease infinite;
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
  display: inline-block;
  padding-bottom: 0.1em;
}
</style>
