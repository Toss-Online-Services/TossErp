<template>
  <div class="barcode-scanner">
    <!-- Scanner Modal -->
    <div v-if="isScanning" class="fixed inset-0 bg-black bg-opacity-75 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-slate-800 rounded-xl p-6 max-w-2xl w-full mx-4">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-slate-900 dark:text-white">Barcode Scanner</h3>
          <button @click="stopScanning" class="text-slate-600 dark:text-slate-400 hover:text-slate-900 dark:hover:text-white">
            <XMarkIcon class="w-6 h-6" />
          </button>
        </div>

        <!-- Scanner Type Selection -->
        <div class="flex gap-2 mb-4">
          <button 
            @click="scannerType = 'camera'"
            :class="[
              'flex-1 px-4 py-2 rounded-lg text-sm font-medium transition-colors',
              scannerType === 'camera' 
                ? 'bg-blue-600 text-white' 
                : 'bg-slate-100 dark:bg-slate-700 text-slate-700 dark:text-slate-300'
            ]"
          >
            <CameraIcon class="w-4 h-4 inline mr-2" />
            Camera
          </button>
          <button 
            @click="scannerType = 'usb'"
            :class="[
              'flex-1 px-4 py-2 rounded-lg text-sm font-medium transition-colors',
              scannerType === 'usb' 
                ? 'bg-blue-600 text-white' 
                : 'bg-slate-100 dark:bg-slate-700 text-slate-700 dark:text-slate-300'
            ]"
          >
            <QrCodeIcon class="w-4 h-4 inline mr-2" />
            USB Scanner
          </button>
        </div>

        <!-- Camera Scanner -->
        <div v-if="scannerType === 'camera'" class="space-y-4">
          <div class="relative bg-slate-900 rounded-lg overflow-hidden" style="aspect-ratio: 4/3;">
            <video ref="videoElement" class="w-full h-full object-cover" autoplay playsinline></video>
            <div class="absolute inset-0 flex items-center justify-center pointer-events-none">
              <div class="w-64 h-32 border-2 border-green-500 rounded-lg"></div>
            </div>
            <canvas ref="canvasElement" class="hidden"></canvas>
          </div>
          
          <!-- Camera Controls -->
          <div class="flex gap-2">
            <select v-model="selectedCamera" @change="switchCamera" 
                    class="flex-1 px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg dark:bg-slate-700 dark:text-white">
              <option v-for="camera in availableCameras" :key="camera.deviceId" :value="camera.deviceId">
                {{ camera.label || `Camera ${camera.deviceId.substring(0, 8)}` }}
              </option>
            </select>
            <button @click="toggleFlash" 
                    class="px-4 py-2 bg-slate-200 dark:bg-slate-700 rounded-lg hover:bg-slate-300 dark:hover:bg-slate-600">
              <BoltIcon class="w-5 h-5" :class="flashEnabled ? 'text-yellow-500' : 'text-slate-600 dark:text-slate-400'" />
            </button>
          </div>

          <!-- Last Scanned -->
          <div v-if="lastScanned" class="bg-green-50 dark:bg-green-900/20 p-4 rounded-lg">
            <p class="text-sm text-green-800 dark:text-green-200 font-medium">Last Scanned:</p>
            <p class="text-lg font-bold text-green-900 dark:text-green-100">{{ lastScanned }}</p>
          </div>
        </div>

        <!-- USB Scanner -->
        <div v-if="scannerType === 'usb'" class="space-y-4">
          <div class="bg-slate-50 dark:bg-slate-700 p-8 rounded-lg text-center">
            <QrCodeIcon class="w-16 h-16 text-slate-400 mx-auto mb-4" />
            <p class="text-slate-900 dark:text-white font-medium mb-2">USB Barcode Scanner Ready</p>
            <p class="text-sm text-slate-600 dark:text-slate-400">Scan any product barcode to add to cart</p>
          </div>

          <!-- Scanner Status -->
          <div class="flex items-center justify-center gap-2">
            <div class="w-3 h-3 bg-green-500 rounded-full animate-pulse"></div>
            <span class="text-sm text-slate-600 dark:text-slate-400">Listening for barcode input...</span>
          </div>

          <!-- Last Scanned -->
          <div v-if="lastScanned" class="bg-green-50 dark:bg-green-900/20 p-4 rounded-lg">
            <p class="text-sm text-green-800 dark:text-green-200 font-medium">Last Scanned:</p>
            <p class="text-lg font-bold text-green-900 dark:text-green-100">{{ lastScanned }}</p>
          </div>

          <!-- Manual Input Fallback -->
          <div>
            <label class="block text-sm font-medium text-slate-700 dark:text-slate-300 mb-2">Manual Barcode Entry</label>
            <div class="flex gap-2">
              <input 
                v-model="manualBarcode"
                @keyup.enter="processManualBarcode"
                type="text"
                placeholder="Enter barcode manually..."
                class="flex-1 px-3 py-2 border border-slate-300 dark:border-slate-600 rounded-lg dark:bg-slate-700 dark:text-white"
              />
              <button @click="processManualBarcode" 
                      class="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg">
                Add
              </button>
            </div>
          </div>
        </div>

        <!-- Scanner Stats -->
        <div class="grid grid-cols-3 gap-4 mt-4">
          <div class="text-center">
            <p class="text-2xl font-bold text-blue-600">{{ scannedCount }}</p>
            <p class="text-xs text-slate-600 dark:text-slate-400">Scanned</p>
          </div>
          <div class="text-center">
            <p class="text-2xl font-bold text-green-600">{{ successCount }}</p>
            <p class="text-xs text-slate-600 dark:text-slate-400">Success</p>
          </div>
          <div class="text-center">
            <p class="text-2xl font-bold text-red-600">{{ failedCount }}</p>
            <p class="text-xs text-slate-600 dark:text-slate-400">Failed</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import { XMarkIcon, QrCodeIcon, CameraIcon, BoltIcon } from '@heroicons/vue/24/outline'

