<script setup lang="ts">
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'
import { Badge } from '~/components/ui/badge'

type Collection = {
  title: string
  label: string
  description: string
  accent: string
  cta: string
  pill?: string
}

const props = withDefaults(
  defineProps<{
    collections?: Collection[]
  }>(),
  {
    collections: () => [
      {
        title: 'Urban Chick Collection',
        label: 'New',
        description: 'Gear up and enjoy savings | Up to 50% off',
        accent: 'from-primary/10 to-secondary/20',
        cta: 'Shop Sales',
        pill: 'New',
      },
      {
        title: 'Urban Chick Collection',
        label: 'Read More',
        description: 'Editorial looks curated by township stylists.',
        accent: 'from-amber-100 to-orange-100',
        cta: 'Read More',
        pill: 'Featured',
      },
      {
        title: 'Urban Chick Collection',
        label: '50% off',
        description: 'Weekend wardrobe edit with summer neutrals.',
        accent: 'from-rose-100 to-pink-100',
        cta: 'Shop Sales',
        pill: '50% OFF',
      },
      {
        title: 'Formal Elegance Series',
        label: 'Discover',
        description: 'Up to 70% off tuxedos and bespoke tailoring.',
        accent: 'from-slate-900 to-slate-800 text-white',
        cta: 'Discover',
        pill: 'UP TO 70%',
      },
    ],
  }
)
</script>

<template>
  <section class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">grid-ecommerce-01</p>
      <h2 class="text-xl font-semibold">Product grid with filters</h2>
    </div>

    <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
      <Card
        v-for="collection in props.collections"
        :key="collection.title + collection.label"
        class="border-border/70"
      >
        <CardHeader>
          <div class="flex items-center justify-between text-xs uppercase text-muted-foreground">
            <Badge variant="secondary">{{ collection.pill }}</Badge>
            <span>{{ collection.label }}</span>
          </div>
          <CardTitle>{{ collection.title }}</CardTitle>
          <CardDescription>{{ collection.description }}</CardDescription>
        </CardHeader>
        <CardContent>
          <Button variant="outline" class="w-full">
            {{ collection.cta }}
          </Button>
        </CardContent>
      </Card>
    </div>
  </section>
</template>

