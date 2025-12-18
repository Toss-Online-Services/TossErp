<template>
  <div class="container-fluid px-0">
    <div class="row g-0">
      <div class="col-lg-4 d-flex flex-column mx-auto">
        <div class="card card-plain mt-8">
          <div class="card-header pb-0 text-left bg-transparent">
            <MDTypography variant="h3" font-weight="bold" color="dark">
              Welcome back
            </MDTypography>
            <MDTypography variant="body2" color="text">
              Enter your email and password to sign in
            </MDTypography>
          </div>
          <div class="card-body pb-3">
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
                  id="rememberMeCover"
                >
                <label class="form-check-label" for="rememberMeCover">
                  Remember me
                </label>
              </div>
              <div class="text-center">
                <MDButton 
                  color="info" 
                  size="large" 
                  full-width 
                  class="mt-4 mb-0"
                  @click.prevent="handleSignIn"
                >
                  Sign in
                </MDButton>
              </div>
            </form>
          </div>
          <div class="card-footer text-center pt-0 px-lg-2 px-1">
            <MDTypography variant="button" color="text">
              Don't have an account?
              <NuxtLink to="/authentication/sign-up/cover" class="text-info font-weight-bold">
                Sign up
              </NuxtLink>
            </MDTypography>
          </div>
        </div>
      </div>
      <div class="col-lg-8 d-none d-lg-flex">
        <div class="position-relative h-100 m-3 px-7 border-radius-lg d-flex flex-column justify-content-center" style="background-image: url('https://images.unsplash.com/photo-1486406146926-c627a92ad1ab?auto=format&fit=crop&w=1920'); background-size: cover;">
          <div class="position-absolute top-0 start-0 w-100 h-100 bg-gradient-primary opacity-6 border-radius-lg"></div>
          <MDTypography variant="h4" color="white" class="position-relative" font-weight="bold">
            "Attention is the new currency"
          </MDTypography>
          <MDTypography variant="body1" color="white" class="position-relative opacity-8 mt-3">
            The more effortless the writing looks, the more effort the writer actually put into the process.
          </MDTypography>
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
.container-fluid {
  min-height: 100vh;
}

.row {
  min-height: 100vh;
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

.border-radius-lg {
  border-radius: 1rem;
}

.opacity-6 {
  opacity: 0.6;
}

.opacity-8 {
  opacity: 0.8;
}
</style>
