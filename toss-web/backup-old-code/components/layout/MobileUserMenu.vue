<template>
  <div class="relative">
    <button @click="isOpen = !isOpen" class="flex items-center p-1 rounded-lg hover:bg-slate-100 dark:hover:bg-slate-700">
      <img class="h-7 w-7 rounded-full" src="https://www.gravatar.com/avatar/00000000000000000000000000000000?d=mp" alt="User">
    </button>
    
    <!-- Mobile dropdown menu -->
    <div v-if="isOpen" class="absolute right-0 mt-2 w-56 bg-white dark:bg-slate-800 rounded-lg shadow-lg border border-slate-200 dark:border-slate-700 z-50">
      <div class="py-2">
        <!-- User info -->
        <div class="px-4 py-3 border-b border-slate-200 dark:border-slate-700">
          <div class="flex items-center">
            <img class="h-10 w-10 rounded-full" src="https://www.gravatar.com/avatar/00000000000000000000000000000000?d=mp" alt="User">
            <div class="ml-3">
              <p class="text-sm font-medium text-slate-900 dark:text-white">John Doe</p>
              <p class="text-xs text-slate-600 dark:text-slate-400">john@example.com</p>
            </div>
          </div>
        </div>
        
        <!-- Menu items -->
        <div class="py-1">
          <NuxtLink 
            to="/profile" 
            class="mobile-menu-item"
            @click="isOpen = false"
          >
            <UserIcon class="h-4 w-4 mr-3" />
            My Profile
          </NuxtLink>
          
          <NuxtLink 
            to="/settings" 
            class="mobile-menu-item"
            @click="isOpen = false"
          >
            <CogIcon class="h-4 w-4 mr-3" />
            Settings
          </NuxtLink>
          
          <NuxtLink 
            to="/help" 
            class="mobile-menu-item"
            @click="isOpen = false"
          >
            <QuestionMarkCircleIcon class="h-4 w-4 mr-3" />
            Help & Support
          </NuxtLink>
          
          <div class="border-t border-slate-200 dark:border-slate-700 my-1"></div>
          
          <button @click="logout" class="mobile-menu-item text-red-600 dark:text-red-400 w-full text-left">
            <ArrowRightOnRectangleIcon class="h-4 w-4 mr-3" />
            Sign Out
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { 
  UserIcon, 
  CogIcon, 
  QuestionMarkCircleIcon, 
  ArrowRightOnRectangleIcon 
} from '@heroicons/vue/24/outline'

const isOpen = ref(false)

// Close menu when clicking outside
onMounted(() => {
  document.addEventListener('click', (e) => {
    if (!e.target.closest('.relative')) {
      isOpen.value = false
    }
  })
})

function logout() {
  isOpen.value = false
  // Handle logout logic
  console.log('Logging out...')
}
</script>

<style scoped>
.mobile-menu-item {
  display: flex;
  align-items: center;
  width: 100%;
  padding: 0.5rem 1rem;
  text-align: left;
  font-size: 0.875rem;
  color: rgb(71 85 105);
  transition: background-color 0.2s;
  touch-action: manipulation;
}

.mobile-menu-item:hover {
  background-color: rgb(248 250 252);
}

.dark .mobile-menu-item {
  color: rgb(203 213 225);
}

.dark .mobile-menu-item:hover {
  background-color: rgb(30 41 59);
}
</style>
