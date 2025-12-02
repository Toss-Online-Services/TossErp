<template>
  <div>
    <!-- Floating AI Assistant Button (Draggable) -->
    <Transition name="bounce">
      <button
        v-if="!isOpen"
        ref="draggableButton"
        @click="handleButtonClick"
        @mousedown="startDrag"
        @touchstart="startDrag"
        :style="{ left: buttonPosition.x + 'px', top: buttonPosition.y + 'px' }"
        class="fixed z-50 w-14 h-14 bg-gradient-to-r from-blue-500 to-purple-600 hover:from-blue-600 hover:to-purple-700 text-white rounded-full shadow-lg hover:shadow-xl transition-shadow duration-300 flex items-center justify-center group cursor-move touch-none"
      >
        <SparklesIcon class="w-6 h-6 group-hover:scale-110 transition-transform pointer-events-none" />
        <div v-if="unreadCount > 0" class="absolute -top-2 -right-2 w-4 h-4 bg-red-500 rounded-full flex items-center justify-center pointer-events-none">
          <span class="text-xs text-white">{{ unreadCount }}</span>
        </div>
      </button>
    </Transition>

    <!-- AI Assistant Panel (Draggable) -->
    <Transition name="slide-up">
      <div
        v-if="isOpen"
        ref="draggablePanel"
        :style="{ left: panelPosition.x + 'px', top: panelPosition.y + 'px' }"
        class="fixed z-50 w-[calc(100vw-16px)] max-w-96 h-[calc(100vh-160px)] max-h-[600px] bg-white dark:bg-slate-800 rounded-xl shadow-2xl border border-slate-200 dark:border-slate-700 flex flex-col overflow-hidden"
      >
        <!-- Header (Draggable Handle) -->
        <div 
          @mousedown="startDragPanel"
          @touchstart="startDragPanel"
          class="p-4 border-b border-slate-200 dark:border-slate-700 bg-gradient-to-r from-blue-500 to-purple-600 cursor-move touch-none"
        >
          <div class="flex items-center justify-between">
            <div class="flex items-center gap-3">
              <div class="w-8 h-8 rounded-full bg-white/20 flex items-center justify-center">
                <SparklesIcon class="w-4 h-4 text-white" />
              </div>
              <div>
                <h3 class="font-semibold text-white">TOSS AI Assistant</h3>
                <p class="text-xs text-white/80">{{ currentModule }}</p>
              </div>
            </div>
            <div class="flex items-center gap-2 flex-shrink-0">
              <button
                @click.stop="minimizeAssistant"
                @mousedown.stop
                @touchstart.stop
                class="w-8 h-8 rounded-full bg-white/20 hover:bg-white/30 flex items-center justify-center transition-colors cursor-pointer shrink-0"
                title="Minimize"
                type="button"
              >
                <MinusIcon class="w-4 h-4 text-white flex-shrink-0" />
              </button>
              <button
                @click.stop="toggleAssistant"
                @mousedown.stop
                @touchstart.stop
                class="w-8 h-8 rounded-full bg-white/20 hover:bg-white/30 flex items-center justify-center transition-colors cursor-pointer shrink-0"
                title="Close"
                type="button"
              >
                <XMarkIcon class="w-4 h-4 text-white flex-shrink-0" />
              </button>
            </div>
          </div>
        </div>

        <!-- Context Indicator -->
        <div class="px-4 py-2 bg-slate-50 dark:bg-slate-700 border-b border-slate-200 dark:border-slate-600">
          <div class="flex items-center gap-2 text-xs">
            <component :is="getModuleIcon()" class="w-4 h-4 text-slate-600 dark:text-slate-400" />
            <span class="text-slate-600 dark:text-slate-400">Currently in: {{ currentModule }}</span>
            <div class="ml-auto flex gap-1">
              <div class="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
              <span class="text-green-600 dark:text-green-400">AI Online</span>
            </div>
          </div>
        </div>

        <!-- Chat Messages -->
        <div class="flex-1 p-4 overflow-y-auto space-y-3" ref="chatContainer">
          <!-- Messages -->
          <div v-for="message in messages" :key="message.id" class="space-y-3">
            <!-- Assistant Message -->
            <div v-if="message.type === 'assistant'" class="flex items-start gap-3">
              <div class="w-6 h-6 rounded-full bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center flex-shrink-0">
                <SparklesIcon class="w-3 h-3 text-white" />
              </div>
              <div class="flex-1">
                <div class="bg-slate-50 dark:bg-slate-700 p-3 rounded-lg rounded-tl-none text-sm">
                  <p class="text-slate-900 dark:text-white">{{ message.content }}</p>
                  
                  <!-- Data metrics if available -->
                  <div v-if="message.data?.metrics" class="mt-3 grid grid-cols-2 gap-2">
                    <div v-for="metric in message.data.metrics" :key="metric.label" class="bg-white dark:bg-slate-600 p-2 rounded text-center">
                      <div class="text-xs text-slate-600 dark:text-slate-400">{{ metric.label }}</div>
                      <div class="font-semibold text-slate-900 dark:text-white">{{ metric.value }}</div>
                    </div>
                  </div>
                  
                  <!-- Action buttons if available -->
                  <div v-if="message.data?.actions" class="mt-3 space-y-1">
                    <button
                      v-for="action in message.data.actions"
                      :key="action.title"
                      @click="handleAction(action)"
                      class="w-full text-left p-2 bg-blue-50 dark:bg-blue-900/20 hover:bg-blue-100 dark:hover:bg-blue-900/40 rounded text-xs transition-colors flex items-center gap-2"
                    >
                      <ArrowRightIcon class="w-3 h-3 text-blue-600 dark:text-blue-400" />
                      <span class="text-blue-900 dark:text-blue-100">{{ action.title }}</span>
                    </button>
                  </div>
                </div>
                <div class="text-xs text-slate-500 dark:text-slate-400 mt-1">
                  {{ formatTime(message.timestamp) }}
                </div>
              </div>
            </div>

            <!-- User Message -->
            <div v-else class="flex items-start gap-3 justify-end">
              <div class="flex-1 text-right">
                <div class="inline-block bg-blue-600 text-white p-3 rounded-lg rounded-tr-none text-sm max-w-[80%]">
                  {{ message.content }}
                </div>
                <div class="text-xs text-slate-500 dark:text-slate-400 mt-1">
                  {{ formatTime(message.timestamp) }}
                </div>
              </div>
            </div>
          </div>

          <!-- Typing indicator -->
          <div v-if="isTyping" class="flex items-start gap-3">
            <div class="w-6 h-6 rounded-full bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center flex-shrink-0">
              <SparklesIcon class="w-3 h-3 text-white" />
            </div>
            <div class="bg-slate-50 dark:bg-slate-700 p-3 rounded-lg rounded-tl-none">
              <div class="flex gap-1">
                <div class="w-2 h-2 bg-slate-400 rounded-full animate-bounce" style="animation-delay: 0ms"></div>
                <div class="w-2 h-2 bg-slate-400 rounded-full animate-bounce" style="animation-delay: 150ms"></div>
                <div class="w-2 h-2 bg-slate-400 rounded-full animate-bounce" style="animation-delay: 300ms"></div>
              </div>
            </div>
          </div>
        </div>

        <!-- Input Area -->
        <div class="p-4 border-t border-slate-200 dark:border-slate-700">
          <form @submit.prevent="sendMessage" class="flex gap-2">
            <input
              v-model="newMessage"
              type="text"
              placeholder="Ask me anything about your business..."
              class="flex-1 px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-slate-700 dark:text-white text-sm"
              :disabled="isTyping"
            >
            <button
              type="submit"
              :disabled="!newMessage.trim() || isTyping"
              class="px-3 py-2 bg-blue-600 hover:bg-blue-700 disabled:bg-slate-300 dark:disabled:bg-slate-600 text-white rounded-lg transition-colors flex items-center justify-center"
            >
              <PaperAirplaneIcon class="w-4 h-4" />
            </button>
          </form>
        </div>
      </div>
    </Transition>

    <!-- Minimized indicator -->
    <Transition name="slide-right">
      <div
        v-if="isMinimized"
        @click="expandAssistant"
        class="fixed bottom-6 right-6 z-50 bg-gradient-to-r from-blue-500 to-purple-600 text-white px-4 py-2 rounded-full shadow-lg cursor-pointer hover:shadow-xl transition-all duration-300 flex items-center gap-2"
      >
        <ChatBubbleLeftRightIcon class="w-4 h-4" />
        <span class="text-sm">AI Assistant</span>
        <div v-if="unreadCount > 0" class="w-2 h-2 bg-red-400 rounded-full"></div>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch, nextTick } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAI } from '~/composables/useAI'
