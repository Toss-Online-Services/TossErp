<script setup lang="ts">
import { Button } from '~/components/ui/button'
import { Archive, Trash2, Receipt, RotateCcw } from 'lucide-vue-next'

defineProps<{
  canHold: boolean
  canVoid: boolean
  canComplete: boolean
}>()

const emit = defineEmits<{
  holdSale: []
  voidSale: []
  viewHeldSales: []
  viewRecentSales: []
}>()
</script>

<template>
  <div class="flex flex-wrap gap-2">
    <Button
      variant="outline"
      :disabled="!canHold"
      @click="emit('holdSale')"
    >
      <Archive class="h-4 w-4 mr-2" />
      Hold Sale
    </Button>

    <Button
      variant="outline"
      :disabled="!canVoid"
      @click="emit('voidSale')"
    >
      <Trash2 class="h-4 w-4 mr-2" />
      Void
    </Button>

    <Button
      variant="outline"
      @click="emit('viewHeldSales')"
    >
      <RotateCcw class="h-4 w-4 mr-2" />
      Held Sales
    </Button>

    <Button
      variant="outline"
      @click="emit('viewRecentSales')"
    >
      <Receipt class="h-4 w-4 mr-2" />
      Recent Sales
    </Button>
  </div>
</template>
