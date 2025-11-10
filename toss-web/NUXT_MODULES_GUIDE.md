# TOSS ERP - Nuxt Modules Guide

This document provides an overview of all Nuxt modules installed in the TOSS ERP project and how to use them.

## Installed Modules

### Core & Styling

#### 1. @nuxtjs/tailwindcss
**Purpose**: Utility-first CSS framework for rapid UI development
**Usage**: 
```vue
<template>
  <div class="bg-blue-500 text-white p-4 rounded-lg">
    Styled with Tailwind
  </div>
</template>
```

#### 2. @nuxtjs/color-mode
**Purpose**: Dark/Light mode support with auto-detection
**Usage**:
```vue
<script setup>
const colorMode = useColorMode()
</script>

<template>
  <button @click="colorMode.preference = colorMode.value === 'dark' ? 'light' : 'dark'">
    Toggle {{ colorMode.value }} mode
  </button>
</template>
```

#### 3. @nuxt/fonts
**Purpose**: Optimized font loading with Google Fonts and custom fonts
**Configuration**: Already configured in `nuxt.config.ts` with Inter and Roboto fonts

#### 4. @nuxt/icon
**Purpose**: Icon components with 100,000+ icons from Iconify
**Usage**:
```vue
<template>
  <Icon name="mdi:cart" size="24" />
  <Icon name="cart" /> <!-- Using alias from config -->
</template>
```

#### 5. @nuxt/image
**Purpose**: Optimized image handling with automatic WebP conversion
**Usage**:
```vue
<template>
  <NuxtImg 
    src="/products/item.jpg" 
    width="400" 
    height="300" 
    format="webp" 
    quality="80"
  />
</template>
```

---

### State Management

#### 6. @pinia/nuxt
**Purpose**: Official Vue state management with TypeScript support
**Usage**:
```typescript
// stores/user.ts
export const useUserStore = defineStore('user', {
  state: () => ({
    name: '',
    email: ''
  }),
  actions: {
    setUser(user) {
      this.name = user.name
      this.email = user.email
    }
  }
})

// In component
const userStore = useUserStore()
userStore.setUser({ name: 'John', email: 'john@example.com' })
```

#### 7. @vueuse/nuxt
**Purpose**: Collection of essential Vue composition utilities
**Usage**:
```vue
<script setup>
const { x, y } = useMouse()
const { online } = useOnline()
const { copy, copied } = useClipboard()
</script>
```

---

### PWA & Performance

#### 8. @vite-pwa/nuxt
**Purpose**: Progressive Web App support for offline functionality
**Features**: 
- Service Worker generation
- Offline caching
- App manifest
- Install prompts

#### 9. @nuxtjs/web-vitals
**Purpose**: Monitor Core Web Vitals (LCP, FID, CLS, TTFB)
**Usage**: Automatically tracks performance metrics in production

#### 10. @nuxtjs/partytown
**Purpose**: Run third-party scripts in a web worker for better performance
**Usage**: Automatically optimizes analytics and tracking scripts

---

### Internationalization

#### 11. @nuxtjs/i18n
**Purpose**: Internationalization for multi-language support
**Configured Languages**:
- English (en)
- isiZulu (zu)
- isiXhosa (xh)
- Afrikaans (af)
- Sesotho (st)

**Usage**:
```vue
<script setup>
const { t, locale } = useI18n()
</script>

<template>
  <h1>{{ t('common.welcome') }}</h1>
  <button @click="locale = 'zu'">Switch to Zulu</button>
</template>
```

---

### Forms & Validation

#### 12. @formkit/nuxt
**Purpose**: Comprehensive form building with validation
**Usage**:
```vue
<template>
  <FormKit
    type="form"
    @submit="handleSubmit"
  >
    <FormKit
      type="text"
      name="email"
      label="Email"
      validation="required|email"
    />
    <FormKit
      type="password"
      name="password"
      label="Password"
      validation="required|length:8"
    />
  </FormKit>
</template>
```

#### 13. @vee-validate/nuxt
**Purpose**: Form validation with comprehensive rules
**Usage**:
```vue
<script setup>
import { useForm } from 'vee-validate'
import * as yup from 'yup'

const schema = yup.object({
  email: yup.string().required().email(),
  password: yup.string().required().min(8)
})

const { handleSubmit, errors } = useForm({
  validationSchema: schema
})
</script>
```

---

### SEO & Analytics

#### 14. @nuxtjs/sitemap
**Purpose**: Automatic XML sitemap generation
**Configuration**: Routes defined in `nuxt.config.ts`

#### 15. @nuxtjs/robots
**Purpose**: Robots.txt generation
**Configuration**: Blocks `/api/`, `/admin/`, `/private/` paths

#### 16. nuxt-schema-org
**Purpose**: Schema.org structured data for SEO
**Benefits**: Better search engine understanding and rich snippets

#### 17. nuxt-gtag
**Purpose**: Google Analytics 4 integration
**Setup**: Set `NUXT_PUBLIC_GTAG_ID` in `.env`

---

### Content & Utilities

#### 18. @nuxt/content
**Purpose**: File-based CMS with Markdown support
**Usage**:
```vue
<template>
  <ContentDoc path="/docs/guide" />
</template>
```

Create content in `content/` directory:
```markdown
---
title: Getting Started
description: Learn how to use TOSS ERP
---

# Getting Started

Welcome to TOSS ERP!
```

#### 19. nuxt-lodash
**Purpose**: Lodash utility functions
**Usage**:
```vue
<script setup>
const items = [1, 2, 3, 4, 5]
const chunked = _chunk(items, 2) // [[1,2], [3,4], [5]]
const unique = _uniq([1, 2, 2, 3, 3, 3])
</script>
```

