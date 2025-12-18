<template>
  <div class="container-fluid py-4">
    <div class="row">
      <div class="col-lg-3 mb-4">
        <!-- Event Details Card -->
        <div class="card">
          <div class="card-header pb-0">
            <MDTypography variant="h6" font-weight="medium">
              Event Details
            </MDTypography>
          </div>
          <div class="card-body">
            <MDButton color="primary" class="w-100 mb-3">
              <Icon name="mdi:plus" class="me-1" />
              Add New Event
            </MDButton>
            
            <div v-if="selectedEvent" class="mt-3">
              <MDTypography variant="button" font-weight="bold" class="d-block mb-2">
                {{ selectedEvent.title }}
              </MDTypography>
              <MDTypography variant="caption" color="text" class="d-block mb-2">
                <Icon name="mdi:calendar" size="16" class="me-1" />
                {{ selectedEvent.start }}
              </MDTypography>
              <MDTypography variant="caption" color="text" class="d-block mb-2">
                <Icon name="mdi:clock-outline" size="16" class="me-1" />
                {{ selectedEvent.time }}
              </MDTypography>
              <MDTypography variant="caption" color="text" class="d-block mb-3">
                {{ selectedEvent.description }}
              </MDTypography>
              <MDBadge :color="selectedEvent.color" variant="gradient">
                {{ selectedEvent.category }}
              </MDBadge>
            </div>
          </div>
        </div>

        <!-- Upcoming Events -->
        <div class="card mt-4">
          <div class="card-header pb-0">
            <MDTypography variant="h6" font-weight="medium">
              Upcoming Events
            </MDTypography>
          </div>
          <div class="card-body pt-0">
            <div v-for="event in upcomingEvents" :key="event.id" class="mb-3">
              <div class="d-flex align-items-center">
                <div class="icon icon-sm icon-shape bg-gradient-{{ event.color }} shadow text-center border-radius-md me-2">
                  <Icon name="mdi:calendar" size="16" class="text-white" />
                </div>
                <div class="flex-grow-1">
                  <MDTypography variant="button" font-weight="medium" class="d-block">
                    {{ event.title }}
                  </MDTypography>
                  <MDTypography variant="caption" color="text">
                    {{ event.date }}
                  </MDTypography>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-9">
        <div class="card">
          <div class="card-header pb-0">
            <div class="d-flex justify-content-between align-items-center">
              <MDTypography variant="h5" font-weight="bold">
                {{ currentMonth }} {{ currentYear }}
              </MDTypography>
              <div class="d-flex gap-2">
                <MDButton color="light" size="sm" @click="previousMonth">
                  <Icon name="mdi:chevron-left" />
                </MDButton>
                <MDButton color="light" size="sm" @click="nextMonth">
                  <Icon name="mdi:chevron-right" />
                </MDButton>
                <MDButton color="primary" size="sm" @click="goToToday">
                  Today
                </MDButton>
              </div>
            </div>
          </div>
          <div class="card-body">
            <div class="calendar">
              <div class="calendar-header">
                <div v-for="day in weekDays" :key="day" class="calendar-day-header">
                  <MDTypography variant="caption" font-weight="bold" color="text">
                    {{ day }}
                  </MDTypography>
                </div>
              </div>
              <div class="calendar-grid">
                <div 
                  v-for="day in calendarDays" 
                  :key="day.date"
                  class="calendar-day"
                  :class="{
                    'other-month': day.isOtherMonth,
                    'today': day.isToday,
                    'has-events': day.events.length > 0
                  }"
                  @click="selectDay(day)"
                >
                  <div class="day-number">
                    {{ day.day }}
                  </div>
                  <div class="day-events">
                    <div 
                      v-for="event in day.events.slice(0, 2)" 
                      :key="event.id"
                      class="event-dot"
                      :class="`bg-gradient-${event.color}`"
                      :title="event.title"
                      @click.stop="selectEvent(event)"
                    >
                      <MDTypography variant="caption" color="white" class="event-title">
                        {{ event.title }}
                      </MDTypography>
                    </div>
                    <MDTypography 
                      v-if="day.events.length > 2" 
                      variant="caption" 
                      color="text"
                      class="more-events"
                    >
                      +{{ day.events.length - 2 }} more
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
import { ref, computed } from 'vue'

definePageMeta({
  layout: 'default'
})

const weekDays = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat']
const months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December']

const today = new Date()
const currentDate = ref(new Date())
const selectedEvent = ref(null)

const currentMonth = computed(() => months[currentDate.value.getMonth()])
const currentYear = computed(() => currentDate.value.getFullYear())

