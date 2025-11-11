<template>
  <div class="min-h-screen flex items-center justify-center bg-slate-50 dark:bg-slate-900 px-4">
    <div class="max-w-md w-full space-y-8">
      <!-- Logo and Title -->
      <div class="text-center">
        <div class="flex justify-center mb-4">
          <div class="w-16 h-16 bg-gradient-to-r from-blue-500 to-purple-600 rounded-lg flex items-center justify-center">
            <span class="text-3xl font-bold text-white">T</span>
          </div>
        </div>
        <h2 class="text-3xl font-bold text-slate-900 dark:text-white">Reset Password</h2>
        <p class="mt-2 text-sm text-slate-600 dark:text-slate-400">
          Enter your phone number or email to receive a reset link
        </p>
      </div>

      <!-- Reset Form -->
      <div class="bg-white dark:bg-slate-800 rounded-lg shadow-lg border border-slate-200 dark:border-slate-700 p-8">
        <form v-if="!sent" @submit.prevent="handleReset" class="space-y-6">
          <div>
            <label for="identifier" class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
              Phone Number or Email
            </label>
            <input
              id="identifier"
              v-model="form.identifier"
              type="text"
              required
              class="w-full px-4 py-3 border border-slate-300 dark:border-slate-600 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent bg-white dark:bg-slate-900 text-slate-900 dark:text-white"
              placeholder="+27 XX XXX XXXX or email@example.com"
            />
          </div>

          <button
            type="submit"
            :disabled="loading"
            class="w-full py-3 px-4 bg-blue-600 hover:bg-blue-700 text-white font-medium rounded-lg transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {{ loading ? 'Sending...' : 'Send Reset Link' }}
          </button>
        </form>

        <!-- Success Message -->
        <div v-else class="text-center py-4">
          <div class="w-16 h-16 bg-green-100 dark:bg-green-900/30 rounded-full flex items-center justify-center mx-auto mb-4">
            <CheckCircleIcon class="w-10 h-10 text-green-600 dark:text-green-400" />
          </div>
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white mb-2">
            Reset Link Sent!
          </h3>
          <p class="text-sm text-slate-600 dark:text-slate-400 mb-6">
            We've sent a password reset link to {{ form.identifier }}.
            Check your WhatsApp messages or email.
          </p>
          <button
            @click="sent = false"
            class="text-sm text-blue-600 hover:text-blue-700 dark:text-blue-400 dark:hover:text-blue-300 font-medium"
          >
            ← Try a different number/email
          </button>
        </div>
      </div>

      <!-- Back to Login -->
      <p class="text-center text-sm text-slate-600 dark:text-slate-400">
        Remember your password?
        <NuxtLink to="/auth/login" class="text-blue-600 hover:text-blue-700 dark:text-blue-400 dark:hover:text-blue-300 font-medium">
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

