<template>
  <div class="min-h-screen bg-gray-50">
    <!-- Onboarding Modal - First Time Users -->
    <OnboardingModal />
    
    <!-- Mobile-First Page Container -->
    <div class="p-4 pb-20 space-y-4 sm:p-6 sm:space-y-6 lg:pb-6">
      <!-- Language Switcher - Top Right -->
      <div class="flex justify-end mb-4">
        <LanguageSwitcher />
      </div>
      
      <!-- Hero Section -->
      <HeroSection />
      
      <!-- App Purpose Explanation Card - NEW! -->
      <div class="bg-white rounded-xl border-2 border-blue-200 p-5 mb-4 shadow-sm">
        <div class="flex items-start gap-3">
          <div class="flex-shrink-0 w-12 h-12 bg-blue-100 rounded-full flex items-center justify-center">
            <svg class="w-6 h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
          </div>
          <div class="flex-1 min-w-0">
            <h3 class="text-lg font-bold text-gray-900 mb-2">Welcome to TOSS!</h3>
            <p class="text-base text-gray-700 leading-relaxed mb-3">
              TOSS helps you order stock for your spaza shop quickly and easily. No complicated forms or paperwork - just pick what you need and we deliver to your shop.
            </p>
            <div class="space-y-2">
              <div class="flex items-center gap-2 text-sm text-gray-700">
                <svg class="w-5 h-5 text-green-600 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                  <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
                </svg>
                <span class="font-medium">Browse products and add to your order</span>
              </div>
              <div class="flex items-center gap-2 text-sm text-gray-700">
                <svg class="w-5 h-5 text-green-600 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                  <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
                </svg>
                <span class="font-medium">Confirm your order</span>
              </div>
              <div class="flex items-center gap-2 text-sm text-gray-700">
                <svg class="w-5 h-5 text-green-600 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
                  <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
                </svg>
                <span class="font-medium">Track delivery to your shop</span>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <!-- Trust Signals - Community Testimonials - NEW! -->
      <div class="bg-gradient-to-r from-purple-50 to-pink-50 rounded-xl border-2 border-purple-200 p-5 mb-4 shadow-sm">
        <div class="text-center mb-4">
          <div class="inline-flex items-center justify-center gap-2 px-4 py-2 bg-purple-600 text-white rounded-full font-bold text-sm shadow-md mb-3">
            <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
              <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
            </svg>
            <span>Trusted by Township Businesses</span>
          </div>
          <h3 class="text-xl font-bold text-gray-900 mb-2">What Shop Owners Say</h3>
        </div>
        
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <!-- Testimonial 1 -->
          <div class="bg-white rounded-lg p-4 shadow-sm">
            <div class="flex items-start gap-3 mb-3">
              <div class="w-12 h-12 bg-purple-500 rounded-full flex items-center justify-center flex-shrink-0 text-white font-bold text-lg">
                T
              </div>
              <div class="min-w-0">
                <p class="font-bold text-gray-900 text-base">Thabo M.</p>
                <p class="text-sm text-gray-600">Soweto, Johannesburg</p>
              </div>
            </div>
            <p class="text-base text-gray-700 italic leading-relaxed">
              "TOSS makes ordering easy! I can order my stock from my phone and it arrives the next day. No more traveling to the wholesaler."
            </p>
            <div class="flex gap-1 mt-2">
              <svg class="w-5 h-5 text-yellow-400" fill="currentColor" viewBox="0 0 20 20"><path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" /></svg>
              <svg class="w-5 h-5 text-yellow-400" fill="currentColor" viewBox="0 0 20 20"><path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" /></svg>
              <svg class="w-5 h-5 text-yellow-400" fill="currentColor" viewBox="0 0 20 20"><path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" /></svg>
              <svg class="w-5 h-5 text-yellow-400" fill="currentColor" viewBox="0 0 20 20"><path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" /></svg>
              <svg class="w-5 h-5 text-yellow-400" fill="currentColor" viewBox="0 0 20 20"><path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" /></svg>
            </div>
          </div>
          
          <!-- Testimonial 2 -->
          <div class="bg-white rounded-lg p-4 shadow-sm">
            <div class="flex items-start gap-3 mb-3">
              <div class="w-12 h-12 bg-pink-500 rounded-full flex items-center justify-center flex-shrink-0 text-white font-bold text-lg">
                N
              </div>
              <div class="min-w-0">
                <p class="font-bold text-gray-900 text-base">Nomsa K.</p>
                <p class="text-sm text-gray-600">Khayelitsha, Cape Town</p>
              </div>
            </div>
            <p class="text-base text-gray-700 italic leading-relaxed">
              "Group buying with my neighbors saves me money! We order together and get better prices. Sharp!"
            </p>
            <div class="flex gap-1 mt-2">
              <svg class="w-5 h-5 text-yellow-400" fill="currentColor" viewBox="0 0 20 20"><path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" /></svg>
              <svg class="w-5 h-5 text-yellow-400" fill="currentColor" viewBox="0 0 20 20"><path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" /></svg>
              <svg class="w-5 h-5 text-yellow-400" fill="currentColor" viewBox="0 0 20 20"><path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" /></svg>
              <svg class="w-5 h-5 text-yellow-400" fill="currentColor" viewBox="0 0 20 20"><path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" /></svg>
              <svg class="w-5 h-5 text-yellow-400" fill="currentColor" viewBox="0 0 20 20"><path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" /></svg>
              <svg class="w-5 h-5 text-yellow-400" fill="currentColor" viewBox="0 0 20 20"><path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" /></svg>
            </div>
          </div>
        </div>
        
        <!-- Community Stats -->
        <div class="mt-4 pt-4 border-t border-purple-200">
          <div class="grid grid-cols-3 gap-4 text-center">
            <div>
              <p class="text-2xl font-bold text-purple-600">500+</p>
              <p class="text-sm text-gray-700 font-medium">Spaza Shops</p>
            </div>
            <div>
              <p class="text-2xl font-bold text-purple-600">98%</p>
              <p class="text-sm text-gray-700 font-medium">Happy Customers</p>
            </div>
            <div>
              <p class="text-2xl font-bold text-purple-600">24-48h</p>
              <p class="text-sm text-gray-700 font-medium">Delivery Time</p>
            </div>
          </div>
        </div>
      </div>

      <!-- WhatsApp Quick Order -->
      <div class="mb-4">
        <WhatsAppChatPlaceholder />
      </div>
      
      <!-- Group Buying Feature -->
      <GroupBuyingCard />

      <!-- Quick Stats - Spaza Shop Focused -->
      <div class="grid grid-cols-1 gap-3 xs:grid-cols-2 lg:grid-cols-4 sm:gap-6">
        <div class="p-5 bg-white rounded-xl border-2 border-gray-200 shadow-sm sm:p-6 hover:border-blue-500 transition-colors">
          <div class="flex justify-between items-center">
            <div>
              <p class="text-base text-gray-700 sm:text-lg font-medium">Today's Sales</p>
              <p class="text-2xl font-bold text-gray-900 sm:text-3xl">R 1,245</p>
              <p class="text-base text-green-600 sm:text-lg font-semibold">+15% from yesterday</p>
            </div>
            <div class="p-3 bg-green-100 rounded-full sm:p-4">
              <CurrencyDollarIcon class="w-7 h-7 text-green-600 sm:w-8 sm:h-8" />
            </div>
          </div>
        </div>

        <div class="p-5 bg-white rounded-xl border-2 border-gray-200 shadow-sm sm:p-6 hover:border-blue-500 transition-colors">
          <div class="flex justify-between items-center">
            <div>
              <p class="text-base text-gray-700 sm:text-lg font-medium">Stock Items</p>
              <p class="text-2xl font-bold text-gray-900 sm:text-3xl">156</p>
              <p class="text-base text-orange-600 sm:text-lg font-semibold">12 items low</p>
            </div>
            <div class="p-3 bg-blue-100 rounded-full sm:p-4">
              <ArchiveBoxIcon class="w-7 h-7 text-blue-600 sm:w-8 sm:h-8" />
            </div>
          </div>
        </div>

        <div class="p-5 bg-white rounded-xl border-2 border-gray-200 shadow-sm sm:p-6 hover:border-blue-500 transition-colors">
          <div class="flex justify-between items-center">
            <div>
              <p class="text-base text-gray-700 sm:text-lg font-medium">Pending Orders</p>
              <p class="text-2xl font-bold text-gray-900 sm:text-3xl">3</p>
              <p class="text-base text-yellow-600 sm:text-lg font-semibold">1 arrives today</p>
            </div>
            <div class="p-3 bg-yellow-100 rounded-full sm:p-4">
              <ShoppingCartIcon class="w-7 h-7 text-yellow-600 sm:w-8 sm:h-8" />
            </div>
          </div>
        </div>

        <div class="p-5 bg-white rounded-xl border-2 border-gray-200 shadow-sm sm:p-6 hover:border-blue-500 transition-colors">
          <div class="flex justify-between items-center">
            <div>
              <p class="text-base text-gray-700 sm:text-lg font-medium">This Month</p>
              <p class="text-2xl font-bold text-gray-900 sm:text-3xl">R 28.5K</p>
              <p class="text-base text-purple-600 sm:text-lg font-semibold">Best month yet!</p>
            </div>
            <div class="p-3 bg-purple-100 rounded-full sm:p-4">
              <ArrowTrendingUpIcon class="w-7 h-7 text-purple-600 sm:w-8 sm:h-8" />
            </div>
          </div>
        </div>
      </div>

      <!-- Recent Delivery Timeline -->
      <div class="lg:col-span-2">
        <DeliveryTimeline />
      </div>
      
      <!-- Main Content Grid - Mobile Responsive -->
      <div class="grid grid-cols-1 gap-4 lg:grid-cols-3 sm:gap-6">
        <!-- Recent Activity -->
        <div class="lg:col-span-2">
          <div class="bg-white rounded-xl border-2 border-gray-200 shadow-sm">
            <div class="p-4 border-b-2 border-gray-200 sm:p-6">
              <h3 class="text-lg font-bold text-gray-900 sm:text-xl">What's Happening</h3>
            </div>
            <div class="p-4 sm:p-6">
              <div class="space-y-4 sm:space-y-5">
                <div v-for="activity in recentActivities" :key="activity.id" class="flex items-start space-x-4 sm:space-x-5 p-3 rounded-lg hover:bg-gray-50 transition-colors">
                  <div class="flex justify-center items-center w-10 h-10 rounded-full sm:w-12 sm:h-12" :class="activity.color">
                    <component :is="activity.icon" class="w-5 h-5 text-white sm:w-6 sm:h-6" />
                  </div>
                  <div class="flex-1 min-w-0">
                    <p class="text-base font-semibold text-gray-900 sm:text-lg">{{ activity.title }}</p>
                    <p class="text-sm text-gray-700 sm:text-base">{{ activity.description }}</p>
                    <p class="mt-1 text-sm text-gray-500">{{ formatDate(activity.date) }}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

      <!-- Quick Actions - Enhanced for Low-Tech Users -->
      <div>
        <div class="bg-white rounded-xl border-2 border-gray-200 shadow-sm">
          <div class="p-4 border-b-2 border-gray-200 sm:p-6">
            <h3 class="text-xl font-bold text-gray-900 sm:text-2xl">What would you like to do?</h3>
            <p class="text-base text-gray-600 mt-1">Choose an action below</p>
          </div>
          <div class="p-4 sm:p-6">
            <div class="space-y-3 sm:space-y-4">
              <!-- Order Stock - Most Important Action -->
              <NuxtLink to="/stock/order" class="block p-6 w-full text-left rounded-xl border-3 transition-all border-blue-300 bg-blue-50 hover:border-blue-600 hover:bg-blue-100 hover:shadow-lg touch-manipulation min-h-[80px] active:scale-[0.98]">
                <div class="flex items-center space-x-4">
                  <div class="p-3 bg-blue-600 rounded-xl shadow-md">
                    <ShoppingCartIcon class="flex-shrink-0 w-8 h-8 text-white" />
                  </div>
                  <div class="min-w-0">
                    <p class="text-xl font-bold text-gray-900 mb-1">Order Stock</p>
                    <p class="text-base text-gray-700 font-medium">Browse products and place a new order</p>
                  </div>
                  <svg class="w-6 h-6 text-blue-600 ml-auto flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="3" d="M9 5l7 7-7 7" />
                  </svg>
                </div>
              </NuxtLink>

              <!-- Track Orders - Important -->
              <NuxtLink to="/stock/track" class="block p-6 w-full text-left rounded-xl border-3 transition-all border-green-300 bg-green-50 hover:border-green-600 hover:bg-green-100 hover:shadow-lg touch-manipulation min-h-[80px] active:scale-[0.98]">
                <div class="flex items-center space-x-4">
                  <div class="p-3 bg-green-600 rounded-xl shadow-md">
                    <svg class="w-8 h-8 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                    </svg>
                  </div>
                  <div class="min-w-0">
                    <p class="text-xl font-bold text-gray-900 mb-1">Track My Orders</p>
                    <p class="text-base text-gray-700 font-medium">See where your delivery is</p>
                  </div>
                  <svg class="w-6 h-6 text-green-600 ml-auto flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="3" d="M9 5l7 7-7 7" />
                  </svg>
                </div>
              </NuxtLink>

              <!-- My Stock -->
              <NuxtLink to="/stock" class="block p-5 w-full text-left rounded-xl border-2 transition-all border-gray-200 hover:border-purple-500 hover:bg-purple-50 hover:shadow-md touch-manipulation min-h-[72px] active:scale-[0.98]">
                <div class="flex items-center space-x-4">
                  <div class="p-2 bg-purple-500 rounded-lg">
                    <ArchiveBoxIcon class="flex-shrink-0 w-7 h-7 text-white" />
                  </div>
                  <div class="min-w-0">
                    <p class="text-lg font-bold text-gray-900">My Stock</p>
                    <p class="text-base text-gray-600">View what you have in your shop</p>
                  </div>
                  <svg class="w-5 h-5 text-gray-400 ml-auto flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                  </svg>
                </div>
              </NuxtLink>

              <!-- Group Buying -->
              <NuxtLink to="/group-buying" class="block p-5 w-full text-left rounded-xl border-2 transition-all border-gray-200 hover:border-yellow-500 hover:bg-yellow-50 hover:shadow-md touch-manipulation min-h-[72px] active:scale-[0.98]">
                <div class="flex items-center space-x-4">
                  <div class="p-2 bg-yellow-500 rounded-lg">
                    <UsersIcon class="flex-shrink-0 w-7 h-7 text-white" />
                  </div>
                  <div class="min-w-0">
                    <p class="text-lg font-bold text-gray-900">Group Buying</p>
                    <p class="text-base text-gray-600">Order with neighbors and save money</p>
                  </div>
                  <svg class="w-5 h-5 text-gray-400 ml-auto flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
                  </svg>
                </div>
              </NuxtLink>
            </div>
          </div>
        </div>
      </div>
      </div>
      
      <!-- Need Help? Support Section - NEW! -->
      <div class="bg-gradient-to-r from-green-50 to-blue-50 rounded-xl border-2 border-green-200 p-5 mb-4 shadow-sm">
        <div class="text-center space-y-3">
          <div class="flex justify-center">
            <div class="w-16 h-16 bg-green-500 rounded-full flex items-center justify-center shadow-lg">
              <svg class="w-8 h-8 text-white" fill="currentColor" viewBox="0 0 24 24">
                <path d="M12 2C6.48 2 2 6.48 2 12c0 1.75.46 3.39 1.26 4.82L2 22l5.25-1.24C8.68 21.54 10.32 22 12 22c5.52 0 10-4.48 10-10S17.52 2 12 2zm0 18c-1.38 0-2.69-.28-3.89-.78l-.28-.14-2.89.68.68-2.89-.14-.28C4.78 14.69 4.5 13.38 4.5 12c0-4.14 3.36-7.5 7.5-7.5s7.5 3.36 7.5 7.5-3.36 7.5-7.5 7.5z"/>
                <path d="M15.75 13.5c-.2-.1-1.2-.6-1.38-.67-.19-.07-.33-.1-.47.1-.14.2-.53.67-.65.81-.12.14-.24.15-.44.05-.2-.1-.84-.31-1.6-.99-.59-.53-1-.

