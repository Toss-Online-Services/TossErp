<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <!-- Mobile-First Page Container -->
    <div class="p-4 sm:p-6 space-y-4 sm:space-y-6 pb-20 lg:pb-6">
      <!-- Page Header -->
      <div class="text-center sm:text-left">
        <h1 class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">CRM Automation</h1>
        <p class="text-slate-600 dark:text-slate-400 mt-1 text-sm sm:text-base">
          Automate your customer relationships for township enterprises
        </p>
      </div>

      <!-- Automation Stats Cards -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-4">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Active Workflows</p>
              <p class="text-2xl font-bold text-slate-900 dark:text-white">{{ activeWorkflowCount }}</p>
            </div>
            <div class="h-8 w-8 bg-blue-100 dark:bg-blue-900/20 rounded-lg flex items-center justify-center">
              <CogIcon class="h-5 w-5 text-blue-600 dark:text-blue-400" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-4">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Executions Today</p>
              <p class="text-2xl font-bold text-slate-900 dark:text-white">{{ executionsToday }}</p>
            </div>
            <div class="h-8 w-8 bg-green-100 dark:bg-green-900/20 rounded-lg flex items-center justify-center">
              <PlayIcon class="h-5 w-5 text-green-600 dark:text-green-400" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-4">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Success Rate</p>
              <p class="text-2xl font-bold text-slate-900 dark:text-white">{{ successRate }}%</p>
            </div>
            <div class="h-8 w-8 bg-purple-100 dark:bg-purple-900/20 rounded-lg flex items-center justify-center">
              <ChartBarIcon class="h-5 w-5 text-purple-600 dark:text-purple-400" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700 p-4">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400">Avg Lead Score</p>
              <p class="text-2xl font-bold text-slate-900 dark:text-white">{{ averageLeadScore }}</p>
            </div>
            <div class="h-8 w-8 bg-orange-100 dark:bg-orange-900/20 rounded-lg flex items-center justify-center">
              <StarIcon class="h-5 w-5 text-orange-600 dark:text-orange-400" />
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="flex flex-wrap gap-3">
        <button
          @click="openWorkflowBuilder"
          class="inline-flex items-center px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white text-sm font-medium rounded-lg transition-colors"
        >
          <PlusIcon class="w-4 h-4 mr-2" />
          Create Workflow
        </button>
        <button
          @click="openTemplateLibrary"
          class="inline-flex items-center px-4 py-2 bg-green-600 hover:bg-green-700 text-white text-sm font-medium rounded-lg transition-colors"
        >
          <DocumentTextIcon class="w-4 h-4 mr-2" />
          Template Library
        </button>
        <button
          @click="openLeadScoring"
          class="inline-flex items-center px-4 py-2 bg-purple-600 hover:bg-purple-700 text-white text-sm font-medium rounded-lg transition-colors"
        >
          <StarIcon class="w-4 h-4 mr-2" />
          Lead Scoring
        </button>
      </div>

      <!-- Tabs for different sections -->
      <div class="border-b border-slate-200 dark:border-slate-700">
        <nav class="-mb-px flex space-x-8">
          <button
            v-for="tab in tabs"
            :key="tab.id"
            @click="activeTab = tab.id"
            :class="[
              'py-2 px-1 border-b-2 font-medium text-sm transition-colors',
              activeTab === tab.id
                ? 'border-blue-500 text-blue-600 dark:text-blue-400'
                : 'border-transparent text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-300 hover:border-slate-300 dark:hover:border-slate-600'
            ]"
          >
            {{ tab.name }}
          </button>
        </nav>
      </div>

      <!-- Active Workflows Tab -->
      <div v-if="activeTab === 'workflows'" class="space-y-4">
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700">
            <h3 class="text-lg font-medium text-slate-900 dark:text-white">Active Workflows</h3>
          </div>
          <div class="p-6">
            <div v-if="automationWorkflows.length === 0" class="text-center py-8">
              <CogIcon class="mx-auto h-12 w-12 text-slate-400" />
              <h3 class="mt-2 text-sm font-medium text-slate-900 dark:text-white">No workflows yet</h3>
              <p class="mt-1 text-sm text-slate-500 dark:text-slate-400">Get started by creating your first automation workflow.</p>
              <div class="mt-6">
                <button
                  @click="openWorkflowBuilder"
                  class="inline-flex items-center px-4 py-2 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700"
                >
                  <PlusIcon class="-ml-1 mr-2 h-5 w-5" />
                  New Workflow
                </button>
              </div>
            </div>
            
            <div v-else class="space-y-4">
              <div
                v-for="workflow in automationWorkflows"
                :key="workflow.id"
                class="border border-slate-200 dark:border-slate-700 rounded-lg p-4"
              >
                <div class="flex items-center justify-between">
                  <div class="flex-1">
                    <div class="flex items-center space-x-3">
                      <h4 class="text-lg font-medium text-slate-900 dark:text-white">{{ workflow.name }}</h4>
                      <span
                        :class="[
                          'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
                          workflow.active
                            ? 'bg-green-100 text-green-800 dark:bg-green-900/20 dark:text-green-400'
                            : 'bg-gray-100 text-gray-800 dark:bg-gray-900/20 dark:text-gray-400'
                        ]"
                      >
                        {{ workflow.active ? 'Active' : 'Inactive' }}
                      </span>
                    </div>
                    <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">{{ workflow.description }}</p>
                    <div class="flex items-center space-x-4 mt-2 text-xs text-slate-500 dark:text-slate-400">
                      <span>{{ workflow.enterpriseTypes.length }} enterprise types</span>
                      <span>{{ workflow.stats.timesTriggered }} executions</span>
                      <span>{{ Math.round((workflow.stats.successfulExecutions / Math.max(workflow.stats.timesTriggered, 1)) * 100) }}% success rate</span>
                    </div>
                  </div>
                  <div class="flex items-center space-x-2">
                    <button
                      @click="toggleWorkflow(workflow.id)"
                      :class="[
                        'inline-flex items-center px-3 py-1 rounded-md text-sm font-medium',
                        workflow.active
                          ? 'text-red-700 bg-red-100 hover:bg-red-200 dark:bg-red-900/20 dark:text-red-400 dark:hover:bg-red-900/30'
                          : 'text-green-700 bg-green-100 hover:bg-green-200 dark:bg-green-900/20 dark:text-green-400 dark:hover:bg-green-900/30'
                      ]"
                    >
                      {{ workflow.active ? 'Pause' : 'Activate' }}
                    </button>
                    <button
                      @click="editWorkflow(workflow.id)"
                      class="inline-flex items-center px-3 py-1 rounded-md text-sm font-medium text-slate-700 bg-slate-100 hover:bg-slate-200 dark:bg-slate-700 dark:text-slate-300 dark:hover:bg-slate-600"
                    >
                      Edit
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Township Templates Tab -->
      <div v-if="activeTab === 'templates'" class="space-y-4">
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700">
            <h3 class="text-lg font-medium text-slate-900 dark:text-white">Township Enterprise Templates</h3>
            <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">
              Pre-built automation templates specifically designed for South African township enterprises
            </p>
          </div>
          <div class="p-6">
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              <div
                v-for="(template, key) in TOWNSHIP_AUTOMATION_TEMPLATES"
                :key="key"
                class="border border-slate-200 dark:border-slate-700 rounded-lg p-4 hover:border-blue-300 dark:hover:border-blue-600 transition-colors cursor-pointer"
                @click="previewTemplate(key)"
              >
                <div class="flex items-start justify-between">
                  <div class="flex-1">
                    <h4 class="text-md font-medium text-slate-900 dark:text-white">{{ template.name }}</h4>
                    <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">{{ template.description }}</p>
                    <div class="mt-3">
                      <div class="flex flex-wrap gap-1">
                        <span
                          v-for="enterpriseType in template.enterpriseTypes.slice(0, 2)"
                          :key="enterpriseType"
                          class="inline-flex items-center px-2 py-1 rounded-md text-xs font-medium bg-blue-100 text-blue-800 dark:bg-blue-900/20 dark:text-blue-400"
                        >
                          {{ enterpriseType }}
                        </span>
                        <span
                          v-if="template.enterpriseTypes.length > 2"
                          class="inline-flex items-center px-2 py-1 rounded-md text-xs font-medium bg-slate-100 text-slate-800 dark:bg-slate-700 dark:text-slate-300"
                        >
                          +{{ template.enterpriseTypes.length - 2 }} more
                        </span>
                      </div>
                    </div>
                  </div>
                  <ChevronRightIcon class="h-5 w-5 text-slate-400" />
                </div>
                <div class="mt-4 flex items-center justify-between">
                  <span class="text-xs text-slate-500 dark:text-slate-400">{{ template.actions.length }} actions</span>
                  <button
                    @click.stop="useTemplate(key)"
                    class="inline-flex items-center px-3 py-1 rounded-md text-xs font-medium text-blue-700 bg-blue-100 hover:bg-blue-200 dark:bg-blue-900/20 dark:text-blue-400 dark:hover:bg-blue-900/30"
                  >
                    Use Template
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Lead Scoring Tab -->
      <div v-if="activeTab === 'scoring'" class="space-y-4">
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700">
            <h3 class="text-lg font-medium text-slate-900 dark:text-white">Lead Scoring Rules</h3>
            <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">
              Automatically score leads based on enterprise type and business characteristics
            </p>
          </div>
          <div class="p-6">
            <div class="space-y-4">
              <div
                v-for="rule in leadScoringRules"
                :key="rule.id"
                class="border border-slate-200 dark:border-slate-700 rounded-lg p-4"
              >
                <div class="flex items-center justify-between">
                  <div class="flex-1">
                    <div class="flex items-center space-x-3">
                      <h4 class="text-md font-medium text-slate-900 dark:text-white">{{ rule.name }}</h4>
                      <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900/20 dark:text-green-400">
                        +{{ rule.points }} points
                      </span>
                      <span
                        :class="[
                          'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
                          rule.active
                            ? 'bg-blue-100 text-blue-800 dark:bg-blue-900/20 dark:text-blue-400'
                            : 'bg-gray-100 text-gray-800 dark:bg-gray-900/20 dark:text-gray-400'
                        ]"
                      >
                        {{ rule.active ? 'Active' : 'Inactive' }}
                      </span>
                    </div>
                    <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">
                      {{ rule.field }} {{ rule.condition.operator }} {{ rule.condition.value }}
                    </p>
                    <div class="mt-2">
                      <span class="text-xs text-slate-500 dark:text-slate-400">
                        Applies to: {{ rule.enterpriseTypes.join(', ') }}
                      </span>
                    </div>
                  </div>
                  <div class="flex items-center space-x-2">
                    <button
                      @click="toggleScoringRule(rule.id)"
                      :class="[
                        'inline-flex items-center px-3 py-1 rounded-md text-sm font-medium',
                        rule.active
                          ? 'text-red-700 bg-red-100 hover:bg-red-200 dark:bg-red-900/20 dark:text-red-400'
                          : 'text-green-700 bg-green-100 hover:bg-green-200 dark:bg-green-900/20 dark:text-green-400'
                      ]"
                    >
                      {{ rule.active ? 'Disable' : 'Enable' }}
                    </button>
                    <button
                      @click="editScoringRule(rule.id)"
                      class="inline-flex items-center px-3 py-1 rounded-md text-sm font-medium text-slate-700 bg-slate-100 hover:bg-slate-200 dark:bg-slate-700 dark:text-slate-300"
                    >
                      Edit
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Executions Tab -->
      <div v-if="activeTab === 'executions'" class="space-y-4">
        <div class="bg-white dark:bg-slate-800 rounded-lg shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700">
            <h3 class="text-lg font-medium text-slate-900 dark:text-white">Recent Executions</h3>
          </div>
          <div class="p-6">
            <div v-if="activeExecutions.length === 0" class="text-center py-8">
              <ClockIcon class="mx-auto h-12 w-12 text-slate-400" />
              <h3 class="mt-2 text-sm font-medium text-slate-900 dark:text-white">No recent executions</h3>
              <p class="mt-1 text-sm text-slate-500 dark:text-slate-400">Automation executions will appear here when workflows are triggered.</p>
            </div>
            
            <div v-else class="space-y-4">
              <div
                v-for="execution in activeExecutions.slice(0, 10)"
                :key="execution.id"
                class="border border-slate-200 dark:border-slate-700 rounded-lg p-4"
              >
                <div class="flex items-center justify-between">
                  <div class="flex-1">
                    <div class="flex items-center space-x-3">
                      <h4 class="text-md font-medium text-slate-900 dark:text-white">
                        {{ getWorkflowName(execution.workflowId) }}
                      </h4>
                      <span
                        :class="[
                          'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium',
                          execution.status === 'completed'
                            ? 'bg-green-100 text-green-800 dark:bg-green-900/20 dark:text-green-400'
                            : execution.status === 'failed'
                            ? 'bg-red-100 text-red-800 dark:bg-red-900/20 dark:text-red-400'
                            : execution.status === 'running'
                            ? 'bg-blue-100 text-blue-800 dark:bg-blue-900/20 dark:text-blue-400'
                            : 'bg-gray-100 text-gray-800 dark:bg-gray-900/20 dark:text-gray-400'
                        ]"
                      >
                        {{ execution.status }}
                      </span>
                    </div>
                    <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">
                      Contact ID: {{ execution.contactId }}
                    </p>
                    <div class="flex items-center space-x-4 mt-2 text-xs text-slate-500 dark:text-slate-400">
                      <span>Started: {{ formatDate(execution.triggeredAt) }}</span>
                      <span v-if="execution.completedAt">Completed: {{ formatDate(execution.completedAt) }}</span>
                      <span>Progress: {{ execution.currentStep }}/{{ execution.totalSteps }}</span>
                    </div>
                  </div>
                  <button
                    @click="viewExecutionLogs(execution.id)"
                    class="inline-flex items-center px-3 py-1 rounded-md text-sm font-medium text-slate-700 bg-slate-100 hover:bg-slate-200 dark:bg-slate-700 dark:text-slate-300"
                  >
                    View Logs
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- Workflow Builder Modal -->
  <WorkflowBuilder
    v-if="showWorkflowBuilder"
    :editing-workflow="editingWorkflow"
    @close="closeWorkflowBuilder"
    @save="saveWorkflow"
  />

  <!-- Template Library Modal -->
  <TemplateLibrary
    v-if="showTemplateLibrary"
    @close="closeTemplateLibrary"
    @select="selectTemplate"
  />
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import WorkflowBuilder from '~/components/crm/WorkflowBuilder.vue'
import TemplateLibrary from '~/components/crm/TemplateLibrary.vue'
import {
  CogIcon,
  PlayIcon,
  ChartBarIcon,
  StarIcon,
  PlusIcon,
  DocumentTextIcon,
  ChevronRightIcon,
  ClockIcon
} from '@heroicons/vue/24/outline'

