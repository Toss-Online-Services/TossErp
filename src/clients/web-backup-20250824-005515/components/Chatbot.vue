<template>
  <div class="fixed bottom-4 right-4 z-50">
    <!-- Chatbot Toggle Button -->
    <button
      @click="toggleChatbot"
      class="bg-orange-600 hover:bg-orange-700 text-white rounded-full p-4 shadow-lg transition-all duration-300 transform hover:scale-110"
      :class="{ 'rotate-45': isOpen }"
      data-testid="chatbot-toggle"
    >
      <svg v-if="!isOpen" class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
      </svg>
      <svg v-else class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
      </svg>
    </button>

    <!-- Chatbot Window -->
    <div
      v-show="isOpen"
      class="absolute bottom-16 right-0 w-96 h-[500px] bg-white rounded-lg shadow-2xl border border-gray-200 flex flex-col"
      data-testid="chatbot-window"
    >
      <!-- Header -->
      <div class="bg-orange-600 text-white px-4 py-3 rounded-t-lg flex items-center justify-between">
        <div class="flex items-center">
          <div class="w-8 h-8 bg-white/20 rounded-full flex items-center justify-center mr-3">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
            </svg>
          </div>
          <div>
            <h3 class="font-semibold">TOSS ERP Assistant</h3>
            <p class="text-xs text-orange-100">{{ isTyping ? 'Typing...' : 'Online' }}</p>
          </div>
        </div>
        <div class="flex items-center space-x-2">
          <button @click="clearChat" class="text-orange-100 hover:text-white p-1" title="Clear chat">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
            </svg>
          </button>
          <button @click="toggleChatbot" class="text-orange-100 hover:text-white p-1">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
      </div>

      <!-- Messages Area -->
      <div class="flex-1 overflow-y-auto p-4 space-y-4" ref="messagesContainer">
        <!-- Welcome Message -->
        <div v-if="messages.length === 0" class="text-center text-gray-500 py-8">
          <svg class="w-12 h-12 mx-auto mb-4 text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
          </svg>
          <p class="text-sm">Hello! I'm your TOSS ERP assistant.</p>
          <p class="text-xs mt-1">Ask me about inventory, sales, or any business questions.</p>
        </div>

        <!-- Messages -->
        <div v-for="message in messages" :key="message.id" class="flex" :class="message.sender === 'user' ? 'justify-end' : 'justify-start'">
          <div
            class="max-w-xs px-4 py-2 rounded-lg"
            :class="message.sender === 'user' 
              ? 'bg-orange-600 text-white' 
              : 'bg-gray-100 text-gray-900'"
          >
            <p class="text-sm whitespace-pre-wrap">{{ message.text }}</p>
            <p class="text-xs mt-1 opacity-70">{{ formatTime(message.timestamp) }}</p>
            <!-- Message Actions for Bot Messages -->
            <div v-if="message.sender === 'bot' && message.actions" class="mt-2 space-y-1">
              <button
                v-for="action in message.actions"
                :key="action.label"
                @click="executeAction(action)"
                class="block w-full text-left px-2 py-1 text-xs bg-white/20 rounded hover:bg-white/30 transition-colors"
              >
                {{ action.label }}
              </button>
            </div>
          </div>
        </div>

        <!-- Typing Indicator -->
        <div v-if="isTyping" class="flex justify-start">
          <div class="bg-gray-100 text-gray-900 max-w-xs px-4 py-2 rounded-lg">
            <div class="flex space-x-1">
              <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce"></div>
              <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="animation-delay: 0.1s"></div>
              <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="animation-delay: 0.2s"></div>
            </div>
          </div>
        </div>
      </div>

      <!-- Input Area -->
      <div class="border-t border-gray-200 p-4">
        <!-- File Upload Preview -->
        <div v-if="selectedFile" class="mb-3 p-2 bg-gray-50 rounded-lg flex items-center justify-between">
          <div class="flex items-center">
            <svg class="w-4 h-4 text-gray-500 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
            </svg>
            <span class="text-sm text-gray-700">{{ selectedFile.name }}</span>
          </div>
          <button @click="removeFile" class="text-red-500 hover:text-red-700">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div class="flex space-x-2">
          <input
            v-model="newMessage"
            @keyup.enter="sendMessage"
            type="text"
            placeholder="Type your message..."
            class="flex-1 px-3 py-2 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-orange-500"
            :disabled="isTyping"
            data-testid="chatbot-input"
          />
          
          <!-- Voice Input Button -->
          <button
            @click="toggleVoiceInput"
            :class="isListening ? 'bg-red-500 hover:bg-red-600' : 'bg-gray-500 hover:bg-gray-600'"
            class="px-3 py-2 text-white rounded-md transition-colors"
            :disabled="isTyping"
            title="Voice input"
          >
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11a7 7 0 01-7 7m0 0a7 7 0 01-7-7m7 7v4m0 0H8m4 0h4m-4-8a3 3 0 01-3-3V5a3 3 0 116 0v6a3 3 0 01-3 3z" />
            </svg>
          </button>

          <!-- File Upload Button -->
          <label class="px-3 py-2 bg-gray-500 hover:bg-gray-600 text-white rounded-md transition-colors cursor-pointer">
            <input
              type="file"
              @change="handleFileSelect"
              class="hidden"
              accept=".txt,.pdf,.doc,.docx,.xls,.xlsx"
            />
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.172 7l-6.586 6.586a2 2 0 102.828 2.828l6.414-6.586a4 4 0 00-5.656-5.656l-6.415 6.585a6 6 0 108.486 8.486L20.5 13" />
            </svg>
          </label>

          <button
            @click="sendMessage"
            :disabled="!newMessage.trim() || isTyping"
            class="px-4 py-2 bg-orange-600 text-white rounded-md hover:bg-orange-700 focus:outline-none focus:ring-2 focus:ring-orange-500 disabled:opacity-50 disabled:cursor-not-allowed"
            data-testid="chatbot-send"
          >
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
            </svg>
          </button>
        </div>
        
        <!-- Quick Actions -->
        <div class="mt-3 flex flex-wrap gap-2">
          <button
            v-for="action in quickActions"
            :key="action.text"
            @click="sendQuickMessage(action.text)"
            class="px-3 py-1 text-xs bg-gray-100 text-gray-700 rounded-full hover:bg-gray-200 transition-colors"
          >
            {{ action.text }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, nextTick, onMounted } from 'vue'

