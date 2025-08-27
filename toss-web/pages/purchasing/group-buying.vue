<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Group Buying & Collective Procurement</h1>
              <p class="text-gray-600 dark:text-gray-400">Collaborate with network partners for better pricing and shared resources</p>
            </div>
            <div class="flex space-x-3">
              <button @click="openCreateGroupBuyModal" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center">
                <PlusIcon class="w-5 h-5 mr-2" />
                Start Group Buy
              </button>
              <button @click="findExistingGroupBuys" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center">
                <MagnifyingGlassIcon class="w-5 h-5 mr-2" />
                Find Group Buys
              </button>
              <button @click="viewNetworkMap" class="bg-purple-600 text-white px-4 py-2 rounded-lg hover:bg-purple-700 transition-colors flex items-center">
                <MapIcon class="w-5 h-5 mr-2" />
                Network Map
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- TOSS Network Stats -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="grid grid-cols-1 md:grid-cols-5 gap-6 mb-8">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-blue-100 dark:bg-blue-900/30">
              <UserGroupIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Network Members</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ networkStats.totalMembers }}</p>
            </div>
          </div>
        </div>
        
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-green-100 dark:bg-green-900/30">
              <ShoppingCartIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Active Group Buys</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ networkStats.activeGroupBuys }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-yellow-100 dark:bg-yellow-900/30">
              <CurrencyDollarIcon class="w-6 h-6 text-yellow-600 dark:text-yellow-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total Savings</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">${{ networkStats.totalSavings }}K</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-purple-100 dark:bg-purple-900/30">
              <TruckIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Shared Assets</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ networkStats.sharedAssets }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-indigo-100 dark:bg-indigo-900/30">
              <StarIcon class="w-6 h-6 text-indigo-600 dark:text-indigo-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Trust Score</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ networkStats.trustScore }}/5</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Active Group Buys -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        <!-- My Group Buys -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">My Active Group Buys</h3>
            <span class="bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400 px-2 py-1 rounded-full text-xs font-medium">{{ myGroupBuys.length }} active</span>
          </div>
          <div class="space-y-4">
            <div v-for="groupBuy in myGroupBuys" :key="groupBuy.id" class="border border-gray-200 dark:border-gray-600 rounded-lg p-4">
              <div class="flex items-center justify-between mb-2">
                <h4 class="font-medium text-gray-900 dark:text-white">{{ groupBuy.title }}</h4>
                <span class="text-sm px-2 py-1 rounded-full" :class="getStatusClass(groupBuy.status)">{{ groupBuy.status }}</span>
              </div>
              <p class="text-sm text-gray-600 dark:text-gray-400 mb-3">{{ groupBuy.description }}</p>
              
              <!-- Progress Bar -->
              <div class="mb-3">
                <div class="flex justify-between text-sm mb-1">
                  <span class="text-gray-600 dark:text-gray-400">Progress</span>
                  <span class="text-gray-900 dark:text-white">{{ groupBuy.committed }}/{{ groupBuy.minQuantity }} units</span>
                </div>
                <div class="w-full bg-gray-200 rounded-full h-2 dark:bg-gray-700">
                  <div class="bg-blue-600 h-2 rounded-full" :style="{ width: (groupBuy.committed / groupBuy.minQuantity * 100) + '%' }"></div>
                </div>
              </div>

              <!-- Collaboration Details -->
              <div class="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <span class="text-gray-600 dark:text-gray-400">Members:</span>
                  <span class="font-medium ml-1">{{ groupBuy.memberCount }}</span>
                </div>
                <div>
                  <span class="text-gray-600 dark:text-gray-400">Est. Savings:</span>
                  <span class="font-medium ml-1 text-green-600">${{ groupBuy.estimatedSavings }}</span>
                </div>
                <div>
                  <span class="text-gray-600 dark:text-gray-400">Deadline:</span>
                  <span class="font-medium ml-1">{{ formatDate(groupBuy.deadline) }}</span>
                </div>
                <div>
                  <span class="text-gray-600 dark:text-gray-400">Trust Level:</span>
                  <div class="flex items-center ml-1">
                    <StarIcon v-for="i in 5" :key="i" class="w-3 h-3" :class="i <= groupBuy.trustLevel ? 'text-yellow-400' : 'text-gray-300'" />
                  </div>
                </div>
              </div>

              <div class="mt-4 flex space-x-2">
                <button @click="viewGroupBuyDetails(groupBuy)" class="text-blue-600 hover:text-blue-800 text-sm font-medium">View Details</button>
                <button @click="inviteMembers(groupBuy)" class="text-green-600 hover:text-green-800 text-sm font-medium">Invite Members</button>
                <button @click="negotiateTerms(groupBuy)" class="text-purple-600 hover:text-purple-800 text-sm font-medium">Negotiate</button>
              </div>
            </div>
          </div>
        </div>

        <!-- Available Group Buys -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Available to Join</h3>
            <span class="bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400 px-2 py-1 rounded-full text-xs font-medium">{{ availableGroupBuys.length }} opportunities</span>
          </div>
          <div class="space-y-4">
            <div v-for="groupBuy in availableGroupBuys" :key="groupBuy.id" class="border border-gray-200 dark:border-gray-600 rounded-lg p-4">
              <div class="flex items-center justify-between mb-2">
                <h4 class="font-medium text-gray-900 dark:text-white">{{ groupBuy.title }}</h4>
                <span class="text-sm px-2 py-1 rounded-full bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400">Open</span>
              </div>
              <p class="text-sm text-gray-600 dark:text-gray-400 mb-3">{{ groupBuy.description }}</p>
              
              <!-- Lead Organization -->
              <div class="flex items-center mb-3">
                <div class="flex-shrink-0 h-6 w-6">
                  <div class="h-6 w-6 rounded-full bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center">
                    <span class="text-xs font-medium text-white">{{ groupBuy.leadOrganization.charAt(0) }}</span>
                  </div>
                </div>
                <div class="ml-2">
                  <span class="text-sm text-gray-600 dark:text-gray-400">Led by </span>
                  <span class="text-sm font-medium text-gray-900 dark:text-white">{{ groupBuy.leadOrganization }}</span>
                  <div class="flex items-center ml-1">
                    <StarIcon v-for="i in 5" :key="i" class="w-3 h-3" :class="i <= groupBuy.leadTrustScore ? 'text-yellow-400' : 'text-gray-300'" />
                  </div>
                </div>
              </div>

              <!-- Join Details -->
              <div class="grid grid-cols-2 gap-4 text-sm mb-4">
                <div>
                  <span class="text-gray-600 dark:text-gray-400">Current Members:</span>
                  <span class="font-medium ml-1">{{ groupBuy.memberCount }}</span>
                </div>
                <div>
                  <span class="text-gray-600 dark:text-gray-400">Potential Savings:</span>
                  <span class="font-medium ml-1 text-green-600">{{ groupBuy.savingsPercentage }}%</span>
                </div>
                <div>
                  <span class="text-gray-600 dark:text-gray-400">Min Commitment:</span>
                  <span class="font-medium ml-1">{{ groupBuy.minCommitment }} units</span>
                </div>
                <div>
                  <span class="text-gray-600 dark:text-gray-400">Deadline:</span>
                  <span class="font-medium ml-1">{{ formatDate(groupBuy.deadline) }}</span>
                </div>
              </div>

              <div class="flex space-x-2">
                <button @click="joinGroupBuy(groupBuy)" class="bg-blue-600 text-white px-3 py-1 rounded text-sm hover:bg-blue-700">Join Group Buy</button>
                <button @click="requestInfo(groupBuy)" class="text-blue-600 hover:text-blue-800 text-sm font-medium">Request Info</button>
                <button @click="checkCompatibility(groupBuy)" class="text-purple-600 hover:text-purple-800 text-sm font-medium">Check Compatibility</button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- TOSS Network Collaboration Features -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
        <!-- Shared Asset Pool -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center mb-4">
            <div class="p-3 rounded-full bg-orange-100 dark:bg-orange-900/30">
              <TruckIcon class="w-8 h-8 text-orange-600 dark:text-orange-400" />
            </div>
            <div class="ml-4">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Shared Asset Pool</h3>
              <p class="text-sm text-gray-600 dark:text-gray-400">Network equipment & tools</p>
            </div>
          </div>
          <div class="space-y-3">
            <div v-for="asset in sharedAssets" :key="asset.id" class="flex items-center justify-between">
              <div>
                <p class="text-sm font-medium text-gray-900 dark:text-white">{{ asset.name }}</p>
                <p class="text-xs text-gray-600 dark:text-gray-400">{{ asset.owner }} • {{ asset.location }}</p>
              </div>
              <div class="text-right">
                <span class="text-xs px-2 py-1 rounded-full" :class="asset.available ? 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400' : 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400'">
                  {{ asset.available ? 'Available' : 'Booked' }}
                </span>
                <p class="text-xs text-gray-600 dark:text-gray-400 mt-1">${{ asset.dailyRate }}/day</p>
              </div>
            </div>
          </div>
          <NuxtLink to="/purchasing/asset-sharing" class="block mt-4 text-center bg-orange-600 text-white py-2 rounded-lg hover:bg-orange-700 transition-colors">
            Browse All Assets
          </NuxtLink>
        </div>

        <!-- Pooled Credit & Financing -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center mb-4">
            <div class="p-3 rounded-full bg-green-100 dark:bg-green-900/30">
              <BanknotesIcon class="w-8 h-8 text-green-600 dark:text-green-400" />
            </div>
            <div class="ml-4">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Pooled Credit</h3>
              <p class="text-sm text-gray-600 dark:text-gray-400">Mutual financing network</p>
            </div>
          </div>
          <div class="space-y-3">
            <div class="flex justify-between">
              <span class="text-sm text-gray-600 dark:text-gray-400">Available Credit:</span>
              <span class="text-sm font-medium">${{ pooledCredit.availableCredit }}K</span>
            </div>
            <div class="flex justify-between">
              <span class="text-sm text-gray-600 dark:text-gray-400">Your Contribution:</span>
              <span class="text-sm font-medium">${{ pooledCredit.myContribution }}K</span>
            </div>
            <div class="flex justify-between">
              <span class="text-sm text-gray-600 dark:text-gray-400">Network Rate:</span>
              <span class="text-sm font-medium">{{ pooledCredit.interestRate }}% APR</span>
            </div>
            <div class="flex justify-between">
              <span class="text-sm text-gray-600 dark:text-gray-400">Trust Level:</span>
              <div class="flex items-center">
                <StarIcon v-for="i in 5" :key="i" class="w-3 h-3" :class="i <= pooledCredit.trustLevel ? 'text-yellow-400' : 'text-gray-300'" />
              </div>
            </div>
          </div>
          <div class="mt-4 space-y-2">
            <button @click="requestFinancing" class="w-full bg-green-600 text-white py-2 rounded-lg hover:bg-green-700 transition-colors">
              Request Financing
            </button>
            <button @click="viewCreditPool" class="w-full text-green-600 border border-green-600 py-2 rounded-lg hover:bg-green-50 dark:hover:bg-green-900/20 transition-colors">
              View Credit Pool
            </button>
          </div>
        </div>

        <!-- Network Analytics -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center mb-4">
            <div class="p-3 rounded-full bg-purple-100 dark:bg-purple-900/30">
              <ChartBarIcon class="w-8 h-8 text-purple-600 dark:text-purple-400" />
            </div>
            <div class="ml-4">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Network Analytics</h3>
              <p class="text-sm text-gray-600 dark:text-gray-400">Collaboration insights</p>
            </div>
          </div>
          <div class="space-y-3">
            <div class="flex justify-between">
              <span class="text-sm text-gray-600 dark:text-gray-400">Participation Rate:</span>
              <span class="text-sm font-medium">{{ analytics.participationRate }}%</span>
            </div>
            <div class="flex justify-between">
              <span class="text-sm text-gray-600 dark:text-gray-400">Avg Savings:</span>
              <span class="text-sm font-medium text-green-600">{{ analytics.avgSavings }}%</span>
            </div>
            <div class="flex justify-between">
              <span class="text-sm text-gray-600 dark:text-gray-400">Success Rate:</span>
              <span class="text-sm font-medium">{{ analytics.successRate }}%</span>
            </div>
            <div class="flex justify-between">
              <span class="text-sm text-gray-600 dark:text-gray-400">Network Growth:</span>
              <span class="text-sm font-medium text-blue-600">+{{ analytics.networkGrowth }}%</span>
            </div>
          </div>
          <div class="mt-4">
            <div class="text-xs text-gray-600 dark:text-gray-400 mb-2">Your Network Score</div>
            <div class="w-full bg-gray-200 rounded-full h-3 dark:bg-gray-700">
              <div class="bg-purple-600 h-3 rounded-full" :style="{ width: analytics.yourNetworkScore + '%' }"></div>
            </div>
            <div class="text-right text-xs text-gray-600 dark:text-gray-400 mt-1">{{ analytics.yourNetworkScore }}/100</div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Group Buy Modal -->
    <div v-if="showCreateModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-10 mx-auto p-5 border w-11/12 md:w-4/5 lg:w-3/4 shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Start New Group Buy</h3>
            <button @click="closeCreateModal" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-200">
              <XMarkIcon class="w-6 h-6" />
            </button>
          </div>
          
          <form @submit.prevent="submitGroupBuy" class="space-y-6">
            <!-- Basic Information -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Group Buy Title *</label>
                <input 
                  v-model="newGroupBuy.title"
                  type="text" 
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  placeholder="e.g., Office Supplies Bulk Purchase"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Category *</label>
                <select 
                  v-model="newGroupBuy.category"
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                >
                  <option value="">Select Category</option>
                  <option value="office-supplies">Office Supplies</option>
                  <option value="equipment">Equipment</option>
                  <option value="raw-materials">Raw Materials</option>
                  <option value="services">Services</option>
                  <option value="technology">Technology</option>
                </select>
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Description *</label>
              <textarea 
                v-model="newGroupBuy.description"
                rows="3"
                required
                class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                placeholder="Describe what you want to purchase and why others should join..."
              ></textarea>
            </div>

            <!-- Collaboration Settings -->
            <div class="border-t pt-4">
              <h4 class="text-md font-medium text-gray-900 dark:text-white mb-4">Collaboration Settings</h4>
              <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Minimum Quantity *</label>
                  <input 
                    v-model="newGroupBuy.minQuantity"
                    type="number" 
                    min="1"
                    required
                    class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  />
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Target Price Per Unit</label>
                  <input 
                    v-model="newGroupBuy.targetPrice"
                    type="number" 
                    step="0.01"
                    class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  />
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Deadline *</label>
                  <input 
                    v-model="newGroupBuy.deadline"
                    type="date" 
                    required
                    class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  />
                </div>
              </div>
            </div>

            <!-- Network Permissions -->
            <div class="border-t pt-4">
              <h4 class="text-md font-medium text-gray-900 dark:text-white mb-4">Network Permissions</h4>
              <div class="space-y-3">
                <div class="flex items-center">
                  <input 
                    v-model="newGroupBuy.openToPublic"
                    type="checkbox" 
                    class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                  />
                  <label class="ml-2 text-sm text-gray-700 dark:text-gray-300">Open to public TOSS network</label>
                </div>
                <div class="flex items-center">
                  <input 
                    v-model="newGroupBuy.requireApproval"
                    type="checkbox" 
                    class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                  />
                  <label class="ml-2 text-sm text-gray-700 dark:text-gray-300">Require approval for new members</label>
                </div>
                <div class="flex items-center">
                  <input 
                    v-model="newGroupBuy.allowSharedAssets"
                    type="checkbox" 
                    class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                  />
                  <label class="ml-2 text-sm text-gray-700 dark:text-gray-300">Allow shared asset contributions</label>
                </div>
                <div class="flex items-center">
                  <input 
                    v-model="newGroupBuy.enablePooledCredit"
                    type="checkbox" 
                    class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                  />
                  <label class="ml-2 text-sm text-gray-700 dark:text-gray-300">Enable pooled credit financing</label>
                </div>
              </div>
            </div>
            
            <div class="flex items-center justify-end space-x-3 pt-4">
              <button 
                type="button"
                @click="closeCreateModal"
                class="px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700"
              >
                Cancel
              </button>
              <button 
                type="submit"
                class="px-4 py-2 bg-blue-600 border border-transparent rounded-md text-sm font-medium text-white hover:bg-blue-700"
              >
                Start Group Buy
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import {
  PlusIcon,
  MagnifyingGlassIcon,
  MapIcon,
  UserGroupIcon,
  ShoppingCartIcon,
  CurrencyDollarIcon,
  TruckIcon,
  StarIcon,
  BanknotesIcon,
  ChartBarIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Group Buying & Collective Procurement - TOSS ERP',
  meta: [
    { name: 'description', content: 'Collaborative procurement and group buying platform for TOSS network members' }
  ]
})

