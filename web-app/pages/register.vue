<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-br from-background to-muted p-4">
    <Card class="w-full max-w-md">
      <CardHeader class="text-center">
        <div class="flex items-center justify-center space-x-2 mb-4">
          <div class="w-12 h-12 bg-primary rounded-lg flex items-center justify-center">
            <span class="text-primary-foreground font-bold text-2xl">T</span>
          </div>
          <span class="text-3xl font-bold">TOSS</span>
        </div>
        <CardTitle class="text-2xl">Create Your Account</CardTitle>
        <p class="text-muted-foreground mt-2">Start managing your business like a pro</p>
      </CardHeader>
      <CardContent>
        <form @submit.prevent="handleRegister" class="space-y-4">
          <div class="space-y-2">
            <Label for="businessName">Business Name</Label>
            <Input
              id="businessName"
              v-model="form.businessName"
              type="text"
              placeholder="Mama Dlamini's Spaza"
              required
            />
          </div>
          <div class="space-y-2">
            <Label for="businessType">Business Type</Label>
            <select
              id="businessType"
              v-model="form.businessType"
              class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2"
              required
            >
              <option value="">Select business type</option>
              <option value="spaza">Spaza Shop</option>
              <option value="tshisanyama">Tshisanyama</option>
              <option value="bakery">Bakery</option>
              <option value="salon">Hair Salon</option>
              <option value="mechanic">Mechanic</option>
              <option value="tailor">Tailor</option>
              <option value="butchery">Butchery</option>
              <option value="hawker">Fruit & Vegetable Hawker</option>
              <option value="other">Other</option>
            </select>
          </div>
          <div class="space-y-2">
            <Label for="fullName">Your Full Name</Label>
            <Input
              id="fullName"
              v-model="form.fullName"
              type="text"
              placeholder="John Doe"
              required
            />
          </div>
          <div class="space-y-2">
            <Label for="email">Email</Label>
            <Input
              id="email"
              v-model="form.email"
              type="email"
              placeholder="your@email.com"
              required
            />
          </div>
          <div class="space-y-2">
            <Label for="phone">Phone Number</Label>
            <Input
              id="phone"
              v-model="form.phone"
              type="tel"
              placeholder="+27 12 345 6789"
              required
            />
          </div>
          <div class="space-y-2">
            <Label for="password">Password</Label>
            <Input
              id="password"
              v-model="form.password"
              type="password"
              placeholder="••••••••"
              required
            />
          </div>
          <div class="space-y-2">
            <Label for="confirmPassword">Confirm Password</Label>
            <Input
              id="confirmPassword"
              v-model="form.confirmPassword"
              type="password"
              placeholder="••••••••"
              required
            />
          </div>
          <div class="flex items-center space-x-2">
            <input type="checkbox" id="terms" class="rounded" required />
            <Label for="terms" class="text-sm">
              I agree to the <a href="#" class="text-primary hover:underline">Terms of Service</a> and <a href="#" class="text-primary hover:underline">Privacy Policy</a>
            </Label>
          </div>
          <Button type="submit" class="w-full" :disabled="loading">
            {{ loading ? 'Creating account...' : 'Create Account' }}
          </Button>
        </form>
        <div class="mt-6 text-center text-sm">
          <span class="text-muted-foreground">Already have an account? </span>
          <NuxtLink to="/login" class="text-primary hover:underline font-medium">
            Sign in
          </NuxtLink>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const loading = ref(false)
const form = ref({
  businessName: '',
  businessType: '',
  fullName: '',
  email: '',
  phone: '',
  password: '',
  confirmPassword: ''
})

const handleRegister = async () => {
  if (form.value.password !== form.value.confirmPassword) {
    alert('Passwords do not match')
    return
  }
  
  loading.value = true
  try {
    // TODO: Implement actual registration
    await new Promise(resolve => setTimeout(resolve, 1000))
    // Navigate to dashboard after successful registration
    await router.push('/dashboard')
  } catch (error) {
    console.error('Registration failed:', error)
  } finally {
    loading.value = false
  }
}

useHead({
  title: 'Sign Up - TOSS'
})
</script>


