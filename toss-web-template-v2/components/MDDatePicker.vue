<template>
  <div class="md-datepicker">
    <div class="md-datepicker-input" @click="toggleCalendar">
      <input
        :value="formattedDate"
        type="text"
        readonly
        :placeholder="placeholder"
        class="form-control"
        :class="{ 'is-invalid': error }"
      />
      <Icon name="mdi:calendar" size="20" class="md-datepicker-icon" />
    </div>
    
    <div v-if="showCalendar" class="md-datepicker-dropdown" ref="dropdown">
      <div class="md-datepicker-header">
        <button type="button" class="md-datepicker-nav" @click="previousMonth">
          <Icon name="mdi:chevron-left" size="20" />
        </button>
        <div class="md-datepicker-title">
          <select v-model="currentMonth" class="md-datepicker-select">
            <option v-for="(month, index) in months" :key="index" :value="index">
              {{ month }}
            </option>
          </select>
          <select v-model="currentYear" class="md-datepicker-select">
            <option v-for="year in yearRange" :key="year" :value="year">
              {{ year }}
            </option>
          </select>
        </div>
        <button type="button" class="md-datepicker-nav" @click="nextMonth">
          <Icon name="mdi:chevron-right" size="20" />
        </button>
      </div>
      
      <div class="md-datepicker-weekdays">
        <div v-for="day in weekDays" :key="day" class="md-datepicker-weekday">
          {{ day }}
        </div>
      </div>
      
      <div class="md-datepicker-days">
        <button
          v-for="day in calendarDays"
          :key="`${day.date.getTime()}`"
          type="button"
          class="md-datepicker-day"
          :class="{
            'md-datepicker-day-other-month': !day.isCurrentMonth,
            'md-datepicker-day-today': day.isToday,
            'md-datepicker-day-selected': day.isSelected,
            'md-datepicker-day-disabled': day.isDisabled
          }"
          :disabled="day.isDisabled"
          @click="selectDate(day.date)"
        >
          {{ day.date.getDate() }}
        </button>
      </div>
      
      <div class="md-datepicker-footer">
        <MDButton color="secondary" size="sm" @click="clearDate">
          Clear
        </MDButton>
        <MDButton color="primary" size="sm" @click="selectToday">
          Today
        </MDButton>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'

interface Props {
  modelValue?: Date | null
  placeholder?: string
  minDate?: Date
  maxDate?: Date
  error?: boolean
  format?: string
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: null,
  placeholder: 'Select date',
  format: 'MM/DD/YYYY'
})

const emit = defineEmits<{
  'update:modelValue': [value: Date | null]
}>()

const showCalendar = ref(false)
const dropdown = ref<HTMLElement | null>(null)
const selectedDate = ref<Date | null>(props.modelValue)
const currentMonth = ref(new Date().getMonth())
const currentYear = ref(new Date().getFullYear())

const months = [
  'January', 'February', 'March', 'April', 'May', 'June',
  'July', 'August', 'September', 'October', 'November', 'December'
]

const weekDays = ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa']

const yearRange = computed(() => {
  const start = currentYear.value - 10
  const end = currentYear.value + 10
  return Array.from({ length: end - start + 1 }, (_, i) => start + i)
})

const calendarDays = computed(() => {
  const days: Array<{
    date: Date
    isCurrentMonth: boolean
    isToday: boolean
    isSelected: boolean
    isDisabled: boolean
  }> = []
  
  const firstDay = new Date(currentYear.value, currentMonth.value, 1)
  const lastDay = new Date(currentYear.value, currentMonth.value + 1, 0)
  const today = new Date()
  today.setHours(0, 0, 0, 0)
  
  // Previous month days
  const firstDayOfWeek = firstDay.getDay()
  for (let i = firstDayOfWeek - 1; i >= 0; i--) {
    const date = new Date(firstDay)
    date.setDate(date.getDate() - (i + 1))
    days.push({
      date,
      isCurrentMonth: false,
      isToday: date.getTime() === today.getTime(),
      isSelected: selectedDate.value ? date.getTime() === selectedDate.value.getTime() : false,
      isDisabled: isDateDisabled(date)
    })
  }
  
  // Current month days
  for (let i = 1; i <= lastDay.getDate(); i++) {
    const date = new Date(currentYear.value, currentMonth.value, i)
    days.push({
      date,
      isCurrentMonth: true,
      isToday: date.getTime() === today.getTime(),
      isSelected: selectedDate.value ? date.getTime() === selectedDate.value.getTime() : false,
      isDisabled: isDateDisabled(date)
    })
  }
  
  // Next month days
  const remainingDays = 42 - days.length // 6 weeks * 7 days
  for (let i = 1; i <= remainingDays; i++) {
    const date = new Date(currentYear.value, currentMonth.value + 1, i)
    days.push({
      date,
      isCurrentMonth: false,
      isToday: date.getTime() === today.getTime(),
      isSelected: selectedDate.value ? date.getTime() === selectedDate.value.getTime() : false,
      isDisabled: isDateDisabled(date)
    })
  }
  
  return days
})

