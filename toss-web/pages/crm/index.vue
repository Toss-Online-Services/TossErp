<template>
  <div class="min-h-screen bg-slate-50 dark:bg-slate-900">
    <!-- Mobile-First Page Container -->
    <div class="p-4 sm:p-6 space-y-4 sm:space-y-6 pb-20 lg:pb-6">
      <!-- Page Header -->
      <div class="text-center sm:text-left">
        <h1 class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">CRM Dashboard</h1>
        <p class="text-slate-600 dark:text-slate-400 mt-1 text-sm sm:text-base">Manage your customer relationships and sales pipeline.</p>
        <div class="bg-yellow-100 border border-yellow-300 p-3 mt-4 rounded">
          <p class="text-yellow-800">🔍 Debug: Page is loading properly. Store status: {{ pending ? 'Loading...' : 'Ready' }}</p>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="pending" class="flex justify-center items-center h-32 sm:h-64">
        <div class="animate-spin rounded-full h-8 w-8 sm:h-12 sm:w-12 border-b-2 border-blue-600"></div>
      </div>

      <!-- Quick Stats - Mobile First Grid -->
      <div v-else class="grid grid-cols-1 xs:grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-6">
        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Total Customers</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ analytics?.totalCustomers || 0 }}</p>
              <p class="text-xs sm:text-sm text-red-500">Debug: Stats = {{ customerStats?.total || 'undefined' }}</p>
              <p class="text-xs sm:text-sm text-blue-600">+8.2%</p>
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
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ analytics?.activeLeads || 0 }}</p>
              <p class="text-xs sm:text-sm text-yellow-600">{{ analytics?.activeLeads || 0 }} new</p>
            </div>
            <div class="p-2 sm:p-3 bg-yellow-100 dark:bg-yellow-900 rounded-full">
              <ChartBarIcon class="w-4 h-4 sm:w-6 sm:h-6 text-yellow-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Conversion Rate</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">{{ analytics?.conversionRate || 0 }}%</p>
              <p class="text-xs sm:text-sm text-green-600">+12.5%</p>
            </div>
            <div class="p-2 sm:p-3 bg-green-100 dark:bg-green-900 rounded-full">
              <ArrowTrendingUpIcon class="w-4 h-4 sm:w-6 sm:h-6 text-green-600" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Pipeline Value</p>
              <p class="text-lg sm:text-2xl font-bold text-slate-900 dark:text-white">R {{ formatCurrency(analytics?.pipelineValue || 0) }}</p>
              <p class="text-xs sm:text-sm text-purple-600">15 ops</p>
            </div>
            <div class="p-2 sm:p-3 bg-purple-100 dark:bg-purple-900 rounded-full">
              <CurrencyDollarIcon class="w-4 h-4 sm:w-6 sm:h-6 text-purple-600" />
            </div>
          </div>
        </div>
      </div>

      <!-- Main Content Grid - Mobile Responsive -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-4 sm:gap-6">
        <!-- Recent Activity -->
        <div>
          <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
            <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
              <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">Recent CRM Activity</h3>
              <p class="text-sm text-red-500">Debug: Activities count = {{ recentActivities.length }}</p>
            </div>
            <div class="p-4 sm:p-6">
              <div class="space-y-3 sm:space-y-4">
                <!-- Test with static content first -->
                <div class="flex items-start space-x-3 sm:space-x-4">
                  <div class="w-8 h-8 sm:w-10 sm:h-10 rounded-full flex items-center justify-center bg-blue-500">
                    <PhoneIcon class="w-4 h-4 sm:w-5 sm:h-5 text-white" />
                  </div>
                  <div class="flex-1 min-w-0">
                    <h4 class="text-sm font-medium text-slate-900 dark:text-white">Test Static Activity</h4>
                    <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">This is a test to see if templates are working</p>
                    <p class="text-xs text-slate-500 dark:text-slate-400">Just now</p>
                  </div>
                </div>
                
                <!-- Dynamic content with fallback -->
                <div v-if="recentActivities && recentActivities.length > 0">
                  <div v-for="activity in recentActivities" :key="activity.id" class="flex items-start space-x-3 sm:space-x-4">
                    <div class="w-8 h-8 sm:w-10 sm:h-10 rounded-full flex items-center justify-center" :class="getActivityColor(activity.type)">
                      <component :is="getActivityIcon(activity.type)" class="w-4 h-4 sm:w-5 sm:h-5 text-white" />
                    </div>
                    <div class="flex-1 min-w-0">
                      <p class="text-sm font-medium text-slate-900 dark:text-white truncate">{{ activity.title }}</p>
                      <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 truncate">{{ activity.description }}</p>
                      <p class="text-xs text-slate-500 dark:text-slate-500 mt-1">{{ formatDate(activity.date) }}</p>
                    </div>
                  </div>
                </div>
                
                <!-- Fallback for empty state -->
                <div v-else class="text-center py-8">
                  <p class="text-slate-500 dark:text-slate-400">No recent activities</p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- AI Co-Pilot Insights -->
    <div class="bg-gradient-to-r from-blue-50 to-purple-50 dark:from-blue-900/20 dark:to-purple-900/20 p-6 rounded-xl border border-blue-200 dark:border-blue-800">
      <div class="flex items-center mb-4">
        <div class="flex items-center justify-center w-10 h-10 mr-3 bg-blue-100 rounded-lg dark:bg-blue-900">
          <SparklesIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
        </div>
        <div>
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white">AI Customer Insights</h3>
          <p class="text-sm text-slate-600 dark:text-slate-400">Smart recommendations for your business</p>
        </div>
      </div>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div class="p-4 bg-white dark:bg-slate-800 rounded-lg border border-slate-200 dark:border-slate-700">
          <div class="flex items-center mb-2">
            <BoltIcon class="w-5 h-5 mr-2 text-yellow-500" />
            <span class="font-medium text-slate-900 dark:text-white">At-Risk Customers</span>
          </div>
          <p class="text-sm text-slate-600 dark:text-slate-400">3 customers haven't purchased in 30+ days</p>
          <button class="mt-2 text-sm text-blue-600 dark:text-blue-400 hover:text-blue-700">View & Follow Up</button>
        </div>
        <div class="p-4 bg-white dark:bg-slate-800 rounded-lg border border-slate-200 dark:border-slate-700">
          <div class="flex items-center mb-2">
            <UserGroupIcon class="w-5 h-5 mr-2 text-green-500" />
            <span class="font-medium text-slate-900 dark:text-white">Group Opportunities</span>
          </div>
          <p class="text-sm text-slate-600 dark:text-slate-400">5 nearby businesses need similar products</p>
          <button class="mt-2 text-sm text-blue-600 dark:text-blue-400 hover:text-blue-700">Create Group Order</button>
        </div>
      </div>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Collaborative CRM Network -->
        <div>
          <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
            <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
              <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">Network Collaboration</h3>
              <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">Connect with other TOSS businesses</p>
            </div>
            <div class="p-4 sm:p-6">
              <div class="space-y-3">
                <div class="flex items-center justify-between p-3 bg-green-50 dark:bg-green-900/20 rounded-lg border border-green-200 dark:border-green-800">
                  <div class="flex items-center">
                    <UserGroupIcon class="w-5 h-5 text-green-600 dark:text-green-400 mr-3" />
                    <div>
                      <p class="text-sm font-medium text-slate-900 dark:text-white">Customer Referrals</p>
                      <p class="text-xs text-slate-600 dark:text-slate-400">Share customers within network</p>
                    </div>
                  </div>
                  <button class="text-green-600 dark:text-green-400 hover:text-green-700">
                    <ChevronRightIcon class="w-4 h-4" />
                  </button>
                </div>
                
                <div class="flex items-center justify-between p-3 bg-blue-50 dark:bg-blue-900/20 rounded-lg border border-blue-200 dark:border-blue-800">
                  <div class="flex items-center">
                    <ChartBarIcon class="w-5 h-5 text-blue-600 dark:text-blue-400 mr-3" />
                    <div>
                      <p class="text-sm font-medium text-slate-900 dark:text-white">Shared Analytics</p>
                      <p class="text-xs text-slate-600 dark:text-slate-400">Compare with network benchmarks</p>
                    </div>
                  </div>
                  <button class="text-blue-600 dark:text-blue-400 hover:text-blue-700">
                    <ChevronRightIcon class="w-4 h-4" />
                  </button>
                </div>

                <div class="flex items-center justify-between p-3 bg-purple-50 dark:bg-purple-900/20 rounded-lg border border-purple-200 dark:border-purple-800">
                  <div class="flex items-center">
                    <CurrencyDollarIcon class="w-5 h-5 text-purple-600 dark:text-purple-400 mr-3" />
                    <div>
                      <p class="text-sm font-medium text-slate-900 dark:text-white">Group Credit Sales</p>
                      <p class="text-xs text-slate-600 dark:text-slate-400">Pooled credit for community</p>
                    </div>
                  </div>
                  <button class="text-purple-600 dark:text-purple-400 hover:text-purple-700">
                    <ChevronRightIcon class="w-4 h-4" />
                  </button>
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
                <button @click="showCreateCustomerModal = true" class="block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors touch-manipulation">
                  <div class="flex items-center space-x-3">
                    <UserPlusIcon class="w-5 h-5 text-blue-600 flex-shrink-0" />
                    <div class="min-w-0">
                      <p class="font-medium text-slate-900 dark:text-white text-sm">Add Customer</p>
                      <p class="text-xs text-slate-600 dark:text-slate-400 truncate">Create new customer record</p>
                    </div>
                  </div>
                </button>

                <button @click="showCreateLeadModal = true" class="block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors touch-manipulation">
                  <div class="flex items-center space-x-3">
                    <PlusIcon class="w-5 h-5 text-green-600 flex-shrink-0" />
                    <div class="min-w-0">
                      <p class="font-medium text-slate-900 dark:text-white text-sm">Add Lead</p>
                      <p class="text-xs text-slate-600 dark:text-slate-400 truncate">Create new lead opportunity</p>
                    </div>
                  </div>
                </button>

                <NuxtLink to="/crm/customers" class="block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors touch-manipulation">
                  <div class="flex items-center space-x-3">
                    <UsersIcon class="w-5 h-5 text-purple-600 flex-shrink-0" />
                    <div class="min-w-0">
                      <p class="font-medium text-slate-900 dark:text-white text-sm">View All Customers</p>
                      <p class="text-xs text-slate-600 dark:text-slate-400 truncate">Browse customer database</p>
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
                    <CurrencyDollarIcon class="w-5 h-5 text-yellow-600 flex-shrink-0" />
                    <div class="min-w-0">
                      <p class="font-medium text-slate-900 dark:text-white text-sm">Sales Opportunities</p>
                      <p class="text-xs text-slate-600 dark:text-slate-400 truncate">Track sales pipeline</p>
                    </div>
                  </div>
                </NuxtLink>

                <NuxtLink to="/crm/pipeline" class="block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors touch-manipulation">
                  <div class="flex items-center space-x-3">
                    <ChartBarIcon class="w-5 h-5 text-blue-600 flex-shrink-0" />
                    <div class="min-w-0">
                      <p class="font-medium text-slate-900 dark:text-white text-sm">Pipeline View</p>
                      <p class="text-xs text-slate-600 dark:text-slate-400 truncate">Visualize sales stages</p>
                    </div>
                  </div>
                </NuxtLink>

                <NuxtLink to="/crm/contacts" class="block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors touch-manipulation">
                  <div class="flex items-center space-x-3">
                    <PhoneIcon class="w-5 h-5 text-indigo-600 flex-shrink-0" />
                    <div class="min-w-0">
                      <p class="font-medium text-slate-900 dark:text-white text-sm">Contact Management</p>
                      <p class="text-xs text-slate-600 dark:text-slate-400 truncate">Manage contact details</p>
                    </div>
                  </div>
                </NuxtLink>

                <button @click="refreshCustomers" class="block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-700 transition-colors touch-manipulation">
                  <div class="flex items-center space-x-3">
                    <ArrowPathIcon class="w-5 h-5 text-yellow-600 flex-shrink-0" />
                    <div class="min-w-0">
                      <p class="font-medium text-slate-900 dark:text-white text-sm">Sync Data</p>
                      <p class="text-xs text-slate-600 dark:text-slate-400 truncate">Refresh customer data</p>
                    </div>
                  </div>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

      <!-- Charts and Analytics - Mobile Responsive -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-4 sm:gap-6">
        <!-- Sales Pipeline -->
        <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
            <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">Sales Pipeline</h3>
          </div>
          <div class="p-4 sm:p-6">
            <div class="space-y-3 sm:space-y-4">
              <div v-for="stage in pipelineStages" :key="stage.name" class="flex items-center justify-between">
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

        <!-- Customer Segments -->
        <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
          <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
            <h3 class="text-base sm:text-lg font-semibold text-slate-900 dark:text-white">Customer Segments</h3>
          </div>
          <div class="p-4 sm:p-6">
            <div class="space-y-3 sm:space-y-4">
              <div v-for="segment in customerSegments" :key="segment.name" class="flex items-center justify-between">
                <div class="flex items-center space-x-2 sm:space-x-3 min-w-0">
                  <div class="w-3 h-3 sm:w-4 sm:h-4 rounded-full flex-shrink-0" :class="segment.color"></div>
                  <span class="text-sm font-medium text-slate-900 dark:text-white truncate">{{ segment.name }}</span>
                </div>
                <div class="flex items-center space-x-2 flex-shrink-0">
                  <span class="text-sm text-slate-600 dark:text-slate-400">{{ segment.count }}</span>
                  <div class="w-16 sm:w-20 bg-slate-200 dark:bg-slate-600 rounded-full h-2">
                    <div class="h-2 rounded-full transition-all duration-300" :class="segment.color" :style="{ width: segment.percentage + '%' }"></div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Mobile-First Create Customer Modal -->
    <div v-if="showCreateCustomerModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div class="bg-white dark:bg-slate-800 rounded-lg w-full max-w-md max-h-[90vh] overflow-y-auto">
        <div class="sticky top-0 bg-white dark:bg-slate-800 p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Create New Customer</h3>
        </div>
        <form @submit.prevent="createCustomer" class="p-4 sm:p-6">
          <div class="space-y-4">
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">First Name</label>
                <input v-model="newCustomer.firstName" type="text" required 
                       class="w-full border border-slate-300 dark:border-slate-600 rounded-lg px-3 py-2 bg-white dark:bg-slate-700 text-slate-900 dark:text-white text-sm">
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Last Name</label>
                <input v-model="newCustomer.lastName" type="text" required 
                       class="w-full border border-slate-300 dark:border-slate-600 rounded-lg px-3 py-2 bg-white dark:bg-slate-700 text-slate-900 dark:text-white text-sm">
              </div>
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Email</label>
              <input v-model="newCustomer.email" type="email" required 
                     class="w-full border border-slate-300 dark:border-slate-600 rounded-lg px-3 py-2 bg-white dark:bg-slate-700 text-slate-900 dark:text-white text-sm">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Phone</label>
              <input v-model="newCustomer.phone" type="tel" required 
                     class="w-full border border-slate-300 dark:border-slate-600 rounded-lg px-3 py-2 bg-white dark:bg-slate-700 text-slate-900 dark:text-white text-sm">
            </div>
            <div>
              <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-1">Address</label>
              <textarea v-model="newCustomer.address" required rows="3"
                        class="w-full border border-slate-300 dark:border-slate-600 rounded-lg px-3 py-2 bg-white dark:bg-slate-700 text-slate-900 dark:text-white text-sm resize-none"></textarea>
            </div>
          </div>
          <div class="flex flex-col sm:flex-row justify-end gap-3 mt-6">
            <button type="button" @click="showCreateCustomerModal = false" 
                    class="order-2 sm:order-1 px-4 py-2 border border-slate-300 rounded-lg text-slate-700 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-700 text-sm">
              Cancel
            </button>
            <button type="submit" :disabled="creatingCustomer" 
                    class="order-1 sm:order-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50 text-sm">
              {{ creatingCustomer ? 'Creating...' : 'Create Customer' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { 
  PlusIcon, 
  UserPlusIcon, 
  UsersIcon, 
  ChartBarIcon, 
  ArrowTrendingUpIcon, 
  CurrencyDollarIcon, 
  ArrowPathIcon,
  PhoneIcon,
  EnvelopeIcon,
  CalendarIcon,
  DocumentIcon,
  SparklesIcon,
  BoltIcon,
  UserGroupIcon,
  ChevronRightIcon
} from '@heroicons/vue/24/outline'
import { useCustomerStore } from '../../stores/customers'

// Page metadata would be set via definePageMeta in real Nuxt 3 app

// Initialize store
const customersStore = useCustomerStore()

// Reactive data
const showCreateLeadModal = ref(false)
const showCreateCustomerModal = ref(false)
const creatingCustomer = ref(false)

const newCustomer = ref({
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  address: ''
})

// Mount lifecycle
onMounted(async () => {
  console.log('CRM page mounted')
  console.log('Customer store:', customersStore)
  await customersStore.fetchCustomers()
  console.log('Customers loaded:', customersStore.customers)
  console.log('Customer stats:', customersStore.customerStats)
})

// Computed properties from store
const customers = computed(() => {
  console.log('Customers computed:', customersStore.customers)
  return customersStore.customers
})
const customerStats = computed(() => {
  console.log('Customer stats computed:', customersStore.customerStats)
  return customersStore.customerStats
})
const pending = computed(() => customersStore.loading)

// Mock analytics data based on customer store
const analytics = computed(() => ({
  totalCustomers: customerStats.value.total || 1428,
  activeLeads: customerStats.value.leads || 87,
  conversionRate: Math.round((customerStats.value.active / customerStats.value.total) * 100) || 72,
  pipelineValue: 892000 // Mock pipeline value
}))

// Mock pipeline data
const pipelineStages = computed(() => [
  {
    name: 'Prospecting',
    count: 12,
    percentage: 100,
    color: 'bg-blue-500',
    opportunities: [
      { id: 1, customer: 'TechCorp Ltd', product: 'ERP Package', value: 45000 },
      { id: 2, customer: 'NewCo Inc', product: 'CRM Solution', value: 25000 }
    ]
  },
  {
    name: 'Qualification',
    count: 8,
    percentage: 67,
    color: 'bg-green-500',
    opportunities: [
      { id: 3, customer: 'StartupCo', product: 'Basic Package', value: 15000 }
    ]
  },
  {
    name: 'Proposal',
    count: 5,
    percentage: 42,
    color: 'bg-yellow-500',
    opportunities: [
      { id: 4, customer: 'BigCorp', product: 'Enterprise Suite', value: 85000 }
    ]
  },
  {
    name: 'Closed Won',
    count: 3,
    percentage: 25,
    color: 'bg-purple-500',
    opportunities: [
      { id: 5, customer: 'MegaCorp', product: 'Full Integration', value: 120000 }
    ]
  }
])

const customerSegments = ref([
  { name: 'Premium', count: 28, percentage: 100, color: 'bg-purple-500' },
  { name: 'Gold', count: 45, percentage: 80, color: 'bg-yellow-500' },
  { name: 'Silver', count: 67, percentage: 60, color: 'bg-slate-500' },
  { name: 'Regular', count: 124, percentage: 40, color: 'bg-blue-500' }
])

const recentActivities = computed(() => [
  { id: 1, type: 'call', title: 'New Customer Call', description: 'TechCorp Ltd interested in ERP solution', date: new Date() },
  { id: 2, type: 'email', title: 'Follow-up Email', description: 'Follow-up sent to BigCorp for proposal review', date: new Date(Date.now() - 3600000) },
  { id: 3, type: 'meeting', title: 'Demo Scheduled', description: 'Demo scheduled with StartupCo next week', date: new Date(Date.now() - 7200000) },
  { id: 4, type: 'note', title: 'Contact Updated', description: 'Updated contact info for John Doe at Acme Corp', date: new Date(Date.now() - 86400000) }
])

// Methods
async function createCustomer() {
  creatingCustomer.value = true
  try {
    const customerData = {
      ...newCustomer.value,
      fullName: `${newCustomer.value.firstName} ${newCustomer.value.lastName}`,
      status: 'Lead' as const,
      company: '',
      totalSpent: 0,
      lastPurchaseDate: null,
      notes: '',
      subscriptions: []
    }
    
    await customersStore.createCustomer(customerData)
    
    // Reset form and close modal
    newCustomer.value = {
      firstName: '',
      lastName: '',
      email: '',
      phone: '',
      address: ''
    }
    showCreateCustomerModal.value = false
    
    // Show success message
    showToast('Customer created successfully!', 'success')
  } catch (error) {
    console.error('Error creating customer:', error)
    showToast('Failed to create customer. Please try again.', 'error')
  } finally {
    creatingCustomer.value = false
  }
}

async function refreshCustomers() {
  await customersStore.fetchCustomers()
}

// Toast notification system (simple implementation)
function showToast(message: string, type: 'success' | 'error' | 'warning' = 'success') {
  // Simple alert for now - could be enhanced with a proper toast component
  alert(message)
}

// Helper functions
function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('en-ZA', {
    style: 'decimal',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

function formatDate(date: Date): string {
  return new Intl.DateTimeFormat('en-ZA', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date)
}

function getActivityColor(type: string): string {
  switch (type) {
    case 'call': return 'bg-blue-500'
    case 'email': return 'bg-green-500'
    case 'meeting': return 'bg-purple-500'
    case 'note': return 'bg-yellow-500'
    default: return 'bg-slate-500'
  }
}

function getActivityIcon(type: string) {
  switch (type) {
    case 'call': return PhoneIcon
    case 'email': return EnvelopeIcon
    case 'meeting': return CalendarIcon
    case 'note': return DocumentIcon
    default: return DocumentIcon
  }
}
</script>

<style scoped>
/* Mobile-first responsive utilities */
@media (min-width: 475px) {
  .xs\:inline {
    display: inline;
  }
  .xs\:hidden {
    display: none;
  }
}

/* Touch-friendly interactions */
.touch-manipulation {
  touch-action: manipulation;
  -webkit-tap-highlight-color: transparent;
}

/* Improved mobile scrolling */
.overflow-touch {
  -webkit-overflow-scrolling: touch;
}

/* Custom mobile-optimized spacing */
.mobile-safe-area {
  padding-left: env(safe-area-inset-left);
  padding-right: env(safe-area-inset-right);
}

/* Offline indicator pulse */
@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: .5; }
}

.offline-pulse {
  animation: pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite;
}
</style>
