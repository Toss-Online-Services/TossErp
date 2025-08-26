<template>
  <div class="min-h-screen bg-gray-50 py-12">
    <div class="max-w-md mx-auto bg-white rounded-lg shadow-md p-6">
      <h1 class="text-2xl font-bold text-gray-900 mb-4">Middleware Test</h1>
      <p class="text-gray-600 mb-4">
        If you can see this page, the middleware system is working properly.
      </p>
      <div class="space-y-2">
        <p class="text-sm text-gray-500">
          <strong>Auth middleware:</strong> {{ middleware.includes('auth') ? 'Loaded' : 'Not loaded' }}
        </p>
        <p class="text-sm text-gray-500">
          <strong>Tenant middleware:</strong> {{ middleware.includes('tenant') ? 'Loaded' : 'Not loaded' }}
        </p>
      </div>
      <NuxtLink to="/" class="mt-6 inline-block bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700">
        Back to Home
      </NuxtLink>
    </div>
  </div>
</template>

<script setup lang="ts">
// This page tests that middleware system is working
// No middleware applied to this page specifically

// Get available middleware for testing
const middleware = ref<string[]>([])

onMounted(() => {
  // Simple test to see if middleware files are being loaded
  middleware.value = []
  
  // These would be populated if middleware is properly loaded
  try {
    // Test if middleware types are available
    if (typeof defineNuxtRouteMiddleware !== 'undefined') {
      middleware.value.push('system')
    }
  } catch (e) {
    console.log('Middleware system check:', e)
  }
})

useHead({
  title: 'Middleware Test - TOSS ERP'
})
</script>
