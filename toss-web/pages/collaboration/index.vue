<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="border-b border-gray-200 dark:border-gray-700 pb-4">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900 dark:text-white">Collaboration Hub</h1>
          <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">
            Connect, communicate, and collaborate with your team
          </p>
        </div>
        <div class="flex space-x-3">
          <button class="inline-flex items-center px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600">
            <VideoCameraIcon class="w-4 h-4 mr-2" />
            Start Meeting
          </button>
          <button class="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-lg text-white bg-blue-600 hover:bg-blue-700">
            <PlusIcon class="w-4 h-4 mr-2" />
            New Channel
          </button>
        </div>
      </div>
    </div>

    <!-- Quick Actions -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-4">
      <NuxtLink to="/collaboration/chat" class="p-4 bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700 hover:shadow-md transition-shadow">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900 rounded-lg flex items-center justify-center mr-3">
            <ChatBubbleLeftRightIcon class="w-5 h-5 text-blue-600 dark:text-blue-400" />
          </div>
          <div>
            <h3 class="font-medium text-gray-900 dark:text-white">Team Chat</h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">5 unread</p>
          </div>
        </div>
      </NuxtLink>

      <NuxtLink to="/collaboration/meetings" class="p-4 bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700 hover:shadow-md transition-shadow">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-green-100 dark:bg-green-900 rounded-lg flex items-center justify-center mr-3">
            <VideoCameraIcon class="w-5 h-5 text-green-600 dark:text-green-400" />
          </div>
          <div>
            <h3 class="font-medium text-gray-900 dark:text-white">Meetings</h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">2 upcoming</p>
          </div>
        </div>
      </NuxtLink>

      <NuxtLink to="/collaboration/documents" class="p-4 bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700 hover:shadow-md transition-shadow">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-purple-100 dark:bg-purple-900 rounded-lg flex items-center justify-center mr-3">
            <DocumentTextIcon class="w-5 h-5 text-purple-600 dark:text-purple-400" />
          </div>
          <div>
            <h3 class="font-medium text-gray-900 dark:text-white">Documents</h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">12 shared</p>
          </div>
        </div>
      </NuxtLink>

      <NuxtLink to="/collaboration/whiteboard" class="p-4 bg-white dark:bg-gray-800 rounded-lg border border-gray-200 dark:border-gray-700 hover:shadow-md transition-shadow">
        <div class="flex items-center">
          <div class="w-10 h-10 bg-orange-100 dark:bg-orange-900 rounded-lg flex items-center justify-center mr-3">
            <PaintBrushIcon class="w-5 h-5 text-orange-600 dark:text-orange-400" />
          </div>
          <div>
            <h3 class="font-medium text-gray-900 dark:text-white">Whiteboard</h3>
            <p class="text-sm text-gray-500 dark:text-gray-400">3 active</p>
          </div>
        </div>
      </NuxtLink>
    </div>

    <!-- Team Activity & Chat Channels -->
    <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
      <!-- Team Activity -->
      <div class="lg:col-span-2 bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Team Activity</h3>
          <button class="text-sm text-blue-600 dark:text-blue-400 hover:text-blue-700">
            View all
          </button>
        </div>
        <div class="space-y-4">
          <div v-for="activity in teamActivity" :key="activity.id" class="flex items-start space-x-3">
            <img :src="activity.avatar" :alt="activity.user" class="w-8 h-8 rounded-full">
            <div class="flex-1 min-w-0">
              <div class="flex items-center space-x-2">
                <p class="text-sm font-medium text-gray-900 dark:text-white">{{ activity.user }}</p>
                <span class="text-xs text-gray-500 dark:text-gray-400">{{ activity.time }}</span>
              </div>
              <p class="text-sm text-gray-600 dark:text-gray-400">{{ activity.action }}</p>
              <div v-if="activity.type === 'file'" class="mt-2 p-2 bg-gray-50 dark:bg-gray-700 rounded-lg">
                <div class="flex items-center">
                  <DocumentTextIcon class="w-4 h-4 text-gray-500 mr-2" />
                  <span class="text-sm text-gray-900 dark:text-white">{{ activity.fileName }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Chat Channels -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Channels</h3>
          <button class="text-blue-600 dark:text-blue-400 hover:text-blue-700">
            <PlusIcon class="w-4 h-4" />
          </button>
        </div>
        <div class="space-y-2">
          <div v-for="channel in channels" :key="channel.id" class="flex items-center justify-between p-2 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 cursor-pointer">
            <div class="flex items-center">
              <div class="w-2 h-2 rounded-full mr-3" :class="channel.online ? 'bg-green-500' : 'bg-gray-300'"></div>
              <div>
                <p class="text-sm font-medium text-gray-900 dark:text-white"># {{ channel.name }}</p>
                <p class="text-xs text-gray-500 dark:text-gray-400">{{ channel.members }} members</p>
              </div>
            </div>
            <div v-if="channel.unread" class="w-5 h-5 bg-blue-500 text-white rounded-full flex items-center justify-center text-xs">
              {{ channel.unread }}
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Online Team Members & Upcoming Meetings -->
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Online Team Members -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Team Members Online</h3>
          <span class="text-sm text-gray-500 dark:text-gray-400">{{ onlineMembers.length }} online</span>
        </div>
        <div class="space-y-3">
          <div v-for="member in onlineMembers" :key="member.id" class="flex items-center justify-between">
            <div class="flex items-center">
              <div class="relative">
                <img :src="member.avatar" :alt="member.name" class="w-8 h-8 rounded-full">
                <div class="absolute -bottom-1 -right-1 w-3 h-3 bg-green-500 border-2 border-white dark:border-gray-800 rounded-full"></div>
              </div>
              <div class="ml-3">
                <p class="text-sm font-medium text-gray-900 dark:text-white">{{ member.name }}</p>
                <p class="text-xs text-gray-500 dark:text-gray-400">{{ member.role }}</p>
              </div>
            </div>
            <div class="flex space-x-2">
              <button class="text-gray-400 hover:text-blue-600 dark:hover:text-blue-400">
                <ChatBubbleLeftRightIcon class="w-4 h-4" />
              </button>
              <button class="text-gray-400 hover:text-green-600 dark:hover:text-green-400">
                <VideoCameraIcon class="w-4 h-4" />
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Upcoming Meetings -->
      <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Upcoming Meetings</h3>
          <NuxtLink to="/collaboration/meetings" class="text-sm text-blue-600 dark:text-blue-400 hover:text-blue-700">
            View all
          </NuxtLink>
        </div>
        <div class="space-y-3">
          <div v-for="meeting in upcomingMeetings" :key="meeting.id" class="p-3 border border-gray-200 dark:border-gray-600 rounded-lg">
            <div class="flex items-center justify-between mb-2">
              <h4 class="font-medium text-gray-900 dark:text-white">{{ meeting.title }}</h4>
              <span class="text-xs text-gray-500 dark:text-gray-400">{{ meeting.time }}</span>
            </div>
            <div class="flex items-center space-x-2 mb-2">
              <ClockIcon class="w-4 h-4 text-gray-400" />
              <span class="text-sm text-gray-600 dark:text-gray-400">{{ meeting.duration }}</span>
            </div>
            <div class="flex items-center space-x-2">
              <div class="flex -space-x-1">
                <img v-for="attendee in meeting.attendees.slice(0, 3)" :key="attendee.id" :src="attendee.avatar" :alt="attendee.name" class="w-6 h-6 rounded-full border-2 border-white dark:border-gray-800">
                <div v-if="meeting.attendees.length > 3" class="w-6 h-6 bg-gray-200 dark:bg-gray-600 rounded-full border-2 border-white dark:border-gray-800 flex items-center justify-center text-xs text-gray-600 dark:text-gray-400">
                  +{{ meeting.attendees.length - 3 }}
                </div>
              </div>
              <button class="ml-auto text-blue-600 dark:text-blue-400 hover:text-blue-700 text-sm font-medium">
                Join
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Shared Documents -->
    <div class="bg-white dark:bg-gray-800 rounded-lg shadow-sm border border-gray-200 dark:border-gray-700 p-6">
      <div class="flex items-center justify-between mb-4">
        <h3 class="text-lg font-semibold text-gray-900 dark:text-white">Recent Shared Documents</h3>
        <NuxtLink to="/collaboration/documents" class="text-sm text-blue-600 dark:text-blue-400 hover:text-blue-700">
          View all
        </NuxtLink>
      </div>
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        <div v-for="doc in sharedDocuments" :key="doc.id" class="p-4 border border-gray-200 dark:border-gray-600 rounded-lg hover:shadow-md transition-shadow cursor-pointer">
          <div class="flex items-center mb-3">
            <div class="w-10 h-10 rounded-lg flex items-center justify-center mr-3" :class="doc.iconBg">
              <component :is="doc.icon" class="w-5 h-5" :class="doc.iconColor" />
            </div>
            <div class="flex-1 min-w-0">
              <h4 class="font-medium text-gray-900 dark:text-white truncate">{{ doc.name }}</h4>
              <p class="text-xs text-gray-500 dark:text-gray-400">{{ doc.size }}</p>
            </div>
          </div>
          <div class="flex items-center justify-between">
            <div class="flex items-center">
              <img :src="doc.sharedBy.avatar" :alt="doc.sharedBy.name" class="w-4 h-4 rounded-full mr-1">
              <span class="text-xs text-gray-500 dark:text-gray-400">{{ doc.sharedBy.name }}</span>
            </div>
            <span class="text-xs text-gray-500 dark:text-gray-400">{{ doc.updatedAt }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import {
  PlusIcon,
  ChatBubbleLeftRightIcon,
  VideoCameraIcon,
  DocumentTextIcon,
  PaintBrushIcon,
  ClockIcon
} from '@heroicons/vue/24/outline'

useHead({
  title: 'Collaboration Hub - TOSS ERP III'
})

// Sample data
const teamActivity = ref([
  {
    id: 1,
    user: 'Sarah Chen',
    avatar: 'https://images.unsplash.com/photo-1494790108755-2616b02b9e98?w=32&h=32&fit=crop&crop=face',
    action: 'shared a document in #sales-team',
    time: '2 minutes ago',
    type: 'file',
    fileName: 'Q4 Sales Report.pdf'
  },
  {
    id: 2,
    user: 'Mike Johnson',
    avatar: 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=32&h=32&fit=crop&crop=face',
    action: 'started a meeting in #product-dev',
    time: '15 minutes ago',
    type: 'meeting'
  },
  {
    id: 3,
    user: 'Lisa Wang',
    avatar: 'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=32&h=32&fit=crop&crop=face',
    action: 'commented on the project timeline',
    time: '1 hour ago',
    type: 'comment'
  },
  {
    id: 4,
    user: 'David Park',
    avatar: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=32&h=32&fit=crop&crop=face',
    action: 'updated the inventory spreadsheet',
    time: '2 hours ago',
    type: 'edit'
  }
])

const channels = ref([
  { id: 1, name: 'general', members: 24, unread: 3, online: true },
  { id: 2, name: 'sales-team', members: 8, unread: 0, online: true },
  { id: 3, name: 'product-dev', members: 12, unread: 2, online: true },
  { id: 4, name: 'support', members: 6, unread: 0, online: false },
  { id: 5, name: 'marketing', members: 9, unread: 1, online: true }
])

const onlineMembers = ref([
  {
    id: 1,
    name: 'Sarah Chen',
    role: 'Sales Manager',
    avatar: 'https://images.unsplash.com/photo-1494790108755-2616b02b9e98?w=32&h=32&fit=crop&crop=face'
  },
  {
    id: 2,
    name: 'Mike Johnson',
    role: 'Product Manager',
    avatar: 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=32&h=32&fit=crop&crop=face'
  },
  {
    id: 3,
    name: 'Lisa Wang',
    role: 'UI Designer',
    avatar: 'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=32&h=32&fit=crop&crop=face'
  },
  {
    id: 4,
    name: 'David Park',
    role: 'Developer',
    avatar: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=32&h=32&fit=crop&crop=face'
  }
])

const upcomingMeetings = ref([
  {
    id: 1,
    title: 'Weekly Standup',
    time: 'In 15 minutes',
    duration: '30 minutes',
    attendees: [
      { id: 1, name: 'Sarah', avatar: 'https://images.unsplash.com/photo-1494790108755-2616b02b9e98?w=24&h=24&fit=crop&crop=face' },
      { id: 2, name: 'Mike', avatar: 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=24&h=24&fit=crop&crop=face' },
      { id: 3, name: 'Lisa', avatar: 'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=24&h=24&fit=crop&crop=face' },
      { id: 4, name: 'David', avatar: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=24&h=24&fit=crop&crop=face' }
    ]
  },
  {
    id: 2,
    title: 'Product Review',
    time: 'Tomorrow 2:00 PM',
    duration: '1 hour',
    attendees: [
      { id: 1, name: 'Sarah', avatar: 'https://images.unsplash.com/photo-1494790108755-2616b02b9e98?w=24&h=24&fit=crop&crop=face' },
      { id: 2, name: 'Mike', avatar: 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=24&h=24&fit=crop&crop=face' }
    ]
  }
])

const sharedDocuments = ref([
  {
    id: 1,
    name: 'Q4 Budget Plan.xlsx',
    size: '2.3 MB',
    icon: DocumentTextIcon,
    iconBg: 'bg-green-100 dark:bg-green-900',
    iconColor: 'text-green-600 dark:text-green-400',
    sharedBy: {
      name: 'Sarah',
      avatar: 'https://images.unsplash.com/photo-1494790108755-2616b02b9e98?w=16&h=16&fit=crop&crop=face'
    },
    updatedAt: '2 hours ago'
  },
  {
    id: 2,
    name: 'Product Wireframes.fig',
    size: '5.1 MB',
    icon: PaintBrushIcon,
    iconBg: 'bg-purple-100 dark:bg-purple-900',
    iconColor: 'text-purple-600 dark:text-purple-400',
    sharedBy: {
      name: 'Lisa',
      avatar: 'https://images.unsplash.com/photo-1438761681033-6461ffad8d80?w=16&h=16&fit=crop&crop=face'
    },
    updatedAt: '1 day ago'
  },
  {
    id: 3,
    name: 'API Documentation.pdf',
    size: '890 KB',
    icon: DocumentTextIcon,
    iconBg: 'bg-blue-100 dark:bg-blue-900',
    iconColor: 'text-blue-600 dark:text-blue-400',
    sharedBy: {
      name: 'David',
      avatar: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=16&h=16&fit=crop&crop=face'
    },
    updatedAt: '3 days ago'
  }
])
</script>
