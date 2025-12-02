<template>
  <section id="faq" class="px-4 py-20 sm:px-6 lg:px-8 bg-slate-50 dark:bg-slate-900/40">
    <div class="mx-auto max-w-4xl">
      <div class="mb-16 text-center">
        <div class="inline-block px-3 py-1 mb-4 text-xs font-semibold tracking-wider uppercase rounded-full text-orange-700 dark:text-orange-300 bg-orange-100 dark:bg-orange-900/30">
          Questions
        </div>
        <h2 class="mb-4 text-3xl font-bold text-slate-900 dark:text-white sm:text-4xl md:text-5xl">
          Common questions
        </h2>
      </div>

      <div class="space-y-6">
        <div v-for="(faq, index) in faqs" :key="index" class="p-6 bg-white border rounded-lg dark:bg-slate-900 dark:border-slate-800 transition-all duration-300 hover:shadow-lg hover:-translate-y-1 hover:border-orange-300 dark:hover:border-orange-700 cursor-pointer">
          <h3 class="mb-2 text-lg font-bold text-slate-900 dark:text-white">{{ faq.question }}</h3>
          <p class="text-slate-600 dark:text-slate-400">{{ faq.answer }}</p>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
const faqs = [
  {
    question: "How much does TOSS cost?",
    answer: "TOSS is free to get started with core features. We offer affordable premium plans with advanced features that save you more money than they cost."
  },
  {
    question: "Do I need WiFi or can I use normal data?",
    answer: "TOSS works with normal mobile data and uses very little. It also works completely offline – you can ring up sales even without any connection, and everything syncs later when you have data."
  },
  {
    question: "Can I use it on my phone?",
    answer: "Yes! TOSS is made for phones. It works on Android phones (even older ones). You don't need a computer or special equipment."
  },
  {
    question: "What if I'm not good with technology?",
    answer: "TOSS is designed to be as simple as WhatsApp. If you can send a message, you can use TOSS. We also provide training and support to help you get started."
  },
  {
    question: "Will my sales and customer data be safe?",
    answer: "Yes, very safe. Your data is encrypted and stored securely. Only you can see your business information. We don't share it with anyone without your permission."
  },
  {
    question: "How does the group buying work?",
    answer: "TOSS connects shops in the same area who want to buy the same products. When you join a group order, you combine your order with other shops to get better wholesale prices – like buying in bulk together."
  }
]
</script>
