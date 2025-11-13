<template>
  <div class="p-6">
    <div class="flex items-center justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          CRM Opportunities
        </h1>
        <p class="text-gray-600 dark:text-gray-400">
          Track sales opportunities and manage the sales pipeline
        </p>
      </div>
      <button class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg">
        <Icon name="heroicons:plus" class="w-4 h-4 mr-2" />
        Add Opportunity
      </button>
    </div>

    <!-- Pipeline Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-blue-100 rounded-full">
            <Icon name="mdi:target-account" class="w-5 h-5 text-blue-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Total Opportunities</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">47</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-green-100 rounded-full">
            <Icon name="heroicons:banknotes" class="w-5 h-5 text-green-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Pipeline Value</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">R 890K</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-purple-100 rounded-full">
            <Icon name="heroicons:chart-bar" class="w-5 h-5 text-purple-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Win Rate</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">64.2%</p>
          </div>
        </div>
      </div>
      
      <div class="bg-white dark:bg-gray-800 p-4 rounded-lg shadow-sm">
        <div class="flex items-center">
          <div class="p-2 bg-orange-100 rounded-full">
            <Icon name="heroicons:clock" class="w-5 h-5 text-orange-600" />
          </div>
          <div class="ml-3">
            <p class="text-sm text-gray-600 dark:text-gray-400">Avg Deal Time</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-white">18 days</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Pipeline Board -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm mb-6">
      <div class="p-6">
        <h2 class="text-lg font-medium text-gray-900 dark:text-white mb-4">
          Sales Pipeline
        </h2>
        
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div v-for="stage in pipelineStages" :key="stage.id" class="space-y-3">
            <div class="flex items-center justify-between">
              <h3 class="font-medium text-gray-900 dark:text-white">{{ stage.name }}</h3>
              <span class="text-sm text-gray-600 dark:text-gray-400">
                {{ stage.opportunities.length }}
              </span>
            </div>
            
            <div class="space-y-2">
              <div v-for="opportunity in stage.opportunities" :key="opportunity.id" 
                   class="bg-gray-50 dark:bg-gray-700 p-3 rounded-lg border-l-4"
                   :class="getPriorityBorderClass(opportunity.priority)">
                <div class="flex items-start justify-between mb-2">
                  <h4 class="font-medium text-sm text-gray-900 dark:text-white">
                    {{ opportunity.title }}
                  </h4>
                  <span class="text-xs text-gray-600 dark:text-gray-400">
                    {{ opportunity.probability }}%
                  </span>
                </div>
                
                <div class="space-y-1">
                  <p class="text-xs text-gray-600 dark:text-gray-400">
                    {{ opportunity.company }}
                  </p>
                  <div class="flex items-center justify-between">
                    <span class="text-sm font-medium text-green-600">
                      R {{ opportunity.value.toLocaleString() }}
                    </span>
                    <span class="text-xs text-gray-500">
                      {{ formatDaysAgo(opportunity.lastActivity) }}
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Opportunities List -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm">
      <div class="p-6">
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-lg font-medium text-gray-900 dark:text-white">
            All Opportunities
          </h2>
          
          <div class="flex space-x-2">
            <select class="px-3 py-1 text-sm border border-gray-300 rounded focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-gray-700 dark:border-gray-600 dark:text-white">
              <option value="">All Stages</option>
              <option value="lead">Lead</option>
              <option value="qualified">Qualified</option>
              <option value="proposal">Proposal</option>
              <option value="negotiation">Negotiation</option>
            </select>
          </div>
        </div>
        
        <div class="space-y-4">
          <div v-for="opportunity in mockOpportunities" :key="opportunity.id" 
               class="border border-gray-200 dark:border-gray-700 rounded-lg p-4">
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <div class="flex items-center mb-3">
                  <div class="w-12 h-12 rounded-lg flex items-center justify-center mr-4"
                       :class="getStageColor(opportunity.stage)">
                    <Icon name="mdi:target-account" class="w-6 h-6 text-white" />
                  </div>
                  <div>
                    <h3 class="font-medium text-gray-900 dark:text-white text-lg">
                      {{ opportunity.title }}
                    </h3>
                    <div class="flex items-center space-x-3">
                      <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                            :class="getStageClass(opportunity.stage)">
                        {{ formatStage(opportunity.stage) }}
                      </span>
                      <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                            :class="getPriorityClass(opportunity.priority)">
                        {{ formatPriority(opportunity.priority) }}
                      </span>
                      <span class="text-sm text-gray-600 dark:text-gray-400">
                        {{ opportunity.probability }}% probability
                      </span>
                    </div>
                  </div>
                </div>
                
                <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Company:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      {{ opportunity.company }}<br>
                      {{ opportunity.contact.name }}
                    </div>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Value:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      R {{ opportunity.value.toLocaleString() }}<br>
                      Expected: {{ formatDate(opportunity.expectedCloseDate) }}
                    </div>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Source:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      {{ formatSource(opportunity.source) }}<br>
                      Assigned: {{ opportunity.assignedTo }}
                    </div>
                  </div>
                  
                  <div>
                    <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Activity:</span>
                    <div class="text-sm text-gray-600 dark:text-gray-400">
                      Last: {{ formatDaysAgo(opportunity.lastActivity) }}<br>
                      Next: {{ opportunity.nextActivity || 'Not scheduled' }}
                    </div>
                  </div>
                </div>
                
                <div v-if="opportunity.description" class="mt-3">
                  <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Description:</span>
                  <p class="text-sm text-gray-600 dark:text-gray-400 mt-1">
                    {{ opportunity.description }}
                  </p>
                </div>
                
                <div v-if="opportunity.products.length" class="mt-3">
                  <span class="text-sm font-medium text-gray-700 dark:text-gray-300">Products:</span>
                  <div class="flex flex-wrap gap-2 mt-1">
                    <span v-for="product in opportunity.products" :key="product" 
                          class="inline-flex items-center px-2 py-1 text-xs bg-blue-50 text-blue-700 rounded dark:bg-blue-900 dark:text-blue-300">
                      {{ product }}
                    </span>
                  </div>
                </div>
              </div>
              
              <div class="flex items-center space-x-2 ml-4">
                <button class="text-blue-600 hover:text-blue-800 text-sm px-3 py-1 border border-blue-200 rounded">
                  View
                </button>
                <button class="text-green-600 hover:text-green-800 text-sm px-3 py-1 border border-green-200 rounded">
                  Update
                </button>
                <button class="text-purple-600 hover:text-purple-800 text-sm px-3 py-1 border border-purple-200 rounded">
                  Activity
                </button>
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

