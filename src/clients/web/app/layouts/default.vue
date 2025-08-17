<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Navigation Header -->
    <nav class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16">
          <!-- Logo and Brand -->
          <div class="flex items-center">
            <div class="flex-shrink-0 flex items-center">
              <div class="h-8 w-8 bg-blue-600 rounded-lg flex items-center justify-center mr-3">
                <span class="text-white font-bold text-sm">T</span>
              </div>
              <h1 class="text-xl font-bold text-gray-900 dark:text-white">
                TOSS ERP
              </h1>
            </div>
            
            <!-- Desktop Navigation -->
            <div class="hidden md:ml-6 md:flex md:space-x-8">
              <NuxtLink
                to="/dashboard"
                class="border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 dark:text-gray-300 dark:hover:text-white whitespace-nowrap py-2 px-1 border-b-2 font-medium text-sm transition-colors"
                active-class="border-blue-500 text-blue-600 dark:text-blue-400"
              >
                Dashboard
              </NuxtLink>
              <NuxtLink
                to="/stock"
                class="border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 dark:text-gray-300 dark:hover:text-white whitespace-nowrap py-2 px-1 border-b-2 font-medium text-sm transition-colors"
                active-class="border-blue-500 text-blue-600 dark:text-blue-400"
              >
                Stock Management
              </NuxtLink>
              <NuxtLink
                to="/sales"
                class="border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 dark:text-gray-300 dark:hover:text-white whitespace-nowrap py-2 px-1 border-b-2 font-medium text-sm transition-colors"
                active-class="border-blue-500 text-blue-600 dark:text-blue-400"
              >
                Sales & POS
              </NuxtLink>
              <NuxtLink
                to="/finance"
                class="border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 dark:text-gray-300 dark:hover:text-white whitespace-nowrap py-2 px-1 border-b-2 font-medium text-sm transition-colors"
                active-class="border-blue-500 text-blue-600 dark:text-blue-400"
              >
                Finance
              </NuxtLink>
              <NuxtLink
                to="/collaboration"
                class="border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 dark:text-gray-300 dark:hover:text-white whitespace-nowrap py-2 px-1 border-b-2 font-medium text-sm transition-colors"
                active-class="border-blue-500 text-blue-600 dark:text-blue-400"
              >
                Collaboration
              </NuxtLink>
            </div>
          </div>

          <!-- Right side actions -->
          <div class="flex items-center space-x-4">
            <!-- AI Assistant Toggle -->
            <button
              @click="toggleAIAssistant"
              class="p-2 rounded-md text-gray-500 hover:text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:text-white dark:hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
              title="AI Assistant"
            >
              <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
              </svg>
            </button>

            <!-- Notifications -->
            <button
              class="p-2 rounded-md text-gray-500 hover:text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:text-white dark:hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-blue-500 relative"
              title="Notifications"
            >
              <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-5-5 5-5h-5m-6 0H4l5 5-5 5h5" />
              </svg>
              <span class="absolute top-0 right-0 block h-2 w-2 rounded-full ring-2 ring-white bg-red-500"></span>
            </button>

            <!-- Color mode toggle -->
            <button
              @click="toggleColorMode"
              class="p-2 rounded-md text-gray-500 hover:text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:text-white dark:hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
              title="Toggle dark mode"
            >
              <svg v-if="colorMode.value === 'dark'" class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z" />
              </svg>
              <svg v-else class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z" />
              </svg>
            </button>

            <!-- User menu -->
            <div class="relative">
              <button
                @click="toggleUserMenu"
                class="flex items-center text-sm rounded-full focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <img class="h-8 w-8 rounded-full" src="https://ui-avatars.com/api/?name=User&background=2563eb&color=fff" alt="User avatar" />
              </button>
              
              <!-- User dropdown menu -->
              <div
                v-show="showUserMenu"
                class="absolute right-0 mt-2 w-48 bg-white dark:bg-gray-800 rounded-md shadow-lg py-1 z-50 border border-gray-200 dark:border-gray-700"
              >
                <a href="#" class="block px-4 py-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700">Profile</a>
                <a href="#" class="block px-4 py-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700">Settings</a>
                <a href="#" class="block px-4 py-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700">Help</a>
                <hr class="my-1 border-gray-200 dark:border-gray-700">
                <a href="#" class="block px-4 py-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700">Sign out</a>
              </div>
            </div>

            <!-- Mobile menu button -->
            <button
              @click="toggleMobileMenu"
              class="md:hidden p-2 rounded-md text-gray-500 hover:text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:text-white dark:hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              <svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path v-if="!showMobileMenu" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
                <path v-else stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>
        </div>
      </div>

      <!-- Mobile menu -->
      <div v-show="showMobileMenu" class="md:hidden border-t border-gray-200 dark:border-gray-700">
        <div class="px-2 pt-2 pb-3 space-y-1">
          <NuxtLink
            to="/dashboard"
            class="block px-3 py-2 rounded-md text-base font-medium text-gray-500 hover:text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:text-white dark:hover:bg-gray-700"
            active-class="text-blue-600 bg-blue-50 dark:text-blue-400 dark:bg-blue-900"
          >
            Dashboard
          </NuxtLink>
          <NuxtLink
            to="/stock"
            class="block px-3 py-2 rounded-md text-base font-medium text-gray-500 hover:text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:text-white dark:hover:bg-gray-700"
            active-class="text-blue-600 bg-blue-50 dark:text-blue-400 dark:bg-blue-900"
          >
            Stock Management
          </NuxtLink>
          <NuxtLink
            to="/sales"
            class="block px-3 py-2 rounded-md text-base font-medium text-gray-500 hover:text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:text-white dark:hover:bg-gray-700"
            active-class="text-blue-600 bg-blue-50 dark:text-blue-400 dark:bg-blue-900"
          >
            Sales & POS
          </NuxtLink>
          <NuxtLink
            to="/finance"
            class="block px-3 py-2 rounded-md text-base font-medium text-gray-500 hover:text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:text-white dark:hover:bg-gray-700"
            active-class="text-blue-600 bg-blue-50 dark:text-blue-400 dark:bg-blue-900"
          >
            Finance
          </NuxtLink>
          <NuxtLink
            to="/collaboration"
            class="block px-3 py-2 rounded-md text-base font-medium text-gray-500 hover:text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:text-white dark:hover:bg-gray-700"
            active-class="text-blue-600 bg-blue-50 dark:text-blue-400 dark:bg-blue-900"
          >
            Collaboration
          </NuxtLink>
        </div>
      </div>
    </nav>

    <!-- Main content area -->
    <main class="flex-1">
      <slot />
    </main>

    <!-- AI Assistant Sidebar (when enabled) -->
    <div
      v-show="showAIAssistant"
      class="fixed inset-y-0 right-0 w-80 bg-white dark:bg-gray-800 shadow-xl border-l border-gray-200 dark:border-gray-700 z-40 transform transition-transform duration-300 ease-in-out"
    >
      <div class="h-full flex flex-col">
        <div class="flex items-center justify-between p-4 border-b border-gray-200 dark:border-gray-700">
          <h3 class="text-lg font-medium text-gray-900 dark:text-white">AI Assistant</h3>
          <button
            @click="toggleAIAssistant"
            class="p-2 rounded-md text-gray-500 hover:text-gray-700 hover:bg-gray-100 dark:text-gray-300 dark:hover:text-white dark:hover:bg-gray-700"
          >
            <svg class="h-5 w-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
        <div class="flex-1 p-4 overflow-y-auto">
          <div class="text-center text-gray-500 dark:text-gray-400">
            <p class="mb-4">AI Assistant is ready to help!</p>
            <p class="text-sm">Ask me about your business, inventory, sales, or any questions you have.</p>
          </div>
          <!-- AI chat interface will be implemented here -->
        </div>
        <div class="p-4 border-t border-gray-200 dark:border-gray-700">
          <div class="flex space-x-2">
            <input
              type="text"
              placeholder="Ask your AI assistant..."
              class="flex-1 px-3 py-2 border border-gray-300 dark:border-gray-600 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-700 dark:text-white"
            />
            <button class="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500">
              Send
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Overlay for AI Assistant -->
    <div
      v-show="showAIAssistant"
      @click="toggleAIAssistant"
      class="fixed inset-0 bg-black bg-opacity-25 z-30"
    ></div>
  </div>
