<template>
  <div class="container mx-auto px-4 py-8 max-w-5xl">
    <!-- Loading State -->
    <div v-if="loading" class="text-center py-12">
      <Icon name="mdi:loading" class="animate-spin text-4xl text-blue-600 mb-4" />
      <p class="text-gray-600">{{ t('common.loading') }}</p>
    </div>

    <!-- Error State -->
    <div v-else-if="error" class="text-center py-12">
      <Icon name="mdi:alert-circle" class="text-4xl text-red-600 mb-4" />
      <p class="text-red-600 mb-4">{{ error }}</p>
      <button @click="navigateTo('/sales/quotations')" class="btn btn-secondary">
        {{ t('common.back') }}
      </button>
    </div>

    <!-- Quotation Content -->
    <div v-else-if="quotation">
      <!-- Header with Actions -->
      <div class="flex items-start justify-between mb-8">
        <div>
          <div class="flex items-center gap-2 mb-2">
            <NuxtLink to="/sales/quotations" class="text-gray-600 hover:text-gray-900">
              <Icon name="mdi:arrow-left" size="24" />
            </NuxtLink>
            <h1 class="text-3xl font-bold">{{ quotation.quotationNo }}</h1>
            <span :class="getStatusClass(quotation.status)" class="px-3 py-1 text-sm font-semibold rounded-full">
              {{ t(`sales.quotations.${quotation.status}`) }}
            </span>
          </div>
          <p class="text-gray-600">{{ t('sales.quotations.createdOn') }} {{ formatDate(quotation.createdAt) }}</p>
        </div>

        <!-- Action Buttons -->
        <div class="flex gap-2">
          <button
            v-if="quotation.status === 'draft' || quotation.status === 'sent'"
            @click="editQuotation"
            class="btn btn-secondary flex items-center gap-2"
          >
            <Icon name="mdi:pencil" />
            {{ t('common.edit') }}
          </button>
          
          <button
            v-if="quotation.status !== 'accepted' && quotation.status !== 'rejected'"
            @click="downloadPDF"
            class="btn btn-secondary flex items-center gap-2"
          >
            <Icon name="mdi:file-pdf-box" />
            {{ t('sales.quotations.viewPdf') }}
          </button>
          
          <button
            v-if="quotation.status === 'draft'"
            @click="sendEmail"
            class="btn btn-primary flex items-center gap-2"
          >
            <Icon name="mdi:email-send" />
            {{ t('sales.quotations.sendEmail') }}
          </button>
          
          <button
            v-if="quotation.status === 'accepted'"
            @click="convertToOrder"
            class="btn btn-primary flex items-center gap-2"
          >
            <Icon name="mdi:swap-horizontal" />
            {{ t('sales.quotations.convertToOrder') }}
          </button>
        </div>
      </div>

      <!-- Quotation Details Card -->
      <div class="bg-white rounded-lg shadow-sm mb-6">
        <!-- Header Section -->
        <div class="p-8 border-b">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
            <!-- From (Company) -->
            <div>
              <h3 class="text-sm font-semibold text-gray-500 uppercase mb-2">{{ t('sales.quotations.from') }}</h3>
              <div class="text-lg font-bold mb-1">TOSS Online Services</div>
              <div class="text-gray-600 text-sm">
                <p>123 Township Street</p>
                <p>Johannesburg, Gauteng 2000</p>
                <p>South Africa</p>
                <p class="mt-2">
                  <Icon name="mdi:phone" class="inline mr-1" />
                  +27 11 123 4567
                </p>
                <p>
                  <Icon name="mdi:email" class="inline mr-1" />
                  sales@toss.co.za
                </p>
              </div>
            </div>

            <!-- To (Customer) -->
            <div>
              <h3 class="text-sm font-semibold text-gray-500 uppercase mb-2">{{ t('sales.quotations.to') }}</h3>
              <div class="text-lg font-bold mb-1">{{ quotation.customerName }}</div>
              <div class="text-gray-600 text-sm">
                <p v-if="quotation.customer.businessName">{{ quotation.customer.businessName }}</p>
                <p>{{ quotation.customer.address }}</p>
                <p class="mt-2">
                  <Icon name="mdi:phone" class="inline mr-1" />
                  {{ quotation.customer.phone }}
                </p>
                <p v-if="quotation.customer.email">
                  <Icon name="mdi:email" class="inline mr-1" />
                  {{ quotation.customer.email }}
                </p>
              </div>
            </div>
          </div>

          <!-- Quotation Meta -->
          <div class="grid grid-cols-2 md:grid-cols-4 gap-4 mt-8 pt-6 border-t">
            <div>
              <div class="text-xs text-gray-500 uppercase">{{ t('sales.quotations.quotationNo') }}</div>
              <div class="font-medium">{{ quotation.quotationNo }}</div>
            </div>
            <div>
              <div class="text-xs text-gray-500 uppercase">{{ t('sales.quotations.date') }}</div>
              <div class="font-medium">{{ formatDate(quotation.quotationDate) }}</div>
            </div>
            <div>
              <div class="text-xs text-gray-500 uppercase">{{ t('sales.quotations.validUntil') }}</div>
              <div class="font-medium">{{ formatDate(quotation.validUntil) }}</div>
            </div>
            <div>
              <div class="text-xs text-gray-500 uppercase">{{ t('sales.quotations.priceList') }}</div>
              <div class="font-medium capitalize">{{ quotation.priceList }}</div>
            </div>
          </div>
        </div>

        <!-- Line Items -->
        <div class="p-8">
          <h3 class="text-lg font-semibold mb-4">{{ t('sales.quotations.items') }}</h3>
          <div class="overflow-x-auto">
            <table class="min-w-full">
              <thead class="border-b-2 border-gray-200">
                <tr>
                  <th class="text-left py-3 px-2 text-sm font-semibold text-gray-600">#</th>
                  <th class="text-left py-3 px-4 text-sm font-semibold text-gray-600">{{ t('sales.quotations.product') }}</th>
                  <th class="text-right py-3 px-4 text-sm font-semibold text-gray-600">{{ t('sales.quotations.quantity') }}</th>
                  <th class="text-right py-3 px-4 text-sm font-semibold text-gray-600">{{ t('sales.quotations.rate') }}</th>
                  <th class="text-right py-3 px-4 text-sm font-semibold text-gray-600">{{ t('sales.quotations.discount') }}</th>
                  <th class="text-right py-3 px-4 text-sm font-semibold text-gray-600">{{ t('sales.quotations.amount') }}</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(item, index) in quotation.items" :key="item.id" class="border-b border-gray-100">
                  <td class="py-3 px-2 text-gray-600">{{ index + 1 }}</td>
                  <td class="py-3 px-4">
                    <div class="font-medium">{{ item.productName }}</div>
                    <div class="text-sm text-gray-500">{{ item.description }}</div>
                  </td>
                  <td class="py-3 px-4 text-right">{{ formatNumber(item.quantity) }}</td>
                  <td class="py-3 px-4 text-right">R{{ formatCurrency(item.rate) }}</td>
                  <td class="py-3 px-4 text-right">{{ item.discountPercent }}%</td>
                  <td class="py-3 px-4 text-right font-medium">R{{ formatCurrency(item.amount) }}</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <!-- Totals -->
        <div class="px-8 pb-8">
          <div class="flex justify-end">
            <div class="w-full md:w-1/2 lg:w-1/3 space-y-2">
              <div class="flex justify-between py-2 border-b">
                <span class="text-gray-600">{{ t('sales.quotations.subtotal') }}</span>
                <span class="font-medium">R{{ formatCurrency(quotation.subtotal) }}</span>
              </div>
              
              <div v-if="quotation.discountAmount > 0" class="flex justify-between py-2 border-b">
                <span class="text-gray-600">{{ t('sales.quotations.discount') }}</span>
                <span class="font-medium text-red-600">-R{{ formatCurrency(quotation.discountAmount) }}</span>
              </div>
              
              <div class="flex justify-between py-2 border-b">
                <span class="text-gray-600">{{ t('sales.quotations.taxableAmount') }}</span>
                <span class="font-medium">R{{ formatCurrency(quotation.taxableAmount) }}</span>
              </div>
              
              <div class="flex justify-between py-2 border-b">
                <span class="text-gray-600">{{ t('sales.quotations.vat') }} (15%)</span>
                <span class="font-medium">R{{ formatCurrency(quotation.taxAmount) }}</span>
              </div>
              
              <div class="flex justify-between py-3 border-t-2 border-gray-300">
                <span class="text-lg font-bold">{{ t('sales.quotations.grandTotal') }}</span>
                <span class="text-2xl font-bold text-blue-600">R{{ formatCurrency(quotation.grandTotal) }}</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Terms and Notes -->
        <div v-if="quotation.termsAndConditions || quotation.notes" class="px-8 pb-8 border-t">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-8 mt-8">
            <div v-if="quotation.termsAndConditions">
              <h3 class="text-sm font-semibold text-gray-500 uppercase mb-2">
                {{ t('sales.quotations.termsAndConditions') }}
              </h3>
              <p class="text-sm text-gray-600 whitespace-pre-line">{{ quotation.termsAndConditions }}</p>
            </div>
            
            <div v-if="quotation.notes">
              <h3 class="text-sm font-semibold text-gray-500 uppercase mb-2">
                {{ t('sales.quotations.notes') }}
              </h3>
              <p class="text-sm text-gray-600 whitespace-pre-line">{{ quotation.notes }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Activity Timeline -->
      <div class="bg-white rounded-lg shadow-sm p-6">
        <h3 class="text-lg font-semibold mb-4 flex items-center gap-2">
          <Icon name="mdi:history" />
          {{ t('sales.quotations.activityTimeline') }}
        </h3>
        
        <div class="space-y-4">
          <div v-for="activity in quotation.activities" :key="activity.id" class="flex gap-4">
            <div class="flex-shrink-0">
              <div class="w-10 h-10 rounded-full bg-blue-100 flex items-center justify-center">
                <Icon :name="getActivityIcon(activity.type)" class="text-blue-600" />
              </div>
            </div>
            <div class="flex-1">
              <div class="flex items-center gap-2 mb-1">
                <span class="font-medium">{{ activity.title }}</span>
                <span class="text-sm text-gray-500">{{ formatDateTime(activity.timestamp) }}</span>
              </div>
              <p class="text-sm text-gray-600">{{ activity.description }}</p>
              <p v-if="activity.user" class="text-xs text-gray-500 mt-1">{{ t('sales.quotations.by') }} {{ activity.user }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
const { t } = useI18n()
const route = useRoute()
const router = useRouter()

// State
const loading = ref(true)
const error = ref('')
const quotation = ref(null)

// Fetch quotation on mount
onMounted(async () => {
  await fetchQuotation()
})

const fetchQuotation = async () => {
  try {
    loading.value = true
    const id = route.params.id
    
    // Mock API call - Replace with actual API
    await new Promise(resolve => setTimeout(resolve, 500))
    
    // Mock data
    quotation.value = {
      id: parseInt(id),
      quotationNo: 'QTN-2025-001',
      quotationDate: '2025-01-10',
      validUntil: '2025-01-31',
      status: 'sent',
      customerName: 'Mama Dlamini Spaza',
      customer: {
        businessName: 'Mama Dlamini Spaza Shop',
        address: '45 Main Road, Soweto, Johannesburg 1800',
        phone: '082 123 4567',
        email: 'mama@example.com'
      },
      priceList: 'standard',
      items: [
        {
          id: 1,
          productName: 'White Bread',
          description: 'Albany White Bread 700g',
          quantity: 50,
          rate: 15.99,
          discountPercent: 5,
          amount: 759.53
        },
        {
          id: 2,
          productName: 'Milk 2L',
          description: 'Full Cream Milk 2L',
          quantity: 30,
          rate: 32.99,
          discountPercent: 0,
          amount: 989.70
        },
        {
          id: 3,
          productName: 'Sugar 2.5kg',
          description: 'White Sugar 2.5kg',
          quantity: 20,
          rate: 42.99,
          discountPercent: 10,
          amount: 773.82
        }
      ],
      subtotal: 2562.95,
      discountAmount: 39.90,
      taxableAmount: 2523.05,
      taxAmount: 378.46,
      grandTotal: 2901.51,
      termsAndConditions: 'Payment due within 30 days\nDelivery within 5 business days\nPrices valid for 21 days',
      notes: 'First order - special bulk discount applied',
      createdAt: '2025-01-10T09:30:00',
      activities: [
        {
          id: 1,
          type: 'created',
          title: 'Quotation Created',
          description: 'Quotation was created as draft',
          user: 'John Doe',
          timestamp: '2025-01-10T09:30:00'
        },
        {
          id: 2,
          type: 'sent',
          title: 'Quotation Sent',
          description: 'Quotation was sent to customer via email',
          user: 'John Doe',
          timestamp: '2025-01-10T10:15:00'
        }
      ]
    }
    
    loading.value = false
  } catch (err) {
    error.value = t('sales.quotations.errorLoading')
    loading.value = false
  }
}

// Methods
const getStatusClass = (status: string) => {
  const classes = {
    'draft': 'bg-gray-100 text-gray-800',
    'sent': 'bg-blue-100 text-blue-800',
    'accepted': 'bg-green-100 text-green-800',
    'rejected': 'bg-red-100 text-red-800',
    'expired': 'bg-orange-100 text-orange-800'
  }
  return classes[status] || 'bg-gray-100 text-gray-800'
}

const getActivityIcon = (type: string) => {
  const icons = {
    'created': 'mdi:file-document-plus',
    'sent': 'mdi:email-send',
    'accepted': 'mdi:check-circle',
    'rejected': 'mdi:close-circle',
    'converted': 'mdi:swap-horizontal'
  }
  return icons[type] || 'mdi:circle'
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('en-ZA', { 
    day: 'numeric',
    month: 'long',
    year: 'numeric'
  })
}

const formatDateTime = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleString('en-ZA', {
    day: 'numeric',
    month: 'short',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const formatCurrency = (amount: number): string => {
  return amount.toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',')
}

const formatNumber = (num: number): string => {
  return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',')
}

const editQuotation = () => {
  router.push(`/sales/quotations/${quotation.value.id}/edit`)
}

const downloadPDF = () => {
  // Generate and download PDF
  console.log('Downloading PDF for quotation:', quotation.value.id)
  alert(t('sales.quotations.pdfGenerating'))
  // Implement PDF generation using jsPDF
}

const sendEmail = async () => {
  try {
    // Send email via API
    console.log('Sending email for quotation:', quotation.value.id)
    
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    quotation.value.status = 'sent'
    quotation.value.activities.push({
      id: quotation.value.activities.length + 1,
      type: 'sent',
      title: 'Quotation Sent',
      description: 'Quotation was sent to customer via email',
      user: 'Current User',
      timestamp: new Date().toISOString()
    })
    
    alert(t('sales.quotations.emailSent'))
  } catch (err) {
    alert(t('sales.quotations.errorSending'))
  }
}

const convertToOrder = async () => {
  if (!confirm(t('sales.quotations.confirmConvertToOrder'))) {
    return
  }
  
  try {
    // Convert to order via API
    console.log('Converting quotation to order:', quotation.value.id)
    
    await new Promise(resolve => setTimeout(resolve, 1000))
    
    // Navigate to new order
    router.push('/sales/orders/123')
  } catch (err) {
    alert(t('sales.quotations.errorConverting'))
  }
}

// Page meta
definePageMeta({
  layout: 'default',
  middleware: 'auth'
})

// SEO
useHead({
  title: computed(() => quotation.value ? quotation.value.quotationNo : t('sales.quotations.title'))
})
</script>

<style scoped>
.btn {
  @apply px-4 py-2 rounded-lg font-medium transition-colors;
}

.btn-primary {
  @apply bg-blue-600 text-white hover:bg-blue-700;
}

.btn-secondary {
  @apply bg-gray-200 text-gray-800 hover:bg-gray-300;
}
</style>
