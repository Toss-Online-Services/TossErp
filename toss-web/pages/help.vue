<template>
  <div class="h-full overflow-y-auto p-6 custom-scrollbar">
    <!-- Help & Support Page -->
    <div class="text-center mb-8">
      <div class="w-16 h-16 rounded-2xl bg-gradient-to-br from-stone-700 to-stone-900 dark:from-stone-600 dark:to-stone-800 flex items-center justify-center mx-auto mb-4">
        <Icon name="lucide:help-circle" class="w-8 h-8 text-white" />
      </div>
      <h1 class="text-3xl font-bold text-stone-900 dark:text-white mb-2">Help & Support</h1>
      <p class="text-stone-500 dark:text-stone-400">Find answers and get help with TOSS ERP</p>
    </div>

    <!-- Search -->
    <div class="max-w-2xl mx-auto mb-8">
      <div class="relative">
        <Icon name="lucide:search" class="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-stone-400" />
        <input 
          type="text" 
          placeholder="Search help articles, guides, and FAQs..." 
          class="w-full pl-12 pr-4 py-4 border border-stone-200 dark:border-stone-700 rounded-xl bg-white dark:bg-stone-800 text-stone-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-stone-500 text-lg"
        />
      </div>
    </div>

    <!-- Quick Links -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-8">
      <Card v-for="link in quickLinks" :key="link.title" class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700 hover:shadow-lg transition-all cursor-pointer">
        <CardContent class="p-6 text-center">
          <div :class="['w-12 h-12 rounded-xl flex items-center justify-center mx-auto mb-3', link.bgColor]">
            <Icon :name="link.icon" :class="['w-6 h-6', link.iconColor]" />
          </div>
          <h3 class="text-sm font-semibold text-stone-900 dark:text-white">{{ link.title }}</h3>
        </CardContent>
      </Card>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Popular Articles -->
      <Card class="lg:col-span-2 bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardHeader class="border-b border-stone-200 dark:border-stone-700">
          <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Popular Articles</CardTitle>
        </CardHeader>
        <CardContent class="p-0">
          <div class="divide-y divide-stone-200 dark:divide-stone-700">
            <a v-for="article in articles" :key="article.id" href="#" class="flex items-center gap-4 p-4 hover:bg-stone-50 dark:hover:bg-stone-700">
              <div :class="['w-10 h-10 rounded-lg flex items-center justify-center flex-shrink-0', article.bgColor]">
                <Icon :name="article.icon" :class="['w-5 h-5', article.iconColor]" />
              </div>
              <div class="flex-1 min-w-0">
                <h4 class="text-sm font-medium text-stone-900 dark:text-white truncate">{{ article.title }}</h4>
                <p class="text-xs text-stone-500 dark:text-stone-400">{{ article.category }}</p>
              </div>
              <Icon name="lucide:chevron-right" class="w-5 h-5 text-stone-400" />
            </a>
          </div>
        </CardContent>
      </Card>

      <!-- Contact Support -->
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardHeader class="border-b border-stone-200 dark:border-stone-700">
          <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Contact Support</CardTitle>
        </CardHeader>
        <CardContent class="p-6 space-y-4">
          <div class="flex items-center gap-4 p-4 bg-stone-50 dark:bg-stone-900 rounded-lg">
            <div class="w-10 h-10 rounded-full bg-green-100 dark:bg-green-900/30 flex items-center justify-center">
              <Icon name="lucide:message-circle" class="w-5 h-5 text-green-600" />
            </div>
            <div>
              <h4 class="text-sm font-medium text-stone-900 dark:text-white">WhatsApp</h4>
              <p class="text-xs text-stone-500">+27 80 123 4567</p>
            </div>
          </div>
          <div class="flex items-center gap-4 p-4 bg-stone-50 dark:bg-stone-900 rounded-lg">
            <div class="w-10 h-10 rounded-full bg-blue-100 dark:bg-blue-900/30 flex items-center justify-center">
              <Icon name="lucide:mail" class="w-5 h-5 text-blue-600" />
            </div>
            <div>
              <h4 class="text-sm font-medium text-stone-900 dark:text-white">Email</h4>
              <p class="text-xs text-stone-500">support@toss.co.za</p>
            </div>
          </div>
          <div class="flex items-center gap-4 p-4 bg-stone-50 dark:bg-stone-900 rounded-lg">
            <div class="w-10 h-10 rounded-full bg-purple-100 dark:bg-purple-900/30 flex items-center justify-center">
              <Icon name="lucide:phone" class="w-5 h-5 text-purple-600" />
            </div>
            <div>
              <h4 class="text-sm font-medium text-stone-900 dark:text-white">Phone</h4>
              <p class="text-xs text-stone-500">0800 TOSS SA</p>
            </div>
          </div>
          <Button class="w-full bg-stone-900 hover:bg-stone-800 dark:bg-white dark:hover:bg-stone-100 dark:text-stone-900">
            <Icon name="lucide:headphones" class="w-5 h-5 mr-2" />
            Request Callback
          </Button>
        </CardContent>
      </Card>
    </div>

    <!-- FAQs -->
    <Card class="mt-6 bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
      <CardHeader class="border-b border-stone-200 dark:border-stone-700">
        <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Frequently Asked Questions</CardTitle>
      </CardHeader>
      <CardContent class="p-0">
        <div class="divide-y divide-stone-200 dark:divide-stone-700">
          <div v-for="faq in faqs" :key="faq.id" class="p-4">
            <button class="flex items-center justify-between w-full text-left" @click="faq.open = !faq.open">
              <h4 class="text-sm font-medium text-stone-900 dark:text-white">{{ faq.question }}</h4>
              <Icon :name="faq.open ? 'lucide:chevron-up' : 'lucide:chevron-down'" class="w-5 h-5 text-stone-400" />
            </button>
            <p v-if="faq.open" class="mt-2 text-sm text-stone-500 dark:text-stone-400">{{ faq.answer }}</p>
          </div>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { Card, CardContent, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'

definePageMeta({
  layout: 'default'
})

const quickLinks = [
  { title: 'Getting Started', icon: 'lucide:rocket', bgColor: 'bg-blue-100 dark:bg-blue-900/30', iconColor: 'text-blue-600' },
  { title: 'Video Tutorials', icon: 'lucide:play-circle', bgColor: 'bg-red-100 dark:bg-red-900/30', iconColor: 'text-red-600' },
  { title: 'User Guides', icon: 'lucide:book-open', bgColor: 'bg-green-100 dark:bg-green-900/30', iconColor: 'text-green-600' },
  { title: 'Community', icon: 'lucide:users', bgColor: 'bg-purple-100 dark:bg-purple-900/30', iconColor: 'text-purple-600' },
]

const articles = [
  { id: 1, title: 'How to process a sale at the POS', category: 'Point of Sale', icon: 'lucide:shopping-cart', bgColor: 'bg-blue-100 dark:bg-blue-900/30', iconColor: 'text-blue-600' },
  { id: 2, title: 'Managing your inventory effectively', category: 'Stock Management', icon: 'lucide:package', bgColor: 'bg-green-100 dark:bg-green-900/30', iconColor: 'text-green-600' },
  { id: 3, title: 'Setting up customer credit accounts', category: 'Customers', icon: 'lucide:credit-card', bgColor: 'bg-purple-100 dark:bg-purple-900/30', iconColor: 'text-purple-600' },
  { id: 4, title: 'Creating and managing purchase orders', category: 'Purchasing', icon: 'lucide:file-text', bgColor: 'bg-orange-100 dark:bg-orange-900/30', iconColor: 'text-orange-600' },
  { id: 5, title: 'Understanding your sales reports', category: 'Reports', icon: 'lucide:bar-chart', bgColor: 'bg-teal-100 dark:bg-teal-900/30', iconColor: 'text-teal-600' },
]

const faqs = ref([
  { id: 1, question: 'How do I add a new product to my inventory?', answer: 'Go to Stock > Products, click "Add Product", fill in the product details including name, price, and quantity, then save.', open: false },
  { id: 2, question: 'Can I use TOSS offline?', answer: 'Yes! TOSS works offline and will sync automatically when you reconnect to the internet.', open: false },
  { id: 3, question: 'How do I process a refund?', answer: 'From the POS, select the original sale from history, click "Refund", select items to refund, and process the return.', open: false },
  { id: 4, question: 'How do I set up DebiCheck for customer payments?', answer: 'Go to Settings > Payments > DebiCheck, link your bank account, and follow the verification steps.', open: false },
])
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: hsl(var(--muted)); }
.custom-scrollbar::-webkit-scrollbar-thumb { background: hsl(var(--muted-foreground)); border-radius: 3px; }
</style>
