<template>
  <div class="container-fluid py-4">
    <div class="row">
      <div class="col-12">
        <div class="d-flex justify-content-between align-items-center mb-4">
          <MDTypography variant="h4" font-weight="bold">
            Project Board
          </MDTypography>
          <MDButton color="primary">
            <Icon name="mdi:plus" class="me-1" />
            Add Card
          </MDButton>
        </div>
      </div>
    </div>
    
    <div class="row">
      <div class="col-12">
        <div class="kanban-board">
          <div v-for="column in columns" :key="column.id" class="kanban-column">
            <div class="card">
              <div class="card-header pb-3">
                <div class="d-flex justify-content-between align-items-center">
                  <MDTypography variant="h6" font-weight="medium">
                    {{ column.title }}
                  </MDTypography>
                  <MDBadge :color="column.badgeColor" variant="gradient" size="sm">
                    {{ column.cards.length }}
                  </MDBadge>
                </div>
              </div>
              <div class="card-body pt-0">
                <div class="kanban-cards">
                  <div v-for="card in column.cards" :key="card.id" class="kanban-card mb-3">
                    <div class="card shadow-sm">
                      <div class="card-body p-3">
                        <div class="d-flex justify-content-between align-items-start mb-2">
                          <MDBadge 
                            :color="getPriorityColor(card.priority)" 
                            variant="gradient" 
                            size="xs"
                          >
                            {{ card.priority }}
                          </MDBadge>
                          <div class="dropdown">
                            <Icon 
                              name="mdi:dots-vertical" 
                              class="cursor-pointer"
                              size="20"
                            />
                          </div>
                        </div>
                        
                        <MDTypography variant="button" font-weight="bold" class="d-block mb-2">
                          {{ card.title }}
                        </MDTypography>
                        
                        <MDTypography variant="caption" color="text" class="d-block mb-3">
                          {{ card.description }}
                        </MDTypography>
                        
                        <div v-if="card.progress !== undefined" class="mb-3">
                          <div class="d-flex justify-content-between mb-1">
                            <MDTypography variant="caption" color="text">
                              Progress
                            </MDTypography>
                            <MDTypography variant="caption" font-weight="medium">
                              {{ card.progress }}%
                            </MDTypography>
                          </div>
                          <MDProgress 
                            :value="card.progress" 
                            :color="card.progress === 100 ? 'success' : 'info'"
                            height="0.25rem"
                          />
                        </div>
                        
                        <div class="d-flex justify-content-between align-items-center">
                          <div class="avatar-group">
                            <MDAvatar 
                              v-for="member in card.members" 
                              :key="member.id"
                              :src="member.avatar"
                              :alt="member.name"
                              size="xs"
                              class="avatar-group-item"
                            />
                          </div>
                          
                          <div class="d-flex gap-2">
                            <div v-if="card.comments" class="d-flex align-items-center">
                              <Icon name="mdi:comment-outline" size="16" class="text-secondary me-1" />
                              <MDTypography variant="caption" color="text">
                                {{ card.comments }}
                              </MDTypography>
                            </div>
                            <div v-if="card.attachments" class="d-flex align-items-center">
                              <Icon name="mdi:paperclip" size="16" class="text-secondary me-1" />
                              <MDTypography variant="caption" color="text">
                                {{ card.attachments }}
                              </MDTypography>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                  
                  <MDButton color="light" size="sm" class="w-100">
                    <Icon name="mdi:plus" class="me-1" />
                    Add Card
                  </MDButton>
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

const columns = ref([
  {
    id: 1,
    title: 'To Do',
    badgeColor: 'secondary',
    cards: [
      {
        id: 1,
        title: 'Update user authentication',
        description: 'Implement JWT token refresh mechanism',
        priority: 'high',
        progress: 0,
        members: [
          { id: 1, name: 'User 1', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-1.jpg' },
          { id: 2, name: 'User 2', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-2.jpg' }
        ],
        comments: 3,
        attachments: 2
      },
      {
        id: 2,
        title: 'Design new dashboard',
        description: 'Create wireframes and mockups for analytics dashboard',
        priority: 'medium',
        members: [
          { id: 3, name: 'User 3', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-3.jpg' }
        ],
        comments: 1,
        attachments: 5
      },
      {
        id: 3,
        title: 'Fix responsive issues',
        description: 'Address mobile layout problems on product page',
        priority: 'low',
        members: [
          { id: 4, name: 'User 4', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-4.jpg' }
        ],
        comments: 0,
        attachments: 0
      }
    ]
  },
  {
    id: 2,
    title: 'In Progress',
    badgeColor: 'info',
    cards: [
      {
        id: 4,
        title: 'API integration',
        description: 'Connect frontend with new REST API endpoints',
        priority: 'high',
        progress: 65,
        members: [
          { id: 1, name: 'User 1', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-1.jpg' },
          { id: 3, name: 'User 3', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-3.jpg' }
        ],
        comments: 8,
        attachments: 3
      },
      {
        id: 5,
        title: 'Database optimization',
        description: 'Improve query performance and add indexes',
        priority: 'medium',
        progress: 40,
        members: [
          { id: 2, name: 'User 2', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-2.jpg' }
        ],
        comments: 4,
        attachments: 1
      }
    ]
  },
  {
    id: 3,
    title: 'Review',
    badgeColor: 'warning',
    cards: [
      {
        id: 6,
        title: 'Payment gateway',
        description: 'Stripe integration for checkout process',
        priority: 'high',
        progress: 95,
        members: [
          { id: 4, name: 'User 4', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-4.jpg' }
        ],
        comments: 2,
        attachments: 1
      }
    ]
  },
  {
    id: 4,
    title: 'Done',
    badgeColor: 'success',
    cards: [
      {
        id: 7,
        title: 'Email notifications',
        description: 'Send automated emails for user actions',
        priority: 'medium',
        progress: 100,
        members: [
          { id: 1, name: 'User 1', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-1.jpg' }
        ],
        comments: 5,
        attachments: 2
      },
      {
        id: 8,
        title: 'User profile page',
        description: 'Complete user profile with edit functionality',
        priority: 'low',
        progress: 100,
        members: [
          { id: 3, name: 'User 3', avatar: 'https://demos.creative-tim.com/material-dashboard-pro/assets/img/team-3.jpg' }
        ],
        comments: 0,
        attachments: 0
      }
    ]
  }
])

const getPriorityColor = (priority: string) => {
  const colors: Record<string, string> = {
    high: 'error',
    medium: 'warning',
    low: 'secondary'
  }
  return colors[priority] || 'secondary'
}
</script>

<style scoped>
.kanban-board {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 1.5rem;
  overflow-x: auto;
}

.kanban-column {
  min-width: 300px;
}

.kanban-cards {
  min-height: 200px;
}

.kanban-card {
  cursor: move;
  transition: transform 0.2s;
}

.kanban-card:hover {
  transform: translateY(-2px);
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

.cursor-pointer {
  cursor: pointer;
}

.shadow-sm {
  box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
}

@media (max-width: 992px) {
  .kanban-board {
    grid-template-columns: 1fr;
  }
}
</style>
