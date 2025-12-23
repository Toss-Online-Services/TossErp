<script setup lang="ts">
definePageMeta({
  layout: 'landing'
})
</script>

<template>
  <div class="min-h-screen bg-background py-12">
    <div class="container mx-auto px-4 max-w-4xl">
      <h1 class="text-3xl font-bold mb-8">Help Center</h1>
      
      <div class="space-y-6">
        <div class="bg-card border border-border rounded-lg p-6">
          <h2 class="text-xl font-semibold mb-4">Getting Started</h2>
          <p class="text-muted-foreground">
            Welcome to TOSS ERP! This help center will guide you through using our platform 
            designed specifically for South African SMMEs.
          </p>
        </div>
        
        <div class="bg-card border border-border rounded-lg p-6">
          <h2 class="text-xl font-semibold mb-4">Contact Support</h2>
          <p class="text-muted-foreground mb-4">
            Need assistance? Our support team is here to help.
          </p>
          <ul class="list-disc list-inside text-muted-foreground space-y-2">
            <li>Email: support@toss.co.za</li>
            <li>Phone: 0800-TOSS-HELP</li>
            <li>WhatsApp: +27 XX XXX XXXX</li>
          </ul>
        </div>

        <div class="bg-card border border-border rounded-lg p-6">
          <h2 class="text-xl font-semibold mb-4">FAQs</h2>
          <div class="space-y-4">
            <div>
              <h3 class="font-medium">How do I register my shop?</h3>
              <p class="text-muted-foreground text-sm">
                Click on "Register" and follow the step-by-step wizard to set up your shop profile.
              </p>
            </div>
            <div>
              <h3 class="font-medium">Can I use TOSS offline?</h3>
              <p class="text-muted-foreground text-sm">
                Yes! TOSS is designed to work offline. Your data syncs automatically when you're back online.
              </p>
            </div>
            <div>
              <h3 class="font-medium">What payment methods are supported?</h3>
              <p class="text-muted-foreground text-sm">
                We support cash, card payments, EFT, and mobile wallets including SnapScan and Zapper.
              </p>
            </div>
          </div>
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
