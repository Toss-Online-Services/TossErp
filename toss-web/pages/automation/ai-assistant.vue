<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="border-b border-gray-200 dark:border-gray-700 pb-4">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">AI Assistant</h1>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            Intelligent automation recommendations and workflow optimization
          </p>
        </div>
        <div class="flex space-x-3">
          <button class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600">
            <ChartBarIcon class="w-4 h-4 mr-2" />
            View Analytics
          </button>
          <button class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-lg text-white bg-gradient-to-r from-blue-600 to-purple-600 hover:from-blue-700 hover:to-purple-700">
            <SparklesIcon class="w-4 h-4 mr-2" />
            Generate Workflow
          </button>
        </div>
      </div>
    </div>

    <!-- AI Insights Dashboard -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- AI Chat Interface -->
      <div class="lg:col-span-2">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 h-96">
          <div class="flex items-center justify-between p-4 border-b border-gray-200 dark:border-gray-700">
            <div class="flex items-center">
              <div class="w-8 h-8 bg-gradient-to-r from-blue-500 to-purple-500 rounded-full flex items-center justify-center mr-3">
                <SparklesIcon class="w-4 h-4 text-white" />
              </div>
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">TOSS AI Assistant</h3>
              <span class="ml-2 px-2 py-1 text-xs bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200 rounded-full">
                Online
              </span>
            </div>
            <button class="text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300">
              <ArrowsPointingOutIcon class="w-5 h-5" />
            </button>
          </div>
          
          <div class="flex-1 p-4 space-y-4 h-64 overflow-y-auto">
            <div v-for="message in chatMessages" :key="message.id" class="flex" :class="message.type === 'user' ? 'justify-end' : 'justify-start'">
              <div class="max-w-xs lg:max-w-md" :class="message.type === 'user' ? 'order-2' : 'order-1'">
                <div class="px-4 py-2 rounded-lg" :class="message.type === 'user' ? 'bg-blue-600 text-white' : 'bg-gray-100 dark:bg-gray-700 text-gray-900 dark:text-white'">
                  <p class="text-sm">{{ message.content }}</p>
                </div>
                <p class="text-xs text-gray-500 dark:text-gray-400 mt-1" :class="message.type === 'user' ? 'text-right' : 'text-left'">
                  {{ message.timestamp }}
                </p>
              </div>
            </div>
          </div>
          
          <div class="p-4 border-t border-gray-200 dark:border-gray-700">
            <div class="flex space-x-2">
              <input v-model="newMessage" @keyup.enter="sendMessage" type="text" placeholder="Ask AI for automation suggestions..." 
                     class="flex-1 px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white placeholder-gray-500 dark:placeholder-gray-400">
              <button @click="sendMessage" class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
                <PaperAirplaneIcon class="w-4 h-4" />
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- AI Recommendations Panel -->
      <div class="space-y-6">
        <!-- Quick Actions -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Quick Actions</h3>
          <div class="space-y-2">
            <button v-for="action in quickActions" :key="action.id" 
                    class="w-full text-left p-3 rounded-lg border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
                    @click="executeQuickAction(action)">
              <div class="flex items-center">
                <div class="w-8 h-8 rounded-lg flex items-center justify-center mr-3" :class="action.iconBg">
                  <component :is="action.icon" class="w-4 h-4" :class="action.iconColor" />
                </div>
                <div class="flex-1">
                  <p class="text-sm font-medium text-gray-900 dark:text-white">{{ action.title }}</p>
                  <p class="text-xs text-gray-500 dark:text-gray-400">{{ action.description }}</p>
                </div>
              </div>
            </button>
          </div>
        </div>

        <!-- AI Metrics -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-4">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">AI Performance</h3>
          <div class="space-y-4">
            <div>
              <div class="flex items-center justify-between mb-2">
                <span class="text-sm text-gray-600 dark:text-gray-400">Accuracy Rate</span>
                <span class="text-sm font-medium text-gray-900 dark:text-white">{{ aiMetrics.accuracy }}%</span>
              </div>
              <div class="w-full bg-gray-200 dark:bg-gray-700 rounded-full h-2">
                <div class="bg-green-500 h-2 rounded-full transition-all duration-300" :style="{ width: aiMetrics.accuracy + '%' }"></div>
              </div>
            </div>
            
            <div>
              <div class="flex items-center justify-between mb-2">
                <span class="text-sm text-gray-600 dark:text-gray-400">Optimization Score</span>
                <span class="text-sm font-medium text-gray-900 dark:text-white">{{ aiMetrics.optimization }}%</span>
              </div>
              <div class="w-full bg-gray-200 dark:bg-gray-700 rounded-full h-2">
                <div class="bg-blue-500 h-2 rounded-full transition-all duration-300" :style="{ width: aiMetrics.optimization + '%' }"></div>
              </div>
            </div>
            
            <div>
              <div class="flex items-center justify-between mb-2">
                <span class="text-sm text-gray-600 dark:text-gray-400">Learning Progress</span>
                <span class="text-sm font-medium text-gray-900 dark:text-white">{{ aiMetrics.learning }}%</span>
              </div>
              <div class="w-full bg-gray-200 dark:bg-gray-700 rounded-full h-2">
                <div class="bg-purple-500 h-2 rounded-full transition-all duration-300" :style="{ width: aiMetrics.learning + '%' }"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- AI Recommendations -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
      <div class="flex items-center justify-between mb-6">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white">AI Recommendations</h3>
        <button class="text-sm text-blue-600 dark:text-blue-400 hover:text-blue-700">
          View All Suggestions
        </button>
      </div>
      
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        <div v-for="recommendation in aiRecommendations" :key="recommendation.id" 
             class="p-4 border border-gray-200 dark:border-gray-600 rounded-lg hover:shadow-md transition-shadow">
          <div class="flex items-start">
            <div class="w-10 h-10 rounded-lg flex items-center justify-center mr-3" :class="recommendation.iconBg">
              <component :is="recommendation.icon" class="w-5 h-5" :class="recommendation.iconColor" />
            </div>
            <div class="flex-1">
              <div class="flex items-center justify-between mb-2">
                <h4 class="font-medium text-gray-900 dark:text-white">{{ recommendation.title }}</h4>
                <span class="text-xs px-2 py-1 rounded-full" :class="getPriorityClass(recommendation.priority)">
                  {{ recommendation.priority }}
                </span>
              </div>
              <p class="text-sm text-gray-600 dark:text-gray-400 mb-3">{{ recommendation.description }}</p>
              <div class="flex items-center justify-between">
                <div class="flex items-center text-xs text-gray-500 dark:text-gray-400">
                  <ClockIcon class="w-3 h-3 mr-1" />
                  {{ recommendation.timeToImplement }}
                </div>
                <div class="flex items-center text-xs text-green-600 dark:text-green-400">
                  <ArrowTrendingUpIcon class="w-3 h-3 mr-1" />
                  {{ recommendation.expectedImpact }}
                </div>
              </div>
              <div class="mt-3 flex space-x-2">
                <button class="flex-1 px-3 py-1 text-xs bg-blue-600 text-white rounded hover:bg-blue-700">
                  Implement
                </button>
                <button class="px-3 py-1 text-xs border border-gray-300 dark:border-gray-600 text-gray-700 dark:text-gray-300 rounded hover:bg-gray-50 dark:hover:bg-gray-700">
                  Learn More
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Process Analysis -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
      <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-6">Process Analysis & Optimization</h3>
      
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Inefficiency Detection -->
        <div>
          <h4 class="font-medium text-gray-900 dark:text-white mb-4">Detected Inefficiencies</h4>
          <div class="space-y-3">
            <div v-for="inefficiency in processInefficiencies" :key="inefficiency.id" 
                 class="p-3 border border-gray-200 dark:border-gray-600 rounded-lg">
              <div class="flex items-center justify-between mb-2">
                <span class="text-sm font-medium text-gray-900 dark:text-white">{{ inefficiency.process }}</span>
                <span class="text-xs px-2 py-1 rounded-full" :class="getSeverityClass(inefficiency.severity)">
                  {{ inefficiency.severity }}
                </span>
              </div>
              <p class="text-sm text-gray-600 dark:text-gray-400 mb-2">{{ inefficiency.description }}</p>
              <div class="flex items-center justify-between text-xs text-gray-500 dark:text-gray-400">
                <span>Impact: {{ inefficiency.impact }}</span>
                <span>Frequency: {{ inefficiency.frequency }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Optimization Opportunities -->
        <div>
          <h4 class="font-medium text-gray-900 dark:text-white mb-4">Optimization Opportunities</h4>
          <div class="space-y-3">
            <div v-for="opportunity in optimizationOpportunities" :key="opportunity.id" 
                 class="p-3 border border-gray-200 dark:border-gray-600 rounded-lg">
              <div class="flex items-center justify-between mb-2">
                <span class="text-sm font-medium text-gray-900 dark:text-white">{{ opportunity.area }}</span>
                <span class="text-xs text-green-600 dark:text-green-400">{{ opportunity.potentialSavings }}</span>
              </div>
              <p class="text-sm text-gray-600 dark:text-gray-400 mb-2">{{ opportunity.description }}</p>
              <div class="flex items-center justify-between text-xs text-gray-500 dark:text-gray-400">
                <span>Effort: {{ opportunity.implementationEffort }}</span>
                <span>ROI: {{ opportunity.expectedROI }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import {
  ChartBarIcon,
  SparklesIcon,
  ArrowsPointingOutIcon,
  PaperAirplaneIcon,
  ClockIcon,
  ArrowTrendingUpIcon,
  LightBulbIcon,
  CogIcon,
  RocketLaunchIcon,
  ExclamationTriangleIcon,
  ShieldCheckIcon,
  BeakerIcon
} from '@heroicons/vue/24/outline'

// Reactive data
const newMessage = ref('')

// Chat messages
const chatMessages = ref([
  {
    id: 1,
    type: 'assistant',
    content: 'Hello! I\'m your AI automation assistant. I can help you optimize workflows, identify inefficiencies, and suggest new automation opportunities. What would you like to work on today?',
    timestamp: '10:30 AM'
  },
  {
    id: 2,
    type: 'user',
    content: 'Can you analyze our customer onboarding process?',
    timestamp: '10:32 AM'
  },
  {
    id: 3,
    type: 'assistant',
    content: 'I\'ve analyzed your customer onboarding process. I found 3 potential optimizations that could reduce onboarding time by 40% and improve customer satisfaction. Would you like me to create automated workflows for these improvements?',
    timestamp: '10:33 AM'
  }
])

// Quick actions
const quickActions = ref([
  {
    id: 1,
    title: 'Analyze Workflows',
    description: 'Review all active workflows for optimization',
    icon: ChartBarIcon,
    iconBg: 'bg-blue-100 dark:bg-blue-900',
    iconColor: 'text-blue-600 dark:text-blue-400'
  },
  {
    id: 2,
    title: 'Suggest Automations',
    description: 'Find new automation opportunities',
    icon: LightBulbIcon,
    iconBg: 'bg-yellow-100 dark:bg-yellow-900',
    iconColor: 'text-yellow-600 dark:text-yellow-400'
  },
  {
    id: 3,
    title: 'Optimize Performance',
    description: 'Improve existing workflow efficiency',
    icon: CogIcon,
    iconBg: 'bg-green-100 dark:bg-green-900',
    iconColor: 'text-green-600 dark:text-green-400'
  },
  {
    id: 4,
    title: 'Generate Report',
    description: 'Create automation performance report',
    icon: RocketLaunchIcon,
    iconBg: 'bg-purple-100 dark:bg-purple-900',
    iconColor: 'text-purple-600 dark:text-purple-400'
  }
])

// AI metrics
const aiMetrics = ref({
  accuracy: 94,
  optimization: 87,
  learning: 76
})

// AI recommendations
const aiRecommendations = ref([
  {
    id: 1,
    title: 'Automate Lead Qualification',
    description: 'Use ML to automatically score and qualify incoming leads based on historical data',
    priority: 'High',
    timeToImplement: '2 days',
    expectedImpact: '+25% efficiency',
    icon: LightBulbIcon,
    iconBg: 'bg-blue-100 dark:bg-blue-900',
    iconColor: 'text-blue-600 dark:text-blue-400'
  },
  {
    id: 2,
    title: 'Smart Inventory Reordering',
    description: 'Implement predictive analytics for optimal stock level management',
    priority: 'Medium',
    timeToImplement: '3 days',
    expectedImpact: '+15% cost savings',
    icon: CogIcon,
    iconBg: 'bg-green-100 dark:bg-green-900',
    iconColor: 'text-green-600 dark:text-green-400'
  },
  {
    id: 3,
    title: 'Customer Support Triage',
    description: 'Auto-categorize and route support tickets to appropriate teams',
    priority: 'High',
    timeToImplement: '1 day',
    expectedImpact: '+40% response time',
    icon: ShieldCheckIcon,
    iconBg: 'bg-purple-100 dark:bg-purple-900',
    iconColor: 'text-purple-600 dark:text-purple-400'
  },
  {
    id: 4,
    title: 'A/B Test Automation',
    description: 'Automatically run and analyze A/B tests for email campaigns',
    priority: 'Low',
    timeToImplement: '5 days',
    expectedImpact: '+20% conversion',
    icon: BeakerIcon,
    iconBg: 'bg-orange-100 dark:bg-orange-900',
    iconColor: 'text-orange-600 dark:text-orange-400'
  }
])

// Process inefficiencies
const processInefficiencies = ref([
  {
    id: 1,
    process: 'Invoice Processing',
    description: 'Manual approval steps causing 2-day delays',
    severity: 'High',
    impact: 'High',
    frequency: 'Daily'
  },
  {
    id: 2,
    process: 'Customer Data Entry',
    description: 'Duplicate data entry across 3 systems',
    severity: 'Medium',
    impact: 'Medium',
    frequency: 'Weekly'
  },
  {
    id: 3,
    process: 'Report Generation',
    description: 'Manual compilation of 15 different reports',
    severity: 'Low',
    impact: 'Low',
    frequency: 'Monthly'
  }
])

// Optimization opportunities
const optimizationOpportunities = ref([
  {
    id: 1,
    area: 'Email Marketing',
    description: 'Implement smart send-time optimization',
    potentialSavings: '40% time saved',
    implementationEffort: 'Low',
    expectedROI: '300%'
  },
  {
    id: 2,
    area: 'Inventory Management',
    description: 'Add automated reorder point calculations',
    potentialSavings: '25% cost reduction',
    implementationEffort: 'Medium',
    expectedROI: '250%'
  },
  {
    id: 3,
    area: 'Customer Onboarding',
    description: 'Create progressive profiling workflows',
    potentialSavings: '60% faster onboarding',
    implementationEffort: 'High',
    expectedROI: '180%'
  }
])

// Methods
const sendMessage = () => {
  if (newMessage.value.trim()) {
    const userMessage = {
      id: chatMessages.value.length + 1,
      type: 'user',
      content: newMessage.value,
      timestamp: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
    }
    chatMessages.value.push(userMessage)
    
    // Simulate AI response
    setTimeout(() => {
      const aiResponse = {
        id: chatMessages.value.length + 1,
        type: 'assistant',
        content: 'I understand your request. Let me analyze that for you and provide some recommendations...',
        timestamp: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
      }
      chatMessages.value.push(aiResponse)
    }, 1000)
    
    newMessage.value = ''
  }
}

const executeQuickAction = (action: any) => {
  // Simulate quick action execution
  console.log('Executing:', action.title)
}

const getPriorityClass = (priority: string) => {
  const classes = {
    'High': 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    'Medium': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    'Low': 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
  }
  return classes[priority as keyof typeof classes] || classes.Low
}

const getSeverityClass = (severity: string) => {
  const classes = {
    'High': 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    'Medium': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    'Low': 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
  }
  return classes[severity as keyof typeof classes] || classes.Low
}
</script>
