<template>
  <nav class="nav-animate fixed top-0 w-full z-50 border-b border-slate-200 dark:border-slate-800 bg-white/95 dark:bg-slate-950/95 backdrop-blur-xl supports-[backdrop-filter]:bg-white/80 dark:supports-[backdrop-filter]:bg-slate-950/80 shadow-lg">
    <div class="px-4 mx-auto max-w-7xl sm:px-6 lg:px-8">
      <div class="flex items-center justify-between h-20">
        <!-- Logo -->
        <NuxtLink to="/" class="flex items-center space-x-3 no-underline group">
          <div class="relative flex items-center justify-center w-12 h-12 transition-all duration-500 rounded-xl shadow-lg bg-gradient-to-br from-orange-500 to-orange-600 group-hover:scale-110 group-hover:rotate-3 group-hover:shadow-2xl">
            <div class="absolute inset-0 rounded-xl opacity-0 bg-gradient-to-br from-orange-400 to-orange-500 group-hover:opacity-100 transition-opacity duration-500"></div>
            <svg class="relative z-10 w-7 h-7 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M13 10V3L4 14h7v7l9-11h-7z"></path>
            </svg>
          </div>
          <div class="flex flex-col">
            <span class="text-2xl font-black leading-none overflow-visible pb-1">
              <span class="bg-gradient-to-r from-slate-900 to-slate-700 dark:from-white dark:to-slate-300 bg-clip-text text-transparent">TOSS</span>
            </span>
            <span class="text-[10px] font-semibold tracking-wider text-orange-600 dark:text-orange-400 uppercase leading-none">ONLINE SERVICES</span>
          </div>
        </NuxtLink>

        <!-- Desktop Navigation -->
        <div class="items-center hidden space-x-1 md:flex">
          <a href="#how-it-works" class="nav-link relative px-4 py-2 text-sm font-medium transition-all duration-300 rounded-lg text-slate-700 dark:text-slate-200 hover:bg-slate-100 dark:hover:bg-slate-800">
            How It Works
          </a>
          <a href="#pricing" class="nav-link relative px-4 py-2 text-sm font-medium transition-all duration-300 rounded-lg text-slate-700 dark:text-slate-200 hover:bg-slate-100 dark:hover:bg-slate-800">
            Pricing
          </a>
          <a href="#get-started" class="nav-link relative px-4 py-2 text-sm font-medium transition-all duration-300 rounded-lg text-slate-700 dark:text-slate-200 hover:bg-slate-100 dark:hover:bg-slate-800">
            Get Started
          </a>
          
          <div class="w-px h-8 mx-3 bg-slate-300 dark:bg-slate-700"></div>
          
          <a href="/auth/register" class="relative px-5 py-2.5 text-sm font-semibold transition-all duration-300 rounded-lg text-slate-700 dark:text-slate-200 hover:bg-slate-100 dark:hover:bg-slate-800">
            Register
          </a>
          <a href="/auth/login" class="relative px-6 py-2.5 text-sm font-bold text-white overflow-hidden transition-all duration-300 rounded-lg shadow-lg bg-gradient-to-r from-orange-500 to-orange-600 hover:from-orange-600 hover:to-orange-700 dark:from-orange-600 dark:to-orange-700 dark:hover:from-orange-700 dark:hover:to-orange-800 hover:shadow-xl hover:scale-105 group">
            <span class="relative z-10 flex items-center gap-2">
              <span>Sign In</span>
              <svg class="w-4 h-4 transition-transform duration-300 group-hover:translate-x-1 group-hover:scale-110" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path>
              </svg>
            </span>
            <div class="absolute inset-0 transition-opacity duration-300 opacity-0 bg-gradient-to-r from-orange-400 to-orange-500 group-hover:opacity-100"></div>
          </a>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup lang="ts">
</script>

<style scoped>
.nav-link::after {
  content: '';
  position: absolute;
  bottom: 6px;
  left: 50%;
  transform: translateX(-50%);
  width: 0;
  height: 2px;
  background: currentColor;
  transition: width 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

.nav-link:hover::after {
  width: 60%;
}

@keyframes fadeInDown {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.nav-animate {
  animation: fadeInDown 0.5s ease-out;
}
</style>
