<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import MaterialSymbol from '~/components/common/MaterialSymbol.vue'
import { materialDashboardRoutes, type NavCollapseItem, type NavItem } from '~/lib/navigation/materialDashboardRoutes'

interface Props {
  open?: boolean
  collapsed?: boolean
  role?: 'admin' | 'retailer' | 'supplier' | 'driver'
  userInfo?: {
    name: string
    email: string
    avatar?: string
  }
}

type NavEntry =
  | { kind: 'divider'; id: string }
  | { kind: 'title'; id: string; title: string }
  | {
      kind: 'group'
      id: string
      label: string
      icon?: string
      depth: number
      active: boolean
      expanded: boolean
    }
  | {
      kind: 'link'
      id: string
      label: string
      icon?: string
      depth: number
      active: boolean
      to?: string
      href?: string
      external: boolean
    }

const props = withDefaults(defineProps<Props>(), {
  open: true,
  role: 'retailer'
})

const emit = defineEmits<{
  'update:open': [value: boolean]
  'update:collapsed': [value: boolean]
}>()

const route = useRoute()
const isMobile = ref(false)
const collapsedLocal = ref(false)
const showProfileMenu = ref(false)
const expanded = ref<Record<string, boolean>>({})

const collapsed = computed({
  get: () => (props.collapsed ?? collapsedLocal.value),
  set: (value: boolean) => {
    collapsedLocal.value = value
    emit('update:collapsed', value)
  }
})

const sidebarOpen = computed({
  get: () => props.open,
  set: (value) => emit('update:open', value)
})

const checkMobile = () => {
  isMobile.value = window.innerWidth < 1024
  if (isMobile.value) {
    sidebarOpen.value = false
  }
}

const toggleSidebar = () => {
  if (isMobile.value) {
    sidebarOpen.value = !sidebarOpen.value
  } else {
    collapsed.value = !collapsed.value
  }
}

const normalizePath = (path: string) => {
  const withoutHash = path.split('#')[0] ?? ''
  const withoutQuery = withoutHash.split('?')[0] ?? ''
  return withoutQuery.startsWith('/') ? withoutQuery : `/${withoutQuery}`
}

const isRouteActive = (path?: string) => {
  if (!path) return false
  return normalizePath(path) === normalizePath(route.path)
}

const isNavItemActive = (item: NavCollapseItem, targetPath: string): boolean => {
  if (item.route && normalizePath(item.route) === targetPath) return true
  if (item.collapse?.length) {
    return item.collapse.some((child) => isNavItemActive(child, targetPath))
  }
  return false
}

const findIdPathByRoute = (items: NavItem[], targetPath: string, parents: string[], parentId: string): string[] | null => {
  for (const item of items) {
    if (item.type !== 'collapse') continue

    const currentId = parentId ? `${parentId}/${item.key}` : item.key
    const nextParents = [...parents, currentId]
    if (item.route && normalizePath(item.route) === targetPath) return nextParents

    if (item.collapse?.length) {
      const result = findIdPathByRoute(item.collapse, targetPath, nextParents, currentId)
      if (result) return result
    }
  }

  return null
}

const activeIdPath = computed(() => {
  return findIdPathByRoute(materialDashboardRoutes, normalizePath(route.path), [], '') ?? []
})

const isIdActive = (id: string) => activeIdPath.value.includes(id)

const toggleExpanded = (id: string) => {
  if (collapsed.value && !isMobile.value) return
  expanded.value = {
    ...expanded.value,
    [id]: !expanded.value[id]
  }
}

const ensureActiveExpanded = () => {
  const ids = activeIdPath.value
  if (!ids.length) return
  const next = { ...expanded.value }
  for (const id of ids) next[id] = true
  expanded.value = next
}

watch(
  () => route.path,
  () => ensureActiveExpanded(),
  { immediate: true }
)

const isGroupItem = (item: NavCollapseItem) => {
  return Boolean(item.collapse?.length) && !item.noCollapse && !item.route && !item.href
}

const buildEntries = (items: NavItem[], depth: number, acc: NavEntry[], parentId: string) => {
  for (const item of items) {
    if (item.type === 'divider') {
      const id = parentId ? `${parentId}/${item.key}` : item.key
      acc.push({ kind: 'divider', id })
      continue
    }

    if (item.type === 'title') {
      const id = parentId ? `${parentId}/${item.key}` : item.key
      acc.push({ kind: 'title', id, title: item.title })
      continue
    }

    const id = parentId ? `${parentId}/${item.key}` : item.key
    const target = normalizePath(route.path)

    const isGroup = isGroupItem(item)
    if (isGroup) {
      const isExpanded = Boolean(expanded.value[id])
      acc.push({
        kind: 'group',
        id,
        label: item.name,
        icon: item.icon,
        depth,
        active: isNavItemActive(item, target) || isIdActive(id),
        expanded: isExpanded
      })
      if ((!collapsed.value || isMobile.value) && isExpanded) {
        buildEntries(item.collapse ?? [], depth + 1, acc, id)
      }
      continue
    }

    const to = item.route
    const href = item.href
    acc.push({
      kind: 'link',
      id,
      label: item.name,
      icon: item.icon,
      depth,
      active: isRouteActive(to),
      to,
      href,
      external: Boolean(href) && !to
    })
  }
}

