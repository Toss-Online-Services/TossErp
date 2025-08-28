<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Page Header -->
    <div class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="py-4">
          <div class="flex items-center justify-between">
            <div>
              <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Shared Asset & Tool Network</h1>
              <p class="text-gray-600 dark:text-gray-400">Access and share equipment, tools, and facilities across the TOSS network</p>
            </div>
            <div class="flex space-x-3">
              <button @click="openShareAssetModal" class="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors flex items-center">
                <PlusIcon class="w-5 h-5 mr-2" />
                Share Asset
              </button>
              <button @click="viewMyBookings" class="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors flex items-center">
                <CalendarIcon class="w-5 h-5 mr-2" />
                My Bookings
              </button>
              <button @click="openAssetMap" class="bg-purple-600 text-white px-4 py-2 rounded-lg hover:bg-purple-700 transition-colors flex items-center">
                <MapPinIcon class="w-5 h-5 mr-2" />
                Asset Map
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Asset Network Stats -->
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
      <div class="grid grid-cols-1 md:grid-cols-5 gap-6 mb-8">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-blue-100 dark:bg-blue-900/30">
              <TruckIcon class="w-6 h-6 text-blue-600 dark:text-blue-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Total Assets</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ assetStats.totalAssets }}</p>
            </div>
          </div>
        </div>
        
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-green-100 dark:bg-green-900/30">
              <CheckCircleIcon class="w-6 h-6 text-green-600 dark:text-green-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Available Now</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ assetStats.availableNow }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-yellow-100 dark:bg-yellow-900/30">
              <CurrencyDollarIcon class="w-6 h-6 text-yellow-600 dark:text-yellow-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Cost Saved</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">${{ assetStats.costSaved }}K</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-purple-100 dark:bg-purple-900/30">
              <CalendarIcon class="w-6 h-6 text-purple-600 dark:text-purple-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Active Bookings</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ assetStats.activeBookings }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <div class="flex items-center">
            <div class="p-3 rounded-full bg-indigo-100 dark:bg-indigo-900/30">
              <UserGroupIcon class="w-6 h-6 text-indigo-600 dark:text-indigo-400" />
            </div>
            <div class="ml-4">
              <p class="text-sm font-medium text-gray-600 dark:text-gray-400">Network Partners</p>
              <p class="text-2xl font-bold text-gray-900 dark:text-white">{{ assetStats.networkPartners }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Asset Categories Filter -->
      <div class="mb-6">
        <div class="flex flex-wrap gap-2">
          <button 
            v-for="category in assetCategories" 
            :key="category.id"
            @click="filterByCategory(category.id)"
            :class="[
              'px-4 py-2 rounded-lg text-sm font-medium transition-colors',
              selectedCategory === category.id 
                ? 'bg-blue-600 text-white' 
                : 'bg-white dark:bg-gray-800 text-gray-700 dark:text-gray-300 border border-gray-200 dark:border-gray-600 hover:bg-gray-50 dark:hover:bg-gray-700'
            ]"
          >
            <component :is="category.icon" class="w-4 h-4 mr-2 inline" />
            {{ category.name }} ({{ category.count }})
          </button>
        </div>
      </div>

      <!-- Available Assets Grid -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
        <div v-for="asset in filteredAssets" :key="asset.id" class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden">
          <!-- Asset Image -->
          <div class="h-48 bg-gray-200 dark:bg-gray-700 relative">
            <img v-if="asset.image" :src="asset.image" :alt="asset.name" class="w-full h-full object-cover" />
            <div v-else class="w-full h-full flex items-center justify-center">
              <component :is="getAssetIcon(asset.category)" class="w-16 h-16 text-gray-400" />
            </div>
            <div class="absolute top-2 right-2">
              <span :class="[
                'px-2 py-1 rounded-full text-xs font-medium',
                asset.available 
                  ? 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400' 
                  : 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400'
              ]">
                {{ asset.available ? 'Available' : 'Booked' }}
              </span>
            </div>
            <div class="absolute top-2 left-2">
              <span class="bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400 px-2 py-1 rounded-full text-xs font-medium">
                {{ asset.category }}
              </span>
            </div>
          </div>

          <!-- Asset Details -->
          <div class="p-6">
            <div class="flex items-start justify-between mb-2">
              <h3 class="text-lg font-semibold text-gray-900 dark:text-white">{{ asset.name }}</h3>
              <div class="flex items-center">
                <StarIcon v-for="i in 5" :key="i" class="w-4 h-4" :class="i <= asset.rating ? 'text-yellow-400' : 'text-gray-300'" />
                <span class="ml-1 text-sm text-gray-600 dark:text-gray-400">({{ asset.reviews }})</span>
              </div>
            </div>
            
            <p class="text-gray-600 dark:text-gray-400 text-sm mb-4">{{ asset.description }}</p>
            
            <!-- Owner Information -->
            <div class="flex items-center mb-4">
              <div class="flex-shrink-0 h-8 w-8">
                <div class="h-8 w-8 rounded-full bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center">
                  <span class="text-sm font-medium text-white">{{ asset.owner.charAt(0) }}</span>
                </div>
              </div>
              <div class="ml-3">
                <p class="text-sm font-medium text-gray-900 dark:text-white">{{ asset.owner }}</p>
                <p class="text-xs text-gray-600 dark:text-gray-400">{{ asset.location }}</p>
              </div>
              <div class="ml-auto">
                <div class="flex items-center">
                  <ShieldCheckIcon class="w-4 h-4 text-green-500 mr-1" />
                  <span class="text-xs text-green-600 dark:text-green-400">Verified</span>
                </div>
              </div>
            </div>

            <!-- Pricing and Details -->
            <div class="space-y-2 mb-4">
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">Daily Rate:</span>
                <span class="font-medium text-gray-900 dark:text-white">${{ asset.dailyRate }}</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">Security Deposit:</span>
                <span class="font-medium text-gray-900 dark:text-white">${{ asset.securityDeposit }}</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">Distance:</span>
                <span class="font-medium text-gray-900 dark:text-white">{{ asset.distance }} km</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">Usage Terms:</span>
                <span class="font-medium text-gray-900 dark:text-white">{{ asset.usageTerms }}</span>
              </div>
            </div>

            <!-- Features -->
            <div class="mb-4">
              <div class="flex flex-wrap gap-1">
                <span v-for="feature in asset.features" :key="feature" class="bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 px-2 py-1 rounded text-xs">
                  {{ feature }}
                </span>
              </div>
            </div>

            <!-- Action Buttons -->
            <div class="flex space-x-2">
              <button 
                v-if="asset.available"
                @click="bookAsset(asset)" 
                class="flex-1 bg-blue-600 text-white py-2 px-3 rounded-lg hover:bg-blue-700 transition-colors text-sm font-medium"
              >
                Book Now
              </button>
              <button 
                v-else
                @click="joinWaitlist(asset)" 
                class="flex-1 bg-gray-600 text-white py-2 px-3 rounded-lg hover:bg-gray-700 transition-colors text-sm font-medium"
              >
                Join Waitlist
              </button>
              <button @click="viewAssetDetails(asset)" class="px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700">
                Details
              </button>
              <button @click="contactOwner(asset)" class="px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700">
                Contact
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- My Shared Assets -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6 mb-8">
        <div class="flex items-center justify-between mb-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">My Shared Assets</h3>
          <span class="bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400 px-3 py-1 rounded-full text-sm font-medium">
            {{ mySharedAssets.length }} assets shared
          </span>
        </div>
        
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <div v-for="asset in mySharedAssets" :key="asset.id" class="border border-gray-200 dark:border-gray-600 rounded-lg p-4">
            <div class="flex items-center justify-between mb-2">
              <h4 class="font-medium text-gray-900 dark:text-white">{{ asset.name }}</h4>
              <span :class="[
                'px-2 py-1 rounded-full text-xs font-medium',
                asset.status === 'available' 
                  ? 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400'
                  : asset.status === 'booked'
                  ? 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400'
                  : 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400'
              ]">
                {{ asset.status }}
              </span>
            </div>
            
            <div class="space-y-1 text-sm">
              <div class="flex justify-between">
                <span class="text-gray-600 dark:text-gray-400">Total Bookings:</span>
                <span class="font-medium">{{ asset.totalBookings }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-gray-600 dark:text-gray-400">Revenue:</span>
                <span class="font-medium text-green-600">${{ asset.revenue }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-gray-600 dark:text-gray-400">Rating:</span>
                <div class="flex items-center">
                  <StarIcon v-for="i in 5" :key="i" class="w-3 h-3" :class="i <= asset.rating ? 'text-yellow-400' : 'text-gray-300'" />
                </div>
              </div>
            </div>
            
            <div class="mt-3 flex space-x-2">
              <button @click="manageAsset(asset)" class="text-blue-600 hover:text-blue-800 text-xs font-medium">Manage</button>
              <button @click="viewBookings(asset)" class="text-green-600 hover:text-green-800 text-xs font-medium">Bookings</button>
              <button @click="updateAsset(asset)" class="text-purple-600 hover:text-purple-800 text-xs font-medium">Update</button>
            </div>
          </div>
        </div>
      </div>

      <!-- Asset Usage Analytics -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <!-- Usage Trends -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Network Usage Trends</h3>
          <div class="space-y-4">
            <div class="flex justify-between items-center">
              <span class="text-sm text-gray-600 dark:text-gray-400">Equipment Sharing</span>
              <div class="flex items-center">
                <div class="w-32 bg-gray-200 rounded-full h-2 mr-2 dark:bg-gray-700">
                  <div class="bg-blue-600 h-2 rounded-full" style="width: 78%"></div>
                </div>
                <span class="text-sm font-medium">78%</span>
              </div>
            </div>
            <div class="flex justify-between items-center">
              <span class="text-sm text-gray-600 dark:text-gray-400">Facility Sharing</span>
              <div class="flex items-center">
                <div class="w-32 bg-gray-200 rounded-full h-2 mr-2 dark:bg-gray-700">
                  <div class="bg-green-600 h-2 rounded-full" style="width: 65%"></div>
                </div>
                <span class="text-sm font-medium">65%</span>
              </div>
            </div>
            <div class="flex justify-between items-center">
              <span class="text-sm text-gray-600 dark:text-gray-400">Vehicle Sharing</span>
              <div class="flex items-center">
                <div class="w-32 bg-gray-200 rounded-full h-2 mr-2 dark:bg-gray-700">
                  <div class="bg-purple-600 h-2 rounded-full" style="width: 45%"></div>
                </div>
                <span class="text-sm font-medium">45%</span>
              </div>
            </div>
            <div class="flex justify-between items-center">
              <span class="text-sm text-gray-600 dark:text-gray-400">Technology Assets</span>
              <div class="flex items-center">
                <div class="w-32 bg-gray-200 rounded-full h-2 mr-2 dark:bg-gray-700">
                  <div class="bg-yellow-600 h-2 rounded-full" style="width: 89%"></div>
                </div>
                <span class="text-sm font-medium">89%</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Cost Savings -->
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Cost Savings Impact</h3>
          <div class="space-y-4">
            <div class="text-center">
              <div class="text-3xl font-bold text-green-600 mb-1">$125,000</div>
              <div class="text-sm text-gray-600 dark:text-gray-400">Total Network Savings</div>
            </div>
            <div class="grid grid-cols-2 gap-4 text-center">
              <div>
                <div class="text-xl font-bold text-blue-600">68%</div>
                <div class="text-xs text-gray-600 dark:text-gray-400">Avg Cost Reduction</div>
              </div>
              <div>
                <div class="text-xl font-bold text-purple-600">$2,150</div>
                <div class="text-xs text-gray-600 dark:text-gray-400">Your Savings</div>
              </div>
            </div>
            <div class="space-y-2">
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">Equipment Purchase Avoided:</span>
                <span class="font-medium">$85K</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">Maintenance Cost Shared:</span>
                <span class="font-medium">$25K</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-gray-600 dark:text-gray-400">Storage Cost Saved:</span>
                <span class="font-medium">$15K</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Share Asset Modal -->
    <div v-if="showShareModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
      <div class="relative top-10 mx-auto p-5 border w-11/12 md:w-4/5 lg:w-3/4 shadow-lg rounded-md bg-white dark:bg-gray-800">
        <div class="mt-3">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-medium text-gray-900 dark:text-white">Share Asset with Network</h3>
            <button @click="closeShareModal" class="text-gray-400 hover:text-gray-600 dark:hover:text-gray-200">
              <XMarkIcon class="w-6 h-6" />
            </button>
          </div>
          
          <form @submit.prevent="submitSharedAsset" class="space-y-6">
            <!-- Basic Information -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Asset Name *</label>
                <input 
                  v-model="newAsset.name"
                  type="text" 
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  placeholder="e.g., Industrial 3D Printer"
                />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Category *</label>
                <select 
                  v-model="newAsset.category"
                  required
                  class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                >
                  <option value="">Select Category</option>
                  <option value="equipment">Equipment</option>
                  <option value="vehicles">Vehicles</option>
                  <option value="facilities">Facilities</option>
                  <option value="technology">Technology</option>
                  <option value="tools">Tools</option>
                </select>
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Description *</label>
              <textarea 
                v-model="newAsset.description"
                rows="3"
                required
                class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                placeholder="Describe the asset, its capabilities, and any special requirements..."
              ></textarea>
            </div>

            <!-- Pricing and Terms -->
            <div class="border-t pt-4">
              <h4 class="text-md font-medium text-gray-900 dark:text-white mb-4">Pricing & Terms</h4>
              <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Daily Rate *</label>
                  <input 
                    v-model="newAsset.dailyRate"
                    type="number" 
                    min="0"
                    step="0.01"
                    required
                    class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  />
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Security Deposit</label>
                  <input 
                    v-model="newAsset.securityDeposit"
                    type="number" 
                    min="0"
                    step="0.01"
                    class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  />
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Usage Terms</label>
                  <select 
                    v-model="newAsset.usageTerms"
                    class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  >
                    <option value="supervised">Supervised Use</option>
                    <option value="independent">Independent Use</option>
                    <option value="training-required">Training Required</option>
                    <option value="certified-only">Certified Users Only</option>
                  </select>
                </div>
              </div>
            </div>

            <!-- Availability -->
            <div class="border-t pt-4">
              <h4 class="text-md font-medium text-gray-900 dark:text-white mb-4">Availability</h4>
              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Available From</label>
                  <input 
                    v-model="newAsset.availableFrom"
                    type="date" 
                    class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                  />
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Location *</label>
                  <input 
                    v-model="newAsset.location"
                    type="text" 
                    required
                    class="w-full p-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white"
                    placeholder="e.g., Downtown Workshop, Building A"
                  />
                </div>
              </div>
            </div>

            <!-- Sharing Preferences -->
            <div class="border-t pt-4">
              <h4 class="text-md font-medium text-gray-900 dark:text-white mb-4">Sharing Preferences</h4>
              <div class="space-y-3">
                <div class="flex items-center">
                  <input 
                    v-model="newAsset.requireApproval"
                    type="checkbox" 
                    class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                  />
                  <label class="ml-2 text-sm text-gray-700 dark:text-gray-300">Require approval for bookings</label>
                </div>
                <div class="flex items-center">
                  <input 
                    v-model="newAsset.allowInstantBooking"
                    type="checkbox" 
                    class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                  />
                  <label class="ml-2 text-sm text-gray-700 dark:text-gray-300">Allow instant booking</label>
                </div>
                <div class="flex items-center">
                  <input 
                    v-model="newAsset.trackUsage"
                    type="checkbox" 
                    class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                  />
                  <label class="ml-2 text-sm text-gray-700 dark:text-gray-300">Enable usage tracking and reporting</label>
                </div>
                <div class="flex items-center">
                  <input 
                    v-model="newAsset.enableRatings"
                    type="checkbox" 
                    class="rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                  />
                  <label class="ml-2 text-sm text-gray-700 dark:text-gray-300">Enable user ratings and reviews</label>
                </div>
              </div>
            </div>
            
            <div class="flex items-center justify-end space-x-3 pt-4">
              <button 
                type="button"
                @click="closeShareModal"
                class="px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700"
              >
                Cancel
              </button>
              <button 
                type="submit"
                class="px-4 py-2 bg-blue-600 border border-transparent rounded-md text-sm font-medium text-white hover:bg-blue-700"
              >
                Share Asset
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
  CalendarIcon,
  MapPinIcon,
  TruckIcon,
  CheckCircleIcon,
  CurrencyDollarIcon,
  UserGroupIcon,
  StarIcon,
  ShieldCheckIcon,
  XMarkIcon,
  WrenchScrewdriverIcon,
  ComputerDesktopIcon,
  BuildingOfficeIcon,
  BeakerIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Shared Asset & Tool Network - TOSS ERP',
  meta: [
    { name: 'description', content: 'Network asset sharing platform for equipment, tools, and facilities' }
  ]
})

