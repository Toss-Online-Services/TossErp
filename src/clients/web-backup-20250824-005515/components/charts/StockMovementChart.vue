<template>
  <div class="chart-container">
    <canvas ref="chartCanvas" :id="chartId"></canvas>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch, computed } from 'vue'
import { Chart, registerables } from 'chart.js'
import type { ChartConfiguration, ChartData } from 'chart.js'
import { useStockStore } from '../../stores/stock'

// Register Chart.js components
Chart.register(...registerables)

interface Props {
  chartId?: string
  height?: number
}

const props = withDefaults(defineProps<Props>(), {
  chartId: 'stock-movement-chart',
  height: 300
})

const stockStore = useStockStore()
const chartCanvas = ref<HTMLCanvasElement>()
let chart: Chart | null = null

// Generate mock data for the last 30 days
const generateChartData = computed(() => {
  const days = 30
  const labels: string[] = []
  const inData: number[] = []
  const outData: number[] = []
  
  for (let i = days - 1; i >= 0; i--) {
    const date = new Date()
    date.setDate(date.getDate() - i)
    labels.push(date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' }))
    
    // Generate realistic mock data
    const baseIn = Math.random() * 100 + 50
    const baseOut = Math.random() * 80 + 30
    inData.push(Math.round(baseIn + Math.random() * 50))
    outData.push(Math.round(baseOut + Math.random() * 40))
  }
  
  return {
    labels,
    datasets: [
      {
        label: 'Stock In',
        data: inData,
        borderColor: 'rgb(34, 197, 94)',
        backgroundColor: 'rgba(34, 197, 94, 0.1)',
        tension: 0.4,
        fill: true
      },
      {
        label: 'Stock Out',
        data: outData,
        borderColor: 'rgb(239, 68, 68)',
        backgroundColor: 'rgba(239, 68, 68, 0.1)',
        tension: 0.4,
        fill: true
      }
    ]
  }
})

const chartConfig: ChartConfiguration<'line'> = {
  type: 'line',
  data: generateChartData.value,
  options: {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'top' as const,
        labels: {
          usePointStyle: true,
          padding: 20,
          color: '#6B7280'
        }
      },
      tooltip: {
        mode: 'index',
        intersect: false,
        backgroundColor: 'rgba(0, 0, 0, 0.8)',
        titleColor: '#FFFFFF',
        bodyColor: '#FFFFFF',
        borderColor: '#374151',
        borderWidth: 1
      }
    },
    scales: {
      x: {
        grid: {
          display: false
        },
        ticks: {
          color: '#6B7280',
          maxRotation: 0
        }
      },
      y: {
        beginAtZero: true,
        grid: {
          color: '#E5E7EB'
        },
        ticks: {
          color: '#6B7280',
          callback: function(value) {
            return value + ' units'
          }
        }
      }
    },
    interaction: {
      mode: 'nearest',
      axis: 'x',
      intersect: false
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
watch(() => stockStore.movements, () => {
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
