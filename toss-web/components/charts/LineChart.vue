<template>
  <BaseChart
    type="line"
    :data="chartData"
    :options="chartOptions"
    :height="height"
  />
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { ChartData, ChartOptions } from 'chart.js'
import BaseChart from './BaseChart.vue'

interface Props {
  labels: string[]
  datasets: Array<{
    label: string
    data: number[]
    borderColor?: string
    backgroundColor?: string
    tension?: number
  }>
  height?: string
  title?: string
  yAxisLabel?: string
  xAxisLabel?: string
}

const props = withDefaults(defineProps<Props>(), {
  height: '400px'
})

const chartData = computed((): ChartData<'line'> => ({
  labels: props.labels,
  datasets: props.datasets.map((dataset, index) => ({
    ...dataset,
    borderColor: dataset.borderColor || getDefaultColor(index),
    backgroundColor: dataset.backgroundColor || getDefaultColor(index, 0.1),
    tension: dataset.tension ?? 0.4,
    fill: false,
  }))
}))

const chartOptions = computed((): ChartOptions<'line'> => ({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'top' as const,
    },
    title: {
      display: !!props.title,
      text: props.title,
    },
  },
  scales: {
    y: {
      beginAtZero: true,
      title: {
        display: !!props.yAxisLabel,
        text: props.yAxisLabel,
      },
      grid: {
        color: 'rgba(0, 0, 0, 0.1)',
      },
    },
    x: {
      title: {
        display: !!props.xAxisLabel,
        text: props.xAxisLabel,
      },
      grid: {
        color: 'rgba(0, 0, 0, 0.1)',
      },
    },
  },
}))

function getDefaultColor(index: number, alpha: number = 1): string {
  const colors = [
    `rgba(59, 130, 246, ${alpha})`, // Blue
    `rgba(16, 185, 129, ${alpha})`, // Green
    `rgba(245, 158, 11, ${alpha})`, // Amber
    `rgba(239, 68, 68, ${alpha})`,  // Red
    `rgba(139, 92, 246, ${alpha})`, // Purple
    `rgba(236, 72, 153, ${alpha})`, // Pink
  ]
  return colors[index % colors.length]
}
</script>
