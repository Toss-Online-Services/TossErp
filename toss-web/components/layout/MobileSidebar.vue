<template>
  <div class="h-full flex flex-col bg-slate-900">
    <!-- Mobile Logo Section -->
    <div class="h-16 flex items-center justify-between px-4 border-b border-slate-800">
      <div class="flex items-center">
        <div class="w-8 h-8 bg-gradient-to-r from-blue-500 to-purple-600 rounded-lg flex items-center justify-center mr-3">
          <span class="text-white font-bold text-sm">T</span>
        </div>
        <h1 class="text-xl font-bold text-white">TOSS ERP</h1>
      </div>
      <button @click="$emit('close')" class="p-2 text-slate-400 hover:text-white rounded-lg">
        <XMarkIcon class="w-5 h-5" />
      </button>
    </div>
    
    <!-- Mobile Navigation -->
    <nav class="flex-1 px-3 py-6 space-y-2 overflow-y-auto">
      <NuxtLink 
        to="/" 
        class="mobile-nav-link"
        :class="{ 'mobile-nav-link-active': route.path === '/' }"
        @click="$emit('close')"
      >
        <HomeIcon class="h-5 w-5 mr-3 flex-shrink-0" />
        <span class="truncate">Home</span>
      </NuxtLink>
      
      <NuxtLink 
        to="/retailer/dashboard" 
        class="mobile-nav-link"
        :class="{ 'mobile-nav-link-active': route.path === '/retailer/dashboard' || route.path === '/dashboard' }"
        @click="$emit('close')"
      >
        <ChartBarIcon class="h-5 w-5 mr-3 flex-shrink-0" />
        <span class="truncate">Dashboard</span>
      </NuxtLink>
      
      <NuxtLink 
        to="/retailer/inventory" 
        class="mobile-nav-link"
        :class="{ 'mobile-nav-link-active': route.path.startsWith('/stock') || route.path.startsWith('/retailer/inventory') || route.path.startsWith('/retailer/products') }"
        @click="$emit('close')"
      >
        <ArchiveBoxIcon class="h-5 w-5 mr-3 flex-shrink-0" />
        <span class="truncate">Stock & Inventory</span>
      </NuxtLink>
      
      <NuxtLink 
        to="/logistics" 
        class="mobile-nav-link"
        :class="{ 'mobile-nav-link-active': route.path.startsWith('/logistics') }"
        @click="$emit('close')"
      >
        <TruckIcon class="h-5 w-5 mr-3 flex-shrink-0" />
        <span class="truncate">Logistics</span>
      </NuxtLink>
      
      <NuxtLink 
        to="/sales" 
        class="mobile-nav-link"
        :class="{ 'mobile-nav-link-active': route.path.startsWith('/sales') }"
        @click="$emit('close')"
      >
        <ShoppingCartIcon class="h-5 w-5 mr-3 flex-shrink-0" />
        <span class="truncate">Sales</span>
      </NuxtLink>
      
      <NuxtLink 
        to="/stores" 
        class="mobile-nav-link"
        :class="{ 'mobile-nav-link-active': route.path.startsWith('/stores') }"
        @click="$emit('close')"
      >
        <BuildingStorefrontIcon class="h-5 w-5 mr-3 flex-shrink-0" />
        <span class="truncate">Stores</span>
      </NuxtLink>
      
      <NuxtLink 
        to="/retailer/orders" 
        class="mobile-nav-link"
        :class="{ 'mobile-nav-link-active': route.path.startsWith('/buying') || route.path.startsWith('/retailer/orders') }"
        @click="$emit('close')"
      >
        <ShoppingBagIcon class="h-5 w-5 mr-3 flex-shrink-0" />
        <span class="truncate">Buying</span>
      </NuxtLink>
      
      <NuxtLink 
        to="/crm" 
        class="mobile-nav-link"
        :class="{ 'mobile-nav-link-active': route.path.startsWith('/crm') }"
        @click="$emit('close')"
      >
        <UsersIcon class="h-5 w-5 mr-3 flex-shrink-0" />
        <span class="truncate">CRM</span>
      </NuxtLink>
      
      <NuxtLink 
        to="/automation" 
        class="mobile-nav-link"
        :class="{ 'mobile-nav-link-active': route.path.startsWith('/automation') }"
        @click="$emit('close')"
      >
        <CogIcon class="h-5 w-5 mr-3 flex-shrink-0" />
        <span class="truncate">Automation</span>
        <span class="ml-2 px-2 py-0.5 text-xs bg-blue-500 text-white rounded-full">AI</span>
      </NuxtLink>
      
      <NuxtLink 
        to="/retailer/onboarding" 
        class="mobile-nav-link"
        :class="{ 'mobile-nav-link-active': route.path.startsWith('/onboarding') || route.path.startsWith('/retailer/onboarding') }"
        @click="$emit('close')"
      >
        <UserPlusIcon class="h-5 w-5 mr-3 flex-shrink-0" />
        <span class="truncate">Onboarding</span>
        <span class="ml-2 px-2 py-0.5 text-xs bg-green-500 text-white rounded-full">New</span>
      </NuxtLink>
      
      <!-- Additional Mobile Links -->
      <div class="border-t border-slate-800 mt-4 pt-4">
        <h3 class="text-xs uppercase text-slate-400 font-semibold mb-2 px-3">More</h3>
        <NuxtLink 
          to="/sales/reports" 
          class="mobile-nav-link"
          :class="{ 'mobile-nav-link-active': route.path.startsWith('/sales/reports') }"
          @click="$emit('close')"
        >
          <ChartBarIcon class="h-5 w-5 mr-3 flex-shrink-0" />
          <span class="truncate">Sales Reports</span>
        </NuxtLink>
        
        <NuxtLink 
          to="/settings" 
          class="mobile-nav-link"
          :class="{ 'mobile-nav-link-active': route.path.startsWith('/settings') }"
          @click="$emit('close')"
        >
          <Cog6ToothIcon class="h-5 w-5 mr-3 flex-shrink-0" />
          <span class="truncate">Settings</span>
        </NuxtLink>
      </div>
    </nav>
    
    <!-- Mobile Footer -->
    <div class="p-4 border-t border-slate-800">
      <div class="text-xs text-slate-400 text-center">
        TOSS ERP v1.0.0
      </div>
      <div class="mt-2 flex justify-center">
        <ThemeSwitcher />
      </div>
    </div>
  </div>