// Reactive data
const showCreateModal = ref(false)

// TOSS Network Statistics
const networkStats = ref({
  totalMembers: 1247,
  activeGroupBuys: 23,
  totalSavings: 485,
  sharedAssets: 156,
  trustScore: 4.3
})

// Mock data for group buys
const myGroupBuys = ref([
  {
    id: 1,
    title: 'Office Supplies Bulk Purchase',
    description: 'High-quality office supplies for Q2 2025',
    status: 'collecting',
    committed: 850,
    minQuantity: 1000,
    memberCount: 12,
    estimatedSavings: 1250,
    deadline: new Date('2025-09-15'),
    trustLevel: 4
  },
  {
    id: 2,
    title: 'Industrial Cleaning Equipment',
    description: 'Professional cleaning equipment for warehouses',
    status: 'negotiating',
    committed: 5,
    minQuantity: 8,
    memberCount: 5,
    estimatedSavings: 3200,
    deadline: new Date('2025-09-30'),
    trustLevel: 5
  }
])

const availableGroupBuys = ref([
  {
    id: 3,
    title: 'IT Hardware Upgrade Bundle',
    description: 'Enterprise-grade servers and networking equipment',
    leadOrganization: 'Tech Solutions Network',
    leadTrustScore: 4,
    memberCount: 8,
    savingsPercentage: 18,
    minCommitment: 2,
    deadline: new Date('2025-09-20')
  },
  {
    id: 4,
    title: 'Renewable Energy Components',
    description: 'Solar panels and battery systems for sustainable operations',
    leadOrganization: 'Green Enterprise Alliance',
    leadTrustScore: 5,
    memberCount: 15,
    savingsPercentage: 25,
    minCommitment: 1,
    deadline: new Date('2025-10-01')
  }
])

