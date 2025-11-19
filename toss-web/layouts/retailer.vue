<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <!-- Navigation Header -->
    <nav class="bg-white dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex justify-between h-16">
          <div class="flex">
            <div class="flex-shrink-0 flex items-center">
              <NuxtLink to="/retailer" class="text-xl font-bold text-blue-600 dark:text-blue-400">
                TOSS Retailer
              </NuxtLink>
            </div>
            <div class="hidden sm:ml-6 sm:flex sm:space-x-8">
              <NuxtLink
                to="/retailer/dashboard"
                class="inline-flex items-center px-1 pt-1 text-sm font-medium text-gray-900 dark:text-gray-100"
                active-class="border-b-2 border-blue-500"
              >
                Dashboard
              </NuxtLink>
              <NuxtLink
                to="/retailer/products"
                class="inline-flex items-center px-1 pt-1 text-sm font-medium text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300"
                active-class="border-b-2 border-blue-500"
              >
                Products
              </NuxtLink>
              <NuxtLink
                to="/retailer/inventory"
                class="inline-flex items-center px-1 pt-1 text-sm font-medium text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300"
                active-class="border-b-2 border-blue-500"
              >
                Inventory
              </NuxtLink>
              <NuxtLink
                to="/sales/pos"
                class="inline-flex items-center px-1 pt-1 text-sm font-medium text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300"
                active-class="border-b-2 border-blue-500"
              >
                POS
              </NuxtLink>
              <NuxtLink
                to="/retailer/orders"
                class="inline-flex items-center px-1 pt-1 text-sm font-medium text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300"
                active-class="border-b-2 border-blue-500"
              >
                Orders
              </NuxtLink>
            </div>
          </div>
          <div class="flex items-center">
            <div class="flex-shrink-0">
              <span class="text-sm text-gray-700 dark:text-gray-300 mr-4">
                {{ userStore.fullName }}
              </span>
              <button
                @click="handleLogout"
                class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700"
              >
                Logout
              </button>
            </div>
          </div>
        </div>
      </div>
    </nav>

    <!-- Main Content -->
    <main>
      <slot />
    </main>
  </div>
</template>

<script setup lang="ts">
const userStore = useUserStore()
const { logout } = useAuth()

const handleLogout = async () => {
  await logout()
  await navigateTo('/login')
}
</script>

