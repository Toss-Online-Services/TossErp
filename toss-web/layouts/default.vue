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
  
  // Ensure Bootstrap is loaded and initialize collapse elements
  if (typeof window !== 'undefined') {
    // Wait for Bootstrap to be loaded
    const initializeBootstrap = () => {
      if ((window as any).bootstrap) {
        console.log('Bootstrap loaded, initializing sidebar...')
        // Material Dashboard should auto-initialize, but we can help
        const sidenav = document.getElementById('sidenav-main')
        if (sidenav) {
          console.log('Sidebar found:', sidenav)
          // Force visibility
          sidenav.style.display = 'block'
        }
      } else {
        console.log('Waiting for Bootstrap...')
        setTimeout(initializeBootstrap, 100)
      }
    }
    initializeBootstrap()
  }
})
</script>

<template>
  <div class="min-vh-100">
    <!-- Offline Indicator -->
    <OfflineIndicator />
    <!-- Sidebar -->
    <aside
      :class="[
        'sidenav navbar navbar-vertical navbar-expand-xs border-0 border-radius-xl my-3 fixed-start ms-3 bg-white',
        sidebarMinimized ? 'sidenav-mini' : ''
      ]"
      id="sidenav-main"
    >
      <!-- Sidebar Header -->
      <div class="sidenav-header">
        <NuxtLink to="/" class="navbar-brand m-0">
          <span class="ms-1 font-weight-bold text-dark">TOSS</span>
        </NuxtLink>
      </div>

      <hr class="horizontal dark mt-0 mb-2">

      <!-- Navigation -->
      <div class="navbar-nav-wrapper w-auto">
        <ul class="navbar-nav">
          <!-- User Profile Section -->
          <li class="mb-2">
            <button class="ct-nav-item w-full group">
              <img src="https://ui-avatars.com/api/?name=Brooklyn+Alice&background=1f2937&color=fff" alt="User" class="w-8 h-8 rounded-full">
              <span v-if="!sidebarMinimized" class="text-sm font-medium">Brooklyn Alice</span>
              <i v-if="!sidebarMinimized" class="ml-auto text-sm material-symbols-rounded ct-nav-icon">expand_more</i>
            </button>
          </li>

          <hr class="my-2 border-[hsl(var(--ct-border))]">

          <!-- Main Navigation -->
          <li class="nav-item">
            <NuxtLink
              to="/"
              class="nav-link text-dark"
              active-class="active bg-gradient-primary"
            >
              <div class="text-center text-dark me-2 d-flex align-items-center justify-content-center">
                <i class="material-symbols-rounded opacity-10">space_dashboard</i>
              </div>
              <span class="nav-link-text ms-1">Dashboard</span>
            </NuxtLink>
          </li>

          <li class="nav-item">
            <NuxtLink
              to="/pos"
              class="nav-link text-dark"
              active-class="active bg-gradient-primary"
            >
              <div class="text-center text-dark me-2 d-flex align-items-center justify-content-center">
                <i class="material-symbols-rounded opacity-10">point_of_sale</i>
              </div>
              <span class="nav-link-text ms-1">POS</span>
            </NuxtLink>
          </li>

          <li class="nav-item">
            <a 
              data-bs-toggle="collapse" 
              href="#stockNav" 
              class="nav-link text-dark"
              :class="route.path.startsWith('/stock') ? 'active bg-gradient-primary' : ''"
              aria-controls="stockNav" 
              role="button" 
              :aria-expanded="stockMenuOpen"
            >
              <div class="text-center text-dark me-2 d-flex align-items-center justify-content-center">
                <i class="material-symbols-rounded opacity-10">inventory_2</i>
              </div>
              <span class="nav-link-text ms-1">Stock</span>
            </a>
            <div class="collapse" :class="{show: stockMenuOpen}" id="stockNav">
              <ul class="nav ms-4 ps-3">
                <li class="nav-item">
                  <NuxtLink
                    to="/stock/items"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/stock/items') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> I </span>
                    <span class="sidenav-normal"> Items </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/stock/movements"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/stock/movements') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> M </span>
                    <span class="sidenav-normal"> Movements </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/stock/reconciliation"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/stock/reconciliation') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> R </span>
                    <span class="sidenav-normal"> Reconciliation </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/stock/alerts"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/stock/alerts') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> A </span>
                    <span class="sidenav-normal"> Alerts </span>
                  </NuxtLink>
                </li>
              </ul>
            </div>
          </li>

          <li class="nav-item">
            <NuxtLink
              to="/customers"
              class="nav-link text-dark"
              active-class="active bg-gradient-primary"
            >
              <div class="text-center text-dark me-2 d-flex align-items-center justify-content-center">
                <i class="material-symbols-rounded opacity-10">group</i>
              </div>
              <span class="nav-link-text ms-1">Customers</span>
            </NuxtLink>
          </li>

          <li class="nav-item">
            <a 
              data-bs-toggle="collapse" 
              href="#salesNav" 
              class="nav-link text-dark"
              :class="route.path.startsWith('/sales') ? 'active bg-gradient-primary' : ''"
              aria-controls="salesNav" 
              role="button" 
              :aria-expanded="salesMenuOpen"
            >
              <div class="text-center text-dark me-2 d-flex align-items-center justify-content-center">
                <i class="material-symbols-rounded opacity-10">receipt_long</i>
              </div>
              <span class="nav-link-text ms-1">Sales</span>
            </a>
            <div class="collapse" :class="{show: salesMenuOpen}" id="salesNav">
              <ul class="nav ms-4 ps-3">
                <li class="nav-item">
                  <NuxtLink
                    to="/sales/quotations"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/sales/quotations') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> Q </span>
                    <span class="sidenav-normal"> Quotations </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/sales/orders"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/sales/orders') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> O </span>
                    <span class="sidenav-normal"> Orders </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/sales/invoices"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/sales/invoices') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> I </span>
                    <span class="sidenav-normal"> Invoices </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/sales/deliveries"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/sales/deliveries') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> D </span>
                    <span class="sidenav-normal"> Deliveries </span>
                  </NuxtLink>
                </li>
              </ul>
            </div>
          </li>

          <li class="nav-item">
            <a 
              data-bs-toggle="collapse" 
              href="#buyingNav" 
              class="nav-link text-dark"
              :class="route.path.startsWith('/buying') ? 'active bg-gradient-primary' : ''"
              aria-controls="buyingNav" 
              role="button" 
              :aria-expanded="buyingMenuOpen"
            >
              <div class="text-center text-dark me-2 d-flex align-items-center justify-content-center">
                <i class="material-symbols-rounded opacity-10">shopping_cart</i>
              </div>
              <span class="nav-link-text ms-1">Buying</span>
            </a>
            <div class="collapse" :class="{show: buyingMenuOpen}" id="buyingNav">
              <ul class="nav ms-4 ps-3">
                <li class="nav-item">
                  <NuxtLink
                    to="/buying/purchase-orders"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/buying/purchase-orders') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> PO </span>
                    <span class="sidenav-normal"> Purchase Orders </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/buying/suppliers"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/buying/suppliers') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> S </span>
                    <span class="sidenav-normal"> Suppliers </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/buying/receipts"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/buying/receipts') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> GR </span>
                    <span class="sidenav-normal"> Goods Receipts </span>
                  </NuxtLink>
                </li>
              </ul>
            </div>
          </li>

          <li class="nav-item">
            <a 
              data-bs-toggle="collapse" 
              href="#accountingNav" 
              class="nav-link text-dark"
              :class="route.path.startsWith('/accounting') ? 'active bg-gradient-primary' : ''"
              aria-controls="accountingNav" 
              role="button" 
              :aria-expanded="accountingMenuOpen"
            >
              <div class="text-center text-dark me-2 d-flex align-items-center justify-content-center">
                <i class="material-symbols-rounded opacity-10">account_balance</i>
              </div>
              <span class="nav-link-text ms-1">Accounting</span>
            </a>
            <div class="collapse" :class="{show: accountingMenuOpen}" id="accountingNav">
              <ul class="nav ms-4 ps-3">
                <li class="nav-item">
                  <NuxtLink
                    to="/accounting/chart-of-accounts"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/accounting/chart-of-accounts') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> COA </span>
                    <span class="sidenav-normal"> Chart of Accounts </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/accounting/journals"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/accounting/journals') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> JE </span>
                    <span class="sidenav-normal"> Journal Entries </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/accounting/reports"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/accounting/reports') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> R </span>
                    <span class="sidenav-normal"> Reports </span>
                  </NuxtLink>
                </li>
              </ul>
            </div>
          </li>

          <li class="nav-item">
            <a 
              data-bs-toggle="collapse" 
              href="#logisticsNav" 
              class="nav-link text-dark"
              :class="route.path.startsWith('/logistics') ? 'active bg-gradient-primary' : ''"
              aria-controls="logisticsNav" 
              role="button" 
              :aria-expanded="logisticsMenuOpen"
            >
              <div class="text-center text-dark me-2 d-flex align-items-center justify-content-center">
                <i class="material-symbols-rounded opacity-10">local_shipping</i>
              </div>
              <span class="nav-link-text ms-1">Logistics</span>
            </a>
            <div class="collapse" :class="{show: logisticsMenuOpen}" id="logisticsNav">
              <ul class="nav ms-4 ps-3">
                <li class="nav-item">
                  <NuxtLink
                    to="/logistics/drivers"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/logistics/drivers') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> D </span>
                    <span class="sidenav-normal"> Drivers </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/logistics/deliveries"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/logistics/deliveries') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> DL </span>
                    <span class="sidenav-normal"> Deliveries </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/logistics/routes"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/logistics/routes') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> R </span>
                    <span class="sidenav-normal"> Routes </span>
                  </NuxtLink>
                </li>
              </ul>
            </div>
          </li>

          <!-- MORE Section -->
          <hr class="horizontal light mt-4 mb-2">
          
          <li class="nav-item">
            <a 
              data-bs-toggle="collapse" 
              href="#projectsNav" 
              class="nav-link text-dark"
              :class="route.path.startsWith('/projects') ? 'active bg-gradient-primary' : ''"
              aria-controls="projectsNav" 
              role="button" 
              :aria-expanded="projectsMenuOpen"
            >
              <div class="text-center text-dark me-2 d-flex align-items-center justify-content-center">
                <i class="material-symbols-rounded opacity-10">widgets</i>
              </div>
              <span class="nav-link-text ms-1">Projects</span>
            </a>
            <div class="collapse" :class="{show: projectsMenuOpen}" id="projectsNav">
              <ul class="nav ms-4 ps-3">
                <li class="nav-item">
                  <NuxtLink
                    to="/projects/list"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/projects/list') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> AP </span>
                    <span class="sidenav-normal"> All Projects </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/projects/tasks"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/projects/tasks') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> T </span>
                    <span class="sidenav-normal"> Tasks </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/projects/time-tracking"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/projects/time-tracking') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> TT </span>
                    <span class="sidenav-normal"> Time Tracking </span>
                  </NuxtLink>
                </li>
              </ul>
            </div>
          </li>

          <li class="nav-item">
            <a 
              data-bs-toggle="collapse" 
              href="#hrNav" 
              class="nav-link text-dark"
              :class="route.path.startsWith('/hr') ? 'active bg-gradient-primary' : ''"
              aria-controls="hrNav" 
              role="button" 
              :aria-expanded="hrMenuOpen"
            >
              <div class="text-center text-dark me-2 d-flex align-items-center justify-content-center">
                <i class="material-symbols-rounded opacity-10">badge</i>
              </div>
              <span class="nav-link-text ms-1">HR & Payroll</span>
            </a>
            <div class="collapse" :class="{show: hrMenuOpen}" id="hrNav">
              <ul class="nav ms-4 ps-3">
                <li class="nav-item">
                  <NuxtLink
                    to="/hr/employees"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/hr/employees') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> E </span>
                    <span class="sidenav-normal"> Employees </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/hr/attendance"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/hr/attendance') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> A </span>
                    <span class="sidenav-normal"> Attendance </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/hr/payroll"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/hr/payroll') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> P </span>
                    <span class="sidenav-normal"> Payroll </span>
                  </NuxtLink>
                </li>
              </ul>
            </div>
          </li>

          <li class="nav-item">
            <NuxtLink
              to="/copilot"
              class="nav-link text-dark"
              active-class="active bg-gradient-primary"
            >
              <div class="text-center text-dark me-2 d-flex align-items-center justify-content-center">
                <i class="material-symbols-rounded opacity-10">psychology</i>
              </div>
              <span class="nav-link-text ms-1">AI Copilot</span>
            </NuxtLink>
          </li>

          <!-- SETTINGS Section -->
          <hr class="horizontal light mt-4 mb-2">
          
          <li class="nav-item">
            <NuxtLink
              to="/settings"
              class="nav-link text-dark"
              active-class="active bg-gradient-primary"
            >
              <div class="text-center text-dark me-2 d-flex align-items-center justify-content-center">
                <i class="material-symbols-rounded opacity-10">settings</i>
              </div>
              <span class="nav-link-text ms-1">Settings</span>
            </NuxtLink>
          </li>
          
          <li class="nav-item">
            <NuxtLink
              to="/help"
              class="nav-link text-dark"
              active-class="active bg-gradient-primary"
            >
              <div class="text-center text-dark me-2 d-flex align-items-center justify-content-center">
                <i class="material-symbols-rounded opacity-10">help</i>
              </div>
              <span class="nav-link-text ms-1">Help & Support</span>
            </NuxtLink>
          </li>
        </ul>
      </div>
    </aside>

    <!-- Main Content Area -->
    <main class="main-content position-relative max-height-vh-100 h-100 border-radius-lg">
      <!-- Top Navbar -->
      <nav class="navbar navbar-main navbar-expand-lg px-0 mx-4 shadow-none border-radius-xl" id="navbarBlur" data-scroll="true">
        <div class="container-fluid py-1 px-3">
          <nav aria-label="breadcrumb">
            <ol class="breadcrumb bg-transparent mb-0 pb-0 pt-1 px-0 me-sm-6 me-5">
              <li
                v-for="(crumb, index) in breadcrumbs"
                :key="index"
                class="breadcrumb-item text-sm"
                :class="{'active': index === breadcrumbs.length - 1}"
              >
                <NuxtLink
                  v-if="index < breadcrumbs.length - 1"
                  :to="crumb.path"
                  class="opacity-5 text-dark"
                >
                  {{ crumb.label }}
                </NuxtLink>
                <span v-else class="text-dark">{{ crumb.label }}</span>
              </li>
            </ol>
            <h6 class="font-weight-bolder mb-0" v-if="breadcrumbs.length > 0">{{ breadcrumbs[breadcrumbs.length - 1].label }}</h6>
          </nav>

          <div class="collapse navbar-collapse mt-sm-0 mt-2 me-md-0 me-sm-4" id="navbar">
            <div class="ms-md-auto pe-md-3 d-flex align-items-center">
              <!-- Search -->
              <div class="input-group input-group-outline">
                <input
                  v-model="searchQuery"
                  type="text"
                  placeholder="Search here"
                  class="form-control"
                  @keyup.enter="handleSearch"
                >
              </div>
            </div>
            <ul class="navbar-nav justify-content-end">

              <!-- User Menu -->
              <li class="nav-item dropdown pe-2 d-flex align-items-center">
                <a 
                  href="javascript:;" 
                  class="nav-link text-body p-0" 
                  id="dropdownMenuButton" 
                  data-bs-toggle="dropdown" 
                  aria-expanded="false"
                >
                  <i class="material-symbols-rounded cursor-pointer">account_circle</i>
                </a>
                <ul class="dropdown-menu dropdown-menu-end px-2 py-3 me-sm-n4" aria-labelledby="dropdownMenuButton">
                  <li>
                    <NuxtLink
                      to="/settings"
                      class="dropdown-item border-radius-md"
                    >
                      <div class="d-flex py-1">
                        <div class="d-flex flex-column justify-content-center">
                          <h6 class="text-sm font-weight-normal mb-1">
                            <span class="font-weight-bold">My Profile</span>
                          </h6>
                        </div>
                      </div>
                    </NuxtLink>
                  </li>
                  <li>
                    <NuxtLink
                      to="/settings"
                      class="dropdown-item border-radius-md"
                    >
                      <div class="d-flex py-1">
                        <div class="d-flex flex-column justify-content-center">
                          <h6 class="text-sm font-weight-normal mb-1">
                            <span class="font-weight-bold">Settings</span>
                          </h6>
                        </div>
                      </div>
                    </NuxtLink>
                  </li>
                  <li><hr class="dropdown-divider"></li>
                  <li>
                    <a
                      href="javascript:;"
                      class="dropdown-item border-radius-md"
                    >
                      <div class="d-flex py-1">
                        <div class="d-flex flex-column justify-content-center">
                          <h6 class="text-sm font-weight-normal mb-1">
                            <span class="font-weight-bold">Logout</span>
                          </h6>
                        </div>
                      </div>
                    </a>
                  </li>
                </ul>
              </li>

              <!-- Settings Icon -->
              <li class="nav-item d-flex align-items-center">
                <NuxtLink
                  to="/settings"
                  class="nav-link text-body font-weight-bold px-0"
                >
                  <i class="material-symbols-rounded">settings</i>
                </NuxtLink>
              </li>

              <!-- Notifications -->
              <li class="nav-item dropdown pe-2 d-flex align-items-center">
                <a 
                  href="javascript:;" 
                  class="nav-link text-body p-0" 
                  id="dropdownNotification" 
                  data-bs-toggle="dropdown" 
                  aria-expanded="false"
                >
                  <i class="material-symbols-rounded cursor-pointer">notifications</i>
                  <span v-if="unreadCount > 0" class="position-absolute top-5 start-100 translate-middle badge rounded-pill bg-danger border border-white small py-1 px-2">
                    {{ unreadCount > 9 ? '9+' : unreadCount }}
                    <span class="visually-hidden">unread notifications</span>
                  </span>
                </a>
                <ul class="dropdown-menu dropdown-menu-end px-2 py-3 me-sm-n4" aria-labelledby="dropdownNotification" style="max-height: 400px; overflow-y: auto; min-width: 300px;">
                  <li class="mb-2">
                    <div class="d-flex justify-content-between align-items-center px-3">
                      <h6 class="text-sm font-weight-bold mb-0">Notifications</h6>
                      <button
                        v-if="unreadCount > 0"
                        @click="clearAllNotifications"
                        class="btn btn-link text-xs text-primary mb-0 p-0"
                      >
                        Mark all read
                      </button>
                    </div>
                  </li>
                  <li v-if="notifications.length === 0">
                    <div class="text-center py-4">
                      <i class="material-symbols-rounded opacity-6" style="font-size: 3rem;">notifications_off</i>
                      <p class="text-sm text-secondary mb-0">No notifications</p>
                    </div>
                  </li>
                  <li v-else v-for="notification in notifications" :key="notification.id">
                    <a
                      href="javascript:;"
                      @click="markNotificationAsRead(notification.id)"
                      class="dropdown-item border-radius-md"
                      :class="{ 'bg-light': !notification.read }"
                    >
                      <div class="d-flex py-1">
                        <div class="my-auto">
                          <div 
                            class="icon icon-sm icon-shape shadow text-center border-radius-md"
                            :class="{
                              'bg-gradient-danger': notification.color === 'red',
                              'bg-gradient-success': notification.color === 'green',
                              'bg-gradient-primary': notification.color === 'blue',
                              'bg-gradient-warning': notification.color === 'orange'
                            }"
                          >
                            <i class="material-symbols-rounded opacity-10 text-white">{{ notification.icon }}</i>
                          </div>
                        </div>
                        <div class="d-flex flex-column justify-content-center ms-3">
                          <h6 class="text-sm font-weight-bold mb-1">
                            {{ notification.title }}
                          </h6>
                          <p class="text-xs text-secondary mb-0">
                            {{ notification.message }}
                          </p>
                          <p class="text-xs text-secondary mb-0">
                            {{ notification.time }}
                          </p>
                        </div>
                      </div>
                    </a>
                  </li>
                </ul>
              </li>

              <!-- Mobile Sidebar Toggle -->
              <li class="nav-item d-xl-none ps-3 d-flex align-items-center">
                <a 
                  href="javascript:;" 
                  class="nav-link text-body p-0" 
                  id="iconNavbarSidenav"
                >
                  <div class="sidenav-toggler-inner">
                    <i class="sidenav-toggler-line"></i>
                    <i class="sidenav-toggler-line"></i>
                    <i class="sidenav-toggler-line"></i>
                  </div>
                </a>
              </li>
            </ul>
          </div>
        </div>
      </nav>

      <!-- Page Content -->
      <div class="container-fluid py-4">
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