import { 
  SparklesIcon, 
  XMarkIcon, 
  PaperAirplaneIcon,
  MinusIcon,
  ChatBubbleLeftRightIcon,
  ChartBarIcon,
  CurrencyDollarIcon,
  UsersIcon,
  CubeIcon,
  UserGroupIcon,
  CalculatorIcon,
  ArrowRightIcon
} from '@heroicons/vue/24/outline'

const router = useRouter()
const route = useRoute()
const chatContainer = ref<HTMLElement>()
const draggableButton = ref<HTMLElement>()
const draggablePanel = ref<HTMLElement>()

// AI Service
const { askAI, getSuggestions, isLoading: aiLoading, error: aiError } = useAI()

// State
const isOpen = ref(false)
const isMinimized = ref(false)
const isTyping = ref(false)
const newMessage = ref('')
const unreadCount = ref(3)
const shopId = ref(1) // TODO: Get from auth/store context

// Drag state
const isDragging = ref(false)
const wasDragged = ref(false)
const dragStartTime = ref(0)
const buttonPosition = ref({ x: 0, y: 0 })
const panelPosition = ref({ x: 0, y: 0 })
const dragOffset = ref({ x: 0, y: 0 })

// Chat messages
const messages = ref<Array<{
  id: string
  type: 'user' | 'assistant'
  content: string
  timestamp: Date
  data?: any
}>>([])

