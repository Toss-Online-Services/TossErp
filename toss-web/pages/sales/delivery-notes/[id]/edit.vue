<template>
  <div class="p-4 sm:p-6">
    <div class="flex items-center justify-between mb-6">
      <div>
        <h1 class="text-2xl font-bold text-gray-900 dark:text-white">
          {{ $t('sales.deliveryNotes.edit.title') }} #{{ deliveryNoteId }}
        </h1>
        <p class="text-sm text-gray-500 dark:text-gray-400">
          {{ $t('sales.deliveryNotes.edit.description') }}
        </p>
      </div>
    </div>

    <FormKit type="form" :value="initialValues" @submit="updateDeliveryNote" #default="{ value }">
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Main Form -->
        <div class="lg:col-span-2 bg-white dark:bg-gray-800 shadow-sm rounded-lg p-6">
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-6">
            <FormKit
              type="text"
              name="orderId"
              :label="$t('sales.deliveryNotes.create.salesOrder')"
              disabled
            />
            <FormKit
              type="text"
              name="customerName"
              :label="$t('sales.deliveryNotes.create.customer')"
              disabled
            />
            <FormKit
              type="date"
              name="deliveryDate"
              :label="$t('sales.deliveryNotes.create.deliveryDate')"
              validation="required"
            />
            <FormKit
              type="select"
              name="driverId"
              :label="$t('sales.deliveryNotes.create.driver')"
              :placeholder="$t('sales.deliveryNotes.create.assignDriver')"
              :options="['Themba', 'Lefa']"
              validation="required"
            />
          </div>
          <div class="mt-6">
            <FormKit
              type="textarea"
              name="deliveryAddress"
              :label="$t('sales.deliveryNotes.create.deliveryAddress')"
              rows="3"
              disabled
            />
          </div>
          <div class="mt-6">
            <FormKit
              type="textarea"
              name="notes"
              :label="$t('sales.deliveryNotes.create.notes')"
              :placeholder="$t('sales.deliveryNotes.create.notesPlaceholder')"
              rows="3"
            />
          </div>
        </div>

        <!-- Summary -->
        <div class="lg:col-span-1">
          <div class="bg-white dark:bg-gray-800 shadow-sm rounded-lg p-6 sticky top-6">
            <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-4">{{ $t('common.summary') }}</h3>
            <div class="space-y-3 text-sm">
              <div class="flex justify-between">
                <span class="text-gray-500 dark:text-gray-400">{{ $t('sales.deliveryNotes.create.order') }}:</span>
                <span class="font-medium text-gray-800 dark:text-gray-200">{{ value.orderId || 'N/A' }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-gray-500 dark:text-gray-400">{{ $t('sales.deliveryNotes.create.customer') }}:</span>
                <span class="font-medium text-gray-800 dark:text-gray-200">{{ value.customerName || 'N/A' }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-gray-500 dark:text-gray-400">{{ $t('sales.deliveryNotes.create.deliveryDate') }}:</span>
                <span class="font-medium text-gray-800 dark:text-gray-200">{{ value.deliveryDate ? formatDate(value.deliveryDate) : 'N/A' }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-gray-500 dark:text-gray-400">{{ $t('sales.deliveryNotes.create.driver') }}:</span>
                <span class="font-medium text-gray-800 dark:text-gray-200">{{ value.driverId || 'N/A' }}</span>
              </div>
            </div>
            <div class="mt-6">
              <FormKit type="submit" :label="$t('sales.deliveryNotes.edit.updateButton')" outer-class="!mb-0" />
            </div>
          </div>
        </div>
      </div>
    </FormKit>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { FormKit } from '@formkit/vue'

const { t } = useI18n()
const route = useRoute()

const deliveryNoteId = route.params.id

// Mock data for the delivery note being edited
const initialValues = ref({
  orderId: 'SO-123',
  customerName: 'Jabu\'s Spaza',
  deliveryDate: '2025-11-12',
  deliveryAddress: '123 Main Road, Soweto, Johannesburg, 1818',
  driverId: 'Themba',
  notes: 'Customer requested delivery after 2 PM. Confirmed receipt with signature.',
})

useHead({
  title: t('sales.deliveryNotes.edit.pageTitle', { id: deliveryNoteId }),
})

const updateDeliveryNote = async (formData: any) => {
  console.log(`Updating delivery note ${deliveryNoteId} with:`, formData)
  // Here you would typically make an API call
  alert(t('sales.deliveryNotes.edit.successMessage') + `\n\n${JSON.stringify(formData, null, 2)}`)
  // Redirect to the delivery note's page
  // For example: await navigateTo(`/sales/delivery-notes/${deliveryNoteId}`)
}

const formatDate = (dateString: string) => new Date(dateString).toLocaleDateString('en-ZA')
</script>
