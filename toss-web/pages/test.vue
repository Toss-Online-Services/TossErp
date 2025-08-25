<template>
  <div class="test-page">
    <h1>TOSS Service as Software Platform - Test Page</h1>
    <p>If you can see this, the application is working correctly!</p>
    <div class="test-info">
      <h2>Service Status:</h2>
      <ul>
        <li>✅ Application loaded successfully</li>
        <li>✅ Vue.js components rendering</li>
        <li>✅ Nuxt.js routing working</li>
        <li>✅ Service as Software platform ready</li>
      </ul>
    </div>
    <div class="test-navigation">
      <h2>Test Navigation:</h2>
      <NuxtLink to="/" class="test-link">🏠 Home Page</NuxtLink>
      <NuxtLink to="/services" class="test-link">🤖 AI Services</NuxtLink>
      <NuxtLink to="/dashboard/service" class="test-link">📊 Business Dashboard</NuxtLink>
      <NuxtLink to="/onboarding/assessment" class="test-link">📋 Business Assessment</NuxtLink>
    </div>
  </div>
</template>

<script setup>
useHead({
  title: 'TOSS Test Page - Service as Software Platform',
  meta: [
    { name: 'description', content: 'Test page for TOSS Service as Software platform' }
  ]
})
</script>

<style scoped>
.test-page {
  padding: 2rem;
  max-width: 800px;
  margin: 0 auto;
  background: white;
  border-radius: 10px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.test-page h1 {
  color: #1d4ed8;
  margin-bottom: 1rem;
  text-align: center;
}

.test-page h2 {
  color: #374151;
  margin: 1.5rem 0 1rem 0;
}

.test-info ul {
  list-style: none;
  padding: 0;
}

.test-info li {
  padding: 0.5rem 0;
  border-bottom: 1px solid #e5e7eb;
}

.test-navigation {
  margin-top: 2rem;
}

.test-link {
  display: inline-block;
  margin: 0.5rem 1rem 0.5rem 0;
  padding: 0.75rem 1.5rem;
  background: #3b82f6;
  color: white;
  text-decoration: none;
  border-radius: 8px;
  transition: background-color 0.2s;
}

.test-link:hover {
  background: #2563eb;
}
</style>
