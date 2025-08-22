<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Navigation -->
    <nav class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16">
          <div class="flex">
            <!-- Logo -->
            <div class="flex-shrink-0 flex items-center">
              <NuxtLink to="/" class="flex items-center">
                <div class="w-8 h-8 bg-gradient-to-r from-purple-600 to-blue-600 rounded-lg flex items-center justify-center mr-3">
                  <svg class="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 10V3L4 14h7v7l9-11h-7z" />
                  </svg>
                </div>
                <span class="font-bold text-xl text-gray-900 dark:text-white">TOSS ERP III</span>
              </NuxtLink>
            </div>

            <!-- Main Navigation -->
            <div class="hidden sm:ml-6 sm:flex sm:items-center sm:space-x-8">
              <NuxtLink to="/" class="text-gray-900 dark:text-white hover:text-purple-600 dark:hover:text-purple-400 px-3 py-2 text-sm font-medium transition-colors">
                Dashboard
              </NuxtLink>
              <NuxtLink to="/service-dashboard" class="text-gray-500 dark:text-gray-300 hover:text-purple-600 dark:hover:text-purple-400 px-3 py-2 text-sm font-medium transition-colors">
                Service-as-a-Software
              </NuxtLink>
              <NuxtLink to="/stock" class="text-gray-500 dark:text-gray-300 hover:text-purple-600 dark:hover:text-purple-400 px-3 py-2 text-sm font-medium transition-colors">
                Stock
              </NuxtLink>
              <NuxtLink to="/sales" class="text-gray-500 dark:text-gray-300 hover:text-purple-600 dark:hover:text-purple-400 px-3 py-2 text-sm font-medium transition-colors">
                Sales
              </NuxtLink>
              <NuxtLink to="/finance" class="text-gray-500 dark:text-gray-300 hover:text-purple-600 dark:hover:text-purple-400 px-3 py-2 text-sm font-medium transition-colors">
                Finance
              </NuxtLink>
              <NuxtLink to="/ai-copilot" class="text-gray-500 dark:text-gray-300 hover:text-purple-600 dark:hover:text-purple-400 px-3 py-2 text-sm font-medium transition-colors">
                AI Co-Pilot
              </NuxtLink>
            </div>
          </div>

          <!-- Right side -->
          <div class="flex items-center space-x-4">
            <!-- Dark mode toggle -->
            <button @click="toggleDarkMode" class="p-2 text-gray-500 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white transition-colors">
              <svg v-if="!isDarkMode" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z" />
              </svg>
              <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z" />
              </svg>
            </button>

            <!-- Mobile menu button -->
            <button @click="toggleMobileMenu" class="sm:hidden p-2 text-gray-500 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white transition-colors">
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
              </svg>
            </button>
          </div>
        </div>

        <!-- Mobile Navigation -->
        <div v-show="showMobileMenu" class="sm:hidden py-4 space-y-2">
          <NuxtLink to="/" class="block px-3 py-2 text-base font-medium text-gray-900 dark:text-white hover:text-purple-600 dark:hover:text-purple-400 transition-colors">
            Dashboard
          </NuxtLink>
          <NuxtLink to="/service-dashboard" class="block px-3 py-2 text-base font-medium text-gray-500 dark:text-gray-300 hover:text-purple-600 dark:hover:text-purple-400 transition-colors">
            Service-as-a-Software
          </NuxtLink>
          <NuxtLink to="/stock" class="block px-3 py-2 text-base font-medium text-gray-500 dark:text-gray-300 hover:text-purple-600 dark:hover:text-purple-400 transition-colors">
            Stock
          </NuxtLink>
          <NuxtLink to="/sales" class="block px-3 py-2 text-base font-medium text-gray-500 dark:text-gray-300 hover:text-purple-600 dark:hover:text-purple-400 transition-colors">
            Sales
          </NuxtLink>
          <NuxtLink to="/finance" class="block px-3 py-2 text-base font-medium text-gray-500 dark:text-gray-300 hover:text-purple-600 dark:hover:text-purple-400 transition-colors">
            Finance
          </NuxtLink>
          <NuxtLink to="/ai-copilot" class="block px-3 py-2 text-base font-medium text-gray-500 dark:text-gray-300 hover:text-purple-600 dark:hover:text-purple-400 transition-colors">
            AI Co-Pilot
          </NuxtLink>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <main class="flex-1">
      <slot />
    </main>

    <!-- Footer -->
    <footer class="bg-white dark:bg-gray-800 border-t border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto py-6 px-4 sm:px-6 lg:px-8">
        <div class="flex items-center justify-between">
          <div class="flex items-center space-x-4">
            <span class="text-sm text-gray-500 dark:text-gray-400">
              © 2025 TOSS ERP III. Service-as-a-Software for Township SMMEs.
            </span>
          </div>
          <div class="flex items-center space-x-4">
            <div class="flex items-center space-x-2">
              <div class="w-2 h-2 bg-green-400 rounded-full"></div>
              <span class="text-xs text-gray-500 dark:text-gray-400">AI Systems Online</span>
            </div>
          </div>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

// Mobile menu state
const showMobileMenu = ref(false)

// Dark mode state
const isDarkMode = ref(false)

// Toggle mobile menu
const toggleMobileMenu = () => {
  showMobileMenu.value = !showMobileMenu.value
}

// Toggle dark mode
const toggleDarkMode = () => {
  isDarkMode.value = !isDarkMode.value
  
  if (isDarkMode.value) {
    document.documentElement.classList.add('dark')
    localStorage.setItem('theme', 'dark')
  } else {
    document.documentElement.classList.remove('dark')
    localStorage.setItem('theme', 'light')
  }
}

// Initialize theme on mount
onMounted(() => {
  // Check for saved theme preference or default to light mode
  const savedTheme = localStorage.getItem('theme')
  const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches
  
  if (savedTheme === 'dark' || (!savedTheme && prefersDark)) {
    isDarkMode.value = true
    document.documentElement.classList.add('dark')
  } else {
    isDarkMode.value = false
    document.documentElement.classList.remove('dark')
  }
})
</script>

<style scoped>
/* Transition effects */
.transition-colors {
  transition-property: color, background-color, border-color;
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
  transition-duration: 200ms;
}

/* Active link styles */
.router-link-active {
  color: rgb(147 51 234); /* purple-600 */
}

.dark .router-link-active {
  color: rgb(196 181 253); /* purple-300 */
}

/* Smooth scrolling */
html {
  scroll-behavior: smooth;
}
</style>