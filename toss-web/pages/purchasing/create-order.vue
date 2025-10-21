<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800 pb-24">
    <!-- Page Header with Glass Morphism -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-20">
      <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 sm:py-6">
        <div class="flex items-center justify-between">
          <div class="flex items-center space-x-3">
            <NuxtLink 
              to="/purchasing" 
              class="p-2 hover:bg-slate-100 dark:hover:bg-slate-700 rounded-lg transition-colors"
            >
              <ArrowLeftIcon class="w-5 h-5 text-slate-600 dark:text-slate-400" />
            </NuxtLink>
            <div class="flex-1 min-w-0">
              <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">
                Create Order
              </h1>
              <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
                AI-powered ordering for your business
              </p>
            </div>
          </div>
          <div class="flex items-center space-x-2">
            <button
              v-if="orderItems.length > 0"
              @click="viewCart = !viewCart"
              class="relative p-2 sm:px-4 sm:py-2 bg-gradient-to-r from-blue-600 to-purple-600 text-white rounded-xl hover:from-blue-700 hover:to-purple-700 shadow-lg transition-all duration-200 transform hover:scale-105"
            >
              <ShoppingCartIcon class="w-5 h-5" />
              <span class="absolute -top-2 -right-2 bg-red-500 text-white text-xs font-bold rounded-full w-6 h-6 flex items-center justify-center">
                {{ orderItems.length }}
              </span>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      
      <!-- AI Insight Banner -->
      <div class="bg-gradient-to-r from-purple-500 via-pink-500 to-red-500 rounded-2xl shadow-2xl p-6 text-white mb-8 relative overflow-hidden">
        <div class="absolute top-0 right-0 w-64 h-64 bg-white/10 rounded-full -mr-32 -mt-32"></div>
        <div class="relative z-10 flex items-start">
          <div class="p-3 bg-white/20 backdrop-blur-sm rounded-xl mr-4 flex-shrink-0">
            <SparklesIcon class="w-6 h-6" />
          </div>
          <div class="flex-1 min-w-0">
            <h3 class="text-lg font-bold mb-1">AI Recommendations</h3>
            <p class="text-white/90 text-sm">
              We've analyzed your stock levels and sales patterns. <strong>{{ lowStockItems.length }}</strong> items need immediate attention, 
              and <strong>{{ focusedItems.length }}</strong> items are trending up.
            </p>
          </div>
        </div>
      </div>

      <!-- Smart Purchase Options -->
      <Transition name="slide-down">
        <div v-if="smartAnalysis && orderItems.length > 0" class="bg-gradient-to-br from-blue-50 to-purple-50 dark:from-blue-900/20 dark:to-purple-900/20 rounded-2xl border-2 border-blue-200 dark:border-blue-800 p-6 mb-8 shadow-lg">
          <div class="flex items-start gap-3 mb-4">
            <div class="p-2 bg-gradient-to-r from-blue-500 to-purple-600 rounded-lg">
              <SparklesIcon class="w-6 h-6 text-white" />
            </div>
            <div class="flex-1">
              <h3 class="text-xl font-bold text-slate-900 dark:text-white mb-1">
                💡 Smart Purchase Options Available!
              </h3>
              <p class="text-sm text-slate-600 dark:text-slate-400">
                Save up to R{{ smartAnalysis.estimatedTotalSavings }} with intelligent ordering
              </p>
            </div>
          </div>

          <!-- Option 1: Join Active Group Buy -->
          <div v-if="smartAnalysis.hasActiveGroupBuy && smartAnalysis.groupBuyOpportunity" 
               class="bg-white dark:bg-slate-800 rounded-xl p-4 mb-3 border-2 border-green-500 hover:shadow-md transition-all cursor-pointer"
               @click="handleJoinGroupBuy">
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <div class="flex items-center gap-2 mb-2">
                  <UserGroupIcon class="w-5 h-5 text-green-600" />
                  <h4 class="font-bold text-slate-900 dark:text-white">Join Active Group Buy</h4>
                  <span class="px-2 py-0.5 bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-400 text-xs rounded-full font-bold">
                    RECOMMENDED
                  </span>
                </div>
                <p class="text-sm text-slate-600 dark:text-slate-400 mb-2">
                  {{ smartAnalysis.groupBuyOpportunity.title }}
                </p>
                <div class="flex flex-wrap gap-3 text-xs">
                  <span class="flex items-center gap-1 text-green-600 font-medium">
                    <ArrowDownIcon class="w-3 h-3" />
                    Save {{ smartAnalysis.groupBuyOpportunity.savingsPercentage }}%
                  </span>
                  <span class="text-slate-600 dark:text-slate-400">
                    {{ smartAnalysis.groupBuyOpportunity.currentParticipants }} members
                  </span>
                  <span class="text-slate-600 dark:text-slate-400">
                    {{ smartAnalysis.groupBuyOpportunity.currentQuantity }}/{{ smartAnalysis.groupBuyOpportunity.targetQuantity }} units
                  </span>
                  <span class="text-orange-600">
                    Ends {{ formatDeadline(smartAnalysis.groupBuyOpportunity.deadline) }}
                  </span>
                </div>
              </div>
              <button class="px-4 py-2 bg-gradient-to-r from-green-600 to-green-700 text-white rounded-lg hover:from-green-700 hover:to-green-800 shadow-md font-bold text-sm">
                Join Now
              </button>
            </div>
          </div>

          <!-- Option 2: Auto-Aggregation Available -->
          <div v-else-if="smartAnalysis.canAggregate && smartAnalysis.aggregationOpportunity" 
               class="bg-white dark:bg-slate-800 rounded-xl p-4 mb-3 border-2 border-blue-500 hover:shadow-md transition-all">
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <div class="flex items-center gap-2 mb-2">
                  <BoltIcon class="w-5 h-5 text-blue-600" />
                  <h4 class="font-bold text-slate-900 dark:text-white">Auto-Aggregation Active</h4>
                  <span class="px-2 py-0.5 bg-blue-100 dark:bg-blue-900/30 text-blue-700 dark:text-blue-400 text-xs rounded-full font-bold">
                    AUTOMATIC
                  </span>
                </div>
                <p class="text-sm text-slate-600 dark:text-slate-400 mb-2">
                  Your order will be combined with {{ smartAnalysis.aggregationOpportunity.participatingOrders.length }} other shops
                </p>
                <div class="flex flex-wrap gap-3 text-xs">
                  <span class="flex items-center gap-1 text-blue-600 font-medium">
                    <ArrowDownIcon class="w-3 h-3" />
                    Save R{{ smartAnalysis.aggregationOpportunity.estimatedSavings }}
                  </span>
                  <span class="text-slate-600 dark:text-slate-400">
                    Total: {{ smartAnalysis.aggregationOpportunity.totalQuantity }} units
                  </span>
                  <span class="text-slate-600 dark:text-slate-400">
                    {{ smartAnalysis.aggregationOpportunity.savingsPercentage }}% bulk discount
                  </span>
                </div>
              </div>
              <div class="flex items-center gap-2">
                <CheckCircleIcon class="w-6 h-6 text-blue-600" />
                <span class="text-sm font-bold text-blue-600">Active</span>
              </div>
            </div>
          </div>

          <!-- Option 3: Create Group Buy -->
          <div v-else-if="smartAnalysis.shouldCreateGroupBuy && smartAnalysis.groupBuyPotential" 
               class="bg-white dark:bg-slate-800 rounded-xl p-4 border-2 border-purple-500 hover:shadow-md transition-all cursor-pointer"
               @click="handleCreateGroupBuy">
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <div class="flex items-center gap-2 mb-2">
                  <PlusCircleIcon class="w-5 h-5 text-purple-600" />
                  <h4 class="font-bold text-slate-900 dark:text-white">Create Group Buy</h4>
                  <span class="px-2 py-0.5 bg-purple-100 dark:bg-purple-900/30 text-purple-700 dark:text-purple-400 text-xs rounded-full font-bold">
                    OPPORTUNITY
                  </span>
                </div>
                <p class="text-sm text-slate-600 dark:text-slate-400 mb-2">
                  Invite others to join and unlock bulk pricing
                </p>
                <div class="flex flex-wrap gap-3 text-xs">
                  <span class="flex items-center gap-1 text-purple-600 font-medium">
                    <ArrowDownIcon class="w-3 h-3" />
                    Potential {{ smartAnalysis.groupBuyPotential.savingsPercentage }}% savings
                  </span>
                  <span class="text-slate-600 dark:text-slate-400">
                    Est. R{{ smartAnalysis.groupBuyPotential.estimatedSavings }} saved
                  </span>
                </div>
              </div>
              <button class="px-4 py-2 bg-gradient-to-r from-purple-600 to-purple-700 text-white rounded-lg hover:from-purple-700 hover:to-purple-800 shadow-md font-bold text-sm">
                Create
              </button>
            </div>
          </div>

          <!-- Participating Shops (for aggregation) -->
          <div v-if="smartAnalysis.canAggregate && smartAnalysis.aggregationOpportunity && smartAnalysis.aggregationOpportunity.participatingOrders.length > 0" 
               class="mt-3 pt-3 border-t border-slate-200 dark:border-slate-600">
            <p class="text-xs text-slate-500 dark:text-slate-400 mb-2">Aggregating with:</p>
            <div class="flex flex-wrap gap-2">
              <div v-for="shop in smartAnalysis.aggregationOpportunity.participatingOrders" :key="shop.shopId"
                   class="flex items-center gap-1 px-2 py-1 bg-slate-100 dark:bg-slate-700 rounded-lg text-xs">
                <BuildingStorefrontIcon class="w-3 h-3 text-slate-500" />
                <span class="font-medium">{{ shop.shopName }}</span>
                <span class="text-slate-500">({{ shop.quantity }} units)</span>
              </div>
            </div>
          </div>
        </div>
      </Transition>

      <!-- Section Tabs -->
      <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 mb-6 overflow-hidden">
        <div class="flex border-b border-slate-200 dark:border-slate-700">
          <button
            v-for="tab in tabs"
            :key="tab.id"
            @click="activeTab = tab.id"
            :class="[
              'flex-1 px-4 sm:px-6 py-4 text-sm sm:text-base font-semibold transition-all duration-200',
              activeTab === tab.id
                ? 'bg-gradient-to-r from-blue-600 to-purple-600 text-white'
                : 'text-slate-600 dark:text-slate-400 hover:bg-slate-50 dark:hover:bg-slate-700/50'
            ]"
          >
            <div class="flex items-center justify-center space-x-2">
              <component :is="tab.icon" class="w-5 h-5" />
              <span class="hidden sm:inline">{{ tab.label }}</span>
              <span v-if="tab.badge" class="ml-2 px-2 py-0.5 bg-red-500 text-white text-xs rounded-full">
                {{ tab.badge }}
              </span>
            </div>
          </button>
        </div>

        <!-- Tab Content -->
        <div class="p-4 sm:p-6">
          
          <!-- Low Stock Items Tab -->
          <div v-if="activeTab === 'low-stock'" class="space-y-4">
            <div class="flex items-center justify-between mb-4">
              <div>
                <h3 class="text-lg font-bold text-slate-900 dark:text-white flex items-center">
                  <ExclamationTriangleIcon class="w-5 h-5 text-orange-500 mr-2" />
                  Low Stock Items (Priority)
                </h3>
                <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">
                  These items need immediate restocking to avoid stockouts
                </p>
              </div>
            </div>

            <div v-if="lowStockItems.length === 0" class="text-center py-12">
              <div class="w-20 h-20 bg-green-100 dark:bg-green-900/20 rounded-full flex items-center justify-center mx-auto mb-4">
                <CheckCircleIcon class="w-10 h-10 text-green-600 dark:text-green-400" />
              </div>
              <p class="text-lg font-medium text-slate-900 dark:text-white">All stock levels are healthy!</p>
              <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">No items currently below minimum stock level</p>
            </div>

            <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
              <div
                v-for="item in lowStockItems"
                :key="item.id"
                class="bg-gradient-to-br from-orange-50 to-red-50 dark:from-orange-900/20 dark:to-red-900/20 rounded-xl border-2 border-orange-200 dark:border-orange-800 p-5 hover:shadow-lg transition-all duration-200 transform hover:-translate-y-1"
              >
                <div class="flex items-start justify-between mb-3">
                  <div class="flex-1 min-w-0">
                    <h4 class="font-bold text-slate-900 dark:text-white text-lg truncate">{{ item.name }}</h4>
                    <p class="text-sm text-slate-600 dark:text-slate-400">{{ item.sku }}</p>
                  </div>
                  <div class="flex-shrink-0 ml-2">
                    <span class="px-2 py-1 bg-red-500 text-white text-xs font-bold rounded-full">
                      {{ item.currentStock }}/{{ item.minStock }}
                    </span>
                  </div>
                </div>

                <div class="mb-3">
                  <div class="flex justify-between text-sm mb-1">
                    <span class="text-slate-600 dark:text-slate-400">Stock Level</span>
                    <span class="font-medium text-red-600">{{ Math.round((item.currentStock / item.minStock) * 100) }}%</span>
                  </div>
                  <div class="w-full bg-slate-200 dark:bg-slate-700 rounded-full h-2">
                    <div 
                      class="bg-gradient-to-r from-orange-500 to-red-600 h-2 rounded-full transition-all duration-500"
                      :style="{ width: Math.min((item.currentStock / item.minStock) * 100, 100) + '%' }"
                    ></div>
                  </div>
                </div>

                <div class="flex items-center justify-between text-sm mb-3">
                  <span class="text-slate-600 dark:text-slate-400">Suggested Qty:</span>
                  <span class="font-bold text-slate-900 dark:text-white">{{ item.suggestedQty }} units</span>
                </div>

                <div class="flex items-center justify-between">
                  <span class="text-lg font-bold text-slate-900 dark:text-white">R{{ item.price.toFixed(2) }}</span>
                  <button
                    @click="addToCart(item, item.suggestedQty)"
                    class="px-4 py-2 bg-gradient-to-r from-orange-600 to-red-600 text-white rounded-lg font-semibold hover:from-orange-700 hover:to-red-700 transition-all duration-200 transform hover:scale-105 flex items-center space-x-1"
                  >
                    <PlusIcon class="w-4 h-4" />
                    <span>Add</span>
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Focused Items Tab -->
          <div v-if="activeTab === 'focused'" class="space-y-4">
            <div class="flex items-center justify-between mb-4">
              <div>
                <h3 class="text-lg font-bold text-slate-900 dark:text-white flex items-center">
                  <ChartBarIcon class="w-5 h-5 text-blue-500 mr-2" />
                  Predicted High-Demand Items
                </h3>
                <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">
                  Based on sales trends and AI predictions, these items are expected to sell more
                </p>
              </div>
            </div>

            <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
              <div
                v-for="item in focusedItems"
                :key="item.id"
                class="bg-gradient-to-br from-blue-50 to-purple-50 dark:from-blue-900/20 dark:to-purple-900/20 rounded-xl border-2 border-blue-200 dark:border-blue-800 p-5 hover:shadow-lg transition-all duration-200 transform hover:-translate-y-1"
              >
                <div class="flex items-start justify-between mb-3">
                  <div class="flex-1 min-w-0">
                    <h4 class="font-bold text-slate-900 dark:text-white text-lg truncate">{{ item.name }}</h4>
                    <p class="text-sm text-slate-600 dark:text-slate-400">{{ item.sku }}</p>
                  </div>
                  <div class="flex-shrink-0 ml-2">
                    <span class="px-2 py-1 bg-blue-500 text-white text-xs font-bold rounded-full flex items-center">
                      <ArrowTrendingUpIcon class="w-3 h-3 mr-1" />
                      {{ item.trend }}%
                    </span>
                  </div>
                </div>

                <div class="mb-3 bg-white/50 dark:bg-slate-800/50 rounded-lg p-3">
                  <div class="flex justify-between text-xs mb-1">
                    <span class="text-slate-600 dark:text-slate-400">Current Stock:</span>
                    <span class="font-medium text-slate-900 dark:text-white">{{ item.currentStock }} units</span>
                  </div>
                  <div class="flex justify-between text-xs mb-1">
                    <span class="text-slate-600 dark:text-slate-400">Avg Daily Sales:</span>
                    <span class="font-medium text-blue-600">{{ item.avgDailySales }} units</span>
                  </div>
                  <div class="flex justify-between text-xs">
                    <span class="text-slate-600 dark:text-slate-400">Predicted Demand:</span>
                    <span class="font-medium text-purple-600">{{ item.predictedDemand }} units</span>
                  </div>
                </div>

                <div class="flex items-center justify-between text-sm mb-3">
                  <span class="text-slate-600 dark:text-slate-400">Suggested Qty:</span>
                  <span class="font-bold text-slate-900 dark:text-white">{{ item.suggestedQty }} units</span>
                </div>

                <div class="flex items-center justify-between">
                  <span class="text-lg font-bold text-slate-900 dark:text-white">R{{ item.price.toFixed(2) }}</span>
                  <button
                    @click="addToCart(item, item.suggestedQty)"
                    class="px-4 py-2 bg-gradient-to-r from-blue-600 to-purple-600 text-white rounded-lg font-semibold hover:from-blue-700 hover:to-purple-700 transition-all duration-200 transform hover:scale-105 flex items-center space-x-1"
                  >
                    <PlusIcon class="w-4 h-4" />
                    <span>Add</span>
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- Search & Order Tab -->
          <div v-if="activeTab === 'search'" class="space-y-4">
            <div class="mb-4">
              <h3 class="text-lg font-bold text-slate-900 dark:text-white flex items-center mb-2">
                <MagnifyingGlassIcon class="w-5 h-5 text-green-500 mr-2" />
                Search & Order Anything
              </h3>
              <p class="text-sm text-slate-600 dark:text-slate-400 mb-4">
                Browse or search for any item in the catalog
              </p>

              <!-- Search Bar -->
              <div class="relative">
                <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                  <MagnifyingGlassIcon class="h-5 w-5 text-slate-400" />
                </div>
                <input
                  v-model="searchQuery"
                  type="text"
                  placeholder="Search by name, SKU, or category..."
                  class="block w-full pl-12 pr-4 py-3 border-2 border-slate-200 dark:border-slate-700 rounded-xl text-slate-900 dark:text-white dark:bg-slate-800 focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                />
              </div>

              <!-- Category Filters -->
              <div class="flex flex-wrap gap-2 mt-4">
                <button
                  v-for="category in categories"
                  :key="category"
                  @click="selectedCategory = selectedCategory === category ? null : category"
                  :class="[
                    'px-4 py-2 rounded-lg font-medium text-sm transition-all duration-200',
                    selectedCategory === category
                      ? 'bg-gradient-to-r from-green-600 to-emerald-600 text-white shadow-lg'
                      : 'bg-slate-100 dark:bg-slate-700 text-slate-700 dark:text-slate-300 hover:bg-slate-200 dark:hover:bg-slate-600'
                  ]"
                >
                  {{ category }}
                </button>
              </div>
            </div>

            <!-- Search Results -->
            <div v-if="filteredSearchItems.length === 0" class="text-center py-12">
              <div class="w-20 h-20 bg-slate-100 dark:bg-slate-800 rounded-full flex items-center justify-center mx-auto mb-4">
                <MagnifyingGlassIcon class="w-10 h-10 text-slate-400" />
              </div>
              <p class="text-lg font-medium text-slate-900 dark:text-white">No items found</p>
              <p class="text-sm text-slate-600 dark:text-slate-400 mt-1">Try adjusting your search or filters</p>
            </div>

            <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
              <div
                v-for="item in filteredSearchItems"
                :key="item.id"
                class="bg-white dark:bg-slate-800 rounded-xl border-2 border-slate-200 dark:border-slate-700 p-5 hover:shadow-lg transition-all duration-200 transform hover:-translate-y-1 hover:border-green-500"
              >
                <div class="flex items-start justify-between mb-3">
                  <div class="flex-1 min-w-0">
                    <h4 class="font-bold text-slate-900 dark:text-white text-lg truncate">{{ item.name }}</h4>
                    <p class="text-sm text-slate-600 dark:text-slate-400">{{ item.sku }}</p>
                  </div>
                  <span class="px-2 py-1 bg-slate-100 dark:bg-slate-700 text-slate-700 dark:text-slate-300 text-xs font-medium rounded">
                    {{ item.category }}
                  </span>
                </div>

                <div class="mb-3">
                  <div class="flex justify-between text-sm mb-1">
                    <span class="text-slate-600 dark:text-slate-400">In Stock:</span>
                    <span class="font-medium text-slate-900 dark:text-white">{{ item.currentStock }} units</span>
                  </div>
                </div>

                <div class="mb-3">
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Quantity</label>
                  <input
                    v-model.number="item.orderQty"
                    type="number"
                    min="1"
                    class="block w-full px-3 py-2 border-2 border-slate-200 dark:border-slate-700 rounded-lg text-slate-900 dark:text-white dark:bg-slate-800 focus:ring-2 focus:ring-green-500 focus:border-transparent"
                  />
                </div>

                <div class="flex items-center justify-between">
                  <span class="text-lg font-bold text-slate-900 dark:text-white">R{{ item.price.toFixed(2) }}</span>
                  <button
                    @click="addToCart(item, item.orderQty || 1)"
                    class="px-4 py-2 bg-gradient-to-r from-green-600 to-emerald-600 text-white rounded-lg font-semibold hover:from-green-700 hover:to-emerald-700 transition-all duration-200 transform hover:scale-105 flex items-center space-x-1"
                  >
                    <PlusIcon class="w-4 h-4" />
                    <span>Add</span>
                  </button>
                </div>
              </div>
            </div>
          </div>

        </div>
      </div>
    </div>

    <!-- Floating Cart Button (Mobile) -->
    <div v-if="orderItems.length > 0 && !viewCart" class="fixed bottom-20 right-4 z-30 sm:hidden">
      <button
        @click="viewCart = true"
        class="relative p-4 bg-gradient-to-r from-blue-600 to-purple-600 text-white rounded-full shadow-2xl hover:from-blue-700 hover:to-purple-700 transition-all duration-200 transform hover:scale-110"
      >
        <ShoppingCartIcon class="w-6 h-6" />
        <span class="absolute -top-2 -right-2 bg-red-500 text-white text-xs font-bold rounded-full w-6 h-6 flex items-center justify-center">
          {{ orderItems.length }}
        </span>
      </button>
    </div>

    <!-- Cart Sidebar/Modal -->
    <Transition name="slide">
      <div v-if="viewCart" class="fixed inset-0 z-50 overflow-hidden">
        <div class="absolute inset-0 bg-black/50 backdrop-blur-sm" @click="viewCart = false"></div>
        
        <div class="absolute right-0 top-0 bottom-0 w-full sm:w-96 bg-white dark:bg-slate-800 shadow-2xl flex flex-col">
          <!-- Cart Header -->
          <div class="bg-gradient-to-r from-blue-600 to-purple-600 text-white p-6">
            <div class="flex items-center justify-between mb-2">
              <h2 class="text-2xl font-bold">Your Order</h2>
              <button @click="viewCart = false" class="p-2 hover:bg-white/20 rounded-lg transition-colors">
                <XMarkIcon class="w-6 h-6" />
              </button>
            </div>
            <p class="text-white/80 text-sm">{{ orderItems.length }} item(s)</p>
          </div>

          <!-- Cart Items -->
          <div class="flex-1 overflow-y-auto p-6 space-y-4">
            <div v-if="orderItems.length === 0" class="text-center py-12">
              <ShoppingCartIcon class="w-16 h-16 text-slate-300 dark:text-slate-600 mx-auto mb-4" />
              <p class="text-slate-600 dark:text-slate-400">Your cart is empty</p>
            </div>

            <div
              v-for="(item, index) in orderItems"
              :key="index"
              class="bg-slate-50 dark:bg-slate-900 rounded-xl p-4 border-2"
              :class="item.groupBuyEnabled ? 'border-purple-500 dark:border-purple-600' : 'border-transparent'"
            >
              <div class="flex items-start justify-between mb-2">
                <div class="flex-1 min-w-0">
                  <h4 class="font-bold text-slate-900 dark:text-white truncate">{{ item.name }}</h4>
                  <p class="text-sm text-slate-600 dark:text-slate-400">{{ item.sku }}</p>
                </div>
                <button
                  @click="removeFromCart(index)"
                  class="p-1 hover:bg-red-100 dark:hover:bg-red-900/30 rounded transition-colors"
                >
                  <XMarkIcon class="w-5 h-5 text-red-600" />
                </button>
              </div>

              <!-- Group Buy Toggle -->
              <div class="mb-3 p-2 bg-white dark:bg-slate-800 rounded-lg">
                <div class="flex items-center justify-between">
                  <div class="flex items-center gap-2">
                    <UserGroupIcon class="w-4 h-4 text-purple-600" />
                    <span class="text-sm font-semibold text-slate-900 dark:text-white">Group Buy</span>
                  </div>
                  <button
                    @click="toggleGroupBuy(index)"
                    class="relative inline-flex h-6 w-11 items-center rounded-full transition-colors"
                    :class="item.groupBuyEnabled ? 'bg-purple-600' : 'bg-slate-300 dark:bg-slate-600'"
                  >
                    <span
                      class="inline-block h-4 w-4 transform rounded-full bg-white transition-transform"
                      :class="item.groupBuyEnabled ? 'translate-x-6' : 'translate-x-1'"
                    ></span>
                  </button>
                </div>
                
                <!-- Group Buy Details (shown when enabled) -->
                <Transition name="slide-down">
                  <div v-if="item.groupBuyEnabled && item.groupBuyDetails" class="mt-2 pt-2 border-t border-slate-200 dark:border-slate-700 space-y-1">
                    <div class="flex items-center justify-between text-xs">
                      <span class="text-slate-600 dark:text-slate-400">💰 Savings:</span>
                      <span class="font-bold text-green-600">R{{ item.groupBuyDetails.savings }}</span>
                    </div>
                    <div class="flex items-center justify-between text-xs">
                      <span class="text-slate-600 dark:text-slate-400">⏰ Ends:</span>
                      <span class="font-medium text-orange-600">{{ item.groupBuyDetails.endsIn }}</span>
                    </div>
                    <div class="flex items-center justify-between text-xs">
                      <span class="text-slate-600 dark:text-slate-400">👥 Progress:</span>
                      <span class="font-medium text-blue-600">{{ item.groupBuyDetails.progress }}</span>
                    </div>
                    <div class="mt-1 pt-1 border-t border-slate-200 dark:border-slate-700">
                      <p class="text-xs text-purple-600 dark:text-purple-400 font-medium">
                        {{ item.groupBuyDetails.status }}
                      </p>
                    </div>
                  </div>
                </Transition>
              </div>

              <div class="flex items-center justify-between">
                <div class="flex items-center space-x-3">
                  <button
                    @click="updateQuantity(index, item.quantity - 1)"
                    class="w-8 h-8 bg-slate-200 dark:bg-slate-700 rounded-lg flex items-center justify-center hover:bg-slate-300 dark:hover:bg-slate-600 transition-colors"
                  >
                    <MinusIcon class="w-4 h-4" />
                  </button>
                  <span class="font-bold text-slate-900 dark:text-white">{{ item.quantity }}</span>
                  <button
                    @click="updateQuantity(index, item.quantity + 1)"
                    class="w-8 h-8 bg-slate-200 dark:bg-slate-700 rounded-lg flex items-center justify-center hover:bg-slate-300 dark:hover:bg-slate-600 transition-colors"
                  >
                    <PlusIcon class="w-4 h-4" />
                  </button>
                </div>
                <div class="text-right">
                  <span class="text-lg font-bold text-slate-900 dark:text-white">
                    R{{ (item.price * item.quantity).toFixed(2) }}
                  </span>
                  <p v-if="item.groupBuyEnabled && item.groupBuyDetails" class="text-xs text-green-600 font-medium">
                    Save R{{ item.groupBuyDetails.savings }}
                  </p>
                </div>
              </div>
            </div>
          </div>

          <!-- Cart Footer -->
          <div class="border-t border-slate-200 dark:border-slate-700 p-6 space-y-4">
            <!-- Group Buy Savings Summary -->
            <div v-if="totalGroupBuySavings > 0" class="bg-gradient-to-r from-purple-50 to-green-50 dark:from-purple-900/20 dark:to-green-900/20 rounded-xl p-3 border-2 border-purple-200 dark:border-purple-800">
              <div class="flex items-center justify-between">
                <div class="flex items-center gap-2">
                  <UserGroupIcon class="w-5 h-5 text-purple-600" />
                  <span class="text-sm font-bold text-purple-900 dark:text-purple-100">Group Buy Savings</span>
                </div>
                <span class="text-lg font-bold text-green-600">-R{{ totalGroupBuySavings.toFixed(2) }}</span>
              </div>
              <p class="text-xs text-purple-700 dark:text-purple-300 mt-1">
                💡 {{ activeGroupBuyItemsCount }} items in group buys
              </p>
            </div>

            <div class="space-y-2">
              <div class="flex justify-between text-sm">
                <span class="text-slate-600 dark:text-slate-400">Subtotal:</span>
                <span class="font-medium text-slate-900 dark:text-white">R{{ subtotal.toFixed(2) }}</span>
              </div>
              <div v-if="totalGroupBuySavings > 0" class="flex justify-between text-sm">
                <span class="text-green-600 dark:text-green-400">Group Buy Savings:</span>
                <span class="font-bold text-green-600">-R{{ totalGroupBuySavings.toFixed(2) }}</span>
              </div>
              <div class="flex justify-between text-sm">
                <span class="text-slate-600 dark:text-slate-400">Delivery:</span>
                <span class="font-medium text-slate-900 dark:text-white">R{{ deliveryFee.toFixed(2) }}</span>
              </div>
              <div class="flex justify-between text-lg font-bold pt-2 border-t border-slate-200 dark:border-slate-700">
                <span class="text-slate-900 dark:text-white">Total:</span>
                <span class="text-blue-600">R{{ (totalAmount - totalGroupBuySavings).toFixed(2) }}</span>
              </div>
              <p v-if="totalGroupBuySavings > 0" class="text-xs text-green-600 dark:text-green-400 font-medium text-center">
                🎉 You're saving R{{ totalGroupBuySavings.toFixed(2) }} with group buying!
              </p>
            </div>

            <button
              @click="placeOrder"
              :disabled="orderItems.length === 0"
              class="w-full px-6 py-4 bg-gradient-to-r from-blue-600 to-purple-600 text-white rounded-xl font-bold text-lg shadow-lg hover:from-blue-700 hover:to-purple-700 transition-all disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center space-x-2"
            >
              <CheckCircleIcon class="w-6 h-6" />
              <span>Place Order</span>
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useToast } from '~/composables/useToast'
import { useSmartPurchasing } from '~/composables/useSmartPurchasing'
import type { SmartPurchaseAnalysis } from '~/types/smart-purchasing'
import {
  ShoppingCartIcon,
  PlusIcon,
  MinusIcon,
  XMarkIcon,
  ArrowLeftIcon,
  ExclamationTriangleIcon,
  ChartBarIcon,
  MagnifyingGlassIcon,
  SparklesIcon,
  CheckCircleIcon,
  ArrowTrendingUpIcon,
  CubeIcon,
  UserGroupIcon,
  BoltIcon,
  PlusCircleIcon,
  ArrowDownIcon,
  BuildingStorefrontIcon
} from '@heroicons/vue/24/outline'

