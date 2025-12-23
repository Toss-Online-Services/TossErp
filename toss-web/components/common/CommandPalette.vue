<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch, nextTick } from 'vue'
import { Search, Command } from 'lucide-vue-next'

interface CommandItem {
  id: string
  label: string
  icon?: string
  action: () => void
  keywords?: string[]
}

interface Props {
  open: boolean
  commands?: CommandItem[]
}

const props = withDefaults(defineProps<Props>(), {
  commands: () => []
})

const emit = defineEmits<{
  'update:open': [value: boolean]
}>()

const searchQuery = ref('')
const selectedIndex = ref(0)

const isOpen = computed({
  get: () => props.open,
  set: (value) => emit('update:open', value)
})

const filteredCommands = computed(() => {
  if (!searchQuery.value) return props.commands
  const query = searchQuery.value.toLowerCase()
  return props.commands.filter(cmd => {
    const matchLabel = cmd.label.toLowerCase().includes(query)
    const matchKeywords = cmd.keywords?.some(k => k.toLowerCase().includes(query))
    return matchLabel || matchKeywords
  })
})

const handleKeyDown = (e: KeyboardEvent) => {
  if (e.key === 'Escape') {
    isOpen.value = false
  } else if (e.key === 'ArrowDown') {
    e.preventDefault()
    selectedIndex.value = Math.min(selectedIndex.value + 1, filteredCommands.value.length - 1)
  } else if (e.key === 'ArrowUp') {
    e.preventDefault()
    selectedIndex.value = Math.max(selectedIndex.value - 1, 0)
  } else if (e.key === 'Enter' && filteredCommands.value[selectedIndex.value]) {
    e.preventDefault()
    filteredCommands.value[selectedIndex.value].action()
    isOpen.value = false
  }
}

const handleCommand = (cmd: CommandItem) => {
  cmd.action()
  isOpen.value = false
}

watch(isOpen, (open) => {
  if (open) {
    searchQuery.value = ''
    selectedIndex.value = 0
    nextTick(() => {
      const input = document.querySelector('[data-command-input]') as HTMLInputElement
      input?.focus()
    })
  }
})

onMounted(() => {
  document.addEventListener('keydown', handleKeyDown)
})

onUnmounted(() => {
  document.removeEventListener('keydown', handleKeyDown)
})
</script>

<template>
  <div
    v-if="isOpen"
    class="fixed inset-0 z-50 flex items-start justify-center pt-[20vh]"
    @click.self="isOpen = false"
  >
    <div class="w-full max-w-2xl mx-4 bg-card border border-border rounded-lg shadow-lg">
      <div class="flex items-center gap-3 px-4 py-3 border-b border-border">
        <Search :size="20" class="text-muted-foreground" />
        <input
          v-model="searchQuery"
          data-command-input
          type="text"
          placeholder="Type a command or search..."
          class="flex-1 bg-transparent border-none outline-none text-foreground placeholder:text-muted-foreground"
        />
        <kbd class="hidden sm:inline-flex items-center gap-1 px-2 py-1 text-xs font-semibold text-muted-foreground bg-muted rounded border border-border">
          <Command :size="12" />K
        </kbd>
      </div>
      <div class="max-h-96 overflow-y-auto">
        <div v-if="filteredCommands.length === 0" class="px-4 py-8 text-center text-sm text-muted-foreground">
          No commands found
        </div>
        <button
          v-for="(cmd, index) in filteredCommands"
          :key="cmd.id"
          @click="handleCommand(cmd)"
          :class="[
            'w-full px-4 py-3 text-left text-sm hover:bg-muted transition-colors flex items-center gap-3',
            index === selectedIndex ? 'bg-muted' : ''
          ]"
        >
          <span class="flex-1">{{ cmd.label }}</span>
        </button>
      </div>
    </div>
  </div>
</template>


