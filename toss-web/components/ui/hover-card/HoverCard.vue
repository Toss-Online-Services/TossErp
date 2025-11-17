<script setup lang="ts">
import type { HoverCardRootEmits, HoverCardRootProps } from "reka-ui"
import { HoverCardRoot, useForwardPropsEmits } from "reka-ui"

const props = defineProps</* @vue-ignore */ HoverCardRootProps>()
const emits = defineEmits</* @vue-ignore */ HoverCardRootEmits>()

const forwarded = useForwardPropsEmits(props, emits)
</script>

<template>
  <HoverCardRoot v-bind="forwarded">
    <slot />
  </HoverCardRoot>
</template>
