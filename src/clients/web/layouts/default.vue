<template>
  <div class="min-h-screen bg-gray-100">
    <!-- Material Design Sidebar -->
    <aside 
      :class="[
        'material-sidebar',
        isSidebarOpen ? 'sidebar-open' : 'sidebar-closed',
        isSidebarMini ? 'sidebar-mini' : ''
      ]"
      @click.self="closeSidebar"
    >
      <!-- Sidebar Header -->
      <div class="sidebar-header">
        <div class="flex items-center justify-center">
          <div class="logo-container">
            <div class="logo-icon">
              <span class="text-white font-bold text-lg">CT</span>
            </div>
          </div>
          <span v-if="!isSidebarMini" class="logo-text">Creative Tim</span>
        </div>
        <button
          @click="toggleSidebarMini"
          class="sidebar-toggle-btn hidden lg:flex"
        >
          <Bars3Icon class="w-5 h-5 text-white" />
        </button>
        <button
          @click="closeSidebar"
          class="sidebar-close-btn lg:hidden"
        >
          <XMarkIcon class="w-5 h-5 text-white" />
        </button>
      </div>
      
      <!-- User Profile -->
      <div class="user-profile" v-if="!isSidebarMini">
        <div class="flex items-center px-6 py-4">
          <div class="w-10 h-10 rounded-full bg-white shadow-md flex items-center justify-center">
            <span class="text-gray-700 font-semibold text-sm">BA</span>
          </div>
          <div class="ml-3">
            <p class="text-white text-sm font-medium">Brooklyn Alice</p>
            <p class="text-white/70 text-xs">Administrator</p>
          </div>
        </div>
      </div>
      
      <!-- Navigation -->
      <nav class="sidebar-nav">
        <!-- Dashboard Section -->
        <div class="nav-section">
          <div class="nav-section-icon">
            <HomeIcon class="w-5 h-5" />
          </div>
          <div v-if="!isSidebarMini" class="nav-section-content">
            <h6 class="nav-section-title">Dashboards</h6>
            <div class="nav-section-items">
              <NuxtLink
                v-for="dashboardItem in dashboardItems"
                :key="dashboardItem.name"
                :to="dashboardItem.href"
                :class="[
                  'nav-item',
                  $route.path === dashboardItem.href ? 'nav-item-active' : ''
                ]"
              >
                <component :is="dashboardItem.icon" class="nav-item-icon" />
                {{ dashboardItem.name }}
              </NuxtLink>
            </div>
          </div>
        </div>

        <!-- Pages Section -->
        <div class="nav-section-header" v-if="!isSidebarMini">
          <span>PAGES</span>
        </div>

        <!-- Stock Management -->
        <div class="nav-section">
          <div class="nav-section-icon">
            <CubeIcon class="w-5 h-5" />
          </div>
          <div v-if="!isSidebarMini" class="nav-section-content">
            <h6 class="nav-section-title">Stock Management</h6>
            <div class="nav-section-items">
              <NuxtLink
                v-for="stockItem in stockItems"
                :key="stockItem.name"
                :to="stockItem.href"
                :class="[
                  'nav-item',
                  $route.path === stockItem.href ? 'nav-item-active' : ''
                ]"
              >
                <component :is="stockItem.icon" class="nav-item-icon" />
                {{ stockItem.name }}
              </NuxtLink>
            </div>
          </div>
        </div>

        <!-- Applications -->
        <div class="nav-section">
          <div class="nav-section-icon">
            <ChartBarIcon class="w-5 h-5" />
          </div>
          <div v-if="!isSidebarMini" class="nav-section-content">
            <h6 class="nav-section-title">Applications</h6>
            <div class="nav-section-items">
              <NuxtLink
                v-for="appItem in applicationItems"
                :key="appItem.name"
                :to="appItem.href"
                :class="[
                  'nav-item',
                  $route.path === appItem.href ? 'nav-item-active' : ''
                ]"
              >
                <component :is="appItem.icon" class="nav-item-icon" />
                {{ appItem.name }}
              </NuxtLink>
            </div>
          </div>
        </div>

        <!-- Authentication -->
        <div class="nav-section">
          <div class="nav-section-icon">
            <UserGroupIcon class="w-5 h-5" />
          </div>
          <div v-if="!isSidebarMini" class="nav-section-content">
            <h6 class="nav-section-title">Authentication</h6>
            <div class="nav-section-items">
              <NuxtLink
                v-for="authItem in authItems"
                :key="authItem.name"
                :to="authItem.href"
                :class="[
                  'nav-item',
                  $route.path === authItem.href ? 'nav-item-active' : ''
                ]"
              >
                <component :is="authItem.icon" class="nav-item-icon" />
                {{ authItem.name }}
              </NuxtLink>
            </div>
          </div>
        </div>
      </nav>
    </aside>

    <!-- Main Content Area -->
    <div :class="['main-content', isSidebarMini ? 'sidebar-mini-active' : '']">
      <!-- Material Header -->
      <header class="material-header">
        <div class="header-content">
          <!-- Breadcrumb -->
          <div class="flex items-center text-sm">
            <span class="text-gray-600">Pages</span>
            <span class="mx-2 text-gray-400">/</span>
            <span class="text-gray-900 font-medium">{{ pageTitle }}</span>
          </div>
          
          <!-- Header Right -->
          <div class="flex items-center space-x-3">
            <!-- Search -->
            <div class="search-container">
              <input
                type="text"
                placeholder="Search here"
                class="search-input"
                v-model="searchQuery"
              />
            </div>
            
            <!-- User Actions -->
            <button class="header-btn">
              <UserCircleIcon class="w-5 h-5" />
            </button>
            
            <button class="header-btn">
              <Cog6ToothIcon class="w-5 h-5" />
            </button>
            
            <!-- Notifications -->
            <div class="relative">
              <button class="header-btn notification-btn">
                <BellIcon class="w-5 h-5" />
                <span class="notification-badge">11</span>
              </button>
            </div>
          </div>
        </div>
      </header>

      <!-- Page Content -->
      <main class="page-content">
        <div class="content-container">
          <!-- Page Title -->
          <div class="page-title-section">
            <h1 class="page-title">{{ pageTitle }}</h1>
            <p class="page-subtitle">{{ pageSubtitle }}</p>
          </div>
          
          <!-- Page Content Slot -->
          <slot />
        </div>
      </main>
    </div>

    <!-- Mobile Overlay -->
    <div
      v-if="isSidebarOpen"
      class="fixed inset-0 z-40 bg-black bg-opacity-50 lg:hidden"
      @click="closeSidebar"
    ></div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import {
  Bars3Icon,
  XMarkIcon,
  BellIcon,
  HomeIcon,
  CubeIcon,
  ChartBarIcon,
  Cog6ToothIcon,
  UserGroupIcon,
  TruckIcon,
  UserCircleIcon,
  BuildingStorefrontIcon,
  DocumentTextIcon,
  CalendarIcon,
  ChartPieIcon,
  LockClosedIcon,
  KeyIcon
} from '@heroicons/vue/24/outline'

