<template>
  <div v-if="show" class="fixed inset-0 bg-slate-600 bg-opacity-50 overflow-y-auto h-full w-full z-50 flex items-center justify-center p-4">
    <div class="relative bg-white dark:bg-slate-800 rounded-2xl shadow-2xl border border-slate-200 dark:border-slate-700 w-full max-w-lg p-6">
      <!-- Header -->
      <div class="flex items-center justify-between mb-6">
        <h3 class="text-xl font-bold text-slate-900 dark:text-white">Capture Proof of Delivery</h3>
        <button
          @click="emit('close')"
          class="text-slate-400 hover:text-slate-600 dark:hover:text-slate-300 transition-colors"
        >
          <XMarkIcon class="w-6 h-6" />
        </button>
      </div>

      <!-- Stop Info -->
      <div v-if="stop" class="mb-6 p-4 bg-slate-50 dark:bg-slate-700/50 rounded-xl">
        <p class="text-sm text-slate-600 dark:text-slate-400 mb-1">Delivery to</p>
        <p class="text-lg font-bold text-slate-900 dark:text-white">{{ stop.shopName }}</p>
        <p class="text-sm text-slate-600 dark:text-slate-400">{{ stop.address }}</p>
        <p class="text-xs text-slate-500 dark:text-slate-400 mt-2">{{ stop.items }} items</p>
      </div>

      <!-- POD Method Selection -->
      <div class="mb-6">
        <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-3">
          Select Proof Method
        </label>
        <div class="grid grid-cols-2 gap-3">
          <button
            @click="podMethod = 'pin'"
            :class="[
              'p-4 border-2 rounded-xl text-left transition-all',
              podMethod === 'pin'
                ? 'border-blue-600 bg-blue-50 dark:bg-blue-900/20'
                : 'border-slate-200 dark:border-slate-700 hover:border-slate-300 dark:hover:border-slate-600'
            ]"
          >
            <div class="flex items-center gap-3">
              <div :class="[
                'p-2 rounded-lg',
                podMethod === 'pin' ? 'bg-blue-100 dark:bg-blue-900/30' : 'bg-slate-100 dark:bg-slate-700'
              ]">
                <KeyIcon :class="[
                  'w-5 h-5',
                  podMethod === 'pin' ? 'text-blue-600 dark:text-blue-400' : 'text-slate-600 dark:text-slate-400'
                ]" />
              </div>
              <div>
                <p :class="[
                  'text-sm font-medium',
                  podMethod === 'pin' ? 'text-blue-900 dark:text-blue-100' : 'text-slate-900 dark:text-white'
                ]">PIN Code</p>
                <p class="text-xs text-slate-500 dark:text-slate-400">Customer enters PIN</p>
              </div>
            </div>
          </button>

          <button
            @click="podMethod = 'photo'"
            :class="[
              'p-4 border-2 rounded-xl text-left transition-all',
              podMethod === 'photo'
                ? 'border-blue-600 bg-blue-50 dark:bg-blue-900/20'
                : 'border-slate-200 dark:border-slate-700 hover:border-slate-300 dark:hover:border-slate-600'
            ]"
          >
            <div class="flex items-center gap-3">
              <div :class="[
                'p-2 rounded-lg',
                podMethod === 'photo' ? 'bg-blue-100 dark:bg-blue-900/30' : 'bg-slate-100 dark:bg-slate-700'
              ]">
                <CameraIcon :class="[
                  'w-5 h-5',
                  podMethod === 'photo' ? 'text-blue-600 dark:text-blue-400' : 'text-slate-600 dark:text-slate-400'
                ]" />
              </div>
              <div>
                <p :class="[
                  'text-sm font-medium',
                  podMethod === 'photo' ? 'text-blue-900 dark:text-blue-100' : 'text-slate-900 dark:text-white'
                ]">Photo</p>
                <p class="text-xs text-slate-500 dark:text-slate-400">Take a picture</p>
              </div>
            </div>
          </button>
        </div>
      </div>

      <!-- PIN Input -->
      <div v-if="podMethod === 'pin'" class="mb-6">
        <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
          Enter Delivery PIN
        </label>
        <input
          v-model="pinCode"
          type="text"
          maxlength="4"
          pattern="[0-9]*"
          inputmode="numeric"
          class="w-full px-4 py-3 text-center text-2xl font-bold tracking-widest border border-slate-300 dark:border-slate-600 rounded-xl bg-white dark:bg-slate-700 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 focus:border-transparent transition-all"
          placeholder="0000"
          @input="pinCode = pinCode.replace(/[^0-9]/g, '')"
        />
        <p class="mt-2 text-xs text-slate-500 dark:text-slate-400">
          Ask the customer for their 4-digit delivery PIN
        </p>
      </div>

      <!-- Photo Capture -->
      <div v-if="podMethod === 'photo'" class="mb-6">
        <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
          Take Photo
        </label>
        <div v-if="!photoPreview" class="border-2 border-dashed border-slate-300 dark:border-slate-600 rounded-xl p-8 text-center">
          <CameraIcon class="w-12 h-12 text-slate-400 mx-auto mb-3" />
          <p class="text-sm text-slate-600 dark:text-slate-400 mb-4">Take a photo of the delivered items</p>
          <button
            @click="capturePhoto"
            class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg font-medium transition-colors"
          >
            Open Camera
          </button>
        </div>
        <div v-else class="relative">
          <img :src="photoPreview" alt="Delivery photo" class="w-full rounded-xl" />
          <button
            @click="photoPreview = null"
            class="absolute top-2 right-2 p-2 bg-red-600 hover:bg-red-700 text-white rounded-lg transition-colors"
          >
            <XMarkIcon class="w-5 h-5" />
          </button>
        </div>
      </div>

      <!-- Notes -->
      <div class="mb-6">
        <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">
          Delivery Notes (Optional)
        </label>
        <textarea
          v-model="notes"
          rows="3"
          class="w-full px-4 py-2 border border-slate-300 dark:border-slate-600 rounded-xl bg-white dark:bg-slate-700 text-slate-900 dark:text-white focus:ring-2 focus:ring-blue-500 dark:focus:ring-blue-400 focus:border-transparent transition-all resize-none"
          placeholder="Any additional notes about the delivery..."
        ></textarea>
      </div>

      <!-- Actions -->
      <div class="flex gap-3">
        <button
          @click="emit('close')"
          class="flex-1 px-4 py-3 border border-slate-300 dark:border-slate-600 text-slate-700 dark:text-slate-300 rounded-xl hover:bg-slate-50 dark:hover:bg-slate-700 font-medium transition-colors"
        >
          Cancel
        </button>
        <button
          @click="handleSubmit"
          :disabled="!canSubmit"
          class="flex-1 px-4 py-3 bg-gradient-to-r from-green-600 to-blue-600 hover:from-green-700 hover:to-blue-700 disabled:from-slate-400 disabled:to-slate-500 text-white rounded-xl font-semibold shadow-lg hover:shadow-xl transition-all duration-200 disabled:cursor-not-allowed"
        >
          Confirm Delivery
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { XMarkIcon, CameraIcon, KeyIcon } from '@heroicons/vue/24/outline'

