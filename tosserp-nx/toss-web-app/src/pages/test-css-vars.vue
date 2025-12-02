<script setup lang="ts">
// Test page to verify CSS variables are loading
</script>

<template>
  <div class="min-h-screen" style="background: hsl(var(--background)); color: hsl(var(--foreground)); padding: 2rem;">
    <div class="max-w-4xl mx-auto space-y-6">
      <h1 class="text-4xl font-bold" style="color: hsl(var(--foreground));">
        CSS Variables Test Page
      </h1>
      
      <div class="p-6 rounded-lg border" style="background: hsl(var(--card)); border-color: hsl(var(--border));">
        <h2 class="text-2xl font-semibold mb-4" style="color: hsl(var(--card-foreground));">
          Card with CSS Variables
        </h2>
        <p class="mb-4" style="color: hsl(var(--muted-foreground));">
          This card uses CSS variables from the Material Dashboard Pro theme.
        </p>
      </div>

      <div class="p-6 rounded-lg" style="background: hsl(var(--primary)); color: hsl(var(--primary-foreground));">
        <h2 class="text-2xl font-semibold mb-4">Primary Color Test</h2>
        <p>Material Dashboard Pro primary color (#e91e63)</p>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div class="p-4 rounded-lg" style="background: hsl(var(--secondary)); color: hsl(var(--secondary-foreground));">
          <h3 class="font-semibold mb-2">Secondary</h3>
          <p class="text-sm">Secondary color test</p>
        </div>
        <div class="p-4 rounded-lg" style="background: hsl(var(--accent)); color: hsl(var(--accent-foreground));">
          <h3 class="font-semibold mb-2">Accent</h3>
          <p class="text-sm">Accent color test</p>
        </div>
      </div>

      <div class="p-4 rounded-lg border-2" style="border-color: hsl(var(--destructive));">
        <h3 class="font-semibold mb-2" style="color: hsl(var(--destructive));">
          Destructive Color Test
        </h3>
        <p style="color: hsl(var(--muted-foreground));">
          Border uses destructive color variable
        </p>
      </div>

      <div class="p-4 bg-yellow-100 border-l-4 border-yellow-500 rounded">
        <strong>Debug Info:</strong>
        <ul class="mt-2 list-disc list-inside text-sm">
          <li>If colors appear, CSS variables are loading</li>
          <li>If everything is black/white, CSS variables may not be initialized</li>
          <li>Check browser DevTools → Elements → Computed styles</li>
        </ul>
      </div>
    </div>
  </div>
</template>

