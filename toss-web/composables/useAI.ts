/**
 * Composable for AI functionality
 * Provides methods to interact with TOSS AI backend services
 */

import type { Ref } from 'vue'

export interface TossAIMessage {
  role: 'user' | 'assistant'
  content: string
  timestamp: Date
}

export interface AIResponseDto {
  question: string
  answer: string
  timestamp: string
  suggestions: string[]
}

export interface AISuggestionDto {
  type: string
  title: string
  description: string
  actionUrl: string
  priority: number
}

export interface MetaTagsResult {
  metaTitle: string
  metaKeywords: string
  metaDescription: string
}

export interface AISettingsDto {
  id: number
  enabled: boolean
  providerType: string
  apiKey?: string
  apiEndpoint?: string
  requestTimeoutSeconds: number
  supportedLanguages: string[]
  allowSalesForecasting: boolean
  allowInventoryPrediction: boolean
  allowBusinessInsights: boolean
  allowPriceSuggestions: boolean
  allowProductDescriptionGeneration: boolean
  allowMetaTitleGeneration: boolean
  allowMetaKeywordsGeneration: boolean
  allowMetaDescriptionGeneration: boolean
  productDescriptionQuery?: string
  metaTitleQuery?: string
  metaKeywordsQuery?: string
  metaDescriptionQuery?: string
}

export function useAI() {
  const config = useRuntimeConfig()
  const apiBaseUrl = config.public.apiBase
  
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  /**
   * Ask AI a question with optional business context
   */
  const askAI = async (
    question: string,
    shopId: number,
    context?: string
  ): Promise<AIResponseDto | null> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<AIResponseDto>(`${apiBaseUrl}/api/ai-copilot/ask`, {
        method: 'POST',
        body: {
          shopId,
          question,
          context
        }
      })

      return response
    } catch (err: any) {
      console.error('AI Ask error:', err)
      error.value = err.message || 'Failed to get AI response'
      return null
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Get AI-powered business suggestions
   */
  const getSuggestions = async (
    shopId: number,
    maxSuggestions: number = 5
  ): Promise<AISuggestionDto[]> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<AISuggestionDto[]>(
        `${apiBaseUrl}/api/ai-copilot/suggestions?shopId=${shopId}&maxSuggestions=${maxSuggestions}`
      )

      return response || []
    } catch (err: any) {
      console.error('AI Suggestions error:', err)
      error.value = err.message || 'Failed to get AI suggestions'
      return []
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Generate meta tags for an entity using AI
   */
  const generateMetaTags = async (
    entityType: 'Product' | 'ProductCategory' | 'Vendor',
    entityId: number,
    languageId: number = 0
  ): Promise<MetaTagsResult | null> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<MetaTagsResult>(`${apiBaseUrl}/api/ai-copilot/meta-tags`, {
        method: 'POST',
        body: {
          entityType,
          entityId,
          languageId
        }
      })

      return response
    } catch (err: any) {
      console.error('Generate Meta Tags error:', err)
      error.value = err.message || 'Failed to generate meta tags'
      return null
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Get AI settings for a shop
   */
  const getAISettings = async (shopId: number): Promise<AISettingsDto | null> => {
    isLoading.value = true
    error.value = null

    try {
      const response = await $fetch<AISettingsDto>(`${apiBaseUrl}/api/ai-copilot/settings/${shopId}`)
      return response
    } catch (err: any) {
      console.error('Get AI Settings error:', err)
      error.value = err.message || 'Failed to get AI settings'
      return null
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Update AI settings for a shop
   */
  const updateAISettings = async (settings: Partial<AISettingsDto> & { shopId: number }): Promise<boolean> => {
    isLoading.value = true
    error.value = null

    try {
      await $fetch(`${apiBaseUrl}/api/ai-copilot/settings`, {
        method: 'PUT',
        body: {
          shopId: settings.shopId,
          enabled: settings.enabled ?? true,
          providerType: settings.providerType ?? 'Gemini',
          apiKey: settings.apiKey,
          apiEndpoint: settings.apiEndpoint,
          requestTimeoutSeconds: settings.requestTimeoutSeconds ?? 30,
          allowSalesForecasting: settings.allowSalesForecasting ?? true,
          allowInventoryPrediction: settings.allowInventoryPrediction ?? true,
          allowBusinessInsights: settings.allowBusinessInsights ?? true,
          allowPriceSuggestions: settings.allowPriceSuggestions ?? true,
          allowProductDescriptionGeneration: settings.allowProductDescriptionGeneration ?? true,
          allowMetaTitleGeneration: settings.allowMetaTitleGeneration ?? true,
          allowMetaKeywordsGeneration: settings.allowMetaKeywordsGeneration ?? true,
          allowMetaDescriptionGeneration: settings.allowMetaDescriptionGeneration ?? true,
          productDescriptionQuery: settings.productDescriptionQuery,
          metaTitleQuery: settings.metaTitleQuery,
          metaKeywordsQuery: settings.metaKeywordsQuery,
          metaDescriptionQuery: settings.metaDescriptionQuery,
          supportedLanguages: settings.supportedLanguages ?? ['en', 'zu', 'xh', 'st', 'tn', 'af']
        }
      })

      return true
    } catch (err: any) {
      console.error('Update AI Settings error:', err)
      error.value = err.message || 'Failed to update AI settings'
      return false
    } finally {
      isLoading.value = false
    }
  }

  return {
    isLoading,
    error,
    askAI,
    getSuggestions,
    generateMetaTags,
    getAISettings,
    updateAISettings
  }
}

