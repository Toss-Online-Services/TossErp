<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <!-- Mobile-First Page Container -->
    <div class="p-4 sm:p-6 space-y-4 sm:space-y-6 pb-20 lg:pb-6">
      <!-- Page Header -->
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-3 sm:gap-0">
        <div>
          <div class="flex items-center gap-3">
            <div class="w-8 h-8 sm:w-10 sm:h-10 rounded-full bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center">
              <SparklesIcon class="w-4 h-4 sm:w-5 sm:h-5 text-white" />
            </div>
            <div>
              <h1 class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">Sales AI Assistant</h1>
              <p class="text-slate-600 dark:text-slate-400 mt-1 text-sm sm:text-base">Your intelligent sales companion for Thabo's Spaza Shop</p>
            </div>
          </div>
        </div>
        <div class="flex gap-2 sm:gap-3">
          <button @click="clearChat" 
                  class="px-4 py-2 text-slate-600 dark:text-slate-400 hover:text-slate-900 dark:hover:text-white border border-slate-300 dark:border-slate-600 rounded-lg transition-colors">
            Clear Chat
          </button>
        </div>
      </div>

      <!-- AI Assistant Chat Interface -->
      <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700 flex flex-col h-[70vh]">
        <!-- Chat Header -->
        <div class="p-4 border-b border-slate-200 dark:border-slate-700">
          <div class="flex items-center gap-3">
            <div class="w-3 h-3 bg-green-500 rounded-full animate-pulse"></div>
            <span class="text-sm font-medium text-slate-900 dark:text-white">AI Assistant Online</span>
            <span class="text-xs text-slate-500 bg-slate-100 dark:bg-slate-700 px-2 py-1 rounded-full">Sales Expert</span>
          </div>
        </div>

        <!-- Chat Messages -->
        <div class="flex-1 p-4 overflow-y-auto space-y-4" ref="chatContainer">
          <!-- Welcome Message -->
          <div v-if="messages.length === 0" class="space-y-4">
            <div class="flex items-start gap-3">
              <div class="w-8 h-8 rounded-full bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center flex-shrink-0">
                <SparklesIcon class="w-4 h-4 text-white" />
              </div>
              <div class="bg-slate-50 dark:bg-slate-700 p-4 rounded-lg rounded-tl-none max-w-sm">
                <p class="text-slate-900 dark:text-white">Hello! I'm your Sales AI Assistant. I can help you with:</p>
                <ul class="mt-2 text-sm text-slate-600 dark:text-slate-400 space-y-1">
                  <li>• Sales forecasting and predictions</li>
                  <li>• Customer behavior analysis</li>
                  <li>• Inventory optimization</li>
                  <li>• Pricing strategies</li>
                  <li>• Revenue insights</li>
                </ul>
              </div>
            </div>

            <!-- Quick Actions -->
            <div class="ml-11">
              <p class="text-sm text-slate-600 dark:text-slate-400 mb-3">Quick actions to get started:</p>
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-2">
                <button v-for="action in quickActions" :key="action.id"
                        @click="sendQuickAction(action.message)"
                        class="text-left p-3 bg-blue-50 dark:bg-blue-900/20 hover:bg-blue-100 dark:hover:bg-blue-900/40 rounded-lg transition-colors border border-blue-200 dark:border-blue-800">
                  <div class="flex items-center gap-2">
                    <component :is="action.icon" class="w-4 h-4 text-blue-600 dark:text-blue-400" />
                    <span class="text-sm font-medium text-blue-900 dark:text-blue-100">{{ action.title }}</span>
                  </div>
                  <p class="text-xs text-blue-700 dark:text-blue-300 mt-1">{{ action.description }}</p>
                </button>
              </div>
            </div>
          </div>

          <!-- Chat Messages -->
          <div v-for="message in messages" :key="message.id" class="flex items-start gap-3"
               :class="message.type === 'user' ? 'flex-row-reverse' : ''">
            <div v-if="message.type === 'assistant'" 
                 class="w-8 h-8 rounded-full bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center flex-shrink-0">
              <SparklesIcon class="w-4 h-4 text-white" />
            </div>
            <div v-else 
                 class="w-8 h-8 rounded-full bg-slate-200 dark:bg-slate-600 flex items-center justify-center flex-shrink-0">
              <UserIcon class="w-4 h-4 text-slate-600 dark:text-slate-400" />
            </div>
            
            <div class="p-4 rounded-lg max-w-sm sm:max-w-md lg:max-w-lg"
                 :class="message.type === 'user' 
                   ? 'bg-blue-600 text-white rounded-br-none' 
                   : 'bg-slate-50 dark:bg-slate-700 text-slate-900 dark:text-white rounded-tl-none'">
              <div v-if="message.type === 'assistant' && message.data" class="space-y-3">
                <!-- Text Response -->
                <p v-if="message.content">{{ message.content }}</p>
                
                <!-- Data Visualization -->
                <div v-if="message.data.metrics" class="grid grid-cols-2 gap-2 text-xs">
                  <div v-for="metric in message.data.metrics" :key="metric.label"
                       class="bg-white dark:bg-slate-800 p-2 rounded border">
                    <p class="text-slate-600 dark:text-slate-400">{{ metric.label }}</p>
                    <p class="font-bold text-slate-900 dark:text-white">{{ metric.value }}</p>
                  </div>
                </div>

                <!-- Recommendations -->
                <div v-if="message.data.recommendations" class="space-y-2">
                  <h4 class="font-medium text-sm">Recommendations:</h4>
                  <ul class="space-y-1 text-sm">
                    <li v-for="rec in message.data.recommendations" :key="rec" 
                        class="flex items-start gap-2">
                      <ChevronRightIcon class="w-3 h-3 mt-0.5 text-blue-500 flex-shrink-0" />
                      <span>{{ rec }}</span>
                    </li>
                  </ul>
                </div>
              </div>
              <p v-else>{{ message.content }}</p>
              <p class="text-xs opacity-70 mt-2">{{ formatTime(message.timestamp) }}</p>
            </div>
          </div>

          <!-- Typing Indicator -->
          <div v-if="isTyping" class="flex items-start gap-3">
            <div class="w-8 h-8 rounded-full bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center flex-shrink-0">
              <SparklesIcon class="w-4 h-4 text-white" />
            </div>
            <div class="bg-slate-50 dark:bg-slate-700 p-4 rounded-lg rounded-tl-none">
              <div class="flex space-x-1">
                <div class="w-2 h-2 bg-slate-400 rounded-full animate-bounce"></div>
                <div class="w-2 h-2 bg-slate-400 rounded-full animate-bounce" style="animation-delay: 0.1s"></div>
                <div class="w-2 h-2 bg-slate-400 rounded-full animate-bounce" style="animation-delay: 0.2s"></div>
              </div>
            </div>
          </div>
        </div>

        <!-- Input Area -->
        <div class="p-4 border-t border-slate-200 dark:border-slate-700">
          <div class="flex gap-3">
            <input 
              v-model="newMessage"
              @keypress.enter="sendMessage"
              type="text"
              placeholder="Ask me about sales insights, forecasts, or recommendations..."
              class="flex-1 px-4 py-3 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white"
            />
            <button 
              @click="sendMessage"
              :disabled="!newMessage.trim() || isTyping"
              class="px-6 py-3 bg-blue-600 hover:bg-blue-700 disabled:bg-slate-400 text-white rounded-lg transition-colors flex items-center gap-2">
              <PaperAirplaneIcon class="w-4 h-4" />
            </button>
          </div>
        </div>
      </div>

      <!-- AI Insights Panel -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-4 sm:gap-6">
        <!-- Real-time Insights -->
        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center gap-2 mb-4">
            <ChartBarIcon class="w-5 h-5 text-blue-600" />
            <h3 class="font-semibold text-slate-900 dark:text-white">Live Insights</h3>
          </div>
          <div class="space-y-3">
            <div v-for="insight in liveInsights" :key="insight.id" 
                 class="flex items-center justify-between text-sm">
              <span class="text-slate-600 dark:text-slate-400">{{ insight.label }}</span>
              <span class="font-medium text-slate-900 dark:text-white">{{ insight.value }}</span>
            </div>
          </div>
        </div>

        <!-- Quick Analytics -->
        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center gap-2 mb-4">
            <CalculatorIcon class="w-5 h-5 text-green-600" />
            <h3 class="font-semibold text-slate-900 dark:text-white">Quick Analytics</h3>
          </div>
          <div class="space-y-3">
            <div class="text-center p-3 bg-green-50 dark:bg-green-900/20 rounded-lg">
              <p class="text-xs text-slate-600 dark:text-slate-400">Today's Revenue</p>
              <p class="text-lg font-bold text-green-600">R {{ formatCurrency(todayRevenue) }}</p>
            </div>
            <div class="text-center p-3 bg-blue-50 dark:bg-blue-900/20 rounded-lg">
              <p class="text-xs text-slate-600 dark:text-slate-400">Avg. Order Value</p>
              <p class="text-lg font-bold text-blue-600">R {{ formatCurrency(avgOrderValue) }}</p>
            </div>
          </div>
        </div>

        <!-- AI Status -->
        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center gap-2 mb-4">
            <SparklesIcon class="w-5 h-5 text-purple-600" />
            <h3 class="font-semibold text-slate-900 dark:text-white">AI Status</h3>
          </div>
          <div class="space-y-3">
            <div class="flex items-center justify-between">
              <span class="text-sm text-slate-600 dark:text-slate-400">Model</span>
              <span class="text-sm font-medium text-green-600">GPT-4 Sales</span>
            </div>
            <div class="flex items-center justify-between">
              <span class="text-sm text-slate-600 dark:text-slate-400">Accuracy</span>
              <span class="text-sm font-medium text-slate-900 dark:text-white">94.7%</span>
            </div>
            <div class="flex items-center justify-between">
              <span class="text-sm text-slate-600 dark:text-slate-400">Last Updated</span>
              <span class="text-sm font-medium text-slate-900 dark:text-white">2 min ago</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, nextTick } from 'vue'