// Business data for AI responses
const businessData = {
  revenue: { daily: 2450, weekly: 19600, monthly: 89450, growth: 12.5 },
  customers: { total: 234, active: 23, repeatRate: 68 },
  inventory: { totalItems: 156, lowStockAlerts: 3, criticalItems: ['Milk', 'White Bread', 'Coca Cola'] },
  employees: { total: 8, monthlyPayroll: 45600, attendance: 96 },
  financial: { cashOnHand: 43500, profitMargin: 28, accountsReceivable: 12300 }
}

// Current module context
const currentModule = computed(() => {
  const pathSegments = route.path.split('/').filter(Boolean)
  
  if (pathSegments.length > 0) {
    const firstSegment = pathSegments[0]
    
    switch (firstSegment) {
      case 'sales': return 'Sales & Selling'
      case 'crm': return 'Customer Relationship Management'
      case 'inventory': return 'Inventory Management'
      case 'hr': return 'Human Resources'
      case 'accounts': case 'accounting': return 'Accounting & Finance'
      case 'purchasing': return 'Purchasing'
      case 'manufacturing': return 'Manufacturing'
      case 'reports': return 'Reports & Analytics'
      default: return 'Dashboard'
    }
  }
  
  return 'Dashboard'
})

// Initialize with welcome message
onMounted(() => {
  if (messages.value.length === 0) {
    const welcomeMessage = {
      id: '1',
      type: 'assistant' as const,
      content: `Hello! I'm your AI assistant for Thabo's Spaza Shop. I can help you with insights across all modules including sales, inventory, customers, HR, and financials. What would you like to know?`,
      timestamp: new Date(),
      data: {
        actions: [
          { title: 'Business Overview', action: 'overview' },
          { title: 'Sales Insights', action: 'sales' },
          { title: 'Inventory Status', action: 'inventory' }
        ]
      }
    }
    messages.value.push(welcomeMessage)
  }
  
  // Initialize positions
  initializePositions()
  
  // Add event listeners for drag
  window.addEventListener('mousemove', handleDragMove)
  window.addEventListener('mouseup', handleDragEnd)
  window.addEventListener('touchmove', handleDragMove, { passive: false })
  window.addEventListener('touchend', handleDragEnd)
})

// Initialize positions
function initializePositions() {
  // Button initial position (bottom-right)
  if (typeof window !== 'undefined') {
    // On mobile, account for bottom navigation (80px height + 24px spacing = 104px)
    const isMobile = window.innerWidth < 1024
    const bottomOffset = isMobile ? 104 : 24
    
    buttonPosition.value = {
      x: window.innerWidth - 80, // 24px from right + 56px button width
      y: window.innerHeight - bottomOffset - 56  // Bottom offset + button height
    }
    
    // Panel initial position (bottom-right)
    // On mobile, make it more centered and higher up
    if (isMobile) {
      panelPosition.value = {
        x: Math.max(8, (window.innerWidth - 384) / 2), // Center horizontally with 8px min margin
        y: 80 // Start below mobile header
      }
    } else {
      panelPosition.value = {
        x: window.innerWidth - 408, // 24px from right + 384px panel width
        y: window.innerHeight - 624  // 24px from bottom + 600px panel height
      }
    }
  }
}

