<template>
  <nav class="bg-white dark:bg-gray-800 shadow-sm border-b border-gray-200 dark:border-gray-700">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex justify-between h-16">
        <!-- Logo and Brand -->
        <div class="flex items-center">
          <NuxtLink to="/" class="flex items-center space-x-3">
            <div class="w-8 h-8 bg-gradient-to-r from-blue-600 to-blue-800 rounded-lg flex items-center justify-center">
              <span class="text-white font-bold text-sm">T</span>
            </div>
            <span class="text-xl font-bold text-gray-900 dark:text-white">TOSS ERP III</span>
          </NuxtLink>
        </div>

        <!-- Desktop Navigation -->
        <div class="hidden md:flex items-center space-x-8">
          <NuxtLink to="/dashboard" class="nav-link">Dashboard</NuxtLink>
          
          <!-- Modules Dropdown -->
          <div class="relative" @mouseover="showModules = true" @mouseleave="showModules = false">
            <button class="nav-link flex items-center">
              Modules
              <svg class="ml-1 w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"></path>
              </svg>
            </button>
            
            <!-- Dropdown Menu -->
            <div v-show="showModules" class="absolute top-full left-0 mt-1 w-64 bg-white dark:bg-gray-800 rounded-md shadow-lg border border-gray-200 dark:border-gray-700 z-50 max-h-[80vh] overflow-y-auto">
              <div class="py-2">
                <h3 class="px-4 py-2 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider">Core Modules</h3>
                <NuxtLink to="/accounting" class="dropdown-link">💰 Money (Finance)</NuxtLink>
                <NuxtLink to="/sales" class="dropdown-link">🛒 Sales & CRM</NuxtLink>
                <NuxtLink to="/stock" class="dropdown-link">📦 Stock</NuxtLink>
                <NuxtLink to="/pos" class="dropdown-link">💳 Point of Sale</NuxtLink>
                <NuxtLink to="/purchasing" class="dropdown-link">🛍️ Purchasing</NuxtLink>
                
                <div class="border-t border-gray-200 dark:border-gray-700 my-2"></div>
                <h3 class="px-4 py-2 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider">Operations</h3>
                <NuxtLink to="/manufacturing" class="dropdown-link">🏭 Manufacturing</NuxtLink>
                <NuxtLink to="/projects" class="dropdown-link">📁 Projects</NuxtLink>
                <NuxtLink to="/quality" class="dropdown-link">✅ Quality</NuxtLink>
                <NuxtLink to="/assets" class="dropdown-link">🏢 Assets</NuxtLink>
                
                <div class="border-t border-gray-200 dark:border-gray-700 my-2"></div>
                <h3 class="px-4 py-2 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider">People & Support</h3>
                <NuxtLink to="/hr" class="dropdown-link">👥 HR & Payroll</NuxtLink>
                <NuxtLink to="/support" class="dropdown-link">🎧 Support</NuxtLink>
                
                <div class="border-t border-gray-200 dark:border-gray-700 my-2"></div>
                <h3 class="px-4 py-2 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider">Collaboration</h3>
                <NuxtLink to="/purchasing/group-buying" class="dropdown-link">🤝 Group Buying</NuxtLink>
                <NuxtLink to="/logistics" class="dropdown-link">🚚 Shared Logistics</NuxtLink>
              </div>
            </div>
          </div>

          <NuxtLink to="/collaboration" class="nav-link">Collaboration</NuxtLink>
          <NuxtLink to="/reports" class="nav-link">Reports</NuxtLink>
        </div>

        <!-- Right Side -->
        <div class="flex items-center space-x-4">
          <!-- AI Assistant Toggle -->
          <button @click="toggleAI" class="p-2 text-gray-500 hover:text-blue-600 dark:text-gray-400 dark:hover:text-blue-400 transition-colors">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z"></path>
            </svg>
          </button>

          <!-- Notifications -->
          <button class="relative p-2 text-gray-500 hover:text-blue-600 dark:text-gray-400 dark:hover:text-blue-400 transition-colors">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9"></path>
            </svg>
            <span class="absolute top-0 right-0 block h-2 w-2 rounded-full ring-2 ring-white bg-red-400"></span>
          </button>

          <!-- Dark Mode Toggle -->
          <button @click="toggleDarkMode" class="p-2 text-gray-500 hover:text-blue-600 dark:text-gray-400 dark:hover:text-blue-400 transition-colors">
            <svg v-if="isDark" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364 6.364l-.707-.707M6.343 6.343l-.707-.707m12.728 0l-.707.707M6.343 17.657l-.707.707M16 12a4 4 0 11-8 0 4 4 0 018 0z"></path>
            </svg>
            <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z"></path>
            </svg>
          </button>

          <!-- User Menu -->
          <div class="relative">
            <button @click="showUserMenu = !showUserMenu" class="flex items-center space-x-3 p-2 rounded-lg hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors">
              <div class="w-8 h-8 bg-blue-600 rounded-full flex items-center justify-center">
                <span class="text-white text-sm font-medium">TU</span>
              </div>
              <span class="hidden lg:block text-sm font-medium text-gray-900 dark:text-white">Test User</span>
            </button>

            <!-- User Dropdown -->
            <div v-show="showUserMenu" class="absolute top-full right-0 mt-1 w-48 bg-white dark:bg-gray-800 rounded-md shadow-lg border border-gray-200 dark:border-gray-700 z-50">
              <div class="py-1">
                <NuxtLink to="/profile" class="dropdown-link">👤 Profile</NuxtLink>
                <NuxtLink to="/settings" class="dropdown-link">⚙️ Settings</NuxtLink>
                <NuxtLink to="/help" class="dropdown-link">❓ Help</NuxtLink>
                <div class="border-t border-gray-200 dark:border-gray-700 my-1"></div>
                <button @click="logout" class="dropdown-link w-full text-left text-red-600 dark:text-red-400">🚪 Logout</button>
              </div>
            </div>
          </div>

          <!-- Mobile Menu Button -->
          <button @click="showMobileMenu = !showMobileMenu" class="md:hidden p-2 text-gray-500 hover:text-blue-600 dark:text-gray-400 dark:hover:text-blue-400">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"></path>
            </svg>
          </button>
        </div>
      </div>
    </div>

    <!-- Mobile Menu -->
    <div v-show="showMobileMenu" class="md:hidden bg-white dark:bg-gray-800 border-t border-gray-200 dark:border-gray-700 max-h-[80vh] overflow-y-auto">
      <div class="px-4 pt-2 pb-3 space-y-1">
        <NuxtLink to="/dashboard" class="mobile-nav-link">Dashboard</NuxtLink>
        
        <p class="px-3 mt-4 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider">Core</p>
        <NuxtLink to="/accounting" class="mobile-nav-link">💰 Money</NuxtLink>
        <NuxtLink to="/sales" class="mobile-nav-link">🛒 Sales</NuxtLink>
        <NuxtLink to="/stock" class="mobile-nav-link">📦 Stock</NuxtLink>
        <NuxtLink to="/pos" class="mobile-nav-link">💳 POS</NuxtLink>
        <NuxtLink to="/purchasing" class="mobile-nav-link">🛍️ Purchasing</NuxtLink>
        
        <p class="px-3 mt-4 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider">Operations</p>
        <NuxtLink to="/manufacturing" class="mobile-nav-link">🏭 Manufacturing</NuxtLink>
        <NuxtLink to="/projects" class="mobile-nav-link">📁 Projects</NuxtLink>
        <NuxtLink to="/quality" class="mobile-nav-link">✅ Quality</NuxtLink>
        <NuxtLink to="/assets" class="mobile-nav-link">🏢 Assets</NuxtLink>
        
        <p class="px-3 mt-4 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider">People</p>
        <NuxtLink to="/hr" class="mobile-nav-link">👥 HR</NuxtLink>
        <NuxtLink to="/support" class="mobile-nav-link">🎧 Support</NuxtLink>
        
        <p class="px-3 mt-4 text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider">More</p>
        <NuxtLink to="/purchasing/group-buying" class="mobile-nav-link">🤝 Group Buying</NuxtLink>
        <NuxtLink to="/logistics" class="mobile-nav-link">🚚 Logistics</NuxtLink>
      </div>
    </div>
  </nav>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'

