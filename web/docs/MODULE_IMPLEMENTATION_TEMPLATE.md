# TOSS ERP Module Implementation Template

This document serves as a template for implementing any ERP module in TOSS ERP, based on the patterns established in the Sales module implementation.

## Module Structure

```
app/pages/{module}/
├── index.vue                    # Module Dashboard
├── {entity}/
│   ├── index.vue               # List all entities
│   ├── create.vue              # Create new entity
│   └── [id].vue                # View/edit entity
└── {sub-module}/               # Sub-modules if needed
```

## Implementation Checklist

### Phase 1: Setup
- [ ] Install required shadcn-vue components
- [ ] Install additional dependencies (@tanstack/vue-table, vee-validate, etc.)
- [ ] Create module directory structure
- [ ] Review ERPNext module documentation

### Phase 2: Dashboard
- [ ] Create dashboard page (`index.vue`)
- [ ] Add key metrics cards
- [ ] Add charts/visualizations
- [ ] Add recent activity tables
- [ ] Add quick action buttons

### Phase 3: List Pages
- [ ] Create list page with data table
- [ ] Add search functionality
- [ ] Add filters
- [ ] Add sorting
- [ ] Add pagination
- [ ] Add action buttons (View, Edit, Delete)

### Phase 4: Create/Edit Forms
- [ ] Create form page
- [ ] Add form fields
- [ ] Add validation with vee-validate
- [ ] Add save functionality
- [ ] Add draft/save as draft
- [ ] Add real-time calculations if needed

### Phase 5: View Pages
- [ ] Create view page
- [ ] Display all entity details
- [ ] Add action buttons (Edit, Print, etc.)
- [ ] Add status workflow actions

### Phase 6: Documentation
- [ ] Create module implementation guide
- [ ] Document data structures
- [ ] Document workflows
- [ ] Document API endpoints (future)

## Standard Patterns

### Dashboard Pattern
```vue
<script setup lang="ts">
useHead({
  title: 'Module Name - TOSS ERP'
})

// Metrics data
const metrics = ref({
  // Define metrics
})

// Recent items
const recentItems = ref([
  // Mock data
])
</script>

<template>
  <div class="space-y-6">
    <!-- Header -->
    <!-- Metrics Cards -->
    <!-- Charts -->
    <!-- Recent Activity -->
  </div>
</template>
```

### List Page Pattern
```vue
<script setup lang="ts">
import { useVueTable } from '@tanstack/vue-table'

// Data
const items = ref([])
const searchQuery = ref('')

// Table setup
const columns: ColumnDef[] = [
  // Column definitions
]

const table = useVueTable({
  data: items,
  columns,
  // ... config
})
</script>

<template>
  <div class="space-y-6">
    <!-- Header with create button -->
    <!-- Stats cards -->
    <!-- Filters and search -->
    <!-- Data table -->
    <!-- Pagination -->
  </div>
</template>
```

### Form Pattern
```vue
<script setup lang="ts">
import { useForm } from 'vee-validate'
import { z } from 'zod'

const schema = z.object({
  // Form schema
})

const { handleSubmit } = useForm({ validationSchema: schema })

const onSubmit = handleSubmit((values) => {
  // Submit logic
})
</script>

<template>
  <form @submit="onSubmit">
    <!-- Form fields -->
    <!-- Action buttons -->
  </form>
</template>
```

## Component Library

### Standard Components
- **Card** - Container for content
- **Table** - Data tables with sorting/filtering
- **Dialog** - Modals for forms/confirmations
- **Form** - Form components with validation
- **Select** - Dropdown selects
- **Input** - Text inputs
- **Button** - Action buttons
- **Badge** - Status indicators
- **Label** - Form labels

### Status Badge Colors
- Draft/Pending: `secondary` (gray)
- Active/Sent: `default` (blue)
- Completed/Approved: `default` (green)
- Rejected/Cancelled: `destructive` (red)
- Overdue: `destructive` (orange)

## Data Patterns

### TypeScript Interfaces
```typescript
interface Entity {
  id: string
  name: string
  status: string
  created_at: Date
  updated_at: Date
}
```

### Reactive Data
```typescript
const items = ref<Entity[]>([])
const loading = ref(false)
const error = ref<string | null>(null)
```

### Computed Properties
```typescript
const filteredItems = computed(() => {
  return items.value.filter(/* filter logic */)
})
```

## API Integration Pattern (Future)

```typescript
// Using Nuxt's useFetch
const { data, pending, error } = await useFetch('/api/module/items')

// Using useAsyncData
const { data, refresh } = await useAsyncData('items', () => 
  $fetch('/api/module/items')
)
```

## Styling Guidelines

1. Use Tailwind CSS utility classes
2. Follow Material Dashboard color scheme
3. Maintain consistent spacing (space-y-6 for sections)
4. Use responsive grid (grid-cols-1 md:grid-cols-2 lg:grid-cols-4)
5. Dark mode support (use CSS variables)

## Testing Checklist

- [ ] Dashboard loads correctly
- [ ] Metrics display accurate data
- [ ] List page shows all items
- [ ] Search works
- [ ] Filters work
- [ ] Sorting works
- [ ] Pagination works
- [ ] Create form validates correctly
- [ ] Save functionality works
- [ ] View page displays correctly
- [ ] Edit functionality works
- [ ] Mobile responsive

## Documentation Requirements

1. **Implementation Guide** - Technical details, patterns, code examples
2. **Summary Document** - What's done, what's pending
3. **User Guide** (optional) - How to use the module

## Examples

See the Sales module implementation for complete examples:
- Dashboard: `app/pages/sales/index.vue`
- List Page: `app/pages/sales/quotations/index.vue`
- Documentation: `docs/SALES_MODULE_IMPLEMENTATION_GUIDE.md`

---

**Use this template to maintain consistency across all TOSS ERP modules.**

