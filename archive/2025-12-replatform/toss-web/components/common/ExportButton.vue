<template>
  <div class="relative">
    <button
      @click="showDropdown = !showDropdown"
      :disabled="loading || !data || data.length === 0"
      class="inline-flex items-center gap-2 px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
    >
      <svg v-if="!loading" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 10v6m0 0l-3-3m3 3l3-3m2 8H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
      </svg>
      <svg v-else class="w-5 h-5 animate-spin" fill="none" viewBox="0 0 24 24">
        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
        <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
      </svg>
      {{ loading ? 'Exporting...' : 'Export' }}
      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"/>
      </svg>
    </button>

    <!-- Dropdown Menu -->
    <div
      v-if="showDropdown"
      v-click-outside="() => showDropdown = false"
      class="absolute right-0 mt-2 w-48 bg-white dark:bg-slate-800 rounded-lg shadow-lg border border-slate-200 dark:border-slate-700 z-50"
    >
      <div class="py-1">
        <button
          @click="handleExport('csv')"
          class="flex items-center gap-3 w-full px-4 py-2 text-sm text-slate-700 dark:text-slate-300 hover:bg-slate-100 dark:hover:bg-slate-700 transition-colors"
        >
          <svg class="w-4 h-4 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
          </svg>
          Export as CSV
        </button>
        <button
          @click="handleExport('excel')"
          class="flex items-center gap-3 w-full px-4 py-2 text-sm text-slate-700 dark:text-slate-300 hover:bg-slate-100 dark:hover:bg-slate-700 transition-colors"
        >
          <svg class="w-4 h-4 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2H5a2 2 0 00-2-2z"/>
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 5v4m4-4v4m4-4v4"/>
          </svg>
          Export as Excel
        </button>
        <button
          @click="handleExport('pdf')"
          class="flex items-center gap-3 w-full px-4 py-2 text-sm text-slate-700 dark:text-slate-300 hover:bg-slate-100 dark:hover:bg-slate-700 transition-colors"
        >
          <svg class="w-4 h-4 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 21h10a2 2 0 002-2V9.414a1 1 0 00-.293-.707l-5.414-5.414A1 1 0 0012.586 3H7a2 2 0 00-2 2v14a2 2 0 002 2z"/>
          </svg>
          Export as PDF
        </button>
        <div v-if="showChartExport" class="border-t border-slate-200 dark:border-slate-700 mt-1 pt-1">
          <button
            @click="handleChartExport"
            class="flex items-center gap-3 w-full px-4 py-2 text-sm text-slate-700 dark:text-slate-300 hover:bg-slate-100 dark:hover:bg-slate-700 transition-colors"
          >
            <svg class="w-4 h-4 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"/>
            </svg>
            Export Chart as Image
          </button>
        </div>
        <div v-if="showDashboardExport" class="border-t border-slate-200 dark:border-slate-700 mt-1 pt-1">
          <button
            @click="handleDashboardExport"
            class="flex items-center gap-3 w-full px-4 py-2 text-sm text-slate-700 dark:text-slate-300 hover:bg-slate-100 dark:hover:bg-slate-700 transition-colors"
          >
            <svg class="w-4 h-4 text-purple-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 17V7m0 10a2 2 0 01-2 2H5a2 2 0 01-2-2V7a2 2 0 012-2h2a2 2 0 012 2m0 10a2 2 0 002 2h2a2 2 0 002-2M9 7a2 2 0 012-2h2a2 2 0 012 2m0 10V7m0 10a2 2 0 002 2h2a2 2 0 002-2V7a2 2 0 00-2-2h-2a2 2 0 00-2 2"/>
            </svg>
            Export Dashboard as PDF
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import type { ExportOptions } from '~/composables/useExport'

interface Props {
  data: any[]
  filename?: string
  title?: string
  subtitle?: string
  dataType?: string
  showChartExport?: boolean
  showDashboardExport?: boolean
  chartElementId?: string
  dashboardElementId?: string
  customColumns?: any[]
}

const props = withDefaults(defineProps<Props>(), {
  filename: 'export',
  dataType: 'generic',
  showChartExport: false,
  showDashboardExport: false
})

const emit = defineEmits<{
  exportStart: []
  exportComplete: [format: string]
  exportError: [error: string]
}>()

const { 
  exportToCSV, 
  exportToExcel, 
  exportToPDF, 
  exportChartAsImage, 
  exportDashboardToPDF,
  getExportConfig 
} = useExport()

const showDropdown = ref(false)
const loading = ref(false)

// Click outside directive
const vClickOutside = {
  beforeMount(el: HTMLElement, binding: any) {
    el.clickOutsideEvent = (event: Event) => {
      if (!(el === event.target || el.contains(event.target as Node))) {
        binding.value()
      }
    }
    document.addEventListener('click', el.clickOutsideEvent)
  },
  unmounted(el: HTMLElement) {
    document.removeEventListener('click', el.clickOutsideEvent)
  }
}

const handleExport = async (format: 'csv' | 'excel' | 'pdf') => {
  try {
    loading.value = true
    showDropdown.value = false
    emit('exportStart')

    const options: ExportOptions = {
      filename: props.filename,
      title: props.title,
      subtitle: props.subtitle,
      includeTimestamp: true
    }

    // Use custom columns if provided, otherwise get predefined config
    if (props.customColumns) {
      options.columns = props.customColumns
    } else if (props.dataType !== 'generic') {
      const config = getExportConfig(props.dataType)
      options.columns = config.columns
    }

    switch (format) {
      case 'csv':
        await exportToCSV(props.data, options)
        break
      case 'excel':
        await exportToExcel(props.data, options)
        break
      case 'pdf':
        await exportToPDF(props.data, options)
        break
    }

    emit('exportComplete', format)
  } catch (error) {
    console.error('Export error:', error)
    emit('exportError', error instanceof Error ? error.message : 'Export failed')
  } finally {
    loading.value = false
  }
}

const handleChartExport = async () => {
  try {
    if (!props.chartElementId) {
      throw new Error('Chart element ID not provided')
    }

    loading.value = true
    showDropdown.value = false
    emit('exportStart')

    await exportChartAsImage(props.chartElementId, `${props.filename}_chart`)
    emit('exportComplete', 'chart')
  } catch (error) {
    console.error('Chart export error:', error)
    emit('exportError', error instanceof Error ? error.message : 'Chart export failed')
  } finally {
    loading.value = false
  }
}

const handleDashboardExport = async () => {
  try {
    if (!props.dashboardElementId) {
      throw new Error('Dashboard element ID not provided')
    }

    loading.value = true
    showDropdown.value = false
    emit('exportStart')

    const options: ExportOptions = {
      filename: `${props.filename}_dashboard`,
      title: props.title || 'Dashboard Report',
      includeTimestamp: true,
      orientation: 'landscape'
    }

    await exportDashboardToPDF(props.dashboardElementId, options)
    emit('exportComplete', 'dashboard')
  } catch (error) {
    console.error('Dashboard export error:', error)
    emit('exportError', error instanceof Error ? error.message : 'Dashboard export failed')
  } finally {
    loading.value = false
  }
}
</script>
