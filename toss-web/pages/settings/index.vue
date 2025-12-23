<script setup lang="ts">
import { ref } from 'vue'
import AppCard from '~/components/common/AppCard.vue'
import AppSectionHeader from '~/components/common/AppSectionHeader.vue'
import { Tabs, TabsList, TabsTrigger, TabsContent } from '~/components/ui/tabs'
import { Input } from '~/components/ui/input'
import { Button } from '~/components/ui/button'

useHead({
  title: 'Settings - TOSS Admin',
  meta: [{ name: 'description', content: 'Settings for TOSS Admin' }]
})

definePageMeta({
  layout: 'dashboard'
})

const activeTab = ref('profile')

const businessProfile = ref({
  name: 'My Spaza Shop',
  registration: '123456789',
  address: '123 Main Street',
  phone: '+27 82 123 4567',
  email: 'shop@example.com'
})

const users = ref([
  { id: 1, name: 'John Doe', email: 'john@example.com', role: 'Owner', status: 'Active' },
  { id: 2, name: 'Jane Smith', email: 'jane@example.com', role: 'Staff', status: 'Active' }
])
</script>

<template>
  <div class="space-y-6">
    <AppSectionHeader
      title="Settings"
      description="Manage your business profile, users, and preferences"
    />

    <Tabs v-model="activeTab" class="w-full">
      <TabsList class="grid w-full grid-cols-3">
        <TabsTrigger value="profile">Business Profile</TabsTrigger>
        <TabsTrigger value="users">Users & Roles</TabsTrigger>
        <TabsTrigger value="device">Device & PWA</TabsTrigger>
      </TabsList>

      <TabsContent value="profile" class="mt-6">
        <AppCard title="Business Profile">
          <div class="space-y-4">
            <div>
              <label class="block text-sm font-medium text-foreground mb-1">Business Name</label>
              <Input v-model="businessProfile.name" />
            </div>
            <div>
              <label class="block text-sm font-medium text-foreground mb-1">Registration Number</label>
              <Input v-model="businessProfile.registration" />
            </div>
            <div>
              <label class="block text-sm font-medium text-foreground mb-1">Address</label>
              <Input v-model="businessProfile.address" />
            </div>
            <div>
              <label class="block text-sm font-medium text-foreground mb-1">Phone</label>
              <Input v-model="businessProfile.phone" />
            </div>
            <div>
              <label class="block text-sm font-medium text-foreground mb-1">Email</label>
              <Input v-model="businessProfile.email" type="email" />
            </div>
            <div class="pt-4">
              <Button>Save Changes</Button>
            </div>
          </div>
        </AppCard>
      </TabsContent>

      <TabsContent value="users" class="mt-6">
        <AppCard title="Users & Roles">
          <div class="space-y-3">
            <div
              v-for="user in users"
              :key="user.id"
              class="flex items-center justify-between p-4 border border-border rounded-lg"
            >
              <div>
                <p class="font-medium text-foreground">{{ user.name }}</p>
                <p class="text-sm text-muted-foreground">{{ user.email }}</p>
                <p class="text-xs text-muted-foreground mt-1">Role: {{ user.role }}</p>
              </div>
              <span
                :class="[
                  'inline-flex items-center px-2 py-1 rounded text-xs font-medium',
                  user.status === 'Active' ? 'bg-green-100 dark:bg-green-900/20 text-green-700 dark:text-green-400' : ''
                ]"
              >
                {{ user.status }}
              </span>
            </div>
          </div>
        </AppCard>
      </TabsContent>

      <TabsContent value="device" class="mt-6">
        <AppCard title="Device & PWA Settings">
          <div class="space-y-4">
            <div>
              <h3 class="text-sm font-medium text-foreground mb-2">Offline Mode</h3>
              <p class="text-sm text-muted-foreground mb-4">Enable offline mode to work without internet connection</p>
              <label class="flex items-center gap-2">
                <input type="checkbox" class="rounded" />
                <span class="text-sm text-foreground">Enable offline mode</span>
              </label>
            </div>
            <div>
              <h3 class="text-sm font-medium text-foreground mb-2">Auto Sync</h3>
              <p class="text-sm text-muted-foreground mb-4">Automatically sync data when connection is restored</p>
              <label class="flex items-center gap-2">
                <input type="checkbox" class="rounded" checked />
                <span class="text-sm text-foreground">Enable auto sync</span>
              </label>
            </div>
            <div>
              <h3 class="text-sm font-medium text-foreground mb-2">Install App</h3>
              <p class="text-sm text-muted-foreground mb-4">Install TOSS as a Progressive Web App on your device</p>
              <Button variant="outline">Install PWA</Button>
            </div>
          </div>
        </AppCard>
      </TabsContent>
    </Tabs>
  </div>
</template>
