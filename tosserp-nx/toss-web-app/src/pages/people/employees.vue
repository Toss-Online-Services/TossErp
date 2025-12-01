<script setup lang="ts">
import { ref, onMounted } from 'vue'
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import Button from '@/components/ui/Button.vue'
import { Briefcase, Plus, Search, Filter, Phone, Mail, CheckCircle2, XCircle } from 'lucide-vue-next'
import { useHrApi } from '@/composables/useHrApi'

const { getEmployees, isLoading } = useHrApi()

const employees = ref<any[]>([])
const searchQuery = ref('')
const showActiveOnly = ref(false)
const currentPage = ref(1)
const totalCount = ref(0)

const loadEmployees = async () => {
  try {
    const result = await getEmployees({
      searchTerm: searchQuery.value || undefined,
      isActive: showActiveOnly ? true : undefined,
      pageNumber: currentPage.value,
      pageSize: 20
    })
    employees.value = result.items
    totalCount.value = result.totalCount
  } catch (error) {
    console.error('Failed to load employees:', error)
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
        <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">Employees</h1>
        <p class="text-muted-foreground mt-1">Manage staff and employee information</p>
      </div>
      <Button>
        <Plus :size="18" class="mr-2" />
        Add Employee
      </Button>
    </div>

    <!-- Filters -->
    <Card>
      <CardContent class="pt-6">
        <div class="flex flex-col md:flex-row gap-4">
          <div class="flex-1 relative">
            <Search
              :size="18"
              class="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground"
            />
            <input
              v-model="searchQuery"
              @input="loadEmployees"
              type="text"
              placeholder="Search employees by name, email, or phone..."
              class="w-full pl-10 pr-4 py-2 bg-background border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            />
          </div>
          <Button
            variant="outline"
            :class="{ 'bg-primary/10 border-primary': showActiveOnly }"
            @click="showActiveOnly = !showActiveOnly; loadEmployees()"
          >
            <Filter :size="18" class="mr-2" />
            Active Only
          </Button>
        </div>
      </CardContent>
    </Card>

    <!-- Employees List -->
    <Card>
      <CardHeader>
        <CardTitle>Employees ({{ totalCount }})</CardTitle>
      </CardHeader>
      <CardContent>
        <div v-if="isLoading" class="text-center py-12 text-muted-foreground">
          Loading employees...
        </div>
        <div v-else-if="employees.length === 0" class="text-center py-12 text-muted-foreground">
          <Briefcase :size="48" class="mx-auto mb-3 opacity-50" />
          <p>No employees found</p>
          <p class="text-sm mt-1">Add your first employee to get started</p>
        </div>
        <div v-else class="space-y-3">
          <div
            v-for="employee in employees"
            :key="employee.id"
            class="p-4 border rounded-lg hover:bg-accent/50 transition-colors cursor-pointer"
          >
            <div class="flex items-start justify-between">
              <div class="flex-1">
                <div class="flex items-center gap-2">
                  <div class="font-medium text-lg">{{ employee.name }}</div>
                  <component
                    :is="employee.isActive ? CheckCircle2 : XCircle"
                    :size="16"
                    :class="employee.isActive ? 'text-emerald-600' : 'text-red-600'"
                  />
                </div>
                <div class="flex items-center gap-4 mt-2 text-sm text-muted-foreground">
                  <div v-if="employee.phone" class="flex items-center gap-1">
                    <Phone :size="14" />
                    {{ employee.phone }}
                  </div>
                  <div v-if="employee.email" class="flex items-center gap-1">
                    <Mail :size="14" />
                    {{ employee.email }}
                  </div>
                </div>
                <div class="mt-2 text-sm">
                  <span class="text-muted-foreground">Rate:</span>
                  <span class="font-semibold ml-1">
                    R{{ employee.rate.toFixed(2) }} / {{ employee.rateType.toLowerCase() }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  </div>
</template>

