<template>
  <aside 
    class="flex flex-col flex-shrink-0 border-r bg-white border-gray-200 shadow-sm transition-all duration-300"
    :class="isCollapsed ? 'w-16' : 'w-64'"
  >
    <!-- Logo Section -->
    <div class="flex items-center h-16 px-4 border-b border-gray-200" :class="isCollapsed ? 'justify-center' : 'justify-between'">
      <div class="flex items-center">
        <div class="flex items-center justify-center w-8 h-8 rounded-lg bg-gradient-to-r from-blue-500 to-purple-600" :class="isCollapsed ? '' : 'mr-3'">
          <span class="text-sm font-bold text-white">T</span>
        </div>
        <h1 v-if="!isCollapsed" class="text-xl font-bold text-gray-900">TOSS ERP</h1>
      </div>
      <button 
        @click="toggleCollapse"
        class="p-1.5 text-gray-400 hover:text-gray-600 rounded-lg hover:bg-gray-100 transition-colors"
        :title="isCollapsed ? 'Expand sidebar' : 'Collapse sidebar'"
      >
        <ChevronLeftIcon v-if="!isCollapsed" class="w-5 h-5" />
        <ChevronRightIcon v-else class="w-5 h-5" />
      </button>
    </div>
    
    <!-- Navigation -->
    <nav class="flex-1 px-3 py-6 space-y-2 overflow-y-auto">
      <!-- Home -->
      <NuxtLink 
        to="/" 
        class="nav-link"
        :class="{ 'nav-link-active': route.path === '/', 'justify-center': isCollapsed }"
        :title="isCollapsed ? 'Home' : ''"
      >
        <HomeIcon class="w-5 h-5" :class="isCollapsed ? '' : 'mr-3'" />
        <span v-if="!isCollapsed">Home</span>
      </NuxtLink>

      <!-- Dashboard -->
      <NuxtLink 
        to="/retailer/dashboard" 
        class="nav-link"
        :class="{ 'nav-link-active': route.path === '/retailer/dashboard' || route.path === '/dashboard', 'justify-center': isCollapsed }"
        :title="isCollapsed ? 'Dashboard' : ''"
      >
        <ChartBarIcon class="w-5 h-5" :class="isCollapsed ? '' : 'mr-3'" />
        <span v-if="!isCollapsed">Dashboard</span>
      </NuxtLink>
      
      <!-- Stock & Inventory Section -->
      <div class="space-y-1">
        <button 
          @click="toggleStockDropdown"
          class="w-full nav-link"
          :class="{ 
            'nav-link-active': route.path.startsWith('/stock') || route.path.startsWith('/retailer/inventory') || route.path.startsWith('/retailer/products'),
            'justify-center': isCollapsed,
            'justify-between': !isCollapsed
          }"
          :title="isCollapsed ? 'Stock & Inventory' : ''"
        >
          <div class="flex items-center">
            <ArchiveBoxIcon class="w-5 h-5" :class="isCollapsed ? '' : 'mr-3'" />
            <span v-if="!isCollapsed">Stock & Inventory</span>
          </div>
          <ChevronDownIcon 
            v-if="!isCollapsed"
            class="w-4 h-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': stockDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="stockDropdownOpen && !isCollapsed"
          class="pl-3 ml-6 space-y-1 border-l border-slate-700"
        >
          <NuxtLink to="/retailer/inventory" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/retailer/inventory' || route.path === '/stock' }">
            Stock Dashboard
          </NuxtLink>
          <NuxtLink to="/retailer/products" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path.startsWith('/retailer/products') || route.path === '/stock/items' }">
            Products
          </NuxtLink>
          <NuxtLink to="/buying/suppliers" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/stock/suppliers' || route.path === '/buying/suppliers' }">
            Suppliers
          </NuxtLink>
        </div>
      </div>

      <!-- Logistics Section -->
      <div class="space-y-1">
        <button 
          @click="toggleLogisticsDropdown"
          class="w-full nav-link"
          :class="{ 
            'nav-link-active': route.path.startsWith('/logistics'),
            'justify-center': isCollapsed,
            'justify-between': !isCollapsed
          }"
          :title="isCollapsed ? 'Logistics' : ''"
        >
          <div class="flex items-center">
            <TruckIcon class="w-5 h-5" :class="isCollapsed ? '' : 'mr-3'" />
            <span v-if="!isCollapsed">Logistics</span>
          </div>
          <ChevronDownIcon 
            v-if="!isCollapsed"
            class="w-4 h-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': logisticsDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="logisticsDropdownOpen && !isCollapsed"
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
          class="w-full nav-link"
          :class="{ 
            'nav-link-active': route.path.startsWith('/sales'),
            'justify-center': isCollapsed,
            'justify-between': !isCollapsed
          }"
          :title="isCollapsed ? 'Sales' : ''"
        >
          <div class="flex items-center">
            <ShoppingCartIcon class="w-5 h-5" :class="isCollapsed ? '' : 'mr-3'" />
            <span v-if="!isCollapsed">Sales</span>
          </div>
          <ChevronDownIcon 
            v-if="!isCollapsed"
            class="w-4 h-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': salesDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="salesDropdownOpen && !isCollapsed"
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
          <NuxtLink to="/sales/reports" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path.startsWith('/sales/reports') }">
            Reports
          </NuxtLink>
        </div>
      </div>

      <!-- Stores Section -->
      <div class="space-y-1">
        <button 
          @click="toggleStoresDropdown"
          class="w-full nav-link"
          :class="{ 
            'nav-link-active': route.path.startsWith('/stores'),
            'justify-center': isCollapsed,
            'justify-between': !isCollapsed
          }"
          :title="isCollapsed ? 'Stores' : ''"
        >
          <div class="flex items-center">
            <BuildingStorefrontIcon class="w-5 h-5" :class="isCollapsed ? '' : 'mr-3'" />
            <span v-if="!isCollapsed">Stores</span>
          </div>
          <ChevronDownIcon 
            v-if="!isCollapsed"
            class="w-4 h-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': storesDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="storesDropdownOpen && !isCollapsed"
          class="pl-3 ml-6 space-y-1 border-l border-slate-700"
        >
          <NuxtLink to="/stores" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/stores' }">
            All Stores
          </NuxtLink>
          <NuxtLink to="/stores/create" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/stores/create' }">
            Create Store
          </NuxtLink>
        </div>
      </div>

      <!-- Purchasing Section -->
      <div class="space-y-1">
        <button 
          @click="toggleBuyingDropdown"
          class="w-full nav-link"
          :class="{ 
            'nav-link-active': route.path.startsWith('/buying') || route.path.startsWith('/retailer/orders'),
            'justify-center': isCollapsed,
            'justify-between': !isCollapsed
          }"
          :title="isCollapsed ? 'Buying' : ''"
        >
          <div class="flex items-center">
            <ShoppingBagIcon class="w-5 h-5" :class="isCollapsed ? '' : 'mr-3'" />
            <span v-if="!isCollapsed">Buying</span>
          </div>
          <ChevronDownIcon 
            v-if="!isCollapsed"
            class="w-4 h-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': buyingDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="buyingDropdownOpen && !isCollapsed"
          class="pl-3 ml-6 space-y-1 border-l border-slate-700"
        >
          <NuxtLink to="/buying" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/buying' }">
            Buy Dashboard
          </NuxtLink>
          
          <!-- Purchase Orders Submenu -->
          <div class="space-y-1">
            <button 
              @click="toggleBuyingOrdersDropdown"
              class="justify-between w-full nav-sub-link"
              :class="{ 'nav-sub-link-active': route.path.startsWith('/buying/orders') || route.path.startsWith('/retailer/orders') }"
            >
              <div class="flex items-center w-full">
                <span class="flex-1 text-left">Purchase Orders</span>
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
              <NuxtLink to="/retailer/orders" class="nav-sub-sub-link" :class="{ 'nav-sub-sub-link-active': route.path === '/retailer/orders' || route.path === '/buying/orders' }">
                All Orders
              </NuxtLink>
              <NuxtLink to="/retailer/orders/new" class="nav-sub-sub-link" :class="{ 'nav-sub-sub-link-active': route.path === '/retailer/orders/new' || route.path === '/buying/orders/create-order' }">
                Create Order
              </NuxtLink>
            </div>
          </div>
          
          <NuxtLink to="/buying/invoices" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/buying/invoices' }">
            Invoices
          </NuxtLink>
          <NuxtLink to="/buying/group-buying" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/buying/group-buying' }">
            Group Buying
          </NuxtLink>
        </div>
      </div>

      <!-- CRM Section -->
      <div class="space-y-1">
        <button 
          @click="toggleCrmDropdown"
          class="w-full nav-link"
          :class="{ 
            'nav-link-active': route.path.startsWith('/crm'),
            'justify-center': isCollapsed,
            'justify-between': !isCollapsed
          }"
          :title="isCollapsed ? 'CRM' : ''"
        >
          <div class="flex items-center">
            <UsersIcon class="w-5 h-5" :class="isCollapsed ? '' : 'mr-3'" />
            <span v-if="!isCollapsed">CRM</span>
          </div>
          <ChevronDownIcon 
            v-if="!isCollapsed"
            class="w-4 h-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': crmDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="crmDropdownOpen && !isCollapsed"
          class="pl-3 ml-6 space-y-1 border-l border-slate-700"
        >
          <NuxtLink to="/crm" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/crm' }">
            CRM Dashboard
          </NuxtLink>
          <NuxtLink to="/crm/customers" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path === '/crm/customers' }">
            Customers
          </NuxtLink>
        </div>
      </div>

      <!-- Automation Section -->
      <div class="space-y-1">
        <button 
          @click="toggleAutomationDropdown"
          class="w-full nav-link"
          :class="{ 
            'nav-link-active': route.path.startsWith('/automation'),
            'justify-center': isCollapsed,
            'justify-between': !isCollapsed
          }"
          :title="isCollapsed ? 'Automation' : ''"
        >
          <div class="flex items-center">
            <CogIcon class="w-5 h-5" :class="isCollapsed ? '' : 'mr-3'" />
            <span v-if="!isCollapsed">Automation</span>
            <span v-if="!isCollapsed" class="ml-2 px-2 py-0.5 text-xs bg-blue-500 text-white rounded-full">AI</span>
          </div>
          <ChevronDownIcon 
            v-if="!isCollapsed"
            class="w-4 h-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': automationDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="automationDropdownOpen && !isCollapsed"
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
          class="w-full nav-link"
          :class="{ 
            'nav-link-active': route.path.startsWith('/onboarding') || route.path.startsWith('/retailer/onboarding'),
            'justify-center': isCollapsed,
            'justify-between': !isCollapsed
          }"
          :title="isCollapsed ? 'Onboarding' : ''"
        >
          <div class="flex items-center">
            <UserPlusIcon class="w-5 h-5" :class="isCollapsed ? '' : 'mr-3'" />
            <span v-if="!isCollapsed">Onboarding</span>
            <span v-if="!isCollapsed" class="ml-2 px-2 py-0.5 text-xs bg-green-500 text-white rounded-full">New</span>
          </div>
          <ChevronDownIcon 
            v-if="!isCollapsed"
            class="w-4 h-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': onboardingDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="onboardingDropdownOpen && !isCollapsed"
          class="pl-3 ml-6 space-y-1 border-l border-slate-700"
        >
          <NuxtLink to="/retailer/onboarding" class="nav-sub-link" :class="{ 'nav-sub-link-active': route.path.startsWith('/retailer/onboarding') || route.path === '/onboarding' }">
            Onboarding Dashboard
          </NuxtLink>
        </div>
      </div>


      <!-- Settings Section -->
      <div class="space-y-1">
        <button 
          @click="toggleSettingsDropdown"
          class="w-full nav-link"
          :class="{ 
            'nav-link-active': route.path.startsWith('/settings'),
            'justify-center': isCollapsed,
            'justify-between': !isCollapsed
          }"
          :title="isCollapsed ? 'Settings' : ''"
        >
          <div class="flex items-center">
            <Cog6ToothIcon class="w-5 h-5" :class="isCollapsed ? '' : 'mr-3'" />
            <span v-if="!isCollapsed">Settings</span>
          </div>
          <ChevronDownIcon 
            v-if="!isCollapsed"
            class="w-4 h-4 transition-transform duration-200"
            :class="{ 'transform rotate-180': settingsDropdownOpen }"
          />
        </button>
        
        <div 
          v-show="settingsDropdownOpen && !isCollapsed"
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
      <div class="text-xs text-center text-slate-400" :class="isCollapsed ? 'hidden' : ''">
        TOSS ERP v1.0.0
      </div>
    </div>
  </aside>
