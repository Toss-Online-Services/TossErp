<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-green-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header with Glass Morphism -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 sm:py-6">
          <div class="flex items-center justify-between">
          <div class="flex-1 min-w-0">
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-green-600 to-blue-600 bg-clip-text text-transparent">
              Group Buying
            </h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
              Buy together with others to save money
            </p>
            </div>
          <div class="flex space-x-2 sm:space-x-3 flex-shrink-0">
            <button
              @click="showCreateModal = true"
              class="inline-flex items-center justify-center px-4 sm:px-6 py-2.5 sm:py-3 bg-gradient-to-r from-green-600 to-blue-600 text-white rounded-xl hover:from-green-700 hover:to-blue-700 shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 font-semibold text-sm sm:text-base"
            >
                <PlusIcon class="w-5 h-5 mr-2" />
                Start Group Buy
              </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      
      <!-- Network Stats -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Network Members</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ networkStats.totalMembers }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-blue-500 to-purple-600 rounded-xl">
              <UserGroupIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Active Groups</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ networkStats.activeGroupBuys }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl">
              <ShoppingCartIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Total Savings</p>
              <p class="text-3xl font-bold text-green-600">R{{ networkStats.totalSavings }}K</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-yellow-500 to-orange-600 rounded-xl">
              <CurrencyDollarIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Trust Score</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ networkStats.trustScore }}/5</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-purple-500 to-pink-600 rounded-xl">
              <StarIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>
      </div>

      <!-- Group Buys Tabs -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- My Group Buys -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 overflow-hidden">
          <div class="bg-gradient-to-r from-blue-50 to-purple-50 dark:from-blue-900/20 dark:to-purple-900/20 px-6 py-4 border-b border-slate-200 dark:border-slate-600">
            <div class="flex items-center justify-between">
              <h3 class="text-lg font-bold text-slate-900 dark:text-white">My Active Group Buys</h3>
              <div class="flex items-center gap-2">
                <button
                  @click="loadActiveGroupBuys"
                  class="p-2 hover:bg-white/50 dark:hover:bg-slate-700 rounded-lg transition-colors"
                  title="Refresh"
                >
                  <ArrowPathIcon class="w-4 h-4 text-blue-600 dark:text-blue-400" />
                </button>
                <span class="bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400 px-3 py-1 rounded-full text-sm font-medium">
                  {{ myGroupBuys.length }} active
                </span>
              </div>
          </div>
              </div>
          
          <div class="p-6 space-y-4">
            <div v-for="group in myGroupBuys" :key="group.id" 
              class="border border-slate-200 dark:border-slate-600 rounded-xl p-4 hover:shadow-md transition-all duration-200"
            >
              <div class="flex items-center justify-between mb-3">
                <h4 class="font-semibold text-slate-900 dark:text-white">{{ group.title }}</h4>
                <span 
                  class="px-2 py-1 rounded-full text-xs font-medium"
                  :class="getStatusClass(group.status)"
                >
                  {{ group.status }}
                </span>
              </div>
              <p class="text-sm text-slate-600 dark:text-slate-400 mb-3">{{ group.description }}</p>
              
              <!-- Item Details (if available) -->
              <div v-if="group.itemName" class="mb-3 p-3 bg-gradient-to-r from-purple-50 to-blue-50 dark:from-purple-900/20 dark:to-blue-900/20 rounded-lg border border-purple-200 dark:border-purple-800">
                <div class="flex items-center justify-between">
                  <div>
                    <p class="text-sm font-semibold text-purple-900 dark:text-purple-100">{{ group.itemName }}</p>
                    <p class="text-xs text-purple-600 dark:text-purple-400">SKU: {{ group.itemSku }}</p>
                  </div>
                  <div class="text-right">
                    <p class="text-sm font-bold text-purple-900 dark:text-purple-100">{{ group.myQuantity }} units</p>
                    <p class="text-xs text-purple-600 dark:text-purple-400">Your commitment</p>
                  </div>
                </div>
              </div>
              
              <!-- Progress Bar -->
              <div class="mb-3">
                <div class="flex justify-between text-sm mb-2">
                  <span class="text-slate-600 dark:text-slate-400">Progress</span>
                  <span class="font-medium text-slate-900 dark:text-white">{{ group.committed }}/{{ group.minQuantity }} units</span>
                </div>
                <div class="w-full bg-slate-200 rounded-full h-2 dark:bg-slate-700">
                  <div 
                    class="bg-gradient-to-r from-blue-600 to-purple-600 h-2 rounded-full transition-all duration-300" 
                    :style="{ width: Math.min((group.committed / group.minQuantity * 100), 100) + '%' }"
                  ></div>
                </div>
              </div>

              <!-- Details -->
              <div class="grid grid-cols-2 gap-4 text-sm mb-3">
                <div>
                  <span class="text-slate-600 dark:text-slate-400">Members:</span>
                  <span class="font-medium ml-1 text-slate-900 dark:text-white">{{ group.memberCount }}</span>
                </div>
                <div>
                  <span class="text-slate-600 dark:text-slate-400">Savings:</span>
                  <span class="font-medium ml-1 text-green-600">R{{ group.estimatedSavings }}</span>
                </div>
                <div>
                  <span class="text-slate-600 dark:text-slate-400">Deadline:</span>
                  <span class="font-medium ml-1 text-slate-900 dark:text-white">{{ formatDate(group.deadline) }}</span>
                </div>
                <div class="flex items-center">
                  <span class="text-slate-600 dark:text-slate-400">Rating:</span>
                  <div class="flex ml-1">
                    <StarIcon v-for="i in 5" :key="i" class="w-3 h-3" :class="i <= group.trustLevel ? 'text-yellow-400' : 'text-slate-300'" />
                  </div>
                </div>
              </div>

              <!-- Actions -->
              <div class="flex space-x-2">
                <button 
                  @click="viewGroupDetails(group)" 
                  class="flex-1 px-3 py-2 bg-gradient-to-r from-blue-600 to-purple-600 text-white rounded-lg hover:from-blue-700 hover:to-purple-700 shadow-md hover:shadow-lg transition-all duration-200 text-sm font-medium"
                >
                  View Details
                </button>
                <button 
                  @click="inviteMembers(group)" 
                  class="px-3 py-2 border-2 border-slate-300 dark:border-slate-600 rounded-lg text-sm font-medium text-slate-700 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-700 transition-all duration-200"
                >
                  Invite
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Available Group Buys -->
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 overflow-hidden">
          <div class="bg-gradient-to-r from-green-50 to-blue-50 dark:from-green-900/20 dark:to-blue-900/20 px-6 py-4 border-b border-slate-200 dark:border-slate-600">
            <div class="flex items-center justify-between">
              <h3 class="text-lg font-bold text-slate-900 dark:text-white">Available to Join</h3>
              <span class="bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400 px-3 py-1 rounded-full text-sm font-medium">
                {{ availableGroupBuys.length}} opportunities
              </span>
          </div>
              </div>
          
          <div class="p-6 space-y-4">
            <div v-for="group in availableGroupBuys" :key="group.id" 
              class="border border-slate-200 dark:border-slate-600 rounded-xl p-4 hover:shadow-md transition-all duration-200"
            >
              <div class="flex items-center justify-between mb-3">
                <h4 class="font-semibold text-slate-900 dark:text-white">{{ group.title }}</h4>
                <span class="px-2 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400">
                  Open
                </span>
              </div>
              <p class="text-sm text-slate-600 dark:text-slate-400 mb-3">{{ group.description }}</p>
              
              <!-- Lead Organization -->
              <div class="flex items-center mb-3 p-2 bg-slate-50 dark:bg-slate-700/50 rounded-lg">
                <div class="flex-shrink-0 h-8 w-8">
                  <div class="h-8 w-8 rounded-full bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center">
                    <span class="text-sm font-medium text-white">{{ group.leadOrganization.charAt(0) }}</span>
                  </div>
                </div>
                <div class="ml-3 flex-1">
                  <p class="text-sm font-medium text-slate-900 dark:text-white">{{ group.leadOrganization }}</p>
                  <div class="flex items-center">
                    <StarIcon v-for="i in 5" :key="i" class="w-3 h-3" :class="i <= group.leadTrustScore ? 'text-yellow-400' : 'text-slate-300'" />
                  </div>
                </div>
              </div>

              <!-- Join Details -->
              <div class="grid grid-cols-2 gap-3 text-sm mb-4">
                <div>
                  <span class="text-slate-600 dark:text-slate-400">Members:</span>
                  <span class="font-medium ml-1 text-slate-900 dark:text-white">{{ group.memberCount }}</span>
                </div>
                <div>
                  <span class="text-slate-600 dark:text-slate-400">Savings:</span>
                  <span class="font-medium ml-1 text-green-600">{{ group.savingsPercentage }}%</span>
                </div>
                <div>
                  <span class="text-slate-600 dark:text-slate-400">Min Qty:</span>
                  <span class="font-medium ml-1 text-slate-900 dark:text-white">{{ group.minCommitment }} units</span>
                </div>
                <div>
                  <span class="text-slate-600 dark:text-slate-400">Deadline:</span>
                  <span class="font-medium ml-1 text-slate-900 dark:text-white">{{ formatDate(group.deadline) }}</span>
                </div>
              </div>

              <!-- Actions -->
              <div class="flex space-x-2">
                <button 
                  @click="joinGroup(group)" 
                  class="flex-1 px-3 py-2 bg-gradient-to-r from-green-600 to-blue-600 text-white rounded-lg hover:from-green-700 hover:to-blue-700 shadow-md hover:shadow-lg transition-all duration-200 text-sm font-medium"
                >
                  Join Group
                </button>
                <button 
                  @click="viewGroupDetails(group)" 
                  class="px-3 py-2 border-2 border-slate-300 dark:border-slate-600 rounded-lg text-sm font-medium text-slate-700 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-700 transition-all duration-200"
                >
                  Details
                </button>
              </div>
              </div>
            </div>
          </div>
        </div>
      </div>

    <!-- Create Group Buy Modal -->
    <Transition name="modal">
      <div v-if="showCreateModal" class="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 overflow-y-auto">
        <div class="flex min-h-full items-center justify-center p-4">
          <div class="relative bg-white dark:bg-slate-800 rounded-2xl shadow-2xl border border-slate-200 dark:border-slate-700 w-full max-w-3xl">
            
            <!-- Header -->
            <div class="bg-gradient-to-r from-green-600 to-blue-600 px-6 py-4 flex items-center justify-between rounded-t-2xl">
              <div>
                <h3 class="text-xl font-bold text-white">Start New Group Buy</h3>
                <p class="text-sm text-white/80">Collaborate with others for better pricing</p>
            </div>
              <button @click="showCreateModal = false" class="p-2 hover:bg-white/20 rounded-lg transition-colors">
                <XMarkIcon class="w-6 h-6 text-white" />
              </button>
            </div>

            <!-- Form -->
            <div class="p-6 max-h-[70vh] overflow-y-auto">
              <form @submit.prevent="createGroupBuy" class="space-y-6">
                <!-- Category Selection - FIRST STEP -->
              <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                    <svg class="w-4 h-4 inline mr-1 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z"></path>
                    </svg>
                    Step 1: Select Category *
                  </label>
                  <div class="relative">
                    <select
                      v-model="newGroupBuy.category"
                      required
                      @change="handleCategoryChange"
                      class="w-full appearance-none px-4 py-3 pr-10 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-purple-500 focus:border-purple-500 transition-all font-medium cursor-pointer"
                      :class="{ 'border-purple-500 bg-purple-50 dark:bg-purple-900/20': newGroupBuy.category }"
                    >
                      <option value="">Select a category to continue</option>
                      <option value="food">🍽️ Food & Beverages</option>
                      <option value="supplies">📋 Office Supplies</option>
                      <option value="equipment">🔧 Equipment & Tools</option>
                      <option value="technology">💻 Technology</option>
                      <option value="cleaning">🧹 Cleaning Supplies</option>
                      <option value="other">📦 Other</option>
                    </select>
                    <div class="absolute right-3 top-1/2 -translate-y-1/2 pointer-events-none">
                      <svg class="w-5 h-5 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
                      </svg>
              </div>
              </div>
                  <p v-if="newGroupBuy.category" class="text-xs text-purple-600 dark:text-purple-400 mt-1">
                    ✓ Category selected. Now search for suppliers in this category.
                  </p>
            </div>

                <!-- Supplier Search - SECOND STEP (appears after category selected) -->
                <Transition name="slide-down">
                  <div v-if="newGroupBuy.category" class="relative">
                    <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                      <svg class="w-4 h-4 inline mr-1 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"></path>
                      </svg>
                      Step 2: Search for Supplier *
                    </label>
                    <div class="relative">
                      <input
                        v-model="supplierSearchQuery"
                        @input="handleSupplierInput"
                        @focus="showSupplierDropdown = true"
                        @blur="setTimeout(() => showSupplierDropdown = false, 200)"
                        type="text"
                        required
                        :placeholder="`Search suppliers in ${getCategoryName(newGroupBuy.category)}...`"
                        class="w-full px-4 py-3 pr-10 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-green-500 focus:border-green-500 transition-all"
                        :class="{ 'border-green-500': selectedSupplier }"
                      />
                      <div class="absolute right-3 top-1/2 -translate-y-1/2">
                        <button
                          v-if="selectedSupplier"
                          @click.prevent="clearSupplier"
                          type="button"
                          class="text-green-600 hover:text-green-700"
                        >
                          <XMarkIcon class="w-5 h-5" />
                        </button>
                        <MagnifyingGlassIcon v-else class="w-5 h-5 text-slate-400" />
          </div>
        </div>

                  <!-- Selected Supplier Badge -->
                  <div v-if="selectedSupplier" class="mt-2 flex items-center gap-2 p-3 bg-green-50 dark:bg-green-900/20 rounded-lg border-2 border-green-200 dark:border-green-800">
                    <CheckCircleIcon class="w-5 h-5 text-green-600 flex-shrink-0" />
                    <div class="flex-1">
                      <p class="font-semibold text-slate-900 dark:text-white">{{ selectedSupplier.name }}</p>
                      <div class="flex items-center gap-2 mt-1">
                        <div class="flex items-center">
                          <StarIcon v-for="i in 5" :key="i" class="w-3 h-3" 
                            :class="i <= Math.round(selectedSupplier.rating) ? 'text-yellow-400' : 'text-slate-300'" 
                          />
            </div>
                        <span class="text-xs text-slate-600 dark:text-slate-400">{{ selectedSupplier.rating }}/5</span>
            </div>
          </div>
            </div>

                  <!-- Supplier Dropdown -->
                  <Transition name="dropdown">
                    <div 
                      v-if="showSupplierDropdown && filteredSuppliers.length > 0" 
                      class="absolute z-10 w-full mt-2 bg-white dark:bg-slate-800 rounded-xl shadow-2xl border-2 border-slate-200 dark:border-slate-700 max-h-80 overflow-y-auto"
                    >
                      <div class="p-2">
                        <button
                          v-for="supplier in filteredSuppliers"
                          :key="supplier.id"
                          @click.prevent="selectSupplier(supplier)"
                          type="button"
                          class="w-full text-left px-4 py-3 rounded-lg hover:bg-blue-50 dark:hover:bg-blue-900/20 transition-all group"
                        >
                          <div class="flex items-start justify-between gap-3">
                            <div class="flex-1">
                              <p class="font-semibold text-slate-900 dark:text-white group-hover:text-blue-600 dark:group-hover:text-blue-400 transition-colors">
                                {{ supplier.name }}
                              </p>
                              <div class="flex flex-wrap gap-1 mt-1">
                                <span 
                                  v-for="product in supplier.products.slice(0, 3)" 
                                  :key="product"
                                  class="text-xs px-2 py-0.5 bg-slate-100 dark:bg-slate-700 text-slate-600 dark:text-slate-300 rounded-full"
                                >
                                  {{ product }}
                                </span>
            </div>
            </div>
                            <div class="flex items-center gap-1 flex-shrink-0">
                              <StarIcon class="w-4 h-4 text-yellow-400" />
                              <span class="text-sm font-medium text-slate-700 dark:text-slate-300">{{ supplier.rating }}</span>
              </div>
            </div>
                        </button>
          </div>
                    </div>
                  </Transition>

                  <!-- No results message -->
                  <div 
                    v-if="showSupplierDropdown && filteredSuppliers.length === 0 && supplierSearchQuery"
                    class="absolute z-10 w-full mt-2 bg-white dark:bg-slate-800 rounded-xl shadow-2xl border-2 border-slate-200 dark:border-slate-700 p-4"
                  >
                    <p class="text-center text-slate-600 dark:text-slate-400">No suppliers found in this category. Try a different search term.</p>
                  </div>

                  <!-- Suppliers count hint -->
                  <p v-if="!selectedSupplier && !supplierSearchQuery" class="text-xs text-slate-500 dark:text-slate-400 mt-2">
                    💡 {{ categorySuppliers.length }} suppliers available in {{ getCategoryName(newGroupBuy.category) }}
                  </p>
                </div>
              </Transition>

                <!-- Product Search - THIRD STEP (shown after supplier selected) -->
                <Transition name="slide-down">
                  <div v-if="selectedSupplier" class="relative">
                    <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                      <ShoppingCartIcon class="w-4 h-4 inline mr-1 text-blue-600" />
                      Step 3: Search Products from {{ selectedSupplier.name }} *
                    </label>
                    <div class="relative">
                      <input
                        v-model="productSearchQuery"
                        @input="handleProductInput"
                        @focus="showProductDropdown = true"
                        @blur="setTimeout(() => showProductDropdown = false, 200)"
                        type="text"
                        placeholder="Search products or services..."
                        class="w-full px-4 py-3 pr-10 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all"
                        :class="{ 'border-green-500': selectedProduct }"
                      />
                      <div class="absolute right-3 top-1/2 -translate-y-1/2">
                        <button
                          v-if="selectedProduct"
                          @click.prevent="clearProduct"
                          type="button"
                          class="text-green-600 hover:text-green-700"
                        >
                          <XMarkIcon class="w-5 h-5" />
            </button>
                        <MagnifyingGlassIcon v-else class="w-5 h-5 text-slate-400" />
          </div>
        </div>

                    <!-- Selected Product Card -->
                    <div v-if="selectedProduct" class="mt-2 bg-gradient-to-r from-green-50 to-blue-50 dark:from-green-900/20 dark:to-blue-900/20 rounded-xl p-4 border-2 border-green-200 dark:border-green-800">
                      <div class="flex items-start gap-3">
                        <div class="p-2 bg-green-500 rounded-lg">
                          <CheckCircleIcon class="w-6 h-6 text-white" />
            </div>
                        <div class="flex-1">
                          <h4 class="font-bold text-slate-900 dark:text-white mb-1">{{ selectedProduct.name }}</h4>
                          <p class="text-sm text-slate-600 dark:text-slate-400 mb-2">{{ selectedProduct.description }}</p>
                          <div class="grid grid-cols-3 gap-2 text-xs">
                            <div class="bg-white dark:bg-slate-800 rounded-lg px-2 py-1">
                              <span class="text-slate-500">Price</span>
                              <p class="font-bold text-blue-600">R{{ selectedProduct.unitPrice }}</p>
            </div>
                            <div class="bg-white dark:bg-slate-800 rounded-lg px-2 py-1">
                              <span class="text-slate-500">Unit</span>
                              <p class="font-bold text-slate-900 dark:text-white">{{ selectedProduct.unit }}</p>
          </div>
                            <div class="bg-white dark:bg-slate-800 rounded-lg px-2 py-1">
                              <span class="text-slate-500">Min Order</span>
                              <p class="font-bold text-slate-900 dark:text-white">{{ selectedProduct.minOrder }}</p>
            </div>
            </div>
            </div>
            </div>
          </div>

                    <!-- Product Dropdown -->
                    <Transition name="dropdown">
                      <div 
                        v-if="showProductDropdown && filteredProducts.length > 0" 
                        class="absolute z-10 w-full mt-2 bg-white dark:bg-slate-800 rounded-xl shadow-2xl border-2 border-slate-200 dark:border-slate-700 max-h-96 overflow-y-auto"
                      >
                        <div class="p-2">
                          <button
                            v-for="product in filteredProducts"
                            :key="product.id"
                            @click.prevent="selectProduct(product)"
                            type="button"
                            class="w-full text-left px-4 py-3 rounded-lg hover:bg-blue-50 dark:hover:bg-blue-900/20 transition-all group border-b border-slate-100 dark:border-slate-700 last:border-0"
                          >
                            <div class="flex items-start justify-between gap-3">
                              <div class="flex-1">
                                <p class="font-semibold text-slate-900 dark:text-white group-hover:text-blue-600 dark:group-hover:text-blue-400 transition-colors">
                                  {{ product.name }}
                                </p>
                                <p class="text-xs text-slate-600 dark:text-slate-400 mt-1">{{ product.description }}</p>
                                <div class="flex gap-3 mt-2 text-xs">
                                  <span class="text-blue-600 font-medium">R{{ product.unitPrice }}/{{ product.unit }}</span>
                                  <span class="text-slate-500">Min: {{ product.minOrder }} {{ product.unit }}s</span>
            </div>
          </div>
                              <div class="flex-shrink-0 text-right">
                                <div class="px-2 py-1 bg-blue-100 dark:bg-blue-900/30 text-blue-700 dark:text-blue-400 rounded-lg text-xs font-bold">
                                  R{{ product.unitPrice }}
        </div>
      </div>
    </div>
            </button>
          </div>
                      </div>
                    </Transition>

                    <!-- No products message -->
                    <div 
                      v-if="showProductDropdown && filteredProducts.length === 0 && productSearchQuery"
                      class="absolute z-10 w-full mt-2 bg-white dark:bg-slate-800 rounded-xl shadow-2xl border-2 border-slate-200 dark:border-slate-700 p-4"
                    >
                      <p class="text-center text-slate-600 dark:text-slate-400">No products found. Try a different search term.</p>
                    </div>

                    <!-- Browse all products link -->
                    <p v-if="!selectedProduct && !productSearchQuery" class="text-xs text-slate-500 dark:text-slate-400 mt-2">
                      💡 Start typing to search from {{ availableProducts.length }} available products
                    </p>
                  </div>
                </Transition>

                <!-- Product/Service Name -->
              <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                    Product or Service Name *
                  </label>
                <input 
                  v-model="newGroupBuy.title"
                  type="text" 
                  required
                  placeholder="e.g., Office Supplies Bulk Purchase"
                    class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all"
                    :disabled="!!selectedProduct"
                  />
                  <p v-if="selectedProduct" class="text-xs text-blue-600 dark:text-blue-400 mt-1">
                    ✓ Auto-filled from selected product
                  </p>
            </div>

                <!-- Description -->
            <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                    Description *
                  </label>
              <textarea 
                v-model="newGroupBuy.description"
                required
                    rows="3"
                    placeholder="Describe what you're buying and why others should join"
                    class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all"
                    :disabled="!!selectedProduct"
              ></textarea>
                  <p v-if="selectedProduct" class="text-xs text-blue-600 dark:text-blue-400 mt-1">
                    ✓ Auto-filled from selected product
                  </p>
            </div>

                <!-- Min Quantity and Price -->
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                    <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                      Minimum Quantity *
                    </label>
                  <input 
                      v-model.number="newGroupBuy.minQuantity"
                    type="number" 
                    required
                      min="2"
                      placeholder="e.g., 100"
                      class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all"
                  />
                </div>
                <div>
                    <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                      Target Price per Unit *
                    </label>
                    <div class="relative">
                      <span class="absolute left-4 top-1/2 -translate-y-1/2 text-slate-500">R</span>
                  <input 
                        v-model.number="newGroupBuy.targetPrice"
                    type="number" 
                        required
                        min="0"
                    step="0.01"
                        placeholder="0.00"
                        class="w-full pl-8 pr-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all"
                  />
                </div>
                  </div>
                </div>

                <!-- Commitment and Deadline -->
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                    <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                      Your Commitment (Units) *
                    </label>
                    <input
                      v-model.number="newGroupBuy.myCommitment"
                      type="number"
                      required
                      min="1"
                      placeholder="How many units do you need?"
                      class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all"
                    />
                  </div>
                  <div>
                    <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                      Deadline *
                    </label>
                  <input 
                    v-model="newGroupBuy.deadline"
                    type="date" 
                    required
                      :min="new Date().toISOString().split('T')[0]"
                      class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all"
                  />
                </div>
              </div>

                <!-- Payment Terms -->
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                    Payment Terms
                  </label>
                  <select
                    v-model="newGroupBuy.paymentTerms"
                    class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all"
                  >
                    <option value="upfront">Pay Upfront</option>
                    <option value="on-delivery">Pay on Delivery</option>
                    <option value="net30">Net 30</option>
                  </select>
            </div>

                <!-- Benefits Info -->
                <div class="bg-gradient-to-r from-blue-50 to-green-50 dark:from-blue-900/20 dark:to-green-900/20 rounded-xl p-4 border-2 border-blue-200 dark:border-blue-800">
                  <div class="flex items-start gap-3">
                    <div class="p-2 bg-blue-500 rounded-lg">
                      <SparklesIcon class="w-5 h-5 text-white" />
                </div>
                    <div>
                      <h4 class="font-bold text-slate-900 dark:text-white mb-1">Estimated Benefits</h4>
                      <p class="text-sm text-slate-600 dark:text-slate-400">
                        Group buying can save 15-30% on average. The more participants, the better the price!
                      </p>
                    </div>
                  </div>
                </div>

                <!-- Actions -->
                <div class="flex gap-3 pt-4">
                  <button
                    type="button"
                    @click="showCreateModal = false"
                    class="flex-1 px-6 py-3 border-2 border-slate-300 dark:border-slate-600 rounded-xl text-slate-700 dark:text-slate-300 font-semibold hover:bg-slate-50 dark:hover:bg-slate-700 transition-all"
                  >
                    Cancel
                  </button>
                  <button
                    type="submit"
                    class="flex-1 px-6 py-3 bg-gradient-to-r from-green-600 to-blue-600 text-white rounded-xl font-semibold hover:from-green-700 hover:to-blue-700 shadow-lg hover:shadow-xl transition-all"
                  >
                    Create Group Buy
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </Transition>

    <!-- View Details Modal -->
    <Transition name="modal">
      <div v-if="selectedGroup" class="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 overflow-y-auto">
        <div class="flex min-h-full items-center justify-center p-4">
          <div class="relative bg-white dark:bg-slate-800 rounded-2xl shadow-2xl border border-slate-200 dark:border-slate-700 w-full max-w-4xl">
            
            <!-- Header -->
            <div class="bg-gradient-to-r from-blue-600 to-purple-600 px-6 py-4 flex items-center justify-between rounded-t-2xl">
              <div>
                <h3 class="text-xl font-bold text-white">{{ selectedGroup.title }}</h3>
                <p class="text-sm text-white/80">Group Buy Details</p>
              </div>
              <button @click="selectedGroup = null" class="p-2 hover:bg-white/20 rounded-lg transition-colors">
                <XMarkIcon class="w-6 h-6 text-white" />
              </button>
            </div>

            <!-- Content -->
            <div class="p-6 max-h-[70vh] overflow-y-auto space-y-6">
              
              <!-- Description -->
              <div>
                <h4 class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-2">Description</h4>
                <p class="text-slate-900 dark:text-white">{{ selectedGroup.description }}</p>
              </div>

              <!-- Progress -->
              <div>
                <h4 class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-3">Commitment Progress</h4>
                <div class="bg-slate-50 dark:bg-slate-900 rounded-xl p-4">
                  <div class="flex justify-between mb-2">
                    <span class="font-medium text-slate-900 dark:text-white">
                      {{ selectedGroup.committed || 0 }} / {{ selectedGroup.minQuantity }} units
                    </span>
                    <span class="font-bold text-blue-600">
                      {{ Math.min(Math.round(((selectedGroup.committed || 0) / selectedGroup.minQuantity) * 100), 100) }}%
                    </span>
                  </div>
                  <div class="w-full bg-slate-200 rounded-full h-3 dark:bg-slate-700">
                    <div 
                      class="bg-gradient-to-r from-blue-600 to-purple-600 h-3 rounded-full transition-all duration-300" 
                      :style="{ width: Math.min(((selectedGroup.committed || 0) / selectedGroup.minQuantity * 100), 100) + '%' }"
                    ></div>
                  </div>
                </div>
              </div>

              <!-- Stats Grid -->
              <div class="grid grid-cols-2 md:grid-cols-4 gap-4">
                <div class="bg-gradient-to-br from-blue-50 to-blue-100 dark:from-blue-900/20 dark:to-blue-900/30 rounded-xl p-4">
                  <p class="text-xs text-blue-600 dark:text-blue-400 mb-1">Members</p>
                  <p class="text-2xl font-bold text-slate-900 dark:text-white">{{ selectedGroup.memberCount }}</p>
                </div>
                <div class="bg-gradient-to-br from-green-50 to-green-100 dark:from-green-900/20 dark:to-green-900/30 rounded-xl p-4">
                  <p class="text-xs text-green-600 dark:text-green-400 mb-1">Savings</p>
                  <p class="text-2xl font-bold text-slate-900 dark:text-white">
                    {{ selectedGroup.estimatedSavings ? 'R' + selectedGroup.estimatedSavings : selectedGroup.savingsPercentage + '%' }}
                  </p>
                </div>
                <div class="bg-gradient-to-br from-orange-50 to-orange-100 dark:from-orange-900/20 dark:to-orange-900/30 rounded-xl p-4">
                  <p class="text-xs text-orange-600 dark:text-orange-400 mb-1">Min Qty</p>
                  <p class="text-2xl font-bold text-slate-900 dark:text-white">
                    {{ selectedGroup.minQuantity || selectedGroup.minCommitment }}
                  </p>
                </div>
                <div class="bg-gradient-to-br from-purple-50 to-purple-100 dark:from-purple-900/20 dark:to-purple-900/30 rounded-xl p-4">
                  <p class="text-xs text-purple-600 dark:text-purple-400 mb-1">Trust</p>
                  <div class="flex items-center gap-1">
                    <StarIcon v-for="i in 5" :key="i" class="w-4 h-4" 
                      :class="i <= (selectedGroup.trustLevel || selectedGroup.leadTrustScore) ? 'text-yellow-400' : 'text-slate-300'" 
                    />
                </div>
                </div>
              </div>

              <!-- Lead Organization (if available) -->
              <div v-if="selectedGroup.leadOrganization" class="bg-slate-50 dark:bg-slate-900 rounded-xl p-4">
                <h4 class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-3">Lead Organization</h4>
                <div class="flex items-center gap-3">
                  <div class="h-12 w-12 rounded-full bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center">
                    <span class="text-lg font-bold text-white">{{ selectedGroup.leadOrganization.charAt(0) }}</span>
                  </div>
                  <div>
                    <p class="font-semibold text-slate-900 dark:text-white">{{ selectedGroup.leadOrganization }}</p>
                    <div class="flex items-center gap-1">
                      <StarIcon v-for="i in 5" :key="i" class="w-3 h-3" 
                        :class="i <= selectedGroup.leadTrustScore ? 'text-yellow-400' : 'text-slate-300'" 
                      />
                </div>
                  </div>
                </div>
              </div>

              <!-- Timeline -->
              <div>
                <h4 class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-3">Timeline</h4>
                <div class="flex items-center gap-2 text-slate-900 dark:text-white">
                  <ClockIcon class="w-5 h-5 text-blue-600" />
                  <span>Deadline: <strong>{{ formatDate(selectedGroup.deadline) }}</strong></span>
                </div>
              </div>
            </div>

            <!-- Footer Actions -->
            <div class="border-t border-slate-200 dark:border-slate-700 px-6 py-4 bg-slate-50 dark:bg-slate-900/50 flex gap-3">
              <button
                v-if="!selectedGroup.leadOrganization"
                @click="inviteToGroup(selectedGroup)"
                class="flex-1 px-6 py-3 border-2 border-blue-600 text-blue-600 rounded-xl font-semibold hover:bg-blue-50 dark:hover:bg-blue-900/20 transition-all"
              >
                <ShareIcon class="w-5 h-5 inline mr-2" />
                Invite Members
              </button>
              <button
                v-if="selectedGroup.leadOrganization"
                @click="joinSelectedGroup"
                class="flex-1 px-6 py-3 bg-gradient-to-r from-green-600 to-blue-600 text-white rounded-xl font-semibold hover:from-green-700 hover:to-blue-700 shadow-lg hover:shadow-xl transition-all"
              >
                Join This Group
              </button>
              <button
                @click="selectedGroup = null"
                class="px-6 py-3 border-2 border-slate-300 dark:border-slate-600 rounded-xl text-slate-700 dark:text-slate-300 font-semibold hover:bg-slate-50 dark:hover:bg-slate-700 transition-all"
              >
                Close
              </button>
            </div>
          </div>
        </div>
      </div>
    </Transition>

    <!-- Invite Members Modal -->
    <Transition name="modal">
      <div v-if="showInviteModal" class="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 overflow-y-auto">
        <div class="flex min-h-full items-center justify-center p-4">
          <div class="relative bg-white dark:bg-slate-800 rounded-2xl shadow-2xl border border-slate-200 dark:border-slate-700 w-full max-w-2xl">
            
            <!-- Header -->
            <div class="bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-4 flex items-center justify-between rounded-t-2xl">
              <div>
                <h3 class="text-xl font-bold text-white">Invite Members</h3>
                <p class="text-sm text-white/80">{{ inviteGroup?.title }}</p>
              </div>
              <button @click="showInviteModal = false" class="p-2 hover:bg-white/20 rounded-lg transition-colors">
                <XMarkIcon class="w-6 h-6 text-white" />
              </button>
            </div>

            <!-- Content -->
            <div class="p-6 space-y-6">
              
              <!-- Share Link -->
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                  Share Link
                </label>
                <div class="flex gap-2">
                  <input 
                    :value="shareLink"
                    readonly
                    class="flex-1 px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-slate-50 dark:bg-slate-900 text-slate-900 dark:text-white"
                  />
                  <button
                    @click="copyShareLink"
                    class="px-6 py-3 bg-blue-600 text-white rounded-xl font-semibold hover:bg-blue-700 transition-all"
                  >
                    <DocumentDuplicateIcon class="w-5 h-5" />
                  </button>
                </div>
                <p v-if="linkCopied" class="text-sm text-green-600 mt-2">✓ Link copied to clipboard!</p>
              </div>

              <!-- Share via WhatsApp -->
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                  Quick Share
                </label>
                <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
                  <a
                    :href="getWhatsAppShareLink()"
                    target="_blank"
                    class="flex items-center justify-center gap-2 px-4 py-3 bg-green-500 text-white rounded-xl font-semibold hover:bg-green-600 transition-all"
                  >
                    <ChatBubbleLeftRightIcon class="w-5 h-5" />
                    Share on WhatsApp
                  </a>
                  <button
                    @click="shareViaEmail"
                    class="flex items-center justify-center gap-2 px-4 py-3 bg-blue-500 text-white rounded-xl font-semibold hover:bg-blue-600 transition-all"
                  >
                    <EnvelopeIcon class="w-5 h-5" />
                    Share via Email
                  </button>
              </div>
            </div>
            
              <!-- Invite by Phone -->
              <div>
                <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                  Invite by Phone Number
                </label>
                <div class="flex gap-2">
                  <input
                    v-model="invitePhone"
                    type="tel"
                    placeholder="+27 XX XXX XXXX"
                    class="flex-1 px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all"
                  />
              <button 
                    @click="sendInvite"
                    class="px-6 py-3 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-xl font-semibold hover:from-purple-700 hover:to-blue-700 transition-all"
              >
                    Send
              </button>
                </div>
              </div>

              <!-- Info Box -->
              <div class="bg-blue-50 dark:bg-blue-900/20 rounded-xl p-4 border-2 border-blue-200 dark:border-blue-800">
                <div class="flex items-start gap-3">
                  <InformationCircleIcon class="w-5 h-5 text-blue-600 flex-shrink-0 mt-0.5" />
                  <div class="text-sm text-slate-700 dark:text-slate-300">
                    <p class="font-semibold mb-1">How invites work:</p>
                    <ul class="list-disc list-inside space-y-1 text-slate-600 dark:text-slate-400">
                      <li>Members you invite can join instantly</li>
                      <li>They'll see the current progress and benefits</li>
                      <li>More members = better pricing for everyone</li>
                    </ul>
                  </div>
                </div>
              </div>
            </div>

            <!-- Footer -->
            <div class="border-t border-slate-200 dark:border-slate-700 px-6 py-4 bg-slate-50 dark:bg-slate-900/50">
              <button 
                @click="showInviteModal = false"
                class="w-full px-6 py-3 border-2 border-slate-300 dark:border-slate-600 rounded-xl text-slate-700 dark:text-slate-300 font-semibold hover:bg-slate-50 dark:hover:bg-slate-700 transition-all"
              >
                Done
              </button>
            </div>
        </div>
      </div>
    </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import {
  PlusIcon,
  UserGroupIcon,
  ShoppingCartIcon,
  CurrencyDollarIcon,
  StarIcon,
  XMarkIcon,
  SparklesIcon,
  ClockIcon,
  ShareIcon,
  DocumentDuplicateIcon,
  ChatBubbleLeftRightIcon,
  EnvelopeIcon,
  InformationCircleIcon,
  MagnifyingGlassIcon,
  CheckCircleIcon,
  ArrowPathIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Group Buying - TOSS ERP',
  meta: [
    { name: 'description', content: 'Collaborative procurement and group buying platform for TOSS network members' }
  ]
})

