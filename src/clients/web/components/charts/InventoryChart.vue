<template>
  <div class="inventory-chart h-full w-full">
    <canvas ref="chartCanvas"></canvas>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend
} from 'chart.js'

ChartJS.register(
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend
)

const chartCanvas = ref<HTMLCanvasElement>()
let chartInstance: ChartJS | null = null

const chartData = {
  labels: ['Electronics', 'Clothing', 'Food & Beverage', 'Automotive', 'Home & Garden', 'Sports'],
  datasets: [
    {
      label: 'In Stock',
      data: [450, 320, 180, 240, 380, 290],
      backgroundColor: 'rgba(34, 197, 94, 0.8)',
      borderColor: 'rgb(34, 197, 94)',
      borderWidth: 1
    },
    {
      label: 'Low Stock',
      data: [50, 80, 30, 60, 40, 35],
      backgroundColor: 'rgba(251, 191, 36, 0.8)',
      borderColor: 'rgb(251, 191, 36)',
      borderWidth: 1
    },
    {
      label: 'Out of Stock',
      data: [15, 25, 8, 12, 18, 10],
      backgroundColor: 'rgba(239, 68, 68, 0.8)',
      borderColor: 'rgb(239, 68, 68)',
      borderWidth: 1
    }
  ]
}

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: {
      position: 'top' as const,
      labels: {
        usePointStyle: true,
        padding: 20,
        color: '#374151'
      }
    },
    tooltip: {
      backgroundColor: 'rgba(17, 24, 39, 0.9)',
      titleColor: '#fff',
      bodyColor: '#fff',
      borderColor: 'rgba(59, 130, 246, 0.5)',
      borderWidth: 1,
      cornerRadius: 8,
      callbacks: {
        label: function(context: any) {
          return `${context.dataset.label}: ${context.parsed.y} items`
        }
      }
    }
  },
  scales: {
    x: {
      grid: {
        display: false
      },
      ticks: {
        color: '#6B7280'
      }
    },
    y: {
      grid: {
        color: 'rgba(107, 114, 128, 0.1)'
      },
      ticks: {
        color: '#6B7280'
      },
      beginAtZero: true
    }
  }
}

onMounted(() => {
  if (chartCanvas.value) {
    chartInstance = new ChartJS(chartCanvas.value, {
      type: 'bar',
      data: chartData,
      options: chartOptions
    })
  }
})

onUnmounted(() => {
  if (chartInstance) {
    chartInstance.destroy()
  }
})

defineOptions({
  name: 'InventoryChart'
})
</script>
