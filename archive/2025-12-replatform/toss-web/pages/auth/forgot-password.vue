<template>
  <div class="min-h-screen flex items-center justify-center bg-background px-4">
    <!-- Decorative background elements -->
    <div class="absolute inset-0 overflow-hidden pointer-events-none">
      <div class="absolute top-20 left-10 w-72 h-72 bg-primary/5 rounded-full blur-3xl"></div>
      <div class="absolute bottom-20 right-10 w-96 h-96 bg-primary/5 rounded-full blur-3xl"></div>
    </div>

    <div class="max-w-md w-full space-y-8 relative z-10">
      <!-- Logo and Title -->
      <div class="text-center">
        <NuxtLink to="/" class="inline-flex justify-center mb-6 group">
          <div class="relative flex items-center justify-center w-16 h-16 bg-primary text-primary-foreground rounded-2xl shadow-xl transition-transform group-hover:scale-110 group-hover:rotate-3">
            <span class="text-3xl font-black text-white">T</span>
            <div class="absolute inset-0 bg-primary/20 rounded-2xl opacity-0 group-hover:opacity-20 transition-opacity"></div>
          </div>
        </NuxtLink>
        <h2 class="text-3xl font-bold bg-gradient-to-r from-slate-900 to-slate-700 dark:from-white dark:to-slate-300 bg-clip-text text-transparent">Reset Password</h2>
        <p class="mt-2 text-sm text-slate-600 dark:text-slate-400">
          Enter your phone number or email to receive a reset link
        </p>
      </div>

      <!-- Reset Form -->
      <MaterialCard variant="elevated" class="p-8">
        <form v-if="!sent" @submit.prevent="handleReset" class="space-y-6">
          <MaterialInput
            v-model="form.identifier"
            label="Phone Number or Email"
            type="text"
            placeholder="+27 XX XXX XXXX or email@example.com"
            required
            variant="outlined"
          />

          <MaterialButton
            type="submit"
            :disabled="loading"
            :loading="loading"
            color="primary"
            size="lg"
            full-width
          >
            {{ loading ? 'Sending...' : 'Send Reset Link' }}
          </MaterialButton>
        </form>

        <!-- Success Message -->
        <div v-else class="text-center py-4">
          <div class="w-16 h-16 bg-gradient-to-br from-green-100 to-emerald-100 dark:from-green-900/30 dark:to-emerald-900/30 rounded-full flex items-center justify-center mx-auto mb-4 shadow-lg">
            <CheckCircleIcon class="w-10 h-10 text-green-600 dark:text-green-400" />
          </div>
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-2">
            Reset Link Sent!
          </h3>
          <p class="text-sm text-slate-600 dark:text-slate-400 mb-6">
            We've sent a password reset link to {{ form.identifier }}.
            Check your WhatsApp messages or email.
          </p>
          <MaterialButton
            @click="sent = false"
            variant="text"
            color="primary"
          >
            ← Try a different number/email
          </MaterialButton>
        </div>
      </MaterialCard>

      <!-- Back to Login -->
      <p class="text-center text-sm text-slate-600 dark:text-slate-400">
        Remember your password?
        <NuxtLink to="/auth/login" class="text-primary hover:text-primary/80 font-semibold">
          Sign in
        </NuxtLink>
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { CheckCircleIcon } from '@heroicons/vue/24/outline'

definePageMeta({
  layout: false,
  middleware: []
})

useHead({
  title: 'Forgot Password - TOSS ERP',
  meta: [
    { name: 'description', content: 'Reset your TOSS ERP password' }
  ]
})

const form = ref({
  identifier: ''
})

const loading = ref(false)
const sent = ref(false)

const handleReset = async () => {
  loading.value = true
  try {
    await $fetch('/api/auth/forgot-password', {
      method: 'POST',
      body: form.value
    })
    sent.value = true
  } catch (error) {
    console.error('Reset failed:', error)
    alert('Failed to send reset link. Please try again.')
  } finally {
    loading.value = false
  }
}
</script>

