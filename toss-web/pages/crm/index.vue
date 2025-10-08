<template>
  <div>
    <!-- Page Container matching main dashboard -->
    <div class="p-4 pb-20 space-y-4 sm:p-6 sm:space-y-6 lg:pb-6">
      <!-- Page Header -->
      <div class="text-center sm:text-left">
        <div class="flex flex-col space-y-4 sm:flex-row sm:items-center sm:justify-between sm:space-y-0">
          <div>
            <h1 class="text-2xl font-bold sm:text-3xl text-slate-900 dark:text-white">
              CRM Dashboard
            </h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400 sm:text-base">
              Manage your customer relationships with AI-powered insights
            </p>
          </div>
          <div class="flex flex-col gap-3 sm:flex-row">
            <button @click="showCreateCustomerModal = true" 
                    class="inline-flex items-center px-4 py-2 space-x-2 font-medium text-white transition-all duration-200 bg-blue-600 shadow-lg sm:px-6 sm:py-3 hover:bg-blue-700 rounded-xl hover:shadow-xl">
              <UserPlusIcon class="w-4 h-4 sm:w-5 sm:h-5" />
              <span class="text-sm sm:text-base">Add Customer</span>
            </button>
            <button @click="showCreateLeadModal = true"
                    class="inline-flex items-center px-4 py-2 space-x-2 font-medium text-white transition-all duration-200 shadow-lg sm:px-6 sm:py-3 bg-emerald-600 hover:bg-emerald-700 rounded-xl hover:shadow-xl">
              <PlusIcon class="w-4 h-4 sm:w-5 sm:h-5" />
              <span class="text-sm sm:text-base">Add Lead</span>
            </button>
          </div>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="pending" class="flex items-center justify-center h-64">
        <div class="w-16 h-16 border-4 border-blue-200 rounded-full animate-spin border-t-blue-600"></div>
      </div>

      <!-- Main Content -->
      <div v-else>
        <!-- Enhanced Stats Grid with Better Visual Design -->
        <div class="grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-4 sm:gap-6">
          <!-- Total Customers Card -->
          <div class="overflow-hidden transition-all duration-200 bg-white border shadow-lg dark:bg-slate-800 rounded-2xl hover:shadow-xl border-slate-200 dark:border-slate-700">
            <div class="p-4 sm:p-6">
              <div class="flex items-center justify-between">
                <div class="flex-1">
                  <p class="text-xs font-medium tracking-wide uppercase sm:text-sm text-slate-600 dark:text-slate-400">Total Customers</p>
                  <p class="mt-2 text-lg font-bold sm:text-3xl text-slate-900 dark:text-white">{{ analytics?.totalCustomers || 0 }}</p>
                  <div class="flex items-center mt-2 space-x-1">
                    <ArrowTrendingUpIcon class="w-3 h-3 sm:w-4 sm:h-4 text-emerald-500" />
                    <span class="text-xs font-medium sm:text-sm text-emerald-600">+8.2%</span>
                    <span class="text-xs sm:text-sm text-slate-500">vs last month</span>
                  </div>
                </div>
                <div class="p-3 bg-gradient-to-br from-blue-500 to-blue-600 sm:p-4 rounded-xl">
                  <UsersIcon class="w-6 h-6 text-white sm:w-8 sm:h-8" />
                </div>
              </div>
            </div>
            <div class="px-4 py-2 bg-gradient-to-r from-blue-50 to-blue-100 dark:from-blue-900/20 dark:to-blue-800/20 sm:px-6 sm:py-3">
              <p class="text-xs font-medium text-blue-700 dark:text-blue-300">Active customer base</p>
            </div>
          </div>

          <!-- Active Leads Card -->
          <div class="overflow-hidden transition-all duration-200 bg-white border shadow-lg dark:bg-slate-800 rounded-2xl hover:shadow-xl border-slate-200 dark:border-slate-700">
            <div class="p-4 sm:p-6">
              <div class="flex items-center justify-between">
                <div class="flex-1">
                  <p class="text-xs font-medium tracking-wide uppercase sm:text-sm text-slate-600 dark:text-slate-400">Active Leads</p>
                  <p class="mt-2 text-lg font-bold sm:text-3xl text-slate-900 dark:text-white">{{ analytics?.activeLeads || 0 }}</p>
                  <div class="flex items-center mt-2 space-x-1">
                    <ArrowTrendingUpIcon class="w-3 h-3 sm:w-4 sm:h-4 text-emerald-500" />
                    <span class="text-xs font-medium sm:text-sm text-emerald-600">{{ analytics?.activeLeads || 0 }} new</span>
                    <span class="text-xs sm:text-sm text-slate-500">this week</span>
                  </div>
                </div>
                <div class="p-3 bg-gradient-to-br from-emerald-500 to-emerald-600 sm:p-4 rounded-xl">
                  <ChartBarIcon class="w-6 h-6 text-white sm:w-8 sm:h-8" />
                </div>
              </div>
            </div>
            <div class="px-4 py-2 bg-gradient-to-r from-emerald-50 to-emerald-100 dark:from-emerald-900/20 dark:to-emerald-800/20 sm:px-6 sm:py-3">
              <p class="text-xs font-medium text-emerald-700 dark:text-emerald-300">Potential opportunities</p>
            </div>
          </div>

          <!-- Conversion Rate Card -->
          <div class="overflow-hidden transition-all duration-200 bg-white border shadow-lg dark:bg-slate-800 rounded-2xl hover:shadow-xl border-slate-200 dark:border-slate-700">
            <div class="p-4 sm:p-6">
              <div class="flex items-center justify-between">
                <div class="flex-1">
                  <p class="text-xs font-medium tracking-wide uppercase sm:text-sm text-slate-600 dark:text-slate-400">Conversion Rate</p>
                  <p class="mt-2 text-lg font-bold sm:text-3xl text-slate-900 dark:text-white">{{ analytics?.conversionRate || 0 }}%</p>
                  <div class="flex items-center mt-2 space-x-1">
                    <ArrowTrendingUpIcon class="w-3 h-3 sm:w-4 sm:h-4 text-emerald-500" />
                    <span class="text-xs font-medium sm:text-sm text-emerald-600">+12.5%</span>
                    <span class="text-xs sm:text-sm text-slate-500">improvement</span>
                  </div>
                </div>
                <div class="p-3 bg-gradient-to-br from-purple-500 to-purple-600 sm:p-4 rounded-xl">
                  <ArrowTrendingUpIcon class="w-6 h-6 text-white sm:w-8 sm:h-8" />
                </div>
              </div>
            </div>
            <div class="px-4 py-2 bg-gradient-to-r from-purple-50 to-purple-100 dark:from-purple-900/20 dark:to-purple-800/20 sm:px-6 sm:py-3">
              <p class="text-xs font-medium text-purple-700 dark:text-purple-300">Lead to customer rate</p>
            </div>
          </div>

          <!-- Pipeline Value Card -->
          <div class="overflow-hidden transition-all duration-200 bg-white border shadow-lg dark:bg-slate-800 rounded-2xl hover:shadow-xl border-slate-200 dark:border-slate-700">
            <div class="p-4 sm:p-6">
              <div class="flex items-center justify-between">
                <div class="flex-1">
                  <p class="text-xs font-medium tracking-wide uppercase sm:text-sm text-slate-600 dark:text-slate-400">Pipeline Value</p>
                  <p class="mt-2 text-lg font-bold sm:text-3xl text-slate-900 dark:text-white">R{{ formatCurrency(analytics?.pipelineValue || 0) }}</p>
                  <div class="flex items-center mt-2 space-x-1">
                    <ArrowTrendingUpIcon class="w-3 h-3 sm:w-4 sm:h-4 text-emerald-500" />
                    <span class="text-xs font-medium sm:text-sm text-emerald-600">+15.8%</span>
                    <span class="text-xs sm:text-sm text-slate-500">this quarter</span>
                  </div>
                </div>
                <div class="p-3 bg-gradient-to-br from-amber-500 to-amber-600 sm:p-4 rounded-xl">
                  <CurrencyDollarIcon class="w-6 h-6 text-white sm:w-8 sm:h-8" />
                </div>
              </div>
            </div>
            <div class="px-4 py-2 bg-gradient-to-r from-amber-50 to-amber-100 dark:from-amber-900/20 dark:to-amber-800/20 sm:px-6 sm:py-3">
              <p class="text-xs font-medium text-amber-700 dark:text-amber-300">Total opportunity value</p>
            </div>
          </div>
        </div>

        <!-- Main Content Section with Better Layout -->
        <div class="grid grid-cols-1 gap-4 lg:grid-cols-3 sm:gap-6">
          <!-- Recent Activity Section - Enhanced Design -->
          <div>
            <div class="bg-white border shadow-sm dark:bg-slate-800 rounded-xl border-slate-200 dark:border-slate-700">
              <div class="p-4 border-b sm:p-6 border-slate-200 dark:border-slate-700">
                <h3 class="text-base font-semibold sm:text-xl text-slate-900 dark:text-white">Recent Activity</h3>
                <p class="mt-1 text-xs sm:text-sm text-slate-600 dark:text-slate-400">Latest customer interactions</p>
              </div>
              <div class="p-4 sm:p-6">
                <div class="space-y-3 sm:space-y-4">
                  <!-- Static content for demo -->
                  <div class="flex items-start space-x-3 sm:space-x-4">
                    <div class="flex items-center justify-center w-8 h-8 bg-blue-500 rounded-full sm:w-10 sm:h-10">
                      <PhoneIcon class="w-4 h-4 text-white sm:w-5 sm:h-5" />
                    </div>
                    <div class="flex-1 min-w-0">
                      <h4 class="text-sm font-medium text-slate-900 dark:text-white">New Customer Added</h4>
                      <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">TechCorp Ltd was added to customer database</p>
                      <p class="mt-1 text-xs text-slate-500 dark:text-slate-400">2 hours ago</p>
                    </div>
                  </div>
                  
                  <div class="flex items-start space-x-3 sm:space-x-4">
                    <div class="flex items-center justify-center w-8 h-8 rounded-full sm:w-10 sm:h-10 bg-emerald-500">
                      <UserPlusIcon class="w-4 h-4 text-white sm:w-5 sm:h-5" />
                    </div>
                    <div class="flex-1 min-w-0">
                      <h4 class="text-sm font-medium text-slate-900 dark:text-white">Lead Converted</h4>
                      <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Small Business Solutions converted to customer</p>
                      <p class="mt-1 text-xs text-slate-500 dark:text-slate-400">4 hours ago</p>
                    </div>
                  </div>

                  <div class="flex items-start space-x-3 sm:space-x-4">
                    <div class="flex items-center justify-center w-8 h-8 bg-purple-500 rounded-full sm:w-10 sm:h-10">
                      <CurrencyDollarIcon class="w-4 h-4 text-white sm:w-5 sm:h-5" />
                    </div>
                    <div class="flex-1 min-w-0">
                      <h4 class="text-sm font-medium text-slate-900 dark:text-white">Opportunity Created</h4>
                      <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">R50,000 software upgrade opportunity</p>
                      <p class="mt-1 text-xs text-slate-500 dark:text-slate-400">6 hours ago</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Collaborative Network Section - Redesigned with Better Spacing -->
          <div>
            <div class="bg-white border shadow-sm dark:bg-slate-800 rounded-xl border-slate-200 dark:border-slate-700">
              <div class="p-4 border-b sm:p-6 border-slate-200 dark:border-slate-700">
                <h3 class="text-base font-semibold sm:text-xl text-slate-900 dark:text-white">Network Collaboration</h3>
                <p class="mt-1 text-xs sm:text-sm text-slate-600 dark:text-slate-400">Connect with other TOSS businesses</p>
              </div>
              <div class="p-4 sm:p-6">
                <div class="space-y-3 sm:space-y-4">
                  <!-- Customer Referrals -->
                  <div class="cursor-pointer group">
                    <div class="flex items-center justify-between p-3 transition-colors border border-green-200 rounded-lg sm:p-4 bg-green-50 dark:bg-green-900/20 sm:rounded-xl dark:border-green-800 hover:bg-green-100 dark:hover:bg-green-900/30">
                      <div class="flex items-center space-x-3 sm:space-x-4">
                        <div class="flex items-center justify-center w-10 h-10 bg-green-500 rounded-lg sm:w-12 sm:h-12 sm:rounded-xl">
                          <UserGroupIcon class="w-5 h-5 text-white sm:w-6 sm:h-6" />
                        </div>
                        <div>
                          <h4 class="text-sm font-medium sm:text-base text-slate-900 dark:text-white">Customer Referrals</h4>
                          <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Share customers within network</p>
                        </div>
                      </div>
                      <ChevronRightIcon class="w-4 h-4 text-green-600 transition-transform sm:w-5 sm:h-5 dark:text-green-400 group-hover:translate-x-1" />
                    </div>
                  </div>
                  
                  <!-- Shared Analytics -->
                  <div class="cursor-pointer group">
                    <div class="flex items-center justify-between p-3 transition-colors border border-blue-200 rounded-lg sm:p-4 bg-blue-50 dark:bg-blue-900/20 sm:rounded-xl dark:border-blue-800 hover:bg-blue-100 dark:hover:bg-blue-900/30">
                      <div class="flex items-center space-x-3 sm:space-x-4">
                        <div class="flex items-center justify-center w-10 h-10 bg-blue-500 rounded-lg sm:w-12 sm:h-12 sm:rounded-xl">
                          <ChartBarIcon class="w-5 h-5 text-white sm:w-6 sm:h-6" />
                        </div>
                        <div>
                          <h4 class="text-sm font-medium sm:text-base text-slate-900 dark:text-white">Shared Analytics</h4>
                          <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Compare with network benchmarks</p>
                        </div>
                      </div>
                      <ChevronRightIcon class="w-4 h-4 text-blue-600 transition-transform sm:w-5 sm:h-5 dark:text-blue-400 group-hover:translate-x-1" />
                    </div>
                  </div>

                  <!-- Group Credit Sales -->
                  <div class="cursor-pointer group">
                    <div class="flex items-center justify-between p-3 transition-colors border border-purple-200 rounded-lg sm:p-4 bg-purple-50 dark:bg-purple-900/20 sm:rounded-xl dark:border-purple-800 hover:bg-purple-100 dark:hover:bg-purple-900/30">
                      <div class="flex items-center space-x-3 sm:space-x-4">
                        <div class="flex items-center justify-center w-10 h-10 bg-purple-500 rounded-lg sm:w-12 sm:h-12 sm:rounded-xl">
                          <CurrencyDollarIcon class="w-5 h-5 text-white sm:w-6 sm:h-6" />
                        </div>
                        <div>
                          <h4 class="text-sm font-medium sm:text-base text-slate-900 dark:text-white">Group Credit Sales</h4>
                          <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Pooled credit for community</p>
                        </div>
                      </div>
                      <ChevronRightIcon class="w-4 h-4 text-purple-600 transition-transform sm:w-5 sm:h-5 dark:text-purple-400 group-hover:translate-x-1" />
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Quick Actions Section - Completely Redesigned -->
          <div>
            <div class="bg-white border shadow-sm dark:bg-slate-800 rounded-xl border-slate-200 dark:border-slate-700">
              <div class="p-4 border-b sm:p-6 border-slate-200 dark:border-slate-700">
                <h3 class="text-base font-semibold sm:text-xl text-slate-900 dark:text-white">Quick Actions</h3>
                <p class="mt-1 text-xs sm:text-sm text-slate-600 dark:text-slate-400">Frequently used CRM actions</p>
              </div>
              <div class="p-4 sm:p-6">
                <div class="space-y-2 sm:space-y-3">
                  <!-- Add Customer -->
                  <button @click="showCreateCustomerModal = true" 
                          class="w-full p-3 text-left transition-all duration-200 border rounded-lg group border-slate-200 dark:border-slate-600 hover:border-blue-300 dark:hover:border-blue-500 hover:bg-blue-50 dark:hover:bg-blue-900/20">
                    <div class="flex items-center space-x-3 sm:space-x-4">
                      <div class="flex items-center justify-center w-10 h-10 transition-colors bg-blue-100 rounded-lg sm:w-12 sm:h-12 dark:bg-blue-900/50 sm:rounded-xl group-hover:bg-blue-200 dark:group-hover:bg-blue-800/50">
                        <UserPlusIcon class="w-5 h-5 text-blue-600 sm:w-6 sm:h-6 dark:text-blue-400" />
                      </div>
                      <div class="flex-1">
                        <h4 class="text-sm font-medium sm:text-base text-slate-900 dark:text-white">Add Customer</h4>
                        <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Create new customer record</p>
                      </div>
                      <ChevronRightIcon class="w-4 h-4 transition-all sm:w-5 sm:h-5 text-slate-400 group-hover:text-blue-600 group-hover:translate-x-1" />
                    </div>
                  </button>

                  <!-- Add Lead -->
                  <button @click="showCreateLeadModal = true" 
                          class="w-full p-3 text-left transition-all duration-200 border rounded-lg group border-slate-200 dark:border-slate-600 hover:border-emerald-300 dark:hover:border-emerald-500 hover:bg-emerald-50 dark:hover:bg-emerald-900/20">
                    <div class="flex items-center space-x-3 sm:space-x-4">
                      <div class="flex items-center justify-center w-10 h-10 transition-colors rounded-lg sm:w-12 sm:h-12 bg-emerald-100 dark:bg-emerald-900/50 sm:rounded-xl group-hover:bg-emerald-200 dark:group-hover:bg-emerald-800/50">
                        <PlusIcon class="w-5 h-5 sm:w-6 sm:h-6 text-emerald-600 dark:text-emerald-400" />
                      </div>
                      <div class="flex-1">
                        <h4 class="text-sm font-medium sm:text-base text-slate-900 dark:text-white">Add Lead</h4>
                        <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Create new lead opportunity</p>
                      </div>
                      <ChevronRightIcon class="w-4 h-4 transition-all sm:w-5 sm:h-5 text-slate-400 group-hover:text-emerald-600 group-hover:translate-x-1" />
                    </div>
                  </button>

                  <!-- View All Customers -->
                  <NuxtLink to="/crm/customers" 
                            class="block w-full p-3 text-left transition-all duration-200 border rounded-lg group border-slate-200 dark:border-slate-600 hover:border-purple-300 dark:hover:border-purple-500 hover:bg-purple-50 dark:hover:bg-purple-900/20">
                    <div class="flex items-center space-x-3 sm:space-x-4">
                      <div class="flex items-center justify-center w-10 h-10 transition-colors bg-purple-100 rounded-lg sm:w-12 sm:h-12 dark:bg-purple-900/50 sm:rounded-xl group-hover:bg-purple-200 dark:group-hover:bg-purple-800/50">
                        <UsersIcon class="w-5 h-5 text-purple-600 sm:w-6 sm:h-6 dark:text-purple-400" />
                      </div>
                      <div class="flex-1">
                        <h4 class="text-sm font-medium sm:text-base text-slate-900 dark:text-white">View All Customers</h4>
                        <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Browse customer database</p>
                      </div>
                      <ChevronRightIcon class="w-4 h-4 transition-all sm:w-5 sm:h-5 text-slate-400 group-hover:text-purple-600 group-hover:translate-x-1" />
                    </div>
                  </NuxtLink>

                  <!-- Manage Leads -->
                  <NuxtLink to="/crm/leads" 
                            class="block w-full p-3 text-left transition-all duration-200 border rounded-lg group border-slate-200 dark:border-slate-600 hover:border-amber-300 dark:hover:border-amber-500 hover:bg-amber-50 dark:hover:bg-amber-900/20">
                    <div class="flex items-center space-x-3 sm:space-x-4">
                      <div class="flex items-center justify-center w-10 h-10 transition-colors rounded-lg sm:w-12 sm:h-12 bg-amber-100 dark:bg-amber-900/50 sm:rounded-xl group-hover:bg-amber-200 dark:group-hover:bg-amber-800/50">
                        <UserPlusIcon class="w-5 h-5 sm:w-6 sm:h-6 text-amber-600 dark:text-amber-400" />
                      </div>
                      <div class="flex-1">
                        <h4 class="text-sm font-medium sm:text-base text-slate-900 dark:text-white">Manage Leads</h4>
                        <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Track potential customers</p>
                      </div>
                      <ChevronRightIcon class="w-4 h-4 transition-all sm:w-5 sm:h-5 text-slate-400 group-hover:text-amber-600 group-hover:translate-x-1" />
                    </div>
                  </NuxtLink>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- AI Co-Pilot Insights Section -->
        <div class="p-4 border border-blue-200 bg-gradient-to-r from-blue-50 to-purple-50 dark:from-blue-900/20 dark:to-purple-900/20 sm:p-6 lg:p-8 rounded-xl sm:rounded-2xl dark:border-blue-800">
          <div class="flex items-center mb-4 sm:mb-6">
            <div class="flex items-center justify-center w-10 h-10 mr-3 bg-blue-100 rounded-lg sm:w-12 sm:h-12 sm:mr-4 sm:rounded-xl dark:bg-blue-900">
              <SparklesIcon class="w-6 h-6 text-blue-600 sm:w-8 sm:h-8 dark:text-blue-400" />
            </div>
            <div>
              <h3 class="text-base font-semibold sm:text-xl text-slate-900 dark:text-white">AI Co-Pilot Insights</h3>
              <p class="mt-1 text-xs sm:text-sm text-slate-600 dark:text-slate-400">Intelligent recommendations for your business</p>
            </div>
          </div>
          <div class="grid grid-cols-1 gap-4 md:grid-cols-3 sm:gap-6">
            <div class="p-4 bg-white rounded-lg shadow-sm dark:bg-slate-800 sm:p-6 sm:rounded-xl">
              <div class="flex items-start space-x-3 sm:space-x-4">
                <BoltIcon class="flex-shrink-0 w-5 h-5 mt-1 text-yellow-500 sm:w-6 sm:h-6" />
                <div>
                  <h4 class="text-sm font-medium sm:text-base text-slate-900 dark:text-white">Customer Engagement Alert</h4>
                  <p class="mt-2 text-xs sm:text-sm text-slate-600 dark:text-slate-400">3 customers haven't been contacted in 30+ days. Consider follow-up calls to maintain engagement.</p>
                </div>
              </div>
            </div>
            <div class="p-4 bg-white rounded-lg shadow-sm dark:bg-slate-800 sm:p-6 sm:rounded-xl">
              <div class="flex items-start space-x-3 sm:space-x-4">
                <ArrowTrendingUpIcon class="flex-shrink-0 w-5 h-5 mt-1 text-green-500 sm:w-6 sm:h-6" />
                <div>
                  <h4 class="text-sm font-medium sm:text-base text-slate-900 dark:text-white">Upsell Opportunity</h4>
                  <p class="mt-2 text-xs sm:text-sm text-slate-600 dark:text-slate-400">TechCorp Ltd shows 85% likelihood for premium service upgrade based on usage patterns.</p>
                </div>
              </div>
            </div>
            <div class="p-4 bg-white rounded-lg shadow-sm dark:bg-slate-800 sm:p-6 sm:rounded-xl">
              <div class="flex items-start space-x-3 sm:space-x-4">
                <ChartBarIcon class="flex-shrink-0 w-5 h-5 mt-1 text-blue-500 sm:w-6 sm:h-6" />
                <div>
                  <h4 class="text-sm font-medium sm:text-base text-slate-900 dark:text-white">Performance Insight</h4>
                  <p class="mt-2 text-xs sm:text-sm text-slate-600 dark:text-slate-400">Email campaigns show 23% higher conversion on Wednesdays. Schedule accordingly.</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Customer Modal -->
    <div v-if="showCreateCustomerModal" class="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50">
      <div class="w-full max-w-md mx-4 bg-white shadow-xl dark:bg-slate-800 rounded-xl sm:rounded-2xl">
        <div class="p-4 border-b sm:p-6 border-slate-200 dark:border-slate-700">
          <h3 class="text-lg font-semibold sm:text-xl text-slate-900 dark:text-white">Add New Customer</h3>
        </div>
        <div class="p-4 sm:p-6">
          <form @submit.prevent="createCustomer">
            <div class="space-y-3 sm:space-y-4">
              <div>
                <label class="block mb-2 text-sm font-medium text-slate-700 dark:text-slate-300">Company Name</label>
                <input v-model="newCustomer.name" type="text" required 
                       class="w-full px-3 py-2 border rounded-lg sm:px-4 border-slate-300 dark:border-slate-600 focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-slate-700 dark:text-white">
              </div>
              <div>
                <label class="block mb-2 text-sm font-medium text-slate-700 dark:text-slate-300">Email</label>
                <input v-model="newCustomer.email" type="email" required 
                       class="w-full px-3 py-2 border rounded-lg sm:px-4 border-slate-300 dark:border-slate-600 focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-slate-700 dark:text-white">
              </div>
              <div>
                <label class="block mb-2 text-sm font-medium text-slate-700 dark:text-slate-300">Phone</label>
                <input v-model="newCustomer.phone" type="tel" 
                       class="w-full px-3 py-2 border rounded-lg sm:px-4 border-slate-300 dark:border-slate-600 focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-slate-700 dark:text-white">
              </div>
            </div>
            <div class="flex justify-end mt-4 space-x-3 sm:mt-6">
              <button @click="showCreateCustomerModal = false" type="button" 
                      class="px-4 py-2 text-slate-600 dark:text-slate-400 hover:text-slate-800 dark:hover:text-slate-200">
                Cancel
              </button>
              <button type="submit" 
                      class="px-4 py-2 text-white bg-blue-600 rounded-lg sm:px-6 hover:bg-blue-700">
                Create Customer
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Create Lead Modal -->
    <div v-if="showCreateLeadModal" class="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50">
      <div class="w-full max-w-md mx-4 bg-white shadow-xl dark:bg-slate-800 rounded-xl sm:rounded-2xl">
        <div class="p-4 border-b sm:p-6 border-slate-200 dark:border-slate-700">
          <h3 class="text-lg font-semibold sm:text-xl text-slate-900 dark:text-white">Add New Lead</h3>
        </div>
        <div class="p-4 sm:p-6">
          <form @submit.prevent="createLead">
            <div class="space-y-3 sm:space-y-4">
              <div>
                <label class="block mb-2 text-sm font-medium text-slate-700 dark:text-slate-300">Company Name</label>
                <input v-model="newLead.name" type="text" required 
                       class="w-full px-3 py-2 border rounded-lg sm:px-4 border-slate-300 dark:border-slate-600 focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-slate-700 dark:text-white">
              </div>
              <div>
                <label class="block mb-2 text-sm font-medium text-slate-700 dark:text-slate-300">Email</label>
                <input v-model="newLead.email" type="email" required 
                       class="w-full px-3 py-2 border rounded-lg sm:px-4 border-slate-300 dark:border-slate-600 focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-slate-700 dark:text-white">
              </div>
              <div>
                <label class="block mb-2 text-sm font-medium text-slate-700 dark:text-slate-300">Status</label>
                <select v-model="newLead.status" 
                        class="w-full px-3 py-2 border rounded-lg sm:px-4 border-slate-300 dark:border-slate-600 focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-slate-700 dark:text-white">
                  <option value="new">New</option>
                  <option value="contacted">Contacted</option>
                  <option value="qualified">Qualified</option>
                  <option value="proposal">Proposal</option>
                </select>
              </div>
            </div>
            <div class="flex justify-end mt-4 space-x-3 sm:mt-6">
              <button @click="showCreateLeadModal = false" type="button" 
                      class="px-4 py-2 text-slate-600 dark:text-slate-400 hover:text-slate-800 dark:hover:text-slate-200">
                Cancel
              </button>
              <button type="submit" 
                      class="px-4 py-2 text-white rounded-lg sm:px-6 bg-emerald-600 hover:bg-emerald-700">
                Create Lead
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { 
  UsersIcon, 
  ChartBarIcon, 
  ArrowTrendingUpIcon, 
  CurrencyDollarIcon,
  UserPlusIcon,
  PlusIcon,
  PhoneIcon,
  UserGroupIcon,
  ChevronRightIcon,
  SparklesIcon,
  BoltIcon
} from '@heroicons/vue/24/outline'

