<template>
  <div class="p-4 sm:p-6 space-y-6">
    <div class="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
      <div>
        <h1 class="text-2xl font-bold text-foreground">
          {{ translate('sales.quotations.create.title', 'New quotation') }}
        </h1>
        <p class="text-muted-foreground">
          {{ translate('sales.quotations.create.subtitle', 'Prepare a proposal for your customer') }}
        </p>
      </div>
      <Button variant="ghost" as-child>
        <NuxtLink to="/sales/quotations">
          {{ translate('common.viewList', 'Back to list') }}
        </NuxtLink>
      </Button>
    </div>

    <QuotationForm mode="create" @submitted="handleSubmitted" @cancel="handleCancel" />
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'

import { Button } from '~/components/ui/button'
import QuotationForm from '~/components/sales/quotations/QuotationForm.vue'
import type { Quotation } from '~/types/sales'
import { useToast } from '~/composables/useToast'

const router = useRouter()
const { t } = useI18n()
const toast = useToast()

const translate = (key: string, fallback: string, params?: Record<string, unknown>) => {
  const result = t(key, params ?? {})
  return result === key ? fallback : result
}

const handleSubmitted = (quotation: Quotation) => {
  toast.success(translate('sales.quotations.create.success', 'Quotation created successfully'))
  router.push(`/sales/quotations/${quotation.id}`)
}

const handleCancel = () => {
  router.push('/sales/quotations')
}
</script>