// Drag functions for button
function startDrag(event: MouseEvent | TouchEvent) {
  dragStartTime.value = Date.now()
  wasDragged.value = false // Reset drag flag
  
  const clientX = 'touches' in event ? event.touches[0]?.clientX ?? 0 : event.clientX
  const clientY = 'touches' in event ? event.touches[0]?.clientY ?? 0 : event.clientY
  
  dragOffset.value = {
    x: clientX - buttonPosition.value.x,
    y: clientY - buttonPosition.value.y
  }
  
  // Don't set isDragging immediately - wait for actual movement
}

// Drag functions for panel
function startDragPanel(event: MouseEvent | TouchEvent) {
  event.preventDefault()
  isDragging.value = true
  wasDragged.value = true // Panel drag is intentional
  
  const clientX = 'touches' in event ? event.touches[0]?.clientX ?? 0 : event.clientX
  const clientY = 'touches' in event ? event.touches[0]?.clientY ?? 0 : event.clientY
  
  dragOffset.value = {
    x: clientX - panelPosition.value.x,
    y: clientY - panelPosition.value.y
  }
}

function handleDragMove(event: MouseEvent | TouchEvent) {
  // Only start dragging if we've moved more than 5px (prevents accidental drags)
  if (!isDragging.value && dragStartTime.value > 0) {
    const clientX = 'touches' in event ? event.touches[0]?.clientX ?? 0 : event.clientX
    const clientY = 'touches' in event ? event.touches[0]?.clientY ?? 0 : event.clientY
    
    const targetX = isOpen.value ? panelPosition.value.x : buttonPosition.value.x
    const targetY = isOpen.value ? panelPosition.value.y : buttonPosition.value.y
    
    const deltaX = Math.abs(clientX - dragOffset.value.x - targetX)
    const deltaY = Math.abs(clientY - dragOffset.value.y - targetY)
    
    // Only start dragging if moved more than 5px
    if (deltaX > 5 || deltaY > 5) {
      isDragging.value = true
      wasDragged.value = true
      event.preventDefault()
    } else {
      return // Not enough movement yet
    }
  }
  
  if (!isDragging.value) return
  
  event.preventDefault()
  
  const clientX = 'touches' in event ? event.touches[0]?.clientX ?? 0 : event.clientX
  const clientY = 'touches' in event ? event.touches[0]?.clientY ?? 0 : event.clientY
  
  if (isOpen.value) {
    // Dragging panel
    const newX = clientX - dragOffset.value.x
    const newY = clientY - dragOffset.value.y
    
    // Constrain to viewport (responsive panel size)
    const isMobile = window.innerWidth < 1024
    const panelWidth = isMobile ? window.innerWidth - 16 : 384
    const panelHeight = isMobile ? window.innerHeight - 160 : 600
    
    const maxX = window.innerWidth - panelWidth
    const maxY = window.innerHeight - panelHeight
    
    panelPosition.value = {
      x: Math.max(0, Math.min(newX, maxX)),
      y: Math.max(0, Math.min(newY, maxY))
    }
  } else {
    // Dragging button
    const newX = clientX - dragOffset.value.x
    const newY = clientY - dragOffset.value.y
    
    // Constrain to viewport
    const isMobile = window.innerWidth < 1024
    const bottomNavHeight = isMobile ? 80 : 0
    const maxX = window.innerWidth - 56 // button width
    const maxY = window.innerHeight - bottomNavHeight - 56 // button height + bottom nav
    
    buttonPosition.value = {
      x: Math.max(0, Math.min(newX, maxX)),
      y: Math.max(0, Math.min(newY, maxY))
    }
  }
}

function handleDragEnd() {
  const wasDragging = isDragging.value
  isDragging.value = false
  dragStartTime.value = 0
  
  // If we were dragging, prevent the click event
  if (wasDragging) {
    return
  }
}

function handleButtonClick(event: MouseEvent) {
  // Only toggle if we didn't drag
  if (!wasDragged.value) {
    toggleAssistant()
  }
}