// Page metadata
definePageMeta({
  layout: 'default',
  title: 'CRM Dashboard'
})

// Reactive data
const showCreateCustomerModal = ref(false)
const showCreateLeadModal = ref(false)
const pending = ref(false)

const newCustomer = ref({
  name: '',
  email: '',
  phone: ''
})

const newLead = ref({
  name: '',
  email: '',
  status: 'new'
})

// Mock analytics data
const analytics = ref({
  totalCustomers: 89,
  activeLeads: 23,
  conversionRate: 68.2,
  pipelineValue: 347850
})

// Mock recent activities for Thabo's Spaza Shop
const recentActivities = ref([
  {
    id: 1,
    type: 'customer',
    title: 'New customer registered',
    description: 'Nomsa Community Kitchen joined as regular customer',
    timestamp: new Date(Date.now() - 2 * 60 * 60 * 1000),
    user: 'System'
  },
  {
    id: 2,
    type: 'opportunity',
    title: 'Large order opportunity',
    description: 'Mandla Construction requesting bulk supplies quote',
    timestamp: new Date(Date.now() - 4 * 60 * 60 * 1000),
    user: 'Thabo'
  },
  {
    id: 3,
    type: 'call',
    title: 'Follow-up call completed',
    description: 'Spoke with Grace Catering about weekly delivery schedule',
    timestamp: new Date(Date.now() - 6 * 60 * 60 * 1000),
    user: 'Thabo'
  },
  {
    id: 4,
    type: 'lead',
    title: 'New lead from referral',
    description: 'Beauty salon referred by Lerato Hair Studio',
    timestamp: new Date(Date.now() - 8 * 60 * 60 * 1000),
    user: 'System'
  },
  {
    id: 5,
    type: 'email',
    title: 'Payment reminder sent',
    description: 'Outstanding invoice reminder to Sipho Auto Repair',
    timestamp: new Date(Date.now() - 10 * 60 * 60 * 1000),
    user: 'System'
  }
])

