<template>
  <div class="h-full overflow-y-auto p-6 custom-scrollbar">
    <!-- TOSS AI Copilot Page -->
    <div class="text-center mb-8">
      <div class="w-16 h-16 rounded-2xl bg-gradient-to-br from-blue-500 to-purple-600 flex items-center justify-center mx-auto mb-4">
        <Icon name="lucide:bot" class="w-8 h-8 text-white" />
      </div>
      <h1 class="text-3xl font-bold text-stone-900 dark:text-white mb-2">TOSS AI Copilot</h1>
      <p class="text-stone-500 dark:text-stone-400">Your intelligent business assistant</p>
    </div>

    <!-- Quick Actions -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-8">
      <button v-for="action in quickActions" :key="action.label" class="p-4 bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700 rounded-xl hover:shadow-lg transition-all hover:border-blue-300 dark:hover:border-blue-600 text-left">
        <div :class="['w-10 h-10 rounded-lg flex items-center justify-center mb-3', action.bgColor]">
          <Icon :name="action.icon" :class="['w-5 h-5', action.iconColor]" />
        </div>
        <h3 class="text-sm font-semibold text-stone-900 dark:text-white mb-1">{{ action.label }}</h3>
        <p class="text-xs text-stone-500 dark:text-stone-400">{{ action.description }}</p>
      </button>
    </div>

    <!-- Chat Interface -->
    <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
      <CardHeader class="border-b border-stone-200 dark:border-stone-700">
        <div class="flex items-center justify-between">
          <div class="flex items-center gap-3">
            <div class="w-8 h-8 rounded-full bg-gradient-to-br from-blue-500 to-purple-600 flex items-center justify-center">
              <Icon name="lucide:sparkles" class="w-4 h-4 text-white" />
            </div>
            <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Chat with Copilot</CardTitle>
          </div>
          <Badge class="bg-green-100 text-green-800">Online</Badge>
        </div>
      </CardHeader>
      <CardContent class="p-0">
        <!-- Messages Area -->
        <div class="h-96 overflow-y-auto p-4 space-y-4">
          <div v-for="message in messages" :key="message.id" :class="['flex', message.sender === 'user' ? 'justify-end' : 'justify-start']">
            <div :class="['max-w-[80%] rounded-2xl px-4 py-3', message.sender === 'user' ? 'bg-blue-500 text-white' : 'bg-stone-100 dark:bg-stone-700 text-stone-900 dark:text-white']">
              <p class="text-sm">{{ message.content }}</p>
              <span :class="['text-xs mt-1 block', message.sender === 'user' ? 'text-blue-200' : 'text-stone-400']">{{ message.time }}</span>
            </div>
          </div>
        </div>

        <!-- Input Area -->
        <div class="border-t border-stone-200 dark:border-stone-700 p-4">
          <div class="flex gap-3">
            <input 
              v-model="newMessage"
              type="text" 
              placeholder="Ask me anything about your business..." 
              class="flex-1 px-4 py-3 border border-stone-200 dark:border-stone-700 rounded-xl bg-white dark:bg-stone-900 text-stone-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-blue-500"
              @keyup.enter="sendMessage"
            />
            <Button class="bg-blue-500 hover:bg-blue-600 text-white px-6" @click="sendMessage">
              <Icon name="lucide:send" class="w-5 h-5" />
            </Button>
          </div>
          <div class="flex gap-2 mt-3">
            <button class="px-3 py-1.5 text-xs bg-stone-100 dark:bg-stone-700 text-stone-600 dark:text-stone-300 rounded-full hover:bg-stone-200 dark:hover:bg-stone-600">
              📊 Show sales report
            </button>
            <button class="px-3 py-1.5 text-xs bg-stone-100 dark:bg-stone-700 text-stone-600 dark:text-stone-300 rounded-full hover:bg-stone-200 dark:hover:bg-stone-600">
              📦 Check low stock
            </button>
            <button class="px-3 py-1.5 text-xs bg-stone-100 dark:bg-stone-700 text-stone-600 dark:text-stone-300 rounded-full hover:bg-stone-200 dark:hover:bg-stone-600">
              💰 Today's earnings
            </button>
          </div>
        </div>
      </CardContent>
    </Card>

    <!-- Recent Insights -->
    <div class="mt-6">
      <h2 class="text-lg font-semibold text-stone-900 dark:text-white mb-4">Recent Insights</h2>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <Card v-for="insight in insights" :key="insight.id" class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
          <CardContent class="p-4">
            <div class="flex items-start gap-3">
              <div :class="['w-8 h-8 rounded-lg flex items-center justify-center flex-shrink-0', insight.bgColor]">
                <Icon :name="insight.icon" :class="['w-4 h-4', insight.iconColor]" />
              </div>
              <div>
                <h4 class="text-sm font-medium text-stone-900 dark:text-white mb-1">{{ insight.title }}</h4>
                <p class="text-xs text-stone-500 dark:text-stone-400">{{ insight.description }}</p>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Card, CardContent, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'
