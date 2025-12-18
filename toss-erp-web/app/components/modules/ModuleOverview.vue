<script setup lang="ts">
import type { ModuleData } from '~/types/modules'

const props = defineProps<{
  moduleData: ModuleData
}>()
</script>

<template>
  <div class="space-y-6">
    <section class="grid gap-4 lg:grid-cols-[2fr,1fr]">
      <MaterialCard variant="elevated" class="p-6 space-y-3">
        <div class="flex items-center justify-between gap-4">
          <div>
            <p class="text-xs uppercase tracking-widest text-muted-foreground">
              {{ moduleData.timeframe }}
            </p>
            <h2 class="text-2xl font-bold text-foreground mt-1">{{ moduleData.title }}</h2>
          </div>
          <MaterialButton color="primary" variant="tonal" @click="$router.go(0)">
            Refresh
          </MaterialButton>
        </div>
        <p class="text-muted-foreground leading-relaxed">
          {{ moduleData.summary }}
        </p>
      </MaterialCard>

      <MaterialCard variant="elevated" class="p-6 space-y-3">
        <h3 class="text-sm font-semibold text-muted-foreground">Alerts & Signals</h3>
        <div class="space-y-3">
          <div
            v-for="alert in moduleData.alerts"
            :key="alert.title"
            class="rounded-xl border border-border/60 p-3 bg-muted/30"
          >
            <p class="text-xs uppercase tracking-widest text-muted-foreground">
              {{ alert.badge || 'Update' }}
            </p>
            <p class="font-semibold text-foreground">{{ alert.title }}</p>
            <p class="text-sm text-muted-foreground mt-1">{{ alert.description }}</p>
          </div>
        </div>
      </MaterialCard>
    </section>

    <section class="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
      <MaterialStatsCard
        v-for="metric in moduleData.metrics"
        :key="metric.label"
        :title="metric.label"
        :value="metric.value"
        :change="metric.change"
        :change-type="metric.trend === 'down' || metric.change.includes('-') ? 'negative' : metric.trend === 'up' ? 'positive' : 'neutral'"
        subtitle="vs last period"
        :icon="metric.icon"
        :color="metric.color"
      />
    </section>

    <section class="grid gap-4 lg:grid-cols-[2fr,1fr]">
      <MaterialCard variant="elevated" class="p-6 space-y-4">
        <div class="flex items-center justify-between">
          <div>
            <h3 class="text-lg font-semibold text-foreground">Operational Board</h3>
            <p class="text-sm text-muted-foreground">{{ moduleData.table.title }}</p>
          </div>
          <MaterialButton color="primary" variant="text">
            View all
          </MaterialButton>
        </div>
        <div class="overflow-x-auto">
          <table class="w-full text-sm">
            <thead>
              <tr class="text-left text-muted-foreground border-b border-border">
                <th
                  v-for="column in moduleData.table.columns"
                  :key="column"
                  class="py-2 font-medium"
                >
                  {{ column }}
                </th>
              </tr>
            </thead>
            <tbody>
              <tr
                v-for="(row, index) in moduleData.table.rows"
                :key="index"
                class="border-b border-border/60 last:border-0"
              >
                <td
                  v-for="column in moduleData.table.columns"
                  :key="column"
                  class="py-2"
                >
                  {{ row[column] }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </MaterialCard>

      <MaterialCard variant="elevated" class="p-6 space-y-4">
        <div class="flex items-center justify-between">
          <div>
            <h3 class="text-lg font-semibold text-foreground">Active Initiatives</h3>
            <p class="text-sm text-muted-foreground">Watch the critical tasks closely</p>
          </div>
        </div>

        <div class="space-y-3">
          <div
            v-for="task in moduleData.tasks"
            :key="task.title"
            class="rounded-xl border border-border/60 p-3"
          >
            <div class="flex items-center justify-between">
              <p class="font-semibold text-foreground">{{ task.title }}</p>
              <span
                class="text-xs px-2 py-0.5 rounded-full"
                :class="{
                  'bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-300': task.status === 'on-track',
                  'bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-300': task.status === 'due-soon',
                  'bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-300': task.status === 'at-risk'
                }"
              >
                {{ task.status.replace('-', ' ') }}
              </span>
            </div>
            <p v-if="task.due" class="text-xs text-muted-foreground mt-1">
              Due {{ task.due }}
            </p>
          </div>
        </div>
      </MaterialCard>
    </section>

    <section class="grid gap-4 lg:grid-cols-2">
      <MaterialCard variant="elevated" class="p-6 space-y-4">
        <div class="flex items-center justify-between">
          <div>
            <h3 class="text-lg font-semibold text-foreground">Insights & Highlights</h3>
            <p class="text-sm text-muted-foreground">Auto curated from all data sources</p>
          </div>
        </div>
        <div class="space-y-3">
          <div
            v-for="insight in moduleData.insights"
            :key="insight.title"
            class="rounded-xl border border-border/50 p-4 hover:bg-muted/30 transition-colors"
          >
            <p class="text-sm font-semibold text-foreground">{{ insight.title }}</p>
            <p class="text-sm text-muted-foreground mt-1">{{ insight.description }}</p>
          </div>
        </div>
      </MaterialCard>

      <MaterialCard variant="elevated" class="p-6 space-y-4">
        <div class="flex items-center justify-between">
          <div>
            <h3 class="text-lg font-semibold text-foreground">Quick Actions</h3>
            <p class="text-sm text-muted-foreground">
              Launch workflows for this capability
            </p>
          </div>
        </div>
        <div class="grid gap-3">
          <MaterialButton
            v-for="action in moduleData.actions"
            :key="action.label"
            color="primary"
            variant="tonal"
            class="justify-between"
            :to="action.to"
          >
            {{ action.label }}
            <Icon name="lucide:arrow-up-right" class="w-4 h-4" />
          </MaterialButton>
        </div>
      </MaterialCard>
    </section>

    <section class="grid gap-4 lg:grid-cols-2">
      <MaterialCard variant="elevated" class="p-6 space-y-4">
        <div class="flex items-center justify-between">
          <div>
            <h3 class="text-lg font-semibold text-foreground">Business Model Alignment</h3>
            <p class="text-sm text-muted-foreground">Direct tie back to township needs</p>
          </div>
        </div>
        <div class="space-y-3">
          <div
            v-for="need in moduleData.businessNeeds"
            :key="need.title"
            class="rounded-xl border border-border/50 p-4 bg-muted/20"
          >
            <p class="font-semibold text-foreground">{{ need.title }}</p>
            <p class="text-sm text-muted-foreground mt-1">
              {{ need.detail }}
            </p>
          </div>
        </div>
      </MaterialCard>

      <MaterialCard variant="elevated" class="p-6 space-y-4">
        <div class="flex items-center justify-between">
          <div>
            <h3 class="text-lg font-semibold text-foreground">ERPNext Alignment</h3>
            <p class="text-sm text-muted-foreground">
              Which native modules power this slice
            </p>
          </div>
        </div>
        <div class="space-y-3">
          <div
            v-for="erp in moduleData.erpnextMapping"
            :key="erp.module"
            class="rounded-xl border border-border/60 p-3 flex items-start gap-3"
          >
            <div class="p-2 rounded-lg bg-primary/10 text-primary text-xs font-semibold uppercase tracking-widest">
              {{ erp.module }}
            </div>
            <p class="text-sm text-muted-foreground leading-relaxed">
              {{ erp.capability }}
            </p>
          </div>
        </div>
      </MaterialCard>
    </section>

    <section v-if="moduleData.aiPlaybooks?.length" class="space-y-4">
      <div class="flex items-center justify-between">
        <div>
          <h3 class="text-lg font-semibold text-foreground">AI Copilot Playbooks</h3>
          <p class="text-sm text-muted-foreground">Live automations inspired by the Service-as-Software blueprint</p>
        </div>
        <MaterialButton color="primary" variant="text">
          Manage playbooks
        </MaterialButton>
      </div>
      <div class="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
        <MaterialCard
          v-for="playbook in moduleData.aiPlaybooks"
          :key="playbook.title"
          variant="elevated"
          class="p-5 space-y-2 border border-primary/10"
          hover
        >
          <div class="flex items-center justify-between">
            <p class="font-semibold text-foreground">{{ playbook.title }}</p>
            <span
              class="text-xs px-2 py-0.5 rounded-full uppercase tracking-wider font-semibold"
              :class="playbook.status === 'live' ? 'bg-emerald-100 text-emerald-700 dark:bg-emerald-900/30 dark:text-emerald-300' : 'bg-amber-100 text-amber-700 dark:bg-amber-900/30 dark:text-amber-300'"
            >
              {{ playbook.status }}
            </span>
          </div>
          <p class="text-sm text-muted-foreground leading-relaxed">
            {{ playbook.description }}
          </p>
        </MaterialCard>
      </div>
    </section>
  </div>
</template>

