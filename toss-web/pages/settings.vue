<template>
  <AppLayout>
    <div class="p-6 space-y-6">
      <!-- Header -->
      <div class="flex justify-between items-center">
        <div>
          <h1 class="text-2xl font-bold">Settings</h1>
          <p class="text-muted-foreground">Configure your TOSS ERP system preferences and business settings</p>
        </div>
        <div class="flex items-center space-x-4">
          <Button variant="outline">
            <RefreshCw class="h-4 w-4 mr-2" />
            Reset to Defaults
          </Button>
          <Button>
            <Save class="h-4 w-4 mr-2" />
            Save All Changes
          </Button>
        </div>
      </div>

      <!-- Settings Navigation -->
      <div class="grid grid-cols-1 lg:grid-cols-4 gap-6">
        <!-- Settings Menu -->
        <div class="bg-white dark:bg-gray-800 rounded-lg border p-6">
          <h3 class="font-semibold mb-4">Settings Categories</h3>
          <nav class="space-y-2">
            <button
              v-for="category in settingsCategories"
              :key="category.id"
              @click="activeCategory = category.id"
              :class="[
                'w-full text-left px-3 py-2 rounded-md flex items-center space-x-2 transition-colors',
                activeCategory === category.id 
                  ? 'bg-primary text-primary-foreground' 
                  : 'hover:bg-muted'
              ]"
            >
              <component :is="category.icon" class="h-4 w-4" />
              <span>{{ category.name }}</span>
            </button>
          </nav>
        </div>

        <!-- Settings Content -->
        <div class="lg:col-span-3 space-y-6">
          <!-- Business Settings -->
          <div v-if="activeCategory === 'business'" class="bg-white dark:bg-gray-800 rounded-lg border">
            <div class="p-6 border-b">
              <div class="flex items-center space-x-2">
                <Building class="h-5 w-5 text-blue-600" />
                <h3 class="text-lg font-semibold">Business Information</h3>
              </div>
            </div>
            <div class="p-6 space-y-6">
              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium mb-2">Business Name</label>
                  <input
                    v-model="businessSettings.name"
                    type="text"
                    class="w-full px-3 py-2 border rounded-md"
                    placeholder="Enter business name"
                  />
                </div>
                <div>
                  <label class="block text-sm font-medium mb-2">Business Type</label>
                  <select v-model="businessSettings.type" class="w-full px-3 py-2 border rounded-md">
                    <option value="spaza">Spaza Shop</option>
                    <option value="restaurant">Restaurant/Chisa Nyama</option>
                    <option value="retail">General Retail</option>
                    <option value="services">Service Business</option>
                    <option value="wholesale">Wholesale</option>
                  </select>
                </div>
                <div>
                  <label class="block text-sm font-medium mb-2">VAT Registration Number</label>
                  <input
                    v-model="businessSettings.vatNumber"
                    type="text"
                    class="w-full px-3 py-2 border rounded-md"
                    placeholder="4123456789"
                  />
                </div>
                <div>
                  <label class="block text-sm font-medium mb-2">Company Registration</label>
                  <input
                    v-model="businessSettings.companyReg"
                    type="text"
                    class="w-full px-3 py-2 border rounded-md"
                    placeholder="2023/123456/23"
                  />
                </div>
              </div>
              <div>
                <label class="block text-sm font-medium mb-2">Business Address</label>
                <textarea
                  v-model="businessSettings.address"
                  class="w-full px-3 py-2 border rounded-md"
                  rows="3"
                  placeholder="Enter full business address including township/suburb"
                ></textarea>
              </div>
              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium mb-2">Contact Number</label>
                  <input
                    v-model="businessSettings.phone"
                    type="tel"
                    class="w-full px-3 py-2 border rounded-md"
                    placeholder="+27 12 345 6789"
                  />
                </div>
                <div>
                  <label class="block text-sm font-medium mb-2">Email Address</label>
                  <input
                    v-model="businessSettings.email"
                    type="email"
                    class="w-full px-3 py-2 border rounded-md"
                    placeholder="business@example.com"
                  />
                </div>
              </div>
            </div>
          </div>

          <!-- System Preferences -->
          <div v-if="activeCategory === 'system'" class="bg-white dark:bg-gray-800 rounded-lg border">
            <div class="p-6 border-b">
              <div class="flex items-center space-x-2">
                <Settings class="h-5 w-5 text-green-600" />
                <h3 class="text-lg font-semibold">System Preferences</h3>
              </div>
            </div>
            <div class="p-6 space-y-6">
              <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div>
                  <label class="block text-sm font-medium mb-2">Default Currency</label>
                  <select v-model="systemSettings.currency" class="w-full px-3 py-2 border rounded-md">
                    <option value="ZAR">South African Rand (R)</option>
                    <option value="USD">US Dollar ($)</option>
                    <option value="EUR">Euro (€)</option>
                  </select>
                </div>
                <div>
                  <label class="block text-sm font-medium mb-2">Date Format</label>
                  <select v-model="systemSettings.dateFormat" class="w-full px-3 py-2 border rounded-md">
                    <option value="DD/MM/YYYY">DD/MM/YYYY</option>
                    <option value="MM/DD/YYYY">MM/DD/YYYY</option>
                    <option value="YYYY-MM-DD">YYYY-MM-DD</option>
                  </select>
                </div>
                <div>
                  <label class="block text-sm font-medium mb-2">Default Language</label>
                  <select v-model="systemSettings.language" class="w-full px-3 py-2 border rounded-md">
                    <option value="en">English</option>
                    <option value="af">Afrikaans</option>
                    <option value="zu">isiZulu</option>
                    <option value="xh">isiXhosa</option>
                    <option value="st">Sesotho</option>
                  </select>
                </div>
                <div>
                  <label class="block text-sm font-medium mb-2">Time Zone</label>
                  <select v-model="systemSettings.timezone" class="w-full px-3 py-2 border rounded-md">
                    <option value="Africa/Johannesburg">South Africa Standard Time</option>
                    <option value="UTC">UTC</option>
                  </select>
                </div>
              </div>
              
              <!-- Theme Settings -->
              <div>
                <label class="block text-sm font-medium mb-3">Theme Preference</label>
                <div class="flex space-x-4">
                  <button
                    @click="systemSettings.theme = 'light'"
                    :class="[
                      'flex items-center space-x-2 px-4 py-3 border rounded-lg',
                      systemSettings.theme === 'light' ? 'border-primary bg-primary/10' : 'border-muted'
                    ]"
                  >
                    <Sun class="h-4 w-4" />
                    <span>Light</span>
                  </button>
                  <button
                    @click="systemSettings.theme = 'dark'"
                    :class="[
                      'flex items-center space-x-2 px-4 py-3 border rounded-lg',
                      systemSettings.theme === 'dark' ? 'border-primary bg-primary/10' : 'border-muted'
                    ]"
                  >
                    <Moon class="h-4 w-4" />
                    <span>Dark</span>
                  </button>
                  <button
                    @click="systemSettings.theme = 'auto'"
                    :class="[
                      'flex items-center space-x-2 px-4 py-3 border rounded-lg',
                      systemSettings.theme === 'auto' ? 'border-primary bg-primary/10' : 'border-muted'
                    ]"
                  >
                    <Laptop class="h-4 w-4" />
                    <span>Auto</span>
                  </button>
                </div>
              </div>

              <!-- Notification Settings -->
              <div>
                <label class="block text-sm font-medium mb-3">Notifications</label>
                <div class="space-y-3">
                  <div class="flex items-center justify-between">
                    <span>Low stock alerts</span>
                    <input type="checkbox" v-model="systemSettings.notifications.lowStock" class="rounded" />
                  </div>
                  <div class="flex items-center justify-between">
                    <span>Daily sales summary</span>
                    <input type="checkbox" v-model="systemSettings.notifications.dailySales" class="rounded" />
                  </div>
                  <div class="flex items-center justify-between">
                    <span>Payment reminders</span>
                    <input type="checkbox" v-model="systemSettings.notifications.payments" class="rounded" />
                  </div>
                  <div class="flex items-center justify-between">
                    <span>System updates</span>
                    <input type="checkbox" v-model="systemSettings.notifications.updates" class="rounded" />
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Payment Settings -->
          <div v-if="activeCategory === 'payments'" class="bg-white dark:bg-gray-800 rounded-lg border">
            <div class="p-6 border-b">
              <div class="flex items-center space-x-2">
                <CreditCard class="h-5 w-5 text-purple-600" />
                <h3 class="text-lg font-semibold">Payment Settings</h3>
              </div>
            </div>
            <div class="p-6 space-y-6">
              <div>
                <label class="block text-sm font-medium mb-3">Accepted Payment Methods</label>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div v-for="method in paymentMethods" :key="method.id" class="flex items-center justify-between p-3 border rounded-lg">
                    <div class="flex items-center space-x-3">
                      <component :is="method.icon" class="h-5 w-5" />
                      <span>{{ method.name }}</span>
                    </div>
                    <input type="checkbox" v-model="method.enabled" class="rounded" />
                  </div>
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium mb-2">Default Payment Terms</label>
                <select v-model="paymentSettings.defaultTerms" class="w-full px-3 py-2 border rounded-md">
                  <option value="immediate">Cash on Delivery</option>
                  <option value="7_days">7 Days</option>
                  <option value="14_days">14 Days</option>
                  <option value="30_days">30 Days</option>
                  <option value="custom">Custom Terms</option>
                </select>
              </div>

              <div>
                <label class="block text-sm font-medium mb-2">Credit Limit Policy</label>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-xs text-muted-foreground mb-1">Default Credit Limit (R)</label>
                    <input
                      v-model="paymentSettings.defaultCreditLimit"
                      type="number"
                      class="w-full px-3 py-2 border rounded-md"
                      placeholder="5000"
                    />
                  </div>
                  <div>
                    <label class="block text-xs text-muted-foreground mb-1">Maximum Credit Period (Days)</label>
                    <input
                      v-model="paymentSettings.maxCreditDays"
                      type="number"
                      class="w-full px-3 py-2 border rounded-md"
                      placeholder="30"
                    />
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Inventory Settings -->
          <div v-if="activeCategory === 'inventory'" class="bg-white dark:bg-gray-800 rounded-lg border">
            <div class="p-6 border-b">
              <div class="flex items-center space-x-2">
                <Package class="h-5 w-5 text-orange-600" />
                <h3 class="text-lg font-semibold">Inventory Settings</h3>
              </div>
            </div>
            <div class="p-6 space-y-6">
              <div>
                <label class="block text-sm font-medium mb-3">Low Stock Alerts</label>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-xs text-muted-foreground mb-1">Default Low Stock Threshold</label>
                    <input
                      v-model="inventorySettings.lowStockThreshold"
                      type="number"
                      class="w-full px-3 py-2 border rounded-md"
                      placeholder="10"
                    />
                  </div>
                  <div>
                    <label class="block text-xs text-muted-foreground mb-1">Critical Stock Threshold</label>
                    <input
                      v-model="inventorySettings.criticalStockThreshold"
                      type="number"
                      class="w-full px-3 py-2 border rounded-md"
                      placeholder="5"
                    />
                  </div>
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium mb-3">Auto-Reorder Settings</label>
                <div class="space-y-3">
                  <div class="flex items-center justify-between">
                    <span>Enable automatic reordering</span>
                    <input type="checkbox" v-model="inventorySettings.autoReorder" class="rounded" />
                  </div>
                  <div class="flex items-center justify-between">
                    <span>Include seasonal adjustments</span>
                    <input type="checkbox" v-model="inventorySettings.seasonalAdjustment" class="rounded" />
                  </div>
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium mb-2">Barcode Settings</label>
                <div class="space-y-3">
                  <div class="flex items-center justify-between">
                    <span>Generate barcodes for new products</span>
                    <input type="checkbox" v-model="inventorySettings.autoBarcodes" class="rounded" />
                  </div>
                  <div>
                    <label class="block text-xs text-muted-foreground mb-1">Barcode Prefix</label>
                    <input
                      v-model="inventorySettings.barcodePrefix"
                      type="text"
                      class="w-full px-3 py-2 border rounded-md"
                      placeholder="TSS"
                      maxlength="3"
                    />
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Security Settings -->
          <div v-if="activeCategory === 'security'" class="bg-white dark:bg-gray-800 rounded-lg border">
            <div class="p-6 border-b">
              <div class="flex items-center space-x-2">
                <Shield class="h-5 w-5 text-red-600" />
                <h3 class="text-lg font-semibold">Security Settings</h3>
              </div>
            </div>
            <div class="p-6 space-y-6">
              <div>
                <label class="block text-sm font-medium mb-2">Password Requirements</label>
                <div class="space-y-3">
                  <div class="flex items-center justify-between">
                    <span>Require strong passwords</span>
                    <input type="checkbox" v-model="securitySettings.strongPasswords" class="rounded" />
                  </div>
                  <div class="flex items-center justify-between">
                    <span>Password expiry (90 days)</span>
                    <input type="checkbox" v-model="securitySettings.passwordExpiry" class="rounded" />
                  </div>
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium mb-2">Session Settings</label>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label class="block text-xs text-muted-foreground mb-1">Session Timeout (minutes)</label>
                    <select v-model="securitySettings.sessionTimeout" class="w-full px-3 py-2 border rounded-md">
                      <option value="15">15 minutes</option>
                      <option value="30">30 minutes</option>
                      <option value="60">1 hour</option>
                      <option value="120">2 hours</option>
                    </select>
                  </div>
                  <div>
                    <label class="block text-xs text-muted-foreground mb-1">Maximum Failed Attempts</label>
                    <select v-model="securitySettings.maxFailedAttempts" class="w-full px-3 py-2 border rounded-md">
                      <option value="3">3 attempts</option>
                      <option value="5">5 attempts</option>
                      <option value="10">10 attempts</option>
                    </select>
                  </div>
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium mb-3">Backup & Recovery</label>
                <div class="space-y-3">
                  <div class="flex items-center justify-between">
                    <span>Automatic daily backups</span>
                    <input type="checkbox" v-model="securitySettings.autoBackup" class="rounded" />
                  </div>
                  <div class="flex items-center justify-between">
                    <span>Cloud backup sync</span>
                    <input type="checkbox" v-model="securitySettings.cloudBackup" class="rounded" />
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </AppLayout>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { 
  RefreshCw,
  Save,
  Building,
  Settings,
  CreditCard,
  Package,
  Shield,
  Sun,
  Moon,
  Laptop,
  Banknote,
  Smartphone,
  Wallet,
  Coins
} from 'lucide-vue-next'

