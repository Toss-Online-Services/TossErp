<template>
  <div class="min-h-screen bg-gray-50 pb-20">
    <div class="p-4 space-y-4 sm:p-6 sm:space-y-6">
      <!-- Header -->
      <div class="flex items-center gap-3 mb-6">
        <NuxtLink to="/" class="flex items-center justify-center w-10 h-10 bg-white rounded-lg border-2 border-gray-200 hover:border-blue-500 transition-colors touch-manipulation">
          <svg class="w-5 h-5 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
          </svg>
        </NuxtLink>
        <div>
          <h1 class="text-2xl font-bold text-gray-900">Track Your Order</h1>
          <p class="text-base text-gray-600">See where your stock is</p>
        </div>
      </div>
      
      <!-- DeliveryTimeline Component -->
      <DeliveryTimeline />
      
      <!-- Recent Orders -->
      <div class="bg-white rounded-xl border-2 border-gray-200 p-5 shadow-sm">
        <h2 class="text-xl font-bold text-gray-900 mb-4">Recent Orders</h2>
        <div class="space-y-3">
          <div class="bg-green-50 rounded-lg p-4 border-2 border-green-200">
            <div class="flex justify-between items-start mb-2">
              <div>
                <p class="font-bold text-base text-gray-900">#ORD12345</p>
                <p class="text-sm text-gray-600">Delivered 2 days ago</p>
              </div>
              <span class="px-3 py-1 bg-green-500 text-white rounded-full text-sm font-bold">
                ✓ Delivered
              </span>
            </div>
            <p class="text-base text-gray-700">Bread, Milk, Chips • R65</p>
          </div>
          
          <div class="bg-yellow-50 rounded-lg p-4 border-2 border-yellow-200">
            <div class="flex justify-between items-start mb-2">
              <div>
                <p class="font-bold text-base text-gray-900">#ORD12346</p>
                <p class="text-sm text-gray-600">Ordered today</p>
              </div>
              <span class="px-3 py-1 bg-yellow-500 text-white rounded-full text-sm font-bold">
                🚚 On The Way
              </span>
            </div>
            <p class="text-base text-gray-700">Sugar, Maize Meal, Oil • R148</p>
            <p class="text-sm text-green-600 font-medium mt-2">Arrives by 3pm today</p>
          </div>
        </div>
      </div>
      
      <!-- Help Card -->
      <div class="bg-blue-50 rounded-xl border-2 border-blue-200 p-5 shadow-sm">
        <div class="flex items-start gap-3">
          <div class="w-10 h-10 bg-blue-500 rounded-full flex items-center justify-center flex-shrink-0">
            <svg class="w-6 h-6 text-white" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd" />
            </svg>
          </div>
          <div class="min-w-0">
            <h3 class="font-bold text-lg text-gray-900 mb-2">Need help with your order?</h3>
            <p class="text-base text-gray-700 mb-3">
              If you have questions about your delivery, chat with us on WhatsApp or call us.
            </p>
            <div class="flex flex-col sm:flex-row gap-2">
              <a 
                href="https://wa.me/27123456789?text=Hi!%20I%20need%20help%20tracking%20my%20order."
                target="_blank"
                class="px-4 py-3 bg-green-500 text-white rounded-lg font-bold text-base hover:bg-green-600 transition-all touch-manipulation min-h-[48px] flex items-center justify-center gap-2"
              >
                <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 24 24">
                  <path d="M12 2C6.48 2 2 6.48 2 12c0 1.75.46 3.39 1.26 4.82L2 22l5.25-1.24C8.68 21.54 10.32 22 12 22c5.52 0 10-4.48 10-10S17.52 2 12 2z"/>
                </svg>
                <span>Chat on WhatsApp</span>
              </a>
              <a 
                href="tel:+27123456789"
                class="px-4 py-3 bg-blue-500 text-white rounded-lg font-bold text-base hover:bg-blue-600 transition-all touch-manipulation min-h-[48px] flex items-center justify-center gap-2"
              >
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
                </svg>
                <span>Call Us</span>
              </a>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// Using DeliveryTimeline component from township components
</script>