// Type declarations for speech recognition
declare global {
  interface Window {
    SpeechRecognition: any
    webkitSpeechRecognition: any
  }
}

interface Message {
  id: string
  text: string
  sender: 'user' | 'bot'
  timestamp: Date
  actions?: Array<{
    type: string
    label: string
    data?: any
  }>
}

interface ChatbotAction {
  type: string
  label: string
  data?: any
}

// Reactive data
const isOpen = ref(false)
const messages = ref<Message[]>([])
const newMessage = ref('')
const isTyping = ref(false)
const isListening = ref(false)
const selectedFile = ref<File | null>(null)
const messagesContainer = ref<HTMLElement>()

// Quick action buttons
const quickActions = [
  { text: 'Check inventory' },
  { text: 'Sales report' },
  { text: 'Financial summary' },
  { text: 'Help' },
  { text: 'Settings' }
]

// Methods
const toggleChatbot = () => {
  isOpen.value = !isOpen.value
  if (isOpen.value) {
    nextTick(() => {
      scrollToBottom()
    })
  }
}

const clearChat = () => {
  messages.value = []
}

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

  // Simulate bot typing
  isTyping.value = true
  await new Promise(resolve => setTimeout(resolve, 1000))

  // Generate bot response
  const botResponse = await generateBotResponse(userInput)
  
  const botMessage: Message = {
    id: (Date.now() + 1).toString(),
    text: botResponse.reply,
    sender: 'bot',
    timestamp: new Date(),
    actions: botResponse.actions
  }

  messages.value.push(botMessage)
  isTyping.value = false

  nextTick(() => {
    scrollToBottom()
  })
}

const sendQuickMessage = (text: string) => {
  newMessage.value = text
  sendMessage()
}

const executeAction = (action: ChatbotAction) => {
  switch (action.type) {
    case 'navigate':
      // Navigate to specific page
      navigateTo(action.data?.route || '/dashboard')
      break
    case 'export':
      // Export data
      exportData(action.data?.type || 'report')
      break
    case 'refresh':
      // Refresh data
      refreshData()
      break
    default:
      console.log('Action executed:', action)
  }
}

