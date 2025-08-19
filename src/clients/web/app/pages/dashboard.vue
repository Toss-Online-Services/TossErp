<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Page Header -->
    <div class="bg-white shadow">
      <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        <div class="md:flex md:items-center md:justify-between">
          <div class="flex-1 min-w-0">
            <h2 class="text-2xl font-bold leading-7 text-gray-900 sm:text-3xl sm:truncate">
              General Dashboard
            </h2>
            <p class="mt-1 text-sm text-gray-500">
              Real-time business analytics and performance metrics
            </p>
          </div>
          <div class="mt-4 flex md:mt-0 md:ml-4 space-x-3">
            <button @click="exportData" class="inline-flex items-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-orange-500">
              <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 10v6m0 0l-3-3m3 3l3-3m2 8H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
              </svg>
              Export
            </button>
            <button @click="refreshData" class="inline-flex items-center px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-orange-600 hover:bg-orange-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-orange-500">
              <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
              </svg>
              Refresh
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
      <!-- Real-time Stats Cards -->
      <div class="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4 mb-8">
        <!-- Today's Money -->
        <div class="bg-white overflow-hidden shadow rounded-lg" data-testid="today-money">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-orange-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">
                    Today's Money
                  </dt>
                  <dd class="flex items-baseline">
                    <div class="text-2xl font-semibold text-gray-900">
                      {{ formatCurrency(dashboardData.todayMoney) }}
                    </div>
                    <div class="ml-2 flex items-baseline text-sm font-semibold" :class="dashboardData.todayMoneyChange >= 0 ? 'text-green-600' : 'text-red-600'">
                      <svg class="self-center flex-shrink-0 h-5 w-5" :class="dashboardData.todayMoneyChange >= 0 ? 'text-green-500' : 'text-red-500'" fill="currentColor" viewBox="0 0 20 20">
                        <path v-if="dashboardData.todayMoneyChange >= 0" fill-rule="evenodd" d="M5.293 9.707a1 1 0 010-1.414l4-4a1 1 0 011.414 0l4 4a1 1 0 01-1.414 1.414L11 7.414V15a1 1 0 11-2 0V7.414L6.707 9.707a1 1 0 01-1.414 0z" clip-rule="evenodd" />
                        <path v-else fill-rule="evenodd" d="M14.707 10.293a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 111.414-1.414L9 13.586V6a1 1 0 012 0v7.586l2.293-2.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                      </svg>
                      <span class="sr-only">{{ dashboardData.todayMoneyChange >= 0 ? 'Increased' : 'Decreased' }} by</span>
                      {{ dashboardData.todayMoneyChange >= 0 ? '+' : '' }}{{ dashboardData.todayMoneyChange }}%
                    </div>
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <!-- Today's Users -->
        <div class="bg-white overflow-hidden shadow rounded-lg" data-testid="today-users">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-orange-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">
                    Today's Users
                  </dt>
                  <dd class="flex items-baseline">
                    <div class="text-2xl font-semibold text-gray-900">
                      {{ dashboardData.todayUsers.toLocaleString() }}
                    </div>
                    <div class="ml-2 flex items-baseline text-sm font-semibold" :class="dashboardData.todayUsersChange >= 0 ? 'text-green-600' : 'text-red-600'">
                      <svg class="self-center flex-shrink-0 h-5 w-5" :class="dashboardData.todayUsersChange >= 0 ? 'text-green-500' : 'text-red-500'" fill="currentColor" viewBox="0 0 20 20">
                        <path v-if="dashboardData.todayUsersChange >= 0" fill-rule="evenodd" d="M5.293 9.707a1 1 0 010-1.414l4-4a1 1 0 011.414 0l4 4a1 1 0 01-1.414 1.414L11 7.414V15a1 1 0 11-2 0V7.414L6.707 9.707a1 1 0 01-1.414 0z" clip-rule="evenodd" />
                        <path v-else fill-rule="evenodd" d="M14.707 10.293a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 111.414-1.414L9 13.586V6a1 1 0 012 0v7.586l2.293-2.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                      </svg>
                      <span class="sr-only">{{ dashboardData.todayUsersChange >= 0 ? 'Increased' : 'Decreased' }} by</span>
                      {{ dashboardData.todayUsersChange >= 0 ? '+' : '' }}{{ dashboardData.todayUsersChange }}%
                    </div>
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <!-- New Clients -->
        <div class="bg-white overflow-hidden shadow rounded-lg" data-testid="new-clients">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-orange-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M18 9v3m0 0v3m0-3h3m-3 0h-3m-2-5a4 4 0 11-8 0 4 4 0 018 0zM3 20a6 6 0 0112 0v1H3v-1z" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">
                    New Clients
                  </dt>
                  <dd class="flex items-baseline">
                    <div class="text-2xl font-semibold text-gray-900">
                      +{{ dashboardData.newClients.toLocaleString() }}
                    </div>
                    <div class="ml-2 flex items-baseline text-sm font-semibold" :class="dashboardData.newClientsChange >= 0 ? 'text-green-600' : 'text-red-600'">
                      <svg class="self-center flex-shrink-0 h-5 w-5" :class="dashboardData.newClientsChange >= 0 ? 'text-green-500' : 'text-red-500'" fill="currentColor" viewBox="0 0 20 20">
                        <path v-if="dashboardData.newClientsChange >= 0" fill-rule="evenodd" d="M5.293 9.707a1 1 0 010-1.414l4-4a1 1 0 011.414 0l4 4a1 1 0 01-1.414 1.414L11 7.414V15a1 1 0 11-2 0V7.414L6.707 9.707a1 1 0 01-1.414 0z" clip-rule="evenodd" />
                        <path v-else fill-rule="evenodd" d="M14.707 10.293a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 111.414-1.414L9 13.586V6a1 1 0 012 0v7.586l2.293-2.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                      </svg>
                      <span class="sr-only">{{ dashboardData.newClientsChange >= 0 ? 'Increased' : 'Decreased' }} by</span>
                      {{ dashboardData.newClientsChange >= 0 ? '+' : '' }}{{ dashboardData.newClientsChange }}%
                    </div>
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <!-- Sales -->
        <div class="bg-white overflow-hidden shadow rounded-lg" data-testid="sales">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-orange-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">
                    Sales
                  </dt>
                  <dd class="flex items-baseline">
                    <div class="text-2xl font-semibold text-gray-900">
                      {{ formatCurrency(dashboardData.sales) }}
                    </div>
                    <div class="ml-2 flex items-baseline text-sm font-semibold" :class="dashboardData.salesChange >= 0 ? 'text-green-600' : 'text-red-600'">
                      <svg class="self-center flex-shrink-0 h-5 w-5" :class="dashboardData.salesChange >= 0 ? 'text-green-500' : 'text-red-500'" fill="currentColor" viewBox="0 0 20 20">
                        <path v-if="dashboardData.salesChange >= 0" fill-rule="evenodd" d="M5.293 9.707a1 1 0 010-1.414l4-4a1 1 0 011.414 0l4 4a1 1 0 01-1.414 1.414L11 7.414V15a1 1 0 11-2 0V7.414L6.707 9.707a1 1 0 01-1.414 0z" clip-rule="evenodd" />
                        <path v-else fill-rule="evenodd" d="M14.707 10.293a1 1 0 010 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 111.414-1.414L9 13.586V6a1 1 0 012 0v7.586l2.293-2.293a1 1 0 011.414 0z" clip-rule="evenodd" />
                      </svg>
                      <span class="sr-only">{{ dashboardData.salesChange >= 0 ? 'Increased' : 'Decreased' }} by</span>
                      {{ dashboardData.salesChange >= 0 ? '+' : '' }}{{ dashboardData.salesChange }}%
                    </div>
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Charts and Analytics Section -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-8 mb-8">
        <!-- Sales Chart -->
        <div class="bg-white shadow rounded-lg p-6">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Sales Trend</h3>
          <div class="h-64 flex items-center justify-center">
            <div class="text-center">
              <svg class="w-16 h-16 text-gray-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
              </svg>
              <p class="text-gray-500">Chart component will be implemented</p>
            </div>
          </div>
        </div>

        <!-- Revenue Chart -->
        <div class="bg-white shadow rounded-lg p-6">
          <h3 class="text-lg font-medium text-gray-900 mb-4">Revenue Overview</h3>
          <div class="h-64 flex items-center justify-center">
            <div class="text-center">
              <svg class="w-16 h-16 text-gray-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
              <p class="text-gray-500">Chart component will be implemented</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Sales by Country and Global Sales -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-8 mb-8">
        <!-- Sales by Country -->
        <div class="bg-white shadow rounded-lg">
          <div class="px-6 py-4 border-b border-gray-200">
            <h3 class="text-lg font-medium text-gray-900">Sales by Country</h3>
          </div>
          <div class="overflow-x-auto">
            <table class="min-w-full divide-y divide-gray-200">
              <thead class="bg-gray-50">
                <tr>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Country</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Sales</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Value</th>
                  <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Bounce</th>
                </tr>
              </thead>
              <tbody class="bg-white divide-y divide-gray-200">
                <tr v-for="country in dashboardData.salesByCountry" :key="country.name">
                  <td class="px-6 py-4 whitespace-nowrap">
                    <div class="flex items-center">
                      <div class="flex-shrink-0 h-8 w-8">
                        <div class="h-8 w-8 rounded-full" :style="{ backgroundColor: country.color }"></div>
                      </div>
                      <div class="ml-4">
                        <div class="text-sm font-medium text-gray-900">{{ country.name }}</div>
                      </div>
                    </div>
                  </td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{{ country.sales.toLocaleString() }}</td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{{ formatCurrency(country.value) }}</td>
                  <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{{ country.bounce }}%</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Global Sales -->
        <div class="bg-white shadow rounded-lg">
          <div class="px-6 py-4 border-b border-gray-200">
            <h3 class="text-lg font-medium text-gray-900">Global Sales</h3>
            <p class="text-sm text-gray-500">Check the global stats</p>
          </div>
          <div class="p-6">
            <div class="text-center">
              <div class="text-3xl font-bold text-gray-900 mb-2">{{ formatCurrency(dashboardData.globalSales) }}</div>
              <div class="text-sm text-gray-500 mb-6">Total Sales</div>
              
              <div class="grid grid-cols-2 gap-4">
                <div class="text-center">
                  <div class="text-2xl font-bold text-gray-900">{{ dashboardData.reachedUsers.toLocaleString() }}</div>
                  <div class="text-sm text-gray-500">Reached Users</div>
                </div>
                <div class="text-center">
                  <div class="text-2xl font-bold text-gray-900">{{ dashboardData.activeUsers.toLocaleString() }}</div>
                  <div class="text-sm text-gray-500">Active Users</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Bottom Stats Row -->
      <div class="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4 mb-8">
        <div class="bg-white overflow-hidden shadow rounded-lg" data-testid="users-stat">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-blue-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">Users</dt>
                  <dd class="text-lg font-medium text-gray-900">{{ dashboardData.users.toLocaleString() }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white overflow-hidden shadow rounded-lg" data-testid="clicks-stat">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-green-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 15l-2 5L9 9l11 4-5 2zm0 0l5 5M7.188 2.239l.777 2.897M5.136 7.965l-2.898-.777M13.95 4.05l-2.122 2.122m-5.657 5.656l-2.122 2.122" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">Clicks</dt>
                  <dd class="text-lg font-medium text-gray-900">{{ dashboardData.clicks.toLocaleString() }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white overflow-hidden shadow rounded-lg" data-testid="sales-stat">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-purple-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">Sales</dt>
                  <dd class="text-lg font-medium text-gray-900">{{ formatCurrency(dashboardData.salesAmount) }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white overflow-hidden shadow rounded-lg" data-testid="items-stat">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-red-500 rounded-md flex items-center justify-center">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
                  </svg>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">Items</dt>
                  <dd class="text-lg font-medium text-gray-900">{{ dashboardData.items.toLocaleString() }}</dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Sales Overview -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-6 py-4 border-b border-gray-200">
          <h3 class="text-lg font-medium text-gray-900">Sales overview</h3>
          <p class="text-sm text-gray-500">{{ dashboardData.salesOverviewChange }}% more in comparison to last month</p>
        </div>
        <div class="p-6">
          <div class="h-64 flex items-center justify-center">
            <div class="text-center">
              <svg class="w-16 h-16 text-gray-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
              </svg>
              <p class="text-gray-500">Sales overview chart will be implemented</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Chatbot Component -->
    <Chatbot />
  </div>
</template>

<script setup lang="ts">
// Vue composables are auto-imported in Nuxt 3

// Types
interface DashboardData {
  todayMoney: number
  todayMoneyChange: number
  todayUsers: number
  todayUsersChange: number
  newClients: number
  newClientsChange: number
  sales: number
  salesChange: number
  globalSales: number
  reachedUsers: number
  activeUsers: number
  activeUsersChange: number
  users: number
  clicks: number
  salesAmount: number
  items: number
  salesOverviewChange: number
  salesByCountry: Array<{
    name: string
    sales: number
    value: number
    bounce: number
    color: string
  }>
}

// Reactive data
const dashboardData = ref<DashboardData>({
  todayMoney: 53000,
  todayMoneyChange: 55,
  todayUsers: 2300,
  todayUsersChange: 12,
  newClients: 3462,
  newClientsChange: 8,
  sales: 103430,
  salesChange: 23,
  globalSales: 103430,
  reachedUsers: 24500,
  activeUsers: 24500,
  activeUsersChange: 15,
  users: 24500,
  clicks: 24500,
  salesAmount: 103430,
  items: 24500,
  salesOverviewChange: 55,
  salesByCountry: [
    { name: 'United States', sales: 4000, value: 230900, bounce: 29.9, color: '#FF6384' },
    { name: 'Germany', sales: 3000, value: 440000, bounce: 40.22, color: '#36A2EB' },
    { name: 'Great Britain', sales: 2000, value: 290000, bounce: 53.78, color: '#FFCE56' },
    { name: 'Brasil', sales: 2780, value: 230900, bounce: 19.15, color: '#4BC0C0' }
  ]
})

// Methods
const refreshData = async () => {
  // Simulate API call
  await new Promise(resolve => setTimeout(resolve, 1000))
  
  // Update with random data
  dashboardData.value = {
    ...dashboardData.value,
    todayMoney: Math.floor(Math.random() * 100000) + 20000,
    todayUsers: Math.floor(Math.random() * 5000) + 1000,
    newClients: Math.floor(Math.random() * 5000) + 2000,
    sales: Math.floor(Math.random() * 200000) + 50000
  }
}

const exportData = () => {
  // Implement export functionality
  const dataStr = JSON.stringify(dashboardData.value, null, 2)
  const dataBlob = new Blob([dataStr], { type: 'application/json' })
  const url = URL.createObjectURL(dataBlob)
  const link = document.createElement('a')
  link.href = url
  link.download = 'dashboard-data.json'
  link.click()
  URL.revokeObjectURL(url)
}

const formatCurrency = (amount: number): string => {
  return new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0
  }).format(amount)
}

// Lifecycle
onMounted(() => {
  // Load initial data
  refreshData()
})
</script>

<style scoped>
/* Add any component-specific styles here */
</style>

