<script setup lang="ts">
import { Sheet, SheetContent, SheetHeader, SheetTitle, SheetDescription } from '~/components/ui/sheet'

interface Props {
  open: boolean
  title?: string
  description?: string
  side?: 'left' | 'right' | 'top' | 'bottom'
}

const props = withDefaults(defineProps<Props>(), {
  side: 'right'
})

const emit = defineEmits<{
  'update:open': [value: boolean]
}>()

const isOpen = computed({
  get: () => props.open,
  set: (value) => emit('update:open', value)
})
</script>

<template>
  <Sheet v-model:open="isOpen">
    <SheetContent :side="side" class="w-full sm:max-w-lg">
      <SheetHeader v-if="title || description">
        <SheetTitle v-if="title">{{ title }}</SheetTitle>
        <SheetDescription v-if="description">{{ description }}</SheetDescription>
      </SheetHeader>
      <div class="mt-6">
        <slot />
      </div>
    </SheetContent>
  </Sheet>
</template>


