<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from '#imports'

export interface NavLink {
  label: string
  to: string
  icon?: string
}

export interface NavSection {
  label: string
  items: NavLink[]
}

const props = defineProps<{
  sections: NavSection[]
  onNavigate?: () => void
}>()

const route = useRoute()

const isActive = (to: string) => computed(() => route.path.startsWith(to))
</script>

<template>
  <div class="h-full flex flex-col">
    <div class="px-4 py-5">
      <div class="text-xl font-semibold text-primary">TOSS ERP</div>
      <p class="text-sm text-muted-foreground">ERPNext-style modules</p>
    </div>

    <nav class="flex-1 overflow-y-auto px-2 space-y-4 pb-6">
      <div
        v-for="section in sections"
        :key="section.label"
        class="space-y-2"
      >
        <p class="px-3 text-xs font-semibold uppercase tracking-wide text-muted-foreground">
          {{ section.label }}
        </p>
        <ul class="space-y-1">
          <li v-for="item in section.items" :key="item.to">
            <NuxtLink
              :to="item.to"
              class="flex items-center gap-3 px-3 py-2 rounded-lg text-sm transition hover:bg-muted"
              :class="{
                'bg-primary/10 text-primary border border-primary/30': isActive(item.to).value
              }"
              @click="onNavigate?.()"
            >
              <span
                v-if="item.icon"
                class="material-symbols-rounded text-base"
              >
                {{ item.icon }}
              </span>
              <span>{{ item.label }}</span>
            </NuxtLink>
          </li>
        </ul>
      </div>
    </nav>
  </div>
</template>

