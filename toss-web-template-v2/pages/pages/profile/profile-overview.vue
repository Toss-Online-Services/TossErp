<template>
  <div class="container-fluid py-4">
    <!-- Profile Header -->
    <div class="row">
      <div class="col-12">
        <div class="card card-profile-header" style="background-image: url('https://images.unsplash.com/photo-1531512073830-ba890ca4eba2?auto=format&fit=crop&w=1920'); background-size: cover; height: 300px;">
          <div class="position-absolute w-100 h-100 bg-gradient-dark opacity-6"></div>
        </div>
      </div>
    </div>
    
    <!-- Profile Info -->
    <div class="row mt-n6">
      <div class="col-12">
        <div class="card">
          <div class="card-body p-3">
            <div class="row gx-4">
              <div class="col-auto">
                <div class="avatar avatar-xl position-relative">
                  <MDAvatar 
                    src="https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-1.jpg"
                    alt="profile"
                    size="xxl"
                    shadow="sm"
                  />
                </div>
              </div>
              <div class="col-auto my-auto">
                <div class="h-100">
                  <MDTypography variant="h5" font-weight="bold" class="mb-1">
                    {{ profile.name }}
                  </MDTypography>
                  <MDTypography variant="button" color="text">
                    {{ profile.role }} / {{ profile.organization }}
                  </MDTypography>
                </div>
              </div>
              <div class="col-lg-4 col-md-6 my-sm-auto ms-sm-auto me-sm-0 mx-auto mt-3">
                <div class="nav-wrapper position-relative end-0">
                  <ul class="nav nav-pills nav-fill p-1" role="tablist">
                    <li class="nav-item">
                      <a class="nav-link mb-0 px-0 py-1 active">
                        <Icon name="mdi:home" size="16" class="me-1" />
                        Overview
                      </a>
                    </li>
                    <li class="nav-item">
                      <a class="nav-link mb-0 px-0 py-1">
                        <Icon name="mdi:account-group" size="16" class="me-1" />
                        Teams
                      </a>
                    </li>
                    <li class="nav-item">
                      <a class="nav-link mb-0 px-0 py-1">
                        <Icon name="mdi:folder" size="16" class="me-1" />
                        Projects
                      </a>
                    </li>
                  </ul>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <div class="row mt-4">
      <!-- Platform Settings -->
      <div class="col-12 col-xl-4">
        <div class="card h-100">
          <div class="card-header pb-0 p-3">
            <MDTypography variant="h6" font-weight="medium">
              Platform Settings
            </MDTypography>
          </div>
          <div class="card-body p-3">
            <MDTypography variant="button" font-weight="bold" class="text-uppercase text-xs">
              Account
            </MDTypography>
            <ul class="list-group">
              <li class="list-group-item border-0 px-0">
                <div class="form-check form-switch ps-0">
                  <input v-model="settings.emailNotifications" class="form-check-input ms-auto" type="checkbox" id="emailNotif">
                  <label class="form-check-label text-body ms-3 text-truncate w-80 mb-0" for="emailNotif">
                    Email me when someone follows me
                  </label>
                </div>
              </li>
              <li class="list-group-item border-0 px-0">
                <div class="form-check form-switch ps-0">
                  <input v-model="settings.emailAnswers" class="form-check-input ms-auto" type="checkbox" id="emailAnswers">
                  <label class="form-check-label text-body ms-3 text-truncate w-80 mb-0" for="emailAnswers">
                    Email me when someone answers on my post
                  </label>
                </div>
              </li>
              <li class="list-group-item border-0 px-0">
                <div class="form-check form-switch ps-0">
                  <input v-model="settings.emailMentions" class="form-check-input ms-auto" type="checkbox" id="emailMentions">
                  <label class="form-check-label text-body ms-3 text-truncate w-80 mb-0" for="emailMentions">
                    Email me when someone mentions me
                  </label>
                </div>
              </li>
            </ul>
            <MDTypography variant="button" font-weight="bold" class="text-uppercase text-xs mt-4">
              Application
            </MDTypography>
            <ul class="list-group">
              <li class="list-group-item border-0 px-0">
                <div class="form-check form-switch ps-0">
                  <input v-model="settings.newLaunches" class="form-check-input ms-auto" type="checkbox" id="newLaunches">
                  <label class="form-check-label text-body ms-3 text-truncate w-80 mb-0" for="newLaunches">
                    New launches and projects
                  </label>
                </div>
              </li>
              <li class="list-group-item border-0 px-0 pb-0">
                <div class="form-check form-switch ps-0">
                  <input v-model="settings.productUpdates" class="form-check-input ms-auto" type="checkbox" id="productUpdates">
                  <label class="form-check-label text-body ms-3 text-truncate w-80 mb-0" for="productUpdates">
                    Monthly product updates
                  </label>
                </div>
              </li>
            </ul>
          </div>
        </div>
      </div>
      
      <!-- Profile Information -->
      <div class="col-12 col-xl-4">
        <div class="card h-100">
          <div class="card-header pb-0 p-3">
            <MDTypography variant="h6" font-weight="medium">
              Profile Information
            </MDTypography>
          </div>
          <div class="card-body p-3">
            <MDTypography variant="body2" color="text" class="mb-4">
              {{ profile.bio }}
            </MDTypography>
            <hr class="horizontal gray-light my-3">
            <ul class="list-group">
              <li class="list-group-item border-0 ps-0 pt-0 text-sm">
                <strong class="text-dark">Full Name:</strong> &nbsp; {{ profile.name }}
              </li>
              <li class="list-group-item border-0 ps-0 text-sm">
                <strong class="text-dark">Mobile:</strong> &nbsp; {{ profile.mobile }}
              </li>
              <li class="list-group-item border-0 ps-0 text-sm">
                <strong class="text-dark">Email:</strong> &nbsp; {{ profile.email }}
              </li>
              <li class="list-group-item border-0 ps-0 text-sm">
                <strong class="text-dark">Location:</strong> &nbsp; {{ profile.location }}
              </li>
              <li class="list-group-item border-0 ps-0 pb-0">
                <strong class="text-dark">Social:</strong> &nbsp;
                <a href="#" class="btn btn-icon-only btn-rounded btn-outline-info btn-sm me-2">
                  <Icon name="mdi:facebook" size="16" />
                </a>
                <a href="#" class="btn btn-icon-only btn-rounded btn-outline-info btn-sm me-2">
                  <Icon name="mdi:twitter" size="16" />
                </a>
                <a href="#" class="btn btn-icon-only btn-rounded btn-outline-info btn-sm">
                  <Icon name="mdi:instagram" size="16" />
                </a>
              </li>
            </ul>
          </div>
        </div>
      </div>
      
      <!-- Conversations -->
      <div class="col-12 col-xl-4">
        <div class="card h-100">
          <div class="card-header pb-0 p-3">
            <MDTypography variant="h6" font-weight="medium">
              Conversations
            </MDTypography>
          </div>
          <div class="card-body p-3">
            <ul class="list-group">
              <li v-for="conversation in conversations" :key="conversation.id" class="list-group-item border-0 d-flex align-items-center px-0 mb-2">
                <MDAvatar 
                  :src="conversation.avatar"
                  alt="user"
                  size="md"
                  class="me-3"
                />
                <div class="d-flex align-items-start flex-column justify-content-center">
                  <MDTypography variant="button" font-weight="medium">
                    {{ conversation.name }}
                  </MDTypography>
                  <MDTypography variant="caption" color="text">
                    {{ conversation.message }}
                  </MDTypography>
                </div>
                <MDButton 
                  color="info"
                  size="small"
                  class="ms-auto"
                >
                  Reply
                </MDButton>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Projects -->
    <div class="row mt-4">
      <div class="col-12">
        <div class="card">
          <div class="card-header pb-0">
            <MDTypography variant="h6" font-weight="medium">
              Projects
            </MDTypography>
          </div>
          <div class="card-body p-3">
            <div class="row">
              <div v-for="project in projects" :key="project.id" class="col-xl-3 col-md-6 mb-xl-0 mb-4">
                <div class="card card-blog card-plain">
                  <div class="position-relative">
                    <div class="d-block shadow-xl border-radius-xl">
                      <img :src="project.image" alt="img-blur" class="img-fluid shadow border-radius-xl">
                    </div>
                  </div>
                  <div class="card-body px-1 pb-0">
                    <MDTypography variant="caption" color="text" class="text-uppercase font-weight-bold">
                      {{ project.category }}
                    </MDTypography>
                    <MDTypography variant="h5" class="mt-3 mb-2">
                      <a href="javascript:;">{{ project.title }}</a>
                    </MDTypography>
                    <MDTypography variant="body2" color="text">
                      {{ project.description }}
                    </MDTypography>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

