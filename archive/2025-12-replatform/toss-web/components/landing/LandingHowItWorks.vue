<template>
  <section id="how-it-works" class="px-4 py-20 bg-white sm:px-6 lg:px-8 dark:bg-slate-950">
    <div class="mx-auto max-w-7xl">
      <div class="mb-16 text-center">
        <div class="inline-block px-3 py-1 mb-4 text-xs font-semibold tracking-wider uppercase rounded-full text-orange-700 dark:text-orange-300 bg-orange-100 dark:bg-orange-900/30">
          Simple steps
        </div>
        <h2 class="mb-4 text-3xl font-bold text-slate-900 dark:text-white sm:text-4xl md:text-5xl">
          How it works – week by week
        </h2>
        <p class="max-w-2xl mx-auto text-lg text-slate-600 dark:text-slate-300">
          Follow these simple steps to stay in control of your shop
        </p>
      </div>

      <div class="relative max-w-4xl mx-auto">
        <!-- Timeline line -->
        <div class="absolute top-0 bottom-0 w-1 bg-orange-200 dark:bg-orange-900/50 left-6"></div>

        <!-- Steps -->
        <ol class="relative pl-12 space-y-12">
          <li v-for="(step, index) in steps" :key="index" class="relative transition-all duration-500 group hover:translate-x-2">
            <div class="absolute flex items-center justify-center text-lg font-bold text-white transition-all duration-500 bg-orange-500 rounded-full shadow-lg -left-6 w-12 h-12 dark:bg-orange-600 group-hover:scale-125 group-hover:shadow-xl origin-center">
              {{ index + 1 }}
            </div>
            <div class="ml-4 transition-all duration-500 hover:translate-x-2">
              <h3 class="mb-3 text-lg font-bold text-slate-900 dark:text-white">{{ step.title }}</h3>
              <p class="text-slate-600 dark:text-slate-400">{{ step.description }}</p>
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
    title: "Monday morning: Check stock",
    description: "See what you sold over the weekend and what needs restocking"
  },
  {
    title: "Tuesday: Join a group order",
    description: "TOSS shows you nearby shops ordering the same products. Buy together and save."
  },
  {
    title: "Wednesday: Restock alert",
    description: "When stock is low, the platform tells you what to buy and helps you order from suppliers"
  },
  {
    title: "All week: Sell with confidence",
    description: "Every sale is recorded. Know what you make each day, who owes you, and what's moving fast."
  },
  {
    title: "Weekend: Cash up & review",
    description: "See what sold best, what profit you made, and plan next week's orders"
  }
]
</script>
