<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useRoute } from 'vue-router'
import Navbar from '@/components/Navbar.vue'
import Footer from '@/components/Footer.vue'
import {
  LayoutDashboard,
  ShoppingBag,
  Boxes,
  DollarSign,
  Users,
  Truck,
  Radio,
  Settings,
  HelpCircle,
  ChevronLeft,
  ChevronRight
} from 'lucide-vue-next'

const route = useRoute()
const sidebarOpen = ref(true)
const isMobile = ref(false)

const navigationSections = [
  {
    label: 'Run the shop',
    items: [
      {
        name: 'Home',
        description: 'KPI cards, AI nudges, daily pulse',
        path: '/dashboard',
        icon: LayoutDashboard
      },
      {
        name: 'Sales / POS',
        description: 'Offline-first cart, split payments',
        path: '/sales/pos',
        icon: ShoppingBag,
        signalKey: 'pos'
      },
      {
        name: 'Stock',
        description: 'Products, low stock alerts, adjustments',
        path: '/stock',
        icon: Boxes,
        signalKey: 'stock'
      },
      {
        name: 'Money',
        description: 'Money in/out, cash on hand, VAT',
        path: '/money',
        icon: DollarSign
      },
      {
        name: 'People',
        description: 'Customers, informal credit, staff',
        path: '/people',
        icon: Users
      }
    ]
  },
  {
    label: 'Network & Services',
    items: [
      {
        name: 'Jobs / Deliveries',
        description: 'Driver manifests, shared logistics',
        path: '/jobs/deliveries',
        icon: Truck,
        signalKey: 'deliveries'
      },
      {
        name: 'Community Deals',
        description: 'Group buying pools & supplier promos',
        path: '/network/deals',
        icon: Radio,
        signalKey: 'groupBuying'
      },
      {
        name: 'Settings',
        description: 'Tenants, roles, channels, integrations',
        path: '/settings',
        icon: Settings
      },
      {
        name: 'Help & Docs',
        description: 'Playbooks, training, WhatsApp support',
        path: '/docs',
        icon: HelpCircle
      }
    ]
  }
]

const moduleSignals: Record<
  string,
  {
    tone: 'warning' | 'info' | 'success'
    text: string
  }
> = {
  stock: { tone: 'warning', text: '3 low items' },
  deliveries: { tone: 'info', text: 'Driver en route' },
  groupBuying: { tone: 'success', text: 'Bulk flour -8%' },
  pos: { tone: 'info', text: '2 carts offline' }
}

const checkMobile = () => {
  isMobile.value = window.innerWidth < 1024
  sidebarOpen.value = !isMobile.value
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
  window.addEventListener('resize', checkMobile)
})

onUnmounted(() => {
  window.removeEventListener('resize', checkMobile)
})
</script>

<template>
  <div class="flex h-screen bg-background">
    <div v-if="sidebarOpen && isMobile" @click="closeSidebarOnMobile" class="fixed inset-0 bg-black/50 z-40 lg:hidden">
    </div>

    <aside :class="[
      'bg-card border-r transition-all duration-300 flex flex-col fixed lg:relative h-full z-50 overflow-hidden',
      isMobile ? (sidebarOpen ? 'w-64' : 'w-0') : (sidebarOpen ? 'w-64' : 'w-16'),
      isMobile && !sidebarOpen ? '-translate-x-full' : 'translate-x-0'
    ]">
      <div class="p-4 border-b flex items-center justify-between">
        <div v-if="sidebarOpen">
          <p class="text-xs uppercase tracking-wide text-muted-foreground">TOSS ERP III</p>
          <h3 class="text-sm font-semibold">Township Command Center</h3>
        </div>
        <button @click="toggleSidebar" class="p-2 hover:bg-accent rounded-md hidden lg:block"
          :class="{ 'mx-auto': !sidebarOpen }">
          <ChevronRight v-if="!sidebarOpen" :size="20" />
          <ChevronLeft v-else :size="20" />
        </button>
      </div>

      <nav class="flex-1 p-4 space-y-5 overflow-y-auto">
        <div v-for="section in navigationSections" :key="section.label">
          <p
            v-if="sidebarOpen"
            class="text-xs uppercase tracking-wide text-muted-foreground mb-2"
          >
            {{ section.label }}
          </p>
          <div class="space-y-1">
            <NuxtLink
              v-for="item in section.items"
              :key="item.path"
              :to="item.path"
              @click="closeSidebarOnMobile"
              :class="[
                'flex items-center gap-2 px-3 py-2 rounded-lg transition-colors text-sm',
                route.path.startsWith(item.path)
                  ? 'bg-primary text-primary-foreground'
                  : 'hover:bg-accent hover:text-accent-foreground'
              ]"
            >
              <component :is="item.icon" :size="20" />
              <div v-if="sidebarOpen" class="flex-1">
                <p class="font-medium">{{ item.name }}</p>
                <p class="text-xs text-muted-foreground">{{ item.description }}</p>
              </div>
              <span
                v-if="item.signalKey && sidebarOpen"
                class="text-[10px] px-2 py-0.5 rounded-full font-semibold"
                :class="{
                  'bg-amber-100 text-amber-800': moduleSignals[item.signalKey]?.tone === 'warning',
                  'bg-sky-100 text-sky-800': moduleSignals[item.signalKey]?.tone === 'info',
                  'bg-emerald-100 text-emerald-800': moduleSignals[item.signalKey]?.tone === 'success'
                }"
              >
                {{ moduleSignals[item.signalKey]?.text }}
              </span>
            </NuxtLink>
          </div>
        </div>
      </nav>

      <div class="p-4 border-t space-y-2 text-xs text-muted-foreground" v-if="sidebarOpen">
        <p class="font-semibold text-sm text-foreground">Next delivery</p>
        <p>AfroSupply driver Musa arriving at 14:35</p>
        <p class="text-[11px]">Have stock ready for cross-dock to Spaza Club partner.</p>
      </div>
    </aside>

    <div class="flex-1 flex flex-col overflow-hidden">
      <Navbar :on-toggle-sidebar="toggleSidebar" />

      <main class="flex-1 overflow-auto">
        <div class="p-4 md:p-8">
          <slot />
        </div>
      </main>

      <Footer />
    </div>
  </div>
</template>