// Component state
const activeTab = ref('workflows')
const showWorkflowBuilder = ref(false)
const showTemplateLibrary = ref(false)
const editingWorkflow = ref(null as any)
const tabs = [
  { id: 'workflows', name: 'Active Workflows' },
  { id: 'templates', name: 'Township Templates' },
  { id: 'scoring', name: 'Lead Scoring' },
  { id: 'executions', name: 'Recent Executions' }
]

// Mock data for demonstration
const automationWorkflows = ref([
  {
    id: '1',
    name: 'Spaza Shop Onboarding',
    description: 'Welcome sequence for new spaza shop owners with inventory tips',
    enterpriseTypes: ['Spaza / Convenience shop'],
    triggers: [],
    actions: [],
    active: true,
    createdAt: new Date(),
    updatedAt: new Date(),
    stats: { timesTriggered: 45, successfulExecutions: 42, failedExecutions: 3 }
  },
  {
    id: '2',
    name: 'Service Provider Nurture',
    description: 'Educational sequence for electricians, plumbers, and trades',
    enterpriseTypes: ['Electricians', 'Plumbers', 'Carpenters & joiners'],
    triggers: [],
    actions: [],
    active: true,
    createdAt: new Date(),
    updatedAt: new Date(),
    stats: { timesTriggered: 28, successfulExecutions: 26, failedExecutions: 2 }
  },
  {
    id: '3',
    name: 'Agricultural Seasonal Reminders',
    description: 'Seasonal tips and market updates for agricultural enterprises',
    enterpriseTypes: ['Small-scale vegetable gardening', 'Poultry (chicken) farming'],
    triggers: [],
    actions: [],
    active: false,
    createdAt: new Date(),
    updatedAt: new Date(),
    stats: { timesTriggered: 12, successfulExecutions: 11, failedExecutions: 1 }
  }
])

