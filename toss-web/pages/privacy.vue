<script setup lang="ts">
definePageMeta({
  layout: 'landing'
})
</script>

<template>
  <div class="min-h-screen bg-background py-12">
    <div class="container mx-auto px-4 max-w-4xl">
      <h1 class="text-3xl font-bold mb-8">Privacy Policy</h1>
      
      <div class="prose prose-sm max-w-none text-muted-foreground space-y-6">
        <div class="bg-card border border-border rounded-lg p-6">
          <p class="text-sm text-muted-foreground mb-4">
            Last updated: {{ new Date().toLocaleDateString('en-ZA') }}
          </p>
          
          <h2 class="text-xl font-semibold text-foreground mb-4">1. Introduction</h2>
          <p>
            TOSS Online Services ("we", "our", or "us") is committed to protecting your privacy.
            This Privacy Policy explains how we collect, use, disclose, and safeguard your information
            when you use our TOSS ERP platform.
          </p>
        </div>

        <div class="bg-card border border-border rounded-lg p-6">
          <h2 class="text-xl font-semibold text-foreground mb-4">2. Information We Collect</h2>
          <p class="mb-3">We collect information you provide directly to us, including:</p>
          <ul class="list-disc list-inside space-y-1">
            <li>Business registration details</li>
            <li>Contact information (name, email, phone number)</li>
            <li>Transaction and sales data</li>
            <li>Inventory and product information</li>
            <li>Location data for delivery services</li>
          </ul>
        </div>

        <div class="bg-card border border-border rounded-lg p-6">
          <h2 class="text-xl font-semibold text-foreground mb-4">3. How We Use Your Information</h2>
          <p class="mb-3">We use the information we collect to:</p>
          <ul class="list-disc list-inside space-y-1">
            <li>Provide, maintain, and improve our services</li>
            <li>Process transactions and send related information</li>
            <li>Connect you with suppliers and delivery partners</li>
            <li>Provide customer support</li>
            <li>Generate aggregated, anonymized market insights</li>
          </ul>
        </div>

        <div class="bg-card border border-border rounded-lg p-6">
          <h2 class="text-xl font-semibold text-foreground mb-4">4. Data Protection</h2>
          <p>
            We implement appropriate security measures to protect your personal information
            in accordance with the Protection of Personal Information Act (POPIA) of South Africa.
            Your data is encrypted both in transit and at rest.
          </p>
        </div>

        <div class="bg-card border border-border rounded-lg p-6">
          <h2 class="text-xl font-semibold text-foreground mb-4">5. Your Rights</h2>
          <p class="mb-3">Under POPIA, you have the right to:</p>
          <ul class="list-disc list-inside space-y-1">
            <li>Access your personal information</li>
            <li>Request correction of your data</li>
            <li>Request deletion of your data</li>
            <li>Object to processing of your data</li>
            <li>Withdraw consent at any time</li>
          </ul>
        </div>

        <div class="bg-card border border-border rounded-lg p-6">
          <h2 class="text-xl font-semibold text-foreground mb-4">6. Contact Us</h2>
          <p>
            If you have any questions about this Privacy Policy, please contact our 
            Information Officer at privacy@toss.co.za
          </p>
        </div>
      </div>
      
      <div class="mt-8 text-center">
        <NuxtLink to="/" class="text-primary hover:underline">
          ← Back to Home
        </NuxtLink>
      </div>
    </div>
  </div>
</template>
