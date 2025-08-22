<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Hero Section -->
    <div class="relative bg-gradient-to-br from-indigo-900 via-purple-900 to-pink-900 dark:from-gray-900 dark:via-gray-800 dark:to-gray-900">
      <div class="absolute inset-0 bg-black opacity-50"></div>
      <div class="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-24">
        <div class="text-center">
          <h1 class="text-4xl md:text-6xl font-bold text-white mb-6">
            Service-as-a-Software
          </h1>
          <p class="text-xl md:text-2xl text-gray-300 mb-8 max-w-4xl mx-auto">
            Revolutionary AI-powered business automation that delivers outcomes, not just tools. 
            Let intelligent agents run your township business while you focus on growth.
          </p>
          <div class="flex flex-col sm:flex-row gap-4 justify-center">
            <button
              @click="scrollToSection('how-it-works')"
              class="inline-flex items-center px-8 py-4 border border-transparent text-lg font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 transition-colors"
            >
              <svg class="-ml-1 mr-3 h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
              </svg>
              See How It Works
            </button>
            <button
              @click="scrollToSection('pricing')"
              class="inline-flex items-center px-8 py-4 border border-white text-lg font-medium rounded-md text-white hover:bg-white hover:text-gray-900 transition-colors"
            >
              <svg class="-ml-1 mr-3 h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              View Pricing
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Problem Section -->
    <div class="py-16 bg-white dark:bg-gray-800">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-16">
          <h2 class="text-3xl md:text-4xl font-bold text-gray-900 dark:text-white mb-6">
            The Challenge for Township Businesses
          </h2>
          <p class="text-xl text-gray-600 dark:text-gray-300 max-w-3xl mx-auto">
            Small and medium businesses in townships struggle with manual processes, limited resources, 
            and the complexity of traditional software solutions.
          </p>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
          <div v-for="problem in businessProblems" :key="problem.id" class="text-center p-6 bg-red-50 dark:bg-red-900/20 rounded-lg border border-red-200 dark:border-red-800">
            <div class="w-16 h-16 bg-red-500 rounded-full flex items-center justify-center mx-auto mb-4">
              <component :is="problem.icon" class="w-8 h-8 text-white" />
            </div>
            <h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-3">{{ problem.title }}</h3>
            <p class="text-gray-600 dark:text-gray-300">{{ problem.description }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Solution Section -->
    <div id="how-it-works" class="py-16 bg-gray-50 dark:bg-gray-900">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-16">
          <h2 class="text-3xl md:text-4xl font-bold text-gray-900 dark:text-white mb-6">
            How Service-as-a-Software Works
          </h2>
          <p class="text-xl text-gray-600 dark:text-gray-300 max-w-3xl mx-auto">
            Instead of learning complex software, you simply tell our AI agents what business outcomes you need. 
            They handle everything autonomously in the background.
          </p>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-2 gap-12 items-center mb-16">
          <div>
            <h3 class="text-2xl font-bold text-gray-900 dark:text-white mb-6">Traditional Software vs Service-as-a-Software</h3>
            <div class="space-y-6">
              <div class="flex items-start space-x-4">
                <div class="w-8 h-8 bg-red-500 rounded-full flex items-center justify-center flex-shrink-0">
                  <svg class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
                  </svg>
                </div>
                <div>
                  <h4 class="font-semibold text-gray-900 dark:text-white">Traditional Software</h4>
                  <p class="text-gray-600 dark:text-gray-300">You learn the software, navigate complex interfaces, and manually execute processes</p>
                </div>
              </div>
              
              <div class="flex items-start space-x-4">
                <div class="w-8 h-8 bg-green-500 rounded-full flex items-center justify-center flex-shrink-0">
                  <svg class="w-4 h-4 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                  </svg>
                </div>
                <div>
                  <h4 class="font-semibold text-gray-900 dark:text-white">Service-as-a-Software</h4>
                  <p class="text-gray-600 dark:text-gray-300">AI agents understand your needs and deliver business outcomes automatically</p>
                </div>
              </div>
            </div>
          </div>
          
          <div class="bg-white dark:bg-gray-800 rounded-lg shadow-lg p-8">
            <h4 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Example: Processing Customer Orders</h4>
            <div class="space-y-3">
              <div v-for="(step, index) in processSteps" :key="index" class="flex items-center space-x-3">
                <div class="w-6 h-6 bg-indigo-500 rounded-full flex items-center justify-center text-white text-xs font-bold">
                  {{ index + 1 }}
                </div>
                <span class="text-gray-700 dark:text-gray-300">{{ step }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- AI Agents Showcase -->
        <div class="mb-16">
          <h3 class="text-2xl font-bold text-center text-gray-900 dark:text-white mb-12">Meet Your AI Business Team</h3>
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            <div v-for="agent in aiAgents" :key="agent.id" class="bg-white dark:bg-gray-800 rounded-lg shadow p-6 text-center">
              <div class="w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4" :class="agent.iconBg">
                <component :is="agent.icon" class="w-8 h-8 text-white" />
              </div>
              <h4 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">{{ agent.name }}</h4>
              <p class="text-sm text-gray-600 dark:text-gray-300 mb-3">{{ agent.description }}</p>
              <div class="text-xs text-green-600 dark:text-green-400 font-medium">
                ✓ {{ agent.automation }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Benefits Section -->
    <div class="py-16 bg-white dark:bg-gray-800">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-16">
          <h2 class="text-3xl md:text-4xl font-bold text-gray-900 dark:text-white mb-6">
            Transformative Benefits for Your Business
          </h2>
          <p class="text-xl text-gray-600 dark:text-gray-300 max-w-3xl mx-auto">
            Experience unprecedented growth and efficiency with AI agents handling your business operations
          </p>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <div v-for="benefit in businessBenefits" :key="benefit.id" class="bg-gradient-to-br from-indigo-50 to-purple-50 dark:from-indigo-900/20 dark:to-purple-900/20 rounded-lg p-8 border border-indigo-200 dark:border-indigo-800">
            <div class="w-12 h-12 rounded-lg flex items-center justify-center mb-6" :class="benefit.iconBg">
              <component :is="benefit.icon" class="w-6 h-6 text-white" />
            </div>
            <h3 class="text-xl font-bold text-gray-900 dark:text-white mb-4">{{ benefit.title }}</h3>
            <p class="text-gray-600 dark:text-gray-300 mb-6">{{ benefit.description }}</p>
            <div class="space-y-2">
              <div v-for="metric in benefit.metrics" :key="metric" class="flex items-center text-sm text-gray-700 dark:text-gray-300">
                <svg class="w-4 h-4 text-green-500 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                </svg>
                {{ metric }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Use Cases Section -->
    <div class="py-16 bg-gray-50 dark:bg-gray-900">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-16">
          <h2 class="text-3xl md:text-4xl font-bold text-gray-900 dark:text-white mb-6">
            Perfect for Township Businesses
          </h2>
          <p class="text-xl text-gray-600 dark:text-gray-300 max-w-3xl mx-auto">
            See how Service-as-a-Software transforms different types of businesses in townships
          </p>
        </div>

        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
          <div v-for="useCase in useCases" :key="useCase.id" class="bg-white dark:bg-gray-800 rounded-lg shadow overflow-hidden">
            <div class="p-6">
              <div class="w-12 h-12 rounded-lg flex items-center justify-center mb-4" :class="useCase.iconBg">
                <component :is="useCase.icon" class="w-6 h-6 text-white" />
              </div>
              <h3 class="text-xl font-semibold text-gray-900 dark:text-white mb-3">{{ useCase.title }}</h3>
              <p class="text-gray-600 dark:text-gray-300 mb-4">{{ useCase.description }}</p>
              
              <div class="space-y-2">
                <h4 class="text-sm font-medium text-gray-900 dark:text-white">AI Handles:</h4>
                <ul class="space-y-1">
                  <li v-for="task in useCase.automatedTasks" :key="task" class="flex items-center text-sm text-gray-600 dark:text-gray-300">
                    <svg class="w-3 h-3 text-green-500 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                    </svg>
                    {{ task }}
                  </li>
                </ul>
              </div>
            </div>
            
            <div class="bg-gray-50 dark:bg-gray-700 px-6 py-4">
              <div class="flex justify-between items-center">
                <span class="text-sm font-medium text-gray-900 dark:text-white">ROI Impact</span>
                <span class="text-lg font-bold text-green-600 dark:text-green-400">{{ useCase.roi }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Pricing Section -->
    <div id="pricing" class="py-16 bg-white dark:bg-gray-800">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="text-center mb-16">
          <h2 class="text-3xl md:text-4xl font-bold text-gray-900 dark:text-white mb-6">
            Outcome-Based Pricing
          </h2>
          <p class="text-xl text-gray-600 dark:text-gray-300 max-w-3xl mx-auto">
            Pay for results, not software licenses. Our pricing scales with the value we deliver to your business.
          </p>
        </div>

        <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
          <div v-for="plan in pricingPlans" :key="plan.id" class="relative bg-white dark:bg-gray-700 rounded-lg shadow-lg border-2" :class="plan.featured ? 'border-indigo-500' : 'border-gray-200 dark:border-gray-600'">
            <div v-if="plan.featured" class="absolute top-0 left-1/2 transform -translate-x-1/2 -translate-y-1/2">
              <span class="bg-indigo-500 text-white px-4 py-1 text-sm font-medium rounded-full">Most Popular</span>
            </div>
            
            <div class="p-8">
              <h3 class="text-2xl font-bold text-gray-900 dark:text-white mb-4">{{ plan.name }}</h3>
              <p class="text-gray-600 dark:text-gray-300 mb-6">{{ plan.description }}</p>
              
              <div class="mb-6">
                <span class="text-4xl font-bold text-gray-900 dark:text-white">{{ plan.price }}</span>
                <span class="text-gray-600 dark:text-gray-300">{{ plan.period }}</span>
              </div>
              
              <ul class="space-y-3 mb-8">
                <li v-for="feature in plan.features" :key="feature" class="flex items-center text-gray-700 dark:text-gray-300">
                  <svg class="w-5 h-5 text-green-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
                  </svg>
                  {{ feature }}
                </li>
              </ul>
              
              <button
                @click="selectPlan(plan)"
                class="w-full py-3 px-6 rounded-md font-medium transition-colors"
                :class="plan.featured ? 'bg-indigo-600 hover:bg-indigo-700 text-white' : 'bg-gray-100 dark:bg-gray-600 hover:bg-gray-200 dark:hover:bg-gray-500 text-gray-900 dark:text-white'"
              >
                {{ plan.cta }}
              </button>
            </div>
          </div>
        </div>

        <div class="mt-12 text-center">
          <p class="text-lg text-gray-600 dark:text-gray-300 mb-4">
            All plans include 24/7 AI monitoring, automatic updates, and outcome guarantees
          </p>
          <button
            @click="scheduleDemo"
            class="inline-flex items-center px-6 py-3 border border-transparent text-lg font-medium rounded-md text-indigo-600 dark:text-indigo-400 bg-indigo-50 dark:bg-indigo-900/20 hover:bg-indigo-100 dark:hover:bg-indigo-900/30 transition-colors"
          >
            <svg class="-ml-1 mr-2 h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
            </svg>
            Schedule Free Consultation
          </button>
        </div>
      </div>
    </div>

    <!-- CTA Section -->
    <div class="py-16 bg-gradient-to-r from-indigo-600 to-purple-600">
      <div class="max-w-4xl mx-auto text-center px-4 sm:px-6 lg:px-8">
        <h2 class="text-3xl md:text-4xl font-bold text-white mb-6">
          Ready to Transform Your Business?
        </h2>
        <p class="text-xl text-indigo-100 mb-8">
          Join hundreds of township businesses already using Service-as-a-Software to achieve extraordinary results
        </p>
        <div class="flex flex-col sm:flex-row gap-4 justify-center">
          <button
            @click="startTrial"
            class="inline-flex items-center px-8 py-4 border border-transparent text-lg font-medium rounded-md text-indigo-600 bg-white hover:bg-gray-50 transition-colors"
          >
            <svg class="-ml-1 mr-3 h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
            </svg>
            Start Free Trial
          </button>
          <button
            @click="contactSales"
            class="inline-flex items-center px-8 py-4 border border-white text-lg font-medium rounded-md text-white hover:bg-white hover:text-indigo-600 transition-colors"
          >
            <svg class="-ml-1 mr-3 h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
            </svg>
            Contact Sales
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

// Mock Nuxt functions
function definePageMeta(meta: any) {}
function useHead(options: any) {
  if (typeof document !== 'undefined') {
    document.title = options.title || 'Service-as-a-Software - TOSS ERP'
  }
}

// Business problems
const businessProblems = ref([
  {
    id: 1,
    title: 'Manual Processes',
    description: 'Spending hours on paperwork, data entry, and repetitive tasks that could be automated',
    icon: 'clock-icon'
  },
  {
    id: 2,
    title: 'Limited Resources',
    description: 'Cannot afford expensive software licenses or dedicated IT staff to manage complex systems',
    icon: 'money-icon'
  },
  {
    id: 3,
    title: 'Learning Curve',
    description: 'Traditional software requires extensive training and technical knowledge to use effectively',
    icon: 'academic-cap-icon'
  }
])

// Process steps
const processSteps = ref([
  'Customer places order via WhatsApp',
  'AI Agent validates order and checks inventory',
  'Payment is processed automatically',
  'Invoice is generated and sent to customer',
  'Order is dispatched with tracking info',
  'Customer receives automated updates'
])

// AI Agents
const aiAgents = ref([
  {
    id: 1,
    name: 'Sales Agent',
    description: 'Handles customer inquiries, processes orders, and manages invoicing',
    automation: '100% Autonomous',
    icon: 'chart-icon',
    iconBg: 'bg-green-500'
  },
  {
    id: 2,
    name: 'Inventory Agent',
    description: 'Tracks stock levels, predicts demand, and manages supplier orders',
    automation: '24/7 Monitoring',
    icon: 'box-icon',
    iconBg: 'bg-blue-500'
  },
  {
    id: 3,
    name: 'Finance Agent',
    description: 'Processes payments, reconciles accounts, and generates financial reports',
    automation: 'Real-time Processing',
    icon: 'calculator-icon',
    iconBg: 'bg-purple-500'
  },
  {
    id: 4,
    name: 'Customer Agent',
    description: 'Provides 24/7 customer support and handles service requests',
    automation: 'Instant Response',
    icon: 'chat-icon',
    iconBg: 'bg-yellow-500'
  }
])

// Business benefits
const businessBenefits = ref([
  {
    id: 1,
    title: 'Dramatic Cost Reduction',
    description: 'Eliminate manual labor costs and reduce operational overhead by up to 70%',
    icon: 'trending-down-icon',
    iconBg: 'bg-green-500',
    metrics: [
      'Save 40+ hours per week on admin tasks',
      'Reduce staffing costs by 60%',
      'Eliminate costly software licenses'
    ]
  },
  {
    id: 2,
    title: 'Exponential Revenue Growth',
    description: 'AI agents work 24/7 to capture every sales opportunity and optimize pricing',
    icon: 'trending-up-icon',
    iconBg: 'bg-blue-500',
    metrics: [
      'Increase sales by 200-400%',
      'Never miss a customer inquiry',
      'Optimize pricing for maximum profit'
    ]
  },
  {
    id: 3,
    title: 'Perfect Accuracy & Compliance',
    description: 'Eliminate human errors and ensure consistent compliance with regulations',
    icon: 'shield-check-icon',
    iconBg: 'bg-purple-500',
    metrics: [
      '99.9% processing accuracy',
      'Automatic regulatory compliance',
      'Reduced audit risks'
    ]
  }
])

// Use cases
const useCases = ref([
  {
    id: 1,
    title: 'General Store',
    description: 'Complete automation of inventory, sales, and customer management',
    icon: 'store-icon',
    iconBg: 'bg-green-500',
    automatedTasks: [
      'Stock level monitoring',
      'Automatic reordering',
      'Price optimization',
      'Customer loyalty programs'
    ],
    roi: '+250% Revenue'
  },
  {
    id: 2,
    title: 'Restaurant/Tavern',
    description: 'Streamlined order management, inventory control, and customer service',
    icon: 'utensils-icon',
    iconBg: 'bg-orange-500',
    automatedTasks: [
      'Order processing',
      'Inventory tracking',
      'Menu optimization',
      'Customer feedback analysis'
    ],
    roi: '+180% Profit'
  },
  {
    id: 3,
    title: 'Auto Repair Shop',
    description: 'Intelligent scheduling, parts management, and customer communication',
    icon: 'wrench-icon',
    iconBg: 'bg-blue-500',
    automatedTasks: [
      'Appointment scheduling',
      'Parts inventory',
      'Customer updates',
      'Invoice generation'
    ],
    roi: '+320% Efficiency'
  },
  {
    id: 4,
    title: 'Hair Salon',
    description: 'Automated booking, customer management, and inventory control',
    icon: 'scissors-icon',
    iconBg: 'bg-pink-500',
    automatedTasks: [
      'Online booking',
      'Customer reminders',
      'Product tracking',
      'Staff scheduling'
    ],
    roi: '+150% Bookings'
  },
  {
    id: 5,
    title: 'Hardware Store',
    description: 'Smart inventory management and customer service automation',
    icon: 'hammer-icon',
    iconBg: 'bg-gray-500',
    automatedTasks: [
      'Stock management',
      'Supplier coordination',
      'Customer quotes',
      'Delivery scheduling'
    ],
    roi: '+200% Turnover'
  },
  {
    id: 6,
    title: 'Mobile Services',
    description: 'Complete business automation for mobile service providers',
    icon: 'phone-icon',
    iconBg: 'bg-indigo-500',
    automatedTasks: [
      'Service requests',
      'Route optimization',
      'Customer communication',
      'Payment processing'
    ],
    roi: '+400% Coverage'
  }
])

// Pricing plans
const pricingPlans = ref([
  {
    id: 1,
    name: 'Starter',
    description: 'Perfect for small township businesses just getting started',
    price: 'R299',
    period: '/month',
    featured: false,
    cta: 'Start Free Trial',
    features: [
      '1 AI Agent (Sales or Inventory)',
      'Up to 100 transactions/month',
      'Basic automation',
      'WhatsApp integration',
      'Email support'
    ]
  },
  {
    id: 2,
    name: 'Growth',
    description: 'Ideal for growing businesses ready to scale operations',
    price: 'R599',
    period: '/month',
    featured: true,
    cta: 'Start Free Trial',
    features: [
      '3 AI Agents (Sales, Inventory, Finance)',
      'Up to 1,000 transactions/month',
      'Advanced automation',
      'Multi-channel integration',
      'Priority support',
      'Custom reporting'
    ]
  },
  {
    id: 3,
    name: 'Enterprise',
    description: 'Complete AI-powered business transformation',
    price: 'R1,299',
    period: '/month',
    featured: false,
    cta: 'Contact Sales',
    features: [
      'Unlimited AI Agents',
      'Unlimited transactions',
      'Full business automation',
      'All integrations',
      '24/7 dedicated support',
      'Custom AI training',
      'ROI guarantee'
    ]
  }
])

// Methods
const scrollToSection = (sectionId: string) => {
  const element = document.getElementById(sectionId)
  if (element) {
    element.scrollIntoView({ behavior: 'smooth' })
  }
}

const selectPlan = (plan: any) => {
  console.log(`Selected plan: ${plan.name}`)
  // Implementation would handle plan selection
}

const scheduleDemo = () => {
  console.log('Scheduling demo...')
  // Implementation would open demo scheduling
}

const startTrial = () => {
  console.log('Starting free trial...')
  // Implementation would start trial signup
}

const contactSales = () => {
  console.log('Contacting sales...')
  // Implementation would open contact form
}

// Page meta
definePageMeta({
  title: 'Service-as-a-Software',
  description: 'Revolutionary AI-powered business automation for township enterprises'
})

// SEO
useHead({
  title: 'Service-as-a-Software - TOSS ERP',
  meta: [
    { name: 'description', content: 'Revolutionary AI-powered business automation that delivers outcomes, not just tools. Perfect for township businesses seeking growth and efficiency.' }
  ]
})
</script>

<style scoped>
/* Smooth scroll behavior */
html {
  scroll-behavior: smooth;
}

/* Gradient backgrounds */
.bg-gradient-to-br {
  background-image: linear-gradient(to bottom right, var(--tw-gradient-stops));
}

.bg-gradient-to-r {
  background-image: linear-gradient(to right, var(--tw-gradient-stops));
}

/* Custom animations */
@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fadeInUp {
  animation: fadeInUp 0.6s ease-out;
}

/* Transition effects */
.transition-colors {
  transition-property: color, background-color, border-color, text-decoration-color, fill, stroke;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 300ms;
}

.transition-transform {
  transition-property: transform;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 300ms;
}

/* Hover effects */
.hover-lift:hover {
  transform: translateY(-4px);
}

/* Card shadows */
.shadow-lg {
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
}

.dark .shadow-lg {
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.3), 0 4px 6px -2px rgba(0, 0, 0, 0.2);
}

/* Responsive text sizing */
@media (max-width: 640px) {
  .text-4xl {
    font-size: 2.25rem;
    line-height: 2.5rem;
  }
  
  .text-6xl {
    font-size: 3rem;
    line-height: 1;
  }
}
</style>
