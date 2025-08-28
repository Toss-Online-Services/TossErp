<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Pooled Credit & Mutual Financing</h1>
              <p class="text-gray-600 dark:text-gray-400">Collaborative financing network for TOSS member organizations</p>
            </div>
            <div class="flex space-x-3">
              <button @click="openContributeModal" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center">
                <PlusIcon class="w-5 h-5 mr-2" />
                Contribute
              </button>
              <button @click="openLoanRequestModal" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center">
                <CurrencyDollarIcon class="w-5 h-5 mr-2" />
                Request Loan
              </button>
              <button @click="viewGovernance" class="bg-purple-600 text-white px-4 py-2 rounded-lg hover:bg-purple-700 transition-colors flex items-center">
                <ShieldCheckIcon class="w-5 h-5 mr-2" />
                Governance
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Credit Pool Overview -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-green-100 dark:bg-green-900/30">
              <BanknotesIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total Pool</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">${{ poolStats.totalPool }}K</p>
            </div>
          </div>
        </div>
        
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-blue-100 dark:bg-blue-900/30">
              <CurrencyDollarIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Available Credit</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">${{ poolStats.availableCredit }}K</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-yellow-100 dark:bg-yellow-900/30">
              <ChartBarIcon class="w-6 h-6 text-yellow-600 dark:text-yellow-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Active Loans</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ poolStats.activeLoans }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-purple-100 dark:bg-purple-900/30">
              <UserGroupIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Pool Members</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ poolStats.poolMembers }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Your Contribution & Position -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        <!-- Your Pool Position -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center justify-between mb-6">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Your Pool Position</h3>
            <span class="bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400 px-3 py-1 rounded-full text-sm font-medium">
              Active Member
            </span>
          </div>
          
          <div class="space-y-4">
            <div class="flex justify-between items-center p-4 bg-green-50 dark:bg-green-900/20 rounded-lg">
              <div>
                <p class="text-sm text-gray-600 dark:text-gray-400">Your Contribution</p>
                <p class="text-2xl font-bold text-green-600">${{ yourPosition.contribution }}K</p>
              </div>
              <div class="text-right">
                <p class="text-sm text-gray-600 dark:text-gray-400">Pool Share</p>
                <p class="text-lg font-semibold text-gray-900 dark:text-white">{{ yourPosition.poolShare }}%</p>
              </div>
            </div>
            
            <div class="grid grid-cols-2 gap-4">
              <div class="text-center p-3 border border-gray-200 dark:border-gray-600 rounded-lg">
                <p class="text-sm text-gray-600 dark:text-gray-400">Credit Limit</p>
                <p class="text-lg font-semibold text-blue-600">${{ yourPosition.creditLimit }}K</p>
              </div>
              <div class="text-center p-3 border border-gray-200 dark:border-gray-600 rounded-lg">
                <p class="text-sm text-gray-600 dark:text-gray-400">Used Credit</p>
                <p class="text-lg font-semibold text-orange-600">${{ yourPosition.usedCredit }}K</p>
              </div>
            </div>
            
            <div class="grid grid-cols-2 gap-4">
              <div class="text-center p-3 border border-gray-200 dark:border-gray-600 rounded-lg">
                <p class="text-sm text-gray-600 dark:text-gray-400">Interest Earned</p>
                <p class="text-lg font-semibold text-green-600">${{ yourPosition.interestEarned }}</p>
              </div>
              <div class="text-center p-3 border border-gray-200 dark:border-gray-600 rounded-lg">
                <p class="text-sm text-gray-600 dark:text-gray-400">Trust Score</p>
                <div class="flex items-center justify-center">
                  <StarIcon v-for="i in 5" :key="i" class="w-4 h-4" :class="i <= yourPosition.trustScore ? 'text-yellow-400' : 'text-gray-300'" />
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Pool Health & Analytics -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-6">Pool Health & Analytics</h3>
          
          <div class="space-y-4">
            <div>
              <div class="flex justify-between text-sm mb-2">
                <span class="text-gray-600 dark:text-gray-400">Pool Utilization</span>
                <span class="text-gray-900 dark:text-white">{{ poolHealth.utilization }}%</span>
              </div>
              <div class="w-full bg-gray-200 rounded-full h-3 dark:bg-gray-700">
                <div class="bg-blue-600 h-3 rounded-full" :style="{ width: poolHealth.utilization + '%' }"></div>
              </div>
            </div>
            
            <div>
              <div class="flex justify-between text-sm mb-2">
                <span class="text-gray-600 dark:text-gray-400">Default Rate</span>
                <span class="text-gray-900 dark:text-white">{{ poolHealth.defaultRate }}%</span>
              </div>
              <div class="w-full bg-gray-200 rounded-full h-3 dark:bg-gray-700">
                <div class="bg-red-600 h-3 rounded-full" :style="{ width: poolHealth.defaultRate + '%' }"></div>
              </div>
            </div>
            
            <div>
              <div class="flex justify-between text-sm mb-2">
                <span class="text-gray-600 dark:text-gray-400">Average ROI</span>
                <span class="text-gray-900 dark:text-white">{{ poolHealth.averageROI }}%</span>
              </div>
              <div class="w-full bg-gray-200 rounded-full h-3 dark:bg-gray-700">
                <div class="bg-green-600 h-3 rounded-full" :style="{ width: poolHealth.averageROI + '%' }"></div>
              </div>
            </div>
            
            <div class="pt-4 border-t border-gray-200 dark:border-gray-700">
              <div class="grid grid-cols-2 gap-4 text-center">
                <div>
                  <p class="text-lg font-bold text-blue-600">{{ poolHealth.avgLoanTerm }}</p>
                  <p class="text-xs text-gray-600 dark:text-gray-400">Avg Loan Term (months)</p>
                </div>
                <div>
                  <p class="text-lg font-bold text-green-600">{{ poolHealth.successRate }}%</p>
                  <p class="text-xs text-gray-600 dark:text-gray-400">Success Rate</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Active Loans & Requests -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        <!-- Your Active Loans -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center justify-between mb-6">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Your Active Loans</h3>
            <span class="bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400 px-3 py-1 rounded-full text-sm font-medium">
              {{ yourLoans.length }} active
            </span>
          </div>
          
          <div class="space-y-4">
            <div v-for="loan in yourLoans" :key="loan.id" class="border border-gray-200 dark:border-gray-600 rounded-lg p-4">
              <div class="flex items-center justify-between mb-2">
                <h4 class="font-medium text-gray-900 dark:text-white">{{ loan.purpose }}</h4>
                <span class="text-sm px-2 py-1 rounded-full" :class="getLoanStatusClass(loan.status)">{{ loan.status }}</span>
              </div>
              
              <div class="grid grid-cols-2 gap-4 text-sm mb-3">
                <div>
                  <span class="text-gray-600 dark:text-gray-400">Amount:</span>
                  <span class="font-medium ml-1">${{ loan.amount }}K</span>
                </div>
                <div>
                  <span class="text-gray-600 dark:text-gray-400">Interest Rate:</span>
                  <span class="font-medium ml-1">{{ loan.interestRate }}%</span>
                </div>
                <div>
                  <span class="text-gray-600 dark:text-gray-400">Term:</span>
                  <span class="font-medium ml-1">{{ loan.term }} months</span>
                </div>
                <div>
                  <span class="text-gray-600 dark:text-gray-400">Next Payment:</span>
                  <span class="font-medium ml-1">{{ formatDate(loan.nextPayment) }}</span>
                </div>
              </div>
              
              <!-- Payment Progress -->
              <div class="mb-3">
                <div class="flex justify-between text-sm mb-1">
                  <span class="text-gray-600 dark:text-gray-400">Payment Progress</span>
                  <span class="text-gray-900 dark:text-white">{{ loan.paymentsMade }}/{{ loan.totalPayments }}</span>
                </div>
                <div class="w-full bg-gray-200 rounded-full h-2 dark:bg-gray-700">
                  <div class="bg-green-600 h-2 rounded-full" :style="{ width: (loan.paymentsMade / loan.totalPayments * 100) + '%' }"></div>
                </div>
              </div>
              
              <div class="flex space-x-2">
                <button @click="makePayment(loan)" class="text-blue-600 hover:text-blue-800 text-sm font-medium">Make Payment</button>
                <button @click="viewLoanDetails(loan)" class="text-green-600 hover:text-green-800 text-sm font-medium">View Details</button>
                <button @click="renegotiateLoan(loan)" class="text-purple-600 hover:text-purple-800 text-sm font-medium">Renegotiate</button>
              </div>
            </div>
          </div>
        </div>

        <!-- Loan Requests for Review -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center justify-between mb-6">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Loan Requests for Review</h3>
            <span class="bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400 px-3 py-1 rounded-full text-sm font-medium">
              {{ pendingRequests.length }} pending
            </span>
          </div>
          
          <div class="space-y-4">
            <div v-for="request in pendingRequests" :key="request.id" class="border border-gray-200 dark:border-gray-600 rounded-lg p-4">
              <div class="flex items-center justify-between mb-2">
                <h4 class="font-medium text-gray-900 dark:text-white">{{ request.purpose }}</h4>
                <span class="text-sm px-2 py-1 rounded-full bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400">Pending Review</span>
              </div>
              
              <!-- Requestor Information -->
              <div class="flex items-center mb-3">
                <div class="flex-shrink-0 h-8 w-8">
                  <div class="h-8 w-8 rounded-full bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center">
                    <span class="text-sm font-medium text-white">{{ request.requestor.charAt(0) }}</span>
                  </div>
                </div>
                <div class="ml-3">
                  <p class="text-sm font-medium text-gray-900 dark:text-white">{{ request.requestor }}</p>
                  <div class="flex items-center">
                    <StarIcon v-for="i in 5" :key="i" class="w-3 h-3" :class="i <= request.trustScore ? 'text-yellow-400' : 'text-gray-300'" />
                    <span class="ml-1 text-xs text-gray-600 dark:text-gray-400">Trust Score</span>
                  </div>
                </div>
              </div>
              
              <div class="grid grid-cols-2 gap-4 text-sm mb-3">
                <div>
                  <span class="text-gray-600 dark:text-gray-400">Amount:</span>
                  <span class="font-medium ml-1">${{ request.amount }}K</span>
                </div>
                <div>
                  <span class="text-gray-600 dark:text-gray-400">Proposed Rate:</span>
                  <span class="font-medium ml-1">{{ request.proposedRate }}%</span>
                </div>
                <div>
                  <span class="text-gray-600 dark:text-gray-400">Term:</span>
                  <span class="font-medium ml-1">{{ request.term }} months</span>
                </div>
                <div>
                  <span class="text-gray-600 dark:text-gray-400">Risk Level:</span>
                  <span class="font-medium ml-1 text-orange-600">{{ request.riskLevel }}</span>
                </div>
              </div>
              
              <!-- Guarantors -->
              <div class="mb-3">
                <p class="text-xs text-gray-600 dark:text-gray-400 mb-1">Guarantors ({{ request.guarantors.length }}):</p>
                <div class="flex flex-wrap gap-1">
                  <span v-for="guarantor in request.guarantors" :key="guarantor" class="bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400 px-2 py-1 rounded text-xs">
                    {{ guarantor }}
                  </span>
                </div>
              </div>
              
              <div class="flex space-x-2">
                <button @click="approveRequest(request)" class="bg-green-600 text-white px-3 py-1 rounded text-sm hover:bg-green-700">Approve</button>
                <button @click="requestMoreInfo(request)" class="text-blue-600 hover:text-blue-800 text-sm font-medium">More Info</button>
                <button @click="suggestTerms(request)" class="text-purple-600 hover:text-purple-800 text-sm font-medium">Suggest Terms</button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Network Performance & Insights -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <!-- Member Performance -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Top Performing Members</h3>
          <div class="space-y-3">
            <div v-for="member in topMembers" :key="member.id" class="flex items-center justify-between">
              <div class="flex items-center">
                <div class="flex-shrink-0 h-8 w-8">
                  <div class="h-8 w-8 rounded-full bg-gradient-to-r from-green-500 to-blue-600 flex items-center justify-center">
                    <span class="text-sm font-medium text-white">{{ member.name.charAt(0) }}</span>
                  </div>
                </div>
                <div class="ml-3">
                  <p class="text-sm font-medium text-gray-900 dark:text-white">{{ member.name }}</p>
                  <div class="flex items-center">
                    <StarIcon v-for="i in 5" :key="i" class="w-3 h-3" :class="i <= member.trustScore ? 'text-yellow-400' : 'text-gray-300'" />
                  </div>
                </div>
              </div>
              <div class="text-right">
                <p class="text-sm font-medium text-green-600">${{ member.contribution }}K</p>
                <p class="text-xs text-gray-600 dark:text-gray-400">{{ member.loansIssued }} loans</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Risk Monitoring -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Risk Monitoring</h3>
          <div class="space-y-4">
            <div class="flex justify-between items-center">
              <span class="text-sm text-gray-600 dark:text-gray-400">Pool Risk Score</span>
              <div class="flex items-center">
                <div class="w-20 bg-gray-200 rounded-full h-2 mr-2 dark:bg-gray-700">
                  <div class="bg-green-600 h-2 rounded-full" style="width: 25%"></div>
                </div>
                <span class="text-sm font-medium text-green-600">Low</span>
              </div>
            </div>
            <div class="flex justify-between items-center">
              <span class="text-sm text-gray-600 dark:text-gray-400">Diversification</span>
              <div class="flex items-center">
                <div class="w-20 bg-gray-200 rounded-full h-2 mr-2 dark:bg-gray-700">
                  <div class="bg-blue-600 h-2 rounded-full" style="width: 85%"></div>
                </div>
                <span class="text-sm font-medium text-blue-600">High</span>
              </div>
            </div>
            <div class="flex justify-between items-center">
              <span class="text-sm text-gray-600 dark:text-gray-400">Liquidity Ratio</span>
              <div class="flex items-center">
                <div class="w-20 bg-gray-200 rounded-full h-2 mr-2 dark:bg-gray-700">
                  <div class="bg-yellow-600 h-2 rounded-full" style="width: 65%"></div>
                </div>
                <span class="text-sm font-medium text-yellow-600">Medium</span>
              </div>
            </div>
            <div class="pt-2 border-t border-gray-200 dark:border-gray-700">
              <p class="text-xs text-gray-600 dark:text-gray-400">Next risk assessment: <span class="font-medium">{{ formatDate(new Date(Date.now() + 7 * 24 * 60 * 60 * 1000)) }}</span></p>
            </div>
          </div>
        </div>

        <!-- Profit Distribution -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Profit Distribution</h3>
          <div class="space-y-4">
            <div class="text-center">
              <div class="text-2xl font-bold text-green-600 mb-1">${{ profitDistribution.totalProfit }}</div>
              <div class="text-sm text-gray-600 dark:text-gray-400">Total Pool Profit (YTD)</div>
            </div>
            <div class="space-y-2">
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">Your Share ({{ yourPosition.poolShare }}%):</span>
                <span class="font-medium text-green-600">${{ profitDistribution.yourShare }}</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">Reinvested:</span>
                <span class="font-medium">${{ profitDistribution.reinvested }}</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">Distributed:</span>
                <span class="font-medium">${{ profitDistribution.distributed }}</span>
              </div>
            </div>
            <div class="pt-2 border-t border-gray-200 dark:border-gray-700">
              <button @click="viewProfitHistory" class="w-full text-center text-blue-600 hover:text-blue-800 text-sm font-medium">
                View Distribution History
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Contribute Modal -->
    <div v-if="showContributeModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-10 mx-auto p-5 border w-11/12 md:w-2/3 lg:w-1/2 shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Contribute to Pool</h3>
            <button @click="closeContributeModal" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-200">
              <XMarkIcon class="w-6 h-6" />
            </button>
          </div>
          
          <form @submit.prevent="submitContribution" class="space-y-6">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Contribution Amount *</label>
                <input 
                  v-model="newContribution.amount"
                  type="number" 
                  min="1000"
                  step="100"
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  placeholder="e.g., 25000"
                />
                <p class="text-xs text-gray-600 dark:text-gray-400 mt-1">Minimum contribution: $1,000</p>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Contribution Type</label>
                <select 
                  v-model="newContribution.type"
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                >
                  <option value="cash">Cash Contribution</option>
                  <option value="asset">Asset Backing</option>
                  <option value="guarantee">Loan Guarantee</option>
                </select>
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Commitment Duration</label>
              <select 
                v-model="newContribution.duration"
                class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
              >
                <option value="6">6 months</option>
                <option value="12">12 months</option>
                <option value="24">24 months</option>
                <option value="36">36 months</option>
              </select>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Expected Return Rate</label>
              <div class="text-sm text-gray-600 dark:text-gray-400 mb-2">
                Current pool rate: {{ poolStats.currentRate }}% APR
              </div>
              <input 
                v-model="newContribution.expectedRate"
                type="number" 
                min="0"
                max="15"
                step="0.1"
                class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                :placeholder="poolStats.currentRate"
              />
            </div>

            <div class="space-y-3">
              <div class="flex items-center">
                <input 
                  v-model="newContribution.acceptTerms"
                  type="checkbox" 
                  required
                  class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                />
                <label class="ml-2 text-sm text-gray-700 dark:text-gray-300">I accept the pool terms and governance agreement</label>
              </div>
              <div class="flex items-center">
                <input 
                  v-model="newContribution.allowRebalancing"
                  type="checkbox" 
                  class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                />
                <label class="ml-2 text-sm text-gray-700 dark:text-gray-300">Allow automatic rebalancing for optimal returns</label>
              </div>
            </div>
            
            <div class="flex items-center justify-end space-x-3 pt-4">
              <button 
                type="button"
                @click="closeContributeModal"
                class="px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700"
              >
                Cancel
              </button>
              <button 
                type="submit"
                class="px-4 py-2 bg-green-600 border border-transparent rounded-md text-sm font-medium text-white hover:bg-green-700"
              >
                Submit Contribution
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>

    <!-- Loan Request Modal -->
    <div v-if="showLoanModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-10 mx-auto p-5 border w-11/12 md:w-4/5 lg:w-3/4 shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Request Pool Loan</h3>
            <button @click="closeLoanModal" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-200">
              <XMarkIcon class="w-6 h-6" />
            </button>
          </div>
          
          <form @submit.prevent="submitLoanRequest" class="space-y-6">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Loan Amount *</label>
                <input 
                  v-model="newLoanRequest.amount"
                  type="number" 
                  min="1000"
                  step="100"
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                />
                <p class="text-xs text-gray-600 dark:text-gray-400 mt-1">Your credit limit: ${{ yourPosition.creditLimit }}K</p>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Loan Purpose *</label>
                <select 
                  v-model="newLoanRequest.purpose"
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                >
                  <option value="">Select Purpose</option>
                  <option value="equipment">Equipment Purchase</option>
                  <option value="inventory">Inventory Financing</option>
                  <option value="expansion">Business Expansion</option>
                  <option value="working-capital">Working Capital</option>
                  <option value="group-buying">Group Buying Initiative</option>
                </select>
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Detailed Description *</label>
              <textarea 
                v-model="newLoanRequest.description"
                rows="3"
                required
                class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                placeholder="Explain how you'll use the funds and your repayment plan..."
              ></textarea>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Loan Term *</label>
                <select 
                  v-model="newLoanRequest.term"
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                >
                  <option value="6">6 months</option>
                  <option value="12">12 months</option>
                  <option value="18">18 months</option>
                  <option value="24">24 months</option>
                  <option value="36">36 months</option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Proposed Interest Rate</label>
                <input 
                  v-model="newLoanRequest.proposedRate"
                  type="number" 
                  min="0"
                  max="20"
                  step="0.1"
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  placeholder="e.g., 6.5"
                />
                <p class="text-xs text-gray-600 dark:text-gray-400 mt-1">Pool range: 4.5% - 12%</p>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Collateral Value</label>
                <input 
                  v-model="newLoanRequest.collateral"
                  type="number" 
                  min="0"
                  step="100"
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                />
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Guarantors</label>
              <div class="space-y-2">
                <input 
                  v-for="(guarantor, index) in newLoanRequest.guarantors" 
                  :key="index"
                  v-model="newLoanRequest.guarantors[index]"
                  type="text" 
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  :placeholder="`Guarantor ${index + 1} (Organization Name)`"
                />
                <button 
                  type="button"
                  @click="addGuarantor"
                  class="text-blue-600 hover:text-blue-800 text-sm font-medium"
                >
                  + Add Another Guarantor
                </button>
              </div>
            </div>

            <div class="space-y-3">
              <div class="flex items-center">
                <input 
                  v-model="newLoanRequest.acceptPoolTerms"
                  type="checkbox" 
                  required
                  class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                />
                <label class="ml-2 text-sm text-gray-700 dark:text-gray-300">I accept the pool lending terms and repayment obligations</label>
              </div>
              <div class="flex items-center">
                <input 
                  v-model="newLoanRequest.allowPublicReview"
                  type="checkbox" 
                  class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                />
                <label class="ml-2 text-sm text-gray-700 dark:text-gray-300">Allow public review by all pool members</label>
              </div>
            </div>
            
            <div class="flex items-center justify-end space-x-3 pt-4">
              <button 
                type="button"
                @click="closeLoanModal"
                class="px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700"
              >
                Cancel
              </button>
              <button 
                type="submit"
                class="px-4 py-2 bg-blue-600 border border-transparent rounded-md text-sm font-medium text-white hover:bg-blue-700"
              >
                Submit Request
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import {
  PlusIcon,
  CurrencyDollarIcon,
  ShieldCheckIcon,
  BanknotesIcon,
  ChartBarIcon,
  UserGroupIcon,
  StarIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Pooled Credit & Mutual Financing - TOSS ERP',
  meta: [
    { name: 'description', content: 'Collaborative financing network for TOSS member organizations' }
  ]
})

