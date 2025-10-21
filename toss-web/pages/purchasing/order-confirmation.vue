<template>
  <div class="min-h-screen bg-gray-50 pb-20">
    <div class="p-4 space-y-4 sm:p-6 sm:space-y-6">
      <!-- Success Animation / Icon -->
      <div class="text-center py-8">
        <div class="w-24 h-24 bg-green-500 rounded-full flex items-center justify-center mx-auto mb-4 shadow-lg animate-bounce">
          <svg class="w-14 h-14 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="3" d="M5 13l4 4L19 7" />
          </svg>
        </div>
        <h1 class="text-3xl font-bold text-gray-900 mb-2">Order Placed Successfully!</h1>
        <p class="text-lg text-gray-700">Your stock is on its way to your shop</p>
      </div>
      
      <!-- Order Number Card -->
      <div class="bg-gradient-to-r from-blue-50 to-purple-50 rounded-xl border-2 border-blue-200 p-6 shadow-sm">
        <div class="text-center">
          <p class="text-base text-gray-700 mb-2">Your Order Number</p>
          <div class="inline-block bg-white rounded-lg px-6 py-3 border-2 border-blue-300 shadow-md">
            <p class="text-3xl font-bold text-blue-600">#{{ orderNumber }}</p>
          </div>
          <p class="text-sm text-gray-600 mt-3">Save this number to track your order</p>
        </div>
      </div>
      
      <!-- Order Summary -->
      <div class="bg-white rounded-xl border-2 border-gray-200 p-5 shadow-sm">
        <h2 class="text-xl font-bold text-gray-900 mb-4">Order Summary</h2>
        <div class="space-y-2 mb-4">
          <div 
            v-for="(item, index) in orderItems"
            :key="index"
            class="flex justify-between items-center bg-gray-50 rounded-lg p-3"
          >
            <div class="flex-1 min-w-0">
              <div class="flex items-center justify-between">
                <div>
                  <p class="font-bold text-base text-gray-900">{{ item.name }}</p>
                  <p class="text-sm text-gray-600">{{ item.sku }}</p>
                </div>
                <div class="text-right ml-4">
                  <p class="text-sm text-gray-600">Qty: {{ item.quantity }}</p>
                  <p class="font-bold text-base text-gray-900">R {{ (item.price * item.quantity).toFixed(2) }}</p>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="border-t-2 border-gray-200 pt-4 space-y-2">
          <div class="flex justify-between text-sm">
            <span class="text-gray-600">Subtotal:</span>
            <span class="font-medium text-gray-900">R {{ subtotal.toFixed(2) }}</span>
          </div>
          <div class="flex justify-between text-sm">
            <span class="text-gray-600">Delivery Fee:</span>
            <span class="font-medium text-gray-900">R {{ deliveryFee.toFixed(2) }}</span>
          </div>
          <div class="flex justify-between items-center pt-2 border-t-2 border-gray-200">
            <span class="text-lg font-bold text-gray-900">Total Paid:</span>
            <span class="text-2xl font-bold text-blue-600">R {{ totalPrice.toFixed(2) }}</span>
          </div>
        </div>
      </div>
      
      <!-- What Happens Next - Clear Next Steps -->
      <div class="bg-gradient-to-r from-green-50 to-blue-50 rounded-xl border-2 border-green-200 p-6 shadow-sm">
        <div class="flex items-center gap-2 mb-4">
          <div class="w-8 h-8 bg-green-500 rounded-full flex items-center justify-center">
            <svg class="w-5 h-5 text-white" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd" />
            </svg>
          </div>
          <h2 class="text-xl font-bold text-gray-900">What Happens Next?</h2>
        </div>
        
        <div class="space-y-4">
          <!-- Step 1 -->
          <div class="flex gap-3">
            <div class="flex-shrink-0 w-8 h-8 bg-blue-600 text-white rounded-full flex items-center justify-center font-bold">
              1
            </div>
            <div>
              <p class="font-bold text-base text-gray-900 mb-1">We prepare your order</p>
              <p class="text-base text-gray-700">We're packing your stock carefully and getting it ready for delivery.</p>
            </div>
          </div>
          
          <!-- Step 2 -->
          <div class="flex gap-3">
            <div class="flex-shrink-0 w-8 h-8 bg-blue-600 text-white rounded-full flex items-center justify-center font-bold">
              2
            </div>
            <div>
              <p class="font-bold text-base text-gray-900 mb-1">You get a notification</p>
              <p class="text-base text-gray-700">We'll send you a WhatsApp message when your order is on the way.</p>
            </div>
          </div>
          
          <!-- Step 3 -->
          <div class="flex gap-3">
            <div class="flex-shrink-0 w-8 h-8 bg-blue-600 text-white rounded-full flex items-center justify-center font-bold">
              3
            </div>
            <div>
              <p class="font-bold text-base text-gray-900 mb-1">Delivery to your shop</p>
              <p class="text-base text-gray-700">Your stock arrives at your shop within 24-48 hours.</p>
            </div>
          </div>
        </div>
        
        <!-- Estimated Delivery -->
        <div class="mt-4 p-4 bg-white rounded-lg border-2 border-green-300">
          <div class="flex items-center gap-2">
            <svg class="w-6 h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
            </svg>
            <div>
              <p class="text-sm text-gray-600">Expected Delivery:</p>
              <p class="font-bold text-lg text-gray-900">{{ estimatedDelivery }}</p>
            </div>
          </div>
        </div>
      </div>
      
      <!-- Action Buttons -->
      <div class="space-y-3">
        <!-- Track Order - Primary Action -->
        <NuxtLink 
          to="/purchasing/track-orders"
          class="w-full px-6 py-5 bg-blue-600 text-white rounded-xl font-bold text-lg shadow-lg hover:bg-blue-700 transition-all touch-manipulation min-h-[60px] flex items-center justify-center gap-2"
        >
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
          </svg>
          <span>Track My Order</span>
        </NuxtLink>
        
        <!-- WhatsApp Support -->
        <a 
          href="https://wa.me/27123456789?text=Hi!%20I%20just%20placed%20order%20"
          target="_blank"
          class="w-full px-6 py-4 bg-green-500 text-white rounded-xl font-bold text-lg shadow-lg hover:bg-green-600 transition-all touch-manipulation min-h-[56px] flex items-center justify-center gap-2"
        >
          <svg class="w-6 h-6" fill="currentColor" viewBox="0 0 24 24">
            <path d="M12 2C6.48 2 2 6.48 2 12c0 1.75.46 3.39 1.26 4.82L2 22l5.25-1.24C8.68 21.54 10.32 22 12 22c5.52 0 10-4.48 10-10S17.52 2 12 2z"/>
          </svg>
          <span>Chat on WhatsApp</span>
        </a>
        
        <!-- Back to Home -->
        <NuxtLink 
          to="/"
          class="w-full px-6 py-4 bg-gray-200 text-gray-900 rounded-xl font-bold text-lg hover:bg-gray-300 transition-all touch-manipulation min-h-[56px] flex items-center justify-center gap-2"
        >
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
          </svg>
          <span>Back to Home</span>
        </NuxtLink>
      </div>
      
      <!-- Confirmation Message -->
      <div class="bg-blue-50 rounded-lg border border-blue-200 p-4 text-center">
        <p class="text-base text-gray-700 font-medium">
          📱 A confirmation has been sent to your phone
        </p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const orderItems = ref([])
const totalPrice = ref(0)
const subtotal = ref(0)
const deliveryFee = ref(0)
const orderNumber = ref('')

const estimatedDelivery = computed(() => {
  const tomorrow = new Date()
  tomorrow.setDate(tomorrow.getDate() + 1)
  return tomorrow.toLocaleDateString('en-ZA', { 
    weekday: 'long',
    month: 'long',
    day: 'numeric'
  })
})

onMounted(() => {
  // Get order from localStorage
  const orderData = localStorage.getItem('toss-current-order')
  
  if (orderData) {
    const order = JSON.parse(orderData)
    orderItems.value = order.items || []
    subtotal.value = order.subtotal || 0
    deliveryFee.value = order.deliveryFee || 0
    totalPrice.value = order.total || 0
    orderNumber.value = order.orderNumber || 'ORD' + Date.now()
    
    // Clear the order from storage
    localStorage.removeItem('toss-current-order')
  } else {
    // No order found, redirect to create order page
    router.push('/purchasing/create-order')
  }
})
</script>

<style scoped>
@keyframes bounce {
  0%, 100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-10px);
  }
}

.animate-bounce {
  animation: bounce 1s ease-in-out 3;
}
</style>

