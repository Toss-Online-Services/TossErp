<script setup lang="ts">
import { ref } from 'vue'
import AppShell from '~/components/layout/AppShell.vue'
import SidebarNav, { type NavSection } from '~/components/layout/SidebarNav.vue'
import TopbarNav from '~/components/layout/TopbarNav.vue'
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
      { label: 'Quotations', to: '/selling/quotations', icon: 'request_quote' },
      { label: 'Sales Orders', to: '/selling/orders', icon: 'shopping_bag' },
      { label: 'Invoices', to: '/selling/invoices', icon: 'receipt_long' },
      { label: 'Deliveries', to: '/selling/deliveries', icon: 'local_shipping' }
    ]
  },
  {
    label: 'Buying',
    items: [
      { label: 'Purchase Orders', to: '/buying/purchase-orders', icon: 'assignment' },
      { label: 'Receipts', to: '/buying/receipts', icon: 'inventory' },
      { label: 'Suppliers', to: '/buying/suppliers', icon: 'group' }
    ]
  },
  {
    label: 'Stock',
    items: [
      { label: 'Items', to: '/stock/items', icon: 'inventory_2' },
      { label: 'Alerts', to: '/stock/alerts', icon: 'notification_important' },
      { label: 'Movements', to: '/stock/movements', icon: 'sync_alt' }
    ]
  },
  {
    label: 'Accounts',
    items: [
      { label: 'Summary', to: '/accounts/summary', icon: 'payments' },
      { label: 'Cashbook', to: '/accounts/cashbook', icon: 'account_balance_wallet' },
      { label: 'Reports', to: '/accounts/reports', icon: 'insights' }
    ]
  },
  {
    label: 'CRM',
    items: [
      { label: 'Customers', to: '/crm/customers', icon: 'contacts' },
      { label: 'Leads', to: '/crm/leads', icon: 'emoji_people' }
    ]
  },
  {
    label: 'Projects',
    items: [
      { label: 'Projects', to: '/projects/list', icon: 'work' },
      { label: 'Tasks', to: '/projects/tasks', icon: 'checklist' }
    ]
  },
  {
    label: 'POS',
    items: [{ label: 'Point of Sale', to: '/pos', icon: 'point_of_sale' }]
  },
  {
    label: 'Admin',
    items: [
      { label: 'Settings', to: '/settings', icon: 'settings' },
      { label: 'Users', to: '/admin/users', icon: 'shield_person' }
    ]
  }
]

const closeMobile = () => {
  mobileOpen.value = false
}
</script>

<template>
  <AppShell>
    <template #sidebar>
      <SidebarNav :sections="navSections" :on-navigate="closeMobile" />
    </template>

    <template #topbar>
      <TopbarNav @toggle-sidebar="mobileOpen = true" />
    </template>

    <OfflineIndicator />
    <NuxtPage />

    <transition name="fade">
      <div
        v-if="mobileOpen"
        class="fixed inset-0 z-30 bg-black/50 backdrop-blur-sm lg:hidden"
        @click="closeMobile"
      />
    </transition>
    <transition name="slide">
      <aside
        v-if="mobileOpen"
        class="fixed inset-y-0 left-0 z-40 w-72 bg-card shadow-xl lg:hidden"
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
  transition: transform 0.2s ease, opacity 0.2s ease;
}
.slide-enter-from,
.slide-leave-to {
  transform: translateX(-100%);
  opacity: 0;
}
</style>

