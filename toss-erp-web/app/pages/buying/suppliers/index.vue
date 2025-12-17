<template>
  <div class="min-h-screen bg-gradient-to-br from-slate-50 via-purple-50/30 to-slate-100 dark:from-slate-900 dark:via-slate-900 dark:to-slate-800">
    <!-- Page Header with Glass Morphism -->
    <div class="bg-white/80 dark:bg-slate-800/80 backdrop-blur-xl shadow-sm border-b border-slate-200/50 dark:border-slate-700/50 sticky top-0 z-10">
      <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 sm:py-6">
        <div class="flex items-center justify-between">
          <div class="flex-1 min-w-0">
            <h1 class="text-2xl sm:text-3xl font-bold bg-gradient-to-r from-purple-600 to-blue-600 bg-clip-text text-transparent">
              Suppliers
            </h1>
            <p class="mt-1 text-sm text-slate-600 dark:text-slate-400">
              Manage vendor relationships and procurement partners
            </p>
          </div>
          <div class="flex space-x-2 sm:space-x-3 flex-shrink-0">
            <button
              @click="showAddModal = true"
              class="inline-flex items-center justify-center px-4 sm:px-6 py-2.5 sm:py-3 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-xl hover:from-purple-700 hover:to-blue-700 shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 font-semibold text-sm sm:text-base"
            >
              <PlusIcon class="w-5 h-5 mr-2" />
              Add Supplier
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Main Content -->
    <div class="w-full max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      
      <!-- Stats Cards -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Total Suppliers</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ stats.totalSuppliers }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-purple-500 to-blue-600 rounded-xl">
              <UserGroupIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Active</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ stats.activeSuppliers }}</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-green-500 to-emerald-600 rounded-xl">
              <CheckCircleIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">Avg Rating</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ stats.avgRating }}/5</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-yellow-500 to-orange-600 rounded-xl">
              <StarIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>

        <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-slate-600 dark:text-slate-400 mb-1">On-Time Rate</p>
              <p class="text-3xl font-bold text-slate-900 dark:text-white">{{ stats.onTimeDelivery }}%</p>
            </div>
            <div class="p-3 bg-gradient-to-br from-blue-500 to-cyan-600 rounded-xl">
              <TruckIcon class="w-8 h-8 text-white" />
            </div>
          </div>
        </div>
      </div>

      <!-- Filters and Search -->
      <div class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 mb-6">
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <!-- Search -->
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
              <MagnifyingGlassIcon class="h-5 w-5 text-slate-400" />
            </div>
            <input
              type="text"
              v-model="searchQuery"
              placeholder="Search by name, product, location..."
              class="w-full pl-11 pr-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white transition-all duration-200"
            />
          </div>

          <!-- Status Filter -->
          <select
            v-model="statusFilter"
            class="w-full px-4 py-2.5 border border-slate-300 dark:border-slate-600 rounded-xl focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white dark:bg-slate-700 text-slate-900 dark:text-white transition-all duration-200"
          >
            <option value="">All Status</option>
            <option value="active">Active</option>
            <option value="inactive">Inactive</option>
            <option value="pending">Pending Verification</option>
          </select>

          <!-- Actions -->
          <div class="flex space-x-2">
            <button
              @click="exportSuppliers"
              class="flex-1 inline-flex items-center justify-center px-4 py-2.5 border-2 border-slate-300 dark:border-slate-600 rounded-xl text-sm font-medium text-slate-700 dark:text-slate-300 bg-white dark:bg-slate-700 hover:bg-slate-50 dark:hover:bg-slate-600 hover:border-slate-400 dark:hover:border-slate-500 transition-all duration-200"
            >
              <ArrowDownTrayIcon class="w-4 h-4 mr-2" />
              Export
            </button>
          </div>
        </div>
      </div>

      <!-- Suppliers Grid -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <template v-if="loading">
          <div v-for="n in 6" :key="n" class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-6 animate-pulse">
            <div class="flex items-center mb-4">
              <div class="h-12 w-12 bg-slate-200 dark:bg-slate-700 rounded-xl"></div>
              <div class="ml-4 flex-1">
                <div class="h-5 bg-slate-200 dark:bg-slate-700 rounded w-32 mb-2"></div>
                <div class="h-4 bg-slate-200 dark:bg-slate-700 rounded w-24"></div>
              </div>
            </div>
          </div>
        </template>

        <div v-for="supplier in filteredSuppliers" v-else :key="supplier.id"
          class="bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 overflow-hidden hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1"
        >
          <!-- Supplier Header -->
          <div class="bg-gradient-to-r from-purple-50 to-blue-50 dark:from-purple-900/20 dark:to-blue-900/20 px-6 py-4 border-b border-slate-200 dark:border-slate-600">
            <div class="flex items-center justify-between">
              <div class="flex items-center space-x-4">
                <div class="flex-shrink-0 h-12 w-12">
                  <div class="h-12 w-12 rounded-xl bg-gradient-to-br from-purple-500 to-blue-600 flex items-center justify-center">
                    <span class="text-xl font-bold text-white">{{ supplier.name.charAt(0) }}</span>
                  </div>
                </div>
                <div>
                  <h3 class="text-lg font-bold text-slate-900 dark:text-white">{{ supplier.name }}</h3>
                  <div class="flex items-center mt-1">
                    <StarIcon v-for="i in 5" :key="i" class="w-4 h-4" :class="i <= supplier.rating ? 'text-yellow-400' : 'text-slate-300 dark:text-slate-600'" />
                    <span class="ml-1 text-xs text-slate-600 dark:text-slate-400">({{ supplier.rating }})</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Supplier Details -->
          <div class="px-6 py-4">
            <div class="space-y-3 mb-4">
              <div class="flex items-center text-sm">
                <EnvelopeIcon class="w-4 h-4 text-slate-400 mr-2" />
                <span class="text-slate-600 dark:text-slate-400">{{ supplier.email }}</span>
              </div>
              <div class="flex items-center text-sm">
                <PhoneIcon class="w-4 h-4 text-slate-400 mr-2" />
                <span class="text-slate-600 dark:text-slate-400">{{ supplier.phone }}</span>
              </div>
              <div class="flex items-center text-sm">
                <MapPinIcon class="w-4 h-4 text-slate-400 mr-2" />
                <span class="text-slate-600 dark:text-slate-400">{{ supplier.location }}</span>
              </div>
              <div v-if="supplier.currency" class="flex items-center text-sm">
                <CurrencyDollarIcon class="w-4 h-4 text-slate-400 mr-2" />
                <span class="text-slate-600 dark:text-slate-400">{{ supplier.currency }}</span>
              </div>
            </div>

            <div class="grid grid-cols-2 gap-3 mb-4 pt-4 border-t border-slate-200 dark:border-slate-700">
              <div>
                <p class="text-xs text-slate-500 dark:text-slate-500">Total Orders</p>
                <p class="text-lg font-bold text-slate-900 dark:text-white">{{ supplier.totalOrders }}</p>
              </div>
              <div>
                <p class="text-xs text-slate-500 dark:text-slate-500">On-Time Rate</p>
                <p class="text-lg font-bold text-green-600">{{ supplier.onTimeRate }}%</p>
              </div>
            </div>

            <!-- Status Badge -->
            <div class="mb-4">
              <span 
                class="px-3 py-1 rounded-full text-xs font-medium"
                :class="getStatusBadge(supplier.status)"
              >
                {{ getStatusLabel(supplier.status) }}
              </span>
            </div>

            <!-- Products/Services -->
            <div v-if="supplier.products && supplier.products.length > 0" class="mb-4 pb-4 border-b border-slate-200 dark:border-slate-700">
              <p class="text-xs font-medium text-slate-600 dark:text-slate-400 mb-2">Products & Services:</p>
              <div class="flex flex-wrap gap-1.5">
                <span 
                  v-for="(product, idx) in supplier.products.slice(0, 3)" 
                  :key="idx"
                  class="inline-flex items-center px-2.5 py-1 rounded-lg text-xs font-medium bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400"
                >
                  {{ product }}
                </span>
                <span 
                  v-if="supplier.products.length > 3"
                  class="inline-flex items-center px-2.5 py-1 rounded-lg text-xs font-medium bg-slate-100 text-slate-600 dark:bg-slate-900/30 dark:text-slate-400"
                >
                  +{{ supplier.products.length - 3 }} more
                </span>
              </div>
            </div>

            <!-- Actions -->
            <div class="flex space-x-2">
              <button 
                @click="viewSupplier(supplier)"
                class="flex-1 px-4 py-2 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-lg hover:from-purple-700 hover:to-blue-700 shadow-md hover:shadow-lg transition-all duration-200 text-sm font-medium"
              >
                View Details
              </button>
              <button 
                @click="contactSupplier(supplier)"
                class="px-4 py-2 border-2 border-slate-300 dark:border-slate-600 rounded-lg text-sm font-medium text-slate-700 dark:text-slate-300 hover:bg-slate-50 dark:hover:bg-slate-700 transition-all duration-200"
              >
                Contact
              </button>
            </div>
          </div>
        </div>

        <!-- Empty State -->
        <div v-if="filteredSuppliers.length === 0 && !loading" class="col-span-full bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-200 dark:border-slate-700 p-12 text-center">
          <div class="flex flex-col items-center justify-center">
            <div class="p-4 bg-gradient-to-br from-purple-100 to-blue-100 dark:from-purple-900/20 dark:to-blue-900/20 rounded-full mb-4">
              <UserGroupIcon class="w-12 h-12 text-purple-600 dark:text-purple-400" />
            </div>
            <p class="text-lg font-semibold text-slate-900 dark:text-white mb-2">No suppliers found</p>
            <p class="text-slate-600 dark:text-slate-400 mb-4">Start by adding your first supplier!</p>
            <button
              @click="showAddModal = true"
              class="inline-flex items-center px-6 py-3 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-xl hover:from-purple-700 hover:to-blue-700 shadow-lg hover:shadow-xl transition-all duration-200 transform hover:scale-105 font-semibold"
            >
              <PlusIcon class="w-5 h-5 mr-2" />
              Add Supplier
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Add Supplier Modal -->
    <Transition name="modal">
      <div v-if="showAddModal" class="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 overflow-y-auto">
        <div class="flex min-h-full items-center justify-center p-4">
          <div class="relative bg-white dark:bg-slate-800 rounded-2xl shadow-2xl border border-slate-200 dark:border-slate-700 w-full max-w-4xl">
            
            <!-- Header -->
            <div class="bg-gradient-to-r from-purple-600 to-blue-600 px-6 py-4 flex items-center justify-between rounded-t-2xl">
              <div>
                <h3 class="text-xl font-bold text-white">Add New Supplier</h3>
                <p class="text-sm text-white/80">Register a new vendor in your network</p>
              </div>
              <button @click="showAddModal = false" class="p-2 hover:bg-white/20 rounded-lg transition-colors">
                <XMarkIcon class="w-6 h-6 text-white" />
              </button>
            </div>
            
            <!-- Form -->
            <form @submit.prevent="addSupplier" class="p-6 space-y-6 max-h-[70vh] overflow-y-auto">
              
              <!-- Basic Information -->
              <div class="space-y-4">
                <h4 class="text-sm font-semibold text-slate-900 dark:text-white flex items-center">
                  <UserGroupIcon class="w-4 h-4 mr-2 text-purple-600" />
                  Basic Information
                </h4>
                
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                      Supplier Name *
                    </label>
                    <input 
                      v-model="newSupplier.name"
                      type="text" 
                      required
                      placeholder="ABC Suppliers Ltd"
                      class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-purple-500 focus:border-purple-500 transition-all"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                      Supplier Group/Category *
                    </label>
                    <select
                      v-model="newSupplier.category"
                      required
                      class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-purple-500 focus:border-purple-500 transition-all"
                    >
                      <option value="">Select Category</option>
                      <option value="Food & Beverage">Food & Beverage</option>
                      <option value="Hardware">Hardware</option>
                      <option value="Packaging">Packaging</option>
                      <option value="Cleaning">Cleaning</option>
                      <option value="Stationery">Stationery</option>
                      <option value="Technology">Technology</option>
                      <option value="Pharmaceutical">Pharmaceutical</option>
                      <option value="Other">Other</option>
                    </select>
                  </div>
                </div>
              </div>

              <!-- Tax Details (ERPNext Feature) -->
              <div class="space-y-4">
                <h4 class="text-sm font-semibold text-slate-900 dark:text-white flex items-center">
                  <DocumentTextIcon class="w-4 h-4 mr-2 text-blue-600" />
                  Tax & Legal Information
                </h4>
                
                <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                      Tax ID / VAT Number
                    </label>
                    <input 
                      v-model="newSupplier.taxId"
                      type="text" 
                      placeholder="e.g., 1234567890"
                      class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-purple-500 focus:border-purple-500 transition-all"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                      Tax Category
                    </label>
                    <select
                      v-model="newSupplier.taxCategory"
                      class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-purple-500 focus:border-purple-500 transition-all"
                    >
                      <option value="">Select Tax Category</option>
                      <option value="Standard">Standard</option>
                      <option value="Exempt">Exempt</option>
                      <option value="Zero-Rated">Zero-Rated</option>
                    </select>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                      Country
                    </label>
                    <select
                      v-model="newSupplier.country"
                      class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-purple-500 focus:border-purple-500 transition-all"
                    >
                      <option value="South Africa">South Africa</option>
                      <option value="Zimbabwe">Zimbabwe</option>
                      <option value="Botswana">Botswana</option>
                      <option value="Namibia">Namibia</option>
                    </select>
                  </div>
                </div>
              </div>

              <!-- Contact Information -->
              <div class="space-y-4">
                <h4 class="text-sm font-semibold text-slate-900 dark:text-white flex items-center">
                  <PhoneIcon class="w-4 h-4 mr-2 text-blue-600" />
                  Contact Information
                </h4>
                
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                      Contact Person
                    </label>
                    <input 
                      v-model="newSupplier.contactPerson"
                      type="text" 
                      placeholder="John Doe"
                      class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-purple-500 focus:border-purple-500 transition-all"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                      Phone Number *
                    </label>
                    <input 
                      v-model="newSupplier.phone"
                      type="tel" 
                      required
                      placeholder="+27 XX XXX XXXX"
                      class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-purple-500 focus:border-purple-500 transition-all"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                      Email Address
                    </label>
                    <input 
                      v-model="newSupplier.email"
                      type="email" 
                      placeholder="supplier@example.com"
                      class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-purple-500 focus:border-purple-500 transition-all"
                    />
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                      Website
                    </label>
                    <input 
                      v-model="newSupplier.website"
                      type="url" 
                      placeholder="https://example.com"
                      class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-purple-500 focus:border-purple-500 transition-all"
                    />
                  </div>
                </div>
              </div>

              <!-- Currency & Payment Terms (ERPNext Feature) -->
              <div class="space-y-4">
                <h4 class="text-sm font-semibold text-slate-900 dark:text-white flex items-center">
                  <CurrencyDollarIcon class="w-4 h-4 mr-2 text-green-600" />
                  Currency & Payment Terms
                </h4>
                
                <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div>
                    <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                      Billing Currency
                    </label>
                    <select
                      v-model="newSupplier.currency"
                      class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-purple-500 focus:border-purple-500 transition-all"
                    >
                      <option value="ZAR">ZAR (South African Rand)</option>
                      <option value="USD">USD (US Dollar)</option>
                      <option value="EUR">EUR (Euro)</option>
                      <option value="GBP">GBP (British Pound)</option>
                    </select>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                      Default Payment Terms
                    </label>
                    <select
                      v-model="newSupplier.paymentTerms"
                      class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-purple-500 focus:border-purple-500 transition-all"
                    >
                      <option value="COD">Cash on Delivery</option>
                      <option value="Net 7">Net 7 Days</option>
                      <option value="Net 30">Net 30 Days</option>
                      <option value="Net 60">Net 60 Days</option>
                      <option value="Net 90">Net 90 Days</option>
                    </select>
                  </div>

                  <div>
                    <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                      Credit Limit (Optional)
                    </label>
                    <input 
                      v-model.number="newSupplier.creditLimit"
                      type="number" 
                      step="0.01"
                      placeholder="0.00"
                      class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-purple-500 focus:border-purple-500 transition-all"
                    />
                  </div>
                </div>
              </div>

              <!-- Address -->
              <div class="space-y-4">
                <h4 class="text-sm font-semibold text-slate-900 dark:text-white flex items-center">
                  <MapPinIcon class="w-4 h-4 mr-2 text-green-600" />
                  Address
                </h4>
                
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                    Physical Address
                  </label>
                  <textarea 
                    v-model="newSupplier.address"
                    rows="2"
                    placeholder="123 Main Street, City, Province, Postal Code"
                    class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-purple-500 focus:border-purple-500 transition-all"
                  ></textarea>
                </div>
              </div>

              <!-- Products & Services (Crucial Link) -->
              <div class="space-y-4">
                <h4 class="text-sm font-semibold text-slate-900 dark:text-white flex items-center">
                  <svg class="w-4 h-4 mr-2 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
                  </svg>
                  Products & Services Supplied
                </h4>
                
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                    What does this supplier provide? *
                  </label>
                  <div class="flex gap-2 mb-3">
                    <input 
                      v-model="newProductInput"
                      type="text" 
                      placeholder="e.g., Fresh Vegetables, Canned Goods, Office Supplies..."
                      @keyup.enter="addProduct"
                      class="flex-1 px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-purple-500 focus:border-purple-500 transition-all"
                    />
                    <button
                      type="button"
                      @click="addProduct"
                      class="px-4 py-3 bg-purple-600 hover:bg-purple-700 text-white rounded-xl font-medium transition-all"
                    >
                      Add
                    </button>
                  </div>
                  
                  <!-- Product Tags -->
                  <div v-if="newSupplier.products.length > 0" class="flex flex-wrap gap-2">
                    <span 
                      v-for="(product, idx) in newSupplier.products" 
                      :key="idx"
                      class="inline-flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-sm font-medium bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400"
                    >
                      {{ product }}
                      <button
                        type="button"
                        @click="removeProduct(idx)"
                        class="ml-1 hover:bg-purple-200 dark:hover:bg-purple-800 rounded-full p-0.5 transition-colors"
                      >
                        <XMarkIcon class="w-3.5 h-3.5" />
                      </button>
                    </span>
                  </div>
                  <p v-else class="text-sm text-slate-500 dark:text-slate-400">
                    Add products or services this supplier provides (e.g., for group buying, procurement, etc.)
                  </p>
                </div>
              </div>

              <!-- Additional Information -->
              <div class="space-y-4">
                <h4 class="text-sm font-semibold text-slate-900 dark:text-white flex items-center">
                  <TruckIcon class="w-4 h-4 mr-2 text-orange-600" />
                  Additional Information
                </h4>
                
                <div>
                  <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
                    Notes
                  </label>
                  <textarea 
                    v-model="newSupplier.notes"
                    rows="3"
                    placeholder="Any special notes about this supplier (delivery areas, MOQ, special terms, etc.)..."
                    class="w-full px-4 py-3 rounded-xl border-2 border-slate-200 dark:border-slate-600 bg-white dark:bg-slate-900 text-slate-900 dark:text-white focus:ring-2 focus:ring-purple-500 focus:border-purple-500 transition-all"
                  ></textarea>
                </div>

                <div class="flex items-center gap-3">
                  <input 
                    v-model="newSupplier.isTransporter"
                    type="checkbox"
                    id="isTransporter"
                    class="w-4 h-4 text-purple-600 bg-white dark:bg-slate-900 border-slate-300 dark:border-slate-600 rounded focus:ring-purple-500"
                  />
                  <label for="isTransporter" class="text-sm font-medium text-slate-700 dark:text-slate-300">
                    This supplier also provides transport/logistics services
                  </label>
                </div>
              </div>

              <!-- Info Box -->
              <div class="bg-blue-50 dark:bg-blue-900/20 rounded-xl p-4 border-2 border-blue-200 dark:border-blue-800">
                <div class="flex items-start gap-3">
                  <div class="p-2 bg-blue-500 rounded-lg flex-shrink-0">
                    <CheckCircleIcon class="w-5 h-5 text-white" />
                  </div>
                  <div class="text-sm text-slate-700 dark:text-slate-300">
                    <p class="font-semibold mb-1">Supplier Verification</p>
                    <p>New suppliers will be added with a "Pending" status. Link products/services to enable:</p>
                    <ul class="list-disc list-inside mt-2 text-xs space-y-1">
                      <li>Group buying pool creation</li>
                      <li>Purchase order generation</li>
                      <li>Smart procurement suggestions</li>
                      <li>Supplier scorecarding</li>
                    </ul>
                  </div>
                </div>
              </div>

              <!-- Actions -->
              <div class="flex gap-3 pt-4 border-t border-slate-200 dark:border-slate-700">
                <button
                  type="button"
                  @click="showAddModal = false"
                  class="flex-1 px-6 py-3 border-2 border-slate-300 dark:border-slate-600 rounded-xl text-slate-700 dark:text-slate-300 font-semibold hover:bg-slate-50 dark:hover:bg-slate-700 transition-all"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  :disabled="!newSupplier.name || !newSupplier.phone || !newSupplier.category || newSupplier.products.length === 0"
                  class="flex-1 px-6 py-3 bg-gradient-to-r from-purple-600 to-blue-600 text-white rounded-xl font-semibold hover:from-purple-700 hover:to-blue-700 shadow-lg hover:shadow-xl transition-all disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  Add Supplier
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import {
  PlusIcon,
  MagnifyingGlassIcon,
  ArrowDownTrayIcon,
  UserGroupIcon,
  CheckCircleIcon,
  StarIcon,
  TruckIcon,
  EnvelopeIcon,
  PhoneIcon,
  MapPinIcon,
  XMarkIcon,
  CurrencyDollarIcon,
  DocumentTextIcon
} from '@heroicons/vue/24/outline'

