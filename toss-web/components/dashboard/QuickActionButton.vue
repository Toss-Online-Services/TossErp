<template>
  <Button
    variant="outline"
    class="h-20 flex flex-col items-center justify-center space-y-2 hover:bg-accent/50"
    @click="$emit('click')"
  >
    <Component :is="action.icon" class="h-5 w-5" />
    <div class="text-center">
      <p class="text-xs font-medium">{{ action.label }}</p>
      <p class="text-xs text-muted-foreground">{{ action.description }}</p>
    </div>
  </Button>
</template>

<script setup lang="ts">
import { Button } from '../ui/button'

interface QuickAction {
  id: string
  label: string
  description: string
  icon: any
  handler: () => void
}

interface Props {
  action: QuickAction
}

defineProps<Props>()
defineEmits<{
  click: []
}>()
</script>