// Type definitions
interface BaseItem {
  id: string
  name: string
  sku: string
  price: number
  category: string
  currentStock: number
}

interface LowStockItem extends BaseItem {
  minStock: number
  suggestedQty: number
}

interface FocusedItem extends BaseItem {
  avgDailySales: number
  predictedDemand: number
  trend: number
  suggestedQty: number
}

interface SearchItem extends BaseItem {
  orderQty: number
  avgDailySales?: number
  predictedDemand?: number
  trend?: number
  suggestedQty?: number
  minStock?: number
}

interface CartItem extends BaseItem {
  quantity: number
  groupBuyEnabled?: boolean
  groupBuyDetails?: {
    savings: number
    endsIn: string
    progress: string
    status: string
    groupBuyId?: string
    isNew?: boolean
  }
}

// Page metadata
useHead({
  title: 'Create Order - TOSS ERP',
  meta: [
    { name: 'description', content: 'AI-powered order creation with smart recommendations' }
  ]
})

const router = useRouter()
const toast = useToast()
const { analyzeCartForOpportunities, joinGroupBuy, createAggregatedOrder, createGroupBuyFromOrder } = useSmartPurchasing()

// State
const activeTab = ref('low-stock')
const viewCart = ref(false)
const orderItems = ref<CartItem[]>([])
const searchQuery = ref('')
const selectedCategory = ref<string | null>(null)