import { 
  SparklesIcon,
  UserIcon,
  PaperAirplaneIcon,
  ChartBarIcon,
  CalculatorIcon,
  CurrencyDollarIcon,
  ShoppingCartIcon,
  UsersIcon,
  ArrowTrendingUpIcon,
  ChevronRightIcon,
  LightBulbIcon,
  ClockIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Sales AI Assistant - TOSS ERP',
  meta: [
    { name: 'description', content: 'Intelligent sales assistant for Thabo\'s Spaza Shop' }
  ]
})

// Layout
definePageMeta({
  layout: 'default'
})

// Chat state
const messages = ref<Array<{
  id: string
  type: 'user' | 'assistant'
  content: string
  timestamp: Date
  data?: any
}>>([])

const newMessage = ref('')
const isTyping = ref(false)
const chatContainer = ref<HTMLElement>()

// Quick actions
const quickActions = ref([
  {
    id: '1',
    title: 'Sales Forecast',
    description: 'Predict next week revenue',
    icon: ArrowTrendingUpIcon,
    message: 'What will my sales be next week?'
  },
  {
    id: '2',
    title: 'Top Products',
    description: 'Analyze best sellers',
    icon: ShoppingCartIcon,
    message: 'Which products are performing best?'
  },
  {
    id: '3',
    title: 'Customer Insights',
    description: 'Understand buying patterns',
    icon: UsersIcon,
    message: 'Tell me about my customer buying patterns'
  },
  {
    id: '4',
    title: 'Pricing Strategy',
    description: 'Optimize pricing',
    icon: CurrencyDollarIcon,
    message: 'How can I optimize my pricing strategy?'
  }
])

