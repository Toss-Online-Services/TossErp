<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <!-- Mobile-First Page Container -->
    <div class="p-4 sm:p-6 space-y-4 sm:space-y-6 pb-20 lg:pb-6">
      <!-- Page Header -->
      <div class="text-center sm:text-left">
        <h1 class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">CRM Dashboard</h1>
        <p class="text-slate-600 dark:text-slate-400 mt-1 text-sm sm:text-base">Manage your leads, customers, and sales pipeline</p>
      </div>

      <!-- Quick Stats - Mobile First Grid -->
      <div class="grid grid-cols-1 xs:grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-6">
        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Total Customers</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">1,428</p>
              <p class="text-xs sm:text-sm text-green-600">+8.2%</p>
            </div>
            <div class="p-2 sm:p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
              <UsersIcon class="w-4 h-4 sm:w-6 sm:h-6 text-blue-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Active Leads</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">124</p>
              <p class="text-xs sm:text-sm text-blue-600">15 new</p>
            </div>
            <div class="p-2 sm:p-3 bg-green-100 dark:bg-green-900 rounded-full">
              <UserPlusIcon class="w-4 h-4 sm:w-6 sm:h-6 text-green-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Opportunities</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">45</p>
              <p class="text-xs sm:text-sm text-yellow-600">R 892K</p>
            </div>
            <div class="p-2 sm:p-3 bg-yellow-100 dark:bg-yellow-900 rounded-full">
              <CurrencyDollarIcon class="w-4 h-4 sm:w-6 sm:h-6 text-yellow-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Conv. Rate</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">24.8%</p>
              <p class="text-xs sm:text-sm text-purple-600">+2.1%</p>
            </div>
            <div class="p-2 sm:p-3 bg-purple-100 dark:bg-purple-900 rounded-full">
              <ArrowTrendingUpIcon class="w-4 h-4 sm:w-6 sm:h-6 text-purple-600" />
            </div>
          </div>
        </div>
      </div>

      <!-- Main Content Grid - Mobile Responsive -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-4 sm:gap-6">
        <!-- Recent Activity -->
        <div class="lg:col-span-2">
          <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
            <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
              <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">Recent CRM Activity</h3>
            </div>
            <div class="p-4 sm:p-6">
              <div class="space-y-3 sm:space-y-4">
                <div v-for="activity in recentActivities" :key="activity.id" class="flex items-start space-x-3 sm:space-x-4">
                  <div class="w-8 h-8 sm:w-10 sm:h-10 rounded-full flex items-center justify-center" :class="activity.color">
                    <component :is="activity.icon" class="w-4 h-4 sm:w-5 sm:h-5 text-white" />
                  </div>
                  <div class="flex-1 min-w-0">
                    <p class="text-sm font-medium text-slate-900 dark:text-white truncate">{{ activity.title }}</p>
                    <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 truncate">{{ activity.description }}</p>
                    <p class="text-xs text-slate-500 dark:text-slate-500 mt-1">{{ formatDate(activity.date) }}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Quick Actions -->
        <div>
          <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
            <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
              <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">Quick Actions</h3>
            </div>
            <div class="p-4 sm:p-6">
              <div class="space-y-2 sm:space-y-3">
                <NuxtLink to="/crm/customers" class="block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors touch-manipulation">
                  <div class="flex items-center space-x-3">
                    <UsersIcon class="w-5 h-5 text-blue-600 flex-shrink-0" />
                    <div class="min-w-0">
                      <p class="font-medium text-slate-900 dark:text-white text-sm">View Customers</p>
                      <p class="text-xs text-slate-600 dark:text-slate-400 truncate">Manage customer database</p>
                    </div>
                  </div>
                </NuxtLink>

                <NuxtLink to="/crm/leads" class="block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors touch-manipulation">
                  <div class="flex items-center space-x-3">
                    <UserPlusIcon class="w-5 h-5 text-green-600 flex-shrink-0" />
                    <div class="min-w-0">
                      <p class="font-medium text-slate-900 dark:text-white text-sm">Manage Leads</p>
                      <p class="text-xs text-slate-600 dark:text-slate-400 truncate">Track potential customers</p>
                    </div>
                  </div>
                </NuxtLink>

                <NuxtLink to="/crm/opportunities" class="block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors touch-manipulation">
                  <div class="flex items-center space-x-3">
                    <CurrencyDollarIcon class="w-5 h-5 text-purple-600 flex-shrink-0" />
                    <div class="min-w-0">
                      <p class="font-medium text-slate-900 dark:text-white text-sm">Opportunities</p>
                      <p class="text-xs text-slate-600 dark:text-slate-400 truncate">Track sales pipeline</p>
                    </div>
                  </div>
                </NuxtLink>

                <NuxtLink to="/crm/pipeline" class="block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors touch-manipulation">
                  <div class="flex items-center space-x-3">
                    <ChartBarIcon class="w-5 h-5 text-yellow-600 flex-shrink-0" />
                    <div class="min-w-0">
                      <p class="font-medium text-slate-900 dark:text-white text-sm">Sales Pipeline</p>
                      <p class="text-xs text-slate-600 dark:text-slate-400 truncate">View sales funnel</p>
                    </div>
                  </div>
                </NuxtLink>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Charts and Analytics - Mobile Responsive -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-4 sm:gap-6">
        <!-- Sales Pipeline Chart -->
        <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
            <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">Sales Pipeline</h3>
          </div>
          <div class="p-4 sm:p-6">
            <div class="space-y-3 sm:space-y-4">
              <div v-for="stage in salesFunnel" :key="stage.name" class="flex items-center justify-between">
                <div class="flex items-center space-x-2 sm:space-x-3 min-w-0">
                  <div class="w-3 h-3 sm:w-4 sm:h-4 rounded-full flex-shrink-0" :class="stage.color"></div>
                  <span class="text-sm font-medium text-slate-900 dark:text-white truncate">{{ stage.name }}</span>
                </div>
                <div class="flex items-center space-x-2 flex-shrink-0">
                  <span class="text-sm text-slate-600 dark:text-slate-400">{{ stage.count }}</span>
                  <div class="w-16 sm:w-20 bg-slate-200 dark:bg-slate-600 rounded-full h-2">
                    <div class="h-2 rounded-full transition-all duration-300" :class="stage.color" :style="{ width: stage.percentage + '%' }"></div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Customer Growth -->
        <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
            <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">Customer Growth</h3>
          </div>
          <div class="p-4 sm:p-6">
            <div class="h-48 sm:h-64 flex items-center justify-center bg-slate-50 dark:bg-slate-700 rounded-lg">
              <p class="text-slate-500 dark:text-slate-400 text-sm">Customer growth chart would be displayed here</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { 
  CurrencyDollarIcon, 
  UsersIcon, 
  UserPlusIcon,
  ArrowTrendingUpIcon,
  ChartBarIcon,
  PhoneIcon,
  EnvelopeIcon,
  DocumentIcon,
  CheckCircleIcon
} from '@heroicons/vue/24/outline'