// Computed properties
const formatCurrency = (value) => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: 'ZAR',
    minimumFractionDigits: 0
  }).format(value).replace('ZAR', '').trim()
}

const formatDate = (date) => {
  return new Date(date).toLocaleDateString('en-ZA', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

// Helper functions for activities
const getActivityColor = (type) => {
  const colors = {
    'customer': 'bg-blue-500',
    'lead': 'bg-emerald-500',
    'opportunity': 'bg-purple-500',
    'call': 'bg-blue-500',
    'email': 'bg-indigo-500',
    'meeting': 'bg-amber-500'
  }
  return colors[type] || 'bg-slate-500'
}

const getActivityIcon = (type) => {
  const icons = {
    'customer': UsersIcon,
    'lead': UserPlusIcon,
    'opportunity': CurrencyDollarIcon,
    'call': PhoneIcon,
    'email': 'EnvelopeIcon',
    'meeting': 'CalendarIcon'
  }
  return icons[type] || PhoneIcon
}

// Modal functions
const createCustomer = async () => {
  // Simulate API call
  console.log('Creating customer:', newCustomer.value)
  
  // Reset form and close modal
  newCustomer.value = { name: '', email: '', phone: '' }
  showCreateCustomerModal.value = false
  
  // Show success message (you might want to use a toast notification)
  alert('Customer created successfully!')
}

const createLead = async () => {
  // Simulate API call
  console.log('Creating lead:', newLead.value)
  
  // Reset form and close modal
  newLead.value = { name: '', email: '', status: 'new' }
  showCreateLeadModal.value = false
  
  // Show success message
  alert('Lead created successfully!')
}
</script>