// Composables
const toast = useToast()

// State
const showCreateModal = ref(false)
const selectedGroup = ref<any>(null)
const showInviteModal = ref(false)
const inviteGroup = ref<any>(null)
const invitePhone = ref('')
const linkCopied = ref(false)

// New Group Buy Form
const newGroupBuy = ref({
  title: '',
  description: '',
  minQuantity: 100,
  targetPrice: 0,
  myCommitment: 1,
  deadline: '',
  category: '',
  paymentTerms: 'upfront',
  supplierId: null,
  supplierName: ''
})

// Supplier search
const supplierSearchQuery = ref('')
const showSupplierDropdown = ref(false)
const selectedSupplier = ref<any>(null)

// Product search
const productSearchQuery = ref('')
const showProductDropdown = ref(false)
const selectedProduct = ref<any>(null)

// Mock suppliers data with detailed product catalogs
const allSuppliers = ref([
  { 
    id: 1, 
    name: 'Office Pro Suppliers', 
    category: 'supplies', 
    rating: 4.5, 
    products: ['Office Supplies', 'Stationery', 'Furniture'],
    catalog: [
      { id: 101, name: 'A4 Copy Paper - 5 Reams', description: 'Premium quality white paper, 80gsm', unitPrice: 285, unit: 'box', minOrder: 10 },
      { id: 102, name: 'Ballpoint Pens - Box of 50', description: 'Blue ink, smooth writing', unitPrice: 95, unit: 'box', minOrder: 5 },
      { id: 103, name: 'Office Desk Chair', description: 'Ergonomic with lumbar support', unitPrice: 1850, unit: 'unit', minOrder: 3 },
      { id: 104, name: 'Stapler Set - Heavy Duty', description: 'Includes staples and remover', unitPrice: 145, unit: 'set', minOrder: 10 },
      { id: 105, name: 'File Folders - Pack of 100', description: 'Manila folders, legal size', unitPrice: 220, unit: 'pack', minOrder: 5 }
    ]
  },
  { 
    id: 2, 
    name: 'Tech Solutions SA', 
    category: 'technology', 
    rating: 4.8, 
    products: ['Computers', 'Software', 'IT Equipment'],
    catalog: [
      { id: 201, name: 'Dell Optiplex Desktop PC', description: 'i5 processor, 8GB RAM, 256GB SSD', unitPrice: 8500, unit: 'unit', minOrder: 2 },
      { id: 202, name: 'HP LaserJet Printer', description: 'Network ready, duplex printing', unitPrice: 4200, unit: 'unit', minOrder: 2 },
      { id: 203, name: 'Microsoft Office 365 License', description: 'Annual subscription per user', unitPrice: 950, unit: 'license', minOrder: 5 },
      { id: 204, name: 'Wireless Mouse & Keyboard Set', description: 'Ergonomic design, long battery life', unitPrice: 385, unit: 'set', minOrder: 10 },
      { id: 205, name: '24" LED Monitor', description: 'Full HD, HDMI & VGA inputs', unitPrice: 2100, unit: 'unit', minOrder: 3 }
    ]
  },
  { 
    id: 3, 
    name: 'Food & Beverage Wholesalers', 
    category: 'food', 
    rating: 4.2, 
    products: ['Food', 'Beverages', 'Snacks'],
    catalog: [
      { id: 301, name: 'Instant Coffee - 500g Jar', description: 'Premium blend, aromatic', unitPrice: 125, unit: 'jar', minOrder: 20 },
      { id: 302, name: 'Sugar Sachets - Box of 1000', description: 'White sugar, 5g sachets', unitPrice: 85, unit: 'box', minOrder: 15 },
      { id: 303, name: 'Tea Bags - Box of 200', description: 'Black tea, premium quality', unitPrice: 95, unit: 'box', minOrder: 20 },
      { id: 304, name: 'Bottled Water - 24 x 500ml', description: 'Purified spring water', unitPrice: 65, unit: 'case', minOrder: 30 },
      { id: 305, name: 'Assorted Biscuits Pack', description: 'Mixed variety, 2kg pack', unitPrice: 145, unit: 'pack', minOrder: 15 }
    ]
  },
  { 
    id: 4, 
    name: 'Cleaning Masters', 
    category: 'cleaning', 
    rating: 4.6, 
    products: ['Cleaning Supplies', 'Chemicals', 'Equipment'],
    catalog: [
      { id: 401, name: 'Industrial Floor Cleaner - 5L', description: 'Multi-surface, fresh scent', unitPrice: 185, unit: 'bottle', minOrder: 10 },
      { id: 402, name: 'Microfiber Cleaning Cloths - 50 Pack', description: 'Reusable, lint-free', unitPrice: 245, unit: 'pack', minOrder: 5 },
      { id: 403, name: 'Disinfectant Spray - 750ml', description: 'Kills 99.9% germs', unitPrice: 65, unit: 'bottle', minOrder: 20 },
      { id: 404, name: 'Mop & Bucket Set', description: 'Heavy duty with wringer', unitPrice: 295, unit: 'set', minOrder: 5 },
      { id: 405, name: 'Garbage Bags - Roll of 100', description: 'Extra strong, 75L capacity', unitPrice: 125, unit: 'roll', minOrder: 10 }
    ]
  },
  { 
    id: 5, 
    name: 'Industrial Equipment Co', 
    category: 'equipment', 
    rating: 4.4, 
    products: ['Industrial Equipment', 'Machinery', 'Tools'],
    catalog: [
      { id: 501, name: 'Electric Drill Set', description: 'Cordless, 20V with bits', unitPrice: 1450, unit: 'set', minOrder: 3 },
      { id: 502, name: 'Tool Cabinet - 7 Drawer', description: 'Heavy duty steel construction', unitPrice: 3200, unit: 'unit', minOrder: 2 },
      { id: 503, name: 'Safety Gloves - Box of 100 pairs', description: 'Cut-resistant, multiple sizes', unitPrice: 385, unit: 'box', minOrder: 5 },
      { id: 504, name: 'Industrial Shelving Unit', description: '2m high, 5 shelf capacity', unitPrice: 1850, unit: 'unit', minOrder: 3 },
      { id: 505, name: 'Hand Tool Set - 120 Pieces', description: 'Complete workshop set', unitPrice: 2400, unit: 'set', minOrder: 2 }
    ]
  },
  { 
    id: 6, 
    name: 'Fresh Produce Direct', 
    category: 'food', 
    rating: 4.7, 
    products: ['Fresh Produce', 'Vegetables', 'Fruits'],
    catalog: [
      { id: 601, name: 'Fresh Tomatoes - 10kg Box', description: 'Grade A, locally sourced', unitPrice: 85, unit: 'box', minOrder: 20 },
      { id: 602, name: 'Onions - 10kg Bag', description: 'Medium size, premium quality', unitPrice: 65, unit: 'bag', minOrder: 20 },
      { id: 603, name: 'Potatoes - 10kg Bag', description: 'Washed, ready to cook', unitPrice: 75, unit: 'bag', minOrder: 20 },
      { id: 604, name: 'Mixed Salad Greens - 2kg', description: 'Fresh daily, pre-washed', unitPrice: 95, unit: 'pack', minOrder: 15 },
      { id: 605, name: 'Apples - 12kg Box', description: 'Golden Delicious, crisp & sweet', unitPrice: 145, unit: 'box', minOrder: 15 }
    ]
  },
  { 
    id: 7, 
    name: 'Stationery World', 
    category: 'supplies', 
    rating: 4.3, 
    products: ['Stationery', 'Office Supplies', 'Paper Products'],
    catalog: [
      { id: 701, name: 'Notebook - A4 Ruled, Pack of 10', description: '200 pages each, hardcover', unitPrice: 185, unit: 'pack', minOrder: 10 },
      { id: 702, name: 'Highlighters - Set of 6 Colors', description: 'Chisel tip, vibrant colors', unitPrice: 45, unit: 'set', minOrder: 20 },
      { id: 703, name: 'Sticky Notes - Multi Pack', description: '12 pads, assorted colors', unitPrice: 85, unit: 'pack', minOrder: 15 },
      { id: 704, name: 'Printer Cartridge - Black', description: 'HP compatible, high yield', unitPrice: 385, unit: 'unit', minOrder: 5 },
      { id: 705, name: 'Binder Clips - Assorted Sizes, 100 Pack', description: 'Steel construction, durable', unitPrice: 65, unit: 'pack', minOrder: 15 }
    ]
  },
  { 
    id: 8, 
    name: 'Tech Hardware Hub', 
    category: 'technology', 
    rating: 4.9, 
    products: ['Hardware', 'Peripherals', 'Accessories'],
    catalog: [
      { id: 801, name: 'USB Flash Drive - 64GB', description: 'USB 3.0, high speed transfer', unitPrice: 185, unit: 'unit', minOrder: 10 },
      { id: 802, name: 'HDMI Cable - 2m', description: '4K compatible, gold plated', unitPrice: 95, unit: 'unit', minOrder: 15 },
      { id: 803, name: 'Laptop Bag - Padded', description: 'Fits up to 15.6" laptop', unitPrice: 385, unit: 'unit', minOrder: 5 },
      { id: 804, name: 'Webcam - Full HD 1080p', description: 'Auto-focus, built-in mic', unitPrice: 650, unit: 'unit', minOrder: 5 },
      { id: 805, name: 'Power Bank - 20000mAh', description: 'Fast charging, dual USB ports', unitPrice: 485, unit: 'unit', minOrder: 8 }
    ]
  }
])

