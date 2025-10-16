<template>
  <BaseChart
    type="bar"
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
    backgroundColor?: string | string[]
    borderColor?: string | string[]
  }>
  height?: string
  title?: string
  yAxisLabel?: string
  xAxisLabel?: string
  horizontal?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  height: '400px',
  horizontal: false
})

const chartData = computed((): ChartData<'bar'> => ({
  labels: props.labels,
  datasets: props.datasets.map((dataset, index) => ({
    ...dataset,
    backgroundColor: dataset.backgroundColor || getDefaultColors(dataset.data.length, index),
    borderColor: dataset.borderColor || getDefaultColors(dataset.data.length, index, 1),
    borderWidth: 1,
  }))
}))

const chartOptions = computed((): ChartOptions<'bar'> => ({
  responsive: true,
  maintainAspectRatio: false,
  indexAxis: props.horizontal ? 'y' as const : 'x' as const,
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

function getDefaultColors(count: number, datasetIndex: number = 0, alpha: number = 0.8): string[] {
  const baseColors = [
    `rgba(59, 130, 246, ${alpha})`, // Blue
    `rgba(16, 185, 129, ${alpha})`, // Green
    `rgba(245, 158, 11, ${alpha})`, // Amber
    `rgba(239, 68, 68, ${alpha})`,  // Red
    `rgba(139, 92, 246, ${alpha})`, // Purple
    `rgba(236, 72, 153, ${alpha})`, // Pink
  ]
  
  if (count === 1) {
    return [baseColors[datasetIndex % baseColors.length]]
  }
  
  return Array.from({ length: count }, (_, i) => 
    baseColors[(i + datasetIndex) % baseColors.length]
  )
}
</script>
