<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import {
  LayoutDashboard,
  Users,
  Building2,
  TrendingUp,
  CheckSquare,
  BarChart3,
  Settings,
  BookOpen,
  ChevronLeft,
  ChevronRight,
  CreditCard,
  Package,
  ShoppingCart,
  FileText,
  DollarSign,
  Warehouse,
  Wrench,
  UtensilsCrossed,
  Shirt,
  Car
} from 'lucide-vue-next'

const route = useRoute()
const sidebarOpen = ref(true)
const isMobile = ref(false)

// TOSS ERP Navigation - based on functional spec
const navigation = [
  { name: 'Dashboard', path: '/', icon: LayoutDashboard },
  { name: 'Point of Sale', path: '/pos', icon: ShoppingCart },
  { name: 'Sales', path: '/sales', icon: TrendingUp },
  { name: 'Stock', path: '/stock', icon: Warehouse },
  { name: 'Procurement', path: '/procurement', icon: Package },
  { name: 'Accounting', path: '/accounting', icon: DollarSign },
  { name: 'CRM', path: '/crm', icon: Users },
  { name: 'Manufacturing', path: '/manufacturing', icon: Wrench },
  { name: 'Projects', path: '/projects', icon: FileText },
  { name: 'Assets', path: '/assets', icon: Car },
  { name: 'HR & Payroll', path: '/hr', icon: Users },
  { name: 'Reports', path: '/reports', icon: BarChart3 },
  { name: 'Settings', path: '/settings', icon: Settings }
]

const checkMobile = () => {
  if (process.client) {
    isMobile.value = window.innerWidth < 1024
    if (isMobile.value) {
      sidebarOpen.value = false
    } else {
      sidebarOpen.value = true
    }
  }
}

const toggleSidebar = () => {
  sidebarOpen.value = !sidebarOpen.value
}

const closeSidebarOnMobile = () => {
  if (isMobile.value) {
    sidebarOpen.value = false
  }
}

onMounted(() => {
  checkMobile()
  if (process.client) {
    window.addEventListener('resize', checkMobile)
  }
})

onUnmounted(() => {
  if (process.client) {
    window.removeEventListener('resize', checkMobile)
  }
})
</script>

<template>
  <div class="flex h-screen bg-background">
    <div
      v-if="sidebarOpen && isMobile"
      @click="closeSidebarOnMobile"
      class="fixed inset-0 bg-black/50 z-40 lg:hidden"
    />

    <aside
      :class="[
        'bg-card border-r transition-all duration-300 flex flex-col fixed lg:relative h-full z-50 overflow-hidden',
        isMobile ? (sidebarOpen ? 'w-64' : 'w-0') : (sidebarOpen ? 'w-64' : 'w-16'),
        isMobile && !sidebarOpen ? '-translate-x-full' : 'translate-x-0'
      ]"
    >
      <div class="p-4 border-b flex items-center justify-between">
        <h3 v-if="sidebarOpen" class="text-sm font-semibold">TOSS ERP</h3>
        <button
          @click="toggleSidebar"
          class="p-2 hover:bg-accent rounded-md hidden lg:block"
          :class="{ 'mx-auto': !sidebarOpen }"
        >
          <ChevronRight v-if="!sidebarOpen" :size="20" />
          <ChevronLeft v-else :size="20" />
        </button>
      </div>

      <nav class="flex-1 p-4 space-y-1 overflow-y-auto custom-scrollbar">
        <NuxtLink
          v-for="item in navigation"
          :key="item.path"
          :to="item.path"
          @click="closeSidebarOnMobile"
          :class="[
            'flex items-center gap-2 px-3 py-2 rounded-lg transition-colors text-sm',
            route.path === item.path || (item.path !== '/' && route.path.startsWith(item.path))
              ? 'bg-primary text-primary-foreground'
              : 'hover:bg-accent hover:text-accent-foreground'
          ]"
        >
          <component :is="item.icon" :size="20" />
          <span v-if="sidebarOpen">{{ item.name }}</span>
        </NuxtLink>
      </nav>
    </aside>

    <div class="flex-1 flex flex-col overflow-hidden">
      <LayoutNavbar :on-toggle-sidebar="toggleSidebar" />

      <main class="flex-1 overflow-auto custom-scrollbar">
        <div class="p-4 md:p-8">
          <slot />
        </div>
      </main>

      <LayoutFooter />
    </div>
  </div>
</template>