const props = defineProps<{
  modelValue: boolean
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  'barcode-scanned': [barcode: string]
}>()

// State
const isScanning = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

const scannerType = ref<'camera' | 'usb'>('usb')
const videoElement = ref<HTMLVideoElement>()
const canvasElement = ref<HTMLCanvasElement>()
const selectedCamera = ref('')
const availableCameras = ref<MediaDeviceInfo[]>([])
const flashEnabled = ref(false)
const lastScanned = ref('')
const manualBarcode = ref('')

// Stats
const scannedCount = ref(0)
const successCount = ref(0)
const failedCount = ref(0)

// Camera stream
let stream: MediaStream | null = null
let scanInterval: ReturnType<typeof setInterval> | null = null

// Barcode detection
let barcodeBuffer = ''
let barcodeTimeout: ReturnType<typeof setTimeout> | null = null

onMounted(async () => {
  await getCameras()
  setupKeyboardListener()
})

onUnmounted(() => {
  stopScanning()
  cleanupKeyboardListener()
})

// Camera functions
const getCameras = async () => {
  try {
    const devices = await navigator.mediaDevices.enumerateDevices()
    availableCameras.value = devices.filter(device => device.kind === 'videoinput')
    if (availableCameras.value.length > 0 && availableCameras.value[0]) {
      selectedCamera.value = availableCameras.value[0].deviceId
    }
  } catch (error) {
    console.error('Failed to get cameras:', error)
  }
}

const startCamera = async () => {
  try {
    const constraints: MediaStreamConstraints = {
      video: {
        deviceId: selectedCamera.value ? { exact: selectedCamera.value } : undefined,
        facingMode: 'environment',
        width: { ideal: 1280 },
        height: { ideal: 720 }
      }
    }

    stream = await navigator.mediaDevices.getUserMedia(constraints)
    
    if (videoElement.value) {
      videoElement.value.srcObject = stream
      await videoElement.value.play()
      startBarcodeDetection()
    }
  } catch (error) {
    console.error('Failed to start camera:', error)
    alert('Failed to access camera. Please check permissions.')
  }
}

const stopCamera = () => {
  if (stream) {
    stream.getTracks().forEach(track => track.stop())
    stream = null
  }
  
  if (scanInterval) {
    clearInterval(scanInterval)
    scanInterval = null
  }
}

const switchCamera = async () => {
  stopCamera()
  await startCamera()
}

