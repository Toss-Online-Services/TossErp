<template>
  <div class="min-h-screen bg-background">
    <!-- Navigation - Material Design -->
    <nav class="fixed top-0 w-full z-50 bg-card/95 backdrop-blur-md shadow-sm border-b border-border">
      <div class="px-4 mx-auto max-w-7xl sm:px-6 lg:px-8">
        <div class="flex items-center justify-between h-16">
          <!-- Logo with animation -->
          <div class="flex items-center space-x-3 cursor-pointer group">
            <div class="relative flex items-center justify-center w-12 h-12 transition-all duration-300 shadow-lg rounded-lg bg-primary text-primary-foreground group-hover:scale-110 group-hover:shadow-xl">
              <span class="text-2xl font-black">T</span>
            </div>
            <div class="flex flex-col">
              <span class="text-2xl font-black text-foreground">TOSS</span>
              <span class="text-[10px] font-semibold text-muted-foreground -mt-1 tracking-wider">ONLINE SERVICES</span>
            </div>
          </div>

          <!-- Desktop Menu -->
          <div class="items-center hidden space-x-1 md:flex">
            <a href="#how-it-works" class="relative px-4 py-2 text-sm font-semibold transition-all duration-300 rounded-lg text-foreground hover:bg-accent group">
              <span class="relative z-10">How it works</span>
            </a>
            <a href="#for-shops" class="relative px-4 py-2 text-sm font-semibold transition-all duration-300 rounded-lg text-foreground hover:bg-accent group">
              <span class="relative z-10">For shops</span>
            </a>
            <a href="#for-suppliers" class="relative px-4 py-2 text-sm font-semibold transition-all duration-300 rounded-lg text-foreground hover:bg-accent group">
              <span class="relative z-10">For suppliers & drivers</span>
            </a>
            <a href="#faq" class="relative px-4 py-2 text-sm font-semibold transition-all duration-300 rounded-lg text-foreground hover:bg-accent group">
              <span class="relative z-10">FAQ</span>
            </a>
            <div class="w-px h-6 mx-2 bg-border"></div>
            <a href="/auth/register" class="relative px-4 py-2 text-sm font-bold text-primary hover:bg-accent rounded-lg transition-colors">
              <span class="relative z-10">Register</span>
            </a>
          </div>

          <!-- CTA Button -->
          <div class="flex items-center space-x-4">
            <a href="/auth/login" class="relative px-6 py-2.5 text-sm font-bold text-primary-foreground overflow-hidden transition-all duration-300 rounded-lg shadow-sm bg-primary hover:bg-primary/90 hover:shadow-md group">
              <span class="relative z-10 flex items-center gap-2">
                Sign in
                <svg class="w-4 h-4 transition-transform duration-300 group-hover:translate-x-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7l5 5m0 0l-5 5m5-5H6"></path>
                </svg>
              </span>
            </a>
          </div>
        </div>
      </div>
    </nav>

    <!-- Hero Section -->
    <section class="relative px-4 pt-32 pb-24 overflow-hidden sm:px-6 lg:px-8 bg-background">
      <div class="mx-auto max-w-7xl">
        <div class="text-center">
          <div class="inline-flex items-center gap-2 px-4 py-2 mb-8 text-sm font-semibold border rounded-full text-foreground bg-card border-border backdrop-blur-sm animate-fade-in-down">
            <span class="relative flex w-2 h-2">
              <span class="absolute inline-flex w-full h-full bg-primary rounded-full opacity-75 animate-ping"></span>
              <span class="relative inline-flex w-2 h-2 bg-primary rounded-full"></span>
            </span>
            Built for South African small businesses
          </div>

          <h1 class="mb-6 text-5xl font-extrabold tracking-tight text-foreground sm:text-6xl md:text-7xl animate-fade-in-up animation-delay-100">
            <span class="block">Run your hustle like</span>
            <span class="block mt-2 text-primary">a big enterprise</span>
          </h1>

          <p class="max-w-3xl mx-auto mb-4 text-xl font-medium leading-relaxed text-muted-foreground animate-fade-in-up animation-delay-200">
            One simple platform to track your sales and stock, get smart advice, and buy together with other shops to save money
          </p>
          <p class="max-w-2xl mx-auto mb-12 text-lg text-muted-foreground animate-fade-in-up animation-delay-300">
            Works offline. In your language. Made for township and rural businesses like yours.
          </p>

          <div class="flex flex-col items-center justify-center gap-4 mb-16 sm:flex-row animate-fade-in-up animation-delay-400">
            <a href="/auth/register" class="inline-flex items-center gap-2 px-8 py-4 text-lg font-semibold text-primary-foreground transition-all duration-300 bg-primary rounded-lg shadow-sm hover:bg-primary/90 hover:shadow-md group">
              <span>Join TOSS</span>
              <svg class="w-5 h-5 transition-transform group-hover:translate-x-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7l5 5m0 0l-5 5m5-5H6"></path>
              </svg>
            </a>
            <a href="#how-it-works" class="inline-flex items-center gap-2 px-8 py-4 text-lg font-semibold transition-all duration-300 border-2 rounded-lg text-foreground border-border hover:bg-accent group">
              <svg class="w-5 h-5 transition-transform group-hover:scale-110" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
              </svg>
              <span>See how it works</span>
            </a>
          </div>

          <div class="flex flex-wrap items-center justify-center gap-8 text-sm text-muted-foreground animate-fade-in-up animation-delay-500">
            <div class="flex items-center gap-2">
              <svg class="w-5 h-5 text-primary" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
              </svg>
              <span>Works offline</span>
            </div>
            <div class="flex items-center gap-2">
              <svg class="w-5 h-5 text-primary" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
              </svg>
              <span>Save money by buying together</span>
            </div>
            <div class="flex items-center gap-2">
              <svg class="w-5 h-5 text-primary" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
              </svg>
              <span>Stock delivered to your door</span>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Problem/Solution Section -->
    <section class="px-4 py-20 sm:px-6 lg:px-8 bg-muted/30">
      <div class="mx-auto max-w-7xl">
        <div class="mb-16 text-center">
          <h2 class="mb-4 text-3xl font-bold text-foreground sm:text-4xl md:text-5xl">The everyday struggle</h2>
          <p class="max-w-2xl mx-auto text-lg text-muted-foreground">
            Running a small shop is hard work. Here's what you deal with every day:
          </p>
        </div>

        <div class="grid gap-8 mb-16 md:grid-cols-2">
          <div class="toss-card p-8">
            <h3 class="mb-6 text-xl font-bold text-destructive">😓 The Problems</h3>
            <ul class="space-y-4">
              <li class="flex items-start gap-3">
                <span class="flex-shrink-0 w-6 h-6 mt-0.5 text-destructive">✗</span>
                <span class="text-foreground">You get to month-end with empty shelves and no cash</span>
              </li>
              <li class="flex items-start gap-3">
                <span class="flex-shrink-0 w-6 h-6 mt-0.5 text-destructive">✗</span>
                <span class="text-foreground">You pay retail prices at the wholesaler because you buy alone</span>
              </li>
              <li class="flex items-start gap-3">
                <span class="flex-shrink-0 w-6 h-6 mt-0.5 text-destructive">✗</span>
                <span class="text-foreground">You sell on credit but don't have proper records</span>
              </li>
              <li class="flex items-start gap-3">
                <span class="flex-shrink-0 w-6 h-6 mt-0.5 text-destructive">✗</span>
                <span class="text-foreground">You close the shop to go buy stock and lose sales time</span>
              </li>
            </ul>
          </div>

          <div class="toss-card p-8">
            <h3 class="mb-6 text-xl font-bold text-primary">✓ How TOSS Helps</h3>
            <ul class="space-y-4">
              <li class="flex items-start gap-3">
                <span class="flex-shrink-0 w-6 h-6 mt-0.5 text-primary">✓</span>
                <span class="text-foreground">See what's selling and what's running out – in one view</span>
              </li>
              <li class="flex items-start gap-3">
                <span class="flex-shrink-0 w-6 h-6 mt-0.5 text-primary">✓</span>
                <span class="text-foreground">Join group orders so you can buy like a big shop</span>
              </li>
              <li class="flex items-start gap-3">
                <span class="flex-shrink-0 w-6 h-6 mt-0.5 text-primary">✓</span>
                <span class="text-foreground">Keep simple records of every sale and payment</span>
              </li>
              <li class="flex items-start gap-3">
                <span class="flex-shrink-0 w-6 h-6 mt-0.5 text-primary">✓</span>
                <span class="text-foreground">Use local drivers to bring stock from the wholesaler to your door</span>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </section>

    <!-- For Shops Section -->
    <section id="for-shops" class="px-4 py-20 bg-background sm:px-6 lg:px-8">
      <div class="mx-auto max-w-7xl">
        <div class="mb-16 text-center">
          <div class="inline-block px-3 py-1 mb-4 text-xs font-semibold tracking-wider text-primary uppercase bg-primary/10 rounded-full">
            For shops
          </div>
          <h2 class="mb-4 text-3xl font-bold text-foreground sm:text-4xl md:text-5xl">
            For small businesses and local shops
          </h2>
          <p class="max-w-2xl mx-auto text-lg text-muted-foreground">
            Perfect for township and rural businesses like yours
          </p>
        </div>

        <div class="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-4">
          <div class="toss-card p-6 group hover:shadow-md transition-all duration-300">
            <div class="flex items-center justify-center w-12 h-12 mb-4 transition-all duration-300 bg-primary/10 rounded-lg group-hover:scale-110 group-hover:bg-primary/20">
              <svg class="w-6 h-6 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 18h.01M8 21h8a2 2 0 002-2V5a2 2 0 00-2-2H8a2 2 0 00-2 2v14a2 2 0 002 2z"></path>
              </svg>
            </div>
            <h3 class="mb-2 text-lg font-bold text-foreground">Simple POS on your phone</h3>
            <p class="text-sm text-muted-foreground">Ring up sales, track totals and download receipts – no fancy hardware needed</p>
          </div>

          <div class="toss-card p-6 group hover:shadow-md transition-all duration-300">
            <div class="flex items-center justify-center w-12 h-12 mb-4 transition-all duration-300 bg-primary/10 rounded-lg group-hover:scale-110 group-hover:bg-primary/20">
              <svg class="w-6 h-6 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4"></path>
              </svg>
            </div>
            <h3 class="mb-2 text-lg font-bold text-foreground">See your stock in one place</h3>
            <p class="text-sm text-muted-foreground">Know what's low, what's finished, and what to buy next</p>
          </div>

          <div class="toss-card p-6 group hover:shadow-md transition-all duration-300">
            <div class="flex items-center justify-center w-12 h-12 mb-4 transition-all duration-300 bg-primary/10 rounded-lg group-hover:scale-110 group-hover:bg-primary/20">
              <svg class="w-6 h-6 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"></path>
              </svg>
            </div>
            <h3 class="mb-2 text-lg font-bold text-foreground">Order from suppliers in a few taps</h3>
            <p class="text-sm text-muted-foreground">Send digital orders instead of long WhatsApp lists and phone calls</p>
          </div>

          <div class="toss-card p-6 group hover:shadow-md transition-all duration-300">
            <div class="flex items-center justify-center w-12 h-12 mb-4 transition-all duration-300 bg-primary/10 rounded-lg group-hover:scale-110 group-hover:bg-primary/20">
              <svg class="w-6 h-6 text-primary" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 9V7a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2m2 4h10a2 2 0 002-2v-6a2 2 0 00-2-2H9a2 2 0 00-2 2v6a2 2 0 002 2zm7-5a2 2 0 11-4 0 2 2 0 014 0z"></path>
              </svg>
            </div>
            <h3 class="mb-2 text-lg font-bold text-foreground">Save by buying together</h3>
            <p class="text-sm text-muted-foreground">Join group orders with other shops to unlock better prices</p>
          </div>
        </div>
      </div>
    </section>

    <!-- For Suppliers & Drivers Section -->
    <section id="for-suppliers" class="px-4 py-20 sm:px-6 lg:px-8 bg-slate-50 dark:bg-slate-900/40">
      <div class="mx-auto max-w-7xl">
        <div class="mb-16 text-center">
          <div class="inline-block px-3 py-1 mb-4 text-xs font-semibold tracking-wider text-primary uppercase bg-primary/10 rounded-full">
            For suppliers & drivers
          </div>
          <h2 class="mb-4 text-3xl font-bold text-foreground sm:text-4xl md:text-5xl">
            For suppliers and local drivers
          </h2>
        </div>

        <div class="grid gap-8 md:grid-cols-2">
          <div class="toss-card p-8">
            <h3 class="mb-6 text-2xl font-bold text-foreground">Suppliers</h3>
            <ul class="space-y-4">
              <li class="flex items-start gap-3">
                <span class="flex-shrink-0 w-6 h-6 mt-0.5 text-primary">✓</span>
                <span class="text-foreground">Receive clear digital orders from multiple township shops</span>
              </li>
              <li class="flex items-start gap-3">
                <span class="flex-shrink-0 w-6 h-6 mt-0.5 text-primary">✓</span>
                <span class="text-foreground">See demand by area and plan deliveries better</span>
              </li>
              <li class="flex items-start gap-3">
                <span class="flex-shrink-0 w-6 h-6 mt-0.5 text-primary">✓</span>
                <span class="text-foreground">Grow your reach without adding more salespeople</span>
              </li>
            </ul>
          </div>

          <div class="toss-card p-8">
            <h3 class="mb-6 text-2xl font-bold text-foreground">Drivers (bakkie owners, etc.)</h3>
            <ul class="space-y-4">
              <li class="flex items-start gap-3">
                <span class="flex-shrink-0 w-6 h-6 mt-0.5 text-primary">✓</span>
                <span class="text-foreground">Get paid delivery jobs from local shops and suppliers</span>
              </li>
              <li class="flex items-start gap-3">
                <span class="flex-shrink-0 w-6 h-6 mt-0.5 text-primary">✓</span>
                <span class="text-foreground">Do one trip with many drop-offs to save fuel</span>
              </li>
              <li class="flex items-start gap-3">
                <span class="flex-shrink-0 w-6 h-6 mt-0.5 text-primary">✓</span>
                <span class="text-foreground">Build a regular route with steady work</span>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </section>

    <!-- How TOSS Works Timeline -->
    <section id="how-it-works" class="px-4 py-20 bg-background sm:px-6 lg:px-8">
      <div class="max-w-4xl mx-auto">
        <div class="mb-16 text-center">
          <div class="inline-block px-3 py-1 mb-4 text-xs font-semibold tracking-wider text-primary uppercase bg-primary/10 rounded-full">
            How it works
          </div>
          <h2 class="mb-4 text-3xl font-bold text-foreground sm:text-4xl md:text-5xl">
            Simple steps to save money and grow
          </h2>
        </div>

        <ol class="relative pl-12 space-y-12 border-l-2 border-border">
          <li class="ml-16">
            <div class="absolute flex items-center justify-center w-12 h-12 text-lg font-bold text-primary-foreground transition-all duration-300 origin-center bg-primary rounded-full -left-6 hover:scale-125 hover:shadow-md">1</div>
            <div class="ml-4 transition-all duration-500 hover:translate-x-2">
              <h3 class="mb-3 text-lg font-bold text-foreground">Shop signs up and sets up products</h3>
              <p class="text-sm text-muted-foreground">Quick setup on your phone. Add your products with simple names and prices.</p>
            </div>
          </li>
          <li class="ml-16">
            <div class="absolute flex items-center justify-center w-12 h-12 text-lg font-bold text-primary-foreground transition-all duration-300 origin-center bg-primary rounded-full -left-6 hover:scale-125 hover:shadow-md">2</div>
            <div class="ml-4 transition-all duration-500 hover:translate-x-2">
              <h3 class="mb-3 text-lg font-bold text-foreground">Use TOSS POS to sell</h3>
              <p class="text-sm text-muted-foreground">Sales and stock update automatically. No more notebooks.</p>
            </div>
          </li>
          <li class="ml-16">
            <div class="absolute flex items-center justify-center w-12 h-12 text-lg font-bold text-primary-foreground transition-all duration-300 origin-center bg-primary rounded-full -left-6 hover:scale-125 hover:shadow-md">3</div>
            <div class="ml-4 transition-all duration-500 hover:translate-x-2">
              <h3 class="mb-3 text-lg font-bold text-foreground">TOSS suggests what to order</h3>
              <p class="text-sm text-muted-foreground">When stock is low, the platform tells you what to buy and helps you order.</p>
            </div>
          </li>
          <li class="ml-16">
            <div class="absolute flex items-center justify-center w-12 h-12 text-lg font-bold text-primary-foreground transition-all duration-300 origin-center bg-primary rounded-full -left-6 hover:scale-125 hover:shadow-md">4</div>
            <div class="ml-4 transition-all duration-500 hover:translate-x-2">
              <h3 class="mb-3 text-lg font-bold text-foreground">Shops join a group order</h3>
              <p class="text-sm text-muted-foreground">Buy together with other shops to get wholesale prices. TOSS sends it to suppliers.</p>
            </div>
          </li>
          <li class="ml-16">
            <div class="absolute flex items-center justify-center w-12 h-12 text-lg font-bold text-primary-foreground transition-all duration-300 origin-center bg-primary rounded-full -left-6 hover:scale-125 hover:shadow-md">5</div>
            <div class="ml-4 transition-all duration-500 hover:translate-x-2">
              <h3 class="mb-3 text-lg font-bold text-foreground">Driver delivers stock to your shop</h3>
              <p class="text-sm text-muted-foreground">Local driver picks up from wholesaler and delivers straight to your door. No more taxi trips.</p>
            </div>
          </li>
        </ol>
      </div>
    </section>

    <!-- Why TOSS Section -->
    <section class="px-4 py-20 sm:px-6 lg:px-8 bg-muted/30">
      <div class="mx-auto max-w-7xl">
        <div class="mb-12 text-center">
          <h2 class="mb-4 text-3xl font-bold text-foreground sm:text-4xl">Built for South African small businesses</h2>
          <p class="max-w-2xl mx-auto text-lg text-muted-foreground">
            We understand your challenges because we built this for you
          </p>
        </div>

        <div class="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
          <div class="flex items-start gap-4">
            <svg class="flex-shrink-0 w-6 h-6 mt-1 text-primary" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
            </svg>
            <div>
              <h4 class="mb-1 font-semibold text-foreground">Works on low data</h4>
              <p class="text-sm text-muted-foreground">Uses very little data. Works offline and syncs when you have connection.</p>
            </div>
          </div>

          <div class="flex items-start gap-4">
            <svg class="flex-shrink-0 w-6 h-6 mt-1 text-primary" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
            </svg>
            <div>
              <h4 class="mb-1 font-semibold text-foreground">Shows prices in Rand (R)</h4>
              <p class="text-sm text-muted-foreground">All pricing in South African Rand with proper cash rounding.</p>
            </div>
          </div>

          <div class="flex items-start gap-4">
            <svg class="flex-shrink-0 w-6 h-6 mt-1 text-primary" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
            </svg>
            <div>
              <h4 class="mb-1 font-semibold text-foreground">Simple language</h4>
              <p class="text-sm text-muted-foreground">No complicated business words. Plain English (and local languages coming soon).</p>
            </div>
          </div>

          <div class="flex items-start gap-4">
            <svg class="flex-shrink-0 w-6 h-6 mt-1 text-primary" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
            </svg>
            <div>
              <h4 class="mb-1 font-semibold text-foreground">For all small businesses</h4>
              <p class="text-sm text-muted-foreground">From retail shops to food vendors, salons to mechanics, and everything in between.</p>
            </div>
          </div>

          <div class="flex items-start gap-4">
            <svg class="flex-shrink-0 w-6 h-6 mt-1 text-primary" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
            </svg>
            <div>
              <h4 class="mb-1 font-semibold text-foreground">Easy to use</h4>
              <p class="text-sm text-muted-foreground">You don't need to be good with technology. If you can use WhatsApp, you can use TOSS.</p>
            </div>
          </div>

          <div class="flex items-start gap-4">
            <svg class="flex-shrink-0 w-6 h-6 mt-1 text-primary" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
            </svg>
            <div>
              <h4 class="mb-1 font-semibold text-foreground">Your data is safe</h4>
              <p class="text-sm text-muted-foreground">Your sales and customer info stays private and secure.</p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- Get Started Section -->
    <section id="get-started" class="relative px-4 py-20 overflow-hidden bg-background sm:px-6 lg:px-8">
      <div class="max-w-3xl mx-auto">
        <div class="mb-12 text-center">
          <h2 class="mb-4 text-3xl font-bold text-foreground sm:text-4xl md:text-5xl">
            Start using TOSS today
          </h2>
          <p class="text-lg text-muted-foreground">
            Join thousands of South African businesses already growing with TOSS
          </p>
        </div>

        <div class="flex flex-col items-center justify-center gap-6">
          <a href="/auth/register" class="inline-flex items-center gap-2 px-12 py-5 text-xl font-semibold text-primary-foreground transition-all duration-300 bg-primary rounded-lg shadow-sm hover:bg-primary/90 hover:shadow-md">
            <span>Create your account</span>
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7l5 5m0 0l-5 5m5-5H6"></path>
            </svg>
          </a>
          
          <p class="text-sm text-muted-foreground">
            Already have an account? 
            <a href="/auth/login" class="font-semibold text-primary hover:text-primary/80">
              Sign in here
            </a>
          </p>

          <div class="flex flex-wrap items-center justify-center gap-6 mt-6 text-sm text-muted-foreground">
            <div class="flex items-center gap-2">
              <svg class="w-5 h-5 text-primary" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
              </svg>
              <span>Free to get started</span>
            </div>
            <div class="flex items-center gap-2">
              <svg class="w-5 h-5 text-primary" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
              </svg>
              <span>No credit card required</span>
            </div>
            <div class="flex items-center gap-2">
              <svg class="w-5 h-5 text-primary" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
              </svg>
              <span>Ready in 5 minutes</span>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- FAQ Section -->
    <section id="faq" class="px-4 py-20 sm:px-6 lg:px-8 bg-muted/30">
      <div class="max-w-4xl mx-auto">
          <div class="mb-16 text-center">
          <div class="inline-block px-3 py-1 mb-4 text-xs font-semibold tracking-wider text-primary uppercase bg-primary/10 rounded-full">
            Questions
          </div>
          <h2 class="mb-4 text-3xl font-bold text-foreground sm:text-4xl md:text-5xl">
            Common questions
          </h2>
        </div>

        <div class="space-y-6">
          <div class="toss-card p-6 cursor-pointer hover:shadow-md transition-all duration-300">
            <h3 class="mb-2 text-lg font-bold text-foreground">How much does TOSS cost?</h3>
            <p class="text-muted-foreground">TOSS is free to get started with core features. We offer affordable premium plans with advanced features that save you more money than they cost.</p>
          </div>
          <div class="toss-card p-6 cursor-pointer hover:shadow-md transition-all duration-300">
            <h3 class="mb-2 text-lg font-bold text-foreground">Do I need WiFi or can I use normal data?</h3>
            <p class="text-muted-foreground">TOSS works with normal mobile data and uses very little. It also works completely offline – you can ring up sales even without any connection, and everything syncs later when you have data.</p>
          </div>

          <div class="toss-card p-6 cursor-pointer hover:shadow-md transition-all duration-300">
            <h3 class="mb-2 text-lg font-bold text-foreground">Can I use it on my phone?</h3>
            <p class="text-muted-foreground">Yes! TOSS is made for phones. It works on Android phones (even older ones). You don't need a computer or special equipment.</p>
          </div>

          <div class="toss-card p-6 cursor-pointer hover:shadow-md transition-all duration-300">
            <h3 class="mb-2 text-lg font-bold text-foreground">What if I'm not good with technology?</h3>
            <p class="text-muted-foreground">TOSS is designed to be as simple as WhatsApp. If you can send a message, you can use TOSS. We also provide training and support to help you get started.</p>
          </div>

          <div class="toss-card p-6 cursor-pointer hover:shadow-md transition-all duration-300">
            <h3 class="mb-2 text-lg font-bold text-foreground">Will my sales and customer data be safe?</h3>
            <p class="text-muted-foreground">Yes, very safe. Your data is encrypted and stored securely. Only you can see your business information. We don't share it with anyone without your permission.</p>
          </div>

          <div class="toss-card p-6 cursor-pointer hover:shadow-md transition-all duration-300">
            <h3 class="mb-2 text-lg font-bold text-foreground">How does the group buying work?</h3>
            <p class="text-muted-foreground">TOSS connects shops in the same area who want to buy the same products. When you join a group order, you combine your order with other shops to get better wholesale prices – like buying in bulk together.</p>
          </div>
        </div>
      </div>
    </section>

    <!-- Footer -->
    <footer class="px-4 py-12 bg-card border-t border-border sm:px-6 lg:px-8">
      <div class="mx-auto max-w-7xl">
        <div class="grid grid-cols-1 gap-8 md:grid-cols-4">
          <div>
            <div class="flex items-center mb-4 space-x-2">
              <div class="flex items-center justify-center w-10 h-10 rounded-lg shadow-sm bg-primary text-primary-foreground">
                <span class="text-xl font-bold">T</span>
              </div>
              <span class="text-xl font-bold text-foreground">TOSS</span>
            </div>
            <p class="text-sm leading-relaxed text-muted-foreground">
              Helping South African small businesses grow together
            </p>
          </div>
          
          <div>
            <h4 class="mb-4 text-sm font-semibold tracking-wider uppercase text-foreground">Quick Links</h4>
            <ul class="space-y-3 text-sm text-muted-foreground">
              <li><a href="#how-it-works" class="transition-colors hover:text-foreground">How it works</a></li>
              <li><a href="#for-shops" class="transition-colors hover:text-foreground">For shops</a></li>
              <li><a href="#for-suppliers" class="transition-colors hover:text-foreground">For suppliers & drivers</a></li>
              <li><a href="#faq" class="transition-colors hover:text-foreground">FAQ</a></li>
            </ul>
          </div>

          <div>
            <h4 class="mb-4 text-sm font-semibold tracking-wider uppercase text-foreground">Legal</h4>
            <ul class="space-y-3 text-sm text-muted-foreground">
              <li><a href="#" class="transition-colors hover:text-foreground">Privacy Policy</a></li>
              <li><a href="#" class="transition-colors hover:text-foreground">Terms of Service</a></li>
            </ul>
          </div>

          <div>
            <h4 class="mb-4 text-sm font-semibold tracking-wider uppercase text-foreground">Contact</h4>
            <ul class="space-y-3 text-sm text-muted-foreground">
              <li>Email: hello@toss.co.za</li>
              <li>WhatsApp: Coming soon</li>
            </ul>
          </div>
        </div>
        
        <div class="pt-8 mt-12 text-sm text-center border-t border-border text-muted-foreground">
          <p>&copy; {{ new Date().getFullYear() }} TOSS Online Services (Pty) Ltd – South Africa. All rights reserved.</p>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'landing'
})

