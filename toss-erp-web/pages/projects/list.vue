<script setup lang="ts">
const { data: tasks } = await useFetch('/api/projects/tasks')

const projects = computed(() => [
  { id: 'PRJ-001', name: 'Shared logistics rollout', owner: 'Team', status: 'In Progress' },
  { id: 'PRJ-002', name: 'POS refresh', owner: 'Store', status: 'Planned' }
])
</script>

<template>
  <div class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">Projects</p>
      <h1 class="text-xl font-semibold">Projects</h1>
    </div>

    <div class="card-surface p-4 space-y-4">
      <div class="grid gap-3 md:grid-cols-2">
        <div
          v-for="project in projects"
          :key="project.id"
          class="rounded-xl border border-border/70 bg-card p-3"
        >
          <p class="text-sm text-muted-foreground">{{ project.id }}</p>
          <p class="text-lg font-semibold">{{ project.name }}</p>
          <p class="text-xs text-muted-foreground">Owner: {{ project.owner }}</p>
          <p class="mt-1 inline-flex rounded-full bg-primary/10 px-2 py-1 text-xs text-primary">
            {{ project.status }}
          </p>
        </div>
      </div>

      <div>
        <h3 class="section-title mb-2">Recent tasks</h3>
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
  </div>
</template>