useHead({
  title: 'CRM Opportunities - TOSS ERP'
})

// Pipeline stages
const pipelineStages = ref([
  {
    id: 'lead',
    name: 'Lead',
    opportunities: [
      { 
        id: 'opp-001', 
        title: 'New Spaza Network', 
        company: 'Alexandra Traders', 
        value: 45000, 
        probability: 20, 
        priority: 'medium',
        lastActivity: new Date('2024-01-10')
      },
      { 
        id: 'opp-002', 
        title: 'Stokvel Integration', 
        company: 'Tembisa Stokvel', 
        value: 23000, 
        probability: 15, 
        priority: 'low',
        lastActivity: new Date('2024-01-11')
      }
    ]
  },
  {
    id: 'qualified',
    name: 'Qualified',
    opportunities: [
      { 
        id: 'opp-003', 
        title: 'Township Logistics', 
        company: 'Soweto Distribution', 
        value: 125000, 
        probability: 60, 
        priority: 'high',
        lastActivity: new Date('2024-01-12')
      }
    ]
  },
  {
    id: 'proposal',
    name: 'Proposal',
    opportunities: [
      { 
        id: 'opp-004', 
        title: 'Group Buying Setup', 
        company: 'Orange Farm Co-op', 
        value: 78000, 
        probability: 75, 
        priority: 'high',
        lastActivity: new Date('2024-01-13')
      },
      { 
        id: 'opp-005', 
        title: 'Digital Payments', 
        company: 'Rural Market Hub', 
        value: 34000, 
        probability: 65, 
        priority: 'medium',
        lastActivity: new Date('2024-01-12')
      }
    ]
  },
  {
    id: 'negotiation',
    name: 'Negotiation',
    opportunities: [
      { 
        id: 'opp-006', 
        title: 'ERP Implementation', 
        company: 'Diepkloof Business Park', 
        value: 156000, 
        probability: 85, 
        priority: 'high',
        lastActivity: new Date('2024-01-14')
      }
    ]
  }
])