const activeExecutions = ref([])

const leadScoringRules = ref([
  {
    id: '1',
    name: 'High-Value Spaza Shop',
    field: 'enterpriseType',
    condition: { operator: 'equals', value: 'Spaza / Convenience shop' },
    points: 15,
    enterpriseTypes: ['Spaza / Convenience shop'],
    active: true
  },
  {
    id: '2',
    name: 'Professional Service Provider',
    field: 'enterpriseType',
    condition: { operator: 'in', value: ['Electricians', 'Plumbers', 'Mechanics'] },
    points: 12,
    enterpriseTypes: ['Electricians', 'Plumbers', 'Mechanics (car repair)'],
    active: true
  }
])

const TOWNSHIP_AUTOMATION_TEMPLATES = {
  spaza_shop_onboarding: {
    name: 'Spaza Shop Onboarding',
    description: 'Welcome sequence with inventory management tips and business support',
    enterpriseTypes: ['Spaza / Convenience shop'],
    actions: [
      { type: 'send_email', description: 'Welcome email with inventory tips' },
      { type: 'send_sms', description: 'SMS with supplier contacts' },
      { type: 'create_task', description: 'Schedule follow-up call' }
    ]
  },
  service_provider_growth: {
    name: 'Service Provider Growth',
    description: 'Business development sequence for skilled trades',
    enterpriseTypes: ['Electricians', 'Plumbers', 'Carpenters & joiners'],
    actions: [
      { type: 'send_email', description: 'Business growth tips' },
      { type: 'send_whatsapp', description: 'Pricing strategy guide' },
      { type: 'add_tag', description: 'Add growth-focused tag' }
    ]
  },
  agricultural_support: {
    name: 'Agricultural Support',
    description: 'Seasonal reminders and market updates for farmers',
    enterpriseTypes: ['Small-scale vegetable gardening', 'Poultry (chicken) farming'],
    actions: [
      { type: 'send_email', description: 'Seasonal farming tips' },
      { type: 'send_sms', description: 'Market price updates' },
      { type: 'create_task', description: 'Schedule seasonal check-in' }
    ]
  }
}

