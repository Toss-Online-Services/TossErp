<template>
  <div class="h-full overflow-y-auto p-6 custom-scrollbar">
    <!-- Page Header -->
    <div class="flex items-center justify-between mb-8">
      <div>
        <h1 class="text-3xl font-bold text-stone-900 dark:text-white mb-2">Attendance</h1>
        <p class="text-stone-500 dark:text-stone-400">Track employee attendance and work hours</p>
      </div>
      <div class="flex gap-2">
        <Button variant="outline">
          <Icon name="lucide:calendar" class="w-5 h-5 mr-2" />
          Dec 2024
        </Button>
        <Button class="bg-stone-900 hover:bg-stone-800 dark:bg-white dark:hover:bg-stone-100 dark:text-stone-900">
          <Icon name="lucide:download" class="w-5 h-5 mr-2" />
          Export
        </Button>
      </div>
    </div>

    <!-- Stats -->
    <div class="grid grid-cols-1 md:grid-cols-5 gap-4 mb-8">
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-4">
          <p class="text-sm font-medium text-stone-500">Present Today</p>
          <p class="text-2xl font-bold text-green-600">42</p>
        </CardContent>
      </Card>
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-4">
          <p class="text-sm font-medium text-stone-500">Late</p>
          <p class="text-2xl font-bold text-yellow-600">3</p>
        </CardContent>
      </Card>
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-4">
          <p class="text-sm font-medium text-stone-500">Absent</p>
          <p class="text-2xl font-bold text-red-600">2</p>
        </CardContent>
      </Card>
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-4">
          <p class="text-sm font-medium text-stone-500">On Leave</p>
          <p class="text-2xl font-bold text-blue-600">1</p>
        </CardContent>
      </Card>
      <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
        <CardContent class="p-4">
          <p class="text-sm font-medium text-stone-500">Attendance Rate</p>
          <p class="text-2xl font-bold text-stone-900 dark:text-white">93%</p>
        </CardContent>
      </Card>
    </div>

    <!-- Attendance Table -->
    <Card class="bg-white dark:bg-stone-800 border border-stone-200 dark:border-stone-700">
      <CardHeader class="border-b border-stone-200 dark:border-stone-700">
        <div class="flex items-center justify-between">
          <CardTitle class="text-lg font-semibold text-stone-900 dark:text-white">Today's Attendance</CardTitle>
          <div class="relative">
            <Icon name="lucide:search" class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-stone-400" />
            <input type="text" placeholder="Search..." class="pl-9 pr-4 py-2 text-sm border border-stone-200 dark:border-stone-700 rounded-lg bg-white dark:bg-stone-900 text-stone-900 dark:text-white focus:outline-none focus:ring-2 focus:ring-stone-500" />
          </div>
        </div>
      </CardHeader>
      <CardContent class="p-0">
        <div class="overflow-x-auto">
          <table class="w-full">
            <thead class="bg-stone-50 dark:bg-stone-900">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Employee</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Department</th>
                <th class="px-6 py-3 text-center text-xs font-normal text-stone-500 uppercase">Clock In</th>
                <th class="px-6 py-3 text-center text-xs font-normal text-stone-500 uppercase">Clock Out</th>
                <th class="px-6 py-3 text-center text-xs font-normal text-stone-500 uppercase">Hours</th>
                <th class="px-6 py-3 text-left text-xs font-normal text-stone-500 uppercase">Status</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-stone-200 dark:divide-stone-700">
              <tr v-for="record in attendanceRecords" :key="record.id" class="hover:bg-stone-50 dark:hover:bg-stone-700">
                <td class="px-6 py-4">
                  <div class="flex items-center gap-3">
                    <div class="w-8 h-8 rounded-full bg-stone-200 dark:bg-stone-700 flex items-center justify-center">
                      <span class="text-xs font-medium text-stone-600 dark:text-stone-300">{{ record.initials }}</span>
                    </div>
                    <span class="text-sm font-medium text-stone-900 dark:text-white">{{ record.name }}</span>
                  </div>
                </td>
                <td class="px-6 py-4 text-sm text-stone-600 dark:text-stone-400">{{ record.department }}</td>
                <td class="px-6 py-4 text-sm text-center text-stone-900 dark:text-white">{{ record.clockIn || '-' }}</td>
                <td class="px-6 py-4 text-sm text-center text-stone-900 dark:text-white">{{ record.clockOut || '-' }}</td>
                <td class="px-6 py-4 text-sm text-center font-medium text-stone-900 dark:text-white">{{ record.hours || '-' }}</td>
                <td class="px-6 py-4">
                  <Badge :class="getStatusBadgeClass(record.status)">{{ record.status }}</Badge>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

<script setup lang="ts">
import { Card, CardContent, CardHeader, CardTitle } from '~/components/ui/card'
import { Button } from '~/components/ui/button'
import { Badge } from '~/components/ui/badge'

definePageMeta({
  layout: 'default'
})

const attendanceRecords = ref([
  { id: 1, name: 'Nomvula Mbeki', initials: 'NM', department: 'Sales', clockIn: '07:55', clockOut: '17:02', hours: '9h 07m', status: 'present' },
  { id: 2, name: 'Thabo Molefe', initials: 'TM', department: 'Logistics', clockIn: '08:15', clockOut: '17:30', hours: '9h 15m', status: 'late' },
  { id: 3, name: 'Lindiwe Nkosi', initials: 'LN', department: 'Finance', clockIn: '07:45', clockOut: null, hours: null, status: 'present' },
  { id: 4, name: 'Sipho Dlamini', initials: 'SD', department: 'Operations', clockIn: null, clockOut: null, hours: null, status: 'on-leave' },
  { id: 5, name: 'Zanele Mthembu', initials: 'ZM', department: 'Sales', clockIn: '08:00', clockOut: null, hours: null, status: 'present' },
  { id: 6, name: 'Mandla Khumalo', initials: 'MK', department: 'Logistics', clockIn: null, clockOut: null, hours: null, status: 'absent' },
])

const getStatusBadgeClass = (status: string) => {
  const classes: Record<string, string> = {
    'present': 'bg-green-100 text-green-800',
    'late': 'bg-yellow-100 text-yellow-800',
    'absent': 'bg-red-100 text-red-800',
    'on-leave': 'bg-blue-100 text-blue-800',
  }
  return classes[status] || 'bg-stone-100 text-stone-800'
}
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: hsl(var(--muted)); }
.custom-scrollbar::-webkit-scrollbar-thumb { background: hsl(var(--muted-foreground)); border-radius: 3px; }
</style>