// Filter suppliers by selected category
const categorySuppliers = computed(() => {
  if (!newGroupBuy.value.category) return []
  return allSuppliers.value.filter((supplier: any) => 
    supplier.category === newGroupBuy.value.category || newGroupBuy.value.category === 'other'
  )
})

const filteredSuppliers = computed(() => {
  // First filter by category
  let suppliers = categorySuppliers.value
  
  // Then filter by search query if provided
  if (supplierSearchQuery.value) {
    const query = supplierSearchQuery.value.toLowerCase()
    suppliers = suppliers.filter((supplier: any) => 
      supplier.name.toLowerCase().includes(query) ||
      supplier.products.some((p: string) => p.toLowerCase().includes(query))
    )
  }
  
  return suppliers
})

// Product filtering based on selected supplier
const availableProducts = computed(() => {
  if (!selectedSupplier.value || !selectedSupplier.value.catalog) return []
  return selectedSupplier.value.catalog
})

const filteredProducts = computed(() => {
  if (!productSearchQuery.value) return availableProducts.value
  
  const query = productSearchQuery.value.toLowerCase()
  return availableProducts.value.filter((product: any) => 
    product.name.toLowerCase().includes(query) ||
    product.description.toLowerCase().includes(query)
  )
})

// Network Statistics
const networkStats = ref({
  totalMembers: 1247,
  activeGroupBuys: 23,
  totalSavings: 485,
  trustScore: 4.3
})

