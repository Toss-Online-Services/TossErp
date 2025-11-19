<template>
  <div class="container mx-auto px-4 py-8">
    <div class="mb-8">
      <div class="flex items-center gap-2 mb-2">
        <NuxtLink :to="`/sales/quotations/${route.params.id}`" class="text-gray-600 hover:text-gray-900">
          <Icon name="mdi:arrow-left" size="24" />
        </NuxtLink>
        <h1 class="text-3xl font-bold">{{ t('sales.quotations.editQuotation') }}</h1>
      </div>
      <p class="text-gray-600">{{ t('sales.quotations.editSubtitle') }}</p>
    </div>

    <div v-if="loading" class="bg-white rounded-lg shadow-sm p-6">
      <div class="animate-pulse space-y-4">
        <div class="h-6 bg-gray-200 rounded w-1/3" />
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div class="h-10 bg-gray-200 rounded" />
          <div class="h-10 bg-gray-200 rounded" />
        </div>
        <div class="h-64 bg-gray-100 rounded" />
      </div>
    </div>

    <div v-else-if="loadError" class="bg-white rounded-lg shadow-sm p-6 text-center">
      <Icon name="mdi:alert" class="text-red-500 mx-auto mb-4" size="32" />
      <p class="text-gray-700 mb-4">{{ loadError }}</p>
      <div class="flex justify-center gap-3">
        <button class="btn btn-secondary" @click="handleCancel">{{ t('common.back') }}</button>
        <button class="btn btn-primary" @click="loadResources">{{ t('common.retry') }}</button>
      </div>
    </div>

    <QuotationForm
      v-else-if="meta && quotation"
      :meta="meta"
      :initial-quotation="quotation"
      mode="edit"
      :submitting="isSubmitting"
      @submit="handleFormSubmit"
      @cancel="handleCancel"
    />
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { definePageMeta, useHead, useNuxtApp } from 'nuxt/app'
import type { $Fetch } from 'ofetch'
import type { QuotationMeta, QuotationRecord } from '../../../../types/sales'
import { useToast } from '../../../../composables/useToast'

const { t } = useI18n()
const router = useRouter()
const route = useRoute()
const toast = useToast()
const nuxtApp = useNuxtApp()
const nuxtFetch = nuxtApp.$fetch as $Fetch

const isSubmitting = ref(false)
const loading = ref(true)
const loadError = ref<string | null>(null)
const metaData = ref<QuotationMeta | null>(null)
const quotation = ref<QuotationRecord | null>(null)

const meta = computed(() => metaData.value)
const quotationId = computed(() => route.params.id as string)

const loadResources = async () => {
  loading.value = true
  loadError.value = null

  try {
    const [metaResponse, quotationResponse] = await Promise.all([
      nuxtFetch<{ data: QuotationMeta }>('/api/sales/quotations/meta'),
      nuxtFetch<{ data: QuotationRecord }>(`/api/sales/quotations/${quotationId.value}`)
    ])

    metaData.value = metaResponse.data
    quotation.value = quotationResponse.data
  } catch (error) {
    console.error('Failed to load quotation resources', error)
    loadError.value = t('sales.quotations.errorLoading')
  } finally {
    loading.value = false
  }
}

loadResources()

const handleFormSubmit = async ({ status, body }: { status: 'draft' | 'sent'; body: Record<string, any> }) => {
  if (!quotation.value) {
    toast.error(t('sales.quotations.errorLoading'))
    return
  }

  isSubmitting.value = true
  try {
    await nuxtFetch(`/api/sales/quotations/${quotationId.value}`, {
      method: 'PUT',
      body: {
        ...body,
        status
      }
    })

    toast.success(t('sales.quotations.quotationUpdated'))
    router.push(`/sales/quotations/${quotationId.value}`)
  } catch (error) {
    console.error('Failed to update quotation', error)
    toast.error(t('sales.quotations.errorSaving'))
  } finally {
    isSubmitting.value = false
  }
}

const handleCancel = () => {
  router.push(`/sales/quotations/${quotationId.value}`)
}

definePageMeta({
  layout: 'default',
  middleware: 'auth'
})

useHead({
  title: computed(() => `${t('sales.quotations.editQuotation')} • ${quotation.value?.quotationNumber || ''}`)
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