// Reactive data
const showShareModal = ref(false)
const selectedCategory = ref('all')

// Asset network statistics
const assetStats = ref({
  totalAssets: 156,
  availableNow: 89,
  costSaved: 125,
  activeBookings: 34,
  networkPartners: 67
})

// Asset categories
const assetCategories = ref([
  { id: 'all', name: 'All Assets', count: 156, icon: TruckIcon },
  { id: 'equipment', name: 'Equipment', count: 45, icon: WrenchScrewdriverIcon },
  { id: 'vehicles', name: 'Vehicles', count: 23, icon: TruckIcon },
  { id: 'facilities', name: 'Facilities', count: 18, icon: BuildingOfficeIcon },
  { id: 'technology', name: 'Technology', count: 38, icon: ComputerDesktopIcon },
  { id: 'tools', name: 'Tools', count: 32, icon: BeakerIcon }
])

// Available assets
const availableAssets = ref([
  {
    id: 1,
    name: 'Industrial 3D Printer (Ultimaker S5)',
    category: 'technology',
    description: 'High-precision industrial 3D printer suitable for prototyping and small-batch production',
    owner: 'TechFab Solutions',
    location: 'Innovation District, Building C',
    available: true,
    dailyRate: 200,
    securityDeposit: 500,
    distance: 2.5,
    usageTerms: 'Training Required',
    rating: 5,
    reviews: 12,
    features: ['Dual Extruder', 'Heated Bed', 'Air Filtration', 'Remote Monitoring'],
    image: null
  },
  {
    id: 2,
    name: 'Electric Forklift (5-ton)',
    category: 'equipment',
    description: 'Heavy-duty electric forklift for warehouse and logistics operations',
    owner: 'Metro Logistics Hub',
    location: 'Warehouse District, Dock 7',
    available: true,
    dailyRate: 150,
    securityDeposit: 1000,
    distance: 5.2,
    usageTerms: 'Certified Only',
    rating: 4,
    reviews: 8,
    features: ['Electric Power', 'Side Shift', 'Fork Positioner', 'LED Lighting'],
    image: null
  },
  {
    id: 3,
    name: 'Conference Room (50-seat)',
    category: 'facilities',
    description: 'Modern conference facility with AV equipment and catering options',
    owner: 'Business Center Plus',
    location: 'Downtown, Executive Tower',
    available: false,
    dailyRate: 300,
    securityDeposit: 200,
    distance: 1.8,
    usageTerms: 'Supervised',
    rating: 5,
    reviews: 25,
    features: ['AV Equipment', 'Video Conferencing', 'Catering Kitchen', 'Parking'],
    image: null
  },
  {
    id: 4,
    name: 'Mobile Crane (25-ton)',
    category: 'equipment',
    description: 'Mobile hydraulic crane for construction and heavy lifting',
    owner: 'Construction Alliance',
    location: 'Industrial Zone, Yard 12',
    available: true,
    dailyRate: 800,
    securityDeposit: 2000,
    distance: 8.7,
    usageTerms: 'Certified Only',
    rating: 4,
    reviews: 6,
    features: ['Hydraulic Boom', 'Remote Control', 'Load Monitor', 'Outriggers'],
    image: null
  },
  {
    id: 5,
    name: 'Delivery Van Fleet (3 vehicles)',
    category: 'vehicles',
    description: 'Clean electric delivery vans for last-mile logistics',
    owner: 'Green Fleet Co-op',
    location: 'East Side Depot',
    available: true,
    dailyRate: 120,
    securityDeposit: 800,
    distance: 4.1,
    usageTerms: 'Independent',
    rating: 4,
    reviews: 15,
    features: ['Electric Power', 'GPS Tracking', 'Climate Control', 'Loading Ramp'],
    image: null
  },
  {
    id: 6,
    name: 'Professional Recording Studio',
    category: 'facilities',
    description: 'Fully equipped recording studio with mixing boards and instruments',
    owner: 'Creative Sound Collective',
    location: 'Arts District, Studio Complex',
    available: true,
    dailyRate: 250,
    securityDeposit: 300,
    distance: 3.9,
    usageTerms: 'Supervised',
    rating: 5,
    reviews: 18,
    features: ['Mixing Console', 'Instruments', 'Vocal Booth', 'Mastering Suite'],
    image: null
  }
])

