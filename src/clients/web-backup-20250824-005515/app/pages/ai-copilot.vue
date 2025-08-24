<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
            AI Business Co-Pilot
          </h1>
          <p class="mt-2 text-gray-600 dark:text-gray-300">
            Your intelligent assistant for business operations and decision-making
          </p>
        </div>
        <div class="flex space-x-2">
          <UButton
            variant="outline"
            icon="i-heroicons-microphone"
            @click="toggleVoiceMode"
          >
            {{ voiceMode ? 'Disable Voice' : 'Enable Voice' }}
          </UButton>
          <UButton
            icon="i-heroicons-cog-6-tooth"
            @click="showSettings = true"
          >
            Settings
          </UButton>
        </div>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div class="flex items-center mb-4">
          <div class="w-8 h-8 bg-blue-100 dark:bg-blue-900 rounded-md flex items-center justify-center mr-3">
            <UIcon name="i-heroicons-chart-bar" class="w-5 h-5 text-blue-600 dark:text-blue-400" />
          </div>
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Analytics</h3>
        </div>
        <p class="text-sm text-gray-600 dark:text-gray-300 mb-4">
          Get insights and recommendations based on your business data
        </p>
        <UButton size="sm" @click="startAnalyticsQuery">Ask About Performance</UButton>
      </div>

      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div class="flex items-center mb-4">
          <div class="w-8 h-8 bg-green-100 dark:bg-green-900 rounded-md flex items-center justify-center mr-3">
            <UIcon name="i-heroicons-cog" class="w-5 h-5 text-green-600 dark:text-green-400" />
          </div>
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Automation</h3>
        </div>
        <p class="text-sm text-gray-600 dark:text-gray-300 mb-4">
          Automate routine tasks and workflows with AI assistance
        </p>
        <UButton size="sm" @click="showAutomations = true">Manage Workflows</UButton>
      </div>

      <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
        <div class="flex items-center mb-4">
          <div class="w-8 h-8 bg-purple-100 dark:bg-purple-900 rounded-md flex items-center justify-center mr-3">
            <UIcon name="i-heroicons-light-bulb" class="w-5 h-5 text-purple-600 dark:text-purple-400" />
          </div>
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Insights</h3>
        </div>
        <p class="text-sm text-gray-600 dark:text-gray-300 mb-4">
          Discover trends and opportunities in your business data
        </p>
        <UButton size="sm" @click="generateInsights">Generate Report</UButton>
      </div>
    </div>

    <!-- Chat Interface -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <div class="flex items-center justify-between mb-4">
        <h2 class="text-lg font-semibold text-gray-900 dark:text-white">
          Chat with Co-Pilot
        </h2>
        <UButton size="sm" variant="ghost" @click="clearChat">
          Clear Chat
        </UButton>
      </div>

      <!-- Chat Messages -->
      <div class="h-96 overflow-y-auto border border-gray-200 dark:border-gray-700 rounded-lg p-4 mb-4 space-y-4">
        <div
          v-for="message in chatMessages"
          :key="message.id"
          :class="[
            'flex',
            message.sender === 'user' ? 'justify-end' : 'justify-start'
          ]"
        >
          <div
            :class="[
              'max-w-xs lg:max-w-md px-4 py-2 rounded-lg',
              message.sender === 'user'
                ? 'bg-blue-500 text-white'
                : 'bg-gray-100 dark:bg-gray-700 text-gray-900 dark:text-white'
            ]"
          >
            <p class="text-sm">{{ message.content }}</p>
            <p class="text-xs mt-1 opacity-75">{{ formatTime(message.timestamp) }}</p>
          </div>
        </div>
      </div>

      <!-- Chat Input -->
      <div class="flex space-x-2">
        <UInput
          v-model="currentMessage"
          placeholder="Ask your AI co-pilot anything..."
          class="flex-1"
          @keydown.enter="sendMessage"
        />
        <UButton
          icon="i-heroicons-paper-airplane"
          @click="sendMessage"
          :disabled="!currentMessage.trim()"
        />
        <UButton
          v-if="voiceMode"
          icon="i-heroicons-microphone"
          variant="outline"
          @click="startVoiceInput"
          :color="isListening ? 'red' : 'gray'"
        />
      </div>
    </div>

    <!-- AI Recommendations -->
    <div class="bg-white dark:bg-gray-800 shadow rounded-lg p-6">
      <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">
        AI Recommendations
      </h2>
      
      <div class="space-y-4">
        <div
          v-for="recommendation in aiRecommendations"
          :key="recommendation.id"
          class="border border-gray-200 dark:border-gray-700 rounded-lg p-4"
        >
          <div class="flex items-start justify-between mb-2">
            <div class="flex items-start">
              <div class="w-6 h-6 bg-yellow-100 dark:bg-yellow-900 rounded-md flex items-center justify-center mr-3 mt-1">
                <UIcon name="i-heroicons-light-bulb" class="w-4 h-4 text-yellow-600 dark:text-yellow-400" />
              </div>
              <div>
                <h3 class="font-medium text-gray-900 dark:text-white">
                  {{ recommendation.title }}
                </h3>
                <p class="text-sm text-gray-600 dark:text-gray-300 mt-1">
                  {{ recommendation.description }}
                </p>
              </div>
            </div>
            <UBadge
              :color="getPriorityColor(recommendation.priority)"
              variant="soft"
            >
              {{ recommendation.priority }}
            </UBadge>
          </div>
          
          <div class="ml-9">
            <p class="text-sm text-gray-500 dark:text-gray-400 mb-3">
              Impact: {{ recommendation.impact }}
            </p>
            <div class="flex space-x-2">
              <UButton size="sm" @click="implementRecommendation(recommendation.id)">
                Implement
              </UButton>
              <UButton size="sm" variant="outline" @click="learnMore(recommendation.id)">
                Learn More
              </UButton>
              <UButton size="sm" variant="ghost" @click="dismissRecommendation(recommendation.id)">
                Dismiss
              </UButton>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Settings Modal -->
    <UModal v-model="showSettings">
      <UCard>
        <template #header>
          <h3 class="text-lg font-semibold">AI Co-Pilot Settings</h3>
        </template>

        <div class="space-y-4">
          <UFormGroup label="Preferred Response Style">
            <USelect
              v-model="settings.responseStyle"
              :options="responseStyleOptions"
              placeholder="Select style"
            />
          </UFormGroup>

          <UFormGroup label="Data Access Level">
            <USelect
              v-model="settings.dataAccess"
              :options="dataAccessOptions"
              placeholder="Select access level"
            />
          </UFormGroup>

          <UFormGroup label="Notification Preferences">
            <div class="space-y-2">
              <UCheckbox v-model="settings.notifications.insights" label="AI Insights" />
              <UCheckbox v-model="settings.notifications.recommendations" label="Recommendations" />
              <UCheckbox v-model="settings.notifications.alerts" label="Business Alerts" />
            </div>
          </UFormGroup>

          <UFormGroup label="Voice Settings">
            <div class="space-y-2">
              <UCheckbox v-model="settings.voice.enabled" label="Enable Voice Commands" />
              <UCheckbox v-model="settings.voice.autoResponse" label="Voice Responses" />
            </div>
          </UFormGroup>
        </div>

        <template #footer>
          <div class="flex justify-end space-x-2">
            <UButton variant="ghost" @click="showSettings = false">Cancel</UButton>
            <UButton @click="saveSettings">Save Settings</UButton>
          </div>
        </template>
      </UCard>
    </UModal>

    <!-- Automations Modal -->
    <UModal v-model="showAutomations">
      <UCard>
        <template #header>
          <h3 class="text-lg font-semibold">Workflow Automation</h3>
        </template>

        <div class="space-y-4">
          <div
            v-for="automation in automations"
            :key="automation.id"
            class="border border-gray-200 dark:border-gray-700 rounded-lg p-4"
          >
            <div class="flex items-center justify-between mb-2">
              <h4 class="font-medium text-gray-900 dark:text-white">
                {{ automation.name }}
              </h4>
              <UToggle v-model="automation.enabled" />
            </div>
            <p class="text-sm text-gray-600 dark:text-gray-300">
              {{ automation.description }}
            </p>
          </div>
        </div>

        <template #footer>
          <div class="flex justify-end space-x-2">
            <UButton variant="ghost" @click="showAutomations = false">Close</UButton>
            <UButton @click="createAutomation">Create New</UButton>
          </div>
        </template>
      </UCard>
    </UModal>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