const entries = computed(() => {
  const acc: NavEntry[] = []
  buildEntries(materialDashboardRoutes, 0, acc, '')
  return acc
})

onMounted(() => {
  checkMobile()
  window.addEventListener('resize', checkMobile)
})

onUnmounted(() => {
  window.removeEventListener('resize', checkMobile)
})
</script>

<template>
  <div class="relative">
    <aside
      :class="[
        'fixed top-0 left-0 h-screen bg-card border-r border-border transition-all duration-300 z-50 flex flex-col overflow-hidden',
        collapsed && !isMobile ? 'w-16' : 'w-64',
        isMobile && !sidebarOpen ? '-translate-x-full' : 'translate-x-0',
        isMobile ? 'w-64' : 'lg:m-4 lg:h-[calc(100vh-2rem)] lg:rounded-2xl lg:border lg:shadow-xl'
      ]"
    >
    <!-- Sidebar Header -->
      <div class="flex items-center justify-between h-16 px-4 border-b border-border">
        <div class="flex items-center gap-3" :class="{ 'mx-auto': collapsed && !isMobile }">
          <div class="flex items-center justify-center w-9 h-9 rounded-lg bg-primary text-primary-foreground">
            <span class="text-sm font-bold">T</span>
          </div>
          <h3 v-if="!collapsed || isMobile" class="text-sm font-semibold text-foreground">TOSS ERP</h3>
        </div>
        <button
          @click="toggleSidebar"
          class="p-2 rounded-lg hover:bg-accent text-muted-foreground transition-colors"
          :class="{ 'mx-auto': collapsed && !isMobile }"
        >
        <MaterialSymbol v-if="!collapsed && !isMobile" name="chevron_left" :size="20" />
        <MaterialSymbol v-else-if="collapsed && !isMobile" name="chevron_right" :size="20" />
        <MaterialSymbol v-else name="menu" :size="20" />
      </button>
    </div>

    <!-- User Profile Section -->
    <div
      v-if="userInfo && (!collapsed || isMobile)"
      class="px-4 py-4 border-b border-border"
    >
      <div class="relative">
        <button
          @click="showProfileMenu = !showProfileMenu"
          class="flex items-center gap-3 w-full p-2 rounded-lg hover:bg-accent transition-colors"
        >
          <div class="w-10 h-10 rounded-full bg-primary text-primary-foreground flex items-center justify-center">
            <MaterialSymbol name="account_circle" :size="20" />
          </div>
          <div class="flex-1 text-left">
            <p class="text-sm font-medium text-foreground">{{ userInfo.name }}</p>
            <p class="text-xs text-muted-foreground truncate">{{ userInfo.email }}</p>
          </div>
        </button>
        <div
          v-if="showProfileMenu"
          class="absolute left-0 right-0 mt-2 bg-card border border-border rounded-lg shadow-lg z-50"
        >
          <NuxtLink
            to="/settings"
            class="block px-4 py-2 text-sm hover:bg-accent rounded-t-lg"
            @click="showProfileMenu = false"
          >
            Settings
          </NuxtLink>
          <button
            @click="showProfileMenu = false"
            class="w-full text-left px-4 py-2 text-sm text-destructive hover:bg-accent rounded-b-lg"
          >
            Logout
          </button>
        </div>
      </div>
    </div>

    <!-- Navigation -->
    <nav class="flex-1 overflow-y-auto px-3 py-4 space-y-1">
      <template v-for="entry in entries" :key="entry.id">
        <hr
          v-if="entry.kind === 'divider'"
          class="my-3 border-border"
        />

        <p
          v-else-if="entry.kind === 'title'"
          v-show="!collapsed || isMobile"
          class="px-3 text-[0.65rem] font-bold uppercase tracking-wider text-muted-foreground mb-2 mt-4 first:mt-0"
        >
          {{ entry.title }}
        </p>

        <button
          v-else-if="entry.kind === 'group'"
          type="button"
          :title="(collapsed && !isMobile) ? entry.label : ''"
          :class="[
            'w-full flex items-center gap-3 px-3 py-2 rounded-lg text-sm transition-all text-left',
            entry.active
              ? 'bg-background text-foreground shadow-sm'
              : 'text-muted-foreground hover:bg-accent/60 hover:text-foreground'
          ]"
          :style="(!collapsed || isMobile) ? { paddingLeft: `${12 + entry.depth * 12}px` } : undefined"
          @click="toggleExpanded(entry.id)"
        >
          <span
            :class="[
              'flex items-center justify-center w-8 h-8 rounded-lg flex-shrink-0 transition-colors',
              entry.active
                ? 'bg-primary text-primary-foreground'
                : 'bg-muted text-muted-foreground'
            ]"
          >
            <MaterialSymbol v-if="entry.icon" :name="entry.icon" :size="18" />
            <span v-else class="w-2 h-2 rounded-full bg-current opacity-70" />
          </span>
          <span v-if="!collapsed || isMobile" class="truncate flex-1">{{ entry.label }}</span>
          <MaterialSymbol
            v-if="(!collapsed || isMobile)"
            name="expand_more"
            :size="18"
            class="transition-transform"
            :class="entry.expanded ? 'rotate-180' : ''"
          />
        </button>

        <component
          v-else
          :is="entry.external ? 'a' : 'NuxtLink'"
          :href="entry.external ? entry.href : undefined"
          :to="entry.external ? undefined : entry.to"
          :target="entry.external ? '_blank' : undefined"
          :rel="entry.external ? 'noopener noreferrer' : undefined"
          :title="(collapsed && !isMobile) ? entry.label : ''"
          :class="[
            'flex items-center gap-3 px-3 py-2 rounded-lg text-sm transition-all',
            entry.active
              ? 'bg-background text-foreground shadow-sm'
              : 'text-muted-foreground hover:bg-accent/60 hover:text-foreground'
          ]"
          :style="(!collapsed || isMobile) ? { paddingLeft: `${12 + entry.depth * 12}px` } : undefined"
          @click="isMobile && (sidebarOpen = false)"
        >
          <span
            :class="[
              'flex items-center justify-center w-8 h-8 rounded-lg flex-shrink-0 transition-colors',
              entry.active
                ? 'bg-primary text-primary-foreground'
                : 'bg-muted text-muted-foreground'
            ]"
          >
            <MaterialSymbol v-if="entry.icon" :name="entry.icon" :size="18" />
            <span v-else class="w-2 h-2 rounded-full bg-current opacity-70" />
          </span>
          <span v-if="!collapsed || isMobile" class="truncate">{{ entry.label }}</span>
        </component>
      </template>
    </nav>

    <!-- Footer -->
      <div class="p-4 border-t border-border">
      <NuxtLink
        to="/settings"
        :class="[
          'flex items-center gap-2 px-3 py-2 rounded-lg text-sm transition-colors',
          route.path === '/settings'
            ? 'bg-background text-foreground shadow-sm'
            : 'text-muted-foreground hover:bg-accent/60 hover:text-foreground'
        ]"
      >
        <span
          :class="[
            'flex items-center justify-center w-8 h-8 rounded-lg transition-colors',
            route.path === '/settings'
              ? 'bg-primary text-primary-foreground'
              : 'bg-muted text-muted-foreground'
          ]"
        >
          <MaterialSymbol name="settings" :size="18" />
        </span>
        <span v-if="!collapsed || isMobile">Settings</span>
      </NuxtLink>
    </div>
    </aside>

    <!-- Mobile Overlay -->
    <Transition
      enter-active-class="transition-opacity duration-300"
      enter-from-class="opacity-0"
      enter-to-class="opacity-100"
      leave-active-class="transition-opacity duration-300"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div
        v-if="isMobile && sidebarOpen"
        @click="sidebarOpen = false"
        @touchstart="sidebarOpen = false"
        class="fixed inset-0 bg-black/50 backdrop-blur-sm z-40 lg:hidden"
      />
    </Transition>
  </div>
</template>

<style scoped>
aside {
  will-change: width, transform;
}

/* Mobile menu improvements */
@media (max-width: 1023px) {
  aside {
    box-shadow: 2px 0 8px rgba(0, 0, 0, 0.15);
  }
}

/* Scrollbar styling for mobile */
.scrollbar-thin::-webkit-scrollbar {
  width: 4px;
}

.scrollbar-thin::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 4px;
}

.scrollbar-thin::-webkit-scrollbar-thumb {
  background: #cbd5e1;
  border-radius: 4px;
}

.scrollbar-thin::-webkit-scrollbar-thumb:hover {
  background: #94a3b8;
}

/* Touch-friendly tap targets on mobile */
@media (max-width: 1023px) {
  nav a {
    min-height: 44px;
    touch-action: manipulation;
  }
}
</style>
