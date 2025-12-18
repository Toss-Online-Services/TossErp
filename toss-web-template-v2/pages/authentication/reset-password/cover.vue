<template>
  <div class="container position-sticky z-index-sticky top-0">
    <div class="row">
      <div class="col-12">
        <!-- Navbar placeholder -->
      </div>
    </div>
  </div>
  <main class="main-content mt-0">
    <div class="page-header align-items-start min-vh-100"
         style="background-image: url('https://images.unsplash.com/photo-1497366811353-6870744d04b2?auto=format&fit=crop&w=1920');">
      <span class="mask bg-gradient-dark opacity-6"></span>
      <div class="container my-auto">
        <div class="row">
          <div class="col-lg-4 col-md-8 col-12 mx-auto">
            <div class="card z-index-0 fadeIn3 fadeInBottom">
              <div class="card-header p-0 position-relative mt-n4 mx-3 z-index-2">
                <div class="bg-gradient-primary shadow-primary border-radius-lg py-3 pe-1">
                  <MDTypography variant="h4" color="white" class="text-center mt-2 mb-0 font-weight-bold">
                    Reset Password
                  </MDTypography>
                </div>
              </div>
              <div class="card-body">
                <MDTypography variant="body2" color="text" class="mb-4">
                  You will receive an e-mail in maximum 60 seconds
                </MDTypography>
                <form role="form" class="text-start" @submit.prevent="handleResetPassword">
                  <div class="mb-3">
                    <MDInput
                      v-model="email"
                      label="Email"
                      placeholder="Enter your email"
                      type="email"
                      required
                    />
                  </div>
                  <div class="text-center">
                    <MDButton type="submit" color="primary" variant="gradient" class="w-100 my-4 mb-2">
                      Reset Password
                    </MDButton>
                  </div>
                  <MDTypography variant="button" color="text" class="text-center">
                    Remember your password?
                    <NuxtLink to="/authentication/sign-in/cover" class="text-primary font-weight-bold">
                      Sign in
                    </NuxtLink>
                  </MDTypography>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </main>
</template>

<script setup lang="ts">
import { ref } from 'vue'

definePageMeta({
  layout: false
})

const email = ref('')

const handleResetPassword = () => {
  alert('Password reset link sent to your email!')
}
</script>

<style scoped>
.page-header {
  padding: 0;
  position: relative;
  overflow: hidden;
  display: flex;
  align-items: center;
  background-size: cover;
  background-position: center;
}

.min-vh-100 {
  min-height: 100vh !important;
}

.align-items-start {
  align-items: flex-start !important;
}

.mask {
  position: absolute;
  background-size: cover;
  background-position: center;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
}

.bg-gradient-dark {
  background-image: linear-gradient(195deg, #42424a 0%, #191919 100%);
}

.opacity-6 {
  opacity: 0.6;
}

.my-auto {
  margin-top: auto !important;
  margin-bottom: auto !important;
}

.z-index-0 {
  z-index: 0;
}

.z-index-2 {
  z-index: 2;
}

.z-index-sticky {
  z-index: 1020;
}

.position-relative {
  position: relative;
}

.mt-n4 {
  margin-top: -1.5rem;
}

.fadeIn3 {
  animation: fadeIn 0.7s ease-in;
}

.fadeInBottom {
  animation: fadeInBottom 0.7s ease-in;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

@keyframes fadeInBottom {
  from {
    opacity: 0;
    transform: translateY(50px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.bg-gradient-primary {
  background-image: linear-gradient(195deg, #5e72e4 0%, #825ee4 100%);
}

.shadow-primary {
  box-shadow: 0 4px 20px 0 rgba(0, 0, 0, 0.14), 0 7px 10px -5px rgba(94, 114, 228, 0.4);
}

.border-radius-lg {
  border-radius: 0.75rem;
}

.text-center {
  text-align: center;
}
</style>
