<script setup lang="ts">
import { Button } from '~/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Input } from '~/components/ui/input'
import { Label } from '~/components/ui/label'
import { Separator } from '~/components/ui/separator'

definePageMeta({
  layout: 'auth',
})

const email = ref('')
const password = ref('')
const isLoading = ref(false)

const handleLogin = async () => {
  isLoading.value = true
  try {
    // TODO: Implement login logic
    await navigateTo('/dashboard')
  } catch (error) {
    console.error('Login failed:', error)
  } finally {
    isLoading.value = false
  }
}

const handleGithubLogin = () => {
  // TODO: Implement GitHub OAuth
  console.log('GitHub login')
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center p-4 bg-muted/50">
    <Card class="w-full max-w-md">
      <CardHeader class="space-y-1">
        <div class="flex justify-center mb-4">
          <div class="w-12 h-12 bg-primary rounded-lg flex items-center justify-center">
            <span class="text-2xl font-bold text-primary-foreground">T</span>
          </div>
        </div>
        <CardTitle class="text-2xl text-center">Welcome back</CardTitle>
        <CardDescription class="text-center">
          Enter your email to sign in to your account
        </CardDescription>
      </CardHeader>
      <CardContent class="space-y-4">
        <div class="space-y-2">
          <Label for="email">Email</Label>
          <Input
            id="email"
            v-model="email"
            type="email"
            placeholder="name@example.com"
            autocomplete="email"
          />
        </div>
        <div class="space-y-2">
          <div class="flex items-center justify-between">
            <Label for="password">Password</Label>
            <NuxtLink
              to="/auth/forgot-password"
              class="text-sm text-primary hover:underline"
            >
              Forgot password?
            </NuxtLink>
          </div>
          <Input
            id="password"
            v-model="password"
            type="password"
            placeholder="Enter your password"
            autocomplete="current-password"
            @keyup.enter="handleLogin"
          />
        </div>

        <Button
          class="w-full"
          :disabled="isLoading"
          @click="handleLogin"
        >
          <span v-if="isLoading" class="flex items-center gap-2">
            <Icon name="mdi:loading" class="animate-spin" />
            Signing in...
          </span>
          <span v-else>Sign In</span>
        </Button>

        <div class="relative">
          <div class="absolute inset-0 flex items-center">
            <Separator />
          </div>
          <div class="relative flex justify-center text-xs uppercase">
            <span class="bg-card px-2 text-muted-foreground">
              Or continue with
            </span>
          </div>
        </div>

        <Button
          variant="outline"
          class="w-full"
          @click="handleGithubLogin"
        >
          <Icon name="mdi:github" class="mr-2 h-4 w-4" />
          GitHub
        </Button>

        <div class="text-center text-sm text-muted-foreground">
          Don't have an account?
          <NuxtLink to="/auth/register" class="text-primary hover:underline font-medium">
            Sign up
          </NuxtLink>
        </div>
      </CardContent>
    </Card>
  </div>
</template>
