<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow">
      <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        <div class="md:flex md:items-center md:justify-between">
          <div class="flex-1 min-w-0">
            <h2 class="text-2xl font-bold leading-7 text-gray-900 dark:text-white sm:text-3xl sm:truncate">
              AI Co-Pilot Assistant
            </h2>
            <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
              Your intelligent business assistant powered by advanced AI technology
            </p>
          </div>
          <div class="mt-4 flex md:mt-0 md:ml-4 space-x-3">
            <button
              @click="clearConversation"
              type="button"
              class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600"
            >
              <svg class="-ml-1 mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
              Clear Chat
            </button>
            <button
              @click="refreshAI"
              type="button"
              class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-orange-600 hover:bg-orange-700"
            >
              <svg class="-ml-1 mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
              </svg>
              Refresh
            </button>
          </div>
        </div>
      </div>
    </div>

    <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        
        <!-- AI Chat Interface -->
        <div class="lg:col-span-2 space-y-6">
          <!-- Chat Messages -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg h-96 overflow-hidden">
            <div class="border-b border-gray-200 dark:border-gray-700 p-4">
              <h3 class="text-lg font-medium text-gray-900 dark:text-white">AI Conversation</h3>
              <p class="text-sm text-gray-500 dark:text-gray-400">Chat with your AI business assistant</p>
            </div>
            
            <div class="h-80 overflow-y-auto p-4 space-y-4" ref="messagesContainer">
              <!-- Welcome Message -->
              <div v-if="messages.length === 0" class="text-center py-8">
                <div class="inline-flex items-center justify-center w-16 h-16 bg-orange-100 dark:bg-orange-900 rounded-full mb-4">
                  <svg class="w-8 h-8 text-orange-600 dark:text-orange-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
                  </svg>
                </div>
                <h4 class="text-lg font-medium text-gray-900 dark:text-white mb-2">Welcome to your AI Assistant</h4>
                <p class="text-gray-500 dark:text-gray-400 max-w-md mx-auto">
                  I can help you with inventory management, financial analysis, business insights, and much more. 
                  Just ask me anything about your business!
                </p>
              </div>

              <!-- Messages -->
              <div v-for="message in messages" :key="message.id" class="flex" :class="message.sender === 'user' ? 'justify-end' : 'justify-start'">
                <div class="max-w-xs lg:max-w-md">
                  <div
                    class="px-4 py-2 rounded-lg"
                    :class="message.sender === 'user' 
                      ? 'bg-orange-600 text-white' 
                      : 'bg-gray-100 dark:bg-gray-700 text-gray-900 dark:text-white'"
                  >
                    <p class="text-sm whitespace-pre-wrap">{{ message.text }}</p>
                    <p class="text-xs mt-1 opacity-70">{{ formatTime(message.timestamp) }}</p>
                  </div>
                  
                  <!-- AI Actions -->
                  <div v-if="message.sender === 'ai' && message.actions" class="mt-2 space-y-1">
                    <button
                      v-for="action in message.actions"
                      :key="action.label"
                      @click="executeAction(action)"
                      class="block w-full text-left px-3 py-1 text-xs bg-white dark:bg-gray-600 border border-gray-200 dark:border-gray-500 rounded hover:bg-gray-50 dark:hover:bg-gray-500 transition-colors"
                    >
                      {{ action.label }}
                    </button>
                  </div>
                </div>
              </div>

              <!-- Typing Indicator -->
              <div v-if="isTyping" class="flex justify-start">
                <div class="bg-gray-100 dark:bg-gray-700 text-gray-900 dark:text-white max-w-xs px-4 py-2 rounded-lg">
                  <div class="flex space-x-1">
                    <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce"></div>
                    <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="animation-delay: 0.1s"></div>
                    <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="animation-delay: 0.2s"></div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Input Area -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-4">
            <div class="flex space-x-3">
              <div class="flex-1">
                <label for="message-input" class="sr-only">Message</label>
                <textarea
                  id="message-input"
                  v-model="newMessage"
                  @keydown.enter.prevent="sendMessage"
                  rows="3"
                  class="w-full px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-orange-500 focus:border-orange-500 dark:bg-gray-700 dark:text-white"
                  placeholder="Ask your AI assistant anything about your business..."
                  :disabled="isTyping"
                ></textarea>
              </div>
              <div class="flex flex-col space-y-2">
                <!-- Send Button -->
                <button
                  @click="sendMessage"
                  :disabled="!newMessage.trim() || isTyping"
                  class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-orange-600 hover:bg-orange-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-orange-500 disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
                  </svg>
                </button>
                
                <!-- Voice Input -->
                <button
                  @click="toggleVoiceInput"
                  :class="isListening ? 'bg-red-500 hover:bg-red-600' : 'bg-gray-500 hover:bg-gray-600'"
                  class="inline-flex items-center px-4 py-2 text-sm font-medium rounded-md shadow-sm text-white focus:outline-none focus:ring-2 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed"
                  :disabled="isTyping"
                >
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11a7 7 0 01-7 7m0 0a7 7 0 01-7-7m7 7v4m0 0H8m4 0h4m-4-8a3 3 0 01-3-3V5a3 3 0 116 0v6a3 3 0 01-3 3z" />
                  </svg>
                </button>
              </div>
            </div>

            <!-- Quick Actions -->
            <div class="mt-4 flex flex-wrap gap-2">
              <button
                v-for="action in quickActions"
                :key="action"
                @click="sendQuickMessage(action)"
                class="inline-flex items-center px-3 py-1 text-xs font-medium rounded-full text-orange-700 bg-orange-100 hover:bg-orange-200 dark:text-orange-300 dark:bg-orange-900 dark:hover:bg-orange-800"
              >
                {{ action }}
              </button>
            </div>
          </div>
        </div>

        <!-- AI Insights Panel -->
        <div class="space-y-6">
          <!-- AI Status -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">AI Status</h3>
            <div class="space-y-3">
              <div class="flex items-center">
                <div class="w-3 h-3 bg-green-400 rounded-full mr-3"></div>
                <span class="text-sm text-gray-600 dark:text-gray-300">AI Assistant Online</span>
              </div>
              <div class="flex items-center">
                <div class="w-3 h-3 bg-blue-400 rounded-full mr-3"></div>
                <span class="text-sm text-gray-600 dark:text-gray-300">Real-time Data Access</span>
              </div>
              <div class="flex items-center">
                <div class="w-3 h-3 bg-purple-400 rounded-full mr-3"></div>
                <span class="text-sm text-gray-600 dark:text-gray-300">Advanced Analytics Ready</span>
              </div>
            </div>
          </div>

          <!-- AI Capabilities -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">AI Capabilities</h3>
            <div class="space-y-3">
              <div class="flex items-start">
                <svg class="w-5 h-5 text-orange-500 mt-0.5 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4M7.835 4.697a3.42 3.42 0 001.946-.806 3.42 3.42 0 014.438 0 3.42 3.42 0 001.946.806 3.42 3.42 0 013.138 3.138 3.42 3.42 0 00.806 1.946 3.42 3.42 0 010 4.438 3.42 3.42 0 00-.806 1.946 3.42 3.42 0 01-3.138 3.138 3.42 3.42 0 00-1.946.806 3.42 3.42 0 01-4.438 0 3.42 3.42 0 00-1.946-.806 3.42 3.42 0 01-3.138-3.138 3.42 3.42 0 00-.806-1.946 3.42 3.42 0 010-4.438 3.42 3.42 0 00.806-1.946 3.42 3.42 0 013.138-3.138z" />
                </svg>
                <div>
                  <h4 class="text-sm font-medium text-gray-900 dark:text-white">Business Analytics</h4>
                  <p class="text-xs text-gray-500 dark:text-gray-400">AI-powered insights and recommendations</p>
                </div>
              </div>
              
              <div class="flex items-start">
                <svg class="w-5 h-5 text-orange-500 mt-0.5 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4M7.835 4.697a3.42 3.42 0 001.946-.806 3.42 3.42 0 014.438 0 3.42 3.42 0 001.946.806 3.42 3.42 0 013.138 3.138 3.42 3.42 0 00.806 1.946 3.42 3.42 0 010 4.438 3.42 3.42 0 00-.806 1.946 3.42 3.42 0 01-3.138 3.138 3.42 3.42 0 00-1.946.806 3.42 3.42 0 01-4.438 0 3.42 3.42 0 00-1.946-.806 3.42 3.42 0 01-3.138-3.138 3.42 3.42 0 00-.806-1.946 3.42 3.42 0 010-4.438 3.42 3.42 0 00.806-1.946 3.42 3.42 0 013.138-3.138z" />
                </svg>
                <div>
                  <h4 class="text-sm font-medium text-gray-900 dark:text-white">Natural Language</h4>
                  <p class="text-xs text-gray-500 dark:text-gray-400">Understand and respond in multiple languages</p>
                </div>
              </div>
              
              <div class="flex items-start">
                <svg class="w-5 h-5 text-orange-500 mt-0.5 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4M7.835 4.697a3.42 3.42 0 001.946-.806 3.42 3.42 0 014.438 0 3.42 3.42 0 001.946.806 3.42 3.42 0 013.138 3.138 3.42 3.42 0 00.806 1.946 3.42 3.42 0 010 4.438 3.42 3.42 0 00-.806 1.946 3.42 3.42 0 01-3.138 3.138 3.42 3.42 0 00-1.946.806 3.42 3.42 0 01-4.438 0 3.42 3.42 0 00-1.946-.806 3.42 3.42 0 01-3.138-3.138 3.42 3.42 0 00-.806-1.946 3.42 3.42 0 010-4.438 3.42 3.42 0 00.806-1.946 3.42 3.42 0 013.138-3.138z" />
                </svg>
                <div>
                  <h4 class="text-sm font-medium text-gray-900 dark:text-white">Task Automation</h4>
                  <p class="text-xs text-gray-500 dark:text-gray-400">Execute complex business workflows</p>
                </div>
              </div>
              
              <div class="flex items-start">
                <svg class="w-5 h-5 text-orange-500 mt-0.5 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4M7.835 4.697a3.42 3.42 0 001.946-.806 3.42 3.42 0 014.438 0 3.42 3.42 0 001.946.806 3.42 3.42 0 013.138 3.138 3.42 3.42 0 00.806 1.946 3.42 3.42 0 010 4.438 3.42 3.42 0 00-.806 1.946 3.42 3.42 0 01-3.138 3.138 3.42 3.42 0 00-1.946.806 3.42 3.42 0 01-4.438 0 3.42 3.42 0 00-1.946-.806 3.42 3.42 0 01-3.138-3.138 3.42 3.42 0 00-.806-1.946 3.42 3.42 0 010-4.438 3.42 3.42 0 00.806-1.946 3.42 3.42 0 013.138-3.138z" />
                </svg>
                <div>
                  <h4 class="text-sm font-medium text-gray-900 dark:text-white">Predictive Intelligence</h4>
                  <p class="text-xs text-gray-500 dark:text-gray-400">Forecast trends and business outcomes</p>
                </div>
              </div>
            </div>
          </div>

          <!-- Recent AI Actions -->
          <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white mb-4">Recent AI Actions</h3>
            <div class="space-y-3">
              <div v-for="action in recentActions" :key="action.id" class="flex items-start space-x-3">
                <div class="flex-shrink-0 w-2 h-2 mt-2 bg-orange-400 rounded-full"></div>
                <div class="flex-1 min-w-0">
                  <p class="text-sm text-gray-900 dark:text-white">{{ action.description }}</p>
                  <p class="text-xs text-gray-500 dark:text-gray-400">{{ formatTime(action.timestamp) }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, nextTick } from 'vue'

