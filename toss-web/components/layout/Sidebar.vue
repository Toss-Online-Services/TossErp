<template>
  <aside class="flex flex-col flex-shrink-0 w-64 border-r bg-white border-gray-200 shadow-sm">
    <!-- Logo Section -->
    <div class="flex items-center justify-center h-16 px-4 border-b border-gray-200">
      <div class="flex items-center justify-center w-8 h-8 mr-3 rounded-lg bg-gradient-to-r from-blue-500 to-purple-600">
        <span class="text-sm font-bold text-white">T</span>
      </div>
      <h1 class="text-xl font-bold text-gray-900">TOSS ERP</h1>
    </div>
    
    <!-- Navigation -->
    <nav class="flex-1 px-3 py-6 space-y-2 overflow-y-auto">
      <!-- Home -->
      <NuxtLink 
        to="/" 
        class="nav-link"
        :class="{ 'nav-link-active': route.path === '/' }"
      >
        <HomeIcon class="w-5 h-5 mr-3" />
        Home
      </NuxtLink>

      <!-- Dashboard -->
      <NuxtLink 
        to="/dashboard" 
        class="nav-link"
        :class="{ 'nav-link-active': route.path === '/dashboard' }"
      >
        <ChartBarIcon class="w-5 h-5 mr-3" />
        Dashboard
      </NuxtLink>
      
      <!-- Stock & Inventory Section -->
      <div class="space-y-1">
        <button 
          @click="toggleStockDropdown"
          class="justify-between w-full nav-link"
          :class="{ 'nav-link-active': route.path.startsWith('/stock') }"
        >
          <div class="flex items-center">
            <ArchiveBoxIcon class="w-5 h-5 mr-3" />
            Stock & Inventory
          </div>
          <ChevronDownIcon 
            class="w-4 h-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': stockDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="stockDropdownOpen"
          class="pl-3 ml-6 space-y-1 border-l border-slate-700"
        >
          <NuxtLink to="/stock" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/stock' }">
            Stock Dashboard
          </NuxtLink>
          <NuxtLink to="/stock/movements" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/stock/movements' }">
            Stock Movements
          </NuxtLink>
          <NuxtLink to="/stock/items" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/stock/items' }">
            Items
          </NuxtLink>
        </div>
      </div>

      <!-- Logistics Section -->
      <div class="space-y-1">
        <button 
          @click="toggleLogisticsDropdown"
          class="justify-between w-full nav-link"
          :class="{ 'nav-link-active': route.path.startsWith('/logistics') }"
        >
          <div class="flex items-center">
            <TruckIcon class="w-5 h-5 mr-3" />
            Logistics
          </div>
          <ChevronDownIcon 
            class="w-4 h-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': logisticsDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="logisticsDropdownOpen"
          class="pl-3 ml-6 space-y-1 border-l border-slate-700"
        >
          <NuxtLink to="/logistics" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/logistics' }">
            Logistics Dashboard
          </NuxtLink>
          <NuxtLink to="/logistics/shared-runs" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/logistics/shared-runs' }">
            Shared Delivery Runs
          </NuxtLink>
          <NuxtLink to="/logistics/tracking" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/logistics/tracking' }">
            Live Tracking
          </NuxtLink>
          <NuxtLink to="/logistics/driver" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/logistics/driver' }">
            Driver Interface
          </NuxtLink>
        </div>
      </div>

      <!-- Sales Section -->
      <div class="space-y-1">
        <button 
          @click="toggleSalesDropdown"
          class="justify-between w-full nav-link"
          :class="{ 'nav-link-active': route.path.startsWith('/sales') }"
        >
          <div class="flex items-center">
            <ShoppingCartIcon class="w-5 h-5 mr-3" />
            Sales
          </div>
          <ChevronDownIcon 
            class="w-4 h-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': salesDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="salesDropdownOpen"
          class="pl-3 ml-6 space-y-1 border-l border-slate-700"
        >
          <NuxtLink to="/sales" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/sales' }">
            Sales Dashboard
          </NuxtLink>
          
          <!-- Orders Submenu -->
          <div class="space-y-1">
            <button 
              @click="toggleOrdersDropdown"
              class="justify-between w-full nav-sub-link"
              :class="{ 'nav-sub-link-active': route.path.startsWith('/sales/orders') }"
            >
              <div class="flex items-center w-full">
                <span class="flex-1 text-left">Orders</span>
                <ChevronDownIcon 
                  class="w-3 h-3 transition-transform duration-200"
                  :class="{ 'transform rotate-180': ordersDropdownOpen }"
                />
              </div>
            </button>
            
            <div 
              v-show="ordersDropdownOpen"
              class="pl-3 ml-3 space-y-1 border-l border-slate-600"
            >
              <NuxtLink to="/sales/orders" class="nav-sub-sub-link" :class="{ 'nav-sub-sub-link-active': route.path === '/sales/orders' }">
                Orders
              </NuxtLink>
              <NuxtLink to="/sales/orders/create-order" class="nav-sub-sub-link" :class="{ 'nav-sub-sub-link-active': route.path === '/sales/orders/create-order' }">
                Create Order
              </NuxtLink>
              <NuxtLink to="/sales/orders/queue" class="nav-sub-sub-link" :class="{ 'nav-sub-sub-link-active': route.path === '/sales/orders/queue' }">
                Queue
              </NuxtLink>
            </div>
          </div>
          
          <NuxtLink to="/sales/invoices" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/sales/invoices' }">
            Invoices
          </NuxtLink>
          <NuxtLink to="/sales/pos" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path.startsWith('/sales/pos') }">
            Point of Sale
          </NuxtLink>
        </div>
      </div>

      <!-- Purchasing Section -->
      <div class="space-y-1">
        <button 
          @click="toggleBuyingDropdown"
          class="justify-between w-full nav-link"
          :class="{ 'nav-link-active': route.path.startsWith('/buying') }"
        >
          <div class="flex items-center">
            <ShoppingBagIcon class="w-5 h-5 mr-3" />
            Buying
          </div>
          <ChevronDownIcon 
            class="w-4 h-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': buyingDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="buyingDropdownOpen"
          class="pl-3 ml-6 space-y-1 border-l border-slate-700"
        >
          <NuxtLink to="/buying" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/buying' }">
            Buy Dashboard
          </NuxtLink>
          
          <!-- Orders Submenu -->
          <div class="space-y-1">
            <button 
              @click="toggleBuyingOrdersDropdown"
              class="justify-between w-full nav-sub-link"
              :class="{ 'nav-sub-link-active': route.path.startsWith('/buying/orders') }"
            >
              <div class="flex items-center w-full">
                <span class="flex-1 text-left">Orders</span>
                <ChevronDownIcon 
                  class="w-3 h-3 transition-transform duration-200"
                  :class="{ 'transform rotate-180': buyingOrdersDropdownOpen }"
                />
              </div>
            </button>
            
            <div 
              v-show="buyingOrdersDropdownOpen"
              class="pl-3 ml-3 space-y-1 border-l border-slate-600"
            >
              <NuxtLink to="/buying/orders" class="nav-sub-sub-link" :class="{ 'nav-sub-sub-link-active': route.path === '/buying/orders' }">
                Orders
              </NuxtLink>
              <NuxtLink to="/buying/orders/create-order" class="nav-sub-sub-link" :class="{ 'nav-sub-sub-link-active': route.path === '/buying/orders/create-order' }">
                Create Order
              </NuxtLink>
            </div>
          </div>
          
          <NuxtLink to="/buying/invoices" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/buying/invoices' }">
            Invoices
          </NuxtLink>
          <NuxtLink to="/buying/suppliers" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/buying/suppliers' }">
            Suppliers
          </NuxtLink>
          <NuxtLink to="/buying/group-buying" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/buying/group-buying' }">
            Group Buying
          </NuxtLink>
        </div>
      </div>

      <!-- Automation Section -->
      <div class="space-y-1">
        <button 
          @click="toggleAutomationDropdown"
          class="justify-between w-full nav-link"
          :class="{ 'nav-link-active': route.path.startsWith('/automation') }"
        >
          <div class="flex items-center">
            <CogIcon class="w-5 h-5 mr-3" />
            Automation
            <span class="ml-2 px-2 py-0.5 text-xs bg-blue-500 text-white rounded-full">AI</span>
          </div>
          <ChevronDownIcon 
            class="w-4 h-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': automationDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="automationDropdownOpen"
          class="pl-3 ml-6 space-y-1 border-l border-slate-700"
        >
          <NuxtLink to="/automation" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/automation' }">
            Automation Hub
          </NuxtLink>
          <NuxtLink to="/automation/workflows" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/automation/workflows' }">
            Workflows
          </NuxtLink>
          <NuxtLink to="/automation/triggers" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/automation/triggers' }">
            Triggers & Rules
          </NuxtLink>
          <NuxtLink to="/automation/ai-assistant" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/automation/ai-assistant' }">
            AI Assistant
          </NuxtLink>
          <NuxtLink to="/automation/reports" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/automation/reports' }">
            Automation Reports
          </NuxtLink>
        </div>
      </div>

      <!-- Onboarding Section -->
      <div class="space-y-1">
        <button 
          @click="toggleOnboardingDropdown"
          class="justify-between w-full nav-link"
          :class="{ 'nav-link-active': route.path.startsWith('/onboarding') }"
        >
          <div class="flex items-center">
            <UserPlusIcon class="w-5 h-5 mr-3" />
            Onboarding
            <span class="ml-2 px-2 py-0.5 text-xs bg-green-500 text-white rounded-full">New</span>
          </div>
          <ChevronDownIcon 
            class="w-4 h-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': onboardingDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="onboardingDropdownOpen"
          class="pl-3 ml-6 space-y-1 border-l border-slate-700"
        >
          <NuxtLink to="/onboarding" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/onboarding' }">
            Onboarding Dashboard
          </NuxtLink>
        </div>
      </div>

      <!-- Settings Section -->
      <div class="space-y-1">
        <button 
          @click="toggleSettingsDropdown"
          class="justify-between w-full nav-link"
          :class="{ 'nav-link-active': route.path.startsWith('/settings') }"
        >
          <div class="flex items-center">
            <Cog6ToothIcon class="w-5 h-5 mr-3" />
            Settings
          </div>
          <ChevronDownIcon 
            class="w-4 h-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': settingsDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="settingsDropdownOpen"
          class="pl-3 ml-6 space-y-1 border-l border-slate-700"
        >
          <NuxtLink to="/settings" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/settings' }">
            Settings
          </NuxtLink>
        </div>
      </div>
    </nav>
    
    <!-- Footer -->
    <div class="p-4 border-t border-slate-800">
      <div class="text-xs text-center text-slate-400">
        TOSS ERP v1.0.0
      </div>
    </div>
  </aside>
