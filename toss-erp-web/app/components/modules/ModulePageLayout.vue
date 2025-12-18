<script setup lang="ts">
import type { ModuleData } from '~/types/modules'

const props = defineProps<{
  slug: string
}>()

const { data, pending, error, refresh } = await useFetch<ModuleData>(`/api/modules/${props.slug}`)
</script>

<template>
  <div class="space-y-4">
    <MaterialCard v-if="pending" variant="elevated" class="p-6 flex items-center justify-between">
      <div>
        <p class="text-sm text-muted-foreground">Loading live telemetry</p>
        <p class="text-lg font-semibold mt-1">Fetching {{ props.slug }} metrics…</p>
      </div>
      <MaterialButton color="primary" variant="tonal" loading />
    </MaterialCard>

    <MaterialCard
      v-else-if="error"
      variant="elevated"
      class="p-6 border border-destructive/30 bg-destructive/5 text-destructive"
    >
      <div class="flex items-start justify-between gap-6">
        <div>
          <p class="font-semibold">We could not load {{ props.slug }} data</p>
          <p class="text-sm opacity-80 mt-1">
            {{ error?.message || 'Unknown error' }}
          </p>
        </div>
        <MaterialButton color="primary" variant="filled" @click="refresh">
          Retry
        </MaterialButton>
      </div>
    </MaterialCard>

    <ModuleOverview v-else-if="data" :module-data="data" />
  </div>
</template>





