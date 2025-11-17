# TOSS ERP - shadcn-vue Complete Redesign

## ✅ Completed Steps

### 1. Cleanup & Component Installation
- ✅ Backed up all old components to `backup-old-code/components/`
- ✅ Backed up all old pages to `backup-old-code/pages/`
- ✅ Removed all custom components except `components/ui/`
- ✅ Cleared all pages directory
- ✅ Fixed tailwindcss installation issue
- ✅ Fixed app.vue ToastContainer import error

### 2. shadcn-vue Component Library Installation
All components installed successfully using `npx shadcn-vue@latest add`:

**Form Components:**
- ✅ Input
- ✅ Label  
- ✅ Textarea
- ✅ Select (with all sub-components)
- ✅ Checkbox
- ✅ Radio Group
- ✅ Switch
- ✅ Form (with all form utilities)

**Layout Components:**
- ✅ Card (already had, updated)
- ✅ Separator
- ✅ Tabs
- ✅ Accordion
- ✅ Collapsible
- ✅ Aspect Ratio
- ✅ Scroll Area

**Feedback Components:**
- ✅ Dialog
- ✅ Sheet
- ✅ Alert
- ✅ Alert Dialog
- ✅ Sonner (toast notifications)
- ✅ Progress
- ✅ Skeleton

**Navigation Components:**
- ✅ Breadcrumb
- ✅ Dropdown Menu
- ✅ Command

**Data Display Components:**
- ✅ Table
- ✅ Badge
- ✅ Avatar
- ✅ Pagination

**Overlay Components:**
- ✅ Popover
- ✅ Tooltip
- ✅ Hover Card

**Interactive Components:**
- ✅ Button (already had, updated)
- ✅ Calendar
- ✅ Command

### 3. Updated Cursor Rules
- ✅ Updated `.cursor/rules/shadcn-vue.mdc` with MANDATORY CLI installation requirement
- ✅ Added clear instructions to ALWAYS use `npx shadcn-vue@latest add` for new components
- ✅ Added warning against manual component creation

## 📋 Implementation Progress

### ✅ Phase 2: Pages Created (COMPLETED)
- ✅ Created index page (landing page with features)
- ✅ Created login page (`auth/login.vue`)
- ✅ Created register page (`auth/register.vue`)
- ✅ Created dashboard page (`dashboard/index.vue`)
- ✅ Created POS page (`sales/pos.vue`)
- ✅ Created inventory page (`stock/index.vue`)
- ✅ Created customers page (`crm/customers.vue`)
- ✅ Created settings page (`settings/index.vue`)

### 📋 Next Steps - Additional Pages to Create

### Phase 3: Complete Sales Module
- [ ] Create sales orders list page
- [ ] Create invoices page
- [ ] Create quotations page
- [ ] Create sales reports page

### Phase 4: Complete Inventory Module
- [ ] Create product details/edit page
- [ ] Create stock adjustments page
- [ ] Create inventory reports page
- [ ] Create categories management page

### Phase 5: Complete CRM Module
- [ ] Create customer details page
- [ ] Create contacts management page
- [ ] Create CRM dashboard page
- [ ] Create customer interactions/history page

### Phase 6: Buying Module
- [ ] Create suppliers list page
- [ ] Create supplier details page
- [ ] Create purchase orders page
- [ ] Create supplier products page

### Phase 7: Logistics Module
- [ ] Create deliveries list page
- [ ] Create delivery tracking page
- [ ] Create routes management page
- [ ] Create drivers management page

### Phase 8: Additional Features
- [ ] Create layouts (default, auth, landing)
- [ ] Create navigation components
- [ ] Add charts/graphs to dashboard
- [ ] Implement middleware for protected routes
- [ ] Add user profile/menu dropdown
- [ ] Create forgot password page
- [ ] Create reports module

## 🎨 Design Principles