// Reactive state
const isSidebarOpen = ref(false)
const isSidebarMini = ref(false)
const searchQuery = ref('')

// Composables
const route = useRoute()

// Navigation items
const dashboardItems = [
  { name: 'Analytics', href: '/', icon: ChartPieIcon },
  { name: 'Discover', href: '/discover', icon: ChartBarIcon },
  { name: 'Sales', href: '/sales', icon: BuildingStorefrontIcon },
  { name: 'Stock Management', href: '/stock', icon: CubeIcon }
]

const stockItems = [
  { name: 'Items', href: '/items', icon: CubeIcon },
  { name: 'Warehouses', href: '/warehouses', icon: TruckIcon },
  { name: 'Stock Reports', href: '/stock-reports', icon: DocumentTextIcon }
]

const applicationItems = [
  { name: 'Calendar', href: '/calendar', icon: CalendarIcon },
  { name: 'Reports', href: '/reports', icon: ChartBarIcon },
  { name: 'Analytics', href: '/analytics', icon: ChartPieIcon }
]

const authItems = [
  { name: 'Users', href: '/users', icon: UserGroupIcon },
  { name: 'Settings', href: '/settings', icon: Cog6ToothIcon },
  { name: 'Security', href: '/security', icon: LockClosedIcon }
]

// Computed
const pageTitle = computed(() => {
  const allItems = [...dashboardItems, ...stockItems, ...applicationItems, ...authItems]
  const currentItem = allItems.find(item => item.href === route.path)
  return currentItem?.name || 'Analytics'
})

const pageSubtitle = computed(() => {
  const subtitles: Record<string, string> = {
    '/': 'Check the sales, value and bounce rate by country.',
    '/stock': 'Manage your inventory and stock levels.',
    '/items': 'Add, edit and organize your inventory items.',
    '/warehouses': 'Manage warehouse locations and capacity.',
    '/reports': 'View detailed analytics and reports.',
    '/users': 'Manage user accounts and permissions.',
    '/settings': 'Configure system settings and preferences.'
  }
  return subtitles[route.path] || 'Welcome to TOSS ERP'
})

// Methods
const openSidebar = () => {
  isSidebarOpen.value = true
}

const closeSidebar = () => {
  isSidebarOpen.value = false
}

const toggleSidebarMini = () => {
  isSidebarMini.value = !isSidebarMini.value
}
</script>

<style scoped>
/* Material Design Sidebar */
.material-sidebar {
  @apply fixed left-0 top-0 h-full w-64 bg-gradient-to-b from-gray-800 to-gray-900 text-white z-50 transition-all duration-300 ease-in-out;
  transform: translateX(-100%);
}

