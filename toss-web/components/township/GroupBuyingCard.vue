<template>
  <div class="bg-gradient-to-br from-purple-50 to-blue-50 dark:from-purple-900/20 dark:to-blue-900/20 rounded-2xl p-6 border-2 border-purple-200 dark:border-purple-700 shadow-md">
    <div class="flex items-start gap-4">
      <!-- Community Icon -->
      <div class="flex-shrink-0 p-3 bg-purple-500 rounded-xl">
        <svg class="w-8 h-8 text-white" fill="currentColor" viewBox="0 0 20 20">
          <path d="M13 6a3 3 0 11-6 0 3 3 0 016 0zM18 8a2 2 0 11-4 0 2 2 0 014 0zM14 15a4 4 0 00-8 0v3h8v-3zM6 8a2 2 0 11-4 0 2 2 0 014 0zM16 18v-3a5.972 5.972 0 00-.75-2.906A3.005 3.005 0 0119 15v3h-3zM4.75 12.094A5.973 5.973 0 004 15v3H1v-3a3 3 0 013.75-2.906z" />
        </svg>
      </div>
      
      <div class="flex-1 space-y-3">
        <!-- Badge -->
        <div class="inline-flex items-center gap-2 px-3 py-1 bg-purple-600 text-white text-sm font-semibold rounded-full">
          <span>✨</span>
          <span>{{ $t('groupBuying.badge') }}</span>
        </div>
        
        <!-- Title and Description -->
        <div>
          <h3 class="text-xl sm:text-2xl font-bold text-gray-900 dark:text-white mb-2">
            {{ $t('groupBuying.title') }}
          </h3>
          <p class="text-base text-gray-700 dark:text-gray-300 leading-relaxed">
            {{ $t('groupBuying.description') }}
          </p>
        </div>
        
        <!-- Benefits List -->
        <ul class="space-y-2">
          <li class="flex items-center gap-2 text-base text-gray-700 dark:text-gray-300">
            <svg class="w-5 h-5 text-green-600 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
            </svg>
            <span>{{ $t('groupBuying.benefit1') }}</span>
          </li>
          <li class="flex items-center gap-2 text-base text-gray-700 dark:text-gray-300">
            <svg class="w-5 h-5 text-green-600 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
            </svg>
            <span>{{ $t('groupBuying.benefit2') }}</span>
          </li>
          <li class="flex items-center gap-2 text-base text-gray-700 dark:text-gray-300">
            <svg class="w-5 h-5 text-green-600 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
            </svg>
            <span>{{ $t('groupBuying.benefit3') }}</span>
          </li>
        </ul>
        
        <!-- CTA Button -->
        <button
          @click="joinGroupBuying"
          class="w-full sm:w-auto px-6 py-3 bg-purple-600 hover:bg-purple-700 text-white font-semibold text-lg rounded-xl shadow-lg hover:shadow-xl transform hover:scale-105 transition-all touch-manipulation min-h-[48px]"
        >
          {{ $t('groupBuying.joinNow') }}
        </button>
        
        <!-- Social Proof -->
        <div class="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400">
          <div class="flex -space-x-2">
            <div class="w-8 h-8 rounded-full bg-blue-500 border-2 border-white flex items-center justify-center text-white text-xs font-bold">
              M
            </div>
            <div class="w-8 h-8 rounded-full bg-green-500 border-2 border-white flex items-center justify-center text-white text-xs font-bold">
              T
            </div>
            <div class="w-8 h-8 rounded-full bg-purple-500 border-2 border-white flex items-center justify-center text-white text-xs font-bold">
              S
            </div>
            <div class="w-8 h-8 rounded-full bg-gray-400 border-2 border-white flex items-center justify-center text-white text-xs font-bold">
              +50
            </div>
          </div>
          <span class="font-medium">{{ $t('groupBuying.memberCount') }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'

const { t } = useI18n()
const router = useRouter()

const joinGroupBuying = () => {
  router.push('/group-buying')
}
</script>

