<script setup lang="ts">
import { ref, computed, onMounted, nextTick, watch, onUnmounted } from 'vue'
import { useChatbot } from '~/composables/useChatbot'

const props = defineProps<{
  isOpen: boolean
}>()

const emit = defineEmits<{
  'update:isOpen': [value: boolean]
  'close': []
}>()

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

const handleClose = () => {
  emit('update:isOpen', false)
  emit('close')
}

const handleClickOutside = (e: MouseEvent) => {
  const target = e.target as HTMLElement
  if (props.isOpen && !target.closest('.chatbot-popup') && !target.closest('.chatbot-button')) {
    handleClose()
  }
}

onMounted(() => {
  scrollToBottom()
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})

watch(() => messages.value.length, () => {
  scrollToBottom()
})

watch(() => props.isOpen, (newVal) => {
  if (newVal) {
    nextTick(() => {
      scrollToBottom()
    })
  }
})
</script>

<template>
  <!-- Fixed Button -->
  <a
    v-if="!isOpen"
    class="chatbot-button fixed-plugin-button text-dark position-fixed px-3 py-2"
    style="right: 20px; bottom: 20px; z-index: 990;"
    @click.stop="emit('update:isOpen', true)"
  >
    <div class="w-12 h-12 rounded-lg bg-gradient-to-br from-gray-800 to-gray-900 shadow-lg flex items-center justify-center hover:from-gray-900 hover:to-gray-950 transition-all">
      <i class="material-symbols-rounded text-white opacity-90">psychology</i>
    </div>
  </a>

  <!-- Popup Card -->
  <div
    v-if="isOpen"
    class="chatbot-popup fixed-plugin position-fixed"
    style="right: 20px; bottom: 20px; z-index: 1030; width: 420px; max-width: calc(100vw - 40px);"
  >
    <div class="card shadow-lg" style="max-height: calc(100vh - 40px);">
      <!-- Card Header -->
      <div class="card-header pb-0 pt-3">
        <div class="float-start">
          <div class="flex items-center gap-2">
            <div class="w-10 h-10 rounded-lg bg-gradient-to-br from-gray-800 to-gray-900 shadow-md flex items-center justify-center">
              <i class="text-white material-symbols-rounded opacity-90 text-base">psychology</i>
            </div>
            <div>
              <h5 class="mt-3 mb-0 text-base font-semibold text-gray-900">TOSS AI Assistant</h5>
              <p class="text-sm text-gray-600 mb-0">Ask me anything about your business</p>
            </div>
          </div>
        </div>
        <div class="float-end mt-4">
          <button
            class="btn btn-link text-dark p-0"
            @click="handleClose"
          >
            <i class="material-symbols-rounded">clear</i>
          </button>
        </div>
      </div>

      <hr class="horizontal dark my-1">

      <!-- Card Body -->
      <div class="card-body pt-sm-3 pt-0" style="display: flex; flex-direction: column; height: calc(100vh - 200px); max-height: 600px;">
        <!-- Messages -->
        <div
          ref="chatContainer"
          class="flex-1 overflow-y-auto px-3 py-3 space-y-3 mb-3"
          style="min-height: 200px;"
        >
          <div
            v-for="(message, index) in messages"
            :key="index"
            class="flex"
            :class="message.role === 'user' ? 'justify-end' : 'justify-start'"
          >
            <div
              class="max-w-[85%] rounded-lg px-3 py-2 shadow-sm"
              :class="
                message.role === 'user'
                  ? 'bg-gradient-to-br from-blue-500 to-blue-600 text-white'
                  : 'bg-gray-100 text-gray-900'
              "
            >
              <div v-if="message.role === 'assistant'" class="flex items-center gap-2 mb-1">
                <div class="w-5 h-5 rounded-lg bg-gradient-to-br from-gray-800 to-gray-900 shadow-sm flex items-center justify-center">
                  <i class="text-white text-xs material-symbols-rounded opacity-90">psychology</i>
                </div>
                <span class="text-xs font-semibold text-gray-700">TOSS AI</span>
              </div>
              
              <div class="text-sm whitespace-pre-wrap leading-relaxed">{{ message.content }}</div>
              
              <div v-if="message.action" class="mt-2 pt-2 border-t" :class="message.role === 'user' ? 'border-blue-400/30' : 'border-gray-300'">
                <button
                  v-if="message.action.type === 'navigate'"
                  @click="$router.push(message.action.path); handleClose()"
                  class="text-xs px-3 py-1.5 rounded-lg font-medium transition-all shadow-sm hover:shadow-md"
                  :class="message.role === 'user' ? 'bg-white/20 text-white hover:bg-white/30' : 'bg-gradient-to-br from-gray-800 to-gray-900 text-white hover:from-gray-900 hover:to-gray-950'"
                >
                  {{ message.action.label }}
                </button>
              </div>
              
              <div class="text-xs mt-1.5" :class="message.role === 'user' ? 'text-white/70' : 'text-gray-500'">
                {{ new Date(message.timestamp).toLocaleTimeString('en-ZA', { hour: '2-digit', minute: '2-digit' }) }}
              </div>
            </div>
          </div>

          <!-- Loading Indicator -->
          <div v-if="isLoading" class="flex justify-start">
            <div class="max-w-[85%] rounded-lg px-3 py-2 bg-gray-100 shadow-sm">
              <div class="flex items-center gap-2">
                <div class="w-5 h-5 rounded-lg bg-gradient-to-br from-gray-800 to-gray-900 shadow-sm flex items-center justify-center">
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
        <div class="px-3 py-2 border-t border-gray-200">
          <div class="flex gap-2 overflow-x-auto pb-2">
            <button
              v-for="quickAction in quickActions"
              :key="quickAction.text"
              @click="inputMessage = quickAction.text; handleSend()"
              class="px-3 py-1.5 text-xs font-medium bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 hover:shadow-sm transition-all whitespace-nowrap"
            >
              {{ quickAction.text }}
            </button>
          </div>
        </div>

        <!-- Input -->
        <div class="px-3 py-3 border-t border-gray-200">
          <div class="flex gap-2">
            <input
              v-model="inputMessage"
              @keypress="handleKeyPress"
              type="text"
              placeholder="Ask me anything..."
              class="flex-1 px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-gray-800 focus:border-transparent text-sm transition-all"
              :disabled="isLoading"
            />
            <button
              @click="handleSend"
              :disabled="!inputMessage.trim() || isLoading"
              class="px-4 py-2 bg-gradient-to-br from-gray-800 to-gray-900 text-white rounded-lg hover:from-gray-900 hover:to-gray-950 disabled:opacity-50 disabled:cursor-not-allowed transition-all shadow-md hover:shadow-lg flex items-center gap-1 font-medium"
            >
              <i class="material-symbols-rounded text-base">send</i>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.fixed-plugin {
  transition: all 0.3s ease;
}

.fixed-plugin-button {
  cursor: pointer;
  transition: all 0.3s ease;
}

.fixed-plugin-button:hover {
  transform: scale(1.05);
}

.card {
  border-radius: 0.75rem;
}

.horizontal.dark {
  background-color: #e9ecef;
  height: 1px;
  margin: 0;
  border: 0;
  opacity: 0.25;
}

.float-start {
  float: left;
}

.float-end {
  float: right;
}

@media (max-width: 480px) {
  .chatbot-popup {
    width: calc(100vw - 20px) !important;
    right: 10px !important;
    bottom: 10px !important;
  }
}
</style>