// Computed stats
const activeWorkflowCount = computed(() => 
  automationWorkflows.value.filter(w => w.active).length
)

const executionsToday = computed(() => {
  const today = new Date().toDateString()
  return activeExecutions.value.filter(e => 
    e.triggeredAt.toDateString() === today
  ).length
})

const successRate = computed(() => {
  const total = activeExecutions.value.length
  if (total === 0) return 100
  const successful = activeExecutions.value.filter(e => e.status === 'completed').length
  return Math.round((successful / total) * 100)
})

const averageLeadScore = computed(() => {
  // This would be calculated from actual lead scores
  return 65
})

// Methods
const openWorkflowBuilder = () => {
  editingWorkflow.value = null
  showWorkflowBuilder.value = true
}

const openTemplateLibrary = () => {
  showTemplateLibrary.value = true
}

const openLeadScoring = () => {
  activeTab.value = 'scoring'
}

const closeWorkflowBuilder = () => {
  showWorkflowBuilder.value = false
  editingWorkflow.value = null
}

const closeTemplateLibrary = () => {
  showTemplateLibrary.value = false
}

const saveWorkflow = (workflow) => {
  const existingIndex = automationWorkflows.value.findIndex(w => w.id === workflow.id)
  if (existingIndex >= 0) {
    automationWorkflows.value[existingIndex] = workflow
  } else {
    automationWorkflows.value.push(workflow)
  }
  closeWorkflowBuilder()
}