// Mock opportunities data
const mockOpportunities = ref([
  {
    id: 'opp-001',
    title: 'New Spaza Shop Network Integration',
    company: 'Alexandra Township Traders',
    stage: 'lead',
    priority: 'medium',
    value: 45000,
    probability: 20,
    contact: {
      name: 'Themba Mthembu',
      phone: '+27 82 345 6789',
      email: 'themba@alextraders.co.za'
    },
    source: 'referral',
    assignedTo: 'Sarah Johnson',
    createdDate: new Date('2024-01-08'),
    expectedCloseDate: new Date('2024-02-15'),
    lastActivity: new Date('2024-01-10'),
    nextActivity: 'Demo call scheduled',
    description: 'Looking to onboard 15 spaza shops in Alexandra to our platform for inventory management and group purchasing.',
    products: ['POS System', 'Inventory Management', 'Group Buying'],
    notes: 'Interested in digital payment integration'
  },
  {
    id: 'opp-002',
    title: 'Township Logistics Partnership',
    company: 'Soweto Distribution Company',
    stage: 'qualified',
    priority: 'high',
    value: 125000,
    probability: 60,
    contact: {
      name: 'Nomsa Dlamini',
      phone: '+27 76 789 0123',
      email: 'nomsa@sowetodist.co.za'
    },
    source: 'website',
    assignedTo: 'Mike Chen',
    createdDate: new Date('2024-01-05'),
    expectedCloseDate: new Date('2024-01-30'),
    lastActivity: new Date('2024-01-12'),
    nextActivity: 'Site visit planned',
    description: 'Major distributor wanting to integrate our delivery tracking and route optimization for township deliveries.',
    products: ['Logistics Management', 'Route Optimization', 'Delivery Tracking'],
    notes: 'Has existing relationship with 200+ spaza shops'
  },
  {
    id: 'opp-003',
    title: 'Stokvel Financial Services Integration',
    company: 'Tembisa Community Stokvel',
    stage: 'proposal',
    priority: 'high',
    value: 78000,
    probability: 75,
    contact: {
      name: 'Mpho Molefe',
      phone: '+27 72 123 4567',
      email: 'mpho@tembisakomstokvel.co.za'
    },
    source: 'cold-call',
    assignedTo: 'David Wilson',
    createdDate: new Date('2024-01-03'),
    expectedCloseDate: new Date('2024-01-25'),
    lastActivity: new Date('2024-01-13'),
    nextActivity: 'Contract review meeting',
    description: 'Large stokvel group wanting to implement group buying and financial tracking for 50+ members.',
    products: ['Stokvel Management', 'Group Buying', 'Financial Tracking'],
    notes: 'Decision makers meeting next week'
  },
  {
    id: 'opp-004',
    title: 'Complete ERP Implementation',
    company: 'Diepkloof Business Park',
    stage: 'negotiation',
    priority: 'high',
    value: 156000,
    probability: 85,
    contact: {
      name: 'Lucky Mahlangu',
      phone: '+27 83 456 7890',
      email: 'lucky@diepkloofbp.co.za'
    },
    source: 'partner',
    assignedTo: 'Sarah Johnson',
    createdDate: new Date('2023-12-20'),
    expectedCloseDate: new Date('2024-01-20'),
    lastActivity: new Date('2024-01-14'),
    nextActivity: 'Final pricing discussion',
    description: 'Business park management company wanting full ERP solution for 25 tenant businesses.',
    products: ['Full ERP Suite', 'Multi-tenant Setup', 'Training Package'],
    notes: 'Budget approved, finalizing contract terms'
  }
])

const getStageColor = (stage: string) => {
  const colorMap: Record<string, string> = {
    lead: 'bg-gradient-to-br from-gray-500 to-gray-600',
    qualified: 'bg-gradient-to-br from-blue-500 to-blue-600',
    proposal: 'bg-gradient-to-br from-purple-500 to-purple-600',
    negotiation: 'bg-gradient-to-br from-orange-500 to-orange-600',
    won: 'bg-gradient-to-br from-green-500 to-green-600',
    lost: 'bg-gradient-to-br from-red-500 to-red-600'
  }
  return colorMap[stage] || 'bg-gradient-to-br from-gray-500 to-gray-600'
}

const getStageClass = (stage: string) => {
  const classMap: Record<string, string> = {
    lead: 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300',
    qualified: 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    proposal: 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200',
    negotiation: 'bg-orange-100 text-orange-800 dark:bg-orange-900 dark:text-orange-200',
    won: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200',
    lost: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
  }
  return classMap[stage] || 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
}

const getPriorityClass = (priority: string) => {
  const classMap: Record<string, string> = {
    high: 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200',
    medium: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-200',
    low: 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
  }
  return classMap[priority] || 'bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300'
}

const getPriorityBorderClass = (priority: string) => {
  const classMap: Record<string, string> = {
    high: 'border-red-500',
    medium: 'border-yellow-500',
    low: 'border-green-500'
  }
  return classMap[priority] || 'border-gray-300'
}

const formatStage = (stage: string) => {
  const stageMap: Record<string, string> = {
    lead: 'Lead',
    qualified: 'Qualified',
    proposal: 'Proposal',
    negotiation: 'Negotiation',
    won: 'Won',
    lost: 'Lost'
  }
  return stageMap[stage] || stage
}

const formatPriority = (priority: string) => {
  const priorityMap: Record<string, string> = {
    high: 'High Priority',
    medium: 'Medium Priority',
    low: 'Low Priority'
  }
  return priorityMap[priority] || priority
}

const formatSource = (source: string) => {
  const sourceMap: Record<string, string> = {
    website: 'Website',
    referral: 'Referral',
    'cold-call': 'Cold Call',
    partner: 'Partner',
    'social-media': 'Social Media',
    advertising: 'Advertising'
  }
  return sourceMap[source] || source
}

const formatDate = (date: Date) => {
  return new Date(date).toLocaleDateString('en-ZA', { 
    year: 'numeric', 
    month: 'short', 
    day: 'numeric' 
  })
}

const formatDaysAgo = (date: Date) => {
  const now = new Date()
  const diffTime = Math.abs(now.getTime() - date.getTime())
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))
  
  if (diffDays === 0) return 'Today'
  if (diffDays === 1) return 'Yesterday'
  return `${diffDays} days ago`
}
</script>