<template>
  <div class="container-fluid py-4">
    <div class="row">
      <div class="col-12">
        <div class="card">
          <div class="card-header pb-0">
            <div class="d-flex justify-content-between align-items-center">
              <MDTypography variant="h6" font-weight="medium">
                Orders List
              </MDTypography>
              <MDButton color="info" size="small">
                <Icon name="mdi:download" class="me-1" />
                Export
              </MDButton>
            </div>
          </div>
          <div class="card-body px-0 pt-0 pb-2">
            <div class="table-responsive p-0">
              <table class="table align-items-center mb-0">
                <thead>
                  <tr>
                    <th class="text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">
                      Order ID
                    </th>
                    <th class="text-uppercase text-secondary text-xxs font-weight-bolder opacity-7 ps-2">
                      Customer
                    </th>
                    <th class="text-center text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">
                      Date
                    </th>
                    <th class="text-center text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">
                      Total
                    </th>
                    <th class="text-center text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">
                      Status
                    </th>
                    <th class="text-secondary opacity-7"></th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="order in orders" :key="order.id">
                    <td>
                      <div class="d-flex px-2 py-1">
                        <MDTypography variant="button" font-weight="bold" class="text-info">
                          #{{ order.id }}
                        </MDTypography>
                      </div>
                    </td>
                    <td>
                      <MDTypography variant="caption" font-weight="medium">
                        {{ order.customer }}
                      </MDTypography>
                      <br>
                      <MDTypography variant="caption" color="text">
                        {{ order.email }}
                      </MDTypography>
                    </td>
                    <td class="align-middle text-center">
                      <MDTypography variant="caption" color="text" font-weight="medium">
                        {{ order.date }}
                      </MDTypography>
                    </td>
                    <td class="align-middle text-center">
                      <MDTypography variant="button" font-weight="bold">
                        ${{ order.total.toFixed(2) }}
                      </MDTypography>
                    </td>
                    <td class="align-middle text-center text-sm">
                      <MDBadge 
                        :color="getStatusColor(order.status)" 
                        variant="gradient" 
                        size="sm"
                      >
                        {{ order.status }}
                      </MDBadge>
                    </td>
                    <td class="align-middle">
                      <NuxtLink :to="`/ecommerce/orders/order-details?id=${order.id}`">
                        <MDButton color="info" size="small">
                          View
                        </MDButton>
                      </NuxtLink>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
            <div class="d-flex justify-content-between align-items-center px-3 mt-3">
              <MDTypography variant="caption" color="text">
                Showing {{ orders.length }} entries
              </MDTypography>
              <div class="d-flex gap-1">
                <MDButton 
                  v-for="page in 4" 
                  :key="page" 
                  :color="page === 1 ? 'info' : 'light'"
                  size="small"
                  circular
                >
                  {{ page }}
                </MDButton>
              </div>
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

const orders = ref([
  {
    id: '10421',
    customer: 'John Michael',
    email: 'john@example.com',
    date: '12/18/2024',
    total: 2345.00,
    status: 'delivered'
  },
  {
    id: '10422',
    customer: 'Alexa Liras',
    email: 'alexa@example.com',
    date: '12/17/2024',
    total: 1234.50,
    status: 'processing'
  },
  {
    id: '10423',
    customer: 'Laurent Perrier',
    email: 'laurent@example.com',
    date: '12/16/2024',
    total: 987.25,
    status: 'shipped'
  },
  {
    id: '10424',
    customer: 'Michael Levi',
    email: 'michael@example.com',
    date: '12/15/2024',
    total: 3456.75,
    status: 'pending'
  },
  {
    id: '10425',
    customer: 'Richard Gran',
    email: 'richard@example.com',
    date: '12/14/2024',
    total: 567.00,
    status: 'cancelled'
  },
  {
    id: '10426',
    customer: 'Miriam Eric',
    email: 'miriam@example.com',
    date: '12/13/2024',
    total: 1987.50,
    status: 'delivered'
  }
])

const getStatusColor = (status: string) => {
  const colors: Record<string, string> = {
    delivered: 'success',
    processing: 'info',
    shipped: 'warning',
    pending: 'secondary',
    cancelled: 'error'
  }
  return colors[status] || 'secondary'
}
</script>

<style scoped>
.gap-1 {
  gap: 0.25rem;
}
</style>
