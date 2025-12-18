<template>
  <div class="container-fluid py-4">
    <div class="row">
      <div class="col-12">
        <div class="card">
          <div class="card-header pb-0">
            <div class="d-flex justify-content-between align-items-center">
              <div>
                <MDTypography variant="h4" font-weight="bold">
                  Order #{{ order.id }}
                </MDTypography>
                <MDTypography variant="button" color="text">
                  Placed on {{ order.date }}
                </MDTypography>
              </div>
              <MDBadge 
                :color="getStatusColor(order.status)" 
                variant="gradient" 
                size="lg"
              >
                {{ order.status }}
              </MDBadge>
            </div>
          </div>
          <div class="card-body">
            <div class="row">
              <!-- Customer Information -->
              <div class="col-lg-4 mb-4">
                <MDTypography variant="h6" font-weight="medium" class="mb-3">
                  Customer Information
                </MDTypography>
                <div class="d-flex align-items-center mb-3">
                  <MDAvatar 
                    :src="order.customer.avatar"
                    alt="customer"
                    size="lg"
                    class="me-3"
                  />
                  <div>
                    <MDTypography variant="button" font-weight="bold">
                      {{ order.customer.name }}
                    </MDTypography>
                    <MDTypography variant="caption" color="text" class="d-block">
                      {{ order.customer.email }}
                    </MDTypography>
                  </div>
                </div>
                <MDTypography variant="caption" color="text">
                  <Icon name="mdi:phone" size="16" class="me-1" />
                  {{ order.customer.phone }}
                </MDTypography>
              </div>
              
              <!-- Shipping Address -->
              <div class="col-lg-4 mb-4">
                <MDTypography variant="h6" font-weight="medium" class="mb-3">
                  Shipping Address
                </MDTypography>
                <MDTypography variant="button">
                  {{ order.shipping.street }}
                </MDTypography>
                <MDTypography variant="button" class="d-block">
                  {{ order.shipping.city }}, {{ order.shipping.state }} {{ order.shipping.zip }}
                </MDTypography>
                <MDTypography variant="button" class="d-block">
                  {{ order.shipping.country }}
                </MDTypography>
              </div>
              
              <!-- Payment Method -->
              <div class="col-lg-4 mb-4">
                <MDTypography variant="h6" font-weight="medium" class="mb-3">
                  Payment Method
                </MDTypography>
                <div class="d-flex align-items-center mb-2">
                  <Icon name="mdi:credit-card" size="24" class="text-info me-2" />
                  <MDTypography variant="button">
                    {{ order.payment.type }}
                  </MDTypography>
                </div>
                <MDTypography variant="caption" color="text">
                  {{ order.payment.cardNumber }}
                </MDTypography>
              </div>
            </div>
            
            <hr class="horizontal dark my-4">
            
            <!-- Order Items -->
            <MDTypography variant="h6" font-weight="medium" class="mb-3">
              Order Items
            </MDTypography>
            <div class="table-responsive">
              <table class="table align-items-center mb-0">
                <thead>
                  <tr>
                    <th class="text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">Product</th>
                    <th class="text-center text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">Quantity</th>
                    <th class="text-center text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">Price</th>
                    <th class="text-center text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">Total</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="item in order.items" :key="item.id">
                    <td>
                      <div class="d-flex px-2 py-1">
                        <div>
                          <img :src="item.image" class="avatar avatar-sm me-3" alt="product">
                        </div>
                        <div class="d-flex flex-column justify-content-center">
                          <MDTypography variant="button" font-weight="medium">
                            {{ item.name }}
                          </MDTypography>
                          <MDTypography variant="caption" color="text">
                            SKU: {{ item.sku }}
                          </MDTypography>
                        </div>
                      </div>
                    </td>
                    <td class="align-middle text-center">
                      <MDTypography variant="button" font-weight="medium">
                        {{ item.quantity }}
                      </MDTypography>
                    </td>
                    <td class="align-middle text-center">
                      <MDTypography variant="button" font-weight="medium">
                        ${{ item.price.toFixed(2) }}
                      </MDTypography>
                    </td>
                    <td class="align-middle text-center">
                      <MDTypography variant="button" font-weight="bold">
                        ${{ (item.quantity * item.price).toFixed(2) }}
                      </MDTypography>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
            
            <!-- Order Summary -->
            <div class="row mt-4">
              <div class="col-lg-6 ms-auto">
                <div class="table-responsive">
                  <table class="table">
                    <tbody>
                      <tr>
                        <td class="text-start">
                          <MDTypography variant="button" color="text">
                            Subtotal:
                          </MDTypography>
                        </td>
                        <td class="text-end">
                          <MDTypography variant="button" font-weight="medium">
                            ${{ order.summary.subtotal.toFixed(2) }}
                          </MDTypography>
                        </td>
                      </tr>
                      <tr>
                        <td class="text-start">
                          <MDTypography variant="button" color="text">
                            Shipping:
                          </MDTypography>
                        </td>
                        <td class="text-end">
                          <MDTypography variant="button" font-weight="medium">
                            ${{ order.summary.shipping.toFixed(2) }}
                          </MDTypography>
                        </td>
                      </tr>
                      <tr>
                        <td class="text-start">
                          <MDTypography variant="button" color="text">
                            Tax:
                          </MDTypography>
                        </td>
                        <td class="text-end">
                          <MDTypography variant="button" font-weight="medium">
                            ${{ order.summary.tax.toFixed(2) }}
                          </MDTypography>
                        </td>
                      </tr>
                      <tr>
                        <td class="text-start">
                          <MDTypography variant="h6" font-weight="bold">
                            Total:
                          </MDTypography>
                        </td>
                        <td class="text-end">
                          <MDTypography variant="h6" font-weight="bold" color="success">
                            ${{ order.summary.total.toFixed(2) }}
                          </MDTypography>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
            
            <div class="d-flex justify-content-end mt-4 gap-2">
              <MDButton color="light">
                <Icon name="mdi:printer" class="me-1" />
                Print Invoice
              </MDButton>
              <MDButton color="info">
                <Icon name="mdi:email" class="me-1" />
                Email Customer
              </MDButton>
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

