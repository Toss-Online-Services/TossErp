# TOSS ERP III Component Library

This directory contains reusable UI components built specifically for the TOSS ERP III platform. All components are built with Nuxt 3, TypeScript, and Tailwind CSS, following modern Vue.js best practices.

## Components Overview

### 1. Card Component (`card.vue`)

A flexible card component that provides consistent styling and layout for content containers.

**Features:**
- Configurable title and subtitle
- Header actions slot
- Footer slot
- Customizable padding and margin
- Responsive design

**Usage:**
```vue
<Card title="Card Title" subtitle="Card subtitle">
  <p>Card content goes here</p>
  
  <template #headerActions>
    <button class="btn">Action</button>
  </template>
  
  <template #footer>
    <div class="flex justify-end space-x-2">
      <button>Cancel</button>
      <button>Save</button>
    </div>
  </template>
</Card>
```

**Props:**
- `title?: string` - Card title
- `subtitle?: string` - Card subtitle
- `padding?: string` - Custom padding classes (default: 'p-6')
- `margin?: string` - Custom margin classes
- `customClass?: string` - Additional CSS classes

**Slots:**
- `default` - Main content
- `header` - Custom header content
- `headerActions` - Actions in the header
- `footer` - Footer content

### 2. Table Component (`table.vue`)

A data table component with built-in formatting, sorting, and customization options.

**Features:**
- Column-based configuration
- Custom cell formatting
- Empty state handling
- Header actions
- Responsive design
- Hover effects

**Usage:**
```vue
<Table 
  title="Data Table" 
  subtitle="Table description"
  :columns="columns"
  :data="tableData"
>
  <template #headerActions>
    <button>Add Item</button>
  </template>
  
  <template #cell-status="{ value }">
    <span :class="getStatusClass(value)">{{ value }}</span>
  </template>
</Table>
```

**Props:**
- `title?: string` - Table title
- `subtitle?: string` - Table subtitle
- `columns: TableColumn[]` - Column definitions
- `data: any[]` - Table data

**Column Configuration:**
```typescript
interface TableColumn {
  key: string          // Data property key
  label: string        // Display label
  class?: string       // CSS classes
  formatter?: (value: any) => string  // Custom formatting function
}
```

**Slots:**
- `header` - Custom header content
- `headerActions` - Actions in the header
- `cell-{columnKey}` - Custom cell content for specific columns
- `empty` - Empty state content
- `footer` - Footer content

### 3. Modal Component (`modal.vue`)

A modal dialog component with multiple sizes and customization options.

**Features:**
- Multiple size options (small, medium, large)
- Backdrop click handling
- Escape key support
- Smooth animations
- Teleport to body
- Customizable header and footer

**Usage:**
```vue
<Modal 
  v-model:is-open="showModal" 
  title="Modal Title"
  size="medium"
>
  <p>Modal content goes here</p>
  
  <template #footer>
    <button @click="showModal = false">Cancel</button>
    <button @click="save">Save</button>
  </template>
</Modal>
```

**Props:**
- `isOpen: boolean` - Controls modal visibility
- `title?: string` - Modal title
- `size?: 'small' | 'medium' | 'large'` - Modal size
- `closeOnBackdrop?: boolean` - Whether clicking backdrop closes modal

**Events:**
- `close` - Emitted when modal is closed
- `update:is-open` - For v-model support

**Slots:**
- `default` - Main content
- `header` - Custom header content
- `footer` - Footer content

### 4. Chart Component (`chart.vue`)

A chart wrapper component that handles loading states, errors, and empty data.

**Features:**
- Loading state management
- Error handling with retry functionality
- Empty state handling
- Configurable height
- Chart options passing

**Usage:**
```vue
<Chart 
  title="Revenue Chart" 
  subtitle="Monthly data"
  :loading="isLoading"
  :error="chartError"
  height="500px"
>
  <canvas ref="chartCanvas"></canvas>
</Chart>
```

**Props:**
- `title?: string` - Chart title
- `subtitle?: string` - Chart subtitle
- `data?: any[]` - Chart data
- `loading?: boolean` - Loading state
- `error?: string` - Error message
- `height?: string` - Chart height (default: '400px')
- `chartOptions?: Record<string, any>` - Chart configuration

**Events:**
- `retry` - Emitted when retry button is clicked

**Slots:**
- `default` - Chart content
- `header` - Custom header content
- `headerActions` - Actions in the header
- `empty` - Empty state content
- `footer` - Footer content

### 5. Notification Component (`notification.vue`)

A toast notification component with multiple types and auto-close functionality.

**Features:**
- Multiple notification types (success, error, warning, info)
- Auto-close functionality
- Custom duration
- Action buttons
- Smooth animations
- Type-specific styling

**Usage:**
```vue
<Notification
  v-model:is-visible="showNotification"
  type="success"
  title="Success!"
  message="Operation completed successfully"
  :duration="5000"
>
  <template #actions>
    <button @click="undo">Undo</button>
    <button @click="showNotification = false">Dismiss</button>
  </template>
</Notification>
```

**Props:**
- `isVisible: boolean` - Controls notification visibility
- `type?: 'success' | 'error' | 'warning' | 'info'` - Notification type
- `title?: string` - Notification title
- `message?: string` - Notification message
- `duration?: number` - Auto-close duration in milliseconds
- `autoClose?: boolean` - Whether to auto-close

**Events:**
- `close` - Emitted when notification is closed
- `update:is-visible` - For v-model support

**Slots:**
- `actions` - Action buttons

## Design System

### Color Palette
- **Primary**: Orange (`orange-600`, `orange-700`)
- **Success**: Green (`green-400`, `green-600`)
- **Error**: Red (`red-400`, `red-600`)
- **Warning**: Yellow (`yellow-400`, `yellow-600`)
- **Info**: Blue (`blue-400`, `blue-600`)
- **Neutral**: Gray scale (`gray-50` to `gray-900`)

### Typography
- **Headings**: `text-lg`, `text-2xl`, `text-3xl` with `font-medium` or `font-bold`
- **Body**: `text-sm`, `text-base` with `text-gray-600` or `text-gray-900`
- **Captions**: `text-xs` with `text-gray-500`

### Spacing
- **Padding**: `p-4`, `p-6` for component content
- **Margins**: `mb-6`, `mb-8`, `mb-12` for section spacing
- **Gaps**: `gap-4`, `gap-6` for grid layouts

### Shadows & Borders
- **Cards**: `shadow` with `border border-gray-200`
- **Modals**: `shadow-xl` with rounded corners
- **Notifications**: `shadow-lg` with ring borders

## Usage Guidelines

### 1. Component Import
All components are auto-imported by Nuxt 3, so no manual imports are needed.

### 2. Responsive Design
All components are built with responsive design in mind using Tailwind's responsive prefixes.

### 3. Accessibility
Components include proper ARIA labels, keyboard navigation, and screen reader support.

### 4. Customization
Use the provided props and slots for customization rather than overriding styles.

### 5. State Management
Components use Vue 3's Composition API with `ref`, `reactive`, and `computed`.

## Examples

See `app/pages/components-demo.vue` for comprehensive examples of all components in action.

## Contributing

When adding new components:

1. Follow the existing naming conventions
2. Use TypeScript interfaces for props
3. Include proper JSDoc comments
4. Add examples to the demo page
5. Update this README
6. Ensure responsive design
7. Include accessibility features

## Dependencies

- **Vue 3**: Composition API and modern Vue features
- **Nuxt 3**: Auto-imports and framework features
- **Tailwind CSS**: Utility-first CSS framework
- **TypeScript**: Type safety and better developer experience
