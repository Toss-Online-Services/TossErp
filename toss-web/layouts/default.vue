<script setup lang="ts">
import { ref } from 'vue'

const sidebarMinimized = ref(false)
const notificationsOpen = ref(false)

const toggleSidebarMinimize = () => {
  sidebarMinimized.value = !sidebarMinimized.value
}
</script>

<template>
  <div class="min-h-screen bg-gray-100">
    <!-- Sidebar -->
    <aside
      :class="[
        'fixed top-0 left-0 z-40 h-screen transition-all duration-300 my-2 ms-2 bg-white rounded-xl shadow-lg',
        sidebarMinimized ? 'w-20' : 'w-64'
      ]"
    >
      <!-- Sidebar Header -->
      <div class="px-4 py-3">
        <NuxtLink to="/" class="flex items-center">
          <div class="w-8 h-8 bg-gradient-to-br from-blue-500 to-blue-600 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-white text-xl">store</i>
          </div>
            <span v-if="!sidebarMinimized" class="ml-2 text-sm font-semibold text-gray-900">TOSS</span>
        </NuxtLink>
      </div>

      <hr class="border-gray-300 my-2 mx-3">

      <!-- Navigation -->
      <div class="px-2 pb-4 overflow-y-auto h-[calc(100vh-120px)]">
        <ul class="space-y-0.5">
          <!-- User Profile Section -->
          <li class="mb-2">
            <button class="flex items-center w-full px-3 py-2 text-gray-900 rounded-lg hover:bg-gray-100 transition-colors group">
              <img src="https://ui-avatars.com/api/?name=Brooklyn+Alice&background=3b82f6&color=fff" alt="User" class="w-8 h-8 rounded-full">
              <span v-if="!sidebarMinimized" class="ml-3 text-sm font-medium">Brooklyn Alice</span>
              <i v-if="!sidebarMinimized" class="material-symbols-rounded ml-auto text-gray-400 text-sm">expand_more</i>
            </button>
          </li>

          <hr class="border-gray-300 my-2">

          <!-- Dashboards Section -->
          <li>
            <button class="flex items-center w-full px-3 py-2 text-gray-900 rounded-lg hover:bg-gray-100 transition-colors group">
              <i class="material-symbols-rounded text-gray-600 group-hover:text-gray-900 text-xl">space_dashboard</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm">Dashboards</span>
              <i v-if="!sidebarMinimized" class="material-symbols-rounded ml-auto text-gray-400 text-sm">expand_more</i>
            </button>
          </li>

          <!-- Dashboard Sub-items -->
          <li>
            <NuxtLink
              to="/"
              class="flex items-center px-3 py-2 ml-8 text-gray-700 rounded-lg hover:bg-gray-100 transition-colors group"
              active-class="!bg-gray-900 !text-white"
            >
              <span v-if="!sidebarMinimized" class="text-xs font-medium mr-2">A</span>
              <span v-if="!sidebarMinimized" class="text-sm">Analytics</span>
            </NuxtLink>
          </li>
          <li>
            <NuxtLink
              to="/discover"
              class="flex items-center px-3 py-2 ml-8 text-gray-700 rounded-lg hover:bg-gray-100 transition-colors group"
            >
              <span v-if="!sidebarMinimized" class="text-xs font-medium mr-2">D</span>
              <span v-if="!sidebarMinimized" class="text-sm">Discover</span>
            </NuxtLink>
          </li>
          <li>
            <NuxtLink
              to="/sales-dashboard"
              class="flex items-center px-3 py-2 ml-8 text-gray-700 rounded-lg hover:bg-gray-100 transition-colors group"
            >
              <span v-if="!sidebarMinimized" class="text-xs font-medium mr-2">S</span>
              <span v-if="!sidebarMinimized" class="text-sm">Sales</span>
            </NuxtLink>
          </li>
          <li>
            <NuxtLink
              to="/automotive"
              class="flex items-center px-3 py-2 ml-8 text-gray-700 rounded-lg hover:bg-gray-100 transition-colors group"
            >
              <span v-if="!sidebarMinimized" class="text-xs font-medium mr-2">A</span>
              <span v-if="!sidebarMinimized" class="text-sm">Automotive</span>
            </NuxtLink>
          </li>
          <li>
            <NuxtLink
              to="/smart-home"
              class="flex items-center px-3 py-2 ml-8 text-gray-700 rounded-lg hover:bg-gray-100 transition-colors group"
            >
              <span v-if="!sidebarMinimized" class="text-xs font-medium mr-2">S</span>
              <span v-if="!sidebarMinimized" class="text-sm">Smart Home</span>
            </NuxtLink>
          </li>

          <!-- PAGES Section -->
          <li class="mt-3">
            <h6 v-if="!sidebarMinimized" class="px-3 text-xs font-bold text-gray-600 uppercase tracking-wider">Pages</h6>
          </li>
          <li>
            <button class="flex items-center w-full px-3 py-2 text-gray-900 rounded-lg hover:bg-gray-100 transition-colors group">
              <i class="material-symbols-rounded text-gray-600 group-hover:text-gray-900 text-xl">contract</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm">Pages</span>
              <i v-if="!sidebarMinimized" class="material-symbols-rounded ml-auto text-gray-400 text-sm">expand_more</i>
            </button>
          </li>

          <li>
            <button class="flex items-center w-full px-3 py-2 text-gray-900 rounded-lg hover:bg-gray-100 transition-colors group">
              <i class="material-symbols-rounded text-gray-600 group-hover:text-gray-900 text-xl">account_circle</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm">Account</span>
              <i v-if="!sidebarMinimized" class="material-symbols-rounded ml-auto text-gray-400 text-sm">expand_more</i>
            </button>
          </li>

          <li>
            <button class="flex items-center w-full px-3 py-2 text-gray-900 rounded-lg hover:bg-gray-100 transition-colors group">
              <i class="material-symbols-rounded text-gray-600 group-hover:text-gray-900 text-xl">apps</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm">Applications</span>
              <i v-if="!sidebarMinimized" class="material-symbols-rounded ml-auto text-gray-400 text-sm">expand_more</i>
            </button>
          </li>

          <li>
            <button class="flex items-center w-full px-3 py-2 text-gray-900 rounded-lg hover:bg-gray-100 transition-colors group">
              <i class="material-symbols-rounded text-gray-600 group-hover:text-gray-900 text-xl">storefront</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm">Ecommerce</span>
              <i v-if="!sidebarMinimized" class="material-symbols-rounded ml-auto text-gray-400 text-sm">expand_more</i>
            </button>
          </li>

          <li>
            <button class="flex items-center w-full px-3 py-2 text-gray-900 rounded-lg hover:bg-gray-100 transition-colors group">
              <i class="material-symbols-rounded text-gray-600 group-hover:text-gray-900 text-xl">group</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm">Team</span>
              <i v-if="!sidebarMinimized" class="material-symbols-rounded ml-auto text-gray-400 text-sm">expand_more</i>
            </button>
          </li>

          <li>
            <button class="flex items-center w-full px-3 py-2 text-gray-900 rounded-lg hover:bg-gray-100 transition-colors group">
              <i class="material-symbols-rounded text-gray-600 group-hover:text-gray-900 text-xl">widgets</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm">Projects</span>
              <i v-if="!sidebarMinimized" class="material-symbols-rounded ml-auto text-gray-400 text-sm">expand_more</i>
            </button>
          </li>

          <li>
            <button class="flex items-center w-full px-3 py-2 text-gray-900 rounded-lg hover:bg-gray-100 transition-colors group">
              <i class="material-symbols-rounded text-gray-600 group-hover:text-gray-900 text-xl">tv_signin</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm">Authentication</span>
              <i v-if="!sidebarMinimized" class="material-symbols-rounded ml-auto text-gray-400 text-sm">expand_more</i>
            </button>
          </li>

          <!-- DOCS Section -->
          <li class="mt-3">
            <hr class="border-gray-300 mb-3">
            <h6 v-if="!sidebarMinimized" class="px-3 text-xs font-bold text-gray-600 uppercase tracking-wider">Docs</h6>
          </li>
          <li>
            <button class="flex items-center w-full px-3 py-2 text-gray-900 rounded-lg hover:bg-gray-100 transition-colors group">
              <i class="material-symbols-rounded text-gray-600 group-hover:text-gray-900 text-xl">upcoming</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm">Basic</span>
              <i v-if="!sidebarMinimized" class="material-symbols-rounded ml-auto text-gray-400 text-sm">expand_more</i>
            </button>
          </li>

          <li>
            <button class="flex items-center w-full px-3 py-2 text-gray-900 rounded-lg hover:bg-gray-100 transition-colors group">
              <i class="material-symbols-rounded text-gray-600 group-hover:text-gray-900 text-xl">view_in_ar</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm">Components</span>
              <i v-if="!sidebarMinimized" class="material-symbols-rounded ml-auto text-gray-400 text-sm">expand_more</i>
            </button>
          </li>

          <li>
            <NuxtLink
              to="/changelog"
              class="flex items-center px-3 py-2 text-gray-900 rounded-lg hover:bg-gray-100 transition-colors group"
            >
              <i class="material-symbols-rounded text-gray-600 group-hover:text-gray-900 text-xl">receipt_long</i>
              <span v-if="!sidebarMinimized" class="ml-3 text-sm">Changelog</span>
            </NuxtLink>
          </li>
        </ul>
      </div>
    </aside>

    <!-- Main Content Area -->
    <main :class="['transition-all duration-300', sidebarMinimized ? 'ml-24' : 'ml-72']">
      <!-- Top Navbar -->
      <nav class="sticky top-2 z-30 mx-3 mt-2 rounded-xl bg-white/80 backdrop-blur-md shadow-md px-4 py-2">
        <div class="flex items-center justify-between">
          <!-- Left: Sidebar Toggle & Breadcrumbs -->
          <div class="flex items-center gap-4">
            <button
              @click="toggleSidebarMinimize"
              class="p-2 text-gray-600 hover:text-gray-900 transition-colors"
            >
              <div class="flex flex-col gap-1">
                <span class="w-5 h-0.5 bg-current"></span>
                <span class="w-5 h-0.5 bg-current"></span>
                <span class="w-5 h-0.5 bg-current"></span>
              </div>
            </button>

            <nav aria-label="breadcrumb">
              <ol class="flex items-center gap-2 text-sm">
                <li>
                  <a href="javascript:;" class="text-gray-500 hover:text-gray-700">Pages</a>
                </li>
                <li class="text-gray-400">/</li>
                <li class="text-gray-900 font-semibold">Analytics</li>
              </ol>
            </nav>
          </div>

          <!-- Right: Search, Notifications, Settings, User -->
          <div class="flex items-center gap-3">
            <!-- Search -->
            <div class="relative hidden md:block">
              <input
                type="text"
                placeholder="Search here"
                class="px-4 py-2 pr-10 text-sm bg-transparent border border-gray-300 rounded-lg focus:outline-none focus:border-blue-500 transition-colors w-64"
              >
              <i class="material-symbols-rounded absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 text-lg">search</i>
            </div>

            <!-- User Icon -->
            <button class="p-2 text-gray-600 hover:text-gray-900 transition-colors">
              <i class="material-symbols-rounded">account_circle</i>
            </button>

            <!-- Settings -->
            <button class="p-2 text-gray-600 hover:text-gray-900 transition-colors">
              <i class="material-symbols-rounded">settings</i>
            </button>

            <!-- Notifications -->
            <div class="relative">
              <button
                @click="notificationsOpen = !notificationsOpen"
                class="p-2 text-gray-600 hover:text-gray-900 transition-colors relative"
              >
                <i class="material-symbols-rounded">notifications</i>
                <span class="absolute top-0 right-0 w-4 h-4 bg-red-500 text-white text-xs rounded-full flex items-center justify-center">
                  11
                </span>
              </button>

              <!-- Notifications Dropdown -->
              <div
                v-if="notificationsOpen"
                class="absolute right-0 mt-2 w-80 bg-white rounded-xl shadow-lg border border-gray-200 py-2"
              >
                <a
                  href="javascript:;"
                  class="flex items-start gap-3 px-4 py-3 hover:bg-gray-50 transition-colors"
                >
                  <i class="material-symbols-rounded text-blue-500">email</i>
                  <div>
                    <h6 class="text-sm font-semibold text-gray-900">Check new messages</h6>
                  </div>
                </a>
                <a
                  href="javascript:;"
                  class="flex items-start gap-3 px-4 py-3 hover:bg-gray-50 transition-colors"
                >
                  <i class="material-symbols-rounded text-green-500">podcasts</i>
                  <div>
                    <h6 class="text-sm font-semibold text-gray-900">Manage podcast session</h6>
                  </div>
                </a>
                <a
                  href="javascript:;"
                  class="flex items-start gap-3 px-4 py-3 hover:bg-gray-50 transition-colors"
                >
                  <i class="material-symbols-rounded text-orange-500">shopping_cart</i>
                  <div>
                    <h6 class="text-sm font-semibold text-gray-900">Payment successfully completed</h6>
                  </div>
                </a>
              </div>
            </div>

            <!-- Mobile Sidebar Toggle -->
            <button
              @click="toggleSidebarMinimize"
              class="p-2 text-gray-600 hover:text-gray-900 transition-colors xl:hidden"
            >
              <div class="flex flex-col gap-1">
                <span class="w-5 h-0.5 bg-current"></span>
                <span class="w-5 h-0.5 bg-current"></span>
                <span class="w-5 h-0.5 bg-current"></span>
              </div>
            </button>
          </div>
        </div>
      </nav>

      <!-- Page Content -->
      <div class="px-4 py-4">
        <slot />
      </div>
    </main>
  </div>
</template>

<style>
.material-symbols-rounded {
  font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
}
</style>
