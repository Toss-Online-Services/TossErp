<template>
  <div class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center p-4 z-50">
    <div class="bg-white dark:bg-slate-800 rounded-lg shadow-xl max-w-4xl w-full max-h-[90vh] overflow-hidden">
      <!-- Modal Header -->
      <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700 flex items-center justify-between">
        <div>
          <h3 class="text-lg font-medium text-slate-900 dark:text-white">
            {{ editingWorkflow ? 'Edit Workflow' : 'Create New Workflow' }}
          </h3>
          <p class="text-sm text-slate-600 dark:text-slate-400">
            Build automation workflows for township enterprises
          </p>
        </div>
        <button
          @click="$emit('close')"
          class="text-slate-400 hover:text-slate-600 dark:hover:text-slate-300"
        >
          <XMarkIcon class="h-6 w-6" />
        </button>
      </div>

      <!-- Modal Body -->
      <div class="flex h-[calc(90vh-120px)]">
        <!-- Workflow Configuration Panel -->
        <div class="w-1/3 border-r border-slate-200 dark:border-slate-700 overflow-y-auto">
          <div class="p-6 space-y-6">
            <!-- Basic Info -->
            <div>
              <h4 class="text-md font-medium text-slate-900 dark:text-white mb-3">Basic Information</h4>
              <div class="space-y-4">
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
                    Workflow Name
                  </label>
                  <input
                    v-model="workflowForm.name"
                    type="text"
                    placeholder="e.g., Spaza Shop Welcome Sequence"
                    class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-slate-700 dark:text-white"
                  />
                </div>
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
                    Description
                  </label>
                  <textarea
                    v-model="workflowForm.description"
                    rows="3"
                    placeholder="Brief description of what this workflow does"
                    class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-slate-700 dark:text-white"
                  ></textarea>
                </div>
              </div>
            </div>

            <!-- Enterprise Types -->
            <div>
              <h4 class="text-md font-medium text-slate-900 dark:text-white mb-3">Target Enterprise Types</h4>
              <div class="space-y-2 max-h-40 overflow-y-auto">
                <div
                  v-for="enterpriseType in availableEnterpriseTypes"
                  :key="enterpriseType"
                  class="flex items-center"
                >
                  <input
                    :id="`enterprise-${enterpriseType}`"
                    v-model="workflowForm.enterpriseTypes"
                    :value="enterpriseType"
                    type="checkbox"
                    class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-slate-300 dark:border-slate-600 rounded"
                  />
                  <label
                    :for="`enterprise-${enterpriseType}`"
                    class="ml-2 text-sm text-slate-700 dark:text-slate-300"
                  >
                    {{ enterpriseType }}
                  </label>
                </div>
              </div>
            </div>

            <!-- Quick Templates -->
            <div>
              <h4 class="text-md font-medium text-slate-900 dark:text-white mb-3">Quick Start</h4>
              <div class="space-y-2">
                <button
                  v-for="(template, key) in quickTemplates"
                  :key="key"
                  @click="loadTemplate(key)"
                  class="w-full text-left p-3 border border-slate-200 dark:border-slate-700 rounded-lg hover:border-blue-300 dark:hover:border-blue-600 transition-colors"
                >
                  <div class="text-sm font-medium text-slate-900 dark:text-white">{{ template.name }}</div>
                  <div class="text-xs text-slate-600 dark:text-slate-400">{{ template.description }}</div>
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Workflow Builder Canvas -->
        <div class="flex-1 overflow-y-auto">
          <div class="p-6">
            <!-- Workflow Steps -->
            <div class="space-y-4">
              <div class="flex items-center justify-between">
                <h4 class="text-md font-medium text-slate-900 dark:text-white">Workflow Steps</h4>
                <div class="flex space-x-2">
                  <button
                    @click="addTrigger"
                    class="inline-flex items-center px-3 py-1 border border-slate-300 dark:border-slate-600 rounded-md text-sm font-medium text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-700 hover:bg-slate-50 dark:hover:bg-slate-600"
                  >
                    <PlusIcon class="w-4 h-4 mr-1" />
                    Add Trigger
                  </button>
                  <button
                    @click="addAction"
                    class="inline-flex items-center px-3 py-1 border border-slate-300 dark:border-slate-600 rounded-md text-sm font-medium text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-700 hover:bg-slate-50 dark:hover:bg-slate-600"
                  >
                    <PlusIcon class="w-4 h-4 mr-1" />
                    Add Action
                  </button>
                </div>
              </div>

              <!-- Triggers Section -->
              <div v-if="workflowForm.triggers.length > 0">
                <h5 class="text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Triggers</h5>
                <div class="space-y-2">
                  <div
                    v-for="(trigger, index) in workflowForm.triggers"
                    :key="`trigger-${index}`"
                    class="flex items-center justify-between p-3 border border-blue-200 dark:border-blue-700 bg-blue-50 dark:bg-blue-900/20 rounded-lg"
                  >
                    <div class="flex items-center space-x-3">
                      <div class="h-8 w-8 bg-blue-100 dark:bg-blue-900/40 rounded-lg flex items-center justify-center">
                        <BoltIcon class="h-4 w-4 text-blue-600 dark:text-blue-400" />
                      </div>
                      <div>
                        <div class="text-sm font-medium text-slate-900 dark:text-white">{{ trigger.type }}</div>
                        <div class="text-xs text-slate-600 dark:text-slate-400">{{ trigger.description }}</div>
                      </div>
                    </div>
                    <button
                      @click="removeTrigger(index)"
                      class="text-red-600 hover:text-red-700 dark:text-red-400 dark:hover:text-red-300"
                    >
                      <TrashIcon class="h-4 w-4" />
                    </button>
                  </div>
                </div>
              </div>

              <!-- Actions Section -->
              <div v-if="workflowForm.actions.length > 0">
                <h5 class="text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Actions</h5>
                <div class="space-y-2">
                  <div
                    v-for="(action, index) in workflowForm.actions"
                    :key="`action-${index}`"
                    class="flex items-center justify-between p-3 border border-green-200 dark:border-green-700 bg-green-50 dark:bg-green-900/20 rounded-lg"
                  >
                    <div class="flex items-center space-x-3">
                      <div class="h-8 w-8 bg-green-100 dark:bg-green-900/40 rounded-lg flex items-center justify-center">
                        <PlayIcon class="h-4 w-4 text-green-600 dark:text-green-400" />
                      </div>
                      <div>
                        <div class="text-sm font-medium text-slate-900 dark:text-white">{{ action.type }}</div>
                        <div class="text-xs text-slate-600 dark:text-slate-400">{{ action.description }}</div>
                        <div v-if="action.delay" class="text-xs text-orange-600 dark:text-orange-400">
                          Delay: {{ action.delay }}ms
                        </div>
                      </div>
                    </div>
                    <div class="flex items-center space-x-2">
                      <button
                        @click="editAction(index)"
                        class="text-slate-600 hover:text-slate-700 dark:text-slate-400 dark:hover:text-slate-300"
                      >
                        <PencilIcon class="h-4 w-4" />
                      </button>
                      <button
                        @click="removeAction(index)"
                        class="text-red-600 hover:text-red-700 dark:text-red-400 dark:hover:text-red-300"
                      >
                        <TrashIcon class="h-4 w-4" />
                      </button>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Empty State -->
              <div v-if="workflowForm.triggers.length === 0 && workflowForm.actions.length === 0" class="text-center py-8">
                <CogIcon class="mx-auto h-12 w-12 text-slate-400" />
                <h3 class="mt-2 text-sm font-medium text-slate-900 dark:text-white">No workflow steps yet</h3>
                <p class="mt-1 text-sm text-slate-500 dark:text-slate-400">Add triggers and actions to build your automation workflow.</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Modal Footer -->
      <div class="px-6 py-4 border-t border-slate-200 dark:border-slate-700 flex items-center justify-between">
        <div class="flex items-center space-x-2">
          <input
            v-model="workflowForm.active"
            type="checkbox"
            id="workflow-active"
            class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-slate-300 dark:border-slate-600 rounded"
          />
          <label for="workflow-active" class="text-sm text-slate-700 dark:text-slate-300">
            Activate workflow immediately
          </label>
        </div>
        <div class="flex space-x-3">
          <button
            @click="$emit('close')"
            class="px-4 py-2 text-sm font-medium text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-700 border border-slate-300 dark:border-slate-600 rounded-md hover:bg-slate-50 dark:hover:bg-slate-600"
          >
            Cancel
          </button>
          <button
            @click="saveWorkflow"
            :disabled="!canSave"
            :class="[
              'px-4 py-2 text-sm font-medium text-white rounded-md',
              canSave
                ? 'bg-blue-600 hover:bg-blue-700'
                : 'bg-slate-400 cursor-not-allowed'
            ]"
          >
            {{ editingWorkflow ? 'Update Workflow' : 'Create Workflow' }}
          </button>
        </div>
      </div>
    </div>

    <!-- Action Configuration Modal -->
    <div v-if="showActionConfig" class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center p-4 z-60">
      <div class="bg-white dark:bg-slate-800 rounded-lg shadow-xl max-w-md w-full">
        <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-lg font-medium text-slate-900 dark:text-white">Configure Action</h3>
        </div>
        <div class="p-6 space-y-4">
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
              Action Type
            </label>
            <select
              v-model="actionConfig.type"
              class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-slate-700 dark:text-white"
            >
              <option value="send_email">Send Email</option>
              <option value="send_sms">Send SMS</option>
              <option value="add_tag">Add Tag</option>
              <option value="update_field">Update Field</option>
              <option value="create_task">Create Task</option>
              <option value="send_whatsapp">Send WhatsApp</option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
              Description
            </label>
            <input
              v-model="actionConfig.description"
              type="text"
              placeholder="Brief description of this action"
              class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-slate-700 dark:text-white"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">
              Delay (milliseconds)
            </label>
            <input
              v-model.number="actionConfig.delay"
              type="number"
              placeholder="0"
              class="w-full px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-md shadow-sm focus:outline-none focus:ring-blue-500 focus:border-blue-500 dark:bg-slate-700 dark:text-white"
            />
          </div>
        </div>
        <div class="px-6 py-4 border-t border-slate-200 dark:border-slate-700 flex justify-end space-x-3">
          <button
            @click="cancelActionConfig"
            class="px-4 py-2 text-sm font-medium text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-700 border border-slate-300 dark:border-slate-600 rounded-md hover:bg-slate-50 dark:hover:bg-slate-600"
          >
            Cancel
          </button>
          <button
            @click="saveActionConfig"
            class="px-4 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-md"
          >
            Save Action
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import {
  XMarkIcon,
  PlusIcon,
  BoltIcon,
  PlayIcon,
  TrashIcon,
  PencilIcon,
  CogIcon
} from '@heroicons/vue/24/outline'

