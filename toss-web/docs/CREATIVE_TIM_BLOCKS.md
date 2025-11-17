## Creative Tim Ecommerce Blocks

### Overview

The `/sales/ecommerce-blocks` route showcases a Nuxt-native port of the Creative Tim ecommerce block library that lives at [creative-tim.com/ui/blocks/ecommerce](https://www.creative-tim.com/ui/blocks/ecommerce). Each block is rebuilt with shadcn-vue primitives, so the layouts inherit Toss ERP theming, spacing, and typography while matching the reference designs.

### Included Blocks

- `shopping-cart-01` – Cart grid with summary card
- `simple-product-details-01` – Hero product detail grid
- `grid-ecommerce-01` – Promotional tiles with CTAs
- `promotional-cards-01` – Gradient promo cards
- `order-history-01` – Customer orders table
- `empty-shopping-cart-01` – Empty state message
- `digital-product-overview-01` – SaaS plan overview
- `interactive-product-preview-01` – Room previewer concept
- `order-details-01` – Order summary plus timeline
- `product-details-01` – Product hero with carousel
- `product-listing-filters-01` – Grid with filter pane
- `ecommerce-sections-01` – Video spotlight + product cards
- `ecommerce-sections-02` – Luxury detail page with gallery

Every block lives under `components/creative-tim/blocks/<block-name>/` with a Vue component and an `index.ts` barrel file to make imports ergonomic:

```ts
import ShoppingCart01 from '~/components/creative-tim/blocks/shopping-cart-01'
```

### Usage

- Drop the components inside any Nuxt page or feature module. They are self-contained and expose light props (i.e., `items?`, `collections?`) with sensible defaults.
- For custom content, clone a block into a feature module and replace the sample data.
- Because the blocks lean on shadcn-vue atoms (`Card`, `Tabs`, `Badge`, etc.), they automatically respect the OKLCH theme tokens in `assets/css/main.css`.

### Navigation

The Sales & CRM navigation now includes an **Ecommerce Blocks** entry pointing to `/sales/ecommerce-blocks`, making the showcase easy to find for designers and product teams.