// Page metadata
useHead({
  title: 'Suppliers - TOSS ERP',
  meta: [
    { name: 'description', content: 'Manage supplier network and procurement relationships in TOSS ERP' }
  ]
})

// State
const loading = ref(false)
const showAddModal = ref(false)
const searchQuery = ref('')
const statusFilter = ref('')

// New Supplier Form
const newSupplier = ref({
  name: '',
  category: '',
  contactPerson: '',
  phone: '',
  email: '',
  website: '',
  address: '',
  country: 'South Africa',
  taxId: '',
  taxCategory: '',
  currency: 'ZAR',
  paymentTerms: 'Net 30',
  creditLimit: 0,
  notes: '',
  isTransporter: false,
  products: [] as string[]
})

// Product input for adding multiple products
const newProductInput = ref('')

// Stats
const stats = ref({
  totalSuppliers: 42,
  activeSuppliers: 38,
  avgRating: 4.2,
  onTimeDelivery: 94
})

// Mock suppliers data
const suppliers = ref([
  {
    id: 1,
    name: 'ABC Suppliers',
    email: 'contact@abc-suppliers.co.za',
    phone: '+27 11 123 4567',
    location: 'Johannesburg, GP',
    rating: 5,
    totalOrders: 124,
    onTimeRate: 98,
    status: 'active',
    currency: 'ZAR',
    products: ['Canned Goods', 'Beverages', 'Snacks', 'Dairy Products']
  },
  {
    id: 2,
    name: 'XYZ Wholesalers',
    email: 'sales@xyz-wholesale.co.za',
    phone: '+27 21 234 5678',
    location: 'Cape Town, WC',
    rating: 4,
    totalOrders: 89,
    onTimeRate: 92,
    status: 'active',
    currency: 'ZAR',
    products: ['Fresh Produce', 'Vegetables', 'Fruits', 'Organic Items']
  },
  {
    id: 3,
    name: 'Quality Foods Ltd',
    email: 'info@qualityfoods.co.za',
    phone: '+27 31 345 6789',
    location: 'Durban, KZN',
    rating: 5,
    totalOrders: 156,
    onTimeRate: 96,
    status: 'active',
    currency: 'ZAR',
    products: ['Frozen Foods', 'Meat Products', 'Seafood', 'Poultry']
  },
  {
    id: 4,
    name: 'Tech Solutions Inc',
    email: 'support@techsolutions.co.za',
    phone: '+27 12 456 7890',
    location: 'Pretoria, GP',
    rating: 4,
    totalOrders: 67,
    onTimeRate: 88,
    status: 'active',
    currency: 'USD',
    products: ['POS Systems', 'Computers', 'Printers', 'Software Licenses']
  },
  {
    id: 5,
    name: 'Fresh Produce Co',
    email: 'orders@freshproduce.co.za',
    phone: '+27 11 567 8901',
    location: 'Johannesburg, GP',
    rating: 3,
    totalOrders: 45,
    onTimeRate: 85,
    status: 'inactive',
    currency: 'ZAR',
    products: ['Local Vegetables', 'Seasonal Fruits']
  },
  {
    id: 6,
    name: 'Industrial Supplies SA',
    email: 'sales@industrial-sa.co.za',
    phone: '+27 21 678 9012',
    location: 'Cape Town, WC',
    rating: 5,
    totalOrders: 203,
    onTimeRate: 99,
    status: 'active',
    currency: 'ZAR',
    products: ['Cleaning Supplies', 'Packaging Materials', 'Safety Equipment', 'Stationery']
  }
])

