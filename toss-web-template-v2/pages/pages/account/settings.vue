<template>
  <div class="container-fluid py-4">
    <div class="row">
      <div class="col-12">
        <div class="card">
          <div class="card-header pb-0">
            <MDTypography variant="h6" font-weight="medium">
              Profile Settings
            </MDTypography>
          </div>
          <div class="card-body">
            <MDTypography variant="h6" font-weight="medium" class="mb-3">
              Profile Information
            </MDTypography>
            <div class="row">
              <div class="col-md-6">
                <MDInput 
                  v-model="profile.firstName" 
                  label="First Name"
                  full-width 
                  class="mb-3"
                />
              </div>
              <div class="col-md-6">
                <MDInput 
                  v-model="profile.lastName" 
                  label="Last Name"
                  full-width 
                  class="mb-3"
                />
              </div>
            </div>
            <div class="row">
              <div class="col-md-6">
                <MDInput 
                  v-model="profile.email" 
                  type="email"
                  label="Email"
                  full-width 
                  class="mb-3"
                />
              </div>
              <div class="col-md-6">
                <MDInput 
                  v-model="profile.phone" 
                  type="tel"
                  label="Phone Number"
                  full-width 
                  class="mb-3"
                />
              </div>
            </div>
            <div class="row">
              <div class="col-12">
                <label class="form-label">About Me</label>
                <textarea 
                  v-model="profile.bio" 
                  class="form-control" 
                  rows="5" 
                  placeholder="Tell us about yourself"
                ></textarea>
              </div>
            </div>
            
            <hr class="horizontal dark my-4">
            
            <MDTypography variant="h6" font-weight="medium" class="mb-3">
              Change Password
            </MDTypography>
            <div class="row">
              <div class="col-md-6">
                <MDInput 
                  v-model="passwords.current" 
                  type="password"
                  label="Current Password"
                  full-width 
                  class="mb-3"
                />
              </div>
            </div>
            <div class="row">
              <div class="col-md-6">
                <MDInput 
                  v-model="passwords.new" 
                  type="password"
                  label="New Password"
                  full-width 
                  class="mb-3"
                />
              </div>
              <div class="col-md-6">
                <MDInput 
                  v-model="passwords.confirm" 
                  type="password"
                  label="Confirm New Password"
                  full-width 
                  class="mb-3"
                />
              </div>
            </div>
            
            <hr class="horizontal dark my-4">
            
            <MDTypography variant="h6" font-weight="medium" class="mb-3">
              Notification Settings
            </MDTypography>
            <div class="row">
              <div class="col-12">
                <div class="form-check form-switch mb-3">
                  <input 
                    v-model="notifications.email" 
                    class="form-check-input" 
                    type="checkbox" 
                    id="emailNotif"
                  >
                  <label class="form-check-label" for="emailNotif">
                    Email notifications
                  </label>
                </div>
                <div class="form-check form-switch mb-3">
                  <input 
                    v-model="notifications.push" 
                    class="form-check-input" 
                    type="checkbox" 
                    id="pushNotif"
                  >
                  <label class="form-check-label" for="pushNotif">
                    Push notifications
                  </label>
                </div>
                <div class="form-check form-switch mb-3">
                  <input 
                    v-model="notifications.newsletter" 
                    class="form-check-input" 
                    type="checkbox" 
                    id="newsletter"
                  >
                  <label class="form-check-label" for="newsletter">
                    Newsletter subscription
                  </label>
                </div>
              </div>
            </div>
            
            <div class="d-flex justify-content-end mt-4">
              <MDButton color="light" class="me-2">
                Cancel
              </MDButton>
              <MDButton color="info" @click="saveSettings">
                Save Changes
              </MDButton>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

definePageMeta({
  layout: 'default'
})

const profile = ref({
  firstName: 'John',
  lastName: 'Doe',
  email: 'john.doe@example.com',
  phone: '+1 234 567 890',
  bio: 'Product designer with a focus on creating intuitive and beautiful user experiences.'
})

const passwords = ref({
  current: '',
  new: '',
  confirm: ''
})

const notifications = ref({
  email: true,
  push: false,
  newsletter: true
})

const saveSettings = () => {
  console.log('Saving settings', { profile: profile.value, notifications: notifications.value })
  alert('Settings saved successfully!')
}
</script>

<style scoped>
.form-control {
  display: block;
  width: 100%;
  padding: 0.5rem 0.75rem;
  font-size: 0.875rem;
  font-weight: 400;
  line-height: 1.5;
  color: #495057;
  background-color: #fff;
  background-clip: padding-box;
  border: 1px solid #d2d6da;
  border-radius: 0.5rem;
  transition: border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out;
}

.form-control:focus {
  color: #495057;
  background-color: #fff;
  border-color: #5e72e4;
  outline: 0;
  box-shadow: 0 0 0 2px rgba(94, 114, 228, 0.1);
}

.form-label {
  display: block;
  margin-bottom: 0.5rem;
  font-size: 0.875rem;
  font-weight: 600;
  color: #344767;
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

.horizontal.dark {
  background-color: rgba(0, 0, 0, 0.1);
}
</style>
