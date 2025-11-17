<template>
  <Teleport to="body">
    <Transition
      enter-active-class="transition ease-out duration-300"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition ease-in duration-200"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div v-if="showOnboarding" class="fixed inset-0 z-50 overflow-y-auto">
        <!-- Backdrop -->
        <div class="fixed inset-0 bg-black/60 backdrop-blur-sm" @click="skipOnboarding"></div>
        
        <!-- Modal Content -->
        <div class="flex min-h-screen items-center justify-center p-4">
          <div class="relative w-full max-w-md bg-white rounded-2xl shadow-2xl transform">
            <!-- Progress Dots -->
            <div class="absolute top-4 left-0 right-0 flex justify-center gap-2 z-10">
              <div
                v-for="i in 3"
                :key="i"
                class="w-2 h-2 rounded-full transition-all"
                :class="currentStep === i - 1 ? 'bg-blue-600 w-8' : 'bg-gray-300'"
              ></div>
            </div>
            
            <!-- Skip Button -->
            <button
              @click="skipOnboarding"
              class="absolute top-4 right-4 text-gray-500 hover:text-gray-700 text-sm font-medium z-10 touch-manipulation"
            >
              Skip
            </button>
            
            <!-- Steps -->
            <div class="p-8 pt-16">
              <!-- Step 1: Browse Products -->
              <div v-if="currentStep === 0" class="text-center space-y-4">
                <div class="w-20 h-20 bg-blue-100 rounded-full flex items-center justify-center mx-auto">
                  <svg class="w-10 h-10 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
                  </svg>
                </div>
                <h2 class="text-2xl font-bold text-gray-900">Browse & Order Stock</h2>
                <p class="text-lg text-gray-700 leading-relaxed">
                  Look through our products and tap "Add to Order" for items you need. Simple as that!
                </p>
              </div>
              
              <!-- Step 2: Confirm Order -->
              <div v-if="currentStep === 1" class="text-center space-y-4">
                <div class="w-20 h-20 bg-green-100 rounded-full flex items-center justify-center mx-auto">
                  <svg class="w-10 h-10 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                </div>
                <h2 class="text-2xl font-bold text-gray-900">Confirm Your Order</h2>
                <p class="text-lg text-gray-700 leading-relaxed">
                  Review your order, check your delivery address, and tap "Place Order". We'll send you a confirmation.
                </p>
              </div>
              
              <!-- Step 3: Track Delivery -->
              <div v-if="currentStep === 2" class="text-center space-y-4">
                <div class="w-20 h-20 bg-purple-100 rounded-full flex items-center justify-center mx-auto">
                  <svg class="w-10 h-10 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path d="M9 17a2 2 0 11-4 0 2 2 0 014 0zM19 17a2 2 0 11-4 0 2 2 0 014 0z" />
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16V6a1 1 0 00-1-1H4a1 1 0 00-1 1v10a1 1 0 001 1h1m8-1a1 1 0 01-1 1H9m4-1V8a1 1 0 011-1h2.586a1 1 0 01.707.293l3.414 3.414a1 1 0 01.293.707V16a1 1 0 01-1 1h-1m-6-1a1 1 0 001 1h1M5 17a2 2 0 104 0m-4 0h2m2 0h2" />
                  </svg>
                </div>
                <h2 class="text-2xl font-bold text-gray-900">Track Your Delivery</h2>
                <p class="text-lg text-gray-700 leading-relaxed">
                  Watch your order move from "Preparing" to "On the Way" to "Delivered". Usually within 24-48 hours!
                </p>
              </div>
              
              <!-- Navigation Buttons -->
              <div class="flex gap-3 mt-8">
                <button
                  v-if="currentStep > 0"
                  @click="previousStep"
                  class="flex-1 px-6 py-4 bg-gray-200 text-gray-900 rounded-xl font-bold text-lg hover:bg-gray-300 transition-all touch-manipulation min-h-[56px]"
                >
                  Back
                </button>
                <button
                  @click="nextStep"
                  class="flex-1 px-6 py-4 bg-blue-600 text-white rounded-xl font-bold text-lg hover:bg-blue-700 transition-all touch-manipulation min-h-[56px] shadow-lg"
                >
                  {{ currentStep === 2 ? "Get Started" : "Next" }}
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

const showOnboarding = ref(false)
const currentStep = ref(0)

const nextStep = () => {
  if (currentStep.value < 2) {
    currentStep.value++
  } else {
    completeOnboarding()
  }
}

const previousStep = () => {
  if (currentStep.value > 0) {
    currentStep.value--
  }
}

const skipOnboarding = () => {
  showOnboarding.value = false
  localStorage.setItem('toss-onboarding-completed', 'true')
}

const completeOnboarding = () => {
  showOnboarding.value = false
  localStorage.setItem('toss-onboarding-completed', 'true')
}

onMounted(() => {
  // Check if user has already seen onboarding
  const hasSeenOnboarding = localStorage.getItem('toss-onboarding-completed')
  if (!hasSeenOnboarding) {
    // Show onboarding after a short delay
    setTimeout(() => {
      showOnboarding.value = true
    }, 1000)
  }
})
</script>

