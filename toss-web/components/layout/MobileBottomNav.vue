<template>
  <div class="fixed bottom-0 left-0 right-0 z-30 bg-white border-t-2 border-gray-200 shadow-lg lg:hidden safe-area-bottom">
    <!-- Simplified Navigation - Only Essential Actions -->
    <div class="flex items-center justify-around py-3 px-2">
      <!-- Home -->
      <NuxtLink to="/" class="mobile-bottom-nav-item" :class="{ 'mobile-bottom-nav-active': route.path === '/' }">
        <div class="relative">
          <HomeIcon class="w-7 h-7 mb-1" />
        </div>
        <span class="text-sm font-medium">{{ $t('navigation.dashboard') || 'Home' }}</span>
      </NuxtLink>
      
      <!-- Order Stock -->
      <NuxtLink to="/stock/order" class="mobile-bottom-nav-item" :class="{ 'mobile-bottom-nav-active': route.path.startsWith('/stock/order') }">
        <div class="relative">
          <ShoppingCartIcon class="w-7 h-7 mb-1" />
          <span class="absolute -top-1 -right-1 flex h-5 w-5 items-center justify-center rounded-full bg-blue-600 text-xs font-bold text-white">
            !
          </span>
        </div>
        <span class="text-sm font-medium">{{ $t('navigation.orderStock') || 'Order' }}</span>
      </NuxtLink>
      
      <!-- My Orders/Track -->
      <NuxtLink to="/stock/track" class="mobile-bottom-nav-item" :class="{ 'mobile-bottom-nav-active': route.path.startsWith('/stock/track') }">
        <div class="relative">
          <svg class="w-7 h-7 mb-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
          </svg>
        </div>
        <span class="text-sm font-medium">{{ $t('navigation.myOrders') || 'Orders' }}</span>
      </NuxtLink>
      
      <!-- Group Buying -->
      <NuxtLink to="/group-buying" class="mobile-bottom-nav-item" :class="{ 'mobile-bottom-nav-active': route.path.startsWith('/group-buying') }">
        <div class="relative">
          <svg class="w-7 h-7 mb-1" fill="currentColor" viewBox="0 0 20 20">
            <path d="M13 6a3 3 0 11-6 0 3 3 0 016 0zM18 8a2 2 0 11-4 0 2 2 0 014 0zM14 15a4 4 0 00-8 0v3h8v-3zM6 8a2 2 0 11-4 0 2 2 0 014 0zM16 18v-3a5.972 5.972 0 00-.75-2.906A3.005 3.005 0 0119 15v3h-3zM4.75 12.094A5.973 5.973 0 004 15v3H1v-3a3 3 0 013.75-2.906z" />
          </svg>
        </div>
        <span class="text-sm font-medium">{{ $t('navigation.groupBuying') || 'Group' }}</span>
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
