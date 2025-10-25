<template>
  <div v-if="isVisible" class="fixed bottom-4 right-4 z-50">
    <!-- Chat Widget -->
    <div v-if="isOpen" class="bg-white dark:bg-gray-800 rounded-lg shadow-2xl border border-gray-200 dark:border-gray-700 w-80 h-96 flex flex-col">
      <!-- Header -->
      <div class="flex items-center justify-between p-4 border-b border-gray-200 dark:border-gray-700">
        <div class="flex items-center space-x-2">
          <div class="w-8 h-8 bg-gradient-to-r from-green-400 to-green-600 rounded-full flex items-center justify-center">
            <svg class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z"></path>
            </svg>
          </div>
          <div>
            <h3 class="text-sm font-semibold text-gray-900 dark:text-white">AI Co-Pilot</h3>
            <p class="text-xs text-green-600 dark:text-green-400">Online</p>
          </div>
        </div>
        <button @click="toggleChat" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
          </svg>
        </button>
      </div>

      <!-- Messages -->
      <div class="flex-1 overflow-y-auto p-4 space-y-4" ref="messagesContainer">
        <div v-for="message in messages" :key="message.id" :class="messageClass(message)">
          <div v-if="message.type === 'bot'" class="flex items-start space-x-2">
            <div class="w-6 h-6 bg-green-500 rounded-full flex items-center justify-center flex-shrink-0">
              <svg class="w-3 h-3 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z"></path>
              </svg>
            </div>
            <div class="bg-gray-100 dark:bg-gray-700 rounded-lg p-3 max-w-xs">
              <p class="text-sm text-gray-900 dark:text-white" v-html="message.content"></p>
              <p class="text-xs text-gray-500 dark:text-gray-400 mt-1">{{ formatTime(message.timestamp) }}</p>
            </div>
          </div>
          
          <div v-else class="flex items-start space-x-2 justify-end">
            <div class="bg-blue-600 rounded-lg p-3 max-w-xs">
              <p class="text-sm text-white">{{ message.content }}</p>
              <p class="text-xs text-blue-200 mt-1">{{ formatTime(message.timestamp) }}</p>
            </div>
            <div class="w-6 h-6 bg-blue-600 rounded-full flex items-center justify-center flex-shrink-0">
              <span class="text-white text-xs font-medium">You</span>
            </div>
          </div>
        </div>
        
        <!-- Typing Indicator -->
        <div v-if="isTyping" class="flex items-start space-x-2">
          <div class="w-6 h-6 bg-green-500 rounded-full flex items-center justify-center flex-shrink-0">
            <svg class="w-3 h-3 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z"></path>
            </svg>
          </div>
          <div class="bg-gray-100 dark:bg-gray-700 rounded-lg p-3">
            <div class="flex space-x-1">
              <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce"></div>
              <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="animation-delay: 0.1s"></div>
              <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="animation-delay: 0.2s"></div>
            </div>
          </div>
        </div>
      </div>

      <!-- Input -->
      <div class="p-4 border-t border-gray-200 dark:border-gray-700">
        <div class="flex space-x-2">
          <input
            v-model="newMessage"
            @keyup.enter="sendMessage"
            :disabled="isTyping"
            type="text"
            placeholder="Ask me anything about your business..."
            class="flex-1 px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400 focus:ring-2 focus:ring-blue-500 focus:border-transparent text-sm"
          />
          <!-- Voice Input Button -->
          <button
            @click="handleVoiceInput"
            :disabled="isTyping"
            :class="[
              'p-2 rounded-lg transition-colors',
              isVoiceListening ? 'bg-red-500 hover:bg-red-600 text-white' : 'bg-gray-100 dark:bg-gray-700 hover:bg-gray-200 dark:hover:bg-gray-600 text-gray-700 dark:text-gray-300'
            ]"
            :title="isVoiceListening ? 'Stop listening' : 'Voice input'"
          >
            <svg v-if="!isVoiceListening" class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11a7 7 0 01-7 7m0 0a7 7 0 01-7-7m7 7v4m0 0H8m4 0h4m-4-8a3 3 0 01-3-3V5a3 3 0 116 0v6a3 3 0 01-3 3z" />
            </svg>
            <div v-else class="w-4 h-4 bg-red-600 rounded-full animate-pulse"></div>
          </button>
          <button
            @click="sendMessage"
            :disabled="!newMessage.trim() || isTyping"
            class="bg-blue-600 text-white p-2 rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
          >
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8"></path>
            </svg>
          </button>
        </div>

        <!-- Quick Actions -->
        <div class="mt-2 flex flex-wrap gap-1">
          <button
            v-for="action in quickActions"
            :key="action.text"
            @click="sendQuickAction(action.text)"
            class="px-2 py-1 text-xs bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-200 dark:hover:bg-gray-600 transition-colors"
          >
            {{ action.text }}
          </button>
        </div>
      </div>
    </div>

    <!-- Floating Button -->
    <button
      v-else
      @click="toggleChat"
      class="bg-gradient-to-r from-green-400 to-green-600 text-white p-4 rounded-full shadow-lg hover:shadow-xl transition-all duration-200 hover:scale-105"
    >
      <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z"></path>
      </svg>
    </button>
  </div>
</template>

<script setup lang="ts">
import { useVoiceCommands } from '~/composables/useVoiceCommands'

interface Message {
  id: string
  type: 'user' | 'bot'
  content: string
  timestamp: Date
}

interface QuickAction {
  text: string
  action?: () => void
}