// Page meta
definePageMeta({
  title: 'AI Co-Pilot',
  description: 'AI-powered business assistant'
})

// Reactive data
const voiceMode = ref(false)
const isListening = ref(false)
const showSettings = ref(false)
const showAutomations = ref(false)
const currentMessage = ref('')

const settings = ref({
  responseStyle: 'conversational',
  dataAccess: 'full',
  notifications: {
    insights: true,
    recommendations: true,
    alerts: false
  },
  voice: {
    enabled: false,
    autoResponse: false
  }
})

const responseStyleOptions = [
  { value: 'conversational', label: 'Conversational' },
  { value: 'formal', label: 'Formal' },
  { value: 'technical', label: 'Technical' },
  { value: 'brief', label: 'Brief' }
]

const dataAccessOptions = [
  { value: 'full', label: 'Full Access' },
  { value: 'limited', label: 'Limited Access' },
  { value: 'basic', label: 'Basic Only' }
]

const chatMessages = ref([
  {
    id: 1,
    sender: 'ai',
    content: 'Hello! I\'m your AI business co-pilot. How can I help you today?',
    timestamp: new Date()
  }
])

const aiRecommendations = ref([
  {
    id: 1,
    title: 'Optimize Inventory Levels',
    description: 'Your inventory turnover rate has decreased by 15%. Consider adjusting stock levels for better cash flow.',
    priority: 'High',
    impact: 'Potential 20% improvement in cash flow'
  },
  {
    id: 2,
    title: 'Automate Invoice Follow-ups',
    description: 'Set up automated reminders for overdue invoices to improve collection times.',
    priority: 'Medium',
    impact: 'Reduce average collection time by 5-7 days'
  }
])

