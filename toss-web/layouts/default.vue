<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import {
  findNavItemByPath,
  isPathActive,
  navigation,
  type NavNode,
  secondaryNavigation,
} from '~/lib/navigation'

const route = useRoute()
const mobileOpen = ref(false)
const expandedGroups = ref(new Set<string>())

const primarySections = navigation
const authSections = secondaryNavigation

const normalizePath = (path: string) => {
  if (!path) {
    return '/'
  }

  if (path.length > 1 && path.endsWith('/')) {
    return path.slice(0, -1)
  }

  return path
}

const activePath = computed(() => normalizePath(route.path))

const navKey = (parents: string[], label: string) => [...parents, label].join('>')

const syncExpandedGroups = () => {
  const match = findNavItemByPath(activePath.value)
  if (!match) {
    expandedGroups.value = new Set()
    return
  }

  const keys = new Set<string>()
  let lineage = [match.parents[0]]
  for (const parentLabel of match.parents.slice(1)) {
    const key = navKey(lineage, parentLabel)
    keys.add(key)
    lineage = [...lineage, parentLabel]
  }

  expandedGroups.value = keys
}

const toggleGroup = (key: string) => {
  const next = new Set(expandedGroups.value)
  if (next.has(key)) {
    next.delete(key)
  } else {
    next.add(key)
  }
  expandedGroups.value = next
}

const isGroupExpanded = (key: string) => expandedGroups.value.has(key)

const isNodeActive = (item: NavNode): boolean => {
  if (isPathActive(activePath.value, item)) {
    return true
  }

  return item.children?.some(child => isNodeActive(child)) ?? false
}

watch(
  () => route.fullPath,
  () => {
    mobileOpen.value = false
    syncExpandedGroups()
  },
)

onMounted(() => {
  syncExpandedGroups()
})

const currentLabel = computed(() => findNavItemByPath(activePath.value)?.label ?? 'Overview')
const currentYear = new Date().getFullYear()
</script>

