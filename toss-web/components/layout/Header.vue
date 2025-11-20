<template>
  <header class="h-16 flex items-center justify-between px-4 sm:px-6 bg-white border-b border-gray-200 shadow-sm">
    <!-- Search Section -->
    <div class="flex-1 max-w-lg">
      <div class="relative">
        <MagnifyingGlassIcon class="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-muted-foreground z-10" />
        <Input 
          type="text" 
          placeholder="Search..." 
          class="w-full pl-10"
        />
      </div>
    </div>

    <!-- Actions -->
    <div class="flex items-center space-x-2 sm:space-x-4 ml-4">
      <!-- Notifications -->
      <Button variant="ghost" size="icon" class="relative">
        <BellIcon class="w-5 h-5" />
        <span class="absolute top-1 right-1 w-2 h-2 bg-destructive rounded-full"></span>
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
import { Input } from '@/components/ui/input'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
</script>