// Mock data
const myGroupBuys = ref<any[]>([])

// Load active group buys from localStorage
onMounted(() => {
  loadActiveGroupBuys()
})

const loadActiveGroupBuys = () => {
  try {
    const savedGroupBuys = localStorage.getItem('toss-active-group-buys')
    if (savedGroupBuys) {
      const groupBuys = JSON.parse(savedGroupBuys)
      
      // Filter out expired group buys and format the data
      myGroupBuys.value = groupBuys
        .filter((gb: any) => new Date(gb.deadline) > new Date())
        .map((gb: any) => ({
          id: gb.id,
          title: gb.title,
          description: gb.description,
          itemName: gb.itemName || 'Various Items',
          itemSku: gb.itemSku || 'N/A',
          myQuantity: gb.myQuantity || 0,
          status: gb.status || 'collecting',
          committed: gb.committed,
          minQuantity: gb.minQuantity,
          memberCount: gb.memberCount,
          estimatedSavings: gb.estimatedSavings,
          deadline: new Date(gb.deadline),
          joinedAt: new Date(gb.joinedAt),
          savingsPercentage: gb.savingsPercentage || 15,
          trustLevel: 5
        }))
      
      // Update network stats
      networkStats.value.activeGroupBuys = myGroupBuys.value.length
      networkStats.value.totalSavings = Math.round(
        myGroupBuys.value.reduce((sum: number, gb: any) => sum + gb.estimatedSavings, 0) / 1000
      )
    }
  } catch (error) {
    console.error('Error loading active group buys:', error)
    // Fallback to mock data
    myGroupBuys.value = [
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
      }
    ]
  }
}

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
    title: 'Food Supplies - Bulk Order',
    description: 'Fresh produce and packaged goods for retail',
    leadOrganization: 'Fresh Market Co-op',
    leadTrustScore: 5,
    memberCount: 15,
    savingsPercentage: 25,
    minCommitment: 1,
    deadline: new Date('2025-10-01')
  }
])

