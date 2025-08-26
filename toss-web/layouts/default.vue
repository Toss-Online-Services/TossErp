<template>
  <div class="flex h-screen bg-slate-50 dark:bg-slate-900 text-slate-900 dark:text-slate-100">
    <Sidebar />
    <div class="flex-1 flex flex-col overflow-hidden">
      <Header />
      <main class="flex-1 overflow-x-hidden overflow-y-auto bg-slate-50 dark:bg-slate-900">
        <slot />
      </main>
    </div>
  </div>
</template>

<script setup>
import Sidebar from '~/components/layout/Sidebar.vue'
import Header from '~/components/layout/Header.vue'
</script>