// Reactive data
const showContributeModal = ref(false)
const showLoanModal = ref(false)

// Pool statistics
const poolStats = ref({
  totalPool: 850,
  availableCredit: 245,
  activeLoans: 18,
  poolMembers: 34,
  currentRate: 6.2
})

// Your position in the pool
const yourPosition = ref({
  contribution: 25,
  poolShare: 2.9,
  creditLimit: 75,
  usedCredit: 15,
  interestEarned: 1250,
  trustScore: 4
})

// Pool health metrics
const poolHealth = ref({
  utilization: 71,
  defaultRate: 2.1,
  averageROI: 8.5,
  avgLoanTerm: 18,
  successRate: 94
})

// Your active loans
const yourLoans = ref([
  {
    id: 1,
    purpose: 'Equipment Purchase',
    amount: 15,
    interestRate: 5.8,
    term: 24,
    status: 'active',
    paymentsMade: 8,
    totalPayments: 24,
    nextPayment: new Date('2025-09-15')
  }
])

// Pending loan requests for review
const pendingRequests = ref([
  {
    id: 1,
    purpose: 'Inventory Financing',
    requestor: 'Green Valley Farms',
    amount: 45,
    proposedRate: 6.5,
    term: 12,
    riskLevel: 'Medium',
    trustScore: 4,
    guarantors: ['Organic Collective', 'Farm Alliance Co-op']
  },
  {
    id: 2,
    purpose: 'Technology Upgrade',
    requestor: 'TechStart Solutions',
    amount: 30,
    proposedRate: 7.2,
    term: 18,
    riskLevel: 'Low',
    trustScore: 5,
    guarantors: ['Innovation Hub', 'Tech Accelerator']
  }
])

