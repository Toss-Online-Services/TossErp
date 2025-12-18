<template>
  <BaseChart
    type="doughnut"
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
  data: number[]
  height?: number | string
  title?: string
  colors?: string[]
  cutout?: number | string
}

const props = withDefaults(defineProps<Props>(), {
  height: 400,
  cutout: '50%'
})

const chartData = computed((): ChartData<'doughnut'> => ({
  labels: props.labels,
  datasets: [{
    data: props.data,
    backgroundColor: props.colors || getDefaultColors(props.data.length),
    borderColor: props.colors?.map(color => color.replace(/[\d.]+\)$/g, '1)')) || getDefaultColors(props.data.length, 1),
    borderWidth: 2,
  }]
}))

const chartOptions = computed((): ChartOptions<'doughnut'> => ({
  responsive: true,
  maintainAspectRatio: false,
  cutout: props.cutout,
  plugins: {
    legend: {
      position: 'right' as const,
    },
    title: {
      display: !!props.title,
      text: props.title,
    },
    tooltip: {
      callbacks: {
        label: function(context) {
          const total = context.dataset.data.reduce((a: number, b: number) => a + b, 0)
          const percentage = ((context.parsed / total) * 100).toFixed(1)
          return `${context.label}: ${context.parsed} (${percentage}%)`
        }
      }
    }
  },
}))

function getDefaultColors(count: number, alpha: number = 0.8): string[] {
  const colors = [
    `rgba(59, 130, 246, ${alpha})`,   // Blue
    `rgba(16, 185, 129, ${alpha})`,   // Green
    `rgba(245, 158, 11, ${alpha})`,   // Amber
    `rgba(239, 68, 68, ${alpha})`,    // Red
    `rgba(139, 92, 246, ${alpha})`,   // Purple
    `rgba(236, 72, 153, ${alpha})`,   // Pink
    `rgba(20, 184, 166, ${alpha})`,   // Teal
    `rgba(251, 146, 60, ${alpha})`,   // Orange
    `rgba(168, 85, 247, ${alpha})`,   // Violet
    `rgba(34, 197, 94, ${alpha})`,    // Emerald
  ]
  
  return Array.from({ length: count }, (_, i) => colors[i % colors.length])
}
</script>
