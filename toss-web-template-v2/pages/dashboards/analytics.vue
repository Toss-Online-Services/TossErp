<template>
  <div>
    <!-- Statistics Cards Row -->
    <div class="row">
      <div class="col-xl-3 col-sm-6 mb-4">
        <div class="card">
          <div class="card-body p-3">
            <div class="row">
              <div class="col-8">
                <div class="numbers">
                  <MDTypography variant="caption" color="text" font-weight="bold" text-transform="uppercase">
                    Today's Money
                  </MDTypography>
                  <MDTypography variant="h5" font-weight="bold">
                    $53,000
                  </MDTypography>
                  <MDTypography variant="button" color="success" font-weight="bold">
                    +55%
                  </MDTypography>
                  <span class="text-sm text-gray-600"> since yesterday</span>
                </div>
              </div>
              <div class="col-4 text-end">
                <MDBox 
                  variant="gradient"
                  bg-color="primary"
                  :p="2"
                  class="text-center border-radius-md"
                >
                  <Icon name="material-symbols:weekend-outline" class="text-white text-2xl" />
                </MDBox>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-xl-3 col-sm-6 mb-4">
        <div class="card">
          <div class="card-body p-3">
            <div class="row">
              <div class="col-8">
                <div class="numbers">
                  <MDTypography variant="caption" color="text" font-weight="bold" text-transform="uppercase">
                    Today's Users
                  </MDTypography>
                  <MDTypography variant="h5" font-weight="bold">
                    2,300
                  </MDTypography>
                  <MDTypography variant="button" color="success" font-weight="bold">
                    +3%
                  </MDTypography>
                  <span class="text-sm text-gray-600"> since last week</span>
                </div>
              </div>
              <div class="col-4 text-end">
                <MDBox 
                  variant="gradient"
                  bg-color="success"
                  :p="2"
                  class="text-center border-radius-md"
                >
                  <Icon name="material-symbols:person-outline" class="text-white text-2xl" />
                </MDBox>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-xl-3 col-sm-6 mb-4">
        <div class="card">
          <div class="card-body p-3">
            <div class="row">
              <div class="col-8">
                <div class="numbers">
                  <MDTypography variant="caption" color="text" font-weight="bold" text-transform="uppercase">
                    New Clients
                  </MDTypography>
                  <MDTypography variant="h5" font-weight="bold">
                    +3,462
                  </MDTypography>
                  <MDTypography variant="button" color="error" font-weight="bold">
                    -2%
                  </MDTypography>
                  <span class="text-sm text-gray-600"> since last quarter</span>
                </div>
              </div>
              <div class="col-4 text-end">
                <MDBox 
                  variant="gradient"
                  bg-color="error"
                  :p="2"
                  class="text-center border-radius-md"
                >
                  <Icon name="material-symbols:leaderboard-outline" class="text-white text-2xl" />
                </MDBox>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-xl-3 col-sm-6 mb-4">
        <div class="card">
          <div class="card-body p-3">
            <div class="row">
              <div class="col-8">
                <div class="numbers">
                  <MDTypography variant="caption" color="text" font-weight="bold" text-transform="uppercase">
                    Sales
                  </MDTypography>
                  <MDTypography variant="h5" font-weight="bold">
                    $103,430
                  </MDTypography>
                  <MDTypography variant="button" color="success" font-weight="bold">
                    +5%
                  </MDTypography>
                  <span class="text-sm text-gray-600"> than last month</span>
                </div>
              </div>
              <div class="col-4 text-end">
                <MDBox 
                  variant="gradient"
                  bg-color="warning"
                  :p="2"
                  class="text-center border-radius-md"
                >
                  <Icon name="material-symbols:shopping-cart-outline" class="text-white text-2xl" />
                </MDBox>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Charts Row -->
    <div class="row mt-4">
      <div class="col-lg-7 mb-4">
        <div class="card">
          <div class="card-header pb-0">
            <MDTypography variant="h6">Sales Overview</MDTypography>
            <MDTypography variant="button" color="text" font-weight="regular">
              <Icon name="material-symbols:arrow-upward" class="text-success" />
              <span class="font-weight-bold">4% more</span> in 2024
            </MDTypography>
          </div>
          <div class="card-body p-3">
            <div class="chart">
              <canvas id="sales-chart" height="170"></canvas>
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-5 mb-4">
        <div class="card">
          <div class="card-header pb-0 p-3">
            <MDTypography variant="h6">Categories</MDTypography>
          </div>
          <div class="card-body p-3">
            <div class="chart">
              <canvas id="pie-chart" height="230"></canvas>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Projects Table -->
    <div class="row mt-4">
      <div class="col-12">
        <div class="card">
          <div class="card-header pb-0">
            <div class="row">
              <div class="col-lg-6 col-7">
                <MDTypography variant="h6">Projects</MDTypography>
                <MDTypography variant="button" color="text" font-weight="regular">
                  <Icon name="material-symbols:check-circle-outline" class="text-success" />
                  <span class="font-weight-bold ms-1">30 done</span> this month
                </MDTypography>
              </div>
              <div class="col-lg-6 col-5 text-end">
                <MDButton color="info" size="small">
                  <Icon name="material-symbols:add" /> New Project
                </MDButton>
              </div>
            </div>
          </div>
          <div class="card-body px-0 pb-2">
            <div class="table-responsive">
              <table class="table align-items-center mb-0">
                <thead>
                  <tr>
                    <th class="text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">Project</th>
                    <th class="text-uppercase text-secondary text-xxs font-weight-bolder opacity-7 ps-2">Members</th>
                    <th class="text-center text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">Budget</th>
                    <th class="text-center text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">Completion</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="project in projects" :key="project.name">
                    <td>
                      <div class="d-flex px-2 py-1">
                        <div>
                          <Icon :name="project.icon" class="me-3 text-primary text-xl" />
                        </div>
                        <div class="d-flex flex-column justify-content-center">
                          <MDTypography variant="button" font-weight="medium">{{ project.name }}</MDTypography>
                        </div>
                      </div>
                    </td>
                    <td>
                      <div class="avatar-group">
                        <span v-for="(member, i) in project.members.slice(0, 4)" :key="i" class="text-xs">
                          {{ member }}
                        </span>
                      </div>
                    </td>
                    <td class="align-middle text-center text-sm">
                      <MDTypography variant="caption" color="text" font-weight="medium">{{ project.budget }}</MDTypography>
                    </td>
                    <td class="align-middle">
                      <div class="d-flex align-items-center justify-content-center">
                        <span class="me-2 text-xs font-weight-bold">{{ project.completion }}%</span>
                        <div class="progress w-50">
                          <div 
                            class="progress-bar" 
                            :class="`bg-gradient-${project.color}`"
                            role="progressbar"
                            :style="`width: ${project.completion}%`"
                          ></div>
                        </div>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { Chart, registerables } from 'chart.js'