// Mock Nuxt functions
function navigateTo(route: string) {
  window.location.href = route
}

function definePageMeta(meta: any) {
  // Mock implementation for development
}

function useHead(options: any) {
  if (typeof document !== 'undefined') {
    document.title = options.title || 'AI Co-Pilot Assistant - TOSS ERP'
  }
}
// Vue composables are auto-imported in Nuxt 3

// Types
interface Message {
  id: string
  text: string
  sender: 'user' | 'ai'
  timestamp: Date
  actions?: Array<{
    type: string
    label: string
    data?: any
  }>
}

interface RecentAction {
  id: string
  description: string
  timestamp: Date
}

// Reactive data
const messages = ref<Message[]>([])
const newMessage = ref('')
const isTyping = ref(false)
const isListening = ref(false)
const messagesContainer = ref<HTMLElement>()

// Quick actions
const quickActions = [
  'Show today\'s sales',
  'Check inventory levels',
  'Generate financial report',
  'Analyze customer trends',
  'Suggest cost optimizations',
  'Create marketing campaign'
]

// Recent AI actions
const recentActions = ref<RecentAction[]>([
  {
    id: '1',
    description: 'Generated inventory optimization recommendations',
    timestamp: new Date(Date.now() - 300000) // 5 minutes ago
  },
  {
    id: '2',
    description: 'Analyzed sales performance for last month',
    timestamp: new Date(Date.now() - 900000) // 15 minutes ago
  },
  {
    id: '3',
    description: 'Created automated pricing suggestions',
    timestamp: new Date(Date.now() - 1800000) // 30 minutes ago
  }
])

