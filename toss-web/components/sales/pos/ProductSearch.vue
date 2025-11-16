<script setup lang="ts">
import { Search } from 'lucide-vue-next'
import { Input } from '~/components/ui/input'
import { Button } from '~/components/ui/button'

const emit = defineEmits<{
  search: [term: string]
  scanBarcode: []
}>()

const searchTerm = ref('')

const handleSearch = () => {
  emit('search', searchTerm.value)
}

const handleScan = () => {
  emit('scanBarcode')
}

watch(searchTerm, (newValue) => {
  if (newValue.length >= 2) {
    emit('search', newValue)
  } else if (newValue.length === 0) {
    emit('search', '')
  }
})
</script>

<template>
  <div class="flex gap-2">
    <div class="relative flex-1">
      <Search class="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
      <Input
        v-model="searchTerm"
        type="search"
        placeholder="Search products by name, SKU, or barcode..."
        class="pl-10"
        @keyup.enter="handleSearch"
      />
    </div>
    <Button variant="outline" size="icon" @click="handleScan" title="Scan Barcode">
      <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
        <path d="M3 5v14" />
        <path d="M8 5v14" />
        <path d="M12 5v14" />
        <path d="M17 5v14" />
        <path d="M21 5v14" />
      </svg>
    </Button>
  </div>
</template>
