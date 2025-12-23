<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch, nextTick } from 'vue'
import { useColorMode } from '#imports'

interface Props {
  data: Array<{
    name: string
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
  const grays = isDark
    ? ['rgba(255, 255, 255, 0.9)', 'rgba(255, 255, 255, 0.7)', 'rgba(255, 255, 255, 0.5)', 'rgba(255, 255, 255, 0.3)', 'rgba(255, 255, 255, 0.1)']
    : ['rgba(0, 0, 0, 0.9)', 'rgba(0, 0, 0, 0.7)', 'rgba(0, 0, 0, 0.5)', 'rgba(0, 0, 0, 0.3)', 'rgba(0, 0, 0, 0.1)']
  
  return {
    backgroundColor: grays,
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
  const labels = props.data.map(d => d.name)
  const values = props.data.map(d => d.value)

  chartInstance = new Chart(chartCanvas.value, {
    type: 'bar',
    data: {
      labels,
      datasets: [
        {
          label: 'Sales',
          data: values,
          backgroundColor: colors.backgroundColor,
          borderWidth: 0
        }
      ]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      indexAxis: 'y',
      plugins: {
        legend: {
          display: false
        },
        tooltip: {
          backgroundColor: colorMode.value === 'dark' ? 'rgba(0, 0, 0, 0.8)' : 'rgba(255, 255, 255, 0.95)',
          titleColor: colors.textColor,
          bodyColor: colors.textColor,
          borderColor: colors.textColor,
          borderWidth: 1
        }
      },
      scales: {
        x: {
          beginAtZero: true,
          grid: {
            color: colors.gridColor
          },
          ticks: {
            color: colors.textColor,
            callback: (value: any) => `R${value}`
          }
        },
        y: {
          grid: {
            display: false
          },
          ticks: {
            color: colors.textColor
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
