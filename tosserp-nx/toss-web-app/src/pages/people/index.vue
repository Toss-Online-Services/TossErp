<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardTitle from '@/components/ui/CardTitle.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Breadcrumbs from '@/components/ui/Breadcrumbs.vue'
import KpiCard from '@/components/ui/KpiCard.vue'
import Button from '@/components/ui/Button.vue'
import { Users, UserPlus, Briefcase, Clock, DollarSign, ArrowRight } from 'lucide-vue-next'
import { useCrmApi } from '@/composables/useCrmApi'
import { useHrApi } from '@/composables/useHrApi'

const { getCustomers, isLoading: isCrmLoading } = useCrmApi()
const { getEmployees, isLoading: isHrLoading } = useHrApi()

const totalCustomers = ref(0)
const totalEmployees = ref(0)
const activeEmployees = ref(0)

const loadData = async () => {
  try {
    const [customersData, employeesData] = await Promise.all([
      getCustomers({ pageSize: 1 }),
      getEmployees({ pageSize: 100 })
    ])
    totalCustomers.value = customersData.totalCount
    totalEmployees.value = employeesData.totalCount
    activeEmployees.value = employeesData.items.filter(e => e.isActive).length
  } catch (error) {
    console.error('Failed to load people data:', error)
  }
}

onMounted(() => {
  loadData()
})
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <Breadcrumbs />
        <h1 class="text-2xl md:text-3xl font-bold tracking-tight mt-2">People</h1>
        <p class="text-muted-foreground mt-1">Manage customers and staff</p>
      </div>
      <Button>
        <UserPlus :size="18" class="mr-2" />
        Add Customer
      </Button>
    </div>

    <!-- KPIs -->
    <div class="grid gap-4 md:grid-cols-3">
      <KpiCard
        title="Total Customers"
        :value="totalCustomers"
        :icon="Users"
        status="neutral"
      />
      <KpiCard
        title="Total Employees"
        :value="totalEmployees"
        :icon="Briefcase"
        status="neutral"
      />
      <KpiCard
        title="Active Employees"
        :value="activeEmployees"
        :icon="Briefcase"
        status="good"
      />
    </div>

    <!-- Quick Navigation -->
    <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
      <NuxtLink to="/people/customers" class="block">
        <Card class="hover:shadow-material-md transition-shadow cursor-pointer">
          <CardHeader>
            <CardTitle class="flex items-center gap-2">
              <Users class="h-5 w-5 text-primary" />
              Customers
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p class="text-sm text-muted-foreground">
              Manage customer relationships and interactions
            </p>
          </CardContent>
        </Card>
      </NuxtLink>

      <NuxtLink to="/people/employees" class="block">
        <Card class="hover:shadow-material-md transition-shadow cursor-pointer">
          <CardHeader>
            <CardTitle class="flex items-center gap-2">
              <Briefcase class="h-5 w-5 text-primary" />
              Employees
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p class="text-sm text-muted-foreground">
              Manage staff and employee information
            </p>
          </CardContent>
        </Card>
      </NuxtLink>

      <NuxtLink to="/people/attendance" class="block">
        <Card class="hover:shadow-material-md transition-shadow cursor-pointer">
          <CardHeader>
            <CardTitle class="flex items-center gap-2">
              <Clock class="h-5 w-5 text-primary" />
              Attendance
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p class="text-sm text-muted-foreground">
              Track employee attendance and hours
            </p>
          </CardContent>
        </Card>
      </NuxtLink>

      <NuxtLink to="/people/payroll" class="block">
        <Card class="hover:shadow-material-md transition-shadow cursor-pointer">
          <CardHeader>
            <CardTitle class="flex items-center gap-2">
              <DollarSign class="h-5 w-5 text-primary" />
              Payroll
            </CardTitle>
          </CardHeader>
          <CardContent>
            <p class="text-sm text-muted-foreground">
              Run payroll and view payment history
            </p>
          </CardContent>
        </Card>
      </NuxtLink>
    </div>
  </div>
</template>

