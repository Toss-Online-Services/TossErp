<template>
  <div class="container-fluid py-4">
    <div class="row">
      <div class="col-lg-8 mb-4">
        <!-- Payment Method -->
        <div class="card mb-4">
          <div class="card-header pb-0">
            <div class="d-flex justify-content-between align-items-center">
              <MDTypography variant="h6" font-weight="medium">
                Payment Method
              </MDTypography>
              <MDButton color="primary" size="sm">
                <Icon name="mdi:plus" class="me-1" />
                Add New Card
              </MDButton>
            </div>
          </div>
          <div class="card-body">
            <div class="row">
              <div v-for="card in paymentMethods" :key="card.id" class="col-md-6 mb-3">
                <div class="card border" :class="card.default ? 'border-primary' : ''">
                  <div class="card-body">
                    <div class="d-flex justify-content-between align-items-start mb-3">
                      <div class="d-flex align-items-center">
                        <Icon 
                          :name="getCardIcon(card.type)" 
                          size="40" 
                          class="me-2"
                        />
                        <div>
                          <MDTypography variant="button" font-weight="bold" class="d-block">
                            {{ card.type }}
                          </MDTypography>
                          <MDTypography variant="caption" color="text">
                            •••• •••• •••• {{ card.last4 }}
                          </MDTypography>
                        </div>
                      </div>
                      <MDBadge v-if="card.default" color="success" variant="gradient" size="sm">
                        Default
                      </MDBadge>
                    </div>
                    
                    <div class="d-flex justify-content-between mb-2">
                      <MDTypography variant="caption" color="text">
                        Expires {{ card.expiry }}
                      </MDTypography>
                    </div>
                    
                    <div class="d-flex gap-2">
                      <MDButton v-if="!card.default" color="light" size="sm" class="flex-grow-1">
                        Set as Default
                      </MDButton>
                      <MDButton color="light" size="sm">
                        <Icon name="mdi:pencil" />
                      </MDButton>
                      <MDButton color="light" size="sm">
                        <Icon name="mdi:delete" />
                      </MDButton>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Invoices -->
        <div class="card">
          <div class="card-header pb-0">
            <MDTypography variant="h6" font-weight="medium">
              Invoices
            </MDTypography>
          </div>
          <div class="card-body">
            <div class="table-responsive">
              <table class="table align-items-center mb-0">
                <thead>
                  <tr>
                    <th class="text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">Invoice</th>
                    <th class="text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">Date</th>
                    <th class="text-center text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">Amount</th>
                    <th class="text-center text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">Status</th>
                    <th class="text-center text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">Action</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="invoice in invoices" :key="invoice.id">
                    <td>
                      <MDTypography variant="button" font-weight="medium">
                        {{ invoice.number }}
                      </MDTypography>
                    </td>
                    <td>
                      <MDTypography variant="caption" color="text">
                        {{ invoice.date }}
                      </MDTypography>
                    </td>
                    <td class="align-middle text-center">
                      <MDTypography variant="button" font-weight="bold">
                        ${{ invoice.amount.toFixed(2) }}
                      </MDTypography>
                    </td>
                    <td class="align-middle text-center">
                      <MDBadge 
                        :color="getStatusColor(invoice.status)" 
                        variant="gradient"
                      >
                        {{ invoice.status }}
                      </MDBadge>
                    </td>
                    <td class="align-middle text-center">
                      <NuxtLink :to="`/pages/account/invoice?id=${invoice.id}`">
                        <MDButton color="light" size="sm">
                          <Icon name="mdi:download" class="me-1" />
                          PDF
                        </MDButton>
                      </NuxtLink>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-4">
        <!-- Current Plan -->
        <div class="card mb-4">
          <div class="card-header pb-0">
            <MDTypography variant="h6" font-weight="medium">
              Current Plan
            </MDTypography>
          </div>
          <div class="card-body">
            <div class="text-center mb-3">
              <MDTypography variant="h3" font-weight="bold" color="primary">
                Professional
              </MDTypography>
              <MDTypography variant="h2" font-weight="bold" class="my-3">
                $59
                <MDTypography variant="button" color="text">
                  /month
                </MDTypography>
              </MDTypography>
            </div>
            
            <ul class="list-unstyled mb-4">
              <li class="mb-2">
                <Icon name="mdi:check-circle" class="text-success me-2" />
                <MDTypography variant="button" color="text">
                  Unlimited projects
                </MDTypography>
              </li>
              <li class="mb-2">
                <Icon name="mdi:check-circle" class="text-success me-2" />
                <MDTypography variant="button" color="text">
                  Advanced analytics
                </MDTypography>
              </li>
              <li class="mb-2">
                <Icon name="mdi:check-circle" class="text-success me-2" />
                <MDTypography variant="button" color="text">
                  Priority support
                </MDTypography>
              </li>
              <li class="mb-2">
                <Icon name="mdi:check-circle" class="text-success me-2" />
                <MDTypography variant="button" color="text">
                  Custom integrations
                </MDTypography>
              </li>
            </ul>
            
            <MDButton color="primary" class="w-100 mb-2">
              Upgrade Plan
            </MDButton>
            <MDButton color="light" class="w-100">
              Cancel Subscription
            </MDButton>
            
            <MDTypography variant="caption" color="text" class="d-block text-center mt-3">
              Next billing date: January 1, 2025
            </MDTypography>
          </div>
        </div>

        <!-- Billing Information -->
        <div class="card">
          <div class="card-header pb-0">
            <div class="d-flex justify-content-between align-items-center">
              <MDTypography variant="h6" font-weight="medium">
                Billing Information
              </MDTypography>
              <MDButton color="light" size="sm">
                <Icon name="mdi:pencil" />
              </MDButton>
            </div>
          </div>
          <div class="card-body">
            <MDTypography variant="button" font-weight="bold" class="d-block mb-1">
              John Michael
            </MDTypography>
            <MDTypography variant="caption" color="text" class="d-block mb-1">
              Company: Creative Tim
            </MDTypography>
            <MDTypography variant="caption" color="text" class="d-block mb-1">
              Email: john@example.com
            </MDTypography>
            <MDTypography variant="caption" color="text" class="d-block mb-1">
              VAT Number: FRB1235476
            </MDTypography>
            
            <hr class="horizontal dark my-3">
            
            <MDTypography variant="button" font-weight="bold" class="d-block mb-2">
              Address
            </MDTypography>
            <MDTypography variant="caption" color="text" class="d-block">
              123 Main Street, Apt 4B<br>
              New York, NY 10001<br>
              United States
            </MDTypography>
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

