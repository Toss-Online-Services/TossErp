<script setup lang="ts">
import * as Sentry from '@sentry/nuxt'

function throwClientError() {
  try {
    throw new Error('Sentry client test error')
  } catch (e) {
    // Also capture manually to verify SDK initialization
    Sentry.captureException(e)
    throw e
  }
}

async function callServerError() {
  await $fetch('/api/sentry-example')
}
</script>

<template>
  <div class="p-6 space-y-4">
    <h1 class="text-2xl font-semibold">Sentry Example Page</h1>
    <p class="text-gray-600">Use these buttons to trigger sample errors for Sentry verification.</p>
    <div class="flex gap-3">
      <button class="px-4 py-2 bg-red-600 text-white rounded" @click="throwClientError">
        Throw client error
      </button>
      <button class="px-4 py-2 bg-amber-600 text-white rounded" @click="callServerError">
        Call server error
      </button>
    </div>
    <p class="text-sm text-gray-500">Ensure NUXT_PUBLIC_SENTRY_DSN is set; then open Sentry to see captured issues.</p>
  </div>
  
</template>