</template>

<script setup>
import { ref, watch } from 'vue'
import { 
  HomeIcon,
  ChartBarIcon, 
  ArchiveBoxIcon, 
  ShoppingCartIcon, 
  ShoppingBagIcon, 
  ChevronDownIcon,
  CogIcon,
  Cog6ToothIcon,
  TruckIcon,
  UserPlusIcon
} from '@heroicons/vue/24/outline'

// Ensure router is available
const router = useRouter()
const route = useRoute()

// Dropdown states
const stockDropdownOpen = ref(false)
const logisticsDropdownOpen = ref(false)
const salesDropdownOpen = ref(false)
const ordersDropdownOpen = ref(false)
const buyingDropdownOpen = ref(false)
const buyingOrdersDropdownOpen = ref(false)
const automationDropdownOpen = ref(false)
const onboardingDropdownOpen = ref(false)
const settingsDropdownOpen = ref(false)

// Auto-open dropdowns if we're on a page within that section
watch(() => route.path, (newPath) => {
  if (newPath.startsWith('/stock')) {
    stockDropdownOpen.value = true
  }
  if (newPath.startsWith('/logistics')) {
    logisticsDropdownOpen.value = true
  }
  if (newPath.startsWith('/sales')) {
    salesDropdownOpen.value = true
    if (newPath.startsWith('/sales/orders')) {
      ordersDropdownOpen.value = true
    }
  }
  if (newPath.startsWith('/buying')) {
    buyingDropdownOpen.value = true
    if (newPath.startsWith('/buying/orders')) {
      buyingOrdersDropdownOpen.value = true
    }
  }
  if (newPath.startsWith('/automation')) {
    automationDropdownOpen.value = true
  }
  if (newPath.startsWith('/onboarding')) {
    onboardingDropdownOpen.value = true
  }
  if (newPath.startsWith('/settings')) {
    settingsDropdownOpen.value = true
  }
}, { immediate: true })

