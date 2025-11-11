# TOSS ERP III - shadcn-vue Redesign

## 🎨 Design System Update

This application has been redesigned using **shadcn-vue**, a modern component library built on Radix Vue and Tailwind CSS v4.

## ✅ What's Been Installed

### Core Dependencies
- ✅ `shadcn-nuxt` - Nuxt module for shadcn-vue
- ✅ `@tailwindcss/vite` - Tailwind CSS v4 with Vite
- ✅ `radix-vue` - Headless UI components
- ✅ `class-variance-authority` - CVA for component variants
- ✅ `clsx` & `tailwind-merge` - Class name utilities

### Configuration Files
- ✅ `components.json` - shadcn-vue configuration
- ✅ `lib/utils.ts` - Utility functions (`cn()`)
- ✅ `plugins/ssr-width.ts` - SSR width provider for hydration
- ✅ `.cursor/rules/shadcn-vue.mdc` - AI coding assistant rules

### UI Components Created
Located in `components/ui/`:
- ✅ `button/` - Button component with variants
- ✅ `card/` - Card components (Card, CardHeader, CardTitle, CardDescription, CardContent, CardFooter)

## 🎯 Design System Features

### Theme System
All components use CSS variables for theming (defined in `assets/css/main.css`):

```css
--background / --foreground
--primary / --primary-foreground
--secondary / --secondary-foreground
--muted / --muted-foreground
--accent / --accent-foreground
--destructive / --destructive-foreground
--card / --card-foreground
--border / --input
--ring
```

### Dark Mode Support
Built-in dark mode support through CSS variables. Toggle with Nuxt's color mode module.

### Component Variants
Components support multiple variants and sizes:

**Button Variants:**
- `default` - Primary blue button
- `secondary` - Gray button
- `destructive` - Red button for dangerous actions
- `outline` - Outlined button
- `ghost` - Transparent button
- `link` - Link-styled button

**Button Sizes:**
- `sm` - Small (h-8)
- `default` - Default (h-9)
- `lg` - Large (h-10)
- `icon` - Icon only (h-9 w-9)

## 📦 Adding More Components

To add additional shadcn-vue components:

```bash
pnpm dlx shadcn-vue@latest add [component-name]
```

### Recommended Components to Add:
```bash
# Forms
pnpm dlx shadcn-vue@latest add input label select checkbox radio-group switch textarea

# Overlays
pnpm dlx shadcn-vue@latest add dialog sheet alert-dialog popover dropdown-menu

# Data Display
pnpm dlx shadcn-vue@latest add table badge avatar separator tabs

# Navigation
pnpm dlx shadcn-vue@latest add breadcrumb pagination

# Feedback
pnpm dlx shadcn-vue@latest add alert toast progress skeleton

# Advanced
pnpm dlx shadcn-vue@latest add command calendar date-picker
```

## 🚀 Usage Examples

### Basic Button
```vue
<script setup lang="ts">
import { Button } from '~/components/ui/button'
</script>

<template>
  <Button>Click me</Button>
  <Button variant="outline">Outline</Button>
  <Button variant="destructive">Delete</Button>
</template>
```

### Card Component
```vue
<script setup lang="ts">
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
</script>

<template>
  <Card>
    <CardHeader>
      <CardTitle>Card Title</CardTitle>
      <CardDescription>Card description goes here</CardDescription>
    </CardHeader>
    <CardContent>
      <p>Card content</p>
    </CardContent>
  </Card>
</template>
```

### Dashboard Grid
```vue
<template>
  <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
    <Card>
      <CardHeader>
        <CardTitle>Total Revenue</CardTitle>
      </CardHeader>
      <CardContent>
        <div class="text-2xl font-bold">R45,231.89</div>
      </CardContent>
    </Card>
  </div>
</template>
```

## 🎨 Styling Guidelines

### Use the `cn()` Utility
```vue
<script setup lang="ts">
import { cn } from '~/lib/utils'

const props = defineProps<{
  variant?: 'default' | 'error'
}>()
</script>

<template>
  <div :class="cn(
    'base-class',
    variant === 'error' && 'text-red-600'
  )">
    Content
  </div>
</template>
```

