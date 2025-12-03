<script setup lang="ts">
import { computed } from 'vue'
import { Bar } from 'vue-chartjs'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend
} from 'chart.js'

ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend)

interface Props {
  labels: string[]
  data: number[]
  backgroundColor?: string
  height?: number
  horizontal?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  backgroundColor: '#3A416F',
  height: 300,
  horizontal: false
})

const chartData = computed(() => ({
  labels: props.labels,
  datasets: [{
    label: 'Sales',
    data: props.data,
    backgroundColor: props.backgroundColor,
    borderWidth: 0,
    borderRadius: 4,
    borderSkipped: false,
    maxBarThickness: 35
  }]
}))

const chartOptions = computed(() => ({
  indexAxis: props.horizontal ? 'y' as const : 'x' as const,
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: false
    },
    tooltip: {
      enabled: true
    }
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
        color: '#9ca2b7',
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
        display: false,
        drawOnChartArea: true,
        drawTicks: true,
        color: '#c1c4ce5c'
      },
      ticks: {
        display: true,
        color: '#9ca2b7',
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
}))
</script>

<template>
  <div :style="{ height: `${height}px` }">
    <Bar :data="chartData" :options="chartOptions" />
  </div>
</template>