.sidebar-open {
  transform: translateX(0);
}

.sidebar-closed {
  transform: translateX(-100%);
}

.sidebar-mini {
  @apply w-16;
}

@media (min-width: 1024px) {
  .material-sidebar {
    transform: translateX(0);
  }
  
  .sidebar-closed {
    transform: translateX(0);
  }
}

/* Sidebar Header */
.sidebar-header {
  @apply h-16 px-6 flex items-center justify-between border-b border-gray-700;
}

.logo-container {
  @apply flex items-center;
}

.logo-icon {
  @apply w-10 h-10 bg-gradient-to-r from-blue-500 to-purple-600 rounded-lg flex items-center justify-center shadow-lg;
}

.logo-text {
  @apply ml-3 text-xl font-bold;
}

.sidebar-toggle-btn, .sidebar-close-btn {
  @apply p-2 rounded-lg hover:bg-gray-700 transition-colors;
}

/* User Profile */
.user-profile {
  @apply border-b border-gray-700;
}

/* Navigation */
.sidebar-nav {
  @apply flex-1 px-4 py-6 overflow-y-auto;
}

.nav-section {
  @apply mb-6 flex items-start;
}

.nav-section-icon {
  @apply w-12 flex items-center justify-center text-white/70;
}

.nav-section-content {
  @apply flex-1 ml-2;
}

.nav-section-title {
  @apply text-sm font-semibold text-white mb-2 uppercase tracking-wider;
}

.nav-section-header {
  @apply px-2 py-3 text-xs font-bold text-white/50 uppercase tracking-wider;
}

.nav-section-items {
  @apply space-y-1;
}

.nav-item {
  @apply flex items-center px-3 py-2 text-sm text-white/80 hover:text-white hover:bg-white/10 rounded-lg transition-all duration-200;
}

.nav-item-active {
  @apply text-white bg-white/20 shadow-md;
}

.nav-item-icon {
  @apply w-4 h-4 mr-3;
}

/* Main Content */
.main-content {
  @apply min-h-screen transition-all duration-300 ease-in-out;
  margin-left: 256px;
}

.sidebar-mini-active {
  margin-left: 64px;
}

@media (max-width: 1023px) {
  .main-content {
    margin-left: 0;
  }
  
  .sidebar-mini-active {
    margin-left: 0;
  }
}

/* Material Header */
.material-header {
  @apply bg-white shadow-sm border-b border-gray-200 sticky top-0 z-40;
}

.header-content {
  @apply flex items-center justify-between h-16 px-6;
}

/* Search */
.search-container {
  @apply relative;
}

.search-input {
  @apply w-64 px-4 py-2 text-sm bg-gray-50 border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent;
}

/* Header Buttons */
.header-btn {
  @apply p-2 rounded-lg text-gray-500 hover:text-gray-700 hover:bg-gray-100 transition-colors;
}

.notification-btn {
  @apply relative;
}

.notification-badge {
  @apply absolute -top-1 -right-1 w-5 h-5 bg-red-500 text-white text-xs rounded-full flex items-center justify-center;
}

/* Page Content */
.page-content {
  @apply bg-gray-100 min-h-screen;
}

.content-container {
  @apply p-6;
}

.page-title-section {
  @apply mb-6;
}

.page-title {
  @apply text-2xl font-bold text-gray-900;
}

.page-subtitle {
  @apply text-gray-600 mt-1;
}

/* Material Design Cards */
.material-card {
  @apply bg-white rounded-xl shadow-lg border border-gray-200 p-6 transition-all duration-200 hover:shadow-xl;
}

.material-card-header {
  @apply flex items-center justify-between mb-4;
}

.material-card-title {
  @apply text-lg font-semibold text-gray-900;
}

.material-card-subtitle {
  @apply text-sm text-gray-600;
}

/* Gradients */
.gradient-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.gradient-success {
  background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
}

.gradient-warning {
  background: linear-gradient(135deg, #fa709a 0%, #fee140 100%);
}

.gradient-danger {
  background: linear-gradient(135deg, #ff9a9e 0%, #fecfef 100%);
}

/* Stats Cards */
.stats-card {
  @apply bg-white rounded-xl p-6 shadow-lg border border-gray-200 relative overflow-hidden;
}

.stats-card::before {
  content: '';
  @apply absolute top-0 left-0 w-full h-1;
  background: linear-gradient(90deg, #667eea 0%, #764ba2 100%);
}

.stats-icon {
  @apply w-12 h-12 rounded-lg flex items-center justify-center text-white shadow-md;
}

.stats-value {
  @apply text-2xl font-bold text-gray-900;
}

.stats-label {
  @apply text-sm text-gray-600 font-medium;
}

.stats-change {
  @apply text-sm font-medium;
}

.stats-change.positive {
  @apply text-green-600;
}

.stats-change.negative {
  @apply text-red-600;
}
</style>
