<script setup lang="ts">
import { ref } from 'vue'
import BarChart from '~/components/charts/BarChart.vue'
import LineChart from '~/components/charts/LineChart.vue'
import DoughnutChart from '~/components/charts/DoughnutChart.vue'

// Set page title
useHead({
  title: 'Dashboard - TOSS'
})

// Mock data for demonstration
const stats = ref({
  todaySales: 15420,
  cashIn: 12300,
  cashOut: 4500,
  lowStock: 8
})

// Chart data based on Material Dashboard template
const salesTrendLabels = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']
const salesTrendData = [50, 40, 300, 220, 500, 250, 400]

const dailySalesLabels = ['Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
const dailySalesDatasets = [
  {
    label: 'Organic Search',
    data: [50, 40, 300, 220, 500, 250, 400, 230, 500],
    borderColor: '#e91e63',
    backgroundColor: 'transparent',
    tension: 0.4,
    pointRadius: 2
  },
  {
    label: 'Referral',
    data: [30, 90, 40, 140, 290, 290, 340, 230, 400],
    borderColor: '#3A416F',
    backgroundColor: 'transparent',
    tension: 0.4,
    pointRadius: 2
  },
  {
    label: 'Direct',
    data: [40, 80, 70, 90, 30, 90, 140, 130, 200],
    borderColor: '#03A9F4',
    backgroundColor: 'transparent',
    tension: 0.4,
    pointRadius: 2
  }
]

const salesOverviewLabels = ['Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
const salesOverviewData = [50, 100, 200, 190, 400, 350, 500, 450, 700]

const affiliatesLabels = ['Creative Tim', 'Github', 'Bootsnipp', 'Dev.to', 'Codeinwp']
const affiliatesData = [15, 20, 12, 60, 20]
const affiliatesColors = ['#03A9F4', '#3A416F', '#fb8c00', '#a8b8d8', '#e91e63']
</script>

<template>
  <div class="py-6 pb-12">
    <!-- Page Header -->
    <div class="mb-8">
      <h3 class="text-3xl font-bold text-gray-900 mb-2">Analytics</h3>
      <p class="text-gray-600 text-sm">
        Check the sales, value and bounce rate by country.
      </p>
    </div>

    <!-- Chart Cards Row -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
      <!-- Today's Sales - Bar Chart -->
      <div class="bg-white rounded-xl shadow-sm hover:shadow-md transition-shadow">
        <div class="p-6 pb-4">
          <h6 class="text-base font-semibold text-gray-900 mb-1">Today's Sales</h6>
          <p class="text-sm text-gray-600 mb-4">Last Campaign Performance</p>
          <div class="mt-2">
            <ClientOnly>
              <BarChart 
                :labels="salesTrendLabels" 
                :data="salesTrendData"
                backgroundColor="#3A416F"
                :height="176"
              />
              <template #fallback>
                <div class="h-44 flex items-center justify-center text-gray-400">Loading chart...</div>
              </template>
            </ClientOnly>
          </div>
        </div>
        <hr class="border-gray-200 my-0">
        <div class="px-6 py-3 flex items-center">
          <i class="material-symbols-rounded text-sm text-gray-600 mr-2">schedule</i>
          <p class="text-sm text-gray-600 mb-0">campaign sent 2 days ago</p>
        </div>
      </div>

      <!-- Daily Sales - Multi-line Chart -->
      <div class="bg-white rounded-xl shadow-sm hover:shadow-md transition-shadow">
        <div class="p-6 pb-4">
          <h6 class="text-base font-semibold text-gray-900 mb-1">Daily Sales</h6>
          <p class="text-sm text-gray-600 mb-4">
            (<span class="font-bold">+15%</span>) increase in today sales.
          </p>
          <div class="mt-2">
            <ClientOnly>
              <LineChart 
                :labels="dailySalesLabels" 
                :datasets="dailySalesDatasets"
                :height="176"
              />
              <template #fallback>
                <div class="h-44 flex items-center justify-center text-gray-400">Loading chart...</div>
              </template>
            </ClientOnly>
          </div>
        </div>
        <hr class="border-gray-200 my-0">
        <div class="px-6 py-3 flex items-center">
          <i class="material-symbols-rounded text-sm text-gray-600 mr-2">schedule</i>
          <p class="text-sm text-gray-600 mb-0">updated 4 min ago</p>
        </div>
      </div>

      <!-- Sales Overview - Line Chart with Fill -->
      <div class="bg-white rounded-xl shadow-sm hover:shadow-md transition-shadow">
        <div class="p-6 pb-4">
          <h6 class="text-base font-semibold text-gray-900 mb-1">Sales Overview</h6>
          <p class="text-sm text-gray-600 mb-4">
            <span class="font-bold text-green-600">+4%</span> more in 2025
          </p>
          <div class="mt-2">
            <ClientOnly>
              <LineChart 
                :labels="salesOverviewLabels" 
                :datasets="[{
                  label: 'Sales',
                  data: salesOverviewData,
                  borderColor: '#0ea5e9',
                  backgroundColor: 'rgba(14, 165, 233, 0.1)',
                  fill: true,
                  tension: 0.4,
                  pointRadius: 0
                }]"
                :height="176"
              />
              <template #fallback>
                <div class="h-44 flex items-center justify-center text-gray-400">Loading chart...</div>
              </template>
            </ClientOnly>
          </div>
        </div>
        <hr class="border-gray-200 my-0">
        <div class="px-6 py-3 flex items-center">
          <i class="material-symbols-rounded text-sm text-gray-600 mr-2">schedule</i>
          <p class="text-sm text-gray-600 mb-0">just updated</p>
        </div>
      </div>
    </div>

    <!-- Stats Cards Row -->
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
      <!-- Today's Sales -->
      <div class="bg-white rounded-xl shadow-sm">
        <div class="p-3 px-4">
          <div class="flex justify-between items-start">
            <div class="flex-1">
              <p class="text-sm text-gray-600 mb-0 capitalize">Today's Sales</p>
              <h4 class="text-2xl font-bold text-gray-900 mb-0">R {{ stats.todaySales.toLocaleString() }}</h4>
            </div>
            <div class="w-12 h-12 rounded-lg bg-gradient-to-br from-gray-800 to-gray-900 shadow-lg flex items-center justify-center">
              <i class="material-symbols-rounded text-white opacity-90">payments</i>
            </div>
          </div>
        </div>
        <hr class="border-gray-200 my-0">
        <div class="px-4 py-2">
          <p class="text-sm mb-0">
            <span class="text-green-600 font-bold">+55% </span>
            <span class="text-gray-600">than last week</span>
          </p>
        </div>
      </div>

      <!-- Money In -->
      <div class="bg-white rounded-xl shadow-sm">
        <div class="p-3 px-4">
          <div class="flex justify-between items-start">
            <div class="flex-1">
              <p class="text-sm text-gray-600 mb-0 capitalize">Money In</p>
              <h4 class="text-2xl font-bold text-gray-900 mb-0">R {{ stats.cashIn.toLocaleString() }}</h4>
            </div>
            <div class="w-12 h-12 rounded-lg bg-gradient-to-br from-gray-800 to-gray-900 shadow-lg flex items-center justify-center">
              <i class="material-symbols-rounded text-white opacity-90">trending_up</i>
            </div>
          </div>
        </div>
        <hr class="border-gray-200 my-0">
        <div class="px-4 py-2">
          <p class="text-sm mb-0">
            <span class="text-green-600 font-bold">+3% </span>
            <span class="text-gray-600">than last month</span>
          </p>
        </div>
      </div>

      <!-- Money Out -->
      <div class="bg-white rounded-xl shadow-sm">
        <div class="p-3 px-4">
          <div class="flex justify-between items-start">
            <div class="flex-1">
              <p class="text-sm text-gray-600 mb-0 capitalize">Money Out</p>
              <h4 class="text-2xl font-bold text-gray-900 mb-0">R {{ stats.cashOut.toLocaleString() }}</h4>
            </div>
            <div class="w-12 h-12 rounded-lg bg-gradient-to-br from-gray-800 to-gray-900 shadow-lg flex items-center justify-center">
              <i class="material-symbols-rounded text-white opacity-90">trending_down</i>
            </div>
          </div>
        </div>
        <hr class="border-gray-200 my-0">
        <div class="px-4 py-2">
          <p class="text-sm mb-0">
            <span class="text-green-600 font-bold">+35% </span>
            <span class="text-gray-600">than last month</span>
          </p>
        </div>
      </div>

      <!-- Low Stock Items -->
      <div class="bg-white rounded-xl shadow-sm">
        <div class="p-3 px-4">
          <div class="flex justify-between items-start">
            <div class="flex-1">
              <p class="text-sm text-gray-600 mb-0 capitalize">Low Stock</p>
              <h4 class="text-2xl font-bold text-gray-900 mb-0">{{ stats.lowStock }}</h4>
            </div>
            <div class="w-12 h-12 rounded-lg bg-gradient-to-br from-gray-800 to-gray-900 shadow-lg flex items-center justify-center">
              <i class="material-symbols-rounded text-white opacity-90">inventory_2</i>
            </div>
          </div>
        </div>
        <hr class="border-gray-200 my-0">
        <div class="px-4 py-2">
          <p class="text-sm text-gray-600 mb-0">Just updated</p>
        </div>
      </div>
    </div>

    <!-- Sales by Country / Products Section -->
    <div class="grid grid-cols-1 lg:grid-cols-12 gap-6 mb-8">
      <!-- Sales Table (left side) -->
      <div class="lg:col-span-7">
        <div class="bg-white rounded-xl shadow-sm">
          <div class="p-6 pb-0">
            <h6 class="text-base font-semibold mb-0">Top Selling Products</h6>
            <p class="text-sm text-gray-600 mb-4">
              Check the sales, value and stock by product.
            </p>
          </div>
          <div class="p-6 pt-3">
            <div class="overflow-x-auto">
              <table class="w-full">
                <thead>
                  <tr class="border-b border-gray-200">
                    <th class="text-left text-xs font-semibold text-gray-600 uppercase pb-3">Product</th>
                    <th class="text-left text-xs font-semibold text-gray-600 uppercase pb-3">Sales</th>
                    <th class="text-left text-xs font-semibold text-gray-600 uppercase pb-3">Value</th>
                    <th class="text-left text-xs font-semibold text-gray-600 uppercase pb-3">Stock</th>
                  </tr>
                </thead>
                <tbody>
                  <tr class="border-b border-gray-100">
                    <td class="py-3">
                      <div class="flex items-center">
                        <div class="w-10 h-10 bg-gray-200 rounded-lg mr-3"></div>
                        <h6 class="text-sm font-normal mb-0">2L Coke</h6>
                      </div>
                    </td>
                    <td class="text-sm font-normal">45</td>
                    <td class="text-sm font-normal">R 450</td>
                    <td class="text-sm font-normal">120</td>
                  </tr>
                  <tr class="border-b border-gray-100">
                    <td class="py-3">
                      <div class="flex items-center">
                        <div class="w-10 h-10 bg-gray-200 rounded-lg mr-3"></div>
                        <h6 class="text-sm font-normal mb-0">White Bread</h6>
                      </div>
                    </td>
                    <td class="text-sm font-normal">89</td>
                    <td class="text-sm font-normal">R 890</td>
                    <td class="text-sm font-normal text-red-600">8</td>
                  </tr>
                  <tr class="border-b border-gray-100">
                    <td class="py-3">
                      <div class="flex items-center">
                        <div class="w-10 h-10 bg-gray-200 rounded-lg mr-3"></div>
                        <h6 class="text-sm font-normal mb-0">Sugar 2.5kg</h6>
                      </div>
                    </td>
                    <td class="text-sm font-normal">23</td>
                    <td class="text-sm font-normal">R 575</td>
                    <td class="text-sm font-normal">45</td>
                  </tr>
                  <tr>
                    <td class="py-3">
                      <div class="flex items-center">
                        <div class="w-10 h-10 bg-gray-200 rounded-lg mr-3"></div>
                        <h6 class="text-sm font-normal mb-0">Milk 1L</h6>
                      </div>
                    </td>
                    <td class="text-sm font-normal">67</td>
                    <td class="text-sm font-normal">R 1,340</td>
                    <td class="text-sm font-normal">34</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>

      <!-- Quick Actions (right side) -->
      <div class="lg:col-span-5">
        <div class="bg-white rounded-xl shadow-sm p-6">
          <h6 class="text-base font-semibold mb-4">Quick Actions</h6>
          <div class="grid grid-cols-2 gap-3">
            <button class="bg-gradient-to-br from-gray-800 to-gray-900 hover:from-gray-900 hover:to-gray-950 text-white rounded-lg p-6 flex flex-col items-center justify-center gap-3 transition-all shadow-md hover:shadow-lg">
              <i class="material-symbols-rounded text-3xl">add_shopping_cart</i>
              <span class="text-sm font-medium">New Sale</span>
            </button>
            
            <button class="bg-gradient-to-br from-green-500 to-green-600 hover:from-green-600 hover:to-green-700 text-white rounded-lg p-6 flex flex-col items-center justify-center gap-3 transition-all shadow-md hover:shadow-lg">
              <i class="material-symbols-rounded text-3xl">inventory</i>
              <span class="text-sm font-medium">Receive Stock</span>
            </button>
            
            <button class="bg-gradient-to-br from-orange-500 to-orange-600 hover:from-orange-600 hover:to-orange-700 text-white rounded-lg p-6 flex flex-col items-center justify-center gap-3 transition-all shadow-md hover:shadow-lg">
              <i class="material-symbols-rounded text-3xl">account_balance_wallet</i>
              <span class="text-sm font-medium">Pay Supplier</span>
            </button>
            
            <button class="bg-gradient-to-br from-purple-500 to-purple-600 hover:from-purple-600 hover:to-purple-700 text-white rounded-lg p-6 flex flex-col items-center justify-center gap-3 transition-all shadow-md hover:shadow-lg">
              <i class="material-symbols-rounded text-3xl">person_add</i>
              <span class="text-sm font-medium">Add Customer</span>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Bottom Section: Active Users & Sales Overview -->
    <div class="grid grid-cols-1 lg:grid-cols-12 gap-6">
      <!-- Active Users Card with Bar Chart -->
      <div class="lg:col-span-5">
        <div class="bg-white rounded-xl shadow-sm">
          <div class="p-6 pb-0">
            <div class="bg-gradient-to-br from-gray-800 to-gray-900 rounded-xl shadow-lg -mt-10 mb-6 p-6">
              <ClientOnly>
                <BarChart 
                  :labels="['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun']"
                  :data="[300, 230, 224, 218, 156, 200, 330]"
                  backgroundColor="rgba(255, 255, 255, 0.8)"
                  :height="176"
                />
                <template #fallback>
                  <div class="h-44 flex items-center justify-center text-white/50">Loading chart...</div>
                </template>
              </ClientOnly>
            </div>

            <h6 class="ms-2 mb-0 font-semibold">Active Users</h6>
            <p class="text-sm text-gray-600 ms-2">
              (<span class="font-bold text-gray-900">+11%</span>) than last week
            </p>

            <div class="grid grid-cols-4 gap-2 mt-4 px-2 pb-4">
              <div>
                <div class="flex items-center mb-2">
                  <div class="w-6 h-6 rounded bg-gradient-to-br from-gray-800 to-gray-900 flex items-center justify-center mr-2">
                    <i class="material-symbols-rounded text-white text-xs opacity-90">groups</i>
                  </div>
                  <p class="text-xs font-bold mb-0">Users</p>
                </div>
                <h4 class="text-lg font-bold">42K</h4>
                <div class="w-3/4 bg-gray-200 rounded-full h-1">
                  <div class="bg-gray-800 h-1 rounded-full" style="width: 60%"></div>
                </div>
              </div>

              <div>
                <div class="flex items-center mb-2">
                  <div class="w-6 h-6 rounded bg-gradient-to-br from-gray-800 to-gray-900 flex items-center justify-center mr-2">
                    <i class="material-symbols-rounded text-white text-xs opacity-90">ads_click</i>
                  </div>
                  <p class="text-xs font-bold mb-0">Clicks</p>
                </div>
                <h4 class="text-lg font-bold">1.7m</h4>
                <div class="w-3/4 bg-gray-200 rounded-full h-1">
                  <div class="bg-gray-800 h-1 rounded-full" style="width: 90%"></div>
                </div>
              </div>

              <div>
                <div class="flex items-center mb-2">
                  <div class="w-6 h-6 rounded bg-gradient-to-br from-orange-500 to-orange-600 flex items-center justify-center mr-2">
                    <i class="material-symbols-rounded text-white text-xs opacity-90">receipt</i>
                  </div>
                  <p class="text-xs font-bold mb-0">Sales</p>
                </div>
                <h4 class="text-lg font-bold">R 399</h4>
                <div class="w-3/4 bg-gray-200 rounded-full h-1">
                  <div class="bg-gray-800 h-1 rounded-full" style="width: 30%"></div>
                </div>
              </div>

              <div>
                <div class="flex items-center mb-2">
                  <div class="w-6 h-6 rounded bg-gradient-to-br from-red-500 to-red-600 flex items-center justify-center mr-2">
                    <i class="material-symbols-rounded text-white text-xs opacity-90">category</i>
                  </div>
                  <p class="text-xs font-bold mb-0">Items</p>
                </div>
                <h4 class="text-lg font-bold">74</h4>
                <div class="w-3/4 bg-gray-200 rounded-full h-1">
                  <div class="bg-gray-800 h-1 rounded-full" style="width: 50%"></div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Affiliates Program - Doughnut Chart (Right) -->
      <div class="lg:col-span-7">
        <div class="bg-white rounded-xl shadow-sm">
          <div class="p-6 pb-0">
            <div class="bg-gradient-to-br from-orange-500 to-orange-600 rounded-xl shadow-lg -mt-10 mb-6 p-6">
              <div class="flex items-center justify-center">
                <div class="w-64">
                  <ClientOnly>
                    <DoughnutChart 
                      :labels="affiliatesLabels"
                      :data="affiliatesData"
                      :colors="affiliatesColors"
                      :height="176"
                      :cutout="60"
                    />
                    <template #fallback>
                      <div class="h-44 flex items-center justify-center text-white/50">Loading chart...</div>
                    </template>
                  </ClientOnly>
                </div>
              </div>
            </div>

            <h6 class="ms-2 mb-0 font-semibold">Affiliates Program</h6>
            <p class="text-sm text-gray-600 ms-2 mb-4">
              Top referral sources
            </p>

            <div class="px-2 pb-4">
              <table class="w-full">
                <tbody>
                  <tr v-for="(label, index) in affiliatesLabels" :key="index" class="border-b border-gray-100">
                    <td class="py-3">
                      <div class="flex items-center">
                        <div 
                          class="w-3 h-3 rounded-full mr-3"
                          :style="{ backgroundColor: affiliatesColors[index] }"
                        ></div>
                        <h6 class="text-sm font-normal mb-0">{{ label }}</h6>
                      </div>
                    </td>
                    <td class="text-sm font-bold text-right">{{ affiliatesData[index] }}%</td>
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

<style>
.material-symbols-rounded {
  font-variation-settings: 'FILL' 0, 'wght' 400, 'GRAD' 0, 'opsz' 24;
}
</style>