import { Badge } from '~/components/ui/badge'

definePageMeta({
  layout: 'default'
})

const quickActions = [
  { label: 'Draft Restock Order', description: 'Auto-generate purchase orders', icon: 'lucide:package-plus', bgColor: 'bg-blue-100 dark:bg-blue-900/30', iconColor: 'text-blue-600' },
  { label: 'Analyze Sales', description: 'Get sales insights & trends', icon: 'lucide:trending-up', bgColor: 'bg-green-100 dark:bg-green-900/30', iconColor: 'text-green-600' },
  { label: 'Customer Insights', description: 'Understand buying patterns', icon: 'lucide:users', bgColor: 'bg-purple-100 dark:bg-purple-900/30', iconColor: 'text-purple-600' },
  { label: 'Business Advice', description: 'Get actionable recommendations', icon: 'lucide:lightbulb', bgColor: 'bg-orange-100 dark:bg-orange-900/30', iconColor: 'text-orange-600' },
]

const messages = ref([
  { id: 1, sender: 'ai', content: 'Hello! I\'m your TOSS AI Copilot. I can help you manage your business, analyze sales, suggest restock orders, and much more. What would you like to know?', time: '10:30 AM' },
  { id: 2, sender: 'user', content: 'What products are running low on stock?', time: '10:31 AM' },
  { id: 3, sender: 'ai', content: 'Based on your current inventory, I found 8 items below reorder level:\n\n• Sasko Bread (5 left, reorder at 10)\n• Sunfoil Oil 750ml (3 left, reorder at 8)\n• White Star Maize 2.5kg (4 left, reorder at 12)\n\nWould you like me to draft a purchase order for these items?', time: '10:31 AM' },
])

const newMessage = ref('')

const sendMessage = () => {
  if (newMessage.value.trim()) {
    messages.value.push({
      id: messages.value.length + 1,
      sender: 'user',
      content: newMessage.value,
      time: new Date().toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' })
    })
    newMessage.value = ''
  }
}

const insights = [
  { id: 1, title: 'Sales trending up', description: 'Your sales are 15% higher than last week', icon: 'lucide:trending-up', bgColor: 'bg-green-100 dark:bg-green-900/30', iconColor: 'text-green-600' },
  { id: 2, title: 'Popular time', description: 'Peak sales hours are 4-6 PM', icon: 'lucide:clock', bgColor: 'bg-blue-100 dark:bg-blue-900/30', iconColor: 'text-blue-600' },
  { id: 3, title: 'Restock alert', description: '8 items need restocking soon', icon: 'lucide:alert-triangle', bgColor: 'bg-orange-100 dark:bg-orange-900/30', iconColor: 'text-orange-600' },
]
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: hsl(var(--muted)); }
.custom-scrollbar::-webkit-scrollbar-thumb { background: hsl(var(--muted-foreground)); border-radius: 3px; }
</style>