// Methods
const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    'collecting': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400',
    'negotiating': 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400',
    'confirmed': 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    'completed': 'bg-slate-100 text-slate-800 dark:bg-slate-900/30 dark:text-slate-400'
  }
  return classes[status] || 'bg-slate-100 text-slate-800'
}

const formatDate = (date: Date) => {
  return date.toLocaleDateString('en-US', { 
    month: 'short', 
    day: 'numeric',
    year: 'numeric'
  })
}

// Category helper methods
const getCategoryName = (category: string) => {
  const categoryNames: Record<string, string> = {
    'food': 'Food & Beverages',
    'supplies': 'Office Supplies',
    'equipment': 'Equipment & Tools',
    'technology': 'Technology',
    'cleaning': 'Cleaning Supplies',
    'other': 'Other'
  }
  return categoryNames[category] || category
}

const handleCategoryChange = () => {
  // Reset supplier and product when category changes
  clearSupplier()
}

// Supplier selection methods
const selectSupplier = (supplier: any) => {
  selectedSupplier.value = supplier
  supplierSearchQuery.value = supplier.name
  newGroupBuy.value.supplierId = supplier.id
  newGroupBuy.value.supplierName = supplier.name
  
  showSupplierDropdown.value = false
  
  // Reset product selection when supplier changes
  clearProduct()
}