interface Props {
  show: boolean
  stop: any
}

const props = defineProps<Props>()

const emit = defineEmits<{
  close: []
  capture: [payload: { stop: any; pod: any }]
}>()

const podMethod = ref<'pin' | 'photo'>('pin')
const pinCode = ref('')
const photoPreview = ref<string | null>(null)
const notes = ref('')

const canSubmit = computed(() => {
  if (podMethod.value === 'pin') {
    return pinCode.value.length === 4
  } else {
    return photoPreview.value !== null
  }
})

const capturePhoto = () => {
  // In a real app, this would open the device camera
  // For now, we'll simulate with a placeholder
  const canvas = document.createElement('canvas')
  canvas.width = 400
  canvas.height = 300
  const ctx = canvas.getContext('2d')
  if (ctx) {
    ctx.fillStyle = '#f0f0f0'
    ctx.fillRect(0, 0, 400, 300)
    ctx.fillStyle = '#666'
    ctx.font = '20px sans-serif'
    ctx.textAlign = 'center'
    ctx.fillText('Delivery Photo', 200, 150)
    photoPreview.value = canvas.toDataURL()
  }
}

const handleSubmit = () => {
  const podData = {
    method: podMethod.value,
    timestamp: new Date(),
    notes: notes.value
  }

  if (podMethod.value === 'pin') {
    Object.assign(podData, { pinCode: pinCode.value })
  } else {
    Object.assign(podData, { photo: photoPreview.value })
  }

  emit('capture', { stop: props.stop, pod: podData })
  
  // Reset form
  pinCode.value = ''
  photoPreview.value = null
  notes.value = ''
  podMethod.value = 'pin'
}
</script>

