<script setup lang="ts">
// Test page to verify layout is working
definePageMeta({
  layout: 'default'
})
</script>

<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-3xl font-bold">Layout Test Page</h1>
      <p class="text-muted-foreground mt-2">
        This page uses the default layout. You should see the sidebar and navbar.
      </p>
    </div>

    <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      <div class="bg-card border rounded-lg p-6 shadow-material-sm">
        <h2 class="text-xl font-semibold mb-2">Card 1</h2>
        <p class="text-sm text-muted-foreground">
          Using Material Dashboard Pro card styling
        </p>
      </div>
      <div class="bg-card border rounded-lg p-6 shadow-material-sm">
        <h2 class="text-xl font-semibold mb-2">Card 2</h2>
        <p class="text-sm text-muted-foreground">
          Using Material Dashboard Pro card styling
        </p>
      </div>
      <div class="bg-card border rounded-lg p-6 shadow-material-sm">
        <h2 class="text-xl font-semibold mb-2">Card 3</h2>
        <p class="text-sm text-muted-foreground">
          Using Material Dashboard Pro card styling
        </p>
      </div>
    </div>

    <div class="bg-primary text-primary-foreground p-6 rounded-lg">
      <h2 class="text-2xl font-semibold mb-2">Primary Color Test</h2>
      <p>This should show the Material Dashboard Pro primary color</p>
    </div>

    <div class="p-4 bg-yellow-50 border border-yellow-200 rounded-lg">
      <h3 class="font-semibold text-yellow-800 mb-2">Layout Verification</h3>
      <ul class="text-sm text-yellow-700 list-disc list-inside space-y-1">
        <li>✓ Sidebar should be visible on the left</li>
        <li>✓ Navbar should be visible at the top</li>
        <li>✓ Footer should be visible at the bottom (desktop)</li>
        <li>✓ Bottom navigation should be visible (mobile)</li>
        <li>✓ Content should be in the main area</li>
      </ul>
    </div>
  </div>
</template>

