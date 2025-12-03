<script setup lang="ts">
import { Line } from 'vue-chartjs'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler
} from 'chart.js'

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  Filler
)

interface Props {
  labels: string[]
  datasets: Array<{
    label: string
    data: number[]
    borderColor?: string
    backgroundColor?: string
    tension?: number
    fill?: boolean
    pointRadius?: number
  }>
  height?: number
}

const props = withDefaults(defineProps<Props>(), {
  height: 300
})

const chartData = computed(() => ({
  labels: props.labels,
  datasets: props.datasets.map(dataset => ({
    ...dataset,
    tension: dataset.tension ?? 0.4,
    borderWidth: 3,
    pointRadius: dataset.pointRadius ?? 2,
    pointBackgroundColor: dataset.borderColor,
    fill: dataset.fill ?? false
  }))
}))

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: false
    },
    tooltip: {
      enabled: true,
      mode: 'index' as const,
      intersect: false
    }
  },
  interaction: {
    intersect: false,
    mode: 'index' as const
  },
  scales: {
    y: {
      grid: {
        drawBorder: false,
        display: true,
        drawOnChartArea: true,
        drawTicks: false,
        borderDash: [5, 5],
        color: '#c1c4ce5c'
      },
      ticks: {
        display: true,
        padding: 10,
        color: '#b2b9bf',
        font: {
          size: 12,
          weight: 300,
          family: 'Inter',
          style: 'normal',
          lineHeight: 2
        }
      }
    },
    x: {
      grid: {
        drawBorder: false,
        display: true,
        drawOnChartArea: true,
        drawTicks: true,
        borderDash: [5, 5],
        color: '#c1c4ce5c'
      },
      ticks: {
        display: true,
        color: '#b2b9bf',
        padding: 10,
        font: {
          size: 12,
          weight: 300,
          family: 'Inter',
          style: 'normal',
          lineHeight: 2
        }
      }
    }
  }
}
</script>

<template>
  <div :style="{ height: `${height}px` }">
    <Line :data="chartData" :options="chartOptions" />
  </div>
</template>