### Use shadcn-vue Components for Everything
- **Buttons**: Use `<Button>` with variants (default, secondary, destructive, outline, ghost, link)
- **Forms**: Use `<Input>`, `<Label>`, `<Select>`, `<Textarea>`, `<Checkbox>`, `<Switch>`
- **Cards**: Use `<Card>` with `<CardHeader>`, `<CardTitle>`, `<CardDescription>`, `<CardContent>`, `<CardFooter>`
- **Dialogs**: Use `<Dialog>` for modals, `<Sheet>` for slide-outs
- **Tables**: Use `<Table>` components for all data tables
- **Navigation**: Use `<Breadcrumb>`, `<Tabs>`, `<DropdownMenu>`

### Responsive Design
- Mobile-first approach
- Use Tailwind responsive prefixes (sm:, md:, lg:, xl:)
- Ensure all touch targets are minimum 44x44px
- Test on mobile, tablet, and desktop

### Accessibility
- Proper ARIA labels
- Keyboard navigation support
- Focus management
- Screen reader friendly

### Theme Consistency
- Use CSS variables from `assets/css/main.css`
- Stick to color palette: primary, secondary, accent, destructive, muted
- Use consistent spacing (4px grid system)

## 📝 Component Usage Examples

### Example Page Structure
```vue
<script setup lang="ts">
import { Button } from '~/components/ui/button'
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from '~/components/ui/card'
import { Input } from '~/components/ui/input'
import { Label } from '~/components/ui/label'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '~/components/ui/table'

// Page logic here
</script>

<template>
  <div class="container mx-auto p-6">
    <!-- Page Header -->
    <div class="mb-6">
      <h1 class="text-3xl font-bold">Page Title</h1>
      <p class="text-muted-foreground">Page description</p>
    </div>
    
    <!-- Actions Bar -->
    <div class="mb-6 flex items-center justify-between">
      <div class="flex gap-2">
        <Button variant="outline">Filter</Button>
        <Button variant="outline">Sort</Button>
      </div>
      <Button>Add New</Button>
    </div>
    
    <!-- Content Card -->
    <Card>
      <CardHeader>
        <CardTitle>Card Title</CardTitle>
        <CardDescription>Card description</CardDescription>
      </CardHeader>
      <CardContent>
        <!-- Content here -->
      </CardContent>
    </Card>
  </div>
</template>
```

## 🔧 Configuration Files

### components.json
Location: `toss-web/components.json`
- Style: "new-york"
- Base Color: "slate"
- CSS Variables: true

### Tailwind Config
Location: `toss-web/tailwind.config.js`
- Using Tailwind CSS v4
- CSS variables enabled

### Theme Variables
Location: `toss-web/assets/css/main.css`
- Light and dark mode variables defined
- Custom TOSS brand colors included

## 📦 Installed Packages
- shadcn-nuxt: 2.3.2
- radix-vue: 1.9.17
- tailwindcss: 4.1.17
- @tailwindcss/vite: 4.1.17
- clsx: 2.1.1
- tailwind-merge: 3.4.0
- class-variance-authority: 0.7.1
- @vueuse/core: (already installed)
- lucide-vue-next: (installed)
- vaul-vue: (installed)
- embla-carousel-vue: (installed)

## 🚀 Development Commands

```bash
# Start dev server
pnpm dev

# Add new shadcn-vue component
npx shadcn-vue@latest add [component-name]

# Build for production
pnpm build

# Run tests
pnpm test
```

## 📚 Resources
- [shadcn-vue Documentation](https://www.shadcn-vue.com)
- [shadcn-vue Components](https://www.shadcn-vue.com/docs/components)
- [shadcn-vue Blocks](https://www.shadcn-vue.com/blocks)
- [shadcn-vue Examples](https://www.shadcn-vue.com/examples)
- [Radix Vue](https://www.radix-vue.com)
- [Tailwind CSS](https://tailwindcss.com)
