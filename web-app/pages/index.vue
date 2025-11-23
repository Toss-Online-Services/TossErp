<template>
  <div class="min-h-screen bg-gradient-to-b from-background to-muted">
    <!-- Navigation -->
    <nav class="border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div class="container mx-auto px-4 py-4 flex items-center justify-between">
        <div class="flex items-center space-x-2">
          <div class="w-10 h-10 bg-primary rounded-lg flex items-center justify-center">
            <span class="text-primary-foreground font-bold text-xl">T</span>
          </div>
          <span class="text-2xl font-bold">TOSS</span>
        </div>
        <div class="hidden md:flex items-center space-x-6">
          <a href="#features" class="text-sm font-medium hover:text-primary transition-colors">Features</a>
          <a href="#how-it-works" class="text-sm font-medium hover:text-primary transition-colors">How It Works</a>
          <a href="#pricing" class="text-sm font-medium hover:text-primary transition-colors">Pricing</a>
          <NuxtLink to="/login" class="text-sm font-medium hover:text-primary transition-colors">Sign In</NuxtLink>
          <NuxtLink to="/register">
            <Button>Get Started</Button>
          </NuxtLink>
        </div>
      </div>
    </nav>

    <!-- Hero Section -->
    <section class="container mx-auto px-4 py-20 text-center">
      <div class="max-w-4xl mx-auto">
        <h1 class="text-5xl md:text-6xl font-bold mb-6 bg-gradient-to-r from-primary to-primary/60 bg-clip-text text-transparent">
          Run Your Business Like a Pro
        </h1>
        <p class="text-xl text-muted-foreground mb-8 max-w-2xl mx-auto">
          TOSS is a digital business backbone for South African township and rural enterprises. 
          Combine ERP-III tools, AI business management, and collaborative networks to buy better, run better, and grow together.
        </p>
        <div class="flex flex-col sm:flex-row gap-4 justify-center">
          <NuxtLink to="/register">
            <Button size="lg" class="w-full sm:w-auto">
              Start Free Trial
            </Button>
          </NuxtLink>
          <Button size="lg" variant="outline" class="w-full sm:w-auto">
            Watch Demo
          </Button>
        </div>
      </div>
    </section>

    <!-- Features Section -->
    <section id="features" class="container mx-auto px-4 py-20">
      <div class="text-center mb-16">
        <h2 class="text-4xl font-bold mb-4">Three Powerful Pillars</h2>
        <p class="text-muted-foreground max-w-2xl mx-auto">
          Everything you need to transform your business from survival mode to growth mode
        </p>
      </div>
      <div class="grid md:grid-cols-3 gap-8">
        <Card>
          <CardHeader>
            <div class="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center mb-4">
              <Icon name="lucide:database" class="w-6 h-6 text-primary" />
            </div>
            <CardTitle>ERP-III Platform</CardTitle>
          </CardHeader>
          <CardContent>
            <p class="text-muted-foreground">
              One app to manage stock, sales, orders, and finances. Replace notebooks, WhatsApp orders, and memory with a digital system that works offline.
            </p>
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <div class="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center mb-4">
              <Icon name="lucide:brain" class="w-6 h-6 text-primary" />
            </div>
            <CardTitle>AI Business Manager</CardTitle>
          </CardHeader>
          <CardContent>
            <p class="text-muted-foreground">
              Like having an MBA in your pocket. Get alerts, suggestions, and automated routines. The AI watches your numbers and helps you make better decisions.
            </p>
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <div class="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center mb-4">
              <Icon name="lucide:network" class="w-6 h-6 text-primary" />
            </div>
            <CardTitle>Collaborative Network</CardTitle>
          </CardHeader>
          <CardContent>
            <p class="text-muted-foreground">
              Work together like a cooperative. Group buying for better prices, shared logistics, and community trust. Turn individual hustles into a collaborative economy.
            </p>
          </CardContent>
        </Card>
      </div>
    </section>

    <!-- Who We Serve -->
    <section class="container mx-auto px-4 py-20 bg-muted/50 rounded-lg">
      <div class="text-center mb-16">
        <h2 class="text-4xl font-bold mb-4">Built for Township & Rural Businesses</h2>
        <p class="text-muted-foreground max-w-2xl mx-auto">
          Designed for the enterprises that power South Africa's informal economy
        </p>
      </div>
      <div class="grid md:grid-cols-3 lg:grid-cols-4 gap-6">
        <div class="text-center p-6 bg-background rounded-lg">
          <Icon name="lucide:store" class="w-8 h-8 text-primary mx-auto mb-2" />
          <h3 class="font-semibold">Spaza Shops</h3>
        </div>
        <div class="text-center p-6 bg-background rounded-lg">
          <Icon name="lucide:utensils" class="w-8 h-8 text-primary mx-auto mb-2" />
          <h3 class="font-semibold">Tshisanyama</h3>
        </div>
        <div class="text-center p-6 bg-background rounded-lg">
          <Icon name="lucide:chef-hat" class="w-8 h-8 text-primary mx-auto mb-2" />
          <h3 class="font-semibold">Bakeries</h3>
        </div>
        <div class="text-center p-6 bg-background rounded-lg">
          <Icon name="lucide:scissors" class="w-8 h-8 text-primary mx-auto mb-2" />
          <h3 class="font-semibold">Salons</h3>
        </div>
        <div class="text-center p-6 bg-background rounded-lg">
          <Icon name="lucide:wrench" class="w-8 h-8 text-primary mx-auto mb-2" />
          <h3 class="font-semibold">Mechanics</h3>
        </div>
        <div class="text-center p-6 bg-background rounded-lg">
          <Icon name="lucide:shirt" class="w-8 h-8 text-primary mx-auto mb-2" />
          <h3 class="font-semibold">Tailors</h3>
        </div>
        <div class="text-center p-6 bg-background rounded-lg">
          <Icon name="lucide:car" class="w-8 h-8 text-primary mx-auto mb-2" />
          <h3 class="font-semibold">Panel Beaters</h3>
        </div>
        <div class="text-center p-6 bg-background rounded-lg">
          <Icon name="lucide:apple" class="w-8 h-8 text-primary mx-auto mb-2" />
          <h3 class="font-semibold">Hawkers</h3>
        </div>
      </div>
    </section>

    <!-- How It Works -->
    <section id="how-it-works" class="container mx-auto px-4 py-20">
      <div class="text-center mb-16">
        <h2 class="text-4xl font-bold mb-4">How It Works</h2>
      </div>
      <div class="max-w-4xl mx-auto space-y-8">
        <div class="flex gap-6">
          <div class="flex-shrink-0 w-12 h-12 bg-primary text-primary-foreground rounded-full flex items-center justify-center font-bold text-xl">
            1
          </div>
          <div>
            <h3 class="text-xl font-semibold mb-2">Sign Up & Set Up</h3>
            <p class="text-muted-foreground">
              Create your account in minutes. Add your products, suppliers, and basic business info. Works on any smartphone.
            </p>
          </div>
        </div>
        <div class="flex gap-6">
          <div class="flex-shrink-0 w-12 h-12 bg-primary text-primary-foreground rounded-full flex items-center justify-center font-bold text-xl">
            2
          </div>
          <div>
            <h3 class="text-xl font-semibold mb-2">Start Using Daily</h3>
            <p class="text-muted-foreground">
              Record sales, track stock, and manage orders. The AI watches your patterns and sends helpful alerts and suggestions.
            </p>
          </div>
        </div>
        <div class="flex gap-6">
          <div class="flex-shrink-0 w-12 h-12 bg-primary text-primary-foreground rounded-full flex items-center justify-center font-bold text-xl">
            3
          </div>
          <div>
            <h3 class="text-xl font-semibold mb-2">Join the Network</h3>
            <p class="text-muted-foreground">
              Connect with suppliers, join group orders for better prices, and use shared delivery services. Build your business credit profile.
            </p>
          </div>
        </div>
        <div class="flex gap-6">
          <div class="flex-shrink-0 w-12 h-12 bg-primary text-primary-foreground rounded-full flex items-center justify-center font-bold text-xl">
            4
          </div>
          <div>
            <h3 class="text-xl font-semibold mb-2">Grow Together</h3>
            <p class="text-muted-foreground">
              Access credit, expand your product range, and scale your business. Turn survival mode into growth mode.
            </p>
          </div>
        </div>
      </div>
    </section>

    <!-- CTA Section -->
    <section class="container mx-auto px-4 py-20">
      <Card class="max-w-4xl mx-auto bg-primary text-primary-foreground">
        <CardHeader class="text-center">
          <CardTitle class="text-3xl mb-4">Ready to Transform Your Business?</CardTitle>
          <p class="text-primary-foreground/90 text-lg">
            Join thousands of township and rural businesses using TOSS to run better, buy better, and grow together.
          </p>
        </CardHeader>
        <CardContent class="text-center">
          <NuxtLink to="/register">
            <Button size="lg" variant="secondary" class="mt-4">
              Start Your Free Trial Today
            </Button>
          </NuxtLink>
        </CardContent>
      </Card>
    </section>

    <!-- Footer -->
    <footer class="border-t bg-muted/50 py-12">
      <div class="container mx-auto px-4">
        <div class="grid md:grid-cols-4 gap-8">
          <div>
            <div class="flex items-center space-x-2 mb-4">
              <div class="w-8 h-8 bg-primary rounded-lg flex items-center justify-center">
                <span class="text-primary-foreground font-bold">T</span>
              </div>
              <span class="text-xl font-bold">TOSS</span>
            </div>
            <p class="text-sm text-muted-foreground">
              Empowering South African SMMEs with modern business tools.
            </p>
          </div>
          <div>
            <h4 class="font-semibold mb-4">Product</h4>
            <ul class="space-y-2 text-sm text-muted-foreground">
              <li><a href="#features" class="hover:text-foreground">Features</a></li>
              <li><a href="#pricing" class="hover:text-foreground">Pricing</a></li>
              <li><NuxtLink to="/login" class="hover:text-foreground">Sign In</NuxtLink></li>
            </ul>
          </div>
          <div>
            <h4 class="font-semibold mb-4">Company</h4>
            <ul class="space-y-2 text-sm text-muted-foreground">
              <li><a href="#" class="hover:text-foreground">About</a></li>
              <li><a href="#" class="hover:text-foreground">Contact</a></li>
              <li><a href="#" class="hover:text-foreground">Support</a></li>
            </ul>
          </div>
          <div>
            <h4 class="font-semibold mb-4">Legal</h4>
            <ul class="space-y-2 text-sm text-muted-foreground">
              <li><a href="#" class="hover:text-foreground">Privacy</a></li>
              <li><a href="#" class="hover:text-foreground">Terms</a></li>
            </ul>
          </div>
        </div>
        <div class="mt-8 pt-8 border-t text-center text-sm text-muted-foreground">
          <p>&copy; 2025 TOSS. All rights reserved.</p>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
useHead({
  title: 'TOSS - ERP for South African SMMEs',
  meta: [
    { name: 'description', content: 'Digital business backbone for township and rural enterprises' }
  ]
})
</script>


