<template>
  <div class="min-h-screen bg-white dark:bg-slate-950">
    <LandingNavigation />
    <LandingHero />
    <LandingProblems />
    <LandingHowItWorks />
    <LandingFeatures />
    <LandingForSuppliers />
    <LandingGetStarted />
    <LandingWhyToss />
    <LandingCTA />
    <LandingFAQ />
    <LandingFooter />
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'landing'
})

useHead({
  title: 'TOSS - Township Operations Support System',
  meta: [
    { name: 'description', content: 'One simple platform to track sales and stock, get smart advice, and buy together with other shops. Built for South African small businesses.' }
  ]
})
</script>
