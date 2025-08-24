<template>
  <nav class="space-y-2">
    <!-- Main Navigation -->
    <div class="space-y-1">
      <h3 class="px-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">
        {{ navigationLabels.main }}
      </h3>
      
      <UVerticalNavigation
        :links="mainNavigation"
        :ui="{
          wrapper: 'space-y-1',
          base: 'group flex items-center px-3 py-2 text-sm font-medium rounded-lg',
          active: 'bg-primary-50 dark:bg-primary-900/50 text-primary-600 dark:text-primary-400',
          inactive: 'text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-800 hover:text-gray-900 dark:hover:text-white'
        }"
        @click="$emit('close')"
      />
    </div>

    <!-- ERP Modules -->
    <div class="space-y-1 pt-4">
      <h3 class="px-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">
        {{ navigationLabels.erp_modules }}
      </h3>
      
      <UVerticalNavigation
        :links="erpModules"
        :ui="{
          wrapper: 'space-y-1',
          base: 'group flex items-center px-3 py-2 text-sm font-medium rounded-lg',
          active: 'bg-primary-50 dark:bg-primary-900/50 text-primary-600 dark:text-primary-400',
          inactive: 'text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-800 hover:text-gray-900 dark:hover:text-white'
        }"
        @click="$emit('close')"
      />
    </div>

    <!-- Collaborative Features -->
    <div class="space-y-1 pt-4">
      <h3 class="px-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">
        {{ navigationLabels.collaborative }}
      </h3>
      
      <UVerticalNavigation
        :links="collaborativeFeatures"
        :ui="{
          wrapper: 'space-y-1',
          base: 'group flex items-center px-3 py-2 text-sm font-medium rounded-lg',
          active: 'bg-primary-50 dark:bg-primary-900/50 text-primary-600 dark:text-primary-400',
          inactive: 'text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-800 hover:text-gray-900 dark:hover:text-white'
        }"
        @click="$emit('close')"
      />
    </div>

    <!-- AI & Analytics -->
    <div class="space-y-1 pt-4">
      <h3 class="px-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">
        {{ navigationLabels.ai_analytics }}
      </h3>
      
      <UVerticalNavigation
        :links="aiAnalytics"
        :ui="{
          wrapper: 'space-y-1',
          base: 'group flex items-center px-3 py-2 text-sm font-medium rounded-lg',
          active: 'bg-primary-50 dark:bg-primary-900/50 text-primary-600 dark:text-primary-400',
          inactive: 'text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-800 hover:text-gray-900 dark:hover:text-white'
        }"
        @click="$emit('close')"
      />
    </div>

    <!-- Administration -->
    <div class="space-y-1 pt-4">
      <h3 class="px-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">
        {{ navigationLabels.administration }}
      </h3>
      
      <UVerticalNavigation
        :links="administration"
        :ui="{
          wrapper: 'space-y-1',
          base: 'group flex items-center px-3 py-2 text-sm font-medium rounded-lg',
          active: 'bg-primary-50 dark:bg-primary-900/50 text-primary-600 dark:text-primary-400',
          inactive: 'text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-800 hover:text-gray-900 dark:hover:text-white'
        }"
        @click="$emit('close')"
      />
    </div>
  </nav>
</template>

<script setup lang="ts">
// Emits
defineEmits<{
  close: []
}>()

// Use computed values for navigation labels since we can't use useI18n in components
// These will be replaced by proper i18n when the app initializes
const navigationLabels = {
  main: 'Main',
  erp_modules: 'ERP Modules',
  collaborative: 'Collaborative',
  ai_analytics: 'AI & Analytics',
  administration: 'Administration'
}

// Navigation structure
const mainNavigation = [
  {
    label: 'Dashboard',
    icon: 'i-heroicons-home',
    to: '/'
  },
  {
    label: 'Quick Actions',
    icon: 'i-heroicons-bolt',
    to: '/quick-actions'
  }
]

const erpModules = [
  {
    label: 'Financial Management',
    icon: 'i-heroicons-banknotes',
    to: '/finance'
  },
  {
    label: 'Sales & CRM',
    icon: 'i-heroicons-user-group',
    to: '/sales'
  },
  {
    label: 'Inventory & Procurement',
    icon: 'i-heroicons-cube',
    to: '/inventory'
  },
  {
    label: 'Project Management',
    icon: 'i-heroicons-clipboard-document-list',
    to: '/projects'
  },
  {
    label: 'HR & Payroll',
    icon: 'i-heroicons-users',
    to: '/hr'
  },
  {
    label: 'Service Management',
    icon: 'i-heroicons-wrench-screwdriver',
    to: '/services'
  }
]

const collaborativeFeatures = [
  {
    label: 'Group Buying',
    icon: 'i-heroicons-shopping-cart',
    to: '/group-buying'
  },
  {
    label: 'Resource Sharing',
    icon: 'i-heroicons-arrow-path-rounded-square',
    to: '/resource-sharing'
  },
  {
    label: 'Cost Sharing',
    icon: 'i-heroicons-calculator',
    to: '/cost-sharing'
  },
  {
    label: 'Pooled Credit',
    icon: 'i-heroicons-currency-dollar',
    to: '/pooled-credit'
  },
  {
    label: 'Mutual Financing',
    icon: 'i-heroicons-banknotes',
    to: '/mutual-financing'
  },
  {
    label: 'Collaborative Networks',
    icon: 'i-heroicons-globe-alt',
    to: '/collaborative-networks'
  }
]

const aiAnalytics = [
  {
    label: 'AI Co-Pilot',
    icon: 'i-heroicons-sparkles',
    to: '/ai-copilot'
  },
  {
    label: 'Analytics Dashboard',
    icon: 'i-heroicons-chart-bar',
    to: '/analytics'
  },
  {
    label: 'Reports',
    icon: 'i-heroicons-document-chart-bar',
    to: '/reports'
  },
  {
    label: 'Business Intelligence',
    icon: 'i-heroicons-light-bulb',
    to: '/business-intelligence'
  }
]

const administration = [
  {
    label: 'User Management',
    icon: 'i-heroicons-user-circle',
    to: '/admin/users'
  },
  {
    label: 'System Settings',
    icon: 'i-heroicons-cog-6-tooth',
    to: '/admin/settings'
  },
  {
    label: 'Security',
    icon: 'i-heroicons-shield-check',
    to: '/admin/security'
  },
  {
    label: 'Audit Logs',
    icon: 'i-heroicons-document-text',
    to: '/admin/audit'
  }
]
</script>