### Theme Colors
Use semantic color classes:
- `bg-background` / `text-foreground` - Base colors
- `bg-primary` / `text-primary-foreground` - Primary actions
- `bg-secondary` / `text-secondary-foreground` - Secondary actions
- `bg-muted` / `text-muted-foreground` - Muted/disabled states
- `bg-destructive` / `text-destructive-foreground` - Destructive actions
- `border-border` - Borders
- `ring-ring` - Focus rings

## 📁 Project Structure

```
toss-web/
├── components/
│   ├── ui/                      # shadcn-vue components
│   │   ├── button/
│   │   │   ├── Button.vue
│   │   │   └── index.ts
│   │   └── card/
│   │       ├── Card.vue
│   │       ├── CardHeader.vue
│   │       ├── CardTitle.vue
│   │       ├── CardDescription.vue
│   │       ├── CardContent.vue
│   │       ├── CardFooter.vue
│   │       └── index.ts
│   └── ...                      # Other custom components
├── lib/
│   └── utils.ts                 # Utility functions
├── assets/
│   └── css/
│       └── main.css             # Tailwind + Theme variables
├── plugins/
│   └── ssr-width.ts             # SSR width provider
├── .cursor/
│   └── rules/
│       ├── nuxt.mdc
│       └── shadcn-vue.mdc       # shadcn-vue AI rules
└── components.json              # shadcn-vue config
```

## 🔧 Configuration

### nuxt.config.ts
```typescript
import tailwindcss from '@tailwindcss/vite'

export default defineNuxtConfig({
  modules: [
    'shadcn-nuxt',
    '@nuxtjs/tailwindcss',
    // ... other modules
  ],
  
  shadcn: {
    prefix: '',
    componentDir: './components/ui'
  },
  
  vite: {
    plugins: [tailwindcss()],
  },
})
```

### components.json
```json
{
  "style": "new-york",
  "tailwind": {
    "config": "tailwind.config.js",
    "css": "assets/css/main.css",
    "baseColor": "slate",
    "cssVariables": true
  },
  "aliases": {
    "components": "~/components",
    "utils": "~/lib/utils"
  }
}
```

## 🎯 Migration Guide

### Before (Custom Components)
```vue
<button class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded">
  Click me
</button>
```

### After (shadcn-vue)
```vue
<Button>Click me</Button>
```

## 📝 Cursor AI Rules

The AI assistant has been configured to:
- ✅ Always use shadcn-vue components instead of custom UI
- ✅ Follow shadcn-vue patterns and best practices
- ✅ Use proper component imports and variants
- ✅ Apply consistent theming with CSS variables
- ✅ Use the `cn()` utility for conditional styling

See `.cursor/rules/shadcn-vue.mdc` for complete AI guidelines.

## 🌐 Demo Page

Visit `/shadcn-demo` to see a comprehensive demonstration of shadcn-vue components in action.

## 📚 Resources

- [shadcn-vue Documentation](https://www.shadcn-vue.com)
- [Radix Vue](https://www.radix-vue.com)
- [Tailwind CSS v4](https://tailwindcss.com)
- [Component Examples](https://www.shadcn-vue.com/examples)

## ✨ Next Steps

1. **Add more components**: Install additional shadcn-vue components as needed
2. **Update existing pages**: Migrate existing custom components to shadcn-vue
3. **Customize theme**: Adjust CSS variables in `assets/css/main.css`
4. **Create composed components**: Build complex UIs from shadcn-vue primitives
5. **Test dark mode**: Ensure components work well in both light and dark modes

## 🎉 Benefits

- ✅ **Consistent Design**: All components follow the same design language
- ✅ **Accessibility**: Built on Radix Vue with ARIA support
- ✅ **Customizable**: Components are in your codebase - modify as needed
- ✅ **Type-Safe**: Full TypeScript support
- ✅ **Modern**: Uses latest Vue 3, Tailwind CSS v4, and Nuxt 4 features
- ✅ **Dark Mode**: Built-in dark mode support
- ✅ **AI-Friendly**: Cursor AI rules ensure consistent usage

---

**Note**: This redesign maintains all existing functionality while providing a modern, accessible, and consistent UI foundation.