// Computed
const filteredSuppliers = computed(() => {
  return suppliers.value.filter((supplier: any) => {
    const matchesSearch = !searchQuery.value || 
      supplier.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      supplier.email.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      supplier.location.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      (supplier.products && supplier.products.some((p: string) => 
        p.toLowerCase().includes(searchQuery.value.toLowerCase())
      ))
    
    const matchesStatus = !statusFilter.value || supplier.status === statusFilter.value
    
    return matchesSearch && matchesStatus
  })
})

// Methods
const viewSupplier = (supplier: any) => {
  alert(`Viewing supplier details for ${supplier.name}\n\nProducts/Services: ${supplier.products.join(', ')}\n\nActions Available:\n- Create Purchase Order\n- Request for Quotation\n- View Purchase History\n- Supplier Scorecard`)
}

const contactSupplier = (supplier: any) => {
  alert(`Contacting ${supplier.name}...\nEmail: ${supplier.email}\nPhone: ${supplier.phone}`)
}

const exportSuppliers = () => {
  alert('Exporting suppliers to CSV/Excel with linked products')
}

// Add product to list
const addProduct = () => {
  const product = newProductInput.value.trim()
  if (product && !newSupplier.value.products.includes(product)) {
    newSupplier.value.products.push(product)
    newProductInput.value = ''
  }
}

