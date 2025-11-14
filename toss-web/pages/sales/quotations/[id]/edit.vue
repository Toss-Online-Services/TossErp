<script setup lang="ts">
import { computed, ref, watch, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'

import { Button } from '~/components/ui/button'
import { Card, CardContent } from '~/components/ui/card'
import { Alert, AlertDescription, AlertTitle } from '~/components/ui/alert'
import { Skeleton } from '~/components/ui/skeleton'
import QuotationForm from '~/components/sales/quotations/QuotationForm.vue'
import { useQuotations } from '~/composables/useQuotations'
import { useToast } from '~/composables/useToast'
import type { Quotation } from '~/types/sales'

const route = useRoute()
const router = useRouter()
const { t } = useI18n()
const toast = useToast()

const translate = (key: string, fallback: string, params?: Record<string, unknown>) => {
  const result = t(key, params ?? {})
  return result === key ? fallback : result
}

const quotationId = computed(() => String(route.params.id))

const {
  fetchQuotation,
  currentQuotation,
  updateQuotation
} = useQuotations()

const quotation = computed(() => currentQuotation.value as Quotation | null)
const loadError = ref<string | null>(null)
const isInitialLoading = ref(true)

const loadQuotation = async () => {
  if (!quotationId.value) {
    loadError.value = translate('sales.quotations.edit.missingId', 'Quotation identifier is missing.')
    isInitialLoading.value = false
    return
  }

  loadError.value = null
  isInitialLoading.value = true
  try {
    await fetchQuotation(quotationId.value)
  } catch (err: any) {
    loadError.value = err?.message ?? translate('sales.quotations.edit.loadFailed', 'Unable to load quotation. Try again later.')
  } finally {
    isInitialLoading.value = false
  }
}

onMounted(loadQuotation)

watch(
  () => route.params.id,
  async () => {
    await loadQuotation()
  }
)

const handleSubmitted = (quotation: Quotation) => {
  toast.success(translate('sales.quotations.edit.success', 'Quotation updated successfully.'))
  router.push(`/sales/quotations/${quotation.id}`)
}

const handleCancel = () => {
  router.push('/sales/quotations')
}
</script>

<template>
  <div class="p-4 sm:p-6 space-y-6">
    <div class="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <h1 class="text-2xl font-bold text-foreground">
          {{ translate('sales.quotations.edit.title', 'Edit quotation') }} #{{ quotationId }}
        </h1>
        <p class="text-muted-foreground">
          {{ translate('sales.quotations.edit.subtitle', 'Update pricing, items, and notes before confirming with the customer') }}
        </p>
      </div>
      <Button variant="ghost" as-child>
        <NuxtLink to="/sales/quotations">
          {{ translate('common.viewList', 'Back to list') }}
        </NuxtLink>
      </Button>
    </div>

    <Alert v-if="loadError" variant="destructive">
      <AlertTitle>{{ translate('common.error', 'Something went wrong') }}</AlertTitle>
      <AlertDescription>{{ loadError }}</AlertDescription>
    </Alert>

    <Card v-if="isInitialLoading">
      <CardContent class="space-y-4 py-6">
        <Skeleton class="h-6 w-56" />
        <Skeleton class="h-4 w-72" />
        <Skeleton class="h-32 w-full" />
        <Skeleton class="h-32 w-full" />
      </CardContent>
    </Card>

    <QuotationForm
      v-else-if="quotation"
      mode="edit"
      :quotation="quotation"
      @submitted="handleSubmitted"
      @cancel="handleCancel"
    />

    <Card v-else>
      <CardContent class="py-10 text-center text-muted-foreground">
        {{ translate('sales.quotations.details.notFound', 'Quotation not found.') }}
      </CardContent>
    </Card>
  </div>
</template>

<style scoped>
.card { @apply bg-white dark:bg-gray-800 shadow-sm rounded-lg; }
.card-header { @apply p-4 border-b border-gray-200 dark:border-gray-700; }
.card-title { @apply text-lg font-semibold text-gray-800 dark:text-gray-200; }
.card-body { @apply p-4; }
.card-footer { @apply p-4 bg-gray-50 dark:bg-gray-900/50 border-t border-gray-200 dark:border-gray-700 rounded-b-lg; }
.btn { @apply inline-flex items-center justify-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-offset-2; }
.btn-secondary { @apply text-gray-700 bg-gray-100 hover:bg-gray-200 focus:ring-gray-500 dark:bg-gray-700 dark:text-gray-200 dark:hover:bg-gray-600; }
.btn-danger-outline { @apply text-red-700 bg-white border-red-300 hover:bg-red-50 focus:ring-red-500 dark:bg-gray-800 dark:border-red-600 dark:text-red-400 dark:hover:bg-gray-700; }
</style>
