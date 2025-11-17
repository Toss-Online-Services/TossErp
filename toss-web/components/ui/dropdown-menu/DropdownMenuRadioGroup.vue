<script setup lang="ts">
import type { DropdownMenuRadioGroupEmits, DropdownMenuRadioGroupProps } from "reka-ui"
import {
  DropdownMenuRadioGroup,

  useForwardPropsEmits,
} from "reka-ui"

const props = defineProps</* @vue-ignore */ DropdownMenuRadioGroupProps>()
const emits = defineEmits</* @vue-ignore */ DropdownMenuRadioGroupEmits>()

const forwarded = useForwardPropsEmits(props, emits)
</script>

<template>
  <DropdownMenuRadioGroup v-bind="forwarded">
    <slot />
  </DropdownMenuRadioGroup>
</template>
