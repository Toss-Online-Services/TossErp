<template>
  <div class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center p-4 z-50">
    <div class="bg-white dark:bg-slate-800 rounded-lg shadow-xl max-w-2xl w-full max-h-[80vh] overflow-hidden">
      <!-- Modal Header -->
      <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700 flex items-center justify-between">
        <div>
          <h3 class="text-lg font-medium text-slate-900 dark:text-white">
            Template Library
          </h3>
          <p class="text-sm text-slate-600 dark:text-slate-400">
            Choose from pre-built automation templates for South African township enterprises
          </p>
        </div>
        <button
          @click="$emit('close')"
          class="text-slate-400 hover:text-slate-600 dark:hover:text-slate-300"
        >
          <XMarkIcon class="h-6 w-6" />
        </button>
      </div>

      <!-- Template Categories -->
      <div class="flex border-b border-slate-200 dark:border-slate-700">
        <button
          v-for="category in categories"
          :key="category.id"
          @click="selectedCategory = category.id"
          :class="[
            'px-6 py-3 text-sm font-medium border-b-2 transition-colors',
            selectedCategory === category.id
              ? 'border-blue-500 text-blue-600 dark:text-blue-400 bg-blue-50 dark:bg-blue-900/20'
              : 'border-transparent text-slate-500 dark:text-slate-400 hover:text-slate-700 dark:hover:text-slate-300'
          ]"
        >
          {{ category.name }}
        </button>
      </div>

      <!-- Template Grid -->
      <div class="p-6 overflow-y-auto max-h-[60vh]">
        <div class="grid grid-cols-1 gap-4">
          <div
            v-for="(template, key) in filteredTemplates"
            :key="key"
            class="border border-slate-200 dark:border-slate-700 rounded-lg p-4 hover:border-blue-300 dark:hover:border-blue-600 transition-colors cursor-pointer"
            @click="selectTemplate(key, template)"
          >
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <h4 class="text-lg font-medium text-slate-900 dark:text-white">{{ template.name }}</h4>
                <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">{{ template.description }}</p>
                
                <!-- Enterprise Types -->
                <div class="mt-3">
                  <h5 class="text-xs font-medium text-slate-700 dark:text-slate-300 mb-1">Applicable To:</h5>
                  <div class="flex flex-wrap gap-1">
                    <span
                      v-for="enterpriseType in template.enterpriseTypes.slice(0, 3)"
                      :key="enterpriseType"
                      class="inline-flex items-center px-2 py-1 rounded-md text-xs font-medium bg-blue-100 text-blue-800 dark:bg-blue-900/20 dark:text-blue-400"
                    >
                      {{ enterpriseType }}
                    </span>
                    <span
                      v-if="template.enterpriseTypes.length > 3"
                      class="inline-flex items-center px-2 py-1 rounded-md text-xs font-medium bg-slate-100 text-slate-800 dark:bg-slate-700 dark:text-slate-300"
                    >
                      +{{ template.enterpriseTypes.length - 3 }} more
                    </span>
                  </div>
                </div>

                <!-- Actions Preview -->
                <div class="mt-3">
                  <h5 class="text-xs font-medium text-slate-700 dark:text-slate-300 mb-1">
                    Automation Steps ({{ template.actions.length }}):
                  </h5>
                  <div class="space-y-1">
                    <div
                      v-for="(action, index) in template.actions.slice(0, 2)"
                      :key="index"
                      class="text-xs text-slate-600 dark:text-slate-400"
                    >
                      {{ index + 1 }}. {{ action.description }}
                    </div>
                    <div
                      v-if="template.actions.length > 2"
                      class="text-xs text-slate-500 dark:text-slate-400"
                    >
                      ... and {{ template.actions.length - 2 }} more steps
                    </div>
                  </div>
                </div>
              </div>
              
              <ChevronRightIcon class="h-5 w-5 text-slate-400 ml-4" />
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div v-if="Object.keys(filteredTemplates).length === 0" class="text-center py-8">
          <DocumentTextIcon class="mx-auto h-12 w-12 text-slate-400" />
          <h3 class="mt-2 text-sm font-medium text-slate-900 dark:text-white">No templates found</h3>
          <p class="mt-1 text-sm text-slate-500 dark:text-slate-400">
            Try selecting a different category or check back later for more templates.
          </p>
        </div>
      </div>

      <!-- Modal Footer -->
      <div class="px-6 py-4 border-t border-slate-200 dark:border-slate-700 flex justify-end">
        <button
          @click="$emit('close')"
          class="px-4 py-2 text-sm font-medium text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-700 border border-slate-300 dark:border-slate-600 rounded-md hover:bg-slate-50 dark:hover:bg-slate-600"
        >
          Close
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import {
  XMarkIcon,
  ChevronRightIcon,
  DocumentTextIcon
} from '@heroicons/vue/24/outline'