Chart.register(...registerables)

definePageMeta({
  layout: 'default'
})

const projects = ref([
  { 
    name: 'Material UI XD Version',
    icon: 'material-symbols:apps',
    members: ['Ryan', 'Jessica', 'Alex'],
    budget: '$14,000',
    completion: 60,
    color: 'info'
  },
  { 
    name: 'Add Progress Track',
    icon: 'material-symbols:code',
    members: ['Alex', 'Michael'],
    budget: '$3,000',
    completion: 10,
    color: 'info'
  },
  { 
    name: 'Fix Platform Errors',
    icon: 'material-symbols:shopping-cart',
    members: ['Jessica', 'Michael'],
    budget: 'Not set',
    completion: 100,
    color: 'success'
  },
  { 
    name: 'Launch our Mobile App',
    icon: 'material-symbols:phone-iphone',
    members: ['Ryan', 'Jessica', 'Alex', 'Michael'],
    budget: '$20,500',
    completion: 100,
    color: 'success'
  },
  { 
    name: 'Add the New Pricing Page',
    icon: 'material-symbols:shopping-cart',
    members: ['Alex'],
    budget: '$500',
    completion: 25,
    color: 'info'
  },
  { 
    name: 'Redesign New Online Shop',
    icon: 'material-symbols:storefront',
    members: ['Ryan', 'Jessica'],
    budget: '$2,000',
    completion: 40,
    color: 'info'
  }
])

// Chart initialization
onMounted(() => {
  // Sales Overview Line Chart
  const salesCtx = document.getElementById('sales-chart') as HTMLCanvasElement
  if (salesCtx) {
    new Chart(salesCtx, {
      type: 'line',
      data: {
        labels: ['Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
        datasets: [{
          label: 'Sales',
          data: [50, 40, 300, 220, 500, 250, 400, 230, 500],
          borderColor: '#1a73e8',
          backgroundColor: 'rgba(26, 115, 232, 0.1)',
          tension: 0.4,
          fill: true,
          borderWidth: 3,
          pointRadius: 0
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
            beginAtZero: true,
            grid: {
              drawBorder: false,
              display: true,
              color: 'rgba(0, 0, 0, 0.05)'
            },
            ticks: {
              padding: 10
            }
          },
          x: {
            grid: {
              drawBorder: false,
              display: false
            },
            ticks: {
              padding: 10
            }
          }
        }
      }
    })
  }
  
  // Categories Pie Chart
  const pieCtx = document.getElementById('pie-chart') as HTMLCanvasElement
  if (pieCtx) {
    new Chart(pieCtx, {
      type: 'doughnut',
      data: {
        labels: ['Electronics', 'Clothing', 'Furniture', 'Others'],
        datasets: [{
          data: [30, 25, 20, 25],
          backgroundColor: [
            '#1a73e8',
            '#4caf50',
            '#fb8c00',
            '#e91e63'
          ],
          borderWidth: 0
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            position: 'bottom',
            labels: {
              padding: 20,
              usePointStyle: true
            }
          }
        }
      }
    })
  }
})
</script>

<style scoped>
.progress {
  height: 0.375rem;
  border-radius: 0.5rem;
  background-color: rgb(232, 234, 237);
}

.progress-bar {
  border-radius: 0.5rem;
}

.avatar-group {
  display: flex;
  gap: 0.5rem;
}
</style>