function getModuleIcon() {
  const module = currentModule.value.toLowerCase()
  
  if (module.includes('sales')) return CurrencyDollarIcon
  if (module.includes('customer') || module.includes('crm')) return UsersIcon
  if (module.includes('inventory')) return CubeIcon
  if (module.includes('human') || module.includes('hr')) return UserGroupIcon
  if (module.includes('accounting') || module.includes('finance')) return CalculatorIcon
  return ChartBarIcon
}

const toggleAssistant = () => {
  isOpen.value = !isOpen.value
  isMinimized.value = false
  if (isOpen.value) {
    unreadCount.value = 0
  }
}

const minimizeAssistant = () => {
  isOpen.value = false
  isMinimized.value = true
}

const expandAssistant = () => {
  isMinimized.value = false
  isOpen.value = true
}

const sendMessage = async () => {
  if (!newMessage.value.trim() || isTyping.value) return

  const userMessage = newMessage.value.trim()
  newMessage.value = ''

  // Add user message
  messages.value.push({
    id: Date.now().toString(),
    type: 'user',
    content: userMessage,
    timestamp: new Date()
  })

  // Show typing indicator
  isTyping.value = true
  await scrollToBottom()

  try {
    // Call backend AI service
    const response = await askAI(userMessage, shopId.value, `Current module: ${currentModule.value}`)
    
    if (response) {
      // Transform backend response to include contextual data
      const contextualData = generateContextualData(userMessage, response)
      
      messages.value.push({
        id: (Date.now() + 1).toString(),
        type: 'assistant',
        content: response.answer,
        timestamp: new Date(),
        data: contextualData
      })
    } else {
      // Fallback to local response if backend fails
      const fallbackResponse = generateContextualResponse(userMessage)
      messages.value.push({
        id: (Date.now() + 1).toString(),
        type: 'assistant',
        content: fallbackResponse.content,
        timestamp: new Date(),
        data: fallbackResponse.data
      })
    }
  } catch (err) {
    console.error('AI Error:', err)
    // Use fallback on error
    const fallbackResponse = generateContextualResponse(userMessage)
    messages.value.push({
      id: (Date.now() + 1).toString(),
      type: 'assistant',
      content: fallbackResponse.content,
      timestamp: new Date(),
      data: fallbackResponse.data
    })
  } finally {
    isTyping.value = false
    await scrollToBottom()
  }
}

/**
 * Generate contextual data (metrics and actions) based on the AI response
 * This adds UI-friendly data structures to the AI response
 */
const generateContextualData = (userMessage: string, aiResponse: any) => {
  const lowerMessage = userMessage.toLowerCase()
  const module = currentModule.value.toLowerCase()
  
  // Context-aware data based on current module and question
  if (module.includes('sales') || lowerMessage.includes('sales') || lowerMessage.includes('revenue')) {
    return {
      metrics: [
        { label: 'Daily Revenue', value: `R${businessData.revenue.daily.toLocaleString()}` },
        { label: 'Weekly Revenue', value: `R${businessData.revenue.weekly.toLocaleString()}` },
        { label: 'Growth', value: `+${businessData.revenue.growth}%` }
      ],
      actions: [
        { title: 'View Sales Analytics', route: '/sales/analytics' },
        { title: 'Check Inventory', route: '/inventory' }
      ]
    }
  }
  
  if (module.includes('customer') || module.includes('crm') || lowerMessage.includes('customer')) {
    return {
      metrics: [
        { label: 'Total Customers', value: businessData.customers.total.toString() },
        { label: 'Repeat Rate', value: `${businessData.customers.repeatRate}%` },
        { label: 'Active Today', value: businessData.customers.active.toString() }
      ],
      actions: [
        { title: 'View Customer List', route: '/crm/customers' },
        { title: 'Create Campaign', route: '/crm/automation' }
      ]
    }
  }
  
  if (module.includes('inventory') || lowerMessage.includes('inventory') || lowerMessage.includes('stock')) {
    return {
      metrics: [
        { label: 'Total Items', value: businessData.inventory.totalItems.toString() },
        { label: 'Low Stock Alerts', value: businessData.inventory.lowStockAlerts.toString() },
        { label: 'Critical Items', value: businessData.inventory.criticalItems.length.toString() }
      ],
      actions: [
        { title: 'View Inventory', route: '/inventory' },
        { title: 'Create Purchase Order', route: '/purchasing/orders' }
      ]
    }
  }
  
  // Default actions from AI suggestions if available
  const actions = aiResponse.suggestions?.map((suggestion: string) => ({
    title: suggestion,
    action: suggestion.toLowerCase()
  })) || []
  
  return {
    actions: actions.length > 0 ? actions.slice(0, 3) : [
      { title: `Go to ${currentModule.value}`, route: route.path },
      { title: 'View Dashboard', route: '/' }
    ]
  }
}

