<template>
  <div class="container position-sticky z-index-sticky top-0">
    <div class="row">
      <div class="col-12">
        <!-- Navbar placeholder -->
      </div>
    </div>
  </div>
  <main class="main-content mt-0">
    <section>
      <div class="page-header min-vh-100">
        <div class="container">
          <div class="row">
            <div class="col-xl-4 col-lg-5 col-md-7 d-flex flex-column mx-lg-0 mx-auto">
              <div class="card card-plain">
                <div class="card-header pb-0 text-start">
                  <MDTypography variant="h4" font-weight="bold">
                    Sign In
                  </MDTypography>
                  <MDTypography variant="body2" color="text">
                    Enter your email and password to sign in
                  </MDTypography>
                </div>
                <div class="card-body">
                  <form role="form" @submit.prevent="handleSignIn">
                    <div class="mb-3">
                      <MDInput
                        v-model="email"
                        label="Email"
                        placeholder="Enter your email"
                        type="email"
                        required
                      />
                    </div>
                    <div class="mb-3">
                      <MDInput
                        v-model="password"
                        label="Password"
                        placeholder="Enter your password"
                        type="password"
                        required
                      />
                    </div>
                    <div class="form-check form-switch mb-3">
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
                      <MDButton type="submit" color="primary" variant="gradient" class="w-100 my-4 mb-2">
                        Sign in
                      </MDButton>
                    </div>
                    <div class="mb-2 text-sm text-center">
                      <a href="/authentication/reset-password/cover" class="text-primary font-weight-bold">
                        Forgot password?
                      </a>
                    </div>
                  </form>
                </div>
                <div class="card-footer text-center pt-0 px-lg-2 px-1">
                  <MDTypography variant="button" color="text">
                    Don't have an account?
                    <NuxtLink to="/authentication/sign-up/cover" class="text-primary font-weight-bold">
                      Sign up
                    </NuxtLink>
                  </MDTypography>
                </div>
              </div>
            </div>
            <div class="col-6 d-lg-flex d-none h-100 my-auto pe-0 position-absolute top-0 end-0 text-center justify-content-center flex-column">
              <div class="position-relative bg-gradient-primary h-100 m-3 px-7 border-radius-lg d-flex flex-column justify-content-center overflow-hidden"
                   style="background-image: url('https://images.unsplash.com/photo-1497366216548-37526070297c?auto=format&fit=crop&w=1200'); background-size: cover;">
                <span class="mask bg-gradient-primary opacity-6"></span>
                <MDTypography variant="h4" color="white" font-weight="bold" class="position-relative">
                  "Attention is the new currency"
                </MDTypography>
                <MDTypography variant="body2" color="white" class="position-relative">
                  The more effortless the writing looks, the more effort the writer actually put into the process.
                </MDTypography>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  </main>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { navigateTo } from 'nuxt/app'

definePageMeta({
  layout: false
})

const email = ref('')
const password = ref('')
const rememberMe = ref(false)

const handleSignIn = () => {
  // Handle sign in logic here
  navigateTo('/dashboards/analytics')
}
</script>

<style scoped>
.page-header {
  padding: 0;
}

.min-vh-100 {
  min-height: 100vh !important;
}

.z-index-sticky {
  z-index: 1020;
}

.form-check-input {
  width: 2.5rem;
  height: 1.25rem;
  cursor: pointer;
}

.form-check-input:checked {
  background-color: #5e72e4;
  border-color: #5e72e4;
}

.text-sm {
  font-size: 0.875rem;
}

.card-plain {
  background: transparent;
  box-shadow: none;
}

.mask {
  position: absolute;
  background-size: cover;
  background-position: center;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  opacity: 0.6;
}

.position-relative {
  position: relative;
}

.position-absolute {
  position: absolute;
}

.top-0 {
  top: 0;
}

.end-0 {
  right: 0;
}

.overflow-hidden {
  overflow: hidden;
}

.border-radius-lg {
  border-radius: 0.75rem;
}

.bg-gradient-primary {
  background-image: linear-gradient(195deg, #5e72e4 0%, #825ee4 100%);
}

.opacity-6 {
  opacity: 0.6;
}

.px-7 {
  padding-left: 7rem;
  padding-right: 7rem;
}

.pe-0 {
  padding-right: 0;
}

.m-3 {
  margin: 1rem;
}

.my-auto {
  margin-top: auto;
  margin-bottom: auto;
}
</style>
