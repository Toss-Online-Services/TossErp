<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import type { Component } from 'vue'
import { useRoute } from 'vue-router'
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
  Settings
} from 'lucide-vue-next'

interface MenuItem {
  label: string
  path: string
  icon: Component
}

interface MenuGroup {
  title: string
  items: MenuItem[]
}

const route = useRoute()
const isCollapsed = ref(false)
const isMobile = ref(false)
const sidebarOpen = ref(true)

const currentYear = new Date().getFullYear()

const menuGroups: MenuGroup[] = [
  {
    title: 'Executive Overview',
    items: [{ label: 'Dashboard', path: '/dashboard', icon: LayoutDashboard }]
  },
  {
    title: 'Commerce & Sales',
    items: [
      { label: 'Sales & Marketing', path: '/selling', icon: Megaphone },
      { label: 'POS & Store Solutions', path: '/selling/pos', icon: ShoppingCart },
      { label: 'Cross Commerce', path: '/selling/cross-commerce', icon: ArrowLeftRight }
    ]
  },
  {
    title: 'Planning & Intelligence',
    items: [
      { label: 'Planning & Assortment', path: '/planning/assortment', icon: Layers },
      { label: 'Merchandising Management', path: '/planning/merchandising', icon: ShoppingBag },
      { label: 'Business Intelligence', path: '/planning/business-intelligence', icon: BarChart3 }
    ]
  },
  {
    title: 'Relationships & Finance',
    items: [
      { label: 'Account Management', path: '/relationships/account', icon: UserCog },
      { label: 'Invoice Management', path: '/relationships/invoice', icon: FileText },
      { label: 'Vendor Relationship', path: '/relationships/vendor', icon: Handshake },
      { label: 'Customer Relationship', path: '/relationships/customer', icon: Users }
    ]
  },
  {
    title: 'Operations & Supply Chain',
    items: [
      { label: 'Audits & Operations', path: '/operations/audits', icon: ShieldCheck },
      { label: 'Inventory Management', path: '/operations/inventory', icon: Boxes },
      { label: 'Warehouse Management', path: '/operations/warehouse', icon: Package },
      { label: 'Supply & Chain Integration', path: '/operations/supply-chain', icon: Truck }
    ]
  }
]

const isActive = (path: string) => {
  if (path === '/') {
    return route.path === '/'
  }
  return route.path === path || route.path.startsWith(`${path}/`)
}

const handleResize = () => {
  isMobile.value = window.innerWidth < 1024
  sidebarOpen.value = !isMobile.value
  if (isMobile.value) {
    isCollapsed.value = false
  }
}

const toggleSidebar = () => {
  if (isMobile.value) {
    sidebarOpen.value = !sidebarOpen.value
  } else {
    isCollapsed.value = !isCollapsed.value
  }
}

onMounted(() => {
  handleResize()
  window.addEventListener('resize', handleResize)
})

onUnmounted(() => {
  window.removeEventListener('resize', handleResize)
})
</script>

<template>
  <div class="relative">
    <aside
      class="fixed lg:fixed top-0 left-0 flex flex-col border-r border-sidebar-border bg-sidebar shadow-sm transition-all duration-300 z-50 h-screen"
      :class="[
        isCollapsed && !isMobile ? 'w-16' : 'w-64',
        isMobile ? (sidebarOpen ? 'translate-x-0' : '-translate-x-full') : ''
      ]"
    >
      <div class="flex items-center justify-between h-16 px-4 border-b border-sidebar-border">
        <div class="flex items-center gap-3" :class="{ 'mx-auto': isCollapsed && !isMobile }">
          <div class="flex items-center justify-center w-8 h-8 rounded-lg bg-primary text-primary-foreground font-semibold">
            T
          </div>
          <div v-if="!isCollapsed || isMobile" class="flex flex-col leading-tight">
            <span class="text-sm font-semibold text-sidebar-foreground">TOSS ERP</span>
            <span class="text-xs text-muted-foreground">Material Shell</span>
          </div>
        </div>
        <button
          class="p-2 rounded-lg text-sidebar-foreground hover:text-sidebar-foreground hover:bg-sidebar-accent transition-colors"
          @click="toggleSidebar"
        >
          <span v-if="isMobile">✕</span>
          <span v-else>{{ isCollapsed ? '›' : '‹' }}</span>
        </button>
      </div>

    <nav class="flex-1 overflow-y-auto px-3 py-4 space-y-4">
      <div
        v-for="group in menuGroups"
        :key="group.title"
        class="space-y-1"
      >
        <p
          v-if="!isCollapsed || isMobile"
          class="px-3 text-xs font-semibold uppercase tracking-wider text-muted-foreground"
        >
          {{ group.title }}
        </p>
        <NuxtLink
          v-for="item in group.items"
          :key="item.path"
          :to="item.path"
          class="nav-link"
          :class="[
            isActive(item.path) ? 'nav-link-active' : '',
            isCollapsed && !isMobile ? 'justify-center' : ''
          ]"
          :title="isCollapsed && !isMobile ? `${group.title} • ${item.label}` : ''"
          @click="isMobile && (sidebarOpen = false)"
        >
          <component :is="item.icon" class="w-5 h-5" :class="isCollapsed && !isMobile ? '' : 'mr-3'" />
          <span v-if="!isCollapsed || isMobile">{{ item.label }}</span>
        </NuxtLink>
      </div>
      </nav>

      <div class="p-4 border-t border-sidebar-border space-y-2">
        <NuxtLink
          to="/settings"
          class="nav-link"
          :class="[route.path.startsWith('/settings') ? 'nav-link-active' : '', isCollapsed && !isMobile ? 'justify-center' : '']"
          @click="isMobile && (sidebarOpen = false)"
        >
          <Settings class="w-5 h-5" :class="isCollapsed && !isMobile ? '' : 'mr-3'" />
          <span v-if="!isCollapsed || isMobile">Settings</span>
        </NuxtLink>
        <p v-if="!isCollapsed || isMobile" class="text-xs text-center text-muted-foreground">
          © {{ currentYear }} · TOSS Online Services
        </p>
      </div>
    </aside>

    <div
      v-if="isMobile && sidebarOpen"
      class="fixed inset-0 bg-black/50 z-40"
      @click="sidebarOpen = false"
    />
  </div>
</template>

<style scoped>
.nav-link {
  display: flex;
  align-items: center;
  padding: 0.5rem 0.75rem;
  border-radius: 0.5rem;
  font-size: 0.875rem;
  font-weight: 500;
  color: hsl(var(--sidebar-foreground));
  transition: all 0.2s ease;
}

.nav-link:hover {
  background-color: hsl(var(--sidebar-accent));
  color: hsl(var(--sidebar-foreground));
}

.nav-link-active {
  background: #1A73E8;
  color: white;
  box-shadow: 0 10px 15px -3px rgba(26, 115, 232, 0.3);
}

.nav-link-active:hover {
  color: hsl(var(--primary-foreground));
}
</style>

