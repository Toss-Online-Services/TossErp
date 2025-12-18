<template>
  <div class="container-fluid py-4">
    <div class="row">
      <div class="col-lg-10 mx-auto">
        <div class="card">
          <div class="card-header pb-0">
            <div class="d-flex justify-content-between align-items-center mb-4">
              <div>
                <MDTypography variant="h4" font-weight="bold">
                  Invoice
                </MDTypography>
                <MDTypography variant="button" color="text">
                  Invoice #{{ invoice.number }}
                </MDTypography>
              </div>
              <div class="d-flex gap-2">
                <MDButton color="light">
                  <Icon name="mdi:printer" class="me-1" />
                  Print
                </MDButton>
                <MDButton color="primary">
                  <Icon name="mdi:download" class="me-1" />
                  Download PDF
                </MDButton>
              </div>
            </div>
          </div>
          <div class="card-body">
            <!-- Company & Client Info -->
            <div class="row mb-5">
              <div class="col-md-6">
                <MDTypography variant="h6" font-weight="bold" class="mb-2">
                  From:
                </MDTypography>
                <MDTypography variant="button" font-weight="bold" class="d-block">
                  {{ invoice.from.company }}
                </MDTypography>
                <MDTypography variant="button" color="text" class="d-block">
                  {{ invoice.from.address }}
                </MDTypography>
                <MDTypography variant="button" color="text" class="d-block">
                  {{ invoice.from.city }}, {{ invoice.from.state }} {{ invoice.from.zip }}
                </MDTypography>
                <MDTypography variant="button" color="text" class="d-block">
                  {{ invoice.from.country }}
                </MDTypography>
                <MDTypography variant="button" color="text" class="d-block mt-2">
                  Email: {{ invoice.from.email }}
                </MDTypography>
                <MDTypography variant="button" color="text" class="d-block">
                  Phone: {{ invoice.from.phone }}
                </MDTypography>
              </div>
              
              <div class="col-md-6 text-md-end">
                <MDTypography variant="h6" font-weight="bold" class="mb-2">
                  Bill To:
                </MDTypography>
                <MDTypography variant="button" font-weight="bold" class="d-block">
                  {{ invoice.to.name }}
                </MDTypography>
                <MDTypography variant="button" color="text" class="d-block">
                  {{ invoice.to.company }}
                </MDTypography>
                <MDTypography variant="button" color="text" class="d-block">
                  {{ invoice.to.address }}
                </MDTypography>
                <MDTypography variant="button" color="text" class="d-block">
                  {{ invoice.to.city }}, {{ invoice.to.state }} {{ invoice.to.zip }}
                </MDTypography>
                <MDTypography variant="button" color="text" class="d-block">
                  {{ invoice.to.country }}
                </MDTypography>
                <MDTypography variant="button" color="text" class="d-block mt-2">
                  Email: {{ invoice.to.email }}
                </MDTypography>
              </div>
            </div>

            <!-- Invoice Details -->
            <div class="row mb-4">
              <div class="col-md-4">
                <MDTypography variant="caption" color="text" class="d-block">
                  Invoice Number
                </MDTypography>
                <MDTypography variant="button" font-weight="bold">
                  {{ invoice.number }}
                </MDTypography>
              </div>
              <div class="col-md-4">
                <MDTypography variant="caption" color="text" class="d-block">
                  Invoice Date
                </MDTypography>
                <MDTypography variant="button" font-weight="bold">
                  {{ invoice.date }}
                </MDTypography>
              </div>
              <div class="col-md-4">
                <MDTypography variant="caption" color="text" class="d-block">
                  Due Date
                </MDTypography>
                <MDTypography variant="button" font-weight="bold">
                  {{ invoice.dueDate }}
                </MDTypography>
              </div>
            </div>

            <!-- Items Table -->
            <div class="table-responsive mb-4">
              <table class="table align-items-center mb-0">
                <thead>
                  <tr>
                    <th class="text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">Item Description</th>
                    <th class="text-center text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">Quantity</th>
                    <th class="text-center text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">Rate</th>
                    <th class="text-end text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">Amount</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="item in invoice.items" :key="item.id">
                    <td>
                      <MDTypography variant="button" font-weight="medium" class="d-block">
                        {{ item.description }}
                      </MDTypography>
                      <MDTypography variant="caption" color="text">
                        {{ item.details }}
                      </MDTypography>
                    </td>
                    <td class="align-middle text-center">
                      <MDTypography variant="button">
                        {{ item.quantity }}
                      </MDTypography>
                    </td>
                    <td class="align-middle text-center">
                      <MDTypography variant="button">
                        ${{ item.rate.toFixed(2) }}
                      </MDTypography>
                    </td>
                    <td class="align-middle text-end">
                      <MDTypography variant="button" font-weight="medium">
                        ${{ (item.quantity * item.rate).toFixed(2) }}
                      </MDTypography>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <!-- Totals -->
            <div class="row">
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
                            ${{ invoice.summary.subtotal.toFixed(2) }}
                          </MDTypography>
                        </td>
                      </tr>
                      <tr>
                        <td class="text-start">
                          <MDTypography variant="button" color="text">
                            Discount ({{ invoice.summary.discountPercent }}%):
                          </MDTypography>
                        </td>
                        <td class="text-end">
                          <MDTypography variant="button" font-weight="medium">
                            -${{ invoice.summary.discount.toFixed(2) }}
                          </MDTypography>
                        </td>
                      </tr>
                      <tr>
                        <td class="text-start">
                          <MDTypography variant="button" color="text">
                            Tax ({{ invoice.summary.taxPercent }}%):
                          </MDTypography>
                        </td>
                        <td class="text-end">
                          <MDTypography variant="button" font-weight="medium">
                            ${{ invoice.summary.tax.toFixed(2) }}
                          </MDTypography>
                        </td>
                      </tr>
                      <tr>
                        <td class="text-start border-top pt-3">
                          <MDTypography variant="h6" font-weight="bold">
                            Total:
                          </MDTypography>
                        </td>
                        <td class="text-end border-top pt-3">
                          <MDTypography variant="h6" font-weight="bold" color="success">
                            ${{ invoice.summary.total.toFixed(2) }}
                          </MDTypography>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
            </div>

            <!-- Notes & Terms -->
            <div class="row mt-5">
              <div class="col-12">
                <MDTypography variant="h6" font-weight="medium" class="mb-2">
                  Notes
                </MDTypography>
                <MDTypography variant="button" color="text">
                  {{ invoice.notes }}
                </MDTypography>
              </div>
            </div>

            <div class="row mt-4">
              <div class="col-12">
                <MDTypography variant="h6" font-weight="medium" class="mb-2">
                  Terms & Conditions
                </MDTypography>
                <MDTypography variant="caption" color="text">
                  {{ invoice.terms }}
                </MDTypography>
              </div>
            </div>

            <!-- Status Badge -->
            <div class="row mt-5">
              <div class="col-12 text-center">
                <MDBadge 
                  :color="getStatusColor(invoice.status)" 
                  variant="gradient"
                  size="lg"
                >
                  {{ invoice.status.toUpperCase() }}
                </MDBadge>
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

