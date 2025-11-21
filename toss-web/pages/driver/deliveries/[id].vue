<template>
  <div class="p-6">
    <div class="mb-6">
      <MaterialButton to="/driver/deliveries" color="primary" variant="text" class="mb-6" as="NuxtLink">
        &#8592; Back to Deliveries
      </MaterialButton>
    </div>

    <div v-if="isLoading" class="text-center py-12">
      <div class="inline-block w-8 h-8 border-4 border-purple-600 border-t-transparent rounded-full animate-spin"></div>
      <p class="mt-2 text-gray-600">Loading delivery...</p>
    </div>

    <div v-else-if="delivery" class="space-y-6">
      <!-- Delivery Header -->
      <MaterialCard variant="elevated" class="mb-6">
        <div class="flex justify-between items-start mb-4">
          <div>
            <h1 class="text-2xl font-bold text-gray-900 dark:text-gray-100">{{ delivery.poNumber }}</h1>
            <p class="text-gray-600 dark:text-gray-400 mt-2">
              <span class="font-semibold">From:</span> {{ delivery.supplierName || 'Supplier' }}<br>
              <span class="font-semibold">To:</span> {{ delivery.shopName || 'Retailer' }}
            </p>
            <p class="text-gray-600 dark:text-gray-400 mt-2">
              Date: {{ formatDate(delivery.orderDate) }}
            </p>
          </div>
          <UiBadge :color="getStatusColor(delivery.status)" class="px-4 py-2 text-sm font-semibold">{{ delivery.status }}</UiBadge>
        </div>

        <!-- Contact Information -->
        <MaterialCard variant="outlined" class="mt-6">
          <h3 class="font-semibold mb-2">Contact Information</h3>
          <p class="text-sm text-gray-600 dark:text-gray-400">
            Supplier: {{ delivery.supplierPhone || 'N/A' }}<br>
            Retailer: {{ delivery.shopPhone || 'N/A' }}
          </p>
        </div>

        <!-- Delivery Items -->
        <MaterialCard variant="outlined" class="mt-6">
          <h2 class="text-lg font-semibold mb-4">Items to Deliver</h2>
          <div class="space-y-2">
            <div
              v-for="item in delivery.items"
              :key="item.id"
              class="flex justify-between items-center p-3 bg-gray-50 dark:bg-gray-700 rounded"
            >
              <div>
                <p class="font-medium text-gray-900 dark:text-gray-100">{{ item.productName }}</p>
                <p class="text-sm text-gray-600 dark:text-gray-400">SKU: {{ item.productSKU }}</p>
              </div>
              <div class="text-right">
                <p class="font-medium text-gray-900 dark:text-gray-100">Qty: {{ item.quantity }}</p>
                <p class="text-sm text-gray-600 dark:text-gray-400">R{{ formatCurrency(item.lineTotal) }}</p>
              </div>
            </div>
          </div>
        </div>

        <div class="mt-6 pt-6 border-t flex justify-between items-center">
          <div>
            <p class="text-sm text-gray-600 dark:text-gray-400">Total Amount</p>
            <p class="text-lg font-semibold text-gray-900 dark:text-gray-100">R{{ formatCurrency(delivery.totalAmount) }}</p>
          </div>
        </div>
      </div>

      <!-- Status Actions -->
      <MaterialCard variant="elevated" class="mt-6">
        <h2 class="text-lg font-semibold mb-4">Update Status</h2>
        
        <div v-if="delivery.status === 'Accepted'" class="space-y-4">
          <MaterialButton @click="markPickedUp" :disabled="isProcessing" color="primary" class="w-full">
            Mark as Picked Up
          </MaterialButton>
        </div>

        <div v-else-if="delivery.status === 'PickedUp'" class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Delivery Notes (optional)
            </label>
            <MaterialTextarea
              v-model="deliveryNotes"
              rows="3"
              placeholder="Any notes about the delivery..."
              class="mb-4"
            />
          </div>
          <template>
            <div class="min-h-screen bg-gray-100 dark:bg-gray-900 py-8 px-4">
              <MaterialButton to="/driver/deliveries" color="primary" variant="text" class="mb-6" as="NuxtLink">
                &#8592; Back to Deliveries
              </MaterialButton>

              <div>
                <MaterialCard variant="elevated" class="mb-6">
                  <h1 class="text-2xl font-bold mb-2">Delivery #{{ deliveryId }}</h1>
                  <div class="flex items-center mb-2">
                    <span class="font-medium mr-2">Status:</span>
                    <UiBadge :color="getStatusColor(delivery.status)" class="px-4 py-2 text-sm font-semibold">{{ delivery.status }}</UiBadge>
                  </div>
                  <div class="mb-2">
                    <span class="font-medium mr-2">Assigned Driver:</span>
                    {{ delivery.driverName || 'N/A' }}
                  </div>
                  <div class="mb-2">
                    <span class="font-medium mr-2">Pickup Location:</span>
                    {{ delivery.pickupLocation }}
                  </div>
                  <div class="mb-2">
                    <span class="font-medium mr-2">Dropoff Location:</span>
                    {{ delivery.dropoffLocation }}
                  </div>
                  <div class="mb-2">
                    <span class="font-medium mr-2">Scheduled Time:</span>
                    {{ delivery.scheduledTime }}
                  </div>
                </MaterialCard>

                <MaterialCard variant="outlined" class="mt-6">
                  <h2 class="text-lg font-semibold mb-4">Delivery Details</h2>
                  <div class="mb-2">
                    <span class="font-medium mr-2">Product:</span>
                    {{ delivery.productName }}
                  </div>
                  <div class="mb-2">
                    <span class="font-medium mr-2">Quantity:</span>
                    {{ delivery.quantity }}
                  </div>
                  <div class="mb-2">
                    <span class="font-medium mr-2">Notes:</span>
                    {{ delivery.notes || 'None' }}
                  </div>
                </MaterialCard>

                <MaterialCard variant="elevated" class="mt-6">
                  <h2 class="text-lg font-semibold mb-4">Update Status</h2>
                  <div v-if="delivery.status === 'Accepted'" class="space-y-4">
                    <MaterialButton @click="markPickedUp" :disabled="isProcessing" color="primary" class="w-full">
                      Mark as Picked Up
                    </MaterialButton>
                  </div>
                  <div v-else-if="delivery.status === 'PickedUp'" class="space-y-4">
                    <div>
                      <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                        Delivery Notes (optional)
                      </label>
                      <MaterialTextarea
                        v-model="deliveryNotes"
                        rows="3"
                        placeholder="Any notes about the delivery..."
                        class="mb-4"
                      />
                    </div>
                    <div class="flex items-center mb-4">
                      <input
                        v-model="deliveryConfirmed"
                        type="checkbox"
                        class="mr-2"
                      />
                      <label class="text-sm font-medium text-gray-700 dark:text-gray-300">
                        Delivery confirmed by recipient
                      </label>
                    </div>
                    <MaterialButton @click="markDelivered" :disabled="isProcessing" color="success" class="w-full">
                      Mark as Delivered
                    </MaterialButton>
                  </div>
                  <div v-else-if="delivery.status === 'Delivered'" class="p-4 bg-green-50 dark:bg-green-900/20 rounded-lg">
                    <p class="text-green-800 dark:text-green-200 font-semibold">
                      &#10003; Delivery completed
                    </p>
                  </div>
                </MaterialCard>
              </div>
            </div>
          </template>
    isProcessing.value = false
  }
}

const formatCurrency = (amount: number) => {
  return new Intl.NumberFormat('en-ZA', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

const formatDate = (date: string) => {
  return new Date(date).toLocaleDateString('en-ZA')
}

const getStatusClass = (status: string) => {
  const classes: Record<string, string> = {
    'Accepted': 'bg-blue-100 text-blue-800 dark:bg-blue-900 dark:text-blue-200',
    'PickedUp': 'bg-purple-100 text-purple-800 dark:bg-purple-900 dark:text-purple-200',
    'Delivered': 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
  }
  return classes[status] || 'bg-gray-100 text-gray-800'
}

onMounted(() => {
  loadDelivery()
})
</script>