const selectTemplate = (templateKey, template) => {
  // Pre-populate workflow builder with template data
  editingWorkflow.value = {
    id: '',
    name: template.name,
    description: template.description,
    enterpriseTypes: [...template.enterpriseTypes],
    triggers: [{ type: 'contact_created', description: 'When a new contact is created' }],
    actions: [...template.actions.map(action => ({ ...action, delay: 0 }))],
    active: false,
    createdAt: new Date(),
    updatedAt: new Date(),
    stats: { timesTriggered: 0, successfulExecutions: 0, failedExecutions: 0 }
  }
  closeTemplateLibrary()
  openWorkflowBuilder()
}

const toggleWorkflow = (workflowId: string) => {
  const workflow = automationWorkflows.value.find(w => w.id === workflowId)
  if (workflow) {
    workflow.active = !workflow.active
  }
}

const editWorkflow = (workflowId: string) => {
  console.log('Editing workflow:', workflowId)
  // Implementation for editing workflow
}

const previewTemplate = (templateKey: string) => {
  console.log('Previewing template:', templateKey)
  // Implementation for template preview
}

const useTemplate = (templateKey: string) => {
  console.log('Using template:', templateKey)
  // Implementation for using template
}

const toggleScoringRule = (ruleId: string) => {
  const rule = leadScoringRules.value.find(r => r.id === ruleId)
  if (rule) {
    rule.active = !rule.active
  }
}

const editScoringRule = (ruleId: string) => {
  console.log('Editing scoring rule:', ruleId)
  // Implementation for editing scoring rule
}

const viewExecutionLogs = (executionId: string) => {
  console.log('Viewing execution logs:', executionId)
  // Implementation for viewing execution logs
}

const getWorkflowName = (workflowId: string) => {
  const workflow = automationWorkflows.value.find(w => w.id === workflowId)
  return workflow?.name || 'Unknown Workflow'
}

const formatDate = (date: Date) => {
  return date.toLocaleString()
}

// Initialize on mount
onMounted(() => {
  // Data is already initialized above, no need for initializeLeadScoring()
})
</script>
