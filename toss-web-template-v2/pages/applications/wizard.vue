<template>
  <div class="container-fluid py-4">
    <div class="row justify-content-center">
      <div class="col-lg-10">
        <div class="card">
          <div class="card-header pb-0">
            <MDTypography variant="h4" font-weight="bold" class="text-center">
              Create New Project
            </MDTypography>
            <MDTypography variant="body2" color="text" class="text-center">
              Follow the steps to set up your project
            </MDTypography>
          </div>
          <div class="card-body">
            <!-- Progress Steps -->
            <div class="row">
              <div class="col-12 mb-4">
                <div class="wizard-steps">
                  <div 
                    v-for="(step, index) in steps" 
                    :key="index"
                    class="wizard-step"
                    :class="{
                      'active': currentStep === index + 1,
                      'completed': currentStep > index + 1
                    }"
                  >
                    <div class="step-number">
                      <Icon 
                        v-if="currentStep > index + 1"
                        name="mdi:check" 
                        size="20"
                        class="text-white"
                      />
                      <span v-else>{{ index + 1 }}</span>
                    </div>
                    <MDTypography 
                      variant="button" 
                      font-weight="medium"
                      :color="currentStep >= index + 1 ? 'dark' : 'text'"
                    >
                      {{ step.title }}
                    </MDTypography>
                  </div>
                </div>
              </div>
            </div>

            <!-- Step Content -->
            <form @submit.prevent="handleNext">
              <!-- Step 1: Project Info -->
              <div v-show="currentStep === 1" class="step-content">
                <div class="row">
                  <div class="col-12 mb-3">
                    <MDInput
                      v-model="formData.projectName"
                      label="Project Name"
                      placeholder="Enter project name"
                      type="text"
                      required
                    />
                  </div>
                  <div class="col-md-6 mb-3">
                    <label class="form-label">Project Type</label>
                    <select v-model="formData.projectType" class="form-select" required>
                      <option value="">Select type</option>
                      <option value="web">Web Application</option>
                      <option value="mobile">Mobile App</option>
                      <option value="desktop">Desktop Software</option>
                      <option value="api">API Service</option>
                    </select>
                  </div>
                  <div class="col-md-6 mb-3">
                    <label class="form-label">Priority</label>
                    <select v-model="formData.priority" class="form-select" required>
                      <option value="">Select priority</option>
                      <option value="high">High</option>
                      <option value="medium">Medium</option>
                      <option value="low">Low</option>
                    </select>
                  </div>
                  <div class="col-12 mb-3">
                    <label class="form-label">Description</label>
                    <textarea 
                      v-model="formData.description"
                      class="form-control" 
                      rows="4"
                      placeholder="Enter project description"
                      required
                    ></textarea>
                  </div>
                </div>
              </div>

              <!-- Step 2: Team Members -->
              <div v-show="currentStep === 2" class="step-content">
                <div class="row">
                  <div class="col-12 mb-3">
                    <MDTypography variant="h6" font-weight="medium" class="mb-3">
                      Select Team Members
                    </MDTypography>
                    <div class="team-members-list">
                      <div 
                        v-for="member in availableMembers" 
                        :key="member.id"
                        class="member-item"
                        @click="toggleMember(member.id)"
                      >
                        <div class="d-flex align-items-center">
                          <div class="form-check me-3">
                            <input 
                              :checked="formData.teamMembers.includes(member.id)"
                              class="form-check-input" 
                              type="checkbox"
                              @click.stop="toggleMember(member.id)"
                            >
                          </div>
                          <MDAvatar :src="member.avatar" alt="member" size="md" class="me-3" />
                          <div class="flex-grow-1">
                            <MDTypography variant="button" font-weight="medium" class="d-block">
                              {{ member.name }}
                            </MDTypography>
                            <MDTypography variant="caption" color="text">
                              {{ member.role }}
                            </MDTypography>
                          </div>
                          <MDBadge 
                            v-if="member.available" 
                            color="success" 
                            variant="gradient"
                            size="sm"
                          >
                            Available
                          </MDBadge>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Step 3: Budget & Timeline -->
              <div v-show="currentStep === 3" class="step-content">
                <div class="row">
                  <div class="col-md-6 mb-3">
                    <MDInput
                      v-model="formData.budget"
                      label="Budget ($)"
                      placeholder="0.00"
                      type="number"
                      step="0.01"
                      required
                    />
                  </div>
                  <div class="col-md-6 mb-3">
                    <MDInput
                      v-model="formData.currency"
                      label="Currency"
                      placeholder="USD"
                      type="text"
                      required
                    />
                  </div>
                  <div class="col-md-6 mb-3">
                    <MDInput
                      v-model="formData.startDate"
                      label="Start Date"
                      type="date"
                      required
                    />
                  </div>
                  <div class="col-md-6 mb-3">
                    <MDInput
                      v-model="formData.endDate"
                      label="End Date"
                      type="date"
                      required
                    />
                  </div>
                  <div class="col-12 mb-3">
                    <label class="form-label">Milestones</label>
                    <div v-for="(milestone, index) in formData.milestones" :key="index" class="mb-2 d-flex gap-2">
                      <input 
                        v-model="milestone.title"
                        type="text" 
                        class="form-control" 
                        placeholder="Milestone title"
                      >
                      <input 
                        v-model="milestone.date"
                        type="date" 
                        class="form-control"
                      >
                      <MDButton 
                        color="error" 
                        size="sm"
                        @click="removeMilestone(index)"
                      >
                        <Icon name="mdi:delete" />
                      </MDButton>
                    </div>
                    <MDButton color="light" size="sm" @click="addMilestone">
                      <Icon name="mdi:plus" class="me-1" />
                      Add Milestone
                    </MDButton>
                  </div>
                </div>
              </div>

              <!-- Step 4: Review -->
              <div v-show="currentStep === 4" class="step-content">
                <MDTypography variant="h6" font-weight="medium" class="mb-3">
                  Review Project Details
                </MDTypography>
                
                <div class="card bg-light">
                  <div class="card-body">
                    <div class="row mb-3">
                      <div class="col-md-6">
                        <MDTypography variant="caption" color="text" class="d-block mb-1">
                          Project Name
                        </MDTypography>
                        <MDTypography variant="button" font-weight="medium">
                          {{ formData.projectName }}
                        </MDTypography>
                      </div>
                      <div class="col-md-6">
                        <MDTypography variant="caption" color="text" class="d-block mb-1">
                          Project Type
                        </MDTypography>
                        <MDTypography variant="button" font-weight="medium">
                          {{ formData.projectType }}
                        </MDTypography>
                      </div>
                    </div>
                    
                    <div class="row mb-3">
                      <div class="col-12">
                        <MDTypography variant="caption" color="text" class="d-block mb-1">
                          Description
                        </MDTypography>
                        <MDTypography variant="button" font-weight="medium">
                          {{ formData.description }}
                        </MDTypography>
                      </div>
                    </div>
                    
                    <div class="row mb-3">
                      <div class="col-12">
                        <MDTypography variant="caption" color="text" class="d-block mb-2">
                          Team Members ({{ formData.teamMembers.length }})
                        </MDTypography>
                        <div class="avatar-group">
                          <MDAvatar 
                            v-for="memberId in formData.teamMembers" 
                            :key="memberId"
                            :src="getMember(memberId).avatar"
                            :alt="getMember(memberId).name"
                            size="sm"
                            class="avatar-group-item"
                          />
                        </div>
                      </div>
                    </div>
                    
                    <div class="row mb-3">
                      <div class="col-md-6">
                        <MDTypography variant="caption" color="text" class="d-block mb-1">
                          Budget
                        </MDTypography>
                        <MDTypography variant="button" font-weight="medium">
                          ${{ formData.budget }} {{ formData.currency }}
                        </MDTypography>
                      </div>
                      <div class="col-md-6">
                        <MDTypography variant="caption" color="text" class="d-block mb-1">
                          Timeline
                        </MDTypography>
                        <MDTypography variant="button" font-weight="medium">
                          {{ formData.startDate }} to {{ formData.endDate }}
                        </MDTypography>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Navigation Buttons -->
              <div class="row mt-4">
                <div class="col-12">
                  <div class="d-flex justify-content-between">
                    <MDButton 
                      v-if="currentStep > 1"
                      color="light"
                      @click="handlePrevious"
                    >
                      <Icon name="mdi:chevron-left" class="me-1" />
                      Previous
                    </MDButton>
                    <div v-else></div>
                    
                    <MDButton 
                      v-if="currentStep < 4"
                      color="primary"
                      type="submit"
                    >
                      Next
                      <Icon name="mdi:chevron-right" class="ms-1" />
                    </MDButton>
                    <MDButton 
                      v-else
                      color="success"
                      @click="submitProject"
                    >
                      <Icon name="mdi:check" class="me-1" />
                      Create Project
                    </MDButton>
                  </div>
                </div>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'nuxt/app'

