<template>
  <div>
    <!-- Page Container matching main dashboard -->
    <div class="p-4 sm:p-6 space-y-4 sm:space-y-6 pb-20 lg:pb-6">
      <!-- Page Header -->
      <div class="text-center sm:text-left">
        <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between space-y-4 sm:space-y-0">
          <div>
            <h1 class="text-2xl sm:text-3xl font-bold text-slate-900 dark:text-white">
              CRM Dashboard
            </h1>
            <p class="text-slate-600 dark:text-slate-400 mt-1 text-sm sm:text-base">
              Manage your customer relationships with AI-powered insights
            </p>
          </div>
          <div class="flex flex-col sm:flex-row gap-3">
            <button @click="showCreateCustomerModal = true" 
                    class="inline-flex items-center px-4 py-2 sm:px-6 sm:py-3 bg-blue-600 hover:bg-blue-700 text-white font-medium rounded-xl shadow-lg hover:shadow-xl transition-all duration-200 space-x-2">
              <UserPlusIcon class="w-4 h-4 sm:w-5 sm:h-5" />
              <span class="text-sm sm:text-base">Add Customer</span>
            </button>
            <button @click="showCreateLeadModal = true"
                    class="inline-flex items-center px-4 py-2 sm:px-6 sm:py-3 bg-emerald-600 hover:bg-emerald-700 text-white font-medium rounded-xl shadow-lg hover:shadow-xl transition-all duration-200 space-x-2">
              <PlusIcon class="w-4 h-4 sm:w-5 sm:h-5" />
              <span class="text-sm sm:text-base">Add Lead</span>
            </button>
          </div>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="pending" class="flex justify-center items-center h-64">
        <div class="animate-spin rounded-full h-16 w-16 border-4 border-blue-200 border-t-blue-600"></div>
      </div>

      <!-- Main Content -->
      <div v-else>
        <!-- Enhanced Stats Grid with Better Visual Design -->
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-6">
          <!-- Total Customers Card -->
          <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg hover:shadow-xl transition-all duration-200 border border-slate-200 dark:border-slate-700 overflow-hidden">
            <div class="p-4 sm:p-6">
              <div class="flex items-center justify-between">
                <div class="flex-1">
                  <p class="text-xs sm:text-sm font-medium text-slate-600 dark:text-slate-400 uppercase tracking-wide">Total Customers</p>
                  <p class="text-lg sm:text-3xl font-bold text-slate-900 dark:text-white mt-2">{{ analytics?.totalCustomers || 0 }}</p>
                  <div class="flex items-center mt-2 space-x-1">
                    <ArrowTrendingUpIcon class="w-3 h-3 sm:w-4 sm:h-4 text-emerald-500" />
                    <span class="text-xs sm:text-sm font-medium text-emerald-600">+8.2%</span>
                    <span class="text-xs sm:text-sm text-slate-500">vs last month</span>
                  </div>
                </div>
                <div class="bg-gradient-to-br from-blue-500 to-blue-600 p-3 sm:p-4 rounded-xl">
                  <UsersIcon class="w-6 h-6 sm:w-8 sm:h-8 text-white" />
                </div>
              </div>
            </div>
            <div class="bg-gradient-to-r from-blue-50 to-blue-100 dark:from-blue-900/20 dark:to-blue-800/20 px-4 sm:px-6 py-2 sm:py-3">
              <p class="text-xs font-medium text-blue-700 dark:text-blue-300">Active customer base</p>
            </div>
          </div>

          <!-- Active Leads Card -->
          <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg hover:shadow-xl transition-all duration-200 border border-slate-200 dark:border-slate-700 overflow-hidden">
            <div class="p-4 sm:p-6">
              <div class="flex items-center justify-between">
                <div class="flex-1">
                  <p class="text-xs sm:text-sm font-medium text-slate-600 dark:text-slate-400 uppercase tracking-wide">Active Leads</p>
                  <p class="text-lg sm:text-3xl font-bold text-slate-900 dark:text-white mt-2">{{ analytics?.activeLeads || 0 }}</p>
                  <div class="flex items-center mt-2 space-x-1">
                    <ArrowTrendingUpIcon class="w-3 h-3 sm:w-4 sm:h-4 text-emerald-500" />
                    <span class="text-xs sm:text-sm font-medium text-emerald-600">{{ analytics?.activeLeads || 0 }} new</span>
                    <span class="text-xs sm:text-sm text-slate-500">this week</span>
                  </div>
                </div>
                <div class="bg-gradient-to-br from-emerald-500 to-emerald-600 p-3 sm:p-4 rounded-xl">
                  <ChartBarIcon class="w-6 h-6 sm:w-8 sm:h-8 text-white" />
                </div>
              </div>
            </div>
            <div class="bg-gradient-to-r from-emerald-50 to-emerald-100 dark:from-emerald-900/20 dark:to-emerald-800/20 px-4 sm:px-6 py-2 sm:py-3">
              <p class="text-xs font-medium text-emerald-700 dark:text-emerald-300">Potential opportunities</p>
            </div>
          </div>

          <!-- Conversion Rate Card -->
          <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg hover:shadow-xl transition-all duration-200 border border-slate-200 dark:border-slate-700 overflow-hidden">
            <div class="p-4 sm:p-6">
              <div class="flex items-center justify-between">
                <div class="flex-1">
                  <p class="text-xs sm:text-sm font-medium text-slate-600 dark:text-slate-400 uppercase tracking-wide">Conversion Rate</p>
                  <p class="text-lg sm:text-3xl font-bold text-slate-900 dark:text-white mt-2">{{ analytics?.conversionRate || 0 }}%</p>
                  <div class="flex items-center mt-2 space-x-1">
                    <ArrowTrendingUpIcon class="w-3 h-3 sm:w-4 sm:h-4 text-emerald-500" />
                    <span class="text-xs sm:text-sm font-medium text-emerald-600">+12.5%</span>
                    <span class="text-xs sm:text-sm text-slate-500">improvement</span>
                  </div>
                </div>
                <div class="bg-gradient-to-br from-purple-500 to-purple-600 p-3 sm:p-4 rounded-xl">
                  <ArrowTrendingUpIcon class="w-6 h-6 sm:w-8 sm:h-8 text-white" />
                </div>
              </div>
            </div>
            <div class="bg-gradient-to-r from-purple-50 to-purple-100 dark:from-purple-900/20 dark:to-purple-800/20 px-4 sm:px-6 py-2 sm:py-3">
              <p class="text-xs font-medium text-purple-700 dark:text-purple-300">Lead to customer rate</p>
            </div>
          </div>

          <!-- Pipeline Value Card -->
          <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg hover:shadow-xl transition-all duration-200 border border-slate-200 dark:border-slate-700 overflow-hidden">
            <div class="p-4 sm:p-6">
              <div class="flex items-center justify-between">
                <div class="flex-1">
                  <p class="text-xs sm:text-sm font-medium text-slate-600 dark:text-slate-400 uppercase tracking-wide">Pipeline Value</p>
                  <p class="text-lg sm:text-3xl font-bold text-slate-900 dark:text-white mt-2">R{{ formatCurrency(analytics?.pipelineValue || 0) }}</p>
                  <div class="flex items-center mt-2 space-x-1">
                    <ArrowTrendingUpIcon class="w-3 h-3 sm:w-4 sm:h-4 text-emerald-500" />
                    <span class="text-xs sm:text-sm font-medium text-emerald-600">+15.8%</span>
                    <span class="text-xs sm:text-sm text-slate-500">this quarter</span>
                  </div>
                </div>
                <div class="bg-gradient-to-br from-amber-500 to-amber-600 p-3 sm:p-4 rounded-xl">
                  <CurrencyDollarIcon class="w-6 h-6 sm:w-8 sm:h-8 text-white" />
                </div>
              </div>
            </div>
            <div class="bg-gradient-to-r from-amber-50 to-amber-100 dark:from-amber-900/20 dark:to-amber-800/20 px-4 sm:px-6 py-2 sm:py-3">
              <p class="text-xs font-medium text-amber-700 dark:text-amber-300">Total opportunity value</p>
            </div>
          </div>
        </div>

        <!-- Main Content Section with Better Layout -->
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-4 sm:gap-6">
          <!-- Recent Activity Section - Enhanced Design -->
          <div>
            <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
              <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
                <h3 class="text-base sm:text-xl font-semibold text-slate-900 dark:text-white">Recent Activity</h3>
                <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mt-1">Latest customer interactions</p>
              </div>
              <div class="p-4 sm:p-6">
                <div class="space-y-3 sm:space-y-4">
                  <!-- Static content for demo -->
                  <div class="flex items-start space-x-3 sm:space-x-4">
                    <div class="w-8 h-8 sm:w-10 sm:h-10 rounded-full flex items-center justify-center bg-blue-500">
                      <PhoneIcon class="w-4 h-4 sm:w-5 sm:h-5 text-white" />
                    </div>
                    <div class="flex-1 min-w-0">
                      <h4 class="text-sm font-medium text-slate-900 dark:text-white">New Customer Added</h4>
                      <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">TechCorp Ltd was added to customer database</p>
                      <p class="text-xs text-slate-500 dark:text-slate-400 mt-1">2 hours ago</p>
                    </div>
                  </div>
                  
                  <div class="flex items-start space-x-3 sm:space-x-4">
                    <div class="w-8 h-8 sm:w-10 sm:h-10 rounded-full flex items-center justify-center bg-emerald-500">
                      <UserPlusIcon class="w-4 h-4 sm:w-5 sm:h-5 text-white" />
                    </div>
                    <div class="flex-1 min-w-0">
                      <h4 class="text-sm font-medium text-slate-900 dark:text-white">Lead Converted</h4>
                      <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Small Business Solutions converted to customer</p>
                      <p class="text-xs text-slate-500 dark:text-slate-400 mt-1">4 hours ago</p>
                    </div>
                  </div>

                  <div class="flex items-start space-x-3 sm:space-x-4">
                    <div class="w-8 h-8 sm:w-10 sm:h-10 rounded-full flex items-center justify-center bg-purple-500">
                      <CurrencyDollarIcon class="w-4 h-4 sm:w-5 sm:h-5 text-white" />
                    </div>
                    <div class="flex-1 min-w-0">
                      <h4 class="text-sm font-medium text-slate-900 dark:text-white">Opportunity Created</h4>
                      <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">R50,000 software upgrade opportunity</p>
                      <p class="text-xs text-slate-500 dark:text-slate-400 mt-1">6 hours ago</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Collaborative Network Section - Redesigned with Better Spacing -->
          <div>
            <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
              <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
                <h3 class="text-base sm:text-xl font-semibold text-slate-900 dark:text-white">Network Collaboration</h3>
                <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mt-1">Connect with other TOSS businesses</p>
              </div>
              <div class="p-4 sm:p-6">
                <div class="space-y-3 sm:space-y-4">
                  <!-- Customer Referrals -->
                  <div class="group cursor-pointer">
                    <div class="flex items-center justify-between p-3 sm:p-4 bg-green-50 dark:bg-green-900/20 rounded-lg sm:rounded-xl border border-green-200 dark:border-green-800 hover:bg-green-100 dark:hover:bg-green-900/30 transition-colors">
                      <div class="flex items-center space-x-3 sm:space-x-4">
                        <div class="w-10 h-10 sm:w-12 sm:h-12 bg-green-500 rounded-lg sm:rounded-xl flex items-center justify-center">
                          <UserGroupIcon class="w-5 h-5 sm:w-6 sm:h-6 text-white" />
                        </div>
                        <div>
                          <h4 class="text-sm sm:text-base font-medium text-slate-900 dark:text-white">Customer Referrals</h4>
                          <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Share customers within network</p>
                        </div>
                      </div>
                      <ChevronRightIcon class="w-4 h-4 sm:w-5 sm:h-5 text-green-600 dark:text-green-400 group-hover:translate-x-1 transition-transform" />
                    </div>
                  </div>
                  
                  <!-- Shared Analytics -->
                  <div class="group cursor-pointer">
                    <div class="flex items-center justify-between p-3 sm:p-4 bg-blue-50 dark:bg-blue-900/20 rounded-lg sm:rounded-xl border border-blue-200 dark:border-blue-800 hover:bg-blue-100 dark:hover:bg-blue-900/30 transition-colors">
                      <div class="flex items-center space-x-3 sm:space-x-4">
                        <div class="w-10 h-10 sm:w-12 sm:h-12 bg-blue-500 rounded-lg sm:rounded-xl flex items-center justify-center">
                          <ChartBarIcon class="w-5 h-5 sm:w-6 sm:h-6 text-white" />
                        </div>
                        <div>
                          <h4 class="text-sm sm:text-base font-medium text-slate-900 dark:text-white">Shared Analytics</h4>
                          <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Compare with network benchmarks</p>
                        </div>
                      </div>
                      <ChevronRightIcon class="w-4 h-4 sm:w-5 sm:h-5 text-blue-600 dark:text-blue-400 group-hover:translate-x-1 transition-transform" />
                    </div>
                  </div>

                  <!-- Group Credit Sales -->
                  <div class="group cursor-pointer">
                    <div class="flex items-center justify-between p-3 sm:p-4 bg-purple-50 dark:bg-purple-900/20 rounded-lg sm:rounded-xl border border-purple-200 dark:border-purple-800 hover:bg-purple-100 dark:hover:bg-purple-900/30 transition-colors">
                      <div class="flex items-center space-x-3 sm:space-x-4">
                        <div class="w-10 h-10 sm:w-12 sm:h-12 bg-purple-500 rounded-lg sm:rounded-xl flex items-center justify-center">
                          <CurrencyDollarIcon class="w-5 h-5 sm:w-6 sm:h-6 text-white" />
                        </div>
                        <div>
                          <h4 class="text-sm sm:text-base font-medium text-slate-900 dark:text-white">Group Credit Sales</h4>
                          <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Pooled credit for community</p>
                        </div>
                      </div>
                      <ChevronRightIcon class="w-4 h-4 sm:w-5 sm:h-5 text-purple-600 dark:text-purple-400 group-hover:translate-x-1 transition-transform" />
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Quick Actions Section - Completely Redesigned -->
          <div>
            <div class="bg-white dark:bg-slate-800 rounded-xl shadow-sm border border-slate-200 dark:border-slate-700">
              <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
                <h3 class="text-base sm:text-xl font-semibold text-slate-900 dark:text-white">Quick Actions</h3>
                <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mt-1">Frequently used CRM actions</p>
              </div>
              <div class="p-4 sm:p-6">
                <div class="space-y-2 sm:space-y-3">
                  <!-- Add Customer -->
                  <button @click="showCreateCustomerModal = true" 
                          class="group w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:border-blue-300 dark:hover:border-blue-500 hover:bg-blue-50 dark:hover:bg-blue-900/20 transition-all duration-200">
                    <div class="flex items-center space-x-3 sm:space-x-4">
                      <div class="w-10 h-10 sm:w-12 sm:h-12 bg-blue-100 dark:bg-blue-900/50 rounded-lg sm:rounded-xl flex items-center justify-center group-hover:bg-blue-200 dark:group-hover:bg-blue-800/50 transition-colors">
                        <UserPlusIcon class="w-5 h-5 sm:w-6 sm:h-6 text-blue-600 dark:text-blue-400" />
                      </div>
                      <div class="flex-1">
                        <h4 class="text-sm sm:text-base font-medium text-slate-900 dark:text-white">Add Customer</h4>
                        <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Create new customer record</p>
                      </div>
                      <ChevronRightIcon class="w-4 h-4 sm:w-5 sm:h-5 text-slate-400 group-hover:text-blue-600 group-hover:translate-x-1 transition-all" />
                    </div>
                  </button>

                  <!-- Add Lead -->
                  <button @click="showCreateLeadModal = true" 
                          class="group w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:border-emerald-300 dark:hover:border-emerald-500 hover:bg-emerald-50 dark:hover:bg-emerald-900/20 transition-all duration-200">
                    <div class="flex items-center space-x-3 sm:space-x-4">
                      <div class="w-10 h-10 sm:w-12 sm:h-12 bg-emerald-100 dark:bg-emerald-900/50 rounded-lg sm:rounded-xl flex items-center justify-center group-hover:bg-emerald-200 dark:group-hover:bg-emerald-800/50 transition-colors">
                        <PlusIcon class="w-5 h-5 sm:w-6 sm:h-6 text-emerald-600 dark:text-emerald-400" />
                      </div>
                      <div class="flex-1">
                        <h4 class="text-sm sm:text-base font-medium text-slate-900 dark:text-white">Add Lead</h4>
                        <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Create new lead opportunity</p>
                      </div>
                      <ChevronRightIcon class="w-4 h-4 sm:w-5 sm:h-5 text-slate-400 group-hover:text-emerald-600 group-hover:translate-x-1 transition-all" />
                    </div>
                  </button>

                  <!-- View All Customers -->
                  <NuxtLink to="/crm/customers" 
                            class="group block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:border-purple-300 dark:hover:border-purple-500 hover:bg-purple-50 dark:hover:bg-purple-900/20 transition-all duration-200">
                    <div class="flex items-center space-x-3 sm:space-x-4">
                      <div class="w-10 h-10 sm:w-12 sm:h-12 bg-purple-100 dark:bg-purple-900/50 rounded-lg sm:rounded-xl flex items-center justify-center group-hover:bg-purple-200 dark:group-hover:bg-purple-800/50 transition-colors">
                        <UsersIcon class="w-5 h-5 sm:w-6 sm:h-6 text-purple-600 dark:text-purple-400" />
                      </div>
                      <div class="flex-1">
                        <h4 class="text-sm sm:text-base font-medium text-slate-900 dark:text-white">View All Customers</h4>
                        <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Browse customer database</p>
                      </div>
                      <ChevronRightIcon class="w-4 h-4 sm:w-5 sm:h-5 text-slate-400 group-hover:text-purple-600 group-hover:translate-x-1 transition-all" />
                    </div>
                  </NuxtLink>

                  <!-- Manage Leads -->
                  <NuxtLink to="/crm/leads" 
                            class="group block w-full text-left p-3 rounded-lg border border-slate-200 dark:border-slate-600 hover:border-amber-300 dark:hover:border-amber-500 hover:bg-amber-50 dark:hover:bg-amber-900/20 transition-all duration-200">
                    <div class="flex items-center space-x-3 sm:space-x-4">
                      <div class="w-10 h-10 sm:w-12 sm:h-12 bg-amber-100 dark:bg-amber-900/50 rounded-lg sm:rounded-xl flex items-center justify-center group-hover:bg-amber-200 dark:group-hover:bg-amber-800/50 transition-colors">
                        <UserPlusIcon class="w-5 h-5 sm:w-6 sm:h-6 text-amber-600 dark:text-amber-400" />
                      </div>
                      <div class="flex-1">
                        <h4 class="text-sm sm:text-base font-medium text-slate-900 dark:text-white">Manage Leads</h4>
                        <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400">Track potential customers</p>
                      </div>
                      <ChevronRightIcon class="w-4 h-4 sm:w-5 sm:h-5 text-slate-400 group-hover:text-amber-600 group-hover:translate-x-1 transition-all" />
                    </div>
                  </NuxtLink>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- AI Co-Pilot Insights Section -->
        <div class="bg-gradient-to-r from-blue-50 to-purple-50 dark:from-blue-900/20 dark:to-purple-900/20 p-4 sm:p-6 lg:p-8 rounded-xl sm:rounded-2xl border border-blue-200 dark:border-blue-800">
          <div class="flex items-center mb-4 sm:mb-6">
            <div class="flex items-center justify-center w-10 h-10 sm:w-12 sm:h-12 mr-3 sm:mr-4 bg-blue-100 rounded-lg sm:rounded-xl dark:bg-blue-900">
              <SparklesIcon class="w-6 h-6 sm:w-8 sm:h-8 text-blue-600 dark:text-blue-400" />
            </div>
            <div>
              <h3 class="text-base sm:text-xl font-semibold text-slate-900 dark:text-white">AI Co-Pilot Insights</h3>
              <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mt-1">Intelligent recommendations for your business</p>
            </div>
          </div>
          <div class="grid grid-cols-1 md:grid-cols-3 gap-4 sm:gap-6">
            <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-lg sm:rounded-xl shadow-sm">
              <div class="flex items-start space-x-3 sm:space-x-4">
                <BoltIcon class="w-5 h-5 sm:w-6 sm:h-6 text-yellow-500 mt-1 flex-shrink-0" />
                <div>
                  <h4 class="text-sm sm:text-base font-medium text-slate-900 dark:text-white">Customer Engagement Alert</h4>
                  <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mt-2">3 customers haven't been contacted in 30+ days. Consider follow-up calls to maintain engagement.</p>
                </div>
              </div>
            </div>
            <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-lg sm:rounded-xl shadow-sm">
              <div class="flex items-start space-x-3 sm:space-x-4">
                <ArrowTrendingUpIcon class="w-5 h-5 sm:w-6 sm:h-6 text-green-500 mt-1 flex-shrink-0" />
                <div>
                  <h4 class="text-sm sm:text-base font-medium text-slate-900 dark:text-white">Upsell Opportunity</h4>
                  <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mt-2">TechCorp Ltd shows 85% likelihood for premium service upgrade based on usage patterns.</p>
                </div>
              </div>
            </div>
            <div class="bg-white dark:bg-slate-800 p-4 sm:p-6 rounded-lg sm:rounded-xl shadow-sm">
              <div class="flex items-start space-x-3 sm:space-x-4">
                <ChartBarIcon class="w-5 h-5 sm:w-6 sm:h-6 text-blue-500 mt-1 flex-shrink-0" />
                <div>
                  <h4 class="text-sm sm:text-base font-medium text-slate-900 dark:text-white">Performance Insight</h4>
                  <p class="text-xs sm:text-sm text-slate-600 dark:text-slate-400 mt-2">Email campaigns show 23% higher conversion on Wednesdays. Schedule accordingly.</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Customer Modal -->
    <div v-if="showCreateCustomerModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-slate-800 rounded-xl sm:rounded-2xl shadow-xl max-w-md w-full mx-4">
        <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-lg sm:text-xl font-semibold text-slate-900 dark:text-white">Add New Customer</h3>
        </div>
        <div class="p-4 sm:p-6">
          <form @submit.prevent="createCustomer">
            <div class="space-y-3 sm:space-y-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Company Name</label>
                <input v-model="newCustomer.name" type="text" required 
                       class="w-full px-3 sm:px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-slate-700 dark:text-white">
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Email</label>
                <input v-model="newCustomer.email" type="email" required 
                       class="w-full px-3 sm:px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-slate-700 dark:text-white">
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Phone</label>
                <input v-model="newCustomer.phone" type="tel" 
                       class="w-full px-3 sm:px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-slate-700 dark:text-white">
              </div>
            </div>
            <div class="flex justify-end space-x-3 mt-4 sm:mt-6">
              <button @click="showCreateCustomerModal = false" type="button" 
                      class="px-4 py-2 text-slate-600 dark:text-slate-400 hover:text-slate-800 dark:hover:text-slate-200">
                Cancel
              </button>
              <button type="submit" 
                      class="px-4 sm:px-6 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg">
                Create Customer
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Create Lead Modal -->
    <div v-if="showCreateLeadModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-slate-800 rounded-xl sm:rounded-2xl shadow-xl max-w-md w-full mx-4">
        <div class="p-4 sm:p-6 border-b border-slate-200 dark:border-slate-700">
          <h3 class="text-lg sm:text-xl font-semibold text-slate-900 dark:text-white">Add New Lead</h3>
        </div>
        <div class="p-4 sm:p-6">
          <form @submit.prevent="createLead">
            <div class="space-y-3 sm:space-y-4">
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Company Name</label>
                <input v-model="newLead.name" type="text" required 
                       class="w-full px-3 sm:px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-slate-700 dark:text-white">
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Email</label>
                <input v-model="newLead.email" type="email" required 
                       class="w-full px-3 sm:px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-slate-700 dark:text-white">
              </div>
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Status</label>
                <select v-model="newLead.status" 
                        class="w-full px-3 sm:px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent dark:bg-slate-700 dark:text-white">
                  <option value="new">New</option>
                  <option value="contacted">Contacted</option>
                  <option value="qualified">Qualified</option>
                  <option value="proposal">Proposal</option>
                </select>
              </div>
            </div>
            <div class="flex justify-end space-x-3 mt-4 sm:mt-6">
              <button @click="showCreateLeadModal = false" type="button" 
                      class="px-4 py-2 text-slate-600 dark:text-slate-400 hover:text-slate-800 dark:hover:text-slate-200">
                Cancel
              </button>
              <button type="submit" 
                      class="px-4 sm:px-6 py-2 bg-emerald-600 hover:bg-emerald-700 text-white rounded-lg">
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
  layout: 'dashboard',
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
  totalCustomers: 342,
  activeLeads: 47,
  conversionRate: 23.5,
  pipelineValue: 125000
})

// Mock recent activities
const recentActivities = ref([])

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
