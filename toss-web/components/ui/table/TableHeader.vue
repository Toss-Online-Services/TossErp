<script setup lang="ts">
import type { HTMLAttributes } from "vue"
import { cn } from '~/lib/utils'

const props = defineProps</* @vue-ignore */ {
  class?: HTMLAttributes["class"]
}>()
</script>

<template>
  <thead :class="cn('[&_tr]:border-b', props.class)">
    <slot />
  </thead>
</template>
