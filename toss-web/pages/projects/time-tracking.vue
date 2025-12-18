<template>
  <div class="h-full overflow-y-auto p-6 custom-scrollbar">
    <!-- Page Header -->
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-stone-900 dark:text-white mb-2">Time Tracking</h1>
        <p class="text-stone-500 dark:text-stone-400">Track time spent on projects and tasks</p>
      </div>
      <Button class="bg-stone-900 hover:bg-stone-800 dark:bg-white dark:hover:bg-stone-100 dark:text-stone-900">
        <Icon name="lucide:play" class="w-5 h-5 mr-2" />
        Start Timer
      </Button>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Today</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">6h 45m</p>
            </div>
            <Icon name="lucide:clock" class="w-10 h-10 text-blue-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">This Week</p>
              <p class="text-2xl font-bold text-green-600">32h 15m</p>
            </div>
            <Icon name="lucide:calendar-days" class="w-10 h-10 text-green-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">This Month</p>
              <p class="text-2xl font-bold text-purple-600">128h 30m</p>
            </div>
            <Icon name="lucide:calendar" class="w-10 h-10 text-purple-500" />
          </div>
        </CardContent>
      </Card>

      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm font-medium text-stone-500">Billable</p>
              <p class="text-2xl font-bold text-stone-900 dark:text-white">78%</p>
            </div>
            <Icon name="lucide:receipt" class="w-10 h-10 text-orange-500" />
          </div>
        </CardContent>
      </Card>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Current Timer -->
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardHeader class="border-b border-stone-200 dark:border-stone-700">
          <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Current Session</CardTitle>
        </CardHeader>
        <CardContent class="p-6">
          <div class="text-center py-8">
            <p class="text-5xl font-bold text-stone-900 dark:text-white mb-4">02:34:17</p>
            <p class="text-sm text-stone-500 dark:text-stone-400 mb-2">Working on</p>
            <p class="text-lg font-medium text-stone-900 dark:text-white">POS System Upgrade</p>
            <p class="text-sm text-stone-500 dark:text-stone-400 mb-6">Configure new terminals</p>
            <div class="flex gap-3 justify-center">
              <Button variant="outline" size="lg">
                <Icon name="lucide:pause" class="w-5 h-5 mr-2" />
                Pause
              </Button>
              <Button variant="destructive" size="lg">
                <Icon name="lucide:square" class="w-5 h-5 mr-2" />
                Stop
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>

      <!-- Recent Time Entries -->
      <Card class="lg:col-span-2 bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardHeader class="border-b border-stone-200 dark:border-stone-700">
          <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Recent Entries</CardTitle>
        </CardHeader>
        <CardContent class="p-0">
          <div class="divide-y divide-stone-200 dark:divide-stone-700">
            <div v-for="entry in timeEntries" :key="entry.id" class="p-4 hover:bg-stone-50 dark:hover:bg-stone-700">
              <div class="flex items-center justify-between">
                <div class="flex items-center gap-4">
                  <div :class="['w-10 h-10 rounded-lg flex items-center justify-center', entry.bgColor]">
                    <Icon :name="entry.icon" :class="['w-5 h-5', entry.iconColor]" />
                  </div>
                  <div>
                    <p class="text-sm font-medium text-stone-900 dark:text-white">{{ entry.task }}</p>
                    <p class="text-xs text-stone-500 dark:text-stone-400">{{ entry.project }}</p>
                  </div>
                </div>
                <div class="text-right">
                  <p class="text-sm font-semibold text-stone-900 dark:text-white">{{ entry.duration }}</p>
                  <p class="text-xs text-stone-500 dark:text-stone-400">{{ entry.date }}</p>
                </div>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>

    <!-- Weekly Summary -->
    <Card class="mt-6 bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
      <CardHeader class="border-b border-stone-200 dark:border-stone-700">
        <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Weekly Summary</CardTitle>
      </CardHeader>
      <CardContent class="p-6">
        <div class="grid grid-cols-7 gap-4">
          <div v-for="day in weekDays" :key="day.name" class="text-center">
            <p class="text-xs font-medium text-stone-500 mb-2">{{ day.name }}</p>
            <div class="h-24 bg-stone-100 dark:bg-stone-900 rounded-lg flex flex-col justify-end overflow-hidden">
              <div :class="['bg-blue-500 rounded-t', day.isToday ? 'opacity-100' : 'opacity-70']" :style="`height: ${(day.hours / 8) * 100}%`"></div>
            </div>
            <p class="text-sm font-medium text-stone-900 dark:text-white mt-2">{{ day.hours }}h</p>
          </div>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { Card, CardContent, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'

definePageMeta({
  layout: 'default'
})

const timeEntries = ref([
  { id: 1, task: 'Configure POS terminals', project: 'POS System Upgrade', duration: '2h 15m', date: 'Today', icon: 'lucide:monitor', bgColor: 'bg-purple-100 dark:bg-purple-900/30', iconColor: 'text-purple-600' },
  { id: 2, task: 'Review supplier docs', project: 'Supplier Onboarding', duration: '1h 30m', date: 'Today', icon: 'lucide:file-text', bgColor: 'bg-green-100 dark:bg-green-900/30', iconColor: 'text-green-600' },
  { id: 3, task: 'Site survey planning', project: 'Store Expansion', duration: '3h 00m', date: 'Yesterday', icon: 'lucide:building', bgColor: 'bg-blue-100 dark:bg-blue-900/30', iconColor: 'text-blue-600' },
  { id: 4, task: 'Training curriculum', project: 'Staff Training', duration: '1h 45m', date: 'Yesterday', icon: 'lucide:graduation-cap', bgColor: 'bg-orange-100 dark:bg-orange-900/30', iconColor: 'text-orange-600' },
])

const weekDays = ref([
  { name: 'Mon', hours: 7.5, isToday: false },
  { name: 'Tue', hours: 8, isToday: false },
  { name: 'Wed', hours: 6.5, isToday: false },
  { name: 'Thu', hours: 7, isToday: false },
  { name: 'Fri', hours: 6.75, isToday: true },
  { name: 'Sat', hours: 0, isToday: false },
  { name: 'Sun', hours: 0, isToday: false },
])
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: hsl(var(--muted)); }
.custom-scrollbar::-webkit-scrollbar-thumb { background: hsl(var(--muted-foreground)); border-radius: 3px; }
</style>
