import { ref, computed } from 'vue'

// Enterprise-specific configuration for SaaS tenants
export interface EnterpriseConfig {
  id: string
  name: string
  type: string
  sector: string
  customFields: CustomField[]
  contactTypes: ContactType[]
  serviceOfferings: ServiceOffering[]
  automationTemplates: string[]
  dashboardWidgets: DashboardWidget[]
  businessHours: BusinessHours
  contactStages: ContactStage[]
  pricing: PricingTier[]
}

export interface CustomField {
  id: string
  name: string
  type: 'text' | 'number' | 'select' | 'multiselect' | 'date' | 'phone' | 'email'
  required: boolean
  options?: string[]
  placeholder?: string
  validation?: string
}

export interface ContactType {
  id: string
  name: string
  description: string
  icon: string
  color: string
  defaultStage: string
}

export interface ServiceOffering {
  id: string
  name: string
  description: string
  category: string
  estimatedDuration: number
  basePrice: number
  currency: string
}

export interface DashboardWidget {
  id: string
  title: string
  type: 'metric' | 'chart' | 'list' | 'calendar'
  size: 'small' | 'medium' | 'large'
  position: number
}

export interface BusinessHours {
  monday: { open: string; close: string; closed: boolean }
  tuesday: { open: string; close: string; closed: boolean }
  wednesday: { open: string; close: string; closed: boolean }
  thursday: { open: string; close: string; closed: boolean }
  friday: { open: string; close: string; closed: boolean }
  saturday: { open: string; close: string; closed: boolean }
  sunday: { open: string; close: string; closed: boolean }
}

export interface ContactStage {
  id: string
  name: string
  description: string
  color: string
  order: number
  automationTriggers?: string[]
}

export interface PricingTier {
  id: string
  name: string
  description: string
  basePrice: number
  currency: string
  features: string[]
}

