<script setup lang="ts">
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'
import { Badge } from '~/components/ui/badge'
import { Separator } from '~/components/ui/separator'

const features = [
  'Unlimited seats for your team',
  'Guided onboarding with AI coach',
  '500+ ecommerce templates',
  'Priority support with SLA',
]
</script>

<template>
  <section class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">digital-product-overview-01</p>
      <h2 class="text-xl font-semibold">Digital product with plan options</h2>
    </div>
    <Card class="border-border/70 bg-gradient-to-br from-primary/5 to-secondary/10">
      <CardHeader class="space-y-2">
        <Badge variant="secondary" class="w-fit">
          Bestseller
        </Badge>
        <CardTitle class="text-3xl">Komati Commerce Suite</CardTitle>
        <CardDescription>
          Add this overview section including pricing plan, CTAs, sales count, and star rating.
        </CardDescription>
      </CardHeader>
      <CardContent class="grid gap-6 lg:grid-cols-[1fr,320px]">
        <div class="space-y-3">
          <p class="text-sm font-semibold uppercase text-muted-foreground">
            Includes
          </p>
          <ul class="list-disc space-y-1 pl-5 text-sm text-muted-foreground">
            <li v-for="feature in features" :key="feature">
              {{ feature }}
            </li>
          </ul>
        </div>
        <Card class="border-primary/40 bg-card">
          <CardHeader>
            <CardTitle>Commerce Pro</CardTitle>
            <CardDescription>Billed month-to-month</CardDescription>
          </CardHeader>
          <CardContent class="space-y-4">
            <div>
              <div class="text-4xl font-semibold">R 1,999</div>
              <p class="text-xs text-muted-foreground">per workspace / month</p>
            </div>
            <Separator />
            <div class="space-y-2 text-sm text-muted-foreground">
              <p>12,840+ stores launched</p>
              <p>Rating 4.9 • 2,500+ reviews</p>
            </div>
          </CardContent>
          <CardFooter class="flex flex-col gap-3">
            <Button class="w-full">
              Start 14-day trial
            </Button>
            <Button variant="outline" class="w-full">
              Book a demo
            </Button>
          </CardFooter>
        </Card>
      </CardContent>
    </Card>
  </section>
</template>