// Shared assets in the network
const sharedAssets = ref([
  {
    id: 1,
    name: 'Forklift (5-ton)',
    owner: 'Metro Logistics',
    location: 'Downtown Warehouse',
    available: true,
    dailyRate: 150
  },
  {
    id: 2,
    name: '3D Printer (Industrial)',
    owner: 'TechPrint Co',
    location: 'Innovation Hub',
    available: false,
    dailyRate: 200
  },
  {
    id: 3,
    name: 'Conference Room (50-seat)',
    owner: 'Business Center Plus',
    location: 'City Center',
    available: true,
    dailyRate: 300
  }
])

// Pooled credit information
const pooledCredit = ref({
  availableCredit: 150,
  myContribution: 25,
  interestRate: 4.5,
  trustLevel: 4
})

// Network analytics
const analytics = ref({
  participationRate: 78,
  avgSavings: 22,
  successRate: 89,
  networkGrowth: 15,
  yourNetworkScore: 82
})

// New group buy form
const newGroupBuy = ref({
  title: '',
  category: '',
  description: '',
  minQuantity: '',
  targetPrice: '',
  deadline: '',
  openToPublic: true,
  requireApproval: false,
  allowSharedAssets: true,
  enablePooledCredit: false
})

// Helper functions
const getStatusClass = (status: string) => {
  const classes = {
    collecting: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    negotiating: 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
    confirmed: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
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
const openCreateGroupBuyModal = () => {
  const tomorrow = new Date()
  tomorrow.setDate(tomorrow.getDate() + 30)
  newGroupBuy.value.deadline = tomorrow.toISOString().split('T')[0]
  showCreateModal.value = true
}

const closeCreateModal = () => {
  showCreateModal.value = false
  newGroupBuy.value = {
    title: '',
    category: '',
    description: '',
    minQuantity: '',
    targetPrice: '',
    deadline: '',
    openToPublic: true,
    requireApproval: false,
    allowSharedAssets: true,
    enablePooledCredit: false
  }
}

const submitGroupBuy = () => {
  // Create new group buy with TOSS collaboration features
  const groupBuy = {
    id: myGroupBuys.value.length + 1,
    title: newGroupBuy.value.title,
    description: newGroupBuy.value.description,
    status: 'collecting',
    committed: 0,
    minQuantity: parseInt(newGroupBuy.value.minQuantity),
    memberCount: 1,
    estimatedSavings: 0,
    deadline: new Date(newGroupBuy.value.deadline),
    trustLevel: 4,
    category: newGroupBuy.value.category,
    settings: {
      openToPublic: newGroupBuy.value.openToPublic,
      requireApproval: newGroupBuy.value.requireApproval,
      allowSharedAssets: newGroupBuy.value.allowSharedAssets,
      enablePooledCredit: newGroupBuy.value.enablePooledCredit
    }
  }
  
  myGroupBuys.value.unshift(groupBuy)
  closeCreateModal()
  alert('Group buy created successfully! Network members will be notified.')
}

// Action functions
const viewGroupBuyDetails = (groupBuy: any) => {
  console.log('View group buy details:', groupBuy)
  navigateTo(`/purchasing/group-buying/${groupBuy.id}`)
}

const inviteMembers = (groupBuy: any) => {
  console.log('Invite members to group buy:', groupBuy)
  alert('Member invitation feature will be implemented')
}

const negotiateTerms = (groupBuy: any) => {
  console.log('Negotiate terms for group buy:', groupBuy)
  alert('Terms negotiation feature will be implemented')
}

const joinGroupBuy = (groupBuy: any) => {
  console.log('Join group buy:', groupBuy)
  alert(`Joining "${groupBuy.title}". You'll be added to the collaboration network.`)
}

const requestInfo = (groupBuy: any) => {
  console.log('Request info for group buy:', groupBuy)
  alert('Information request sent to group buy organizer')
}

const checkCompatibility = (groupBuy: any) => {
  console.log('Check compatibility for group buy:', groupBuy)
  alert('Compatibility check will analyze your business profile and requirements')
}

const findExistingGroupBuys = () => {
  alert('Advanced search for group buys will be implemented')
}

const viewNetworkMap = () => {
  navigateTo('/purchasing/network-map')
}

const requestFinancing = () => {
  navigateTo('/purchasing/pooled-credit')
}

const viewCreditPool = () => {
  navigateTo('/purchasing/pooled-credit')
}

onMounted(() => {
  console.log('Group Buying & Collective Procurement page loaded')
})
</script>
