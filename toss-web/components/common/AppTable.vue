<script setup lang="ts">
interface Props {
  headers: string[]
  loading?: boolean
  emptyMessage?: string
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
  emptyMessage: 'No data available'
})
</script>

<template>
  <div class="overflow-x-auto">
    <table class="w-full">
      <thead>
        <tr class="border-b border-border">
          <th
            v-for="header in headers"
            :key="header"
            class="px-4 py-3 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider"
          >
            {{ header }}
          </th>
        </tr>
      </thead>
      <tbody class="divide-y divide-border">
        <slot />
        <tr v-if="loading">
          <td :colspan="headers.length" class="px-4 py-8 text-center text-muted-foreground">
            <div class="flex items-center justify-center gap-2">
              <div class="w-4 h-4 border-2 border-primary border-t-transparent rounded-full animate-spin" />
              Loading...
            </div>
          </td>
        </tr>
        <tr v-if="!loading && !$slots.default">
          <td :colspan="headers.length" class="px-4 py-8 text-center text-muted-foreground">
            {{ emptyMessage }}
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>