// Smart Purchasing State
const smartAnalysis = ref<SmartPurchaseAnalysis | null>(null)
const isAnalyzing = ref(false)

// Watch cart changes and analyze for smart purchasing opportunities
watch(orderItems, async (newItems: CartItem[]) => {
  if (newItems.length > 0) {
    isAnalyzing.value = true
    try {
      const items = newItems.map((item: CartItem) => ({
        id: item.id,
        sku: item.sku,
        name: item.name,
        quantity: item.quantity,
        unitPrice: item.price,
        totalPrice: item.quantity * item.price
      }))
      
      smartAnalysis.value = await analyzeCartForOpportunities(items)
    } catch (error) {
      console.error('Error analyzing cart:', error)
      smartAnalysis.value = null
    } finally {
      isAnalyzing.value = false
    }
  } else {
    smartAnalysis.value = null
  }
}, { deep: true })

// Tabs configuration
const tabs = computed(() => [
  { 
    id: 'low-stock', 
    label: 'Low Stock', 
    icon: ExclamationTriangleIcon,
    badge: lowStockItems.value.length
  },
  { 
    id: 'focused', 
    label: 'Trending', 
    icon: ChartBarIcon,
    badge: null
  },
  { 
    id: 'search', 
    label: 'Search All', 
    icon: MagnifyingGlassIcon,
    badge: null
  }
])