const formattedDate = computed(() => {
  if (!selectedDate.value) return ''
  
  const date = selectedDate.value
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  const year = date.getFullYear()
  
  return props.format
    .replace('MM', month)
    .replace('DD', day)
    .replace('YYYY', String(year))
})

watch(() => props.modelValue, (newValue) => {
  selectedDate.value = newValue
  if (newValue) {
    currentMonth.value = newValue.getMonth()
    currentYear.value = newValue.getFullYear()
  }
})

const isDateDisabled = (date: Date): boolean => {
  if (props.minDate && date < props.minDate) return true
  if (props.maxDate && date > props.maxDate) return true
  return false
}

const toggleCalendar = () => {
  showCalendar.value = !showCalendar.value
}

const previousMonth = () => {
  if (currentMonth.value === 0) {
    currentMonth.value = 11
    currentYear.value--
  } else {
    currentMonth.value--
  }
}

const nextMonth = () => {
  if (currentMonth.value === 11) {
    currentMonth.value = 0
    currentYear.value++
  } else {
    currentMonth.value++
  }
}

const selectDate = (date: Date) => {
  if (isDateDisabled(date)) return
  selectedDate.value = date
  emit('update:modelValue', date)
  showCalendar.value = false
}

const selectToday = () => {
  const today = new Date()
  today.setHours(0, 0, 0, 0)
  selectDate(today)
}

const clearDate = () => {
  selectedDate.value = null
  emit('update:modelValue', null)
  showCalendar.value = false
}

const handleClickOutside = (event: MouseEvent) => {
  if (dropdown.value && !dropdown.value.contains(event.target as Node)) {
    const input = (event.target as HTMLElement).closest('.md-datepicker-input')
    if (!input) {
      showCalendar.value = false
    }
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>

<style scoped>
.md-datepicker {
  position: relative;
  width: 100%;
}

.md-datepicker-input {
  position: relative;
  cursor: pointer;
}

.md-datepicker-input input {
  cursor: pointer;
  padding-right: 2.5rem;
}

.md-datepicker-icon {
  position: absolute;
  right: 0.75rem;
  top: 50%;
  transform: translateY(-50%);
  color: #67748e;
  pointer-events: none;
}

.md-datepicker-dropdown {
  position: absolute;
  top: calc(100% + 0.5rem);
  left: 0;
  background: white;
  border-radius: 0.75rem;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
  padding: 1rem;
  z-index: 1000;
  min-width: 320px;
}

.md-datepicker-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
  gap: 0.5rem;
}

.md-datepicker-nav {
  background: transparent;
  border: none;
  color: #67748e;
  cursor: pointer;
  padding: 0.25rem;
  border-radius: 0.375rem;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: background-color 0.2s;
}

.md-datepicker-nav:hover {
  background-color: #f0f2f5;
}

.md-datepicker-title {
  display: flex;
  gap: 0.5rem;
  flex: 1;
  justify-content: center;
}

.md-datepicker-select {
  border: 1px solid #d2d6da;
  border-radius: 0.375rem;
  padding: 0.25rem 0.5rem;
  font-size: 0.875rem;
  color: #344767;
  background: white;
  cursor: pointer;
}

.md-datepicker-select:focus {
  outline: none;
  border-color: #5e72e4;
}

.md-datepicker-weekdays {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 0.25rem;
  margin-bottom: 0.5rem;
}

.md-datepicker-weekday {
  text-align: center;
  font-size: 0.75rem;
  font-weight: 600;
  color: #67748e;
  padding: 0.5rem 0;
}

.md-datepicker-days {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 0.25rem;
}

.md-datepicker-day {
  aspect-ratio: 1;
  border: none;
  background: transparent;
  border-radius: 0.375rem;
  font-size: 0.875rem;
  color: #344767;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
}

.md-datepicker-day:hover:not(.md-datepicker-day-disabled) {
  background-color: #f0f2f5;
}

.md-datepicker-day-other-month {
  color: #a8b8d8;
}

.md-datepicker-day-today {
  font-weight: 700;
  color: #5e72e4;
}

.md-datepicker-day-selected {
  background: linear-gradient(195deg, #5e72e4 0%, #825ee4 100%);
  color: white !important;
}

.md-datepicker-day-selected:hover {
  background: linear-gradient(195deg, #5e72e4 0%, #825ee4 100%);
}

.md-datepicker-day-disabled {
  color: #d2d6da;
  cursor: not-allowed;
  opacity: 0.5;
}

.md-datepicker-footer {
  display: flex;
  justify-content: space-between;
  margin-top: 1rem;
  padding-top: 1rem;
  border-top: 1px solid #e9ecef;
}
</style>
