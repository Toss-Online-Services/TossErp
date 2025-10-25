<template>
  <div class="voice-input-container">
    <!-- Voice Button -->
    <button
      @click="handleVoiceToggle"
      :disabled="!isSupported"
      :class="[
        'voice-button',
        {
          'listening': isListening,
          'speaking': isSpeaking,
          'disabled': !isSupported,
          'error': hasError
        }
      ]"
      :title="buttonTitle"
    >
      <!-- Microphone Icon -->
      <svg
        v-if="!isListening && !isSpeaking"
        class="w-6 h-6"
        fill="none"
        stroke="currentColor"
        viewBox="0 0 24 24"
      >
        <path
          stroke-linecap="round"
          stroke-linejoin="round"
          stroke-width="2"
          d="M19 11a7 7 0 01-7 7m0 0a7 7 0 01-7-7m7 7v4m0 0H8m4 0h4m-4-8a3 3 0 01-3-3V5a3 3 0 116 0v6a3 3 0 01-3 3z"
        />
      </svg>

      <!-- Listening Animation -->
      <div v-if="isListening" class="listening-animation">
        <span class="pulse-ring"></span>
        <span class="pulse-ring delay-1"></span>
        <span class="pulse-ring delay-2"></span>
        <svg class="w-6 h-6 text-red-500" fill="currentColor" viewBox="0 0 24 24">
          <circle cx="12" cy="12" r="4" />
        </svg>
      </div>

      <!-- Speaking Animation -->
      <div v-if="isSpeaking" class="speaking-animation">
        <span class="wave"></span>
        <span class="wave delay-1"></span>
        <span class="wave delay-2"></span>
      </div>
    </button>

    <!-- Transcript Display -->
    <div v-if="isListening || fullTranscript" class="transcript-display">
      <div class="transcript-content">
        <!-- Final Transcript -->
        <span v-if="transcript" class="final-transcript">
          {{ transcript }}
        </span>
        
        <!-- Interim Transcript -->
        <span v-if="interimTranscript" class="interim-transcript">
          {{ interimTranscript }}
        </span>

        <!-- Listening Indicator -->
        <span v-if="isListening && !fullTranscript" class="listening-indicator">
          {{ $t('ai.listening', 'Listening...') }}
        </span>
      </div>

      <!-- Confidence Badge -->
      <div v-if="confidence > 0" class="confidence-badge">
        {{ Math.round(confidence * 100) }}% confident
      </div>
    </div>

    <!-- Error Message -->
    <div v-if="hasError" class="error-message">
      <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
        <path
          fill-rule="evenodd"
          d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7 4a1 1 0 11-2 0 1 1 0 012 0zm-1-9a1 1 0 00-1 1v4a1 1 0 102 0V6a1 1 0 00-1-1z"
          clip-rule="evenodd"
        />
      </svg>
      <span>{{ errorMessage }}</span>
    </div>

    <!-- Language Selector -->
    <div v-if="showLanguageSelector" class="language-selector">
      <button
        v-for="lang in languages"
        :key="lang.code"
        @click="selectLanguage(lang.code)"
        :class="[
          'language-option',
          { 'active': currentLanguage === lang.code }
        ]"
      >
        {{ lang.flag }} {{ lang.name }}
      </button>
    </div>

    <!-- Instructions -->
    <div v-if="showInstructions" class="instructions">
      <p class="text-sm text-gray-600 dark:text-gray-400">
        {{ $t('ai.voiceInstructions', 'Click the microphone and speak in your preferred language. Try asking: "How were my sales today?" or "What stock is running low?"') }}
      </p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useVoiceCommands } from '~/composables/useVoiceCommands'
import { useI18n } from 'vue-i18n'

const props = defineProps<{
  showLanguageSelector?: boolean
  showInstructions?: boolean
  autoSpeak?: boolean
}>()

const emit = defineEmits<{
  transcript: [text: string]
  voiceCommand: [text: string]
  error: [message: string]
  languageChange: [lang: string]
}>()

const { t, locale } = useI18n()

// Initialize voice commands
const {
  isListening,
  isSupported,
  transcript,
  interimTranscript,
  fullTranscript,
  confidence,
  error: voiceError,
  isSpeaking,
  currentLanguage,
  startListening,
  stopListening,
  toggleListening,
  speak,
  stopSpeaking,
  setLanguage,
  clearTranscript
} = useVoiceCommands({
  continuous: false,
  interimResults: true,
  maxAlternatives: 1
})

// Computed
const hasError = computed(() => !!voiceError.value)
const errorMessage = computed(() => {
  const errorMap: Record<string, string> = {
    'no-speech': t('ai.voiceErrors.noSpeech', 'No speech detected. Please try again.'),
    'audio-capture': t('ai.voiceErrors.noMicrophone', 'No microphone found. Please check your device.'),
    'not-allowed': t('ai.voiceErrors.permissionDenied', 'Microphone permission denied. Please allow access.'),
    'network': t('ai.voiceErrors.network', 'Network error. Check your connection.'),
    'aborted': t('ai.voiceErrors.aborted', 'Speech recognition aborted.')
  }
  
  return errorMap[voiceError.value || ''] || voiceError.value || ''
})