1.18-1.11-1.39-.11-.2 0-.31.09-.41.09-.1.2-.23.3-.35.1-.12.13-.2.2-.33.07-.13.03-.25-.02-.35-.05-.1-.47-1.13-.65-1.55-.17-.41-.35-.35-.47-.35h-.41c-.14 0-.36.05-.55.25-.19.2-.73.71-.73 1.74s.75 2.02.85 2.16c.1.14 1.44 2.2 3.49 3.08.49.21.87.33 1.17.43.49.16.94.13 1.29.08.39-.06 1.2-.49 1.37-.96.17-.47.17-.88.12-.96-.05-.09-.19-.14-.39-.24z"/>
              </svg>
            </div>
          </div>
          <div>
            <h3 class="text-xl font-bold text-gray-900 mb-2">Need Help?</h3>
            <p class="text-base text-gray-700 mb-3">
              Our team is here to help you! Chat with us on WhatsApp or call us.
            </p>
          </div>
          <div class="flex flex-col sm:flex-row gap-3 justify-center">
            <a 
              href="https://wa.me/27123456789?text=Hi!%20I%20need%20help%20with%20my%20TOSS%20order." 
              target="_blank"
              class="flex items-center justify-center gap-2 px-6 py-4 bg-green-500 text-white rounded-xl font-bold text-lg shadow-lg hover:bg-green-600 hover:shadow-xl transform hover:scale-105 transition-all touch-manipulation min-h-[56px]"
            >
              <svg class="w-6 h-6" fill="currentColor" viewBox="0 0 24 24">
                <path d="M12 2C6.48 2 2 6.48 2 12c0 1.75.46 3.39 1.26 4.82L2 22l5.25-1.24C8.68 21.54 10.32 22 12 22c5.52 0 10-4.48 10-10S17.52 2 12 2z"/>
              </svg>
              <span>Chat on WhatsApp</span>
            </a>
            <a 
              href="tel:+27123456789"
              class="flex items-center justify-center gap-2 px-6 py-4 bg-blue-500 text-white rounded-xl font-bold text-lg shadow-lg hover:bg-blue-600 hover:shadow-xl transform hover:scale-105 transition-all touch-manipulation min-h-[56px]"
            >
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
              </svg>
              <span>Call Us</span>
            </a>
          </div>
          <p class="text-sm text-gray-600">
            📞 <strong>0123 456 789</strong> • Available Monday-Saturday, 8am-6pm
          </p>
        </div>
      </div>
      
      <!-- WhatsApp Support Button -->
      <WhatsAppSupport />

      <!-- Charts and Analytics - Mobile Responsive -->
      <div class="grid grid-cols-1 gap-4 lg:grid-cols-2 sm:gap-6">
        <!-- Revenue Chart -->
        <div class="bg-white rounded-xl border border-gray-200 shadow-sm">
          <div class="p-4 border-b border-gray-200 sm:p-6">
            <h3 class="text-base font-semibold text-gray-900 sm:text-lg">Revenue Trend</h3>
          </div>
          <div class="p-4 sm:p-6">
            <div class="flex justify-center items-center h-48 rounded-lg sm:h-64 bg-slate-50 dark:bg-slate-700">
              <p class="text-sm text-slate-500 dark:text-slate-400">Revenue chart would be displayed here</p>
            </div>
          </div>
        </div>

        <!-- Sales Funnel -->
        <div class="bg-white rounded-xl border border-gray-200 shadow-sm">
          <div class="p-4 border-b border-gray-200 sm:p-6">
            <h3 class="text-base font-semibold text-gray-900 sm:text-lg">Sales Funnel</h3>
          </div>
          <div class="p-4 sm:p-6">
            <div class="space-y-3 sm:space-y-4">
              <div v-for="stage in salesFunnel" :key="stage.name" class="flex justify-between items-center">
                <div class="flex items-center space-x-2 min-w-0 sm:space-x-3">
                  <div class="flex-shrink-0 w-3 h-3 rounded-full sm:w-4 sm:h-4" :class="stage.color"></div>
                  <span class="text-sm font-medium text-gray-900 truncate">{{ stage.name }}</span>
                </div>
                <div class="flex flex-shrink-0 items-center space-x-2">
                  <span class="text-sm text-gray-600">{{ stage.count }}</span>
                  <div class="w-16 h-2 rounded-full sm:w-20 bg-slate-200 dark:bg-slate-600">
                    <div class="h-2 rounded-full transition-all duration-300" :class="stage.color" :style="{ width: stage.percentage + '%' }"></div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { 
  CurrencyDollarIcon, 
  UsersIcon, 
  BriefcaseIcon, 
  ArrowTrendingUpIcon,
  ArchiveBoxIcon,
  PhoneIcon,
  EnvelopeIcon,
  DocumentIcon,
  ShoppingCartIcon
} from '@heroicons/vue/24/outline'