</template>

<script setup>
import { ref, watch, onMounted, onUnmounted } from 'vue'
import { 
  HomeIcon,
  ChartBarIcon, 
  ArchiveBoxIcon, 
  ShoppingCartIcon, 
  ShoppingBagIcon, 
  BuildingStorefrontIcon,
  ChevronDownIcon,
  ChevronLeftIcon,
  ChevronRightIcon,
  CogIcon,
  Cog6ToothIcon,
  TruckIcon,
  UserPlusIcon,
  UsersIcon
} from '@heroicons/vue/24/outline'

// Ensure router is available
const router = useRouter()
const route = useRoute()

// Collapse state
const isCollapsed = ref(false)

// Toggle collapse function
const toggleCollapse = () => {
  isCollapsed.value = !isCollapsed.value
  // Close all dropdowns when collapsing
  if (isCollapsed.value) {
    stockDropdownOpen.value = false
    logisticsDropdownOpen.value = false
    salesDropdownOpen.value = false
    ordersDropdownOpen.value = false
    storesDropdownOpen.value = false
    buyingDropdownOpen.value = false
    buyingOrdersDropdownOpen.value = false
    crmDropdownOpen.value = false
    automationDropdownOpen.value = false
    onboardingDropdownOpen.value = false
    settingsDropdownOpen.value = false
  }
}

