<template>
  <div class="space-y-6">
    <div>
      <h1 class="text-3xl font-bold">Settings</h1>
      <p class="text-muted-foreground">Manage your business settings and preferences</p>
    </div>
    
    <div class="grid gap-6 lg:grid-cols-3">
      <!-- Settings Navigation -->
      <Card class="lg:col-span-1">
        <CardHeader>
          <CardTitle>Settings</CardTitle>
        </CardHeader>
        <CardContent>
          <nav class="space-y-1">
            <button
              v-for="section in settingsSections"
              :key="section.id"
              @click="activeSection = section.id"
              class="w-full text-left px-4 py-2 rounded-lg transition-colors"
              :class="activeSection === section.id ? 'bg-accent text-accent-foreground' : 'hover:bg-accent'"
            >
              <div class="flex items-center space-x-2">
                <Icon :name="section.icon" class="w-4 h-4" />
                <span>{{ section.label }}</span>
              </div>
            </button>
          </nav>
        </CardContent>
      </Card>
      
      <!-- Settings Content -->
      <Card class="lg:col-span-2">
        <CardHeader>
          <CardTitle>{{ activeSectionLabel }}</CardTitle>
        </CardHeader>
        <CardContent>
          <!-- Business Information -->
          <div v-if="activeSection === 'business'" class="space-y-4">
            <div class="space-y-2">
              <Label>Business Name</Label>
              <Input v-model="businessInfo.name" />
            </div>
            <div class="space-y-2">
              <Label>Business Type</Label>
              <select v-model="businessInfo.type" class="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-sm">
                <option>Spaza Shop</option>
                <option>Tshisanyama</option>
                <option>Bakery</option>
                <option>Other</option>
              </select>
            </div>
            <div class="space-y-2">
              <Label>Address</Label>
              <Input v-model="businessInfo.address" />
            </div>
            <div class="grid gap-4 md:grid-cols-2">
              <div class="space-y-2">
                <Label>Phone</Label>
                <Input v-model="businessInfo.phone" />
              </div>
              <div class="space-y-2">
                <Label>Email</Label>
                <Input v-model="businessInfo.email" type="email" />
              </div>
            </div>
            <Button>Save Changes</Button>
          </div>
          
          <!-- Account Settings -->
          <div v-if="activeSection === 'account'" class="space-y-4">
            <div class="space-y-2">
              <Label>Full Name</Label>
              <Input v-model="accountInfo.name" />
            </div>
            <div class="space-y-2">
              <Label>Email</Label>
              <Input v-model="accountInfo.email" type="email" />
            </div>
            <div class="space-y-2">
              <Label>Current Password</Label>
              <Input v-model="accountInfo.currentPassword" type="password" />
            </div>
            <div class="space-y-2">
              <Label>New Password</Label>
              <Input v-model="accountInfo.newPassword" type="password" />
            </div>
            <Button>Update Account</Button>
          </div>
          
          <!-- Notifications -->
          <div v-if="activeSection === 'notifications'" class="space-y-4">
            <div class="flex items-center justify-between">
              <div>
                <Label>Stock Alerts</Label>
                <p class="text-sm text-muted-foreground">Get notified when stock is running low</p>
              </div>
              <input type="checkbox" v-model="notifications.stockAlerts" class="rounded" />
            </div>
            <div class="flex items-center justify-between">
              <div>
                <Label>Order Updates</Label>
                <p class="text-sm text-muted-foreground">Receive updates on purchase orders</p>
              </div>
              <input type="checkbox" v-model="notifications.orderUpdates" class="rounded" />
            </div>
            <div class="flex items-center justify-between">
              <div>
                <Label>AI Insights</Label>
                <p class="text-sm text-muted-foreground">Daily business insights from AI Copilot</p>
              </div>
              <input type="checkbox" v-model="notifications.aiInsights" class="rounded" />
            </div>
            <Button>Save Preferences</Button>
          </div>
          
          <!-- Network Settings -->
          <div v-if="activeSection === 'network'" class="space-y-4">
            <div class="flex items-center justify-between">
              <div>
                <Label>Join Network</Label>
                <p class="text-sm text-muted-foreground">Participate in group buying and shared logistics</p>
              </div>
              <input type="checkbox" v-model="networkSettings.enabled" class="rounded" />
            </div>
            <div class="space-y-2">
              <Label>Share Location</Label>
              <p class="text-sm text-muted-foreground">Allow nearby businesses to find you</p>
              <input type="checkbox" v-model="networkSettings.shareLocation" class="rounded" />
            </div>
            <div class="space-y-2">
              <Label>Auto-Join Group Orders</Label>
              <p class="text-sm text-muted-foreground">Automatically join group orders for products you regularly buy</p>
              <input type="checkbox" v-model="networkSettings.autoJoin" class="rounded" />
            </div>
            <Button>Save Network Settings</Button>
          </div>
        </CardContent>
      </Card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'

definePageMeta({
  layout: 'admin'
})

const activeSection = ref('business')

const settingsSections = [
  { id: 'business', label: 'Business Information', icon: 'lucide:store' },
  { id: 'account', label: 'Account Settings', icon: 'lucide:user' },
  { id: 'notifications', label: 'Notifications', icon: 'lucide:bell' },
  { id: 'network', label: 'Network Settings', icon: 'lucide:network' }
]

const activeSectionLabel = computed(() => {
  return settingsSections.find(s => s.id === activeSection.value)?.label || 'Settings'
})

const businessInfo = ref({
  name: 'Mama Dlamini\'s Spaza',
  type: 'Spaza Shop',
  address: '123 Main Street, Township',
  phone: '+27 12 345 6789',
  email: 'business@example.com'
})

const accountInfo = ref({
  name: 'Business Owner',
  email: 'owner@example.com',
  currentPassword: '',
  newPassword: ''
})

const notifications = ref({
  stockAlerts: true,
  orderUpdates: true,
  aiInsights: true
})

const networkSettings = ref({
  enabled: true,
  shareLocation: false,
  autoJoin: false
})

useHead({
  title: 'Settings - TOSS'
})
</script>