/**
 * Fallback response generator when backend is unavailable
 * Keeps existing mock functionality as graceful degradation
 */
const generateContextualResponse = (userMessage: string) => {
  const lowerMessage = userMessage.toLowerCase()
  const module = currentModule.value.toLowerCase()
  
  // Context-aware responses based on current module
  if (module.includes('sales') || lowerMessage.includes('sales') || lowerMessage.includes('revenue')) {
    if (lowerMessage.includes('forecast') || lowerMessage.includes('predict')) {
      return {
        content: `Based on sales patterns, I predict next week's revenue will be R${businessData.revenue.weekly.toLocaleString()} with 87% confidence.`,
        data: {
          metrics: [
            { label: 'Next Week', value: `R${businessData.revenue.weekly.toLocaleString()}` },
            { label: 'Confidence', value: '87%' },
            { label: 'Growth', value: `+${businessData.revenue.growth}%` }
          ],
          actions: [
            { title: 'View Sales Analytics', route: '/sales/analytics' },
            { title: 'Check Inventory', route: '/inventory' }
          ]
        }
      }
    }
    
    return {
      content: "Top performing products: White Bread (156 units), Coca Cola 2L (89 units), Milk 1L (78 units). These represent 65% of daily revenue.",
      data: {
        metrics: [
          { label: 'Top Seller', value: 'White Bread' },
          { label: 'Units Sold', value: '156' },
          { label: 'Revenue Share', value: '32%' }
        ],
        actions: [
          { title: 'View Product Details', route: '/inventory' },
          { title: 'Optimize Pricing', route: '/sales/analytics' }
        ]
      }
    }
  }
  
  if (module.includes('customer') || module.includes('crm') || lowerMessage.includes('customer')) {
    return {
      content: `You have ${businessData.customers.total} total customers with a ${businessData.customers.repeatRate}% repeat rate. ${businessData.customers.active} customers are currently active.`,
      data: {
        metrics: [
          { label: 'Total Customers', value: businessData.customers.total.toString() },
          { label: 'Repeat Rate', value: `${businessData.customers.repeatRate}%` },
          { label: 'Active Today', value: businessData.customers.active.toString() }
        ],
        actions: [
          { title: 'View Customer List', route: '/crm/customers' },
          { title: 'Create Campaign', route: '/crm/automation' }
        ]
      }
    }
  }
  
  if (module.includes('inventory') || lowerMessage.includes('inventory') || lowerMessage.includes('stock')) {
    return {
      content: `${businessData.inventory.lowStockAlerts} items need attention: ${businessData.inventory.criticalItems.join(', ')}. Immediate reorder recommended.`,
      data: {
        metrics: [
          { label: 'Total Items', value: businessData.inventory.totalItems.toString() },
          { label: 'Low Stock Alerts', value: businessData.inventory.lowStockAlerts.toString() },
          { label: 'Critical Items', value: businessData.inventory.criticalItems.length.toString() }
        ],
        actions: [
          { title: 'View Inventory', route: '/inventory' },
          { title: 'Create Purchase Order', route: '/purchasing/orders' }
        ]
      }
    }
  }
  
  if (module.includes('human') || module.includes('hr') || lowerMessage.includes('employee') || lowerMessage.includes('payroll')) {
    return {
      content: `${businessData.employees.total} active employees. Monthly payroll: R${businessData.employees.monthlyPayroll.toLocaleString()}. Attendance rate: ${businessData.employees.attendance}%.`,
      data: {
        metrics: [
          { label: 'Employees', value: businessData.employees.total.toString() },
          { label: 'Monthly Payroll', value: `R${businessData.employees.monthlyPayroll.toLocaleString()}` },
          { label: 'Attendance', value: `${businessData.employees.attendance}%` }
        ],
        actions: [
          { title: 'View Employees', route: '/hr/employees' },
          { title: 'Process Payroll', route: '/hr/payroll' }
        ]
      }
    }
  }
  
  if (module.includes('accounting') || module.includes('finance') || lowerMessage.includes('financial') || lowerMessage.includes('accounting')) {
    return {
      content: `Financial health is strong. Cash: R${businessData.financial.cashOnHand.toLocaleString()}, Profit margin: ${businessData.financial.profitMargin}%, Receivables: R${businessData.financial.accountsReceivable.toLocaleString()}.`,
      data: {
        metrics: [
          { label: 'Cash on Hand', value: `R${businessData.financial.cashOnHand.toLocaleString()}` },
          { label: 'Profit Margin', value: `${businessData.financial.profitMargin}%` },
          { label: 'Receivables', value: `R${businessData.financial.accountsReceivable.toLocaleString()}` }
        ],
        actions: [
          { title: 'View Accounts', route: '/accounts' },
          { title: 'Generate Report', route: '/reports/financial' }
        ]
      }
    }
  }
  
  // General business overview
  if (lowerMessage.includes('overview') || lowerMessage.includes('business') || lowerMessage.includes('summary')) {
    return {
      content: `Business overview: Daily revenue R${businessData.revenue.daily.toLocaleString()}, ${businessData.customers.active} customers today, ${businessData.inventory.lowStockAlerts} stock alerts.`,
      data: {
        metrics: [
          { label: 'Daily Revenue', value: `R${businessData.revenue.daily.toLocaleString()}` },
          { label: 'Customers Today', value: businessData.customers.active.toString() },
          { label: 'Stock Alerts', value: businessData.inventory.lowStockAlerts.toString() }
        ],
        actions: [
          { title: 'View Dashboard', route: '/' },
          { title: 'Check Sales', route: '/sales' },
          { title: 'Review Inventory', route: '/inventory' }
        ]
      }
    }
  }
  
  // Default response
  return {
    content: `I'm here to help with ${currentModule.value}! You can ask me about any aspect of your business operations.`,
    data: {
      actions: [
        { title: `Go to ${currentModule.value}`, route: route.path },
        { title: 'View Dashboard', route: '/' }
      ]
    }
  }
}