// Mock data - Low Stock Items (Priority)
const lowStockItems = ref<LowStockItem[]>([
  {
    id: 'LS001',
    name: 'White Bread',
    sku: 'BRD-WHT-001',
    currentStock: 5,
    minStock: 50,
    suggestedQty: 100,
    price: 12.50,
    category: 'Bakery'
  },
  {
    id: 'LS002',
    name: 'Full Cream Milk',
    sku: 'MLK-FC-1L',
    currentStock: 8,
    minStock: 30,
    suggestedQty: 60,
    price: 18.99,
    category: 'Dairy'
  },
  {
    id: 'LS003',
    name: 'Coca-Cola 2L',
    sku: 'BEV-COK-2L',
    currentStock: 12,
    minStock: 48,
    suggestedQty: 72,
    price: 22.50,
    category: 'Beverages'
  },
  {
    id: 'LS004',
    name: 'Maize Meal 10kg',
    sku: 'GRN-MZE-10K',
    currentStock: 3,
    minStock: 20,
    suggestedQty: 40,
    price: 85.00,
    category: 'Grains'
  }
])

// Mock data - Focused Items (AI Predictions)
const focusedItems = ref<FocusedItem[]>([
  {
    id: 'FC001',
    name: 'Brown Bread',
    sku: 'BRD-BRN-001',
    currentStock: 45,
    avgDailySales: 15,
    predictedDemand: 25,
    trend: 42,
    suggestedQty: 50,
    price: 14.00,
    category: 'Bakery'
  },
  {
    id: 'FC002',
    name: 'Cooking Oil 750ml',
    sku: 'OIL-COK-750',
    currentStock: 32,
    avgDailySales: 8,
    predictedDemand: 15,
    trend: 38,
    suggestedQty: 40,
    price: 28.99,
    category: 'Cooking'
  },
  {
    id: 'FC003',
    name: 'Sugar 2.5kg',
    sku: 'SGR-WHT-2.5',
    currentStock: 28,
    avgDailySales: 6,
    predictedDemand: 12,
    trend: 35,
    suggestedQty: 30,
    price: 35.50,
    category: 'Pantry'
  },
  {
    id: 'FC004',
    name: 'Eggs (Dozen)',
    sku: 'EGG-LRG-12',
    currentStock: 40,
    avgDailySales: 10,
    predictedDemand: 18,
    trend: 45,
    suggestedQty: 48,
    price: 42.00,
    category: 'Dairy'
  },
  {
    id: 'FC005',
    name: 'Rice 2kg',
    sku: 'GRN-RCE-2K',
    currentStock: 25,
    avgDailySales: 5,
    predictedDemand: 10,
    trend: 30,
    suggestedQty: 25,
    price: 45.99,
    category: 'Grains'
  },
  {
    id: 'FC006',
    name: 'Soap Bar',
    sku: 'HYG-SOP-001',
    currentStock: 60,
    avgDailySales: 12,
    predictedDemand: 20,
    trend: 40,
    suggestedQty: 50,
    price: 8.50,
    category: 'Hygiene'
  }
])

