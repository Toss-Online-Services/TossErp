<template>
  <div class="container-fluid">
    <div class="row min-vh-100">
      <div class="col-xl-5 col-lg-6 col-md-7 d-flex flex-column mx-auto">
        <div class="card card-plain mt-8">
          <div class="card-header pb-0 text-left bg-transparent">
            <MDTypography variant="h3" font-weight="bold" color="dark">
              Sign In
            </MDTypography>
            <MDTypography variant="body2" color="text">
              Enter your email and password to sign in
            </MDTypography>
          </div>
          <div class="card-body">
            <form role="form">
              <label>Email</label>
              <MDInput 
                v-model="email" 
                type="email" 
                placeholder="Email" 
                full-width 
                class="mb-3"
              />
              <label>Password</label>
              <MDInput 
                v-model="password" 
                type="password" 
                placeholder="Password" 
                full-width 
                class="mb-3"
              />
              <div class="form-check form-switch">
                <input 
                  v-model="rememberMe" 
                  class="form-check-input" 
                  type="checkbox" 
                  id="rememberMe"
                >
                <label class="form-check-label" for="rememberMe">
                  Remember me
                </label>
              </div>
              <div class="text-center">
                <MDButton 
                  color="info" 
                  size="large" 
                  full-width 
                  class="mt-4"
                  @click.prevent="handleSignIn"
                >
                  Sign in
                </MDButton>
              </div>
            </form>
          </div>
          <div class="card-footer text-center pt-0 px-lg-2 px-1">
            <MDTypography variant="button" color="text" class="mt-4">
              Don't have an account?
              <NuxtLink to="/authentication/sign-up/cover" class="text-info font-weight-bold">
                Sign up
              </NuxtLink>
            </MDTypography>
          </div>
        </div>
      </div>
      <div class="col-md-6">
        <div class="oblique position-absolute top-0 h-100 d-md-block d-none me-n8">
          <div class="oblique-image bg-gradient-primary opacity-85 position-absolute h-100" style="background-image: url('https://images.unsplash.com/photo-1567919317717-ad8e4dcafe6f?auto=format&fit=crop&w=1920'); background-size: cover;"></div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

definePageMeta({
  layout: false
})

const email = ref('')
const password = ref('')
const rememberMe = ref(false)

const handleSignIn = () => {
  console.log('Sign in with', { email: email.value, password: password.value })
  navigateTo('/dashboards/analytics')
}
</script>

<style scoped>
.min-vh-100 {
  min-height: 100vh;
}

.oblique {
  position: absolute;
  transform: skewX(-10deg);
  border-radius: 0 0 0 1.5rem;
  overflow: hidden;
}

.oblique-image {
  transform: skewX(10deg);
  margin-left: -30%;
}

.form-check-input {
  width: 2.5rem;
  height: 1.25rem;
  margin-right: 0.75rem;
}

.form-check-label {
  font-size: 0.875rem;
  color: rgb(123, 128, 154);
}
</style>