---

### Device Detection

#### 20. @nuxtjs/device
**Purpose**: Detect device type (mobile, tablet, desktop)
**Usage**:
```vue
<script setup>
const { isMobile, isTablet, isDesktop } = useDevice()
</script>

<template>
  <div v-if="isMobile">Mobile View</div>
  <div v-else-if="isTablet">Tablet View</div>
  <div v-else>Desktop View</div>
</template>
```

---

### UI Components

#### 21. nuxt-swiper
**Purpose**: Swiper/carousel component for touch sliders
**Usage**:
```vue
<template>
  <Swiper
    :modules="[SwiperAutoplay, SwiperPagination]"
    :slides-per-view="1"
    :autoplay="{ delay: 3000 }"
    :pagination="{ clickable: true }"
  >
    <SwiperSlide v-for="item in items" :key="item.id">
      <img :src="item.image" />
    </SwiperSlide>
  </Swiper>
</template>
```

---

### Security & Monitoring

#### 22. nuxt-security
**Purpose**: Security headers and rate limiting
**Features**:
- Content Security Policy (CSP)
- CORS configuration
- Rate limiting
- XSS protection

#### 23. @sentry/nuxt
**Purpose**: Error tracking and performance monitoring
**Setup**: Configure `NUXT_PUBLIC_SENTRY_DSN` in `.env`

---

## Module Configuration Tips

### Performance Optimization

1. **Lazy Load Heavy Components**:
```vue
<script setup>
const HeavyChart = defineAsyncComponent(() => 
  import('~/components/charts/HeavyChart.vue')
)
</script>
```

2. **Use Image Optimization**:
```vue
<!-- Instead of regular img -->
<NuxtImg src="/large-image.jpg" width="800" height="600" format="webp" />
```

3. **Leverage Offline Caching (PWA)**:
- API responses cached for 5 minutes
- Images cached for 30 days
- Fonts cached for 1 year

### Internationalization Best Practices

1. **Extract all text to locale files**:
```json
// locales/en.json
{
  "sales": {
    "newSale": "New Sale",
    "total": "Total: {amount}"
  }
}
```

2. **Use interpolation**:
```vue
{{ t('sales.total', { amount: formatCurrency(total) }) }}
```

### Security Best Practices

1. **Configure CSP for external resources**:
   - Already configured in `nuxt.config.ts`
   - Add new domains to CSP when integrating third-party services

2. **Rate Limiting**:
   - Configured at 150 requests per minute
   - Adjust in `nuxt.config.ts` based on needs

### SEO Optimization

1. **Use Schema.org**:
```vue
<script setup>
useSchemaOrg([
  defineProduct({
    name: 'Product Name',
    image: '/product.jpg',
    price: 299.99,
    priceCurrency: 'ZAR'
  })
])
</script>
```

2. **Dynamic Meta Tags**:
```vue
<script setup>
useSeoMeta({
  title: 'Sales Dashboard - TOSS ERP',
  description: 'Manage your sales efficiently',
  ogImage: '/og-sales.jpg'
})
</script>
```

---

## Common Patterns for TOSS ERP

### 1. Protected Route with Auth
```vue
<script setup>
definePageMeta({
  middleware: 'auth'
})

const { user } = useAuth()
</script>
```

### 2. Offline-First Data Fetching
```vue
<script setup>
const { data, pending, error, refresh } = await useFetch('/api/products', {
  // Cache for 5 minutes
  getCachedData: (key) => {
    return nuxtApp.static.data[key] || nuxtApp.payload.data[key]
  }
})
</script>
```

### 3. Multi-Language Form
```vue
<script setup>
const { t } = useI18n()
</script>

<template>
  <FormKit
    type="form"
    :submit-label="t('common.submit')"
    @submit="handleSubmit"
  >
    <FormKit
      type="text"
      name="name"
      :label="t('inventory.productName')"
      validation="required"
    />
  </FormKit>
</template>
```

### 4. Responsive Image Gallery
```vue
<template>
  <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
    <NuxtImg
      v-for="product in products"
      :key="product.id"
      :src="product.image"
      :alt="product.name"
      class="rounded-lg"
      sizes="sm:100vw md:33vw"
      quality="80"
    />
  </div>
</template>
```

---

## Troubleshooting

### Module Not Working?

1. **Clear .nuxt directory**:
```bash
rm -rf .nuxt
pnpm run dev
```

2. **Reinstall dependencies**:
```bash
rm -rf node_modules pnpm-lock.yaml
pnpm install
```

3. **Check module configuration** in `nuxt.config.ts`

### Performance Issues?

1. Use **Nuxt DevTools** (included):
   - Press Shift+Alt+D to open
   - Check bundle size
   - Analyze component performance

2. Monitor **Web Vitals**:
   - Check browser console for metrics
   - Use Lighthouse for auditing

---

## Next Steps

1. **Explore Nuxt DevTools**: Built-in visual tools for debugging
2. **Read Module Docs**: Visit https://nuxt.com/modules for detailed documentation
3. **Customize Themes**: Update FormKit theme in `formkit.theme.ts`
4. **Add More Icons**: Browse https://icones.js.org for available icons
5. **Configure Analytics**: Set up Google Analytics with your tracking ID

---

## Resources

- [Nuxt 3 Documentation](https://nuxt.com/docs)
- [Nuxt Modules](https://nuxt.com/modules)
- [Tailwind CSS](https://tailwindcss.com)
- [FormKit](https://formkit.com)
- [VueUse](https://vueuse.org)
- [Iconify](https://iconify.design)

---

**Last Updated**: November 10, 2025
