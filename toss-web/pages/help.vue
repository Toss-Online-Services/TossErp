<script setup lang="ts">
useHead({ title: 'Help & Support - TOSS' })

const faqs = [
  {
    question: 'How do I add a new product?',
    answer: 'Go to Stock > Items and click "Add Item". Fill in the product details including name, price, and stock levels.'
  },
  {
    question: 'How do I process a sale?',
    answer: 'Navigate to Sales > POS to access the point of sale interface. Search for products, add them to cart, and process payment.'
  },
  {
    question: 'Can I use TOSS offline?',
    answer: 'Yes! TOSS works offline. Sales and other operations are queued and automatically synced when you\'re back online.'
  },
  {
    question: 'How do I track employee attendance?',
    answer: 'Go to HR > Attendance to record clock in/out times or days worked for your employees.'
  },
  {
    question: 'How do I generate payroll?',
    answer: 'Navigate to HR > Payroll, select the period and employees, then click "Run Payroll" to generate payslips.'
  }
]

const supportContacts = [
  {
    type: 'Email',
    value: 'support@toss.co.za',
    icon: 'email'
  },
  {
    type: 'Phone',
    value: '+27 11 123 4567',
    icon: 'phone'
  },
  {
    type: 'WhatsApp',
    value: '+27 82 123 4567',
    icon: 'chat'
  }
]

const expandedFaq = ref<number | null>(null)

function toggleFaq(index: number) {
  expandedFaq.value = expandedFaq.value === index ? null : index
}
</script>

<template>
  <div class="py-6">
    <!-- Header -->
    <div class="mb-8">
      <h3 class="text-3xl font-bold text-gray-900 mb-2">Help & Support</h3>
      <p class="text-gray-600 text-sm">Get help and find answers to common questions</p>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Main Content -->
      <div class="lg:col-span-2 space-y-6">
        <!-- Quick Start Guide -->
        <div class="bg-white rounded-xl shadow-sm p-6">
          <h4 class="text-lg font-semibold text-gray-900 mb-4">Quick Start Guide</h4>
          <div class="space-y-4">
            <div class="flex items-start gap-4">
              <div class="w-8 h-8 bg-blue-100 rounded-full flex items-center justify-center flex-shrink-0">
                <span class="text-blue-600 font-semibold text-sm">1</span>
              </div>
              <div>
                <h5 class="font-medium text-gray-900 mb-1">Add Your Products</h5>
                <p class="text-sm text-gray-600">Start by adding your inventory items in the Stock section</p>
              </div>
            </div>
            <div class="flex items-start gap-4">
              <div class="w-8 h-8 bg-blue-100 rounded-full flex items-center justify-center flex-shrink-0">
                <span class="text-blue-600 font-semibold text-sm">2</span>
              </div>
              <div>
                <h5 class="font-medium text-gray-900 mb-1">Set Up Customers</h5>
                <p class="text-sm text-gray-600">Add your customers in the People section to track sales</p>
              </div>
            </div>
            <div class="flex items-start gap-4">
              <div class="w-8 h-8 bg-blue-100 rounded-full flex items-center justify-center flex-shrink-0">
                <span class="text-blue-600 font-semibold text-sm">3</span>
              </div>
              <div>
                <h5 class="font-medium text-gray-900 mb-1">Start Selling</h5>
                <p class="text-sm text-gray-600">Use the POS interface to process sales and track revenue</p>
              </div>
            </div>
          </div>
        </div>

        <!-- FAQ -->
        <div class="bg-white rounded-xl shadow-sm p-6">
          <h4 class="text-lg font-semibold text-gray-900 mb-4">Frequently Asked Questions</h4>
          <div class="space-y-3">
            <div
              v-for="(faq, index) in faqs"
              :key="index"
              class="border border-gray-200 rounded-lg"
            >
              <button
                @click="toggleFaq(index)"
                class="w-full px-4 py-3 flex items-center justify-between text-left hover:bg-gray-50 transition-colors"
              >
                <span class="font-medium text-gray-900">{{ faq.question }}</span>
                <i
                  class="material-symbols-rounded transition-transform"
                  :class="{ 'rotate-180': expandedFaq === index }"
                >
                  expand_more
                </i>
              </button>
              <div
                v-if="expandedFaq === index"
                class="px-4 pb-3 text-sm text-gray-600"
              >
                {{ faq.answer }}
              </div>
            </div>
          </div>
        </div>

        <!-- Video Tutorials -->
        <div class="bg-white rounded-xl shadow-sm p-6">
          <h4 class="text-lg font-semibold text-gray-900 mb-4">Video Tutorials</h4>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div class="border border-gray-200 rounded-lg p-4">
              <div class="w-full h-32 bg-gray-100 rounded-lg mb-3 flex items-center justify-center">
                <i class="material-symbols-rounded text-4xl text-gray-400">play_circle</i>
              </div>
              <h5 class="font-medium text-gray-900 mb-1">Getting Started</h5>
              <p class="text-xs text-gray-600">Learn the basics of TOSS ERP</p>
            </div>
            <div class="border border-gray-200 rounded-lg p-4">
              <div class="w-full h-32 bg-gray-100 rounded-lg mb-3 flex items-center justify-center">
                <i class="material-symbols-rounded text-4xl text-gray-400">play_circle</i>
              </div>
              <h5 class="font-medium text-gray-900 mb-1">Using POS</h5>
              <p class="text-xs text-gray-600">How to process sales efficiently</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Sidebar -->
      <div class="space-y-6">
        <!-- Contact Support -->
        <div class="bg-white rounded-xl shadow-sm p-6">
          <h4 class="text-lg font-semibold text-gray-900 mb-4">Contact Support</h4>
          <div class="space-y-3">
            <div
              v-for="contact in supportContacts"
              :key="contact.type"
              class="flex items-center gap-3 p-3 bg-gray-50 rounded-lg"
            >
              <i class="material-symbols-rounded text-gray-600">{{ contact.icon }}</i>
              <div>
                <p class="text-xs text-gray-500">{{ contact.type }}</p>
                <p class="text-sm font-medium text-gray-900">{{ contact.value }}</p>
              </div>
            </div>
          </div>
          <button
            class="w-full mt-4 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
          >
            Open Support Ticket
          </button>
        </div>

        <!-- Resources -->
        <div class="bg-white rounded-xl shadow-sm p-6">
          <h4 class="text-lg font-semibold text-gray-900 mb-4">Resources</h4>
          <div class="space-y-2">
            <a
              href="#"
              class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-50 rounded-lg transition-colors"
            >
              <i class="material-symbols-rounded text-base mr-2">description</i>
              User Manual
            </a>
            <a
              href="#"
              class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-50 rounded-lg transition-colors"
            >
              <i class="material-symbols-rounded text-base mr-2">video_library</i>
              Video Library
            </a>
            <a
              href="#"
              class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-50 rounded-lg transition-colors"
            >
              <i class="material-symbols-rounded text-base mr-2">forum</i>
              Community Forum
            </a>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>