// Sample data
const recentActivities = ref([
  {
    id: 1,
    title: 'Stock Order Delivered',
    description: 'Your order of bread, milk, and chips arrived',
    date: new Date(),
    icon: ShoppingCartIcon,
    color: 'bg-green-500'
  },
  {
    id: 2,
    title: 'Low Stock Alert',
    description: 'Cool drinks running low - only 12 bottles left',
    date: new Date(Date.now() - 3600000),
    icon: ArchiveBoxIcon,
    color: 'bg-orange-500'
  },
  {
    id: 3,
    title: 'Group Order Ready',
    description: 'Join group order for sugar - closing in 2 days',
    date: new Date(Date.now() - 7200000),
    icon: UsersIcon,
    color: 'bg-purple-500'
  },
  {
    id: 4,
    title: 'Payment Received',
    description: 'R245 payment received from WhatsApp order',
    date: new Date(Date.now() - 86400000),
    icon: CurrencyDollarIcon,
    color: 'bg-blue-500'
  }
])

const salesFunnel = ref([
  { name: 'Leads', count: 124, percentage: 100, color: 'bg-blue-500' },
  { name: 'Qualified', count: 87, percentage: 70, color: 'bg-green-500' },
  { name: 'Proposal', count: 45, percentage: 36, color: 'bg-yellow-500' },
  { name: 'Negotiation', count: 23, percentage: 19, color: 'bg-orange-500' },
  { name: 'Closed Won', count: 12, percentage: 10, color: 'bg-purple-500' }
])

// Helper functions 
function formatDate(date: Date): string {
  return new Intl.DateTimeFormat('en-ZA', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(date)
}
</script>

