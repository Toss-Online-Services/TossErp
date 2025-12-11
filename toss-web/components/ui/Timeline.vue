<template>
  <div class="relative mx-auto max-w-4xl" :class="containerClass">
    <!-- Vertical Separator Line -->
    <div
      class="absolute left-2 top-4 h-full w-[1px] shrink-0 bg-gray-200"
      role="separator"
      aria-orientation="vertical"
    />

    <!-- Timeline Items -->
    <div
      v-for="(item, index) in items"
      :key="item.id || index"
      class="relative pl-8 mb-10 last:mb-0"
    >
      <!-- Timeline Dot -->
      <div
        v-if="item.icon || $slots.icon"
        class="flex absolute left-0 top-3.5 justify-center items-center bg-gray-900 rounded-full size-4"
        :class="[item.iconClass || iconClass]"
      >
        <i v-if="item.icon" class="text-xs text-white material-symbols-rounded">
          {{ item.icon }}
        </i>
        <slot v-else name="icon" :item="item" :index="index" />
      </div>

      <!-- Timeline Content -->
      <div>
        <!-- Title -->
        <h4 v-if="item.title" class="py-2 text-xl font-bold tracking-tight text-gray-900 rounded-xl xl:mb-4 xl:px-3">
          {{ item.title }}
        </h4>

        <!-- Date -->
        <h5 v-if="item.date" class="top-3 tracking-tight text-gray-500 rounded-xl text-md xl:absolute">
          {{ item.date }}
        </h5>

        <!-- Content Slot with Card Support -->
        <slot name="item" :item="item" :index="index">
          <div v-if="item.description || item.content" class="my-5 text-sm text-gray-600">
            <div v-if="item.description">
              {{ item.description }}
            </div>
            <div
              v-else-if="item.content"
              class="prose dark:prose-invert"
              v-html="item.content"
            />
          </div>
        </slot>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface TimelineItem {
  id?: string | number
  icon?: string
  iconClass?: string
  title?: string
  date?: string
  description?: string
  content?: string
  [key: string]: any
}

interface Props {
  items: TimelineItem[]
  containerClass?: string
  iconClass?: string
}

withDefaults(defineProps<Props>(), {
  containerClass: '',
  iconClass: ''
})
</script>
