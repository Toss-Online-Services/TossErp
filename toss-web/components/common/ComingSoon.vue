<script setup lang="ts">
import MaterialSymbol from '~/components/common/MaterialSymbol.vue'

interface Props {
  title: string
  description?: string
}

withDefaults(defineProps<Props>(), {
  description: 'This page is being built to match the Material Dashboard Pro experience.'
})
</script>

<template>
  <div class="space-y-6">
    <div class="rounded-xl border border-border bg-card p-6">
      <div class="flex items-start gap-4">
        <div class="flex h-12 w-12 items-center justify-center rounded-xl bg-primary text-primary-foreground">
          <MaterialSymbol name="upcoming" :size="24" />
        </div>
        <div class="min-w-0">
          <h2 class="text-lg font-semibold text-foreground truncate">{{ title }}</h2>
          <p class="mt-1 text-sm text-muted-foreground">{{ description }}</p>
        </div>
      </div>
    </div>

    <div class="rounded-xl border border-border bg-card p-6">
      <p class="text-sm text-muted-foreground">
        Coming soon.
      </p>
    </div>
  </div>
</template>
