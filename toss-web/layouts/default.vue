<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const sidebarMinimized = ref(false)
const notificationsOpen = ref(false)
const userMenuOpen = ref(false)
const searchQuery = ref('')
const salesMenuOpen = ref(false)
const buyingMenuOpen = ref(false)
const accountingMenuOpen = ref(false)
const logisticsMenuOpen = ref(false)
const projectsMenuOpen = ref(false)
const hrMenuOpen = ref(false)
const stockMenuOpen = ref(false)
const isChatbotOpen = ref(false)

// Notifications data
const notifications = ref([
  {
    id: 1,
    type: 'low_stock',
    title: 'Low Stock Alert',
    message: 'Sugar is running low (15 units remaining)',
    icon: 'inventory_2',
    color: 'red',
    time: '2 hours ago',
    read: false
  },
  {
    id: 2,
    type: 'payment',
    title: 'Payment Received',
    message: 'R 5,000 received from John Mkhize',
    icon: 'payments',
    color: 'green',
    time: '5 hours ago',
    read: false
  },
  {
    id: 3,
    type: 'order',
    title: 'New Order',
    message: 'New purchase order from Supplier ABC',
    icon: 'shopping_cart',
    color: 'blue',
    time: '1 day ago',
    read: true
  }
])

const unreadCount = computed(() => {
  return notifications.value.filter(n => !n.read).length
})

// Breadcrumbs
const breadcrumbs = computed(() => {
  const path = route.path
  const segments = path.split('/').filter(Boolean)
  
  if (segments.length === 0) {
    return [{ label: 'Dashboard', path: '/' }]
  }
  
  const crumbs = [{ label: 'Home', path: '/' }]
  
  let currentPath = ''
  segments.forEach((segment, index) => {
    currentPath += `/${segment}`
    const label = segment
      .split('-')
      .map(word => word.charAt(0).toUpperCase() + word.slice(1))
      .join(' ')
    crumbs.push({ label, path: currentPath })
  })
  
  return crumbs
})

const router = useRouter()

function handleSearch() {
  if (searchQuery.value.trim()) {
    // TODO: Implement global search
    router.push(`/search?q=${encodeURIComponent(searchQuery.value)}`)
  }
}

function markNotificationAsRead(id: number) {
  const notification = notifications.value.find(n => n.id === id)
  if (notification) {
    notification.read = true
  }
}

function clearAllNotifications() {
  notifications.value.forEach(n => n.read = true)
}

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
  notificationsOpen.value = false
  userMenuOpen.value = false
})

