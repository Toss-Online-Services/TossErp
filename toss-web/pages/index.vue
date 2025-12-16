// @ts-nocheck
<script setup lang="ts">
import { computed, defineAsyncComponent } from 'vue'

const BarChart = defineAsyncComponent(() => import('../components/charts/BarChart.vue'))
const LineChart = defineAsyncComponent(() => import('../components/charts/LineChart.vue'))
const DoughnutChart = defineAsyncComponent(() => import('../components/charts/DoughnutChart.vue'))

const statCards = [
  { title: 'Bookings', value: 281, delta: '+55%', icon: 'weekend' },
  { title: "Today's Users", value: 2300, delta: '+3%', icon: 'leaderboard' },
  { title: 'Revenue', value: '$34,000', delta: '+35%', icon: 'store' },
  { title: 'Followers', value: '+2,910', delta: 'Just updated', icon: 'person_add' }
]

const salesByCountry = [
  { country: 'United States', sales: 2500, value: '$230,900', bounce: '29.9%', flag: '/assets/img/icons/flags/US.png' },
  { country: 'Germany', sales: 3900, value: '$440,000', bounce: '40.22%', flag: '/assets/img/icons/flags/DE.png' },
  { country: 'Great Britain', sales: 1400, value: '$190,700', bounce: '23.44%', flag: '/assets/img/icons/flags/GB.png' },
  { country: 'Brasil', sales: 562, value: '$143,960', bounce: '32.14%', flag: '/assets/img/icons/flags/BR.png' }
]

const activeUsers = [
  { label: 'Users', value: '42K', progress: 60, icon: 'groups' },
  { label: 'Clicks', value: '1.7m', progress: 90, icon: 'ads_click' },
  { label: 'Sales', value: '399$', progress: 30, icon: 'receipt', color: 'from-orange-500 to-orange-600' },
  { label: 'Items', value: '74', progress: 50, icon: 'category', color: 'from-red-500 to-red-600' }
]

