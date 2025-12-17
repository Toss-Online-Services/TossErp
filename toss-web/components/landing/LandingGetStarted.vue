<template>
  <section id="get-started" class="relative px-4 py-20 overflow-hidden bg-white sm:px-6 lg:px-8 dark:bg-slate-950">
    <div class="mx-auto max-w-7xl">
      <div class="mb-16 text-center">
        <div class="inline-block px-3 py-1 mb-4 text-xs font-semibold tracking-wider uppercase rounded-full text-orange-700 dark:text-orange-300 bg-orange-100 dark:bg-orange-900/30">
          Get Started
        </div>
        <h2 class="mb-4 text-3xl font-bold text-slate-900 dark:text-white sm:text-4xl md:text-5xl">
          How to get started with TOSS
        </h2>
        <p class="max-w-2xl mx-auto text-lg text-slate-600 dark:text-slate-300">
          Five simple steps to start running your shop better
        </p>
      </div>

      <div class="max-w-4xl mx-auto">
        <ol class="relative pl-12 space-y-12 border-l-2 border-orange-200 dark:border-orange-900">
          <li v-for="(step, index) in steps" :key="index" class="ml-16">
            <div class="absolute w-12 h-12 -left-6 flex items-center justify-center rounded-full bg-orange-500 text-white font-bold text-lg transition-all duration-300 hover:scale-125 hover:shadow-lg origin-center">
              {{ index + 1 }}
            </div>
            <div class="ml-4 transition-all duration-500 hover:translate-x-2">
              <h3 class="mb-3 text-lg font-bold text-slate-800 dark:text-slate-200">{{ step.title }}</h3>
              <p class="text-sm text-slate-600 dark:text-slate-400">{{ step.description }}</p>
            </div>
          </li>
        </ol>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
const steps = [
  {
    title: "Shop signs up and sets up products",
    description: "Quick setup on your phone. Add your products with simple names and prices."
  },
  {
    title: "Use TOSS POS to sell",
    description: "Sales and stock update automatically. No more notebooks."
  },
  {
    title: "TOSS suggests what to order",
    description: "When stock is low, the platform tells you what to buy and helps you order."
  },
  {
    title: "Shops join a group order",
    description: "Buy together with other shops to get wholesale prices. TOSS sends it to suppliers."
  },
  {
    title: "Driver delivers stock to your shop",
    description: "Local driver picks up from wholesaler and delivers straight to your door. No more taxi trips."
  }
]
</script>
