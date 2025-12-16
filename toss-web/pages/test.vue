<template>
  <div class="page-wrapper" style="background-color: yellow !important; padding: 20px; color: black;">
    <h1 style="color: black; font-weight: bold; font-size: 2rem;">IF YOU SEE THIS YELLOW BOX, THE PAGE RENDERS</h1>
    <p style="color: black; font-size: 1.2rem;">Layout and routing are working correctly.</p>
    <div style="background-color: lime; padding: 10px; margin-top: 20px; color: black;">
      Main content is visible and functional.
    </div>
    <p style="color: black; margin-top: 20px;">{{ testMessage }}</p>
  </div>
</template>

<script setup lang="ts">
const testMessage = "Component is rendered and reactive data works"
console.log('Test page component mounted successfully')
</script>
