<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import { ChevronRight, Home } from 'lucide-vue-next'
import { cn } from '@/lib/utils'

export interface BreadcrumbItem {
  label: string
  href?: string
}

const props = defineProps<{
  items?: BreadcrumbItem[]
  class?: string
}>()

const route = useRoute()

const breadcrumbs = computed(() => {
  if (props.items && props.items.length > 0) {
    return props.items
  }

  // Auto-generate from route
  const pathSegments = route.path.split('/').filter(Boolean)
  const crumbs: BreadcrumbItem[] = [
    { label: 'Home', href: '/dashboard' }
  ]

  let currentPath = ''
  pathSegments.forEach((segment, index) => {
    currentPath += `/${segment}`
    const label = segment
      .split('-')
      .map(word => word.charAt(0).toUpperCase() + word.slice(1))
      .join(' ')

    // Don't add current page as clickable breadcrumb
    if (index < pathSegments.length - 1) {
      crumbs.push({ label, href: currentPath })
    } else {
      crumbs.push({ label })
    }
  })

  return crumbs
})
</script>

<template>
  <nav :class="cn('flex items-center gap-2 text-sm text-muted-foreground', $props.class)" aria-label="Breadcrumb">
    <ol class="flex items-center gap-2">
      <li v-for="(item, index) in breadcrumbs" :key="index" class="flex items-center gap-2">
        <NuxtLink
          v-if="item.href && index < breadcrumbs.length - 1"
          :to="item.href"
          class="hover:text-foreground transition-colors flex items-center gap-1"
        >
          <Home v-if="index === 0" :size="14" class="text-muted-foreground" />
          <span v-else>{{ item.label }}</span>
        </NuxtLink>
        <span v-else class="text-foreground font-medium">{{ item.label }}</span>
        <ChevronRight
          v-if="index < breadcrumbs.length - 1"
          :size="14"
          class="text-muted-foreground"
        />
      </li>
    </ol>
  </nav>
</template>

