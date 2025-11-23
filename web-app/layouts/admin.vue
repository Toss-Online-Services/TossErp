<template>
  <div class="min-h-screen bg-background">
    <!-- Sidebar -->
    <aside class="fixed left-0 top-0 h-full w-64 border-r bg-card z-40 transform transition-transform duration-200 ease-in-out"
           :class="sidebarOpen ? 'translate-x-0' : '-translate-x-full md:translate-x-0'">
      <div class="flex flex-col h-full">
        <!-- Logo -->
        <div class="flex items-center space-x-2 p-6 border-b">
          <div class="w-10 h-10 bg-primary rounded-lg flex items-center justify-center">
            <span class="text-primary-foreground font-bold text-xl">T</span>
          </div>
          <span class="text-xl font-bold">TOSS</span>
        </div>
        
        <!-- Navigation -->
        <nav class="flex-1 p-4 space-y-1 overflow-y-auto">
          <NuxtLink
            v-for="item in menuItems"
            :key="item.path"
            :to="item.path"
            class="flex items-center space-x-3 px-4 py-3 rounded-lg transition-colors hover:bg-accent"
            :class="$route.path === item.path ? 'bg-accent text-accent-foreground' : 'text-muted-foreground hover:text-foreground'"
          >
            <Icon :name="item.icon" class="w-5 h-5" />
            <span>{{ item.label }}</span>
          </NuxtLink>
        </nav>
        
        <!-- User Section -->
        <div class="p-4 border-t">
          <div class="flex items-center space-x-3 px-4 py-3">
            <div class="w-10 h-10 bg-primary/10 rounded-full flex items-center justify-center">
              <Icon name="lucide:user" class="w-5 h-5 text-primary" />
            </div>
            <div class="flex-1 min-w-0">
              <p class="text-sm font-medium truncate">Business Owner</p>
              <p class="text-xs text-muted-foreground truncate">owner@example.com</p>
            </div>
          </div>
        </div>
      </div>
    </aside>
    
    <!-- Mobile Overlay -->
    <div
      v-if="sidebarOpen"
      class="fixed inset-0 bg-black/50 z-30 md:hidden"
      @click="sidebarOpen = false"
    ></div>
    
    <!-- Main Content -->
    <div class="md:ml-64">
      <!-- Top Bar -->
      <header class="sticky top-0 z-20 border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
        <div class="flex items-center justify-between px-4 py-4">
          <button
            @click="sidebarOpen = !sidebarOpen"
            class="md:hidden p-2 rounded-lg hover:bg-accent"
          >
            <Icon name="lucide:menu" class="w-5 h-5" />
          </button>
          <div class="flex items-center space-x-4 ml-auto">
            <!-- AI Copilot Button -->
            <Button variant="outline" size="sm" @click="showCopilot = true">
              <Icon name="lucide:sparkles" class="w-4 h-4 mr-2" />
              AI Copilot
            </Button>
            <button class="p-2 rounded-lg hover:bg-accent">
              <Icon name="lucide:bell" class="w-5 h-5" />
            </button>
            <button class="p-2 rounded-lg hover:bg-accent">
              <Icon name="lucide:settings" class="w-5 h-5" />
            </button>
          </div>
        </div>
      </header>
      
      <!-- Page Content -->
      <main class="p-6">
        <slot />
      </main>
    </div>
    
    <!-- AI Copilot Modal -->
    <div
      v-if="showCopilot"
      class="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4"
      @click.self="showCopilot = false"
    >
      <Card class="w-full max-w-2xl max-h-[80vh] overflow-hidden flex flex-col">
        <CardHeader class="flex-shrink-0">
          <div class="flex items-center justify-between">
            <div class="flex items-center space-x-2">
              <Icon name="lucide:sparkles" class="w-6 h-6 text-primary" />
              <CardTitle>AI Business Copilot</CardTitle>
            </div>
            <button @click="showCopilot = false" class="p-2 rounded-lg hover:bg-accent">
              <Icon name="lucide:x" class="w-5 h-5" />
            </button>
          </div>
        </CardHeader>
        <CardContent class="flex-1 overflow-y-auto">
          <div class="space-y-4">
            <div class="p-4 bg-muted rounded-lg">
              <p class="text-sm text-muted-foreground">
                👋 Hi! I'm your AI business manager. I can help you with:
              </p>
              <ul class="mt-2 space-y-1 text-sm text-muted-foreground list-disc list-inside">
                <li>Stock alerts and reorder suggestions</li>
                <li>Sales insights and trends</li>
                <li>Financial advice and cash flow tips</li>
                <li>Business growth recommendations</li>
              </ul>
            </div>
            <div class="space-y-2">
              <div v-for="message in copilotMessages" :key="message.id" class="p-3 rounded-lg"
                   :class="message.type === 'user' ? 'bg-primary/10 ml-8' : 'bg-muted mr-8'">
                <p class="text-sm">{{ message.text }}</p>
              </div>
            </div>
          </div>
        </CardContent>
        <div class="p-4 border-t flex-shrink-0">
          <div class="flex space-x-2">
            <Input
              v-model="copilotInput"
              placeholder="Ask me anything about your business..."
              class="flex-1"
              @keyup.enter="sendCopilotMessage"
            />
            <Button @click="sendCopilotMessage">
              <Icon name="lucide:send" class="w-4 h-4" />
            </Button>
          </div>
        </div>
      </Card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

const sidebarOpen = ref(false)
const showCopilot = ref(false)
const copilotInput = ref('')
const copilotMessages = ref([
  {
    id: 1,
    type: 'assistant',
    text: 'Hello! I noticed your bread stock is running low. Would you like me to prepare a reorder list?'
  }
])

const menuItems = [
  { path: '/dashboard', label: 'Dashboard', icon: 'lucide:layout-dashboard' },
  { path: '/dashboard/inventory', label: 'Inventory', icon: 'lucide:package' },
  { path: '/dashboard/sales', label: 'Sales & POS', icon: 'lucide:shopping-cart' },
  { path: '/dashboard/purchasing', label: 'Purchasing', icon: 'lucide:shopping-bag' },
  { path: '/dashboard/customers', label: 'Customers', icon: 'lucide:users' },
  { path: '/dashboard/suppliers', label: 'Suppliers', icon: 'lucide:truck' },
  { path: '/dashboard/financials', label: 'Financials', icon: 'lucide:wallet' },
  { path: '/dashboard/reports', label: 'Reports', icon: 'lucide:file-text' },
  { path: '/dashboard/network', label: 'Network', icon: 'lucide:network' },
  { path: '/dashboard/settings', label: 'Settings', icon: 'lucide:settings' }
]

const sendCopilotMessage = () => {
  if (!copilotInput.value.trim()) return
  
  copilotMessages.value.push({
    id: copilotMessages.value.length + 1,
    type: 'user',
    text: copilotInput.value
  })
  
  // Simulate AI response
  setTimeout(() => {
    copilotMessages.value.push({
      id: copilotMessages.value.length + 1,
      type: 'assistant',
      text: 'I understand your question. Let me analyze your business data and provide you with insights...'
    })
  }, 1000)
  
  copilotInput.value = ''
}
</script>


