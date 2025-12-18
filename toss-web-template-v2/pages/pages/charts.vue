<template>
  <div class="container-fluid py-4">
    <div class="row">
      <div class="col-12">
        <div class="text-center mb-5">
          <MDTypography variant="h3" font-weight="bold" class="mb-2">
            Charts
          </MDTypography>
          <MDTypography variant="body2" color="text">
            Visualize your data with beautiful, responsive charts
          </MDTypography>
        </div>
      </div>
    </div>

    <!-- Line Charts -->
    <div class="row mb-4">
      <div class="col-lg-6 mb-4">
        <div class="card h-100">
          <div class="card-header pb-0">
            <MDTypography variant="h6" font-weight="medium">
              Line Chart
            </MDTypography>
            <MDTypography variant="caption" color="text">
              Sales performance over time
            </MDTypography>
          </div>
          <div class="card-body">
            <canvas id="line-chart" height="200"></canvas>
          </div>
        </div>
      </div>
      
      <div class="col-lg-6 mb-4">
        <div class="card h-100">
          <div class="card-header pb-0">
            <MDTypography variant="h6" font-weight="medium">
              Area Chart
            </MDTypography>
            <MDTypography variant="caption" color="text">
              Revenue with filled area
            </MDTypography>
          </div>
          <div class="card-body">
            <canvas id="area-chart" height="200"></canvas>
          </div>
        </div>
      </div>
    </div>

    <!-- Bar Charts -->
    <div class="row mb-4">
      <div class="col-lg-6 mb-4">
        <div class="card h-100">
          <div class="card-header pb-0">
            <MDTypography variant="h6" font-weight="medium">
              Bar Chart
            </MDTypography>
            <MDTypography variant="caption" color="text">
              Product sales comparison
            </MDTypography>
          </div>
          <div class="card-body">
            <canvas id="bar-chart" height="200"></canvas>
          </div>
        </div>
      </div>
      
      <div class="col-lg-6 mb-4">
        <div class="card h-100">
          <div class="card-header pb-0">
            <MDTypography variant="h6" font-weight="medium">
              Horizontal Bar Chart
            </MDTypography>
            <MDTypography variant="caption" color="text">
              Sales by region
            </MDTypography>
          </div>
          <div class="card-body">
            <canvas id="horizontal-bar-chart" height="200"></canvas>
          </div>
        </div>
      </div>
    </div>

    <!-- Pie and Doughnut Charts -->
    <div class="row mb-4">
      <div class="col-lg-4 mb-4">
        <div class="card h-100">
          <div class="card-header pb-0">
            <MDTypography variant="h6" font-weight="medium">
              Pie Chart
            </MDTypography>
            <MDTypography variant="caption" color="text">
              Market share distribution
            </MDTypography>
          </div>
          <div class="card-body d-flex align-items-center">
            <canvas id="pie-chart" height="250"></canvas>
          </div>
        </div>
      </div>
      
      <div class="col-lg-4 mb-4">
        <div class="card h-100">
          <div class="card-header pb-0">
            <MDTypography variant="h6" font-weight="medium">
              Doughnut Chart
            </MDTypography>
            <MDTypography variant="caption" color="text">
              Traffic sources
            </MDTypography>
          </div>
          <div class="card-body d-flex align-items-center">
            <canvas id="doughnut-chart" height="250"></canvas>
          </div>
        </div>
      </div>
      
      <div class="col-lg-4 mb-4">
        <div class="card h-100">
          <div class="card-header pb-0">
            <MDTypography variant="h6" font-weight="medium">
              Polar Area Chart
            </MDTypography>
            <MDTypography variant="caption" color="text">
              Category distribution
            </MDTypography>
          </div>
          <div class="card-body d-flex align-items-center">
            <canvas id="polar-chart" height="250"></canvas>
          </div>
        </div>
      </div>
    </div>

    <!-- Mixed and Advanced Charts -->
    <div class="row mb-4">
      <div class="col-lg-6 mb-4">
        <div class="card h-100">
          <div class="card-header pb-0">
            <MDTypography variant="h6" font-weight="medium">
              Radar Chart
            </MDTypography>
            <MDTypography variant="caption" color="text">
              Multi-dimensional analysis
            </MDTypography>
          </div>
          <div class="card-body d-flex align-items-center">
            <canvas id="radar-chart" height="250"></canvas>
          </div>
        </div>
      </div>
      
      <div class="col-lg-6 mb-4">
        <div class="card h-100">
          <div class="card-header pb-0">
            <MDTypography variant="h6" font-weight="medium">
              Mixed Chart
            </MDTypography>
            <MDTypography variant="caption" color="text">
              Combined line and bar
            </MDTypography>
          </div>
          <div class="card-body">
            <canvas id="mixed-chart" height="250"></canvas>
          </div>
        </div>
      </div>
    </div>

    <!-- Bubble and Scatter Charts -->
    <div class="row">
      <div class="col-lg-6 mb-4">
        <div class="card h-100">
          <div class="card-header pb-0">
            <MDTypography variant="h6" font-weight="medium">
              Bubble Chart
            </MDTypography>
            <MDTypography variant="caption" color="text">
              Three-dimensional data visualization
            </MDTypography>
          </div>
          <div class="card-body">
            <canvas id="bubble-chart" height="250"></canvas>
          </div>
        </div>
      </div>
      
      <div class="col-lg-6 mb-4">
        <div class="card h-100">
          <div class="card-header pb-0">
            <MDTypography variant="h6" font-weight="medium">
              Scatter Chart
            </MDTypography>
            <MDTypography variant="caption" color="text">
              Correlation analysis
            </MDTypography>
          </div>
          <div class="card-body">
            <canvas id="scatter-chart" height="250"></canvas>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted } from 'vue'
