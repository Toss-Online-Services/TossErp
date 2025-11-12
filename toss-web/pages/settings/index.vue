<script setup lang="ts">
import { Button } from '~/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '~/components/ui/card'
import { Input } from '~/components/ui/input'
import { Label } from '~/components/ui/label'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '~/components/ui/tabs'
import { Switch } from '~/components/ui/switch'
import { Separator } from '~/components/ui/separator'

definePageMeta({
  middleware: 'auth',
})

const businessSettings = ref({
  name: 'My Spaza Shop',
  email: 'shop@example.com',
  phone: '+27 71 234 5678',
  address: 'Township, Johannesburg',
  vatNumber: 'VAT123456',
})

const notifications = ref({
  emailAlerts: true,
  smsAlerts: false,
  lowStockAlerts: true,
  dailySummary: true,
})

const saveSettings = () => {
  console.log('Saving settings...')
}
</script>

<template>
  <div class="flex-1 space-y-4 p-4 md:p-8 pt-6">
    <div>
      <h2 class="text-3xl font-bold tracking-tight">
        Settings
      </h2>
      <p class="text-muted-foreground">
        Manage your account and application preferences
      </p>
    </div>

    <Tabs default-value="business" class="space-y-4">
      <TabsList>
        <TabsTrigger value="business">
          Business
        </TabsTrigger>
        <TabsTrigger value="notifications">
          Notifications
        </TabsTrigger>
        <TabsTrigger value="security">
          Security
        </TabsTrigger>
        <TabsTrigger value="billing">
          Billing
        </TabsTrigger>
      </TabsList>

      <!-- Business Settings -->
      <TabsContent value="business" class="space-y-4">
        <Card>
          <CardHeader>
            <CardTitle>Business Information</CardTitle>
            <CardDescription>
              Update your business details and contact information
            </CardDescription>
          </CardHeader>
          <CardContent class="space-y-4">
            <div class="grid gap-4 md:grid-cols-2">
              <div class="space-y-2">
                <Label for="businessName">Business Name</Label>
                <Input
                  id="businessName"
                  v-model="businessSettings.name"
                  placeholder="My Spaza Shop"
                />
              </div>

              <div class="space-y-2">
                <Label for="email">Email</Label>
                <Input
                  id="email"
                  v-model="businessSettings.email"
                  type="email"
                  placeholder="shop@example.com"
                />
              </div>

              <div class="space-y-2">
                <Label for="phone">Phone Number</Label>
                <Input
                  id="phone"
                  v-model="businessSettings.phone"
                  type="tel"
                  placeholder="+27 XX XXX XXXX"
                />
              </div>

              <div class="space-y-2">
                <Label for="vat">VAT Number</Label>
                <Input
                  id="vat"
                  v-model="businessSettings.vatNumber"
                  placeholder="VAT123456"
                />
              </div>

              <div class="space-y-2 md:col-span-2">
                <Label for="address">Address</Label>
                <Input
                  id="address"
                  v-model="businessSettings.address"
                  placeholder="Full business address"
                />
              </div>
            </div>

            <Separator />

            <div class="flex justify-end gap-2">
              <Button variant="outline">Cancel</Button>
              <Button @click="saveSettings">Save Changes</Button>
            </div>
          </CardContent>
        </Card>
      </TabsContent>

      <!-- Notifications Settings -->
      <TabsContent value="notifications" class="space-y-4">
        <Card>
          <CardHeader>
            <CardTitle>Notification Preferences</CardTitle>
            <CardDescription>
              Choose how you want to be notified about important events
            </CardDescription>
          </CardHeader>
          <CardContent class="space-y-4">
            <div class="flex items-center justify-between">
              <div class="space-y-0.5">
                <Label>Email Alerts</Label>
                <p class="text-sm text-muted-foreground">
                  Receive email notifications for important events
                </p>
              </div>
              <Switch v-model:checked="notifications.emailAlerts" />
            </div>

            <Separator />

            <div class="flex items-center justify-between">
              <div class="space-y-0.5">
                <Label>SMS Alerts</Label>
                <p class="text-sm text-muted-foreground">
                  Get SMS notifications for critical updates
                </p>
              </div>
              <Switch v-model:checked="notifications.smsAlerts" />
            </div>

            <Separator />

            <div class="flex items-center justify-between">
              <div class="space-y-0.5">
                <Label>Low Stock Alerts</Label>
                <p class="text-sm text-muted-foreground">
                  Get notified when products are running low
                </p>
              </div>
              <Switch v-model:checked="notifications.lowStockAlerts" />
            </div>

            <Separator />

            <div class="flex items-center justify-between">
              <div class="space-y-0.5">
                <Label>Daily Summary</Label>
                <p class="text-sm text-muted-foreground">
                  Receive a daily summary of sales and activity
                </p>
              </div>
              <Switch v-model:checked="notifications.dailySummary" />
            </div>

            <Separator />

            <div class="flex justify-end gap-2">
              <Button variant="outline">Cancel</Button>
              <Button @click="saveSettings">Save Preferences</Button>
            </div>
          </CardContent>
        </Card>
      </TabsContent>

      <!-- Security Settings -->
      <TabsContent value="security" class="space-y-4">
        <Card>
          <CardHeader>
            <CardTitle>Security Settings</CardTitle>
            <CardDescription>
              Manage your password and security options
            </CardDescription>
          </CardHeader>
          <CardContent class="space-y-4">
            <div class="space-y-2">
              <Label for="currentPassword">Current Password</Label>
              <Input
                id="currentPassword"
                type="password"
                placeholder="Enter current password"
              />
            </div>

            <div class="space-y-2">
              <Label for="newPassword">New Password</Label>
              <Input
                id="newPassword"
                type="password"
                placeholder="Enter new password"
              />
            </div>

            <div class="space-y-2">
              <Label for="confirmPassword">Confirm New Password</Label>
              <Input
                id="confirmPassword"
                type="password"
                placeholder="Confirm new password"
              />
            </div>

            <Separator />

            <div class="flex justify-end gap-2">
              <Button variant="outline">Cancel</Button>
              <Button>Update Password</Button>
            </div>
          </CardContent>
        </Card>
      </TabsContent>

      <!-- Billing Settings -->
      <TabsContent value="billing" class="space-y-4">
        <Card>
          <CardHeader>
            <CardTitle>Billing & Subscription</CardTitle>
            <CardDescription>
              Manage your subscription and payment methods
            </CardDescription>
          </CardHeader>
          <CardContent class="space-y-4">
            <div class="rounded-lg border p-4">
              <div class="flex items-center justify-between">
                <div>
                  <p class="font-medium">Current Plan: Free</p>
                  <p class="text-sm text-muted-foreground">Limited features available</p>
                </div>
                <Button>Upgrade Plan</Button>
              </div>
            </div>

            <Separator />

            <p class="text-sm text-muted-foreground">
              Billing features will be available soon
            </p>
          </CardContent>
        </Card>
      </TabsContent>
    </Tabs>
  </div>
</template>
