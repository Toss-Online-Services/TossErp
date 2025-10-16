/**
 * Mock Automation Data Service
 * Simulates workflows, triggers, and AI-powered automation
 */

export interface Workflow {
  id: number
  name: string
  description: string
  category: string
  trigger: string
  lastRun: string
  lastRunTime: string
  successRate: number
  status: 'active' | 'paused' | 'draft' | 'disabled'
  executionCount: number
}

export interface Trigger {
  id: number
  name: string
  description: string
  eventType: 'Create' | 'Update' | 'Delete' | 'Schedule' | 'Time'
  module: string
  conditions: string
  lastFired: string
  lastFiredTime: string
  fireCount: number
  status: 'active' | 'paused' | 'disabled'
}

export interface WorkflowExecution {
  id: number
  workflowId: number
  workflow: string
  module: string
  executedAt: string
  executedTime: string
  status: 'Success' | 'Failed' | 'Warning' | 'Pending'
  duration: number
  trigger: string
  result: string
  resultDetails: string
}

export interface AIRecommendation {
  id: number
  title: string
  description: string
  priority: 'High' | 'Medium' | 'Low'
  timeToImplement: string
  expectedImpact: string
  category: string
}

const mockWorkflows: Workflow[] = [
  { id: 1, name: 'Lead Assignment Workflow', description: 'Automatically assign new leads to sales representatives', category: 'CRM', trigger: 'New lead created', lastRun: '2 minutes ago', lastRunTime: '14:35', successRate: 98, status: 'active', executionCount: 245 },
  { id: 2, name: 'Invoice Payment Reminder', description: 'Send automatic payment reminders for overdue invoices', category: 'Accounting', trigger: 'Invoice overdue', lastRun: '1 hour ago', lastRunTime: '13:45', successRate: 95, status: 'active', executionCount: 156 },
  { id: 3, name: 'Low Stock Alert', description: 'Notify when inventory levels fall below minimum', category: 'Inventory', trigger: 'Stock level threshold', lastRun: '3 hours ago', lastRunTime: '11:20', successRate: 92, status: 'paused', executionCount: 89 },
  { id: 4, name: 'Order Processing', description: 'Automatically process and fulfill customer orders', category: 'Sales', trigger: 'Order confirmed', lastRun: '5 minutes ago', lastRunTime: '14:30', successRate: 99, status: 'active', executionCount: 567 }
]

const mockTriggers: Trigger[] = [
  { id: 1, name: 'New Customer Registration', description: 'Trigger when a new customer signs up', eventType: 'Create', module: 'CRM', conditions: 'customer.status = "new" AND customer.verified = true', lastFired: '5 minutes ago', lastFiredTime: '14:30', fireCount: 45, status: 'active' },
  { id: 2, name: 'Low Stock Alert', description: 'Trigger when inventory falls below threshold', eventType: 'Update', module: 'Inventory', conditions: 'stock.quantity <= stock.minimum_threshold', lastFired: '2 hours ago', lastFiredTime: '12:35', fireCount: 23, status: 'active' },
  { id: 3, name: 'Order Confirmation', description: 'Trigger when order status changes to confirmed', eventType: 'Update', module: 'Sales', conditions: 'order.status = "confirmed" AND order.payment_status = "paid"', lastFired: '10 minutes ago', lastFiredTime: '14:25', fireCount: 156, status: 'active' },
  { id: 4, name: 'Daily Sales Report', description: 'Generate daily sales summary at 6 PM', eventType: 'Schedule', module: 'Sales', conditions: 'time = "18:00" AND weekday != "saturday,sunday"', lastFired: 'Yesterday', lastFiredTime: '18:00', fireCount: 365, status: 'active' }
]

const mockExecutions: WorkflowExecution[] = [
  { id: 1, workflowId: 1, workflow: 'Customer Onboarding', module: 'CRM', executedAt: 'Today', executedTime: '14:30', status: 'Success', duration: 2340, trigger: 'Customer registration', result: 'Completed', resultDetails: '15 new customers onboarded' },
  { id: 2, workflowId: 4, workflow: 'Order Processing', module: 'Sales', executedAt: 'Today', executedTime: '14:15', status: 'Success', duration: 156, trigger: 'Order confirmed', result: 'Processed', resultDetails: '23 orders processed' },
  { id: 3, workflowId: 2, workflow: 'Email Campaign', module: 'Marketing', executedAt: 'Today', executedTime: '13:45', status: 'Failed', duration: 8900, trigger: 'Scheduled', result: 'Error', resultDetails: 'SMTP connection failed' }
]

const mockAIRecommendations: AIRecommendation[] = [
  { id: 1, title: 'Automate Lead Qualification', description: 'Use ML to automatically score and qualify incoming leads based on historical data', priority: 'High', timeToImplement: '2 days', expectedImpact: '+25% efficiency', category: 'Sales' },
  { id: 2, title: 'Smart Inventory Reordering', description: 'Implement predictive analytics for optimal stock level management', priority: 'Medium', timeToImplement: '3 days', expectedImpact: '+15% cost savings', category: 'Inventory' },
  { id: 3, title: 'Customer Support Triage', description: 'Auto-categorize and route support tickets to appropriate teams', priority: 'High', timeToImplement: '1 day', expectedImpact: '+40% response time', category: 'Support' },
  { id: 4, title: 'A/B Test Automation', description: 'Automatically run and analyze A/B tests for email campaigns', priority: 'Low', timeToImplement: '5 days', expectedImpact: '+20% conversion', category: 'Marketing' }
]

export class MockAutomationService {
  static getWorkflows(): Workflow[] {
    return mockWorkflows
  }

  static getTriggers(): Trigger[] {
    return mockTriggers
  }

  static getExecutions(): WorkflowExecution[] {
    return mockExecutions
  }

  static getAIRecommendations(): AIRecommendation[] {
    return mockAIRecommendations
  }

  static getAutomationStats() {
    return {
      activeWorkflows: mockWorkflows.filter(w => w.status === 'active').length,
      timeSaved: 42,
      executions: mockWorkflows.reduce((sum, w) => sum + w.executionCount, 0),
      failedRuns: 3,
      successRate: 96.8
    }
  }

  static getTriggerStats() {
    return {
      activeTriggers: mockTriggers.filter(t => t.status === 'active').length,
      todayFires: mockTriggers.reduce((sum, t) => sum + t.fireCount, 0),
      avgResponse: 125,
      failedTriggers: 2
    }
  }

  static executeWorkflow(workflowId: number): WorkflowExecution {
    const workflow = mockWorkflows.find(w => w.id === workflowId)
    const newExecution: WorkflowExecution = {
      id: mockExecutions.length + 1,
      workflowId,
      workflow: workflow?.name || 'Unknown',
      module: workflow?.category || 'General',
      executedAt: 'Just now',
      executedTime: new Date().toLocaleTimeString(),
      status: 'Success',
      duration: Math.floor(Math.random() * 5000) + 100,
      trigger: 'Manual',
      result: 'Completed',
      resultDetails: 'Executed successfully'
    }
    mockExecutions.unshift(newExecution)
    return newExecution
  }

  static getAIMetrics() {
    return {
      accuracy: 94,
      optimization: 87,
      learning: 76
    }
  }
}