// Mock data - Search Items (All Catalog)
const searchItems = ref<SearchItem[]>([
  ...lowStockItems.value.map((item: LowStockItem): SearchItem => ({ ...item, orderQty: item.suggestedQty })),
  ...focusedItems.value.map((item: FocusedItem): SearchItem => ({ ...item, orderQty: item.suggestedQty })),
  {
    id: 'SR001',
    name: 'Potato Chips 120g',
    sku: 'SNK-CHP-120',
    currentStock: 85,
    price: 15.00,
    category: 'Snacks',
    orderQty: 1
  },
  {
    id: 'SR002',
    name: 'Peanut Butter 400g',
    sku: 'SPR-PNB-400',
    currentStock: 42,
    price: 38.99,
    category: 'Pantry',
    orderQty: 1
  },
  {
    id: 'SR003',
    name: 'Toothpaste 100ml',
    sku: 'HYG-TPS-100',
    currentStock: 55,
    price: 22.50,
    category: 'Hygiene',
    orderQty: 1
  },
  {
    id: 'SR004',
    name: 'Dishwashing Liquid 750ml',
    sku: 'CLN-DSH-750',
    currentStock: 38,
    price: 18.99,
    category: 'Cleaning',
    orderQty: 1
  },
  {
    id: 'SR005',
    name: 'Candles (Pack of 10)',
    sku: 'UTL-CND-10P',
    currentStock: 120,
    price: 12.00,
    category: 'Utilities',
    orderQty: 1
  },
  {
    id: 'SR006',
    name: 'Matches (Box)',
    sku: 'UTL-MCH-BOX',
    currentStock: 200,
    price: 3.50,
    category: 'Utilities',
    orderQty: 1
  }
])

