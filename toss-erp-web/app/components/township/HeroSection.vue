<template>
  <div class="bg-gradient-to-r from-blue-600 to-purple-600 rounded-2xl p-6 sm:p-8 mb-4 sm:mb-6 shadow-lg">
    <div class="text-center space-y-4">
      <!-- Plain Language Tagline -->
      <h1 class="text-2xl sm:text-3xl md:text-4xl font-bold text-white leading-tight">
        {{ $t('hero.tagline') }}
      </h1>
      <p class="text-base sm:text-lg text-white/90 max-w-2xl mx-auto">
        {{ $t('hero.subtitle') }}
      </p>
      
      <!-- Primary Action Buttons - Large and Colorful -->
      <div class="flex flex-col sm:flex-row gap-3 sm:gap-4 justify-center items-stretch sm:items-center pt-4">
        <NuxtLink 
          to="/stock/order" 
          class="flex items-center justify-center gap-2 px-8 py-4 bg-white text-blue-600 rounded-xl font-semibold text-lg shadow-lg hover:shadow-xl transform hover:scale-105 transition-all touch-manipulation min-h-[56px]"
        >
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
          </svg>
          <span>{{ $t('hero.orderNow') }}</span>
        </NuxtLink>
        
        <NuxtLink 
          to="/stock/track" 
          class="flex items-center justify-center gap-2 px-8 py-4 bg-yellow-400 text-gray-900 rounded-xl font-semibold text-lg shadow-lg hover:shadow-xl transform hover:scale-105 transition-all touch-manipulation min-h-[56px]"
        >
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
          </svg>
          <span>{{ $t('hero.trackOrder') }}</span>
        </NuxtLink>
      </div>
      
      <!-- Community Badge -->
      <div class="flex items-center justify-center gap-2 text-white/80 text-sm">
        <svg class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
          <path d="M13 6a3 3 0 11-6 0 3 3 0 016 0zM18 8a2 2 0 11-4 0 2 2 0 014 0zM14 15a4 4 0 00-8 0v3h8v-3zM6 8a2 2 0 11-4 0 2 2 0 014 0zM16 18v-3a5.972 5.972 0 00-.75-2.906A3.005 3.005 0 0119 15v3h-3zM4.75 12.094A5.973 5.973 0 004 15v3H1v-3a3 3 0 013.75-2.906z" />
        </svg>
        <span>{{ $t('hero.communitySupported') }}</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'

const { t } = useI18n()
</script>