// Active category
const activeCategory = ref('business')

// Settings categories
const settingsCategories = ref([
  { id: 'business', name: 'Business Info', icon: Building },
  { id: 'system', name: 'System', icon: Settings },
  { id: 'payments', name: 'Payments', icon: CreditCard },
  { id: 'inventory', name: 'Inventory', icon: Package },
  { id: 'security', name: 'Security', icon: Shield }
])

// Business settings
const businessSettings = ref({
  name: 'Siphiwo\'s Spaza Shop',
  type: 'spaza',
  vatNumber: '4123456789',
  companyReg: '2023/123456/23',
  address: '123 Main Road\nSoweto, Johannesburg\nGauteng, 1818',
  phone: '+27 11 123 4567',
  email: 'siphiwo@example.com'
})

// System settings
const systemSettings = ref({
  currency: 'ZAR',
  dateFormat: 'DD/MM/YYYY',
  language: 'en',
  timezone: 'Africa/Johannesburg',
  theme: 'light',
  notifications: {
    lowStock: true,
    dailySales: true,
    payments: true,
    updates: false
  }
})

// Payment methods
const paymentMethods = ref([
  { id: 'cash', name: 'Cash', icon: Banknote, enabled: true },
  { id: 'card', name: 'Card Payment', icon: CreditCard, enabled: true },
  { id: 'mobile', name: 'Mobile Money', icon: Smartphone, enabled: true },
  { id: 'ewallet', name: 'E-Wallet', icon: Wallet, enabled: false },
  { id: 'crypto', name: 'Cryptocurrency', icon: Coins, enabled: false }
])

// Payment settings
const paymentSettings = ref({
  defaultTerms: 'immediate',
  defaultCreditLimit: 5000,
  maxCreditDays: 30
})

// Inventory settings
const inventorySettings = ref({
  lowStockThreshold: 10,
  criticalStockThreshold: 5,
  autoReorder: false,
  seasonalAdjustment: true,
  autoBarcodes: true,
  barcodePrefix: 'TSS'
})

// Security settings
const securitySettings = ref({
  strongPasswords: true,
  passwordExpiry: false,
  sessionTimeout: '30',
  maxFailedAttempts: '5',
  autoBackup: true,
  cloudBackup: false
})
</script>