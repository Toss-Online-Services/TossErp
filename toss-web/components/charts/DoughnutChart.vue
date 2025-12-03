<script setup lang="ts">
import { computed } from 'vue'
import { Doughnut } from 'vue-chartjs'
import {
  Chart as ChartJS,
  ArcElement,
  Tooltip,
  Legend
} from 'chart.js'

ChartJS.register(ArcElement, Tooltip, Legend)

interface Props {
  labels: string[]
  data: number[]
  colors?: string[]
  height?: number
  cutout?: number
}

const props = withDefaults(defineProps<Props>(), {
  colors: () => ['#03A9F4', '#3A416F', '#fb8c00', '#a8b8d8', '#e91e63'],
  height: 300,
  cutout: 60
})

const chartData = computed(() => ({
  labels: props.labels,
  datasets: [{
    label: 'Data',
    data: props.data,
    backgroundColor: props.colors,
    borderWidth: 2,
    cutout: `${props.cutout}%`,
    weight: 9,
    tension: 0.9,
    pointRadius: 2
  }]
}))

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      display: false
    },
    tooltip: {
      enabled: true
    }
  }
}
</script>

<template>
  <div :style="{ height: `${height}px` }">
    <Doughnut :data="chartData" :options="chartOptions" />
  </div>
</template>