// Methods
const sendMessage = async () => {
  if (!newMessage.value.trim() || isTyping.value) return

  const userMessage: Message = {
    id: Date.now().toString(),
    text: newMessage.value.trim(),
    sender: 'user',
    timestamp: new Date()
  }

  messages.value.push(userMessage)
  const userInput = newMessage.value.trim()
  newMessage.value = ''
  
  nextTick(() => {
    scrollToBottom()
  })

  // Show typing indicator
  isTyping.value = true
  await new Promise(resolve => setTimeout(resolve, 1500))

  // Generate AI response
  const aiResponse = await generateAIResponse(userInput)
  
  const aiMessage: Message = {
    id: (Date.now() + 1).toString(),
    text: aiResponse.reply,
    sender: 'ai',
    timestamp: new Date(),
    actions: aiResponse.actions
  }

  messages.value.push(aiMessage)
  isTyping.value = false

  nextTick(() => {
    scrollToBottom()
  })

  // Add to recent actions
  recentActions.value.unshift({
    id: Date.now().toString(),
    description: `Responded to: "${userInput.substring(0, 50)}${userInput.length > 50 ? '...' : ''}"`,
    timestamp: new Date()
  })
  
  // Keep only last 5 actions
  if (recentActions.value.length > 5) {
    recentActions.value = recentActions.value.slice(0, 5)
  }
}

