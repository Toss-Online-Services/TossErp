<script setup lang="ts">
// Test page to verify Tailwind CSS is working
</script>

<template>
  <div class="min-h-screen bg-gray-100 p-8">
    <div class="max-w-4xl mx-auto">
      <h1 class="text-4xl font-bold text-gray-900 mb-4">
        Tailwind CSS Test Page
      </h1>
      
      <div class="bg-white rounded-lg shadow-md p-6 mb-6">
        <h2 class="text-2xl font-semibold text-gray-800 mb-4">Card Component Test</h2>
        <p class="text-gray-600 mb-4">
          If you can see this styled card, Tailwind CSS is working correctly.
        </p>
        <div class="flex gap-4">
          <button class="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded">
            Blue Button
          </button>
          <button class="bg-green-500 hover:bg-green-600 text-white px-4 py-2 rounded">
            Green Button
          </button>
        </div>
      </div>

      <div class="bg-gradient-to-r from-pink-500 to-purple-500 rounded-lg shadow-lg p-6 text-white mb-6">
        <h2 class="text-2xl font-semibold mb-4">Gradient Test</h2>
        <p>Material Dashboard Pro primary color gradient</p>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div class="bg-white rounded-lg shadow p-4">
          <h3 class="font-semibold text-gray-800 mb-2">Card 1</h3>
          <p class="text-sm text-gray-600">Grid layout test</p>
        </div>
        <div class="bg-white rounded-lg shadow p-4">
          <h3 class="font-semibold text-gray-800 mb-2">Card 2</h3>
          <p class="text-sm text-gray-600">Grid layout test</p>
        </div>
        <div class="bg-white rounded-lg shadow p-4">
          <h3 class="font-semibold text-gray-800 mb-2">Card 3</h3>
          <p class="text-sm text-gray-600">Grid layout test</p>
        </div>
      </div>
    </div>
  </div>
</template>