import { 
  Chart,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  ArcElement,
  RadialLinearScale,
  Title,
  Tooltip,
  Legend,
  Filler
} from 'chart.js'

definePageMeta({
  layout: 'default'
})

// Register Chart.js components
Chart.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  BarElement,
  ArcElement,
  RadialLinearScale,
  Title,
  Tooltip,
  Legend,
  Filler
)

onMounted(() => {
  // Line Chart
  const lineCtx = document.getElementById('line-chart') as HTMLCanvasElement
  new Chart(lineCtx, {
    type: 'line',
    data: {
      labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul'],
      datasets: [{
        label: 'Sales',
        data: [65, 59, 80, 81, 56, 55, 70],
        borderColor: 'rgb(94, 114, 228)',
        backgroundColor: 'rgba(94, 114, 228, 0.1)',
        tension: 0.4
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          display: true,
          position: 'top'
        }
      },
      scales: {
        y: {
          beginAtZero: true
        }
      }
    }
  })

  // Area Chart
  const areaCtx = document.getElementById('area-chart') as HTMLCanvasElement
  new Chart(areaCtx, {
    type: 'line',
    data: {
      labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul'],
      datasets: [{
        label: 'Revenue',
        data: [12000, 19000, 15000, 25000, 22000, 30000, 28000],
        borderColor: 'rgb(26, 115, 232)',
        backgroundColor: 'rgba(26, 115, 232, 0.3)',
        fill: true,
        tension: 0.4
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          display: true,
          position: 'top'
        }
      },
      scales: {
        y: {
          beginAtZero: true
        }
      }
    }
  })

  // Bar Chart
  const barCtx = document.getElementById('bar-chart') as HTMLCanvasElement
  new Chart(barCtx, {
    type: 'bar',
    data: {
      labels: ['Product A', 'Product B', 'Product C', 'Product D', 'Product E'],
      datasets: [{
        label: 'Sales',
        data: [12, 19, 15, 25, 22],
        backgroundColor: [
          'rgba(94, 114, 228, 0.8)',
          'rgba(26, 115, 232, 0.8)',
          'rgba(102, 187, 106, 0.8)',
          'rgba(255, 167, 38, 0.8)',
          'rgba(239, 83, 80, 0.8)'
        ],
        borderColor: [
          'rgb(94, 114, 228)',
          'rgb(26, 115, 232)',
          'rgb(102, 187, 106)',
          'rgb(255, 167, 38)',
          'rgb(239, 83, 80)'
        ],
        borderWidth: 1
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          display: false
        }
      },
      scales: {
        y: {
          beginAtZero: true
        }
      }
    }
  })

  // Horizontal Bar Chart
  const hbarCtx = document.getElementById('horizontal-bar-chart') as HTMLCanvasElement
  new Chart(hbarCtx, {
    type: 'bar',
    data: {
      labels: ['North America', 'Europe', 'Asia', 'South America', 'Africa'],
      datasets: [{
        label: 'Sales by Region',
        data: [45000, 35000, 52000, 18000, 12000],
        backgroundColor: 'rgba(94, 114, 228, 0.8)',
        borderColor: 'rgb(94, 114, 228)',
        borderWidth: 1
      }]
    },
    options: {
      indexAxis: 'y',
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          display: false
        }
      },
      scales: {
        x: {
          beginAtZero: true
        }
      }
    }
  })

  // Pie Chart
  const pieCtx = document.getElementById('pie-chart') as HTMLCanvasElement
  new Chart(pieCtx, {
    type: 'pie',
    data: {
      labels: ['Company A', 'Company B', 'Company C', 'Company D'],
      datasets: [{
        data: [35, 25, 25, 15],
        backgroundColor: [
          'rgba(94, 114, 228, 0.8)',
          'rgba(26, 115, 232, 0.8)',
          'rgba(102, 187, 106, 0.8)',
          'rgba(255, 167, 38, 0.8)'
        ],
        borderColor: [
          'rgb(94, 114, 228)',
          'rgb(26, 115, 232)',
          'rgb(102, 187, 106)',
          'rgb(255, 167, 38)'
        ],
        borderWidth: 1
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: 'bottom'
        }
      }
    }
  })

  // Doughnut Chart
  const doughnutCtx = document.getElementById('doughnut-chart') as HTMLCanvasElement
  new Chart(doughnutCtx, {
    type: 'doughnut',
    data: {
      labels: ['Direct', 'Organic', 'Referral', 'Social'],
      datasets: [{
        data: [45, 30, 15, 10],
        backgroundColor: [
          'rgba(94, 114, 228, 0.8)',
          'rgba(26, 115, 232, 0.8)',
          'rgba(102, 187, 106, 0.8)',
          'rgba(255, 167, 38, 0.8)'
        ],
        borderColor: [
          'rgb(94, 114, 228)',
          'rgb(26, 115, 232)',
          'rgb(102, 187, 106)',
          'rgb(255, 167, 38)'
        ],
        borderWidth: 1
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: 'bottom'
        }
      }
    }
  })

  // Polar Area Chart
  const polarCtx = document.getElementById('polar-chart') as HTMLCanvasElement
  new Chart(polarCtx, {
    type: 'polarArea',
    data: {
      labels: ['Electronics', 'Clothing', 'Food', 'Books', 'Toys'],
      datasets: [{
        data: [30, 25, 20, 15, 10],
        backgroundColor: [
          'rgba(94, 114, 228, 0.6)',
          'rgba(26, 115, 232, 0.6)',
          'rgba(102, 187, 106, 0.6)',
          'rgba(255, 167, 38, 0.6)',
          'rgba(239, 83, 80, 0.6)'
        ],
        borderColor: [
          'rgb(94, 114, 228)',
          'rgb(26, 115, 232)',
          'rgb(102, 187, 106)',
          'rgb(255, 167, 38)',
          'rgb(239, 83, 80)'
        ],
        borderWidth: 1
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: 'bottom'
        }
      }
    }
  })

  // Radar Chart
  const radarCtx = document.getElementById('radar-chart') as HTMLCanvasElement
  new Chart(radarCtx, {
    type: 'radar',
    data: {
      labels: ['Speed', 'Reliability', 'Comfort', 'Safety', 'Efficiency', 'Performance'],
      datasets: [{
        label: 'Product A',
        data: [85, 90, 75, 95, 80, 88],
        borderColor: 'rgb(94, 114, 228)',
        backgroundColor: 'rgba(94, 114, 228, 0.2)'
      }, {
        label: 'Product B',
        data: [78, 85, 90, 88, 92, 82],
        borderColor: 'rgb(26, 115, 232)',
        backgroundColor: 'rgba(26, 115, 232, 0.2)'
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: 'bottom'
        }
      }
    }
  })

  // Mixed Chart
  const mixedCtx = document.getElementById('mixed-chart') as HTMLCanvasElement
  new Chart(mixedCtx, {
    type: 'bar',
    data: {
      labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
      datasets: [{
        type: 'line',
        label: 'Revenue',
        data: [50000, 55000, 52000, 60000, 58000, 65000],
        borderColor: 'rgb(94, 114, 228)',
        backgroundColor: 'rgba(94, 114, 228, 0.1)',
        yAxisID: 'y'
      }, {
        type: 'bar',
        label: 'Orders',
        data: [450, 480, 460, 520, 500, 540],
        backgroundColor: 'rgba(26, 115, 232, 0.8)',
        yAxisID: 'y1'
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: 'top'
        }
      },
      scales: {
        y: {
          type: 'linear',
          display: true,
          position: 'left'
        },
        y1: {
          type: 'linear',
          display: true,
          position: 'right',
          grid: {
            drawOnChartArea: false
          }
        }
      }
    }
  })

  // Bubble Chart
  const bubbleCtx = document.getElementById('bubble-chart') as HTMLCanvasElement
  new Chart(bubbleCtx, {
    type: 'bubble',
    data: {
      datasets: [{
        label: 'Dataset 1',
        data: [
          { x: 20, y: 30, r: 15 },
          { x: 40, y: 10, r: 10 },
          { x: 30, y: 40, r: 20 },
          { x: 10, y: 20, r: 12 },
          { x: 50, y: 50, r: 18 }
        ],
        backgroundColor: 'rgba(94, 114, 228, 0.6)',
        borderColor: 'rgb(94, 114, 228)'
      }, {
        label: 'Dataset 2',
        data: [
          { x: 15, y: 45, r: 14 },
          { x: 35, y: 25, r: 11 },
          { x: 45, y: 15, r: 16 },
          { x: 25, y: 35, r: 13 },
          { x: 55, y: 55, r: 19 }
        ],
        backgroundColor: 'rgba(26, 115, 232, 0.6)',
        borderColor: 'rgb(26, 115, 232)'
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: 'top'
        }
      },
      scales: {
        x: {
          beginAtZero: true
        },
        y: {
          beginAtZero: true
        }
      }
    }
  })

  // Scatter Chart
  const scatterCtx = document.getElementById('scatter-chart') as HTMLCanvasElement
  new Chart(scatterCtx, {
    type: 'scatter',
    data: {
      datasets: [{
        label: 'Group A',
        data: [
          { x: -10, y: 0 },
          { x: 0, y: 10 },
          { x: 10, y: 5 },
          { x: 0.5, y: 5.5 },
          { x: -5, y: -5 }
        ],
        backgroundColor: 'rgba(94, 114, 228, 0.6)',
        borderColor: 'rgb(94, 114, 228)'
      }, {
        label: 'Group B',
        data: [
          { x: -8, y: 8 },
          { x: 2, y: -2 },
          { x: 8, y: -8 },
          { x: -2, y: 2 },
          { x: 5, y: -5 }
        ],
        backgroundColor: 'rgba(26, 115, 232, 0.6)',
        borderColor: 'rgb(26, 115, 232)'
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          position: 'top'
        }
      },
      scales: {
        x: {
          type: 'linear',
          position: 'bottom'
        },
        y: {
          beginAtZero: true
        }
      }
    }
  })
})
</script>

<style scoped>
.card {
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
  border: none;
  border-radius: 0.75rem;
}

.card-header {
  background: transparent;
  border-bottom: none;
  padding: 1.5rem;
}

.card-body {
  padding: 1.5rem;
}
</style>
