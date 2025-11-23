<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  LayoutDashboard,
  ShoppingCart,
  Package,
  Users,
  Truck,
  Settings,
  BarChart3,
  FileText,
  Store,
  ChevronLeft,
  ChevronRight,
  Menu,
  UserCircle,
  LogOut
} from 'lucide-vue-next'

interface Props {
  open?: boolean
  role?: 'admin' | 'retailer' | 'supplier' | 'driver'
  userInfo?: {
    name: string
    email: string
    avatar?: string
  }
}

const props = withDefaults(defineProps<Props>(), {
  open: true,
  role: 'retailer'
})

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

// Navigation items based on role
const navigation = computed(() => {
  const base = {
    retailer: [
      { name: 'Dashboard', path: '/retailer/dashboard', icon: LayoutDashboard },
      { name: 'POS', path: '/sales/pos', icon: ShoppingCart },
      { name: 'Products', path: '/retailer/products', icon: Package },
      { name: 'Inventory', path: '/retailer/inventory', icon: Store },
      { name: 'Orders', path: '/retailer/orders', icon: FileText },
      { name: 'Customers', path: '/crm/customers', icon: Users },
      { name: 'Reports', path: '/sales/reports', icon: BarChart3 }
    ],
    supplier: [
      { name: 'Dashboard', path: '/supplier/dashboard', icon: LayoutDashboard },
      { name: 'Orders', path: '/supplier/orders', icon: FileText },
      { name: 'Products', path: '/supplier/products', icon: Package },
      { name: 'Analytics', path: '/sales/reports/analytics', icon: BarChart3 }
    ],
    driver: [
      { name: 'Dashboard', path: '/driver/deliveries', icon: LayoutDashboard },
      { name: 'Deliveries', path: '/driver/deliveries', icon: Truck },
      { name: 'History', path: '/driver/deliveries', icon: FileText }
    ],
    admin: [
      { name: 'Dashboard', path: '/admin/dashboard', icon: LayoutDashboard },
      { name: 'Users', path: '/admin/users', icon: Users },
      { name: 'Orders', path: '/admin/orders', icon: FileText },
      { name: 'Analytics', path: '/sales/reports/analytics', icon: BarChart3 }
    ]
  }

  return base[props.role] || []
})

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
  <aside
    :class="[
      'fixed lg:relative h-full bg-sidebar border-r border-sidebar-border transition-all duration-300 z-50 flex flex-col',
      collapsed && !isMobile ? 'w-16' : 'w-64',
      isMobile && !sidebarOpen ? '-translate-x-full' : 'translate-x-0'
    ]"
  >
    <!-- Sidebar Header -->
    <div class="flex items-center justify-between h-16 px-4 border-b border-sidebar-border">
      <div v-if="!collapsed || isMobile" class="flex items-center gap-3">
        <div class="flex items-center justify-center w-8 h-8 bg-primary text-primary-foreground rounded-lg">
          <span class="text-sm font-bold">T</span>
        </div>
        <div class="flex flex-col">
          <span class="text-sm font-bold text-sidebar-foreground">TOSS</span>
          <span class="text-[10px] text-muted-foreground">ERP III</span>
        </div>
      </div>
      <button
        @click="toggleSidebar"
        class="p-2 rounded-lg hover:bg-sidebar-accent transition-colors"
        :class="{ 'mx-auto': collapsed && !isMobile }"
      >
        <ChevronLeft v-if="!collapsed && !isMobile" :size="20" />
        <ChevronRight v-else-if="collapsed && !isMobile" :size="20" />
        <Menu v-else :size="20" />
      </button>
    </div>

    <!-- User Profile Section -->
    <div
      v-if="userInfo && (!collapsed || isMobile)"
      class="px-4 py-4 border-b border-sidebar-border"
    >
      <div class="relative">
        <button
          @click="showProfileMenu = !showProfileMenu"
          class="flex items-center gap-3 w-full p-2 rounded-lg hover:bg-sidebar-accent transition-colors"
        >
          <div class="w-10 h-10 rounded-full bg-primary text-primary-foreground flex items-center justify-center">
            <UserCircle :size="20" />
          </div>
          <div class="flex-1 text-left">
            <p class="text-sm font-medium text-sidebar-foreground">{{ userInfo.name }}</p>
            <p class="text-xs text-muted-foreground truncate">{{ userInfo.email }}</p>
          </div>
        </button>
        <div
          v-if="showProfileMenu"
          class="absolute left-0 right-0 mt-2 bg-card border border-border rounded-lg shadow-lg z-50"
        >
          <NuxtLink
            to="/settings"
            class="block px-4 py-2 text-sm hover:bg-accent rounded-t-lg"
            @click="showProfileMenu = false"
          >
            Settings
          </NuxtLink>
          <button
            @click="handleLogout"
            class="w-full text-left px-4 py-2 text-sm text-destructive hover:bg-accent rounded-b-lg"
          >
            Logout
          </button>
        </div>
      </div>
    </div>

    <!-- Navigation -->
    <nav class="flex-1 overflow-y-auto px-3 py-4 space-y-1">
      <NuxtLink
        v-for="item in navigation"
        :key="item.path"
        :to="item.path"
        :class="[
          'flex items-center gap-3 px-3 py-2 rounded-lg text-sm font-medium transition-colors',
          isActive(item.path)
            ? 'bg-primary text-primary-foreground'
            : 'text-sidebar-foreground hover:bg-sidebar-accent'
        ]"
        @click="isMobile && (sidebarOpen = false)"
      >
        <component :is="item.icon" :size="20" />
        <span v-if="!collapsed || isMobile">{{ item.name }}</span>
      </NuxtLink>
    </nav>

    <!-- Footer -->
    <div class="p-4 border-t border-sidebar-border">
      <NuxtLink
        to="/settings"
        :class="[
          'flex items-center gap-3 px-3 py-2 rounded-lg text-sm font-medium transition-colors',
          route.path === '/settings'
            ? 'bg-primary text-primary-foreground'
            : 'text-sidebar-foreground hover:bg-sidebar-accent'
        ]"
      >
        <Settings :size="20" />
        <span v-if="!collapsed || isMobile">Settings</span>
      </NuxtLink>
    </div>
  </aside>

  <!-- Mobile Overlay -->
  <div
    v-if="isMobile && sidebarOpen"
    @click="sidebarOpen = false"
    class="fixed inset-0 bg-black/50 z-40 lg:hidden"
  />
</template>

<style scoped>
/* Smooth transitions */
aside {
  box-shadow: 1px 0 3px rgba(0, 0, 0, 0.05);
}
</style>
