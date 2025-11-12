<template>
  <div class="relative w-full" :style="{ height: `${height}px` }">
    <canvas ref="chartCanvas"></canvas>
  </div>
</template>

<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref, watch } from 'vue'
import {
  Chart,
  LineController,
  LineElement,
  PointElement,
  LinearScale,
  CategoryScale,
  Tooltip,
  Legend,
  Filler,
  type ChartConfiguration,
} from 'chart.js'

Chart.register(
  LineController,
  LineElement,
  PointElement,
  LinearScale,
  CategoryScale,
  Tooltip,
  Legend,
  Filler,
)

interface Props {
  labels: string[]
  data: number[]
  label?: string
  color?: string
  height?: number
  showGrid?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  label: 'Data',
  color: '#10B981',
  height: 200,
  showGrid: true,
})

const chartCanvas = ref<HTMLCanvasElement | null>(null)
let chartInstance: Chart | null = null

const createChart = () => {
  if (!chartCanvas.value) {
    return
  }

  if (chartInstance) {
    chartInstance.destroy()
  }

  const config: ChartConfiguration = {
    type: 'line',
    data: {
      labels: props.labels,
      datasets: [
        {
          label: props.label,
          data: props.data,
          borderColor: props.color,
          backgroundColor: `${props.color}33`,
          borderWidth: 3,
          tension: 0.4,
          fill: true,
          pointRadius: 4,
          pointHoverRadius: 6,
          pointBackgroundColor: props.color,
          pointBorderColor: '#fff',
          pointBorderWidth: 2,
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: { display: false },
        tooltip: {
          mode: 'index',
          intersect: false,
          backgroundColor: 'rgba(0, 0, 0, 0.8)',
          padding: 12,
          cornerRadius: 8,
          titleFont: { size: 14, weight: 'bold' },
          bodyFont: { size: 13 },
        },
      },
      scales: {
        x: {
          grid: {
            display: props.showGrid,
            color: 'rgba(0, 0, 0, 0.05)',
          },
          border: { display: false },
        },
        y: {
          beginAtZero: true,
          grid: {
            display: props.showGrid,
            color: 'rgba(0, 0, 0, 0.05)',
          },
          border: { display: false },
        },
      },
      interaction: {
        mode: 'nearest',
        axis: 'x',
        intersect: false,
      },
    },
  }

  chartInstance = new Chart(chartCanvas.value, config)
}

onMounted(() => {
  createChart()
})

watch(
  () => [props.data, props.labels],
  () => {
    createChart()
  },
  { deep: true },
)

onBeforeUnmount(() => {
  if (chartInstance) {
    chartInstance.destroy()
  }
})
</script>