// Top performing members
const topMembers = ref([
  {
    id: 1,
    name: 'Metro Business Alliance',
    contribution: 120,
    trustScore: 5,
    loansIssued: 12
  },
  {
    id: 2,
    name: 'Green Enterprise Network',
    contribution: 95,
    trustScore: 4,
    loansIssued: 8
  },
  {
    id: 3,
    name: 'Tech Innovation Co-op',
    contribution: 80,
    trustScore: 5,
    loansIssued: 15
  }
])

// Profit distribution
const profitDistribution = ref({
  totalProfit: 42500,
  yourShare: 1233,
  reinvested: 25500,
  distributed: 17000
})

// Form data
const newContribution = ref({
  amount: '',
  type: 'cash',
  duration: '12',
  expectedRate: '',
  acceptTerms: false,
  allowRebalancing: true
})

const newLoanRequest = ref({
  amount: '',
  purpose: '',
  description: '',
  term: '12',
  proposedRate: '',
  collateral: '',
  guarantors: [''],
  acceptPoolTerms: false,
  allowPublicReview: true
})

// Helper functions
const getLoanStatusClass = (status: string) => {
  const classes = {
    active: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    overdue: 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400',
    completed: 'bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-400'
  }
  return classes[status as keyof typeof classes] || 'bg-gray-100 text-gray-800'
}

