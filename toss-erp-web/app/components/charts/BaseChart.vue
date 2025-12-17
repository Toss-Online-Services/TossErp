<template>
  <div class="chart-container" :style="{ height: height }">
    <canvas ref="chartCanvas"></canvas>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, onUnmounted, nextTick } from 'vue'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  Title,
  Tooltip,
  Legend,
  ArcElement,
  TimeScale,
  type ChartConfiguration,
  type ChartData,
  type ChartOptions
} from 'chart.js'
import 'chartjs-adapter-date-fns'

// Register Chart.js components
ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  Title,
  Tooltip,
  Legend,
  ArcElement,
  TimeScale
)

interface Props {
  type: 'line' | 'bar' | 'pie' | 'doughnut'
  data: ChartData<any>
  options?: ChartOptions<any>
  height?: string
}

const props = withDefaults(defineProps<Props>(), {
  height: '400px',
  options: () => ({})
})

const chartCanvas = ref<HTMLCanvasElement>()
let chartInstance: ChartJS | null = null

const defaultOptions: ChartOptions<any> = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'top' as const,
    },
    title: {
      display: false,
    },
  },
  scales: {
    y: {
      beginAtZero: true,
      grid: {
        color: 'rgba(0, 0, 0, 0.1)',
      },
    },
    x: {
      grid: {
        color: 'rgba(0, 0, 0, 0.1)',
      },
    },
  },
}

const mergedOptions = computed(() => {
  return {
    ...defaultOptions,
    ...props.options,
  }
})

const createChart = () => {
  if (!chartCanvas.value) return

  const config: ChartConfiguration = {
    type: props.type,
    data: props.data,
    options: mergedOptions.value,
  }

  chartInstance = new ChartJS(chartCanvas.value, config)
}

const updateChart = () => {
  if (!chartInstance) return

  chartInstance.data = props.data
  chartInstance.options = mergedOptions.value
  chartInstance.update()
}

const destroyChart = () => {
  if (chartInstance) {
    chartInstance.destroy()
    chartInstance = null
  }
}

watch(
  () => props.data,
  () => {
    updateChart()
  },
  { deep: true }
)

watch(
  () => props.options,
  () => {
    updateChart()
  },
  { deep: true }
)

onMounted(() => {
  nextTick(() => {
    createChart()
  })
})

onUnmounted(() => {
  destroyChart()
})
</script>

<style scoped>
.chart-container {
  position: relative;
  width: 100%;
}
</style>