const buttonTitle = computed(() => {
  if (!isSupported.value) return t('ai.voiceNotSupported', 'Voice not supported')
  if (isListening.value) return t('ai.stopListening', 'Stop listening')
  return t('ai.startListening', 'Start voice input')
})

// Language options
const languages = [
  { code: 'en', name: 'English', flag: '🇬🇧' },
  { code: 'zu', name: 'isiZulu', flag: '🇿🇦' },
  { code: 'xh', name: 'isiXhosa', flag: '🇿🇦' },
  { code: 'st', name: 'Sesotho', flag: '🇿🇦' },
  { code: 'tn', name: 'Setswana', flag: '🇿🇦' }
]

// Methods
const handleVoiceToggle = () => {
  if (isListening.value) {
    stopListening()
    
    // Emit transcript when stopped
    if (transcript.value) {
      emit('voiceCommand', transcript.value)
      emit('transcript', transcript.value)
    }
  } else {
    clearTranscript()
    startListening()
  }
}

const selectLanguage = (lang: string) => {
  setLanguage(lang)
  emit('languageChange', lang)
}

// Auto-speak response if enabled
const speakResponse = async (text: string) => {
  if (props.autoSpeak && isSupported.value) {
    try {
      await speak(text, languageCode.value)
    } catch (err) {
      console.error('Speech synthesis error:', err)
    }
  }
}

// Watch for transcript changes
watch(fullTranscript, (newTranscript) => {
  if (newTranscript) {
    emit('transcript', newTranscript)
  }
})

watch(voiceError, (newError) => {
  if (newError) {
    emit('error', newError)
  }
})

// Expose methods to parent
defineExpose({
  speak: speakResponse,
  stopSpeaking,
  clearTranscript
})
</script>

<style scoped>
.voice-input-container {
  @apply relative;
}

.voice-button {
  @apply relative w-14 h-14 rounded-full flex items-center justify-center transition-all duration-300;
  @apply bg-blue-500 hover:bg-blue-600 text-white shadow-lg;
  @apply focus:outline-none focus:ring-4 focus:ring-blue-300 dark:focus:ring-blue-800;
}

.voice-button.listening {
  @apply bg-red-500 hover:bg-red-600;
  animation: pulse-scale 1.5s ease-in-out infinite;
}

.voice-button.speaking {
  @apply bg-green-500;
}

.voice-button.disabled {
  @apply bg-gray-300 cursor-not-allowed opacity-50;
}

.voice-button.error {
  @apply bg-red-600 ring-2 ring-red-300;
}

/* Listening Animation */
.listening-animation {
  @apply relative flex items-center justify-center;
}

.pulse-ring {
  @apply absolute rounded-full border-2 border-red-500 opacity-0;
  width: 100%;
  height: 100%;
  animation: pulse-ring 1.5s ease-out infinite;
}

.pulse-ring.delay-1 {
  animation-delay: 0.5s;
}

.pulse-ring.delay-2 {
  animation-delay: 1s;
}

@keyframes pulse-ring {
  0% {
    transform: scale(0.8);
    opacity: 1;
  }
  100% {
    transform: scale(1.8);
    opacity: 0;
  }
}

@keyframes pulse-scale {
  0%, 100% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.1);
  }
}

/* Speaking Animation */
.speaking-animation {
  @apply flex items-center justify-center gap-1;
}

.wave {
  @apply w-1 h-6 bg-white rounded-full;
  animation: wave-animation 1.2s ease-in-out infinite;
}

.wave.delay-1 {
  animation-delay: 0.2s;
}

.wave.delay-2 {
  animation-delay: 0.4s;
}

@keyframes wave-animation {
  0%, 100% {
    transform: scaleY(0.5);
  }
  50% {
    transform: scaleY(1);
  }
}

/* Transcript Display */
.transcript-display {
  @apply mt-4 p-4 bg-white dark:bg-gray-800 rounded-lg shadow-md border border-gray-200 dark:border-gray-700;
}

.transcript-content {
  @apply text-gray-900 dark:text-white;
}

.final-transcript {
  @apply font-medium;
}

.interim-transcript {
  @apply text-gray-500 dark:text-gray-400 italic;
}

.listening-indicator {
  @apply text-blue-600 dark:text-blue-400 animate-pulse;
}

.confidence-badge {
  @apply mt-2 inline-block px-2 py-1 bg-green-100 dark:bg-green-900 text-green-800 dark:text-green-200 text-xs font-medium rounded;
}

/* Error Message */
.error-message {
  @apply mt-2 p-3 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg;
  @apply flex items-center gap-2 text-sm text-red-800 dark:text-red-200;
}

/* Language Selector */
.language-selector {
  @apply mt-4 flex flex-wrap gap-2;
}

.language-option {
  @apply px-3 py-2 bg-white dark:bg-gray-800 border border-gray-300 dark:border-gray-600;
  @apply rounded-lg text-sm font-medium transition-all;
  @apply hover:bg-gray-50 dark:hover:bg-gray-700;
}

.language-option.active {
  @apply bg-blue-500 text-white border-blue-500;
  @apply hover:bg-blue-600;
}

/* Instructions */
.instructions {
  @apply mt-4 p-3 bg-blue-50 dark:bg-blue-900/20 rounded-lg;
}
</style>