// Toggle functions
const toggleStockDropdown = () => {
  stockDropdownOpen.value = !stockDropdownOpen.value
}

const toggleLogisticsDropdown = () => {
  logisticsDropdownOpen.value = !logisticsDropdownOpen.value
}

const toggleSalesDropdown = () => {
  salesDropdownOpen.value = !salesDropdownOpen.value
}

const toggleOrdersDropdown = () => {
  ordersDropdownOpen.value = !ordersDropdownOpen.value
}

const toggleBuyingDropdown = () => {
  buyingDropdownOpen.value = !buyingDropdownOpen.value
}

const toggleBuyingOrdersDropdown = () => {
  buyingOrdersDropdownOpen.value = !buyingOrdersDropdownOpen.value
}

const toggleAutomationDropdown = () => {
  automationDropdownOpen.value = !automationDropdownOpen.value
}

const toggleOnboardingDropdown = () => {
  onboardingDropdownOpen.value = !onboardingDropdownOpen.value
}

const toggleSettingsDropdown = () => {
  settingsDropdownOpen.value = !settingsDropdownOpen.value
}
</script>

<style scoped>
.nav-link {
  display: flex;
  align-items: center;
  padding: 0.625rem 0.75rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: rgb(75 85 99);
  border-radius: 0.5rem;
  transition: all 0.2s;
  text-decoration: none;
}

