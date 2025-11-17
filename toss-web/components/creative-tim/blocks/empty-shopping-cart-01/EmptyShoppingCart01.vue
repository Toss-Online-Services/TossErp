<script setup lang="ts">
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'
</script>

<template>
  <section class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">empty-shopping-cart-01</p>
      <h2 class="text-xl font-semibold">Empty shopping cart state</h2>
    </div>
    <Card class="border-dashed border-border/70 bg-muted/20">
      <CardHeader class="items-center text-center">
        <div class="mb-4 h-16 w-16 rounded-full border border-dashed border-border/60 bg-card text-3xl leading-[64px]">
          🛒
        </div>
        <CardTitle>Your shopping cart is empty</CardTitle>
        <CardDescription>Store</CardDescription>
      </CardHeader>
      <CardContent class="flex flex-wrap justify-center gap-3">
        <Button variant="outline">
          Back to Store
        </Button>
        <Button>
          Explore Collections
        </Button>
      </CardContent>
    </Card>
  </section>
</template>