// Remove product from list
const removeProduct = (index: number) => {
  newSupplier.value.products.splice(index, 1)
}

const getStatusLabel = (status: string) => {
  const labels: Record<string, string> = {
    'active': 'Active',
    'inactive': 'Inactive',
    'pending': 'Pending Verification'
  }
  return labels[status] || status
}

const getStatusBadge = (status: string) => {
  const badges = {
    active: 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400',
    inactive: 'bg-slate-100 text-slate-800 dark:bg-slate-900/30 dark:text-slate-400',
    pending: 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400'
  }
  return badges[status as keyof typeof badges] || 'bg-slate-100 text-slate-800'
}

const addSupplier = () => {
  if (!newSupplier.value.name || !newSupplier.value.phone || !newSupplier.value.category) {
    alert('Please fill in all required fields')
    return
  }

  if (newSupplier.value.products.length === 0) {
    alert('Please add at least one product or service that this supplier provides')
    return
  }

  // Create new supplier object
  const supplier = {
    id: suppliers.value.length + 1,
    name: newSupplier.value.name,
    email: newSupplier.value.email || 'N/A',
    phone: newSupplier.value.phone,
    location: newSupplier.value.address || 'Not specified',
    rating: 0,
    totalOrders: 0,
    onTimeRate: 0,
    status: 'pending',
    currency: newSupplier.value.currency,
    products: [...newSupplier.value.products]
  }

  // Add to suppliers list
  suppliers.value.unshift(supplier)

  // Update stats
  stats.value.totalSuppliers++

  // Reset form
  newSupplier.value = {
    name: '',
    category: '',
    contactPerson: '',
    phone: '',
    email: '',
    website: '',
    address: '',
    country: 'South Africa',
    taxId: '',
    taxCategory: '',
    currency: 'ZAR',
    paymentTerms: 'Net 30',
    creditLimit: 0,
    notes: '',
    isTransporter: false,
    products: []
  }
  newProductInput.value = ''

  // Close modal
  showAddModal.value = false

  // Show success message
  const productCount = supplier.products.length
  alert(`✓ Supplier "${supplier.name}" added successfully!\n\nProducts/Services Linked: ${productCount} items\nStatus: Pending verification\n\nNext Steps:\n- Verify supplier credentials\n- Create first Purchase Order\n- Set up automated ordering\n- Link to Group Buying pools`)
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
</style>

