/**
 * Voice Commands Composable for TOSS AI Assistant
 * Supports multi-language voice input/output (English, Zulu, Xhosa, Sotho, Tswana)
 */

import { ref, computed, onMounted, onUnmounted } from 'vue'

export interface VoiceCommandOptions {
  language?: string
  continuous?: boolean
  interimResults?: boolean
  maxAlternatives?: number
}

export const useVoiceCommands = (options: VoiceCommandOptions = {}) => {
  // State
  const isListening = ref(false)
  const isSupported = ref(false)
  const transcript = ref('')
  const interimTranscript = ref('')
  const confidence = ref(0)
  const error = ref<string | null>(null)
  const isSpeaking = ref(false)
  
  // Recognition instance
  let recognition: any = null
  let synthesis: any = null

  // Language mapping for South African languages
  const languageMap: Record<string, string> = {
    'en': 'en-ZA', // English (South Africa)
    'zu': 'zu-ZA', // Zulu
    'xh': 'xh-ZA', // Xhosa
    'st': 'st-ZA', // Southern Sotho
    'tn': 'tn-ZA', // Tswana
    'af': 'af-ZA'  // Afrikaans
  }

  const currentLanguage = ref(options.language || 'en')

  // Computed
  const fullTranscript = computed(() => {
    if (interimTranscript.value) {
      return transcript.value + ' ' + interimTranscript.value
    }
    return transcript.value
  })

  const languageCode = computed(() => {
    return languageMap[currentLanguage.value] || 'en-ZA'
  })

  // Initialize Speech Recognition
  const initRecognition = () => {
    if (typeof window === 'undefined') return

    const SpeechRecognition = (window as any).SpeechRecognition || (window as any).webkitSpeechRecognition
    
    if (!SpeechRecognition) {
      isSupported.value = false
      error.value = 'Speech recognition not supported in this browser'
      return
    }

    isSupported.value = true
    recognition = new SpeechRecognition()

    // Configure recognition
    recognition.continuous = options.continuous ?? false
    recognition.interimResults = options.interimResults ?? true
    recognition.maxAlternatives = options.maxAlternatives ?? 1
    recognition.lang = languageCode.value

    // Event handlers
    recognition.onstart = () => {
      isListening.value = true
      error.value = null
      transcript.value = ''
      interimTranscript.value = ''
    }

    recognition.onresult = (event: any) => {
      let interim = ''
      let final = ''

      for (let i = event.resultIndex; i < event.results.length; i++) {
        const result = event.results[i]
        const transcriptText = result[0].transcript

        if (result.isFinal) {
          final += transcriptText + ' '
          confidence.value = result[0].confidence
        } else {
          interim += transcriptText
        }
      }

      if (final) {
        transcript.value = (transcript.value + final).trim()
      }
      interimTranscript.value = interim
    }

    recognition.onerror = (event: any) => {
      console.error('Speech recognition error:', event.error)
      error.value = event.error
      isListening.value = false
    }

    recognition.onend = () => {
      isListening.value = false
    }
  }

  // Initialize Text-to-Speech
  const initSynthesis = () => {
    if (typeof window === 'undefined') return
    synthesis = window.speechSynthesis
  }

  // Start listening
  const startListening = () => {
    if (!recognition) {
      error.value = 'Speech recognition not initialized'
      return
    }

    try {
      recognition.lang = languageCode.value
      recognition.start()
    } catch (err: any) {
      if (err.message.includes('already started')) {
        // Recognition already running, stop and restart
        recognition.stop()
        setTimeout(() => {
          recognition.start()
        }, 100)
      } else {
        error.value = err.message
      }
    }
  }

  // Stop listening
  const stopListening = () => {
    if (recognition && isListening.value) {
      recognition.stop()
    }
  }

  // Toggle listening
  const toggleListening = () => {
    if (isListening.value) {
      stopListening()
    } else {
      startListening()
    }
  }

  // Speak text (Text-to-Speech)
  const speak = (text: string, lang?: string): Promise<void> => {
    return new Promise((resolve, reject) => {
      if (!synthesis) {
        reject(new Error('Speech synthesis not supported'))
        return
      }

      // Cancel any ongoing speech
      synthesis.cancel()

      const utterance = new SpeechSynthesisUtterance(text)
      utterance.lang = lang || languageCode.value
      utterance.rate = 0.9 // Slightly slower for clarity
      utterance.pitch = 1
      utterance.volume = 1

      utterance.onstart = () => {
        isSpeaking.value = true
      }

      utterance.onend = () => {
        isSpeaking.value = false
        resolve()
      }

      utterance.onerror = (event) => {
        isSpeaking.value = false
        reject(new Error(event.error))
      }

      synthesis.speak(utterance)
    })
  }

  // Stop speaking
  const stopSpeaking = () => {
    if (synthesis) {
      synthesis.cancel()
      isSpeaking.value = false
    }
  }

  // Change language
  const setLanguage = (lang: string) => {
    currentLanguage.value = lang
    if (recognition) {
      recognition.lang = languageCode.value
    }
  }

  // Clear transcript
  const clearTranscript = () => {
    transcript.value = ''
    interimTranscript.value = ''
    confidence.value = 0
  }

  // Get available voices for current language
  const getAvailableVoices = () => {
    if (!synthesis) return []
    return synthesis.getVoices().filter((voice: any) => 
      voice.lang.startsWith(languageCode.value.split('-')[0])
    )
  }

  // Lifecycle
  onMounted(() => {
    initRecognition()
    initSynthesis()
  })

  onUnmounted(() => {
    if (recognition) {
      recognition.stop()
      recognition = null
    }
    if (synthesis) {
      synthesis.cancel()
    }
  })

  return {
    // State
    isListening,
    isSupported,
    transcript,
    interimTranscript,
    fullTranscript,
    confidence,
    error,
    isSpeaking,
    currentLanguage,
    
    // Actions
    startListening,
    stopListening,
    toggleListening,
    speak,
    stopSpeaking,
    setLanguage,
    clearTranscript,
    getAvailableVoices
  }
}