const events = ref([
  {
    id: 1,
    title: 'Team Meeting',
    date: new Date(2024, 11, 20),
    start: 'Dec 20, 2024',
    time: '10:00 AM - 11:00 AM',
    description: 'Weekly team sync meeting',
    category: 'Meeting',
    color: 'primary'
  },
  {
    id: 2,
    title: 'Project Deadline',
    date: new Date(2024, 11, 25),
    start: 'Dec 25, 2024',
    time: '5:00 PM',
    description: 'Final submission for Q4 project',
    category: 'Deadline',
    color: 'error'
  },
  {
    id: 3,
    title: 'Client Presentation',
    date: new Date(2024, 11, 22),
    start: 'Dec 22, 2024',
    time: '2:00 PM - 3:30 PM',
    description: 'Present new dashboard features',
    category: 'Presentation',
    color: 'info'
  },
  {
    id: 4,
    title: 'Code Review',
    date: new Date(2024, 11, 21),
    start: 'Dec 21, 2024',
    time: '3:00 PM - 4:00 PM',
    description: 'Review authentication module',
    category: 'Development',
    color: 'success'
  }
])

const upcomingEvents = computed(() => {
  return events.value
    .filter(e => e.date >= today)
    .sort((a, b) => a.date.getTime() - b.date.getTime())
    .slice(0, 5)
    .map(e => ({
      ...e,
      date: e.start
    }))
})

const calendarDays = computed(() => {
  const year = currentDate.value.getFullYear()
  const month = currentDate.value.getMonth()
  
  const firstDay = new Date(year, month, 1)
  const lastDay = new Date(year, month + 1, 0)
  const daysInMonth = lastDay.getDate()
  const startingDayOfWeek = firstDay.getDay()
  
  const days = []
  
  // Previous month days
  const prevMonthLastDay = new Date(year, month, 0).getDate()
  for (let i = startingDayOfWeek - 1; i >= 0; i--) {
    days.push({
      day: prevMonthLastDay - i,
      date: new Date(year, month - 1, prevMonthLastDay - i),
      isOtherMonth: true,
      isToday: false,
      events: []
    })
  }
  
  // Current month days
  for (let i = 1; i <= daysInMonth; i++) {
    const date = new Date(year, month, i)
    const dayEvents = events.value.filter(e => 
      e.date.getDate() === i && 
      e.date.getMonth() === month && 
      e.date.getFullYear() === year
    )
    
    days.push({
      day: i,
      date,
      isOtherMonth: false,
      isToday: date.toDateString() === today.toDateString(),
      events: dayEvents
    })
  }
  
  // Next month days
  const remainingDays = 42 - days.length
  for (let i = 1; i <= remainingDays; i++) {
    days.push({
      day: i,
      date: new Date(year, month + 1, i),
      isOtherMonth: true,
      isToday: false,
      events: []
    })
  }
  
  return days
})

const previousMonth = () => {
  currentDate.value = new Date(currentDate.value.getFullYear(), currentDate.value.getMonth() - 1, 1)
}

const nextMonth = () => {
  currentDate.value = new Date(currentDate.value.getFullYear(), currentDate.value.getMonth() + 1, 1)
}

const goToToday = () => {
  currentDate.value = new Date()
}

const selectDay = (day: any) => {
  if (day.events.length > 0) {
    selectedEvent.value = day.events[0]
  }
}

const selectEvent = (event: any) => {
  selectedEvent.value = event
}
</script>

<style scoped>
.calendar {
  width: 100%;
}

.calendar-header {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 0.5rem;
  margin-bottom: 1rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid #dee2e6;
}

.calendar-day-header {
  text-align: center;
  padding: 0.5rem;
}

.calendar-grid {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 0.5rem;
}

.calendar-day {
  min-height: 100px;
  padding: 0.5rem;
  border: 1px solid #e9ecef;
  border-radius: 0.5rem;
  cursor: pointer;
  transition: all 0.2s;
}

.calendar-day:hover {
  background-color: #f8f9fa;
  border-color: #5e72e4;
}

.calendar-day.today {
  background-color: #e8eaf6;
  border-color: #5e72e4;
  font-weight: bold;
}

.calendar-day.other-month {
  opacity: 0.4;
}

.day-number {
  font-weight: 600;
  margin-bottom: 0.5rem;
}

.day-events {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.event-dot {
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  font-size: 0.75rem;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.event-title {
  font-size: 0.7rem;
}

.more-events {
  font-size: 0.7rem;
  margin-top: 0.25rem;
}

.gap-2 {
  gap: 0.5rem;
}

.icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 2rem;
  height: 2rem;
  border-radius: 0.5rem;
}

.icon-sm {
  width: 2rem;
  height: 2rem;
}

.bg-gradient-primary {
  background-image: linear-gradient(195deg, #5e72e4 0%, #825ee4 100%);
}

.bg-gradient-info {
  background-image: linear-gradient(195deg, #49a3f1 0%, #1A73E8 100%);
}

.bg-gradient-success {
  background-image: linear-gradient(195deg, #66BB6A 0%, #43A047 100%);
}

.bg-gradient-error {
  background-image: linear-gradient(195deg, #EF5350 0%, #E53935 100%);
}
</style>
