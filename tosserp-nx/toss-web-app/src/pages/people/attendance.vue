<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import Button from '@/components/ui/Button.vue'
import { Clock, Plus, Search, Filter, Calendar } from 'lucide-vue-next'
import { useHrApi } from '@/composables/useHrApi'

const { getEmployees, getAttendanceForPeriod, isLoading } = useHrApi()

const employees = ref<any[]>([])
const attendance = ref<any[]>([])
const selectedEmployeeId = ref<number | null>(null)
const fromDate = ref(new Date(new Date().getFullYear(), new Date().getMonth(), 1).toISOString().split('T')[0])
const toDate = ref(new Date().toISOString().split('T')[0])

const loadEmployees = async () => {
  try {
    const result = await getEmployees({ pageSize: 100 })
    employees.value = result.items
  } catch (error) {
    console.error('Failed to load employees:', error)
  }
}

const loadAttendance = async () => {
  if (!selectedEmployeeId.value) return
  
  try {
    const result = await getAttendanceForPeriod({
      employeeId: selectedEmployeeId.value,
      fromDate: new Date(fromDate.value).toISOString(),
      toDate: new Date(toDate.value + 'T23:59:59').toISOString()
    })
    attendance.value = result.items
  } catch (error) {
    console.error('Failed to load attendance:', error)
  }
}

onMounted(() => {
  loadEmployees()
})
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <Breadcrumbs />
        <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">Attendance</h1>
        <p class="text-muted-foreground mt-1">Track employee attendance and hours</p>
      </div>
      <Button>
        <Plus :size="18" class="mr-2" />
        Record Attendance
      </Button>
    </div>

    <!-- Filters -->
    <Card>
      <CardContent class="pt-6">
        <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div>
            <label class="block text-sm font-medium mb-2">Employee</label>
            <select
              v-model="selectedEmployeeId"
              @change="loadAttendance"
              class="w-full px-4 py-2 bg-background border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            >
              <option :value="null">All Employees</option>
              <option v-for="emp in employees" :key="emp.id" :value="emp.id">
                {{ emp.name }}
              </option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium mb-2">From Date</label>
            <input
              v-model="fromDate"
              @change="loadAttendance"
              type="date"
              class="w-full px-4 py-2 bg-background border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
          <div>
            <label class="block text-sm font-medium mb-2">To Date</label>
            <input
              v-model="toDate"
              @change="loadAttendance"
              type="date"
              class="w-full px-4 py-2 bg-background border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
          <div class="flex items-end">
            <Button @click="loadAttendance" class="w-full">
              <Search :size="18" class="mr-2" />
              Search
            </Button>
          </div>
        </div>
      </CardContent>
    </Card>

    <!-- Attendance List -->
    <Card>
      <CardHeader>
        <CardTitle>Attendance Records</CardTitle>
      </CardHeader>
      <CardContent>
        <div v-if="isLoading" class="text-center py-12 text-muted-foreground">
          Loading attendance...
        </div>
        <div v-else-if="attendance.length === 0" class="text-center py-12 text-muted-foreground">
          <Clock :size="48" class="mx-auto mb-3 opacity-50" />
          <p>No attendance records found</p>
          <p class="text-sm mt-1">Select an employee and date range to view attendance</p>
        </div>
        <div v-else class="space-y-2">
          <div
            v-for="record in attendance"
            :key="record.id"
            class="p-4 border rounded-lg hover:bg-accent/50 transition-colors"
          >
            <div class="flex items-center justify-between">
              <div>
                <div class="font-medium">
                  {{ new Date(record.attendanceDate).toLocaleDateString() }}
                </div>
                <div class="text-sm text-muted-foreground mt-1">
                  <span v-if="record.clockIn">
                    Clock In: {{ new Date(record.clockIn).toLocaleTimeString() }}
                  </span>
                  <span v-if="record.clockOut" class="ml-4">
                    Clock Out: {{ new Date(record.clockOut).toLocaleTimeString() }}
                  </span>
                </div>
              </div>
              <div class="text-right">
                <div v-if="record.hoursWorked" class="font-semibold">
                  {{ record.hoursWorked }} hours
                </div>
                <div v-else-if="record.daysWorked" class="font-semibold">
                  {{ record.daysWorked }} day{{ record.daysWorked !== 1 ? 's' : '' }}
                </div>
              </div>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

