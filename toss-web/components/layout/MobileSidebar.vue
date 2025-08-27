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
        <span class="truncate">Dashboard</span>
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
        to="/projects" 
        class="mobile-nav-link"
        :class="{ 'mobile-nav-link-active': route.path.startsWith('/projects') }"
        @click="$emit('close')"
      >
        <BriefcaseIcon class="h-5 w-5 mr-3 flex-shrink-0" />
        <span class="truncate">Projects</span>
      </NuxtLink>
      
      <NuxtLink 
        to="/accounts" 
        class="mobile-nav-link"
        :class="{ 'mobile-nav-link-active': route.path.startsWith('/accounts') }"
        @click="$emit('close')"
      >
        <CurrencyDollarIcon class="h-5 w-5 mr-3 flex-shrink-0" />
        <span class="truncate">Accounts</span>
      </NuxtLink>
      
      <NuxtLink 
        to="/hr" 
        class="mobile-nav-link"
        :class="{ 'mobile-nav-link-active': route.path.startsWith('/hr') }"
        @click="$emit('close')"
      >
        <UserGroupIcon class="h-5 w-5 mr-3 flex-shrink-0" />
        <span class="truncate">HR</span>
      </NuxtLink>
      
      <NuxtLink 
        to="/stock" 
        class="mobile-nav-link"
        :class="{ 'mobile-nav-link-active': route.path.startsWith('/stock') }"
        @click="$emit('close')"
      >
        <ArchiveBoxIcon class="h-5 w-5 mr-3 flex-shrink-0" />
        <span class="truncate">Stock</span>
      </NuxtLink>
      
      <NuxtLink 
        to="/selling" 
        class="mobile-nav-link"
        :class="{ 'mobile-nav-link-active': route.path.startsWith('/selling') }"
        @click="$emit('close')"
      >
        <ShoppingCartIcon class="h-5 w-5 mr-3 flex-shrink-0" />
        <span class="truncate">Selling</span>
      </NuxtLink>
      
      <NuxtLink 
        to="/buying" 
        class="mobile-nav-link"
        :class="{ 'mobile-nav-link-active': route.path.startsWith('/buying') }"
        @click="$emit('close')"
      >
        <ShoppingBagIcon class="h-5 w-5 mr-3 flex-shrink-0" />
        <span class="truncate">Buying</span>
      </NuxtLink>
      
      <!-- Additional Mobile Links -->
      <div class="border-t border-slate-800 mt-4 pt-4">
        <h3 class="text-xs uppercase text-slate-400 font-semibold mb-2 px-3">More</h3>
        
        <NuxtLink 
          to="/reports" 
          class="mobile-nav-link"
          :class="{ 'mobile-nav-link-active': route.path.startsWith('/reports') }"
          @click="$emit('close')"
        >
          <ChartBarIcon class="h-5 w-5 mr-3 flex-shrink-0" />
          <span class="truncate">Reports</span>
        </NuxtLink>
        
        <NuxtLink 
          to="/settings" 
          class="mobile-nav-link"
          :class="{ 'mobile-nav-link-active': route.path.startsWith('/settings') }"
          @click="$emit('close')"
        >
          <CogIcon class="h-5 w-5 mr-3 flex-shrink-0" />
          <span class="truncate">Settings</span>
        </NuxtLink>
        
        <NuxtLink 
          to="/help" 
          class="mobile-nav-link"
          :class="{ 'mobile-nav-link-active': route.path.startsWith('/help') }"
          @click="$emit('close')"
        >
          <QuestionMarkCircleIcon class="h-5 w-5 mr-3 flex-shrink-0" />
          <span class="truncate">Help & Support</span>
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
  UsersIcon, 
  BriefcaseIcon, 
  CurrencyDollarIcon, 
  UserGroupIcon, 
  ArchiveBoxIcon, 
  ShoppingCartIcon, 
  ShoppingBagIcon,
  ChartBarIcon,
  CogIcon,
  QuestionMarkCircleIcon,
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
}

.mobile-nav-link:hover {
  background-color: rgb(30 41 59);
  color: white;
}

.mobile-nav-link-active {
  background: linear-gradient(to right, rgb(37 99 235), rgb(147 51 234));
  color: white;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
}

.mobile-nav-link-active:hover {
  background: linear-gradient(to right, rgb(29 78 216), rgb(126 34 206));
}
</style>