// Define types locally
interface AutomationTrigger {
  type: string
  description: string
}

interface AutomationAction {
  type: string
  description: string
  delay?: number
}

interface AutomationWorkflow {
  id: string
  name: string
  description: string
  enterpriseTypes: string[]
  triggers: AutomationTrigger[]
  actions: AutomationAction[]
  active: boolean
  createdAt: Date
  updatedAt: Date
  stats: {
    timesTriggered: number
    successfulExecutions: number
    failedExecutions: number
  }
}

interface Props {
  editingWorkflow?: AutomationWorkflow | null
}

const props = defineProps<Props>()
const emit = defineEmits<{
  close: []
  save: [workflow: AutomationWorkflow]
}>()

// Form state
const workflowForm = ref<Partial<AutomationWorkflow>>({
  name: '',
  description: '',
  enterpriseTypes: [],
  triggers: [],
  actions: [],
  active: true
})

// Action configuration modal
const showActionConfig = ref(false)
const actionConfig = ref<Partial<AutomationAction>>({
  type: 'send_email',
  description: '',
  delay: 0
})
const editingActionIndex = ref<number | null>(null)

// Sample enterprise types for demo
const availableEnterpriseTypes = ref([
  'Spaza / Convenience shop',
  'Hair salon',
  'Electricians',
  'Plumbers',
  'Small-scale vegetable gardening',
  'Poultry (chicken) farming',
  'Fast food / Takeaway',
  'Carpenters & joiners',
  'Mechanics (car repair)',
  'Taxis / Transport services'
])