// Categories
const categories = ['All', 'Bakery', 'Dairy', 'Beverages', 'Grains', 'Pantry', 'Cooking', 'Snacks', 'Hygiene', 'Cleaning', 'Utilities']

// Computed
const filteredSearchItems = computed(() => {
  let items = searchItems.value

  // Filter by category
  if (selectedCategory.value && selectedCategory.value !== 'All') {
    items = items.filter((item: SearchItem) => item.category === selectedCategory.value)
  }

  // Filter by search query
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase()
    items = items.filter((item: SearchItem) =>
      item.name.toLowerCase().includes(query) ||
      item.sku.toLowerCase().includes(query) ||
      item.category.toLowerCase().includes(query)
    )
  }

  return items
})

const subtotal = computed(() => {
  return orderItems.value.reduce((sum: number, item: CartItem) => sum + (item.price * item.quantity), 0)
})

const deliveryFee = computed(() => {
  return subtotal.value > 500 ? 0 : 50 // Free delivery over R500
})

const totalAmount = computed(() => {
  return subtotal.value + deliveryFee.value
})

// Total savings from group buys
const totalGroupBuySavings = computed(() => {
  return orderItems.value.reduce((sum: number, item: CartItem) => {
    if (item.groupBuyEnabled && item.groupBuyDetails) {
      return sum + item.groupBuyDetails.savings
    }
    return sum
  }, 0)
})