export const useEnterpriseConfig = () => {
  const currentEnterprise = ref<EnterpriseConfig | null>(null)

  // Predefined enterprise configurations
  const enterpriseConfigs: Record<string, EnterpriseConfig> = {
    beauty_salon: {
      id: 'beauty_salon',
      name: 'Hair & Beauty Salon',
      type: 'Hair salon',
      sector: 'Beauty & Personal Care',
      customFields: [
        { id: 'hair_type', name: 'Hair Type', type: 'select', required: false, options: ['Straight', 'Wavy', 'Curly', 'Coily', 'Relaxed'] },
        { id: 'preferred_stylist', name: 'Preferred Stylist', type: 'select', required: false, options: ['Sarah', 'Thando', 'Nomsa', 'Lerato'] },
        { id: 'allergies', name: 'Allergies/Sensitivities', type: 'text', required: false, placeholder: 'Any chemical allergies or sensitivities' },
        { id: 'last_visit', name: 'Last Visit Date', type: 'date', required: false },
        { id: 'loyalty_points', name: 'Loyalty Points', type: 'number', required: false },
        { id: 'preferred_appointment_time', name: 'Preferred Time', type: 'select', required: false, options: ['Morning (8-12)', 'Afternoon (12-17)', 'Evening (17-20)'] }
      ],
      contactTypes: [
        { id: 'regular_client', name: 'Regular Client', description: 'Returning customers with appointment history', icon: 'UserIcon', color: 'blue', defaultStage: 'active' },
        { id: 'new_client', name: 'New Client', description: 'First-time customers', icon: 'SparklesIcon', color: 'green', defaultStage: 'consultation' },
        { id: 'vip_client', name: 'VIP Client', description: 'High-value or frequent customers', icon: 'StarIcon', color: 'gold', defaultStage: 'priority' },
        { id: 'bridal_client', name: 'Bridal Client', description: 'Wedding and special event customers', icon: 'HeartIcon', color: 'pink', defaultStage: 'planning' }
      ],
      serviceOfferings: [
        { id: 'wash_and_set', name: 'Wash & Set', description: 'Basic hair wash and styling', category: 'Basic Services', estimatedDuration: 90, basePrice: 80, currency: 'ZAR' },
        { id: 'relaxer', name: 'Hair Relaxer', description: 'Chemical hair straightening treatment', category: 'Chemical Services', estimatedDuration: 180, basePrice: 150, currency: 'ZAR' },
        { id: 'braids', name: 'Braiding Services', description: 'Various braiding styles', category: 'Styling', estimatedDuration: 240, basePrice: 200, currency: 'ZAR' },
        { id: 'weave_install', name: 'Weave Installation', description: 'Hair extension installation', category: 'Extensions', estimatedDuration: 300, basePrice: 350, currency: 'ZAR' },
        { id: 'manicure', name: 'Manicure', description: 'Nail care and polish', category: 'Nail Services', estimatedDuration: 45, basePrice: 50, currency: 'ZAR' },
        { id: 'pedicure', name: 'Pedicure', description: 'Foot care and nail polish', category: 'Nail Services', estimatedDuration: 60, basePrice: 70, currency: 'ZAR' }
      ],
      automationTemplates: ['beauty_appointment_reminders', 'loyalty_program', 'birthday_specials', 'service_follow_up'],
      dashboardWidgets: [
        { id: 'todays_appointments', title: "Today's Appointments", type: 'list', size: 'large', position: 1 },
        { id: 'weekly_revenue', title: 'Weekly Revenue', type: 'metric', size: 'medium', position: 2 },
        { id: 'popular_services', title: 'Popular Services', type: 'chart', size: 'medium', position: 3 },
        { id: 'client_retention', title: 'Client Retention Rate', type: 'metric', size: 'small', position: 4 },
        { id: 'upcoming_birthdays', title: 'Upcoming Client Birthdays', type: 'list', size: 'medium', position: 5 }
      ],
      businessHours: {
        monday: { open: '08:00', close: '18:00', closed: false },
        tuesday: { open: '08:00', close: '18:00', closed: false },
        wednesday: { open: '08:00', close: '18:00', closed: false },
        thursday: { open: '08:00', close: '18:00', closed: false },
        friday: { open: '08:00', close: '19:00', closed: false },
        saturday: { open: '07:00', close: '17:00', closed: false },
        sunday: { open: '09:00', close: '15:00', closed: false }
      },
      contactStages: [
        { id: 'inquiry', name: 'Inquiry', description: 'Initial contact or interest', color: 'gray', order: 1 },
        { id: 'consultation', name: 'Consultation', description: 'Hair consultation scheduled/completed', color: 'blue', order: 2 },
        { id: 'active', name: 'Active Client', description: 'Regular appointments', color: 'green', order: 3 },
        { id: 'priority', name: 'VIP/Priority', description: 'High-value client', color: 'purple', order: 4 },
        { id: 'planning', name: 'Event Planning', description: 'Bridal or special event planning', color: 'pink', order: 5 },
        { id: 'inactive', name: 'Inactive', description: 'No recent appointments', color: 'orange', order: 6 }
      ],
      pricingTiers: [
        { id: 'basic', name: 'Basic Package', description: 'Essential hair services', basePrice: 80, currency: 'ZAR', features: ['Wash & Set', 'Basic Styling'] },
        { id: 'premium', name: 'Premium Package', description: 'Advanced treatments', basePrice: 200, currency: 'ZAR', features: ['Chemical Services', 'Deep Conditioning', 'Styling'] },
        { id: 'luxury', name: 'Luxury Package', description: 'Full service experience', basePrice: 400, currency: 'ZAR', features: ['All Hair Services', 'Nail Services', 'Relaxation Treatment'] }
      ]
    },

    plumbing_service: {
      id: 'plumbing_service',
      name: 'Plumbing Services',
      type: 'Plumbers',
      sector: 'Home Services',
      customFields: [
        { id: 'property_type', name: 'Property Type', type: 'select', required: true, options: ['Residential House', 'Apartment', 'Commercial Building', 'Industrial'] },
        { id: 'emergency_contact', name: 'Emergency Contact', type: 'phone', required: true, placeholder: 'Alternative contact number' },
        { id: 'preferred_visit_time', name: 'Preferred Visit Time', type: 'select', required: false, options: ['Morning (8-12)', 'Afternoon (12-17)', 'Evening (17-19)', 'Anytime'] },
        { id: 'service_history', name: 'Service History', type: 'text', required: false, placeholder: 'Previous plumbing work done' },
        { id: 'payment_terms', name: 'Payment Terms', type: 'select', required: false, options: ['Cash on Completion', '30 Days', '60 Days', 'Upfront Payment'] },
        { id: 'location_notes', name: 'Location Notes', type: 'text', required: false, placeholder: 'Directions or access instructions' }
      ],
      contactTypes: [
        { id: 'emergency_client', name: 'Emergency Client', description: 'Urgent plumbing issues', icon: 'ExclamationTriangleIcon', color: 'red', defaultStage: 'urgent' },
        { id: 'regular_client', name: 'Regular Client', description: 'Scheduled maintenance customers', icon: 'WrenchScrewdriverIcon', color: 'blue', defaultStage: 'active' },
        { id: 'commercial_client', name: 'Commercial Client', description: 'Business or commercial properties', icon: 'BuildingOfficeIcon', color: 'purple', defaultStage: 'contracted' },
        { id: 'new_client', name: 'New Client', description: 'First-time customers', icon: 'UserPlusIcon', color: 'green', defaultStage: 'estimate' }
      ],
      serviceOfferings: [
        { id: 'drain_cleaning', name: 'Drain Cleaning', description: 'Unblock drains and pipes', category: 'Emergency Services', estimatedDuration: 60, basePrice: 250, currency: 'ZAR' },
        { id: 'pipe_repair', name: 'Pipe Repair', description: 'Fix burst or leaking pipes', category: 'Repairs', estimatedDuration: 120, basePrice: 400, currency: 'ZAR' },
        { id: 'toilet_installation', name: 'Toilet Installation', description: 'Install new toilet systems', category: 'Installation', estimatedDuration: 180, basePrice: 600, currency: 'ZAR' },
        { id: 'geyser_repair', name: 'Geyser Repair', description: 'Hot water system repairs', category: 'Appliance Repair', estimatedDuration: 150, basePrice: 500, currency: 'ZAR' },
        { id: 'bathroom_renovation', name: 'Bathroom Renovation', description: 'Complete bathroom plumbing', category: 'Major Projects', estimatedDuration: 480, basePrice: 2500, currency: 'ZAR' },
        { id: 'maintenance_inspection', name: 'Maintenance Inspection', description: 'Preventive maintenance check', category: 'Maintenance', estimatedDuration: 90, basePrice: 200, currency: 'ZAR' }
      ],
      automationTemplates: ['emergency_response', 'maintenance_reminders', 'quote_follow_up', 'payment_reminders'],
      dashboardWidgets: [
        { id: 'emergency_calls', title: 'Emergency Calls Today', type: 'metric', size: 'large', position: 1 },
        { id: 'scheduled_jobs', title: 'Scheduled Jobs', type: 'list', size: 'large', position: 2 },
        { id: 'revenue_tracker', title: 'Monthly Revenue', type: 'chart', size: 'medium', position: 3 },
        { id: 'outstanding_invoices', title: 'Outstanding Invoices', type: 'metric', size: 'medium', position: 4 },
        { id: 'repeat_customers', title: 'Repeat Customers', type: 'list', size: 'medium', position: 5 }
      ],
      businessHours: {
        monday: { open: '07:00', close: '17:00', closed: false },
        tuesday: { open: '07:00', close: '17:00', closed: false },
        wednesday: { open: '07:00', close: '17:00', closed: false },
        thursday: { open: '07:00', close: '17:00', closed: false },
        friday: { open: '07:00', close: '17:00', closed: false },
        saturday: { open: '08:00', close: '14:00', closed: false },
        sunday: { open: '09:00', close: '12:00', closed: false }
      },
      contactStages: [
        { id: 'inquiry', name: 'Initial Inquiry', description: 'Customer contacted for service', color: 'gray', order: 1 },
        { id: 'estimate', name: 'Estimate Provided', description: 'Quote sent to customer', color: 'blue', order: 2 },
        { id: 'scheduled', name: 'Job Scheduled', description: 'Appointment confirmed', color: 'yellow', order: 3 },
        { id: 'urgent', name: 'Emergency Service', description: 'Urgent plumbing issue', color: 'red', order: 4 },
        { id: 'active', name: 'Job In Progress', description: 'Currently working on site', color: 'orange', order: 5 },
        { id: 'completed', name: 'Job Completed', description: 'Work finished and paid', color: 'green', order: 6 },
        { id: 'contracted', name: 'Maintenance Contract', description: 'Regular maintenance client', color: 'purple', order: 7 }
      ],
      pricingTiers: [
        { id: 'emergency', name: 'Emergency Service', description: 'Urgent repairs and callouts', basePrice: 350, currency: 'ZAR', features: ['24/7 Availability', 'Priority Response', 'Emergency Repairs'] },
        { id: 'standard', name: 'Standard Service', description: 'Regular plumbing services', basePrice: 200, currency: 'ZAR', features: ['Scheduled Appointments', 'Basic Repairs', 'Installations'] },
        { id: 'maintenance', name: 'Maintenance Contract', description: 'Ongoing maintenance agreement', basePrice: 500, currency: 'ZAR', features: ['Monthly Inspections', 'Preventive Maintenance', 'Priority Booking', 'Discounted Repairs'] }
      ]
    },

    spaza_shop: {
      id: 'spaza_shop',
      name: 'Spaza Shop',
      type: 'Spaza / Convenience shop',
      sector: 'Retail',
      customFields: [
        { id: 'credit_limit', name: 'Credit Limit', type: 'number', required: false, placeholder: 'R0.00' },
        { id: 'payment_method', name: 'Preferred Payment', type: 'select', required: false, options: ['Cash', 'Credit', 'EFT', 'Mobile Money'] },
        { id: 'delivery_address', name: 'Delivery Address', type: 'text', required: false, placeholder: 'Home delivery address' },
        { id: 'family_size', name: 'Family Size', type: 'number', required: false, placeholder: 'Number of people in household' },
        { id: 'shopping_frequency', name: 'Shopping Frequency', type: 'select', required: false, options: ['Daily', 'Weekly', 'Bi-weekly', 'Monthly'] },
        { id: 'loyalty_card', name: 'Loyalty Card Number', type: 'text', required: false, placeholder: 'Loyalty card or account number' }
      ],
      contactTypes: [
        { id: 'regular_customer', name: 'Regular Customer', description: 'Frequent shoppers', icon: 'UserIcon', color: 'blue', defaultStage: 'active' },
        { id: 'credit_customer', name: 'Credit Customer', description: 'Customers with store credit', icon: 'CreditCardIcon', color: 'green', defaultStage: 'credit_approved' },
        { id: 'wholesale_customer', name: 'Wholesale Customer', description: 'Bulk purchase customers', icon: 'ShoppingCartIcon', color: 'purple', defaultStage: 'wholesale' },
        { id: 'new_customer', name: 'New Customer', description: 'First-time shoppers', icon: 'UserPlusIcon', color: 'yellow', defaultStage: 'welcome' }
      ],
      serviceOfferings: [
        { id: 'grocery_shopping', name: 'Grocery Shopping', description: 'Daily essentials and food items', category: 'Retail Sales', estimatedDuration: 15, basePrice: 50, currency: 'ZAR' },
        { id: 'delivery_service', name: 'Home Delivery', description: 'Deliver groceries to customer home', category: 'Delivery', estimatedDuration: 30, basePrice: 10, currency: 'ZAR' },
        { id: 'credit_purchase', name: 'Credit Purchase', description: 'Buy now, pay later option', category: 'Credit Services', estimatedDuration: 10, basePrice: 0, currency: 'ZAR' },
        { id: 'bulk_order', name: 'Bulk Order', description: 'Large quantity purchases', category: 'Wholesale', estimatedDuration: 45, basePrice: 200, currency: 'ZAR' }
      ],
      automationTemplates: ['loyalty_program', 'credit_reminders', 'new_stock_alerts', 'payment_due_notifications'],
      dashboardWidgets: [
        { id: 'daily_sales', title: 'Daily Sales', type: 'metric', size: 'large', position: 1 },
        { id: 'popular_products', title: 'Popular Products', type: 'chart', size: 'medium', position: 2 },
        { id: 'credit_owing', title: 'Credit Outstanding', type: 'metric', size: 'medium', position: 3 },
        { id: 'low_stock_alerts', title: 'Low Stock Alerts', type: 'list', size: 'medium', position: 4 },
        { id: 'top_customers', title: 'Top Customers', type: 'list', size: 'medium', position: 5 }
      ],
      businessHours: {
        monday: { open: '06:00', close: '20:00', closed: false },
        tuesday: { open: '06:00', close: '20:00', closed: false },
        wednesday: { open: '06:00', close: '20:00', closed: false },
        thursday: { open: '06:00', close: '20:00', closed: false },
        friday: { open: '06:00', close: '20:00', closed: false },
        saturday: { open: '06:00', close: '20:00', closed: false },
        sunday: { open: '07:00', close: '18:00', closed: false }
      },
      contactStages: [
        { id: 'welcome', name: 'New Customer', description: 'Recently registered customer', color: 'green', order: 1 },
        { id: 'active', name: 'Active Shopper', description: 'Regular purchasing customer', color: 'blue', order: 2 },
        { id: 'credit_approved', name: 'Credit Approved', description: 'Approved for store credit', color: 'purple', order: 3 },
        { id: 'wholesale', name: 'Wholesale Customer', description: 'Bulk purchase customer', color: 'orange', order: 4 },
        { id: 'vip', name: 'VIP Customer', description: 'High-value customer', color: 'gold', order: 5 },
        { id: 'inactive', name: 'Inactive', description: 'No recent purchases', color: 'gray', order: 6 }
      ],
      pricingTiers: [
        { id: 'retail', name: 'Retail Prices', description: 'Standard retail pricing', basePrice: 0, currency: 'ZAR', features: ['Standard Pricing', 'Cash/Card Payment'] },
        { id: 'wholesale', name: 'Wholesale Prices', description: 'Bulk purchase discounts', basePrice: 0, currency: 'ZAR', features: ['Volume Discounts', 'Bulk Pricing', 'Special Orders'] },
        { id: 'credit', name: 'Credit Terms', description: 'Credit purchase option', basePrice: 0, currency: 'ZAR', features: ['Credit Facility', 'Payment Plans', 'Interest on Overdue'] }
      ]
    }
  }

  const setCurrentEnterprise = (enterpriseType: string) => {
    const config = enterpriseConfigs[enterpriseType]
    if (config) {
      currentEnterprise.value = config
      // Store in localStorage for persistence
      localStorage.setItem('toss_enterprise_config', JSON.stringify(config))
    }
  }

  const loadEnterpriseFromStorage = () => {
    const stored = localStorage.getItem('toss_enterprise_config')
    if (stored) {
      try {
        currentEnterprise.value = JSON.parse(stored)
      } catch (error) {
        console.error('Failed to load enterprise config from storage:', error)
      }
    }
  }

  const getCustomFieldsForContactForm = computed(() => {
    return currentEnterprise.value?.customFields || []
  })

  const getContactTypesForEnterprise = computed(() => {
    return currentEnterprise.value?.contactTypes || []
  })

  const getServiceOfferingsForEnterprise = computed(() => {
    return currentEnterprise.value?.serviceOfferings || []
  })

  const getDashboardWidgetsForEnterprise = computed(() => {
    return currentEnterprise.value?.dashboardWidgets || []
  })

  const getContactStagesForEnterprise = computed(() => {
    return currentEnterprise.value?.contactStages || []
  })

  const getAvailableEnterpriseTypes = computed(() => {
    return Object.keys(enterpriseConfigs).map(key => ({
      id: key,
      name: enterpriseConfigs[key].name,
      type: enterpriseConfigs[key].type,
      sector: enterpriseConfigs[key].sector
    }))
  })

  return {
    currentEnterprise,
    setCurrentEnterprise,
    loadEnterpriseFromStorage,
    getCustomFieldsForContactForm,
    getContactTypesForEnterprise,
    getServiceOfferingsForEnterprise,
    getDashboardWidgetsForEnterprise,
    getContactStagesForEnterprise,
    getAvailableEnterpriseTypes,
    enterpriseConfigs
  }
}