// Quick templates
const quickTemplates = {
  'spaza_welcome': {
    name: 'Spaza Shop Welcome',
    description: 'Welcome sequence for new spaza shop owners',
    triggers: [
      { type: 'contact_created', description: 'When a new spaza shop contact is created' }
    ],
    actions: [
      { type: 'send_email', description: 'Send welcome email with business tips', delay: 0 },
      { type: 'send_sms', description: 'Send SMS with inventory management tips', delay: 86400000 },
      { type: 'create_task', description: 'Create follow-up call task', delay: 604800000 }
    ]
  },
  'service_nurture': {
    name: 'Service Provider Nurture',
    description: 'Educational sequence for trade services',
    triggers: [
      { type: 'tag_added', description: 'When "service_provider" tag is added' }
    ],
    actions: [
      { type: 'send_email', description: 'Send business growth tips', delay: 0 },
      { type: 'send_whatsapp', description: 'Share pricing strategy guide', delay: 259200000 },
      { type: 'add_tag', description: 'Add "engaged" tag', delay: 604800000 }
    ]
  }
}

// Computed
const canSave = computed(() => {
  return workflowForm.value.name && 
         workflowForm.value.description && 
         workflowForm.value.enterpriseTypes?.length > 0 &&
         (workflowForm.value.triggers?.length > 0 || workflowForm.value.actions?.length > 0)
})

