<script setup lang="ts">
import Card from '@/components/ui/Card.vue'
import CardHeader from '@/components/ui/CardHeader.vue'
import CardContent from '@/components/ui/CardContent.vue'
import Button from '@/components/ui/Button.vue'
import { CheckSquare, Plus } from 'lucide-vue-next'
</script>

<template>
  <div class="space-y-6">
    <div class="flex items-center justify-between">
      <div>
        <h1 class="text-xl font-bold tracking-tight">Tasks</h1>
        <p class="text-muted-foreground">Task management and status tracking</p>
      </div>
      <Button size="sm">
        <Plus class="mr-2 h-4 w-4" />
        Add Task
      </Button>
    </div>
    <Card>
      <CardHeader>
        <div class="flex items-center gap-2">
          <CheckSquare class="h-4 w-4 text-muted-foreground" />
          <span class="font-semibold">My Tasks</span>
        </div>
      </CardHeader>
      <CardContent>
        <ul class="space-y-2">
          <li class="flex items-center justify-between">
            <span>Follow up with Acme Corp</span>
            <span class="text-xs bg-primary text-primary-foreground px-2 py-1 rounded">In Progress</span>
          </li>
          <li class="flex items-center justify-between">
            <span>Prepare sales report</span>
            <span class="text-xs bg-muted px-2 py-1 rounded">Pending</span>
          </li>
          <li class="flex items-center justify-between">
            <span>Update company profile</span>
            <span class="text-xs bg-success text-success-foreground px-2 py-1 rounded">Completed</span>
          </li>
        </ul>
      </CardContent>
    </Card>
  </div>
</template>
