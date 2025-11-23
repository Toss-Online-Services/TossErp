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
        <CardTitle class="text-2xl">Welcome Back</CardTitle>
        <p class="text-muted-foreground mt-2">Sign in to your account to continue</p>
      </CardHeader>
      <CardContent>
        <form @submit.prevent="handleLogin" class="space-y-4">
          <div class="space-y-2">
            <Label for="email">Email</Label>
            <Input
              id="email"
              v-model="email"
              type="email"
              placeholder="your@email.com"
              required
            />
          </div>
          <div class="space-y-2">
            <Label for="password">Password</Label>
            <Input
              id="password"
              v-model="password"
              type="password"
              placeholder="••••••••"
              required
            />
          </div>
          <div class="flex items-center justify-between">
            <div class="flex items-center space-x-2">
              <input type="checkbox" id="remember" class="rounded" />
              <Label for="remember" class="text-sm">Remember me</Label>
            </div>
            <NuxtLink to="/forgot-password" class="text-sm text-primary hover:underline">
              Forgot password?
            </NuxtLink>
          </div>
          <Button type="submit" class="w-full" :disabled="loading">
            {{ loading ? 'Signing in...' : 'Sign In' }}
          </Button>
        </form>
        <div class="mt-6 text-center text-sm">
          <span class="text-muted-foreground">Don't have an account? </span>
          <NuxtLink to="/register" class="text-primary hover:underline font-medium">
            Sign up
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
const email = ref('')
const password = ref('')
const loading = ref(false)

const handleLogin = async () => {
  loading.value = true
  try {
    // TODO: Implement actual authentication
    await new Promise(resolve => setTimeout(resolve, 1000))
    // Navigate to dashboard after successful login
    await router.push('/dashboard')
  } catch (error) {
    console.error('Login failed:', error)
  } finally {
    loading.value = false
  }
}

useHead({
  title: 'Sign In - TOSS'
})
</script>