<template>
  <div class="flex min-h-screen bg-muted/50">
    <!-- Mobile sidebar -->
    <transition name="fade">
      <div
        v-if="mobileOpen"
        class="fixed inset-0 z-40 flex md:hidden"
      >
        <div class="fixed inset-0 bg-background/70 backdrop-blur" @click="mobileOpen = false" />
        <aside class="relative z-50 flex flex-col pt-6 pb-6 shadow-lg w-80 bg-background">
          <div class="px-6">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-sm font-semibold text-primary">TOSS ERP</p>
                <p class="text-xl font-bold tracking-tight">Material Shadcn</p>
              </div>
              <Button variant="ghost" size="icon" @click="mobileOpen = false">
                <Icon name="mdi:close" class="w-5 h-5" />
              </Button>
            </div>
          </div>
          <nav class="flex-1 px-4 mt-6 overflow-y-auto">
            <div v-for="section in primarySections" :key="section.title" class="space-y-2">
              <p class="px-2 text-xs font-semibold uppercase text-muted-foreground">{{ section.title }}</p>
              <div class="space-y-1">
                <div v-for="item in section.items" :key="navKey([section.title], item.label)">
                  <div v-if="item.children?.length" class="space-y-1">
                    <button
                      type="button"
                      class="flex items-center justify-between w-full px-3 py-2 text-sm font-medium transition rounded-lg"
                      :class="isNodeActive(item)
                        ? 'bg-primary/10 text-foreground'
                        : 'text-muted-foreground hover:bg-muted hover:text-foreground'"
                      @click="toggleGroup(navKey([section.title], item.label))"
                    >
                      <div class="flex items-center gap-3">
                        <Icon v-if="item.icon" :name="item.icon" class="w-4 h-4" />
                        <span>{{ item.label }}</span>
                      </div>
                      <Icon
                        name="mdi:chevron-down"
                        class="w-4 h-4 transition-transform"
                        :class="isGroupExpanded(navKey([section.title], item.label)) ? 'rotate-180' : ''"
                      />
                    </button>
                    <transition name="collapse">
                      <div
                        v-show="isGroupExpanded(navKey([section.title], item.label))"
                        class="pl-4 space-y-1 border-l border-muted"
                      >
                        <NuxtLink
                          v-for="child in item.children"
                          :key="navKey([section.title, item.label], child.label)"
                          :to="child.to"
                          class="flex items-center gap-3 px-3 py-2 text-sm font-medium transition rounded-lg"
                          :class="isPathActive(activePath, child)
                            ? 'bg-primary text-primary-foreground shadow-sm'
                            : 'text-muted-foreground hover:bg-muted hover:text-foreground'"
                        >
                          <Icon v-if="child.icon" :name="child.icon" class="h-3.5 w-3.5" />
                          <span>{{ child.label }}</span>
                        </NuxtLink>
                      </div>
                    </transition>
                  </div>
                  <NuxtLink
                    v-else-if="item.to"
                    :to="item.to"
                    class="flex items-center gap-3 px-3 py-2 text-sm font-medium transition rounded-lg"
                    :class="isPathActive(activePath, item)
                      ? 'bg-primary text-primary-foreground shadow-sm'
                      : 'text-muted-foreground hover:bg-muted hover:text-foreground'"
                  >
                    <Icon v-if="item.icon" :name="item.icon" class="w-4 h-4" />
                    <span>{{ item.label }}</span>
                  </NuxtLink>
                </div>
              </div>
            </div>

            <div v-for="section in authSections" :key="`auth-${section.title}`" class="mt-6 space-y-2">
              <p class="px-2 text-xs font-semibold uppercase text-muted-foreground">{{ section.title }}</p>
              <div class="mt-2 space-y-1">
                <NuxtLink
                  v-for="item in section.items"
                  :key="item.to"
                  :to="item.to"
                  class="flex items-center gap-3 px-3 py-2 text-sm font-medium transition rounded-lg"
                  :class="isPathActive(activePath, item)
                    ? 'bg-primary text-primary-foreground shadow-sm'
                    : 'text-muted-foreground hover:bg-muted hover:text-foreground'"
                >
                  <Icon v-if="item.icon" :name="item.icon" class="w-4 h-4" />
                  <span>{{ item.label }}</span>
                </NuxtLink>
              </div>
            </div>
          </nav>
          <div class="px-6 mt-auto">
            <p class="text-xs text-muted-foreground">&copy; {{ currentYear }} TOSS ERP. All rights reserved.</p>
          </div>
        </aside>
      </div>
    </transition>

    <!-- Desktop sidebar -->
    <aside class="flex-col hidden pt-6 pb-6 border-r shadow-sm w-80 bg-background md:flex">
      <div class="px-6">
        <p class="text-sm font-semibold text-primary">TOSS ERP</p>
        <p class="text-xl font-bold tracking-tight">Material Shadcn</p>
      </div>
      <nav class="flex-1 px-4 mt-6 overflow-y-auto">
        <div v-for="section in primarySections" :key="section.title" class="space-y-2">
          <p class="px-2 text-xs font-semibold uppercase text-muted-foreground">{{ section.title }}</p>
          <div class="space-y-1">
            <div v-for="item in section.items" :key="navKey([section.title], item.label)">
              <div v-if="item.children?.length" class="space-y-1">
                <button
                  type="button"
                  class="flex items-center justify-between w-full px-3 py-2 text-sm font-medium transition rounded-lg"
                  :class="isNodeActive(item)
                    ? 'bg-primary/10 text-foreground'
                    : 'text-muted-foreground hover:bg-muted hover:text-foreground'"
                  @click="toggleGroup(navKey([section.title], item.label))"
                >
                  <div class="flex items-center gap-3">
                    <Icon v-if="item.icon" :name="item.icon" class="w-4 h-4" />
                    <span>{{ item.label }}</span>
                  </div>
                  <Icon
                    name="mdi:chevron-down"
                    class="w-4 h-4 transition-transform"
                    :class="isGroupExpanded(navKey([section.title], item.label)) ? 'rotate-180' : ''"
                  />
                </button>
                <transition name="collapse">
                  <div
                    v-show="isGroupExpanded(navKey([section.title], item.label))"
                    class="pl-4 space-y-1 border-l border-muted"
                  >
                    <NuxtLink
                      v-for="child in item.children"
                      :key="navKey([section.title, item.label], child.label)"
                      :to="child.to"
                      class="flex items-center gap-3 px-3 py-2 text-sm font-medium transition rounded-lg"
                      :class="isPathActive(activePath, child)
                        ? 'bg-primary text-primary-foreground shadow-sm'
                        : 'text-muted-foreground hover:bg-muted hover:text-foreground'"
                    >
                      <Icon v-if="child.icon" :name="child.icon" class="h-3.5 w-3.5" />
                      <span>{{ child.label }}</span>
                    </NuxtLink>
                  </div>
                </transition>
              </div>
              <NuxtLink
                v-else-if="item.to"
                :to="item.to"
                class="flex items-center gap-3 px-3 py-2 text-sm font-medium transition rounded-lg"
                :class="isPathActive(activePath, item)
                  ? 'bg-primary text-primary-foreground shadow-sm'
                  : 'text-muted-foreground hover:bg-muted hover:text-foreground'"
              >
                <Icon v-if="item.icon" :name="item.icon" class="w-4 h-4" />
                <span>{{ item.label }}</span>
              </NuxtLink>
            </div>
          </div>
          <div class="mt-6" />
        </div>

        <div v-for="section in authSections" :key="`auth-${section.title}`" class="space-y-2">
          <p class="px-2 text-xs font-semibold uppercase text-muted-foreground">{{ section.title }}</p>
          <div class="mt-2 space-y-1">
            <NuxtLink
              v-for="item in section.items"
              :key="item.to"
              :to="item.to"
              class="flex items-center gap-3 px-3 py-2 text-sm font-medium transition rounded-lg"
              :class="isPathActive(activePath, item)
                ? 'bg-primary text-primary-foreground shadow-sm'
                : 'text-muted-foreground hover:bg-muted hover:text-foreground'"
            >
              <Icon v-if="item.icon" :name="item.icon" class="w-4 h-4" />
              <span>{{ item.label }}</span>
            </NuxtLink>
          </div>
        </div>
      </nav>
      <div class="px-6 mt-auto">
        <p class="text-xs text-muted-foreground">&copy; {{ currentYear }} TOSS ERP. All rights reserved.</p>
      </div>
    </aside>

    <!-- Main content -->
    <div class="flex flex-col flex-1 min-h-screen">
      <header class="sticky top-0 z-30 border-b bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60">
        <div class="flex items-center justify-between h-16 px-4 md:px-8">
          <div class="flex items-center gap-3">
            <Button variant="ghost" size="icon" class="md:hidden" @click="mobileOpen = true">
              <Icon name="mdi:menu" class="w-5 h-5" />
            </Button>
            <div>
              <p class="text-xs uppercase text-muted-foreground">Current Page</p>
              <h1 class="text-lg font-semibold md:text-xl">{{ currentLabel }}</h1>
            </div>
          </div>
          <div class="flex items-center gap-3">
            <div class="hidden md:block">
              <Input class="w-72" placeholder="Search everything..." type="search" />
            </div>
            <Button variant="outline" size="icon">
              <Icon name="mdi:bell-outline" class="w-5 h-5" />
            </Button>
            <Button variant="outline" size="icon" class="hidden md:inline-flex">
              <Icon name="mdi:cog-outline" class="w-5 h-5" />
            </Button>
            <div class="flex items-center gap-2 px-2 py-1 border rounded-full bg-card">
              <Avatar class="w-8 h-8">
                <AvatarImage src="https://i.pravatar.cc/100?img=12" alt="User avatar" />
                <AvatarFallback>TN</AvatarFallback>
              </Avatar>
              <div class="hidden text-sm leading-tight text-left md:block">
                <p class="font-semibold">TOSS Admin</p>
                <p class="text-xs text-muted-foreground">admin@toss-erp.com</p>
              </div>
            </div>
          </div>
        </div>
      </header>

      <main class="flex flex-col flex-1 overflow-y-auto bg-background">
        <slot />
      </main>
    </div>
  </div>
</template>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

.collapse-enter-active,
.collapse-leave-active {
  transition: all 0.2s ease;
}

.collapse-enter-from,
.collapse-leave-to {
  opacity: 0;
  max-height: 0;
}
</style>
