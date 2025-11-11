<template>
  <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden">
    <!-- Header -->
    <div class="bg-gradient-to-r from-green-500 to-green-600 px-6 py-4">
      <div class="flex items-center space-x-3">
        <div class="w-10 h-10 bg-white rounded-full flex items-center justify-center">
          <svg class="w-6 h-6 text-green-600" fill="currentColor" viewBox="0 0 24 24">
            <path d="M17.472 14.382c-.297-.149-1.758-.867-2.03-.967-.273-.099-.471-.148-.67.15-.197.297-.767.966-.94 1.164-.173.199-.347.223-.644.075-.297-.15-1.255-.463-2.39-1.475-.883-.788-1.48-1.761-1.653-2.059-.173-.297-.018-.458.13-.606.134-.133.298-.347.446-.52.149-.174.198-.298.298-.497.099-.198.05-.371-.025-.52-.075-.149-.669-1.612-.916-2.207-.242-.579-.487-.5-.669-.51-.173-.008-.371-.01-.57-.01-.198 0-.52.074-.792.372-.272.297-1.04 1.016-1.04 2.479 0 1.462 1.065 2.875 1.213 3.074.149.198 2.096 3.2 5.077 4.487.709.306 1.262.489 1.694.625.712.227 1.36.195 1.871.118.571-.085 1.758-.719 2.006-1.413.248-.694.248-1.289.173-1.413-.074-.124-.272-.198-.57-.347m-5.421 7.403h-.004a9.87 9.87 0 01-5.031-1.378l-.361-.214-3.741.982.998-3.648-.235-.374a9.86 9.86 0 01-1.51-5.26c.001-5.45 4.436-9.884 9.888-9.884 2.64 0 5.122 1.03 6.988 2.898a9.825 9.825 0 012.893 6.994c-.003 5.45-4.437 9.884-9.885 9.884m8.413-18.297A11.815 11.815 0 0012.05 0C5.495 0 .16 5.335.157 11.892c0 2.096.547 4.142 1.588 5.945L.057 24l6.305-1.654a11.882 11.882 0 005.683 1.448h.005c6.554 0 11.89-5.335 11.893-11.893a11.821 11.821 0 00-3.48-8.413z"/>
          </svg>
        </div>
        <div class="text-white">
          <h3 class="font-semibold text-lg">Order via WhatsApp</h3>
          <p class="text-sm text-green-100">Fast, easy, familiar</p>
        </div>
      </div>
    </div>

    <!-- Mock Chat Interface -->
    <div class="p-6 space-y-4">
      <!-- Feature Preview -->
      <div class="bg-gray-50 dark:bg-gray-700 rounded-lg p-4">
        <p class="text-sm text-gray-600 dark:text-gray-400 mb-3">
          <strong>How it works:</strong>
        </p>
        <ol class="space-y-2 text-sm text-gray-700 dark:text-gray-300">
          <li class="flex items-start">
            <span class="inline-flex items-center justify-center w-5 h-5 rounded-full bg-green-500 text-white text-xs mr-2 flex-shrink-0">1</span>
            <span>Send your order via WhatsApp message</span>
          </li>
          <li class="flex items-start">
            <span class="inline-flex items-center justify-center w-5 h-5 rounded-full bg-green-500 text-white text-xs mr-2 flex-shrink-0">2</span>
            <span>Get instant confirmation and pricing</span>
          </li>
          <li class="flex items-start">
            <span class="inline-flex items-center justify-center w-5 h-5 rounded-full bg-green-500 text-white text-xs mr-2 flex-shrink-0">3</span>
            <span>Track delivery in real-time</span>
          </li>
          <li class="flex items-start">
            <span class="inline-flex items-center justify-center w-5 h-5 rounded-full bg-green-500 text-white text-xs mr-2 flex-shrink-0">4</span>
            <span>Pay on delivery or via mobile money</span>
          </li>
        </ol>
      </div>

      <!-- Example Messages -->
      <div class="space-y-3">
        <p class="text-xs text-gray-500 dark:text-gray-500 font-medium uppercase">Example Chat:</p>
        
        <!-- User Message -->
        <div class="flex justify-end">
          <div class="max-w-xs bg-green-500 text-white rounded-lg px-4 py-2">
            <p class="text-sm">Hi, I need 10 bread, 5 Coca Cola 2L</p>
            <p class="text-xs text-green-100 mt-1 text-right">10:30 AM</p>
          </div>
        </div>

        <!-- Bot Response -->
        <div class="flex justify-start">
          <div class="max-w-xs bg-gray-100 dark:bg-gray-700 rounded-lg px-4 py-2">
            <p class="text-sm text-gray-900 dark:text-white">
              Great! I found:<br>
              • 10x White Bread 700g - R180<br>
              • 5x Coca Cola 2L - R175<br>
              <br>
              <strong>Total: R355</strong><br>
              Delivery available today. Confirm order?
            </p>
            <p class="text-xs text-gray-500 mt-1">10:30 AM</p>
          </div>
        </div>

        <!-- User Confirmation -->
        <div class="flex justify-end">
          <div class="max-w-xs bg-green-500 text-white rounded-lg px-4 py-2">
            <p class="text-sm">Yes, confirm!</p>
            <p class="text-xs text-green-100 mt-1 text-right">10:31 AM</p>
          </div>
        </div>
      </div>

      <!-- Coming Soon Badge -->
      <div class="mt-6 text-center">
        <span class="inline-flex items-center px-4 py-2 rounded-full text-sm font-medium bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400">
          <svg class="w-4 h-4 mr-2" fill="currentColor" viewBox="0 0 20 20">
            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-12a1 1 0 10-2 0v4a1 1 0 00.293.707l2.828 2.829a1 1 0 101.415-1.415L11 9.586V6z" clip-rule="evenodd"/>
          </svg>
          Feature in Development
        </span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
// WhatsApp ordering interface placeholder
</script>

