<script setup lang="ts">
import { ref, computed, onMounted, nextTick, watch } from 'vue'
import { useChatbot } from '~/composables/useChatbot'

const quickActions = [
  { text: 'Show low stock items' },
  { text: 'What are today\'s sales?' },
  { text: 'List all customers' },
  { text: 'Show active employees' },
  { text: 'Check cashflow' },
  { text: 'View pending orders' }
]

const { messages, sendMessage, isLoading, clearChat } = useChatbot()

const inputMessage = ref('')
const chatContainer = ref<HTMLElement | null>(null)

const scrollToBottom = async () => {
  await nextTick()
  if (chatContainer.value) {
    chatContainer.value.scrollTop = chatContainer.value.scrollHeight
  }
}

const handleSend = async () => {
  if (!inputMessage.value.trim() || isLoading.value) return
  
  const message = inputMessage.value.trim()
  inputMessage.value = ''
  
  await sendMessage(message)
  await scrollToBottom()
}

const handleKeyPress = (e: KeyboardEvent) => {
  if (e.key === 'Enter' && !e.shiftKey) {
    e.preventDefault()
    handleSend()
  }
}

onMounted(() => {
  scrollToBottom()
})

watch(() => messages.value.length, () => {
  scrollToBottom()
})
</script>

<template>
  <div class="flex flex-col h-full bg-white rounded-xl shadow-sm hover:shadow-md transition-shadow">
    <!-- Chat Header -->
    <div class="flex items-center justify-between px-6 py-4 border-b border-gray-200">
      <div class="flex items-center gap-3">
        <div class="w-12 h-12 rounded-lg bg-gradient-to-br from-gray-800 to-gray-900 shadow-lg flex items-center justify-center">
          <i class="text-white material-symbols-rounded opacity-90">psychology</i>
        </div>
        <div>
          <h3 class="text-base font-semibold text-gray-900 mb-0">TOSS AI Assistant</h3>
          <p class="text-sm text-gray-600 mb-0">Ask me anything about your business</p>
        </div>
      </div>
      <button
        @click="clearChat"
        class="p-2 text-gray-400 hover:text-gray-600 hover:bg-gray-100 rounded-lg transition-all"
        title="Clear chat"
      >
        <i class="material-symbols-rounded text-lg">delete</i>
      </button>
    </div>

    <!-- Messages -->
    <div
      ref="chatContainer"
      class="flex-1 overflow-y-auto px-6 py-4 space-y-4"
    >
      <div
        v-for="(message, index) in messages"
        :key="index"
        class="flex"
        :class="message.role === 'user' ? 'justify-end' : 'justify-start'"
      >
        <div
          class="max-w-[80%] rounded-lg px-4 py-3 shadow-sm"
          :class="
            message.role === 'user'
              ? 'bg-gradient-to-br from-blue-500 to-blue-600 text-white'
              : 'bg-gray-100 text-gray-900'
          "
        >
          <div v-if="message.role === 'assistant'" class="flex items-center gap-2 mb-2">
            <div class="w-6 h-6 rounded-lg bg-gradient-to-br from-gray-800 to-gray-900 shadow-md flex items-center justify-center">
              <i class="text-white text-xs material-symbols-rounded opacity-90">psychology</i>
            </div>
            <span class="text-xs font-semibold text-gray-700">TOSS AI</span>
          </div>
          
          <div class="text-sm whitespace-pre-wrap leading-relaxed">{{ message.content }}</div>
          
          <div v-if="message.action" class="mt-3 pt-3 border-t" :class="message.role === 'user' ? 'border-blue-400/30' : 'border-gray-300'">
            <button
              v-if="message.action.type === 'navigate'"
              @click="$router.push(message.action.path)"
              class="text-xs px-4 py-2 rounded-lg font-medium transition-all shadow-sm hover:shadow-md"
              :class="message.role === 'user' ? 'bg-white/20 text-white hover:bg-white/30' : 'bg-gradient-to-br from-gray-800 to-gray-900 text-white hover:from-gray-900 hover:to-gray-950'"
            >
              {{ message.action.label }}
            </button>
          </div>
          
          <div class="text-xs mt-2" :class="message.role === 'user' ? 'text-white/70' : 'text-gray-500'">
            {{ new Date(message.timestamp).toLocaleTimeString('en-ZA', { hour: '2-digit', minute: '2-digit' }) }}
          </div>
        </div>
      </div>

      <!-- Loading Indicator -->
      <div v-if="isLoading" class="flex justify-start">
        <div class="max-w-[80%] rounded-lg px-4 py-3 bg-gray-100 shadow-sm">
          <div class="flex items-center gap-2">
            <div class="w-6 h-6 rounded-lg bg-gradient-to-br from-gray-800 to-gray-900 shadow-md flex items-center justify-center">
              <i class="text-white text-xs material-symbols-rounded opacity-90 animate-pulse">psychology</i>
            </div>
            <div class="flex gap-1">
              <span class="w-2 h-2 bg-gray-500 rounded-full animate-bounce" style="animation-delay: 0s"></span>
              <span class="w-2 h-2 bg-gray-500 rounded-full animate-bounce" style="animation-delay: 0.2s"></span>
              <span class="w-2 h-2 bg-gray-500 rounded-full animate-bounce" style="animation-delay: 0.4s"></span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="px-6 py-3 border-t border-gray-200">
      <div class="flex gap-2 overflow-x-auto pb-2">
        <button
          v-for="quickAction in quickActions"
          :key="quickAction.text"
          @click="inputMessage = quickAction.text; handleSend()"
          class="px-4 py-2 text-xs font-medium bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 hover:shadow-sm transition-all whitespace-nowrap"
        >
          {{ quickAction.text }}
        </button>
      </div>
    </div>

    <!-- Input -->
    <div class="px-6 py-4 border-t border-gray-200">
      <div class="flex gap-2">
        <input
          v-model="inputMessage"
          @keypress="handleKeyPress"
          type="text"
          placeholder="Ask me anything... (e.g., 'Show me low stock items', 'What are today's sales?')"
          class="flex-1 px-4 py-2.5 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-gray-800 focus:border-transparent text-sm transition-all"
          :disabled="isLoading"
        />
        <button
          @click="handleSend"
          :disabled="!inputMessage.trim() || isLoading"
          class="px-6 py-2.5 bg-gradient-to-br from-gray-800 to-gray-900 text-white rounded-lg hover:from-gray-900 hover:to-gray-950 disabled:opacity-50 disabled:cursor-not-allowed transition-all shadow-md hover:shadow-lg flex items-center gap-2 font-medium"
        >
          <i class="material-symbols-rounded text-lg">send</i>
          <span class="hidden sm:inline text-sm">Send</span>
        </button>
      </div>
    </div>
  </div>
</template>



