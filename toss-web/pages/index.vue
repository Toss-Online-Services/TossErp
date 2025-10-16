<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Mobile-First Page Container -->
    <div class="p-4 sm:p-6 space-y-4 sm:space-y-6 pb-20 lg:pb-6">
      <!-- Page Header -->
      <div class="text-center sm:text-left">
        <h1 class="text-2xl sm:text-3xl font-bold text-gray-900">Dashboard</h1>
        <p class="text-gray-600 mt-1 text-sm sm:text-base">Welcome back! Here's what's happening with your business today.</p>
      </div>

      <!-- WhatsApp Quick Order -->
      <div class="mb-4">
        <WhatsAppChatPlaceholder />
      </div>

      <!-- Quick Stats - Mobile First Grid -->
      <div class="grid grid-cols-1 xs:grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-6">
        <div class="bg-white p-4 sm:p-6 rounded-xl shadow-sm border border-gray-200">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-gray-600">Total Revenue</p>
              <p class="text-lg sm:text-2xl font-bold text-gray-900">R 1.2M</p>
              <p class="text-xs sm:text-sm text-green-600">+12.5%</p>
            </div>
            <div class="p-2 sm:p-3 bg-green-100 dark:bg-green-900 rounded-full">
              <CurrencyDollarIcon class="w-4 h-4 sm:w-6 sm:h-6 text-green-600" />
            </div>
          </div>
        </div>

        <div class="bg-white p-4 sm:p-6 rounded-xl shadow-sm border border-gray-200">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-gray-600">Customers</p>
              <p class="text-lg sm:text-2xl font-bold text-gray-900">1,428</p>
              <p class="text-xs sm:text-sm text-blue-600">+8.2%</p>
            </div>
            <div class="p-2 sm:p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
              <UsersIcon class="w-4 h-4 sm:w-6 sm:h-6 text-blue-600" />
            </div>
          </div>
        </div>

        <div class="bg-white p-4 sm:p-6 rounded-xl shadow-sm border border-gray-200">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-gray-600">Projects</p>
              <p class="text-lg sm:text-2xl font-bold text-gray-900">24</p>
              <p class="text-xs sm:text-sm text-yellow-600">3 due</p>
            </div>
            <div class="p-2 sm:p-3 bg-yellow-100 dark:bg-yellow-900 rounded-full">
              <BriefcaseIcon class="w-4 h-4 sm:w-6 sm:h-6 text-yellow-600" />
            </div>
          </div>
        </div>

        <div class="bg-white p-4 sm:p-6 rounded-xl shadow-sm border border-gray-200">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-gray-600">Pipeline</p>
              <p class="text-lg sm:text-2xl font-bold text-gray-900">R 892K</p>
              <p class="text-xs sm:text-sm text-purple-600">15 ops</p>
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
          <div class="bg-white rounded-xl shadow-sm border border-gray-200">
            <div class="p-4 sm:p-6 border-b border-gray-200">
              <h3 class="text-base sm:text-lg font-semibold text-gray-900">Recent Activity</h3>
            </div>
            <div class="p-4 sm:p-6">
              <div class="space-y-3 sm:space-y-4">
                <div v-for="activity in recentActivities" :key="activity.id" class="flex items-start space-x-3 sm:space-x-4">
                  <div class="w-8 h-8 sm:w-10 sm:h-10 rounded-full flex items-center justify-center" :class="activity.color">
                    <component :is="activity.icon" class="w-4 h-4 sm:w-5 sm:h-5 text-white" />
                  </div>
                  <div class="flex-1 min-w-0">
                    <p class="text-sm font-medium text-gray-900 truncate">{{ activity.title }}</p>
                    <p class="text-xs sm:text-sm text-gray-600 truncate">{{ activity.description }}</p>
                    <p class="text-xs text-slate-500 dark:text-slate-500 mt-1">{{ formatDate(activity.date) }}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Quick Actions -->
        <div>
          <div class="bg-white rounded-xl shadow-sm border border-gray-200">
            <div class="p-4 sm:p-6 border-b border-gray-200">
              <h3 class="text-base sm:text-lg font-semibold text-gray-900">Quick Actions</h3>
            </div>
            <div class="p-4 sm:p-6">
              <div class="space-y-2 sm:space-y-3">
                <NuxtLink to="/crm" class="block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors touch-manipulation">
                  <div class="flex items-center space-x-3">
                    <UsersIcon class="w-5 h-5 text-blue-600 flex-shrink-0" />
                    <div class="min-w-0">
                      <p class="font-medium text-gray-900 text-sm">Manage CRM</p>
                      <p class="text-xs text-gray-600 truncate">View customers and opportunities</p>
                    </div>
                  </div>
                </NuxtLink>

                <NuxtLink to="/projects" class="block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors touch-manipulation">
                  <div class="flex items-center space-x-3">
                    <BriefcaseIcon class="w-5 h-5 text-green-600 flex-shrink-0" />
                    <div class="min-w-0">
                      <p class="font-medium text-gray-900 text-sm">View Projects</p>
                      <p class="text-xs text-gray-600 truncate">Track project progress</p>
                    </div>
                  </div>
                </NuxtLink>

                <NuxtLink to="/accounts" class="block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors touch-manipulation">
                  <div class="flex items-center space-x-3">
                    <CurrencyDollarIcon class="w-5 h-5 text-purple-600 flex-shrink-0" />
                    <div class="min-w-0">
                      <p class="font-medium text-gray-900 text-sm">Financial Reports</p>
                      <p class="text-xs text-gray-600 truncate">View financial data</p>
                    </div>
                  </div>
                </NuxtLink>

                <NuxtLink to="/stock" class="block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors touch-manipulation">
                  <div class="flex items-center space-x-3">
                    <ArchiveBoxIcon class="w-5 h-5 text-yellow-600 flex-shrink-0" />
                    <div class="min-w-0">
                      <p class="font-medium text-gray-900 text-sm">Inventory</p>
                      <p class="text-xs text-gray-600 truncate">Manage stock levels</p>
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
        <!-- Revenue Chart -->
        <div class="bg-white rounded-xl shadow-sm border border-gray-200">
          <div class="p-4 sm:p-6 border-b border-gray-200">
            <h3 class="text-base sm:text-lg font-semibold text-gray-900">Revenue Trend</h3>
          </div>
          <div class="p-4 sm:p-6">
            <div class="h-48 sm:h-64 flex items-center justify-center bg-slate-50 dark:bg-slate-700 rounded-lg">
              <p class="text-slate-500 dark:text-slate-400 text-sm">Revenue chart would be displayed here</p>
            </div>
          </div>
        </div>

        <!-- Sales Funnel -->
        <div class="bg-white rounded-xl shadow-sm border border-gray-200">
          <div class="p-4 sm:p-6 border-b border-gray-200">
            <h3 class="text-base sm:text-lg font-semibold text-gray-900">Sales Funnel</h3>
          </div>
          <div class="p-4 sm:p-6">
            <div class="space-y-3 sm:space-y-4">
              <div v-for="stage in salesFunnel" :key="stage.name" class="flex items-center justify-between">
                <div class="flex items-center space-x-2 sm:space-x-3 min-w-0">
                  <div class="w-3 h-3 sm:w-4 sm:h-4 rounded-full flex-shrink-0" :class="stage.color"></div>
                  <span class="text-sm font-medium text-gray-900 truncate">{{ stage.name }}</span>
                </div>
                <div class="flex items-center space-x-2 flex-shrink-0">
                  <span class="text-sm text-gray-600">{{ stage.count }}</span>
                  <div class="w-16 sm:w-20 bg-slate-200 dark:bg-slate-600 rounded-full h-2">
                    <div class="h-2 rounded-full transition-all duration-300" :class="stage.color" :style="{ width: stage.percentage + '%' }"></div>
                  </div>
                </div>
              </div>
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
  BriefcaseIcon, 
  ArrowTrendingUpIcon,
  ArchiveBoxIcon,
  PhoneIcon,
  EnvelopeIcon,
  DocumentIcon,
  ShoppingCartIcon
} from '@heroicons/vue/24/outline'

// Sample data
const recentActivities = ref([
  {
    id: 1,
    title: 'New Customer Registration',
    description: 'TechCorp Ltd has registered as a new customer',
    date: new Date(),
    icon: UsersIcon,
    color: 'bg-blue-500'
  },
  {
    id: 2,
    title: 'Order Completed',
    description: 'Order #1234 for R45,000 has been completed',
    date: new Date(Date.now() - 3600000),
    icon: ShoppingCartIcon,
    color: 'bg-green-500'
  },
  {
    id: 3,
    title: 'Quote Sent',
    description: 'Quote sent to BigCorp for enterprise package',
    date: new Date(Date.now() - 7200000),
    icon: DocumentIcon,
    color: 'bg-purple-500'
  },
  {
    id: 4,
    title: 'Support Ticket',
    description: 'New support ticket from existing customer',
    date: new Date(Date.now() - 86400000),
    icon: EnvelopeIcon,
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

// Page metadata
definePageMeta({
  layout: 'default',
  title: 'Dashboard - TOSS ERP'
})
</script>