// My shared assets
const mySharedAssets = ref([
  {
    id: 101,
    name: 'Laser Cutter (CO2)',
    status: 'available',
    totalBookings: 15,
    revenue: 2250,
    rating: 5
  },
  {
    id: 102,
    name: 'Meeting Room (12-seat)',
    status: 'booked',
    totalBookings: 42,
    revenue: 3150,
    rating: 4
  },
  {
    id: 103,
    name: 'CNC Milling Machine',
    status: 'maintenance',
    totalBookings: 8,
    revenue: 1600,
    rating: 4
  }
])

// New asset form
const newAsset = ref({
  name: '',
  category: '',
  description: '',
  dailyRate: '',
  securityDeposit: '',
  usageTerms: 'independent',
  availableFrom: '',
  location: '',
  requireApproval: false,
  allowInstantBooking: true,
  trackUsage: true,
  enableRatings: true
})

// Computed properties
const filteredAssets = computed(() => {
  if (selectedCategory.value === 'all') {
    return availableAssets.value
  }
  return availableAssets.value.filter(asset => asset.category === selectedCategory.value)
})

// Helper functions
const getAssetIcon = (category: string) => {
  const icons = {
    equipment: WrenchScrewdriverIcon,
    vehicles: TruckIcon,
    facilities: BuildingOfficeIcon,
    technology: ComputerDesktopIcon,
    tools: BeakerIcon
  }
  return icons[category as keyof typeof icons] || TruckIcon
}

