<template>
  <div class="flex h-[calc(100vh-4rem)]">
    <!-- Sidebar with Quick Actions -->
    <div class="w-80 bg-white border-r border-gray-200 flex flex-col">
      <div class="p-4 border-b border-gray-200">
        <h2 class="text-lg font-semibold text-gray-900">TOSS AI Copilot</h2>
        <p class="text-sm text-gray-600 mt-1">Your intelligent business assistant</p>
      </div>

      <!-- Language Selector -->
      <div class="p-4 border-b border-gray-200">
        <label class="text-sm font-medium text-gray-700">Language</label>
        <select v-model="selectedLanguage" 
                class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500">
          <option value="en">English</option>
          <option value="zu">isiZulu</option>
          <option value="af">Afrikaans</option>
          <option value="xh">isiXhosa</option>
        </select>
      </div>

      <!-- Quick Questions -->
      <div class="flex-1 overflow-y-auto p-4">
        <h3 class="text-sm font-semibold text-gray-700 mb-3">Quick Questions</h3>
        <div class="space-y-2">
          <button v-for="(question, index) in quickQuestions" :key="index"
                  @click="askQuestion(question)"
                  class="w-full text-left p-3 rounded-lg bg-gray-50 hover:bg-gray-100 text-sm text-gray-700 transition-colors">
            {{ question }}
          </button>
        </div>

        <!-- Recommendations -->
        <div v-if="recommendations.length > 0" class="mt-6">
          <h3 class="text-sm font-semibold text-gray-700 mb-3">Recommendations</h3>
          <div class="space-y-2">
            <div v-for="rec in recommendations" :key="rec.title"
                 class="p-3 rounded-lg border border-blue-200 bg-blue-50">
              <div class="flex items-start justify-between">
                <div class="flex-1">
                  <div class="text-sm font-medium text-gray-900">{{ rec.title }}</div>
                  <div class="text-xs text-gray-600 mt-1">{{ rec.description }}</div>
                  <div class="mt-2">
                    <span class="text-xs px-2 py-1 rounded" 
                          :class="getPriorityClass(rec.priority)">
                      {{ rec.priority }}
                    </span>
                  </div>
                </div>
                <span class="text-xs text-blue-600 cursor-pointer hover:text-blue-800">→</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Chat Interface -->
    <div class="flex-1 flex flex-col bg-gray-50">
      <!-- Chat Messages -->
      <div class="flex-1 overflow-y-auto p-6 space-y-4">
        <div v-if="messages.length === 0" class="flex items-center justify-center h-full">
          <div class="text-center">
            <svg class="w-16 h-16 mx-auto text-gray-400 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 10h.01M12 10h.01M16 10h.01M9 16H5a2 2 0 01-2-2V6a2 2 0 012-2h14a2 2 0 012 2v8a2 2 0 01-2 2h-5l-5 5v-5z"/>
            </svg>
            <h3 class="text-lg font-medium text-gray-900 mb-2">Ask me anything about your business</h3>
            <p class="text-gray-600">I can help with sales, inventory, finances, and more.</p>
          </div>
        </div>

        <div v-for="(message, index) in messages" :key="index" 
             class="flex" :class="message.role === 'user' ? 'justify-end' : 'justify-start'">
          <div class="max-w-3xl" :class="message.role === 'user' ? 'bg-blue-600 text-white' : 'bg-white border border-gray-200'"
               class="rounded-lg px-4 py-3 shadow-sm">
            <div v-if="message.role === 'assistant'" class="flex items-start gap-3">
              <div class="p-2 bg-blue-100 rounded-full flex-shrink-0">
                <svg class="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z"/>
                </svg>
              </div>
              <div class="flex-1">
                <div class="text-sm text-gray-900">{{ message.content }}</div>
                <div v-if="message.confidence" class="text-xs text-gray-500 mt-2">
                  Confidence: {{ (message.confidence * 100).toFixed(0) }}%
                </div>
                <div v-if="message.suggestedActions && message.suggestedActions.length > 0" 
                     class="mt-3 space-y-1">
                  <div class="text-xs font-medium text-gray-700">Suggested Actions:</div>
                  <button v-for="(action, idx) in message.suggestedActions" :key="idx"
                          class="text-xs px-3 py-1 bg-blue-100 text-blue-700 rounded hover:bg-blue-200 mr-2">
                    {{ action }}
                  </button>
                </div>
              </div>
            </div>
            <div v-else class="text-sm">
              {{ message.content }}
            </div>
          </div>
        </div>

        <div v-if="isLoading" class="flex justify-start">
          <div class="bg-white border border-gray-200 rounded-lg px-4 py-3 shadow-sm">
            <div class="flex items-center gap-2">
              <div class="animate-pulse flex space-x-1">
                <div class="w-2 h-2 bg-blue-600 rounded-full"></div>
                <div class="w-2 h-2 bg-blue-600 rounded-full" style="animation-delay: 0.2s"></div>
                <div class="w-2 h-2 bg-blue-600 rounded-full" style="animation-delay: 0.4s"></div>
              </div>
              <span class="text-sm text-gray-600">Thinking...</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Input Area -->
      <div class="bg-white border-t border-gray-200 p-4">
        <form @submit.prevent="sendMessage" class="flex gap-3">
          <input 
            v-model="currentMessage"
            type="text"
            placeholder="Ask anything... (e.g., 'Show me today's sales' or 'Which products are low in stock?')"
            class="flex-1 rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500"
            :disabled="isLoading"
          />
          <button 
            type="submit"
            :disabled="!currentMessage.trim() || isLoading"
            class="px-6 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:bg-gray-300 disabled:cursor-not-allowed transition-colors">
            <span v-if="!isLoading" class="flex items-center gap-2">
              <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8"/>
              </svg>
              Send
            </span>
            <span v-else>Processing...</span>
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'dashboard',
  middleware: ['auth']
})