const formatDate = (date: Date) => {
  return date.toLocaleDateString('en-US', { 
    month: 'short', 
    day: 'numeric',
    year: 'numeric'
  })
}

// Modal functions
const openContributeModal = () => {
  newContribution.value.expectedRate = poolStats.value.currentRate.toString()
  showContributeModal.value = true
}

const closeContributeModal = () => {
  showContributeModal.value = false
  newContribution.value = {
    amount: '',
    type: 'cash',
    duration: '12',
    expectedRate: '',
    acceptTerms: false,
    allowRebalancing: true
  }
}

const openLoanRequestModal = () => {
  newLoanRequest.value.proposedRate = poolStats.value.currentRate.toString()
  showLoanModal.value = true
}

const closeLoanModal = () => {
  showLoanModal.value = false
  newLoanRequest.value = {
    amount: '',
    purpose: '',
    description: '',
    term: '12',
    proposedRate: '',
    collateral: '',
    guarantors: [''],
    acceptPoolTerms: false,
    allowPublicReview: true
  }
}

const submitContribution = () => {
  // Process pool contribution
  console.log('Submitting contribution:', newContribution.value)
  
  // Update your position
  const contributionAmount = parseFloat(newContribution.value.amount) / 1000
  yourPosition.value.contribution += contributionAmount
  poolStats.value.totalPool += contributionAmount
  
  // Recalculate pool share
  yourPosition.value.poolShare = (yourPosition.value.contribution / poolStats.value.totalPool * 100)
  yourPosition.value.creditLimit = yourPosition.value.contribution * 3 // 3x leverage
  
  closeContributeModal()
  alert('Contribution submitted successfully! Your pool position has been updated.')
}

