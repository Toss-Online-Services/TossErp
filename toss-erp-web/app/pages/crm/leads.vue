<script setup lang="ts">
const { data: leads } = await useFetch('/api/crm/leads')
</script>

<template>
  <div class="space-y-4">
    <div>
      <p class="text-sm text-muted-foreground">CRM</p>
      <h1 class="text-xl font-semibold">Leads</h1>
    </div>

    <div class="card-surface p-4">
      <div class="overflow-x-auto">
        <table class="min-w-full text-sm">
          <thead class="text-left text-xs uppercase text-muted-foreground">
            <tr>
              <th class="pb-2">Lead</th>
              <th class="pb-2">Stage</th>
              <th class="pb-2">Source</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="lead in leads || []"
              :key="lead.id"
              class="border-t border-border/60"
            >
              <td class="py-2 font-medium">{{ lead.name }}</td>
              <td class="py-2">{{ lead.stage }}</td>
              <td class="py-2 text-muted-foreground">{{ lead.source }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