// Close dropdowns when clicking outside
onMounted(() => {
  document.addEventListener('click', (e) => {
    if (!(e.target as HTMLElement).closest('.relative')) {
      notificationsOpen.value = false
      userMenuOpen.value = false
    }
  })
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
          <div class="flex justify-center items-center w-8 h-8 bg-gradient-to-br from-gray-700 to-gray-900 rounded-lg">
            <i class="text-xl text-white material-symbols-rounded">store</i>
          </div>
            <span v-if="!sidebarMinimized" class="ml-2 text-sm font-semibold text-gray-900">TOSS</span>
        </NuxtLink>
      </div>

      <hr class="mx-3 my-2 border-gray-300">

      <!-- Navigation -->
      <div class="px-2 pb-4 overflow-y-auto h-[calc(100vh-120px)]">
        <ul class="space-y-0.5">
          <!-- User Profile Section -->
          <li class="mb-2">
            <button class="flex items-center px-3 py-2 w-full text-gray-900 rounded-lg transition-colors hover:bg-gray-100 group">
              <img src="https://ui-avatars.com/api/?name=Brooklyn+Alice&background=1f2937&color=fff" alt="User" class="w-8 h-8 rounded-full">
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">Brooklyn Alice</span>
              <i v-if="!sidebarMinimized" class="ml-auto text-sm text-gray-400 material-symbols-rounded">expand_more</i>
            </button>
          </li>

          <hr class="my-2 border-gray-300">

          <!-- Main Navigation -->
          <li>
            <NuxtLink
              to="/"
              class="flex items-center px-3 py-2 text-gray-900 rounded-lg transition-colors hover:bg-gray-100 group"
              active-class="!bg-gray-100 !text-gray-900"
            >
              <i class="text-xl text-gray-900 opacity-50 material-symbols-rounded group-hover:opacity-100">space_dashboard</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">Dashboard</span>
            </NuxtLink>
          </li>

          <li>
            <NuxtLink
              to="/pos"
              class="flex items-center px-3 py-2 text-gray-900 rounded-lg transition-colors hover:bg-gray-100 group"
              active-class="!bg-gray-100 !text-gray-900"
            >
              <i class="text-xl text-gray-900 opacity-50 material-symbols-rounded group-hover:opacity-100">point_of_sale</i>
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
              <i class="text-xl text-gray-900 opacity-50 material-symbols-rounded group-hover:opacity-100">inventory_2</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">Stock</span>
              <i 
                v-if="!sidebarMinimized" 
                class="ml-auto text-sm text-gray-400 transition-transform material-symbols-rounded"
                :class="{ 'rotate-180': stockMenuOpen }"
              >
                expand_more
              </i>
            </button>
            <!-- Stock Submenu -->
            <ul v-if="stockMenuOpen && !sidebarMinimized" class="mt-1 ml-8 space-y-0.5">
              <li>
                <NuxtLink
                  to="/stock/items"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
                  :class="route.path.startsWith('/stock/items') ? '!bg-gray-900 !text-white' : ''"
                >
                  Items
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/stock/movements"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
                  :class="route.path.startsWith('/stock/movements') ? '!bg-gray-900 !text-white' : ''"
                >
                  Movements
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/stock/reconciliation"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
                  :class="route.path.startsWith('/stock/reconciliation') ? '!bg-gray-900 !text-white' : ''"
                >
                  Reconciliation
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/stock/alerts"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
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
              class="flex items-center px-3 py-2 text-gray-900 rounded-lg transition-colors hover:bg-gray-100 group"
              active-class="!bg-gray-100 !text-gray-900"
            >
              <i class="text-xl text-gray-900 opacity-50 material-symbols-rounded group-hover:opacity-100">group</i>
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
              <i class="text-xl text-gray-900 opacity-50 material-symbols-rounded group-hover:opacity-100">receipt_long</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">Sales</span>
              <i 
                v-if="!sidebarMinimized" 
                class="ml-auto text-sm text-gray-400 transition-transform material-symbols-rounded"
                :class="{ 'rotate-180': salesMenuOpen }"
              >
                expand_more
              </i>
            </button>
            <!-- Sales Submenu -->
            <ul v-if="salesMenuOpen && !sidebarMinimized" class="mt-1 ml-8 space-y-0.5">
              <li>
                <NuxtLink
                  to="/sales/quotations"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
                  :class="route.path.startsWith('/sales/quotations') ? '!bg-gray-900 !text-white' : ''"
                >
                  Quotations
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/sales/orders"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
                  :class="route.path.startsWith('/sales/orders') ? '!bg-gray-900 !text-white' : ''"
                >
                  Orders
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/sales/invoices"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
                  :class="route.path.startsWith('/sales/invoices') ? '!bg-gray-900 !text-white' : ''"
                >
                  Invoices
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/sales/deliveries"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
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
              <i class="text-xl text-gray-900 opacity-50 material-symbols-rounded group-hover:opacity-100">shopping_cart</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">Buying</span>
              <i 
                v-if="!sidebarMinimized" 
                class="ml-auto text-sm text-gray-400 transition-transform material-symbols-rounded"
                :class="{ 'rotate-180': buyingMenuOpen }"
              >
                expand_more
              </i>
            </button>
            <!-- Buying Submenu -->
            <ul v-if="buyingMenuOpen && !sidebarMinimized" class="mt-1 ml-8 space-y-0.5">
              <li>
                <NuxtLink
                  to="/buying/purchase-orders"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
                  :class="route.path.startsWith('/buying/purchase-orders') ? '!bg-gray-900 !text-white' : ''"
                >
                  Purchase Orders
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/buying/suppliers"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
                  :class="route.path.startsWith('/buying/suppliers') ? '!bg-gray-900 !text-white' : ''"
                >
                  Suppliers
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/buying/receipts"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
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
              <i class="text-xl text-gray-900 opacity-50 material-symbols-rounded group-hover:opacity-100">account_balance</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">Accounting</span>
              <i 
                v-if="!sidebarMinimized" 
                class="ml-auto text-sm text-gray-400 transition-transform material-symbols-rounded"
                :class="{ 'rotate-180': accountingMenuOpen }"
              >
                expand_more
              </i>
            </button>
            <!-- Accounting Submenu -->
            <ul v-if="accountingMenuOpen && !sidebarMinimized" class="mt-1 ml-8 space-y-0.5">
              <li>
                <NuxtLink
                  to="/accounting/chart-of-accounts"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
                  :class="route.path.startsWith('/accounting/chart-of-accounts') ? '!bg-gray-900 !text-white' : ''"
                >
                  Chart of Accounts
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/accounting/journals"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
                  :class="route.path.startsWith('/accounting/journals') ? '!bg-gray-900 !text-white' : ''"
                >
                  Journal Entries
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/accounting/reports"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
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
              <i class="text-xl text-gray-900 opacity-50 material-symbols-rounded group-hover:opacity-100">local_shipping</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">Logistics</span>
              <i 
                v-if="!sidebarMinimized" 
                class="ml-auto text-sm text-gray-400 transition-transform material-symbols-rounded"
                :class="{ 'rotate-180': logisticsMenuOpen }"
              >
                expand_more
              </i>
            </button>
            <!-- Logistics Submenu -->
            <ul v-if="logisticsMenuOpen && !sidebarMinimized" class="mt-1 ml-8 space-y-0.5">
              <li>
                <NuxtLink
                  to="/logistics/drivers"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
                  :class="route.path.startsWith('/logistics/drivers') ? '!bg-gray-900 !text-white' : ''"
                >
                  Drivers
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/logistics/deliveries"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
                  :class="route.path.startsWith('/logistics/deliveries') ? '!bg-gray-900 !text-white' : ''"
                >
                  Deliveries
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/logistics/routes"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
                  :class="route.path.startsWith('/logistics/routes') ? '!bg-gray-900 !text-white' : ''"
                >
                  Routes
                </NuxtLink>
              </li>
            </ul>
          </li>

          <!-- MORE Section -->
          <hr class="mx-3 my-3 border-gray-300">
          
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
              <i class="text-xl text-gray-900 opacity-50 material-symbols-rounded group-hover:opacity-100">widgets</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">Projects</span>
              <i 
                v-if="!sidebarMinimized" 
                class="ml-auto text-sm text-gray-400 transition-transform material-symbols-rounded"
                :class="{ 'rotate-180': projectsMenuOpen }"
              >
                expand_more
              </i>
            </button>
            <!-- Projects Submenu -->
            <ul v-if="projectsMenuOpen && !sidebarMinimized" class="mt-1 ml-8 space-y-0.5">
              <li>
                <NuxtLink
                  to="/projects/list"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
                  :class="route.path.startsWith('/projects/list') ? '!bg-gray-900 !text-white' : ''"
                >
                  All Projects
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/projects/tasks"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
                  :class="route.path.startsWith('/projects/tasks') ? '!bg-gray-900 !text-white' : ''"
                >
                  Tasks
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/projects/time-tracking"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
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
              <i class="text-xl text-gray-900 opacity-50 material-symbols-rounded group-hover:opacity-100">badge</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">HR & Payroll</span>
              <i 
                v-if="!sidebarMinimized" 
                class="ml-auto text-sm text-gray-400 transition-transform material-symbols-rounded"
                :class="{ 'rotate-180': hrMenuOpen }"
              >
                expand_more
              </i>
            </button>
            <!-- HR Submenu -->
            <ul v-if="hrMenuOpen && !sidebarMinimized" class="mt-1 ml-8 space-y-0.5">
              <li>
                <NuxtLink
                  to="/hr/employees"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
                  :class="route.path.startsWith('/hr/employees') ? '!bg-gray-900 !text-white' : ''"
                >
                  Employees
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/hr/attendance"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
                  :class="route.path.startsWith('/hr/attendance') ? '!bg-gray-900 !text-white' : ''"
                >
                  Attendance
                </NuxtLink>
              </li>
              <li>
                <NuxtLink
                  to="/hr/payroll"
                  class="flex items-center px-3 py-2 text-sm text-gray-700 rounded-lg transition-colors hover:bg-gray-100"
                  :class="route.path.startsWith('/hr/payroll') ? '!bg-gray-900 !text-white' : ''"
                >
                  Payroll
                </NuxtLink>
              </li>
            </ul>
          </li>

          <li>
            <NuxtLink
              to="/copilot"
              class="flex items-center px-3 py-2 text-gray-900 rounded-lg transition-colors hover:bg-gray-100 group"
              active-class="!bg-gray-100 !text-gray-900"
            >
              <i class="text-xl text-gray-900 opacity-50 material-symbols-rounded group-hover:opacity-100">psychology</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">AI Copilot</span>
            </NuxtLink>
          </li>

          <!-- SETTINGS Section -->
          <li class="mt-3">
            <hr class="mb-3 border-gray-300">
          </li>
          
          <li>
            <NuxtLink
              to="/settings"
              class="flex items-center px-3 py-2 text-gray-900 rounded-lg transition-colors hover:bg-gray-100 group"
              active-class="!bg-gray-100 !text-gray-900"
            >
              <i class="text-xl text-gray-900 opacity-50 material-symbols-rounded group-hover:opacity-100">settings</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">Settings</span>
            </NuxtLink>
          </li>
          
          <li>
            <NuxtLink
              to="/help"
              class="flex items-center px-3 py-2 text-gray-900 rounded-lg transition-colors hover:bg-gray-100 group"
              active-class="!bg-gray-100 !text-gray-900"
            >
              <i class="text-xl text-gray-900 opacity-50 material-symbols-rounded group-hover:opacity-100">help</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">Help & Support</span>
            </NuxtLink>
          </li>
        </ul>
      </div>
    </aside>

    <!-- Main Content Area -->
    <main :class="['transition-all duration-300', sidebarMinimized ? 'ml-24' : 'ml-72']">
      <!-- Top Navbar -->
      <nav class="sticky top-2 z-30 px-4 py-2 mx-3 mt-2 rounded-xl shadow-md backdrop-blur-md bg-white/80">
        <div class="flex justify-between items-center">
          <!-- Left: Sidebar Toggle & Breadcrumbs -->
          <div class="flex gap-4 items-center">
            <button
              @click="toggleSidebarMinimize"
              class="p-2 text-gray-600 transition-colors hover:text-gray-900"
            >
              <div class="flex flex-col gap-1">
                <span class="w-5 h-0.5 bg-current"></span>
                <span class="w-5 h-0.5 bg-current"></span>
                <span class="w-5 h-0.5 bg-current"></span>
              </div>
            </button>

            <nav aria-label="breadcrumb">
              <ol class="flex gap-2 items-center text-sm">
                <li
                  v-for="(crumb, index) in breadcrumbs"
                  :key="index"
                  class="flex gap-2 items-center"
                >
                  <NuxtLink
                    v-if="index < breadcrumbs.length - 1"
                    :to="crumb.path"
                    class="text-gray-500 hover:text-gray-700"
                  >
                    {{ crumb.label }}
                  </NuxtLink>
                  <span v-else class="font-semibold text-gray-900">{{ crumb.label }}</span>
                  <span v-if="index < breadcrumbs.length - 1" class="text-gray-400">/</span>
                </li>
              </ol>
            </nav>
          </div>

          <!-- Right: Search, Notifications, Settings, User -->
          <div class="flex gap-3 items-center">
            <!-- Search -->
            <div class="hidden relative md:block">
              <form @submit.prevent="handleSearch">
                <input
                  v-model="searchQuery"
                  type="text"
                  placeholder="Search here"
                  class="px-4 py-2 pr-10 w-64 text-sm bg-transparent rounded-lg border border-gray-300 transition-colors focus:outline-none focus:border-gray-900"
                >
                <button type="submit" class="absolute right-3 top-1/2 text-gray-400 -translate-y-1/2 hover:text-gray-600">
                  <i class="text-lg material-symbols-rounded">search</i>
                </button>
              </form>
            </div>

            <!-- User Menu -->
            <div class="relative">
              <button
                @click="userMenuOpen = !userMenuOpen"
                class="p-2 text-gray-600 transition-colors hover:text-gray-900"
              >
                <i class="material-symbols-rounded">account_circle</i>
              </button>
              <!-- User Dropdown -->
              <div
                v-if="userMenuOpen"
                class="absolute right-0 z-50 py-2 mt-2 w-48 bg-white rounded-xl border border-gray-200 shadow-lg"
                @click.stop
              >
                <NuxtLink
                  to="/settings"
                  class="flex gap-3 items-center px-4 py-2 transition-colors hover:bg-gray-50"
                  @click="userMenuOpen = false"
                >
                  <i class="text-base text-gray-600 material-symbols-rounded">person</i>
                  <span class="text-sm text-gray-900">My Profile</span>
                </NuxtLink>
                <NuxtLink
                  to="/settings"
                  class="flex gap-3 items-center px-4 py-2 transition-colors hover:bg-gray-50"
                  @click="userMenuOpen = false"
                >
                  <i class="text-base text-gray-600 material-symbols-rounded">settings</i>
                  <span class="text-sm text-gray-900">Settings</span>
                </NuxtLink>
                <hr class="my-2 border-gray-200">
                <button
                  class="flex gap-3 items-center px-4 py-2 w-full text-left transition-colors hover:bg-gray-50"
                  @click="userMenuOpen = false"
                >
                  <i class="text-base text-gray-600 material-symbols-rounded">logout</i>
                  <span class="text-sm text-gray-900">Logout</span>
                </button>
              </div>
            </div>

            <!-- Settings -->
            <NuxtLink
              to="/settings"
              class="p-2 text-gray-600 transition-colors hover:text-gray-900"
            >
              <i class="material-symbols-rounded">settings</i>
            </NuxtLink>

            <!-- Notifications -->
            <div class="relative">
              <button
                @click="notificationsOpen = !notificationsOpen"
                class="relative p-2 text-gray-600 transition-colors hover:text-gray-900"
              >
                <i class="material-symbols-rounded">notifications</i>
                <span
                  v-if="unreadCount > 0"
                  class="flex absolute top-0 right-0 justify-center items-center w-4 h-4 text-xs text-white bg-red-500 rounded-full"
                >
                  {{ unreadCount > 9 ? '9+' : unreadCount }}
                </span>
              </button>

              <!-- Notifications Dropdown -->
              <div
                v-if="notificationsOpen"
                class="overflow-y-auto absolute right-0 z-50 py-2 mt-2 w-80 max-h-96 bg-white rounded-xl border border-gray-200 shadow-lg"
                @click.stop
              >
                <div class="flex justify-between items-center px-4 py-2 border-b border-gray-200">
                  <h6 class="text-sm font-semibold text-gray-900">Notifications</h6>
                  <button
                    v-if="unreadCount > 0"
                    @click="clearAllNotifications"
                    class="text-xs text-blue-600 hover:text-blue-800"
                  >
                    Mark all read
                  </button>
                </div>
                <div v-if="notifications.length === 0" class="px-4 py-8 text-center">
                  <i class="mb-2 text-4xl text-gray-400 material-symbols-rounded">notifications_off</i>
                  <p class="text-sm text-gray-600">No notifications</p>
                </div>
                <div v-else>
                  <button
                    v-for="notification in notifications"
                    :key="notification.id"
                    @click="markNotificationAsRead(notification.id)"
                    class="flex gap-3 items-start px-4 py-3 w-full text-left border-b border-gray-100 transition-colors hover:bg-gray-50 last:border-0"
                    :class="{ 'bg-blue-50': !notification.read }"
                  >
                    <div
                      class="flex flex-shrink-0 justify-center items-center w-8 h-8 rounded-lg"
                      :class="{
                        'bg-red-100': notification.color === 'red',
                        'bg-green-100': notification.color === 'green',
                        'bg-blue-100': notification.color === 'blue',
                        'bg-orange-100': notification.color === 'orange'
                      }"
                    >
                      <i
                        class="text-base material-symbols-rounded"
                        :class="{
                          'text-red-600': notification.color === 'red',
                          'text-green-600': notification.color === 'green',
                          'text-blue-600': notification.color === 'blue',
                          'text-orange-600': notification.color === 'orange'
                        }"
                      >
                        {{ notification.icon }}
                      </i>
                    </div>
                    <div class="flex-1 min-w-0">
                      <h6 class="text-sm font-semibold text-gray-900">{{ notification.title }}</h6>
                      <p class="mt-1 text-xs text-gray-600">{{ notification.message }}</p>
                      <p class="mt-1 text-xs text-gray-400">{{ notification.time }}</p>
                    </div>
                    <div v-if="!notification.read" class="flex-shrink-0 mt-2 w-2 h-2 bg-blue-600 rounded-full"></div>
                  </button>
                </div>
              </div>
            </div>

            <!-- Mobile Sidebar Toggle -->
            <button
              @click="toggleSidebarMinimize"
              class="p-2 text-gray-600 transition-colors hover:text-gray-900 xl:hidden"
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

    <!-- AI Copilot Chatbot Popup -->
    <ClientOnly>
      <CopilotChatbot v-model:isOpen="isChatbotOpen" />
    </ClientOnly>
  </div>
</template>

<style>
.material-symbols-rounded {
  font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
}
</style>