// Dropdown states
const stockDropdownOpen = ref(false)
const logisticsDropdownOpen = ref(false)
const salesDropdownOpen = ref(false)
const ordersDropdownOpen = ref(false)
const storesDropdownOpen = ref(false)
const buyingDropdownOpen = ref(false)
const buyingOrdersDropdownOpen = ref(false)
const crmDropdownOpen = ref(false)
const automationDropdownOpen = ref(false)
const onboardingDropdownOpen = ref(false)
const settingsDropdownOpen = ref(false)

// Auto-open dropdowns if we're on a page within that section
watch(() => route.path, (newPath) => {
  if (newPath.startsWith('/stock') || newPath.startsWith('/retailer/inventory') || newPath.startsWith('/retailer/products')) {
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
  if (newPath.startsWith('/stores')) {
    storesDropdownOpen.value = true
  }
  if (newPath.startsWith('/buying') || newPath.startsWith('/retailer/orders')) {
    buyingDropdownOpen.value = true
    if (newPath.startsWith('/buying/orders') || newPath.startsWith('/retailer/orders')) {
      buyingOrdersDropdownOpen.value = true
    }
  }
  if (newPath.startsWith('/crm')) {
    crmDropdownOpen.value = true
  }
  if (newPath.startsWith('/automation')) {
    automationDropdownOpen.value = true
  }
  if (newPath.startsWith('/onboarding') || newPath.startsWith('/retailer/onboarding')) {
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

const toggleStoresDropdown = () => {
  storesDropdownOpen.value = !storesDropdownOpen.value
}

const toggleBuyingDropdown = () => {
  buyingDropdownOpen.value = !buyingDropdownOpen.value
}

const toggleBuyingOrdersDropdown = () => {
  buyingOrdersDropdownOpen.value = !buyingOrdersDropdownOpen.value
}

const toggleCrmDropdown = () => {
  crmDropdownOpen.value = !crmDropdownOpen.value
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

// Listen for fullscreen collapse event
onMounted(() => {
  const handleCollapseEvent = (event) => {
    if (event.detail?.collapse === true) {
      isCollapsed.value = true
      // Close all dropdowns
      stockDropdownOpen.value = false
      logisticsDropdownOpen.value = false
      salesDropdownOpen.value = false
      ordersDropdownOpen.value = false
      storesDropdownOpen.value = false
      buyingDropdownOpen.value = false
      buyingOrdersDropdownOpen.value = false
      crmDropdownOpen.value = false
      automationDropdownOpen.value = false
      onboardingDropdownOpen.value = false
      settingsDropdownOpen.value = false
    } else if (event.detail?.collapse === false) {
      isCollapsed.value = false
    }
  }
  
  window.addEventListener('collapse-sidebar', handleCollapseEvent)
  
  onUnmounted(() => {
    window.removeEventListener('collapse-sidebar', handleCollapseEvent)
  })
})
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