useHead({
  title: 'TOSS - Run your hustle like a big enterprise',
  meta: [
    {
      name: 'description',
      content:
        'One simple platform to track sales and stock, get smart advice, and buy together with other shops. Made for South African small businesses.'
    }
  ]
})
</script>

<style scoped>
a {
  text-decoration: none;
}

/* Floating animations for background shapes */
@keyframes float {
  0%, 100% {
    transform: translateY(0px) rotate(0deg);
  }
  50% {
    transform: translateY(-20px) rotate(5deg);
  }
}

@keyframes float-delayed {
  0%, 100% {
    transform: translateY(0px) rotate(0deg);
  }
  50% {
    transform: translateY(-30px) rotate(-5deg);
  }
}

.animate-float {
  animation: float 8s ease-in-out infinite;
}

.animate-float-delayed {
  animation: float-delayed 10s ease-in-out infinite;
  animation-delay: 1s;
}

/* Fade in animations */
@keyframes fade-in-down {
  from {
    opacity: 0;
    transform: translateY(-20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes fade-in-up {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fade-in-down {
  animation: fade-in-down 0.6s ease-out;
}

.animate-fade-in-up {
  animation: fade-in-up 0.6s ease-out;
}

/* Animation delays */
.animation-delay-100 {
  animation-delay: 0.1s;
  opacity: 0;
  animation-fill-mode: forwards;
}

.animation-delay-200 {
  animation-delay: 0.2s;
  opacity: 0;
  animation-fill-mode: forwards;
}

.animation-delay-300 {
  animation-delay: 0.3s;
  opacity: 0;
  animation-fill-mode: forwards;
}

.animation-delay-400 {
  animation-delay: 0.4s;
  opacity: 0;
  animation-fill-mode: forwards;
}

.animation-delay-500 {
  animation-delay: 0.5s;
  opacity: 0;
  animation-fill-mode: forwards;
}

/* Gradient animation for hero text */
@keyframes gradient {
  0%, 100% {
    background-position: 0% 50%;
  }
  50% {
    background-position: 100% 50%;
  }
}

.animate-gradient {
  background: linear-gradient(90deg, hsl(var(--primary)), hsl(var(--primary) / 0.8), hsl(var(--primary)));
  background-size: 200% 200%;
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
  animation: gradient 3s ease infinite;
}

/* Smooth transitions */
* {
  transition-timing-function: cubic-bezier(0.4, 0, 0.2, 1);
}
</style>