const clearSupplier = () => {
  selectedSupplier.value = null
  supplierSearchQuery.value = ''
  newGroupBuy.value.supplierId = null
  newGroupBuy.value.supplierName = ''
  clearProduct()
}

const handleSupplierInput = () => {
  showSupplierDropdown.value = true
  // Clear selection if user starts typing
  if (selectedSupplier.value && supplierSearchQuery.value !== selectedSupplier.value.name) {
    selectedSupplier.value = null
    newGroupBuy.value.supplierId = null
    newGroupBuy.value.supplierName = ''
    clearProduct()
  }
}

// Product selection methods
const selectProduct = (product: any) => {
  selectedProduct.value = product
  productSearchQuery.value = product.name
  
  // Auto-fill form fields based on product
  newGroupBuy.value.title = product.name
  newGroupBuy.value.description = `${product.description} - ${selectedSupplier.value.name}`
  newGroupBuy.value.targetPrice = product.unitPrice
  newGroupBuy.value.minQuantity = product.minOrder
  newGroupBuy.value.myCommitment = product.minOrder
  
  showProductDropdown.value = false
  
  toast.success('Product details auto-filled!')
}

const clearProduct = () => {
  selectedProduct.value = null
  productSearchQuery.value = ''
}

const handleProductInput = () => {
  showProductDropdown.value = true
  // Clear selection if user starts typing
  if (selectedProduct.value && productSearchQuery.value !== selectedProduct.value.name) {
    selectedProduct.value = null
  }
}