definePageMeta({
  layout: 'default'
})

const profile = ref({
  name: 'Alec Thompson',
  role: 'CEO',
  organization: 'Creative Tim',
  bio: 'Hi, I\'m Alec Thompson, Decisions: If you can\'t decide, the answer is no. If two equally difficult paths, choose the one more painful in the short term (pain avoidance is creating an illusion of equality).',
  mobile: '+1 (239) 816-9029',
  email: 'alecthompson@mail.com',
  location: 'USA'
})

const settings = ref({
  emailNotifications: true,
  emailAnswers: false,
  emailMentions: true,
  newLaunches: false,
  productUpdates: true
})

const conversations = ref([
  {
    id: 1,
    name: 'Sophie B.',
    avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-2.jpg',
    message: 'Hi! I need more information..'
  },
  {
    id: 2,
    name: 'Alexander',
    avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-3.jpg',
    message: 'Awesome work, can you..'
  },
  {
    id: 3,
    name: 'Ivanna',
    avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-4.jpg',
    message: 'About files I can..'
  }
])

const projects = ref([
  {
    id: 1,
    image: 'https://images.unsplash.com/photo-1497215728101-856f4ea42174?auto=format&fit=crop&w=600',
    category: 'Project #1',
    title: 'Modern',
    description: 'As Uber works through a huge amount of internal management turmoil.'
  },
  {
    id: 2,
    image: 'https://images.unsplash.com/photo-1493723843671-1d655e66ac1c?auto=format&fit=crop&w=600',
    category: 'Project #2',
    title: 'Scandinavian',
    description: 'Music is something that everyone has their own specific opinion about.'
  },
  {
    id: 3,
    image: 'https://images.unsplash.com/photo-1518732714860-b62714ce0c59?auto=format&fit=crop&w=600',
    category: 'Project #3',
    title: 'Minimalist',
    description: 'Different people have different taste, and various types of music.'
  },
  {
    id: 4,
    image: 'https://images.unsplash.com/photo-1470434767159-ac7bf1b43351?auto=format&fit=crop&w=600',
    category: 'Project #4',
    title: 'Gothic',
    description: 'Why would anyone pick blue over pink? Pink is obviously a better color.'
  }
])
</script>

