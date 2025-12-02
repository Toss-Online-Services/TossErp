<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-green-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 sm:py-6">
        <div class="flex items-center justify-between">
          <div class="flex-1 min-w-0">
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-green-600 to-blue-600 bg-clip-text text-transparent">
              Customer Relationships
            </h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
              Manage customer loyalty and credit
            </p>
          </div>
          <div class="flex space-x-2 sm:space-x-3 flex-shrink-0">
            <NuxtLink
              to="/crm/customers"
              class="inline-flex items-center justify-center px-4 sm:px-6 py-2.5 sm:py-3 bg-gradient-to-r from-green-600 to-blue-600 text-white rounded-xl hover:from-green-700 hover:to-blue-700 shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 font-semibold text-sm sm:text-base"
            >
              <UsersIcon class="w-5 h-5 mr-2" />
              View All Customers
            </NuxtLink>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      
      <!-- Key Metrics -->
      <div class="grid grid-cols-2 sm:grid-cols-4 gap-4 sm:gap-6 mb-8">
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Total Customers</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ metrics.totalCustomers }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl">
              <UsersIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Loyal Customers</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ metrics.loyalCustomers }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-blue-500 to-indigo-600 rounded-xl">
              <StarIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Credit Customers</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ metrics.creditCustomers }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-orange-500 to-amber-600 rounded-xl">
              <CreditCardIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Avg Lifetime Value</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">R{{ metrics.avgLifetimeValue }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-purple-500 to-pink-600 rounded-xl">
              <CurrencyDollarIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>
      </div>

      <!-- Charts Row -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        <!-- Customer Growth -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <div class="flex items-center justify-between mb-6">
            <div>
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Customer Growth</h3>
              <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">Last 30 days</p>
            </div>
          </div>
          <LineChart
            :labels="['Week 1', 'Week 2', 'Week 3', 'Week 4']"
            :data="[145, 162, 178, 189]"
            label="Customers"
            color="#10B981"
            :height="250"
          />
        </div>

        <!-- Customer Segments -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6">
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-6">Customer Segments</h3>
          <div class="space-y-4">
            <div>
              <div class="flex justify-between text-sm mb-2">
                <span class="text-slate-600 dark:text-slate-400">Regular (Weekly+)</span>
                <span class="font-medium text-slate-900 dark:text-white">{{ metrics.regularCustomers }} ({{ Math.round((metrics.regularCustomers / metrics.totalCustomers) * 100) }}%)</span>
              </div>
              <div class="w-full bg-slate-200 rounded-full h-3 dark:bg-slate-700">
                <div class="bg-gradient-to-r from-green-500 to-emerald-600 h-3 rounded-full" :style="{ width: `${(metrics.regularCustomers / metrics.totalCustomers) * 100}%` }"></div>
              </div>
            </div>

            <div>
              <div class="flex justify-between text-sm mb-2">
                <span class="text-slate-600 dark:text-slate-400">Occasional</span>
                <span class="font-medium text-slate-900 dark:text-white">{{ metrics.occasionalCustomers }} ({{ Math.round((metrics.occasionalCustomers / metrics.totalCustomers) * 100) }}%)</span>
              </div>
              <div class="w-full bg-slate-200 rounded-full h-3 dark:bg-slate-700">
                <div class="bg-gradient-to-r from-blue-500 to-indigo-600 h-3 rounded-full" :style="{ width: `${(metrics.occasionalCustomers / metrics.totalCustomers) * 100}%` }"></div>
              </div>
            </div>

            <div>
              <div class="flex justify-between text-sm mb-2">
                <span class="text-slate-600 dark:text-slate-400">At Risk</span>
                <span class="font-medium text-slate-900 dark:text-white">{{ metrics.atRiskCustomers }} ({{ Math.round((metrics.atRiskCustomers / metrics.totalCustomers) * 100) }}%)</span>
              </div>
              <div class="w-full bg-slate-200 rounded-full h-3 dark:bg-slate-700">
                <div class="bg-gradient-to-r from-orange-500 to-red-600 h-3 rounded-full" :style="{ width: `${(metrics.atRiskCustomers / metrics.totalCustomers) * 100}%` }"></div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Actions Grid -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <!-- View All Customers -->
        <NuxtLink
          to="/crm/customers"
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 group"
        >
          <div class="flex items-center mb-4">
            <div class="p-3 bg-gradient-to-br from-green-100 to-green-200 dark:from-green-900/30 dark:to-green-900/20 rounded-xl group-hover:scale-110 transition-transform">
              <UsersIcon class="w-8 h-8 text-green-600 dark:text-green-400" />
            </div>
            <div class="ml-4">
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Customer Directory</h3>
              <p class="text-sm text-slate-600 dark:text-slate-400">View all customers</p>
            </div>
          </div>
          <div class="text-sm text-slate-600 dark:text-slate-400">
            Manage {{ metrics.totalCustomers }} customers with detailed profiles & purchase history
          </div>
        </NuxtLink>

        <!-- Credit Management -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 group cursor-pointer">
          <div class="flex items-center mb-4">
            <div class="p-3 bg-gradient-to-br from-orange-100 to-orange-200 dark:from-orange-900/30 dark:to-orange-900/20 rounded-xl group-hover:scale-110 transition-transform">
              <CreditCardIcon class="w-8 h-8 text-orange-600 dark:text-orange-400" />
            </div>
            <div class="ml-4">
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Credit Management</h3>
              <p class="text-sm text-slate-600 dark:text-slate-400">Track credit accounts</p>
            </div>
          </div>
          <div class="space-y-2 text-sm">
            <div class="flex justify-between">
              <span class="text-slate-600 dark:text-slate-400">Total Credit Out:</span>
              <span class="font-medium text-orange-600">R{{ metrics.totalCreditOut }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-slate-600 dark:text-slate-400">Overdue:</span>
              <span class="font-medium text-red-600">R{{ metrics.overdueCredit }}</span>
            </div>
          </div>
        </div>

        <!-- Loyalty Program -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 group cursor-pointer">
          <div class="flex items-center mb-4">
            <div class="p-3 bg-gradient-to-br from-blue-100 to-blue-200 dark:from-blue-900/30 dark:to-blue-900/20 rounded-xl group-hover:scale-110 transition-transform">
              <StarIcon class="w-8 h-8 text-blue-600 dark:text-blue-400" />
            </div>
            <div class="ml-4">
              <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Loyalty Program</h3>
              <p class="text-sm text-slate-600 dark:text-slate-400">Reward your customers</p>
            </div>
          </div>
          <div class="space-y-2 text-sm">
            <div class="flex justify-between">
              <span class="text-slate-600 dark:text-slate-400">Members:</span>
              <span class="font-medium text-slate-900 dark:text-white">{{ metrics.loyaltyMembers }}</span>
            </div>
            <div class="flex justify-between">
              <span class="text-slate-600 dark:text-slate-400">Avg Points:</span>
              <span class="font-medium text-blue-600">{{ metrics.avgLoyaltyPoints }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Customers -->
      <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 overflow-hidden">
        <div class="px-6 py-4 border-b border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Recently Active Customers</h3>
            <NuxtLink
              to="/crm/customers"
              class="text-sm text-green-600 dark:text-green-400 hover:text-green-700 dark:hover:text-green-300 font-medium"
            >
              View All →
            </NuxtLink>
          </div>
        </div>

        <div class="divide-y divide-slate-200 dark:divide-slate-700">
          <div
            v-for="customer in recentCustomers"
            :key="customer.id"
            class="px-6 py-4 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors cursor-pointer"
            @click="navigateTo(`/crm/customers/${customer.id}`)"
          >
            <div class="flex items-center justify-between">
              <div class="flex items-center">
                <div class="flex-shrink-0 h-12 w-12 bg-green-100 dark:bg-green-900/30 rounded-full flex items-center justify-center">
                  <span class="text-green-600 dark:text-green-400 font-semibold text-lg">
                    {{ customer.name.charAt(0) }}
                  </span>
                </div>
                <div class="ml-4">
                  <div class="text-sm font-medium text-slate-900 dark:text-white">{{ customer.name }}</div>
                  <div class="text-sm text-slate-500 dark:text-slate-400">{{ customer.phone }}</div>
                </div>
              </div>
              <div class="text-right">
                <div class="text-sm font-medium text-slate-900 dark:text-white">R{{ customer.totalSpent }}</div>
                <div class="text-sm text-slate-500 dark:text-slate-400">{{ customer.visits }} visits</div>
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
import {
  UsersIcon,
  StarIcon,
  CreditCardIcon,
  CurrencyDollarIcon
} from '@heroicons/vue/24/outline'

useHead({
  title: 'CRM Dashboard - TOSS ERP',
  meta: [
    { name: 'description', content: 'Customer relationship management overview' }
  ]
})

// Metrics
const metrics = ref({
  totalCustomers: 189,
  loyalCustomers: 67,
  creditCustomers: 24,
  avgLifetimeValue: 2845,
  regularCustomers: 98,
  occasionalCustomers: 65,
  atRiskCustomers: 26,
  totalCreditOut: 15240,
  overdueCredit: 3200,
  loyaltyMembers: 142,
  avgLoyaltyPoints: 385
})

// Recent customers
const recentCustomers = ref([
  {
    id: 'CUST-001',
    name: 'Nomsa Dlamini',
    phone: '+27 82 345 6789',
    totalSpent: 4250,
    visits: 45
  },
  {
    id: 'CUST-002',
    name: 'Sipho Ndlovu',
    phone: '+27 83 456 7890',
    totalSpent: 3890,
    visits: 38
  },
  {
    id: 'CUST-003',
    name: 'Lerato Molefe',
    phone: '+27 84 567 8901',
    totalSpent: 2340,
    visits: 28
  },
  {
    id: 'CUST-004',
    name: 'Thandi Zulu',
    phone: '+27 85 678 9012',
    totalSpent: 1980,
    visits: 22
  }
])
</script>