const submitLoanRequest = () => {
  // Process loan request
  const request = {
    id: pendingRequests.value.length + 1,
    purpose: newLoanRequest.value.purpose,
    requestor: 'Your Organization',
    amount: parseFloat(newLoanRequest.value.amount),
    proposedRate: parseFloat(newLoanRequest.value.proposedRate),
    term: parseInt(newLoanRequest.value.term),
    riskLevel: 'Low', // Would be calculated based on your profile
    trustScore: yourPosition.value.trustScore,
    guarantors: newLoanRequest.value.guarantors.filter(g => g.trim() !== '')
  }
  
  pendingRequests.value.unshift(request)
  closeLoanModal()
  alert('Loan request submitted! Pool members will review and respond shortly.')
}

const addGuarantor = () => {
  newLoanRequest.value.guarantors.push('')
}

// Action functions
const makePayment = (loan: any) => {
  console.log('Make payment for loan:', loan)
  alert('Payment interface will be implemented')
}

const viewLoanDetails = (loan: any) => {
  console.log('View loan details:', loan)
  alert('Loan details page will be implemented')
}

const renegotiateLoan = (loan: any) => {
  console.log('Renegotiate loan:', loan)
  alert('Loan renegotiation interface will be implemented')
}

const approveRequest = (request: any) => {
  console.log('Approve loan request:', request)
  alert('Loan approval workflow will be implemented')
}

const requestMoreInfo = (request: any) => {
  console.log('Request more info:', request)
  alert('Information request system will be implemented')
}

const suggestTerms = (request: any) => {
  console.log('Suggest terms for request:', request)
  alert('Terms negotiation interface will be implemented')
}

const viewGovernance = () => {
  alert('Pool governance interface will be implemented')
}

const viewProfitHistory = () => {
  alert('Profit distribution history will be implemented')
}

onMounted(() => {
  console.log('Pooled Credit & Mutual Financing page loaded')
})
</script>
