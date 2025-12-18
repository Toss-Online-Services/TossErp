<template>
  <div class="container-fluid py-4">
    <!-- Header -->
    <div class="row mb-4">
      <div class="col-lg-8">
        <MDTypography variant="h3" font-weight="bold">
          All Projects
        </MDTypography>
        <MDTypography variant="body2" color="text" class="mt-1">
          Browse and manage all your projects
        </MDTypography>
      </div>
      <div class="col-lg-4 text-end">
        <MDButton color="primary" size="lg">
          <Icon name="mdi:plus" size="20" class="me-2" />
          New Project
        </MDButton>
      </div>
    </div>

    <!-- Filters -->
    <div class="row mb-4">
      <div class="col-lg-3 col-md-6 mb-3">
        <select v-model="filters.status" class="form-select">
          <option value="">All Status</option>
          <option value="active">Active</option>
          <option value="completed">Completed</option>
          <option value="on-hold">On Hold</option>
          <option value="cancelled">Cancelled</option>
        </select>
      </div>
      <div class="col-lg-3 col-md-6 mb-3">
        <select v-model="filters.category" class="form-select">
          <option value="">All Categories</option>
          <option value="web">Web Development</option>
          <option value="mobile">Mobile App</option>
          <option value="design">Design</option>
          <option value="marketing">Marketing</option>
        </select>
      </div>
      <div class="col-lg-4 col-md-8 mb-3">
        <input 
          v-model="filters.search" 
          type="text" 
          class="form-control" 
          placeholder="Search projects..."
        />
      </div>
      <div class="col-lg-2 col-md-4 mb-3">
        <select v-model="filters.sort" class="form-select">
          <option value="newest">Newest First</option>
          <option value="oldest">Oldest First</option>
          <option value="name">Name (A-Z)</option>
          <option value="progress">Progress</option>
        </select>
      </div>
    </div>

    <!-- Projects Grid -->
    <div class="row">
      <div 
        v-for="project in filteredProjects" 
        :key="project.id" 
        class="col-lg-4 col-md-6 mb-4"
      >
        <div class="card h-100">
          <div class="card-header p-3 pb-0">
            <div class="d-flex justify-content-between align-items-center mb-3">
              <MDBadge :color="getStatusColor(project.status)" variant="gradient">
                {{ project.status }}
              </MDBadge>
              <div class="dropdown">
                <button 
                  class="btn btn-link text-secondary p-0" 
                  type="button"
                  @click.stop="console.log('Open menu for', project.id)"
                >
                  <Icon name="mdi:dots-vertical" size="20" />
                </button>
              </div>
            </div>
            <MDTypography variant="h5" font-weight="bold" class="mb-2">
              {{ project.name }}
            </MDTypography>
            <MDTypography variant="caption" color="text">
              {{ project.category }}
            </MDTypography>
          </div>
          
          <div class="card-body p-3">
            <MDTypography variant="body2" color="text" class="mb-3">
              {{ project.description }}
            </MDTypography>
            
            <!-- Progress -->
            <div class="mb-3">
              <div class="d-flex justify-content-between align-items-center mb-1">
                <MDTypography variant="caption" color="text" font-weight="bold">
                  Progress
                </MDTypography>
                <MDTypography variant="caption" color="text" font-weight="bold">
                  {{ project.progress }}%
                </MDTypography>
              </div>
              <MDProgress :value="project.progress" :color="getProgressColor(project.progress)" />
            </div>
            
            <!-- Team -->
            <div class="d-flex justify-content-between align-items-center">
              <div class="d-flex align-items-center">
                <MDAvatar
                  v-for="(member, index) in project.team.slice(0, 3)"
                  :key="index"
                  :src="member.avatar"
                  :alt="member.name"
                  size="xs"
                  class="border border-white"
                  :style="{ marginLeft: index > 0 ? '-8px' : '0', zIndex: 3 - index }"
                />
                <MDTypography 
                  v-if="project.team.length > 3" 
                  variant="caption" 
                  color="text" 
                  class="ms-2"
                >
                  +{{ project.team.length - 3 }} more
                </MDTypography>
              </div>
              
              <div class="d-flex align-items-center">
                <Icon name="mdi:calendar" size="16" class="text-secondary me-1" />
                <MDTypography variant="caption" color="text">
                  {{ formatDate(project.deadline) }}
                </MDTypography>
              </div>
            </div>
          </div>
          
          <div class="card-footer p-3 pt-0">
            <div class="d-flex justify-content-between align-items-center">
              <div class="d-flex align-items-center">
                <Icon name="mdi:check-circle" size="18" class="text-success me-1" />
                <MDTypography variant="caption" color="text">
                  {{ project.completedTasks }}/{{ project.totalTasks }} tasks
                </MDTypography>
              </div>
              <NuxtLink 
                :to="`/pages/projects/project-${project.id}`"
                class="text-sm text-primary font-weight-bold"
              >
                View Project
                <Icon name="mdi:arrow-right" size="16" class="ms-1" />
              </NuxtLink>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-if="filteredProjects.length === 0" class="row">
      <div class="col-12">
        <div class="card text-center py-5">
          <div class="card-body">
            <Icon name="mdi:folder-open-outline" size="64" class="text-secondary mb-3" />
            <MDTypography variant="h5" font-weight="medium" class="mb-2">
              No projects found
            </MDTypography>
            <MDTypography variant="body2" color="text">
              Try adjusting your filters or create a new project
            </MDTypography>
            <MDButton color="primary" class="mt-3">
              <Icon name="mdi:plus" size="20" class="me-2" />
              Create Project
            </MDButton>
          </div>
        </div>
      </div>
    </div>

    <!-- Pagination -->
    <div v-if="filteredProjects.length > 0" class="row mt-4">
      <div class="col-12">
        <nav aria-label="Page navigation">
          <ul class="pagination justify-content-center">
            <li class="page-item">
              <a class="page-link" href="#" aria-label="Previous">
                <Icon name="mdi:chevron-left" size="18" />
              </a>
            </li>
            <li class="page-item active"><a class="page-link" href="#">1</a></li>
            <li class="page-item"><a class="page-link" href="#">2</a></li>
            <li class="page-item"><a class="page-link" href="#">3</a></li>
            <li class="page-item">
              <a class="page-link" href="#" aria-label="Next">
                <Icon name="mdi:chevron-right" size="18" />
              </a>
            </li>
          </ul>
        </nav>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