const barLabels = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']
const barData = [300, 230, 224, 218, 156, 200, 330]
const lineLabels = ['Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
const lineData = [50, 100, 200, 190, 400, 350, 500, 450, 700]
const multiLineDatasets = [
  { label: 'Organic Search', data: [50, 40, 300, 220, 500, 250, 400, 230, 500], borderColor: '#e91e63', backgroundColor: 'transparent', tension: 0.4, pointRadius: 2 },
  { label: 'Referral', data: [30, 90, 40, 140, 290, 290, 340, 230, 400], borderColor: '#3A416F', backgroundColor: 'transparent', tension: 0.4, pointRadius: 2 },
  { label: 'Direct', data: [40, 80, 70, 90, 30, 90, 140, 130, 200], borderColor: '#03A9F4', backgroundColor: 'transparent', tension: 0.4, pointRadius: 2 }
]

const affiliatesLabels = ['Creative Tim', 'Github', 'Bootsnipp', 'Dev.to', 'Codeinwp']
const affiliatesData = [15, 20, 12, 60, 20]
const affiliatesColors = ['#03A9F4', '#3A416F', '#fb8c00', '#a8b8d8', '#e91e63']

const formattedStatCards = computed(() => statCards)
</script>

<template>
  <div class="container-fluid py-2">
    <div class="row">
      <div class="col-lg-12 position-relative z-index-2">
        <div class="ms-3">
          <h3 class="mb-0 h4 font-weight-bolder">Analytics Dashboard</h3>
          <p class="mb-4">Check the sales, value and performance metrics.</p>
        </div>

        <!-- Stat Cards Row -->
        <div class="row">
          <div
            v-for="card in formattedStatCards"
            :key="card.title"
            class="col-lg-3 col-md-6 col-sm-6 mb-4"
          >
            <div class="card">
              <div class="card-header p-2 ps-3">
                <div class="d-flex justify-content-between">
                  <div>
                    <p class="text-sm mb-0 text-capitalize">{{ card.title }}</p>
                    <h4 class="mb-0">{{ card.value }}</h4>
                  </div>
                  <div class="icon icon-md icon-shape bg-gradient-dark shadow-dark shadow text-center border-radius-lg">
                    <i class="material-symbols-rounded opacity-10">{{ card.icon }}</i>
                  </div>
                </div>
              </div>
              <hr class="dark horizontal my-0">
              <div class="card-footer p-2 ps-3">
                <p class="mb-0 text-sm">
                  <span 
                    class="font-weight-bolder"
                    :class="card.delta.toString().includes('+') ? 'text-success' : ''"
                  >
                    {{ card.delta }}
                  </span> 
                  {{ card.delta.toString().includes('+') ? 'than last week' : '' }}
                </p>
              </div>
            </div>
          </div>
        </div>

        <!-- Chart Cards Row -->
        <div class="row">
          <div class="col-lg-4 col-md-6 mb-4">
            <div class="card">
              <div class="card-body">
                <h6 class="mb-0">Sales Overview</h6>
                <p class="text-sm">Last Campaign Performance</p>
                <div class="pe-2">
                  <div class="chart">
                    <ClientOnly>
                      <BarChart :labels="barLabels" :data="barData" backgroundColor="rgba(233, 30, 99, 0.8)" :height="170" />
                      <template #fallback>
                        <div style="height: 170px"></div>
                      </template>
                    </ClientOnly>
                  </div>
                </div>
                <hr class="dark horizontal">
                <div class="d-flex">
                  <i class="material-symbols-rounded text-sm my-auto me-1">schedule</i>
                  <p class="mb-0 text-sm">campaign sent 2 days ago</p>
                </div>
              </div>
            </div>
          </div>

          <div class="col-lg-4 col-md-6 mb-4">
            <div class="card">
              <div class="card-body">
                <h6 class="mb-0">Daily Sales</h6>
                <p class="text-sm">(<span class="font-weight-bolder">+15%</span>) increase in today sales.</p>
                <div class="pe-2">
                  <div class="chart">
                    <ClientOnly>
                      <LineChart
                        :labels="lineLabels"
                        :datasets="[{ label: 'Sales', data: lineData, borderColor: '#e91e63', backgroundColor: 'rgba(233, 30, 99, 0.1)', fill: true, tension: 0.4, pointRadius: 0 }]"
                        :height="170"
                      />
                      <template #fallback>
                        <div style="height: 170px"></div>
                      </template>
                    </ClientOnly>
                  </div>
                </div>
                <hr class="dark horizontal">
                <div class="d-flex">
                  <i class="material-symbols-rounded text-sm my-auto me-1">schedule</i>
                  <p class="mb-0 text-sm">updated 4 min ago</p>
                </div>
              </div>
            </div>
          </div>

          <div class="col-lg-4 col-md-6 mb-4">
            <div class="card">
              <div class="card-body">
                <h6 class="mb-0">Completed Tasks</h6>
                <p class="text-sm">Last Campaign Performance</p>
                <div class="pe-2">
                  <div class="chart">
                    <ClientOnly>
                      <LineChart
                        :labels="barLabels"
                        :datasets="[{ label: 'Tasks', data: [50, 45, 60, 70, 65, 75, 80], borderColor: '#03a9f4', backgroundColor: 'rgba(3, 169, 244, 0.1)', fill: true, tension: 0.4, pointRadius: 0 }]"
                        :height="170"
                      />
                      <template #fallback>
                        <div style="height: 170px"></div>
                      </template>
                    </ClientOnly>
                  </div>
                </div>
                <hr class="dark horizontal">
                <div class="d-flex">
                  <i class="material-symbols-rounded text-sm my-auto me-1">schedule</i>
                  <p class="mb-0 text-sm">just updated</p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Sales by Country & Active Users -->
        <div class="row mt-4">
          <div class="col-lg-7 mb-lg-0 mb-4">
            <div class="card">
              <div class="card-header pb-0 p-3">
                <div class="d-flex justify-content-between">
                  <h6 class="mb-2">Sales by Country</h6>
                </div>
              </div>
              <div class="table-responsive">
                <table class="table align-items-center">
                  <tbody>
                    <tr v-for="row in salesByCountry" :key="row.country">
                      <td class="w-30">
                        <div class="d-flex px-2 py-1 align-items-center">
                          <div>
                            <img :src="row.flag" alt="Country flag" style="width: 32px; height: 32px; object-fit: cover;" class="border-radius-lg">
                          </div>
                          <div class="ms-4">
                            <p class="text-xs font-weight-bold mb-0">Country:</p>
                            <h6 class="text-sm mb-0">{{ row.country }}</h6>
                          </div>
                        </div>
                      </td>
                      <td>
                        <div class="text-center">
                          <p class="text-xs font-weight-bold mb-0">Sales:</p>
                          <h6 class="text-sm mb-0">{{ row.sales }}</h6>
                        </div>
                      </td>
                      <td>
                        <div class="text-center">
                          <p class="text-xs font-weight-bold mb-0">Value:</p>
                          <h6 class="text-sm mb-0">{{ row.value }}</h6>
                        </div>
                      </td>
                      <td class="align-middle text-sm">
                        <div class="text-center">
                          <p class="text-xs font-weight-bold mb-0">Bounce:</p>
                          <h6 class="text-sm mb-0">{{ row.bounce }}</h6>
                        </div>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
          <div class="col-lg-5">
            <div class="card">
              <div class="card-header pb-0 p-3">
                <h6 class="mb-0">Categories</h6>
              </div>
              <div class="card-body p-3">
                <ul class="list-group">
                  <li v-for="item in activeUsers" :key="item.label" class="list-group-item border-0 d-flex justify-content-between ps-0 mb-2 border-radius-lg">
                    <div class="d-flex align-items-center">
                      <div class="icon icon-shape icon-sm me-3 bg-gradient-dark shadow text-center">
                        <i class="material-symbols-rounded opacity-10">{{ item.icon }}</i>
                      </div>
                      <div class="d-flex flex-column">
                        <h6 class="mb-1 text-dark text-sm">{{ item.label }}</h6>
                        <span class="text-xs">{{ item.value }} <span class="font-weight-bold">{{ item.label }}</span></span>
                      </div>
                    </div>
                    <div class="d-flex">
                      <button class="btn btn-link btn-icon-only btn-rounded btn-sm text-dark icon-move-right my-auto">
                        <i class="ni ni-bold-right"></i>
                      </button>
                    </div>
                  </li>
                </ul>
              </div>
            </div>
          </div>
        </div>

        <!-- Affiliates Chart -->
        <div class="row mt-4">
          <div class="col-lg-7 mb-lg-0 mb-4">
            <div class="card">
              <div class="card-header pb-0 p-3">
                <h6 class="mb-0">Affiliates Program</h6>
                <p class="text-sm mb-0">Top referral sources</p>
              </div>
              <div class="card-body p-3">
                <div class="row">
                  <div class="col-lg-6">
                    <div class="d-flex flex-column h-100">
                      <ClientOnly>
                        <DoughnutChart 
                          :labels="affiliatesLabels" 
                          :data="affiliatesData" 
                          :colors="affiliatesColors" 
                          :height="220" 
                          :cutout="60" 
                        />
                        <template #fallback>
                          <div style="height: 220px"></div>
                        </template>
                      </ClientOnly>
                    </div>
                  </div>
                  <div class="col-lg-6 d-flex flex-column justify-content-center">
                    <div class="table-responsive">
                      <table class="table align-items-center mb-0">
                        <tbody>
                          <tr v-for="(label, index) in affiliatesLabels" :key="label">
                            <td>
                              <div class="d-flex px-2 py-1">
                                <div>
                                  <div class="icon icon-shape icon-xs rounded-circle shadow text-center" :style="{ backgroundColor: affiliatesColors[index] }">
                                  </div>
                                </div>
                                <div class="d-flex flex-column justify-content-center ms-2">
                                  <h6 class="mb-0 text-xs">{{ label }}</h6>
                                </div>
                              </div>
                            </td>
                            <td class="align-middle text-center text-sm">
                              <span class="text-xs font-weight-bold">{{ affiliatesData[index] }}%</span>
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
        </div>
      </div>
    </div>
  </div>
</template>
