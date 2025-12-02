<script setup lang="ts">
// Test page without layout
definePageMeta({
  layout: false
})
</script>

<template>
  <div style="padding: 2rem; background: white; color: black; min-height: 100vh; font-family: Arial, sans-serif;">
    <h1 style="color: #e91e63; font-size: 2rem; margin-bottom: 1rem;">
      No Layout Test Page
    </h1>
    <p style="color: #333; font-size: 1rem; margin-bottom: 1rem;">
      This page bypasses the layout completely. If you see this, the issue is with the layout component.
    </p>
    
    <div style="background: #f5f5f5; padding: 1rem; border-radius: 8px; margin-bottom: 1rem;">
      <h2 style="color: #666; font-size: 1.25rem; margin-bottom: 0.5rem;">Diagnosis</h2>
      <ul style="color: #333; list-style: disc; padding-left: 2rem;">
        <li>If you see this page: Layout component has an error</li>
        <li>If you still see black: More fundamental issue (Vue/JS not loading)</li>
      </ul>
    </div>

    <div style="background: #e91e63; color: white; padding: 1rem; border-radius: 8px;">
      <h2 style="font-size: 1.25rem; margin-bottom: 0.5rem;">Material Dashboard Color</h2>
      <p>Primary color (#e91e63) test</p>
    </div>
  </div>
</template>