const showModules = ref(false)
const showUserMenu = ref(false)
const showMobileMenu = ref(false)

// Initialize dark mode state
const isDark = ref(false)

// Check if we're on client side and get initial theme
onMounted(() => {
  // Check for saved theme preference or default to system preference
  const savedTheme = localStorage.getItem('theme')
  const systemDark = window.matchMedia('(prefers-color-scheme: dark)').matches
  
  if (savedTheme === 'dark' || (!savedTheme && systemDark)) {
    isDark.value = true
    document.documentElement.classList.add('dark')
  } else {
    isDark.value = false
    document.documentElement.classList.remove('dark')
  }
  
  // Close dropdowns when clicking outside
  document.addEventListener('click', (e) => {
    const target = e.target as HTMLElement
    if (!target.closest('.relative')) {
      showUserMenu.value = false
      showModules.value = false
    }
  })
})

function toggleAI() {
  // Toggle AI copilot visibility
  console.log('Toggle AI Assistant')
}

function toggleDarkMode() {
  isDark.value = !isDark.value
  
  if (isDark.value) {
    document.documentElement.classList.add('dark')
    localStorage.setItem('theme', 'dark')
  } else {
    document.documentElement.classList.remove('dark')
    localStorage.setItem('theme', 'light')
  }
}

function logout() {
  // Implement logout logic
  console.log('Logout')
}
</script>

<style scoped>
.nav-link {
  @apply text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 px-3 py-2 rounded-md text-sm font-medium transition-colors;
}

.nav-link.router-link-active {
  @apply text-blue-600 dark:text-blue-400;
}

.dropdown-link {
  @apply block px-4 py-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 hover:text-blue-600 dark:hover:text-blue-400 transition-colors;
}

.mobile-nav-link {
  @apply block px-3 py-2 text-base font-medium text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 hover:bg-gray-50 dark:hover:bg-gray-700 rounded-md transition-colors;
}

.mobile-nav-link.router-link-active {
  @apply text-blue-600 dark:text-blue-400 bg-blue-50 dark:bg-blue-900/20;
}
</style>