definePageMeta({
  layout: 'default'
})

const router = useRouter()
const currentStep = ref(1)

const steps = [
  { title: 'Project Info' },
  { title: 'Team' },
  { title: 'Budget & Timeline' },
  { title: 'Review' }
]

const formData = ref({
  projectName: '',
  projectType: '',
  priority: '',
  description: '',
  teamMembers: [],
  budget: '',
  currency: 'USD',
  startDate: '',
  endDate: '',
  milestones: []
})

const availableMembers = ref([
  {
    id: 1,
    name: 'John Michael',
    role: 'Full Stack Developer',
    avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-1.jpg',
    available: true
  },
  {
    id: 2,
    name: 'Alexa Smith',
    role: 'UI/UX Designer',
    avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-2.jpg',
    available: true
  },
  {
    id: 3,
    name: 'Laurent Perrier',
    role: 'Backend Developer',
    avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-3.jpg',
    available: false
  },
  {
    id: 4,
    name: 'Michael Levi',
    role: 'Project Manager',
    avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-4.jpg',
    available: true
  }
])

const toggleMember = (memberId: number) => {
  const index = formData.value.teamMembers.indexOf(memberId)
  if (index === -1) {
    formData.value.teamMembers.push(memberId)
  } else {
    formData.value.teamMembers.splice(index, 1)
  }
}

