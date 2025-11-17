<template>
  <AppLayout>
    <div class="p-6 space-y-6 max-w-4xl mx-auto">
      <!-- Header -->
      <div class="flex justify-between items-center">
        <div>
          <h1 class="text-2xl font-bold">AI Business Assistant</h1>
          <p class="text-muted-foreground">Get instant help with your business operations and decisions</p>
        </div>
        <div class="flex items-center space-x-4">
          <Button variant="outline">
            <History class="h-4 w-4 mr-2" />
            Chat History
          </Button>
          <Button variant="outline">
            <Settings class="h-4 w-4 mr-2" />
            Settings
          </Button>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <Button variant="outline" class="h-20 flex-col space-y-2 p-4" @click="quickAction('inventory')">
          <Package class="h-6 w-6 text-blue-600" />
          <div class="text-center">
            <p class="font-medium">Check Inventory</p>
            <p class="text-xs text-muted-foreground">View stock levels and alerts</p>
          </div>
        </Button>
        <Button variant="outline" class="h-20 flex-col space-y-2 p-4" @click="quickAction('sales')">
          <TrendingUp class="h-6 w-6 text-green-600" />
          <div class="text-center">
            <p class="font-medium">Sales Analysis</p>
            <p class="text-xs text-muted-foreground">Analyze sales performance</p>
          </div>
        </Button>
        <Button variant="outline" class="h-20 flex-col space-y-2 p-4" @click="quickAction('reorder')">
          <ShoppingCart class="h-6 w-6 text-purple-600" />
          <div class="text-center">
            <p class="font-medium">Suggest Reorder</p>
            <p class="text-xs text-muted-foreground">AI-powered restocking</p>
          </div>
        </Button>
      </div>

      <!-- Chat Interface -->
      <div class="bg-white dark:bg-gray-800 rounded-lg border h-96 flex flex-col">
        <!-- Chat Messages -->
        <div class="flex-1 p-6 overflow-y-auto space-y-4">
          <div v-for="message in messages" :key="message.id" class="flex" :class="message.type === 'user' ? 'justify-end' : 'justify-start'">
            <div class="max-w-3xl">
              <div 
                class="rounded-lg p-4"
                :class="message.type === 'user' 
                  ? 'bg-primary text-primary-foreground ml-12' 
                  : 'bg-muted mr-12'"
              >
                <div v-if="message.type === 'ai'" class="flex items-start space-x-3">
                  <div class="w-8 h-8 bg-gradient-to-r from-blue-500 to-purple-600 rounded-full flex items-center justify-center flex-shrink-0 mt-1">
                    <Bot class="h-4 w-4 text-white" />
                  </div>
                  <div class="flex-1">
                    <p class="text-sm font-medium text-blue-600 dark:text-blue-400 mb-1">TOSS AI Assistant</p>
                    <div v-html="message.content"></div>
                  </div>
                </div>
                <div v-else>
                  {{ message.content }}
                </div>
              </div>
              <p class="text-xs text-muted-foreground mt-1" :class="message.type === 'user' ? 'text-right' : 'text-left'">
                {{ message.timestamp }}
              </p>
            </div>
          </div>
          
          <!-- Typing indicator -->
          <div v-if="isTyping" class="flex justify-start">
            <div class="max-w-3xl">
              <div class="bg-muted rounded-lg p-4 mr-12">
                <div class="flex items-start space-x-3">
                  <div class="w-8 h-8 bg-gradient-to-r from-blue-500 to-purple-600 rounded-full flex items-center justify-center flex-shrink-0">
                    <Bot class="h-4 w-4 text-white" />
                  </div>
                  <div class="flex-1">
                    <p class="text-sm font-medium text-blue-600 dark:text-blue-400 mb-1">TOSS AI Assistant</p>
                    <div class="flex space-x-1">
                      <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce"></div>
                      <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="animation-delay: 0.1s"></div>
                      <div class="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style="animation-delay: 0.2s"></div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Chat Input -->
        <div class="border-t p-4">
          <div class="flex space-x-4">
            <div class="relative flex-1">
              <input
                v-model="newMessage"
                @keyup.enter="sendMessage"
                type="text"
                placeholder="Ask me anything about your business..."
                class="w-full px-4 py-3 pr-12 rounded-lg border border-input bg-background"
                :disabled="isTyping"
              />
              <Button 
                size="sm" 
                variant="ghost" 
                class="absolute right-2 top-2"
                @click="sendMessage"
                :disabled="!newMessage.trim() || isTyping"
              >
                <Send class="h-4 w-4" />
              </Button>
            </div>
            <Button variant="outline" size="sm">
              <Mic class="h-4 w-4" />
            </Button>
          </div>
          <p class="text-xs text-muted-foreground mt-2 text-center">
            AI assistant can make mistakes. Verify important information.
          </p>
        </div>
      </div>

      <!-- Suggested Questions -->
      <div class="bg-white dark:bg-gray-800 rounded-lg border p-6">
        <h3 class="text-lg font-semibold mb-4">Suggested Questions</h3>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-3">
          <Button 
            variant="outline" 
            class="justify-start h-auto p-3 text-left"
            v-for="suggestion in suggestions" 
            :key="suggestion"
            @click="sendSuggestion(suggestion)"
          >
            <MessageCircle class="h-4 w-4 mr-2 flex-shrink-0" />
            <span class="text-sm">{{ suggestion }}</span>
          </Button>
        </div>
      </div>

      <!-- Recent Insights -->
      <div class="bg-white dark:bg-gray-800 rounded-lg border p-6">
        <h3 class="text-lg font-semibold mb-4">Recent AI Insights</h3>
        <div class="space-y-3">
          <div v-for="insight in recentInsights" :key="insight.id" class="flex items-start space-x-3 p-3 bg-blue-50 dark:bg-blue-900/20 rounded-lg">
            <div class="w-8 h-8 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center flex-shrink-0">
              <Lightbulb class="h-4 w-4 text-blue-600" />
            </div>
            <div class="flex-1">
              <p class="font-medium text-blue-800 dark:text-blue-200">{{ insight.title }}</p>
              <p class="text-sm text-blue-600 dark:text-blue-400 mt-1">{{ insight.description }}</p>
              <p class="text-xs text-blue-500 dark:text-blue-500 mt-2">{{ insight.timestamp }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </AppLayout>
</template>

<script setup lang="ts">
import { ref, nextTick } from 'vue'
import { 
  Bot,
  History,
  Settings,
  Package,
  TrendingUp,
  ShoppingCart,
  Send,
  Mic,
  MessageCircle,
  Lightbulb
} from 'lucide-vue-next'

// Reactive data
const newMessage = ref('')
const isTyping = ref(false)

// Chat messages
const messages = ref([
  {
    id: 1,
    type: 'ai',
    content: `<p>Hello! I'm your TOSS AI Assistant. I'm here to help you with:</p>
              <ul class="list-disc list-inside mt-2 space-y-1">
                <li>Inventory management and stock alerts</li>
                <li>Sales analysis and business insights</li>
                <li>Customer recommendations and marketing</li>
                <li>Financial planning and cash flow</li>
                <li>Township-specific business advice</li>
              </ul>
              <p class="mt-2">What would you like to know?</p>`,
    timestamp: '10:30 AM'
  }
])

// Suggested questions
const suggestions = ref([
  "What products should I reorder this week?",
  "Show me my best-selling items this month",
  "How can I improve my cash flow?",
  "What marketing strategies work for township businesses?",
  "Which customers haven't bought recently?",
  "Calculate my profit margins by category"
])

// Recent insights
const recentInsights = ref([
  {
    id: 1,
    title: "Stock Alert: Low Inventory",
    description: "Maize meal and cooking oil are running low. Consider reordering from Township Supply Co.",
    timestamp: "2 hours ago"
  },
  {
    id: 2,
    title: "Sales Opportunity",
    description: "Weekend sales typically increase by 35%. Stock up on bread and beverages.",
    timestamp: "Yesterday"
  },
  {
    id: 3,
    title: "Customer Retention",
    description: "Loyal customers spend 40% more. Consider a WhatsApp loyalty program.",
    timestamp: "2 days ago"
  }
])

// Methods
const sendMessage = async () => {
  if (!newMessage.value.trim()) return

  const userMessage = {
    id: messages.value.length + 1,
    type: 'user',
    content: newMessage.value,
    timestamp: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
  }

  messages.value.push(userMessage)
  const query = newMessage.value
  newMessage.value = ''
  isTyping.value = true

  // Simulate AI response delay
  await new Promise(resolve => setTimeout(resolve, 1000 + Math.random() * 2000))

  const aiResponse = generateAIResponse(query)
  messages.value.push({
    id: messages.value.length + 1,
    type: 'ai',
    content: aiResponse,
    timestamp: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
  })

  isTyping.value = false
  await nextTick()
  // Scroll to bottom would go here
}

const sendSuggestion = (suggestion: string) => {
  newMessage.value = suggestion
  sendMessage()
}

const quickAction = (action: string) => {
  const actions = {
    'inventory': 'Check my current inventory levels and show any low stock alerts',
    'sales': 'Analyze my sales performance for this month and show trends',
    'reorder': 'What products should I reorder based on my sales patterns?'
  }
  newMessage.value = actions[action as keyof typeof actions] || ''
  sendMessage()
}

const generateAIResponse = (query: string): string => {
  const lowerQuery = query.toLowerCase()
  
  if (lowerQuery.includes('inventory') || lowerQuery.includes('stock')) {
    return `<p>Here's your current inventory status:</p>
            <div class="mt-3 p-3 bg-yellow-100 dark:bg-yellow-900/30 rounded">
              <p class="font-medium text-yellow-800 dark:text-yellow-200">⚠️ Low Stock Items:</p>
              <ul class="list-disc list-inside mt-1 text-sm">
                <li>Maize Meal 2.5kg: 8 units (reorder level: 15)</li>
                <li>Cooking Oil 750ml: 12 units (reorder level: 20)</li>
                <li>Sugar 2kg: 0 units (OUT OF STOCK)</li>
              </ul>
            </div>
            <p class="mt-3">I recommend placing an order with Fresh Produce Wholesalers today to avoid stockouts.</p>`
  }
  
  if (lowerQuery.includes('sales') || lowerQuery.includes('performance')) {
    return `<p>Your sales performance this month:</p>
            <div class="mt-3 space-y-2">
              <div class="flex justify-between p-2 bg-green-100 dark:bg-green-900/30 rounded">
                <span>Total Revenue:</span>
                <span class="font-bold">R 34,580</span>
              </div>
              <div class="flex justify-between p-2 bg-blue-100 dark:bg-blue-900/30 rounded">
                <span>Orders:</span>
                <span class="font-bold">247 (+12% vs last month)</span>
              </div>
              <div class="flex justify-between p-2 bg-purple-100 dark:bg-purple-900/30 rounded">
                <span>Best Seller:</span>
                <span class="font-bold">White Bread (156 sold)</span>
              </div>
            </div>
            <p class="mt-3">Great job! Your sales are trending upward. Consider promoting slow-moving items like household products.</p>`
  }
  
  if (lowerQuery.includes('reorder') || lowerQuery.includes('should i')) {
    return `<p>Based on your sales patterns, I recommend ordering:</p>
            <div class="mt-3 p-3 bg-blue-100 dark:bg-blue-900/30 rounded">
              <p class="font-medium">🛒 Recommended Orders:</p>
              <ul class="list-disc list-inside mt-2 text-sm space-y-1">
                <li>Maize Meal 2.5kg: 30 units (supplier: Township Supply Co)</li>
                <li>Cooking Oil 750ml: 40 units (supplier: Fresh Produce Wholesalers)</li>
                <li>Sugar 2kg: 20 units (URGENT - currently out of stock)</li>
                <li>White Bread: 50 units (high demand weekends)</li>
              </ul>
            </div>
            <p class="mt-3">Total estimated cost: R 3,250. This should cover 2 weeks of demand based on your sales history.</p>`
  }
  
  if (lowerQuery.includes('cash flow') || lowerQuery.includes('financial')) {
    return `<p>Here's your cash flow analysis:</p>
            <div class="mt-3 space-y-2">
              <div class="p-3 bg-green-100 dark:bg-green-900/30 rounded">
                <p class="font-medium">💰 Current Cash Position: R 12,450</p>
                <p class="text-sm mt-1">Healthy level for operations</p>
              </div>
              <div class="p-3 bg-yellow-100 dark:bg-yellow-900/30 rounded">
                <p class="font-medium">📊 Weekly Cash Flow:</p>
                <p class="text-sm">Inflow: R 8,500 | Outflow: R 6,200</p>
              </div>
            </div>
            <p class="mt-3">Tip: Consider offering small discounts for cash payments to improve cash flow timing.</p>`
  }
  
  // Default response
  return `<p>I understand you're asking about "${query}". Let me help you with that.</p>
          <p class="mt-2">As your AI business assistant, I can provide insights on:</p>
          <ul class="list-disc list-inside mt-2 space-y-1">
            <li>Inventory management and stock optimization</li>
            <li>Sales trends and customer behavior</li>
            <li>Financial planning and cash flow</li>
            <li>Marketing strategies for township businesses</li>
          </ul>
          <p class="mt-2">Could you be more specific about what you'd like to know?</p>`
}
</script>