<template>
  <div class="fixed bottom-0 left-0 right-0 z-30 bg-white dark:bg-slate-800 border-t-2 border-gray-200 dark:border-slate-700 shadow-lg lg:hidden safe-area-bottom">
    <!-- Core Functions Navigation -->
    <div class="flex items-center justify-around py-3 px-2">
      <!-- Home -->
      <NuxtLink to="/" class="mobile-bottom-nav-item" :class="{ 'mobile-bottom-nav-active': route.path === '/' }">
        <div class="relative">
          <HomeIcon class="w-7 h-7 mb-1" />
        </div>
        <span class="text-xs font-medium">Home</span>
      </NuxtLink>
      
      <!-- Buy (Purchasing) -->
      <NuxtLink to="/purchasing/group-buying" class="mobile-bottom-nav-item" :class="{ 'mobile-bottom-nav-active': route.path.startsWith('/purchasing') }">
        <div class="relative">
          <ShoppingCartIcon class="w-7 h-7 mb-1" />
        </div>
        <span class="text-xs font-medium">Buy</span>
      </NuxtLink>
      
      <!-- Sell (Sales) -->
      <NuxtLink to="/sales" class="mobile-bottom-nav-item" :class="{ 'mobile-bottom-nav-active': route.path.startsWith('/sales') }">
        <div class="relative">
          <svg class="w-7 h-7 mb-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
        </div>
        <span class="text-xs font-medium">Sell</span>
      </NuxtLink>
      
      <!-- Stock -->
      <NuxtLink to="/stock" class="mobile-bottom-nav-item" :class="{ 'mobile-bottom-nav-active': route.path.startsWith('/stock') }">
        <div class="relative">
          <svg class="w-7 h-7 mb-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
          </svg>
        </div>
        <span class="text-xs font-medium">Stock</span>
      </NuxtLink>
    </div>
  </div>
</template>

<script setup>
import { useI18n } from 'vue-i18n'
import { 
  HomeIcon, 
  ShoppingCartIcon
} from '@heroicons/vue/24/outline'

const { t } = useI18n()
const route = useRoute()
</script>

<style scoped>
.mobile-bottom-nav-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 0.625rem 0.5rem;
  min-height: 4rem; /* Increased for better tap targets - WCAG 2.5.5 */
  min-width: 4rem; /* WCAG compliant tap target */
  color: #4a4a4a; /* Improved contrast */
  transition: all 0.2s ease-in-out;
  touch-action: manipulation;
  flex: 1;
  text-align: center;
  border-radius: 0.5rem;
  position: relative;
  text-decoration: none;
}

.mobile-bottom-nav-item:hover {
  color: #2563eb;
  background-color: #eff6ff;
  text-decoration: none;
}

.mobile-bottom-nav-item:active {
  transform: scale(0.95);
  background-color: #dbeafe;
  text-decoration: none;
}

.mobile-bottom-nav-active {
  color: #2563eb; /* Blue 600 - Better contrast */
  font-weight: 700; /* Bolder for emphasis */
  background-color: #eff6ff;
  text-decoration: none;
}

.mobile-bottom-nav-active svg {
  transform: scale(1.15);
}

/* Add visual indicator (dot) for active state */
.mobile-bottom-nav-active::after {
  content: '';
  position: absolute;
  bottom: 0.25rem;
  left: 50%;
  transform: translateX(-50%);
  width: 0.375rem;
  height: 0.375rem;
  background-color: #2563eb;
  border-radius: 50%;
}

/* Ensure icons have good contrast */
.mobile-bottom-nav-item svg {
  stroke-width: 2.5; /* Thicker lines for better visibility */
}
</style>