// Methods
const loadTemplate = (templateKey: keyof typeof quickTemplates) => {
  const template = quickTemplates[templateKey]
  workflowForm.value.triggers = [...template.triggers]
  workflowForm.value.actions = [...template.actions]
}

const addTrigger = () => {
  if (!workflowForm.value.triggers) {
    workflowForm.value.triggers = []
  }
  workflowForm.value.triggers.push({
    type: 'contact_created',
    description: 'When a new contact is created'
  })
}

const removeTrigger = (index: number) => {
  workflowForm.value.triggers?.splice(index, 1)
}

const addAction = () => {
  showActionConfig.value = true
  editingActionIndex.value = null
  actionConfig.value = {
    type: 'send_email',
    description: '',
    delay: 0
  }
}

const editAction = (index: number) => {
  const action = workflowForm.value.actions?.[index]
  if (action) {
    showActionConfig.value = true
    editingActionIndex.value = index
    actionConfig.value = { ...action }
  }
}

const removeAction = (index: number) => {
  workflowForm.value.actions?.splice(index, 1)
}

const saveActionConfig = () => {
  if (!workflowForm.value.actions) {
    workflowForm.value.actions = []
  }

  const action: AutomationAction = {
    type: actionConfig.value.type!,
    description: actionConfig.value.description || '',
    delay: actionConfig.value.delay || 0
  }

  if (editingActionIndex.value !== null) {
    workflowForm.value.actions[editingActionIndex.value] = action
  } else {
    workflowForm.value.actions.push(action)
  }

  showActionConfig.value = false
}

const cancelActionConfig = () => {
  showActionConfig.value = false
  editingActionIndex.value = null
}

const saveWorkflow = () => {
  if (!canSave.value) return

  const workflow: AutomationWorkflow = {
    id: props.editingWorkflow?.id || Date.now().toString(),
    name: workflowForm.value.name!,
    description: workflowForm.value.description!,
    enterpriseTypes: workflowForm.value.enterpriseTypes!,
    triggers: workflowForm.value.triggers || [],
    actions: workflowForm.value.actions || [],
    active: workflowForm.value.active || false,
    createdAt: props.editingWorkflow?.createdAt || new Date(),
    updatedAt: new Date(),
    stats: props.editingWorkflow?.stats || {
      timesTriggered: 0,
      successfulExecutions: 0,
      failedExecutions: 0
    }
  }

  emit('save', workflow)
}

// Initialize form if editing
onMounted(() => {
  if (props.editingWorkflow) {
    workflowForm.value = { ...props.editingWorkflow }
  }
})
</script>