const filterByCategory = (categoryId: string) => {
  selectedCategory.value = categoryId
}

// Modal functions
const openShareAssetModal = () => {
  const today = new Date()
  newAsset.value.availableFrom = today.toISOString().split('T')[0]
  showShareModal.value = true
}

const closeShareModal = () => {
  showShareModal.value = false
  newAsset.value = {
    name: '',
    category: '',
    description: '',
    dailyRate: '',
    securityDeposit: '',
    usageTerms: 'independent',
    availableFrom: '',
    location: '',
    requireApproval: false,
    allowInstantBooking: true,
    trackUsage: true,
    enableRatings: true
  }
}

const submitSharedAsset = () => {
  // Create new shared asset
  const asset = {
    id: mySharedAssets.value.length + 101,
    name: newAsset.value.name,
    status: 'available',
    totalBookings: 0,
    revenue: 0,
    rating: 0,
    category: newAsset.value.category,
    description: newAsset.value.description,
    dailyRate: parseFloat(newAsset.value.dailyRate),
    settings: {
      requireApproval: newAsset.value.requireApproval,
      allowInstantBooking: newAsset.value.allowInstantBooking,
      trackUsage: newAsset.value.trackUsage,
      enableRatings: newAsset.value.enableRatings
    }
  }
  
  mySharedAssets.value.unshift(asset)
  closeShareModal()
  alert('Asset shared successfully! It will be visible to network members shortly.')
}

