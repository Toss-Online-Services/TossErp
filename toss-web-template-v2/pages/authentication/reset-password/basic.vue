<template>
  <main class="main-content mt-0">
    <section>
      <div class="page-header min-vh-100">
        <div class="container">
          <div class="row">
            <div class="col-xl-4 col-lg-5 col-md-6 d-flex flex-column mx-auto">
              <div class="card card-plain mt-8">
                <div class="card-header pb-0 text-start">
                  <MDTypography variant="h3" font-weight="bold">
                    Reset Password
                  </MDTypography>
                  <MDTypography variant="body2" color="text" class="mb-0">
                    You will receive an e-mail in maximum 60 seconds
                  </MDTypography>
                </div>
                <div class="card-body">
                  <form role="form" @submit.prevent="handleResetPassword">
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
                      <MDButton type="submit" color="primary" variant="gradient" class="w-100 mt-4 mb-0">
                        Reset Password
                      </MDButton>
                    </div>
                  </form>
                </div>
                <div class="card-footer text-center pt-0 px-lg-2 px-1">
                  <MDTypography variant="button" color="text">
                    Remember your password?
                    <NuxtLink to="/authentication/sign-in/basic" class="text-primary font-weight-bold">
                      Sign in
                    </NuxtLink>
                  </MDTypography>
                </div>
              </div>
            </div>
            <div class="col-md-6">
              <div class="oblique position-absolute top-0 h-100 d-md-block d-none me-n8">
                <div class="oblique-image bg-cover position-absolute fixed-top ms-auto h-100 z-index-0 ms-n6"
                     style="background-image: url('https://images.unsplash.com/photo-1557804506-669a67965ba0?auto=format&fit=crop&w=1200')">
                </div>
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
}

.min-vh-100 {
  min-height: 100vh !important;
}

.card-plain {
  background: transparent;
  box-shadow: none;
}

.mt-8 {
  margin-top: 5rem;
}

.oblique {
  transform: skewX(-10deg);
  overflow: hidden;
}

.oblique-image {
  transform: skewX(10deg);
  background-size: cover;
  background-position: center;
}

.position-absolute {
  position: absolute;
}

.position-relative {
  position: relative;
}

.top-0 {
  top: 0;
}

.fixed-top {
  top: 0;
}

.h-100 {
  height: 100%;
}

.z-index-0 {
  z-index: 0;
}

.ms-auto {
  margin-left: auto;
}

.ms-n6 {
  margin-left: -3rem;
}

.me-n8 {
  margin-right: -4rem;
}

.bg-cover {
  background-size: cover;
}

@media (max-width: 768px) {
  .d-md-block {
    display: none !important;
  }
}
</style>
