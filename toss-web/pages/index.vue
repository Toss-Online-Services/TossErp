<template>
  <div class="min-h-screen bg-background">
    <!-- Navigation -->
    <nav class="nav-animate fixed top-0 w-full z-50 border-b border-slate-200 dark:border-slate-800 bg-white/95 dark:bg-slate-950/95 backdrop-blur-md supports-[backdrop-filter]:bg-white/80 dark:supports-[backdrop-filter]:bg-slate-950/80 shadow-sm">
      <div class="px-4 mx-auto max-w-7xl sm:px-6 lg:px-8">
        <div class="flex justify-between items-center h-16">
          <!-- Logo -->
          <NuxtLink to="/" class="flex items-center space-x-2 group no-underline">
            <div class="flex justify-center items-center w-10 h-10 bg-gradient-to-br from-blue-600 to-purple-600 rounded-lg shadow-lg group-hover:shadow-xl transition-all duration-300">
              <span class="text-xl font-bold text-white">T</span>
            </div>
            <span class="text-xl font-bold text-slate-900 dark:text-white">TOSS</span>
          </NuxtLink>

          <!-- Desktop Navigation -->
          <div class="hidden items-center space-x-8 md:flex">
            <NavigationMenu>
              <NavigationMenuList class="flex items-center space-x-1">
                <NavigationMenuItem>
                  <NavigationMenuLink as-child>
                    <NuxtLink to="#features" class="nav-link group relative px-3 py-2 text-sm font-medium text-slate-700 dark:text-slate-300 hover:text-slate-900 dark:hover:text-white rounded-md transition-all duration-300 no-underline">
                      <span class="relative z-10">Features</span>
                      <span class="absolute bottom-0 left-0 w-0 h-0.5 bg-blue-600 dark:bg-blue-400 transition-all duration-300 ease-out group-hover:w-full"></span>
                    </NuxtLink>
                  </NavigationMenuLink>
                </NavigationMenuItem>
                <NavigationMenuItem>
                  <NavigationMenuLink as-child>
                    <NuxtLink to="#modules" class="nav-link group relative px-3 py-2 text-sm font-medium text-slate-700 dark:text-slate-300 hover:text-slate-900 dark:hover:text-white rounded-md transition-all duration-300 no-underline">
                      <span class="relative z-10">Modules</span>
                      <span class="absolute bottom-0 left-0 w-0 h-0.5 bg-purple-600 dark:bg-purple-400 transition-all duration-300 ease-out group-hover:w-full"></span>
                    </NuxtLink>
                  </NavigationMenuLink>
                </NavigationMenuItem>
                <NavigationMenuItem>
                  <NavigationMenuLink as-child>
                    <NuxtLink to="#stats" class="nav-link group relative px-3 py-2 text-sm font-medium text-slate-700 dark:text-slate-300 hover:text-slate-900 dark:hover:text-white rounded-md transition-all duration-300 no-underline">
                      <span class="relative z-10">Stats</span>
                      <span class="absolute bottom-0 left-0 w-0 h-0.5 bg-pink-600 dark:bg-pink-400 transition-all duration-300 ease-out group-hover:w-full"></span>
                    </NuxtLink>
                  </NavigationMenuLink>
                </NavigationMenuItem>
                <NavigationMenuItem>
                  <NavigationMenuLink as-child>
                    <NuxtLink to="#cta" class="nav-link group relative px-3 py-2 text-sm font-medium text-slate-700 dark:text-slate-300 hover:text-slate-900 dark:hover:text-white rounded-md transition-all duration-300 no-underline">
                      <span class="relative z-10">Pricing</span>
                      <span class="absolute bottom-0 left-0 w-0 h-0.5 bg-green-600 dark:bg-green-400 transition-all duration-300 ease-out group-hover:w-full"></span>
                    </NuxtLink>
                  </NavigationMenuLink>
                </NavigationMenuItem>
              </NavigationMenuList>
            </NavigationMenu>
          </div>

          <!-- CTA Buttons -->
          <div class="hidden items-center space-x-4 md:flex">
            <NuxtLink to="/auth/login" class="sign-in-button btn-animate group relative inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-md px-4 py-2 text-sm font-semibold uppercase tracking-wide text-slate-700 dark:text-slate-300 hover:text-slate-900 dark:hover:text-white hover:bg-slate-100/50 dark:hover:bg-slate-800/50 transition-all duration-300 no-underline focus-visible:outline-none focus-visible:ring-1 focus-visible:ring-ring disabled:pointer-events-none disabled:opacity-50">
              SIGN IN
            </NuxtLink>
            <Button size="default" class="btn-animate group relative text-sm font-semibold bg-slate-900 dark:bg-slate-800 text-white hover:bg-slate-800 dark:hover:bg-slate-700 shadow-md hover:shadow-xl hover:scale-105 active:scale-95 transition-all duration-300 overflow-hidden" as-child>
              <NuxtLink to="/auth/login" class="relative z-10 flex items-center gap-2 no-underline">
                <span>Get Started</span>
                <svg class="w-4 h-4 transition-transform duration-300 ease-out group-hover:translate-x-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7l5 5m0 0l-5 5m5-5H6"></path>
                </svg>
              </NuxtLink>
            </Button>
          </div>

          <!-- Mobile Menu Button -->
          <div class="flex items-center space-x-2 md:hidden">
            <Button variant="ghost" size="sm" class="text-sm font-semibold uppercase tracking-wide text-slate-700 dark:text-slate-300 hover:text-slate-900 dark:hover:text-white transition-all duration-300" as-child>
              <NuxtLink to="/auth/login" class="no-underline">Sign In</NuxtLink>
            </Button>
          </div>
        </div>
      </div>
    </nav>

    <!-- Hero Section -->
    <section class="relative overflow-hidden px-4 pt-32 pb-24 sm:px-6 lg:px-8 bg-gradient-to-br from-slate-50 via-blue-50/50 to-purple-50/50 dark:from-slate-950 dark:via-slate-900 dark:to-slate-950">
      <!-- Animated Background Elements -->
      <div class="absolute inset-0 overflow-hidden -z-10">
        <div class="absolute top-0 left-1/4 w-96 h-96 bg-blue-400/10 rounded-full blur-3xl animate-pulse"></div>
        <div class="absolute bottom-0 right-1/4 w-96 h-96 bg-purple-400/10 rounded-full blur-3xl animate-pulse delay-1000"></div>
        <div class="absolute inset-0 bg-[radial-gradient(circle_at_30%_20%,rgba(59,130,246,0.08),transparent_50%)] dark:bg-[radial-gradient(circle_at_30%_20%,rgba(59,130,246,0.04),transparent_50%)]"></div>
      </div>
      
      <div class="mx-auto max-w-7xl">
        <div class="text-center">
          <!-- Badge -->
          <div class="inline-flex items-center gap-2 px-4 py-2 mb-8 text-sm font-semibold text-blue-700 dark:text-blue-300 bg-blue-100/80 dark:bg-blue-900/30 rounded-full border border-blue-200 dark:border-blue-800 backdrop-blur-sm shadow-sm">
            <span class="relative flex h-2 w-2">
              <span class="animate-ping absolute inline-flex h-full w-full rounded-full bg-blue-400 opacity-75"></span>
              <span class="relative inline-flex rounded-full h-2 w-2 bg-blue-500"></span>
            </span>
            Collaborative Service-as-Software ERP Platform
          </div>

          <!-- Main Heading -->
          <h1 class="mb-6 text-5xl font-extrabold tracking-tight text-slate-900 dark:text-white sm:text-6xl md:text-7xl lg:text-8xl">
            <span class="block">Empower Your Business</span>
            <span class="block mt-3 bg-gradient-to-r from-blue-600 via-purple-600 to-pink-600 bg-clip-text text-transparent">
              with TOSS ERP
            </span>
          </h1>

          <!-- Subheading -->
          <p class="mx-auto mb-4 max-w-3xl text-xl leading-relaxed text-slate-600 dark:text-slate-300 sm:text-2xl font-medium">
            Complete ERP solution powered by <span class="font-semibold text-slate-900 dark:text-white">ERPNext modules</span>, enhanced with <span class="font-semibold text-blue-600 dark:text-blue-400">AI Copilot</span> and a <span class="font-semibold text-purple-600 dark:text-purple-400">collaborative network</span>
          </p>
          <p class="mx-auto mb-12 max-w-2xl text-lg text-slate-500 dark:text-slate-400">
            Connect shops, suppliers, drivers, and financial partners in one powerful platform
          </p>

          <!-- CTA Buttons -->
          <div class="flex flex-col gap-4 justify-center items-center sm:flex-row mb-16">
            <Button size="lg" class="px-10 py-7 h-auto text-lg font-semibold shadow-xl shadow-blue-500/25 hover:shadow-2xl hover:shadow-blue-500/40 hover:scale-105 transition-all duration-300" as-child>
              <NuxtLink to="/auth/login" class="flex items-center gap-2 no-underline">
                <span>Login to TOSS Platform</span>
                <svg class="w-5 h-5 transition-transform duration-300 group-hover:translate-x-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 7l5 5m0 0l-5 5m5-5H6"></path>
                </svg>
              </NuxtLink>
            </Button>
            <Button size="lg" variant="outline" class="px-10 py-7 h-auto text-lg font-semibold border-2 border-slate-300 dark:border-slate-700 hover:border-slate-400 dark:hover:border-slate-600 hover:bg-slate-50 dark:hover:bg-slate-900 transition-all duration-300" as-child>
              <NuxtLink to="#features" class="flex items-center gap-2 no-underline">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                </svg>
                <span>Explore Features</span>
              </NuxtLink>
            </Button>
          </div>

          <!-- Trust Indicators -->
          <div class="flex flex-wrap justify-center items-center gap-8 mb-16 text-sm text-slate-500 dark:text-slate-400">
            <div class="flex items-center gap-2">
              <svg class="w-5 h-5 text-green-500" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
              </svg>
              <span>ERPNext Powered</span>
            </div>
            <div class="flex items-center gap-2">
              <svg class="w-5 h-5 text-blue-500" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
              </svg>
              <span>AI Copilot Included</span>
            </div>
            <div class="flex items-center gap-2">
              <svg class="w-5 h-5 text-purple-500" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd"></path>
              </svg>
              <span>Collaborative Network</span>
            </div>
          </div>
        </div>

        <!-- Hero Stats -->
        <div class="grid grid-cols-2 gap-6 mx-auto max-w-5xl sm:grid-cols-4">
          <Card class="group p-8 text-center border-2 border-slate-200 dark:border-slate-800 bg-white/90 dark:bg-slate-900/90 backdrop-blur-md transition-all duration-300 hover:shadow-2xl hover:shadow-blue-500/20 hover:border-blue-300 dark:hover:border-blue-700 hover:-translate-y-2">
            <div class="flex justify-center mb-4">
              <div class="p-3 bg-gradient-to-br from-blue-500 to-blue-600 rounded-2xl shadow-lg shadow-blue-500/30 group-hover:scale-110 transition-transform duration-300">
                <UsersIcon class="w-6 h-6 text-white" />
              </div>
            </div>
            <div class="mb-2 text-4xl font-extrabold bg-gradient-to-r from-blue-600 to-blue-700 bg-clip-text text-transparent dark:from-blue-400 dark:to-blue-500">500+</div>
            <div class="text-sm font-semibold text-slate-600 dark:text-slate-400 uppercase tracking-wide">Active Users</div>
          </Card>
          <Card class="group p-8 text-center border-2 border-slate-200 dark:border-slate-800 bg-white/90 dark:bg-slate-900/90 backdrop-blur-md transition-all duration-300 hover:shadow-2xl hover:shadow-purple-500/20 hover:border-purple-300 dark:hover:border-purple-700 hover:-translate-y-2">
            <div class="flex justify-center mb-4">
              <div class="p-3 bg-gradient-to-br from-purple-500 to-purple-600 rounded-2xl shadow-lg shadow-purple-500/30 group-hover:scale-110 transition-transform duration-300">
                <CurrencyDollarIcon class="w-6 h-6 text-white" />
              </div>
            </div>
            <div class="mb-2 text-4xl font-extrabold bg-gradient-to-r from-purple-600 to-purple-700 bg-clip-text text-transparent dark:from-purple-400 dark:to-purple-500">10K+</div>
            <div class="text-sm font-semibold text-slate-600 dark:text-slate-400 uppercase tracking-wide">Transactions</div>
          </Card>
          <Card class="group p-8 text-center border-2 border-slate-200 dark:border-slate-800 bg-white/90 dark:bg-slate-900/90 backdrop-blur-md transition-all duration-300 hover:shadow-2xl hover:shadow-pink-500/20 hover:border-pink-300 dark:hover:border-pink-700 hover:-translate-y-2">
            <div class="flex justify-center mb-4">
              <div class="p-3 bg-gradient-to-br from-pink-500 to-pink-600 rounded-2xl shadow-lg shadow-pink-500/30 group-hover:scale-110 transition-transform duration-300">
                <ArchiveBoxIcon class="w-6 h-6 text-white" />
              </div>
            </div>
            <div class="mb-2 text-4xl font-extrabold bg-gradient-to-r from-pink-600 to-pink-700 bg-clip-text text-transparent dark:from-pink-400 dark:to-pink-500">50+</div>
            <div class="text-sm font-semibold text-slate-600 dark:text-slate-400 uppercase tracking-wide">Businesses</div>
          </Card>
          <Card class="group p-8 text-center border-2 border-slate-200 dark:border-slate-800 bg-white/90 dark:bg-slate-900/90 backdrop-blur-md transition-all duration-300 hover:shadow-2xl hover:shadow-green-500/20 hover:border-green-300 dark:hover:border-green-700 hover:-translate-y-2">
            <div class="flex justify-center mb-4">
              <div class="p-3 bg-gradient-to-br from-green-500 to-green-600 rounded-2xl shadow-lg shadow-green-500/30 group-hover:scale-110 transition-transform duration-300">
                <ArrowTrendingUpIcon class="w-6 h-6 text-white" />
              </div>
            </div>
            <div class="mb-2 text-4xl font-extrabold bg-gradient-to-r from-green-600 to-green-700 bg-clip-text text-transparent dark:from-green-400 dark:to-green-500">99.9%</div>
            <div class="text-sm font-semibold text-slate-600 dark:text-slate-400 uppercase tracking-wide">Uptime</div>
          </Card>
        </div>
      </div>
    </section>

    <!-- Features Section -->
    <section id="features" class="px-4 py-20 sm:px-6 lg:px-8 bg-white dark:bg-slate-950">
      <div class="mx-auto max-w-7xl">
        <div class="mb-16 text-center">
          <Badge class="mb-4 px-3 py-1 text-xs font-semibold uppercase tracking-wider" variant="outline">ERPNext-Powered</Badge>
          <h2 class="mb-4 text-3xl font-bold text-slate-900 dark:text-white sm:text-4xl md:text-5xl">Complete Business Management</h2>
          <p class="mx-auto max-w-2xl text-lg text-slate-600 dark:text-slate-300">
            Full-featured ERP solution with all the modules you need, enhanced by AI Copilot and collaborative network features
          </p>
        </div>

        <div class="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3">
          <Card class="p-8 border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 transition-all hover:shadow-xl hover:shadow-blue-500/10 hover:-translate-y-1 group">
            <div class="flex justify-center items-center mb-6 w-14 h-14 bg-gradient-to-br from-blue-500 to-blue-600 rounded-xl shadow-lg shadow-blue-500/20 transition-transform group-hover:scale-110 group-hover:rotate-3">
              <CurrencyDollarIcon class="w-7 h-7 text-white" />
            </div>
            <h3 class="mb-3 text-xl font-bold text-slate-900 dark:text-white">Accounts</h3>
            <p class="leading-relaxed text-slate-600 dark:text-slate-300">
              Complete financial management with accounting, invoicing, payments, and financial reporting.
            </p>
          </Card>

          <Card class="p-8 border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 transition-all hover:shadow-xl hover:shadow-green-500/10 hover:-translate-y-1 group">
            <div class="flex justify-center items-center mb-6 w-14 h-14 bg-gradient-to-br from-green-500 to-green-600 rounded-xl shadow-lg shadow-green-500/20 transition-transform group-hover:scale-110 group-hover:rotate-3">
              <UsersIcon class="w-7 h-7 text-white" />
            </div>
            <h3 class="mb-3 text-xl font-bold text-slate-900 dark:text-white">CRM</h3>
            <p class="leading-relaxed text-slate-600 dark:text-slate-300">
              Manage customer relationships, track leads, opportunities, and build lasting customer connections.
            </p>
          </Card>

          <Card class="p-8 border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 transition-all hover:shadow-xl hover:shadow-purple-500/10 hover:-translate-y-1 group">
            <div class="flex justify-center items-center mb-6 w-14 h-14 bg-gradient-to-br from-purple-500 to-purple-600 rounded-xl shadow-lg shadow-purple-500/20 transition-transform group-hover:scale-110 group-hover:rotate-3">
              <ShoppingCartIcon class="w-7 h-7 text-white" />
            </div>
            <h3 class="mb-3 text-xl font-bold text-slate-900 dark:text-white">Buying</h3>
            <p class="leading-relaxed text-slate-600 dark:text-slate-300">
              Streamline procurement with purchase orders, supplier management, and automated workflows.
            </p>
          </Card>

          <Card class="p-8 border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 transition-all hover:shadow-xl hover:shadow-yellow-500/10 hover:-translate-y-1 group">
            <div class="flex justify-center items-center mb-6 w-14 h-14 bg-gradient-to-br from-yellow-500 to-yellow-600 rounded-xl shadow-lg shadow-yellow-500/20 transition-transform group-hover:scale-110 group-hover:rotate-3">
              <ChartBarIcon class="w-7 h-7 text-white" />
            </div>
            <h3 class="mb-3 text-xl font-bold text-slate-900 dark:text-white">Selling</h3>
            <p class="leading-relaxed text-slate-600 dark:text-slate-300">
              Complete sales management with quotations, orders, invoices, and customer portal integration.
            </p>
          </Card>

          <Card class="p-8 border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 transition-all hover:shadow-xl hover:shadow-pink-500/10 hover:-translate-y-1 group">
            <div class="flex justify-center items-center mb-6 w-14 h-14 bg-gradient-to-br from-pink-500 to-pink-600 rounded-xl shadow-lg shadow-pink-500/20 transition-transform group-hover:scale-110 group-hover:rotate-3">
              <ArchiveBoxIcon class="w-7 h-7 text-white" />
            </div>
            <h3 class="mb-3 text-xl font-bold text-slate-900 dark:text-white">Stock</h3>
            <p class="leading-relaxed text-slate-600 dark:text-slate-300">
              Advanced inventory management with stock tracking, warehouses, batch/serial numbers, and automated reordering.
            </p>
          </Card>

          <Card class="p-8 border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 transition-all hover:shadow-xl hover:shadow-indigo-500/10 hover:-translate-y-1 group">
            <div class="flex justify-center items-center mb-6 w-14 h-14 bg-gradient-to-br from-indigo-500 to-indigo-600 rounded-xl shadow-lg shadow-indigo-500/20 transition-transform group-hover:scale-110 group-hover:rotate-3">
              <ArrowTrendingUpIcon class="w-7 h-7 text-white" />
            </div>
            <h3 class="mb-3 text-xl font-bold text-slate-900 dark:text-white">AI Copilot</h3>
            <p class="leading-relaxed text-slate-600 dark:text-slate-300">
              Service-as-Software AI assistant that automates tasks, provides insights, and helps you make better decisions.
            </p>
          </Card>
        </div>
      </div>
    </section>

    <!-- Modules Section -->
    <section id="modules" class="px-4 py-20 sm:px-6 lg:px-8 bg-slate-50 dark:bg-slate-900/50">
      <div class="mx-auto max-w-7xl">
        <div class="mb-16 text-center">
          <Badge class="mb-4 px-3 py-1 text-xs font-semibold uppercase tracking-wider" variant="outline">ERPNext Modules</Badge>
          <h2 class="mb-4 text-3xl font-bold text-slate-900 dark:text-white sm:text-4xl md:text-5xl">Complete ERP Solution</h2>
          <p class="mx-auto max-w-2xl text-lg text-slate-600 dark:text-slate-300">
            TOSS includes all standard ERPNext modules plus collaborative network features
          </p>
        </div>

        <div class="grid grid-cols-2 gap-3 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-7">
          <Card class="p-4 text-center border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 hover:shadow-md hover:border-blue-300 dark:hover:border-blue-700 transition-all cursor-pointer group">
            <div class="text-sm font-semibold text-slate-700 dark:text-slate-300 group-hover:text-blue-600 dark:group-hover:text-blue-400 transition-colors">Accounts</div>
          </Card>
          <Card class="p-4 text-center border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 hover:shadow-md hover:border-green-300 dark:hover:border-green-700 transition-all cursor-pointer group">
            <div class="text-sm font-semibold text-slate-700 dark:text-slate-300 group-hover:text-green-600 dark:group-hover:text-green-400 transition-colors">CRM</div>
          </Card>
          <Card class="p-4 text-center border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 hover:shadow-md hover:border-purple-300 dark:hover:border-purple-700 transition-all cursor-pointer group">
            <div class="text-sm font-semibold text-slate-700 dark:text-slate-300 group-hover:text-purple-600 dark:group-hover:text-purple-400 transition-colors">Buying</div>
          </Card>
          <Card class="p-4 text-center border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 hover:shadow-md hover:border-yellow-300 dark:hover:border-yellow-700 transition-all cursor-pointer group">
            <div class="text-sm font-semibold text-slate-700 dark:text-slate-300 group-hover:text-yellow-600 dark:group-hover:text-yellow-400 transition-colors">Selling</div>
          </Card>
          <Card class="p-4 text-center border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 hover:shadow-md hover:border-pink-300 dark:hover:border-pink-700 transition-all cursor-pointer group">
            <div class="text-sm font-semibold text-slate-700 dark:text-slate-300 group-hover:text-pink-600 dark:group-hover:text-pink-400 transition-colors">Stock</div>
          </Card>
          <Card class="p-4 text-center border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 hover:shadow-md hover:border-indigo-300 dark:hover:border-indigo-700 transition-all cursor-pointer group">
            <div class="text-sm font-semibold text-slate-700 dark:text-slate-300 group-hover:text-indigo-600 dark:group-hover:text-indigo-400 transition-colors">Projects</div>
          </Card>
          <Card class="p-4 text-center border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 hover:shadow-md hover:border-orange-300 dark:hover:border-orange-700 transition-all cursor-pointer group">
            <div class="text-sm font-semibold text-slate-700 dark:text-slate-300 group-hover:text-orange-600 dark:group-hover:text-orange-400 transition-colors">Manufacturing</div>
          </Card>
          <Card class="p-4 text-center border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 hover:shadow-md hover:border-teal-300 dark:hover:border-teal-700 transition-all cursor-pointer group">
            <div class="text-sm font-semibold text-slate-700 dark:text-slate-300 group-hover:text-teal-600 dark:group-hover:text-teal-400 transition-colors">Assets</div>
          </Card>
          <Card class="p-4 text-center border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 hover:shadow-md hover:border-red-300 dark:hover:border-red-700 transition-all cursor-pointer group">
            <div class="text-sm font-semibold text-slate-700 dark:text-slate-300 group-hover:text-red-600 dark:group-hover:text-red-400 transition-colors">Quality</div>
          </Card>
          <Card class="p-4 text-center border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 hover:shadow-md hover:border-cyan-300 dark:hover:border-cyan-700 transition-all cursor-pointer group">
            <div class="text-sm font-semibold text-slate-700 dark:text-slate-300 group-hover:text-cyan-600 dark:group-hover:text-cyan-400 transition-colors">Support</div>
          </Card>
          <Card class="p-4 text-center border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 hover:shadow-md hover:border-violet-300 dark:hover:border-violet-700 transition-all cursor-pointer group">
            <div class="text-sm font-semibold text-slate-700 dark:text-slate-300 group-hover:text-violet-600 dark:group-hover:text-violet-400 transition-colors">Portal</div>
          </Card>
          <Card class="p-4 text-center border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 hover:shadow-md hover:border-emerald-300 dark:hover:border-emerald-700 transition-all cursor-pointer group">
            <div class="text-sm font-semibold text-slate-700 dark:text-slate-300 group-hover:text-emerald-600 dark:group-hover:text-emerald-400 transition-colors">Utilities</div>
          </Card>
          <Card class="p-4 text-center border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 hover:shadow-md hover:border-rose-300 dark:hover:border-rose-700 transition-all cursor-pointer group">
            <div class="text-sm font-semibold text-slate-700 dark:text-slate-300 group-hover:text-rose-600 dark:group-hover:text-rose-400 transition-colors">Regional</div>
          </Card>
          <Card class="p-4 text-center border border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-900 hover:shadow-md hover:border-sky-300 dark:hover:border-sky-700 transition-all cursor-pointer group">
            <div class="text-sm font-semibold text-slate-700 dark:text-slate-300 group-hover:text-sky-600 dark:group-hover:text-sky-400 transition-colors">Communication</div>
          </Card>
        </div>

        <div class="mt-12 text-center">
          <p class="text-base font-medium text-slate-700 dark:text-slate-300 mb-6">
            Plus collaborative network features: <span class="text-blue-600 dark:text-blue-400 font-semibold">Group Buying</span>, <span class="text-purple-600 dark:text-purple-400 font-semibold">Delivery Coordination</span>, <span class="text-green-600 dark:text-green-400 font-semibold">Financial Services Integration</span>
          </p>
          <Button size="lg" class="shadow-lg shadow-blue-500/20 hover:shadow-xl hover:shadow-blue-500/30" as-child>
            <NuxtLink to="/auth/login" class="no-underline">Access All Modules</NuxtLink>
          </Button>
        </div>
      </div>
    </section>

    <!-- Stats Section -->
    <section id="stats" class="px-4 py-20 sm:px-6 lg:px-8 bg-white dark:bg-slate-950">
      <div class="mx-auto max-w-7xl">
        <div class="mb-16 text-center">
          <Badge class="mb-4 px-3 py-1 text-xs font-semibold uppercase tracking-wider" variant="outline">Performance</Badge>
          <h2 class="mb-4 text-3xl font-bold text-slate-900 dark:text-white sm:text-4xl md:text-5xl">Trusted by Growing Businesses</h2>
          <p class="mx-auto max-w-2xl text-lg text-slate-600 dark:text-slate-300">
            See how TOSS is helping businesses thrive
          </p>
        </div>

        <div class="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-4">
          <Card class="p-8 text-center border border-slate-200 dark:border-slate-800 bg-gradient-to-br from-white to-green-50/50 dark:from-slate-900 dark:to-green-950/20 hover:shadow-xl transition-all">
            <div class="flex justify-center items-center mb-4">
              <div class="p-4 bg-gradient-to-br from-green-500 to-green-600 rounded-2xl shadow-lg shadow-green-500/20">
                <CurrencyDollarIcon class="w-8 h-8 text-white" />
              </div>
            </div>
            <div class="mb-2 text-4xl font-bold text-slate-900 dark:text-white">R 2.5M+</div>
            <div class="mb-1 text-sm font-semibold text-slate-600 dark:text-slate-400">Total Sales Processed</div>
            <div class="text-xs font-medium text-green-600 dark:text-green-400">+45% this month</div>
          </Card>

          <Card class="p-8 text-center border border-slate-200 dark:border-slate-800 bg-gradient-to-br from-white to-blue-50/50 dark:from-slate-900 dark:to-blue-950/20 hover:shadow-xl transition-all">
            <div class="flex justify-center items-center mb-4">
              <div class="p-4 bg-gradient-to-br from-blue-500 to-blue-600 rounded-2xl shadow-lg shadow-blue-500/20">
                <ArchiveBoxIcon class="w-8 h-8 text-white" />
              </div>
            </div>
            <div class="mb-2 text-4xl font-bold text-slate-900 dark:text-white">15K+</div>
            <div class="mb-1 text-sm font-semibold text-slate-600 dark:text-slate-400">Stock Items Managed</div>
            <div class="text-xs font-medium text-blue-600 dark:text-blue-400">Real-time tracking</div>
          </Card>

          <Card class="p-8 text-center border border-slate-200 dark:border-slate-800 bg-gradient-to-br from-white to-purple-50/50 dark:from-slate-900 dark:to-purple-950/20 hover:shadow-xl transition-all">
            <div class="flex justify-center items-center mb-4">
              <div class="p-4 bg-gradient-to-br from-purple-500 to-purple-600 rounded-2xl shadow-lg shadow-purple-500/20">
                <ShoppingCartIcon class="w-8 h-8 text-white" />
              </div>
            </div>
            <div class="mb-2 text-4xl font-bold text-slate-900 dark:text-white">8K+</div>
            <div class="mb-1 text-sm font-semibold text-slate-600 dark:text-slate-400">Orders Completed</div>
            <div class="text-xs font-medium text-purple-600 dark:text-purple-400">99.2% success rate</div>
          </Card>

          <Card class="p-8 text-center border border-slate-200 dark:border-slate-800 bg-gradient-to-br from-white to-yellow-50/50 dark:from-slate-900 dark:to-yellow-950/20 hover:shadow-xl transition-all">
            <div class="flex justify-center items-center mb-4">
              <div class="p-4 bg-gradient-to-br from-yellow-500 to-yellow-600 rounded-2xl shadow-lg shadow-yellow-500/20">
                <UsersIcon class="w-8 h-8 text-white" />
              </div>
            </div>
            <div class="mb-2 text-4xl font-bold text-slate-900 dark:text-white">500+</div>
            <div class="mb-1 text-sm font-semibold text-slate-600 dark:text-slate-400">Active Users</div>
            <div class="text-xs font-medium text-yellow-600 dark:text-yellow-400">Growing daily</div>
          </Card>
        </div>
      </div>
    </section>

    <!-- CTA Section -->
    <section id="cta" class="overflow-hidden relative px-4 py-20 sm:px-6 lg:px-8 bg-gradient-to-br from-blue-600 via-purple-600 to-pink-600">
      <div class="absolute inset-0 bg-[radial-gradient(circle_at_50%_50%,rgba(255,255,255,0.1),transparent_70%)] -z-10"></div>
      <div class="absolute inset-0 opacity-20 -z-10" style="background-image: url('data:image/svg+xml,%3Csvg width=&quot;60&quot; height=&quot;60&quot; viewBox=&quot;0 0 60 60&quot; xmlns=&quot;http://www.w3.org/2000/svg&quot;%3E%3Cg fill=&quot;none&quot; fill-rule=&quot;evenodd&quot;%3E%3Cg fill=&quot;%23ffffff&quot; fill-opacity=&quot;0.05&quot;%3E%3Cpath d=&quot;M36 34v-4h-2v4h-4v2h4v4h2v-4h4v-2h-4zm0-30V0h-2v4h-4v2h4v4h2V6h4V4h-4zM6 34v-4H4v4H0v2h4v4h2v-4h4v-2H6zM6 4V0H4v4H0v2h4v4h2V6h4V4H6z&quot;/%3E%3C/g%3E%3C/g%3E%3C/svg%3E');"></div>
      
      <div class="relative mx-auto max-w-4xl text-center">
        <h2 class="mb-6 text-3xl font-bold text-white sm:text-4xl md:text-5xl">
          Ready to Transform Your Business?
        </h2>
        <p class="mx-auto mb-10 max-w-2xl text-lg text-white/90 sm:text-xl">
          Join hundreds of businesses already using TOSS to streamline their operations and grow faster.
        </p>
        <div class="flex flex-col gap-4 justify-center sm:flex-row">
          <Button size="lg" variant="secondary" class="px-8 py-6 h-auto text-base font-semibold shadow-xl hover:shadow-2xl transition-all" as-child>
              <NuxtLink to="/auth/login" class="no-underline">Login to TOSS Platform</NuxtLink>
            </Button>
            <Button size="lg" variant="outline" class="px-8 py-6 h-auto text-base font-semibold text-white bg-white/10 border-2 border-white/30 hover:bg-white/20 backdrop-blur-sm" as-child>
              <NuxtLink to="#modules" class="no-underline">View All Modules</NuxtLink>
          </Button>
        </div>
      </div>
    </section>

    <!-- Footer -->
    <footer class="px-4 py-12 border-t border-slate-200 dark:border-slate-800 bg-white dark:bg-slate-950 sm:px-6 lg:px-8">
      <div class="mx-auto max-w-7xl">
        <div class="grid grid-cols-1 gap-8 md:grid-cols-4">
          <div>
            <div class="flex items-center mb-4 space-x-2">
              <div class="flex justify-center items-center w-10 h-10 bg-gradient-to-br from-blue-600 to-purple-600 rounded-lg shadow-lg">
                <span class="text-xl font-bold text-white">T</span>
              </div>
              <span class="text-xl font-bold text-slate-900 dark:text-white">TOSS</span>
            </div>
            <p class="text-sm text-slate-600 dark:text-slate-400 leading-relaxed">
              Township Operations Support System - Empowering businesses to thrive with collaborative ERP solutions.
            </p>
          </div>
          <div>
            <h4 class="mb-4 text-sm font-semibold text-slate-900 dark:text-white uppercase tracking-wider">Platform</h4>
            <ul class="space-y-3 text-sm text-slate-600 dark:text-slate-400">
              <li><NuxtLink to="/auth/login" class="transition-colors hover:text-blue-600 dark:hover:text-blue-400 no-underline">Login</NuxtLink></li>
              <li><NuxtLink to="/auth/register" class="transition-colors hover:text-blue-600 dark:hover:text-blue-400 no-underline">Register</NuxtLink></li>
              <li><NuxtLink to="#modules" class="transition-colors hover:text-blue-600 dark:hover:text-blue-400 no-underline">Modules</NuxtLink></li>
            </ul>
          </div>
          <div>
            <h4 class="mb-4 text-sm font-semibold text-slate-900 dark:text-white uppercase tracking-wider">ERP Modules</h4>
            <ul class="space-y-3 text-sm text-slate-600 dark:text-slate-400">
              <li><a href="#modules" class="transition-colors hover:text-blue-600 dark:hover:text-blue-400 no-underline">All Modules</a></li>
              <li><a href="#features" class="transition-colors hover:text-blue-600 dark:hover:text-blue-400 no-underline">Key Features</a></li>
              <li><a href="#cta" class="transition-colors hover:text-blue-600 dark:hover:text-blue-400 no-underline">Get Started</a></li>
            </ul>
          </div>
          <div>
            <h4 class="mb-4 text-sm font-semibold text-slate-900 dark:text-white uppercase tracking-wider">Support</h4>
            <ul class="space-y-3 text-sm text-slate-600 dark:text-slate-400">
              <li><a href="#" class="transition-colors hover:text-blue-600 dark:hover:text-blue-400 no-underline">Documentation</a></li>
              <li><a href="#" class="transition-colors hover:text-blue-600 dark:hover:text-blue-400 no-underline">Help Center</a></li>
              <li><a href="#" class="transition-colors hover:text-blue-600 dark:hover:text-blue-400 no-underline">Contact</a></li>
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
