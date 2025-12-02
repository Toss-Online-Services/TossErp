<template>
  <section class="px-4 py-20 sm:px-6 lg:px-8 bg-slate-50 dark:bg-slate-900/40">
    <div class="mx-auto max-w-7xl">
      <div class="mb-12 text-center">
        <h2 class="mb-4 text-3xl font-bold text-slate-900 dark:text-white sm:text-4xl">Built for South African small businesses</h2>
        <p class="max-w-2xl mx-auto text-lg text-slate-600 dark:text-slate-300">
          We understand your challenges because we built this for you
        </p>
      </div>

      <div class="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
        <div v-for="(benefit, index) in benefits" :key="index" class="flex items-start gap-4">
          <svg class="flex-shrink-0 w-6 h-6 mt-1 text-green-500" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
          </svg>
          <div>
            <h4 class="mb-1 font-semibold text-slate-900 dark:text-white">{{ benefit.title }}</h4>
            <p class="text-sm text-slate-600 dark:text-slate-400">{{ benefit.description }}</p>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
const benefits = [
  {
    title: "Works on low data",
    description: "Uses very little data. Works offline and syncs when you have connection."
  },
  {
    title: "Shows prices in Rand (R)",
    description: "All pricing in South African Rand with proper cash rounding."
  },
  {
    title: "Simple language",
    description: "No complicated business words. Plain English (and local languages coming soon)."
  },
  {
    title: "For all small businesses",
    description: "From retail shops to food vendors, salons to mechanics, and everything in between."
  },
  {
    title: "Easy to use",
    description: "You don't need to be good with technology. If you can use WhatsApp, you can use TOSS."
  },
  {
    title: "Free to start",
    description: "Try it for free. Only pay when you're making more money."
  }
]
</script>
