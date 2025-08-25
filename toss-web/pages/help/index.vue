<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Header -->
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-gray-900 dark:text-white">Help & Support</h1>
        <p class="mt-2 text-gray-600 dark:text-gray-400">Get help and find answers to your questions</p>
      </div>

      <!-- Quick Help Cards -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6 hover:shadow-lg transition-shadow cursor-pointer">
          <IconBase class="h-8 w-8 text-blue-600 mb-3">
            <path d="M9.879 7.519c.892-1.54 3.17-1.54 4.062 0l.995 1.719c.196.339.574.575.995.575h2.078c1.825 0 2.5 1.775 1.021 2.683l-1.677 1.028c-.313.192-.503.535-.503.908 0 .374.19.716.503.908l1.677 1.028c1.479.908.804 2.683-1.021 2.683h-2.078c-.421 0-.799.236-.995.575l-.995 1.719c-.892 1.54-3.17 1.54-4.062 0l-.995-1.719c-.196-.339-.574-.575-.995-.575H4.87c-1.825 0-2.5-1.775-1.021-2.683l1.677-1.028c.313-.192.503-.535.503-.908 0-.374-.19-.716-.503-.908L3.849 11.48c-1.479-.908-.804-2.683 1.021-2.683h2.078c.421 0 .799-.236.995-.575l.995-1.719Z" />
          </IconBase>
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Getting Started</h3>
          <p class="text-gray-600 dark:text-gray-400">Learn the basics of TOSS ERP and set up your business</p>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6 hover:shadow-lg transition-shadow cursor-pointer">
          <IconBase class="h-8 w-8 text-green-600 mb-3">
            <path d="M7.5 3.375c0-1.036.84-1.875 1.875-1.875S11.25 2.34 11.25 3.375v.75c0 1.036-.84 1.875-1.875 1.875S7.5 5.16 7.5 4.125v-.75Zm3.75 11.25c0 1.036-.84 1.875-1.875 1.875S7.5 15.66 7.5 14.625v-.75c0-1.036.84-1.875 1.875-1.875s1.875.84 1.875 1.875v.75Z" />
          </IconBase>
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Video Tutorials</h3>
          <p class="text-gray-600 dark:text-gray-400">Watch step-by-step guides for all major features</p>
        </div>

        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6 hover:shadow-lg transition-shadow cursor-pointer">
          <IconBase class="h-8 w-8 text-purple-600 mb-3">
            <path d="M8.625 9.75a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Zm0 0H8.25m4.125 0a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Zm0 0H12m4.125 0a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Zm0 0h-.375M21 12c0 4.556-4.03 8.25-9 8.25a9.764 9.764 0 0 1-2.555-.337A5.972 5.972 0 0 1 5.41 20.97a5.969 5.969 0 0 1-.474-.065 4.48 4.48 0 0 0 .978-2.025c.09-.457-.133-.901-.467-1.226C3.93 16.178 3 14.189 3 12c0-4.556 4.03-8.25 9-8.25s9 3.694 9 8.25Z" />
          </IconBase>
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Live Chat</h3>
          <p class="text-gray-600 dark:text-gray-400">Chat with our support team in real-time</p>
        </div>
      </div>

      <!-- FAQ Section -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6 mb-8">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Frequently Asked Questions</h3>
        <div class="space-y-4">
          <details class="group">
            <summary class="flex justify-between items-center cursor-pointer list-none">
              <span class="text-gray-700 dark:text-gray-300">How do I set up my first business on TOSS ERP?</span>
              <IconBase class="h-5 w-5 text-gray-400 group-open:rotate-180 transition-transform">
                <path d="M5.23 7.21a.75.75 0 011.06.02L10 11.168l3.71-3.938a.75.75 0 111.08 1.04l-4.25 4.5a.75.75 0 01-1.08 0l-4.25-4.5a.75.75 0 01.02-1.06z" />
              </IconBase>
            </summary>
            <div class="mt-2 text-gray-600 dark:text-gray-400">
              After signing up, use the Getting Started guide to configure your business profile, add your products or services, and set up your first customers.
            </div>
          </details>

          <details class="group">
            <summary class="flex justify-between items-center cursor-pointer list-none">
              <span class="text-gray-700 dark:text-gray-300">How does the AI Copilot work?</span>
              <IconBase class="h-5 w-5 text-gray-400 group-open:rotate-180 transition-transform">
                <path d="M5.23 7.21a.75.75 0 011.06.02L10 11.168l3.71-3.938a.75.75 0 111.08 1.04l-4.25 4.5a.75.75 0 01-1.08 0l-4.25-4.5a.75.75 0 01.02-1.06z" />
              </IconBase>
            </summary>
            <div class="mt-2 text-gray-600 dark:text-gray-400">
              The AI Copilot analyzes your business data to provide intelligent suggestions, automate routine tasks, and help you make better business decisions.
            </div>
          </details>

          <details class="group">
            <summary class="flex justify-between items-center cursor-pointer list-none">
              <span class="text-gray-700 dark:text-gray-300">What is Group Buying and how can it benefit my business?</span>
              <IconBase class="h-5 w-5 text-gray-400 group-open:rotate-180 transition-transform">
                <path d="M5.23 7.21a.75.75 0 011.06.02L10 11.168l3.71-3.938a.75.75 0 111.08 1.04l-4.25 4.5a.75.75 0 01-1.08 0l-4.25-4.5a.75.75 0 01.02-1.06z" />
              </IconBase>
            </summary>
            <div class="mt-2 text-gray-600 dark:text-gray-400">
              Group Buying allows multiple businesses to combine their purchasing power to get better prices from suppliers. You can join existing group purchases or start your own.
            </div>
          </details>
        </div>
      </div>

      <!-- Contact Support -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-4">Contact Support</h3>
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <div>
            <h4 class="font-medium text-gray-900 dark:text-white mb-2">Email Support</h4>
            <p class="text-gray-600 dark:text-gray-400 mb-2">support@tosserp.co.za</p>
            <p class="text-sm text-gray-500 dark:text-gray-500">Response within 24 hours</p>
          </div>
          <div>
            <h4 class="font-medium text-gray-900 dark:text-white mb-2">Phone Support</h4>
            <p class="text-gray-600 dark:text-gray-400 mb-2">+27 11 123 4567</p>
            <p class="text-sm text-gray-500 dark:text-gray-500">Mon-Fri, 8AM-5PM SAST</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
useHead({
  title: 'Help & Support - TOSS ERP III'
})
</script>