const getMember = (memberId: number) => {
  return availableMembers.value.find(m => m.id === memberId) || {}
}

const addMilestone = () => {
  formData.value.milestones.push({ title: '', date: '' })
}

const removeMilestone = (index: number) => {
  formData.value.milestones.splice(index, 1)
}

const handleNext = () => {
  if (currentStep.value < 4) {
    currentStep.value++
  }
}

const handlePrevious = () => {
  if (currentStep.value > 1) {
    currentStep.value--
  }
}

const submitProject = () => {
  alert('Project created successfully!')
  router.push('/dashboards/analytics')
}
</script>

<style scoped>
.wizard-steps {
  display: flex;
  justify-content: space-between;
  align-items: center;
  position: relative;
  padding: 0 2rem;
}

.wizard-steps::before {
  content: '';
  position: absolute;
  top: 20px;
  left: 0;
  right: 0;
  height: 2px;
  background: #e9ecef;
  z-index: 0;
}

.wizard-step {
  display: flex;
  flex-direction: column;
  align-items: center;
  position: relative;
  z-index: 1;
}

.step-number {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: #e9ecef;
  color: #8392ab;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: bold;
  margin-bottom: 0.5rem;
  transition: all 0.3s;
}

.wizard-step.active .step-number {
  background: linear-gradient(195deg, #5e72e4 0%, #825ee4 100%);
  color: white;
  box-shadow: 0 4px 20px 0 rgba(0, 0, 0, 0.14), 0 7px 10px -5px rgba(94, 114, 228, 0.4);
}

.wizard-step.completed .step-number {
  background: linear-gradient(195deg, #66BB6A 0%, #43A047 100%);
  color: white;
}

.step-content {
  min-height: 300px;
}

.team-members-list {
  max-height: 400px;
  overflow-y: auto;
}

.member-item {
  padding: 1rem;
  border: 1px solid #e9ecef;
  border-radius: 0.5rem;
  margin-bottom: 0.5rem;
  cursor: pointer;
  transition: all 0.2s;
}

.member-item:hover {
  background: #f8f9fa;
  border-color: #5e72e4;
}

.form-select,
.form-control {
  border: 1px solid #d2d6da;
  border-radius: 0.5rem;
  padding: 0.5rem 0.75rem;
  font-size: 0.875rem;
  transition: border-color 0.15s ease-in-out;
}

.form-select:focus,
.form-control:focus {
  border-color: #5e72e4;
  outline: 0;
}

.form-check-input {
  width: 1.25rem;
  height: 1.25rem;
  cursor: pointer;
}

.form-check-input:checked {
  background-color: #5e72e4;
  border-color: #5e72e4;
}

.avatar-group {
  display: flex;
  margin-left: -0.5rem;
}

.avatar-group-item {
  margin-left: -0.5rem;
  border: 2px solid white;
  position: relative;
}

.avatar-group-item:first-child {
  margin-left: 0;
}

.gap-2 {
  gap: 0.5rem;
}

.bg-light {
  background-color: #f8f9fa !important;
}

@media (max-width: 768px) {
  .wizard-steps {
    padding: 0 1rem;
  }
  
  .step-number {
    width: 30px;
    height: 30px;
    font-size: 0.8rem;
  }
}
</style>