const invoice = ref({
  number: 'INV-2024-001',
  date: 'December 1, 2024',
  dueDate: 'December 31, 2024',
  status: 'paid',
  from: {
    company: 'Material Dashboard Inc.',
    address: '1234 Innovation Drive',
    city: 'San Francisco',
    state: 'CA',
    zip: '94102',
    country: 'United States',
    email: 'billing@materialdashboard.com',
    phone: '+1 (555) 123-4567'
  },
  to: {
    name: 'John Michael',
    company: 'Acme Corporation',
    address: '123 Main Street, Suite 100',
    city: 'New York',
    state: 'NY',
    zip: '10001',
    country: 'United States',
    email: 'john@example.com'
  },
  items: [
    {
      id: 1,
      description: 'Professional Plan Subscription',
      details: 'Monthly subscription - December 2024',
      quantity: 1,
      rate: 59.00
    },
    {
      id: 2,
      description: 'Additional User Licenses',
      details: '5 extra team members',
      quantity: 5,
      rate: 10.00
    },
    {
      id: 3,
      description: 'Premium Support Package',
      details: '24/7 priority support',
      quantity: 1,
      rate: 29.00
    }
  ],
  summary: {
    subtotal: 138.00,
    discountPercent: 10,
    discount: 13.80,
    taxPercent: 8,
    tax: 9.94,
    total: 134.14
  },
  notes: 'Thank you for your business! We appreciate your continued partnership with Material Dashboard. If you have any questions about this invoice, please contact our billing department.',
  terms: 'Payment is due within 30 days. Please make checks payable to Material Dashboard Inc. Late payments may be subject to a 1.5% monthly interest charge. All amounts are in USD.'
})

const getStatusColor = (status: string) => {
  const colors: Record<string, string> = {
    paid: 'success',
    pending: 'warning',
    overdue: 'error',
    draft: 'secondary'
  }
  return colors[status] || 'secondary'
}
</script>

<style scoped>
.gap-2 {
  gap: 0.5rem;
}

.border-top {
  border-top: 1px solid #dee2e6 !important;
}

@media print {
  .card-header {
    display: none;
  }
  
  .d-flex.gap-2 {
    display: none !important;
  }
}
</style>
