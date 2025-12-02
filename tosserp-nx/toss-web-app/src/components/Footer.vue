<script setup lang="ts">
import { Heart } from 'lucide-vue-next'
</script>

<template>
  <footer class="bg-card border-t py-6 px-4 md:px-8">
    <div class="flex flex-col md:flex-row items-center justify-between gap-4">
      <div class="flex items-center gap-1 text-sm text-muted-foreground">
        <span>© 2025 TOSS ERP-III, made with</span>
        <Heart :size="16" class="text-red-500 fill-red-500" />
        <span>for township businesses.</span>
      </div>

      <div class="flex items-center gap-6 text-sm">
        <NuxtLink to="/settings" class="text-muted-foreground hover:text-primary transition-colors">
          Settings
        </NuxtLink>
        <NuxtLink to="/docs" class="text-muted-foreground hover:text-primary transition-colors">
          Help
        </NuxtLink>
      </div>
    </div>
  </footer>
</template>
