<template>
  <aside class="w-64 flex-shrink-0 bg-slate-900 border-r border-slate-800 flex flex-col">
    <!-- Logo Section -->
    <div class="h-16 flex items-center justify-center px-4 border-b border-slate-800">
      <div class="w-8 h-8 bg-gradient-to-r from-blue-500 to-purple-600 rounded-lg flex items-center justify-center mr-3">
        <span class="text-white font-bold text-sm">T</span>
      </div>
      <h1 class="text-xl font-bold text-white">TOSS ERP</h1>
    </div>
    
    <!-- Navigation -->
    <nav class="flex-1 px-3 py-6 space-y-2">
      <NuxtLink 
        to="/" 
        class="nav-link"
        :class="{ 'nav-link-active': route.path === '/' }"
      >
        <HomeIcon class="h-5 w-5 mr-3" />
        Dashboard
      </NuxtLink>
      
      <NuxtLink 
        to="/crm" 
        class="nav-link"
        :class="{ 'nav-link-active': route.path.startsWith('/crm') }"
      >
        <UsersIcon class="h-5 w-5 mr-3" />
        CRM
      </NuxtLink>
      
      <NuxtLink 
        to="/projects" 
        class="nav-link"
        :class="{ 'nav-link-active': route.path.startsWith('/projects') }"
      >
        <BriefcaseIcon class="h-5 w-5 mr-3" />
        Projects
      </NuxtLink>
      
      <NuxtLink 
        to="/accounts" 
        class="nav-link"
        :class="{ 'nav-link-active': route.path.startsWith('/accounts') }"
      >
        <CurrencyDollarIcon class="h-5 w-5 mr-3" />
        Accounts
      </NuxtLink>
      
      <NuxtLink 
        to="/hr" 
        class="nav-link"
        :class="{ 'nav-link-active': route.path.startsWith('/hr') }"
      >
        <UserGroupIcon class="h-5 w-5 mr-3" />
        HR
      </NuxtLink>
      
      <NuxtLink 
        to="/stock" 
        class="nav-link"
        :class="{ 'nav-link-active': route.path.startsWith('/stock') }"
      >
        <ArchiveBoxIcon class="h-5 w-5 mr-3" />
        Stock
      </NuxtLink>
      
      <NuxtLink 
        to="/selling" 
        class="nav-link"
        :class="{ 'nav-link-active': route.path.startsWith('/selling') }"
      >
        <ShoppingCartIcon class="h-5 w-5 mr-3" />
        Selling
      </NuxtLink>
      
      <NuxtLink 
        to="/buying" 
        class="nav-link"
        :class="{ 'nav-link-active': route.path.startsWith('/buying') }"
      >
        <ShoppingBagIcon class="h-5 w-5 mr-3" />
        Buying
      </NuxtLink>
    </nav>
    
    <!-- Footer -->
    <div class="p-4 border-t border-slate-800">
      <div class="text-xs text-slate-400 text-center">
        TOSS ERP v1.0.0
      </div>
    </div>
  </aside>
</template>

<script setup>
import { HomeIcon, UsersIcon, BriefcaseIcon, CurrencyDollarIcon, UserGroupIcon, ArchiveBoxIcon, ShoppingCartIcon, ShoppingBagIcon } from '@heroicons/vue/24/outline'

// Ensure router is available
const router = useRouter()
const route = useRoute()
</script>

<style scoped>
.nav-link {
  display: flex;
  align-items: center;
  padding: 0.625rem 0.75rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: rgb(203 213 225);
  border-radius: 0.5rem;
  transition: all 0.2s;
}

.nav-link:hover {
  background-color: rgb(30 41 59);
  color: white;
}

.nav-link-active {
  background: linear-gradient(to right, rgb(59 130 246), rgb(147 51 234));
  color: white;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
}

.nav-link-active:hover {
  background: linear-gradient(to right, rgb(37 99 235), rgb(126 34 206));
}
</style>
