<script setup lang="ts">
</script>

<template>
  <footer class="bg-card border-t py-4 px-4 md:px-8">
    <div class="flex flex-col md:flex-row items-center justify-between gap-4">
      <div class="text-sm text-muted-foreground">
        <p class="font-medium text-foreground">TOSS ERP-III</p>
        <p>© 2025 TOSS. Empowering township businesses.</p>
      </div>

      <div class="flex items-center gap-6 text-sm text-muted-foreground">
        <NuxtLink to="/settings" class="hover:text-primary transition-colors">
          Settings
        </NuxtLink>
        <NuxtLink to="/docs" class="hover:text-primary transition-colors">
          Help
        </NuxtLink>
      </div>
    </div>
  </footer>
</template>