const handleAction = (action: any) => {
  if (action.route) {
    router.push(action.route)
  } else if (action.action) {
    newMessage.value = getActionMessage(action.action)
    sendMessage()
  }
}

const getActionMessage = (actionType: string) => {
  const actionMessages: Record<string, string> = {
    overview: 'Give me a business overview',
    sales: 'Show me sales insights',
    inventory: 'What\'s my inventory status?',
    customers: 'Tell me about my customers',
    financial: 'Show me financial summary',
    hr: 'Give me HR insights'
  }
  
  return actionMessages[actionType] || actionType
}

const formatTime = (date: Date) => {
  return date.toLocaleTimeString('en-ZA', { 
    hour: '2-digit', 
    minute: '2-digit' 
  })
}

const scrollToBottom = async () => {
  await nextTick()
  if (chatContainer.value) {
    chatContainer.value.scrollTop = chatContainer.value.scrollHeight
  }
}

// Watch for route changes to update context
watch(() => route.path, () => {
  // Could add auto-message about new module context
  if (messages.value.length > 0 && Math.random() > 0.7) {
    // Occasionally show context awareness
    unreadCount.value += 1
  }
})
</script>

<style scoped>
.bounce-enter-active {
  animation: bounce-in 0.5s;
}
.bounce-leave-active {
  animation: bounce-in 0.5s reverse;
}

@keyframes bounce-in {
  0% {
    transform: scale(0);
  }
  50% {
    transform: scale(1.2);
  }
  100% {
    transform: scale(1);
  }
}

.slide-up-enter-active,
.slide-up-leave-active {
  transition: all 0.3s ease;
}

.slide-up-enter-from {
  transform: translateY(100px);
  opacity: 0;
}

.slide-up-leave-to {
  transform: translateY(100px);
  opacity: 0;
}

.slide-right-enter-active,
.slide-right-leave-active {
  transition: all 0.3s ease;
}

.slide-right-enter-from {
  transform: translateX(100px);
  opacity: 0;
}

.slide-right-leave-to {
  transform: translateX(100px);
  opacity: 0;
}
</style>