const order = ref({
  id: '10421',
  date: 'December 18, 2024',
  status: 'delivered',
  customer: {
    name: 'John Michael',
    email: 'john@example.com',
    phone: '+1 (239) 816-9029',
    avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-1.jpg'
  },
  shipping: {
    street: '123 Main Street, Apt 4B',
    city: 'New York',
    state: 'NY',
    zip: '10001',
    country: 'United States'
  },
  payment: {
    type: 'Visa',
    cardNumber: '**** **** **** 4242'
  },
  items: [
    {
      id: 1,
      name: 'Premium Wireless Headphones',
      sku: 'WH-1000XM4',
      image: 'https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=100',
      quantity: 2,
      price: 349.99
    },
    {
      id: 2,
      name: 'Smart Watch Series 7',
      sku: 'SW-S7-44',
      image: 'https://images.unsplash.com/photo-1523275335684-37898b6baf30?auto=format&fit=crop&w=100',
      quantity: 1,
      price: 399.00
    },
    {
      id: 3,
      name: 'Laptop Stand Aluminum',
      sku: 'LS-ALU-001',
      image: 'https://images.unsplash.com/photo-1527864550417-7fd91fc51a46?auto=format&fit=crop&w=100',
      quantity: 1,
      price: 45.99
    }
  ],
  summary: {
    subtotal: 1144.97,
    shipping: 25.00,
    tax: 91.60,
    total: 1261.57
  }
})

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
.avatar {
  width: 3rem;
  height: 3rem;
  border-radius: 0.5rem;
}

.avatar-sm {
  width: 3rem;
  height: 3rem;
}

.horizontal.dark {
  background-color: rgba(0, 0, 0, 0.1);
}

.gap-2 {
  gap: 0.5rem;
}
</style>
