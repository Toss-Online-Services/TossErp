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
      
      <!-- CRM Dropdown -->
      <div class="space-y-1">
        <button 
          @click="toggleCrmDropdown"
          class="nav-link w-full justify-between"
          :class="{ 'nav-link-active': route.path.startsWith('/crm') }"
        >
          <div class="flex items-center">
            <UsersIcon class="h-5 w-5 mr-3" />
            CRM
          </div>
          <ChevronDownIcon 
            class="h-4 w-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': crmDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="crmDropdownOpen"
          class="ml-6 space-y-1 border-l border-slate-700 pl-3"
        >
          <NuxtLink 
            to="/crm" 
            class="nav-sub-link"
            :class="{ 'nav-sub-link-active': route.path === '/crm' }"
          >
            Dashboard
          </NuxtLink>
          
          <NuxtLink 
            to="/crm/customers" 
            class="nav-sub-link"
            :class="{ 'nav-sub-link-active': route.path === '/crm/customers' }"
          >
            Customers
          </NuxtLink>
          
          <NuxtLink 
            to="/crm/leads" 
            class="nav-sub-link"
            :class="{ 'nav-sub-link-active': route.path === '/crm/leads' }"
          >
            Leads
          </NuxtLink>
          
          <NuxtLink 
            to="/crm/opportunities" 
            class="nav-sub-link"
            :class="{ 'nav-sub-link-active': route.path === '/crm/opportunities' }"
          >
            Opportunities
          </NuxtLink>
          
          <NuxtLink 
            to="/crm/contacts" 
            class="nav-sub-link"
            :class="{ 'nav-sub-link-active': route.path === '/crm/contacts' }"
          >
            Contacts
          </NuxtLink>
          
          <NuxtLink 
            to="/crm/automation" 
            class="nav-sub-link"
            :class="{ 'nav-sub-link-active': route.path === '/crm/automation' }"
          >
            Automation
          </NuxtLink>
          
          <NuxtLink 
            to="/crm/pipeline" 
            class="nav-sub-link"
            :class="{ 'nav-sub-link-active': route.path === '/crm/pipeline' }"
          >
            Pipeline
          </NuxtLink>
        </div>
      </div>
      
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
import { ref, watch } from 'vue'
import { HomeIcon, UsersIcon, BriefcaseIcon, CurrencyDollarIcon, UserGroupIcon, ArchiveBoxIcon, ShoppingCartIcon, ShoppingBagIcon, ChevronDownIcon } from '@heroicons/vue/24/outline'

// Ensure router is available
const router = useRouter()
const route = useRoute()

// CRM dropdown state
const crmDropdownOpen = ref(false)

// Auto-open CRM dropdown if we're on a CRM page
watch(() => route.path, (newPath) => {
  if (newPath.startsWith('/crm')) {
    crmDropdownOpen.value = true
  }
}, { immediate: true })

const toggleCrmDropdown = () => {
  crmDropdownOpen.value = !crmDropdownOpen.value
}
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

.nav-sub-link {
  display: block;
  padding: 0.5rem 0.75rem;
  font-size: 0.8125rem;
  font-weight: 400;
  color: rgb(148 163 184);
  border-radius: 0.375rem;
  transition: all 0.2s;
}

.nav-sub-link:hover {
  background-color: rgb(30 41 59);
  color: rgb(203 213 225);
}

.nav-sub-link-active {
  background-color: rgba(59, 130, 246, 0.2);
  color: rgb(147 197 253);
  font-weight: 500;
}

.nav-sub-link-active:hover {
  background-color: rgba(59, 130, 246, 0.3);
}
</style>
