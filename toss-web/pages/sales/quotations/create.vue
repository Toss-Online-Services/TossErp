<template>
  <div class="container mx-auto px-4 py-8">
    <!-- Header -->
    <div class="mb-8">
      <div class="flex items-center gap-2 mb-2">
        <NuxtLink to="/sales/quotations" class="text-gray-600 hover:text-gray-900">
          <Icon name="mdi:arrow-left" size="24" />
        </NuxtLink>
        <h1 class="text-3xl font-bold">{{ t('sales.quotations.newQuotation') }}</h1>
      </div>
      <p class="text-gray-600">{{ t('sales.quotations.createSubtitle') }}</p>
    </div>

    <div v-if="pendingMeta" class="bg-white rounded-lg shadow-sm p-6">
      <div class="animate-pulse space-y-4">
        <div class="h-6 bg-gray-200 rounded w-1/3" />
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div class="h-10 bg-gray-200 rounded" />
          <div class="h-10 bg-gray-200 rounded" />
        </div>
        <div class="h-64 bg-gray-100 rounded" />
      </div>
    </div>

    <div v-else-if="metaError" class="bg-white rounded-lg shadow-sm p-6">
      <div class="text-center">
        <Icon name="mdi:alert" class="text-red-500 mx-auto mb-4" size="32" />
        <p class="text-gray-700 mb-4">{{ t('sales.quotations.metaLoadError') }}</p>
        <button class="btn btn-primary" @click="refreshMeta">{{ t('common.retry') }}</button>
      </div>
    </div>

    <QuotationForm
      v-else-if="meta"
      :meta="meta"
      mode="create"
      :submitting="isSubmitting"
      @submit="handleFormSubmit"
      @cancel="handleCancel"
    />
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { definePageMeta, useHead, useNuxtApp } from 'nuxt/app'
import type { $Fetch } from 'ofetch'
import type { QuotationMeta } from '../../../types/sales'
import { useToast } from '../../../composables/useToast'

const { t } = useI18n()
const router = useRouter()
const toast = useToast()
const nuxtApp = useNuxtApp()
const nuxtFetch = nuxtApp.$fetch as $Fetch

const isSubmitting = ref(false)
const metaData = ref<QuotationMeta | null>(null)
const pendingMeta = ref(true)
const metaError = ref<Error | null>(null)

const loadMeta = async () => {
  pendingMeta.value = true
  metaError.value = null

  try {
    const response = await nuxtFetch<{ data: QuotationMeta }>('/api/sales/quotations/meta')
    metaData.value = response.data
  } catch (error) {
    console.error('Failed to load quotation meta', error)
    metaError.value = error as Error
  } finally {
    pendingMeta.value = false
  }
}

loadMeta()

const refreshMeta = () => loadMeta()

const meta = computed(() => metaData.value)

const handleFormSubmit = async ({ status, body }: { status: 'draft' | 'sent'; body: Record<string, any> }) => {
  if (!meta.value) {
    toast.error(t('sales.quotations.metaLoadError'))
    return
  }

  const payload = { ...body, status }
  isSubmitting.value = true
  try {
    await nuxtFetch('/api/sales/quotations', {
      method: 'POST',
      body: payload
    })

    toast.success(t('sales.quotations.quotationCreated'))
    router.push('/sales/quotations')
  } catch (error) {
    console.error('Failed to create quotation', error)
    toast.error(t('sales.quotations.errorCreating'))
  } finally {
    isSubmitting.value = false
  }
}

const handleCancel = () => {
  router.push('/sales/quotations')
}

definePageMeta({
  layout: 'default',
  middleware: 'auth'
})

useHead({
  title: t('sales.quotations.newQuotation')
})
</script>
