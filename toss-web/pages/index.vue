<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Mobile-First Page Container -->
    <div class="p-4 pb-20 space-y-4 sm:p-6 sm:space-y-6 lg:pb-6">
      <!-- Page Header -->
      <div class="text-center sm:text-left">
        <h1 class="text-2xl font-bold text-gray-900 sm:text-3xl">Dashboard</h1>
        <p class="mt-1 text-sm text-gray-600 sm:text-base">Welcome back! Here's what's happening with your business today.</p>
      </div>

      <!-- WhatsApp Quick Order -->
      <div class="mb-4">
        <WhatsAppChatPlaceholder />
      </div>

      <!-- Quick Stats - Mobile First Grid -->
      <div class="grid grid-cols-1 gap-3 xs:grid-cols-2 lg:grid-cols-4 sm:gap-6">
        <div class="p-4 bg-white rounded-xl border border-gray-200 shadow-sm sm:p-6">
          <div class="flex justify-between items-center">
            <div>
              <p class="text-xs text-gray-600 sm:text-sm">Total Revenue</p>
              <p class="text-lg font-bold text-gray-900 sm:text-2xl">R 1.2M</p>
              <p class="text-xs text-green-600 sm:text-sm">+12.5%</p>
            </div>
            <div class="p-2 bg-green-100 rounded-full sm:p-3 dark:bg-green-900">
              <CurrencyDollarIcon class="w-4 h-4 text-green-600 sm:w-6 sm:h-6" />
            </div>
          </div>
        </div>

        <div class="p-4 bg-white rounded-xl border border-gray-200 shadow-sm sm:p-6">
          <div class="flex justify-between items-center">
            <div>
              <p class="text-xs text-gray-600 sm:text-sm">Customers</p>
              <p class="text-lg font-bold text-gray-900 sm:text-2xl">1,428</p>
              <p class="text-xs text-blue-600 sm:text-sm">+8.2%</p>
            </div>
            <div class="p-2 bg-blue-100 rounded-full sm:p-3 dark:bg-blue-900">
              <UsersIcon class="w-4 h-4 text-blue-600 sm:w-6 sm:h-6" />
            </div>
          </div>
        </div>

        <div class="p-4 bg-white rounded-xl border border-gray-200 shadow-sm sm:p-6">
          <div class="flex justify-between items-center">
            <div>
              <p class="text-xs text-gray-600 sm:text-sm">Projects</p>
              <p class="text-lg font-bold text-gray-900 sm:text-2xl">24</p>
              <p class="text-xs text-yellow-600 sm:text-sm">3 due</p>
            </div>
            <div class="p-2 bg-yellow-100 rounded-full sm:p-3 dark:bg-yellow-900">
              <BriefcaseIcon class="w-4 h-4 text-yellow-600 sm:w-6 sm:h-6" />
            </div>
          </div>
        </div>

        <div class="p-4 bg-white rounded-xl border border-gray-200 shadow-sm sm:p-6">
          <div class="flex justify-between items-center">
            <div>
              <p class="text-xs text-gray-600 sm:text-sm">Pipeline</p>
              <p class="text-lg font-bold text-gray-900 sm:text-2xl">R 892K</p>
              <p class="text-xs text-purple-600 sm:text-sm">15 ops</p>
            </div>
            <div class="p-2 bg-purple-100 rounded-full sm:p-3 dark:bg-purple-900">
              <ArrowTrendingUpIcon class="w-4 h-4 text-purple-600 sm:w-6 sm:h-6" />
            </div>
          </div>
        </div>
      </div>

      <!-- Main Content Grid - Mobile Responsive -->
      <div class="grid grid-cols-1 gap-4 lg:grid-cols-3 sm:gap-6">
        <!-- Recent Activity -->
        <div class="lg:col-span-2">
          <div class="bg-white rounded-xl border border-gray-200 shadow-sm">
            <div class="p-4 border-b border-gray-200 sm:p-6">
              <h3 class="text-base font-semibold text-gray-900 sm:text-lg">Recent Activity</h3>
            </div>
            <div class="p-4 sm:p-6">
              <div class="space-y-3 sm:space-y-4">
                <div v-for="activity in recentActivities" :key="activity.id" class="flex items-start space-x-3 sm:space-x-4">
                  <div class="flex justify-center items-center w-8 h-8 rounded-full sm:w-10 sm:h-10" :class="activity.color">
                    <component :is="activity.icon" class="w-4 h-4 text-white sm:w-5 sm:h-5" />
                  </div>
                  <div class="flex-1 min-w-0">
                    <p class="text-sm font-medium text-gray-900 truncate">{{ activity.title }}</p>
                    <p class="text-xs text-gray-600 truncate sm:text-sm">{{ activity.description }}</p>
                    <p class="mt-1 text-xs text-slate-500 dark:text-slate-500">{{ formatDate(activity.date) }}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Quick Actions -->
        <div>
          <div class="bg-white rounded-xl border border-gray-200 shadow-sm">
            <div class="p-4 border-b border-gray-200 sm:p-6">
              <h3 class="text-base font-semibold text-gray-900 sm:text-lg">Quick Actions</h3>
            </div>
            <div class="p-4 sm:p-6">
              <div class="space-y-2 sm:space-y-3">
                <NuxtLink to="/crm" class="block p-3 w-full text-left rounded-lg border transition-colors border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 touch-manipulation">
                  <div class="flex items-center space-x-3">
                    <UsersIcon class="flex-shrink-0 w-5 h-5 text-blue-600" />
                    <div class="min-w-0">
                      <p class="text-sm font-medium text-gray-900">Manage CRM</p>
                      <p class="text-xs text-gray-600 truncate">View customers and opportunities</p>
                    </div>
                  </div>
                </NuxtLink>

                <NuxtLink to="/projects" class="block p-3 w-full text-left rounded-lg border transition-colors border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 touch-manipulation">
                  <div class="flex items-center space-x-3">
                    <BriefcaseIcon class="flex-shrink-0 w-5 h-5 text-green-600" />
                    <div class="min-w-0">
                      <p class="text-sm font-medium text-gray-900">View Projects</p>
                      <p class="text-xs text-gray-600 truncate">Track project progress</p>
                    </div>
                  </div>
                </NuxtLink>

                <NuxtLink to="/accounts" class="block p-3 w-full text-left rounded-lg border transition-colors border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 touch-manipulation">
                  <div class="flex items-center space-x-3">
                    <CurrencyDollarIcon class="flex-shrink-0 w-5 h-5 text-purple-600" />
                    <div class="min-w-0">
                      <p class="text-sm font-medium text-gray-900">Financial Reports</p>
                      <p class="text-xs text-gray-600 truncate">View financial data</p>
                    </div>
                  </div>
                </NuxtLink>

                <NuxtLink to="/stock" class="block p-3 w-full text-left rounded-lg border transition-colors border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 touch-manipulation">
                  <div class="flex items-center space-x-3">
                    <ArchiveBoxIcon class="flex-shrink-0 w-5 h-5 text-yellow-600" />
                    <div class="min-w-0">
                      <p class="text-sm font-medium text-gray-900">Inventory</p>
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
      <div class="grid grid-cols-1 gap-4 lg:grid-cols-2 sm:gap-6">
        <!-- Revenue Chart -->
        <div class="bg-white rounded-xl border border-gray-200 shadow-sm">
          <div class="p-4 border-b border-gray-200 sm:p-6">
            <h3 class="text-base font-semibold text-gray-900 sm:text-lg">Revenue Trend</h3>
          </div>
          <div class="p-4 sm:p-6">
            <div class="flex justify-center items-center h-48 rounded-lg sm:h-64 bg-slate-50 dark:bg-slate-700">
              <p class="text-sm text-slate-500 dark:text-slate-400">Revenue chart would be displayed here</p>
            </div>
          </div>
        </div>

        <!-- Sales Funnel -->
        <div class="bg-white rounded-xl border border-gray-200 shadow-sm">
          <div class="p-4 border-b border-gray-200 sm:p-6">
            <h3 class="text-base font-semibold text-gray-900 sm:text-lg">Sales Funnel</h3>
          </div>
          <div class="p-4 sm:p-6">
            <div class="space-y-3 sm:space-y-4">
              <div v-for="stage in salesFunnel" :key="stage.name" class="flex justify-between items-center">
                <div class="flex items-center space-x-2 min-w-0 sm:space-x-3">
                  <div class="flex-shrink-0 w-3 h-3 rounded-full sm:w-4 sm:h-4" :class="stage.color"></div>
                  <span class="text-sm font-medium text-gray-900 truncate">{{ stage.name }}</span>
                </div>
                <div class="flex flex-shrink-0 items-center space-x-2">
                  <span class="text-sm text-gray-600">{{ stage.count }}</span>
                  <div class="w-16 h-2 rounded-full sm:w-20 bg-slate-200 dark:bg-slate-600">
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

// Helper functions pipeline
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