// Action functions
const bookAsset = (asset: any) => {
  console.log('Book asset:', asset)
  alert(`Booking "${asset.name}" for $${asset.dailyRate}/day. You'll be connected with ${asset.owner}.`)
}

const joinWaitlist = (asset: any) => {
  console.log('Join waitlist for asset:', asset)
  alert(`Added to waitlist for "${asset.name}". You'll be notified when it becomes available.`)
}

const viewAssetDetails = (asset: any) => {
  console.log('View asset details:', asset)
  // navigateTo(`/purchasing/assets/${asset.id}`)
  alert(`Asset details for "${asset.name}" will be implemented`)
}

const contactOwner = (asset: any) => {
  console.log('Contact asset owner:', asset)
  alert(`Contacting ${asset.owner} about "${asset.name}". Message system will be implemented.`)
}

const viewMyBookings = () => {
  // navigateTo('/purchasing/my-bookings')
  alert('My bookings page will be implemented')
}

const openAssetMap = () => {
  alert('Interactive asset map will be implemented')
}

const manageAsset = (asset: any) => {
  console.log('Manage asset:', asset)
  alert('Asset management interface will be implemented')
}

const viewBookings = (asset: any) => {
  console.log('View bookings for asset:', asset)
  alert('Booking history and calendar will be implemented')
}

const updateAsset = (asset: any) => {
  console.log('Update asset:', asset)
  alert('Asset update interface will be implemented')
}

onMounted(() => {
  console.log('Shared Asset & Tool Network page loaded')
})
</script>