const isVisible = ref(true)
const isOpen = ref(false)
const isTyping = ref(false)
const newMessage = ref('')
const messagesContainer = ref<HTMLElement>()

// Voice commands integration
const {
  isListening: isVoiceListening,
  isSupported: isVoiceSupported,
  transcript: voiceTranscript,
  toggleListening,
  speak,
  clearTranscript
} = useVoiceCommands({
  continuous: false,
  interimResults: true
})

const messages = ref<Message[]>([
  {
    id: '1',
    type: 'bot',
    content: 'Hello! I\'m your AI business co-pilot. I can help you with inventory management, sales analysis, group purchasing, and much more. What would you like to know?',
    timestamp: new Date()
  }
])

const quickActions: QuickAction[] = [
  { text: 'Show inventory status' },
  { text: 'Today\'s sales summary' },
  { text: 'Find group orders' },
  { text: 'Check cash flow' },
  { text: 'Order supplies' }
]

function toggleChat() {
  isOpen.value = !isOpen.value
}

async function sendMessage() {
  if (!newMessage.value.trim()) return

  // Add user message
  const userMessage: Message = {
    id: Date.now().toString(),
    type: 'user',
    content: newMessage.value,
    timestamp: new Date()
  }
  messages.value.push(userMessage)

  const userInput = newMessage.value
  newMessage.value = ''
  
  // Scroll to bottom
  nextTick(() => {
    scrollToBottom()
  })

  // Show typing indicator
  isTyping.value = true

  try {
    // Simulate API call to AI service
    await new Promise(resolve => setTimeout(resolve, 1000 + Math.random() * 2000))
    
    // Generate AI response (this would be a real API call)
    const aiResponse = generateAIResponse(userInput)
    
    const botMessage: Message = {
      id: (Date.now() + 1).toString(),
      type: 'bot',
      content: aiResponse,
      timestamp: new Date()
    }
    messages.value.push(botMessage)
    
  } catch (error) {
    const errorMessage: Message = {
      id: (Date.now() + 1).toString(),
      type: 'bot',
      content: 'Sorry, I encountered an error. Please try again.',
      timestamp: new Date()
    }
    messages.value.push(errorMessage)
  } finally {
    isTyping.value = false
    nextTick(() => {
      scrollToBottom()
    })
  }
}

function sendQuickAction(text: string) {
  newMessage.value = text
  sendMessage()
}

// Voice input handler
function handleVoiceInput() {
  toggleListening()
}

// Watch for voice transcript changes
watch(voiceTranscript, async (newTranscript) => {
  if (newTranscript && !isVoiceListening.value) {
    // Voice input completed, send as message
    newMessage.value = newTranscript
    await sendMessage()
    clearTranscript()
  }
})

// Watch for AI responses to speak them if voice was used
let lastMessageCount = messages.value.length
watch(() => messages.value.length, async (newLength) => {
  if (newLength > lastMessageCount && isVoiceSupported.value) {
    const lastMessage = messages.value[messages.value.length - 1]
    if (lastMessage.type === 'bot' && isVoiceListening.value) {
      // Speak the AI response
      try {
        await speak(lastMessage.content)
      } catch (err) {
        console.error('Failed to speak response:', err)
      }
    }
  }
  lastMessageCount = newLength
})

function generateAIResponse(input: string): string {
  const lowerInput = input.toLowerCase()
  
  if (lowerInput.includes('inventory') || lowerInput.includes('stock')) {
    return 'I can see you have 1,247 items in inventory. 3 items are running low: Maize meal (5 bags left), Cooking oil (2 bottles), and Sugar (8kg remaining). Would you like me to create a group purchase order for these items?'
  } else if (lowerInput.includes('sales') || lowerInput.includes('revenue')) {
    return 'Today\'s sales are looking good! You\'ve made R8,450 so far, which is 15% higher than yesterday. Your top-selling items are: 1) Cold drinks (45 sold), 2) Bread (32 loaves), 3) Airtime vouchers (28 sold). Need help with anything specific?'
  } else if (lowerInput.includes('group') || lowerInput.includes('bulk')) {
    return 'There are currently 3 active group purchases you can join: 1) Bulk flour order (save 20%) - 6 businesses participating, 2) Cleaning supplies (save 15%) - 4 businesses, 3) Cold drinks (save 12%) - 8 businesses. Which interests you?'
  } else if (lowerInput.includes('cash') || lowerInput.includes('money') || lowerInput.includes('financial')) {
    return 'Your cash flow looks healthy! Current balance: R12,350. This week\'s income: R45,200, expenses: R32,100. You have R3,200 in accounts receivable due this week. Need help with payment reminders?'
  } else if (lowerInput.includes('order') || lowerInput.includes('supply')) {
    return 'I can help you order supplies! Based on your sales patterns, I suggest ordering: Maize meal (20 bags), Cooking oil (12 bottles), Sugar (25kg). I found a group order for these items that could save you 18%. Shall I add you to it?'
  } else {
    return `I understand you're asking about "${input}". I can help with inventory management, sales analysis, group purchasing, financial tracking, supplier coordination, and much more. Could you be more specific about what you need help with?`
  }
}

function messageClass(message: Message) {
  return message.type === 'user' ? 'flex justify-end' : 'flex justify-start'
}

function formatTime(timestamp: Date) {
  return timestamp.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

function scrollToBottom() {
  if (messagesContainer.value) {
    messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
  }
}

// Auto-scroll on new messages
watch(messages, () => {
  nextTick(() => {
    scrollToBottom()
  })
}, { deep: true })
</script>
