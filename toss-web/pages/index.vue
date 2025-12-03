<script setup lang="ts">
import { ref } from 'vue'

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

const salesTrendData = ref([
  { day: 'M', amount: 50 },
  { day: 'T', amount: 60 },
  { day: 'W', amount: 45 },
  { day: 'T', amount: 55 },
  { day: 'F', amount: 70 },
  { day: 'S', amount: 85 },
  { day: 'S', amount: 65 }
])

const dailySalesData = ref([
  { point: 0, value: 40 },
  { point: 1, value: 60 },
  { point: 2, value: 50 },
  { point: 3, value: 80 },
  { point: 4, value: 70 },
  { point: 5, value: 90 },
  { point: 6, value: 85 }
])

const completedTasksData = ref([
  { point: 0, value: 60 },
  { point: 1, value: 70 },
  { point: 2, value: 65 },
  { point: 3, value: 75 },
  { point: 4, value: 80 },
  { point: 5, value: 85 },
  { point: 6, value: 90 }
])
</script>

<template>
  <div class="py-2">
    <!-- Page Header -->
    <div class="ms-3 mb-4">
      <h3 class="text-3xl font-bold text-gray-900 mb-1">Analytics</h3>
      <p class="text-gray-600 text-sm">
        Check the sales, value and bounce rate by country.
      </p>
    </div>

    <!-- Chart Cards Row -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-4">
      <!-- Website Views / Sales Trend -->
      <div class="bg-white rounded-xl shadow-sm">
        <div class="p-6">
          <h6 class="text-base font-semibold text-gray-900 mb-0">Today's Sales</h6>
          <p class="text-sm text-gray-600">Last Campaign Performance</p>
          <div class="mt-4">
            <!-- Bar Chart -->
            <div class="h-44 flex items-end justify-between gap-1 px-2">
              <div
                v-for="(item, index) in salesTrendData"
                :key="index"
                class="flex-1 flex flex-col items-center group relative"
              >
                <div
                  class="w-full bg-gradient-to-t from-blue-600 to-blue-500 rounded-t transition-all hover:from-blue-700 hover:to-blue-600"
                  :style="{ height: `${item.amount}%` }"
                >
                </div>
                <p class="text-xs text-gray-600 mt-2">{{ item.day }}</p>
              </div>
            </div>
          </div>
        </div>
        <hr class="border-gray-200 my-0">
        <div class="px-6 py-3 flex items-center">
          <i class="material-symbols-rounded text-sm text-gray-600 mr-2">schedule</i>
          <p class="text-sm text-gray-600 mb-0">campaign sent 2 days ago</p>
        </div>
      </div>

      <!-- Daily Sales -->
      <div class="bg-white rounded-xl shadow-sm">
        <div class="p-6">
          <h6 class="text-base font-semibold text-gray-900 mb-0">Daily Sales</h6>
          <p class="text-sm text-gray-600">
            (<span class="font-bold">+15%</span>) increase in today sales.
          </p>
          <div class="mt-4">
            <!-- Line Chart -->
            <div class="h-44 relative">
              <svg class="w-full h-full" viewBox="0 0 100 100" preserveAspectRatio="none">
                <defs>
                  <linearGradient id="salesGradient" x1="0%" y1="0%" x2="0%" y2="100%">
                    <stop offset="0%" style="stop-color:#10b981;stop-opacity:0.3" />
                    <stop offset="100%" style="stop-color:#10b981;stop-opacity:0" />
                  </linearGradient>
                </defs>
                <path
                  d="M 0,60 L 15,40 L 30,50 L 45,20 L 60,30 L 75,10 L 90,15 L 100,15"
                  fill="none"
                  stroke="#10b981"
                  stroke-width="2"
                  vector-effect="non-scaling-stroke"
                />
                <path
                  d="M 0,60 L 15,40 L 30,50 L 45,20 L 60,30 L 75,10 L 90,15 L 100,15 L 100,100 L 0,100 Z"
                  fill="url(#salesGradient)"
                />
              </svg>
            </div>
          </div>
        </div>
        <hr class="border-gray-200 my-0">
        <div class="px-6 py-3 flex items-center">
          <i class="material-symbols-rounded text-sm text-gray-600 mr-2">schedule</i>
          <p class="text-sm text-gray-600 mb-0">updated 4 min ago</p>
        </div>
      </div>

      <!-- Completed Tasks / Orders -->
      <div class="bg-white rounded-xl shadow-sm">
        <div class="p-6">
          <h6 class="text-base font-semibold text-gray-900 mb-0">Pending Orders</h6>
          <p class="text-sm text-gray-600">Last Campaign Performance</p>
          <div class="mt-4">
            <!-- Line Chart -->
            <div class="h-44 relative">
              <svg class="w-full h-full" viewBox="0 0 100 100" preserveAspectRatio="none">
                <defs>
                  <linearGradient id="ordersGradient" x1="0%" y1="0%" x2="0%" y2="100%">
                    <stop offset="0%" style="stop-color:#6366f1;stop-opacity:0.3" />
                    <stop offset="100%" style="stop-color:#6366f1;stop-opacity:0" />
                  </linearGradient>
                </defs>
                <path
                  d="M 0,40 L 15,35 L 30,45 L 45,30 L 60,42 L 75,28 L 90,38 L 100,35"
                  fill="none"
                  stroke="#6366f1"
                  stroke-width="2"
                  vector-effect="non-scaling-stroke"
                />
                <path
                  d="M 0,40 L 15,35 L 30,45 L 45,30 L 60,42 L 75,28 L 90,38 L 100,35 L 100,100 L 0,100 Z"
                  fill="url(#ordersGradient)"
                />
              </svg>
            </div>
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
    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-4">
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
    <div class="grid grid-cols-1 lg:grid-cols-12 gap-4 mb-4">
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
            <button class="bg-gradient-to-br from-blue-500 to-blue-600 hover:from-blue-600 hover:to-blue-700 text-white rounded-lg p-6 flex flex-col items-center justify-center gap-3 transition-all shadow-md hover:shadow-lg">
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
    <div class="grid grid-cols-1 lg:grid-cols-12 gap-4">
      <!-- Active Users Card (Left) -->
      <div class="lg:col-span-5">
        <div class="bg-white rounded-xl shadow-sm">
          <div class="p-6 pb-0">
            <div class="bg-gradient-to-br from-gray-800 to-gray-900 rounded-xl shadow-lg -mt-10 mb-6 p-4">
              <div class="h-44 flex items-end justify-between gap-2 px-4">
                <div class="flex-1 flex flex-col items-center">
                  <div class="w-full bg-white/30 rounded-t h-[60%]"></div>
                </div>
                <div class="flex-1 flex flex-col items-center">
                  <div class="w-full bg-white/30 rounded-t h-[75%]"></div>
                </div>
                <div class="flex-1 flex flex-col items-center">
                  <div class="w-full bg-white/30 rounded-t h-[50%]"></div>
                </div>
                <div class="flex-1 flex flex-col items-center">
                  <div class="w-full bg-white/30 rounded-t h-[85%]"></div>
                </div>
                <div class="flex-1 flex flex-col items-center">
                  <div class="w-full bg-white/30 rounded-t h-[65%]"></div>
                </div>
                <div class="flex-1 flex flex-col items-center">
                  <div class="w-full bg-white/30 rounded-t h-[90%]"></div>
                </div>
                <div class="flex-1 flex flex-col items-center">
                  <div class="w-full bg-white/30 rounded-t h-[70%]"></div>
                </div>
              </div>
            </div>

            <h6 class="ms-2 mb-0 font-semibold">Active Users</h6>
            <p class="text-sm text-gray-600 ms-2">
              (<span class="font-bold text-gray-900">+11%</span>) than last week
            </p>

            <div class="grid grid-cols-4 gap-2 mt-4 px-2">
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
                  <div class="w-6 h-6 rounded bg-gradient-to-br from-blue-500 to-blue-600 flex items-center justify-center mr-2">
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

      <!-- Sales Overview (Right) -->
      <div class="lg:col-span-7">
        <div class="bg-white rounded-xl shadow-sm p-6">
          <div class="mb-4">
            <h6 class="text-base font-semibold mb-1">Sales overview</h6>
            <p class="text-sm text-gray-600">
              <i class="fa fa-arrow-up text-success"></i>
              <span class="font-bold">4% more</span> in 2021
            </p>
          </div>
          <div class="h-72 relative">
            <svg class="w-full h-full" viewBox="0 0 100 100" preserveAspectRatio="none">
              <defs>
                <linearGradient id="overviewGradient" x1="0%" y1="0%" x2="0%" y2="100%">
                  <stop offset="0%" style="stop-color:#0ea5e9;stop-opacity:0.3" />
                  <stop offset="100%" style="stop-color:#0ea5e9;stop-opacity:0" />
                </linearGradient>
              </defs>
              <!-- Grid lines -->
              <line x1="0" y1="25" x2="100" y2="25" stroke="#e5e7eb" stroke-width="0.5" vector-effect="non-scaling-stroke" />
              <line x1="0" y1="50" x2="100" y2="50" stroke="#e5e7eb" stroke-width="0.5" vector-effect="non-scaling-stroke" />
              <line x1="0" y1="75" x2="100" y2="75" stroke="#e5e7eb" stroke-width="0.5" vector-effect="non-scaling-stroke" />
              
              <!-- Line chart -->
              <path
                d="M 0,70 L 10,60 L 20,65 L 30,45 L 40,50 L 50,35 L 60,40 L 70,30 L 80,35 L 90,25 L 100,30"
                fill="none"
                stroke="#0ea5e9"
                stroke-width="3"
                vector-effect="non-scaling-stroke"
              />
              <path
                d="M 0,70 L 10,60 L 20,65 L 30,45 L 40,50 L 50,35 L 60,40 L 70,30 L 80,35 L 90,25 L 100,30 L 100,100 L 0,100 Z"
                fill="url(#overviewGradient)"
              />
            </svg>
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
