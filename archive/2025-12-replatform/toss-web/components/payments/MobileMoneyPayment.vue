<template>
  <div class="mobile-money-payment">
    <!-- Provider Selection -->
    <div v-if="!selectedProvider" class="provider-selection">
      <h3 class="text-lg font-semibold mb-4 text-gray-900 dark:text-white">
        {{ $t('payments.selectProvider', 'Select Payment Method') }}
      </h3>
      
      <div class="grid grid-cols-1 gap-3">
        <button
          v-for="provider in availableProviders"
          :key="provider"
          @click="selectProvider(provider)"
          class="provider-button"
        >
          <div class="flex items-center space-x-3">
            <span class="text-3xl">{{ getProviderIcon(provider) }}</span>
            <div class="text-left">
              <div class="font-semibold text-gray-900 dark:text-white">
                {{ getProviderName(provider) }}
              </div>
              <div class="text-sm text-gray-600 dark:text-gray-400">
                {{ getProviderDescription(provider) }}
              </div>
            </div>
          </div>
          <svg class="w-5 h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
          </svg>
        </button>
      </div>
    </div>

    <!-- Payment Form -->
    <div v-else class="payment-form">
      <!-- Header -->
      <div class="flex items-center justify-between mb-6">
        <button @click="selectedProvider = null" class="text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7" />
          </svg>
        </button>
        <div class="flex items-center space-x-2">
          <span class="text-2xl">{{ getProviderIcon(selectedProvider) }}</span>
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">
            {{ getProviderName(selectedProvider) }}
          </h3>
        </div>
        <div class="w-6"></div>
      </div>

      <!-- Amount Display -->
      <div class="amount-display">
        <div class="text-sm text-gray-600 dark:text-gray-400 mb-1">
          {{ $t('payments.amountToPay', 'Amount to Pay') }}
        </div>
        <div class="text-3xl font-bold text-gray-900 dark:text-white">
          R{{ amount.toFixed(2) }}
        </div>
      </div>

      <!-- Phone Number Input -->
      <div class="mb-6">
        <label class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
          {{ $t('payments.phoneNumber', 'Phone Number') }}
        </label>
        <input
          v-model="phoneNumber"
          type="tel"
          :placeholder="getPhonePlaceholder(selectedProvider)"
          class="w-full px-4 py-3 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-blue-500"
          :class="{ 'border-red-500': phoneError }"
        />
        <p v-if="phoneError" class="mt-1 text-sm text-red-600 dark:text-red-400">
          {{ phoneError }}
        </p>
        <p v-else class="mt-1 text-xs text-gray-500 dark:text-gray-400">
          {{ $t('payments.phoneHint', 'Enter the number registered with') }} {{ getProviderName(selectedProvider) }}
        </p>
      </div>

      <!-- Pay Button -->
      <button
        @click="initiatePayment"
        :disabled="isProcessing || !isPhoneValid"
        class="w-full py-4 bg-blue-600 hover:bg-blue-700 disabled:bg-gray-400 text-white font-semibold rounded-lg transition-colors disabled:cursor-not-allowed"
      >
        <span v-if="!isProcessing">
          {{ $t('payments.payNow', 'Pay Now') }} - R{{ amount.toFixed(2) }}
        </span>
        <span v-else class="flex items-center justify-center">
          <svg class="animate-spin h-5 w-5 mr-2" fill="none" viewBox="0 0 24 24">
            <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
            <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
          </svg>
          {{ $t('payments.processing', 'Processing...') }}
        </span>
      </button>

      <!-- Instructions -->
      <div class="mt-6 p-4 bg-blue-50 dark:bg-blue-900/20 rounded-lg">
        <h4 class="text-sm font-semibold text-blue-900 dark:text-blue-200 mb-2">
          {{ $t('payments.instructions', 'Payment Instructions') }}
        </h4>
        <ol class="text-sm text-blue-800 dark:text-blue-300 space-y-1 list-decimal list-inside">
          <li>{{ $t('payments.step1', 'You will receive a prompt on your phone') }}</li>
          <li>{{ $t('payments.step2', 'Enter your PIN to authorize') }}</li>
          <li>{{ $t('payments.step3', 'Wait for confirmation') }}</li>
        </ol>
      </div>
    </div>

    <!-- Success Modal -->
    <div v-if="showSuccessModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50" @click="closeSuccessModal">
      <div class="bg-white dark:bg-gray-800 rounded-lg p-8 max-w-md mx-4" @click.stop>
        <div class="text-center">
          <div class="w-16 h-16 bg-green-100 dark:bg-green-900 rounded-full flex items-center justify-center mx-auto mb-4">
            <svg class="w-8 h-8 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path>
            </svg>
          </div>
          <h3 class="text-xl font-bold text-gray-900 dark:text-white mb-2">
            {{ $t('payments.requestSent', 'Payment Request Sent!') }}
          </h3>
          <p class="text-gray-600 dark:text-gray-400 mb-4">
            {{ successMessage }}
          </p>
          <div class="text-sm text-gray-500 dark:text-gray-400 mb-6">
            {{ $t('payments.transactionId', 'Transaction ID') }}: {{ transactionId }}
          </div>
          <button
            @click="closeSuccessModal"
            class="px-6 py-2 bg-blue-600 hover:bg-blue-700 text-white font-medium rounded-lg transition-colors"
          >
            {{ $t('common.ok', 'OK') }}
          </button>
        </div>
      </div>
    </div>

    <!-- Error Modal -->
    <div v-if="showErrorModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50" @click="closeErrorModal">
      <div class="bg-white dark:bg-gray-800 rounded-lg p-8 max-w-md mx-4" @click.stop>
        <div class="text-center">
          <div class="w-16 h-16 bg-red-100 dark:bg-red-900 rounded-full flex items-center justify-center mx-auto mb-4">
            <svg class="w-8 h-8 text-red-600 dark:text-red-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </div>
          <h3 class="text-xl font-bold text-gray-900 dark:text-white mb-2">
            {{ $t('payments.failed', 'Payment Failed') }}
          </h3>
          <p class="text-gray-600 dark:text-gray-400 mb-6">
            {{ errorMessage }}
          </p>
          <button
            @click="closeErrorModal"
            class="px-6 py-2 bg-gray-600 hover:bg-gray-700 text-white font-medium rounded-lg transition-colors"
          >
            {{ $t('common.tryAgain', 'Try Again') }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useMobileMoney, type MobileMoneyProvider } from '~/composables/useMobileMoney'

