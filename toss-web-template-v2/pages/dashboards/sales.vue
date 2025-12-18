<template>
  <div>
    <!-- Stats Row -->
    <div class="row">
      <div class="col-lg-7">
        <div class="row">
          <div class="col-lg-6 col-md-6 col-12">
            <div class="card mb-4">
              <MDBox variant="gradient" bg-color="dark" color="white" :p="3" class="border-radius-lg">
                <div class="row">
                  <div class="col-8">
                    <div class="numbers">
                      <MDTypography variant="caption" color="white" font-weight="bold">
                        Revenue
                      </MDTypography>
                      <MDTypography variant="h5" color="white" font-weight="bold">
                        $230,220
                      </MDTypography>
                    </div>
                  </div>
                  <div class="col-4 text-end">
                    <Icon name="material-symbols:store-outline" class="text-white text-3xl opacity-70" />
                  </div>
                </div>
                <MDTypography variant="button" color="white" class="mt-3">
                  <span class="font-weight-bold">+23%</span> since last month
                </MDTypography>
              </MDBox>
            </div>
            <div class="card">
              <MDBox variant="gradient" bg-color="primary" color="white" :p="3" class="border-radius-lg">
                <div class="row">
                  <div class="col-8">
                    <div class="numbers">
                      <MDTypography variant="caption" color="white" font-weight="bold">
                        Orders
                      </MDTypography>
                      <MDTypography variant="h5" color="white" font-weight="bold">
                        3,550
                      </MDTypography>
                    </div>
                  </div>
                  <div class="col-4 text-end">
                    <Icon name="material-symbols:shopping-bag-outline" class="text-white text-3xl opacity-70" />
                  </div>
                </div>
                <MDTypography variant="button" color="white" class="mt-3">
                  <span class="font-weight-bold">+12%</span> since last month
                </MDTypography>
              </MDBox>
            </div>
          </div>
          
          <div class="col-lg-6 col-md-6 col-12">
            <div class="card mb-4">
              <MDBox variant="gradient" bg-color="success" color="white" :p="3" class="border-radius-lg">
                <div class="row">
                  <div class="col-8">
                    <div class="numbers">
                      <MDTypography variant="caption" color="white" font-weight="bold">
                        Customers
                      </MDTypography>
                      <MDTypography variant="h5" color="white" font-weight="bold">
                        12,389
                      </MDTypography>
                    </div>
                  </div>
                  <div class="col-4 text-end">
                    <Icon name="material-symbols:group-outline" class="text-white text-3xl opacity-70" />
                  </div>
                </div>
                <MDTypography variant="button" color="white" class="mt-3">
                  <span class="font-weight-bold">+8%</span> since yesterday
                </MDTypography>
              </MDBox>
            </div>
            <div class="card">
              <MDBox variant="gradient" bg-color="info" color="white" :p="3" class="border-radius-lg">
                <div class="row">
                  <div class="col-8">
                    <div class="numbers">
                      <MDTypography variant="caption" color="white" font-weight="bold">
                        Avg. Revenue
                      </MDTypography>
                      <MDTypography variant="h5" color="white" font-weight="bold">
                        $17,321
                      </MDTypography>
                    </div>
                  </div>
                  <div class="col-4 text-end">
                    <Icon name="material-symbols:trending-up" class="text-white text-3xl opacity-70" />
                  </div>
                </div>
                <MDTypography variant="button" color="white" class="mt-3">
                  <span class="font-weight-bold">+15%</span> since last week
                </MDTypography>
              </MDBox>
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-5">
        <div class="card">
          <div class="card-header pb-0 p-3">
            <MDTypography variant="h6">Sales by Country</MDTypography>
            <MDTypography variant="button" color="text" font-weight="regular">
              Performance by region
            </MDTypography>
          </div>
          <div class="card-body p-3">
            <div class="row">
              <div class="col-md-6 mb-3" v-for="country in salesByCountry" :key="country.name">
                <div class="d-flex align-items-center">
                  <Icon :name="country.flag" class="text-2xl me-2" />
                  <div>
                    <MDTypography variant="button" font-weight="medium">{{ country.name }}</MDTypography>
                    <MDTypography variant="caption" color="text">{{ country.sales }} sales</MDTypography>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Revenue Chart -->
    <div class="row mt-4">
      <div class="col-12">
        <div class="card">
          <div class="card-header pb-0 p-3">
            <div class="d-flex align-items-center justify-content-between">
              <MDTypography variant="h6">Revenue</MDTypography>
              <MDBadge color="success" variant="contained" size="sm">Last 12 months</MDBadge>
            </div>
          </div>
          <div class="card-body p-3">
            <div class="chart">
              <canvas id="revenue-chart" height="300"></canvas>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Top Selling Products -->
    <div class="row mt-4">
      <div class="col-12">
        <div class="card">
          <div class="card-header pb-0">
            <MDTypography variant="h6">Top Selling Products</MDTypography>
          </div>
          <div class="card-body px-0 pb-2">
            <div class="table-responsive">
              <table class="table align-items-center mb-0">
                <thead>
                  <tr>
                    <th class="text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">Product</th>
                    <th class="text-uppercase text-secondary text-xxs font-weight-bolder opacity-7 ps-2">Value</th>
                    <th class="text-center text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">Ads Spent</th>
                    <th class="text-center text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">Refunds</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="product in topProducts" :key="product.name">
                    <td>
                      <div class="d-flex px-2 py-1">
                        <div class="d-flex flex-column justify-content-center">
                          <MDTypography variant="button" font-weight="medium">{{ product.name }}</MDTypography>
                          <MDTypography variant="caption" color="text">{{ product.category }}</MDTypography>
                        </div>
                      </div>
                    </td>
                    <td>
                      <MDTypography variant="caption" font-weight="medium">{{ product.value }}</MDTypography>
                    </td>
                    <td class="align-middle text-center text-sm">
                      <MDTypography variant="caption" color="text">{{ product.adsSpent }}</MDTypography>
                    </td>
                    <td class="align-middle text-center">
                      <MDTypography variant="caption" :color="product.refunds > 20 ? 'error' : 'text'">
                        {{ product.refunds }}
                      </MDTypography>
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
import { ref } from 'vue'

definePageMeta({
  layout: 'default'
})

const salesByCountry = ref([
  { name: 'United States', flag: 'flag:us-4x3', sales: '2,500' },
  { name: 'Germany', flag: 'flag:de-4x3', sales: '3,900' },
  { name: 'Great Britain', flag: 'flag:gb-4x3', sales: '1,400' },
  { name: 'Brasil', flag: 'flag:br-4x3', sales: '562' }
])

const topProducts = ref([
  { name: 'Nike Sport V2', category: 'Clothes', value: '$8,752', adsSpent: '$4,212', refunds: 13 },
  { name: 'Wireless Headphones', category: 'Electronics', value: '$9,521', adsSpent: '$3,201', refunds: 8 },
  { name: 'Mountain Bike', category: 'Sport', value: '$7,321', adsSpent: '$2,141', refunds: 25 },
  { name: 'Business Kit', category: 'Office', value: '$4,215', adsSpent: '$1,502', refunds: 5 },
  { name: 'Black Chair', category: 'Furniture', value: '$2,143', adsSpent: '$921', refunds: 2 }
])
</script>
