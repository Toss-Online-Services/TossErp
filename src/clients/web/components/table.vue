<template>
  <div class="bg-white shadow rounded-lg border border-gray-200">
    <!-- Table Header -->
    <div v-if="title || $slots.header" class="px-6 py-4 border-b border-gray-200">
      <slot name="header">
        <div class="flex items-center justify-between">
          <div>
            <h3 v-if="title" class="text-lg font-medium text-gray-900">{{ title }}</h3>
            <p v-if="subtitle" class="text-sm text-gray-500 mt-1">{{ subtitle }}</p>
          </div>
          <div v-if="$slots.headerActions" class="flex items-center space-x-2">
            <slot name="headerActions" />
          </div>
        </div>
      </slot>
    </div>

    <!-- Table Content -->
    <div class="overflow-x-auto">
      <table class="min-w-full divide-y divide-gray-200">
        <thead class="bg-gray-50">
          <tr>
            <th 
              v-for="column in columns" 
              :key="column.key"
              class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
              :class="column.class"
            >
              {{ column.label }}
            </th>
          </tr>
        </thead>
        <tbody class="bg-white divide-y divide-gray-200">
          <tr v-for="(row, index) in data" :key="row.id || index" class="hover:bg-gray-50">
            <td 
              v-for="column in columns" 
              :key="column.key"
              class="px-6 py-4 whitespace-nowrap text-sm text-gray-900"
              :class="column.class"
            >
              <slot :name="`cell-${column.key}`" :row="row" :column="column" :value="row[column.key]">
                {{ formatCellValue(row[column.key], column) }}
              </slot>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Empty State -->
    <div v-if="!data || data.length === 0" class="px-6 py-12 text-center">
      <slot name="empty">
        <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
        </svg>
        <h3 class="mt-2 text-sm font-medium text-gray-900">No data available</h3>
        <p class="mt-1 text-sm text-gray-500">Get started by creating a new record.</p>
      </slot>
    </div>

    <!-- Table Footer -->
    <div v-if="$slots.footer" class="px-6 py-4 border-t border-gray-200 bg-gray-50 rounded-b-lg">
      <slot name="footer" />
    </div>
  </div>
</template>

<script setup lang="ts">
interface TableColumn {
  key: string
  label: string
  class?: string
  formatter?: (value: any) => string
}

interface Props {
  title?: string
  subtitle?: string
  columns: TableColumn[]
  data: any[]
}

const props = defineProps<Props>()

const formatCellValue = (value: any, column: TableColumn): string => {
  if (column.formatter) {
    return column.formatter(value)
  }
  
  if (value === null || value === undefined) {
    return '-'
  }
  
  if (typeof value === 'boolean') {
    return value ? 'Yes' : 'No'
  }
  
  if (typeof value === 'number') {
    return value.toLocaleString()
  }
  
  return String(value)
}
</script>

<style scoped>
/* Component-specific styles */
</style>