const props = defineProps<{
  amount: number
  reference: string
  description?: string
  phoneNumber?: string
}>()

const emit = defineEmits<{
  success: [transactionId: string]
  cancel: []
}>()

const {
  pay,
  isProcessing,
  error,
  validatePhoneNumber,
  formatPhoneNumber,
  getProviderName,
  getProviderIcon,
  getSupportedProviders
} = useMobileMoney()

// State
const selectedProvider = ref<MobileMoneyProvider | null>(null)
const phoneNumber = ref(props.phoneNumber || '')
const phoneError = ref('')
const showSuccessModal = ref(false)
const showErrorModal = ref(false)
const successMessage = ref('')
const errorMessage = ref('')
const transactionId = ref('')

// Computed
const availableProviders = computed(() => {
  // Detect country from user's phone or default to South Africa
  const countryCode = phoneNumber.value ? phoneNumber.value.replace(/\D/g, '').slice(0, 2) : '27'
  return getSupportedProviders(countryCode).length > 0 
    ? getSupportedProviders(countryCode) 
    : ['mpesa', 'airtel', 'mtn'] as MobileMoneyProvider[]
})

const isPhoneValid = computed(() => {
  if (!selectedProvider.value || !phoneNumber.value) return false
  return validatePhoneNumber(phoneNumber.value, selectedProvider.value)
})

// Methods
const selectProvider = (provider: MobileMoneyProvider) => {
  selectedProvider.value = provider
  phoneError.value = ''
}

const getProviderDescription = (provider: MobileMoneyProvider): string => {
  switch (provider) {
    case 'mpesa':
      return 'Pay with M-Pesa'
    case 'airtel':
      return 'Pay with Airtel Money'
    case 'mtn':
      return 'Pay with MTN Mobile Money'
    default:
      return ''
  }
}

const getPhonePlaceholder = (provider: MobileMoneyProvider): string => {
  switch (provider) {
    case 'mpesa':
      return '+27 XX XXX XXXX or +254 XXX XXX XXX'
    case 'airtel':
      return '+27 XX XXX XXXX or +256 XXX XXX XXX'
    case 'mtn':
      return '+27 XX XXX XXXX'
    default:
      return '+27 XX XXX XXXX'
  }
}

const initiatePayment = async () => {
  if (!selectedProvider.value) return

  // Validate phone
  if (!validatePhoneNumber(phoneNumber.value, selectedProvider.value)) {
    phoneError.value = `Invalid phone number for ${getProviderName(selectedProvider.value)}`
    return
  }

  phoneError.value = ''

  // Initiate payment
  const result = await pay({
    amount: props.amount,
    phoneNumber: phoneNumber.value,
    provider: selectedProvider.value,
    reference: props.reference,
    description: props.description
  })

  if (result.success) {
    transactionId.value = result.transactionId || ''
    successMessage.value = result.message
    showSuccessModal.value = true
  } else {
    errorMessage.value = result.message
    showErrorModal.value = true
  }
}

const closeSuccessModal = () => {
  showSuccessModal.value = false
  emit('success', transactionId.value)
}

const closeErrorModal = () => {
  showErrorModal.value = false
}

// Watch for phone number changes
watch(phoneNumber, () => {
  phoneError.value = ''
})
</script>

<style scoped>
.mobile-money-payment {
  @apply max-w-md mx-auto;
}

.provider-button {
  @apply w-full p-4 border-2 border-gray-200 dark:border-gray-700 rounded-lg;
  @apply hover:border-blue-500 dark:hover:border-blue-400;
  @apply transition-all duration-200;
  @apply flex items-center justify-between;
  @apply bg-white dark:bg-gray-800;
}

.provider-button:hover {
  @apply shadow-lg transform scale-105;
}

.amount-display {
  @apply mb-6 p-6 bg-gradient-to-r from-blue-50 to-indigo-50 dark:from-blue-900/20 dark:to-indigo-900/20;
  @apply rounded-lg text-center;
}
</style>

