<script setup lang="ts">
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'

type PromoCard = {
  headline: string
  title: string
  description: string
  price?: string
  action: string
  accent: string
}

const props = withDefaults(
  defineProps<{
    cards?: PromoCard[]
  }>(),
  {
    cards: () => [
      {
        headline: 'Active toning essence',
        title: 'Glow up with nature',
        description: 'Hydrating booster with baobab oil & rooibos extracts.',
        price: '$59.00',
        action: 'Shop product',
        accent: 'from-emerald-100 to-emerald-50',
      },
      {
        headline: 'Beauty model',
        title: 'Hydra Balance Serum',
        description: 'Balances sebum levels and calms sensitive skin.',
        action: 'Explore',
        accent: 'from-primary/20 to-secondary/10',
      },
      {
        headline: 'Limited drop',
        title: 'Midnight Ritual Set',
        description: 'Complete four-step night routine in travel sizes.',
        action: 'Add to cart',
        accent: 'from-rose-100 to-orange-50',
      },
    ],
  }
)
</script>

<template>
  <section class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">promotional-cards-01</p>
      <h2 class="text-xl font-semibold">Promotional product cards</h2>
    </div>
    <div class="grid gap-4 md:grid-cols-3">
      <Card
        v-for="card in props.cards"
        :key="card.title"
        class="border-border/70 bg-gradient-to-br"
        :class="card.accent"
      >
        <CardHeader>
          <CardDescription class="uppercase tracking-wide text-xs">
            {{ card.headline }}
          </CardDescription>
          <CardTitle class="text-2xl">{{ card.title }}</CardTitle>
        </CardHeader>
        <CardContent class="space-y-4">
          <p class="text-sm text-muted-foreground">{{ card.description }}</p>
          <div class="flex items-center justify-between">
            <p v-if="card.price" class="text-lg font-semibold">
              {{ card.price }}
            </p>
            <Button variant="outline" size="sm">
              {{ card.action }}
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  </section>
</template>

