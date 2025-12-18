<template>
  <div class="container-fluid">
    <div class="row">
      <div class="col-lg-5 col-md-7 mx-auto">
        <div class="card z-index-0 mt-8">
          <div class="card-header text-center pt-4 pb-1">
            <MDTypography variant="h4" font-weight="bold">
              Register
            </MDTypography>
          </div>
          <div class="row px-xl-5 px-sm-4 px-3 justify-content-center">
            <div class="col-3 ms-auto px-1">
              <MDButton 
                color="white"
                circular
                icon-only
                class="w-100"
              >
                <Icon name="mdi:facebook" size="24" class="text-primary" />
              </MDButton>
            </div>
            <div class="col-3 px-1">
              <MDButton 
                color="white"
                circular
                icon-only
                class="w-100"
              >
                <Icon name="mdi:apple" size="24" class="text-dark" />
              </MDButton>
            </div>
            <div class="col-3 me-auto px-1">
              <MDButton 
                color="white"
                circular
                icon-only
                class="w-100"
              >
                <Icon name="mdi:google" size="24" class="text-danger" />
              </MDButton>
            </div>
            <div class="mt-2 position-relative text-center">
              <MDTypography variant="button" color="text" font-weight="regular">
                or
              </MDTypography>
            </div>
          </div>
          <div class="card-body">
            <form role="form">
              <MDInput 
                v-model="name" 
                type="text" 
                placeholder="Name" 
                full-width 
                class="mb-3"
              />
              <MDInput 
                v-model="email" 
                type="email" 
                placeholder="Email" 
                full-width 
                class="mb-3"
              />
              <MDInput 
                v-model="password" 
                type="password" 
                placeholder="Password" 
                full-width 
                class="mb-3"
              />
              <div class="form-check">
                <input 
                  v-model="termsAgreed" 
                  class="form-check-input" 
                  type="checkbox" 
                  id="terms"
                >
                <label class="form-check-label" for="terms">
                  I agree to the <a href="#" class="text-info font-weight-bolder">Terms and Conditions</a>
                </label>
              </div>
              <div class="text-center">
                <MDButton 
                  color="info" 
                  size="large" 
                  full-width 
                  class="mt-4"
                  @click.prevent="handleSignUp"
                >
                  Sign up
                </MDButton>
              </div>
            </form>
          </div>
          <div class="card-footer text-center pt-0 px-lg-2 px-1">
            <MDTypography variant="button" color="text" class="mt-4">
              Already have an account?
              <NuxtLink to="/authentication/sign-in/cover" class="text-info font-weight-bold">
                Sign in
              </NuxtLink>
            </MDTypography>
          </div>
        </div>
      </div>
      <div class="col-md-6">
        <div class="oblique position-absolute top-0 h-100 d-md-block d-none me-n8">
          <div class="oblique-image bg-gradient-info opacity-85 position-absolute h-100" style="background-image: url('https://images.unsplash.com/photo-1497215728101-856f4ea42174?auto=format&fit=crop&w=1920'); background-size: cover;"></div>
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

const name = ref('')
const email = ref('')
const password = ref('')
const termsAgreed = ref(false)

const handleSignUp = () => {
  if (!termsAgreed.value) {
    alert('Please agree to the Terms and Conditions')
    return
  }
  console.log('Sign up with', { name: name.value, email: email.value, password: password.value })
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
  width: 1.25rem;
  height: 1.25rem;
  margin-right: 0.5rem;
}

.form-check-label {
  font-size: 0.875rem;
  color: rgb(123, 128, 154);
}

.z-index-0 {
  z-index: 0;
}

.w-100 {
  width: 100%;
}
</style>
