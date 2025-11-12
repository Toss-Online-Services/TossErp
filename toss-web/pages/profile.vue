<template>
  <AppLayout>
    <div class="p-6 space-y-6">
      <!-- Header -->
      <div class="flex justify-between items-center">
        <div>
          <h1 class="text-2xl font-bold">My Profile</h1>
          <p class="text-muted-foreground">Manage your personal account information and preferences</p>
        </div>
        <div class="flex items-center space-x-4">
          <Button variant="outline">
            <Shield class="h-4 w-4 mr-2" />
            Privacy Settings
          </Button>
          <Button>
            <Save class="h-4 w-4 mr-2" />
            Save Changes
          </Button>
        </div>
      </div>

      <!-- Profile Overview -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Profile Card -->
        <div class="bg-white dark:bg-gray-800 rounded-lg border p-6">
          <div class="text-center">
            <div class="relative inline-block">
              <div class="w-24 h-24 bg-primary/10 rounded-full flex items-center justify-center mx-auto mb-4">
                <User class="h-12 w-12 text-primary" />
              </div>
              <button class="absolute -bottom-2 -right-2 w-8 h-8 bg-primary text-primary-foreground rounded-full flex items-center justify-center">
                <Camera class="h-4 w-4" />
              </button>
            </div>
            <h3 class="text-lg font-semibold">{{ profileData.firstName }} {{ profileData.lastName }}</h3>
            <p class="text-muted-foreground text-sm">{{ profileData.role }}</p>
            <p class="text-muted-foreground text-sm">{{ profileData.business }}</p>
            
            <!-- Quick Stats -->
            <div class="grid grid-cols-2 gap-4 mt-6">
              <div class="p-3 border rounded-lg">
                <p class="text-xs text-muted-foreground">Member Since</p>
                <p class="font-semibold">{{ profileData.memberSince }}</p>
              </div>
              <div class="p-3 border rounded-lg">
                <p class="text-xs text-muted-foreground">Last Login</p>
                <p class="font-semibold">{{ profileData.lastLogin }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Personal Information -->
        <div class="lg:col-span-2 bg-white dark:bg-gray-800 rounded-lg border">
          <div class="p-6 border-b">
            <div class="flex items-center space-x-2">
              <User class="h-5 w-5 text-blue-600" />
              <h3 class="text-lg font-semibold">Personal Information</h3>
            </div>
          </div>
          <div class="p-6 space-y-6">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium mb-2">First Name</label>
                <input
                  v-model="profileData.firstName"
                  type="text"
                  class="w-full px-3 py-2 border rounded-md"
                  placeholder="Enter first name"
                />
              </div>
              <div>
                <label class="block text-sm font-medium mb-2">Last Name</label>
                <input
                  v-model="profileData.lastName"
                  type="text"
                  class="w-full px-3 py-2 border rounded-md"
                  placeholder="Enter last name"
                />
              </div>
              <div>
                <label class="block text-sm font-medium mb-2">Email Address</label>
                <input
                  v-model="profileData.email"
                  type="email"
                  class="w-full px-3 py-2 border rounded-md"
                  placeholder="email@example.com"
                />
              </div>
              <div>
                <label class="block text-sm font-medium mb-2">Phone Number</label>
                <input
                  v-model="profileData.phone"
                  type="tel"
                  class="w-full px-3 py-2 border rounded-md"
                  placeholder="+27 12 345 6789"
                />
              </div>
              <div>
                <label class="block text-sm font-medium mb-2">ID Number</label>
                <input
                  v-model="profileData.idNumber"
                  type="text"
                  class="w-full px-3 py-2 border rounded-md"
                  placeholder="Enter SA ID number"
                />
              </div>
              <div>
                <label class="block text-sm font-medium mb-2">Date of Birth</label>
                <input
                  v-model="profileData.dateOfBirth"
                  type="date"
                  class="w-full px-3 py-2 border rounded-md"
                />
              </div>
            </div>

            <div>
              <label class="block text-sm font-medium mb-2">Address</label>
              <textarea
                v-model="profileData.address"
                class="w-full px-3 py-2 border rounded-md"
                rows="3"
                placeholder="Enter your address including township/suburb"
              ></textarea>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium mb-2">Province</label>
                <select v-model="profileData.province" class="w-full px-3 py-2 border rounded-md">
                  <option value="gauteng">Gauteng</option>
                  <option value="western_cape">Western Cape</option>
                  <option value="kwazulu_natal">KwaZulu-Natal</option>
                  <option value="eastern_cape">Eastern Cape</option>
                  <option value="free_state">Free State</option>
                  <option value="limpopo">Limpopo</option>
                  <option value="mpumalanga">Mpumalanga</option>
                  <option value="northern_cape">Northern Cape</option>
                  <option value="north_west">North West</option>
                </select>
              </div>
              <div>
                <label class="block text-sm font-medium mb-2">Preferred Language</label>
                <select v-model="profileData.language" class="w-full px-3 py-2 border rounded-md">
                  <option value="en">English</option>
                  <option value="af">Afrikaans</option>
                  <option value="zu">isiZulu</option>
                  <option value="xh">isiXhosa</option>
                  <option value="st">Sesotho</option>
                  <option value="tn">Setswana</option>
                  <option value="ss">siSwati</option>
                  <option value="ve">Tshivenda</option>
                  <option value="ts">Xitsonga</option>
                </select>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Security Settings -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Password & Security -->
        <div class="bg-white dark:bg-gray-800 rounded-lg border">
          <div class="p-6 border-b">
            <div class="flex items-center space-x-2">
              <Lock class="h-5 w-5 text-red-600" />
              <h3 class="text-lg font-semibold">Password & Security</h3>
            </div>
          </div>
          <div class="p-6 space-y-6">
            <div>
              <label class="block text-sm font-medium mb-2">Current Password</label>
              <input
                v-model="passwordData.current"
                type="password"
                class="w-full px-3 py-2 border rounded-md"
                placeholder="Enter current password"
              />
            </div>
            <div>
              <label class="block text-sm font-medium mb-2">New Password</label>
              <input
                v-model="passwordData.new"
                type="password"
                class="w-full px-3 py-2 border rounded-md"
                placeholder="Enter new password"
              />
            </div>
            <div>
              <label class="block text-sm font-medium mb-2">Confirm New Password</label>
              <input
                v-model="passwordData.confirm"
                type="password"
                class="w-full px-3 py-2 border rounded-md"
                placeholder="Confirm new password"
              />
            </div>
            
            <!-- Password Requirements -->
            <div class="p-4 bg-muted rounded-lg">
              <h4 class="text-sm font-medium mb-2">Password Requirements:</h4>
              <ul class="text-sm text-muted-foreground space-y-1">
                <li class="flex items-center space-x-2">
                  <div class="w-2 h-2 bg-green-500 rounded-full"></div>
                  <span>At least 8 characters long</span>
                </li>
                <li class="flex items-center space-x-2">
                  <div class="w-2 h-2 bg-gray-300 rounded-full"></div>
                  <span>Include uppercase and lowercase letters</span>
                </li>
                <li class="flex items-center space-x-2">
                  <div class="w-2 h-2 bg-gray-300 rounded-full"></div>
                  <span>Include at least one number</span>
                </li>
                <li class="flex items-center space-x-2">
                  <div class="w-2 h-2 bg-gray-300 rounded-full"></div>
                  <span>Include special characters</span>
                </li>
              </ul>
            </div>

            <Button class="w-full">Update Password</Button>
          </div>
        </div>

        <!-- Two-Factor Authentication -->
        <div class="bg-white dark:bg-gray-800 rounded-lg border">
          <div class="p-6 border-b">
            <div class="flex items-center space-x-2">
              <Smartphone class="h-5 w-5 text-green-600" />
              <h3 class="text-lg font-semibold">Two-Factor Authentication</h3>
            </div>
          </div>
          <div class="p-6 space-y-6">
            <div class="flex items-center justify-between">
              <div>
                <p class="font-medium">SMS Authentication</p>
                <p class="text-sm text-muted-foreground">Receive codes via SMS</p>
              </div>
              <input type="checkbox" v-model="securitySettings.smsAuth" class="rounded" />
            </div>

            <div class="flex items-center justify-between">
              <div>
                <p class="font-medium">Email Authentication</p>
                <p class="text-sm text-muted-foreground">Receive codes via email</p>
              </div>
              <input type="checkbox" v-model="securitySettings.emailAuth" class="rounded" />
            </div>

            <div class="flex items-center justify-between">
              <div>
                <p class="font-medium">Backup Codes</p>
                <p class="text-sm text-muted-foreground">Generate backup codes</p>
              </div>
              <Button variant="outline" size="sm">Generate</Button>
            </div>

            <!-- Recent Login Activity -->
            <div>
              <h4 class="font-medium mb-3">Recent Login Activity</h4>
              <div class="space-y-2">
                <div v-for="activity in loginActivity" :key="activity.id" class="flex items-center justify-between p-2 border rounded">
                  <div class="flex items-center space-x-3">
                    <component :is="activity.icon" class="h-4 w-4 text-muted-foreground" />
                    <div>
                      <p class="text-sm font-medium">{{ activity.device }}</p>
                      <p class="text-xs text-muted-foreground">{{ activity.location }}</p>
                    </div>
                  </div>
                  <div class="text-right">
                    <p class="text-xs text-muted-foreground">{{ activity.time }}</p>
                    <span :class="activity.status === 'success' ? 'text-green-600' : 'text-red-600'" class="text-xs">
                      {{ activity.status }}
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Business Role & Permissions -->
      <div class="bg-white dark:bg-gray-800 rounded-lg border">
        <div class="p-6 border-b">
          <div class="flex items-center space-x-2">
            <Building class="h-5 w-5 text-purple-600" />
            <h3 class="text-lg font-semibold">Business Role & Permissions</h3>
          </div>
        </div>
        <div class="p-6">
          <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <!-- Current Role -->
            <div>
              <h4 class="font-medium mb-3">Current Role</h4>
              <div class="p-4 border rounded-lg">
                <div class="flex items-center justify-between">
                  <div>
                    <p class="font-medium text-primary">{{ profileData.role }}</p>
                    <p class="text-sm text-muted-foreground">Full system access</p>
                  </div>
                  <Badge variant="secondary">Active</Badge>
                </div>
              </div>
            </div>

            <!-- Permissions -->
            <div>
              <h4 class="font-medium mb-3">Permissions</h4>
              <div class="space-y-2">
                <div v-for="permission in permissions" :key="permission.name" class="flex items-center justify-between">
                  <div class="flex items-center space-x-3">
                    <component :is="permission.icon" class="h-4 w-4 text-muted-foreground" />
                    <span class="text-sm">{{ permission.name }}</span>
                  </div>
                  <div class="flex items-center space-x-2">
                    <span :class="permission.granted ? 'text-green-600' : 'text-red-600'" class="text-sm">
                      {{ permission.granted ? 'Granted' : 'Denied' }}
                    </span>
                    <component :is="permission.granted ? Check : X" class="h-4 w-4" :class="permission.granted ? 'text-green-600' : 'text-red-600'" />
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Account Actions -->
      <div class="bg-white dark:bg-gray-800 rounded-lg border">
        <div class="p-6 border-b">
          <div class="flex items-center space-x-2">
            <AlertTriangle class="h-5 w-5 text-yellow-600" />
            <h3 class="text-lg font-semibold">Account Actions</h3>
          </div>
        </div>
        <div class="p-6">
          <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div class="text-center">
              <Button variant="outline" class="w-full mb-2">
                <Download class="h-4 w-4 mr-2" />
                Export Data
              </Button>
              <p class="text-sm text-muted-foreground">Download your account data</p>
            </div>
            <div class="text-center">
              <Button variant="outline" class="w-full mb-2">
                <RefreshCw class="h-4 w-4 mr-2" />
                Reset Settings
              </Button>
              <p class="text-sm text-muted-foreground">Reset to default settings</p>
            </div>
            <div class="text-center">
              <Button variant="destructive" class="w-full mb-2">
                <Trash2 class="h-4 w-4 mr-2" />
                Delete Account
              </Button>
              <p class="text-sm text-muted-foreground">Permanently delete account</p>
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
  Shield,
  Save,
  User,
  Camera,
  Lock,
  Smartphone,
  Building,
  AlertTriangle,
  Download,
  RefreshCw,
  Trash2,
  Check,
  X,
  Monitor,
  ShoppingCart,
  Package,
  CreditCard,
  Users,
  Settings,
  BarChart3
} from 'lucide-vue-next'