const toggleFlash = async () => {
  if (!stream) return
  
  try {
    const track = stream.getVideoTracks()[0]
    if (!track) return
    
    const capabilities = track.getCapabilities() as any
    
    if (capabilities.torch) {
      flashEnabled.value = !flashEnabled.value
      await track.applyConstraints({
        advanced: [{ torch: flashEnabled.value }]
      } as any)
    }
  } catch (error) {
    console.error('Flash not supported:', error)
  }
}

const startBarcodeDetection = () => {
  scanInterval = setInterval(() => {
    detectBarcode()
  }, 500) // Scan every 500ms
}

const detectBarcode = async () => {
  if (!videoElement.value || !canvasElement.value) return

  const video = videoElement.value
  const canvas = canvasElement.value
  const context = canvas.getContext('2d')
  
  if (!context) return

  // Set canvas size to match video
  canvas.width = video.videoWidth
  canvas.height = video.videoHeight

  // Draw current video frame to canvas
  context.drawImage(video, 0, 0, canvas.width, canvas.height)

  // Get image data
  const imageData = context.getImageData(0, 0, canvas.width, canvas.height)

  try {
    // Use browser's built-in barcode detector if available
    if ('BarcodeDetector' in window) {
      const barcodeDetector = new (window as any).BarcodeDetector({
        formats: ['code_128', 'code_39', 'ean_13', 'ean_8', 'upc_a', 'upc_e', 'qr_code']
      })
      
      const barcodes = await barcodeDetector.detect(canvas)
      
      if (barcodes.length > 0) {
        const barcode = barcodes[0].rawValue
        handleBarcodeDetected(barcode)
      }
    } else {
      // Fallback: Use external library (would need to integrate Dynamsoft or similar)
      console.log('BarcodeDetector not supported, using fallback')
    }
  } catch (error) {
    console.error('Barcode detection error:', error)
  }
}

// Keyboard listener for USB scanners
const setupKeyboardListener = () => {
  document.addEventListener('keypress', handleKeyPress)
}

const cleanupKeyboardListener = () => {
  document.removeEventListener('keypress', handleKeyPress)
}

const handleKeyPress = (event: KeyboardEvent) => {
  // Only process if scanner is active
  if (!isScanning.value) return

  // Clear timeout
  if (barcodeTimeout) {
    clearTimeout(barcodeTimeout)
  }

  // Add character to buffer
  if (event.key.length === 1) {
    barcodeBuffer += event.key
  }

  // Process on Enter (scanners send Enter after barcode)
  if (event.key === 'Enter' && barcodeBuffer.length >= 8) {
    handleBarcodeDetected(barcodeBuffer)
    barcodeBuffer = ''
    event.preventDefault()
    return
  }

  // Reset buffer after 100ms (scanners are fast)
  barcodeTimeout = setTimeout(() => {
    barcodeBuffer = ''
  }, 100)
}

const handleBarcodeDetected = (barcode: string) => {
  lastScanned.value = barcode
  scannedCount.value++
  
  // Emit to parent
  emit('barcode-scanned', barcode)
  
  // Visual/audio feedback
  playBeep()
  flashScreen()
}

const processManualBarcode = () => {
  if (manualBarcode.value.trim()) {
    handleBarcodeDetected(manualBarcode.value.trim())
    manualBarcode.value = ''
  }
}

const playBeep = () => {
  // Create audio beep
  const audioContext = new (window.AudioContext || (window as any).webkitAudioContext)()
  const oscillator = audioContext.createOscillator()
  const gainNode = audioContext.createGain()
  
  oscillator.connect(gainNode)
  gainNode.connect(audioContext.destination)
  
  oscillator.frequency.value = 800
  gainNode.gain.value = 0.3
  
  oscillator.start()
  setTimeout(() => oscillator.stop(), 100)
}

const flashScreen = () => {
  // Brief flash effect
  const flash = document.createElement('div')
  flash.className = 'fixed inset-0 bg-green-500 opacity-30 z-50 pointer-events-none'
  document.body.appendChild(flash)
  setTimeout(() => flash.remove(), 100)
}

const stopScanning = () => {
  stopCamera()
  isScanning.value = false
}

// Watch for scanner type changes
watch(scannerType, (newType) => {
  if (newType === 'camera') {
    startCamera()
  } else {
    stopCamera()
  }
})

// Watch for scanning state changes
watch(isScanning, (newValue) => {
  if (newValue && scannerType.value === 'camera') {
    startCamera()
  } else {
    stopCamera()
  }
})
</script>