const automations = ref([
  {
    id: 1,
    name: 'Invoice Processing',
    description: 'Automatically extract data from invoices and create entries',
    enabled: true
  },
  {
    id: 2,
    name: 'Expense Categorization',
    description: 'Auto-categorize expenses based on descriptions and vendors',
    enabled: false
  }
])

// Methods
const toggleVoiceMode = () => {
  voiceMode.value = !voiceMode.value
}

const startVoiceInput = () => {
  isListening.value = !isListening.value
  // Voice input logic would go here
}

const sendMessage = () => {
  if (!currentMessage.value.trim()) return
  
  chatMessages.value.push({
    id: chatMessages.value.length + 1,
    sender: 'user',
    content: currentMessage.value,
    timestamp: new Date()
  })
  
  // Simulate AI response
  setTimeout(() => {
    chatMessages.value.push({
      id: chatMessages.value.length + 1,
      sender: 'ai',
      content: 'I understand your question. Let me analyze your data and provide insights.',
      timestamp: new Date()
    })
  }, 1000)
  
  currentMessage.value = ''
}

const clearChat = () => {
  chatMessages.value = [
    {
      id: 1,
      sender: 'ai',
      content: 'Hello! I\'m your AI business co-pilot. How can I help you today?',
      timestamp: new Date()
    }
  ]
}

const formatTime = (date: Date): string => {
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

const getPriorityColor = (priority: string): string => {
  const colors: Record<string, string> = {
    'High': 'red',
    'Medium': 'yellow',
    'Low': 'green'
  }
  return colors[priority] || 'gray'
}

const startAnalyticsQuery = () => {
  currentMessage.value = 'Show me this month\'s performance analytics'
  sendMessage()
}

const generateInsights = () => {
  currentMessage.value = 'Generate business insights from recent data'
  sendMessage()
}

const implementRecommendation = (id: number) => {
  console.log('Implementing recommendation:', id)
}

const learnMore = (id: number) => {
  console.log('Learning more about recommendation:', id)
}

const dismissRecommendation = (id: number) => {
  const index = aiRecommendations.value.findIndex(r => r.id === id)
  if (index > -1) {
    aiRecommendations.value.splice(index, 1)
  }
}

const saveSettings = () => {
  console.log('Saving settings:', settings.value)
  showSettings.value = false
}

const createAutomation = () => {
  console.log('Creating new automation')
}
</script>