<style scoped>
.avatar-xl {
  width: 74px;
  height: 74px;
}

.position-relative {
  position: relative;
}

.position-absolute {
  position: absolute;
}

.nav-pills .nav-link {
  background: transparent;
  border-radius: 0.5rem;
  color: #7b809a;
  cursor: pointer;
  font-size: 0.875rem;
  font-weight: 600;
  padding: 0.5rem 1rem;
}

.nav-pills .nav-link.active {
  background-color: #fff;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
  color: #344767;
}

.list-group-item {
  border: 0;
  padding: 0.5rem 0;
}

.form-check-input {
  width: 2.5rem;
  height: 1.25rem;
}

.btn-icon-only {
  width: 2rem;
  height: 2rem;
  padding: 0;
}

.btn-rounded {
  border-radius: 50%;
}

.btn-outline-info {
  border: 1px solid var(--md-info);
  color: var(--md-info);
}

.btn-outline-info:hover {
  background-color: var(--md-info);
  color: white;
}

.img-fluid {
  max-width: 100%;
  height: auto;
}

.shadow-xl {
  box-shadow: 0 20px 27px 0 rgba(0, 0, 0, 0.05);
}

.border-radius-xl {
  border-radius: 0.75rem;
}

.card-blog .card-body {
  padding: 1rem 0;
}

.w-80 {
  width: 80%;
}

.text-truncate {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.horizontal.gray-light {
  background-color: rgba(0, 0, 0, 0.05);
}
</style>