definePageMeta({
  layout: 'default'
})

interface TeamMember {
  name: string
  avatar: string
}

interface Project {
  id: number
  name: string
  description: string
  category: string
  status: 'active' | 'completed' | 'on-hold' | 'cancelled'
  progress: number
  team: TeamMember[]
  deadline: string
  completedTasks: number
  totalTasks: number
}

const filters = ref({
  status: '',
  category: '',
  search: '',
  sort: 'newest'
})

const projects = ref<Project[]>([
  {
    id: 1,
    name: 'E-Commerce Website',
    description: 'Modern e-commerce platform with advanced features and payment integration',
    category: 'Web Development',
    status: 'active',
    progress: 75,
    team: [
      { name: 'John Doe', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-1.jpg' },
      { name: 'Jane Smith', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-2.jpg' },
      { name: 'Mike Johnson', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-3.jpg' }
    ],
    deadline: '2024-03-15',
    completedTasks: 18,
    totalTasks: 24
  },
  {
    id: 2,
    name: 'Mobile Banking App',
    description: 'Secure mobile banking application with biometric authentication',
    category: 'Mobile App',
    status: 'active',
    progress: 60,
    team: [
      { name: 'Sarah Williams', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-4.jpg' },
      { name: 'Tom Brown', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-1.jpg' }
    ],
    deadline: '2024-04-20',
    completedTasks: 12,
    totalTasks: 20
  },
  {
    id: 3,
    name: 'Brand Identity Redesign',
    description: 'Complete brand identity refresh including logo, colors, and guidelines',
    category: 'Design',
    status: 'completed',
    progress: 100,
    team: [
      { name: 'Emma Davis', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-2.jpg' }
    ],
    deadline: '2024-01-30',
    completedTasks: 15,
    totalTasks: 15
  },
  {
    id: 4,
    name: 'Social Media Campaign',
    description: 'Multi-platform social media marketing campaign for product launch',
    category: 'Marketing',
    status: 'active',
    progress: 45,
    team: [
      { name: 'Alex Martinez', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-3.jpg' },
      { name: 'Lisa Anderson', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-4.jpg' },
      { name: 'Chris Taylor', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-1.jpg' },
      { name: 'Nina Wilson', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-2.jpg' }
    ],
    deadline: '2024-02-28',
    completedTasks: 9,
    totalTasks: 20
  },
  {
    id: 5,
    name: 'CRM System Integration',
    description: 'Integration of new CRM system with existing business tools',
    category: 'Web Development',
    status: 'on-hold',
    progress: 30,
    team: [
      { name: 'David Lee', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-3.jpg' }
    ],
    deadline: '2024-05-10',
    completedTasks: 6,
    totalTasks: 20
  },
  {
    id: 6,
    name: 'Fitness Tracking App',
    description: 'Mobile app for tracking workouts, nutrition, and health metrics',
    category: 'Mobile App',
    status: 'active',
    progress: 85,
    team: [
      { name: 'Rachel Green', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-4.jpg' },
      { name: 'Ross Geller', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-1.jpg' }
    ],
    deadline: '2024-02-15',
    completedTasks: 17,
    totalTasks: 20
  },
  {
    id: 7,
    name: 'UI Component Library',
    description: 'Comprehensive design system with reusable UI components',
    category: 'Design',
    status: 'active',
    progress: 70,
    team: [
      { name: 'Monica Bing', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-2.jpg' },
      { name: 'Chandler Bing', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-3.jpg' }
    ],
    deadline: '2024-03-30',
    completedTasks: 14,
    totalTasks: 20
  },
  {
    id: 8,
    name: 'SEO Optimization',
    description: 'Complete SEO audit and optimization for improved search rankings',
    category: 'Marketing',
    status: 'completed',
    progress: 100,
    team: [
      { name: 'Joey Tribbiani', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-4.jpg' }
    ],
    deadline: '2024-01-15',
    completedTasks: 12,
    totalTasks: 12
  },
  {
    id: 9,
    name: 'Cloud Migration',
    description: 'Migration of infrastructure to cloud-based services',
    category: 'Web Development',
    status: 'on-hold',
    progress: 20,
    team: [
      { name: 'Phoebe Buffay', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-1.jpg' },
      { name: 'Mike Hannigan', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-2.jpg' }
    ],
    deadline: '2024-06-30',
    completedTasks: 4,
    totalTasks: 20
  }
])

const filteredProjects = computed(() => {
  let result = [...projects.value]
  
  // Filter by status
  if (filters.value.status) {
    result = result.filter(p => p.status === filters.value.status)
  }
  
  // Filter by category
  if (filters.value.category) {
    result = result.filter(p => p.category.toLowerCase().includes(filters.value.category))
  }
  
  // Filter by search
  if (filters.value.search) {
    const search = filters.value.search.toLowerCase()
    result = result.filter(p => 
      p.name.toLowerCase().includes(search) || 
      p.description.toLowerCase().includes(search)
    )
  }
  
  // Sort
  switch (filters.value.sort) {
    case 'oldest':
      result.sort((a, b) => a.id - b.id)
      break
    case 'name':
      result.sort((a, b) => a.name.localeCompare(b.name))
      break
    case 'progress':
      result.sort((a, b) => b.progress - a.progress)
      break
    case 'newest':
    default:
      result.sort((a, b) => b.id - a.id)
  }
  
  return result
})

const getStatusColor = (status: string): string => {
  const colors: Record<string, string> = {
    'active': 'success',
    'completed': 'info',
    'on-hold': 'warning',
    'cancelled': 'error'
  }
  return colors[status] || 'secondary'
}

const getProgressColor = (progress: number): string => {
  if (progress >= 80) return 'success'
  if (progress >= 50) return 'info'
  if (progress >= 30) return 'warning'
  return 'error'
}

const formatDate = (dateString: string): string => {
  const date = new Date(dateString)
  const options: Intl.DateTimeFormatOptions = { month: 'short', day: 'numeric', year: 'numeric' }
  return date.toLocaleDateString('en-US', options)
}
</script>

<style scoped>
.card {
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06);
  border: none;
  border-radius: 0.75rem;
  transition: transform 0.2s, box-shadow 0.2s;
}

.card:hover {
  transform: translateY(-4px);
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
}

.card-header {
  background: transparent;
  border-bottom: none;
}

.card-footer {
  background: transparent;
  border-top: 1px solid rgba(0, 0, 0, 0.05);
}

.form-select,
.form-control {
  border: 1px solid #d2d6da;
  border-radius: 0.5rem;
  padding: 0.5rem 0.75rem;
  font-size: 0.875rem;
}

.form-select:focus,
.form-control:focus {
  border-color: #5e72e4;
  box-shadow: 0 0 0 0.2rem rgba(94, 114, 228, 0.25);
  outline: none;
}

.pagination {
  margin: 0;
}

.page-link {
  border: none;
  color: #67748e;
  padding: 0.5rem 0.75rem;
  margin: 0 0.25rem;
  border-radius: 0.375rem;
}

.page-link:hover {
  background-color: #f0f2f5;
  color: #344767;
}

.page-item.active .page-link {
  background-color: #5e72e4;
  color: white;
}
</style>
