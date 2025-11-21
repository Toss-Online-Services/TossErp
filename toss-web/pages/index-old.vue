<template>
  <div class="min-h-screen bg-white dark:bg-slate-950">
    <!-- Navigation -->
    <nav class="nav-animate fixed top-0 w-full z-50 border-b border-slate-200 dark:border-slate-800 bg-white/95 dark:bg-slate-950/95 backdrop-blur-md supports-[backdrop-filter]:bg-white/80 dark:supports-[backdrop-filter]:bg-slate-950/80 shadow-sm">
      <div class="px-4 mx-auto max-w-7xl sm:px-6 lg:px-8">
        <div class="flex items-center justify-between h-16">
          <!-- Logo -->
          <NuxtLink to="/" class="flex items-center space-x-2 no-underline group">
            <div class="flex items-center justify-center w-10 h-10 transition-all duration-300 rounded-lg shadow-lg bg-gradient-to-br from-slate-900 to-slate-700 dark:from-slate-700 dark:to-slate-900 group-hover:shadow-xl">
              <span class="text-xl font-bold text-white">T</span>
            </div>
            <span class="text-xl font-bold text-slate-900 dark:text-white">TOSS</span>
          </NuxtLink>

          <!-- Desktop Navigation -->
          <div class="items-center hidden space-x-8 md:flex">
            <NavigationMenu>
              <NavigationMenuList class="flex items-center space-x-1">
                <NavigationMenuItem>
                  <NavigationMenuLink as-child>
                    <a href="#how-it-works" class="relative px-3 py-2 text-sm font-medium no-underline transition-all duration-300 rounded-md nav-link group text-slate-700 dark:text-slate-300 hover:text-slate-900 dark:hover:text-white">
                      <span class="relative z-10">How it works</span>
                      <span class="absolute bottom-0 left-0 w-0 h-0.5 bg-orange-500 dark:bg-orange-400 transition-all duration-300 ease-out group-hover:w-full"></span>
                    </a>
                  </NavigationMenuLink>
                </NavigationMenuItem>
                <NavigationMenuItem>
                  <NavigationMenuLink as-child>
                    <a href="#for-shops" class="relative px-3 py-2 text-sm font-medium no-underline transition-all duration-300 rounded-md nav-link group text-slate-700 dark:text-slate-300 hover:text-slate-900 dark:hover:text-white">
                      <span class="relative z-10">For shops</span>
                      <span class="absolute bottom-0 left-0 w-0 h-0.5 bg-orange-500 dark:bg-orange-400 transition-all duration-300 ease-out group-hover:w-full"></span>
                    </a>
                  </NavigationMenuLink>
                </NavigationMenuItem>
                <NavigationMenuItem>
                  <NavigationMenuLink as-child>
                    <a href="#for-suppliers" class="relative px-3 py-2 text-sm font-medium no-underline transition-all duration-300 rounded-md nav-link group text-slate-700 dark:text-slate-300 hover:text-slate-900 dark:hover:text-white">
                      <span class="relative z-10">For suppliers & drivers</span>
                      <span class="absolute bottom-0 left-0 w-0 h-0.5 bg-orange-500 dark:bg-orange-400 transition-all duration-300 ease-out group-hover:w-full"></span>
                    </a>
                  </NavigationMenuLink>
                </NavigationMenuItem>
                <NavigationMenuItem>
                  <NavigationMenuLink as-child>
                    <a href="#faq" class="relative px-3 py-2 text-sm font-medium no-underline transition-all duration-300 rounded-md nav-link group text-slate-700 dark:text-slate-300 hover:text-slate-900 dark:hover:text-white">
                      <span class="relative z-10">FAQ</span>
                      <span class="absolute bottom-0 left-0 w-0 h-0.5 bg-orange-500 dark:bg-orange-400 transition-all duration-300 ease-out group-hover:w-full"></span>
                    </a>
                  </NavigationMenuLink>
                </NavigationMenuItem>
              </NavigationMenuList>
            </NavigationMenu>
          </div>

          <!-- CTA Buttons -->
          <div class="items-center hidden space-x-4 md:flex">
            <Button size="default" class="relative overflow-hidden text-sm font-semibold text-white transition-all duration-300 shadow-md btn-animate group bg-orange-500 hover:bg-orange-600 dark:bg-orange-600 dark:hover:bg-orange-700 hover:shadow-xl hover:scale-105 active:scale-95" as-child>
              <a href="#waitlist" class="relative z-10 flex items-center gap-2 no-underline">
                <span>Join waiting list</span>
                <svg class="w-4 h-4 transition-transform duration-300 ease-out group-hover:translate-x-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7l5 5m0 0l-5 5m5-5H6"></path>
                </svg>
              </a>
            </Button>
          </div>

          <!-- Mobile Menu Button -->
          <div class="flex items-center space-x-2 md:hidden">
            <Button size="sm" class="text-sm font-semibold text-white bg-orange-500 hover:bg-orange-600" as-child>
              <a href="#waitlist" class="no-underline">Join list</a>
            </Button>
          </div>
        </div>
      </div>
    </nav>

    <!-- Hero Section -->
    <section class="relative px-4 pt-32 pb-24 overflow-hidden sm:px-6 lg:px-8 bg-gradient-to-br from-slate-50 via-blue-50/50 to-purple-50/50 dark:from-slate-950 dark:via-slate-900 dark:to-slate-950">
      <!-- Animated Background Elements -->
      <div class="absolute inset-0 overflow-hidden -z-10" aria-hidden="true" role="presentation">
        <div class="absolute top-0 rounded-full left-1/4 w-96 h-96 bg-blue-400/10 blur-3xl animate-pulse" aria-hidden="true"></div>
        <div class="absolute bottom-0 delay-1000 rounded-full right-1/4 w-96 h-96 bg-purple-400/10 blur-3xl animate-pulse" aria-hidden="true"></div>
        <div class="absolute inset-0 bg-[radial-gradient(circle_at_30%_20%,rgba(59,130,246,0.08),transparent_50%)] dark:bg-[radial-gradient(circle_at_30%_20%,rgba(59,130,246,0.04),transparent_50%)]" aria-hidden="true"></div>
      </div>
      
      <div class="mx-auto max-w-7xl">
        <div class="text-center">
          <!-- Badge -->
          <div class="inline-flex items-center gap-2 px-4 py-2 mb-8 text-sm font-semibold text-blue-700 border border-blue-200 rounded-full shadow-sm dark:text-blue-300 bg-blue-100/80 dark:bg-blue-900/30 dark:border-blue-800 backdrop-blur-sm">
            <span class="relative flex w-2 h-2">
              <span class="absolute inline-flex w-full h-full bg-blue-400 rounded-full opacity-75 animate-ping"></span>
              <span class="relative inline-flex w-2 h-2 bg-blue-500 rounded-full"></span>
            </span>
            Smart Business App for Small Shops
          </div>

          <!-- Main Heading -->
          <h1 class="mb-6 text-5xl font-extrabold tracking-tight text-slate-900 dark:text-white sm:text-6xl md:text-7xl lg:text-8xl">
            <span class="block">ERP-III + AI Copilot</span>
            <span class="block mt-3 text-transparent bg-gradient-to-r from-blue-600 via-purple-600 to-pink-600 bg-clip-text">Collaborative Network for Small Shops</span>
          </h1>

          <!-- Subheading -->
          <p class="max-w-3xl mx-auto mb-4 text-xl font-medium leading-relaxed text-slate-600 dark:text-slate-300 sm:text-2xl">
            One phone app that <span class="font-semibold text-blue-600 dark:text-blue-400">keeps records</span>, <span class="font-semibold text-purple-600 dark:text-purple-400">does routine work for you</span>, and <span class="font-semibold text-pink-600 dark:text-pink-400">connects you to better prices & delivery</span>.
          </p>
          <p class="max-w-2xl mx-auto mb-12 text-lg text-slate-500 dark:text-slate-400">
            Built for spaza shops, chisa nyamas and township retailers: track sales, avoid stockouts, join group orders, access credit. Works offline. Plain language. Real help.
          </p>

          <!-- CTA Buttons -->
          <div class="flex flex-col items-center justify-center gap-4 mb-16 sm:flex-row">
            <Button size="lg" class="h-auto px-10 text-lg font-semibold transition-all duration-300 shadow-xl py-7 shadow-blue-500/25 hover:shadow-2xl hover:shadow-blue-500/40 hover:scale-105" as-child>
              <NuxtLink to="/auth/login" class="flex items-center gap-2 no-underline">
                <span>Login to TOSS Platform</span>
                <svg class="w-5 h-5 transition-transform duration-300 group-hover:translate-x-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7l5 5m0 0l-5 5m5-5H6"></path>
                </svg>
              </NuxtLink>
            </Button>
            <Button size="lg" variant="outline" class="h-auto px-10 text-lg font-semibold transition-all duration-300 border-2 py-7 border-slate-300 dark:border-slate-700 hover:border-slate-400 dark:hover:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-900" as-child>
              <NuxtLink to="#features" class="flex items-center gap-2 no-underline">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
                <span>Explore Features</span>
              </NuxtLink>
            </Button>
          </div>

          <!-- Trust Indicators -->
          <div class="flex flex-wrap items-center justify-center gap-8 mb-16 text-sm text-slate-500 dark:text-slate-400">
            <div class="flex items-center gap-2">
              <svg class="w-5 h-5 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
              </svg>
              <span>Complete Business Tools</span>
            </div>
            <div class="flex items-center gap-2">
              <svg class="w-5 h-5 text-blue-500" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
              </svg>
              <span>Smart Helper Included</span>
            </div>
            <div class="flex items-center gap-2">
              <svg class="w-5 h-5 text-purple-500" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
              </svg>
              <span>Connect with Suppliers</span>
            </div>
          </div>
          
          <!-- What is TOSS Section -->
          <div class="max-w-5xl mx-auto mb-16">
            <div class="relative overflow-hidden border-2 shadow-2xl rounded-3xl bg-gradient-to-br from-blue-50 via-purple-50 to-pink-50 dark:from-slate-900 dark:via-slate-800 dark:to-slate-900 border-blue-200/50 dark:border-slate-700">
              <!-- Decorative Background -->
              <div class="absolute inset-0 opacity-10">
                <div class="absolute top-0 right-0 w-64 h-64 bg-blue-400 rounded-full blur-3xl"></div>
                <div class="absolute bottom-0 left-0 w-64 h-64 bg-purple-400 rounded-full blur-3xl"></div>
              </div>
              
              <div class="relative p-8 md:p-12">
                <!-- Header -->
                <div class="mb-8 text-center">
                  <div class="inline-flex items-center gap-2 px-4 py-2 mb-4 text-sm font-semibold text-blue-700 bg-blue-100 rounded-full dark:text-blue-300 dark:bg-blue-900/50">
                    <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                      <path fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z" clip-rule="evenodd"></path>
                    </svg>
                    What is TOSS?
                  </div>
                  <h3 class="mb-4 text-3xl font-bold md:text-4xl text-slate-900 dark:text-white">
                    Your Smart Business Partner
                  </h3>
                  <p class="max-w-2xl mx-auto text-lg leading-relaxed text-slate-700 dark:text-slate-300">
                    TOSS is a simple app on your phone that helps small business owners run their shop better. Instead of writing everything in a notebook or guessing what to order, TOSS helps you manage everything in one place.
                  </p>
                </div>

                <!-- Three Pillars (Updated) -->
                <div class="grid gap-6 mb-8 md:grid-cols-3">
                  <!-- ERP-III Platform -->
                  <div class="p-6 border bg-white/80 dark:bg-slate-800/80 backdrop-blur-sm rounded-xl border-blue-200/50 dark:border-slate-700">
                    <div class="flex items-center gap-3 mb-3">
                      <div class="p-2 bg-blue-100 rounded-lg dark:bg-blue-900/50">
                        <svg class="w-6 h-6 text-blue-600 dark:text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
                        </svg>
                      </div>
                      <h4 class="text-lg font-bold text-slate-900 dark:text-white">ERP-III Platform</h4>
                    </div>
                    <p class="text-sm text-slate-600 dark:text-slate-400">Cloud + offline business backbone: sales, stock, money & suppliers in one place.</p>
                  </div>
                  <!-- AI Copilot -->
                  <div class="p-6 border bg-white/80 dark:bg-slate-800/80 backdrop-blur-sm rounded-xl border-purple-200/50 dark:border-slate-700">
                    <div class="flex items-center gap-3 mb-3">
                      <div class="p-2 bg-purple-100 rounded-lg dark:bg-purple-900/50">
                        <svg class="w-6 h-6 text-purple-600 dark:text-purple-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z" />
                        </svg>
                      </div>
                      <h4 class="text-lg font-bold text-slate-900 dark:text-white">Service-as-Software Copilot</h4>
                    </div>
                    <p class="text-sm text-slate-600 dark:text-slate-400">Not just advice—drafts orders, summaries & reminders automatically.</p>
                  </div>
                  <!-- Collaborative Network -->
                  <div class="p-6 border bg-white/80 dark:bg-slate-800/80 backdrop-blur-sm rounded-xl border-pink-200/50 dark:border-slate-700">
                    <div class="flex items-center gap-3 mb-3">
                      <div class="p-2 bg-pink-100 rounded-lg dark:bg-pink-900/50">
                        <svg class="w-6 h-6 text-pink-600 dark:text-pink-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
                        </svg>
                      </div>
                      <h4 class="text-lg font-bold text-slate-900 dark:text-white">Collaborative Network</h4>
                    </div>
                    <p class="text-sm text-slate-600 dark:text-slate-400">Group buying, delivery routing & trusted credit access in one ecosystem.</p>
                  </div>
                </div>

                <!-- Key Features Grid -->
                <div class="grid gap-4 mb-8 sm:grid-cols-2 lg:grid-cols-3">
                  <div class="flex items-start gap-3 p-4 rounded-lg bg-white/60 dark:bg-slate-800/60 backdrop-blur-sm">
                    <div class="p-1.5 bg-blue-100 dark:bg-blue-900/50 rounded-md flex-shrink-0">
                      <svg class="w-5 h-5 text-blue-600 dark:text-blue-400" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
                      </svg>
                    </div>
                    <div>
                      <div class="mb-1 font-semibold text-slate-900 dark:text-white">Track Sales & Stock</div>
                      <div class="text-sm text-slate-600 dark:text-slate-400">Know what you sold and what you have left</div>
                    </div>
                  </div>
                  
                  <div class="flex items-start gap-3 p-4 rounded-lg bg-white/60 dark:bg-slate-800/60 backdrop-blur-sm">
                    <div class="p-1.5 bg-green-100 dark:bg-green-900/50 rounded-md flex-shrink-0">
                      <svg class="w-5 h-5 text-green-600 dark:text-green-400" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
                      </svg>
                    </div>
                    <div>
                      <div class="mb-1 font-semibold text-slate-900 dark:text-white">Save Money</div>
                      <div class="text-sm text-slate-600 dark:text-slate-400">Buy together with other shops for better prices</div>
                    </div>
                  </div>
                  
                  <div class="flex items-start gap-3 p-4 rounded-lg bg-white/60 dark:bg-slate-800/60 backdrop-blur-sm">
                    <div class="p-1.5 bg-purple-100 dark:bg-purple-900/50 rounded-md flex-shrink-0">
                      <svg class="w-5 h-5 text-purple-600 dark:text-purple-400" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
                      </svg>
                    </div>
                    <div>
                      <div class="mb-1 font-semibold text-slate-900 dark:text-white">Get Stock Delivered</div>
                      <div class="text-sm text-slate-600 dark:text-slate-400">Order from suppliers, delivered to your door</div>
                    </div>
                  </div>
                  
                  <div class="flex items-start gap-3 p-4 rounded-lg bg-white/60 dark:bg-slate-800/60 backdrop-blur-sm">
                    <div class="p-1.5 bg-pink-100 dark:bg-pink-900/50 rounded-md flex-shrink-0">
                      <svg class="w-5 h-5 text-pink-600 dark:text-pink-400" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
                      </svg>
                    </div>
                    <div>
                      <div class="mb-1 font-semibold text-slate-900 dark:text-white">Get Smart Advice</div>
                      <div class="text-sm text-slate-600 dark:text-slate-400">Smart helper tells you what to order and when</div>
                    </div>
                  </div>
                  
                  <div class="flex items-start gap-3 p-4 rounded-lg bg-white/60 dark:bg-slate-800/60 backdrop-blur-sm">
                    <div class="p-1.5 bg-yellow-100 dark:bg-yellow-900/50 rounded-md flex-shrink-0">
                      <svg class="w-5 h-5 text-yellow-600 dark:text-yellow-400" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
                      </svg>
                    </div>
                    <div>
                      <div class="mb-1 font-semibold text-slate-900 dark:text-white">Access Credit</div>
                      <div class="text-sm text-slate-600 dark:text-slate-400">Build records to get loans and grow</div>
                    </div>
                  </div>
                  
                  <div class="flex items-start gap-3 p-4 rounded-lg bg-white/60 dark:bg-slate-800/60 backdrop-blur-sm">
                    <div class="p-1.5 bg-indigo-100 dark:bg-indigo-900/50 rounded-md flex-shrink-0">
                      <svg class="w-5 h-5 text-indigo-600 dark:text-indigo-400" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
                      </svg>
                    </div>
                    <div>
                      <div class="mb-1 font-semibold text-slate-900 dark:text-white">Works Offline</div>
                      <div class="text-sm text-slate-600 dark:text-slate-400">Use it even without internet connection</div>
                    </div>
                  </div>
                </div>

                <!-- Footer Note -->
                <div class="p-6 text-center border bg-white/40 dark:bg-slate-800/40 rounded-xl backdrop-blur-sm border-slate-200/50 dark:border-slate-700">
                  <p class="text-base font-medium text-slate-700 dark:text-slate-300">
                    Perfect for <span class="font-semibold text-blue-600 dark:text-blue-400">local shops</span>, <span class="font-semibold text-purple-600 dark:text-purple-400">small retailers</span>, and <span class="font-semibold text-pink-600 dark:text-pink-400">any small business</span>. Works on your phone, even when you don't have internet. Simple to use, even if you're not good with technology.
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Hero Stats -->
        <div class="grid max-w-5xl grid-cols-2 gap-6 mx-auto sm:grid-cols-4">
          <Card class="p-8 text-center transition-all duration-300 border-2 group border-slate-200 dark:border-slate-800 bg-white/90 dark:bg-slate-900/90 backdrop-blur-md hover:shadow-2xl hover:shadow-blue-500/20 hover:border-blue-300 dark:hover:border-blue-700 hover:-translate-y-2">
            <div class="flex justify-center mb-4">
              <div class="p-3 transition-transform duration-300 shadow-lg bg-gradient-to-br from-blue-500 to-blue-600 rounded-2xl shadow-blue-500/30 group-hover:scale-110">
                <UsersIcon class="w-6 h-6 text-white" />
              </div>
            </div>
            <div class="mb-2 text-4xl font-extrabold text-transparent bg-gradient-to-r from-blue-600 to-blue-700 bg-clip-text dark:from-blue-400 dark:to-blue-500">500+</div>
            <div class="text-sm font-semibold tracking-wide uppercase text-slate-600 dark:text-slate-400">Shops Using TOSS</div>
          </Card>
          <Card class="p-8 text-center transition-all duration-300 border-2 group border-slate-200 dark:border-slate-800 bg-white/90 dark:bg-slate-900/90 backdrop-blur-md hover:shadow-2xl hover:shadow-purple-500/20 hover:border-purple-300 dark:hover:border-purple-700 hover:-translate-y-2">
            <div class="flex justify-center mb-4">
              <div class="p-3 transition-transform duration-300 shadow-lg bg-gradient-to-br from-purple-500 to-purple-600 rounded-2xl shadow-purple-500/30 group-hover:scale-110">
                <CurrencyDollarIcon class="w-6 h-6 text-white" />
              </div>
            </div>
            <div class="mb-2 text-4xl font-extrabold text-transparent bg-gradient-to-r from-purple-600 to-purple-700 bg-clip-text dark:from-purple-400 dark:to-purple-500">10K+</div>
            <div class="text-sm font-semibold tracking-wide uppercase text-slate-600 dark:text-slate-400">Transactions</div>
          </Card>
          <Card class="p-8 text-center transition-all duration-300 border-2 group border-slate-200 dark:border-slate-800 bg-white/90 dark:bg-slate-900/90 backdrop-blur-md hover:shadow-2xl hover:shadow-pink-500/20 hover:border-pink-300 dark:hover:border-pink-700 hover:-translate-y-2">
            <div class="flex justify-center mb-4">
              <div class="p-3 transition-transform duration-300 shadow-lg bg-gradient-to-br from-pink-500 to-pink-600 rounded-2xl shadow-pink-500/30 group-hover:scale-110">
                <ArchiveBoxIcon class="w-6 h-6 text-white" />
              </div>
            </div>
            <div class="mb-2 text-4xl font-extrabold text-transparent bg-gradient-to-r from-pink-600 to-pink-700 bg-clip-text dark:from-pink-400 dark:to-pink-500">50+</div>
            <div class="text-sm font-semibold tracking-wide uppercase text-slate-600 dark:text-slate-400">Businesses</div>
          </Card>
          <Card class="p-8 text-center transition-all duration-300 border-2 group border-slate-200 dark:border-slate-800 bg-white/90 dark:bg-slate-900/90 backdrop-blur-md hover:shadow-2xl hover:shadow-green-500/20 hover:border-green-300 dark:hover:border-green-700 hover:-translate-y-2">
            <div class="flex justify-center mb-4">
              <div class="p-3 transition-transform duration-300 shadow-lg bg-gradient-to-br from-green-500 to-green-600 rounded-2xl shadow-green-500/30 group-hover:scale-110">
                <ArrowTrendingUpIcon class="w-6 h-6 text-white" />
              </div>
            </div>
            <div class="mb-2 text-4xl font-extrabold text-transparent bg-gradient-to-r from-green-600 to-green-700 bg-clip-text dark:from-green-400 dark:to-green-500">99.9%</div>
            <div class="text-sm font-semibold tracking-wide uppercase text-slate-600 dark:text-slate-400">Uptime</div>
          </Card>
        </div>
      </div>
    </section>
      <!-- Day-to-Day Flow & Automation Section -->
      <section id="flow" class="px-4 py-20 bg-slate-50 dark:bg-slate-900/40 sm:px-6 lg:px-8">
        <div class="mx-auto max-w-7xl">
          <div class="mb-12 text-center">
            <Badge class="px-3 py-1 mb-4 text-xs font-semibold tracking-wider uppercase" variant="outline">Daily & Weekly Flow</Badge>
            <h2 class="mb-4 text-3xl font-bold text-slate-900 dark:text-white sm:text-4xl md:text-5xl">How TOSS Helps Every Week</h2>
            <p class="max-w-2xl mx-auto text-lg text-slate-600 dark:text-slate-300">A simple rhythm: morning insight, mid-week restock, daily sales, end-of-day cash-up, month-end summary. TOSS automates and assists along the way.</p>
          </div>

          <!-- Timeline -->
          <div class="grid gap-10 lg:grid-cols-2">
            <div>
              <div class="mb-6 text-sm font-semibold tracking-wider text-blue-600 uppercase dark:text-blue-400">Weekly Flow Timeline</div>
              <ol class="relative pl-6 space-y-8 border-l border-slate-200 dark:border-slate-700" aria-label="Weekly operational flow timeline">
                <li class="ml-2">
                  <div class="absolute w-4 h-4 -left-2 top-1.5 rounded-full bg-blue-500 ring-4 ring-blue-100 dark:ring-blue-900/40" aria-hidden="true"></div>
                  <h3 class="mb-1 text-base font-bold text-slate-800 dark:text-slate-200">Morning Dashboard</h3>
                  <p class="text-sm text-slate-600 dark:text-slate-400">Yesterday’s sales, cash, low-stock alerts. Copilot suggests adjusting bread & cold drink quantities.</p>
                </li>
                <li class="ml-2">
                  <div class="absolute w-4 h-4 -left-2 top-1.5 rounded-full bg-purple-500 ring-4 ring-purple-100 dark:ring-purple-900/40" aria-hidden="true"></div>
                  <h3 class="mb-1 text-base font-bold text-slate-800 dark:text-slate-200">Mid-Week Restock Draft</h3>
                  <p class="text-sm text-slate-600 dark:text-slate-400">Auto-drafted group order based on low-stock + demand spikes. Owner reviews & confirms. Delivery scheduled.</p>
                </li>
                <li class="ml-2">
                  <div class="absolute w-4 h-4 -left-2 top-1.5 rounded-full bg-pink-500 ring-4 ring-pink-100 dark:ring-pink-900/40" aria-hidden="true"></div>
                  <h3 class="mb-1 text-base font-bold text-slate-800 dark:text-slate-200">Ad-Hoc Product Requests</h3>
                  <p class="text-sm text-slate-600 dark:text-slate-400">Customer asks for new item → marketplace check → small batch added to test demand. Credit sale logged.</p>
                </li>
                <li class="ml-2">
                  <div class="absolute w-4 h-4 -left-2 top-1.5 rounded-full bg-indigo-500 ring-4 ring-indigo-100 dark:ring-indigo-900/40" aria-hidden="true"></div>
                  <h3 class="mb-1 text-base font-bold text-slate-800 dark:text-slate-200">Evening Cash-Up</h3>
                  <p class="text-sm text-slate-600 dark:text-slate-400">One-tap close: sales summary, margin insight. Copilot prompts Sunday promotion idea.</p>
                </li>
                <li class="ml-2">
                  <div class="absolute w-4 h-4 -left-2 top-1.5 rounded-full bg-green-500 ring-4 ring-green-100 dark:ring-green-900/40" aria-hidden="true"></div>
                  <h3 class="mb-1 text-base font-bold text-slate-800 dark:text-slate-200">Month-End Pack</h3>
                  <p class="text-sm text-slate-600 dark:text-slate-400">Auto-generated simple P&L + loan pack draft. Optional share with coach or lender.</p>
                </li>
              </ol>
            </div>

            <!-- Automation Comparison Table -->
            <div>
              <div class="mb-6 text-sm font-semibold tracking-wider text-purple-600 uppercase dark:text-purple-400">Manual vs Assisted</div>
              <div class="overflow-x-auto border rounded-lg border-slate-200 dark:border-slate-700 bg-white/60 dark:bg-slate-800/60 backdrop-blur-sm" role="region" aria-label="Manual versus assisted tasks">
                <table class="w-full text-sm text-left">
                  <caption class="p-4 text-base font-semibold text-slate-700 dark:text-slate-300">Common tasks: before vs with TOSS Copilot</caption>
                  <thead class="text-xs font-semibold tracking-wide uppercase bg-slate-100 dark:bg-slate-800/80 text-slate-700 dark:text-slate-300">
                    <tr>
                      <th scope="col" class="px-4 py-3">Task</th>
                      <th scope="col" class="px-4 py-3">Before (Manual)</th>
                      <th scope="col" class="px-4 py-3">With TOSS (Assisted / Automated)</th>
                    </tr>
                  </thead>
                  <tbody class="divide-y divide-slate-200 dark:divide-slate-700">
                    <tr class="hover:bg-slate-50 dark:hover:bg-slate-800/40">
                      <th scope="row" class="px-4 py-3 font-medium text-slate-800 dark:text-slate-200">Restock Planning</th>
                      <td class="px-4 py-3">Guess & travel to wholesaler</td>
                      <td class="px-4 py-3">Auto draft order + group buying savings</td>
                    </tr>
                    <tr class="hover:bg-slate-50 dark:hover:bg-slate-800/40">
                      <th scope="row" class="px-4 py-3 font-medium text-slate-800 dark:text-slate-200">Low-Stock Alerts</th>
                      <td class="px-4 py-3">Notice only when empty</td>
                      <td class="px-4 py-3">Early warning & reorder suggestions</td>
                    </tr>
                    <tr class="hover:bg-slate-50 dark:hover:bg-slate-800/40">
                      <th scope="row" class="px-4 py-3 font-medium text-slate-800 dark:text-slate-200">Cash-Up Summary</th>
                      <td class="px-4 py-3">Manual tally in notebook</td>
                      <td class="px-4 py-3">One-tap daily summary & trends</td>
                    </tr>
                    <tr class="hover:bg-slate-50 dark:hover:bg-slate-800/40">
                      <th scope="row" class="px-4 py-3 font-medium text-slate-800 dark:text-slate-200">Credit Access</th>
                      <td class="px-4 py-3">No formal records</td>
                      <td class="px-4 py-3">Structured ledger builds loan footprint</td>
                    </tr>
                    <tr class="hover:bg-slate-50 dark:hover:bg-slate-800/40">
                      <th scope="row" class="px-4 py-3 font-medium text-slate-800 dark:text-slate-200">Loan Pack / Statements</th>
                      <td class="px-4 py-3">Not produced / time-consuming</td>
                      <td class="px-4 py-3">Auto-generated month-end pack</td>
                    </tr>
                    <tr class="hover:bg-slate-50 dark:hover:bg-slate-800/40">
                      <th scope="row" class="px-4 py-3 font-medium text-slate-800 dark:text-slate-200">Promotion Ideas</th>
                      <td class="px-4 py-3">Ad-hoc / reactive</td>
                      <td class="px-4 py-3">Data-driven nudges (e.g. weekend specials)</td>
                    </tr>
                  </tbody>
                </table>
              </div>
              <div class="mt-4 text-sm text-slate-600 dark:text-slate-400">Each assisted task frees time, reduces errors and builds a trustworthy digital trail.</div>
            </div>
          </div>
        </div>
      </section>

    <!-- Features Section -->
    <section id="features" class="px-4 py-20 bg-white sm:px-6 lg:px-8 dark:bg-slate-950">
      <div class="mx-auto max-w-7xl">
        <div class="mb-16 text-center">
          <Badge class="px-3 py-1 mb-4 text-xs font-semibold tracking-wider uppercase" variant="outline">Everything You Need</Badge>
          <h2 class="mb-4 text-3xl font-bold text-slate-900 dark:text-white sm:text-4xl md:text-5xl">All Your Business Tools in One App</h2>
          <p class="max-w-2xl mx-auto text-lg text-slate-600 dark:text-slate-300">
            TOSS gives you everything to run your shop: track sales, manage stock, handle money, connect with suppliers, and get helpful advice from our smart business helper
          </p>
        </div>

        <div class="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3">
          <Card class="p-8 transition-all bg-white border border-slate-200 dark:border-slate-800 dark:bg-slate-900 hover:shadow-xl hover:shadow-blue-500/10 hover:-translate-y-1 group">
            <div class="flex items-center justify-center mb-6 transition-transform shadow-lg w-14 h-14 bg-gradient-to-br from-blue-500 to-blue-600 rounded-xl shadow-blue-500/20 group-hover:scale-110 group-hover:rotate-3">
              <CurrencyDollarIcon class="text-white w-7 h-7" />
            </div>
            <h3 class="mb-3 text-xl font-bold text-slate-900 dark:text-white">Track Your Money</h3>
            <p class="leading-relaxed text-slate-600 dark:text-slate-300">
              See how much you make and spend every day. Know if your shop is making profit. Keep records for banks and suppliers.
            </p>
          </Card>

          <Card class="p-8 transition-all bg-white border border-slate-200 dark:border-slate-800 dark:bg-slate-900 hover:shadow-xl hover:shadow-green-500/10 hover:-translate-y-1 group">
            <div class="flex items-center justify-center mb-6 transition-transform shadow-lg w-14 h-14 bg-gradient-to-br from-green-500 to-green-600 rounded-xl shadow-green-500/20 group-hover:scale-110 group-hover:rotate-3">
              <UsersIcon class="text-white w-7 h-7" />
            </div>
            <h3 class="mb-3 text-xl font-bold text-slate-900 dark:text-white">Know Your Customers</h3>
            <p class="leading-relaxed text-slate-600 dark:text-slate-300">
              Remember who buys what. Track customers who buy on credit. Build relationships with regular customers.
            </p>
          </Card>

          <Card class="p-8 transition-all bg-white border border-slate-200 dark:border-slate-800 dark:bg-slate-900 hover:shadow-xl hover:shadow-purple-500/10 hover:-translate-y-1 group">
            <div class="flex items-center justify-center mb-6 transition-transform shadow-lg w-14 h-14 bg-gradient-to-br from-purple-500 to-purple-600 rounded-xl shadow-purple-500/20 group-hover:scale-110 group-hover:rotate-3">
              <ArchiveBoxIcon class="text-white w-7 h-7" />
            </div>
            <h3 class="mb-3 text-xl font-bold text-slate-900 dark:text-white">Manage Your Stock</h3>
            <p class="leading-relaxed text-slate-600 dark:text-slate-300">
              Know what you have in stock. Get alerts when items are running low. Never run out of popular items again.
            </p>
          </Card>

          <Card class="p-8 transition-all bg-white border border-slate-200 dark:border-slate-800 dark:bg-slate-900 hover:shadow-xl hover:shadow-yellow-500/10 hover:-translate-y-1 group">
            <div class="flex items-center justify-center mb-6 transition-transform shadow-lg w-14 h-14 bg-gradient-to-br from-yellow-500 to-yellow-600 rounded-xl shadow-yellow-500/20 group-hover:scale-110 group-hover:rotate-3">
              <ShoppingCartIcon class="text-white w-7 h-7" />
            </div>
            <h3 class="mb-3 text-xl font-bold text-slate-900 dark:text-white">Sell at Your Shop</h3>
            <p class="leading-relaxed text-slate-600 dark:text-slate-300">
              Use your phone as a cash register. Record every sale quickly. Accept cash and mobile payments.
            </p>
          </Card>

          <Card class="p-8 transition-all bg-white border border-slate-200 dark:border-slate-800 dark:bg-slate-900 hover:shadow-xl hover:shadow-pink-500/10 hover:-translate-y-1 group">
            <div class="flex items-center justify-center mb-6 transition-transform shadow-lg w-14 h-14 bg-gradient-to-br from-pink-500 to-pink-600 rounded-xl shadow-pink-500/20 group-hover:scale-110 group-hover:rotate-3">
              <ChartBarIcon class="text-white w-7 h-7" />
            </div>
            <h3 class="mb-3 text-xl font-bold text-slate-900 dark:text-white">Grow Your Sales</h3>
            <p class="leading-relaxed text-slate-600 dark:text-slate-300">
              See what sells best. Run promotions to attract customers. Learn which products make the most profit.
            </p>
          </Card>

          <Card class="p-8 transition-all bg-white border border-slate-200 dark:border-slate-800 dark:bg-slate-900 hover:shadow-xl hover:shadow-indigo-500/10 hover:-translate-y-1 group">
            <div class="flex items-center justify-center mb-6 transition-transform shadow-lg w-14 h-14 bg-gradient-to-br from-indigo-500 to-indigo-600 rounded-xl shadow-indigo-500/20 group-hover:scale-110 group-hover:rotate-3">
              <ArrowTrendingUpIcon class="text-white w-7 h-7" />
            </div>
            <h3 class="mb-3 text-xl font-bold text-slate-900 dark:text-white">Smart Business Helper</h3>
            <p class="leading-relaxed text-slate-600 dark:text-slate-300">
              Get smart advice for your shop. Automatically create shopping lists. Learn tips to make more money.
            </p>
          </Card>
        </div>
      </div>
    </section>

    <!-- Modules Section -->
    <section id="modules" class="px-4 py-20 sm:px-6 lg:px-8 bg-slate-50 dark:bg-slate-900/50">
      <div class="mx-auto max-w-7xl">
        <div class="mb-16 text-center">
          <Badge class="px-3 py-1 mb-4 text-xs font-semibold tracking-wider uppercase" variant="outline">Complete Business Tools</Badge>
          <h2 class="mb-4 text-3xl font-bold text-slate-900 dark:text-white sm:text-4xl md:text-5xl">Everything Your Business Needs</h2>
          <p class="max-w-2xl mx-auto mb-2 text-lg text-slate-600 dark:text-slate-300">
            TOSS has all the tools you need that work together to help you run every part of your business, from buying stock to managing money.
          </p>
          <p class="max-w-2xl mx-auto text-base font-medium text-blue-600 dark:text-blue-400">
            And TOSS doesn't just give you the tools - it can also do some things for you automatically!
          </p>
        </div>

        <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
          <Card class="p-5 transition-all bg-white border cursor-pointer border-slate-200 dark:border-slate-800 dark:bg-slate-900 hover:shadow-lg hover:border-blue-300 dark:hover:border-blue-700 hover:-translate-y-1 group">
            <div class="mb-2 text-sm font-bold transition-colors text-slate-900 dark:text-white group-hover:text-blue-600 dark:group-hover:text-blue-400">Connect with Suppliers</div>
            <div class="text-xs text-slate-600 dark:text-slate-400">Order stock directly from suppliers and manage deliveries</div>
          </Card>
          <Card class="p-5 transition-all bg-white border cursor-pointer border-slate-200 dark:border-slate-800 dark:bg-slate-900 hover:shadow-lg hover:border-green-300 dark:hover:border-green-700 hover:-translate-y-1 group">
            <div class="mb-2 text-sm font-bold transition-colors text-slate-900 dark:text-white group-hover:text-green-600 dark:group-hover:text-green-400">Manage Your Stock</div>
            <div class="text-xs text-slate-600 dark:text-slate-400">Track what you have, what you need, and get alerts when stock is low</div>
          </Card>
          <Card class="p-5 transition-all bg-white border cursor-pointer border-slate-200 dark:border-slate-800 dark:bg-slate-900 hover:shadow-lg hover:border-purple-300 dark:hover:border-purple-700 hover:-translate-y-1 group">
            <div class="mb-2 text-sm font-bold transition-colors text-slate-900 dark:text-white group-hover:text-purple-600 dark:group-hover:text-purple-400">Sell Online & In-Store</div>
            <div class="text-xs text-slate-600 dark:text-slate-400">Sell to customers both in your shop and through online channels</div>
          </Card>
          <Card class="p-5 transition-all bg-white border cursor-pointer border-slate-200 dark:border-slate-800 dark:bg-slate-900 hover:shadow-lg hover:border-pink-300 dark:hover:border-pink-700 hover:-translate-y-1 group">
            <div class="mb-2 text-sm font-bold transition-colors text-slate-900 dark:text-white group-hover:text-pink-600 dark:group-hover:text-pink-400">Know Your Customers</div>
            <div class="text-xs text-slate-600 dark:text-slate-400">Remember customer details, track what they buy, and manage credit sales</div>
          </Card>
          <Card class="p-5 transition-all bg-white border cursor-pointer border-slate-200 dark:border-slate-800 dark:bg-slate-900 hover:shadow-lg hover:border-yellow-300 dark:hover:border-yellow-700 hover:-translate-y-1 group">
            <div class="mb-2 text-sm font-bold transition-colors text-slate-900 dark:text-white group-hover:text-yellow-600 dark:group-hover:text-yellow-400">Check Your Business</div>
            <div class="text-xs text-slate-600 dark:text-slate-400">Audit your operations and make sure everything runs smoothly</div>
          </Card>
          <Card class="p-5 transition-all bg-white border cursor-pointer border-slate-200 dark:border-slate-800 dark:bg-slate-900 hover:shadow-lg hover:border-indigo-300 dark:hover:border-indigo-700 hover:-translate-y-1 group">
            <div class="mb-2 text-sm font-bold transition-colors text-slate-900 dark:text-white group-hover:text-indigo-600 dark:group-hover:text-indigo-400">Work with Suppliers</div>
            <div class="text-xs text-slate-600 dark:text-slate-400">Manage relationships with suppliers and get better deals</div>
          </Card>
          <Card class="p-5 transition-all bg-white border cursor-pointer border-slate-200 dark:border-slate-800 dark:bg-slate-900 hover:shadow-lg hover:border-orange-300 dark:hover:border-orange-700 hover:-translate-y-1 group">
            <div class="mb-2 text-sm font-bold transition-colors text-slate-900 dark:text-white group-hover:text-orange-600 dark:group-hover:text-orange-400">Send & Track Invoices</div>
            <div class="text-xs text-slate-600 dark:text-slate-400">Create invoices, send them to customers, and track payments</div>
          </Card>
          <Card class="p-5 transition-all bg-white border cursor-pointer border-slate-200 dark:border-slate-800 dark:bg-slate-900 hover:shadow-lg hover:border-teal-300 dark:hover:border-teal-700 hover:-translate-y-1 group">
            <div class="mb-2 text-sm font-bold transition-colors text-slate-900 dark:text-white group-hover:text-teal-600 dark:group-hover:text-teal-400">See Your Business Reports</div>
            <div class="text-xs text-slate-600 dark:text-slate-400">Get insights and reports to understand how your business is doing</div>
          </Card>
          <Card class="p-5 transition-all bg-white border cursor-pointer border-slate-200 dark:border-slate-800 dark:bg-slate-900 hover:shadow-lg hover:border-red-300 dark:hover:border-red-700 hover:-translate-y-1 group">
            <div class="mb-2 text-sm font-bold transition-colors text-slate-900 dark:text-white group-hover:text-red-600 dark:group-hover:text-red-400">Manage Your Money</div>
            <div class="text-xs text-slate-600 dark:text-slate-400">Track income, expenses, and see if you're making profit</div>
          </Card>
          <Card class="p-5 transition-all bg-white border cursor-pointer border-slate-200 dark:border-slate-800 dark:bg-slate-900 hover:shadow-lg hover:border-cyan-300 dark:hover:border-cyan-700 hover:-translate-y-1 group">
            <div class="mb-2 text-sm font-bold transition-colors text-slate-900 dark:text-white group-hover:text-cyan-600 dark:group-hover:text-cyan-400">Plan Your Products</div>
            <div class="text-xs text-slate-600 dark:text-slate-400">Decide what products to sell and how to display them</div>
          </Card>
          <Card class="p-5 transition-all bg-white border cursor-pointer border-slate-200 dark:border-slate-800 dark:bg-slate-900 hover:shadow-lg hover:border-violet-300 dark:hover:border-violet-700 hover:-translate-y-1 group">
            <div class="mb-2 text-sm font-bold transition-colors text-slate-900 dark:text-white group-hover:text-violet-600 dark:group-hover:text-violet-400">Plan What to Stock</div>
            <div class="text-xs text-slate-600 dark:text-slate-400">Plan your product mix and decide what to order</div>
          </Card>
          <Card class="p-5 transition-all bg-white border cursor-pointer border-slate-200 dark:border-slate-800 dark:bg-slate-900 hover:shadow-lg hover:border-emerald-300 dark:hover:border-emerald-700 hover:-translate-y-1 group">
            <div class="mb-2 text-sm font-bold transition-colors text-slate-900 dark:text-white group-hover:text-emerald-600 dark:group-hover:text-emerald-400">Sell at Your Shop</div>
            <div class="text-xs text-slate-600 dark:text-slate-400">Use your phone as a cash register and record sales</div>
          </Card>
          <Card class="p-5 transition-all bg-white border cursor-pointer border-slate-200 dark:border-slate-800 dark:bg-slate-900 hover:shadow-lg hover:border-rose-300 dark:hover:border-rose-700 hover:-translate-y-1 group">
            <div class="mb-2 text-sm font-bold transition-colors text-slate-900 dark:text-white group-hover:text-rose-600 dark:group-hover:text-rose-400">Grow Your Sales</div>
            <div class="text-xs text-slate-600 dark:text-slate-400">Run promotions, track what sells, and attract more customers</div>
          </Card>
          <Card class="p-5 transition-all bg-white border cursor-pointer border-slate-200 dark:border-slate-800 dark:bg-slate-900 hover:shadow-lg hover:border-sky-300 dark:hover:border-sky-700 hover:-translate-y-1 group">
            <div class="mb-2 text-sm font-bold transition-colors text-slate-900 dark:text-white group-hover:text-sky-600 dark:group-hover:text-sky-400">Manage Your Warehouse</div>
            <div class="text-xs text-slate-600 dark:text-slate-400">Organize storage, track stock locations, and manage deliveries</div>
          </Card>
        </div>

        <div class="mt-12 text-center">
          <p class="mb-6 text-base font-medium text-slate-700 dark:text-slate-300">
            Plus special features: <span class="font-semibold text-blue-600 dark:text-blue-400">Buy together with other shops to save money</span>, <span class="font-semibold text-purple-600 dark:text-purple-400">Get stock delivered to your door</span>, <span class="font-semibold text-green-600 dark:text-green-400">Access loans and credit</span>
          </p>
          <Button size="lg" class="shadow-lg shadow-blue-500/20 hover:shadow-xl hover:shadow-blue-500/30" as-child>
            <NuxtLink to="/auth/login" class="no-underline">Start Using TOSS</NuxtLink>
          </Button>
        </div>
      </div>
    </section>

    <!-- Stats Section -->
    <section id="stats" class="px-4 py-20 bg-white sm:px-6 lg:px-8 dark:bg-slate-950">
      <div class="mx-auto max-w-7xl">
        <div class="mb-16 text-center">
          <Badge class="px-3 py-1 mb-4 text-xs font-semibold tracking-wider uppercase" variant="outline">Results</Badge>
          <h2 class="mb-4 text-3xl font-bold text-slate-900 dark:text-white sm:text-4xl md:text-5xl">Shops Using TOSS Are Growing</h2>
          <p class="max-w-2xl mx-auto text-lg text-slate-600 dark:text-slate-300">
            See how TOSS is helping small shops make more money and run better
          </p>
        </div>

        <div class="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-4">
          <Card class="p-8 text-center transition-all border border-slate-200 dark:border-slate-800 bg-gradient-to-br from-white to-green-50/50 dark:from-slate-900 dark:to-green-950/20 hover:shadow-xl">
            <div class="flex items-center justify-center mb-4">
              <div class="p-4 shadow-lg bg-gradient-to-br from-green-500 to-green-600 rounded-2xl shadow-green-500/20">
                <CurrencyDollarIcon class="w-8 h-8 text-white" />
              </div>
            </div>
            <div class="mb-2 text-4xl font-bold text-slate-900 dark:text-white">R 2.5M+</div>
            <div class="mb-1 text-sm font-semibold text-slate-600 dark:text-slate-400">Sales Tracked</div>
            <div class="text-xs font-medium text-green-600 dark:text-green-400">45% more this month</div>
          </Card>

          <Card class="p-8 text-center transition-all border border-slate-200 dark:border-slate-800 bg-gradient-to-br from-white to-blue-50/50 dark:from-slate-900 dark:to-blue-950/20 hover:shadow-xl">
            <div class="flex items-center justify-center mb-4">
              <div class="p-4 shadow-lg bg-gradient-to-br from-blue-500 to-blue-600 rounded-2xl shadow-blue-500/20">
                <ArchiveBoxIcon class="w-8 h-8 text-white" />
              </div>
            </div>
            <div class="mb-2 text-4xl font-bold text-slate-900 dark:text-white">15K+</div>
            <div class="mb-1 text-sm font-semibold text-slate-600 dark:text-slate-400">Products Tracked</div>
            <div class="text-xs font-medium text-blue-600 dark:text-blue-400">Always up to date</div>
          </Card>

          <Card class="p-8 text-center transition-all border border-slate-200 dark:border-slate-800 bg-gradient-to-br from-white to-purple-50/50 dark:from-slate-900 dark:to-purple-950/20 hover:shadow-xl">
            <div class="flex items-center justify-center mb-4">
              <div class="p-4 shadow-lg bg-gradient-to-br from-purple-500 to-purple-600 rounded-2xl shadow-purple-500/20">
                <ShoppingCartIcon class="w-8 h-8 text-white" />
              </div>
            </div>
            <div class="mb-2 text-4xl font-bold text-slate-900 dark:text-white">8K+</div>
            <div class="mb-1 text-sm font-semibold text-slate-600 dark:text-slate-400">Orders Completed</div>
            <div class="text-xs font-medium text-purple-600 dark:text-purple-400">99.2% success rate</div>
          </Card>

          <Card class="p-8 text-center transition-all border border-slate-200 dark:border-slate-800 bg-gradient-to-br from-white to-yellow-50/50 dark:from-slate-900 dark:to-yellow-950/20 hover:shadow-xl">
            <div class="flex items-center justify-center mb-4">
              <div class="p-4 shadow-lg bg-gradient-to-br from-yellow-500 to-yellow-600 rounded-2xl shadow-yellow-500/20">
                <UsersIcon class="w-8 h-8 text-white" />
              </div>
            </div>
            <div class="mb-2 text-4xl font-bold text-slate-900 dark:text-white">500+</div>
            <div class="mb-1 text-sm font-semibold text-slate-600 dark:text-slate-400">Shops Using TOSS</div>
            <div class="text-xs font-medium text-yellow-600 dark:text-yellow-400">More joining every day</div>
          </Card>
        </div>
      </div>
    </section>

    <!-- CTA Section -->
    <section id="cta" class="relative px-4 py-20 overflow-hidden sm:px-6 lg:px-8 bg-gradient-to-br from-blue-600 via-purple-600 to-pink-600">
      <div class="absolute inset-0 bg-[radial-gradient(circle_at_50%_50%,rgba(255,255,255,0.1),transparent_70%)] -z-10"></div>
      <div class="absolute inset-0 opacity-20 -z-10" style="background-image: url('data:image/svg+xml,%3Csvg width=&quot;60&quot; height=&quot;60&quot; viewBox=&quot;0 0 60 60&quot; xmlns=&quot;http://www.w3.org/2000/svg&quot;%3E%3Cg fill=&quot;none&quot; fill-rule=&quot;evenodd&quot;%3E%3Cg fill=&quot;%23ffffff&quot; fill-opacity=&quot;0.05&quot;%3E%3Cpath d=&quot;M36 34v-4h-2v4h-4v2h4v4h2v-4h4v-2h-4zm0-30V0h-2v4h-4v2h4v4h2V6h4V4h-4zM6 34v-4H4v4H0v2h4v4h2v-4h4v-2H6zM6 4V0H4v4H0v2h4v4h2V6h4V4H6z&quot;/%3E%3C/g%3E%3C/g%3E%3C/svg%3E');"></div>
      
      <div class="relative max-w-4xl mx-auto text-center">
        <h2 class="mb-6 text-3xl font-bold text-white sm:text-4xl md:text-5xl">
          Ready to Make Your Shop Better?
        </h2>
        <p class="max-w-2xl mx-auto mb-10 text-lg text-white/90 sm:text-xl">
          Join other shop owners who are using TOSS to save time, save money, and grow their business. Start today - it's free to try!
        </p>
        <div class="flex flex-col justify-center gap-4 sm:flex-row">
          <Button size="lg" variant="secondary" class="h-auto px-8 py-6 text-base font-semibold transition-all shadow-xl hover:shadow-2xl" as-child>
              <NuxtLink to="/auth/login" class="no-underline">Login to TOSS Platform</NuxtLink>
            </Button>
            <Button size="lg" variant="outline" class="h-auto px-8 py-6 text-base font-semibold text-white border-2 bg-white/10 border-white/30 hover:bg-white/20 backdrop-blur-sm" as-child>
              <NuxtLink to="#modules" class="no-underline">View All Modules</NuxtLink>
          </Button>
        </div>
      </div>
    </section>

    <!-- Footer -->
    <footer class="px-4 py-12 bg-white border-t border-slate-200 dark:border-slate-800 dark:bg-slate-950 sm:px-6 lg:px-8">
      <div class="mx-auto max-w-7xl">
        <div class="grid grid-cols-1 gap-8 md:grid-cols-4">
          <div>
            <div class="flex items-center mb-4 space-x-2">
              <div class="flex items-center justify-center w-10 h-10 rounded-lg shadow-lg bg-gradient-to-br from-blue-600 to-purple-600">
                <span class="text-xl font-bold text-white">T</span>
              </div>
              <span class="text-xl font-bold text-slate-900 dark:text-white">TOSS</span>
            </div>
            <p class="text-sm leading-relaxed text-slate-600 dark:text-slate-400">
              Township Operations Support System - Empowering businesses to thrive with collaborative ERP solutions.
            </p>
          </div>
          <div>
            <h4 class="mb-4 text-sm font-semibold tracking-wider uppercase text-slate-900 dark:text-white">Platform</h4>
            <ul class="space-y-3 text-sm text-slate-600 dark:text-slate-400">
              <li><NuxtLink to="/auth/login" class="no-underline transition-colors hover:text-blue-600 dark:hover:text-blue-400">Login</NuxtLink></li>
              <li><NuxtLink to="/auth/register" class="no-underline transition-colors hover:text-blue-600 dark:hover:text-blue-400">Register</NuxtLink></li>
              <li><NuxtLink to="#modules" class="no-underline transition-colors hover:text-blue-600 dark:hover:text-blue-400">Modules</NuxtLink></li>
            </ul>
          </div>
            <div>
              <h4 class="mb-4 text-sm font-semibold tracking-wider uppercase text-slate-900 dark:text-white">Modules</h4>
              <ul class="space-y-3 text-sm text-slate-600 dark:text-slate-400">
                <li><a href="#modules" class="no-underline transition-colors hover:text-blue-600 dark:hover:text-blue-400">All Modules</a></li>
                <li><a href="#features" class="no-underline transition-colors hover:text-blue-600 dark:hover:text-blue-400">Key Features</a></li>
                <li><a href="#cta" class="no-underline transition-colors hover:text-blue-600 dark:hover:text-blue-400">Get Started</a></li>
              </ul>
            </div>
          <div>
            <h4 class="mb-4 text-sm font-semibold tracking-wider uppercase text-slate-900 dark:text-white">Support</h4>
            <ul class="space-y-3 text-sm text-slate-600 dark:text-slate-400">
              <li><a href="#" class="no-underline transition-colors hover:text-blue-600 dark:hover:text-blue-400">Documentation</a></li>
              <li><a href="#" class="no-underline transition-colors hover:text-blue-600 dark:hover:text-blue-400">Help Center</a></li>
              <li><a href="#" class="no-underline transition-colors hover:text-blue-600 dark:hover:text-blue-400">Contact</a></li>
            </ul>
          </div>
        </div>
        <div class="pt-8 mt-12 text-sm text-center border-t border-slate-200 dark:border-slate-800 text-slate-600 dark:text-slate-400">
          <p>&copy; {{ new Date().getFullYear() }} TOSS. All rights reserved.</p>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { 
  CurrencyDollarIcon, 
  UsersIcon, 
  ArrowTrendingUpIcon,
  ArchiveBoxIcon,
  ShoppingCartIcon,
  ChartBarIcon
} from '@heroicons/vue/24/outline'
import { Card } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import {
  NavigationMenu,
  NavigationMenuItem,
  NavigationMenuLink,
  NavigationMenuList,
} from '@/components/ui/navigation-menu'