</template>

<script setup lang="ts">
// Layout state management
const showMobileMenu = ref(false)
const showUserMenu = ref(false)
const showAIAssistant = ref(false)

// Color mode
const colorMode = useColorMode()

// Methods
const toggleMobileMenu = () => {
  showMobileMenu.value = !showMobileMenu.value
  showUserMenu.value = false
}

const toggleUserMenu = () => {
  showUserMenu.value = !showUserMenu.value
  showMobileMenu.value = false
}

const toggleAIAssistant = () => {
  showAIAssistant.value = !showAIAssistant.value
}

const toggleColorMode = () => {
  colorMode.preference = colorMode.value === 'dark' ? 'light' : 'dark'
}

// Close menus when clicking outside
const handleClickOutside = (event: Event) => {
  if (!event.target) return
  
  const target = event.target as Element
  if (!target.closest('.relative')) {
    showUserMenu.value = false
  }
}

// Setup click outside listener
onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onBeforeUnmount(() => {
  document.removeEventListener('click', handleClickOutside)
})

// Close mobile menu on route change
watch(() => useRoute().path, () => {
  showMobileMenu.value = false
  showUserMenu.value = false
})
</script>

<style scoped>
/* Component-specific styles */
.router-link-active {
  @apply border-blue-500 text-blue-600 dark:text-blue-400;
}

/* Smooth transitions */
.transform {
  transition-property: transform;
}

.transition-transform {
  transition-property: transform;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 300ms;
}
</style>