// Sample CRM data
const recentActivities = ref([
  {
    id: 1,
    title: 'New Lead Added',
    description: 'TechCorp Ltd submitted inquiry for enterprise solution',
    date: new Date(),
    icon: UserPlusIcon,
    color: 'bg-blue-500'
  },
  {
    id: 2,
    title: 'Deal Closed',
    description: 'BigCorp signed contract for R450,000',
    date: new Date(Date.now() - 3600000),
    icon: CheckCircleIcon,
    color: 'bg-green-500'
  },
  {
    id: 3,
    title: 'Quote Sent',
    description: 'Proposal sent to MegaCorp for consultation services',
    date: new Date(Date.now() - 7200000),
    icon: DocumentIcon,
    color: 'bg-purple-500'
  },
  {
    id: 4,
    title: 'Customer Call',
    description: 'Follow-up call completed with existing client',
    date: new Date(Date.now() - 86400000),
    icon: PhoneIcon,
    color: 'bg-yellow-500'
  }
])

const salesFunnel = ref([
  { name: 'Leads', count: 124, percentage: 100, color: 'bg-blue-500' },
  { name: 'Qualified', count: 87, percentage: 70, color: 'bg-green-500' },
  { name: 'Proposal', count: 45, percentage: 36, color: 'bg-yellow-500' },
  { name: 'Negotiation', count: 23, percentage: 19, color: 'bg-orange-500' },
  { name: 'Closed Won', count: 12, percentage: 10, color: 'bg-purple-500' }
])

// Helper functions
function formatDate(date: Date): string {
  return new Intl.DateTimeFormat('en-ZA', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date)
}
</script>