// Methods
const createGroupBuy = () => {
  // Validate form
  if (!newGroupBuy.value.title || !newGroupBuy.value.description || !newGroupBuy.value.deadline) {
    toast.error('Please fill in all required fields')
    return
  }
  
  if (!selectedSupplier.value) {
    toast.error('Please select a supplier')
    return
  }

  // Create new group buy
  const newGroup = {
    id: myGroupBuys.value.length + availableGroupBuys.value.length + 1,
    title: newGroupBuy.value.title,
    description: newGroupBuy.value.description,
    status: 'collecting',
    committed: newGroupBuy.value.myCommitment,
    minQuantity: newGroupBuy.value.minQuantity,
    memberCount: 1,
    estimatedSavings: Math.round(newGroupBuy.value.targetPrice * newGroupBuy.value.myCommitment * 0.2), // 20% estimated savings
    deadline: new Date(newGroupBuy.value.deadline),
    trustLevel: 4,
    category: newGroupBuy.value.category,
    paymentTerms: newGroupBuy.value.paymentTerms
  }

  // Add to my group buys
  myGroupBuys.value.unshift(newGroup)
  
  // Update network stats
  networkStats.value.activeGroupBuys++

  // Show success message
  toast.success('Group buy created successfully! Share it with others to reach your target.')
  
  // Reset form and close modal
  newGroupBuy.value = {
    title: '',
    description: '',
    minQuantity: 100,
    targetPrice: 0,
    myCommitment: 1,
    deadline: '',
    category: '',
    paymentTerms: 'upfront',
    supplierId: null,
    supplierName: ''
  }
  clearSupplier()
  showCreateModal.value = false
}

