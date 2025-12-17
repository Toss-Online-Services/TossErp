<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import type { Component } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  LayoutDashboard,
  Megaphone,
  ShoppingCart,
  Layers,
  ShoppingBag,
  UserCog,
  BarChart3,
  FileText,
  Handshake,
  ShieldCheck,
  Users,
  ArrowLeftRight,
  Boxes,
  Package,
  Truck,
  Settings,
  ChevronLeft,
  ChevronRight,
  Menu,
  UserCircle,
  ShoppingBasket,
  Wallet,
  MessageSquare,
  Factory,
  Wrench,
  CheckSquare,
  Phone,
  FileSpreadsheet,
  Network,
  Globe
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

interface MenuItem {
  name: string
  path: string
  icon: Component
}

interface MenuGroup {
  title: string
  items: MenuItem[]
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

const moduleGroups: MenuGroup[] = [
  {
    title: 'Executive Overview',
    items: [{ name: 'Dashboard', path: '/dashboard', icon: LayoutDashboard }]
  },
  {
    title: 'Selling',
    items: [
      { name: 'Sales & Marketing', path: '/selling', icon: Megaphone },
      { name: 'POS & Store Solutions', path: '/selling/pos', icon: ShoppingCart },
      { name: 'Cross Commerce', path: '/selling/cross-commerce', icon: ArrowLeftRight }
    ]
  },
  {
    title: 'Buying',
    items: [
      { name: 'Planning & Assortment', path: '/planning/assortment', icon: Layers },
      { name: 'Vendor Relationship', path: '/relationships/vendor', icon: Handshake }
    ]
  },
  {
    title: 'Stock',
    items: [
      { name: 'Inventory Management', path: '/operations/inventory', icon: Boxes },
      { name: 'Warehouse Management', path: '/operations/warehouse', icon: Package },
      { name: 'Supply & Chain Integration', path: '/operations/supply-chain', icon: Truck }
    ]
  },
  {
    title: 'Accounts',
    items: [
      { name: 'Account Management', path: '/relationships/account', icon: UserCog },
      { name: 'Invoice Management', path: '/relationships/invoice', icon: FileText }
    ]
  },
  {
    title: 'CRM',
    items: [
      { name: 'Customer Relationship', path: '/relationships/customer', icon: Users }
    ]
  },
  {
    title: 'Projects',
    items: [
      { name: 'Merchandising Management', path: '/planning/merchandising', icon: ShoppingBag }
    ]
  },
  {
    title: 'Support & Utilities',
    items: [
      { name: 'Business Intelligence', path: '/planning/business-intelligence', icon: BarChart3 },
      { name: 'Audits & Operations', path: '/operations/audits', icon: ShieldCheck }
    ]
  }
]

// Navigation items based on role
const navigation = computed(() => moduleGroups)

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
      :class="[
        'fixed lg:fixed top-0 left-0 h-screen bg-white border-r border-gray-200 transition-all duration-300 z-50 flex flex-col shadow-lg',
        collapsed && !isMobile ? 'w-16' : 'w-64',
        isMobile && !sidebarOpen ? '-translate-x-full' : 'translate-x-0',
        !isMobile ? 'lg:ml-2 lg:mt-2 lg:mb-2 lg:rounded-lg lg:h-[calc(100vh-1rem)]' : '',
        isMobile ? 'w-80' : ''
      ]"
    >
    <!-- Sidebar Header -->
      <div class="flex items-center justify-between h-16 px-4 border-b border-gray-200">
        <div v-if="!collapsed || isMobile" class="flex items-center gap-3">
          <div class="flex items-center justify-center w-8 h-8 bg-[#e91e63] text-white rounded-lg shadow-md">
            <span class="text-sm font-bold">T</span>
          </div>
          <div class="flex flex-col">
            <span class="text-sm font-bold text-gray-900">TOSS</span>
            <span class="text-[10px] text-gray-500">ERP III</span>
          </div>
        </div>
        <button
          @click="toggleSidebar"
          class="p-2 rounded-lg hover:bg-gray-100 text-gray-600 transition-colors"
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
          class="flex items-center gap-3 w-full p-2 rounded-lg hover:bg-gray-100 transition-colors"
        >
          <div class="w-10 h-10 rounded-full bg-[#e91e63] text-white flex items-center justify-center shadow-md">
            <UserCircle :size="20" />
          </div>
          <div class="flex-1 text-left">
            <p class="text-sm font-medium text-gray-900">{{ userInfo.name }}</p>
            <p class="text-xs text-gray-500 truncate">{{ userInfo.email }}</p>
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
    <nav class="flex-1 overflow-y-auto px-3 py-4 space-y-4 scrollbar-thin scrollbar-thumb-gray-300 scrollbar-track-gray-100">
      <div
        v-for="group in navigation"
        :key="group.title"
        class="space-y-1"
      >
        <p
          v-if="!collapsed || isMobile"
          class="px-3 text-xs font-semibold uppercase tracking-wider text-gray-500 mb-2"
        >
          {{ group.title }}
        </p>
        <NuxtLink
          v-for="item in group.items"
          :key="item.path"
          :to="item.path"
          :title="(collapsed && !isMobile) ? `${group.title} • ${item.name}` : ''"
          :class="[
            'flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium transition-all duration-200',
            isActive(item.path)
              ? 'bg-[#e91e63] text-white shadow-md'
              : 'text-gray-700 hover:bg-gray-100 active:bg-gray-200'
          ]"
          @click="isMobile && (sidebarOpen = false)"
        >
          <component :is="item.icon" :size="20" class="flex-shrink-0" />
          <span v-if="!collapsed || isMobile" class="truncate">{{ item.name }}</span>
        </NuxtLink>
      </div>
    </nav>

    <!-- Footer -->
      <div class="p-4 border-t border-gray-200">
      <NuxtLink
        to="/settings"
        :class="[
          'flex items-center gap-3 px-3 py-2 rounded-lg text-sm font-medium transition-colors',
          route.path === '/settings'
            ? 'bg-[#e91e63] text-white shadow-md'
            : 'text-gray-700 hover:bg-gray-100'
        ]"
      >
        <Settings :size="20" />
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
        class="fixed inset-0 bg-black/50 backdrop-blur-sm z-40 lg:hidden"
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

/* Scrollbar styling for mobile */
.scrollbar-thin::-webkit-scrollbar {
  width: 4px;
}

.scrollbar-thin::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 4px;
}

.scrollbar-thin::-webkit-scrollbar-thumb {
  background: #cbd5e1;
  border-radius: 4px;
}

.scrollbar-thin::-webkit-scrollbar-thumb:hover {
  background: #94a3b8;
}

/* Touch-friendly tap targets on mobile */
@media (max-width: 1023px) {
  nav a {
    min-height: 44px;
    touch-action: manipulation;
  }
}
</style>
