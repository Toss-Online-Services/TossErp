<script setup lang="ts">
definePageMeta({
  middleware: 'auth',
  layout: 'default'
})

const payrollPeriod = ref('December 2025')
const payrollEntries = ref([
  { id: 'EMP-001', name: 'Thabo Mokoena', basicSalary: 8500, deductions: 850, netPay: 7650, status: 'processed' },
  { id: 'EMP-002', name: 'Sipho Khumalo', basicSalary: 6500, deductions: 650, netPay: 5850, status: 'processed' },
  { id: 'EMP-003', name: 'Mary Sithole', basicSalary: 7000, deductions: 700, netPay: 6300, status: 'pending' },
  { id: 'EMP-004', name: 'John Dlamini', basicSalary: 7500, deductions: 750, netPay: 6750, status: 'pending' }
])

const columns = [
  { key: 'id', label: 'Employee ID' },
  { key: 'name', label: 'Name' },
  { key: 'basicSalary', label: 'Basic Salary' },
  { key: 'deductions', label: 'Deductions' },
  { key: 'netPay', label: 'Net Pay' },
  { key: 'status', label: 'Status' },
  { key: 'actions', label: 'Actions' }
]

const totalPayroll = computed(() => {
  return payrollEntries.value.reduce((sum, entry) => sum + entry.netPay, 0)
})
</script>

<template>
  <div class="p-4 md:p-6">
    <div class="mb-6">
      <h1 class="text-2xl font-bold mb-2">Payroll</h1>
      <p class="text-muted-foreground">Process and manage employee payroll</p>
    </div>

    <!-- Payroll Summary -->
    <div class="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
      <UCard>
        <div>
          <p class="text-sm text-muted-foreground">Payroll Period</p>
          <p class="text-2xl font-bold">{{ payrollPeriod }}</p>
        </div>
      </UCard>
      <UCard>
        <div>
          <p class="text-sm text-muted-foreground">Total Payroll</p>
          <p class="text-2xl font-bold">R{{ totalPayroll.toLocaleString() }}</p>
        </div>
      </UCard>
      <UCard>
        <div>
          <p class="text-sm text-muted-foreground">Employees</p>
          <p class="text-2xl font-bold">{{ payrollEntries.length }}</p>
        </div>
      </UCard>
    </div>

    <!-- Actions -->
    <div class="mb-4 flex gap-2">
      <UButton>
        <UIcon name="i-heroicons-calculator" class="mr-2" />
        Process Payroll
      </UButton>
      <UButton variant="outline">
        <UIcon name="i-heroicons-document-arrow-down" class="mr-2" />
        Download Payslips
      </UButton>
    </div>

    <!-- Payroll Table -->
    <UCard>
      <UTable :rows="payrollEntries" :columns="columns">
        <template #basicSalary-data="{ row }">
          <span class="font-medium">R{{ row.basicSalary.toLocaleString() }}</span>
        </template>

        <template #deductions-data="{ row }">
          <span class="text-error">-R{{ row.deductions.toLocaleString() }}</span>
        </template>

        <template #netPay-data="{ row }">
          <span class="font-bold text-success">R{{ row.netPay.toLocaleString() }}</span>
        </template>

        <template #status-data="{ row }">
          <UBadge :color="row.status === 'processed' ? 'success' : 'warning'">
            {{ row.status }}
          </UBadge>
        </template>

        <template #actions-data="{ row }">
          <div class="flex gap-2">
            <UButton size="xs" variant="ghost">View Payslip</UButton>
            <UButton v-if="row.status === 'pending'" size="xs" variant="ghost">Process</UButton>
          </div>
        </template>
      </UTable>
    </UCard>
  </div>
</template>