// Count of items with group buy enabled
const activeGroupBuyItemsCount = computed(() => {
  return orderItems.value.filter((item: CartItem) => item.groupBuyEnabled && item.groupBuyDetails).length
})

// Save active group buy to localStorage for reporting
const saveToActiveGroupBuys = (groupBuy: any) => {
  try {
    const existingGroupBuys = localStorage.getItem('toss-active-group-buys')
    let groupBuys = existingGroupBuys ? JSON.parse(existingGroupBuys) : []
    
    // Check if this group buy already exists
    const existingIndex = groupBuys.findIndex((gb: any) => gb.id === groupBuy.id)
    
    if (existingIndex > -1) {
      // Update existing group buy
      groupBuys[existingIndex] = {
        ...groupBuys[existingIndex],
        ...groupBuy,
        lastUpdated: new Date().toISOString()
      }
    } else {
      // Add new group buy
      groupBuys.push({
        ...groupBuy,
        joinedAt: new Date().toISOString(),
        lastUpdated: new Date().toISOString()
      })
    }
    
    localStorage.setItem('toss-active-group-buys', JSON.stringify(groupBuys))
  } catch (error) {
    console.error('Error saving active group buy:', error)
  }
}

// Methods
const addToCart = async (item: BaseItem | LowStockItem | FocusedItem | SearchItem, quantity: number = 1) => {
  const existingIndex = orderItems.value.findIndex((i: CartItem) => i.id === item.id)
  
  if (existingIndex > -1) {
    orderItems.value[existingIndex].quantity += quantity
    // Update group buy details when quantity changes
    if (orderItems.value[existingIndex].groupBuyEnabled) {
      await checkAndUpdateGroupBuy(existingIndex)
    }
  } else {
    // Add new item with group buy enabled by default
    const newItem: CartItem = {
      ...item,
      quantity,
      groupBuyEnabled: true, // ON by default
      groupBuyDetails: undefined
    }
    
    orderItems.value.push(newItem)
    
    // Check for active group buy or create new one
    const newIndex = orderItems.value.length - 1
    await checkAndUpdateGroupBuy(newIndex)
  }

  // Show toast notification instead of auto-opening cart on mobile
  if (window.innerWidth < 640) {
    const itemName = item.name.length > 30 ? item.name.substring(0, 30) + '...' : item.name
    toast.success(`${itemName} added to cart! (${orderItems.value.length} items)`)
  }
}

// Check for active group buy and auto-join or create
const checkAndUpdateGroupBuy = async (itemIndex: number) => {
  const item = orderItems.value[itemIndex]
  if (!item || !item.groupBuyEnabled) return
  
  try {
    // Check for active group buy using POST
    const response = await $fetch<{ hasActive: boolean; opportunity: any }>('/api/purchasing/group-buys/active', {
      method: 'POST' as any,
      body: { skus: [item.sku] } as any
    })
    
    if (response?.hasActive && response?.opportunity) {
      // Active group buy found - auto-join
      const gb = response.opportunity
      const estimatedSavings = Math.round((item.price * item.quantity * gb.savingsPercentage) / 100)
      const daysLeft = Math.ceil((new Date(gb.deadline).getTime() - Date.now()) / (1000 * 60 * 60 * 24))
      
      item.groupBuyDetails = {
        savings: estimatedSavings,
        endsIn: daysLeft === 0 ? 'today' : daysLeft === 1 ? 'tomorrow' : `${daysLeft} days`,
        progress: `${gb.currentQuantity}/${gb.targetQuantity} units`,
        status: `✅ Joined! ${gb.currentParticipants} members`,
        groupBuyId: gb.id,
        isNew: false
      }
      
      // Save to active group buys for reporting
      saveToActiveGroupBuys({
        id: gb.id,
        title: gb.title,
        description: gb.description,
        itemName: item.name,
        itemSku: item.sku,
        myQuantity: item.quantity,
        status: 'active',
        committed: gb.currentQuantity,
        minQuantity: gb.targetQuantity,
        memberCount: gb.currentParticipants,
        estimatedSavings,
        deadline: new Date(gb.deadline),
        joinedAt: new Date(),
        savingsPercentage: gb.savingsPercentage
      })
    } else {
      // No active group buy - create new one automatically
      const estimatedSavings = Math.round((item.price * item.quantity * 15) / 100) // Estimate 15% savings
      const newGroupBuyId = `GB-NEW-${Date.now()}`
      
      item.groupBuyDetails = {
        savings: estimatedSavings,
        endsIn: '7 days',
        progress: `${item.quantity}/${item.quantity * 5} units`,
        status: `🆕 New group buy started!`,
        groupBuyId: newGroupBuyId,
        isNew: true
      }
      
      // Save new group buy for reporting
      saveToActiveGroupBuys({
        id: newGroupBuyId,
        title: `${item.name} Group Buy`,
        description: `Bulk purchase for ${item.name}`,
        itemName: item.name,
        itemSku: item.sku,
        myQuantity: item.quantity,
        status: 'collecting',
        committed: item.quantity,
        minQuantity: item.quantity * 5,
        memberCount: 1,
        estimatedSavings,
        deadline: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000),
        joinedAt: new Date(),
        savingsPercentage: 15
      })
      
      toast.success(`Group buy started for ${item.name}! Invite others to unlock R${estimatedSavings} savings!`)
    }
  } catch (error) {
    console.error('Error checking group buy:', error)
    // Set default values on error
    if (item) {
      item.groupBuyDetails = {
        savings: Math.round((item.price * item.quantity * 10) / 100),
        endsIn: '7 days',
        progress: `${item.quantity}/500 units`,
        status: '🆕 Group buy available',
        isNew: true
      }
    }
  }
}