// Live insights
const liveInsights = ref([
  { id: '1', label: 'Active Customers', value: '23 online' },
  { id: '2', label: 'Conversion Rate', value: '12.4%' },
  { id: '3', label: 'Cart Abandonment', value: '8.7%' },
  { id: '4', label: 'Inventory Alerts', value: '3 items' }
])

// Analytics data
const todayRevenue = ref(2450)
const avgOrderValue = ref(186)

// Helper functions
const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

const formatTime = (date: Date) => {
  return date.toLocaleTimeString('en-ZA', { 
    hour: '2-digit', 
    minute: '2-digit' 
  })
}

// Chat functions
const sendMessage = async () => {
  if (!newMessage.value.trim() || isTyping.value) return

  const userMessage = {
    id: Date.now().toString(),
    type: 'user' as const,
    content: newMessage.value,
    timestamp: new Date()
  }

  messages.value.push(userMessage)
  const messageText = newMessage.value
  newMessage.value = ''
  
  await scrollToBottom()
  await simulateAIResponse(messageText)
}

const sendQuickAction = async (message: string) => {
  newMessage.value = message
  await sendMessage()
}

const simulateAIResponse = async (userMessage: string) => {
  isTyping.value = true
  
  // Simulate typing delay
  await new Promise(resolve => setTimeout(resolve, 1500))
  
  let response = generateAIResponse(userMessage)
  
  messages.value.push({
    id: Date.now().toString(),
    type: 'assistant',
    content: response.content,
    timestamp: new Date(),
    data: response.data
  })
  
  isTyping.value = false
  await scrollToBottom()
}

