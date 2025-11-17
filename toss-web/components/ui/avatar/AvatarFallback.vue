<script setup lang="ts">
import type { AvatarFallbackProps } from "reka-ui"
import { AvatarFallback } from "reka-ui"

const props = defineProps</* @vue-ignore */ AvatarFallbackProps>()
</script>

<template>
  <AvatarFallback v-bind="props">
    <slot />
  </AvatarFallback>
</template>
