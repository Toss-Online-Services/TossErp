<script setup lang="ts">
import { Button } from '~/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'

definePageMeta({
  layout: 'landing',
})

const features = [
  {
    title: 'Point of Sale',
    description: 'Fast and efficient POS system for quick sales processing',
    icon: 'mdi:cash-register',
  },
  {
    title: 'Inventory Management',
    description: 'Track stock levels, manage products, and automate reordering',
    icon: 'mdi:package-variant-closed',
  },
  {
    title: 'CRM',
    description: 'Manage customer relationships and track interactions',
    icon: 'mdi:account-group',
  },
  {
    title: 'Group Buying',
    description: 'Join forces with other shops for better supplier prices',
    icon: 'mdi:account-multiple',
  },
  {
    title: 'Last-Mile Delivery',
    description: 'Coordinate deliveries with local drivers',
    icon: 'mdi:truck-delivery',
  },
  {
    title: 'AI Assistant',
    description: 'Get help with business decisions and automation',
    icon: 'mdi:robot',
  },
]
</script>

<template>
  <div class="min-h-screen bg-background">
    <!-- Hero Section -->
    <section class="container mx-auto px-4 py-20 text-center">
      <h1 class="text-4xl font-bold tracking-tight sm:text-5xl md:text-6xl">
        Welcome to TOSS ERP
      </h1>
      <p class="mt-6 text-lg text-muted-foreground max-w-3xl mx-auto">
        The complete ERP solution for township and rural SMMEs in South Africa.
        Manage sales, inventory, customers, and grow your business with AI-powered tools.
      </p>
      <div class="mt-10 flex flex-col sm:flex-row gap-4 justify-center">
        <Button size="lg" as-child>
          <NuxtLink to="/auth/register">
            Get Started
          </NuxtLink>
        </Button>
        <Button size="lg" variant="outline" as-child>
          <NuxtLink to="/auth/login">
            Sign In
          </NuxtLink>
        </Button>
      </div>
    </section>

    <!-- Features Section -->
    <section class="container mx-auto px-4 py-20">
      <div class="text-center mb-12">
        <h2 class="text-3xl font-bold tracking-tight">
          Everything You Need to Run Your Business
        </h2>
        <p class="mt-4 text-lg text-muted-foreground">
          Powerful features designed for South African township businesses
        </p>
      </div>

      <div class="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
        <Card v-for="feature in features" :key="feature.title">
          <CardHeader>
            <div class="flex items-center gap-3">
              <div class="p-2 bg-primary/10 rounded-lg">
                <Icon :name="feature.icon" class="h-6 w-6 text-primary" />
              </div>
              <CardTitle>{{ feature.title }}</CardTitle>
            </div>
          </CardHeader>
          <CardContent>
            <CardDescription>{{ feature.description }}</CardDescription>
          </CardContent>
        </Card>
      </div>
    </section>

    <!-- CTA Section -->
    <section class="container mx-auto px-4 py-20">
      <Card class="bg-primary text-primary-foreground">
        <CardHeader class="text-center">
          <CardTitle class="text-3xl">Ready to Transform Your Business?</CardTitle>
          <CardDescription class="text-primary-foreground/80 text-lg">
            Join thousands of South African businesses using TOSS ERP
          </CardDescription>
        </CardHeader>
        <CardContent class="flex justify-center">
          <Button size="lg" variant="secondary" as-child>
            <NuxtLink to="/auth/register">
              Start Free Trial
            </NuxtLink>
          </Button>
        </CardContent>
      </Card>
    </section>
  </div>
</template>
