<script setup lang="ts">
import type { DropdownMenuSubEmits, DropdownMenuSubProps } from "reka-ui"
import {
  DropdownMenuSub,

  useForwardPropsEmits,
} from "reka-ui"

const props = defineProps</* @vue-ignore */ DropdownMenuSubProps>()
const emits = defineEmits</* @vue-ignore */ DropdownMenuSubEmits>()

const forwarded = useForwardPropsEmits(props, emits)
</script>

<template>
  <DropdownMenuSub v-bind="forwarded">
    <slot />
  </DropdownMenuSub>
</template>