const generateBotResponse = async (userInput: string): Promise<{ reply: string; actions?: ChatbotAction[] }> => {
  // TODO: Replace with actual AI API call
  // const response = await $fetch('/api/chatbot', {
  //   method: 'POST',
  //   body: { message: userInput }
  // })
  // return response

  // Simulate AI response based on keywords
  const input = userInput.toLowerCase()
  
  if (input.includes('inventory') || input.includes('stock')) {
    return {
      reply: "I can help you with inventory management. You can view current stock levels, add new items, or check low stock alerts. Would you like me to show you the inventory dashboard?",
      actions: [
        { type: 'navigate', label: 'Go to Inventory', data: { route: '/stock' } },
        { type: 'export', label: 'Export Stock Report', data: { type: 'inventory' } }
      ]
    }
  } else if (input.includes('sales') || input.includes('report')) {
    return {
      reply: "Here's your sales overview: Today's sales are $53,000 (+55% from yesterday). You have 2,300 active users and 3,462 new clients this month. Would you like a detailed sales report?",
      actions: [
        { type: 'navigate', label: 'Go to Sales', data: { route: '/sales' } },
        { type: 'export', label: 'Export Sales Report', data: { type: 'sales' } }
      ]
    }
  } else if (input.includes('financial') || input.includes('finance')) {
    return {
      reply: "I can help you with financial analytics. Your current revenue is $103,430 with a 23% increase from last month. Would you like to see detailed financial reports?",
      actions: [
        { type: 'navigate', label: 'Go to Finance', data: { route: '/finance' } },
        { type: 'export', label: 'Export Financial Report', data: { type: 'finance' } }
      ]
    }
  } else if (input.includes('help')) {
    return {
      reply: "I'm here to help! I can assist with:\n• Inventory management\n• Sales reports\n• Financial analytics\n• User management\n• System settings\n\nWhat would you like to know?",
      actions: [
        { type: 'navigate', label: 'View Documentation', data: { route: '/docs' } },
        { type: 'refresh', label: 'Refresh Dashboard', data: {} }
      ]
    }
  } else if (input.includes('settings')) {
    return {
      reply: "You can access settings through the sidebar menu. I can help you with:\n• User preferences\n• System configuration\n• Notification settings\n• Security settings\n\nWhich settings would you like to modify?",
      actions: [
        { type: 'navigate', label: 'Go to Settings', data: { route: '/settings' } }
      ]
    }
  } else {
    return {
      reply: "I'm your TOSS ERP assistant! I can help you with inventory management, sales reports, financial analytics, and more. How can I assist you today?",
      actions: [
        { type: 'navigate', label: 'View Dashboard', data: { route: '/dashboard' } },
        { type: 'refresh', label: 'Refresh Data', data: {} }
      ]
    }
  }
}

const toggleVoiceInput = () => {
  if (!isListening.value) {
    // Start voice recognition
    if ('webkitSpeechRecognition' in window || 'SpeechRecognition' in window) {
      const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition
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
  } else {
    // Stop voice recognition
    isListening.value = false
  }
}

const handleFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement
  if (target.files && target.files[0]) {
    selectedFile.value = target.files[0]
  }
}

const removeFile = () => {
  selectedFile.value = null
}

const formatTime = (date: Date): string => {
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

const scrollToBottom = () => {
  if (messagesContainer.value) {
    messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
  }
}

// Utility functions for actions
const navigateTo = (route: string) => {
  // Navigate to the specified route
  window.location.href = route
}

const exportData = (type: string) => {
  // Implement export functionality
  console.log(`Exporting ${type} data...`)
}

const refreshData = () => {
  // Implement refresh functionality
  console.log('Refreshing data...')
}

// Auto-close chatbot when clicking outside
onMounted(() => {
  document.addEventListener('click', (event) => {
    const target = event.target as HTMLElement
    if (!target.closest('.fixed.bottom-4.right-4')) {
      isOpen.value = false
    }
  })
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

/* Voice input animation */
@keyframes pulse {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.5;
  }
}

.bg-red-500 {
  animation: pulse 1s infinite;
}
</style>
