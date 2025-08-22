<template>
  <div class="chart-container">
    <canvas ref="chartCanvas" :id="chartId"></canvas>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch, computed } from 'vue'
import { Chart, registerables } from 'chart.js'
import type { ChartConfiguration } from 'chart.js'
import { useStockStore } from '../../stores/stock'

// Register Chart.js components
Chart.register(...registerables)

interface Props {
  chartId?: string
  height?: number
}

const props = withDefaults(defineProps<Props>(), {
  chartId: 'category-distribution-chart',
  height: 300
})

const stockStore = useStockStore()
const chartCanvas = ref<HTMLCanvasElement>()
let chart: Chart | null = null

// Generate chart data from store
const generateChartData = computed(() => {
  const categoryData = stockStore.items.reduce((acc, item) => {
    const category = item.category
    if (!acc[category]) {
      acc[category] = {
        count: 0,
        value: 0
      }
    }
    acc[category].count++
    acc[category].value += item.quantity * item.unitCost
    return acc
  }, {} as Record<string, { count: number; value: number }>)

  const labels = Object.keys(categoryData)
  const data = labels.map(category => categoryData[category].count)
  const values = labels.map(category => categoryData[category].value)

  // Generate colors for each category
  const colors = [
    'rgb(59, 130, 246)',   // Blue
    'rgb(16, 185, 129)',   // Green
    'rgb(245, 158, 11)',   // Yellow
    'rgb(239, 68, 68)',    // Red
    'rgb(139, 92, 246)',   // Purple
    'rgb(236, 72, 153)',   // Pink
    'rgb(14, 165, 233)',   // Sky
    'rgb(34, 197, 94)',    // Emerald
  ]

  return {
    labels,
    datasets: [
      {
        data,
        backgroundColor: colors.slice(0, labels.length),
        borderColor: colors.slice(0, labels.length).map(color => color.replace('rgb', 'rgba').replace(')', ', 0.8)')),
        borderWidth: 2,
        hoverOffset: 4
      }
    ],
    values // Additional data for tooltips
  }
})

const chartConfig: ChartConfiguration<'pie'> = {
  type: 'pie',
  data: generateChartData.value,
  options: {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'right' as const,
        labels: {
          usePointStyle: true,
          padding: 20,
          color: '#6B7280',
          generateLabels: (chart) => {
            const data = chart.data
            if (data.labels && data.datasets[0].data) {
              return data.labels.map((label, i) => {
                const value = data.datasets[0].data[i] as number
                const totalValue = (data.datasets[0].data as number[]).reduce((a, b) => a + b, 0)
                const percentage = ((value / totalValue) * 100).toFixed(1)
                return {
                  text: `${label} (${percentage}%)`,
                  fillStyle: (data.datasets[0].backgroundColor as string[])?.[i] as string,
                  strokeStyle: (data.datasets[0].borderColor as string[])?.[i] as string,
                  lineWidth: 0,
                  pointStyle: 'circle',
                  hidden: false,
                  index: i
                }
              })
            }
            return []
          }
        }
      },
      tooltip: {
        callbacks: {
          label: (context) => {
            const label = context.label || ''
            const value = context.parsed
            const total = context.dataset.data.reduce((a: number, b: number) => a + b, 0)
            const percentage = ((value / total) * 100).toFixed(1)
            return `${label}: ${value} items (${percentage}%)`
          }
        },
        backgroundColor: 'rgba(0, 0, 0, 0.8)',
        titleColor: '#FFFFFF',
        bodyColor: '#FFFFFF',
        borderColor: '#374151',
        borderWidth: 1
      }
    }
  }
}

const createChart = () => {
  if (!chartCanvas.value) return
  
  const ctx = chartCanvas.value.getContext('2d')
  if (!ctx) return
  
  chart = new Chart(ctx, chartConfig)
}

const destroyChart = () => {
  if (chart) {
    chart.destroy()
    chart = null
  }
}

const updateChart = () => {
  if (chart) {
    chart.data = generateChartData.value
    chart.update('none')
  }
}

onMounted(() => {
  createChart()
})

onUnmounted(() => {
  destroyChart()
})

// Watch for store changes to update chart
watch(() => stockStore.items, () => {
  updateChart()
}, { deep: true })
</script>

<style scoped>
.chart-container {
  position: relative;
  height: v-bind(height + 'px');
  width: 100%;
}
</style>
