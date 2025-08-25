<template>
  <div class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Header -->
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-gray-900 dark:text-white">Project Management</h1>
        <p class="mt-2 text-gray-600 dark:text-gray-400">Manage projects, tasks, timelines, and team collaboration</p>
      </div>

      <!-- Coming Soon Banner -->
      <div class="bg-amber-50 dark:bg-amber-900/20 border border-amber-200 dark:border-amber-800 rounded-lg p-6 text-center">
        <IconBase class="mx-auto h-12 w-12 text-amber-600 mb-4">
          <path d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 0 1 2.25-2.25h13.5A2.25 2.25 0 0 1 21 7.5v11.25m-18 0A2.25 2.25 0 0 0 5.25 21h13.5A2.25 2.25 0 0 0 21 18.75m-18 0v-7.5A2.25 2.25 0 0 1 5.25 9h13.5A2.25 2.25 0 0 1 21 11.25v7.5" />
        </IconBase>
        <h2 class="text-xl font-semibold text-amber-900 dark:text-amber-100 mb-2">Coming Soon</h2>
        <p class="text-amber-700 dark:text-amber-300">
          The project management module is currently under development and will be available soon.
        </p>
      </div>

      <!-- Feature Preview -->
      <div class="mt-8 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Task Management</h3>
          <p class="text-gray-600 dark:text-gray-400">Create, assign, and track tasks with deadlines and priorities.</p>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Timeline Planning</h3>
          <p class="text-gray-600 dark:text-gray-400">Gantt charts, milestones, and project scheduling tools.</p>
        </div>
        <div class="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">Team Collaboration</h3>
          <p class="text-gray-600 dark:text-gray-400">Share files, communicate, and collaborate on project deliverables.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
useHead({
  title: 'Project Management - TOSS ERP III'
})
</script>