const sendQuickMessage = (message: string) => {
  newMessage.value = message
  sendMessage()
}

const generateAIResponse = async (userInput: string) => {
  // Simulate AI response based on input
  const input = userInput.toLowerCase()
  
  if (input.includes('sales') || input.includes('revenue')) {
    return {
      reply: "Based on your current sales data, I can see that your revenue for this month is R 124,500, which is 23% higher than last month. Your top-selling categories are groceries (45%) and household items (32%). Would you like me to generate a detailed sales report or provide recommendations for increasing sales?",
      actions: [
        { type: 'navigate', label: 'View Sales Dashboard', data: { route: '/sales' } },
        { type: 'generate', label: 'Generate Sales Report', data: { type: 'sales' } },
        { type: 'analyze', label: 'Sales Trend Analysis', data: { period: 'monthly' } }
      ]
    }
  } else if (input.includes('inventory') || input.includes('stock')) {
    return {
      reply: "Your current inventory shows 347 items in stock with a total value of R 89,200. I've identified 12 items that are running low and 5 items that are overstocked. Your inventory turnover rate is healthy at 8.2 times per year. Shall I create automatic reorder recommendations?",
      actions: [
        { type: 'navigate', label: 'View Stock Management', data: { route: '/stock' } },
        { type: 'generate', label: 'Low Stock Report', data: { type: 'low-stock' } },
        { type: 'automate', label: 'Setup Auto-Reorder', data: { feature: 'auto-reorder' } }
      ]
    }
  } else if (input.includes('financial') || input.includes('finance') || input.includes('money')) {
    return {
      reply: "Your financial overview shows strong performance: Monthly profit margin of 18.5%, cash flow is positive at R 34,800, and your debt-to-equity ratio is healthy at 0.35. I recommend reviewing your expense categories as I've identified potential savings of R 8,500 monthly. Would you like me to analyze your financial health in detail?",
      actions: [
        { type: 'navigate', label: 'View Financial Dashboard', data: { route: '/finance' } },
        { type: 'generate', label: 'Financial Health Report', data: { type: 'financial-health' } },
        { type: 'optimize', label: 'Cost Optimization Plan', data: { feature: 'cost-optimize' } }
      ]
    }
  } else if (input.includes('customer') || input.includes('marketing')) {
    return {
      reply: "Your customer analytics show 1,247 active customers with an average order value of R 156. Customer retention rate is 78%, and your best customers are in the 25-45 age group. I can help create targeted marketing campaigns or loyalty programs to increase customer engagement. What would you like to focus on?",
      actions: [
        { type: 'navigate', label: 'View Marketing Dashboard', data: { route: '/marketing' } },
        { type: 'generate', label: 'Customer Segmentation Report', data: { type: 'customer-segments' } },
        { type: 'create', label: 'Design Marketing Campaign', data: { feature: 'campaign-builder' } }
      ]
    }
  } else if (input.includes('predict') || input.includes('forecast')) {
    return {
      reply: "Based on historical data and current trends, I predict your sales will increase by 15% next month, primarily driven by seasonal demand. Your top 3 products will likely be Maize Meal (↑22%), Cooking Oil (↑18%), and Cleaning Products (↑12%). I recommend increasing stock for these items. Shall I create a detailed forecast report?",
      actions: [
        { type: 'generate', label: 'Detailed Forecast Report', data: { type: 'forecast' } },
        { type: 'optimize', label: 'Stock Level Recommendations', data: { feature: 'stock-optimize' } },
        { type: 'automate', label: 'Setup Demand Forecasting', data: { feature: 'demand-forecast' } }
      ]
    }
  } else {
    return {
      reply: "I'm your AI business assistant for TOSS ERP! I can help you with:\n\n• Sales and revenue analysis\n• Inventory management and optimization\n• Financial planning and insights\n• Customer analytics and marketing\n• Business forecasting and predictions\n• Process automation and workflows\n\nWhat would you like to explore today?",
      actions: [
        { type: 'navigate', label: 'View Dashboard', data: { route: '/dashboard' } },
        { type: 'analyze', label: 'Business Health Check', data: { type: 'health-check' } },
        { type: 'tutorial', label: 'Show AI Features', data: { feature: 'tutorial' } }
      ]
    }
  }
}