.nav-link:hover {
  background-color: rgb(243 244 246);
  color: rgb(17 24 39);
  text-decoration: none;
}

.nav-link-active {
  background: linear-gradient(to right, rgb(59 130 246), rgb(147 51 234));
  color: white;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
  text-decoration: none;
}

.nav-link-active:hover {
  background: linear-gradient(to right, rgb(37 99 235), rgb(126 34 206));
  text-decoration: none;
}

.nav-sub-link {
  display: block;
  padding: 0.5rem 0.75rem;
  font-size: 0.8125rem;
  font-weight: 400;
  color: rgb(107 114 128);
  border-radius: 0.375rem;
  transition: all 0.2s;
  text-decoration: none;
}

.nav-sub-link:hover {
  background-color: rgb(249 250 251);
  color: rgb(17 24 39);
  text-decoration: none;
}

.nav-sub-link-active {
  background-color: rgba(59, 130, 246, 0.1);
  color: rgb(59 130 246);
  font-weight: 500;
  text-decoration: none;
}

.nav-sub-link-active:hover {
  background-color: rgba(59, 130, 246, 0.2);
  text-decoration: none;
}

.nav-sub-sub-link {
  display: block;
  padding: 0.375rem 0.625rem;
  font-size: 0.75rem;
  font-weight: 400;
  color: rgb(107 114 128);
  border-radius: 0.375rem;
  transition: all 0.2s;
  text-decoration: none;
}

.nav-sub-sub-link:hover {
  background-color: rgb(249 250 251);
  color: rgb(17 24 39);
  text-decoration: none;
}

.nav-sub-sub-link-active {
  background-color: rgba(59, 130, 246, 0.08);
  color: rgb(59 130 246);
  font-weight: 500;
  text-decoration: none;
}
</style>
