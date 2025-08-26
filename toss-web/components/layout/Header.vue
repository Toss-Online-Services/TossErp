<template>
  <header class="h-16 flex items-center justify-between px-4 bg-white dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700">
    <div>
      <!-- Search bar -->
    </div>
    <div class="flex items-center">
      <ThemeSwitcher />
      <UserMenu />
    </div>
  </header>
</template>

<script setup>
import ThemeSwitcher from '~/components/common/ThemeSwitcher.vue'
import UserMenu from '~/components/common/UserMenu.vue'
</script>
