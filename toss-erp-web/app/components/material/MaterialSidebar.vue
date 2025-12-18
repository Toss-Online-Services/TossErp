<script setup lang="ts">
import type { Component } from 'vue'
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  ArrowLeftRight,
  Banknote,
  BarChart3,
  Bell,
  Boxes,
  CheckSquare,
  ChevronLeft,
  ChevronRight,
  CircleUserRound,
  ContactRound,
  CreditCard,
  FileSignature,
  FileSpreadsheet,
  FolderKanban,
  LayoutDashboard,
  Menu,
  Package,
  ReceiptText,
  Settings,
  ShoppingBag,
  Truck,
  UserCircle,
  Users,
  Wallet
} from 'lucide-vue-next'

interface Props {
  open: boolean
  role?: 'admin' | 'retailer' | 'supplier' | 'driver'
  userInfo: {
    name: string
    email: string
    avatar?: string
  }
}

const props = withDefaults(defineProps<Props>(), {
  role: 'retailer'
})

interface MenuLink {
  name: string
  path: string
  icon: Component
}

interface MenuGroup {
  title: string
  items: MenuLink[]
}

const emit = defineEmits<{
  'update:open': [value: boolean]
}>()

const route = useRoute()
const router = useRouter()
const isMobile = ref(false)
const collapsed = ref(false)
const showProfileMenu = ref(false)

const sidebarOpen = computed({
  get: () => props.open,
  set: (value) => emit('update:open', value)
})

const checkMobile = () => {
  if (typeof window === 'undefined') return
  isMobile.value = window.innerWidth < 1024
  if (isMobile.value) {
    sidebarOpen.value = false
  }
}

const toggleSidebar = () => {
  if (isMobile.value) {
    sidebarOpen.value = !sidebarOpen.value
  } else {
    collapsed.value = !collapsed.value
  }
}

const moduleGroups: MenuGroup[] = [
  {
    title: 'Dashboard',
    items: [
      { name: 'Analytics', path: '/dashboard/analytics', icon: LayoutDashboard }
    ]
  },
  {
    title: 'Selling',
    items: [
      { name: 'Quotations', path: '/selling/quotations', icon: FileSignature },
      { name: 'Sales Orders', path: '/selling/orders', icon: ShoppingBag },
      { name: 'Invoices', path: '/selling/invoices', icon: ReceiptText },
      { name: 'Deliveries', path: '/selling/deliveries', icon: Truck }
    ]
  },
  {
    title: 'Buying',
    items: [
      { name: 'Purchase Orders', path: '/buying/purchase-orders', icon: FileSpreadsheet },
      { name: 'Receipts', path: '/buying/receipts', icon: Package },
      { name: 'Suppliers', path: '/buying/suppliers', icon: Users }
    ]
  },
  {
    title: 'Stock',
    items: [
      { name: 'Items', path: '/stock/items', icon: Boxes },
      { name: 'Alerts', path: '/stock/alerts', icon: Bell },
      { name: 'Movements', path: '/stock/movements', icon: ArrowLeftRight }
    ]
  },
  {
    title: 'Accounts',
    items: [
      { name: 'Summary', path: '/accounts/summary', icon: Wallet },
      { name: 'Cashbook', path: '/accounts/cashbook', icon: Banknote },
      { name: 'Reports', path: '/accounts/reports', icon: BarChart3 }
    ]
  },
  {
    title: 'CRM',
    items: [
      { name: 'Customers', path: '/crm/customers', icon: ContactRound },
      { name: 'Leads', path: '/crm/leads', icon: CircleUserRound }
    ]
  },
  {
    title: 'Projects',
    items: [
      { name: 'Projects', path: '/projects', icon: FolderKanban },
      { name: 'Tasks', path: '/projects/tasks', icon: CheckSquare }
    ]
  },
  {
    title: 'POS',
    items: [
      { name: 'Point of Sale', path: '/pos', icon: CreditCard }
    ]
  },
  {
    title: 'Admin',
    items: [
      { name: 'Settings', path: '/admin/settings', icon: Settings },
      { name: 'Users', path: '/admin/users', icon: UserCircle }
    ]
  }
]

// Navigation items based on role
const navigation = computed(() => moduleGroups)

const activeLinkClasses = 'bg-gradient-to-r from-[#e91e63] to-[#d81b60] text-white shadow-material-primary'
const inactiveLinkClasses = 'text-stone-700 hover:bg-gray-100 transition-colors duration-200 border border-transparent'

const isActive = (path: string) => {
  return route.path.startsWith(path)
}

const handleLogout = () => {
  // Handle logout
  router.push('/auth/login')
}

onMounted(() => {
  checkMobile()
  window.addEventListener('resize', checkMobile)
})

onUnmounted(() => {
  window.removeEventListener('resize', checkMobile)
})
</script>

