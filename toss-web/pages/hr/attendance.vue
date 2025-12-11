<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useHrStore, type Attendance } from '~/stores/hr'
import AttendanceModal from '~/components/hr/AttendanceModal.vue'

useHead({ title: 'Attendance - TOSS' })

const hrStore = useHrStore()
const showModal = ref(false)
const selectedEntry = ref<Attendance | null>(null)
const selectedEmployeeId = ref<number | null>(null)
const dateFilter = ref<string>('')

const filteredAttendance = computed(() => {
  let filtered = hrStore.attendanceRecords

  // Filter by employee
  if (selectedEmployeeId.value) {
    filtered = filtered.filter(a => a.employeeId === selectedEmployeeId.value)
  }

  // Filter by date
  if (dateFilter.value) {
    const filterDate = new Date(dateFilter.value)
    filtered = filtered.filter(a => {
      const attDate = new Date(a.attendanceDate)
      return attDate.toDateString() === filterDate.toDateString()
    })
  }

  return filtered.sort((a, b) => 
    new Date(b.attendanceDate).getTime() - new Date(a.attendanceDate).getTime()
  )
})

function openCreateModal(employeeId?: number) {
  selectedEntry.value = null
  selectedEmployeeId.value = employeeId || null
  showModal.value = true
}

function handleSaved() {
  showModal.value = false
  selectedEntry.value = null
  hrStore.fetchAttendance()
}

function formatDate(date: Date) {
  return new Intl.DateTimeFormat('en-ZA', {
    day: '2-digit',
    month: 'short',
    year: 'numeric'
  }).format(new Date(date))
}

function formatTime(date: Date | undefined) {
  if (!date) return '-'
  return new Intl.DateTimeFormat('en-ZA', {
    hour: '2-digit',
    minute: '2-digit'
  }).format(new Date(date))
}

onMounted(() => {
  hrStore.fetchEmployees()
  hrStore.fetchAttendance()
})
</script>

<template>
  <div class="py-6">
    <!-- Header -->
    <div class="mb-8 flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
      <div>
        <h3 class="text-3xl font-bold text-gray-900 mb-2">Attendance</h3>
        <p class="text-gray-600 text-sm">Track employee attendance and leave</p>
      </div>
      <button
        @click="openCreateModal()"
        class="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 flex items-center gap-2 transition-colors"
      >
        <i class="material-symbols-rounded text-xl">add</i>
        <span>Record Attendance</span>
      </button>
    </div>

    <!-- Stats Cards -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Total Hours</p>
            <p class="text-2xl font-bold text-gray-900">{{ hrStore.totalHoursWorked.toFixed(1) }}h</p>
          </div>
          <div class="w-12 h-12 bg-blue-100 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-blue-600 text-2xl">schedule</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Records</p>
            <p class="text-2xl font-bold text-gray-900">{{ hrStore.attendanceRecords.length }}</p>
          </div>
          <div class="w-12 h-12 bg-green-100 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-green-600 text-2xl">event_available</i>
          </div>
        </div>
      </div>

      <div class="bg-white rounded-xl shadow-sm p-6">
        <div class="flex items-center justify-between">
          <div>
            <p class="text-sm text-gray-600 mb-1">Active Employees</p>
            <p class="text-2xl font-bold text-gray-900">{{ hrStore.activeEmployees.length }}</p>
          </div>
          <div class="w-12 h-12 bg-purple-100 rounded-lg flex items-center justify-center">
            <i class="material-symbols-rounded text-purple-600 text-2xl">badge</i>
          </div>
        </div>
      </div>
    </div>

    <!-- Filters -->
    <div class="bg-white rounded-xl shadow-sm p-4 mb-6">
      <div class="flex flex-col sm:flex-row gap-4">
        <div class="sm:w-64">
          <select
            v-model="selectedEmployeeId"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          >
            <option :value="null">All Employees</option>
            <option
              v-for="employee in hrStore.activeEmployees"
              :key="employee.id"
              :value="employee.id"
            >
              {{ employee.name }}
            </option>
          </select>
        </div>
        <div class="sm:w-48">
          <input
            v-model="dateFilter"
            type="date"
            class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
          />
        </div>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
      <button
        v-for="employee in hrStore.activeEmployees.slice(0, 3)"
        :key="employee.id"
        @click="openCreateModal(employee.id)"
        class="bg-white rounded-xl shadow-sm p-4 hover:shadow-md transition-shadow text-left"
      >
        <div class="flex items-center justify-between">
          <div>
            <p class="font-semibold text-gray-900">{{ employee.name }}</p>
            <p class="text-sm text-gray-600">Quick clock in/out</p>
          </div>
          <i class="material-symbols-rounded text-blue-600">schedule</i>
        </div>
      </button>
    </div>

    <!-- Attendance List -->
    <div v-if="hrStore.loading" class="text-center py-12">
      <i class="material-symbols-rounded text-6xl text-gray-400 animate-spin">refresh</i>
      <p class="text-gray-600 mt-4">Loading attendance...</p>
    </div>

    <div v-else-if="filteredAttendance.length === 0" class="bg-white rounded-xl shadow-sm p-12 text-center">
      <i class="material-symbols-rounded text-6xl text-gray-400 mb-4">event_available</i>
      <h3 class="text-xl font-semibold text-gray-900 mb-2">No Attendance Records</h3>
      <p class="text-gray-600 mb-6">Start tracking attendance by recording your first entry</p>
      <button
        @click="openCreateModal()"
        class="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700"
      >
        Record Attendance
      </button>
    </div>

    <div v-else class="bg-white rounded-xl shadow-sm overflow-hidden">
      <div class="overflow-x-auto">
        <table class="w-full">
          <thead class="bg-gray-50 border-b border-gray-200">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Date</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Employee</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Clock In</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Clock Out</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Hours</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Days</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Notes</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr
              v-for="entry in filteredAttendance"
              :key="entry.id"
              class="hover:bg-gray-50 transition-colors"
            >
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ formatDate(entry.attendanceDate) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ entry.employeeName || hrStore.getEmployeeById(entry.employeeId)?.name || `Employee #${entry.employeeId}` }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ formatTime(entry.clockIn) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ formatTime(entry.clockOut) }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ entry.hoursWorked ? `${entry.hoursWorked.toFixed(1)}h` : '-' }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                {{ entry.daysWorked || '-' }}
              </td>
              <td class="px-6 py-4 text-sm text-gray-600">
                {{ entry.notes || '-' }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Attendance Modal -->
    <AttendanceModal
      v-if="showModal"
      :show="showModal"
      :entry="selectedEntry"
      :employee-id="selectedEmployeeId"
      @close="showModal = false"
      @saved="handleSaved"
    />
  </div>
</template>
