<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Mobile-First Page Container -->
    <div class="p-4 pb-20 space-y-4 sm:p-6 sm:space-y-6 lg:pb-6">
      <!-- Language Switcher - Top Right -->
      <div class="flex justify-end mb-4">
        <LanguageSwitcher />
      </div>
      
      <!-- Hero Section -->
      <HeroSection />

      <!-- WhatsApp Quick Order -->
      <div class="mb-4">
        <WhatsAppChatPlaceholder />
      </div>
      
      <!-- Group Buying Feature -->
      <GroupBuyingCard />

      <!-- Quick Stats - Spaza Shop Focused -->
      <div class="grid grid-cols-1 gap-3 xs:grid-cols-2 lg:grid-cols-4 sm:gap-6">
        <div class="p-5 bg-white rounded-xl border-2 border-gray-200 shadow-sm sm:p-6 hover:border-blue-500 transition-colors">
          <div class="flex justify-between items-center">
            <div>
              <p class="text-base text-gray-700 sm:text-lg font-medium">Today's Sales</p>
              <p class="text-2xl font-bold text-gray-900 sm:text-3xl">R 1,245</p>
              <p class="text-base text-green-600 sm:text-lg font-semibold">+15% from yesterday</p>
            </div>
            <div class="p-3 bg-green-100 rounded-full sm:p-4">
              <CurrencyDollarIcon class="w-7 h-7 text-green-600 sm:w-8 sm:h-8" />
            </div>
          </div>
        </div>

        <div class="p-5 bg-white rounded-xl border-2 border-gray-200 shadow-sm sm:p-6 hover:border-blue-500 transition-colors">
          <div class="flex justify-between items-center">
            <div>
              <p class="text-base text-gray-700 sm:text-lg font-medium">Stock Items</p>
              <p class="text-2xl font-bold text-gray-900 sm:text-3xl">156</p>
              <p class="text-base text-orange-600 sm:text-lg font-semibold">12 items low</p>
            </div>
            <div class="p-3 bg-blue-100 rounded-full sm:p-4">
              <ArchiveBoxIcon class="w-7 h-7 text-blue-600 sm:w-8 sm:h-8" />
            </div>
          </div>
        </div>

        <div class="p-5 bg-white rounded-xl border-2 border-gray-200 shadow-sm sm:p-6 hover:border-blue-500 transition-colors">
          <div class="flex justify-between items-center">
            <div>
              <p class="text-base text-gray-700 sm:text-lg font-medium">Pending Orders</p>
              <p class="text-2xl font-bold text-gray-900 sm:text-3xl">3</p>
              <p class="text-base text-yellow-600 sm:text-lg font-semibold">1 arrives today</p>
            </div>
            <div class="p-3 bg-yellow-100 rounded-full sm:p-4">
              <ShoppingCartIcon class="w-7 h-7 text-yellow-600 sm:w-8 sm:h-8" />
            </div>
          </div>
        </div>

        <div class="p-5 bg-white rounded-xl border-2 border-gray-200 shadow-sm sm:p-6 hover:border-blue-500 transition-colors">
          <div class="flex justify-between items-center">
            <div>
              <p class="text-base text-gray-700 sm:text-lg font-medium">This Month</p>
              <p class="text-2xl font-bold text-gray-900 sm:text-3xl">R 28.5K</p>
              <p class="text-base text-purple-600 sm:text-lg font-semibold">Best month yet!</p>
            </div>
            <div class="p-3 bg-purple-100 rounded-full sm:p-4">
              <ArrowTrendingUpIcon class="w-7 h-7 text-purple-600 sm:w-8 sm:h-8" />
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Delivery Timeline -->
      <div class="lg:col-span-2">
        <DeliveryTimeline />
      </div>
      
      <!-- Main Content Grid - Mobile Responsive -->
      <div class="grid grid-cols-1 gap-4 lg:grid-cols-3 sm:gap-6">
        <!-- Recent Activity -->
        <div class="lg:col-span-2">
          <div class="bg-white rounded-xl border-2 border-gray-200 shadow-sm">
            <div class="p-4 border-b-2 border-gray-200 sm:p-6">
              <h3 class="text-lg font-bold text-gray-900 sm:text-xl">What's Happening</h3>
            </div>
            <div class="p-4 sm:p-6">
              <div class="space-y-4 sm:space-y-5">
                <div v-for="activity in recentActivities" :key="activity.id" class="flex items-start space-x-4 sm:space-x-5 p-3 rounded-lg hover:bg-gray-50 transition-colors">
                  <div class="flex justify-center items-center w-10 h-10 rounded-full sm:w-12 sm:h-12" :class="activity.color">
                    <component :is="activity.icon" class="w-5 h-5 text-white sm:w-6 sm:h-6" />
                  </div>
                  <div class="flex-1 min-w-0">
                    <p class="text-base font-semibold text-gray-900 sm:text-lg">{{ activity.title }}</p>
                    <p class="text-sm text-gray-700 sm:text-base">{{ activity.description }}</p>
                    <p class="mt-1 text-sm text-gray-500">{{ formatDate(activity.date) }}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Quick Actions -->
        <div>
          <div class="bg-white rounded-xl border-2 border-gray-200 shadow-sm">
            <div class="p-4 border-b-2 border-gray-200 sm:p-6">
              <h3 class="text-lg font-bold text-gray-900 sm:text-xl">Quick Actions</h3>
            </div>
            <div class="p-4 sm:p-6">
              <div class="space-y-3 sm:space-y-4">
                <NuxtLink to="/stock/order" class="block p-4 w-full text-left rounded-xl border-2 transition-all border-blue-200 hover:border-blue-500 hover:bg-blue-50 hover:shadow-md touch-manipulation min-h-[64px]">
                  <div class="flex items-center space-x-4">
                    <div class="p-2 bg-blue-500 rounded-lg">
                      <ShoppingCartIcon class="flex-shrink-0 w-6 h-6 text-white" />
                    </div>
                    <div class="min-w-0">
                      <p class="text-base font-bold text-gray-900 sm:text-lg">Order Stock</p>
                      <p class="text-sm text-gray-600">Place a new order</p>
                    </div>
                  </div>
                </NuxtLink>

                <NuxtLink to="/stock/track" class="block p-4 w-full text-left rounded-xl border-2 transition-all border-green-200 hover:border-green-500 hover:bg-green-50 hover:shadow-md touch-manipulation min-h-[64px]">
                  <div class="flex items-center space-x-4">
                    <div class="p-2 bg-green-500 rounded-lg">
                      <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                      </svg>
                    </div>
                    <div class="min-w-0">
                      <p class="text-base font-bold text-gray-900 sm:text-lg">Track Orders</p>
                      <p class="text-sm text-gray-600">See delivery status</p>
                    </div>
                  </div>
                </NuxtLink>

                <NuxtLink to="/stock" class="block p-4 w-full text-left rounded-xl border-2 transition-all border-purple-200 hover:border-purple-500 hover:bg-purple-50 hover:shadow-md touch-manipulation min-h-[64px]">
                  <div class="flex items-center space-x-4">
                    <div class="p-2 bg-purple-500 rounded-lg">
                      <ArchiveBoxIcon class="flex-shrink-0 w-6 h-6 text-white" />
                    </div>
                    <div class="min-w-0">
                      <p class="text-base font-bold text-gray-900 sm:text-lg">My Stock</p>
                      <p class="text-sm text-gray-600">View inventory</p>
                    </div>
                  </div>
                </NuxtLink>

                <NuxtLink to="/group-buying" class="block p-4 w-full text-left rounded-xl border-2 transition-all border-yellow-200 hover:border-yellow-500 hover:bg-yellow-50 hover:shadow-md touch-manipulation min-h-[64px]">
                  <div class="flex items-center space-x-4">
                    <div class="p-2 bg-yellow-500 rounded-lg">
                      <UsersIcon class="flex-shrink-0 w-6 h-6 text-white" />
                    </div>
                    <div class="min-w-0">
                      <p class="text-base font-bold text-gray-900 sm:text-lg">Group Buying</p>
                      <p class="text-sm text-gray-600">Save with neighbors</p>
                    </div>
                  </div>
                </NuxtLink>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <!-- WhatsApp Support Button -->
      <WhatsAppSupport />

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
    title: 'Stock Order Delivered',
    description: 'Your order of bread, milk, and chips arrived',
    date: new Date(),
    icon: ShoppingCartIcon,
    color: 'bg-green-500'
  },
  {
    id: 2,
    title: 'Low Stock Alert',
    description: 'Cool drinks running low - only 12 bottles left',
    date: new Date(Date.now() - 3600000),
    icon: ArchiveBoxIcon,
    color: 'bg-orange-500'
  },
  {
    id: 3,
    title: 'Group Order Ready',
    description: 'Join group order for sugar - closing in 2 days',
    date: new Date(Date.now() - 7200000),
    icon: UsersIcon,
    color: 'bg-purple-500'
  },
  {
    id: 4,
    title: 'Payment Received',
    description: 'R245 payment received from WhatsApp order',
    date: new Date(Date.now() - 86400000),
    icon: CurrencyDollarIcon,
    color: 'bg-blue-500'
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

