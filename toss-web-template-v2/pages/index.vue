<script setup>
// Redirect to analytics dashboard on page load
definePageMeta({
  middleware: () => {
    return navigateTo('/dashboards/analytics')
  }
})
</script>

<template>
  <div></div>
</template>
