<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()
const sidebarMinimized = ref(false)
const notificationsOpen = ref(false)
const salesMenuOpen = ref(false)
const buyingMenuOpen = ref(false)
const accountingMenuOpen = ref(false)
const logisticsMenuOpen = ref(false)
const projectsMenuOpen = ref(false)
const hrMenuOpen = ref(false)
const stockMenuOpen = ref(false)

const toggleSidebarMinimize = () => {
  sidebarMinimized.value = !sidebarMinimized.value
}

// Function to update menu states based on current route
function updateMenuStates() {
  const path = route.path

  // Reset all menus
  stockMenuOpen.value = false
  salesMenuOpen.value = false
  buyingMenuOpen.value = false
  accountingMenuOpen.value = false
  logisticsMenuOpen.value = false
  projectsMenuOpen.value = false
  hrMenuOpen.value = false

  // Auto-expand menu based on current route
  if (path.startsWith('/stock')) {
    stockMenuOpen.value = true
  } else if (path.startsWith('/sales')) {
    salesMenuOpen.value = true
  } else if (path.startsWith('/buying')) {
    buyingMenuOpen.value = true
  } else if (path.startsWith('/accounting')) {
    accountingMenuOpen.value = true
  } else if (path.startsWith('/logistics')) {
    logisticsMenuOpen.value = true
  } else if (path.startsWith('/projects')) {
    projectsMenuOpen.value = true
  } else if (path.startsWith('/hr')) {
    hrMenuOpen.value = true
  }
}

// Watch for route changes
watch(() => route.path, () => {
  updateMenuStates()
})

// Initialize menu states on mount
onMounted(() => {
  updateMenuStates()
})
</script>