definePageMeta({
  layout: 'landing'
})

useHead({
  title: 'TOSS - Township Operations Support System',
  meta: [
    { name: 'description', content: 'Powerful ERP solution designed for township businesses. Manage inventory, sales, purchasing, and group buying all in one place.' }
  ]
})
</script>

<style scoped>
/* Smooth navigation link animations - no default underlines */
.nav-link,
a,
NuxtLink {
  text-decoration: none !important;
}

.nav-link {
  position: relative;
}

.nav-link::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  width: 0;
  height: 2px;
  background: currentColor;
  transition: width 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.nav-link:hover::after {
  width: 100%;
}

/* Sign In button underline animation */
.sign-in-button {
  position: relative;
  overflow: visible;
}

.sign-in-button::after {
  content: '';
  position: absolute;
  bottom: 6px;
  left: 50%;
  transform: translateX(-50%);
  width: 0;
  height: 2px;
  background: currentColor;
  transition: width 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  z-index: 1;
}

.sign-in-button:hover::after {
  width: 75%;
}

/* Smooth fade-in animation for navigation */
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

/* Smooth scale animation for buttons */
@keyframes scaleIn {
  from {
    opacity: 0;
    transform: scale(0.95);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}

.btn-animate {
  animation: scaleIn 0.3s ease-out;
}
</style>
