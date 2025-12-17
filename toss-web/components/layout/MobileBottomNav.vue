<template>
  <div class="fixed bottom-0 left-0 right-0 z-30 bg-white dark:bg-slate-800 border-t-2 border-gray-200 dark:border-slate-700 shadow-lg lg:hidden safe-area-bottom">
    <!-- Core Functions Navigation -->
    <div class="flex items-center justify-around py-3 px-2">
      <!-- Dashboard -->
      <NuxtLink to="/retailer/dashboard" class="mobile-bottom-nav-item" :class="{ 'mobile-bottom-nav-active': route.path === '/retailer/dashboard' }">
        <div class="relative">
          <HomeIcon class="w-7 h-7 mb-1" />
        </div>
        <span class="text-xs font-medium">Dashboard</span>
      </NuxtLink>
      
      <!-- Products -->
      <NuxtLink to="/retailer/products" class="mobile-bottom-nav-item" :class="{ 'mobile-bottom-nav-active': route.path.startsWith('/retailer/products') }">
        <div class="relative">
          <svg class="w-7 h-7 mb-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
          </svg>
        </div>
        <span class="text-xs font-medium">Products</span>
      </NuxtLink>
      
      <!-- Orders -->
      <NuxtLink to="/retailer/orders" class="mobile-bottom-nav-item" :class="{ 'mobile-bottom-nav-active': route.path.startsWith('/retailer/orders') }">
        <div class="relative">
          <ShoppingCartIcon class="w-7 h-7 mb-1" />
        </div>
        <span class="text-xs font-medium">Orders</span>
      </NuxtLink>
      
      <!-- Inventory -->
      <NuxtLink to="/retailer/inventory" class="mobile-bottom-nav-item" :class="{ 'mobile-bottom-nav-active': route.path === '/retailer/inventory' }">
        <div class="relative">
          <svg class="w-7 h-7 mb-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
          </svg>
        </div>
        <span class="text-xs font-medium">Inventory</span>
      </NuxtLink>
    </div>
  </div>
</template>

<script setup>
import { 
  HomeIcon, 
  ShoppingCartIcon
} from '@heroicons/vue/24/outline'

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