const paymentMethods = ref([
  {
    id: 1,
    type: 'Visa',
    last4: '4242',
    expiry: '12/24',
    default: true
  },
  {
    id: 2,
    type: 'Mastercard',
    last4: '5555',
    expiry: '06/25',
    default: false
  }
])

const invoices = ref([
  {
    id: 1,
    number: 'INV-2024-001',
    date: 'December 1, 2024',
    amount: 59.00,
    status: 'paid'
  },
  {
    id: 2,
    number: 'INV-2024-002',
    date: 'November 1, 2024',
    amount: 59.00,
    status: 'paid'
  },
  {
    id: 3,
    number: 'INV-2024-003',
    date: 'October 1, 2024',
    amount: 59.00,
    status: 'paid'
  },
  {
    id: 4,
    number: 'INV-2024-004',
    date: 'September 1, 2024',
    amount: 59.00,
    status: 'paid'
  }
])

const getCardIcon = (type: string) => {
  const icons: Record<string, string> = {
    'Visa': 'mdi:credit-card',
    'Mastercard': 'mdi:credit-card',
    'American Express': 'mdi:credit-card',
    'Discover': 'mdi:credit-card'
  }
  return icons[type] || 'mdi:credit-card'
}

const getStatusColor = (status: string) => {
  const colors: Record<string, string> = {
    paid: 'success',
    pending: 'warning',
    overdue: 'error'
  }
  return colors[status] || 'secondary'
}
</script>

<style scoped>
.gap-2 {
  gap: 0.5rem;
}

.list-unstyled {
  padding-left: 0;
  list-style: none;
}

.border {
  border: 1px solid #dee2e6 !important;
}

.border-primary {
  border-color: #5e72e4 !important;
  border-width: 2px !important;
}

.horizontal.dark {
  background-color: rgba(0, 0, 0, 0.1);
}
</style>
