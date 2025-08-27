// CRM Automation Engine for TOSS ERP III
import { ref } from 'vue'

export interface AutomationTrigger {
  id: string
  name: string
  type: 'contact_created' | 'status_changed' | 'time_based' | 'field_updated' | 'activity_completed' | 'email_opened' | 'sms_received'
  conditions: AutomationCondition[]
  active: boolean
}

export interface AutomationCondition {
  field: string
  operator: 'equals' | 'not_equals' | 'contains' | 'greater_than' | 'less_than' | 'is_empty' | 'is_not_empty'
  value: string | number | boolean
  logicalOperator?: 'AND' | 'OR'
}

export interface AutomationAction {
  id: string
  type: 'send_email' | 'send_sms' | 'create_task' | 'update_field' | 'add_tag' | 'move_pipeline' | 'create_opportunity' | 'schedule_callback' | 'assign_user' | 'wait' | 'webhook'
  config: Record<string, any>
  delay?: number // in minutes
}

export interface AutomationWorkflow {
  id: string
  name: string
  description: string
  enterpriseTypes: string[] // Which enterprise types this applies to
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

export interface AutomationExecution {
  id: string
  workflowId: string
  contactId: string
  triggeredAt: Date
  completedAt?: Date
  status: 'pending' | 'running' | 'completed' | 'failed' | 'cancelled'
  currentStep: number
  totalSteps: number
  error?: string
  logs: AutomationLog[]
}

export interface AutomationLog {
  timestamp: Date
  action: string
  status: 'success' | 'error' | 'warning'
  message: string
  data?: Record<string, any>
}

// Lead Scoring Configuration
export interface LeadScoringRule {
  id: string
  name: string
  field: string
  condition: AutomationCondition
  points: number
  enterpriseTypes: string[]
  active: boolean
}

export interface LeadScore {
  contactId: string
  totalScore: number
  grade: 'A' | 'B' | 'C' | 'D' | 'F'
  scoreBreakdown: {
    ruleId: string
    ruleName: string
    points: number
  }[]
  lastCalculated: Date
}

// Email/SMS Templates for Township Enterprises
export interface CommunicationTemplate {
  id: string
  name: string
  type: 'email' | 'sms'
  subject?: string // for emails
  content: string
  variables: string[] // {{firstName}}, {{enterpriseType}}, etc.
  enterpriseTypes: string[]
  language: 'en' | 'zu' | 'xh' | 'af' | 'st' // Common SA languages
  category: 'welcome' | 'follow_up' | 'nurture' | 'promotion' | 'reminder' | 'survey'
  active: boolean
}

// Customer Journey Stages for Township Enterprises
export interface JourneyStage {
  id: string
  name: string
  description: string
  enterpriseTypes: string[]
  triggers: AutomationTrigger[]
  actions: AutomationAction[]
  nextStages: string[]
  averageDuration: number // in days
}

export interface CustomerJourney {
  id: string
  contactId: string
  currentStage: string
  stageHistory: {
    stageId: string
    enteredAt: Date
    exitedAt?: Date
    actions: string[]
  }[]
  completedActions: string[]
  score: number
}

// Township-Specific Automation Templates
export const TOWNSHIP_AUTOMATION_TEMPLATES = {
  // For Spaza Shop Owners
  spaza_onboarding: {
    name: "Spaza Shop Owner Onboarding",
    description: "Welcome sequence for spaza shop owners with inventory management tips",
    enterpriseTypes: ["Spaza / Convenience shop"],
    actions: [
      { type: 'send_sms', delay: 0, config: { template: 'spaza_welcome_sms' } },
      { type: 'create_task', delay: 1440, config: { title: 'Follow up on inventory needs', priority: 'medium' } },
      { type: 'send_email', delay: 2880, config: { template: 'spaza_inventory_tips' } }
    ]
  },
  
  // For Service Providers
  service_provider_nurture: {
    name: "Service Provider Nurture Campaign",
    description: "Educational sequence for electricians, plumbers, and other trades",
    enterpriseTypes: ["Electricians", "Plumbers", "Carpenters & joiners", "Welders & metalworkers"],
    actions: [
      { type: 'send_sms', delay: 0, config: { template: 'service_welcome' } },
      { type: 'send_email', delay: 1440, config: { template: 'business_growth_tips' } },
      { type: 'create_task', delay: 4320, config: { title: 'Check business registration status' } }
    ]
  },
  
  // For Food & Hospitality
  food_business_compliance: {
    name: "Food Business Compliance Reminder",
    description: "Health and safety reminders for food businesses",
    enterpriseTypes: ["Township restaurant / Imbizo / braai stalls", "Prepared-food vendors / street food", "Home bakeries & confectioneries"],
    actions: [
      { type: 'send_sms', delay: 0, config: { template: 'food_safety_reminder' } },
      { type: 'create_task', delay: 10080, config: { title: 'Schedule health inspection follow-up' } }
    ]
  },
  
  // For Agricultural Enterprises
  agri_seasonal_support: {
    name: "Agricultural Seasonal Support",
    description: "Seasonal reminders and support for agricultural enterprises",
    enterpriseTypes: ["Small-scale vegetable gardening", "Poultry (chicken) farming", "Small livestock (goats, sheep)"],
    actions: [
      { type: 'send_sms', delay: 0, config: { template: 'seasonal_farming_tips' } },
      { type: 'send_email', delay: 2880, config: { template: 'market_price_updates' } },
      { type: 'create_task', delay: 7200, config: { title: 'Check on harvest planning' } }
    ]
  }
}

// Composable for automation management
export const useAutomationEngine = () => {
  const automationWorkflows = ref<AutomationWorkflow[]>([])
  const activeExecutions = ref<AutomationExecution[]>([])
  const leadScoringRules = ref<LeadScoringRule[]>([])
  const communicationTemplates = ref<CommunicationTemplate[]>([])
  
  // Initialize default lead scoring rules for township enterprises
  const initializeLeadScoring = () => {
    const defaultRules: LeadScoringRule[] = [
      {
        id: 'enterprise-type-spaza',
        name: 'Spaza Shop Owner',
        field: 'enterpriseType',
        condition: { field: 'enterpriseType', operator: 'equals', value: 'Spaza / Convenience shop' },
        points: 15,
        enterpriseTypes: ['Spaza / Convenience shop'],
        active: true
      },
      {
        id: 'business-registered',
        name: 'Registered Business',
        field: 'businessRegistered',
        condition: { field: 'businessRegistered', operator: 'equals', value: true },
        points: 20,
        enterpriseTypes: ['*'],
        active: true
      },
      {
        id: 'monthly-revenue-high',
        name: 'High Monthly Revenue',
        field: 'monthlyRevenue',
        condition: { field: 'monthlyRevenue', operator: 'greater_than', value: 10000 },
        points: 25,
        enterpriseTypes: ['*'],
        active: true
      },
      {
        id: 'location-urban',
        name: 'Urban Location',
        field: 'location',
        condition: { field: 'location', operator: 'contains', value: 'urban' },
        points: 10,
        enterpriseTypes: ['*'],
        active: true
      }
    ]
    
    leadScoringRules.value = defaultRules
  }
  
  // Calculate lead score for a contact
  const calculateLeadScore = (contact: any): LeadScore => {
    let totalScore = 0
    const scoreBreakdown: any[] = []
    
    leadScoringRules.value.forEach(rule => {
      if (!rule.active) return
      if (rule.enterpriseTypes[0] !== '*' && !rule.enterpriseTypes.includes(contact.enterpriseType)) return
      
      const fieldValue = contact[rule.field]
      let ruleMatches = false
      
      switch (rule.condition.operator) {
        case 'equals':
          ruleMatches = fieldValue === rule.condition.value
          break
        case 'greater_than':
          ruleMatches = Number(fieldValue) > Number(rule.condition.value)
          break
        case 'contains':
          ruleMatches = String(fieldValue).toLowerCase().includes(String(rule.condition.value).toLowerCase())
          break
        // Add more operators as needed
      }
      
      if (ruleMatches) {
        totalScore += rule.points
        scoreBreakdown.push({
          ruleId: rule.id,
          ruleName: rule.name,
          points: rule.points
        })
      }
    })
    
    // Determine grade
    let grade: 'A' | 'B' | 'C' | 'D' | 'F' = 'F'
    if (totalScore >= 80) grade = 'A'
    else if (totalScore >= 60) grade = 'B'
    else if (totalScore >= 40) grade = 'C'
    else if (totalScore >= 20) grade = 'D'
    
    return {
      contactId: contact.id,
      totalScore,
      grade,
      scoreBreakdown,
      lastCalculated: new Date()
    }
  }
  
  // Execute automation workflow
  const executeWorkflow = async (workflowId: string, contactId: string, trigger: string) => {
    const workflow = automationWorkflows.value.find(w => w.id === workflowId)
    if (!workflow || !workflow.active) return
    
    const execution: AutomationExecution = {
      id: generateId(),
      workflowId,
      contactId,
      triggeredAt: new Date(),
      status: 'running',
      currentStep: 0,
      totalSteps: workflow.actions.length,
      logs: [{
        timestamp: new Date(),
        action: 'workflow_started',
        status: 'success',
        message: `Workflow triggered by: ${trigger}`
      }]
    }
    
    activeExecutions.value.push(execution)
    
    // Execute actions sequentially with delays
    for (let i = 0; i < workflow.actions.length; i++) {
      const action = workflow.actions[i]
      
      if (action.delay && action.delay > 0) {
        // In real implementation, this would schedule the action
        await new Promise(resolve => setTimeout(resolve, (action.delay || 0) * 60 * 1000))
      }
      
      try {
        await executeAction(action, contactId, execution)
        execution.currentStep = i + 1
        execution.logs.push({
          timestamp: new Date(),
          action: action.type,
          status: 'success',
          message: `Action ${action.type} completed successfully`
        })
      } catch (error) {
        execution.status = 'failed'
        execution.error = String(error)
        execution.logs.push({
          timestamp: new Date(),
          action: action.type,
          status: 'error',
          message: `Action ${action.type} failed: ${error}`
        })
        break
      }
    }
    
    execution.status = execution.status === 'failed' ? 'failed' : 'completed'
    execution.completedAt = new Date()
    
    // Update workflow stats
    workflow.stats.timesTriggered++
    if (execution.status === 'completed') {
      workflow.stats.successfulExecutions++
    } else {
      workflow.stats.failedExecutions++
    }
  }
  
  const executeAction = async (action: AutomationAction, contactId: string, execution: AutomationExecution) => {
    switch (action.type) {
      case 'send_email':
        // Implement email sending
        console.log(`Sending email to contact ${contactId}`, action.config)
        break
      case 'send_sms':
        // Implement SMS sending
        console.log(`Sending SMS to contact ${contactId}`, action.config)
        break
      case 'create_task':
        // Implement task creation
        console.log(`Creating task for contact ${contactId}`, action.config)
        break
      case 'update_field':
        // Implement field update
        console.log(`Updating field for contact ${contactId}`, action.config)
        break
      default:
        throw new Error(`Unknown action type: ${action.type}`)
    }
  }
  
  const generateId = () => Math.random().toString(36).substr(2, 9)
  
  return {
    automationWorkflows,
    activeExecutions,
    leadScoringRules,
    communicationTemplates,
    initializeLeadScoring,
    calculateLeadScore,
    executeWorkflow,
    TOWNSHIP_AUTOMATION_TEMPLATES
  }
}
