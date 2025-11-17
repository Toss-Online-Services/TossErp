<template>
  <div class="mini-chart">
    <canvas ref="chartCanvas"></canvas>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch, onBeforeUnmount } from 'vue'
import {
  Chart,
  LineController,
  LineElement,
  PointElement,
  LinearScale,
  CategoryScale,
  type ChartConfiguration
} from 'chart.js'

// Register Chart.js components
Chart.register(
  LineController,
  LineElement,
  PointElement,
  LinearScale,
  CategoryScale
)

interface Props {
  data: number[]
  color?: string
}

const props = withDefaults(defineProps<Props>(), {
  color: '#FFFFFF'
})

const chartCanvas = ref<HTMLCanvasElement | null>(null)
let chartInstance: Chart | null = null

const createChart = () => {
  if (!chartCanvas.value) return

  if (chartInstance) {
    chartInstance.destroy()
  }

  const config: ChartConfiguration = {
    type: 'line',
    data: {
      labels: props.data.map((_, i) => i.toString()),
      datasets: [
        {
          data: props.data,
          borderColor: props.color,
          backgroundColor: `${props.color}33`,
          borderWidth: 2,
          tension: 0.4,
          fill: true,
          pointRadius: 0,
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
          enabled: false
        }
      },
      scales: {
        x: {
          display: false
        },
        y: {
          display: false,
          beginAtZero: true
        }
      }
    }
  }

  chartInstance = new Chart(chartCanvas.value, config)
}

onMounted(() => {
  createChart()
})

watch(() => props.data, () => {
  createChart()
}, { deep: true })

onBeforeUnmount(() => {
  if (chartInstance) {
    chartInstance.destroy()
  }
})
</script>

<style scoped>
.mini-chart {
  height: 50px;
  width: 100%;
}
</style>


