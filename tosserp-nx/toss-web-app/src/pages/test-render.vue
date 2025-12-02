<script setup lang="ts">
// Minimal test page to verify basic rendering
</script>

<template>
  <div style="padding: 2rem; background: white; color: black; min-height: 100vh;">
    <h1 style="color: #e91e63; font-size: 2rem; margin-bottom: 1rem;">
      TOSS ERP Test Page
    </h1>
    <p style="color: #333; font-size: 1rem; margin-bottom: 1rem;">
      If you can see this, basic rendering is working!
    </p>
    
    <div style="background: #f5f5f5; padding: 1rem; border-radius: 8px; margin-bottom: 1rem;">
      <h2 style="color: #666; font-size: 1.25rem; margin-bottom: 0.5rem;">Test Information</h2>
      <ul style="color: #333; list-style: disc; padding-left: 2rem;">
        <li>Vue is rendering</li>
        <li>HTML structure is working</li>
        <li>Inline styles are applied</li>
      </ul>
    </div>

    <div style="background: #e91e63; color: white; padding: 1rem; border-radius: 8px; margin-bottom: 1rem;">
      <h2 style="font-size: 1.25rem; margin-bottom: 0.5rem;">Material Dashboard Color Test</h2>
      <p>Primary color (#e91e63) is visible</p>
    </div>

    <div style="display: flex; gap: 1rem; flex-wrap: wrap;">
      <button style="background: #e91e63; color: white; padding: 0.75rem 1.5rem; border: none; border-radius: 4px; cursor: pointer;">
        Primary Button
      </button>
      <button style="background: #f5f5f5; color: #333; padding: 0.75rem 1.5rem; border: 1px solid #ddd; border-radius: 4px; cursor: pointer;">
        Secondary Button
      </button>
    </div>

    <div style="margin-top: 2rem; padding: 1rem; background: #fff3cd; border-left: 4px solid #ffc107; border-radius: 4px;">
      <strong>Next Steps:</strong>
      <ol style="margin-top: 0.5rem; padding-left: 1.5rem;">
        <li>If this page renders, the issue is with the layout or CSS variables</li>
        <li>If this page doesn't render, there's a more fundamental issue</li>
        <li>Check browser console for any errors</li>
      </ol>
    </div>
  </div>
</template>

