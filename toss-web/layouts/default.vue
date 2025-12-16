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
  
  // Ensure navigation is visible after Material Dashboard initializes
  // Material Dashboard JS should handle this, but we ensure it in Vue context
  if (typeof window !== 'undefined') {
    const ensureNavVisible = () => {
      const navCollapse = document.getElementById('sidenav-collapse-main')
      if (navCollapse) {
        // Material Dashboard should initialize this, but ensure it's visible
        // Use Bootstrap Collapse API if available
        if ((window as any).bootstrap?.Collapse) {
          try {
            // Get existing instance or create new one
            let collapseInstance = (window as any).bootstrap.Collapse.getInstance(navCollapse)
            if (!collapseInstance) {
              collapseInstance = new (window as any).bootstrap.Collapse(navCollapse, {
                toggle: false
              })
            }
            // Show it
            if (!navCollapse.classList.contains('show')) {
              collapseInstance.show()
            }
          } catch (e) {
            // Fallback: manual show
            navCollapse.classList.add('show')
          }
        } else {
          // Bootstrap not ready yet, add show class manually
          navCollapse.classList.add('show')
        }
      }
    }
    
    // Try multiple times to catch different load scenarios
    ensureNavVisible() // Immediate
    setTimeout(ensureNavVisible, 100)
    setTimeout(ensureNavVisible, 500)
    setTimeout(ensureNavVisible, 1000)
    setTimeout(ensureNavVisible, 2000) // After Material Dashboard JS should be loaded
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
        'sidenav navbar navbar-vertical navbar-expand-xs border-radius-lg fixed-start ms-2  bg-white my-2',
        sidebarMinimized ? 'sidenav-mini' : ''
      ]"
      id="sidenav-main"
    >
      <!-- Sidebar Header -->
      <div class="sidenav-header">
        <i class="fas fa-times p-3 cursor-pointer text-dark opacity-5 position-absolute end-0 top-0 d-none d-xl-none" aria-hidden="true" id="iconSidenav"></i>
        <NuxtLink to="/" class="navbar-brand px-4 py-3 m-0">
          <img src="/assets/img/logo-ct-dark.png" class="navbar-brand-img" width="26" height="26" alt="main_logo">
          <span class="ms-1 text-sm text-dark">TOSS</span>
        </NuxtLink>
      </div>

      <hr class="horizontal dark mt-0 mb-2">

      <!-- Navigation -->
      <div class="collapse navbar-collapse  w-auto h-auto" id="sidenav-collapse-main">
        <ul class="navbar-nav">
          <!-- User Profile Section -->
          <li class="nav-item mb-2 mt-0">
            <a data-bs-toggle="collapse" href="#ProfileNav" class="nav-link text-dark" aria-controls="ProfileNav" role="button" aria-expanded="false">
              <img src="https://ui-avatars.com/api/?name=Brooklyn+Alice&background=1f2937&color=fff" class="avatar" alt="User">
              <span class="nav-link-text ms-2 ps-1">Brooklyn Alice</span>
            </a>
            <div class="collapse" id="ProfileNav" style="">
              <ul class="nav ">
                <li class="nav-item">
                  <NuxtLink to="/settings" class="nav-link text-dark">
                    <span class="sidenav-mini-icon"> MP </span>
                    <span class="sidenav-normal  ms-3  ps-1"> My Profile </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink to="/settings" class="nav-link text-dark ">
                    <span class="sidenav-mini-icon"> S </span>
                    <span class="sidenav-normal  ms-3  ps-1"> Settings </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <a href="javascript:;" class="nav-link text-dark ">
                    <span class="sidenav-mini-icon"> L </span>
                    <span class="sidenav-normal  ms-3  ps-1"> Logout </span>
                  </a>
                </li>
              </ul>
            </div>
          </li>

          <hr class="horizontal dark mt-0">

          <!-- Main Navigation -->
          <li class="nav-item">
            <NuxtLink
              to="/"
              class="nav-link text-dark"
              active-class="active bg-gradient-primary"
            >
              <i class="material-symbols-rounded opacity-5">space_dashboard</i>
              <span class="nav-link-text ms-1 ps-1">Dashboard</span>
            </NuxtLink>
          </li>

          <li class="nav-item">
            <NuxtLink
              to="/pos"
              class="nav-link text-dark"
              active-class="active bg-gradient-primary"
            >
              <i class="material-symbols-rounded opacity-5">point_of_sale</i>
              <span class="nav-link-text ms-1 ps-1">POS</span>
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
              <i class="material-symbols-rounded opacity-5">inventory_2</i>
              <span class="nav-link-text ms-1 ps-1">Stock</span>
            </a>
            <div class="collapse" :class="{show: stockMenuOpen}" id="stockNav">
              <ul class="nav">
                <li class="nav-item">
                  <NuxtLink
                    to="/stock/items"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/stock/items') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> I </span>
                    <span class="sidenav-normal ms-1 ps-1"> Items </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/stock/movements"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/stock/movements') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> M </span>
                    <span class="sidenav-normal ms-1 ps-1"> Movements </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/stock/reconciliation"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/stock/reconciliation') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> R </span>
                    <span class="sidenav-normal ms-1 ps-1"> Reconciliation </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/stock/alerts"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/stock/alerts') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> A </span>
                    <span class="sidenav-normal ms-1 ps-1"> Alerts </span>
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
              <i class="material-symbols-rounded opacity-5">group</i>
              <span class="nav-link-text ms-1 ps-1">Customers</span>
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
              <i class="material-symbols-rounded opacity-5">receipt_long</i>
              <span class="nav-link-text ms-1 ps-1">Sales</span>
            </a>
            <div class="collapse" :class="{show: salesMenuOpen}" id="salesNav">
              <ul class="nav">
                <li class="nav-item">
                  <NuxtLink
                    to="/sales/quotations"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/sales/quotations') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> Q </span>
                    <span class="sidenav-normal ms-1 ps-1"> Quotations </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/sales/orders"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/sales/orders') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> O </span>
                    <span class="sidenav-normal ms-1 ps-1"> Orders </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/sales/invoices"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/sales/invoices') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> I </span>
                    <span class="sidenav-normal ms-1 ps-1"> Invoices </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/sales/deliveries"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/sales/deliveries') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> D </span>
                    <span class="sidenav-normal ms-1 ps-1"> Deliveries </span>
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
              <i class="material-symbols-rounded opacity-5">shopping_cart</i>
              <span class="nav-link-text ms-1 ps-1">Buying</span>
            </a>
            <div class="collapse" :class="{show: buyingMenuOpen}" id="buyingNav">
              <ul class="nav">
                <li class="nav-item">
                  <NuxtLink
                    to="/buying/purchase-orders"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/buying/purchase-orders') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> PO </span>
                    <span class="sidenav-normal ms-1 ps-1"> Purchase Orders </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/buying/suppliers"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/buying/suppliers') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> S </span>
                    <span class="sidenav-normal ms-1 ps-1"> Suppliers </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/buying/receipts"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/buying/receipts') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> GR </span>
                    <span class="sidenav-normal ms-1 ps-1"> Goods Receipts </span>
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
              <i class="material-symbols-rounded opacity-5">account_balance</i>
              <span class="nav-link-text ms-1 ps-1">Accounting</span>
            </a>
            <div class="collapse" :class="{show: accountingMenuOpen}" id="accountingNav">
              <ul class="nav">
                <li class="nav-item">
                  <NuxtLink
                    to="/accounting/chart-of-accounts"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/accounting/chart-of-accounts') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> COA </span>
                    <span class="sidenav-normal ms-1 ps-1"> Chart of Accounts </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/accounting/journals"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/accounting/journals') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> JE </span>
                    <span class="sidenav-normal ms-1 ps-1"> Journal Entries </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/accounting/reports"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/accounting/reports') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> R </span>
                    <span class="sidenav-normal ms-1 ps-1"> Reports </span>
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
              <i class="material-symbols-rounded opacity-5">local_shipping</i>
              <span class="nav-link-text ms-1 ps-1">Logistics</span>
            </a>
            <div class="collapse" :class="{show: logisticsMenuOpen}" id="logisticsNav">
              <ul class="nav">
                <li class="nav-item">
                  <NuxtLink
                    to="/logistics/drivers"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/logistics/drivers') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> D </span>
                    <span class="sidenav-normal ms-1 ps-1"> Drivers </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/logistics/deliveries"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/logistics/deliveries') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> DL </span>
                    <span class="sidenav-normal ms-1 ps-1"> Deliveries </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/logistics/routes"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/logistics/routes') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> R </span>
                    <span class="sidenav-normal ms-1 ps-1"> Routes </span>
                  </NuxtLink>
                </li>
              </ul>
            </div>
          </li>

          <!-- MORE Section -->
          <li class="nav-item mt-3">
            <h6 class="ps-3 ms-2 text-uppercase text-xs font-weight-bolder text-dark">MORE</h6>
          </li>
          
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
              <i class="material-symbols-rounded opacity-5">widgets</i>
              <span class="nav-link-text ms-1 ps-1">Projects</span>
            </a>
            <div class="collapse" :class="{show: projectsMenuOpen}" id="projectsNav">
              <ul class="nav">
                <li class="nav-item">
                  <NuxtLink
                    to="/projects/list"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/projects/list') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> AP </span>
                    <span class="sidenav-normal ms-1 ps-1"> All Projects </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/projects/tasks"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/projects/tasks') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> T </span>
                    <span class="sidenav-normal ms-1 ps-1"> Tasks </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/projects/time-tracking"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/projects/time-tracking') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> TT </span>
                    <span class="sidenav-normal ms-1 ps-1"> Time Tracking </span>
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
              <i class="material-symbols-rounded opacity-5">badge</i>
              <span class="nav-link-text ms-1 ps-1">HR & Payroll</span>
            </a>
            <div class="collapse" :class="{show: hrMenuOpen}" id="hrNav">
              <ul class="nav">
                <li class="nav-item">
                  <NuxtLink
                    to="/hr/employees"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/hr/employees') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> E </span>
                    <span class="sidenav-normal ms-1 ps-1"> Employees </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/hr/attendance"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/hr/attendance') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> A </span>
                    <span class="sidenav-normal ms-1 ps-1"> Attendance </span>
                  </NuxtLink>
                </li>
                <li class="nav-item">
                  <NuxtLink
                    to="/hr/payroll"
                    class="nav-link text-dark"
                    :class="route.path.startsWith('/hr/payroll') ? 'active' : ''"
                  >
                    <span class="sidenav-mini-icon"> P </span>
                    <span class="sidenav-normal ms-1 ps-1"> Payroll </span>
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
              <i class="material-symbols-rounded opacity-5">psychology</i>
              <span class="nav-link-text ms-1 ps-1">AI Copilot</span>
            </NuxtLink>
          </li>

          <!-- SETTINGS Section -->
          <li class="nav-item mt-3">
            <h6 class="ps-3 ms-2 text-uppercase text-xs font-weight-bolder text-dark">SETTINGS</h6>
          </li>
          
          <li class="nav-item">
            <NuxtLink
              to="/settings"
              class="nav-link text-dark"
              active-class="active bg-gradient-primary"
            >
              <i class="material-symbols-rounded opacity-5">settings</i>
              <span class="nav-link-text ms-1 ps-1">Settings</span>
            </NuxtLink>
          </li>
          
          <li class="nav-item">
            <NuxtLink
              to="/help"
              class="nav-link text-dark"
              active-class="active bg-gradient-primary"
            >
              <i class="material-symbols-rounded opacity-5">help</i>
              <span class="nav-link-text ms-1 ps-1">Help & Support</span>
            </NuxtLink>
          </li>
        </ul>
      </div>
    </aside>

    <!-- Main Content Area -->
    <main class="main-content border-radius-lg ">
      <!-- Top Navbar -->
      <nav class="navbar navbar-main navbar-expand-lg position-sticky mt-2 top-1 px-0 py-1 mx-3 shadow-none border-radius-lg z-index-sticky" id="navbarBlur" data-scroll="true">
        <div class="container-fluid py-1 px-2">
          <div class="sidenav-toggler sidenav-toggler-inner d-xl-block d-none ">
            <a href="javascript:;" class="nav-link text-body p-0">
              <div class="sidenav-toggler-inner">
                <i class="sidenav-toggler-line"></i>
                <i class="sidenav-toggler-line"></i>
                <i class="sidenav-toggler-line"></i>
              </div>
            </a>
          </div>
          <nav aria-label="breadcrumb" class="ps-2">
            <ol class="breadcrumb bg-transparent mb-0 p-0">
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
          </nav>

          <div class="collapse navbar-collapse mt-sm-0 mt-2 me-md-0 me-sm-4" id="navbar">
            <div class="ms-md-auto pe-md-3 d-flex align-items-center">
              <!-- Search -->
              <div class="input-group input-group-outline">
                <label class="form-label">Search here</label>
                <input
                  v-model="searchQuery"
                  type="text"
                  class="form-control"
                  @keyup.enter="handleSearch"
                >
              </div>
            </div>
            <ul class="navbar-nav  justify-content-end">

              <!-- User Menu -->
              <li class="nav-item">
                <a href="javascript:;" class="px-1 py-0 nav-link line-height-0" target="_blank">
                  <i class="material-symbols-rounded">
                    account_circle
                  </i>
                </a>
              </li>
              
              <!-- Settings Icon -->
              <li class="nav-item">
                <a href="javascript:;" class="nav-link py-0 px-1 line-height-0">
                  <i class="material-symbols-rounded fixed-plugin-button-nav">
                    settings
                  </i>
                </a>
              </li>

              <!-- Notifications -->
              <li class="nav-item dropdown py-0 pe-3">
                <a 
                  href="javascript:;" 
                  class="nav-link py-0 px-1 position-relative line-height-0" 
                  id="dropdownMenuButton" 
                  data-bs-toggle="dropdown" 
                  aria-expanded="false"
                >
                  <i class="material-symbols-rounded">
                    notifications
                  </i>
                  <span v-if="unreadCount > 0" class="position-absolute top-5 start-100 translate-middle badge rounded-pill bg-danger border border-white small py-1 px-2">
                    <span class="small">{{ unreadCount > 9 ? '9+' : unreadCount }}</span>
                    <span class="visually-hidden">unread notifications</span>
                  </span>
                </a>
                <ul class="dropdown-menu dropdown-menu-end p-2 me-sm-n4" aria-labelledby="dropdownMenuButton">
                  <li v-for="notification in notifications.slice(0, 3)" :key="notification.id" class="mb-2">
                    <a
                      href="javascript:;"
                      class="dropdown-item border-radius-md"
                    >
                      <div class="d-flex align-items-center py-1">
                        <span class="material-symbols-rounded">{{ notification.icon }}</span>
                        <div class="ms-2">
                          <h6 class="text-sm font-weight-normal my-auto">
                            {{ notification.message }}
                          </h6>
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
