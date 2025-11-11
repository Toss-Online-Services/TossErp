<script setup lang="ts">
import { Button } from '~/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Input } from '~/components/ui/input'
import { Label } from '~/components/ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '~/components/ui/select'
import { Checkbox } from '~/components/ui/checkbox'
import { Separator } from '~/components/ui/separator'

definePageMeta({
  layout: 'auth',
})

const formData = ref({
  businessName: '',
  ownerName: '',
  email: '',
  phone: '',
  password: '',
  confirmPassword: '',
  businessType: '',
  agreeToTerms: false,
})

const isLoading = ref(false)

const handleRegister = async () => {
  isLoading.value = true
  try {
    // TODO: Implement registration logic
    await navigateTo('/dashboard')
  } catch (error) {
    console.error('Registration failed:', error)
  } finally {
    isLoading.value = false
  }
}

const handleGithubSignup = () => {
  // TODO: Implement GitHub OAuth
  console.log('GitHub signup')
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center p-4 bg-muted/50">
    <Card class="w-full max-w-2xl">
      <CardHeader class="space-y-1">
        <div class="flex justify-center mb-4">
          <div class="w-12 h-12 bg-primary rounded-lg flex items-center justify-center">
            <span class="text-2xl font-bold text-primary-foreground">T</span>
          </div>
        </div>
        <CardTitle class="text-2xl text-center">Create an account</CardTitle>
        <CardDescription class="text-center">
          Enter your details below to create your TOSS ERP account
        </CardDescription>
      </CardHeader>
      <CardContent class="space-y-4">
        <!-- Quick OAuth Option -->
        <Button
          variant="outline"
          class="w-full"
          @click="handleGithubSignup"
        >
          <Icon name="mdi:github" class="mr-2 h-4 w-4" />
          Sign up with GitHub
        </Button>

        <div class="relative">
          <div class="absolute inset-0 flex items-center">
            <Separator />
          </div>
          <div class="relative flex justify-center text-xs uppercase">
            <span class="bg-card px-2 text-muted-foreground">
              Or continue with email
            </span>
          </div>
        </div>

        <!-- Registration Form -->
        <div class="grid gap-4 md:grid-cols-2">
          <div class="space-y-2">
            <Label for="businessName">Business Name *</Label>
            <Input
              id="businessName"
              v-model="formData.businessName"
              placeholder="e.g., Mama's Spaza Shop"
              required
            />
          </div>

          <div class="space-y-2">
            <Label for="ownerName">Owner Name *</Label>
            <Input
              id="ownerName"
              v-model="formData.ownerName"
              placeholder="e.g., John Doe"
              required
            />
          </div>

          <div class="space-y-2">
            <Label for="email">Email *</Label>
            <Input
              id="email"
              v-model="formData.email"
              type="email"
              placeholder="name@example.com"
              autocomplete="email"
              required
            />
          </div>

          <div class="space-y-2">
            <Label for="phone">Phone Number *</Label>
            <Input
              id="phone"
              v-model="formData.phone"
              type="tel"
              placeholder="+27 XX XXX XXXX"
              required
            />
          </div>

          <div class="space-y-2">
            <Label for="businessType">Business Type *</Label>
            <Select v-model="formData.businessType">
              <SelectTrigger>
                <SelectValue placeholder="Select business type" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="spaza">Spaza Shop</SelectItem>
                <SelectItem value="chisa-nyama">Chisa Nyama</SelectItem>
                <SelectItem value="tuck-shop">Tuck Shop</SelectItem>
                <SelectItem value="salon">Hair Salon</SelectItem>
                <SelectItem value="mechanic">Mechanic</SelectItem>
                <SelectItem value="tailor">Tailor</SelectItem>
                <SelectItem value="other">Other</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <div class="space-y-2">
            <Label for="password">Password *</Label>
            <Input
              id="password"
              v-model="formData.password"
              type="password"
              placeholder="Create a password"
              autocomplete="new-password"
              required
            />
          </div>

          <div class="space-y-2 md:col-span-2">
            <Label for="confirmPassword">Confirm Password *</Label>
            <Input
              id="confirmPassword"
              v-model="formData.confirmPassword"
              type="password"
              placeholder="Confirm your password"
              autocomplete="new-password"
              required
            />
          </div>
        </div>

        <!-- Terms and Conditions -->
        <div class="flex items-start space-x-2">
          <Checkbox
            id="terms"
            v-model:checked="formData.agreeToTerms"
          />
          <div class="grid gap-1.5 leading-none">
            <Label
              for="terms"
              class="text-sm font-normal cursor-pointer"
            >
              I agree to the
              <NuxtLink to="/terms" class="text-primary hover:underline">
                Terms of Service
              </NuxtLink>
              and
              <NuxtLink to="/privacy" class="text-primary hover:underline">
                Privacy Policy
              </NuxtLink>
            </Label>
          </div>
        </div>

        <!-- Submit Button -->
        <Button
          class="w-full"
          :disabled="isLoading || !formData.agreeToTerms"
          @click="handleRegister"
        >
          <span v-if="isLoading" class="flex items-center gap-2">
            <Icon name="mdi:loading" class="animate-spin" />
            Creating account...
          </span>
          <span v-else>Create Account</span>
        </Button>

        <!-- Login Link -->
        <div class="text-center text-sm text-muted-foreground">
          Already have an account?
          <NuxtLink to="/auth/login" class="text-primary hover:underline font-medium">
            Sign in
          </NuxtLink>
        </div>
      </CardContent>
    </Card>
  </div>
</template>
