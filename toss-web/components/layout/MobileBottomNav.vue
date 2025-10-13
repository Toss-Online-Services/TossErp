<template>
  <div class="fixed bottom-0 left-0 right-0 z-30 bg-white dark:bg-slate-800 border-t border-slate-200 dark:border-slate-700 lg:hidden">
    <div class="flex items-center justify-around py-2">
      <NuxtLink to="/" class="mobile-bottom-nav-item" :class="{ 'mobile-bottom-nav-active': route.path === '/' }">
        <HomeIcon class="w-5 h-5 mb-1" />
        <span class="text-xs">Home</span>
      </NuxtLink>
      
      <NuxtLink to="/crm" class="mobile-bottom-nav-item" :class="{ 'mobile-bottom-nav-active': route.path.startsWith('/crm') }">
        <UsersIcon class="w-5 h-5 mb-1" />
        <span class="text-xs">CRM</span>
      </NuxtLink>
      
      <NuxtLink to="/projects" class="mobile-bottom-nav-item" :class="{ 'mobile-bottom-nav-active': route.path.startsWith('/projects') }">
        <BriefcaseIcon class="w-5 h-5 mb-1" />
        <span class="text-xs">Projects</span>
      </NuxtLink>
      
      <NuxtLink to="/accounting" class="mobile-bottom-nav-item" :class="{ 'mobile-bottom-nav-active': route.path.startsWith('/accounting') }">
        <CurrencyDollarIcon class="w-5 h-5 mb-1" />
        <span class="text-xs">Accounts</span>
      </NuxtLink>
      
      <NuxtLink to="/stock" class="mobile-bottom-nav-item" :class="{ 'mobile-bottom-nav-active': route.path.startsWith('/stock') }">
        <ArchiveBoxIcon class="w-5 h-5 mb-1" />
        <span class="text-xs">Stock</span>
      </NuxtLink>
    </div>
  </div>
</template>

<script setup>
import { 
  HomeIcon, 
  UsersIcon, 
  BriefcaseIcon, 
  CurrencyDollarIcon, 
  ArchiveBoxIcon
} from '@heroicons/vue/24/outline'

const route = useRoute()
</script>

<style scoped>
.mobile-bottom-nav-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 0.5rem;
  min-height: 3.5rem;
  color: rgb(100 116 139);
  transition: all 0.2s;
  touch-action: manipulation;
  flex: 1;
  text-align: center;
}

.mobile-bottom-nav-item:hover {
  color: rgb(59 130 246);
}

.mobile-bottom-nav-active {
  color: rgb(59 130 246);
  font-weight: 600;
}

.mobile-bottom-nav-active svg {
  transform: scale(1.1);
}
</style>
