<script setup lang="ts">
import { ref } from 'vue'
import AppShell from '~/components/layout/AppShell.vue'
import SidebarNav, { type NavSection } from '~/components/layout/SidebarNav.vue'
import TopbarNav from '~/components/layout/TopbarNav.vue'
import Footer from '~/components/layout/Footer.vue'
import OfflineIndicator from '~/components/OfflineIndicator.vue'

const mobileOpen = ref(false)

const navSections: NavSection[] = [
  {
    label: 'Dashboard',
    items: [{ label: 'Today', to: '/', icon: 'dashboard' }]
  },
  {
    label: 'Selling',
    items: [
      { label: 'Quotations', to: '/selling/quotations', icon: 'quotation' },
      { label: 'Sales Orders', to: '/selling/orders', icon: 'salesorder' },
      { label: 'Invoices', to: '/selling/invoices', icon: 'invoice' },
      { label: 'Deliveries', to: '/selling/deliveries', icon: 'delivery' }
    ]
  },
  {
    label: 'Buying',
    items: [
      { label: 'Purchase Orders', to: '/buying/purchase-orders', icon: 'po' },
      { label: 'Receipts', to: '/buying/receipts', icon: 'receipt' },
      { label: 'Suppliers', to: '/buying/suppliers', icon: 'supplier' }
    ]
  },
  {
    label: 'Stock',
    items: [
      { label: 'Items', to: '/stock/items', icon: 'items' },
      { label: 'Alerts', to: '/stock/alerts', icon: 'alerts' },
      { label: 'Movements', to: '/stock/movements', icon: 'movement' }
    ]
  },
  {
    label: 'Accounts',
    items: [
      { label: 'Summary', to: '/accounts/summary', icon: 'accounts' },
      { label: 'Cashbook', to: '/accounts/cashbook', icon: 'cashbook' },
      { label: 'Reports', to: '/accounts/reports', icon: 'reports' }
    ]
  },
  {
    label: 'CRM',
    items: [
      { label: 'Customers', to: '/crm/customers', icon: 'customers' },
      { label: 'Leads', to: '/crm/leads', icon: 'leads' }
    ]
  },
  {
    label: 'Projects',
    items: [
      { label: 'Projects', to: '/projects/list', icon: 'projects' },
      { label: 'Tasks', to: '/projects/tasks', icon: 'tasks' }
    ]
  },
  {
    label: 'POS',
    items: [{ label: 'Point of Sale', to: '/pos', icon: 'pos' }]
  },
  {
    label: 'Admin',
    items: [
      { label: 'Settings', to: '/settings', icon: 'settings' },
      { label: 'Users', to: '/admin/users', icon: 'users' }
    ]
  }
]

const closeMobile = () => {
  mobileOpen.value = false
}
</script>

<template>
  <AppShell>
    <template #sidebar="{ collapsed, toggle }">
      <SidebarNav
        :sections="navSections"
        :collapsed="collapsed"
        :on-toggle-collapse="toggle"
        :on-navigate="closeMobile"
      />
    </template>

    <template #topbar>
      <TopbarNav @toggle-sidebar="mobileOpen = true" />
    </template>

    <OfflineIndicator />
    <NuxtPage />

    <template #footer>
      <Footer />
    </template>

    <!-- Mobile sidebar overlay -->
    <transition name="fade">
      <div
        v-if="mobileOpen"
        class="fixed inset-0 z-30 bg-black/50 backdrop-blur-sm lg:hidden"
        @click="closeMobile"
      />
    </transition>

    <!-- Mobile sidebar drawer -->
    <transition name="slide">
      <aside
        v-if="mobileOpen"
        class="fixed inset-y-0 left-0 z-40 w-72 bg-card shadow-2xl lg:hidden overflow-hidden"
      >
        <SidebarNav :sections="navSections" :on-navigate="closeMobile" />
      </aside>
    </transition>
  </AppShell>
</template>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
.slide-enter-active,
.slide-leave-active {
  transition: transform 0.25s ease, opacity 0.2s ease;
}
.slide-enter-from,
.slide-leave-to {
  transform: translateX(-100%);
  opacity: 0;
}
</style>