</template>

<script setup>
import { 
  HomeIcon,
  ArchiveBoxIcon, 
  ShoppingCartIcon, 
  ShoppingBagIcon,
  BuildingStorefrontIcon,
  ChartBarIcon,
  CogIcon,
  Cog6ToothIcon,
  TruckIcon,
  UserPlusIcon,
  UsersIcon,
  XMarkIcon
} from '@heroicons/vue/24/outline'
import ThemeSwitcher from '~/components/common/ThemeSwitcher.vue'

// Define emits
defineEmits(['close'])

// Get route for active state
const route = useRoute()
</script>

<style scoped>
.mobile-nav-link {
  display: flex;
  align-items: center;
  padding: 0.75rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: rgb(203 213 225);
  border-radius: 0.5rem;
  transition: all 0.2s;
  touch-action: manipulation;
  text-decoration: none;
}

.mobile-nav-link:hover {
  background-color: rgb(30 41 59);
  color: white;
  text-decoration: none;
}

.mobile-nav-link-active {
  background: linear-gradient(to right, rgb(37 99 235), rgb(147 51 234));
  color: white;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
  text-decoration: none;
}

.mobile-nav-link-active:hover {
  background: linear-gradient(to right, rgb(29 78 216), rgb(126 34 206));
  text-decoration: none;
}
</style>
