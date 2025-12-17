<script setup lang="ts">
const { data: tasks } = await useFetch('/api/projects/tasks')
</script>

<template>
  <div class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">Projects</p>
      <h1 class="text-xl font-semibold">Tasks</h1>
    </div>

    <div class="card-surface p-4">
      <div class="overflow-x-auto">
        <table class="min-w-full text-sm">
          <thead class="text-left text-xs uppercase text-muted-foreground">
            <tr>
              <th class="pb-2">Task</th>
              <th class="pb-2">Assignee</th>
              <th class="pb-2">Status</th>
              <th class="pb-2">Due</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="task in tasks || []"
              :key="task.id"
              class="border-t border-border/60"
            >
              <td class="py-2 font-medium">{{ task.title }}</td>
              <td class="py-2">{{ task.assignee }}</td>
              <td class="py-2">{{ task.status }}</td>
              <td class="py-2 text-muted-foreground">{{ task.due.split('T')[0] }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

