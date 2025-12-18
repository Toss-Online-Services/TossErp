<template>
  <div class="container-fluid py-4">
    <div class="row">
      <div class="col-12 mb-4">
        <MDTypography variant="h3" font-weight="bold">
          Widgets
        </MDTypography>
        <MDTypography variant="body2" color="text">
          Browse our collection of ready-to-use widgets
        </MDTypography>
      </div>
    </div>
    
    <!-- Alerts Section -->
    <div class="row">
      <div class="col-12">
        <MDTypography variant="h5" font-weight="bold" class="mb-3">
          Alerts
        </MDTypography>
      </div>
      <div class="col-md-6 mb-4">
        <MDAlert 
          v-model="alerts.success"
          color="success"
          variant="gradient"
          icon="mdi:check-circle"
          title="Success!"
          dismissible
        >
          Your action was completed successfully.
        </MDAlert>
      </div>
      <div class="col-md-6 mb-4">
        <MDAlert 
          v-model="alerts.info"
          color="info"
          variant="gradient"
          icon="mdi:information"
          title="Information"
          dismissible
        >
          This is an informational alert message.
        </MDAlert>
      </div>
      <div class="col-md-6 mb-4">
        <MDAlert 
          v-model="alerts.warning"
          color="warning"
          variant="gradient"
          icon="mdi:alert"
          title="Warning"
          dismissible
        >
          Please review this warning message carefully.
        </MDAlert>
      </div>
      <div class="col-md-6 mb-4">
        <MDAlert 
          v-model="alerts.error"
          color="error"
          variant="gradient"
          icon="mdi:alert-circle"
          title="Error"
          dismissible
        >
          An error occurred. Please try again.
        </MDAlert>
      </div>
    </div>
    
    <!-- Progress Bars Section -->
    <div class="row mt-5">
      <div class="col-12">
        <MDTypography variant="h5" font-weight="bold" class="mb-3">
          Progress Bars
        </MDTypography>
      </div>
      <div class="col-md-6 mb-4">
        <div class="card">
          <div class="card-body">
            <MDTypography variant="button" font-weight="bold" class="mb-2">
              Default Progress
            </MDTypography>
            <MDProgress :value="60" color="info" variant="gradient" class="mb-3" />
            <MDProgress :value="40" color="success" variant="gradient" class="mb-3" />
            <MDProgress :value="80" color="warning" variant="gradient" class="mb-3" />
            <MDProgress :value="30" color="error" variant="gradient" />
          </div>
        </div>
      </div>
      <div class="col-md-6 mb-4">
        <div class="card">
          <div class="card-body">
            <MDTypography variant="button" font-weight="bold" class="mb-2">
              With Labels
            </MDTypography>
            <MDProgress :value="75" color="info" variant="gradient" label="75%" height="1.5rem" class="mb-3" />
            <MDProgress :value="55" color="success" variant="gradient" label="55%" height="1.5rem" class="mb-3" />
            <MDProgress :value="90" color="warning" variant="gradient" label="90%" height="1.5rem" class="mb-3" />
            <MDProgress :value="45" color="error" variant="gradient" label="45%" height="1.5rem" />
          </div>
        </div>
      </div>
    </div>
    
    <!-- Pagination Section -->
    <div class="row mt-5">
      <div class="col-12">
        <MDTypography variant="h5" font-weight="bold" class="mb-3">
          Pagination
        </MDTypography>
      </div>
      <div class="col-md-6 mb-4">
        <div class="card">
          <div class="card-body">
            <MDTypography variant="button" font-weight="bold" class="mb-3">
              Default Pagination
            </MDTypography>
            <MDPagination 
              v-model:current-page="currentPage1" 
              :total-pages="10" 
              color="info"
              variant="gradient"
            />
          </div>
        </div>
      </div>
      <div class="col-md-6 mb-4">
        <div class="card">
          <div class="card-body">
            <MDTypography variant="button" font-weight="bold" class="mb-3">
              Large Pagination
            </MDTypography>
            <MDPagination 
              v-model:current-page="currentPage2" 
              :total-pages="8" 
              color="success"
              size="large"
              variant="gradient"
            />
          </div>
        </div>
      </div>
    </div>
    
    <!-- Badges Section -->
    <div class="row mt-5">
      <div class="col-12">
        <MDTypography variant="h5" font-weight="bold" class="mb-3">
          Badges
        </MDTypography>
      </div>
      <div class="col-12 mb-4">
        <div class="card">
          <div class="card-body">
            <MDTypography variant="button" font-weight="bold" class="mb-3 d-block">
              Gradient Badges
            </MDTypography>
            <div class="d-flex flex-wrap gap-2">
              <MDBadge color="primary" variant="gradient">Primary</MDBadge>
              <MDBadge color="secondary" variant="gradient">Secondary</MDBadge>
              <MDBadge color="info" variant="gradient">Info</MDBadge>
              <MDBadge color="success" variant="gradient">Success</MDBadge>
              <MDBadge color="warning" variant="gradient">Warning</MDBadge>
              <MDBadge color="error" variant="gradient">Error</MDBadge>
              <MDBadge color="light" variant="gradient">Light</MDBadge>
              <MDBadge color="dark" variant="gradient">Dark</MDBadge>
            </div>
            
            <MDTypography variant="button" font-weight="bold" class="mb-3 mt-4 d-block">
              Badge Sizes
            </MDTypography>
            <div class="d-flex flex-wrap gap-2 align-items-center">
              <MDBadge color="info" variant="gradient" size="xs">Extra Small</MDBadge>
              <MDBadge color="info" variant="gradient" size="sm">Small</MDBadge>
              <MDBadge color="info" variant="gradient" size="md">Medium</MDBadge>
              <MDBadge color="info" variant="gradient" size="lg">Large</MDBadge>
            </div>
            
            <MDTypography variant="button" font-weight="bold" class="mb-3 mt-4 d-block">
              Circular Badges
            </MDTypography>
            <div class="d-flex flex-wrap gap-2">
              <MDBadge color="info" variant="gradient" circular>99+</MDBadge>
              <MDBadge color="success" variant="gradient" circular>5</MDBadge>
              <MDBadge color="warning" variant="gradient" circular>12</MDBadge>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Avatars Section -->
    <div class="row mt-5">
      <div class="col-12">
        <MDTypography variant="h5" font-weight="bold" class="mb-3">
          Avatars
        </MDTypography>
      </div>
      <div class="col-12 mb-4">
        <div class="card">
          <div class="card-body">
            <MDTypography variant="button" font-weight="bold" class="mb-3 d-block">
              Avatar Sizes
            </MDTypography>
            <div class="d-flex flex-wrap gap-3 align-items-center">
              <MDAvatar 
                src="https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-1.jpg"
                alt="user"
                size="xs"
              />
              <MDAvatar 
                src="https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-2.jpg"
                alt="user"
                size="sm"
              />
              <MDAvatar 
                src="https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-3.jpg"
                alt="user"
                size="md"
              />
              <MDAvatar 
                src="https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-4.jpg"
                alt="user"
                size="lg"
              />
              <MDAvatar 
                src="https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-5.jpg"
                alt="user"
                size="xl"
              />
              <MDAvatar 
                src="https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-6.jpg"
                alt="user"
                size="xxl"
              />
            </div>
            
            <MDTypography variant="button" font-weight="bold" class="mb-3 mt-4 d-block">
              Avatar Group
            </MDTypography>
            <div class="avatar-group">
              <MDAvatar 
                v-for="i in 5" 
                :key="i"
                :src="`https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-${i}.jpg`"
                alt="user"
                size="md"
              />
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Buttons Section -->
    <div class="row mt-5">
      <div class="col-12">
        <MDTypography variant="h5" font-weight="bold" class="mb-3">
          Buttons
        </MDTypography>
      </div>
      <div class="col-md-6 mb-4">
        <div class="card">
          <div class="card-body">
            <MDTypography variant="button" font-weight="bold" class="mb-3 d-block">
              Gradient Buttons
            </MDTypography>
            <div class="d-flex flex-wrap gap-2">
              <MDButton color="primary" variant="gradient">Primary</MDButton>
              <MDButton color="info" variant="gradient">Info</MDButton>
              <MDButton color="success" variant="gradient">Success</MDButton>
              <MDButton color="warning" variant="gradient">Warning</MDButton>
              <MDButton color="error" variant="gradient">Error</MDButton>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-6 mb-4">
        <div class="card">
          <div class="card-body">
            <MDTypography variant="button" font-weight="bold" class="mb-3 d-block">
              Button Sizes
            </MDTypography>
            <div class="d-flex flex-wrap gap-2 align-items-center">
              <MDButton color="info" size="small">Small</MDButton>
              <MDButton color="info" size="medium">Medium</MDButton>
              <MDButton color="info" size="large">Large</MDButton>
            </div>
            
            <MDTypography variant="button" font-weight="bold" class="mb-3 mt-4 d-block">
              Icon Buttons
            </MDTypography>
            <div class="d-flex flex-wrap gap-2">
              <MDButton color="info" icon-only circular>
                <Icon name="mdi:heart" size="20" />
              </MDButton>
              <MDButton color="success" icon-only circular>
                <Icon name="mdi:check" size="20" />
              </MDButton>
              <MDButton color="warning" icon-only circular>
                <Icon name="mdi:star" size="20" />
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

const alerts = ref({
  success: true,
  info: true,
  warning: true,
  error: true
})

const currentPage1 = ref(1)
const currentPage2 = ref(1)
</script>

<style scoped>
.gap-2 {
  gap: 0.5rem;
}

.gap-3 {
  gap: 1rem;
}

.d-block {
  display: block;
}

.avatar-group {
  display: flex;
  gap: -0.75rem;
}

.avatar-group > * {
  border: 2px solid white;
}
</style>
