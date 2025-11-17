<script setup lang="ts">
import ShoppingCart01 from '~/components/creative-tim/blocks/shopping-cart-01'
import SimpleProductDetails01 from '~/components/creative-tim/blocks/simple-product-details-01'
import GridEcommerce01 from '~/components/creative-tim/blocks/grid-ecommerce-01'
import PromotionalCards01 from '~/components/creative-tim/blocks/promotional-cards-01'
import OrderHistory01 from '~/components/creative-tim/blocks/order-history-01'
import EmptyShoppingCart01 from '~/components/creative-tim/blocks/empty-shopping-cart-01'
import DigitalProductOverview01 from '~/components/creative-tim/blocks/digital-product-overview-01'
import InteractiveProductPreview01 from '~/components/creative-tim/blocks/interactive-product-preview-01'
import OrderDetails01 from '~/components/creative-tim/blocks/order-details-01'
import ProductDetails01 from '~/components/creative-tim/blocks/product-details-01'
import ProductListingFilters01 from '~/components/creative-tim/blocks/product-listing-filters-01'
import EcommerceSections01 from '~/components/creative-tim/blocks/ecommerce-sections-01'
import EcommerceSections02 from '~/components/creative-tim/blocks/ecommerce-sections-02'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'

definePageMeta({
  middleware: 'auth',
})
</script>

<template>
  <div class="space-y-10">
    <Card class="border-primary/40 bg-primary/5">
      <CardHeader class="flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between">
        <div class="space-y-2">
          <CardTitle class="text-2xl">Creative Tim · Ecommerce Blocks</CardTitle>
          <CardDescription>
            Showcase of shopping cart, product detail, promotional, and order management blocks inspired by
            Creative Tim’s shadcn/ui collection. Each block is ported into Nuxt + shadcn-vue so product, merchandising,
            and after-sales teams can reuse the layouts directly inside the ERP front-end.
          </CardDescription>
        </div>
        <Button variant="outline" as="a" href="https://www.creative-tim.com/ui/blocks/ecommerce" target="_blank">
          Browse original reference
        </Button>
      </CardHeader>
      <CardContent>
        <p class="text-sm text-muted-foreground">
          All layouts below are responsive, theme-aware, and rely exclusively on existing shadcn-vue primitives for tight
          integration with the Toss Web design system.
        </p>
      </CardContent>
    </Card>

    <ShoppingCart01 />
    <SimpleProductDetails01 />
    <GridEcommerce01 />
    <PromotionalCards01 />
    <OrderHistory01 />
    <EmptyShoppingCart01 />
    <DigitalProductOverview01 />
    <InteractiveProductPreview01 />
    <OrderDetails01 />
    <ProductDetails01 />
    <ProductListingFilters01 />
    <EcommerceSections01 />
    <EcommerceSections02 />
  </div>
</template>

