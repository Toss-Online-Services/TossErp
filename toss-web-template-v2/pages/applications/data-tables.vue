<template>
  <div class="container-fluid py-4">
    <div class="row">
      <div class="col-12">
        <div class="card">
          <div class="card-header pb-0">
            <div class="d-flex justify-content-between align-items-center">
              <MDTypography variant="h6" font-weight="medium">
                Data Tables
              </MDTypography>
              <div class="d-flex gap-2">
                <MDInput 
                  v-model="search" 
                  placeholder="Search..."
                  size="small"
                  style="width: 250px;"
                />
                <MDButton color="info" size="small">
                  <Icon name="mdi:plus" class="me-1" />
                  Add New
                </MDButton>
              </div>
            </div>
          </div>
          <div class="card-body px-0 pt-0 pb-2">
            <div class="table-responsive p-0">
              <table class="table align-items-center mb-0">
                <thead>
                  <tr>
                    <th class="text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">
                      Author
                    </th>
                    <th class="text-uppercase text-secondary text-xxs font-weight-bolder opacity-7 ps-2">
                      Function
                    </th>
                    <th class="text-center text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">
                      Status
                    </th>
                    <th class="text-center text-uppercase text-secondary text-xxs font-weight-bolder opacity-7">
                      Employed
                    </th>
                    <th class="text-secondary opacity-7"></th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="employee in filteredEmployees" :key="employee.id">
                    <td>
                      <div class="d-flex px-2 py-1">
                        <div>
                          <MDAvatar :src="employee.avatar" alt="user" size="sm" class="me-3" />
                        </div>
                        <div class="d-flex flex-column justify-content-center">
                          <MDTypography variant="button" font-weight="medium">
                            {{ employee.name }}
                          </MDTypography>
                          <MDTypography variant="caption" color="text">
                            {{ employee.email }}
                          </MDTypography>
                        </div>
                      </div>
                    </td>
                    <td>
                      <MDTypography variant="caption" color="text" font-weight="medium">
                        {{ employee.function }}
                      </MDTypography>
                      <br>
                      <MDTypography variant="caption" color="text">
                        {{ employee.organization }}
                      </MDTypography>
                    </td>
                    <td class="align-middle text-center text-sm">
                      <MDBadge 
                        :color="employee.status === 'online' ? 'success' : 'secondary'" 
                        variant="gradient" 
                        size="sm"
                      >
                        {{ employee.status }}
                      </MDBadge>
                    </td>
                    <td class="align-middle text-center">
                      <MDTypography variant="caption" color="text" font-weight="medium">
                        {{ employee.employed }}
                      </MDTypography>
                    </td>
                    <td class="align-middle">
                      <MDButton color="text" size="small" icon-only>
                        <Icon name="mdi:pencil" size="16" />
                      </MDButton>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
            <div class="d-flex justify-content-between align-items-center px-3 mt-3">
              <MDTypography variant="caption" color="text">
                Showing {{ filteredEmployees.length }} of {{ employees.length }} entries
              </MDTypography>
              <div class="d-flex gap-1">
                <MDButton 
                  v-for="page in 3" 
                  :key="page" 
                  :color="page === 1 ? 'info' : 'light'"
                  size="small"
                  circular
                >
                  {{ page }}
                </MDButton>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

definePageMeta({
  layout: 'default'
})

const search = ref('')

const employees = ref([
  {
    id: 1,
    name: 'John Michael',
    email: 'john@creative-tim.com',
    avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-1.jpg',
    function: 'Manager',
    organization: 'Organization',
    status: 'online',
    employed: '23/04/18'
  },
  {
    id: 2,
    name: 'Alexa Liras',
    email: 'alexa@creative-tim.com',
    avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-2.jpg',
    function: 'Programator',
    organization: 'Developer',
    status: 'offline',
    employed: '11/01/19'
  },
  {
    id: 3,
    name: 'Laurent Perrier',
    email: 'laurent@creative-tim.com',
    avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-3.jpg',
    function: 'Executive',
    organization: 'Projects',
    status: 'online',
    employed: '19/09/17'
  },
  {
    id: 4,
    name: 'Michael Levi',
    email: 'michael@creative-tim.com',
    avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-4.jpg',
    function: 'Programator',
    organization: 'Developer',
    status: 'online',
    employed: '24/12/08'
  },
  {
    id: 5,
    name: 'Richard Gran',
    email: 'richard@creative-tim.com',
    avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-5.jpg',
    function: 'Manager',
    organization: 'Executive',
    status: 'offline',
    employed: '04/10/21'
  },
  {
    id: 6,
    name: 'Miriam Eric',
    email: 'miriam@creative-tim.com',
    avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-6.jpg',
    function: 'Programator',
    organization: 'Developer',
    status: 'offline',
    employed: '14/09/20'
  }
])

const filteredEmployees = computed(() => {
  if (!search.value) return employees.value
  const searchLower = search.value.toLowerCase()
  return employees.value.filter(emp => 
    emp.name.toLowerCase().includes(searchLower) || 
    emp.email.toLowerCase().includes(searchLower) ||
    emp.function.toLowerCase().includes(searchLower)
  )
})
</script>

<style scoped>
.gap-2 {
  gap: 0.5rem;
}

.gap-1 {
  gap: 0.25rem;
}
</style>
