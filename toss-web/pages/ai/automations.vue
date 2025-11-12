<template>
  <AppLayout>
    <div class="p-6 space-y-6">
      <!-- Header -->
      <div class="flex justify-between items-center">
        <div>
          <h1 class="text-2xl font-bold">AI Automations</h1>
          <p class="text-muted-foreground">Set up automated business processes powered by AI</p>
        </div>
        <div class="flex items-center space-x-4">
          <Button variant="outline">
            <FileText class="h-4 w-4 mr-2" />
            Templates
          </Button>
          <Button>
            <Plus class="h-4 w-4 mr-2" />
            Create Automation
          </Button>
        </div>
      </div>

      <!-- Automation Stats -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-blue-100 dark:bg-blue-900 rounded-lg">
              <Zap class="h-6 w-6 text-blue-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Active Automations</p>
              <p class="text-2xl font-bold">{{ activeAutomations }}</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-green-100 dark:bg-green-900 rounded-lg">
              <CheckCircle class="h-6 w-6 text-green-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Tasks Completed</p>
              <p class="text-2xl font-bold">{{ tasksCompleted }}</p>
              <p class="text-xs text-green-600">This month</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-purple-100 dark:bg-purple-900 rounded-lg">
              <Clock class="h-6 w-6 text-purple-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Time Saved</p>
              <p class="text-2xl font-bold">{{ timeSaved }}h</p>
              <p class="text-xs text-purple-600">This month</p>
            </div>
          </div>
        </div>
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <div class="flex items-center space-x-3">
            <div class="p-2 bg-yellow-100 dark:bg-yellow-900 rounded-lg">
              <TrendingUp class="h-6 w-6 text-yellow-600" />
            </div>
            <div>
              <p class="text-sm text-muted-foreground">Success Rate</p>
              <p class="text-2xl font-bold">{{ successRate }}%</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Setup Templates -->
      <div class="bg-white dark:bg-gray-800 rounded-lg border">
        <div class="p-6 border-b">
          <h3 class="text-lg font-semibold">Quick Setup Templates</h3>
          <p class="text-muted-foreground">Get started with pre-built automation templates</p>
        </div>
        <div class="p-6">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div v-for="template in templates" :key="template.id" class="border rounded-lg p-4 hover:shadow-md transition-shadow cursor-pointer" @click="setupTemplate(template)">
              <div class="flex items-start justify-between mb-3">
                <div :class="`p-3 rounded-lg ${template.color}`">
                  <component :is="template.icon" class="h-6 w-6" />
                </div>
                <span class="px-2 py-1 bg-blue-100 text-blue-800 text-xs rounded">{{ template.category }}</span>
              </div>
              <h4 class="font-semibold mb-2">{{ template.name }}</h4>
              <p class="text-sm text-muted-foreground mb-3">{{ template.description }}</p>
              <div class="flex items-center justify-between text-xs">
                <span class="text-green-600">{{ template.setupTime }} setup</span>
                <span class="text-muted-foreground">{{ template.popularity }} users</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Active Automations -->
      <div class="bg-white dark:bg-gray-800 rounded-lg border">
        <div class="p-6 border-b">
          <div class="flex justify-between items-center">
            <h3 class="text-lg font-semibold">Your Automations</h3>
            <select class="px-3 py-2 border rounded-md text-sm">
              <option value="all">All Automations</option>
              <option value="active">Active</option>
              <option value="paused">Paused</option>
              <option value="draft">Draft</option>
            </select>
          </div>
        </div>
        <div class="p-6">
          <div class="space-y-4">
            <div v-for="automation in automations" :key="automation.id" class="border rounded-lg p-4">
              <div class="flex items-start justify-between">
                <div class="flex items-start space-x-4 flex-1">
                  <div :class="`p-3 rounded-lg ${automation.iconColor}`">
                    <component :is="automation.icon" class="h-6 w-6" />
                  </div>
                  <div class="flex-1">
                    <div class="flex items-center space-x-3 mb-2">
                      <h4 class="text-lg font-medium">{{ automation.name }}</h4>
                      <span class="px-2 py-1 rounded text-xs font-medium" :class="getStatusColor(automation.status)">
                        {{ automation.status }}
                      </span>
                    </div>
                    <p class="text-muted-foreground text-sm mb-3">{{ automation.description }}</p>
                    
                    <!-- Automation Flow -->
                    <div class="bg-muted/50 rounded-lg p-3 mb-4">
                      <p class="text-sm font-medium mb-2">Automation Flow:</p>
                      <div class="flex items-center space-x-2 text-sm">
                        <span class="px-2 py-1 bg-blue-100 text-blue-800 rounded">{{ automation.trigger }}</span>
                        <ArrowRight class="h-4 w-4 text-muted-foreground" />
                        <span class="px-2 py-1 bg-green-100 text-green-800 rounded">{{ automation.action }}</span>
                      </div>
                    </div>

                    <!-- Performance Metrics -->
                    <div class="grid grid-cols-3 gap-4 mb-4">
                      <div class="text-center">
                        <p class="text-lg font-bold text-blue-600">{{ automation.metrics.executions }}</p>
                        <p class="text-xs text-muted-foreground">Executions</p>
                      </div>
                      <div class="text-center">
                        <p class="text-lg font-bold text-green-600">{{ automation.metrics.successRate }}%</p>
                        <p class="text-xs text-muted-foreground">Success Rate</p>
                      </div>
                      <div class="text-center">
                        <p class="text-lg font-bold text-purple-600">{{ automation.metrics.timeSaved }}h</p>
                        <p class="text-xs text-muted-foreground">Time Saved</p>
                      </div>
                    </div>

                    <div class="flex items-center space-x-4 text-sm text-muted-foreground">
                      <span>Created: {{ automation.created }}</span>
                      <span>Last run: {{ automation.lastRun }}</span>
                    </div>
                  </div>
                </div>
                
                <!-- Action Buttons -->
                <div class="flex items-center space-x-2 ml-4">
                  <Button size="sm" variant="outline">
                    <Eye class="h-4 w-4" />
                  </Button>
                  <Button size="sm" variant="outline">
                    <Edit class="h-4 w-4" />
                  </Button>
                  <Button size="sm" variant="outline" v-if="automation.status === 'active'">
                    <Pause class="h-4 w-4" />
                  </Button>
                  <Button size="sm" variant="outline" v-else-if="automation.status === 'paused'">
                    <Play class="h-4 w-4" />
                  </Button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Automation Analytics -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Performance Chart -->
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <h3 class="text-lg font-semibold mb-4">Automation Performance</h3>
          <div class="h-48 flex items-center justify-center border rounded-lg bg-gray-50 dark:bg-gray-700">
            <p class="text-muted-foreground">Performance chart would go here</p>
          </div>
        </div>

        <!-- Recent Activity -->
        <div class="bg-white dark:bg-gray-800 p-6 rounded-lg border">
          <h3 class="text-lg font-semibold mb-4">Recent Activity</h3>
          <div class="space-y-3">
            <div v-for="activity in recentActivity" :key="activity.id" class="flex items-start space-x-3 p-3 bg-muted/30 rounded-lg">
              <div class="w-8 h-8 bg-green-100 dark:bg-green-900 rounded-lg flex items-center justify-center flex-shrink-0">
                <CheckCircle class="h-4 w-4 text-green-600" />
              </div>
              <div class="flex-1">
                <p class="font-medium text-sm">{{ activity.automation }}</p>
                <p class="text-xs text-muted-foreground">{{ activity.description }}</p>
                <p class="text-xs text-green-600 mt-1">{{ activity.timestamp }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </AppLayout>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { 
  Plus,
  FileText,
  Zap,
  CheckCircle,
  Clock,
  TrendingUp,
  Eye,
  Edit,
  Pause,
  Play,
  ArrowRight,
  Package,
  MessageSquare,
  DollarSign,
  Users,
  ShoppingCart,
  Bell,
  BarChart
} from 'lucide-vue-next'

// Reactive data
const activeAutomations = ref(7)
const tasksCompleted = ref(342)
const timeSaved = ref(18)
const successRate = ref(94)

// Automation templates
const templates = ref([
  {
    id: 1,
    name: 'Stock Reorder Alerts',
    description: 'Automatically send WhatsApp alerts when inventory reaches minimum levels',
    category: 'Inventory',
    icon: Package,
    color: 'bg-blue-100 text-blue-600',
    setupTime: '5 min',
    popularity: '120+'
  },
  {
    id: 2,
    name: 'Customer Follow-up',
    description: 'Send personalized messages to customers who haven\'t visited in 30 days',
    category: 'CRM',
    icon: MessageSquare,
    color: 'bg-green-100 text-green-600',
    setupTime: '3 min',
    popularity: '85+'
  },
  {
    id: 3,
    name: 'Daily Sales Report',
    description: 'Automatically generate and send daily sales summaries via WhatsApp',
    category: 'Reports',
    icon: BarChart,
    color: 'bg-purple-100 text-purple-600',
    setupTime: '4 min',
    popularity: '200+'
  },
  {
    id: 4,
    name: 'Payment Reminders',
    description: 'Send gentle reminders to customers with outstanding credit balances',
    category: 'Finance',
    icon: DollarSign,
    color: 'bg-yellow-100 text-yellow-600',
    setupTime: '6 min',
    popularity: '95+'
  },
  {
    id: 5,
    name: 'New Customer Welcome',
    description: 'Automatically welcome new customers and share your business info',
    category: 'CRM',
    icon: Users,
    color: 'bg-pink-100 text-pink-600',
    setupTime: '2 min',
    popularity: '150+'
  },
  {
    id: 6,
    name: 'Order Confirmation',
    description: 'Send order confirmations and delivery updates via SMS/WhatsApp',
    category: 'Sales',
    icon: ShoppingCart,
    color: 'bg-indigo-100 text-indigo-600',
    setupTime: '4 min',
    popularity: '75+'
  }
])

// Active automations
const automations = ref([
  {
    id: 1,
    name: 'Low Stock Alert System',
    description: 'Monitors inventory levels and sends WhatsApp alerts when items need reordering',
    status: 'active',
    trigger: 'Stock < Min Level',
    action: 'Send WhatsApp Alert',
    icon: Package,
    iconColor: 'bg-blue-100 text-blue-600',
    created: '2024-01-10',
    lastRun: '2 hours ago',
    metrics: {
      executions: 45,
      successRate: 98,
      timeSaved: 8
    }
  },
  {
    id: 2,
    name: 'Daily Sales Summary',
    description: 'Generates and sends daily sales reports to your phone at 6 PM',
    status: 'active',
    trigger: 'Daily at 6 PM',
    action: 'Generate & Send Report',
    icon: BarChart,
    iconColor: 'bg-purple-100 text-purple-600',
    created: '2024-01-08',
    lastRun: 'Yesterday',
    metrics: {
      executions: 15,
      successRate: 100,
      timeSaved: 5
    }
  },
  {
    id: 3,
    name: 'Customer Win-Back Campaign',
    description: 'Automatically reaches out to customers who haven\'t purchased in 30 days',
    status: 'paused',
    trigger: 'Customer Inactive 30d',
    action: 'Send WhatsApp Message',
    icon: MessageSquare,
    iconColor: 'bg-green-100 text-green-600',
    created: '2024-01-05',
    lastRun: '1 week ago',
    metrics: {
      executions: 8,
      successRate: 87,
      timeSaved: 3
    }
  },
  {
    id: 4,
    name: 'Credit Payment Reminders',
    description: 'Sends gentle reminders for overdue credit accounts via SMS',
    status: 'active',
    trigger: 'Payment Overdue 7d',
    action: 'Send SMS Reminder',
    icon: DollarSign,
    iconColor: 'bg-yellow-100 text-yellow-600',
    created: '2024-01-12',
    lastRun: '3 days ago',
    metrics: {
      executions: 12,
      successRate: 92,
      timeSaved: 2
    }
  }
])

// Recent activity
const recentActivity = ref([
  {
    id: 1,
    automation: 'Low Stock Alert System',
    description: 'Sent WhatsApp alert for Maize Meal 2.5kg (8 units remaining)',
    timestamp: '2 hours ago'
  },
  {
    id: 2,
    automation: 'Daily Sales Summary',
    description: 'Generated sales report - R 2,450 revenue, 18 transactions',
    timestamp: 'Yesterday 6:00 PM'
  },
  {
    id: 3,
    automation: 'Credit Payment Reminders',
    description: 'Sent 3 SMS reminders to customers with overdue payments',
    timestamp: '3 days ago'
  },
  {
    id: 4,
    automation: 'Low Stock Alert System',
    description: 'Sent WhatsApp alert for Cooking Oil 750ml (12 units remaining)',
    timestamp: '5 days ago'
  }
])

// Methods
const getStatusColor = (status: string) => {
  const colors = {
    'active': 'bg-green-100 text-green-800',
    'paused': 'bg-yellow-100 text-yellow-800',
    'draft': 'bg-gray-100 text-gray-800',
    'error': 'bg-red-100 text-red-800'
  }
  return colors[status as keyof typeof colors] || 'bg-gray-100 text-gray-800'
}

const setupTemplate = (template: any) => {
  // Would open template setup dialog
  console.log('Setting up template:', template.name)
}
</script>