<template>
  <div class="relative">
    <aside
      id="sidenav-main"
      :class="[
        'sidenav navbar navbar-vertical navbar-expand-xs fixed-start top-0 h-screen bg-white border border-gray-200 transition-all duration-300 z-50 flex flex-col border-radius-lg ms-2 my-2 shadow-material',
        collapsed && !isMobile ? 'w-16' : 'w-[260px]',
        isMobile && !sidebarOpen ? '-translate-x-full' : 'translate-x-0',
        !isMobile ? 'lg:h-[calc(100vh-1rem)]' : '',
        isMobile ? 'w-80' : ''
      ]"
    >
    <!-- Sidebar Header -->
      <div class="flex items-center justify-between h-16 px-4">
        <NuxtLink
          v-if="!collapsed || isMobile"
          to="/"
          class="flex items-center gap-3"
        >
          <div class="flex items-center justify-center w-10 h-10 text-white rounded-lg shadow-md bg-stone-900">
            <span class="text-sm font-bold">T</span>
          </div>
          <div class="flex flex-col leading-tight">
            <span class="text-sm font-semibold text-stone-900">TOSS ERP</span>
            <span class="text-[11px] text-stone-500">Material dashboard</span>
          </div>
        </NuxtLink>
        <button
          @click="toggleSidebar"
          class="p-2 transition-colors rounded-lg hover:bg-gray-100 text-stone-700"
          :class="{ 'mx-auto': collapsed && !isMobile }"
          aria-label="Toggle sidebar"
        >
          <ChevronLeft v-if="!collapsed && !isMobile" :size="20" />
          <ChevronRight v-else-if="collapsed && !isMobile" :size="20" />
          <Menu v-else :size="20" />
        </button>
      </div>

      <div class="h-px mx-4 bg-gray-200"></div>

      <!-- User Profile Section -->
      <div
        v-if="userInfo && (!collapsed || isMobile)"
        class="px-4 py-4"
      >
        <div class="relative">
          <button
            @click="showProfileMenu = !showProfileMenu"
            class="flex items-center w-full gap-3 p-2 transition-colors rounded-xl hover:bg-gray-100"
          >
            <div class="flex items-center justify-center w-10 h-10 text-white rounded-full shadow-md bg-stone-800">
              <UserCircle :size="20" />
            </div>
            <div class="flex-1 text-left">
              <p class="text-sm font-medium text-stone-900">{{ userInfo.name }}</p>
              <p class="text-xs truncate text-stone-500">{{ userInfo.email }}</p>
            </div>
          </button>
          <div
            v-if="showProfileMenu"
            class="absolute left-0 right-0 z-50 mt-2 bg-white border shadow-lg border-stone-200 rounded-xl"
          >
            <NuxtLink
              to="/admin/settings"
              class="block px-4 py-2 text-sm text-stone-700 hover:bg-gray-50 rounded-t-xl"
              @click="showProfileMenu = false"
            >
              Settings
            </NuxtLink>
            <button
              @click="handleLogout"
              class="w-full px-4 py-2 text-sm text-left text-red-600 hover:bg-gray-50 rounded-b-xl"
            >
              Logout
            </button>
          </div>
        </div>
      </div>

      <!-- Navigation -->
      <nav class="flex-1 px-3 pt-3 pb-4 space-y-4 overflow-y-auto">
        <div
          v-for="group in navigation"
          :key="group.title"
          class="space-y-2"
        >
          <p
            v-if="!collapsed || isMobile"
            class="px-2 text-[11px] font-semibold uppercase tracking-wide text-stone-500"
          >
            {{ group.title }}
          </p>
          <div class="space-y-1">
            <NuxtLink
              v-for="item in group.items"
              :key="item.path"
              :to="item.path"
              :title="(collapsed && !isMobile) ? `${group.title} • ${item.name}` : ''"
              :class="[
                'flex items-center text-sm font-medium rounded-xl cursor-pointer transition-all duration-200 border px-3 py-2',
                isActive(item.path) ? activeLinkClasses : inactiveLinkClasses,
                collapsed && !isMobile ? 'justify-center px-2' : 'gap-3'
              ]"
              :aria-current="isActive(item.path) ? 'page' : undefined"
              @click="isMobile && (sidebarOpen = false)"
            >
              <component :is="item.icon" :size="16" class="flex-shrink-0" />
              <span v-if="!collapsed || isMobile" class="truncate">{{ item.name }}</span>
            </NuxtLink>
          </div>
        </div>
      </nav>

      <!-- Footer -->
      <div class="px-3 pb-4 mt-auto">
        <NuxtLink
          to="/admin/settings"
          :class="[
            'flex items-center text-sm font-medium rounded-xl cursor-pointer transition-all duration-200 border px-3 py-2',
            route.path === '/admin/settings' ? activeLinkClasses : inactiveLinkClasses,
            collapsed && !isMobile ? 'justify-center px-2' : 'gap-3'
          ]"
        >
          <Settings :size="16" />
          <span v-if="!collapsed || isMobile">Settings</span>
        </NuxtLink>
      </div>
    </aside>

    <!-- Mobile Overlay -->
    <Transition
      enter-active-class="transition-opacity duration-300"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition-opacity duration-300"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div
        v-if="isMobile && sidebarOpen"
        @click="sidebarOpen = false"
        @touchstart="sidebarOpen = false"
        class="fixed inset-0 z-40 bg-black/50 backdrop-blur-sm lg:hidden"
      />
    </Transition>
  </div>
</template>

<style scoped>
/* Smooth transitions */
aside {
  box-shadow: 1px 0 3px rgba(0, 0, 0, 0.05);
}

/* Mobile menu improvements */
@media (max-width: 1023px) {
  aside {
    box-shadow: 2px 0 8px rgba(0, 0, 0, 0.15);
  }
}

/* Active nav link styling with inset shadows */
.nav-active::after {
  content: '';
  position: absolute;
  inset: 0;
  border-radius: inherit;
  box-shadow: inset 0 1px 0px rgba(255, 255, 255, 0.25), inset 0 -2px 0px rgba(0, 0, 0, 0.35);
  pointer-events: none;
}

/* Touch-friendly tap targets on mobile */
@media (max-width: 1023px) {
  nav a {
    min-height: 44px;
    touch-action: manipulation;
  }
}
</style>
