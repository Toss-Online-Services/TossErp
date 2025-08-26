<template>
  <div class="p-6 space-y-6">
    <!-- Page Header -->
    <div>
      <h1 class="text-3xl font-bold text-slate-900 dark:text-white">Dashboard</h1>
      <p class="text-slate-600 dark:text-slate-400 mt-1">Welcome back! Here's what's happening with your business today.</p>
    </div>

    <!-- Quick Stats -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
      <div class="bg-white dark:bg-slate-800 p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-slate-600 dark:text-slate-400">Total Revenue</p>
            <p class="text-2xl font-bold text-slate-900 dark:text-white">R 1,234,567</p>
            <p class="text-sm text-green-600">+12.5% from last month</p>
          </div>
          <div class="p-3 bg-green-100 dark:bg-green-900 rounded-full">
            <CurrencyDollarIcon class="w-6 h-6 text-green-600" />
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-slate-800 p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-slate-600 dark:text-slate-400">Active Customers</p>
            <p class="text-2xl font-bold text-slate-900 dark:text-white">1,428</p>
            <p class="text-sm text-blue-600">+8.2% from last month</p>
          </div>
          <div class="p-3 bg-blue-100 dark:bg-blue-900 rounded-full">
            <UsersIcon class="w-6 h-6 text-blue-600" />
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-slate-800 p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-slate-600 dark:text-slate-400">Open Projects</p>
            <p class="text-2xl font-bold text-slate-900 dark:text-white">24</p>
            <p class="text-sm text-yellow-600">3 due this week</p>
          </div>
          <div class="p-3 bg-yellow-100 dark:bg-yellow-900 rounded-full">
            <BriefcaseIcon class="w-6 h-6 text-yellow-600" />
          </div>
        </div>
      </div>

      <div class="bg-white dark:bg-slate-800 p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-slate-600 dark:text-slate-400">Pipeline Value</p>
            <p class="text-2xl font-bold text-slate-900 dark:text-white">R 892,340</p>
            <p class="text-sm text-purple-600">15 opportunities</p>
          </div>
          <div class="p-3 bg-purple-100 dark:bg-purple-900 rounded-full">
            <ArrowTrendingUpIcon class="w-6 h-6 text-purple-600" />
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content Grid -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Recent Activity -->
      <div class="lg:col-span-2">
        <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="p-6 border-b border-slate-200 dark:border-slate-700">
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Recent Activity</h3>
          </div>
          <div class="p-6">
            <div class="space-y-4">
              <div v-for="activity in recentActivities" :key="activity.id" class="flex items-start space-x-4">
                <div class="w-10 h-10 rounded-full flex items-center justify-center" :class="activity.color">
                  <component :is="activity.icon" class="w-5 h-5 text-white" />
                </div>
                <div class="flex-1">
                  <p class="text-sm font-medium text-slate-900 dark:text-white">{{ activity.title }}</p>
                  <p class="text-sm text-slate-600 dark:text-slate-400">{{ activity.description }}</p>
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
          <div class="p-6 border-b border-slate-200 dark:border-slate-700">
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Quick Actions</h3>
          </div>
          <div class="p-6">
            <div class="space-y-3">
              <NuxtLink to="/crm" class="block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors">
                <div class="flex items-center space-x-3">
                  <UsersIcon class="w-5 h-5 text-blue-600" />
                  <div>
                    <p class="font-medium text-slate-900 dark:text-white">Manage CRM</p>
                    <p class="text-xs text-slate-600 dark:text-slate-400">View customers and opportunities</p>
                  </div>
                </div>
              </NuxtLink>

              <NuxtLink to="/projects" class="block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors">
                <div class="flex items-center space-x-3">
                  <BriefcaseIcon class="w-5 h-5 text-green-600" />
                  <div>
                    <p class="font-medium text-slate-900 dark:text-white">View Projects</p>
                    <p class="text-xs text-slate-600 dark:text-slate-400">Track project progress</p>
                  </div>
                </div>
              </NuxtLink>

              <NuxtLink to="/accounts" class="block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors">
                <div class="flex items-center space-x-3">
                  <CurrencyDollarIcon class="w-5 h-5 text-purple-600" />
                  <div>
                    <p class="font-medium text-slate-900 dark:text-white">Financial Reports</p>
                    <p class="text-xs text-slate-600 dark:text-slate-400">View financial data</p>
                  </div>
                </div>
              </NuxtLink>

              <NuxtLink to="/stock" class="block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors">
                <div class="flex items-center space-x-3">
                  <ArchiveBoxIcon class="w-5 h-5 text-yellow-600" />
                  <div>
                    <p class="font-medium text-slate-900 dark:text-white">Inventory</p>
                    <p class="text-xs text-slate-600 dark:text-slate-400">Manage stock levels</p>
                  </div>
                </div>
              </NuxtLink>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Charts and Analytics -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Revenue Chart -->
      <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
        <div class="p-6 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Revenue Trend</h3>
        </div>
        <div class="p-6">
          <div class="h-64 flex items-center justify-center bg-slate-50 dark:bg-slate-700 rounded-lg">
            <p class="text-slate-500 dark:text-slate-400">Revenue chart would be displayed here</p>
          </div>
        </div>
      </div>

      <!-- Sales Funnel -->
      <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
        <div class="p-6 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Sales Funnel</h3>
        </div>
        <div class="p-6">
          <div class="space-y-4">
            <div v-for="stage in salesFunnel" :key="stage.name" class="flex items-center justify-between">
              <div class="flex items-center space-x-3">
                <div class="w-4 h-4 rounded-full" :class="stage.color"></div>
                <span class="text-sm font-medium text-slate-900 dark:text-white">{{ stage.name }}</span>
              </div>
              <div class="flex items-center space-x-2">
                <span class="text-sm text-slate-600 dark:text-slate-400">{{ stage.count }}</span>
                <div class="w-20 bg-slate-200 dark:bg-slate-600 rounded-full h-2">
                  <div class="h-2 rounded-full" :class="stage.color" :style="{ width: stage.percentage + '%' }"></div>
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
</script>

