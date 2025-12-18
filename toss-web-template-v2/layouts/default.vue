<template>
  <div class="g-sidenav-show bg-gray-100">
    <!-- Sidenav -->
    <aside class="sidenav">
      <div class="sidenav-header">
        <NuxtLink to="/" class="sidenav-brand">
          <span class="sidenav-brand-img">
            <Icon name="material-symbols:dashboard-rounded" class="text-2xl text-info" />
          </span>
          <span>Material Dashboard</span>
        </NuxtLink>
      </div>
      
      <div class="sidenav-collapse">
        <ul class="nav">
          <li class="sidenav-item">
            <NuxtLink 
              to="/dashboards/analytics" 
              class="sidenav-link"
              active-class="sidenav-link-active"
            >
              <Icon name="material-symbols:dashboard-outline" class="sidenav-icon" />
              <span class="sidenav-text">Analytics</span>
            </NuxtLink>
          </li>
          <li class="sidenav-item">
            <NuxtLink 
              to="/dashboards/sales" 
              class="sidenav-link"
              active-class="sidenav-link-active"
            >
              <Icon name="material-symbols:shop-outline" class="sidenav-icon" />
              <span class="sidenav-text">Sales</span>
            </NuxtLink>
          </li>
          
          <!-- Pages Section -->
          <li class="sidenav-item mt-3">
            <h6 class="px-3 text-xs font-bold uppercase text-gray-600">Pages</h6>
          </li>
          <li class="sidenav-item">
            <NuxtLink 
              to="/pages/account/settings" 
              class="sidenav-link"
              active-class="sidenav-link-active"
            >
              <Icon name="material-symbols:settings-outline" class="sidenav-icon" />
              <span class="sidenav-text">Settings</span>
            </NuxtLink>
          </li>
          <li class="sidenav-item">
            <NuxtLink 
              to="/pages/account/billing" 
              class="sidenav-link"
              active-class="sidenav-link-active"
            >
              <Icon name="material-symbols:receipt-outline" class="sidenav-icon" />
              <span class="sidenav-text">Billing</span>
            </NuxtLink>
          </li>
          <li class="sidenav-item">
            <NuxtLink 
              to="/pages/pricing" 
              class="sidenav-link"
              active-class="sidenav-link-active"
            >
              <Icon name="material-symbols:attach-money" class="sidenav-icon" />
              <span class="sidenav-text">Pricing</span>
            </NuxtLink>
          </li>
          
          <!-- Applications Section -->
          <li class="sidenav-item mt-3">
            <h6 class="px-3 text-xs font-bold uppercase text-gray-600">Applications</h6>
          </li>
          <li class="sidenav-item">
            <NuxtLink 
              to="/applications/kanban" 
              class="sidenav-link"
              active-class="sidenav-link-active"
            >
              <Icon name="material-symbols:view-kanban-outline" class="sidenav-icon" />
              <span class="sidenav-text">Kanban</span>
            </NuxtLink>
          </li>
          <li class="sidenav-item">
            <NuxtLink 
              to="/applications/calendar" 
              class="sidenav-link"
              active-class="sidenav-link-active"
            >
              <Icon name="material-symbols:calendar-month-outline" class="sidenav-icon" />
              <span class="sidenav-text">Calendar</span>
            </NuxtLink>
          </li>
          <li class="sidenav-item">
            <NuxtLink 
              to="/applications/data-tables" 
              class="sidenav-link"
              active-class="sidenav-link-active"
            >
              <Icon name="material-symbols:table-chart-outline" class="sidenav-icon" />
              <span class="sidenav-text">Data Tables</span>
            </NuxtLink>
          </li>
          
          <!-- Ecommerce Section -->
          <li class="sidenav-item mt-3">
            <h6 class="px-3 text-xs font-bold uppercase text-gray-600">Ecommerce</h6>
          </li>
          <li class="sidenav-item">
            <NuxtLink 
              to="/ecommerce/products/new-product" 
              class="sidenav-link"
              active-class="sidenav-link-active"
            >
              <Icon name="material-symbols:add-box-outline" class="sidenav-icon" />
              <span class="sidenav-text">New Product</span>
            </NuxtLink>
          </li>
          <li class="sidenav-item">
            <NuxtLink 
              to="/ecommerce/orders/order-list" 
              class="sidenav-link"
              active-class="sidenav-link-active"
            >
              <Icon name="material-symbols:shopping-cart-outline" class="sidenav-icon" />
              <span class="sidenav-text">Order List</span>
            </NuxtLink>
          </li>
          
          <!-- Authentication Section -->
          <li class="sidenav-item mt-3">
            <h6 class="px-3 text-xs font-bold uppercase text-gray-600">Authentication</h6>
          </li>
          <li class="sidenav-item">
            <NuxtLink 
              to="/authentication/sign-in/basic" 
              class="sidenav-link"
              active-class="sidenav-link-active"
            >
              <Icon name="material-symbols:login" class="sidenav-icon" />
              <span class="sidenav-text">Sign In</span>
            </NuxtLink>
          </li>
          <li class="sidenav-item">
            <NuxtLink 
              to="/authentication/sign-up/cover" 
              class="sidenav-link"
              active-class="sidenav-link-active"
            >
              <Icon name="material-symbols:person-add-outline" class="sidenav-icon" />
              <span class="sidenav-text">Sign Up</span>
            </NuxtLink>
          </li>
        </ul>
      </div>
    </aside>
    
    <!-- Main content -->
    <main class="main-content">
      <!-- Navbar -->
      <nav class="navbar">
        <div class="container-fluid">
          <nav class="breadcrumb">
            <li class="breadcrumb-item">
              <NuxtLink to="/">Pages</NuxtLink>
            </li>
            <li class="breadcrumb-item active">
              {{ currentPage }}
            </li>
          </nav>
          
          <ul class="navbar-nav ml-auto">
            <li class="nav-item">
              <NuxtLink to="/pages/account/settings" class="nav-link">
                <Icon name="material-symbols:person-outline" class="nav-link-icon" />
              </NuxtLink>
            </li>
            <li class="nav-item">
              <NuxtLink to="/pages/notifications" class="nav-link">
                <Icon name="material-symbols:notifications-outline" class="nav-link-icon" />
              </NuxtLink>
            </li>
          </ul>
        </div>
      </nav>
      
      <!-- Page content -->
      <div class="container-fluid py-4">
        <slot />
      </div>
    </main>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()

const currentPage = computed(() => {
  const path = route.path
  const segments = path.split('/').filter(Boolean)
  return segments[segments.length - 1] || 'Dashboard'
})
</script>

<style scoped>
/* Additional layout-specific styles */
</style>