// Profile data
const profileData = ref({
  firstName: 'Siphiwo',
  lastName: 'Mthembu',
  email: 'siphiwo@example.com',
  phone: '+27 81 123 4567',
  idNumber: '8901234567890',
  dateOfBirth: '1989-03-15',
  address: '123 Main Road\nSoweto\nJohannesburg',
  province: 'gauteng',
  language: 'en',
  role: 'Business Owner',
  business: 'Siphiwo\'s Spaza Shop',
  memberSince: 'Jan 2024',
  lastLogin: '2 hours ago'
})

// Password change data
const passwordData = ref({
  current: '',
  new: '',
  confirm: ''
})

// Security settings
const securitySettings = ref({
  smsAuth: true,
  emailAuth: false,
  backupCodes: false
})

// Login activity
const loginActivity = ref([
  {
    id: 1,
    device: 'Chrome on Windows',
    location: 'Johannesburg, South Africa',
    time: '2 hours ago',
    status: 'success',
    icon: Monitor
  },
  {
    id: 2,
    device: 'Mobile App on Android',
    location: 'Johannesburg, South Africa',
    time: '1 day ago',
    status: 'success',
    icon: Smartphone
  },
  {
    id: 3,
    device: 'Chrome on Windows',
    location: 'Unknown Location',
    time: '3 days ago',
    status: 'failed',
    icon: Monitor
  }
])

// Permissions
const permissions = ref([
  { name: 'View Sales Reports', icon: BarChart3, granted: true },
  { name: 'Manage Inventory', icon: Package, granted: true },
  { name: 'Process Payments', icon: CreditCard, granted: true },
  { name: 'Manage Customers', icon: Users, granted: true },
  { name: 'System Settings', icon: Settings, granted: true },
  { name: 'User Management', icon: Users, granted: false }
])
</script>