const viewGroupDetails = (group: any) => {
  selectedGroup.value = group
}

const inviteMembers = (group: any) => {
  inviteGroup.value = group
  showInviteModal.value = true
}

const inviteToGroup = (group: any) => {
  inviteGroup.value = group
  selectedGroup.value = null
  showInviteModal.value = true
}

const joinGroup = (group: any) => {
  // Add commitment prompt (in a real app, this would be a modal)
  const commitment = prompt(`How many units would you like to commit? (Min: ${group.minCommitment || 1})`)
  
  if (!commitment || parseInt(commitment) < (group.minCommitment || 1)) {
    toast.warning('Please enter a valid commitment amount')
    return
  }

  // Move from available to my groups
  const index = availableGroupBuys.value.findIndex((g: any) => g.id === group.id)
  if (index !== -1) {
    const joinedGroup = {
      ...group,
      status: 'collecting',
      committed: group.committed ? group.committed + parseInt(commitment) : parseInt(commitment),
      minQuantity: group.minQuantity || group.minCommitment * 10,
      memberCount: group.memberCount + 1,
      estimatedSavings: Math.round(parseInt(commitment) * 50 * (group.savingsPercentage / 100)),
      trustLevel: group.leadTrustScore
    }
    
    myGroupBuys.value.unshift(joinedGroup)
    availableGroupBuys.value.splice(index, 1)
    
    toast.success(`Successfully joined "${group.title}"! You committed ${commitment} units.`)
  }
}

const joinSelectedGroup = () => {
  if (selectedGroup.value) {
    const group = selectedGroup.value
    selectedGroup.value = null
    joinGroup(group)
  }
}

// Share functionality
const shareLink = computed(() => {
  if (!inviteGroup.value) return ''
  return `${window.location.origin}/buying/group-buying?join=${inviteGroup.value.id}`
})

const copyShareLink = async () => {
  try {
    await navigator.clipboard.writeText(shareLink.value)
    linkCopied.value = true
    toast.success('Link copied to clipboard!')
    setTimeout(() => {
      linkCopied.value = false
    }, 3000)
  } catch (err) {
    toast.error('Failed to copy link')
  }
}

const getWhatsAppShareLink = () => {
  if (!inviteGroup.value) return '#'
  const message = `Join our group buy for ${inviteGroup.value.title}! Save ${inviteGroup.value.savingsPercentage || 20}% by buying together. ${shareLink.value}`
  return `https://wa.me/?text=${encodeURIComponent(message)}`
}

const shareViaEmail = () => {
  if (!inviteGroup.value) return
  const subject = `Join Group Buy: ${inviteGroup.value.title}`
  const body = `I'm organizing a group buy for ${inviteGroup.value.title}.\n\nWe can save ${inviteGroup.value.savingsPercentage || 20}% by buying together!\n\nJoin here: ${shareLink.value}`
  window.location.href = `mailto:?subject=${encodeURIComponent(subject)}&body=${encodeURIComponent(body)}`
}

const sendInvite = () => {
  if (!invitePhone.value) {
    toast.warning('Please enter a phone number')
    return
  }

  // In a real app, this would send an SMS via API
  toast.success(`Invitation sent to ${invitePhone.value}!`)
  invitePhone.value = ''
}
</script>

<style scoped>
/* Modal transition animations */
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-active > div,
.modal-leave-active > div {
  transition: transform 0.3s ease, opacity 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-from > div,
.modal-leave-to > div {
  transform: scale(0.95);
  opacity: 0;
}

/* Dropdown transition animations */
.dropdown-enter-active,
.dropdown-leave-active {
  transition: all 0.2s ease;
}

.dropdown-enter-from,
.dropdown-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}

/* Slide down transition for product search */
.slide-down-enter-active,
.slide-down-leave-active {
  transition: all 0.3s ease;
}

.slide-down-enter-from,
.slide-down-leave-to {
  opacity: 0;
  transform: translateY(-20px);
  max-height: 0;
}

.slide-down-enter-to,
.slide-down-leave-from {
  opacity: 1;
  transform: translateY(0);
  max-height: 500px;
}
</style>

