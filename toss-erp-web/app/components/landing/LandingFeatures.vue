<template>
  <section id="features" class="px-4 py-20 sm:px-6 lg:px-8 bg-slate-50 dark:bg-slate-900/40">
    <div class="mx-auto max-w-7xl">
      <div class="mb-16 text-center">
        <div class="inline-block px-3 py-1 mb-4 text-xs font-semibold tracking-wider uppercase rounded-full text-orange-700 dark:text-orange-300 bg-orange-100 dark:bg-orange-900/30">
          What you get
        </div>
        <h2 class="mb-4 text-3xl font-bold text-slate-900 dark:text-white sm:text-4xl md:text-5xl">
          For small businesses and local shops
        </h2>
        <p class="max-w-2xl mx-auto text-lg text-slate-600 dark:text-slate-300">
          Perfect for township and rural businesses like yours
        </p>
      </div>

      <div class="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-4">
        <div v-for="(feature, index) in features" :key="index" class="p-6 border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 rounded-lg transition-all duration-300 hover:shadow-lg hover:scale-105 hover:border-orange-300 dark:hover:border-orange-700 group">
          <div class="flex items-center justify-center w-12 h-12 mb-4 rounded-lg bg-orange-100 dark:bg-orange-900/30 transition-all duration-300 group-hover:scale-110 group-hover:bg-orange-200 dark:group-hover:bg-orange-900/50">
            <component :is="feature.icon" class="w-6 h-6 text-orange-600 dark:text-orange-400" />
          </div>
          <h3 class="mb-2 text-lg font-bold text-slate-900 dark:text-white">{{ feature.title }}</h3>
          <p class="text-sm text-slate-600 dark:text-slate-400">{{ feature.description }}</p>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { h } from 'vue'

const PhoneIcon = () => h('svg', { fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
  h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M12 18h.01M8 21h8a2 2 0 002-2V5a2 2 0 00-2-2H8a2 2 0 00-2 2v14a2 2 0 002 2z' })
])

const BoxIcon = () => h('svg', { fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
  h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4' })
])

const ClipboardIcon = () => h('svg', { fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
  h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2' })
])

const MoneyIcon = () => h('svg', { fill: 'none', stroke: 'currentColor', viewBox: '0 0 24 24' }, [
  h('path', { 'stroke-linecap': 'round', 'stroke-linejoin': 'round', 'stroke-width': '2', d: 'M17 9V7a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2m2 4h10a2 2 0 002-2v-6a2 2 0 00-2-2H9a2 2 0 00-2 2v6a2 2 0 002 2zm7-5a2 2 0 11-4 0 2 2 0 014 0z' })
])

const features = [
  {
    icon: PhoneIcon,
    title: 'Simple POS on your phone',
    description: 'Ring up sales, track totals and download receipts – no fancy hardware needed'
  },
  {
    icon: BoxIcon,
    title: 'See your stock in one place',
    description: "Know what's low, what's finished, and what to buy next"
  },
  {
    icon: ClipboardIcon,
    title: 'Order from suppliers in a few taps',
    description: 'Send digital orders instead of long WhatsApp lists and phone calls'
  },
  {
    icon: MoneyIcon,
    title: 'Save by buying together',
    description: 'Join group orders with other shops to unlock better prices'
  }
]
</script>