interface TemplateAction {
  type: string
  description: string
}

interface AutomationTemplate {
  name: string
  description: string
  category: string
  enterpriseTypes: string[]
  actions: TemplateAction[]
}

const emit = defineEmits<{
  close: []
  select: [templateKey: string, template: AutomationTemplate]
}>()

// Component state
const selectedCategory = ref('retail')

// Template categories
const categories = [
  { id: 'retail', name: 'Retail & Shops' },
  { id: 'services', name: 'Professional Services' },
  { id: 'food', name: 'Food & Hospitality' },
  { id: 'agriculture', name: 'Agriculture' },
  { id: 'transport', name: 'Transport & Logistics' },
  { id: 'all', name: 'All Templates' }
]

// Comprehensive automation templates for South African township enterprises
const templates: Record<string, AutomationTemplate> = {
  // Retail & Shops
  spaza_shop_onboarding: {
    name: 'Spaza Shop Onboarding',
    description: 'Welcome new spaza shop owners with inventory management tips and supplier connections',
    category: 'retail',
    enterpriseTypes: ['Spaza / Convenience shop'],
    actions: [
      { type: 'send_email', description: 'Send welcome email with inventory management guide' },
      { type: 'send_sms', description: 'Share supplier contact list and wholesale market info' },
      { type: 'create_task', description: 'Schedule initial business consultation call' },
      { type: 'add_tag', description: 'Add "new_spaza_owner" tag for future targeting' }
    ]
  },
  
  hair_salon_growth: {
    name: 'Hair Salon Business Growth',
    description: 'Help salon owners expand their client base and improve service quality',
    category: 'retail',
    enterpriseTypes: ['Hair salon', 'Beauty salon'],
    actions: [
      { type: 'send_email', description: 'Share customer retention strategies for salons' },
      { type: 'send_whatsapp', description: 'Send appointment booking template and pricing guide' },
      { type: 'create_task', description: 'Schedule marketing strategy consultation' },
      { type: 'send_sms', description: 'Share tips for social media promotion' }
    ]
  },

  // Professional Services
  electrician_lead_nurture: {
    name: 'Electrician Lead Nurturing',
    description: 'Convert leads for electrical services with trust-building and safety messaging',
    category: 'services',
    enterpriseTypes: ['Electricians'],
    actions: [
      { type: 'send_email', description: 'Share electrical safety tips and certifications' },
      { type: 'send_sms', description: 'Provide emergency contact info and service areas' },
      { type: 'create_task', description: 'Schedule site visit for estimate' },
      { type: 'add_tag', description: 'Tag based on service type (residential/commercial)' }
    ]
  },

  plumber_emergency_response: {
    name: 'Plumber Emergency Response',
    description: 'Quick response system for plumbing emergencies with follow-up care',
    category: 'services',
    enterpriseTypes: ['Plumbers'],
    actions: [
      { type: 'send_sms', description: 'Confirm emergency call received and ETA' },
      { type: 'create_task', description: 'Dispatch plumber to location' },
      { type: 'send_email', description: 'Follow-up with maintenance tips' },
      { type: 'send_sms', description: 'Request feedback and future service needs' }
    ]
  },

  carpenter_project_workflow: {
    name: 'Carpenter Project Management',
    description: 'Manage custom carpentry projects from quote to completion',
    category: 'services',
    enterpriseTypes: ['Carpenters & joiners'],
    actions: [
      { type: 'send_email', description: 'Send project proposal and timeline' },
      { type: 'create_task', description: 'Schedule material procurement' },
      { type: 'send_sms', description: 'Project milestone updates' },
      { type: 'send_email', description: 'Final project delivery and care instructions' }
    ]
  },

  // Food & Hospitality
  fast_food_customer_loyalty: {
    name: 'Fast Food Customer Loyalty',
    description: 'Build repeat customers for takeaway and fast food businesses',
    category: 'food',
    enterpriseTypes: ['Fast food / Takeaway'],
    actions: [
      { type: 'send_sms', description: 'Welcome new customer with menu highlights' },
      { type: 'send_whatsapp', description: 'Share daily specials and promotions' },
      { type: 'add_tag', description: 'Track favorite orders for personalization' },
      { type: 'send_email', description: 'Monthly loyalty rewards and special offers' }
    ]
  },

  restaurant_review_management: {
    name: 'Restaurant Review Management',
    description: 'Encourage positive reviews and handle feedback professionally',
    category: 'food',
    enterpriseTypes: ['Restaurant', 'Fast food / Takeaway'],
    actions: [
      { type: 'send_sms', description: 'Thank customer for visit and request review' },
      { type: 'create_task', description: 'Follow up on any service issues mentioned' },
      { type: 'send_email', description: 'Share how feedback helps improve service' },
      { type: 'add_tag', description: 'Tag as reviewer for special recognition' }
    ]
  },

  // Agriculture
  vegetable_farming_seasonal: {
    name: 'Vegetable Farming Seasonal Support',
    description: 'Provide seasonal farming tips and market price updates',
    category: 'agriculture',
    enterpriseTypes: ['Small-scale vegetable gardening'],
    actions: [
      { type: 'send_email', description: 'Seasonal planting calendar and crop rotation tips' },
      { type: 'send_sms', description: 'Weekly market prices for fresh produce' },
      { type: 'create_task', description: 'Schedule agricultural extension visit' },
      { type: 'send_whatsapp', description: 'Weather alerts and farming best practices' }
    ]
  },

  poultry_farming_health: {
    name: 'Poultry Health Monitoring',
    description: 'Monitor chicken health and optimize egg/meat production',
    category: 'agriculture',
    enterpriseTypes: ['Poultry (chicken) farming'],
    actions: [
      { type: 'send_email', description: 'Poultry health checklist and vaccination schedule' },
      { type: 'send_sms', description: 'Feed optimization tips for better production' },
      { type: 'create_task', description: 'Schedule veterinary consultation if needed' },
      { type: 'add_tag', description: 'Track production levels and health status' }
    ]
  },

  // Transport & Logistics
  taxi_driver_efficiency: {
    name: 'Taxi Driver Route Optimization',
    description: 'Help taxi drivers optimize routes and improve passenger satisfaction',
    category: 'transport',
    enterpriseTypes: ['Taxis / Transport services'],
    actions: [
      { type: 'send_sms', description: 'Daily route suggestions based on traffic patterns' },
      { type: 'send_whatsapp', description: 'Share fuel-saving driving tips' },
      { type: 'create_task', description: 'Vehicle maintenance reminder scheduling' },
      { type: 'add_tag', description: 'Track preferred routes and peak hours' }
    ]
  },

  delivery_service_tracking: {
    name: 'Delivery Service Customer Updates',
    description: 'Keep customers informed about delivery status and build trust',
    category: 'transport',
    enterpriseTypes: ['Delivery services', 'Taxis / Transport services'],
    actions: [
      { type: 'send_sms', description: 'Pickup confirmation and estimated delivery time' },
      { type: 'send_whatsapp', description: 'Real-time delivery status updates' },
      { type: 'send_sms', description: 'Delivery completion confirmation' },
      { type: 'send_email', description: 'Request feedback and offer future services' }
    ]
  }
}

// Computed filtered templates
const filteredTemplates = computed(() => {
  if (selectedCategory.value === 'all') {
    return templates
  }
  
  return Object.fromEntries(
    Object.entries(templates).filter(([_, template]) => 
      template.category === selectedCategory.value
    )
  )
})

// Methods
const selectTemplate = (templateKey: string, template: AutomationTemplate) => {
  emit('select', templateKey, template)
}
</script>