const executeAction = (action: any) => {
  switch (action.type) {
    case 'navigate':
      navigateTo(action.data.route)
      break
    case 'generate':
      generateReport(action.data.type)
      break
    case 'analyze':
      performAnalysis(action.data)
      break
    case 'automate':
      setupAutomation(action.data.feature)
      break
    case 'optimize':
      optimizeProcess(action.data.feature)
      break
    case 'create':
      createContent(action.data.feature)
      break
    case 'tutorial':
      showTutorial(action.data.feature)
      break
    default:
      console.log('Action executed:', action)
  }
}

const toggleVoiceInput = () => {
  if (!isListening.value) {
    startVoiceRecognition()
  } else {
    stopVoiceRecognition()
  }
}

const startVoiceRecognition = () => {
  if ('webkitSpeechRecognition' in window || 'SpeechRecognition' in window) {
    const SpeechRecognition = (window as any).SpeechRecognition || (window as any).webkitSpeechRecognition
    const recognition = new SpeechRecognition()
    
    recognition.continuous = false
    recognition.interimResults = false
    recognition.lang = 'en-US'
    
    recognition.onstart = () => {
      isListening.value = true
    }
    
    recognition.onresult = (event: any) => {
      const transcript = event.results[0][0].transcript
      newMessage.value = transcript
      sendMessage()
    }
    
    recognition.onend = () => {
      isListening.value = false
    }
    
    recognition.start()
  } else {
    alert('Speech recognition is not supported in this browser.')
  }
}

const stopVoiceRecognition = () => {
  isListening.value = false
}

const clearConversation = () => {
  messages.value = []
}

const refreshAI = () => {
  // Simulate AI refresh
  console.log('AI system refreshed')
}

const formatTime = (date: Date): string => {
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

const scrollToBottom = () => {
  if (messagesContainer.value) {
    messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
  }
}

// Helper functions for actions
const generateReport = (type: string) => {
  console.log(`Generating ${type} report...`)
  // Implement report generation
}

const performAnalysis = (data: any) => {
  console.log('Performing analysis:', data)
  // Implement analysis functionality
}

const setupAutomation = (feature: string) => {
  console.log(`Setting up ${feature} automation...`)
  // Implement automation setup
}

const optimizeProcess = (feature: string) => {
  console.log(`Optimizing ${feature}...`)
  // Implement optimization
}

const createContent = (feature: string) => {
  console.log(`Creating ${feature}...`)
  // Implement content creation
}

const showTutorial = (feature: string) => {
  console.log(`Showing ${feature} tutorial...`)
  // Implement tutorial display
}

// Page meta
definePageMeta({
  title: 'AI Co-Pilot Assistant',
  description: 'Intelligent business assistant powered by advanced AI technology'
})

// SEO
useHead({
  title: 'AI Co-Pilot Assistant - TOSS ERP',
  meta: [
    { name: 'description', content: 'Chat with your intelligent AI business assistant. Get insights, automate tasks, and optimize your business operations with advanced AI technology.' }
  ]
})
</script>

<style scoped>
.animate-bounce {
  animation: bounce 1s infinite;
}

@keyframes bounce {
  0%, 20%, 53%, 80%, 100% {
    transform: translate3d(0, 0, 0);
  }
  40%, 43% {
    transform: translate3d(0, -8px, 0);
  }
  70% {
    transform: translate3d(0, -4px, 0);
  }
  90% {
    transform: translate3d(0, -2px, 0);
  }
}
</style>
