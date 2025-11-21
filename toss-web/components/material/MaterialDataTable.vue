<script setup lang="ts">
import { computed } from 'vue'
import MaterialCard from './MaterialCard.vue'

interface Column {
  key: string
  label: string
  sortable?: boolean
  align?: 'left' | 'center' | 'right'
  format?: (value: any) => string
}

interface Props {
  columns: Column[]
  data: any[]
  loading?: boolean
  striped?: boolean
  hoverable?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
  striped: true,
  hoverable: true
})

const emit = defineEmits<{
  'row-click': [row: any]
  'sort': [column: Column, direction: 'asc' | 'desc']
}>()

const handleSort = (column: Column) => {
  if (!column.sortable) return
  // Emit sort event - parent should handle sorting logic
  emit('sort', column, 'asc')
}
</script>

<template>
  <MaterialCard variant="elevated" class="overflow-hidden">
    <div class="overflow-x-auto">
      <table class="w-full">
        <thead class="bg-slate-50 dark:bg-slate-800/50 border-b-2 border-slate-200 dark:border-slate-700">
          <tr>
            <th
              v-for="column in columns"
              :key="column.key"
              :class="[
                'px-6 py-4 text-sm font-semibold text-slate-700 dark:text-slate-300',
                column.align === 'center' ? 'text-center' : column.align === 'right' ? 'text-right' : 'text-left',
                column.sortable ? 'cursor-pointer hover:bg-slate-100 dark:hover:bg-slate-700/50 transition-colors' : ''
              ]"
              @click="handleSort(column)"
            >
              <div class="flex items-center gap-2" :class="column.align === 'center' ? 'justify-center' : column.align === 'right' ? 'justify-end' : ''">
                <span>{{ column.label }}</span>
                <svg v-if="column.sortable" class="w-4 h-4 text-slate-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 16V4m0 0L3 8m4-4l4 4m6 0v12m0 0l4-4m-4 4l-4-4"></path>
                </svg>
              </div>
            </th>
          </tr>
        </thead>
        
        <tbody v-if="!loading">
          <tr
            v-for="(row, index) in data"
            :key="index"
            :class="[
              'border-b border-slate-200 dark:border-slate-700 transition-colors',
              striped && index % 2 === 1 ? 'bg-slate-50/50 dark:bg-slate-800/30' : '',
              hoverable ? 'hover:bg-slate-100 dark:hover:bg-slate-700/50 cursor-pointer' : ''
            ]"
            @click="emit('row-click', row)"
          >
            <td
              v-for="column in columns"
              :key="column.key"
              :class="[
                'px-6 py-4 text-sm text-slate-600 dark:text-slate-300',
                column.align === 'center' ? 'text-center' : column.align === 'right' ? 'text-right' : 'text-left'
              ]"
            >
              {{ column.format ? column.format(row[column.key]) : row[column.key] }}
            </td>
          </tr>
          
          <!-- Empty state -->
          <tr v-if="data.length === 0">
            <td :colspan="columns.length" class="px-6 py-12 text-center text-slate-500 dark:text-slate-400">
              <svg class="w-12 h-12 mx-auto mb-4 text-slate-300 dark:text-slate-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4"></path>
              </svg>
              <p class="text-lg font-medium">No data available</p>
            </td>
          </tr>
        </tbody>
        
        <!-- Loading state -->
        <tbody v-else>
          <tr v-for="i in 5" :key="i" class="border-b border-slate-200 dark:border-slate-700">
            <td v-for="column in columns" :key="column.key" class="px-6 py-4">
              <div class="h-4 bg-slate-200 dark:bg-slate-700 rounded animate-pulse"></div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </MaterialCard>
</template>