<template>
  <div class="min-h-screen bg-gray-100">
    <!-- Offline Indicator -->
    <OfflineIndicator />
    <!-- Sidebar -->
    <aside
      :class="[
        'fixed top-0 left-0 z-40 h-screen transition-all duration-300 my-2 ms-2 bg-white rounded-xl shadow-lg',
        sidebarMinimized ? 'w-20' : 'w-64'
      ]"
    >
      <!-- Sidebar Header -->
      <div class="px-4 py-3">
        <NuxtLink to="/" class="flex items-center">
          <div class="w-8 h-8 bg-gradient-to-br from-gray-700 to-gray-900 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-white text-xl">store</i>
          </div>
            <span v-if="!sidebarMinimized" class="ml-2 text-sm font-semibold text-gray-900">TOSS</span>
        </NuxtLink>
      </div>

      <hr class="border-gray-300 my-2 mx-3">

      <!-- Navigation -->
      <div class="px-2 pb-4 overflow-y-auto h-[calc(100vh-120px)]">
        <ul class="space-y-0.5">
          <!-- User Profile Section -->
          <li class="mb-2">
            <button class="flex items-center w-full px-3 py-2 text-gray-900 rounded-lg hover:bg-gray-100 transition-colors group">
              <img src="https://ui-avatars.com/api/?name=Brooklyn+Alice&background=1f2937&color=fff" alt="User" class="w-8 h-8 rounded-full">
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">Brooklyn Alice</span>
              <i v-if="!sidebarMinimized" class="material-symbols-rounded ml-auto text-gray-400 text-sm">expand_more</i>
            </button>
          </li>

          <hr class="border-gray-300 my-2">

          <!-- Main Navigation -->
          <li>
            <NuxtLink
              to="/"
              class="flex items-center px-3 py-2 text-gray-900 rounded-lg hover:bg-gray-100 transition-colors group"
              active-class="!bg-gray-100 !text-gray-900"
            >
              <i class="material-symbols-rounded opacity-50 text-gray-900 group-hover:opacity-100 text-xl">space_dashboard</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">Dashboard</span>
            </NuxtLink>
          </li>

          <li>
            <NuxtLink
              to="/pos"
              class="flex items-center px-3 py-2 text-gray-900 rounded-lg hover:bg-gray-100 transition-colors group"
              active-class="!bg-gray-100 !text-gray-900"
            >
              <i class="material-symbols-rounded opacity-50 text-gray-900 group-hover:opacity-100 text-xl">point_of_sale</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">POS</span>
            </NuxtLink>
          </li>

          <li>
            <button 
              @click="stockMenuOpen = !stockMenuOpen"
              :class="[
                'flex items-center w-full px-3 py-2 rounded-lg transition-colors group',
                route.path.startsWith('/stock') 
                  ? 'bg-gray-100 text-gray-900' 
                  : 'text-gray-900 hover:bg-gray-100'
              ]"
            >
              <i class="material-symbols-rounded opacity-50 text-gray-900 group-hover:opacity-100 text-xl">inventory_2</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">Stock</span>
              <i 
                v-if="!sidebarMinimized" 
                class="material-symbols-rounded ml-auto text-gray-400 text-sm transition-transform"
                :class="{ 'rotate-180': stockMenuOpen }"
              >
                expand_more
              </i>
            </button>
            <!-- Stock Submenu -->
            <ul v-if="stockMenuOpen && !sidebarMinimized" class="ml-8 mt-1 space-y-0.5">
              <li>
                <NuxtLink
                  to="/stock/items"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/stock/items') ? '!bg-gray-900 !text-white' : ''"
                >
                  Items
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/stock/movements"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/stock/movements') ? '!bg-gray-900 !text-white' : ''"
                >
                  Movements
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/stock/reconciliation"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/stock/reconciliation') ? '!bg-gray-900 !text-white' : ''"
                >
                  Reconciliation
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/stock/alerts"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/stock/alerts') ? '!bg-gray-900 !text-white' : ''"
                >
                  Alerts
                </NuxtLink>
              </li>
            </ul>
          </li>

          <li>
            <NuxtLink
              to="/customers"
              class="flex items-center px-3 py-2 text-gray-900 rounded-lg hover:bg-gray-100 transition-colors group"
              active-class="!bg-gray-100 !text-gray-900"
            >
              <i class="material-symbols-rounded opacity-50 text-gray-900 group-hover:opacity-100 text-xl">group</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">Customers</span>
            </NuxtLink>
          </li>

          <li>
            <button 
              @click="salesMenuOpen = !salesMenuOpen"
              :class="[
                'flex items-center w-full px-3 py-2 rounded-lg transition-colors group',
                route.path.startsWith('/sales') 
                  ? 'bg-gray-100 text-gray-900' 
                  : 'text-gray-900 hover:bg-gray-100'
              ]"
            >
              <i class="material-symbols-rounded opacity-50 text-gray-900 group-hover:opacity-100 text-xl">receipt_long</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">Sales</span>
              <i 
                v-if="!sidebarMinimized" 
                class="material-symbols-rounded ml-auto text-gray-400 text-sm transition-transform"
                :class="{ 'rotate-180': salesMenuOpen }"
              >
                expand_more
              </i>
            </button>
            <!-- Sales Submenu -->
            <ul v-if="salesMenuOpen && !sidebarMinimized" class="ml-8 mt-1 space-y-0.5">
              <li>
                <NuxtLink
                  to="/sales/quotations"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/sales/quotations') ? '!bg-gray-900 !text-white' : ''"
                >
                  Quotations
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/sales/orders"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/sales/orders') ? '!bg-gray-900 !text-white' : ''"
                >
                  Orders
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/sales/invoices"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/sales/invoices') ? '!bg-gray-900 !text-white' : ''"
                >
                  Invoices
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/sales/deliveries"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/sales/deliveries') ? '!bg-gray-900 !text-white' : ''"
                >
                  Deliveries
                </NuxtLink>
              </li>
            </ul>
          </li>

          <li>
            <button 
              @click="buyingMenuOpen = !buyingMenuOpen"
              :class="[
                'flex items-center w-full px-3 py-2 rounded-lg transition-colors group',
                route.path.startsWith('/buying') 
                  ? 'bg-gray-100 text-gray-900' 
                  : 'text-gray-900 hover:bg-gray-100'
              ]"
            >
              <i class="material-symbols-rounded opacity-50 text-gray-900 group-hover:opacity-100 text-xl">shopping_cart</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">Buying</span>
              <i 
                v-if="!sidebarMinimized" 
                class="material-symbols-rounded ml-auto text-gray-400 text-sm transition-transform"
                :class="{ 'rotate-180': buyingMenuOpen }"
              >
                expand_more
              </i>
            </button>
            <!-- Buying Submenu -->
            <ul v-if="buyingMenuOpen && !sidebarMinimized" class="ml-8 mt-1 space-y-0.5">
              <li>
                <NuxtLink
                  to="/buying/purchase-orders"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/buying/purchase-orders') ? '!bg-gray-900 !text-white' : ''"
                >
                  Purchase Orders
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/buying/suppliers"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/buying/suppliers') ? '!bg-gray-900 !text-white' : ''"
                >
                  Suppliers
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/buying/receipts"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/buying/receipts') ? '!bg-gray-900 !text-white' : ''"
                >
                  Goods Receipts
                </NuxtLink>
              </li>
            </ul>
          </li>

          <li>
            <button 
              @click="accountingMenuOpen = !accountingMenuOpen"
              :class="[
                'flex items-center w-full px-3 py-2 rounded-lg transition-colors group',
                route.path.startsWith('/accounting') 
                  ? 'bg-gray-100 text-gray-900' 
                  : 'text-gray-900 hover:bg-gray-100'
              ]"
            >
              <i class="material-symbols-rounded opacity-50 text-gray-900 group-hover:opacity-100 text-xl">account_balance</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">Accounting</span>
              <i 
                v-if="!sidebarMinimized" 
                class="material-symbols-rounded ml-auto text-gray-400 text-sm transition-transform"
                :class="{ 'rotate-180': accountingMenuOpen }"
              >
                expand_more
              </i>
            </button>
            <!-- Accounting Submenu -->
            <ul v-if="accountingMenuOpen && !sidebarMinimized" class="ml-8 mt-1 space-y-0.5">
              <li>
                <NuxtLink
                  to="/accounting/chart-of-accounts"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/accounting/chart-of-accounts') ? '!bg-gray-900 !text-white' : ''"
                >
                  Chart of Accounts
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/accounting/journals"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/accounting/journals') ? '!bg-gray-900 !text-white' : ''"
                >
                  Journal Entries
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/accounting/reports"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/accounting/reports') ? '!bg-gray-900 !text-white' : ''"
                >
                  Reports
                </NuxtLink>
              </li>
            </ul>
          </li>

          <li>
            <button 
              @click="logisticsMenuOpen = !logisticsMenuOpen"
              :class="[
                'flex items-center w-full px-3 py-2 rounded-lg transition-colors group',
                route.path.startsWith('/logistics') 
                  ? 'bg-gray-100 text-gray-900' 
                  : 'text-gray-900 hover:bg-gray-100'
              ]"
            >
              <i class="material-symbols-rounded opacity-50 text-gray-900 group-hover:opacity-100 text-xl">local_shipping</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">Logistics</span>
              <i 
                v-if="!sidebarMinimized" 
                class="material-symbols-rounded ml-auto text-gray-400 text-sm transition-transform"
                :class="{ 'rotate-180': logisticsMenuOpen }"
              >
                expand_more
              </i>
            </button>
            <!-- Logistics Submenu -->
            <ul v-if="logisticsMenuOpen && !sidebarMinimized" class="ml-8 mt-1 space-y-0.5">
              <li>
                <NuxtLink
                  to="/logistics/drivers"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/logistics/drivers') ? '!bg-gray-900 !text-white' : ''"
                >
                  Drivers
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/logistics/deliveries"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/logistics/deliveries') ? '!bg-gray-900 !text-white' : ''"
                >
                  Deliveries
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/logistics/routes"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/logistics/routes') ? '!bg-gray-900 !text-white' : ''"
                >
                  Routes
                </NuxtLink>
              </li>
            </ul>
          </li>

          <!-- MORE Section -->
          <hr class="border-gray-300 my-3 mx-3">
          
          <li>
            <button 
              @click="projectsMenuOpen = !projectsMenuOpen"
              :class="[
                'flex items-center w-full px-3 py-2 rounded-lg transition-colors group',
                route.path.startsWith('/projects') 
                  ? 'bg-gray-100 text-gray-900' 
                  : 'text-gray-900 hover:bg-gray-100'
              ]"
            >
              <i class="material-symbols-rounded opacity-50 text-gray-900 group-hover:opacity-100 text-xl">widgets</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">Projects</span>
              <i 
                v-if="!sidebarMinimized" 
                class="material-symbols-rounded ml-auto text-gray-400 text-sm transition-transform"
                :class="{ 'rotate-180': projectsMenuOpen }"
              >
                expand_more
              </i>
            </button>
            <!-- Projects Submenu -->
            <ul v-if="projectsMenuOpen && !sidebarMinimized" class="ml-8 mt-1 space-y-0.5">
              <li>
                <NuxtLink
                  to="/projects/list"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/projects/list') ? '!bg-gray-900 !text-white' : ''"
                >
                  All Projects
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/projects/tasks"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/projects/tasks') ? '!bg-gray-900 !text-white' : ''"
                >
                  Tasks
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/projects/time-tracking"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/projects/time-tracking') ? '!bg-gray-900 !text-white' : ''"
                >
                  Time Tracking
                </NuxtLink>
              </li>
            </ul>
          </li>

          <li>
            <button 
              @click="hrMenuOpen = !hrMenuOpen"
              :class="[
                'flex items-center w-full px-3 py-2 rounded-lg transition-colors group',
                route.path.startsWith('/hr') 
                  ? 'bg-gray-100 text-gray-900' 
                  : 'text-gray-900 hover:bg-gray-100'
              ]"
            >
              <i class="material-symbols-rounded opacity-50 text-gray-900 group-hover:opacity-100 text-xl">badge</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">HR & Payroll</span>
              <i 
                v-if="!sidebarMinimized" 
                class="material-symbols-rounded ml-auto text-gray-400 text-sm transition-transform"
                :class="{ 'rotate-180': hrMenuOpen }"
              >
                expand_more
              </i>
            </button>
            <!-- HR Submenu -->
            <ul v-if="hrMenuOpen && !sidebarMinimized" class="ml-8 mt-1 space-y-0.5">
              <li>
                <NuxtLink
                  to="/hr/employees"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/hr/employees') ? '!bg-gray-900 !text-white' : ''"
                >
                  Employees
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/hr/attendance"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/hr/attendance') ? '!bg-gray-900 !text-white' : ''"
                >
                  Attendance
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/hr/payroll"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg hover:bg-gray-100 transition-colors"
                  :class="route.path.startsWith('/hr/payroll') ? '!bg-gray-900 !text-white' : ''"
                >
                  Payroll
                </NuxtLink>
              </li>
            </ul>
          </li>

          <li>
            <button class="flex items-center w-full px-3 py-2 text-gray-900 rounded-lg hover:bg-gray-100 transition-colors group">
              <i class="material-symbols-rounded opacity-50 text-gray-900 group-hover:opacity-100 text-xl">psychology</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">AI Copilot</span>
            </button>
          </li>

          <!-- SETTINGS Section -->
          <li class="mt-3">
            <hr class="border-gray-300 mb-3">
          </li>
          
          <li>
            <button class="flex items-center w-full px-3 py-2 text-gray-900 rounded-lg hover:bg-gray-100 transition-colors group">
              <i class="material-symbols-rounded opacity-50 text-gray-900 group-hover:opacity-100 text-xl">settings</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">Settings</span>
            </button>
          </li>
          
          <li>
            <button class="flex items-center w-full px-3 py-2 text-gray-900 rounded-lg hover:bg-gray-100 transition-colors group">
              <i class="material-symbols-rounded opacity-50 text-gray-900 group-hover:opacity-100 text-xl">help</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">Help & Support</span>
            </button>
          </li>
        </ul>
      </div>
    </aside>

    <!-- Main Content Area -->
    <main :class="['transition-all duration-300', sidebarMinimized ? 'ml-24' : 'ml-72']">
      <!-- Top Navbar -->
      <nav class="sticky top-2 z-30 mx-3 mt-2 rounded-xl bg-white/80 backdrop-blur-md shadow-md px-4 py-2">
        <div class="flex items-center justify-between">
          <!-- Left: Sidebar Toggle & Breadcrumbs -->
          <div class="flex items-center gap-4">
            <button
              @click="toggleSidebarMinimize"
              class="p-2 text-gray-600 hover:text-gray-900 transition-colors"
            >
              <div class="flex flex-col gap-1">
                <span class="w-5 h-0.5 bg-current"></span>
                <span class="w-5 h-0.5 bg-current"></span>
                <span class="w-5 h-0.5 bg-current"></span>
              </div>
            </button>

            <nav aria-label="breadcrumb">
              <ol class="flex items-center gap-2 text-sm">
                <li>
                  <a href="javascript:;" class="text-gray-500 hover:text-gray-700">Pages</a>
                </li>
                <li class="text-gray-400">/</li>
                <li class="text-gray-900 font-semibold">Analytics</li>
              </ol>
            </nav>
          </div>

          <!-- Right: Search, Notifications, Settings, User -->
          <div class="flex items-center gap-3">
            <!-- Search -->
            <div class="relative hidden md:block">
              <input
                type="text"
                placeholder="Search here"
                class="px-4 py-2 pr-10 text-sm bg-transparent border border-gray-300 rounded-lg focus:outline-none focus:border-gray-900 transition-colors w-64"
              >
              <i class="material-symbols-rounded absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 text-lg">search</i>
            </div>

            <!-- User Icon -->
            <button class="p-2 text-gray-600 hover:text-gray-900 transition-colors">
              <i class="material-symbols-rounded">account_circle</i>
            </button>

            <!-- Settings -->
            <button class="p-2 text-gray-600 hover:text-gray-900 transition-colors">
              <i class="material-symbols-rounded">settings</i>
            </button>

            <!-- Notifications -->
            <div class="relative">
              <button
                @click="notificationsOpen = !notificationsOpen"
                class="p-2 text-gray-600 hover:text-gray-900 transition-colors relative"
              >
                <i class="material-symbols-rounded">notifications</i>
                <span class="absolute top-0 right-0 w-4 h-4 bg-red-500 text-white text-xs rounded-full flex items-center justify-center">
                  11
                </span>
              </button>

              <!-- Notifications Dropdown -->
              <div
                v-if="notificationsOpen"
                class="absolute right-0 mt-2 w-80 bg-white rounded-xl shadow-lg border border-gray-200 py-2"
              >
                <a
                  href="javascript:;"
                  class="flex items-start gap-3 px-4 py-3 hover:bg-gray-50 transition-colors"
                >
                  <i class="material-symbols-rounded text-gray-900">email</i>
                  <div>
                    <h6 class="text-sm font-semibold text-gray-900">Check new messages</h6>
                  </div>
                </a>
                <a
                  href="javascript:;"
                  class="flex items-start gap-3 px-4 py-3 hover:bg-gray-50 transition-colors"
                >
                  <i class="material-symbols-rounded text-green-500">podcasts</i>
                  <div>
                    <h6 class="text-sm font-semibold text-gray-900">Manage podcast session</h6>
                  </div>
                </a>
                <a
                  href="javascript:;"
                  class="flex items-start gap-3 px-4 py-3 hover:bg-gray-50 transition-colors"
                >
                  <i class="material-symbols-rounded text-orange-500">shopping_cart</i>
                  <div>
                    <h6 class="text-sm font-semibold text-gray-900">Payment successfully completed</h6>
                  </div>
                </a>
              </div>
            </div>

            <!-- Mobile Sidebar Toggle -->
            <button
              @click="toggleSidebarMinimize"
              class="p-2 text-gray-600 hover:text-gray-900 transition-colors xl:hidden"
            >
              <div class="flex flex-col gap-1">
                <span class="w-5 h-0.5 bg-current"></span>
                <span class="w-5 h-0.5 bg-current"></span>
                <span class="w-5 h-0.5 bg-current"></span>
              </div>
            </button>
          </div>
        </div>
      </nav>

      <!-- Page Content -->
      <div class="px-4 py-4 pb-12">
        <slot />
      </div>
    </main>
  </div>
</template>

<style>
.material-symbols-rounded {
  font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
}
</style>
