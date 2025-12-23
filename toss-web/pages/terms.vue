<script setup lang="ts">
definePageMeta({
  layout: 'landing'
})
</script>

<template>
  <div class="min-h-screen bg-background py-12">
    <div class="container mx-auto px-4 max-w-4xl">
      <h1 class="text-3xl font-bold mb-8">Terms of Service</h1>
      
      <div class="prose prose-sm max-w-none text-muted-foreground space-y-6">
        <div class="bg-card border border-border rounded-lg p-6">
          <p class="text-sm text-muted-foreground mb-4">
            Last updated: {{ new Date().toLocaleDateString('en-ZA') }}
          </p>
          
          <h2 class="text-xl font-semibold text-foreground mb-4">1. Agreement to Terms</h2>
          <p>
            By accessing or using the TOSS ERP platform, you agree to be bound by these Terms of Service.
            If you disagree with any part of these terms, you may not access the service.
          </p>
        </div>

        <div class="bg-card border border-border rounded-lg p-6">
          <h2 class="text-xl font-semibold text-foreground mb-4">2. Description of Service</h2>
          <p>
            TOSS provides an ERP platform designed for South African SMMEs, including features for
            point-of-sale, inventory management, purchasing, customer relationship management,
            and access to a collaborative supply network.
          </p>
        </div>

        <div class="bg-card border border-border rounded-lg p-6">
          <h2 class="text-xl font-semibold text-foreground mb-4">3. User Accounts</h2>
          <ul class="list-disc list-inside space-y-2">
            <li>You must provide accurate and complete registration information</li>
            <li>You are responsible for maintaining the security of your account</li>
            <li>You must notify us immediately of any unauthorized access</li>
            <li>You may not share your account credentials with others</li>
          </ul>
        </div>

        <div class="bg-card border border-border rounded-lg p-6">
          <h2 class="text-xl font-semibold text-foreground mb-4">4. Acceptable Use</h2>
          <p class="mb-3">You agree not to:</p>
          <ul class="list-disc list-inside space-y-1">
            <li>Use the service for any unlawful purpose</li>
            <li>Attempt to gain unauthorized access to any part of the service</li>
            <li>Interfere with or disrupt the service or servers</li>
            <li>Upload malicious code or harmful content</li>
            <li>Violate any applicable South African laws or regulations</li>
          </ul>
        </div>

        <div class="bg-card border border-border rounded-lg p-6">
          <h2 class="text-xl font-semibold text-foreground mb-4">5. Payment Terms</h2>
          <p>
            Certain features of TOSS may require payment. All fees are quoted in South African Rand (ZAR)
            and include VAT where applicable. Payment terms are specified at the time of subscription.
          </p>
        </div>

        <div class="bg-card border border-border rounded-lg p-6">
          <h2 class="text-xl font-semibold text-foreground mb-4">6. Intellectual Property</h2>
          <p>
            The TOSS platform and its original content, features, and functionality are owned by
            TOSS Online Services and are protected by South African and international copyright,
            trademark, and other intellectual property laws.
          </p>
        </div>

        <div class="bg-card border border-border rounded-lg p-6">
          <h2 class="text-xl font-semibold text-foreground mb-4">7. Limitation of Liability</h2>
          <p>
            To the maximum extent permitted by law, TOSS Online Services shall not be liable for any
            indirect, incidental, special, consequential, or punitive damages resulting from your
            use of the service.
          </p>
        </div>

        <div class="bg-card border border-border rounded-lg p-6">
          <h2 class="text-xl font-semibold text-foreground mb-4">8. Governing Law</h2>
          <p>
            These Terms shall be governed by and construed in accordance with the laws of the
            Republic of South Africa, without regard to its conflict of law provisions.
          </p>
        </div>

        <div class="bg-card border border-border rounded-lg p-6">
          <h2 class="text-xl font-semibold text-foreground mb-4">9. Contact Information</h2>
          <p>
            For questions about these Terms, please contact us at legal@toss.co.za
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