const currentMessage = ref('')
const isLoading = ref(false)
const selectedLanguage = ref('en')
const messages = ref<Array<{role: string, content: string, confidence?: number, suggestedActions?: string[]}>>([])
const recommendations = ref<any[]>([])

const quickQuestions = [
  "What were my sales today?",
  "Which products are low in stock?",
  "Show me my top 5 customers",
  "What's my cash flow this month?",
  "Who has pending invoices?",
  "Show inventory by warehouse",
  "What's my profit margin?",
  "Which suppliers have best performance?"
]

const { $api } = useNuxtApp()

async function sendMessage() {
  if (!currentMessage.value.trim()) return

  const userMessage = currentMessage.value
  messages.value.push({ role: 'user', content: userMessage })
  currentMessage.value = ''
  isLoading.value = true

  try {
    // Call AI Copilot API
    const response = await $api.post('/copilot/query', {
      query: userMessage,
      language: selectedLanguage.value
    })

    messages.value.push({
      role: 'assistant',
      content: response.answer,
      confidence: response.confidence,
      suggestedActions: response.suggestedActions
    })
  } catch (error) {
    console.error('Copilot error:', error)
    messages.value.push({
      role: 'assistant',
      content: "I'm having trouble processing your request. Please try again.",
      confidence: 0
    })
  } finally {
    isLoading.value = false
  }
}

function askQuestion(question: string) {
  currentMessage.value = question
  sendMessage()
}

function getPriorityClass(priority: string) {
  if (priority === 'High') return 'bg-red-100 text-red-800'
  if (priority === 'Medium') return 'bg-yellow-100 text-yellow-800'
  return 'bg-green-100 text-green-800'
}

// Load recommendations on mount
onMounted(async () => {
  try {
    const recs = await $api.get('/copilot/recommendations/InventoryOptimization')
    recommendations.value = recs
  } catch (error) {
    console.error('Failed to load recommendations:', error)
  }
})
</script>