const generateAIResponse = (userMessage: string) => {
  const lowerMessage = userMessage.toLowerCase()
  
  if (lowerMessage.includes('forecast') || lowerMessage.includes('predict') || lowerMessage.includes('next week')) {
    return {
      content: "Based on your sales patterns and seasonal trends, I predict next week's revenue will be R19,600 with 87% confidence.",
      data: {
        metrics: [
          { label: 'Next Week', value: 'R19,600' },
          { label: 'Confidence', value: '87%' },
          { label: 'Growth', value: '+12%' },
          { label: 'Peak Day', value: 'Friday' }
        ],
        recommendations: [
          'Stock up on weekend essentials',
          'Prepare for Friday rush hour',
          'Consider bulk buying promotions'
        ]
      }
    }
  }
  
  if (lowerMessage.includes('product') || lowerMessage.includes('best seller') || lowerMessage.includes('performing')) {
    return {
      content: "Your top performing products show strong demand for essentials and beverages.",
      data: {
        metrics: [
          { label: 'Top Seller', value: 'White Bread' },
          { label: 'Units Sold', value: '156' },
          { label: 'Revenue', value: 'R2,808' },
          { label: 'Margin', value: '35%' }
        ],
        recommendations: [
          'Increase bread stock during peak hours',
          'Bundle bread with margarine/butter',
          'Monitor competitor pricing'
        ]
      }
    }
  }
  
  if (lowerMessage.includes('customer') || lowerMessage.includes('buying') || lowerMessage.includes('pattern')) {
    return {
      content: "Your customers show strong loyalty with 68% being repeat buyers. Peak shopping is 4-7PM.",
      data: {
        metrics: [
          { label: 'Repeat Rate', value: '68%' },
          { label: 'Avg Visit', value: '3.2/week' },
          { label: 'Peak Hours', value: '4-7PM' },
          { label: 'Basket Size', value: '4.2 items' }
        ],
        recommendations: [
          'Create loyalty rewards program',
          'Offer bulk discounts for frequent buyers',
          'Optimize staff schedule for peak hours'
        ]
      }
    }
  }
  
  if (lowerMessage.includes('pric') || lowerMessage.includes('optim') || lowerMessage.includes('strategy')) {
    return {
      content: "Your pricing is competitive, but there's room for optimization through dynamic pricing and bundles.",
      data: {
        metrics: [
          { label: 'Margin', value: '28%' },
          { label: 'Competitor Gap', value: '+5%' },
          { label: 'Price Sensitivity', value: 'Medium' },
          { label: 'Opportunity', value: 'R3,200' }
        ],
        recommendations: [
          'Test dynamic pricing on beverages',
          'Create combo deals for snacks',
          'Implement seasonal pricing adjustments'
        ]
      }
    }
  }
  
  // Default response
  return {
    content: "I'm here to help with your sales insights! You can ask me about forecasts, customer behavior, product performance, or pricing strategies.",
    data: null
  }
}

const clearChat = () => {
  messages.value = []
}

const scrollToBottom = async () => {
  await nextTick()
  if (chatContainer.value) {
    chatContainer.value.scrollTop = chatContainer.value.scrollHeight
  }
}
</script>
