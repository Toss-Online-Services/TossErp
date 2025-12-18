<template>
  <div class="container-fluid py-4">
    <div class="row">
      <div class="col-12 mb-4">
        <MDTypography variant="h3" font-weight="bold">
          Notifications
        </MDTypography>
        <MDTypography variant="body2" color="text">
          Manage and view your notifications
        </MDTypography>
      </div>
    </div>
    
    <div class="row">
      <div class="col-lg-8">
        <div class="card">
          <div class="card-header pb-0">
            <div class="d-flex justify-content-between align-items-center">
              <MDTypography variant="h6" font-weight="medium">
                Recent Notifications
              </MDTypography>
              <MDButton color="info" size="small" @click="markAllAsRead">
                Mark all as read
              </MDButton>
            </div>
          </div>
          <div class="card-body p-3">
            <div class="timeline timeline-one-side">
              <div 
                v-for="notification in notifications" 
                :key="notification.id"
                class="timeline-block mb-3"
              >
                <span class="timeline-step" :class="`bg-gradient-${notification.color}`">
                  <Icon :name="notification.icon" size="16" class="text-white" />
                </span>
                <div class="timeline-content">
                  <MDTypography variant="caption" color="text" class="text-xs">
                    {{ notification.time }}
                  </MDTypography>
                  <MDTypography variant="button" font-weight="medium" class="mt-0 mb-0">
                    {{ notification.title }}
                  </MDTypography>
                  <MDTypography variant="caption" color="text" class="mt-1 mb-0">
                    {{ notification.message }}
                  </MDTypography>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <div class="col-lg-4">
        <div class="card">
          <div class="card-header pb-0">
            <MDTypography variant="h6" font-weight="medium">
              Notification Settings
            </MDTypography>
          </div>
          <div class="card-body">
            <div class="form-check form-switch mb-3">
              <input 
                v-model="settings.email" 
                class="form-check-input" 
                type="checkbox" 
                id="emailNotif"
              >
              <label class="form-check-label" for="emailNotif">
                Email Notifications
              </label>
            </div>
            <div class="form-check form-switch mb-3">
              <input 
                v-model="settings.push" 
                class="form-check-input" 
                type="checkbox" 
                id="pushNotif"
              >
              <label class="form-check-label" for="pushNotif">
                Push Notifications
              </label>
            </div>
            <div class="form-check form-switch mb-3">
              <input 
                v-model="settings.desktop" 
                class="form-check-input" 
                type="checkbox" 
                id="desktopNotif"
              >
              <label class="form-check-label" for="desktopNotif">
                Desktop Notifications
              </label>
            </div>
            <div class="form-check form-switch mb-3">
              <input 
                v-model="settings.sms" 
                class="form-check-input" 
                type="checkbox" 
                id="smsNotif"
              >
              <label class="form-check-label" for="smsNotif">
                SMS Notifications
              </label>
            </div>
            
            <hr class="horizontal dark my-3">
            
            <MDTypography variant="button" font-weight="bold" class="mb-3">
              Notification Types
            </MDTypography>
            <div class="form-check mb-2">
              <input 
                v-model="types.comments" 
                class="form-check-input" 
                type="checkbox" 
                id="comments"
              >
              <label class="form-check-label" for="comments">
                Comments
              </label>
            </div>
            <div class="form-check mb-2">
              <input 
                v-model="types.updates" 
                class="form-check-input" 
                type="checkbox" 
                id="updates"
              >
              <label class="form-check-label" for="updates">
                Product Updates
              </label>
            </div>
            <div class="form-check mb-2">
              <input 
                v-model="types.promotions" 
                class="form-check-input" 
                type="checkbox" 
                id="promotions"
              >
              <label class="form-check-label" for="promotions">
                Promotions
              </label>
            </div>
            <div class="form-check mb-2">
              <input 
                v-model="types.messages" 
                class="form-check-input" 
                type="checkbox" 
                id="messages"
              >
              <label class="form-check-label" for="messages">
                Direct Messages
              </label>
            </div>
            
            <MDButton color="info" size="medium" full-width class="mt-4" @click="saveSettings">
              Save Settings
            </MDButton>
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

const notifications = ref([
  {
    id: 1,
    icon: 'mdi:bell',
    color: 'success',
    time: '2 minutes ago',
    title: 'New order received',
    message: 'Order #10421 has been placed by John Doe'
  },
  {
    id: 2,
    icon: 'mdi:account-plus',
    color: 'info',
    time: '15 minutes ago',
    title: 'New user registered',
    message: 'Alexa Liras just signed up for an account'
  },
  {
    id: 3,
    icon: 'mdi:email',
    color: 'warning',
    time: '1 hour ago',
    title: 'New message received',
    message: 'You have a new message from your support team'
  },
  {
    id: 4,
    icon: 'mdi:chart-line',
    color: 'primary',
    time: '2 hours ago',
    title: 'Sales milestone reached',
    message: 'Congratulations! You reached $10,000 in sales'
  },
  {
    id: 5,
    icon: 'mdi:alert-circle',
    color: 'error',
    time: '3 hours ago',
    title: 'Server warning',
    message: 'High CPU usage detected on production server'
  },
  {
    id: 6,
    icon: 'mdi:package-variant',
    color: 'dark',
    time: '5 hours ago',
    title: 'Order shipped',
    message: 'Order #10419 has been shipped and is on the way'
  }
])

const settings = ref({
  email: true,
  push: true,
  desktop: false,
  sms: false
})

const types = ref({
  comments: true,
  updates: true,
  promotions: false,
  messages: true
})

const markAllAsRead = () => {
  alert('All notifications marked as read')
}

const saveSettings = () => {
  console.log('Saving notification settings', { settings: settings.value, types: types.value })
  alert('Settings saved successfully!')
}
</script>

<style scoped>
.timeline {
  position: relative;
  padding-left: 0;
  list-style: none;
}

.timeline-one-side .timeline-block {
  position: relative;
  padding-left: 45px;
}

.timeline-step {
  position: absolute;
  left: 0;
  top: 4px;
  width: 30px;
  height: 30px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
}

.timeline-content {
  position: relative;
  padding: 10px 15px;
  border-radius: 0.5rem;
  background-color: #f8f9fa;
  margin-bottom: 10px;
}

.timeline-content::before {
  content: '';
  position: absolute;
  left: -8px;
  top: 15px;
  width: 0;
  height: 0;
  border-style: solid;
  border-width: 5px 8px 5px 0;
  border-color: transparent #f8f9fa transparent transparent;
}

.form-check-input {
  width: 2.5rem;
  height: 1.25rem;
  margin-right: 0.75rem;
}

.form-check-input[type="checkbox"] {
  width: 1.25rem;
  height: 1.25rem;
}

.form-check-label {
  font-size: 0.875rem;
  color: rgb(123, 128, 154);
}

.horizontal.dark {
  background-color: rgba(0, 0, 0, 0.1);
}
</style>
