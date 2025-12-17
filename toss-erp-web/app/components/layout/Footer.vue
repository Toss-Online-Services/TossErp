<script setup lang="ts">
import { Heart, Github, Twitter, Globe } from 'lucide-vue-next'
</script>

<template>
  <footer class="bg-white dark:bg-stone-900 border-t border-stone-200 dark:border-stone-700 py-4 px-4 md:px-8 mt-auto">
    <div class="flex flex-col md:flex-row items-center justify-between gap-4 text-sm">
      <!-- Copyright -->
      <div class="flex items-center gap-1 text-stone-500 dark:text-stone-400">
        <span>© {{ new Date().getFullYear() }} TOSS ERP, made with</span>
        <Heart :size="14" class="text-red-500 fill-red-500" />
        <span>for South African SMMEs</span>
      </div>

      <!-- Links -->
      <div class="flex items-center gap-6">
        <a 
          href="https://github.com/Toss-Online-Services" 
          target="_blank" 
          rel="noopener noreferrer"
          class="text-stone-500 dark:text-stone-400 hover:text-stone-900 dark:hover:text-white transition-colors flex items-center gap-1.5"
        >
          <Github :size="16" />
          <span class="hidden sm:inline">GitHub</span>
        </a>
        <a 
          href="#"
          class="text-stone-500 dark:text-stone-400 hover:text-stone-900 dark:hover:text-white transition-colors flex items-center gap-1.5"
        >
          <Twitter :size="16" />
          <span class="hidden sm:inline">Twitter</span>
        </a>
        <a 
          href="https://toss.co.za" 
          target="_blank" 
          rel="noopener noreferrer"
          class="text-stone-500 dark:text-stone-400 hover:text-stone-900 dark:hover:text-white transition-colors flex items-center gap-1.5"
        >
          <Globe :size="16" />
          <span class="hidden sm:inline">Website</span>
        </a>
      </div>
    </div>
  </footer>
</template>
