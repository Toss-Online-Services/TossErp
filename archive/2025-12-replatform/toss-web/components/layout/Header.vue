<template>
  <header class="flex justify-between items-center px-4 h-16 bg-white border-b border-gray-200 shadow-sm sm:px-6">
    <!-- Search Section -->
    <div class="flex-1 max-w-lg">
      <div class="relative">
        <MagnifyingGlassIcon class="absolute left-3 top-1/2 z-10 w-4 h-4 transform -translate-y-1/2 text-muted-foreground" />
        <Input 
          type="text" 
          placeholder="Search..." 
          class="pl-10 w-full"
        />
      </div>
    </div>

    <!-- Actions -->
    <div class="flex items-center ml-4 space-x-2 sm:space-x-4">
      <!-- Notifications -->
      <Button variant="ghost" size="icon" class="relative">
        <BellIcon class="w-5 h-5" />
        <span class="absolute top-1 right-1 w-2 h-2 rounded-full bg-destructive"></span>
      </Button>

      <!-- Theme Switcher -->
      <ThemeSwitcher />
      
      <!-- Desktop User Menu -->
      <div class="hidden sm:block">
        <UserMenu />
      </div>
    </div>
  </header>
</template>

<script setup>
import { MagnifyingGlassIcon, BellIcon } from '@heroicons/vue/24/outline'
import ThemeSwitcher from '~/components/common/ThemeSwitcher.vue'
import UserMenu from '~/components/common/UserMenu.vue'
// Input, Button, and Badge are auto-imported in Nuxt 4 with shadcn-vue
</script>
