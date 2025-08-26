<template>
  <div class="relative">
    <button @click="isOpen = !isOpen" class="flex items-center space-x-2">
      <img class="h-8 w-8 rounded-full" src="https://www.gravatar.com/avatar/00000000000000000000000000000000?d=mp" alt="">
      <span>Username</span>
    </button>
    <div v-if="isOpen" class="absolute right-0 mt-2 w-48 bg-white dark:bg-gray-800 rounded-md shadow-lg py-1">
      <NuxtLink to="/profile" class="block px-4 py-2 text-sm text-gray-700 dark:text-gray-200 hover:bg-gray-100 dark:hover:bg-gray-700">Profile</NuxtLink>
      <a href="#" @click.prevent="logout" class="block px-4 py-2 text-sm text-gray-700 dark:text-gray-200 hover:bg-gray-100 dark:hover:bg-gray-700">Logout</a>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const isOpen = ref(false)

function logout() {
  // handle logout
}
</script>
