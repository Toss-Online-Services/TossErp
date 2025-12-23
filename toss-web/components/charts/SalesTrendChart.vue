<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch, nextTick } from 'vue'
import { useColorMode } from '#imports'

interface Props {
  data: Array<{
    date: string
    value: number
  }>
  loading?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  loading: false
})

const colorMode = useColorMode()
const chartCanvas = ref<HTMLCanvasElement | null>(null)
let chartInstance: any = null
let Chart: any = null

const getChartColors = () => {
  const isDark = colorMode.value === 'dark'
  return {
    borderColor: isDark ? 'rgb(255, 255, 255)' : 'rgb(0, 0, 0)',
    backgroundColor: isDark ? 'rgba(255, 255, 255, 0.1)' : 'rgba(0, 0, 0, 0.05)',
    gridColor: isDark ? 'rgba(255, 255, 255, 0.1)' : 'rgba(0, 0, 0, 0.1)',
    textColor: isDark ? 'rgb(255, 255, 255)' : 'rgb(0, 0, 0)'
  }
}

const renderChart = () => {
  if (!Chart || !chartCanvas.value || props.loading) return

  if (chartInstance) {
    chartInstance.destroy()
  }

  const colors = getChartColors()
  const labels = props.data.map(d => d.date)
  const values = props.data.map(d => d.value)

  chartInstance = new Chart(chartCanvas.value, {
    type: 'line',
    data: {
      labels,
      datasets: [
        {
          label: 'Sales',
          data: values,
          borderColor: colors.borderColor,
          backgroundColor: colors.backgroundColor,
          tension: 0.4,
          fill: true,
          borderWidth: 2
        }
      ]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          display: false
        },
        tooltip: {
          backgroundColor: colorMode.value === 'dark' ? 'rgba(0, 0, 0, 0.8)' : 'rgba(255, 255, 255, 0.95)',
          titleColor: colors.textColor,
          bodyColor: colors.textColor,
          borderColor: colors.borderColor,
          borderWidth: 1
        }
      },
      scales: {
        x: {
          grid: {
            color: colors.gridColor
          },
          ticks: {
            color: colors.textColor
          }
        },
        y: {
          beginAtZero: true,
          grid: {
            color: colors.gridColor
          },
          ticks: {
            color: colors.textColor,
            callback: (value: any) => `R${value}`
          }
        }
      }
    }
  })
}

watch(() => props.data, () => renderChart(), { deep: true })
watch(() => props.loading, () => {
  if (!props.loading) {
    nextTick(() => renderChart())
  }
})
watch(() => colorMode.value, () => renderChart())

onMounted(async () => {
  if (import.meta.client) {
    const chartModule = await import('chart.js')
    Chart = chartModule.Chart
    const registerables = chartModule.registerables
    Chart.register(...registerables)
    nextTick(() => renderChart())
  }
})

onUnmounted(() => {
  if (chartInstance) {
    chartInstance.destroy()
  }
})
</script>

<template>
  <div class="relative h-80">
    <canvas v-if="!loading" ref="chartCanvas" />
    <div v-else class="flex items-center justify-center h-full">
      <div class="w-8 h-8 border-2 border-primary border-t-transparent rounded-full animate-spin" />
    </div>
  </div>
</template>