// Toggle group buy on/off for an item
const toggleGroupBuy = async (itemIndex: number) => {
  const item = orderItems.value[itemIndex]
  if (!item) return
  
  item.groupBuyEnabled = !item.groupBuyEnabled
  
  if (item.groupBuyEnabled) {
    // Enabled - check and setup group buy
    await checkAndUpdateGroupBuy(itemIndex)
    toast.success(`Group buy enabled for ${item.name}!`)
  } else {
    // Disabled - clear group buy details
    item.groupBuyDetails = undefined
    toast.info(`Group buy disabled for ${item.name}`)
  }
}

const removeFromCart = (index: number) => {
  orderItems.value.splice(index, 1)
}

const updateQuantity = async (index: number, newQuantity: number) => {
  if (newQuantity < 1) {
    removeFromCart(index)
  } else {
    const item = orderItems.value[index]
    if (item) {
      item.quantity = newQuantity
      // Update group buy details if enabled
      if (item.groupBuyEnabled) {
        await checkAndUpdateGroupBuy(index)
      }
    }
  }
}

// Smart Purchasing Handlers
const handleJoinGroupBuy = async () => {
  if (!smartAnalysis.value?.groupBuyOpportunity) return
  
  try {
    const items = orderItems.value.map((item: CartItem) => ({
      id: item.id,
      sku: item.sku,
      name: item.name,
      quantity: item.quantity,
      unitPrice: item.price,
      totalPrice: item.quantity * item.price
    }))
    
    await joinGroupBuy(smartAnalysis.value.groupBuyOpportunity.id, items)
    
    // Clear cart and redirect
    orderItems.value = []
    router.push('/purchasing/group-buying')
  } catch (error) {
    console.error('Error joining group buy:', error)
  }
}

const handleCreateGroupBuy = () => {
  // Store current cart for group buy creation
  const cartData = {
    items: orderItems.value.map((item: CartItem) => ({
      id: item.id,
      sku: item.sku,
      name: item.name,
      quantity: item.quantity,
      unitPrice: item.price,
      totalPrice: item.quantity * item.price
    })),
    potential: smartAnalysis.value?.groupBuyPotential
  }
  
  localStorage.setItem('group-buy-cart-data', JSON.stringify(cartData))
  router.push('/purchasing/group-buying?action=create')
}

const formatDeadline = (deadline: Date) => {
  const now = new Date()
  const diffTime = new Date(deadline).getTime() - now.getTime()
  const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24))
  
  if (diffDays === 0) return 'today'
  if (diffDays === 1) return 'tomorrow'
  return `in ${diffDays} days`
}

const placeOrder = async () => {
  if (orderItems.value.length === 0) return

  const orderNumber = 'PO-' + Date.now()
  const orderDate = new Date().toISOString()

  // Check if aggregation is available
  let purchaseType = 'individual'
  let savingsAmount = 0
  let savingsMethod = 'none'
  let aggregationGroupId = undefined

  if (smartAnalysis.value?.canAggregate && smartAnalysis.value.aggregationOpportunity) {
    purchaseType = 'aggregated'
    savingsAmount = smartAnalysis.value.aggregationOpportunity.estimatedSavings
    savingsMethod = 'auto-aggregation'
    aggregationGroupId = smartAnalysis.value.aggregationOpportunity.id
  }

  // Create order object with smart purchasing metadata
  const order = {
    id: orderNumber,
    orderNumber,
    items: orderItems.value,
    subtotal: subtotal.value,
    deliveryFee: deliveryFee.value,
    total: totalAmount.value - savingsAmount, // Apply savings
    originalPrice: totalAmount.value, // Store original price
    date: orderDate,
    status: purchaseType === 'aggregated' ? 'Aggregated' : 'Pending',
    supplier: 'Multiple Suppliers',
    expectedDelivery: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000).toISOString(), // 2 days from now
    
    // Smart Purchasing fields
    purchaseType,
    savingsAmount,
    savingsMethod,
    aggregationGroupId
  }

  // Store order in orders list (for orders page)
  const existingOrders = JSON.parse(localStorage.getItem('toss-orders') || '[]')
  existingOrders.unshift(order) // Add to beginning of array
  localStorage.setItem('toss-orders', JSON.stringify(existingOrders))

  // Show success toast with savings info
  const successMessage = savingsAmount > 0 
    ? `Order ${orderNumber} created! You saved R${savingsAmount} through ${savingsMethod}! 🎉`
    : `Order ${orderNumber} created successfully!`
  
  toast.success(
    successMessage + ' Redirecting...',
    savingsAmount > 0 ? '💰 Smart Purchase!' : '✅ Order Placed',
    3000
  )

  // Wait a moment for toast to show, then navigate
  setTimeout(() => {
    router.push('/purchasing/orders')
  }, 1500)
}
</script>

<style scoped>
.slide-enter-active,
.slide-leave-active {
  transition: all 0.3s ease;
}

.slide-enter-from {
  transform: translateX(100%);
  opacity: 0;
}

.slide-leave-to {
  transform: translateX(100%);
  opacity: 0;
}
</style>

